using Backend2.Pages.Apis.UserAPI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Driver.Core.Operations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;

namespace Backend2.Pages.Apis.Models.Leaderobard
{
    public class Leaderboard : BasicAPIs
    {
        //pass
        public async Task<BsonDocument> ReciveLeaderboards(string Token, string Studio)
        {
            try
            {
                if (await CheackToken(Token))
                {
                    var Pipe = new[]
                    {
                        new BsonDocument { {"$unwind" ,"$Leaderboards" } },
                        new BsonDocument{{"$group",new BsonDocument { {"_id","Leaderboard List " },{"List",new BsonDocument { {"$push", "$Leaderboards"} } } } }}
                    };
                    var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").AggregateAsync<BsonDocument>(Pipe).Result.SingleAsync<BsonDocument>();


                    return Result;
                }
                else
                {
                    return new BsonDocument();
                }

            }
            catch (Exception)
            {

                return new BsonDocument();
            }
        }

        //pass
        public async Task<bool> CreatLeaderBoard(string Token, string Studio, string DetailLeaderboard)
        {
            var deserilseDetail = BsonDocument.Parse(DetailLeaderboard);

            if (await CheackToken(Token) && !await CheackLeaderboardName(Studio, deserilseDetail["Name"].AsString))
            {
                //init values from backend
                deserilseDetail.Add("Token", ObjectId.GenerateNewId());
                deserilseDetail.Add("Start", DateTime.Now);

                var Newleaderboard = new BsonDocument { { "Settings", deserilseDetail }, { "Backups", new BsonArray() } };

                //inject
                var filter = new BsonDocument { { "_id", "Setting" } };
                var Update = new UpdateDefinitionBuilder<BsonDocument>().Push($"Leaderboards", Newleaderboard);

                await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").FindOneAndUpdateAsync(filter, Update);

                return true;
            }
            else
            {
                return false;
            }
        }

