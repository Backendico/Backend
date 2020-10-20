using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend2.Pages.Apis.Models.Leaderobard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend2.Pages.Apis.UserAPI.Leaderboard
{
    [Controller]
    public class Leaderboard : UserAPIBase
    {
        Models.Leaderobard.Leaderboard LeaderboardModel = new Models.Leaderobard.Leaderboard();


        [HttpPost]
        public async Task<string> ReciveLeaderboardSetting(string Token, string Studio, string NameLeaderboard)
        {
            var result = await LeaderboardModel.SettingLeaderboard(Token, Studio, NameLeaderboard);
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
        public async Task<string> ReciveLeaderboard(string Token, string Studio, string NameLeaderboard, string Count)
        {
            var result = await LeaderboardModel.LeaderboardDetail(Token, Studio, NameLeaderboard, Count);
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

        [HttpPost]
        public async Task AddPlayer(string Token, string Studio, string TokenPlayer, string NameLeaderboard, string Value)
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

        public async Task RemovePlayer(string Token, string Studio, string TokenPlayer, string NameLeaderboard)
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


        [HttpPost]
        public async Task<string> RecivePlayerLeaderboards(string Token, string Studio, string TokenPlayer)
        {
            var Result = await LeaderboardModel.RecivePlayerLeaderboard(Token, Studio, TokenPlayer);
            if (Result.IsBsonNull)
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
            else
            {
                Response.StatusCode = Ok().StatusCode;
            }

            return Result.ToString();
        }

    }
}
