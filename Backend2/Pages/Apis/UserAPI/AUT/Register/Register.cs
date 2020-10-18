using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend2.Pages.Apis.UserAPI.AUT.Register
{
    [Controller]
    public class Register : UserAPIBase
    {
        [HttpPost]
        public Task<string> Token()
        {
            return Task.FromResult("");
        }


        [HttpPost]
        public Task<string> UsernamePassword()
        {
            return Task.FromResult("");
        }

        [HttpPost]
        public Task<string> Email()
        {
            return Task.FromResult("");
        }


    }
}
