using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend2.Pages.Apis.Models;
using Backend2.Pages.Apis.Models.Leaderobard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Backend2.Pages.Apis.UserAPI.Leaderboard
{
    [Controller]
    public class Leaderboard : UserAPIBase
    {
        Models.Leaderobard.Leaderboard LeaderboardModel = new Models.Leaderobard.Leaderboard();


        [HttpPost]
        public async Task<string> ReciveLeaderboardSetting(string Token, string Studio, string NameLeaderboard)
        {
            if (await BasicAPIs.ReadWriteControll(Studio, API.Read))
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
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
                return new BsonDocument().ToString();
            }
        }


        [HttpPost]
        public async Task<string> ReciveLeaderboard(string Token, string Studio, string NameLeaderboard, string Count)
        {
            if (await BasicAPIs.ReadWriteControll(Studio, API.Read))
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
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
                return new BsonDocument().ToString();
            }
        }

        [HttpPost]
        public async Task AddPlayer(string Token, string Studio, string TokenPlayer, string NameLeaderboard, string Value)
        {
            if (await LeaderboardModel.Add(Token, Studio, TokenPlayer, NameLeaderboard, Value) && await BasicAPIs.ReadWriteControll(Studio, API.Write))
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
            if (await LeaderboardModel.Remove(Token, Studio, TokenPlayer, NameLeaderboard) && await BasicAPIs.ReadWriteControll(Studio, API.Write))
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
            if (await BasicAPIs.ReadWriteControll(Studio, API.Read))
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
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
                return new BsonDocument().ToString();
            }
        }

    }
}
