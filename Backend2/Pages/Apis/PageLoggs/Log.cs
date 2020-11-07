using Backend2.Pages.Apis.Models.Logs;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Driver.Core.Operations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Backend2.Pages.Apis.PageLoggs
{
    [Controller]
    public class Log : ControllerBase
    {
        Logs Logs = new Logs();

        [HttpPost]
        public async Task<string> ReciveLogs(string Token, string Studio, string Count)
        {
            var result = await Logs.ReciveLogs(Token, Studio, Count);
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
        public async Task AddLog(string Token, string Studio, string Header, string Description, string detail, string IsNotifaction)
        {
            if (await Logs.AddLog(Token, Studio, Header, Description, detail, IsNotifaction))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }


        [HttpDelete]
        public async Task DeleteLog(string Token, string Studio, string Detail)
        {
            if (await Logs.DeleteLog(Token, Studio, Detail))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }


        [HttpPost]
        public async Task MarkReadNotifaction(string Token, string Studio)
        {
            if (await Logs.CheackToken(Token))
            {

                if (await Logs.MarkReadNotifactions(Token, Studio))
                {
                    Response.StatusCode = Ok().StatusCode;
                }
                else
                {
                    Response.StatusCode = BadRequest().StatusCode;
                }
            }
            else
            {
                Response.StatusCode = Ok().StatusCode;
            }


        }


    }
}