        //pass
        /// <summary>
        /// Cheack Leaderboard Name
        /// </summary>
        /// <param name="Studio"></param>
        /// <param name="NameLeaderbaord"></param>
        /// <returns>
        /// <list type="number">
        /// <item>
        /// If Find Return True
        /// 
        /// </item>
        /// <item>
        /// if not find return false    
        /// </item>
        /// </list>
        /// </returns>
        public async Task<bool> CheackLeaderboardName(string Studio, string NameLeaderbaord)
        {
            try
            {
                var filter = new BsonDocument { { "_id", "Setting" } };

                var Pipe = new[]
                {
                    new BsonDocument{ {"$unwind","$Leaderboards" } },
                    new BsonDocument{{"$match",new BsonDocument { { "Leaderboards.Settings.Name",NameLeaderbaord} } }},
                    new BsonDocument{{"$limit",1}}
                };

                var Leaderboards = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").AggregateAsync<BsonDocument>(Pipe).Result.SingleAsync();
                if (Leaderboards.ElementCount >= 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception)
            {
                return false;
            }
        }

        //pass
        public async Task<bool> EditLeaderBoard(string Token, string Studio, string DetailLeaderboard)
        {
            if (await CheackToken(Token))
            {
                var DeserilseData = BsonDocument.Parse(DetailLeaderboard);

                var filter = new BsonDocument { { "_id", "Setting" } };
                var update = new UpdateDefinitionBuilder<BsonDocument>().Set("Leaderboards.$[f].Settings", DeserilseData);

                var FilterArray = new[] { new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("f.Settings.Token", new BsonDocument("$in", new BsonArray(new[] { DeserilseData["Token"].AsObjectId })))) };

                var result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").UpdateOneAsync(filter, update, new UpdateOptions { ArrayFilters = FilterArray });

                if (result.ModifiedCount >= 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }


        public async Task<string> LeaderboardDetail(string Token, string Studio, string NameLeaderboard, string Count)
        {
            try
            {
                if (await CheackToken(Token))
                {
                    var filter = new BsonDocument { { $"Leaderboards.List.{NameLeaderboard}", new BsonDocument { { "$gt", long.MinValue } } } };
                    var option = new FindOptions<BsonDocument>();
                    option.Projection = new BsonDocument { { $"Leaderboards.List.{NameLeaderboard}", 1 }, { "Account.Token", 1 }, { "_id", 0 } };
                    option.Sort = new BsonDocument { { $"Leaderboards.List.{NameLeaderboard}", -1 } };
                    option.Limit = int.Parse(Count);
                    var Finder = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync(filter, option).Result.ToListAsync();

                    var result = new BsonDocument { };
                    var Rank = 0;



                    foreach (var item in Finder)
                    {
                        result.Add(Rank.ToString(), new BsonDocument { { "Rank", Rank }, { "Token", item["Account"]["Token"] }, { "Value", item["Leaderboards"]["List"][NameLeaderboard] } });
                        Rank++;
                    }



                    return result.ToString();

                }
                else
                {
                    return "";
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        //pass
        public async Task<bool> Add(string Token, string Studio, string TokenPlayer, string NameLeaderboard, string Value)
        {
            try
            {
                var Score = Int64.Parse(Value);
                var DetailScore = new BsonDocument
                {
                    {"Leaderboard",NameLeaderboard },
                    {"Score",Score }
                };

                if (await CheackToken(Token) && await CheackMinMax())
                {
                    var Setting = await SettingLeaderboard();

                    if (await CheackLeaderboardinplayer())
                    {
                        switch (Setting["Leaderboards"]["Settings"]["Sort"].ToInt64())
                        {
                            case 0:
                                {
                                    return await UpdateScore();
                                }
                            case 1:
                                {
                                    var Pipe = new[]
                                    {
                                        new BsonDocument{ {"$unwind","$Leaderboards" } },
                                        new BsonDocument{{"$match",new BsonDocument { {"Leaderboards.Leaderboard",NameLeaderboard } } }},
                                         new BsonDocument{{"$limit",1}}
                                    };

                                    var PlayerLeaderboard = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").AggregateAsync<BsonDocument>(Pipe).Result.SingleAsync();

                                    if (PlayerLeaderboard["Leaderboards"]["Score"].ToInt64() >= Score)
                                    {
                                        return await UpdateScore();
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                            case 2:
                                {
                                    var Pipe = new[]
                                   {
                                        new BsonDocument{ {"$unwind","$Leaderboards" } },
                                        new BsonDocument{{"$match",new BsonDocument { {"Leaderboards.Leaderboard",NameLeaderboard } } }},
                                        new BsonDocument{{"$limit",1}}
                                    };

                                    var PlayerLeaderboard = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").AggregateAsync<BsonDocument>(Pipe).Result.SingleAsync();

                                    if (PlayerLeaderboard["Leaderboards"]["Score"].ToInt64() <= Score)
                                    {
                                        return await UpdateScore();
                                    }
                                    else
                                    {
                                        return false;
                                    }

                                }
                            case 3:
                                {
                                    var Pipe = new[]
                                 {
                                        new BsonDocument{ {"$unwind","$Leaderboards" } },
                                        new BsonDocument{{"$match",new BsonDocument { {"Leaderboards.Leaderboard",NameLeaderboard } } }},
                                        new BsonDocument{{"$limit",1}}
                                    };

                                    var PlayerLeaderboard = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").AggregateAsync<BsonDocument>(Pipe).Result.SingleAsync();

                                    Score += PlayerLeaderboard["Leaderboards"]["Score"].ToInt64();
                                    return await UpdateScore();
                                }
                            default:
                                {
                                    return false;
                                }
                        }

                    }
                    else
                    {
                        return await SendNewValue();
                    }
                }
                else
                {
                    return false;
                }


                //send to player
                async Task<bool> SendNewValue()
                {
                    var Filter = new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) } };
                    var Update = new UpdateDefinitionBuilder<BsonDocument>().Push($"Leaderboards", DetailScore);
                    var result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").UpdateOneAsync(Filter, Update);

                    if (result.ModifiedCount >= 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }



                async Task<bool> UpdateScore()
                {
                    var Filter = new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) } };

                    var Update = Builders<BsonDocument>.Update.Set("Leaderboards.$[f].Score", Score);

                    var FilterArray = new[] { new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("f.Leaderboard", new BsonDocument("$in", new BsonArray(new[] { NameLeaderboard })))) };

                    var result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").UpdateOneAsync(Filter, Update, new UpdateOptions() { ArrayFilters = FilterArray });

                    if (result.ModifiedCount >= 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                };



                //cheack Leaderboard in Player
                async Task<bool> CheackLeaderboardinplayer()
                {
                    try
                    {
                        var Pipe = new[]
                        {
                            new BsonDocument{ {"$match",new BsonDocument { {"Account.Token",ObjectId.Parse(TokenPlayer) } } } },
                            new BsonDocument{ {"$unwind","$Leaderboards" } },
                            new BsonDocument{{"$match",new BsonDocument { {"Leaderboards.Leaderboard",NameLeaderboard } } }},
                        };

                        var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").AggregateAsync<BsonDocument>(Pipe).Result.SingleAsync();


                        return Result.ElementCount >= 1 ? true : false;

                    }
                    catch (Exception)
                    {
                        return false;
                    }

                }

                //cheack min max setting leaderboard
                async Task<bool> CheackMinMax()
                {
                    if (await CheackLeaderboardName(Studio, NameLeaderboard))
                    {

                        var Pipe = new[] {
                        new BsonDocument{ {"$unwind","$Leaderboards" } },
                        new BsonDocument{{"$match",new BsonDocument { {"Leaderboards.Settings.Name",NameLeaderboard } } }},
                        new BsonDocument{{"$project",new BsonDocument { {"Leaderboards.Backups",0 } } }},
                          new BsonDocument{{"$project",new BsonDocument { {"Leaderboards",1 } } }}
                    };

                        var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").AggregateAsync<BsonDocument>(Pipe).Result.SingleAsync();

                        if (Result.ElementCount >= 1 && Score <= Result["Leaderboards"]["Settings"]["Max"].ToInt64() && Score >= Result["Leaderboards"]["Settings"]["Min"].ToInt64())
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }

                    }
                    else
                    {
                        return false;
                    }
                }

                async Task<BsonDocument> SettingLeaderboard()
                {
                    var Pipe = new[]
                    {
                        new BsonDocument{ { "$unwind", "$Leaderboards" } },
                        new BsonDocument{{"$match",new BsonDocument { {"Leaderboards.Settings.Name",NameLeaderboard } } }},
                        new BsonDocument{{"$project",new BsonDocument { {"Leaderboards.Settings",1 } } }}
                    };

                    return await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").AggregateAsync<BsonDocument>(Pipe).Result.SingleAsync();

                }

            }
            catch (Exception)
            {

                return false;
            }


            {
                //try
                //{
                //    var Score = long.Parse(Value);

                //    var Options1 = new FindOptions<BsonDocument>()
                //    {
                //        Projection = new BsonDocument {
                //        { $"Leaderboards.List.{NameLeaderboard}.Min", 1 } ,
                //        { $"Leaderboards.List.{NameLeaderboard}.Max", 1 },
                //        { $"Leaderboards.List.{NameLeaderboard}.Sort", 1 }
                //    }
                //    };
                //    var SettingLeaderboard = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").FindAsync(new BsonDocument { { "_id", "Setting" } }, Options1).Result.SingleAsync();

                //    switch (SettingLeaderboard["Leaderboards"]["List"][$"{NameLeaderboard}"]["Sort"].ToInt32())
                //    {
                //        case 0: //last value
                //            SendValue(long.Parse(Value));
                //            break;
                //        case 1: //min value
                //            {
                //                var Filter = new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) } };
                //                var Option = new FindOptions<BsonDocument>() { Projection = new BsonDocument { { "Leaderboards.List", 1 } } };
                //                var Player = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync(Filter, Option).Result.SingleAsync();
                //                try
                //                {
                //                    if (Score <= Player["Leaderboards"]["List"][$"{NameLeaderboard}"])
                //                        SendValue(Score);
                //                }
                //                catch (Exception)
                //                {
                //                    SendValue(Score);
                //                }

                //            }
                //            break;
                //        case 2: //max value
                //            {
                //                var Filter = new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) } };
                //                var Option = new FindOptions<BsonDocument>() { Projection = new BsonDocument { { "Leaderboards.List", 1 } } };
                //                var Player = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync(Filter, Option).Result.SingleAsync();
                //                try
                //                {
                //                    if (Score >= Player["Leaderboards"]["List"][$"{NameLeaderboard}"])
                //                        SendValue(Score);
                //                }
                //                catch (Exception)
                //                {
                //                    SendValue(Score);
                //                }
                //            }
                //            break;
                //        case 3: //sum value
                //            {
                //                var Filter = new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) } };
                //                var Option = new FindOptions<BsonDocument>() { Projection = new BsonDocument { { "Leaderboards.List", 1 } } };
                //                var Player = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync(Filter, Option).Result.SingleAsync();

                //                try
                //                {
                //                    SendValue(Score + Player["Leaderboards"]["List"][$"{NameLeaderboard}"].ToInt64());
                //                }
                //                catch (Exception)
                //                {
                //                    SendValue(Score);
                //                }
                //            }
                //            break;
                //        default:
                //            Debug.WriteLine("not set");
                //            break;
                //    }

                //    return true;


                //}
                //catch (Exception)
                //{

                //    return false;
                //}

            }

        }

        //pass
        public async Task<bool> Remove(string Token, string Studio, string TokenPlayer, string NameLeaderboard)
        {
            if (await CheackToken(Token))
            {
                var Filter = new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) } };

