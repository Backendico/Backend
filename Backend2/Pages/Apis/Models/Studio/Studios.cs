using MongoDB.Bson;
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

               ,{ "Leaderboards",new BsonDocument{ {"List",new BsonDocument { } } } },
                    { "Monetiz", new BsonDocument {
                    { "PaymentList",new BsonArray()},
                    { "Leaderboards", 3 },
                     { "Apis", 90000 },
                      { "Studios", 2 },
                       { "Logs", 200 },
                        {"Cash",0 },
                    { "Players", 5000 } } } };

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

        public async Task<string> ReciveStudios(string Token)
        {
            if (await CheackToken(Token))
            {
                var Option = new FindOptions<BsonDocument>();
                Option.Projection = new BsonDocument { { "Games", 1 }, { "_id", 0 } };


                var filter = new BsonDocument { { "AccountSetting.Token", Token } };

                var ResultFind = await Client.GetDatabase(UsersDB).GetCollection<BsonDocument>(UsersCollection).FindAsync(filter, Option).Result.SingleAsync();


                var finalResult = new BsonDocument() { { "Settings", new BsonArray() } };
                foreach (var StudioName in ResultFind["Games"].AsBsonArray)
                {
                    var Option1 = new FindOptions<BsonDocument>();
                    Option.Projection = new BsonDocument { { "Setting", 1 } };
                    var FilterSettingFind = new BsonDocument { { "_id", "Setting" } };

                    var Setting = await Client.GetDatabase(StudioName.AsString).GetCollection<BsonDocument>("Setting").FindAsync(FilterSettingFind, Option1).Result.SingleAsync<BsonDocument>();


                    finalResult["Settings"].AsBsonArray.Add(Setting);
                }

                return finalResult.ToString();
            }
            else
            {
                return "";
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
                         Set("Monetiz.Studios", deserilse["Studios"]).
                         Set("Monetiz.Logs", deserilse["Logs"]).
                         Set("Monetiz.Players", deserilse["Players"]).
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
    }
}
