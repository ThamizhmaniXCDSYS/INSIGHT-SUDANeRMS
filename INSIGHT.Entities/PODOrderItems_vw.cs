using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities
{
    [DataContract]
   public  class PODOrderItems_vw
    {

        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual long OrderId { get; set; }
        [DataMember]
        public virtual string Name { get; set; }


        [DataMember]
        public virtual long LineId { get; set; }
        [DataMember]
        public virtual long UNCode { get; set; }
        [DataMember]
        public virtual string Commodity { get; set; }
        [DataMember]
        public virtual decimal OrderQty { get; set; }


        [DataMember]
        public virtual decimal SectorPrice { get; set; }
        [DataMember]
        public virtual decimal Total { get; set; }
        
        [DataMember]
        public virtual decimal AcceptedOrdQty { get; set; }
        [DataMember]
        public virtual decimal DeliveredOrdQty { get; set; }
        [DataMember]
        public virtual decimal RemainingOrdQty { get; set; }

        [DataMember]
        public virtual string Period { get; set; }
        [DataMember]
        public virtual string Sector { get; set; }
        [DataMember]
        public virtual Int64 Week { get; set; }
        [DataMember]
        public virtual string PeriodYear { get; set; }
        [DataMember]
        public virtual string ControlId { get; set; }
        [DataMember]
        public virtual string Status { get; set; }
        
    }
}
