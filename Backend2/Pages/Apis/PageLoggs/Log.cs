using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Driver.Core.Operations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Backend2.Pages.Apis.PageLoggs
{
    [Controller]
    public class Log : APIBase
    {

        [HttpPost]
        public async Task<string> ReciveLogs(string Token, string Studio, string Count)
        {
            if (await CheackToken(Token))
            {
                var Pipe = new BsonDocument[]
                {
                    new BsonDocument{ {"$unwind","$Logs" } },
                    new BsonDocument{{"$sort",new BsonDocument { { "Logs.Time", -1 } } }},
                    new BsonDocument{ {"$limit",int.Parse(Count )} },
                    new BsonDocument{{"$group",new BsonDocument { {"_id","$_id" },{"Details",new BsonDocument { {"$push","$Logs" } } } } }}
                };


                var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").AggregateAsync<BsonDocument>(Pipe).Result.SingleAsync();

                Response.StatusCode = Ok().StatusCode;
                return Result.ToString();
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
                return "";
            }

        }

        [HttpPost]
        public async Task<string> AddLog(string Token, string Studio, string Header, string Description, string detail, string IsNotifaction)
        {
            if (await CheackToken(Token))
            {

                var DataUpdate = new BsonDocument
                {
                    { "Header",Header},
                    {"Description",Description },
                    {"Time",DateTime.Now },
                    {"Detail",BsonDocument.Parse(detail) },
                    {"IsNotifaction", bool.Parse(IsNotifaction)}
                };

                var Update = new UpdateDefinitionBuilder<BsonDocument>().Push("Logs", DataUpdate);
                await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").UpdateOneAsync(new BsonDocument { { "_id", "Setting" } }, Update);

                Response.StatusCode = Ok().StatusCode;
                return "";

            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;

                return "";

            }
        }


        [HttpDelete]
        public async Task DeleteLog(string Token, string Studio, string Detail)
        {
            if (await CheackToken(Token))
            {

                var DataUpdate = BsonDocument.Parse(Detail);

                var Update = new UpdateDefinitionBuilder<BsonDocument>().Pull("Logs", DataUpdate);
                await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").UpdateOneAsync(new BsonDocument { { "_id", "Setting" } }, Update);

                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }

        }
    }
}
