using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using INSIGHT.WCFServices;
using INSIGHT.Entities;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using INSIGHT.Entities.InvoiceEntities;
using System.Web.UI.WebControls;
using System.Web.UI;
using INSIGHT.Entities.PDFEntities;
using System.Threading.Tasks;
using System.Web.Mail;
using System.Net.Mail;
using System.Text;
using System.IO;
using System.Data;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System.Configuration;
using INSIGHT.Entities.ReportEntities;
using INSIGHT.Entities.InputUploadEntities;



namespace INSIGHT.Controllers
{
    public class ReportsController : InvoiceController
    {
        InvoiceService IS = new InvoiceService();
        OrdersService OS = new OrdersService();
        Dictionary<string, object> criteria = new Dictionary<string, object>();

        public ActionResult OrdersPerMonthReport()
        {
            try
            {
                int[] years = new int[15];
                DateTime daytime = DateTime.Now;
                int CurYear = daytime.Year;
                ViewBag.CurYear = CurYear;
                CurYear = CurYear - 5;
                for (int i = 0; i < 15; i++)
                {
                    years[i] = CurYear + i;
                }
                ViewBag.years = years;
                return View();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightReportPolicy");
                throw ex;
            }
        }

        public ActionResult OrdersPerMonthReportJqGrid(OrdersPerMonth_vw opm, string ExptType, int? CountYear, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                OrdersService orSrvc = new OrdersService();
                sord = sord == "desc" ? "Desc" : "Asc";
                if (CountYear >= 0)
                    criteria.Add("Year", CountYear);
                if (opm.Jan > 0) criteria.Add("Jan", opm.Jan);
                if (opm.Feb > 0) criteria.Add("Feb", opm.Feb);
                if (opm.Mar > 0) criteria.Add("Mar", opm.Mar);
                if (opm.Apr > 0) criteria.Add("Apr", opm.Apr);
                if (opm.May > 0) criteria.Add("May", opm.May);
                if (opm.Jun > 0) criteria.Add("Jun", opm.Jun);
                if (opm.Jul > 0) criteria.Add("Jul", opm.Jul);
                if (opm.Aug > 0) criteria.Add("Aug", opm.Aug);
                if (opm.Sep > 0) criteria.Add("Sep", opm.Sep);
                if (opm.Oct > 0) criteria.Add("Oct", opm.Oct);
                if (opm.Nov > 0) criteria.Add("Nov", opm.Nov);
                if (opm.Dec > 0) criteria.Add("Dec", opm.Dec);
                Dictionary<long, IList<OrdersPerMonth_vw>> LoginCountList = orSrvc.GetOrdersPerMonth_vwListWithPagingAndCriteria(page - 1, rows, sord, sidx, criteria);
                if (LoginCountList != null && LoginCountList.Count > 0 && LoginCountList.FirstOrDefault().Key > 0 && LoginCountList.FirstOrDefault().Value.Count > 0)
                {
                    if (ExptType == "Excel")
                    {
                        var List = LoginCountList.First().Value.ToList();
                        List<string> lstHeader = new List<string>() { "Id", "Year", "January", "February", "March", "Apirl", "May", "June", "July", "Augest", "September", "October", "November", "December" };

                        base.NewExportToExcel(List, "OrdersPerMonthReportList", (items => new
                        {
                            items.Id,
                            items.Year,
                            items.Jan,
                            items.Feb,
                            items.Mar,
                            items.Apr,
                            items.May,
                            items.Jun,
                            items.Jul,
                            items.Aug,
                            items.Sep,
                            items.Oct,
                            items.Nov,
                            items.Dec,
                        }), lstHeader);
                        return new EmptyResult();
                    }
                    else
                    {
                        long totalRecords = LoginCountList.First().Key;
                        int totalPages = (int)Math.Ceiling(totalRecords / (float)rows);
                        var jsonData = new
                        {
                            total = totalPages,
                            page = page,
                            records = totalRecords,
                            rows = (
                            from items in LoginCountList.First().Value
                            select new
                            {
                                i = items.Id,
                                cell = new string[] { 
                                items.Id.ToString(), 
                                items.Year.ToString(), 
                                items.Jan.ToString(), 
                                items.Feb.ToString(), 
                                items.Mar.ToString(), 
                                items.Apr.ToString(), 
                                items.May.ToString(), 
                                items.Jun.ToString(), 
                                items.Jul.ToString(), 
                                items.Aug.ToString(), 
                                items.Sep.ToString(),
                                items.Oct.ToString(),
                                items.Nov.ToString(),
                                items.Dec.ToString()
                                }
                            })
                        };
                        return Json(jsonData, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightReportPolicy");
                throw ex;
            }
        }

        public ActionResult GetOrdersPerMonthReportChart(int? CountYear)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    OrdersService os = new OrdersService();
                    Dictionary<string, object> Criteria = new Dictionary<string, object>();
                    if (CountYear >= 0)
                        Criteria.Add("Year", CountYear);
                    Dictionary<long, IList<OrdersPerMonth_vw>> OrdersPerMonth_vw = os.GetOrdersPerMonth_vwListWithPagingAndCriteria(null, 9999, string.Empty, string.Empty, Criteria);
                    var OrdersCount = (from u in OrdersPerMonth_vw.First().Value
                                       select u).ToList();
                    var Jan = 0; var Feb = 0; var Mar = 0; var Apr = 0; var May = 0; var Jun = 0; var Jul = 0; var Aug = 0; var Sep = 0; var Oct = 0; var Nov = 0; var Dec = 0;
                    foreach (var itemdata in OrdersCount)
                    {
                        Jan = Jan + itemdata.Jan;
                        Feb = Feb + itemdata.Feb;
                        Mar = Mar + itemdata.Mar;
                        Apr = Apr + itemdata.Apr;
                        May = May + itemdata.May;
                        Jun = Jun + itemdata.Jun;
                        Jul = Jul + itemdata.Jul;
                        Aug = Aug + itemdata.Aug;
                        Sep = Sep + itemdata.Sep;
                        Oct = Oct + itemdata.Oct;
                        Nov = Nov + itemdata.Nov;
                        Dec = Dec + itemdata.Dec;

                    }
                    var OrderChart = "<graph caption='' xAxisName='Month' yAxisName='Orders Count' decimalPrecision='0' formatNumberScale='0' showNames='1' rotateNames='1'>";
                    OrderChart = OrderChart + " <set name='January' value='" + Jan + "' color='AFD8F8' />";
                    OrderChart = OrderChart + " <set name='February' value='" + Feb + "' color='F6BD0F' />";
                    OrderChart = OrderChart + " <set name='March' value='" + Mar + "' color='8BBA00' />";
                    OrderChart = OrderChart + " <set name='April' value='" + Apr + "' color='FF8E46' />";
                    OrderChart = OrderChart + " <set name='May' value='" + May + "' color='08E8E' />";
                    OrderChart = OrderChart + " <set name='June' value='" + Jun + "' color='D64646' />";
                    OrderChart = OrderChart + " <set name='July' value='" + Jul + "' color='8BBA00' />";
                    OrderChart = OrderChart + " <set name='August' value='" + Aug + "' color='FF8E46' />";
                    OrderChart = OrderChart + " <set name='September' value='" + Sep + "' color='08E8EA' />";
                    OrderChart = OrderChart + " <set name='October' value='" + Oct + "' color='D64646' />";
                    OrderChart = OrderChart + " <set name='November' value='" + Nov + "' color='08A8EA' />";
                    OrderChart = OrderChart + " <set name='December' value='" + Dec + "' color='08E0E5' /></graph>";
                    Response.Write(OrderChart);
                    return null;
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightReportPolicy");
                throw ex;
            }
        }


