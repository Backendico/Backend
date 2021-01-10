using Backend2.Pages.Apis;
using Backend2.Pages.Apis.Models.Leaderobard;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Backend.Controllers.PageLeaderBoard
{
    [Controller]
    public class PageLeaderBoard : ControllerBase
    {
        Leaderboard LeaderboardModel = new Leaderboard();

        /// <summary>
        /// recive abstract leaderboard
        /// </summary>
        /// <returns>
        /// recive leaderboards string
        /// </returns>
        [HttpPost]
        public async Task<string> ReciveLeaderboards(string Token, string Studio)
        {
            var Result = await LeaderboardModel.ReciveLeaderboards(Token, Studio);

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


        /// <summary>
        /// 1: Cheack token user
        /// 2: insert Leaderboard
        /// </summary>
        /// <returns>
        /// <list type="bullet">
        /// <item> if Token vrifie and creat leaderboard  <see cref="HttpStatusCode.OK"/></item>
        /// <item> Else <see cref="HttpStatusCode.BadRequest"/></item>
        /// </list>
        /// </returns>
        [HttpPost]
        public async Task CreatLeaderBoard(string Token, string Studio, string DetailLeaderboard)
        {
            if (await LeaderboardModel.CreatLeaderBoard(Token, Studio, DetailLeaderboard))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }


        /// <summary>
        /// 1:cheack token
        /// 2: insert setting =setting
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Studio"></param>
        /// <param name="DetailLeaderboard"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task EditLeaderBoard(string Token, string Studio, string DetailLeaderboard)
        {
            if (await LeaderboardModel.EditLeaderBoard(Token, Studio, DetailLeaderboard))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }


        /// <summary>
        /// Recive Leaderboard Detail
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Studio"></param>
        /// <param name="NameLeaderboard"></param>
        /// <param name="Count"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> Leaderboard(string Token, string Studio, string NameLeaderboard, string Count)
        {
            var result = await LeaderboardModel.LeaderboardDetail(Token, Studio, NameLeaderboard,int.Parse( Count));
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
        /// Inject Player to leaderboard
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Detail">
        /// 1:TokenLeaderboard
        /// 2:Data
        /// 3:Studio
        /// </param>
        /// <returns></returns>
        public async Task Add(string Token, string Studio, string TokenPlayer, string NameLeaderboard, string Value)
        {
            if (await LeaderboardModel.Add(Token, Studio, TokenPlayer, NameLeaderboard, Value))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }


        /// <summary>
        /// Remove Player from Leaderboard 
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Detail">
        /// 1:TokenLeaderboard
        /// 2:Data
        /// 3:Studio
        /// </param>
        /// <returns></returns>
        [HttpDelete]
        public async Task Remove(string Token, string Studio, string TokenPlayer, string NameLeaderboard)
        {
            if (await LeaderboardModel.Remove(Token, Studio, TokenPlayer, NameLeaderboard))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }


        /// <summary>
        /// 1: remove all value from all users <paramref name="Studio"/>
        /// 2: reset Start in setting <paramref name="NameLeaderboard"/>
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Studio"></param>
        /// <param name="NameLeaderboard"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task Reset(string Token, string Studio, string NameLeaderboard)
        {
            if (await LeaderboardModel.Reset(Token, Studio, NameLeaderboard))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }

        }


        /// <summary>
        /// Make Backup
        /// 1: find all Players 
        /// 2: deploy start and end time
        /// 3: inject to setting
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Studio"></param>
        /// <param name="NameLeaderboard"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task Backup(string Token, string Studio, string NameLeaderboard)
        {

            if (await LeaderboardModel.Backup(Token, Studio, NameLeaderboard))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }

        /// <summary>
        /// recive backups list
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Studio"></param>
        /// <param name="NameLeaderboard"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> ReciveBackup(string Token, string Studio, string NameLeaderboard,string Count)
        {
            var result = await LeaderboardModel.ReciveBackup(Token, Studio, NameLeaderboard,int.Parse(Count));

            if (result.ElementCount >= 1)
            {
                Response.StatusCode = Ok().StatusCode;
                return result.ToString();
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
                return result.ToString();
            }
        }

        /// <summary>
        /// remove backup from list
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Studio"></param>
        /// <param name="NameLeaderboard"></param>
        /// <param name="Version"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task  RemoveBackup(string Token, string Studio, string TokenBackups)
        {
            if (await LeaderboardModel.RemoveBackup(Token, Studio, ObjectId.Parse(TokenBackups)))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }


        /// <summary>
        /// Cheack Leaderboard Name
        /// </summary>
        /// <param name="Studio"></param>
        /// <param name="NameLeaderbaord"></param>
        /// <returns>
        /// <list type="number">
        /// <item>
        /// If Find Return True
        /// 
        /// </item>
        /// <item>
        /// if not find return false    
        /// </item>
        /// </list>
        /// </returns>
        [HttpPost]
        public async Task<bool> CheackLeaderboardName(string Studio, string NameLeaderbaord)
        {
            if (await LeaderboardModel.CheackLeaderboardName(Studio, NameLeaderbaord))
            {
                Response.StatusCode = Ok().StatusCode;
                return true;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
                return false;
            }

        }

    }
}
