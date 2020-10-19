﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend2.Pages.Apis.PageDashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend2.Pages.Apis.UserAPI.AdminAPI
{
    [Controller]
    public class AdminAPI : UserAPIBase
    {

        [HttpPost]
        public async Task<string> ReciveStatices(string Token, string Studio)
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


                    Response.StatusCode = Ok().StatusCode;
                    return Result.ToString();
                }
                else
                {
                    Response.StatusCode = BadRequest().StatusCode;
                    return Result.ToString();
                }

            }
            catch (Exception)
            {
                Response.StatusCode = BadRequest().StatusCode;
                return Result.ToString();
            }
        }

        public void ResetLeaderboard()
        {

        }

        public void AddLog()
        {

        }

        public void ReciveStatusServer()
        {

        }

        public void ReciveStudioSetting()
        {

        }

        public void DeletePlayer()
        {

        }

        public void BanPlayer()
        {

        }
        public void SearchPlayer()
        {

        }

        public void ReciveBackups()
        {

        }

        public void SaveLeaderboard()
        {

        }


    }
}
