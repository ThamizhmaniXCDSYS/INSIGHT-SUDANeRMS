using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;


namespace INSIGHT.Entities
{
    
   public class ImportedDeliveryNoteItems
    {
        [DataMember]
       public virtual long ImpDeliveryNoteItemsId { get; set; }
        [DataMember]
        public virtual long ImpDeliveryNoteId { get; set; }
        [DataMember]
        public virtual string ImpDeliveryNoteName { get; set; }
        [DataMember]
        public virtual string ImpControlId { get; set; }
        [DataMember]
        public virtual long ImpUNCode { get; set; }


        [DataMember]
        public virtual string ImpCommodity { get; set; }
        [DataMember]
        public virtual decimal ImpOrderQty { get; set; }
        [DataMember]
        public virtual decimal ImpDeliveryQty    { get; set; }
        [DataMember]
        public virtual decimal ImpNoOfPacks { get; set; }
        [DataMember]
        public virtual decimal ImpNoOfPieces { get; set; }



        [DataMember]
        public virtual long ImpSubsItemCode { get; set; }
        [DataMember]
        public virtual string ImpSubsItemName { get; set; }
        [DataMember]
        public virtual string ImpUOM { get; set; }
        [DataMember]
        public virtual string ImpIssueType { get; set; }
        [DataMember]
        public virtual string ImpRemarks { get; set; }
        [DataMember]
        public virtual long OrderId { get; set; }
        [DataMember]
        public virtual string CreatedBy { get; set; }
        [DataMember]
        public virtual DateTime CreatedDate { get; set; }
        //[DataMember]
        //public virtual string ModifiedBy { get; set; }
        //[DataMember]
        //public virtual DateTime ModifiedDate { get; set; }
        [DataMember]
        public virtual string ImpDeliveryMode { get; set; }
        [DataMember]
        public virtual string ImpSubsStatus { get; set; }
        [DataMember]
        public virtual decimal ImpExpDeliveryQty { get;set;}
        [DataMember]
        public virtual DateTime? ImpDeliveryDate{ get; set; }
        [DataMember]
        public virtual string IsValid { get; set; }

       
    }
}
