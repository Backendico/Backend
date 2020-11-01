using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend2.Pages.Apis.Models.Achievements;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver.Core.Clusters.ServerSelectors;

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

        [HttpPost]
        public async Task EditAchievements(string Token, string Studio, string TokenAchievements, string Detail)
        {
            if (await Achievements.EditAchievements(Token, Studio, ObjectId.Parse(TokenAchievements), BsonDocument.Parse(Detail)))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }

        }

        [HttpPut]
        public async Task AddPlayerAchievements(string Token, string Studio, string TokenPlayer, string Detail)
        {
            if (await Achievements.AddPlayerAchievements(Token, Studio, ObjectId.Parse(TokenPlayer), BsonDocument.Parse(Detail)))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }


        [HttpPost]
        public async Task<string> ReciveAchievementsPlayerList(string Token, string Studio, string TokenAchievement, string Count)
        {
            try
            {
                var Result = await Achievements.RecivePlayersAchivementsList(Token, Studio, ObjectId.Parse(TokenAchievement), int.Parse(Count));

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
            catch (Exception)
            {
                return new BsonDocument().ToString();
            }

        }


        [HttpDelete]
        public async Task RemoveAchievementPlayer(string Token, string Studio, string TokenPlayer, string Detail)
        {
            if (await Achievements.Remove(Token, Studio, ObjectId.Parse(TokenPlayer), BsonDocument.Parse(Detail)))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }

        [HttpDelete]
        public async Task RemoveAchievements(string Token, string Studio, string Detail)
        {
            if (await Achievements.RemoveAchievements(Token, Studio, BsonDocument.Parse(Detail)))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }


        [HttpPost]
        public async Task<string> PlayerAchievements(string Token, string Studio, string TokenPlayer)
        {
            var Result = await Achievements.PlayerAchievements(Token, Studio, ObjectId.Parse(TokenPlayer));

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
