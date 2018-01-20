using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace INSIGHT.Entities.DashboardEntities
{
    public class PeriodWiseDeductionReport
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual string PeriodYear { get; set; }
        [DataMember]
        public virtual string Period { get; set; }
        [DataMember]
        public virtual string Sector { get; set; }
        [DataMember]
        public virtual string ContingentType { get; set; }
        [DataMember]
        public virtual decimal APL_TimelyDelivery { get; set; }
        [DataMember]
        public virtual decimal APL_OrderbyLineItems { get; set; }
        [DataMember]
        public virtual decimal APL_OrdersbyWeight { get; set; }
        [DataMember]
        public virtual decimal APL_NoofAuthorizedSubstitutions { get; set; }

    }
}
