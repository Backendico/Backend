using MongoDB.Bson;
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
                var Option = new FindOptions<BsonDocument>() { Projection = new BsonDocument { { "Achievements", 1 }, { "_id", 0 } } };

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


        public async Task<bool> EditAchievements(string Token, string Studio, ObjectId TokenAchievements, BsonDocument Detail)
        {
            if (await CheackToken(Token))
            {
                var Update = Builders<BsonDocument>.Update.Set("Achievements.$[f].Value", Detail["Value"].ToInt64());
                var filterarra = new[]{
                new  BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument
                {
                    {"f.Token",new BsonDocument{ {"$in" ,new BsonArray(new[] {TokenAchievements })} } }
                }) };

                var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").UpdateOneAsync(new BsonDocument { { "_id", "Setting" } }, Update, options: new UpdateOptions() { ArrayFilters = filterarra });

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


        public async Task<bool> AddPlayerAchievements(string Token, string Studio, ObjectId TokenPlayer, BsonDocument Detail)
        {
            if (await CheackToken(Token))
            {

                try
                {
                    try
                    {
                        var Pipe = new[]
                        {
                new BsonDocument{{"$project",new BsonDocument { {"Account.Token",1 },{ "Achievements",1 } } }},
                new BsonDocument{{"$match",new BsonDocument { { "Account.Token", TokenPlayer } } } },
                new BsonDocument{ {"$unwind", "$Achievements" } },
                new BsonDocument{{"$match",new BsonDocument { {"Achievements.Token", Detail["Token"].AsObjectId} } }}
            };
                        var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").AggregateAsync<BsonDocument>(Pipe).Result.SingleAsync();

                        _ = Result.ElementCount >= 1;

                        return false;
                    }
                    catch (Exception)
                    {
                        var Filter = new BsonDocument { { "Account.Token", TokenPlayer } };
                        var Update = new UpdateDefinitionBuilder<BsonDocument>().Push<BsonDocument>("Achievements", Detail);
                        var resultPush = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").UpdateOneAsync(Filter, Update);

                        if (resultPush.ModifiedCount >= 1)
                        {
                            //return true;
                            return true;
                        }
                        else
                        {
                            return false;
                            //return false;
                        }
                    }


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

        public async Task<BsonDocument> RecivePlayersAchivementsList(string Token, string Studio, ObjectId TokenAchievement, int Count)
        {
            try
            {
                if (await CheackToken(Token))
                {
                    var Pipe = new[]
                    {
                    new BsonDocument{ {"$project",new BsonDocument { {"Token","$Account.Token" },{"Username","$Account.Username" },{ "Achievements",1} } } },
                    new BsonDocument{{"$unwind","$Achievements" } },
                    new BsonDocument{{"$match",new BsonDocument { {"Achievements.Token" ,TokenAchievement} } } },
                    new BsonDocument{{"$project",new BsonDocument { {"_id",0 },{ "Achievements", 0 } }} },
                    new BsonDocument{{"$limit",Count}},
                    new BsonDocument{{"$group",new BsonDocument { {"_id","List Achivements" },{"List",new BsonDocument { {"$push",new BsonDocument { {"Username","$Username" } ,{"Token","$Token" } } } } } } }}
                };

                    var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").AggregateAsync<BsonDocument>(Pipe).Result.SingleAsync();

                    return Result;
                }
                else
                {
                    return new BsonDocument();
                }
            }
            catch (Exception)
            {
                return new BsonDocument();
            }
        }


        public async Task<bool> Remove(string Token, string Studio, ObjectId TokenPlayer, BsonDocument Detail)
        {
            if (await CheackToken(Token))
            {

                var Filter = new BsonDocument { { "Account.Token", TokenPlayer } };
                var Update = new UpdateDefinitionBuilder<BsonDocument>().Pull<BsonDocument>("Achievements", Detail);
                var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").UpdateOneAsync(Filter, Update);

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
