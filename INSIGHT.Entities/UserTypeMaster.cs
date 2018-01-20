using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace INSIGHT.Entities
{
   [DataContract]
   public class UserTypeMaster
    {
         [DataMember]
         public virtual long Id { get; set; }
         [DataMember]
         public virtual string UserType { get; set; }
    }
}
