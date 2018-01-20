using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace INSIGHT.Entities
{
    [DataContract]
    public class InvoiceQtyDelSector_PODItems
    {

        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual long UNCode { get; set; }
        [DataMember]
        public virtual long OrderId { get; set; }
        [DataMember]
        public virtual long LineId { get; set; }
        [DataMember]
        public virtual long PODItemsId { get; set; }
        [DataMember]
        public virtual decimal InvoiceQty { get; set; }
        [DataMember]
        public virtual string DeliverySector { get; set; }

    }
}
