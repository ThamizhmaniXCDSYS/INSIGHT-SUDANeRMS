using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities
{
    [DataContract]
    public class InvoiceNumberMaster
    {
        [DataMember]
        public virtual long InvoiceMasterId { get; set; }
        [DataMember]
        public virtual string ControlId { get; set; }
        [DataMember]
        public virtual string InvoiceNumber { get; set; }
        [DataMember]
        public virtual string PeriodYear { get; set; }
        [DataMember]
        public virtual string Period { get; set; }
        [DataMember]
        public virtual long Week { get; set; }
        [DataMember]
        public virtual long OrderId { get; set; }
        [DataMember]
        public virtual long RequestId { get; set; }
        [DataMember]
        public virtual string CreatedBy { get; set; }
        [DataMember]
        public virtual DateTime? CreatedDate { get; set; }
        [DataMember]
        public virtual string ModifiedBy { get; set; }
        [DataMember]
        public virtual DateTime? ModifiedDate { get; set; }
        [DataMember]
        public virtual string IsValid { get; set; }
    }
}
