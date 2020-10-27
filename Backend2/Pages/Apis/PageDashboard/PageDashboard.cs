using Backend2.Pages.Apis.Models.Dashboard;
using Backend2.Pages.Apis.Models.Player;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Backend2.Pages.Apis.PageDashboard
{
    [Controller]
    public class PageDashboard : ControllerBase
    {
        Dashboard Dashboard = new Dashboard();

        /// <summary>
        /// recive Detail Page Dashboard
        /// </summary>
        /// 
        /// <param name="Token"></param>
        /// <param name="Studio"></param>
        /// <returns>
        /// <list  type="bullet">
        /// <listheader >Players</listheader>
        /// <list type="bullet">
        ///   <item>24 Hours</item>
        ///   <item>1Day</item>
        ///   <item>7 Day</item>
        ///   <item>30 day</item>
        /// </list>
        /// </list>
        /// <list type="bullet">
        /// <listheader> Logins</listheader>
        /// <list type="bullet">
        ///   <item>24 Hours</item>
        ///   <item>1Day</item>
        ///   <item>7 Day</item>
        ///   <item>30 day</item>
        /// 
        /// </list>
        /// </list>
        /// 
        /// <list type="bullet">
        /// <listheader>Counts</listheader>
        /// <list type="bullet">
        ///   <item>Leaderboards</item>
        ///   <item> Players</item>
        /// 
        /// </list>
        /// </list>
        /// </returns>
        [HttpPost]
        public async Task<string> ReciveDetail(string Token, string Studio)
        {
            var Result = await Dashboard.ReciveDetail(Token, Studio);

            if (Result.ElementCount >= 1)
            {
                Response.StatusCode = Ok().StatusCode;
                return Result.ToString();
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
                return Result.ToString();
            }
        }


        /// <summary>
        /// cheack status server
        /// </summary>
        [HttpGet]
        public void CheackStatusServer()
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


        /// <summary>
        /// Recive Notifaction Count Support Count Hitsotyr
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Studio"></param>
        /// <returns></returns>
        public async Task<string> Notifaction(string Token, string Studio)
        {
            var Result = await Dashboard.Notifaction(Token, Studio);

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
        public async Task<string> CheackUpdate()
        {
            var Result = await Dashboard.CheackUpdate();
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

    }
}
