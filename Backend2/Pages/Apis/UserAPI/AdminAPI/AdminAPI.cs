using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Backend2.Pages.Apis.Models.Dashboard;
using Backend2.Pages.Apis.Models.Leaderobard;
using Backend2.Pages.Apis.PageDashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend2.Pages.Apis.UserAPI.AdminAPI
{
    [Controller]
    public class AdminAPI : ControllerBase
    {
        Dashboard Dashboard = new Dashboard();
        Models.Leaderobard.Leaderboard Leaderboard = new Models.Leaderobard.Leaderboard();
        Models.Logs.Logs Log = new Models.Logs.Logs();
        Models.Studio.Studios Studio = new Models.Studio.Studios();
        Models.Player.Player Player = new Models.Player.Player();


        [HttpPost]
        public async Task<string> ReciveStatices(string Token, string Studio)
        {
            var result = await Dashboard.ReciveDetail(Token, Studio);
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
        public async Task ResetLeaderboard(string Token, string Studio, string NameLeaderboard)
        {
            if (await Leaderboard.Reset(Token, Studio, NameLeaderboard))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }

        [HttpPost]
        public async Task AddLog(string Token, string Studio, string Header, string Description, string detail, string IsNotifaction)
        {
            if (await Log.AddLog(Token, Studio, Header, Description, detail, IsNotifaction))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }

        [HttpGet]
        public void ReciveStatusServer()
        {
            if (Dashboard.CheackStatusServer())
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }

        [HttpPost]
        public async Task ReciveStudioSetting(string Token, string Studio)
        {
            var Result = await this.Studio.ReciveSetting(Token, Studio);
            if (Result.ElementCount >= 1)
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }

        [HttpDelete]
        public async Task DeletePlayer(string Token, string Studio, string TokenPlayer)
        {
            if (await Player.DeletePlayer(Token, Studio, TokenPlayer))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }


        [HttpPost]
        public async Task<string> SearchPlayerToken(string Token,string Studio,string TokenPlayer)
        {
            var result = await Player.SearchToken(Token, Studio, TokenPlayer);
            if (result.ElementCount>=1)
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
        public async Task<string> SearchPlayerEmail(string Token, string Studio, string Email)
        {
            var result = await Player.SearchEmail(Token, Studio, Email);
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
        public async Task<string> SearchPlayerUsername(string Token, string Studio, string Username)
        {
            var result = await Player.SearchUsername(Token, Studio, Username);
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
        public async Task<string> ReciveBackups(string Token,string Studio,string NameLeaderboard)
        {
            var Result = await Leaderboard.ReciveBackup(Token, Studio, NameLeaderboard);
            if (Result.ElementCount>=1)
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
        public async Task BackupLeaderboard(string Token,string Studio,string NameLeaderboard)
        {
            if (await Leaderboard.Backup(Token,Studio,NameLeaderboard))
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
