using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INSIGHT.Entities
{
    public class ORDRPT_BulkFoodOrder_SP
    {
        

        public virtual long Id {get;set;}
        public virtual long  UNCode {get;set;}
        public virtual string Commodity {get;set;}
        public virtual  decimal OrdQty_SS {get;set;}
        public virtual decimal  OrdQty_SN{get;set;}
        public virtual decimal OrdQty_SW {get;set;}
        public virtual decimal TotalOrdQty { get; set; }
        public virtual long Troops_SN { get; set; }
        public virtual long Troops_SS { get; set; }
        public virtual long Troops_SW { get; set; }

    }
}
