using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Backend2.Pages.Apis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

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


            //Email Cheack
            {
                Statices["Emails"]["Send"] = await Client.GetDatabase(AdminDB).GetCollection<BsonDocument>("Emails").CountDocumentsAsync("{}");

                if (Statices["Emails"]["Send"] >= 1)
                {
                    Debug.WriteLine("find ");
                }

            }

            //Bugs
            {
                Statices["BugReport"]["Count"] = await Client.GetDatabase(AdminDB).GetCollection<BsonDocument>("Bugs").CountDocumentsAsync("{}");
            }

            //Support
            {
                Statices["Supports"]["Count"] = await Client.GetDatabase(AdminDB).GetCollection<BsonDocument>("Support").CountDocumentsAsync("{}");
            }

            //Cash Cheack
            {


            }

            return Statices.ToString();
        }


    }
}
