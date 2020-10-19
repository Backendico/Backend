using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend2.Pages.Apis.Models.Player;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend2.Pages.Apis.UserAPI.AUT.Register
{
    [Controller]
    public class Register : UserAPIBase
    {
        Player Player = new Player();

        [HttpPost]
        public async Task<string> Token(string Token, string Studio, string Username, string Password)
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

    }
}
