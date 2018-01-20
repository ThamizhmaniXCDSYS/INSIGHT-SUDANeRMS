using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INSIGHT.Entities
{
    public class ORDRPT_ConsolidatedWeekReport
    {
        public virtual long Id { get; set; }
        public virtual long UNCode { get; set; }
        public virtual string Commodity { get; set; }
        public virtual string Temperature { get; set; }
        public virtual long OrdItemsUNCode { get; set; }

        public virtual decimal OrderQty_SS1 { get; set; }
        public virtual decimal OrderQty_SN1 { get; set; }
        public virtual decimal OrderQty_SW1 { get; set; }
        public virtual decimal TotalOrdQtyWK1 { get; set; }


        public virtual decimal OrderQty_SS2 { get; set; }
        public virtual decimal OrderQty_SN2 { get; set; }
        public virtual decimal OrderQty_SW2 { get; set; }
        public virtual decimal TotalOrdQtyWK2 { get; set; }


        public virtual decimal OrderQty_SS3 { get; set; }
        public virtual decimal OrderQty_SN3 { get; set; }
        public virtual decimal OrderQty_SW3 { get; set; }
        public virtual decimal TotalOrdQtyWK3 { get; set; }


        public virtual decimal OrderQty_SS4 { get; set; }
        public virtual decimal OrderQty_SN4 { get; set; }
        public virtual decimal OrderQty_SW4 { get; set; }
        public virtual decimal TotalOrdQtyWK4 { get; set; }

        public virtual decimal TotalOrdQtySS { get; set; }
        public virtual decimal TotalOrdQtySN { get; set; }
        public virtual decimal TotalOrdQtySW { get; set; }
        public virtual decimal OrdQty { get; set; }





    }
}
