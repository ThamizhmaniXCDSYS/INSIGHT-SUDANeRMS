using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace INSIGHT.Entities.TicketingSystem
{
    [DataContract]
    public class TicketStatus
    {
        [DataMember]
        public virtual long StatusId { get; set; }
        [DataMember]
        public virtual string StatusCode { get; set; }
        [DataMember]
        public virtual string StatusName { get; set; }
    }
}