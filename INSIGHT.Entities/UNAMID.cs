using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities
{
    [DataContract]
   public class UNAMID
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual long UNCode { get; set; }
        [DataMember]
        public virtual string Commodity { get; set; }
        [DataMember]
        public virtual long OrderQty { get; set; }
        [DataMember]
        public virtual long SectorPrice { get; set; }
        [DataMember]
        public virtual long Total { get; set; }
    }
}
