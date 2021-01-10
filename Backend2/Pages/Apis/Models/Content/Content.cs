using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualBasic.CompilerServices;
using MongoDB.Bson;
using MongoDB.Bson.IO;
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

        public async Task<bool> EditContent(string Token, string Studio, ObjectId TokenContent, BsonDocument Detail)
        {
            if (await CheackToken(Token))
            {
                try
                {
                    var Update = Builders<BsonDocument>.Update.Set("Content.$[f]", Detail);

                    var FilterArray = new[] { new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("f.Settings.Token", new BsonDocument("$in", new BsonArray(new[] { TokenContent })))) };

                    var result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").UpdateOneAsync(new BsonDocument { { "_id", "Setting" } }, Update, new UpdateOptions { ArrayFilters = FilterArray });


                    return result.ModifiedCount >= 1;
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


        public async Task<bool> DeleteContent(string Token, string Studio, ObjectId TokenContent)
        {
            if (await CheackToken(Token))
            {
                try
                {
                    var Update = Builders<BsonDocument>.Update.Pull("Content", new BsonDocument { { "Settings.Token", TokenContent } });

                    var result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").UpdateOneAsync(new BsonDocument { { "_id", "Setting" } }, Update);

                    return result.ModifiedCount >= 1;
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

        public async Task<BsonDocument> ReceiveContent(string Token, string Studio, ObjectId TokenContent)
        {
            if (await CheackToken(Token))
            {

                try
                {
                    var Pipe = new[]
                    {
                        new BsonDocument{{"$unwind","$Content"}},
                        new BsonDocument{{"$match",new BsonDocument { {"Content.Settings.Token",TokenContent } } }},
                        new BsonDocument{{"$project",new BsonDocument { {"Content",1 } } }}
                    };

                    var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").AggregateAsync<BsonDocument>(Pipe).Result.SingleAsync();


                    var Update = Builders<BsonDocument>.Update.Inc("Content.$[f].Settings.Index",1);

                    var FilterArray = new[] { new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("f.Settings.Token", new BsonDocument("$in", new BsonArray(new[] {TokenContent })))) };

                    await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").UpdateOneAsync(new BsonDocument { { "_id", "Setting" } }, Update,new UpdateOptions {ArrayFilters=FilterArray });


                    return Result;

                }
                catch (Exception)
                {
                    return new BsonDocument();
                }
            }
            else
            {
                return new BsonDocument();
            }

        }
   
    }
}
