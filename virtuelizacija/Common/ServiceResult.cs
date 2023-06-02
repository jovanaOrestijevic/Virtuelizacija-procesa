using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Common
{
    [DataContract]
    public class ServiceResult
    {
        [DataMember]
        public List<Load> Loads { get; set; } = new List<Load>();
        [DataMember]
        public List<Audit> Audits { get; set; } = new List<Audit>();
    }
}