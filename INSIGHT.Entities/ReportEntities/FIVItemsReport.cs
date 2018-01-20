using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace INSIGHT.Entities.ReportEntities
{
    [DataContract]
    public class FIVItemsReport
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual long OrderId { get; set; }
        [DataMember]
        public virtual string ControlId { get; set; }
        [DataMember]
        public virtual string DeliveryNoteName { get; set; }
        
        [DataMember]
        public virtual long UNCode { get; set; }
        [DataMember]
        public virtual long SubstituteItemCode { get; set; }
        [DataMember]
        public virtual decimal OrderQty { get; set; }
        [DataMember]
        public virtual decimal DeliveredQty { get; set; }
        [DataMember]
        public virtual decimal AcceptedQty { get; set; }
        [DataMember]
        public virtual decimal InvoiceQty { get; set; }
        [DataMember]
        public virtual decimal SectorPrice { get; set; }
        [DataMember]
        public virtual decimal Total { get; set; }
        [DataMember]
        public virtual string Comment { get; set; }
        [DataMember]
        public virtual string InvoiceNumber { get; set; }
        [DataMember]
        public virtual string DiscrepancyCode { get; set; }
    }
}
