using Backend2.Pages.Apis;
using Backend2.Pages.Apis.PageLoggs;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Controller]
    public class AUT : APIBase
    {
        #region posts

        [HttpPost]
        /// <summary>
        /// Register new User
        /// </summary>
        /// <returns>
        /// <list type="bullet">
        /// <item>
        /// if register complited return <see cref="HttpStatusCode.OK"/>
        /// </item>
        /// <item>
        /// if Username Doublicate <see cref="HttpStatusCode.BadRequest"/>
        /// </item>
        /// <item>
        /// if Email Doublicate <see cref="HttpStatusCode.BadRequest"/>
        /// </item>
        /// </list>
        /// </returns>
        public async Task<string> Register(string Username, string Email, string Password, string Phone)
        {
            var result = "";

            if (await CheackUsername(Username) != true && await CheackEmail(Email) != true && Password.Length >= 6)
            {
                UserModel ModelUser = new UserModel
                {
                    _id = ObjectId.GenerateNewId(),
                    AccountSetting = new UserModel.ModelAccountSetting
                    {
                        Username = Username,
                        Password = Password,
                        Email = Email,
                        Phone = Phone,
                    }
                    
                };
                ModelUser.Support = new BsonArray();
                ModelUser.Leaderboards = new BsonDocument();


                await Client.GetDatabase(UsersDB).GetCollection<UserModel>(UsersCollection).InsertOneAsync(ModelUser);

                Response.StatusCode = Ok().StatusCode;

                return ModelUser.AccountSetting.Token;
            }
            else
            {

                Response.StatusCode = BadRequest().StatusCode;
                return result;
            }
        }


        [HttpPost]
        /// <summary>
        /// Login User with <para  >  Username </para> <para  >  Password </para>
        /// </summary>
        /// <returns>
        /// <list type="bullet">
        /// <item>
        /// if Username Password Match and finde new document  <see cref="HttpStatusCode.OK"/>
        /// </item>
        /// <item>
        /// if Username Password Notmatch <see cref="HttpStatusCode.BadRequest"/>
        /// </item>
        /// </list>
        /// </returns>
        public async Task<string> Login(string Username, string Password)
        {
            var filter = new BsonDocument { { "AccountSetting.Username", Username }, { "AccountSetting.Password", Password } };
            var Token = "";
            var Result = await Client.GetDatabase(UsersDB).GetCollection<BsonDocument>(UsersCollection).FindAsync(filter).Result.SingleAsync();
            try
            {
                Token = Result["AccountSetting"]["Token"].AsString;
                Response.StatusCode = Ok().StatusCode;

                
                return Token;
            }
            catch (Exception)
            {
                Response.StatusCode = BadRequest().StatusCode;
                return Token;
            }
        }


        [HttpPost]
        /// <summary>
        /// 1: vote to placess
        /// 2: return vote place
        /// </summary>
        /// <returns>
        /// <list type="bullet">
        /// <item> Count vote  </item>
        /// </list>
        /// </returns>
        public async Task<int> VoteToBuild(string Token, string Vote)
        {
            if (await CheackToken(Token))
            {
                //vote
                var Filterusers = new BsonDocument { { "AccountSetting.Token", Token } };
                var Update = new UpdateDefinitionBuilder<BsonDocument>();

                await Client.GetDatabase(UsersDB).GetCollection<BsonDocument>(UsersCollection).UpdateOneAsync(Filterusers, Update.Set($"AccountSetting.Votes.{Vote}", true));


                //find and return count vote

                var filterfind = new BsonDocument { { $"AccountSetting.Votes.{Vote}", true } };

                var CountVote = await Client.GetDatabase(UsersDB).GetCollection<BsonDocument>(UsersCollection).FindAsync(filterfind);

                Response.StatusCode = Ok().StatusCode;

                return CountVote.ToList().Count;
            }
            else
            {

                Response.StatusCode = BadRequest().StatusCode;
                return 0;
            }

        }

        #endregion

        #region Internal methods

        [HttpPost]
        /// <summary>
        /// cheack Username
        /// </summary>
        /// <param name="Username"> cheack Username</param>
        /// <returns>
        /// if username find <see langword="true"/>
        /// if username notfind <see langword="false"/>
        /// </returns>
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

        #endregion
    }
}
