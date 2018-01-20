using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INSIGHT.Entities
{
   public  class ORDRPT_ConsolidateWeekReport_vw
    {
       public virtual long Id { get; set; }
       public virtual string Sector { get; set; }
       public virtual long Week { get; set; }
       public virtual string Period { get; set; }
       public virtual string PeriodYear { get; set; }
       public virtual string Warehouse { get; set; }
       public virtual long TroopStrength { get; set; }
       public virtual long Contingents { get; set; }
       public virtual DateTime StartDate { get; set; }
       public virtual DateTime EndDate { get; set; }
    }
}
