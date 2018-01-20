using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities
{
    [DataContract]
   public class AppRoleConfig
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual string AppCode { get; set; }
        [DataMember]
        public virtual string RoleCode { get; set; }
    }
}
