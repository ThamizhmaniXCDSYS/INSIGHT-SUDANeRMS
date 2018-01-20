using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INSIGHT.Entities
{
    public class ORDRPT_AnalysisSP
    {
        public virtual long Id { get; set; }
        public virtual long UNCode{get;set;}
        public virtual string Commodity{get;set;}
        public virtual string Temperature{get;set;}
        public virtual decimal OrderQty{get;set;}
        public virtual decimal InitialOrdQty{get;set;}
        public virtual decimal Difference{get;set;}
        public virtual decimal DiffPercentage{get;set; }
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime InitialStartDate { get; set; }
        public virtual DateTime EndDate { get; set; }
        public virtual DateTime InitialEndDate { get; set; }
        public virtual long Troops { get; set; }
        public virtual long InitialTroops { get; set; }
    }

}