        public ActionResult OrderItemsReport()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightReportPolicy");
                throw ex;
            }
        }

        public ActionResult OrderItemsReportCountJqGrid(OrderItemsCountAndMismatchCount_vw ord, string ExptType, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                OrdersService os = new OrdersService();
                sord = sord == "desc" ? "Desc" : "Asc";
                if (!string.IsNullOrWhiteSpace(ord.Sector)) { criteria.Add("Sector", ord.Sector); }
                if (!string.IsNullOrWhiteSpace(ord.Contingent)) { criteria.Add("Contingent", ord.Contingent); }
                if (!string.IsNullOrWhiteSpace(ord.Period)) { criteria.Add("Period", ord.Period); }
                Dictionary<long, IList<OrderItemsCountAndMismatchCount_vw>> OrderItemsCount = os.GetOrderItemsCountListWithPagingAndCriteria(page - 1, rows, sord, sidx, criteria);
                if (OrderItemsCount != null && OrderItemsCount.Count > 0 && OrderItemsCount.FirstOrDefault().Key > 0 && OrderItemsCount.FirstOrDefault().Value.Count > 0)
                {
                    if (ExptType == "Excel")
                    {
                        var List = OrderItemsCount.First().Value.ToList();
                        List<string> lstHeader = new List<string>() { "Id", "Sector", "Continget", "Period", "Order Id", "Line Items Orderd", "Actual Count" };
                        base.NewExportToExcel(List, "OrderItemsCount", (items => new
                        {
                            items.Id,
                            items.Sector,
                            items.Contingent,
                            items.Period,
                            items.OrderId,
                            LineItemsOrdered = items.LineItemsOrdered.Value,
                            ActualItems = items.ActualItems.Value.ToString()
                        }), lstHeader);
                        return new EmptyResult();
                    }
                    else
                    {
                        long totalRecords = OrderItemsCount.First().Key;
                        int totalPages = (int)Math.Ceiling(totalRecords / (float)rows);
                        var jsonData = new
                        {
                            total = totalPages,
                            page = page,
                            records = totalRecords,
                            rows = (
                            from items in OrderItemsCount.First().Value
                            select new
                            {
                                i = items.Id,
                                cell = new string[] { 
                                items.Id.ToString(), 
                                items.Sector,items.Contingent,items.Period, items.OrderId.ToString(),
                                items.LineItemsOrdered.ToString(), 
                                items.ActualItems.ToString(),
                                items.LineItemsOrdered!=items.ActualItems?"<span style=color:red>Mismatch</span>":"<span style=color:blue>Same</span>"
                                    }
                            })
                        };
                        return Json(jsonData, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightReportPolicy");
                throw ex;
            }
        }

        public JsonResult MismatchedCountForChart(string Sector, string Contingent, string Period)
        {
            try
            {
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                OrdersService os = new OrdersService();
                if (!string.IsNullOrWhiteSpace(Sector)) { criteria.Add("Sector", Sector); }
                if (!string.IsNullOrWhiteSpace(Contingent)) { criteria.Add("Contingent", Contingent); }
                if (!string.IsNullOrWhiteSpace(Period)) { criteria.Add("Period", Period); }
                Dictionary<long, IList<OrderItemsCountAndMismatchCount_vw>> OrderItemsCount = os.GetOrderItemsCountListWithPagingAndCriteria(0, 9999, "", "", criteria);

                var OrdersCount = (from u in OrderItemsCount.First().Value
                                   where u.LineItemsOrdered != u.ActualItems
                                   select u).ToList();

                var OrderChart = "<graph caption='' xAxisName='' yAxisName='Mismatch Count' decimalPrecision='0' formatNumberScale='0' showNames='1' rotateNames='1'>";
                OrderChart = OrderChart + " <set name='' value='" + OrdersCount.Count() + "' color='AFD8F8' /></graph>";
                Response.Write(OrderChart);
                return null;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightReportPolicy");
                throw ex;
            }
        }


        public ActionResult PODItemsCountReport()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightReportPolicy");
                throw ex;
            }
        }

        public ActionResult PODItemsCountJqGrid(PODItemsCount_vw ord, string ExptType, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                OrdersService os = new OrdersService();
                sord = sord == "desc" ? "Desc" : "Asc";
                if (!string.IsNullOrWhiteSpace(ord.Sector)) { criteria.Add("Sector", ord.Sector); }
                if (!string.IsNullOrWhiteSpace(ord.Contingent)) { criteria.Add("Contingent", ord.Contingent); }
                if (!string.IsNullOrWhiteSpace(ord.Period)) { criteria.Add("Period", ord.Period); }
                Dictionary<long, IList<PODItemsCount_vw>> OrderItemsCount = os.GetPOdItemsCount_vwListWithPagingAndCriteria(page - 1, rows, sord, sidx, criteria);
                if (OrderItemsCount != null && OrderItemsCount.Count > 0 && OrderItemsCount.FirstOrDefault().Key > 0 && OrderItemsCount.FirstOrDefault().Value.Count > 0)
                {
                    if (ExptType == "Excel")
                    {
                        var List = OrderItemsCount.First().Value.ToList();
                        List<string> lstHeader = new List<string>() { "Id", "Sector", "Continget", "Period", "Pod Id", "Items Count" };
                        base.NewExportToExcel(List, "POdItemsCount", (items => new
                        {
                            items.Id,
                            items.Sector,
                            items.Contingent,
                            items.Period,
                            PODId = items.PODId.Value,
                            ItemsCount = items.ItemsCount.Value
                        }), lstHeader);
                        return new EmptyResult();
                    }
                    else
                    {
                        long totalRecords = OrderItemsCount.First().Key;
                        int totalPages = (int)Math.Ceiling(totalRecords / (float)rows);
                        var jsonData = new
                        {
                            total = totalPages,
                            page = page,
                            records = totalRecords,
                            rows = (
                            from items in OrderItemsCount.First().Value
                            select new
                            {
                                i = items.Id,
                                cell = new string[] { 
                                items.Id.ToString(), 
                                items.Sector,items.Contingent,items.Period, items.PODId.ToString(),
                                items.ItemsCount.ToString()
                                    }
                            })
                        };
                        return Json(jsonData, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightReportPolicy");
                throw ex;
            }
        }


        public ActionResult InvoiceCountReport()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightReportPolicy");
                throw ex;
            }
        }

        public ActionResult InvoiceCountReportJqGrid(InvoiceItems Inv, string ExptType, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                InvoiceService invSrvc = new InvoiceService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                if (Inv.InvoiceId > 0) criteria.Add("InvoiceId", Inv.InvoiceId);
                sord = sord == "desc" ? "Desc" : "Asc";
                Dictionary<long, IList<InvoiceItems>> OrderItemsCount = invSrvc.GetInvoiceItemsListWithPagingAndCriteria(page - 1, rows, sord, sidx, criteria);
                if (OrderItemsCount != null && OrderItemsCount.Count > 0 && OrderItemsCount.FirstOrDefault().Key > 0 && OrderItemsCount.FirstOrDefault().Value.Count > 0)
                {
                    var Rows = (
                    from items in OrderItemsCount.First().Value
                    group items by new { items.InvoiceId } into grp
                    select new
                    {
                        cell = new string[] { 
                        grp.Key.InvoiceId.ToString(),
                        grp.Count().ToString(),
                        }
                    });
                    if (ExptType == "Excel")
                    {
                        var List = Rows.ToList();
                        List<string> lstHeader = new List<string>() { "Invoice Id", "Invoice Items Count" };
                        base.NewExportToExcel(List, "InvoiceItemsCount", (items => new
                        {
                            InvoiceId = items.cell[0],
                            InvoiceItemsCount = items.cell[1],
                        }), lstHeader);
                        return new EmptyResult();
                    }
                    else
                    {
                        long totalRecords = Rows.Count();
                        int totalPages = (int)Math.Ceiling(totalRecords / (float)rows);
                        var jsonData = new
                        {
                            total = totalPages,
                            page = page,
                            records = totalRecords,
                            rows = Rows
                        };
                        return Json(jsonData, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightReportPolicy");
                throw ex;
            }
        }

        #region Added  by Arun Orderitems report

        public ActionResult OrderItemsReport_vw()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightReportPolicy");
                throw ex;
            }
        }
        #endregion

        #region WeekWiseReport Added by Arun


        public ActionResult WeekWiseReport()
        {
            return View();
        }

        public ActionResult WeekWiseReportjqGrid(string searchItems, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                OrdersService ords = new OrdersService();
                // Dictionary<string, object> criteria = new Dictionary<string, object>();

                sord = sord == "desc" ? "Desc" : "Asc";

                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    if (searchItems == null)
                    {
                        return null;
                    }
                    else
                    {
                        Dictionary<string, object> criteria = new Dictionary<string, object>();
                        sord = sord == "desc" ? "Desc" : "Asc";

                        criteria.Clear();
                        if (searchItems != null && searchItems != "")
                        {
                            var Items = searchItems.ToString().Split(',');
                            if (!string.IsNullOrWhiteSpace(Items[0])) { criteria.Add("Sector", Items[0]); }
                            if (!string.IsNullOrWhiteSpace(Items[1])) { criteria.Add("ContingentType", Items[1]); }
                            if (!string.IsNullOrWhiteSpace(Items[2])) { criteria.Add("Name", Items[2]); }
                            if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Period", Items[3]); }
                            if (!string.IsNullOrWhiteSpace(Items[4])) { criteria.Add("PeriodYear", Items[4]); }
                            //if (!string.IsNullOrWhiteSpace(Items[4])) { criteria.Add("Week", Convert.ToInt64(Items[4])); }
                            var TrimItems = searchItems.Replace(",", "");
                            if (string.IsNullOrWhiteSpace(TrimItems))
                                return null;
                        }

                        OrdersService orderService = new OrdersService();
                        Dictionary<long, IList<Vw_DistinctValuesFOP>> FOPList = orderService.GetWeekWiseFOPListtWithPagingAndCriteria(page - 1, 99999, string.Empty, string.Empty, criteria);

                        IList<Vw_WeekWiseFinalOutPut> FinalOutputList = orderService.GetWeekWiseFinalOutputListtWithPagingAndCriteria(page - 1, 99999, string.Empty, string.Empty, criteria);


                        IEnumerable<Vw_WeekWiseFinalOutPut> WWFOP;

                        for (int i = 0; i < FOPList.FirstOrDefault().Value.Count; i++)
                        {
                            WWFOP = from num in FinalOutputList
                                    where num.Name == FOPList.FirstOrDefault().Value[i].Name &&
                                    num.Location == FOPList.FirstOrDefault().Value[i].Location &&
                                    num.Sector == FOPList.FirstOrDefault().Value[i].Sector &&
                                    num.Period == FOPList.FirstOrDefault().Value[i].Period &&
                                    num.ContingentType == FOPList.FirstOrDefault().Value[i].ContingentType &&
                                    num.PeriodYear == FOPList.FirstOrDefault().Value[i].PeriodYear
                                    select num;

                            foreach (Vw_WeekWiseFinalOutPut GetVal in WWFOP)
                            {
                                if (GetVal.Rank == 1)
                                {
                                    FOPList.FirstOrDefault().Value[i].Week1 = GetVal.Week;
                                    FOPList.FirstOrDefault().Value[i].OrderQty1 = GetVal.OrderQty;
                                    FOPList.FirstOrDefault().Value[i].DeliveredQty1 = GetVal.DeliveredQty;
                                    FOPList.FirstOrDefault().Value[i].AcceptedQty1 = GetVal.AcceptedQty;
                                    FOPList.FirstOrDefault().Value[i].InvoiceQty1 = GetVal.InvoiceQty;
                                    FOPList.FirstOrDefault().Value[i].InvoiceValue1 = GetVal.InvoiceValue;
                                }
                                else if (GetVal.Rank == 2)
                                {
                                    FOPList.FirstOrDefault().Value[i].Week2 = GetVal.Week;
                                    FOPList.FirstOrDefault().Value[i].OrderQty2 = GetVal.OrderQty;
                                    FOPList.FirstOrDefault().Value[i].DeliveredQty2 = GetVal.DeliveredQty;
                                    FOPList.FirstOrDefault().Value[i].AcceptedQty2 = GetVal.AcceptedQty;
                                    FOPList.FirstOrDefault().Value[i].InvoiceQty2 = GetVal.InvoiceQty;
                                    FOPList.FirstOrDefault().Value[i].InvoiceValue2 = GetVal.InvoiceValue;
                                }
                                else if (GetVal.Rank == 3)
                                {
                                    FOPList.FirstOrDefault().Value[i].Week3 = GetVal.Week;
                                    FOPList.FirstOrDefault().Value[i].OrderQty3 = GetVal.OrderQty;
                                    FOPList.FirstOrDefault().Value[i].DeliveredQty3 = GetVal.DeliveredQty;
                                    FOPList.FirstOrDefault().Value[i].AcceptedQty3 = GetVal.AcceptedQty;
                                    FOPList.FirstOrDefault().Value[i].InvoiceQty3 = GetVal.InvoiceQty;
                                    FOPList.FirstOrDefault().Value[i].InvoiceValue3 = GetVal.InvoiceValue;
                                }
                                else if (GetVal.Rank == 4)
                                {
                                    FOPList.FirstOrDefault().Value[i].Week4 = GetVal.Week;
                                    FOPList.FirstOrDefault().Value[i].OrderQty4 = GetVal.OrderQty;
                                    FOPList.FirstOrDefault().Value[i].DeliveredQty4 = GetVal.DeliveredQty;
                                    FOPList.FirstOrDefault().Value[i].AcceptedQty4 = GetVal.AcceptedQty;
                                    FOPList.FirstOrDefault().Value[i].InvoiceQty4 = GetVal.InvoiceQty;
                                    FOPList.FirstOrDefault().Value[i].InvoiceValue4 = GetVal.InvoiceValue;
                                }
                            }
                        }

                        if (FOPList != null && FOPList.Count > 0)
                        {
                            long totalrecords = FOPList.First().Key;
                            int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                            var jsondat = new
                            {
                                total = totalPages,
                                page = page,
                                records = totalrecords,

                                rows = (from items in FOPList.First().Value
                                        select new
                                        {
                                            i = 2,
                                            cell = new string[] {
                                        items.Id.ToString(),
                                        items.Name,
                                        items.Sector,
                                        items.Period,
                                        items.ContingentType,
                                        items.PeriodYear,
                                        // items.SubstituteItemCode==0?"":items.SubstituteItemCode.ToString(),
                                        items.Week1==0?"":items.Week1.ToString(),
                                        items.OrderQty1==0?"":items.OrderQty1.ToString(),
                                        items.DeliveredQty1==0?"":items.DeliveredQty1.ToString(),
                                        items.AcceptedQty1==0?"":items.AcceptedQty1.ToString(),
                                        items.InvoiceQty1==0?"":items.InvoiceQty1.ToString(),
                                        items.InvoiceValue1==0?"":items.InvoiceValue1.ToString(),
                                        items.Week2==0?"":items.Week2.ToString(),
                                        items.OrderQty2==0?"":items.OrderQty2.ToString(),
                                        items.DeliveredQty2==0?"":items.DeliveredQty2.ToString(),
                                        items.AcceptedQty2==0?"":items.AcceptedQty2.ToString(),
                                        items.InvoiceQty2==0?"":items.InvoiceQty2.ToString(),
                                        items.InvoiceValue2==0?"":items.InvoiceValue2.ToString(),
                                        items.Week3==0?"":items.Week3.ToString(),
                                        items.OrderQty3==0?"":items.OrderQty3.ToString(),
                                        items.DeliveredQty3==0?"":items.DeliveredQty3.ToString(),
                                        items.AcceptedQty3==0?"":items.AcceptedQty3.ToString(),
                                        items.InvoiceQty3==0?"":items.InvoiceQty3.ToString(),
                                        items.InvoiceValue3==0?"":items.InvoiceValue3.ToString(),
                                        items.Week4==0?"":items.Week4.ToString(),
                                        items.OrderQty4==0?"":items.OrderQty4.ToString(),
                                        items.DeliveredQty4==0?"":items.DeliveredQty4.ToString(),
                                        items.AcceptedQty4==0?"":items.AcceptedQty4.ToString(),
                                        items.InvoiceQty4==0?"":items.InvoiceQty4.ToString(),
                                        items.InvoiceValue4==0?"":items.InvoiceValue4.ToString(),

                                          }
                                        })
                            };
                            return Json(jsondat, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var jsondat = new { rows = (new { cell = new string[] { } }) };
                            return Json(jsondat, JsonRequestBehavior.AllowGet);

                        }

                        // return null;
                    }
                }
            }


            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }






        //public ActionResult WeekWiseReportjqGrid(string searchItems, string Id, string Name, string Sector, string Period, string ContingentType, string Week1, string OrderQty1, string DeliveredQty1, string AcceptedQty1, string InvoiceValue1, string Week2, string OrderQty2, string DeliveredQty2, string AcceptedQty2, string InvoiceValue2, string Week3, string OrderQty3, string DeliveredQty3, string AcceptedQty3, string InvoiceValue3, string Week4, string OrderQty4, string DeliveredQty4, string AcceptedQty4, string InvoiceValue4, string ExptType, int rows, string sidx, string sord, int? page = 1)
        //{
        //    try
        //    {
        //        OrdersService ords = new OrdersService();
        //        string userId = base.ValidateUser();
        //        if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
        //        else
        //        {

        //            Dictionary<string, object> criteria = new Dictionary<string, object>();
        //            sord = sord == "desc" ? "Desc" : "Asc";

        //            criteria.Clear();
        //            if (searchItems != null && searchItems != "")
        //            {
        //                var Items = searchItems.ToString().Split(',');
        //                if (!string.IsNullOrWhiteSpace(Items[0])) { criteria.Add("Sector", Items[0]); }
        //                if (!string.IsNullOrWhiteSpace(Items[1])) { criteria.Add("ContingentType", Items[1]); }
        //                if (!string.IsNullOrWhiteSpace(Items[2])) { criteria.Add("Name", Items[2]); }
        //                if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Period", Items[3]); }
        //                //if (!string.IsNullOrWhiteSpace(Items[4])) { criteria.Add("Week", Convert.ToInt64(Items[4])); }
        //            }
        //            if (!string.IsNullOrEmpty(Id))
        //                criteria.Add("Id", Convert.ToInt64(Id));
        //            if (!string.IsNullOrEmpty(Name))
        //                criteria.Add("Name", Name);
        //            if (!string.IsNullOrEmpty(Sector))
        //                criteria.Add("Sector", Sector);
        //            if (!string.IsNullOrEmpty(Period))
        //                criteria.Add("Period", Period);
        //            if (!string.IsNullOrEmpty(ContingentType))
        //                criteria.Add("ContingentType", ContingentType);
        //            if (!string.IsNullOrEmpty(Week1))
        //                criteria.Add("Week1", Week1);
        //            if (!string.IsNullOrEmpty(OrderQty1))
        //                criteria.Add("OrderQty1", OrderQty1);
        //            if (!string.IsNullOrEmpty(DeliveredQty1))
        //                criteria.Add("DeliveredQty1", Convert.ToInt64(DeliveredQty1));
        //            if (!string.IsNullOrEmpty(AcceptedQty1))
        //                criteria.Add("AcceptedQty1", Convert.ToInt64(AcceptedQty1));
        //            if (!string.IsNullOrEmpty(InvoiceValue1))
        //                criteria.Add("InvoiceValue1", Convert.ToInt64(InvoiceValue1));
        //            if (!string.IsNullOrEmpty(Week2))
        //                criteria.Add("Week2", Week2);
        //            if (!string.IsNullOrEmpty(OrderQty2))
        //                criteria.Add("OrderQty2", OrderQty2);
        //            if (!string.IsNullOrEmpty(DeliveredQty2))
        //                criteria.Add("DeliveredQty2", Convert.ToInt64(DeliveredQty2));
        //            if (!string.IsNullOrEmpty(AcceptedQty2))
        //                criteria.Add("AcceptedQty2", Convert.ToInt64(AcceptedQty2));
        //            if (!string.IsNullOrEmpty(InvoiceValue2))
        //                criteria.Add("InvoiceValue2", Convert.ToInt64(InvoiceValue2));
        //            if (!string.IsNullOrEmpty(Week3))
        //                criteria.Add("Week3", Week3);
        //            if (!string.IsNullOrEmpty(OrderQty3))
        //                criteria.Add("OrderQty3", OrderQty3);
        //            if (!string.IsNullOrEmpty(DeliveredQty3))
        //                criteria.Add("DeliveredQty3", Convert.ToInt64(DeliveredQty3));
        //            if (!string.IsNullOrEmpty(AcceptedQty3))
        //                criteria.Add("AcceptedQty3", Convert.ToInt64(AcceptedQty3));
        //            if (!string.IsNullOrEmpty(InvoiceValue3))
        //                criteria.Add("InvoiceValue3", Convert.ToInt64(InvoiceValue3));
        //            if (!string.IsNullOrEmpty(Week4))
        //                criteria.Add("Week4", Week4);
        //            if (!string.IsNullOrEmpty(OrderQty4))
        //                criteria.Add("OrderQty4", OrderQty4);
        //            if (!string.IsNullOrEmpty(DeliveredQty4))
        //                criteria.Add("DeliveredQty4", Convert.ToInt64(DeliveredQty4));
        //            if (!string.IsNullOrEmpty(AcceptedQty4))
        //                criteria.Add("AcceptedQty4", Convert.ToInt64(AcceptedQty4));
        //            if (!string.IsNullOrEmpty(InvoiceValue4))
        //                criteria.Add("InvoiceValue4", Convert.ToInt64(InvoiceValue4));

        //            Dictionary<long, IList<ContingentWeekWiseReport_vw>> WeekList = ords.GetWeekWiseReportListWithPagingAndCriteria(page - 1, rows, sord, sidx, criteria);
        //            if (WeekList != null && WeekList.FirstOrDefault().Value != null && WeekList.FirstOrDefault().Value.Count > 0)
        //            {
        //                if (ExptType == "Excel")
        //                {
        //                    var List = WeekList.First().Value.ToList();
        //                    base.NewExportToExcel(List, "OrdersReportList", (items => new
        //                    {
        //                        items.Id,
        //                        Name = items.Name,
        //                        Sector = items.Sector,
        //                        Period = items.Period,
        //                        ContingentType = items.ContingentType,

        //                        Week1 = items.Week1,
        //                        OrderQty1 = items.OrderQty1.ToString(),
        //                        DeliveredQty1 = items.DeliveredQty1.ToString(),
        //                        AcceptedQty1 = items.AcceptedQty1.ToString(),
        //                        InvoiceValue1 = items.InvoiceValue1.ToString(),
        //                        Week2 = items.Week2,
        //                        OrderQty2 = items.OrderQty2.ToString(),
        //                        DeliveredQty2 = items.DeliveredQty2.ToString(),
        //                        AcceptedQty2 = items.AcceptedQty2.ToString(),
        //                        InvoiceValue2 = items.InvoiceValue2.ToString(),
        //                        Week3 = items.Week3,
        //                        OrderQty3 = items.OrderQty3.ToString(),
        //                        DeliveredQty3 = items.DeliveredQty3.ToString(),
        //                        AcceptedQty3 = items.AcceptedQty3.ToString(),
        //                        InvoiceValue3 = items.InvoiceValue3.ToString(),
        //                        Week4 = items.Week4,
        //                        OrderQty4 = items.OrderQty4.ToString(),
        //                        DeliveredQty4 = items.DeliveredQty4.ToString(),
        //                        AcceptedQty4 = items.AcceptedQty4.ToString(),
        //                        InvoiceValue4 = items.InvoiceValue4.ToString()
        //                    }));
        //                    return new EmptyResult();
        //                }
        //                else
        //                {

        //                    long totalrecords = WeekList.FirstOrDefault().Key;
        //                    int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
        //                    var jsondat = new
        //                    {
        //                        total = totalPages,
        //                        page = page,
        //                        records = totalrecords,

        //                        rows = (from items in WeekList.FirstOrDefault().Value
        //                                select new
        //                                {
        //                                    i = 2,
        //                                    cell = new string[] {
        //                    items.Id.ToString(),
        //                    items.Name,
        //                    items.Sector,
        //                    items.Period,
        //                    items.ContingentType,
        //                  //  items.SubstituteItemCode==0?"":items.SubstituteItemCode.ToString(),
        //                    items.Week1==0?"":items.Week1.ToString(),
        //                    items.OrderQty1==0?"":items.OrderQty1.ToString(),
        //                    items.DeliveredQty1==0?"":items.DeliveredQty1.ToString(),
        //                    items.AcceptedQty1==0?"":items.AcceptedQty1.ToString(),
        //                    items.InvoiceQty1==0?"":items.InvoiceQty1.ToString(),
        //                    items.InvoiceValue1==0?"":items.InvoiceValue1.ToString(),
        //                    items.Week2==0?"":items.Week2.ToString(),
        //                    items.OrderQty2==0?"":items.OrderQty2.ToString(),
        //                    items.DeliveredQty2==0?"":items.DeliveredQty2.ToString(),
        //                    items.AcceptedQty2==0?"":items.AcceptedQty2.ToString(),
        //                      items.InvoiceQty2==0?"":items.InvoiceQty2.ToString(),
        //                    items.InvoiceValue2==0?"":items.InvoiceValue2.ToString(),
        //                    items.Week3==0?"":items.Week3.ToString(),
        //                    items.OrderQty3==0?"":items.OrderQty3.ToString(),
        //                    items.DeliveredQty3==0?"":items.DeliveredQty3.ToString(),
        //                    items.AcceptedQty3==0?"":items.AcceptedQty3.ToString(),
        //                      items.InvoiceQty3==0?"":items.InvoiceQty3.ToString(),
        //                    items.InvoiceValue3==0?"":items.InvoiceValue3.ToString(),
        //                    items.Week4==0?"":items.Week4.ToString(),
        //                    items.OrderQty4==0?"":items.OrderQty4.ToString(),
        //                    items.DeliveredQty4==0?"":items.DeliveredQty4.ToString(),
        //                    items.AcceptedQty4==0?"":items.AcceptedQty4.ToString(),
        //                      items.InvoiceQty4==0?"":items.InvoiceQty4.ToString(),
        //                    items.InvoiceValue4==0?"":items.InvoiceValue4.ToString(),

        //                    }
        //                                })
        //                    };
        //                    return Json(jsondat, JsonRequestBehavior.AllowGet);
        //                }
        //            }
        //        }
        //        return Json(null, JsonRequestBehavior.AllowGet);

        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
        //        throw ex;

        //    }

        //}


        #endregion

        #region Week Wise Excel Report By Gobi
        public ActionResult WeekWiseReportExcel(string Sector, string ContingentType, string Name, string Period, int rows, string sidx, string sord, int? page = 1)
        {
            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
            {
                try
                {
                    string title = "WeekWiseReport";
                    OrdersService ords = new OrdersService();
                    Dictionary<string, object> criteria = new Dictionary<string, object>();
                    sord = sord == "desc" ? "Desc" : "Asc";
                    criteria.Clear();

                    //*************************
                    if (!string.IsNullOrWhiteSpace(Sector)) { criteria.Add("Sector", Sector); }
                    if (!string.IsNullOrWhiteSpace(ContingentType)) { criteria.Add("ContingentType", ContingentType); }
                    if (!string.IsNullOrWhiteSpace(Name)) { criteria.Add("Name", Name); }
                    if (!string.IsNullOrWhiteSpace(Period)) { criteria.Add("Period", Period); }


                    OrdersService orderService = new OrdersService();
                    Dictionary<long, IList<Vw_DistinctValuesFOP>> FOPList = orderService.GetWeekWiseFOPListtWithPagingAndCriteria(page - 1, 99999, string.Empty, string.Empty, criteria);
                    IList<Vw_WeekWiseFinalOutPut> FinalOutputList = orderService.GetWeekWiseFinalOutputListtWithPagingAndCriteria(page - 1, 99999, string.Empty, string.Empty, criteria);
                    IEnumerable<Vw_WeekWiseFinalOutPut> WWFOP;
                    for (int i = 0; i < FOPList.FirstOrDefault().Value.Count; i++)
                    {
                        WWFOP = from num in FinalOutputList
                                where num.Name == FOPList.FirstOrDefault().Value[i].Name &&
                                num.Location == FOPList.FirstOrDefault().Value[i].Location &&
                                num.Sector == FOPList.FirstOrDefault().Value[i].Sector &&
                                num.Period == FOPList.FirstOrDefault().Value[i].Period &&
                                num.ContingentType == FOPList.FirstOrDefault().Value[i].ContingentType
                                select num;

                        foreach (Vw_WeekWiseFinalOutPut GetVal in WWFOP)
                        {
                            if (GetVal.Rank == 1)
                            {
                                FOPList.FirstOrDefault().Value[i].Week1 = GetVal.Week;
                                FOPList.FirstOrDefault().Value[i].OrderQty1 = GetVal.OrderQty;
                                FOPList.FirstOrDefault().Value[i].DeliveredQty1 = GetVal.DeliveredQty;
                                FOPList.FirstOrDefault().Value[i].AcceptedQty1 = GetVal.AcceptedQty;
                                FOPList.FirstOrDefault().Value[i].InvoiceQty1 = GetVal.InvoiceQty;
                                FOPList.FirstOrDefault().Value[i].InvoiceValue1 = GetVal.InvoiceValue;
                            }
                            else if (GetVal.Rank == 2)
                            {
                                FOPList.FirstOrDefault().Value[i].Week2 = GetVal.Week;
                                FOPList.FirstOrDefault().Value[i].OrderQty2 = GetVal.OrderQty;
                                FOPList.FirstOrDefault().Value[i].DeliveredQty2 = GetVal.DeliveredQty;
                                FOPList.FirstOrDefault().Value[i].AcceptedQty2 = GetVal.AcceptedQty;
                                FOPList.FirstOrDefault().Value[i].InvoiceQty2 = GetVal.InvoiceQty;
                                FOPList.FirstOrDefault().Value[i].InvoiceValue2 = GetVal.InvoiceValue;
                            }
                            else if (GetVal.Rank == 3)
                            {
                                FOPList.FirstOrDefault().Value[i].Week3 = GetVal.Week;
                                FOPList.FirstOrDefault().Value[i].OrderQty3 = GetVal.OrderQty;
                                FOPList.FirstOrDefault().Value[i].DeliveredQty3 = GetVal.DeliveredQty;
                                FOPList.FirstOrDefault().Value[i].AcceptedQty3 = GetVal.AcceptedQty;
                                FOPList.FirstOrDefault().Value[i].InvoiceQty3 = GetVal.InvoiceQty;
                                FOPList.FirstOrDefault().Value[i].InvoiceValue3 = GetVal.InvoiceValue;
                            }
                            else if (GetVal.Rank == 4)
                            {
                                FOPList.FirstOrDefault().Value[i].Week4 = GetVal.Week;
                                FOPList.FirstOrDefault().Value[i].OrderQty4 = GetVal.OrderQty;
                                FOPList.FirstOrDefault().Value[i].DeliveredQty4 = GetVal.DeliveredQty;
                                FOPList.FirstOrDefault().Value[i].AcceptedQty4 = GetVal.AcceptedQty;
                                FOPList.FirstOrDefault().Value[i].InvoiceQty4 = GetVal.InvoiceQty;
                                FOPList.FirstOrDefault().Value[i].InvoiceValue4 = GetVal.InvoiceValue;
                            }
                        }
                    }
                    IList<Vw_DistinctValuesFOP> FOPList1 = FOPList.FirstOrDefault().Value;
                    // IList<Vw_WeekWiseFinalOutPut> WeeklyInvoiceList = FinalOutputList.FirstO

                    //***************

                    //Dictionary<long, IList<ContingentWeekWiseReport_vw>> WeekList = ords.GetWeekWiseReportListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                    //IList<ContingentWeekWiseReport_vw> WeeklyInvoiceList = WeekList.FirstOrDefault().Value;
                    string headerTable =
                    @"<img src='D:\HeaderImage\main_logo.jpg' height='40' width='280'><tr></tr><tr></tr><tr></tr><table width='1496' border='1'>
                    <tr>
    <td>&nbsp;</td>
    <td colspan='5'><div align='center'><strong>Week 1</strong></div></td>
    <td>&nbsp;</td>
    <td colspan='5'><div align='center'><strong>Week 2</strong></div></td>
    <td>&nbsp;</td>
    <td colspan='5'><div align='center'><strong>Week 3</strong></div></td>
    <td>&nbsp;</td>
    <td colspan='6'><div align='center'><strong>Week 4</strong></div></td>
  </tr>";
                    headerTable = headerTable + "</b></Table>";
                    ExptToInvoiceXL(FOPList1, title, (items => new

                    {

                        Name = items.Name,
                        Sector = items.Sector,
                        Period = items.Period,
                        ContingentType = items.ContingentType,

                        Week1 = items.Week1 == 0 ? "" : items.Week1.ToString(),//items.Week1,
                        OrderQty1 = items.OrderQty1 == 0 ? "" : items.OrderQty1.ToString(),//items.OrderQty1,
                        DeliveredQty1 = items.DeliveredQty1 == 0 ? "" : items.DeliveredQty1.ToString(),//items.DeliveredQty1,
                        AcceptedQty1 = items.AcceptedQty1 == 0 ? "" : items.AcceptedQty1.ToString(),//items.AcceptedQty1,
                        InvoiceQty1 = items.InvoiceQty1 == 0 ? "" : items.InvoiceQty1.ToString(),//items.InvoiceQty1,
                        InvoiceValue1 = items.InvoiceValue1 == 0 ? "" : items.InvoiceValue1.ToString(),//items.InvoiceValue1,
                        Week2 = items.Week2 == 0 ? "" : items.Week2.ToString(),//items.Week2,
                        OrderQty2 = items.OrderQty2 == 0 ? "" : items.OrderQty2.ToString(),//items.OrderQty2
                        DeliveredQty2 = items.DeliveredQty2 == 0 ? "" : items.DeliveredQty2.ToString(),//items.DeliveredQty2,
                        AcceptedQty2 = items.AcceptedQty2 == 0 ? "" : items.AcceptedQty2.ToString(),//items.AcceptedQty2,
                        InvoiceQty2 = items.InvoiceQty2 == 0 ? "" : items.InvoiceQty2.ToString(),//items.InvoiceQty2,
                        InvoiceValue2 = items.InvoiceValue2 == 0 ? "" : items.InvoiceValue2.ToString(),//items.InvoiceValue2,
                        Week3 = items.Week3 == 0 ? "" : items.Week3.ToString(),//items.Week3,
                        OrderQty3 = items.OrderQty3 == 0 ? "" : items.OrderQty3.ToString(),//items.OrderQty3,
                        DeliveredQty3 = items.DeliveredQty3 == 0 ? "" : items.DeliveredQty3.ToString(),//items.DeliveredQty3,
                        AcceptedQty3 = items.AcceptedQty3 == 0 ? "" : items.AcceptedQty3.ToString(),//items.AcceptedQty3,
                        InvoiceQty3 = items.InvoiceQty3 == 0 ? "" : items.InvoiceQty3.ToString(),//items.InvoiceQty3,
                        InvoiceValue3 = items.InvoiceValue3 == 0 ? "" : items.InvoiceValue3.ToString(),//items.InvoiceValue3,
                        Week4 = items.Week4 == 0 ? "" : items.Week4.ToString(),//items.Week4,
                        OrderQty4 = items.OrderQty4 == 0 ? "" : items.OrderQty4.ToString(),//items.OrderQty4,
                        DeliveredQty4 = items.DeliveredQty4 == 0 ? "" : items.DeliveredQty4.ToString(),//items.DeliveredQty4,
                        AcceptedQty4 = items.AcceptedQty4 == 0 ? "" : items.AcceptedQty4.ToString(),//items.AcceptedQty4,
                        InvoiceQty4 = items.InvoiceQty4 == 0 ? "" : items.InvoiceQty4.ToString(),//items.InvoiceQty4,
                        InvoiceValue4 = items.InvoiceValue4 == 0 ? "" : items.InvoiceValue4.ToString(),//items.InvoiceValue4,

                    }), headerTable);
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                    throw ex;
                }
            }
        }
        public void ExptToInvoiceXL<T, TResult>(IList<T> stuList, string filename, Func<T, TResult> selector, string headerTable)
        {
            try
            {
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment; filename=" + filename + ".xls");
                Response.ContentType = "application/vnd.ms-excel";
                System.IO.StringWriter stw = new System.IO.StringWriter();
                HtmlTextWriter htextw = new HtmlTextWriter(stw);
                DataGrid dg = new DataGrid();
                dg.HeaderStyle.BackColor = System.Drawing.Color.FromName("#B6B6B6");
                dg.HeaderStyle.Font.Bold = true;
                dg.HeaderStyle.ForeColor = System.Drawing.Color.White;
                dg.DataSource = stuList.Select(selector);

                dg.DataBind();
                dg.RenderControl(htextw);
                Response.Write(headerTable);
                Response.Write(stw.ToString());
                Response.End();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }

        }
        #endregion

        #region Insight Report

        public ActionResult orderItemMissMatch()
        {
            return View();
        }

        public ActionResult JqgridorderItemMissMatch(int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                string[] cri = { "UNORDEREDDELIVERY", "SUBSTITUTIONMISMATCH" };
                OrdersService ords = new OrdersService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                criteria.Add("ReportCode", cri);
                Dictionary<long, IList<InsightReport>> OrderList = ords.InsightReportListWithLikeSearchPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                if (OrderList != null && OrderList.FirstOrDefault().Value.Count > 0 && OrderList.FirstOrDefault().Key > 0)
                {
                    long totalrecords = OrderList.FirstOrDefault().Key;
                    int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                    var jsondat = new
                    {
                        total = totalPages,
                        page = page,
                        records = totalrecords,

                        rows = (from items in OrderList.FirstOrDefault().Value
                                select new
                                {
                                    i = 2,
                                    cell = new string[] {
                                        items.ReportId.ToString(),
                                        items.Description,
                                        items.ReportCode,
                                        items.FileNames,
                                        items.OrderId.ToString(),
                                        items.ControlId,
                                        items.UNCode.ToString(),
                                        items.Commodity,
                                        items.DeliveredQty.ToString(),
                                        items.SubsCode.ToString(),
                                        items.SubsName,
                                        items.ReplacementCode.ToString(),
                                        items.ReplacementName,
                                        items.CreatedDate.ToString(),
                                        items.CreatedBy,
                                        items.DeliveryNoteName,

                                }
                                })
                    };
                    return Json(jsondat, JsonRequestBehavior.AllowGet);
                }






                return null;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }

        public ActionResult OrderitemsMisMatchJqGrid(string searchItems, string ExptType, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                OrdersService ords = new OrdersService();
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");

                if (searchItems == null)
                {
                    return null;
                }
                else
                {
                    Dictionary<string, object> criteria = new Dictionary<string, object>();
                    criteria.Clear();
                    if (searchItems != null && searchItems != "")
                    {
                        var Items = searchItems.ToString().Split(',');
                        if (!string.IsNullOrWhiteSpace(Items[0])) { criteria.Add("Sector", Items[0]); }
                        if (!string.IsNullOrWhiteSpace(Items[1])) { criteria.Add("ContingentType", Items[1]); }
                        if (!string.IsNullOrWhiteSpace(Items[2])) { criteria.Add("Name", Items[2]); }
                        if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Period", Items[3]); }
                        if (!string.IsNullOrWhiteSpace(Items[4])) { criteria.Add("PeriodYear", Items[4]); }
                        var TrimItems = searchItems.Replace(",", "");
                        if (string.IsNullOrWhiteSpace(TrimItems))
                            return null;
                    }
                    Dictionary<long, IList<OrderItemMismatch>> OrderList = ords.OrderItemMismatchReportListWithLikeSearchPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                    IList<OrderItemMismatch> OrditemRpt = OrderList.FirstOrDefault().Value.ToList();
                    if (OrditemRpt != null && OrditemRpt.Count() > 0)
                    {

                        if (ExptType == "Excel")
                        {
                            int i = 1;
                            foreach (var item in OrditemRpt)
                            {
                                item.Id = i;
                                i = i + 1;
                            }
                            var List = OrditemRpt;
                            List<string> lstHeader = new List<string>() { "Sl.No", "Sector", "Contingent Type", "Contingent", "Period", "Period Year", "ControlId", "UNCode", "Commodity", "Delivered Qty", "Substitution Code", "Substitution Commodity", "Created Date", "Delivery Note Name", "Description" };
                            base.NewExportToExcel(List, "OrderItemsMismatchReport", (item => new
                            {
                                Id = item.Id,
                                Sector = item.Sector,
                                ContingentType = item.ContingentType,
                                Name = item.Name,
                                Period = item.Period,
                                PeriodYear = item.PeriodYear,
                                ControlId = item.ControlId,
                                UNCode = item.UNCode,
                                Commodity = item.Commodity,
                                DeliveredQty = item.DeliveredQty,
                                SubsCode = item.SubsCode,
                                SubsName = item.SubsName,
                                CreatedDate = ConvertDateTimeToDate(item.CreatedDate.ToString("dd-MMM-yyyy"), "en-GB"),
                                DeliveryNoteName = item.DeliveryNoteName,
                                Description = item.Description
                            }), lstHeader);
                            return new EmptyResult();
                        }
                        else
                        {
                            long totalrecords = OrditemRpt.Count();
                            int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                            var jsondat = new
                            {
                                total = totalPages,
                                page = page,
                                records = totalrecords,

                                rows = (from items in OrditemRpt
                                        select new
                                        {
                                            i = 2,
                                            cell = new string[] {
                                    items.Id.ToString(),
                                    items.OrderId.ToString(),
                                    items.ControlId,
                                    items.Sector,
                                    items.ContingentType,
                                    items.Name,
                                    items.Period,
                                    items.PeriodYear,
                                    items.UNCode.ToString(),
                                    items.Commodity,
                                    items.DeliveredQty.ToString(),
                                    items.SubsCode.ToString(),
                                    items.SubsName,
                                    items.CreatedDate!=null? items.CreatedDate.ToString("dd/MM/yyyy") : "",
                                    items.CreatedBy,
                                    items.DeliveryNoteName,
                                    items.Description
                                }
                                        })
                            };
                            return Json(jsondat, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }




        #endregion Insight Report

        #region order items report


        public ActionResult OrderReportjqGrid(string searchItems, string ReportType, string OrderId, string ControlId, string Sector, string ContingentType, string Name, string Period, string Week, string ExptType, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                OrdersService ords = new OrdersService();
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");

                if (searchItems == null)
                {
                    return null;
                }
                else
                {
                    Dictionary<string, object> criteria = new Dictionary<string, object>();
                    sord = sord == "desc" ? "Desc" : "Asc";
                    criteria.Clear();
                    if (searchItems != null && searchItems != "")
                    {
                        var Items = searchItems.ToString().Split(',');
                        if (!string.IsNullOrWhiteSpace(Items[0])) { criteria.Add("Sector", Items[0]); }
                        if (!string.IsNullOrWhiteSpace(Items[1])) { criteria.Add("ContingentType", Items[1]); }
                        if (!string.IsNullOrWhiteSpace(Items[2])) { criteria.Add("Name", Items[2]); }
                        if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Period", Items[3]); }
                        if (!string.IsNullOrWhiteSpace(Items[4])) { criteria.Add("Week", Convert.ToInt64(Items[4])); }
                        if (!string.IsNullOrWhiteSpace(Items[5])) { criteria.Add("PeriodYear", Items[5]); }
                        var TrimItems = searchItems.Replace(",", "");
                        if (string.IsNullOrWhiteSpace(TrimItems))
                            return null;
                    }
                    if (!string.IsNullOrEmpty(OrderId))
                        criteria.Add("OrderId", Convert.ToInt64(OrderId));
                    if (!string.IsNullOrEmpty(ControlId))
                        criteria.Add("ControlId", ControlId);
                    if (!string.IsNullOrEmpty(Sector))
                        criteria.Add("Sector", Sector);
                    if (!string.IsNullOrEmpty(ContingentType))
                        criteria.Add("ContingentType", ContingentType);
                    if (!string.IsNullOrEmpty(Name))
                        criteria.Add("Name", Name);
                    if (!string.IsNullOrEmpty(Period))
                        criteria.Add("Period", Period);
                    if (!string.IsNullOrEmpty(Week))
                        criteria.Add("Week", Convert.ToInt64(Week));
                    if (!string.IsNullOrEmpty(ReportType))
                        criteria.Add("ReportType", ReportType);

                    Dictionary<long, IList<InvoiceReports>> DictInvRpt = null;
                    InvoiceService IS = new InvoiceService();

                    DictInvRpt = IS.GetInvoiceReportsListWithPagingAndCriteria(page - 1, rows, "ControlId", sord, criteria);

                    IList<InvoiceReports> InvRpt = DictInvRpt.FirstOrDefault().Value.ToList();

                    if (InvRpt != null && InvRpt.Count() > 0)
                    {
                        if (ExptType == "Excel")
                        {
                            int i = 1;
                            foreach (var item in InvRpt)
                            {
                                item.Id = i;
                                i = i + 1;
                            }
                            string Docname = "";
                            if (ReportType == "ORDINV")
                                Docname = "OrderItemsReport";
                            else
                                Docname = "WeekInvoiceReport";

                            var List = InvRpt;
                            List<string> lstHeader = new List<string>() {"Sl.No","Requisition Number","Invoice Number","Location","Strength","No.of Days","Week","Line Item Ordered","Total Line Item Substituted","Quantity Ordered","Quantity Delivered","Quantity Accepted",
"Amount Ordered","Amount Accepted","Troop Strength Discount","Other Credit Notes","Weekly Invoice Discount","Net Amount for Rations","APL-Timely Delivery","APL-Order by Line Items",
"APL-Orders by Weight","APL-No. of  Authorized Substitutions","Total Invoice Amount","Net Invoice Value for Food","Mode of Transportation","Distance (KM)","Transportation Per Kg Cost","Total Transportation Cost","DN #","Approved Delivery Date","Actual Date of Receipt","# Days Delay","Authorized CMR","Order CMR",
"Accepted CMR","% of CMR Utilized","Line item delivered>=98%","Confirmity to line item ordered >=98%","Conformity to Order by Weight %","No.of sub %","Amount substituted","Days delay-performance","Delivery Notes" ,"ContingentId"};
                            base.NewExportToExcel(List, "" + Docname + "-" + InvRpt[0].Period + "-" + InvRpt[0].PeriodYear, (item => new
                            {
                                Id = item.Id,
                                ControlId = item.ControlId,
                                InvoiceNumber = item.InvoiceNumber,
                                Location = item.Location,
                                Strength = item.Strength,
                                Noofdays = item.Noofdays,
                                Week = item.Week,
                                Lineitemordered = item.Lineitemordered,
                                Totallineitemsubstituted = item.Totallineitemsubstituted,
                                Orderedqty = item.Orderedqty,
                                Deliveredqty = item.Deliveredqty,
                                Acceptedqty = item.Acceptedqty,
                                Amountordered = item.Amountordered,
                                Amountaccepted = item.Amountaccepted,
                                Troopstrengthdiscount = item.Troopstrengthdiscount,
                                Othercreditnotes = item.Othercreditnotes,
                                Weeklyinvoicediscount = item.Weeklyinvoicediscount,
                                Netamountforrations = item.Netamountforrations,
                                AplTimelydelivery = item.AplTimelydelivery,
                                AplOrderbylineitems = item.AplOrderbylineitems,
                                AplOrdersbyweight = item.AplOrdersbyweight,
                                AplNoofauthorizedsubstitutions = item.AplNoofauthorizedsubstitutions,
                                Totalinvoiceamount = item.Totalinvoiceamount,
                                NetAmountforFood = Math.Round((item.Totalinvoiceamount - item.Totaltransportationcost), 2, MidpointRounding.AwayFromZero),
                                Modeoftransportation = item.Modeoftransportation,
                                Distancekm = item.Distancekm,
                                Transportationperkgcost = item.Transportationperkgcost,
                                Totaltransportationcost = item.Totaltransportationcost,
                                Dn = item.Dn,
                                //Approveddeliverydate = String.Format("{0:d}", item.Approveddeliverydate),//ConvertDateTimeToDate(item.Approveddeliverydate.ToString("dd-MMM-yyyy"), "en-GB"),
                                //Actualdateofreceipt = String.Format("{0:d}", item.Actualdateofreceipt), //ConvertDateTimeToDate(item.Actualdateofreceipt.ToString("dd-MMM-yyyy"), "en-GB"),
                                Approveddeliverydate = item.Approveddeliverydate,
                                Actualdateofreceipt = item.Actualdateofreceipt,
                                Daysdelay = item.Daysdelay,
                                Authorizedcmr = Math.Round(item.Authorizedcmr, 2),
                                Ordercmr = Math.Round(item.Ordercmr, 2),
                                Acceptedcmr = Math.Round(item.Acceptedcmr, 2),
                                Cmrutilized = Math.Round(item.Cmrutilized, 2),
                                Lineitemdelivered98 = item.Lineitemdelivered98,
                                Confirmitytolineitemorder98 = Math.Round(item.Confirmitytolineitemorder98, 2),
                                Conformitytoorderbyweight = Math.Round(item.Conformitytoorderbyweight, 2),
                                Noofsubtitution = Math.Round(item.Noofsubtitution, 2),
                                Amountsubstituted = Math.Round(item.Amountsubstituted, 2),
                                Daysdelayperformance = item.Daysdelayperformance,
                                DeliveryNotes = item.DeliveryNotes,
                                ContingentId = item.ContingentID,
                            }), lstHeader);
                            return new EmptyResult();
                        }
                        else
                        {

                            long totalrecords = InvRpt.Count();
                            int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                            var jsondat = new
                            {
                                total = totalPages,
                                page = page,
                                records = totalrecords,

                                rows = (from items in InvRpt
                                        select new
                                        {
                                            i = 2,
                                            cell = new string[] {
                                                    items.Id.ToString(),
                                                    items.ControlId,
                                                    items.InvoiceNumber,
                                                    items.Location,
                                                    items.Strength.ToString(),
                                                    items.Noofdays.ToString(),
                                                    items.Week.ToString(),
                                                    items.Period,
                                                    items.PeriodYear,
                                                    items.Sector,
                                                    items.Contingent,
                                                    items.ContingentType,
                                                    items.Lineitemordered.ToString(),
                                                    items.Totallineitemsubstituted.ToString(),
                                                    items.Orderedqty.ToString(),
                                                    items.Deliveredqty.ToString(),
                                                    items.Acceptedqty.ToString(),
                                                    items.Amountordered.ToString(),
                                                    items.Amountaccepted.ToString(),
                                                    items.Troopstrengthdiscount.ToString(),
                                                    items.Othercreditnotes.ToString(),
                                                    items.Weeklyinvoicediscount.ToString(),
                                                    items.Netamountforrations.ToString(),
                                                    items.AplTimelydelivery.ToString(),
                                                    items.AplOrderbylineitems.ToString(),
                                                    items.AplOrdersbyweight.ToString(),
                                                    items.AplNoofauthorizedsubstitutions.ToString(),
                                                    items.Totalinvoiceamount.ToString(),
                                                    items.Modeoftransportation,
                                                    items.Distancekm.ToString(),
                                                    items.Transportationperkgcost.ToString(),
                                                    items.Totaltransportationcost.ToString(),
                                                    items.Dn,
                                                    items.Approveddeliverydate!=null? ConvertDateTimeToDate(items.Approveddeliverydate.ToString("MM/dd/yyyy"),"en-GB"):"",
                                                    items.Actualdateofreceipt!=null? ConvertDateTimeToDate(items.Actualdateofreceipt.ToString("MM/dd/yyyy"),"en-GB"):"",
                                                    items.Daysdelay.ToString(),
                                                    items.Authorizedcmr.ToString(),
                                                    items.Ordercmr.ToString(),
                                                    items.Acceptedcmr.ToString(),
                                                    items.Cmrutilized.ToString(),
                                                    items.Lineitemdelivered98.ToString(),
                                                    items.Confirmitytolineitemorder98.ToString(),
                                                    items.Conformitytoorderbyweight.ToString(),
                                                    items.Noofsubtitution.ToString(),
                                                    items.Amountsubstituted.ToString(),
                                                    items.Daysdelayperformance.ToString(),
                                                    items.DeliveryNotes,
                                                    items.ContingentID.ToString()
                                                   }
                                        })
                            };
                            return Json(jsondat, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                return Json(null, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }
        public ActionResult OrderReportsjqGridtest(string searchItems, string ControlId, int rows, string sidx, string sord, int? page = 1)
        {
            string userId = base.ValidateUser();
            Dictionary<string, object> criteria = new Dictionary<string, object>();
            sord = sord == "desc" ? "Desc" : "Asc";
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(searchItems) && string.IsNullOrWhiteSpace(ControlId))
                    {
                        return null;
                    }
                    else
                    {
                        Dictionary<long, IList<vw_OrderItemsReport>> OrderList = null;
                        Dictionary<long, IList<PerformanceCalculateView>> pCalList = null;
                        Dictionary<long, IList<PerformanceDetails>> pDetails = null;

                        criteria.Clear();
                        if (searchItems != null && searchItems != "")
                        {
                            var Items = searchItems.ToString().Split(',');
                            if (!string.IsNullOrWhiteSpace(Items[0])) { criteria.Add("Sector", Items[0]); }
                            if (!string.IsNullOrWhiteSpace(Items[1])) { criteria.Add("ContingentType", Items[1]); }
                            if (!string.IsNullOrWhiteSpace(Items[2])) { criteria.Add("Name", Items[2]); }
                            if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Period", Items[3]); }
                            if (!string.IsNullOrWhiteSpace(Items[4])) { criteria.Add("Week", Convert.ToInt64(Items[4])); }
                            var TrimItems = searchItems.Replace(",", "");
                            if (string.IsNullOrWhiteSpace(TrimItems))
                                return null;
                        }
                        if (!string.IsNullOrEmpty(ControlId))
                            criteria.Add("ControlId", ControlId);

                        InvoiceService IS = new InvoiceService();
                        pCalList = IS.GetPerformanceCalculateListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                        pDetails = IS.GetPerformanceDetailsListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);

                        if (OrderList != null && OrderList.FirstOrDefault().Value != null && OrderList.FirstOrDefault().Value.Count > 0)
                        {
                        }
                        return View();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        #endregion

        #region Modified OrderItems report
        public ActionResult OrderReportjqGridORDINV(string searchItems, string ReportType, string ControlId, string ExptType, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                OrdersService ords = new OrdersService();
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                if (searchItems != null && searchItems != "")
                {
                    var Items = searchItems.ToString().Split(',');




                    IList<OrderItemsReport_SP> OrderItemsReportlist = null;
                    OrdersService OS = new OrdersService();



                    OrderItemsReportlist = OS.GetOrderItemsReport(Items[0], Items[1]);
                    long totalrecords = 0;
                    int totalpages = 0;

                    if (ExptType == "Excel")
                    {
                        int i = 1;
                        foreach (var item in OrderItemsReportlist)
                        {
                            item.Id = i;
                            i = i + 1;
                        }
                        string Docname = "";
                        if (ReportType == "ORDINV")
                            Docname = "OrderItemsReport";
                        else
                            Docname = "WeekInvoiceReport";

                        var List = OrderItemsReportlist;
                        List<string> lstHeader = new List<string>() {
                                                    "Sl.No","Requisition Number","Invoice Number","Location","Contingent Type","Strength","No.of Days","Week","Line Item Ordered","Total Line Item Substituted","Quantity Ordered","Quantity Delivered","Quantity Accepted",
                    "Amount Ordered","AmountDelivered","Amount Accepted","Troop Strength Discount","Other Credit Notes","Weekly Invoice Discount","Net Amount for Rations","APL-Timely Delivery","APL-Order by Line Items",
                    "APL-Orders by Weight","APL-No. of  Authorized Substitutions","Total Invoice Amount","Mode of Transportation","Distance (KM)","Transportation Per Kg Cost","Total Transportation Cost","DN #","Approved Delivery Date","Actual Date of Receipt","# Days Delay","Authorized CMR","Order CMR",
                    "Accepted CMR","% of CMR Utilized","Line item delivered>=98%","Confirmity to line item ordered >=98%","Conformity to Order by Weight %","No.of sub %","Amount substituted","Days delay-performance","Delivery Notes" ,"ContingentID"};
                        base.NewExportToExcel(List, "" + Docname + "-" + OrderItemsReportlist[0].Period + "-" + OrderItemsReportlist[0].PeriodYear, (item => new
                        {
                            Id = item.Id,
                            ControlId = item.ControlId,
                            InvoiceNumber = item.InvoiceNumber,
                            Location = item.Location,
                            ContingentType = item.ContingentType,
                            Strength = item.Strength,
                            Noofdays = item.Noofdays,
                            Week = item.Week,
                            Lineitemordered = item.Lineitemordered,
                            Totallineitemsubstituted = item.Totallineitemsubstituted,
                            Orderedqty = item.Orderedqty,
                            Deliveredqty = item.Deliveredqty,
                            Acceptedqty = item.Acceptedqty,
                            Amountordered = item.Amountordered,
                            AmountDelivered = item.Deliveredvalue,
                            Amountaccepted = item.Amountaccepted,
                            Troopstrengthdiscount = item.Troopstrengthdiscount,
                            Othercreditnotes = item.Othercreditnotes,
                            Weeklyinvoicediscount = item.Weeklyinvoicediscount,
                            Netamountforrations = item.Netamountforrations,
                            AplTimelydelivery = item.AplTimelydelivery,
                            AplOrderbylineitems = item.AplOrderbylineitems,
                            AplOrdersbyweight = item.AplOrdersbyweight,
                            AplNoofauthorizedsubstitutions = item.AplNoofauthorizedsubstitutions,
                            Totalinvoiceamount = item.Totalinvoiceamount,
                            Modeoftransportation = item.Modeoftransportation,
                            Distancekm = item.Distancekm,
                            Transportationperkgcost = item.Transportationperkgcost,
                            Totaltransportationcost = item.Totaltransportationcost,
                            Dn = item.Dn,
                            //Approveddeliverydate = String.Format("{0:d}", item.Approveddeliverydate),//ConvertDateTimeToDate(item.Approveddeliverydate.ToString("dd-MMM-yyyy"), "en-GB"),
                            //Actualdateofreceipt = String.Format("{0:d}", item.Actualdateofreceipt), //ConvertDateTimeToDate(item.Actualdateofreceipt.ToString("dd-MMM-yyyy"), "en-GB"),
                            Approveddeliverydate = item.Approveddeliverydate,
                            Actualdateofreceipt = item.Actualdateofreceipt,
                            Daysdelay = item.Daysdelay,
                            Authorizedcmr = Math.Round(item.Authorizedcmr, 2),
                            Ordercmr = Math.Round(item.Ordercmr, 2),
                            Acceptedcmr = Math.Round(item.Acceptedcmr, 2),
                            Cmrutilized = Math.Round(item.Cmrutilized, 2),
                            Lineitemdelivered98 = item.Lineitemdelivered98,
                            Confirmitytolineitemorder98 = Math.Round(item.Confirmitytolineitemorder98, 2),
                            Conformitytoorderbyweight = Math.Round(item.Conformitytoorderbyweight, 2),
                            Noofsubtitution = Math.Round(item.Noofsubtitution, 2),
                            Amountsubstituted = Math.Round(item.Amountsubstituted, 2),
                            Daysdelayperformance = item.Daysdelayperformance,
                            DeliveryNotes = item.DeliveryNotes,
                            ContingentID = item.ContingentID
                        }), lstHeader);
                        return new EmptyResult();
                    }
                    else
                    {
                        if (OrderItemsReportlist != null && OrderItemsReportlist.Count() > 0)
                        {
                            totalrecords = OrderItemsReportlist.Count;
                            totalpages = (int)Math.Ceiling(totalrecords / (float)rows);
                            var jsondat = new
                            {
                                total = totalpages,
                                page = page,
                                records = totalrecords,
                                rows = (from items in OrderItemsReportlist
                                        select new
                                        {
                                            i = 2,
                                            cell = new string[]{
                                        items.Id.ToString(),
                                        items.ControlId,
                                        items.InvoiceNumber,
                                        items.Location,
                                        items.ContingentType,
                                        items.Strength.ToString(),
                                        items.Noofdays.ToString(),
                                        items.Week.ToString(),
                                        items.Period,
                                        items.PeriodYear,
                                        items.Sector,
                                        items.Contingent,
                                        items.ContingentType,
                                        items.Lineitemordered.ToString(),
                                        items.Totallineitemsubstituted.ToString(),
                                        items.Orderedqty.ToString(),
                                        items.Deliveredqty.ToString(),
                                        items.Acceptedqty.ToString(),
                                        items.Amountordered.ToString(),
                                        items.Deliveredvalue.ToString(),
                                        items.Amountaccepted.ToString(),
                                        items.Troopstrengthdiscount.ToString(),
                                        items.Othercreditnotes.ToString(),
                                        items.Weeklyinvoicediscount.ToString(),
                                        items.Netamountforrations.ToString(),
                                        items.AplTimelydelivery.ToString(),
                                        items.AplOrderbylineitems.ToString(),
                                        items.AplOrdersbyweight.ToString(),
                                        items.AplNoofauthorizedsubstitutions.ToString(),
                                        items.Totalinvoiceamount.ToString(),
                                        items.Modeoftransportation,
                                        items.Distancekm.ToString(),
                                        items.Transportationperkgcost.ToString(),
                                        items.Totaltransportationcost.ToString(),
                                        items.Dn,
                                        items.Approveddeliverydate.ToString(),
                                        items.Actualdateofreceipt.ToString(),
                                        items.Daysdelay.ToString(),
                                        items.Authorizedcmr.ToString(),
                                        items.Ordercmr.ToString(),
                                        items.Acceptedcmr.ToString(),
                                        items.Cmrutilized.ToString(),
                                        items.Lineitemdelivered98.ToString(),
                                        items.Confirmitytolineitemorder98.ToString(),
                                        items.Conformitytoorderbyweight.ToString(),
                                        items.Noofsubtitution.ToString(),
                                        items.Amountsubstituted.ToString(),
                                        items.Daysdelayperformance.ToString(),
                                        items.DeliveryNotes.ToString(),
                                        items.ContingentID.ToString(),
                                        

                                        items.OrderId.ToString(),
                                        items.CreatedDate.ToString(),
                                        items.CreatedBy,
                                        items.ModifiedBy,
                                        items.ModifiedDate.ToString(),
                                        items.ReportType,
                                        items.IsActive.ToString()
                                        }
                                        })

                            };


                            return Json(jsondat, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var Empty = new { rows = (new { cell = new string[] { } }) };
                            return Json(Empty, JsonRequestBehavior.AllowGet);
                        }

                    }
                }
                else
                {
                    var Empty = new { rows = (new { cell = new string[] { } }) };
                    return Json(Empty, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }


        #endregion


        #region GCC Revised Report

        public ActionResult GCCRevisedReport()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightReportPolicy");
                throw ex;
            }
        }
        /// <summary>
        /// Check and ReGenerate GCCRevisedReport One by one
        /// </summary>
        /// <param name="searchItems">Have Period and Period year</param>
        /// <param name="Ids">Selected Document in GCCRevisedReport(Already Created)</param>
        /// <returns></returns>
        public JsonResult GCCRevisedReportGenerate(string searchItems)
        {
            string userId = base.ValidateUser();

            try
            {

                int Count = 0;


                //To Generate all the documents from Searchitems

                if (searchItems != null && searchItems != "")
                {
                    var Items = searchItems.ToString().Split(',');
                    if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Period", Items[3]); }
                    if (!string.IsNullOrWhiteSpace(Items[4])) { criteria.Add("PeriodYear", Items[4]); }
                    var TrimItems = searchItems.Replace(",", "");
                    if (string.IsNullOrWhiteSpace(TrimItems))
                        return null;
                }
                Dictionary<long, IList<InvoiceManagementView>> invoiceItems = IS.GetInvoiceListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);

                if (invoiceItems != null && invoiceItems.FirstOrDefault().Key > 0)
                {
                    var PeriodList = (from items in invoiceItems.First().Value
                                      select new { items.Period, items.PeriodYear }).OrderBy(i => i.Period).Distinct().ToArray();

                    foreach (var item in PeriodList)
                    {
                        GenerateGCCRevisedReport(item.Period, item.PeriodYear);
                        Count = Count + 1;
                    }
                }
                else
                {
                    return Json("GCC Revised version Gerneration Failed !", JsonRequestBehavior.AllowGet);
                }


                return Json("GCC Revised version Generated Successfully !", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightReportPolicy");
                throw ex;
            }
        }
        /// <summary>
        /// Create New GCC Revised
        /// </summary>
        /// <param name="Period"></param>
        /// <param name="PeriodYear"></param>
        private void GenerateGCCRevisedReport(string Period, string PeriodYear)
        {
            try
            {
                string userId = base.ValidateUser();
                //Deleting the old GCCRevised version
                OS.DeleteGCCRevisedVersion(Period, PeriodYear);


                criteria.Clear();
                if (!string.IsNullOrWhiteSpace(Period)) { criteria.Add("Period", Period); }
                if (!string.IsNullOrWhiteSpace(PeriodYear)) { criteria.Add("PeriodYear", PeriodYear); }


                Dictionary<long, IList<GCCRevised_vw>> GCCRevised = OS.GetGCCRevisedListbySP(Period, PeriodYear);
                List<GCCRevised_vw> GCCRevisedVersion = GCCRevised.FirstOrDefault().Value.ToList();

                // Dictionary<long, IList<GCCRevised_vw>> RevisedList = OS.GetGCCRevisedListWithPagingAndCriteria(0, 9999999, string.Empty, string.Empty, criteria);
                if (GCCRevisedVersion != null && GCCRevisedVersion.Count > 0)
                {



                    var List = GCCRevisedVersion;
                    int i = 1;
                    foreach (var item in List)
                    {
                        item.Id = i;
                        i = i + 1;
                    }
                    List<string> lstHeader = new List<string>() { "Id", "ControlId", "Invoice Number", "App D.Date", "DN # as per UN", "DeliveryMode", "Delivery Date", "UNCode", "Item Name/Commodity", "Order Qty", "Substitute UN Code", "Substitute Item/commodity", "Delivered Qty", "Invoice Qty", "Remaining Qty", "Delivered Value", "Accepted Value", "Food Value", "Transport Value", "Insurance Value", "FoodType", "ContingentID" };
                    string txtTName = "GCCRevised-" + List[0].Period + "-" + List[0].PeriodYear;
                    byte[] data = GenerateByteExcel_GCCRevised(List, txtTName, (items => new
                    {
                        Id = items.Id,
                        ControlId = items.ControlId,
                        InvoiceNumber = items.InvoiceNumber,
                        ExpectedDeliveryDate = ConvertDateTimeToDate(items.ExpectedDeliveryDate.ToString("dd-MMM-yyyy"), "en-GB"),
                        DeliveryNoteName = items.DeliveryNoteName,
                        DeliveryMode = items.DeliveryMode,
                        DeliveredDate = items.DeliveredDate,
                        UNCode = items.UNCode,
                        Commodity = items.Commodity,
                        OrderQty = string.Format("{0:N}", Math.Round(items.OrderQty, 2)),
                        SubstituteItemCode = items.SubstituteItemCode != "0" ? items.SubstituteItemCode : " ",
                        SubstituteItemName = items.SubstituteItemName,
                        DeliveredQty = string.Format("{0:N}", Math.Round(items.DeliveredQty, 2)),
                        //AcceptedQty = string.Format("{0:N}", Math.Round(items.AcceptedQty, 2)),
                        InvoiceQty = string.Format("{0:N}", Math.Round(items.InvoiceQty, 2)),
                        RemainingQty = string.Format("{0:N}", Math.Round(items.RemainingQty, 2)),
                        DeliveredValue = string.Format("{0:N}", Math.Round(items.DeliveredValue, 2)),
                        AcceptedValue = string.Format("{0:N}", Math.Round(items.AcceptedValue, 2)),
                        FoodValue = string.Format("{0:N}", Math.Round(items.FoodValue, 2)),
                        TransportValue = string.Format("{0:N}", Math.Round(items.TransportValue, 2)),
                        InsuranceValue = string.Format("{0:N}", Math.Round(items.InsuranceValue, 2)),
                        //Added by Thamizhmani for FFV & NON-FFV on 25 Aug 2016
                        FoodType = items.FoodType,
                        ContingentID = items.ContingentID

                    }), lstHeader);



                    criteria.Clear();
                    if (!string.IsNullOrWhiteSpace(Period)) { criteria.Add("Period", Period); }
                    if (!string.IsNullOrWhiteSpace(PeriodYear)) { criteria.Add("PeriodYear", PeriodYear); }
                    criteria.Add("DocumentType", "Revised-Book");
                    Dictionary<long, IList<ExcelDocuments>> DocumentItems = IS.GetExcelDocumentListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);

                    if (DocumentItems != null && DocumentItems.FirstOrDefault().Key > 0)
                    {
                        ExcelDocuments ED = IS.GetExcelDocumentsDetailsById(DocumentItems.FirstOrDefault().Value[0].Id);
                        ED.DocumentData = data;
                        long id = IS.SaveOrUpdateExcelDocuments(ED, userId);
                    }
                    else
                    {

                        ExcelDocuments doc = new ExcelDocuments();
                        doc.ControlId = txtTName;
                        doc.Period = Period;
                        doc.PeriodYear = PeriodYear;
                        doc.DocumentData = data;
                        doc.DocumentType = "Revised-Book";
                        doc.DocumentName = txtTName;
                        long id = IS.SaveOrUpdateExcelDocuments(doc, userId);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightReportPolicy");
                throw ex;
            }
        }

        #endregion

        #region Week Invoices
        public ActionResult WeekInvoices()
        {
            return View();
        }
        public ActionResult WeekGeneratePartial()
        {
            return View();
        }
        public JsonResult WeekGeneratePopup(string searchItems)
        {
            string userId = base.ValidateUser();
            try
            {
                //new Task(CreateWeekInvoiceParallel(searchItems)).Start();
                new Task(() =>
                {
                    CreateWeekInvoiceParallel(searchItems);
                }).Start();

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }
        public void CreateWeekInvoiceParallel(string searchItems)
        {
            try
            {
                int successCount = 0;
                DateTime Dt = DateTime.Now.Date;
                string OrderQuery = " ";
                string InvoiceQuery = " ";

                //Creating Dummy Invoice records
                List<long> InvoiceIds = new List<long>();


                //Added by kingston Deleting the old records in InvoiceReports table based on the criteria and regenerate the Weekwise invoice reports



                //Getting Orderid in InvoiceItems
                if (searchItems != "")
                {
                    var Items = searchItems.ToString().Split(',');
                    if (!string.IsNullOrWhiteSpace(Items[2])) { criteria.Add("Week", Convert.ToInt64(Items[2])); }
                    if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Period", Items[3]); }
                    if (!string.IsNullOrWhiteSpace(Items[4]))
                    {
                        criteria.Add("PeriodYear", Items[4]);
                        IS.DeleteWeekInvoiceInInvoiceReportsTbl(Convert.ToInt64(Items[2]), Items[3], Items[4]);
                    }
                }
                Dictionary<long, IList<InvoiceManagementView>> invoiceItems = IS.GetInvoiceListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                if (invoiceItems.First().Key == 0)
                {
                    InvoiceIds = DummyInvoiceGeneration(searchItems, Dt);
                }
                Dictionary<long, IList<InvoiceManagementView>> invoiceItems1 = IS.GetInvoiceListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                if (invoiceItems != null && invoiceItems1.First().Key > 0)
                {
                    var InvoiceOrderList = (from items in invoiceItems1.First().Value
                                            select items.OrderId).OrderBy(i => i).Distinct().ToArray();
                    foreach (var item in InvoiceOrderList)
                    {
                        SingleInvoice singleInvoice = SingleInvoiceListForWeekReport(item);
                        successCount = successCount + 1;
                    }
                }
                if (InvoiceIds.Count > 0)
                {

                    OrderQuery = OrderQuery + "Update Orders set Invoiceid=null where Invoiceid in (";
                    InvoiceQuery = InvoiceQuery + "Delete Invoice where id in (";
                    foreach (var item in InvoiceIds)
                    {
                        OrderQuery = OrderQuery + "" + item + ",";
                        InvoiceQuery = InvoiceQuery + "" + item + ",";
                    }
                    OrderQuery = OrderQuery.Remove(OrderQuery.Length - 1);
                    InvoiceQuery = InvoiceQuery.Remove(InvoiceQuery.Length - 1);
                    OrderQuery = OrderQuery + ")";
                    InvoiceQuery = InvoiceQuery + ")";

                    OS.UpdateUsingQuries(InvoiceQuery);
                    OS.UpdateUsingQuries(OrderQuery);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }

        }
        #endregion


        #region DeliveryNoteBased Report
        public ActionResult DeliveryNoteBasedReport()
        {
            return View();
        }
        public ActionResult DeliveryNoteBasedReportjqGrid(string ExptType, string Sector, string ContingentType, string Contingent, string Period, string PeriodYear, int rows, string sidx, string sord, int? page = 1)
        {
            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            try
            {
                OrdersService ords = new OrdersService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                sord = sord == "desc" ? "Desc" : "Asc";
                criteria.Clear();

                if (!string.IsNullOrEmpty(Sector))
                    criteria.Add("Sector", Sector);
                if (!string.IsNullOrEmpty(ContingentType))
                    criteria.Add("ContingentType", ContingentType);
                if (!string.IsNullOrEmpty(Contingent))
                    criteria.Add("Name", Contingent);
                if (!string.IsNullOrEmpty(Period))
                    criteria.Add("Period", Period);
                if (!string.IsNullOrEmpty(PeriodYear))
                    criteria.Add("PeriodYear", PeriodYear);

                if (criteria.Count == 0)
                {
                    return null;
                }
                else
                {
                    Dictionary<long, IList<DeliveryNoteBasedReport_Vw>> DeliveryNoteReportCount = ords.GetDeliveryNoteBasedReportListWithPagingAndCriteria(page - 1, rows, sord, sidx, criteria);
                    //declare new list for DeliveryNoteBasedReport_Vw

                    //IList<DeliveryNoteBasedReport_Vw> NewDeliveryNoteReportList;// = ords.GetDeliveryNoteListById(Id);
                    //Dictionary<long, IList<DeliveryNoteBasedReport_Vw>> DeliveryNoteReportCount1 = null;
                    Dictionary<long, DeliveryNoteBasedReport_Vw> objEmp = new Dictionary<long, DeliveryNoteBasedReport_Vw>();
                    IList<DeliveryNoteBasedReport_Vw> NewDeliveryNoteReportList = new List<DeliveryNoteBasedReport_Vw>();
                    DeliveryNoteBasedReport_Vw DelNoteReport = new DeliveryNoteBasedReport_Vw();
                    long i = 1;
                    foreach (var item in DeliveryNoteReportCount.FirstOrDefault().Value)
                    {
                        DeliveryNoteBasedReport_Vw obj = new DeliveryNoteBasedReport_Vw();
                        if (NewDeliveryNoteReportList.Count > 0)
                        {
                            //var TmpLst = (from u in DeliveryNoteReportCount1.FirstOrDefault().Value
                            //              where u.ControlId == item.ControlId
                            //              select u).ToList();
                            bool test = NewDeliveryNoteReportList.Any(items => items.ControlId == item.ControlId);
                            if (test == true)
                            {
                                obj.ControlId = item.ControlId;
                                obj.Sector = item.Sector;
                                obj.ContingentType = item.ContingentType;
                                obj.Name = item.Name;
                                obj.Location = item.Location;
                                obj.PeriodYear = item.PeriodYear;
                                obj.Period = item.Period;
                                obj.DeliveryNoteName = item.DeliveryNoteName;
                                obj.ExpectedDeliveryDate = item.ExpectedDeliveryDate;
                                obj.ReceivedDate = item.ReceivedDate;
                                obj.DateDiffer = item.DateDiffer;
                                obj.DNType = item.DNType;

                                obj.TotalWeight = item.TotalWeight;
                                obj.LineItemOrdered = item.LineItemOrdered;
                                obj.SumOfAccRecQty = item.SumOfAccRecQty;
                                obj.LineItemsDelivered = item.LineItemsDelivered;
                                obj.PercentByLineItemsDelevered = item.PercentByLineItemsDelevered;
                                obj.PercentByDeliveredQty = item.PercentByDeliveredQty;
                                obj.SubstitutionCount = item.SubstitutionCount;

                                obj.GrossAmount = 0;
                                obj.TmpGross = "";
                                obj.APL_TimelyDelivery = 0;
                                obj.TmpAPL_TimelyDelivery = "";
                                obj.APL_OrderbyLineItems = 0;
                                obj.TmpAPL_OrderbyLineItems = "";
                                obj.APL_OrdersbyWeight = 0;
                                obj.TmpAPL_OrdersbyWeight = "";
                                obj.APL_NoofAuthorizedSubstitutions = 0;
                                obj.TmpAPL_NoofAuthorizedSubstitutions = "";
                                obj.TOTALAPL = 0;
                                obj.TmpTotalAPL = "";
                            }
                            else
                            {
                                obj.ControlId = item.ControlId;
                                obj.Sector = item.Sector;
                                obj.ContingentType = item.ContingentType;
                                obj.Name = item.Name;
                                obj.Location = item.Location;
                                obj.PeriodYear = item.PeriodYear;
                                obj.Period = item.Period;
                                obj.DeliveryNoteName = item.DeliveryNoteName;
                                obj.ExpectedDeliveryDate = item.ExpectedDeliveryDate;
                                obj.ReceivedDate = item.ReceivedDate;
                                obj.DateDiffer = item.DateDiffer;
                                obj.DNType = item.DNType;
                                obj.TotalWeight = item.TotalWeight;
                                obj.LineItemOrdered = item.LineItemOrdered;
                                obj.SumOfAccRecQty = item.SumOfAccRecQty;
                                obj.LineItemsDelivered = item.LineItemsDelivered;
                                obj.PercentByLineItemsDelevered = item.PercentByLineItemsDelevered;
                                obj.PercentByDeliveredQty = item.PercentByDeliveredQty;
                                obj.SubstitutionCount = item.SubstitutionCount;

                                obj.GrossAmount = item.GrossAmount;
                                obj.APL_TimelyDelivery = item.APL_TimelyDelivery;
                                obj.APL_OrderbyLineItems = item.APL_OrderbyLineItems;
                                obj.APL_OrdersbyWeight = item.APL_OrdersbyWeight;
                                obj.APL_NoofAuthorizedSubstitutions = item.APL_NoofAuthorizedSubstitutions;
                                obj.TOTALAPL = item.TOTALAPL;
                            }
                        }
                        else
                        {
                            obj.ControlId = item.ControlId;
                            obj.Sector = item.Sector;
                            obj.ContingentType = item.ContingentType;
                            obj.Name = item.Name;
                            obj.Location = item.Location;
                            obj.PeriodYear = item.PeriodYear;
                            obj.Period = item.Period;
                            obj.DeliveryNoteName = item.DeliveryNoteName;
                            obj.ExpectedDeliveryDate = item.ExpectedDeliveryDate;
                            obj.ReceivedDate = item.ReceivedDate;
                            obj.DateDiffer = item.DateDiffer;
                            obj.DNType = item.DNType;
                            obj.TotalWeight = item.TotalWeight;
                            obj.LineItemOrdered = item.LineItemOrdered;
                            obj.SumOfAccRecQty = item.SumOfAccRecQty;
                            obj.LineItemsDelivered = item.LineItemsDelivered;
                            obj.PercentByLineItemsDelevered = item.PercentByLineItemsDelevered;
                            obj.PercentByDeliveredQty = item.PercentByDeliveredQty;
                            obj.SubstitutionCount = item.SubstitutionCount;

                            obj.GrossAmount = item.GrossAmount;
                            obj.APL_TimelyDelivery = item.APL_TimelyDelivery;
                            obj.APL_OrderbyLineItems = item.APL_OrderbyLineItems;
                            obj.APL_OrdersbyWeight = item.APL_OrdersbyWeight;
                            obj.APL_NoofAuthorizedSubstitutions = item.APL_NoofAuthorizedSubstitutions;
                            obj.TOTALAPL = item.TOTALAPL;
                        }



                        NewDeliveryNoteReportList.Add(obj);
                        i++;
                    }
                    string Docname = "";
                    long totalrecords = 0;
                    int totalpages = 0;

                    if (ExptType == "Excel")
                    {

                        Docname = "DeliveryNotebasedReport";
                        int j = 1;
                        foreach (var item in NewDeliveryNoteReportList)
                        {
                            item.Id = j;
                            j = j + 1;
                        }
                        var List = NewDeliveryNoteReportList;
                        List<string> lstHeader = new List<string>() { 
                    "Sl.No","Requisition Name","DN Number","Approved Date as per DP","Recd Date @ Contingent","Difference b/w Approved date & Recd Date @ contingent","DN Type","APL Application",
                    "Remark","Ordered Qty per contingent","Line Item Ordered","Sum of Actual Received Qty.(Egg Convt)","Line Items Delivered","% by Line Items Delivered","% by Delivered Qty","Substitution Count",
                    "Gross Amount","Timely Delivery","Order by Line Items","Orders by Weight","No. of  Authorized Substitutions","Total APL"};
                        base.NewExportToExcel(List, "" + Docname + "-" + NewDeliveryNoteReportList[0].Period + "-" + NewDeliveryNoteReportList[0].PeriodYear, (item => new
                        {
                            Id = item.Id,
                            ControlId = item.ControlId,
                            DeliveryNoteName = item.DeliveryNoteName,
                            ExpectedDeliveryDate = String.Format("{0:dd-MMM-yy}", item.ExpectedDeliveryDate),//item.ExpectedDeliveryDate,//String.Format("{0:d}", item.Approveddeliverydate)
                            ReceivedDate = String.Format("{0:dd-MMM-yy}", item.ReceivedDate),//item.ReceivedDate,
                            DateDiffer = item.DateDiffer,
                            DNType = item.DNType,
                            APL_Application = item.APL_Application,
                            Remark = item.Remark,
                            TotalWeight = item.TotalWeight,
                            LineItemOrdered = item.LineItemOrdered,
                            SumOfAccRecQty = item.SumOfAccRecQty,
                            LineItemsDelivered = item.LineItemsDelivered,
                            PercentByLineItemsDelevered = item.PercentByLineItemsDelevered,
                            PercentByDeliveredQty = item.PercentByDeliveredQty,
                            SubstitutionCount = item.SubstitutionCount,
                            GrossAmount = item.GrossAmount,
                            APL_TimelyDelivery = item.APL_TimelyDelivery,
                            APL_OrderbyLineItems = item.APL_OrderbyLineItems,
                            APL_OrdersbyWeight = item.APL_OrdersbyWeight,
                            APL_NoofAuthorizedSubstitutions = item.APL_NoofAuthorizedSubstitutions,
                            TOTALAPL = item.TOTALAPL,
                        }), lstHeader);
                        return new EmptyResult();
                    }
                    else
                    {
                        if (NewDeliveryNoteReportList != null && NewDeliveryNoteReportList.Count > 0)
                        {
                            totalrecords = NewDeliveryNoteReportList.Count();
                            totalpages = (int)Math.Ceiling(totalrecords / (float)rows);
                            var jsonData = new
                            {
                                total = totalpages,
                                page = page,
                                records = totalrecords,
                                rows = (from items in NewDeliveryNoteReportList
                                        select new
                                        {
                                            j = 2,
                                            cell = new string[]{
                                        items.Id.ToString(),
                                        //items.OrderId.ToString(),
                                        items.ControlId,
                                        //items.Sector,
                                        //items.ContingentType,
                                        //items.Name,
                                        //items.Location,
                                        //items.PeriodYear,
                                        //items.Period,
                                        items.DeliveryNoteName,
                                        
                                        items.ExpectedDeliveryDate.ToString(),
                                        //items.ExpectedDeliveryDate .ToString("{0:dd-MMM-yy}"),
                                        items.ReceivedDate.ToString(),
                                        //ReceivedDate = String.Format("{0:dd-MMM-yy}", item.ReceivedDate),

                                        items.DateDiffer.ToString(),
                                        items.DNType,
                                        items.APL_Application,
                                        items.Remark,

                                        items.TotalWeight.ToString(),
                                        items.LineItemOrdered.ToString(),
                                        items.SumOfAccRecQty.ToString(),
                                        items.LineItemsDelivered.ToString(),
                                        items.PercentByLineItemsDelevered.ToString(),
                                        items.PercentByDeliveredQty.ToString(),
                                        items.SubstitutionCount.ToString(),
                                        //items.GrossAmount!=0?items.GrossAmount.ToString():"",
                                        items.TmpGross!=""?items.GrossAmount.ToString():items.TmpGross,
                                        //items.APL_TimelyDelivery!=0?items.APL_TimelyDelivery.ToString():"",
                                        items.TmpAPL_TimelyDelivery!=""?items.APL_TimelyDelivery.ToString():items.TmpAPL_TimelyDelivery,
                                        //items.APL_OrderbyLineItems!=0?items.APL_OrderbyLineItems.ToString():"",
                                        items.TmpAPL_OrderbyLineItems!=""?items.APL_OrderbyLineItems.ToString():items.TmpAPL_OrderbyLineItems,
                                        //items.APL_OrdersbyWeight!=0?items.APL_OrdersbyWeight.ToString():"",
                                        items.TmpAPL_OrdersbyWeight!=""?items.APL_OrdersbyWeight.ToString():items.TmpAPL_OrdersbyWeight,
                                        //items.APL_NoofAuthorizedSubstitutions!=0?items.APL_NoofAuthorizedSubstitutions.ToString():"",
                                        items.TmpAPL_NoofAuthorizedSubstitutions!=""?items.APL_NoofAuthorizedSubstitutions.ToString():items.TmpAPL_NoofAuthorizedSubstitutions,
                                        //items.TOTALAPL!=0?items.TOTALAPL.ToString():""
                                        items.TmpTotalAPL!=""?items.TOTALAPL.ToString():items.TmpTotalAPL
                                    }
                                        })
                            };
                            return Json(jsonData, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var Empty = new { rows = (new { cell = new string[] { } }) };
                            return Json(Empty, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        #endregion

        #region FinalFood Order Details
        public ActionResult FinalFoodOrderReport()
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                return View();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightReportPolicy");
                throw ex;
            }
        }
        public ActionResult FinalFoodOrderReportjqGrid(string ReportFlag, string Period, string PeriodYear, int rows, string sidx, string sord, int? page = 1)
        {
            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            try
            {
                bool IsSubsite = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSubsite"]);
                string SubsiteName = ConfigurationManager.AppSettings["SubsiteName"].ToString();
                if (string.IsNullOrWhiteSpace(Period) && string.IsNullOrWhiteSpace(PeriodYear))
                {

                    return null;
                }
                else
                {
                    OrdersService ords = new OrdersService();
                    Dictionary<string, object> criteria = new Dictionary<string, object>();
                    sord = sord == "desc" ? "Desc" : "Asc";
                    criteria.Clear();
                    criteria.Add("Period", Period);
                    criteria.Add("PeriodYear", PeriodYear);
                    Dictionary<long, IList<FinalFoodOrderDetails_vw>> OrderList = ords.GetFinalOrderListWithPagingAndCriteria(page - 1, rows, sord, sidx, criteria);
                    if (OrderList != null && OrderList.Count > 0 && OrderList.FirstOrDefault().Key > 0 && OrderList.FirstOrDefault().Value.Count > 0)
                    {
                        long totalRecords = OrderList.First().Key;
                        int totalPages = (int)Math.Ceiling(totalRecords / (float)rows);
                        var jsonData = new
                        {
                            total = totalPages,
                            page = page,
                            records = totalRecords,
                            rows = (
                            from items in OrderList.First().Value
                            select new
                            {
                                i = items.Id,
                                cell = new string[] { 
                                items.Id.ToString(), 
                                items.Period,
                                items.PeriodYear,
                                items.TroopsStrength.ToString(),
                                items.TotalAmount.ToString(),
                                items.TotalWeight.ToString(),
                                IsSubsite==true?String.Format(@"<a style='color:#034af3;text-decoration:underline' href= '/"+SubsiteName+"/Reports/GenerateFinalFoodOrderReport?Period="+items.Period+"&PeriodYear="+items.PeriodYear+"&ReportFlag="+ReportFlag+"' target='_Blank' >{0}</a>","Download"):String.Format(@"<a style='color:#034af3;text-decoration:underline' href= '/Reports/GenerateFinalFoodOrderReport?Period="+items.Period+"&PeriodYear="+items.PeriodYear+"&ReportFlag="+ReportFlag+"' target='_Blank' >{0}</a>","Download")
                                //String.Format(@"<a style='color:#034af3;text-decoration:underline' href= '/Reports/GenerateFinalFoodOrderReport?Period="+items.Period+"&PeriodYear="+items.PeriodYear+"&ReportFlag="+ReportFlag+"' target='_Blank' >{0}</a>","Download")
                                }
                            })
                        };
                        return Json(jsonData, JsonRequestBehavior.AllowGet);
                    }
                    var Empty = new { rows = (new { cell = new string[] { } }) };
                    return Json(Empty, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightReportPolicy");
                throw ex;
            }
        }

        public ActionResult GenerateFinalFoodOrderReport(string ReportFlag, string Period, string PeriodYear)
        {
            try
            {
                OrdersService ords = new OrdersService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();

                criteria.Clear();
                if (!string.IsNullOrWhiteSpace(Period)) { criteria.Add("Period", Period); }
                if (!string.IsNullOrWhiteSpace(PeriodYear)) { criteria.Add("PeriodYear", PeriodYear); }
                if (ReportFlag == "Report-2")
                {
                    GenerateFinalRequisitionReport(Period, PeriodYear);
                }
                else
                {
                    Dictionary<long, IList<FinalFoodOrderdetails_SP>> FinalOrderList = OS.GetFinalFoodOrderListbySP(Period, PeriodYear);
                    IList<FinalFoodOrderdetails_SP> FinalFoodOrderList = new List<FinalFoodOrderdetails_SP>();
                    if (FinalOrderList != null && FinalOrderList.Count > 0 && FinalOrderList.FirstOrDefault().Key > 0 && FinalOrderList.FirstOrDefault().Value.Count > 0)
                    {
                        long i = 1;
                        foreach (var item in FinalOrderList.FirstOrDefault().Value)
                        {
                            FinalFoodOrderdetails_SP obj = new FinalFoodOrderdetails_SP();
                            obj.Id = item.Id;
                            obj.Line_No = item.Line_No;
                            obj.FFOId = item.FFOId;
                            obj.UNCode = item.UNCode;
                            obj.Commodity = item.Commodity;
                            obj.ControlId = item.ControlId;
                            obj.Warehouse = item.Warehouse;
                            obj.Sector = item.Sector;
                            obj.Location = item.Location;
                            obj.Loc_Contingent = item.Loc_Contingent != null ? item.Loc_Contingent.Replace(" ", "-") : "";
                            obj.Name = item.Name;
                            obj.Period = item.Period;
                            obj.Week = item.Week;
                            obj.PeriodYear = item.PeriodYear;
                            obj.LocationCMR = item.LocationCMR;
                            obj.ControlCMR = item.ControlCMR;
                            obj.OrderQty = Math.Round(item.OrderQty, 3);
                            //obj.OrderQty = item.OrderQty;
                            obj.SectorPrice = item.SectorPrice;
                            obj.Total = item.Total;
                            obj.Troops = item.Troops;
                            obj.StartDate = item.StartDate;
                            obj.EndDate = item.EndDate;
                            obj.DP = item.DP;

                            FinalFoodOrderList.Add(obj);
                            i++;
                        }
                        string Docname = "Final Food Order details";
                        //int j = 1;
                        //foreach (var item in FinalFoodOrderList)
                        //{
                        //    item.Id = j;
                        //    j = j + 1;
                        //}
                        var List = FinalFoodOrderList;

                        //    List<string> lstHeader = new List<string>() { 
                        //"Sl.No","UNCode","Commodity","Control Number","Warehouse","Sector","Delivery Location","Loc_Contingent",
                        //"Contingent","Period","Week","Year","CMR","CAL","Order Qty(Kg/Ltr/Each)","Sector Price",
                        //"Values","Troops","Start Date","End Date"};
                        List<string> lstHeader = new List<string>() { 
                    "Sl.No","Line#","UNCode","Commodity","DP","FFO#","Control Number","Warehouse","Sector","Delivery Location","Loc_Contingent",
                    "Contingent","Period","Week","Year","CMR","Order Qty(Kg/Ltr/Each)","Sector Price",
                    "Values","Troops","Start Date","End Date"};
                        base.NewExportToExcel(List, "" + Docname + "-" + Period + "-" + PeriodYear, (item => new
                        {
                            Id = item.Id,
                            Line_No = item.Line_No,
                            UNCode = item.UNCode,
                            Commodity = item.Commodity,
                            DP = item.DP,
                            FFOId = item.FFOId,
                            ControlId = item.ControlId,
                            Warehouse = item.Warehouse,
                            Sector = item.Sector,
                            DeliveryLocation = item.Location,
                            Loc_Contingent = item.Loc_Contingent,
                            Contingent = item.Name,
                            Period = item.Period,
                            Week = item.Week,
                            PeriodYear = item.PeriodYear,
                            LocationCMR = item.LocationCMR,
                            //ControlCMR = item.ControlCMR,
                            OrderQty = item.OrderQty,
                            //OrderQty = Math.Round(item.OrderQty, 3),
                            //OrderQty =string.Format("{0:N}",(String.Format("{0:0.000}", item.OrderQty))),
                            SectorPrice = item.SectorPrice,
                            Total = item.Total,
                            Troops = item.Troops,
                            StartDate = String.Format("{0:dd-MMM-yy}", item.StartDate),
                            EndDate = String.Format("{0:dd-MMM-yy}", item.EndDate),

                        }), lstHeader);
                        return new EmptyResult();
                    }
                }


                return Json(new { success = true, result = "Report Generated successfully." }, "text/html", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightReportPolicy");
                throw ex;
            }
        }
        #endregion

        #region Final Food Requisition
        public ActionResult FinalFoodRequisitionReport()
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                return View();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightReportPolicy");
                throw ex;
            }
        }

        public ActionResult ConsolidateFoodReport()
        {

            try
            {
                Orders ord = new Orders();
                return View();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightReportPolicy");
                throw ex;

            }
        }
        [HttpPost]
        public ActionResult ConsolidateFoodReport(Orders Ord)
        {
            string Period = Request.Form["Period"].ToString().Replace(",", "");
            string PeriodYear = Request.Form["PeriodYear"].ToString().Replace(",", "");


            {
                MastersService mssvc = new MastersService();


                //Deleting the old Documents
                OS.DeleteConsolidateFoodReport(Period, PeriodYear);

                //string[] Reports = new string[] { "Analysis", "Final " + Period, "Summary", "Weekly Consolidated", "Final " + Period + "-WK1", "Final " + Period + "-WK2", "Final " + Period + "-WK3", "Final " + Period + "-WK4", "Bulk Food Order " + Period };
                string[] Reports = new string[] { "Final " + Period + "-WK1", "Final " + Period + "-WK2", "Final " + Period + "-WK3", "Final " + Period + "-WK4", "Weekly Consolidated", "Bulk Food Order " + Period, "Analysis", "Final " + Period, "Summary" };

                DataSet DS = new DataSet("Workbook");

                criteria.Clear();
                criteria.Add("Period", Period);
                criteria.Add("PeriodYear", PeriodYear);
                Dictionary<long, IList<Orders>> FinalOrderList = OS.GetOrdersListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                var WeekList = (from u in FinalOrderList.FirstOrDefault().Value orderby u.Week ascending select u.Week).Distinct().ToList();


                criteria.Clear();

                List<FinalFoodOrderbyWeek> WK1OrdList = new List<FinalFoodOrderbyWeek>();
                string WeekFlag = "";
                foreach (var item in Reports)
                {
                    if (item == "Final " + Period + "-WK1")
                    {



                        WeekFlag = Convert.ToString(WeekList[0]);
                        FinalFoodOrderbyWeek OrdListWk1 = GetFinalFoodOrderByWeeK(Period, PeriodYear, WeekFlag, item);
                        WK1OrdList.Add(OrdListWk1);






                    }
                    if (item == "Final " + Period + "-WK2")
                    {



                        WeekFlag = Convert.ToString(WeekList[1]);

                        FinalFoodOrderbyWeek OrdListWk1 = GetFinalFoodOrderByWeeK(Period, PeriodYear, WeekFlag, item);
                        WK1OrdList.Add(OrdListWk1);



                    }
                    if (item == "Final " + Period + "-WK3")
                    {

                        WeekFlag = Convert.ToString(WeekList[2]);
                        FinalFoodOrderbyWeek OrdListWk1 = GetFinalFoodOrderByWeeK(Period, PeriodYear, WeekFlag, item);
                        WK1OrdList.Add(OrdListWk1);



                    }
                    if (item == "Final " + Period + "-WK4")
                    {


                        WeekFlag = Convert.ToString(WeekList[3]);

                        FinalFoodOrderbyWeek OrdListWk1 = GetFinalFoodOrderByWeeK(Period, PeriodYear, WeekFlag, item);
                        WK1OrdList.Add(OrdListWk1);



                    }
                    if (item == "Weekly Consolidated")
                    {


                        //  WeekFlag = Convert.ToString(WeekList[3]);
                        // FinalFoodOrderbyWeek OrdListWk1 = GetFinalFoodOrderByWeeK(Period, PeriodYear, WeekFlag, item);
                        WK1OrdList.Add(null);

                    }
                    if (item == "Bulk Food Order " + Period)
                    {

                        // WeekFlag = Convert.ToString(WeekList[3]);
                        // FinalFoodOrderbyWeek OrdListWk1 = GetFinalFoodOrderByWeeK(Period, PeriodYear, WeekFlag, item);
                        WK1OrdList.Add(null);


                    }
                    if (item == "Analysis")
                    {

                        //WeekFlag = Convert.ToString(WeekList[3]);
                        //  FinalFoodOrderbyWeek OrdListWk1 = GetFinalFoodOrderByWeeK(Period, PeriodYear, WeekFlag, item);
                        WK1OrdList.Add(null);


                    }
                    if (item == "Summary")
                    {

                        WeekFlag = Convert.ToString(WeekList[3]);
                        // FinalFoodOrderbyWeek OrdListWk1 = GetFinalFoodOrderByWeeK(Period, PeriodYear, WeekFlag, item);
                        WK1OrdList.Add(null);


                    }
                    if (item == "Final " + Period)
                    {

                        WeekFlag = Convert.ToString(WeekList[3]);
                        // FinalFoodOrderbyWeek OrdListWk1 = GetFinalFoodOrderByWeeK(Period, PeriodYear, WeekFlag, item);
                        WK1OrdList.Add(null);


                    }

                }

                foreach (var item in Reports)
                {

                    DataTable table = new DataTable();
                    table.TableName = item;
                    DS.Tables.Add(table);
                }



                base.FinalFoodRequistionExcelSheet(DS, WK1OrdList);


                return Json("Consolidated Food Report Generated Successfully !", JsonRequestBehavior.AllowGet);
            }


        }
        public JsonResult GenerateFinalRequisitionReport(string Period, string PeriodYear)
        {
            try
            {
                MastersService mssvc = new MastersService();


                //Deleting the old Documents
                OS.DeleteConsolidateFoodReport(Period, PeriodYear);

                //string[] Reports = new string[] { "Analysis", "Final " + Period, "Summary", "Weekly Consolidated", "Final " + Period + "-WK1", "Final " + Period + "-WK2", "Final " + Period + "-WK3", "Final " + Period + "-WK4", "Bulk Food Order " + Period };
                string[] Reports = new string[] { "Final " + Period + "-WK1", "Final " + Period + "-WK2", "Final " + Period + "-WK3", "Final " + Period + "-WK4", "Weekly Consolidated", "Bulk Food Order " + Period, "Analysis", "Final " + Period, "Summary" };

                DataSet DS = new DataSet("Workbook");

                criteria.Clear();
                criteria.Add("Period", Period);
                criteria.Add("PeriodYear", PeriodYear);
                Dictionary<long, IList<Orders>> FinalOrderList = OS.GetOrdersListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                var WeekList = (from u in FinalOrderList.FirstOrDefault().Value select u.Week).Distinct().ToList();


                criteria.Clear();

                List<FinalFoodOrderbyWeek> WK1OrdList = new List<FinalFoodOrderbyWeek>();
                string WeekFlag = "";
                foreach (var item in Reports)
                {
                    if (item == "Final " + Period + "-WK1")
                    {
                        WeekFlag = Convert.ToString(WeekList[0]);
                        FinalFoodOrderbyWeek OrdListWk1 = GetFinalFoodOrderByWeeK(Period, PeriodYear, WeekFlag, item);
                        WK1OrdList.Add(OrdListWk1);
                    }
                    if (item == "Final " + Period + "-WK2")
                    {
                        WeekFlag = Convert.ToString(WeekList[1]);

                        FinalFoodOrderbyWeek OrdListWk1 = GetFinalFoodOrderByWeeK(Period, PeriodYear, WeekFlag, item);
                        WK1OrdList.Add(OrdListWk1);
                    }
                    if (item == "Final " + Period + "-WK3")
                    {
                        WeekFlag = Convert.ToString(WeekList[2]);
                        FinalFoodOrderbyWeek OrdListWk1 = GetFinalFoodOrderByWeeK(Period, PeriodYear, WeekFlag, item);
                        WK1OrdList.Add(OrdListWk1);
                    }
                    if (item == "Final " + Period + "-WK4")
                    {
                        WeekFlag = Convert.ToString(WeekList[3]);

                        FinalFoodOrderbyWeek OrdListWk1 = GetFinalFoodOrderByWeeK(Period, PeriodYear, WeekFlag, item);
                        WK1OrdList.Add(OrdListWk1);
                    }
                    if (item == "Weekly Consolidated")
                    {
                        //  WeekFlag = Convert.ToString(WeekList[3]);
                        // FinalFoodOrderbyWeek OrdListWk1 = GetFinalFoodOrderByWeeK(Period, PeriodYear, WeekFlag, item);
                        WK1OrdList.Add(null);
                    }
                    if (item == "Bulk Food Order " + Period)
                    {
                        // WeekFlag = Convert.ToString(WeekList[3]);
                        // FinalFoodOrderbyWeek OrdListWk1 = GetFinalFoodOrderByWeeK(Period, PeriodYear, WeekFlag, item);
                        WK1OrdList.Add(null);
                    }
                    if (item == "Analysis")
                    {
                        //WeekFlag = Convert.ToString(WeekList[3]);
                        //  FinalFoodOrderbyWeek OrdListWk1 = GetFinalFoodOrderByWeeK(Period, PeriodYear, WeekFlag, item);
                        WK1OrdList.Add(null);
                    }
                    if (item == "Summary")
                    {
                        WeekFlag = Convert.ToString(WeekList[3]);
                        // FinalFoodOrderbyWeek OrdListWk1 = GetFinalFoodOrderByWeeK(Period, PeriodYear, WeekFlag, item);
                        WK1OrdList.Add(null);
                    }
                    if (item == "Final " + Period)
                    {
                        WeekFlag = Convert.ToString(WeekList[3]);
                        // FinalFoodOrderbyWeek OrdListWk1 = GetFinalFoodOrderByWeeK(Period, PeriodYear, WeekFlag, item);
                        WK1OrdList.Add(null);
                    }
                }
                foreach (var item in Reports)
                {
                    DataTable table = new DataTable();
                    table.TableName = item;
                    DS.Tables.Add(table);
                }
                base.FinalFoodRequistionExcelSheet(DS, WK1OrdList);
                return Json("Consolidated Food Report Generated Successfully !", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightReportPolicy");
                return Json("Consolidated Food Report Generated Failed !", JsonRequestBehavior.AllowGet);
                // throw ex;
            }
        }
        private FinalFoodOrderbyWeek GetFinalFoodOrderByWeeK(string Period, string PeriodYear, string WeekFlag, string sheetName)
        {
            try
            {
                MastersService mssvc = new MastersService();

                criteria.Clear();

                //criteria.Add("Period", Period);
                //criteria.Add("PeriodYear", PeriodYear);
                //criteria.Add("Week", Convert.ToInt64(WeekFlag));

                Dictionary<long, IList<ORDRPT_FinalFoodOrderReport_SP>> FinalOrderList = OS.GetFinalFoodOrderRequistionListbySP(Period, PeriodYear, Convert.ToInt64(WeekFlag));
                var ControlId = (from u in FinalOrderList.FirstOrDefault().Value select u.ControlId).Distinct().ToArray();

                List<ORDRPT_FinalFoodOrderReport_SP> TempOrdList = new List<ORDRPT_FinalFoodOrderReport_SP>();
                for (int i = 1; i < ControlId.Length; i++)
                {
                    var ItemList = (from u in FinalOrderList.FirstOrDefault().Value where u.ControlId == ControlId[i] select u).ToList();

                    foreach (var item in ItemList)
                    {
                        ORDRPT_FinalFoodOrderReport_SP tempobj = new ORDRPT_FinalFoodOrderReport_SP();

                        tempobj.OrderId = item.OrderId;
                        tempobj.ControlId = item.ControlId;
                        tempobj.Sector = item.Sector;
                        tempobj.Location = item.Location;
                        tempobj.Name = item.Name;
                        tempobj.SectorLocContingent = item.SectorLocContingent;
                        tempobj.Week = item.Week;
                        tempobj.Period = item.Period;
                        tempobj.PeriodYear = item.PeriodYear;
                        tempobj.Troops = item.Troops;
                        tempobj.Warehouse = item.Warehouse;
                        tempobj.UNCode = item.UNCode;
                        tempobj.Commodity = item.Commodity;
                        tempobj.OrderQty = item.OrderQty;
                        tempobj.UNCode1 = item.UNCode1;
                        tempobj.Commodity1 = item.Commodity1;

                        tempobj.sector1 = item.sector1;
                        tempobj.sector2 = item.sector2;
                        tempobj.sector3 = item.sector3;
                        tempobj.TotalOrdQty = item.TotalOrdQty;
                        tempobj.QtyWithEggs = item.QtyWithEggs;
                        tempobj.TotalTroops = item.TotalTroops;

                        tempobj.SSOrders = item.SSOrders;
                        tempobj.SNOrders = item.SNOrders;
                        tempobj.SWOrders = item.SWOrders;

                        tempobj.SSTroops = item.SSTroops;
                        tempobj.SNTroops = item.SNTroops;
                        tempobj.SWTroops = item.SWTroops;

                        TempOrdList.Add(tempobj);
                    }
                }
                return new FinalFoodOrderbyWeek()
                {

                    FinalFoodOrderList = TempOrdList,
                    SheetName = sheetName,
                    Title = "Final Requisition UNAMID " + Period + " WEEK-" + WeekFlag + " " + PeriodYear
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightReportPolicy");
                throw ex;
            }
        }

        #endregion


        #region Local Purchase
        public ActionResult LocalPruchaseDetails()
        {

            return View();
        }
        public ActionResult GetLocalPurchaseDetailsListJqGrid(LocalPurchase localPurchaseFiles, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                sord = sord == "desc" ? "Desc" : "Asc";
                if (!string.IsNullOrEmpty(localPurchaseFiles.Period))
                    criteria.Add("Period", ((localPurchaseFiles.Period)));
                if (!string.IsNullOrEmpty(localPurchaseFiles.PeriodYear))
                    criteria.Add("PeriodYear", ((localPurchaseFiles.PeriodYear)));
                if (!string.IsNullOrEmpty(localPurchaseFiles.DocumentType))
                    criteria.Add("DocumentType", ((localPurchaseFiles.DocumentType)));
                Dictionary<long, IList<LocalPurchase>> LocalPurchaseFiles = null;
                // PayRollService payRollService = new PayRollService();
                if (string.IsNullOrEmpty(localPurchaseFiles.Period) && string.IsNullOrEmpty(localPurchaseFiles.PeriodYear) && string.IsNullOrEmpty(localPurchaseFiles.DocumentType))
                {

                    var Empty = new { rows = (new { cell = new string[] { } }) };
                    return Json(Empty, JsonRequestBehavior.AllowGet);
                }
                LocalPurchaseFiles = OS.GetLocalPurchaseFilesListWithPagingAndCriteria(page - 1, 9999, sidx, sord, criteria);
                if (LocalPurchaseFiles == null || LocalPurchaseFiles.FirstOrDefault().Key == 0)
                {
                    var Empty = new { rows = (new { cell = new string[] { } }) };
                    return Json(Empty, JsonRequestBehavior.AllowGet);
                }

                else
                {
                    IList<LocalPurchase> localPurchaseFilesList = LocalPurchaseFiles.FirstOrDefault().Value;
                    long totalRecords = LocalPurchaseFiles.FirstOrDefault().Key;
                    int totalPages = (int)Math.Ceiling(totalRecords / (float)rows);
                    var jsonData = new
                    {
                        total = totalPages,
                        page = page,
                        records = totalRecords,
                        rows =
                        (
                        from items in localPurchaseFilesList
                        select new
                        {
                            i = items.Id,
                            cell = new string[]
                           {
                               items.Id.ToString(),
                                items.Period,
                                items.PeriodYear,
                                items.DocumentName,
                                items.DocumentData!=null?items.DocumentData.ToString():"",
                                items.CreatedBy,
                                items.CreatedDate!=null?items.CreatedDate.Value.ToString("dd/MM/yyyy"):"",
                                items.ModifiedBy,
                                items.ModifiedDate!=null?items.ModifiedDate.Value.ToString("dd/MM/yyyy"):"",
                                items.DocumentType
                   
                                
                           }
                        }
                        )
                    };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                //ExceptionPolicy.HandleException(ex, "MasterPolicy");
                throw ex;
            }
        }




        public ActionResult DownloadLocalPurchaseZip(string Period, string PeriodYear, string DocumentType)
        {
            string userId = base.ValidateUser();
            string ZipFileName = string.Empty;
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
            {
                try
                {

                    if (Period != "" && PeriodYear != "" && DocumentType != "")
                    {
                        if (!string.IsNullOrWhiteSpace(Period)) { criteria.Add("Period", Period); }
                        if (!string.IsNullOrWhiteSpace(PeriodYear)) { criteria.Add("PeriodYear", PeriodYear); }
                        if (!string.IsNullOrWhiteSpace(DocumentType)) { criteria.Add("DocumentType", DocumentType); }

                    }
                    else
                        return null;

                    Dictionary<long, IList<LocalPurchase>> documents = OS.GetLocalPurchaseFilesListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                    //Dictionary<long, IList<DeliveryNoteOrders_vw>> delnote = orderService.GetDeliveryNoteOrderListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                    IList<LocalPurchase> documentslist = null;
                    if (documents != null && documents.Count > 0)
                    {
                        documentslist = documents.FirstOrDefault().Value;
                    }


                    //Create a Output Memory stream
                    MemoryStream outputMemStream = new MemoryStream();
                    //Create a Output Zipfile stream
                    ZipOutputStream zipStream = new ZipOutputStream(outputMemStream);

                    zipStream.SetLevel(3); //0-9, 9 being the highest level of compression

                    if (DocumentType == "Excel-Single")
                    {   //For Excel files


                        foreach (var item in documentslist)
                        {
                            if (item.DocumentData != null)
                            {
                                //Assigning Byte[] to a new stream 
                                MemoryStream stream = new MemoryStream(item.DocumentData);
                                ZipEntry xmlEntry = new ZipEntry(Period + "-" + PeriodYear + "-" + item.DocumentName + ".xlsx");
                                xmlEntry.DateTime = DateTime.Now;
                                zipStream.PutNextEntry(xmlEntry);
                                StreamUtils.Copy(stream, zipStream, new byte[4096]);
                                zipStream.CloseEntry(); //Close each Zip stream
                                ZipFileName = Period + "-" + PeriodYear + "-Excel-Single";
                            }
                        }
                    }

                    //Excel-Consol
                    if (DocumentType == "Excel-Consol")
                    {   //For Excel files


                        foreach (var item in documentslist)
                        {
                            if (item.DocumentData != null)
                            {
                                //Assigning Byte[] to a new stream 
                                MemoryStream stream = new MemoryStream(item.DocumentData);
                                ZipEntry xmlEntry = new ZipEntry(Period + "-" + PeriodYear + "-" + item.DocumentName + ".xlsx");
                                xmlEntry.DateTime = DateTime.Now;
                                zipStream.PutNextEntry(xmlEntry);
                                StreamUtils.Copy(stream, zipStream, new byte[4096]);
                                zipStream.CloseEntry(); //Close each Zip stream
                                ZipFileName = Period + "-" + PeriodYear + "-Excel-Consol";
                            }
                        }
                    }
                    //Transpt-Book
                    if (DocumentType == "Transpt-Book")
                    {   //For Excel files


                        foreach (var item in documentslist)
                        {
                            if (item.DocumentData != null)
                            {
                                //Assigning Byte[] to a new stream 
                                MemoryStream stream = new MemoryStream(item.DocumentData);
                                ZipEntry xmlEntry = new ZipEntry(Period + "-" + PeriodYear + "-" + item.DocumentName + ".xlsx");
                                xmlEntry.DateTime = DateTime.Now;
                                zipStream.PutNextEntry(xmlEntry);
                                StreamUtils.Copy(stream, zipStream, new byte[4096]);
                                zipStream.CloseEntry(); //Close each Zip stream
                                ZipFileName = Period + "-" + PeriodYear + "-Transpt-Book";
                            }
                        }
                    }

                    //PDF-Single
                    if (DocumentType == "PDF-Single")
                    {   //For Excel files


                        foreach (var item in documentslist)
                        {
                            if (item.DocumentData != null)
                            {
                                //Assigning Byte[] to a new stream 
                                MemoryStream stream = new MemoryStream(item.DocumentData);
                                ZipEntry xmlEntry = new ZipEntry(Period + "-" + PeriodYear + "-" + item.DocumentName + ".xlsx");
                                xmlEntry.DateTime = DateTime.Now;
                                zipStream.PutNextEntry(xmlEntry);
                                StreamUtils.Copy(stream, zipStream, new byte[4096]);
                                zipStream.CloseEntry(); //Close each Zip stream
                                ZipFileName = Period + "-" + PeriodYear + "-PDF-Single";
                            }
                        }
                    }


                    //Revised-Book

                    if (DocumentType == "Revised-Book")
                    {   //For Excel files


                        foreach (var item in documentslist)
                        {
                            if (item.DocumentData != null)
                            {
                                //Assigning Byte[] to a new stream 
                                MemoryStream stream = new MemoryStream(item.DocumentData);
                                ZipEntry xmlEntry = new ZipEntry(Period + "-" + PeriodYear + "-" + item.DocumentName + ".xlsx");
                                xmlEntry.DateTime = DateTime.Now;
                                zipStream.PutNextEntry(xmlEntry);
                                StreamUtils.Copy(stream, zipStream, new byte[4096]);
                                zipStream.CloseEntry(); //Close each Zip stream
                                ZipFileName = Period + "-" + PeriodYear + "-Revised-Book";
                            }
                        }
                    }


                    zipStream.IsStreamOwner = false;
                    zipStream.Close();

                    outputMemStream.Position = 0;

                    byte[] byteArray = outputMemStream.ToArray();

                    Response.Clear();
                    Response.AppendHeader("Content-Disposition", "attachment; filename=" + ZipFileName + ".zip");
                    Response.AppendHeader("Content-Length", byteArray.Length.ToString());
                    Response.ContentType = "application/octet-stream";
                    Response.BinaryWrite(byteArray);

                    return Json(true, JsonRequestBehavior.AllowGet);

                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }
        #endregion
        #region FIV Report
        public ActionResult FIVReportGeneration()
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                return View();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightReportPolicy");
                throw ex;
            }
        }
        public ActionResult GenerateFIVPopup()
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                return View();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightReportPolicy");
                throw ex;
            }
        }
        public void FIVParallelTasking(string searchItems)
        {
            try
            {
                new Task(() => { GenerateFIVinCSVParallel(searchItems); }).Start();

            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightReportPolicy");
                throw ex;
            }
        }
        public void GenerateFIVinCSVParallel(string searchItems)
        {
            try
            {
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                OrdersService orderService = new OrdersService();
                criteria.Clear();

                if (searchItems != null && searchItems != "")
                {
                    criteria.Clear();
                    var Items = searchItems.ToString().Split(',');
                    if (!string.IsNullOrWhiteSpace(Items[0])) { criteria.Add("Sector", Items[0]); }
                    if (!string.IsNullOrWhiteSpace(Items[1])) { criteria.Add("PeriodYear", Items[1]); }
                    if (!string.IsNullOrWhiteSpace(Items[2])) { criteria.Add("Period", Items[2]); }
                    if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Week", Convert.ToInt64(Items[3])); }

                    //getting all the Info from Orders and Document saved in Excel
                    Dictionary<long, IList<FIVItems_vw>> ordersList = orderService.GetFIVItemsListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                    criteria.Add("DocumentType", "FIV");
                    criteria.Add("DocumentFor", "VERIFIED");
                    Dictionary<long, IList<ExcelDocuments>> EXCELDocumentItems = IS.GetExcelDocumentListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                    //getting order list from orders
                    var FIVOrderList = (from items in ordersList.First().Value

                                        select items.OrderId).OrderBy(i => i).Distinct().ToArray();
                    //getting order list from EXCELDocumentItems
                    var ExcelOrderList = (from items in EXCELDocumentItems.First().Value
                                          where items.OrderId != 0
                                          select items.OrderId).OrderBy(i => i).Distinct().ToArray();

                    //For comparing Long arrays
                    long[] INVOrdList = FIVOrderList.Select(i => i).ToArray();
                    long[] ExcelOrdList = ExcelOrderList.Select(i => i).ToArray();

                    //Taking orderid's of Excel not in documents table
                    var T2 = INVOrdList.Except(ExcelOrdList);

                    //long []
                    long[] NewExcelOrderId = T2.Select(i => i).ToArray();

                    foreach (var item1 in NewExcelOrderId)
                    {
                        CallFIVinCSVParallel(item1);
                    }
                }

            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightReportPolicy");
                throw ex;
            }
        }
        public void CallFIVinCSVParallel(long OrderId)
        {
            try
            {
                OrdersService orderService = new OrdersService();
                //string userId = base.ValidateUser();
                string DocumentFor = string.Empty;
                Dictionary<long, IList<FIVItemsReport>> FIVReport = orderService.GetFIVItemsReportListbySP(Convert.ToString(OrderId));
                List<FIVItemsReport> FIVList = FIVReport.FirstOrDefault().Value.ToList();
                criteria.Clear();
                criteria.Add("OrderId", OrderId);
                Dictionary<long, IList<FIVItemsReport>> FIVmismatchReportList = orderService.GetFIVItemsReportListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                if (FIVmismatchReportList != null && FIVmismatchReportList.FirstOrDefault().Key > 0)
                {
                    long Count = 1;
                    foreach (var item in FIVmismatchReportList.FirstOrDefault().Value)
                    {
                        FIVItemsReport fivObj = new FIVItemsReport();
                        //fivObj.Id = item.Id;
                        fivObj.DeliveryNoteName = item.DeliveryNoteName;
                        fivObj.ControlId = item.ControlId;
                        fivObj.UNCode = item.UNCode;
                        fivObj.SubstituteItemCode = item.SubstituteItemCode;
                        fivObj.OrderQty = item.OrderQty;
                        //fivObj.DeliveredQty = item.DeliveredQty;
                        fivObj.DeliveredQty = 0;
                        fivObj.AcceptedQty = item.AcceptedQty;
                        fivObj.SectorPrice = item.SectorPrice;
                        fivObj.Total = item.Total;
                        fivObj.Comment = item.Comment;
                        fivObj.InvoiceNumber = item.InvoiceNumber;
                        fivObj.DiscrepancyCode = item.DiscrepancyCode;

                        FIVList.Add(fivObj);
                        Count = Count + 1;
                    }
                    DocumentFor = "FAILLED";
                }
                if (FIVList != null && FIVList.Count > 0)
                {
                    //var res = (from u in FIVList where u.Comment.Contains("MISMATCH") select u).ToList();
                    //var res = FIVList.Where(x => x.Comment.Contains("MISMATCH")).ToList();
                    bool IsMismatch = FIVList.Any(cus => cus.Comment!=null&&cus.Comment.Contains("MISMATCH"));
                    if (IsMismatch == true)
                    {
                        orderService.updateFIVStatusByOrderId(Convert.ToString(OrderId));//NOT DELIVERED
                        DocumentFor = "FAILLED";
                    }
                    //if (res != null && res.Count > 0)
                    //{
                    //    orderService.updateFIVStatusByOrderId(Convert.ToString(OrderId));//NOT DELIVERED
                    //    DocumentFor = "FAILLED";
                    //}
                }
                List<FIVItemsReport> FIVReportItems = FIVReport.FirstOrDefault().Value.ToList();
                if (FIVList != null && FIVList.Count > 0)
                {
                    var CSVFIVList = (from u in FIVList
                                      select new
                                      {
                                          u.DeliveryNoteName
                                          ,
                                          u.ControlId,
                                          u.UNCode,
                                          u.SubstituteItemCode,
                                          u.OrderQty,
                                          u.DeliveredQty,
                                          u.AcceptedQty,
                                          u.SectorPrice,
                                          u.Total
                                          ,
                                          u.Comment,
                                          u.InvoiceNumber,
                                          u.DiscrepancyCode
                                      }).ToList();
                    #region CSV Creation
                    string txtTName = "FIV-" + FIVList[0].ControlId;

                    var properties = typeof(FIVItemsReport).GetProperties();
                    var result = new StringBuilder();
                    string[] headerArr = { "Delivery_Note", "ControlId", "UNRS_Code", "UNRS_Code(S)", "Ordered_Quantity", "Sent_Quantity", "Accepted_Quantity", "Unit_Food_Price", "Total_Price", "Comment for Stock", "Invoice Number", "Desc. Code" };
                    foreach (var item in headerArr)
                    {
                        result.Append(item + ',');
                    }
                    result.AppendLine();

                    foreach (var item in CSVFIVList)
                    {
                        result.Append(item.DeliveryNoteName + ',' + item.ControlId + ',' + item.UNCode + ',' + item.SubstituteItemCode + ',' + item.OrderQty + ',' + item.DeliveredQty + ',' + item.AcceptedQty + ',' + item.SectorPrice + ',' + item.Total + ',' + item.Comment + ',' + item.InvoiceNumber + ',' + item.DiscrepancyCode + ',');
                        result.AppendLine();
                    }


                    byte[] data = System.Text.Encoding.Unicode.GetBytes(result.ToString());
                    #endregion


                    criteria.Clear();
                    criteria.Add("OrderId", OrderId);
                    criteria.Add("DocumentType", "FIV");
                    Dictionary<long, IList<ExcelDocuments>> DocumentItems = IS.GetExcelDocumentListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);

                    if (DocumentItems != null && DocumentItems.FirstOrDefault().Key > 0)
                    {
                        ExcelDocuments ED = IS.GetExcelDocumentsDetailsById(DocumentItems.FirstOrDefault().Value[0].Id);
                        ED.DocumentData = data;
                        if (!string.IsNullOrEmpty(DocumentFor) && DocumentFor == "FAILLED")
                            ED.DocumentFor = DocumentFor;
                        else
                            ED.DocumentFor = "VERIFIED";
                        ED.ModifiedBy = ED.CreatedBy;
                        ED.ModifiedDate = DateTime.Now;
                        long id = IS.SaveOrUpdateExcelDocuments(ED, "THAMIZH");
                    }
                    else
                    {
                        var items = txtTName.Split('-').ToArray();

                        ExcelDocuments doc = new ExcelDocuments();
                        doc.PeriodYear = items[8];
                        doc.Period = items[6];
                        doc.Sector = items[3];
                        if (!string.IsNullOrEmpty(DocumentFor) && DocumentFor == "FAILLED")
                            doc.DocumentFor = DocumentFor;
                        else
                            doc.DocumentFor = "VERIFIED";

                        doc.Week = Convert.ToInt64(items[7].Replace("WK0", "").ToString());
                        doc.ControlId = txtTName;
                        doc.OrderId = OrderId;
                        doc.DocumentData = data;
                        doc.DocumentType = "FIV";
                        doc.DocumentName = txtTName;
                        long id = IS.SaveOrUpdateExcelDocuments(doc, "THAMIZH");
                    }
                }
            }
            catch (Exception ex)
            {
                #region saveorupdate Errorlog
                OrdersService OrdSer = new OrdersService();
                ErrorLog err = new ErrorLog();
                err.Controller = "Reports";
                err.Action = "CallFIVinCSVParallel";
                err.Err_Desc = ex.ToString();
                err.CreatedDate = DateTime.Now;
                OrdSer.SaveOrUpdateErrorLog(err);
                #endregion
                ExceptionPolicy.HandleException(ex, "InsightReportPolicy");
                throw ex;
            }
        }

        public ActionResult FIVMismatchReport()
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                return View();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightReportPolicy");
                throw;
            }
        }
        public ActionResult GeneratedFIVDocumentListJQGrid(string ExcelState, string DocumentFor, string searchItems, int rows, string sidx, string sord, int? page = 1)
        {
            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
            {
                try
                {
                    bool IsSubsite = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSubsite"]);
                    string SubsiteName = ConfigurationManager.AppSettings["SubsiteName"].ToString();
                    if (string.IsNullOrWhiteSpace(searchItems))
                    {

                        return null;
                    }
                    else
                    {
                        Dictionary<string, object> criteria = new Dictionary<string, object>();
                        if (!string.IsNullOrWhiteSpace(sord) && sord == "desc")
                            sord = "Desc";
                        else
                            sord = "Asc";
                        criteria.Clear();
                        if (searchItems != null && searchItems != "")
                        {
                            var Items = searchItems.ToString().Split(',');
                            if (!string.IsNullOrWhiteSpace(Items[0])) { criteria.Add("Sector", Items[0]); }
                            if (!string.IsNullOrWhiteSpace(Items[4])) { criteria.Add("Period", Items[4]); }
                            if (!string.IsNullOrWhiteSpace(Items[5])) { criteria.Add("Week", Convert.ToInt64(Items[5])); }
                            if (!string.IsNullOrWhiteSpace(Items[6])) { criteria.Add("PeriodYear", Items[6]); }


                            if (!string.IsNullOrWhiteSpace(ExcelState) && ExcelState == "FIV") { criteria.Add("DocumentType", "FIV"); }
                            if (!string.IsNullOrWhiteSpace(DocumentFor)) { criteria.Add("DocumentFor", DocumentFor); }
                            var TrimItems = searchItems.Replace(",", "");
                            if (string.IsNullOrWhiteSpace(TrimItems))
                                return null;
                        }
                        Dictionary<long, IList<ExcelDocumentsView>> DocumentItems = IS.GetExcelDocumentViewListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                        if (DocumentItems != null && DocumentItems.FirstOrDefault().Key > 0)
                        {
                            long totalrecords = DocumentItems.First().Key;
                            int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                            var jsondata = new
                            {
                                total = totalPages,
                                page = page,
                                records = totalrecords,

                                rows = (from items in DocumentItems.First().Value
                                        select new
                                        {
                                            i = 2,
                                            cell = new string[] {
                                            items.Id.ToString(),
                                            items.ControlId,
                                            items.Sector,
                                            items.Name,
                                            items.Period,
                                            items.PeriodYear,
                                            items.Week.ToString(),
                                            items.ModifiedDate.ToString(),
                                            IsSubsite==true?String.Format(@"<a style='color:#034af3;text-decoration:underline' href= '/"+SubsiteName+"/Reports/CSVDownload?Id="+items.Id+"' target='_Blank' >{0}</a>",items.DocumentName):String.Format(@"<a style='color:#034af3;text-decoration:underline' href= '/Reports/CSVDownload?Id="+items.Id+"' target='_Blank' >{0}</a>",items.DocumentName)
                            }
                                        })
                            };
                            return Json(jsondata, JsonRequestBehavior.AllowGet);
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
                    ExceptionPolicy.HandleException(ex, "InsightReportPolicy");
                    throw ex;
                }
            }
        }

        public ActionResult CSVDownload(long Id)
        {
            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
            {
                try
                {
                    ExcelDocuments ed = IS.GetExcelDocumentsDetailsById(Id);
                    //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //Response.AddHeader("content-disposition", "attachment;  filename=" + ed.ControlId + ".xlsx");
                    Response.ContentType = "text/csv";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + ed.ControlId + ".csv");
                    Response.BinaryWrite(ed.DocumentData);
                    Response.End();
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "InsightReportPolicy");
                    throw ex;
                }
            }
        }
        public ActionResult DownloadFIVZipFiles(string searchItems, string DocumentFor, string invType)
        {
            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
            {
                try
                {
                    var Items = searchItems.ToString().Split(',');
                    if (searchItems != null && searchItems != "")
                    {
                        if (!string.IsNullOrWhiteSpace(Items[0])) { criteria.Add("Sector", Items[0]); }
                        if (!string.IsNullOrWhiteSpace(Items[4])) { criteria.Add("Period", Items[4]); }
                        if (!string.IsNullOrWhiteSpace(Items[5])) { criteria.Add("Week", Convert.ToInt64(Items[5])); }
                        if (!string.IsNullOrWhiteSpace(Items[6])) { criteria.Add("PeriodYear", Items[6]); }


                        if (!string.IsNullOrWhiteSpace(invType) && invType == "FIV") { criteria.Add("DocumentType", "FIV"); }
                        if (!string.IsNullOrWhiteSpace(DocumentFor)) { criteria.Add("DocumentFor", DocumentFor); }
                    }
                    IList<ExcelDocuments> DocumentExcelItemsList = null;
                    Dictionary<long, IList<ExcelDocuments>> DocumentItems = IS.GetExcelDocumentListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                    DocumentExcelItemsList = DocumentItems.FirstOrDefault().Value;
                    //Create a Output Memory stream
                    MemoryStream outputMemStream = new MemoryStream();
                    //Create a Output Zipfile stream
                    ZipOutputStream zipStream = new ZipOutputStream(outputMemStream);

                    zipStream.SetLevel(3); //0-9, 9 being the highest level of compression

                    foreach (var item in DocumentExcelItemsList)
                    {
                        //Assigning Byte[] to a new stream 
                        MemoryStream stream = new MemoryStream(item.DocumentData);
                        ZipEntry xmlEntry = new ZipEntry(item.ControlId + ".csv");
                        xmlEntry.DateTime = DateTime.Now;
                        zipStream.PutNextEntry(xmlEntry);
                        StreamUtils.Copy(stream, zipStream, new byte[4096]);
                        zipStream.CloseEntry(); //Close each Zip stream
                    }

                    zipStream.IsStreamOwner = false;
                    zipStream.Close();

                    outputMemStream.Position = 0;

                    byte[] byteArray = outputMemStream.ToArray();

                    Response.Clear();
                    Response.AppendHeader("Content-Disposition", "attachment; filename=" + "FIV-" + Items[0] + "-" + Items[2] + "-" + Items[3] + "-" + Items[4] + ".zip");
                    Response.AppendHeader("Content-Length", byteArray.Length.ToString());
                    Response.ContentType = "application/octet-stream";
                    Response.BinaryWrite(byteArray);

                    SaveDocumentToRecentDownloads(null, null, searchItems, byteArray, invType);

                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "InsightReportPolicy");
                    throw ex;
                }
            }
        }
        #endregion
    }


}







