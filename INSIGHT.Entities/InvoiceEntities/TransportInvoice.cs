using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace INSIGHT.Entities.InvoiceEntities
{
    public class TransportInvoice
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual string ControlId { get; set; }
        [DataMember]
        public virtual string DeliverySector { get; set; }
        [DataMember]
        public virtual string ContingentLocation { get; set; }
        [DataMember]
        public virtual string DeliveryMode { get; set; }
        [DataMember]
        public virtual decimal Distance { get; set; }
        [DataMember]
        public virtual decimal RatePerKg { get; set; }
        [DataMember]
        public virtual decimal InvoiceQty { get; set; }
        [DataMember]
        public virtual decimal TransportCost { get; set; }
        [DataMember]
        public virtual string Location { get; set; }
        [DataMember]
        public virtual string Sector { get; set; }
        [DataMember]
        public virtual string ContingentType { get; set; }
        [DataMember]
        public virtual string Period { get; set; }
        [DataMember]
        public virtual string PeriodYear { get; set; }
        [DataMember]
        public virtual long OrderId { get; set; }
        [DataMember]
        public virtual long Week { get; set; }
    }
}
