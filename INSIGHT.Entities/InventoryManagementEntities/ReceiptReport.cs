using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities.InventoryManagementEntities
{
    [DataContract]
    public class ReceiptReport
    {
        [DataMember]
        public virtual long Receipt_Id { get; set; }
        [DataMember]
        public virtual long POId { get; set; }
        [DataMember]
        public virtual long PODetails_Id { get; set; }
        [DataMember]
        public virtual long ReceiptKey { get; set; }
        [DataMember]
        public virtual long ExternPOKey { get; set; }
        [DataMember]
        public virtual long StorerKey { get; set; }
        [DataMember]
        public virtual string Company { get; set; }
        [DataMember]
        public virtual long SKU { get; set; }
        [DataMember]
        public virtual string Description { get; set; }
        [DataMember]
        public virtual decimal QtyExpected { get; set; }
        [DataMember]
        public virtual decimal QtyReceived { get; set; }
        [DataMember]
        public virtual DateTime? DateReceived { get; set; }
        [DataMember]
        public virtual string StockType { get; set; }
        [DataMember]
        public virtual string Batch { get; set; }
        [DataMember]
        public virtual DateTime? ManDate { get; set; }
        [DataMember]
        public virtual DateTime? ReceiptDate { get; set; }
        [DataMember]
        public virtual DateTime? ExpiryDate { get; set; }
        [DataMember]
        public virtual decimal Cost { get; set; }
        [DataMember]
        public virtual decimal UnitCostInUSD { get; set; }
        [DataMember]
        public virtual decimal TotalCost { get; set; }
        [DataMember]
        public virtual decimal TotalCostInUSD { get; set; }
        [DataMember]
        public virtual long POKey { get; set; }
        [DataMember]
        public virtual string SupplierCode { get; set; }
        [DataMember]
        public virtual string SupplierName { get; set; }
        [DataMember]
        public virtual string SupplierDeliveryNoteNo { get; set; }
        [DataMember]
        public virtual string InvoiceNumber { get; set; }
        [DataMember]
        public virtual string ContainerKey { get; set; }
        [DataMember]
        public virtual string BillofLoading { get; set; }
        [DataMember]
        public virtual string Notes { get; set; }
        [DataMember]
        public virtual decimal DamageQty { get; set; }
        [DataMember]
        public virtual string ConditionCode { get; set; }
        [DataMember]
        public virtual long LOT { get; set; }
        [DataMember]
        public virtual long RetailSKU { get; set; }
        [DataMember]
        public virtual decimal ASNPrice { get; set; }
        [DataMember]
        public virtual string Currency { get; set; }
        [DataMember]
        public virtual decimal PortalPrice { get; set; }
        [DataMember]
        public virtual long ShelfLifeAtReceipt { get; set; }
        [DataMember]
        public virtual long ShelfLife { get; set; }
        [DataMember]
        public virtual long ShelfLifePercentageAtReceipt { get; set; }
        [DataMember]
        public virtual DateTime? DateStart { get; set; }
        [DataMember]
        public virtual DateTime? DateEnd { get; set; }
        [DataMember]
        public virtual string ASNStart { get; set; }
        [DataMember]
        public virtual string ASNEnd { get; set; }
        [DataMember]
        public virtual string SKUStart { get; set; }
        [DataMember]
        public virtual string SKUEnd { get; set; }
        [DataMember]
        public virtual string Lottable01 { get; set; }
        [DataMember]
        public virtual string Lottable09 { get; set; }
        [DataMember]
        public virtual decimal ContainerQty { get; set; }
        [DataMember]
        public virtual decimal TotalQtyReceived { get; set; }
        [DataMember]
        public virtual string CreatedBy { get; set; }
        [DataMember]
        public virtual DateTime? CreatedDate { get; set; }
        [DataMember]
        public virtual string ModifiedBy { get; set; }
        [DataMember]
        public virtual DateTime? ModifiedDate { get; set; }
        [DataMember]
        public virtual long RequestId { get; set; }
        [DataMember]
        public virtual string IsValid { get; set; }
        [DataMember]
        public virtual long PORequestId { get; set; }
    }
}
