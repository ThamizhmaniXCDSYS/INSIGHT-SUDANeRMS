using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace INSIGHT.Entities
{
    public class POTypeMaster
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual string POType { get; set; }
    }
}
