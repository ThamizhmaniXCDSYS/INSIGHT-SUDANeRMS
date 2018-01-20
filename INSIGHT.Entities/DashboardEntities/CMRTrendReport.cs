using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INSIGHT.Entities.DashboardEntities
{
   public  class CMRTrendReport
    {
      
        public virtual long Id { get; set; }
 
        public virtual long Week { get; set; }
     
        public virtual string ContingentType { get; set; }
     
        public virtual string Name { get; set; }
   
        public virtual string Location { get; set; }
    
        public virtual string Sector { get; set; }
 
        public virtual string PeriodYear { get; set; }

        public virtual string Period { get; set; }
        public virtual decimal AuthorizedCMR { get; set; }
        public virtual decimal OrderCMR { get; set; }
        public virtual decimal AcceptedCMR { get; set; }
     
    }
}
