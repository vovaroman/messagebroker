using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using messageBroker;

namespace messageBroker
{
    public static class Helper
    {

        public static int UdpPort = 616;

        public static int TcpPort = 13000;

        public static bool IsClientExists(IClient client)
        {
            foreach (var x in ClientsControl.clients){
                if(x.ClientName == client.ClientName) return true;
            }
            return false;
        }
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        public static string ExternalIp()
        {
            var addresses = Dns.GetHostEntry((Dns.GetHostName()))
                    .AddressList
                    .Where(x => x.AddressFamily == AddressFamily.InterNetwork)
                    .Select(x => x.ToString())
                    .ToArray();

            // foreach(var ip in addresses)
            // {
            //     Console.WriteLine(ip);
            // }
            return addresses[1];
        }




    }
}



