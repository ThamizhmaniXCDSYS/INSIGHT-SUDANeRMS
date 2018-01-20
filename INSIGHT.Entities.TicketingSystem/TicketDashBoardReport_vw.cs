using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace INSIGHT.Entities.TicketingSystem
{
    [DataContract]
    public class TicketDashBoardReport_vw
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual string Performer { get; set; }
        [DataMember]
        public virtual long GeneralAvailable { get; set; }
        [DataMember]
        public virtual long RejectedCount { get; set; }
        //[DataMember]
        //public virtual long Available { get; set; }
        [DataMember]
        public virtual long Assigned { get; set; }
        [DataMember]
        public virtual long Resolved { get; set; }
        [DataMember]
        public virtual long Completed { get; set; }
        [DataMember]
        public virtual DateTime CreatedDate { get; set; }
    }
}
