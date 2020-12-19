using Backend2.Pages.Apis.Models.Store;
using Backend2.Pages.Apis.PageStore;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver.Core.Clusters.ServerSelectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend2.Pages.Apis.UserAPI.Store
{
    [Controller]
    public class Store : UserAPIBase
    {
        Models.Store.Store ModelStore = new Models.Store.Store();

        [HttpPost]
        public async Task<string> RecieveStores(string Token, string Studio)
        {
            var Result = await ModelStore.ReciveAbstractStores(Token, Studio);

            if (Result.ElementCount >= 1)
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }

            return Result.ToString();
        }

        [HttpPost]
        public async Task<string> RecieveProducts(string Token, string Studio, string TokenStore)
        {
            var Result = await ModelStore.ReciveProducts(Token, Studio, ObjectId.Parse(TokenStore));

            if (Result.ElementCount >= 1)
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }

            return Result.ToString();
        }

        [HttpPost]
        public async Task AddPayments(string Token, string Studio, string TokenStore, string TokenPlayer,string Detail)
        {
            if (await ModelStore.AddPayment(Token, Studio, ObjectId.Parse(TokenStore), ObjectId.Parse(TokenPlayer),Detail))
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
