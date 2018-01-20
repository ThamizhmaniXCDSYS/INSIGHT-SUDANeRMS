using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace INSIGHT.Entities.EmailEntities
{
    [DataContract]
    public class MailSchedule
    {
        [DataMember]
        public virtual long MailScheduleId { get; set; }
        [DataMember]
        public virtual long MailPeriodMasterId { get; set; }
        [DataMember]
        public virtual long MailTemplateMasterId { get; set; }
        [DataMember]
        public virtual DateTime ScheduleDate { get; set; }
        [DataMember]
        public virtual string UserName { get; set; }
        [DataMember]
        public virtual string UserMailId { get; set; }
        [DataMember]
        public virtual long UserRefId { get; set; }
        [DataMember]
        public virtual DateTime? CreatedDate { get; set; }
        [DataMember]
        public virtual string CreatedBy { get; set; }
        [DataMember]
        public virtual string ModifiedBy { get; set; }
        [DataMember]
        public virtual DateTime? ModifiedDate { get; set; }
        [DataMember]
        public virtual bool IsActive { get; set; }

    }
}
