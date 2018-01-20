using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

namespace INSIGHT.Entities
{
    public  class InsightHomePageDashboard
    {
        public virtual long Id { get; set; }
        public virtual string Category { get; set; }
        public virtual string PeriodYear { get; set; }
        public virtual long NoOfInvoices { get; set; }
        public virtual decimal InvoiceValue { get; set; }
        public virtual long NoOfContingents { get; set; }
        public virtual long TroopStrength { get; set; }

    }
}
