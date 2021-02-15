using Backend2.Pages.Apis.Models.KeyValue;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Backend2.Pages.Apis.UserAPI.KeyValue
{
    [Controller]
    public class KeyValue : ControllerBase
    {
        Models.KeyValue.KeyValue ModelKeyValue = new Models.KeyValue.KeyValue();

        [HttpPost]
        public async Task<string> ReceiveValue(string Token, string Studio, string Key)
        {
            var Result = await ModelKeyValue.ReceiveValue(Token, Studio, Key);

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
        public async Task<string> ReceiveKeys(string Token, string Studio)
        {
            var Result = await ModelKeyValue.ReceiveKeys(Token, Studio);

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
