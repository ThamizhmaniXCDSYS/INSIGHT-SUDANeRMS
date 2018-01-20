using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INSIGHT.Entities
{
   public class ORDRPT_SummaryTroops
    {
       public virtual long Id { get; set; }
       public virtual string Period {get;set;}
       public virtual string PeriodYear {get;set;}
       public virtual string FinalSector { get; set; }
       public virtual long FinalWeek { get; set; }
       public virtual long FinalTroops { get; set; }
       public virtual string Category { get; set; }
       public virtual DateTime CreatedDate { get; set; }
    }
}
