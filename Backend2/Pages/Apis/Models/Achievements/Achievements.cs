﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend2.Pages.Apis.Models.Achievements
{
    public class Achievements : BasicAPIs
    {

        public async Task<bool> AddAchievements(string Studio, string Token, BsonDocument Detail)
        {
            if (await CheackToken(Token))
            {
                Detail.Add("Created", DateTime.Now);
                Detail.Add("Token", ObjectId.GenerateNewId());

                var Update = new UpdateDefinitionBuilder<BsonDocument>().Push("Achievements", Detail);

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

        public async Task<BsonDocument> ReciveAchievements(string Token, string Studio)
        {
            if (await CheackToken(Token))
            {
                var Option = new FindOptions<BsonDocument>() { Projection = new BsonDocument { { "Achievements", 1 } ,{"_id",0 } } };

                var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").FindAsync(new BsonDocument { { "_id", "Setting" } }, Option).Result.SingleAsync();
               
                return Result;
            }
            else
            {
                return new BsonDocument();
            }

        }

        public async Task<bool> CheackAchievementsName(string Studio, string Token, string NameAchievements)
        {
            if (await CheackToken(Token))
            {
                var Pipe = new BsonDocument[]
                {
                    new BsonDocument{ {"$project",new BsonDocument { { "Achievements", 1 } } } },
                    new BsonDocument{ {"$unwind", "$Achievements" } },
                    new BsonDocument{ {"$match",new BsonDocument { { "Achievements.Name",NameAchievements } } } }
                };

                var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").AggregateAsync<BsonDocument>(Pipe).Result.SingleAsync();


                if (Result.ElementCount >= 1)
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