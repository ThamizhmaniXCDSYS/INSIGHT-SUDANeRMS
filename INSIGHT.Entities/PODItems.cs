using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities
{
    [DataContract]
    public partial class PODItems
    {
        [DataMember]
        public virtual long PODItemsId { get; set; }
        [DataMember]
        public virtual long PODId { get; set; }
        [DataMember]
        public virtual long OrderId { get; set; }
        [DataMember]
        public virtual long LineId { get; set; }
        [DataMember]
        public virtual decimal OrderedQty { get; set; }
        [DataMember]
        public virtual decimal DeliveredQty { get; set; }
        [DataMember]
        public virtual decimal AcceptedQty { get; set; }
        [DataMember]
        public virtual DateTime? DeliveredDate { get; set; }
        [DataMember]
        public virtual string CreatedBy { get; set; }
        [DataMember]
        public virtual DateTime? CreatedDate { get; set; }
        [DataMember]
        public virtual string ModifiedBy { get; set; }
        [DataMember]
        public virtual DateTime? ModifiedDate { get; set; }
        [DataMember]
        public virtual string SubstituteItemName { get; set; }
        [DataMember]
        public virtual long SubstituteItemCode { get; set; }
        [DataMember]
        public virtual decimal RemainingQty { get; set; }
        [DataMember]
        public virtual string Status { get; set; }
        [DataMember]
        public virtual decimal SubsDeliveredQty { get; set; }
        [DataMember]
        public virtual decimal SubsAcceptedQty { get; set; }
        [DataMember]
        public virtual long DeliveryNoteId { get; set; }
        [DataMember]
        public virtual string DeliveryNoteName { get; set; }
        [DataMember]
        public virtual string DiscrepancyCode { get; set; }
        [DataMember]
        public virtual decimal InvoiceQty { get; set; }
        [DataMember]
        public virtual string DeliverySector { get; set; }
        [DataMember]
        public virtual decimal RevisedOrderQty { get; set; }
        [DataMember]
        public virtual decimal MAXINVQTY { get; set; }
    }
}
