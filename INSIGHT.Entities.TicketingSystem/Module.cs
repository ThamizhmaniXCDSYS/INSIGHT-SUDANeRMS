using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace INSIGHT.Entities.TicketingSystem
{
    [DataContract]
    public class Module
    {
        [DataMember]
        public virtual long ModuleId { get; set; }
        [DataMember]
        public virtual string ModuleCode { get; set; }
        [DataMember]
        public virtual string ModuleName { get; set; }
    }
}
