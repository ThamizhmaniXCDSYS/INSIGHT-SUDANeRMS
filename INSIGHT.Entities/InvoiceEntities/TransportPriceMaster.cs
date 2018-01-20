using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace INSIGHT.Entities.InvoiceEntities
{
    public class TransportPriceMaster
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual string Sector { get; set; }
        [DataMember]
        public virtual string SectorName { get; set; }
        [DataMember]
        public virtual long StartDistance { get; set; }
        [DataMember]
        public virtual long EndDistance { get; set; }
        [DataMember]
        public virtual decimal SurfacePrice { get; set; }
        [DataMember]
        public virtual decimal AirPrice { get; set; }
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
