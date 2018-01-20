using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INSIGHT.Entities
{
    public class ORDRPT_Summary
    {
        public virtual long Id {get;set;}
        public virtual string Temperature {get;set;}
        public virtual decimal Total {get;set;}
        public virtual decimal WeekTotal {get;set;}
        public virtual decimal DayTotal { get; set; }
       


    }
}
