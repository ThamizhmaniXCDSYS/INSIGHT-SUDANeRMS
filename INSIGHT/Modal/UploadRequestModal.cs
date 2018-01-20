using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace INSIGHT.Modal
{
    [DataContract]
    public class UploadRequestModal
    {
        [DataMember]
        public virtual long Config_Id { get; set; }
        [DataMember]
        public virtual string RequestName { get; set; }
        [DataMember]
        public virtual string Period { get; set; }
        [DataMember]
        public virtual string PeriodYear { get; set; }
        [DataMember]
        public virtual long Week { get; set; }
        [DataMember]
        public virtual string Sector { get; set; }
        [DataMember]
        public virtual string ContingentType { get; set; }
        [DataMember]
        public virtual string Name { get; set; }
        [DataMember]
        public virtual string Location { get; set; }
    }
}