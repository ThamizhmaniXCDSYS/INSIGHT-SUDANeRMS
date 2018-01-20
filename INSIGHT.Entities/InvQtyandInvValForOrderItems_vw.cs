using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace INSIGHT.Entities
{

   public  class InvQtyandInvValForOrderItems_vw
    {
      [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual long OrderId { get; set; }
        [DataMember]
        public virtual long UNCode { get; set; }
        [DataMember]
        public virtual decimal OQWithOutEggs { get; set; }
        [DataMember]
        public virtual decimal OQEggsOnly { get; set; }
        [DataMember]
        public virtual decimal DQWithOutEggs { get; set; }
        [DataMember]
        public virtual decimal DQEggsOnly { get; set; }

        [DataMember]
        public virtual decimal AQWithOutEggs { get; set; }
        [DataMember]
        public virtual decimal AQEggsOnly { get; set; }
        [DataMember]
        public virtual decimal InvoiceQty { get; set; }
        [DataMember]
        public virtual decimal InvoiceValue { get; set; }
    }
}
