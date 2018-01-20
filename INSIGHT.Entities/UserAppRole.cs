using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace INSIGHT.Entities
{
    [DataContract]
    public class UserAppRole
    {
        [DataMember]
        public virtual Int32 Id { get; set; }
        [DataMember]
        public virtual string UserId { get; set; }
        [DataMember]
        public virtual string AppCode { get; set; }
        [DataMember]
        public virtual string RoleCode { get; set; }
        [DataMember]
        public virtual string SectorCode { get; set; }
        [DataMember]
        public virtual string ContingentCode { get; set; }
        [DataMember]
        public virtual string Location { get; set; }
        [DataMember]
        public virtual string AppName { get; set; }
        [DataMember]
        public virtual string RoleName { get; set; }
        [DataMember]
        public virtual string Email { get; set; }
        [DataMember]
        public virtual bool IsActive { get; set; }
       
    }
}
