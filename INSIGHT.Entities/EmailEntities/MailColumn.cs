using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace INSIGHT.Entities.EmailEntities
{
    [DataContract]
    public class MailColumn
    {
        [DataMember]
        public virtual long MailColumnId { get; set; }
        [DataMember]
        public virtual long MailTemplateMasterId { get; set; }
        [DataMember]
        public virtual string Columns { get; set; }
        [DataMember]
        public virtual string TableName { get; set; }
        [DataMember]
        public virtual string RollOverId { get; set; }
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
