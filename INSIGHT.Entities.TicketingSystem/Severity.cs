using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities.TicketingSystem
{
    [DataContract]
    public class Severity
    {
        [DataMember]
        public virtual long SeverityId { get; set; }
        [DataMember]
        public virtual string SeverityCode { get; set; }
        [DataMember]
        public virtual string SeverityName { get; set; }
    }
}
