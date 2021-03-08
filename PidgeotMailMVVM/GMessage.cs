using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MimeKit;

namespace PidgeotMailMVVM
{
    public class GMessage
    {
        public MimeMessage message { get; set; }
        public string MessageId { get; set; }
        public string Subject
        {
            get
            {
                return (string.IsNullOrEmpty(message.Subject)) ? "None subject" : message.Subject;
            }
        }
        public GMessage(string id = "", MimeMessage m = null)
        {
            message = m;
            MessageId = id;
        }
    }
}
