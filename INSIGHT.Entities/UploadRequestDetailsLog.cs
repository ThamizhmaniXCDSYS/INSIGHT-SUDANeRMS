using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace INSIGHT.Entities
{
    public class UploadRequestDetailsLog
    {

        [DataMember]
        public virtual long UploadReqDetLogId { get; set; }
        [DataMember]
        public virtual long RequestId { get; set; }

        [DataMember]
        public virtual string FileName { get; set; }
        [DataMember]
        public virtual string UploadStatus { get; set; }
        [DataMember]
        public virtual string ErrDesc { get; set; }

        [DataMember]
        public virtual string CreatedBy { get; set; }

        [DataMember]
        public virtual DateTime CreatedDate { get; set; }
        [DataMember]
        public virtual string ModifiedBy { get; set; }
        [DataMember]
        public virtual DateTime? ModifiedDate { get; set; }
        [DataMember]
        public virtual string ReferenceNo { get; set; }
    }
}
