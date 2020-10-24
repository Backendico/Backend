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
                    };

            try
            {
                if (await CheackToken(Token))
                {
                    //player_24hours
                    try
                    {
                        var Filter1 = new BsonDocument { { "Account.Created", new BsonDocument { { "$gte", DateTime.Now.AddHours(-24) } } } };
                        var Player_24Hours = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync(Filter1).Result.ToListAsync();
                        Result["Players"]["24Hours"] = Player_24Hours.Count;
                    }
                    catch (Exception)
                    {
                    }

                    //player_1Day
                    try
                    {
                        var Filter2 = new BsonDocument { { "Account.Created", new BsonDocument { { "$gte", DateTime.Now.AddDays(-1) } } } };
                        var Player_1day = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync(Filter2).Result.ToListAsync();
                        Result["Players"]["1Days"] = Player_1day.Count;
                    }
                    catch (Exception)
                    {
                    }


                    //player_7Day
                    try
                    {
                        var Filter3 = new BsonDocument { { "Account.Created", new BsonDocument { { "$gte", DateTime.Now.AddDays(-7) } } } };
                        var Player_7day = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync(Filter3).Result.ToListAsync();
                        Result["Players"]["7Days"] = Player_7day.Count;
                    }
                    catch (Exception)
                    {
                    }

                    //player_30Day
                    try
                    {
                        var Filter4 = new BsonDocument { { "Account.Created", new BsonDocument { { "$gte", DateTime.Now.AddMonths(-1) } } } };
                        var Player_30Day = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync(Filter4).Result.ToListAsync();
                        Result["Players"]["30Days"] = Player_30Day.Count;
                    }
                    catch (Exception)
                    {
                    }

                    //Login_24Hours
                    try
                    {

                        var Filter5 = new BsonDocument { { "Account.LastLogin", new BsonDocument { { "$gte", DateTime.Now.AddHours(-24) } } } };
                        var Login_24Hours = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync(Filter5).Result.ToListAsync();
                        Result["Logins"]["24Hours"] = Login_24Hours.Count;
                    }
                    catch (Exception)
                    {
                    }


                    //Login_1Day
                    try
                    {
                        var Filter6 = new BsonDocument { { "Account.LastLogin", new BsonDocument { { "$gte", DateTime.Now.AddDays(-1) } } } };
                        var Login_1Day = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync(Filter6).Result.ToListAsync();
                        Result["Logins"]["1Days"] = Login_1Day.Count;
                    }
                    catch (Exception)
                    {
                    }

                    //Login_7Day
                    try
                    {

                        var Filter7 = new BsonDocument { { "Account.LastLogin", new BsonDocument { { "$gte", DateTime.Now.AddDays(-7) } } } };
                        var Login_7Day = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync(Filter7).Result.ToListAsync();
                        Result["Logins"]["7Days"] = Login_7Day.Count;
                    }
                    catch (Exception)
                    {
                    }


                    //Login_30Day
                    try
                    {
                        var Filter8 = new BsonDocument { { "Account.LastLogin", new BsonDocument { { "$gte", DateTime.Now.AddMonths(-1) } } } };
                        var Login_30Day = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync(Filter8).Result.ToListAsync();
                        Result["Logins"]["30Days"] = Login_30Day.Count;
                    }
                    catch (Exception)
                    {
                    }


                    //LeaderboardCount
                    try
                    {
                        var Option1 = new FindOptions<BsonDocument>();
                        Option1.Projection = new BsonDocument { { "Leaderboards", 1 } };
                        var Count_Leaderboards = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").FindAsync(new BsonDocument { { "_id", "Setting" } }).Result.ToListAsync();
                        Result["Leaderboards"]["Count"] = Count_Leaderboards.Count;
                    }
                    catch (Exception)
                    {
                        Result["Counts"].AsBsonDocument.Add("Leaderboards", 0);
                    }

                    //PlayerCount
                    try
                    {
                        var Count_Player = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync("{}").Result.ToListAsync();

                        Result["PlayersMonetiz"]["Count"] = Count_Player.Count;
                    }
                    catch (Exception) { };


                    //Logs
                    try
                    {
                        var Pipe = new[]
                        {
                            new BsonDocument{{"$project" , new BsonDocument { {"_id",0 } ,{"Count",new BsonDocument { { "$size", "$Logs" } } },{"Totall","$Monetiz.Logs" } } } },
                        };

                        var Results = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").AggregateAsync<BsonDocument>(Pipe).Result.SingleAsync();
                        Result["Logs"] = Results;
                    }
                    catch (Exception)
                    {
                    }

                    //APIs
                    try
                    {
                        var Pipe = new[]
                        {
                           new BsonDocument{ {"$project",new BsonDocument { {"_id",0 },{"Count",new BsonDocument { {"$sum",new BsonArray { {"$APIs.Write" },{"$APIs.Read" } } } } },{"Totall","$Monetiz.Apis" } } } }
                        };

                        var ResultAPI = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").AggregateAsync<BsonDocument>(Pipe).Result.SingleAsync();

                        Result["APIs"] = ResultAPI;

                    }
                    catch (Exception)
                    {

                    }

                    //player & Leaderboard Totall
                    try
                    {
                        var Option = new FindOptions<BsonDocument>() { Projection = new BsonDocument { { "Monetiz", 1 } } };

                        var Monetiz = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").FindAsync(new BsonDocument { { "_id", "Setting" } }, Option).Result.SingleAsync();
                        Result["PlayersMonetiz"]["Totall"] = Monetiz["Monetiz"]["Players"];
                        Result["Leaderboards"]["Totall"] = Monetiz["Monetiz"]["Leaderboards"];
                    }
                    catch (Exception)
                    {

                    }

                    //add read Write
                    await ReadWriteControll(Studio, API.Read);

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

    }
}
