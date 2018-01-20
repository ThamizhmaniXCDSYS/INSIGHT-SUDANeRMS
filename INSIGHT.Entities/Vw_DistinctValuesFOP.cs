using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities
{
     [DataContract]
  public   class Vw_DistinctValuesFOP
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual string Name { get; set; }
        [DataMember]
        public virtual string ContingentType { get; set; }
        [DataMember]
        public virtual string Period { get; set; }
        [DataMember]
        public virtual string Sector { get; set; }
        [DataMember]
        public virtual string Location { get; set; }



        public decimal Week1 { get; set; }
        [DataMember]
        public decimal OrderQty1 { get; set; }
        [DataMember]
        public decimal DeliveredQty1 { get; set; }
        [DataMember]
        public decimal AcceptedQty1 { get; set; }

        [DataMember]
        public decimal InvoiceQty1 { get; set; }
        [DataMember]
        public decimal InvoiceValue1 { get; set; }

        [DataMember]
        public decimal Week2 { get; set; }
        [DataMember]
        public decimal OrderQty2 { get; set; }
        [DataMember]
        public decimal DeliveredQty2 { get; set; }
        [DataMember]
        public decimal AcceptedQty2 { get; set; }
        [DataMember]
        public decimal InvoiceQty2 { get; set; }
        [DataMember]
        public decimal InvoiceValue2 { get; set; }

        [DataMember]
        public decimal Week3 { get; set; }
        [DataMember]
        public decimal OrderQty3 { get; set; }
        [DataMember]
        public decimal DeliveredQty3 { get; set; }
        [DataMember]
        public decimal AcceptedQty3 { get; set; }
        [DataMember]
        public decimal InvoiceQty3 { get; set; }
        [DataMember]
        public decimal InvoiceValue3 { get; set; }

        [DataMember]
        public decimal Week4 { get; set; }
        [DataMember]
        public decimal OrderQty4 { get; set; }
        [DataMember]
        public decimal DeliveredQty4 { get; set; }
        [DataMember]
        public decimal AcceptedQty4 { get; set; }
        [DataMember]
        public decimal InvoiceQty4 { get; set; }
        [DataMember]
        public decimal InvoiceValue4 { get; set; }
        [DataMember]
        public virtual string PeriodYear { get; set; }
    }
}
