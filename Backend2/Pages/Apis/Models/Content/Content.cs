using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualBasic.CompilerServices;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Driver.Core.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend2.Pages.Apis.Models.Content
{
    public class Content : BasicAPIs
    {

        public async Task<bool> AddContent(string Token, string Studio, string NameContent)
        {
            if (await CheackToken(Token))
            {
                try
                {
                    var Result = await CreatDoc();

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


            async Task<bool> CreatDoc()
            {

                var Doc = new BsonDocument
                {
                    {"Settings",new BsonDocument
                    {
                        {"Name",NameContent },
                        {"Token",ObjectId.GenerateNewId() },
                        {"Access",0} ,
                        {"Created",DateTime.Now },
                        {"Index",long.Parse("0") }
                    }},

                    {"Content",new BsonDocument() }
                };

                var Update = Builders<BsonDocument>.Update.Push("Content", Doc);

                try
                {
                    var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").UpdateOneAsync(new BsonDocument { { "_id", "Setting" } }, Update);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }


            }

        }

        public async Task<BsonDocument> RecieveContents(string Token, string Studio, int Count)
        {
            if (await CheackToken(Token))
            {
                try
                {

                    Count = Count >= 1 ? Count : 1;

                    var Pipe = new[]
                    {
                    new BsonDocument{ {"$project",new BsonDocument { {"Content",1 } } } },
                    new BsonDocument{{"$limit",Count}}
                };
                    var Contents = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").AggregateAsync<BsonDocument>(Pipe).Result.SingleAsync();
                    return Contents;
                }
                catch (Exception ex)
                {
                    return new BsonDocument("ERR", ex.Message);
                }
            }
            else
            {
                return new BsonDocument("Token", true);
            }

        }

        public async Task<BsonDocument> EditContent(string Token, string Studio, ObjectId TokenDetail, BsonDocument Detail)
        {
            try
            {

                var Update = Builders<BsonDocument>.Update.Set("Content.$[f]", Detail);

                var FilterArray = new[]
                {
                    new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument{ {"f.Setting.Token",new BsonDocument { {"$in",new BsonArray() {TokenDetail } } } } })
                };

                var result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").UpdateOneAsync(new BsonDocument { { "_id", "Setting" } }, Update, new UpdateOptions { ArrayFilters = FilterArray });

                return new BsonDocument("Result", result.ModifiedCount);
            }

            catch (Exception ex)
            {
                return new BsonDocument("ERR", ex.Message);

            }


        }
    }
}
