using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace INSIGHT.Entities
{
    [DataContract]
    public class GCCRevised
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual long UNCode { get; set; }
        [DataMember]
        public virtual string Commodity { get; set; }
        [DataMember]
        public virtual decimal OrderedQty { get; set; }
        [DataMember]
        public virtual decimal DeliveredQty { get; set; }
        [DataMember]
        public virtual decimal NoOfPacks { get; set; }
        [DataMember]
        public virtual decimal NoOfPieces { get; set; }
        [DataMember]
        public virtual long SubstituteItemCode { get; set; }
        [DataMember]
        public virtual string SubstituteItemName { get; set; }
        [DataMember]
        public virtual string UOM { get; set; }
        [DataMember]
        public virtual string IssueType { get; set; }
        [DataMember]
        public virtual string Remarks { get; set; }
        [DataMember]
        public virtual string Authorized { get; set; }
        [DataMember]
        public virtual decimal ReceivedQty { get; set; }
        [DataMember]
        public virtual DateTime? ReceivedDate { get; set; }
        [DataMember]
        public virtual string ControlId { get; set; }
        [DataMember]
        public virtual string DeliveryNoteName { get; set; }
        [DataMember]
        public virtual string DeliveryNoteType { get; set; }
        [DataMember]
        public virtual string DeliveryMode { get; set; }
        [DataMember]
        public virtual DateTime? ApprovedDeliveryDate { get; set; }
        [DataMember]
        public virtual DateTime? ShipmentDate { get; set; }
        [DataMember]
        public virtual DateTime? ActualWarehouseShippedOutDate { get; set; }
        [DataMember]
        public virtual string ImpPeriod { get; set; }
        [DataMember]
        public virtual Int64 Week { get; set; }
        [DataMember]
        public virtual decimal ConsumptionWeek { get; set; }
        [DataMember]
        public virtual decimal DeliveryWeek { get; set; }
        [DataMember]
        public virtual decimal RequestNo { get; set; }
        [DataMember]
        public virtual string Warehouse { get; set; }
        [DataMember]
        public virtual decimal Strength { get; set; }
        [DataMember]
        public virtual decimal DOS { get; set; }
        [DataMember]
        public virtual decimal ManDays { get; set; }
        [DataMember]
        public virtual string CreatedBy { get; set; }
        [DataMember]
        public virtual DateTime CreatedDate { get; set; }
        [DataMember]
        public virtual string Period { get; set; }
        [DataMember]
        public virtual string PeriodYear { get; set; }

        [DataMember]
        public virtual decimal RemainingQty { get; set; }
        [DataMember]
        public virtual decimal OrderId { get; set; }
        [DataMember]
        public virtual decimal PODId { get; set; }
        [DataMember]
        public virtual decimal LineId { get; set; }
        [DataMember]
        public virtual string Sector { get; set; }
        [DataMember]
        public virtual string Name { get; set; }
        [DataMember]
        public virtual string Location { get; set; }
        [DataMember]
        public virtual string IsValid { get; set; }
        [DataMember]
        public virtual string StorerKey { get; set; }
        [DataMember]
        public virtual long DeliveryNoteId { get; set; }
        [DataMember]
        public virtual long RequestId { get; set; }
    }

    public class GCCRevisedQueryList
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual string Step { get; set; }
        [DataMember]
        public virtual string Query { get; set; }
    }
}