                var Update = Builders<BsonDocument>.Update.Pull("Leaderboards", new BsonDocument { { "Leaderboard", NameLeaderboard } });

                var result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").UpdateOneAsync(Filter, Update);

                if (result.ModifiedCount >= 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        //Pass
        public async Task<bool> Reset(string Token, string Studio, string NameLeaderboard)
        {
            if (await CheackToken(Token))
            {
                try
                {
                    //remove all value in user
                    var Update = Builders<BsonDocument>.Update.Pull("Leaderboards", new BsonDocument { { "Leaderboard", NameLeaderboard } });

                    await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").UpdateManyAsync("{}", Update);

                    //reset start in setting
                    var filter = new BsonDocument { { "_id", "Setting" } };
                    var Update1 = new UpdateDefinitionBuilder<BsonDocument>().Set($"Leaderboards.$[f].Settings.Start", DateTime.Now);

                    var FilterArray = new[] { new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("f.Settings.Name", new BsonDocument("$in", new BsonArray(new[] { NameLeaderboard })))) };

                    await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").UpdateOneAsync(filter, Update1, new UpdateOptions() { ArrayFilters = FilterArray });

                    return true;

                }
                catch (Exception)
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
        }


        //pass
        public async Task<bool> Backup(string Token, string Studio, string NameLeaderboard)
        {
            try
            {
                if (await CheackToken(Token))
                {

                    //find all player score
                    var Pipe = new[]
                    {
                        new BsonDocument{ {"$unwind","$Leaderboards" } },
                        new BsonDocument{ {"$match",new BsonDocument("Leaderboards.Leaderboard",NameLeaderboard) } },
                        new BsonDocument{ {"$project",new BsonDocument { {"_id",0 },{"Username","$Account.Username" },{"Token","$Account.Token" },{"Score","$Leaderboards.Score" } } } }
                    };

                    var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").AggregateAsync<BsonDocument>(Pipe).Result.ToListAsync();

                    var FinalResult = new BsonDocument();
                    FinalResult.Add("List", new BsonArray());

                    Result.ForEach(T =>
                    {
                        FinalResult["List"].AsBsonArray.Add(T);
                    });


                    //find Time Leaderboards

                    FinalResult.Add("Settings", new BsonDocument {
                        {"End",DateTime.Now },
                        {"Start","" },
                        {"Token",ObjectId.GenerateNewId() }
                    });

                    var Pipe2 = new[]{
                    new BsonDocument{ {"$unwind","$Leaderboards" } },
                    new BsonDocument{ {"$match",new BsonDocument { {"Leaderboards.Settings.Name",NameLeaderboard } } } },
                    new BsonDocument{ {"$project" ,new BsonDocument { {"Leaderboards",1 } } } }
                    };

                    var Time = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").AggregateAsync<BsonDocument>(Pipe2).Result.SingleAsync();

                    FinalResult["Settings"]["Start"] = Time["Leaderboards"]["Settings"]["Start"];


                    //reset time Leaderboard


                    var filter2 = new BsonDocument { { "_id", "Setting" } };
                    var Update2 = new UpdateDefinitionBuilder<BsonDocument>().Set($"Leaderboards.$[f].Settings.Start", DateTime.Now);

                    var FilterArray2 = new[] { new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("f.Settings.Name", new BsonDocument("$in", new BsonArray(new[] { NameLeaderboard })))) };

                    await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").UpdateOneAsync(filter2, Update2, new UpdateOptions() { ArrayFilters = FilterArray2 });



                    //inject to list
                    var filter = new BsonDocument { { "_id", "Setting" } };
                    var Update1 = new UpdateDefinitionBuilder<BsonDocument>().Push($"Leaderboards.$[f].Backups", FinalResult);

                    var FilterArray = new[] { new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("f.Settings.Name", new BsonDocument("$in", new BsonArray(new[] { NameLeaderboard })))) };

                    var ResultAdd = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").UpdateOneAsync(filter, Update1, new UpdateOptions() { ArrayFilters = FilterArray });

                    return ResultAdd.ModifiedCount >= 1 ? true : false;

                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }



        }


