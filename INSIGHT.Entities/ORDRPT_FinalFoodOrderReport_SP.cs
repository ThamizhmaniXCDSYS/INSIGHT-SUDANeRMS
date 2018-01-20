using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INSIGHT.Entities
{
    public class ORDRPT_FinalFoodOrderReport_SP
    {
        public virtual long Id { get; set; }
        public virtual long OrderId { get; set; }
        public virtual string ControlId { get; set; }
        public virtual string Sector { get; set; }
        public virtual string Location { get; set; }
        public virtual string Name { get; set; }
        public virtual string SectorLocContingent { get; set; }
        public virtual long Week { get; set; }
        public virtual string Period { get; set; }
        public virtual string PeriodYear { get; set; }
        public virtual long Troops { get; set; }
        public virtual string Warehouse { get; set; }
        public virtual long UNCode { get; set; }
        public virtual string Commodity { get; set; }
        public virtual decimal OrderQty { get; set; }
        public virtual long UNCode1 { get; set; }
        public virtual string Commodity1 { get; set; }
        public virtual string Temperature { get; set; }

        public virtual decimal sector1 { get; set; }
        public virtual decimal sector2 { get; set; }
        public virtual decimal sector3 { get; set; }

        public virtual decimal TotalOrdQty { get; set; }

        public virtual decimal QtyWithEggs { get; set; }
        public virtual long TotalTroops { get; set; }
        public virtual long SSOrders { get; set; }
        public virtual long SNOrders { get; set; }
        public virtual long SWOrders { get; set; }
        public virtual long SSTroops { get; set; }
        public virtual long SNTroops { get; set; }
        public virtual long SWTroops { get; set; }
    }
}
