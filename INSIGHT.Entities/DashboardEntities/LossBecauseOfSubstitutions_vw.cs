using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace INSIGHT.Entities.DashboardEntities
{
    [DataContract]
    public class LossBecauseOfSubstitutions_vw
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual long OrderId       {get;set;}
        [DataMember]
        public virtual string ControlId   {get;set;}
        [DataMember]
        public virtual string  Name  {get;set;}
        [DataMember]
        public virtual string  ContingentType  {get;set;}
        [DataMember]
        public virtual string  Period  {get;set;}
        [DataMember]
        public virtual string  Sector  {get;set;}
        [DataMember]
        public virtual long   Week  {get;set;}
        [DataMember]
        public virtual string   Location {get;set;}
        [DataMember]
        public virtual string  PeriodYear {get;set;}
        [DataMember]
        public virtual long  UNCode     {get;set;}
        [DataMember]
        public virtual string ItemName { get; set; }
        [DataMember]
        public virtual long  SubstituteItemCode   {get;set;}
        [DataMember]
        public virtual string SubstituteItemName { get; set; }
        [DataMember]
        public virtual decimal   ActualItemPrice    {get;set;}
        [DataMember]
        public virtual decimal   SubstituteItemPrice    {get;set;}
        [DataMember]
        public virtual decimal      InvoiceValue {get;set;}
        [DataMember]
        public virtual string   Status    {get;set;}
        [DataMember]
        public virtual decimal   InvoiceQty    {get;set;}
        [DataMember]
        public virtual decimal    ActualItemValue   {get;set;}
        [DataMember]
        public virtual decimal   SubsItemValue    {get;set;}
        [DataMember]
        public virtual decimal Loss { get; set; }
        [DataMember]
        public virtual decimal DeliveredQty{get;set;}
    }
}

