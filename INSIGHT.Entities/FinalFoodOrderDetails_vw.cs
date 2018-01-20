using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INSIGHT.Entities
{
    public class FinalFoodOrderDetails_vw
    {
        public virtual long Id { get; set; }
        public virtual string Period { get; set; }
        public virtual string PeriodYear { get; set; }
        public virtual decimal TroopsStrength { get; set; }
        public virtual decimal TotalAmount { get; set; }
        public virtual decimal TotalWeight { get; set; }
    }
}
