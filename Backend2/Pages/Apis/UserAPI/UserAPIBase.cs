using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend2.Pages.Apis.UserAPI
{
    public class UserAPIBase : ControllerBase
    {
        MongoClient Client = new MongoClient();

    }
}
