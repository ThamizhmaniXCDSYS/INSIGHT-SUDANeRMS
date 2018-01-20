using System;
using System.Runtime.Serialization;
namespace INSIGHT.Entities
{
    [DataContract]
    public class SectorMaster
    {
        [DataMember]
        public virtual Int64 Id { get; set; }
        [DataMember]
        public virtual string SectorCode { get; set; }
        [DataMember]
        public virtual string SectorName { get; set; }
        [DataMember]
        public virtual string Description { get; set; }
        [DataMember]
        public virtual string Location { get; set; }
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
