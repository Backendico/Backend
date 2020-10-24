using Backend2.Pages.Apis.UserAPI;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Backend2.Pages.Apis.Models.Leaderobard
{
    public class Leaderboard : BasicAPIs
    {
        public async Task<BsonDocument> ReciveLeaderboards(string Token, string Studio)
        {
            try
            {
                if (await CheackToken(Token))
                {
                    var Filter = new BsonDocument { { "_id", "Setting" } };
                    var options = new FindOptions<BsonDocument>();
                    options.Projection = new BsonDocument { { $"Leaderboard.History", 0 } };
                    var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").FindAsync(Filter).Result.SingleAsync();

                    foreach (var item in Result["Leaderboards"]["List"].AsBsonDocument)
                    {
                        var F = new BsonDocument { { $"Leaderboards.List.{item.Name}", new BsonDocument { { "$gte", long.MinValue } } } };

                        var Count = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").CountDocumentsAsync(F);

                        Result["Leaderboards"]["List"][item.Name].AsBsonDocument.Add("Count", Count);
                    }

                    return Result["Leaderboards"]["List"].ToBsonDocument();
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

        public async Task<bool> CreatLeaderBoard(string Token, string Studio, string DetailLeaderboard)
        {
            var deserilseDetail = BsonDocument.Parse(DetailLeaderboard);

            if (await CheackToken(Token) && await CheackLeaderboardName(Studio, deserilseDetail["Name"].ToString()) != true)
            {
                //init values from backend
                deserilseDetail.Add("Token", ObjectId.GenerateNewId());
                deserilseDetail.Add("Start", DateTime.Now);

                //inject
                var filter = new BsonDocument { { "_id", "Setting" } };
                var Update = new UpdateDefinitionBuilder<BsonDocument>().Set($"Leaderboards.List.{deserilseDetail["Name"]}", deserilseDetail);

                await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").FindOneAndUpdateAsync(filter, Update);

                return true;
            }
            else
            {
                return false;
            }
        }

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
                var option = new FindOptions<BsonDocument>();
                option.Projection = new BsonDocument { { "Leaderboards.Histor", 0 } };
                var Setting = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").FindAsync(filter, option).Result.SingleAsync();


              


                if (Setting["Leaderboards"]["List"].AsBsonDocument.TryGetElement(NameLeaderbaord, out _))
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


        public async Task<bool> EditLeaderBoard(string Token, string Studio, string DetailLeaderboard)
        {
            if (await CheackToken(Token))
            {
                var DeserilseData = BsonDocument.Parse(DetailLeaderboard);

                var filter = new BsonDocument { { "_id", "Setting" } };
                var update = new UpdateDefinitionBuilder<BsonDocument>().Set($"Leaderboards.List.{DeserilseData["Name"]}", DeserilseData);
                var result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").UpdateOneAsync(filter, update);

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
            try
            {
                int.Parse(Value);

                if (await CheackToken(Token))
                {
                    var Filter = new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) } };
                    var Update = new UpdateDefinitionBuilder<BsonDocument>().Set($"Leaderboards.List.{NameLeaderboard}", long.Parse(Value));
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
            catch (Exception)
            {

                return false;
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
                var Option = new FindOptions<BsonDocument>() { Projection = new BsonDocument { { "Leaderboards", 1 } ,{"_id",0 } } };

                return await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync(new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) } }, Option).Result.SingleAsync();
            }
            else
            {
                return new BsonDocument();
            }
        }


    }
}
