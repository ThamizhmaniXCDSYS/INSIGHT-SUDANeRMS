using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities.InputUploadEntities
{
    [DataContract]
    public class FIVItems
    {
        [DataMember]
        public virtual long FIVItems_Id { get; set; }
        [DataMember]
        public virtual long OrderId { get; set; }
        [DataMember]
        public virtual string ControlId { get; set; }
        [DataMember]
        public virtual long UNCode { get; set; }
        [DataMember]
        public virtual long SubstituteItemCode { get; set; }
        [DataMember]
        public virtual decimal OrderedQuantity { get; set; }
        [DataMember]
        public virtual decimal DeliveredQuantity { get; set; }
        [DataMember]
        public virtual decimal AcceptedQuantity { get; set; }
        [DataMember]
        public virtual decimal UnitPrice { get; set; }
        [DataMember]
        public virtual decimal TotalPrice { get; set; }

        [DataMember]
        public virtual string IsValid { get; set; }
        [DataMember]
        public virtual long RequestId { get; set; }
        [DataMember]
        public virtual string FIVStatus { get; set; }

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
