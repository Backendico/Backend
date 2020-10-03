using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend2.Pages.Apis.SignalR.PageSupport
{
    [Controller]
    public class PageSupport:Hub
    {
        public  async Task support(string user,string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

    }
}
