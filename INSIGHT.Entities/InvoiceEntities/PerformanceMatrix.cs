﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities.InvoiceEntities
{
    [DataContract]
    public class PerformanceMatrix
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual long OrderId { get; set; }
        [DataMember]
        public virtual string Period { get; set; }
        [DataMember]
        public virtual long Week { get; set; }
        [DataMember]
        public virtual string ContingentType { get; set; }
        [DataMember]
        public virtual string Sector { get; set; }
        [DataMember]
        public virtual string Name { get; set; }
        [DataMember]
        public virtual long TotalLineItems { get; set; }
        [DataMember]
        public virtual long PercentCount { get; set; }
        [DataMember]
        public virtual decimal OrderQty { get; set; }
        [DataMember]
        public virtual decimal DeliveredQty { get; set; }
        [DataMember]
        public virtual decimal AcceptedQty { get; set; }
        [DataMember]
        public virtual decimal OrderValue { get; set; }
        [DataMember]
        public virtual decimal InvoiceValue { get; set; }
        [DataMember]
        public virtual DateTime Deliverydate { get; set; }
        [DataMember]
        public virtual DateTime ExpectedDeliveryDate { get; set; }
        [DataMember]
        public virtual long NofDateDelay { get; set; }
        [DataMember]
        public virtual long SubtituteCount { get; set; }
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
        public virtual decimal APLDeductionTOTAL { get; set; }

        [DataMember]
        public virtual decimal Troops { get; set; }
        [DataMember]
        public virtual decimal Mandays { get; set; }
    }
}
