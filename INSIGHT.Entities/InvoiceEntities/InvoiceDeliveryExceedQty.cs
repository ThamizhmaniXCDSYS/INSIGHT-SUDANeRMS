using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities.InvoiceEntities
{
    [DataContract]
    public class InvoiceDeliveryExceedQty
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual string Period { get; set; }
        [DataMember]
        public virtual string Sector { get; set; }
        [DataMember]
        public virtual string Week { get; set; }
        [DataMember]
        public virtual string ContingentType { get; set; }
        [DataMember]
        public virtual string Name { get; set; }
        [DataMember]
        public virtual decimal InvoiceValue { get; set; }
    }
}
