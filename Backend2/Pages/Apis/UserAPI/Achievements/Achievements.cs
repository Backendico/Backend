using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Backend2.Pages.Apis.Models;
using Backend2.Pages.Apis.Models.Achievements;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Backend2.Pages.Apis.UserAPI.Achievements
{
    [Controller]
    public class Achievements : ControllerBase
    {
        Models.Achievements.Achievements ModelAchievements = new Models.Achievements.Achievements();


        [HttpPost]
        public async Task<string> ListAchievements(string Studio, string Token)
        {
            if (await BasicAPIs.ReadWriteControll(Studio, API.Read))
            {

                var Result = await ModelAchievements.ReciveAchievements(Token, Studio);

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
        public async Task AddPlayerAchievements(string Token, string Studio, string TokenPlayer, string Detail)
        {
            if (await BasicAPIs.ReadWriteControll(Studio, API.Write))
            {

                if (await ModelAchievements.AddPlayerAchievements(Token, Studio, ObjectId.Parse(TokenPlayer), BsonDocument.Parse(Detail)))
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
                Response.StatusCode = BadRequest().StatusCode;
            }

        }

        [HttpPost]
        public async Task<string> PlayerAchievements(string Token, string Studio, string TokenPlayer)
        {
            if (await BasicAPIs.ReadWriteControll(Studio, API.Write))
            {

                var Result = await ModelAchievements.PlayerAchievements(Token, Studio, ObjectId.Parse(TokenPlayer));
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

        [HttpDelete]
        public async Task RemovePlayerAchievements(string Token, string Studio, string TokenPlayer, string Detail)
        {
            if (await BasicAPIs.ReadWriteControll(Studio, API.Write))
            {
                if (await ModelAchievements.Remove(Token, Studio, ObjectId.Parse(TokenPlayer), BsonDocument.Parse(Detail)))
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
                Response.StatusCode = BadRequest().StatusCode;
            }
        }

    }
}
