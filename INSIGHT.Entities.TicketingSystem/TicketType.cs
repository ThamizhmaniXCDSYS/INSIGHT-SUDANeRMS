using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace INSIGHT.Entities.TicketingSystem
{
    [DataContract]
    public class TicketType
    {
        [DataMember]
        public virtual long TicketTypeId { get; set; }
        [DataMember]
        public virtual string TicketTypeCode { get; set; }
        [DataMember]
        public virtual string TicketTypeName { get; set; }
    }
}
