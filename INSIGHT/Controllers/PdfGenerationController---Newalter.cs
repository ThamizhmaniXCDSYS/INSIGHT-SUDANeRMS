using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using INSIGHT.Entities.PDFEntities;
using INSIGHT.WCFServices;
using INSIGHT.Entities.InvoiceEntities;
using INSIGHT.Entities;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using System.Globalization;
using System.IO;
using System.Configuration;
using ICSharpCode.SharpZipLib.Zip;
using System.Text;
using ICSharpCode.SharpZipLib.Core;


namespace INSIGHT.Controllers
{
    public class PdfGenerationController : INSIGHT.Controllers.PDFGeneration.PdfViewController
    {
        InvoiceService IS = new InvoiceService();
        OrdersService OS = new OrdersService();
        MastersService MS = new MastersService();
        Dictionary<string, object> criteria = new Dictionary<string, object>();
        IFormatProvider provider = new System.Globalization.CultureInfo("en-CA", true);
        //
        // GET: /PdfGeneration/
        public ActionResult Home()
        {
            return View();
        }
        /// <summary>
        /// Sample PDF
        /// </summary>
        /// <returns></returns>
        public ActionResult Print()
        {
            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
            {

                try
                {
                    BasicList basicList = CreateBasicList();
                    FillImageUrl(basicList, "main_logo.jpg");
                    return this.ViewPdf("", "InvFPUWK3234", basicList, "portrait");
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                    throw ex;
                }
            }
        }
        private void FillImageUrl(BasicList basicList, string imageName)
        {
            string url = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
            basicList.ImageUrl = url + "Images/" + imageName;
        }
        private BasicList CreateBasicList()
        {
            return new BasicList()
                {
                    new Basic { Id = 1, Name = "Patrick", Address = "Geuzenstraat 29", Place = "Amsterdam" },
                    new Basic { Id = 2, Name = "Fred", Address = "Flink 9a", Place = "Rotterdam" },
                    new Basic { Id = 3, Name = "Sjonnie", Address = "Paternatenplaats 44", Place = "Enkhuizen" },
                };
        }

        /// <summary>
        /// Consolidate PDF
        /// </summary>
        /// <param name="OrderId"></param>
        /// <param name="Period"></param>
        /// <returns></returns>
        public ActionResult InvoicePrint(string Period, string searchItems)
        {
            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
            {

                try
                {
                    InvoiceList invoiceList = ConsolidateInvoiceListPDF(Period, searchItems);
                    FillImageUrl(invoiceList, "main_logo.jpg");
                    return this.ViewPdf("", "InvFPUWK3234", invoiceList, "portrait");
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                    throw ex;
                }
            }
        }
        private void FillImageUrl(InvoiceList invoiceList, string imageName)
        {
            string url = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
            invoiceList.ImageUrl = url + "Images/" + imageName;
        }

        /// <summary>
        /// INVOICE QTY & INVOICE VALUE
        /// </summary>
        /// <param name="Sector"></param>
        /// <param name="ContingentType"></param>
        /// <param name="Period"></param>
        /// <returns></returns>


        public IList<PeriodWeek> GetPeriodListVal(string Sector, string ContingentType, string Period, string Year)
        {
            OrdersService orderService = new OrdersService();
            criteria.Clear();
            if (!string.IsNullOrWhiteSpace(Sector)) { criteria.Add("Sector", Sector); }
            if (!string.IsNullOrWhiteSpace(ContingentType)) { criteria.Add("ContingentType", ContingentType); }
            if (!string.IsNullOrWhiteSpace(Period)) { criteria.Add("Period", Period); }
            if (!string.IsNullOrWhiteSpace(Year)) { criteria.Add("PeriodYear", Year); }

            //IList<Vw_WeekWiseFinalOutPut> FinalOutputList = orderService.GetWeekWiseFinalOutputListtWithPagingAndCriteria(0, 99999, string.Empty, string.Empty, criteria);

            IList<InvoiceReports> DictInvRpt = IS.GetInvoiceReportListtWithPagingAndCriteria(0, 99999, string.Empty, string.Empty, criteria);

            criteria.Clear();
            if (!string.IsNullOrEmpty(Period)) { criteria.Add("Period", Period); criteria.Add("Year", Year); }
            Dictionary<long, IList<PeriodMaster>> PeriodMasterList = MS.GetPeriodMasterListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
            var weekList = (from items in PeriodMasterList.First().Value
                            select items.Week).OrderBy(i => i).Distinct().ToList();
            IList<PeriodWeek> periodWeekLst = new List<PeriodWeek>();
            if (weekList.Count != 0)
            {

                for (int i = 0; i < weekList.Count; i++)
                {
                    PeriodWeek pW = new PeriodWeek();
                    var invoiceQty = (from items in DictInvRpt
                                      where items.Sector == Sector && items.Period == Period && items.ContingentType == ContingentType && items.Week == weekList[i]
                                      group items by new { items.Week } into g
                                      select g.Sum(items => items.Acceptedqty)).ToList();
                    var invoiceVal = (from items in DictInvRpt
                                      where items.Sector == Sector && items.Period == Period && items.ContingentType == ContingentType && items.Week == weekList[i]
                                      group items by new { items.Week } into g
                                      select g.Sum(items => items.Amountaccepted)).ToList();

                    pW.Week = weekList[i].ToString();
                    pW.AcceptedQty = invoiceQty.Count != 0 ? invoiceQty[0].ToString() : "";// AcceptedQty named as InvoiceQty
                    pW.InvoiceValue = invoiceVal.Count != 0 ? invoiceVal[0].ToString() : "";
                    periodWeekLst.Add(pW);

                }
            }

            return periodWeekLst;
        }

        public string words(int numbers)
        {
            int number = numbers;

            if (number == 0) return "Zero";
            if (number == -2147483648) return "Minus Two Hundred and Fourteen Crore Seventy Four Lakh Eighty Three Thousand Six Hundred and Forty Eight";
            int[] num = new int[4];
            int first = 0;
            int u, h, t;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (number < 0)
            {
                sb.Append("Minus ");
                number = -number;
            }
            string[] words0 = {"" ,"One ", "Two ", "Three ", "Four ",
            "Five " ,"Six ", "Seven ", "Eight ", "Nine "};
            string[] words1 = {"Ten ", "Eleven ", "Twelve ", "Thirteen ", "Fourteen ",
            "Fifteen ","Sixteen ","Seventeen ","Eighteen ", "Nineteen "};
            string[] words2 = {"Twenty ", "Thirty ", "Forty ", "Fifty ", "Sixty ",
            "Seventy ","Eighty ", "Ninety "};
            string[] words3 = { "Thousand ", "Lakh ", "Crore " };
            num[0] = number % 1000; // units
            num[1] = number / 1000;
            num[2] = number / 100000;
            num[1] = num[1] - 100 * num[2]; // thousands
            num[3] = number / 10000000; // crores
            num[2] = num[2] - 100 * num[3]; // lakhs
            for (int i = 3; i > 0; i--)
            {
                if (num[i] != 0)
                {
                    first = i;
                    break;
                }
            }
            for (int i = first; i >= 0; i--)
            {
                if (num[i] == 0) continue;
                u = num[i] % 10; // ones
                t = num[i] / 10;
                h = num[i] / 100; // hundreds
                t = t - 10 * h; // tens
                if (h > 0) sb.Append(words0[h] + "Hundred ");
                if (u > 0 || t > 0)
                {
                    if (h > 0 || i == 0) sb.Append("and ");
                    if (t == 0)
                        sb.Append(words0[u]);
                    else if (t == 1)
                        sb.Append(words1[u]);
                    else
                        sb.Append(words2[t - 2] + words0[u]);
                }
                if (i != 0) sb.Append(words3[i - 1]);
            }
            return sb.ToString().TrimEnd();
        }


