using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities.InvoiceEntities
{
    [DataContract]
    public class SingleInvoiceView
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual long OrderId { get; set; }
        [DataMember]
        public virtual long LineId { get; set; }
        [DataMember]
        public virtual long UNCode { get; set; }
        [DataMember]
        public virtual string Commodity { get; set; }
        [DataMember]
        public virtual decimal OrderQty { get; set; }
        [DataMember]
        public virtual decimal InvoiceQty { get; set; }
        [DataMember]
        public virtual decimal DeliveredOrdQty { get; set; }
        [DataMember]
        public virtual decimal AcceptedOrdQty { get; set; }
        [DataMember]
        public virtual decimal SectorPrice { get; set; }
        [DataMember]
        public virtual decimal NetAmt { get; set; }
        [DataMember]
        public virtual decimal OrderValue { get; set; }
        [DataMember]
        public virtual decimal ActualPackPrice { get; set; }
        [DataMember]
        public virtual string DeliveryNote { get; set; }
        [DataMember]
        public virtual string UOM { get; set; }
        [DataMember]
        public virtual decimal CalcPackPrice { get; set; }
        [DataMember]
        public virtual decimal APLWeight { get; set; }
        [DataMember]
        public virtual decimal ActualAPLWeight { get; set; }
        [DataMember]
        public virtual string DiscrepancyCode { get; set; }
        //Added by Gobi on 07/05/14 for Display 0 where discripency code = AS
        [DataMember]
        public virtual decimal DeliveredOrdQtyTemp { get; set; }
        [DataMember]
        public virtual decimal AcceptedOrdQtyTemp { get; set; }
        [DataMember]
        public virtual decimal InvoiceQtyTemp { get; set; }
        [DataMember]
        public virtual decimal NetAmtTemp { get; set; }
        [DataMember]
        public virtual decimal APLWeightTemp { get; set; }
        [DataMember]
        public virtual string NPACode { get; set; }
        
    }
}
