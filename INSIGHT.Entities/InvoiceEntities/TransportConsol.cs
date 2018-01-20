using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace INSIGHT.Entities.InvoiceEntities
{
    public class TransportConsol
    {
        [DataMember]
        public string InvoiceNo { get; set; }
        [DataMember]
        public string Contract { get; set; }
        [DataMember]
        public string InvoiceDate { get; set; }
        [DataMember]
        public string PayTerms { get; set; }
        [DataMember]
        public string PO { get; set; }
        [DataMember]
        public string Period { get; set; }
        [DataMember]
        public string Sector { get; set; }
        [DataMember]
        public string UnINo { get; set; }
        [DataMember]
        public string SubTotal { get; set; }
        [DataMember]
        public string GrandTotal { get; set; }
        [DataMember]
        public IList<TransportFeeList> TransportFeeList { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Usd_words { get; set; }
        //Added by Thmizh
        [DataMember]
        public virtual decimal WeeklyDiscount { get; set; }
        [DataMember]
        public virtual long Week { get; set; }

    }
    public class TransportFeeList
    {
        [DataMember]
        public string AcceptedQty { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public Decimal TotalAmt { get; set; }
    }
}
