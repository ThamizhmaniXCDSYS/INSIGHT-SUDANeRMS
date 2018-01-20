using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities.Masters
{
    [DataContract]
    public class Inventory_ExchangeRateMaster
    {
        [DataMember]
        public virtual long Rate_Id { get; set; }
        [DataMember]
        public virtual decimal ExchangeRate { get; set; }
        [DataMember]
        public virtual string CreatedBy { get; set; }
        [DataMember]
        public virtual DateTime? CreatedDate { get; set; }
        [DataMember]
        public virtual string ModifiedBy { get; set; }
        [DataMember]
        public virtual DateTime? ModifiedDate { get; set; }
        [DataMember]
        public virtual Inventory_CurrencyMaster Inventory_CurrencyMaster { get; set; }
        [DataMember]
        public virtual string GLDate { get; set; }
    }
}
