using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace INSIGHT.Entities.Masters
{
    [DataContract]
    public class Inventory_CurrencyMaster
    {
        [DataMember]
        public virtual long Currency_Id { get; set; }
        [DataMember]
        public virtual string Currency { get; set; }
        [DataMember]
        public virtual string Description { get; set; }
        [DataMember]
        public virtual DateTime? CreatedDate { get; set; }
        [DataMember]
        public virtual string CreatedBy { get; set; }
        [DataMember]
        public virtual DateTime? ModifiedDate { get; set; }
        [DataMember]
        public virtual string ModifiedBy { get; set; }
    }
}