        /// <summary>
        /// Single PDF
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public ActionResult InvoiceSinglePrint(long OrderId)
        {
            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
            {
                try
                {
                    Orders ord = new Orders();
                    OrdersService ordser = new OrdersService();
                    ord = ordser.GetOrdersById(OrderId);
                    if (ord != null)
                    {
                        if (ord.InvoiceStatus == null || ord.InvoiceStatus == "YetToGenerate")
                            ord.InvoiceStatus = "Generated";
                        ordser.SaveOrUpdateOrder(ord);
                    }
                    SingleInvoice singleInvoice = SingleInvoiceListPDF(OrderId);
                    FillImageUrl(singleInvoice, "main_logo.jpg");

                    return this.ViewPdf("", "SingleInvoice", singleInvoice, "Landscape");

                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                    throw ex;
                }
            }
        }

        public decimal CheckAuthorizedCMRisValid(CMRMaster cmr)
        {
            try
            {
                if (cmr == null)
                {
                    decimal valid = 0;
                    return valid;
                }
                else
                {
                    decimal valid = Convert.ToDecimal(cmr.Price);
                    return valid;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void FillImageUrl(SingleInvoice singleInvoice, string imageName)
        {
            //string url = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));

            //string curpath = "D:\\HDMSInbox";
            //string mailbox = String.Format("{0}\\" + DateTime.Now.ToString("dd/MM/yyyy") + "Inbox", curpath);
            //// If the folder is not existed, create it.
            //if (!Directory.Exists(mailbox)) { Directory.CreateDirectory(mailbox); }
            string curpath1 = ConfigurationManager.AppSettings["AddHeader"] + imageName;

            singleInvoice.ImageUrl = curpath1;
        }

        #region Delivery Reports

        public ActionResult PrintDeliveryReports(long DeliveryNoteId)
        {

            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
            {
                try
                {
                    MastersService mssvc = new MastersService();
                    OrdersService orderService = new OrdersService();
                    criteria.Add("DeliveryNoteId", DeliveryNoteId);
                    DeliveryNote deliverynote = new DeliveryNote();
                    Dictionary<long, IList<DeliveredPODItems_vw>> delitems = orderService.GetDeliveredPODItemsListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                    if (delitems != null && delitems.Count > 0 && delitems.FirstOrDefault().Key > 0)
                    {
                        deliverynote.DeliveredPODItems = delitems.Values.FirstOrDefault();
                        orderService = new OrdersService();
                        Orders Orders = new Orders();
                        //  Orders = orderService.GetOrdersById(delitems.Values.FirstOrDefault()[0].OrderId);
                        // get the data for the orders table
                        criteria.Clear();
                        criteria.Add("OrderId", delitems.Values.FirstOrDefault()[0].OrderId);
                        Dictionary<long, IList<Orders>> orditems = orderService.GetOrdersListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                        if (orditems != null && orditems.FirstOrDefault().Value.Count > 0 && orditems.FirstOrDefault().Key > 0)
                        {
                            deliverynote.DeliveryWeek = orditems.FirstOrDefault().Value[0].Week.ToString();
                            deliverynote.UnitControlNo = orditems.FirstOrDefault().Value[0].ControlId;
                            deliverynote.Period = orditems.FirstOrDefault().Value[0].Period + "-" + orditems.FirstOrDefault().Value[0].PeriodYear;
                            deliverynote.ContingentStrength = orditems.FirstOrDefault().Value[0].Troops;
                            deliverynote.DOS = "7";
                            deliverynote.ManDays = (Int32)orditems.FirstOrDefault().Value[0].Troops * 7;
                            deliverynote.ShipmentDate = DateTime.Now.ToString("dd-MMM-yyyy");

                            // get the data from the ContingentMaster table
                            criteria.Clear();
                            criteria.Add("ContingentControlNo", orditems.FirstOrDefault().Value[0].Name);
                            criteria.Add("TypeofUnit", orditems.FirstOrDefault().Value[0].ContingentType);
                            criteria.Add("LocationCode", orditems.FirstOrDefault().Value[0].Location);
                            Dictionary<long, IList<ContingentMaster>> ContingentList = mssvc.GetContigentListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                            if (ContingentList != null && ContingentList.FirstOrDefault().Value.Count > 0 && ContingentList.FirstOrDefault().Key > 0)
                            {
                                deliverynote.WareHouse = ContingentList.FirstOrDefault().Value[0].SectorName;

                            }

                            DeliveryNote delnote = orderService.GetDeliveryNoteById(DeliveryNoteId);
                            if (delnote != null)
                            {
                                deliverynote.DeliveryNoteName = delnote.DeliveryNoteName;
                            }
                        }
                        FillImageUrlForDelivery(deliverynote, "main_logo.jpg");
                        return this.ViewPdf("Header", "PrintDeliveryReports", deliverynote, "Landscape");
                    }
                    else
                    {
                        return JavaScript(null);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                    throw ex;
                }
            }

        }

        private void FillImageUrlForDelivery(DeliveryNote invoiceList, string imageName)
        {
            string url = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
            invoiceList.ImageUrl = url + "Images/" + imageName;
        }
        #endregion end Delivery Reports

        public ActionResult UpdateInvoiceQtyandValueInOrderItems(long OrderId)
        {
            try
            {
                OrdersService ordser = new OrdersService();
                Orders ord = new Orders();
                if (OrderId != null && OrderId > 0)
                {
                    Dictionary<string, object> criteria = new Dictionary<string, object>();
                    criteria.Add("OrderId", OrderId);
                    Dictionary<long, IList<OrderItems>> orditems = ordser.GetOrderItemsListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                    if (orditems.FirstOrDefault().Value.Count > 0)
                    {
                        for (int i = 0; i < orditems.FirstOrDefault().Value.Count; i++)
                        {
                            if ((orditems.FirstOrDefault().Value[i].OrderQty) * (decimal)1.02 < orditems.FirstOrDefault().Value[i].DeliveredOrdQty)
                            {



                            }

                        }

                    }
                    return null;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }

        }

        public SingleInvoice SingleInvoiceListPDF(long OrderId)
        {
            try
            {
                //OrderId = 9970;
                Orders Ord = OS.GetOrdersById(OrderId);
                DateTime ApprovedDeliverydate = IS.GetApprovedDeliveryDateById(OrderId);
                criteria.Clear();
                criteria.Add("OrderId", OrderId);
                Dictionary<long, IList<SingleInvoiceView>> SingleInvoice2 = IS.GetSingleInvoiceListWithPagingAndCriteria(0, 9999, "UNCode", "Asc", criteria);
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
                        sw.DiscrepancyCode = " ";
                        sw.UOM = UomStr[0];
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
                criteria.Clear();
                criteria.Add("OrderId", OrderId);
                Dictionary<long, IList<SubReplacementView>> SubReplaceList = IS.GetSubstitudeReplacementList(0, 9999, "UNCode", string.Empty, criteria);
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
                        else { SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode = "AR"; }
                    }
                    else if (SingleInvoice1.FirstOrDefault().Value[i].UNCode == 1129)
                    {
                        SingleInvoice1.FirstOrDefault().Value[i].InvoiceQty = SingleInvoice1.FirstOrDefault().Value[i].DeliveredOrdQty;

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
                        }
                        else
                        {
                            TempEggOrderedQtySum = TempEggOrderedQtySum + SingleInvoice1.First().Value[i].OrderQty;
                            TempEggDeliveredQtySum = TempEggDeliveredQtySum + SingleInvoice1.First().Value[i].DeliveredOrdQty;
                            TempEggAcceptedQtySum = (TempEggAcceptedQtySum + SingleInvoice1.First().Value[i].AcceptedOrdQty) * Convert.ToDecimal(0.0625);
                            TempEggInvoiceQtySum = (TempEggInvoiceQtySum + SingleInvoice1.First().Value[i].InvoiceQty) * Convert.ToDecimal(0.0625);
                        }
                    }
                }

                decimal OrderedQtySum = TemOrderedQtySum;
                decimal DeliveredQtySum = TempDeliveredQtySum;
                decimal AcceptedQtySum = TempAcceptedQtySum;
                decimal InvoiceQtySum = TempInvoiceQtySum;

                decimal NetAmountSum = SingleInvoiceList1.Sum(x => x.NetAmt);
                decimal OrdervalueSum = SingleInvoiceList1.Sum(x => x.OrderValue);
                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        if ((SingleInvoice1.First().Value[i].DiscrepancyCode != "AS") && (SingleInvoice1.First().Value[i].DiscrepancyCode != "AR") && (SingleInvoice1.First().Value[i].APLWeight) >= ((decimal)98))
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
                decimal EggOrderedQtySum = TempEggOrderedQtySum * (decimal)0.0625;
                decimal EggDeliveredQtySum = TempEggDeliveredQtySum * (decimal)0.0625;
                decimal EggAcceptedQtySum = TempEggAcceptedQtySum * (decimal)0.0625;
                decimal EggInvoiceQtySum = TempEggInvoiceQtySum;

                decimal TotalOrderedQtySum = OrderedQtySum + EggOrderedQtySum;
                decimal TotalDeliveredQtySum = DeliveredQtySum + EggDeliveredQtySum;
                decimal TotalAcceptedQtySum = AcceptedQtySum + EggAcceptedQtySum;
                decimal TotalInvoiceQtySum = InvoiceQtySum + EggInvoiceQtySum;




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
                        else if (discCode == "NPA 01/02") { APLweight = (decimal)98.00; }
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
                                    TempINVQty = List.InvoiceQty;
                                }
                                else if (List.DiscrepancyCode == "AR")
                                {
                                    if ((OrderQty * (decimal)1.02) < DelQty)
                                        List.InvoiceQty = (OrderQty * (decimal)1.02) - TempINVQty;
                                    if (List.SubstituteItemCode != 0)
                                        List.AcceptedAmt = List.InvoiceQty * List.SubstituteSectorPrice;
                                    else
                                        List.AcceptedAmt = List.InvoiceQty * List.SectorPrice;
                                    List.APLWeight = 0;
                                }
                                else
                                {
                                    if ((OrderQty * (decimal)1.02) < DelQty)
                                        List.InvoiceQty = (OrderQty * (decimal)1.02) - TempINVQty;
                                    if (List.SubstituteItemCode != 0 && List.SectorPrice < List.SubstituteSectorPrice)
                                        List.AcceptedAmt = List.InvoiceQty * List.SectorPrice;
                                    else
                                        List.AcceptedAmt = List.InvoiceQty * List.SubstituteSectorPrice;
                                    List.APLWeight = 0;
                                }
                            }

                        }
                    }
                }
                #endregion
                #region DeliveryList Change for Existing items has AS And AR



