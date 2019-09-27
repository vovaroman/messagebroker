using System;

namespace messageBroker
{
    public interface IClient
    {
        string ClientName {get;set;}
        string ClientIp {get;set;}

    }
}