using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace INSIGHT.Entities
{
    [DataContract]
    public class DeliveryNoteBasedReport_Vw
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual long OrderId { get; set; }
        [DataMember]
        public virtual string ControlId { get; set; }
        [DataMember]
        public virtual string Sector { get; set; }
        [DataMember]
        public virtual string ContingentType { get; set; }
        [DataMember]
        public virtual string Name { get; set; }
        [DataMember]
        public virtual string Location { get; set; }
        [DataMember]
        public virtual string PeriodYear { get; set; }
        [DataMember]
        public virtual string Period { get; set; }
        [DataMember]
        public virtual string DeliveryNoteName { get; set; }
        [DataMember]
        public virtual DateTime? ExpectedDeliveryDate { get; set; }
        [DataMember]
        public virtual DateTime? ReceivedDate { get; set; }
        [DataMember]
        public virtual Int64 DateDiffer { get; set; }
        [DataMember]
        public virtual string DNType { get; set; }

        [DataMember]
        public virtual string APL_Application { get; set; }
        [DataMember]
        public virtual string Remark { get; set; }

        [DataMember]
        public virtual decimal TotalWeight { get; set; }
        [DataMember]
        public virtual decimal LineItemOrdered { get; set; }
        [DataMember]
        public virtual decimal SumOfAccRecQty { get; set; }
        [DataMember]
        public virtual decimal LineItemsDelivered { get; set; }
        [DataMember]
        public virtual decimal PercentByLineItemsDelevered { get; set; }
        [DataMember]
        public virtual decimal PercentByDeliveredQty { get; set; }
        [DataMember]
        public virtual Int64 SubstitutionCount { get; set; }
        [DataMember]
        public virtual decimal GrossAmount { get; set; }
        [DataMember]
        public virtual decimal APL_TimelyDelivery { get; set; }
        [DataMember]
        public virtual decimal APL_OrderbyLineItems { get; set; }
        [DataMember]
        public virtual decimal APL_OrdersbyWeight { get; set; }
        [DataMember]
        public virtual decimal APL_NoofAuthorizedSubstitutions { get; set; }
        [DataMember]
        public virtual decimal TOTALAPL { get; set; }

        public virtual string TmpGross { get; set; }
        public virtual string TmpAPL_TimelyDelivery { get; set; }
        public virtual string TmpAPL_OrderbyLineItems { get; set; }
        public virtual string TmpAPL_OrdersbyWeight { get; set; }
        public virtual string TmpAPL_NoofAuthorizedSubstitutions { get; set; }
        public virtual string TmpTotalAPL { get; set; }


        //public virtual string ReportType { get; set; }
    }
}
