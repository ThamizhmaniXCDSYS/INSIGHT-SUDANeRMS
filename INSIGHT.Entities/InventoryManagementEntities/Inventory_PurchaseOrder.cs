using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities.InventoryManagementEntities
{
    [DataContract]
    public class Inventory_PurchaseOrder
    {
        [DataMember]
        public virtual long PurchOrderId { get; set; }
        [DataMember]
        public virtual long PO { get; set; }
        [DataMember]
        public virtual DateTime? POIssuedDate { get; set; }
        [DataMember]
        public virtual string POCurrency { get; set; }
        [DataMember]
        public virtual string Supplier { get; set; }
        [DataMember]
        public virtual string SupplierNumber { get; set; }
        [DataMember]
        public virtual decimal InvoiceAmount { get; set; }
        [DataMember]
        public virtual DateTime? CreatedDate { get; set; }
        [DataMember]
        public virtual string CreatedBy { get; set; }
        [DataMember]
        public virtual DateTime? ModifiedDate { get; set; }
        [DataMember]
        public virtual string ModifiedBy { get; set; }
        [DataMember]
        public virtual long RequestId { get; set; }

        
    }
}
