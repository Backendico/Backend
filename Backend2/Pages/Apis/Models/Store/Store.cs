using MongoDB.Bson;
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
    }
}
