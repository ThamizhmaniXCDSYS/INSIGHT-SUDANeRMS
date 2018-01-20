using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INSIGHT.Entities.DashboardEntities
{
    class LossBecauseofExcessDelivery_SP
    {
        public virtual long Id { get; set; }
        public virtual string ControlId { get; set; }
        public virtual string Sector { get; set; }
        public virtual string Name { get; set; }
        public virtual string Location { get; set; }
        public virtual string ContingentType { get; set; }
        public virtual long Week { get; set; }
        public virtual long OrderId { get; set; }
        public virtual long LineId { get; set; }
        public virtual long UNCode { get; set; }
        public virtual string Commodity { get; set; }
        public virtual string DeliverySector { get; set; }
        public virtual decimal OrderQty { get; set; }
        public virtual string DeliveryNoteName { get; set; }
        public virtual decimal DeliveredQty { get; set; }
        public virtual decimal InvoiceQty { get; set; }
        public virtual decimal ExcessDeliveryQty { get; set; }
        public virtual decimal Sectorprice { get; set; }
        public virtual decimal AmountOfLoss { get; set; }
        public virtual string Period { get; set; }
        public virtual string PeriodYear { get; set; }
    }
}
