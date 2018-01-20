using System;
using System.Runtime.Serialization;
namespace INSIGHT.Entities
{
    [DataContract]
    public class LocationMaster
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual string LocationCode { get; set; }
        [DataMember]
        public virtual string LocationName { get; set; }
        [DataMember]
        public virtual string CountryCode { get; set; }
        [DataMember]
        public virtual string CreatedBy { get; set; }
        [DataMember]
        public virtual DateTime DateCreated { get; set; }
        [DataMember]
        public virtual string ModifiedBy { get; set; }
        [DataMember]
        public virtual DateTime DateModified { get; set; }

    }
}
