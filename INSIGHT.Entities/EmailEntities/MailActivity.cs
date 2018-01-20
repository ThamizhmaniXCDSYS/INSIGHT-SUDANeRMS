using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace INSIGHT.Entities.EmailEntities
{
    [DataContract]
    public class MailActivity
    {
        [DataMember]
        public virtual long ActivityId { get; set; }
        [DataMember]
        public virtual long MailTemplateId { get; set; }
        [DataMember]
        public virtual DateTime StartDate { get; set; }
        [DataMember]
        public virtual DateTime ScheduleNextDate { get; set; }
        [DataMember]
        public virtual string MailTo { get; set; }
        [DataMember]
        public virtual string Subject { get; set; }
        [DataMember]
        public virtual string MailDescription { get; set; }
        [DataMember]
        public virtual DateTime MailSentDate { get; set; }
        [DataMember]
        public virtual bool IsActive { get; set; }
        [DataMember]
        public virtual DateTime CreatedDate { get; set; }
        [DataMember]
        public virtual string MailOn { get; set; }

    }
}
