using System;
using System.Runtime.Serialization;
namespace INSIGHT.Entities
{
    [DataContract]
    public class CountryMaster
    {
        [DataMember]
        public virtual Int64 Id { get; set; }
        [DataMember]
        public virtual string CountryCode { get; set; }
        [DataMember]
        public virtual string CountryName { get; set; }
        [DataMember]
        public virtual string SectorCode { get; set; }
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
