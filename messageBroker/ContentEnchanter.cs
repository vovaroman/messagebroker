using System;
using System.Collections.Generic;

namespace messageBroker
{
    public static class ContentEnchanter
    {
        private static Dictionary<string, string> words = new Dictionary<string, string>()
        {
            {"Hi","Hello" },
            { "pretik","Hi"}
        };



        public static string EnchantMessage(string message)
        {
            foreach(var word in words)
            {
                message = message.Replace(word.Key,word.Value);

            }
            return message;
        }
    }
}
