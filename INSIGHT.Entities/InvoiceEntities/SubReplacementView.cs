using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace INSIGHT.Entities.InvoiceEntities
{
    public class SubReplacementView
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual long LineId { get; set; }
        [DataMember]
        public virtual long OrderId { get; set; }
        [DataMember]
        public virtual long PODId { get; set; }
        [DataMember]
        public virtual long UNCode { get; set; }
        [DataMember]
        public virtual string Commodity { get; set; }
        [DataMember]
        public virtual decimal SectorPrice { get; set; }

        [DataMember]
        public virtual decimal OrderedQty { get; set; }
        [DataMember]
        public virtual decimal InvoiceQty { get; set; }
        [DataMember]
        public virtual decimal DeliveredQty { get; set; }
        [DataMember]
        public virtual decimal AcceptedAmt { get; set; }
             

        [DataMember]
        public virtual long SubstituteItemCode { get; set; }
        [DataMember]
        public virtual string SubstituteItemName { get; set; }
        [DataMember]
        public virtual decimal SubstituteSectorPrice { get; set; }

        [DataMember]
        public virtual string DeliveryNoteName { get; set; }
        [DataMember]
        public virtual string DiscrepancyCode { get; set; }
        [DataMember]
        public virtual string DeliverySector { get; set; }
        [DataMember]
        public virtual decimal APLWeight { get; set; }
        [DataMember]
        public virtual string DiscCode { get; set; }
        
    }
}
