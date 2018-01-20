using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace INSIGHT.Entities.InventoryManagementEntities
{
    [DataContract]
    public class Inventory_PurchaseOrderInvoiceItem_VW
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual long POInvoiceId { get; set; }
        [DataMember]
        public virtual string InvoiceNumber { get; set; }
        [DataMember]
        public virtual DateTime? InvoiceDate { get; set; }
        [DataMember]
        public virtual decimal InvoiceAmount { get; set; }
        [DataMember]
        public virtual string ContainerNumber { get; set; }
        [DataMember]
        public virtual string BillOfLading { get; set; }
        [DataMember]
        public virtual DateTime? GLDate { get; set; }
        [DataMember]
        public virtual long VoucherNumber { get; set; }
        [DataMember]
        public virtual decimal RemainingAmount { get; set; }
        [DataMember]
        public virtual decimal InvoiceAmountUSD { get; set; }
        [DataMember]
        public virtual decimal ExchangeRate { get; set; }
        [DataMember]
        public virtual long INVConfig_Id { get; set; }
        [DataMember]
        public virtual long POInvoiceItemId { get; set; }
        [DataMember]
        public virtual decimal InvoiceQty { get; set; }
        [DataMember]
        public virtual decimal InvoiceUnitPrice { get; set; }
        [DataMember]
        public virtual decimal InvoiceValue { get; set; }
        [DataMember]
        public virtual string Remarks { get; set; }
        [DataMember]
        public virtual bool IsDisconnect { get; set; }
        [DataMember]
        public virtual bool IsComplete { get; set; }
        [DataMember]
        public virtual string CreatedBy { get; set; }
        [DataMember]
        public virtual DateTime? CreatedDate { get; set; }
        [DataMember]
        public virtual long PurchOrderId { get; set; }
        [DataMember]
        public virtual long PO { get; set; }
        [DataMember]
        public virtual long POLineId { get; set; }
        [DataMember]
        public virtual long UNCode { get; set; }
        [DataMember]
        public virtual string Commodity { get; set; }
        [DataMember]
        public virtual decimal OrderedQty { get; set; }
        [DataMember]
        public virtual decimal POUnitPrice { get; set; }
        [DataMember]
        public virtual decimal POValue { get; set; }
        [DataMember]
        public virtual decimal InvoicedQty { get; set; }
        [DataMember]
        public virtual decimal RemainingQty { get; set; }
    }
}
