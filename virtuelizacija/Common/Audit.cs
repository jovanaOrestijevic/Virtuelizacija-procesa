using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [DataContract]
    public class Audit
    {
        public Audit()
        {
            Timestamp = DateTime.Now;
        }

        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public DateTime Timestamp { get; set; }
        [DataMember]
        public MessageType MessageType { get; set; }
        [DataMember]
        public string Message { get; set; }


    }
}
