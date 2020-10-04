using Backend2.Pages.Apis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace Backend.Controllers.PageLeaderBoard
{
    [Controller]
    public class PageLeaderBoard : APIBase
    {

        /// <summary>
        /// recive abstract leaderboard
        /// </summary>
        /// <returns>
        /// recive leaderboards string
        /// </returns>
        [HttpPost]
        public async Task<string> ReciveLeaderboards(string Token, string Studio)
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

                    Response.StatusCode = Ok().StatusCode;

                    return Result["Leaderboards"]["List"].ToString();
                }
                else
                {
                    Response.StatusCode = BadRequest().StatusCode;
                    return new BsonDocument().ToString();
                }

            }
            catch (Exception )
            {
                Response.StatusCode = BadRequest().StatusCode;

                return new BsonDocument().ToString();
            }
        }


        /// <summary>
        /// 1: Cheack token user
        /// 2: insert Leaderboard
        /// </summary>
        /// <returns>
        /// <list type="bullet">
        /// <item> if Token vrifie and creat leaderboard  <see cref="HttpStatusCode.OK"/></item>
        /// <item> Else <see cref="HttpStatusCode.BadRequest"/></item>
        /// </list>
        /// </returns>
        [HttpPost]
        public async Task CreatLeaderBoard(string Token, string Studio, string DetailLeaderboard)
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

                Response.StatusCode = Ok().StatusCode;


            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }


        /// <summary>
        /// 1:cheack token
        /// 2: insert setting =setting
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Studio"></param>
        /// <param name="DetailLeaderboard"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task EditLeaderBoard(string Token, string Studio, string DetailLeaderboard)
        {
            if (await CheackToken(Token))
            {
                var DeserilseData = BsonDocument.Parse(DetailLeaderboard);

                var filter = new BsonDocument { { "_id", "Setting" } };
                var update = new UpdateDefinitionBuilder<BsonDocument>().Set($"Leaderboards.List.{DeserilseData["Name"]}", DeserilseData);
                var result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").UpdateOneAsync(filter, update);

                if (result.ModifiedCount >= 1)
                {
                    Response.StatusCode = Ok().StatusCode;

                }
                else
                {
                    Response.StatusCode = BadRequest().StatusCode;

                }
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }

        }


        /// <summary>
        /// Recive Leaderboard Detail
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Studio"></param>
        /// <param name="NameLeaderboard"></param>
        /// <param name="Count"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> Leaderboard(string Token, string Studio, string NameLeaderboard, string Count)
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

                    Response.StatusCode = Ok().StatusCode;
                    return result.ToString();

                }
                else
                {
                    Response.StatusCode = BadRequest().StatusCode;
                    return "";
                }
            }
            catch (Exception)
            {
                Response.StatusCode = BadRequest().StatusCode;
                return "";
            }
        }


        /// <summary>
        /// Inject Player to leaderboard
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Detail">
        /// 1:TokenLeaderboard
        /// 2:Data
        /// 3:Studio
        /// </param>
        /// <returns></returns>
        public async Task Add(string Token, string Studio, string TokenPlayer, string NameLeaderboard, string Value)
        {
            if (await CheackToken(Token))
            {
                var Filter = new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) } };
                var Update = new UpdateDefinitionBuilder<BsonDocument>().Set($"Leaderboards.List.{NameLeaderboard}", long.Parse(Value));
                var result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").UpdateOneAsync(Filter, Update);
                if (result.ModifiedCount >= 1)
                {
                    Response.StatusCode = Ok().StatusCode;

                }
                else
                {
                    Response.StatusCode = BadRequest().StatusCode;

                }
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;

            }
        }


        /// <summary>
        /// Remove Player from Leaderboard 
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Detail">
        /// 1:TokenLeaderboard
        /// 2:Data
        /// 3:Studio
        /// </param>
        /// <returns></returns>
        [HttpDelete]
        public async Task Remove(string Token, string Studio, string TokenPlayer, string NameLeaderboard)
        {
            if (await CheackToken(Token))
            {
                var Filter = new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) } };
                var Update = new UpdateDefinitionBuilder<BsonDocument>().Unset($"Leaderboards.List.{NameLeaderboard}");
                var result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").UpdateOneAsync(Filter, Update);
                if (result.ModifiedCount >= 1)
                {
                    Response.StatusCode = Ok().StatusCode;

                }
                else
                {
                    Response.StatusCode = BadRequest().StatusCode;

                }
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }

        }


        /// <summary>
        /// 1: remove all value from all users <paramref name="Studio"/>
        /// 2: reset Start in setting <paramref name="NameLeaderboard"/>
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Studio"></param>
        /// <param name="NameLeaderboard"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task Reset(string Token, string Studio, string NameLeaderboard)
        {
            if (await CheackToken(Token))
            {
                //remove all value in user
                var Update = new UpdateDefinitionBuilder<BsonDocument>().Unset($"Leaderboards.List.{NameLeaderboard}");
                await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").UpdateManyAsync("{}", Update);


                //reset start in setting
                var filter = new BsonDocument { { "_id", "Setting" } };
                var Update1 = new UpdateDefinitionBuilder<BsonDocument>().Set($"Leaderboards.List.{NameLeaderboard}.Start", DateTime.Now);

                await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").UpdateOneAsync(filter, Update1);

                Response.StatusCode = Ok().StatusCode;

            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }


        /// <summary>
        /// Make Backup
        /// 1: find all Players 
        /// 2: deploy start and end time
        /// 3: inject to setting
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Studio"></param>
        /// <param name="NameLeaderboard"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task Backup(string Token, string Studio, string NameLeaderboard)
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
                    var update1 = new UpdateDefinitionBuilder<BsonDocument>().Set($"Leaderboards.List.{NameLeaderboard}.Backups.{new Random().Next()}", Result.AsBsonDocument);

                    var FinalResult = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").UpdateOneAsync(new BsonDocument { { "_id", "Setting" } }, update1);


                    if (FinalResult.ModifiedCount >= 1)
                    {
                        Response.StatusCode = Ok().StatusCode;

                    }
                    else
                    {
                        Response.StatusCode = BadRequest().StatusCode;


                    }
                }
                else
                {
                    Response.StatusCode = BadRequest().StatusCode;
                }
            }
            catch (Exception)
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }

        [HttpPost]
        public async Task<string> ReciveBackup(string Token, string Studio, string NameLeaderboard)
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
                    Response.StatusCode = Ok().StatusCode;
                    return Result.ToString();
                }
                else
                {
                    Response.StatusCode = BadRequest().StatusCode;
                    return "";
                }

            }
            catch (Exception)
            {
                Response.StatusCode = BadRequest().StatusCode;
                return "";
            }
        }

        [HttpDelete]
        public async Task RemoveBackup(string Token, string Studio, string NameLeaderboard, string Version)
        {
            if (await CheackToken(Token))
            {
                var Filter = new BsonDocument { { "_id", "Setting" } };
                var Update = new UpdateDefinitionBuilder<BsonDocument>().Unset($"Leaderboards.List.{NameLeaderboard}.Backups.{Version}");
                var result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").UpdateOneAsync(Filter, Update);

                if (result.ModifiedCount >= 1)
                {
                    Response.StatusCode = Ok().StatusCode;

                }
                else
                {
                    Response.StatusCode = BadRequest().StatusCode;

                }
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }

        }


        public async Task<string> DownloadBackup(string Token, string Studio, string NameLeaderboard, string Version)
        {
            if (await CheackToken(Token))
            {
                var Filter = new BsonDocument { { "_id", "Setting" } };
                var option = new FindOptions<BsonDocument>();
                option.Projection = new BsonDocument { { "_id", 0 }, { $"Leaderboards.List.{NameLeaderboard}.Backups.{Version}.List", 1 } };
                var result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").FindAsync(Filter, option).Result.SingleAsync();


                return result["Leaderboards"]["List"][NameLeaderboard]["Backups"][Version]["List"].ToString();
            }
            else
            {

                Response.StatusCode = BadRequest().StatusCode;
                return "";
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
        [HttpPost]
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

    }
}
