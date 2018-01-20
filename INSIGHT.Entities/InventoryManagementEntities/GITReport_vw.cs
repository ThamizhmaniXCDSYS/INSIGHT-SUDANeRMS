using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace INSIGHT.Entities.InventoryManagementEntities
{
    [DataContract]
    public class GITReport_vw
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual long PONumber { get; set; }
        [DataMember]
        public virtual string POStatus { get; set; }
        [DataMember]
        public virtual long UNCode { get; set; }
        [DataMember]
        public virtual string Commodity { get; set; }
        [DataMember]
        public virtual decimal OrderQty { get; set; }
        
        [DataMember]
        public virtual decimal ShippedQty { get; set; }
        [DataMember]
        public virtual decimal POUnitPrice { get; set; }
        [DataMember]
        public virtual string InvoiceCurrency { get; set; }
        [DataMember]
        public virtual decimal InvoiceUnitPrice { get; set; }
        [DataMember]
        public virtual decimal InvoiceAmount { get; set; }
        [DataMember]
        public virtual decimal ExchangeRate { get; set; }
        [DataMember]
        public virtual decimal InvoiceValueUSD { get; set; }
        [DataMember]
        public virtual decimal UnitPriceUSD { get; set; }
        [DataMember]
        public virtual string Supplier { get; set; }
        [DataMember]
        public virtual string BLNo { get; set; }
        [DataMember]
        public virtual string ContainerNo { get; set; }
        [DataMember]
        public virtual string InvoiceNo { get; set; }
        [DataMember]
        public virtual DateTime? InvoiceDate { get; set; }
        [DataMember]
        public virtual long OracleVoucherNo { get; set; }
        [DataMember]
        public virtual DateTime? Updates { get; set; }
        [DataMember]
        public virtual decimal OpeningGITBal { get; set; }
        [DataMember]
        public virtual string Sector { get; set; }
        [DataMember]
        public virtual decimal InventoryQty { get; set; }
        [DataMember]
        public virtual decimal InventoryValue { get; set; }
        [DataMember]
        public virtual decimal VarianceQty { get; set; }
        [DataMember]
        public virtual decimal VarianceValueperIPCRate { get; set; }
        [DataMember]
        public virtual decimal VarianceValue { get; set; }
        [DataMember]
        public virtual string Remarks { get; set; }
    }
}
