using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities
{
    [DataContract]
    public class DeliveredPODItems_vw
    {
        [DataMember]
        public virtual long Id { get; set; }
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
        public virtual decimal RemainingQty { get; set; }
        [DataMember]
        public virtual string Name { get; set; }

        [DataMember]
        public virtual string Period { get; set; }
       
        [DataMember]
        public virtual Int64 Week { get; set; }
        [DataMember]
        public virtual long UNCode { get; set; }
        [DataMember]
        public virtual string Commodity { get; set; }
        [DataMember]
        public virtual string Status { get; set; }
        [DataMember]
        public virtual string SubstituteItemName { get; set; }
        [DataMember]
        public virtual long SubstituteItemCode { get; set; }
        [DataMember]
        public virtual DateTime? DeliveredDate { get; set; }
        [DataMember]
        public virtual long DeliveryNoteId { get; set; }
        [DataMember]
        public virtual DateTime ExpectedDeliveryDate { get; set; }
        [DataMember]
        public virtual string DeliveryNoteName { get; set; }
        [DataMember]
        public virtual string Sector { get; set; }
        [DataMember]
        public virtual string Location { get; set; }
        [DataMember]
        public virtual decimal RemQtyForAccQty { get; set; }

        /// <summary>
        /// added by felix kinoniya
        /// </summary>
        [DataMember]
        public virtual string UOM { get; set; }
        [DataMember]
        public virtual string ControlId { get; set; }
        [DataMember]
        public virtual string DiscrepancyCode { get; set; }
    }
}
