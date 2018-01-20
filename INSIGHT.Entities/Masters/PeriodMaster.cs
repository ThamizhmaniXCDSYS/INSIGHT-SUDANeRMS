using System;
using System.Runtime.Serialization;
namespace INSIGHT.Entities
{
    [DataContract]
    public class PeriodMaster
    {
        [DataMember]
        public virtual Int64 Id { get; set; }
        [DataMember]
        public virtual Int32 Week { get; set; }
        [DataMember]
        public virtual string StartDay { get; set; }
        [DataMember]
        public virtual string EndDay { get; set; }
        [DataMember]
        public virtual string Period { get; set; }
        [DataMember]
        public virtual string Year { get; set; }
        [DataMember]
        public virtual string StartDate { get; set; }
        [DataMember]
        public virtual string EndDate { get; set; }
        [DataMember]
        public virtual string CreatedBy { get; set; }
        [DataMember]
        public virtual DateTime? DateCreated { get; set; }
        [DataMember]
        public virtual string ModifiedBy { get; set; }
        [DataMember]
        public virtual DateTime DateModified { get; set; }


    }
}
