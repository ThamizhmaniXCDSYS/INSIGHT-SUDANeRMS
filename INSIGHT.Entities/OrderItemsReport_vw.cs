using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities
{
    [DataContract]
    public class OrderItemsReport_vw
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual long OrderId { get; set; }
        [DataMember]
        public virtual string ControlId { get; set; }
        [DataMember]
        public virtual string Sector { get; set; }
        [DataMember]
        public virtual string ContingentType { get; set; }
        [DataMember]
        public virtual string Name { get; set; }
        [DataMember]
        public virtual string Period { get; set; }
        [DataMember]
        public virtual long Week { get; set; }
        [DataMember]
        public virtual decimal? TotalOrderQty { get; set; }
        [DataMember]
        public virtual decimal? TotalSectorPrice { get; set; }
        [DataMember]
        public virtual decimal? TotalAcceptedQty { get; set; }
        [DataMember]
        public virtual decimal? TotalDeliveredQty { get; set; }
        [DataMember]
        public virtual decimal? TotalRemainingOrdQty { get; set; }
        [DataMember]
        public virtual decimal? NetAmt { get; set; }
        [DataMember]
        public virtual decimal? OrderValue { get; set; }
        [DataMember]
        public virtual decimal? DeliveryDeduction { get; set; }
        [DataMember]
        public virtual decimal? LineItemDeduction { get; set; }
        [DataMember]
        public virtual decimal? OrderWeightDeduction { get; set; }
        [DataMember]
        public virtual decimal? ComplaintsDeduction { get; set; }
        [DataMember]
        public virtual decimal? APLDeductionTOTAL { get; set; }
        [DataMember]
        public virtual string PeriodYear { get; set; }
    }
}
