using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend2.Pages.Apis.UserAPI
{
    public class UserAPIBase : ControllerBase
    {
        internal MongoClient Client = new MongoClient();

        public async Task<bool> CheackToken(string Token)
        {
            var filter = new BsonDocument { { "AccountSetting.Token", Token } };

            var User = await Client.GetDatabase("Users").GetCollection<BsonDocument>("Users").Find(filter).SingleAsync<BsonDocument>();

            if (User.ElementCount >= 1)
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
