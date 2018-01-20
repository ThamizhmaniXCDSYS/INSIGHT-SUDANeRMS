using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace INSIGHT.Entities.TicketingSystem
{
    [DataContract]
    public class TicketComments
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual long TicketId { get; set; }
        [DataMember]
        public virtual string CommentedBy { get; set; }
        [DataMember]
        public virtual DateTime? CommentedOn { get; set; }
        [DataMember]
        public virtual string RejectionComments { get; set; }
        [DataMember]
        public virtual string ResolutionComments { get; set; }
        [DataMember]
        public virtual string Note { get; set; }
        [DataMember]
        public virtual string IsRejectionOrResolution { get; set; }
        [DataMember]
        public virtual string ActivityName { get; set; }
    }
}
