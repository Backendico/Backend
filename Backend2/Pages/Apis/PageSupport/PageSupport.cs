﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Backend2.Pages.Apis.PageSupport.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Backend2.Pages.Apis.PageSupport
{
    [Controller]
    public class PageSupport : APIBase
    {

        [HttpPost]
        public async Task AddSupport(string Token, string Studio, string Detail)
        {
            if (await CheackToken(Token))
            {

                var deseriledata = BsonDocument.Parse(Detail);
                deseriledata.Add("Created", DateTime.Now);
                deseriledata.Add("IsOpen", true);
                deseriledata.Add("Messages", new BsonArray());
                deseriledata.Add("Token", ObjectId.GenerateNewId());

                var Update = new UpdateDefinitionBuilder<BsonDocument>().Push("Support", deseriledata);

                await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").UpdateOneAsync(new BsonDocument { { "_id", "Setting" } }, Update);

                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }

        [HttpPost]
        public async Task<string> ReciveSupports(string Token, string Studio)
        {
            var Pipe = new[]
            {
                new BsonDocument { {"$unwind","$Support" } },
                new BsonDocument{ {"$sort",new BsonDocument { {"Support.IsOpen",-1 },{"Support.Created",-1 } } } },
                new BsonDocument{{"$group",new BsonDocument { {"_id","$_id" },{"Detail",new BsonDocument { {"$push", "$Support" } } } } }}
            };


            var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").AggregateAsync<BsonDocument>(Pipe).Result.SingleAsync();

            return Result.ToString();
        }

        [HttpPut]
        public async Task AddMessage(string Token, string Studio, string TokenSupport, string Detail)
        {

            if (await CheackToken(Token))
            {
                var update = Builders<BsonDocument>.Update.Push("Support.$[f].Messages", BsonDocument.Parse(Detail));

                var arrayFilters = new[]
                {new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("f.Token",new BsonDocument("$in",new BsonArray{ {new ObjectId(TokenSupport)} }))),};


                await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").UpdateOneAsync(new BsonDocument { { "_id", "Setting" } }, update, new UpdateOptions { ArrayFilters = arrayFilters });
                Response.StatusCode = Ok().StatusCode;

            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }

        }

        [HttpPost]
        public async Task CloseSupport(string Token,string Studio,string TokenSupport)
        {
            var update = new BsonDocument
            {
                {"$set",new BsonDocument{ {"Support.$[f].IsOpen",false } } }
            };

            var arrayfilter = new[]
            {
            new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument{ {"f.Token" ,new BsonDocument { { "$in",new BsonArray { ObjectId.Parse(TokenSupport) } } } } })
            };

            await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").UpdateOneAsync(new BsonDocument { { "_id", "Setting" } },update,new UpdateOptions {ArrayFilters=arrayfilter });
        }

    }
}
