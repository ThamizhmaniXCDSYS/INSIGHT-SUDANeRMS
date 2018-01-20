using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities
{
    [DataContract]
    public class GCCCounter
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual string TableName { get; set; }
        [DataMember]
        public virtual string ProcessName { get; set; }
        [DataMember]
        public virtual long CounterValue { get; set; }
        [DataMember]
        public virtual long IncrementBy { get; set; }
    }
}
