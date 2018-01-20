using INSIGHT.Entities.EmailEntities;
using PersistenceFactory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace INSIGHT.Component
{
    public class EmailBC
    {
        PersistenceServiceFactory PSF = null;
        ProjectSpecificPSF SPSF = null;
        public EmailBC()
        {
            List<String> Assembly = new List<String>();
            Assembly.Add("INSIGHT.Entities");
            Assembly.Add("INSIGHT.Entities.TicketingSystem");
            PSF = new PersistenceServiceFactory(Assembly);
            SPSF = new ProjectSpecificPSF(Assembly);
        }
        #region MailPeriodMaster
        public Dictionary<long, IList<MailPeriodMaster>> GetMailPeriodMasterListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<MailPeriodMaster>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public MailPeriodMaster GetMailPeriodMasterDetailsById(long Id)
        {
            try
            {
                return PSF.Get<MailPeriodMaster>("MailPeriodMasterId", Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region MailSchedule
        public Dictionary<long, IList<MailSchedule>> GetMailScheduleListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<MailSchedule>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public MailSchedule GetMailScheduleDetailsById(long Id)
        {
            try
            {
                return PSF.Get<MailSchedule>("MailScheduleId", Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region MailTemplateMaster
        public Dictionary<long, IList<MailTemplateMaster>> GetMailTemplateMasterListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<MailTemplateMaster>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public MailTemplateMaster GetMailTemplateMasterDetailsById(long Id)
        {
            try
            {
                return PSF.Get<MailTemplateMaster>("MailTemplateMasterId", Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region EmailScheduleView
        public Dictionary<long, IList<EmailScheduleView>> GetEmailScheduleViewListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<EmailScheduleView>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public EmailScheduleView GetEmailScheduleViewDetailsById(long Id)
        {
            try
            {
                return PSF.Get<EmailScheduleView>("Id", Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region MailTemplate
        public Dictionary<long, IList<MailTemplate>> GetMailTemplateListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<MailTemplate>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Dictionary<long, IList<MailConfig>> GetMailConfigListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<MailConfig>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public MailTemplate GetMailTemplateDetailsById(long Id)
        {
            try
            {
                return PSF.Get<MailTemplate>("MailTemplateId", Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public long SaveOrUpdateMailTemplate(MailTemplate Mt, string userId)
        {
            try
            {
                if (Mt != null && Mt.MailTemplateId > 0)
                {
                    Mt.ModifiedBy = userId;
                    Mt.ModifiedDate = DateTime.Now;
                    PSF.Update<MailTemplate>(Mt);
                }
                else
                {

                    Mt.CreatedBy = userId;
                    Mt.CreatedDate = DateTime.Now;
                    Mt.ModifiedDate = null;
                    PSF.Save<MailTemplate>(Mt);
                }
                return Mt.MailTemplateId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
        #region MailColumn
        public Dictionary<long, IList<MailColumn>> GetMailColumnListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<MailColumn>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public MailColumn GetMailColumnDetailsById(long Id)
        {
            try
            {
                return PSF.Get<MailColumn>("MailColumnId", Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion



        public long[] GetMailTemplatesIdUsingQuery()
        {
            try
            {
                string query = "Select Distinct MailTemplateId from MailActivity where ScheduleNextDate = CAST(GETDATE() AS DATE) AND IsActive= 1";
                long[] empty = new long[5];
                IList list = PSF.ExecuteSql(query);
                if (list != null && list[0] != null)
                {
                    long[] longlist = new long[list.Count];
                    int i = 0;
                    foreach (var item in list)
                    {
                        longlist[i] = Convert.ToInt64(item);
                        i++;
                    }
                    return longlist; 
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public IList GetListWithQuery(string QueryString)
        {
            try
            {
                IList List = PSF.ExecuteSql(QueryString);
                if (List != null && List[0] != null)
                    return List;
                else
                    return null;
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public DataTable GetListWithDataTable(string QueryString)
        {
            DataTable dt = PSF.ExecuteSqlUsingSQLCommand(QueryString);
            if (dt != null)
                return dt;
            else
                return null;
        }
        public long SaveOrUpdateMailActivity(MailActivity Mt)
        {
            try
            {
                if (Mt != null && Mt.ActivityId > 0)
                {
                    PSF.Update<MailActivity>(Mt);
                }
                else
                {

                    Mt.CreatedDate = DateTime.Now;
                    PSF.Save<MailActivity>(Mt);
                }
                return Mt.ActivityId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public MailActivity GetMailActivityDetailsByMailTemplateId(long MailTemplateId, string Mailon)
        {
            try
            {
                return PSF.Get<MailActivity>("MailTemplateId", MailTemplateId, "MailOn", Mailon);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
