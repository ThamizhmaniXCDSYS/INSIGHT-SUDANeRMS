using System;
using System.Text;
using System.Collections.Generic;


namespace INSIGHT.Entities {
    
    public class OrderItemsCountAndMismatchCount_vw {
        public virtual long? Id { get; set; }
        public virtual string Sector { get; set; }
        public virtual string Contingent { get; set; }
        public virtual string Period { get; set; }
        public virtual double? OrderId { get; set; }
        public virtual decimal? LineItemsOrdered { get; set; }
        public virtual int? ActualItems { get; set; }
    }
}
