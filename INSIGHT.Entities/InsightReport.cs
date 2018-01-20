using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace INSIGHT.Entities
{
    public class InsightReport
    {
          
        [DataMember]
        public virtual long ReportId { get; set; }
           [DataMember]
        public virtual string Description { get; set; }
           [DataMember]
           public virtual string ReportCode { get; set; }




           [DataMember]
           public virtual string FileNames { get; set; }
           [DataMember]
           public virtual long OrderId { get; set; }
           [DataMember]
           public virtual string ControlId { get; set; }


           [DataMember]
           public virtual long UNCode { get; set; }
           [DataMember]
           public virtual string Commodity { get; set; }
           [DataMember]
           public virtual decimal DeliveredQty { get; set; }

           [DataMember]
           public virtual long SubsCode { get; set; }
           [DataMember]
           public virtual string SubsName { get; set; }
           [DataMember]
           public virtual long ReplacementCode { get; set; }

           [DataMember]
           public virtual string ReplacementName { get; set; }
           [DataMember]
           public virtual DateTime CreatedDate { get; set; }
           [DataMember]
           public virtual string CreatedBy { get; set; }
           [DataMember]
           public virtual string DeliveryNoteName { get; set; }

    }
}
