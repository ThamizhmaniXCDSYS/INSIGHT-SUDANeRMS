using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistenceFactory;
using INSIGHT.Entities.TicketingSystem;

namespace INSIGHT.Component.TicketingSystem
{
    public class TicketSystemBC
    {
        PersistenceServiceFactory PSF = null;
        public TicketSystemBC()
        {
            List<String> Assembly = new List<String>();
            Assembly.Add("INSIGHT.Entities");
            Assembly.Add("INSIGHT.Entities.TicketingSystem");
            PSF = new PersistenceServiceFactory(Assembly);
        }
        public TicketSystem GetTicketSystemById(long Id)
        {
            try
            {
                return PSF.Get<TicketSystem>(Id);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public string SaveOrUpdateTicketSystem(TicketSystem TicketSystem)
        {
            try
            {
                if (TicketSystem.Id > 0)
                {
                    PSF.SaveOrUpdate<TicketSystem>(TicketSystem);
                }
                else
                {
                    string ticketNo = "ETicket-" + TicketSystem.Id;
                    TicketSystem.TicketNo = ticketNo;
                    PSF.Save<TicketSystem>(TicketSystem);
                }
                return TicketSystem.TicketNo;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<long, IList<TicketSystem>> GetTicketSystemBCListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithExactSearchCriteriaCount<TicketSystem>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<long, IList<Module>> GetModuleListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithExactSearchCriteriaCount<Module>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<long, IList<Priority>> GetPriorityListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithExactSearchCriteriaCount<Priority>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<long, IList<TicketStatus>> GetTicketStatusListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithExactSearchCriteriaCount<TicketStatus>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<long, IList<TicketType>> GetTicketTypeListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithExactSearchCriteriaCount<TicketType>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<long, IList<Severity>> GetSeverityListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithExactSearchCriteriaCount<Severity>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public long CreateOrUpdateTicketComments(TicketComments TicketComments)
        {
            try
            {
                if (TicketComments != null)
                    PSF.SaveOrUpdate<TicketComments>(TicketComments);
                else { throw new Exception("Comments is required and it cannot be null.."); }
                return TicketComments.Id;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public bool EditTicketNote(long id, string note)
        {
            try
            {
                if (id > 0)
                {
                    TicketComments TicketComments = PSF.Get<TicketComments>(id);
                    TicketComments.Note = note;
                    PSF.Update<TicketComments>(TicketComments);
                    return true;
                }

                else return false;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public bool DeleteTicketComments(long[] Ids)
        {
            try
            {
                if (Ids != null && Ids.Length > 0)
                {
                    IList<TicketComments> list = PSF.GetListByIds<TicketComments>(Ids);
                    PSF.DeleteAll<TicketComments>(list);
                    return true;
                }

                else return false;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public TicketComments GetTicketCommentsById(long Id)
        {
            try
            {
                TicketComments TicketComments = null;
                if (Id > 0)
                    TicketComments = PSF.Get<TicketComments>(Id);
                else { throw new Exception("Id is required and it cannot be 0"); }
                return TicketComments;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public IList<TicketComments> GetTicketCommentsByTicketId(long TicketId)
        {
            try
            {
                IList<TicketComments> TicketComments = null;
                if (TicketId > 0)
                    TicketComments = PSF.GetListById<TicketComments>("TicketId", TicketId);
                else { throw new Exception("Id is required and it cannot be 0"); }
                return TicketComments;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public Dictionary<long, IList<TicketComments>> GetTicketCommentsListWithPaging(int? page, int? pagesize, string sortby, string sorttype, Dictionary<string, object> criteria)
        {
            try
            {
                Dictionary<long, IList<TicketComments>> retValue = new Dictionary<long, IList<TicketComments>>();
                return PSF.GetListWithExactSearchCriteriaCount<TicketComments>(page, pagesize, sortby, sorttype, criteria);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public bool UpdateTicketStatus(long TicketId, string TicketStatus)
        {
            try
            {
                if (TicketId > 0)
                {
                    TicketSystem TicketSystem = PSF.Get<TicketSystem>(TicketId);
                    TicketSystem.TicketStatus = TicketStatus;
                    PSF.Update<TicketSystem>(TicketSystem);
                    return true;
                }

                else return false;
            }
            catch (Exception)
            { throw; }
        }

        #region Ticket Report By Gobi
        public Dictionary<long, IList<TicketReport_vw>> GetTicketReport_vwListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<TicketReport_vw>(page, pageSize, sortType, sortby, criteria);
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
                return PSF.GetListWithEQSearchCriteriaCount<TicketDashBoardReport_vw>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
