using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace INSIGHT.Entities.DashboardEntities
{
   public  class PeriodWiseQtyReport_SP
    {

        [DataMember]
        public virtual long Id { get; set; }
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
      
        public virtual string Period { get; set; }
        public virtual decimal OrderQty { get; set; }
        public virtual decimal DeliveredQty { get; set; }
        public virtual decimal InvoiceQty { get; set; }
        public virtual decimal InvoiceValue { get; set; }
       
       
    }
}
