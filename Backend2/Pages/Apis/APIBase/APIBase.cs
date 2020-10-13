using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace Backend2.Pages.Apis
{
    public class APIBase : ControllerBase
    {
        internal MongoClient Client = new MongoClient();
        internal string UsersDB => "Users";
        internal string UsersCollection => "Users";

        public async Task<bool> CheackToken(string Token)
        {
            var filter = new BsonDocument { { "AccountSetting.Token", Token } };

            var User = await Client.GetDatabase(UsersDB).GetCollection<BsonDocument>(UsersCollection).Find(filter).SingleAsync<BsonDocument>();


            if (User.ElementCount >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }

        }


        /// <summary>
        /// cheack Email
        /// </summary>
        /// <param name="Username"> cheack Username</param>
        /// <returns>
        /// if Email find <see langword="true"/>
        /// if Email notfind <see langword="false"/>
        /// </returns>
        public async Task<bool> CheackEmail(string Email)
        {
            try
            {
                var filter = new BsonDocument { { "AccountSetting.Email", Email } };

                var Result = await Client.GetDatabase(UsersDB).GetCollection<BsonDocument>(UsersCollection).Find(filter).SingleAsync();
                _ = Result.ElementCount >= 1;
                return true;
            }
            catch (Exception)
            {

                return false;
            }


        }



        public class UserModel
        {

            public ObjectId _id;
            public BsonArray Games = new BsonArray();
            public ModelAccountSetting AccountSetting;


            public class ModelAccountSetting
            {
                public string Username;
                public string Password;
                public string Email;
                public string Phone;
                public string Token = ObjectId.GenerateNewId().ToString();
            }
        }
    }


}
