using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities.InputUploadEntities
{
    [DataContract]
    public class FIVItems_vw
    {
        [DataMember]
        public virtual long Id{get;set;}
        [DataMember]
        public virtual long OrderId{get;set;}
        [DataMember]
        public virtual string Sector{get;set;}
        [DataMember]
        public virtual string Period{get;set;}
        [DataMember]
        public virtual string PeriodYear{get;set;}
        [DataMember]
        public virtual long Week{get;set;}

    }
}
