using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace INSIGHT.Entities.InvoiceEntities
{
    [DataContract]
    public class PerformanceCalculateView
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual long OrderId { get; set; }
        [DataMember]
        public virtual string Period { get; set; }
        [DataMember]
        public virtual string ControlId { get; set; }
        [DataMember]
        public virtual long Week { get; set; }
        [DataMember]
        public virtual string Name { get; set; }
        [DataMember]
        public virtual string ContingentType { get; set; }
        [DataMember]
        public virtual string Sector { get; set; }
        [DataMember]
        public virtual decimal DeliveryPerformance { get; set; }
        [DataMember]
        public virtual decimal DeliveryDeduction { get; set; }
        [DataMember]
        public virtual decimal LineItemPerformance { get; set; }
        [DataMember]
        public virtual decimal LineItemDeduction { get; set; }
        [DataMember]
        public virtual decimal OrderWeightPerformance { get; set; }
        [DataMember]
        public virtual decimal OrderWeightDeduction { get; set; }
        [DataMember]
        public virtual decimal ComplaintsPerformance { get; set; }
        [DataMember]
        public virtual decimal ComplaintsDeduction { get; set; }
        [DataMember]
        public virtual string PeriodYear { get; set; }
    }
}
