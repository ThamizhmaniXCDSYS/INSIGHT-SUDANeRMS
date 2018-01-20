using System;
using System.Collections.Generic;
using System.Linq;
using INSIGHT.Entities.TicketingSystem;
using System.ServiceModel;
using INSIGHT.Entities;

namespace INSIGHT.ServiceContract
{
    [ServiceContract]
    public interface IProcessFlowSC
    {
        [OperationContract]
        long CreateActivity(Activity Activity);
        [OperationContract]
        ProcessInstance GetProcessInstanceById(long Id);
        [OperationContract]
        WorkFlowStatus GetWorkFlowStatusById(long Id);
        [OperationContract]
        Activity GetActivityById(long Id);
        [OperationContract]
        Dictionary<long, IList<ProcessInstance>> GetProcessInstanceListWithsearchCriteria(int? page, int? pageSize, string sortType, string sortBy, Dictionary<string, object> criteria);
        [OperationContract]
        Dictionary<long, IList<WorkFlowStatus>> GetWorkFlowStatusListWithsearchCriteria(int? page, int? pageSize, string sortType, string sortBy, Dictionary<string, object> criteria);
        [OperationContract]
        bool AssignActivity(long activityId, string userId);
        [OperationContract]
        bool AssignActivityCheckBeforeAssigning(long activityId, string userId);
        [OperationContract]
        Dictionary<long, IList<TicketSystemActivity>> GetTicketSystemActivityListWithsearchCriteria(int? page, int? pageSize, string sortBy, string sortType, Dictionary<string, object> criteria, string ColumnName, string[] values, string[] alias);
        [OperationContract]
        string StartETicketingSystem(TicketSystem TicketSystem, string Template, string userId);
    }
}
