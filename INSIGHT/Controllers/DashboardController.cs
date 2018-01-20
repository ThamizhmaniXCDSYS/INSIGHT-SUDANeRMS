using INSIGHT.Entities;
using INSIGHT.Entities.DashboardEntities;
using INSIGHT.Entities.InvoiceEntities;
using INSIGHT.WCFServices;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace INSIGHT.Controllers
{

    public class DashboardController : BaseController
    {
        // GET: /Dashboard/
        IFormatProvider provider = new System.Globalization.CultureInfo("en-IN", true);
        OrdersService orderService = new OrdersService();
        InvoiceService IS = new InvoiceService();
        DashboardService DashboardService = new DashboardService();
        #region old reports with graph

        #region TroopsReport
        public ActionResult TroopsReport1()
        {
            return View();
        }
        //public JsonResult GetTroopsReport_PeriodYearJQGrid(string PeriodYear, string Period, string Flag, string Criteria, int rows, string sidx, string sord, int? page = 1)
        //{
        //    IList<TroopsReport> troopsreport = new List<TroopsReport>();

        //    long totalrecords = 0;
        //    int totalPages = 0;
        //    DateTime s = DateTime.Now;
        //    troopsreport = DashboardService.GetTroopsReportbyFlag(PeriodYear, Period, Flag, Criteria);
        //    if (troopsreport != null && troopsreport.Count > 0)
        //    {
        //        switch (Flag)
        //        {
        //            case "PERIODYEAR":

        //                totalrecords = troopsreport.Count;
        //                totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
        //                var jsondat = new
        //                {
        //                    total = totalPages,
        //                    page = page,
        //                    records = totalrecords,

        //                    rows = (from items in troopsreport
        //                            select new
        //                            {
        //                                i = 2,
        //                                cell = new string[] {
        //                                items.TroopsReportId.ToString(),
        //                                items.PeriodYear,
        //                                items.SumofTroops.ToString()
        //                    }
        //                            })
        //                };
        //                return Json(jsondat, JsonRequestBehavior.AllowGet);


        //            case "PERIOD":
        //                totalrecords = troopsreport.Count;
        //                totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
        //                jsondat = new
        //                {
        //                    total = totalPages,
        //                    page = page,
        //                    records = totalrecords,

        //                    rows = (from items in troopsreport
        //                            select new
        //                            {
        //                                i = 2,
        //                                cell = new string[] {
        //                                items.TroopsReportId.ToString(),
        //                                items.Period,

        //                                items.SumofTroops.ToString()
        //                    }
        //                            })
        //                };
        //                return Json(jsondat, JsonRequestBehavior.AllowGet);
        //            case "SECTOR":
        //                totalrecords = troopsreport.Count;
        //                totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
        //                jsondat = new
        //                {
        //                    total = totalPages,
        //                    page = page,
        //                    records = totalrecords,

        //                    rows = (from items in troopsreport
        //                            select new
        //                            {
        //                                i = 2,
        //                                cell = new string[] {
        //                                items.TroopsReportId.ToString(),
        //                                items.Sector,
        //                                items.SumofTroops.ToString()
        //                    }
        //                            })
        //                };
        //                return Json(jsondat, JsonRequestBehavior.AllowGet);
        //            case "LOCATION":
        //                totalrecords = troopsreport.Count;
        //                totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
        //                jsondat = new
        //                {
        //                    total = totalPages,
        //                    page = page,
        //                    records = totalrecords,

        //                    rows = (from items in troopsreport
        //                            select new
        //                            {
        //                                i = 2,
        //                                cell = new string[] {
        //                                items.TroopsReportId.ToString(),
        //                                items.Location,
        //                                items.SumofTroops.ToString()
        //                    }
        //                            })
        //                };
        //                return Json(jsondat, JsonRequestBehavior.AllowGet);

        //            case "NAME":
        //                totalrecords = troopsreport.Count;
        //                totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
        //                jsondat = new
        //                {
        //                    total = totalPages,
        //                    page = page,
        //                    records = totalrecords,

        //                    rows = (from items in troopsreport
        //                            select new
        //                            {
        //                                i = 2,
        //                                cell = new string[] {
        //                                items.TroopsReportId.ToString(),
        //                                items.Name,
        //                                items.SumofTroops.ToString()
        //                    }
        //                            })
        //                };
        //                return Json(jsondat, JsonRequestBehavior.AllowGet);
        //            case "CONTINGENTTYPE":
        //                totalrecords = troopsreport.Count;
        //                totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
        //                jsondat = new
        //                {
        //                    total = totalPages,
        //                    page = page,
        //                    records = totalrecords,

        //                    rows = (from items in troopsreport
        //                            select new
        //                            {
        //                                i = 2,
        //                                cell = new string[] {
        //                                items.TroopsReportId.ToString(),
        //                                items.ContingentType,
        //                                items.SumofTroops.ToString()
        //                    }
        //                            })
        //                };
        //                return Json(jsondat, JsonRequestBehavior.AllowGet);
        //            case "WEEK":
        //                totalrecords = troopsreport.Count;
        //                totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
        //                jsondat = new
        //                {
        //                    total = totalPages,
        //                    page = page,
        //                    records = totalrecords,

        //                    rows = (from items in troopsreport
        //                            select new
        //                            {
        //                                i = 2,
        //                                cell = new string[] {
        //                                items.TroopsReportId.ToString(),
        //                                items.Week.ToString(),
        //                                items.SumofTroops.ToString()
        //                    }
        //                            })
        //                };
        //                return Json(jsondat, JsonRequestBehavior.AllowGet);
        //            default:
        //                return Json(null, JsonRequestBehavior.AllowGet);

        //        }



        //    }
        //    else
        //    {
        //        return Json(null, JsonRequestBehavior.AllowGet);
        //    }
        //}
        #endregion

        //#region  Periodwise graph for orderqty, deliveredqty,invoiceqty,invoice value

        //public ActionResult PeriodWiseQtyReport1()
        //{
        //    return View();
        //}

        //public JsonResult PeriodWiseQtyDashboardReport(string Flag, string PeriodYear, int rows, string sidx, string sord, int? page = 1)
        //{
        //    try
        //    {

        //        Dictionary<string, object> criteria = new Dictionary<string, object>();

        //        if (!string.IsNullOrWhiteSpace(sord) && sord == "desc")
        //            sord = "Desc";
        //        else
        //            sord = "Asc";
        //        long totalrecords;
        //        long totalPages;


        //        if (!string.IsNullOrWhiteSpace(PeriodYear)) { criteria.Add("PeriodYear", PeriodYear); }
        //        Dictionary<long, IList<PeriodWiseValuesDashboard_vw>> Qtylist = DashboardService.GetPeriodWiseQtyListWithPagingAndCriteria(0, 999999, string.Empty, string.Empty, criteria);
        //        switch (Flag)
        //        {
        //            case "PERIODYEAR":
        //                IList<PeriodWiseValuesDashboard_vw> QtyIList = Qtylist.FirstOrDefault().Value.ToList();
        //                var Periodyear = (from u in QtyIList select new { u.PeriodYear }).Distinct().ToList();
        //                totalrecords = Periodyear.Count;
        //                totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
        //                var jsondat = new
        //                {
        //                    total = totalPages,
        //                    page = page,
        //                    records = totalrecords,

        //                    rows = (from items in Periodyear
        //                            select new
        //                            {
        //                                i = 2,
        //                                cell = new string[] {

        //                                items.PeriodYear,

        //                    }
        //                            })
        //                };
        //                return Json(jsondat, JsonRequestBehavior.AllowGet);
        //            case "PERIOD":

        //                totalrecords = Qtylist.Count;
        //                totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
        //                var jsondat1 = new
        //                {
        //                    total = totalPages,
        //                    page = page,
        //                    records = totalrecords,

        //                    rows = (from items in Qtylist.FirstOrDefault().Value
        //                            select new
        //                            {
        //                                i = 2,
        //                                cell = new string[] {
        //                                    items.Id.ToString(),
        //                                    items.PeriodYear,
        //                                    items.Period,
        //                                    items.OrderQty.ToString(),
        //                                    items.DeliveredQty.ToString(),
        //                                    items.InvoiceQty.ToString(),
        //                                    items.InvoiceValue.ToString()


        //                    }
        //                            })
        //                };
        //                return Json(jsondat1, JsonRequestBehavior.AllowGet);
        //            default:
        //                return Json(null, JsonRequestBehavior.AllowGet);


        //        }





        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionPolicy.HandleException(ex, "InsightDNPolicy");
        //        throw ex;
        //    }
        //}




        //#endregion


        #region  PeriodWise deduction chart

        public ActionResult PeriodWiseDeductionReport1()
        {


            return View();
        }

        //public JsonResult PeriodWiseDeductionReportJQGrid(string Flag, string Criteria, int rows, string sidx, string sord, int? page = 1)
        //{
        //    IList<PeriodWiseDeductionReport> deductionreport = new List<PeriodWiseDeductionReport>();

        //    long totalrecords = 0;
        //    int totalPages = 0;
        //    DateTime s = DateTime.Now;
        //    deductionreport = DashboardService.GetPeriodWiseDeductionReportbyFlag(Flag, Criteria);
        //    if (deductionreport != null && deductionreport.Count > 0)
        //    {
        //        switch (Flag)
        //        {
        //            case "PERIODYEAR":

        //                totalrecords = deductionreport.Count;
        //                totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
        //                var jsondat = new
        //                {
        //                    total = totalPages,
        //                    page = page,
        //                    records = totalrecords,

        //                    rows = (from items in deductionreport
        //                            select new
        //                            {
        //                                i = 2,
        //                                cell = new string[] {
        //                                items.Id.ToString(),
        //                                items.PeriodYear,
        //                                items.APL_TimelyDelivery.ToString(),
        //                                items.APL_OrderbyLineItems.ToString(),
        //                                items.APL_OrdersbyWeight.ToString(),
        //                                items.APL_NoofAuthorizedSubstitutions.ToString()

        //                    }
        //                            })
        //                };
        //                return Json(jsondat, JsonRequestBehavior.AllowGet);


        //            case "PERIOD":
        //                totalrecords = deductionreport.Count;
        //                totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
        //                jsondat = new
        //                {
        //                    total = totalPages,
        //                    page = page,
        //                    records = totalrecords,

        //                    rows = (from items in deductionreport
        //                            select new
        //                            {
        //                                i = 2,
        //                                cell = new string[] {
        //                                items.Id.ToString(),
        //                                items.Period,
        //                                items.APL_TimelyDelivery.ToString(),
        //                                items.APL_OrderbyLineItems.ToString(),
        //                                items.APL_OrdersbyWeight.ToString(),
        //                                items.APL_NoofAuthorizedSubstitutions.ToString()
        //                    }
        //                            })
        //                };
        //                return Json(jsondat, JsonRequestBehavior.AllowGet);

        //            default:
        //                return Json(null, JsonRequestBehavior.AllowGet);

        //        }



        //    }
        //    else
        //    {
        //        return Json(null, JsonRequestBehavior.AllowGet);
        //    }
        //}

        #endregion
        #endregion


        #region new reports without graph
        #region troops report new
        public ActionResult troopsReport()
        {
            return View();
        }

        public ActionResult TroopsReportJQGrid(string searchItems, string ExptType, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                OrdersService us = new OrdersService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();

                if (!string.IsNullOrWhiteSpace(sord) && sord == "desc")
                    sord = "Desc";
                else
                    sord = "Asc";



                if (string.IsNullOrWhiteSpace(searchItems))
                {

                    return null;
                }
                else
                {

                    #region Desouza

                    if (searchItems != null && searchItems != "")
                    {
                        var Items = searchItems.ToString().Split(',');
                        if (!string.IsNullOrWhiteSpace(Items[0])) { criteria.Add("Sector", Items[0]); }
                        if (!string.IsNullOrWhiteSpace(Items[1])) { criteria.Add("Location", Items[1]); }
                        if (!string.IsNullOrWhiteSpace(Items[2])) { criteria.Add("ContingentType", Items[2]); }
                        if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Name", Items[3]); }
                        if (!string.IsNullOrWhiteSpace(Items[4])) { criteria.Add("PeriodYear", Items[4]); }
                        if (!string.IsNullOrWhiteSpace(Items[5])) { criteria.Add("Period", Items[5]); }
                        if (!string.IsNullOrWhiteSpace(Items[6])) { criteria.Add("Week", Convert.ToInt64(Items[6])); }
                    }

                    #endregion

                    Dictionary<long, IList<Orders>> troopslist1 = us.GetOrdersListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                    IList<Orders> troopslist = troopslist1.FirstOrDefault().Value.ToList();
                    if (troopslist != null && troopslist.Count > 0)
                    {
                        if (ExptType == "Excel")
                        {

                            int i = 1;
                            foreach (var item in troopslist)
                            {
                                item.OrderId = i;
                                i = i + 1;
                            }
                            var List = troopslist;
                            List<string> lstHeader = new List<string>() { "ControlId", "Sector", "Location", "Contingent Type", "Contingent", "Period Year", "Period", "Week", "Troops" };
                            base.NewExportToExcel(List, "Troops Report", (item => new
                            {

                                ControlId = item.ControlId,
                                Sector = item.Sector,
                                Location = item.Location,
                                ContingentType = item.ContingentType,
                                Contingent = item.Name,
                                PeriodYear = item.PeriodYear,
                                Period = item.Period,
                                Week = item.Week,
                                Troops = item.Troops

                            }), lstHeader);
                            return new EmptyResult();


                        }
                        else
                        {
                            long totalrecords = troopslist.Count;
                            int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                            var jsondat = new
                            {
                                total = totalPages,
                                page = page,
                                records = totalrecords,

                                rows = (from items in troopslist
                                        select new
                                        {
                                            i = 2,
                                            cell = new string[] {

                                            items.OrderId.ToString(),
                                            items.ControlId.ToString(),
                                            items.Sector,
                                            items.Location,
                                            items.ContingentType,
                                            items.Name,
                                            items.PeriodYear,
                                            items.Period,
                                            items.Week.ToString(),
                                            items.Troops.ToString()
                                            
                                            }
                                        })
                            };
                            return Json(jsondat, JsonRequestBehavior.AllowGet);

                        }

                    }
                    return null;

                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }

        //Troops report charts
        public ActionResult GetPeriodYearGraph_TroopsChart(string Flag, string Criteria, string period, string PriodYear, string Sector, string Location, string ContingentType)
        {
            IList<TroopsReport> troopsreport = new List<TroopsReport>();
            troopsreport = DashboardService.GetTroopsReportbyFlag(string.Empty, string.Empty, Sector, ContingentType, Location, Flag, Criteria);
            var PeriodYearGraph = " <graph caption='Period Year based Troop Strength  ' xAxisName='Period Year' forceDecimals='0'  rotateLabels='1' formatNumberScale='0'   yAxisName='' animation='1' showNames='1' showValues='1'  divlinecolor='d3d3d3' distance='6'   rotateNames='0'>";
            string[] color = new string[12] { "#0E426C", "FB9C6C", "e44a00", "52D017", "FFA62F", "E66C2C", "C25A7C", "59E817", "00FF00", "FFA62F", "FF7F50", "FF00FF" };

            int i = 0;
            foreach (var year in troopsreport)
            {

                // PeriodYearGraph = PeriodYearGraph + " <set name='2013-2014' value='" + Period13to14 + "' color='8B008B' link=\"JavaScript: MCClickEvent('"+a+"');"+"\" />";
                PeriodYearGraph = PeriodYearGraph + " <set name='" + year.PeriodYear + "' value='" + year.SumofTroops + "' color='" + color[i] + "' link=\"JavaScript: MCClickEvent('" + year.PeriodYear + "');" + "\" />";
                i = i + 1;
            }
            PeriodYearGraph = PeriodYearGraph + "</graph>";
            Response.Write(PeriodYearGraph);

            return null;
        }

        //Period year Graph

        public ActionResult GetPeriodGraph_Troops(string PeriodYear, string Flag, string Sector, string ContingentType, string Location)
        {
            //string PeriodYear = "13-14";
            IList<TroopsReport> troopsreport = new List<TroopsReport>();
            troopsreport = DashboardService.GetTroopsReportbyFlag(string.Empty, string.Empty, Sector, ContingentType, Location, Flag, PeriodYear);
            var PeriodYearGraph = " <graph caption='Period based Troop Strength' xAxisName='Period' forceDecimals='0'  rotateLabels='1' formatNumberScale='0'   yAxisName='' animation='1' showNames='1' showValues='1'  divlinecolor='d3d3d3' distance='6'   rotateNames='0'>";
            string[] color = new string[12] { "#0E426C", "#6586A7", "#FFC652", "FB9C6C", "EAFBC5", "D55121", "CFE990", "E66C2C", "E4287C", "#83AE84", "#5F021F", "#546F8B" };
            int i = 0;
            foreach (var Period in troopsreport)
            {

                // PeriodYearGraph = PeriodYearGraph + " <set name='2013-2014' value='" + Period13to14 + "' color='8B008B' link=\"JavaScript: MCClickEvent('"+a+"');"+"\" />";
                PeriodYearGraph = PeriodYearGraph + " <set name='" + Period.Period + "' value='" + Period.SumofTroops + "' color='" + color[i] + "' link=\"JavaScript: PeriodSelection('" + Period.PeriodYear + "," + Period.Period + "');" + "\" />";
                i = i + 1;
            }
            PeriodYearGraph = PeriodYearGraph + "</graph>";
            Response.Write(PeriodYearGraph);
            return null;
        }

        //Sector Graph

        public ActionResult GetSectorGraph_Troops(string PeriodYear, string Period, string Flag, string Sector, string ContingentType, string Location)
        {
            IList<TroopsReport> troopsreport = new List<TroopsReport>();
            troopsreport = DashboardService.GetTroopsReportbyFlag(PeriodYear, Period, string.Empty, string.Empty, Location, Flag, string.Empty);

            var FPU = (from u in troopsreport where u.ContingentType == "FPU" select new { u.ContingentType, u.SumofTroops, u.PeriodYear, u.Period, u.Sector }).ToArray();
            var MIL = (from u in troopsreport where u.ContingentType == "MIL" select new { u.ContingentType, u.SumofTroops, u.PeriodYear, u.Period, u.Sector }).ToArray();
            var SectorArray = (from u in troopsreport select new { u.Sector }).Distinct().ToArray();

            var SectorGraph = " <graph caption='Sector based Troops Strength ' xAxisName='Sector' forceDecimals='0' divlineisdashed='1' formatNumberScale='0'   yAxisName='' animation='1' showNames='1' showValues='1'  divlinecolor='d3d3d3' distance='6' angle='45'  rotateNames='0'>";
            SectorGraph = SectorGraph + "<categories>";
            foreach (var Sec in SectorArray)
            {
                SectorGraph = SectorGraph + "<category name='" + Sec.Sector + "' />";
            }

            SectorGraph = SectorGraph + "</categories>";

            SectorGraph = SectorGraph + " <dataset seriesname='FPU' color='#A3C586'>";
            foreach (var fpu in FPU)
            {
                SectorGraph = SectorGraph + "<set value='" + fpu.SumofTroops + "' link=\"JavaScript: SectorConttypeSelection('" + fpu.PeriodYear + "," + fpu.Period + "," + fpu.Sector + "," + fpu.ContingentType + "');" + "\" />";
            }

            SectorGraph = SectorGraph + "</dataset>";

            SectorGraph = SectorGraph + " <dataset seriesname='MIL' color='#FFCC33'>";
            foreach (var mil in MIL)
            {
                SectorGraph = SectorGraph + "<set value='" + mil.SumofTroops + "' link=\"JavaScript: SectorConttypeSelection('" + mil.PeriodYear + "," + mil.Period + "," + mil.Sector + "," + mil.ContingentType + "');" + "\" />";

            }

            SectorGraph = SectorGraph + "</dataset>";
            SectorGraph = SectorGraph + "</graph>";
            Response.Write(SectorGraph);

            return null;
        }

        //Location Graph

        public ActionResult GetLocationWeekGraph_Troops(string PeriodYear, string Period, string Sector, string ContingentType, string Flag, string Location1)
        {
            IList<TroopsReport> troopsreport = new List<TroopsReport>();
            troopsreport = DashboardService.GetTroopsReportbyFlag(PeriodYear, Period, Sector, ContingentType, Location1, Flag, string.Empty);
            var Location = (from u in troopsreport select new { u.PeriodYear, u.Period, u.Sector, u.Location }).Distinct().ToArray();

            decimal troopstrength = 0;

            //IList<TroopsReport_LocationGraph> ConsolidatedList = new List<TroopsReport_LocationGraph>();

            IList<TroopsReport_LocationGraph> loclist = new List<TroopsReport_LocationGraph>();

            foreach (var item in Location)
            {
                decimal Week1 = 0; decimal Week2 = 0; decimal Week3 = 0; decimal Week4 = 0;

                var te1 = (from u in troopsreport
                           where u.Location == item.Location && u.rank == 1
                           select u.SumofTroops).ToList();
                Week1 = (te1.Count != 0) ? te1[0] : 0;

                var te2 = (from u in troopsreport
                           where u.Location == item.Location && u.rank == 2
                           select u.SumofTroops).ToList();
                Week2 = (te2.Count != 0) ? te2[0] : 0;
                var te3 = (from u in troopsreport
                           where u.Location == item.Location && u.rank == 3
                           select u.SumofTroops).ToList();
                Week3 = (te3.Count != 0) ? te3[0] : 0;
                var te4 = (from u in troopsreport
                           where u.Location == item.Location && u.rank == 4
                           select u.SumofTroops).ToList();
                Week4 = (te4.Count != 0) ? te4[0] : 0;

                TroopsReport_LocationGraph Locgp = new TroopsReport_LocationGraph { Location = item.Location, Troops1 = Week1, Troops2 = Week2, Troops3 = Week3, Troops4 = Week4 };

                loclist.Add(Locgp);



                // ConsolidatedList= (from u in troopsreport where u.PeriodYear==item.PeriodYear && u.Period==item.Period && u.Sector==item.Sector && u.Location==item.Location select u);
            }

            var Week = (from u in troopsreport select new { u.Week }).Distinct().ToArray();

            var LocationWeekGraph = " <graph caption='Location and Week Troops strength ' xAxisName='Location' forceDecimals='0' divlineisdashed='1' formatNumberScale='0'   yAxisName='' animation='1' showNames='1' showValues='0'  divlinecolor='d3d3d3' distance='6' angle='45'  rotateNames='1'>";
            LocationWeekGraph = LocationWeekGraph + "<categories>";
            foreach (var item in Location)
            {
                LocationWeekGraph = LocationWeekGraph + "<category name='" + item.Location + "' />";
            }

            LocationWeekGraph = LocationWeekGraph + "</categories>";

            if (Week.Length >= 1)
            {

                LocationWeekGraph = LocationWeekGraph + " <dataset seriesname='Week " + Week[0].Week + "' color='3399FF'>";
                foreach (var item in loclist)
                {


                    LocationWeekGraph = LocationWeekGraph + "<set value='" + item.Troops1 + "' link=\"JavaScript: LocationWeekSelection('" + PeriodYear + "," + Period + "," + Sector + "," + ContingentType + "," + item.Location + "," + Week[0].Week + "');" + "\" />";

                }
            }

            LocationWeekGraph = LocationWeekGraph + "</dataset>";
            if (Week.Length >= 2)
            {

                LocationWeekGraph = LocationWeekGraph + " <dataset seriesname='Week " + Week[1].Week + "' color='8C489F'>";
                foreach (var item in loclist)
                {
                    LocationWeekGraph = LocationWeekGraph + "<set value='" + item.Troops2 + "'link=\"JavaScript: LocationWeekSelection('" + PeriodYear + "," + Period + "," + Sector + "," + ContingentType + "," + item.Location + "," + Week[1].Week + "');" + "\" />";
                }
                LocationWeekGraph = LocationWeekGraph + "</dataset>";
            }
            if (Week.Length >= 3)
            {

                LocationWeekGraph = LocationWeekGraph + " <dataset seriesname='Week " + Week[2].Week + "' color='FF9900'>";
                foreach (var item in loclist)
                {
                    LocationWeekGraph = LocationWeekGraph + "<set value='" + item.Troops3 + "' link=\"JavaScript: LocationWeekSelection('" + PeriodYear + "," + Period + "," + Sector + "," + ContingentType + "," + item.Location + "," + Week[2].Week + "');" + "\" />";
                }
                LocationWeekGraph = LocationWeekGraph + "</dataset>";
            }
            if (Week.Length >= 4)
            {
                LocationWeekGraph = LocationWeekGraph + " <dataset seriesname='Week " + Week[3].Week + "' color='990033'>";
                foreach (var item in loclist)
                {
                    LocationWeekGraph = LocationWeekGraph + "<set value='" + item.Troops4 + "' link=\"JavaScript: LocationWeekSelection('" + PeriodYear + "," + Period + "," + Sector + "," + ContingentType + "," + item.Location + "," + Week[3].Week + "');" + "\" />";
                }
                LocationWeekGraph = LocationWeekGraph + "</dataset>";

            }

            LocationWeekGraph = LocationWeekGraph + "</graph>";
            Response.Write(LocationWeekGraph);
            return null;

        }
        //Contingent Graph
        //  
        public ActionResult GetNameGraph_TroopsReport(string PeriodYear, string Period, string Sector, string ContingentType, string Location, string Criteria, string Flag)
        {
            IList<TroopsReport> troopsreport = new List<TroopsReport>();
            troopsreport = DashboardService.GetTroopsReportbyFlag(PeriodYear, Period, Sector, ContingentType, Location, Flag, Criteria);
            var ContingentGraph = " <graph caption='Contingent based Troops Strength ' xAxisName='Contingent' forceDecimals='0'  rotateLabels='1' formatNumberScale='0'   yAxisName='' animation='1' showNames='1' showValues='1'  divlinecolor='d3d3d3' distance='6'   rotateNames='0'>";
            string[] color = new string[12] { "0000A0", "800080", "e44a00", "52D017", "FFA62F", "E66C2C", "C25A7C", "59E817", "00FF00", "FFA62F", "FF7F50", "FF00FF" };

            int i = 0;
            foreach (var contingent in troopsreport)
            {
                i = i + 1;
                // PeriodYearGraph = PeriodYearGraph + " <set name='2013-2014' value='" + Period13to14 + "' color='8B008B' link=\"JavaScript: MCClickEvent('"+a+"');"+"\" />";
                ContingentGraph = ContingentGraph + " <set name='" + contingent.Name + "' value='" + contingent.SumofTroops + "' color='" + color[i] + "' />";
            }
            ContingentGraph = ContingentGraph + "</graph>";
            Response.Write(ContingentGraph);

            return null;
        }

        #endregion

        #region  period wise qty report
        public ActionResult PeriodWiseQtyReport()
        {

            return View();
        }


        public ActionResult PeriodWiseQtyValueReport(string searchItems, string ExptType, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                OrdersService us = new OrdersService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();

                if (!string.IsNullOrWhiteSpace(sord) && sord == "desc")
                    sord = "Desc";
                else
                    sord = "Asc";



                if (string.IsNullOrWhiteSpace(searchItems))
                {

                    return null;
                }
                else
                {

                    #region Desouza

                    if (searchItems != null && searchItems != "")
                    {
                        var Items = searchItems.ToString().Split(',');
                        if (!string.IsNullOrWhiteSpace(Items[0])) { criteria.Add("Sector", Items[0]); }
                        if (!string.IsNullOrWhiteSpace(Items[1])) { criteria.Add("Location", Items[1]); }
                        if (!string.IsNullOrWhiteSpace(Items[2])) { criteria.Add("ContingentType", Items[2]); }
                        if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Contingent", Items[3]); }
                        if (!string.IsNullOrWhiteSpace(Items[4])) { criteria.Add("PeriodYear", Items[4]); }
                        if (!string.IsNullOrWhiteSpace(Items[5])) { criteria.Add("Period", Items[5]); }
                        if (!string.IsNullOrWhiteSpace(Items[6]))
                        {
                            criteria.Add("Week", Convert.ToInt64(Items[6]));
                            criteria.Add("ReportType", "ORDINV");
                        }
                    }

                    #endregion

                    Dictionary<long, IList<InvoiceReports>> invreport1 = IS.GetInvoiceReportsListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                    IList<InvoiceReports> invreport = invreport1.FirstOrDefault().Value.ToList();

                    if (invreport != null && invreport.Count > 0)
                    {
                        if (ExptType == "Excel")
                        {


                            int i = 1;
                            foreach (var item in invreport)
                            {
                                item.OrderId = i;
                                i = i + 1;
                            }
                            var List = invreport;
                            List<string> lstHeader = new List<string>() { "ControlId", "OrderQty", "DeliveredQty", "InvoiceQty", "OrderValue", "InvoiceValue" };
                            base.NewExportToExcel(List, "Order Qty and values  Report", (item => new
                            {

                                ControlId = item.ControlId,
                                OrderQty = item.Orderedqty.ToString(),
                                DeliveredQty = item.Deliveredqty,
                                InvoiceQty = item.Acceptedqty,
                                OrderValue = item.Amountordered,
                                InvoiceValue = item.Amountaccepted
                            }), lstHeader);
                            return new EmptyResult();


                        }
                        else
                        {
                            long totalrecords = invreport.Count;
                            int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                            var jsondat = new
                            {
                                total = totalPages,
                                page = page,
                                records = totalrecords,

                                rows = (from items in invreport
                                        select new
                                        {
                                            i = 2,
                                            cell = new string[] {

                                            items.OrderId.ToString(),
                                            items.ControlId.ToString(),
                                            items.Orderedqty.ToString(),
                                            items.Deliveredqty.ToString(),
                                            items.Acceptedqty.ToString(),
                                            items.Amountordered.ToString(),
                                            items.Amountaccepted.ToString()
                                            
                                            }
                                        })
                            };
                            return Json(jsondat, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        var jsondat = new { rows = (new { cell = new string[] { } }) };
                        return Json(jsondat, JsonRequestBehavior.AllowGet);

                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }

        public ActionResult GetPeriodYearGraph()
        {

            IList<PeriodWiseQtyReport_SP> qtyreport = new List<PeriodWiseQtyReport_SP>();
            qtyreport = DashboardService.GetPeriodWiseQtyReportbyFlag("PERIODYEAR", string.Empty, string.Empty, string.Empty);
            var OrderQtyArray = (from u in qtyreport select new { u.PeriodYear, u.OrderQty }).ToArray();
            var DeliveredQtyArray = (from u in qtyreport select new { u.PeriodYear, u.DeliveredQty }).ToArray();
            var InvoiceQtyArray = (from u in qtyreport select new { u.PeriodYear, u.InvoiceQty }).ToArray();
            var InvoiceValueArray = (from u in qtyreport select new { u.PeriodYear, u.InvoiceValue }).ToArray();
            var PeriodyearArray = (from u in qtyreport select new { u.PeriodYear }).ToArray();

            var PeriodGraph = " <Graph caption='Period Year Based Qty Report ' xAxisName='Period Year' forceDecimals='1'  rotateLabels='0' valuePadding='0' formatNumberScale='0'   showValues='0' yAxisName='' animation='1' showNames='0'  divlinecolor='d3d3d3' distance='6'  rotateNames='0' rotateValues='1'>";
            PeriodGraph = PeriodGraph + "<categories>";
            foreach (var Periodyear in PeriodyearArray)
            {
                PeriodGraph = PeriodGraph + "<category name='" + Periodyear.PeriodYear + "' />";
            }

            PeriodGraph = PeriodGraph + "</categories>";

            PeriodGraph = PeriodGraph + " <dataset seriesname='OrderQty'  color='#A3C586' rotateValues='1'>";
            foreach (var OrderQty in OrderQtyArray)
            {
                PeriodGraph = PeriodGraph + "<set value='" + OrderQty.OrderQty + "' link=\"JavaScript: MCClickEvent('" + OrderQty.PeriodYear + "');" + "\" />";
            }

            PeriodGraph = PeriodGraph + "</dataset>";

            PeriodGraph = PeriodGraph + " <dataset seriesname='DeliveredQty'   color='#FFCC33'>";
            foreach (var DeliveredQty in DeliveredQtyArray)
            {
                PeriodGraph = PeriodGraph + "<set value='" + DeliveredQty.DeliveredQty + "' link=\"JavaScript: MCClickEvent('" + DeliveredQty.PeriodYear + "');" + "\" />";

            }

            PeriodGraph = PeriodGraph + "</dataset>";

            PeriodGraph = PeriodGraph + " <dataset seriesname='InvoiceQty'   color='#FCF1D1' >";
            foreach (var InvoiceQty in InvoiceQtyArray)
            {
                PeriodGraph = PeriodGraph + "<set value='" + InvoiceQty.InvoiceQty + "' link=\"JavaScript: MCClickEvent('" + InvoiceQty.PeriodYear + "');" + "\" />";

            }

            PeriodGraph = PeriodGraph + "</dataset>";




            //PeriodGraph = PeriodGraph + " <dataset seriesname='InvoiceValue'  color='f8bd19' showValue='1'>";
            //foreach (var Invoicevalue in InvoiceValueArray)
            //{
            //    PeriodGraph = PeriodGraph + "<set value='" + Invoicevalue.InvoiceValue + "' link=\"JavaScript: MCClickEvent('" + Invoicevalue.PeriodYear + "');" + "\" />"; 
            //}

            //PeriodGraph = PeriodGraph + "</dataset>";

            PeriodGraph = PeriodGraph + "</Graph>";
            Response.Write(PeriodGraph);
            return null;


        }

        public ActionResult GetPeriodGraph(string PeriodYear)
        {
            //string PeriodYear = "13-14";
            IList<PeriodWiseQtyReport_SP> qtyreport = new List<PeriodWiseQtyReport_SP>();
            qtyreport = DashboardService.GetPeriodWiseQtyReportbyFlag("PERIOD", PeriodYear, string.Empty, string.Empty);
            var OrderQtyArray = (from u in qtyreport select new { u.Period, u.OrderQty }).ToArray();
            var DeliveredQtyArray = (from u in qtyreport select new { u.Period, u.DeliveredQty }).ToArray();
            var InvoiceQtyArray = (from u in qtyreport select new { u.Period, u.InvoiceQty }).ToArray();
            var InvoiceValueArray = (from u in qtyreport select new { u.Period, u.InvoiceValue }).ToArray();
            var PeriodArray = (from u in qtyreport select new { u.Period }).ToArray();

            var PeriodGraph = " <graph caption='Periodwise Qty Report for" + PeriodYear + " ' showYAxisValues='0' xAxisName='Period' forceDecimals='0'  rotateLabels='1' formatNumberScale='0'   yAxisName='' animation='1' showNames='1' showValues='0'  divlinecolor='d3d3d3' distance='0'   rotateNames='0'>";
            PeriodGraph = PeriodGraph + "<categories>";
            foreach (var Period in PeriodArray)
            {
                PeriodGraph = PeriodGraph + "<category name='" + Period.Period + "' />";
            }

            PeriodGraph = PeriodGraph + "</categories>";

            PeriodGraph = PeriodGraph + " <dataset seriesname='Order Qty'   color='#A3C586'>";
            foreach (var OrderQty in OrderQtyArray)
            {
                PeriodGraph = PeriodGraph + "<set value='" + OrderQty.OrderQty + "' link=\"JavaScript: PeriodSelection('" + PeriodYear + "," + OrderQty.Period + "');" + "\" />";
            }

            PeriodGraph = PeriodGraph + "</dataset>";

            PeriodGraph = PeriodGraph + " <dataset seriesname='Delivered Qty'   color='#FFCC33'>";
            foreach (var DeliveredQty in DeliveredQtyArray)
            {
                PeriodGraph = PeriodGraph + "<set value='" + DeliveredQty.DeliveredQty + "' link=\"JavaScript: PeriodSelection('" + PeriodYear + "," + DeliveredQty.Period + "');" + "\" />";

            }

            PeriodGraph = PeriodGraph + "</dataset>";

            PeriodGraph = PeriodGraph + " <dataset seriesname='Invoice Qty'   color='#FCF1D1' >";
            foreach (var InvoiceQty in InvoiceQtyArray)
            {
                PeriodGraph = PeriodGraph + "<set value='" + InvoiceQty.InvoiceQty + "' link=\"JavaScript: PeriodSelection('" + PeriodYear + "," + InvoiceQty.Period + "');" + "\" />";

            }

            PeriodGraph = PeriodGraph + "</dataset>";



            //PeriodGraph = PeriodGraph + " <dataset seriesname='Invoice Value'  color='f8bd19' showValue='1'>";
            //foreach (var Invoicevalue in InvoiceValueArray)
            //{
            //    PeriodGraph = PeriodGraph + "<set value='" + Invoicevalue.InvoiceValue + "' />";
            //}

            //PeriodGraph = PeriodGraph + "</dataset>";

            PeriodGraph = PeriodGraph + "</graph>";
            Response.Write(PeriodGraph);
            return null;
        }

        //Sector graph qty report
        public ActionResult GetSectorGraph_QtyReport(string PeriodYear, string Period, string Sector)
        {

            IList<PeriodWiseQtyReport_SP> qtyreport = new List<PeriodWiseQtyReport_SP>();
            qtyreport = DashboardService.GetPeriodWiseQtyReportbyFlag("SECTOR", PeriodYear, Period, Sector);
            var OrderQtyArray = (from u in qtyreport select new { u.Period, u.OrderQty }).ToArray();
            var DeliveredQtyArray = (from u in qtyreport select new { u.Period, u.DeliveredQty }).ToArray();
            var InvoiceQtyArray = (from u in qtyreport select new { u.Period, u.InvoiceQty }).ToArray();
            var InvoiceValueArray = (from u in qtyreport select new { u.Period, u.InvoiceValue }).ToArray();
            var ContingentType = (from u in qtyreport select new { u.ContingentType }).Distinct().ToArray();

            var SectorGraph = " <graph caption='PeriodYear : " + PeriodYear + "   Period : " + Period + "   Sector :  " + Sector + " ' showYAxisValues='0' showXAxisValues='0' xAxisName='Contingent Type' forceDecimals='0'  rotateLabels='' formatNumberScale='0'   yAxisName='' animation='1' showNames='1' showValues='1'  divlinecolor='d3d3d3' distance='0'   rotateNames='1'>";
            SectorGraph = SectorGraph + "<categories>";
            foreach (var conType in ContingentType)
            {
                SectorGraph = SectorGraph + "<category name='" + conType.ContingentType + "' />";
            }

            SectorGraph = SectorGraph + "</categories>";

            SectorGraph = SectorGraph + " <dataset seriesname='Order Qty'   showValues='1' color='#A3C586'>";
            foreach (var OrderQty in OrderQtyArray)
            {
                SectorGraph = SectorGraph + "<set value='" + OrderQty.OrderQty + "' />";
            }

            SectorGraph = SectorGraph + "</dataset>";

            SectorGraph = SectorGraph + " <dataset seriesname='Delivered Qty'   color='#FFCC33'>";
            foreach (var DeliveredQty in DeliveredQtyArray)
            {
                SectorGraph = SectorGraph + "<set value='" + DeliveredQty.DeliveredQty + "'/>";

            }

            SectorGraph = SectorGraph + "</dataset>";

            SectorGraph = SectorGraph + " <dataset seriesname='Invoice Qty'   color='#FCF1D1' >";
            foreach (var InvoiceQty in InvoiceQtyArray)
            {
                SectorGraph = SectorGraph + "<set value='" + InvoiceQty.InvoiceQty + "'/>";

            }

            SectorGraph = SectorGraph + "</dataset>";

            //PeriodGraph = PeriodGraph + " <dataset seriesname='Invoice Value'  color='f8bd19' showValue='1'>";
            //foreach (var Invoicevalue in InvoiceValueArray)
            //{
            //    PeriodGraph = PeriodGraph + "<set value='" + Invoicevalue.InvoiceValue + "' />";
            //}

            //PeriodGraph = PeriodGraph + "</dataset>";

            SectorGraph = SectorGraph + "</graph>";
            Response.Write(SectorGraph);
            return null;
        }
        #endregion

        #region period wise deduction report

        public ActionResult PeriodWiseDeductionReport()
        {


            return View();
        }

        public ActionResult PeriodwiseDeductionList(string searchItems, string ExptType, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                OrdersService us = new OrdersService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();

                if (!string.IsNullOrWhiteSpace(sord) && sord == "desc")
                    sord = "Desc";
                else
                    sord = "Asc";



                if (string.IsNullOrWhiteSpace(searchItems))
                {

                    return null;
                }
                else
                {

                    if (searchItems != null && searchItems != "")
                    {
                        var Items = searchItems.ToString().Split(',');
                        if (!string.IsNullOrWhiteSpace(Items[0])) { criteria.Add("Sector", Items[0]); }
                        if (!string.IsNullOrWhiteSpace(Items[1])) { criteria.Add("Location", Items[1]); }
                        if (!string.IsNullOrWhiteSpace(Items[2])) { criteria.Add("ContingentType", Items[2]); }
                        if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Contingent", Items[3]); }
                        if (!string.IsNullOrWhiteSpace(Items[4])) { criteria.Add("PeriodYear", Items[4]); }
                        if (!string.IsNullOrWhiteSpace(Items[5])) { criteria.Add("Period", Items[5]); }
                        if (!string.IsNullOrWhiteSpace(Items[6]))
                        {
                            criteria.Add("Week", Convert.ToInt64(Items[6]));
                            criteria.Add("ReportType", "ORDINV");
                        }
                    }

                    Dictionary<long, IList<InvoiceReports>> invreport1 = IS.GetInvoiceReportsListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                    IList<InvoiceReports> invreport = invreport1.FirstOrDefault().Value.ToList();

                    if (invreport != null && invreport.Count > 0)
                    {
                        if (ExptType == "Excel")
                        {
                            int i = 1;
                            foreach (var item in invreport)
                            {
                                item.OrderId = i;
                                i = i + 1;
                            }
                            var List = invreport;
                            List<string> lstHeader = new List<string>() { "ControlId", "AplTimelydelivery", "AplOrderbylineitems", "AplOrdersbyweight", "AplNoofauthorizedsubstitutions" };
                            base.NewExportToExcel(List, "Deduction Report", (item => new
                            {

                                ControlId = item.ControlId,
                                OrderQty = item.AplTimelydelivery.ToString(),
                                DeliveredQty = item.AplOrderbylineitems.ToString(),
                                InvoiceQty = item.AplOrdersbyweight.ToString(),
                                OrderValue = item.AplNoofauthorizedsubstitutions.ToString()

                            }), lstHeader);
                            return new EmptyResult();

                        }
                        else
                        {
                            long totalrecords = invreport1.First().Key;
                            int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                            var jsondat = new
                            {
                                total = totalPages,
                                page = page,
                                records = totalrecords,

                                rows = (from items in invreport1.FirstOrDefault().Value
                                        select new
                                        {
                                            i = 2,
                                            cell = new string[] {

                                            items.OrderId.ToString(),
                                            items.ControlId.ToString(),
                                            items.AplTimelydelivery.ToString(),
                                            items.AplOrderbylineitems.ToString(),
                                            items.AplOrdersbyweight.ToString(),
                                            items.AplNoofauthorizedsubstitutions.ToString()
                                            }
                                        })
                            };
                            return Json(jsondat, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        var jsondat = new { rows = (new { cell = new string[] { } }) };
                        return Json(jsondat, JsonRequestBehavior.AllowGet);

                    }

                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }


        public ActionResult GetPeriodYearGraph_DeductionChart()
        {
            IList<PeriodWiseDeductionReport> deductionreport = new List<PeriodWiseDeductionReport>();
            deductionreport = DashboardService.GetPeriodWiseDeductionReportbyFlag("PERIODYEAR", string.Empty, string.Empty, string.Empty);

            var APL_TimelyDeliveryArray = (from u in deductionreport select new { u.PeriodYear, u.APL_TimelyDelivery }).ToArray();
            var APL_OrderbyLineItemsArray = (from u in deductionreport select new { u.PeriodYear, u.APL_OrderbyLineItems }).ToArray();
            var APL_OrdersbyWeightArray = (from u in deductionreport select new { u.PeriodYear, u.APL_OrdersbyWeight }).ToArray();
            var APL_NoofAuthorizedSubstitutionsArray = (from u in deductionreport select new { u.PeriodYear, u.APL_NoofAuthorizedSubstitutions }).ToArray();
            var PeriodYearArray = (from u in deductionreport select new { u.PeriodYear }).ToArray();

            var PeriodGraph = " <Graph caption='Period Year Based Qty Report ' xAxisName='Period Year'  rotateLabels='0' valuePadding='0' formatNumberScale='0'   yAxisName='' animation='1' showNames='1' showValues='0'  divlinecolor='d3d3d3' distance='6'  rotateNames='0'>";
            PeriodGraph = PeriodGraph + "<categories>";
            foreach (var Periodyear in PeriodYearArray)
            {
                PeriodGraph = PeriodGraph + "<category name='" + Periodyear.PeriodYear + "' />";
            }

            PeriodGraph = PeriodGraph + "</categories>";

            PeriodGraph = PeriodGraph + " <dataset seriesname='APL_TimelyDeliveryArray'  color='#EDBD3E' rotateValues='1'>";
            foreach (var time in APL_TimelyDeliveryArray)
            {
                PeriodGraph = PeriodGraph + "<set value='" + time.APL_TimelyDelivery + "' link=\"JavaScript: MCClickEvent('" + time.PeriodYear + "');" + "\" />";
            }

            PeriodGraph = PeriodGraph + "</dataset>";

            PeriodGraph = PeriodGraph + " <dataset seriesname='APL_OrderbyLineItemsArray'   color='#5B7778'>";
            foreach (var line in APL_OrderbyLineItemsArray)
            {
                PeriodGraph = PeriodGraph + "<set value='" + line.APL_OrderbyLineItems + "' link=\"JavaScript: MCClickEvent('" + line.PeriodYear + "');" + "\" />";

            }

            PeriodGraph = PeriodGraph + "</dataset>";

            PeriodGraph = PeriodGraph + " <dataset seriesname='APL_OrdersbyWeightArray'   color='#93A8A9' >";
            foreach (var weight in APL_OrdersbyWeightArray)
            {
                PeriodGraph = PeriodGraph + "<set value='" + weight.APL_OrdersbyWeight + "' link=\"JavaScript: MCClickEvent('" + weight.PeriodYear + "');" + "\" />";

            }

            PeriodGraph = PeriodGraph + "</dataset>";

            PeriodGraph = PeriodGraph + " <dataset seriesname='APL_NoofAuthorizedSubstitutionsArray'  color='#D1E6E7' showValue='1'>";
            foreach (var substitution in APL_NoofAuthorizedSubstitutionsArray)
            {
                PeriodGraph = PeriodGraph + "<set value='" + substitution.APL_NoofAuthorizedSubstitutions + "' link=\"JavaScript: MCClickEvent('" + substitution.PeriodYear + "');" + "\" />";
            }

            PeriodGraph = PeriodGraph + "</dataset>";

            PeriodGraph = PeriodGraph + "</Graph>";
            Response.Write(PeriodGraph);
            return null;

        }

        public ActionResult GetPeriodGraph_DeductionChart(string PeriodYear)
        {
            //string PeriodYear = "13-14";
            Dictionary<string, object> criteria = new Dictionary<string, object>();
            //criteria.Add("PeriodYear", PeriodYear);
            //Dictionary<long, IList<PeriodWiseValuesDashboard_vw>> Qtylist = DashboardService.GetPeriodWiseQtyListWithPagingAndCriteria(0, 999999, string.Empty, string.Empty, criteria);
            //IList<PeriodWiseValuesDashboard_vw> QtyIList = Qtylist.FirstOrDefault().Value.ToList();

            IList<PeriodWiseDeductionReport> deductionreport = new List<PeriodWiseDeductionReport>();
            deductionreport = DashboardService.GetPeriodWiseDeductionReportbyFlag("PERIOD", PeriodYear, string.Empty, string.Empty);

            var APL_TimelyDeliveryArray = (from u in deductionreport select new { u.PeriodYear, u.Period, u.APL_TimelyDelivery }).ToArray();
            var APL_OrderbyLineItemsArray = (from u in deductionreport select new { u.PeriodYear, u.Period, u.APL_OrderbyLineItems }).ToArray();
            var APL_OrdersbyWeightArray = (from u in deductionreport select new { u.PeriodYear, u.Period, u.APL_OrdersbyWeight }).ToArray();
            var APL_NoofAuthorizedSubstitutionsArray = (from u in deductionreport select new { u.PeriodYear, u.Period, u.APL_NoofAuthorizedSubstitutions }).ToArray();
            var PeriodArray = (from u in deductionreport select new { u.Period }).ToArray();

            var PeriodGraph = " <graph caption='Periodwise Deduction Report " + PeriodYear + "' xAxisName='Period' forceDecimals='0' divlineisdashed='1' formatNumberScale='0'   yAxisName='' animation='1' showNames='1' showValues='0'  divlinecolor='d3d3d3' distance='6' angle='45'  rotateNames='1'>";
            PeriodGraph = PeriodGraph + "<categories>";
            foreach (var Period in PeriodArray)
            {
                PeriodGraph = PeriodGraph + "<category name='" + Period.Period + "' />";
            }

            PeriodGraph = PeriodGraph + "</categories>";

            PeriodGraph = PeriodGraph + " <dataset seriesname='APL_TimelyDelivery' color='#EDBD3E'>";
            foreach (var time in APL_TimelyDeliveryArray)
            {
                PeriodGraph = PeriodGraph + "<set value='" + time.APL_TimelyDelivery + "' link=\"JavaScript: PeriodSelection('" + PeriodYear + "," + time.Period + "');" + "\" />";
            }

            PeriodGraph = PeriodGraph + "</dataset>";

            PeriodGraph = PeriodGraph + " <dataset seriesname='APL_OrderbyLineItems' color='#5B7778'>";
            foreach (var lineitem in APL_OrderbyLineItemsArray)
            {
                PeriodGraph = PeriodGraph + "<set value='" + lineitem.APL_OrderbyLineItems + "' link=\"JavaScript: PeriodSelection('" + PeriodYear + "," + lineitem.Period + "');" + "\" />";

            }

            PeriodGraph = PeriodGraph + "</dataset>";

            PeriodGraph = PeriodGraph + " <dataset seriesname='APL_OrdersbyWeight' color='#93A8A9' >";
            foreach (var weight in APL_OrdersbyWeightArray)
            {
                PeriodGraph = PeriodGraph + "<set value='" + weight.APL_OrdersbyWeight + "' link=\"JavaScript: PeriodSelection('" + PeriodYear + "," + weight.Period + "');" + "\" />";

            }

            PeriodGraph = PeriodGraph + "</dataset>";

            PeriodGraph = PeriodGraph + " <dataset seriesname='APL_NoofAuthorizedSubstitutions' color='#D1E6E7' showValue='1'>";
            foreach (var substitution in APL_NoofAuthorizedSubstitutionsArray)
            {
                PeriodGraph = PeriodGraph + "<set value='" + substitution.APL_NoofAuthorizedSubstitutions + "' link=\"JavaScript: PeriodSelection('" + PeriodYear + "," + substitution.Period + "');" + "\" />";
            }

            PeriodGraph = PeriodGraph + "</dataset>";
            PeriodGraph = PeriodGraph + "</graph>";
            Response.Write(PeriodGraph);
            return null;
        }

        public ActionResult GetSectorGraph_DeductionReport(string PeriodYear, string Period, string Sector)
        {

            IList<PeriodWiseDeductionReport> deductionreport = new List<PeriodWiseDeductionReport>();
            deductionreport = DashboardService.GetPeriodWiseDeductionReportbyFlag("SECTOR", PeriodYear, Period, Sector);
            var APL_TimelyDeliveryArray = (from u in deductionreport select new { u.PeriodYear, u.Period, u.APL_TimelyDelivery }).ToArray();
            var APL_OrderbyLineItemsArray = (from u in deductionreport select new { u.PeriodYear, u.Period, u.APL_OrderbyLineItems }).ToArray();
            var APL_OrdersbyWeightArray = (from u in deductionreport select new { u.PeriodYear, u.Period, u.APL_OrdersbyWeight }).ToArray();
            var APL_NoofAuthorizedSubstitutionsArray = (from u in deductionreport select new { u.PeriodYear, u.Period, u.APL_NoofAuthorizedSubstitutions }).ToArray();
            var ContingentType = (from u in deductionreport select new { u.ContingentType }).Distinct().ToArray();

            var SectorGraph = " <graph caption='PeriodYear : " + PeriodYear + "   Period : " + Period + "   Sector :  " + Sector + " ' showYAxisValues='0' xAxisName='Contingent Type' forceDecimals='0'  rotateLabels='0' formatNumberScale='0'   yAxisName='' animation='1' showNames='1' showValues='1'  divlinecolor='d3d3d3' distance='0'   rotateNames='0'>";
            SectorGraph = SectorGraph + "<categories>";
            foreach (var conType in ContingentType)
            {
                SectorGraph = SectorGraph + "<category name='" + conType.ContingentType + "' />";
            }

            SectorGraph = SectorGraph + "</categories>";

            SectorGraph = SectorGraph + " <dataset seriesname='APL_TimelyDelivery'   showValues='1' color='#EDBD3E'>";
            foreach (var time in APL_TimelyDeliveryArray)
            {
                SectorGraph = SectorGraph + "<set value='" + time.APL_TimelyDelivery + "' />";
            }

            SectorGraph = SectorGraph + "</dataset>";

            SectorGraph = SectorGraph + " <dataset seriesname='APL_OrderbyLineItems'   color='#5B7778'>";
            foreach (var lineitem in APL_OrderbyLineItemsArray)
            {
                SectorGraph = SectorGraph + "<set value='" + lineitem.APL_OrderbyLineItems + "'/>";

            }

            SectorGraph = SectorGraph + "</dataset>";

            SectorGraph = SectorGraph + " <dataset seriesname='APL_OrdersbyWeight'   color='#93A8A9' >";
            foreach (var weight in APL_OrdersbyWeightArray)
            {
                SectorGraph = SectorGraph + "<set value='" + weight.APL_OrdersbyWeight + "'/>";

            }

            SectorGraph = SectorGraph + "</dataset>";

            SectorGraph = SectorGraph + " <dataset seriesname='APL_NoofAuthorizedSubstitutions'  color='#D1E6E7' showValue='1'>";
            foreach (var substitution in APL_NoofAuthorizedSubstitutionsArray)
            {
                SectorGraph = SectorGraph + "<set value='" + substitution.APL_NoofAuthorizedSubstitutions + "' />";
            }

            SectorGraph = SectorGraph + "</dataset>";

            SectorGraph = SectorGraph + "</graph>";
            Response.Write(SectorGraph);
            return null;
        }

        #endregion

        #region Loss because of Substitution

        public ActionResult LossBecauseOfSubstitutionReport()
        {
            return View();
        }



        public ActionResult LossBecauseOfSubstitutionJQGrid(string searchItems, string ExptType, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                OrdersService us = new OrdersService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();

                if (!string.IsNullOrWhiteSpace(sord) && sord == "desc")
                    sord = "Desc";
                else
                    sord = "Asc";



                if (string.IsNullOrWhiteSpace(searchItems))
                {

                    return null;
                }
                else
                {

                    #region Desouza

                    if (searchItems != null && searchItems != "")
                    {
                        var Items = searchItems.ToString().Split(',');
                        if (!string.IsNullOrWhiteSpace(Items[0])) { criteria.Add("Sector", Items[0]); }
                        if (!string.IsNullOrWhiteSpace(Items[1])) { criteria.Add("Location", Items[1]); }
                        if (!string.IsNullOrWhiteSpace(Items[2])) { criteria.Add("ContingentType", Items[2]); }
                        if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Contingent", Items[3]); }
                        if (!string.IsNullOrWhiteSpace(Items[4])) { criteria.Add("PeriodYear", Items[4]); }
                        if (!string.IsNullOrWhiteSpace(Items[5])) { criteria.Add("Period", Items[5]); }
                        if (!string.IsNullOrWhiteSpace(Items[6])) { criteria.Add("Week", Convert.ToInt64(Items[6])); }
                    }

                    #endregion

                    Dictionary<long, IList<LossBecauseOfSubstitutions_vw>> Lossist = DashboardService.GetLossBecauseOfSubstitutionListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                    IList<LossBecauseOfSubstitutions_vw> LossReport = Lossist.FirstOrDefault().Value.ToList();

                    if (LossReport != null && LossReport.Count > 0)
                    {
                        if (ExptType == "Excel")
                        {


                            int i = 1;
                            foreach (var item in LossReport)
                            {
                                item.OrderId = i;
                                i = i + 1;
                            }
                            var List = LossReport;
                            List<string> lstHeader = new List<string>() { "S.No", "ContorId", "Contingent", "ContingentType", "Period", "Sector", "Week", "Location", "PeriodYear", "UNCode", "ItemName", "SubstituteItemCode", "SubstituteItemName", "ActualItemPrice", "SubstituteItemPrice", "InvoiceQty", "InvoiceValue", "Loss because of Substitution" };
                            base.NewExportToExcel(List, "Loss Because of Substitution", (item => new
                            {

                                Id = item.Id.ToString(),
                                ControlId = item.ControlId,
                                Name = item.Name,
                                ContingentType = item.ContingentType,
                                Period = item.Period,
                                Sector = item.Sector,
                                Week = item.Week.ToString(),
                                Location = item.Location,
                                PeriodYear = item.PeriodYear.ToString(),
                                UNCode = item.UNCode.ToString(),
                                ItemName = item.ItemName,
                                SubstituteItemCode = item.SubstituteItemCode.ToString(),
                                SubstituteItemName = item.SubstituteItemName,
                                ActualItemPrice = item.ActualItemPrice.ToString(),
                                SubstituteItemPrice = item.SubstituteItemPrice.ToString(),
                                InvoiceQty = item.InvoiceQty.ToString(),
                                InvoiceValue = item.InvoiceValue.ToString(),
                                Loss = item.Loss.ToString()
                            }), lstHeader);
                            return new EmptyResult();


                        }
                        else
                        {
                            long totalrecords = LossReport.Count;
                            int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                            var jsondat = new
                            {
                                total = totalPages,
                                page = page,
                                records = totalrecords,

                                rows = (from items in LossReport
                                        select new
                                        {
                                            i = 2,
                                            cell = new string[] {

                                            items.Id.ToString(),
                                            items.OrderId.ToString(),
                                            items.ControlId,
                                            items.Name,
                                            items.ContingentType,
                                            items.Period,
                                            items.Sector,
                                            items.Week.ToString(),
                                            items.Location,
                                            items.PeriodYear,
                                            items.UNCode.ToString(),
                                            items.ItemName,
                                            items.SubstituteItemCode.ToString(),
                                            items.SubstituteItemName,
                                            items.ActualItemPrice.ToString(),
                                            items.SubstituteItemPrice.ToString(),
                                            items.Status,
                                            items.DeliveredQty.ToString(),
                                            items.InvoiceQty.ToString(),
                                            items.ActualItemValue.ToString(),
                                            items.SubsItemValue.ToString(),
                                            items.InvoiceValue.ToString(),
                                            items.Loss.ToString()
                                            }
                                        })
                            };
                            return Json(jsondat, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        var jsondat = new { rows = (new { cell = new string[] { } }) };
                        return Json(jsondat, JsonRequestBehavior.AllowGet);

                    }

                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }



        #endregion


        #region  Authorized and unauthorized substitution report
        #region AuthorizedUnAuthorizedSubstitutionList_VW
        public ActionResult ConsolidatedSubstitutionReport()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;

            }
        }



        public ActionResult ConsolidatedSubstitutionReportJQGrid(string searchItems, string ExptType, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                OrdersService us = new OrdersService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();

                if (!string.IsNullOrWhiteSpace(sord) && sord == "desc")
                    sord = "Desc";
                else
                    sord = "Asc";



                if (string.IsNullOrWhiteSpace(searchItems))
                {

                    return null;
                }
                else
                {

                    #region Desouza

                    if (searchItems != null && searchItems != "")
                    {
                        var Items = searchItems.ToString().Split(',');
                        if (!string.IsNullOrWhiteSpace(Items[0])) { criteria.Add("Sector", Items[0]); }
                        if (!string.IsNullOrWhiteSpace(Items[1])) { criteria.Add("Location", Items[1]); }
                        if (!string.IsNullOrWhiteSpace(Items[2])) { criteria.Add("ContingentType", Items[2]); }
                        if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Contingent", Items[3]); }
                        if (!string.IsNullOrWhiteSpace(Items[4])) { criteria.Add("PeriodYear", Items[4]); }
                        if (!string.IsNullOrWhiteSpace(Items[5])) { criteria.Add("Period", Items[5]); }
                        if (!string.IsNullOrWhiteSpace(Items[6])) { criteria.Add("Week", Convert.ToInt64(Items[6])); }
                    }

                    #endregion

                    Dictionary<long, IList<AuthorizedUnAuthorizedSubstitutionList_VW>> subslist1 = DashboardService.GetConsolidatedSubstitutionReportsListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                    IList<AuthorizedUnAuthorizedSubstitutionList_VW> SubstitutionReport = subslist1.FirstOrDefault().Value.ToList();

                    if (SubstitutionReport != null && SubstitutionReport.Count > 0)
                    {
                        if (ExptType == "Excel")
                        {


                            int i = 1;
                            foreach (var item in SubstitutionReport)
                            {
                                item.OrderId = i;
                                i = i + 1;
                            }
                            var List = SubstitutionReport;
                            List<string> lstHeader = new List<string>() { "ControlId", "UNCode", "ItemName", "SubstituteItemCode", "SubstituteItemName", "DeliveredQty", "Status" };
                            base.NewExportToExcel(List, "Substitution Report", (item => new
                            {
                                ControlId = item.ControlId,
                                UNCode = item.UNCode.ToString(),
                                ItemName = item.ItemName,
                                SubstituteItemCode = item.SubstituteItemCode.ToString(),
                                SubstituteItemName = item.SubstituteItemName,
                                DeliveredQty = item.DeliveredQty.ToString(),
                                Status = item.Status

                            }), lstHeader);
                            return new EmptyResult();


                        }
                        else
                        {
                            long totalrecords = SubstitutionReport.Count;
                            int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                            var jsondat = new
                            {
                                total = totalPages,
                                page = page,
                                records = totalrecords,

                                rows = (from items in SubstitutionReport
                                        select new
                                        {
                                            i = 2,
                                            cell = new string[] {

                                            items.Id.ToString(),
                                  items.ControlId,
                                  items.OrderId.ToString(),
                                  items.Sector,
                                  items.ContingentType,
                                  items.Name,
                                  items.Period,
                                  items.PeriodYear,
                                  items.Location,
                                  items.Week.ToString(),
                                  items.UNCode.ToString(),
                                  items.ItemName,
                                  items.SubstituteItemCode.ToString(),
                                  items.SubstituteItemName,
                                  items.DeliveredQty.ToString(),
                                  items.Status
                                            
                                            }
                                        })
                            };
                            return Json(jsondat, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        var jsondat = new { rows = (new { cell = new string[] { } }) };
                        return Json(jsondat, JsonRequestBehavior.AllowGet);

                    }

                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }


        public ActionResult Subgrid()
        {
            return View();
        }
        //public JsonResult SubstitutionListjQGrid(int rows, string sidx, string sord, int? page = 1)
        //{
        //    try
        //    {

        //        Dictionary<string, object> criteria = new Dictionary<string, object>();
        //        Dictionary<long, IList<AuthorizedUnAuthorizedSubstitutionList_VW>> SubstitutionList = DashboardService.GetSubstitutionReportListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
        //        if (SubstitutionList != null && SubstitutionList.Count > 0)
        //        {
        //            long totalrecords = SubstitutionList.First().Key;
        //            int totalpages = (int)Math.Ceiling(totalrecords / (float)rows);
        //            var jsondat = new
        //            {
        //                total = totalpages,
        //                page = page,
        //                records = totalrecords,
        //                rows = (from items in SubstitutionList.FirstOrDefault().Value
        //                        select new
        //                        {
        //                            i = 2,
        //                            cell = new string[]{
        //                          items.Id.ToString(),
        //                          items.ControlId,
        //                          items.OrderId.ToString(),
        //                          items.Sector,
        //                          items.ContingentType,
        //                          items.Name,
        //                          items.Period,
        //                          items.PeriodYear,
        //                          items.Location,
        //                          items.Week.ToString(),
        //                          items.UNCode.ToString(),
        //                          items.ItemName,
        //                          items.SubstituteItemCode.ToString(),
        //                          items.SubstituteItemName,
        //                          items.Status

        //                      }

        //                        })
        //            };
        //            return Json(jsondat, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //            return null;

        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        #endregion
        #endregion

        #region  CMR TREND chart
        public ActionResult CMRTrendReport()
        {

            return View();
        }

        public ActionResult CMRTrendJQGrid(string searchItems, string ExptType, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                OrdersService us = new OrdersService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();

                if (!string.IsNullOrWhiteSpace(sord) && sord == "desc")
                    sord = "Desc";
                else
                    sord = "Asc";



                if (string.IsNullOrWhiteSpace(searchItems))
                {

                    return null;
                }
                else
                {

                    if (searchItems != null && searchItems != "")
                    {
                        var Items = searchItems.ToString().Split(',');
                        if (!string.IsNullOrWhiteSpace(Items[0])) { criteria.Add("Sector", Items[0]); }
                        if (!string.IsNullOrWhiteSpace(Items[1])) { criteria.Add("Location", Items[1]); }
                        if (!string.IsNullOrWhiteSpace(Items[2])) { criteria.Add("ContingentType", Items[2]); }
                        if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Contingent", Items[3]); }
                        if (!string.IsNullOrWhiteSpace(Items[4])) { criteria.Add("PeriodYear", Items[4]); }
                        if (!string.IsNullOrWhiteSpace(Items[5])) { criteria.Add("Period", Items[5]); }
                        if (!string.IsNullOrWhiteSpace(Items[6]))
                        {
                            criteria.Add("Week", Convert.ToInt64(Items[6]));
                            criteria.Add("ReportType", "ORDINV");
                        }
                    }

                    Dictionary<long, IList<InvoiceReports>> invreport1 = IS.GetInvoiceReportsListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                    IList<InvoiceReports> invreport = invreport1.FirstOrDefault().Value.ToList();

                    if (invreport != null && invreport.Count > 0)
                    {
                        if (ExptType == "Excel")
                        {
                            int i = 1;
                            foreach (var item in invreport)
                            {
                                item.OrderId = i;
                                i = i + 1;
                            }
                            var List = invreport;
                            List<string> lstHeader = new List<string>() { "ControlId", "Sector", "Contingent Type", "Location", "Week", "Authorized CMR", "Order CMR", "Accepted CMR", };
                            base.NewExportToExcel(List, "Deduction Report", (item => new
                            {

                                ControlId = item.ControlId,
                                Sector = item.Sector,
                                ContingentType = item.ContingentType,
                                Location = item.Location,
                                Week = item.Week,
                                OrderQty = item.Authorizedcmr.ToString(),
                                DeliveredQty = item.Ordercmr.ToString(),
                                InvoiceQty = item.Acceptedcmr.ToString(),


                            }), lstHeader);
                            return new EmptyResult();

                        }
                        else
                        {
                            long totalrecords = invreport1.First().Key;
                            int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                            var jsondat = new
                            {
                                total = totalPages,
                                page = page,
                                records = totalrecords,

                                rows = (from items in invreport1.FirstOrDefault().Value
                                        select new
                                        {
                                            i = 2,
                                            cell = new string[] {

                                            items.OrderId.ToString(),
                                            items.ControlId.ToString(),
                                            items.Sector,
                                            items.ContingentType,
                                            items.Location,
                                            items.Week.ToString(),
                                            items.Authorizedcmr.ToString(),
                                            items.Ordercmr.ToString(),
                                            items.Acceptedcmr.ToString(),
                                           
                                            }
                                        })
                            };
                            return Json(jsondat, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        var jsondat = new { rows = (new { cell = new string[] { } }) };
                        return Json(jsondat, JsonRequestBehavior.AllowGet);

                    }

                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }

        public ActionResult GetPeriodYearGraph_CMRTrend()
        {

            IList<CMRTrendReport> cmrreport = new List<CMRTrendReport>();
            cmrreport = DashboardService.GetCMRTrendByFlag("PERIODYEAR", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
            var AuthorizedCMRArray = (from u in cmrreport select new { u.PeriodYear, u.AuthorizedCMR }).ToArray();
            var OrderCMRArray = (from u in cmrreport select new { u.PeriodYear, u.OrderCMR }).ToArray();
            var AcceptedCMRArray = (from u in cmrreport select new { u.PeriodYear, u.AcceptedCMR }).ToArray();
            var PeriodyearArray = (from u in cmrreport select new { u.PeriodYear }).ToArray();

            var PeriodGraph = " <Graph caption='Period Year Based CMR Trend ' xAxisName='Period Year' forceDecimals='1'  rotateLabels='0' valuePadding='0' formatNumberScale='0'   showValues='0' yAxisName='' animation='1' showNames='1'  divlinecolor='d3d3d3' distance='6'  rotateNames='0' rotateValues='1'>";
            PeriodGraph = PeriodGraph + "<categories>";
            foreach (var Periodyear in PeriodyearArray)
            {
                PeriodGraph = PeriodGraph + "<category name='" + Periodyear.PeriodYear + "' />";
            }

            PeriodGraph = PeriodGraph + "</categories>";

            PeriodGraph = PeriodGraph + " <dataset seriesname='Authorized CMR'  color='#84596B' rotateValues='1'>";
            foreach (var authorizedcmr in AuthorizedCMRArray)
            {
                PeriodGraph = PeriodGraph + "<set value='" + authorizedcmr.AuthorizedCMR + "' link=\"JavaScript: MCClickEvent('" + authorizedcmr.PeriodYear + "');" + "\" />";
            }

            PeriodGraph = PeriodGraph + "</dataset>";

            PeriodGraph = PeriodGraph + " <dataset seriesname='Order CMR'   color='#CECFCE'>";
            foreach (var ordercmr in OrderCMRArray)
            {
                PeriodGraph = PeriodGraph + "<set value='" + ordercmr.OrderCMR + "' link=\"JavaScript: MCClickEvent('" + ordercmr.PeriodYear + "');" + "\" />";

            }

            PeriodGraph = PeriodGraph + "</dataset>";

            PeriodGraph = PeriodGraph + " <dataset seriesname='Accepted CMR'   color='#B58AA5' >";
            foreach (var acceptedcmr in AcceptedCMRArray)
            {
                PeriodGraph = PeriodGraph + "<set value='" + acceptedcmr.AcceptedCMR + "' link=\"JavaScript: MCClickEvent('" + acceptedcmr.PeriodYear + "');" + "\" />";

            }

            PeriodGraph = PeriodGraph + "</dataset>";

            PeriodGraph = PeriodGraph + "</Graph>";
            Response.Write(PeriodGraph);
            return null;


        }

        public ActionResult GetPeriodGraph_CMRTrendReport(string PeriodYear)
        {
            //string PeriodYear = "13-14";
            IList<CMRTrendReport> cmrreport = new List<CMRTrendReport>();
            cmrreport = DashboardService.GetCMRTrendByFlag("PERIOD", PeriodYear, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
            var AuthorizedCMRArray = (from u in cmrreport select new { u.PeriodYear, u.Period, u.AuthorizedCMR }).ToArray();
            var OrderCMRArray = (from u in cmrreport select new { u.PeriodYear, u.Period, u.OrderCMR }).ToArray();
            var AcceptedCMRArray = (from u in cmrreport select new { u.PeriodYear, u.Period, u.AcceptedCMR }).ToArray();

            var PeriodArray = (from u in cmrreport select new { u.Period }).ToArray();

            var PeriodGraph = " <graph caption='Periodwise Qty Report for" + PeriodYear + " ' showYAxisValues='0' xAxisName='Period' forceDecimals='0'  rotateLabels='1' formatNumberScale='0'   yAxisName='' animation='1' showNames='1' showValues='0'  divlinecolor='d3d3d3' distance='0'   rotateNames='1'>";
            PeriodGraph = PeriodGraph + "<categories>";
            foreach (var Period in PeriodArray)
            {
                PeriodGraph = PeriodGraph + "<category name='" + Period.Period + "' />";
            }

            PeriodGraph = PeriodGraph + "</categories>";

            PeriodGraph = PeriodGraph + " <dataset seriesname='Authorized CMR'   color='#84596B'>";
            foreach (var authorizedcmr in AuthorizedCMRArray)
            {
                PeriodGraph = PeriodGraph + "<set value='" + authorizedcmr.AuthorizedCMR + "' link=\"JavaScript: PeriodSelection('" + PeriodYear + "," + authorizedcmr.Period + "');" + "\" />";
            }

            PeriodGraph = PeriodGraph + "</dataset>";

            PeriodGraph = PeriodGraph + " <dataset seriesname='Order CMR'   color='#CECFCE'>";
            foreach (var ordercmr in OrderCMRArray)
            {
                PeriodGraph = PeriodGraph + "<set value='" + ordercmr.OrderCMR + "' link=\"JavaScript: PeriodSelection('" + PeriodYear + "," + ordercmr.Period + "');" + "\" />";

            }

            PeriodGraph = PeriodGraph + "</dataset>";

            PeriodGraph = PeriodGraph + " <dataset seriesname='Accepted CMR'   color='#B58AA5' >";
            foreach (var acceptedcmr in AcceptedCMRArray)
            {
                PeriodGraph = PeriodGraph + "<set value='" + acceptedcmr.AcceptedCMR + "' link=\"JavaScript: PeriodSelection('" + PeriodYear + "," + acceptedcmr.Period + "');" + "\" />";

            }

            PeriodGraph = PeriodGraph + "</dataset>";

            PeriodGraph = PeriodGraph + "</graph>";
            Response.Write(PeriodGraph);
            return null;
        }

        //Sector graph qty report
        public ActionResult GetSectorGraph_CMRTrendReport(string PeriodYear, string Period, string Sector)
        {

            IList<CMRTrendReport> cmrreport = new List<CMRTrendReport>();
            cmrreport = DashboardService.GetCMRTrendByFlag("SECTOR", PeriodYear, Period, Sector, string.Empty, string.Empty, string.Empty);
            var AuthorizedCMRArray = (from u in cmrreport select new { u.PeriodYear, u.Period, u.Sector, u.ContingentType, u.AuthorizedCMR }).ToArray();
            var OrderCMRArray = (from u in cmrreport select new { u.PeriodYear, u.Period, u.Sector, u.ContingentType, u.OrderCMR }).ToArray();
            var AcceptedCMRArray = (from u in cmrreport select new { u.PeriodYear, u.Period, u.Sector, u.ContingentType, u.AcceptedCMR }).ToArray();
            var ContingentType = (from u in cmrreport select new { u.ContingentType }).Distinct().ToArray();

            var SectorGraph = " <graph caption='PeriodYear : " + PeriodYear + "   Period : " + Period + "   Sector :  " + Sector + " ' showYAxisValues='0' showXAxisValues='0' xAxisName='Contingent Type' forceDecimals='0'  rotateLabels='' formatNumberScale='0'   yAxisName='' animation='1' showNames='1' showValues='1'  divlinecolor='d3d3d3' distance='0'   rotateNames='1'>";
            SectorGraph = SectorGraph + "<categories>";
            foreach (var conType in ContingentType)
            {
                SectorGraph = SectorGraph + "<category name='" + conType.ContingentType + "' />";
            }

            SectorGraph = SectorGraph + "</categories>";

            SectorGraph = SectorGraph + " <dataset seriesname='Authorized CMR'   showValues='1' color='#84596B'>";
            foreach (var authorizedcmr in AuthorizedCMRArray)
            {
                SectorGraph = SectorGraph + "<set value='" + authorizedcmr.AuthorizedCMR + "' link=\"JavaScript: SectorConttypeSelection('" + authorizedcmr.PeriodYear + "," + authorizedcmr.Period + "," + authorizedcmr.Sector + "," + authorizedcmr.ContingentType + "');" + "\" />";
            }

            SectorGraph = SectorGraph + "</dataset>";

            SectorGraph = SectorGraph + " <dataset seriesname='Order CMR'   color='#CECFCE'>";
            foreach (var ordercmr in OrderCMRArray)
            {
                SectorGraph = SectorGraph + "<set value='" + ordercmr.OrderCMR + "' link=\"JavaScript: SectorConttypeSelection('" + ordercmr.PeriodYear + "," + ordercmr.Period + "," + ordercmr.Sector + "," + ordercmr.ContingentType + "');" + "\" />";

            }

            SectorGraph = SectorGraph + "</dataset>";

            SectorGraph = SectorGraph + " <dataset seriesname='Accepted CMR'   color='#B58AA5' >";
            foreach (var acceptedcmr in AcceptedCMRArray)
            {
                SectorGraph = SectorGraph + "<set value='" + acceptedcmr.AcceptedCMR + "' link=\"JavaScript: SectorConttypeSelection('" + acceptedcmr.PeriodYear + "," + acceptedcmr.Period + "," + acceptedcmr.Sector + "," + acceptedcmr.ContingentType + "');" + "\" />";

            }

            SectorGraph = SectorGraph + "</dataset>";
            SectorGraph = SectorGraph + "</graph>";
            Response.Write(SectorGraph);
            return null;
        }

        //Location graph
        public ActionResult GetLocationGraph_CMRTrendReport(string PeriodYear, string Period, string Sector, string ContingentType)
        {

            IList<CMRTrendReport> cmrreport = new List<CMRTrendReport>();
            cmrreport = DashboardService.GetCMRTrendByFlag("LOCATION", PeriodYear, Period, Sector, ContingentType, string.Empty, string.Empty);
            var AuthorizedCMRArray = (from u in cmrreport select new { u.PeriodYear, u.Period, u.Sector, u.ContingentType, u.Location, u.AuthorizedCMR }).ToArray();
            var OrderCMRArray = (from u in cmrreport select new { u.PeriodYear, u.Period, u.Sector, u.ContingentType, u.Location, u.OrderCMR }).ToArray();
            var AcceptedCMRArray = (from u in cmrreport select new { u.PeriodYear, u.Period, u.Sector, u.ContingentType, u.Location, u.AcceptedCMR }).ToArray();
            var Location = (from u in cmrreport select new { u.Location }).Distinct().ToArray();

            var LocationGraph = " <graph caption='PeriodYear : " + PeriodYear + "   Period : " + Period + "   Sector :  " + Sector + "   ContingentType : " + ContingentType + " ' showYAxisValues='0' showXAxisValues='0' xAxisName='Location' forceDecimals='0'  rotateLabels='' formatNumberScale='0'   yAxisName='' animation='1' showNames='1'   divlinecolor='d3d3d3' distance='0'  showValues='0' rotateNames='0'>";
            LocationGraph = LocationGraph + "<categories>";
            foreach (var location in Location)
            {
                LocationGraph = LocationGraph + "<category name='" + location.Location + "' />";
            }

            LocationGraph = LocationGraph + "</categories>";

            LocationGraph = LocationGraph + " <dataset seriesname='Authorized CMR'   color='#84596B'>";
            foreach (var authorizedcmr in AuthorizedCMRArray)
            {
                LocationGraph = LocationGraph + "<set value='" + authorizedcmr.AuthorizedCMR + "' link=\"JavaScript: LocationSelection('" + authorizedcmr.PeriodYear + "," + authorizedcmr.Period + "," + authorizedcmr.Sector + "," + authorizedcmr.ContingentType + ',' + authorizedcmr.Location + "');" + "\" />";
            }

            LocationGraph = LocationGraph + "</dataset>";

            LocationGraph = LocationGraph + " <dataset seriesname='Order CMR'   color='#CECFCE'>";
            foreach (var ordercmr in OrderCMRArray)
            {
                LocationGraph = LocationGraph + "<set value='" + ordercmr.OrderCMR + "' link=\"JavaScript: LocationSelection('" + ordercmr.PeriodYear + "," + ordercmr.Period + "," + ordercmr.Sector + "," + ordercmr.ContingentType + ',' + ordercmr.Location + "');" + "\" />";

            }

            LocationGraph = LocationGraph + "</dataset>";

            LocationGraph = LocationGraph + " <dataset seriesname='Accepted CMR'   color='#B58AA5' >";
            foreach (var acceptedcmr in AcceptedCMRArray)
            {
                LocationGraph = LocationGraph + "<set value='" + acceptedcmr.AcceptedCMR + "' link=\"JavaScript: LocationSelection('" + acceptedcmr.PeriodYear + "," + acceptedcmr.Period + "," + acceptedcmr.Sector + "," + acceptedcmr.ContingentType + ',' + acceptedcmr.Location + "');" + "\" />";

            }

            LocationGraph = LocationGraph + "</dataset>";
            LocationGraph = LocationGraph + "</graph>";
            Response.Write(LocationGraph);
            return null;
        }

        //Contingent Graph
        public ActionResult GetContingentGraph_CMRTrendReport(string PeriodYear, string Period, string Sector, string ContingentType, string Location)
        {

            IList<CMRTrendReport> cmrreport = new List<CMRTrendReport>();
            cmrreport = DashboardService.GetCMRTrendByFlag("CONTINGENT", PeriodYear, Period, Sector, ContingentType, Location, string.Empty);
            var AuthorizedCMRArray = (from u in cmrreport select new { u.PeriodYear, u.Period, u.Sector, u.ContingentType, u.Location, u.Week, u.Name, u.AuthorizedCMR }).ToArray();
            var OrderCMRArray = (from u in cmrreport select new { u.PeriodYear, u.Period, u.Sector, u.ContingentType, u.Location, u.Week, u.Name, u.OrderCMR }).ToArray();
            var AcceptedCMRArray = (from u in cmrreport select new { u.PeriodYear, u.Period, u.Sector, u.ContingentType, u.Location, u.Week, u.Name, u.AcceptedCMR }).ToArray();
            var Contingent = (from u in cmrreport select new { u.Name }).Distinct().ToArray();

            var ContingentGraph = " <graph caption='PeriodYear : " + PeriodYear + "   Period : " + Period + "   Sector :  " + Sector + "   ContingentType : " + ContingentType + "   Location: " + Location + " ' showYAxisValues='0' showXAxisValues='0' xAxisName='Contingent' forceDecimals='0'  rotateLabels='' formatNumberScale='0'   yAxisName='' animation='1' showNames='1'   divlinecolor='d3d3d3' distance='0'  showValues='1' rotateNames='0'>";
            ContingentGraph = ContingentGraph + "<categories>";
            foreach (var contingent in Contingent)
            {
                ContingentGraph = ContingentGraph + "<category name='" + contingent.Name + "' />";
            }

            ContingentGraph = ContingentGraph + "</categories>";

            ContingentGraph = ContingentGraph + " <dataset seriesname='Authorized CMR'   color='#84596B'>";
            foreach (var authorizedcmr in AuthorizedCMRArray)
            {
                ContingentGraph = ContingentGraph + "<set value='" + authorizedcmr.AuthorizedCMR + "' link=\"JavaScript: ContingentSelection('" + authorizedcmr.PeriodYear + "," + authorizedcmr.Period + "," + authorizedcmr.Sector + "," + authorizedcmr.ContingentType + "," + authorizedcmr.Location + ',' + authorizedcmr.Name + "');" + "\" />";
            }

            ContingentGraph = ContingentGraph + "</dataset>";

            ContingentGraph = ContingentGraph + " <dataset seriesname='Order CMR'   color='#CECFCE'>";
            foreach (var ordercmr in OrderCMRArray)
            {
                ContingentGraph = ContingentGraph + "<set value='" + ordercmr.OrderCMR + "' link=\"JavaScript: ContingentSelection('" + ordercmr.PeriodYear + "," + ordercmr.Period + "," + ordercmr.Sector + "," + ordercmr.ContingentType + "," + ordercmr.Location + ',' + ordercmr.Name + "');" + "\" />";

            }

            ContingentGraph = ContingentGraph + "</dataset>";

            ContingentGraph = ContingentGraph + " <dataset seriesname='Accepted CMR'   color='#B58AA5' >";
            foreach (var acceptedcmr in AcceptedCMRArray)
            {
                ContingentGraph = ContingentGraph + "<set value='" + acceptedcmr.AcceptedCMR + "' link=\"JavaScript: ContingentSelection('" + acceptedcmr.PeriodYear + "," + acceptedcmr.Period + "," + acceptedcmr.Sector + "," + acceptedcmr.ContingentType + "," + acceptedcmr.Location + ',' + acceptedcmr.Name + "');" + "\" />";

            }

            ContingentGraph = ContingentGraph + "</dataset>";
            ContingentGraph = ContingentGraph + "</graph>";
            Response.Write(ContingentGraph);
            return null;
        }

        //Week Graph

        public ActionResult GetWeekGraph_CMRTrendReport(string PeriodYear, string Period, string Sector, string ContingentType, string Location, string Contingent)
        {

            IList<CMRTrendReport> cmrreport = new List<CMRTrendReport>();
            cmrreport = DashboardService.GetCMRTrendByFlag("WEEK", PeriodYear, Period, Sector, ContingentType, Location, Contingent);
            var AuthorizedCMRArray = (from u in cmrreport select new { u.PeriodYear, u.Period, u.Sector, u.ContingentType, u.Location, u.Week, u.AuthorizedCMR }).ToArray();
            var OrderCMRArray = (from u in cmrreport select new { u.PeriodYear, u.Period, u.Sector, u.ContingentType, u.Location, u.Week, u.OrderCMR }).ToArray();
            var AcceptedCMRArray = (from u in cmrreport select new { u.PeriodYear, u.Period, u.Sector, u.ContingentType, u.Location, u.Week, u.AcceptedCMR }).ToArray();
            var Week = (from u in cmrreport select new { u.Week }).Distinct().ToArray();

            var ContingentGraph = " <graph caption='PeriodYear : " + PeriodYear + "   Period : " + Period + "   Sector :  " + Sector + "   ContingentType : " + ContingentType + "   Location: " + Location + "   Contingent : " + Contingent + " ' showYAxisValues='0' showXAxisValues='0' xAxisName='Week' forceDecimals='0'  rotateLabels='' formatNumberScale='0'   yAxisName='' animation='1' showNames='1'   divlinecolor='d3d3d3' distance='0'  showValues='1' rotateNames='0'>";
            ContingentGraph = ContingentGraph + "<categories>";
            foreach (var week in Week)
            {
                ContingentGraph = ContingentGraph + "<category name='" + week.Week + "' />";
            }

            ContingentGraph = ContingentGraph + "</categories>";

            ContingentGraph = ContingentGraph + " <dataset seriesname='Authorized CMR'   color='#84596B'>";
            foreach (var authorizedcmr in AuthorizedCMRArray)
            {
                ContingentGraph = ContingentGraph + "<set value='" + authorizedcmr.AuthorizedCMR + "' />";
            }

            ContingentGraph = ContingentGraph + "</dataset>";

            ContingentGraph = ContingentGraph + " <dataset seriesname='Order CMR'   color='#CECFCE'>";
            foreach (var ordercmr in OrderCMRArray)
            {
                ContingentGraph = ContingentGraph + "<set value='" + ordercmr.OrderCMR + "' />";

            }

            ContingentGraph = ContingentGraph + "</dataset>";

            ContingentGraph = ContingentGraph + " <dataset seriesname='Accepted CMR'   color='#B58AA5' >";
            foreach (var acceptedcmr in AcceptedCMRArray)
            {
                ContingentGraph = ContingentGraph + "<set value='" + acceptedcmr.AcceptedCMR + "' />";

            }

            ContingentGraph = ContingentGraph + "</dataset>";
            ContingentGraph = ContingentGraph + "</graph>";
            Response.Write(ContingentGraph);
            return null;
        }

        #endregion


        #region Dashboard charts

        public ActionResult GetInvoiceProcessed_Dashboard(string Flag)
        {
            IList<InsightHomePageDashboard> dashboardlist = new List<InsightHomePageDashboard>();
            dashboardlist = DashboardService.GetHomePageDashboardByFlag(Flag);

            var FoodArray = (from u in dashboardlist where u.Category == "Food" select new { u.PeriodYear, u.NoOfInvoices, u.Category }).ToArray();
            var TransportArray = (from u in dashboardlist where u.Category == "Transport" select new { u.PeriodYear, u.NoOfInvoices }).ToArray();

            var PeriodYearArray = (from u in dashboardlist select new { u.PeriodYear }).Distinct().ToArray();

            var PeriodGraph = " <Graph caption='Invoice Processed ' xAxisName='Period Year'  showBorder='0' decimalPrecision='0' rotateLabels='0' valuePadding='0' formatNumberScale='0'   yAxisName='' animation='1' showNames='1' showValues='1' borderColor='d3d3d3' divlinecolor='d3d3d3' distance='6'  rotateNames='0'>";
            PeriodGraph = PeriodGraph + "<categories>";
            foreach (var Periodyear in PeriodYearArray)
            {
                PeriodGraph = PeriodGraph + "<category name='" + Periodyear.PeriodYear + "' />";
            }

            PeriodGraph = PeriodGraph + "</categories>";

            PeriodGraph = PeriodGraph + " <dataset seriesname='Food Invoice'  color='#A3C586' rotateValues='1'>";
            foreach (var food in FoodArray)
            {
                PeriodGraph = PeriodGraph + "<set value='" + food.NoOfInvoices + "'/>";
            }

            PeriodGraph = PeriodGraph + "</dataset>";

            PeriodGraph = PeriodGraph + " <dataset seriesname='Transport Invoice'   color='#FFCC33'>";
            foreach (var transport in TransportArray)
            {
                PeriodGraph = PeriodGraph + "<set value='" + transport.NoOfInvoices + "' />";

            }
            PeriodGraph = PeriodGraph + "</dataset>";
            PeriodGraph = PeriodGraph + "</Graph>";
            Response.Write(PeriodGraph);
            return null;
        }

        public ActionResult GetInvoiceValue_Dashboard(string Flag)
        {
            IList<InsightHomePageDashboard> dashboardlist = new List<InsightHomePageDashboard>();
            dashboardlist = DashboardService.GetHomePageDashboardByFlag(Flag);

            var FoodArray = (from u in dashboardlist where u.Category == "Food" select new { u.PeriodYear, u.InvoiceValue, u.Category }).ToArray();
            var TransportArray = (from u in dashboardlist where u.Category == "Transport" select new { u.PeriodYear, u.InvoiceValue, u.Category }).ToArray();

            var PeriodYearArray = (from u in dashboardlist select new { u.PeriodYear }).Distinct().ToArray();

            var PeriodGraph = " <Graph caption='Invoice Values  ' xAxisName='Period Year' decimalPrecision='0'   rotateXAxisName='1' rotateLabels='0' valuePadding='0' formatNumberScale='0'   yAxisName='' animation='1' showNames='1' showValues='1'  divlinecolor='d3d3d3' distance='6'  rotateNames='1'>";
            PeriodGraph = PeriodGraph + "<categories>";
            foreach (var Periodyear in PeriodYearArray)
            {
                PeriodGraph = PeriodGraph + "<category name='" + Periodyear.PeriodYear + "' />";
            }

            PeriodGraph = PeriodGraph + "</categories>";

            PeriodGraph = PeriodGraph + " <dataset seriesname='Food Invoice'  color='#D77A44' rotateValues='1'>";
            foreach (var food in FoodArray)
            {
                PeriodGraph = PeriodGraph + "<set value='" + food.InvoiceValue + "'/>";
            }

            PeriodGraph = PeriodGraph + "</dataset>";

            PeriodGraph = PeriodGraph + " <dataset seriesname='Transport Invoice'   color='#274245'>";
            foreach (var transport in TransportArray)
            {
                PeriodGraph = PeriodGraph + "<set value='" + transport.InvoiceValue + "' />";

            }
            PeriodGraph = PeriodGraph + "</dataset>";
            PeriodGraph = PeriodGraph + "</Graph>";
            Response.Write(PeriodGraph);
            return null;
        }


        public ActionResult GetContingentCount_Dashboard(string Flag)
        {
            IList<InsightHomePageDashboard> dashboardlist = new List<InsightHomePageDashboard>();
            dashboardlist = DashboardService.GetHomePageDashboardByFlag(Flag);

            //var FoodArray = (from u in dashboardlist where u.Category == "Food" select new { u.PeriodYear,u.NoOfContingents }).ToArray();
            //var TransportArray = (from u in dashboardlist where u.Category == "Transport" select new { u.PeriodYear,u.NoOfContingents}).ToArray();

            //var PeriodYearArray = (from u in dashboardlist select new { u.PeriodYear }).Distinct().ToArray();

            var PeriodYearGraph = " <graph caption=' Total Contingents' xAxisName='Period Year' forceDecimals='0' decimalPrecision='0'  formatNumberScale='0'   yAxisName='' animation='1' showNames='1' showValues='1'  divlinecolor='d3d3d3' distance='6'   rotateNames='0'>";
            string[] color = new string[12] { "#993300", "#003366", "#93A8A9", "#D1E6E7", "FFA62F", "E66C2C", "C25A7C", "59E817", "00FF00", "FFA62F", "FF7F50", "FF00FF" };

            int i = 0;
            foreach (var list in dashboardlist)
            {

                // PeriodYearGraph = PeriodYearGraph + " <set name='2013-2014' value='" + Period13to14 + "' color='8B008B' link=\"JavaScript: MCClickEvent('"+a+"');"+"\" />";
                PeriodYearGraph = PeriodYearGraph + " <set name='" + list.PeriodYear + "' color='" + color[i] + "' value='" + list.NoOfContingents + "' />";
                i = i + 1;
            }
            PeriodYearGraph = PeriodYearGraph + "</graph>";
            Response.Write(PeriodYearGraph);

            return null;
        }



        public ActionResult GetTroops_Dashboard(string Flag)
        {
            try
            {
                IList<InsightHomePageDashboard> dashboardlist = new List<InsightHomePageDashboard>();
                dashboardlist = DashboardService.GetHomePageDashboardByFlag(Flag);
                var troops = (from u in dashboardlist where u.PeriodYear == "13-14" select new { u.TroopStrength, u.PeriodYear }).ToArray();
                //Modified by Thamizhmani on 04 Nov 2016
                if (troops.Length > 0)
                {
                    var PeriodYearGraph = "<chart caption='13-14' manageResize='1' bgColor='FFFFFF' bgAlpha='0' showBorder='0' lowerLimit='0' upperLimit='800000' showTickMarks='1' showTickValues='1' showLimits='1' decmials='0' cylFillColor='#8A2BE2' baseFontColor='#000000' chartLeftMargin='10' chartRightMargin='10' chartTopMargin='10'>";
                    PeriodYearGraph = PeriodYearGraph + "<value>" + troops[0].TroopStrength + "</value></chart>";
                    Response.Write(PeriodYearGraph);
                }
                return null;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public ActionResult GetTroops_Dashboard14to15(string Flag)
        {
            try
            {
                IList<InsightHomePageDashboard> dashboardlist = new List<InsightHomePageDashboard>();
                dashboardlist = DashboardService.GetHomePageDashboardByFlag(Flag);
                var troops = (from u in dashboardlist where u.PeriodYear == "14-15" select new { u.TroopStrength, u.PeriodYear }).ToArray();
                //Modified by Thamizhmani on 04 Nov 2016
                if (troops.Length > 0)
                {
                    var PeriodYearGraph = "<chart caption='13-14' manageResize='1' bgColor='FFFFFF' bgAlpha='0' showBorder='0' lowerLimit='0' upperLimit='800000' showTickMarks='1' showTickValues='1' showLimits='1' decmials='0' cylFillColor='#6baa01' baseFontColor='#000000' chartLeftMargin='10' chartRightMargin='10' chartTopMargin='10'>";
                    PeriodYearGraph = PeriodYearGraph + "<value>" + troops[0].TroopStrength + "</value></chart>";
                    Response.Write(PeriodYearGraph);
                }
                return null;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        #endregion

        #region  Loss because of Excess Delivery

        public ActionResult LossBecauseofExcessDelivery()
        {
            return View();

        }

        public ActionResult LossBecauseofExcessDeliveryJQGrid(string searchItems, string ExptType, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                OrdersService us = new OrdersService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();

                if (!string.IsNullOrWhiteSpace(sord) && sord == "desc")
                    sord = "Desc";
                else
                    sord = "Asc";



                if (string.IsNullOrWhiteSpace(searchItems))
                {

                    return null;
                }
                else
                {

                    if (searchItems != null && searchItems != "")
                    {
                        var Items = searchItems.ToString().Split(',');
                        if (!string.IsNullOrWhiteSpace(Items[0])) { criteria.Add("Sector", Items[0]); }
                        if (!string.IsNullOrWhiteSpace(Items[1])) { criteria.Add("Location", Items[1]); }
                        if (!string.IsNullOrWhiteSpace(Items[2])) { criteria.Add("ContingentType", Items[2]); }
                        if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Contingent", Items[3]); }
                        if (!string.IsNullOrWhiteSpace(Items[4])) { criteria.Add("PeriodYear", Items[4]); }
                        if (!string.IsNullOrWhiteSpace(Items[5])) { criteria.Add("Period", Items[5]); }
                        if (!string.IsNullOrWhiteSpace(Items[6]))
                        {
                            criteria.Add("Week", Convert.ToInt64(Items[6]));
                            criteria.Add("ReportType", "ORDINV");
                        }
                    }

                    Dictionary<long, IList<LossBecauseofExcessDelivery>> loss1 = DashboardService.GetLossBecauseofExcessDeliveryListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                    IList<LossBecauseofExcessDelivery> loss = loss1.FirstOrDefault().Value.ToList();

                    if (loss != null && loss.Count > 0)
                    {
                        if (ExptType == "Excel")
                        {
                            int i = 1;
                            foreach (var item in loss)
                            {
                                item.OrderId = i;
                                i = i + 1;
                            }
                            var List = loss;
                            List<string> lstHeader = new List<string>() { "ControlId", "Sector", "Contingent Type", "Location", "Week", "Authorized CMR", "Order CMR", "Accepted CMR", };
                            base.NewExportToExcel(List, "Deduction Report", (item => new
                            {

                                ControlId = item.ControlId,
                                Sector = item.Sector,
                                ContingentType = item.ContingentType,
                                Location = item.Location,
                                Week = item.Week,
                                //OrderQty = item.Authorizedcmr.ToString(),
                                //DeliveredQty = item.Ordercmr.ToString(),
                                //InvoiceQty = item.Acceptedcmr.ToString(),


                            }), lstHeader);
                            return new EmptyResult();

                        }
                        else
                        {
                            long totalrecords = loss1.First().Key;
                            int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                            var jsondat = new
                            {
                                total = totalPages,
                                page = page,
                                records = totalrecords,

                                rows = (from items in loss1.FirstOrDefault().Value
                                        select new
                                        {
                                            i = 2,
                                            cell = new string[] {
                                            items.Id.ToString(),
                                            items.ControlId,
                                            items.DeliveryNoteName,
                                            items.Sector,
                                            items.Name,
                                            items.Location,
                                            items.ContingentType,
                                            items.Week.ToString(),
                                            items.OrderId.ToString(),
                                            items.LineId.ToString(),
                                            items.UNCode.ToString(),
                                            items.Commodity,
                                            items.DeliverySector,
                                            items.OrderQty.ToString(),
                                            items.DeliveredQty.ToString(),
                                            items.InvoiceQty.ToString(),
                                            items.ExcessDeliveryQty.ToString(),
                                            items.Sectorprice.ToString(),
                                            items.AmountOfLoss.ToString()
                                           
                                            }
                                        })
                            };
                            return Json(jsondat, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        var jsondat = new { rows = (new { cell = new string[] { } }) };
                        return Json(jsondat, JsonRequestBehavior.AllowGet);

                    }

                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }

        //Period Year Graph

        public ActionResult GetPeriodYearGraph_ExcessDelivery(string Flag)
        {
            //string PeriodYear = "13-14";
            IList<LossBecauseofExcessDelivery> lossreport = new List<LossBecauseofExcessDelivery>();
            lossreport = DashboardService.GetLossBecauseofExcessDeliveryReportbyFlag(Flag, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
            var PeriodYearGraph = " <graph caption='Period Year based Loss' xAxisName='Period' forceDecimals='0'  rotateLabels='1' formatNumberScale='0'   yAxisName='' animation='1' showNames='1' showValues='1'  divlinecolor='d3d3d3' distance='6'   rotateNames='0'>";
            string[] color = new string[12] { "#0E426C", "#6586A7", "#FFC652", "FB9C6C", "EAFBC5", "D55121", "CFE990", "E66C2C", "E4287C", "#83AE84", "#5F021F", "#546F8B" };
            int i = 0;
            foreach (var PeriodYear in lossreport)
            {

                // PeriodYearGraph = PeriodYearGraph + " <set name='2013-2014' value='" + Period13to14 + "' color='8B008B' link=\"JavaScript: MCClickEvent('"+a+"');"+"\" />";
                PeriodYearGraph = PeriodYearGraph + " <set name='" + PeriodYear.PeriodYear + "' value='" + PeriodYear.AmountOfLoss + "' color='" + color[i] + "' link=\"JavaScript: PeriodSelection('" + PeriodYear.PeriodYear + "');" + "\" />";
                i = i + 1;
            }
            PeriodYearGraph = PeriodYearGraph + "</graph>";
            Response.Write(PeriodYearGraph);
            return null;
        }
        #endregion

        #endregion







    }
}
