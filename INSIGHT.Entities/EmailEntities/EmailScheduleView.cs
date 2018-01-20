using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace INSIGHT.Entities.EmailEntities
{
    [DataContract]
    public class EmailScheduleView
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual string MailTemplate { get; set; }
        [DataMember]
        public virtual string MailPeriod { get; set; }
        [DataMember]
        public virtual DateTime ScheduleDate { get; set; }
        [DataMember]
        public virtual string Username { get; set; }
        [DataMember]
        public virtual string Usermailid { get; set; }
        [DataMember]
        public virtual long UserRefid { get; set; }
        [DataMember]
        public virtual string Createdby { get; set; }
        [DataMember]
        public virtual string ModifiedBy { get; set; }
        [DataMember]
        public virtual bool IsActive { get; set; }
    }
}
