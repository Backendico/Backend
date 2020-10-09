using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Backend2.Pages.AdminApis.SubpageSupport;
using Backend2.Pages.Apis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend2.Pages.AdminApis.SubpageStatices
{
    [Controller]
    public class PageStatices : APIBase
    {
        [HttpPost]
        public async Task<string> ReciveStatices()
        {
            var Statices = new BsonDocument
            {
                { "Users", 0 },
                {"Emails",new BsonDocument{{"Send",0 },{"Register",0 } } },
                {"BugReport" ,0 },
                {"Supports",0},
                {"Cash",0}
            };

            //Users
            {
                Statices["Users"] = await Client.GetDatabase(UsersDB).GetCollection<BsonDocument>(UsersCollection).CountDocumentsAsync("{}");
            }

            //Email 
            {
                var Emails = await Client.GetDatabase(UsersDB).GetCollection<BsonDocument>("Emails").FindAsync("{}").Result.ToListAsync();

                Statices["Emails"]["Send"] = Emails.Count;

                foreach (var item in Emails)
                {
                    var filter = new BsonDocument { { "AccountSetting.Email", item["Email"] } };

                    var Result = await Client.GetDatabase(UsersDB).GetCollection<BsonDocument>(UsersCollection).FindAsync(filter).Result.ToListAsync();

                    if (Result.Count >= 1)
                    {
                        Statices["Emails"]["Register"] = Statices["Emails"]["Register"].AsInt32 + 1;
                    }
                }

            }

            //Bugs
            {
                Statices["BugReport"] = await Client.GetDatabase(UsersDB).GetCollection<BsonDocument>("Bugs").CountDocumentsAsync("{}");
            }

            //Support
            {
                Statices["Supports"] = await SubpageSupport.SubpageSupport.SupportCount();
            }

            //Cash Cheack
            {
                var Option = new FindOptions<BsonDocument>() { Projection = new BsonDocument { { "Games", 1 } } };

                var Users = await Client.GetDatabase("Users").GetCollection<BsonDocument>("Users").FindAsync("{}", Option).Result.ToListAsync();

                //findusers
                foreach (var User in Users)
                {
                    //findgames
                    foreach (var Studio in User["Games"].AsBsonArray)
                    {
                        var Option1 = new FindOptions<BsonDocument>() { Projection = new BsonDocument { { "Monetiz.Cash", 1 } } };
                        var Setting = await Client.GetDatabase(Studio.AsString).GetCollection<BsonDocument>("Setting").FindAsync(new BsonDocument { { "_id", "Setting" } }, Option1).Result.SingleAsync();
                        Statices["Cash"] = Statices["Cash"].ToInt64() + Setting["Monetiz"]["Cash"].ToInt64();
                    }
                }
            }

            return Statices.ToString();
        }


    }
}
