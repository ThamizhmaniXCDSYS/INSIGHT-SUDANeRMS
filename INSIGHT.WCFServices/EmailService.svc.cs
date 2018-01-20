using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using INSIGHT.Component;
using INSIGHT.Entities.EmailEntities;

namespace INSIGHT.WCFServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "EmailService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select EmailService.svc or EmailService.svc.cs at the Solution Explorer and start debugging.
    public class EmailService : IEmailService
    {
        EmailBC emailBC = new EmailBC();
        #region MailPeriodMaster
        public Dictionary<long, IList<MailPeriodMaster>> GetMailPeriodMasterListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return emailBC.GetMailPeriodMasterListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                return emailBC.GetMailPeriodMasterDetailsById(Id);
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
                return emailBC.GetMailScheduleListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                return emailBC.GetMailScheduleDetailsById(Id);
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
                return emailBC.GetMailTemplateMasterListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                return emailBC.GetMailTemplateMasterDetailsById(Id);
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
                return emailBC.GetEmailScheduleViewListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                return emailBC.GetEmailScheduleViewDetailsById(Id);
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
                return emailBC.GetMailTemplateListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                return emailBC.GetMailConfigListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                return emailBC.GetMailTemplateDetailsById(Id);
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
                return emailBC.SaveOrUpdateMailTemplate(Mt, userId);
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
                return emailBC.GetMailColumnListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                return emailBC.GetMailColumnDetailsById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        public long SaveOrUpdateMailActivity(MailActivity Mt)
        {
            try
            {
                return emailBC.SaveOrUpdateMailActivity(Mt);
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
                return emailBC.GetMailActivityDetailsByMailTemplateId(MailTemplateId,Mailon);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       
    }
}
