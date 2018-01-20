using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities.InvoiceEntities
{
    [DataContract]
    public class InvoiceOrdersView
    {

        [DataMember]
        public virtual long Id { get; set; }
            [DataMember]
            public virtual long InvoiceId { get; set; }
            [DataMember]
            public virtual long OrderId { get; set; }
            [DataMember]
            public virtual string Name { get; set; }
            [DataMember]
            public virtual string Location { get; set; }
            [DataMember]
            public virtual string ControlId { get; set; }

            [DataMember]
            public virtual DateTime StartDate { get; set; }
            [DataMember]
            public virtual DateTime EndDate { get; set; }
            [DataMember]
            public virtual decimal Troops { get; set; }
            [DataMember]
            public virtual decimal TotalAmount { get; set; }

            [DataMember]
            public virtual decimal LineItemsOrdered { get; set; }
            [DataMember]
            public virtual decimal KgOrderedWOEggs { get; set; }
            [DataMember]
            public virtual decimal EggsWeight { get; set; }
            [DataMember]
            public virtual decimal TotalWeight { get; set; }

            [DataMember]
            public virtual string CreatedBy { get; set; }
            [DataMember]
            public virtual DateTime CreatedDate { get; set; }
            [DataMember]
            public virtual decimal LocationCMR { get; set; }
            [DataMember]
            public virtual decimal ControlCMR { get; set; }

            [DataMember]
            public virtual string Period { get; set; }
            [DataMember]
            public virtual string Sector { get; set; }
            [DataMember]
            public virtual Int64 Week { get; set; }
            [DataMember]
            public virtual string PeriodYear { get; set; }
            [DataMember]
            public virtual string ModifiedBy { get; set; }
            [DataMember]
            public virtual DateTime ModifiedDate { get; set; }

    }
}
