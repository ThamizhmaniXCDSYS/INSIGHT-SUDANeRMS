using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities.InventoryManagementEntities
{
    [DataContract]
    public class PORequestMaster
    {
        [DataMember]
        public virtual long POId { get; set; }
        [DataMember]
        public virtual long PONumber { get; set; }
        [DataMember]
        public virtual DateTime? POIssueddate { get; set; }
        [DataMember]
        public virtual string POStatus { get; set; }
        [DataMember]
        public virtual string InvoiceCurrency { get; set; }
        
        [DataMember]
        public virtual string Supplier { get; set; }
        [DataMember]
        public virtual string SupplierNo { get; set; }

        

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
