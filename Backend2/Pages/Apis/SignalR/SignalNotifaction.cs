using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace Backend2.Pages.Apis.SignalR
{

    public class SignalNotifaction : Hub
    {
        public static Dictionary<ObjectId, string> ClientList = new Dictionary<ObjectId, string>();

        public static IHubCallerClients Client;



        public void AddClient(string ID, string Token)
        {
            if (Client==null)
            Client = Clients;

            Debug.WriteLine(Token.ToString());

            try
            {
                ClientList.Add(ObjectId.Parse(Token), ID);

            }
            catch (Exception)
            {
                ClientList[ObjectId.Parse(Token)] = ID;
            }
        }

        public static async void SendSignal(string Token)
        {
            await Client.Client(ClientList[ObjectId.Parse(Token)]).SendAsync("Notifaction");
        }


    }



}
