﻿using Backend2.Pages.Apis;
using Backend2.Pages.Apis.Models.Studio;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

#nullable enable
namespace Backend.Controllers.PageChoiceStudioGame
{
    [Controller]
    public class ChoiceStudioGame : ControllerBase
    {
        Studios Studio = new Studios();

        /// <summary>
        /// 1: Install database
        /// 2:insert databasename to game list
        /// </summary>
        /// <returns>
        /// Name database
        /// </returns>
        [HttpPost]
        public async Task<string> CreatNewStudio(string NameStudio, string Token)
        {
            var Result = await Studio.CreatStudio(NameStudio, Token);
            if (Result.Length >= 1)
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
            return Result;
        }


        /// <summary>
        /// 1: AUT Token
        /// 2: find token 
        /// </summary>
        /// <returns>
        /// List Games Return
        /// </returns>
        [HttpPost]
        public async Task<string> ReciveStudios(string Token)
        {
            var result = await Studio.ReciveStudios(Token);

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
        /// delete Studio in Future
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="NameStudio"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task Delete(string Token, string NameStudio)
        {
            if (await Studio.Delete(Token, NameStudio))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }


        /// <summary>
        /// Recive Status and Detail Database
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="NameStudio"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> Status(string Token, string NameStudio)
        {
            var result = await Studio.Status(Token, NameStudio);

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
        public async Task<string> RecivePaymentList(string Token, string NameStudio)
        {
            var Result = await Studio.RecivePaymentList(Token, NameStudio);
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
        /// Recive Monetize List
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="NameStudio"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> ReciveMonetize(string Token, string NameStudio)
        {
            var result = await Studio.ReciveMonetize(Token, NameStudio);
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


        /// <summary>
        /// add payment log 
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="NameStudio"></param>
        /// <param name="DetailMonetize"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task AddPayment(string Token, string NameStudio, string DetailMonetize)
        {
            if (await Studio.AddPayment(Token, NameStudio, DetailMonetize))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }

        }

        /// <summary>
        /// genereate new token for aut 
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task GenerateToken(string Token)
        {
            if (await Studio.GenerateNewToken(Token))
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
