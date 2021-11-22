using System;
using System.Linq;
using System.Collections.Generic;

namespace Shinobu.Models.Assets.Event.Winter
{
    public class EventQuery
    {
        public string Query = "";
        public List<string>? Answer = null;

        public bool IsValidAnswer(string message)
        {
            message = message.Trim().ToLower();
            
            if (null == Answer)
            {
                return Query.ToLower() == message;
            }

            return Answer.Contains(message, StringComparer.OrdinalIgnoreCase);
        }
    }
}