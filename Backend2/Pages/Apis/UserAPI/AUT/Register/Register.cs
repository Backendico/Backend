using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend2.Pages.Apis.Models;
using Backend2.Pages.Apis.Models.Player;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Backend2.Pages.Apis.UserAPI.AUT.Register
{
    [Controller]
    public class Register : UserAPIBase
    {
        Player Player = new Player();

        [HttpPost]
        public async Task<string> Token(string Token, string Studio, string Username, string Password)
        {
            if (await BasicAPIs.ReadWriteControll(Studio, API.Write))
            {
                var Result = await Player.CreatPlayer(Token, Studio, Username, Password);

                if (Result.Length >= 1)
                {
                    Response.StatusCode = Ok().StatusCode;
                }
                else
                {
                    Response.StatusCode = BadRequest().StatusCode;
                }

                return Result;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
                return new BsonDocument().ToString();
            }
        }

    }
}
