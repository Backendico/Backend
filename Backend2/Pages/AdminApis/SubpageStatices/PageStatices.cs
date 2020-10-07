using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
                { "Users", new BsonDocument{ {"Count",0 } } },
                {"Emails",new BsonDocument{{"Send",0 },{"Register",0 } } },
                {"BugReport" ,new BsonDocument{ { "Count",0} } },
                {"Supports",new BsonDocument{ {"Count",0 } } },
                {"Cash",new BsonDocument{ {"Block" ,0} ,{"Cash",0 } } }
            };

            //Users
            {
                Statices["Users"]["Count"] = await Client.GetDatabase(UsersDB).GetCollection<BsonDocument>(UsersCollection).CountDocumentsAsync("{}");
            }


            //Email 
            {
                var Emails = await Client.GetDatabase(UsersDB).GetCollection<BsonDocument>("Emails").FindAsync("{}").Result.ToListAsync();

                Statices["Emails"]["Send"] = Emails.Count;

                foreach (var item in Emails)
                {
                    var filter = new BsonDocument { {"AccountSetting.Email",item["Email"] } };

                   var Result= await Client.GetDatabase(UsersDB).GetCollection<BsonDocument>(UsersCollection).FindAsync(filter).Result.ToListAsync();
             
                    if (Result.Count>=1)
                    {
                        Statices["Emails"]["Register"] = Statices["Emails"]["Register"].AsInt32 + 1;
                    }
                }

            }

            //Bugs
            {
                Statices["BugReport"]["Count"] = await Client.GetDatabase(UsersDB).GetCollection<BsonDocument>("Bugs").CountDocumentsAsync("{}");
            }

            //Support
            {
                Statices["Supports"]["Count"] = await Client.GetDatabase(UsersDB).GetCollection<BsonDocument>("Support").CountDocumentsAsync("{}");
            }

            //Cash Cheack
            {


            }

            return Statices.ToString();
        }


    }
}
