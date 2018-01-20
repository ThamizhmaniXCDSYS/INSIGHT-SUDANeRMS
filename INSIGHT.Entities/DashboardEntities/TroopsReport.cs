using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace INSIGHT.Entities.DashboardEntities
{
    [DataContract]
    public class TroopsReport
    {
        [DataMember]
        public virtual long TroopsReportId { get; set; }
        [DataMember]
        public virtual long Week { get; set; }
        [DataMember]
        public virtual string ContingentType { get; set; }
        [DataMember]
        public virtual string Name { get; set; }
        [DataMember]
        public virtual string Location { get; set; }
        [DataMember]
        public virtual string Sector { get; set; }
        [DataMember]
        public virtual string PeriodYear { get; set; }
        [DataMember]
        public virtual string Period { get; set; }
        [DataMember]
        public virtual decimal SumofTroops { get; set; }
        [DataMember]
        public virtual long rank { get; set; }
    }
}
