using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
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

        internal async Task<bool> AddProduct(string Token, string Studio, ObjectId Token_Studio, BsonDocument Detail)
        {
            if (await CheackToken(Token))
            {
                var Update = new UpdateDefinitionBuilder<BsonDocument>().Push("Store.$[f].Products", Detail);

                var arrayFilters = new[]
                {
                    new BsonDocumentArrayFilterDefinition<BsonDocument>(
                        new BsonDocument("f.Token",
                        new BsonDocument("$in", new BsonArray(new [] { Token_Studio })))),
                };

                var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").UpdateOneAsync(new BsonDocument { { "_id", "Setting" } }, Update, new UpdateOptions { ArrayFilters = arrayFilters });

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


        internal async Task<BsonDocument> ReciveProduct(string Token, string Studio, string TokenProduct)
        {

            if (await CheackToken(Token))
            {

                var Filter = new BsonDocument[]
                {
                    new BsonDocument{ {"$project",new BsonDocument { { "Store",1} } } },
                    new BsonDocument{ {"$unwind",new BsonDocument { {"path","$Store" } } } },
                    new BsonDocument{{"$match",new BsonDocument { { "Store.Token", ObjectId.Parse(TokenProduct) } } }
                    }
                };

                var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").AggregateAsync<BsonDocument>(Filter).Result.SingleAsync();

                return Result;
            }
            else
            {
                return new BsonDocument();
            }
        }

    }
}
