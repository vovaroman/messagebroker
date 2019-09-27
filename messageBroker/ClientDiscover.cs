using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using messageBroker.Models;
using System.Collections.Generic;
using MySql;
using System.Linq;
using messageBroker.Connectors;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Xml;

namespace messageBroker
{
    public static class ClientDiscover
    {
        private  static IPAddress localAddr = IPAddress.Parse(Helper.ExternalIp());

        private static TcpListener tcpServer;

        private static UdpClient udpServer = new UdpClient(Helper.UdpPort);
        //8080

        private static void listenTCPClients()
        {
            Console.WriteLine(localAddr);
            tcpServer = new TcpListener(localAddr, Helper.TcpPort);
            tcpServer.Start();
            Byte[] bytes = new Byte[256];
            String data = null;
            while(true)
            {
                Console.Write("Waiting for a connection... ");
                TcpClient client = tcpServer.AcceptTcpClient();
                Console.WriteLine("ping!");
                NetworkStream stream = client.GetStream();
                int i;
                while((i = stream.Read(bytes, 0, bytes.Length))!=0)
                {
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    var splittedData = data.Split(new char[]{'-'});
                    var action = splittedData[0].TrimStart('[');
                    var name = splittedData[1];
                    var port = int.Parse(splittedData[2].TrimEnd(']'));
                    switch(action)
                    {
                        case "HELLO":
                            var tempReceiver = new Client();
                            tempReceiver.ClientIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                            tempReceiver.ClientName = name;
                            tempReceiver.UdpPort = port;
                            tempReceiver.Categories = new MysqlConnector().GetUserCategories(tempReceiver.ClientName);
                            var chanelsToSend = new Dictionary<string, string>();
                            chanelsToSend.Add("action","getChannels");
                            chanelsToSend.Add("value",Newtonsoft.Json.JsonConvert.SerializeObject(tempReceiver.Categories));
                            var channelsJson = Newtonsoft.Json.JsonConvert.SerializeObject(chanelsToSend);

                            SendData(channelsJson,tempReceiver.ClientIp,tempReceiver.UdpPort);
                            if (!Helper.IsClientExists(tempReceiver))
                            {
                                ClientsControl.clients.Add(tempReceiver);
                            }
                            break;
                        case "CLOSED":
                            ClientsControl.clients.Remove(
                                ClientsControl.clients.Where
                                (x => x.ClientName == name).FirstOrDefault()
                            );
                            break;
                    }
                    
                }
                client.Close();
            }

        }


        public static void Get(JObject data, string host, int port)
        {
            var channel = data["channel"].ToString();
            var messages = new MysqlConnector().GetMessagesForCategory(channel);
            string output = string.Empty;
            foreach(var message in messages)
            {
                var replacedMessage = ContentEnchanter.EnchantMessage(message.Message);
                output += $"<div class=\"incoming_msg\"><div class=\"received_msg\"><div class=\"received_withd_msg\"><p>{replacedMessage} - {message.Author} - {message.Timestamp}</p></div></div></div><br/>";
            }
            var chanelsToSend = new Dictionary<string, string>();
            chanelsToSend.Add("action","getMessages");
            chanelsToSend.Add("value",Newtonsoft.Json.JsonConvert.SerializeObject(output));
            var channelsJson = Newtonsoft.Json.JsonConvert.SerializeObject(chanelsToSend);

            SendData(channelsJson, host, port);
            
        }


        public static void Send(JObject data, string Name)
        {
            var xml = Translator.TranslateJsonToXml(data.ToString());
            XmlElement xRoot = xml.DocumentElement;
            var message = xRoot["message"].InnerText;
            var channel = xRoot["channel"].InnerText;;
            new MysqlConnector().InsertMessage(Name, channel, message);
        }

        public static void listenUdp()
        {
            while (true)
            {
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(
                    IPAddress.Any,
                    Helper.UdpPort
                    );
                Byte[] receiveBytes = udpServer.Receive(ref RemoteIpEndPoint);
                string returnData = Encoding.ASCII.GetString(receiveBytes);
                JObject data = new JObject();
                if (Translator.IsJson(returnData))
                {
                    data = JObject.Parse(returnData);
                }
                Console.WriteLine(data);
                switch(data["action"].ToString())
                {
                    case "send":
                        Send(data, data["name"].ToString());
                        break;
                    case "get":
                        Get(data,RemoteIpEndPoint.Address.ToString(),
                            ClientsControl.clients.Where(c => c.ClientIp ==
                             RemoteIpEndPoint.Address.ToString()).FirstOrDefault().UdpPort
                        );
                        break;
                }



            }
        }

        public static void SendData(string mymessage, string host, int port)
        {
            UdpClient client = new UdpClient();
            string message = mymessage;
            byte[] data = Encoding.UTF8.GetBytes(message);
            client.Send(data, data.Length, host, port);
            client.Close();
        }

        public static void Discover()
        {
            var searchClients =  new Thread(() => listenTCPClients());
            searchClients.Start();
            var udpServerThread = new Thread(() => listenUdp());
            udpServerThread.Start();
        }
    }
}
