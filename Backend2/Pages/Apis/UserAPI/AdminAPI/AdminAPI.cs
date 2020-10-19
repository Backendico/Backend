using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend2.Pages.Apis.Models.Dashboard;
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


        [HttpPost]
        public async Task<string> ReciveStatices(string Token, string Studio)
        {
            var result =await  Dashboard.ReciveDetail(Token, Studio);
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

        public void ResetLeaderboard()
        {

        }

        public void AddLog()
        {

        }

        public void ReciveStatusServer()
        {

        }

        public void ReciveStudioSetting()
        {

        }

        public void DeletePlayer()
        {

        }

        public void BanPlayer()
        {

        }
        public void SearchPlayer()
        {

        }

        public void ReciveBackups()
        {

        }

        public void SaveLeaderboard()
        {

        }


    }
}
