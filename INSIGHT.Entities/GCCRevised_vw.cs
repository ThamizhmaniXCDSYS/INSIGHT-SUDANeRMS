using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace INSIGHT.Entities
{
    public class GCCRevised_vw
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual long LineId { get; set; }
        [DataMember]
        public virtual long OrderId { get; set; }
        [DataMember]
        public virtual string Sector { get; set; }
        [DataMember]
        public virtual string ContingentType { get; set; }
        [DataMember]
        public virtual string Period { get; set; }
        [DataMember]
        public virtual string PeriodYear { get; set; }
        [DataMember]
        public virtual string Name { get; set; }
        [DataMember]
        public virtual string ControlId { get; set; }
        [DataMember]
        public virtual DateTime ExpectedDeliveryDate { get; set; }
        [DataMember]
        public virtual string DeliveryNoteName { get; set; }
        [DataMember]
        public virtual string DeliveryMode { get; set; }
        [DataMember]
        public virtual string DeliveredDate { get; set; }
        [DataMember]
        public virtual long UNCode { get; set; }
        [DataMember]
        public virtual string Commodity { get; set; }
        [DataMember]
        public virtual decimal OrderQty { get; set; }
        [DataMember]
        public virtual string SubstituteItemName { get; set; }
        [DataMember]
        public virtual string SubstituteItemCode { get; set; }
        [DataMember]
        public virtual decimal DeliveredQty { get; set; }
        [DataMember]
        public virtual decimal AcceptedQty { get; set; }
        [DataMember]
        public virtual decimal RemainingQty { get; set; }
        [DataMember]
        public virtual string DeliveredDate1 { get; set; }
        [DataMember]
        public virtual decimal InvoiceQty { get; set; }
        [DataMember]
        public virtual decimal DeliveredValue { get; set; }
        [DataMember]
        public virtual decimal AcceptedValue { get; set; }
        [DataMember]
        public virtual decimal FoodValue { get; set; }
        [DataMember]
        public virtual decimal TransportValue { get; set; }
        [DataMember]
        public virtual decimal InsuranceValue { get; set; }
        //Added by Thamizhmani for FFV & NON-FFV on 25 Aug 2016
        [DataMember]
        public virtual string FoodType { get; set; }
        [DataMember]
        public virtual string InvoiceNumber { get; set; }
        [DataMember]
        public virtual long ContingentID { get; set; }
    }
}
