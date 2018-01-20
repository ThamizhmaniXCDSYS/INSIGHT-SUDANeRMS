using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities
{
    [DataContract]
    public partial class POD
    {
        [DataMember]
        public virtual long PODId { get; set; }
        [DataMember]
        public virtual string PODNo { get; set; }
        [DataMember]
        public virtual string CreatedBy { get; set; }
        [DataMember]
        public virtual DateTime? CreatedDate { get; set; }
        [DataMember]
        public virtual DateTime? DeliveryDate { get; set; }
        [DataMember]
        public virtual long OrderId { get; set; }
        [DataMember]
        public virtual string Status { get; set; }

        //[DataMember]
        //public virtual string ModifiedBy { get; set; }
        //[DataMember]
        //public virtual DateTime ModifiedDate { get; set; }
    }
}
