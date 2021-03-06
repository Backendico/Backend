﻿using Backend2.Pages.Apis.Models.Leaderobard;
using Backend2.Pages.Apis.PageLoggs;
using Backend2.Pages.Apis.UserAPI.AdminAPI;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Runtime.CompilerServices;
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
                        {"Phone",new BsonInt64(0)},
                        { "Token",TokenPlayer},
                        {"Username",""},
                        {"Password","" },
                        {"Language","" },
                        {"IsBan",false },
                        {"Created",DateTime.Now },
                        {"LastLogin",DateTime.Now },
                        {"RecoveryCode",0 },
                        {"Country","" }
                    }
                    },
                    {"Logs",new BsonArray() },
                    {"Achievements",new BsonArray() },
                    {"Leaderboards",new BsonArray() }
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


        public async Task<string> ReciveDetailPagePlayer(string Token, string Studio, int Count)
        {
            if (await CheackToken(Token))
            {

                try
                {

                //step 0:
                if (Count <= 0)
                    Count = 100;

                //Step :1 
                var Result = new BsonDocument { };

                var pipe = new[]
                {
                    new BsonDocument{{"$project",new BsonDocument
                    {
                        {"Account.Name",1 },
                        {"Account.IsBan",1 },
                        {"Account.Email",1 },
                        {"Account.LastLogin",1 },
                        {"Account.Created",1 },
                        {"Account.Currencey" ,"Not Implement"},
                        {"Account.Country",1 },
                        {"Account.Token",1 },
                        {"Account.Username",1 },
                        {"Account.Avatar",1 }

                    } },

                    },
                   new BsonDocument{{"$limit",Count}},
                   new BsonDocument{{"$group",new BsonDocument{ {"_id","_id" },{ "Players", new BsonDocument { { "$push", "$Account" } } } } } },


                };

                var Players = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").AggregateAsync<BsonDocument>(pipe).Result.SingleAsync();

                Result["ListPlayers"] = Players;

                //Step:2
                Result["Players"] = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").CountDocumentsAsync("{}");

                return Result.ToString();
                }
                catch (Exception ex)
                {
                    return new BsonDocument("ERR",ex.Message).ToString();
                }
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

        public async Task<bool> BanPlayer(string Studio, string Token, string TokenPlayer)
        {
            if (await CheackToken(Token))
            {
                var filter = new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) } };
                var Update = new BsonDocument { { "Account.IsBan", true } };
                var result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").UpdateOneAsync(filter, Update);



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

        public async Task<bool> UnBanPlayer(string Studio, string Token, string TokenPlayer)
        {
            if (await CheackToken(Token))
            {
                var filter = new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) } };
                var Update = new BsonDocument { { "Account.IsBan", false } };
                var result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").UpdateOneAsync(filter, Update);


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

        public async Task<BsonDocument> LoginPlayer(string Token, string Studio, string TokenPlayer)
        {
            if (await CheackToken(Token))
            {
                var Option = new FindOptions<BsonDocument>() { Projection = new BsonDocument { { "Account", 1 } } };
                var Player = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync(new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) } }, Option).Result.SingleAsync();



                return Player;
            }
            else
            {
                return new BsonDocument();
            }
        }


        public async Task<BsonDocument> LoginPlayer(string Token, string Studio, string Username, string Password)
        {
            if (await CheackToken(Token))
            {
                var Option = new FindOptions<BsonDocument>() { Projection = new BsonDocument { { "Account", 1 } } };
                var Player = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync(new BsonDocument { { "Account.Username", Username }, { "Account.Password", Password } }, Option).Result.SingleAsync();

                return Player;
            }
            else
            {
                return new BsonDocument();
            }
        }


        public async Task<bool> AddAvatar(string Token, string Studio, string TokenPlayer, string Link)
        {
            if (await CheackToken(Token))
            {
                try
                {
                    _ = new Uri(Link);

                    var Update = new UpdateDefinitionBuilder<BsonDocument>().Set<string>("Account.Avatar", Link);
                    var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").UpdateOneAsync(new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) } }, Update);


                    if (Result.ModifiedCount >= 1)
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
            else
            {
                return false;
            }
        }

        public async Task<bool> AddLanguage(string Token, string Studio, string TokenPlayer, string Language)
        {
            if (await CheackToken(Token))
            {
                try
                {
                    var Update = new UpdateDefinitionBuilder<BsonDocument>().Set<string>("Account.Language", Language);
                    var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").UpdateOneAsync(new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) } }, Update);


                    if (Result.ModifiedCount >= 1)
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
            else
            {
                return false;
            }
        }

        public async Task<bool> AddCountry(string Token, string Studio, string TokenPlayer, string Country)
        {
            if (await CheackToken(Token))
            {
                try
                {
                    var Update = new UpdateDefinitionBuilder<BsonDocument>().Set<string>("Account.Country", Country);
                    var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").UpdateOneAsync(new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) } }, Update);

                    if (Result.ModifiedCount >= 1)
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
            else
            {
                return false;
            }
        }

        public async Task<bool> AddUsername(string Token, string Studio, string TokenPlayer, string Username)
        {
            if (await CheackToken(Token) && await CheackUsernamePlayer(Studio, Username))
            {
                var Update = new UpdateDefinitionBuilder<BsonDocument>().Set<string>("Account.Username", Username);
                var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").UpdateOneAsync(new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) } }, Update);


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

        public async Task<bool> AddEmail(string Token, string Studio, string TokenPlayer, string Email)
        {
            if (await CheackToken(Token) && !await CheackEmailPlayer(Studio, Email))
            {
                try
                {
                    _ = new MailAddress(Email);
                    var Update = new UpdateDefinitionBuilder<BsonDocument>().Set<string>("Account.Email", Email);
                    var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").UpdateOneAsync(new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) } }, Update);

                    if (Result.ModifiedCount >= 1)
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
            else
            {
                return false;
            }
        }

        public async Task<bool> AddPassword(string Token, string Studio, string TokenPlayer, string OldPassword, string NewPassword)
        {
            if (await CheackToken(Token))
            {
                var Filter = new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) }, { "Account.Password", OldPassword } };
                var Update = Builders<BsonDocument>.Update.Set("Account.Password", NewPassword);

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

        public async Task<bool> AddNickname(string Token, string Studio, string TokenPlayer, string Nickname)
        {
            if (await CheackToken(Token))
            {
                var Filter = new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) } };
                var Update = Builders<BsonDocument>.Update.Set("Account.Name", Nickname);
                var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").UpdateOneAsync(Filter, Update);

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

        public async Task<bool> AddPhoneNumber(string Token, string Studio, string TokenPlayer, string PhoneNumber)
        {
            if (await CheackToken(Token))
            {

                var Filter = new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) } };
                var Update = Builders<BsonDocument>.Update.Set("Account.Phone", PhoneNumber);
                var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").UpdateOneAsync(Filter, Update);


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

        public async Task<bool> AddLogPlayer(string Token, string Studio, string TokenPlayer, string Header, string Description)
        {
            if (await CheackToken(Token))
            {

                var DataUpdate = new BsonDocument
                {
                    { "Header",Header},
                    {"Description",Description },
                    {"Time",DateTime.Now },
                };

                var Update = new UpdateDefinitionBuilder<BsonDocument>().Push("Logs", DataUpdate);
                var result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").UpdateOneAsync(new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) } }, Update);

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


        public async Task<bool> ClearLogs(string Token, string Studio, string TokenPlayer)
        {
            if (await CheackToken(Token))
            {
                var Filter = new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) } };
                var Update = new UpdateDefinitionBuilder<BsonDocument>().Set("Logs", new BsonArray());

                var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").UpdateOneAsync(Filter, Update);


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

        public async Task<BsonDocument> RecivePlayerLogs(string Token, string Studio, string TokenPlayer, string Count)
        {
            if (await CheackToken(Token))
            {
                var Pipe = new[]
                {
                    new BsonDocument{ {"$match",new BsonDocument { {"Account.Token",ObjectId.Parse(TokenPlayer) } } } },
                    new BsonDocument{ { "$project" ,new BsonDocument { { "Logs", 1 } } } },
                    new BsonDocument{ {"$unwind","$Logs" } } ,
                    new BsonDocument{ {"$sort",new BsonDocument { {"Logs.Time" ,-1} } } },
                    new BsonDocument{{"$group",new BsonDocument { {"_id","$_id" },{"Logs",new BsonDocument { {"$push","$Logs" } } } } }}
                };


                return await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").AggregateAsync<BsonDocument>(Pipe).Result.SingleAsync();

            }
            else
            {
                return new BsonDocument();
            }
        }




        public async Task<int> Recovery_Step1(string Token, string Studio, string TokenPlayer, string Email)
        {
            if (await CheackToken(Token))
            {
                try
                {
                    var code = new Random().Next();

                    var Filter = new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) }, { "Account.Email", Email } };
                    var Update = new UpdateDefinitionBuilder<BsonDocument>().Set("Account.RecoveryCode", code);

                    var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").UpdateOneAsync(Filter, Update);


                    var bodyMessage =
                       $"Hello dear " +
                       "\n" +
                       "\n" +
                       "This email was sent to your request to recover your account." +
                       "\n" +
                       "Ignore this email if you have not submitted an account recovery request.  " +
                       "\n\n" +
                       "Or use this code to recover the account." +
                       "\n\n" +
                       $"Code : {code}" +
                       "\n\n\n" +
                       "Thanks for choosing us" +
                       "\n" +
                       "Backendi.ir";

                    var Message = new MailMessage("recovery@backendi.ir", Email, "Recovery Account", bodyMessage);

                    SendMail_Recovery(Message, (s, e) => { }, () => { });


                    if (Result.ModifiedCount >= 1)
                    {
                        return code;
                    }
                    else
                    {
                        return 0;
                    }

                }
                catch (Exception)
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        public async Task<bool> Recovery_Step2(string Token, string Studio, string TokenPlayer, string Email, int Code)
        {
            if (await CheackToken(Token))
            {
                try
                {
                    var Filter = new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) }, { "Account.Email", Email }, { "Account.RecoveryCode", Code } };
                    var Update = new UpdateDefinitionBuilder<BsonDocument>().Set("Account.RecoveryCode", 0);

                    var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").UpdateOneAsync(Filter, Update);

                    if (Result.ModifiedCount >= 1)
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
            else
            {
                return false;
            }
        }

        public async Task<bool> ChangePassword(string Token, string Studio, string TokenPlayer, string Password)
        {
            if (await CheackToken(Token))
            {
                try
                {
                    var Filter = new BsonDocument { { "Account.Token", ObjectId.Parse(TokenPlayer) } };
                    var Update = new UpdateDefinitionBuilder<BsonDocument>().Set("Account.Password", Password).Set("Account.RecoveryCode", 0);

                    var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").UpdateOneAsync(Filter, Update);


                    var player = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync(Filter).Result.SingleAsync();

                    //send mail status
                    var body = "Your account password changed successfully" +
                        "\n\n" +
                        $"New password : {Password}" +
                        $"\n" +
                        "Thanks" +
                        "\n" +
                        "backendi.ir";
                    var message = new MailMessage("recovery@backendi.ir", player["Account"]["Email"].AsString, "Password Changed", body);


                    SendMail_Recovery(message, (s, e) => { }, () => { });

                    if (Result.ModifiedCount >= 1)
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
            else
            {
                return false;
            }
        }
    }
}
