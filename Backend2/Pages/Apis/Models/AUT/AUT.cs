using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend2.Pages.Apis.Models.AUT
{
    public class AUT : BasicAPIs
    {
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

                await Client.GetDatabase(UsersDB).GetCollection<UserModel>(UsersCollection).InsertOneAsync(ModelUser);

                return ModelUser.AccountSetting.Token;
            }
            else
            {
                return result;
            }
        }

        public async Task<string> Login(string Username, string Password)
        {
            var filter = new BsonDocument { { "AccountSetting.Username", Username }, { "AccountSetting.Password", Password } };
            var Token = "";
            var Result = await Client.GetDatabase(UsersDB).GetCollection<BsonDocument>(UsersCollection).FindAsync(filter).Result.SingleAsync();

            try
            {
                Token = Result["AccountSetting"]["Token"].AsString;
                return Token;
            }
            catch (Exception)
            {
                return Token;
            }
        }

    }
}
