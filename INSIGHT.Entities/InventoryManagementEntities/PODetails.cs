using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities.InventoryManagementEntities
{
    [DataContract]
    public class PODetails
    {
        [DataMember]
        public virtual long PODetails_Id { get; set; }
        [DataMember]
        public virtual long POId { get; set; }
        [DataMember]
        public virtual long UNCode { get; set; }
        [DataMember]
        public virtual string Commodity { get; set; }
        [DataMember]
        public virtual decimal OrderQty { get; set; }
        [DataMember]
        public virtual decimal RemainingQty { get; set; }
        [DataMember]
        public virtual decimal ShippedQty { get; set; }
        [DataMember]
        public virtual decimal POUnitPrice { get; set; }
        [DataMember]
        public virtual decimal POValue { get; set; }
        [DataMember]
        public virtual string InvoiceNo { get; set; }
        [DataMember]
        public virtual DateTime? InvoiceDate { get; set; }

        [DataMember]
        public virtual decimal InvoiceUnitPrice { get; set; }
        [DataMember]
        public virtual decimal InvoiceAmount { get; set; }
        [DataMember]
        public virtual decimal InvoiceValueUSD { get; set; }
        [DataMember]
        public virtual decimal UnitPriceUSD { get; set; }
        [DataMember]
        public virtual decimal InvoiceAmountDiff { get; set; }
        [DataMember]
        public virtual string UOM { get; set; }
        [DataMember]
        public virtual string DeliveryNoteNo { get; set; }
        [DataMember]
        public virtual decimal ExchangeRate { get; set; }
        [DataMember]
        public virtual string ContainerNo { get; set; }

        [DataMember]
        public virtual DateTime? CreatedDate { get; set; }
        [DataMember]
        public virtual string CreatedBy { get; set; }
        [DataMember]
        public virtual DateTime? ModifiedDate { get; set; }
        [DataMember]
        public virtual string ModifiedBy { get; set; }

        [DataMember]
        public virtual string BLNo { get; set; }
        [DataMember]
        public virtual long OracleVoucherNo { get; set; }
        [DataMember]
        public virtual DateTime? Updates { get; set; }
    }
}
