using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace INSIGHT.Entities
{
    public class ImportedExpDelDate
    {
        [DataMember]
        public virtual long  ImpExpDelDtId{get;set;}
        [DataMember]
        public virtual string ControlId { get; set; }
        [DataMember]
        public virtual string Sector { get; set; }
        [DataMember]
        public virtual string Name { get; set; }
        [DataMember]
        public virtual string Location { get; set; }
        [DataMember]
        public virtual string Period { get; set; }
        [DataMember]
        public virtual string PeriodYear { get; set; }
        [DataMember]
        public virtual DateTime ImpExpDeliveryDate { get; set; }
        [DataMember]
        public virtual string CreatedBy { get; set; }
        [DataMember]
        public virtual DateTime CreatedDate { get; set; }
        [DataMember]
        public virtual string ModifiedBy { get; set; }
        [DataMember]
        public virtual DateTime ModifiedDate { get; set; }
        [DataMember]
        public virtual long RequestId { get; set; }

    }
}
