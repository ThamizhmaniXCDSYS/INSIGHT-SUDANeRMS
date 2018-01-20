using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities
{
    [DataContract]
    public class FinalFoodOrderdetails_SP
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual long UNCode { get; set; }
        [DataMember]
        public virtual string Commodity { get; set; }
        [DataMember]
        public virtual string ControlId { get; set; }
        [DataMember]
        public virtual string Warehouse { get; set; }
        [DataMember]
        public virtual string Sector { get; set; }
        [DataMember]
        public virtual string Location{get;set;}
        [DataMember]
        public virtual string Loc_Contingent{get;set;}
        [DataMember]
        public virtual string Name{get;set;}
        [DataMember]
        public virtual string Period{get;set;}
        [DataMember]
        public virtual string Week{get;set;}
        [DataMember]
        public virtual string PeriodYear{get;set;}
        [DataMember]
        public virtual decimal LocationCMR{get;set;}
        [DataMember]
        public virtual decimal ControlCMR{get;set;}
        //[DataMember]
        //public virtual decimal OrderQty { get; set; }
        [DataMember]
        public virtual double OrderQty { get; set; }
        [DataMember]
        public virtual decimal SectorPrice{get;set;}
        [DataMember]
        public virtual decimal Total{get;set;}
        [DataMember]
        public virtual decimal Troops{get;set;}
        [DataMember]
        public virtual DateTime ? StartDate {get;set;}
        [DataMember]
        public virtual DateTime ? EndDate{get;set;}
        [DataMember]
        public virtual long Line_No { get; set; }
        [DataMember]
        public virtual long FFOId { get; set; }
        [DataMember]
        public virtual string DP { get; set; }
    }
}
