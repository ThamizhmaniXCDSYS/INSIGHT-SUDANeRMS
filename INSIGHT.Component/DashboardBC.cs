using INSIGHT.Entities;
using INSIGHT.Entities.DashboardEntities;
using PersistenceFactory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INSIGHT.Component
{
    public class DashboardBC
    {
        PersistenceServiceFactory PSF = null;
        ProjectSpecificPSF SPSF = null;
        public DashboardBC()
        {
            List<String> Assembly = new List<String>();
            Assembly.Add("INSIGHT.Entities");
            Assembly.Add("INSIGHT.Entities.TicketingSystem");
           
            PSF = new PersistenceServiceFactory(Assembly);
            SPSF = new ProjectSpecificPSF(Assembly);

        }

        public IList<TroopsReport> GetTroopsReportbyFlag(string PeriodYear, string Period, string Sector,string ContingentType,string Location,string Flag, string Criteria)
        {
            try
            {

                string query = "Exec TroopReport @spPeriodYear='" + PeriodYear;
                query = query + "', @spPeriod='" + Period;
                query = query + "', @spFlag='" + Flag+"'";
                if (Sector==null)
                    query = query + ", @spSector=null";
                else
                    query = query + ", @spSector='" + Sector + "'";
                if (ContingentType == null)
                    query = query + ", @spContingentType=null";
                else
                    query = query + ", @spContingentType='" + ContingentType + "'";
                if (Location == null)
                    query = query + ", @spLocation=null";
                else
                    query = query + ", @spLocation='" + Location + "'";
                if (Criteria == null)
                    query = query + ", @spCriteria=null";

                else
                    query = query + ", @spCriteria='" + Criteria + "'";

                IList list = PSF.ExecuteSql(query);

                IList<TroopsReport> periodlist = new List<TroopsReport>();
                switch (Flag)
                {
                    case "PERIODYEAR":
                        foreach (var obj in list)
                        {
                            TroopsReport troops = new TroopsReport();
                            troops.TroopsReportId = Convert.ToInt64(((object[])(obj))[0]);
                            troops.PeriodYear = Convert.ToString(((object[])(obj))[1]);
                            troops.SumofTroops = Convert.ToDecimal(((object[])(obj))[2]);
                            periodlist.Add(troops);

                        }
                        break;
                    case "PERIOD":
                        foreach (var obj in list)
                        {
                            TroopsReport troops = new TroopsReport();
                            troops.TroopsReportId = Convert.ToInt64(((object[])(obj))[0]);
                            troops.PeriodYear = Convert.ToString(((object[])(obj))[1]);
                            troops.Period = Convert.ToString(((object[])(obj))[2]);
                            //troops.PeriodYear = Convert.ToString(((object[])(obj))[2]);
                            troops.SumofTroops = Convert.ToDecimal(((object[])(obj))[3]);
                            periodlist.Add(troops);

                        }
                        break;

                    case "SECTOR":
                        foreach (var obj in list)
                        {
                            TroopsReport troops = new TroopsReport();
                            troops.TroopsReportId = Convert.ToInt64(((object[])(obj))[0]);
                            troops.PeriodYear = Convert.ToString(((object[])(obj))[1]);
                            troops.Period = Convert.ToString(((object[])(obj))[2]);
                            troops.ContingentType = Convert.ToString(((object[])(obj))[3]);
                            troops.Sector = Convert.ToString(((object[])(obj))[4]);
                            //troops.PeriodYear = Convert.ToString(((object[])(obj))[2]);
                            troops.SumofTroops = Convert.ToDecimal(((object[])(obj))[5]);
                            periodlist.Add(troops);

                        }
                        break;
                    case "LOCATION":
                        foreach (var obj in list)
                        {
                            TroopsReport troops = new TroopsReport();
                            troops.TroopsReportId = Convert.ToInt64(((object[])(obj))[0]);
                            troops.PeriodYear = Convert.ToString(((object[])(obj))[1]);
                            troops.Period = Convert.ToString(((object[])(obj))[2]);
                            troops.Sector = Convert.ToString(((object[])(obj))[3]);
                            troops.Location = Convert.ToString(((object[])(obj))[4]);
                            troops.Week = Convert.ToInt64(((object[])(obj))[5]);
                            troops.SumofTroops = Convert.ToDecimal(((object[])(obj))[6]);
                            troops.rank = Convert.ToInt64(((object[])(obj))[7]);
                            periodlist.Add(troops);

                        }
                        break;
                    case "NAME":
                        foreach (var obj in list)
                        {
                            TroopsReport troops = new TroopsReport();
                            troops.TroopsReportId = Convert.ToInt64(((object[])(obj))[0]);
                            troops.PeriodYear = Convert.ToString(((object[])(obj))[1]);
                            troops.Period = Convert.ToString(((object[])(obj))[2]);
                            troops.Sector = Convert.ToString(((object[])(obj))[3]);
                            troops.ContingentType = Convert.ToString(((object[])(obj))[4]);
                            troops.Location = Convert.ToString(((object[])(obj))[5]);
                            troops.Week = Convert.ToInt64(((object[])(obj))[6]);
                            troops.Name = Convert.ToString(((object[])(obj))[7]);
                            troops.SumofTroops = Convert.ToDecimal(((object[])(obj))[8]);
                            periodlist.Add(troops);

                        }
                        break;

                    case "CONTINGENTTYPE":
                        foreach (var obj in list)
                        {
                            TroopsReport troops = new TroopsReport();
                            troops.TroopsReportId = Convert.ToInt64(((object[])(obj))[0]);
                            troops.ContingentType = Convert.ToString(((object[])(obj))[1]);
                            //troops.PeriodYear = Convert.ToString(((object[])(obj))[2]);
                            troops.SumofTroops = Convert.ToDecimal(((object[])(obj))[2]);
                            periodlist.Add(troops);

                        }
                        break;
                    case "WEEK":
                        foreach (var obj in list)
                        {
                            TroopsReport troops = new TroopsReport();
                            troops.TroopsReportId = Convert.ToInt64(((object[])(obj))[0]);
                            troops.Week = Convert.ToInt64(((object[])(obj))[1]);
                            //troops.PeriodYear = Convert.ToString(((object[])(obj))[2]);
                            troops.SumofTroops = Convert.ToDecimal(((object[])(obj))[2]);
                            periodlist.Add(troops);

                        }
                        break;
                    default:
                        break;


                }

                return periodlist;
                // return PSF.ExecuteSql<samplestoredproceduure>(query);

            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Periodwise qty report
        public Dictionary<long, IList<PeriodWiseValuesDashboard_vw>> GetPeriodWiseQtyListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<PeriodWiseValuesDashboard_vw>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public IList<PeriodWiseQtyReport_SP> GetPeriodWiseQtyReportbyFlag(string Flag, string Criteria1, string Criteria2, string Criteria3)
        {
            try
            {

                string query = "Exec PeriodWiseQtyReport_SP @spFlag='" + Flag;

                if (string.IsNullOrEmpty(Criteria1))
                    query = query + "', @spCriteria1=null";
                else
                    query = query + "', @spCriteria1='" + Criteria1 + "'";
                if (string.IsNullOrEmpty(Criteria2))
                    query = query + ", @spCriteria2=null";
                else
                    query = query + ", @spCriteria2='" + Criteria2 + "'";
                if (string.IsNullOrEmpty(Criteria3))
                    query = query + ", @spCriteria3=null";
                else
                    query = query + ", @spCriteria3='" + Criteria3 + "'";
              

                IList list = PSF.ExecuteSql(query);

                IList<PeriodWiseQtyReport_SP> qtylist = new List<PeriodWiseQtyReport_SP>();
                switch (Flag)
                {
                    case "PERIODYEAR":
                        foreach (var obj in list)
                        {
                            PeriodWiseQtyReport_SP qtyobj = new PeriodWiseQtyReport_SP();
                            qtyobj.Id = Convert.ToInt64(((object[])(obj))[0]);
                            qtyobj.PeriodYear = Convert.ToString(((object[])(obj))[1]);
                            qtyobj.OrderQty = Convert.ToDecimal(((object[])(obj))[2]);
                            qtyobj.DeliveredQty = Convert.ToDecimal(((object[])(obj))[3]);
                            qtyobj.InvoiceQty = Convert.ToDecimal(((object[])(obj))[4]);
                            qtyobj.InvoiceValue = Convert.ToDecimal(((object[])(obj))[5]);


                            qtylist.Add(qtyobj);

                        }
                        break;
                    case "PERIOD":
                        foreach (var obj in list)
                        {
                            PeriodWiseQtyReport_SP qtyobj = new PeriodWiseQtyReport_SP();
                            qtyobj.Id = Convert.ToInt64(((object[])(obj))[0]);
                            qtyobj.PeriodYear = Convert.ToString(((object[])(obj))[1]);
                            qtyobj.Period = Convert.ToString(((object[])(obj))[2]);
                            qtyobj.OrderQty = Convert.ToDecimal(((object[])(obj))[3]);
                            qtyobj.DeliveredQty = Convert.ToDecimal(((object[])(obj))[4]);
                            qtyobj.InvoiceQty = Convert.ToDecimal(((object[])(obj))[5]);
                            qtyobj.InvoiceValue = Convert.ToDecimal(((object[])(obj))[6]);
                            qtylist.Add(qtyobj);

                        }
                        break;

                    case "SECTOR":
                        foreach (var obj in list)
                        {
                            PeriodWiseQtyReport_SP qtyobj = new PeriodWiseQtyReport_SP();
                            qtyobj.Id = Convert.ToInt64(((object[])(obj))[0]);
                            qtyobj.PeriodYear = Convert.ToString(((object[])(obj))[1]);
                            qtyobj.Period = Convert.ToString(((object[])(obj))[2]);
                            qtyobj.Sector = Convert.ToString(((object[])(obj))[3]);
                            qtyobj.ContingentType = Convert.ToString(((object[])(obj))[4]);

                            qtyobj.OrderQty = Convert.ToDecimal(((object[])(obj))[5]);
                            qtyobj.DeliveredQty = Convert.ToDecimal(((object[])(obj))[6]);
                            qtyobj.InvoiceQty = Convert.ToDecimal(((object[])(obj))[7]);
                            qtyobj.InvoiceValue = Convert.ToDecimal(((object[])(obj))[8]);
                            qtylist.Add(qtyobj);

                        }
                        break;
                    //case "LOCATION":
                    //    foreach (var obj in list)
                    //    {
                    //        TroopsReport troops = new TroopsReport();
                    //        troops.TroopsReportId = Convert.ToInt64(((object[])(obj))[0]);
                    //        troops.PeriodYear = Convert.ToString(((object[])(obj))[1]);
                    //        troops.Period = Convert.ToString(((object[])(obj))[2]);
                    //        troops.Sector = Convert.ToString(((object[])(obj))[3]);
                    //        troops.Location = Convert.ToString(((object[])(obj))[4]);
                    //        troops.Week = Convert.ToInt64(((object[])(obj))[5]);
                    //        troops.SumofTroops = Convert.ToDecimal(((object[])(obj))[6]);
                    //        troops.rank = Convert.ToInt64(((object[])(obj))[7]);
                    //        periodlist.Add(troops);

                    //    }
                    //    break;
                    //case "NAME":
                    //    foreach (var obj in list)
                    //    {
                    //        TroopsReport troops = new TroopsReport();
                    //        troops.TroopsReportId = Convert.ToInt64(((object[])(obj))[0]);
                    //        troops.PeriodYear = Convert.ToString(((object[])(obj))[1]);
                    //        troops.Period = Convert.ToString(((object[])(obj))[2]);
                    //        troops.Sector = Convert.ToString(((object[])(obj))[3]);
                    //        troops.ContingentType = Convert.ToString(((object[])(obj))[4]);
                    //        troops.Location = Convert.ToString(((object[])(obj))[5]);
                    //        troops.Week = Convert.ToInt64(((object[])(obj))[6]);
                    //        troops.Name = Convert.ToString(((object[])(obj))[7]);
                    //        troops.SumofTroops = Convert.ToDecimal(((object[])(obj))[8]);
                    //        periodlist.Add(troops);

                    //    }
                    //    break;

                    //case "CONTINGENTTYPE":
                    //    foreach (var obj in list)
                    //    {
                    //        TroopsReport troops = new TroopsReport();
                    //        troops.TroopsReportId = Convert.ToInt64(((object[])(obj))[0]);
                    //        troops.ContingentType = Convert.ToString(((object[])(obj))[1]);
                    //        //troops.PeriodYear = Convert.ToString(((object[])(obj))[2]);
                    //        troops.SumofTroops = Convert.ToDecimal(((object[])(obj))[2]);
                    //        periodlist.Add(troops);

                    //    }
                    //    break;
                    //case "WEEK":
                    //    foreach (var obj in list)
                    //    {
                    //        TroopsReport troops = new TroopsReport();
                    //        troops.TroopsReportId = Convert.ToInt64(((object[])(obj))[0]);
                    //        troops.Week = Convert.ToInt64(((object[])(obj))[1]);
                    //        //troops.PeriodYear = Convert.ToString(((object[])(obj))[2]);
                    //        troops.SumofTroops = Convert.ToDecimal(((object[])(obj))[2]);
                    //        periodlist.Add(troops);

                    //    }
                    //    break;
                    default:
                        break;


                }

                return qtylist;
                // return PSF.ExecuteSql<samplestoredproceduure>(query);

            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Periodwise deduction chart

        public IList<PeriodWiseDeductionReport> GetPeriodWiseDeductionReportbyFlag(string Flag, string Criteria1,string Criteria2,string Criteria3)
        {
            try
            {

                string query = "Exec PeriodWiseDeductionReport_SP @spFlag='" + Flag;

                if (string.IsNullOrEmpty(Criteria1))
                    query = query + "', @spCriteria1=null";
                else
                    query = query + "', @spCriteria1='" + Criteria1 + "'";
                if (string.IsNullOrEmpty(Criteria2))
                    query = query + ", @spCriteria2=null";
                else
                    query = query + ", @spCriteria2='" + Criteria2 + "'";
                if (string.IsNullOrEmpty(Criteria3))
                    query = query + ", @spCriteria3=null";
                else
                    query = query + ", @spCriteria3='" + Criteria3 + "'";

                IList list = PSF.ExecuteSql(query);

                IList<PeriodWiseDeductionReport> periodlist = new List<PeriodWiseDeductionReport>();
                switch (Flag)
                {
                    case "PERIODYEAR":
                        foreach (var obj in list)
                        {
                            PeriodWiseDeductionReport deduction = new PeriodWiseDeductionReport();
                            deduction.Id = Convert.ToInt64(((object[])(obj))[0]);
                            deduction.PeriodYear = Convert.ToString(((object[])(obj))[1]);
                            deduction.APL_TimelyDelivery = Convert.ToDecimal(((object[])(obj))[2]);
                            deduction.APL_OrderbyLineItems = Convert.ToDecimal(((object[])(obj))[3]);
                            deduction.APL_OrdersbyWeight = Convert.ToDecimal(((object[])(obj))[4]);
                            deduction.APL_NoofAuthorizedSubstitutions = Convert.ToDecimal(((object[])(obj))[5]);
                            periodlist.Add(deduction);

                        }
                        break;
                    case "PERIOD":
                        foreach (var obj in list)
                        {
                            PeriodWiseDeductionReport deduction = new PeriodWiseDeductionReport();
                            deduction.Id = Convert.ToInt64(((object[])(obj))[0]);
                            deduction.PeriodYear = Convert.ToString(((object[])(obj))[1]);
                            deduction.Period = Convert.ToString(((object[])(obj))[2]);
                            deduction.APL_TimelyDelivery = Convert.ToDecimal(((object[])(obj))[3]);
                            deduction.APL_OrderbyLineItems = Convert.ToDecimal(((object[])(obj))[4]);
                            deduction.APL_OrdersbyWeight = Convert.ToDecimal(((object[])(obj))[5]);
                            deduction.APL_NoofAuthorizedSubstitutions = Convert.ToDecimal(((object[])(obj))[6]);
                            periodlist.Add(deduction);

                        }
                        break;
                         case "SECTOR":
                        foreach (var obj in list)
                        {
                            PeriodWiseDeductionReport deduction = new PeriodWiseDeductionReport();
                            deduction.Id = Convert.ToInt64(((object[])(obj))[0]);
                            deduction.PeriodYear = Convert.ToString(((object[])(obj))[1]);
                            deduction.Period = Convert.ToString(((object[])(obj))[2]);
                            deduction.Sector = Convert.ToString(((object[])(obj))[3]);
                            deduction.ContingentType = Convert.ToString(((object[])(obj))[4]);
                            deduction.APL_TimelyDelivery = Convert.ToDecimal(((object[])(obj))[5]);
                            deduction.APL_OrderbyLineItems = Convert.ToDecimal(((object[])(obj))[6]);
                            deduction.APL_OrdersbyWeight = Convert.ToDecimal(((object[])(obj))[7]);
                            deduction.APL_NoofAuthorizedSubstitutions = Convert.ToDecimal(((object[])(obj))[8]);
                            periodlist.Add(deduction);

                        }
                        break;


                    default:
                        break;


                }

                return periodlist;
                // return PSF.ExecuteSql<samplestoredproceduure>(query);

            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        #region Loss because of substitution report


        public Dictionary<long, IList<LossBecauseOfSubstitutions_vw>> GetLossBecauseOfSubstitutionListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<LossBecauseOfSubstitutions_vw>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion


        #region Substitution list

        public Dictionary<long, IList<AuthorizedUnAuthorizedSubstitutionList_VW>> GetConsolidatedSubstitutionReportsListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<AuthorizedUnAuthorizedSubstitutionList_VW>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion


        #region CMRTrendReport

        public IList<CMRTrendReport> GetCMRTrendByFlag(string Flag, string Criteria1, string Criteria2, string Criteria3,string Criteria4,string Criteria5,string Criteria6)
        {
            try
            {

                string query = "Exec CMRTrendReport_SP @spFlag='" + Flag;

                if (string.IsNullOrEmpty(Criteria1))
                    query = query + "', @spCriteria1=null";
                else
                    query = query + "', @spCriteria1='" + Criteria1 + "'";
                if (string.IsNullOrEmpty(Criteria2))
                    query = query + ", @spCriteria2=null";
                else
                    query = query + ", @spCriteria2='" + Criteria2 + "'";


                if (string.IsNullOrEmpty(Criteria3))
                    query = query + ", @spCriteria3=null";
                else
                    query = query + ", @spCriteria3='" + Criteria3 + "'";
                if (string.IsNullOrEmpty(Criteria4))
                    query = query + ", @spCriteria4=null";
                else
                    query = query + ", @spCriteria4='" + Criteria4 + "'";
                if (string.IsNullOrEmpty(Criteria5))
                    query = query + ", @spCriteria5=null";
                else
                    query = query + ", @spCriteria5='" + Criteria5 + "'";

                if (string.IsNullOrEmpty(Criteria6))
                    query = query + ", @spCriteria6=null";
                else
                    query = query + ", @spCriteria6='" + Criteria6 + "'";
              

                IList list = PSF.ExecuteSql(query);

                IList<CMRTrendReport> CMRTrendList = new List<CMRTrendReport>();
                switch (Flag)
                {
                    case "PERIODYEAR":
                        foreach (var obj in list)
                        {
                            CMRTrendReport cmrobj = new CMRTrendReport();
                            cmrobj.Id = Convert.ToInt64(((object[])(obj))[0]);
                            cmrobj.PeriodYear = Convert.ToString(((object[])(obj))[1]);
                            cmrobj.AuthorizedCMR = Convert.ToDecimal(((object[])(obj))[2]);
                            cmrobj.OrderCMR = Convert.ToDecimal(((object[])(obj))[3]);
                            cmrobj.AcceptedCMR = Convert.ToDecimal(((object[])(obj))[4]);



                            CMRTrendList.Add(cmrobj);

                        }
                        break;
                    case "PERIOD":
                        foreach (var obj in list)
                        {
                            CMRTrendReport cmrobj = new CMRTrendReport();
                            cmrobj.Id = Convert.ToInt64(((object[])(obj))[0]);
                            cmrobj.PeriodYear = Convert.ToString(((object[])(obj))[1]);
                            cmrobj.Period = Convert.ToString(((object[])(obj))[2]);
                            cmrobj.AuthorizedCMR = Convert.ToDecimal(((object[])(obj))[3]);
                            cmrobj.OrderCMR = Convert.ToDecimal(((object[])(obj))[4]);
                            cmrobj.AcceptedCMR = Convert.ToDecimal(((object[])(obj))[5]);
                            CMRTrendList.Add(cmrobj);

                        }
                        break;

                    case "SECTOR":
                        foreach (var obj in list)
                        {
                            CMRTrendReport cmrobj = new CMRTrendReport();
                            cmrobj.Id = Convert.ToInt64(((object[])(obj))[0]);
                            cmrobj.PeriodYear = Convert.ToString(((object[])(obj))[1]);
                            cmrobj.Period = Convert.ToString(((object[])(obj))[2]);
                            cmrobj.Sector = Convert.ToString(((object[])(obj))[3]);
                            cmrobj.ContingentType = Convert.ToString(((object[])(obj))[4]);

                            cmrobj.AuthorizedCMR = Convert.ToDecimal(((object[])(obj))[5]);
                            cmrobj.OrderCMR = Convert.ToDecimal(((object[])(obj))[6]);
                            cmrobj.AcceptedCMR = Convert.ToDecimal(((object[])(obj))[7]);
                            CMRTrendList.Add(cmrobj);

                        }
                        break;
                    case "LOCATION":
                        foreach (var obj in list)
                        {
                            CMRTrendReport cmrobj = new CMRTrendReport();
                            cmrobj.Id = Convert.ToInt64(((object[])(obj))[0]);
                            cmrobj.PeriodYear = Convert.ToString(((object[])(obj))[1]);
                            cmrobj.Period = Convert.ToString(((object[])(obj))[2]);
                            cmrobj.Sector = Convert.ToString(((object[])(obj))[3]);
                            cmrobj.ContingentType = Convert.ToString(((object[])(obj))[4]);
                            cmrobj.Location = Convert.ToString(((object[])(obj))[5]);

                            cmrobj.AuthorizedCMR = Convert.ToDecimal(((object[])(obj))[6]);
                            cmrobj.OrderCMR = Convert.ToDecimal(((object[])(obj))[7]);
                            cmrobj.AcceptedCMR = Convert.ToDecimal(((object[])(obj))[8]);
                            CMRTrendList.Add(cmrobj);

                        }
                        break;
                    case "CONTINGENT":
                        foreach (var obj in list)
                        {
                            CMRTrendReport cmrobj = new CMRTrendReport();
                            cmrobj.Id = Convert.ToInt64(((object[])(obj))[0]);
                            cmrobj.PeriodYear = Convert.ToString(((object[])(obj))[1]);
                            cmrobj.Period = Convert.ToString(((object[])(obj))[2]);
                            cmrobj.Sector = Convert.ToString(((object[])(obj))[3]);
                            cmrobj.ContingentType = Convert.ToString(((object[])(obj))[4]);
                            cmrobj.Location = Convert.ToString(((object[])(obj))[5]);
                            cmrobj.Name = Convert.ToString(((object[])(obj))[6]);
                            cmrobj.AuthorizedCMR = Convert.ToDecimal(((object[])(obj))[7]);
                            cmrobj.OrderCMR = Convert.ToDecimal(((object[])(obj))[8]);
                            cmrobj.AcceptedCMR = Convert.ToDecimal(((object[])(obj))[9]);
                            CMRTrendList.Add(cmrobj);

                        }
                        break;

                    case "WEEK":
                        foreach (var obj in list)
                        {
                            CMRTrendReport cmrobj = new CMRTrendReport();
                            cmrobj.Id = Convert.ToInt64(((object[])(obj))[0]);
                            cmrobj.PeriodYear = Convert.ToString(((object[])(obj))[1]);
                            cmrobj.Period = Convert.ToString(((object[])(obj))[2]);
                            cmrobj.Sector = Convert.ToString(((object[])(obj))[3]);
                            cmrobj.ContingentType = Convert.ToString(((object[])(obj))[4]);
                            cmrobj.Location = Convert.ToString(((object[])(obj))[5]);
                            cmrobj.Name = Convert.ToString(((object[])(obj))[6]);
                            cmrobj.Week = Convert.ToInt64(((object[])(obj))[7]);
                            cmrobj.AuthorizedCMR = Convert.ToDecimal(((object[])(obj))[8]);
                            cmrobj.OrderCMR = Convert.ToDecimal(((object[])(obj))[9]);
                            cmrobj.AcceptedCMR = Convert.ToDecimal(((object[])(obj))[10]);
                            CMRTrendList.Add(cmrobj);
                        }
                        break;
                    //case "WEEK":
                    //    foreach (var obj in list)
                    //    {
                    //        TroopsReport troops = new TroopsReport();
                    //        troops.TroopsReportId = Convert.ToInt64(((object[])(obj))[0]);
                    //        troops.Week = Convert.ToInt64(((object[])(obj))[1]);
                    //        //troops.PeriodYear = Convert.ToString(((object[])(obj))[2]);
                    //        troops.SumofTroops = Convert.ToDecimal(((object[])(obj))[2]);
                    //        periodlist.Add(troops);

                    //    }
                    //    break;
                    default:
                        break;


                }

                return CMRTrendList;
                // return PSF.ExecuteSql<samplestoredproceduure>(query);

            }
            catch (Exception)
            {
                throw;
            }
        }

        # endregion

        #region Homepage dashboard

        public IList<InsightHomePageDashboard> GetHomePageDashboardByFlag(string Flag)
        {
            try
            {

                string query = "Exec InsightHomePageDashboard @spFlag='" + Flag+"'";

                

                IList list = PSF.ExecuteSql(query);

                IList<InsightHomePageDashboard> dashboard = new List<InsightHomePageDashboard>();
                switch (Flag)
                {
                    case "INVPROCESSED":
                        foreach (var obj in list)
                        {
                            InsightHomePageDashboard dashboardobj = new InsightHomePageDashboard();
                            dashboardobj.Id = Convert.ToInt64(((object[])(obj))[0]);
                            dashboardobj.Category = Convert.ToString(((object[])(obj))[1]);
                            dashboardobj.PeriodYear = Convert.ToString(((object[])(obj))[2]);
                            dashboardobj.NoOfInvoices = Convert.ToInt64(((object[])(obj))[3]);
                            dashboard.Add(dashboardobj);

                        }
                        break;
                    case "INVVALUE":
                        foreach (var obj in list)
                        {
                            InsightHomePageDashboard dashboardobj = new InsightHomePageDashboard();
                            dashboardobj.Id = Convert.ToInt64(((object[])(obj))[0]);
                            dashboardobj.Category = Convert.ToString(((object[])(obj))[1]);
                            dashboardobj.PeriodYear = Convert.ToString(((object[])(obj))[2]);
                            dashboardobj.InvoiceValue = Convert.ToDecimal(((object[])(obj))[3]);
                            dashboard.Add(dashboardobj);

                        }
                        break;

                    case "CONTINGENTCOUNT":
                        foreach (var obj in list)
                        {
                            InsightHomePageDashboard dashboardobj = new InsightHomePageDashboard();
                            dashboardobj.Id = Convert.ToInt64(((object[])(obj))[0]);
                            dashboardobj.PeriodYear = Convert.ToString(((object[])(obj))[1]);
                            dashboardobj.NoOfContingents = Convert.ToInt64(((object[])(obj))[2]);
                            dashboard.Add(dashboardobj);

                        }
                        break;
                    case "TROOPS":
                        foreach (var obj in list)
                        {
                            InsightHomePageDashboard dashboardobj = new InsightHomePageDashboard();
                            dashboardobj.Id = Convert.ToInt64(((object[])(obj))[0]);
                            dashboardobj.PeriodYear = Convert.ToString(((object[])(obj))[1]);
                            dashboardobj.TroopStrength= Convert.ToInt64(((object[])(obj))[2]);
                            dashboard.Add(dashboardobj);
                        }
                        break;
                    //case "CONTINGENT":
                    //    foreach (var obj in list)
                    //    {
                    //        CMRTrendReport cmrobj = new CMRTrendReport();
                    //        cmrobj.Id = Convert.ToInt64(((object[])(obj))[0]);
                    //        cmrobj.PeriodYear = Convert.ToString(((object[])(obj))[1]);
                    //        cmrobj.Period = Convert.ToString(((object[])(obj))[2]);
                    //        cmrobj.Sector = Convert.ToString(((object[])(obj))[3]);
                    //        cmrobj.ContingentType = Convert.ToString(((object[])(obj))[4]);
                    //        cmrobj.Location = Convert.ToString(((object[])(obj))[5]);
                    //        cmrobj.Name = Convert.ToString(((object[])(obj))[6]);
                    //        cmrobj.AuthorizedCMR = Convert.ToDecimal(((object[])(obj))[7]);
                    //        cmrobj.OrderCMR = Convert.ToDecimal(((object[])(obj))[8]);
                    //        cmrobj.AcceptedCMR = Convert.ToDecimal(((object[])(obj))[9]);
                    //        CMRTrendList.Add(cmrobj);

                    //    }
                    //    break;

                    //case "WEEK":
                    //    foreach (var obj in list)
                    //    {
                    //        CMRTrendReport cmrobj = new CMRTrendReport();
                    //        cmrobj.Id = Convert.ToInt64(((object[])(obj))[0]);
                    //        cmrobj.PeriodYear = Convert.ToString(((object[])(obj))[1]);
                    //        cmrobj.Period = Convert.ToString(((object[])(obj))[2]);
                    //        cmrobj.Sector = Convert.ToString(((object[])(obj))[3]);
                    //        cmrobj.ContingentType = Convert.ToString(((object[])(obj))[4]);
                    //        cmrobj.Location = Convert.ToString(((object[])(obj))[5]);
                    //        cmrobj.Name = Convert.ToString(((object[])(obj))[6]);
                    //        cmrobj.Week = Convert.ToInt64(((object[])(obj))[7]);
                    //        cmrobj.AuthorizedCMR = Convert.ToDecimal(((object[])(obj))[8]);
                    //        cmrobj.OrderCMR = Convert.ToDecimal(((object[])(obj))[9]);
                    //        cmrobj.AcceptedCMR = Convert.ToDecimal(((object[])(obj))[10]);
                    //        CMRTrendList.Add(cmrobj);
                    //    }
                    //    break;
                    //case "WEEK":
                    //    foreach (var obj in list)
                    //    {
                    //        TroopsReport troops = new TroopsReport();
                    //        troops.TroopsReportId = Convert.ToInt64(((object[])(obj))[0]);
                    //        troops.Week = Convert.ToInt64(((object[])(obj))[1]);
                    //        //troops.PeriodYear = Convert.ToString(((object[])(obj))[2]);
                    //        troops.SumofTroops = Convert.ToDecimal(((object[])(obj))[2]);
                    //        periodlist.Add(troops);

                    //    }
                    //    break;
                    default:
                        break;


                }

                return dashboard;
                // return PSF.ExecuteSql<samplestoredproceduure>(query);

            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Loss because of excess delivery

        public Dictionary<long, IList<LossBecauseofExcessDelivery>> GetLossBecauseofExcessDeliveryListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<LossBecauseofExcessDelivery>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public IList<LossBecauseofExcessDelivery> GetLossBecauseofExcessDeliveryReportbyFlag(string Flag, string Criteria1, string Criteria2, string Criteria3, string Criteria4, string Criteria5, string Criteria6)
        {
            try
            {

                string query = "Exec LossBecauseofExcessDelivery_SP @spFlag='" + Flag;

                if (string.IsNullOrEmpty(Criteria1))
                    query = query + "', @spCriteria1=null";
                else
                    query = query + "', @spCriteria1='" + Criteria1 + "'";
                if (string.IsNullOrEmpty(Criteria2))
                    query = query + ", @spCriteria2=null";
                else
                    query = query + ", @spCriteria2='" + Criteria2 + "'";


                if (string.IsNullOrEmpty(Criteria3))
                    query = query + ", @spCriteria3=null";
                else
                    query = query + ", @spCriteria3='" + Criteria3 + "'";
                if (string.IsNullOrEmpty(Criteria4))
                    query = query + ", @spCriteria4=null";
                else
                    query = query + ", @spCriteria4='" + Criteria4 + "'";
                if (string.IsNullOrEmpty(Criteria5))
                    query = query + ", @spCriteria5=null";
                else
                    query = query + ", @spCriteria5='" + Criteria5 + "'";

                if (string.IsNullOrEmpty(Criteria6))
                    query = query + ", @spCriteria6=null";
                else
                    query = query + ", @spCriteria6='" + Criteria6 + "'";
              

                IList list = PSF.ExecuteSql(query);

                IList<LossBecauseofExcessDelivery> LossList = new List<LossBecauseofExcessDelivery>();
                switch (Flag)
                {
                    case "PERIODYEAR":
                        foreach (var obj in list)
                        {
                            LossBecauseofExcessDelivery lossobj = new LossBecauseofExcessDelivery();
                            lossobj.Id = Convert.ToInt64(((object[])(obj))[0]);
                            lossobj.PeriodYear = Convert.ToString(((object[])(obj))[1]);
                            lossobj.AmountOfLoss = Convert.ToDecimal(((object[])(obj))[2]);
                            



                            LossList.Add(lossobj);

                        }
                        break;
                    case "PERIOD":
                        foreach (var obj in list)
                        {
                            LossBecauseofExcessDelivery lossobj = new LossBecauseofExcessDelivery();
                            lossobj.Id = Convert.ToInt64(((object[])(obj))[0]);
                            lossobj.PeriodYear = Convert.ToString(((object[])(obj))[1]);
                            lossobj.Period = Convert.ToString(((object[])(obj))[2]);
                            lossobj.AmountOfLoss = Convert.ToDecimal(((object[])(obj))[3]);
                           
                            LossList.Add(lossobj);

                        }
                        break;

                    case "SECTOR":
                        foreach (var obj in list)
                        {
                            LossBecauseofExcessDelivery lossobj = new LossBecauseofExcessDelivery();
                            lossobj.Id = Convert.ToInt64(((object[])(obj))[0]);
                            lossobj.PeriodYear = Convert.ToString(((object[])(obj))[1]);
                            lossobj.Period = Convert.ToString(((object[])(obj))[2]);
                            lossobj.Sector = Convert.ToString(((object[])(obj))[3]);
                            lossobj.ContingentType = Convert.ToString(((object[])(obj))[4]);

                            lossobj.AmountOfLoss = Convert.ToDecimal(((object[])(obj))[5]);
                            
                            LossList.Add(lossobj);

                        }
                        break;
                    case "LOCATION":
                        foreach (var obj in list)
                        {
                            LossBecauseofExcessDelivery lossobj = new LossBecauseofExcessDelivery();
                            lossobj.Id = Convert.ToInt64(((object[])(obj))[0]);
                            lossobj.PeriodYear = Convert.ToString(((object[])(obj))[1]);
                            lossobj.Period = Convert.ToString(((object[])(obj))[2]);
                            lossobj.Sector = Convert.ToString(((object[])(obj))[3]);
                            lossobj.ContingentType = Convert.ToString(((object[])(obj))[4]);
                            lossobj.Location = Convert.ToString(((object[])(obj))[5]);

                            lossobj.AmountOfLoss = Convert.ToDecimal(((object[])(obj))[6]);
                            
                            LossList.Add(lossobj);

                        }
                        break;
                    case "CONTINGENT":
                        foreach (var obj in list)
                        {
                            LossBecauseofExcessDelivery lossobj = new LossBecauseofExcessDelivery();
                            lossobj.Id = Convert.ToInt64(((object[])(obj))[0]);
                            lossobj.PeriodYear = Convert.ToString(((object[])(obj))[1]);
                            lossobj.Period = Convert.ToString(((object[])(obj))[2]);
                            lossobj.Sector = Convert.ToString(((object[])(obj))[3]);
                            lossobj.ContingentType = Convert.ToString(((object[])(obj))[4]);
                            lossobj.Location = Convert.ToString(((object[])(obj))[5]);
                            lossobj.Name = Convert.ToString(((object[])(obj))[6]);
                            lossobj.AmountOfLoss = Convert.ToDecimal(((object[])(obj))[7]);

                            LossList.Add(lossobj);

                        }
                        break;

                    case "WEEK":
                        foreach (var obj in list)
                        {
                            LossBecauseofExcessDelivery lossobj = new LossBecauseofExcessDelivery();
                            lossobj.Id = Convert.ToInt64(((object[])(obj))[0]);
                            lossobj.PeriodYear = Convert.ToString(((object[])(obj))[1]);
                            lossobj.Period = Convert.ToString(((object[])(obj))[2]);
                            lossobj.Sector = Convert.ToString(((object[])(obj))[3]);
                            lossobj.ContingentType = Convert.ToString(((object[])(obj))[4]);
                            lossobj.Location = Convert.ToString(((object[])(obj))[5]);
                            lossobj.Name = Convert.ToString(((object[])(obj))[6]);
                            lossobj.Week = Convert.ToInt64(((object[])(obj))[7]);
                            lossobj.AmountOfLoss = Convert.ToDecimal(((object[])(obj))[8]);
                            LossList.Add(lossobj);
                        }
                        break;
                    //case "WEEK":
                    //    foreach (var obj in list)
                    //    {
                    //        TroopsReport troops = new TroopsReport();
                    //        troops.TroopsReportId = Convert.ToInt64(((object[])(obj))[0]);
                    //        troops.Week = Convert.ToInt64(((object[])(obj))[1]);
                    //        //troops.PeriodYear = Convert.ToString(((object[])(obj))[2]);
                    //        troops.SumofTroops = Convert.ToDecimal(((object[])(obj))[2]);
                    //        periodlist.Add(troops);

                    //    }
                    //    break;
                    default:
                        break;


                }

                return LossList;
                // return PSF.ExecuteSql<samplestoredproceduure>(query);

            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
