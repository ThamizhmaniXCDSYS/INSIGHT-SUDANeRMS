using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using INSIGHT.Entities.InvoiceEntities;

namespace INSIGHT.Entities.PDFEntities
{
    public class SingleInvoice
    {
        [DataMember]
        public string ImageUrl { get; set; }
        [DataMember]
        public virtual string Reference { get; set; }
        [DataMember]
        public virtual string DeliveryPoint { get; set; }
        [DataMember]
        public virtual string UNID { get; set; }
        [DataMember]
        public virtual decimal Strength { get; set; }
        [DataMember]
        public virtual decimal ManDays { get; set; }
        [DataMember]
        public virtual decimal ApplicableCMR { get; set; }
        [DataMember]
        public virtual decimal AuthorizedCMR { get; set; }
        [DataMember]
        public virtual string Period { get; set; }
        [DataMember]
        public virtual decimal DOS { get; set; }
        [DataMember]
        public virtual string DeliveryWeek { get; set; }
        [DataMember]
        public virtual string DeliveryMode { get; set; }
        [DataMember]
        public virtual string ApprovedDelivery { get; set; }
        [DataMember]
        public virtual string ActualDeliveryDate { get; set; }
        [DataMember]
        public virtual IList<DeliveriesPerOrdQty> deliveriesPerOrdQty { get; set; }
        [DataMember]
        public virtual IList<DeliveryExceed> deliveryExceed { get; set; }
        [DataMember]
        public virtual decimal TotalEggs { get; set; }
        [DataMember]
        public virtual IList<DeliveriesPerOrdQty> deliveriesWithEggsPerOrdQty { get; set; }
        [DataMember]
        public virtual IList<DeliveryExceed> deliveryWithEggsExceed { get; set; }
        [DataMember]
        public virtual IList<DeliveryWithoutOrders> deliveryWithloutOrders { get; set; }
        [DataMember]
        public virtual IList<SingleInvoiceView> DeliveryDetails { get; set; }

        /// <summary>
        /// Subtitude Items total
        /// </summary>

        [DataMember]
        public virtual decimal SDeliveryQuantity { get; set; }
        [DataMember]
        public virtual decimal SOrderedQuantity { get; set; }
        [DataMember]
        public virtual decimal SAcceptedQuantity { get; set; }
        [DataMember]
        public virtual decimal SAcceptedamt { get; set; }

        /// <summary>
        /// Replacement Items total
        /// </summary>

        [DataMember]
        public virtual decimal RDeliveryQuantity { get; set; }
        [DataMember]
        public virtual decimal ROrderedQuantity { get; set; }
        [DataMember]
        public virtual decimal RAcceptedQuantity { get; set; }
        [DataMember]
        public virtual decimal RAcceptedamt { get; set; }


        [DataMember]
        public virtual long NOD { get; set; }

        [DataMember]
        public virtual long TotalDays { get; set; }

        [DataMember]
        public virtual IList<OrderListWithoutDelivery> ordersWithoutDelivery { get; set; }

        [DataMember]
        public virtual decimal OrderedQtySum { get; set; }
        [DataMember]
        public virtual decimal DeliveredQtySum { get; set; }
        [DataMember]
        public virtual decimal AcceptedQtySum { get; set; }
        [DataMember]
        public virtual decimal InvoiceQtySum { get; set; }
        [DataMember]
        public virtual decimal NetAmountSum { get; set; }
        [DataMember]
        public virtual decimal OrdervalueSum { get; set; }


        [DataMember]
        public virtual decimal EggOrderedQtySum { get; set; }
        [DataMember]
        public virtual decimal EggDeliveredQtySum { get; set; }
        [DataMember]
        public virtual decimal EggAcceptedQtySum { get; set; }
        [DataMember]
        public virtual decimal EggInvoiceQtySum { get; set; }

