using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities
{
    [DataContract]
    public class Session
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual string UserId { get; set; }
        [DataMember]
        public virtual DateTime? TimeIn { get; set; }
        [DataMember]
        public virtual DateTime? TimeOut { get; set; }
        [DataMember]
        public virtual string IPAddress { get; set; }
        [DataMember]
        public virtual string BrowserName { get; set; }
        [DataMember]
        public virtual string BrowserVersion { get; set; }
        [DataMember]
        public virtual string Platform { get; set; }
        [DataMember]
        public virtual string BrowserType { get; set; }
        [DataMember]
        public virtual string UserType { get; set; }
       
    }
}
