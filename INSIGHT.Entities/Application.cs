using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace INSIGHT.Entities
{
    [DataContract]
    public class Application
    {
        [DataMember]
        public virtual Int32 Id { get; set; }
        [DataMember]
        public virtual string AppCode { get; set; }
        [DataMember]
        public virtual string AppName { get; set; }

    }
}
