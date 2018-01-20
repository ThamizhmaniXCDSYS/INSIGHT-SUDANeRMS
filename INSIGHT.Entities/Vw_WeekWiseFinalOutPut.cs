using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities
{
    [DataContract]
    public class Vw_WeekWiseFinalOutPut
    {
        [DataMember]
        public virtual Int32 Id { get; set; }
        [DataMember]
        public virtual string ControlId { get; set; }
        [DataMember]
        public virtual long OrderId { get; set; }
        [DataMember]
        public virtual string Name { get; set; }
        [DataMember]
        public virtual string ContingentType { get; set; }
        [DataMember]
        public virtual string Period { get; set; }
        [DataMember]
        public virtual string Sector { get; set; }
        [DataMember]
        public virtual long Week { get; set; }

        [DataMember]
        public virtual decimal OrderQty { get; set; }
        [DataMember]
        public virtual decimal DeliveredQty { get; set; }
        [DataMember]
        public virtual decimal AcceptedQty { get; set; }
        [DataMember]
        public virtual decimal InvoiceQty { get; set; }
        [DataMember]
        public virtual decimal InvoiceValue { get; set; }
        [DataMember]
        public virtual string Location { get; set; }
        [DataMember]
        public virtual long Rank { get; set; }
        [DataMember]
        public virtual string PeriodYear { get; set; }

    }
}