                var UncodeList2 = (from items in DeliveryList
                                   where items.SubstituteItemCode != 0 && (items.DiscCode == "SP" || items.DiscCode == "RP")
                                   select new { items.UNCode, items.SubstituteItemCode }).Distinct().ToArray();

                List<long> tempList = new List<long>();
                foreach (var item in UncodeList2)
                {
                    var tempcount = (from items in DeliveryList
                                  where items.SubstituteItemCode ==  item.SubstituteItemCode && items.UNCode == item.UNCode
                              select new {items.UNCode,items.SubstituteItemCode}).Count();
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
                        else if (discCode == "NPA 01/02") { APLweight = (decimal)98.00; }
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
                                    if ((OrderQty * (decimal)1.02) < DelQty)
                                        List.InvoiceQty = (OrderQty * (decimal)1.02) - TempINVQty;
                                    if (List.SubstituteItemCode != 0)
                                        List.AcceptedAmt = List.InvoiceQty * List.SubstituteSectorPrice;
                                    else
                                        List.AcceptedAmt = List.InvoiceQty * List.SectorPrice;
                                    List.APLWeight = APLweight;
                                    flag = false;
                                }
                                else if (List.DiscrepancyCode == "AS" && flag == true)
                                {
                                    if ((OrderQty * (decimal)1.02) < DelQty)
                                        List.InvoiceQty = (OrderQty * (decimal)1.02) - TempINVQty;
                                    if (List.SubstituteItemCode != 0 && List.SectorPrice < List.SubstituteSectorPrice)
                                        List.AcceptedAmt = List.InvoiceQty * List.SectorPrice;
                                    else
                                        List.AcceptedAmt = List.InvoiceQty * List.SubstituteSectorPrice;
                                    List.APLWeight = APLweight;
                                    flag = false;
                                }
                                else {
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
                //Substitution Calculation

                decimal SDeliveryQuantity = (decimal)0.00;
                decimal SOrderedQuantity = (decimal)0.00;
                decimal SAcceptedQuantity = (decimal)0.00;
                decimal SAcceptedamt = (decimal)0.00;
                long SubAPlCount = 0;
                long SubstituteCount = 0;

                //LINQ for Distinct and take count
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
                }

