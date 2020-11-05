using Backend2.Pages.Apis;
using Backend2.Pages.Apis.Models.AUT;
using Backend2.Pages.Apis.PageLoggs;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Controller]
    public class AUT : ControllerBase
    {
        Backend2.Pages.Apis.Models.AUT.AUT BaseAUT = new Backend2.Pages.Apis.Models.AUT.AUT();

        #region posts

        /// <summary>
        /// Register new User
        /// </summary>
        /// <returns>
        /// <list type="bullet">
        /// <item>
        /// if register complited return <see cref="HttpStatusCode.OK"/>
        /// </item>
        /// <item>
        /// if Username Doublicate <see cref="HttpStatusCode.BadRequest"/>
        /// </item>
        /// <item>
        /// if Email Doublicate <see cref="HttpStatusCode.BadRequest"/>
        /// </item>
        /// </list>
        /// </returns>
        [HttpPost]
        public async Task<string> Register(string Username, string Email, string Password, string Phone)
        {
            var Result = await BaseAUT.Register(Username, Email, Password, Phone);
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
        /// Login User with <para  >  Username </para> <para  >  Password </para>
        /// </summary>
        /// <returns>
        /// <list type="bullet">
        /// <item>
        /// if Username Password Match and finde new document  <see cref="HttpStatusCode.OK"/>
        /// </item>
        /// <item>
        /// if Username Password Notmatch <see cref="HttpStatusCode.BadRequest"/>
        /// </item>
        /// </list>
        /// </returns>
        [HttpPost]
        public async Task<string> Login(string Username, string Password)
        {
            var Result = await BaseAUT.Login(Username, Password);

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
        /// cheack Username
        /// </summary>
        /// <param name="Username"> cheack Username</param>
        /// <returns>
        /// if username find <see langword="true"/>
        /// if username notfind <see langword="false"/>
        /// </returns>
        [HttpPost]
        public async Task<bool> CheackUsername(string Username)
        {
            if (await BaseAUT.CheackUsername(Username))
            {
                Response.StatusCode = Ok().StatusCode;
                return true;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
                return false;
            }
        }



        [HttpPost]
        public async Task Recovery1(string Email)
        {
            if (await BaseAUT.RecoveryStep1(Email))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }

        [HttpPost]
        public async Task Recovery2(string Email, string Code)
        {
            if (await BaseAUT.RecoveryStep2(Email, int.Parse(Code)))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }

        [HttpPost]
        public async Task Recovery3(string Email,string Code,string NewPassword)
        {
            if (await BaseAUT.ChangePassword(Email,int.Parse(Code),NewPassword))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }
        }

        #endregion
    }
}
