using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Backend2.Pages.Apis.PageDashboard
{
    [Controller]
    public class PageDashboard : APIBase
    {
        /// <summary>
        /// recive Detail Page Dashboard
        /// </summary>
        /// 
        /// <param name="Token"></param>
        /// <param name="Studio"></param>
        /// <returns>
        /// <list  type="bullet">
        /// <listheader >Players</listheader>
        /// <list type="bullet">
        ///   <item>24 Hours</item>
        ///   <item>1Day</item>
        ///   <item>7 Day</item>
        ///   <item>30 day</item>
        /// </list>
        /// </list>
        /// <list type="bullet">
        /// <listheader> Logins</listheader>
        /// <list type="bullet">
        ///   <item>24 Hours</item>
        ///   <item>1Day</item>
        ///   <item>7 Day</item>
        ///   <item>30 day</item>
        /// 
        /// </list>
        /// </list>
        /// 
        /// <list type="bullet">
        /// <listheader>Counts</listheader>
        /// <list type="bullet">
        ///   <item>Leaderboards</item>
        ///   <item> Players</item>
        /// 
        /// </list>
        /// </list>
        /// </returns>
        [HttpPost]
        public async Task<string> ReciveDetail(string Token, string Studio)
        {
            if (await CheackToken(Token))
            {
                var Result = new BsonDocument
            {
                {"Players" ,new BsonDocument{ } },
                {"Logins",new BsonDocument{ } },
                {"Counts",new BsonDocument{ } },
                    {"Monetiz",new BsonDocument{} }
            };

                //player_24hours
                try
                {

                    var Filter1 = new BsonDocument { { "Account.Created", new BsonDocument { { "$gte", DateTime.Now.AddHours(-24) } } } };
                    var Player_24Hours = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync(Filter1).Result.ToListAsync();
                    Result["Players"].AsBsonDocument.Add("24Hours", Player_24Hours.Count);
                }
                catch (Exception)
                {
                    Result["Players"].AsBsonDocument.Add("24Hours", 0);
                    throw;
                }

                // player_1Day
                try
                {

                    var Filter2 = new BsonDocument { { "Account.Created", new BsonDocument { { "$gte", DateTime.Now.AddDays(-1) } } } };
                    var Player_1day = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync(Filter2).Result.ToListAsync();
                    Result["Players"].AsBsonDocument.Add("1Days", Player_1day.Count);
                }
                catch (Exception)
                {
                    Result["Players"].AsBsonDocument.Add("1Days", 0);
                }


                // player_7Day
                try
                {
                    var Filter3 = new BsonDocument { { "Account.Created", new BsonDocument { { "$gte", DateTime.Now.AddDays(-7) } } } };
                    var Player_7day = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync(Filter3).Result.ToListAsync();
                    Result["Players"].AsBsonDocument.Add("7Days", Player_7day.Count);
                }
                catch (Exception)
                {
                    Result["Players"].AsBsonDocument.Add("7Days", 0);
                }

                // player_30Day
                try
                {
                    var Filter4 = new BsonDocument { { "Account.Created", new BsonDocument { { "$gte", DateTime.Now.AddMonths(-1) } } } };
                    var Player_30Day = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync(Filter4).Result.ToListAsync();
                    Result["Players"].AsBsonDocument.Add("30Days", Player_30Day.Count);
                }
                catch (Exception)
                {
                    Result["Players"].AsBsonDocument.Add("30Days", 0);
                }

                //Login_24Hours
                try
                {

                    var Filter5 = new BsonDocument { { "Account.LastLogin", new BsonDocument { { "$gte", DateTime.Now.AddHours(-24) } } } };
                    var Login_24Hours = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync(Filter5).Result.ToListAsync();
                    Result["Logins"].AsBsonDocument.Add("24Hours", Login_24Hours.Count);
                }
                catch (Exception)
                {

                    Result["Logins"].AsBsonDocument.Add("24Hours", 0);
                }


                //Login_1Day
                try
                {

                    var Filter6 = new BsonDocument { { "Account.LastLogin", new BsonDocument { { "$gte", DateTime.Now.AddDays(-1) } } } };
                    var Login_1Day = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync(Filter6).Result.ToListAsync();
                    Result["Logins"].AsBsonDocument.Add("1Days", Login_1Day.Count);
                }
                catch (Exception)
                {

                    Result["Logins"].AsBsonDocument.Add("1Days", 0);
                }

                //Login_7Day
                try
                {

                    var Filter7 = new BsonDocument { { "Account.LastLogin", new BsonDocument { { "$gte", DateTime.Now.AddDays(-7) } } } };
                    var Login_7Day = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync(Filter7).Result.ToListAsync();
                    Result["Logins"].AsBsonDocument.Add("7Days", Login_7Day.Count);
                }
                catch (Exception)
                {
                    Result["Logins"].AsBsonDocument.Add("7Days", 0);
                }


                //Login_30Day
                try
                {

                    var Filter8 = new BsonDocument { { "Account.LastLogin", new BsonDocument { { "$gte", DateTime.Now.AddMonths(-1) } } } };
                    var Login_30Day = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync(Filter8).Result.ToListAsync();
                    Result["Logins"].AsBsonDocument.Add("30Days", Login_30Day.Count);
                }
                catch (Exception)
                {
                    Result["Logins"].AsBsonDocument.Add("30Days", 0);
                }


                //LeaderboardCount
                try
                {
                    var Option1 = new FindOptions<BsonDocument>();
                    Option1.Projection = new BsonDocument { { "Leaderboards", 1 } };
                    var Count_Leaderboards = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").FindAsync(new BsonDocument { { "_id", "Setting" } }).Result.ToListAsync();

                    Result["Counts"].AsBsonDocument.Add("Leaderboards", Count_Leaderboards[0]["Leaderboards"]["List"].AsBsonDocument.ElementCount);
                }
                catch (Exception)
                {
                    Result["Counts"].AsBsonDocument.Add("Leaderboards", 0);
                }

                //PlayerCount
                try
                {
                    var Count_Player = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync("{}").Result.ToListAsync();

                    Result["Counts"].AsBsonDocument.Add("Players", Count_Player.Count);
                }
                catch (Exception)
                {

                    Result["Counts"].AsBsonDocument.Add("Players", 0);
                }


                //recive Monetize  list
                var option4 = new FindOptions<BsonDocument>();
                option4.Projection = new BsonDocument { { "Monetiz", 1 } };

                var Monetize = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").FindAsync(new BsonDocument { { "_id", "Setting" } }, option4).Result.SingleAsync();
                Result["Monetiz"] = Monetize;

                Response.StatusCode = Ok().StatusCode;
                return Result.ToString();
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
                return "";
            }


        }


        /// <summary>
        /// cheack status server
        /// </summary>
        [HttpGet]
        public void CheackStatusServer()
        {
            Response.StatusCode = Ok().StatusCode;
        }

    }
}
