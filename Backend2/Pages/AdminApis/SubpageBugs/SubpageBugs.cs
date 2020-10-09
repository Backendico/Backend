using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend2.Pages.AdminApis.ApisBasicAdmin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend2.Pages.AdminApis.SubpageBugs
{
    [Controller]
    public class SubpageBugs : BasicApiAdmin
    {
        public async Task<string> ReciveBugs(string Token)
        {
            var Result = new BsonDocument { { "ListBugs", new BsonArray() } };
            var ListBugs = await Client.GetDatabase("Users").GetCollection<BsonDocument>("Bugs").FindAsync("{}").Result.ToListAsync();

            if (ListBugs.Count >= 1)
            {
                foreach (var Bugs in ListBugs)
                {
                    Result["ListBugs"].AsBsonArray.Add(Bugs);
                }
               
                Response.StatusCode = Ok().StatusCode;
                return Result.ToString();
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
                return Result.ToString();
            }


        }
    }
}
