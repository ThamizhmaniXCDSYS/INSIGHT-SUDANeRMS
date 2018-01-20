using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities.InvoiceEntities
{
    public class DeliveriesPerOrdQty
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual long LineId { get; set; }
        [DataMember]
        public virtual long OrderId { get; set; }
        [DataMember]
        public virtual long PODItemsId { get; set; }
        [DataMember]
        public virtual long PODId { get; set; }

        [DataMember]
        public virtual long UNCode { get; set; }
        [DataMember]
        public virtual string Commodity { get; set; }
        [DataMember]
        public virtual decimal SectorPrice { get; set; }

        [DataMember]
        public virtual decimal InvoiceQty { get; set; }
        [DataMember]
        public virtual decimal OrderedQty { get; set; }
        [DataMember]
        public virtual decimal DeliveredQty { get; set; }
        [DataMember]
        public virtual decimal AcceptedQty { get; set; }
        [DataMember]
        public virtual DateTime DeliveredDate { get; set; }
        [DataMember]
        public virtual string CreatedBy { get; set; }
        //[DataMember]
        //public virtual DateTime CreatedDate { get; set; }
        //[DataMember]
        //public virtual string ModifiedBy { get; set; }
        //[DataMember]
        //public virtual DateTime ModifiedDate { get; set; }

        [DataMember]
        public virtual decimal Total { get; set; }
        [DataMember]
        public virtual decimal RemainingOrdQty { get; set; }
        [DataMember]
        public virtual decimal Difference { get; set; }
        [DataMember]
        public virtual decimal DifferencePercent { get; set; }

        [DataMember]
        public virtual string Period { get; set; }
        [DataMember]
        public virtual string Sector { get; set; }
        [DataMember]
        public virtual long Week { get; set; }
        [DataMember]
        public virtual long DeliveryNoteId { get; set; }
        [DataMember]
        public virtual string DeliveryNoteName { get; set; }
        [DataMember]
        public virtual decimal ActualPackPrice { get; set; }
        [DataMember]
        public virtual string UOM { get; set; }

    }
}
