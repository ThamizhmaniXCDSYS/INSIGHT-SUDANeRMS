using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities
{
    [DataContract]
    public class DeliveryNoteOrders_vw
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual long DeliveryNoteId { get; set; }
        [DataMember]
        public virtual string DeliveryNoteName { get; set; }
        [DataMember]
        public virtual string ControlId { get; set; }
        [DataMember]
        public virtual string Name { get; set; }
        [DataMember]
        public virtual string Sector { get; set; }
        [DataMember]
        public virtual string Location { get; set; }
        [DataMember]
        public virtual string Period { get; set; }
        [DataMember]
        public virtual string PeriodYear { get; set; }
        [DataMember]
        public virtual byte[] DocumentData { get; set; }
        [DataMember]
        public virtual string DeliveryNoteType { get; set; }
        [DataMember]
        public virtual DateTime ActualDeliveryDate { get; set; }
    }
}
