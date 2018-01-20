using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace INSIGHT.Entities.DashboardEntities
{
    public class AuthorizedUnAuthorizedSubstitutionList_VW
    {
        public virtual long Id { get; set; }
        public virtual string ControlId { get; set; }
        public virtual long OrderId { get; set; }
        public virtual string Sector { get; set; }
        public virtual string ContingentType { get; set; }
        public virtual string Name { get; set; }
        public virtual string Period { get; set; }
        public virtual string PeriodYear { get; set; }
        public virtual string Location { get; set; }
        public virtual long Week { get; set; }
        public virtual long UNCode { get; set; }
        public virtual string ItemName { get; set; }
        public virtual long SubstituteItemCode { get; set; }
        public virtual string SubstituteItemName { get; set; }
        public virtual string Status { get; set; }
        public virtual decimal DeliveredQty { get; set; }
    }
}
