using Backend2.Pages.AdminApis.ApisBasicAdmin;
using Backend2.Pages.Apis.Models;
using Backend2.Pages.Apis.Models.Logs;
using Backend2.Pages.Apis.Models.Studio;
using Backend2.Pages.Apis.UserAPI;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using RestSharp;
using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Backend2.Pages.Apis.PagePayments
{
    [Controller]
    public class Payments : APIBase
    {

        Logs log = new Logs();

        [HttpPost]
        public async Task<string> Callback(int status, int track_id, string id, string order_id, int amount, string card_no, string hashed_card_no, string timestamp)
        {
            return "Latest purchase status : " + status;
        }

        [HttpPost]
        public async Task VerifiePay(string Token, string Studio, string Detail)
        {
            if (await CheackToken(Token))
            {
                var DeserilseDetail = BsonDocument.Parse(Detail);

                var client = new RestClient("https://api.idpay.ir/v1.1/payment/verify");
                client.Timeout = -1;
                client.ClearHandlers();
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("X-API-KEY", "a14190c5-c321-4a93-bdcc-e9f753608e00");
                //request.AddHeader("X-SANDBOX", "1");
                request.AddHeader("Cookie", "SSESS39ff69be91203b0b4d2039dd7a713620=7epaMgKagAqyX9SlMEc4j3MKve3PrWsPwYQQ5J0re20");
                request.AddParameter("application/json", DeserilseDetail.ToString(), ParameterType.RequestBody);
                var response = await client.ExecuteAsync(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var Paymentlog = await Client.GetDatabase("Users").GetCollection<BsonDocument>("Payments").FindAsync(new BsonDocument { { "DetailPay.id", DeserilseDetail["id"] } }).Result.SingleAsync();

                    var Update = new UpdateDefinitionBuilder<BsonDocument>().Inc("Monetiz.Cash", Paymentlog["Request"]["amount"]);

                    await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").UpdateOneAsync(new BsonDocument { { "_id", "Setting" } }, Update);

                    var Update1 = new UpdateDefinitionBuilder<BsonDocument>().Set("Status", 200);

                    await Client.GetDatabase("Users").GetCollection<BsonDocument>("Payments").UpdateOneAsync(new BsonDocument { { "DetailPay.id", DeserilseDetail["id"] } }, Update1);


                    //add payment
                    try
                    {
                        var Update2 = new UpdateDefinitionBuilder<BsonDocument>().Push<BsonDocument>("Monetiz.PaymentList", Paymentlog);

                        await Client.GetDatabase(Studio).GetCollection<BsonDocument>("Setting").UpdateOneAsync(new BsonDocument { { "_id", "Setting" } }, Update2);
                    }
                    catch (Exception)
                    {

                        throw;
                    }

                    //add log
                    try
                    {
                        await log.AddLog(Token, Studio, "Payments", $"The amount \" { Paymentlog["Request"]["amount"] } \" was credited to your account", Paymentlog.ToString(), "true");
                    }
                    catch (Exception)
                    {
                    }

                    //send email 
                    try
                    {

                        var BodyMessage = new MailMessage(
                            "pay@backendi.ir",
                            Paymentlog["Request"]["mail"].ToString(),
                            "Payment was successful",
                            body: "Hi" +
                            "\n" +
                            $"Your payment for \" {Paymentlog["Detail"]["Studio"]} \" Studio was successful." +
                            "\n\n" +
                            $"Payment tracking number:{Paymentlog["Request"]["order_id"]}" +
                            $"\n\n" +
                            "Tanks" +
                            "\n" +
                            "Backendi.ir"
                            );

                        BasicAPIs.SendMail_Pay(BodyMessage);
                    }
                    catch (Exception)
                    {
                    }

                    Response.StatusCode = Ok().StatusCode;
                }
                else
                {
                    Response.StatusCode = BadRequest().StatusCode;
                }

            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }


        [HttpPost]
        public async Task AddPaymentToList(string Token, string Detail)
        {
            if (await CheackToken(Token))
            {
                try
                {
                    var deserilseDetail = BsonDocument.Parse(Detail);
                    deserilseDetail["Detail"].AsBsonDocument.Add(new BsonElement("Created", DateTime.Now));
                    deserilseDetail.Add(new BsonElement("LastStatus", 1));
                    await Client.GetDatabase("Users").GetCollection<BsonDocument>("Payments").InsertOneAsync(deserilseDetail);

                    Response.StatusCode = Ok().StatusCode;
                }
                catch (Exception)
                {

                    Response.StatusCode = BadRequest().StatusCode;
                }
            }
            else
            {
                Response.StatusCode = Ok().StatusCode;

            }
        }


        enum PaymentCallback
        {
            Payment_not_made = 1,
            Payment_failed = 2,
            errors_occurred = 3,
            blocked = 4,
            Back_to_payer = 5,
            system_reversals = 6,
            Cancel_payment = 7,
            transferred_to_payment_gateway = 8,
            Waiting_for_payment_confirmation = 10,
            payments_approved = 100,
            Payment_has_already_been_approved = 101,
            deposited_with_the_recipient = 200
        }

    }
}
