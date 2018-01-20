using System;
using System.Runtime.Serialization;
namespace INSIGHT.Entities
{
    [DataContract]
    public class ContingentMaster
    {
        [DataMember]
        public virtual Int64 Id { get; set; }
        [DataMember]
        public virtual string ContingentControlNo { get; set; }
        [DataMember]
        public virtual string Nationality { get; set; }
        [DataMember]
        public virtual string TypeofUnit { get; set; }
        [DataMember]
        public virtual string DeliveryPoint { get; set; }
        [DataMember]
        public virtual string LocationCode { get; set; }
        [DataMember]
        public virtual int TroopStrength { get; set; }
        [DataMember]
        public virtual string DeliveryMode { get; set; }
        [DataMember]
        public virtual Int64 Distance { get; set; }
        [DataMember]
        public virtual string SectorCode { get; set; }
        [DataMember]
        public virtual string SectorName { get; set; }
        [DataMember]
        public virtual string CreatedBy { get; set; }
        [DataMember]
        public virtual DateTime? DateCreated { get; set; }
        [DataMember]
        public virtual string ModifiedBy { get; set; }
        [DataMember]
        public virtual DateTime DateModified { get; set; }
        [DataMember]
        public virtual string DeliveryModeDescription { get; set; }
        [DataMember]
        public virtual long ContingentID { get; set; }

    }
}
