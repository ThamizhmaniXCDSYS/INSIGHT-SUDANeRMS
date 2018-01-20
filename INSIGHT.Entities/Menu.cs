using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities
{
    public class Menu
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual string MenuName { get; set; }
        [DataMember]
        public virtual bool ParentORChild { get; set; }
        [DataMember]
        public virtual string MenuLevel { get; set; }
        [DataMember]
        public virtual Int32 ParentRefId { get; set; }
        [DataMember]
        public virtual string Role { get; set; }
        [DataMember]
        public virtual Int32 OrderNo { get; set; }
        [DataMember]
        public virtual string Controller { get; set; }
        [DataMember]
        public virtual string Action { get; set; }
        [DataMember]
        public virtual bool IsActive { get; set; }

    }
}
