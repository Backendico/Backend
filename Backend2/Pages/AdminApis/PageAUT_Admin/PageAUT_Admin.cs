using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Backend2.Pages.AdminApis.ApisBasicAdmin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend2.Pages.AdminApis.PageAUT_Admin
{
    [Controller]
    public class PageAUT_Admin : BasicApiAdmin
    {
        [HttpPost]
        public async Task<bool> Login(string Username, string Password)
        {
            var Quary = new BsonDocument { { "Account.Username", Username }, { "Account.Password", Password } };


            var Result = await Client.GetDatabase(AdminDatabase).GetCollection<BsonDocument>(AdminCollection).FindAsync(Quary).Result.SingleAsync();

            if (Result.ElementCount >= 1)
            {
                return true;

            }
            else
            {
                return false;
            }

        }


    }
}
