using INSIGHT.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace INSIGHT.ServiceContract
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IDocuments" in both code and config file together.
    [ServiceContract]
    public interface IDocumentsSC
    {
        long CreateOrUpdateDocuments(Documents doc);
        Documents GetDocumentsById(long EntityRefId);
        Dictionary<long, IList<Documents>> GetDocumentsListWithPaging(int? page, int? pageSize, string sortType, string sortBy, Dictionary<string, object> criteria);
    }
}
