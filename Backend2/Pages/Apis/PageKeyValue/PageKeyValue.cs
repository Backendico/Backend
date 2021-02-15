using Backend2.Pages.Apis.Models.KeyValue;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;

namespace Backend2.Pages.Apis.PageKeyValue
{
    [Controller]
    public class PageKeyValue : ControllerBase
    {
        KeyValue ModelKeyValue = new KeyValue();


        [HttpPost]
        public async Task AddKey(string Token, string Studio, string Key, string Value)
        {

            if (await ModelKeyValue.AddKey(Token, Studio, Key, Value))
            {
                Response.StatusCode = Ok().StatusCode;

            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }

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


        [HttpDelete]
        public async Task RemoveKey(string Token, string Studio, string Key)
        {
            if (await ModelKeyValue.RemoveKey(Token, Studio, Key))
            {
                Response.StatusCode = Ok().StatusCode;
            }
            else
            {
                Response.StatusCode = BadRequest().StatusCode;
            }

        }


        [HttpPost]
        public async Task UpdateKey(string Token, string Studio, string Key, string Value)
        {
            if (await ModelKeyValue.UpdateKey(Token, Studio, Key, Value))
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
