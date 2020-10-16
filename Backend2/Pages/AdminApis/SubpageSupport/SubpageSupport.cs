using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Backend2.Pages.AdminApis.ApisBasicAdmin;
using Backend2.Pages.Apis.PageSupport;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Operations;

namespace Backend2.Pages.AdminApis.SubpageSupport
{
    [Controller]
    public class SubpageSupport : BasicApiAdmin
    {
        [HttpPost]
        public async Task<string> ReciveSupports(string Token)
        {
            var Result = new BsonDocument { { "ListSupports", new BsonArray() } };

            var Option = new FindOptions<BsonDocument>() { Projection = new BsonDocument { { "Games", 1 }, { "AccountSetting", 1 } } };

            var Users = await Client.GetDatabase("Users").GetCollection<BsonDocument>("Users").FindAsync("{}", Option).Result.ToListAsync();

            //Find List Game
            foreach (var Games in Users)
            {
                //find list Studio
                foreach (var Studio in Games["Games"].AsBsonArray)
                {
                    var Pipe = new[]
                    {
                        new BsonDocument{ { "$unwind","$Support"} },
                        new BsonDocument{{"$match",new BsonDocument { {"Support.IsOpen",true} }} },
                        new BsonDocument{{"$project",new BsonDocument { { "Support", 1 } ,{"_id",0 } } } },
                        new BsonDocument{ { "$sort", new BsonDocument { { "Support.Priority", -1 } } } }
                    };

                    var Setting = await Client.GetDatabase(Studio.AsString).GetCollection<BsonDocument>("Setting").AggregateAsync<BsonDocument>(Pipe).Result.ToListAsync();

                    foreach (var Support in Setting)
                    {
                        Support["Support"].AsBsonDocument.Add("Creator", Games["AccountSetting"]["Token"]);
                        Support["Support"].AsBsonDocument.Add("Studio", Studio.ToString());
                        Result["ListSupports"].AsBsonArray.Add(Support["Support"]);
                    }

                }
            }

            //send Result
            if (Result["ListSupports"].AsBsonArray.Count >= 1)
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
            return Result.ToString();
        }

        [HttpPut]
        public async Task AddSupportMessage(string Token,string TokenUser,string TokenSupport, string Studio, string MessageDetail)
        {
            var SerilseMessage = BsonDocument.Parse(MessageDetail);
            SerilseMessage.Add("Created", DateTime.Now);

            if (await PageSupport.AddMessageToSupport(TokenSupport, Studio, SerilseMessage))
            {
                Response.StatusCode = Ok().StatusCode;

                //send Signal
                SignalNotifaction(TokenUser);
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }


        [HttpDelete]
        public async Task BlockSupport(string Token,string TokenSupport, string Studio)
        {

            if (await PageSupport.CloseSupport(TokenSupport, Studio))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }

        }

       
        public static async Task<int> SupportCount()
        {

            MongoClient Client = new MongoClient();

            var Result = new BsonDocument { { "ListSupports", new BsonArray() } };

            var Option = new FindOptions<BsonDocument>() { Projection = new BsonDocument { { "Games", 1 }, { "AccountSetting", 1 } } };

            var Users = await Client.GetDatabase("Users").GetCollection<BsonDocument>("Users").FindAsync("{}", Option).Result.ToListAsync();

            //Find List Game
            foreach (var Games in Users)
            {
                //find list Studio
                foreach (var Studio in Games["Games"].AsBsonArray)
                {
                    var Pipe = new[]
                    {
                        new BsonDocument{ { "$unwind","$Support"} },
                        new BsonDocument{{"$match",new BsonDocument { {"Support.IsOpen",true} }} },
                        new BsonDocument{{"$project",new BsonDocument { { "Support", 1 } ,{"_id",0 } } } }
                    };

                    var Setting = await Client.GetDatabase(Studio.AsString).GetCollection<BsonDocument>("Setting").AggregateAsync<BsonDocument>(Pipe).Result.ToListAsync();

                    foreach (var Support in Setting)
                    {
                        Support["Support"].AsBsonDocument.Add("Creator", Games["AccountSetting"]["Token"]);
                        Result["ListSupports"].AsBsonArray.Add(Support["Support"]);

                    }

                }
            }

            return Result["ListSupports"].AsBsonArray.Count;

        }

    }
}
