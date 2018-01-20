using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using INSIGHT.ServiceContract;
using INSIGHT.Entities;
using INSIGHT.Entities.TicketingSystem;
using INSIGHT.Component;
namespace INSIGHT.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ProcessFlowServices" in code, svc and config file together.
    public class ProcessFlowServices : IProcessFlowSC
    {


        public long CreateActivity(Activity Activity)
        {
            try
            {
                ProcessFlowBC ProcessFlowBC = new ProcessFlowBC();
                return ProcessFlowBC.CreateActivity(Activity);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }

        public bool AssignActivity(long activityId, string userId)
        {
            try
            {
                ProcessFlowBC ProcessFlowBC = new ProcessFlowBC();
                return ProcessFlowBC.AssignActivity(activityId, userId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        public bool AssignActivityCheckBeforeAssigning(long activityId, string userId)
        {
            try
            {
                ProcessFlowBC ProcessFlowBC = new ProcessFlowBC();
                return ProcessFlowBC.AssignActivityCheckBeforeAssigning(activityId, userId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        public ProcessInstance GetProcessInstanceById(long Id)
        {
            try
            {
                ProcessFlowBC ProcessFlowBC = new ProcessFlowBC();
                return ProcessFlowBC.GetProcessInstanceById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }

        public WorkFlowStatus GetWorkFlowStatusById(long Id)
        {
            try
            {
                ProcessFlowBC ProcessFlowBC = new ProcessFlowBC();
                return ProcessFlowBC.GetWorkFlowStatusById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }

        public Activity GetActivityById(long Id)
        {
            try
            {
                ProcessFlowBC ProcessFlowBC = new ProcessFlowBC();
                return ProcessFlowBC.GetActivityById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }


        public Dictionary<long, IList<ProcessInstance>> GetProcessInstanceListWithsearchCriteria(int? page, int? pageSize, string sortType, string sortBy, Dictionary<string, object> criteria)
        {
            try
            {
                ProcessFlowBC ProcessFlowBC = new ProcessFlowBC();
                return ProcessFlowBC.GetProcessInstanceListWithsearchCriteria(page, pageSize, sortType, sortBy, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        public Dictionary<long, IList<WorkFlowStatus>> GetWorkFlowStatusListWithsearchCriteria(int? page, int? pageSize, string sortType, string sortBy, Dictionary<string, object> criteria)
        {
            try
            {
                ProcessFlowBC ProcessFlowBC = new ProcessFlowBC();
                return ProcessFlowBC.GetWorkFlowStatusListWithsearchCriteria(page, pageSize, sortType, sortBy, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }

        public Dictionary<long, IList<TicketSystemActivity>> GetTicketSystemActivityListWithsearchCriteria(int? page, int? pageSize, string sortBy, string sortType, Dictionary<string, object> criteria, string ColumnName, string[] values, string[] alias)
        {
            try
            {
                ProcessFlowBC ProcessFlowBC = new ProcessFlowBC();
                return ProcessFlowBC.GetTicketSystemActivityListWithsearchCriteria(page, pageSize, sortBy, sortType, criteria, ColumnName, values, alias);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }

        public string StartETicketingSystem(TicketSystem TicketSystem, string Template, string userId)
        {
            try
            {
                ProcessFlowBC ProcessFlowBC = new ProcessFlowBC();
                return ProcessFlowBC.StartETicketingSystem(TicketSystem, Template, userId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        public bool CompleteActivityTicketSystem(TicketSystem TicketSystem, string Template, string userId, string ActivityName, bool isRejection)
        {
            try
            {
                bool retValue = false;
                ProcessFlowBC ProcessFlowBC = new ProcessFlowBC();
                ProcessFlowBC.CompleteActivityTicketSystem(TicketSystem, Template, userId, ActivityName, isRejection);
                retValue = true;
                return retValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        public bool MoveBackToAvailable(long activityId)
        {
            try
            {
                ProcessFlowBC ProcessFlowBC = new ProcessFlowBC();
                return ProcessFlowBC.MoveBackToAvailable(activityId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
    }
}
