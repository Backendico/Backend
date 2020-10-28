using Backend2.Pages.Apis;
using Backend2.Pages.Apis.Models.Player;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Net;
using System.Threading.Tasks;


namespace Backend.Controllers.Players
{
    [Controller]
    public class PagePlayer : ControllerBase
    {
        Player player = new Player();

        /// <summary>
        /// creat new player
        /// 1:cheack username for dublicate username in studio
        /// </summary>
        /// <returns>
        /// <list type="bullet">
        /// <item> if token validatate <see cref="HttpStatusCode.OK"/></item>
        /// <item> if token validatate <see cref="HttpStatusCode.BadGateway"/></item>
        /// </list>
        /// </returns>
        [HttpPost]
        public async Task<string> CreatPlayer(string Token, string Studio, string Username, string Password)
        {
            var Result = await player.CreatPlayer(Token, Studio, Username, Password);

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


        /// <summary>
        /// recive frist detail for page player
        /// 1: recive 100 list player
        /// 2: recive all count Player
        /// </summary>
        /// <returns>
        /// list and count players
        /// </returns>
        [HttpPost]
        public async Task<string> ReciveDetailPagePlayer(string Token, string Studio,string Count)
        {
            var result = await player.ReciveDetailPagePlayer(Token, Studio,int.Parse(Count));

            if (result.Length >= 1)
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }

            return result;
        }


        [HttpDelete]
        public async Task DeletePlayer(string Token, string Studio, string TokenPlayer)
        {
            if (await player.DeletePlayer(Token, Studio, TokenPlayer))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }


        [HttpPost]
        public async Task SavePlayer(string Token, string Studio, string TokenPlayer, string AccountDetail)
        {
            if (await player.SavePlayer(Token, Studio, TokenPlayer, AccountDetail))
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
            if (await player.SendEmailRecovery(Token, studio, TokenPlayer))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }


        /// <summary>
        /// 1: search username
        /// 2: if not find and exption return  bad request
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Studio"></param>
        /// <param name="Username"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> SearchUsername(string Token, string Studio, string Username)
        {
            var result = await player.SearchUsername(Token, Studio, Username);
            if (result.ElementCount >= 1)
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }

            return result.ToString();
        }


        /// <summary>
        /// 1: search username
        /// 2: if not find and exption return  bad request
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Studio"></param>
        /// <param name="Username"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> SearchEmail(string Token, string Studio, string Email)
        {
            var result = await player.SearchEmail(Token, Studio, Email);
            if (result.ElementCount >= 1)
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
            return result.ToString();
        }


        /// <summary>
        /// 1: search username
        /// 2: if not find and exption return  bad request
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Studio"></param>
        /// <param name="Username"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> SearchToken(string Token, string Studio, string TokenPlayer)
        {
            var result = await player.SearchToken(Token, Studio, TokenPlayer);
            if (result.ElementCount >= 1)
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
            return result.ToString();
        }


        [HttpPost]
        public async Task Save_LeaderboardPlayer(string Token, string TokenPlayer, string Studio, string DetailLeaderboard)
        {
            if (await player.Save_LeaderboardPlayer(Token, TokenPlayer, Studio, DetailLeaderboard))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }


        [HttpPost]
        public async Task<string> RecivePlayerLogs(string Token, string TokenPlayer, string Studio, string Count)
        {
            var Result = await player.RecivePlayerLogs(Token, Studio, TokenPlayer, Count);

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
        public async Task AddPlayerLog(string Token, string TokenPlayer, string Studio, string Header, string Description)
        {
            if (await player.AddLogPlayer(Token, Studio, TokenPlayer, Header, Description))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }

        }


        [HttpDelete]
        public async Task ClearLogs(string Token, string Studio, string TokenPlayer)
        {
            if (await player.ClearLogs(Token, Studio, TokenPlayer))
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
