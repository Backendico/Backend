using Backend2.Pages.Apis.UserAPI;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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

            if (await CheackToken(Token) && !await CheackLeaderboardName(Studio,deserilseDetail["Name"].AsString))
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
                if (Leaderboards.ElementCount>=1)
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
               
                var FilterArray = new[] {new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("f.Settings.Token",new BsonDocument("$in",new BsonArray(new []{DeserilseData["Token"].AsObjectId }))))};

                var result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").UpdateOneAsync(filter, update, new UpdateOptions{ArrayFilters=FilterArray });

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


        public async Task<bool> Add(string Token, string Studio, string TokenPlayer, string NameLeaderboard, string Value)
        {

            if (await CheackToken(Token) && await CheackMinMax())
            {
                try
                {
                    var Score = long.Parse(Value);

                    var Options1 = new FindOptions<BsonDocument>()
                    {
                        Projection = new BsonDocument {
                        { $"Leaderboards.List.{NameLeaderboard}.Min", 1 } ,
                        { $"Leaderboards.List.{NameLeaderboard}.Max", 1 },
                        { $"Leaderboards.List.{NameLeaderboard}.Sort", 1 }
                    }
                    };
                    var SettingLeaderboard = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").FindAsync(new BsonDocument { { "_id", "Setting" } }, Options1).Result.SingleAsync();

                    switch (SettingLeaderboard["Leaderboards"]["List"][$"{NameLeaderboard}"]["Sort"].ToInt32())
                    {
                        case 0: //last value
                            SendValue(long.Parse(Value));
                            break;
                        case 1: //min value
                            {
                                var Filter = new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) } };
                                var Option = new FindOptions<BsonDocument>() { Projection = new BsonDocument { { "Leaderboards.List", 1 } } };
                                var Player = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync(Filter, Option).Result.SingleAsync();
                                try
                                {
                                    if (Score <= Player["Leaderboards"]["List"][$"{NameLeaderboard}"])
                                        SendValue(Score);
                                }
                                catch (Exception)
                                {
                                    SendValue(Score);
                                }

                            }
                            break;
                        case 2: //max value
                            {
                                var Filter = new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) } };
                                var Option = new FindOptions<BsonDocument>() { Projection = new BsonDocument { { "Leaderboards.List", 1 } } };
                                var Player = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync(Filter, Option).Result.SingleAsync();
                                try
                                {
                                    if (Score >= Player["Leaderboards"]["List"][$"{NameLeaderboard}"])
                                        SendValue(Score);
                                }
                                catch (Exception)
                                {
                                    SendValue(Score);
                                }
                            }
                            break;
                        case 3: //sum value
                            {
                                var Filter = new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) } };
                                var Option = new FindOptions<BsonDocument>() { Projection = new BsonDocument { { "Leaderboards.List", 1 } } };
                                var Player = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync(Filter, Option).Result.SingleAsync();

                                try
                                {
                                    SendValue(Score + Player["Leaderboards"]["List"][$"{NameLeaderboard}"].ToInt64());
                                }
                                catch (Exception)
                                {
                                    SendValue(Score);
                                }
                            }
                            break;
                        default:
                            Debug.WriteLine("not set");
                            break;
                    }

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


            //send to player
            async void SendValue(long value)
            {
                var Filter = new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) } };
                var Update = new UpdateDefinitionBuilder<BsonDocument>().Set($"Leaderboards.List.{NameLeaderboard}", value);
                await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").UpdateOneAsync(Filter, Update);
            }

            async Task<bool> CheackMinMax()
            {
                try
                {
                    long.Parse(Value);

                    var Options1 = new FindOptions<BsonDocument>()
                    {
                        Projection = new BsonDocument {
                        { $"Leaderboards.List.{NameLeaderboard}.Min", 1 } ,
                        { $"Leaderboards.List.{NameLeaderboard}.Max", 1 },
                        { $"Leaderboards.List.{NameLeaderboard}.Sort", 1 }
                    }
                    };
                    var SettingLeaderboard = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").FindAsync(new BsonDocument { { "_id", "Setting" } }, Options1).Result.SingleAsync();


                    if (long.Parse(Value) > SettingLeaderboard["Leaderboards"]["List"][$"{NameLeaderboard}"]["Max"] || long.Parse(Value) < SettingLeaderboard["Leaderboards"]["List"][$"{NameLeaderboard}"]["Min"])
                    {
                        return false;
                    }
                    else
                    {

                        return true;
                    }
                }
                catch (Exception)
                {
                    return false;
                }

            }
      
        }


        public async Task<bool> Remove(string Token, string Studio, string TokenPlayer, string NameLeaderboard)
        {
            if (await CheackToken(Token))
            {
                var Filter = new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) } };
                var Update = new UpdateDefinitionBuilder<BsonDocument>().Unset($"Leaderboards.List.{NameLeaderboard}");
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

        public async Task<bool> Reset(string Token, string Studio, string NameLeaderboard)
        {
            if (await CheackToken(Token))
            {
                //remove all value in user
                var Update = new UpdateDefinitionBuilder<BsonDocument>().Unset($"Leaderboards.List.{NameLeaderboard}");
                await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").UpdateManyAsync("{}", Update);


                //reset start in setting
                var filter = new BsonDocument { { "_id", "Setting" } };
                var Update1 = new UpdateDefinitionBuilder<BsonDocument>().Set($"Leaderboards.List.{NameLeaderboard}.Start", DateTime.Now);

                var result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").UpdateOneAsync(filter, Update1);


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


        public async Task<bool> Backup(string Token, string Studio, string NameLeaderboard)
        {
            try
            {
                if (await CheackToken(Token))
                {
                    //find all players in leaderboard
                    var filter = new BsonDocument { { $"Leaderboards.List.{NameLeaderboard}", new BsonDocument { { "$gt", long.MinValue } } } };
                    var option = new FindOptions<BsonDocument>();
                    option.Projection = new BsonDocument { { $"Leaderboards.List.{NameLeaderboard}", 1 }, { "Account.Token", 1 }, { "_id", 0 } };
                    option.Sort = new BsonDocument { { $"Leaderboards.List.{NameLeaderboard}", -1 } };
                    var Finder = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync(filter, option).Result.ToListAsync();

                    var Players = new BsonDocument { };
                    var Rank = 0;

                    foreach (var item in Finder)
                    {
                        Players.Add(Rank.ToString(), new BsonDocument { { "Rank", Rank }, { "Token", item["Account"]["Token"] }, { "Value", item["Leaderboards"]["List"][NameLeaderboard] } });
                        Rank++;
                    }

                    var Result = new BsonDocument();

                    Result.Add("List", Players);

                    //find start and end
                    var Setting = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").FindAsync(new BsonDocument { { "_id", "Setting" } }).Result.SingleAsync();
                    Result.Add("Detail", new BsonDocument
                    {
                        {"Start",Setting["Leaderboards"]["List"][NameLeaderboard]["Start"] },
                        {"End",DateTime.Now }
                    });

                    //inject to database
                    var update1 = new UpdateDefinitionBuilder<BsonDocument>().Set($"Leaderboards.List.{NameLeaderboard}.Backups.{new Random().Next()}", Result.AsBsonDocument).Set($"Leaderboards.List.{NameLeaderboard}.Start", DateTime.Now);

                    var FinalResult = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").UpdateOneAsync(new BsonDocument { { "_id", "Setting" } }, update1);



                    if (FinalResult.ModifiedCount >= 1)
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
            catch (Exception)
            {
                return false;
            }
        }


        public async Task<BsonDocument> ReciveBackup(string Token, string Studio, string NameLeaderboard)
        {
            try
            {
                if (await CheackToken(Token))
                {
                    var filter = new BsonDocument { { "_id", "Setting" } };
                    var Setting = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").FindAsync(filter).Result.SingleAsync();

                    var Result = new BsonDocument { };


                    foreach (var item in Setting["Leaderboards"]["List"][NameLeaderboard]["Backups"].AsBsonDocument)
                    {
                        item.Value["Detail"].AsBsonDocument.Add("Name", item.Name);
                        Result.Add(item.Name, item.Value["Detail"]);
                    }




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


        public async Task<bool> RemoveBackup(string Token, string Studio, string NameLeaderboard, string Version)
        {
            if (await CheackToken(Token))
            {
                var Filter = new BsonDocument { { "_id", "Setting" } };
                var Update = new UpdateDefinitionBuilder<BsonDocument>().Unset($"Leaderboards.List.{NameLeaderboard}.Backups.{Version}");
                var result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").UpdateOneAsync(Filter, Update);


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
