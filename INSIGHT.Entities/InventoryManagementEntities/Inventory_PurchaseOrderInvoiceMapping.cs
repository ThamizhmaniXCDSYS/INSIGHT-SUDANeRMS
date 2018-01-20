using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities.InventoryManagementEntities
{
    [DataContract]
    public class Inventory_PurchaseOrderInvoiceMapping
    {
        [DataMember]
        public virtual long INVConfig_Id { get; set; }
        [DataMember]
        public virtual long PurchOrderId { get; set; }
        [DataMember]
        public virtual long POInvoiceId { get; set; }
        [DataMember]
        public virtual string CreatedBy { get; set; }
        [DataMember]
        public virtual DateTime? CreatedDate { get; set; }
        [DataMember]
        public virtual string ModifiedBy { get; set; }
        [DataMember]
        public virtual DateTime? ModifiedDate { get; set; }
    }
}
