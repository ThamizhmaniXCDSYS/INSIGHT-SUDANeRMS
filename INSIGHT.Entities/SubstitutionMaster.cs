using System;
using System.Runtime.Serialization;
namespace INSIGHT.Entities
{
    [DataContract]
    public class SubstitutionMaster
    {
        [DataMember]
        public virtual Int64 SubstitutionMstId { get; set; }
        [DataMember]
        public virtual string Period { get; set; }
        [DataMember]
        public virtual string PeriodYear { get; set; }
        [DataMember]
        public virtual string Sector { get; set; }
        [DataMember]
        public virtual string Name { get; set; }
        [DataMember]
        public virtual string ContingentType { get; set; }
        [DataMember]
        public virtual string Location { get; set; }
        [DataMember]
        public virtual Int64 UNCode { get; set; }
        [DataMember]
        public virtual string ItemName { get; set; }

        public virtual Int64 SubstituteItemCode { get; set; }
        [DataMember]
        public virtual string SubstituteItemName  { get; set; }
        [DataMember]
        public virtual string CreatedBy  { get; set; }
        [DataMember]
        public virtual DateTime CreatedDate { get; set; }
        [DataMember]
        public virtual string ModifiedBy { get; set; }
        [DataMember]
        public virtual DateTime? ModifiedDate { get; set; }
        [DataMember]
        public virtual decimal OrderedQty { get; set; }
        [DataMember]
        public virtual decimal AcceptedQty  { get; set; }
        [DataMember]
        public virtual string  ControlId { get; set; }
        [DataMember]
        public virtual string Category { get; set; }
        [DataMember]
        public virtual Int64 OrderId { get; set; }
        [DataMember]
        public virtual string SubsOrReplace { get; set; }
        [DataMember]
        public virtual Int64 RequestId { get; set; }
    }
}