using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace INSIGHT.Entities.InvoiceEntities
{
    public class TransportSingle
    {
        [DataMember]
        public string ReferenceNo { get; set; }
        [DataMember]
        public string Sector { get; set; }
        [DataMember]
        public string Period { get; set; }
        [DataMember]
        public string SubInvoiceDate { get; set; }
        [DataMember]
        public string PO { get; set; }
        [DataMember]
        public string TotalAcceptedQty { get; set; }
        [DataMember]
        public string TotalTransportCost { get; set; }
        [DataMember]
        public IList<TransportInvoice> TransportInvoiceList { get; set; }
        [DataMember]
        public string Title { get; set; }
    }
}
