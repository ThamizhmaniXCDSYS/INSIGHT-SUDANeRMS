using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace INSIGHT.Entities.DashboardEntities
{

    public class TroopsReport_LocationGraph
    {
        public virtual string Location { get; set; }
        public virtual decimal Troops1 { get; set; }
        public virtual decimal Troops2 { get; set; }
        public virtual decimal Troops3 { get; set; }
        public virtual decimal Troops4 { get; set; }
  

    }

}
