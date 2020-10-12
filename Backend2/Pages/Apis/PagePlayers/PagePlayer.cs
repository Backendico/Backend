using Backend2.Pages.Apis;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Net;
using System.Threading.Tasks;


namespace Backend.Controllers.Players
{
    [Controller]
    public class PagePlayer : APIBase
    {

        /// <summary>
        /// creat new player
        /// 1:cheack username for dublicate username in studio
        /// </summary>
        /// <returns>
        /// <list type="bullet">
        /// <item> if token validatate <see cref="HttpStatusCode.OK"/></item>
        /// <item> if token validatate <see cref="HttpStatusCode.BadGateway"/></item>
        /// </list>
        /// </returns>
        [HttpPost]
        public async Task<string> CreatPlayer(string Token, string Studio, string Username, string Password)
        {
            if (await CheackToken(Token))
            {
                var TokenPlayer = ObjectId.GenerateNewId();
                var ModelPlayer = new BsonDocument
            {
                {"Account",new BsonDocument
                {
                    { "Name",""},
                    {"Avatar","" },
                    {"Email" ,""},
                    { "Token",TokenPlayer},
                    {"Username",""},
                    {"Password","" },
                    {"Language","" },
                    {"Created",DateTime.Now },
                    {"LastLogin",DateTime.Now },
                    {"RecoveryCode",0 },
                    {"Country","" }
                }

            },
                {"Logs",new BsonArray() }
            };

                //cheack username 

                if (await CheackUsername(Studio, Username) && Password != null)
                {
                    ModelPlayer["Account"]["Username"] = Username;
                    ModelPlayer["Account"]["Password"] = Password;
                }


                await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").InsertOneAsync(ModelPlayer);

                Response.StatusCode = Ok().StatusCode;


                return TokenPlayer.ToString();
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
                return "";

            }
        }


        /// <summary>
        /// recive frist detail for page player
        /// 1: recive 100 list player
        /// 2: recive all count Player
        /// </summary>
        /// <returns>
        /// list and count players
        /// </returns>
        [HttpPost]
        public async Task<string> ReciveDetailPagePlayer(string Token, string Studio)
        {
            if (await CheackToken(Token))
            {
                //Step :1 
                var Result = new BsonDocument { };

                var Filters = new FindOptions<BsonDocument, BsonDocument>();
                Filters.Limit = 100;
                Filters.Projection = new BsonDocument { { "Account.Password", 0 } };
                Filters.Sort = new BsonDocument { { "Account.Created", -1 } };
                var Players = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync("{}", Filters);


                var List = new BsonArray();

                await Players.ForEachAsync(eachplayer =>
                {
                    List.Add(eachplayer);
                });

                Result["ListPlayers"] = List;


                //Step:2
                Result["Players"] = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").CountDocumentsAsync("{}");

                Response.StatusCode = Ok().StatusCode;
                return Result.ToString();
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
                return "";
            }
        }


        [HttpDelete]
        public async Task DeletePlayer(string Token, string Studio, string TokenPlayer)
        {
            if (await CheackToken(Token))
            {

                var filter = new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) } };
                var result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").DeleteOneAsync(filter);


                if (result.DeletedCount >= 1)
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


        [HttpPost]
        public async Task SavePlayer(string Token, string Studio, string TokenPlayer, string AccountDetail)
        {
            if (await CheackToken(Token))
            {
                var deserilse = BsonDocument.Parse(AccountDetail);

                var filter = new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) } };
                var update = new UpdateDefinitionBuilder<BsonDocument>().Set("Account", deserilse);


                var result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").UpdateOneAsync(filter, update);
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

        [HttpPost]
        public async Task SendEmailRecovery(string Token, string studio, string TokenPlayer)
        {
            try
            {
                if (await CheackToken(Token))
                {
                    var Filter = new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) } };
                    var Update = new UpdateDefinitionBuilder<BsonDocument>().Set("Account.RecoveryCode", new Random().Next());

                    var Result = await Client.GetDatabase(studio).GetCollection<BsonDocument>("Players").UpdateOneAsync(Filter, Update);

                    if (Result.ModifiedCount >= 1)
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


        /// <summary>
        /// 1: search username
        /// 2: if not find and exption return  bad request
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Studio"></param>
        /// <param name="Username"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> SearchUsername(string Token, string Studio, string Username)
        {
            try
            {
                if (await CheackToken(Token))
                {
                    var filter = new BsonDocument { { "Account.Username", Username } };
                    var OptionFind = new FindOptions<BsonDocument>();
                    OptionFind.Projection = new BsonDocument { { "Account.Password", 0 } };

                    var Users = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync(filter, OptionFind);

                    var result = await Users.SingleAsync();

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
        /// 1: search username
        /// 2: if not find and exption return  bad request
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Studio"></param>
        /// <param name="Username"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> SearchEmail(string Token, string Studio, string Email)
        {
            try
            {
                if (await CheackToken(Token))
                {
                    var filter = new BsonDocument { { "Account.Email", Email } };
                    var OptionFind = new FindOptions<BsonDocument>();
                    OptionFind.Projection = new BsonDocument { { "Account.Password", 0 } };

                    var Users = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync(filter, OptionFind);

                    var result = await Users.SingleAsync();

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
        /// 1: search username
        /// 2: if not find and exption return  bad request
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Studio"></param>
        /// <param name="Username"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> SearchToken(string Token, string Studio, string TokenPlayer)
        {
            try
            {
                if (await CheackToken(Token))
                {
                    var filter = new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) } };
                    var OptionFind = new FindOptions<BsonDocument>();
                    OptionFind.Projection = new BsonDocument { { "Account.Password", 0 } };

                    var Detailuser = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync(filter, OptionFind).Result.SingleAsync();


                    Response.StatusCode = Ok().StatusCode;
                    return Detailuser.ToString();
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

        [HttpPost]
        public async Task Save_LeaderboardPlayer(string Token, string TokenPlayer, string Studio, string DetailLeaderboard)
        {
            if (await CheackToken(Token))
            {

                var Filter = new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) } };
                var Update = new UpdateDefinitionBuilder<BsonDocument>().Set($"Leaderboards.List", BsonDocument.Parse(DetailLeaderboard));

                var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").UpdateOneAsync(Filter, Update);

                if (Result.MatchedCount >= 1)
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


        #region Internal Method
        /// <summary>
        /// cheack username 
        /// 0: if Database Empty return true
        /// 1: find User return True
        /// 2: not find return False
        /// </summary>
        /// <param name="Studio"></param>
        /// <param name=""></param>
        /// <param name="Username"></param>
        /// <returns></returns>
        async Task<bool> CheackUsername(string Studio, string Username)
        {

            try
            {
                if (Username != null)
                {
                    var filter = new BsonDocument { { "Account.Username", Username } };
                    var result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").Find(filter).SingleAsync();
                    if (result.ElementCount >= 1)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {

                return true;
            }
        }

        #endregion


    }
}
