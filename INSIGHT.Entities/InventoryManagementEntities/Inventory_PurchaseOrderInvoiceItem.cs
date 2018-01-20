using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities.InventoryManagementEntities
{
    [DataContract]
    public class Inventory_PurchaseOrderInvoiceItem
    {
        [DataMember]
        public virtual long POInvoiceItemId { get; set; }
        [DataMember]
        public virtual long POInvoiceId { get; set; }
        [DataMember]
        public virtual long PurchOrderId { get; set; }
        [DataMember]
        public virtual long POLineId { get; set; }
        [DataMember]
        public virtual decimal OrderedQty { get; set; }
        [DataMember]
        public virtual decimal InvoicedQty { get; set; }
        [DataMember]
        public virtual decimal InvoiceUnitPrice { get; set; }
        [DataMember]
        public virtual decimal InvoiceValue { get; set; }
        [DataMember]
        public virtual string Remarks { get; set; }
        [DataMember]
        public virtual DateTime? CreatedDate { get; set; }
        [DataMember]
        public virtual string CreatedBy { get; set; }
        [DataMember]
        public virtual DateTime? ModifiedDate { get; set; }
        [DataMember]
        public virtual string ModifiedBy { get; set; }
        [DataMember]
        public virtual bool IsDisconnect { get; set; }
        [DataMember]
        public virtual long INVConfig_Id { get; set; }
    }
}
