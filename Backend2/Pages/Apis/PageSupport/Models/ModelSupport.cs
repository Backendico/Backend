using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Backend2.Pages.Apis.PageSupport.Models
{
    public class ModelSupport
    {
        public ObjectId _id;
        public string Header;
        public Section Section;
        public Priority Priority;
    }

    public enum Section
    {
        Other,
        Authentication,
        Dashboard,
        Payments
    }

    public enum Priority
    {
        Normal,Hight,Low
    }
}
