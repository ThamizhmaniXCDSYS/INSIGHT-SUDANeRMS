using System.Runtime.Serialization;

namespace INSIGHT.Entities
{
    public class DocumentUpload
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual byte[] Document { get; set; }
        [DataMember]
        public virtual string DocumentName { get; set; }
        [DataMember]
        public virtual string DocumentType { get; set; }
    }
}
