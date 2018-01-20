using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using INSIGHT.Entities.TicketingSystem;
using INSIGHT.Component.TicketingSystem;
using INSIGHT.ServiceContract.TicketingSystem;

namespace NSIGHT.Service.TicketingSystem
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    public class TicketSystemService : ITicketSystemService
    {
        public TicketSystem GetTicketSystemById(long Id)
        {
            try
            {
                TicketSystemBC TicketSystemBC = new TicketSystemBC();
                return TicketSystemBC.GetTicketSystemById(Id);
            }
            catch (Exception)
            {
                throw;
            }
            finally { }
        }
        public string SaveOrUpdateTicketSystem(TicketSystem TicketSystem)
        {
            try
            {
                TicketSystemBC TicketSystemBC = new TicketSystemBC();
                return TicketSystemBC.SaveOrUpdateTicketSystem(TicketSystem);
            }
            catch (Exception)
            {
                throw;
            }
            finally { }
        }
        public Dictionary<long, IList<TicketSystem>> GetTicketSystemBCListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                TicketSystemBC TicketSystemBC = new TicketSystemBC();
                return TicketSystemBC.GetTicketSystemBCListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
            finally { }
        }
        public Dictionary<long, IList<Module>> GetModuleListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                TicketSystemBC TicketSystemBC = new TicketSystemBC();
                return TicketSystemBC.GetModuleListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
            finally { }
        }
        public Dictionary<long, IList<Priority>> GetPriorityListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                TicketSystemBC TicketSystemBC = new TicketSystemBC();
                return TicketSystemBC.GetPriorityListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
            finally { }
        }
        public Dictionary<long, IList<TicketStatus>> GetTicketStatusListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                TicketSystemBC TicketSystemBC = new TicketSystemBC();
                return TicketSystemBC.GetTicketStatusListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
            finally { }
        }
        public Dictionary<long, IList<TicketType>> GetTicketTypeListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                TicketSystemBC TicketSystemBC = new TicketSystemBC();
                return TicketSystemBC.GetTicketTypeListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
            finally { }
        }
        public Dictionary<long, IList<Severity>> GetSeverityListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                TicketSystemBC TicketSystemBC = new TicketSystemBC();
                return TicketSystemBC.GetSeverityListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
            finally { }
        }
        public long CreateOrUpdateTicketComments(TicketComments TicketComments)
        {
            try
            {
                TicketSystemBC TicketSystemBC = new TicketSystemBC();
                return TicketSystemBC.CreateOrUpdateTicketComments(TicketComments);
            }
            catch (Exception)
            {
                throw;
            }
            finally { }
        }
        public TicketComments GetTicketCommentsById(long Id)
        {
            try
            {
                TicketSystemBC TicketSystemBC = new TicketSystemBC();
                return TicketSystemBC.GetTicketCommentsById(Id);
            }
            catch (Exception)
            {
                throw;
            }
            finally { }
        }
        public IList<TicketComments> GetTicketCommentsByTicketId(long TicketId)
        {
            try
            {
                TicketSystemBC TicketSystemBC = new TicketSystemBC();
                return TicketSystemBC.GetTicketCommentsByTicketId(TicketId);
            }
            catch (Exception)
            {
                throw;
            }
            finally { }
        }
        public Dictionary<long, IList<TicketComments>> GetTicketCommentsListWithPaging(int? page, int? pagesize, string sortby, string sorttype, Dictionary<string, object> criteria)
        {
            try
            {
                TicketSystemBC TicketSystemBC = new TicketSystemBC();
                return TicketSystemBC.GetTicketCommentsListWithPaging(page, pagesize, sortby, sorttype, criteria);
            }
            catch (Exception)
            {
                throw;
            }
            finally { }
        }
        public bool EditTicketNote(long id, string note)
        {
            try
            {
                TicketSystemBC TicketSystemBC = new TicketSystemBC();
                return TicketSystemBC.EditTicketNote(id, note);
            }
            catch (Exception)
            {
                throw;
            }
            finally { }
        }
        public bool DeleteTicketComments(long[] Ids)
        {
            try
            {
                TicketSystemBC TicketSystemBC = new TicketSystemBC();
                return TicketSystemBC.DeleteTicketComments(Ids);
            }
            catch (Exception)
            {
                throw;
            }
            finally { }
        }
        public bool UpdateTicketStatus(long TicketId, string TicketStatus)
        {
            try
            {
                TicketSystemBC TicketSystemBC = new TicketSystemBC();
                return TicketSystemBC.UpdateTicketStatus(TicketId, TicketStatus);
            }
            catch (Exception)
            {
                throw;
            }
            finally { }
        }

        #region Ticket Report By Gobi
        public Dictionary<long, IList<TicketReport_vw>> GetTicketReport_vwListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                TicketSystemBC TicketSystemBC = new TicketSystemBC();
                return TicketSystemBC.GetTicketReport_vwListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<long, IList<TicketDashBoardReport_vw>> GetTicketDashBoardReport_vwListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                TicketSystemBC TicketSystemBC = new TicketSystemBC();
                return TicketSystemBC.GetTicketDashBoardReport_vwListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
