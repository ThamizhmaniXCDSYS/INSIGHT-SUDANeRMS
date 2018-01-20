using System.Runtime.Serialization;

namespace INSIGHT.Entities
{
    public class OrdersPerMonth_vw
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual int Year { get; set; }
        [DataMember]
        public virtual int Jan { get; set; }
        [DataMember]
        public virtual int Feb { get; set; }
        [DataMember]
        public virtual int Mar { get; set; }
        [DataMember]
        public virtual int Apr { get; set; }
        [DataMember]
        public virtual int May { get; set; }
        [DataMember]
        public virtual int Jun { get; set; }
        [DataMember]
        public virtual int Jul { get; set; }
        [DataMember]
        public virtual int Aug { get; set; }
        [DataMember]
        public virtual int Sep { get; set; }
        [DataMember]
        public virtual int Nov { get; set; }
        [DataMember]
        public virtual int Oct { get; set; }
        [DataMember]
        public virtual int Dec { get; set; }
    }
}
