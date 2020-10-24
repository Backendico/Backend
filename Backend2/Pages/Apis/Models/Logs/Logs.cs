using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend2.Pages.Apis.Models.Logs
{
    public class Logs : BasicAPIs
    {
        public async Task<BsonDocument> ReciveLogs(string Token, string Studio, string Count)
        {
            if (await CheackToken(Token))
            {
                var Pipe = new BsonDocument[]
                {
                    new BsonDocument{ {"$unwind","$Logs" } },
                    new BsonDocument{{"$sort",new BsonDocument { { "Logs.Time", -1 } } }},
                    new BsonDocument{ {"$limit",int.Parse(Count )} },
                    new BsonDocument{{"$group",new BsonDocument { {"_id","$_id" },{"Details",new BsonDocument { {"$push","$Logs" } } } } }}
                };

                var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").AggregateAsync<BsonDocument>(Pipe).Result.SingleAsync();

                //add ReadWrite
                await ReadWriteControll(Studio, API.Read);

                return Result;
            }
            else
            {
                return new BsonDocument();
            }

        }

        public async Task<bool> AddLog(string Token, string Studio, string Header, string Description, string detail, string IsNotifaction)
        {
            if (await CheackToken(Token))
            {

                var DataUpdate = new BsonDocument
                {
                    { "Header",Header},
                    {"Description",Description },
                    {"Time",DateTime.Now },
                    {"Detail",BsonDocument.Parse(detail) },
                    {"IsNotifaction", bool.Parse(IsNotifaction)}
                };

                if (DataUpdate["IsNotifaction"].AsBoolean)
                {
                    SignalNotifaction(Token);
                }

                var Update = new UpdateDefinitionBuilder<BsonDocument>().Push("Logs", DataUpdate);
                var result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").UpdateOneAsync(new BsonDocument { { "_id", "Setting" } }, Update);


                //add ReadWrite
                await ReadWriteControll(Studio, API.Write);

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


        public async Task<bool> DeleteLog(string Token, string Studio, string Detail)
        {
            if (await CheackToken(Token))
            {

                var DataUpdate = BsonDocument.Parse(Detail);

                var Update = new UpdateDefinitionBuilder<BsonDocument>().Pull("Logs", DataUpdate);
                var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").UpdateOneAsync(new BsonDocument { { "_id", "Setting" } }, Update);

                //add ReadWrite
                await ReadWriteControll(Studio, API.Write);

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
