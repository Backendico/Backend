using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend2.Pages.Apis.Models.Dashboard
{
    public class Dashboard : BasicAPIs
    {
        public async Task<BsonDocument> ReciveDetail(string Token, string Studio)
        {
            var Result = new BsonDocument
                    {
                        {"Players" ,new BsonDocument{
                            {"24Hours",0 },
                            {"1Days",0 },
                            {"7Days",0 },
                            {"30Days",0 }
                        } },
                        {"Logins",new BsonDocument{
                          {"24Hours",0 },
                            {"1Days",0 },
                            {"7Days",0 },
                            {"30Days",0 }
                        } },
                        {"Leaderboards",new BsonDocument{ {"Totall",0 },{"Count" ,0} } },
                        {"PlayersMonetiz",new BsonDocument{ {"Totall",0 }, {"Count",0 } } },
                        {"Logs",new BsonDocument{ {"Count",0 },{"Totall",0 } } },
                        {"APIs",new BsonDocument{ {"Count",0 },{"Totall",0 } }},
                        {"Achievements",new BsonDocument{ {"Count",0 },{"Totall",0 } }},
                        {"Store",new BsonDocument{ {"Count",0 },{"Totall",0 } }} 
            };

            try
            {
                if (await CheackToken(Token))
                {
                    //player_24hours
                    try
                    {
                        var Pipe = new[] {
                        new BsonDocument{ {"$project",new BsonDocument { { "Online",new BsonDocument { {"$gte", new BsonArray() { "$Account.Created", DateTime.Now.AddHours(-24) } } } } } } },
                        new BsonDocument{{"$match",new BsonDocument { {"Online",true } } }}
                        };

                        var Player_24Hours = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").AggregateAsync<BsonDocument>(Pipe).Result.ToListAsync();

                        Result["Players"]["24Hours"] = Player_24Hours.Count;
                    }
                    catch (Exception)
                    {
                    }

                    //player_1Day
                    try
                    {
                        var Pipe = new[] {
                        new BsonDocument{ {"$project",new BsonDocument { { "Online",new BsonDocument { {"$gte", new BsonArray() { "$Account.Created", DateTime.Now.AddDays(-1) } } } } } } },
                        new BsonDocument{{"$match",new BsonDocument { {"Online",true } } }}
                        };

                        var Player_24Hours = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").AggregateAsync<BsonDocument>(Pipe).Result.ToListAsync();

                        Result["Players"]["1Days"] = Player_24Hours.Count;
                    }
                    catch (Exception)
                    {
                    }


                    //player_7day
                    try
                    {
                        var Pipe = new[] {
                        new BsonDocument{ {"$project",new BsonDocument { { "Online",new BsonDocument { {"$gte", new BsonArray() { "$Account.Created", DateTime.Now.AddDays(-7) } } } } } } },
                        new BsonDocument{{"$match",new BsonDocument { {"Online",true } } }}
                        };

                        var Player_24Hours = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").AggregateAsync<BsonDocument>(Pipe).Result.ToListAsync();

                        Result["Players"]["7Days"] = Player_24Hours.Count;
                    }
                    catch (Exception)
                    {
                    }

                    //player_30Day
                    try
                    {
                        var Pipe = new[] {
                        new BsonDocument{ {"$project",new BsonDocument { { "Online",new BsonDocument { {"$gte", new BsonArray() { "$Account.Created", DateTime.Now.AddMonths(-1) } } } } } } },
                        new BsonDocument{{"$match",new BsonDocument { {"Online",true } } }}
                        };

                        var Player_24Hours = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").AggregateAsync<BsonDocument>(Pipe).Result.ToListAsync();

                        Result["Players"]["30Days"] = Player_24Hours.Count;
                    }
                    catch (Exception)
                    {
                    }

                    //Login_24Hours
                    try
                    {

                        var Pipe = new[] {
                        new BsonDocument{ {"$project",new BsonDocument { { "Online",new BsonDocument { {"$gte", new BsonArray() { "$Account.LastLogin", DateTime.Now.AddHours(-24) } } } } } } },
                        new BsonDocument{{"$match",new BsonDocument { {"Online",true } } }}
                        };

                        var Player_24Hours = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").AggregateAsync<BsonDocument>(Pipe).Result.ToListAsync();

                        Result["Logins"]["24Hours"] = Player_24Hours.Count;
                    }
                    catch (Exception)
                    {
                    }


                    //Login_1Day
                    try
                    {
                        var Pipe = new[] {
                        new BsonDocument{ {"$project",new BsonDocument { { "Online",new BsonDocument { {"$gte", new BsonArray() { "$Account.LastLogin", DateTime.Now.AddDays(-1) } } } } } } },
                        new BsonDocument{{"$match",new BsonDocument { {"Online",true } } }}
                        };

                        var Player_24Hours = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").AggregateAsync<BsonDocument>(Pipe).Result.ToListAsync();

                        Result["Logins"]["1Days"] = Player_24Hours.Count;
                    }
                    catch (Exception)
                    {
                    }

                    //Login_7Day
                    try
                    {

                        var Pipe = new[] {
                        new BsonDocument{ {"$project",new BsonDocument { { "Online",new BsonDocument { {"$gte", new BsonArray() { "$Account.LastLogin", DateTime.Now.AddDays(-7) } } } } } } },
                        new BsonDocument{{"$match",new BsonDocument { {"Online",true } } }}
                        };

                        var Player_24Hours = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").AggregateAsync<BsonDocument>(Pipe).Result.ToListAsync();

                        Result["Logins"]["7Days"] = Player_24Hours.Count;
                    }
                    catch (Exception)
                    {
                    }


                    //Login_30Day
                    try
                    {
                        var Pipe = new[] {
                        new BsonDocument{ {"$project",new BsonDocument { { "Online",new BsonDocument { {"$gte", new BsonArray() { "$Account.LastLogin", DateTime.Now.AddMonths(-1) } } } } } } },
                        new BsonDocument{{"$match",new BsonDocument { {"Online",true } } }}
                        };

                        var Player_24Hours = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").AggregateAsync<BsonDocument>(Pipe).Result.ToListAsync();

                        Result["Logins"]["30Days"] = Player_24Hours.Count;
                    }
                    catch (Exception)
                    {
                    }


                    //LeaderboardCount
                    //LeaderboardTotall
                    //playerTotall
                    //LogCount
                    //logTotall
                    //API Count
                    //API Totall
                    //Achievement Count
                    //Achievement Totall
                    try
                    {
                        var Pipe = new[] {
                        new BsonDocument{ {"$project" ,new BsonDocument
                        {
                            { "LeaderboardCount", new BsonDocument {{ "$size", "$Leaderboards" } } },
                            {"LeaderboardsTotall","$Monetiz.Leaderboards" },

                            {"PlayerTotall","$Monetiz.Players" },

                            { "LogCount", new BsonDocument {{ "$size", "$Logs" } } },
                            {"LogTotall","$Monetiz.Logs" },

                            {"APICount","$Monetiz.Apis" },
                            {"APITotall",new BsonDocument{ {"$sum",new BsonArray() {"$APIs.Read","$APIs.Write" } } } },

                            { "AchievementsCount", new BsonDocument {{ "$size", "$Achievements" } } },
                            {"AchievemenetsTotall", "$Monetiz.Achievements" },

                            { "StoreCount", new BsonDocument {{ "$size", "$Store" } } },
                               {"StoreTotall", "$Monetiz.Store" },

                        } } },
                        };
                        var Details = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").AggregateAsync<BsonDocument>(Pipe).Result.SingleAsync();

                        Result["Leaderboards"]["Count"] = Details["LeaderboardCount"];
                        Result["Leaderboards"]["Totall"] = Details["LeaderboardsTotall"];

                        Result["PlayersMonetiz"]["Totall"] = Details["PlayerTotall"];

                        Result["Logs"]["Count"] = Details["LogCount"];
                        Result["Logs"]["Totall"] = Details["LogTotall"];

                        Result["APIs"]["Count"] = Details["APICount"];
                        Result["APIs"]["Totall"] = Details["APITotall"];

                        Result["Achievements"]["Count"] = Details["AchievementsCount"];
                        Result["Achievements"]["Totall"] = Details["AchievemenetsTotall"];

                        Result["Store"]["Count"] = Details["StoreCount"];
                        Result["Store"]["Totall"] = Details["StoreTotall"];
                    }
                    catch (Exception)
                    {
                    }


                    //PlayerCount
                    try
                    {
                        var Pipe = new[]
                        {
                            new BsonDocument{ {"$count" ,"Account"} }
                        };

                        var Count_Player = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").AggregateAsync<BsonDocument>(Pipe).Result.SingleAsync();

                        Result["PlayersMonetiz"]["Count"] = Count_Player["Account"];
                    }
                    catch (Exception) { };



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

        public async Task<BsonDocument> Notifaction(string Token, string Studio)
        {
            BsonDocument Result = new BsonDocument
            {
                {"Logs",0 },
                {"Support",0 }
            };

            try
            {
                if (await CheackToken(Token))
                {

                    //Logs Notifactions
                    {
                        try
                        {
                            var Pipe = new[]
                            {
                                new BsonDocument{ {"$project",new BsonDocument { {"Logs",1 } } } },
                                new BsonDocument{ {"$unwind","$Logs" } },
                                new BsonDocument{{"$match",new BsonDocument { {"Logs.IsNotifaction",true } } }},
                                new BsonDocument{{"$group",new BsonDocument { {"_id","_id" },{"Logs",new BsonDocument { {"$push","$Logs" } } } } }},
                                new BsonDocument{{"$project",new BsonDocument { {"_id",0 } ,{"Logs",new BsonDocument { {"$size", "$Logs" } } } } }}
                            };

                            var CountLogs = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").AggregateAsync<BsonDocument>(Pipe).Result.SingleAsync();

                            Result["Logs"] = CountLogs["Logs"];

                        }
                        catch (Exception)
                        {
                        }
                    }

                    //Support
                    {
                        try
                        {
                            var Pipe1 = new[]
                            {
                            new BsonDocument{ {"$project",new BsonDocument { {"Support",1 } } } },
                            new BsonDocument{ {"$unwind","$Support" } },
                             new BsonDocument{{"$match",new BsonDocument { {"Support.IsOpen",true} } }},
                            new BsonDocument{ {"$project",new BsonDocument { {"Support",new BsonDocument { { "$arrayElemAt", new BsonArray { { "$Support.Messages" }, { -1 } } } } }  } } },
                            new BsonDocument{{"$match",new BsonDocument { {"Support.Sender",1} } }},
                            };

                            var CountSupport = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").AggregateAsync<BsonDocument>(Pipe1).Result.ToListAsync();

                            Result["Support"] = CountSupport.Count;
                        }
                        catch (Exception)
                        {
                        }
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


        public bool CheackStatusServer()
        {
            return true;
        }


        public async Task<BsonDocument> CheackUpdate()
        {
            try
            {

                var Option = new FindOptions<BsonDocument>() { Projection = new BsonDocument { { "Part", 0 }, { "_id", 0 } } };

                return await Client.GetDatabase("Users").GetCollection<BsonDocument>("Setting").FindAsync(new BsonDocument { { "Part", "Setting" } }, Option).Result.SingleAsync();
            }
            catch (Exception)
            {
                return new BsonDocument();
            }

        }
    }
}
