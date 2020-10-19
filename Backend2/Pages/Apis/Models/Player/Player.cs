using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend2.Pages.Apis.Models.Player
{
    public class Player : BasicAPIs
    {
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

                if (await CheackUsernamePlayer(Studio, Username) && Password != null)
                {
                    ModelPlayer["Account"]["Username"] = Username;
                    ModelPlayer["Account"]["Password"] = Password;
                }

                await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").InsertOneAsync(ModelPlayer);

                return TokenPlayer.ToString();
            }
            else
            {
                return "";
            }
        }

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

                return Result.ToString();
            }
            else
            {
                return "";
            }
        }

        public async Task<bool> DeletePlayer(string Token, string Studio, string TokenPlayer)
        {
            if (await CheackToken(Token))
            {

                var filter = new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) } };
                var result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").DeleteOneAsync(filter);


                if (result.DeletedCount >= 1)
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

        public async Task<bool> SavePlayer(string Token, string Studio, string TokenPlayer, string AccountDetail)
        {
            if (await CheackToken(Token))
            {
                var deserilse = BsonDocument.Parse(AccountDetail);

                var filter = new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) } };
                var update = new UpdateDefinitionBuilder<BsonDocument>().Set("Account", deserilse);


                var result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").UpdateOneAsync(filter, update);
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

        public async Task<bool> SendEmailRecovery(string Token, string studio, string TokenPlayer)
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

        public async Task<BsonDocument> SearchUsername(string Token, string Studio, string Username)
        {
            try
            {
                if (await CheackToken(Token))
                {
                    var filter = new BsonDocument { { "Account.Username", Username } };
                    var OptionFind = new FindOptions<BsonDocument>();
                    OptionFind.Projection = new BsonDocument { { "Account.Password", 0 } };

                    var Users = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync(filter, OptionFind).Result.SingleAsync();

                    return Users;
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

        public async Task<BsonDocument> SearchEmail(string Token, string Studio, string Email)
        {
            try
            {
                if (await CheackToken(Token))
                {
                    var filter = new BsonDocument { { "Account.Email", Email } };
                    var OptionFind = new FindOptions<BsonDocument>();
                    OptionFind.Projection = new BsonDocument { { "Account.Password", 0 } };

                    var Users = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync(filter, OptionFind).Result.SingleAsync();

                    return Users;
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

        public async Task<BsonDocument> SearchToken(string Token, string Studio, string TokenPlayer)
        {
            try
            {
                if (await CheackToken(Token))
                {
                    var filter = new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) } };
                    var OptionFind = new FindOptions<BsonDocument>();
                    OptionFind.Projection = new BsonDocument { { "Account.Password", 0 } };

                    var Detailuser = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync(filter, OptionFind).Result.SingleAsync();

                    return Detailuser;
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

        public async Task<bool> Save_LeaderboardPlayer(string Token, string TokenPlayer, string Studio, string DetailLeaderboard)
        {
            if (await CheackToken(Token))
            {

                var Filter = new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) } };
                var Update = new UpdateDefinitionBuilder<BsonDocument>().Set($"Leaderboards.List", BsonDocument.Parse(DetailLeaderboard));

                var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").UpdateOneAsync(Filter, Update);

                if (Result.MatchedCount >= 1)
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
    }
}
