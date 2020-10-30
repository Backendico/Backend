using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend2.Pages.Apis.Models.Achievements;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Backend2.Pages.Apis.PageAchievements
{
    [Controller]
    public class PageAchievements : ControllerBase
    {
        Achievements Achievements = new Achievements();

        [HttpPost]
        public async Task AddAchievements(string Token, string Studio, string Detail)
        {
            var DeserilseDetail = BsonDocument.Parse(Detail);

            if (await Achievements.AddAchievements(Studio, Token, DeserilseDetail))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }


        [HttpPost]
        public async Task<string> ReciveAchievements(string Token, string Studio)
        {
            var Result = await Achievements.ReciveAchievements(Token, Studio);

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
        public async Task CheackNameAchievements(string Token, string Studio, string NameAchievements)
        {
            if (await Achievements.CheackAchievementsName(Studio, Token, NameAchievements))
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
