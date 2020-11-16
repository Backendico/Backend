using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend2.Pages.Apis.Models;
using Backend2.Pages.Apis.Models.Player;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Backend2.Pages.Apis.UserAPI.AUT.Login
{
    [Controller]
    public class Login : UserAPIBase
    {
        Player Player = new Player();

        [HttpPost]
        /// <summary>
        /// Return PlayerAccount
        /// </summary>
        public async Task<string> Token(string Token, string Studio, string TokenPlayer)
        {
            if (await BasicAPIs.ReadWriteControll(Studio, API.Write))
            {
                var Result = await Player.LoginPlayer(Token, Studio, TokenPlayer);
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
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
                return new BsonDocument().ToString();
            }
        }

        [HttpPost]
        public async Task<string> UsernamePassword(string Token, string Studio, string Username, string Password)
        {
            var Result = await Player.LoginPlayer(Token, Studio, Username, Password);
            if (Result.ElementCount <= 1)
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
