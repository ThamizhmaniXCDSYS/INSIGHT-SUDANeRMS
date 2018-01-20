using System;
using System.Runtime.Serialization;
namespace INSIGHT.Entities
{
    [DataContract]
    public class UNSectorConMapping
    {
        [DataMember]
        public virtual Int64 Id { get; set; }
        [DataMember]
        public virtual string MappingCode { get; set; }
        [DataMember]
        public virtual string UNCode { get; set; }
        [DataMember]
        public virtual string Commodity { get; set; }
        [DataMember]
        public virtual string SectorCode { get; set; }
        [DataMember]
        public virtual decimal UnitPrice { get; set; }
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
