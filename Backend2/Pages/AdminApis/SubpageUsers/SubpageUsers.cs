using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Backend2.Pages.AdminApis.ApisBasicAdmin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend2.Pages.AdminApis.SubpageUsers
{
    [Controller]
    public class SubpageUsers : BasicApiAdmin
    {
        [HttpPost]
        public async Task<string> ReciveUsers(string Token)
        {
            try
            {
                var Result = new BsonDocument
            {
                {"ListUsers",new BsonArray() }
            };

                var Option = new FindOptions<BsonDocument>() { Projection = new BsonDocument { { "AccountSetting", 1 }, { "Games", 1 } } };

                var ResultFind = await Client.GetDatabase(AdminDatabase).GetCollection<BsonDocument>("Users").FindAsync("{}", Option).Result.ToListAsync();


                foreach (var item in ResultFind)
                {
                    var DetailUser = new BsonDocument
                    {
                       {"Token",item["AccountSetting"]["Token"]},
                       {"Username",item["AccountSetting"]["Username"] },
                       {"Email",item["AccountSetting"]["Email"] },
                       {"Phone",item["AccountSetting"]["Phone"] },
                       {"GameStudio",item["Games"].AsBsonArray.Count },
                       {"Players",new BsonInt64(0) },
                       {"Leaderboards",new BsonInt32(0) },
                       {"Cash",new BsonInt32(0) },
                    };

                    //RecivePlayers
                    {
                        foreach (var Studio in item["Games"].AsBsonArray)
                        {
                            DetailUser["Players"] = DetailUser["Players"].ToInt64() + await Client.GetDatabase(Studio.ToString()).GetCollection<BsonDocument>("Players").CountDocumentsAsync("{}");
                        }
                    }

                    //Leaderboards & Cash
                    {
                        foreach (var Studio in item["Games"].AsBsonArray)
                        {

                            var PipeLine = new[]
                            {
                                new BsonDocument{ {"$project",new BsonDocument{{"Leaderboards",1 },{"Monetiz.Cash",1 } } } },
                                new BsonDocument{{"$addFields",new BsonDocument { {"LeaderboardArray",new BsonDocument { {"$objectToArray", "$Leaderboards.List" } } } } }},
                                new BsonDocument{{"$addFields",new BsonDocument { {"Cash","$Monetiz.Cash" },{"LeaderboardSize", new BsonDocument { { "$size", "$LeaderboardArray" } } } } } },
                                new BsonDocument{ {"$project",new BsonDocument { {"_id",0 },{"Cash",1 },{"LeaderboardSize",1 } } } }

                            };

                            var Setting = await Client.GetDatabase(Studio.ToString()).GetCollection<BsonDocument>("Setting").AggregateAsync<BsonDocument>(PipeLine).Result.SingleAsync();
                            DetailUser["Leaderboards"] = DetailUser["Leaderboards"].AsInt32 + Setting["LeaderboardSize"].AsInt32;
                            DetailUser["Cash"] = DetailUser["Cash"].AsInt32 + Setting["Cash"].AsInt32;
                        }
                    }



                    Result["ListUsers"].AsBsonArray.Add(DetailUser);

                }
                Response.StatusCode = Ok().StatusCode;
                return Result.ToString();

            }
            catch (Exception ex)
            {
                Response.StatusCode = BadRequest().StatusCode;
                Debug.WriteLine(ex.Message);
                return "";
            }

        }



    }
}