        [DataMember]
        public virtual decimal TotalOrderedQtySum { get; set; }
        [DataMember]
        public virtual decimal TotalDeliveredQtySum { get; set; }
        [DataMember]
        public virtual decimal TotalAcceptedQtySum { get; set; }
        [DataMember]
        public virtual decimal TotalInvoiceQtySum { get; set; }

        [DataMember]
        public virtual decimal AboveCount { get; set; }
        [DataMember]
        public virtual decimal BelowCount { get; set; }
        [DataMember]
        public virtual decimal CountPercent { get; set; }

        [DataMember]
        public virtual decimal DeliveryPerformance { get; set; }
        [DataMember]
        public virtual decimal LineItemPerformance { get; set; }
        [DataMember]
        public virtual decimal OrderWightPerformance { get; set; }
        [DataMember]
        public virtual decimal SubtitutionPerformance { get; set; }

        [DataMember]
        public virtual decimal DeliveryDeduction { get; set; }
        [DataMember]
        public virtual decimal LineItemDeduction { get; set; }
        [DataMember]
        public virtual decimal OrderWightDeduction { get; set; }
        [DataMember]
        public virtual decimal SubtitutionDeduction { get; set; }

        //Sustitution Delivery List
        [DataMember]
        public virtual IList<SubReplacementView> SDeliveryList { get; set; }

        //ReplaceMent Delivery List
        [DataMember]
        public virtual IList<SubReplacementView> RDeliveryList { get; set; }

        [DataMember]
        public virtual decimal TotalLineitem98 { get; set; }
        [DataMember]
        public virtual long SubstituteCount { get; set; }
        [DataMember]
        public virtual decimal AmountSubstituted { get; set; }
        [DataMember]
        public virtual decimal OrderCMR { get; set; }
        [DataMember]
        public virtual decimal AcceptedCMR { get; set; }
        [DataMember]
        public virtual decimal CMRUtilized { get; set; }
        [DataMember]
        public virtual string Deliverynotes { get; set; }
        [DataMember]
        public DateTime ApprovedDeliveryDate { get; set; }
        /// <summary>
        /// UNSubstitution Items count
        /// </summary>
        [DataMember]
        public virtual decimal UNItemCount { get; set; }
        [DataMember]
        public virtual bool IsAPL { get; set; }
        [DataMember]
        public virtual decimal TotalAcceptedQuantity { get; set; }
        [DataMember]
        public virtual decimal AmountAccepted { get; set; }
        [DataMember]
        public virtual decimal NetRationAmount { get; set; }
        [DataMember]
        public virtual decimal Weeklyinvoicediscount { get; set; }
        [DataMember]
        public virtual decimal confirmityCMR { get; set; }
        [DataMember]
        public virtual decimal AcceptedTransportCost { get; set; }
        [DataMember]
        public virtual decimal RatePerKg { get; set; }
        [DataMember]
        public virtual long ContingentID { get; set; }
    }

    public class PenaltyCaculation
    {
        [DataMember]
        public virtual long TotalDays { get; set; }
        [DataMember]
        public virtual decimal TotalLineitem { get; set; }
        [DataMember]
        public virtual decimal TotalLineitem98 { get; set; }
        [DataMember]
        public virtual long SubstituteCount { get; set; }
        [DataMember]
        public virtual decimal OrderedQty { get; set; }
        [DataMember]
        public virtual decimal InvoiceQty { get; set; }
        [DataMember]
        public virtual decimal OrdervalueSum { get; set; }

        [DataMember]
        public virtual decimal DeliveryPerformance { get; set; }
        [DataMember]
        public virtual decimal DeliveryDeduction { get; set; }
        [DataMember]
        public virtual decimal LineItemPerformance { get; set; }
        [DataMember]
        public virtual decimal LineItemDeduction { get; set; }
        [DataMember]
        public virtual decimal OrderWeightPerformance { get; set; }
        [DataMember]
        public virtual decimal OrderWeightDeduction { get; set; }
        [DataMember]
        public virtual decimal ComplaintsPerformance { get; set; }
        [DataMember]
        public virtual decimal ComplaintsDeduction { get; set; }

    }
}
