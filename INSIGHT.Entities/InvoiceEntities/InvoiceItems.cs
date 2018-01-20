using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace INSIGHT.Entities.InvoiceEntities
{
    [DataContract]
    public partial class InvoiceItems
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual long InvoiceId { get; set; }
        [DataMember]
        public virtual long ItemRefId { get; set; }
        [DataMember]
        public virtual long ItemId { get; set; }
    }
}
