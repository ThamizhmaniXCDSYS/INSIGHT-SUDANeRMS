using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistenceFactory;
using INSIGHT.Entities;

namespace INSIGHT.Component
{
   public class DocumentsBC
    {
       PersistenceServiceFactory PSF = null;
       public DocumentsBC()
        {
            List<String> Assembly = new List<String>();
            Assembly.Add("INSIGHT.Entities");
            Assembly.Add("INSIGHT.Entities.TicketingSystem");
            PSF = new PersistenceServiceFactory(Assembly);
        }
       public long CreateOrUpdateDocuments(Documents doc)
       {
           try
           {
               if (doc != null)
                   PSF.SaveOrUpdate<Documents>(doc);
               else { throw new Exception("Documents is required and it cannot be null.."); }
               return doc.EntityRefId;
           }
           catch (Exception)
           {

               throw;
           }
       }
       public Documents GetDocumentsById(long EntityRefId)
       {
           try
           {
               Documents Documents = null;
               if (EntityRefId > 0)
                   Documents = PSF.Get<Documents>(EntityRefId);
               else { throw new Exception("Id is required and it cannot be 0"); }
               return Documents;
           }
           catch (Exception)
           {

               throw;
           }
       }
       public Dictionary<long, IList<Documents>> GetDocumentsListWithPaging(int? page, int? pageSize, string sortBy, string sortType, Dictionary<string, object> criteria)
       {
           try
           {
               Dictionary<long, IList<Documents>> retValue = new Dictionary<long, IList<Documents>>();
               return PSF.GetListWithExactSearchCriteriaCount<Documents>(page, pageSize, sortBy, sortType, criteria);
           }
           catch (Exception)
           {

               throw;
           }
       }
       public void DeleteUploadedFileById(Documents doc)
       {
           if (doc.Upload_Id > 0)
           {
               PSF.Delete<Documents>(doc);
           }
       }
      
    }
}
