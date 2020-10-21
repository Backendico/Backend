using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Backend2.Pages.Apis.Models.Player;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend2.Pages.Apis.UserAPI.EditPlayer
{
    [Controller]
    public class EditPlayer : UserAPIBase
    {
        Player Player = new Player();

        [HttpPost]
        public async Task AddAvatarLink(string Token, string Studio, string TokenPlayer, string Link)
        {
            if (await Player.AddAvatar(Token, Studio, TokenPlayer, Link))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }

        [HttpPost]
        public async Task AddLanguage(string Token, string Studio, string TokenPlayer, string Language)
        {
            if (await Player.AddLanguage(Token, Studio, TokenPlayer, Language))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }

        [HttpPost]
        public async Task SendEmailRecovery(string Token, string studio, string TokenPlayer)
        {
            if (await Player.SendEmailRecovery(Token, studio, TokenPlayer))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }

        }

        [HttpPost]
        public async Task AddCountry(string Token, string Studio, string TokenPlayer, string Country)
        {
            if (await Player.AddCountry(Token, Studio, TokenPlayer, Country))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }


        [HttpPost]
        public async Task AddUsername(string Token, string Studio, string TokenPlayer, string Username)
        {
            if (await Player.AddUsername(Token, Studio, TokenPlayer, Username))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }

        [HttpPost]
        public async Task AddEmail(string Token, string Studio, string TokenPlayer, string Email)
        {
            if (await Player.AddEmail(Token, Studio, TokenPlayer, Email))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }


        [HttpPost]
        public async Task AddPassword(string Token, string Studio, string TokenPlayer, string OldPassword, string NewPassword)
        {
            if (await Player.AddPassword(Token, Studio, TokenPlayer, OldPassword, NewPassword))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }

        [HttpPost]
        public async Task AddNickname(string Token, string Studio, string TokenPlayer, string Nickname)
        {
            if (await Player.AddNickname(Token, Studio, TokenPlayer, Nickname))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }

        [HttpPost]
        public async Task Ban(string Studio, string Token, string TokenPlayer)
        {
            if (await Player.BanPlayer(Studio, Token, TokenPlayer))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }

        [HttpPost]
        public async Task UnBan(string Studio, string Token, string TokenPlayer)
        {
            if (await Player.UnBanPlayer(Studio, Token, TokenPlayer))
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
