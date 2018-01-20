using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace INSIGHT.Entities.InvoiceEntities
{
[DataContract]
public partial class InvoiceOrders
{
 [DataMember]
public virtual long Id {get;set;}
 [DataMember]
public virtual long InvoiceId {get;set;}
 [DataMember]
public virtual long OrderId {get;set;}
 [DataMember]
public virtual string InvoiceType {get;set;}
 [DataMember]
public virtual string Status {get;set;}
 [DataMember]
public virtual string CreatedBy {get;set;}
 [DataMember]
public virtual DateTime? CreatedDate {get;set;}
}
}
