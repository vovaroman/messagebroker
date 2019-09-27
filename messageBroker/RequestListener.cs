using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using messageBroker;

namespace messageBroker
{
    public struct RequestCategories
    {
        public const string GetDataForChanel = "GetDataForChanel";
        public const string SendDataToChanel = "SendDataToChanel";

    }
    public static class RequestListener
    {
        static UdpClient receivingUdpClient = new UdpClient(12000);

 //Creates an IPEndPoint to record the IP Address and port number of the sender. 
        // The IPEndPoint will allow you to read datagrams sent from any source.
        static IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);


        public static void listenRequests(){
             try{

     // Blocks until a message returns on this socket from a remote host.
                Byte[] receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint); 
                string returnData = Encoding.ASCII.GetString(receiveBytes);
            
                Console.WriteLine("This is the message you received " +
                                            returnData.ToString());
                var request = returnData.ToString().Split(new char[] {';'});
                switch (request[0])
                {
                    case RequestCategories.GetDataForChanel:
                    
                    break;
                    case RequestCategories.SendDataToChanel: break;
                }
                Console.WriteLine("This message was sent from " +
                                            RemoteIpEndPoint.Address.ToString() +
                                            " on their port number " +
                                            RemoteIpEndPoint.Port.ToString());
            }
            catch ( Exception e ){
                Console.WriteLine(e.ToString()); 
            }
        }
    }
}