using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities.TicketingSystem
{
    [DataContract]
    public class Priority
    {
        [DataMember]
        public virtual long PriorityId { get; set; }
        [DataMember]
        public virtual string PriorityCode { get; set; }
        [DataMember]
        public virtual string PriorityName { get; set; }
    }
}
