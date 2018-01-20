using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities
{
    [DataContract]
    public class OrderItems
    {
        [DataMember]
        public virtual long LineId { get; set; }
        [DataMember]
        public virtual long OrderId { get; set; }
        [DataMember]
        public virtual long UNCode { get; set; }
        [DataMember]
        public virtual string Commodity { get; set; }
        [DataMember]
        public virtual decimal OrderQty { get; set; }


        [DataMember]
        public virtual decimal SectorPrice { get; set; }
        [DataMember]
        public virtual decimal Total { get; set; }
        [DataMember]
        public virtual string CreatedBy { get; set; }
        [DataMember]
        public virtual DateTime? CreatedDate { get; set; }
        [DataMember]
        public virtual string ModifiedBy { get; set; }
        [DataMember]
        public virtual DateTime? ModifiedDate { get; set; }

        [DataMember]
        public virtual decimal AcceptedOrdQty { get; set; }
        [DataMember]
        public virtual decimal DeliveredOrdQty { get; set; }
        [DataMember]
        public virtual decimal RemainingOrdQty { get; set; }
        [DataMember]
        public virtual string Status { get; set; }
        [DataMember]
        public virtual string SubstituteItemName { get; set; }
        [DataMember]
        public virtual long SubstituteItemCode { get; set; }
        [DataMember]
        public virtual decimal InvoiceQty { get; set; }
        [DataMember]
        public virtual decimal InvoiceValue { get; set; }
        [DataMember]
        public virtual string DiscrepancyCode { get; set; }
        //Added by Thamizhmani
        [DataMember]
        public virtual long Line_No { get; set; }
        [DataMember]
        public virtual string UOM { get; set; }
        [DataMember]
        public virtual string NPACode { get; set; }
    }
}
