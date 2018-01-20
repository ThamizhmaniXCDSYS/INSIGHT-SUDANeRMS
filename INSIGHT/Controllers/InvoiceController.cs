using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using INSIGHT.WCFServices;
using INSIGHT.Entities;
using INSIGHT.Entities.InvoiceEntities;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
using INSIGHT.Entities.PDFEntities;
using System.Data;
using System.Threading.Tasks;
using System.Globalization;
using iTextSharp.text;
using System.Collections;
using INSIGHT.Entities.EmailEntities;
using System.Configuration;

namespace INSIGHT.Controllers
{
    public class InvoiceController : ExcelGenerationController
    {
        OrdersService OS = new OrdersService();
        InvoiceService IS = new InvoiceService();
        MastersService MS = new MastersService();
        Dictionary<string, object> criteria = new Dictionary<string, object>();
        IFormatProvider provider = new System.Globalization.CultureInfo("en-CA", true);

        public ActionResult Index()
        {


            return View();
        }
        public ActionResult InvoiceManagement()
        {
            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
            {
                try
                {
                    //Sample for Nhibernate SP 
                    //IEnumerable<Orders> OrderListq = IS.GetOrderListUsingSP("P01","14-15");

                    return View();
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                    throw ex;
                }
            }
        }


        public ActionResult InvoiceListJQGrid(bool InvGeneration, string InvoiceCode, string searchItems, string Contract, string InvoiceDate, string Period, string Name, string Sector, string ContingentType, string Location, string LocationCMR, string CreatedDate, int rows, string sidx, string sord, int? page = 1)
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
                            if (!string.IsNullOrWhiteSpace(Items[1])) { criteria.Add("Name", Items[1]); }
                            if (!string.IsNullOrWhiteSpace(Items[2])) { criteria.Add("ContingentType", Items[2]); }
                            //if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Location", Items[3]); }
                            if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Period", Items[3]); }
                            if (!string.IsNullOrWhiteSpace(Items[4])) { criteria.Add("PeriodYear", Items[4]); }
                            var TrimItems = searchItems.Replace(",", "");
                            if (string.IsNullOrWhiteSpace(TrimItems))
                                return null;
                        }
                        if (!string.IsNullOrWhiteSpace(InvoiceCode)) { criteria.Add("InvoiceCode", InvoiceCode); }
                        if (!string.IsNullOrWhiteSpace(Contract)) { criteria.Add("Contract", Contract); }
                        if (!string.IsNullOrWhiteSpace(InvoiceDate)) { criteria.Add("InvoiceDate", Convert.ToDateTime(InvoiceDate)); }
                        if (!string.IsNullOrWhiteSpace(Name)) { criteria.Add("Name", Name); }
                        if (!string.IsNullOrWhiteSpace(ContingentType)) { criteria.Add("ContingentType", ContingentType); }
                        if (!string.IsNullOrWhiteSpace(Location)) { criteria.Add("Location", Location); }
                        if (!string.IsNullOrWhiteSpace(Period)) { criteria.Add("Period", Period); }
                        if (!string.IsNullOrWhiteSpace(Sector)) { criteria.Add("Sector", Sector); }
                        if (!string.IsNullOrWhiteSpace(CreatedDate)) { criteria.Add("CreatedDate", Convert.ToDateTime(CreatedDate)); }
                        Dictionary<long, IList<InvoiceManagementView>> invoiceItems = IS.GetInvoiceListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                        if (invoiceItems != null && invoiceItems.Count > 0 && InvGeneration == false)
                        {
                            long totalrecords = invoiceItems.First().Key;
                            int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                            var jsondata = new
                            {
                                total = totalPages,
                                page = page,
                                records = totalrecords,

                                rows = (from items in invoiceItems.First().Value
                                        select new
                                        {
                                            i = 2,
                                            cell = new string[] {
                                            items.Id.ToString(),
                                           IsSubsite==true?String.Format(@"<a style='color:#034af3;text-decoration:underline' href= '/"+SubsiteName+"/Invoice/NewInvoice?OrderId="+items.OrderId+"' >{0}</a>",items.InvoiceCode):String.Format(@"<a style='color:#034af3;text-decoration:underline' href= '/Invoice/NewInvoice?OrderId="+items.OrderId+"' >{0}</a>",items.InvoiceCode),
                                           //String.Format(@"<a style='color:#034af3;text-decoration:underline' href= '/Invoice/NewInvoice?OrderId="+items.OrderId+"' >{0}</a>",items.InvoiceCode),
                                            items.Contract,
                                            items.InvoiceDate!=null? ConvertDateTimeToDate(items.InvoiceDate.ToString("dd/MM/yyyy"),"en-GB"):"",
                                            items.Period,
                                            items.Sector,
                                            items.Name,
                                            items.ContingentType,
                                            items.Location,
                                            items.LocationCMR.ToString(),
                                            items.CreatedDate!=null? ConvertDateTimeToDate(items.CreatedDate.ToString("dd/MM/yyyy"),"en-GB"):"",
                                            items.TotalFeedTroopStrength.ToString(),
                                            items.TotalMadays.ToString(),
                                            items.PeriodYear,
                                            IsSubsite==true?String.Format(@"<a style='color:#034af3;text-decoration:underline' href= '/"+SubsiteName+"/PdfGeneration/InvoiceSinglePrint?OrderId="+items.OrderId+"' target='_Blank' >{0}</a>","PDF"):String.Format(@"<a style='color:#034af3;text-decoration:underline' href= '/PdfGeneration/InvoiceSinglePrint?OrderId="+items.OrderId+"' target='_Blank' >{0}</a>","PDF"),
                                            IsSubsite==true?String.Format(@"<a style='color:#034af3;text-decoration:underline' href= '/"+SubsiteName+"/ExcelGeneration/InvoiceSingleExcel?OrderId="+items.OrderId+"&GenerateInv="+true+"' target='_Blank' >{0}</a>","Excel"):String.Format(@"<a style='color:#034af3;text-decoration:underline' href= '/ExcelGeneration/InvoiceSingleExcel?OrderId="+items.OrderId+"&GenerateInv="+true+"' target='_Blank' >{0}</a>","Excel")
                                            //String.Format(@"<a style='color:#034af3;text-decoration:underline' href= '/PdfGeneration/InvoiceSinglePrint?OrderId="+items.OrderId+"' target='_Blank' >{0}</a>","PDF"),
                                            //String.Format(@"<a style='color:#034af3;text-decoration:underline' href= '/ExcelGeneration/InvoiceSingleExcel?OrderId="+items.OrderId+"&GenerateInv="+true+"' target='_Blank' >{0}</a>","Excel")
                                            }
                                        })
                            };
                            return Json(jsondata, JsonRequestBehavior.AllowGet);
                        }
                        else if (invoiceItems != null && invoiceItems.Count > 0 && InvGeneration == true)
                        {
                            long totalrecords = invoiceItems.First().Key;
                            int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                            var jsondata = new
                            {
                                total = totalPages,
                                page = page,
                                records = totalrecords,

                                rows = (from items in invoiceItems.First().Value
                                        select new
                                        {
                                            i = 2,
                                            cell = new string[] {
                                            items.Id.ToString(),
                                            IsSubsite==true?String.Format(@"<a style='color:#034af3;text-decoration:underline' href= '/"+SubsiteName+"/Invoice/NewInvoice?OrderId="+items.OrderId+"' >{0}</a>",items.InvoiceCode):String.Format(@"<a style='color:#034af3;text-decoration:underline' href= '/Invoice/NewInvoice?OrderId="+items.OrderId+"' >{0}</a>",items.InvoiceCode),
                                            //String.Format(@"<a style='color:#034af3;text-decoration:underline' href= '/Invoice/NewInvoice?OrderId="+items.OrderId+"' >{0}</a>",items.InvoiceCode),
                                            items.Contract,
                                            items.InvoiceDate!=null? ConvertDateTimeToDate(items.InvoiceDate.ToString("dd/MM/yyyy"),"en-GB"):"",
                                            items.Period,
                                            items.Sector,
                                            items.Name,
                                            items.ContingentType,
                                            items.Location,
                                            items.LocationCMR.ToString(),
                                            items.CreatedDate!=null? ConvertDateTimeToDate(items.CreatedDate.ToString("dd/MM/yyyy"),"en-GB"):"",
                                            items.TotalFeedTroopStrength.ToString(),
                                            items.TotalMadays.ToString(),
                                            items.PeriodYear,
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
                    ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                    throw ex;
                }
            }
        }

        public ActionResult NewInvoice(long OrderId)
        {
            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
            {
                try
                {
                    Invoice Invoice = IS.GetInvoiceDetailsByOrderId(OrderId);
                    Orders Od = OS.GetOrdersById(OrderId);
                    if (Invoice != null)
                    {
                        ViewBag.InvoiceCode = Invoice.InvoiceCode;
                        //Od.StartDate = ConvertDateTimeToDate(Od.StartDate.ToString, "en-GB");
                        return View(Od);
                    }
                    else
                    {
                        Invoice I = new Invoice();
                        I.OrderId = OrderId;
                        I.InvoiceCode = "INV-" + Od.Name + "-" + Od.Period + "-WK" + Od.Week;
                        I.Period = Od.Period;
                        I.Week = (int)Od.Week;
                        I.Sector = Od.Sector;
                        I.TotalFeedTroopStrength = (long)Od.Troops;
                        I.TotalMadays = (Od.Troops * 7);
                        I.InvoiceDate = DateTime.Now;
                        I.ModifiedDate = DateTime.Now;
                        I.CreatedDate = DateTime.Now;
                        I.PeriodYear = Od.PeriodYear;
                        I.IsActive = false;
                        IS.SaveOrUpdateInvoice(I, userId);

                        ViewBag.InvoiceCode = I.InvoiceCode;
                        Od.InvoiceId = I.Id;
                        OS.SaveOrUpdateOrder(Od);
                        return View(Od);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                    throw ex;
                }
            }
        }
        [HttpPost]
        public ActionResult NewInvoice(Invoice Iv)
        {
            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
            {
                try
                {
                    Iv.InvoiceDate = DateTime.Now;
                    Iv.ModifiedDate = DateTime.Now;
                    Iv.CreatedDate = DateTime.Now;
                    IS.SaveOrUpdateInvoice(Iv, userId);
                    return View(Iv);
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                    throw ex;
                }
            }
        }

        public ActionResult OrderListPV(string SearchItems, long InvoiceId)
        {
            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
            {
                try
                {
                    ViewBag.SearchItems = SearchItems;
                    ViewBag.InvoiceId = InvoiceId;
                    return View();
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                    throw ex;
                }
            }

        }

        public ActionResult AddOrderList(long InvoiceId, string OrderIds)
        {
            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
            {
                try
                {
                    long[] OrderId = OrderIds.Split(',').Select(n => Convert.ToInt64(n)).ToArray();
                    IS.SaveOrUpdateInvoiceOrders(InvoiceId, OrderId);
                    return Json(true);
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                    throw ex;
                }
            }
        }

        public ActionResult InvoiceOrdersListJQGrid(long? InvoiceId, long? OrdersId, int rows, string sidx, string sord, int? page = 1)
        {
            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
            {
                try
                {
                    criteria.Clear();
                    if (!string.IsNullOrWhiteSpace(sord) && sord == "desc")
                        sord = "Desc";
                    else
                        sord = "Asc";
                    if (InvoiceId != 0)
                    {
                        if (OrdersId != 0)
                        {
                            //long[] OrderIds = OrdersId.Split(',').Select(n => Convert.ToInt64(n)).ToArray();
                            criteria.Add("OrderId", OrdersId);
                        }
                        criteria.Add("InvoiceId", InvoiceId);
                        Dictionary<long, IList<InvoiceOrdersView>> invordItems = IS.GetInvoiceOrdersViewListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                        if (invordItems != null && invordItems.Count > 0)
                        {
                            long totalrecords = invordItems.First().Key;
                            int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                            var jsondat = new
                            {
                                total = totalPages,
                                page = page,
                                records = totalrecords,

                                rows = (from items in invordItems.First().Value
                                        select new
                                        {
                                            i = 2,
                                            cell = new string[] {
                                        items.Id.ToString(),
                                        items.InvoiceId.ToString(),
                                        items.OrderId.ToString(),
                                        items.ControlId.ToString(),
                                        items.Name.ToString(),
                                        items.Location.ToString(),
                                        items.StartDate!=null? ConvertDateTimeToDate(items.StartDate.ToString("dd/MM/yyyy"),"en-GB"):"",
                                        items.EndDate!=null? ConvertDateTimeToDate(items.EndDate.ToString("dd/MM/yyyy"),"en-GB"):"",
                                        items.Troops.ToString(),
                                        items.TotalAmount.ToString(),
                                        items.LineItemsOrdered.ToString(),
                                        items.KgOrderedWOEggs.ToString(),
                                        items.EggsWeight.ToString(),
                                        items.TotalWeight.ToString(),
                                        items.LocationCMR.ToString(),
                                        items.ControlCMR.ToString()
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
                    }
                    else
                    {
                        var jsondat = new { rows = (new { cell = new string[] { } }) };
                        return Json(jsondat, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                    throw ex;
                }
            }
        }

        public ActionResult DeliveriesPerOrdQtyListJQGrid(string UNCode, string Commodity, string SectorPrice, string OrderedQty, string AcceptedQty, string DeliveredQty, string DeliveredDate, string Total, string RemainingOrdQty, string Difference, string DifferencePercent, long? OrdersId, int rows, string sidx, string sord, int? page = 1)
        {
            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
            {
                try
                {
                    sord = sord == "desc" ? "Desc" : "Asc";
                    if (OrdersId != 0)
                    {
                        criteria.Clear();
                        //long[] OrderIds = OrdersId.Split(',').Select(n => Convert.ToInt64(n)).ToArray();
                        criteria.Add("OrderId", OrdersId);
                        if (!string.IsNullOrWhiteSpace(UNCode)) { criteria.Add("UNCode", Convert.ToInt64(UNCode)); }
                        if (!string.IsNullOrWhiteSpace(Commodity)) { criteria.Add("Commodity", Commodity); }
                        if (!string.IsNullOrWhiteSpace(SectorPrice)) { criteria.Add("SectorPrice", Convert.ToDecimal(SectorPrice)); }
                        if (!string.IsNullOrWhiteSpace(OrderedQty)) { criteria.Add("OrderedQty", Convert.ToDecimal(OrderedQty)); }
                        if (!string.IsNullOrWhiteSpace(AcceptedQty)) { criteria.Add("AcceptedQty", Convert.ToDecimal(AcceptedQty)); }
                        if (!string.IsNullOrWhiteSpace(DeliveredQty)) { criteria.Add("DeliveredQty", Convert.ToDecimal(DeliveredQty)); }
                        if (!string.IsNullOrWhiteSpace(DeliveredDate)) { criteria.Add("DeliveredDate", Convert.ToDateTime(DeliveredDate)); }
                        if (!string.IsNullOrWhiteSpace(Total)) { criteria.Add("Total", Convert.ToDecimal(Total)); }
                        if (!string.IsNullOrWhiteSpace(RemainingOrdQty)) { criteria.Add("RemainingOrdQty", Convert.ToDecimal(RemainingOrdQty)); }
                        if (!string.IsNullOrWhiteSpace(Difference)) { criteria.Add("Difference", Convert.ToDecimal(Difference)); }
                        if (!string.IsNullOrWhiteSpace(DifferencePercent)) { criteria.Add("DifferencePercent", Convert.ToDecimal(DifferencePercent)); }
                        Dictionary<long, IList<DeliveriesPerOrdQty>> deliveryQty = IS.GetDeliveriesPerOrdQtyListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                        if (deliveryQty != null && deliveryQty.Count > 0)
                        {
                            long totalrecords = deliveryQty.First().Key;
                            int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                            var jsondata = new
                            {
                                total = totalPages,
                                page = page,
                                records = totalrecords,

                                rows = (from items in deliveryQty.First().Value
                                        select new
                                        {
                                            i = 2,
                                            cell = new string[] {
                                        items.Id.ToString(),
                                        items.PODId.ToString(),
                                        items.PODItemsId.ToString(),
                                        items.OrderId.ToString(),
                                        items.Period.ToString(),
                                        items.Sector,
                                        items.Week.ToString(),
                                        items.LineId.ToString(),
                                        items.UNCode.ToString(),
                                        items.Commodity,
                                        items.SectorPrice.ToString(),
                                        items.OrderedQty.ToString(),
                                        items.AcceptedQty.ToString(),
                                        items.DeliveredQty.ToString(),
                                        items.DeliveredDate!=null? ConvertDateTimeToDate(items.DeliveredDate.ToString("dd/MM/yyyy"),"en-GB"):"",
                                        items.CreatedBy,
                                        items.Total.ToString(),
                                        items.RemainingOrdQty.ToString(),
                                        items.Difference.ToString(),
                                        Math.Round(items.DifferencePercent, 2).ToString(),
                            }
                                        })
                            };
                            return Json(jsondata, JsonRequestBehavior.AllowGet);
                        }
                        return null;
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                    throw ex;
                }
            }
        }

        public ActionResult DeliveryExceedListJQGrid(string UNCode, string Commodity, string SectorPrice, string OrderedQty, string AcceptedQty, string DeliveredQty, string DeliveredDate, string Total, string RemainingOrdQty, string Difference, string DifferencePercent, long? OrdersId, int rows, string sidx, string sord, int? page = 1)
        {
            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
            {
                try
                {


                    sord = sord == "desc" ? "Desc" : "Asc";
                    if (OrdersId != 0)
                    {
                        criteria.Clear();
                        //long[] OrderIds = OrdersId.Split(',').Select(n => Convert.ToInt64(n)).ToArray();
                        criteria.Add("OrderId", OrdersId);
                        if (!string.IsNullOrWhiteSpace(UNCode)) { criteria.Add("UNCode", Convert.ToInt64(UNCode)); }
                        if (!string.IsNullOrWhiteSpace(Commodity)) { criteria.Add("Commodity", Commodity); }
                        if (!string.IsNullOrWhiteSpace(SectorPrice)) { criteria.Add("SectorPrice", Convert.ToDecimal(SectorPrice)); }
                        if (!string.IsNullOrWhiteSpace(OrderedQty)) { criteria.Add("OrderedQty", Convert.ToDecimal(OrderedQty)); }
                        if (!string.IsNullOrWhiteSpace(AcceptedQty)) { criteria.Add("AcceptedQty", Convert.ToDecimal(AcceptedQty)); }
                        if (!string.IsNullOrWhiteSpace(DeliveredQty)) { criteria.Add("DeliveredQty", Convert.ToDecimal(DeliveredQty)); }
                        if (!string.IsNullOrWhiteSpace(DeliveredDate)) { criteria.Add("DeliveredDate", Convert.ToDateTime(DeliveredDate)); }
                        if (!string.IsNullOrWhiteSpace(Total)) { criteria.Add("Total", Convert.ToDecimal(Total)); }
                        if (!string.IsNullOrWhiteSpace(RemainingOrdQty)) { criteria.Add("RemainingOrdQty", Convert.ToDecimal(RemainingOrdQty)); }
                        if (!string.IsNullOrWhiteSpace(Difference)) { criteria.Add("Difference", Convert.ToDecimal(Difference)); }
                        if (!string.IsNullOrWhiteSpace(DifferencePercent)) { criteria.Add("DifferencePercent", Convert.ToDecimal(DifferencePercent)); }
                        Dictionary<long, IList<DeliveryExceed>> deliveryExceed = IS.GetDeliveryExceedListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                        if (deliveryExceed != null && deliveryExceed.Count > 0)
                        {
                            long totalrecords = deliveryExceed.First().Key;
                            int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                            var jsondata = new
                            {
                                total = totalPages,
                                page = page,
                                records = totalrecords,

                                rows = (from items in deliveryExceed.First().Value
                                        select new
                                        {
                                            i = 2,
                                            cell = new string[] {
                                        items.Id.ToString(),
                                        items.PODId.ToString(),
                                        //items.PODItemsId.ToString(),
                                        items.OrderId.ToString(),
                                        items.Period.ToString(),
                                        items.Sector,
                                        items.Week.ToString(),
                                        items.LineId.ToString(),
                                        items.UNCode.ToString(),
                                        items.Commodity,
                                        items.SectorPrice.ToString(),
                                        items.OrderedQty.ToString(),
                                        items.AcceptedQty.ToString(),
                                        items.DeliveredQty.ToString(),
                                        items.DeliveredDate!=null? ConvertDateTimeToDate(items.DeliveredDate.ToString("dd/MM/yyyy"),"en-GB"):"",
                                        items.CreatedBy,
                                        items.Total.ToString(),
                                        items.RemainingOrdQty.ToString(),
                                        items.Difference.ToString(),
                                        Math.Round(items.DifferencePercent, 2).ToString(),
                                        
                            }
                                        })
                            };
                            return Json(jsondata, JsonRequestBehavior.AllowGet);
                        }
                        return null;
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                    throw ex;
                }
            }
        }
        public ActionResult DeliveryWithoutOrderListJQGrid(string UNCode, string Commodity, string SectorPrice, string OrderedQty, string AcceptedQty, string DeliveredQty, string DeliveredDate, string Total, string RemainingOrdQty, string Difference, string DifferencePercent, long? OrdersId, int rows, string sidx, string sord, int? page = 1)
        {
            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
            {
                try
                {
                    sord = sord == "desc" ? "Desc" : "Asc";
                    if (OrdersId != 0)
                    {
                        criteria.Clear();
                        //long[] OrderIds = OrdersId.Split(',').Select(n => Convert.ToInt64(n)).ToArray();
                        criteria.Add("OrderId", OrdersId);
                        if (!string.IsNullOrWhiteSpace(UNCode)) { criteria.Add("UNCode", Convert.ToInt64(UNCode)); }
                        if (!string.IsNullOrWhiteSpace(Commodity)) { criteria.Add("Commodity", Commodity); }
                        if (!string.IsNullOrWhiteSpace(SectorPrice)) { criteria.Add("SectorPrice", Convert.ToDecimal(SectorPrice)); }
                        if (!string.IsNullOrWhiteSpace(OrderedQty)) { criteria.Add("OrderedQty", Convert.ToDecimal(OrderedQty)); }
                        if (!string.IsNullOrWhiteSpace(AcceptedQty)) { criteria.Add("AcceptedQty", Convert.ToDecimal(AcceptedQty)); }
                        if (!string.IsNullOrWhiteSpace(DeliveredQty)) { criteria.Add("DeliveredQty", Convert.ToDecimal(DeliveredQty)); }
                        if (!string.IsNullOrWhiteSpace(DeliveredDate)) { criteria.Add("DeliveredDate", Convert.ToDateTime(DeliveredDate)); }
                        if (!string.IsNullOrWhiteSpace(Total)) { criteria.Add("Total", Convert.ToDecimal(Total)); }
                        if (!string.IsNullOrWhiteSpace(RemainingOrdQty)) { criteria.Add("RemainingOrdQty", Convert.ToDecimal(RemainingOrdQty)); }
                        if (!string.IsNullOrWhiteSpace(Difference)) { criteria.Add("Difference", Convert.ToDecimal(Difference)); }
                        if (!string.IsNullOrWhiteSpace(DifferencePercent)) { criteria.Add("DifferencePercent", Convert.ToDecimal(DifferencePercent)); }
                        Dictionary<long, IList<DeliveryWithoutOrders>> deliveryWithoutOrders = IS.GetDeliveryWithoutOrdersList(0, 9999, string.Empty, string.Empty, criteria);
                        if (deliveryWithoutOrders != null && deliveryWithoutOrders.Count > 0)
                        {
                            long totalrecords = deliveryWithoutOrders.First().Key;
                            int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                            var jsondata = new
                            {
                                total = totalPages,
                                page = page,
                                records = totalrecords,

                                rows = (from items in deliveryWithoutOrders.First().Value
                                        select new
                                        {
                                            i = 2,
                                            cell = new string[] {
                                        items.Id.ToString(),
                                        items.PODId.ToString(),
                                        //items.PODItemsId.ToString(),
                                        items.OrderId.ToString(),
                                        items.Period.ToString(),
                                        items.Sector,
                                        items.Week.ToString(),
                                        items.LineId.ToString(),
                                        items.UNCode.ToString(),
                                        items.Commodity,
                                        items.SectorPrice.ToString(),
                                        items.OrderedQty.ToString(),
                                        items.AcceptedQty.ToString(),
                                        items.DeliveredQty.ToString(),
                                        items.DeliveredDate!=null? ConvertDateTimeToDate(items.DeliveredDate.ToString("dd/MM/yyyy"),"en-GB"):"",
                                        items.CreatedBy,
                                        items.Total.ToString(),
                                        items.RemainingOrdQty.ToString(),
                                        items.Difference.ToString(),
                                        Math.Round(items.DifferencePercent, 2).ToString(),
                                        
                            }
                                        })
                            };
                            return Json(jsondata, JsonRequestBehavior.AllowGet);
                        }
                        return null;
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                    throw ex;
                }
            }
        }

        public ActionResult PeriodListCheck(string Period, string searchItems)
        {
            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
            {
                try
                {
                    criteria.Clear();
                    var Items = searchItems.ToString().Split(',');
                    if (searchItems != null && searchItems != "")
                    {
                        if (!string.IsNullOrWhiteSpace(Items[0])) { criteria.Add("Sector", Items[0]); }
                        if (!string.IsNullOrWhiteSpace(Items[1])) { criteria.Add("Name", Items[1]); }
                        if (!string.IsNullOrWhiteSpace(Items[2])) { criteria.Add("ContingentType", Items[2]); }
                        //if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Location", Items[3]); }
                        if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Period", Items[3]); }
                        if (!string.IsNullOrWhiteSpace(Items[4])) { criteria.Add("PeriodYear", Items[4]); }
                        var TrimItems = searchItems.Replace(",", "");
                        if (string.IsNullOrWhiteSpace(TrimItems))
                            return null;
                    }
                    Dictionary<long, IList<InvoiceManagementView>> invoiceItems = IS.GetInvoiceListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                    criteria.Clear();
                    if (!string.IsNullOrWhiteSpace(Period)) { criteria.Add("Period", Period); criteria.Add("Year", Items[4]); }
                    Dictionary<long, IList<PeriodMaster>> PeriodMasterList = MS.GetPeriodMasterListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);

                    var periodWeek = (from items in PeriodMasterList.First().Value
                                      select items.Week).ToList();
                    var invoiceWeek = (from items in invoiceItems.First().Value
                                       select items.Week).Distinct().ToList();

                    //int[] list1 = periodWeek.ToArray();
                    //int[] List2 = invoiceWeek.ToArray();

                    string[] list1 = periodWeek.Select(i => i.ToString()).ToArray();
                    string[] list2 = invoiceWeek.Select(i => i.ToString()).ToArray();

                    //Comparing To List
                    var T = list1.Intersect(list2);
                    var T1 = list1.Except(list2);

                    string str = string.Empty;
                    foreach (var item in T1)
                        str = str + item + ",";
                    if (str != string.Empty)
                        str = str.Remove(str.Length - 1);

                    var count = list1.Count() - list2.Count();

                    if (count == 0)
                        return Json(true, JsonRequestBehavior.AllowGet);
                    else
                        return Json(str, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                    throw ex;
                }
            }
        }

        public ActionResult GenerateInvoice()
        {
            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
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
        }
        [HttpPost]
        public ActionResult GenerateInvoice(string searchItems, DateTime invoiceDt)
        {
            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
            {
                try
                {
                    criteria.Clear();
                    var Items = searchItems.ToString().Split(',');
                    if (searchItems != null && searchItems != "")
                    {
                        if (!string.IsNullOrWhiteSpace(Items[0])) { criteria.Add("Sector", Items[0]); }
                        if (!string.IsNullOrWhiteSpace(Items[1])) { criteria.Add("ContingentType", Items[1]); }
                        if (!string.IsNullOrWhiteSpace(Items[2])) { criteria.Add("Name", Items[2]); }
                        if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Period", Items[3]); }
                        var TrimItems = searchItems.Replace(",", "");
                        if (string.IsNullOrWhiteSpace(TrimItems))
                            return null;
                    }
                    Dictionary<long, IList<Orders>> orderList = OS.GetOrdersListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                    Dictionary<long, IList<InvoiceManagementView>> invoiceItems = IS.GetInvoiceListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                    var OrderIds = (from items in orderList.First().Value
                                    select items.OrderId).OrderBy(i => i).Distinct().ToList();
                    var invoiceOrderIds = (from items in invoiceItems.First().Value
                                           select items.OrderId).OrderBy(i => i).Distinct().ToList();

                    int successCount = 0;
                    int failureCount = 0;
                    foreach (var item in orderList.First().Value)
                    {
                        if (item.OrderId != invoiceOrderIds.Find(i => i == item.OrderId))
                        {
                            Invoice I = new Invoice();
                            I.OrderId = item.OrderId;
                            I.InvoiceCode = "INV-" + item.Name + "-" + item.Period + "-WK" + item.Week;
                            I.Period = item.Period;
                            I.Week = (int)item.Week;
                            I.Sector = item.Sector;
                            I.TotalFeedTroopStrength = (long)item.Troops;
                            I.TotalMadays = (item.Troops * 7);
                            I.InvoiceDate = invoiceDt;
                            I.ModifiedDate = DateTime.Now;
                            I.CreatedDate = DateTime.Now;
                            IS.SaveOrUpdateInvoice(I, userId);
                            successCount = successCount + 1;
                        }
                        else
                        {
                            failureCount = failureCount + 1;
                        }
                    }
                    return View();
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                    throw ex;
                }
            }
        }
        public JsonResult GenerateInvoicePopup(string searchItems, DateTime invoiceDt)
        {
            string userId = base.ValidateUser();
            try
            {
                criteria.Clear();
                var Items = searchItems.ToString().Split(',');
                if (searchItems != null && searchItems != "")
                {
                    if (!string.IsNullOrWhiteSpace(Items[0])) { criteria.Add("Sector", Items[0]); }
                    if (!string.IsNullOrWhiteSpace(Items[1])) { criteria.Add("ContingentType", Items[1]); }
                    if (!string.IsNullOrWhiteSpace(Items[2])) { criteria.Add("Name", Items[2]); }
                    if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Period", Items[3]); }
                    if (!string.IsNullOrWhiteSpace(Items[4])) { criteria.Add("PeriodYear", Items[4]); }
                    if (!string.IsNullOrWhiteSpace(Items[5])) { criteria.Add("Week", Convert.ToInt64(Items[5])); }
                    var TrimItems = searchItems.Replace(",", "");
                    if (string.IsNullOrWhiteSpace(TrimItems))
                        return null;
                }
                Dictionary<long, IList<Orders>> orderList = OS.GetOrdersListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                Dictionary<long, IList<InvoiceManagementView>> invoiceItems = IS.GetInvoiceListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                var OrderIds = (from items in orderList.First().Value
                                select items.OrderId).OrderBy(i => i).Distinct().ToList();
                var invoiceOrderIds = (from items in invoiceItems.First().Value
                                       select items.OrderId).OrderBy(i => i).Distinct().ToList();

                int successCount = 0;
                int failureCount = 0;
                foreach (var item in orderList.First().Value)
                {
                    if (item.OrderId != invoiceOrderIds.Find(i => i == item.OrderId))
                    {
                        Invoice I = new Invoice();
                        I.OrderId = item.OrderId;
                        I.InvoiceCode = "INV-" + item.Name + "-" + item.Period + "-WK" + item.Week;
                        I.Period = item.Period;
                        I.Week = (int)item.Week;
                        I.Sector = item.Sector;
                        I.TotalFeedTroopStrength = (long)item.Troops;
                        I.TotalMadays = (item.Troops * 7);
                        I.InvoiceDate = invoiceDt;
                        I.ModifiedDate = DateTime.Now;
                        I.CreatedDate = DateTime.Now;
                        I.PeriodYear = item.PeriodYear;
                        I.IsActive = false;
                        IS.SaveOrUpdateInvoice(I, userId);

                        Orders Od = OS.GetOrdersById(item.OrderId);
                        Od.InvoiceId = I.Id;
                        OS.SaveOrUpdateOrder(Od);

                        successCount = successCount + 1;
                    }
                    else
                    {
                        failureCount = failureCount + 1;
                    }
                }
                string str = successCount + "/" + failureCount;
                return Json(str, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }


        public ActionResult SampleTest()
        {
            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
            {
                try
                {
                    long OrderId = 3019;
                    IS.SaveWithNewSessionMethods(OrderId);
                    return null;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        /// <summary>
        /// To Download Zip for both Excel and PDF
        /// The Buttons are Provided in Each Pager of PDF and EXCEL Documents Grid
        /// </summary>
        /// <param name="searchItems"></param>
        /// <param name="PDF"></param>
        /// <param name="EXCEL"></param>
        /// <returns>zip files of PDF and Excel</returns>
        public ActionResult DownloadZipFiles(string searchItems, bool PDF, bool EXCEL, string invType)
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
                        var TrimItems = searchItems.Replace(",", "");
                        if (string.IsNullOrWhiteSpace(TrimItems))
                            return null;
                        if (!string.IsNullOrWhiteSpace(Items[0])) { criteria.Add("Sector", Items[0]); }
                        if (!string.IsNullOrWhiteSpace(Items[1])) { criteria.Add("Name", Items[1]); }
                        if (!string.IsNullOrWhiteSpace(Items[2])) { criteria.Add("ContingentType", Items[2]); }
                        if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Period", Items[3]); }
                        if (!string.IsNullOrWhiteSpace(Items[4])) { criteria.Add("PeriodYear", Items[4]); }
                        if (Items.Length > 5)
                            if (!string.IsNullOrWhiteSpace(Items[5])) { criteria.Add("Week", Convert.ToInt64(Items[5])); }

                        if (!string.IsNullOrWhiteSpace(invType) && invType == "Excel-Single") { criteria.Add("DocumentType", "Excel-Single"); }
                        if (!string.IsNullOrWhiteSpace(invType) && invType == "Excel-Consol")
                        {
                            Items[0] = "Consol";
                            Items[2] = "Excel";
                            criteria.Add("DocumentType", "Excel-Consol");

                        }
                        if (!string.IsNullOrWhiteSpace(invType) && invType == "PDF-Single") { criteria.Add("DocumentType", "PDF-Single"); }
                        if (!string.IsNullOrWhiteSpace(invType) && invType == "PDF-Consol")
                        {
                            Items[0] = "Consol";
                            Items[2] = "PDF";
                            criteria.Add("DocumentType", "PDF-Consol");

                        }
                        if (!string.IsNullOrWhiteSpace(invType) && invType == "Transpt-Book")
                        {
                            Items[0] = "TPT";
                            Items[2] = "Invoices";
                            criteria.Add("DocumentType", "Transpt-Book");
                        }
                    }
                    IList<PDFDocuments> DocumentPDFItemsList = null;
                    IList<ExcelDocuments> DocumentExcelItemsList = null;
                    if (PDF == true) // For PDF ZIP Files
                    {
                        Dictionary<long, IList<PDFDocuments>> DocumentItems = IS.GetPDFDocumentListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                        DocumentPDFItemsList = DocumentItems.FirstOrDefault().Value;
                    }
                    else // For Excel ZIP Files
                    {
                        Dictionary<long, IList<ExcelDocuments>> DocumentItems = IS.GetExcelDocumentListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                        DocumentExcelItemsList = DocumentItems.FirstOrDefault().Value;
                    }
                    #region code to create folder and zip files

                    //string curpath = "D:\\HDMSInbox";
                    //string mailbox = String.Format("{0}\\" +"Inbox", curpath);
                    //// If the folder is not existed, create it.
                    //if (!Directory.Exists(mailbox)) { Directory.CreateDirectory(mailbox); }

                    //foreach (var item in DocumentItemsList)
                    //{
                    //    System.IO.FileStream file = System.IO.File.Create(mailbox + "\\" + item.ControlId + ".pdf ");
                    //    file.Write(item.DocumentData, 0, item.DocumentData.Length);
                    //    file.Close();
                    //}
                    #endregion

                    //Create a Output Memory stream
                    MemoryStream outputMemStream = new MemoryStream();
                    //Create a Output Zipfile stream
                    ZipOutputStream zipStream = new ZipOutputStream(outputMemStream);

                    zipStream.SetLevel(3); //0-9, 9 being the highest level of compression

                    if (EXCEL == true)
                    {   //For Excel files
                        foreach (var item in DocumentExcelItemsList)
                        {
                            //Assigning Byte[] to a new stream 
                            MemoryStream stream = new MemoryStream(item.DocumentData);
                            ZipEntry xmlEntry = new ZipEntry(item.ControlId + ".xlsx");
                            xmlEntry.DateTime = DateTime.Now;
                            zipStream.PutNextEntry(xmlEntry);
                            StreamUtils.Copy(stream, zipStream, new byte[4096]);
                            zipStream.CloseEntry(); //Close each Zip stream
                        }
                    }
                    else
                    {          //For PDF files
                        foreach (var item in DocumentPDFItemsList)
                        {
                            //Assigning Byte[] to a new stream 
                            MemoryStream stream = new MemoryStream(item.DocumentData);
                            ZipEntry xmlEntry = new ZipEntry(item.ControlId + ".pdf");
                            xmlEntry.DateTime = DateTime.Now;
                            zipStream.PutNextEntry(xmlEntry);
                            StreamUtils.Copy(stream, zipStream, new byte[4096]);
                            zipStream.CloseEntry(); //Close each Zip stream
                        }
                    }

                    zipStream.IsStreamOwner = false;
                    zipStream.Close();

                    outputMemStream.Position = 0;

                    byte[] byteArray = outputMemStream.ToArray();

                    Response.Clear();
                    Response.AppendHeader("Content-Disposition", "attachment; filename=" + "GCC-" + Items[0] + "-" + Items[2] + "-" + Items[3] + "-" + Items[4] + ".zip");
                    Response.AppendHeader("Content-Length", byteArray.Length.ToString());
                    Response.ContentType = "application/octet-stream";
                    Response.BinaryWrite(byteArray);

                    SaveDocumentToRecentDownloads(null, null, searchItems, byteArray, invType);

                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Generate Invoice PDF Single Document Parallel
        /// </summary>
        public void GenerateInvoicePdfParallel()
        {
            try
            {
                //getting all the Info from Invoice Created and Document saved in Both PDF and Excel
                Dictionary<long, IList<InvoiceManagementView>> invoiceItems = IS.GetInvoiceListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                Dictionary<long, IList<PDFDocuments>> PDFDocumentItems = IS.GetPDFDocumentListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);

                //getting order list from invoiceItems
                var InvoiceOrderList = (from items in invoiceItems.First().Value
                                        select items.OrderId).OrderBy(i => i).Distinct().ToArray();

                //getting order list from PDFDocumentItems
                var PdfOrderList = (from items in PDFDocumentItems.First().Value
                                    where items.OrderId != 0
                                    select items.OrderId).OrderBy(i => i).Distinct().ToArray();


                //For comparing Long arrays
                long[] INVOrdList = InvoiceOrderList.Select(i => i).ToArray();
                long[] PdfOrdList = PdfOrderList.Select(i => i).ToArray();

                //Taking orderid's of Pdf and Excel not in documents table
                var T1 = INVOrdList.Except(PdfOrdList);

                //long []
                long[] NewPdfOrderId = T1.Select(i => i).Distinct().ToArray();


                foreach (var item in NewPdfOrderId)
                {
                    CallInvoicePdfParallel(item);
                    //RedirectToAction("InvoiceSinglePrint", "PdfGeneration", new { OrderId = item });
                    //@Url.Action("InvoiceSinglePrint", "PdfGeneration", new { OrderId = item });
                }
            }
            catch (Exception ex)
            {
                #region saveorupdate Errorlog
                OrdersService OrdSer = new OrdersService();
                ErrorLog err = new ErrorLog();
                err.Controller = "Invoice";
                err.Action = "GenerateInvoicePdfParallel";
                err.Err_Desc = ex.ToString();
                OrdSer.SaveOrUpdateErrorLog(err);
                #endregion
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }

        /// <summary>
        /// Generate Invoice Excel Single Document Parallel
        /// </summary>
        public void GenerateInvoiceExcelParallel()
        {
            try
            {
                //getting all the Info from Invoice Created and Document saved in Both PDF and Excel
                Dictionary<long, IList<InvoiceManagementView>> invoiceItems = IS.GetInvoiceListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                Dictionary<long, IList<ExcelDocuments>> EXCELDocumentItems = IS.GetExcelDocumentListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                //getting order list from invoiceItems
                var InvoiceOrderList = (from items in invoiceItems.First().Value

                                        select items.OrderId).OrderBy(i => i).Distinct().ToArray();
                //getting order list from EXCELDocumentItems
                var ExcelOrderList = (from items in EXCELDocumentItems.First().Value
                                      where items.OrderId != 0
                                      select items.OrderId).OrderBy(i => i).Distinct().ToArray();

                //For comparing Long arrays
                long[] INVOrdList = InvoiceOrderList.Select(i => i).ToArray();
                long[] ExcelOrdList = ExcelOrderList.Select(i => i).ToArray();

                //Taking orderid's of Pdf and Excel not in documents table
                var T2 = INVOrdList.Except(ExcelOrdList);

                //long []
                long[] NewExcelOrderId = T2.Select(i => i).ToArray();

                foreach (var item1 in NewExcelOrderId)
                {
                    CallInvoiceExcelParallel(item1);
                    //RedirectToAction("InvoiceSingleExcel", "ExcelGeneration", new { OrderId = item1 });
                    //@Url.Action("InvoiceSingleExcel", "ExcelGeneration", new { OrderId = item1 });
                }

            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }

        public ActionResult AllInvoiceGeneration()
        {
            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
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
        }

        public ActionResult RecentDownloads()
        {
            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
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
        }

        public ActionResult RecentDownloadsListJQGrid(string searchItems, int rows, string sidx, string sord, int? page = 1)
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
                            if (!string.IsNullOrWhiteSpace(Items[1])) { criteria.Add("Name", Items[1]); }
                            if (!string.IsNullOrWhiteSpace(Items[2])) { criteria.Add("ContingentType", Items[2]); }
                            //if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Location", Items[3]); }
                            if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Period", Items[3]); }
                            if (!string.IsNullOrWhiteSpace(Items[4])) { criteria.Add("PeriodYear", Items[4]); }
                            var TrimItems = searchItems.Replace(",", "");
                            if (string.IsNullOrWhiteSpace(TrimItems))
                                return null;
                        }
                        Dictionary<long, IList<RecentDownloads>> DownloadsItems = IS.GetRecentDowloadsListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                        if (DownloadsItems != null && DownloadsItems.Count > 0)
                        {
                            long totalrecords = DownloadsItems.First().Key;
                            int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                            var jsondata = new
                            {
                                total = totalPages,
                                page = page,
                                records = totalrecords,

                                rows = (from items in DownloadsItems.First().Value
                                        select new
                                        {
                                            i = 2,
                                            cell = new string[] {
                                            items.Id.ToString(),
                                            items.ControlId,
                                            items.Sector,
                                            items.ContingentType,
                                            items.Name,
                                            items.Period,
                                            items.PeriodYear,
                                            items.CreatedDate.ToString(),
                                            items.DocumentType,
                                            IsSubsite==true?String.Format(@"<a style='color:#034af3;text-decoration:underline' href= '/"+SubsiteName+"/Invoice/GetRecentDownloads?Id="+items.Id+"' target='_Blank' >{0}</a>",items.DocumentName):String.Format(@"<a style='color:#034af3;text-decoration:underline' href= '/Invoice/GetRecentDownloads?Id="+items.Id+"' target='_Blank' >{0}</a>",items.DocumentName)
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
                    ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                    throw ex;
                }
            }
        }

        public ActionResult GetRecentDownloads(long Id)
        {
            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
            {
                try
                {
                    RecentDownloads rd = IS.GetRecentDocumentsDetailsById(Id);

                    switch (rd.DocumentType)
                    {
                        case "PDF":
                            Response.ContentType = "application/pdf";
                            Response.AddHeader("content-disposition", "attachment;  filename=" + rd.ControlId + ".pdf");
                            break;
                        case "Excel":
                            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            Response.AddHeader("content-disposition", "attachment;  filename=" + rd.ControlId + ".xlsx");
                            break;
                        case "ZIP":
                            Response.ContentType = "application/octet-stream";
                            Response.AppendHeader("Content-Disposition", "attachment; filename=" + rd.ControlId + ".zip");
                            break;
                        default:
                            break;
                    }
                    Response.BinaryWrite(rd.DocumentData);
                    Response.End();
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public JsonResult ClearHistory(bool Clear)
        {
            try
            {
                if (Clear == true)
                {
                    IS.ClearRecordsIntable("RecentDownloads");
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #region Transport Invoice
        /// <summary>
        /// TansportInvoices View Pages
        /// </summary>
        /// <returns></returns>
        public ActionResult TransportInvoices()
        {
            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
            {
                try
                {

                    return View();
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "InsightInvoicePolicy");
                    throw ex;
                }
            }
        }
        /// <summary>
        /// Check and ReGenerate Transport Invoices One by one
        /// </summary>
        /// <param name="searchItems">Have Period and Period year</param>
        /// <param name="Ids">Selected Document in Transport Invoice(Already Created)</param>
        /// <returns></returns>
        public JsonResult TransportGenerateInvoice(string searchItems, string Ids)
        {
            string userId = base.ValidateUser();

            try
            {
                long[] DocumentIds = new long[Ids.Split(',').Length];
                //var DocumentIds = Ids.ToString().Split(',');
                if (!string.IsNullOrWhiteSpace(Ids))
                {
                    DocumentIds = Ids.Split(',').Select(n => Convert.ToInt64(n)).ToArray();
                }
                int Count = 0;

                //To Generate all the documents from Selected Ids
                if (DocumentIds.Length != 0 && DocumentIds[0] != 0)
                {
                    criteria.Clear();
                    criteria.Add("Id", DocumentIds);
                    Dictionary<long, IList<ExcelDocuments>> DocumentItems = IS.GetExcelDocumentListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                    var List = (from items in DocumentItems.First().Value
                                select new { items.Period, items.PeriodYear, items.Week }).OrderBy(i => i.Period).Distinct().ToArray();
                    foreach (var item in List)
                    {
                        GenerateTransportInvoices(item.Period, item.PeriodYear, item.Week);
                        Count = Count + 1;
                    }
                }
                else //To Generate all the documents from Searchitems
                {
                    if (searchItems != null && searchItems != "")
                    {
                        criteria.Clear();
                        var Items = searchItems.ToString().Split(',');
                        if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Period", Items[3]); }
                        if (!string.IsNullOrWhiteSpace(Items[4])) { criteria.Add("PeriodYear", Items[4]); }
                        if (!string.IsNullOrWhiteSpace(Items[5])) { criteria.Add("Week", Convert.ToInt64(Items[5])); }
                        var TrimItems = searchItems.Replace(",", "");
                        if (string.IsNullOrWhiteSpace(TrimItems))
                            return null;
                    }
                    Dictionary<long, IList<InvoiceManagementView>> invoiceItems = IS.GetInvoiceListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);

                    if (invoiceItems != null && invoiceItems.FirstOrDefault().Key > 0)
                    {
                        var PeriodList = (from items in invoiceItems.First().Value
                                          select new { items.Period, items.PeriodYear, items.Week }).OrderBy(i => i.Period).Distinct().ToArray();

                        foreach (var item in PeriodList)
                        {
                            GenerateTransportInvoices(item.Period, item.PeriodYear, item.Week);
                            Count = Count + 1;
                        }
                    }

                }

                return Json(Count, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightInvoicePolicy");
                throw ex;
            }
        }
        /// <summary>
        /// Createing an Work book and send it to the Excel generation controller
        /// </summary>
        /// <param name="Period"></param>
        /// <param name="PeriodYear"></param>
        public void GenerateTransportInvoices(string Period, string PeriodYear, Int64 Week)
        {
            try
            {
                string[] Invoices = new string[] { "TPT-FPU", "TPT-MIL", "FS FPU", "FS MIL", "NS FPU", "NS MIL", "GS FPU", "GS MIL" };
                DataSet DS = new DataSet("Workbook");

                criteria.Clear();
                criteria.Add("Period", Period);
                criteria.Add("PeriodYear", PeriodYear);
                criteria.Add("Week", Week);
                Dictionary<long, IList<TransportInvoice>> InvoiceList = IS.GetTransportInvoiceListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);

                List<TransportConsol> TConsolList = new List<TransportConsol>();
                List<TransportSingle> TSingleList = new List<TransportSingle>();

                if (InvoiceList != null && InvoiceList.FirstOrDefault().Key != 0)
                {
                    Invoice id = IS.GetInvoiceDetailsByOrderId(InvoiceList.FirstOrDefault().Value[0].OrderId);


                    foreach (var item in Invoices)
                    {
                        switch (item)
                        {
                            case "TPT-FPU":
                                TransportConsol TcFPU = GetTransportConsolInvoice(InvoiceList, "TPT", "FPU", id.InvoiceDate);
                                TConsolList.Add(TcFPU);
                                break;
                            case "TPT-MIL":
                                TransportConsol TcMIL = GetTransportConsolInvoice(InvoiceList, "TPT", "MIL", id.InvoiceDate);
                                TConsolList.Add(TcMIL);
                                break;
                            case "FS FPU":
                                TransportSingle TsnFPU = GetTransportSingleInvoice(InvoiceList, "FS", "FPU", id.InvoiceDate);
                                TSingleList.Add(TsnFPU);
                                break;
                            case "FS MIL":
                                TransportSingle TsnMIL = GetTransportSingleInvoice(InvoiceList, "FS", "MIL", id.InvoiceDate);
                                TSingleList.Add(TsnMIL);
                                break;
                            case "NS FPU":
                                TransportSingle TssFPU = GetTransportSingleInvoice(InvoiceList, "NS", "FPU", id.InvoiceDate);
                                TSingleList.Add(TssFPU);
                                break;
                            case "NS MIL":
                                TransportSingle TssMIL = GetTransportSingleInvoice(InvoiceList, "NS", "MIL", id.InvoiceDate);
                                TSingleList.Add(TssMIL);
                                break;
                            case "GS FPU":
                                TransportSingle TswFPU = GetTransportSingleInvoice(InvoiceList, "GS", "FPU", id.InvoiceDate);
                                TSingleList.Add(TswFPU);
                                break;
                            case "GS MIL":
                                TransportSingle TswMIL = GetTransportSingleInvoice(InvoiceList, "GS", "MIL", id.InvoiceDate);
                                TSingleList.Add(TswMIL);
                                break;
                            default:
                                break;
                        }
                    }
                }
                foreach (var item in Invoices)
                {

                    DataTable table = new DataTable();
                    table.TableName = item;
                    DS.Tables.Add(table);
                }
                base.TransportImportToExcelSheet(DS, TConsolList, TSingleList);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightInvoicePolicy");
                throw ex;
            }
        }
        /// <summary>
        /// Generating Single Transport Invoices
        /// </summary>
        /// <param name="InvoiceList"></param>
        /// <param name="Sector"></param>
        /// <param name="ContingentType"></param>
        /// <param name="InvoiceDate"></param>
        /// <returns></returns>
        private TransportSingle GetTransportSingleInvoice(Dictionary<long, IList<TransportInvoice>> InvoiceList, string Sector, string ContingentType, DateTime? InvoiceDate)
        {
            try
            {

                List<TransportInvoice> TinvoiceList = (from items in InvoiceList.First().Value
                                                       where items.Sector == Sector && items.ContingentType.Contains(ContingentType)
                                                       select items).OrderBy(i => i.ControlId).Distinct().ToList();


                decimal TotInvoiceQty = TinvoiceList.Sum(item => item.InvoiceQty);
                decimal TotTransCost = TinvoiceList.Sum(item => item.TransportCost);

                string SectorNo = "";
                string SectorName = "";
                switch (Sector)
                {
                    case "FS":
                        SectorNo = "-090-TPT-";
                        SectorName = "Sector North";
                        break;
                    case "NS":
                        SectorNo = "-091-TPT-";
                        SectorName = "Sector South";
                        break;
                    case "GS":
                        SectorNo = "-092-TPT-";
                        SectorName = "Sector West";
                        break;
                    default:
                        break;
                }

                int no = GetSerialNoforPeriod(TinvoiceList.FirstOrDefault().Period, TinvoiceList.FirstOrDefault().PeriodYear, TinvoiceList.FirstOrDefault().Week);

                return new TransportSingle()
                {
                    ReferenceNo = "GCC-" + Sector + SectorNo + ContingentType + "-" + no + "/01",
                    Sector = SectorName,
                    Period = TinvoiceList.FirstOrDefault().Period + "/" + TinvoiceList.FirstOrDefault().PeriodYear,
                    SubInvoiceDate = String.Format(new DateFormatInseries(), "{0}", InvoiceDate),
                    // PO = "MID14-62 - 2400001818", for P13 13-14 yr
                    //PO = "MID14-62 - 2200010314",
                    PO = GetPONumberFromMaster(TinvoiceList.FirstOrDefault().Period, TinvoiceList.FirstOrDefault().PeriodYear, "Transport"),
                    TotalAcceptedQty = string.Format("{0:N}", Math.Round(TotInvoiceQty, 2)),
                    TransportInvoiceList = TinvoiceList,
                    TotalTransportCost = string.Format("{0:C}", (Math.Round(TotTransCost, 2))),
                    Title = Sector + " " + ContingentType
                };
            }
            catch (Exception ex)
            {

                ExceptionPolicy.HandleException(ex, "InsightInvoicePolicy");
                throw ex;
            }
        }
        /// <summary>
        /// Generating Consolidate Transport Invoices
        /// </summary>
        /// <param name="InvoiceList"></param>
        /// <param name="Transprt"></param>
        /// <param name="ContingentType"></param>
        /// <param name="InvoiceDate"></param>
        /// <returns></returns>
        private TransportConsol GetTransportConsolInvoice(Dictionary<long, IList<TransportInvoice>> InvoiceList, string Transprt, string ContingentType, DateTime? InvoiceDate)
        {
            try
            {
                bool IsWeeklyDiscount = Convert.ToBoolean(ConfigurationManager.AppSettings["IsWeeklyDiscount"]);
                IList<TransportInvoice> TinvoiceList = (from items in InvoiceList.First().Value
                                                        where items.ContingentType.Contains(ContingentType)
                                                        select items).OrderBy(i => i.ControlId).Distinct().ToList();

                List<TransportFeeList> feesList = GetTransportFeesList(InvoiceList, ContingentType);

                //decimal Subtotal = feesList.Sum(item => item.TotalAmt);
                //decimal GrandTotal = feesList.Sum(item => item.TotalAmt);

                decimal TotalValue = feesList.Sum(item => item.TotalAmt);
                decimal WeeklyDiscount = 0; decimal Subtotal = 0; decimal GrandTotal = 0;

                if (IsWeeklyDiscount == true)
                {
                    WeeklyDiscount = Math.Round(Convert.ToDecimal((100 * 0.35)) * feesList.Sum(item => item.TotalAmt), 2);
                    Subtotal = WeeklyDiscount - TotalValue;
                    GrandTotal = WeeklyDiscount - TotalValue;
                }
                else
                {
                    Subtotal = TotalValue;
                    GrandTotal = TotalValue;
                }
                var values = GrandTotal.ToString(CultureInfo.InvariantCulture).Split('.');
                int firstValue = int.Parse(values[0]);
                int secondValue = int.Parse(values[1]);

                int no = GetSerialNoforPeriod(TinvoiceList.FirstOrDefault().Period, TinvoiceList.FirstOrDefault().PeriodYear, TinvoiceList.FirstOrDefault().Week);

                return new TransportConsol()
                {
                    InvoiceNo = "GCC-TPT-" + ContingentType + "-" + no,
                    Contract = "PD/C0036/13",
                    InvoiceDate = String.Format(new DateFormatInseries(), "{0}", InvoiceDate),
                    PayTerms = "30 Days",
                    // PO = "MID14-62 - 2400001818", for P13 13-14 yr
                    PO = GetPONumberFromMaster(TinvoiceList.FirstOrDefault().Period, TinvoiceList.FirstOrDefault().PeriodYear, "Transport"),
                    Period = TinvoiceList.FirstOrDefault().Period + "/" + TinvoiceList.FirstOrDefault().PeriodYear,
                    Week = TinvoiceList.FirstOrDefault().Week,
                    UnINo = ContingentType,
                    Sector = "North,South and West",
                    SubTotal = string.Format("{0:C}", (Math.Round(Subtotal, 2))),
                    GrandTotal = string.Format("{0:C}", (Math.Round(GrandTotal, 2))),
                    TransportFeeList = feesList,
                    Title = Transprt + "-" + ContingentType,
                    Usd_words = "Amount in Words : USD " + NumberToText(Convert.ToInt64(firstValue)) + " " + "Dollars" + " " + secondValue + "/100",
                    WeeklyDiscount = WeeklyDiscount
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightInvoicePolicy");
                throw ex;
            }
        }


        /// <summary>
        /// Generating Transport Fees list for Consolidate transport Invoice
        /// </summary>
        /// <param name="InvoiceList"></param>
        /// <param name="ContingentType"></param>
        /// <returns></returns>
        private List<TransportFeeList> GetTransportFeesList(Dictionary<long, IList<TransportInvoice>> InvoiceList, string ContingentType)
        {
            try
            {
                string[] Sector = new string[] { "FS", "NS", "GS" };

                List<TransportFeeList> FeeList = new List<TransportFeeList>();

                foreach (var item in Sector)
                {
                    IList<TransportInvoice> TinvoiceList = (from items in InvoiceList.First().Value
                                                            where items.Sector == item && items.ContingentType.Contains(ContingentType)
                                                            select items).OrderBy(i => i.ControlId).Distinct().ToList();

                    decimal AcceptedQty = TinvoiceList.Sum(i => i.InvoiceQty);
                    decimal TransportCost = TinvoiceList.Sum(i => i.TransportCost);

                    string SectorNo = "";
                    string SectorName = "";

                    switch (item)
                    {
                        case "FS":
                            SectorNo = "-090-TPT-";
                            SectorName = "Sector North";
                            break;
                        case "NS":
                            SectorNo = "-091-TPT-";
                            SectorName = "Sector South";
                            break;
                        case "GS":
                            SectorNo = "-092-TPT-";
                            SectorName = "Sector West";
                            break;
                        default:
                            break;
                    }

                    int no = GetSerialNoforPeriod(TinvoiceList.FirstOrDefault().Period, TinvoiceList.FirstOrDefault().PeriodYear, TinvoiceList.FirstOrDefault().Week);

                    TransportFeeList tranFee = new TransportFeeList();
                    tranFee.AcceptedQty = string.Format("{0:N}", Math.Round(AcceptedQty, 2));
                    tranFee.Description = SectorName + "-" + ContingentType + " Sub invoice reference  #GCC-" + item + SectorNo + "-" + ContingentType + "-" + no + "/01";
                    tranFee.TotalAmt = Math.Round(TransportCost, 2);
                    FeeList.Add(tranFee);

                }
                return FeeList;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightInvoicePolicy");
                throw ex;
            }
        }

        #endregion

        #region Added by kingston (DeliverySequnce in poditems related code)
        public ActionResult GenerateDeliverySequence()
        {

            return View();
        }

        //Generating the sequence for OrderQty and invoiceQty based on the sequence(Actual delivery,Replacement,Substitution)

        public ActionResult GenerateOrderQtyAndInvQtySequence(string Period, string PeriodYear, long Week)
        {


            try
            {


                Dictionary<string, object> criteria = new Dictionary<string, object>();
                if (!string.IsNullOrWhiteSpace(PeriodYear)) { criteria.Add("PeriodYear", PeriodYear); }
                if (!string.IsNullOrWhiteSpace(Period)) { criteria.Add("Period", Period); }
                if (Week > 0) { criteria.Add("Week", Week); }


                //Updating the RevisedOrderQty and InvoiceQty as 0

                IS.UpdateRevisedOrdQtyInvQtyAsZeroForDelSequence(Period, PeriodYear, Week);


                Dictionary<long, IList<DeliverySequence_PODItems>> deliveryseq = IS.GetDeliverySequenceListWithPagingAndCriteria(0, 99999999, string.Empty, string.Empty, criteria);

                IList<DeliverySequence_PODItems> deliveryseqlist = deliveryseq.FirstOrDefault().Value.ToList();
                var LineIdArray = (from u in deliveryseqlist select u.LineId).Distinct().ToArray();
                for (int i = 0; i < LineIdArray.Length; i++)
                {
                    var ItemList = (from u in deliveryseqlist orderby u.SequenceId where u.LineId == LineIdArray[i] select u).ToList();
                    var TotalDelivery = (from items in ItemList
                                         group items by items.LineId into g
                                         select
                                         g.Sum(p => p.DeliveredQty)
                                     ).ToArray();

                    decimal MaximumInvoiceQty = (decimal)(1.02) * ItemList[0].OrderedQty;
                    if ((ItemList[0].OrderedQty < TotalDelivery[0]) && (MaximumInvoiceQty > TotalDelivery[0]))
                    {
                        MaximumInvoiceQty = TotalDelivery[0];
                    }

                    decimal temp = MaximumInvoiceQty;
                    // decimal TotalDeliveryQty = 0;
                    for (int j = 0; j < ItemList.Count; j++)
                    {

                        PODItems poditems = OS.GetPodItemsValById(ItemList[j].PODItemsId);
                        poditems.MAXINVQTY = temp;
                        if (poditems != null)
                        {
                            if (ItemList[j].OrderedQty >= ItemList[j].DeliveredQty && MaximumInvoiceQty > 0)
                            {
                                if (ItemList[j].DeliveredQty <= MaximumInvoiceQty)
                                {

                                    if (ItemList[j].UNCode == 1129)
                                    {
                                        poditems.InvoiceQty = ItemList[j].DeliveredQty * (decimal)0.058824;
                                        OS.SaveOrUpdatePODItems(poditems);
                                    }
                                    else
                                    {
                                        poditems.InvoiceQty = ItemList[j].DeliveredQty;
                                        OS.SaveOrUpdatePODItems(poditems);
                                    }
                                }
                                else if (ItemList[j].DeliveredQty > MaximumInvoiceQty)
                                {
                                    if (ItemList[j].UNCode == 1129)
                                    {
                                        poditems.InvoiceQty = MaximumInvoiceQty * (decimal)0.058824;
                                        OS.SaveOrUpdatePODItems(poditems);
                                    }
                                    else
                                    {
                                        poditems.InvoiceQty = MaximumInvoiceQty;
                                        OS.SaveOrUpdatePODItems(poditems);
                                    }
                                }
                            }
                            else if (ItemList[j].OrderedQty < ItemList[j].DeliveredQty && MaximumInvoiceQty > 0)
                            {
                                if (ItemList[j].DeliveredQty <= MaximumInvoiceQty)
                                {
                                    if (ItemList[j].UNCode == 1129)
                                    {
                                        poditems.InvoiceQty = ItemList[j].DeliveredQty * (decimal)0.058824;
                                        OS.SaveOrUpdatePODItems(poditems);
                                    }
                                    else
                                    {
                                        poditems.InvoiceQty = ItemList[j].DeliveredQty;
                                        OS.SaveOrUpdatePODItems(poditems);
                                    }
                                }
                                else if (ItemList[j].DeliveredQty > MaximumInvoiceQty)
                                {
                                    if (ItemList[j].UNCode == 1129)
                                    {
                                        poditems.InvoiceQty = MaximumInvoiceQty * (decimal)0.058824;
                                        OS.SaveOrUpdatePODItems(poditems);
                                    }
                                    else
                                    {
                                        poditems.InvoiceQty = MaximumInvoiceQty;
                                        OS.SaveOrUpdatePODItems(poditems);
                                    }
                                }


                            }

                            MaximumInvoiceQty = MaximumInvoiceQty - ItemList[j].DeliveredQty;
                        }

                    }

                }
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightInvoicePolicy");
                throw ex;

            }
        }


        //public ActionResult GenerateOrderQtyAndInvQtySequence(string Period, string PeriodYear)
        //{
        //    Dictionary<string, object> criteria = new Dictionary<string, object>();
        //    if (!string.IsNullOrWhiteSpace(Period)) { criteria.Add("Period", Period); }
        //    if (!string.IsNullOrWhiteSpace(PeriodYear)) { criteria.Add("PeriodYear", PeriodYear); }

        //    //Updating the RevisedOrderQty and InvoiceQty as 0

        //    IS.UpdateRevisedOrdQtyInvQtyAsZeroForDelSequence(Period, PeriodYear);


        //    Dictionary<long, IList<DeliverySequence_PODItems>> deliveryseq = IS.GetDeliverySequenceListWithPagingAndCriteria(0, 99999999, string.Empty, string.Empty, criteria);

        //    IList<DeliverySequence_PODItems> deliveryseqlist = deliveryseq.FirstOrDefault().Value.ToList();
        //    var LineIdArray = (from u in deliveryseqlist select u.LineId).Distinct().ToArray();
        //    for (int i = 0; i < LineIdArray.Length; i++)
        //    {
        //        var ItemList = (from u in deliveryseqlist orderby u.SequenceId where u.LineId == LineIdArray[i] select u).ToList();
        //        var TotalDelivery = (from items in ItemList
        //                             group items by items.LineId into g
        //                             select
        //                             g.Sum(p => p.DeliveredQty)
        //                         ).ToArray();

        //        decimal MaximumInvoiceQty = (decimal)(1.02) * ItemList[0].OrderedQty;
        //        if ((ItemList[0].OrderedQty < TotalDelivery[0]) && (MaximumInvoiceQty > TotalDelivery[0]))
        //        {
        //            MaximumInvoiceQty = TotalDelivery[0];
        //        }


        //        // decimal TotalDeliveryQty = 0;
        //        for (int j = 0; j < ItemList.Count; j++)
        //        {
        //            PODItems poditems = OS.GetPodItemsValById(ItemList[j].PODItemsId);
        //            if (poditems != null)
        //            {
        //                // poditems.RevisedOrderQty = (ItemList[j].OrderedQty - TotalDeliveryQty);
        //                //---------------------------new code starts here-------------------------------------------
        //                if (ItemList[j].OrderedQty >= TotalDelivery[0] && ItemList[j].DeliveredQty > 0)
        //                {
        //                    if (ItemList[j].UNCode == 1129)
        //                    {
        //                        poditems.InvoiceQty = poditems.DeliveredQty * (decimal)0.0625;
        //                        OS.SaveOrUpdatePODItems(poditems);
        //                    }
        //                    else
        //                    {

        //                        poditems.InvoiceQty = poditems.DeliveredQty;
        //                        OS.SaveOrUpdatePODItems(poditems);
        //                    }

        //                }
        //                else if (ItemList[j].OrderedQty < TotalDelivery[0] && ItemList[j].DeliveredQty > 0 && MaximumInvoiceQty>0)
        //                {
        //                    if (ItemList[j].OrderedQty > ItemList[j].DeliveredQty && MaximumInvoiceQty > ItemList[j].DeliveredQty)
        //                    {
        //                        if (ItemList[j].UNCode == 1129)
        //                        {
        //                            poditems.InvoiceQty = ItemList[j].DeliveredQty * (decimal)0.0625;
        //                            OS.SaveOrUpdatePODItems(poditems);
        //                        }
        //                        else
        //                        {
        //                            poditems.InvoiceQty = ItemList[j].DeliveredQty;
        //                            OS.SaveOrUpdatePODItems(poditems);
        //                        }
        //                    }
        //                    else if (ItemList[j].OrderedQty >= ItemList[j].DeliveredQty && MaximumInvoiceQty <=ItemList[j].DeliveredQty)
        //                    {
        //                        if (ItemList[j].UNCode == 1129)
        //                        {
        //                            poditems.InvoiceQty = MaximumInvoiceQty * (decimal)0.0625;
        //                            OS.SaveOrUpdatePODItems(poditems);
        //                        }
        //                        else
        //                        {
        //                            poditems.InvoiceQty = MaximumInvoiceQty;
        //                            OS.SaveOrUpdatePODItems(poditems);
        //                        }


        //                    }
        //                    else if (ItemList[j].OrderedQty < ItemList[j].DeliveredQty)
        //                    {
        //                        if (ItemList[j].UNCode == 1129)
        //                        {
        //                            poditems.InvoiceQty = MaximumInvoiceQty * (decimal)0.0625;
        //                            OS.SaveOrUpdatePODItems(poditems);
        //                        }
        //                        else
        //                        {

        //                            poditems.InvoiceQty = MaximumInvoiceQty;
        //                            OS.SaveOrUpdatePODItems(poditems);
        //                        }
        //                    }

        //                }
        //                //else if (ItemList[j].OrderedQty == TotalDelivery[0])
        //                //{
        //                //    if (ItemList[j].UNCode == 1129)
        //                //    {
        //                //        poditems.InvoiceQty = ItemList[j].DeliveredQty * (decimal)0.0625;
        //                //        OS.SaveOrUpdatePODItems(poditems);
        //                //    }
        //                //    else
        //                //    {

        //                //        poditems.InvoiceQty = ItemList[j].DeliveredQty;
        //                //        OS.SaveOrUpdatePODItems(poditems);
        //                //    }

        //                //}

        //                MaximumInvoiceQty = MaximumInvoiceQty - ItemList[j].DeliveredQty;
        //            }

        //        }

        //    }
        //    return Json(null, JsonRequestBehavior.AllowGet);
        //}


        //public ActionResult GenerateOrderQtyAndInvQtySequence(string Period, string PeriodYear)
        //{
        //    Dictionary<string, object> criteria = new Dictionary<string, object>();
        //    if (!string.IsNullOrWhiteSpace(Period)) { criteria.Add("Period", Period); }
        //    if (!string.IsNullOrWhiteSpace(PeriodYear)) { criteria.Add("PeriodYear", PeriodYear); }

        //    //Updating the RevisedOrderQty and InvoiceQty as 0

        //    IS.UpdateRevisedOrdQtyInvQtyAsZeroForDelSequence(Period, PeriodYear);


        //    Dictionary<long, IList<DeliverySequence_PODItems>> deliveryseq = IS.GetDeliverySequenceListWithPagingAndCriteria(0, 99999999, string.Empty, string.Empty, criteria);

        //    IList<DeliverySequence_PODItems> deliveryseqlist = deliveryseq.FirstOrDefault().Value.ToList();
        //    var LineIdArray = (from u in deliveryseqlist select u.LineId).Distinct().ToArray();
        //    for (int i = 0; i < LineIdArray.Length; i++)
        //    {
        //        var ItemList = (from u in deliveryseqlist orderby u.SequenceId where u.LineId == LineIdArray[i] select u).ToList();
        //        decimal TotalDeliveryQty = 0;
        //        for (int j = 0; j < ItemList.Count; j++)
        //        {
        //            PODItems poditems = OS.GetPodItemsValById(ItemList[j].PODItemsId);
        //            if (poditems != null)
        //            {
        //                poditems.RevisedOrderQty = (ItemList[j].OrderedQty - TotalDeliveryQty);

        //                if (poditems.RevisedOrderQty > 0)
        //                {

        //                    OS.SaveOrUpdatePODItems(poditems);
        //                    #region InvoiceQty in poditems
        //                    if ((decimal)(1.02) * poditems.RevisedOrderQty < poditems.DeliveredQty && poditems.DeliveredQty != 0)
        //                    {
        //                        if (ItemList[j].UNCode == 1129)
        //                        {
        //                            poditems.InvoiceQty = (decimal)(1.02) * poditems.RevisedOrderQty * (decimal)0.0625;
        //                            OS.SaveOrUpdatePODItems(poditems);
        //                        }
        //                        else
        //                        {

        //                            poditems.InvoiceQty = (decimal)(1.02) * poditems.RevisedOrderQty;
        //                            OS.SaveOrUpdatePODItems(poditems);
        //                        }

        //                    }
        //                    else if (poditems.DeliveredQty != 0)
        //                    {
        //                        if (ItemList[j].UNCode == 1129)
        //                        {
        //                            poditems.InvoiceQty = poditems.DeliveredQty * (decimal)0.0625;
        //                            OS.SaveOrUpdatePODItems(poditems);
        //                        }
        //                        else
        //                        {

        //                            poditems.InvoiceQty = poditems.DeliveredQty;
        //                            OS.SaveOrUpdatePODItems(poditems);
        //                        }

        //                    }
        //                    #endregion
        //                }
        //                TotalDeliveryQty = TotalDeliveryQty + ItemList[j].DeliveredQty;
        //            }

        //        }

        //    }
        //    return Json(null, JsonRequestBehavior.AllowGet);
        //}

        #endregion

        public ActionResult PdfParallelTasking()
        {
            try
            {
                new Task(GenerateInvoicePdfParallel).Start();
                return RedirectToAction("PDFSingleInvoice", "PDFGeneration");
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightInvoicePolicy");
                throw ex;
            }
        }
        public ActionResult ExcelParallelTasking()
        {
            try
            {
                new Task(GenerateInvoiceExcelParallel).Start();
                return RedirectToAction("ExcelSingleInvoice", "ExcelGeneration");
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightInvoicePolicy");
                throw ex;
            }
        }

        public ActionResult ActivePeriods()
        {
            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
            {
                return View();
            }
        }

        public ActionResult ActivePeriodListJQGrid(string searchItems, string IsActive, int rows, string sidx, string sord, int? page = 1)
        {
            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(searchItems))
                    {

                        return null;
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(sord) && sord == "desc")
                            sord = "Desc";
                        else
                            sord = "Asc";
                        criteria.Clear();
                        Dictionary<long, IList<Invoice>> invoiceItems = null;

                        List<ActiveInvoices> ActiveList = new List<ActiveInvoices>();

                        if (searchItems != null && searchItems != "")
                        {
                            var Items = searchItems.ToString().Split(',');
                            bool Active = false;
                            var TrimItems = searchItems.Replace(",", "");
                            if (string.IsNullOrWhiteSpace(TrimItems))
                                return null;
                            if (!string.IsNullOrWhiteSpace(Items[0])) { criteria.Add("Sector", Items[0]); }
                            if (!string.IsNullOrWhiteSpace(Items[1])) { criteria.Add("Name", Items[1]); }
                            if (!string.IsNullOrWhiteSpace(Items[2])) { criteria.Add("ContingentType", Items[2]); }
                            if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Period", Items[3]); }
                            if (!string.IsNullOrWhiteSpace(Items[4])) { criteria.Add("PeriodYear", Items[4]); }
                            if (!string.IsNullOrWhiteSpace(IsActive))
                            {
                                if (IsActive == "1")
                                    Active = true;
                                else
                                    Active = false;
                                criteria.Add("IsActive", Active);
                            }

                            invoiceItems = IS.GetInvoicetableListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);

                            var InvoiceList = (from items in invoiceItems.First().Value
                                               //where items.Period != Items[3] && items.PeriodYear != Items[4]
                                               select new { items.Period, items.PeriodYear, items.InvoiceDate, items.IsActive }).OrderByDescending(i => i.Period).ThenBy(i => i.PeriodYear).Distinct().ToArray();

                            long Count = 0;
                            foreach (var item in InvoiceList)
                            {
                                ActiveInvoices ai = new ActiveInvoices();
                                ai.ActiveId = Count;
                                ai.Period = item.Period;
                                ai.PeriodYear = item.PeriodYear;
                                ai.InvoiceDate = item.InvoiceDate;
                                ai.IsActive = item.IsActive;
                                ai.RowCount = (from items in invoiceItems.First().Value
                                               where items.Period == item.Period && items.PeriodYear == item.PeriodYear
                                               select items).Count();
                                ActiveList.Add(ai);
                                Count = Count + 1;
                            }
                        }
                        if (ActiveList != null && ActiveList.Count > 0)
                        {
                            long totalrecords = ActiveList.Count();
                            int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                            var jsondata = new
                            {
                                total = totalPages,
                                page = page,
                                records = totalrecords,

                                rows = (from items in ActiveList
                                        select new
                                        {
                                            i = 2,
                                            cell = new string[] {
                                            items.ActiveId.ToString(),
                                            items.Period,
                                            items.PeriodYear,
                                            items.InvoiceDate !=null? items.InvoiceDate.ToString("dd/MM/yyyy") : "",
                                            items.RowCount.ToString(),
                                            items.IsActive ==true? "Completed" : "InProgress"
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
                    ExceptionPolicy.HandleException(ex, "InsightInvoicePolicy");
                    throw ex;
                }


            }
        }

        public ActionResult ChangeActivePeriods(string Period, string PeriodYear, string IsActive)
        {
            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
            {
                if (!string.IsNullOrWhiteSpace(Period)) { ViewBag.Period = Period; }
                if (!string.IsNullOrWhiteSpace(PeriodYear)) { ViewBag.PeriodYear = PeriodYear; }
                if (!string.IsNullOrWhiteSpace(IsActive))
                {
                    var Active = "";
                    if (IsActive == "InProgress")
                        Active = "0";
                    else if (IsActive == "Completed")
                        Active = "1";
                    else
                        Active = "";
                    ViewBag.IsActive = Active;
                }
                return View();
            }
        }
        public JsonResult UpdateActiveStatus(string Period, string PeriodYear, string IsActive)
        {
            IS.UpdateActiveStatusInTable(Period, PeriodYear, IsActive, "Invoice");
            IS.UpdateActiveStatusInTable(Period, PeriodYear, IsActive, "Documents_Pdf");
            IS.UpdateActiveStatusInTable(Period, PeriodYear, IsActive, "Documents_Excel");
            IS.UpdateActiveStatusInTable(Period, PeriodYear, IsActive, "InvoiceReports");
            return Json(IsActive, JsonRequestBehavior.AllowGet);
        }

        public List<long> DummyInvoiceGeneration(string searchItems, DateTime invoiceDt)
        {
            //string userId = base.ValidateUser();
            try
            {
                criteria.Clear();
                var Items = searchItems.ToString().Split(',');
                //Dummy Date
                invoiceDt = DateTime.Now;

                List<long> Ids = new List<long>();

                if (searchItems != null && searchItems != "")
                {
                    if (!string.IsNullOrWhiteSpace(Items[2])) { criteria.Add("Week", Convert.ToInt64(Items[2])); }
                    if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Period", Items[3]); }
                    if (!string.IsNullOrWhiteSpace(Items[4])) { criteria.Add("PeriodYear", Items[4]); }
                    var TrimItems = searchItems.Replace(",", "");
                    if (string.IsNullOrWhiteSpace(TrimItems))
                        return null;
                }
                Dictionary<long, IList<Orders>> orderList = OS.GetOrdersListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                Dictionary<long, IList<InvoiceManagementView>> invoiceItems = IS.GetInvoiceListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                var OrderIds = (from items in orderList.First().Value
                                select items.OrderId).OrderBy(i => i).Distinct().ToList();
                var invoiceOrderIds = (from items in invoiceItems.First().Value
                                       select items.OrderId).OrderBy(i => i).Distinct().ToList();
                foreach (var item in orderList.First().Value)
                {
                    if (item.OrderId != invoiceOrderIds.Find(i => i == item.OrderId))
                    {
                        Invoice I = new Invoice();
                        I.OrderId = item.OrderId;
                        I.InvoiceCode = "INV-" + item.Name + "-" + item.Period + "-WK" + item.Week;
                        I.Period = item.Period;
                        I.Week = (int)item.Week;
                        I.Sector = item.Sector;
                        I.TotalFeedTroopStrength = (long)item.Troops;
                        I.TotalMadays = (item.Troops * 7);
                        I.InvoiceDate = invoiceDt;
                        I.ModifiedDate = DateTime.Now;
                        I.CreatedDate = DateTime.Now;
                        I.PeriodYear = item.PeriodYear;
                        I.IsActive = false;
                        long id = IS.SaveOrUpdateInvoice(I, "Admin");
                        Ids.Add(id);
                        Orders Od = OS.GetOrdersById(item.OrderId);
                        Od.InvoiceId = I.Id;
                        OS.SaveOrUpdateOrder(Od);
                    }
                }

                return Ids;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightInvoicePolicy");
                throw ex;
            }
        }


        public ActionResult ExcelTest()
        {
            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
            {
                try
                {
                    WindowsMailServices wms = new WindowsMailServices();
                    string s1 = "SELECT ROW_NUMBER() OVER (ORDER BY ControlId) AS Id,ContingentLocation,ContingentType,ControlId,DeliveryMode,DeliverySector,Distance,InvoiceQty FROM TransportInvoice_vw";
                    string s2 = "ContingentLocation,ContingentType,ControlId,DeliveryMode,DeliverySector,Distance,InvoiceQty";
                    string s3 = "Transport RP";

                    byte[] data = wms.GenerateReportUsingQueryString(s1, s2, s3);
                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + s3 + ".xlsx");
                    Response.BinaryWrite(data);
                    Response.End();
                    return View();
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "InsightInvoicePolicy");
                    throw ex;
                }
            }
        }

    }
}



