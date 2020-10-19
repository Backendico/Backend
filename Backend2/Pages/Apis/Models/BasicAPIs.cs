using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend2.Pages.Apis.Models
{
    public class BasicAPIs
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


        public async Task<bool> CheackUsername(string Username)
        {
            try
            {
                var filter = new BsonDocument { { "AccountSetting.Username", Username } };

                var Result = await Client.GetDatabase("Users").GetCollection<BsonDocument>("Users").Find(filter).SingleAsync();

                _ = Result.ElementCount >= 1;
                return true;
            }
            catch (Exception)
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

        internal void SignalNotifaction(string Token)
        {
            SignalR.SignalNotifaction.SendSignal(Token.ToString());
        }



        /// <summary>
        /// cheack username 
        /// 0: if Database Empty return true
        /// 1: find User return True
        /// 2: not find return False
        /// </summary>
        /// <param name="Studio"></param>
        /// <param name=""></param>
        /// <param name="Username"></param>
        /// <returns></returns>
        internal async Task<bool> CheackUsernamePlayer(string Studio, string Username)
        {
            try
            {
                if (Username != null)
                {
                    var filter = new BsonDocument { { "Account.Username", Username } };
                    var result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").Find(filter).SingleAsync();
                    if (result.ElementCount >= 1)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return true;
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
