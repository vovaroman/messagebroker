using System;
namespace messageBroker.Models
{
    public class MessageModel
    {
        public string ID { get; set; }

        public string Category { get; set; }

        public string Message { get; set; }

        public string Timestamp { get; set; }

        public string Author { get; set; }
     
    }
}
