using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Backend2.Pages.Apis.Models;
using Backend2.Pages.Apis.Models.Player;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Backend2.Pages.Apis.UserAPI.EditPlayer
{
    [Controller]
    public class EditPlayer : UserAPIBase
    {
        Player Player = new Player();

        [HttpPost]
        public async Task AddAvatarLink(string Token, string Studio, string TokenPlayer, string Link)
        {
            if (await Player.AddAvatar(Token, Studio, TokenPlayer, Link) && await BasicAPIs.ReadWriteControll(Studio, API.Write))
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
            if (await Player.AddLanguage(Token, Studio, TokenPlayer, Language) && await BasicAPIs.ReadWriteControll(Studio, API.Write))
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
            if (await Player.SendEmailRecovery(Token, studio, TokenPlayer) && await BasicAPIs.ReadWriteControll(studio, API.Write))
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
            if (await Player.AddCountry(Token, Studio, TokenPlayer, Country) && await BasicAPIs.ReadWriteControll(Studio, API.Write))
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
            if (await Player.AddUsername(Token, Studio, TokenPlayer, Username) && await BasicAPIs.ReadWriteControll(Studio, API.Write))
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
            if (await Player.AddEmail(Token, Studio, TokenPlayer, Email) && await BasicAPIs.ReadWriteControll(Studio, API.Write))
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
            if (await Player.AddPassword(Token, Studio, TokenPlayer, OldPassword, NewPassword) && await BasicAPIs.ReadWriteControll(Studio, API.Write))
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
            if (await Player.AddNickname(Token, Studio, TokenPlayer, Nickname) && await BasicAPIs.ReadWriteControll(Studio, API.Write))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }


        [HttpPost]
        public async Task AddPhone(string Token, string Studio, string TokenPlayer, string PhoneNumber)
        {
            if (await Player.AddPhoneNumber(Token, Studio, TokenPlayer, PhoneNumber) && await BasicAPIs.ReadWriteControll(Studio, API.Write))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }

        [HttpPost]
        public async Task AddLogPlayer(string Token, string Studio, string TokenPlayer, string Header, string Description)
        {
            if (await Player.AddLogPlayer(Token, Studio, TokenPlayer, Header, Description) && await BasicAPIs.ReadWriteControll(Studio, API.Write))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }

        [HttpPost]
        public async Task<string> RecivePlayerLogs(string Token, string Studio, string TokenPlayer, string Count)
        {
            if (await BasicAPIs.ReadWriteControll(Studio, API.Write))
            {

                var Result = await Player.RecivePlayerLogs(Token, Studio, TokenPlayer, Count);

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
        public async Task Ban(string Studio, string Token, string TokenPlayer)
        {
            if (await Player.BanPlayer(Studio, Token, TokenPlayer) && await BasicAPIs.ReadWriteControll(Studio, API.Write))
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
            if (await Player.UnBanPlayer(Studio, Token, TokenPlayer) && await BasicAPIs.ReadWriteControll(Studio, API.Write))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }

        }


        [HttpPost]
        public async Task<int> RecoveryStep1(string Token, string Studio, string TokenPlayer, string Email)
        {
            var Result = await Player.Recovery_Step1(Token, Studio, TokenPlayer, Email);
            if (Result >= 1)
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }

            return Result;
        }


        [HttpPost]
        public async Task RecoveryStep2(string Token, string Studio, string TokenPlayer, string Email, int Code)
        {
            if (await Player.Recovery_Step2(Token, Studio, TokenPlayer, Email, Code))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }


        [HttpPost]
        public async Task ChangePassword(string Token, string Studio, string TokenPlayer, string Password)
        {
            if (await Player.ChangePassword(Token, Studio, TokenPlayer, Password))
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
