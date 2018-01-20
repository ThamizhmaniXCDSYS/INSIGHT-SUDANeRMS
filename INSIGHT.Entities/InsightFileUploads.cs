using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace INSIGHT.Entities
{
    public class InsightFileUploads
    {
        [DataMember]
        public virtual long UploadFileId{get;set;}
        [DataMember]
        public virtual string UploadFileCategory { get; set; }
        [DataMember]
        public virtual byte[] UploadFile { get; set; }
        [DataMember]
        public virtual string UploadFileName{get;set;}
        [DataMember]
        public virtual string CreatedBy { get; set; }
        [DataMember]
        public virtual DateTime CreatedDate{get;set;}
        [DataMember]
        public virtual string  ModifiedBy{get;set;}
        [DataMember]
        public virtual DateTime? ModifiedDate { get; set; }
        [DataMember]
        public virtual string ReferenceId { get; set; }


    }
}
