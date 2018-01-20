using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace INSIGHT.Entities
{
    [DataContract]
    public class UNSectorDetailedItemPrice
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual long UNCode { get; set; }
        [DataMember]
        public virtual string Commodity { get; set; }
        [DataMember]
        public virtual string SectorCode { get; set; }
        [DataMember]
        public virtual decimal UnitPrice_Halal { get; set; }
        [DataMember]
        public virtual decimal UnitPrice_NonHalal { get; set; }
        [DataMember]
        public virtual decimal TransportationCost { get; set; }
        [DataMember]
        public virtual decimal InsuranceCost { get; set; }
        [DataMember]
        public virtual decimal Surfacedeliveries_Halal { get; set; }
        [DataMember]
        public virtual decimal Surfacedeliveries_NonHalal { get; set; }
        [DataMember]
        public virtual decimal AirCostfromRegional { get; set; }
        [DataMember]
        public virtual decimal AirCostfromOriginal { get; set; }
        [DataMember]
        public virtual decimal AirCostfromKhartoum { get; set; }
        [DataMember]
        public virtual string Warehouse { get; set; }
        [DataMember]
        public virtual string CreatedBy { get; set; }
        [DataMember]
        public virtual DateTime CreatedDate { get; set; }
        [DataMember]
        public virtual string ModifiedBy { get; set; }
        [DataMember]
        public virtual DateTime? ModifiedDate { get; set; }
    }
}
