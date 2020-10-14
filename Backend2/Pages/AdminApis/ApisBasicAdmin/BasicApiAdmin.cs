using Backend2.Pages.Apis.SignalR;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Backend2.Pages.AdminApis.ApisBasicAdmin
{
    public class BasicApiAdmin : ControllerBase
    {
        internal MongoClient Client = new MongoClient();
        internal string AdminDatabase => "Users";
        internal string AdminCollection => "Admins";

        internal void SignalNotifaction(string Token)
        {
            Apis.SignalR.SignalNotifaction.SendSignal(Token.ToString());
        }



    }
}