        //pass
        public async Task<BsonDocument> ReciveBackup(string Token, string Studio, string NameLeaderboard, int Count)
        {
            try
            {
                if (await CheackToken(Token))
                {
                    Count = Count >= 100 ? Count : 100;

                    var Pipe = new[]
                    {
                        new BsonDocument{ {"$unwind","$Leaderboards" } },
                        new BsonDocument{ {"$match" ,new BsonDocument("Leaderboards.Settings.Name",NameLeaderboard )} },
                        new BsonDocument{{"$project",new BsonDocument { {"Leaderboards",new BsonDocument { {"$slice",new BsonArray() { "$Leaderboards.Backups",Count } } } } } }}

                    };

                    var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").AggregateAsync<BsonDocument>(Pipe).Result.SingleAsync();
                    return Result;
                }
                else
                {
                    return new BsonDocument();
                }

            }
            catch (Exception)
            {
                return new BsonDocument();
            }
        }

        //pass
        public async Task<bool> RemoveBackup(string Token, string Studio, ObjectId TokenBackups)
        {
            try
            {

                if (await CheackToken(Token))
                {
                    var Update = Builders<BsonDocument>.Update.Pull("Leaderboards.$[].Backups", new BsonDocument { { "Settings.Token", TokenBackups } });

               
                    var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").UpdateOneAsync(new BsonDocument { { "_id", "Setting" } }, Update);


                    if (Result.ModifiedCount >= 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;

            }

        }


        public async Task<BsonDocument> DownloadBackup(string Token, string Studio, string NameLeaderboard, string Version)
        {
            if (await CheackToken(Token))
            {
                var Filter = new BsonDocument { { "_id", "Setting" } };
                var option = new FindOptions<BsonDocument>();
                option.Projection = new BsonDocument { { "_id", 0 }, { $"Leaderboards.List.{NameLeaderboard}.Backups.{Version}.List", 1 } };
                var result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").FindAsync(Filter, option).Result.SingleAsync();


                if (result["Leaderboards"]["List"][NameLeaderboard]["Backups"][Version]["List"].AsBsonDocument.ElementCount >= 1)
                {
                    return result["Leaderboards"]["List"][NameLeaderboard]["Backups"][Version]["List"].AsBsonDocument;
                }
                else
                {
                    return new BsonDocument();
                }
            }
            else
            {
                return new BsonDocument();
            }

        }


        public async Task<BsonDocument> SettingLeaderboard(string Token, string Studio, string NameLeaderboard)
        {
            if (await CheackToken(Token))
            {
                var Pipe = new[]
                {
                    new BsonDocument{ {"$project",new BsonDocument { {"Leaderboards",$"$Leaderboards.List.{NameLeaderboard}" } } } },
                    new BsonDocument{{"$project",new BsonDocument { {"_id",0 },{ "Leaderboards.Backups", 0 } } }}
                };



                return await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").AggregateAsync<BsonDocument>(Pipe).Result.SingleAsync();
            }
            else
            {
                return new BsonDocument();
            }
        }


        public async Task<BsonDocument> RecivePlayerLeaderboard(string Token, string Studio, string TokenPlayer)
        {
            if (await CheackToken(Token))
            {
                var Option = new FindOptions<BsonDocument>() { Projection = new BsonDocument { { "Leaderboards", 1 }, { "_id", 0 } } };

                return await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync(new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) } }, Option).Result.SingleAsync();

            }
            else
            {
                return new BsonDocument();
            }
        }


    }
}
