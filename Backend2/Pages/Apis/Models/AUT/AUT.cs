using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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

        public async Task<bool> RecoveryStep1(string Email)
        {
            var Result = await Client.GetDatabase("Users").GetCollection<BsonDocument>("Users").FindAsync(new BsonDocument { { "AccountSetting.Email", Email } }).Result.ToListAsync();

            //cheack email 
            if (Result.Count >= 1)
            {
                //work  send
                var RecoveryCode = new Random().Next();

                var Update = new UpdateDefinitionBuilder<BsonDocument>().Set("AccountSetting.RecoveryCode", RecoveryCode);

                var result = await Client.GetDatabase("Users").GetCollection<BsonDocument>("Users").UpdateOneAsync(new BsonDocument { { "AccountSetting.Email", Email } }, Update);

                if (result.ModifiedCount >= 1)
                {
                    var bodyMessage =
                        $"Hello dear : {Result[0]["AccountSetting"]["Username"]}" +
                        "\n" +
                        "\n" +
                        "This email was sent to your request to recover your account." +
                        "\n" +
                        "Ignore this email if you have not submitted an account recovery request.  " +
                        "\n\n" +
                        "Or use this code to recover the account." +
                        "\n\n" +
                        $"Code : {RecoveryCode}" +
                        "\n\n\n" +
                        "Thanks for choosing us" +
                        "\n" +
                        "Backendi.ir";


                    MailMessage Message = new MailMessage("recovery@backendi.ir", Email, "Recovery Account", bodyMessage);


                    SendMail_Recovery( Message, (s, e) => { }, () => { });

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        public async Task<bool> RecoveryStep2(string Email, int Code)
        {
            var Result = await Client.GetDatabase("Users").GetCollection<BsonDocument>("Users").FindAsync(new BsonDocument { { "AccountSetting.RecoveryCode", Code }, { "AccountSetting.Email", Email } }).Result.ToListAsync();

            if (Result.Count >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> ChangePassword(string Email, int Code, string NewPassword)
        {

            var Filter = new BsonDocument { { "AccountSetting.Email", Email }, { "AccountSetting.RecoveryCode", Code } };
            var Update = new UpdateDefinitionBuilder<BsonDocument>().Set("AccountSetting.Password", NewPassword);
            var Result = await Client.GetDatabase("Users").GetCollection<BsonDocument>("Users").UpdateOneAsync(Filter, Update);

            if (Result.ModifiedCount >= 1)
            {
                var Update1 = new UpdateDefinitionBuilder<BsonDocument>().Set("AccountSetting.RecoveryCode", 0);

                await Client.GetDatabase("Users").GetCollection<BsonDocument>("Users").UpdateOneAsync(Filter, Update1);


                //send mail status
                var body = "Your account password changed successfully" +
                    "\n\n" +
                    "Thanks" +
                    "\n" +
                    "backendi.ir";
                var message = new MailMessage("recovery@backendi.ir", Email, "Password Changed", body);


                SendMail_Recovery( message, (s, e) => { }, () => { });


                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
