using System;
using System.Threading;
using messageBroker;

namespace messageBroker
{
    class Program
    {
        static void Main(string[] args)
        {
            ClientsControl.Init();
            var discoverClients = new Thread(() => ClientDiscover.Discover());
            discoverClients.Start();
        }
    }
}
