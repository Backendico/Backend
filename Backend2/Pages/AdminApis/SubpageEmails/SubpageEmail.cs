using Backend2.Pages.AdminApis.ApisBasicAdmin;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Backend2.Pages.AdminApis.SubpageEmails
{
    [Controller]
    public class SubpageEmail : BasicApiAdmin
    {

        [HttpPost]
        public async Task<string> ReciveEmailList(string Token)
        {
            var Result = new BsonDocument { { "ListEmails", new BsonArray() } };

            var Find = await Client.GetDatabase(AdminDatabase).GetCollection<BsonDocument>("Emails").FindAsync("{}").Result.ToListAsync();


            if (Find.Count >= 1)
            {
                foreach (var item in Find)
                {
                    Result["ListEmails"].AsBsonArray.Add(item);
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


        [HttpPost]
        public async Task AddEmail(string Token, string Detail)
        {
            //send Email 
            Debug.WriteLine("Send Email Here");

            //send email to email database
            var SerilseEmailModel = BsonDocument.Parse(Detail);
            SerilseEmailModel.Add("Created", DateTime.Now);
            SerilseEmailModel.Add("LastSend", DateTime.Now);
            SerilseEmailModel.Add("Token", ObjectId.GenerateNewId());


            await Client.GetDatabase(AdminDatabase).GetCollection<BsonDocument>("Emails").InsertOneAsync(SerilseEmailModel);
            Response.StatusCode = Ok().StatusCode;
        }

        [HttpDelete ]
        public async Task RemoveEmail(string Token, string TokenEmail)
        {
          
            var Result = await Client.GetDatabase(AdminDatabase).GetCollection<BsonDocument>("Emails").DeleteOneAsync(new BsonDocument { { "Token", ObjectId.Parse(TokenEmail) } });


            if (Result.DeletedCount >= 1)
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }

        }


    }
}
