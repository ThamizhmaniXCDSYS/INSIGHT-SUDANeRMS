using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities.InventoryManagementEntities
{
    [DataContract]
    public class Inventory_PurchaseOrderItem
    {
        [DataMember]
        public virtual long POLineId { get; set; }
        //[DataMember]
        //public virtual long PurchOrderId { get; set; }
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
        public virtual DateTime? CreatedDate { get; set; }
        [DataMember]
        public virtual string CreatedBy { get; set; }
        [DataMember]
        public virtual DateTime? ModifiedDate { get; set; }
        [DataMember]
        public virtual string ModifiedBy { get; set; }
        [DataMember]
        public virtual long RequestId { get; set; }
        [DataMember]
        public virtual decimal InvoicedQty { get; set; }
        [DataMember]
        public virtual decimal RemainingQty { get; set; }

        public virtual Inventory_PurchaseOrder Inventory_PurchaseOrder { get; set; }
    }
}
