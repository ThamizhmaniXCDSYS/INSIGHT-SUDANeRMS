using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INSIGHT.Entities
{

   public  class InitialOrders
    {
        
        public virtual long OrderId { get; set; }



        
        public virtual string Name { get; set; }
       
        public virtual string ContingentType { get; set; }
      
        public virtual string Location { get; set; }
   
        public virtual string ControlId { get; set; }

        public virtual DateTime StartDate { get; set; }
        
        public virtual DateTime EndDate { get; set; }
       
        public virtual decimal Troops { get; set; }
       
        public virtual decimal TotalAmount { get; set; }


      
        public virtual decimal LineItemsOrdered { get; set; }
      
        public virtual decimal KgOrderedWOEggs { get; set; }
       
        public virtual decimal EggsWeight { get; set; }
     
        public virtual decimal TotalWeight { get; set; }

     
        public virtual string CreatedBy { get; set; }
  
        public virtual DateTime CreatedDate { get; set; }
 
        public virtual decimal LocationCMR { get; set; }
    
        public virtual decimal ControlCMR { get; set; }

        public virtual string Period { get; set; }
        
        public virtual string Sector { get; set; }
   
        public virtual Int64 Week { get; set; }
       
        public virtual string PeriodYear { get; set; }
       
        public virtual DateTime? ExpectedDeliveryDate { get; set; }
   
        public virtual string ModifiedBy { get; set; }
      
        public virtual DateTime? ModifiedDate { get; set; }
      
        public virtual IList<OrderItems> ListOfItems { get; set; }


        public virtual Int64 InvoiceId { get; set; }
     
        public virtual Int64 PODId { get; set; }
       
        public virtual string FinalStatus { get; set; }
        public virtual string OpeningStatus { get; set; }

        public virtual string InvoiceStatus { get; set; }

        public virtual Int64 CalYear { get; set; }


        public virtual byte[] DocumentData { get; set; }


    }
}
