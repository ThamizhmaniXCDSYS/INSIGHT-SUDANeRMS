using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using INSIGHT.Entities.InvoiceEntities;

namespace INSIGHT.Entities.PDFEntities
{
    public class InvoiceList
    {
        [DataMember]
        public string ImageUrl { get; set; }
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
        public string TotMadays { get; set; }
        [DataMember]
        public string TotalFeedingToop { get; set; }
        [DataMember]
        public string UnINo { get; set; }
        [DataMember]
        public string CMR { get; set; }
        [DataMember]
        public string Week1 { get; set; }
        [DataMember]
        public string Week2 { get; set; }
        [DataMember]
        public string Week3 { get; set; }
        [DataMember]
        public string Week4 { get; set; }
        [DataMember]
        public string WeekNo1 { get; set; }
        [DataMember]
        public string WeekNo2 { get; set; }
        [DataMember]
        public string WeekNo3 { get; set; }
        [DataMember]
        public string WeekNo4 { get; set; }
        [DataMember]
        public string SubTotal { get; set; }
        [DataMember]
        public string GrandTotal { get; set; }
        [DataMember]
        public string WeekNo { get; set; }
        [DataMember]
        public string Usd_words { get; set; }
        [DataMember]
        public virtual IList<DeliveriesPerOrdQty> deliveriesPerOrdQty { get; set; }
        [DataMember]
        public virtual IList<DeliveryExceed> deliveryExceed { get; set; }
        [DataMember]
        public virtual IList<PeriodWeek> PeriodWeek { get; set; }

        [DataMember]
        public string Delivery { get; set; }
        [DataMember]
        public string OrderLineItems { get; set; }
        [DataMember]
        public string OrderByWeight { get; set; }
        [DataMember]
        public string ComplainAvalability { get; set; }
        //Added by Thamizhmani
        [DataMember]
        public virtual long Week { get; set; }
        [DataMember]
        public virtual decimal WeeklyDiscount { get; set; }
        [DataMember]
        public virtual long OrderId { get; set; }
        [DataMember]
        public virtual string ControlId { get; set; }
        [DataMember]
        public virtual bool IsAPL { get; set; }
        [DataMember]
        public string TPTPO { get; set; }
        [DataMember]
        public virtual decimal TotalTransportationCost { get; set; }
    }
    public class PeriodWeek
    {
        [DataMember]
        public string AcceptedQty { get; set; }
        [DataMember]
        public string Period { get; set; }
        [DataMember]
        public string Week { get; set; }
        [DataMember]
        public string InvoiceValue { get; set; }
    }
}
