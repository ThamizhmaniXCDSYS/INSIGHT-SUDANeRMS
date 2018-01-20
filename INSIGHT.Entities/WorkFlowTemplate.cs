using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities
{
    [DataContract]
    public class WorkFlowTemplate
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual string TemplateName { get; set; }
        [DataMember]
        public virtual string Description { get; set; }
    }
}
