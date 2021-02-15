using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend2.Pages.Apis.Models.Studio
{
    public class Studios : BasicAPIs
    {
        public async Task<string> CreatStudio(string NameStudio, string Token)
        {
            if (await CheackToken(Token))
            {
                // Step1:
                var NameDataBase = NameStudio + "+" + ObjectId.GenerateNewId();
                var ModelDataBase = new BsonDocument {
                    { "_id","Setting"},
                    {"Logs",new BsonArray() },
                    {"APIs",new BsonDocument{ {"Read",0 },{"Write" ,0 } } },
                    { "Setting",
                        new BsonDocument
                        {
                            {"Name",NameStudio },
                            { "Type","Game"},
                            {"Token",ObjectId.GenerateNewId() },
                            { "Creator",Token},
                            { "Created",DateTime.Now},
                            {"Database",NameDataBase }}
                    }
                    ,{"Achievements",new BsonArray() },
                    {"Support" ,new BsonArray()},
                    { "Leaderboards",new BsonArray() },
                    { "Monetiz", new BsonDocument {
                        { "PaymentList",new BsonArray()},
                        { "Leaderboards", 3 },
                        { "Apis", 90000 },
                        { "Logs", 200 },
                        {"Achievements",4 },
                        {"Cash",0 },
                        { "Players", 5000 } } }
                };


                await Client.GetDatabase(NameDataBase).GetCollection<BsonDocument>("Setting").InsertOneAsync(ModelDataBase);


                //Step2:
                UpdateDefinitionBuilder<BsonDocument> Update = new UpdateDefinitionBuilder<BsonDocument>();

                await Client.GetDatabase(UsersDB).GetCollection<BsonDocument>(UsersDB).FindOneAndUpdateAsync(new BsonDocument("AccountSetting.Token", Token), Update.Push("Games", NameDataBase));

                return NameDataBase;
            }
            else
            {
                return "";
            }

        }

        public async Task<BsonDocument> ReciveStudios(string Token)
        {
            if (await CheackToken(Token))
            {
                try
                {
                    var Pipe = new[]
                    {
                        new BsonDocument{{"$match",new BsonDocument { {"AccountSetting.Token",Token } } }},
                        new BsonDocument{{"$project",new BsonDocument { {"Games",1 } } }}
                    };

                    var Games = await Client.GetDatabase("Users").GetCollection<BsonDocument>("Users").AggregateAsync<BsonDocument>(Pipe).Result.SingleAsync();

                    var FinalResult = new BsonDocument()
                    {
                        {"Studios",new BsonArray() }
                    };


                    foreach (var item in Games["Games"].AsBsonArray)
                    {
                        var Pipe1 = new[]
                        {

                            new BsonDocument{{"$project",new BsonDocument { {"Setting",1 }, { "_id", 0} } }}
                        };

                        var Setting = await Client.GetDatabase(item.AsString).GetCollection<BsonDocument>("Setting").AggregateAsync<BsonDocument>(Pipe1).Result.SingleAsync();
                        FinalResult["Studios"].AsBsonArray.Add(Setting);
                    };

                    FinalResult.Add("ServerTime", DateTime.Now);
                    return FinalResult;
                }
                catch (Exception ex)
                {

                    return new BsonDocument("ERR", ex.Message);
                }
            }
            else
            {
                return new BsonDocument("err", 2);
            }

        }

        public async Task<bool> Delete(string Token, string NameStudio)
        {
            if (await CheackToken(Token))
            {
                var Filter = new BsonDocument { { "AccountSetting.Token", Token } };
                var Update = new UpdateDefinitionBuilder<BsonDocument>().Pull("Games", NameStudio);

                await Client.DropDatabaseAsync(NameStudio);

                await Client.GetDatabase(UsersDB).GetCollection<BsonDocument>(UsersCollection).FindOneAndUpdateAsync(Filter, Update);

                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<string> Status(string Token, string NameStudio)
        {
            if (await CheackToken(Token))
            {
                Command<BsonDocument> Command = new BsonDocument { { "dbStats", 1 } };

                var result = await Client.GetDatabase(NameStudio).RunCommandAsync(Command);

                return result.ToString();
            }
            else
            {
                return "";
            }

        }

        public async Task<string> ReciveMonetize(string Token, string NameStudio)
        {
            if (await CheackToken(Token))
            {

                var Option = new FindOptions<BsonDocument>();
                Option.Projection = new BsonDocument { { "Monetiz", 1 } };
                var Setting = await Client.GetDatabase(NameStudio).GetCollection<BsonDocument>("Setting").FindAsync(new BsonDocument { { "_id", "Setting" } }, Option).Result.SingleAsync();




                return Setting["Monetiz"].AsBsonDocument.ToString();
            }
            else
            {
                return "";
            }
        }

        public async Task<bool> AddPayment(string Token, string NameStudio, string DetailMonetize)
        {
            try
            {
                if (await CheackToken(Token))
                {
                    var deserilse = BsonDocument.Parse(DetailMonetize);


                    var Update1 = new UpdateDefinitionBuilder<BsonDocument>().
                        Set("Monetiz.Leaderboards", deserilse["Leaderboards"]).
                        Set("Monetiz.Apis", deserilse["Apis"]).
                         Set("Monetiz.Logs", deserilse["Logs"]).
                         Set("Monetiz.Players", deserilse["Players"]).
                         Set("Monetiz.Achievements", deserilse["Achievements"]).
                    Set("Monetiz.Cash", deserilse["Cash"]);

                    await Client.GetDatabase(NameStudio).GetCollection<BsonDocument>("Setting").UpdateOneAsync(new BsonDocument { { "_id", "Setting" } }, Update1);


                    deserilse.Add("ID", new Random().Next());
                    deserilse.Add("Created", DateTime.Now);
                    var Update = new UpdateDefinitionBuilder<BsonDocument>().Push("Monetiz.PaymentList", deserilse);
                    await Client.GetDatabase(NameStudio).GetCollection<BsonDocument>("Setting").UpdateOneAsync(new BsonDocument { { "_id", "Setting" } }, Update);


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

        public async Task<BsonDocument> RecivePaymentList(string Token, string NameStudio)
        {
            if (await CheackToken(Token))
            {
                var pipe = new[]
                {
                    new BsonDocument{ {"$unwind","$Monetiz.PaymentList" } },
                    new BsonDocument{{"$sort",new BsonDocument { {"Monetiz.PaymentList.Created", -1 } } } },
                    new BsonDocument{{"$group",new BsonDocument { {"_id","$_id" },{"Detail",new BsonDocument { {"$push", "$Monetiz.PaymentList" } } } } }}
                };

                var Result = await Client.GetDatabase(NameStudio).GetCollection<BsonDocument>("Setting").AggregateAsync<BsonDocument>(pipe).Result.SingleAsync();


                return Result;
            }
            else
            {
                return new BsonDocument();
            }
        }

        public async Task<BsonDocument> ReciveSetting(string Token, string Studio)
        {
            if (await CheackToken(Token))
            {
                var Option = new FindOptions<BsonDocument>() { Projection = new BsonDocument { { "Setting", 1 } } };
                var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").FindAsync(new BsonDocument { { "_id", "Setting" } }, Option).Result.SingleAsync();

                return Result;
            }
            else
            {
                return new BsonDocument();
            }
        }

        public async Task<bool> GenerateNewToken(string Token)
        {
            if (await CheackToken(Token))
            {
                try
                {

                    var NewToken = ObjectId.GenerateNewId().ToString();

                    var Filter = new BsonDocument { { "AccountSetting.Token", Token } };
                    var Update = new UpdateDefinitionBuilder<BsonDocument>().Set("AccountSetting.Token", NewToken);
                    await Client.GetDatabase("Users").GetCollection<BsonDocument>("Users").UpdateOneAsync(Filter, Update);

                    var SettingUser = await Client.GetDatabase("Users").GetCollection<BsonDocument>("Users").FindAsync(new BsonDocument { { "AccountSetting.Token", NewToken } }).Result.SingleAsync();

                    foreach (var Studios in SettingUser["Games"].AsBsonArray)
                    {
                        var Update1 = new UpdateDefinitionBuilder<BsonDocument>().Set("Setting.Creator", NewToken);

                        await Client.GetDatabase(Studios.AsString).GetCollection<BsonDocument>("Setting").UpdateOneAsync(new BsonDocument { { "_id", "Setting" } }, Update1);
                    }


                    //send Email
                    try
                    {

                        var Body = $"Hi Dear " +
                            $"Your token was successfully changed in all studios" +
                            "\n" +
                            $"Previous token: {Token}" +
                            "\n" +
                            $"New token: {NewToken}" +
                            "\n\n" +
                            $"Thanks" +
                            "\n" +
                            $"Backendi.ir";
                        SendMail_Info(new System.Net.Mail.MailMessage("info@backendi.ir", SettingUser["AccountSetting"]["Email"].AsString, "Token Change", Body));

                    }
                    catch (Exception)
                    {

                    }
                    return true;
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
