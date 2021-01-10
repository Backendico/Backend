using Backend2.Pages.Apis.Models.Content;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Backend2.Pages.Apis.PageContent
{
    [Controller]
    public class PageContent : ControllerBase
    {
        Content ModelContent = new Content();


        [HttpPost]
        public async Task AddContent(string Token, string Studio, string NameContent)
        {
            if (await ModelContent.AddContent(Token, Studio, NameContent))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }

        }

        [HttpPost]
        public async Task<string> RecieveContents(string Token, string Studio, int Count)
        {
            var Result = await ModelContent.RecieveContents(Token, Studio, Count);

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
        public async Task EditContent(string Token, string Studio, string TokenContent, string Detail)
        {
            if (await ModelContent.EditContent(Token, Studio, ObjectId.Parse(TokenContent), BsonDocument.Parse(Detail)))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }

        }

        [HttpDelete]
        public async Task DeleteContent(string Token, string Studio, string TokenContent)
        {

            if (await ModelContent.DeleteContent(Token, Studio, ObjectId.Parse(TokenContent)))
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
