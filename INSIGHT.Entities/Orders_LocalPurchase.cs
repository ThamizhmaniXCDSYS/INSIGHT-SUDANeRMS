using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace INSIGHT.Entities
{
   public  class Orders_LocalPurchase
    {
        [DataMember]
        public virtual Int64 LPOrdersId { get; set; }
        [DataMember]
        public virtual string ControlId { get; set; }
        [DataMember]
        public virtual DateTime StartDate { get; set; }
        [DataMember]
        public virtual DateTime EndDate { get; set; }
        [DataMember]
        public virtual Int64 Troops { get; set; }
        [DataMember]
        public virtual decimal TotalAmount { get; set; }
        [DataMember]
        public virtual decimal LineItemsOrdered { get; set; }
        [DataMember]
        public virtual decimal KgOrderedWOEggs { get; set; }
        [DataMember]
        public virtual decimal EggsWeight { get; set; }
        [DataMember]
        public virtual decimal OrderCMR { get; set; }
        [DataMember]
        public virtual decimal TotalWeight { get; set; }
        [DataMember]
        public virtual Int64 UNCode { get; set; }
        [DataMember]
        public virtual string Commodity { get; set; }
        [DataMember]
        public virtual decimal OrderQty { get; set; }
        [DataMember]
        public virtual decimal SectorPrice { get; set; }
        [DataMember]
        public virtual decimal Total { get; set; }
        [DataMember]
        public virtual Int64 OrderId { get; set; }
        [DataMember]
        public virtual string CreatedBy { get; set; }
        [DataMember]
        public virtual DateTime CreatedDate { get; set; }
        [DataMember]
        public virtual string Sector { get; set; }
        [DataMember]
        public virtual string Location { get; set; }
        [DataMember]
        public virtual string Name { get; set; }
        [DataMember]
        public virtual string Period { get; set; }
        [DataMember]
        public virtual string PeriodYear { get; set; }
        [DataMember]
        public virtual Int64 RequestId { get; set; }
        [DataMember]
        public virtual string ValidStatus { get; set; }
       
       
       
    }
}
