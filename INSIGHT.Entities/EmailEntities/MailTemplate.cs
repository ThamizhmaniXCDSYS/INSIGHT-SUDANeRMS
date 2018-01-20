using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace INSIGHT.Entities.EmailEntities
{
    [DataContract]
    public class MailTemplate
    {
        [DataMember]
        public virtual long MailTemplateId { get; set; }
        [DataMember]
        public virtual long MailTemplateMasterId { get; set; }
        [DataMember]
        public virtual string MailTemplateName { get; set; }
        [DataMember]
        public virtual string MailTemplateDescription { get; set; }
        [DataMember]
        public virtual string NewTemplateName { get; set; }
        [DataMember]
        public virtual string ReportName { get; set; }
        [DataMember]
        public virtual long MailColumnId { get; set; }
        [DataMember]
        public virtual string MailColumns { get; set; }
        [DataMember]
        public virtual string SourceList { get; set; }
        [DataMember]
        public virtual string DestinationList { get; set; }
        [DataMember]
        public virtual string EmailList { get; set; }
        [DataMember]
        public virtual bool DailyMail { get; set; }
        [DataMember]
        public virtual bool WeeklyMail { get; set; }
        [DataMember]
        public virtual bool MonthlyMail { get; set; }
        [DataMember]
        public virtual DateTime StartDate { get; set; }
        [DataMember]
        public virtual DateTime? ScheduleNextDate { get; set; }
        [DataMember]
        public virtual string QueryString { get; set; }
        [DataMember]
        public virtual string UserName { get; set; }
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

        //For Display only

        [DataMember]
        public virtual string UserEmail { get; set; }
        [DataMember]
        public virtual string ActiveStatus { get; set; }
        [DataMember]
        public virtual string UserRole { get; set; }
        [DataMember]
        public virtual string UserDate { get; set; }
        [DataMember]
        public virtual DateTime? ScheduleDate { get; set; }

        //[DataMember]
        //public virtual IList<MailActivity> MailActivityList { get; set; }
        //[DataMember]
        //public virtual IList<MailTemplateMaster> MailTemplateMasterList { get; set; }
        //[DataMember]
        //public virtual IList<MailColumn> MailColumnList { get; set; }

    }
}
