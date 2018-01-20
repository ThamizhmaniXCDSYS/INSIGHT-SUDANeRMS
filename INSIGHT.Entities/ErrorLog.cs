using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities
{
    [DataContract]
    public class ErrorLog
    {
        [DataMember]
        public virtual long Err_Id { get; set; }
        [DataMember]
        public virtual string Controller { get; set; }
        [DataMember]
        public virtual string Action { get; set; }
        [DataMember]
        public virtual string Err_Desc { get; set; }
        [DataMember]
        public virtual DateTime? CreatedDate { get; set; }
        [DataMember]
        public virtual string CreatedBy { get; set; }
    }
}
