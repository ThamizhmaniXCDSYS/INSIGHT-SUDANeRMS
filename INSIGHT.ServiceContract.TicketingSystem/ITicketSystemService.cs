using System;
using System.Collections.Generic;
using System.Linq;
using INSIGHT.Entities.TicketingSystem;
using System.ServiceModel;

namespace INSIGHT.ServiceContract.TicketingSystem
{
    [ServiceContract]
    public interface ITicketSystemService
    {
        [OperationContract]
        TicketSystem GetTicketSystemById(long Id);
        [OperationContract]
        string SaveOrUpdateTicketSystem(TicketSystem TicketSystem);
        [OperationContract]
        Dictionary<long, IList<TicketSystem>> GetTicketSystemBCListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria);
        [OperationContract]
        Dictionary<long, IList<Module>> GetModuleListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria);
        [OperationContract]
        Dictionary<long, IList<Priority>> GetPriorityListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria);
        [OperationContract]
        Dictionary<long, IList<TicketStatus>> GetTicketStatusListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria);
        [OperationContract]
        Dictionary<long, IList<TicketType>> GetTicketTypeListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria);
        [OperationContract]
        Dictionary<long, IList<Severity>> GetSeverityListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria);
        [OperationContract]
        long CreateOrUpdateTicketComments(TicketComments TicketComments);
        [OperationContract]
        TicketComments GetTicketCommentsById(long Id);
        [OperationContract]
        IList<TicketComments> GetTicketCommentsByTicketId(long TicketId);
        [OperationContract]
        Dictionary<long, IList<TicketComments>> GetTicketCommentsListWithPaging(int? page, int? pagesize, string sortby, string sorttype, Dictionary<string, object> criteria);

    }
}
