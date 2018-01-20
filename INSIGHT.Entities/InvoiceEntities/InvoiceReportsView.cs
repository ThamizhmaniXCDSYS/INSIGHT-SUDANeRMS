using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace INSIGHT.Entities.InvoiceEntities
{
    [DataContract]
    public class InvoiceReportsView
    {

        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual long OrderId { get; set; }
        [DataMember]
        public virtual string ControlId { get; set; }
        [DataMember]
        public virtual string Location { get; set; }
        [DataMember]
        public virtual long Strength { get; set; }
        [DataMember]
        public virtual long Noofdays { get; set; }
        [DataMember]
        public virtual long Week { get; set; }
        [DataMember]
        public virtual string Period { get; set; }
        [DataMember]
        public virtual string PeriodYear { get; set; }
        [DataMember]
        public virtual string Sector { get; set; }
        [DataMember]
        public virtual string Contingent { get; set; }
        [DataMember]
        public virtual string ContingentType { get; set; }
        [DataMember]
        public virtual long Lineitemordered { get; set; }
        [DataMember]
        public virtual long Totallineitemsubstituted { get; set; }
        [DataMember]
        public virtual decimal Orderedqty { get; set; }
        [DataMember]
        public virtual decimal Deliveredqty { get; set; }
        [DataMember]
        public virtual decimal Acceptedqty { get; set; }
        [DataMember]
        public virtual decimal Amountordered { get; set; }
        [DataMember]
        public virtual decimal Amountaccepted { get; set; }
        [DataMember]
        public virtual decimal Troopstrengthdiscount { get; set; }
        [DataMember]
        public virtual decimal Othercreditnotes { get; set; }
        [DataMember]
        public virtual decimal Weeklyinvoicediscount { get; set; }
        [DataMember]
        public virtual decimal Netamountforrations { get; set; }
        [DataMember]
        public virtual decimal AplTimelydelivery { get; set; }
        [DataMember]
        public virtual decimal AplOrderbylineitems { get; set; }
        [DataMember]
        public virtual decimal AplOrdersbyweight { get; set; }
        [DataMember]
        public virtual decimal AplNoofauthorizedsubstitutions { get; set; }
        [DataMember]
        public virtual decimal Totalinvoiceamount { get; set; }
        [DataMember]
        public virtual string Modeoftransportation { get; set; }
        [DataMember]
        public virtual long Distancekm { get; set; }
        [DataMember]
        public virtual decimal Transportationperkgcost { get; set; }
        [DataMember]
        public virtual decimal Totaltransportationcost { get; set; }
        [DataMember]
        public virtual string Dn { get; set; }
        [DataMember]
        public virtual DateTime Approveddeliverydate { get; set; }
        [DataMember]
        public virtual DateTime Actualdateofreceipt { get; set; }
        [DataMember]
        public virtual long Daysdelay { get; set; }
        [DataMember]
        public virtual decimal Authorizedcmr { get; set; }
        [DataMember]
        public virtual decimal Ordercmr { get; set; }
        [DataMember]
        public virtual decimal Acceptedcmr { get; set; }
        [DataMember]
        public virtual decimal Cmrutilized { get; set; }
        [DataMember]
        public virtual long Lineitemdelivered98 { get; set; }
        [DataMember]
        public virtual decimal Confirmitytolineitemorder98 { get; set; }
        [DataMember]
        public virtual decimal Conformitytoorderbyweight { get; set; }
        [DataMember]
        public virtual decimal Noofsubtitution { get; set; }
        [DataMember]
        public virtual decimal Amountsubstituted { get; set; }
        [DataMember]
        public virtual decimal Daysdelayperformance { get; set; }
        [DataMember]
        public virtual string DeliveryNotes { get; set; }
        [DataMember]
        public virtual DateTime CreatedDate { get; set; }
        [DataMember]
        public virtual string CreatedBy { get; set; }
        [DataMember]
        public virtual string ModifiedBy { get; set; }
        [DataMember]
        public virtual DateTime? ModifiedDate { get; set; }
        [DataMember]
        public virtual string ReportType { get; set; }
        [DataMember]
        public virtual bool IsActive { get; set; }
    }
}
