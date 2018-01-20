using System;
using System.Text;
using System.Collections.Generic;


namespace INSIGHT.Entities {
    
    public class PODItemsCount_vw {
        public virtual long? Id { get; set; }
        public virtual string Sector { get; set; }
        public virtual string Contingent { get; set; }
        public virtual string Period { get; set; }
        public virtual double? PODId { get; set; }
        public virtual int? ItemsCount { get; set; }
    }
}
