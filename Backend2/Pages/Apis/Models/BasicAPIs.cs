using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Backend2.Pages.Apis.Models
{
    public class BasicAPIs
    {
        internal MongoClient Client = new MongoClient();
        internal string UsersDB => "Users";
        internal string UsersCollection => "Users";


        /// <summary>
        /// cheack tokens Users
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
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
        /// cheack Username Users
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
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


        /// <summary>
        /// cheack read write for limited use
        /// </summary>
        /// <param name="Studio"></param>
        /// <param name="API"></param>
        /// <returns></returns>
        public static async Task<bool> ReadWriteControll(string Studio, API API)
        {
            MongoClient Client = new MongoClient();

            var Filter = new FindOptions<BsonDocument>() { Projection = new BsonDocument { { "Monetiz", 1 }, { "APIs", 1 } } };

            var Find = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").FindAsync(new BsonDocument { { "_id", "Setting" } }, Filter).Result.SingleAsync();

            var TotalAPI = Find["Monetiz"]["Apis"].ToInt64();
            var UserAPI = Find["APIs"]["Read"].ToInt64() + Find["APIs"]["Write"].ToInt64();

            if (UserAPI + 1 <= TotalAPI)
            {
                UpdateDefinition<BsonDocument> Update = null;
                switch (API)
                {
                    case API.Read:
                        Update = Builders<BsonDocument>.Update.Inc<int>("APIs.Read", 1);
                        break;
                    case API.Write:
                        Update = Builders<BsonDocument>.Update.Inc<int>("APIs.Write", 1);
                        break;
                }

                await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").UpdateOneAsync(new BsonDocument { { "_id", "Setting" } }, Update);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// send email with smtp
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static void SendMail( MailMessage Message, SendCompletedEventHandler Result, Action ERR)
        {
            SmtpClient Mailsender = new SmtpClient("mail.backendi.ir", 587);
            Mailsender.Credentials = new NetworkCredential("recovery@backendi.ir", "85245685hHH!");

            try
            {
                Mailsender.Send(Message);
                Mailsender.SendCompleted += Result;
            }
            catch (Exception)
            {
                ERR();
            }

        }
        public static void SendMail_Pay(MailMessage Message)
        {
            SmtpClient Mailsender = new SmtpClient("mail.backendi.ir", 587);
            Mailsender.Credentials = new NetworkCredential("pay@backendi.ir", "85245685hHH!");

            try
            {
                Mailsender.Send(Message);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Send Signal notifaction
        /// </summary>
        /// <param name="Token"></param>
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

        internal async Task<bool> CheackEmailPlayer(string Studio, string Email)
        {
            try
            {
                _ = new MailAddress(Email);

                var Result = await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Players").FindAsync(new BsonDocument { { "Account.Email", Email } }).Result.ToListAsync();

                if (Result.Count >= 1)
                {
                    return true;
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

    public enum API
    {
        Read, Write

    }
}