                if (SDeliveryList != null && SDeliveryList.Count > 0)
                {

                    SDeliveryQuantity = SDeliveryList.Sum(item => item.DeliveredQty);
                    SOrderedQuantity = SDeliveryList.Sum(item => item.OrderedQty);
                    SAcceptedQuantity = SDeliveryList.Sum(item => item.InvoiceQty);
                    SAcceptedamt = SDeliveryList.Sum(item => item.AcceptedAmt);
                }

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
                }
                if (RDeliveryList != null && RDeliveryList.Count > 0)
                {

                    RDeliveryQuantity = RDeliveryList.Sum(item => item.DeliveredQty);
                    ROrderedQuantity = RDeliveryList.Sum(item => item.OrderedQty);
                    RAcceptedQuantity = RDeliveryList.Sum(item => item.InvoiceQty);
                    RAcceptedamt = RDeliveryList.Sum(item => item.AcceptedAmt);
                }

                //decimal NumberOfSubstitutions = 0;
                long NumberOfDaysDelay = Convert.ToInt64((ApprovedDeliverydate - (DateTime)Ord.ExpectedDeliveryDate).TotalDays);
                long TotalLineItemsOrdered = SingleInvoiceList.Count;
                CMRMaster cmr = MS.GetCMRMasterBySectorCode(Ord.Sector);
                decimal AuthorizedCMR = CheckAuthorizedCMRisValid(cmr);
                decimal TotalManDays = 7 * Ord.Troops;
                decimal OrderCMR = (OrdervalueSum / TotalManDays);//Modified on 05/05
                decimal AcceptedCMR = ((NetAmountSum + SAcceptedamt + RAcceptedamt) / TotalManDays);//Modified on 05/05
                decimal ActualCMR = Convert.ToDecimal(string.Format("{0:0.000}", OrderCMR));//Created on 05/05
                decimal CMRUtilized = Convert.ToDecimal(string.Format("{0:0.000}", (AcceptedCMR / OrderCMR) * 100));



                criteria.Clear();
                criteria.Add("ContingentControlNo", Ord.Name);
                criteria.Add("LocationCode", Ord.Location);
                criteria.Add("SectorCode", Ord.Sector);
                criteria.Add("TypeofUnit", Ord.ContingentType);
                Dictionary<long, IList<ContingentMaster>> ContingentList = MS.GetContigentListWithPagingAndCriteria(0, 9999, "UNCode", string.Empty, criteria);


                PenaltyCaculation pcl = new PenaltyCaculation();
                pcl.TotalDays = Convert.ToInt64((ApprovedDeliverydate - (DateTime)Ord.ExpectedDeliveryDate).TotalDays);
                pcl.TotalLineitem = Convert.ToInt64(Ord.LineItemsOrdered);
                pcl.TotalLineitem98 = Math.Round(TempAboveAplWeight, 3) + SubAPlCount;
                pcl.InvoiceQty = Math.Round((TotalInvoiceQtySum + SAcceptedQuantity + RAcceptedQuantity), 3);
                pcl.OrderedQty = Math.Round(TotalOrderedQtySum, 3);
                pcl.SubstituteCount = SubstituteCount;
                pcl.OrdervalueSum = Math.Round(OrdervalueSum, 3);

                PenaltyCaculation Pc = GetPenaltyCalculationValues(pcl);

                //PerformanceCalculateView Pc = IS.GetPerformanceCalculateById(OrderId);

                int RefNo = GetControlidInSeries(Ord);
                int no = GetSerialNoforPeriod(Ord.Period, Ord.PeriodYear);
                string SectorNo = "";
                if (Ord.Sector == "SN")
                    SectorNo = "-090-";
                else if (Ord.Sector == "SS")
                    SectorNo = "-091-";
                else
                    SectorNo = "-092-";

                #region Deliverynotes names
                string a = string.Join(",", DeliverySingleList.Select(p => p.DeliveryNote.ToString()));
                string b = string.Join(",", SDeliveryList.Select(p => p.DeliveryNoteName.ToString()));
                string c = string.Join(",", RDeliveryList.Select(p => p.DeliveryNoteName.ToString()));
                string result = a + "," + b + "," + c;
                string[] myArray = result.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string[] ResultAr = myArray.Distinct().ToArray();
                string DeliveryNotes = "";

                if (ResultAr.Count() > 1)
                    DeliveryNotes = String.Join(",", ResultAr);
                else
                    DeliveryNotes = ResultAr[0];
                #endregion

                SingleInvoice si = new SingleInvoice();
                si.Reference = "GCC-" + Ord.Sector + SectorNo + Ord.ContingentType + "-" + no + "/" + RefNo;
                si.DeliveryPoint = ContingentList.FirstOrDefault().Key > 0 ? ContingentList.FirstOrDefault().Value[0].DeliveryPoint : "";
                si.UNID = Ord.ControlId;
                si.Strength = Math.Round(Ord.Troops);
                si.ManDays = Math.Round(TotalManDays);
                si.ApplicableCMR = Math.Round(OrderCMR, 2);
                si.AuthorizedCMR = AuthorizedCMR;
                si.Period = Ord.Period + "/" + Ord.PeriodYear;
                si.DOS = 7;
                si.DeliveryWeek = Ord.Week.ToString();
                si.DeliveryMode = ContingentList.FirstOrDefault().Key > 0 ? ContingentList.FirstOrDefault().Value[0].DeliveryMode : "";
                si.ApprovedDelivery = ConvertDateTimeToDate(Ord.ExpectedDeliveryDate.ToString(), "en-GB");
                si.ActualDeliveryDate = ConvertDateTimeToDate(ApprovedDeliverydate.ToString(), "en-GB");
                //deliveryWithloutOrders = deliveryWithoutOrders.First().Value.ToList(),
                si.DeliveryDetails = DeliverySingleList;
                //Substitution
                si.SDeliveryQuantity = SDeliveryQuantity;
                si.SOrderedQuantity = SOrderedQuantity;
                si.SAcceptedQuantity = SAcceptedQuantity;

                si.SAcceptedamt = Math.Round(SAcceptedamt, 3);

                si.SDeliveryList = SDeliveryList;
                //Replacement
                si.RDeliveryQuantity = RDeliveryQuantity;
                si.ROrderedQuantity = ROrderedQuantity;
                si.RAcceptedQuantity = RAcceptedQuantity;

                si.RAcceptedamt = Math.Round(RAcceptedamt, 3);

                si.RDeliveryList = RDeliveryList;
                si.TotalDays = Convert.ToInt64((ApprovedDeliverydate - (DateTime)Ord.ExpectedDeliveryDate).TotalDays);

                si.OrderedQtySum = Math.Round(OrderedQtySum, 3);

                si.DeliveredQtySum = Math.Round(DeliveredQtySum, 3);
                si.AcceptedQtySum = Math.Round(AcceptedQtySum, 3);
                si.InvoiceQtySum = Math.Round(InvoiceQtySum, 3);

                si.NetAmountSum = Math.Round(NetAmountSum, 3);

                si.OrdervalueSum = Math.Round(OrdervalueSum, 3);
                si.EggOrderedQtySum = Math.Round(EggOrderedQtySum, 3);
                si.EggDeliveredQtySum = Math.Round(EggDeliveredQtySum, 3);
                si.EggAcceptedQtySum = Math.Round(EggAcceptedQtySum, 3);
                si.EggInvoiceQtySum = Math.Round(EggInvoiceQtySum, 3);

                si.TotalDeliveredQtySum = Math.Round(TotalDeliveredQtySum, 3);
                si.TotalAcceptedQtySum = Math.Round(TotalAcceptedQtySum, 3);
                si.TotalInvoiceQtySum = Math.Round((TotalInvoiceQtySum), 3);

                si.AboveCount = Math.Round(TempAboveAplWeight, 3);
                si.BelowCount = Math.Round(TempBelowAplWeight, 3);

                si.CountPercent = Math.Round((TempAboveAplWeight / (TempAboveAplWeight + TempBelowAplWeight)) * 100, 3);

                si.DeliveryPerformance = Math.Round(Pc.DeliveryPerformance, 3);
                si.LineItemPerformance = Math.Round(Pc.LineItemPerformance, 3);
                si.OrderWightPerformance = Math.Round(Pc.OrderWeightPerformance, 3);
                si.SubtitutionPerformance = Math.Round(Pc.ComplaintsPerformance, 3);
                si.DeliveryDeduction = Math.Round(Pc.DeliveryDeduction, 3);
                si.LineItemDeduction = Math.Round(Pc.LineItemDeduction, 3);
                si.OrderWightDeduction = Math.Round(Pc.OrderWeightDeduction, 3);
                si.SubtitutionDeduction = Math.Round(Pc.ComplaintsDeduction, 3);

                si.TotalLineitem98 = Math.Round(TempAboveAplWeight, 3) + SubAPlCount;
                si.SubstituteCount = SubstituteCount;
                si.TotalOrderedQtySum = Math.Round(TotalOrderedQtySum, 3);

                si.AmountSubstituted = SDeliveryList.Sum(item => item.AcceptedAmt);
                si.OrderCMR = OrderCMR;
                si.AcceptedCMR = AcceptedCMR;
                si.CMRUtilized = CMRUtilized;
                si.Deliverynotes = DeliveryNotes;
                si.ApprovedDeliveryDate = ApprovedDeliverydate;

                InvoiceReports Ir = IS.GetInvoiceReportsDetailsByOrderId(OrderId);
                InvoiceReports Newrep = new InvoiceReports();
                if (Ir != null)
                    SaveOrUpdateOrderitemsReports(Ord, si, Ir);
                else
                    SaveOrUpdateOrderitemsReports(Ord, si, Newrep);
                return si;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }



        public InvoiceList ConsolidateInvoiceListPDF(string Period, string searchItems)
        {
            try
            {

                if ((Period != null) && (searchItems != null))
                {
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
                    }
                    Dictionary<long, IList<InvoiceManagementView>> invoiceItems = IS.GetInvoiceListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);

                    var orderList = (from items in invoiceItems.First().Value
                                     select items.OrderId).OrderBy(i => i).Distinct().ToArray();

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

                    decimal TotalDetection = TotDelivery + TotOrderLineItems + TotOrderByWeight + TotComplainAvalability;

                    if (invoiceItems.First().Key != 0)
                    {
                        // Written by Felix kinoniya
                        decimal subTotal = 0;
                        decimal grandTotal = 0;
                        IList<PeriodWeek> periodListVal = GetPeriodListVal(Items[0], Items[2], Items[3], Items[4]);// sector,ContingentType,period
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

                        grandTotal = Math.Round((grandTotal - TotalDetection), 2);
                        var values = grandTotal.ToString(CultureInfo.InvariantCulture).Split('.');
                        int firstValue = int.Parse(values[0]);
                        int secondValue = int.Parse(values[1]);
                        Dictionary<long, IList<SectorMaster>> SectorList = null;
                        criteria.Clear();
                        criteria.Add("SectorCode", Items[0]);
                        SectorList = MS.GetSectorMasterListWithLikeSearchPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                        var tempSector = SectorList.FirstOrDefault().Value[0].Description;

                        DateTime Invdt = invoiceItems.FirstOrDefault().Value[0].InvoiceDate;

                        int no = GetSerialNoforPeriod(Period, Items[4]);
                        string SectorNo = "";
                        string SectorName = "";
                        if (Items[0] == "SN")
                        {
                            SectorNo = "-090-";
                            SectorName = "Sector North";
                        }
                        else if (Items[0] == "SS")
                        {
                            SectorNo = "-091-";
                            SectorName = "Sector South";
                        }
                        else
                        {
                            SectorNo = "-092-";
                            SectorName = "Sector West";
                        }

                        //DateTime dt = DateTime.Parse("1/12/2014");

                        return new InvoiceList()
                        {
                            InvoiceNo = "GCC-" + Items[0] + SectorNo + Items[2] + "-0" + no,
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
                            SubTotal = string.Format("{0:C}", (grandTotal)),
                            GrandTotal = string.Format("{0:C}", (grandTotal)),
                            Usd_words = NumberToText(Convert.ToInt64(firstValue)) + " " + "Dollars" + " " + secondValue + "/100",
                            Delivery = string.Format("{0:C}", TotDelivery),
                            OrderLineItems = string.Format("{0:C}", TotOrderLineItems),
                            OrderByWeight = string.Format("{0:C}", TotOrderByWeight),
                            ComplainAvalability = string.Format("{0:C}", TotComplainAvalability),
                            PeriodWeek = periodListValMain
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

        public int GetControlidInSeries(Orders Ord)
        {
            try
            {
                criteria.Clear();
                criteria.Add("Sector", Ord.Sector);
                criteria.Add("ContingentType", Ord.ContingentType);
                criteria.Add("Period", Ord.Period);
                criteria.Add("PeriodYear", Ord.PeriodYear);
                Dictionary<long, IList<Orders>> Orderlist = OS.GetOrdersListWithPagingAndCriteria(0, 9999, "ControlId", string.Empty, criteria);
                var ControlId = (from items in Orderlist.First().Value
                                 select items.ControlId).OrderBy(i => i).Distinct().ToArray();
                int cnt = 1;
                foreach (var item in ControlId)
                {
                    if (item != Ord.ControlId)
                    {
                        cnt = cnt + 1;
                    }
                    else
                        break;
                }
                return cnt;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }



        public ActionResult PDFDocuments()
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

        public ActionResult PDFDocumentListJQGrid(string PDFState, string searchItems, int rows, string sidx, string sord, int? page = 1)
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
                            if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Period", Items[3]); }
                            if (!string.IsNullOrWhiteSpace(Items[4])) { criteria.Add("PeriodYear", Items[4]); }
                            if (!string.IsNullOrWhiteSpace(PDFState) && PDFState == "PDF-Single") { criteria.Add("DocumentType", "PDF-Single"); }
                            if (!string.IsNullOrWhiteSpace(PDFState) && PDFState == "PDF-Consol") { criteria.Add("DocumentType", "PDF-Consol"); }
                        }
                        Dictionary<long, IList<PDFDocuments>> DocumentItems = IS.GetPDFDocumentListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                        if (DocumentItems != null && DocumentItems.Count > 0)
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
                                            String.Format(@"<a style='color:#034af3;text-decoration:underline' href= '/PdfGeneration/PDFDownload?Id="+items.Id+"' target='_Blank' >{0}</a>",items.DocumentName)
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

        public ActionResult PDFDownload(long Id)
        {
            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
            {
                try
                {
                    PDFDocuments pd = IS.GetPDFDocumentsDetailsById(Id);
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + pd.ControlId + ".pdf");
                    Response.BinaryWrite(pd.DocumentData);
                    Response.End();

                    SaveDocumentToRecentDownloads(null, pd, string.Empty, null, string.Empty);

                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        /// <summary>
        /// Accessing Methods Related to Invoice PDF
        /// </summary>
        /// <param name="OrderId"></param>
        public void CallInvoicePdfParallel(long OrderId)
        {
            try
            {
                Orders ord = new Orders();
                OrdersService ordser = new OrdersService();
                ord = ordser.GetOrdersById(OrderId);
                if (ord != null)
                {
                    if (ord.InvoiceStatus == null || ord.InvoiceStatus == "YetToGenerate")
                        ord.InvoiceStatus = "Generated";
                    ordser.SaveOrUpdateOrder(ord);
                }
                SingleInvoice singleInvoice = SingleInvoiceListPDFParallel(OrderId);
                //SingleInvoice singleInvoice = CreateSingleInvoiceList(OrderId);
                //UpdateInvoiceQtyandValueInOrderItems(OrderId);
                FillImageUrl(singleInvoice, "main_logo.jpg");

                base.ViewPdfForParallel("", "SingleInvoice", singleInvoice, "Landscape");

            }
            catch (Exception)
            {
                //ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
            }

        }
        public SingleInvoice SingleInvoiceListPDFParallel(long OrderId)
        {
            try
            {
                //OrderId = 9808;
                Orders Ord = OS.GetOrdersById(OrderId);
                DateTime ApprovedDeliverydate = IS.GetApprovedDeliveryDateById(OrderId);
                criteria.Clear();
                criteria.Add("OrderId", OrderId);
                Dictionary<long, IList<SingleInvoiceView>> SingleInvoice2 = IS.GetSingleInvoiceListWithPagingAndCriteria(0, 9999, "UNCode", "Asc", criteria);
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
                        sw.DiscrepancyCode = " ";
                        sw.UOM = UomStr[0];
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
                criteria.Clear();
                criteria.Add("OrderId", OrderId);
                Dictionary<long, IList<SubReplacementView>> SubReplaceList = IS.GetSubstitudeReplacementList(0, 9999, "UNCode", string.Empty, criteria);
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
                        else { SingleInvoice1.FirstOrDefault().Value[i].DiscrepancyCode = "AR"; }
                    }
                    else if (SingleInvoice1.FirstOrDefault().Value[i].UNCode == 1129)
                    {
                        SingleInvoice1.FirstOrDefault().Value[i].InvoiceQty = SingleInvoice1.FirstOrDefault().Value[i].DeliveredOrdQty;

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
                        }
                        else
                        {
                            TempEggOrderedQtySum = TempEggOrderedQtySum + SingleInvoice1.First().Value[i].OrderQty;
                            TempEggDeliveredQtySum = TempEggDeliveredQtySum + SingleInvoice1.First().Value[i].DeliveredOrdQty;
                            TempEggAcceptedQtySum = (TempEggAcceptedQtySum + SingleInvoice1.First().Value[i].AcceptedOrdQty) * Convert.ToDecimal(0.0625);
                            TempEggInvoiceQtySum = (TempEggInvoiceQtySum + SingleInvoice1.First().Value[i].InvoiceQty) * Convert.ToDecimal(0.0625);
                        }
                    }
                }

                decimal OrderedQtySum = TemOrderedQtySum;
                decimal DeliveredQtySum = TempDeliveredQtySum;
                decimal AcceptedQtySum = TempAcceptedQtySum;
                decimal InvoiceQtySum = TempInvoiceQtySum;

                decimal NetAmountSum = SingleInvoiceList1.Sum(x => x.NetAmt);
                decimal OrdervalueSum = SingleInvoiceList1.Sum(x => x.OrderValue);
                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        if ((SingleInvoice1.First().Value[i].DiscrepancyCode != "AS") && (SingleInvoice1.First().Value[i].DiscrepancyCode != "AR") && (SingleInvoice1.First().Value[i].APLWeight) >= ((decimal)98))
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
                decimal EggOrderedQtySum = TempEggOrderedQtySum * (decimal)0.0625;
                decimal EggDeliveredQtySum = TempEggDeliveredQtySum * (decimal)0.0625;
                decimal EggAcceptedQtySum = TempEggAcceptedQtySum * (decimal)0.0625;
                decimal EggInvoiceQtySum = TempEggInvoiceQtySum;

                decimal TotalOrderedQtySum = OrderedQtySum + EggOrderedQtySum;
                decimal TotalDeliveredQtySum = DeliveredQtySum + EggDeliveredQtySum;
                decimal TotalAcceptedQtySum = AcceptedQtySum + EggAcceptedQtySum;
                decimal TotalInvoiceQtySum = InvoiceQtySum + EggInvoiceQtySum;




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
                        else if (discCode == "NPA 01/02") { APLweight = (decimal)98.00; }
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
                                    TempINVQty = List.InvoiceQty;
                                }
                                else if (List.DiscrepancyCode == "AR")
                                {
                                    if ((OrderQty * (decimal)1.02) < DelQty)
                                        List.InvoiceQty = (OrderQty * (decimal)1.02) - TempINVQty;
                                    if (List.SubstituteItemCode != 0)
                                        List.AcceptedAmt = List.InvoiceQty * List.SubstituteSectorPrice;
                                    else
                                        List.AcceptedAmt = List.InvoiceQty * List.SectorPrice;
                                    List.APLWeight = 0;
                                }
                                else
                                {
                                    if ((OrderQty * (decimal)1.02) < DelQty)
                                        List.InvoiceQty = (OrderQty * (decimal)1.02) - TempINVQty;
                                    if (List.SubstituteItemCode != 0 && List.SectorPrice < List.SubstituteSectorPrice)
                                        List.AcceptedAmt = List.InvoiceQty * List.SectorPrice;
                                    else
                                        List.AcceptedAmt = List.InvoiceQty * List.SubstituteSectorPrice;
                                    List.APLWeight = 0;
                                }
                            }

                        }
                    }
                }
                #endregion
                #region DeliveryList Change for Existing items has AS And AR



                var UncodeList2 = (from items in DeliveryList
                                   where items.SubstituteItemCode != 0 && (items.DiscCode == "SP" || items.DiscCode == "RP")
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
                        else if (discCode == "NPA 01/02") { APLweight = (decimal)98.00; }
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
                                    if ((OrderQty * (decimal)1.02) < DelQty)
                                        List.InvoiceQty = (OrderQty * (decimal)1.02) - TempINVQty;
                                    if (List.SubstituteItemCode != 0)
                                        List.AcceptedAmt = List.InvoiceQty * List.SubstituteSectorPrice;
                                    else
                                        List.AcceptedAmt = List.InvoiceQty * List.SectorPrice;
                                    List.APLWeight = APLweight;
                                    flag = false;
                                }
                                else if (List.DiscrepancyCode == "AS" && flag == true)
                                {
                                    if ((OrderQty * (decimal)1.02) < DelQty)
                                        List.InvoiceQty = (OrderQty * (decimal)1.02) - TempINVQty;
                                    if (List.SubstituteItemCode != 0 && List.SectorPrice < List.SubstituteSectorPrice)
                                        List.AcceptedAmt = List.InvoiceQty * List.SectorPrice;
                                    else
                                        List.AcceptedAmt = List.InvoiceQty * List.SubstituteSectorPrice;
                                    List.APLWeight = APLweight;
                                    flag = false;
                                }
                                else
                                {
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
                //Substitution Calculation

                decimal SDeliveryQuantity = (decimal)0.00;
                decimal SOrderedQuantity = (decimal)0.00;
                decimal SAcceptedQuantity = (decimal)0.00;
                decimal SAcceptedamt = (decimal)0.00;
                long SubAPlCount = 0;
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
                }




                if (SDeliveryList != null && SDeliveryList.Count > 0)
                {

                    SDeliveryQuantity = SDeliveryList.Sum(item => item.DeliveredQty);
                    SOrderedQuantity = SDeliveryList.Sum(item => item.OrderedQty);
                    SAcceptedQuantity = SDeliveryList.Sum(item => item.InvoiceQty);
                    SAcceptedamt = SDeliveryList.Sum(item => item.AcceptedAmt);
                }

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
                }
                if (RDeliveryList != null && RDeliveryList.Count > 0)
                {

                    RDeliveryQuantity = RDeliveryList.Sum(item => item.DeliveredQty);
                    ROrderedQuantity = RDeliveryList.Sum(item => item.OrderedQty);
                    RAcceptedQuantity = RDeliveryList.Sum(item => item.InvoiceQty);
                    RAcceptedamt = RDeliveryList.Sum(item => item.AcceptedAmt);
                }

                //decimal NumberOfSubstitutions = 0;
                long NumberOfDaysDelay = Convert.ToInt64((ApprovedDeliverydate - (DateTime)Ord.ExpectedDeliveryDate).TotalDays);
                long TotalLineItemsOrdered = SingleInvoiceList.Count;
                CMRMaster cmr = MS.GetCMRMasterBySectorCode(Ord.Sector);
                decimal AuthorizedCMR = CheckAuthorizedCMRisValid(cmr);
                decimal TotalManDays = 7 * Ord.Troops;
                decimal OrderCMR = (OrdervalueSum / TotalManDays);//Modified on 05/05
                decimal AcceptedCMR = ((NetAmountSum + SAcceptedamt + RAcceptedamt) / TotalManDays);//Modified on 05/05
                decimal ActualCMR = Convert.ToDecimal(string.Format("{0:0.000}", OrderCMR));//Created on 05/05
                decimal CMRUtilized = Convert.ToDecimal(string.Format("{0:0.000}", (AcceptedCMR / OrderCMR) * 100));



                criteria.Clear();
                criteria.Add("ContingentControlNo", Ord.Name);
                criteria.Add("LocationCode", Ord.Location);
                criteria.Add("SectorCode", Ord.Sector);
                criteria.Add("TypeofUnit", Ord.ContingentType);
                Dictionary<long, IList<ContingentMaster>> ContingentList = MS.GetContigentListWithPagingAndCriteria(0, 9999, "UNCode", string.Empty, criteria);

                PenaltyCaculation pcl = new PenaltyCaculation();
                pcl.TotalDays = Convert.ToInt64((ApprovedDeliverydate - (DateTime)Ord.ExpectedDeliveryDate).TotalDays);
                pcl.TotalLineitem = Convert.ToInt64(Ord.LineItemsOrdered);
                pcl.TotalLineitem98 = Math.Round(TempAboveAplWeight, 3) + SubAPlCount;
                pcl.InvoiceQty = Math.Round((TotalInvoiceQtySum + SAcceptedQuantity + RAcceptedQuantity), 3);
                pcl.OrderedQty = Math.Round(TotalOrderedQtySum, 3);
                pcl.SubstituteCount = SubstituteCount;
                pcl.OrdervalueSum = Math.Round(OrdervalueSum, 3);

                PenaltyCaculation Pc = GetPenaltyCalculationValues(pcl);

                //PerformanceCalculateView Pc = IS.GetPerformanceCalculateById(OrderId);

                int RefNo = GetControlidInSeries(Ord);
                int no = GetSerialNoforPeriod(Ord.Period, Ord.PeriodYear);
                string SectorNo = "";
                if (Ord.Sector == "SN")
                    SectorNo = "-090-";
                else if (Ord.Sector == "SS")
                    SectorNo = "-091-";
                else
                    SectorNo = "-092-";

                #region Deliverynotes names
                string a = string.Join(",", DeliverySingleList.Select(p => p.DeliveryNote.ToString()));
                string b = string.Join(",", SDeliveryList.Select(p => p.DeliveryNoteName.ToString()));
                string c = string.Join(",", RDeliveryList.Select(p => p.DeliveryNoteName.ToString()));
                string result = a + "," + b + "," + c;
                string[] myArray = result.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string[] ResultAr = myArray.Distinct().ToArray();
                string DeliveryNotes = "";

                if (ResultAr.Count() > 1)
                    DeliveryNotes = String.Join(",", ResultAr);
                else
                    DeliveryNotes = ResultAr[0];
                #endregion

                SingleInvoice si = new SingleInvoice();
                si.Reference = "GCC-" + Ord.Sector + SectorNo + Ord.ContingentType + "-" + no + "/" + RefNo;
                si.DeliveryPoint = ContingentList.FirstOrDefault().Key > 0 ? ContingentList.FirstOrDefault().Value[0].DeliveryPoint : "";
                si.UNID = Ord.ControlId;
                si.Strength = Math.Round(Ord.Troops);
                si.ManDays = Math.Round(TotalManDays);
                si.ApplicableCMR = Math.Round(OrderCMR, 2);
                si.AuthorizedCMR = AuthorizedCMR;
                si.Period = Ord.Period + "/" + Ord.PeriodYear;
                si.DOS = 7;
                si.DeliveryWeek = Ord.Week.ToString();
                si.DeliveryMode = ContingentList.FirstOrDefault().Key > 0 ? ContingentList.FirstOrDefault().Value[0].DeliveryMode : "";
                si.ApprovedDelivery = ConvertDateTimeToDate(Ord.ExpectedDeliveryDate.ToString(), "en-GB");
                si.ActualDeliveryDate = ConvertDateTimeToDate(ApprovedDeliverydate.ToString(), "en-GB");
                //deliveryWithloutOrders = deliveryWithoutOrders.First().Value.ToList(),
                si.DeliveryDetails = DeliverySingleList;
                //Substitution
                si.SDeliveryQuantity = SDeliveryQuantity;
                si.SOrderedQuantity = SOrderedQuantity;
                si.SAcceptedQuantity = SAcceptedQuantity;

                si.SAcceptedamt = Math.Round(SAcceptedamt, 3);

                si.SDeliveryList = SDeliveryList;
                //Replacement
                si.RDeliveryQuantity = RDeliveryQuantity;
                si.ROrderedQuantity = ROrderedQuantity;
                si.RAcceptedQuantity = RAcceptedQuantity;

                si.RAcceptedamt = Math.Round(RAcceptedamt, 3);

                si.RDeliveryList = RDeliveryList;
                si.TotalDays = Convert.ToInt64((ApprovedDeliverydate - (DateTime)Ord.ExpectedDeliveryDate).TotalDays);

                si.OrderedQtySum = Math.Round(OrderedQtySum, 3);

                si.DeliveredQtySum = Math.Round(DeliveredQtySum, 3);
                si.AcceptedQtySum = Math.Round(AcceptedQtySum, 3);
                si.InvoiceQtySum = Math.Round(InvoiceQtySum, 3);

                si.NetAmountSum = Math.Round(NetAmountSum, 3);

                si.OrdervalueSum = Math.Round(OrdervalueSum, 3);
                si.EggOrderedQtySum = Math.Round(EggOrderedQtySum, 3);
                si.EggDeliveredQtySum = Math.Round(EggDeliveredQtySum, 3);
                si.EggAcceptedQtySum = Math.Round(EggAcceptedQtySum, 3);
                si.EggInvoiceQtySum = Math.Round(EggInvoiceQtySum, 3);
                si.TotalOrderedQtySum = Math.Round(TotalOrderedQtySum, 3);
                si.TotalDeliveredQtySum = Math.Round(TotalDeliveredQtySum, 3);
                si.TotalAcceptedQtySum = Math.Round(TotalAcceptedQtySum, 3);
                si.TotalInvoiceQtySum = Math.Round((TotalInvoiceQtySum), 3);

                si.AboveCount = Math.Round(TempAboveAplWeight, 3);
                si.BelowCount = Math.Round(TempBelowAplWeight, 3);

                si.CountPercent = Math.Round((TempAboveAplWeight / (TempAboveAplWeight + TempBelowAplWeight)) * 100, 3);
                si.DeliveryPerformance = Math.Round(Pc.DeliveryPerformance, 3);
                si.LineItemPerformance = Math.Round(Pc.LineItemPerformance, 3);
                si.OrderWightPerformance = Math.Round(Pc.OrderWeightPerformance, 3);
                si.SubtitutionPerformance = Math.Round(Pc.ComplaintsPerformance, 3);
                si.DeliveryDeduction = Math.Round(Pc.DeliveryDeduction, 3);
                si.LineItemDeduction = Math.Round(Pc.LineItemDeduction, 3);
                si.OrderWightDeduction = Math.Round(Pc.OrderWeightDeduction, 3);
                si.SubtitutionDeduction = Math.Round(Pc.ComplaintsDeduction, 3);
                si.TotalLineitem98 = Math.Round(TempAboveAplWeight, 3) + SubAPlCount;
                si.SubstituteCount = SubstituteCount;
                si.AmountSubstituted = SDeliveryList.Sum(item => item.AcceptedAmt);
                si.OrderCMR = OrderCMR;
                si.AcceptedCMR = AcceptedCMR;
                si.CMRUtilized = CMRUtilized;
                si.Deliverynotes = DeliveryNotes;
                si.ApprovedDeliveryDate = ApprovedDeliverydate;

                InvoiceReports Ir = IS.GetInvoiceReportsDetailsByOrderId(OrderId);
                InvoiceReports Newrep = new InvoiceReports();
                if (Ir != null)
                    SaveOrUpdateOrderitemsReports(Ord, si, Ir);
                else
                    SaveOrUpdateOrderitemsReports(Ord, si, Newrep);
                return si;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }

        public ActionResult PDFGeneration()
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

        public ActionResult PDFSingleInvoice()
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

        public ActionResult PDFConsolidateInvoice()
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
        public JsonResult GeneratePdf(string searchItems, string Ids)
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
                    Dictionary<long, IList<PDFDocuments>> DocumentItems = IS.GetPDFDocumentListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                    var Orderids = (from items in DocumentItems.First().Value
                                    select items.OrderId).OrderBy(i => i).Distinct().ToArray();
                    foreach (var item in Orderids)
                    {
                        InvoiceSinglePrintToGenerate(item);
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
                    }

                    Dictionary<long, IList<InvoiceManagementView>> invoiceItems = IS.GetInvoiceListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                    if (invoiceItems != null && invoiceItems.FirstOrDefault().Key > 0)
                    {
                        var InvoiceOrderList = (from items in invoiceItems.First().Value
                                                select items.OrderId).OrderBy(i => i).Distinct().ToArray();

                        foreach (var item in InvoiceOrderList)
                        {
                            InvoiceSinglePrintToGenerate(item);
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
        public void InvoiceSinglePrintToGenerate(long OrderId)
        {
            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) { }
            else
            {
                try
                {
                    Orders ord = new Orders();
                    OrdersService ordser = new OrdersService();
                    ord = ordser.GetOrdersById(OrderId);
                    if (ord != null)
                    {
                        if (ord.InvoiceStatus == null || ord.InvoiceStatus == "YetToGenerate")
                            ord.InvoiceStatus = "Generated";
                        ordser.SaveOrUpdateOrder(ord);
                    }
                    SingleInvoice singleInvoice = SingleInvoiceListPDF(OrderId);
                    //SingleInvoice singleInvoice = CreateSingleInvoiceList(OrderId);
                    //UpdateInvoiceQtyandValueInOrderItems(OrderId);
                    FillImageUrl(singleInvoice, "main_logo.jpg");

                    this.ViewPdf("", "SingleInvoice", singleInvoice, "Landscape");


                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                    throw ex;
                }
            }
        }


    }
}






