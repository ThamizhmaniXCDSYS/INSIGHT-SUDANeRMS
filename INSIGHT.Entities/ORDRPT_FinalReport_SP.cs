using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INSIGHT.Entities
{
    public class ORDRPT_FinalReport_SP
    {
        public virtual long Id { get; set; }
        public virtual long UNCode { get; set; }
        public virtual string Commodity { get; set; }
        public virtual string Temperature { get; set; }
        public virtual decimal OrdQty_WK1 { get; set; }
        public virtual decimal OrdQty_WK2 { get; set; }
        public virtual decimal OrdQty_WK3 { get; set; }
        public virtual decimal OrdQty_WK4 { get; set; }
        public virtual decimal WeekTotal { get; set; }
        public virtual decimal DryTotal { get; set; }
        public virtual decimal ChillTotal { get; set; }
        public virtual decimal FrozenTotal { get; set; }
        public virtual decimal DryWKTotal { get; set; }
        public virtual decimal ChillWKTotal { get; set; }
        public virtual decimal FrozenWKTotal { get; set; }
        public virtual decimal DryDAYTotal { get; set; }
        public virtual decimal ChillDAYTotal { get; set; }
        public virtual decimal FrozenDAYTotal { get; set; }
    }
}
