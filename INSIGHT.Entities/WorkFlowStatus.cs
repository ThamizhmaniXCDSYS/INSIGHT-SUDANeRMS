using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities
{
    [DataContract]
    public partial class WorkFlowStatus
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual long TemplateId { get; set; }
        [DataMember]
        public virtual string WFStatus { get; set; }
        [DataMember]
        public virtual string Description { get; set; }
        [DataMember]
        public virtual int ActivityOrder { get; set; }
        [DataMember]
        public virtual int PreviousActOrder { get; set; }
        [DataMember]
        public virtual int NextActOrder { get; set; }
        [DataMember]
        public virtual string Performer { get; set; }
        [DataMember]
        public virtual bool IsRejectionRequired { get; set; }
        [DataMember]
        public virtual int RejectionFor { get; set; }
        [DataMember]
        public virtual bool IsOptional { get; set; }
        [DataMember]
        public virtual int OptionalFor { get; set; }
    }
}
