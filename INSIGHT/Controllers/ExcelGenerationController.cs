using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using INSIGHT.Entities.PDFEntities;
using INSIGHT.WCFServices;
using INSIGHT.Entities.InvoiceEntities;
using INSIGHT.Entities;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using System.Globalization;
using System.Text;
using System.Data;
using System.Data.Common;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style;
using System.Threading.Tasks;
using System.Threading;
using PersistenceFactory;
using System.IO;
using System.Configuration;

namespace INSIGHT.Controllers
{
    public class ExcelGenerationController : PdfGenerationController
    {

        InvoiceService IS = new InvoiceService();
        OrdersService OS = new OrdersService();
        MastersService MS = new MastersService();
        Dictionary<string, object> criteria = new Dictionary<string, object>();


        public ActionResult SectorWorkbook(string Period, string searchItems)
        {

            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
            {
                try
                {
                    // Thread t1 = new Thread(QuickJob);
                    //t1.Start();
                    Period = "P07";
                    searchItems = "SN,,MIL,P07";
                    criteria.Clear();
                    var Items = searchItems.ToString().Split(',');
                    if (searchItems != null && searchItems != "")
                    {
                        if (!string.IsNullOrWhiteSpace(Items[0])) { criteria.Add("Sector", Items[0]); }
                        if (!string.IsNullOrWhiteSpace(Items[1])) { criteria.Add("Name", Items[1]); }
                        if (!string.IsNullOrWhiteSpace(Items[2])) { criteria.Add("ContingentType", Items[2]); }
                        if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Period", Items[3]); }
                        var TrimItems = searchItems.Replace(",", "");
                        if (string.IsNullOrWhiteSpace(TrimItems))
                            return null;
                    }
                    Dictionary<long, IList<InvoiceManagementView>> invoiceItems = IS.GetInvoiceListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                    var ordersList = (from u in invoiceItems.FirstOrDefault().Value
                                      select u.OrderId).Distinct().ToList();
                    DataSet Workbookset = new DataSet("WorkBook");

                    //DataSet Workbookset1 = MakeDataSetWithValues(ordersList, Period, searchItems);

                    //Consolidate Invoice
                    InvoiceList invoiceList = ConsolidateInvoiceList(Period, searchItems);
                    DataTable table1 = new DataTable();
                    table1.TableName = invoiceList.InvoiceNo;

                    Workbookset.Tables.Add(table1);


                    long[] arr = new long[] { 5203, 5204, 5205, 5206, 5207, 5208, };


                    List<SingleInvoice> stack = new List<SingleInvoice>();
                    for (int i = 0; i < arr.Count(); i++)
                    {
                        //Single Invoice
                        //SingleInvoice si = SingleInvoiceList(ordersList[i]);
                        SingleInvoice si = SingleInvoiceList(arr[i]);
                        stack.Add(si);
                        DataTable table = new DataTable();
                        table.TableName = si.UNID;
                        Workbookset.Tables.Add(table);
                    }
                    ImportToExcelSheet(Workbookset, invoiceList, stack, invoiceList.InvoiceNo, true, true, true);
                    return View();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        private void QuickJob()
        {
            Thread.Sleep(5000);
        }
        private DataSet MakeDataSetWithValues(List<long> ordersList, string Period, string searchItems)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Weekly Consolidate Invoice generation
        /// </summary>
        /// <param name="Workbookset"></param>
        /// <param name="invoiceList"></param>
        /// <param name="stack"></param>
        /// <param name="InvoiceNo"></param>
        /// <param name="Consol"></param>
        /// <param name="Single"></param>
        /// <param name="Generate"></param>
        public void ImportToExcelSheet(DataSet Workbookset, InvoiceList invoiceList, List<SingleInvoice> stack, string InvoiceNo, bool Consol, bool Single, bool Generate)
        {
            try
            {
                string userId = base.ValidateUser();
                using (ExcelPackage pck = new ExcelPackage())
                {
                    int TableCount = Workbookset.Tables.Count;
                    int x = 0; // Main Int is zero 
                    System.Drawing.Image logo = System.Drawing.Image.FromFile("D:\\HeaderImage/main_logo.jpg");
                    for (int i = 0; i < TableCount; i++)
                    {

                        if ((Workbookset.Tables[i].TableName.ToString() == InvoiceNo) && (Consol == true))
                        {
                            #region Consolidate
                            ExcelWorksheet ws = pck.Workbook.Worksheets.Add(Workbookset.Tables[i].TableName.ToString());
                            ws.View.ZoomScale = 85;
                            //Adding image 
                            int img = 0;
                            ws.Row(img * 5).Height = 39.00D;
                            var picture = ws.Drawings.AddPicture(img.ToString(), logo);
                            picture.SetPosition(1, 0, 1, 0);

                            //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                            ///line 1///
                            ws.Cells["D6"].LoadFromCollection("si.UNID", true);
                            ws.Cells["B6:K7"].Merge = true;
                            ws.Cells["B6:K7"].Value = "INVOICE";
                            ws.Cells["B6:K7"].Style.WrapText = true;
                            ws.Cells["B6:K7"].Style.Font.Bold = true;
                            ws.Cells["B6:K7"].Style.Font.Name = "Arial";
                            ws.Cells["B6:K7"].Style.Font.Size = 20;
                            ws.Cells["B6:K7"].AutoFitColumns();
                            ws.Cells["B6:K7"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B6:K7"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B6:K7"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B6:K7"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B6:K7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            ws.Cells["B8:N70"].Style.Font.Size = 10;
                            ws.Cells["B8:N70"].Style.Font.Name = "Arial";

                            ws.Cells["B8:G8"].Merge = true;
                            ws.Cells["B8"].Value = "To";
                            ws.Cells["B8:G8"].Style.Font.Bold = true;
                            ws.Cells["B8:G8"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B8:G8"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["H8:I8"].Merge = true;
                            ws.Cells["H8"].Value = "";
                            ws.Cells["H8:I8"].Style.Font.Bold = true;
                            ws.Cells["H8:I8"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["J8:K8"].Merge = true;
                            ws.Cells["J8"].Value = "";
                            ws.Cells["J8:K8"].Style.Font.Bold = true;
                            ws.Cells["J8:K8"].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                            ws.Cells["B9:G9"].Merge = true;
                            ws.Cells["B9"].Value = "                     Chief Rations Unit";
                            ws.Cells["B9:G9"].Style.Font.Bold = true;
                            ws.Cells["B9:G9"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B9:G9"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["H9:I9"].Merge = true;
                            ws.Cells["H9"].Value = "Invoice #:";
                            ws.Cells["H9:I9"].Style.Font.Bold = true;
                            ws.Cells["H9:I9"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["J9:K9"].Merge = true;
                            ws.Column(11).Width = 25;
                            ws.Cells["J9"].Value = invoiceList.InvoiceNo;                           //InvoiceNo
                            ws.Cells["J9:K9"].Style.Font.Bold = true;
                            ws.Cells["J9:K9"].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                            ws.Cells["B10:G10"].Merge = true;
                            ws.Cells["B10"].Value = "                     African Union - United Nations Hybrid ";
                            ws.Cells["B10:G10"].Style.Font.Bold = true;
                            ws.Cells["B10:G10"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B10:G10"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["H10:I10"].Merge = true;
                            ws.Cells["H10"].Value = "Contract #:";
                            ws.Cells["H10:I10"].Style.Font.Bold = true;
                            ws.Cells["H10:I10"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["J10:K10"].Merge = true;
                            ws.Cells["J10"].Value = invoiceList.Contract;                           //Contract
                            ws.Cells["J10:K10"].Style.Font.Bold = true;
                            ws.Cells["J10:K10"].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                            ws.Cells["B11:G11"].Merge = true;
                            ws.Cells["B11"].Value = "                     Operation in Darfur";
                            ws.Cells["B11:G11"].Style.Font.Bold = true;
                            ws.Cells["B11:G11"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B11:G11"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["H11:I11"].Merge = true;
                            ws.Cells["H11"].Value = "Invoice Date:";
                            ws.Cells["H11:I11"].Style.Font.Bold = true;
                            ws.Cells["H11:I11"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["J11:K11"].Merge = true;
                            ws.Cells["J11"].Value = invoiceList.InvoiceDate;                        //InvoiceDate
                            ws.Cells["J11:K11"].Style.Font.Bold = true;
                            ws.Cells["J11:K11"].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                            ws.Cells["B12:G12"].Merge = true;
                            ws.Cells["B12"].Value = "                     El Fasher, Darfur";
                            ws.Cells["B12:G12"].Style.Font.Bold = true;
                            ws.Cells["B12:G12"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B12:G12"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["H12:I12"].Merge = true;
                            ws.Cells["H12"].Value = "Payment Terms:";
                            ws.Cells["H12:I12"].Style.Font.Bold = true;
                            ws.Cells["H12:I12"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["J12:K12"].Merge = true;
                            ws.Cells["J12"].Value = invoiceList.PayTerms;                           //PayTerms
                            ws.Cells["J12:K12"].Style.Font.Bold = true;
                            ws.Cells["J12:K12"].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                            ws.Cells["B13:G13"].Merge = true;
                            ws.Cells["B13"].Value = "Cc:               Mission Designated Official";
                            ws.Cells["B13:G13"].Style.Font.Bold = true;
                            ws.Cells["B13:G13"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B13:G13"].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            ws.Cells["H13:I13"].Merge = true;
                            ws.Cells["H13"].Value = "PO:";
                            ws.Cells["H13:I13"].Style.Font.Bold = true;
                            ws.Cells["H13:I13"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["J13:K13"].Merge = true;
                            ws.Cells["J13"].Value = invoiceList.PO;                                 //PO
                            ws.Cells["J13:K13"].Style.Font.Bold = true;
                            ws.Cells["J13:K13"].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                            ws.Cells["B14:G14"].Merge = true;
                            ws.Cells["B14"].Value = "                   UNAMID El Fasher";
                            ws.Cells["B14:G14"].Style.Font.Bold = true;
                            ws.Cells["B14:G14"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B14:G14"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["H14:I14"].Merge = true;
                            ws.Cells["H14"].Value = "";
                            ws.Cells["H14:I14"].Style.Font.Bold = true;
                            ws.Cells["H14:I14"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["J14:K14"].Merge = true;
                            ws.Cells["J14"].Value = "";
                            ws.Cells["J14:K14"].Style.Font.Bold = true;
                            ws.Cells["J14:K14"].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                            ws.Cells["B15:G15"].Merge = true;
                            ws.Cells["B15:G15"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B15:G15"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B15:G15"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                            ws.Cells["H15:I15"].Merge = true;
                            ws.Cells["H15:I15"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["H15:I15"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                            ws.Cells["J15:K15"].Merge = true;
                            ws.Cells["J15:K15"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["J15:K15"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                            ws.Cells["C16:H21"].Style.Font.Bold = true;
                            ws.Cells["B16"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["K16"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B17"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["K17"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B18"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["K18"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B19"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["K19"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B20"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B21"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["K20"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["K21"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B21:K21"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                            ws.Cells["C16:F16"].Merge = true;
                            ws.Cells["C16:F16"].Value = "  Period:";
                            ws.Cells["G16:H16"].Merge = true;
                            ws.Cells["G16:H16"].Value = invoiceList.Period;                           //Period

                            ws.Cells["C17:F17"].Merge = true;
                            ws.Cells["C17:F17"].Value = "  UN Identification Number:";
                            ws.Cells["G17:K17"].Merge = true;
                            ws.Cells["G17"].Value = invoiceList.UnINo;                            //UnINo

                            ws.Cells["C18:F18"].Merge = true;
                            ws.Cells["C18:F18"].Value = "  Sector:";
                            ws.Cells["G18:H18"].Merge = true;
                            ws.Cells["G18:H18"].Value = invoiceList.Sector;                           //Sector

                            ws.Cells["C19:F19"].Merge = true;
                            ws.Cells["C19:F19"].Value = "  Total Feeding Troop Strength:";
                            ws.Cells["G19:H19"].Merge = true;
                            ws.Cells["G19:H19"].Value = invoiceList.TotalFeedingToop;                 //TotalFeedingToop

                            ws.Cells["C20:F20"].Merge = true;
                            ws.Cells["C20:F20"].Value = "  Total Mandays:";
                            ws.Cells["G20:H20"].Merge = true;
                            ws.Cells["G20:H20"].Value = invoiceList.TotMadays;                       //TotMadays

                            ws.Cells["C21:F21"].Merge = true;
                            ws.Cells["C21:F21"].Value = "  Accepted CMR";
                            ws.Cells["G21:H21"].Merge = true;
                            ws.Cells["G21:H21"].Value = invoiceList.CMR;                            //CMR

                            ws.Cells["B22:K22"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B22:K22"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B22:K22"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B22:L22"].Style.Font.Bold = true;

                            ws.Cells["B22:C22"].Merge = true;
                            ws.Cells["B22:C22"].Value = "Qty";
                            ws.Cells["B22:C22"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            ws.Cells["D22:G22"].Merge = true;
                            ws.Cells["D22:G22"].Value = "Description";
                            ws.Cells["D22:G22"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            ws.Cells["H22:I22"].Merge = true;
                            ws.Cells["H22:I22"].Value = "Amount in USD";
                            ws.Cells["H22:I22"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            ws.Cells["J22:K22"].Merge = true;
                            ws.Cells["J22:K22"].Value = "Amount in USD";
                            ws.Cells["J22:K22"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                            for (int j = 23; j < 50; j++)
                            {
                                ws.Cells["B" + j + ":C" + j + ""].Merge = true;
                                ws.Cells["D" + j + ":G" + j + ""].Merge = true;
                                ws.Cells["H" + j + ":I" + j + ""].Merge = true;
                                ws.Cells["J" + j + ":K" + j + ""].Merge = true;
                                ws.Cells["B" + j + ":K" + j + ""].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                                ws.Cells["B" + j + ":K" + j + ""].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            }
                            ws.Cells["D24:G24"].Value = "PROVISION OF FOOD RATIONS";
                            ws.Cells["D26:G26"].Value = invoiceList.Sector;
                            ws.Cells["D24:J26"].Style.Font.Bold = true;
                            ws.Cells["J26"].Value = "$   " + invoiceList.PeriodWeek[0].InvoiceValue;
                            ws.Cells["J26"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            ws.Cells["B27"].Value = string.Format("{0:N}", string.Format("{0:0.000}", invoiceList.PeriodWeek[0].AcceptedQty));
                            ws.Cells["B27"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["D27"].Value = "Total Value Delivered - Week 0" + invoiceList.PeriodWeek[0].Week;     //week
                            ws.Cells["H27"].Value = invoiceList.PeriodWeek[0].InvoiceValue;                               //InvoiceValue
                            ws.Cells["H27"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;


                            ws.Cells["D40:G40"].Value = "DISCOUNTS";
                            ws.Cells["D40:J40"].Style.Font.Bold = true;
                            ws.Cells["J40"].Value = "(" + Math.Round(invoiceList.WeeklyDiscount, 2) + ")";
                            ws.Cells["J40"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            ws.Cells["D41:G41"].Value = "       Troop strength discounts";
                            ws.Cells["D42:G42"].Value = "       Weekly Discount";
                            ws.Cells["H42"].Value = "(" + Math.Round(invoiceList.WeeklyDiscount, 2) + ")";
                            ws.Cells["H42"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;


                            ws.Cells["D44:G44"].Value = "PERFORMANCE MATRICES";
                            ws.Cells["D44:G44"].Style.Font.Bold = true;
                            #region APLPerformance OLD
                            //decimal decDelivery = Math.Round(Convert.ToDecimal(invoiceList.Delivery.Replace("Rs.", "$").Replace("$", "")), 2);
                            //decimal decOrderLineItems = Math.Round(Convert.ToDecimal(invoiceList.OrderLineItems.Replace("Rs.", "$").Replace("$", "")), 2);
                            //decimal decOrderByWeight = Math.Round(Convert.ToDecimal(invoiceList.OrderByWeight.Replace("Rs.", "$").Replace("$", "")), 2);
                            //decimal decComplainAvalability = Math.Round(Convert.ToDecimal(invoiceList.ComplainAvalability.Replace("Rs.", "$").Replace("$", "")), 2);

                            //decimal totalPerformance = decDelivery + decOrderLineItems + decOrderByWeight + decComplainAvalability;
                            #endregion

                            decimal decDelivery = 0;
                            decimal decOrderLineItems = 0;
                            decimal decOrderByWeight = 0;
                            decimal decComplainAvalability = 0;
                            decimal totalPerformance = 0;

                            if (invoiceList.IsAPL == true)
                            {
                                decDelivery = Math.Round(Convert.ToDecimal(invoiceList.Delivery.Replace("Rs.", "$").Replace("$", "")), 2);
                                decOrderLineItems = Math.Round(Convert.ToDecimal(invoiceList.OrderLineItems.Replace("Rs.", "$").Replace("$", "")), 2);
                                decOrderByWeight = Math.Round(Convert.ToDecimal(invoiceList.OrderByWeight.Replace("Rs.", "$").Replace("$", "")), 2);
                                decComplainAvalability = Math.Round(Convert.ToDecimal(invoiceList.ComplainAvalability.Replace("Rs.", "$").Replace("$", "")), 2);
                                totalPerformance = decDelivery + decOrderLineItems + decOrderByWeight + decComplainAvalability;
                            }


                            ws.Cells["J44:K44"].Value = "(" + totalPerformance + ")";
                            ws.Cells["J44:K44"].Style.Font.Bold = true;
                            ws.Cells["J44:K44"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            ws.Cells["D45:G45"].Value = "       Conformity to Delivery Schedule";
                            if (invoiceList.IsAPL == true)
                                ws.Cells["H45:I45"].Value = "-" + invoiceList.Delivery;                                                           //Delivery
                            else
                                ws.Cells["H45:I45"].Value = "0.00";

                            ws.Cells["H45:I45"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            ws.Cells["D46:G46"].Value = "       Conformity to Order by Line Items";
                            if (invoiceList.IsAPL == true)
                                ws.Cells["H46:I46"].Value = "-" + invoiceList.OrderLineItems;                                                     //OrderLineItems
                            else
                                ws.Cells["H46:I46"].Value = "0.00";
                            ws.Cells["H46:I46"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            ws.Cells["D47:G47"].Value = "       Conformity to Orders by Weight";
                            if (invoiceList.IsAPL == true)
                                ws.Cells["H47:I47"].Value = "-" + invoiceList.OrderByWeight;                                                      //OrderByWeight
                            else
                                ws.Cells["H47:I47"].Value = "0.00";

                            ws.Cells["H47:I47"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            ws.Cells["D48:G48"].Value = "       Food Order Compliance-Availability";
                            if (invoiceList.IsAPL == true)
                                ws.Cells["H48:I48"].Value = "-" + invoiceList.ComplainAvalability;                                                //ComplainAvalability
                            else
                                ws.Cells["H48:I48"].Value = "0.00";
                            ws.Cells["H48:I48"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            ws.Cells["B50"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["K50"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B50:K50"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B50:K50"].Style.Border.Top.Style = ExcelBorderStyle.Medium;

                            ws.Cells["H50:J50"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["H50:J50"].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                            ws.Cells["B50:G50"].Merge = true;
                            ws.Cells["H50:K50"].Style.Font.Bold = true;
                            ws.Cells["H50:I50"].Merge = true;
                            ws.Cells["H50:I50"].Value = "NET TOTAL";
                            //ws.Cells["H50:I50"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                            ws.Cells["J50:K50"].Merge = true;
                            if (invoiceList.IsAPL == true)
                                ws.Cells["J50:K50"].Value = invoiceList.SubTotal;            //SubTotal
                            else
                                ws.Cells["J50:K50"].Value = "$   " + invoiceList.PeriodWeek[0].InvoiceValue;
                            ws.Cells["J50:K50"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            //ws.Cells["J50:K50"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                            ws.Cells["B51:K51"].Merge = true;
                            ws.Cells["B51:K51"].Value = "Amount in Words : USD " + invoiceList.Usd_words + " Only";
                            ws.Cells["B51:K51"].Style.Font.Bold = true;
                            ws.Cells["B51:K51"].Style.Font.Color.SetColor(Color.Red);
                            ws.Cells["B51:K51"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B51:K51"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B51:K51"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B51:K51"].Style.Border.Left.Style = ExcelBorderStyle.Medium;

                            ws.Cells["B52:I53"].Merge = true;
                            ws.Cells["B52:I53"].Style.WrapText = true;
                            ws.Cells["B52:I53"].Value = "A Prompt Payment Discount of 0.2% of the NET TOTAL applies if payment is made in less than 30 days.";
                            ws.Cells["B52:I53"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            ws.Cells["B52:I53"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
                            ws.Cells["J52:K53"].Merge = true;
                            ws.Cells["J52:K53"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            ws.Cells["J52:K53"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            ws.Cells["J52:K53"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                            ws.Cells["B54:K54"].Style.Font.Bold = true;
                            ws.Cells["B54:G54"].Merge = true;
                            ws.Cells["B54:G54"].Value = "IF payment is made in less than 30 days, the PPD is: ";

                            //decimal PPD = Math.Round(Convert.ToDecimal(0.2 / 100) * (Convert.ToDecimal(invoiceList.SubTotal.Replace("Rs.", "$").Replace("$", ""))), 2);

                            decimal PPD = 0;
                            if (invoiceList.IsAPL == true)
                                PPD = Math.Round(Convert.ToDecimal(0.2 / 100) * (Convert.ToDecimal(invoiceList.SubTotal.Replace("Rs.", "$").Replace("$", ""))), 2);
                            else
                                PPD = Math.Round(Convert.ToDecimal(0.2 / 100) * (Convert.ToDecimal(invoiceList.PeriodWeek[0].InvoiceValue.Replace("Rs.", "$").Replace("$", ""))), 2);


                            ws.Cells["H54:I54"].Merge = true;
                            ws.Cells["H54:I54"].Value = "$" + PPD;


                            ws.Cells["H54:I54"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            ws.Cells["H54:I54"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            ws.Cells["J54:K54"].Merge = true;
                            ws.Cells["H54"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            ws.Cells["J54:K54"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            ws.Cells["J54:K54"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                            ws.Cells["B51:K54"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B51:K54"].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                            ws.Cells["B55"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["K55"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B55:K55"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B55:K55"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                            ws.Cells["B56:K56"].Merge = true;
                            ws.Cells["B56:K56"].Value = "Bank Details";
                            ws.Cells["B56:K56"].Style.Font.UnderLine = true;
                            ws.Cells["B56:K56"].Style.Font.Bold = true;
                            ws.Cells["B57:K57"].Merge = true;
                            ws.Cells["B57:K57"].Value = "   Bank account name: Gulf Catering Company for General Trade and Contracting WLL";
                            ws.Cells["B58:K58"].Merge = true;
                            ws.Cells["B58:K58"].Value = "   Bank name: GULF Bank";
                            ws.Cells["B59:K59"].Merge = true;
                            ws.Cells["B59:K59"].Value = "   Bank account number: KW57GULB0000000000000090622676;";
                            ws.Cells["B60:K60"].Merge = true;
                            ws.Cells["B60:K60"].Value = "   Bank address: Qibla Area Hamad Al-Saqr Street, Kharafi Tower, First Floor, P.O. Box 1683, Safat, Kuwait City, Kuwait 1683";
                            ws.Cells["B61:K61"].Merge = true;
                            ws.Cells["B62:K62"].Merge = true;
                            ws.Cells["B61:K61"].Value = "   SWIFT/ABA: GULBKWKW";
                            ws.Cells["B56:K62"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B56:K62"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B62:K62"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B64:K64"].Merge = true;
                            ws.Cells["B64:K64"].Value = "Email:  UNAMIDAR@GCCServices.com";
                            ws.Cells["B65:K65"].Merge = true;
                            ws.Cells["B65:K65"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B65:K65"].Style.Border.Top.Color.SetColor(Color.Orange);
                            ws.Cells["B65:K65"].Value = "Gulf Catering Company for General Trade and Contracting WLL  - P.O Box 4583, Safat 13046, Kuwait";
                            ws.Cells["B66"].Style.Font.Bold = true;
                            ws.Cells["B66"].Value = "Disclaimer:";
                            ws.Cells["B67:K69"].Merge = true;
                            ws.Cells["B67:K69"].Style.WrapText = true;
                            //ws.Row(67).Height=35;
                            ws.Cells["B67:K69"].Value = "In the interest of ensuring a smooth invoicing/payment process, GCC SERVICES herewith signs this GRR with the intent to officially review the Weekly Billing Discount and APL formulas.  We will submit a correction/recovery request as applicable.";
                            for (int m = 7; m < 50; m++)
                            {
                                ws.Cells["B" + m + ":K" + m + ""].Style.Border.Top.Style = ExcelBorderStyle.Hair;
                                ws.Cells["B" + m + ":K" + m + ""].Style.Border.Top.Color.SetColor(Color.Red);
                            }
                            //To Make Sheet without GridLines
                            ws.View.ShowGridLines = false;

                            //Printer Settings

                            ws.PrinterSettings.TopMargin = ws.PrinterSettings.BottomMargin = 0.25M;
                            ws.PrinterSettings.LeftMargin = ws.PrinterSettings.RightMargin = 0M;
                            ws.PrinterSettings.HeaderMargin = 0M;
                            ws.PrinterSettings.FooterMargin = 0.25M;
                            ws.PrinterSettings.Orientation = eOrientation.Portrait;
                            ws.PrinterSettings.PaperSize = ePaperSize.A4;
                            ws.PrinterSettings.FitToPage = true;
                            ws.PrinterSettings.FitToWidth = 1;
                            ws.PrinterSettings.FitToHeight = 1;
                            ws.PrinterSettings.HorizontalCentered = true;
                            ws.PrinterSettings.VerticalCentered = true;



                            #endregion Consolidate
                        }
                        else if (stack != null && (Workbookset.Tables[i].TableName.ToString() == stack[x].UNID) && (Single == true))
                        {
                            #region Single


                            ExcelWorksheet ws = pck.Workbook.Worksheets.Add(Workbookset.Tables[i].TableName.ToString());
                            ws.View.ZoomScale = 70;
                            ws.View.ShowGridLines = false;

                            int img = 0;
                            //for Adding image 
                            ws.Row(img * 5).Height = 39.00D;
                            var picture = ws.Drawings.AddPicture(img.ToString(), logo);
                            picture.SetPosition(1, 0, 0, 0);

                            ws.Cells["A6:AE276"].Style.Font.Size = 12;
                            Color BlueHex = System.Drawing.ColorTranslator.FromHtml("#8DB4E2");
                            Color WhiteHex = System.Drawing.ColorTranslator.FromHtml("#F2F2F2");
                            Color OrangeHex = System.Drawing.ColorTranslator.FromHtml("#FF9933");
                            Color AshHex = System.Drawing.ColorTranslator.FromHtml("#BFBFBF");
                            Color BottomHex = System.Drawing.ColorTranslator.FromHtml("#C4BD97");

                            ws.Cells["A6:N11"].Style.Font.Bold = true;
                            ws.Cells["A6:B6"].Merge = true;
                            ws.Cells["A6:B6"].Value = "Reference #";
                            ws.Cells["C6:H6"].Merge = true;
                            ws.Cells["C6:H6"].Value = stack[x].Reference;
                            ws.Cells["I6:K6"].Merge = true;
                            ws.Cells["I6:K6"].Value = "Period:";
                            ws.Cells["L6:M6"].Merge = true;
                            ws.Cells["L6:M6"].Value = stack[x].Period;

                            ws.Cells["A7:B7"].Merge = true;
                            ws.Cells["A7:B7"].Value = "Delivery Point:";
                            ws.Cells["C7:H7"].Merge = true;
                            ws.Cells["C7:H7"].Value = stack[x].DeliveryPoint;
                            ws.Cells["I7:K7"].Merge = true;
                            ws.Cells["I7:K7"].Value = "DOS:";
                            ws.Cells["L7:M7"].Merge = true;
                            ws.Cells["L7:M7"].Value = stack[x].DOS;

                            ws.Cells["A8:B8"].Merge = true;
                            ws.Cells["A8:B8"].Value = "UN ID of the FFO:";
                            ws.Cells["C8:H8"].Merge = true;
                            ws.Cells["C8:H8"].Value = stack[x].UNID;
                            ws.Cells["I8:K8"].Merge = true;
                            ws.Cells["I8:K8"].Value = "Delivery Week:";
                            ws.Cells["L8:M8"].Merge = true;
                            ws.Cells["L8:M8"].Value = stack[x].DeliveryWeek;

                            ws.Cells["A9:B9"].Merge = true;
                            ws.Cells["A9:B9"].Value = "Strength:";
                            ws.Cells["C9:H9"].Merge = true;
                            ws.Cells["C9:H9"].Value = stack[x].Strength;
                            ws.Cells["I9:K9"].Merge = true;
                            ws.Cells["I9:K9"].Value = "Delivery Mode:";
                            ws.Cells["L9:M9"].Merge = true;
                            ws.Cells["L9:M9"].Value = stack[x].DeliveryMode;

                            ws.Cells["A10:B10"].Merge = true;
                            ws.Cells["A10:B10"].Value = "Man Days:";
                            ws.Cells["C10:H10"].Merge = true;
                            ws.Cells["C10:H10"].Value = stack[x].ManDays;
                            ws.Cells["I10:K10"].Merge = true;
                            ws.Cells["I10:K10"].Value = "Approved Delivery Dates:";
                            ws.Cells["L10:M10"].Merge = true;
                            ws.Cells["L10:M10"].Value = stack[x].ApprovedDelivery;

                            ws.Cells["A11:B11"].Merge = true;
                            ws.Cells["A11:B11"].Value = "Accepted CMR";
                            ws.Cells["C11:H11"].Merge = true;
                            ws.Cells["C11:H11"].Value = stack[x].AcceptedCMR;
                            //ws.Cells["C11:H11"].Value = stack[x].ApplicableCMR;
                            ws.Cells["I11:K11"].Merge = true;
                            ws.Cells["I11:K11"].Value = "Actual Delivery Date";
                            ws.Cells["L11:M11"].Merge = true;
                            ws.Cells["L11:M11"].Value = stack[x].ActualDeliveryDate;


                            ws.Cells["A6:M11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                            ws.Cells["A13:AC14"].Style.Font.Bold = true;
                            ws.Cells["A13:AC14"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A13:AC14"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A13:AC14"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A13:AC14"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A13:AC14"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["A13:AC14"].Style.Fill.BackgroundColor.SetColor(BlueHex);
                            ws.Cells["A13:AC14"].Style.Font.Color.SetColor(Color.White);
                            ws.Cells["A13:AC14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                            ws.Cells["A13:B14"].Merge = true;
                            ws.Cells["A13:B14"].Value = "S.NO";
                            ws.Cells["A13:B14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["A13:B14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["C13:D14"].Merge = true;
                            ws.Cells["C13:D14"].Value = "UNRS NO";
                            ws.Cells["C13:D14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["C13:D14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["E13:I14"].Merge = true;
                            ws.Cells["E13:I14"].Value = "Discription";
                            ws.Cells["E13:I14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["E13:I14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["J13:K14"].Merge = true;
                            ws.Cells["J13:K14"].Value = "Order Qty";
                            ws.Cells["J13:K14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["J13:K14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["L13:M14"].Merge = true;
                            ws.Cells["L13:M14"].Value = "Delivery Qty";
                            ws.Cells["L13:M14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["L13:M14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["N13:O14"].Merge = true;
                            ws.Cells["N13:O14"].Value = "Accepted Qty";
                            ws.Cells["N13:O14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["N13:O14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["P13:Q14"].Merge = true;
                            ws.Cells["P13:Q14"].Value = "Price/Unit";
                            ws.Cells["P13:Q14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["P13:Q14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["R13:S14"].Merge = true;
                            ws.Cells["R13:S14"].Value = "Net Amount";
                            ws.Cells["R13:S14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["R13:S14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["T13:U14"].Merge = true;
                            ws.Cells["T13:U14"].Value = "APL Weight";
                            ws.Cells["T13:U14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["T13:U14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["V13:W14"].Merge = true;
                            ws.Cells["V13:W14"].Value = "Discrepancy Code";
                            ws.Cells["V13:W14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["V13:W14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["X13:X14"].Merge = true;
                            ws.Cells["X13:X14"].Value = "UOM";
                            ws.Cells["X13:X14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["Y13:Z14"].Merge = true;
                            ws.Cells["Y13:Z14"].Value = "Order Value";
                            ws.Cells["Y13:Z14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["Y13:Z14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["AA13:AC14"].Merge = true;
                            ws.Cells["AA13:AC14"].Value = "DN #";
                            ws.Cells["AA13:AC14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["AA13:AC14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                            ws.Cells["A15:B15"].Merge = true;
                            ws.Cells["C15:D15"].Merge = true;
                            ws.Cells["E15:I15"].Merge = true;
                            ws.Cells["J15:K15"].Merge = true;
                            ws.Cells["L15:M15"].Merge = true;
                            ws.Cells["N15:O15"].Merge = true;
                            ws.Cells["P15:Q15"].Merge = true;
                            ws.Cells["R15:S15"].Merge = true;
                            ws.Cells["T15:U15"].Merge = true;
                            ws.Cells["V15:W15"].Merge = true;
                            ws.Cells["X15:X15"].Merge = true;
                            ws.Cells["Y15:Z15"].Merge = true;
                            ws.Cells["AA15:AC15"].Merge = true;
                            ws.Cells["A15:AC15"].Style.Border.Left.Style = ExcelBorderStyle.Hair;
                            ws.Cells["AC15"].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                            for (int l = 16; l < 166; l++)
                            {
                                ws.Cells["A" + l + ":B" + l + ""].Merge = true;
                                ws.Cells["A" + l + ":B" + l + ""].Value = "";
                                ws.Cells["A" + l + ":B" + l + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["C" + l + ":D" + l + ""].Merge = true;
                                ws.Cells["C" + l + ":D" + l + ""].Value = "";
                                ws.Cells["C" + l + ":D" + l + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["E" + l + ":I" + l + ""].Merge = true;
                                ws.Cells["E" + l + ":I" + l + ""].Value = "";
                                ws.Cells["J" + l + ":K" + l + ""].Merge = true;
                                ws.Cells["J" + l + ":K" + l + ""].Value = "";
                                ws.Cells["J" + l + ":K" + l + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                ws.Cells["L" + l + ":M" + l + ""].Merge = true;
                                ws.Cells["L" + l + ":M" + l + ""].Value = "";
                                ws.Cells["L" + l + ":M" + l + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                ws.Cells["N" + l + ":O" + l + ""].Merge = true;
                                ws.Cells["N" + l + ":O" + l + ""].Value = "";
                                ws.Cells["N" + l + ":O" + l + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                ws.Cells["P" + l + ":Q" + l + ""].Merge = true;
                                ws.Cells["P" + l + ":Q" + l + ""].Value = "";
                                ws.Cells["P" + l + ":Q" + l + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                ws.Cells["R" + l + ":S" + l + ""].Merge = true;
                                ws.Cells["R" + l + ":S" + l + ""].Value = "";
                                ws.Cells["R" + l + ":S" + l + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                ws.Cells["T" + l + ":U" + l + ""].Merge = true;
                                ws.Cells["T" + l + ":U" + l + ""].Value = "";
                                ws.Cells["T" + l + ":U" + l + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                ws.Cells["V" + l + ":W" + l + ""].Merge = true;
                                ws.Cells["V" + l + ":W" + l + ""].Value = "";
                                ws.Cells["V" + l + ":W" + l + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                ws.Cells["X" + l + ":X" + l + ""].Merge = true;
                                ws.Cells["X" + l + ":X" + l + ""].Value = "";
                                ws.Cells["Y" + l + ":Z" + l + ""].Merge = true;
                                ws.Cells["Y" + l + ":Z" + l + ""].Value = "";
                                ws.Cells["Y" + l + ":Z" + l + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                ws.Cells["AA" + l + ":AC" + l + ""].Merge = true;
                                ws.Cells["AA" + l + ":AC" + l + ""].Value = "";
                                ws.Cells["A" + l + ":AC" + l + ""].Style.Border.Right.Style = ExcelBorderStyle.Hair;
                                ws.Cells["AC" + l + ""].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            }



                            int temp = 0;
                            int orderCount = (16 + stack[x].DeliveryDetails.Count());
                            int s = 0;
                            if (stack[x].DeliveryDetails.Count() != 0)
                            {
                                for (int a = 16; a < orderCount; a++)
                                {
                                    temp = temp + 1;
                                    ws.Cells["A" + a + ":B" + a + ""].Merge = true;
                                    ws.Cells["A" + a + ":B" + a + ""].Value = temp;
                                    ws.Cells["A" + a + ":B" + a + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells["C" + a + ":D" + a + ""].Merge = true;
                                    ws.Cells["C" + a + ":D" + a + ""].Value = stack[x].DeliveryDetails[s].UNCode;
                                    ws.Cells["C" + a + ":D" + a + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells["E" + a + ":I" + a + ""].Merge = true;
                                    ws.Cells["E" + a + ":I" + a + ""].Value = stack[x].DeliveryDetails[s].Commodity;
                                    ws.Cells["J" + a + ":K" + a + ""].Merge = true;
                                    ws.Cells["J" + a + ":K" + a + ""].Value = Math.Round(stack[x].DeliveryDetails[s].OrderQty, 3);
                                    ws.Cells["J" + a + ":K" + a + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    ws.Cells["L" + a + ":M" + a + ""].Merge = true;
                                    ws.Cells["L" + a + ":M" + a + ""].Value = Math.Round(stack[x].DeliveryDetails[s].DeliveredOrdQty, 3);
                                    ws.Cells["L" + a + ":M" + a + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    ws.Cells["N" + a + ":O" + a + ""].Merge = true;
                                    ws.Cells["N" + a + ":O" + a + ""].Value = Math.Round(stack[x].DeliveryDetails[s].InvoiceQty, 3);
                                    ws.Cells["N" + a + ":O" + a + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    ws.Cells["P" + a + ":Q" + a + ""].Merge = true;
                                    ws.Cells["P" + a + ":Q" + a + ""].Value = "$" + Math.Round(stack[x].DeliveryDetails[s].SectorPrice, 2);
                                    ws.Cells["P" + a + ":Q" + a + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    ws.Cells["R" + a + ":S" + a + ""].Merge = true;
                                    ws.Cells["R" + a + ":S" + a + ""].Value = "$" + Math.Round(stack[x].DeliveryDetails[s].NetAmt, 2);
                                    ws.Cells["R" + a + ":S" + a + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    ws.Cells["T" + a + ":U" + a + ""].Merge = true;
                                    ws.Cells["T" + a + ":U" + a + ""].Value = Math.Round(stack[x].DeliveryDetails[s].APLWeight, 2) + "%";
                                    ws.Cells["T" + a + ":U" + a + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    ws.Cells["V" + a + ":W" + a + ""].Merge = true;
                                    ws.Cells["V" + a + ":W" + a + ""].Value = stack[x].DeliveryDetails[s].DiscrepancyCode;
                                    ws.Cells["V" + a + ":W" + a + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells["X" + a + ":X" + a + ""].Merge = true;
                                    ws.Cells["X" + a + ":X" + a + ""].Value = stack[x].DeliveryDetails[s].UOM;
                                    ws.Cells["Y" + a + ":Z" + a + ""].Merge = true;
                                    ws.Cells["Y" + a + ":Z" + a + ""].Value = "$" + Math.Round(stack[x].DeliveryDetails[s].OrderValue, 2);
                                    ws.Cells["Y" + a + ":Z" + a + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    ws.Cells["AA" + a + ":AC" + a + ""].Merge = true;
                                    ws.Cells["AA" + a + ":AC" + a + ""].Value = stack[x].DeliveryDetails[s].DeliveryNote;
                                    ws.Cells["AA" + a + ":AC" + a + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    ws.Cells["A" + a + ":AC" + a + ""].Style.Border.Right.Style = ExcelBorderStyle.Hair;
                                    ws.Cells["AC" + a + ""].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                                    s = s + 1;
                                }
                            }
                            ws.Cells["A165:AC165"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                            ws.Cells["A166:B168"].Merge = true;
                            ws.Cells["A166:B168"].Value = temp;
                            ws.Cells["A166:B168"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["A166:B168"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                            ws.Cells["C166:I166"].Merge = true;
                            ws.Cells["C166:I166"].Value = "Sub total without Eggs..................................................................................";
                            ws.Cells["J166:K166"].Merge = true;
                            ws.Cells["J166:K166"].Value = string.Format("{0:N}", string.Format("{0:0.000}", stack[x].OrderedQtySum));
                            ws.Cells["L166:M166"].Merge = true;
                            ws.Cells["L166:M166"].Value = string.Format("{0:N}", string.Format("{0:0.000}", stack[x].DeliveredQtySum));
                            ws.Cells["N166:O166"].Merge = true;
                            ws.Cells["N166:O166"].Value = string.Format("{0:N}", string.Format("{0:0.000}", stack[x].InvoiceQtySum));
                            ws.Cells["P166:Q166"].Merge = true;
                            ws.Cells["P166:Q166"].Value = "";
                            ws.Cells["R166:S166"].Merge = true;
                            ws.Cells["R166:S166"].Value = "$  " + string.Format("{0:N}", (Math.Round(stack[x].NetAmountSum, 2)));
                            ws.Cells["T166:U166"].Merge = true;
                            ws.Cells["T166:U166"].Value = stack[x].AboveCount;
                            ws.Cells["V166:W166"].Merge = true;
                            ws.Cells["V166:W166"].Value = "";
                            ws.Cells["X166:X166"].Merge = true;
                            ws.Cells["X166:X166"].Value = "";
                            ws.Cells["Y166:Z166"].Merge = true;
                            ws.Cells["Y166:Z166"].Value = "$  " + string.Format("{0:N}", (Math.Round(stack[x].OrdervalueSum, 2)));
                            ws.Cells["AA166:AC166"].Merge = true;
                            ws.Cells["AA166:AC166"].Value = "";

                            ws.Cells["C167:I167"].Merge = true;
                            ws.Cells["C167:I167"].Value = "Eggs in KG ...................................................................................................................";
                            ws.Cells["J167:K167"].Merge = true;
                            ws.Cells["J167:K167"].Value = stack[x].EggOrderedQtySum;
                            ws.Cells["L167:M167"].Merge = true;
                            ws.Cells["L167:M167"].Value = stack[x].EggDeliveredQtySum;
                            ws.Cells["N167:O167"].Merge = true;
                            ws.Cells["N167:O167"].Value = stack[x].EggInvoiceQtySum;
                            ws.Cells["P167:Q167"].Merge = true;
                            ws.Cells["P167:Q167"].Value = "";
                            ws.Cells["R167:S167"].Merge = true;
                            ws.Cells["R167:S167"].Value = "$-";
                            ws.Cells["T167:U167"].Merge = true;
                            ws.Cells["T167:U167"].Value = stack[x].BelowCount;
                            ws.Cells["V167:W167"].Merge = true;
                            ws.Cells["V167:W167"].Value = "";
                            ws.Cells["X167:X167"].Merge = true;
                            ws.Cells["X167:X167"].Value = "";
                            ws.Cells["Y167:Z167"].Merge = true;
                            ws.Cells["Y167:Z167"].Value = "$-";
                            ws.Cells["AA167:AC167"].Merge = true;
                            ws.Cells["AA167:AC167"].Value = "";

                            ws.Cells["C168:I168"].Merge = true;
                            ws.Cells["C168:I168"].Value = "Sub total with Eggs in KG...........................................................................................";
                            ws.Cells["J168:K168"].Merge = true;
                            ws.Cells["J168:K168"].Value = Math.Round(stack[x].TotalOrderedQtySum, 3);
                            ws.Cells["L168:M168"].Merge = true;
                            ws.Cells["L168:M168"].Value = Math.Round(stack[x].TotalDeliveredQtySum, 3);
                            ws.Cells["N168:O168"].Merge = true;
                            ws.Cells["N168:O168"].Value = Math.Round(stack[x].TotalInvoiceQtySum, 3);
                            ws.Cells["P168:Q168"].Merge = true;
                            ws.Cells["P168:Q168"].Value = "";
                            ws.Cells["R168:S168"].Merge = true;
                            ws.Cells["R168:S168"].Value = "$  " + string.Format("{0:N}", (Math.Round(stack[x].NetAmountSum, 2)));
                            ws.Cells["T168:U168"].Merge = true;
                            ws.Cells["T168:U168"].Value = Math.Round(stack[x].CountPercent, 2) + "%";
                            ws.Cells["V168:W168"].Merge = true;
                            ws.Cells["V168:W168"].Value = "";
                            ws.Cells["X168:X168"].Merge = true;
                            ws.Cells["X168:X168"].Value = "";
                            ws.Cells["Y168:Z168"].Merge = true;
                            ws.Cells["Y168:Z168"].Value = "$  " + string.Format("{0:N}", (Math.Round(stack[x].OrdervalueSum, 2)));
                            ws.Cells["AA168:AC168"].Merge = true;
                            ws.Cells["AA168:AC168"].Value = "";
                            ws.Cells["A166:AC168"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A166:AC168"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A166:AC168"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                            #region Substitution
                            ws.Cells["A170:R170"].Merge = true;
                            ws.Cells["A170:R170"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                            ws.Cells["A171:R171"].Merge = true;
                            ws.Cells["A171:R171"].Value = "Substitutions";
                            ws.Cells["A171:R171"].Style.Font.Italic = true;
                            ws.Cells["A171:R171"].Style.Font.Color.SetColor(Color.DarkRed);
                            ws.Cells["A171:R171"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["A171:R171"].Style.Border.Right.Style = ExcelBorderStyle.Medium;


                            ws.Cells["A172:R173"].Style.WrapText = true;
                            ws.Cells["A172:R173"].Style.Font.Bold = true;
                            ws.Cells["A172:R173"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A172:R173"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A172:R173"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A172:R173"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A172:R173"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["A172:R173"].Style.Fill.BackgroundColor.SetColor(BlueHex);
                            ws.Cells["A172:R173"].Style.Font.Color.SetColor(Color.White);
                            ws.Cells["A172:R173"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                            ws.Cells["A172:A173"].Merge = true;
                            ws.Cells["A172:A173"].Value = "UNRS Code";
                            ws.Cells["A172:A173"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["A172:A173"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["B172:C173"].Merge = true;
                            ws.Cells["B172:C173"].Value = "Substituted With COMMODITY";
                            ws.Cells["B172:C173"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["B172:C173"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["D172:E173"].Merge = true;
                            ws.Cells["D172:E173"].Value = "Delivered Quantity";
                            ws.Cells["D172:E173"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["D172:E173"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["F172:F173"].Merge = true;
                            ws.Cells["F172:F173"].Value = "UNIT COST";
                            ws.Cells["F172:F173"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["F172:F173"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["G172:G173"].Merge = true;
                            ws.Cells["G172:G173"].Value = "UNRS Code";
                            ws.Cells["G172:G173"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["G172:G173"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["H172:I173"].Merge = true;
                            ws.Cells["H172:I173"].Value = "COMMODITY";
                            ws.Cells["H172:I173"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["H172:I173"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["J172:J173"].Merge = true;
                            ws.Cells["J172:J173"].Value = "Ordered";
                            ws.Cells["J172:J173"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["J172:J173"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["K172:L173"].Merge = true;
                            ws.Cells["K172:L173"].Value = "Accepted  Quantity";
                            ws.Cells["K172:L173"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["K172:L173"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["M172:M173"].Merge = true;
                            ws.Cells["M172:M173"].Value = "UNIT COST";
                            ws.Cells["M172:M173"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["M172:M173"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["N172:O173"].Merge = true;
                            ws.Cells["N172:O173"].Value = "Accepted Amount";
                            ws.Cells["N172:O173"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["N172:O173"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["P172:P173"].Merge = true;
                            ws.Cells["P172:P173"].Value = "APL Weight";
                            ws.Cells["P172:P173"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["P172:P173"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["Q172:R173"].Merge = true;
                            ws.Cells["Q172:R173"].Value = "DN #";
                            ws.Cells["Q172:R173"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["Q172:R173"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                            for (int K = 174; K < 194; K++)
                            {

                                ws.Cells["A" + K + ":A" + K + ""].Merge = true;
                                ws.Cells["A" + K + ":A" + K + ""].Value = "";
                                ws.Cells["A" + K + ":A" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["B" + K + ":C" + K + ""].Merge = true;
                                ws.Cells["B" + K + ":C" + K + ""].Value = "";
                                ws.Cells["D" + K + ":E" + K + ""].Merge = true;
                                ws.Cells["D" + K + ":E" + K + ""].Value = "";
                                ws.Cells["D" + K + ":E" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["F" + K + ":F" + K + ""].Merge = true;
                                ws.Cells["F" + K + ":F" + K + ""].Value = "";
                                ws.Cells["F" + K + ":F" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["G" + K + ":G" + K + ""].Merge = true;
                                ws.Cells["G" + K + ":G" + K + ""].Value = "";
                                ws.Cells["G" + K + ":G" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["H" + K + ":I" + K + ""].Merge = true;
                                ws.Cells["H" + K + ":I" + K + ""].Value = "";
                                ws.Cells["J" + K + ":J" + K + ""].Merge = true;
                                ws.Cells["J" + K + ":J" + K + ""].Value = "";
                                ws.Cells["J" + K + ":J" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["K" + K + ":L" + K + ""].Merge = true;
                                ws.Cells["K" + K + ":L" + K + ""].Value = "";
                                ws.Cells["K" + K + ":L" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["M" + K + ":M" + K + ""].Merge = true;
                                ws.Cells["M" + K + ":M" + K + ""].Value = "";
                                ws.Cells["M" + K + ":M" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["N" + K + ":O" + K + ""].Merge = true;
                                ws.Cells["N" + K + ":O" + K + ""].Value = "";
                                ws.Cells["N" + K + ":O" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["P" + K + ":P" + K + ""].Merge = true;
                                ws.Cells["P" + K + ":P" + K + ""].Value = "";
                                ws.Cells["P" + K + ":P" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["Q" + K + ":R" + K + ""].Merge = true;
                                ws.Cells["Q" + K + ":R" + K + ""].Value = "";
                                ws.Cells["A" + K + ":R" + K + ""].Style.Border.Right.Style = ExcelBorderStyle.Hair;
                                ws.Cells["A" + K + ":R" + K + ""].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                                ws.Cells["R" + K + ""].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            }

                            int tempSub = 0;
                            int x1 = 0;
                            int dcount = (174 + stack[x].SDeliveryList.Count());
                            int AboveSubtituteCount = 0;
                            if (stack[x].SDeliveryList.Count() != 0)
                            {
                                for (int K = 174; K < dcount; K++)
                                {
                                    if (stack[x].SDeliveryList[x1].SubstituteItemCode != 0)
                                    {
                                        tempSub = tempSub + 1;
                                    }
                                    ws.Cells["A" + K + ":A" + K + ""].Merge = true;
                                    ws.Cells["A" + K + ":A" + K + ""].Value = stack[x].SDeliveryList[x1].SubstituteItemCode;
                                    ws.Cells["A" + K + ":A" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells["B" + K + ":C" + K + ""].Merge = true;
                                    ws.Cells["B" + K + ":C" + K + ""].Value = stack[x].SDeliveryList[x1].SubstituteItemName;
                                    ws.Cells["D" + K + ":E" + K + ""].Merge = true;
                                    ws.Cells["D" + K + ":E" + K + ""].Value = Math.Round(stack[x].SDeliveryList[x1].DeliveredQty, 3);
                                    ws.Cells["D" + K + ":E" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    ws.Cells["F" + K + ":F" + K + ""].Merge = true;
                                    ws.Cells["F" + K + ":F" + K + ""].Value = "$" + Math.Round(stack[x].SDeliveryList[x1].SubstituteSectorPrice, 2);
                                    ws.Cells["F" + K + ":F" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells["G" + K + ":G" + K + ""].Merge = true;
                                    ws.Cells["G" + K + ":G" + K + ""].Value = stack[x].SDeliveryList[x1].UNCode;
                                    ws.Cells["G" + K + ":G" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells["H" + K + ":I" + K + ""].Merge = true;
                                    ws.Cells["H" + K + ":I" + K + ""].Value = stack[x].SDeliveryList[x1].Commodity;
                                    ws.Cells["J" + K + ":J" + K + ""].Merge = true;
                                    ws.Cells["J" + K + ":J" + K + ""].Value = Math.Round(stack[x].SDeliveryList[x1].OrderedQty, 3);
                                    ws.Cells["J" + K + ":J" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    ws.Cells["K" + K + ":L" + K + ""].Merge = true;
                                    ws.Cells["K" + K + ":L" + K + ""].Value = Math.Round(stack[x].SDeliveryList[x1].InvoiceQty, 3);
                                    ws.Cells["K" + K + ":L" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    ws.Cells["M" + K + ":M" + K + ""].Merge = true;
                                    ws.Cells["M" + K + ":M" + K + ""].Value = "$" + Math.Round(stack[x].SDeliveryList[x1].SectorPrice, 2);
                                    ws.Cells["M" + K + ":M" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells["N" + K + ":O" + K + ""].Merge = true;

                                    if (stack[x].SDeliveryList[x1].APLWeight >= 98) { AboveSubtituteCount = AboveSubtituteCount + 1; }

                                    ws.Cells["N" + K + ":O" + K + ""].Value = "$" + Math.Round(stack[x].SDeliveryList[x1].AcceptedAmt, 2);
                                    ws.Cells["N" + K + ":O" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    ws.Cells["P" + K + ":P" + K + ""].Merge = true;
                                    ws.Cells["P" + K + ":P" + K + ""].Value = Math.Round(stack[x].SDeliveryList[x1].APLWeight, 2) + "%";
                                    ws.Cells["P" + K + ":P" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    ws.Cells["Q" + K + ":R" + K + ""].Merge = true;
                                    ws.Cells["Q" + K + ":R" + K + ""].Value = stack[x].SDeliveryList[x1].DeliveryNoteName;
                                    ws.Cells["A" + K + ":R" + K + ""].Style.Border.Right.Style = ExcelBorderStyle.Hair;
                                    ws.Cells["A" + K + ":R" + K + ""].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                                    ws.Cells["R" + K + ""].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                                    x1 = x1 + 1;
                                }
                            }

                            ws.Cells["A194:A194"].Merge = true;
                            ws.Cells["A194:A194"].Value = x1;
                            ws.Cells["A194:A194"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["B194:C194"].Merge = true;
                            ws.Cells["B194:C194"].Value = "Sub Total";
                            ws.Cells["D194:E194"].Merge = true;
                            ws.Cells["D194:E194"].Value = string.Format("{0:N}", string.Format("{0:0.000}", (Math.Round(stack[x].SDeliveryQuantity, 3))));
                            ws.Cells["F194:F194"].Merge = true;
                            ws.Cells["F194:F194"].Value = "";
                            ws.Cells["G194:G194"].Merge = true;
                            ws.Cells["G194:G194"].Value = "";
                            ws.Cells["H194:I194"].Merge = true;
                            ws.Cells["H194:I194"].Value = "";
                            ws.Cells["J194:J194"].Merge = true;
                            ws.Cells["J194:J194"].Value = string.Format("{0:N}", string.Format("{0:0.000}", (Math.Round(stack[x].SOrderedQuantity, 3))));
                            ws.Cells["K194:L194"].Merge = true;
                            ws.Cells["K194:L194"].Value = string.Format("{0:N}", string.Format("{0:0.000}", (Math.Round(stack[x].SAcceptedQuantity, 3))));
                            ws.Cells["M194:M194"].Merge = true;
                            ws.Cells["M194:M194"].Value = "";
                            ws.Cells["N194:O194"].Merge = true;
                            ws.Cells["N194:O194"].Value = "$  " + string.Format("{0:N}", (Math.Round(stack[x].SAcceptedamt, 2)));
                            ws.Cells["P194:P194"].Merge = true;
                            ws.Cells["P194:P194"].Value = "";
                            ws.Cells["Q194:R194"].Merge = true;
                            ws.Cells["Q194:R194"].Value = "";
                            ws.Cells["A194:R194"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A194:R194"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A194:R194"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A194:R194"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["A194:R194"].Style.Fill.BackgroundColor.SetColor(BlueHex);
                            ws.Cells["A194:R194"].Style.Font.Color.SetColor(Color.White);
                            #endregion

                            #region Replacement

                            ws.Cells["A195:R195"].Merge = true;
                            ws.Cells["A195:R195"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                            ws.Cells["A196:R196"].Merge = true;
                            ws.Cells["A196:R196"].Value = "Replacements";
                            ws.Cells["A196:R196"].Style.Font.Italic = true;
                            ws.Cells["A196:R196"].Style.Font.Color.SetColor(Color.DarkRed);
                            ws.Cells["A196:R196"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["A196:R196"].Style.Border.Right.Style = ExcelBorderStyle.Medium;


                            ws.Cells["A197:R198"].Style.WrapText = true;
                            ws.Cells["A197:R198"].Style.Font.Bold = true;
                            ws.Cells["A197:R198"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A197:R198"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A197:R198"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A197:R198"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A197:R198"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["A197:R198"].Style.Fill.BackgroundColor.SetColor(BlueHex);
                            ws.Cells["A197:R198"].Style.Font.Color.SetColor(Color.White);
                            ws.Cells["A197:R198"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                            ws.Cells["A197:A198"].Merge = true;
                            ws.Cells["A197:A198"].Value = "UNRS Code";
                            ws.Cells["A197:A198"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["A197:A198"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["B197:C198"].Merge = true;
                            ws.Cells["B197:C198"].Value = "Replacements With COMMODITY";
                            ws.Cells["B197:C198"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["B197:C198"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["D197:E198"].Merge = true;
                            ws.Cells["D197:E198"].Value = "Delivered Quantity";
                            ws.Cells["D197:E198"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["D197:E198"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["F197:F198"].Merge = true;
                            ws.Cells["F197:F198"].Value = "UNIT COST";
                            ws.Cells["F197:F198"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["F197:F198"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["G197:G198"].Merge = true;
                            ws.Cells["G197:G198"].Value = "UNRS Code";
                            ws.Cells["G197:G198"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["G197:G198"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["H197:I198"].Merge = true;
                            ws.Cells["H197:I198"].Value = "COMMODITY";
                            ws.Cells["H197:I198"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["H197:I198"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["J197:J198"].Merge = true;
                            ws.Cells["J197:J198"].Value = "Ordered";
                            ws.Cells["J197:J198"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["J197:J198"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["K197:L198"].Merge = true;
                            ws.Cells["K197:L198"].Value = "Accepted  Quantity";
                            ws.Cells["K197:L198"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["K197:L198"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["M197:M198"].Merge = true;
                            ws.Cells["M197:M198"].Value = "UNIT COST";
                            ws.Cells["M197:M198"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["M197:M198"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["N197:O198"].Merge = true;
                            ws.Cells["N197:O198"].Value = "Accepted Amount";
                            ws.Cells["N197:O198"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["N197:O198"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["P197:P198"].Merge = true;
                            ws.Cells["P197:P198"].Value = "APL Weight";
                            ws.Cells["P197:P198"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["P197:P198"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["Q197:R198"].Merge = true;
                            ws.Cells["Q197:R198"].Value = "DN #";
                            ws.Cells["Q197:R198"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["Q197:R198"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                            for (int K = 199; K < 219; K++)
                            {
                                ws.Cells["A" + K + ":A" + K + ""].Merge = true;
                                ws.Cells["A" + K + ":A" + K + ""].Value = "";
                                ws.Cells["A" + K + ":A" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["B" + K + ":C" + K + ""].Merge = true;
                                ws.Cells["B" + K + ":C" + K + ""].Value = "";
                                ws.Cells["D" + K + ":E" + K + ""].Merge = true;
                                ws.Cells["D" + K + ":E" + K + ""].Value = "";
                                ws.Cells["D" + K + ":E" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["F" + K + ":F" + K + ""].Merge = true;
                                ws.Cells["F" + K + ":F" + K + ""].Value = "";
                                ws.Cells["F" + K + ":F" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["G" + K + ":G" + K + ""].Merge = true;
                                ws.Cells["G" + K + ":G" + K + ""].Value = "";
                                ws.Cells["G" + K + ":G" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["H" + K + ":I" + K + ""].Merge = true;
                                ws.Cells["H" + K + ":I" + K + ""].Value = "";
                                ws.Cells["J" + K + ":J" + K + ""].Merge = true;
                                ws.Cells["J" + K + ":J" + K + ""].Value = "";
                                ws.Cells["J" + K + ":J" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["K" + K + ":L" + K + ""].Merge = true;
                                ws.Cells["K" + K + ":L" + K + ""].Value = "";
                                ws.Cells["K" + K + ":L" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["M" + K + ":M" + K + ""].Merge = true;
                                ws.Cells["M" + K + ":M" + K + ""].Value = "";
                                ws.Cells["M" + K + ":M" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["N" + K + ":O" + K + ""].Merge = true;
                                ws.Cells["N" + K + ":O" + K + ""].Value = "";
                                ws.Cells["N" + K + ":O" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["P" + K + ":P" + K + ""].Merge = true;
                                ws.Cells["P" + K + ":P" + K + ""].Value = "";
                                ws.Cells["P" + K + ":P" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["Q" + K + ":R" + K + ""].Merge = true;
                                ws.Cells["Q" + K + ":R" + K + ""].Value = "";
                                ws.Cells["A" + K + ":R" + K + ""].Style.Border.Right.Style = ExcelBorderStyle.Hair;
                                ws.Cells["A" + K + ":R" + K + ""].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                                ws.Cells["R" + K + ""].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            }

                            int x2 = 0;
                            int Rcount = (199 + stack[x].RDeliveryList.Count());
                            //int AboveSubtituteCount = 0;
                            if (stack[x].RDeliveryList.Count() != 0)
                            {
                                for (int K = 199; K < Rcount; K++)
                                {
                                    ws.Cells["A" + K + ":A" + K + ""].Merge = true;
                                    ws.Cells["A" + K + ":A" + K + ""].Value = stack[x].RDeliveryList[x2].SubstituteItemCode;
                                    ws.Cells["A" + K + ":A" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells["B" + K + ":C" + K + ""].Merge = true;
                                    ws.Cells["B" + K + ":C" + K + ""].Value = stack[x].RDeliveryList[x2].SubstituteItemName;
                                    ws.Cells["D" + K + ":E" + K + ""].Merge = true;
                                    ws.Cells["D" + K + ":E" + K + ""].Value = Math.Round(stack[x].RDeliveryList[x2].DeliveredQty, 3);
                                    ws.Cells["D" + K + ":E" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    ws.Cells["F" + K + ":F" + K + ""].Merge = true;
                                    ws.Cells["F" + K + ":F" + K + ""].Value = "$" + Math.Round(stack[x].RDeliveryList[x2].SubstituteSectorPrice, 2);
                                    ws.Cells["F" + K + ":F" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells["G" + K + ":G" + K + ""].Merge = true;
                                    ws.Cells["G" + K + ":G" + K + ""].Value = stack[x].RDeliveryList[x2].UNCode;
                                    ws.Cells["G" + K + ":G" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells["H" + K + ":I" + K + ""].Merge = true;
                                    ws.Cells["H" + K + ":I" + K + ""].Value = stack[x].RDeliveryList[x2].Commodity;
                                    ws.Cells["J" + K + ":J" + K + ""].Merge = true;
                                    ws.Cells["J" + K + ":J" + K + ""].Value = Math.Round(stack[x].RDeliveryList[x2].OrderedQty, 3);
                                    ws.Cells["J" + K + ":J" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    ws.Cells["K" + K + ":L" + K + ""].Merge = true;
                                    ws.Cells["K" + K + ":L" + K + ""].Value = Math.Round(stack[x].RDeliveryList[x2].InvoiceQty, 3);
                                    ws.Cells["K" + K + ":L" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    ws.Cells["M" + K + ":M" + K + ""].Merge = true;
                                    ws.Cells["M" + K + ":M" + K + ""].Value = "$" + Math.Round(stack[x].RDeliveryList[x2].SectorPrice, 2);
                                    ws.Cells["M" + K + ":M" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells["N" + K + ":O" + K + ""].Merge = true;

                                    if (stack[x].RDeliveryList[x2].APLWeight >= 98) { AboveSubtituteCount = AboveSubtituteCount + 1; }

                                    ws.Cells["N" + K + ":O" + K + ""].Value = "$" + Math.Round(stack[x].RDeliveryList[x2].AcceptedAmt, 2);
                                    ws.Cells["N" + K + ":O" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    ws.Cells["P" + K + ":P" + K + ""].Merge = true;
                                    ws.Cells["P" + K + ":P" + K + ""].Value = Math.Round(stack[x].RDeliveryList[x2].APLWeight, 2) + "%";
                                    ws.Cells["P" + K + ":P" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    ws.Cells["Q" + K + ":R" + K + ""].Merge = true;
                                    ws.Cells["Q" + K + ":R" + K + ""].Value = stack[x].RDeliveryList[x2].DeliveryNoteName;
                                    ws.Cells["A" + K + ":R" + K + ""].Style.Border.Right.Style = ExcelBorderStyle.Hair;
                                    ws.Cells["A" + K + ":R" + K + ""].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                                    ws.Cells["R" + K + ""].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                                    x2 = x2 + 1;
                                }
                            }

                            ws.Cells["A219:A219"].Merge = true;
                            ws.Cells["A219:A219"].Value = x2;
                            ws.Cells["A219:A219"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["B219:C219"].Merge = true;
                            ws.Cells["B219:C219"].Value = "Sub Total";
                            ws.Cells["D219:E219"].Merge = true;
                            ws.Cells["D219:E219"].Value = string.Format("{0:N}", string.Format("{0:0.000}", (Math.Round(stack[x].RDeliveryQuantity, 3))));
                            ws.Cells["F219:F219"].Merge = true;
                            ws.Cells["F219:F219"].Value = "";
                            ws.Cells["G219:G219"].Merge = true;
                            ws.Cells["G219:G219"].Value = "";
                            ws.Cells["H219:I219"].Merge = true;
                            ws.Cells["H219:I219"].Value = "";
                            ws.Cells["J219:J219"].Merge = true;
                            ws.Cells["J219:J219"].Value = string.Format("{0:N}", string.Format("{0:0.000}", (Math.Round(stack[x].ROrderedQuantity, 3))));
                            ws.Cells["K219:L219"].Merge = true;
                            ws.Cells["K219:L219"].Value = string.Format("{0:N}", string.Format("{0:0.000}", (Math.Round(stack[x].RAcceptedQuantity, 3))));
                            ws.Cells["M219:M219"].Merge = true;
                            ws.Cells["M219:M219"].Value = "";
                            ws.Cells["N219:O219"].Merge = true;
                            ws.Cells["N219:O219"].Value = "$  " + string.Format("{0:N}", (Math.Round(stack[x].RAcceptedamt, 2)));
                            ws.Cells["P219:P219"].Merge = true;
                            ws.Cells["P219:P219"].Value = "";
                            ws.Cells["Q219:R219"].Merge = true;
                            ws.Cells["Q219:R219"].Value = "";
                            ws.Cells["A219:R219"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A219:R219"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A219:R219"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A219:R219"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["A219:R219"].Style.Fill.BackgroundColor.SetColor(BlueHex);
                            ws.Cells["A219:R219"].Style.Font.Color.SetColor(Color.White);


                            #endregion

                            #region Summary
                            ws.Cells["A222:D222"].Merge = true;
                            ws.Cells["A222:D222"].Value = "Number of Days Delay";
                            ws.Cells["E222:F222"].Merge = true;
                            ws.Cells["E222:F222"].Value = stack[x].TotalDays;
                            ws.Cells["A223:D223"].Merge = true;
                            ws.Cells["A223:D223"].Value = "Total Line Items ordered";
                            ws.Cells["E223:F223"].Merge = true;
                            ws.Cells["E223:F223"].Value = temp;
                            ws.Cells["A224:D224"].Merge = true;
                            ws.Cells["A224:D224"].Value = "Total Line Items Received >= 98";
                            ws.Cells["E224:F224"].Merge = true;
                            ws.Cells["E224:F224"].Value = (stack[x].AboveCount + AboveSubtituteCount);
                            ws.Cells["A225:D225"].Merge = true;
                            ws.Cells["A225:D225"].Value = "Order Quantity";
                            ws.Cells["E225:F225"].Merge = true;
                            ws.Cells["E225:F225"].Value = stack[x].TotalOrderedQtySum;
                            ws.Cells["A226:D226"].Merge = true;
                            ws.Cells["A226:D226"].Value = "Delivered  Quantity";
                            ws.Cells["E226:F226"].Merge = true;
                            //ws.Cells["E226:F226"].Value = stack[x].TotalDeliveredQtySum + stack[x].SDeliveryQuantity + stack[x].RDeliveryQuantity;
                            ws.Cells["E226:F226"].Value = stack[x].TotalDeliveredQtySum;
                            ws.Cells["A227:D227"].Merge = true;
                            ws.Cells["A227:D227"].Value = "Accepted Quantity";
                            ws.Cells["E227:F227"].Merge = true;
                            //ws.Cells["E227:F227"].Value = stack[x].TotalInvoiceQtySum + stack[x].SAcceptedQuantity + stack[x].RAcceptedQuantity;
                            ws.Cells["E227:F227"].Value = stack[x].TotalAcceptedQtySum;
                            ws.Cells["A228:D228"].Merge = true;
                            ws.Cells["A228:D228"].Value = "Number Line Items Ordered";
                            ws.Cells["E228:F228"].Merge = true;
                            ws.Cells["E228:F228"].Value = temp;
                            ws.Cells["A229:D229"].Merge = true;
                            ws.Cells["A229:D229"].Value = "Number of Authorized substitutions";
                            ws.Cells["E229:F229"].Merge = true;
                            ws.Cells["E229:F229"].Value = stack[x].SubstituteCount;

                            ws.Cells["I222:I225"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["I222:L222"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                            ws.Cells["L222:L225"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["I225:L225"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                            ws.Cells["A222:F229"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            ws.Cells["A222:F229"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["A222:F222"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                            ws.Cells["F222:F229"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A229:F229"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                            ws.Cells["I222:J222"].Merge = true;
                            ws.Cells["I222:J222"].Value = "Applicable CMR";
                            ws.Cells["K222:L222"].Merge = true;
                            ws.Cells["K222:L222"].Value = stack[x].AuthorizedCMR;
                            ws.Cells["K222:L222"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            ws.Cells["I223:J223"].Merge = true;
                            ws.Cells["I223:J223"].Value = "Order CMR";
                            ws.Cells["K223:L223"].Merge = true;

                            decimal OrderCMR = Math.Round((stack[x].OrdervalueSum / stack[x].ManDays), 2);

                            ws.Cells["K223:L223"].Value = OrderCMR;
                            ws.Cells["K223:L223"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            ws.Cells["I224:J224"].Merge = true;
                            ws.Cells["I224:J224"].Value = "Accepted CMR ";
                            ws.Cells["K224:L224"].Merge = true;

                            decimal AcceptedCMR = Math.Round((((stack[x].NetAmountSum) + stack[x].SAcceptedamt + stack[x].RAcceptedamt) / stack[x].ManDays), 2);

                            ws.Cells["K224:L224"].Value = AcceptedCMR;
                            ws.Cells["K224:L224"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            ws.Cells["I225:J225"].Merge = true;
                            ws.Cells["I225:J225"].Value = "% Of CMR Utilized";
                            ws.Cells["K225:L225"].Merge = true;

                            Decimal CmRUtilised = 0;
                            if ((AcceptedCMR != 0) && (stack[x].AuthorizedCMR != 0))
                            {
                                CmRUtilised = Math.Round((AcceptedCMR / OrderCMR) * 100, 2);
                            }


                            ws.Cells["K225:L225"].Value = CmRUtilised + "%";
                            ws.Cells["K225:L225"].Style.Font.Bold = true;
                            ws.Cells["K225:L225"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            ws.Cells["I222:L225"].Style.Border.Diagonal.Style = ExcelBorderStyle.Medium;
                            ws.Cells["I222:L225"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["I222:L225"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                            ws.Cells["I227:M227"].Merge = true;
                            ws.Cells["I227:M227"].Value = "Food order value for APL Purposes";

                            ws.Cells["N227:O227"].Merge = true;
                            ws.Cells["N227:O227"].Value = "$  " + string.Format("{0:N}", stack[x].OrdervalueSum);
                            ws.Cells["N227:O227"].Style.Numberformat.Format = "#,##0.00";
                            ws.Cells["N227:O227"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            ws.Cells["I227:O227"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["I227:O227"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                            ws.Cells["I227:O227"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                            ws.Cells["I227:O227"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            #endregion
                            #region Performance
                            ws.Cells["A231:T231"].Merge = true;
                            ws.Cells["A231:T231"].Value = "Performance Details";
                            ws.Cells["A231:T231"].Style.Font.Italic = true;
                            ws.Cells["A231:T231"].Style.Font.Bold = true;
                            ws.Cells["A231:T231"].Style.Font.Color.SetColor(Color.DarkRed);
                            ws.Cells["A231:T231"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                            ws.Cells["A232:L232"].Merge = true;
                            ws.Cells["A232:L232"].Value = "Amount at risk in terms of a percentage of a Weekly invoice (Rations) 'C'  for each Delivery Point  15%";
                            ws.Cells["A232:L232"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            ws.Cells["M232:T232"].Merge = true;

                            //decimal deliveryPoint = Math.Round(((stack[x].SAcceptedamt + stack[x].RAcceptedamt + stack[x].NetAmountSum) / 100) * 15, 2);
                            decimal deliveryPoint = Math.Round(((stack[x].NetRationAmount) / 100) * 15, 2);

                            ws.Cells["M232:T232"].Value = "$  " + string.Format("{0:N}", deliveryPoint);
                            ws.Cells["M232:T232"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                            ws.Cells["A233:D233"].Merge = true;
                            ws.Cells["A233:D233"].Value = "Performance Details";

                            ws.Cells["E233:E233"].Merge = true;
                            ws.Cells["E233:E233"].Value = "APL Code";

                            ws.Cells["F233:F233"].Merge = true;
                            ws.Cells["F233:F233"].Value = "Target %";

                            ws.Cells["G233:H233"].Merge = true;
                            ws.Cells["G233:H233"].Value = "Acceptable";
                            ws.Cells["I233:L233"].Merge = true;
                            ws.Cells["I233:L233"].Value = "Band (level of performance)";
                            ws.Cells["M233:P233"].Merge = true;
                            ws.Cells["M233:P233"].Value = "Service Level Credit - % of invoice";
                            ws.Cells["Q233:R233"].Merge = true;
                            ws.Cells["Q233:R233"].Value = "Performance";
                            ws.Cells["S233:T233"].Merge = true;
                            ws.Cells["S233:T233"].Value = "Deduction";
                            ws.Cells["A233:T233"].Style.Font.Bold = true;

                            ws.Cells["A234:D239"].Merge = true;
                            ws.Cells["A234:D239"].Value = "1. Conformity to Delivery Schedule";
                            ws.Cells["A234:D239"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["A234:D239"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["A234:D239"].Style.WrapText = true;

                            ws.Cells["E234:E239"].Merge = true;
                            ws.Cells["E234:E239"].Value = "S";

                            ws.Cells["F234:F239"].Merge = true;
                            ws.Cells["F234:F239"].Style.WrapText = true;
                            ws.Cells["F234:F239"].Value = "On time delivery";

                            ws.Cells["G234:H239"].Merge = true;
                            ws.Cells["G234:H239"].Value = "On time";
                            ws.Cells["I234:J236"].Merge = true;
                            ws.Cells["I234:J236"].Value = "1 day delay";
                            ws.Cells["I237:J239"].Style.Border.Top.Style = ExcelBorderStyle.Hair;
                            ws.Cells["I237:J239"].Merge = true;
                            ws.Cells["I237:J239"].Value = "2 day delay";
                            ws.Cells["K234:L236"].Merge = true;
                            ws.Cells["K234:L236"].Value = "40.00%";

                            ws.Cells["K237:L239"].Merge = true;
                            ws.Cells["K237:L239"].Value = "100.00%";
                            ws.Cells["M234:N236"].Merge = true;
                            ws.Cells["M234:N236"].Value = "1.20%";

                            ws.Cells["M237:N239"].Merge = true;
                            ws.Cells["M237:N239"].Value = "3.00%";
                            ws.Cells["O234:P236"].Merge = true;

                            ws.Cells["O237:P239"].Merge = true;
                            ws.Cells["Q234:R239"].Merge = true;
                            //ws.Cells["Q234:R239"].Value = stack[x].DeliveryPerformance + "%";                          //Performance
                            ws.Cells["Q234:R239"].Value = stack[x].DeliveryPerformance;                          //Performance
                            //ws.Cells["Q234:R239"].Value = string.Format("{0:N}", string.Format("{0:0.00}", (Math.Round(stack[x].DeliveryPerformance, 2))) + "%");
                            ws.Cells["S234:T239"].Merge = true;
                            ws.Cells["S234:T239"].Value = "- $  " + string.Format("{0:N}", stack[x].DeliveryDeduction);                          //Day Deduction


                            ws.Cells["A240:D245"].Merge = true;
                            ws.Cells["A240:D245"].Value = "'2. Conformity to Order by Line Items Number of line items ordered is delivered (including authorized substitutions)' ";
                            ws.Cells["A240:D245"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["A240:D245"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["A240:D245"].Style.WrapText = true;

                            ws.Cells["E240:E245"].Merge = true;
                            ws.Cells["E240:E245"].Value = "L";

                            ws.Cells["F240:F245"].Merge = true;
                            ws.Cells["F240:F245"].Value = "100%";
                            ws.Cells["G240:H245"].Merge = true;
                            ws.Cells["G240:H245"].Value = "98%";
                            ws.Cells["I240:J242"].Merge = true;
                            ws.Cells["I240:J242"].Value = "95% - <98%";
                            ws.Cells["I243:J245"].Merge = true;
                            ws.Cells["I243:J245"].Value = "<92% - <95%";
                            ws.Cells["K240:L242"].Merge = true;
                            ws.Cells["K240:L242"].Value = "40.00%";
                            ws.Cells["K243:L245"].Merge = true;
                            ws.Cells["K243:L245"].Value = "100.00%";
                            ws.Cells["M240:N242"].Merge = true;
                            ws.Cells["M240:N242"].Value = "1.20%";
                            ws.Cells["M243:N245"].Merge = true;
                            ws.Cells["M243:N245"].Value = "3.00%";

                            ws.Cells["O240:P242"].Merge = true;
                            ws.Cells["O243:P245"].Merge = true;
                            ws.Cells["Q240:R245"].Merge = true;
                            //ws.Cells["Q240:R245"].Value = Math.Round(stack[x].LineItemPerformance, 2) + "%";                        //Performance
                            ws.Cells["Q240:R245"].Value = string.Format("{0:N}", string.Format("{0:0.00}", (Math.Round(stack[x].LineItemPerformance, 2))) + "%");
                            ws.Cells["S240:T245"].Merge = true;
                            ws.Cells["S240:T245"].Value = "- $  " + string.Format("{0:N}", stack[x].LineItemDeduction);                         //Day Deduction


                            ws.Cells["A246:D251"].Merge = true;
                            ws.Cells["A246:D251"].Value = "3. Conformity to Orders by weight:  Quantity kg/ltr/each Quantity of food order in Kg/Ltr is delivered (including authorized substitutions)";
                            ws.Cells["A246:D251"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["A246:D251"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["A246:D251"].Style.WrapText = true;

                            ws.Cells["E246:E251"].Merge = true;
                            ws.Cells["E246:E251"].Value = "Q";

                            ws.Cells["F246:F251"].Merge = true;
                            ws.Cells["F246:F251"].Value = "100%";
                            ws.Cells["G246:H251"].Merge = true;
                            ws.Cells["G246:H251"].Value = "95%";
                            ws.Cells["I246:J248"].Merge = true;
                            ws.Cells["I246:J248"].Value = "92% - <95%";
                            ws.Cells["I249:J251"].Merge = true;
                            ws.Cells["I249:J251"].Value = "<90% - <92%";
                            ws.Cells["K246:L248"].Merge = true;
                            ws.Cells["K246:L248"].Value = "40.00%";
                            ws.Cells["K249:L251"].Merge = true;
                            ws.Cells["K249:L251"].Value = "100.00%";
                            ws.Cells["M246:N248"].Merge = true;
                            ws.Cells["M246:N248"].Value = "1.80%";
                            ws.Cells["M249:N251"].Merge = true;
                            ws.Cells["M249:N251"].Value = "4.50%";
                            ws.Cells["O246:P248"].Merge = true;
                            ws.Cells["O249:P251"].Merge = true;
                            ws.Cells["Q246:R251"].Merge = true;
                            //ws.Cells["Q246:R251"].Value = stack[x].OrderWightPerformance + "%";                       //Performance
                            ws.Cells["Q246:R251"].Value = string.Format("{0:N}", string.Format("{0:0.00}", (Math.Round(stack[x].OrderWightPerformance, 2))) + "%");
                            ws.Cells["S246:T251"].Merge = true;
                            ws.Cells["S246:T251"].Value = "- $  " + string.Format("{0:N}", stack[x].OrderWightDeduction);                       //Day Deduction

                            ws.Cells["A252:D257"].Merge = true;
                            ws.Cells["A252:D257"].Value = "4. Food Order Compliance-Availability :  Number of  authorized substitutions";
                            ws.Cells["A252:D257"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["A252:D257"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["A252:D257"].Style.WrapText = true;

                            ws.Cells["E252:E257"].Merge = true;
                            ws.Cells["E252:E257"].Value = "A";

                            ws.Cells["F252:F257"].Merge = true;
                            ws.Cells["F252:F257"].Value = "0%";
                            ws.Cells["G252:H257"].Merge = true;
                            ws.Cells["G252:H257"].Value = "3%";
                            ws.Cells["I252:J254"].Merge = true;
                            ws.Cells["I252:J254"].Value = "3% - <4%";
                            ws.Cells["I255:J257"].Merge = true;
                            ws.Cells["I255:J257"].Value = "4% - 5% +";
                            ws.Cells["K252:L254"].Merge = true;
                            ws.Cells["K252:L254"].Value = "40.00%";
                            ws.Cells["K255:L257"].Merge = true;
                            ws.Cells["K255:L257"].Value = "100.00%";
                            ws.Cells["M252:N254"].Merge = true;
                            ws.Cells["M252:N254"].Value = "1.80%";
                            ws.Cells["M255:N257"].Merge = true;
                            ws.Cells["M255:N257"].Value = "4.50%";//
                            ws.Cells["O252:P254"].Merge = true;
                            ws.Cells["O255:P257"].Merge = true;
                            ws.Cells["Q252:R257"].Merge = true;
                            //ws.Cells["Q252:R257"].Value = stack[x].SubtitutionPerformance + "%";                         //Performance
                            ws.Cells["Q252:R257"].Value = string.Format("{0:N}", string.Format("{0:0.00}", (Math.Round(stack[x].SubtitutionPerformance, 2))) + "%");
                            ws.Cells["S252:T257"].Merge = true;
                            ws.Cells["S252:T257"].Value = "- $  " + string.Format("{0:N}", stack[x].SubtitutionDeduction);                           //Day Deduction


                            ws.Cells["A258:T260"].Merge = true;
                            ws.Cells["A258:T260"].Value = "Methodology:  Verification shall be determined by the UN in its sole discretion.  The service credit shall be computed according to the following formula:  Service Level Credit = A x B x C ( A = Allocation (% of at risk amount) B =% of allocation C = At risk amount )";
                            ws.Cells["A258:T260"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["A258:T260"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["A258:T260"].Style.WrapText = true;

                            ws.Cells["A232:T260"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A232:T260"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A232:T260"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A232:T232"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["A232:T232"].Style.Fill.BackgroundColor.SetColor(BlueHex);
                            ws.Cells["A232:T232"].Style.Font.Color.SetColor(Color.White);

                            ws.Cells["A233:T260"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["A233:T260"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                            ws.Cells["I237:P237"].Style.Border.Top.Style = ExcelBorderStyle.Hair;
                            ws.Cells["I243:P243"].Style.Border.Top.Style = ExcelBorderStyle.Hair;
                            ws.Cells["I249:P249"].Style.Border.Top.Style = ExcelBorderStyle.Hair;
                            ws.Cells["I255:P255"].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                            ws.Cells["J234:J257"].Style.Border.Right.Style = ExcelBorderStyle.Hair;
                            ws.Cells["N234:N257"].Style.Border.Right.Style = ExcelBorderStyle.Hair;

                            ws.Cells["A262"].Value = "NPA01 - Quantity under-delivered not relevant";
                            ws.Cells["A263"].Value = "NPA02 - Quantity ordered does not meet packing size requirements";
                            ws.Cells["A264"].Value = "NPA03 - Unreasonable denied access to delivery points";
                            ws.Cells["A265"].Value = "NPA04 - UN proposed substitution";
                            ws.Cells["A266"].Value = "NPA05 – Other";
                            ws.Cells["A267"].Value = "AS- Authorized Substitution";
                            ws.Cells["A268"].Value = "AR – Authorized Replacement ";
                            #endregion

                            ws.Cells["Q262:R262"].Merge = true;
                            ws.Cells["Q262:R262"].Value = "APL Deductions";
                            ws.Cells["S262:T262"].Merge = true;

                            decimal AplDetTotal = Math.Round((stack[x].DeliveryDeduction + stack[x].LineItemDeduction + stack[x].OrderWightDeduction + stack[x].SubtitutionDeduction), 2);

                            ws.Cells["S262:T262"].Value = "- $  " + string.Format("{0:N}", AplDetTotal);
                            ws.Cells["S262:T262"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            ws.Cells["Q262:T262"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["Q262:T262"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                            ws.Cells["Q262:T262"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                            ws.Cells["Q262:T262"].Style.Border.Left.Style = ExcelBorderStyle.Medium;

                            //decimal AmtAccept = Math.Round((stack[x].NetAmountSum + stack[x].SAcceptedamt + stack[x].RAcceptedamt), 2);
                            decimal tropstgh = Math.Round((0 * AplDetTotal), 2);
                            decimal OtherCreditNote = Math.Round((0 * AplDetTotal), 2);
                            //decimal Weeklywise = Math.Round((0 * AplDetTotal), 2);
                            //decimal NetRationAmt = Math.Round((tropstgh + OtherCreditNote + Weeklywise + AmtAccept), 2);

                            //decimal Weeklywise = 0;
                            ////Weeklywise=Math.Round((Convert.ToDecimal(0.35 / 100) * AmtAccept), 2);
                            //decimal NetRationAmt = Math.Round((AmtAccept - Weeklywise), 2);

                            //decimal AccAmtTransportation = 0;
                            //AccAmtTransportation = Math.Round(Convert.ToDecimal(0.31 / 100) * (stack[x].TotalInvoiceQtySum + stack[x].SAcceptedQuantity + stack[x].RAcceptedQuantity), 2);


                            //decimal confirmityCMR = 0;
                            //if (stack[x].AcceptedCMR > stack[x].AuthorizedCMR)
                            //    confirmityCMR = Math.Round((stack[x].AcceptedCMR - stack[x].AuthorizedCMR) * stack[x].Strength * 7, 2);

                            decimal AplDetect = Math.Round(@AplDetTotal, 2);

                            //decimal TotalInvoice = Math.Round((NetRationAmt - AplDetect - confirmityCMR + AccAmtTransportation), 2);
                            decimal TotalInvoice = Math.Round((stack[x].NetRationAmount - AplDetect - stack[x].confirmityCMR + stack[x].AcceptedTransportCost), 2);

                            ws.Cells["M266:O266"].Merge = true;
                            ws.Cells["M266:O266"].Value = " Amount  Accepted";
                            ws.Cells["P266:Q266"].Merge = true;
                            ws.Cells["R266:T266"].Merge = true;
                            //ws.Cells["R266:T266"].Value = "$  " + string.Format("{0:N}", AmtAccept);
                            ws.Cells["R266:T266"].Value = "$  " + string.Format("{0:N}", stack[x].AmountAccepted);
                            ws.Cells["R266:T266"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            ws.Cells["M267:O267"].Merge = true;
                            ws.Cells["M267:O267"].Value = "Confirmity to CMR";
                            ws.Cells["P267:Q267"].Merge = true;
                            ws.Cells["R267:T267"].Merge = true;
                            //ws.Cells["R267:T267"].Value = "- $  " + string.Format("{0:N}", confirmityCMR);
                            ws.Cells["R267:T267"].Value = "- $  " + string.Format("{0:N}", stack[x].confirmityCMR);
                            ws.Cells["R267:T267"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            ws.Cells["M268:O268"].Merge = true;
                            ws.Cells["M268:O268"].Value = "Troops Strength";
                            ws.Cells["P268:Q268"].Merge = true;
                            ws.Cells["R268:T268"].Merge = true;
                            ws.Cells["R268:T268"].Value = "$  " + string.Format("{0:N}", tropstgh);
                            ws.Cells["R268:T268"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            ws.Cells["M269:O269"].Merge = true;
                            ws.Cells["M269:O269"].Value = "Other Credit Notes";
                            ws.Cells["P269:Q269"].Merge = true;
                            ws.Cells["R269:T269"].Merge = true;
                            ws.Cells["R269:T269"].Value = "$  " + string.Format("{0:N}", OtherCreditNote);
                            ws.Cells["R269:T269"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            ws.Cells["M270:O270"].Merge = true;
                            ws.Cells["M270:O270"].Value = "Weekly Invoice";
                            ws.Cells["P270:Q270"].Merge = true;
                            ws.Cells["P270:Q270"].Value = "-0.35%";
                            ws.Cells["R270:T270"].Merge = true;
                            //ws.Cells["R270:T270"].Value = "- $  " + string.Format("{0:N}", Weeklywise);
                            ws.Cells["R270:T270"].Value = "- $  " + string.Format("{0:N}", stack[x].Weeklyinvoicediscount);
                            ws.Cells["R270:T270"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            ws.Cells["M271:O271"].Merge = true;
                            ws.Cells["M271:O271"].Value = "Net amount for Rations";
                            ws.Cells["P271:Q271"].Merge = true;
                            ws.Cells["R271:T271"].Merge = true;
                            //ws.Cells["R271:T271"].Value = "$  " + string.Format("{0:N}", NetRationAmt);
                            ws.Cells["R271:T271"].Value = "$  " + string.Format("{0:N}", stack[x].NetRationAmount);
                            ws.Cells["R271:T271"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            ws.Cells["M272:O272"].Merge = true;
                            ws.Cells["M272:O272"].Value = "Accepted Amount Transportation";
                            ws.Cells["P272:Q272"].Merge = true;
                            ws.Cells["P272:Q272"].Value = stack[x].RatePerKg;
                            ws.Cells["R272:T272"].Merge = true;
                            //ws.Cells["R272:T272"].Value = "$  " + string.Format("{0:N}", AccAmtTransportation);
                            ws.Cells["R272:T272"].Value = "$  " + string.Format("{0:N}", stack[x].AcceptedTransportCost);
                            ws.Cells["R272:T272"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            ws.Cells["M273:O273"].Merge = true;
                            ws.Cells["M273:O273"].Value = "APL Deductions";
                            ws.Cells["P273:Q273"].Merge = true;
                            ws.Cells["R273:T273"].Merge = true;
                            ws.Cells["R273:T273"].Value = "- $  " + string.Format("{0:N}", AplDetect);
                            ws.Cells["R273:T273"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;



                            ws.Cells["M274:T274"].Merge = true;
                            ws.Cells["M274:T274"].Value = "";

                            ws.Cells["M275:Q275"].Merge = true;
                            ws.Cells["M275:Q275"].Value = "Total Invoice";
                            ws.Cells["R275:T275"].Merge = true;
                            ws.Cells["R275:T275"].Value = "$  " + string.Format("{0:N}", TotalInvoice);
                            ws.Cells["R275:T275"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;




                            ws.Cells["M266:T275"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            ws.Cells["M266:T275"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            ws.Cells["M266:T275"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["M266:T275"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                            ws.Cells["E166:Z168"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            ws.Cells["J166:O168"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["J166:O168"].Style.Fill.BackgroundColor.SetColor(WhiteHex);
                            ws.Cells["J166:O168"].Style.Font.Color.SetColor(OrangeHex);

                            ws.Cells["J166:O168,Y166:AC168,R166:U168,F225:F227,N227:O227,E225:F225,E226:F226,E227:F227"].Style.Numberformat.Format = "#,##0.00";


                            ws.Cells["Y166:AC168"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["Y166:AC168"].Style.Fill.BackgroundColor.SetColor(WhiteHex);
                            ws.Cells["Y166:AC168"].Style.Font.Color.SetColor(OrangeHex);

                            ws.Cells["R166:U168"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["R166:U168"].Style.Fill.BackgroundColor.SetColor(WhiteHex);
                            ws.Cells["R166:U168"].Style.Font.Color.SetColor(OrangeHex);


                            ws.Cells["P267:Q267"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["P267:Q267"].Style.Fill.BackgroundColor.SetColor(BottomHex);

                            ws.Cells["M274:T274"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["M274:T274"].Style.Fill.BackgroundColor.SetColor(AshHex);

                            ws.Cells["M266:T275"].Style.Font.Bold = true;
                            ws.Cells["M266:T266"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                            ws.Cells["T266:T275"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["M275:T275"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                            ws.Cells["M266:M275"].Style.Border.Left.Style = ExcelBorderStyle.Medium;

                            ws.Cells["A276"].Value = "Disclaimer:";
                            ws.Cells["A277"].Value = "In the interest of ensuring a smooth invoicing/payment process, GCC SERVICES herewith signs this GRR with the intent to officially review the Weekly Billing Discount and APL formulas.  We will submit a correction/recovery request as applicable.";

                            //ws.Cells["J16:Z165,E174:F194,J174:P194,E199:F219,J199:P219"].Style.Numberformat.Format = "#,##0.00";
                            //ws.Cells["J16:Z165,E174:F194,J174:P194,E199:F219,J199:P219"].Style.Numberformat.Format = "#,##0.000";//Working
                            ws.Cells["J16:Z165,D174:D194,J174:K194,D199:D219,J199:K219,E225:E227,J166:N168"].Style.Numberformat.Format = "#,##0.000";


                            #endregion
                            x = x + 1;
                        }
                    }
                    string txtTName = InvoiceNo;
                    //Write it back to the client
                    if (Generate == true) // flag used to Generate Consolidate and Single for Update the Document in the table
                    {
                        Response.Clear();
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;  filename=" + txtTName + ".xlsx");
                    }
                    if ((stack != null) && (Single == true))
                    {
                        string path = Response.OutputStream.ToString();
                        byte[] data = pck.GetAsByteArray();


                        ExcelDocuments XD = IS.GetExcelDocumentsDetailsByControlId(stack[0].UNID);
                        if (XD != null)
                        {
                            XD.DocumentData = data;
                            long id = IS.SaveOrUpdateExcelDocuments(XD, userId);
                            // SaveDocumentToRecentDownloads(XD, null, string.Empty, null, string.Empty);
                        }
                        else if (Single == true)
                        {

                            InvoiceManagementView Imv = IS.GetInvoiceManagementViewDetailsByControlId(stack[0].UNID);
                            ExcelDocuments ed = new ExcelDocuments();
                            ed.ControlId = Imv.ControlId;
                            ed.OrderId = Imv.OrderId;
                            ed.InvoiceId = Imv.Id;
                            ed.Location = Imv.Location;
                            ed.Name = Imv.Name;
                            ed.ContingentType = Imv.ContingentType;
                            ed.Period = Imv.Period;
                            ed.PeriodYear = Imv.PeriodYear;
                            ed.Sector = Imv.Sector;
                            ed.Week = Imv.Week;
                            ed.DocumentData = data;
                            ed.DocumentType = "Excel-Single";
                            ed.DocumentName = stack[0].Reference;
                            long id = IS.SaveOrUpdateExcelDocuments(ed, userId);
                            //SaveDocumentToRecentDownloads(ed, null, string.Empty, null, string.Empty);
                        }
                        if (Generate == true) // flag used to Generate Consolidate and Single for Update the Document in the table
                        {
                            Response.BinaryWrite(data);
                            Response.End();
                        }
                    }
                    else if ((invoiceList != null) && (Consol == true))
                    {

                        InvoiceList ci = (InvoiceList)invoiceList;
                        var InvNoSplit = ci.InvoiceNo.ToString().Split('-');
                        var Period = ci.Period.ToString().Split('/');
                        //to check weather cosolidate sheet is already exsist or not
                        ExcelDocuments ED = GetExcelDocumentForConsolidate(InvNoSplit[1], InvNoSplit[3], Period[0], Period[1]);

                        byte[] data = pck.GetAsByteArray();

                        if (ED != null && ED.Id > 0)
                        {
                            ED.DocumentData = data;
                            long id = IS.SaveOrUpdateExcelDocuments(ED, userId);
                            //SaveDocumentToRecentDownloads(ED, null, string.Empty, null, string.Empty);
                        }
                        else
                        {
                            ED.ControlId = ci.InvoiceNo;
                            ED.ContingentType = InvNoSplit[3];
                            ED.Period = Period[0];
                            ED.PeriodYear = Period[1];
                            ED.Week = ci.PeriodWeek != null ? Convert.ToInt64(ci.PeriodWeek.FirstOrDefault().Week) : 0;
                            ED.Sector = InvNoSplit[1];
                            ED.DocumentData = data;
                            ED.DocumentType = "Excel-Consol";
                            ED.DocumentName = ci.InvoiceNo;
                            long id = IS.SaveOrUpdateExcelDocuments(ED, userId);
                            //SaveDocumentToRecentDownloads(ED, null, string.Empty, null, string.Empty);
                        }
                        if (Generate == true) // flag used to Generate Consolidate and Single for Update the Document in the table
                        {
                            Response.BinaryWrite(data);
                            Response.End();
                        }
                    }
                    else
                    {
                        if (Generate == true) // flag used to Generate Consolidate and Single for Update the Document in the table
                        {
                            Response.BinaryWrite(pck.GetAsByteArray());
                            Response.End();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #region ImporttoExcelsheet old
        //public void ImportToExcelSheet(DataSet Workbookset, InvoiceList invoiceList, List<SingleInvoice> stack, string InvoiceNo, bool Consol, bool Single, bool Generate)
        //{
        //    try
        //    {
        //        string userId = base.ValidateUser();
        //        using (ExcelPackage pck = new ExcelPackage())
        //        {
        //            int TableCount = Workbookset.Tables.Count;
        //            int x = 0; // Main Int is zero 
        //            System.Drawing.Image logo = System.Drawing.Image.FromFile("D:\\HeaderImage/main_logo.jpg");
        //            for (int i = 0; i < TableCount; i++)
        //            {

        //                if ((Workbookset.Tables[i].TableName.ToString() == InvoiceNo) && (Consol == true))
        //                {
        //                    #region Cosolidate
        //                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add(Workbookset.Tables[i].TableName.ToString());
        //                    ws.View.ZoomScale = 85;
        //                    //Adding image 
        //                    int img = 0;
        //                    ws.Row(img * 5).Height = 39.00D;
        //                    var picture = ws.Drawings.AddPicture(img.ToString(), logo);
        //                    picture.SetPosition(1, 0, 1, 0);

        //                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
        //                    ///line 1///
        //                    ws.Cells["D6"].LoadFromCollection("si.UNID", true);
        //                    ws.Cells["B6:L7"].Merge = true;
        //                    ws.Cells["B6:L7"].Value = "INVOICE";
        //                    ws.Cells["B6:L7"].Style.WrapText = true;
        //                    ws.Cells["B6:L7"].Style.Font.Bold = true;
        //                    ws.Cells["B6:L7"].Style.Font.Name = "Arial";
        //                    ws.Cells["B6:L7"].Style.Font.Size = 20;
        //                    ws.Cells["B6:L7"].AutoFitColumns();
        //                    ws.Cells["B6:L7"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["B6:L7"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["B6:L7"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["B6:L7"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["B6:L7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        //                    ws.Cells["B8:N70"].Style.Font.Size = 10;
        //                    ws.Cells["B8:N70"].Style.Font.Name = "Arial";

        //                    ws.Cells["B8:G8"].Merge = true;
        //                    ws.Cells["B8"].Value = "To";
        //                    ws.Cells["B8:G8"].Style.Font.Bold = true;
        //                    ws.Cells["B8:G8"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["B8:G8"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //                    ws.Cells["H8:I8"].Merge = true;
        //                    ws.Cells["H8"].Value = "";
        //                    ws.Cells["H8:I8"].Style.Font.Bold = true;
        //                    ws.Cells["H8:I8"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //                    ws.Cells["J8:L8"].Merge = true;
        //                    ws.Cells["J8"].Value = "";
        //                    ws.Cells["J8:L8"].Style.Font.Bold = true;
        //                    ws.Cells["J8:L8"].Style.Border.Right.Style = ExcelBorderStyle.Medium;

        //                    ws.Cells["B9:G9"].Merge = true;
        //                    ws.Cells["B9"].Value = "                     Chief Rations Unit";
        //                    ws.Cells["B9:G9"].Style.Font.Bold = true;
        //                    ws.Cells["B9:G9"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["B9:G9"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //                    ws.Cells["H9:I9"].Merge = true;
        //                    ws.Cells["H9"].Value = "Invoice #:";
        //                    ws.Cells["H9:I9"].Style.Font.Bold = true;
        //                    ws.Cells["H9:I9"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //                    ws.Cells["J9:L9"].Merge = true;
        //                    ws.Cells["J9"].Value = invoiceList.InvoiceNo;                           //InvoiceNo
        //                    ws.Cells["J9:L9"].Style.Font.Bold = true;
        //                    ws.Cells["J9:L9"].Style.Border.Right.Style = ExcelBorderStyle.Medium;

        //                    ws.Cells["B10:G10"].Merge = true;
        //                    ws.Cells["B10"].Value = "                     African Union - United Nations Hybrid ";
        //                    ws.Cells["B10:G10"].Style.Font.Bold = true;
        //                    ws.Cells["B10:G10"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["B10:G10"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //                    ws.Cells["H10:I10"].Merge = true;
        //                    ws.Cells["H10"].Value = "Contract #:";
        //                    ws.Cells["H10:I10"].Style.Font.Bold = true;
        //                    ws.Cells["H10:I10"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //                    ws.Cells["J10:L10"].Merge = true;
        //                    ws.Cells["J10"].Value = invoiceList.Contract;                           //Contract
        //                    ws.Cells["J10:L10"].Style.Font.Bold = true;
        //                    ws.Cells["J10:L10"].Style.Border.Right.Style = ExcelBorderStyle.Medium;

        //                    ws.Cells["B11:G11"].Merge = true;
        //                    ws.Cells["B11"].Value = "                     Operation in Darfur";
        //                    ws.Cells["B11:G11"].Style.Font.Bold = true;
        //                    ws.Cells["B11:G11"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["B11:G11"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //                    ws.Cells["H11:I11"].Merge = true;
        //                    ws.Cells["H11"].Value = "Invoice Date:";
        //                    ws.Cells["H11:I11"].Style.Font.Bold = true;
        //                    ws.Cells["H11:I11"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //                    ws.Cells["J11:L11"].Merge = true;
        //                    ws.Cells["J11"].Value = invoiceList.InvoiceDate;                        //InvoiceDate
        //                    ws.Cells["J11:L11"].Style.Font.Bold = true;
        //                    ws.Cells["J11:L11"].Style.Border.Right.Style = ExcelBorderStyle.Medium;

        //                    ws.Cells["B12:G12"].Merge = true;
        //                    ws.Cells["B12"].Value = "                     El Fasher, Darfur";
        //                    ws.Cells["B12:G12"].Style.Font.Bold = true;
        //                    ws.Cells["B12:G12"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["B12:G12"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //                    ws.Cells["H12:I12"].Merge = true;
        //                    ws.Cells["H12"].Value = "Payment Terms:";
        //                    ws.Cells["H12:I12"].Style.Font.Bold = true;
        //                    ws.Cells["H12:I12"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //                    ws.Cells["J12:L12"].Merge = true;
        //                    ws.Cells["J12"].Value = invoiceList.PayTerms;                           //PayTerms
        //                    ws.Cells["J12:L12"].Style.Font.Bold = true;
        //                    ws.Cells["J12:L12"].Style.Border.Right.Style = ExcelBorderStyle.Medium;

        //                    ws.Cells["B13:G13"].Merge = true;
        //                    ws.Cells["B13"].Value = "Cc:               Mission Designated Official";
        //                    ws.Cells["B13:G13"].Style.Font.Bold = true;
        //                    ws.Cells["B13:G13"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["B13:G13"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //                    ws.Cells["H13:I13"].Merge = true;
        //                    ws.Cells["H13"].Value = "PO:";
        //                    ws.Cells["H13:I13"].Style.Font.Bold = true;
        //                    ws.Cells["H13:I13"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //                    ws.Cells["J13:L13"].Merge = true;
        //                    ws.Cells["J13"].Value = invoiceList.PO;                                 //PO
        //                    ws.Cells["J13:L13"].Style.Font.Bold = true;
        //                    ws.Cells["J13:L13"].Style.Border.Right.Style = ExcelBorderStyle.Medium;

        //                    ws.Cells["B14:G14"].Merge = true;
        //                    ws.Cells["B14"].Value = "                    UNAMID El Fasher";
        //                    ws.Cells["B14:G14"].Style.Font.Bold = true;
        //                    ws.Cells["B14:G14"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["B14:G14"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //                    ws.Cells["H14:I14"].Merge = true;
        //                    ws.Cells["H14"].Value = "";
        //                    ws.Cells["H14:I14"].Style.Font.Bold = true;
        //                    ws.Cells["H14:I14"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //                    ws.Cells["J14:L14"].Merge = true;
        //                    ws.Cells["J14"].Value = "";
        //                    ws.Cells["J14:L14"].Style.Font.Bold = true;
        //                    ws.Cells["J14:L14"].Style.Border.Right.Style = ExcelBorderStyle.Medium;

        //                    ws.Cells["B15:G15"].Merge = true;
        //                    ws.Cells["B15:G15"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["B15:G15"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["B15:G15"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["H15:I15"].Merge = true;
        //                    ws.Cells["H15:I15"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //                    ws.Cells["H15:I15"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["J15:L15"].Merge = true;
        //                    ws.Cells["J15:L15"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //                    ws.Cells["J15:L15"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

        //                    ws.Cells["C16:H21"].Style.Font.Bold = true;
        //                    ws.Cells["B16"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["L16"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["B17"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["L17"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["B18"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["L18"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["B19"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["L19"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["B20"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["L20"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["B21"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["L21"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["B21:L21"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

        //                    ws.Cells["C16:F16"].Merge = true;
        //                    ws.Cells["C16:F16"].Value = "  Period:";
        //                    ws.Cells["G16:H16"].Merge = true;
        //                    ws.Cells["G16:H16"].Value = invoiceList.Period;                           //Period

        //                    ws.Cells["C17:F17"].Merge = true;
        //                    ws.Cells["C17:F17"].Value = "  UN Identification Number:";
        //                    ws.Cells["G17:H17"].Merge = true;
        //                    ws.Cells["G17:H17"].Value = invoiceList.UnINo;                            //UnINo

        //                    ws.Cells["C18:F18"].Merge = true;
        //                    ws.Cells["C18:F18"].Value = "  Sector:";
        //                    ws.Cells["G18:H18"].Merge = true;
        //                    ws.Cells["G18:H18"].Value = invoiceList.Sector;                           //Sector

        //                    ws.Cells["C19:F19"].Merge = true;
        //                    ws.Cells["C19:F19"].Value = "  Total Feeding Troop Strength:";
        //                    ws.Cells["G19:H19"].Merge = true;
        //                    ws.Cells["G19:H19"].Value = invoiceList.TotalFeedingToop;                 //TotalFeedingToop

        //                    ws.Cells["C20:F20"].Merge = true;
        //                    ws.Cells["C20:F20"].Value = "  Total Mandays:";
        //                    ws.Cells["G20:H20"].Merge = true;
        //                    ws.Cells["G20:H20"].Value = invoiceList.TotMadays;                       //TotMadays

        //                    ws.Cells["C21:F21"].Merge = true;
        //                    ws.Cells["C21:F21"].Value = "  Accepted CMR:";
        //                    ws.Cells["G21:H21"].Merge = true;
        //                    ws.Cells["G21:H21"].Value = invoiceList.CMR;                            //CMR

        //                    ws.Cells["B22:L22"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["B22:L22"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["B22:L22"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["B22:L22"].Style.Font.Bold = true;

        //                    ws.Cells["B22:C22"].Merge = true;
        //                    ws.Cells["B22:C22"].Value = "Qty";
        //                    ws.Cells["B22:C22"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        //                    ws.Cells["D22:G22"].Merge = true;
        //                    ws.Cells["D22:G22"].Value = "Description";
        //                    ws.Cells["D22:G22"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        //                    ws.Cells["H22:I22"].Merge = true;
        //                    ws.Cells["H22:I22"].Value = "Amount in USD";
        //                    ws.Cells["H22:I22"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        //                    ws.Cells["J22:L22"].Merge = true;
        //                    ws.Cells["J22:L22"].Value = "Amount in USD";
        //                    ws.Cells["J22:L22"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


        //                    for (int j = 23; j < 51; j++)
        //                    {
        //                        ws.Cells["B" + j + ":C" + j + ""].Merge = true;
        //                        ws.Cells["D" + j + ":G" + j + ""].Merge = true;
        //                        ws.Cells["H" + j + ":I" + j + ""].Merge = true;
        //                        ws.Cells["J" + j + ":L" + j + ""].Merge = true;
        //                        ws.Cells["B" + j + ":L" + j + ""].Style.Border.Left.Style = ExcelBorderStyle.Medium;
        //                        ws.Cells["B" + j + ":L" + j + ""].Style.Border.Right.Style = ExcelBorderStyle.Medium;
        //                    }
        //                    ws.Cells["D24:G24"].Value = "PROVISION OF FOOD RATIONS";
        //                    ws.Cells["D26:G26"].Value = invoiceList.Sector;
        //                    ws.Cells["D24:G26"].Style.Font.Bold = true;

        //                    int count = invoiceList.PeriodWeek.Count();

        //                    int k = 27;
        //                    decimal SumofInvValues = 0;
        //                    for (int P = 0; P < count; P++)
        //                    {

        //                        ws.Cells["B" + k + ":C" + k + ""].Value = invoiceList.PeriodWeek[P].AcceptedQty;                                //Accepted Qty
        //                        ws.Cells["B" + k + ":C" + k + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                        ws.Cells["D" + k + ":G" + k + ""].Value = "Total Value Delivered - Week " + invoiceList.PeriodWeek[P].Week;     //week

        //                        ws.Cells["H" + k + ":I" + k + ""].Value = "$     " + invoiceList.PeriodWeek[P].InvoiceValue;                               //InvoiceValue
        //                        ws.Cells["H" + k + ":I" + k + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                        SumofInvValues = SumofInvValues + Convert.ToDecimal(invoiceList.PeriodWeek[P].InvoiceValue);
        //                        //ws.Cells["J" + k + ":L" + k + ""].Value = "$  " + invoiceList.PeriodWeek[P].InvoiceValue;                               //InvoiceValue
        //                        //ws.Cells["J" + k + ":L" + k + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                        k = k + 1;
        //                    }

        //                    ws.Cells["J26"].Value = SumofInvValues;
        //                    ws.Cells["D40:G40"].Value = "DISCOUNTS";
        //                    ws.Cells["D40:G40"].Style.Font.Bold = true;
        //                    ws.Cells["D41:G41"].Value = "       Troop strength discounts";
        //                    ws.Cells["D42:G42"].Value = "       Weekly Discount";
        //                    ws.Cells["J42:L42"].Value = "( $" + Math.Round(invoiceList.WeeklyDiscount, 2) + ")";
        //                    ws.Cells["J42:L42"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

        //                    ws.Cells["H41,H42,J40"].Value = "    -";



        //                    ws.Cells["D44:G44"].Value = "PERFORMANCE MATRICES";
        //                    ws.Cells["D44:G44"].Style.Font.Bold = true;
        //                    ws.Cells["D45:G45"].Value = "       Conformity to Delivery Schedule";
        //                    ws.Cells["H45:I45"].Value = "-" + invoiceList.Delivery;                                                           //Delivery
        //                    ws.Cells["J45:L45"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                    ws.Cells["D46:G46"].Value = "       Conformity to Order by Line Items";
        //                    ws.Cells["H46:I46"].Value = "-" + invoiceList.OrderLineItems;                                                     //OrderLineItems
        //                    ws.Cells["J46:L46"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                    ws.Cells["D47:G47"].Value = "       Conformity to Orders by Weight";
        //                    ws.Cells["H47:I47"].Value = "-" + invoiceList.OrderByWeight;                                                      //OrderByWeight
        //                    ws.Cells["J47:L47"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                    ws.Cells["D48:G48"].Value = "       Food Order Compliance-Availability";
        //                    ws.Cells["H48:I48"].Value = "-" + invoiceList.ComplainAvalability;                                                //ComplainAvalability
        //                    ws.Cells["J48:L48"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                    ws.Cells["B50:L50"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

        //                    ws.Cells["J44"].Value = Convert.ToDecimal(invoiceList.Delivery.Replace("$", "")) + Convert.ToDecimal(invoiceList.OrderLineItems.Replace("$", "")) + Convert.ToDecimal(invoiceList.OrderByWeight.Replace("$", "")) + Convert.ToDecimal(invoiceList.ComplainAvalability.Replace("$", ""));

        //                    ws.Cells["B51:L51"].Style.Font.Bold = true;
        //                    ws.Cells["B51:G51"].Merge = true;
        //                    // ws.Cells["B51:G51"].Value = "Gulf Catering Company for General Trade and Contracting WLL";
        //                    ws.Cells["H51:I51"].Merge = true;
        //                    ws.Cells["H51:I51"].Value = "NET TOTAL";
        //                    ws.Cells["H51:I51"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        //                    ws.Cells["J51:L51"].Merge = true;
        //                    ws.Cells["J51:L51"].Value = invoiceList.SubTotal;                                                           //SubTotal
        //                    ws.Cells["J51:L51"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                    ws.Cells["J51:L51"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


        //                    //  ws.Row(53).Height = 27.00;

        //                    //ws.Cells["B52:I52"].Merge = true;
        //                    //ws.Cells["B52:J52"].Value = "Amount in Words : USD Ninety Five Thousand Five Hundred Seventy Five and 28/100 Only";
        //                    //ws.Cells["H52:I52"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        //                    //ws.Cells["J52:L52"].Merge = true;
        //                    //ws.Cells["J52:L52"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        //                    ws.Cells["B53:G53"].Style.WrapText = true;
        //                    ws.Cells["B53:L53"].Style.Font.Bold = true;
        //                    ws.Cells["B53:G53"].Merge = true;
        //                    ws.Cells["B53:G53"].Value = "A Prompt Payment Discount of 0.2% of the NET TOTAL applies if payment is made in less than 30 days.		";
        //                    ws.Cells["H53:I53"].Merge = true;
        //                    // ws.Cells["H53:I53"].Value = "Grand Total";
        //                    ws.Cells["H53:I53"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
        //                    ws.Cells["H53:I53"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
        //                    ws.Cells["J53:L53"].Merge = true;
        //                    // ws.Cells["J53:L53"].Value = invoiceList.GrandTotal;                                                         //GrandTotal
        //                    ws.Cells["J53:L53"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                    ws.Cells["J53:L53"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
        //                    ws.Cells["J53:L53"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

        //                    ws.Cells["B54:G54"].Merge = true;
        //                    ws.Cells["B54:G54"].Value = "IF payment is made in less than 30 days, the PPD is: 		";
        //                    ws.Cells["H54:I54"].Merge = true;
        //                    ws.Cells["H54:I54"].Value = "$     " + Math.Round(Convert.ToDecimal(invoiceList.SubTotal.Replace("$", "")) * (decimal)0.002, 2);
        //                    ws.Cells["J54:L54"].Merge = true;

        //                    ws.Cells["B51:L54"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["B51:L54"].Style.Border.Right.Style = ExcelBorderStyle.Medium;


        //                    ws.Cells["B52:L52"].Merge = true;
        //                    ws.Cells["B52:L52"].Value = "Amount in words: " + invoiceList.Usd_words + " Only";
        //                    ws.Cells["B52:L52"].Style.Font.Bold = true;
        //                    ws.Cells["B52:L52"].Style.Font.Color.SetColor(Color.Red);
        //                    ws.Cells["B52:L52"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["B52:L52"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["B52:L52"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["B52:L52"].Style.Border.Left.Style = ExcelBorderStyle.Medium;

        //                    ws.Cells["B56:L56"].Merge = true;
        //                    ws.Cells["B56:L56"].Value = "Bank Details";
        //                    ws.Cells["B56:L56"].Style.Font.UnderLine = true;
        //                    ws.Cells["B56:L56"].Style.Font.Bold = true;
        //                    ws.Cells["B57:L57"].Merge = true;
        //                    ws.Cells["B57:L57"].Value = "   Bank account name: Gulf Catering Company for General Trade and Contracting WLL";
        //                    ws.Cells["B58:L58"].Merge = true;
        //                    ws.Cells["B58:L58"].Value = "   Bank name: GULF Bank";
        //                    ws.Cells["B59:L59"].Merge = true;
        //                    ws.Cells["B59:L59"].Value = "   Bank account number: KW57GULB0000000000000090622676;";
        //                    ws.Cells["B60:L60"].Merge = true;
        //                    ws.Cells["B60:L60"].Value = "   Bank address: Qibla Area Hamad Al-Saqr Street, Kharafi Tower, First Floor, P.O. Box 1683, Safat, Kuwait City, Kuwait 1683";
        //                    ws.Cells["B61:L61"].Merge = true;
        //                    ws.Cells["B62:L62"].Merge = true;
        //                    ws.Cells["B61:L61"].Value = "   SWIFT/ABA: GULBKWKW";
        //                    ws.Cells["B56:L62"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["B56:L62"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["B62:L62"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

        //                    ws.Cells["B64:L64,B65:L65"].Merge = true;
        //                    ws.Cells["B64:L64,B65:L65"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["B64:L64,B65:L65"].Style.Border.Top.Color.SetColor(Color.Orange);
        //                    ws.Cells["B64:L64,B65:L65"].Style.Border.Top.Style = ExcelBorderStyle.Hair;
        //                    ws.Cells["B64:L64,B65:L65"].Style.Border.Top.Color.SetColor(Color.Red);
        //                    ws.Cells["B65:L65"].Value = "Gulf Catering Company for General Trade and Contracting WLL  - P.O Box 4583, Safat 13046, Kuwait";
        //                    ws.Cells["B64:L64"].Value = "Email:  UNAMIDAR@GCCServices.com		";

        //                    ws.Cells["B67"].Value = "Disclaimer:";
        //                    ws.Cells["B68"].Value = "In the interest of ensuring a smooth invoicing/payment process, GCC SERVICES herewith signs this GRR with the intent to officially review the Weekly Billing Discount and APL formulas.  We will submit a correction/recovery request as applicable.";

        //                    for (int m = 7; m <= 55; m++)
        //                    {
        //                        ws.Cells["B" + m + ":L" + m + ""].Style.Border.Top.Style = ExcelBorderStyle.Hair;
        //                        ws.Cells["B" + m + ":L" + m + ""].Style.Border.Top.Color.SetColor(Color.Red);
        //                    }
        //                    //To Make Sheet without GridLines
        //                    ws.View.ShowGridLines = false;

        //                    //Printer Settings

        //                    ws.PrinterSettings.TopMargin = ws.PrinterSettings.BottomMargin = 0.25M;
        //                    ws.PrinterSettings.LeftMargin = ws.PrinterSettings.RightMargin = 0M;
        //                    ws.PrinterSettings.HeaderMargin = 0M;
        //                    ws.PrinterSettings.FooterMargin = 0.25M;
        //                    ws.PrinterSettings.Orientation = eOrientation.Portrait;
        //                    ws.PrinterSettings.PaperSize = ePaperSize.A4;
        //                    ws.PrinterSettings.FitToPage = true;
        //                    ws.PrinterSettings.FitToWidth = 1;
        //                    ws.PrinterSettings.FitToHeight = 1;
        //                    ws.PrinterSettings.HorizontalCentered = true;
        //                    ws.PrinterSettings.VerticalCentered = true;



        //                    #endregion Consolidate
        //                }
        //                else if ((Workbookset.Tables[i].TableName.ToString() == stack[x].UNID) && (Single == true))
        //                {
        //                    #region Single


        //                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add(Workbookset.Tables[i].TableName.ToString());
        //                    ws.View.ZoomScale = 70;
        //                    ws.View.ShowGridLines = false;

        //                    int img = 0;
        //                    //for Adding image 
        //                    ws.Row(img * 5).Height = 39.00D;
        //                    var picture = ws.Drawings.AddPicture(img.ToString(), logo);
        //                    picture.SetPosition(1, 0, 0, 0);

        //                    ws.Cells["A6:AE276"].Style.Font.Size = 12;
        //                    Color BlueHex = System.Drawing.ColorTranslator.FromHtml("#8DB4E2");
        //                    Color WhiteHex = System.Drawing.ColorTranslator.FromHtml("#F2F2F2");
        //                    Color OrangeHex = System.Drawing.ColorTranslator.FromHtml("#FF9933");
        //                    Color AshHex = System.Drawing.ColorTranslator.FromHtml("#BFBFBF");
        //                    Color BottomHex = System.Drawing.ColorTranslator.FromHtml("#C4BD97");

        //                    ws.Cells["A6:N11"].Style.Font.Bold = true;
        //                    ws.Cells["A6:B6"].Merge = true;
        //                    ws.Cells["A6:B6"].Value = "Reference #";
        //                    ws.Cells["C6:H6"].Merge = true;
        //                    ws.Cells["C6:H6"].Value = stack[x].Reference;
        //                    ws.Cells["I6:K6"].Merge = true;
        //                    ws.Cells["I6:K6"].Value = "Period:";
        //                    ws.Cells["L6:M6"].Merge = true;
        //                    ws.Cells["L6:M6"].Value = stack[x].Period;

        //                    ws.Cells["A7:B7"].Merge = true;
        //                    ws.Cells["A7:B7"].Value = "Delivery Point:";
        //                    ws.Cells["C7:H7"].Merge = true;
        //                    ws.Cells["C7:H7"].Value = stack[x].DeliveryPoint;
        //                    ws.Cells["I7:K7"].Merge = true;
        //                    ws.Cells["I7:K7"].Value = "DOS:";
        //                    ws.Cells["L7:M7"].Merge = true;
        //                    ws.Cells["L7:M7"].Value = stack[x].DOS;

        //                    ws.Cells["A8:B8"].Merge = true;
        //                    ws.Cells["A8:B8"].Value = "UN ID of the FFO:";
        //                    ws.Cells["C8:H8"].Merge = true;
        //                    ws.Cells["C8:H8"].Value = stack[x].UNID;
        //                    ws.Cells["I8:K8"].Merge = true;
        //                    ws.Cells["I8:K8"].Value = "Delivery Week:";
        //                    ws.Cells["L8:M8"].Merge = true;
        //                    ws.Cells["L8:M8"].Value = stack[x].DeliveryWeek;

        //                    ws.Cells["A9:B9"].Merge = true;
        //                    ws.Cells["A9:B9"].Value = "Strength:";
        //                    ws.Cells["C9:H9"].Merge = true;
        //                    ws.Cells["C9:H9"].Value = stack[x].Strength;
        //                    ws.Cells["I9:K9"].Merge = true;
        //                    ws.Cells["I9:K9"].Value = "Delivery Mode:";
        //                    ws.Cells["L9:M9"].Merge = true;
        //                    ws.Cells["L9:M9"].Value = stack[x].DeliveryMode;

        //                    ws.Cells["A10:B10"].Merge = true;
        //                    ws.Cells["A10:B10"].Value = "Man Days:";
        //                    ws.Cells["C10:H10"].Merge = true;
        //                    ws.Cells["C10:H10"].Value = stack[x].ManDays;
        //                    ws.Cells["I10:K10"].Merge = true;
        //                    ws.Cells["I10:K10"].Value = "Approved Delivery Dates:";
        //                    ws.Cells["L10:M10"].Merge = true;
        //                    ws.Cells["L10:M10"].Value = stack[x].ApprovedDelivery;

        //                    ws.Cells["A11:B11"].Merge = true;
        //                    ws.Cells["A11:B11"].Value = "Accepted CMR";
        //                    ws.Cells["C11:H11"].Merge = true;
        //                    ws.Cells["C11:H11"].Value = stack[x].ApplicableCMR;
        //                    ws.Cells["I11:K11"].Merge = true;
        //                    ws.Cells["I11:K11"].Value = "Actual Delivery Date";
        //                    ws.Cells["L11:M11"].Merge = true;
        //                    ws.Cells["L11:M11"].Value = stack[x].ActualDeliveryDate;


        //                    ws.Cells["A6:M11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

        //                    ws.Cells["A13:AC14"].Style.Font.Bold = true;
        //                    ws.Cells["A13:AC14"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["A13:AC14"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["A13:AC14"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["A13:AC14"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["A13:AC14"].Style.Fill.PatternType = ExcelFillStyle.Solid;
        //                    ws.Cells["A13:AC14"].Style.Fill.BackgroundColor.SetColor(BlueHex);
        //                    ws.Cells["A13:AC14"].Style.Font.Color.SetColor(Color.White);
        //                    ws.Cells["A13:AC14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

        //                    ws.Cells["A13:B14"].Merge = true;
        //                    ws.Cells["A13:B14"].Value = "S.NO";
        //                    ws.Cells["A13:B14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["A13:B14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    ws.Cells["C13:D14"].Merge = true;
        //                    ws.Cells["C13:D14"].Value = "UNRS NO";
        //                    ws.Cells["C13:D14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["C13:D14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    ws.Cells["E13:I14"].Merge = true;
        //                    ws.Cells["E13:I14"].Value = "Discription";
        //                    ws.Cells["E13:I14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["E13:I14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    ws.Cells["J13:K14"].Merge = true;
        //                    ws.Cells["J13:K14"].Value = "Order Qty";
        //                    ws.Cells["J13:K14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["J13:K14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    ws.Cells["L13:M14"].Merge = true;
        //                    ws.Cells["L13:M14"].Value = "Delivery Qty";
        //                    ws.Cells["L13:M14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["L13:M14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    ws.Cells["N13:O14"].Merge = true;
        //                    ws.Cells["N13:O14"].Value = "Accepted Qty";
        //                    ws.Cells["N13:O14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["N13:O14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    ws.Cells["P13:Q14"].Merge = true;
        //                    ws.Cells["P13:Q14"].Value = "Price/Unit";
        //                    ws.Cells["P13:Q14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["P13:Q14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    ws.Cells["R13:S14"].Merge = true;
        //                    ws.Cells["R13:S14"].Value = "Net Amount";
        //                    ws.Cells["R13:S14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["R13:S14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    ws.Cells["T13:U14"].Merge = true;
        //                    ws.Cells["T13:U14"].Value = "APL Weight";
        //                    ws.Cells["T13:U14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["T13:U14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    ws.Cells["V13:W14"].Merge = true;
        //                    ws.Cells["V13:W14"].Value = "Discrepancy Code";
        //                    ws.Cells["V13:W14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["V13:W14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    ws.Cells["X13:X14"].Merge = true;
        //                    ws.Cells["X13:X14"].Value = "UOM";
        //                    ws.Cells["X13:X14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    ws.Cells["Y13:Z14"].Merge = true;
        //                    ws.Cells["Y13:Z14"].Value = "Order Value";
        //                    ws.Cells["Y13:Z14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["Y13:Z14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    ws.Cells["AA13:AC14"].Merge = true;
        //                    ws.Cells["AA13:AC14"].Value = "DN #";
        //                    ws.Cells["AA13:AC14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["AA13:AC14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

        //                    ws.Cells["A15:B15"].Merge = true;
        //                    ws.Cells["C15:D15"].Merge = true;
        //                    ws.Cells["E15:I15"].Merge = true;
        //                    ws.Cells["J15:K15"].Merge = true;
        //                    ws.Cells["L15:M15"].Merge = true;
        //                    ws.Cells["N15:O15"].Merge = true;
        //                    ws.Cells["P15:Q15"].Merge = true;
        //                    ws.Cells["R15:S15"].Merge = true;
        //                    ws.Cells["T15:U15"].Merge = true;
        //                    ws.Cells["V15:W15"].Merge = true;
        //                    ws.Cells["X15:X15"].Merge = true;
        //                    ws.Cells["Y15:Z15"].Merge = true;
        //                    ws.Cells["AA15:AC15"].Merge = true;
        //                    ws.Cells["A15:AC15"].Style.Border.Left.Style = ExcelBorderStyle.Hair;
        //                    ws.Cells["AC15"].Style.Border.Right.Style = ExcelBorderStyle.Medium;

        //                    for (int l = 16; l < 166; l++)
        //                    {
        //                        ws.Cells["A" + l + ":B" + l + ""].Merge = true;
        //                        ws.Cells["A" + l + ":B" + l + ""].Value = "";
        //                        ws.Cells["A" + l + ":B" + l + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                        ws.Cells["C" + l + ":D" + l + ""].Merge = true;
        //                        ws.Cells["C" + l + ":D" + l + ""].Value = "";
        //                        ws.Cells["C" + l + ":D" + l + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                        ws.Cells["E" + l + ":I" + l + ""].Merge = true;
        //                        ws.Cells["E" + l + ":I" + l + ""].Value = "";
        //                        ws.Cells["J" + l + ":K" + l + ""].Merge = true;
        //                        ws.Cells["J" + l + ":K" + l + ""].Value = "";
        //                        ws.Cells["J" + l + ":K" + l + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                        ws.Cells["L" + l + ":M" + l + ""].Merge = true;
        //                        ws.Cells["L" + l + ":M" + l + ""].Value = "";
        //                        ws.Cells["L" + l + ":M" + l + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                        ws.Cells["N" + l + ":O" + l + ""].Merge = true;
        //                        ws.Cells["N" + l + ":O" + l + ""].Value = "";
        //                        ws.Cells["N" + l + ":O" + l + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                        ws.Cells["P" + l + ":Q" + l + ""].Merge = true;
        //                        ws.Cells["P" + l + ":Q" + l + ""].Value = "";
        //                        ws.Cells["P" + l + ":Q" + l + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                        ws.Cells["R" + l + ":S" + l + ""].Merge = true;
        //                        ws.Cells["R" + l + ":S" + l + ""].Value = "";
        //                        ws.Cells["R" + l + ":S" + l + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                        ws.Cells["T" + l + ":U" + l + ""].Merge = true;
        //                        ws.Cells["T" + l + ":U" + l + ""].Value = "";
        //                        ws.Cells["T" + l + ":U" + l + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                        ws.Cells["V" + l + ":W" + l + ""].Merge = true;
        //                        ws.Cells["V" + l + ":W" + l + ""].Value = "";
        //                        ws.Cells["V" + l + ":W" + l + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                        ws.Cells["X" + l + ":X" + l + ""].Merge = true;
        //                        ws.Cells["X" + l + ":X" + l + ""].Value = "";
        //                        ws.Cells["Y" + l + ":Z" + l + ""].Merge = true;
        //                        ws.Cells["Y" + l + ":Z" + l + ""].Value = "";
        //                        ws.Cells["Y" + l + ":Z" + l + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                        ws.Cells["AA" + l + ":AC" + l + ""].Merge = true;
        //                        ws.Cells["AA" + l + ":AC" + l + ""].Value = "";
        //                        ws.Cells["A" + l + ":AC" + l + ""].Style.Border.Right.Style = ExcelBorderStyle.Hair;
        //                        ws.Cells["AC" + l + ""].Style.Border.Right.Style = ExcelBorderStyle.Medium;
        //                    }



        //                    int temp = 0;
        //                    int orderCount = (16 + stack[x].DeliveryDetails.Count());
        //                    int s = 0;
        //                    if (stack[x].DeliveryDetails.Count() != 0)
        //                    {
        //                        for (int a = 16; a < orderCount; a++)
        //                        {
        //                            temp = temp + 1;
        //                            ws.Cells["A" + a + ":B" + a + ""].Merge = true;
        //                            ws.Cells["A" + a + ":B" + a + ""].Value = temp;
        //                            ws.Cells["A" + a + ":B" + a + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                            ws.Cells["C" + a + ":D" + a + ""].Merge = true;
        //                            ws.Cells["C" + a + ":D" + a + ""].Value = stack[x].DeliveryDetails[s].UNCode;
        //                            ws.Cells["C" + a + ":D" + a + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                            ws.Cells["E" + a + ":I" + a + ""].Merge = true;
        //                            ws.Cells["E" + a + ":I" + a + ""].Value = stack[x].DeliveryDetails[s].Commodity;
        //                            ws.Cells["J" + a + ":K" + a + ""].Merge = true;
        //                            ws.Cells["J" + a + ":K" + a + ""].Value = Math.Round(stack[x].DeliveryDetails[s].OrderQty, 3);
        //                            ws.Cells["J" + a + ":K" + a + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                            ws.Cells["L" + a + ":M" + a + ""].Merge = true;
        //                            ws.Cells["L" + a + ":M" + a + ""].Value = Math.Round(stack[x].DeliveryDetails[s].DeliveredOrdQty, 3);
        //                            ws.Cells["L" + a + ":M" + a + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                            ws.Cells["N" + a + ":O" + a + ""].Merge = true;
        //                            ws.Cells["N" + a + ":O" + a + ""].Value = Math.Round(stack[x].DeliveryDetails[s].InvoiceQty, 3);
        //                            ws.Cells["N" + a + ":O" + a + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                            ws.Cells["P" + a + ":Q" + a + ""].Merge = true;
        //                            ws.Cells["P" + a + ":Q" + a + ""].Value = "$" + Math.Round(stack[x].DeliveryDetails[s].SectorPrice, 2);
        //                            ws.Cells["P" + a + ":Q" + a + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                            ws.Cells["R" + a + ":S" + a + ""].Merge = true;
        //                            ws.Cells["R" + a + ":S" + a + ""].Value = "$" + Math.Round(stack[x].DeliveryDetails[s].NetAmt, 2);
        //                            ws.Cells["R" + a + ":S" + a + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                            ws.Cells["T" + a + ":U" + a + ""].Merge = true;
        //                            ws.Cells["T" + a + ":U" + a + ""].Value = Math.Round(stack[x].DeliveryDetails[s].APLWeight, 2) + "%";
        //                            ws.Cells["T" + a + ":U" + a + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                            ws.Cells["V" + a + ":W" + a + ""].Merge = true;
        //                            ws.Cells["V" + a + ":W" + a + ""].Value = stack[x].DeliveryDetails[s].DiscrepancyCode;
        //                            ws.Cells["V" + a + ":W" + a + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                            ws.Cells["X" + a + ":X" + a + ""].Merge = true;
        //                            ws.Cells["X" + a + ":X" + a + ""].Value = stack[x].DeliveryDetails[s].UOM;
        //                            ws.Cells["Y" + a + ":Z" + a + ""].Merge = true;
        //                            ws.Cells["Y" + a + ":Z" + a + ""].Value = "$" + Math.Round(stack[x].DeliveryDetails[s].OrderValue, 2);
        //                            ws.Cells["Y" + a + ":Z" + a + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                            ws.Cells["AA" + a + ":AC" + a + ""].Merge = true;
        //                            ws.Cells["AA" + a + ":AC" + a + ""].Value = stack[x].DeliveryDetails[s].DeliveryNote;
        //                            ws.Cells["AA" + a + ":AC" + a + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
        //                            ws.Cells["A" + a + ":AC" + a + ""].Style.Border.Right.Style = ExcelBorderStyle.Hair;
        //                            ws.Cells["AC" + a + ""].Style.Border.Right.Style = ExcelBorderStyle.Medium;
        //                            s = s + 1;
        //                        }
        //                    }
        //                    ws.Cells["A165:AC165"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

        //                    ws.Cells["A166:B168"].Merge = true;
        //                    ws.Cells["A166:B168"].Value = temp;
        //                    ws.Cells["A166:B168"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["A166:B168"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

        //                    ws.Cells["C166:I166"].Merge = true;
        //                    ws.Cells["C166:I166"].Value = "Sub total without Eggs..................................................................................";
        //                    ws.Cells["J166:K166"].Merge = true;
        //                    ws.Cells["J166:K166"].Value = string.Format("{0:N}", stack[x].OrderedQtySum);
        //                    ws.Cells["L166:M166"].Merge = true;
        //                    ws.Cells["L166:M166"].Value = string.Format("{0:N}", stack[x].DeliveredQtySum);
        //                    ws.Cells["N166:O166"].Merge = true;
        //                    ws.Cells["N166:O166"].Value = string.Format("{0:N}", stack[x].InvoiceQtySum);
        //                    ws.Cells["P166:Q166"].Merge = true;
        //                    ws.Cells["P166:Q166"].Value = "";
        //                    ws.Cells["R166:S166"].Merge = true;
        //                    ws.Cells["R166:S166"].Value = "$  " + string.Format("{0:N}", (Math.Round(stack[x].NetAmountSum, 2)));
        //                    ws.Cells["T166:U166"].Merge = true;
        //                    ws.Cells["T166:U166"].Value = stack[x].AboveCount;
        //                    ws.Cells["V166:W166"].Merge = true;
        //                    ws.Cells["V166:W166"].Value = "";
        //                    ws.Cells["X166:X166"].Merge = true;
        //                    ws.Cells["X166:X166"].Value = "";
        //                    ws.Cells["Y166:Z166"].Merge = true;
        //                    ws.Cells["Y166:Z166"].Value = "$  " + string.Format("{0:N}", (Math.Round(stack[x].OrdervalueSum, 2)));
        //                    ws.Cells["AA166:AC166"].Merge = true;
        //                    ws.Cells["AA166:AC166"].Value = "";

        //                    ws.Cells["C167:I167"].Merge = true;
        //                    ws.Cells["C167:I167"].Value = "Eggs in KG ...................................................................................................................";
        //                    ws.Cells["J167:K167"].Merge = true;
        //                    ws.Cells["J167:K167"].Value = stack[x].EggOrderedQtySum;
        //                    ws.Cells["L167:M167"].Merge = true;
        //                    ws.Cells["L167:M167"].Value = stack[x].EggDeliveredQtySum;
        //                    ws.Cells["N167:O167"].Merge = true;
        //                    ws.Cells["N167:O167"].Value = stack[x].EggInvoiceQtySum;
        //                    ws.Cells["P167:Q167"].Merge = true;
        //                    ws.Cells["P167:Q167"].Value = "";
        //                    ws.Cells["R167:S167"].Merge = true;
        //                    ws.Cells["R167:S167"].Value = "$-";
        //                    ws.Cells["T167:U167"].Merge = true;
        //                    ws.Cells["T167:U167"].Value = stack[x].BelowCount;
        //                    ws.Cells["V167:W167"].Merge = true;
        //                    ws.Cells["V167:W167"].Value = "";
        //                    ws.Cells["X167:X167"].Merge = true;
        //                    ws.Cells["X167:X167"].Value = "";
        //                    ws.Cells["Y167:Z167"].Merge = true;
        //                    ws.Cells["Y167:Z167"].Value = "$-";
        //                    ws.Cells["AA167:AC167"].Merge = true;
        //                    ws.Cells["AA167:AC167"].Value = "";

        //                    ws.Cells["C168:I168"].Merge = true;
        //                    ws.Cells["C168:I168"].Value = "Sub total with Eggs in KG...........................................................................................";
        //                    ws.Cells["J168:K168"].Merge = true;
        //                    ws.Cells["J168:K168"].Value = stack[x].TotalOrderedQtySum;
        //                    ws.Cells["L168:M168"].Merge = true;
        //                    ws.Cells["L168:M168"].Value = stack[x].TotalDeliveredQtySum;
        //                    ws.Cells["N168:O168"].Merge = true;
        //                    ws.Cells["N168:O168"].Value = stack[x].TotalInvoiceQtySum;
        //                    ws.Cells["P168:Q168"].Merge = true;
        //                    ws.Cells["P168:Q168"].Value = "";
        //                    ws.Cells["R168:S168"].Merge = true;
        //                    ws.Cells["R168:S168"].Value = "$  " + string.Format("{0:N}", (Math.Round(stack[x].NetAmountSum, 2)));
        //                    ws.Cells["T168:U168"].Merge = true;
        //                    ws.Cells["T168:U168"].Value = Math.Round(stack[x].CountPercent, 2) + "%";
        //                    ws.Cells["V168:W168"].Merge = true;
        //                    ws.Cells["V168:W168"].Value = "";
        //                    ws.Cells["X168:X168"].Merge = true;
        //                    ws.Cells["X168:X168"].Value = "";
        //                    ws.Cells["Y168:Z168"].Merge = true;
        //                    ws.Cells["Y168:Z168"].Value = "$  " + string.Format("{0:N}", (Math.Round(stack[x].OrdervalueSum, 2)));
        //                    ws.Cells["AA168:AC168"].Merge = true;
        //                    ws.Cells["AA168:AC168"].Value = "";
        //                    ws.Cells["A166:AC168"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["A166:AC168"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["A166:AC168"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

        //                    #region Substitution
        //                    ws.Cells["A170:R170"].Merge = true;
        //                    ws.Cells["A170:R170"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

        //                    ws.Cells["A171:R171"].Merge = true;
        //                    ws.Cells["A171:R171"].Value = "Substitutions";
        //                    ws.Cells["A171:R171"].Style.Font.Italic = true;
        //                    ws.Cells["A171:R171"].Style.Font.Color.SetColor(Color.DarkRed);
        //                    ws.Cells["A171:R171"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["A171:R171"].Style.Border.Right.Style = ExcelBorderStyle.Medium;


        //                    ws.Cells["A172:R173"].Style.WrapText = true;
        //                    ws.Cells["A172:R173"].Style.Font.Bold = true;
        //                    ws.Cells["A172:R173"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["A172:R173"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["A172:R173"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["A172:R173"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["A172:R173"].Style.Fill.PatternType = ExcelFillStyle.Solid;
        //                    ws.Cells["A172:R173"].Style.Fill.BackgroundColor.SetColor(BlueHex);
        //                    ws.Cells["A172:R173"].Style.Font.Color.SetColor(Color.White);
        //                    ws.Cells["A172:R173"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

        //                    ws.Cells["A172:A173"].Merge = true;
        //                    ws.Cells["A172:A173"].Value = "UNRS Code";
        //                    ws.Cells["A172:A173"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["A172:A173"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    ws.Cells["B172:C173"].Merge = true;
        //                    ws.Cells["B172:C173"].Value = "Substituted With COMMODITY";
        //                    ws.Cells["B172:C173"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["B172:C173"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    ws.Cells["D172:E173"].Merge = true;
        //                    ws.Cells["D172:E173"].Value = "Delivered Quantity";
        //                    ws.Cells["D172:E173"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["D172:E173"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    ws.Cells["F172:F173"].Merge = true;
        //                    ws.Cells["F172:F173"].Value = "UNIT COST";
        //                    ws.Cells["F172:F173"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["F172:F173"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    ws.Cells["G172:G173"].Merge = true;
        //                    ws.Cells["G172:G173"].Value = "UNRS Code";
        //                    ws.Cells["G172:G173"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["G172:G173"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    ws.Cells["H172:I173"].Merge = true;
        //                    ws.Cells["H172:I173"].Value = "COMMODITY";
        //                    ws.Cells["H172:I173"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["H172:I173"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    ws.Cells["J172:J173"].Merge = true;
        //                    ws.Cells["J172:J173"].Value = "Ordered";
        //                    ws.Cells["J172:J173"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["J172:J173"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    ws.Cells["K172:L173"].Merge = true;
        //                    ws.Cells["K172:L173"].Value = "Accepted  Quantity";
        //                    ws.Cells["K172:L173"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["K172:L173"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    ws.Cells["M172:M173"].Merge = true;
        //                    ws.Cells["M172:M173"].Value = "UNIT COST";
        //                    ws.Cells["M172:M173"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["M172:M173"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    ws.Cells["N172:O173"].Merge = true;
        //                    ws.Cells["N172:O173"].Value = "Accepted Amount";
        //                    ws.Cells["N172:O173"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["N172:O173"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    ws.Cells["P172:P173"].Merge = true;
        //                    ws.Cells["P172:P173"].Value = "APL Weight";
        //                    ws.Cells["P172:P173"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    ws.Cells["P172:P173"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["Q172:R173"].Merge = true;
        //                    ws.Cells["Q172:R173"].Value = "DN #";
        //                    ws.Cells["Q172:R173"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["Q172:R173"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


        //                    for (int K = 174; K < 194; K++)
        //                    {
        //                        ws.Cells["A" + K + ":A" + K + ""].Merge = true;
        //                        ws.Cells["A" + K + ":A" + K + ""].Value = "";
        //                        ws.Cells["A" + K + ":A" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                        ws.Cells["B" + K + ":C" + K + ""].Merge = true;
        //                        ws.Cells["B" + K + ":C" + K + ""].Value = "";
        //                        ws.Cells["D" + K + ":E" + K + ""].Merge = true;
        //                        ws.Cells["D" + K + ":E" + K + ""].Value = "";
        //                        ws.Cells["D" + K + ":E" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                        ws.Cells["F" + K + ":F" + K + ""].Merge = true;
        //                        ws.Cells["F" + K + ":F" + K + ""].Value = "";
        //                        ws.Cells["F" + K + ":F" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                        ws.Cells["G" + K + ":G" + K + ""].Merge = true;
        //                        ws.Cells["G" + K + ":G" + K + ""].Value = "";
        //                        ws.Cells["G" + K + ":G" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                        ws.Cells["H" + K + ":I" + K + ""].Merge = true;
        //                        ws.Cells["H" + K + ":I" + K + ""].Value = "";
        //                        ws.Cells["J" + K + ":J" + K + ""].Merge = true;
        //                        ws.Cells["J" + K + ":J" + K + ""].Value = "";
        //                        ws.Cells["J" + K + ":J" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                        ws.Cells["K" + K + ":L" + K + ""].Merge = true;
        //                        ws.Cells["K" + K + ":L" + K + ""].Value = "";
        //                        ws.Cells["K" + K + ":L" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                        ws.Cells["M" + K + ":M" + K + ""].Merge = true;
        //                        ws.Cells["M" + K + ":M" + K + ""].Value = "";
        //                        ws.Cells["M" + K + ":M" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                        ws.Cells["N" + K + ":O" + K + ""].Merge = true;
        //                        ws.Cells["N" + K + ":O" + K + ""].Value = "";
        //                        ws.Cells["N" + K + ":O" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                        ws.Cells["P" + K + ":P" + K + ""].Merge = true;
        //                        ws.Cells["P" + K + ":P" + K + ""].Value = "";
        //                        ws.Cells["P" + K + ":P" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                        ws.Cells["Q" + K + ":R" + K + ""].Merge = true;
        //                        ws.Cells["Q" + K + ":R" + K + ""].Value = "";
        //                        ws.Cells["A" + K + ":R" + K + ""].Style.Border.Right.Style = ExcelBorderStyle.Hair;
        //                        ws.Cells["A" + K + ":R" + K + ""].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
        //                        ws.Cells["R" + K + ""].Style.Border.Right.Style = ExcelBorderStyle.Medium;
        //                    }

        //                    int tempSub = 0;
        //                    int x1 = 0;
        //                    int dcount = (174 + stack[x].SDeliveryList.Count());
        //                    int AboveSubtituteCount = 0;
        //                    if (stack[x].SDeliveryList.Count() != 0)
        //                    {
        //                        for (int K = 174; K < dcount; K++)
        //                        {
        //                            if (stack[x].SDeliveryList[x1].SubstituteItemCode != 0)
        //                            {
        //                                tempSub = tempSub + 1;
        //                            }
        //                            //Modified by Thamizh
        //                            if (K < 195)
        //                            {
        //                                ws.Cells["A" + K + ":A" + K + ""].Merge = true;
        //                                ws.Cells["A" + K + ":A" + K + ""].Value = stack[x].SDeliveryList[x1].SubstituteItemCode;
        //                                ws.Cells["A" + K + ":A" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                                ws.Cells["B" + K + ":C" + K + ""].Merge = true;
        //                                ws.Cells["B" + K + ":C" + K + ""].Value = stack[x].SDeliveryList[x1].SubstituteItemName;
        //                                ws.Cells["D" + K + ":E" + K + ""].Merge = true;
        //                                ws.Cells["D" + K + ":E" + K + ""].Value = Math.Round(stack[x].SDeliveryList[x1].DeliveredQty, 3);
        //                                ws.Cells["D" + K + ":E" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                                ws.Cells["F" + K + ":F" + K + ""].Merge = true;
        //                                ws.Cells["F" + K + ":F" + K + ""].Value = "$" + Math.Round(stack[x].SDeliveryList[x1].SubstituteSectorPrice, 2);
        //                                ws.Cells["F" + K + ":F" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                                ws.Cells["G" + K + ":G" + K + ""].Merge = true;
        //                                ws.Cells["G" + K + ":G" + K + ""].Value = stack[x].SDeliveryList[x1].UNCode;
        //                                ws.Cells["G" + K + ":G" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                                ws.Cells["H" + K + ":I" + K + ""].Merge = true;
        //                                ws.Cells["H" + K + ":I" + K + ""].Value = stack[x].SDeliveryList[x1].Commodity;
        //                                ws.Cells["J" + K + ":J" + K + ""].Merge = true;
        //                                ws.Cells["J" + K + ":J" + K + ""].Value = Math.Round(stack[x].SDeliveryList[x1].OrderedQty, 3);
        //                                ws.Cells["J" + K + ":J" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                                ws.Cells["K" + K + ":L" + K + ""].Merge = true;
        //                                ws.Cells["K" + K + ":L" + K + ""].Value = Math.Round(stack[x].SDeliveryList[x1].InvoiceQty, 3);
        //                                ws.Cells["K" + K + ":L" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                                ws.Cells["M" + K + ":M" + K + ""].Merge = true;
        //                                ws.Cells["M" + K + ":M" + K + ""].Value = "$" + Math.Round(stack[x].SDeliveryList[x1].SectorPrice, 2);
        //                                ws.Cells["M" + K + ":M" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                                ws.Cells["N" + K + ":O" + K + ""].Merge = true;

        //                                if (stack[x].SDeliveryList[x1].APLWeight >= 98) { AboveSubtituteCount = AboveSubtituteCount + 1; }

        //                                ws.Cells["N" + K + ":O" + K + ""].Value = "$" + Math.Round(stack[x].SDeliveryList[x1].AcceptedAmt, 2);
        //                                ws.Cells["N" + K + ":O" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                                ws.Cells["P" + K + ":P" + K + ""].Merge = true;
        //                                ws.Cells["P" + K + ":P" + K + ""].Value = Math.Round(stack[x].SDeliveryList[x1].APLWeight, 2) + "%";
        //                                ws.Cells["P" + K + ":P" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                                ws.Cells["Q" + K + ":R" + K + ""].Merge = true;
        //                                ws.Cells["Q" + K + ":R" + K + ""].Value = stack[x].SDeliveryList[x1].DeliveryNoteName;
        //                                ws.Cells["A" + K + ":R" + K + ""].Style.Border.Right.Style = ExcelBorderStyle.Hair;
        //                                ws.Cells["A" + K + ":R" + K + ""].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
        //                                ws.Cells["R" + K + ""].Style.Border.Right.Style = ExcelBorderStyle.Medium;
        //                            }
        //                            x1 = x1 + 1;
        //                        }
        //                    }

        //                    ws.Cells["A194:A194"].Merge = true;
        //                    ws.Cells["A194:A194"].Value = x1;
        //                    ws.Cells["A194:A194"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["B194:C194"].Merge = true;
        //                    ws.Cells["B194:C194"].Value = "Sub Total";
        //                    ws.Cells["D194:E194"].Merge = true;
        //                    ws.Cells["D194:E194"].Value = string.Format("{0:N}", (Math.Round(stack[x].SDeliveryQuantity, 3)));
        //                    ws.Cells["F194:F194"].Merge = true;
        //                    ws.Cells["F194:F194"].Value = "";
        //                    ws.Cells["G194:G194"].Merge = true;
        //                    ws.Cells["G194:G194"].Value = "";
        //                    ws.Cells["H194:I194"].Merge = true;
        //                    ws.Cells["H194:I194"].Value = "";
        //                    ws.Cells["J194:J194"].Merge = true;
        //                    ws.Cells["J194:J194"].Value = string.Format("{0:N}", (Math.Round(stack[x].SOrderedQuantity, 3)));
        //                    ws.Cells["K194:L194"].Merge = true;
        //                    ws.Cells["K194:L194"].Value = string.Format("{0:N}", (Math.Round(stack[x].SAcceptedQuantity, 3)));
        //                    ws.Cells["M194:M194"].Merge = true;
        //                    ws.Cells["M194:M194"].Value = "";
        //                    ws.Cells["N194:O194"].Merge = true;
        //                    ws.Cells["N194:O194"].Value = "$  " + string.Format("{0:N}", (Math.Round(stack[x].SAcceptedamt, 2)));
        //                    ws.Cells["P194:P194"].Merge = true;
        //                    ws.Cells["P194:P194"].Value = "";
        //                    ws.Cells["Q194:R194"].Merge = true;
        //                    ws.Cells["Q194:R194"].Value = "";
        //                    ws.Cells["A194:R194"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["A194:R194"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["A194:R194"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["A194:R194"].Style.Fill.PatternType = ExcelFillStyle.Solid;
        //                    ws.Cells["A194:R194"].Style.Fill.BackgroundColor.SetColor(BlueHex);
        //                    ws.Cells["A194:R194"].Style.Font.Color.SetColor(Color.White);
        //                    #endregion

        //                    #region Replacement

        //                    ws.Cells["A195:R195"].Merge = true;
        //                    ws.Cells["A195:R195"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

        //                    ws.Cells["A196:R196"].Merge = true;
        //                    ws.Cells["A196:R196"].Value = "Replacements";
        //                    ws.Cells["A196:R196"].Style.Font.Italic = true;
        //                    ws.Cells["A196:R196"].Style.Font.Color.SetColor(Color.DarkRed);
        //                    ws.Cells["A196:R196"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["A196:R196"].Style.Border.Right.Style = ExcelBorderStyle.Medium;


        //                    ws.Cells["A197:R198"].Style.WrapText = true;
        //                    ws.Cells["A197:R198"].Style.Font.Bold = true;
        //                    ws.Cells["A197:R198"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["A197:R198"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["A197:R198"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["A197:R198"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["A197:R198"].Style.Fill.PatternType = ExcelFillStyle.Solid;
        //                    ws.Cells["A197:R198"].Style.Fill.BackgroundColor.SetColor(BlueHex);
        //                    ws.Cells["A197:R198"].Style.Font.Color.SetColor(Color.White);
        //                    ws.Cells["A197:R198"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

        //                    ws.Cells["A197:A198"].Merge = true;
        //                    ws.Cells["A197:A198"].Value = "UNRS Code";
        //                    ws.Cells["A197:A198"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["A197:A198"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    ws.Cells["B197:C198"].Merge = true;
        //                    ws.Cells["B197:C198"].Value = "Replacements With COMMODITY";
        //                    ws.Cells["B197:C198"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["B197:C198"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    ws.Cells["D197:E198"].Merge = true;
        //                    ws.Cells["D197:E198"].Value = "Delivered Quantity";
        //                    ws.Cells["D197:E198"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["D197:E198"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    ws.Cells["F197:F198"].Merge = true;
        //                    ws.Cells["F197:F198"].Value = "UNIT COST";
        //                    ws.Cells["F197:F198"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["F197:F198"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    ws.Cells["G197:G198"].Merge = true;
        //                    ws.Cells["G197:G198"].Value = "UNRS Code";
        //                    ws.Cells["G197:G198"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["G197:G198"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    ws.Cells["H197:I198"].Merge = true;
        //                    ws.Cells["H197:I198"].Value = "COMMODITY";
        //                    ws.Cells["H197:I198"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["H197:I198"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    ws.Cells["J197:J198"].Merge = true;
        //                    ws.Cells["J197:J198"].Value = "Ordered";
        //                    ws.Cells["J197:J198"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["J197:J198"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    ws.Cells["K197:L198"].Merge = true;
        //                    ws.Cells["K197:L198"].Value = "Accepted  Quantity";
        //                    ws.Cells["K197:L198"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["K197:L198"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    ws.Cells["M197:M198"].Merge = true;
        //                    ws.Cells["M197:M198"].Value = "UNIT COST";
        //                    ws.Cells["M197:M198"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["M197:M198"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    ws.Cells["N197:O198"].Merge = true;
        //                    ws.Cells["N197:O198"].Value = "Accepted Amount";
        //                    ws.Cells["N197:O198"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["N197:O198"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    ws.Cells["P197:P198"].Merge = true;
        //                    ws.Cells["P197:P198"].Value = "APL Weight";
        //                    ws.Cells["P197:P198"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    ws.Cells["P197:P198"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["Q197:R198"].Merge = true;
        //                    ws.Cells["Q197:R198"].Value = "DN #";
        //                    ws.Cells["Q197:R198"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["Q197:R198"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


        //                    for (int K = 199; K < 219; K++)
        //                    {
        //                        ws.Cells["A" + K + ":A" + K + ""].Merge = true;
        //                        ws.Cells["A" + K + ":A" + K + ""].Value = "";
        //                        ws.Cells["A" + K + ":A" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                        ws.Cells["B" + K + ":C" + K + ""].Merge = true;
        //                        ws.Cells["B" + K + ":C" + K + ""].Value = "";
        //                        ws.Cells["D" + K + ":E" + K + ""].Merge = true;
        //                        ws.Cells["D" + K + ":E" + K + ""].Value = "";
        //                        ws.Cells["D" + K + ":E" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                        ws.Cells["F" + K + ":F" + K + ""].Merge = true;
        //                        ws.Cells["F" + K + ":F" + K + ""].Value = "";
        //                        ws.Cells["F" + K + ":F" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                        ws.Cells["G" + K + ":G" + K + ""].Merge = true;
        //                        ws.Cells["G" + K + ":G" + K + ""].Value = "";
        //                        ws.Cells["G" + K + ":G" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                        ws.Cells["H" + K + ":I" + K + ""].Merge = true;
        //                        ws.Cells["H" + K + ":I" + K + ""].Value = "";
        //                        ws.Cells["J" + K + ":J" + K + ""].Merge = true;
        //                        ws.Cells["J" + K + ":J" + K + ""].Value = "";
        //                        ws.Cells["J" + K + ":J" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                        ws.Cells["K" + K + ":L" + K + ""].Merge = true;
        //                        ws.Cells["K" + K + ":L" + K + ""].Value = "";
        //                        ws.Cells["K" + K + ":L" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                        ws.Cells["M" + K + ":M" + K + ""].Merge = true;
        //                        ws.Cells["M" + K + ":M" + K + ""].Value = "";
        //                        ws.Cells["M" + K + ":M" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                        ws.Cells["N" + K + ":O" + K + ""].Merge = true;
        //                        ws.Cells["N" + K + ":O" + K + ""].Value = "";
        //                        ws.Cells["N" + K + ":O" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                        ws.Cells["P" + K + ":P" + K + ""].Merge = true;
        //                        ws.Cells["P" + K + ":P" + K + ""].Value = "";
        //                        ws.Cells["P" + K + ":P" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                        ws.Cells["Q" + K + ":R" + K + ""].Merge = true;
        //                        ws.Cells["Q" + K + ":R" + K + ""].Value = "";
        //                        ws.Cells["A" + K + ":R" + K + ""].Style.Border.Right.Style = ExcelBorderStyle.Hair;
        //                        ws.Cells["A" + K + ":R" + K + ""].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
        //                        ws.Cells["R" + K + ""].Style.Border.Right.Style = ExcelBorderStyle.Medium;
        //                    }

        //                    int x2 = 0;
        //                    int Rcount = (199 + stack[x].RDeliveryList.Count());
        //                    //int AboveSubtituteCount = 0;
        //                    if (stack[x].RDeliveryList.Count() != 0)
        //                    {
        //                        for (int K = 199; K < Rcount; K++)
        //                        {
        //                            ws.Cells["A" + K + ":A" + K + ""].Merge = true;
        //                            ws.Cells["A" + K + ":A" + K + ""].Value = stack[x].RDeliveryList[x2].SubstituteItemCode;
        //                            ws.Cells["A" + K + ":A" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                            ws.Cells["B" + K + ":C" + K + ""].Merge = true;
        //                            ws.Cells["B" + K + ":C" + K + ""].Value = stack[x].RDeliveryList[x2].SubstituteItemName;
        //                            ws.Cells["D" + K + ":E" + K + ""].Merge = true;
        //                            ws.Cells["D" + K + ":E" + K + ""].Value = Math.Round(stack[x].RDeliveryList[x2].DeliveredQty, 3);
        //                            ws.Cells["D" + K + ":E" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                            ws.Cells["F" + K + ":F" + K + ""].Merge = true;
        //                            ws.Cells["F" + K + ":F" + K + ""].Value = "$" + Math.Round(stack[x].RDeliveryList[x2].SubstituteSectorPrice, 2);
        //                            ws.Cells["F" + K + ":F" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                            ws.Cells["G" + K + ":G" + K + ""].Merge = true;
        //                            ws.Cells["G" + K + ":G" + K + ""].Value = stack[x].RDeliveryList[x2].UNCode;
        //                            ws.Cells["G" + K + ":G" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                            ws.Cells["H" + K + ":I" + K + ""].Merge = true;
        //                            ws.Cells["H" + K + ":I" + K + ""].Value = stack[x].RDeliveryList[x2].Commodity;
        //                            ws.Cells["J" + K + ":J" + K + ""].Merge = true;
        //                            ws.Cells["J" + K + ":J" + K + ""].Value = Math.Round(stack[x].RDeliveryList[x2].OrderedQty, 3);
        //                            ws.Cells["J" + K + ":J" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                            ws.Cells["K" + K + ":L" + K + ""].Merge = true;
        //                            ws.Cells["K" + K + ":L" + K + ""].Value = Math.Round(stack[x].RDeliveryList[x2].InvoiceQty, 3);
        //                            ws.Cells["K" + K + ":L" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                            ws.Cells["M" + K + ":M" + K + ""].Merge = true;
        //                            ws.Cells["M" + K + ":M" + K + ""].Value = "$" + Math.Round(stack[x].RDeliveryList[x2].SectorPrice, 2);
        //                            ws.Cells["M" + K + ":M" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                            ws.Cells["N" + K + ":O" + K + ""].Merge = true;

        //                            if (stack[x].RDeliveryList[x2].APLWeight >= 98) { AboveSubtituteCount = AboveSubtituteCount + 1; }

        //                            ws.Cells["N" + K + ":O" + K + ""].Value = "$" + Math.Round(stack[x].RDeliveryList[x2].AcceptedAmt, 2);
        //                            ws.Cells["N" + K + ":O" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                            ws.Cells["P" + K + ":P" + K + ""].Merge = true;
        //                            ws.Cells["P" + K + ":P" + K + ""].Value = Math.Round(stack[x].RDeliveryList[x2].APLWeight, 2) + "%";
        //                            ws.Cells["P" + K + ":P" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                            ws.Cells["Q" + K + ":R" + K + ""].Merge = true;
        //                            ws.Cells["Q" + K + ":R" + K + ""].Value = stack[x].RDeliveryList[x2].DeliveryNoteName;
        //                            ws.Cells["A" + K + ":R" + K + ""].Style.Border.Right.Style = ExcelBorderStyle.Hair;
        //                            ws.Cells["A" + K + ":R" + K + ""].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
        //                            ws.Cells["R" + K + ""].Style.Border.Right.Style = ExcelBorderStyle.Medium;
        //                            x2 = x2 + 1;
        //                        }
        //                    }

        //                    ws.Cells["A219:A219"].Merge = true;
        //                    ws.Cells["A219:A219"].Value = x2;
        //                    ws.Cells["A219:A219"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["B219:C219"].Merge = true;
        //                    ws.Cells["B219:C219"].Value = "Sub Total";
        //                    ws.Cells["D219:E219"].Merge = true;
        //                    ws.Cells["D219:E219"].Value = string.Format("{0:N}", (Math.Round(stack[x].RDeliveryQuantity, 3)));
        //                    ws.Cells["F219:F219"].Merge = true;
        //                    ws.Cells["F219:F219"].Value = "";
        //                    ws.Cells["G219:G219"].Merge = true;
        //                    ws.Cells["G219:G219"].Value = "";
        //                    ws.Cells["H219:I219"].Merge = true;
        //                    ws.Cells["H219:I219"].Value = "";
        //                    ws.Cells["J219:J219"].Merge = true;
        //                    ws.Cells["J219:J219"].Value = string.Format("{0:N}", (Math.Round(stack[x].ROrderedQuantity, 3)));
        //                    ws.Cells["K219:L219"].Merge = true;
        //                    ws.Cells["K219:L219"].Value = string.Format("{0:N}", (Math.Round(stack[x].RAcceptedQuantity, 3)));
        //                    ws.Cells["M219:M219"].Merge = true;
        //                    ws.Cells["M219:M219"].Value = "";
        //                    ws.Cells["N219:O219"].Merge = true;
        //                    ws.Cells["N219:O219"].Value = "$  " + string.Format("{0:N}", (Math.Round(stack[x].RAcceptedamt, 2)));
        //                    ws.Cells["P219:P219"].Merge = true;
        //                    ws.Cells["P219:P219"].Value = "";
        //                    ws.Cells["Q219:R219"].Merge = true;
        //                    ws.Cells["Q219:R219"].Value = "";
        //                    ws.Cells["A219:R219"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["A219:R219"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["A219:R219"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["A219:R219"].Style.Fill.PatternType = ExcelFillStyle.Solid;
        //                    ws.Cells["A219:R219"].Style.Fill.BackgroundColor.SetColor(BlueHex);
        //                    ws.Cells["A219:R219"].Style.Font.Color.SetColor(Color.White);


        //                    #endregion


        //                    ws.Cells["A222:D222"].Merge = true;
        //                    ws.Cells["A222:D222"].Value = "Number of Days Delay";
        //                    ws.Cells["E222:F222"].Merge = true;
        //                    ws.Cells["E222:F222"].Value = stack[x].TotalDays;
        //                    ws.Cells["A223:D223"].Merge = true;
        //                    ws.Cells["A223:D223"].Value = "Total Line Items ordered";
        //                    ws.Cells["E223:F223"].Merge = true;
        //                    ws.Cells["E223:F223"].Value = temp;
        //                    ws.Cells["A224:D224"].Merge = true;
        //                    ws.Cells["A224:D224"].Value = "Total Line Items Received >= 98";
        //                    ws.Cells["E224:F224"].Merge = true;
        //                    ws.Cells["E224:F224"].Value = (stack[x].AboveCount + AboveSubtituteCount);
        //                    ws.Cells["A225:D225"].Merge = true;
        //                    ws.Cells["A225:D225"].Value = "Order Quantity";
        //                    ws.Cells["E225:F225"].Merge = true;
        //                    ws.Cells["E225:F225"].Value = stack[x].TotalOrderedQtySum;
        //                    ws.Cells["A226:D226"].Merge = true;
        //                    ws.Cells["A226:D226"].Value = "Delivered  Quantity";
        //                    ws.Cells["E226:F226"].Merge = true;
        //                    ws.Cells["E226:F226"].Value = Math.Round(stack[x].TotalDeliveredQtySum + stack[x].SDeliveryQuantity + stack[x].RDeliveryQuantity, 3);
        //                    ws.Cells["A227:D227"].Merge = true;
        //                    ws.Cells["A227:D227"].Value = "Accepted Quantity";
        //                    ws.Cells["E227:F227"].Merge = true;
        //                    ws.Cells["E227:F227"].Value = Math.Round(stack[x].TotalInvoiceQtySum + stack[x].SAcceptedQuantity + stack[x].RAcceptedQuantity, 3);
        //                    ws.Cells["A228:D228"].Merge = true;
        //                    ws.Cells["A228:D228"].Value = "Number Line Items Ordered";
        //                    ws.Cells["E228:F228"].Merge = true;
        //                    ws.Cells["E228:F228"].Value = (temp - tempSub);
        //                    ws.Cells["A229:D229"].Merge = true;
        //                    ws.Cells["A229:D229"].Value = "Number substitutions";
        //                    ws.Cells["E229:F229"].Merge = true;
        //                    ws.Cells["E229:F229"].Value = stack[x].SubstituteCount;

        //                    ws.Cells["I222:I225"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["I222:L222"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["L222:L225"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["I225:L225"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

        //                    ws.Cells["A222:F229"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        //                    ws.Cells["A222:F229"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //                    ws.Cells["A222:F222"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["F222:F229"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["A229:F229"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

        //                    ws.Cells["I222:J222"].Merge = true;
        //                    ws.Cells["I222:J222"].Value = "Applicable CMR";
        //                    ws.Cells["K222:L222"].Merge = true;
        //                    ws.Cells["K222:L222"].Value = stack[x].AuthorizedCMR;
        //                    ws.Cells["K222:L222"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                    ws.Cells["I223:J223"].Merge = true;
        //                    ws.Cells["I223:J223"].Value = "Order CMR";
        //                    ws.Cells["K223:L223"].Merge = true;

        //                    decimal OrderCMR = Math.Round((stack[x].OrdervalueSum / stack[x].ManDays), 2);

        //                    ws.Cells["K223:L223"].Value = OrderCMR;
        //                    ws.Cells["K223:L223"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                    ws.Cells["I224:J224"].Merge = true;
        //                    ws.Cells["I224:J224"].Value = "Accepted CMR ";
        //                    ws.Cells["K224:L224"].Merge = true;

        //                    decimal AcceptedCMR = Math.Round((((stack[x].NetAmountSum) + stack[x].SAcceptedamt + stack[x].RAcceptedamt) / stack[x].ManDays), 2);

        //                    ws.Cells["K224:L224"].Value = AcceptedCMR;
        //                    ws.Cells["K224:L224"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                    ws.Cells["I225:J225"].Merge = true;
        //                    ws.Cells["I225:J225"].Value = "% Of CMR Utilized";
        //                    ws.Cells["K225:L225"].Merge = true;

        //                    Decimal CmRUtilised = 0;
        //                    if ((AcceptedCMR != 0) && (stack[x].AuthorizedCMR != 0))
        //                    {
        //                        CmRUtilised = Math.Round((AcceptedCMR / OrderCMR) * 100, 2);
        //                    }


        //                    ws.Cells["K225:L225"].Value = CmRUtilised + "%";
        //                    ws.Cells["K225:L225"].Style.Font.Bold = true;
        //                    ws.Cells["K225:L225"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                    ws.Cells["I222:L225"].Style.Border.Diagonal.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["I222:L225"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //                    ws.Cells["I222:L225"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


        //                    ws.Cells["I227:M227"].Merge = true;
        //                    ws.Cells["I227:M227"].Value = "Food order value for APL Purposes";

        //                    ws.Cells["N227:O227"].Merge = true;
        //                    ws.Cells["N227:O227"].Value = "$  " + string.Format("{0:N}", stack[x].OrdervalueSum);
        //                    ws.Cells["N227:O227"].Style.Numberformat.Format = "#,##0.00";
        //                    ws.Cells["N227:O227"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                    ws.Cells["I227:O227"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["I227:O227"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["I227:O227"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["I227:O227"].Style.Border.Left.Style = ExcelBorderStyle.Medium;

        //                    ws.Cells["A231:T231"].Merge = true;
        //                    ws.Cells["A231:T231"].Value = "Performance Details";
        //                    ws.Cells["A231:T231"].Style.Font.Italic = true;
        //                    ws.Cells["A231:T231"].Style.Font.Bold = true;
        //                    ws.Cells["A231:T231"].Style.Font.Color.SetColor(Color.DarkRed);
        //                    ws.Cells["A231:T231"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


        //                    ws.Cells["A232:L232"].Merge = true;
        //                    ws.Cells["A232:L232"].Value = "Amount at risk in terms of a percentage of a Weekly invoice (Rations) 'C'  for each Delivery Point  15%";
        //                    ws.Cells["A232:L232"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                    ws.Cells["M232:T232"].Merge = true;

        //                    decimal deliveryPoint = Math.Round(((stack[x].SAcceptedamt + stack[x].RAcceptedamt + stack[x].NetAmountSum) / 100) * 15, 2);

        //                    ws.Cells["M232:T232"].Value = "$  " + string.Format("{0:N}", deliveryPoint);
        //                    ws.Cells["M232:T232"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

        //                    ws.Cells["A233:D233"].Merge = true;
        //                    ws.Cells["A233:D233"].Value = "Performance Details";

        //                    ws.Cells["E233:E233"].Merge = true;
        //                    ws.Cells["E233:E233"].Value = "APL Code";

        //                    ws.Cells["F233:F233"].Merge = true;
        //                    ws.Cells["F233:F233"].Value = "Target %";
        //                    ws.Cells["G233:H233"].Merge = true;
        //                    ws.Cells["G233:H233"].Value = "Acceptable";
        //                    ws.Cells["I233:L233"].Merge = true;
        //                    ws.Cells["I233:L233"].Value = "Band (level of performance)";
        //                    ws.Cells["M233:P233"].Merge = true;
        //                    ws.Cells["M233:P233"].Value = "Service Level Credit - % of invoice";
        //                    ws.Cells["Q233:R233"].Merge = true;
        //                    ws.Cells["Q233:R233"].Value = "Performance";
        //                    ws.Cells["S233:T233"].Merge = true;
        //                    ws.Cells["S233:T233"].Value = "Deduction";
        //                    ws.Cells["A233:T233"].Style.Font.Bold = true;

        //                    ws.Cells["A234:D239"].Merge = true;
        //                    ws.Cells["A234:D239"].Value = "1. Conformity to Delivery Schedule";
        //                    ws.Cells["A234:D239"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["A234:D239"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    ws.Cells["A234:D239"].Style.WrapText = true;

        //                    ws.Cells["E234:E239"].Merge = true;
        //                    ws.Cells["E234:E239"].Value = "S";


        //                    ws.Cells["F234:F239"].Merge = true;
        //                    ws.Cells["F234:F239"].Style.WrapText = true;
        //                    ws.Cells["F234:F239"].Value = "On time delivery";
        //                    ws.Cells["G234:H239"].Merge = true;
        //                    ws.Cells["G234:H239"].Value = "On time";
        //                    ws.Cells["I234:J236"].Merge = true;
        //                    ws.Cells["I234:J236"].Value = "1 day delay";
        //                    ws.Cells["I237:J239"].Style.Border.Top.Style = ExcelBorderStyle.Hair;
        //                    ws.Cells["I237:J239"].Merge = true;
        //                    ws.Cells["I237:J239"].Value = "2 day delay";
        //                    ws.Cells["K234:L236"].Merge = true;
        //                    ws.Cells["K234:L236"].Value = "40.00%";

        //                    ws.Cells["K237:L239"].Merge = true;
        //                    ws.Cells["K237:L239"].Value = "100.00%";
        //                    ws.Cells["M234:N236"].Merge = true;
        //                    ws.Cells["M234:N236"].Value = "1.20%";

        //                    ws.Cells["M237:N239"].Merge = true;
        //                    ws.Cells["M237:N239"].Value = "3.00%";
        //                    ws.Cells["O234:P236"].Merge = true;

        //                    ws.Cells["O237:P239"].Merge = true;
        //                    ws.Cells["Q234:R239"].Merge = true;
        //                    ws.Cells["Q234:R239"].Value = stack[x].DeliveryPerformance + "%";                          //Performance
        //                    ws.Cells["S234:T239"].Merge = true;
        //                    ws.Cells["S234:T239"].Value = "- $  " + string.Format("{0:N}", stack[x].DeliveryDeduction);                          //Day Deduction


        //                    ws.Cells["A240:D245"].Merge = true;
        //                    ws.Cells["A240:D245"].Value = "'2. Conformity to Order by Line Items Number of line items ordered is delivered (including authorized substitutions)' ";
        //                    ws.Cells["A240:D245"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["A240:D245"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    ws.Cells["A240:D245"].Style.WrapText = true;

        //                    ws.Cells["E240:E245"].Merge = true;
        //                    ws.Cells["E240:E245"].Value = "L";

        //                    ws.Cells["F240:F245"].Merge = true;
        //                    ws.Cells["F240:F245"].Value = "100%";
        //                    ws.Cells["G240:H245"].Merge = true;
        //                    ws.Cells["G240:H245"].Value = "98%";
        //                    ws.Cells["I240:J242"].Merge = true;
        //                    ws.Cells["I240:J242"].Value = "95% - <98%";
        //                    ws.Cells["I243:J245"].Merge = true;
        //                    ws.Cells["I243:J245"].Value = "<92% - <95%";
        //                    ws.Cells["K240:L242"].Merge = true;
        //                    ws.Cells["K240:L242"].Value = "40.00%";
        //                    ws.Cells["K243:L245"].Merge = true;
        //                    ws.Cells["K243:L245"].Value = "100.00%";
        //                    ws.Cells["M240:N242"].Merge = true;
        //                    ws.Cells["M240:N242"].Value = "1.20%";
        //                    ws.Cells["M243:N245"].Merge = true;
        //                    ws.Cells["M243:N245"].Value = "3.00%";

        //                    ws.Cells["O240:P242"].Merge = true;
        //                    ws.Cells["O243:P245"].Merge = true;
        //                    ws.Cells["Q240:R245"].Merge = true;
        //                    ws.Cells["Q240:R245"].Value = Math.Round(stack[x].LineItemPerformance, 2) + "%";                        //Performance
        //                    ws.Cells["S240:T245"].Merge = true;
        //                    ws.Cells["S240:T245"].Value = "- $  " + string.Format("{0:N}", stack[x].LineItemDeduction);                         //Day Deduction


        //                    ws.Cells["A246:D251"].Merge = true;
        //                    ws.Cells["A246:D251"].Value = "3. Conformity to Orders by weight:  Quantity kg/ltr/each Quantity of food order in Kg/Ltr is delivered (including authorized substitutions)";
        //                    ws.Cells["A246:D251"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["A246:D251"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    ws.Cells["A246:D251"].Style.WrapText = true;

        //                    ws.Cells["E246:E251"].Merge = true;
        //                    ws.Cells["E246:E251"].Value = "Q";

        //                    ws.Cells["F246:F251"].Merge = true;
        //                    ws.Cells["F246:F251"].Value = "100%";
        //                    ws.Cells["G246:H251"].Merge = true;
        //                    ws.Cells["G246:H251"].Value = "95%";
        //                    ws.Cells["I246:J248"].Merge = true;
        //                    ws.Cells["I246:J248"].Value = "92% - <95%";
        //                    ws.Cells["I249:J251"].Merge = true;
        //                    ws.Cells["I249:J251"].Value = "<90% - <92%";
        //                    ws.Cells["K246:L248"].Merge = true;
        //                    ws.Cells["K246:L248"].Value = "40.00%";
        //                    ws.Cells["K249:L251"].Merge = true;
        //                    ws.Cells["K249:L251"].Value = "100.00%";
        //                    ws.Cells["M246:N248"].Merge = true;
        //                    ws.Cells["M246:N248"].Value = "1.80%";
        //                    ws.Cells["M249:N251"].Merge = true;
        //                    ws.Cells["M249:N251"].Value = "4.50%";
        //                    ws.Cells["O246:P248"].Merge = true;
        //                    ws.Cells["O249:P251"].Merge = true;
        //                    ws.Cells["Q246:R251"].Merge = true;
        //                    ws.Cells["Q246:R251"].Value = stack[x].OrderWightPerformance + "%";                       //Performance
        //                    ws.Cells["S246:T251"].Merge = true;
        //                    ws.Cells["S246:T251"].Value = "- $  " + string.Format("{0:N}", stack[x].OrderWightDeduction);                       //Day Deduction

        //                    ws.Cells["A252:D257"].Merge = true;
        //                    ws.Cells["A252:D257"].Value = "4. Food Order Compliance-Availability :  Number of  authorized substitutions";
        //                    ws.Cells["A252:D257"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["A252:D257"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    ws.Cells["A252:D257"].Style.WrapText = true;

        //                    ws.Cells["E252:E257"].Merge = true;
        //                    ws.Cells["E252:E257"].Value = "A";

        //                    ws.Cells["F252:F257"].Merge = true;
        //                    ws.Cells["F252:F257"].Value = "0%";
        //                    ws.Cells["G252:H257"].Merge = true;
        //                    ws.Cells["G252:H257"].Value = "3%";
        //                    ws.Cells["I252:J254"].Merge = true;
        //                    ws.Cells["I252:J254"].Value = "3% - <4%";
        //                    ws.Cells["I255:J257"].Merge = true;
        //                    ws.Cells["I255:J257"].Value = "4% - 5% +";
        //                    ws.Cells["K252:L254"].Merge = true;
        //                    ws.Cells["K252:L254"].Value = "40.00%";
        //                    ws.Cells["K255:L257"].Merge = true;
        //                    ws.Cells["K255:L257"].Value = "100.00%";
        //                    ws.Cells["M252:N254"].Merge = true;
        //                    ws.Cells["M252:N254"].Value = "1.80%";
        //                    ws.Cells["M255:N257"].Merge = true;
        //                    ws.Cells["M255:N257"].Value = "4.50%";//
        //                    ws.Cells["O252:P254"].Merge = true;
        //                    ws.Cells["O255:P257"].Merge = true;
        //                    ws.Cells["Q252:R257"].Merge = true;
        //                    ws.Cells["Q252:R257"].Value = stack[x].SubtitutionPerformance + "%";                         //Performance
        //                    ws.Cells["S252:T257"].Merge = true;
        //                    ws.Cells["S252:T257"].Value = "- $  " + string.Format("{0:N}", stack[x].SubtitutionDeduction);                           //Day Deduction


        //                    ws.Cells["A258:T260"].Merge = true;
        //                    ws.Cells["A258:T260"].Value = "Methodology:  Verification shall be determined by the UN in its sole discretion.  The service credit shall be computed according to the following formula:  Service Level Credit = A x B x C ( A = Allocation (% of at risk amount) B =% of allocation C = At risk amount )";
        //                    ws.Cells["A258:T260"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["A258:T260"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    ws.Cells["A258:T260"].Style.WrapText = true;

        //                    ws.Cells["A232:T260"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["A232:T260"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["A232:T260"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["A232:T232"].Style.Fill.PatternType = ExcelFillStyle.Solid;
        //                    ws.Cells["A232:T232"].Style.Fill.BackgroundColor.SetColor(BlueHex);
        //                    ws.Cells["A232:T232"].Style.Font.Color.SetColor(Color.White);

        //                    ws.Cells["A233:T260"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    ws.Cells["A233:T260"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

        //                    ws.Cells["I237:P237"].Style.Border.Top.Style = ExcelBorderStyle.Hair;
        //                    ws.Cells["I243:P243"].Style.Border.Top.Style = ExcelBorderStyle.Hair;
        //                    ws.Cells["I249:P249"].Style.Border.Top.Style = ExcelBorderStyle.Hair;
        //                    ws.Cells["I255:P255"].Style.Border.Top.Style = ExcelBorderStyle.Hair;

        //                    ws.Cells["J234:J257"].Style.Border.Right.Style = ExcelBorderStyle.Hair;
        //                    ws.Cells["N234:N257"].Style.Border.Right.Style = ExcelBorderStyle.Hair;

        //                    ws.Cells["A262"].Value = "NPA 01 - Changed to comPliant as quantity under-delivered by the Vendor deemed non-relevant";
        //                    ws.Cells["A263"].Value = "NPA 02 - Change to comPliant as quantity order by the Mission does not meet minimal Packing requirements";
        //                    ws.Cells["A264"].Value = "NR - Shortage Quantity non-relevant to DCO";
        //                    ws.Cells["A265"].Value = "AS - Authorized Substitution";
        //                    ws.Cells["A266"].Value = "AR - Authorized Replacements";

        //                    ws.Cells["Q262:R262"].Merge = true;
        //                    ws.Cells["Q262:R262"].Value = "APL Deductions";
        //                    ws.Cells["S262:T262"].Merge = true;

        //                    decimal AplDetTotal = Math.Round((stack[x].DeliveryDeduction + stack[x].LineItemDeduction + stack[x].OrderWightDeduction + stack[x].SubtitutionDeduction), 2);

        //                    ws.Cells["S262:T262"].Value = "- $  " + string.Format("{0:N}", AplDetTotal);
        //                    ws.Cells["S262:T262"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //                    ws.Cells["Q262:T262"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["Q262:T262"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["Q262:T262"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["Q262:T262"].Style.Border.Left.Style = ExcelBorderStyle.Medium;

        //                    decimal AmtAccept = Math.Round((stack[x].NetAmountSum + stack[x].SAcceptedamt + stack[x].RAcceptedamt), 2);
        //                    decimal tropstgh = Math.Round((0 * AplDetTotal), 2);
        //                    decimal OtherCreditNote = Math.Round((0 * AplDetTotal), 2);
        //                    //decimal Weeklywise = Math.Round((0 * AplDetTotal), 2);
        //                    decimal Weeklywise = Math.Round((Convert.ToDecimal(0.35 / 100) * AplDetTotal), 2);
        //                    decimal NetRationAmt = Math.Round((AmtAccept - Weeklywise), 2);
        //                    //decimal NetRationAmt = Math.Round((tropstgh + OtherCreditNote + Weeklywise + AmtAccept), 2);
        //                    decimal AplDetect = Math.Round(@AplDetTotal, 2);

        //                    decimal TotalInvoice = Math.Round((NetRationAmt - AplDetect), 2);

        //                    ws.Cells["M266:O266"].Merge = true;
        //                    ws.Cells["M266:O266"].Value = " Amount  Accepted";
        //                    ws.Cells["P266:Q266"].Merge = true;
        //                    ws.Cells["R266:T266"].Merge = true;
        //                    ws.Cells["R266:T266"].Value = "$  " + string.Format("{0:N}", AmtAccept);
        //                    ws.Cells["R266:T266"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

        //                    ws.Cells["M267:O267"].Merge = true;
        //                    ws.Cells["M267:O267"].Value = "Troops Strength";
        //                    ws.Cells["P267:Q267"].Merge = true;
        //                    ws.Cells["R267:T267"].Merge = true;
        //                    ws.Cells["R267:T267"].Value = "$  " + string.Format("{0:N}", tropstgh);
        //                    ws.Cells["R267:T267"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

        //                    ws.Cells["M268:O268"].Merge = true;
        //                    ws.Cells["M268:O268"].Value = "Other Credit Notes";
        //                    ws.Cells["P268:Q268"].Merge = true;
        //                    ws.Cells["R268:T268"].Merge = true;
        //                    ws.Cells["R268:T268"].Value = "$  " + string.Format("{0:N}", OtherCreditNote);
        //                    ws.Cells["R268:T268"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

        //                    ws.Cells["M269:O269"].Merge = true;
        //                    ws.Cells["M269:O269"].Value = "Weekly Invoice";
        //                    ws.Cells["P269:Q269"].Merge = true;
        //                    ws.Cells["P269:Q269"].Value = "-0.35%";
        //                    ws.Cells["R269:T269"].Merge = true;
        //                    ws.Cells["R269:T269"].Value = "$  " + string.Format("{0:N}", Weeklywise);
        //                    ws.Cells["R269:T269"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

        //                    ws.Cells["M270:O270"].Merge = true;
        //                    ws.Cells["M270:O270"].Value = "Net amount for Rations";
        //                    ws.Cells["P270:Q270"].Merge = true;
        //                    ws.Cells["R270:T270"].Merge = true;
        //                    ws.Cells["R270:T270"].Value = "$  " + string.Format("{0:N}", NetRationAmt);
        //                    ws.Cells["R270:T270"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

        //                    ws.Cells["M271:O271"].Merge = true;
        //                    ws.Cells["M271:O271"].Value = "APL Deductions";
        //                    ws.Cells["P271:Q271"].Merge = true;
        //                    ws.Cells["R271:T271"].Merge = true;
        //                    ws.Cells["R271:T271"].Value = "- $  " + string.Format("{0:N}", AplDetect);
        //                    ws.Cells["R271:T271"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

        //                    ws.Cells["M272:T272"].Merge = true;
        //                    ws.Cells["M272:T272"].Value = "";

        //                    ws.Cells["M273:Q273"].Merge = true;
        //                    ws.Cells["M273:Q273"].Value = "Total Invoice";
        //                    ws.Cells["R273:T273"].Merge = true;
        //                    ws.Cells["R273:T273"].Value = "$  " + string.Format("{0:N}", TotalInvoice);
        //                    ws.Cells["R273:T273"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;


        //                    ws.Cells["M266:T273"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
        //                    ws.Cells["M266:T273"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
        //                    ws.Cells["M266:T273"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //                    ws.Cells["M266:T273"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

        //                    ws.Cells["E166:Z168"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

        //                    ws.Cells["J166:O168"].Style.Fill.PatternType = ExcelFillStyle.Solid;
        //                    ws.Cells["J166:O168"].Style.Fill.BackgroundColor.SetColor(WhiteHex);
        //                    ws.Cells["J166:O168"].Style.Font.Color.SetColor(OrangeHex);

        //                    ws.Cells["J166:O168,Y166:AC168,R166:U168,F225:F227,N227:O227,E225:F225,E226:F226,E227:F227"].Style.Numberformat.Format = "#,##0.00";


        //                    ws.Cells["Y166:AC168"].Style.Fill.PatternType = ExcelFillStyle.Solid;
        //                    ws.Cells["Y166:AC168"].Style.Fill.BackgroundColor.SetColor(WhiteHex);
        //                    ws.Cells["Y166:AC168"].Style.Font.Color.SetColor(OrangeHex);

        //                    ws.Cells["R166:U168"].Style.Fill.PatternType = ExcelFillStyle.Solid;
        //                    ws.Cells["R166:U168"].Style.Fill.BackgroundColor.SetColor(WhiteHex);
        //                    ws.Cells["R166:U168"].Style.Font.Color.SetColor(OrangeHex);


        //                    ws.Cells["P267:Q267"].Style.Fill.PatternType = ExcelFillStyle.Solid;
        //                    ws.Cells["P267:Q267"].Style.Fill.BackgroundColor.SetColor(BottomHex);

        //                    ws.Cells["M272:T272"].Style.Fill.PatternType = ExcelFillStyle.Solid;
        //                    ws.Cells["M272:T272"].Style.Fill.BackgroundColor.SetColor(AshHex);

        //                    ws.Cells["M266:T273"].Style.Font.Bold = true;
        //                    ws.Cells["M266:T266"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["T266:T273"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["M273:T273"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
        //                    ws.Cells["M266:M273"].Style.Border.Left.Style = ExcelBorderStyle.Medium;

        //                    ws.Cells["A274"].Value = "Disclaimer:";
        //                    ws.Cells["A275"].Value = "In the interest of ensuring a smooth invoicing/payment process, GCC SERVICES herewith signs this GRR with the intent to officially review the Weekly Billing Discount and APL formulas.  We will submit a correction/recovery request as applicable.";

        //                    ws.Cells["J16:Z165,E174:F194,J174:P194,E199:F219,J199:P219"].Style.Numberformat.Format = "#,##0.00";

        //                    x = x + 1;
        //                    #endregion
        //                }
        //            }
        //            string txtTName = InvoiceNo;
        //            //Write it back to the client
        //            if (Generate == true) // flag used to Generate Consolidate and Single for Update the Document in the table
        //            {
        //                Response.Clear();
        //                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //                Response.AddHeader("content-disposition", "attachment;  filename=" + txtTName + ".xlsx");
        //            }
        //            if ((stack != null) && (Single == true))
        //            {
        //                string path = Response.OutputStream.ToString();
        //                byte[] data = pck.GetAsByteArray();


        //                ExcelDocuments XD = IS.GetExcelDocumentsDetailsByControlId(stack[0].UNID);
        //                if (XD != null)
        //                {
        //                    XD.DocumentData = data;
        //                    long id = IS.SaveOrUpdateExcelDocuments(XD, userId);
        //                    // SaveDocumentToRecentDownloads(XD, null, string.Empty, null, string.Empty);
        //                }
        //                else if (Single == true)
        //                {

        //                    InvoiceManagementView Imv = IS.GetInvoiceManagementViewDetailsByControlId(stack[0].UNID);
        //                    ExcelDocuments ed = new ExcelDocuments();
        //                    ed.ControlId = Imv.ControlId;
        //                    ed.OrderId = Imv.OrderId;
        //                    ed.InvoiceId = Imv.Id;
        //                    ed.Location = Imv.Location;
        //                    ed.Name = Imv.Name;
        //                    ed.ContingentType = Imv.ContingentType;
        //                    ed.Period = Imv.Period;
        //                    ed.PeriodYear = Imv.PeriodYear;
        //                    ed.Sector = Imv.Sector;
        //                    ed.Week = Imv.Week;
        //                    ed.DocumentData = data;
        //                    ed.DocumentType = "Excel-Single";
        //                    ed.DocumentName = stack[0].Reference;
        //                    long id = IS.SaveOrUpdateExcelDocuments(ed, userId);
        //                    //SaveDocumentToRecentDownloads(ed, null, string.Empty, null, string.Empty);
        //                }
        //                if (Generate == true) // flag used to Generate Consolidate and Single for Update the Document in the table
        //                {
        //                    Response.BinaryWrite(data);
        //                    Response.End();
        //                }
        //            }
        //            else if ((invoiceList != null) && (Consol == true))
        //            {

        //                InvoiceList ci = (InvoiceList)invoiceList;
        //                var InvNoSplit = ci.InvoiceNo.ToString().Split('-');
        //                var Period = ci.Period.ToString().Split('/');
        //                //to check weather cosolidate sheet is already exsist or not
        //                ExcelDocuments ED = GetExcelDocumentForConsolidate(InvNoSplit[1], InvNoSplit[3], Period[0], Period[1]);

        //                byte[] data = pck.GetAsByteArray();

        //                if (ED != null && ED.Id > 0)
        //                {
        //                    ED.DocumentData = data;
        //                    long id = IS.SaveOrUpdateExcelDocuments(ED, userId);
        //                    //SaveDocumentToRecentDownloads(ED, null, string.Empty, null, string.Empty);
        //                }
        //                else
        //                {
        //                    ED.ControlId = ci.InvoiceNo;
        //                    ED.ContingentType = InvNoSplit[3];
        //                    ED.Period = Period[0];
        //                    ED.PeriodYear = Period[1];
        //                    ED.Sector = InvNoSplit[1];
        //                    ED.DocumentData = data;
        //                    ED.DocumentType = "Excel-Consol";
        //                    ED.DocumentName = ci.InvoiceNo;
        //                    long id = IS.SaveOrUpdateExcelDocuments(ED, userId);
        //                    //SaveDocumentToRecentDownloads(ED, null, string.Empty, null, string.Empty);
        //                }
        //                if (Generate == true) // flag used to Generate Consolidate and Single for Update the Document in the table
        //                {
        //                    Response.BinaryWrite(data);
        //                    Response.End();
        //                }
        //            }
        //            else
        //            {
        //                if (Generate == true) // flag used to Generate Consolidate and Single for Update the Document in the table
        //                {
        //                    Response.BinaryWrite(pck.GetAsByteArray());
        //                    Response.End();
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        #endregion

        /// <summary>
        /// To check weather cosolidate sheet is already exsist or not in Excel Documents table
        /// </summary>
        /// <param name="Sector"></param>
        /// <param name="ContingentType"></param>
        /// <param name="Period"></param>
        /// <param name="Year"></param>
        /// <returns> ExcelDocuments </returns>
        private ExcelDocuments GetExcelDocumentForConsolidate(string Sector, string ContingentType, string Period, string Year)
        {
            try
            {
                ExcelDocuments ED = new ExcelDocuments();
                criteria.Clear();
                if (!string.IsNullOrWhiteSpace(Sector)) { criteria.Add("Sector", Sector); }
                if (!string.IsNullOrWhiteSpace(ContingentType)) { criteria.Add("ContingentType", ContingentType); }
                if (!string.IsNullOrWhiteSpace(Period)) { criteria.Add("Period", Period); }
                if (!string.IsNullOrWhiteSpace(Year)) { criteria.Add("PeriodYear", Year); }
                if (!string.IsNullOrWhiteSpace(Year)) { criteria.Add("DocumentType", "Excel-Consol"); }
                IList<ExcelDocuments> DocumentItemsList = null;
                Dictionary<long, IList<ExcelDocuments>> DocumentItems = IS.GetExcelDocumentListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                if (DocumentItems != null && DocumentItems.First().Key > 0)
                {
                    DocumentItemsList = DocumentItems.FirstOrDefault().Value;
                    ED.Id = DocumentItemsList[0].Id;
                    ED.Sector = DocumentItemsList[0].Sector;
                    ED.ContingentType = DocumentItemsList[0].ContingentType;
                    ED.Period = DocumentItemsList[0].Period;
                    ED.PeriodYear = DocumentItemsList[0].PeriodYear;
                    ED.DocumentType = DocumentItemsList[0].DocumentType;
                    ED.DocumentData = DocumentItemsList[0].DocumentData;
                    ED.DocumentFor = DocumentItemsList[0].DocumentFor;
                    ED.DocumentName = DocumentItemsList[0].DocumentName;
                    ED.DocumentSize = DocumentItemsList[0].DocumentSize;
                    ED.ControlId = DocumentItemsList[0].ControlId;
                }
                return ED;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public DataTable CreateDataSetSingle<T>(List<T> list)
        {
            //list is nothing or has nothing, return nothing (or add exception handling)
            if (list == null || list.Count == 0) { return null; }

            //get the type of the first obj in the list
            var obj = list[0].GetType();

            //now grab all properties
            var properties = obj.GetProperties();

            //make sure the obj has properties, return nothing (or add exception handling)
            if (properties.Length == 0) { return null; }

            //it does so create the dataset and table
            var dataTable = new System.Data.DataTable();

            //now build the columns from the properties
            var columns = new DataColumn[properties.Length];
            for (int i = 0; i < properties.Length; i++)
            {
                columns[i] = new DataColumn(properties[i].Name, properties[i].PropertyType);
            }

            //add columns to table
            dataTable.Columns.AddRange(columns);

            //now add the list values to the table
            foreach (var item in list)
            {
                //create a new row from table
                var dataRow = dataTable.NewRow();

                //now we have to iterate thru each property of the item and retrieve it's value for the corresponding row's cell
                var itemProperties = item.GetType().GetProperties();

                for (int i = 0; i < itemProperties.Length; i++)
                {
                    dataRow[i] = itemProperties[i].GetValue(item, null);
                }

                //now add the populated row to the table
                dataTable.Rows.Add(dataRow);
            }

            //add table to dataset
            //return dataset
            return dataTable;
        }
        public DataTable CreateDataSetConsolidate<T>(List<T> list)
        {
            //list is nothing or has nothing, return nothing (or add exception handling)
            if (list == null || list.Count == 0) { return null; }

            //get the type of the first obj in the list
            var obj = list[0].GetType();

            //now grab all properties
            var properties = obj.GetProperties();

            //make sure the obj has properties, return nothing (or add exception handling)
            if (properties.Length == 0) { return null; }

            //it does so create the dataset and table
            var dataTable = new System.Data.DataTable();

            //now build the columns from the properties
            var columns = new DataColumn[properties.Length];
            for (int i = 0; i < properties.Length; i++)
            {
                columns[i] = new DataColumn(properties[i].Name, properties[i].PropertyType);
            }

            //add columns to table
            dataTable.Columns.AddRange(columns);

            //now add the list values to the table
            foreach (var item in list)
            {
                //create a new row from table
                var dataRow = dataTable.NewRow();

                //now we have to iterate thru each property of the item and retrieve it's value for the corresponding row's cell
                var itemProperties = item.GetType().GetProperties();

                for (int i = 0; i < itemProperties.Length; i++)
                {
                    dataRow[i] = itemProperties[i].GetValue(item, null);
                }

                //now add the populated row to the table
                dataTable.Rows.Add(dataRow);
            }

            //add table to dataset
            //return dataset
            return dataTable;
        }
        /// <summary>
        /// Weekly Consolidate Excel Invoice Generation
        /// </summary>
        /// <param name="Period"></param>
        /// <param name="searchItems"></param>
        /// <returns></returns>
        public InvoiceList ConsolidateInvoiceList(string Period, string searchItems)
        {
            try
            {

                if ((Period != null) && (searchItems != null))
                {
                    /// <summary>
                    /// Check APL Performance switch On/Off
                    /// </summary>
                    bool IsAPL = Convert.ToBoolean(ConfigurationManager.AppSettings["IsAPL"]);

                    Dictionary<long, IList<InvoiceReports>> DictInvRpt = null;

                    //get the weeks for the period from period master
                    //get the orders/POD for the period
                    //both count should match
                    //if true proceed, else find where it is missing
                    criteria.Clear();
                    var Items = searchItems.ToString().Split(',');
                    if (searchItems != null && searchItems != "")
                    {
                        if (!string.IsNullOrWhiteSpace(Items[0])) { criteria.Add("Sector", Items[0]); }
                        if (!string.IsNullOrWhiteSpace(Items[1])) { criteria.Add("Name", Items[1]); }
                        if (!string.IsNullOrWhiteSpace(Items[2])) { criteria.Add("ContingentType", Items[2]); }
                        if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Period", Items[3]); }
                        if (!string.IsNullOrWhiteSpace(Items[4])) { criteria.Add("PeriodYear", Items[4]); }
                        if (Items.Length > 5)
                            if (!string.IsNullOrWhiteSpace(Items[5])) { criteria.Add("Week", Convert.ToInt64(Items[5])); }
                        var TrimItems = searchItems.Replace(",", "");
                        if (string.IsNullOrWhiteSpace(TrimItems))
                            return null;
                    }
                    Dictionary<long, IList<InvoiceManagementView>> invoiceItems = IS.GetInvoiceListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);

                    var orderList = (from items in invoiceItems.First().Value
                                     select items.OrderId).OrderBy(i => i).Distinct().ToArray();


                    criteria.Add("ReportType", "ORDINV");
                    DictInvRpt = IS.GetInvoiceReportsListWithPagingAndCriteria(0, 9999, "ControlId", string.Empty, criteria);



                    //Dictionary<long, IList<PerformanceCalculateView>> PCal = IS.GetPerformanceCalculateListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);

                    var PeriodList = (from items in invoiceItems.First().Value
                                      select items.Week).OrderBy(i => i).Distinct().ToList();
                    long troops = ((invoiceItems.First().Value.Sum(item => (long)item.TotalFeedTroopStrength)) / PeriodList.Count());
                    long Mandays = troops * (PeriodList.Count() * 7);


                    decimal TotDelivery = DictInvRpt.First().Value.Sum(item => item.AplTimelydelivery);
                    decimal TotOrderLineItems = DictInvRpt.First().Value.Sum(item => item.AplOrderbylineitems);
                    decimal TotOrderByWeight = DictInvRpt.First().Value.Sum(item => item.AplOrdersbyweight);
                    decimal TotComplainAvalability = DictInvRpt.First().Value.Sum(item => item.AplNoofauthorizedsubstitutions);

                    //Weekly Discount added by Thamizh
                    decimal TotWeeklyDiscount = DictInvRpt.First().Value.Sum(item => item.Weeklyinvoicediscount);
                    decimal TotalDetection = TotDelivery + TotOrderLineItems + TotOrderByWeight + TotComplainAvalability + TotWeeklyDiscount;

                    //decimal TotalDetection = TotDelivery + TotOrderLineItems + TotOrderByWeight + TotComplainAvalability;


                    if (invoiceItems.First().Key != 0)
                    {
                        // Written by Felix kinoniya
                        decimal subTotal = 0;
                        decimal grandTotal = 0;
                        IList<PeriodWeek> periodListVal = GetPeriodListVal(Items[0], Items[2], Items[3], Items[4], Convert.ToInt64(Items[5]));// sector,ContingentType,period
                        if (periodListVal != null)
                        {
                            var SubVal = (from items in periodListVal
                                          group items by new { items.InvoiceValue } into g
                                          select g.Sum(items => Convert.ToDecimal(items.InvoiceValue))).ToList();
                            if (SubVal.Count != 0)
                            {

                                for (int i = 0; i < SubVal.Count; i++)
                                {
                                    subTotal += SubVal[i]; grandTotal += SubVal[i];
                                }
                            }
                        }


                        List<PeriodWeek> periodListValMain = new List<PeriodWeek>();

                        int length = periodListVal.Count();
                        for (int i = 0; i < length; i++)
                        {
                            PeriodWeek list = new PeriodWeek();
                            list.InvoiceValue = string.Format("{0:N}", Math.Round(Convert.ToDecimal(periodListVal[i].InvoiceValue), 2));
                            list.AcceptedQty = string.Format("{0:N}", Math.Round(Convert.ToDecimal(periodListVal[i].AcceptedQty), 2));
                            list.Week = periodListVal[i].Week;
                            periodListValMain.Add(list);
                        }

                        //grandTotal = Math.Round((grandTotal - TotalDetection), 2);
                        //var values = grandTotal.ToString(CultureInfo.InvariantCulture).Split('.');
                        //int firstValue = int.Parse(values[0]);
                        //int secondValue = int.Parse(values[1]);

                        int firstValue = 0;
                        int secondValue = 0;
                        if (IsAPL == true)
                        {
                            grandTotal = Math.Round((grandTotal - TotalDetection), 2);
                            var values = grandTotal.ToString(CultureInfo.InvariantCulture).Split('.');
                            firstValue = int.Parse(values[0]);
                            secondValue = int.Parse(values[1]);
                        }
                        else
                        {
                            var values = periodListValMain[0].InvoiceValue.ToString(CultureInfo.InvariantCulture).Split('.');
                            firstValue = int.Parse(values[0].Replace(",", ""));
                            secondValue = int.Parse(values[1]);
                        }


                        Dictionary<long, IList<SectorMaster>> SectorList = null;
                        criteria.Clear();
                        criteria.Add("SectorCode", Items[0]);
                        SectorList = MS.GetSectorMasterListWithLikeSearchPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                        var tempSector = SectorList.FirstOrDefault().Value[0].Description;

                        DateTime Invdt = invoiceItems.FirstOrDefault().Value[0].InvoiceDate;

                        int no = GetSerialNoforPeriod(Period, Items[4], Convert.ToInt64(Items[5]));
                        string SectorNo = "";
                        string SectorName = "";
                        if (Items[0] == "FS")
                        {
                            SectorNo = "-090-";
                            SectorName = "FS";
                        }
                        else if (Items[0] == "NS")
                        {
                            SectorNo = "-091-";
                            SectorName = "NS";
                        }
                        else
                        {
                            SectorNo = "-092-";
                            SectorName = "GS";
                        }

                        //DateTime dt = DateTime.Parse("1/12/2014");

                        InvoiceNumberMaster InvNumber = OS.GetInvoiceNumberByOrderId(orderList[0]);
                        string[] myArray = new string[10];
                        if (InvNumber != null)
                            myArray = InvNumber.InvoiceNumber.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                        return new InvoiceList()
                        {
                            //InvoiceNo = "GCC-" + Items[0] + SectorNo + Items[2] + "-0" + no + "/" + Items[5],
                            InvoiceNo = myArray.Length > 0 ? myArray[0] : InvNumber.InvoiceNumber,
                            Contract = "PD/C0036/13",
                            InvoiceDate = String.Format(new DateFormatInseries(), "{0}", Invdt),
                            PayTerms = "30 Days",
                            //PO = "MID14-61 - 2400001817", for P13
                            PO = GetPONumberFromMaster(Period, Items[4], "Food"),
                            Period = Period + "/" + Items[4],
                            Sector = SectorName,
                            TotMadays = Mandays.ToString(),
                            TotalFeedingToop = troops.ToString(),
                            UnINo = Items[2],
                            CMR = Math.Round(((grandTotal) / Mandays), 2).ToString(),
                            SubTotal = grandTotal.ToString(),
                            GrandTotal = grandTotal.ToString(),
                            //SubTotal = string.Format("{0:C}", (grandTotal)),
                            //GrandTotal = string.Format("{0:C}", (grandTotal)),
                            Usd_words = NumberToText(Convert.ToInt64(firstValue)) + " " + "Dollars" + " " + secondValue + "/100",
                            Delivery = TotDelivery.ToString(),
                            OrderLineItems = TotOrderLineItems.ToString(),
                            OrderByWeight = TotOrderByWeight.ToString(),
                            ComplainAvalability = TotComplainAvalability.ToString(),
                            //Delivery = string.Format("{0:C}", TotDelivery),
                            //OrderLineItems = string.Format("{0:C}", TotOrderLineItems),
                            //OrderByWeight = string.Format("{0:C}", TotOrderByWeight),
                            //ComplainAvalability = string.Format("{0:C}", TotComplainAvalability),
                            PeriodWeek = periodListValMain,
                            WeeklyDiscount = TotWeeklyDiscount,
                            IsAPL = IsAPL
                        };
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
        public SingleInvoice SingleInvoiceList(long OrderId)
        {
            try
            {
                Orders Ord = OS.GetOrdersById(OrderId);
                DateTime ApprovedDeliverydate = IS.GetApprovedDeliveryDateById(OrderId);
                criteria.Clear();
                criteria.Add("OrderId", OrderId);

                Dictionary<long, IList<SingleInvoiceView>> SingleInvoice2 = IS.GetSingleInvoiceListUsingSP(OrderId);


                //Dictionary<long, IList<SingleInvoiceView>> SingleInvoice2 = IS.GetSingleInvoiceListWithPagingAndCriteria(0, 9999, "UNCode", "Asc", criteria);
                Dictionary<long, IList<SingleInvoiceView>> SingleInvoice = new Dictionary<long, IList<SingleInvoiceView>>();
                long[] LineIds = (from items in SingleInvoice2.First().Value
                                  select items.LineId).Distinct().ToArray();
                criteria.Add("LineId", LineIds);
                Dictionary<long, IList<OrderItems>> OrderList = OS.GetOrderItemsListWithNotInSearchPagingAndCriteria(0, 9999, string.Empty, string.Empty, string.Empty, null, criteria);
                criteria.Clear();
                Dictionary<long, IList<UOMMaster>> UOMMasterList = MS.GetUOMMasterListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);

                List<SingleInvoiceView> OrdList = SingleInvoice2.FirstOrDefault().Value.ToList();

                if (OrderList != null && OrderList.FirstOrDefault().Key > 0)
                {
                    long Count = 1;
                    foreach (var item in OrderList.FirstOrDefault().Value)
                    {

                        string[] UomStr = (from items in UOMMasterList.FirstOrDefault().Value
                                           where items.UNCode == item.UNCode
                                           select items.UOM).Distinct().ToArray();
                        SingleInvoiceView sw = new SingleInvoiceView();
                        sw.LineId = item.LineId;
                        sw.OrderQty = item.OrderQty;
                        sw.UNCode = item.UNCode;
                        sw.Commodity = item.Commodity;
                        sw.DeliveredOrdQty = 0;
                        sw.InvoiceQty = 0;
                        sw.NetAmt = 0;
                        sw.SectorPrice = item.SectorPrice;
                        sw.APLWeight = 0;
                        //sw.DiscrepancyCode = " ";
                        sw.DiscrepancyCode = item.NPACode;
                        sw.NPACode = item.NPACode;
                        if (UomStr != null && UomStr.Length != 0)
                            sw.UOM = UomStr[0];
                        else
                            sw.UOM = string.Empty;
                        sw.OrderValue = Math.Round((item.OrderQty * item.SectorPrice), 2);
                        sw.DeliveryNote = " ";
                        OrdList.Add(sw);
                        Count = Count + 1;
                    }
                }
                SingleInvoice.Add(OrdList.Count(), OrdList);

                IList<SingleInvoiceView> SingleInvoiceList = OrdList;
                IList<SingleInvoiceView> SingleInvoiceList1 = null;
                Dictionary<long, IList<SingleInvoiceView>> SingleInvoice1 = SingleInvoice;
                SingleInvoiceList1 = SingleInvoice1.FirstOrDefault().Value;
                //To find the substitutions qty
                //criteria.Clear();
                //criteria.Add("OrderId", OrderId);
                //Dictionary<long, IList<SubReplacementView>> SubReplaceList = IS.GetSubstitudeReplacementList(0, 9999, "UNCode", string.Empty, criteria);

                Dictionary<long, IList<SubReplacementView>> SubReplaceList = IS.GetSubstitudeReplacementListUsingSP(OrderId);

                criteria.Clear();
                Dictionary<long, IList<ItemMaster>> ItemMasterList = MS.GetItemMasterListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);



                int temp = 1;
                for (int i = 0; i < SingleInvoice1.FirstOrDefault().Value.Count; i++)
                {

                    if (SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode == "AS")
                    {
                        SingleInvoice1.FirstOrDefault().Value[i].DeliveredOrdQty = (decimal)0.00;
                        SingleInvoice1.FirstOrDefault().Value[i].AcceptedOrdQty = (decimal)0.00;
                        SingleInvoice1.FirstOrDefault().Value[i].InvoiceQty = (decimal)0.00;
                        SingleInvoice1.FirstOrDefault().Value[i].NetAmt = (decimal)0.00;
                        SingleInvoice1.FirstOrDefault().Value[i].APLWeight = (decimal)0.00;
                    }
                    else if (SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode == "AR")
                    {
                        SingleInvoice1.FirstOrDefault().Value[i].DeliveredOrdQty = (decimal)0.00;
                        SingleInvoice1.FirstOrDefault().Value[i].AcceptedOrdQty = (decimal)0.00;
                        SingleInvoice1.FirstOrDefault().Value[i].InvoiceQty = (decimal)0.00;
                        SingleInvoice1.FirstOrDefault().Value[i].NetAmt = (decimal)0.00;
                        SingleInvoice1.FirstOrDefault().Value[i].APLWeight = (decimal)0.00;
                    }
                    else if (SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode == "NPA/04")//For UNSubstitution
                    {
                        SingleInvoice1.FirstOrDefault().Value[i].DeliveredOrdQty = (decimal)0.00;
                        SingleInvoice1.FirstOrDefault().Value[i].AcceptedOrdQty = (decimal)0.00;
                        SingleInvoice1.FirstOrDefault().Value[i].InvoiceQty = (decimal)0.00;
                        SingleInvoice1.FirstOrDefault().Value[i].NetAmt = (decimal)0.00;
                        SingleInvoice1.FirstOrDefault().Value[i].APLWeight = (decimal)0.00;
                    }
                    else if (SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode == "OR")
                    {
                        SingleInvoice1.FirstOrDefault().Value[i].DeliveredOrdQty = (decimal)0.00;
                        SingleInvoice1.FirstOrDefault().Value[i].AcceptedOrdQty = (decimal)0.00;
                        SingleInvoice1.FirstOrDefault().Value[i].InvoiceQty = (decimal)0.00;
                        SingleInvoice1.FirstOrDefault().Value[i].NetAmt = (decimal)0.00;
                        SingleInvoice1.FirstOrDefault().Value[i].APLWeight = (decimal)0.00;

                        var TempCode = (from items in SubReplaceList.First().Value
                                        where items.DiscrepancyCode == "OR" && items.UNCode == SingleInvoice1.FirstOrDefault().Value[i].UNCode
                                        select items.DiscCode).ToString();

                        if (TempCode == "SP") { SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode = "AS"; }
                        else if (TempCode == "UNAS") { SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode = "NPA/04"; }//For UNSubstitution
                        else { SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode = "AR"; }
                    }
                    else if (SingleInvoice1.FirstOrDefault().Value[i].UNCode == 1129)
                    {

                        //SingleInvoice1.FirstOrDefault().Value[i].InvoiceQty = Math.Round(SingleInvoice1.FirstOrDefault().Value[i].InvoiceQty / (decimal)0.058824, 0);
                        SingleInvoice1.FirstOrDefault().Value[i].InvoiceQty = (Math.Truncate(SingleInvoice1.FirstOrDefault().Value[i].InvoiceQty * 1000m) / 1000m) / (decimal)0.058824;

                        //SingleInvoice1.FirstOrDefault().Value[i].OrderQty = (Math.Truncate(SingleInvoice1.FirstOrDefault().Value[i].OrderQty * 1000m) / 1000m) / (decimal)0.058824;
                        //SingleInvoice1.FirstOrDefault().Value[i].DeliveredOrdQty = (Math.Truncate(SingleInvoice1.FirstOrDefault().Value[i].DeliveredOrdQty * 1000m) / 1000m) / (decimal)0.058824;
                        //SingleInvoice1.FirstOrDefault().Value[i].AcceptedOrdQty = (Math.Truncate(SingleInvoice1.FirstOrDefault().Value[i].AcceptedOrdQty * 1000m) / 1000m) / (decimal)0.058824;


                        SingleInvoice1.FirstOrDefault().Value[i].NetAmt = SingleInvoice1.FirstOrDefault().Value[i].InvoiceQty * SingleInvoice1.FirstOrDefault().Value[i].SectorPrice;
                        SingleInvoice1.FirstOrDefault().Value[i].OrderValue = SingleInvoice1.FirstOrDefault().Value[i].OrderQty * SingleInvoice1.FirstOrDefault().Value[i].SectorPrice;

                        if (SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode == "NPA 01/02")
                        {
                            SingleInvoice1.FirstOrDefault().Value[i].APLWeight = (decimal)98.00;
                        }
                        else
                        {
                            if (SingleInvoice1.FirstOrDefault().Value[i].OrderQty == (decimal)0.00 || SingleInvoice1.FirstOrDefault().Value[i].InvoiceQty == (decimal)0.00)
                            {
                                SingleInvoice1.FirstOrDefault().Value[i].APLWeight = (decimal)0.00;
                            }
                            else { SingleInvoice1.FirstOrDefault().Value[i].APLWeight = (SingleInvoice1.FirstOrDefault().Value[i].InvoiceQty / SingleInvoice1.FirstOrDefault().Value[i].OrderQty) * 100; }
                        }


                    }

                    //if ( SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode != "AS" && SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode != "AR" && SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode != "NPA/04" && SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode != "OR" && SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode != "NPA02" && SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode != "NPA01")
                    //{
                    //    SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode = SingleInvoice1.FirstOrDefault().Value[i].NPACode != "NCDD" ? SingleInvoice1.FirstOrDefault().Value[i].NPACode : "";
                    //}
                    if (SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode != "AS" && SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode != "AR" && SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode != "OR" && SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode != "NPA/04")
                    {
                        SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode = SingleInvoice1.FirstOrDefault().Value[i].NPACode != "NCDD" ? SingleInvoice1.FirstOrDefault().Value[i].NPACode : "";
                    }
                    if (SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode == "AS" || SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode == "AR" || SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode == "NPA/04")
                    {
                        if (!string.IsNullOrEmpty(SingleInvoice1.FirstOrDefault().Value[i].NPACode) && SingleInvoice1.FirstOrDefault().Value[i].NPACode != "NCDD")
                            SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode = SingleInvoice1.FirstOrDefault().Value[i].NPACode != "NCDD" ? SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode + "/" + SingleInvoice1.FirstOrDefault().Value[i].NPACode : SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode;
                    }
                    if (!string.IsNullOrEmpty(SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode) && SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode != "AS" && SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode != "AR" && SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode != "OR" && SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode != "NPA03" && SingleInvoice1.FirstOrDefault().Value[i].APLWeight < (decimal)98.00)
                    {
                        if (SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode != null && SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode != "NPA/04" && SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode.Contains("NPA02") == true || SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode.Contains("NPA01") == true && SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode.Contains("NPA/04") != true)
                            SingleInvoice1.FirstOrDefault().Value[i].APLWeight = (decimal)98.00;
                    }
                    if (SingleInvoice1.FirstOrDefault().Value[i].APLWeight >= (decimal)102.00)
                    {
                        SingleInvoice1.FirstOrDefault().Value[i].InvoiceQty = Math.Truncate(SingleInvoice1.FirstOrDefault().Value[i].InvoiceQty * 1000m) / 1000m;
                    }
                    SingleInvoice1.FirstOrDefault().Value[i].Id = temp;
                    temp = temp + 1;
                }
                List<SingleInvoiceView> DeliverySingleList = (from items in SingleInvoice1.FirstOrDefault().Value
                                                              select items).OrderBy(i => i.UNCode).Distinct().ToList();

                decimal ordrQtySum = SingleInvoiceList1.Sum(x => x.OrderQty);



                decimal TemOrderedQtySum = 0;
                decimal TempDeliveredQtySum = 0;
                decimal TempAcceptedQtySum = 0;
                decimal TempInvoiceQtySum = 0;

                decimal TempAboveAplWeight = 0;
                decimal TempBelowAplWeight = 0;

                decimal TempEggOrderedQtySum = 0;
                decimal TempEggDeliveredQtySum = 0;
                decimal TempEggAcceptedQtySum = 0;
                decimal TempEggInvoiceQtySum = 0;

                decimal NPAOrderedQtySum = 0;
                decimal NPADeliveredQtySum = 0;
                decimal NPAAcceptedQtySum = 0;
                decimal NPAInvoiceQtySum = 0;
                decimal NPAEggOrderedQtySum = 0;
                decimal NPAEggDeliveredQtySum = 0;
                decimal NPAEggAcceptedQtySum = 0;
                decimal NPAEggInvoiceQtySum = 0;

                TransportInvoice TrnsprtInv = IS.GetTransportInvoiceDetailsByOrderId(OrderId);

                decimal TotalTransportationCost = 0;


                long count = SingleInvoice.First().Key;
                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {

                        if (SingleInvoice1.First().Value[i].UNCode != 1129)
                        {
                            TemOrderedQtySum = TemOrderedQtySum + SingleInvoice1.First().Value[i].OrderQty;
                            TempDeliveredQtySum = TempDeliveredQtySum + SingleInvoice1.First().Value[i].DeliveredOrdQty;
                            TempAcceptedQtySum = TempAcceptedQtySum + SingleInvoice1.First().Value[i].AcceptedOrdQty;
                            TempInvoiceQtySum = TempInvoiceQtySum + SingleInvoice1.First().Value[i].InvoiceQty;
                            #region Transportation cost calculation
                            if (TrnsprtInv != null && TrnsprtInv.Id > 0)
                                TotalTransportationCost = TotalTransportationCost + Math.Round((SingleInvoice1.First().Value[i].InvoiceQty * TrnsprtInv.RatePerKg), 2, MidpointRounding.AwayFromZero);
                            #endregion
                        }
                        else
                        {


                            TempEggOrderedQtySum = TempEggOrderedQtySum + SingleInvoice1.First().Value[i].OrderQty;
                            TempEggDeliveredQtySum = TempEggDeliveredQtySum + SingleInvoice1.First().Value[i].DeliveredOrdQty;
                            TempEggAcceptedQtySum = (TempEggAcceptedQtySum + SingleInvoice1.First().Value[i].AcceptedOrdQty);
                            TempEggInvoiceQtySum = (TempEggInvoiceQtySum + SingleInvoice1.First().Value[i].InvoiceQty);
                            #region Transportation cost calculation
                            if (TrnsprtInv != null && TrnsprtInv.Id > 0)
                                TotalTransportationCost = TotalTransportationCost + Math.Round(((SingleInvoice1.First().Value[i].InvoiceQty * (decimal)0.058824) * TrnsprtInv.RatePerKg), 2, MidpointRounding.AwayFromZero);
                            #endregion
                        }
                        #region NPA Ordered,Delivered,Accepted and Inoice Qty
                        if (!string.IsNullOrEmpty(SingleInvoice1.First().Value[i].NPACode) && SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode.Contains("NPA") == true && !SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode.Contains("NPA") == true)
                        {
                            if (SingleInvoice1.First().Value[i].UNCode != 1129)
                            {
                                NPAOrderedQtySum = NPAOrderedQtySum + SingleInvoice1.First().Value[i].OrderQty;
                                NPADeliveredQtySum = NPADeliveredQtySum + SingleInvoice1.First().Value[i].DeliveredOrdQty;
                                NPAAcceptedQtySum = NPAAcceptedQtySum + SingleInvoice1.First().Value[i].AcceptedOrdQty;
                                NPAInvoiceQtySum = NPAInvoiceQtySum + SingleInvoice1.First().Value[i].InvoiceQty;
                            }
                            else
                            {
                                NPAEggOrderedQtySum = NPAEggOrderedQtySum + SingleInvoice1.First().Value[i].OrderQty;
                                NPAEggDeliveredQtySum = NPAEggDeliveredQtySum + SingleInvoice1.First().Value[i].DeliveredOrdQty;
                                NPAEggAcceptedQtySum = (NPAEggAcceptedQtySum + SingleInvoice1.First().Value[i].AcceptedOrdQty);
                                NPAEggInvoiceQtySum = (NPAEggInvoiceQtySum + SingleInvoice1.First().Value[i].InvoiceQty);
                            }
                        }
                        #endregion


                    }
                }
                decimal OrderedQtySum = TemOrderedQtySum;
                decimal DeliveredQtySum = TempDeliveredQtySum;
                decimal AcceptedQtySum = TempAcceptedQtySum;
                decimal InvoiceQtySum = TempInvoiceQtySum;

                decimal NetAmountSum = SingleInvoiceList1.Sum(x => x.NetAmt);
                decimal OrdervalueSum = Math.Round(SingleInvoiceList1.Sum(x => x.OrderValue), 2);
                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        if ((SingleInvoice1.First().Value[i].DiscrepancyCode != "AS") && (SingleInvoice1.First().Value[i].DiscrepancyCode != "AR") && (SingleInvoice1.First().Value[i].DiscrepancyCode != "NPA/04") && (SingleInvoice1.First().Value[i].APLWeight) >= ((decimal)98))
                        {
                            TempAboveAplWeight = TempAboveAplWeight + 1;
                        }
                        else
                        {
                            TempBelowAplWeight = TempBelowAplWeight + 1;
                        }
                    }
                }
                decimal AboveAplWeightCount = TempAboveAplWeight;
                decimal BelowAplWeightCount = TempBelowAplWeight;
                decimal TotalAplWeightCount = AboveAplWeightCount + BelowAplWeightCount;

                decimal EggOrderedQtySum = TempEggOrderedQtySum * (decimal)0.058824;
                decimal EggDeliveredQtySum = TempEggDeliveredQtySum * (decimal)0.058824;
                decimal EggAcceptedQtySum = TempEggAcceptedQtySum * (decimal)0.058824;
                decimal EggInvoiceQtySum = TempEggInvoiceQtySum * (decimal)0.058824;


                ///Egg without Kg Conversion
                //decimal EggOrderedQtySum = TempEggOrderedQtySum ;
                //decimal EggDeliveredQtySum = TempEggDeliveredQtySum ;
                //decimal EggAcceptedQtySum = TempEggAcceptedQtySum ;
                //decimal EggInvoiceQtySum = TempEggInvoiceQtySum;

                decimal TotalOrderedQtySum = OrderedQtySum + Math.Truncate(EggOrderedQtySum * 1000m) / 1000m;
                decimal TotalDeliveredQtySum = DeliveredQtySum + Math.Truncate(EggDeliveredQtySum * 1000m) / 1000m;
                decimal TotalAcceptedQtySum = AcceptedQtySum + Math.Truncate(EggAcceptedQtySum * 1000m) / 1000m;
                decimal TotalInvoiceQtySum = InvoiceQtySum + EggInvoiceQtySum;

                //Sum of NPA Items
                decimal TotalNPAOrderedQtySum = NPAOrderedQtySum + (NPAEggOrderedQtySum * (decimal)0.058824);
                decimal TotalNPAInvoicetySum = NPAInvoiceQtySum + (NPAEggInvoiceQtySum * (decimal)0.058824);


                //Replacement
                IList<SubReplacementView> DeliveryList = (from items in SubReplaceList.First().Value
                                                          select items).OrderBy(i => i.UNCode).Distinct().ToList();


                #region DeliveryList Change for Partial Orders items has OR,AR and AS

                var Uncode = (from items in SubReplaceList.First().Value
                              where items.DiscrepancyCode == "OR"
                              select items.UNCode).Distinct().ToArray();

                if (Uncode.Count() > 0)
                {

                    foreach (var item in Uncode)
                    {
                        decimal[] ApCode = (from items in ItemMasterList.First().Value
                                            where items.UNCode == item
                                            select items.APLCode).Distinct().ToArray();

                        var Listcode = (from items in DeliveryList
                                        where items.UNCode == item
                                        select items.DiscrepancyCode).Distinct().ToArray();

                        //decimal OrderQty = DeliveryList.Where(p => p.UNCode == item).Sum(x => x.OrderedQty);
                        decimal OrderQty = DeliverySingleList.Where(p => p.UNCode == item).Sum(x => x.OrderQty);
                        decimal DelQty = DeliveryList.Where(p => p.UNCode == item).Sum(x => x.DeliveredQty);
                        decimal InvQty = DeliveryList.Where(p => p.UNCode == item).Sum(x => x.InvoiceQty);

                        decimal Difference = OrderQty - DelQty;  //To check whether Actual packsize is greater than Delivery diffrence
                        decimal APLweight = 0;
                        string discCode = "";
                        decimal Percentage = 0;

                        //Calulate Percentage for getting Apl weight
                        if (OrderQty == 0 || InvQty == 0)
                            Percentage = 0;
                        else
                        {
                            Percentage = (InvQty / OrderQty) * 100;
                        }

                        //Check NPA 01/02 by calculate Actual pack size value greater than the difference of deliveryQty
                        if (Percentage < 98)
                        {
                            if (ApCode[0] > 0)
                            {
                                if (ApCode[0] > Difference) { discCode = "NPA 01/02"; }
                                else { discCode = ""; }
                            }
                            else { discCode = ""; }
                        }
                        else { discCode = ""; }

                        //Calulation Apl weight
                        if (discCode == "NPA 01/02")
                        {

                            APLweight = (decimal)98.00;

                        }
                        else
                        {
                            if (OrderQty == 0 || InvQty == 0)
                                APLweight = 0;
                            else
                            {
                                APLweight = (InvQty / OrderQty) * 100;
                            }
                        }

                        foreach (var code in Listcode)
                        {

                            //DeliveryList.Where(w => w.UNCode == item && w.DiscrepancyCode == code) = DeliveryList.Where(w => w.UNCode == item && w.DiscrepancyCode == code).Select(s => { s.APLWeight = APLweight; return s; });
                            foreach (var List in DeliveryList.Where(w => w.UNCode == item && w.DiscrepancyCode == code))
                            {
                                if (List.DiscrepancyCode == "OR")
                                {
                                    List.APLWeight = APLweight;
                                }
                                else if (List.DiscrepancyCode == "AR")
                                {
                                    List.APLWeight = 0;
                                    List.OrderedQty = (decimal)0.00;
                                }
                                else if (List.DiscrepancyCode == "UNAS")//For UNSubstitution
                                {
                                    List.APLWeight = 0;
                                    List.OrderedQty = (decimal)0.00;
                                }
                                else
                                {
                                    List.APLWeight = 0;
                                    List.OrderedQty = (decimal)0.00;

                                }
                            }

                        }
                    }
                }
                #endregion

                #region DeliveryList Change for Existing items has AS And AR

                var UncodeList2 = (from items in DeliveryList
                                   where items.SubstituteItemCode != 0 && (items.DiscCode == "SP" || items.DiscCode == "RP" || items.DiscCode == "UNAS")
                                   select new { items.UNCode, items.SubstituteItemCode }).Distinct().ToArray();

                List<long> tempList = new List<long>();
                foreach (var item in UncodeList2)
                {
                    var tempcount = (from items in DeliveryList
                                     where items.SubstituteItemCode == item.SubstituteItemCode && items.UNCode == item.UNCode
                                     select new { items.UNCode, items.SubstituteItemCode }).Count();
                    if (tempcount > 1)
                    {
                        tempList.Add(item.UNCode);
                    }
                }

                if (tempList.Count() > 0)
                {
                    foreach (var item in tempList)
                    {
                        decimal[] ApCode = (from items in ItemMasterList.First().Value
                                            where items.UNCode == item
                                            select items.APLCode).Distinct().ToArray();

                        var Listcode = (from items in DeliveryList
                                        where items.UNCode == item
                                        select items.DiscrepancyCode).Distinct().ToArray();

                        decimal OrderQty = DeliverySingleList.Where(p => p.UNCode == item).Sum(x => x.OrderQty);
                        //decimal OrderQty = DeliveryList.Where(p => p.UNCode == item).Sum(x => x.OrderedQty);
                        decimal DelQty = DeliveryList.Where(p => p.UNCode == item).Sum(x => x.DeliveredQty);
                        decimal InvQty = DeliveryList.Where(p => p.UNCode == item).Sum(x => x.InvoiceQty);

                        decimal Difference = OrderQty - DelQty;  //To check whether Actual packsize is greater than Delivery diffrence
                        decimal APLweight = 0;
                        string discCode = "";
                        decimal Percentage = 0;
                        decimal TempINVQty = 0;

                        //Calulate Percentage for getting Apl weight
                        if (OrderQty == 0 || InvQty == 0)
                            Percentage = 0;
                        else
                        {
                            Percentage = (InvQty / OrderQty) * 100;
                        }

                        //Check NPA 01/02 by calculate Actual pack size value greater than the difference of deliveryQty
                        if (Percentage < 98)
                        {
                            if (ApCode[0] > 0)
                            {
                                if (ApCode[0] > Difference) { discCode = ""; }
                                else { discCode = "NPA 01/02"; }
                            }
                            else { discCode = ""; }
                        }
                        else { discCode = ""; }

                        //Calulation Apl weight
                        if ((OrderQty * (decimal)1.02) < DelQty)
                        {
                            APLweight = (decimal)102.00;
                        }
                        else if (discCode == "NPA 01/02")
                        {

                            // changed by kingston on 9.5.2016 the partial deliveries should not have "NPA 01/02"
                            //APLweight = (decimal)98.00;
                            APLweight = (InvQty / OrderQty) * 100;


                        }
                        else
                        {
                            if (OrderQty == 0 || InvQty == 0)
                                APLweight = 0;
                            else
                            {
                                APLweight = (InvQty / OrderQty) * 100;
                            }
                        }

                        foreach (var code in Listcode)
                        {

                            //DeliveryList.Where(w => w.UNCode == item && w.DiscrepancyCode == code) = DeliveryList.Where(w => w.UNCode == item && w.DiscrepancyCode == code).Select(s => { s.APLWeight = APLweight; return s; });
                            bool flag = true;
                            foreach (var List in DeliveryList.Where(w => w.UNCode == item && w.DiscrepancyCode == code))
                            {

                                if (List.DiscrepancyCode == "AR" && flag == true)
                                {
                                    List.APLWeight = APLweight;
                                    flag = false;
                                }
                                else if (List.DiscrepancyCode == "AS" && flag == true)
                                {
                                    List.APLWeight = APLweight;
                                    flag = false;
                                }
                                else if (List.DiscrepancyCode == "UNAS" && flag == true)//For UNSubstitution
                                {
                                    List.APLWeight = APLweight;
                                    flag = false;
                                }
                                else
                                {
                                    List.OrderedQty = (decimal)0.00;
                                    List.APLWeight = 0;
                                }
                            }

                        }
                    }
                }

                #endregion

                //Substitution

                IList<SubReplacementView> SDeliveryList = (from items in DeliveryList
                                                           where items.DiscCode == "SP"
                                                           select items).OrderBy(i => i.UNCode).Distinct().ToList();
                //Replacement
                IList<SubReplacementView> RDeliveryList = (from items in DeliveryList
                                                           where items.DiscCode == "RP"
                                                           select items).OrderBy(i => i.UNCode).Distinct().ToList();
                long SubAPlCount = 0;
                #region UNSubstitution
                //UNSubstitution
                IList<SubReplacementView> UNDeliveryList = (from items in DeliveryList
                                                            where items.DiscCode == "NPA/04"
                                                            select items).OrderBy(i => i.UNCode).Distinct().ToList();


                //UNSubstitution Calculation
                decimal UNSDeliveryQuantity = (decimal)0.00;
                decimal UNSOrderedQuantity = (decimal)0.00;
                decimal UNSAcceptedQuantity = (decimal)0.00;
                decimal UNSAcceptedamt = (decimal)0.00;

                long UNSubstituteCount = 0;
                var UnSubcodeList = (from items in UNDeliveryList
                                     select items.LineId).Distinct().ToArray();
                UNSubstituteCount = UnSubcodeList.Count();
                foreach (var item in UNDeliveryList)
                {
                    //if (item.DiscCode == "NPA/04" && item.APLWeight < (decimal)98.00)
                    //{
                    //    item.APLWeight = (decimal)98.00;
                    //}
                    if (item.APLWeight >= (decimal)98.00)
                    {
                        SubAPlCount = SubAPlCount + 1;
                    }
                    if (item.UNCode != 0 && item.SubstituteItemCode != 0)
                    {
                        //SubstituteCount = SubstituteCount + 1;
                    }
                    #region Transportation cost calculation
                    if (item.UNCode == 1129)
                    {
                        TotalTransportationCost = TotalTransportationCost + Math.Round(((item.InvoiceQty * (decimal)0.058824) * TrnsprtInv.RatePerKg), 2, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        TotalTransportationCost = TotalTransportationCost + Math.Round((item.InvoiceQty * TrnsprtInv.RatePerKg), 2, MidpointRounding.AwayFromZero);
                    }
                    #endregion
                }
                if (UNDeliveryList != null && UNDeliveryList.Count > 0)
                {
                    UNSDeliveryQuantity = UNDeliveryList.Sum(item => item.DeliveredQty);
                    UNSOrderedQuantity = UNDeliveryList.Sum(item => item.OrderedQty);
                    UNSAcceptedQuantity = UNDeliveryList.Sum(item => item.InvoiceQty);
                    UNSAcceptedamt = UNDeliveryList.Sum(item => item.AcceptedAmt);
                }
                #endregion
                #region Substitution
                //Substitution Calculation
                decimal SDeliveryQuantity = (decimal)0.00;
                decimal SOrderedQuantity = (decimal)0.00;
                decimal SAcceptedQuantity = (decimal)0.00;
                decimal SAcceptedamt = (decimal)0.00;
                //long SubAPlCount = 0;
                long SubstituteCount = 0;
                var UncodeList = (from items in SDeliveryList
                                  select items.LineId).Distinct().ToArray();
                SubstituteCount = UncodeList.Count();
                foreach (var item in SDeliveryList)
                {
                    if (item.APLWeight >= (decimal)98.00)
                    {
                        SubAPlCount = SubAPlCount + 1;
                    }
                    if (item.UNCode != 0 && item.SubstituteItemCode != 0)
                    {
                        //SubstituteCount = SubstituteCount + 1;
                    }
                    #region Transportation cost calculation
                    if (item.UNCode == 1129)
                    {
                        TotalTransportationCost = TotalTransportationCost + Math.Round(((item.InvoiceQty * (decimal)0.058824) * TrnsprtInv.RatePerKg), 2, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        TotalTransportationCost = TotalTransportationCost + Math.Round((item.InvoiceQty * TrnsprtInv.RatePerKg), 2, MidpointRounding.AwayFromZero);
                    }
                    #endregion
                }
                if (SDeliveryList != null && SDeliveryList.Count > 0)
                {
                    SDeliveryQuantity = SDeliveryList.Sum(item => item.DeliveredQty);
                    SOrderedQuantity = SDeliveryList.Sum(item => item.OrderedQty);
                    SAcceptedQuantity = SDeliveryList.Sum(item => item.InvoiceQty);
                    SAcceptedamt = SDeliveryList.Sum(item => item.AcceptedAmt);
                }
                #endregion
                #region Replacement
                //Replacement Calculation
                decimal RDeliveryQuantity = (decimal)0.00;
                decimal ROrderedQuantity = (decimal)0.00;
                decimal RAcceptedQuantity = (decimal)0.00;
                decimal RAcceptedamt = (decimal)0.00;
                foreach (var item in RDeliveryList)
                {
                    if (item.APLWeight >= (decimal)98.00)
                    {
                        SubAPlCount = SubAPlCount + 1;
                    }
                    #region Transportation cost calculation
                    if (item.UNCode == 1129)
                    {
                        TotalTransportationCost = TotalTransportationCost + Math.Round(((item.InvoiceQty * (decimal)0.058824) * TrnsprtInv.RatePerKg), 2, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        TotalTransportationCost = TotalTransportationCost + Math.Round((item.InvoiceQty * TrnsprtInv.RatePerKg), 2, MidpointRounding.AwayFromZero);
                    }
                    #endregion
                }
                if (RDeliveryList != null && RDeliveryList.Count > 0)
                {
                    RDeliveryQuantity = RDeliveryList.Sum(item => item.DeliveredQty);
                    ROrderedQuantity = RDeliveryList.Sum(item => item.OrderedQty);
                    RAcceptedQuantity = RDeliveryList.Sum(item => item.InvoiceQty);
                    RAcceptedamt = RDeliveryList.Sum(item => item.AcceptedAmt);
                }
                #endregion
                //decimal NumberOfSubstitutions = 0;
                long NumberOfDaysDelay = Convert.ToInt64((ApprovedDeliverydate - (DateTime)Ord.ExpectedDeliveryDate).TotalDays);
                long TotalLineItemsOrdered = SingleInvoiceList.Count;
                CMRMaster cmr = MS.GetCMRMasterBySectorCode(Ord.Sector);
                decimal AuthorizedCMR = CheckAuthorizedCMRisValid(cmr);
                decimal TotalManDays = 7 * Ord.Troops;
                decimal OrderCMR = (OrdervalueSum / TotalManDays);//Modified on 05/05
                decimal AcceptedCMR = ((NetAmountSum + SAcceptedamt + RAcceptedamt + UNSAcceptedamt) / TotalManDays);//Modified on 05/05
                decimal ActualCMR = Convert.ToDecimal(string.Format("{0:0.000}", OrderCMR));//Created on 05/05
                decimal CMRUtilized = Convert.ToDecimal(string.Format("{0:0.000}", (AcceptedCMR / OrderCMR) * 100));



                criteria.Clear();
                criteria.Add("ContingentControlNo", Ord.Name);
                criteria.Add("LocationCode", Ord.Location);
                criteria.Add("SectorCode", Ord.Sector);
                if (!string.IsNullOrEmpty(Ord.ContingentType) && Ord.ContingentType == "FPU")
                    criteria.Add("TypeofUnit", "FP");
                else
                    criteria.Add("TypeofUnit", "ML");
                Dictionary<long, IList<ContingentMaster>> ContingentList = MS.GetContigentListWithPagingAndCriteria(0, 9999, "UNCode", string.Empty, criteria);
                #region Performance calculation new
                decimal AmtAccept = NetAmountSum + SAcceptedamt + RAcceptedamt + UNSAcceptedamt;
                decimal Weeklywise = 0;
                Weeklywise = Math.Round(Convert.ToDecimal(0.35 / 100) * AmtAccept, 2);
                decimal confirmityCMR = 0;
                if (AcceptedCMR > AuthorizedCMR)
                {
                    confirmityCMR = (AcceptedCMR - AuthorizedCMR) * Math.Round(Ord.Troops) * 7;
                }
                decimal NetRationAmt = AmtAccept - Weeklywise - confirmityCMR;
                #endregion

                #region APL Performance calculation
                PenaltyCaculation pcl = new PenaltyCaculation();
                pcl.TotalDays = Convert.ToInt64((ApprovedDeliverydate - (DateTime)Ord.ExpectedDeliveryDate).TotalDays);
                pcl.TotalLineitem = Convert.ToInt64(Ord.LineItemsOrdered);
                pcl.TotalLineitem98 = Math.Round(TempAboveAplWeight, 3) + SubAPlCount;
                //pcl.InvoiceQty = Math.Round((TotalInvoiceQtySum + SAcceptedQuantity + RAcceptedQuantity), 3);
                pcl.InvoiceQty = Math.Round((TotalInvoiceQtySum + SAcceptedQuantity + RAcceptedQuantity + UNSAcceptedQuantity), 3);
                pcl.OrderedQty = Math.Round(TotalOrderedQtySum, 3);

                //pcl.InvoiceQty = Math.Round((TotalInvoiceQtySum + SAcceptedQuantity + RAcceptedQuantity - TotalNPAInvoicetySum), 3);
                //pcl.OrderedQty = Math.Round(TotalOrderedQtySum - TotalNPAOrderedQtySum, 3);

                pcl.SubstituteCount = SubstituteCount;
                //pcl.OrdervalueSum = Math.Round(OrdervalueSum, 3);
                pcl.OrdervalueSum = Math.Round(NetRationAmt, 3);

                PenaltyCaculation Pc = GetPenaltyCalculationValues(pcl);
                #endregion

                //PerformanceCalculateView Pc = IS.GetPerformanceCalculateById(OrderId);

                int RefNo = GetControlidInSeries(Ord);
                //int no = GetSerialNoforPeriod(Ord.Period, Ord.PeriodYear);
                string SectorNo = "";
                if (Ord.Sector == "FS")
                    SectorNo = "-090-";
                else if (Ord.Sector == "NS")
                    SectorNo = "-091-";
                else
                    SectorNo = "-092-";

                #region Deliverynotes names
                string a = string.Join(",", DeliverySingleList.Select(p => p.DeliveryNote.ToString()));
                string b = string.Join(",", SDeliveryList.Select(p => p.DeliveryNoteName.ToString()));
                string c = string.Join(",", RDeliveryList.Select(p => p.DeliveryNoteName.ToString()));
                string d = string.Join(",", UNDeliveryList.Select(p => p.DeliveryNoteName.ToString()));////For UNSubstitution
                string result = a + "," + b + "," + c + "," + d;
                string[] myArray = result.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string[] ResultAr = myArray.Distinct().ToArray();
                string DeliveryNotes = "";

                if (ResultAr.Count() > 1)
                    DeliveryNotes = String.Join(",", ResultAr);
                else
                    DeliveryNotes = ResultAr[0];
                #endregion

                SingleInvoice si = new SingleInvoice();
                InvoiceNumberMaster InvNumber = OS.GetInvoiceNumberByOrderId(OrderId);
                if (InvNumber != null && InvNumber.InvoiceMasterId > 0)
                    si.Reference = InvNumber.InvoiceNumber;
                else
                    si.Reference = "N/A"; // Need to discuss.

                //si.Reference = "GCC-" + Ord.Sector + SectorNo + Ord.ContingentType + "-" + no + "/" + RefNo;
                si.DeliveryPoint = ContingentList.FirstOrDefault().Key > 0 ? ContingentList.FirstOrDefault().Value[0].DeliveryPoint : "";
                si.ContingentID = ContingentList.FirstOrDefault().Key > 0 ? ContingentList.FirstOrDefault().Value[0].ContingentID : 0;
                si.UNID = Ord.ControlId;
                si.Strength = Math.Round(Ord.Troops);
                si.ManDays = Math.Round(TotalManDays);
                si.ApplicableCMR = Math.Round(OrderCMR, 2);
                si.AuthorizedCMR = AuthorizedCMR;
                si.Period = Ord.Period + "/" + Ord.PeriodYear;
                si.DOS = 7;
                si.DeliveryWeek = Ord.Week.ToString();
                //si.DeliveryMode = ContingentList.FirstOrDefault().Key > 0 ? ContingentList.FirstOrDefault().Value[0].DeliveryMode : "";
                si.DeliveryMode = ContingentList.FirstOrDefault().Key > 0 ? ContingentList.FirstOrDefault().Value[0].DeliveryModeDescription : "";
                si.ApprovedDelivery = ConvertDateTimeToDate(Ord.ExpectedDeliveryDate.ToString(), "en-GB");
                si.ActualDeliveryDate = ConvertDateTimeToDate(ApprovedDeliverydate.ToString(), "en-GB");
                //deliveryWithloutOrders = deliveryWithoutOrders.First().Value.ToList(),
                si.DeliveryDetails = DeliverySingleList;
                //Substitution
                #region Substitution Table
                si.SDeliveryQuantity = SDeliveryQuantity + UNSDeliveryQuantity;
                si.SOrderedQuantity = SOrderedQuantity + UNSOrderedQuantity;
                si.SAcceptedQuantity = SAcceptedQuantity + UNSAcceptedQuantity;
                si.SAcceptedamt = Math.Round((SAcceptedamt + UNSAcceptedamt), 3);
                si.SDeliveryList = SDeliveryList.Concat(UNDeliveryList).ToList();
                #endregion
                //Replacement
                #region Replacement Table
                si.RDeliveryQuantity = RDeliveryQuantity;
                si.ROrderedQuantity = ROrderedQuantity;
                si.RAcceptedQuantity = RAcceptedQuantity;
                si.RAcceptedamt = Math.Round(RAcceptedamt, 3);
                si.RDeliveryList = RDeliveryList;
                #endregion
                si.TotalDays = Convert.ToInt64((ApprovedDeliverydate - (DateTime)Ord.ExpectedDeliveryDate).TotalDays);

                si.OrderedQtySum = Math.Round(OrderedQtySum, 3);

                si.DeliveredQtySum = Math.Round(DeliveredQtySum, 3);
                si.AcceptedQtySum = Math.Round(AcceptedQtySum, 3);
                si.InvoiceQtySum = Math.Round(InvoiceQtySum, 3);

                si.NetAmountSum = Math.Round(NetAmountSum, 3);

                si.OrdervalueSum = Math.Round(OrdervalueSum, 3);
                //si.EggOrderedQtySum = Math.Round(EggOrderedQtySum, 3);
                //si.EggDeliveredQtySum = Math.Round(EggDeliveredQtySum, 3);
                //si.EggAcceptedQtySum = Math.Round(EggAcceptedQtySum, 3);
                //si.EggInvoiceQtySum = Math.Round(EggInvoiceQtySum, 3);
                //si.TotalOrderedQtySum = Math.Round(TotalOrderedQtySum, 3);
                si.EggOrderedQtySum = Math.Truncate(EggOrderedQtySum * 1000m) / 1000m;
                si.EggDeliveredQtySum = Math.Truncate(EggDeliveredQtySum * 1000m) / 1000m;
                si.EggAcceptedQtySum = Math.Truncate(EggAcceptedQtySum * 1000m) / 1000m;
                si.TotalOrderedQtySum = Math.Truncate(TotalOrderedQtySum * 1000m) / 1000m;

                si.EggInvoiceQtySum = Math.Round(EggInvoiceQtySum, 3);

                si.TotalDeliveredQtySum = Math.Round(TotalDeliveredQtySum + SDeliveryQuantity + RDeliveryQuantity + UNSDeliveryQuantity, 3);
                //si.TotalAcceptedQtySum = Math.Round(TotalAcceptedQtySum, 3);
                si.TotalAcceptedQtySum = Math.Round(TotalInvoiceQtySum + SAcceptedQuantity + RAcceptedQuantity + UNSAcceptedQuantity, 3);
                si.TotalInvoiceQtySum = Math.Round((TotalInvoiceQtySum), 3);

                si.AboveCount = Math.Round(TempAboveAplWeight, 3);
                si.BelowCount = Math.Round(TempBelowAplWeight, 3);

                si.CountPercent = Math.Round((TempAboveAplWeight / (TempAboveAplWeight + TempBelowAplWeight)) * 100, 3);
                #region APL Performance Table
                si.DeliveryPerformance = Math.Round(Pc.DeliveryPerformance, 0);
                si.LineItemPerformance = Math.Round(Pc.LineItemPerformance, 2);
                si.OrderWightPerformance = Math.Round(Pc.OrderWeightPerformance, 2);
                si.SubtitutionPerformance = Math.Round(Pc.ComplaintsPerformance, 2);
                si.DeliveryDeduction = Math.Round(Pc.DeliveryDeduction, 2);
                si.LineItemDeduction = Math.Round(Pc.LineItemDeduction, 2);
                si.OrderWightDeduction = Math.Round(Pc.OrderWeightDeduction, 2);
                si.SubtitutionDeduction = Math.Round(Pc.ComplaintsDeduction, 2);
                #endregion
                si.TotalLineitem98 = Math.Round(TempAboveAplWeight, 3) + SubAPlCount;
                si.SubstituteCount = SubstituteCount;
                si.AmountSubstituted = SDeliveryList.Sum(item => item.AcceptedAmt);
                si.OrderCMR = OrderCMR;
                si.AcceptedCMR = AcceptedCMR;
                si.CMRUtilized = CMRUtilized;
                si.Deliverynotes = DeliveryNotes;
                si.ApprovedDeliveryDate = ApprovedDeliverydate;
                si.UNItemCount = UNSubstituteCount;//For UNSubstitution
                si.AmountAccepted = AmtAccept;
                si.Weeklyinvoicediscount = Weeklywise;
                si.NetRationAmount = NetRationAmt;
                si.confirmityCMR = confirmityCMR;

                si.RatePerKg = TrnsprtInv != null ? TrnsprtInv.RatePerKg : 0;
                //si.AcceptedTransportCost = TrnsprtInv != null ? Math.Round(TrnsprtInv.RatePerKg * si.TotalAcceptedQtySum, 2) : 0;//
                si.AcceptedTransportCost = TotalTransportationCost;

                return si;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }


        //Single Invoice and Contingent wise Consol Excel List
        public ActionResult InvoiceSingleExcel(long OrderId, bool GenerateInv)
        {
            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
            {
                try
                {
                    SingleInvoice si = SingleInvoiceList(OrderId);
                    //Added by Thamizhmani for Main invoice generation for each sub invoice 
                    InvoiceList invoiceList = ConsolidateSingleInvoiceList(OrderId);
                    DataSet Workbookset = new DataSet("WorkBook");
                    List<SingleInvoice> stack = new List<SingleInvoice>();
                    stack.Add(si);
                    if (invoiceList != null)
                    {
                        DataTable table = new DataTable();
                        table.TableName = invoiceList.InvoiceNo;
                        Workbookset.Tables.Add(table);
                    }
                    if (si != null)
                    {
                        DataTable table = new DataTable();
                        table.TableName = si.UNID;
                        Workbookset.Tables.Add(table);
                    }
                    bool generate = true;
                    if (GenerateInv == true)
                        generate = true;
                    else
                        generate = false;
                    ImportToExcelSheet(Workbookset, invoiceList, stack, si.UNID, true, true, generate);
                    return View();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Weekly Consolidate Invoice generation
        /// </summary>
        /// <param name="Period"></param>
        /// <param name="searchItems"></param>
        /// <param name="GenerateInv"></param>
        /// <returns></returns>
        public ActionResult InvoicePrintExcel(string Period, string searchItems, bool GenerateInv)
        {
            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
            {
                try
                {
                    DataSet Workbookset = new DataSet("WorkBook");
                    InvoiceList invoiceList = ConsolidateInvoiceList(Period, searchItems);
                    DataTable table1 = new DataTable();
                    table1.TableName = invoiceList.InvoiceNo;
                    Workbookset.Tables.Add(table1);
                    bool generate = true;
                    if (GenerateInv == true)
                        generate = true;
                    else
                        generate = false;
                    ImportToExcelSheet(Workbookset, invoiceList, null, invoiceList.InvoiceNo, true, false, generate);
                    return View();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public ActionResult ExcelDocuments()
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
                    throw ex;
                }
            }
        }

        public ActionResult ExcelDocumentListJQGrid(string ExcelState, string searchItems, int rows, string sidx, string sord, int? page = 1)
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
                            if (Items.Length > 5)
                                if (!string.IsNullOrWhiteSpace(Items[5])) { criteria.Add("Week", Convert.ToInt64(Items[5])); }


                            if (!string.IsNullOrWhiteSpace(ExcelState) && ExcelState == "Excel-Single") { criteria.Add("DocumentType", "Excel-Single"); }
                            if (!string.IsNullOrWhiteSpace(ExcelState) && ExcelState == "Excel-Consol") { criteria.Add("DocumentType", "Excel-Consol"); }
                            if (!string.IsNullOrWhiteSpace(ExcelState) && ExcelState == "Excel-Book") { criteria.Add("DocumentType", "Excel-Book"); }
                            if (!string.IsNullOrWhiteSpace(ExcelState) && ExcelState == "Transpt-Book") { criteria.Add("DocumentType", "Transpt-Book"); }
                            if (!string.IsNullOrWhiteSpace(ExcelState) && ExcelState == "Revised-Book") { criteria.Add("DocumentType", "Revised-Book"); }
                            if (!string.IsNullOrWhiteSpace(ExcelState) && ExcelState == "Consolidate Food Order Report") { criteria.Add("DocumentType", "Consolidate Food Order Report"); }

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
                                            items.ContingentType,
                                            items.Name,
                                            items.Period,
                                            items.PeriodYear,
                                            items.ModifiedDate.ToString(),
                                            IsSubsite==true?String.Format(@"<a style='color:#034af3;text-decoration:underline' href= '/"+SubsiteName+"/ExcelGeneration/ExcelDownload?Id="+items.Id+"' target='_Blank' >{0}</a>",items.DocumentName):String.Format(@"<a style='color:#034af3;text-decoration:underline' href= '/ExcelGeneration/ExcelDownload?Id="+items.Id+"' target='_Blank' >{0}</a>",items.DocumentName)
                                            //String.Format(@"<a style='color:#034af3;text-decoration:underline' href= '/ExcelGeneration/ExcelDownload?Id="+items.Id+"' target='_Blank' >{0}</a>",items.DocumentName)
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

        public ActionResult ExcelDownload(long Id)
        {
            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
            {
                try
                {
                    ExcelDocuments ed = IS.GetExcelDocumentsDetailsById(Id);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + ed.ControlId + ".xlsx");
                    Response.BinaryWrite(ed.DocumentData);
                    Response.End();

                    //SaveDocumentToRecentDownloads(ed, null, string.Empty, null, string.Empty);

                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public ActionResult ExcelWorkBook(string searchItems)
        {
            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
            {
                try
                {
                    DataSet Workbookset = new DataSet("WorkBook");
                    if (searchItems != null && searchItems != "")
                    {
                        var Items = searchItems.ToString().Split(',');
                        if (!string.IsNullOrWhiteSpace(Items[0])) { criteria.Add("Sector", Items[0]); }
                        if (!string.IsNullOrWhiteSpace(Items[1])) { criteria.Add("Name", Items[1]); }
                        if (!string.IsNullOrWhiteSpace(Items[2])) { criteria.Add("ContingentType", Items[2]); }
                        if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Period", Items[3]); }
                        if (!string.IsNullOrWhiteSpace(Items[4])) { criteria.Add("PeriodYear", Items[4]); }
                        var TrimItems = searchItems.Replace(",", "");
                        if (string.IsNullOrWhiteSpace(TrimItems))
                            return null;
                    }
                    IList<ExcelDocuments> DocumentItemsList = null;
                    Dictionary<long, IList<ExcelDocuments>> DocumentItems = IS.GetExcelDocumentListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                    DocumentItemsList = DocumentItems.FirstOrDefault().Value;

                    for (int i = 0; i < DocumentItemsList.Count; i++)
                    {
                        DataTable table = new DataTable();
                        table.TableName = DocumentItemsList[i].ControlId;
                        Workbookset.Tables.Add(table);
                    }

                    //if (DocumentItems != null && DocumentItems.Count > 0)
                    //{

                    using (ExcelPackage pck = new ExcelPackage())
                    {

                        int TableCount = Workbookset.Tables.Count;
                        for (int i = 0; i < TableCount; i++)
                        {
                            //pck.File
                            ExcelWorksheet ws = pck.Workbook.Worksheets.Add(Workbookset.Tables[i].TableName.ToString());
                            //ws.Cells["A248:AC248"].LoadFromCollection(DocumentItemsList[i].DocumentData);
                            ws.Cells["A1"].LoadFromCollection(DocumentItemsList[i].DocumentData, true);
                            ws.SetValue(1, 1, DocumentItemsList[i].DocumentData);
                            var stream = new MemoryStream(DocumentItemsList[i].DocumentData);

                            //ws.SelectedRange(
                            //var file = File.ReadAllBytes(DocumentItemsList[i].DocumentData);
                            //MemoryStream ms = new MemoryStream(file);
                            //ws = DocumentItemsList[i].DocumentData;
                        }
                        string txtTName = "Sample";
                        //Write it back to the client
                        //pck.Save();
                        //Response.Clear();
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;  filename=" + txtTName + ".xlsx");
                        Response.BinaryWrite(pck.GetAsByteArray());
                        // myMemoryStream.WriteTo(Response.OutputStream); //works too
                        Response.End();
                    }

                    //Excel.Application app = new Excel.Application();

                    //app.Visible = true;
                    //app.Workbooks.Add("");
                    //app.Workbooks.Add(@"c:\MyWork\WorkBook1.xls");
                    //app.Workbooks.Add(@"c:\MyWork\WorkBook2.xls");


                    //for (int i = 2; i <= app.Workbooks.Count; i++)
                    //{
                    //    int count = app.Workbooks[i].Worksheets.Count;

                    //    app.Workbooks[i].Activate();
                    //    for (int j = 1; j <= count; j++)
                    //    {
                    //        Excel._Worksheet ws = (Excel._Worksheet)app.Workbooks[i].Worksheets[j];
                    //        ws.Select(Type.Missing);
                    //        ws.Cells.Select();

                    //        Excel.Range sel = (Excel.Range)app.Selection;
                    //        sel.Copy(Type.Missing);

                    //        Excel._Worksheet sheet = (Excel._Worksheet)app.Workbooks[1].Worksheets.Add(
                    //            Type.Missing, Type.Missing, Type.Missing, Type.Missing
                    //            );

                    //        sheet.Paste(Type.Missing, Type.Missing);

                    //    }


                    //}








                    //}
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Single and Contingent wise consol invoice Excel
        /// </summary>
        /// <param name="OrderId"></param>
        public void CallInvoiceExcelParallel(long OrderId)
        {
            try
            {
                SingleInvoice si = SingleInvoiceListForParallel(OrderId);
                //Added by Thamizhmani for Main invoice generation for each sub invoice 
                InvoiceList invoiceList = ConsolidateSingleInvoiceList(OrderId);
                DataSet Workbookset = new DataSet("WorkBook");
                List<SingleInvoice> stack = new List<SingleInvoice>();
                stack.Add(si);
                if (invoiceList != null)
                {
                    DataTable table = new DataTable();
                    table.TableName = invoiceList.InvoiceNo;
                    Workbookset.Tables.Add(table);
                }
                if (si != null)
                {
                    DataTable table = new DataTable();
                    table.TableName = si.UNID;
                    Workbookset.Tables.Add(table);
                }
                ImportToExcelSheetForParallel(Workbookset, invoiceList, stack, si.UNID, true, true);
                //ImportToExcelSheetForParallel(Workbookset, null, stack, si.UNID, false, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public InvoiceList ConsolidateSingleInvoiceList(long OrderId)
        {
            try
            {
                if (OrderId > 0)
                {
                    /// <summary>
                    /// Check APL Performance switch On/Off
                    /// </summary>
                    bool IsAPL = Convert.ToBoolean(ConfigurationManager.AppSettings["IsAPL"]);

                    //Dictionary<long, IList<InvoiceReports>> DictInvRpt = null;
                    criteria.Clear();
                    criteria.Add("OrderId", OrderId);

                    Dictionary<long, IList<InvoiceManagementView>> invoiceItems = IS.GetInvoiceListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);

                    var orderList = (from items in invoiceItems.First().Value
                                     select items.OrderId).OrderBy(i => i).Distinct().ToArray();


                    criteria.Add("ReportType", "ORDINV");
                    Dictionary<long, IList<InvoiceReports>> DictInvRpt1 = IS.GetInvoiceReportsListWithPagingAndCriteria(0, 9999, "ControlId", string.Empty, criteria);


                    //DictInvRpt = IS.GetInvoiceReportsListWithPagingAndCriteria(0, 9999, "ControlId", string.Empty, criteria);
                    var DictInvRpt = (from item in DictInvRpt1.FirstOrDefault().Value select item).Distinct().ToList();

                    var PeriodList = (from items in invoiceItems.First().Value
                                      select items.Week).OrderBy(i => i).Distinct().ToList();
                    long troops = ((invoiceItems.First().Value.Sum(item => (long)item.TotalFeedTroopStrength)) / PeriodList.Count());
                    long Mandays = troops * (PeriodList.Count() * 7);


                    decimal TotDelivery = Math.Round(DictInvRpt.First().AplTimelydelivery, 2, MidpointRounding.AwayFromZero);
                    decimal TotOrderLineItems = Math.Round(DictInvRpt.First().AplOrderbylineitems, 2, MidpointRounding.AwayFromZero);
                    decimal TotOrderByWeight = Math.Round(DictInvRpt.First().AplOrdersbyweight, 2, MidpointRounding.AwayFromZero);
                    decimal TotComplainAvalability = Math.Round(DictInvRpt.First().AplNoofauthorizedsubstitutions, 2, MidpointRounding.AwayFromZero);

                    //Weekly Discount added by Thamizh
                    decimal TotWeeklyDiscount = DictInvRpt.First().Weeklyinvoicediscount;
                    decimal TotalDetection = TotDelivery + TotOrderLineItems + TotOrderByWeight + TotComplainAvalability + TotWeeklyDiscount;

                    if (invoiceItems.First().Key != 0)
                    {
                        // Written by Felix kinoniya
                        decimal subTotal = 0;
                        decimal grandTotal = 0;
                        IList<PeriodWeek> periodListVal = GetPeriodListValbyOrderId(OrderId);// OrderId
                        if (periodListVal != null)
                        {
                            var SubVal = (from items in periodListVal
                                          group items by new { items.InvoiceValue } into g
                                          select g.Sum(items => Convert.ToDecimal(items.InvoiceValue))).ToList();
                            if (SubVal.Count != 0)
                            {
                                for (int i = 0; i < SubVal.Count; i++)
                                {
                                    subTotal += SubVal[i]; grandTotal += SubVal[i];
                                }
                            }
                        }
                        List<PeriodWeek> periodListValMain = new List<PeriodWeek>();
                        int length = periodListVal.Count();
                        for (int i = 0; i < length; i++)
                        {
                            PeriodWeek list = new PeriodWeek();
                            list.InvoiceValue = string.Format("{0:N}", Math.Round(Convert.ToDecimal(periodListVal[i].InvoiceValue), 2));
                            //list.AcceptedQty = string.Format("{0:N}", Math.Round(Convert.ToDecimal(periodListVal[i].AcceptedQty), 3));
                            list.AcceptedQty = Math.Round(Convert.ToDecimal(periodListVal[i].AcceptedQty), 3).ToString();
                            list.Week = periodListVal[i].Week;
                            periodListValMain.Add(list);
                        }

                        //grandTotal = Math.Round((grandTotal - TotalDetection), 2);
                        //var values = grandTotal.ToString(CultureInfo.InvariantCulture).Split('.');
                        //int firstValue = int.Parse(values[0]);
                        //int secondValue = int.Parse(values[1]);

                        int firstValue = 0;
                        int secondValue = 0;
                        if (IsAPL == true)
                        {
                            grandTotal = Math.Round((grandTotal - TotalDetection + DictInvRpt.First().Totaltransportationcost), 2);
                            var values = grandTotal.ToString(CultureInfo.InvariantCulture).Split('.');
                            firstValue = int.Parse(values[0]);
                            secondValue = int.Parse(values[1]);
                        }
                        else
                        {
                            var values = periodListValMain[0].InvoiceValue.ToString(CultureInfo.InvariantCulture).Split('.');
                            firstValue = int.Parse(values[0].Replace(",", ""));
                            secondValue = int.Parse(values[1]);
                        }

                        Dictionary<long, IList<SectorMaster>> SectorList = null;
                        criteria.Clear();
                        criteria.Add("SectorCode", DictInvRpt.First().Sector);
                        SectorList = MS.GetSectorMasterListWithLikeSearchPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                        var tempSector = SectorList.FirstOrDefault().Value[0].Description;
                        DateTime Invdt = invoiceItems.FirstOrDefault().Value[0].InvoiceDate;




                        InvoiceNumberMaster InvNumber = OS.GetInvoiceNumberByOrderId(OrderId);
                        return new InvoiceList()
                        {
                            InvoiceNo = InvNumber != null ? InvNumber.InvoiceNumber : "N/A",
                            Contract = "PD/C0036/13",
                            InvoiceDate = String.Format(new DateFormatInseries(), "{0}", Invdt),
                            PayTerms = "30 Days",
                            PO = GetPONumberFromMaster(DictInvRpt.First().Period, DictInvRpt.First().PeriodYear, "Food"),
                            TPTPO = GetPONumberFromMaster(DictInvRpt.First().Period, DictInvRpt.First().PeriodYear, "Transport"),
                            Period = DictInvRpt.First().Period + "/" + DictInvRpt.First().PeriodYear,
                            Sector = DictInvRpt.First().Sector,
                            TotMadays = Mandays.ToString(),
                            TotalFeedingToop = troops.ToString(),
                            UnINo = InvNumber != null ? InvNumber.ControlId : "N/A",
                            //CMR = Math.Round(((grandTotal - DictInvRpt.First().Totaltransportationcost) / Mandays), 2).ToString(),
                            CMR = Math.Round(DictInvRpt.First().Acceptedcmr, 2).ToString(),
                            SubTotal = string.Format("{0:C}", (grandTotal)),
                            GrandTotal = string.Format("{0:C}", (grandTotal)),
                            Usd_words = NumberToText(Convert.ToInt64(firstValue)) + " " + "Dollars" + " " + secondValue + "/100",
                            Delivery = string.Format("{0:C}", TotDelivery),
                            OrderLineItems = string.Format("{0:C}", TotOrderLineItems),
                            OrderByWeight = string.Format("{0:C}", TotOrderByWeight),
                            ComplainAvalability = string.Format("{0:C}", TotComplainAvalability),
                            PeriodWeek = periodListValMain,
                            WeeklyDiscount = TotWeeklyDiscount,
                            IsAPL = IsAPL,
                            TotalTransportationCost = DictInvRpt.First().Totaltransportationcost
                        };
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
        public void ImportToExcelSheetForParallel(DataSet Workbookset, InvoiceList invoiceList, List<SingleInvoice> stack, string InvoiceNo, bool Consol, bool Single)
        {
            try
            {
                //string userId = base.ValidateUser();
                using (ExcelPackage pck = new ExcelPackage())
                {
                    int TableCount = Workbookset.Tables.Count;
                    int x = 0; // Main Int is zero 
                    System.Drawing.Image logo = System.Drawing.Image.FromFile("D:\\HeaderImage/main_logo.jpg");
                    for (int i = 0; i < TableCount; i++)
                    {

                        if ((Workbookset.Tables[i].TableName.ToString() == invoiceList.InvoiceNo) && (Consol == true))
                        {
                            #region Cosolidate
                            ExcelWorksheet ws = pck.Workbook.Worksheets.Add(Workbookset.Tables[i].TableName.ToString());
                            ws.View.ZoomScale = 85;
                            //Adding image 
                            int img = 0;
                            ws.Row(img * 5).Height = 39.00D;
                            var picture = ws.Drawings.AddPicture(img.ToString(), logo);
                            picture.SetPosition(1, 0, 1, 0);

                            //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                            ///line 1///
                            ws.Cells["D6"].LoadFromCollection("si.UNID", true);
                            ws.Cells["B6:K7"].Merge = true;
                            ws.Cells["B6:K7"].Value = "INVOICE";
                            ws.Cells["B6:K7"].Style.WrapText = true;
                            ws.Cells["B6:K7"].Style.Font.Bold = true;
                            ws.Cells["B6:K7"].Style.Font.Name = "Arial";
                            ws.Cells["B6:K7"].Style.Font.Size = 20;
                            ws.Cells["B6:K7"].AutoFitColumns();
                            ws.Cells["B6:K7"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B6:K7"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B6:K7"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B6:K7"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B6:K7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            ws.Cells["B8:N70"].Style.Font.Size = 10;
                            ws.Cells["B8:N70"].Style.Font.Name = "Arial";

                            ws.Cells["B8:G8"].Merge = true;
                            ws.Cells["B8"].Value = "To";
                            ws.Cells["B8:G8"].Style.Font.Bold = true;
                            ws.Cells["B8:G8"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B8:G8"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["H8:I8"].Merge = true;
                            ws.Cells["H8"].Value = "";
                            ws.Cells["H8:I8"].Style.Font.Bold = true;
                            ws.Cells["H8:I8"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["J8:K8"].Merge = true;
                            ws.Cells["J8"].Value = "";
                            ws.Cells["J8:K8"].Style.Font.Bold = true;
                            ws.Cells["J8:K8"].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                            ws.Cells["B9:G9"].Merge = true;
                            ws.Cells["B9"].Value = "                     Chief Rations Unit";
                            ws.Cells["B9:G9"].Style.Font.Bold = true;
                            ws.Cells["B9:G9"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B9:G9"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["H9:I9"].Merge = true;
                            ws.Cells["H9"].Value = "Invoice #:";
                            ws.Cells["H9:I9"].Style.Font.Bold = true;
                            ws.Cells["H9:I9"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["J9:K9"].Merge = true;
                            ws.Column(11).Width = 25;
                            ws.Cells["J9"].Value = invoiceList.InvoiceNo;                           //InvoiceNo
                            ws.Cells["J9:K9"].Style.Font.Bold = true;
                            ws.Cells["J9:K9"].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                            ws.Cells["B10:G10"].Merge = true;
                            ws.Cells["B10"].Value = "                     African Union - United Nations Hybrid ";
                            ws.Cells["B10:G10"].Style.Font.Bold = true;
                            ws.Cells["B10:G10"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B10:G10"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["H10:I10"].Merge = true;
                            ws.Cells["H10"].Value = "Contract #:";
                            ws.Cells["H10:I10"].Style.Font.Bold = true;
                            ws.Cells["H10:I10"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["J10:K10"].Merge = true;
                            ws.Cells["J10"].Value = invoiceList.Contract;                           //Contract
                            ws.Cells["J10:K10"].Style.Font.Bold = true;
                            ws.Cells["J10:K10"].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                            ws.Cells["B11:G11"].Merge = true;
                            ws.Cells["B11"].Value = "                     Operation in Darfur";
                            ws.Cells["B11:G11"].Style.Font.Bold = true;
                            ws.Cells["B11:G11"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B11:G11"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["H11:I11"].Merge = true;
                            ws.Cells["H11"].Value = "Invoice Date:";
                            ws.Cells["H11:I11"].Style.Font.Bold = true;
                            ws.Cells["H11:I11"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["J11:K11"].Merge = true;
                            ws.Cells["J11"].Value = invoiceList.InvoiceDate;                        //InvoiceDate
                            ws.Cells["J11:K11"].Style.Font.Bold = true;
                            ws.Cells["J11:K11"].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                            ws.Cells["B12:G12"].Merge = true;
                            ws.Cells["B12"].Value = "                     El Fasher, Darfur";
                            ws.Cells["B12:G12"].Style.Font.Bold = true;
                            ws.Cells["B12:G12"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B12:G12"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["H12:I12"].Merge = true;
                            ws.Cells["H12"].Value = "Payment Terms:";
                            ws.Cells["H12:I12"].Style.Font.Bold = true;
                            ws.Cells["H12:I12"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["J12:K12"].Merge = true;
                            ws.Cells["J12"].Value = invoiceList.PayTerms;                           //PayTerms
                            ws.Cells["J12:K12"].Style.Font.Bold = true;
                            ws.Cells["J12:K12"].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                            ws.Cells["B13:G13"].Merge = true;
                            ws.Cells["B13"].Value = "Cc:               Mission Designated Official";
                            ws.Cells["B13:G13"].Style.Font.Bold = true;
                            ws.Cells["B13:G13"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B13:G13"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["H13:I13"].Merge = true;
                            ws.Cells["H13"].Value = "FR-PO:";
                            ws.Cells["H13:I13"].Style.Font.Bold = true;
                            ws.Cells["H13:I13"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["J13:K13"].Merge = true;
                            ws.Cells["J13"].Value = invoiceList.PO;                                 //PO
                            ws.Cells["J13:K13"].Style.Font.Bold = true;
                            ws.Cells["J13:K13"].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                            ws.Cells["B14:G14"].Merge = true;
                            ws.Cells["B14"].Value = "                   UNAMID El Fasher";
                            ws.Cells["B14:G14"].Style.Font.Bold = true;
                            ws.Cells["B14:G14"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B14:G14"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["H14:I14"].Merge = true;
                            ws.Cells["H14"].Value = "TPT-PO:";
                            ws.Cells["H14:I14"].Style.Font.Bold = true;
                            ws.Cells["H14:I14"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["J14:K14"].Merge = true;
                            ws.Cells["J14"].Value = invoiceList.TPTPO;
                            ws.Cells["J14:K14"].Style.Font.Bold = true;
                            ws.Cells["J14:K14"].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                            ws.Cells["B15:G15"].Merge = true;
                            ws.Cells["B15:G15"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B15:G15"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B15:G15"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                            ws.Cells["H15:I15"].Merge = true;
                            ws.Cells["H15:I15"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["H15:I15"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                            ws.Cells["J15:K15"].Merge = true;
                            ws.Cells["J15:K15"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["J15:K15"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                            ws.Cells["C16:H21"].Style.Font.Bold = true;
                            ws.Cells["B16"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["K16"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B17"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["K17"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B18"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["K18"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B19"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["K19"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B20"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B21"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["K20"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["K21"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B21:K21"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                            ws.Cells["C16:F16"].Merge = true;
                            ws.Cells["C16:F16"].Value = "  Period:";
                            ws.Cells["G16:H16"].Merge = true;
                            ws.Cells["G16:H16"].Value = invoiceList.Period;                           //Period

                            ws.Cells["C17:F17"].Merge = true;
                            ws.Cells["C17:F17"].Value = "  UN Identification Number:";
                            ws.Cells["G17:K17"].Merge = true;
                            ws.Cells["G17"].Value = invoiceList.UnINo;                            //UnINo

                            ws.Cells["C18:F18"].Merge = true;
                            ws.Cells["C18:F18"].Value = "  Sector:";
                            ws.Cells["G18:H18"].Merge = true;
                            ws.Cells["G18:H18"].Value = invoiceList.Sector;                           //Sector

                            ws.Cells["C19:F19"].Merge = true;
                            ws.Cells["C19:F19"].Value = "  Total Feeding Troop Strength:";
                            ws.Cells["G19:H19"].Merge = true;
                            ws.Cells["G19:H19"].Value = invoiceList.TotalFeedingToop;                 //TotalFeedingToop

                            ws.Cells["C20:F20"].Merge = true;
                            ws.Cells["C20:F20"].Value = "  Total Mandays:";
                            ws.Cells["G20:H20"].Merge = true;
                            ws.Cells["G20:H20"].Value = invoiceList.TotMadays;                       //TotMadays

                            ws.Cells["C21:F21"].Merge = true;
                            ws.Cells["C21:F21"].Value = "  Accepted CMR";
                            ws.Cells["G21:H21"].Merge = true;
                            ws.Cells["G21:H21"].Value = invoiceList.CMR;                            //CMR

                            ws.Cells["B22:K22"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B22:K22"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B22:K22"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B22:L22"].Style.Font.Bold = true;

                            ws.Cells["B22:C22"].Merge = true;
                            ws.Cells["B22:C22"].Value = "Qty";
                            ws.Cells["B22:C22"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            ws.Cells["D22:G22"].Merge = true;
                            ws.Cells["D22:G22"].Value = "Description";
                            ws.Cells["D22:G22"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            ws.Cells["H22:I22"].Merge = true;
                            ws.Cells["H22:I22"].Value = "Amount in USD";
                            ws.Cells["H22:I22"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            ws.Cells["J22:K22"].Merge = true;
                            ws.Cells["J22:K22"].Value = "Amount in USD";
                            ws.Cells["J22:K22"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                            for (int j = 23; j < 50; j++)
                            {
                                ws.Cells["B" + j + ":C" + j + ""].Merge = true;
                                ws.Cells["D" + j + ":G" + j + ""].Merge = true;
                                ws.Cells["H" + j + ":I" + j + ""].Merge = true;
                                ws.Cells["J" + j + ":K" + j + ""].Merge = true;
                                ws.Cells["B" + j + ":K" + j + ""].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                                ws.Cells["B" + j + ":K" + j + ""].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            }
                            ws.Cells["D24:G24"].Value = "PROVISION OF FOOD RATIONS";
                            ws.Cells["D26:G26"].Value = invoiceList.Sector;
                            ws.Cells["D24:J26"].Style.Font.Bold = true;
                            ws.Cells["J26"].Value = "$   " + invoiceList.PeriodWeek[0].InvoiceValue;
                            ws.Cells["J26"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            ws.Cells["B27"].Value = string.Format("{0:N}", string.Format("{0:0.000}", invoiceList.PeriodWeek[0].AcceptedQty));
                            ws.Cells["B27"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["D27"].Value = "Total Value Delivered - Week " + invoiceList.PeriodWeek[0].Week;     //week
                            ws.Cells["H27"].Value = invoiceList.PeriodWeek[0].InvoiceValue;                               //InvoiceValue 
                            ws.Cells["H27"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            ws.Cells["D30:G30"].Value = "PROVISION OF TRANSPORTATION";
                            ws.Cells["D30:G30"].Style.Font.Bold = true;
                            ws.Cells["J30"].Value = "$   " + Math.Round(invoiceList.TotalTransportationCost, 2);
                            ws.Cells["J30"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            ws.Cells["B31"].Value = string.Format("{0:N}", string.Format("{0:0.000}", invoiceList.PeriodWeek[0].AcceptedQty));
                            ws.Cells["B31"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["D31"].Value = "Total Value Delivered - Week 0" + invoiceList.PeriodWeek[0].Week;    //week
                            ws.Cells["H31"].Value = invoiceList.TotalTransportationCost;                               //InvoiceValue 
                            ws.Cells["H31"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;


                            ws.Cells["D40:G40"].Value = "DISCOUNTS";
                            ws.Cells["D40:J40"].Style.Font.Bold = true;
                            ws.Cells["J40"].Value = "(" + Math.Round(invoiceList.WeeklyDiscount, 2) + ")";
                            ws.Cells["J40"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            ws.Cells["D41:G41"].Value = "       Troop strength discounts";
                            ws.Cells["D42:G42"].Value = "       Weekly Discount";
                            ws.Cells["H42"].Value = "(" + Math.Round(invoiceList.WeeklyDiscount, 2) + ")";
                            ws.Cells["H42"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;


                            ws.Cells["D44:G44"].Value = "PERFORMANCE MATRICES";
                            ws.Cells["D44:G44"].Style.Font.Bold = true;

                            //decimal decDelivery = Math.Round(Convert.ToDecimal(invoiceList.Delivery.Replace("Rs.", "$").Replace("$", "")), 2);
                            //decimal decOrderLineItems = Math.Round(Convert.ToDecimal(invoiceList.OrderLineItems.Replace("Rs.", "$").Replace("$", "")), 2);
                            //decimal decOrderByWeight = Math.Round(Convert.ToDecimal(invoiceList.OrderByWeight.Replace("Rs.", "$").Replace("$", "")), 2);
                            //decimal decComplainAvalability = Math.Round(Convert.ToDecimal(invoiceList.ComplainAvalability.Replace("Rs.", "$").Replace("$", "")), 2);
                            //decimal totalPerformance = decDelivery + decOrderLineItems + decOrderByWeight + decComplainAvalability;

                            decimal decDelivery = 0;
                            decimal decOrderLineItems = 0;
                            decimal decOrderByWeight = 0;
                            decimal decComplainAvalability = 0;
                            decimal totalPerformance = 0;

                            if (invoiceList.IsAPL == true)
                            {
                                decDelivery = Math.Round(Convert.ToDecimal(invoiceList.Delivery.Replace("Rs.", "$").Replace("$", "")), 2);
                                decOrderLineItems = Math.Round(Convert.ToDecimal(invoiceList.OrderLineItems.Replace("Rs.", "$").Replace("$", "")), 2);
                                decOrderByWeight = Math.Round(Convert.ToDecimal(invoiceList.OrderByWeight.Replace("Rs.", "$").Replace("$", "")), 2);
                                decComplainAvalability = Math.Round(Convert.ToDecimal(invoiceList.ComplainAvalability.Replace("Rs.", "$").Replace("$", "")), 2);
                                totalPerformance = decDelivery + decOrderLineItems + decOrderByWeight + decComplainAvalability;
                            }

                            ws.Cells["J44:K44"].Value = "(" + totalPerformance + ")";
                            ws.Cells["J44:K44"].Style.Font.Bold = true;
                            ws.Cells["J44:K44"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            ws.Cells["D45:G45"].Value = "       Conformity to Delivery Schedule";
                            if (invoiceList.IsAPL == true)
                                ws.Cells["H45:I45"].Value = "-" + invoiceList.Delivery;                                                           //Delivery
                            else
                                ws.Cells["H45:I45"].Value = "0.00";

                            ws.Cells["H45:I45"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            ws.Cells["D46:G46"].Value = "       Conformity to Order by Line Items";
                            if (invoiceList.IsAPL == true)
                                ws.Cells["H46:I46"].Value = "-" + invoiceList.OrderLineItems;                                                     //OrderLineItems
                            else
                                ws.Cells["H46:I46"].Value = "0.00";

                            ws.Cells["H46:I46"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            ws.Cells["D47:G47"].Value = "       Conformity to Orders by Weight";
                            if (invoiceList.IsAPL == true)
                                ws.Cells["H47:I47"].Value = "-" + invoiceList.OrderByWeight;                                                      //OrderByWeight
                            else
                                ws.Cells["H47:I47"].Value = "0.00";

                            ws.Cells["H47:I47"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            ws.Cells["D48:G48"].Value = "       Food Order Compliance-Availability";
                            if (invoiceList.IsAPL == true)
                                ws.Cells["H48:I48"].Value = "-" + invoiceList.ComplainAvalability;                                                //ComplainAvalability
                            else
                                ws.Cells["H48:I48"].Value = "0.00";

                            ws.Cells["H48:I48"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            ws.Cells["B50"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["K50"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B50:K50"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B50:K50"].Style.Border.Top.Style = ExcelBorderStyle.Medium;

                            ws.Cells["H50:J50"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["H50:J50"].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                            ws.Cells["B50:G50"].Merge = true;
                            ws.Cells["H50:K50"].Style.Font.Bold = true;
                            ws.Cells["H50:I50"].Merge = true;
                            ws.Cells["H50:I50"].Value = "NET TOTAL";
                            //ws.Cells["H50:I50"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                            ws.Cells["J50:K50"].Merge = true;
                            if (invoiceList.IsAPL == true)
                                ws.Cells["J50:K50"].Value = invoiceList.SubTotal;            //SubTotal
                            else
                                ws.Cells["J50:K50"].Value = "$   " + invoiceList.PeriodWeek[0].InvoiceValue;

                            ws.Cells["J50:K50"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            //ws.Cells["J50:K50"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                            ws.Cells["B51:K51"].Merge = true;
                            ws.Cells["B51:K51"].Value = "Amount in Words : USD " + invoiceList.Usd_words + " Only";
                            ws.Cells["B51:K51"].Style.Font.Bold = true;
                            ws.Cells["B51:K51"].Style.Font.Color.SetColor(Color.Red);
                            ws.Cells["B51:K51"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B51:K51"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B51:K51"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B51:K51"].Style.Border.Left.Style = ExcelBorderStyle.Medium;

                            ws.Cells["B52:I53"].Merge = true;
                            ws.Cells["B52:I53"].Style.WrapText = true;
                            ws.Cells["B52:I53"].Value = "A Prompt Payment Discount of 0.2% of the NET TOTAL applies if payment is made in less than 30 days.";
                            ws.Cells["B52:I53"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            ws.Cells["B52:I53"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
                            ws.Cells["J52:K53"].Merge = true;
                            ws.Cells["J52:K53"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            ws.Cells["J52:K53"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            ws.Cells["J52:K53"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                            ws.Cells["B54:K54"].Style.Font.Bold = true;
                            ws.Cells["B54:G54"].Merge = true;
                            ws.Cells["B54:G54"].Value = "IF payment is made in less than 30 days, the PPD is: ";

                            //decimal PPD = Math.Round(Convert.ToDecimal(0.2 / 100) * (Convert.ToDecimal(invoiceList.SubTotal.Replace("Rs.", "$").Replace("$", ""))), 2);
                            decimal PPD = 0;
                            if (invoiceList.IsAPL == true)
                                PPD = Math.Round(Convert.ToDecimal(0.2 / 100) * (Convert.ToDecimal(invoiceList.SubTotal.Replace("Rs.", "$").Replace("$", ""))), 2);
                            else
                                PPD = Math.Round(Convert.ToDecimal(0.2 / 100) * (Convert.ToDecimal(invoiceList.PeriodWeek[0].InvoiceValue.Replace("Rs.", "$").Replace("$", ""))), 2);

                            ws.Cells["H54:I54"].Merge = true;
                            ws.Cells["H54:I54"].Value = "$" + PPD;
                            ws.Cells["H54:I54"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            ws.Cells["H54:I54"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            ws.Cells["J54:K54"].Merge = true;
                            ws.Cells["H54"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            ws.Cells["J54:K54"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            ws.Cells["J54:K54"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                            ws.Cells["B51:K54"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B51:K54"].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                            ws.Cells["B55"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["K55"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B55:K55"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B55:K55"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                            ws.Cells["B56:K56"].Merge = true;
                            ws.Cells["B56:K56"].Value = "Bank Details";
                            ws.Cells["B56:K56"].Style.Font.UnderLine = true;
                            ws.Cells["B56:K56"].Style.Font.Bold = true;
                            ws.Cells["B57:K57"].Merge = true;
                            ws.Cells["B57:K57"].Value = "   Bank account name: Gulf Catering Company for General Trade and Contracting WLL";
                            ws.Cells["B58:K58"].Merge = true;
                            ws.Cells["B58:K58"].Value = "   Bank name: GULF Bank";
                            ws.Cells["B59:K59"].Merge = true;
                            ws.Cells["B59:K59"].Value = "   Bank account number: KW57GULB0000000000000090622676;";
                            ws.Cells["B60:K60"].Merge = true;
                            ws.Cells["B60:K60"].Value = "   Bank address: Qibla Area Hamad Al-Saqr Street, Kharafi Tower, First Floor, P.O. Box 1683, Safat, Kuwait City, Kuwait 1683";
                            ws.Cells["B61:K61"].Merge = true;
                            ws.Cells["B62:K62"].Merge = true;
                            ws.Cells["B61:K61"].Value = "   SWIFT/ABA: GULBKWKW";
                            ws.Cells["B56:K62"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B56:K62"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B62:K62"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B64:K64"].Merge = true;
                            ws.Cells["B64:K64"].Value = "Email:  UNAMIDAR@GCCServices.com";
                            ws.Cells["B65:K65"].Merge = true;
                            ws.Cells["B65:K65"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B65:K65"].Style.Border.Top.Color.SetColor(Color.Orange);
                            ws.Cells["B65:K65"].Value = "Gulf Catering Company for General Trade and Contracting WLL  - P.O Box 4583, Safat 13046, Kuwait";
                            ws.Cells["B66"].Style.Font.Bold = true;
                            ws.Cells["B66"].Value = "Disclaimer:";
                            ws.Cells["B67:K69"].Merge = true;
                            ws.Cells["B67:K69"].Style.WrapText = true;
                            //ws.Row(67).Height=35;
                            ws.Cells["B67:K69"].Value = "In the interest of ensuring a smooth invoicing/payment process, GCC SERVICES herewith signs this GRR with the intent to officially review the Weekly Billing Discount and APL formulas.  We will submit a correction/recovery request as applicable.";
                            for (int m = 7; m < 50; m++)
                            {
                                ws.Cells["B" + m + ":K" + m + ""].Style.Border.Top.Style = ExcelBorderStyle.Hair;
                                ws.Cells["B" + m + ":K" + m + ""].Style.Border.Top.Color.SetColor(Color.Red);
                            }
                            //To Make Sheet without GridLines
                            ws.View.ShowGridLines = false;

                            //Printer Settings

                            ws.PrinterSettings.TopMargin = ws.PrinterSettings.BottomMargin = 0.25M;
                            ws.PrinterSettings.LeftMargin = ws.PrinterSettings.RightMargin = 0M;
                            ws.PrinterSettings.HeaderMargin = 0M;
                            ws.PrinterSettings.FooterMargin = 0.25M;
                            ws.PrinterSettings.Orientation = eOrientation.Portrait;
                            ws.PrinterSettings.PaperSize = ePaperSize.A4;
                            ws.PrinterSettings.FitToPage = true;
                            ws.PrinterSettings.FitToWidth = 1;
                            ws.PrinterSettings.FitToHeight = 1;
                            ws.PrinterSettings.HorizontalCentered = true;
                            ws.PrinterSettings.VerticalCentered = true;



                            #endregion Consolidate
                        }
                        else if ((Workbookset.Tables[i].TableName.ToString() == InvoiceNo) && (Single == true))
                        {
                            #region Single


                            ExcelWorksheet ws = pck.Workbook.Worksheets.Add(Workbookset.Tables[i].TableName.ToString());
                            ws.View.ZoomScale = 70;
                            ws.View.ShowGridLines = false;

                            int img = 0;
                            //for Adding image 
                            ws.Row(img * 5).Height = 39.00D;
                            var picture = ws.Drawings.AddPicture(img.ToString(), logo);
                            picture.SetPosition(1, 0, 0, 0);

                            ws.Cells["A6:AE276"].Style.Font.Size = 12;
                            Color BlueHex = System.Drawing.ColorTranslator.FromHtml("#8DB4E2");
                            Color WhiteHex = System.Drawing.ColorTranslator.FromHtml("#F2F2F2");
                            Color OrangeHex = System.Drawing.ColorTranslator.FromHtml("#FF9933");
                            Color AshHex = System.Drawing.ColorTranslator.FromHtml("#BFBFBF");
                            Color BottomHex = System.Drawing.ColorTranslator.FromHtml("#C4BD97");

                            ws.Cells["A6:N11"].Style.Font.Bold = true;
                            ws.Cells["A6:B6"].Merge = true;
                            ws.Cells["A6:B6"].Value = "Reference #";
                            ws.Cells["C6:H6"].Merge = true;
                            ws.Cells["C6:H6"].Value = stack[x].Reference;
                            ws.Cells["I6:K6"].Merge = true;
                            ws.Cells["I6:K6"].Value = "Period:";
                            ws.Cells["L6:M6"].Merge = true;
                            ws.Cells["L6:M6"].Value = stack[x].Period;

                            ws.Cells["A7:B7"].Merge = true;
                            ws.Cells["A7:B7"].Value = "Delivery Point:";
                            ws.Cells["C7:H7"].Merge = true;
                            ws.Cells["C7:H7"].Value = stack[x].DeliveryPoint;
                            ws.Cells["I7:K7"].Merge = true;
                            ws.Cells["I7:K7"].Value = "DOS:";
                            ws.Cells["L7:M7"].Merge = true;
                            ws.Cells["L7:M7"].Value = stack[x].DOS;

                            ws.Cells["A8:B8"].Merge = true;
                            ws.Cells["A8:B8"].Value = "UN ID of the FFO:";
                            ws.Cells["C8:H8"].Merge = true;
                            ws.Cells["C8:H8"].Value = stack[x].UNID;
                            ws.Cells["I8:K8"].Merge = true;
                            ws.Cells["I8:K8"].Value = "Delivery Week:";
                            ws.Cells["L8:M8"].Merge = true;
                            ws.Cells["L8:M8"].Value = stack[x].DeliveryWeek;

                            ws.Cells["A9:B9"].Merge = true;
                            ws.Cells["A9:B9"].Value = "Strength:";
                            ws.Cells["C9:H9"].Merge = true;
                            ws.Cells["C9:H9"].Value = stack[x].Strength;
                            ws.Cells["I9:K9"].Merge = true;
                            ws.Cells["I9:K9"].Value = "Delivery Mode:";
                            ws.Cells["L9:M9"].Merge = true;
                            ws.Cells["L9:M9"].Value = stack[x].DeliveryMode;

                            ws.Cells["A10:B10"].Merge = true;
                            ws.Cells["A10:B10"].Value = "Man Days:";
                            ws.Cells["C10:H10"].Merge = true;
                            ws.Cells["C10:H10"].Value = stack[x].ManDays;
                            ws.Cells["I10:K10"].Merge = true;
                            ws.Cells["I10:K10"].Value = "Approved Delivery Dates:";
                            ws.Cells["L10:M10"].Merge = true;
                            ws.Cells["L10:M10"].Value = stack[x].ApprovedDelivery;

                            ws.Cells["A11:B11"].Merge = true;
                            ws.Cells["A11:B11"].Value = "Accepted CMR";
                            ws.Cells["C11:H11"].Merge = true;
                            ws.Cells["C11:H11"].Value = Math.Round(stack[x].AcceptedCMR, 2);
                            ws.Cells["I11:K11"].Merge = true;
                            ws.Cells["I11:K11"].Value = "Actual Delivery Date";
                            ws.Cells["L11:M11"].Merge = true;
                            ws.Cells["L11:M11"].Value = stack[x].ActualDeliveryDate;


                            ws.Cells["A6:M11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                            ws.Cells["A13:AC14"].Style.Font.Bold = true;
                            ws.Cells["A13:AC14"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A13:AC14"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A13:AC14"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A13:AC14"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A13:AC14"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["A13:AC14"].Style.Fill.BackgroundColor.SetColor(BlueHex);
                            ws.Cells["A13:AC14"].Style.Font.Color.SetColor(Color.White);
                            ws.Cells["A13:AC14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                            ws.Cells["A13:B14"].Merge = true;
                            ws.Cells["A13:B14"].Value = "S.NO";
                            ws.Cells["A13:B14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["A13:B14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["C13:D14"].Merge = true;
                            ws.Cells["C13:D14"].Value = "UNRS NO";
                            ws.Cells["C13:D14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["C13:D14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["E13:I14"].Merge = true;
                            ws.Cells["E13:I14"].Value = "Discription";
                            ws.Cells["E13:I14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["E13:I14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["J13:K14"].Merge = true;
                            ws.Cells["J13:K14"].Value = "Order Qty";
                            ws.Cells["J13:K14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["J13:K14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["L13:M14"].Merge = true;
                            ws.Cells["L13:M14"].Value = "Delivery Qty";
                            ws.Cells["L13:M14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["L13:M14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["N13:O14"].Merge = true;
                            ws.Cells["N13:O14"].Value = "Accepted Qty";
                            ws.Cells["N13:O14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["N13:O14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["P13:Q14"].Merge = true;
                            ws.Cells["P13:Q14"].Value = "Price/Unit";
                            ws.Cells["P13:Q14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["P13:Q14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["R13:S14"].Merge = true;
                            ws.Cells["R13:S14"].Value = "Net Amount";
                            ws.Cells["R13:S14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["R13:S14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["T13:U14"].Merge = true;
                            ws.Cells["T13:U14"].Value = "APL Weight";
                            ws.Cells["T13:U14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["T13:U14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["V13:W14"].Merge = true;
                            ws.Cells["V13:W14"].Value = "Discrepancy Code";
                            ws.Cells["V13:W14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["V13:W14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["X13:X14"].Merge = true;
                            ws.Cells["X13:X14"].Value = "UOM";
                            ws.Cells["X13:X14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["Y13:Z14"].Merge = true;
                            ws.Cells["Y13:Z14"].Value = "Order Value";
                            ws.Cells["Y13:Z14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["Y13:Z14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["AA13:AC14"].Merge = true;
                            ws.Cells["AA13:AC14"].Value = "DN #";
                            ws.Cells["AA13:AC14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["AA13:AC14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                            ws.Cells["A15:B15"].Merge = true;
                            ws.Cells["C15:D15"].Merge = true;
                            ws.Cells["E15:I15"].Merge = true;
                            ws.Cells["J15:K15"].Merge = true;
                            ws.Cells["L15:M15"].Merge = true;
                            ws.Cells["N15:O15"].Merge = true;
                            ws.Cells["P15:Q15"].Merge = true;
                            ws.Cells["R15:S15"].Merge = true;
                            ws.Cells["T15:U15"].Merge = true;
                            ws.Cells["V15:W15"].Merge = true;
                            ws.Cells["X15:X15"].Merge = true;
                            ws.Cells["Y15:Z15"].Merge = true;
                            ws.Cells["AA15:AC15"].Merge = true;
                            ws.Cells["A15:AC15"].Style.Border.Left.Style = ExcelBorderStyle.Hair;
                            ws.Cells["AC15"].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                            for (int l = 16; l < 166; l++)
                            {
                                ws.Cells["A" + l + ":B" + l + ""].Merge = true;
                                ws.Cells["A" + l + ":B" + l + ""].Value = "";
                                ws.Cells["A" + l + ":B" + l + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["C" + l + ":D" + l + ""].Merge = true;
                                ws.Cells["C" + l + ":D" + l + ""].Value = "";
                                ws.Cells["C" + l + ":D" + l + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["E" + l + ":I" + l + ""].Merge = true;
                                ws.Cells["E" + l + ":I" + l + ""].Value = "";
                                ws.Cells["J" + l + ":K" + l + ""].Merge = true;
                                ws.Cells["J" + l + ":K" + l + ""].Value = "";
                                ws.Cells["J" + l + ":K" + l + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                ws.Cells["L" + l + ":M" + l + ""].Merge = true;
                                ws.Cells["L" + l + ":M" + l + ""].Value = "";
                                ws.Cells["L" + l + ":M" + l + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                ws.Cells["N" + l + ":O" + l + ""].Merge = true;
                                ws.Cells["N" + l + ":O" + l + ""].Value = "";
                                ws.Cells["N" + l + ":O" + l + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                ws.Cells["P" + l + ":Q" + l + ""].Merge = true;
                                ws.Cells["P" + l + ":Q" + l + ""].Value = "";
                                ws.Cells["P" + l + ":Q" + l + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                ws.Cells["R" + l + ":S" + l + ""].Merge = true;
                                ws.Cells["R" + l + ":S" + l + ""].Value = "";
                                ws.Cells["R" + l + ":S" + l + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                ws.Cells["T" + l + ":U" + l + ""].Merge = true;
                                ws.Cells["T" + l + ":U" + l + ""].Value = "";
                                ws.Cells["T" + l + ":U" + l + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                ws.Cells["V" + l + ":W" + l + ""].Merge = true;
                                ws.Cells["V" + l + ":W" + l + ""].Value = "";
                                ws.Cells["V" + l + ":W" + l + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                ws.Cells["X" + l + ":X" + l + ""].Merge = true;
                                ws.Cells["X" + l + ":X" + l + ""].Value = "";
                                ws.Cells["Y" + l + ":Z" + l + ""].Merge = true;
                                ws.Cells["Y" + l + ":Z" + l + ""].Value = "";
                                ws.Cells["Y" + l + ":Z" + l + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                ws.Cells["AA" + l + ":AC" + l + ""].Merge = true;
                                ws.Cells["AA" + l + ":AC" + l + ""].Value = "";
                                ws.Cells["A" + l + ":AC" + l + ""].Style.Border.Right.Style = ExcelBorderStyle.Hair;
                                ws.Cells["AC" + l + ""].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            }



                            int temp = 0;
                            int orderCount = (16 + stack[x].DeliveryDetails.Count());
                            int s = 0;
                            if (stack[x].DeliveryDetails.Count() != 0)
                            {
                                for (int a = 16; a < orderCount; a++)
                                {
                                    temp = temp + 1;
                                    ws.Cells["A" + a + ":B" + a + ""].Merge = true;
                                    ws.Cells["A" + a + ":B" + a + ""].Value = temp;
                                    ws.Cells["A" + a + ":B" + a + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells["C" + a + ":D" + a + ""].Merge = true;
                                    ws.Cells["C" + a + ":D" + a + ""].Value = stack[x].DeliveryDetails[s].UNCode;
                                    ws.Cells["C" + a + ":D" + a + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells["E" + a + ":I" + a + ""].Merge = true;
                                    ws.Cells["E" + a + ":I" + a + ""].Value = stack[x].DeliveryDetails[s].Commodity;
                                    ws.Cells["J" + a + ":K" + a + ""].Merge = true;
                                    ws.Cells["J" + a + ":K" + a + ""].Value = Math.Round(stack[x].DeliveryDetails[s].OrderQty, 3);
                                    ws.Cells["J" + a + ":K" + a + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    ws.Cells["L" + a + ":M" + a + ""].Merge = true;
                                    ws.Cells["L" + a + ":M" + a + ""].Value = Math.Round(stack[x].DeliveryDetails[s].DeliveredOrdQty, 3);
                                    ws.Cells["L" + a + ":M" + a + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    ws.Cells["N" + a + ":O" + a + ""].Merge = true;
                                    ws.Cells["N" + a + ":O" + a + ""].Value = Math.Round(stack[x].DeliveryDetails[s].InvoiceQty, 3);
                                    ws.Cells["N" + a + ":O" + a + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    ws.Cells["P" + a + ":Q" + a + ""].Merge = true;
                                    ws.Cells["P" + a + ":Q" + a + ""].Value = "$" + Math.Round(stack[x].DeliveryDetails[s].SectorPrice, 2);
                                    ws.Cells["P" + a + ":Q" + a + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    ws.Cells["R" + a + ":S" + a + ""].Merge = true;
                                    ws.Cells["R" + a + ":S" + a + ""].Value = "$" + Math.Round(stack[x].DeliveryDetails[s].NetAmt, 2);
                                    ws.Cells["R" + a + ":S" + a + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    ws.Cells["T" + a + ":U" + a + ""].Merge = true;
                                    ws.Cells["T" + a + ":U" + a + ""].Value = Math.Round(stack[x].DeliveryDetails[s].APLWeight, 2) + "%";
                                    ws.Cells["T" + a + ":U" + a + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    ws.Cells["V" + a + ":W" + a + ""].Merge = true;
                                    ws.Cells["V" + a + ":W" + a + ""].Value = stack[x].DeliveryDetails[s].DiscrepancyCode;
                                    ws.Cells["V" + a + ":W" + a + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells["X" + a + ":X" + a + ""].Merge = true;
                                    ws.Cells["X" + a + ":X" + a + ""].Value = stack[x].DeliveryDetails[s].UOM;
                                    ws.Cells["Y" + a + ":Z" + a + ""].Merge = true;
                                    ws.Cells["Y" + a + ":Z" + a + ""].Value = "$" + Math.Round(stack[x].DeliveryDetails[s].OrderValue, 2);
                                    ws.Cells["Y" + a + ":Z" + a + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    ws.Cells["AA" + a + ":AC" + a + ""].Merge = true;
                                    ws.Cells["AA" + a + ":AC" + a + ""].Value = stack[x].DeliveryDetails[s].DeliveryNote;
                                    ws.Cells["AA" + a + ":AC" + a + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    ws.Cells["A" + a + ":AC" + a + ""].Style.Border.Right.Style = ExcelBorderStyle.Hair;
                                    ws.Cells["AC" + a + ""].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                                    s = s + 1;
                                }
                            }
                            ws.Cells["A165:AC165"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                            ws.Cells["A166:B168"].Merge = true;
                            ws.Cells["A166:B168"].Value = temp;
                            ws.Cells["A166:B168"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["A166:B168"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                            ws.Cells["C166:I166"].Merge = true;
                            ws.Cells["C166:I166"].Value = "Sub total without Eggs..................................................................................";
                            ws.Cells["J166:K166"].Merge = true;
                            ws.Cells["J166:K166"].Value = string.Format("{0:N}", string.Format("{0:0.000}", stack[x].OrderedQtySum));
                            ws.Cells["L166:M166"].Merge = true;
                            ws.Cells["L166:M166"].Value = string.Format("{0:N}", string.Format("{0:0.000}", stack[x].DeliveredQtySum));
                            ws.Cells["N166:O166"].Merge = true;
                            ws.Cells["N166:O166"].Value = string.Format("{0:N}", string.Format("{0:0.000}", stack[x].InvoiceQtySum));
                            ws.Cells["P166:Q166"].Merge = true;
                            ws.Cells["P166:Q166"].Value = "";
                            ws.Cells["R166:S166"].Merge = true;
                            ws.Cells["R166:S166"].Value = "$  " + string.Format("{0:N}", (Math.Round(stack[x].NetAmountSum, 2)));
                            ws.Cells["T166:U166"].Merge = true;
                            ws.Cells["T166:U166"].Value = stack[x].AboveCount;
                            ws.Cells["V166:W166"].Merge = true;
                            ws.Cells["V166:W166"].Value = "";
                            ws.Cells["X166:X166"].Merge = true;
                            ws.Cells["X166:X166"].Value = "";
                            ws.Cells["Y166:Z166"].Merge = true;
                            ws.Cells["Y166:Z166"].Value = "$  " + string.Format("{0:N}", (Math.Round(stack[x].OrdervalueSum, 2)));
                            ws.Cells["AA166:AC166"].Merge = true;
                            ws.Cells["AA166:AC166"].Value = "";

                            ws.Cells["C167:I167"].Merge = true;
                            ws.Cells["C167:I167"].Value = "Eggs in KG ...................................................................................................................";
                            ws.Cells["J167:K167"].Merge = true;
                            ws.Cells["J167:K167"].Value = stack[x].EggOrderedQtySum;
                            ws.Cells["L167:M167"].Merge = true;
                            ws.Cells["L167:M167"].Value = stack[x].EggDeliveredQtySum;
                            ws.Cells["N167:O167"].Merge = true;
                            ws.Cells["N167:O167"].Value = stack[x].EggInvoiceQtySum;
                            ws.Cells["P167:Q167"].Merge = true;
                            ws.Cells["P167:Q167"].Value = "";
                            ws.Cells["R167:S167"].Merge = true;
                            ws.Cells["R167:S167"].Value = "$-";
                            ws.Cells["T167:U167"].Merge = true;
                            ws.Cells["T167:U167"].Value = stack[x].BelowCount;
                            ws.Cells["V167:W167"].Merge = true;
                            ws.Cells["V167:W167"].Value = "";
                            ws.Cells["X167:X167"].Merge = true;
                            ws.Cells["X167:X167"].Value = "";
                            ws.Cells["Y167:Z167"].Merge = true;
                            ws.Cells["Y167:Z167"].Value = "$-";
                            ws.Cells["AA167:AC167"].Merge = true;
                            ws.Cells["AA167:AC167"].Value = "";

                            ws.Cells["C168:I168"].Merge = true;
                            ws.Cells["C168:I168"].Value = "Sub total with Eggs in KG...........................................................................................";
                            ws.Cells["J168:K168"].Merge = true;
                            ws.Cells["J168:K168"].Value = Math.Round(stack[x].TotalOrderedQtySum, 3);
                            ws.Cells["L168:M168"].Merge = true;
                            ws.Cells["L168:M168"].Value = Math.Round(stack[x].TotalDeliveredQtySum, 3);
                            ws.Cells["N168:O168"].Merge = true;
                            ws.Cells["N168:O168"].Value = Math.Round(stack[x].TotalInvoiceQtySum, 3);
                            ws.Cells["P168:Q168"].Merge = true;
                            ws.Cells["P168:Q168"].Value = "";
                            ws.Cells["R168:S168"].Merge = true;
                            ws.Cells["R168:S168"].Value = "$  " + string.Format("{0:N}", (Math.Round(stack[x].NetAmountSum, 2)));
                            ws.Cells["T168:U168"].Merge = true;
                            ws.Cells["T168:U168"].Value = Math.Round(stack[x].CountPercent, 2) + "%";
                            ws.Cells["V168:W168"].Merge = true;
                            ws.Cells["V168:W168"].Value = "";
                            ws.Cells["X168:X168"].Merge = true;
                            ws.Cells["X168:X168"].Value = "";
                            ws.Cells["Y168:Z168"].Merge = true;
                            ws.Cells["Y168:Z168"].Value = "$  " + string.Format("{0:N}", (Math.Round(stack[x].OrdervalueSum, 2)));
                            ws.Cells["AA168:AC168"].Merge = true;
                            ws.Cells["AA168:AC168"].Value = "";
                            ws.Cells["A166:AC168"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A166:AC168"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A166:AC168"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                            #region Substitution
                            ws.Cells["A170:R170"].Merge = true;
                            ws.Cells["A170:R170"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                            ws.Cells["A171:R171"].Merge = true;
                            ws.Cells["A171:R171"].Value = "Substitutions";
                            ws.Cells["A171:R171"].Style.Font.Italic = true;
                            ws.Cells["A171:R171"].Style.Font.Color.SetColor(Color.DarkRed);
                            ws.Cells["A171:R171"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["A171:R171"].Style.Border.Right.Style = ExcelBorderStyle.Medium;


                            ws.Cells["A172:R173"].Style.WrapText = true;
                            ws.Cells["A172:R173"].Style.Font.Bold = true;
                            ws.Cells["A172:R173"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A172:R173"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A172:R173"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A172:R173"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A172:R173"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["A172:R173"].Style.Fill.BackgroundColor.SetColor(BlueHex);
                            ws.Cells["A172:R173"].Style.Font.Color.SetColor(Color.White);
                            ws.Cells["A172:R173"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                            ws.Cells["A172:A173"].Merge = true;
                            ws.Cells["A172:A173"].Value = "UNRS Code";
                            ws.Cells["A172:A173"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["A172:A173"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["B172:C173"].Merge = true;
                            ws.Cells["B172:C173"].Value = "Substituted With COMMODITY";
                            ws.Cells["B172:C173"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["B172:C173"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["D172:E173"].Merge = true;
                            ws.Cells["D172:E173"].Value = "Delivered Quantity";
                            ws.Cells["D172:E173"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["D172:E173"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["F172:F173"].Merge = true;
                            ws.Cells["F172:F173"].Value = "UNIT COST";
                            ws.Cells["F172:F173"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["F172:F173"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["G172:G173"].Merge = true;
                            ws.Cells["G172:G173"].Value = "UNRS Code";
                            ws.Cells["G172:G173"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["G172:G173"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["H172:I173"].Merge = true;
                            ws.Cells["H172:I173"].Value = "COMMODITY";
                            ws.Cells["H172:I173"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["H172:I173"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["J172:J173"].Merge = true;
                            ws.Cells["J172:J173"].Value = "Ordered";
                            ws.Cells["J172:J173"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["J172:J173"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["K172:L173"].Merge = true;
                            ws.Cells["K172:L173"].Value = "Accepted  Quantity";
                            ws.Cells["K172:L173"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["K172:L173"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["M172:M173"].Merge = true;
                            ws.Cells["M172:M173"].Value = "UNIT COST";
                            ws.Cells["M172:M173"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["M172:M173"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["N172:O173"].Merge = true;
                            ws.Cells["N172:O173"].Value = "Accepted Amount";
                            ws.Cells["N172:O173"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["N172:O173"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["P172:P173"].Merge = true;
                            ws.Cells["P172:P173"].Value = "APL Weight";
                            ws.Cells["P172:P173"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["P172:P173"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["Q172:R173"].Merge = true;
                            ws.Cells["Q172:R173"].Value = "DN #";
                            ws.Cells["Q172:R173"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["Q172:R173"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                            for (int K = 174; K < 194; K++)
                            {

                                ws.Cells["A" + K + ":A" + K + ""].Merge = true;
                                ws.Cells["A" + K + ":A" + K + ""].Value = "";
                                ws.Cells["A" + K + ":A" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["B" + K + ":C" + K + ""].Merge = true;
                                ws.Cells["B" + K + ":C" + K + ""].Value = "";
                                ws.Cells["D" + K + ":E" + K + ""].Merge = true;
                                ws.Cells["D" + K + ":E" + K + ""].Value = "";
                                ws.Cells["D" + K + ":E" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["F" + K + ":F" + K + ""].Merge = true;
                                ws.Cells["F" + K + ":F" + K + ""].Value = "";
                                ws.Cells["F" + K + ":F" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["G" + K + ":G" + K + ""].Merge = true;
                                ws.Cells["G" + K + ":G" + K + ""].Value = "";
                                ws.Cells["G" + K + ":G" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["H" + K + ":I" + K + ""].Merge = true;
                                ws.Cells["H" + K + ":I" + K + ""].Value = "";
                                ws.Cells["J" + K + ":J" + K + ""].Merge = true;
                                ws.Cells["J" + K + ":J" + K + ""].Value = "";
                                ws.Cells["J" + K + ":J" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["K" + K + ":L" + K + ""].Merge = true;
                                ws.Cells["K" + K + ":L" + K + ""].Value = "";
                                ws.Cells["K" + K + ":L" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["M" + K + ":M" + K + ""].Merge = true;
                                ws.Cells["M" + K + ":M" + K + ""].Value = "";
                                ws.Cells["M" + K + ":M" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["N" + K + ":O" + K + ""].Merge = true;
                                ws.Cells["N" + K + ":O" + K + ""].Value = "";
                                ws.Cells["N" + K + ":O" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["P" + K + ":P" + K + ""].Merge = true;
                                ws.Cells["P" + K + ":P" + K + ""].Value = "";
                                ws.Cells["P" + K + ":P" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["Q" + K + ":R" + K + ""].Merge = true;
                                ws.Cells["Q" + K + ":R" + K + ""].Value = "";
                                ws.Cells["A" + K + ":R" + K + ""].Style.Border.Right.Style = ExcelBorderStyle.Hair;
                                ws.Cells["A" + K + ":R" + K + ""].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                                ws.Cells["R" + K + ""].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            }

                            int tempSub = 0;
                            int x1 = 0;
                            int dcount = (174 + stack[x].SDeliveryList.Count());
                            int AboveSubtituteCount = 0;
                            if (stack[x].SDeliveryList.Count() != 0)
                            {
                                for (int K = 174; K < dcount; K++)
                                {
                                    if (stack[x].SDeliveryList[x1].SubstituteItemCode != 0)
                                    {
                                        tempSub = tempSub + 1;
                                    }
                                    ws.Cells["A" + K + ":A" + K + ""].Merge = true;
                                    ws.Cells["A" + K + ":A" + K + ""].Value = stack[x].SDeliveryList[x1].SubstituteItemCode;
                                    ws.Cells["A" + K + ":A" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells["B" + K + ":C" + K + ""].Merge = true;
                                    ws.Cells["B" + K + ":C" + K + ""].Value = stack[x].SDeliveryList[x1].SubstituteItemName;
                                    ws.Cells["D" + K + ":E" + K + ""].Merge = true;
                                    ws.Cells["D" + K + ":E" + K + ""].Value = Math.Round(stack[x].SDeliveryList[x1].DeliveredQty, 3);
                                    ws.Cells["D" + K + ":E" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    ws.Cells["F" + K + ":F" + K + ""].Merge = true;
                                    ws.Cells["F" + K + ":F" + K + ""].Value = "$" + Math.Round(stack[x].SDeliveryList[x1].SubstituteSectorPrice, 2);
                                    ws.Cells["F" + K + ":F" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells["G" + K + ":G" + K + ""].Merge = true;
                                    ws.Cells["G" + K + ":G" + K + ""].Value = stack[x].SDeliveryList[x1].UNCode;
                                    ws.Cells["G" + K + ":G" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells["H" + K + ":I" + K + ""].Merge = true;
                                    ws.Cells["H" + K + ":I" + K + ""].Value = stack[x].SDeliveryList[x1].Commodity;
                                    ws.Cells["J" + K + ":J" + K + ""].Merge = true;
                                    ws.Cells["J" + K + ":J" + K + ""].Value = Math.Round(stack[x].SDeliveryList[x1].OrderedQty, 3);
                                    ws.Cells["J" + K + ":J" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    ws.Cells["K" + K + ":L" + K + ""].Merge = true;
                                    ws.Cells["K" + K + ":L" + K + ""].Value = Math.Round(stack[x].SDeliveryList[x1].InvoiceQty, 3);
                                    ws.Cells["K" + K + ":L" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    ws.Cells["M" + K + ":M" + K + ""].Merge = true;
                                    ws.Cells["M" + K + ":M" + K + ""].Value = "$" + Math.Round(stack[x].SDeliveryList[x1].SectorPrice, 2);
                                    ws.Cells["M" + K + ":M" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells["N" + K + ":O" + K + ""].Merge = true;

                                    if (stack[x].SDeliveryList[x1].APLWeight >= 98) { AboveSubtituteCount = AboveSubtituteCount + 1; }

                                    ws.Cells["N" + K + ":O" + K + ""].Value = "$" + Math.Round(stack[x].SDeliveryList[x1].AcceptedAmt, 2);
                                    ws.Cells["N" + K + ":O" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    ws.Cells["P" + K + ":P" + K + ""].Merge = true;
                                    ws.Cells["P" + K + ":P" + K + ""].Value = Math.Round(stack[x].SDeliveryList[x1].APLWeight, 2) + "%";
                                    ws.Cells["P" + K + ":P" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    ws.Cells["Q" + K + ":R" + K + ""].Merge = true;
                                    ws.Cells["Q" + K + ":R" + K + ""].Value = stack[x].SDeliveryList[x1].DeliveryNoteName;
                                    ws.Cells["A" + K + ":R" + K + ""].Style.Border.Right.Style = ExcelBorderStyle.Hair;
                                    ws.Cells["A" + K + ":R" + K + ""].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                                    ws.Cells["R" + K + ""].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                                    x1 = x1 + 1;
                                }
                            }

                            ws.Cells["A194:A194"].Merge = true;
                            ws.Cells["A194:A194"].Value = x1;
                            ws.Cells["A194:A194"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["B194:C194"].Merge = true;
                            ws.Cells["B194:C194"].Value = "Sub Total";
                            ws.Cells["D194:E194"].Merge = true;
                            ws.Cells["D194:E194"].Value = string.Format("{0:N}", string.Format("{0:0.000}", (Math.Round(stack[x].SDeliveryQuantity, 3))));
                            ws.Cells["F194:F194"].Merge = true;
                            ws.Cells["F194:F194"].Value = "";
                            ws.Cells["G194:G194"].Merge = true;
                            ws.Cells["G194:G194"].Value = "";
                            ws.Cells["H194:I194"].Merge = true;
                            ws.Cells["H194:I194"].Value = "";
                            ws.Cells["J194:J194"].Merge = true;
                            ws.Cells["J194:J194"].Value = string.Format("{0:N}", string.Format("{0:0.000}", (Math.Round(stack[x].SOrderedQuantity, 3))));
                            ws.Cells["K194:L194"].Merge = true;
                            ws.Cells["K194:L194"].Value = string.Format("{0:N}", string.Format("{0:0.000}", (Math.Round(stack[x].SAcceptedQuantity, 3))));
                            ws.Cells["M194:M194"].Merge = true;
                            ws.Cells["M194:M194"].Value = "";
                            ws.Cells["N194:O194"].Merge = true;
                            ws.Cells["N194:O194"].Value = "$  " + string.Format("{0:N}", (Math.Round(stack[x].SAcceptedamt, 2)));
                            ws.Cells["P194:P194"].Merge = true;
                            ws.Cells["P194:P194"].Value = "";
                            ws.Cells["Q194:R194"].Merge = true;
                            ws.Cells["Q194:R194"].Value = "";
                            ws.Cells["A194:R194"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A194:R194"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A194:R194"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A194:R194"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["A194:R194"].Style.Fill.BackgroundColor.SetColor(BlueHex);
                            ws.Cells["A194:R194"].Style.Font.Color.SetColor(Color.White);
                            #endregion

                            #region Replacement

                            ws.Cells["A195:R195"].Merge = true;
                            ws.Cells["A195:R195"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                            ws.Cells["A196:R196"].Merge = true;
                            ws.Cells["A196:R196"].Value = "Replacements";
                            ws.Cells["A196:R196"].Style.Font.Italic = true;
                            ws.Cells["A196:R196"].Style.Font.Color.SetColor(Color.DarkRed);
                            ws.Cells["A196:R196"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["A196:R196"].Style.Border.Right.Style = ExcelBorderStyle.Medium;


                            ws.Cells["A197:R198"].Style.WrapText = true;
                            ws.Cells["A197:R198"].Style.Font.Bold = true;
                            ws.Cells["A197:R198"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A197:R198"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A197:R198"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A197:R198"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A197:R198"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["A197:R198"].Style.Fill.BackgroundColor.SetColor(BlueHex);
                            ws.Cells["A197:R198"].Style.Font.Color.SetColor(Color.White);
                            ws.Cells["A197:R198"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                            ws.Cells["A197:A198"].Merge = true;
                            ws.Cells["A197:A198"].Value = "UNRS Code";
                            ws.Cells["A197:A198"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["A197:A198"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["B197:C198"].Merge = true;
                            ws.Cells["B197:C198"].Value = "Replacements With COMMODITY";
                            ws.Cells["B197:C198"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["B197:C198"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["D197:E198"].Merge = true;
                            ws.Cells["D197:E198"].Value = "Delivered Quantity";
                            ws.Cells["D197:E198"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["D197:E198"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["F197:F198"].Merge = true;
                            ws.Cells["F197:F198"].Value = "UNIT COST";
                            ws.Cells["F197:F198"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["F197:F198"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["G197:G198"].Merge = true;
                            ws.Cells["G197:G198"].Value = "UNRS Code";
                            ws.Cells["G197:G198"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["G197:G198"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["H197:I198"].Merge = true;
                            ws.Cells["H197:I198"].Value = "COMMODITY";
                            ws.Cells["H197:I198"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["H197:I198"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["J197:J198"].Merge = true;
                            ws.Cells["J197:J198"].Value = "Ordered";
                            ws.Cells["J197:J198"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["J197:J198"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["K197:L198"].Merge = true;
                            ws.Cells["K197:L198"].Value = "Accepted  Quantity";
                            ws.Cells["K197:L198"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["K197:L198"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["M197:M198"].Merge = true;
                            ws.Cells["M197:M198"].Value = "UNIT COST";
                            ws.Cells["M197:M198"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["M197:M198"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["N197:O198"].Merge = true;
                            ws.Cells["N197:O198"].Value = "Accepted Amount";
                            ws.Cells["N197:O198"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["N197:O198"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["P197:P198"].Merge = true;
                            ws.Cells["P197:P198"].Value = "APL Weight";
                            ws.Cells["P197:P198"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["P197:P198"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["Q197:R198"].Merge = true;
                            ws.Cells["Q197:R198"].Value = "DN #";
                            ws.Cells["Q197:R198"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["Q197:R198"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                            for (int K = 199; K < 219; K++)
                            {
                                ws.Cells["A" + K + ":A" + K + ""].Merge = true;
                                ws.Cells["A" + K + ":A" + K + ""].Value = "";
                                ws.Cells["A" + K + ":A" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["B" + K + ":C" + K + ""].Merge = true;
                                ws.Cells["B" + K + ":C" + K + ""].Value = "";
                                ws.Cells["D" + K + ":E" + K + ""].Merge = true;
                                ws.Cells["D" + K + ":E" + K + ""].Value = "";
                                ws.Cells["D" + K + ":E" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["F" + K + ":F" + K + ""].Merge = true;
                                ws.Cells["F" + K + ":F" + K + ""].Value = "";
                                ws.Cells["F" + K + ":F" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["G" + K + ":G" + K + ""].Merge = true;
                                ws.Cells["G" + K + ":G" + K + ""].Value = "";
                                ws.Cells["G" + K + ":G" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["H" + K + ":I" + K + ""].Merge = true;
                                ws.Cells["H" + K + ":I" + K + ""].Value = "";
                                ws.Cells["J" + K + ":J" + K + ""].Merge = true;
                                ws.Cells["J" + K + ":J" + K + ""].Value = "";
                                ws.Cells["J" + K + ":J" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["K" + K + ":L" + K + ""].Merge = true;
                                ws.Cells["K" + K + ":L" + K + ""].Value = "";
                                ws.Cells["K" + K + ":L" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["M" + K + ":M" + K + ""].Merge = true;
                                ws.Cells["M" + K + ":M" + K + ""].Value = "";
                                ws.Cells["M" + K + ":M" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["N" + K + ":O" + K + ""].Merge = true;
                                ws.Cells["N" + K + ":O" + K + ""].Value = "";
                                ws.Cells["N" + K + ":O" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["P" + K + ":P" + K + ""].Merge = true;
                                ws.Cells["P" + K + ":P" + K + ""].Value = "";
                                ws.Cells["P" + K + ":P" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["Q" + K + ":R" + K + ""].Merge = true;
                                ws.Cells["Q" + K + ":R" + K + ""].Value = "";
                                ws.Cells["A" + K + ":R" + K + ""].Style.Border.Right.Style = ExcelBorderStyle.Hair;
                                ws.Cells["A" + K + ":R" + K + ""].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                                ws.Cells["R" + K + ""].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            }

                            int x2 = 0;
                            int Rcount = (199 + stack[x].RDeliveryList.Count());
                            //int AboveSubtituteCount = 0;
                            if (stack[x].RDeliveryList.Count() != 0)
                            {
                                for (int K = 199; K < Rcount; K++)
                                {
                                    ws.Cells["A" + K + ":A" + K + ""].Merge = true;
                                    ws.Cells["A" + K + ":A" + K + ""].Value = stack[x].RDeliveryList[x2].SubstituteItemCode;
                                    ws.Cells["A" + K + ":A" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells["B" + K + ":C" + K + ""].Merge = true;
                                    ws.Cells["B" + K + ":C" + K + ""].Value = stack[x].RDeliveryList[x2].SubstituteItemName;
                                    ws.Cells["D" + K + ":E" + K + ""].Merge = true;
                                    ws.Cells["D" + K + ":E" + K + ""].Value = Math.Round(stack[x].RDeliveryList[x2].DeliveredQty, 3);
                                    ws.Cells["D" + K + ":E" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    ws.Cells["F" + K + ":F" + K + ""].Merge = true;
                                    ws.Cells["F" + K + ":F" + K + ""].Value = "$" + Math.Round(stack[x].RDeliveryList[x2].SubstituteSectorPrice, 2);
                                    ws.Cells["F" + K + ":F" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells["G" + K + ":G" + K + ""].Merge = true;
                                    ws.Cells["G" + K + ":G" + K + ""].Value = stack[x].RDeliveryList[x2].UNCode;
                                    ws.Cells["G" + K + ":G" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells["H" + K + ":I" + K + ""].Merge = true;
                                    ws.Cells["H" + K + ":I" + K + ""].Value = stack[x].RDeliveryList[x2].Commodity;
                                    ws.Cells["J" + K + ":J" + K + ""].Merge = true;
                                    ws.Cells["J" + K + ":J" + K + ""].Value = Math.Round(stack[x].RDeliveryList[x2].OrderedQty, 3);
                                    ws.Cells["J" + K + ":J" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    ws.Cells["K" + K + ":L" + K + ""].Merge = true;
                                    ws.Cells["K" + K + ":L" + K + ""].Value = Math.Round(stack[x].RDeliveryList[x2].InvoiceQty, 3);
                                    ws.Cells["K" + K + ":L" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    ws.Cells["M" + K + ":M" + K + ""].Merge = true;
                                    ws.Cells["M" + K + ":M" + K + ""].Value = "$" + Math.Round(stack[x].RDeliveryList[x2].SectorPrice, 2);
                                    ws.Cells["M" + K + ":M" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells["N" + K + ":O" + K + ""].Merge = true;

                                    if (stack[x].RDeliveryList[x2].APLWeight >= 98) { AboveSubtituteCount = AboveSubtituteCount + 1; }

                                    ws.Cells["N" + K + ":O" + K + ""].Value = "$" + Math.Round(stack[x].RDeliveryList[x2].AcceptedAmt, 2);
                                    ws.Cells["N" + K + ":O" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    ws.Cells["P" + K + ":P" + K + ""].Merge = true;
                                    ws.Cells["P" + K + ":P" + K + ""].Value = Math.Round(stack[x].RDeliveryList[x2].APLWeight, 2) + "%";
                                    ws.Cells["P" + K + ":P" + K + ""].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    ws.Cells["Q" + K + ":R" + K + ""].Merge = true;
                                    ws.Cells["Q" + K + ":R" + K + ""].Value = stack[x].RDeliveryList[x2].DeliveryNoteName;
                                    ws.Cells["A" + K + ":R" + K + ""].Style.Border.Right.Style = ExcelBorderStyle.Hair;
                                    ws.Cells["A" + K + ":R" + K + ""].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                                    ws.Cells["R" + K + ""].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                                    x2 = x2 + 1;
                                }
                            }

                            ws.Cells["A219:A219"].Merge = true;
                            ws.Cells["A219:A219"].Value = x2;
                            ws.Cells["A219:A219"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["B219:C219"].Merge = true;
                            ws.Cells["B219:C219"].Value = "Sub Total";
                            ws.Cells["D219:E219"].Merge = true;
                            ws.Cells["D219:E219"].Value = string.Format("{0:N}", string.Format("{0:0.000}", (Math.Round(stack[x].RDeliveryQuantity, 3))));
                            ws.Cells["F219:F219"].Merge = true;
                            ws.Cells["F219:F219"].Value = "";
                            ws.Cells["G219:G219"].Merge = true;
                            ws.Cells["G219:G219"].Value = "";
                            ws.Cells["H219:I219"].Merge = true;
                            ws.Cells["H219:I219"].Value = "";
                            ws.Cells["J219:J219"].Merge = true;
                            ws.Cells["J219:J219"].Value = string.Format("{0:N}", string.Format("{0:0.000}", (Math.Round(stack[x].ROrderedQuantity, 3))));
                            ws.Cells["K219:L219"].Merge = true;
                            ws.Cells["K219:L219"].Value = string.Format("{0:N}", string.Format("{0:0.000}", (Math.Round(stack[x].RAcceptedQuantity, 3))));
                            ws.Cells["M219:M219"].Merge = true;
                            ws.Cells["M219:M219"].Value = "";
                            ws.Cells["N219:O219"].Merge = true;
                            ws.Cells["N219:O219"].Value = "$  " + string.Format("{0:N}", (Math.Round(stack[x].RAcceptedamt, 2)));
                            ws.Cells["P219:P219"].Merge = true;
                            ws.Cells["P219:P219"].Value = "";
                            ws.Cells["Q219:R219"].Merge = true;
                            ws.Cells["Q219:R219"].Value = "";
                            ws.Cells["A219:R219"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A219:R219"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A219:R219"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A219:R219"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["A219:R219"].Style.Fill.BackgroundColor.SetColor(BlueHex);
                            ws.Cells["A219:R219"].Style.Font.Color.SetColor(Color.White);


                            #endregion

                            #region Summary
                            ws.Cells["A222:D222"].Merge = true;
                            ws.Cells["A222:D222"].Value = "Number of Days Delay";
                            ws.Cells["E222:F222"].Merge = true;
                            ws.Cells["E222:F222"].Value = stack[x].TotalDays;
                            ws.Cells["A223:D223"].Merge = true;
                            ws.Cells["A223:D223"].Value = "Total Line Items ordered";
                            ws.Cells["E223:F223"].Merge = true;
                            ws.Cells["E223:F223"].Value = temp;
                            ws.Cells["A224:D224"].Merge = true;
                            ws.Cells["A224:D224"].Value = "Total Line Items Received >= 98";
                            ws.Cells["E224:F224"].Merge = true;
                            ws.Cells["E224:F224"].Value = (stack[x].AboveCount + AboveSubtituteCount);
                            ws.Cells["A225:D225"].Merge = true;
                            ws.Cells["A225:D225"].Value = "Order Quantity";
                            ws.Cells["E225:F225"].Merge = true;
                            ws.Cells["E225:F225"].Value = stack[x].TotalOrderedQtySum;
                            ws.Cells["A226:D226"].Merge = true;
                            ws.Cells["A226:D226"].Value = "Delivered  Quantity";
                            ws.Cells["E226:F226"].Merge = true;
                            //ws.Cells["E226:F226"].Value = stack[x].TotalDeliveredQtySum + stack[x].SDeliveryQuantity + stack[x].RDeliveryQuantity;
                            ws.Cells["E226:F226"].Value = stack[x].TotalDeliveredQtySum;
                            ws.Cells["A227:D227"].Merge = true;
                            ws.Cells["A227:D227"].Value = "Accepted Quantity";
                            ws.Cells["E227:F227"].Merge = true;
                            //ws.Cells["E227:F227"].Value = stack[x].TotalInvoiceQtySum + stack[x].SAcceptedQuantity + stack[x].RAcceptedQuantity;
                            ws.Cells["E227:F227"].Value = stack[x].TotalAcceptedQtySum;
                            ws.Cells["A228:D228"].Merge = true;
                            ws.Cells["A228:D228"].Value = "Number Line Items Ordered";
                            ws.Cells["E228:F228"].Merge = true;
                            //ws.Cells["E228:F228"].Value = (temp - tempSub) + Convert.ToInt64(stack[x].UNItemCount);
                            ws.Cells["E228:F228"].Value = temp;
                            ws.Cells["A229:D229"].Merge = true;
                            ws.Cells["A229:D229"].Value = "Number of Authorized substitutions";
                            ws.Cells["E229:F229"].Merge = true;
                            ws.Cells["E229:F229"].Value = stack[x].SubstituteCount;

                            ws.Cells["I222:I225"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["I222:L222"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                            ws.Cells["L222:L225"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["I225:L225"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                            ws.Cells["A222:F229"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            ws.Cells["A222:F229"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["A222:F222"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                            ws.Cells["F222:F229"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A229:F229"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                            ws.Cells["I222:J222"].Merge = true;
                            ws.Cells["I222:J222"].Value = "Applicable CMR";
                            ws.Cells["K222:L222"].Merge = true;
                            ws.Cells["K222:L222"].Value = stack[x].AuthorizedCMR;
                            ws.Cells["K222:L222"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            ws.Cells["I223:J223"].Merge = true;
                            ws.Cells["I223:J223"].Value = "Order CMR";
                            ws.Cells["K223:L223"].Merge = true;

                            decimal OrderCMR = Math.Round((stack[x].OrdervalueSum / stack[x].ManDays), 2);

                            ws.Cells["K223:L223"].Value = OrderCMR;
                            ws.Cells["K223:L223"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            ws.Cells["I224:J224"].Merge = true;
                            ws.Cells["I224:J224"].Value = "Accepted CMR ";
                            ws.Cells["K224:L224"].Merge = true;

                            decimal AcceptedCMR = Math.Round((((stack[x].NetAmountSum) + stack[x].SAcceptedamt + stack[x].RAcceptedamt) / stack[x].ManDays), 2);

                            ws.Cells["K224:L224"].Value = AcceptedCMR;
                            ws.Cells["K224:L224"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            ws.Cells["I225:J225"].Merge = true;
                            ws.Cells["I225:J225"].Value = "% Of CMR Utilized";
                            ws.Cells["K225:L225"].Merge = true;

                            Decimal CmRUtilised = 0;
                            if ((AcceptedCMR != 0) && (stack[x].AuthorizedCMR != 0))
                            {
                                CmRUtilised = Math.Round((AcceptedCMR / OrderCMR) * 100, 2);
                            }


                            ws.Cells["K225:L225"].Value = CmRUtilised + "%";
                            ws.Cells["K225:L225"].Style.Font.Bold = true;
                            ws.Cells["K225:L225"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            ws.Cells["I222:L225"].Style.Border.Diagonal.Style = ExcelBorderStyle.Medium;
                            ws.Cells["I222:L225"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["I222:L225"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                            ws.Cells["I227:M227"].Merge = true;
                            ws.Cells["I227:M227"].Value = "Food order value for APL Purposes";

                            ws.Cells["N227:O227"].Merge = true;
                            ws.Cells["N227:O227"].Value = "$  " + string.Format("{0:N}", stack[x].OrdervalueSum);
                            ws.Cells["N227:O227"].Style.Numberformat.Format = "#,##0.00";
                            ws.Cells["N227:O227"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            ws.Cells["I227:O227"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["I227:O227"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                            ws.Cells["I227:O227"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                            ws.Cells["I227:O227"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            #endregion
                            #region Performance
                            ws.Cells["A231:T231"].Merge = true;
                            ws.Cells["A231:T231"].Value = "Performance Details";
                            ws.Cells["A231:T231"].Style.Font.Italic = true;
                            ws.Cells["A231:T231"].Style.Font.Bold = true;
                            ws.Cells["A231:T231"].Style.Font.Color.SetColor(Color.DarkRed);
                            ws.Cells["A231:T231"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                            ws.Cells["A232:L232"].Merge = true;
                            ws.Cells["A232:L232"].Value = "Amount at risk in terms of a percentage of a Weekly invoice (Rations) 'C'  for each Delivery Point  15%";
                            ws.Cells["A232:L232"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            ws.Cells["M232:T232"].Merge = true;

                            //decimal deliveryPoint = Math.Round(((stack[x].SAcceptedamt + stack[x].RAcceptedamt + stack[x].NetAmountSum) / 100) * 15, 2);
                            decimal deliveryPoint = Math.Round(((stack[x].NetRationAmount) / 100) * 15, 2);

                            ws.Cells["M232:T232"].Value = "$  " + string.Format("{0:N}", deliveryPoint);
                            ws.Cells["M232:T232"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                            ws.Cells["A233:D233"].Merge = true;
                            ws.Cells["A233:D233"].Value = "Performance Details";

                            ws.Cells["E233:E233"].Merge = true;
                            ws.Cells["E233:E233"].Value = "APL Code";

                            ws.Cells["F233:F233"].Merge = true;
                            ws.Cells["F233:F233"].Value = "Target %";

                            ws.Cells["G233:H233"].Merge = true;
                            ws.Cells["G233:H233"].Value = "Acceptable";
                            ws.Cells["I233:L233"].Merge = true;
                            ws.Cells["I233:L233"].Value = "Band (level of performance)";
                            ws.Cells["M233:P233"].Merge = true;
                            ws.Cells["M233:P233"].Value = "Service Level Credit - % of invoice";
                            ws.Cells["Q233:R233"].Merge = true;
                            ws.Cells["Q233:R233"].Value = "Performance";
                            ws.Cells["S233:T233"].Merge = true;
                            ws.Cells["S233:T233"].Value = "Deduction";
                            ws.Cells["A233:T233"].Style.Font.Bold = true;

                            ws.Cells["A234:D239"].Merge = true;
                            ws.Cells["A234:D239"].Value = "1. Conformity to Delivery Schedule";
                            ws.Cells["A234:D239"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["A234:D239"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["A234:D239"].Style.WrapText = true;

                            ws.Cells["E234:E239"].Merge = true;
                            ws.Cells["E234:E239"].Value = "S";

                            ws.Cells["F234:F239"].Merge = true;
                            ws.Cells["F234:F239"].Style.WrapText = true;
                            ws.Cells["F234:F239"].Value = "On time delivery";

                            ws.Cells["G234:H239"].Merge = true;
                            ws.Cells["G234:H239"].Value = "On time";
                            ws.Cells["I234:J236"].Merge = true;
                            ws.Cells["I234:J236"].Value = "1 day delay";
                            ws.Cells["I237:J239"].Style.Border.Top.Style = ExcelBorderStyle.Hair;
                            ws.Cells["I237:J239"].Merge = true;
                            ws.Cells["I237:J239"].Value = "2 day delay";
                            ws.Cells["K234:L236"].Merge = true;
                            ws.Cells["K234:L236"].Value = "40.00%";

                            ws.Cells["K237:L239"].Merge = true;
                            ws.Cells["K237:L239"].Value = "100.00%";
                            ws.Cells["M234:N236"].Merge = true;
                            ws.Cells["M234:N236"].Value = "1.20%";

                            ws.Cells["M237:N239"].Merge = true;
                            ws.Cells["M237:N239"].Value = "3.00%";
                            ws.Cells["O234:P236"].Merge = true;

                            ws.Cells["O237:P239"].Merge = true;
                            ws.Cells["Q234:R239"].Merge = true;
                            //ws.Cells["Q234:R239"].Value = stack[x].DeliveryPerformance + "%";                          //Performance
                            ws.Cells["Q234:R239"].Value = stack[x].DeliveryPerformance;                          //Performance
                            //ws.Cells["Q234:R239"].Value = string.Format("{0:N}", string.Format("{0:0.00}", (Math.Round(stack[x].DeliveryPerformance, 2))) + "%");
                            ws.Cells["S234:T239"].Merge = true;
                            ws.Cells["S234:T239"].Value = "- $  " + string.Format("{0:N}", stack[x].DeliveryDeduction);                          //Day Deduction


                            ws.Cells["A240:D245"].Merge = true;
                            ws.Cells["A240:D245"].Value = "'2. Conformity to Order by Line Items Number of line items ordered is delivered (including authorized substitutions)' ";
                            ws.Cells["A240:D245"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["A240:D245"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["A240:D245"].Style.WrapText = true;

                            ws.Cells["E240:E245"].Merge = true;
                            ws.Cells["E240:E245"].Value = "L";

                            ws.Cells["F240:F245"].Merge = true;
                            ws.Cells["F240:F245"].Value = "100%";
                            ws.Cells["G240:H245"].Merge = true;
                            ws.Cells["G240:H245"].Value = "98%";
                            ws.Cells["I240:J242"].Merge = true;
                            ws.Cells["I240:J242"].Value = "95% - <98%";
                            ws.Cells["I243:J245"].Merge = true;
                            ws.Cells["I243:J245"].Value = "<92% - <95%";
                            ws.Cells["K240:L242"].Merge = true;
                            ws.Cells["K240:L242"].Value = "40.00%";
                            ws.Cells["K243:L245"].Merge = true;
                            ws.Cells["K243:L245"].Value = "100.00%";
                            ws.Cells["M240:N242"].Merge = true;
                            ws.Cells["M240:N242"].Value = "1.20%";
                            ws.Cells["M243:N245"].Merge = true;
                            ws.Cells["M243:N245"].Value = "3.00%";

                            ws.Cells["O240:P242"].Merge = true;
                            ws.Cells["O243:P245"].Merge = true;
                            ws.Cells["Q240:R245"].Merge = true;
                            //ws.Cells["Q240:R245"].Value = Math.Round(stack[x].LineItemPerformance, 2) + "%";                        //Performance
                            ws.Cells["Q240:R245"].Value = string.Format("{0:N}", string.Format("{0:0.00}", (Math.Round(stack[x].LineItemPerformance, 2))) + "%");
                            ws.Cells["S240:T245"].Merge = true;
                            ws.Cells["S240:T245"].Value = "- $  " + string.Format("{0:N}", stack[x].LineItemDeduction);                         //Day Deduction


                            ws.Cells["A246:D251"].Merge = true;
                            ws.Cells["A246:D251"].Value = "3. Conformity to Orders by weight:  Quantity kg/ltr/each Quantity of food order in Kg/Ltr is delivered (including authorized substitutions)";
                            ws.Cells["A246:D251"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["A246:D251"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["A246:D251"].Style.WrapText = true;

                            ws.Cells["E246:E251"].Merge = true;
                            ws.Cells["E246:E251"].Value = "Q";

                            ws.Cells["F246:F251"].Merge = true;
                            ws.Cells["F246:F251"].Value = "100%";
                            ws.Cells["G246:H251"].Merge = true;
                            ws.Cells["G246:H251"].Value = "95%";
                            ws.Cells["I246:J248"].Merge = true;
                            ws.Cells["I246:J248"].Value = "92% - <95%";
                            ws.Cells["I249:J251"].Merge = true;
                            ws.Cells["I249:J251"].Value = "<90% - <92%";
                            ws.Cells["K246:L248"].Merge = true;
                            ws.Cells["K246:L248"].Value = "40.00%";
                            ws.Cells["K249:L251"].Merge = true;
                            ws.Cells["K249:L251"].Value = "100.00%";
                            ws.Cells["M246:N248"].Merge = true;
                            ws.Cells["M246:N248"].Value = "1.80%";
                            ws.Cells["M249:N251"].Merge = true;
                            ws.Cells["M249:N251"].Value = "4.50%";
                            ws.Cells["O246:P248"].Merge = true;
                            ws.Cells["O249:P251"].Merge = true;
                            ws.Cells["Q246:R251"].Merge = true;
                            //ws.Cells["Q246:R251"].Value = stack[x].OrderWightPerformance + "%";                       //Performance
                            ws.Cells["Q246:R251"].Value = string.Format("{0:N}", string.Format("{0:0.00}", (Math.Round(stack[x].OrderWightPerformance, 2))) + "%");
                            ws.Cells["S246:T251"].Merge = true;
                            ws.Cells["S246:T251"].Value = "- $  " + string.Format("{0:N}", stack[x].OrderWightDeduction);                       //Day Deduction

                            ws.Cells["A252:D257"].Merge = true;
                            ws.Cells["A252:D257"].Value = "4. Food Order Compliance-Availability :  Number of  authorized substitutions";
                            ws.Cells["A252:D257"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["A252:D257"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["A252:D257"].Style.WrapText = true;

                            ws.Cells["E252:E257"].Merge = true;
                            ws.Cells["E252:E257"].Value = "A";

                            ws.Cells["F252:F257"].Merge = true;
                            ws.Cells["F252:F257"].Value = "0%";
                            ws.Cells["G252:H257"].Merge = true;
                            ws.Cells["G252:H257"].Value = "3%";
                            ws.Cells["I252:J254"].Merge = true;
                            ws.Cells["I252:J254"].Value = "3% - <4%";
                            ws.Cells["I255:J257"].Merge = true;
                            ws.Cells["I255:J257"].Value = "4% - 5% +";
                            ws.Cells["K252:L254"].Merge = true;
                            ws.Cells["K252:L254"].Value = "40.00%";
                            ws.Cells["K255:L257"].Merge = true;
                            ws.Cells["K255:L257"].Value = "100.00%";
                            ws.Cells["M252:N254"].Merge = true;
                            ws.Cells["M252:N254"].Value = "1.80%";
                            ws.Cells["M255:N257"].Merge = true;
                            ws.Cells["M255:N257"].Value = "4.50%";//
                            ws.Cells["O252:P254"].Merge = true;
                            ws.Cells["O255:P257"].Merge = true;
                            ws.Cells["Q252:R257"].Merge = true;
                            //ws.Cells["Q252:R257"].Value = stack[x].SubtitutionPerformance + "%";                         //Performance
                            ws.Cells["Q252:R257"].Value = string.Format("{0:N}", string.Format("{0:0.00}", (Math.Round(stack[x].SubtitutionPerformance, 2))) + "%");
                            ws.Cells["S252:T257"].Merge = true;
                            ws.Cells["S252:T257"].Value = "- $  " + string.Format("{0:N}", stack[x].SubtitutionDeduction);                           //Day Deduction


                            ws.Cells["A258:T260"].Merge = true;
                            ws.Cells["A258:T260"].Value = "Methodology:  Verification shall be determined by the UN in its sole discretion.  The service credit shall be computed according to the following formula:  Service Level Credit = A x B x C ( A = Allocation (% of at risk amount) B =% of allocation C = At risk amount )";
                            ws.Cells["A258:T260"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["A258:T260"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells["A258:T260"].Style.WrapText = true;

                            ws.Cells["A232:T260"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A232:T260"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A232:T260"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A232:T232"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["A232:T232"].Style.Fill.BackgroundColor.SetColor(BlueHex);
                            ws.Cells["A232:T232"].Style.Font.Color.SetColor(Color.White);

                            ws.Cells["A233:T260"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["A233:T260"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                            ws.Cells["I237:P237"].Style.Border.Top.Style = ExcelBorderStyle.Hair;
                            ws.Cells["I243:P243"].Style.Border.Top.Style = ExcelBorderStyle.Hair;
                            ws.Cells["I249:P249"].Style.Border.Top.Style = ExcelBorderStyle.Hair;
                            ws.Cells["I255:P255"].Style.Border.Top.Style = ExcelBorderStyle.Hair;

                            ws.Cells["J234:J257"].Style.Border.Right.Style = ExcelBorderStyle.Hair;
                            ws.Cells["N234:N257"].Style.Border.Right.Style = ExcelBorderStyle.Hair;

                            ws.Cells["A262"].Value = "NPA01 - Quantity under-delivered not relevant";
                            ws.Cells["A263"].Value = "NPA02 - Quantity ordered does not meet packing size requirements";
                            ws.Cells["A264"].Value = "NPA03 - Unreasonable denied access to delivery points";
                            ws.Cells["A265"].Value = "NPA04 - UN proposed substitution";
                            ws.Cells["A266"].Value = "NPA05 – Other";
                            ws.Cells["A267"].Value = "AS- Authorized Substitution";
                            ws.Cells["A268"].Value = "AR – Authorized Replacement ";
                            #endregion

                            ws.Cells["Q262:R262"].Merge = true;
                            ws.Cells["Q262:R262"].Value = "APL Deductions";
                            ws.Cells["S262:T262"].Merge = true;

                            decimal AplDetTotal = Math.Round((stack[x].DeliveryDeduction + stack[x].LineItemDeduction + stack[x].OrderWightDeduction + stack[x].SubtitutionDeduction), 2);

                            ws.Cells["S262:T262"].Value = "- $  " + string.Format("{0:N}", AplDetTotal);
                            ws.Cells["S262:T262"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            ws.Cells["Q262:T262"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["Q262:T262"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                            ws.Cells["Q262:T262"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                            ws.Cells["Q262:T262"].Style.Border.Left.Style = ExcelBorderStyle.Medium;

                            decimal AmtAccept = Math.Round((stack[x].NetAmountSum + stack[x].SAcceptedamt + stack[x].RAcceptedamt), 2);
                            decimal tropstgh = Math.Round((0 * AplDetTotal), 2);
                            decimal OtherCreditNote = Math.Round((0 * AplDetTotal), 2);
                            //decimal Weeklywise = Math.Round((0 * AplDetTotal), 2);
                            //decimal NetRationAmt = Math.Round((tropstgh + OtherCreditNote + Weeklywise + AmtAccept), 2);

                            //decimal Weeklywise = 0;
                            //Weeklywise=Math.Round((Convert.ToDecimal(0.35 / 100) * AmtAccept), 2);

                            //decimal AccAmtTransportation = 0;
                            //AccAmtTransportation = Math.Round(Convert.ToDecimal(0.31 / 100) * (stack[x].TotalInvoiceQtySum + stack[x].SAcceptedQuantity + stack[x].RAcceptedQuantity), 2);

                            //decimal NetRationAmt = Math.Round((AmtAccept - Weeklywise), 2);
                            //decimal confirmityCMR = 0;
                            //if (stack[x].AcceptedCMR > stack[x].AuthorizedCMR)
                            //    confirmityCMR = Math.Round((stack[x].AcceptedCMR - stack[x].AuthorizedCMR) * stack[x].Strength * 7, 2);
                            decimal AplDetect = Math.Round(@AplDetTotal, 2);

                            decimal TotalInvoice = Math.Round((stack[x].NetRationAmount - AplDetect - stack[x].confirmityCMR + stack[x].AcceptedTransportCost), 2);

                            ws.Cells["M266:O266"].Merge = true;
                            ws.Cells["M266:O266"].Value = " Amount  Accepted";
                            ws.Cells["P266:Q266"].Merge = true;
                            ws.Cells["R266:T266"].Merge = true;
                            //ws.Cells["R266:T266"].Value = "$  " + string.Format("{0:N}", AmtAccept);
                            ws.Cells["R266:T266"].Value = "$  " + string.Format("{0:N}", stack[x].AmountAccepted);
                            ws.Cells["R266:T266"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            ws.Cells["M267:O267"].Merge = true;
                            ws.Cells["M267:O267"].Value = "Confirmity to CMR";
                            ws.Cells["P267:Q267"].Merge = true;
                            ws.Cells["R267:T267"].Merge = true;
                            //ws.Cells["R267:T267"].Value = "- $  " + string.Format("{0:N}", confirmityCMR);
                            ws.Cells["R267:T267"].Value = "- $  " + string.Format("{0:N}", stack[x].confirmityCMR);
                            ws.Cells["R267:T267"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            ws.Cells["M268:O268"].Merge = true;
                            ws.Cells["M268:O268"].Value = "Troops Strength";
                            ws.Cells["P268:Q268"].Merge = true;
                            ws.Cells["R268:T268"].Merge = true;
                            ws.Cells["R268:T268"].Value = "$  " + string.Format("{0:N}", tropstgh);
                            ws.Cells["R268:T268"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            ws.Cells["M269:O269"].Merge = true;
                            ws.Cells["M269:O269"].Value = "Other Credit Notes";
                            ws.Cells["P269:Q269"].Merge = true;
                            ws.Cells["R269:T269"].Merge = true;
                            ws.Cells["R269:T269"].Value = "$  " + string.Format("{0:N}", OtherCreditNote);
                            ws.Cells["R269:T269"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            ws.Cells["M270:O270"].Merge = true;
                            ws.Cells["M270:O270"].Value = "Weekly Invoice";
                            ws.Cells["P270:Q270"].Merge = true;
                            ws.Cells["P270:Q270"].Value = "-0.35%";
                            ws.Cells["R270:T270"].Merge = true;
                            //ws.Cells["R270:T270"].Value = "- $  " + string.Format("{0:N}", Weeklywise);
                            ws.Cells["R270:T270"].Value = "- $  " + string.Format("{0:N}", stack[x].Weeklyinvoicediscount);
                            ws.Cells["R270:T270"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            ws.Cells["M271:O271"].Merge = true;
                            ws.Cells["M271:O271"].Value = "Net amount for Rations";
                            ws.Cells["P271:Q271"].Merge = true;
                            ws.Cells["R271:T271"].Merge = true;
                            //ws.Cells["R271:T271"].Value = "$  " + string.Format("{0:N}", NetRationAmt);
                            ws.Cells["R271:T271"].Value = "$  " + string.Format("{0:N}", stack[x].NetRationAmount);
                            ws.Cells["R271:T271"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            ws.Cells["M272:O272"].Merge = true;
                            ws.Cells["M272:O272"].Value = "Accepted Amount Transportation";
                            ws.Cells["P272:Q272"].Merge = true;
                            ws.Cells["P272:Q272"].Value = stack[x].RatePerKg;
                            ws.Cells["R272:T272"].Merge = true;
                            //ws.Cells["R272:T272"].Value = "$  " + string.Format("{0:N}", AccAmtTransportation);
                            ws.Cells["R272:T272"].Value = "$  " + string.Format("{0:N}", stack[x].AcceptedTransportCost);
                            ws.Cells["R272:T272"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            ws.Cells["M273:O273"].Merge = true;
                            ws.Cells["M273:O273"].Value = "APL Deductions";
                            ws.Cells["P273:Q273"].Merge = true;
                            ws.Cells["R273:T273"].Merge = true;
                            ws.Cells["R273:T273"].Value = "- $  " + string.Format("{0:N}", AplDetect);
                            ws.Cells["R273:T273"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            ws.Cells["M274:T274"].Merge = true;
                            ws.Cells["M274:T274"].Value = "";

                            ws.Cells["M275:Q275"].Merge = true;
                            ws.Cells["M275:Q275"].Value = "Total Invoice";
                            ws.Cells["R275:T275"].Merge = true;
                            ws.Cells["R275:T275"].Value = "$  " + string.Format("{0:N}", TotalInvoice);
                            ws.Cells["R275:T275"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;




                            ws.Cells["M266:T275"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            ws.Cells["M266:T275"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            ws.Cells["M266:T275"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["M266:T275"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                            ws.Cells["E166:Z168"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            ws.Cells["J166:O168"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["J166:O168"].Style.Fill.BackgroundColor.SetColor(WhiteHex);
                            ws.Cells["J166:O168"].Style.Font.Color.SetColor(OrangeHex);

                            ws.Cells["J166:O168,Y166:AC168,R166:U168,F225:F227,N227:O227,E225:F225,E226:F226,E227:F227"].Style.Numberformat.Format = "#,##0.00";


                            ws.Cells["Y166:AC168"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["Y166:AC168"].Style.Fill.BackgroundColor.SetColor(WhiteHex);
                            ws.Cells["Y166:AC168"].Style.Font.Color.SetColor(OrangeHex);

                            ws.Cells["R166:U168"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["R166:U168"].Style.Fill.BackgroundColor.SetColor(WhiteHex);
                            ws.Cells["R166:U168"].Style.Font.Color.SetColor(OrangeHex);


                            ws.Cells["P267:Q267"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["P267:Q267"].Style.Fill.BackgroundColor.SetColor(BottomHex);

                            ws.Cells["M274:T274"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["M274:T274"].Style.Fill.BackgroundColor.SetColor(AshHex);

                            ws.Cells["M266:T275"].Style.Font.Bold = true;
                            ws.Cells["M266:T266"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                            ws.Cells["T266:T275"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["M275:T275"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                            ws.Cells["M266:M275"].Style.Border.Left.Style = ExcelBorderStyle.Medium;

                            ws.Cells["A276"].Value = "Disclaimer:";
                            ws.Cells["A277"].Value = "In the interest of ensuring a smooth invoicing/payment process, GCC SERVICES herewith signs this GRR with the intent to officially review the Weekly Billing Discount and APL formulas.  We will submit a correction/recovery request as applicable.";

                            //ws.Cells["J16:Z165,E174:F194,J174:P194,E199:F219,J199:P219"].Style.Numberformat.Format = "#,##0.00";
                            //ws.Cells["J16:Z165,E174:F194,J174:P194,E199:F219,J199:P219"].Style.Numberformat.Format = "#,##0.000";//Working
                            ws.Cells["J16:Z165,D174:D194,J174:K194,D199:D219,J199:K219,E225:E227,J166:N168"].Style.Numberformat.Format = "#,##0.000";

                            x = x + 1;
                            #endregion
                        }
                    }
                    string txtTName = InvoiceNo;
                    if ((stack != null) && (Single == true))
                    {
                        string path = Response.OutputStream.ToString();
                        byte[] data = pck.GetAsByteArray();


                        ExcelDocuments XD = IS.GetExcelDocumentsDetailsByControlId(stack[0].UNID);
                        if (XD != null)
                        {
                            XD.DocumentData = data;
                            long id = IS.SaveOrUpdateExcelDocuments(XD, "Binoe");
                        }
                        else if (Single == true)
                        {

                            InvoiceManagementView Imv = IS.GetInvoiceManagementViewDetailsByControlId(stack[0].UNID);
                            ExcelDocuments ed = new ExcelDocuments();
                            ed.ControlId = Imv.ControlId;
                            ed.OrderId = Imv.OrderId;
                            ed.InvoiceId = Imv.Id;
                            ed.Location = Imv.Location;
                            ed.Name = Imv.Name;
                            ed.ContingentType = Imv.ContingentType;
                            ed.Period = Imv.Period;
                            ed.PeriodYear = Imv.PeriodYear;
                            ed.Sector = Imv.Sector;
                            ed.Week = Imv.Week;
                            ed.DocumentData = data;
                            ed.DocumentType = "Excel-Single";
                            ed.DocumentName = stack[0].Reference;
                            long id = IS.SaveOrUpdateExcelDocuments(ed, "Binoe");
                        }
                    }
                    else if ((invoiceList != null) && (Consol == true))
                    {

                        InvoiceList ci = (InvoiceList)invoiceList;
                        var InvNoSplit = ci.InvoiceNo.ToString().Split('-');
                        var Period = ci.Period.ToString().Split('/');
                        //to check weather cosolidate sheet is already exsist or not
                        ExcelDocuments ED = GetExcelDocumentForConsolidate(InvNoSplit[1], InvNoSplit[3], Period[0], Period[1]);

                        byte[] data = pck.GetAsByteArray();

                        if (ED != null && ED.Id > 0)
                        {
                            ED.DocumentData = data;
                            long id = IS.SaveOrUpdateExcelDocuments(ED, "Binoe");
                        }
                        else
                        {
                            ED.ControlId = ci.InvoiceNo;
                            ED.ContingentType = InvNoSplit[3];
                            ED.Period = Period[0];
                            ED.PeriodYear = Period[1];
                            ED.Sector = InvNoSplit[1];
                            ED.DocumentData = data;
                            ED.DocumentType = "Excel-Consol";
                            ED.DocumentName = ci.InvoiceNo;
                            long id = IS.SaveOrUpdateExcelDocuments(ED, "Binoe");
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public SingleInvoice SingleInvoiceListForParallel(long OrderId)
        {
            try
            {
                Orders Ord = OS.GetOrdersById(OrderId);
                DateTime ApprovedDeliverydate = IS.GetApprovedDeliveryDateById(OrderId);
                criteria.Clear();
                criteria.Add("OrderId", OrderId);

                Dictionary<long, IList<SingleInvoiceView>> SingleInvoice2 = IS.GetSingleInvoiceListUsingSP(OrderId);


                //Dictionary<long, IList<SingleInvoiceView>> SingleInvoice2 = IS.GetSingleInvoiceListWithPagingAndCriteria(0, 9999, "UNCode", "Asc", criteria);
                Dictionary<long, IList<SingleInvoiceView>> SingleInvoice = new Dictionary<long, IList<SingleInvoiceView>>();
                long[] LineIds = (from items in SingleInvoice2.First().Value
                                  select items.LineId).Distinct().ToArray();
                criteria.Add("LineId", LineIds);
                Dictionary<long, IList<OrderItems>> OrderList = OS.GetOrderItemsListWithNotInSearchPagingAndCriteria(0, 9999, string.Empty, string.Empty, string.Empty, null, criteria);
                criteria.Clear();
                Dictionary<long, IList<UOMMaster>> UOMMasterList = MS.GetUOMMasterListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);

                List<SingleInvoiceView> OrdList = SingleInvoice2.FirstOrDefault().Value.ToList();

                if (OrderList != null && OrderList.FirstOrDefault().Key > 0)
                {
                    long Count = 1;
                    foreach (var item in OrderList.FirstOrDefault().Value)
                    {

                        string[] UomStr = (from items in UOMMasterList.FirstOrDefault().Value
                                           where items.UNCode == item.UNCode
                                           select items.UOM).Distinct().ToArray();
                        SingleInvoiceView sw = new SingleInvoiceView();
                        sw.LineId = item.LineId;
                        sw.OrderQty = item.OrderQty;
                        sw.UNCode = item.UNCode;
                        sw.Commodity = item.Commodity;
                        sw.DeliveredOrdQty = 0;
                        sw.InvoiceQty = 0;
                        sw.NetAmt = 0;
                        sw.SectorPrice = item.SectorPrice;
                        sw.APLWeight = 0;
                        //sw.DiscrepancyCode = " ";
                        sw.DiscrepancyCode = item.NPACode;
                        sw.NPACode = item.NPACode;
                        if (UomStr != null && UomStr.Length != 0)
                            sw.UOM = UomStr[0];
                        else
                            sw.UOM = string.Empty;
                        sw.OrderValue = Math.Round((item.OrderQty * item.SectorPrice), 2);
                        sw.DeliveryNote = " ";
                        OrdList.Add(sw);
                        Count = Count + 1;
                    }
                }
                SingleInvoice.Add(OrdList.Count(), OrdList);

                IList<SingleInvoiceView> SingleInvoiceList = OrdList;
                IList<SingleInvoiceView> SingleInvoiceList1 = null;
                Dictionary<long, IList<SingleInvoiceView>> SingleInvoice1 = SingleInvoice;
                SingleInvoiceList1 = SingleInvoice1.FirstOrDefault().Value;
                //To find the substitutions qty
                //criteria.Clear();
                //criteria.Add("OrderId", OrderId);
                //Dictionary<long, IList<SubReplacementView>> SubReplaceList = IS.GetSubstitudeReplacementList(0, 9999, "UNCode", string.Empty, criteria);

                Dictionary<long, IList<SubReplacementView>> SubReplaceList = IS.GetSubstitudeReplacementListUsingSP(OrderId);

                criteria.Clear();
                Dictionary<long, IList<ItemMaster>> ItemMasterList = MS.GetItemMasterListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);



                int temp = 1;
                for (int i = 0; i < SingleInvoice1.FirstOrDefault().Value.Count; i++)
                {

                    if (SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode == "AS")
                    {
                        SingleInvoice1.FirstOrDefault().Value[i].DeliveredOrdQty = (decimal)0.00;
                        SingleInvoice1.FirstOrDefault().Value[i].AcceptedOrdQty = (decimal)0.00;
                        SingleInvoice1.FirstOrDefault().Value[i].InvoiceQty = (decimal)0.00;
                        SingleInvoice1.FirstOrDefault().Value[i].NetAmt = (decimal)0.00;
                        SingleInvoice1.FirstOrDefault().Value[i].APLWeight = (decimal)0.00;
                    }
                    else if (SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode == "AR")
                    {
                        SingleInvoice1.FirstOrDefault().Value[i].DeliveredOrdQty = (decimal)0.00;
                        SingleInvoice1.FirstOrDefault().Value[i].AcceptedOrdQty = (decimal)0.00;
                        SingleInvoice1.FirstOrDefault().Value[i].InvoiceQty = (decimal)0.00;
                        SingleInvoice1.FirstOrDefault().Value[i].NetAmt = (decimal)0.00;
                        SingleInvoice1.FirstOrDefault().Value[i].APLWeight = (decimal)0.00;
                    }
                    else if (SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode == "NPA/04")//For UNSubstitution
                    {
                        SingleInvoice1.FirstOrDefault().Value[i].DeliveredOrdQty = (decimal)0.00;
                        SingleInvoice1.FirstOrDefault().Value[i].AcceptedOrdQty = (decimal)0.00;
                        SingleInvoice1.FirstOrDefault().Value[i].InvoiceQty = (decimal)0.00;
                        SingleInvoice1.FirstOrDefault().Value[i].NetAmt = (decimal)0.00;
                        SingleInvoice1.FirstOrDefault().Value[i].APLWeight = (decimal)0.00;
                    }
                    else if (SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode == "OR")
                    {
                        SingleInvoice1.FirstOrDefault().Value[i].DeliveredOrdQty = (decimal)0.00;
                        SingleInvoice1.FirstOrDefault().Value[i].AcceptedOrdQty = (decimal)0.00;
                        SingleInvoice1.FirstOrDefault().Value[i].InvoiceQty = (decimal)0.00;
                        SingleInvoice1.FirstOrDefault().Value[i].NetAmt = (decimal)0.00;
                        SingleInvoice1.FirstOrDefault().Value[i].APLWeight = (decimal)0.00;

                        var TempCode = (from items in SubReplaceList.First().Value
                                        where items.DiscrepancyCode == "OR" && items.UNCode == SingleInvoice1.FirstOrDefault().Value[i].UNCode
                                        select items.DiscCode).ToString();

                        if (TempCode == "SP") { SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode = "AS"; }
                        else if (TempCode == "UNAS") { SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode = "NPA/04"; }//For UNSubstitution
                        else { SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode = "AR"; }
                    }
                    else if (SingleInvoice1.FirstOrDefault().Value[i].UNCode == 1129)
                    {

                        //SingleInvoice1.FirstOrDefault().Value[i].InvoiceQty = Math.Round(SingleInvoice1.FirstOrDefault().Value[i].InvoiceQty / (decimal)0.058824, 0);
                        SingleInvoice1.FirstOrDefault().Value[i].InvoiceQty = (Math.Truncate(SingleInvoice1.FirstOrDefault().Value[i].InvoiceQty * 1000m) / 1000m) / (decimal)0.058824;

                        //SingleInvoice1.FirstOrDefault().Value[i].OrderQty = (Math.Truncate(SingleInvoice1.FirstOrDefault().Value[i].OrderQty * 1000m) / 1000m) / (decimal)0.058824;
                        //SingleInvoice1.FirstOrDefault().Value[i].DeliveredOrdQty = (Math.Truncate(SingleInvoice1.FirstOrDefault().Value[i].DeliveredOrdQty * 1000m) / 1000m) / (decimal)0.058824;
                        //SingleInvoice1.FirstOrDefault().Value[i].AcceptedOrdQty = (Math.Truncate(SingleInvoice1.FirstOrDefault().Value[i].AcceptedOrdQty * 1000m) / 1000m) / (decimal)0.058824;


                        SingleInvoice1.FirstOrDefault().Value[i].NetAmt = SingleInvoice1.FirstOrDefault().Value[i].InvoiceQty * SingleInvoice1.FirstOrDefault().Value[i].SectorPrice;
                        SingleInvoice1.FirstOrDefault().Value[i].OrderValue = SingleInvoice1.FirstOrDefault().Value[i].OrderQty * SingleInvoice1.FirstOrDefault().Value[i].SectorPrice;

                        if (SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode == "NPA 01/02")
                        {
                            SingleInvoice1.FirstOrDefault().Value[i].APLWeight = (decimal)98.00;
                        }
                        else
                        {
                            if (SingleInvoice1.FirstOrDefault().Value[i].OrderQty == (decimal)0.00 || SingleInvoice1.FirstOrDefault().Value[i].InvoiceQty == (decimal)0.00)
                            {
                                SingleInvoice1.FirstOrDefault().Value[i].APLWeight = (decimal)0.00;
                            }
                            else { SingleInvoice1.FirstOrDefault().Value[i].APLWeight = (SingleInvoice1.FirstOrDefault().Value[i].InvoiceQty / SingleInvoice1.FirstOrDefault().Value[i].OrderQty) * 100; }
                        }


                    }

                    //if ( SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode != "AS" && SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode != "AR" && SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode != "NPA/04" && SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode != "OR" && SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode != "NPA02" && SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode != "NPA01")
                    //{
                    //    SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode = SingleInvoice1.FirstOrDefault().Value[i].NPACode != "NCDD" ? SingleInvoice1.FirstOrDefault().Value[i].NPACode : "";
                    //}
                    if (SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode != "AS" && SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode != "AR" && SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode != "OR" && SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode != "NPA/04")
                    {
                        SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode = SingleInvoice1.FirstOrDefault().Value[i].NPACode != "NCDD" ? SingleInvoice1.FirstOrDefault().Value[i].NPACode : "";
                    }
                    if (SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode == "AS" || SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode == "AR" || SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode == "NPA/04")
                    {
                        if (!string.IsNullOrEmpty(SingleInvoice1.FirstOrDefault().Value[i].NPACode) && SingleInvoice1.FirstOrDefault().Value[i].NPACode != "NCDD")
                            SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode = SingleInvoice1.FirstOrDefault().Value[i].NPACode != "NCDD" ? SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode + "/" + SingleInvoice1.FirstOrDefault().Value[i].NPACode : SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode;
                    }
                    if (!string.IsNullOrEmpty(SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode) && SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode != "AS" && SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode != "AR" && SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode != "OR" && SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode != "NPA03" && SingleInvoice1.FirstOrDefault().Value[i].APLWeight < (decimal)98.00)
                    {
                        if (SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode != null && SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode != "NPA/04" && SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode.Contains("NPA02") == true || SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode.Contains("NPA01") == true && SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode.Contains("NPA/04") != true)
                            SingleInvoice1.FirstOrDefault().Value[i].APLWeight = (decimal)98.00;
                    }
                    if (SingleInvoice1.FirstOrDefault().Value[i].APLWeight >= (decimal)102.00)
                    {
                        SingleInvoice1.FirstOrDefault().Value[i].InvoiceQty = Math.Truncate(SingleInvoice1.FirstOrDefault().Value[i].InvoiceQty * 1000m) / 1000m;
                    }
                    SingleInvoice1.FirstOrDefault().Value[i].Id = temp;
                    temp = temp + 1;
                }
                List<SingleInvoiceView> DeliverySingleList = (from items in SingleInvoice1.FirstOrDefault().Value
                                                              select items).OrderBy(i => i.UNCode).Distinct().ToList();

                decimal ordrQtySum = SingleInvoiceList1.Sum(x => x.OrderQty);



                decimal TemOrderedQtySum = 0;
                decimal TempDeliveredQtySum = 0;
                decimal TempAcceptedQtySum = 0;
                decimal TempInvoiceQtySum = 0;

                decimal TempAboveAplWeight = 0;
                decimal TempBelowAplWeight = 0;

                decimal TempEggOrderedQtySum = 0;
                decimal TempEggDeliveredQtySum = 0;
                decimal TempEggAcceptedQtySum = 0;
                decimal TempEggInvoiceQtySum = 0;

                decimal NPAOrderedQtySum = 0;
                decimal NPADeliveredQtySum = 0;
                decimal NPAAcceptedQtySum = 0;
                decimal NPAInvoiceQtySum = 0;
                decimal NPAEggOrderedQtySum = 0;
                decimal NPAEggDeliveredQtySum = 0;
                decimal NPAEggAcceptedQtySum = 0;
                decimal NPAEggInvoiceQtySum = 0;

                TransportInvoice TrnsprtInv = IS.GetTransportInvoiceDetailsByOrderId(OrderId);

                decimal TotalTransportationCost = 0;


                long count = SingleInvoice.First().Key;
                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {

                        if (SingleInvoice1.First().Value[i].UNCode != 1129)
                        {
                            TemOrderedQtySum = TemOrderedQtySum + SingleInvoice1.First().Value[i].OrderQty;
                            TempDeliveredQtySum = TempDeliveredQtySum + SingleInvoice1.First().Value[i].DeliveredOrdQty;
                            TempAcceptedQtySum = TempAcceptedQtySum + SingleInvoice1.First().Value[i].AcceptedOrdQty;
                            TempInvoiceQtySum = TempInvoiceQtySum + SingleInvoice1.First().Value[i].InvoiceQty;
                            #region Transportation cost calculation
                            if (TrnsprtInv != null && TrnsprtInv.Id > 0)
                                TotalTransportationCost = TotalTransportationCost + Math.Round((SingleInvoice1.First().Value[i].InvoiceQty * TrnsprtInv.RatePerKg), 2, MidpointRounding.AwayFromZero);
                            #endregion
                        }
                        else
                        {


                            TempEggOrderedQtySum = TempEggOrderedQtySum + SingleInvoice1.First().Value[i].OrderQty;
                            TempEggDeliveredQtySum = TempEggDeliveredQtySum + SingleInvoice1.First().Value[i].DeliveredOrdQty;
                            TempEggAcceptedQtySum = (TempEggAcceptedQtySum + SingleInvoice1.First().Value[i].AcceptedOrdQty);
                            TempEggInvoiceQtySum = (TempEggInvoiceQtySum + SingleInvoice1.First().Value[i].InvoiceQty);
                            #region Transportation cost calculation
                            if (TrnsprtInv != null && TrnsprtInv.Id > 0)
                                TotalTransportationCost = TotalTransportationCost + Math.Round(((SingleInvoice1.First().Value[i].InvoiceQty * (decimal)0.058824) * TrnsprtInv.RatePerKg), 2, MidpointRounding.AwayFromZero);
                            #endregion
                        }
                        #region NPA Ordered,Delivered,Accepted and Inoice Qty
                        if (!string.IsNullOrEmpty(SingleInvoice1.First().Value[i].NPACode) && SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode.Contains("NPA") == true && !SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode.Contains("NPA") == true)
                        {
                            if (SingleInvoice1.First().Value[i].UNCode != 1129)
                            {
                                NPAOrderedQtySum = NPAOrderedQtySum + SingleInvoice1.First().Value[i].OrderQty;
                                NPADeliveredQtySum = NPADeliveredQtySum + SingleInvoice1.First().Value[i].DeliveredOrdQty;
                                NPAAcceptedQtySum = NPAAcceptedQtySum + SingleInvoice1.First().Value[i].AcceptedOrdQty;
                                NPAInvoiceQtySum = NPAInvoiceQtySum + SingleInvoice1.First().Value[i].InvoiceQty;
                            }
                            else
                            {
                                NPAEggOrderedQtySum = NPAEggOrderedQtySum + SingleInvoice1.First().Value[i].OrderQty;
                                NPAEggDeliveredQtySum = NPAEggDeliveredQtySum + SingleInvoice1.First().Value[i].DeliveredOrdQty;
                                NPAEggAcceptedQtySum = (NPAEggAcceptedQtySum + SingleInvoice1.First().Value[i].AcceptedOrdQty);
                                NPAEggInvoiceQtySum = (NPAEggInvoiceQtySum + SingleInvoice1.First().Value[i].InvoiceQty);
                            }
                        }
                        #endregion


                    }
                }
                decimal OrderedQtySum = TemOrderedQtySum;
                decimal DeliveredQtySum = TempDeliveredQtySum;
                decimal AcceptedQtySum = TempAcceptedQtySum;
                decimal InvoiceQtySum = TempInvoiceQtySum;

                decimal NetAmountSum = SingleInvoiceList1.Sum(x => x.NetAmt);
                decimal OrdervalueSum = Math.Round(SingleInvoiceList1.Sum(x => x.OrderValue), 2);
                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        if ((SingleInvoice1.First().Value[i].DiscrepancyCode != "AS") && (SingleInvoice1.First().Value[i].DiscrepancyCode != "AR") && (SingleInvoice1.First().Value[i].DiscrepancyCode != "NPA/04") && (SingleInvoice1.First().Value[i].APLWeight) >= ((decimal)98))
                        {
                            TempAboveAplWeight = TempAboveAplWeight + 1;
                        }
                        else
                        {
                            TempBelowAplWeight = TempBelowAplWeight + 1;
                        }
                    }
                }
                decimal AboveAplWeightCount = TempAboveAplWeight;
                decimal BelowAplWeightCount = TempBelowAplWeight;
                decimal TotalAplWeightCount = AboveAplWeightCount + BelowAplWeightCount;

                decimal EggOrderedQtySum = TempEggOrderedQtySum * (decimal)0.058824;
                decimal EggDeliveredQtySum = TempEggDeliveredQtySum * (decimal)0.058824;
                decimal EggAcceptedQtySum = TempEggAcceptedQtySum * (decimal)0.058824;
                decimal EggInvoiceQtySum = TempEggInvoiceQtySum * (decimal)0.058824;


                ///Egg without Kg Conversion
                //decimal EggOrderedQtySum = TempEggOrderedQtySum ;
                //decimal EggDeliveredQtySum = TempEggDeliveredQtySum ;
                //decimal EggAcceptedQtySum = TempEggAcceptedQtySum ;
                //decimal EggInvoiceQtySum = TempEggInvoiceQtySum;

                decimal TotalOrderedQtySum = OrderedQtySum + Math.Truncate(EggOrderedQtySum * 1000m) / 1000m;
                decimal TotalDeliveredQtySum = DeliveredQtySum + Math.Truncate(EggDeliveredQtySum * 1000m) / 1000m;
                decimal TotalAcceptedQtySum = AcceptedQtySum + Math.Truncate(EggAcceptedQtySum * 1000m) / 1000m;
                decimal TotalInvoiceQtySum = InvoiceQtySum + EggInvoiceQtySum;

                //Sum of NPA Items
                decimal TotalNPAOrderedQtySum = NPAOrderedQtySum + (NPAEggOrderedQtySum * (decimal)0.058824);
                decimal TotalNPAInvoicetySum = NPAInvoiceQtySum + (NPAEggInvoiceQtySum * (decimal)0.058824);


                //Replacement
                IList<SubReplacementView> DeliveryList = (from items in SubReplaceList.First().Value
                                                          select items).OrderBy(i => i.UNCode).Distinct().ToList();


                #region DeliveryList Change for Partial Orders items has OR,AR and AS

                var Uncode = (from items in SubReplaceList.First().Value
                              where items.DiscrepancyCode == "OR"
                              select items.UNCode).Distinct().ToArray();

                if (Uncode.Count() > 0)
                {

                    foreach (var item in Uncode)
                    {
                        decimal[] ApCode = (from items in ItemMasterList.First().Value
                                            where items.UNCode == item
                                            select items.APLCode).Distinct().ToArray();

                        var Listcode = (from items in DeliveryList
                                        where items.UNCode == item
                                        select items.DiscrepancyCode).Distinct().ToArray();

                        //decimal OrderQty = DeliveryList.Where(p => p.UNCode == item).Sum(x => x.OrderedQty);
                        decimal OrderQty = DeliverySingleList.Where(p => p.UNCode == item).Sum(x => x.OrderQty);
                        decimal DelQty = DeliveryList.Where(p => p.UNCode == item).Sum(x => x.DeliveredQty);
                        decimal InvQty = DeliveryList.Where(p => p.UNCode == item).Sum(x => x.InvoiceQty);

                        decimal Difference = OrderQty - DelQty;  //To check whether Actual packsize is greater than Delivery diffrence
                        decimal APLweight = 0;
                        string discCode = "";
                        decimal Percentage = 0;

                        //Calulate Percentage for getting Apl weight
                        if (OrderQty == 0 || InvQty == 0)
                            Percentage = 0;
                        else
                        {
                            Percentage = (InvQty / OrderQty) * 100;
                        }

                        //Check NPA 01/02 by calculate Actual pack size value greater than the difference of deliveryQty
                        if (Percentage < 98)
                        {
                            if (ApCode[0] > 0)
                            {
                                if (ApCode[0] > Difference) { discCode = "NPA 01/02"; }
                                else { discCode = ""; }
                            }
                            else { discCode = ""; }
                        }
                        else { discCode = ""; }

                        //Calulation Apl weight
                        if (discCode == "NPA 01/02")
                        {

                            APLweight = (decimal)98.00;

                        }
                        else
                        {
                            if (OrderQty == 0 || InvQty == 0)
                                APLweight = 0;
                            else
                            {
                                APLweight = (InvQty / OrderQty) * 100;
                            }
                        }

                        foreach (var code in Listcode)
                        {

                            //DeliveryList.Where(w => w.UNCode == item && w.DiscrepancyCode == code) = DeliveryList.Where(w => w.UNCode == item && w.DiscrepancyCode == code).Select(s => { s.APLWeight = APLweight; return s; });
                            foreach (var List in DeliveryList.Where(w => w.UNCode == item && w.DiscrepancyCode == code))
                            {
                                if (List.DiscrepancyCode == "OR")
                                {
                                    List.APLWeight = APLweight;
                                }
                                else if (List.DiscrepancyCode == "AR")
                                {
                                    List.APLWeight = 0;
                                    List.OrderedQty = (decimal)0.00;
                                }
                                else if (List.DiscrepancyCode == "UNAS")//For UNSubstitution
                                {
                                    List.APLWeight = 0;
                                    List.OrderedQty = (decimal)0.00;
                                }
                                else
                                {
                                    List.APLWeight = 0;
                                    List.OrderedQty = (decimal)0.00;

                                }
                            }

                        }
                    }
                }
                #endregion

                #region DeliveryList Change for Existing items has AS And AR

                var UncodeList2 = (from items in DeliveryList
                                   where items.SubstituteItemCode != 0 && (items.DiscCode == "SP" || items.DiscCode == "RP" || items.DiscCode == "UNAS")
                                   select new { items.UNCode, items.SubstituteItemCode }).Distinct().ToArray();

                List<long> tempList = new List<long>();
                foreach (var item in UncodeList2)
                {
                    var tempcount = (from items in DeliveryList
                                     where items.SubstituteItemCode == item.SubstituteItemCode && items.UNCode == item.UNCode
                                     select new { items.UNCode, items.SubstituteItemCode }).Count();
                    if (tempcount > 1)
                    {
                        tempList.Add(item.UNCode);
                    }
                }

                if (tempList.Count() > 0)
                {
                    foreach (var item in tempList)
                    {
                        decimal[] ApCode = (from items in ItemMasterList.First().Value
                                            where items.UNCode == item
                                            select items.APLCode).Distinct().ToArray();

                        var Listcode = (from items in DeliveryList
                                        where items.UNCode == item
                                        select items.DiscrepancyCode).Distinct().ToArray();

                        decimal OrderQty = DeliverySingleList.Where(p => p.UNCode == item).Sum(x => x.OrderQty);
                        //decimal OrderQty = DeliveryList.Where(p => p.UNCode == item).Sum(x => x.OrderedQty);
                        decimal DelQty = DeliveryList.Where(p => p.UNCode == item).Sum(x => x.DeliveredQty);
                        decimal InvQty = DeliveryList.Where(p => p.UNCode == item).Sum(x => x.InvoiceQty);

                        decimal Difference = OrderQty - DelQty;  //To check whether Actual packsize is greater than Delivery diffrence
                        decimal APLweight = 0;
                        string discCode = "";
                        decimal Percentage = 0;
                        decimal TempINVQty = 0;

                        //Calulate Percentage for getting Apl weight
                        if (OrderQty == 0 || InvQty == 0)
                            Percentage = 0;
                        else
                        {
                            Percentage = (InvQty / OrderQty) * 100;
                        }

                        //Check NPA 01/02 by calculate Actual pack size value greater than the difference of deliveryQty
                        if (Percentage < 98)
                        {
                            if (ApCode[0] > 0)
                            {
                                if (ApCode[0] > Difference) { discCode = ""; }
                                else { discCode = "NPA 01/02"; }
                            }
                            else { discCode = ""; }
                        }
                        else { discCode = ""; }

                        //Calulation Apl weight
                        if ((OrderQty * (decimal)1.02) < DelQty)
                        {
                            APLweight = (decimal)102.00;
                        }
                        else if (discCode == "NPA 01/02")
                        {

                            // changed by kingston on 9.5.2016 the partial deliveries should not have "NPA 01/02"
                            //APLweight = (decimal)98.00;
                            APLweight = (InvQty / OrderQty) * 100;


                        }
                        else
                        {
                            if (OrderQty == 0 || InvQty == 0)
                                APLweight = 0;
                            else
                            {
                                APLweight = (InvQty / OrderQty) * 100;
                            }
                        }

                        foreach (var code in Listcode)
                        {

                            //DeliveryList.Where(w => w.UNCode == item && w.DiscrepancyCode == code) = DeliveryList.Where(w => w.UNCode == item && w.DiscrepancyCode == code).Select(s => { s.APLWeight = APLweight; return s; });
                            bool flag = true;
                            foreach (var List in DeliveryList.Where(w => w.UNCode == item && w.DiscrepancyCode == code))
                            {

                                if (List.DiscrepancyCode == "AR" && flag == true)
                                {
                                    List.APLWeight = APLweight;
                                    flag = false;
                                }
                                else if (List.DiscrepancyCode == "AS" && flag == true)
                                {
                                    List.APLWeight = APLweight;
                                    flag = false;
                                }
                                else if (List.DiscrepancyCode == "UNAS" && flag == true)//For UNSubstitution
                                {
                                    List.APLWeight = APLweight;
                                    flag = false;
                                }
                                else
                                {
                                    List.OrderedQty = (decimal)0.00;
                                    List.APLWeight = 0;
                                }
                            }

                        }
                    }
                }

                #endregion

                //Substitution

                IList<SubReplacementView> SDeliveryList = (from items in DeliveryList
                                                           where items.DiscCode == "SP"
                                                           select items).OrderBy(i => i.UNCode).Distinct().ToList();
                //Replacement
                IList<SubReplacementView> RDeliveryList = (from items in DeliveryList
                                                           where items.DiscCode == "RP"
                                                           select items).OrderBy(i => i.UNCode).Distinct().ToList();
                long SubAPlCount = 0;
                #region UNSubstitution
                //UNSubstitution
                IList<SubReplacementView> UNDeliveryList = (from items in DeliveryList
                                                            where items.DiscCode == "NPA/04"
                                                            select items).OrderBy(i => i.UNCode).Distinct().ToList();


                //UNSubstitution Calculation
                decimal UNSDeliveryQuantity = (decimal)0.00;
                decimal UNSOrderedQuantity = (decimal)0.00;
                decimal UNSAcceptedQuantity = (decimal)0.00;
                decimal UNSAcceptedamt = (decimal)0.00;

                long UNSubstituteCount = 0;
                var UnSubcodeList = (from items in UNDeliveryList
                                     select items.LineId).Distinct().ToArray();
                UNSubstituteCount = UnSubcodeList.Count();
                foreach (var item in UNDeliveryList)
                {
                    //if (item.DiscCode == "NPA/04" && item.APLWeight < (decimal)98.00)
                    //{
                    //    item.APLWeight = (decimal)98.00;
                    //}
                    if (item.APLWeight >= (decimal)98.00)
                    {
                        SubAPlCount = SubAPlCount + 1;
                    }
                    if (item.UNCode != 0 && item.SubstituteItemCode != 0)
                    {
                        //SubstituteCount = SubstituteCount + 1;
                    }
                    #region Transportation cost calculation
                    if (item.UNCode == 1129)
                    {
                        TotalTransportationCost = TotalTransportationCost + Math.Round(((item.InvoiceQty * (decimal)0.058824) * TrnsprtInv.RatePerKg), 2, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        TotalTransportationCost = TotalTransportationCost + Math.Round((item.InvoiceQty * TrnsprtInv.RatePerKg), 2, MidpointRounding.AwayFromZero);
                    }
                    #endregion
                }
                if (UNDeliveryList != null && UNDeliveryList.Count > 0)
                {
                    UNSDeliveryQuantity = UNDeliveryList.Sum(item => item.DeliveredQty);
                    UNSOrderedQuantity = UNDeliveryList.Sum(item => item.OrderedQty);
                    UNSAcceptedQuantity = UNDeliveryList.Sum(item => item.InvoiceQty);
                    UNSAcceptedamt = UNDeliveryList.Sum(item => item.AcceptedAmt);
                }
                #endregion
                #region Substitution
                //Substitution Calculation
                decimal SDeliveryQuantity = (decimal)0.00;
                decimal SOrderedQuantity = (decimal)0.00;
                decimal SAcceptedQuantity = (decimal)0.00;
                decimal SAcceptedamt = (decimal)0.00;
                //long SubAPlCount = 0;
                long SubstituteCount = 0;
                var UncodeList = (from items in SDeliveryList
                                  select items.LineId).Distinct().ToArray();
                SubstituteCount = UncodeList.Count();
                foreach (var item in SDeliveryList)
                {
                    if (item.APLWeight >= (decimal)98.00)
                    {
                        SubAPlCount = SubAPlCount + 1;
                    }
                    if (item.UNCode != 0 && item.SubstituteItemCode != 0)
                    {
                        //SubstituteCount = SubstituteCount + 1;
                    }
                    #region Transportation cost calculation
                    if (item.UNCode == 1129)
                    {
                        TotalTransportationCost = TotalTransportationCost + Math.Round(((item.InvoiceQty * (decimal)0.058824) * TrnsprtInv.RatePerKg), 2, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        TotalTransportationCost = TotalTransportationCost + Math.Round((item.InvoiceQty * TrnsprtInv.RatePerKg), 2, MidpointRounding.AwayFromZero);
                    }
                    #endregion
                }
                if (SDeliveryList != null && SDeliveryList.Count > 0)
                {
                    SDeliveryQuantity = SDeliveryList.Sum(item => item.DeliveredQty);
                    SOrderedQuantity = SDeliveryList.Sum(item => item.OrderedQty);
                    SAcceptedQuantity = SDeliveryList.Sum(item => item.InvoiceQty);
                    SAcceptedamt = SDeliveryList.Sum(item => item.AcceptedAmt);
                }
                #endregion
                #region Replacement
                //Replacement Calculation
                decimal RDeliveryQuantity = (decimal)0.00;
                decimal ROrderedQuantity = (decimal)0.00;
                decimal RAcceptedQuantity = (decimal)0.00;
                decimal RAcceptedamt = (decimal)0.00;
                foreach (var item in RDeliveryList)
                {
                    if (item.APLWeight >= (decimal)98.00)
                    {
                        SubAPlCount = SubAPlCount + 1;
                    }
                    #region Transportation cost calculation
                    if (item.UNCode == 1129)
                    {
                        TotalTransportationCost = TotalTransportationCost + Math.Round(((item.InvoiceQty * (decimal)0.058824) * TrnsprtInv.RatePerKg), 2, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        TotalTransportationCost = TotalTransportationCost + Math.Round((item.InvoiceQty * TrnsprtInv.RatePerKg), 2, MidpointRounding.AwayFromZero);
                    }
                    #endregion
                }
                if (RDeliveryList != null && RDeliveryList.Count > 0)
                {
                    RDeliveryQuantity = RDeliveryList.Sum(item => item.DeliveredQty);
                    ROrderedQuantity = RDeliveryList.Sum(item => item.OrderedQty);
                    RAcceptedQuantity = RDeliveryList.Sum(item => item.InvoiceQty);
                    RAcceptedamt = RDeliveryList.Sum(item => item.AcceptedAmt);
                }
                #endregion
                //decimal NumberOfSubstitutions = 0;
                long NumberOfDaysDelay = Convert.ToInt64((ApprovedDeliverydate - (DateTime)Ord.ExpectedDeliveryDate).TotalDays);
                long TotalLineItemsOrdered = SingleInvoiceList.Count;
                CMRMaster cmr = MS.GetCMRMasterBySectorCode(Ord.Sector);
                decimal AuthorizedCMR = CheckAuthorizedCMRisValid(cmr);
                decimal TotalManDays = 7 * Ord.Troops;
                decimal OrderCMR = (OrdervalueSum / TotalManDays);//Modified on 05/05
                decimal AcceptedCMR = ((NetAmountSum + SAcceptedamt + RAcceptedamt + UNSAcceptedamt) / TotalManDays);//Modified on 05/05
                decimal ActualCMR = Convert.ToDecimal(string.Format("{0:0.000}", OrderCMR));//Created on 05/05
                decimal CMRUtilized = Convert.ToDecimal(string.Format("{0:0.000}", (AcceptedCMR / OrderCMR) * 100));



                criteria.Clear();
                criteria.Add("ContingentControlNo", Ord.Name);
                criteria.Add("LocationCode", Ord.Location);
                criteria.Add("SectorCode", Ord.Sector);
                if (!string.IsNullOrEmpty(Ord.ContingentType) && Ord.ContingentType == "FPU")
                    criteria.Add("TypeofUnit", "FP");
                else
                    criteria.Add("TypeofUnit", "ML");
                Dictionary<long, IList<ContingentMaster>> ContingentList = MS.GetContigentListWithPagingAndCriteria(0, 9999, "UNCode", string.Empty, criteria);
                #region Performance calculation new
                decimal AmtAccept = NetAmountSum + SAcceptedamt + RAcceptedamt + UNSAcceptedamt;
                decimal Weeklywise = 0;
                Weeklywise = Math.Round(Convert.ToDecimal(0.35 / 100) * AmtAccept, 2);
                decimal confirmityCMR = 0;
                if (AcceptedCMR > AuthorizedCMR)
                {
                    confirmityCMR = (AcceptedCMR - AuthorizedCMR) * Math.Round(Ord.Troops) * 7;
                }
                decimal NetRationAmt = AmtAccept - Weeklywise - confirmityCMR;
                #endregion

                #region APL Performance calculation
                PenaltyCaculation pcl = new PenaltyCaculation();
                pcl.TotalDays = Convert.ToInt64((ApprovedDeliverydate - (DateTime)Ord.ExpectedDeliveryDate).TotalDays);
                pcl.TotalLineitem = Convert.ToInt64(Ord.LineItemsOrdered);
                pcl.TotalLineitem98 = Math.Round(TempAboveAplWeight, 3) + SubAPlCount;
                //pcl.InvoiceQty = Math.Round((TotalInvoiceQtySum + SAcceptedQuantity + RAcceptedQuantity), 3);
                pcl.InvoiceQty = Math.Round((TotalInvoiceQtySum + SAcceptedQuantity + RAcceptedQuantity + UNSAcceptedQuantity), 3);
                pcl.OrderedQty = Math.Round(TotalOrderedQtySum, 3);

                pcl.InvoiceQty = Math.Round((TotalInvoiceQtySum + SAcceptedQuantity + RAcceptedQuantity - TotalNPAInvoicetySum), 3);
                pcl.OrderedQty = Math.Round(TotalOrderedQtySum - TotalNPAOrderedQtySum, 3);

                pcl.SubstituteCount = SubstituteCount;
                //pcl.OrdervalueSum = Math.Round(OrdervalueSum, 3);
                pcl.OrdervalueSum = Math.Round(NetRationAmt, 3);

                PenaltyCaculation Pc = GetPenaltyCalculationValues(pcl);
                #endregion

                //PerformanceCalculateView Pc = IS.GetPerformanceCalculateById(OrderId);

                int RefNo = GetControlidInSeries(Ord);
                //int no = GetSerialNoforPeriod(Ord.Period, Ord.PeriodYear);
                string SectorNo = "";
                if (Ord.Sector == "FS")
                    SectorNo = "-090-";
                else if (Ord.Sector == "NS")
                    SectorNo = "-091-";
                else
                    SectorNo = "-092-";

                #region Deliverynotes names
                string a = string.Join(",", DeliverySingleList.Select(p => p.DeliveryNote.ToString()));
                string b = string.Join(",", SDeliveryList.Select(p => p.DeliveryNoteName.ToString()));
                string c = string.Join(",", RDeliveryList.Select(p => p.DeliveryNoteName.ToString()));
                string d = string.Join(",", UNDeliveryList.Select(p => p.DeliveryNoteName.ToString()));////For UNSubstitution
                string result = a + "," + b + "," + c + "," + d;
                string[] myArray = result.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string[] ResultAr = myArray.Distinct().ToArray();
                string DeliveryNotes = "";

                if (ResultAr.Count() > 1)
                    DeliveryNotes = String.Join(",", ResultAr);
                else
                    DeliveryNotes = ResultAr[0];
                #endregion

                SingleInvoice si = new SingleInvoice();
                InvoiceNumberMaster InvNumber = OS.GetInvoiceNumberByOrderId(OrderId);
                if (InvNumber != null && InvNumber.InvoiceMasterId > 0)
                    si.Reference = InvNumber.InvoiceNumber;
                else
                    si.Reference = "N/A"; // Need to discuss.

                //si.Reference = "GCC-" + Ord.Sector + SectorNo + Ord.ContingentType + "-" + no + "/" + RefNo;
                si.DeliveryPoint = ContingentList.FirstOrDefault().Key > 0 ? ContingentList.FirstOrDefault().Value[0].DeliveryPoint : "";
                si.ContingentID = ContingentList.FirstOrDefault().Key > 0 ? ContingentList.FirstOrDefault().Value[0].ContingentID : 0;
                si.UNID = Ord.ControlId;
                si.Strength = Math.Round(Ord.Troops);
                si.ManDays = Math.Round(TotalManDays);
                si.ApplicableCMR = Math.Round(OrderCMR, 2);
                si.AuthorizedCMR = AuthorizedCMR;
                si.Period = Ord.Period + "/" + Ord.PeriodYear;
                si.DOS = 7;
                si.DeliveryWeek = Ord.Week.ToString();
                //si.DeliveryMode = ContingentList.FirstOrDefault().Key > 0 ? ContingentList.FirstOrDefault().Value[0].DeliveryMode : "";
                si.DeliveryMode = ContingentList.FirstOrDefault().Key > 0 ? ContingentList.FirstOrDefault().Value[0].DeliveryModeDescription : "";
                si.ApprovedDelivery = ConvertDateTimeToDate(Ord.ExpectedDeliveryDate.ToString(), "en-GB");
                si.ActualDeliveryDate = ConvertDateTimeToDate(ApprovedDeliverydate.ToString(), "en-GB");
                //deliveryWithloutOrders = deliveryWithoutOrders.First().Value.ToList(),
                si.DeliveryDetails = DeliverySingleList;
                //Substitution
                #region Substitution Table
                si.SDeliveryQuantity = SDeliveryQuantity + UNSDeliveryQuantity;
                si.SOrderedQuantity = SOrderedQuantity + UNSOrderedQuantity;
                si.SAcceptedQuantity = SAcceptedQuantity + UNSAcceptedQuantity;
                si.SAcceptedamt = Math.Round((SAcceptedamt + UNSAcceptedamt), 3);
                si.SDeliveryList = SDeliveryList.Concat(UNDeliveryList).ToList();
                #endregion
                //Replacement
                #region Replacement Table
                si.RDeliveryQuantity = RDeliveryQuantity;
                si.ROrderedQuantity = ROrderedQuantity;
                si.RAcceptedQuantity = RAcceptedQuantity;
                si.RAcceptedamt = Math.Round(RAcceptedamt, 3);
                si.RDeliveryList = RDeliveryList;
                #endregion
                si.TotalDays = Convert.ToInt64((ApprovedDeliverydate - (DateTime)Ord.ExpectedDeliveryDate).TotalDays);

                si.OrderedQtySum = Math.Round(OrderedQtySum, 3);

                si.DeliveredQtySum = Math.Round(DeliveredQtySum, 3);
                si.AcceptedQtySum = Math.Round(AcceptedQtySum, 3);
                si.InvoiceQtySum = Math.Round(InvoiceQtySum, 3);

                si.NetAmountSum = Math.Round(NetAmountSum, 3);

                si.OrdervalueSum = Math.Round(OrdervalueSum, 3);
                //si.EggOrderedQtySum = Math.Round(EggOrderedQtySum, 3);
                //si.EggDeliveredQtySum = Math.Round(EggDeliveredQtySum, 3);
                //si.EggAcceptedQtySum = Math.Round(EggAcceptedQtySum, 3);
                //si.EggInvoiceQtySum = Math.Round(EggInvoiceQtySum, 3);
                //si.TotalOrderedQtySum = Math.Round(TotalOrderedQtySum, 3);
                si.EggOrderedQtySum = Math.Truncate(EggOrderedQtySum * 1000m) / 1000m;
                si.EggDeliveredQtySum = Math.Truncate(EggDeliveredQtySum * 1000m) / 1000m;
                si.EggAcceptedQtySum = Math.Truncate(EggAcceptedQtySum * 1000m) / 1000m;
                si.TotalOrderedQtySum = Math.Truncate(TotalOrderedQtySum * 1000m) / 1000m;

                si.EggInvoiceQtySum = Math.Round(EggInvoiceQtySum, 3);

                si.TotalDeliveredQtySum = Math.Round(TotalDeliveredQtySum + SDeliveryQuantity + RDeliveryQuantity + UNSDeliveryQuantity, 3);
                //si.TotalAcceptedQtySum = Math.Round(TotalAcceptedQtySum, 3);
                si.TotalAcceptedQtySum = Math.Round(TotalInvoiceQtySum + SAcceptedQuantity + RAcceptedQuantity + UNSAcceptedQuantity, 3);
                si.TotalInvoiceQtySum = Math.Round((TotalInvoiceQtySum), 3);

                si.AboveCount = Math.Round(TempAboveAplWeight, 3);
                si.BelowCount = Math.Round(TempBelowAplWeight, 3);

                si.CountPercent = Math.Round((TempAboveAplWeight / (TempAboveAplWeight + TempBelowAplWeight)) * 100, 3);
                #region APL Performance Table
                si.DeliveryPerformance = Math.Round(Pc.DeliveryPerformance, 0);
                si.LineItemPerformance = Math.Round(Pc.LineItemPerformance, 2);
                si.OrderWightPerformance = Math.Round(Pc.OrderWeightPerformance, 2);
                si.SubtitutionPerformance = Math.Round(Pc.ComplaintsPerformance, 2);
                si.DeliveryDeduction = Math.Round(Pc.DeliveryDeduction, 2);
                si.LineItemDeduction = Math.Round(Pc.LineItemDeduction, 2);
                si.OrderWightDeduction = Math.Round(Pc.OrderWeightDeduction, 2);
                si.SubtitutionDeduction = Math.Round(Pc.ComplaintsDeduction, 2);
                #endregion
                si.TotalLineitem98 = Math.Round(TempAboveAplWeight, 3) + SubAPlCount;
                si.SubstituteCount = SubstituteCount;
                si.AmountSubstituted = SDeliveryList.Sum(item => item.AcceptedAmt);
                si.OrderCMR = OrderCMR;
                si.AcceptedCMR = AcceptedCMR;
                si.CMRUtilized = CMRUtilized;
                si.Deliverynotes = DeliveryNotes;
                si.ApprovedDeliveryDate = ApprovedDeliverydate;
                si.UNItemCount = UNSubstituteCount;//For UNSubstitution
                si.AmountAccepted = AmtAccept;
                si.Weeklyinvoicediscount = Weeklywise;
                si.NetRationAmount = NetRationAmt;
                si.confirmityCMR = confirmityCMR;

                si.RatePerKg = TrnsprtInv != null ? TrnsprtInv.RatePerKg : 0;
                //si.AcceptedTransportCost = TrnsprtInv != null ? Math.Round(TrnsprtInv.RatePerKg * si.TotalAcceptedQtySum, 2) : 0;//
                si.AcceptedTransportCost = TotalTransportationCost;

                return si;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }

        public ActionResult ExcelGeneration()
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
                    throw ex;
                }
            }
        }

        public ActionResult ExcelSingleInvoice()
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
                    throw ex;
                }
            }
        }

        public ActionResult ExcelConsolidateInvoice()
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
                    throw ex;
                }
            }
        }

        public ActionResult ExcelWorkBookInvoice()
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
                    throw ex;
                }
            }
        }

        public JsonResult GenerateExcel(string searchItems, string Ids)
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
                    var Orderids = (from items in DocumentItems.First().Value
                                    select items.OrderId).OrderBy(i => i).Distinct().ToArray();
                    foreach (var item in Orderids)
                    {
                        //InvoiceSingleExcel(item, false);
                        CallInvoiceExcelParallel(item);
                        Count = Count + 1;
                    }
                }
                else //To Generate all the documents from Searchitems
                {
                    if (searchItems != null && searchItems != "")
                    {
                        var Items = searchItems.ToString().Split(',');
                        if (!string.IsNullOrWhiteSpace(Items[0])) { criteria.Add("Sector", Items[0]); }
                        if (!string.IsNullOrWhiteSpace(Items[1])) { criteria.Add("Name", Items[1]); }
                        if (!string.IsNullOrWhiteSpace(Items[2])) { criteria.Add("ContingentType", Items[2]); }
                        if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Period", Items[3]); }
                        if (!string.IsNullOrWhiteSpace(Items[4])) { criteria.Add("PeriodYear", Items[4]); }
                        if (Items.Length > 5)
                            if (!string.IsNullOrWhiteSpace(Items[5])) { criteria.Add("Week", Convert.ToInt64(Items[5])); }
                        var TrimItems = searchItems.Replace(",", "");
                        if (string.IsNullOrWhiteSpace(TrimItems))
                            return null;
                    }
                    Dictionary<long, IList<InvoiceManagementView>> invoiceItems = IS.GetInvoiceListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                    if (invoiceItems != null && invoiceItems.FirstOrDefault().Key > 0)
                    {
                        var InvoiceOrderList = (from items in invoiceItems.First().Value
                                                select items.OrderId).OrderBy(i => i).Distinct().ToArray();
                        foreach (var item in InvoiceOrderList)
                        {
                            //InvoiceSingleExcel(item, false);
                            CallInvoiceExcelParallel(item);
                            Count = Count + 1;
                        }
                    }
                }
                return Json(Count, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void TransportImportToExcelSheet(DataSet Workbookset, List<TransportConsol> TConsolList, List<TransportSingle> TSingleList)
        {
            string userId = base.ValidateUser();
            try
            {

                using (ExcelPackage pck = new ExcelPackage())
                {

                    int TableCount = Workbookset.Tables.Count;
                    int x = 0; // Main Int is zero 
                    int c = 0; // Consolidate Int is zero 
                    int s = 0; // Single Int is zero 
                    System.Drawing.Image logo = System.Drawing.Image.FromFile("D:\\HeaderImage/main_logo.jpg");
                    for (int i = 0; i < TableCount; i++)
                    {

                        if (Workbookset.Tables[i].TableName.ToString() == TConsolList[c].Title)
                        {
                            #region Consolidate Transport Invoice

                            ExcelWorksheet ws = pck.Workbook.Worksheets.Add(Workbookset.Tables[i].TableName.ToString());
                            ws.View.ZoomScale = 80;
                            ws.View.ShowGridLines = false;
                            ws.Column(1).Width = 1.16;
                            ws.Column(2).Width = 12.85;
                            ws.Column(3).Width = 28.58;
                            ws.Column(4).Width = 39.42;
                            ws.Column(5).Width = 17.86;
                            ws.Column(6).Width = 17.86;
                            ws.Row(6).Height = 29.25;
                            ws.Row(21).Height = 18.75;
                            ws.Row(52).Height = ws.Row(53).Height = ws.Row(54).Height = ws.Row(55).Height = 19.50;
                            ws.Row(56).Height = 21.75;
                            ws.Row(54).Height = 40;
                            ws.Row(64).Height = 40;
                            //Adding image 
                            int img = 0;
                            ws.Row(img * 5).Height = 39.00D;
                            var picture = ws.Drawings.AddPicture(img.ToString(), logo);
                            picture.SetPosition(1, 0, 1, 0);

                            ws.Cells["B6:F6,B53:F53"].Merge = true;
                            ws.Cells["B6:F6"].Value = "INVOICE";
                            ws.Cells["B6:F6,B53:F53"].Style.WrapText = true;
                            ws.Cells["B6:F6"].Style.Font.Bold = true;
                            ws.Cells["B6:F6"].Style.Font.Name = "Arial";
                            ws.Cells["B6:F6"].Style.Font.Size = 20;
                            ws.Cells["B6:F6"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B6:F6"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B6:F6"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B6:F6"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B6:F6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            //Boarders for Header Part & styles
                            ws.Cells["B7:F70"].Style.Font.Size = 10;
                            ws.Cells["B7:F70"].Style.Font.Name = "Arial";
                            ws.Cells["B7:F21,C23,C43,B52:F52,B54:F54,B56,B57"].Style.Font.Bold = true;
                            ws.Cells["B7:B63"].Style.Border.Left.Style = ws.Cells["F7:F63,D52:D55"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["D7:D14,E7:E14,B21:B50,D21:D51,E21:E55,B51,D51"].Style.Border.Right.Style = ExcelBorderStyle.Thin; //Thin Line Center
                            ws.Cells["B14:F14,B20:F20,B21:F21,B51:F51,B55:F55,B56:F56,B63:F63"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium; // Bottom Line  Medium
                            ws.Cells["E52:F52,E53:F53"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin; // Bottom Line  Thin
                            ws.Cells["E54:F54"].Style.Border.Bottom.Style = ExcelBorderStyle.Double; // Bottom Line  Thin
                            ws.Cells["B7:D7,C21:D21,C26:D26,C27:D27,C28:D28,B50:E50"].Merge = true; // Merge Cells
                            ws.Cells["B52:D52,B54:D54,B55:D55,B56:D56"].Merge = true; // Merge Cells
                            ws.Cells["B21,C21:D21,E21,F21,C26:D26,C27:D27,C28:D28,B26:B28"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; //Center Alignments for Cells
                            ws.Cells["B21,C21:D21,E21,F21,C26:D26,C27:D27,C28:D28,B26:B28"].Style.VerticalAlignment = ExcelVerticalAlignment.Center; //Center Alignments for Cells
                            ws.Cells["B52:D52,B53:D53,B54:D54,B55:D55,B56:D56,E52,E54,E64"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left; //Left Alignments for Cells
                            ws.Cells["B52:D52,B53:D53,B54:D54,B55:D55,B56:D56,E52,E54"].Style.VerticalAlignment = ExcelVerticalAlignment.Center; //Center Alignments for Cells
                            ws.Cells["F52,F54"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right; //Right Alignments for Cells
                            ws.Cells["F52,F54"].Style.VerticalAlignment = ExcelVerticalAlignment.Center; //Center Alignments for Cells

                            ws.Cells["F26:F28"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            ws.Cells["B7"].Value = " To:";
                            ws.Cells["E7"].Value = "";
                            ws.Cells["F7"].Value = "";

                            ws.Cells["C8"].Value = "Chief Rations Unit";
                            ws.Cells["E8"].Value = "Invoice #:";
                            ws.Cells["F8"].Value = TConsolList[c].InvoiceNo;                           //InvoiceNo

                            ws.Cells["C9"].Value = "African Union - United Nations Hybrid Operation in Darfur";
                            ws.Cells["E9"].Value = "Contract #:";
                            ws.Cells["F9"].Value = TConsolList[c].Contract;                              //Contract

                            ws.Cells["C10"].Value = "El Fasher, Darfur";
                            ws.Cells["E10"].Value = "Invoice Date:";
                            ws.Cells["F10"].Value = TConsolList[c].InvoiceDate;                            //InvoiceDate


                            ws.Cells["C11"].Value = "";
                            ws.Cells["E11"].Value = "Payment Terms:";
                            ws.Cells["F11"].Value = TConsolList[c].PayTerms;                                 //PayTerms

                            ws.Cells["B12"].Value = "Cc:";
                            ws.Cells["C12"].Value = "Mission Designated Official";
                            ws.Cells["E12"].Value = "PO:";
                            ws.Cells["F12"].Value = TConsolList[c].PO;                   //PO

                            //ws.Cells["B13"].Value = "Attention:";
                            ws.Cells["C13"].Value = "UNAMID El Fasher";
                            ws.Cells["E13"].Value = "";
                            ws.Cells["F13"].Value = "";

                            ws.Cells["C15"].Value = "Period:";
                            ws.Cells["D15"].Value = TConsolList[c].Period;                           //Period
                            ws.Cells["C16"].Value = "UN Identification Number:";
                            ws.Cells["D16"].Value = TConsolList[c].UnINo;                                  //UnINo
                            ws.Cells["C17"].Value = "Sector:";
                            ws.Cells["D17"].Value = TConsolList[c].Sector;                 //Sector
                            ws.Cells["C18"].Value = "Week:";
                            ws.Cells["D18"].Value = "WK0" + TConsolList[c].Week;
                            ws.Cells["D18"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            //Hearders
                            ws.Cells["B21"].Value = "Qty";
                            ws.Cells["C21:D21"].Value = "Description";
                            ws.Cells["E21"].Value = "Amount in USD";
                            ws.Cells["F21"].Value = "Amount in USD";

                            ws.Cells["C24"].Value = "OUTBOUND TRANSPORTATION FEE";

                            int k = 26;
                            decimal SumofTranportvalues = 0;
                            for (int P = 0; P < TConsolList[c].TransportFeeList.Count(); P++)
                            {

                                ws.Cells["B" + k + ""].Value = TConsolList[c].TransportFeeList[P].AcceptedQty;
                                ws.Cells["B" + k + ""].Style.Numberformat.Format = "#,##0.00"; //Accepted Qty
                                ws.Cells["C" + k + ":D" + k + ""].Value = TConsolList[c].TransportFeeList[P].Description;                     //Description
                                ws.Cells["E" + k + ""].Value = string.Format("{0:C}", (Math.Round(TConsolList[c].TransportFeeList[P].TotalAmt, 2)));                                   //InvoiceValue
                                SumofTranportvalues = SumofTranportvalues + Math.Round(TConsolList[c].TransportFeeList[P].TotalAmt, 2);
                                k = k + 1;
                            }
                            ws.Cells["F24"].Value = "$     " + Math.Round(SumofTranportvalues, 2);
                            ws.Cells["C43"].Value = "DISCOUNTS";
                            ws.Cells["C44"].Value = " Weekly Billing Discount @ 0.35%";
                            ws.Cells["F44"].Value = "-" + TConsolList[c].WeeklyDiscount;

                            ws.Cells["E44"].Value = "       -";
                            ws.Cells["E45"].Value = "       -";
                            ws.Cells["F43"].Value = "       -";

                            //ws.Cells["B50:E50"].Value = "* Disount is applicable only if invoice is paid within 30 days from the date of receipt of invoice.";

                            //ws.Cells["B52:D52"].Value = "Gulf Catering Company for General Trade and Contracting WLL";
                            ws.Cells["E52"].Value = " NET TOTAL";


                            //ws.Cells["B53:D53"].Value = "in case of incorrect invoice or if changes are required for this invoice please contact:";

                            ws.Cells["B54:D54"].Value = "A Prompt Payment Discount of 0.2% of the NET TOTAL applies if payment is made in less than 30 days.";
                            ws.Cells["B54:D54"].Style.WrapText = true;
                            // ws.Cells["E54"].Value = " Grand Total";
                            ws.Cells["F52"].Value = TConsolList[c].GrandTotal;

                            //ws.Cells["E55"].Value = "$     " + Math.Round(Convert.ToDecimal(TConsolList[c].GrandTotal.Replace("$", "")) * (decimal)0.002, 2);
                            ws.Cells["E55"].Value = "$     " + Math.Round(Convert.ToDecimal(TConsolList[c].GrandTotal.Replace("Rs.", "$").Replace("$", "")) * (decimal)0.002, 2);

                            ws.Cells["B55:D55"].Value = "IF payment is made in less than 30 days, the PPD is: ";//Rs.


                            ws.Cells["B53:F53"].Value = TConsolList[c].Usd_words;
                            ws.Cells["B53"].Style.Font.Color.SetColor(Color.Red);

                            ws.Cells["B57"].Value = "Bank Details";
                            ws.Cells["B57"].Style.Font.UnderLine = true;
                            ws.Cells["B58"].Value = "   Bank account name: Gulf Catering Company for General Trade and Contracting WLL";
                            ws.Cells["B59"].Value = "   Bank name: GULF Bank";
                            ws.Cells["B60"].Value = "   Bank account number: KW57GULB0000000000000090622676;";
                            ws.Cells["B61"].Value = "   Bank address: Qibla Area Hamad Al-Saqr Street, Kharafi Tower, First Floor, P.O. Box 1683, Safat, Kuwait City, Kuwait 1683";
                            ws.Cells["B62"].Value = "   SWIFT/ABA: GULBKWKW";



                            ws.Cells["B64"].Value = "Email:  UNAMIDAR@GCCServices.com";
                            ws.Cells["B65:F65"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                            ws.Cells["B65:F65"].Style.Border.Top.Color.SetColor(Color.Orange);
                            ws.Cells["B65"].Value = "Gulf Catering Company for General Trade and Contracting WLL  - P.O Box 4583, Safat 13046, Kuwait";
                            ws.Cells["B66"].Value = "Disclaimer:";
                            ws.Cells["B67"].Value = "In the interest of ensuring a smooth invoicing/payment process, GCC SERVICES herewith signs this GRR with the intent to officially review the Weekly Billing Discount and APL formulas.  We will submit a correction/recovery request as applicable.";
                            for (int m = 7; m < 56; m++)
                            {
                                //ws.Cells["B" + m + ":F" + m + ""].Style.Border.Top.Style = ExcelBorderStyle.Hair;
                                ws.Cells["B" + m + ":F" + m + ""].Style.Border.Top.Style = ExcelBorderStyle.Hair;
                                ws.Cells["B" + m + ":F" + m + ""].Style.Border.Top.Color.SetColor(Color.Red);
                            }

                            //Printer Settings

                            ws.PrinterSettings.TopMargin = ws.PrinterSettings.BottomMargin = 0.25M;
                            ws.PrinterSettings.LeftMargin = ws.PrinterSettings.RightMargin = 0M;
                            ws.PrinterSettings.HeaderMargin = 0M;
                            ws.PrinterSettings.FooterMargin = 0.25M;
                            ws.PrinterSettings.Orientation = eOrientation.Portrait;
                            ws.PrinterSettings.PaperSize = ePaperSize.A4;
                            ws.PrinterSettings.FitToPage = true;
                            ws.PrinterSettings.FitToWidth = 1;
                            ws.PrinterSettings.FitToHeight = 1;
                            ws.PrinterSettings.HorizontalCentered = true;
                            ws.PrinterSettings.VerticalCentered = true;

                            c = 1;
                            #endregion
                        }
                        else if (Workbookset.Tables[i].TableName.ToString() == TSingleList[s].Title)
                        {
                            #region Single Transport Invoice

                            ExcelWorksheet ws = pck.Workbook.Worksheets.Add(Workbookset.Tables[i].TableName.ToString());
                            ws.View.ZoomScale = 70;
                            ws.View.ShowGridLines = false;

                            ws.Row(13).Height = 72.00;
                            ws.Column(1).Width = 17.29;
                            ws.Column(2).Width = 58.00;
                            ws.Column(3).Width = 20.14;
                            ws.Column(4).Width = 21.14;
                            ws.Column(5).Width = 21.71;
                            ws.Column(6).Width = 16.90;
                            ws.Column(7).Width = 11.86;
                            ws.Column(8).Width = 16.29;
                            ws.Column(9).Width = 20.14;
                            ws.Column(10).Width = 20.14;


                            //Alignment, Styles, Border's
                            ws.Cells["A12:I12,A13:I13"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium; // Bottom Line  Medium
                            ws.Cells["A13,B13,C13,D13,E13,F13,G13,H13,I13,J13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; //Center Alignments for Cells
                            ws.Cells["A13,B13,C13,D13,E13,F13,G13,H13,I13,J13"].Style.VerticalAlignment = ExcelVerticalAlignment.Center; //Center Alignments for Cells
                            ws.Cells["A13,B13,C13,D13,E13,F13,G13,H13,I13,J13"].Style.WrapText = true;
                            ws.Cells["A13,B13,C13,D13,E13,F13,G13,H13"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["I13"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A13"].Style.Border.Left.Style = ExcelBorderStyle.Medium;

                            ws.Cells["A6:J250"].Style.Font.Size = 12;
                            ws.Cells["A6:JJ250"].Style.Font.Name = "Calibri";
                            ws.Cells["A6:J13"].Style.Font.Bold = true;

                            ws.Cells["A6"].Value = " Reference #";
                            ws.Cells["B6"].Value = TSingleList[s].ReferenceNo;

                            ws.Cells["A7"].Value = " Sector :";
                            ws.Cells["B7"].Value = TSingleList[s].Sector;

                            ws.Cells["A8"].Value = " Period ";
                            ws.Cells["B8"].Value = TSingleList[s].Period;

                            ws.Cells["A9"].Value = " Sub-Invoice Date";
                            ws.Cells["B9"].Value = TSingleList[s].SubInvoiceDate;

                            ws.Cells["A10"].Value = " PO:";
                            ws.Cells["B10"].Value = TSingleList[s].PO;


                            //Hearder 
                            ws.Cells["A13"].Value = "S.No";
                            ws.Cells["B13"].Value = "Contingent Name";
                            ws.Cells["C13"].Value = "From Sector Warehouse";
                            ws.Cells["D13"].Value = "Contingent Location";
                            ws.Cells["E13"].Value = "Mode of Delivery";
                            ws.Cells["F13"].Value = "Distance from Warehouse (KM)";
                            ws.Cells["G13"].Value = "Rate per Kg";
                            ws.Cells["H13"].Value = "Quantity Accepted";
                            ws.Cells["I13"].Value = "Transportation Cost";
                            ws.Cells["J13"].Value = "Location";

                            int count = TSingleList[s].TransportInvoiceList.Count() + 14;
                            int k = 0; // Sno count
                            int L = 0; // Increament for every line
                            for (int p = 14; p < count; p++)
                            {
                                ws.Cells["A" + p].Value = k + 1;
                                ws.Cells["A" + p].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["A" + p].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells["B" + p].Value = TSingleList[s].TransportInvoiceList[L].ControlId;
                                ws.Cells["B" + p].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                ws.Cells["B" + p].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells["C" + p].Value = TSingleList[s].TransportInvoiceList[L].DeliverySector;
                                ws.Cells["C" + p].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["C" + p].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells["D" + p].Value = TSingleList[s].TransportInvoiceList[L].ContingentLocation;
                                ws.Cells["D" + p].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["D" + p].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells["E" + p].Value = TSingleList[s].TransportInvoiceList[L].DeliveryMode;
                                ws.Cells["E" + p].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["E" + p].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells["F" + p].Value = TSingleList[s].TransportInvoiceList[L].Distance;
                                ws.Cells["F" + p].Style.Numberformat.Format = "#,##0.00";
                                ws.Cells["F" + p].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                ws.Cells["F" + p].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells["G" + p].Value = TSingleList[s].TransportInvoiceList[L].RatePerKg;
                                ws.Cells["G" + p].Style.Numberformat.Format = "#,##0.00";
                                ws.Cells["G" + p].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                ws.Cells["G" + p].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells["H" + p].Value = TSingleList[s].TransportInvoiceList[L].InvoiceQty;
                                ws.Cells["H" + p].Style.Numberformat.Format = "#,##0.00";
                                ws.Cells["H" + p].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                ws.Cells["H" + p].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells["I" + p].Value = string.Format("{0:C}", (Math.Round(TSingleList[s].TransportInvoiceList[L].TransportCost, 2)));
                                ws.Cells["I" + p].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                ws.Cells["I" + p].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells["J" + p].Value = TSingleList[s].TransportInvoiceList[L].Location;
                                ws.Cells["J" + p].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["J" + p].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells["I" + p].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                                ws.Cells["A" + p].Style.Border.Left.Style = ExcelBorderStyle.Medium;

                                ws.Cells["A" + p + ":H" + p + ""].Style.Border.Right.Style = ExcelBorderStyle.Hair;
                                ws.Cells["A" + p + ":I" + p + ""].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

                                ws.Row(p).Height = 21.20;
                                k = k + 1;
                                L = L + 1;
                            }

                            ws.Row((k + 14)).Height = 21.20;
                            ws.Row((k + 14 + 1)).Height = 21.20;
                            ws.Row((k + 14 + 2)).Height = 21.20;
                            ws.Row((k + 14 + 3)).Height = 21.20;
                            ws.Row((k + 14 + 4)).Height = 21.70;
                            ws.Cells["I" + (k + 14) + ":I" + (k + 4 + 14)].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A" + (k + 14) + ":A" + (k + 4 + 14)].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A" + (k + 14) + ":H" + (k + 3 + 14)].Style.Border.Right.Style = ExcelBorderStyle.Hair;
                            ws.Cells["A" + (k + 14) + ":I" + (k + 3 + 14)].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

                            ws.Cells["A" + (k + 3 + 14) + ":I" + (k + 3 + 14)].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                            ws.Cells["A" + (k + 4 + 14) + ":I" + (k + 4 + 14)].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                            ws.Cells["H" + (k + 4 + 14)].Style.Border.Right.Style = ExcelBorderStyle.Medium;

                            ws.Cells["H" + (k + 4 + 14)].Value = string.Format("{0:N}", (Math.Round(Convert.ToDecimal(TSingleList[s].TotalAcceptedQty), 2)));
                            ws.Cells["H" + (k + 4 + 14)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            ws.Cells["H" + (k + 4 + 14)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                            ws.Cells["I" + (k + 4 + 14)].Value = TSingleList[s].TotalTransportCost;
                            ws.Cells["I" + (k + 4 + 14)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            ws.Cells["I" + (k + 4 + 14)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                            ws.Cells["A12:I12,A13:I13"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                            int img = 0;
                            //for Adding image 
                            ws.Row(img * 5).Height = 39.00D;
                            var picture = ws.Drawings.AddPicture(img.ToString(), logo);
                            picture.SetPosition(1, 0, 0, 0);

                            //Printer Settings

                            ws.PrinterSettings.TopMargin = ws.PrinterSettings.BottomMargin = 0.75M;
                            ws.PrinterSettings.LeftMargin = ws.PrinterSettings.RightMargin = 0.7M;
                            ws.PrinterSettings.HeaderMargin = ws.PrinterSettings.FooterMargin = 0.3M;
                            ws.PrinterSettings.Orientation = eOrientation.Landscape;
                            ws.PrinterSettings.PaperSize = ePaperSize.A4;
                            ws.PrinterSettings.FitToPage = true;
                            ws.PrinterSettings.FitToWidth = 1;
                            ws.PrinterSettings.FitToHeight = 0;

                            #endregion
                            s = s + 1;
                        }
                        x = x + 1;
                    }

                    byte[] data = pck.GetAsByteArray();
                    criteria.Clear();

                    var Items = TConsolList[0].Period.ToString().Split('/');

                    string txtTName = "GCC-TPT-Invoice-" + TConsolList[0].Period + "-WK0" + TConsolList[0].Week;

                    if (!string.IsNullOrWhiteSpace(Items[0])) { criteria.Add("Period", Items[0]); }
                    if (!string.IsNullOrWhiteSpace(Items[1])) { criteria.Add("PeriodYear", Items[1]); }
                    criteria.Add("Week", TConsolList[0].Week);
                    criteria.Add("DocumentType", "Transpt-Book");

                    Dictionary<long, IList<ExcelDocuments>> DocumentItems = IS.GetExcelDocumentListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                    if (DocumentItems != null && DocumentItems.FirstOrDefault().Key > 0)
                    {
                        ExcelDocuments ED = IS.GetExcelDocumentsDetailsById(DocumentItems.FirstOrDefault().Value[0].Id);
                        ED.DocumentData = data;
                        long id = IS.SaveOrUpdateExcelDocuments(ED, userId);
                        //SaveDocumentToRecentDownloads(ED, null, string.Empty, null, string.Empty);
                    }
                    else
                    {
                        ExcelDocuments doc = new ExcelDocuments();
                        doc.ControlId = txtTName;
                        doc.Period = Items[0];
                        doc.PeriodYear = Items[1];
                        doc.Week = TConsolList[0].Week;
                        doc.DocumentData = data;
                        doc.DocumentType = "Transpt-Book";
                        doc.DocumentName = txtTName;
                        long id = IS.SaveOrUpdateExcelDocuments(doc, userId);
                        //SaveDocumentToRecentDownloads(doc, null, string.Empty, null, string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightReportPolicy");
                throw ex;
            }
        }

        public ActionResult ExcelConsolGeneration()
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
                    throw ex;
                }
            }
        }
        /// <summary>
        /// Consolidated Excel Invoice generation
        /// </summary>
        /// <param name="searchItems"></param>
        /// <param name="Ids"></param>
        /// <returns></returns>
        public JsonResult GenerateConsolExcel(string searchItems, string Ids)
        {
            string userId = base.ValidateUser();
            try
            {
                Dictionary<string, object> criteria = new Dictionary<string, object>();
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
                    var ConsolList = (from items in DocumentItems.First().Value
                                      select new { items.Sector, items.ContingentType, items.Period, items.PeriodYear }).OrderBy(i => i.Sector).Distinct().ToArray();
                    foreach (var item in ConsolList)
                    {
                        string searchItems1 = item.Sector + ',' + ',' + item.ContingentType + ',' + item.Period + ',' + item.PeriodYear;
                        InvoicePrintExcel(item.Period, searchItems1, false);
                        Count = Count + 1;
                    }
                }
                else //To Generate all the documents from Searchitems
                {
                    if (searchItems != null && searchItems != "")
                    {
                        criteria.Clear();
                        var Items = searchItems.ToString().Split(',');
                        if (!string.IsNullOrWhiteSpace(Items[0])) { criteria.Add("Sector", Items[0]); }
                        if (!string.IsNullOrWhiteSpace(Items[1])) { criteria.Add("Name", Items[1]); }
                        if (!string.IsNullOrWhiteSpace(Items[2])) { criteria.Add("ContingentType", Items[2]); }
                        if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Period", Items[3]); }
                        if (!string.IsNullOrWhiteSpace(Items[4])) { criteria.Add("PeriodYear", Items[4]); }
                        if (Items.Length > 5)
                            if (!string.IsNullOrWhiteSpace(Items[5])) { criteria.Add("Week", Convert.ToInt64(Items[5])); }

                        var TrimItems = searchItems.Replace(",", "");
                        if (string.IsNullOrWhiteSpace(TrimItems))
                            return null;
                    }
                    Dictionary<long, IList<InvoiceManagementView>> invoiceItems = IS.GetInvoiceListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                    if (invoiceItems != null && invoiceItems.FirstOrDefault().Key > 0)
                    {
                        var InvoiceOrderList = (from items in invoiceItems.First().Value
                                                select new { items.Sector, items.ContingentType, items.Period, items.PeriodYear, items.Week }).OrderBy(i => i.Sector).Distinct().ToArray();
                        foreach (var item in InvoiceOrderList)
                        {
                            string searchItems1 = item.Sector + ',' + ',' + item.ContingentType + ',' + item.Period + ',' + item.PeriodYear + ',' + item.Week;
                            InvoicePrintExcel(item.Period, searchItems1, false);
                            Count = Count + 1;
                        }
                    }
                }
                return Json(Count, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region FinalFoodRequistion Report
        public void FinalFoodRequistionExcelSheet(DataSet Workbookset, List<FinalFoodOrderbyWeek> OrdList)
        {
            string userId = base.ValidateUser();
            try
            {
                MastersService mssvc = new MastersService();
                INSIGHT.Entities.User Userobj = (INSIGHT.Entities.User)Session["objUser"];

                string loggedInUserId = Userobj.UserId;
                using (ExcelPackage pck = new ExcelPackage())
                {
                    int TableCount = Workbookset.Tables.Count;
                    int o = 0;
                    decimal TotalDry, TotalChilled, TotalFrozen, TotalDryWk, TotalChilledWk, TotalFrozenWk, TotalDryDay, TotalChilledDay, TotalFrozenDay;
                    TotalDry = TotalChilled = TotalFrozen = TotalDryWk = TotalChilledWk = TotalFrozenWk = TotalDryDay = TotalChilledDay = TotalFrozenDay = 0;


                    string Period = OrdList[o].FinalFoodOrderList.FirstOrDefault().Period;
                    string PeriodYear = OrdList[o].FinalFoodOrderList.FirstOrDefault().PeriodYear;
                    criteria.Add("Period", Period);
                    criteria.Add("PeriodYear", PeriodYear);

                    Dictionary<long, IList<Orders>> OrdersList = OS.GetOrdersListWithPagingAndCriteria(0, 999999, string.Empty, string.Empty, criteria);
                    IList<Orders> Orders = OrdersList.FirstOrDefault().Value;
                    var Weeks = (from u in Orders select u.Week).Distinct().ToArray();
                    //taking the sectorwise sum of troop strength like "SN,SS,SW"
                    var SectorBasedTroops = (from items in Orders
                                             group items by new { items.Sector } into g
                                             orderby g.Key.Sector
                                             select g.Sum(items => Convert.ToDecimal(items.Troops))).ToList();

                    //sector based contingent counts

                    var SectorBasedContingent = (from items in Orders
                                                 group items by new { items.Sector } into g
                                                 orderby g.Key.Sector
                                                 select g.Count()).ToArray();

                    //Week based troops

                    var WeekBasedTroops = (from items in Orders
                                           group items by new { items.Week } into g
                                           orderby g.Key.Week
                                           select g.Sum(items => Convert.ToDecimal(items.Troops))).ToList();



                    //-----------------Consolidate week invoice list----------
                    Dictionary<long, IList<ORDRPT_ConsolidatedWeekReport>> consolidate = OS.GetORDRPT_WeekConsolidateList(Period, PeriodYear, Weeks[0], Weeks[1], Weeks[2], Weeks[3]);
                    IList<ORDRPT_ConsolidatedWeekReport> ConsolidateList = consolidate.FirstOrDefault().Value;
                    //----------------------------ends here---------------------------
                    var TempBasedItemcount = (from items in ConsolidateList
                                              group items by new { items.Temperature, } into g
                                              orderby g.Key.Temperature
                                              select new { temp = g.Key.Temperature, count = g.Count() }).ToArray();

                    #region  Week sheets

                    for (int i = 0; i < TableCount; i++)
                    {
                        if (o < 4)
                        {
                            if (Workbookset.Tables[i].TableName.ToString() == OrdList[o].SheetName)
                            {
                                //Final Food Order Report by Week Added by  Thamizh
                                #region FoodOrder by Week
                                criteria.Clear();
                                Dictionary<long, IList<ItemMaster>> ItemMasterList = mssvc.GetItemMasterListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                                List<ItemMaster> ItemsList = new List<ItemMaster>();
                                var ItemList = (from u in ItemMasterList.FirstOrDefault().Value select new { u.UNCode, u.Commodity, u.Temperature, u.Subs }).ToList();
                                foreach (var item in ItemMasterList.FirstOrDefault().Value)
                                {
                                    ItemMaster obj = new ItemMaster();
                                    obj.UNCode = item.UNCode;
                                    obj.Commodity = item.Commodity;
                                    obj.Temperature = item.Temperature;
                                    obj.Subs = item.Subs;

                                    ItemsList.Add(obj);
                                }
                                ExcelWorksheet ws = pck.Workbook.Worksheets.Add(Workbookset.Tables[i].TableName.ToString());

                                //Header
                                ws.Cells["B3,B4,B5,B6,B7,B8"].Style.Font.Bold = true;

                                ws.Cells["B3"].Value = "SECTOR-LOC-CONTINGENT";
                                ws.Cells["B4"].Value = "HQ location:";
                                ws.Cells["B5"].Value = "location:";
                                ws.Cells["B6"].Value = "Unit Name";
                                ws.Cells["B7"].Value = "Troopstrength:";
                                ws.Cells["B8"].Value = "Per man per day";

                                ws.Cells["A9,B9,C9,A10,B10,C10"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                ws.Cells["A9,B9,C9,A10,B10,C10"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                ws.Cells["A9,B9,C9,A10,B10,C10"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                ws.Cells["A9,B9,C9,A10,B10,C10"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                ws.Cells["A9,B9,C9,A10,B10,C10"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                                ws.Cells["A9,B9,C9,A10,B10,C10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells["A9,B9,C9,A10,B10,C10"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                ws.Cells["A9,B9,C9,A10,B10,C10"].Style.Fill.BackgroundColor.SetColor(Color.DarkSlateBlue);
                                ws.Cells["A9,B9,C9,A10,B10,C10"].Style.Font.Color.SetColor(Color.White);
                                ws.Cells["A9,B9,C9,A10,B10,C10"].Style.Font.Bold = true;

                                ws.Cells["A9"].Value = "Code No.";
                                ws.Cells["B9"].Value = "COMMODITY";
                                ws.Cells["C9"].Value = "Temperature";


                                ws.Cells["A11,B11,C11"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                ws.Cells["A11,B11,C11"].Style.Fill.BackgroundColor.SetColor(Color.Black);
                                ws.Cells["A11,B11,C11"].Style.Font.Color.SetColor(Color.White);
                                ws.Cells["A11,B11,C11"].Style.Font.Bold = true;

                                int Rowcount = ItemList.Count() + 13;
                                ws.Cells["A13:c" + Rowcount].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells["A13:c" + Rowcount].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                ws.Cells["A13:c" + Rowcount].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                ws.Cells["A13:c" + Rowcount].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                ws.Cells["A13:c" + Rowcount].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                ws.View.ZoomScale = 70;
                                ws.View.ShowGridLines = true;

                                ws.Row(9).Height = 72.00;
                                ws.Column(1).Width = 17.29;
                                ws.Column(2).Width = 58.00;
                                ws.Column(3).Width = 20.14;

                                ws.Cells["A9,B9,C9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; //Center Alignments for Cells

                                ws.Cells["B1"].Value = OrdList[o].Title;
                                ws.Cells["B1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                ws.Cells["B1"].Style.Font.Color.SetColor(Color.Red);
                                ws.Cells["B1"].Style.Fill.BackgroundColor.SetColor(Color.White);
                                ws.Cells["B1"].Style.Font.Bold = true;

                                var seclocLst = (from u in OrdList[o].FinalFoodOrderList select new { u.SectorLocContingent, u.Location, u.Name, u.Warehouse, u.Troops, u.Sector }).Distinct().ToArray();

                                int SectorColcount = seclocLst.Count() + 3;

                                ws.Cells[3, 4, 3, SectorColcount].Style.Font.Bold = true;
                                ws.Cells[3, 4, 3, SectorColcount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[3, 4, 3, SectorColcount].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                ws.Cells[4, 4, 4, SectorColcount + 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells[5, 4, 5, SectorColcount + 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells[6, 4, 6, SectorColcount + 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells[7, 4, 7, SectorColcount + 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells[4, 4, 4, SectorColcount + 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[5, 4, 5, SectorColcount + 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[6, 4, 6, SectorColcount + 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[7, 4, 7, SectorColcount + 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                ws.Cells[4, 4, 4, SectorColcount].Style.Border.Top.Style = ws.Cells[4, 4, 4, SectorColcount].Style.Border.Bottom.Style = ws.Cells[4, 4, 4, SectorColcount].Style.Border.Left.Style = ws.Cells[4, 4, 4, SectorColcount].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                ws.Cells[5, 4, 5, SectorColcount].Style.Border.Top.Style = ws.Cells[5, 4, 5, SectorColcount].Style.Border.Bottom.Style = ws.Cells[5, 4, 5, SectorColcount].Style.Border.Left.Style = ws.Cells[5, 4, 5, SectorColcount].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                ws.Cells[6, 4, 6, SectorColcount].Style.Border.Top.Style = ws.Cells[6, 4, 6, SectorColcount].Style.Border.Bottom.Style = ws.Cells[6, 4, 6, SectorColcount].Style.Border.Left.Style = ws.Cells[6, 4, 6, SectorColcount].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                ws.Cells[7, 4, 7, SectorColcount].Style.Border.Top.Style = ws.Cells[7, 4, 7, SectorColcount].Style.Border.Bottom.Style = ws.Cells[7, 4, 7, SectorColcount].Style.Border.Left.Style = ws.Cells[7, 4, 7, SectorColcount].Style.Border.Right.Style = ExcelBorderStyle.Thin;


                                ws.Cells[4, 4, Rowcount, SectorColcount + 21].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells[4, 4, Rowcount, SectorColcount + 21].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[4, 4, Rowcount, SectorColcount].Style.Border.Top.Style = ws.Cells[4, 4, Rowcount, SectorColcount].Style.Border.Bottom.Style = ws.Cells[4, 4, Rowcount, SectorColcount].Style.Border.Left.Style = ws.Cells[4, 4, Rowcount, SectorColcount].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                ws.Cells[4, 4, Rowcount, SectorColcount].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                ws.Cells[4, 4, Rowcount, SectorColcount].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                ws.Cells[4, 4, Rowcount, SectorColcount].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                ws.Cells[9, 4, 9, SectorColcount].Style.HorizontalAlignment = ws.Cells[10, 4, 10, SectorColcount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[9, 4, 9, SectorColcount].Style.VerticalAlignment = ws.Cells[10, 4, 10, SectorColcount].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                ws.Cells[9, 4, 9, SectorColcount].Style.WrapText = ws.Cells[10, 4, 10, SectorColcount].Style.WrapText = true;
                                ws.Cells[9, 4, 9, SectorColcount].Style.Fill.PatternType = ws.Cells[10, 4, 10, SectorColcount].Style.Fill.PatternType = ws.Cells[11, 4, 11, SectorColcount + 21].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                ws.Cells[9, 4, 9, SectorColcount].Style.Font.Bold = ws.Cells[10, 4, 10, SectorColcount].Style.Font.Bold = ws.Cells[11, 4, 11, SectorColcount + 21].Style.Font.Bold = true;

                                ws.Cells[9, 4, 9, SectorColcount].Style.Fill.BackgroundColor.SetColor(Color.DarkSlateBlue);
                                ws.Cells[9, 4, 9, SectorColcount].Style.Font.Color.SetColor(Color.White);

                                ws.Cells[10, 4, 10, SectorColcount].Style.Fill.BackgroundColor.SetColor(Color.DarkSlateBlue);
                                ws.Cells[10, 4, 10, SectorColcount].Style.Font.Color.SetColor(Color.White);


                                ws.Cells[11, 4, 11, SectorColcount + 21].Style.Fill.BackgroundColor.SetColor(Color.Black);
                                ws.Cells[11, 4, 11, SectorColcount + 21].Style.Font.Color.SetColor(Color.White);



                                int j;
                                for (j = 4; j < seclocLst.Length + 4; j++)
                                {
                                    ws.Column(j).Width = 25.14;
                                    ws.Cells[4, j].Style.Font.Color.SetColor(Color.White);
                                    ws.Cells[5, j].Style.Font.Color.SetColor(Color.White);
                                    ws.Cells[3, j].Value = seclocLst[j - 4].SectorLocContingent;

                                    if (seclocLst[j - 4].Sector == "FS")
                                    {
                                        ws.Cells[4, j].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        ws.Cells[4, j].Style.Fill.BackgroundColor.SetColor(Color.Green);

                                        ws.Cells[5, j].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        ws.Cells[5, j].Style.Fill.BackgroundColor.SetColor(Color.Green);

                                    }

                                    if (seclocLst[j - 4].Sector == "NS")
                                    {
                                        ws.Cells[4, j].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        ws.Cells[4, j].Style.Fill.BackgroundColor.SetColor(Color.Red);

                                        ws.Cells[5, j].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        ws.Cells[5, j].Style.Fill.BackgroundColor.SetColor(Color.Red);
                                    }
                                    if (seclocLst[j - 4].Sector == "GS")
                                    {
                                        ws.Cells[4, j].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        ws.Cells[4, j].Style.Fill.BackgroundColor.SetColor(Color.Orange);
                                        ws.Cells[5, j].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        ws.Cells[5, j].Style.Fill.BackgroundColor.SetColor(Color.Orange);
                                        ws.Cells[5, j].Style.Font.Color.SetColor(Color.Black);
                                    }
                                    ws.Cells[4, j].Value = seclocLst[j - 4].Warehouse;
                                    ws.Cells[5, j].Value = seclocLst[j - 4].Location;

                                    ws.Cells[6, j].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells[6, j].Style.Fill.BackgroundColor.SetColor(Color.IndianRed);
                                    ws.Cells[6, j].Value = seclocLst[j - 4].Name;

                                    ws.Cells[7, j].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells[7, j].Style.Fill.BackgroundColor.SetColor(Color.SandyBrown);
                                    ws.Cells[7, j].Value = seclocLst[j - 4].Troops;

                                    ws.Cells[9, j].Value = "Total Quantity Ordered";

                                    ws.Cells[10, j].Value = "(Kg/Lt/EA)";

                                    var uncodelist = (from u in OrdList[o].FinalFoodOrderList where u.SectorLocContingent == seclocLst[j - 4].SectorLocContingent orderby u.UNCode select new { u.UNCode1, u.UNCode, u.Commodity1, u.Commodity, u.OrderQty }).ToList();

                                    ws.Cells[12, j].Value = Math.Round(uncodelist.Sum(x => Convert.ToDecimal(x.OrderQty)), 3);

                                    int k = 0;
                                    int count = ItemsList.Count() + 13;
                                    int L = 0;
                                    for (int p = 13; p < count; p++)
                                    {
                                        if (k < uncodelist.Count())
                                        {
                                            if (ItemList[L].UNCode == uncodelist[k].UNCode)
                                            {
                                                ws.Cells[p, j].Value = Math.Round(uncodelist[k].OrderQty, 3);

                                                if (uncodelist[k].UNCode == 1129)
                                                {
                                                    ws.Cells[11, j].Value = Math.Round(uncodelist.Sum(x => Convert.ToDecimal(x.OrderQty)) - uncodelist[k].OrderQty + uncodelist[k].OrderQty * Convert.ToDecimal(0.058824), 3);
                                                    //   ws.Cells[12, j].Value = uncodelist.Sum(x => Convert.ToDecimal(x.OrderQty));
                                                }
                                                if (uncodelist[k].UNCode != 1129)
                                                {
                                                    if (ws.Cells[11, j].Value == null)
                                                    {
                                                        ws.Cells[11, j].Value = Math.Round(uncodelist.Sum(x => Convert.ToDecimal(x.OrderQty)), 3);
                                                    }
                                                }

                                                k = k + 1;
                                            }
                                            else
                                            {
                                                ws.Cells[p, j].Value = "0.000";
                                            }
                                        }
                                        else
                                        {
                                            ws.Cells[p, j].Value = "0.000";
                                        }

                                        ws.Row(p).Height = 21.20;
                                        L = L + 1;
                                    }
                                }

                                var SectorLst = (from u in OrdList[o].FinalFoodOrderList select u.Sector).Distinct().ToList();
                                for (int m = 0; m < SectorLst.Count; m++)
                                {
                                    j = j + 2;

                                    ws.Cells[4, j, Rowcount, j].Style.Border.Top.Style = ws.Cells[4, j, Rowcount, j].Style.Border.Bottom.Style = ws.Cells[4, j, Rowcount, j].Style.Border.Left.Style = ws.Cells[4, j, Rowcount, j].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                    ws.Column(j).Width = 25.14;
                                    ws.Column(j).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Column(j).Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                    ws.Cells[9, j].Style.Fill.PatternType = ws.Cells[10, j].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells[9, j].Value = "Total Quantity Ordered";
                                    ws.Cells[9, j].Style.Fill.BackgroundColor.SetColor(Color.DarkSlateBlue);
                                    ws.Cells[9, j].Style.Font.Color.SetColor(Color.White);
                                    ws.Cells[10, j].Value = "(Kg/Lt/EA)";
                                    ws.Cells[10, j].Style.Fill.BackgroundColor.SetColor(Color.DarkSlateBlue);
                                    ws.Cells[10, j].Style.Font.Color.SetColor(Color.White);

                                    var uncodelist = (from u in OrdList[o].FinalFoodOrderList where u.Sector == SectorLst[m] orderby u.UNCode select new { u.UNCode, u.sector1, u.sector2, u.sector3, u.SSOrders, u.SNOrders, u.SWOrders, u.SSTroops, u.SNTroops, u.SWTroops }).Distinct().ToList();

                                    if (SectorLst[m] == "NS")
                                    {
                                        ws.Cells[12, j].Value = Math.Round(uncodelist.Sum(x => Convert.ToDecimal(x.sector1)), 3);

                                        ws.Cells[4, j].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        ws.Cells[4, j].Style.Fill.BackgroundColor.SetColor(Color.Red);
                                        ws.Cells[4, j].Style.Font.Color.SetColor(Color.White);

                                        ws.Cells[5, j].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        ws.Cells[5, j].Style.Fill.BackgroundColor.SetColor(Color.Red);
                                        ws.Cells[5, j].Style.Font.Color.SetColor(Color.White);

                                        ws.Cells[4, j].Value = "NYALA";
                                        ws.Cells[5, j].Value = "TOTAL";

                                        ws.Cells[6, j].Style.Fill.PatternType = ws.Cells[7, j].Style.Fill.PatternType = ExcelFillStyle.Solid;

                                        ws.Cells[6, j].Style.Fill.BackgroundColor.SetColor(Color.IndianRed);
                                        ws.Cells[6, j].Style.Font.Color.SetColor(Color.White);

                                        ws.Cells[7, j].Style.Fill.BackgroundColor.SetColor(Color.SandyBrown);
                                        ws.Cells[7, j].Style.Font.Color.SetColor(Color.White);

                                        ws.Cells[6, j].Value = uncodelist.FirstOrDefault().SSOrders;
                                        ws.Cells[7, j].Value = uncodelist.FirstOrDefault().SSTroops;

                                        int k = 0;
                                        int count = ItemsList.Count() + 13;
                                        int L = 0;
                                        for (int p = 13; p < count; p++)
                                        {
                                            if (k < uncodelist.Count())
                                            {
                                                if (ItemList[L].UNCode == uncodelist[k].UNCode)
                                                {
                                                    if (uncodelist[k].UNCode == 1129)
                                                    {

                                                        ws.Cells[11, j].Value = Math.Round(uncodelist.Sum(x => Convert.ToDecimal(x.sector1)) - uncodelist[k].sector1 + uncodelist[k].sector1 * Convert.ToDecimal(0.058824), 3);
                                                        ws.Cells[12, j].Value = Math.Round(uncodelist.Sum(x => Convert.ToDecimal(x.sector1)), 3);
                                                    }
                                                    //if (uncodelist[k].UNCode != 1129)
                                                    //{
                                                    //    if (ws.Cells[11, j].Value == null)
                                                    //    {
                                                    //        ws.Cells[11, j].Value = Math.Round(uncodelist.Sum(x => Convert.ToDecimal(x.sector1)), 2);
                                                    //    }
                                                    //}
                                                    ws.Cells[p, j].Value = Math.Round(uncodelist[k].sector1, 3);
                                                    k = k + 1;
                                                }
                                                else
                                                {
                                                    ws.Cells[p, j].Value = "0.000";
                                                }
                                            }
                                            else
                                            {
                                                ws.Cells[p, j].Value = "0.000";
                                            }

                                            ws.Row(p).Height = 21.20;
                                            L = L + 1;
                                        }
                                    }
                                    if (SectorLst[m] == "FS")
                                    {
                                        ws.Cells[6, j].Value = uncodelist.FirstOrDefault().SNOrders;
                                        ws.Cells[7, j].Value = uncodelist.FirstOrDefault().SNTroops;
                                        ws.Cells[6, j].Style.Fill.PatternType = ws.Cells[7, j].Style.Fill.PatternType = ExcelFillStyle.Solid;

                                        ws.Cells[6, j].Style.Fill.BackgroundColor.SetColor(Color.IndianRed);
                                        ws.Cells[6, j].Style.Font.Color.SetColor(Color.White);

                                        ws.Cells[7, j].Style.Fill.BackgroundColor.SetColor(Color.SandyBrown);
                                        ws.Cells[7, j].Style.Font.Color.SetColor(Color.White);

                                        ws.Cells[12, j].Value = uncodelist.Sum(x => Convert.ToDecimal(x.sector2));

                                        ws.Cells[4, j].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        ws.Cells[4, j].Style.Fill.BackgroundColor.SetColor(Color.Green);
                                        ws.Cells[4, j].Style.Font.Color.SetColor(Color.White);

                                        ws.Cells[5, j].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        ws.Cells[5, j].Style.Fill.BackgroundColor.SetColor(Color.Green);
                                        ws.Cells[5, j].Style.Font.Color.SetColor(Color.White);

                                        ws.Cells[4, j].Value = "EL FASHER";
                                        ws.Cells[5, j].Value = "TOTAL";
                                        int k = 0;
                                        int count = ItemsList.Count() + 13;
                                        int L = 0;
                                        for (int p = 13; p < count; p++)
                                        {
                                            if (k < uncodelist.Count())
                                            {
                                                if (ItemList[L].UNCode == uncodelist[k].UNCode)
                                                {
                                                    if (uncodelist[k].UNCode == 1129)
                                                    {
                                                        ws.Cells[11, j].Value = Math.Round(uncodelist.Sum(x => Convert.ToDecimal(x.sector2)) - uncodelist[k].sector2 + uncodelist[k].sector2 * Convert.ToDecimal(0.058824), 3);
                                                        ws.Cells[12, j].Value = Math.Round(uncodelist.Sum(x => Convert.ToDecimal(x.sector2)), 3);
                                                    }
                                                    //if (uncodelist[k].UNCode != 1129)
                                                    //{
                                                    //    if (ws.Cells[11, j].Value == null)
                                                    //    {
                                                    //        ws.Cells[11, j].Value = Math.Round(uncodelist.Sum(x => Convert.ToDecimal(x.sector2)), 2);
                                                    //    }
                                                    //}
                                                    ws.Cells[p, j].Value = Math.Round(uncodelist[k].sector2, 3);
                                                    k = k + 1;
                                                }
                                                else
                                                {
                                                    ws.Cells[p, j].Value = "0.000";
                                                }
                                            }
                                            else
                                            {
                                                ws.Cells[p, j].Value = "0.000";
                                            }

                                            ws.Row(p).Height = 21.20;
                                            L = L + 1;
                                        }
                                    }
                                    if (SectorLst[m] == "GS")
                                    {
                                        ws.Cells[6, j].Style.Fill.PatternType = ws.Cells[7, j].Style.Fill.PatternType = ExcelFillStyle.Solid;

                                        ws.Cells[6, j].Style.Fill.BackgroundColor.SetColor(Color.IndianRed);
                                        ws.Cells[6, j].Style.Font.Color.SetColor(Color.White);

                                        ws.Cells[7, j].Style.Fill.BackgroundColor.SetColor(Color.SandyBrown);
                                        ws.Cells[7, j].Style.Font.Color.SetColor(Color.White);

                                        ws.Cells[6, j].Value = uncodelist.FirstOrDefault().SWOrders;
                                        ws.Cells[7, j].Value = uncodelist.FirstOrDefault().SWTroops;

                                        ws.Cells[4, j].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        ws.Cells[4, j].Style.Fill.BackgroundColor.SetColor(Color.Orange);
                                        ws.Cells[4, j].Style.Font.Color.SetColor(Color.White);

                                        ws.Cells[5, j].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        ws.Cells[5, j].Style.Fill.BackgroundColor.SetColor(Color.Orange);
                                        ws.Cells[5, j].Style.Font.Color.SetColor(Color.White);

                                        ws.Cells[4, j].Value = "EL GENAINA";
                                        ws.Cells[5, j].Value = "TOTAL";
                                        int k = 0;
                                        int count = ItemsList.Count() + 13;
                                        int L = 0;
                                        for (int p = 13; p < count; p++)
                                        {
                                            if (k < uncodelist.Count())
                                            {
                                                if (ItemList[L].UNCode == uncodelist[k].UNCode)
                                                {
                                                    if (uncodelist[k].UNCode == 1129)
                                                    {
                                                        ws.Cells[11, j].Value = Math.Round(uncodelist.Sum(x => Convert.ToDecimal(x.sector3)) - uncodelist[k].sector3 + uncodelist[k].sector3 * Convert.ToDecimal(0.058824), 3);
                                                        ws.Cells[12, j].Value = Math.Round(uncodelist.Sum(x => Convert.ToDecimal(x.sector3)), 3);
                                                    }
                                                    //if (uncodelist[k].UNCode != 1129)
                                                    //{
                                                    //    if (ws.Cells[11, j].Value == null)
                                                    //    {
                                                    //        ws.Cells[11, j].Value = Math.Round(uncodelist.Sum(x => Convert.ToDecimal(x.sector3)), 2);
                                                    //    }
                                                    //}
                                                    ws.Cells[p, j].Value = Math.Round(uncodelist[k].sector3, 3);
                                                    k = k + 1;
                                                }
                                                else
                                                {
                                                    ws.Cells[p, j].Value = "0.000";
                                                }
                                            }
                                            else
                                            {
                                                ws.Cells[p, j].Value = "0.000";
                                            }

                                            ws.Row(p).Height = 21.20;
                                            L = L + 1;
                                        }
                                    }
                                }


                                var itemslst = (from u in OrdList[o].FinalFoodOrderList orderby u.UNCode select new { u.UNCode, u.TotalOrdQty, u.QtyWithEggs, u.TotalTroops }).Distinct().ToList();

                                int k1 = 0;
                                int count1 = ItemsList.Count() + 13;
                                int L1 = 0;
                                j = j + 2;
                                ws.Column(j).Width = 25.14;
                                ws.Column(j + 1).Width = 25.14;
                                ws.Column(j + 2).Width = 25.14;
                                ws.Column(j + 3).Width = 25.14;
                                ws.Column(j + 4).Width = 25.14;
                                ws.Column(j + 5).Width = 25.14;
                                ws.Column(j + 6).Width = 25.14;
                                ws.Column(j + 7).Width = 25.14;
                                ws.Column(j + 8).Width = 25.14;
                                ws.Column(j + 9).Width = 25.14;
                                ws.Column(j + 10).Width = 25.14;
                                ws.Column(j + 11).Width = 25.14;
                                ws.Column(j + 12).Width = 25.14;

                                ws.Cells[4, j].Value = "Contingents:";
                                ws.Cells[4, j + 1].Value = seclocLst.Count();

                                ws.Cells[5, j].Value = "Eggs in kg";
                                ws.Cells[5, j + 1].Value = Math.Round(itemslst.FirstOrDefault().QtyWithEggs, 3);

                                ws.Cells[6, j].Value = "Eggs each as UN calc:";
                                ws.Cells[6, j].Style.WrapText = true;


                                ws.Cells[7, j].Value = "Total:";
                                ws.Cells[7, j + 1].Value = itemslst.FirstOrDefault().TotalTroops;
                                ws.Cells[7, j].Style.Fill.PatternType = ws.Cells[7, j + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                ws.Cells[7, j].Style.Fill.BackgroundColor.SetColor(Color.SandyBrown);
                                ws.Cells[7, j].Style.Font.Color.SetColor(Color.White);
                                ws.Cells[7, j + 1].Style.Fill.BackgroundColor.SetColor(Color.SandyBrown);
                                ws.Cells[7, j + 1].Style.Font.Color.SetColor(Color.White);

                                ws.Cells[4, j].Style.Font.Bold = ws.Cells[4, j + 1].Style.Font.Bold = ws.Cells[5, j].Style.Font.Bold = ws.Cells[5, j + 1].Style.Font.Bold = true;
                                ws.Cells[6, j].Style.Font.Bold = ws.Cells[6, j + 1].Style.Font.Bold = ws.Cells[7, j].Style.Font.Bold = ws.Cells[7, j + 1].Style.Font.Bold = true;

                                ws.Cells[4, j, 7, j].Style.Border.Left.Style = ws.Cells[4, j + 1, 7, j + 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                ws.Cells[4, j, 4, j + 1].Style.Border.Top.Style = ws.Cells[7, j, 7, j + 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                ws.Cells[9, j].Value = "Total Quantity Ordered";

                                ws.Cells[10, j].Value = "(Kg/Lt/EA)";
                                ws.Cells[9, j].Style.Fill.PatternType = ws.Cells[10, j].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                ws.Cells[9, j].Style.Font.Bold = ws.Cells[10, j].Style.Font.Bold = true;
                                ws.Cells[9, j].Style.Fill.BackgroundColor.SetColor(Color.DarkSlateBlue);
                                ws.Cells[9, j].Style.Font.Color.SetColor(Color.White);
                                ws.Cells[10, j].Style.Fill.BackgroundColor.SetColor(Color.DarkSlateBlue);
                                ws.Cells[10, j].Style.Font.Color.SetColor(Color.White);

                                ws.Cells[9, j, Rowcount, j].Style.Border.Top.Style = ws.Cells[9, j, Rowcount, j].Style.Border.Bottom.Style = ws.Cells[9, j, Rowcount, j].Style.Border.Left.Style = ws.Cells[9, j, Rowcount, j].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                ws.Cells[7, j + 2, 8, j + 4].Merge = ws.Cells[7, j + 6, 8, j + 8].Merge = ws.Cells[7, j + 10, 8, j + 12].Merge = true;
                                ws.Cells[7, j + 2].Style.Font.UnderLine = ws.Cells[7, j + 6].Style.Font.UnderLine = ws.Cells[7, j + 10].Style.Font.UnderLine = true;
                                ws.Cells[7, j + 2].Style.Font.Bold = ws.Cells[7, j + 6].Style.Font.Bold = ws.Cells[7, j + 10].Style.Font.Bold = true;
                                ws.Cells[7, j + 2].Style.WrapText = ws.Cells[7, j + 6].Style.WrapText = ws.Cells[7, j + 10].Style.WrapText = true;

                                ws.Cells[7, j + 2].Style.HorizontalAlignment = ws.Cells[7, j + 6].Style.HorizontalAlignment = ws.Cells[7, j + 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[7, j + 2].Style.VerticalAlignment = ws.Cells[7, j + 6].Style.VerticalAlignment = ws.Cells[7, j + 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                ws.Cells[7, j + 2].Value = "Divided in A/C/F";
                                ws.Cells[7, j + 2, 7, j + 4].Style.Border.Top.Style = ws.Cells[7, j + 2, 7, j + 4].Style.Border.Left.Style = ws.Cells[7, j + 2, 7, j + 4].Style.Border.Right.Style = ws.Cells[7, j + 2, 7, j + 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                ws.Cells[8, j + 2, 8, j + 4].Style.Border.Top.Style = ws.Cells[8, j + 2, 8, j + 4].Style.Border.Left.Style = ws.Cells[8, j + 2, 8, j + 4].Style.Border.Right.Style = ws.Cells[8, j + 2, 8, j + 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                                ws.Cells[7, j + 6].Value = "volume per week";
                                ws.Cells[7, j + 6, 7, j + 8].Style.Border.Top.Style = ws.Cells[7, j + 6, 7, j + 8].Style.Border.Left.Style = ws.Cells[7, j + 6, 7, j + 8].Style.Border.Right.Style = ws.Cells[7, j + 6, 7, j + 8].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                ws.Cells[8, j + 6, 8, j + 8].Style.Border.Top.Style = ws.Cells[8, j + 6, 8, j + 8].Style.Border.Left.Style = ws.Cells[8, j + 6, 8, j + 8].Style.Border.Right.Style = ws.Cells[8, j + 6, 8, j + 8].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                                ws.Cells[7, j + 10].Value = "volume per day";
                                ws.Cells[7, j + 10, 7, j + 12].Style.Border.Top.Style = ws.Cells[7, j + 10, 7, j + 12].Style.Border.Left.Style = ws.Cells[7, j + 10, 7, j + 12].Style.Border.Right.Style = ws.Cells[7, j + 10, 7, j + 12].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                ws.Cells[8, j + 10, 8, j + 12].Style.Border.Top.Style = ws.Cells[8, j + 10, 8, j + 12].Style.Border.Left.Style = ws.Cells[8, j + 10, 8, j + 12].Style.Border.Right.Style = ws.Cells[8, j + 10, 8, j + 12].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                                ws.Cells[9, j + 2, Rowcount, j + 4].Style.Border.Top.Style = ws.Cells[9, j + 2, Rowcount, j + 4].Style.Border.Left.Style = ws.Cells[9, j + 2, Rowcount, j + 4].Style.Border.Right.Style = ws.Cells[9, j + 2, Rowcount, j + 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                ws.Cells[9, j + 6, Rowcount, j + 8].Style.Border.Top.Style = ws.Cells[9, j + 6, Rowcount, j + 8].Style.Border.Left.Style = ws.Cells[9, j + 6, Rowcount, j + 8].Style.Border.Right.Style = ws.Cells[9, j + 6, Rowcount, j + 8].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                ws.Cells[9, j + 10, Rowcount, j + 12].Style.Border.Top.Style = ws.Cells[9, j + 10, Rowcount, j + 12].Style.Border.Left.Style = ws.Cells[9, j + 10, Rowcount, j + 12].Style.Border.Right.Style = ws.Cells[9, j + 10, Rowcount, j + 12].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                                ws.Cells[9, j + 2].Value = "Dry /kg";
                                ws.Cells[9, j + 6].Value = "Dry /kg";
                                ws.Cells[9, j + 10].Value = "Dry /kg";
                                ws.Cells[10, j + 2].Value = "A";
                                ws.Cells[10, j + 6].Value = "A";
                                ws.Cells[10, j + 10].Value = "A";

                                ws.Cells[9, j + 2].Style.HorizontalAlignment = ws.Cells[9, j + 6].Style.HorizontalAlignment = ws.Cells[9, j + 10].Style.HorizontalAlignment = ws.Cells[10, j + 2].Style.HorizontalAlignment = ws.Cells[10, j + 6].Style.HorizontalAlignment = ws.Cells[10, j + 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[9, j + 2].Style.VerticalAlignment = ws.Cells[9, j + 6].Style.VerticalAlignment = ws.Cells[9, j + 10].Style.VerticalAlignment = ws.Cells[10, j + 2].Style.VerticalAlignment = ws.Cells[10, j + 6].Style.VerticalAlignment = ws.Cells[10, j + 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells[9, j + 2].Style.Fill.PatternType = ws.Cells[9, j + 6].Style.Fill.PatternType = ws.Cells[9, j + 10].Style.Fill.PatternType = ws.Cells[10, j + 2].Style.Fill.PatternType = ws.Cells[10, j + 6].Style.Fill.PatternType = ws.Cells[10, j + 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                ws.Cells[9, j + 2].Style.Font.Bold = ws.Cells[9, j + 6].Style.Font.Bold = ws.Cells[9, j + 10].Style.Font.Bold = ws.Cells[10, j + 2].Style.Font.Bold = ws.Cells[10, j + 6].Style.Font.Bold = ws.Cells[10, j + 10].Style.Font.Bold = true;
                                ws.Cells[9, j + 2].Style.WrapText = ws.Cells[9, j + 6].Style.WrapText = ws.Cells[9, j + 10].Style.WrapText = ws.Cells[10, j + 2].Style.WrapText = ws.Cells[10, j + 6].Style.WrapText = ws.Cells[10, j + 10].Style.WrapText = true;

                                ws.Cells[9, j + 3].Value = "Chilled /kg";
                                ws.Cells[9, j + 7].Value = "Chilled /kg";
                                ws.Cells[9, j + 11].Value = "Chilled /kg";
                                ws.Cells[10, j + 3].Value = "C";
                                ws.Cells[10, j + 7].Value = "C";
                                ws.Cells[10, j + 11].Value = "C";
                                ws.Cells[9, j + 3].Style.HorizontalAlignment = ws.Cells[9, j + 7].Style.HorizontalAlignment = ws.Cells[9, j + 11].Style.HorizontalAlignment = ws.Cells[10, j + 3].Style.HorizontalAlignment = ws.Cells[10, j + 7].Style.HorizontalAlignment = ws.Cells[10, j + 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[9, j + 3].Style.VerticalAlignment = ws.Cells[9, j + 7].Style.VerticalAlignment = ws.Cells[9, j + 11].Style.VerticalAlignment = ws.Cells[10, j + 3].Style.VerticalAlignment = ws.Cells[10, j + 7].Style.VerticalAlignment = ws.Cells[10, j + 11].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells[9, j + 3].Style.Fill.PatternType = ws.Cells[9, j + 7].Style.Fill.PatternType = ws.Cells[9, j + 11].Style.Fill.PatternType = ws.Cells[10, j + 3].Style.Fill.PatternType = ws.Cells[10, j + 7].Style.Fill.PatternType = ws.Cells[10, j + 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                ws.Cells[9, j + 3].Style.Font.Bold = ws.Cells[9, j + 7].Style.Font.Bold = ws.Cells[9, j + 11].Style.Font.Bold = ws.Cells[10, j + 3].Style.Font.Bold = ws.Cells[10, j + 7].Style.Font.Bold = ws.Cells[10, j + 11].Style.Font.Bold = true;
                                ws.Cells[9, j + 3].Style.WrapText = ws.Cells[9, j + 7].Style.WrapText = ws.Cells[9, j + 11].Style.WrapText = ws.Cells[10, j + 3].Style.WrapText = ws.Cells[10, j + 7].Style.WrapText = ws.Cells[10, j + 11].Style.WrapText = true;

                                ws.Cells[9, j + 4].Value = "Frozen /kg";
                                ws.Cells[9, j + 8].Value = "Frozen /kg";
                                ws.Cells[9, j + 12].Value = "Frozen /kg";
                                ws.Cells[10, j + 4].Value = "F";
                                ws.Cells[10, j + 8].Value = "F";
                                ws.Cells[10, j + 12].Value = "F";
                                ws.Cells[9, j + 4].Style.HorizontalAlignment = ws.Cells[9, j + 8].Style.HorizontalAlignment = ws.Cells[9, j + 12].Style.HorizontalAlignment = ws.Cells[10, j + 4].Style.HorizontalAlignment = ws.Cells[10, j + 8].Style.HorizontalAlignment = ws.Cells[10, j + 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[9, j + 4].Style.VerticalAlignment = ws.Cells[9, j + 8].Style.VerticalAlignment = ws.Cells[9, j + 12].Style.VerticalAlignment = ws.Cells[10, j + 4].Style.VerticalAlignment = ws.Cells[10, j + 8].Style.VerticalAlignment = ws.Cells[10, j + 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells[9, j + 4].Style.Fill.PatternType = ws.Cells[9, j + 8].Style.Fill.PatternType = ws.Cells[9, j + 12].Style.Fill.PatternType = ws.Cells[10, j + 4].Style.Fill.PatternType = ws.Cells[10, j + 8].Style.Fill.PatternType = ws.Cells[10, j + 12].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                ws.Cells[9, j + 4].Style.Font.Bold = ws.Cells[9, j + 8].Style.Font.Bold = ws.Cells[9, j + 12].Style.Font.Bold = ws.Cells[10, j + 4].Style.Font.Bold = ws.Cells[10, j + 8].Style.Font.Bold = ws.Cells[10, j + 12].Style.Font.Bold = true;
                                ws.Cells[9, j + 4].Style.WrapText = ws.Cells[9, j + 8].Style.WrapText = ws.Cells[9, j + 12].Style.WrapText = ws.Cells[10, j + 4].Style.WrapText = ws.Cells[10, j + 8].Style.WrapText = ws.Cells[10, j + 12].Style.WrapText = true;

                                Color Dry = System.Drawing.ColorTranslator.FromHtml("#CCFFFF");
                                Color Chill = System.Drawing.ColorTranslator.FromHtml("#FFFFCC");
                                Color Frozen = System.Drawing.ColorTranslator.FromHtml("#FF9900");

                                ws.Cells[9, j + 2].Style.Fill.BackgroundColor.SetColor(Dry);
                                ws.Cells[9, j + 2].Style.Font.Color.SetColor(Color.Black);
                                ws.Cells[10, j + 2].Style.Fill.BackgroundColor.SetColor(Dry);
                                ws.Cells[10, j + 2].Style.Font.Color.SetColor(Color.Black);

                                ws.Cells[9, j + 3].Style.Fill.BackgroundColor.SetColor(Chill);
                                ws.Cells[9, j + 3].Style.Font.Color.SetColor(Color.Black);
                                ws.Cells[10, j + 3].Style.Fill.BackgroundColor.SetColor(Chill);
                                ws.Cells[10, j + 3].Style.Font.Color.SetColor(Color.Black);

                                ws.Cells[9, j + 4].Style.Fill.BackgroundColor.SetColor(Frozen);
                                ws.Cells[9, j + 4].Style.Font.Color.SetColor(Color.Black);
                                ws.Cells[10, j + 4].Style.Fill.BackgroundColor.SetColor(Frozen);
                                ws.Cells[10, j + 4].Style.Font.Color.SetColor(Color.Black);

                                ws.Cells[9, j + 6].Style.Fill.BackgroundColor.SetColor(Dry);
                                ws.Cells[9, j + 6].Style.Font.Color.SetColor(Color.Black);
                                ws.Cells[10, j + 6].Style.Fill.BackgroundColor.SetColor(Dry);
                                ws.Cells[10, j + 6].Style.Font.Color.SetColor(Color.Black);

                                ws.Cells[9, j + 7].Style.Fill.BackgroundColor.SetColor(Chill);
                                ws.Cells[9, j + 7].Style.Font.Color.SetColor(Color.Black);
                                ws.Cells[10, j + 7].Style.Fill.BackgroundColor.SetColor(Chill);
                                ws.Cells[10, j + 7].Style.Font.Color.SetColor(Color.Black);

                                ws.Cells[9, j + 8].Style.Fill.BackgroundColor.SetColor(Frozen);
                                ws.Cells[9, j + 8].Style.Font.Color.SetColor(Color.Black);
                                ws.Cells[10, j + 8].Style.Fill.BackgroundColor.SetColor(Frozen);
                                ws.Cells[10, j + 8].Style.Font.Color.SetColor(Color.Black);

                                ws.Cells[9, j + 10].Style.Fill.BackgroundColor.SetColor(Dry);
                                ws.Cells[9, j + 10].Style.Font.Color.SetColor(Color.Black);
                                ws.Cells[10, j + 10].Style.Fill.BackgroundColor.SetColor(Dry);
                                ws.Cells[10, j + 10].Style.Font.Color.SetColor(Color.Black);

                                ws.Cells[9, j + 11].Style.Fill.BackgroundColor.SetColor(Chill);
                                ws.Cells[9, j + 11].Style.Font.Color.SetColor(Color.Black);
                                ws.Cells[10, j + 11].Style.Fill.BackgroundColor.SetColor(Chill);
                                ws.Cells[10, j + 11].Style.Font.Color.SetColor(Color.Black);

                                ws.Cells[9, j + 12].Style.Fill.BackgroundColor.SetColor(Frozen);
                                ws.Cells[9, j + 12].Style.Font.Color.SetColor(Color.Black);
                                ws.Cells[10, j + 12].Style.Fill.BackgroundColor.SetColor(Frozen);
                                ws.Cells[10, j + 12].Style.Font.Color.SetColor(Color.Black);

                                for (int p = 13; p < count1; p++)
                                {

                                    ws.Cells["A" + p].Value = ItemsList[L1].UNCode;
                                    ws.Cells["B" + p].Value = ItemsList[L1].Commodity;
                                    ws.Cells["C" + p].Value = ItemsList[L1].Temperature;
                                    ws.Cells[p, j + 5].Value = ItemsList[L1].Temperature;

                                    if (k1 < itemslst.Count())
                                    {
                                        if (ItemList[L1].UNCode == itemslst[k1].UNCode)
                                        {
                                            ws.Cells[p, j].Value = Math.Round(itemslst[k1].TotalOrdQty, 3);

                                            if (itemslst[k1].UNCode == 1129)
                                            {
                                                ws.Cells[6, j + 1].Value = Math.Round(itemslst[k1].TotalOrdQty, 3);
                                                ws.Cells[11, j].Value = Math.Round(itemslst.Sum(x => Convert.ToDecimal(x.TotalOrdQty)) - itemslst[k1].TotalOrdQty + itemslst[k1].TotalOrdQty * Convert.ToDecimal(0.058824), 3);
                                                ws.Cells[12, j].Value = Math.Round(itemslst.Sum(x => Convert.ToDecimal(x.TotalOrdQty)), 3);
                                                ws.Cells[8, j].Value = Math.Round((itemslst.Sum(x => Convert.ToDecimal(x.TotalOrdQty)) - itemslst[k1].TotalOrdQty) / (itemslst.FirstOrDefault().TotalTroops) / 26, 3);
                                            }
                                            if (ItemList[L1].Subs == "D")
                                            {
                                                ws.Cells[p, j + 2].Value = Math.Round(itemslst[k1].TotalOrdQty, 3);
                                                ws.Cells[p, j + 6].Value = Math.Round(itemslst[k1].TotalOrdQty / 4, 3);
                                                ws.Cells[p, j + 10].Value = Math.Round(itemslst[k1].TotalOrdQty / 26, 3);
                                            }
                                            else
                                            {
                                                ws.Cells[p, j + 2].Value = "-";
                                                ws.Cells[p, j + 6].Value = "0";
                                                ws.Cells[p, j + 10].Value = "0";
                                            }
                                            if (ItemList[L1].Subs == "C")
                                            {
                                                ws.Cells[p, j + 3].Value = Math.Round(itemslst[k1].TotalOrdQty, 3);
                                                ws.Cells[p, j + 7].Value = Math.Round(itemslst[k1].TotalOrdQty / 4, 3);
                                                ws.Cells[p, j + 11].Value = Math.Round(itemslst[k1].TotalOrdQty / 26, 3);
                                            }
                                            else
                                            {
                                                ws.Cells[p, j + 3].Value = "-";
                                                ws.Cells[p, j + 7].Value = "0";
                                                ws.Cells[p, j + 11].Value = "0";
                                            }
                                            if (ItemList[L1].Subs == "F")
                                            {
                                                ws.Cells[p, j + 4].Value = Math.Round(itemslst[k1].TotalOrdQty, 3);
                                                ws.Cells[p, j + 8].Value = Math.Round(itemslst[k1].TotalOrdQty / 4, 3);
                                                ws.Cells[p, j + 12].Value = Math.Round(itemslst[k1].TotalOrdQty / 26, 3);
                                            }
                                            else
                                            {
                                                ws.Cells[p, j + 4].Value = "-";
                                                ws.Cells[p, j + 8].Value = "0";
                                                ws.Cells[p, j + 12].Value = "0";
                                            }
                                            k1 = k1 + 1;
                                        }
                                        else
                                        {
                                            ws.Cells[p, j].Value = "0.000";
                                        }
                                    }
                                    else
                                    {
                                        ws.Cells[p, j].Value = "0.000";
                                    }

                                    ws.Row(p).Height = 21.20;
                                    L1 = L1 + 1;
                                }

                                int rowCount2 = ItemList.Count;
                                ws.Cells[12, j + 2].Formula = string.Format("Sum({0})", new ExcelAddress(13, j + 2, rowCount2 + 12, j + 2).Address);
                                ws.Cells[12, j + 2].Style.Font.Bold = true;
                                ws.Cells[12, j + 2].Style.Numberformat.Format = "#,##0.000";

                                ws.Cells[12, j + 3].Formula = string.Format("Sum({0})", new ExcelAddress(13, j + 3, rowCount2 + 12, j + 3).Address);
                                ws.Cells[12, j + 3].Style.Font.Bold = true;
                                ws.Cells[12, j + 3].Style.Numberformat.Format = "#,##0.000";

                                ws.Cells[11, j + 3].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(13, j + 3, rowCount2 + 12, j + 3).Address, new ExcelAddress(42, j + 3, 42, j + 3).Address, new ExcelAddress(42, j + 3, 42, j + 3).Address);
                                ws.Cells[11, j + 3].Style.Font.Bold = true;
                                ws.Cells[11, j + 3].Style.Numberformat.Format = "#,##0.000";

                                ws.Cells[12, j + 4].Formula = string.Format("Sum({0})", new ExcelAddress(13, j + 4, rowCount2 + 12, j + 4).Address);
                                ws.Cells[12, j + 4].Style.Font.Bold = true;
                                ws.Cells[12, j + 4].Style.Numberformat.Format = "#,##0.000";

                                ws.Cells[12, j + 6].Formula = string.Format("Sum({0})", new ExcelAddress(13, j + 6, rowCount2 + 12, j + 6).Address);
                                ws.Cells[12, j + 6].Style.Font.Bold = true;
                                ws.Cells[12, j + 6].Style.Numberformat.Format = "#,##0.000";

                                ws.Cells[12, j + 7].Formula = string.Format("Sum({0})", new ExcelAddress(13, j + 7, rowCount2 + 12, j + 7).Address);
                                ws.Cells[12, j + 7].Style.Font.Bold = true;
                                ws.Cells[12, j + 7].Style.Numberformat.Format = "#,##0.000";

                                ws.Cells[11, j + 7].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(13, j + 7, rowCount2 + 12, j + 7).Address, new ExcelAddress(42, j + 7, 42, j + 7).Address, new ExcelAddress(42, j + 7, 42, j + 7).Address);
                                ws.Cells[11, j + 7].Style.Font.Bold = true;
                                ws.Cells[11, j + 7].Style.Numberformat.Format = "#,##0.000";

                                ws.Cells[12, j + 8].Formula = string.Format("Sum({0})", new ExcelAddress(13, j + 8, rowCount2 + 12, j + 8).Address);
                                ws.Cells[12, j + 8].Style.Font.Bold = true;
                                ws.Cells[12, j + 8].Style.Numberformat.Format = "#,##0.000";

                                ws.Cells[12, j + 10].Formula = string.Format("Sum({0})", new ExcelAddress(13, j + 10, rowCount2 + 12, j + 10).Address);
                                ws.Cells[12, j + 10].Style.Font.Bold = true;
                                ws.Cells[12, j + 10].Style.Numberformat.Format = "#,##0.000";

                                ws.Cells[12, j + 11].Formula = string.Format("Sum({0})", new ExcelAddress(13, j + 11, rowCount2 + 12, j + 11).Address);
                                ws.Cells[12, j + 11].Style.Font.Bold = true;
                                ws.Cells[12, j + 11].Style.Numberformat.Format = "#,##0.000";

                                ws.Cells[11, j + 11].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(13, j + 11, rowCount2 + 12, j + 11).Address, new ExcelAddress(42, j + 11, 42, j + 11).Address, new ExcelAddress(42, j + 11, 42, j + 11).Address);
                                ws.Cells[11, j + 11].Style.Font.Bold = true;
                                ws.Cells[11, j + 11].Style.Numberformat.Format = "#,##0.000";

                                ws.Cells[12, j + 12].Formula = string.Format("Sum({0})", new ExcelAddress(13, j + 12, rowCount2 + 12, j + 12).Address);
                                ws.Cells[12, j + 12].Style.Font.Bold = true;
                                ws.Cells[12, j + 12].Style.Numberformat.Format = "#,##0.000";

                                o = o + 1;
                                #endregion
                            }
                        }
                    #endregion

                        #region Consolidate week sheet
                        //Added by kingst for Consolidate week report
                        if (Workbookset.Tables[i].TableName.ToString() == "Weekly Consolidated")
                        {

                            criteria.Clear();
                            criteria.Add("Period", Period);
                            criteria.Add("PeriodYear", PeriodYear);

                            //Dictionary<long, IList<ORDRPT_ConsolidatedWeekReport>> ItemList = OS.GetORDRPT_WeekConsolidateList(Period, PeriodYear, Weeks[0], Weeks[1], Weeks[2], Weeks[3]);
                            //IList<ORDRPT_ConsolidatedWeekReport> ConsolidateList = ItemList.FirstOrDefault().Value;
                            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Week Consolidate");

                            ws.View.ZoomScale = 70;
                            ws.View.ShowGridLines = false;


                            ws.Row(8).Height = 50.00;
                            // ws.Row(9).Height = 50.00;
                            ws.Column(1).Width = 6.29;
                            ws.Column(2).Width = 40.00;
                            ws.Column(3).Width = 9.00;
                            ws.Column(4).Width = 2.00;
                            ws.Column(5).Width = 12.43;
                            ws.Column(6).Width = 12.43;
                            ws.Column(7).Width = 12.43;
                            ws.Column(8).Width = 12.43;

                            ws.Column(9).Width = 2.00;
                            ws.Column(10).Width = 12.43;
                            ws.Column(11).Width = 12.43;
                            ws.Column(12).Width = 12.43;
                            ws.Column(13).Width = 12.43;

                            ws.Column(14).Width = 2.00;
                            ws.Column(15).Width = 12.43;
                            ws.Column(16).Width = 12.43;
                            ws.Column(17).Width = 12.43;
                            ws.Column(18).Width = 12.43;

                            ws.Column(19).Width = 2.00;
                            ws.Column(20).Width = 12.43;
                            ws.Column(21).Width = 12.43;
                            ws.Column(22).Width = 12.43;
                            ws.Column(23).Width = 12.43;

                            ws.Column(24).Width = 2.00;
                            ws.Column(25).Width = 12.43;
                            ws.Column(26).Width = 12.43;
                            ws.Column(27).Width = 12.43;
                            ws.Column(28).Width = 15.43;

                            //Font size
                            ws.Cells["A1:AZ"].Style.WrapText = true;
                            ws.Cells["A1:AZ"].Style.Font.Name = "Calibri";
                            ws.Cells["A1:AZ"].Style.Font.Size = 12;

                            Color GREYCOLOR = System.Drawing.ColorTranslator.FromHtml("#b6b6b6");
                            ws.Cells["A8:c9,E8:H9,J8:M9,O8:R9,T8:W9,Y8:AB9"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["A8:c9,E8:H9,J8:M9,O8:R9,T8:W9,Y8:AB9"].Style.Fill.BackgroundColor.SetColor(GREYCOLOR);



                            Color BLUECOLOR = System.Drawing.ColorTranslator.FromHtml("#007000");
                            ws.Cells["E3:G4,J3:L4,O3:Q4,O3:R4,T3:V4,Y3:AA4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["E3:G4,J3:L4,O3:Q4,O3:R4,T3:V4,Y3:AA4"].Style.Fill.BackgroundColor.SetColor(BLUECOLOR);


                            Color VILOTCOLOR = System.Drawing.ColorTranslator.FromHtml("#60497A");
                            ws.Cells["R3:R4,W3:W4,H3:H4,M3:M4,AB3:AB4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["R3:R4,W3:W4,H3:H4,M3:M4,AB3:AB4"].Style.Fill.BackgroundColor.SetColor(VILOTCOLOR);

                            Color YELLOWCOLOR = System.Drawing.ColorTranslator.FromHtml("#FFFF00");
                            ws.Cells["P5:R5,T5:W5,O5:R5,E5:H5,J5:M5,Y5:AB5"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["P5:R5,T5:W5,O5:R5,E5:H5,J5:M5,Y5:AB5"].Style.Fill.BackgroundColor.SetColor(YELLOWCOLOR);



                            Color REDCOLOR = System.Drawing.ColorTranslator.FromHtml("#C00000");
                            ws.Cells["P6:R6,T6:W6,O6:R6,E6:H6,J6:M6,Y6:AB6"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["P6:R6,T6:W6,O6:R6,E6:H6,J6:M6,Y6:AB6"].Style.Fill.BackgroundColor.SetColor(REDCOLOR);


                            Color BROWN = System.Drawing.ColorTranslator.FromHtml("#948A54");
                            ws.Cells["P1:R1,T1:W1,O1:R1,E1:H1,J1:M1,Y1:AB1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["P1:R1,T1:W1,O1:R1,E1:H1,J1:M1,Y1:AB1"].Style.Fill.BackgroundColor.SetColor(BROWN);


                            ws.Cells["A9,B9,C9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; //Center Alignments for Cells

                            ws.Cells["B1"].Value = "Final Requisition - Consumption Date";
                            ws.Cells["B2"].Value = "Week";
                            ws.Cells["B3"].Value = "HQ location:";
                            ws.Cells["B4"].Value = "WH location:";
                            ws.Cells["B5"].Value = "Number of Contingent";
                            ws.Cells["B6"].Value = "Troopstrength:";

                            //Header
                            ws.Cells["A8"].Value = "UNCode";
                            ws.Cells["B8"].Value = "COMMODITY";
                            ws.Cells["C9"].Value = "Temperature";

                            ws.Cells["E8,F8,G8,H8,J8,K8,L8,M8,O8,P8,Q8,R8,T8,U8,V8,W8"].Value = "Quantity Ordered";


                            ws.Cells["Y8"].Value = "Total Quantity Ordered";
                            ws.Cells["Z8"].Value = "Total Quantity Ordered";
                            ws.Cells["AA8"].Value = "Total Quantity Ordered";
                            ws.Cells["AB8"].Value = "Quantity Ordered";


                            ws.Cells["E9,F9,G9,H9,J9,K9,L9,M9,O9,P9,Q9,R9,T9,U9,V9,W9,Y9,Z9,AA9,AB9"].Value = "(Kg/Lt/EA)";

                            int count = ConsolidateList.Count() + 12;
                            int k = 0; // Sno count
                            int L = 0; // Increament for every line
                            ws.Cells["E1:AB" + count].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["E1:AB" + count].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                            ws.Cells["A8:c" + count].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            ws.Cells["A8:c" + count].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            ws.Cells["A8:c" + count].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            ws.Cells["A8:c" + count].Style.Border.Right.Style = ExcelBorderStyle.Thin;


                            ws.Cells["E8:H" + count].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            ws.Cells["E8:H" + count].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            ws.Cells["E8:H" + count].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            ws.Cells["E8:H" + count].Style.Border.Right.Style = ExcelBorderStyle.Thin;


                            ws.Cells["J8:M" + count].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            ws.Cells["J8:M" + count].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            ws.Cells["J8:M" + count].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            ws.Cells["J8:M" + count].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            ws.Cells["O8:R" + count].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            ws.Cells["O8:R" + count].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            ws.Cells["O8:R" + count].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            ws.Cells["O8:R" + count].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            ws.Cells["T8:W" + count].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            ws.Cells["T8:W" + count].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            ws.Cells["T8:W" + count].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            ws.Cells["T8:W" + count].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            ws.Cells["Y8:AB" + count].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            ws.Cells["Y8:AB" + count].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            ws.Cells["Y8:AB" + count].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            ws.Cells["Y:AB" + count].Style.Border.Right.Style = ExcelBorderStyle.Thin;


                            //Headers alignment

                            ws.Cells["E1:h6"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            ws.Cells["E1:h6"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            ws.Cells["E1:h6"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            ws.Cells["E1:h6"].Style.Border.Right.Style = ExcelBorderStyle.Thin;



                            ws.Cells["J1:M6"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            ws.Cells["J1:M6"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            ws.Cells["J1:M6"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            ws.Cells["J1:M6"].Style.Border.Right.Style = ExcelBorderStyle.Thin;


                            ws.Cells["O1:R6"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            ws.Cells["O1:R6"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            ws.Cells["O1:R6"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            ws.Cells["O1:R6"].Style.Border.Right.Style = ExcelBorderStyle.Thin;


                            ws.Cells["T1:W6"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            ws.Cells["T1:W6"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            ws.Cells["T1:W6"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            ws.Cells["T1:W6"].Style.Border.Right.Style = ExcelBorderStyle.Thin;


                            ws.Cells["Y1:AB6"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            ws.Cells["Y1:AB6"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            ws.Cells["Y1:AB6"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            ws.Cells["Y1:AB6"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            //Font color
                            ws.Cells["E1:W1,E3:AK4,E6:AK6,A10:AB10"].Style.Font.Color.SetColor(Color.White);
                            ws.Cells["A10:AB10"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["A10:AB10"].Style.Fill.BackgroundColor.SetColor(Color.Black);


                            //Binding the headers
                            ws.Cells["F1"].Value = "TO";
                            ws.Cells["A1"].Value = Period;
                            ws.Cells["A2"].Value = Weeks[0];


                            criteria.Clear();
                            criteria.Add("Period", Period);
                            criteria.Add("PeriodYear", PeriodYear);
                            Dictionary<long, IList<ORDRPT_ConsolidateWeekReport_vw>> conWeekRpt = OS.GetORDRPT_ConsolidateWeekReport_vwListWithPagingAndCriteria(0, 999999, string.Empty, string.Empty, criteria);
                            IList<ORDRPT_ConsolidateWeekReport_vw> Headerlist = conWeekRpt.FirstOrDefault().Value;

                            foreach (var item in Headerlist)
                            {
                                if (item.Week == Weeks[0])
                                {
                                    ws.Cells["E1"].Value = String.Format("{0:dd-MMM-yy}", item.StartDate);
                                    ws.Cells["G1"].Value = String.Format("{0:dd-MMM-yy}", item.EndDate);
                                    ws.Cells["G2"].Value = "Week-" + item.Week;
                                    ws.Cells["H3"].Value = "Week-" + item.Week;
                                    ws.Cells["H4"].Value = "TOTAL";



                                    if (item.Sector == "NS")
                                    {
                                        ws.Cells["E3"].Value = item.Sector;
                                        ws.Cells["E4"].Value = item.Warehouse;
                                        ws.Cells["E5"].Value = item.Contingents;
                                        ws.Cells["E6"].Value = item.TroopStrength;
                                    }
                                    if (item.Sector == "FS")
                                    {
                                        ws.Cells["F3"].Value = item.Sector;
                                        ws.Cells["F4"].Value = item.Warehouse;
                                        ws.Cells["F5"].Value = item.Contingents;
                                        ws.Cells["F6"].Value = item.TroopStrength;
                                    }
                                    if (item.Sector == "GS")
                                    {
                                        ws.Cells["G3"].Value = item.Sector;
                                        ws.Cells["G4"].Value = item.Warehouse;
                                        ws.Cells["G5"].Value = item.Contingents;
                                        ws.Cells["G6"].Value = item.TroopStrength;
                                    }

                                }

                                //Week 2
                                if (item.Week == Weeks[1])
                                {
                                    ws.Cells["J1"].Value = String.Format("{0:dd-MMM-yy}", item.StartDate);
                                    ws.Cells["L1"].Value = String.Format("{0:dd-MMM-yy}", item.EndDate);
                                    ws.Cells["K2"].Value = "Week-" + item.Week;
                                    ws.Cells["M3"].Value = "Week-" + item.Week;
                                    ws.Cells["M4"].Value = "TOTAL";



                                    if (item.Sector == "NS")
                                    {
                                        ws.Cells["J3"].Value = item.Sector;
                                        ws.Cells["J4"].Value = item.Warehouse;
                                        ws.Cells["J5"].Value = item.Contingents;
                                        ws.Cells["J6"].Value = item.TroopStrength;
                                    }
                                    if (item.Sector == "FS")
                                    {
                                        ws.Cells["K3"].Value = item.Sector;
                                        ws.Cells["K4"].Value = item.Warehouse;
                                        ws.Cells["K5"].Value = item.Contingents;
                                        ws.Cells["K6"].Value = item.TroopStrength;
                                    }
                                    if (item.Sector == "GS")
                                    {
                                        ws.Cells["L3"].Value = item.Sector;
                                        ws.Cells["L4"].Value = item.Warehouse;
                                        ws.Cells["L5"].Value = item.Contingents;
                                        ws.Cells["L6"].Value = item.TroopStrength;
                                    }

                                }

                                //Week 3

                                if (item.Week == Weeks[2])
                                {
                                    ws.Cells["O1"].Value = String.Format("{0:dd-MMM-yy}", item.StartDate);
                                    ws.Cells["Q1"].Value = String.Format("{0:dd-MMM-yy}", item.EndDate);
                                    ws.Cells["P2"].Value = "Week-" + item.Week;
                                    ws.Cells["R3"].Value = "Week-" + item.Week;
                                    ws.Cells["R4"].Value = "TOTAL";



                                    if (item.Sector == "NS")
                                    {
                                        ws.Cells["O3"].Value = item.Sector;
                                        ws.Cells["O4"].Value = item.Warehouse;
                                        ws.Cells["O5"].Value = item.Contingents;
                                        ws.Cells["O6"].Value = item.TroopStrength;
                                    }
                                    if (item.Sector == "FS")
                                    {
                                        ws.Cells["P3"].Value = item.Sector;
                                        ws.Cells["P4"].Value = item.Warehouse;
                                        ws.Cells["P5"].Value = item.Contingents;
                                        ws.Cells["P6"].Value = item.TroopStrength;
                                    }
                                    if (item.Sector == "GS")
                                    {
                                        ws.Cells["Q3"].Value = item.Sector;
                                        ws.Cells["Q4"].Value = item.Warehouse;
                                        ws.Cells["Q5"].Value = item.Contingents;
                                        ws.Cells["Q6"].Value = item.TroopStrength;
                                    }

                                }


                                //Week 4
                                if (item.Week == Weeks[3])
                                {
                                    ws.Cells["T1"].Value = String.Format("{0:dd-MMM-yy}", item.StartDate);
                                    ws.Cells["V1"].Value = String.Format("{0:dd-MMM-yy}", item.EndDate);
                                    ws.Cells["U2"].Value = "Week-" + item.Week;
                                    ws.Cells["W3"].Value = "Week-" + item.Week;
                                    ws.Cells["W4"].Value = "TOTAL";



                                    if (item.Sector == "NS")
                                    {
                                        ws.Cells["T3"].Value = item.Sector;
                                        ws.Cells["T4"].Value = item.Warehouse;
                                        ws.Cells["T5"].Value = item.Contingents;
                                        ws.Cells["T6"].Value = item.TroopStrength;
                                    }
                                    if (item.Sector == "FS")
                                    {
                                        ws.Cells["U3"].Value = item.Sector;
                                        ws.Cells["U4"].Value = item.Warehouse;
                                        ws.Cells["U5"].Value = item.Contingents;
                                        ws.Cells["U6"].Value = item.TroopStrength;
                                    }
                                    if (item.Sector == "GS")
                                    {
                                        ws.Cells["V3"].Value = item.Sector;
                                        ws.Cells["V4"].Value = item.Warehouse;
                                        ws.Cells["V5"].Value = item.Contingents;
                                        ws.Cells["V6"].Value = item.TroopStrength;
                                    }

                                }


                            }

                            ws.Cells["H5"].Value = Convert.ToInt64(ws.Cells["E5"].Value) + Convert.ToInt64(ws.Cells["F5"].Value) + Convert.ToInt64(ws.Cells["G5"].Value);
                            ws.Cells["H6"].Value = Convert.ToInt64(ws.Cells["E6"].Value) + Convert.ToInt64(ws.Cells["F6"].Value) + Convert.ToInt64(ws.Cells["G6"].Value);

                            ws.Cells["M5"].Value = Convert.ToInt64(ws.Cells["J5"].Value) + Convert.ToInt64(ws.Cells["K5"].Value) + Convert.ToInt64(ws.Cells["L5"].Value);
                            ws.Cells["M6"].Value = Convert.ToInt64(ws.Cells["J6"].Value) + Convert.ToInt64(ws.Cells["K6"].Value) + Convert.ToInt64(ws.Cells["L6"].Value);

                            ws.Cells["R5"].Value = Convert.ToInt64(ws.Cells["O5"].Value) + Convert.ToInt64(ws.Cells["P5"].Value) + Convert.ToInt64(ws.Cells["Q5"].Value);
                            ws.Cells["R6"].Value = Convert.ToInt64(ws.Cells["O6"].Value) + Convert.ToInt64(ws.Cells["P6"].Value) + Convert.ToInt64(ws.Cells["Q6"].Value);

                            ws.Cells["W5"].Value = Convert.ToInt64(ws.Cells["T5"].Value) + Convert.ToInt64(ws.Cells["U5"].Value) + Convert.ToInt64(ws.Cells["V5"].Value);
                            ws.Cells["W6"].Value = Convert.ToInt64(ws.Cells["T6"].Value) + Convert.ToInt64(ws.Cells["U6"].Value) + Convert.ToInt64(ws.Cells["V6"].Value);

                            ws.Cells["Y5"].Value = (Convert.ToInt64(ws.Cells["E5"].Value) + Convert.ToInt64(ws.Cells["J5"].Value) + Convert.ToInt64(ws.Cells["O5"].Value) + Convert.ToInt64(ws.Cells["T5"].Value)) / 4;
                            ws.Cells["Z5"].Value = (Convert.ToInt64(ws.Cells["F5"].Value) + Convert.ToInt64(ws.Cells["K5"].Value) + Convert.ToInt64(ws.Cells["P5"].Value) + Convert.ToInt64(ws.Cells["U5"].Value)) / 4;
                            ws.Cells["AA5"].Value = (Convert.ToInt64(ws.Cells["G5"].Value) + Convert.ToInt64(ws.Cells["L5"].Value) + Convert.ToInt64(ws.Cells["Q5"].Value) + Convert.ToInt64(ws.Cells["V5"].Value)) / 4;
                            ws.Cells["AB5"].Value = Convert.ToInt64(ws.Cells["Y5"].Value) + Convert.ToInt64(ws.Cells["Z5"].Value) + Convert.ToInt64(ws.Cells["AA5"].Value);

                            ws.Cells["Y6"].Value = (Convert.ToInt64(ws.Cells["E6"].Value) + Convert.ToInt64(ws.Cells["J6"].Value) + Convert.ToInt64(ws.Cells["O6"].Value) + Convert.ToInt64(ws.Cells["T6"].Value)) / 4;
                            ws.Cells["Z6"].Value = (Convert.ToInt64(ws.Cells["F6"].Value) + Convert.ToInt64(ws.Cells["K6"].Value) + Convert.ToInt64(ws.Cells["P6"].Value) + Convert.ToInt64(ws.Cells["U6"].Value)) / 4;
                            ws.Cells["AA6"].Value = (Convert.ToInt64(ws.Cells["G6"].Value) + Convert.ToInt64(ws.Cells["L6"].Value) + Convert.ToInt64(ws.Cells["Q6"].Value) + Convert.ToInt64(ws.Cells["V6"].Value)) / 4;
                            ws.Cells["AB6"].Value = Convert.ToInt64(ws.Cells["Y6"].Value) + Convert.ToInt64(ws.Cells["Z6"].Value) + Convert.ToInt64(ws.Cells["AA6"].Value);
                            ws.Cells["Y1"].Value = ws.Cells["E1"].Value;
                            ws.Cells["AA1"].Value = ws.Cells["V1"].Value;
                            ws.Cells["Z1"].Value = "TO";
                            ws.Cells["Y3"].Value = ws.Cells["T3"].Value;
                            ws.Cells["Z3"].Value = ws.Cells["U3"].Value;
                            ws.Cells["AA3"].Value = ws.Cells["V3"].Value;
                            ws.Cells["Y4"].Value = ws.Cells["T4"].Value;
                            ws.Cells["Z4"].Value = ws.Cells["U4"].Value;
                            ws.Cells["AA4"].Value = ws.Cells["V4"].Value;
                            ws.Cells["AB3"].Value = Period;
                            ws.Cells["AB4"].Value = "TOTAL";
                            ws.Cells["Y2"].Value = Period;
                            ws.Cells["Z2"].Value = "Total Sector Wise";



                            for (int p = 12; p < count; p++)
                            {


                                ws.Cells["A" + p].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                ws.Cells["A" + p].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                // ws.Cells["B" + p].Value = ItemsList[L].Commodity;
                                ws.Cells["B" + p].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                ws.Cells["B" + p].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells["A" + p].Value = ConsolidateList[k].UNCode;
                                ws.Cells["B" + p].Value = ConsolidateList[k].Commodity;
                                ws.Cells["C" + p].Value = ConsolidateList[k].Temperature;

                                ws.Cells["E" + p].Value = Math.Round(ConsolidateList[k].OrderQty_SS1, 3);
                                ws.Cells["F" + p].Value = Math.Round(ConsolidateList[k].OrderQty_SN1, 3);
                                ws.Cells["G" + p].Value = Math.Round(ConsolidateList[k].OrderQty_SW1, 3);
                                ws.Cells["H" + p].Value = Math.Round(ConsolidateList[k].TotalOrdQtyWK1, 3);

                                ws.Cells["J" + p].Value = Math.Round(ConsolidateList[k].OrderQty_SS2, 3);
                                ws.Cells["K" + p].Value = Math.Round(ConsolidateList[k].OrderQty_SN2, 3);
                                ws.Cells["L" + p].Value = Math.Round(ConsolidateList[k].OrderQty_SW2, 3);
                                ws.Cells["M" + p].Value = Math.Round(ConsolidateList[k].TotalOrdQtyWK2, 3);

                                ws.Cells["O" + p].Value = Math.Round(ConsolidateList[k].OrderQty_SS3, 3);
                                ws.Cells["p" + p].Value = Math.Round(ConsolidateList[k].OrderQty_SN3, 3);
                                ws.Cells["Q" + p].Value = Math.Round(ConsolidateList[k].OrderQty_SW3, 3);
                                ws.Cells["R" + p].Value = Math.Round(ConsolidateList[k].TotalOrdQtyWK3, 3);

                                ws.Cells["T" + p].Value = Math.Round(ConsolidateList[k].OrderQty_SS4, 3);
                                ws.Cells["U" + p].Value = Math.Round(ConsolidateList[k].OrderQty_SN4, 3);
                                ws.Cells["V" + p].Value = Math.Round(ConsolidateList[k].OrderQty_SW4, 3);
                                ws.Cells["W" + p].Value = Math.Round(ConsolidateList[k].TotalOrdQtyWK4, 3);

                                ws.Cells["Y" + p].Value = Math.Round(ConsolidateList[k].TotalOrdQtySS, 3);
                                ws.Cells["Z" + p].Value = Math.Round(ConsolidateList[k].TotalOrdQtySN, 3);
                                ws.Cells["AA" + p].Value = Math.Round(ConsolidateList[k].TotalOrdQtySW, 3);
                                ws.Cells["AB" + p].Value = Math.Round(ConsolidateList[k].OrdQty, 3);


                                ws.Row(p).Height = 21.20;
                                k = k + 1;
                                L = L + 1;
                            }

                            //  ws.Cells["E10:AZ"].Value.ToString().Replace("0", "-");

                            //o = o + 1;

                            ws.Cells[11, 5].Formula = string.Format("Sum({0})", new ExcelAddress(12, 5, count, 5).Address);
                            ws.Cells[11, 5].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[10, 5].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(12, 5, count, 5).Address, new ExcelAddress(41, 5, 41, 5).Address, new ExcelAddress(41, 5, 41, 5).Address);
                            ws.Cells[10, 5].Style.Font.Bold = true;
                            ws.Cells[10, 5].Style.Numberformat.Format = "#,##0.00";


                            ws.Cells[11, 6].Formula = string.Format("Sum({0})", new ExcelAddress(12, 6, count, 6).Address);
                            ws.Cells[11, 6].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[10, 6].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(12, 6, count, 6).Address, new ExcelAddress(41, 6, 41, 6).Address, new ExcelAddress(41, 6, 41, 6).Address);
                            ws.Cells[10, 6].Style.Font.Bold = true;
                            ws.Cells[10, 6].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[11, 7].Formula = string.Format("Sum({0})", new ExcelAddress(12, 7, count, 7).Address);
                            ws.Cells[11, 7].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[10, 7].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(12, 7, count, 7).Address, new ExcelAddress(41, 7, 41, 7).Address, new ExcelAddress(41, 7, 41, 7).Address);
                            ws.Cells[10, 7].Style.Font.Bold = true;
                            ws.Cells[10, 7].Style.Numberformat.Format = "#,##0.00";

                            ws.Cells[11, 8].Formula = string.Format("Sum({0})", new ExcelAddress(12, 8, count, 8).Address);
                            ws.Cells[11, 8].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[10, 8].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(12, 8, count, 8).Address, new ExcelAddress(41, 8, 41, 8).Address, new ExcelAddress(41, 8, 41, 8).Address);
                            ws.Cells[10, 8].Style.Font.Bold = true;
                            ws.Cells[10, 8].Style.Numberformat.Format = "#,##0.000";


                            ws.Cells[11, 10].Formula = string.Format("Sum({0})", new ExcelAddress(12, 10, count, 10).Address);
                            ws.Cells[11, 10].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[10, 10].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(12, 10, count, 10).Address, new ExcelAddress(41, 10, 41, 10).Address, new ExcelAddress(41, 10, 41, 10).Address);
                            ws.Cells[10, 10].Style.Font.Bold = true;
                            ws.Cells[10, 10].Style.Numberformat.Format = "#,##0.000";


                            ws.Cells[11, 11].Formula = string.Format("Sum({0})", new ExcelAddress(12, 11, count, 11).Address);
                            ws.Cells[11, 11].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[10, 11].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(12, 11, count, 11).Address, new ExcelAddress(41, 11, 41, 11).Address, new ExcelAddress(41, 11, 41, 11).Address);
                            ws.Cells[10, 11].Style.Font.Bold = true;
                            ws.Cells[10, 11].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[11, 12].Formula = string.Format("Sum({0})", new ExcelAddress(12, 12, count, 12).Address);
                            ws.Cells[11, 12].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[10, 12].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(12, 12, count, 12).Address, new ExcelAddress(41, 12, 41, 12).Address, new ExcelAddress(41, 12, 41, 12).Address);
                            ws.Cells[10, 12].Style.Font.Bold = true;
                            ws.Cells[10, 12].Style.Numberformat.Format = "#,##0.000";


                            ws.Cells[11, 13].Formula = string.Format("Sum({0})", new ExcelAddress(12, 13, count, 13).Address);
                            ws.Cells[11, 13].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[10, 13].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(12, 13, count, 13).Address, new ExcelAddress(41, 13, 41, 13).Address, new ExcelAddress(41, 13, 41, 13).Address);
                            ws.Cells[10, 13].Style.Font.Bold = true;
                            ws.Cells[10, 13].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[11, 15].Formula = string.Format("Sum({0})", new ExcelAddress(12, 15, count, 15).Address);
                            ws.Cells[11, 15].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[10, 15].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(12, 15, count, 15).Address, new ExcelAddress(41, 15, 41, 15).Address, new ExcelAddress(41, 15, 41, 15).Address);
                            ws.Cells[10, 15].Style.Font.Bold = true;
                            ws.Cells[10, 15].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[11, 16].Formula = string.Format("Sum({0})", new ExcelAddress(12, 16, count, 16).Address);
                            ws.Cells[11, 16].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[10, 16].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(12, 16, count, 16).Address, new ExcelAddress(41, 16, 41, 16).Address, new ExcelAddress(41, 16, 41, 16).Address);
                            ws.Cells[10, 16].Style.Font.Bold = true;
                            ws.Cells[10, 16].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[11, 17].Formula = string.Format("Sum({0})", new ExcelAddress(12, 17, count, 17).Address);
                            ws.Cells[11, 17].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[10, 17].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(12, 17, count, 17).Address, new ExcelAddress(41, 17, 41, 17).Address, new ExcelAddress(41, 17, 41, 17).Address);
                            ws.Cells[10, 17].Style.Font.Bold = true;
                            ws.Cells[10, 17].Style.Numberformat.Format = "#,##0.000";


                            ws.Cells[11, 18].Formula = string.Format("Sum({0})", new ExcelAddress(12, 18, count, 18).Address);
                            ws.Cells[11, 18].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[10, 18].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(12, 18, count, 18).Address, new ExcelAddress(41, 18, 41, 18).Address, new ExcelAddress(41, 18, 41, 18).Address);
                            ws.Cells[10, 18].Style.Font.Bold = true;
                            ws.Cells[10, 18].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[11, 20].Formula = string.Format("Sum({0})", new ExcelAddress(12, 20, count, 20).Address);
                            ws.Cells[11, 20].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[10, 20].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(12, 20, count, 20).Address, new ExcelAddress(41, 20, 41, 20).Address, new ExcelAddress(41, 20, 41, 20).Address);
                            ws.Cells[10, 20].Style.Font.Bold = true;
                            ws.Cells[10, 20].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[11, 21].Formula = string.Format("Sum({0})", new ExcelAddress(12, 21, count, 21).Address);
                            ws.Cells[11, 21].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[10, 21].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(12, 21, count, 21).Address, new ExcelAddress(41, 21, 41, 21).Address, new ExcelAddress(41, 21, 41, 21).Address);
                            ws.Cells[10, 21].Style.Font.Bold = true;
                            ws.Cells[10, 21].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[11, 22].Formula = string.Format("Sum({0})", new ExcelAddress(12, 22, count, 22).Address);
                            ws.Cells[11, 22].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[10, 22].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(12, 22, count, 22).Address, new ExcelAddress(41, 22, 41, 22).Address, new ExcelAddress(41, 22, 41, 22).Address);
                            ws.Cells[10, 22].Style.Font.Bold = true;
                            ws.Cells[10, 22].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[11, 23].Formula = string.Format("Sum({0})", new ExcelAddress(12, 23, count, 23).Address);
                            ws.Cells[11, 23].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[10, 23].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(12, 23, count, 23).Address, new ExcelAddress(41, 23, 41, 23).Address, new ExcelAddress(41, 23, 41, 23).Address);
                            ws.Cells[10, 23].Style.Font.Bold = true;
                            ws.Cells[10, 23].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[11, 25].Formula = string.Format("Sum({0})", new ExcelAddress(12, 25, count, 25).Address);
                            ws.Cells[11, 25].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[10, 25].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(12, 25, count, 25).Address, new ExcelAddress(41, 25, 41, 25).Address, new ExcelAddress(41, 25, 41, 25).Address);
                            ws.Cells[10, 25].Style.Font.Bold = true;
                            ws.Cells[10, 25].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[11, 26].Formula = string.Format("Sum({0})", new ExcelAddress(12, 26, count, 26).Address);
                            ws.Cells[11, 26].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[10, 26].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(12, 26, count, 26).Address, new ExcelAddress(41, 26, 41, 26).Address, new ExcelAddress(41, 26, 41, 26).Address);
                            ws.Cells[10, 26].Style.Font.Bold = true;
                            ws.Cells[10, 26].Style.Numberformat.Format = "#,##0.00";

                            ws.Cells[11, 27].Formula = string.Format("Sum({0})", new ExcelAddress(12, 27, count, 27).Address);
                            ws.Cells[11, 27].Style.Numberformat.Format = "#,##0.00";

                            ws.Cells[10, 27].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(12, 27, count, 27).Address, new ExcelAddress(41, 27, 41, 27).Address, new ExcelAddress(41, 27, 41, 27).Address);
                            ws.Cells[10, 27].Style.Font.Bold = true;
                            ws.Cells[10, 27].Style.Numberformat.Format = "#,##0.000";


                            ws.Cells[11, 28].Formula = string.Format("Sum({0})", new ExcelAddress(12, 28, count, 28).Address);
                            ws.Cells[11, 28].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[10, 28].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(12, 28, count, 28).Address, new ExcelAddress(41, 28, 41, 28).Address, new ExcelAddress(41, 28, 41, 28).Address);
                            ws.Cells[10, 28].Style.Font.Bold = true;
                            ws.Cells[10, 28].Style.Numberformat.Format = "#,##0.000";

                        }
                        #endregion

                        #region Bulk order report
                        if (Workbookset.Tables[i].TableName.ToString() == "Bulk Food Order " + Period)
                        {
                            Dictionary<long, IList<ORDRPT_BulkFoodOrder_SP>> ItemList = OS.GetORDRPT_BulkFoodOrderList(Period, PeriodYear);
                            IList<ORDRPT_BulkFoodOrder_SP> Bulklist = ItemList.FirstOrDefault().Value;

                            var Troops = (from u in Bulklist where u.Troops_SN != 0 && u.Troops_SS != 0 && u.Troops_SW != 0 select new { u.Troops_SS, u.Troops_SN, u.Troops_SW }).Distinct().ToList();
                            int count = Bulklist.Count + 13;
                            ExcelWorksheet ws = pck.Workbook.Worksheets.Add(Workbookset.Tables[i].TableName.ToString());
                            ws.View.ZoomScale = 70;
                            ws.View.ShowGridLines = false;
                            ws.Column(1).Width = 8;
                            ws.Column(2).Width = 60.00;
                            ws.Column(3).Width = 15.00;
                            ws.Column(4).Width = 15.00;
                            ws.Column(5).Width = 15.00;
                            ws.Column(6).Width = 2.00;
                            ws.Column(7).Width = 15.00;


                            ws.Row(9).Height = 51.00;
                            ws.Cells["A1:AZ"].Style.WrapText = true;
                            ws.Cells["A1:AZ"].Style.Font.Name = "Calibri";
                            ws.Cells["A1:AZ"].Style.Font.Size = 12;


                            ws.Cells["B4"].Value = "HQ location:";
                            ws.Cells["B5"].Value = "location:";
                            ws.Cells["B6"].Value = "Unit Name:";
                            ws.Cells["B7"].Value = "Troop Strength";
                            ws.Cells["B8"].Value = "Per man per day";
                            ws.Cells["A9"].Value = "UN Code";
                            ws.Cells["B9"].Value = "Item";
                            ws.Cells["C9,D9,E9"].Value = "Total Quantity Ordered";
                            ws.Cells["G9"].Value = Period + "/" + PeriodYear + " Total Quantity Ordered";
                            ws.Cells["C10,D10,E10,G10"].Value = "(Kg/Lt/EA)";

                            ws.Cells["C4"].Value = "SS";
                            ws.Cells["C5"].Value = "NYL";
                            ws.Cells["C6"].Value = "SS-NSC";
                            ws.Cells["D4"].Value = "SN";
                            ws.Cells["D5"].Value = "FAS";
                            ws.Cells["D6"].Value = "SN-FSC";
                            ws.Cells["E4"].Value = "SW";
                            ws.Cells["E5"].Value = "GEN";
                            ws.Cells["E6"].Value = "SW-GSC";
                            //border
                            ws.Cells["A9:E" + count].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            ws.Cells["A9:E" + count].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            ws.Cells["A9:E" + count].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            ws.Cells["A9:E" + count].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            ws.Cells["G9:G" + count + ",C4:E7"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            ws.Cells["G9:G" + count + ",C4:E7"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            ws.Cells["G9:G" + count + ",C4:E7"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            ws.Cells["G9:G" + count + ",C4:E7"].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            Color BLUECOLOR = System.Drawing.ColorTranslator.FromHtml("#007000");
                            ws.Cells["A9:G10"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["A9:G10"].Style.Fill.BackgroundColor.SetColor(Color.Blue);
                            ws.Cells["C7,D7,E7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                            ws.Cells["A11:G11"].Style.Font.Color.SetColor(Color.White);
                            ws.Cells["A11:G11"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["A11:G11"].Style.Fill.BackgroundColor.SetColor(Color.Black);

                            if (Troops.Count > 0)
                            {
                                ws.Cells["C7"].Value = Troops[0].Troops_SS;
                                ws.Cells["D7"].Value = Troops[0].Troops_SN;
                                ws.Cells["E7"].Value = Troops[0].Troops_SW;
                                ws.Cells["G7"].Value = Troops[0].Troops_SS + Troops[0].Troops_SN + Troops[0].Troops_SW;
                            }
                            else
                            {
                                ws.Cells["C7"].Value = "";
                                ws.Cells["D7"].Value = "";
                                ws.Cells["E7"].Value = "";
                                ws.Cells["G7"].Value = "";
                            }

                            int k = 0;
                            for (int row = 13; row < count; row++)
                            {
                                ws.Cells["A" + row].Value = Bulklist[k].UNCode;
                                ws.Cells["B" + row].Value = Bulklist[k].Commodity;
                                ws.Cells["C" + row].Value = Math.Round(Bulklist[k].OrdQty_SS, 3);
                                ws.Cells["D" + row].Value = Math.Round(Bulklist[k].OrdQty_SN, 3);

                                ws.Cells["E" + row].Value = Math.Round(Bulklist[k].OrdQty_SW, 3);
                                ws.Cells["G" + row].Value = Math.Round(Bulklist[k].TotalOrdQty, 3);
                                k = k + 1;
                            }


                            ws.Cells[12, 3].Formula = string.Format("Sum({0})", new ExcelAddress(13, 3, count, 3).Address);
                            ws.Cells[12, 3].Style.Numberformat.Format = "#,##0.000";


                            ws.Cells[12, 4].Formula = string.Format("Sum({0})", new ExcelAddress(13, 4, count, 4).Address);
                            ws.Cells[12, 4].Style.Numberformat.Format = "#,##0.000";


                            ws.Cells[12, 5].Formula = string.Format("Sum({0})", new ExcelAddress(13, 5, count, 5).Address);
                            ws.Cells[12, 5].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[12, 7].Formula = string.Format("Sum({0})", new ExcelAddress(13, 7, count, 7).Address);
                            ws.Cells[12, 7].Style.Numberformat.Format = "#,##0.000";



                            ws.Cells[11, 3].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(13, 3, count, 3).Address, new ExcelAddress(42, 3, 42, 3).Address, new ExcelAddress(42, 3, 42, 3).Address);
                            ws.Cells[11, 3].Style.Font.Bold = true;
                            ws.Cells[11, 3].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[11, 4].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(13, 4, count, 4).Address, new ExcelAddress(42, 4, 42, 4).Address, new ExcelAddress(42, 4, 42, 4).Address);
                            ws.Cells[11, 4].Style.Font.Bold = true;
                            ws.Cells[11, 4].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[11, 5].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(13, 5, count, 5).Address, new ExcelAddress(42, 5, 42, 5).Address, new ExcelAddress(42, 5, 42, 5).Address);
                            ws.Cells[11, 5].Style.Font.Bold = true;
                            ws.Cells[11, 5].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[11, 7].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(13, 7, count, 7).Address, new ExcelAddress(42, 7, 42, 7).Address, new ExcelAddress(42, 7, 42, 7).Address);
                            ws.Cells[11, 7].Style.Font.Bold = true;
                            ws.Cells[11, 7].Style.Numberformat.Format = "#,##0.000";

                        }
                        #endregion

                        #region Analysis report

                        if (Workbookset.Tables[i].TableName.ToString() == "Analysis")
                        {
                            Dictionary<long, IList<ORDRPT_AnalysisSP>> ItemList = OS.GetORDRPT_AnalysisSPList(Period, PeriodYear);
                            IList<ORDRPT_AnalysisSP> Analysislist = ItemList.FirstOrDefault().Value;
                            int count = Analysislist.Count + 14;
                            ExcelWorksheet ws = pck.Workbook.Worksheets.Add(Workbookset.Tables[i].TableName.ToString());
                            ws.View.ZoomScale = 70;
                            ws.View.ShowGridLines = false;
                            ws.Column(1).Width = 8;
                            ws.Column(2).Width = 20.00;
                            ws.Column(3).Width = 60.00;
                            ws.Column(4).Width = 15.00;
                            ws.Column(5).Width = 20.00;
                            ws.Column(6).Width = 20.00;
                            ws.Column(7).Width = 20.00;
                            ws.Column(8).Width = 20.00;

                            ws.Row(2).Height = 30.00;
                            ws.Row(11).Height = 30.00;
                            ws.Cells["B2:H11,B7:H8,B14:H" + count].Style.WrapText = true;
                            ws.Cells["B2:G10,B7:H8,B14:H" + count].Style.Font.Name = "Calibri";
                            ws.Cells["B14:H" + count].Style.Font.Size = 12;
                            ws.Cells["B2,B8:H11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            ws.Cells["B2:H11"].Style.WrapText = true;
                            ws.Cells["B2:G10"].Style.Font.Name = "Calibri";
                            ws.Cells["B2"].Style.Font.Size = 18;
                            ws.Cells["B2:G10"].Style.Font.Bold = true;


                            ws.Cells["B7:H8"].Style.WrapText = true;
                            ws.Cells["B7:H8"].Style.Font.Name = "Calibri";
                            ws.Cells["B7:10"].Style.Font.Size = 14;
                            ws.Cells["B7:10"].Style.Font.Bold = true;



                            ws.Cells["B2:H2,C8:D10,A8:A" + count].Merge = true;
                            ws.Cells["B2"].Value = "PERIOD " + Period + " " + PeriodYear + " REQUISITION";
                            ws.Cells["B8"].Value = "TROOPS:";
                            ws.Cells["B9"].Value = "START DATE:";
                            ws.Cells["B10"].Value = "END DATE:";
                            ws.Cells["E7"].Value = "INITIAL";
                            ws.Cells["F7"].Value = "FINAL";
                            ws.Cells["G7"].Value = "DIFFERENCE";
                            ws.Cells["B11"].Value = "Code";
                            ws.Cells["C11"].Value = "Item";
                            ws.Cells["D11"].Value = "Temp";
                            ws.Cells["E11,F11"].Value = "Quantity(Kg/Lt)";
                            ws.Cells["G11"].Value = "DIFFERENCE IN KG";
                            ws.Cells["H11"].Value = "DIFFERENCE IN %";


                            ws.Cells["A8:H" + count].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            ws.Cells["A8:H" + count].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            ws.Cells["A8:H" + count].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            ws.Cells["A8:H" + count].Style.Border.Right.Style = ExcelBorderStyle.Thin;


                            Color BLUECOLOR = System.Drawing.ColorTranslator.FromHtml("#007000");
                            ws.Cells["B11:H11,B2:H2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["B11:H11,B2:H2"].Style.Fill.BackgroundColor.SetColor(Color.Gray);

                            ws.Cells["E8"].Value = Analysislist[0].InitialTroops;
                            //Troops from final orders is not coming from  the stored procedure
                            //ws.Cells["F8"].Value = Analysislist[0].Troops;


                            ws.Cells["F8"].Value = Convert.ToInt32(SectorBasedTroops[0] / 4) + Convert.ToInt32(SectorBasedTroops[1] / 4) + Convert.ToInt32(SectorBasedTroops[2] / 4);

                            ws.Cells["G8"].Value = Convert.ToInt64(ws.Cells["F8"].Value) - Analysislist[0].InitialTroops;
                            ws.Cells["E9"].Value = String.Format("{0:dd-MMM-yy}", Analysislist[0].InitialStartDate);
                            ws.Cells["E10"].Value = String.Format("{0:dd-MMM-yy}", Analysislist[0].InitialEndDate);

                            ws.Cells["F9"].Value = String.Format("{0:dd-MMM-yy}", Analysislist[0].StartDate);
                            ws.Cells["F10"].Value = String.Format("{0:dd-MMM-yy}", Analysislist[0].EndDate);

                            int k = 0;
                            for (int row = 14; row < count; row++)
                            {
                                ws.Cells["B" + row].Value = Analysislist[k].UNCode;
                                ws.Cells["C" + row].Value = Analysislist[k].Commodity;
                                ws.Cells["D" + row].Value = Analysislist[k].Temperature;
                                ws.Cells["E" + row].Value = Math.Round(Analysislist[k].InitialOrdQty, 3);
                                ws.Cells["F" + row].Value = Math.Round(Analysislist[k].OrderQty, 3);
                                ws.Cells["G" + row].Value = Math.Round(Analysislist[k].Difference, 3);
                                ws.Cells["H" + row].Value = Convert.ToInt32(Analysislist[k].DiffPercentage) + " %";
                                k = k + 1;
                            }




                            ws.Cells[13, 5].Formula = string.Format("Sum({0})", new ExcelAddress(14, 5, count, 5).Address);
                            ws.Cells[13, 5].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[12, 5].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(14, 5, count, 5).Address, new ExcelAddress(43, 5, 43, 5).Address, new ExcelAddress(43, 5, 43, 5).Address);
                            ws.Cells[12, 5].Style.Font.Bold = true;
                            ws.Cells[12, 5].Style.Numberformat.Format = "#,##0.000";


                            ws.Cells[13, 6].Formula = string.Format("Sum({0})", new ExcelAddress(14, 6, count, 6).Address);
                            ws.Cells[13, 6].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[12, 6].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(14, 6, count, 6).Address, new ExcelAddress(43, 6, 43, 6).Address, new ExcelAddress(43, 6, 43, 6).Address);
                            ws.Cells[12, 6].Style.Font.Bold = true;
                            ws.Cells[12, 6].Style.Numberformat.Format = "#,##0.000";


                            ws.Cells[13, 7].Formula = string.Format("Sum({0})", new ExcelAddress(14, 7, count, 7).Address);
                            ws.Cells[13, 7].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[12, 7].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(14, 7, count, 7).Address, new ExcelAddress(43, 7, 43, 7).Address, new ExcelAddress(43, 7, 43, 7).Address);
                            ws.Cells[12, 7].Style.Font.Bold = true;
                            ws.Cells[12, 7].Style.Numberformat.Format = "#,##0.000";


                        }
                        #endregion

                        #region Final report
                        //Added by kingst for Consolidate week report
                        if (Workbookset.Tables[i].TableName.ToString() == "Final " + Period)
                        {

                            criteria.Clear();
                            criteria.Add("Period", Period);
                            criteria.Add("PeriodYear", PeriodYear);

                            Dictionary<long, IList<ORDRPT_FinalReport_SP>> ItemList = OS.GetORDRPT_FinalReportList(Period, PeriodYear, Weeks[0], Weeks[1], Weeks[2], Weeks[3]);
                            IList<ORDRPT_FinalReport_SP> FinalList = ItemList.FirstOrDefault().Value;
                            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Final " + Period);

                            ws.View.ZoomScale = 70;



                            ws.Row(9).Height = 60.00;
                            // ws.Row(9).Height = 50.00;
                            ws.Column(1).Width = 9.00;
                            ws.Column(2).Width = 40.00;
                            ws.Column(3).Width = 9.00;
                            ws.Column(4).Width = 12.43;
                            ws.Column(5).Width = 12.43;
                            ws.Column(6).Width = 12.43;
                            ws.Column(7).Width = 12.43;
                            ws.Column(8).Width = 5.00;

                            ws.Column(9).Width = 12.43;
                            ws.Column(10).Width = 12.43;
                            ws.Column(11).Width = 12.43;
                            ws.Column(12).Width = 12.43;
                            ws.Column(13).Width = 12.43;

                            ws.Column(14).Width = 5.00;
                            ws.Column(15).Width = 5.00;
                            ws.Column(16).Width = 12.43;
                            ws.Column(17).Width = 12.43;
                            ws.Column(18).Width = 12.43;

                            ws.Column(19).Width = 5.00;
                            ws.Column(20).Width = 12.43;
                            ws.Column(21).Width = 12.43;
                            ws.Column(22).Width = 12.43;

                            //Font size
                            ws.Cells["A1:AZ"].Style.WrapText = true;
                            ws.Cells["A1:AZ"].Style.Font.Name = "Calibri";
                            ws.Cells["A1:AZ"].Style.Font.Size = 12;

                            Color GREYCOLOR = System.Drawing.ColorTranslator.FromHtml("#b6b6b6");
                            ws.Cells["A9:I10"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["A9:I10"].Style.Fill.BackgroundColor.SetColor(GREYCOLOR);

                            Color Green = System.Drawing.ColorTranslator.FromHtml("#33FFFF");
                            ws.Cells["K9:K10,P9:P10,T9:T10"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["K9:K10,P9:P10,T9:T10"].Style.Fill.BackgroundColor.SetColor(Green);

                            Color brown = System.Drawing.ColorTranslator.FromHtml("#FF9933");
                            ws.Cells["L9:L10,Q9:Q10,U9:U10"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["L9:L10,Q9:Q10,U9:U10"].Style.Fill.BackgroundColor.SetColor(brown);


                            Color orange = System.Drawing.ColorTranslator.FromHtml("#CC9900");
                            ws.Cells["M9:M10,R9:R10,V9:V10"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["M9:M10,R9:R10,V9:V10"].Style.Fill.BackgroundColor.SetColor(orange);


                            ws.Cells["A11:V11"].Style.Font.Color.SetColor(Color.White);
                            ws.Cells["A11:V11"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            ws.Cells["A11:V11"].Style.Fill.BackgroundColor.SetColor(Color.Black);


                            ws.Cells["A9,B9,C9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; //Center Alignments for Cells

                            ws.Cells["B1"].Value = "Final Requisition UNAMID" + Period + "-" + PeriodYear;
                            ws.Cells["B4"].Value = "HQ location:";
                            ws.Cells["B5"].Value = "location:";
                            ws.Cells["B6"].Value = "Unit Name";
                            ws.Cells["B7"].Value = "Troopstrength:";
                            ws.Cells["B8"].Value = "Per man per day";

                            //Header
                            ws.Cells["D8"].Value = "Week - " + Weeks[0];
                            ws.Cells["E8"].Value = "Week - " + Weeks[1];
                            ws.Cells["F8"].Value = "Week - " + Weeks[2];
                            ws.Cells["G8"].Value = "Week - " + Weeks[3];
                            ws.Cells["I4"].Value = "Contingents:";
                            ws.Cells["I5"].Value = "Eggs in Kg:";
                            ws.Cells["I6"].Value = "Eggs eachas UN calc:";
                            ws.Cells["I7"].Value = "Total:";
                            ws.Cells["K7"].Value = "Divided in A/C/F";
                            ws.Cells["P7"].Value = "volume per week		";
                            ws.Cells["T7"].Value = "volume per day	";
                            ws.Cells["F9"].Value = "Code No";
                            ws.Cells["B9"].Value = "COMMODITY";
                            ws.Cells["D9,E9,F9,G9,I9"].Value = Period + " - " + PeriodYear + " Total Quantity Ordered ";
                            ws.Cells["K9,P9,T9"].Value = " Dry  /kg ";
                            ws.Cells["L9,Q9,U9"].Value = " Chilled  /kg ";
                            ws.Cells["M9,R9,V9"].Value = " Frozen  /kg ";
                            ws.Cells["K10,P10,T10"].Value = " A ";
                            ws.Cells["L10,Q10,U10"].Value = "C ";
                            ws.Cells["M10,R10,V10"].Value = " F";
                            ws.Cells["K7:M7,P7:R7,T7:V7"].Merge = true;
                            ws.Cells["D10,E10,F10,G10,I10"].Value = "(Kg/Lt/EA)";

                            int count = FinalList.Count() + 13;
                            int k = 0; // Sno count
                            int L = 0; // Increament for every line
                            ws.Cells["E1:AB" + count].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells["E1:AB" + count].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                            ws.Cells["A9:G" + count + ",I9:I" + count + ",K7:M" + count + ",P7:R" + count + ",T7:V" + count + ",I4:J7"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            ws.Cells["A9:G" + count + ",I9:I" + count + ",K7:M" + count + ",P7:R" + count + ",T7:V" + count + ",I4:J7"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            ws.Cells["A9:G" + count + ",I9:I" + count + ",K7:M" + count + ",P7:R" + count + ",T7:V" + count + ",I4:J7"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            ws.Cells["A9:G" + count + ",I9:I" + count + ",K7:M" + count + ",P7:R" + count + ",T7:V" + count + ",I4:J7"].Style.Border.Right.Style = ExcelBorderStyle.Thin;



                            for (int p = 13; p < count; p++)
                            {

                                ws.Cells["A" + p].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                ws.Cells["A" + p].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                // ws.Cells["B" + p].Value = ItemsList[L].Commodity;
                                ws.Cells["B" + p].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                ws.Cells["B" + p].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells["A" + p].Value = FinalList[k].UNCode;
                                ws.Cells["B" + p].Value = FinalList[k].Commodity;
                                ws.Cells["C" + p].Value = FinalList[k].Temperature;

                                ws.Cells["D" + p].Value = Math.Round(FinalList[k].OrdQty_WK1, 3);
                                ws.Cells["E" + p].Value = Math.Round(FinalList[k].OrdQty_WK2, 3);
                                ws.Cells["F" + p].Value = Math.Round(FinalList[k].OrdQty_WK3, 3);
                                ws.Cells["G" + p].Value = Math.Round(FinalList[k].OrdQty_WK4, 3);

                                ws.Cells["I" + p].Value = Math.Round(FinalList[k].WeekTotal, 3);
                                ws.Cells["K" + p].Value = Math.Round(FinalList[k].DryTotal, 3);
                                ws.Cells["L" + p].Value = Math.Round(FinalList[k].ChillTotal, 3);
                                ws.Cells["M" + p].Value = Math.Round(FinalList[k].FrozenTotal, 3);

                                ws.Cells["P" + p].Value = Math.Round(FinalList[k].DryWKTotal, 3);
                                ws.Cells["Q" + p].Value = Math.Round(FinalList[k].ChillWKTotal, 3);
                                ws.Cells["R" + p].Value = Math.Round(FinalList[k].FrozenWKTotal, 3);
                                ws.Cells["T" + p].Value = Math.Round(FinalList[k].DryDAYTotal, 3);

                                ws.Cells["U" + p].Value = Math.Round(FinalList[k].ChillDAYTotal, 3);
                                ws.Cells["V" + p].Value = Math.Round(FinalList[k].FrozenDAYTotal, 3);



                                ws.Row(p).Height = 21.20;
                                k = k + 1;
                                L = L + 1;
                            }
                            ws.Cells["J6"].Value = ws.Cells["I42"].Value;
                            ws.Cells["J5"].Value = Convert.ToDouble(ws.Cells["J6"].Value) * 0.058824;
                            //  ws.Cells["E10:AZ"].Value.ToString().Replace("0", "-");

                            //o = o + 1;

                            ws.Cells[12, 4].Formula = string.Format("Sum({0})", new ExcelAddress(13, 4, count, 4).Address);
                            ws.Cells[12, 4].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[11, 4].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(13, 4, count, 4).Address, new ExcelAddress(42, 4, 42, 4).Address, new ExcelAddress(42, 4, 42, 4).Address);
                            ws.Cells[11, 4].Style.Font.Bold = true;
                            ws.Cells[11, 4].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[12, 5].Formula = string.Format("Sum({0})", new ExcelAddress(13, 5, count, 5).Address);
                            ws.Cells[12, 5].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[11, 5].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(13, 5, count, 5).Address, new ExcelAddress(42, 5, 42, 5).Address, new ExcelAddress(42, 5, 42, 5).Address);
                            ws.Cells[11, 5].Style.Font.Bold = true;
                            ws.Cells[11, 5].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[12, 6].Formula = string.Format("Sum({0})", new ExcelAddress(13, 6, count, 6).Address);
                            ws.Cells[12, 6].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[11, 6].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(13, 6, count, 6).Address, new ExcelAddress(42, 6, 42, 6).Address, new ExcelAddress(42, 6, 42, 6).Address);
                            ws.Cells[11, 6].Style.Font.Bold = true;
                            ws.Cells[11, 6].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[12, 7].Formula = string.Format("Sum({0})", new ExcelAddress(13, 7, count, 7).Address);
                            ws.Cells[12, 7].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[11, 7].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(13, 7, count, 7).Address, new ExcelAddress(42, 7, 42, 7).Address, new ExcelAddress(42, 7, 42, 7).Address);
                            ws.Cells[11, 7].Style.Font.Bold = true;
                            ws.Cells[11, 7].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[12, 9].Formula = string.Format("Sum({0})", new ExcelAddress(13, 9, count, 9).Address);
                            ws.Cells[12, 9].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[11, 9].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(13, 9, count, 9).Address, new ExcelAddress(42, 9, 42, 9).Address, new ExcelAddress(42, 9, 42, 9).Address);
                            ws.Cells[11, 9].Style.Font.Bold = true;
                            ws.Cells[11, 9].Style.Numberformat.Format = "#,##0.000";

                            //----
                            ws.Cells[12, 11].Formula = string.Format("Sum({0})", new ExcelAddress(13, 11, count, 11).Address);
                            ws.Cells[12, 11].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[11, 11].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(13, 11, count, 11).Address, new ExcelAddress(42, 11, 42, 11).Address, new ExcelAddress(42, 11, 42, 11).Address);
                            ws.Cells[11, 11].Style.Font.Bold = true;
                            ws.Cells[11, 11].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[12, 12].Formula = string.Format("Sum({0})", new ExcelAddress(13, 12, count, 12).Address);
                            ws.Cells[12, 12].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[11, 12].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(13, 12, count, 12).Address, new ExcelAddress(42, 12, 42, 12).Address, new ExcelAddress(42, 12, 42, 12).Address);
                            ws.Cells[11, 12].Style.Font.Bold = true;
                            ws.Cells[11, 12].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[12, 13].Formula = string.Format("Sum({0})", new ExcelAddress(13, 13, count, 13).Address);
                            ws.Cells[12, 13].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[11, 13].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(13, 13, count, 13).Address, new ExcelAddress(42, 13, 42, 13).Address, new ExcelAddress(42, 13, 42, 13).Address);
                            ws.Cells[11, 13].Style.Font.Bold = true;
                            ws.Cells[11, 13].Style.Numberformat.Format = "#,##0.000";


                            ws.Cells[12, 16].Formula = string.Format("Sum({0})", new ExcelAddress(13, 16, count, 16).Address);
                            ws.Cells[12, 16].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[11, 16].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(13, 16, count, 16).Address, new ExcelAddress(42, 16, 42, 16).Address, new ExcelAddress(42, 16, 42, 16).Address);
                            ws.Cells[11, 16].Style.Font.Bold = true;
                            ws.Cells[11, 16].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[12, 17].Formula = string.Format("Sum({0})", new ExcelAddress(13, 17, count, 17).Address);
                            ws.Cells[12, 17].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[11, 17].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(13, 17, count, 17).Address, new ExcelAddress(42, 17, 42, 17).Address, new ExcelAddress(42, 17, 42, 17).Address);
                            ws.Cells[11, 17].Style.Font.Bold = true;
                            ws.Cells[11, 17].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[12, 18].Formula = string.Format("Sum({0})", new ExcelAddress(13, 18, count, 18).Address);
                            ws.Cells[12, 18].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[11, 18].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(13, 18, count, 18).Address, new ExcelAddress(42, 18, 42, 18).Address, new ExcelAddress(42, 18, 42, 18).Address);
                            ws.Cells[11, 18].Style.Font.Bold = true;
                            ws.Cells[11, 18].Style.Numberformat.Format = "#,##0.000";



                            ws.Cells[12, 20].Formula = string.Format("Sum({0})", new ExcelAddress(13, 20, count, 20).Address);
                            ws.Cells[12, 20].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[11, 20].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(13, 20, count, 20).Address, new ExcelAddress(42, 20, 42, 20).Address, new ExcelAddress(42, 20, 42, 20).Address);
                            ws.Cells[11, 20].Style.Font.Bold = true;
                            ws.Cells[11, 20].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[12, 21].Formula = string.Format("Sum({0})", new ExcelAddress(13, 21, count, 21).Address);
                            ws.Cells[12, 21].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[11, 21].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(13, 21, count, 21).Address, new ExcelAddress(42, 21, 42, 21).Address, new ExcelAddress(42, 21, 42, 21).Address);
                            ws.Cells[11, 21].Style.Font.Bold = true;
                            ws.Cells[11, 21].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[12, 22].Formula = string.Format("Sum({0})", new ExcelAddress(13, 22, count, 22).Address);
                            ws.Cells[12, 22].Style.Numberformat.Format = "#,##0.000";

                            ws.Cells[11, 22].Formula = string.Format("Sum({0})-{1}+{2}*0.058824", new ExcelAddress(13, 22, count, 22).Address, new ExcelAddress(42, 22, 42, 22).Address, new ExcelAddress(42, 22, 42, 22).Address);
                            ws.Cells[11, 22].Style.Font.Bold = true;
                            ws.Cells[11, 22].Style.Numberformat.Format = "#,##0.000";


                        }

                        #endregion

                        #region summary report

                        if (Workbookset.Tables[i].TableName.ToString() == "Summary")
                        {
                            Dictionary<long, IList<ORDRPT_Summary>> ItemList = OS.GetSummaryList(Period, PeriodYear);
                            IList<ORDRPT_Summary> summarylist = ItemList.FirstOrDefault().Value;

                            ExcelWorksheet ws = pck.Workbook.Worksheets.Add(Workbookset.Tables[i].TableName.ToString());
                            ws.View.ZoomScale = 70;
                            ws.View.ShowGridLines = false;
                            ws.Column(1).Width = 5;
                            ws.Column(2).Width = 5;
                            ws.Column(3).Width = 50.00;
                            ws.Column(4).Width = 15.00;
                            ws.Column(5).Width = 15.00;
                            ws.Column(6).Width = 15.00;
                            ws.Column(7).Width = 15.00;
                            ws.Column(8).Width = 15.00;
                            ws.Column(9).Width = 15.00;
                            ws.Column(10).Width = 15.00;
                            ws.Column(11).Width = 15.00;
                            ws.Column(12).Width = 15.00;
                            ws.Column(13).Width = 15.00;
                            ws.Column(14).Width = 15.00;


                            ws.Cells["C2"].Value = "SUMMARY - PERIOD " + Period + " " + PeriodYear;
                            ws.Cells["C3"].Value = "Troop Strength Details";
                            ws.Cells["C4,I5,I11,I17"].Value = "Sector";
                            ws.Cells["D4,I6,I12,I18"].Value = "North";
                            ws.Cells["E4,I7,I13,I19"].Value = "South";
                            ws.Cells["F4,I8,I14,I20"].Value = "West";
                            //ws.Cells["F4"].Value = "West";
                            ws.Cells["C9"].Value = "Item Details:";
                            ws.Cells["C11"].Value = "Total line item:";
                            ws.Cells["C12"].Value = "Total QTY / Kg";
                            ws.Cells["C13"].Value = "Per week (included bread & eggs)";
                            ws.Cells["C14"].Value = "Per day";
                            ws.Cells["I3"].Value = "Order Changed Date:";
                            ws.Cells["I4"].Value = "Troop Strength When Order Received First Time";
                            ws.Cells["J5,J11,J17"].Value = "Week1";
                            ws.Cells["K5,K11,K17"].Value = "Week2";
                            ws.Cells["L5,L11,L17"].Value = "Week3";
                            ws.Cells["M5,M11,M17"].Value = "Week4";
                            ws.Cells["N5,N11,N17,G4"].Value = "Total";
                            ws.Cells["I10"].Value = "Troop Strength after Order Changes";
                            ws.Cells["I16"].Value = "Difference";
                            ws.Cells["C5"].Value = "Contingents (4 Weeks Avg.)";
                            ws.Cells["C6"].Value = "Troop Strength (4 Weeks Avg.)";

                            ws.View.ShowGridLines = false;


                            ws.Cells["B2:O22"].Style.WrapText = true;
                            ws.Cells["B2:O22"].Style.Font.Name = "Calibri";
                            ws.Cells["B2:O22"].Style.Font.Size = 12;

                            //border
                            ws.Cells["C5:G6,D4:G4,C11:G14,I5:N8,I11:N14,I17:N20,B22:O22"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            ws.Cells["C5:G6,D4:G4,C11:G14,I5:N8,I11:N14,I17:N20,B2:O2"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            ws.Cells["C5:G6,D4:G4,C11:G14,I5:N8,I11:N14,I17:N20,B2:B22"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            ws.Cells["C5:G6,D4:G4,C11:G14,I5:N8,I11:N14,I17:N20,O2:O22"].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            ws.Cells["I4:L4"].Merge = true;
                            ws.Cells["I10:K10"].Merge = true;
                            #region Old
                            //ws.Cells["D6"].Value = Convert.ToInt32(SectorBasedTroops[0] / 4);
                            //ws.Cells["E6"].Value = Convert.ToInt32(SectorBasedTroops[1] / 4);
                            //ws.Cells["F6"].Value = Convert.ToInt32(SectorBasedTroops[2] / 4);
                            //ws.Cells["G6"].Value = Convert.ToInt32(ws.Cells["D6"].Value) + Convert.ToInt32(ws.Cells["E6"].Value) + Convert.ToInt32(ws.Cells["F6"].Value);

                            //ws.Cells["D5"].Value = Convert.ToInt32(SectorBasedContingent[0] / 4);
                            //ws.Cells["E5"].Value = Convert.ToInt32(SectorBasedContingent[1] / 4);
                            //ws.Cells["F5"].Value = Convert.ToInt32(SectorBasedContingent[2] / 4);
                            //ws.Cells["G5"].Value = Convert.ToInt32(ws.Cells["D5"].Value) + Convert.ToInt32(ws.Cells["E5"].Value) + Convert.ToInt32(ws.Cells["F5"].Value);
                            #endregion
                            ws.Cells["D6"].Value = Convert.ToInt32(SectorBasedTroops[0] / 4);
                            ws.Cells["F6"].Value = Convert.ToInt32(SectorBasedTroops[1] / 4);
                            ws.Cells["E6"].Value = Convert.ToInt32(SectorBasedTroops[2] / 4);
                            ws.Cells["G6"].Value = Convert.ToInt32(ws.Cells["D6"].Value) + Convert.ToInt32(ws.Cells["E6"].Value) + Convert.ToInt32(ws.Cells["F6"].Value);

                            ws.Cells["D5"].Value = Convert.ToInt32(SectorBasedContingent[0] / 4);
                            ws.Cells["F5"].Value = Convert.ToInt32(SectorBasedContingent[1] / 4);
                            ws.Cells["E5"].Value = Convert.ToInt32(SectorBasedContingent[2] / 4);
                            ws.Cells["G5"].Value = Convert.ToInt32(ws.Cells["D5"].Value) + Convert.ToInt32(ws.Cells["E5"].Value) + Convert.ToInt32(ws.Cells["F5"].Value);

                            ws.Cells["D11"].Value = "Dry";
                            ws.Cells["E11"].Value = "Chilled";
                            ws.Cells["F11"].Value = "Frozen";
                            ws.Cells["G11"].Value = "Total Order";

                            foreach (var item in summarylist)
                            {
                                if (item.Temperature == "Dry")
                                {

                                    ws.Cells["D12"].Value = Math.Round(item.Total, 3);
                                    ws.Cells["D13"].Value = Math.Round(item.WeekTotal, 3);
                                    ws.Cells["D14"].Value = Math.Round(item.DayTotal, 3);
                                }
                                if (item.Temperature == "Chill")
                                {

                                    ws.Cells["E12"].Value = Math.Round(item.Total, 3);
                                    ws.Cells["E13"].Value = Math.Round(item.WeekTotal, 3);
                                    ws.Cells["E14"].Value = Math.Round(item.DayTotal, 3);
                                }
                                if (item.Temperature == "Frozen")
                                {

                                    ws.Cells["F12"].Value = Math.Round(item.Total, 3);
                                    ws.Cells["F13"].Value = Math.Round(item.WeekTotal, 3);
                                    ws.Cells["F14"].Value = Math.Round(item.DayTotal, 3);
                                }

                            }
                            ws.Cells["G12"].Value = Convert.ToDecimal(ws.Cells["D12"].Value) + Convert.ToDecimal(ws.Cells["E12"].Value) + Convert.ToDecimal(ws.Cells["F12"].Value);
                            ws.Cells["G13"].Value = Convert.ToDecimal(ws.Cells["D13"].Value) + Convert.ToDecimal(ws.Cells["E13"].Value) + Convert.ToDecimal(ws.Cells["F13"].Value);
                            ws.Cells["G14"].Value = Convert.ToDecimal(ws.Cells["D14"].Value) + Convert.ToDecimal(ws.Cells["E14"].Value) + Convert.ToDecimal(ws.Cells["F14"].Value);

                            criteria.Clear();
                            criteria.Add("Period", Period);
                            criteria.Add("PeriodYear", PeriodYear);

                            Dictionary<long, IList<ORDRPT_SummaryTroops>> Summaryviewreport = OS.GetORDRPT_SummaryTroopsWithPagingAndCriteria(0, 999999, string.Empty, string.Empty, criteria);
                            IList<ORDRPT_SummaryTroops> troopsreport = Summaryviewreport.FirstOrDefault().Value;
                            var OrderChangeDate = (from u in troopsreport select u.CreatedDate).Max();

                            foreach (var item in troopsreport)
                            {
                                if (item.Category == "Initial")
                                {

                                    if (item.FinalWeek == Weeks[0])
                                    {
                                        if (item.FinalSector == "FS")
                                            ws.Cells["J6"].Value = item.FinalTroops;
                                        if (item.FinalSector == "NS")
                                            ws.Cells["J7"].Value = item.FinalTroops;
                                        if (item.FinalSector == "GS")
                                            ws.Cells["J8"].Value = item.FinalTroops;

                                    }
                                    if (item.FinalWeek == Weeks[1])
                                    {
                                        if (item.FinalSector == "FS")
                                            ws.Cells["K6"].Value = item.FinalTroops;
                                        if (item.FinalSector == "NS")
                                            ws.Cells["K7"].Value = item.FinalTroops;
                                        if (item.FinalSector == "GS")
                                            ws.Cells["K8"].Value = item.FinalTroops;

                                    }
                                    if (item.FinalWeek == Weeks[2])
                                    {
                                        if (item.FinalSector == "FS")
                                            ws.Cells["L6"].Value = item.FinalTroops;
                                        if (item.FinalSector == "NS")
                                            ws.Cells["L7"].Value = item.FinalTroops;
                                        if (item.FinalSector == "GS")
                                            ws.Cells["L8"].Value = item.FinalTroops;

                                    }
                                    if (item.FinalWeek == Weeks[3])
                                    {
                                        if (item.FinalSector == "FS")
                                            ws.Cells["M6"].Value = item.FinalTroops;
                                        if (item.FinalSector == "NS")
                                            ws.Cells["M7"].Value = item.FinalTroops;
                                        if (item.FinalSector == "GS")
                                            ws.Cells["M8"].Value = item.FinalTroops;

                                    }

                                }

                                //Final values

                                if (item.Category == "Final")
                                {

                                    if (item.FinalWeek == Weeks[0])
                                    {
                                        if (item.FinalSector == "FS")
                                            ws.Cells["J12"].Value = item.FinalTroops;
                                        if (item.FinalSector == "NS")
                                            ws.Cells["J13"].Value = item.FinalTroops;
                                        if (item.FinalSector == "GS")
                                            ws.Cells["J14"].Value = item.FinalTroops;

                                    }
                                    if (item.FinalWeek == Weeks[1])
                                    {
                                        if (item.FinalSector == "FS")
                                            ws.Cells["K12"].Value = item.FinalTroops;
                                        if (item.FinalSector == "NS")
                                            ws.Cells["K13"].Value = item.FinalTroops;
                                        if (item.FinalSector == "GS")
                                            ws.Cells["K14"].Value = item.FinalTroops;

                                    }
                                    if (item.FinalWeek == Weeks[2])
                                    {
                                        if (item.FinalSector == "FS")
                                            ws.Cells["L12"].Value = item.FinalTroops;
                                        if (item.FinalSector == "NS")
                                            ws.Cells["L13"].Value = item.FinalTroops;
                                        if (item.FinalSector == "GS")
                                            ws.Cells["L14"].Value = item.FinalTroops;

                                    }
                                    if (item.FinalWeek == Weeks[3])
                                    {
                                        if (item.FinalSector == "FS")
                                            ws.Cells["M12"].Value = item.FinalTroops;
                                        if (item.FinalSector == "NS")
                                            ws.Cells["M13"].Value = item.FinalTroops;
                                        if (item.FinalSector == "GS")
                                            ws.Cells["M14"].Value = item.FinalTroops;
                                    }
                                }
                            }

                            ws.Cells["J3"].Value = String.Format("{0:dd-MMM-yy}", OrderChangeDate);
                            ws.Cells["N6"].Value = Convert.ToDecimal(ws.Cells["J6"].Value) + Convert.ToDecimal(ws.Cells["K6"].Value) + Convert.ToDecimal(ws.Cells["L6"].Value) + Convert.ToDecimal(ws.Cells["M6"].Value);
                            ws.Cells["N7"].Value = Convert.ToDecimal(ws.Cells["J7"].Value) + Convert.ToDecimal(ws.Cells["K7"].Value) + Convert.ToDecimal(ws.Cells["L7"].Value) + Convert.ToDecimal(ws.Cells["M7"].Value);
                            ws.Cells["N8"].Value = Convert.ToDecimal(ws.Cells["J8"].Value) + Convert.ToDecimal(ws.Cells["K8"].Value) + Convert.ToDecimal(ws.Cells["L8"].Value) + Convert.ToDecimal(ws.Cells["M8"].Value);

                            ws.Cells["N12"].Value = Convert.ToDecimal(ws.Cells["J12"].Value) + Convert.ToDecimal(ws.Cells["K12"].Value) + Convert.ToDecimal(ws.Cells["L12"].Value) + Convert.ToDecimal(ws.Cells["M12"].Value);
                            ws.Cells["N13"].Value = Convert.ToDecimal(ws.Cells["J13"].Value) + Convert.ToDecimal(ws.Cells["K13"].Value) + Convert.ToDecimal(ws.Cells["L13"].Value) + Convert.ToDecimal(ws.Cells["M13"].Value);
                            ws.Cells["N14"].Value = Convert.ToDecimal(ws.Cells["J14"].Value) + Convert.ToDecimal(ws.Cells["K14"].Value) + Convert.ToDecimal(ws.Cells["L14"].Value) + Convert.ToDecimal(ws.Cells["M14"].Value);

                            ws.Cells["J18"].Value = Convert.ToDecimal(ws.Cells["J6"].Value) - Convert.ToDecimal(ws.Cells["J12"].Value);
                            ws.Cells["J19"].Value = Convert.ToDecimal(ws.Cells["J7"].Value) - Convert.ToDecimal(ws.Cells["J13"].Value);
                            ws.Cells["J20"].Value = Convert.ToDecimal(ws.Cells["J8"].Value) - Convert.ToDecimal(ws.Cells["J14"].Value);

                            ws.Cells["K18"].Value = Convert.ToDecimal(ws.Cells["K6"].Value) - Convert.ToDecimal(ws.Cells["K12"].Value);
                            ws.Cells["K19"].Value = Convert.ToDecimal(ws.Cells["K7"].Value) - Convert.ToDecimal(ws.Cells["K13"].Value);
                            ws.Cells["K20"].Value = Convert.ToDecimal(ws.Cells["K8"].Value) - Convert.ToDecimal(ws.Cells["K14"].Value);

                            ws.Cells["L18"].Value = Convert.ToDecimal(ws.Cells["L6"].Value) - Convert.ToDecimal(ws.Cells["L12"].Value);
                            ws.Cells["L19"].Value = Convert.ToDecimal(ws.Cells["L7"].Value) - Convert.ToDecimal(ws.Cells["L13"].Value);
                            ws.Cells["L20"].Value = Convert.ToDecimal(ws.Cells["L8"].Value) - Convert.ToDecimal(ws.Cells["L14"].Value);

                            ws.Cells["M18"].Value = Convert.ToDecimal(ws.Cells["M6"].Value) - Convert.ToDecimal(ws.Cells["M12"].Value);
                            ws.Cells["M19"].Value = Convert.ToDecimal(ws.Cells["M7"].Value) - Convert.ToDecimal(ws.Cells["M13"].Value);
                            ws.Cells["M20"].Value = Convert.ToDecimal(ws.Cells["M8"].Value) - Convert.ToDecimal(ws.Cells["M14"].Value);

                            ws.Cells["N18"].Value = Convert.ToDecimal(ws.Cells["N6"].Value) - Convert.ToDecimal(ws.Cells["N12"].Value);
                            ws.Cells["N19"].Value = Convert.ToDecimal(ws.Cells["N7"].Value) - Convert.ToDecimal(ws.Cells["N13"].Value);
                            ws.Cells["N20"].Value = Convert.ToDecimal(ws.Cells["N8"].Value) - Convert.ToDecimal(ws.Cells["N14"].Value);


                        }
                        #endregion
                    }
                    string txtTName = "FinalFoodRequisition ";
                    byte[] data = pck.GetAsByteArray();

                    ExcelDocuments excel = new ExcelDocuments();
                    excel.ControlId = "Consolidated Food Requisition-" + Period + "-" + PeriodYear;
                    excel.Period = Period;
                    excel.PeriodYear = PeriodYear;
                    excel.DocumentType = "Consolidate Food Order Report";
                    excel.DocumentName = "Consolidated Food Requisition-" + Period + "-" + PeriodYear;
                    excel.DocumentData = data;
                    excel.CreatedDate = DateTime.Now;
                    excel.CreatedBy = loggedInUserId;
                    long id = IS.SaveOrUpdateExcelDocuments(excel, loggedInUserId);


                }

            }
            catch (Exception ex)
            {
                UploadRequest upload = new UploadRequest();
                upload.ErrorDesc = ex.ToString();
                upload.CreatedDate = DateTime.Now;
                OS.SaveOrUpdateUploadRequest(upload);
                ExceptionPolicy.HandleException(ex, "InsightReportPolicy");
                throw ex;
            }
        }
        #endregion
        #region Inventory Related
        public ActionResult InventoryExcelDocumentListJQGrid(string ExcelState, string searchItems, int rows, string sidx, string sord, int? page = 1)
        {
            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
            {
                try
                {
                    bool IsSubsite = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSubsite"]);
                    string SubsiteName = ConfigurationManager.AppSettings["SubsiteName"].ToString();
                    if (!string.IsNullOrEmpty(ExcelState) && ExcelState == "GITReport")
                    {
                        Dictionary<string, object> criteria = new Dictionary<string, object>();
                        

                        if (!string.IsNullOrWhiteSpace(sord) && sord == "desc")
                            sord = "Desc";
                        else
                            sord = "Asc";
                        criteria.Clear();
                        criteria.Add("DocumentType", "GITReport");
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
                                            items.CreatedBy,
                                            items.CreatedDate.ToString(),
                                            IsSubsite==true?String.Format(@"<a style='color:#034af3;text-decoration:underline' href= '/"+SubsiteName+"/ExcelGeneration/ExcelDownload?Id="+items.Id+"' target='_Blank' >{0}</a>",items.DocumentName):String.Format(@"<a style='color:#034af3;text-decoration:underline' href= '/ExcelGeneration/ExcelDownload?Id="+items.Id+"' target='_Blank' >{0}</a>",items.DocumentName)
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
        #endregion
    }
}

































