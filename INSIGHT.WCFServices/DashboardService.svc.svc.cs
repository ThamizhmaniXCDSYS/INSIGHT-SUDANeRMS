using INSIGHT.Component;
using INSIGHT.Entities;
using INSIGHT.Entities.DashboardEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace INSIGHT.WCFServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "DashboardService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select DashboardService.svc or DashboardService.svc.cs at the Solution Explorer and start debugging.
    public class DashboardService 
    {
        DashboardBC DashboardBC = new DashboardBC();
        public IList<TroopsReport> GetTroopsReportbyFlag(string PeriodYear, string Period, string Sector,string ContingentType,string Location,string Flag, string Criteria)
        {

            return DashboardBC.GetTroopsReportbyFlag(PeriodYear, Period,Sector,ContingentType,Location, Flag, Criteria);


        }


        #region Periodwise qty report



        public IList<PeriodWiseQtyReport_SP> GetPeriodWiseQtyReportbyFlag(string Flag, string Criteria1, string Criteria2, string Criteria3)
        {

            return DashboardBC.GetPeriodWiseQtyReportbyFlag(Flag, Criteria1, Criteria2, Criteria3);


        }

        public Dictionary<long, IList<PeriodWiseValuesDashboard_vw>> GetPeriodWiseQtyListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {

                return DashboardBC.GetPeriodWiseQtyListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Periodwise deduction report
        public IList<PeriodWiseDeductionReport> GetPeriodWiseDeductionReportbyFlag(string Flag, string Criteria1,string Criteria2,string Criteria3)
        {

            return DashboardBC.GetPeriodWiseDeductionReportbyFlag(Flag, Criteria1, Criteria2, Criteria3);


        }

        #endregion

        #region loss because substitution report


        public Dictionary<long, IList<LossBecauseOfSubstitutions_vw>> GetLossBecauseOfSubstitutionListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {

                return DashboardBC.GetLossBecauseOfSubstitutionListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion



        public Dictionary<long, IList<AuthorizedUnAuthorizedSubstitutionList_VW>> GetConsolidatedSubstitutionReportsListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {

                return DashboardBC.GetConsolidatedSubstitutionReportsListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }


        #region Substitution report

        #endregion

        #region CMR Trend Report

        public IList<CMRTrendReport> GetCMRTrendByFlag(string Flag, string Criteria1, string Criteria2, string Criteria3,string Criteria4,string Criteria5,string Criteria6)
        {

            return DashboardBC.GetCMRTrendByFlag(Flag, Criteria1, Criteria2, Criteria3,Criteria4,Criteria5,Criteria6);


        }

        #endregion
        #region



        public IList<InsightHomePageDashboard> GetHomePageDashboardByFlag(string Flag)
        {

            return DashboardBC.GetHomePageDashboardByFlag(Flag);


        }
        #endregion

        #region Loss because of excess delivery

        public Dictionary<long, IList<LossBecauseofExcessDelivery>> GetLossBecauseofExcessDeliveryListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {

                return DashboardBC.GetLossBecauseofExcessDeliveryListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public IList<LossBecauseofExcessDelivery> GetLossBecauseofExcessDeliveryReportbyFlag(string Flag, string Criteria1, string Criteria2, string Criteria3, string Criteria4, string Criteria5, string Criteria6)
        {

            return DashboardBC.GetLossBecauseofExcessDeliveryReportbyFlag(Flag, Criteria1, Criteria2, Criteria3, Criteria4, Criteria5, Criteria6);


        }

        #endregion


    }
}
