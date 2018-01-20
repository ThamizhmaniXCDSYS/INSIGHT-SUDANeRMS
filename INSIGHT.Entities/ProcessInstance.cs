using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities
{
    [DataContract]
    public partial class ProcessInstance
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual long TemplateId { get; set; }
        [DataMember]
        public virtual string CreatedBy { get; set; }
        [DataMember]
        public virtual string Status { get; set; }
        [DataMember]
        public virtual DateTime? DateCreated { get; set; }
        [DataMember]
        public virtual TimeSpan? DifferenceInHours { get; set; }
        [DataMember]
        public virtual int Hours { get; set; }
        //[DataMember]
        //public virtual WorkFlowTemplate WorkFlowTemplate { get; set; }

    }
}
