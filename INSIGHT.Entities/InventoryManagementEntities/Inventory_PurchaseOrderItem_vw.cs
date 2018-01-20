using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace INSIGHT.Entities.InventoryManagementEntities
{
    [DataContract]
    public class Inventory_PurchaseOrderItem_vw
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual long PurchOrderId { get; set; }
        [DataMember]
        public virtual long PO { get; set; }
        [DataMember]
        public virtual long POLineId { get; set; }
        [DataMember]
        public virtual long POInvoiceId { get; set; }
        [DataMember]
        public virtual long POInvoiceItemId { get; set; }
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
        public virtual decimal InvoiceUnitPrice { get; set; }
        [DataMember]
        public virtual decimal RemainingQty { get; set; }
        [DataMember]
        public virtual string Remarks { get; set; }
        [DataMember]
        public virtual string InvoiceNumber { get; set; }
    }
}
