using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace INSIGHT.Entities.DeletedRecordEntities
{
    [DataContract]
    public class DeliveryNoteDel
    {
        [DataMember]
        public virtual long Id { get; set; }
        [DataMember]
        public virtual long DeliveryNoteId { get; set; }
        [DataMember]
        public virtual string DeliveryNoteName { get; set; }
        [DataMember]
        public virtual string CreatedBy { get; set; }
        [DataMember]
        public virtual DateTime CreatedDate { get; set; }
        [DataMember]
        public virtual string DeliveryMode { get; set; }

        // added for Create PDF

        public string ImageUrl { get; set; }

        public long OrderId { get; set; }
        public string DNNo { get; set; }
        public string RequestNo { get; set; }
        public string WareHouse { get; set; }
        public decimal ContingentStrength { get; set; }
        public string DOS { get; set; }
        public Int32 ManDays { get; set; }
        public string ConsumptionWeek { get; set; }
        public string DeliveryWeek { get; set; }
        public string DeliveryBy { get; set; }
        public string SealNo { get; set; }
        public string ShipmentDate { get; set; }
        public string UnitControlNo { get; set; }
        public string UNFeedOrders { get; set; }
        public string UNWeek { get; set; }
        public string Period { get; set; }
        public IList<DeliveredPODItems_vw> DeliveredPODItems { get; set; }
        public string DeliveryStatus { get; set; }

        [DataMember]
        public virtual string DeletedBy { get; set; }
        [DataMember]
        public virtual DateTime? DeletedDate { get; set; }

    }
}
