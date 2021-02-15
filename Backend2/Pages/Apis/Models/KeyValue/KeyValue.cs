using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend2.Pages.Apis.Models.Leaderobard;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using StackExchange.Redis;

namespace Backend2.Pages.Apis.Models.KeyValue
{
    public class KeyValue : BasicAPIs
    {

        public async Task<bool> AddKey(string Token, string Studio, string Key, string Value)
        {
            if (await CheackToken(Token))
            {
                var Update = Builders<BsonDocument>.Update.Set($"KeyValue.{Key}", Value);

                var result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").UpdateOneAsync(new BsonDocument { { "_id", "Setting" } }, Update);

                if (result.ModifiedCount >= 1)
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


        public async Task<BsonDocument> ReceiveKeys(string Token, string Studio)
        {
            if (await CheackToken(Token))
            {
                try
                {
                    var Pipe = new[]
                    {
                        new BsonDocument{{"$project",new BsonDocument { { "KeyValue", 1 } } }}
                    };

                    var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").AggregateAsync<BsonDocument>(Pipe).Result.SingleAsync();


                    return Result;
                }
                catch (Exception ex)
                {
                    return new BsonDocument("EX", ex.Message);
                }
            }
            else
            {
                return new BsonDocument("ERR", "TokenERR");
            }
        }

        public async Task<bool> RemoveKey(string Token, string Studio, string Key)
        {
            if (await CheackToken(Token))
            {
                var Update = Builders<BsonDocument>.Update.Unset($"KeyValue.{Key}");
                var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").UpdateOneAsync(new BsonDocument { { "_id", "Setting" } }, Update);

                return Result.ModifiedCount >= 1;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> UpdateKey(string Token, string Studio, string Key, string Value)
        {
            if (await CheackToken(Token))
            {
                var Update = new UpdateDefinitionBuilder<BsonDocument>().Set($"KeyValue.{Key}", Value);

                var result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").UpdateOneAsync(new BsonDocument { { "_id", "Setting" } }, Update);

                return result.ModifiedCount >= 1;
            }
            else
            {
                return false;
            }

        }


        public async Task<BsonDocument> ReceiveValue(string Token, string Studio, string Key)
        {
            if (await CheackToken(Token))
            {
                try
                {

                    var Pipe = new[]
                    {
                        new BsonDocument{ {"$project",new BsonDocument { {$"KeyValue.{Key}",1 } } } }
                    };


                    var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").AggregateAsync<BsonDocument>(Pipe).Result.SingleAsync();

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
