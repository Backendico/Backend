using Backend2.Pages.Apis;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

#nullable enable
namespace Backend.Controllers.PageChoiceStudioGame
{
    [Controller]
    public class ChoiceStudioGame : APIBase
    {

        /// <summary>
        /// 1: Install database
        /// 2:insert databasename to game list
        /// </summary>
        /// <returns>
        /// Name database
        /// </returns>
        [HttpPost]
        public async Task<string?> CreatNewStudio(string NameStudio, string Token)
        {
            if (await CheackToken(Token))
            {
                // Step1:
                var NameDataBase = NameStudio + "+" + ObjectId.GenerateNewId();
                var ModelDataBase = new BsonDocument {
                { "_id","Setting"},
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

               ,{ "Leaderboards",new BsonDocument()},
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

                Response.StatusCode = Ok().StatusCode;


                return NameDataBase;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;

                return null;
            }

        }


        /// <summary>
        /// 1: AUT Token
        /// 2: find token 
        /// </summary>
        /// <returns>
        /// List Games Return
        /// </returns>
        [HttpPost]
        public async Task<string?> ReciveStudios(string Token)
        {
            if (await CheackToken(Token))
            {
                var Option = new FindOptions<BsonDocument>();
                Option.Projection = new BsonDocument { {"Games",1 },{"_id",0 } };


                var filter = new BsonDocument { { "AccountSetting.Token", Token } };

                var ResultFind = await Client.GetDatabase(UsersDB).GetCollection<BsonDocument>(UsersCollection).FindAsync(filter,Option).Result.SingleAsync();

             
                var finalResult = new BsonDocument() { { "Settings" , new BsonArray() }  };
                foreach (var StudioName in ResultFind["Games"].AsBsonArray)
                {
                    var Option1 = new FindOptions<BsonDocument>();
                    Option.Projection = new BsonDocument { {"Setting",1 } };
                    var FilterSettingFind = new BsonDocument { { "_id", "Setting" } };

                    var Setting = await Client.GetDatabase(StudioName.AsString).GetCollection<BsonDocument>("Setting").FindAsync(FilterSettingFind,Option1).Result.SingleAsync<BsonDocument>();


                    finalResult["Settings"].AsBsonArray.Add(Setting);
                }

                return finalResult.ToString();
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
                return null;
            }

        }


        [HttpDelete]
        public async Task<bool> Delete(string Token, string NameStudio)
        {
            if (await CheackToken(Token))
            {
                var Filter = new BsonDocument { { "AccountSetting.Token", Token } };
                var Update = new UpdateDefinitionBuilder<BsonDocument>().Pull("Games", NameStudio);

                await Client.DropDatabaseAsync(NameStudio);

                await Client.GetDatabase(UsersDB).GetCollection<BsonDocument>(UsersCollection).FindOneAndUpdateAsync(Filter, Update);
                Response.StatusCode = Ok().StatusCode;


                return true;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;


                return false;
            }
        }



        [HttpPost]
        public async Task<string?> Status(string Token, string NameStudio)
        {
            if (await CheackToken(Token))
            {
                Command<BsonDocument> Command = new BsonDocument { { "dbStats", 1 } };

                var result = await Client.GetDatabase(NameStudio).RunCommandAsync(Command);

                Response.StatusCode = Ok().StatusCode;

                return result.ToString();
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
                return null;
            }
        }


        [HttpPost]
        public async Task<string> ReciveMonetize(string Token, string NameStudio)
        {
            if (await CheackToken(Token))
            {

                var Option = new FindOptions<BsonDocument>();
                Option.Projection = new BsonDocument { { "Monetiz", 1 } };
                var Setting = await Client.GetDatabase(NameStudio).GetCollection<BsonDocument>("Setting").FindAsync(new BsonDocument { { "_id", "Setting" } }, Option).Result.SingleAsync();

                Response.StatusCode = Ok().StatusCode;
                return Setting["Monetiz"].AsBsonDocument.ToString();
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
                return "";
            }
        }

        [HttpPost]
        public async Task AddPayment(string Token, string NameStudio, string DetailMonetize)
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

                    Response.StatusCode = Ok().StatusCode;
                }
                else
                {
                    Response.StatusCode = BadRequest().StatusCode;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Response.StatusCode = BadRequest().StatusCode;
            }


        }

    }
}
