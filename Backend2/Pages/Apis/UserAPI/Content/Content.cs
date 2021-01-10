using Backend2.Pages.Apis.Models.Content;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend2.Pages.Apis.UserAPI.Content
{
    [Controller]
    public class Content : ControllerBase
    {
        Models.Content.Content ModelContent = new Models.Content.Content();


        [HttpPost]
        public async Task<string> ReceiveContents(string Token, string Studio, string Count)
        {
            var Result = await ModelContent.RecieveContents(Token, Studio, int.Parse(Count));

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
        public async Task<string> ReceiveContent(string Token, string Studio, string TokenContent)
        {

            var Result = await ModelContent.ReceiveContent(Token, Studio, ObjectId.Parse(TokenContent));
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


    }
}
