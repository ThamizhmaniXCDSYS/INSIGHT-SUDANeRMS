using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace INSIGHT.Entities
{
    public class UploadRequest
    {
        [DataMember]
        public virtual long RequestId { get; set; }
        [DataMember]
        public virtual string RequestName { get; set; }
        [DataMember]
        public virtual string RequestNo { get; set; }
        [DataMember]
        public virtual string Category { get; set; }

        [DataMember]
        public virtual string Status { get; set; }
        [DataMember]
        public virtual string CreatedBy { get; set; }

        [DataMember]
        public virtual DateTime CreatedDate { get; set; }
        [DataMember]
        public virtual string ModifiedBy{ get; set; }
        [DataMember]
        public virtual DateTime? ModifiedDate { get; set; }
        [DataMember]
        public virtual string UploadStatus { get; set; }
        [DataMember]
        public virtual string Period { get; set; }
        [DataMember]
        public virtual string PeriodYear { get; set; }
        [DataMember]
        public virtual string ErrorDesc { get; set; }
        //Added by Thamizh for Revised DN Upload
        [DataMember]
        public virtual long Week { get; set; }
        [DataMember]
        public virtual string FileName { get; set; }
        public virtual string Sector { get; set;}
    }
}
