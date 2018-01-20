using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities
{
   
   public  class ImportedDeliveryNote
    {
        [DataMember]
       public virtual long ImpDeliveryNoteId { get; set; }
        [DataMember]
        public virtual string ImpDeliveryNoteName { get; set; }
        [DataMember]
        public virtual long ImpRequestNo { get; set; }
        [DataMember]
        public virtual string ImpWarehouse { get; set; }
        [DataMember]
        public virtual decimal ImpStrength { get; set; }


        [DataMember]
        public virtual decimal ImpDOS { get; set; }
        [DataMember]
        public virtual decimal ImpManDays { get; set; }
        [DataMember]
        public virtual decimal ImpConsumptionWeek { get; set; }
        [DataMember]
        public virtual decimal ImpDeliveryWeek { get; set; }
        [DataMember]
        public virtual string ImpDeliveryMode { get; set; }



        [DataMember]
        public virtual string ImpSealNo { get; set; }
        [DataMember]
        public virtual DateTime ImpShipmentDate { get; set; }
        [DataMember]
        public virtual string ImpControlId { get; set; }
        [DataMember]
        public virtual string ImpUNFoodOrder { get; set; }
        [DataMember]
        public virtual decimal ImpUNWeek { get; set; }
        [DataMember]
        public virtual string ImpPeriod     { get; set; }
        [DataMember]
        public virtual long OrderId { get; set; }

        [DataMember]
        public virtual string Sector { get; set; }
        [DataMember]
        public virtual string Name { get; set; }
        [DataMember]
        public virtual string Location { get; set; }
        [DataMember]
        public virtual string Period { get; set; }
        [DataMember]
        public virtual string CreatedBy { get; set; }
        [DataMember]
        public virtual DateTime CreatedDate { get; set; }
        //[DataMember]
        //public virtual string ModifiedBy { get; set; }
        //[DataMember]
        //public virtual DateTime ModifiedDate { get; set; }
        [DataMember]
        public virtual string PeriodYear { get; set; }
        [DataMember]
        public virtual string ImpDeliveryNoteType { get; set; }
        [DataMember]
        public virtual byte[] DocumentData { get; set; }
    }
}
