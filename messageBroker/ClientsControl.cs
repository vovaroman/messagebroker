using System;
using System.Collections.Generic;
using messageBroker;
using messageBroker.Models;

namespace messageBroker
{
    public static class ClientsControl
    {
        public static List<Client> clients;
        public static void Init()
        {
            clients = new List<Client>();
        }
    }
}