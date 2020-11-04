using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using System.Diagnostics;
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
                //recive  achievements  Setting
                var Option = new FindOptions<BsonDocument>() { Projection = new BsonDocument { { "Achievements", 1 }, { "_id", 0 } } };

                var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").FindAsync(new BsonDocument { { "_id", "Setting" } }, Option).Result.SingleAsync();

                // recive count Player

                try
                {
                    for (int i = 0; i < Result["Achievements"].AsBsonArray; i++)
                    {
                        var Pipe1 = new[]
                        {
                            new BsonDocument{ { "$project" ,new BsonDocument { {"_id",0 },{"Achievements",1 } } } },
                            new BsonDocument{{"$unwind","$Achievements" }},new BsonDocument{{"$match",new BsonDocument { {"Achievements.Token",Result["Achievements"][i]["Token"].AsObjectId } } }},
                        new BsonDocument{{"$count","Achievements"}}
                    };
                        try
                        {
                            var CountPlayer = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").AggregateAsync<BsonDocument>(Pipe1).Result.ToListAsync();

                            if (CountPlayer.Count >= 1)
                            {
                                Result["Achievements"].AsBsonArray[i].AsBsonDocument.Add(new BsonElement("Players", CountPlayer[0]));

                            }
                            else
                            {
                                Result["Achievements"].AsBsonArray[i].AsBsonDocument.Add(new BsonElement("Players", 0));
                            }
                        }
                        catch (Exception)
                        {
                            Result["Achievements"].AsBsonArray[i].AsBsonDocument.Add(new BsonElement("Players", 0));
                        }
                    }

                }
                catch (Exception)
                {


                }

                //total player
                try
                {
                    var TotalPlayer = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").CountDocumentsAsync("{}");

                    Result.Add(new BsonElement("TotalPlayer", TotalPlayer));
                }
                catch (Exception)
                {
                    Result.Add(new BsonElement("TotalPlayer", 0));
                }


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
                        Detail.Add("Recive", DateTime.Now);
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
                    new BsonDocument{{"$project",new BsonDocument { {"_id",0 },{ "Achievements", 1 },{"Username",1 },{"Token" ,1} }} },
                    new BsonDocument{{"$limit",Count}},
                    new BsonDocument{{"$group",new BsonDocument { {"_id","List Achivements" },{"List",new BsonDocument { {"$push",new BsonDocument { {"Recive", "$Achievements.Recive" }, {"Username","$Username" } ,{"Token","$Token" } } } } } } }}
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
            try
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
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RemoveAchievements(string Token, string Studio, BsonDocument Detail)
        {
            if (await CheackToken(Token))
            {
                var SerilseDetail = new BsonDocument
                {
                    {"Token",Detail["Token"] },
                    {"Name",Detail["Name"] }
                };
                var Update = new UpdateDefinitionBuilder<BsonDocument>().Pull("Achievements", SerilseDetail);

                var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").UpdateManyAsync("{}", Update);

                if (Result.ModifiedCount >= 1)
                {
                    var Update1 = new UpdateDefinitionBuilder<BsonDocument>().Pull("Achievements", Detail);

                    var Result1 = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").UpdateManyAsync(new BsonDocument { { "_id", "Setting" } }, Update1);

                    if (Result1.ModifiedCount >= 1)
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
                    var Update1 = new UpdateDefinitionBuilder<BsonDocument>().Pull("Achievements", Detail);

                    var Result1 = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").UpdateManyAsync(new BsonDocument { { "_id", "Setting" } }, Update1);

                    if (Result1.ModifiedCount >= 1)
                    {
                        return true;

                    }
                    else
                    {

                        return false;
                    }
                }
            }
            else
            {
                return false;
            }


        }


        public async Task<BsonDocument> PlayerAchievements(string Token, string Studio, ObjectId TokenPlayer)
        {
            if (await CheackToken(Token))
            {
                try
                {
                    var Filter = new BsonDocument { { "Account.Token", TokenPlayer } };
                    var Option = new FindOptions<BsonDocument>() { Projection = new BsonDocument { { "_id", 0 }, { "Achievements", 1 } } };

                    var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync(Filter, Option).Result.SingleAsync();

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
