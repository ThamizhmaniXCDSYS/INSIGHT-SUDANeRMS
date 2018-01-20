using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace INSIGHT.Entities
{
    [DataContract]
    public class FinalFoodOrderbyWeek
    {
        [DataMember]
        public virtual string SheetName { get; set; }
        [DataMember]
        public virtual string Title { get; set; }
        [DataMember]
        public virtual string Sector_Loc_Contingent { get; set; }
        [DataMember]
        public virtual string HQ_Location { get; set; }
        [DataMember]
        public virtual string Location { get; set; }
        [DataMember]
        public virtual string UnitName { get; set; }
        [DataMember]
        public virtual string TroopsStrength { get; set; }
        [DataMember]
        public virtual decimal PerManPerDay { get; set; }
        [DataMember]
        public IList<ORDRPT_FinalFoodOrderReport_SP> FinalFoodOrderList { get; set; }
    }
}
