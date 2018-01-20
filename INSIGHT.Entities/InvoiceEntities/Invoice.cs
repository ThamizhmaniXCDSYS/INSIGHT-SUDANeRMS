using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace INSIGHT.Entities.InvoiceEntities
{
    [DataContract]
    public class Invoice
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual string InvoiceCode { get; set; }
        [DataMember]
        public virtual long OrderId { get; set; }
        [DataMember]
        public virtual string Contract { get; set; }
        [DataMember]
        public virtual DateTime InvoiceDate { get; set; }
        [DataMember]
        public virtual string PaymentTerms { get; set; }
        [DataMember]
        public virtual string PO { get; set; }
        [DataMember]
        public virtual string Period { get; set; }
        [DataMember]
        public virtual string UNINo { get; set; }
        [DataMember]
        public virtual string Sector { get; set; }
        [DataMember]
        public virtual long TotalFeedTroopStrength { get; set; }
        [DataMember]
        public virtual decimal TotalMadays { get; set; }
        [DataMember]
        public virtual decimal CMR { get; set; }
        [DataMember]
        public virtual decimal SubTotal { get; set; }
        [DataMember]
        public virtual decimal GrandTotal { get; set; }
        [DataMember]
        public virtual DateTime? CreatedDate { get; set; }
        [DataMember]
        public virtual string CreatedBy { get; set; }
        [DataMember]
        public virtual string ModifiedBy { get; set; }
        [DataMember]
        public virtual DateTime? ModifiedDate { get; set; }
        [DataMember]
        public virtual Int64 Week { get; set; }
        [DataMember]
        public virtual string PeriodYear { get; set; }
        [DataMember]
        public virtual bool IsActive { get; set; }

    }

    public class ActiveInvoices : Invoice
    {
        [DataMember]
        public virtual long ActiveId { get; set; }
        [DataMember]
        public virtual long RowCount { get; set; }
    }
}
