using System;
using System.Collections.Generic;

namespace messageBroker.Models
{
    public class Client : IClient
    {
        public string ClientName {get;set;}
        public string ClientIp { get;set; }

        public int UdpPort {get; set;}
        public List<string> Categories {get;set;} = new List<string>();

    }
}