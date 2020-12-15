using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Driver.Core.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend2.Pages.Apis.Models.Store
{
    public class Store : BasicAPIs
    {
        internal async Task<bool> AddStore(string Token, string Studio, BsonDocument Detail)
        {
            if (await CheackToken(Token))
            {
                Detail.Add(new BsonElement("Token", ObjectId.GenerateNewId()));
                Detail.Add(new BsonElement("Created", DateTime.Now));

                var Update = new UpdateDefinitionBuilder<BsonDocument>().Push("Store", Detail);

                var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").UpdateOneAsync(new BsonDocument { { "_id", "Setting" } }, Update);

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

        internal async Task<BsonDocument> ReciveStores(string Token, string Studio)
        {
            if (await CheackToken(Token))
            {

                var Option = new FindOptions<BsonDocument>() { Projection = new BsonDocument { { "Store", 1 } } };

                var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").FindAsync(new BsonDocument { { "_id", "Setting" } }, Option).Result.SingleAsync();

                return Result;
            }
            else
            {
                return new BsonDocument();
            }
        }

        internal async Task<bool> SaveStore(string Token, string Studio, ObjectId TokenStore, string DetailStore)
        {
            if (await CheackToken(Token))
            {

                var update = new UpdateDefinitionBuilder<BsonDocument>().Set("Store.$[f]", BsonDocument.Parse(DetailStore));

                var Arrayfilter = new[]
                {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument{
                {"f.Token",new BsonDocument{ {"$in",new BsonArray(new[] {TokenStore }) } } }
                })
                };

                var result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").UpdateOneAsync(new BsonDocument { { "_id", "Setting" } }, update, new UpdateOptions() { ArrayFilters = Arrayfilter });
                if (result.MatchedCount >= 1)
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


        public async Task<BsonDocument> ReciveAbstractStores(string Token, string Studio)
        {
            if (await CheackToken(Token))
            {
                var pipe = new[]
                {
                    new BsonDocument{ {"$unwind","$Store" } },
                    new BsonDocument{ {"$project",new BsonDocument { {"Store.Products",0 } } } },
                    new BsonDocument{{"$group",new BsonDocument { {"_id","$_id" },{"Stores",new BsonDocument { {"$push","$Store" } } } } }}
                };

                var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").AggregateAsync<BsonDocument>(pipe).Result.SingleAsync();
                return Result;
            }
            else
            {
                return new BsonDocument();
            }
        }

        public async Task<BsonDocument> ReciveProducts(string Token, string Studio, ObjectId TokenStore)
        {
            if (await CheackToken(Token))
            {

                var Pipe = new[]
                {
                    new BsonDocument{ {"$unwind","$Store" } },
                    new BsonDocument{ { "$match",new BsonDocument{ { "Store.Token", TokenStore } } } },
                    new BsonDocument{{"$project",new BsonDocument { {"Store.Products", 1 } } }}
                };

                var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").AggregateAsync<BsonDocument>(Pipe).Result.SingleAsync();

                return Result;
            }
            else
            {
                return new BsonDocument();
            }
        }

        public async Task<bool> AddPayment(string Token,string Studio,ObjectId TokenStore,ObjectId TokenPlayer)
        {

        }

    }
}
