using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class DataMessage
    {
        public DataMessage(string message, int timeToCreateMessage)
        {
            Message = message;
            TimeToCreateMessage = timeToCreateMessage;
        }

        public string Message { get; set; }
        public int TimeToCreateMessage { get; set; }
        public int ProcessingTime { get; set; }
    }
}
