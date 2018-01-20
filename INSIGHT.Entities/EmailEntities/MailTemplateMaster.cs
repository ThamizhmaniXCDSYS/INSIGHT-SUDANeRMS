using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace INSIGHT.Entities.EmailEntities
{
    [DataContract]
    public class MailTemplateMaster
    {
        [DataMember]
        public virtual long MailTemplateMasterId { get; set; }
        [DataMember]
        public virtual string MailTemplate { get; set; }
        [DataMember]
        public virtual string Description { get; set; }
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
