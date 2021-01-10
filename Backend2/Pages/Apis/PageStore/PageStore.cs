using Backend2.Pages.Apis.Models.Store;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend2.Pages.Apis.PageStore
{
    [Controller]
    public class PageStore : ControllerBase
    {
        Store Store = new Store();

        [HttpPost]
        public async Task AddStore(string Token, string Studio, string Detail)
        {
            if (await Store.AddStore(Token, Studio, BsonDocument.Parse(Detail)))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }

        [HttpPost]
        public async Task<string> ReciveStores(string Token, string Studio)
        {
            var Result = await Store.ReciveStores(Token, Studio);
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
        public async Task SaveStore(string Token, string Studio, string TokenStore, string DetailStore)
        {
            if (await Store.SaveStore(Token, Studio, ObjectId.Parse(TokenStore), DetailStore))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }

        [HttpDelete]
        public async Task RemoveStore(string Token, string Studio, string Detail)
        {
            if (await Store.DeleteSotre(Token, Studio, BsonDocument.Parse(Detail)))
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
