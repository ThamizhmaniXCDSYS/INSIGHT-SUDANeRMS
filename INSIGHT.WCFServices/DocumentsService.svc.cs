using INSIGHT.Component;
using INSIGHT.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace INSIGHT.ServiceContract
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Documents" in code, svc and config file together.
    public class DocumentsService : IDocumentsSC
    {
        public long CreateOrUpdateDocuments(Documents doc)
        {
            try
            {
                DocumentsBC DocumentsBC = new DocumentsBC();
                DocumentsBC.CreateOrUpdateDocuments(doc);
                return doc.EntityRefId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        public Documents GetDocumentsById(long EntityRefId)
        {
            try
            {
                DocumentsBC DocumentsBC = new DocumentsBC();
                return DocumentsBC.GetDocumentsById(EntityRefId);
            }
            catch (Exception)
            {
                throw;
            }
            finally { }
        }
        public Dictionary<long, IList<Documents>> GetDocumentsListWithPaging(int? page, int? pageSize, string sortBy, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                DocumentsBC DocumentsBC = new DocumentsBC();
                return DocumentsBC.GetDocumentsListWithPaging(page, pageSize, sortBy, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
            finally { }
        }

        
    }
}
