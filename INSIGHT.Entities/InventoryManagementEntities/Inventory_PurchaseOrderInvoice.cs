using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities.InventoryManagementEntities
{
    [DataContract]
    public class Inventory_PurchaseOrderInvoice
    {
        [DataMember]
        public virtual long POInvoiceId { get; set; }
        //[DataMember]
        //public virtual long PurchOrderId { get; set; }
        [DataMember]
        public virtual string InvoiceNumber { get; set; }
        [DataMember]
        public virtual DateTime? InvoiceDate { get; set; }
        [DataMember]
        public virtual string ContainerNumber { get; set; }
        [DataMember]
        public virtual string BillOfLading { get; set; }
        [DataMember]
        public virtual DateTime? CreatedDate { get; set; }
        [DataMember]
        public virtual string CreatedBy { get; set; }
        [DataMember]
        public virtual DateTime? ModifiedDate { get; set; }
        [DataMember]
        public virtual string ModifiedBy { get; set; }
        [DataMember]
        public virtual decimal InvoiceAmount { get; set; }
        [DataMember]
        public virtual DateTime? GLDate { get; set; }
        [DataMember]
        public virtual long VoucherNumber { get; set; }
        [DataMember]
        public virtual decimal RemainingAmount { get; set; }
        [DataMember]
        public virtual bool IsComplete { get; set; }
        [DataMember]
        public virtual decimal ExchangeRate { get; set; }
        [DataMember]
        public virtual decimal InvoiceAmountUSD { get; set; }
        [DataMember]
        public virtual IList<Inventory_PurchaseOrderInvoiceMapping> Inventory_PurchaseOrderInvoiceMappingList { get; set; }
        [DataMember]
        public virtual string InvCurrency { get; set; }
    }
}
