using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace INSIGHT.Entities.InvoiceEntities
{
    public class TransportRateMaster
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual string ContingentControlno { get; set; }
        [DataMember]
        public virtual string Nationality { get; set; }
        [DataMember]
        public virtual string DeliveryPoint { get; set; }
        [DataMember]
        public virtual string LocationCode { get; set; }
        [DataMember]
        public virtual long TroopStrength { get; set; }
        [DataMember]
        public virtual string DeliveryMode { get; set; }
        [DataMember]
        public virtual long Distance { get; set; }
        [DataMember]
        public virtual string Location { get; set; }
        [DataMember]
        public virtual decimal AirShipment { get; set; }
        [DataMember]
        public virtual decimal SurfaceShipment { get; set; }
        [DataMember]
        public virtual string Warehouse { get; set; }
        [DataMember]
        public virtual string CreatedBy { get; set; }
        [DataMember]
        public virtual DateTime CreatedDate { get; set; }
        [DataMember]
        public virtual string ModifiedBy { get; set; }
        [DataMember]
        public virtual DateTime ModifiedDate { get; set; }

    }
}
