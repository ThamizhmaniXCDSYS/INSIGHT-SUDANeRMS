using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using INSIGHT.Entities;
using INSIGHT.Entities.DeletedRecordEntities;
using INSIGHT.Entities.InputUploadEntities;
using INSIGHT.Entities.InvoiceEntities;
using INSIGHT.Modal;
using INSIGHT.WCFServices;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace INSIGHT.Controllers
{
    public delegate void LongTimeTask_Delegate();
    public class OrdersController : INSIGHT.Controllers.PDFGeneration.PdfViewController
    {
        IFormatProvider provider = new System.Globalization.CultureInfo("en-IN", true);
        OrdersService orderService = new OrdersService();
        InvoiceService IS = new InvoiceService();
        Dictionary<string, object> criteria = new Dictionary<string, object>();
        //
        // GET: /Orders/

        //public ActionResult Index()
        //{
        //    return View();
        //}

        #region excel import
        public ActionResult ImportOrder()
        {
            //try{
            //    orderService = new OrdersService();
            //    Dictionary<string, object> criteria = new Dictionary<string, object>();
            //    Dictionary<long, IList<Orders>> orditems = orderService.GetOrdersListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
            //    if (orditems != null && orditems.Count > 0)
            //    {
            //        IList<Orders> orderlist = orditems.FirstOrDefault().Value.ToList();
            //        var ExpDelDate = (from u in orderlist select  u.ExpectedDeliveryDate ).ToArray();

            //        ViewBag.ExpectedDeliveryDate = ExpDelDate;
            //    }
            try
            {
                INSIGHT.Entities.User Userobj = (INSIGHT.Entities.User)Session["objUser"];
                if (Userobj == null || (Userobj != null && Userobj.UserId == null))
                { return RedirectToAction("LogOn", "Account"); }
                string loggedInUserId = Userobj.UserId;
                ViewBag.CurrentUser = loggedInUserId;
                return View();

            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }


        }

        [HttpPost]
        public ActionResult ImportOrderFiles(HttpPostedFileBase[] uploadedFile)
        {
            try
            {
                int length = uploadedFile.Length;
                string fileName = string.Empty;
                for (int l = 0; l < length; l++)
                {
                    string path = uploadedFile[l].InputStream.ToString();
                    byte[] imageSize = new byte[uploadedFile[l].ContentLength];
                    uploadedFile[l].InputStream.Read(imageSize, 0, (int)uploadedFile[l].ContentLength);

                    fileName = uploadedFile[l].FileName;
                    string fileExtn = Path.GetExtension(uploadedFile[l].FileName);
                    string fileLocation = ConfigurationManager.AppSettings["ImportedFilePath"].ToString() + DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss").Replace(":", ".") + fileName;
                    uploadedFile[l].SaveAs(fileLocation);
                }
                uploadedFile = null;

                LongTimeTask_Delegate d = null;
                //   d = new LongTimeTask_Delegate(CreateOrderParallel);

                IAsyncResult R = null;
                R = d.BeginInvoke(null, null);

                //Parallel.Invoke(CreateOrderParallel);
                //CreateOrderParallel();
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        [HttpPost]
        public ActionResult ImportOrder(HttpPostedFileBase[] uploadedFile)
        {
            //construct the result string
            //first successful uploaded files, then already exists and error
            StringBuilder retValue = new StringBuilder();

            int success = 0;

            HttpPostedFileBase theFile = HttpContext.Request.Files["uploadedFile"];
            StringBuilder alreadyExists = new StringBuilder();
            StringBuilder ErrorFilename = new StringBuilder();
            StringBuilder UploadedFilename = new StringBuilder();
            if (theFile != null && theFile.ContentLength > 0)
            {
                INSIGHT.Entities.User Userobj = (INSIGHT.Entities.User)Session["objUser"];
                if (Userobj == null || (Userobj != null && Userobj.UserId == null))
                { return RedirectToAction("LogOn", "Account"); }
                string loggedInUserId = Userobj.UserId;
                string fileName = string.Empty;
                int length = uploadedFile.Length;
                for (int l = 0; l < length; l++)
                {
                    try
                    {
                        int AlreadyExistFile = 0;
                        //string path = uploadedFile[l].InputStream.ToString();
                        //byte[] imageSize = new byte[uploadedFile[l].ContentLength];
                        //uploadedFile[l].InputStream.Read(imageSize, 0, (int)uploadedFile[l].ContentLength);
                        string UploadConnStr = string.Empty;
                        fileName = uploadedFile[l].FileName;
                        string fileExtn = Path.GetExtension(uploadedFile[l].FileName);
                        string fileLocation = ConfigurationManager.AppSettings["ImportedFilePath"].ToString() + DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss").Replace(":", ".") + fileName;
                        uploadedFile[l].SaveAs(fileLocation);
                        if (fileExtn == ".xls")
                        {
                            UploadConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\"";
                        }
                        if (fileExtn == ".xlsx")
                        {
                            UploadConnStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";
                        }
                        byte[] UploadedFile = System.IO.File.ReadAllBytes(fileName);

                        OleDbConnection conn = new OleDbConnection();
                        DataTable DtblXcelData = new DataTable();
                        string QeryToGetXcelData = "select * from " + string.Format("{0}${1}", "[Form", "A1:AZ]");
                        conn.ConnectionString = UploadConnStr;
                        conn.Open();
                        OleDbCommand cmd = new OleDbCommand(QeryToGetXcelData, conn);
                        cmd.CommandType = CommandType.Text;
                        OleDbDataAdapter DtAdptrr = new OleDbDataAdapter();
                        DtAdptrr.SelectCommand = cmd;
                        DtAdptrr.Fill(DtblXcelData);
                        conn.Close();
                        if (DtblXcelData.Rows.Count == 0)
                        {
                            return Json(new { success = false, result = "No Rows available in the file to update!!!" }, "text/html", JsonRequestBehavior.AllowGet);
                        }
                        else if (DtblXcelData.Rows.Count > 0)
                        {
                            QeryToGetXcelData = "select * from " + string.Format("{0}${1}", "[Form", "A7:AZ]");
                            conn.ConnectionString = UploadConnStr;
                            conn.Open();
                            cmd = new OleDbCommand(QeryToGetXcelData, conn);
                            cmd.CommandType = CommandType.Text;
                            DtAdptrr = new OleDbDataAdapter();
                            DtAdptrr.SelectCommand = cmd;
                            DtAdptrr.Fill(DtblXcelData);
                            string[] strArray = { "F1", "F2", "F3", "F4", "F5", "UNCode", "Commodity", "Order Qty (kg/lt/ea)", "Sector Price", "Total" };
                            char chrFlag = 'Y';
                            if (DtblXcelData.Columns.Count == strArray.Length)
                            {
                                int j = 0;
                                string[] strColumnsAray = new string[DtblXcelData.Columns.Count];
                                foreach (DataColumn dtColumn in DtblXcelData.Columns)
                                {
                                    strColumnsAray[j] = dtColumn.ColumnName;
                                    j++;
                                }
                                for (int i = 0; i < strArray.Length - 1; i++)
                                {
                                    if (strArray[i].Trim() != strColumnsAray[i].Trim())
                                    {
                                        chrFlag = 'N';
                                        break;
                                    }
                                }
                                if (chrFlag == 'Y')
                                {

                                    int orderid = GetCounterValue("Orders");
                                    Orders ord = new Orders();
                                    IList<OrderItems> Orderitemslist = new List<OrderItems>();

                                    foreach (DataRow Ordline in DtblXcelData.Rows)
                                    {
                                        if (Ordline.ItemArray[0].ToString().Trim() == "Name")
                                        {
                                            ord.Name = Ordline.ItemArray[1].ToString();
                                            ord.ContingentType = ord.Name.Contains("FPU") ? "FPU" : "MIL";
                                        }
                                        if (Ordline.ItemArray[3].ToString().Trim() == "Start Date")
                                        {
                                            string ordstdt = Ordline.ItemArray[4].ToString();
                                            if (!string.IsNullOrEmpty(ordstdt))
                                                ord.StartDate = DateTime.Parse(ordstdt, provider, System.Globalization.DateTimeStyles.NoCurrentDateDefault);
                                        }
                                        if (Ordline.ItemArray[0].ToString().Trim() == "Location")
                                        {
                                            ord.Location = Ordline.ItemArray[1].ToString();
                                        }
                                        if (Ordline.ItemArray[0].ToString().Trim() == "Location")
                                        {
                                            ord.LocationCMR = Convert.ToDecimal(Ordline.ItemArray[2].ToString());

                                        }
                                        if (Ordline.ItemArray[3].ToString().Trim() == "End Date")
                                        {
                                            string ordenddt = Ordline.ItemArray[4].ToString();
                                            if (!string.IsNullOrEmpty(ordenddt))
                                                ord.EndDate = DateTime.Parse(ordenddt, provider, System.Globalization.DateTimeStyles.NoCurrentDateDefault);
                                        }
                                        if (Ordline.ItemArray[0].ToString().Trim() == "Control#")
                                        {
                                            ord.ControlId = Ordline.ItemArray[1].ToString();
                                            //Get previous controlids for validation
                                            Dictionary<string, object> criteria = new Dictionary<string, object>();
                                            Dictionary<long, IList<Orders>> orders = orderService.GetOrdersListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                                            if (orders != null && orders.Count > 0)
                                            {
                                                IList<Orders> orderlist = orders.FirstOrDefault().Value.ToList();
                                                var ControlIdArray = (from u in orderlist select new { u.ControlId }).Distinct().ToArray();
                                                for (int i = 0; i < ControlIdArray.Length; i++)
                                                {
                                                    if (ControlIdArray[i].ControlId == ord.ControlId)
                                                    {
                                                        AlreadyExistFile = AlreadyExistFile + 1;
                                                        alreadyExists.Append(fileName);
                                                    }

                                                }
                                            }
                                            string[] controlIdarray = Ordline.ItemArray[1].ToString().Split('-');
                                            ord.Sector = controlIdarray[1];
                                            ord.Week = Convert.ToInt64(controlIdarray[5].ToString().Replace("WK", ""));
                                            ord.Period = controlIdarray[4];
                                            ord.PeriodYear = controlIdarray[6] + "-" + controlIdarray[7];
                                            ord.CalYear = Convert.ToInt64(controlIdarray[6]);

                                        }
                                        if (Ordline.ItemArray[0].ToString().Trim() == "Control#")
                                        {
                                            ord.ControlCMR = Convert.ToDecimal(Ordline.ItemArray[2].ToString());
                                        }
                                        if (Ordline.ItemArray[3].ToString().Trim() == "Troops #")
                                        {
                                            ord.Troops = Convert.ToDecimal(Ordline.ItemArray[4].ToString());
                                        }
                                        if (Ordline.ItemArray[2].ToString().Trim() == "Total Amount")
                                        {
                                            ord.TotalAmount = Convert.ToDecimal(Ordline.ItemArray[4].ToString());
                                        }
                                        if (Ordline.ItemArray[2].ToString().Trim() == "# Line Items Ordered")
                                        {
                                            ord.LineItemsOrdered = Convert.ToDecimal(Ordline.ItemArray[4].ToString());
                                        }
                                        if (Ordline.ItemArray[2].ToString().Trim() == "Kg Ordered w/o eggs")
                                        {
                                            ord.KgOrderedWOEggs = Convert.ToDecimal(Ordline.ItemArray[4].ToString());
                                        }
                                        if (Ordline.ItemArray[2].ToString().Trim() == "Eggs weight")
                                        {
                                            ord.EggsWeight = Convert.ToDecimal(Ordline.ItemArray[4].ToString());
                                        }
                                        if (Ordline.ItemArray[2].ToString().Trim() == "Total weight")
                                        {
                                            ord.TotalWeight = Convert.ToDecimal(Ordline.ItemArray[4].ToString());
                                        }
                                        if (Ordline["UNCode"].ToString() != "")
                                        {
                                            OrderItems orditem = new OrderItems();
                                            //orditem.OrderId = orderid;
                                            // orditem.LineId = GetCounterValue("OrderItems");
                                            orditem.UNCode = Convert.ToInt64(Ordline["UNCode"].ToString());
                                            orditem.Commodity = Ordline["Commodity"].ToString();
                                            orditem.OrderQty = Convert.ToDecimal(Ordline["Order Qty (kg/lt/ea)"].ToString());
                                            orditem.SectorPrice = Convert.ToDecimal(Ordline["Sector Price"].ToString());
                                            orditem.Total = Convert.ToDecimal(Ordline["Total"].ToString().Replace("$", ""));
                                            orditem.CreatedBy = loggedInUserId;
                                            orditem.CreatedDate = DateTime.Now;
                                            orditem.RemainingOrdQty = Convert.ToDecimal(Ordline["Order Qty (kg/lt/ea)"].ToString());
                                            //  orditem.Status = "Not Delivered";
                                            //string total = Ordline["Total"].ToString().Replace("$", "");
                                            //orditem.Total = Convert.ToDecimal();
                                            if (AlreadyExistFile == 0)
                                            {
                                                Orderitemslist.Add(orditem);
                                                //orderService.SaveOrUpdateOrderItems(orditem);
                                            }
                                        }
                                    }
                                    if (AlreadyExistFile > 0)
                                    {
                                        continue;
                                    }
                                    ord.CreatedBy = loggedInUserId;
                                    ord.CreatedDate = DateTime.Now;
                                    //  ord.OrderId = orderid;
                                    ord.InvoiceStatus = "YetToGenerate";
                                    ord.DocumentData = UploadedFile;

                                    if (AlreadyExistFile == 0)
                                    {
                                        orderService.SaveOrUpdateOrdersUsingSession(ord, Orderitemslist);
                                        //long ordid = orderService.SaveOrUpdateOrder(ord);
                                        //orderService.SaveOrUpdateOrderItemsList(Orderitemslist);

                                        decimal OrderqtyEggsonly = 0;
                                        decimal OrderqtyWithoutEggs = 0;
                                        decimal TotalAmt = 0;

                                        Dictionary<string, object> criteria = new Dictionary<string, object>();
                                        criteria.Add("OrderId", ord.OrderId);
                                        Dictionary<long, IList<OrderItems>> orditems = orderService.GetOrderItemsListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                                        for (int i = 0; i < orditems.FirstOrDefault().Value.Count(); i++)
                                        {
                                            TotalAmt = TotalAmt + (orditems.FirstOrDefault().Value[i].OrderQty * orditems.FirstOrDefault().Value[i].SectorPrice);

                                            if (orditems.FirstOrDefault().Value[i].UNCode == 1129)
                                            {
                                                OrderqtyEggsonly = OrderqtyEggsonly + (orditems.FirstOrDefault().Value[i].OrderQty * (decimal)0.058824);

                                            }

                                            if (orditems.FirstOrDefault().Value[i].UNCode != 1129)
                                            {
                                                OrderqtyWithoutEggs = OrderqtyWithoutEggs + (orditems.FirstOrDefault().Value[i].OrderQty);

                                            }

                                        }
                                        ord.TotalAmount = TotalAmt;
                                        ord.EggsWeight = OrderqtyEggsonly;
                                        ord.KgOrderedWOEggs = OrderqtyWithoutEggs;
                                        ord.TotalWeight = OrderqtyEggsonly + OrderqtyWithoutEggs;
                                        ord.DocumentData = UploadedFile;
                                        orderService.SaveOrUpdateOrder(ord);



                                        UploadedFilename.Append(fileName + ",");
                                    }
                                }
                                else
                                {
                                    ErrorFilename.Append(fileName + ",");
                                }
                            }
                            else
                            {
                                ErrorFilename.Append(fileName + ",");
                            }
                        }
                        success = success + 1;
                    }
                    catch (Exception ex)
                    {
                        ErrorFilename.Append(fileName + ",");
                        // ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                    }
                }
            }
            else
            {
                return Json(new { success = false, result = "You have uploded the empty file. Please upload the correct file." }, "text/html", JsonRequestBehavior.AllowGet);
            }


            if (UploadedFilename != null && !string.IsNullOrEmpty(UploadedFilename.ToString()))
            {
                retValue.Append("-------files uploaded successfully-----------");
                retValue.Append("<br />");
                string[] upfiles = UploadedFilename.ToString().Split(',');
                if (upfiles != null && upfiles.Count() > 0)
                {
                    foreach (string s in upfiles)
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                            retValue.Append(s + ";");
                            retValue.Append("<br />");
                        }

                    }

                    retValue.Append("<br />");
                    retValue.Append("Successfully uploaded files" + Convert.ToInt32(UploadedFilename.ToString().Split(',').Count() - 1));
                    retValue.Append("<br />");
                    //retValue.Append("-----------------------------------------------------");
                }
            }
            if (alreadyExists != null && !string.IsNullOrEmpty(alreadyExists.ToString()))
            {
                retValue.Append("-----------files already exists--------------");
                retValue.Append("<br />");
                string[] existsfiles = alreadyExists.ToString().Split(',');
                if (existsfiles != null && existsfiles.Count() > 0)
                {
                    foreach (string s in existsfiles)
                    { if (!string.IsNullOrEmpty(s)) retValue.Append(s + ";"); retValue.Append("<br />"); }
                    //retValue.Append("-------------------------------------------------");
                }
            }
            if (ErrorFilename != null && !string.IsNullOrEmpty(ErrorFilename.ToString()))
            {
                retValue.Append("-----------error occured Files--------------");
                string[] errfiles = ErrorFilename.ToString().Split(',');
                if (errfiles != null && errfiles.Count() > 0)
                {
                    foreach (string s in errfiles)
                    { if (!string.IsNullOrEmpty(s))retValue.Append(s + ";"); retValue.Append("<br />"); }
                    //retValue.Append("-------------------------------------------------");

                }
            }
            return Json(new { success = true, result = retValue.ToString().Replace(Environment.NewLine, "<br />") }, "text/html", JsonRequestBehavior.AllowGet);


            //MyLabel.Text = sb.ToString().Replace(Environment.NewLine, "<br />");
            return Json(new { success = true, result = retValue.ToString().Replace(Environment.NewLine, "<br />") }, "text/html", JsonRequestBehavior.AllowGet);
        }

        //Method to Get Counter Value based on Table Name
        public int GetCounterValue(string TableName)
        {
            try
            {

                orderService = new OrdersService();
                IList lst = orderService.GetCounterValue(TableName);
                return int.Parse(lst[0].ToString());

            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }


        #endregion
        //Orders jqgrid
        public JsonResult OrdersListJQGrid(long? OrdersId, string searchItems, string ControlId, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                OrdersService us = new OrdersService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();

                if (!string.IsNullOrWhiteSpace(sord) && sord == "desc")
                    sord = "Desc";
                else
                    sord = "Asc";


                if (string.IsNullOrWhiteSpace(searchItems) && string.IsNullOrWhiteSpace(ControlId))
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


                        if (!string.IsNullOrWhiteSpace(Items[1])) { criteria.Add("Name", Items[1]); }
                        if (!string.IsNullOrWhiteSpace(Items[2])) { criteria.Add("Location", Items[2]); }
                        if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Period", Items[3]); }
                        if (!string.IsNullOrWhiteSpace(Items[4])) { criteria.Add("PeriodYear", Items[4]); }

                        var TrimItems = searchItems.Replace(",", "");
                        if (string.IsNullOrWhiteSpace(TrimItems))
                            return null;
                    }
                    else
                    {
                        string userid = (Session["UserId"] == null) ? "" : Session["UserId"].ToString();
                        UserService usrSrv = new UserService();
                        Dictionary<string, object> criteriaUserAppRole = new Dictionary<string, object>();
                        criteriaUserAppRole.Add("UserId", userid);
                        Dictionary<long, IList<UserAppRole>> userAppRole = usrSrv.GetAppRoleForAnUserListWithPagingAndCriteria(0, 1000, string.Empty, string.Empty, criteriaUserAppRole);
                        if (userAppRole != null && userAppRole.Count > 0 && userAppRole.First().Key > 0)
                        {
                            int count = userAppRole.First().Value.Count;
                            //if it has values then for each concatenate APP+ROLE
                            string[] sectorCodeArr = new string[count];
                            string[] contingentCodeArr = new string[count];
                            string[] locationArr = new string[count];
                            string[] periodArr = new string[count];
                            int i = 0;
                            foreach (UserAppRole uar in userAppRole.First().Value)
                            {

                                string SectorCode = uar.SectorCode;
                                string ContingentCode = uar.ContingentCode;
                                string location = uar.Location;


                                if (!string.IsNullOrEmpty(SectorCode))
                                {
                                    sectorCodeArr[i] = SectorCode;
                                }
                                if (!string.IsNullOrEmpty(ContingentCode))
                                {
                                    contingentCodeArr[i] = ContingentCode;
                                }
                                if (!string.IsNullOrEmpty(location))
                                {
                                    locationArr[i] = location;
                                }

                                i++;
                            }
                            sectorCodeArr = sectorCodeArr.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                            contingentCodeArr = contingentCodeArr.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                            locationArr = locationArr.Where(x => !string.IsNullOrEmpty(x)).ToArray();

                            criteria.Add("Sector", sectorCodeArr);
                            criteria.Add("Name", contingentCodeArr);
                            criteria.Add("Location", locationArr);

                        }
                    }

                    if (OrdersId != null && OrdersId != 0)
                    {
                        //long[] OrderIds = OrdersId.Split(',').Select(n => Convert.ToInt64(n)).ToArray();
                        criteria.Add("OrderId", OrdersId);

                    }
                    if (!string.IsNullOrWhiteSpace(ControlId)) { criteria.Add("ControlId", ControlId); }
                    #endregion

                    Dictionary<long, IList<Orders>> orditems = us.GetOrdersListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);

                    if (orditems != null && orditems.Count > 0)
                    {
                        long totalrecords = orditems.First().Key;
                        int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                        var jsondat = new
                        {
                            total = totalPages,
                            page = page,
                            records = totalrecords,

                            rows = (from items in orditems.FirstOrDefault().Value
                                    select new
                                    {
                                        i = 2,
                                        cell = new string[] {

                                            items.OrderId.ToString(),
                                            items.ControlId.ToString(),
                                            items.Name.ToString(),
                                            items.Location.ToString(),
                                            items.Period,
                                            items.StartDate!=null? ConvertDateTimeToDate(items.StartDate.ToString("dd/MM/yyyy"),"en-GB"):"",
                                            items.EndDate!=null? ConvertDateTimeToDate(items.EndDate.ToString("dd/MM/yyyy"),"en-GB"):"",
                                            items.Troops.ToString(),
                                            items.TotalAmount.ToString(),
                                            items.LineItemsOrdered.ToString(),
                                            items.KgOrderedWOEggs.ToString(),
                                            items.EggsWeight.ToString(),
                                            items.TotalWeight.ToString(),
                                            items.ExpectedDeliveryDate!=null? ConvertDateTimeToDate(items.ExpectedDeliveryDate.Value.ToString("MM/dd/yyyy"),"en-GB"):"",
                                            items.LocationCMR.ToString(),
                                            items.ControlCMR.ToString(),
                                            items.FinalStatus,
                                            items.OpeningStatus,
                                            items.InvoiceStatus,
                                            items.ModifiedBy,
                                            items.PODId.ToString()
                                            //String.Format(@"<a style='color:#034af3;text-decoration:underline' href= '/Invoice/NewInvoice?OrderId="+items.OrderId+"' >{0}</a>","Inovice"),
                                            //String.Format(@"<a style='color:#034af3;text-decoration:underline' href= '/Orders/GotoPOD?OrderId="+items.OrderId+"' >{0}</a>","POD"),
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
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }

        //Order items jqgrid
        public JsonResult OrderItemsListJQGrid(long OrderId, string LineId, string UNCode, string Commodity, string OrderQty, string AcceptedOrdQty, string DeliveredOrdQty, string RemainingOrdQty, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                Dictionary<string, object> criteria = new Dictionary<string, object>();

                //sord = sord == "desc" ? "Desc" : "Asc";
                criteria.Add("OrderId", OrderId);
                if (!string.IsNullOrWhiteSpace(LineId)) { criteria.Add("LineId", Convert.ToInt64(LineId)); }
                if (!string.IsNullOrWhiteSpace(UNCode)) { criteria.Add("UNCode", Convert.ToInt64(UNCode)); }
                if (!string.IsNullOrWhiteSpace(Commodity)) { criteria.Add("Commodity", Commodity); }
                if (!string.IsNullOrWhiteSpace(OrderQty)) { criteria.Add("OrderQty", Convert.ToDecimal(OrderQty)); }
                if (!string.IsNullOrWhiteSpace(AcceptedOrdQty)) { criteria.Add("AcceptedOrdQty", Convert.ToDecimal(AcceptedOrdQty)); }
                if (!string.IsNullOrWhiteSpace(DeliveredOrdQty)) { criteria.Add("DeliveredOrdQty", Convert.ToDecimal(DeliveredOrdQty)); }
                if (!string.IsNullOrWhiteSpace(RemainingOrdQty)) { criteria.Add("RemainingOrdQty", Convert.ToDecimal(RemainingOrdQty)); }

                sord = sord == "desc" ? "Desc" : "Asc";
                Dictionary<long, IList<OrderItems>> orditems = null;
                if (!string.IsNullOrWhiteSpace(Commodity))
                {
                    orditems = orderService.GetOrderItemsListWithLikeSearchPagingAndCriteria(page - 1, 9999, sidx, sord, criteria);
                }
                else
                {
                    orditems = orderService.GetOrderItemsListWithPagingAndCriteria(page - 1, 9999, sidx, sord, criteria);
                }



                if (orditems != null && orditems.Count > 0)
                {
                    long totalrecords = orditems.First().Key;
                    int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                    var jsondat = new
                    {
                        total = totalPages,
                        page = page,
                        records = totalrecords,

                        rows = (from items in orditems.First().Value
                                select new
                                {
                                    i = 2,
                                    cell = new string[] {
                                        items.LineId.ToString(),
                                        items.OrderId.ToString(),
                                        items.UNCode.ToString(),
                                        items.Commodity.ToString(),
                                        items.OrderQty.ToString(),
                                        items.DeliveredOrdQty==0?"":items.DeliveredOrdQty.ToString(),
                                        items.AcceptedOrdQty==0?"":items.AcceptedOrdQty.ToString(),
                                        items.InvoiceQty==0?"":items.InvoiceQty.ToString(),
                                        items.RemainingOrdQty==0?"":items.RemainingOrdQty.ToString(),
                                        items.SectorPrice.ToString(),
                                        items.Total.ToString(),
                                        items.InvoiceValue==0?"":items.InvoiceValue.ToString(),
                      
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
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }

        //Oder items form
        public ActionResult OrderItemsForm(long OrderId)
        {
            try
            {

                orderService = new OrdersService();
                Orders ord = new Orders();
                ord = orderService.GetOrdersById(OrderId);
                return View(ord);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }

        #region Delivery Note Creation
        public JsonResult DeliveryNoteJQGrid(long OrderId, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                orderService = new OrdersService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();

                if (!string.IsNullOrWhiteSpace(sord) && sord == "desc")
                    sord = "Desc";
                else
                    sord = "Asc";
                criteria.Add("OrderId", OrderId);

                Dictionary<long, IList<DeliveryNote>> delnote = orderService.GetDeliveryNoteListWithPagingAndCriteria(page - 1, 9999, sidx, sord, criteria);
                if (delnote != null && delnote.Count > 0)
                {
                    long totalrecords = delnote.First().Key;
                    int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                    var jsondat = new
                    {
                        total = totalPages,
                        page = page,
                        records = totalrecords,

                        rows = (from items in delnote.First().Value
                                select new
                                {
                                    i = 2,
                                    cell = new string[] {
                                        items.DeliveryNoteId.ToString(),
                                        items.DeliveryNoteName,
                                        "<p tyle='color:#034af3;text-decoration:underline' onclick=\"PDFLink('" + items.DeliveryNoteId+"');\">PDF</>",
                                         items.CreatedDate!=null? ConvertDateTimeToDate(items.CreatedDate.ToString("dd/MM/yyyy"),"en-GB"):"",
                                         items.DeliveryStatus
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
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightDNPolicy");
                throw ex;
            }
        }
        public ActionResult DeliveryNoteList()
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

        //  public ActionResult GetDeliveryNoteListJQGrid(string DeliveryNoteId, string DeliveryNoteName, string ControlId, string Name, string Sector, string Location, string Period, int rows, string sidx, string sord, int? page = 1)

        public ActionResult GetDeliveryNoteListJQGrid(string searchItems, string DeliveryNoteName, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {

                OrdersService us = new OrdersService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();

                if (!string.IsNullOrWhiteSpace(sord) && sord == "desc")
                    sord = "Desc";
                else
                    sord = "Asc";
                #region search criteria

                if (searchItems == null && DeliveryNoteName == null)
                {
                    return null;
                }
                else
                {
                    if (searchItems != null && searchItems != "")
                    {
                        var Items = searchItems.ToString().Split(',');
                        if (!string.IsNullOrWhiteSpace(Items[0])) { criteria.Add("Sector", Items[0]); }


                        if (!string.IsNullOrWhiteSpace(Items[1])) { criteria.Add("Name", Items[1]); }
                        if (!string.IsNullOrWhiteSpace(Items[2])) { criteria.Add("Location", Items[2]); }
                        if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Period", Items[3]); }
                        if (!string.IsNullOrWhiteSpace(Items[4])) { criteria.Add("PeriodYear", Items[4]); }
                        // if (!string.IsNullOrEmpty(DeliveryNoteName)) { criteria.Add("DeliveryNoteName", DeliveryNoteName); }
                        //criteria.Add("Sector", Items[0]);
                        //criteria.Add("Name", Items[1]);
                        //criteria.Add("Location", Items[2]);
                        //criteria.Add("Period", Items[3]);
                        var TrimItems = searchItems.Replace(",", "");
                        if (string.IsNullOrWhiteSpace(TrimItems))
                            return null;

                    }
                    else
                    {
                        string userid = (Session["UserId"] == null) ? "" : Session["UserId"].ToString();
                        UserService usrSrv = new UserService();
                        Dictionary<string, object> criteriaUserAppRole = new Dictionary<string, object>();
                        criteriaUserAppRole.Add("UserId", userid);
                        Dictionary<long, IList<UserAppRole>> userAppRole = usrSrv.GetAppRoleForAnUserListWithPagingAndCriteria(0, 1000, string.Empty, string.Empty, criteriaUserAppRole);
                        if (userAppRole != null && userAppRole.Count > 0 && userAppRole.First().Key > 0)
                        {
                            int count = userAppRole.First().Value.Count;
                            //if it has values then for each concatenate APP+ROLE
                            string[] sectorCodeArr = new string[count];
                            string[] contingentCodeArr = new string[count];
                            string[] locationArr = new string[count];
                            string[] periodArr = new string[count];
                            int i = 0;
                            foreach (UserAppRole uar in userAppRole.First().Value)
                            {

                                string SectorCode = uar.SectorCode;
                                string ContingentCode = uar.ContingentCode;
                                string location = uar.Location;


                                if (!string.IsNullOrEmpty(SectorCode))
                                {
                                    sectorCodeArr[i] = SectorCode;
                                }
                                if (!string.IsNullOrEmpty(ContingentCode))
                                {
                                    contingentCodeArr[i] = ContingentCode;
                                }
                                if (!string.IsNullOrEmpty(location))
                                {
                                    locationArr[i] = location;
                                }

                                i++;
                            }
                            sectorCodeArr = sectorCodeArr.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                            contingentCodeArr = contingentCodeArr.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                            locationArr = locationArr.Where(x => !string.IsNullOrEmpty(x)).ToArray();

                            criteria.Add("Sector", sectorCodeArr);
                            criteria.Add("Name", contingentCodeArr);
                            criteria.Add("Location", locationArr);
                        }
                    }
                }


                #endregion

                //if (!string.IsNullOrEmpty(DeliveryNoteId))
                // criteria.Add("DeliveryNoteId", Convert.ToInt64(DeliveryNoteId));

                //if (!string.IsNullOrEmpty(DeliveryNoteName)) { criteria.Add("DeliveryNoteName", DeliveryNoteName); }

                //if (!string.IsNullOrEmpty(Name))
                // criteria.Add("Name", Name);
                //if (!string.IsNullOrEmpty(Sector))
                // criteria.Add("Sector", Sector);
                //if (!string.IsNullOrEmpty(Location))
                // criteria.Add("Location", Location);
                //if (!string.IsNullOrEmpty(Period))
                // criteria.Add("Period", Period);
                //if (!string.IsNullOrEmpty(ControlId))
                // criteria.Add("ControlId", ControlId);
                //Dictionary<long, IList<DeliveryNoteOrders_vw>> delnote = null;
                //if ((!string.IsNullOrWhiteSpace(DeliveryNoteName)) || (!string.IsNullOrWhiteSpace(ControlId)) || (!string.IsNullOrWhiteSpace(Name)))
                //{
                // delnote = orderService.GetDeliveryNoteOrderListWithLikeSearchPagingAndCriteria(page - 1, 99999, sidx, sord, criteria);
                //}
                //else
                //{
                // delnote = orderService.GetDeliveryNoteOrderListWithPagingAndCriteria(page - 1, 9999, sidx, sord, criteria);
                //}
                Dictionary<long, IList<DeliveryNoteOrders_vw>> delnote = null;
                if (!string.IsNullOrEmpty(DeliveryNoteName))
                {
                    criteria.Add("DeliveryNoteName", DeliveryNoteName);

                    delnote = orderService.GetDeliveryNoteOrderListWithLikeSearchPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                }
                else
                    delnote = orderService.GetDeliveryNoteOrderListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);





                if (delnote != null && delnote.Count > 0)
                {
                    long totalrecords = delnote.First().Key;
                    int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                    var jsondat = new
                    {
                        total = totalPages,
                        page = page,
                        records = totalrecords,

                        rows = (from items in delnote.First().Value
                                select new
                                {
                                    i = 2,
                                    cell = new string[] {
                                         items.Id.ToString(),
                                         items.DeliveryNoteId.ToString(),
                                         items.DeliveryNoteName,
                                         items.DeliveryNoteType,
                                         items.ActualDeliveryDate.ToString()=="1/1/0001 12:00:00 AM"?"":items.ActualDeliveryDate.ToString("dd-MMM-yyyy"),
                                         //items.ActualDeliveryDate==null?"":items.ActualDeliveryDate.ToString("dd-MM-yyyy"),
                                         items.ControlId,
                                         items.Name,
                                         items.Sector,
                                         items.Location,
                                         items.Period,
                                         items.PeriodYear
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
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }




        public ActionResult DeliveryNoteCreation(long? DeliveryNoteId)
        {
            try
            {
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                criteria.Add("DeliveryNoteId", DeliveryNoteId);
                Dictionary<long, IList<DeliveredPODItems_vw>> delitems = orderService.GetDeliveredPODItemsListWithPagingAndCriteria(0, 1, string.Empty, string.Empty, criteria);
                if (delitems != null && delitems.FirstOrDefault().Value.Count > 0 && delitems.FirstOrDefault().Key > 0)
                {
                    ViewBag.DeliveryNoteName = delitems.FirstOrDefault().Value[0].DeliveryNoteName;
                    ViewBag.Name = delitems.FirstOrDefault().Value[0].Name;
                    ViewBag.Week = delitems.FirstOrDefault().Value[0].Week;
                    ViewBag.Sector = delitems.FirstOrDefault().Value[0].Sector;
                    ViewBag.DeliveryNoteId = delitems.FirstOrDefault().Value[0].DeliveryNoteId;
                    ViewBag.Location = delitems.FirstOrDefault().Value[0].Location;
                    ViewBag.PODId = delitems.FirstOrDefault().Value[0].PODId;
                    if (delitems.FirstOrDefault().Value[0].ExpectedDeliveryDate.ToString() != "1/1/0001 12:00:00 AM")
                    {
                        ViewBag.ExpectedDeliveryDate = delitems.FirstOrDefault().Value[0].ExpectedDeliveryDate;
                    }

                    ViewBag.ControlId = delitems.FirstOrDefault().Value[0].ControlId;
                    ViewBag.OrderId = delitems.FirstOrDefault().Value[0].OrderId;
                    INSIGHT.Entities.User Userobj = (INSIGHT.Entities.User)Session["objUser"];

                    string loggedInUserId = Userobj.UserId;
                    ViewBag.AcceptedBy = loggedInUserId;
                    if (delitems.FirstOrDefault().Value[0].DeliveredDate != null)
                    {
                        ViewBag.AcceptedDate = delitems.FirstOrDefault().Value[0].DeliveredDate;
                    }
                }
                //ViewBag.DeliveryNoteId = DeliveryNoteId;
                return View();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }

        public ActionResult DeliveryAndAcceptQtyUpdateJqgrid(long DeliveryNoteId, string DeliveryNoteName, string Name, string Week, string Sector, string Location, string UNCode, string Commodity, int rows, string sidx, string sord, int? page = 1)
        {

            try
            {
                orderService = new OrdersService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                sord = sord == "desc" ? "Desc" : "Asc";
                criteria.Add("DeliveryNoteId", DeliveryNoteId);
                if (!string.IsNullOrWhiteSpace(DeliveryNoteName)) { criteria.Add("DeliveryNoteName", DeliveryNoteName); }
                if (!string.IsNullOrWhiteSpace(Name)) { criteria.Add("Name", Name); }
                if (!string.IsNullOrWhiteSpace(Week)) { criteria.Add("Week", Convert.ToInt64(Week)); }
                if (!string.IsNullOrWhiteSpace(Sector)) { criteria.Add("Sector", Sector); }
                if (!string.IsNullOrWhiteSpace(Location)) { criteria.Add("Location", Location); }
                if (!string.IsNullOrWhiteSpace(UNCode)) { criteria.Add("UNCode", Convert.ToInt64(UNCode)); }

                Dictionary<long, IList<DeliveredPODItems_vw>> delitems = null;
                if (!string.IsNullOrWhiteSpace(Commodity))
                {
                    if (!string.IsNullOrEmpty(Commodity)) { criteria.Add("Commodity", Commodity); }
                    delitems = orderService.GetDeliveredPODItemsListWithLikePagingAndCriteria(page - 1, 9999, sidx, sord, criteria);
                }
                else
                {
                    delitems = orderService.GetDeliveredPODItemsListWithPagingAndCriteria(page - 1, 99999, sord, sidx, criteria);
                }

                if (delitems != null && delitems.Count > 0)
                {
                    long totalrecords = delitems.First().Key;
                    int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                    var jsondat = new
                    {
                        total = totalPages,
                        page = page,
                        records = totalrecords,

                        rows = (from items in delitems.First().Value
                                select new
                                {
                                    i = 2,
                                    cell = new string[] {
                                                items.Id.ToString(),
                                                items.PODItemsId.ToString(),
                                                items.OrderId.ToString(),
                                                items.DeliveryNoteName,
                                                items.Name,
                                                items.LineId.ToString(),
                                                items.Period.ToString(),
                                                items.Week.ToString(),
                                                items.Sector,
                                                items.Location,
                                                items.UNCode.ToString(),
                                                items.Commodity,
                                                items.OrderedQty.ToString(),
                                                items.DeliveredQty.ToString(),
                                                 "<i id=MoveLnk_"+items.Id+" class='icon-white icon-chevron-right move'></i>",
                                                 items.AcceptedQty==(decimal)0.00?"":items.AcceptedQty.ToString(),
                                                items.AcceptedQty.ToString(),
                                                items.RemQtyForAccQty.ToString(),
                                                items.RemainingQty.ToString(),
                                                items.DiscrepancyCode,
                                                items.SubstituteItemCode==0?"":items.SubstituteItemCode.ToString(),
                                                items.SubstituteItemName,
                                                items.Status,
                                                items.DeliveredDate==null?"":items.DeliveredDate.Value.ToString("dd-MM-yyyy"),
                                                items.ExpectedDeliveryDate==null?"":items.ExpectedDeliveryDate.ToString("dd-MM-yyyy"),
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
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }

        public ActionResult UpdateAcceptQtyfromDeliveryPage(DeliveredPODItems_vw delpodvw)
        {
            try
            {
                decimal accqty = 0;
                decimal delqty = 0;
                orderService = new OrdersService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                INSIGHT.Entities.User Userobj = (INSIGHT.Entities.User)Session["objUser"];
                string loggedInUserId = Userobj.UserId;
                PODItems poditems = new PODItems();
                OrderItems orditems = new OrderItems();
                poditems = orderService.GetPodItemsValById(delpodvw.PODItemsId);
                if (poditems != null)
                {
                    poditems.ModifiedBy = loggedInUserId;
                    poditems.ModifiedDate = DateTime.Now;
                    poditems.AcceptedQty = delpodvw.AcceptedQty;
                    //poditems.DeliveredDate = delpodvw.DeliveredDate ?? poditems.DeliveredDate;
                    poditems.Status = "Delivery Accepted";
                    orderService.SaveOrUpdatePODItems(poditems);
                    criteria.Add("LineId", poditems.LineId);
                    Dictionary<long, IList<PODItems>> poditemslist = orderService.GetPODItemsListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                    if (poditemslist != null && poditemslist.Count > 0)
                    {
                        IList<PODItems> podlist = poditemslist.FirstOrDefault().Value.ToList();
                        var AccQtyArray = (from u in podlist select new { u.AcceptedQty, u.DeliveredQty }).ToArray();

                        for (int j = 0; j < AccQtyArray.Length; j++)
                        {
                            accqty = accqty + AccQtyArray[j].AcceptedQty;
                            delqty = delqty + AccQtyArray[j].DeliveredQty;
                        }
                        orditems = orderService.GetOrderItemsById(poditems.LineId);
                        orditems.AcceptedOrdQty = accqty;
                        orditems.DeliveredOrdQty = delqty;
                        orderService.SaveOrUpdateOrderItems(orditems);
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

        public ActionResult CompletedInDeliveryNote(long? DeliveryNoteId)
        {
            try
            {
                int delcnt = 0;
                int podcnt = 0;
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                orderService = new OrdersService();
                criteria.Add("DeliveryNoteId", DeliveryNoteId);
                Dictionary<long, IList<DeliveredPODItems_vw>> delivereditems = orderService.GetDeliveredPODItemsListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                //IList<DeliveredPODItems_vw> DelList = delivereditems.FirstOrDefault().Value;
                if (delivereditems.Count > 0 && delivereditems != null && delivereditems.FirstOrDefault().Key > 0)
                {
                    foreach (var item in delivereditems.FirstOrDefault().Value)
                    {
                        if (item.Status != "DeliveryCompleted")
                        {
                            delcnt = delcnt + 1;
                        }
                    }
                }
                if (delcnt == 0)
                {
                    DeliveryNote delnote = orderService.GetDeliveryNoteById(DeliveryNoteId ?? 0);
                    delnote.DeliveryStatus = "Completed";
                    orderService.SaveOrUpdateDeliveryNote(delnote);
                    criteria.Clear();
                    criteria.Add("OrderId", delnote.OrderId);
                    Dictionary<long, IList<DeliveryNote>> deliverynote = orderService.GetDeliveryNoteListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                    if (deliverynote.Count > 0 && deliverynote != null && deliverynote.FirstOrDefault().Key > 0)
                    {
                        foreach (var item in deliverynote.FirstOrDefault().Value)
                        {
                            if (item.DeliveryStatus != "Completed")
                            {
                                podcnt = podcnt + 1;
                            }
                        }
                    }
                    if (podcnt == 0)
                    {
                        POD po = orderService.GetPODByOrderId(deliverynote.FirstOrDefault().Value[0].OrderId);
                        po.Status = "PODCompleted";
                        orderService.SaveOrUpdatePOD(po);
                        Orders ord = orderService.GetOrdersById(deliverynote.FirstOrDefault().Value[0].OrderId);
                        ord.FinalStatus = "OrderCompleted";
                        orderService.SaveOrUpdateOrder(ord);
                    }

                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
                return null;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }

        }
        #region Micheal
        public ActionResult CheckingDeliveryNote(long? DeliveryNoteId)
        {
            try
            {
                int cnt = 0;
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                orderService = new OrdersService();
                criteria.Add("DeliveryNoteId", DeliveryNoteId);
                Dictionary<long, IList<DeliveredPODItems_vw>> delivereditems = orderService.GetDeliveredPODItemsListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                //IList<DeliveredPODItems_vw> DelList = delivereditems.FirstOrDefault().Value;
                if (delivereditems.Count > 0 && delivereditems != null && delivereditems.FirstOrDefault().Key > 0)
                {
                    foreach (var item in delivereditems.FirstOrDefault().Value)
                    {
                        if (item.Status != "DeliveryCompleted")
                        {
                            cnt = cnt + 1;
                        }
                    }
                }
                if (cnt == 0)
                {
                    return Json("Success", JsonRequestBehavior.AllowGet);
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
        #endregion

        #endregion End Delivery Note Creation

        #region kingston POD related code
        //PODMaster list page
        public ActionResult PODMasterListPage()
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

        //PODMaster JQgrid

        //public JsonResult PODMasterJQgrid(int rows, string sidx, string sord, int? page = 1)
        //{
        //    try
        //    {
        //        orderService = new OrdersService();
        //        Dictionary<string, object> criteria = new Dictionary<string, object>();

        //        if (!string.IsNullOrWhiteSpace(sord) && sord == "desc")
        //            sord = "Desc";
        //        else
        //            sord = "Asc";

        //        Dictionary<long, IList<PODOrdersList_vw>> podorder = orderService.GetPODOrdersListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);


        //        if (podorder != null && podorder.Count > 0)
        //        {
        //            long totalrecords = podorder.First().Key;
        //            int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
        //            var jsondat = new
        //            {
        //                total = totalPages,
        //                page = page,
        //                records = totalrecords,

        //                rows = (from items in podorder.First().Value
        //                        select new
        //                        {
        //                            i = 2,


        //                            cell = new string[] {
        //                                items.Id.ToString(),

        //                                items.PODId.ToString(),

        //                                items.PODNo,
        //                                items.OrderId.ToString(),
        //                                items.Name,
        //                                items.ContingentType,
        //                                items.Location,
        //                                items.ControlId,
        //                                items.Period,
        //                                items.Sector,
        //                                items.Week.ToString(),
        //                                items.PeriodYear,
        //                                items.CreatedBy,
        //                                items.CreatedDate!=null? ConvertDateTimeToDate(items.CreatedDate.ToString("dd/MM/yyyy"),"en-GB"):"",
        //                                items.ExpectedDeliveryDate!=null? ConvertDateTimeToDate(items.ExpectedDeliveryDate.ToString("dd/MM/yyyy"),"en-GB"):""
        //                                }
        //                        })
        //            };
        //            return Json(jsondat, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            var jsondat = new { rows = (new { cell = new string[] { } }) };
        //            return Json(jsondat, JsonRequestBehavior.AllowGet);

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
        //        throw ex;
        //    }
        //}
        public JsonResult PODMasterJQgrid(long? OrdersId, string searchItems, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {

                orderService = new OrdersService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();

                sord = sord == "desc" ? "Desc" : "Asc";

                #region Added by Micheal
                if (searchItems == null)
                {
                    return null;
                }
                else
                {

                    if (searchItems != null && searchItems != "")
                    {
                        var Items = searchItems.ToString().Split(',');
                        if (!string.IsNullOrWhiteSpace(Items[0])) { criteria.Add("Sector", Items[0]); }
                        if (!string.IsNullOrWhiteSpace(Items[1])) { criteria.Add("Name", Items[1]); }
                        if (!string.IsNullOrWhiteSpace(Items[2])) { criteria.Add("Location", Items[2]); }
                        if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Period", Items[3]); }
                        if (!string.IsNullOrWhiteSpace(Items[4])) { criteria.Add("PeriodYear", Items[4]); }
                        var TrimItems = searchItems.Replace(",", "");
                        if (string.IsNullOrWhiteSpace(TrimItems))
                            return null;
                    }
                    else
                    {
                        string userid = (Session["UserId"] == null) ? "" : Session["UserId"].ToString();
                        UserService usrSrv = new UserService();
                        Dictionary<string, object> criteriaUserAppRole = new Dictionary<string, object>();
                        criteriaUserAppRole.Add("UserId", userid);
                        Dictionary<long, IList<UserAppRole>> userAppRole = usrSrv.GetAppRoleForAnUserListWithPagingAndCriteria(0, 1000, string.Empty, string.Empty, criteriaUserAppRole);
                        if (userAppRole != null && userAppRole.Count > 0 && userAppRole.First().Key > 0)
                        {
                            int count = userAppRole.First().Value.Count;
                            //if it has values then for each concatenate APP+ROLE
                            string[] sectorCodeArr = new string[count];
                            string[] contingentCodeArr = new string[count];
                            string[] locationArr = new string[count];
                            string[] periodArr = new string[count];
                            int i = 0;
                            foreach (UserAppRole uar in userAppRole.First().Value)
                            {

                                string SectorCode = uar.SectorCode;
                                string ContingentCode = uar.ContingentCode;
                                string location = uar.Location;


                                if (!string.IsNullOrEmpty(SectorCode))
                                {
                                    sectorCodeArr[i] = SectorCode;
                                }
                                if (!string.IsNullOrEmpty(ContingentCode))
                                {
                                    contingentCodeArr[i] = ContingentCode;
                                }
                                if (!string.IsNullOrEmpty(location))
                                {
                                    locationArr[i] = location;
                                }

                                i++;
                            }
                            sectorCodeArr = sectorCodeArr.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                            contingentCodeArr = contingentCodeArr.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                            locationArr = locationArr.Where(x => !string.IsNullOrEmpty(x)).ToArray();

                            criteria.Add("Sector", sectorCodeArr);
                            criteria.Add("Name", contingentCodeArr);
                            criteria.Add("Location", locationArr);
                        }
                    }

                    if (OrdersId != null && OrdersId != 0)
                    {
                        //long[] OrderIds = OrdersId.Split(',').Select(n => Convert.ToInt64(n)).ToArray();
                        criteria.Add("OrderId", OrdersId);
                    }

                #endregion

                    Dictionary<long, IList<PODOrdersList_vw>> podorder = orderService.GetPODOrdersListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);


                    if (podorder != null && podorder.Count > 0)
                    {
                        long totalrecords = podorder.First().Key;
                        int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                        var jsondat = new
                        {
                            total = totalPages,
                            page = page,
                            records = totalrecords,

                            rows = (from items in podorder.First().Value
                                    select new
                                    {
                                        i = 2,


                                        cell = new string[] {
items.Id.ToString(),

items.PODId.ToString(),

items.PODNo,
items.OrderId.ToString(),
items.Name,
items.ContingentType,
items.Location,
items.ControlId,
items.Period,
items.Sector,
items.Week.ToString(),
items.PeriodYear,
items.CreatedBy,
items.CreatedDate!=null? ConvertDateTimeToDate(items.CreatedDate.ToString("dd/MM/yyyy"),"en-GB"):"",
items.ExpectedDeliveryDate!=null? ConvertDateTimeToDate(items.ExpectedDeliveryDate.ToString("dd/MM/yyyy"),"en-GB"):""
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
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }


        //PODMaster Form page
        public ActionResult PODMasterFormPage()
        {
            try
            {
                MastersService mser = new MastersService();

                INSIGHT.Entities.User Userobj = (INSIGHT.Entities.User)Session["objUser"];
                string loggedInUserId = Userobj.UserId;
                ViewBag.CreatedBy = loggedInUserId;
                return View();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }

        }

        [HttpPost]
        public ActionResult PODMasterFormPage(POD pod)
        {
            try
            {
                INSIGHT.Entities.User Userobj = (INSIGHT.Entities.User)Session["objUser"];
                string loggedInUserId = Userobj.UserId;
                pod.CreatedBy = loggedInUserId;
                pod.CreatedDate = DateTime.Now;
                if (!string.IsNullOrEmpty(Request.Form["DeliveryDate"]))
                {
                    pod.DeliveryDate = DateTime.Parse(Request.Form["DeliveryDate"], provider, System.Globalization.DateTimeStyles.NoCurrentDateDefault);

                }
                pod.DeliveryDate = pod.DeliveryDate;
                long podid = orderService.SaveOrUpdatePOD(pod);
                pod.PODNo = "POD-" + podid;
                orderService.SaveOrUpdatePOD(pod);

                return View();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }

        }

        //PODItemsManagement
        public ActionResult PODItemsManagement(long PODId)
        {
            try
            {
                PODOrdersList_vw podorder = new PODOrdersList_vw();
                podorder = orderService.GetPODOrdersListById(PODId);

                return View(podorder);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }

        }
        //PODOrderItemsSerachJQgrid ---loaded from PODOrderItems_vw(search grid)

        public JsonResult PODOrderItemsSerachJQgrid(long OrderId, string UNCode, string Commodity, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                orderService = new OrdersService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();

                //if (!string.IsNullOrWhiteSpace(sord) && sord == "desc")
                //    sord = "Desc";
                //else
                //    sord = "Asc";
                if (!string.IsNullOrEmpty(UNCode)) { criteria.Add("UNCode", Convert.ToInt64(UNCode)); }
                if (!string.IsNullOrEmpty(Commodity)) { criteria.Add("Commodity", Commodity); }
                //if (!string.IsNullOrEmpty(Week)) { criteria.Add("Week", Convert.ToInt64(Week)); }
                //if (!string.IsNullOrEmpty(PeriodYear)) { criteria.Add("PeriodYear", PeriodYear); }
                criteria.Add("OrderId", OrderId);


                sord = sord == "desc" ? "Desc" : "Asc";
                Dictionary<long, IList<PODOrderItems_vw>> podOrditem = null;
                if (!string.IsNullOrWhiteSpace(Commodity))
                {
                    // orditems = orderService.GetOrderItemsListWithLikeSearchPagingAndCriteria(page - 1, 9999, sidx, sord, criteria);
                    podOrditem = orderService.GetPODOrderItemsListWithLikeSearchPagingAndCriteria(0, 9999, sidx, sord, criteria);
                }
                else
                {
                    podOrditem = orderService.GetPODOrderItemsListWithPagingAndCriteria(0, 9999, sidx, sord, criteria);
                }
                if (podOrditem != null && podOrditem.Count > 0)
                {
                    long totalrecords = podOrditem.First().Key;
                    int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                    var jsondat = new
                    {
                        total = totalPages,
                        page = page,
                        records = totalrecords,

                        rows = (from items in podOrditem.First().Value
                                select new
                                {
                                    i = 2,
                                    cell = new string[] {
                                            items.Id.ToString(),
                                            items.OrderId.ToString(),
                                            items.Name,
                                            items.ControlId,
                                            items.LineId.ToString(),
                                            items.SectorPrice.ToString(),
                                            items.Total.ToString(),
                                            items.Period.ToString(),
                                            items.Sector.ToString(),
                                            items.Week.ToString(),
                                            items.PeriodYear,
                                            items.UNCode.ToString(),
                                            items.Commodity,
                                            items.OrderQty.ToString(),
                                            
                                          "<i id=MoveLnk_"+items.Id+" class='icon-white icon-chevron-right move'></i>",
                                          //String.Format(@"<a style='color:#034af3;text-decoration:underline' class='icon-white icon-chevron-right' id=MoveLnk_"+items.Id+"' >{0}</a>"),
                                            items.DeliveredOrdQty.ToString(),
                                            items.DeliveredOrdQty.ToString(),
                                            items.RemainingOrdQty.ToString(),
                                            items.AcceptedOrdQty.ToString(),
                                           
                                            items.Status
                                            
                                            
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
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }

        //Updating the delivered qty  in Order Items and adding record in   PODItems table
        public ActionResult UpdateOrderItems(string SelLindId, string SelDeliveredQty, string SelRemainingQty, long PODId, string SelOrderId, string SelSubItemCode, string SelSubItemName, string SelDelDate)
        {
            try
            {
                INSIGHT.Entities.User Userobj = (INSIGHT.Entities.User)Session["objUser"];
                string loggedInUserId = Userobj.UserId;
                //decimal DelQty = 0;
                //decimal RemainingQty = 0;
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                orderService = new OrdersService();
                string[] LineIdArray = SelLindId.Split(',');
                string[] DelQtyArray = SelDeliveredQty.Split(',');
                string[] RemQtyArray = SelRemainingQty.Split(',');
                string[] OrdidArray = SelOrderId.Split(',');
                string[] subitemcodeArray = SelSubItemCode.Split(',');
                string[] subitemnameArray = SelSubItemName.Split(',');
                //string[] subDelQtyArray = SelSubDelQty.Split(',');
                //string[] subAccQtyArray = SelSubAccQty.Split(',');
                string[] subdelDateArray = SelDelDate.Split(',');


                for (int i = 0; i < LineIdArray.Length; i++)
                {
                    OrderItems orditems = new OrderItems();
                    orditems = orderService.GetOrderItemsById(Convert.ToInt64(LineIdArray[i]));
                    //if (orditems.Status == "Not Delivered")
                    //{
                    //    orditems.Status = "Delivered";

                    //}
                    orditems.LineId = Convert.ToInt64(LineIdArray[i]);
                    // orditems.DeliveredOrdQty = Convert.ToDecimal(DelQtyArray[i]);
                    // orditems.RemainingOrdQty = Convert.ToDecimal(RemQtyArray[i]);
                    orditems.ModifiedBy = loggedInUserId;
                    orditems.ModifiedDate = DateTime.Now;
                    long LineId = orderService.SaveOrUpdateOrderItems(orditems);

                    PODItems poditems = new PODItems();
                    poditems.PODId = PODId;
                    poditems.LineId = LineId;

                    poditems.CreatedDate = DateTime.Now;
                    poditems.CreatedBy = loggedInUserId;
                    //poditems.DeliveredDate = DateTime.Now;
                    poditems.OrderedQty = orditems.OrderQty;
                    poditems.DeliveredQty = Convert.ToDecimal(DelQtyArray[i]);
                    poditems.OrderId = Convert.ToInt64(OrdidArray[i]);
                    poditems.RemainingQty = Convert.ToDecimal(RemQtyArray[i]);
                    if (subitemnameArray[i] != "")
                    {
                        poditems.SubstituteItemCode = (subitemcodeArray[i] != null) ? Convert.ToInt64(subitemcodeArray[i]) : 0;
                        poditems.SubstituteItemName = subitemnameArray[i];
                    }

                    //poditems.SubsDeliveredQty = Convert.ToDecimal(subDelQtyArray[i]);
                    //poditems.SubsAcceptedQty = Convert.ToDecimal(subAccQtyArray[i]);
                    // if (!string.IsNullOrEmpty(subdelDateArray[i]))
                    //  poditems.DeliveredDate = DateTime.Parse(subdelDateArray[i], provider, System.Globalization.DateTimeStyles.NoCurrentDateDefault);

                    poditems.Status = "Yet to Accept";
                    long PODItemsId = orderService.SaveOrUpdatePODItems(poditems);

                    //Newly added ----to resolve while refreshing the screen after updated in orderitems and not added in poditems
                    orditems = orderService.GetOrderItemsById(Convert.ToInt64(LineIdArray[i]));
                    if (orditems.Status == "ADDED IN ORDERITEMS")
                    {
                        orditems.Status = "ADDED IN PODITEMS";

                    }
                    orderService.SaveOrUpdateOrderItems(orditems);

                    //criteria.Add("LineId", Convert.ToInt64(LineIdArray[i]));
                    //Dictionary<long, IList<PODItems>> poditemslist = orderService.GetPODItemsListWithPagingAndCriteria(0, 999, string.Empty, string.Empty, criteria);
                    //criteria.Clear();
                    //if (poditemslist != null && poditemslist.Count > 0)
                    //{
                    //    IList<PODItems> podlist = poditemslist.FirstOrDefault().Value.ToList();
                    //    var DelQtyarray = (from u in podlist select u.DeliveredQty).ToArray();

                    //    for (int j = 0; j < DelQtyarray.Length; j++)
                    //    {
                    //        DelQty = DelQty + DelQtyarray[j];


                    //    }
                    //    orditems = orderService.GetOrderItemsById(Convert.ToInt64(LineIdArray[i]));
                    //    // orditems.RemainingOrdQty = orditems.OrderQty - DelQty;
                    //    orditems.DeliveredOrdQty = DelQty;
                    //    orderService.SaveOrUpdateOrderItems(orditems);

                    //}

                }
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }

        //DeliveredPODItemsJQGrid

        //public JsonResult DeliveredPODItemsJQGrid(long PODId, int rows, string sidx, string sord, int? page = 1)
        //{
        //    try
        //    {
        //        orderService = new OrdersService();
        //        Dictionary<string, object> criteria = new Dictionary<string, object>();

        //        if (!string.IsNullOrWhiteSpace(sord) && sord == "desc")
        //            sord = "Desc";
        //        else
        //            sord = "Asc";
        //        //if (!string.IsNullOrEmpty(OrderNo)) { criteria.Add("OrderId", Convert.ToInt64(OrderNo)); }
        //        //if (!string.IsNullOrEmpty(Contigent)) { criteria.Add("Name", Contigent); }
        //        //if (!string.IsNullOrEmpty(Week)) { criteria.Add("Week", Convert.ToInt64(Week)); }
        //        //if (!string.IsNullOrEmpty(PeriodYear)) { criteria.Add("PeriodYear", PeriodYear); }
        //        criteria.Add("PODId", PODId);
        //        criteria.Add("DeliveryNoteId", (long)0);
        //        Dictionary<long, IList<DeliveredPODItems_vw>> delitems = orderService.GetDeliveredPODItemsListWithPagingAndCriteria(page - 1, 9999, sidx, sord, criteria);


        //        if (delitems != null && delitems.Count > 0)
        //        {
        //            long totalrecords = delitems.First().Key;
        //            int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
        //            var jsondat = new
        //            {
        //                total = totalPages,
        //                page = page,
        //                records = totalrecords,

        //                rows = (from items in delitems.First().Value
        //                        select new
        //                        {
        //                            i = 2,
        //                            cell = new string[] {
        //                                    items.Id.ToString(),
        //                                    items.PODItemsId.ToString(),
        //                                    items.OrderId.ToString(),
        //                                    items.Name,
        //                                    items.LineId.ToString(),
        //                                    items.Period.ToString(),
        //                                    items.Week.ToString(),
        //                                    items.UNCode.ToString(),
        //                                    items.Commodity,
        //                                    items.OrderedQty.ToString(),
        //                                    items.DeliveredQty.ToString(),
        //                                    items.AcceptedQty.ToString(),
        //                                    items.RemainingQty.ToString(),
        //                                     items.SubstituteItemCode==0?"":items.SubstituteItemCode.ToString(),
        //                                    items.SubstituteItemName,
        //                                    items.Status
        //                                    //"<p tyle='color:#034af3;text-decoration:underline' onclick=\"ShowComments('" + items.PODItemsId +"','" + items.Status +"');\">Substitute Item</>",
        //                                  // "< src='' id='ImgHistory' onclick=\"ShowComments('" + items.PODItemsId +"');\" />",

        //                                  // String.Format(@"<a style='color:#034af3;text-decoration:underline' href= '/Orders/SubstituteItemForPODItemsPopup?PODItemsId="+items.PODItemsId+"' >{0}</a>","Substitute Item"),





        //                                    }
        //                        })
        //            };
        //            return Json(jsondat, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            var jsondat = new { rows = (new { cell = new string[] { } }) };
        //            return Json(jsondat, JsonRequestBehavior.AllowGet);

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
        //        throw ex;
        //    }
        //}


        public JsonResult DeliveredPODItemsJQGrid(long PODId, string UNCode, string Commodity, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                orderService = new OrdersService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();

                sord = sord == "desc" ? "Desc" : "Asc";
                //if (!string.IsNullOrWhiteSpace(sord) && sord == "desc")
                //    sord = "Desc";
                //else
                //    sord = "Asc";
                Dictionary<long, IList<DeliveredPODItems_vw>> delitems = null;
                criteria.Add("PODId", PODId);
                criteria.Add("DeliveryNoteId", (long)0);
                if (!string.IsNullOrEmpty(UNCode)) { criteria.Add("UNCode", Convert.ToInt64(UNCode)); }
                if (!string.IsNullOrWhiteSpace(Commodity))
                {
                    if (!string.IsNullOrEmpty(Commodity)) { criteria.Add("Commodity", Commodity); }
                    delitems = orderService.GetDeliveredPODItemsListWithLikePagingAndCriteria(page - 1, 9999, sidx, sord, criteria);
                }
                else
                {
                    delitems = orderService.GetDeliveredPODItemsListWithPagingAndCriteria(page - 1, 9999, sord, sidx, criteria);
                }
                //delitems = orderService.GetDeliveredPODItemsListWithPagingAndCriteria(page - 1, 9999, sidx, sord, criteria);


                if (delitems != null && delitems.Count > 0)
                {
                    long totalrecords = delitems.First().Key;
                    int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                    var jsondat = new
                    {
                        total = totalPages,
                        page = page,
                        records = totalrecords,

                        rows = (from items in delitems.First().Value
                                select new
                                {
                                    i = 2,
                                    cell = new string[] {
                                            items.Id.ToString(),
                                            items.PODItemsId.ToString(),
                                            items.OrderId.ToString(),
                                            items.Name,
                                            items.LineId.ToString(),
                                            items.Period.ToString(),
                                            items.Week.ToString(),
                                            items.UNCode.ToString(),
                                            items.Commodity,
                                            items.OrderedQty.ToString(),
                                            items.DeliveredQty.ToString(),
                                            items.AcceptedQty.ToString(),
                                            items.RemainingQty.ToString(),
                                            items.DiscrepancyCode,
                                            items.SubstituteItemCode==0?"":items.SubstituteItemCode.ToString(),
                                            items.SubstituteItemName,
                                            items.Status
                                            //"<p tyle='color:#034af3;text-decoration:underline' onclick=\"ShowComments('" + items.PODItemsId +"','" + items.Status +"');\">Substitute Item</>",
                                            // "< src='' id='ImgHistory' onclick=\"ShowComments('" + items.PODItemsId +"');\" />",
                                            
                                            // String.Format(@"<a style='color:#034af3;text-decoration:underline' href= '/Orders/SubstituteItemForPODItemsPopup?PODItemsId="+items.PODItemsId+"' >{0}</a>","Substitute Item"),
                                            
                                            
                                            
                                            
                                            
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
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }

        //updateing the accepted quantity

        public ActionResult UpdateAcceptedQty(DeliveredPODItems_vw delpodvw)
        {
            try
            {
                decimal accqty = 0;
                decimal delqty = 0;
                orderService = new OrdersService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                INSIGHT.Entities.User Userobj = (INSIGHT.Entities.User)Session["objUser"];
                string loggedInUserId = Userobj.UserId;
                PODItems poditems = new PODItems();
                OrderItems orditems = new OrderItems();
                poditems = orderService.GetPodItemsValById(delpodvw.PODItemsId);

                if (poditems != null)
                {
                    poditems.ModifiedBy = loggedInUserId;
                    poditems.ModifiedDate = DateTime.Now;
                    poditems.AcceptedQty = delpodvw.AcceptedQty;
                    poditems.Status = "Delivery Accepted";
                    orderService.SaveOrUpdatePODItems(poditems);

                    criteria.Add("LineId", poditems.LineId);
                    Dictionary<long, IList<PODItems>> poditemslist = orderService.GetPODItemsListWithPagingAndCriteria(0, 999, string.Empty, string.Empty, criteria);
                    if (poditemslist != null && poditemslist.Count > 0)
                    {
                        IList<PODItems> podlist = poditemslist.FirstOrDefault().Value.ToList();
                        var AccQtyArray = (from u in podlist select new { u.AcceptedQty, u.DeliveredQty }).ToArray();

                        for (int j = 0; j < AccQtyArray.Length; j++)
                        {
                            accqty = accqty + AccQtyArray[j].AcceptedQty;
                            delqty = delqty + AccQtyArray[j].DeliveredQty;


                            //accqty = accqty + AccQtyArray[j];

                        }
                        orditems = orderService.GetOrderItemsById(poditems.LineId);
                        orditems.AcceptedOrdQty = accqty;
                        orditems.DeliveredOrdQty = delqty;
                        orderService.SaveOrUpdateOrderItems(orditems);
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

        public ActionResult GoToPOD(long OrderId)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {

                    POD po = orderService.GetPODByOrderId(OrderId);
                    if (po != null)
                    {
                        return RedirectToAction("PODItemsManagement", new { po.PODId });
                    }
                    else
                    {
                        POD pod = new POD();
                        pod.CreatedDate = DateTime.Now;
                        pod.DeliveryDate = DateTime.Now;
                        pod.OrderId = OrderId;
                        pod.CreatedBy = userId;
                        //if (!string.IsNullOrEmpty(Request.Form["DeliveryDate"]))
                        //{
                        // pod.DeliveryDate = DateTime.Parse(Request.Form["DeliveryDate"], provider, System.Globalization.DateTimeStyles.NoCurrentDateDefault);

                        //}
                        long podid = orderService.SaveOrUpdatePOD(pod);
                        pod.PODNo = "POD-" + podid;
                        orderService.SaveOrUpdatePOD(pod);
                        Orders ord = new Orders();
                        ord = orderService.GetOrdersById(OrderId);
                        ord.PODId = pod.PODId;
                        orderService.SaveOrUpdateOrder(ord);

                        return RedirectToAction("PODItemsManagement", new { pod.PODId });
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }

        //Updating delivery date in POD
        public ActionResult UpdatePOD(long PODId, string DeliveryDate)
        {
            try
            {
                POD pod = new POD();
                pod = orderService.GetPODById(PODId);
                if (!string.IsNullOrEmpty(DeliveryDate))
                    pod.DeliveryDate = DateTime.Parse(DeliveryDate, provider, System.Globalization.DateTimeStyles.NoCurrentDateDefault);
                orderService.SaveOrUpdatePOD(pod);
                return View();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }

        //Substitute item link
        public ActionResult SubstituteItemForPODItemsPopup(long PODItemsId)
        {
            try
            {
                PODItems poditems = new PODItems();
                poditems = orderService.GetPodItemsValById(PODItemsId);
                return View(poditems);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }

        }

        //Substitute item jqgrid

        public JsonResult SubstituteItemJQgrid(long PODItemsId, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                orderService = new OrdersService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();

                if (!string.IsNullOrWhiteSpace(sord) && sord == "desc")
                    sord = "Desc";
                else
                    sord = "Asc";
                //if (!string.IsNullOrEmpty(OrderNo)) { criteria.Add("OrderId", Convert.ToInt64(OrderNo)); }
                //if (!string.IsNullOrEmpty(Contigent)) { criteria.Add("Name", Contigent); }
                //if (!string.IsNullOrEmpty(Week)) { criteria.Add("Week", Convert.ToInt64(Week)); }
                //if (!string.IsNullOrEmpty(PeriodYear)) { criteria.Add("PeriodYear", PeriodYear); }
                criteria.Add("PODItemsId", PODItemsId);
                Dictionary<long, IList<PODItems>> poditems = orderService.GetPODItemsListWithPagingAndCriteria(0, 999, string.Empty, string.Empty, criteria);


                if (poditems != null && poditems.Count > 0)
                {
                    if (poditems.FirstOrDefault().Value[0].SubstituteItemCode != 0)
                    {
                        long totalrecords = poditems.First().Key;
                        int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                        var jsondat = new
                        {
                            total = totalPages,
                            page = page,
                            records = totalrecords,

                            rows = (from items in poditems.First().Value
                                    select new
                                    {
                                        i = 2,
                                        cell = new string[] {
                                            items.PODItemsId.ToString(),
                                            items.SubstituteItemCode.ToString(),
                                            items.SubstituteItemName,
                                            items.SubsDeliveredQty.ToString(),
                                            items.SubsAcceptedQty.ToString(),
                                           
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

        //save or update Substitute item details
        public ActionResult SaveOrUpdateSubstituteItem(long PODItemsId, long SubstituteItemCode, string SubstituteItemName, decimal SubsDeliveredQty, decimal SubsAcceptedQty)
        {
            try
            {
                PODItems poditems = new PODItems();
                poditems = orderService.GetPodItemsValById(PODItemsId);
                poditems.SubstituteItemCode = SubstituteItemCode;
                poditems.SubstituteItemName = SubstituteItemName;
                poditems.SubsDeliveredQty = SubsDeliveredQty;
                poditems.SubsAcceptedQty = SubsAcceptedQty;
                orderService.SaveOrUpdatePODItems(poditems);
                return View();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }

        //Listing Substituted item name in  autocomplete

        public JsonResult GetSubstituteItemName(string term)
        {
            try
            {
                MastersService mssvc = new MastersService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                criteria.Add("Commodity", term);
                Dictionary<long, IList<ItemMaster>> ItemMasterList = mssvc.GetItemMasterListWithPagingAndCriteriaForAutoComplete(0, 9999, string.Empty, string.Empty, criteria);
                var UserIds = (from u in ItemMasterList.First().Value
                               where u.Commodity != null
                               select u.Commodity).Distinct().ToList();
                return Json(UserIds, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }

        //Listing the substitute code for the corresponding substitute item name
        public JsonResult AutoCompleteSubsItemCode(string subitemname)
        {
            try
            {
                MastersService mssvc = new MastersService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                criteria.Add("Commodity", subitemname);
                Dictionary<long, IList<ItemMaster>> ItemMasterList = mssvc.GetItemMasterListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                var UserIds = (from u in ItemMasterList.First().Value

                               select u.UNCode).Distinct().ToList();
                return Json(UserIds, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }

        //updating delivery date in orderlist
        public ActionResult UpdateOrders(Orders ord)
        {
            try
            {
                orderService = new OrdersService();
                orderService.updateOrders(ord.OrderId, ord.ExpectedDeliveryDate);
                return null;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }


        //Generating Delivery note for Delivered items---Created Delivery Note Id and updating the 
        public ActionResult GenerateDeliveryReport(string DeliveryNoteName, string DeliveryMode, string SelPODItemsId, long OrderId, string DeliverySector)
        {

            try
            {
                orderService = new OrdersService();
                DeliveryNote dn = new DeliveryNote();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                string DuplicateDelNoteName = "";
                INSIGHT.Entities.User Userobj = (INSIGHT.Entities.User)Session["objUser"];
                string loggedInUserId = Userobj.UserId;

                if (SelPODItemsId != "" && DeliveryNoteName != "undifined")
                {
                    string[] PODItemsIdArr = SelPODItemsId.Split(',');
                    dn.DeliveryNoteName = DeliveryNoteName;
                    dn.DeliveryMode = DeliveryMode;
                    dn.CreatedDate = DateTime.Now;
                    dn.CreatedBy = loggedInUserId;
                    dn.OrderId = OrderId;
                    Dictionary<long, IList<DeliveryNote>> delnote = orderService.GetDeliveryNoteListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                    if (delnote != null && delnote.FirstOrDefault().Value.Count > 0 && delnote.FirstOrDefault().Key > 0)
                    {
                        var DelNoteNameArray = (from u in delnote.First().Value
                                                select u.DeliveryNoteName).ToList();
                        for (int i = 0; i < DelNoteNameArray.Count; i++)
                        {
                            if (DeliveryNoteName == DelNoteNameArray[i])
                            {
                                DuplicateDelNoteName = "ALREADYEXIST";
                            }
                        }


                    }
                    if (DuplicateDelNoteName != "ALREADYEXIST")
                    {
                        long DeliveryNoteId = orderService.SaveOrUpdateDeliveryNote(dn);
                        orderService.updateDeliveryNoteIdinPODItemsTbl(DeliveryNoteId, DeliveryNoteName, SelPODItemsId, DeliverySector);
                        return Json(new { success = true, result = "Delivery Note Created  Successfully!!!" }, "text/html", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = true, result = "Delivery Number Already Exist!!!" }, "text/html", JsonRequestBehavior.AllowGet);

                    }


                }
                return null;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightDNPolicy");
                throw ex;
            }


        }

        //Listing the Delivery note name and delivery note id as  a link
        #region updating the delivered qty ===> previous method  ----no need
        //updating DeliveredQty cell in orderitems
        //public ActionResult UpdateDeliveredQtyCell(long LineId, decimal DelQty, decimal RemainingQty)
        //{
        //    try
        //    {

        //        orderService = new OrdersService();
        //        Dictionary<string, object> criteria = new Dictionary<string, object>();
        //        criteria.Add("LineId",LineId);
        //        Dictionary<long, IList<PODItems>> poditems = orderService.GetPODItemsListWithPagingAndCriteria(0, 999, string.Empty, string.Empty, criteria);
        //        var PoditemsDeliveredQty = (from u in poditems.First().Value

        //                       select u.DeliveredQty).ToList();


        //            OrderItems orditems = orderService.GetOrderItemsById(LineId);

        //            if (string.IsNullOrEmpty(orditems.Status) || orditems.Status=="ADDED IN PODITEMS")
        //            {
        //                orditems.Status = "ADDED IN ORDERITEMS";
        //            }
        //            if (PoditemsDeliveredQty.Count == 0)
        //            {

        //                orditems.DeliveredOrdQty = DelQty;
        //                orditems.RemainingOrdQty = orditems.OrderQty - DelQty;

        //            }
        //            else
        //            {
        //                orditems.DeliveredOrdQty = orditems.DeliveredOrdQty + DelQty;
        //                orditems.RemainingOrdQty = orditems.RemainingOrdQty - DelQty;
        //            }
        //            orderService.SaveOrUpdateOrderItems(orditems);
        //            return Json(new{orditems.DeliveredOrdQty,orditems.RemainingOrdQty}, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
        //        throw ex;
        //    }
        //}

        #endregion

        //updating the DeliveredQty in orderitems and add into the poditems 
        public ActionResult UpdateDeliveredQtyCell(long LineId, decimal DelQty, decimal RemainingQty, long PODId, long OrderId)
        {
            try
            {

                orderService = new OrdersService();
                OrderItems orderitems = new OrderItems();
                PODItems poditems = new PODItems();
                Orders ord = new Orders();
                INSIGHT.Entities.User Userobj = (INSIGHT.Entities.User)Session["objUser"];
                string loggedInUserId = Userobj.UserId;
                OrderItems orditems = orderService.GetOrderItemsById(LineId);

                orditems.DeliveredOrdQty = orditems.DeliveredOrdQty + DelQty;
                orditems.RemainingOrdQty = orditems.RemainingOrdQty - DelQty;
                orderService.SaveOrUpdateOrderItems(orditems);

                poditems = orderService.GetPodItemsValById(orditems.LineId);
                poditems = new PODItems();
                poditems.PODId = PODId;
                poditems.LineId = LineId;
                poditems.OrderId = OrderId;

                poditems.CreatedDate = DateTime.Now;
                poditems.CreatedBy = loggedInUserId;
                //poditems.DeliveredDate = DateTime.Now;
                poditems.OrderedQty = orditems.OrderQty;
                poditems.DeliveredQty = DelQty;
                poditems.RemainingQty = RemainingQty;
                orderService.SaveOrUpdatePODItems(poditems);
                ord = orderService.GetOrdersById(OrderId);
                ord.ModifiedBy = loggedInUserId;
                ord.ModifiedDate = DateTime.Now;

                if (string.IsNullOrEmpty(ord.OpeningStatus) || ord.OpeningStatus == "CLOSED")
                {
                    ord.OpeningStatus = "OPENED";


                }
                orderService.SaveOrUpdateOrder(ord);

                return Json(new { orditems.DeliveredOrdQty, orditems.RemainingOrdQty }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }

        }

        //updatin Substituted item name in OrderItems table
        public ActionResult UpdateSubstituteItemNameCell(long LineId, string SubstituteItemName, long subitemcode, string isReplacement)
        {
            try
            {
                string DiscrepancyCode = "";
                orderService = new OrdersService();
                if (isReplacement == "true")
                {
                    DiscrepancyCode = "AR";

                }
                else
                {

                    DiscrepancyCode = "AS";
                }
                orderService.UpdateSubstituteItemNameCell(LineId, SubstituteItemName, subitemcode, DiscrepancyCode);

                Dictionary<string, object> criteria = new Dictionary<string, object>();
                criteria.Add("LineId", LineId);
                Dictionary<long, IList<PODItems>> poditems = orderService.GetPODItemsListWithPagingAndCriteria(0, 999, string.Empty, string.Empty, criteria);
                var poditemsid = (from u in poditems.First().Value
                                  select u.PODItemsId).ToList();
                long GreatestPODItemsId = poditemsid.Max();
                orderService.UpdateSubItemDetailsinPODItemsTbl(GreatestPODItemsId, SubstituteItemName, subitemcode, DiscrepancyCode);
                return null;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }

        }

        //updating the expected delivery date in orders table
        public ActionResult updateExpectedDeliveryDate(long Orderid, string ExpectedDelDate)
        {
            try
            {
                Orders ord = new Orders();
                orderService = new OrdersService();
                ord = orderService.GetOrdersById(Orderid);

                if (!string.IsNullOrEmpty(ExpectedDelDate))
                    ord.ExpectedDeliveryDate = DateTime.Parse(ExpectedDelDate, provider, System.Globalization.DateTimeStyles.NoCurrentDateDefault);
                orderService.SaveOrUpdateOrder(ord);
                return null;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }

        }

        //updating Deliverydate in poditems table
        public ActionResult SaveDeliveredDateinPODItemsTbl(string AllPODItemsId, string AcceptedDate, string AcceptedBy)
        {
            try
            {

                orderService = new OrdersService();
                if (AllPODItemsId != "" || AllPODItemsId != null)
                {
                    string[] AllPODItemsIdArray = AllPODItemsId.Split(',');
                    for (int i = 0; i < AllPODItemsIdArray.Length; i++)
                    {
                        PODItems poditems = new PODItems();
                        poditems = orderService.GetPodItemsValById(Convert.ToInt64(AllPODItemsIdArray[i]));
                        if (!string.IsNullOrEmpty(AcceptedDate))
                            poditems.DeliveredDate = DateTime.Parse(AcceptedDate, provider, System.Globalization.DateTimeStyles.NoCurrentDateDefault);
                        orderService.SaveOrUpdatePODItems(poditems);
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

        //updating the Accepted Quantity in poditems table
        public ActionResult SaveAccQtyinPODItemsTbl(long PODItemsId, decimal AcceptedQty)
        {
            try
            {
                PODItems poditems = new PODItems();
                OrderItems orditems = new OrderItems();
                orderService = new OrdersService();
                poditems = orderService.GetPodItemsValById(PODItemsId);
                poditems.AcceptedQty = poditems.AcceptedQty + AcceptedQty;
                poditems.Status = "DeliveryCompleted";
                orderService.SaveOrUpdatePODItems(poditems);
                orditems = orderService.GetOrderItemsById(poditems.LineId);
                orditems.AcceptedOrdQty = orditems.AcceptedOrdQty + AcceptedQty;
                //orditems.DeliveredOrdQty = delqty;
                orderService.SaveOrUpdateOrderItems(orditems);
                return Json(poditems.AcceptedQty, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }

        //Deleting the PODItems and  update DeliveredQty and Remainging Qty in OrderItems table
        public ActionResult DeletePODItemsAndUpdateOrderItems(long LineId, decimal DeliveredQty, decimal RemainingQty, long PODItemsId)
        {
            try
            {
                orderService = new OrdersService();
                OrderItems orditems = new OrderItems();
                PODItems poditems = new PODItems();
                //updating in order items table
                orditems = orderService.GetOrderItemsById(LineId);
                orditems.DeliveredOrdQty = orditems.DeliveredOrdQty - DeliveredQty;
                orditems.RemainingOrdQty = orditems.OrderQty - orditems.DeliveredOrdQty;
                orderService.SaveOrUpdateOrderItems(orditems);

                //Delete from PODItems
                poditems = orderService.GetPodItemsValById(PODItemsId);
                orderService.DeletePODItemsbyObj(poditems);

                return null;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }

        //updating the opening status in orders table by clicking the back to list button
        public ActionResult UpdateOpeningStatusinOrderTbl(long OrderId)
        {
            try
            {
                Orders ord = new Orders();
                ord = orderService.GetOrdersById(OrderId);
                if (ord.OpeningStatus == "OPENED")
                {
                    ord.OpeningStatus = "CLOSED";
                }
                orderService.SaveOrUpdateOrder(ord);

                return null;
            }

            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }

        //Read only PODItemsManagement page for Different user based on opening status in orders table
        public ActionResult PODItemsManagementReadOnlyPage(long PODId)
        {

            try
            {
                PODOrdersList_vw podorder = new PODOrdersList_vw();
                podorder = orderService.GetPODOrdersListById(PODId);

                return View(podorder);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }

        #endregion

        #region Delivery note excel import related code
        public ActionResult ImportDeliveryNote()
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

        [HttpPost]
        public ActionResult ImportDeliveryNote(HttpPostedFileBase[] uploadedFile)
        {
            //construct the result string
            //first successful uploaded files, then already exists and error
            StringBuilder retValue = new StringBuilder();

            int success = 0;
            OrdersService ordser = new OrdersService();
            HttpPostedFileBase theFile = HttpContext.Request.Files["uploadedFile"];
            // long ImportedDeliveryNoteId = GetCounterValue("ImportedDeliveryNote");
            StringBuilder alreadyExists = new StringBuilder();
            StringBuilder ErrorFilename = new StringBuilder();
            StringBuilder UploadedFilename = new StringBuilder();
            string fileName = string.Empty;

            if (theFile != null && theFile.ContentLength > 0)
            {
                INSIGHT.Entities.User Userobj = (INSIGHT.Entities.User)Session["objUser"];
                if (Userobj == null || (Userobj != null && Userobj.UserId == null))
                { return RedirectToAction("LogOn", "Account"); }
                string loggedInUserId = Userobj.UserId;

                int length = uploadedFile.Length;
                for (int l = 0; l < length; l++)
                {
                    try
                    {
                        long ImportedDeliveryNoteId = GetCounterValue("ImportedDeliveryNote");
                        int AlreadyExistFile = 0;
                        string path = uploadedFile[l].InputStream.ToString();
                        byte[] imageSize = new byte[uploadedFile[l].ContentLength];
                        uploadedFile[l].InputStream.Read(imageSize, 0, (int)uploadedFile[l].ContentLength);
                        string UploadConnStr = "";
                        fileName = uploadedFile[l].FileName;
                        string fileExtn = Path.GetExtension(uploadedFile[l].FileName);
                        string fileLocation = ConfigurationManager.AppSettings["ImportedDeliveryNoteFilePath"].ToString() + DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss").Replace(":", ".") + fileName;
                        uploadedFile[l].SaveAs(fileLocation);
                        if (fileExtn == ".xls")
                        {
                            UploadConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\"";
                        }
                        if (fileExtn == ".xlsx")
                        {
                            UploadConnStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";
                        }

                        //Delivery note as Byte array
                        byte[] UploadedFile = System.IO.File.ReadAllBytes(fileName);


                        OleDbConnection conn = new OleDbConnection();
                        DataTable DtblXcelData = new DataTable();
                        string QeryToGetXcelData = "select * from " + string.Format("{0}${1}", "[Delivery_Note", "A9:L15]");
                        conn.ConnectionString = UploadConnStr;
                        conn.Open();
                        OleDbCommand cmd = new OleDbCommand(QeryToGetXcelData, conn);
                        cmd.CommandType = CommandType.Text;
                        OleDbDataAdapter DtAdptrr = new OleDbDataAdapter();
                        DtAdptrr.SelectCommand = cmd;
                        DtAdptrr.Fill(DtblXcelData);
                        conn.Close();
                        if (DtblXcelData.Rows.Count == 0)
                        {
                            return Json(new { success = false, result = "No Rows available in the file to update!!!" }, "text/html", JsonRequestBehavior.AllowGet);
                        }
                        else if (DtblXcelData.Rows.Count > 0)
                        {
                            ImportedDeliveryNote impdel = new ImportedDeliveryNote();
                            foreach (DataRow delline in DtblXcelData.Rows)
                            {
                                if (delline.ItemArray[0].ToString().Trim() == "Delivery Note Number:")
                                {
                                    string deliverynotename = delline.ItemArray[2].ToString();
                                    impdel.ImpDeliveryNoteName = deliverynotename;
                                    Dictionary<string, object> criteria = new Dictionary<string, object>();
                                    Dictionary<long, IList<DeliveryNote>> delnote = orderService.GetDeliveryNoteListWithPagingAndCriteria(0, 999999, string.Empty, string.Empty, criteria);
                                    if (delnote != null && delnote.Count > 0)
                                    {
                                        IList<DeliveryNote> deliverynotelist = delnote.FirstOrDefault().Value.ToList();
                                        var DeliveryNoteName = (from u in deliverynotelist select u.DeliveryNoteName).Distinct().ToArray();

                                        Dictionary<long, IList<ImportedDeliveryNote>> impdelnote = orderService.GetImportedDeliveryNoteListWithPagingAndCriteria(0, 999999, string.Empty, string.Empty, criteria);
                                        if (delnote != null && delnote.Count > 0)
                                        {

                                            IList<ImportedDeliveryNote> importeddeliverynotelist = impdelnote.FirstOrDefault().Value.ToList();
                                            var impdelnotename = (from u in importeddeliverynotelist select u.ImpDeliveryNoteName).Distinct().ToArray();
                                            string[] AllDeliveryNotelist = DeliveryNoteName.ToArray();
                                            // string[] AllDeliveryNotelist = DeliveryNoteName.Union(impdelnotename).ToArray();
                                            for (int i = 0; i < AllDeliveryNotelist.Count(); i++)
                                            {
                                                if (AllDeliveryNotelist[i] == deliverynotename)
                                                {
                                                    AlreadyExistFile = AlreadyExistFile + 1;
                                                    alreadyExists.Append(fileName);
                                                }
                                            }
                                        }
                                    }

                                }
                                if (delline.ItemArray[3].ToString().Trim() == "Delivery Note Type:")
                                {
                                    impdel.ImpDeliveryNoteType = delline.ItemArray[5].ToString();
                                }
                                if (delline.ItemArray[8].ToString().Trim() == "Shipment Date:")
                                {

                                    string shipmentdate = delline.ItemArray[9].ToString();
                                    if (!string.IsNullOrEmpty(shipmentdate))
                                        impdel.ImpShipmentDate = DateTime.Parse(shipmentdate, provider, System.Globalization.DateTimeStyles.NoCurrentDateDefault);

                                }
                                if (delline.ItemArray[0].ToString().Trim() == "Request#:")
                                {
                                    impdel.ImpRequestNo = delline.ItemArray[2].ToString().Trim() == "" ? 0 : Convert.ToInt64(delline.ItemArray[2].ToString());
                                    // impdel.ImpRequestNo = Convert.ToInt64(delline.ItemArray[2].ToString());

                                }

                                if (delline.ItemArray[3].ToString().Trim() == "Consumption Week:")
                                {
                                    // impdel.ImpConsumptionWeek = Convert.ToDecimal(delline.ItemArray[5].ToString());
                                    impdel.ImpConsumptionWeek = delline.ItemArray[5].ToString().Trim() == "" ? 0 : Convert.ToDecimal(delline.ItemArray[5].ToString());

                                }

                                if (delline.ItemArray[8].ToString().Trim() == "Unit Control No:")
                                {
                                    impdel.ImpControlId = delline.ItemArray[9].ToString();
                                    string[] controlIdarray = delline.ItemArray[9].ToString().Split('-');
                                    impdel.Sector = controlIdarray[1].ToString();
                                    impdel.Name = controlIdarray[3].ToString();
                                    impdel.Location = controlIdarray[1].ToString() + "-" + controlIdarray[2].ToString();
                                    impdel.Period = controlIdarray[4].ToString();
                                    impdel.PeriodYear = controlIdarray[6].ToString() + "-" + controlIdarray[7].ToString();


                                }
                                if (delline.ItemArray[0].ToString().Trim() == "Warehouse:")
                                {
                                    impdel.ImpWarehouse = delline.ItemArray[2].ToString();

                                }
                                if (delline.ItemArray[3].ToString().Trim() == "Delivery Week:")
                                {
                                    impdel.ImpDeliveryWeek = delline.ItemArray[5].ToString().Trim() == "" ? 0 : Convert.ToDecimal(delline.ItemArray[5].ToString());

                                }
                                if (delline.ItemArray[0].ToString().Trim() == "Strength:")
                                {
                                    impdel.ImpStrength = delline.ItemArray[2].ToString().Trim() == "" ? 0 : Convert.ToDecimal(delline.ItemArray[2].ToString());

                                }
                                if (delline.ItemArray[3].ToString().Trim() == "Delivery Mode:")
                                {
                                    impdel.ImpDeliveryMode = delline.ItemArray[5].ToString();
                                    //impdel.ImpDeliveryMode =delline.ItemArray[5].ToString().Trim()==""?0: delline.ItemArray[5].ToString();

                                }
                                if (delline.ItemArray[8].ToString().Trim() == "UN Food Order:")
                                {
                                    impdel.ImpUNFoodOrder = delline.ItemArray[9].ToString();

                                }

                                if (delline.ItemArray[0].ToString().Trim() == "DOS:")
                                {
                                    // impdel.ImpDOS = Convert.ToDecimal(delline.ItemArray[2].ToString());
                                    impdel.ImpDOS = delline.ItemArray[2].ToString().ToString().Trim() == "" ? 0 : Convert.ToDecimal(delline.ItemArray[2].ToString());

                                }
                                if (delline.ItemArray[3].ToString().Trim() == "Seal No:")
                                {
                                    impdel.ImpSealNo = delline.ItemArray[5].ToString();

                                }
                                if (delline.ItemArray[8].ToString().Trim() == "UN Week:")
                                {
                                    //impdel.ImpUNWeek = Convert.ToDecimal(delline.ItemArray[9].ToString());
                                    impdel.ImpUNWeek = delline.ItemArray[9].ToString().Trim() == "" ? 0 : Convert.ToDecimal(delline.ItemArray[9].ToString());

                                }
                                if (delline.ItemArray[0].ToString().Trim() == "Man days:")
                                {
                                    //impdel.ImpManDays = Convert.ToDecimal(delline.ItemArray[2].ToString());
                                    impdel.ImpManDays = delline.ItemArray[2].ToString().Trim() == "" ? 0 : Convert.ToDecimal(delline.ItemArray[2].ToString());

                                }
                                if (delline.ItemArray[8].ToString().Trim() == "Period:")
                                {
                                    impdel.ImpPeriod = delline.ItemArray[9].ToString();

                                }



                            }
                            Orders ord = ordser.GetOrderByControlId(impdel.ImpControlId);
                            impdel.OrderId = ord.OrderId;
                            // impdel.ImpDeliveryNoteId = ImportedDeliveryNoteId;------------------------>new modification
                            impdel.CreatedBy = loggedInUserId;
                            impdel.CreatedDate = DateTime.Now;
                            //impdel.ImpDeliveryNoteId = ImportedDeliveryNoteId;
                            if (AlreadyExistFile == 0)
                            {

                                //  impdel.status = "ADDEDINIMPDELIVERYNOTE";
                                //  long ImportedDeliveryNoteId = ordser.SaveOrUpdateImportedDeliveryNote(impdel);

                                // UploadedFilename.Append(fileName + ",");
                            }
                            OleDbConnection itemconn = new OleDbConnection();
                            DataTable DtblXcelItemData = new DataTable();
                            string QeryToGetXcelItemData = "select [Line#],[Item],[Description],[Ordered Qty],[Delivered Qty],[Number of Packs],[Number of Pieces],[Substitute For],[Substitute Name],[UOM],[Issue Type],[Remarks],[Line Status if Substitution],[Actual Received Qty#],[Recd Date @ Contingent] from " + string.Format("{0}${1}", "[Delivery_Note", "A16:AZ]");
                            // string QeryToGetXcelItemData = "select * from " + string.Format("{0}${1}", "[Delivery_Note", "A16:AZ]");
                            itemconn.ConnectionString = UploadConnStr;
                            itemconn.Open();
                            cmd = new OleDbCommand(QeryToGetXcelItemData, conn);
                            cmd.CommandType = CommandType.Text;
                            DtAdptrr = new OleDbDataAdapter();
                            DtAdptrr.SelectCommand = cmd;
                            DtAdptrr.Fill(DtblXcelItemData);
                            string[] strArray = { "Line#", "Item", "Description", "Ordered Qty", "Delivered Qty", "Number of Packs", "Number of Pieces", "Substitute For", "Substitute Name", "UOM", "Issue Type", "Remarks", "Line Status if Substitution", "Actual Received Qty#", "Recd Date @ Contingent" };
                            char chrFlag = 'Y';
                            if (DtblXcelItemData.Columns.Count == strArray.Length)
                            {
                                int j = 0;
                                string[] strColumnsAray = new string[DtblXcelItemData.Columns.Count];
                                foreach (DataColumn dtColumn in DtblXcelItemData.Columns)
                                {
                                    strColumnsAray[j] = dtColumn.ColumnName;
                                    j++;
                                }
                                for (int i = 0; i < strArray.Length - 1; i++)
                                {
                                    if (strArray[i].Trim() != strColumnsAray[i].Trim())
                                    {
                                        chrFlag = 'N';
                                        break;
                                    }
                                }
                                if (chrFlag == 'Y')
                                {
                                    // ordser = new OrdersService();
                                    IList<ImportedDeliveryNoteItems> importeddeliverynoteitems = new List<ImportedDeliveryNoteItems>();

                                    foreach (DataRow OrdItemline in DtblXcelItemData.Rows)
                                    {

                                        if (OrdItemline["Item"].ToString().Trim() != "")
                                        {
                                            ImportedDeliveryNoteItems impdelitems = new ImportedDeliveryNoteItems();
                                            impdelitems.OrderId = ord.OrderId;
                                            if (OrdItemline["Line Status if Substitution"].ToString().Trim() == "" || OrdItemline["Line Status if Substitution"].ToString().ToUpper() == "NONE")
                                            {

                                                impdelitems.ImpUNCode = Convert.ToInt64(OrdItemline["Item"].ToString().Trim());
                                                impdelitems.ImpCommodity = OrdItemline["Description"].ToString();

                                                impdelitems.ImpSubsItemCode = OrdItemline["Substitute For"].ToString().Trim() == "" ? 0 : Convert.ToInt64(OrdItemline["Substitute For"].ToString().Trim());
                                                impdelitems.ImpSubsItemName = OrdItemline["Substitute Name"].ToString().Trim();
                                                impdelitems.ImpSubsStatus = "NONE";
                                            }
                                            else if (OrdItemline["Line Status if Substitution"].ToString().Trim() != "")
                                            {
                                                impdelitems.ImpUNCode = OrdItemline["Substitute For"].ToString().Trim() == "" ? 0 : Convert.ToInt64(OrdItemline["Substitute For"].ToString().Trim());
                                                impdelitems.ImpCommodity = OrdItemline["Substitute Name"].ToString().Trim();

                                                impdelitems.ImpSubsItemCode = Convert.ToInt64(OrdItemline["Item"].ToString().Trim());
                                                impdelitems.ImpSubsItemName = OrdItemline["Description"].ToString();
                                                impdelitems.ImpSubsStatus = OrdItemline["Line Status if Substitution"].ToString().Trim();

                                            }
                                            // long ImportedDeliveryNoteId = GetCounterValue("ImportedDeliveryNote");
                                            //  impdelitems.ImpDeliveryNoteId = ImportedDeliveryNoteId;
                                            impdelitems.ImpControlId = impdel.ImpControlId;
                                            impdelitems.ImpDeliveryMode = impdel.ImpDeliveryMode;

                                            impdelitems.ImpDeliveryNoteName = impdel.ImpDeliveryNoteName;
                                            impdelitems.OrderId = ord.OrderId;

                                            impdelitems.ImpOrderQty = Convert.ToDecimal(OrdItemline["Ordered Qty"].ToString().Trim());
                                            impdelitems.ImpDeliveryQty = OrdItemline["Actual Received Qty#"].ToString().Trim() == "" ? 0 : Convert.ToDecimal(OrdItemline["Actual Received Qty#"].ToString().Trim());
                                            impdelitems.ImpNoOfPacks = OrdItemline["Number of Packs"].ToString().Trim() == "" ? 0 : Convert.ToDecimal(OrdItemline["Number of Packs"].ToString().Trim());
                                            //impdelitems.ImpNoOfPacks = Convert.ToDecimal(OrdItemline["Number of Packs"].ToString().Trim());
                                            impdelitems.ImpNoOfPieces = OrdItemline["Number of Pieces"].ToString().Trim() == "" ? 0 : Convert.ToDecimal(OrdItemline["Number of Pieces"].ToString().Trim());
                                            //impdelitems.ImpNoOfPieces = Convert.ToDecimal(OrdItemline["Number of Pieces"].ToString().Trim());

                                            impdelitems.ImpUOM = OrdItemline["UOM"].ToString().Trim();
                                            impdelitems.ImpIssueType = OrdItemline["Issue Type"].ToString().Trim();
                                            impdelitems.ImpRemarks = OrdItemline["Remarks"].ToString().Trim();
                                            // impdelitems.ImpSubsStatus = OrdItemline["Line Status if Substitution"].ToString().Trim();
                                            string ImpDeliveryDate = OrdItemline["Recd Date @ Contingent"].ToString().Trim();
                                            impdelitems.ImpExpDeliveryQty = OrdItemline["Delivered Qty"].ToString().Trim() == "" ? 0 : Convert.ToDecimal(OrdItemline["Delivered Qty"].ToString().Trim());
                                            if (!string.IsNullOrEmpty(ImpDeliveryDate))
                                                // impdelitems.ImpDeliveryDate = DateTime.Parse(ImpDeliveryDate, provider, System.Globalization.DateTimeStyles.NoCurrentDateDefault);
                                                impdelitems.ImpDeliveryDate = Convert.ToDateTime(OrdItemline["Recd Date @ Contingent"].ToString().Trim());
                                            impdelitems.CreatedBy = loggedInUserId;
                                            impdelitems.CreatedDate = DateTime.Now;
                                            if (AlreadyExistFile == 0)
                                            {
                                                importeddeliverynoteitems.Add(impdelitems);
                                                //ordser.SaveOrUpdateImportedDeliveryNoteItems(impdelitems);
                                                // UpdateImportedDeliveryNoteItemDetailsinOrdItemsTbl(impdelitems);

                                            }
                                        }

                                    }
                                    if (importeddeliverynoteitems.Count != 0)
                                    {
                                        orderService.SaveorUpdateDeliveryNoteInSingleSession(impdel, importeddeliverynoteitems);
                                        //orderService.SaveOrUpdateImportedDeliveryNoteItemsList(importeddeliverynoteitems);

                                        Dictionary<string, object> criteria = new Dictionary<string, object>();
                                        criteria.Clear();
                                        criteria.Add("ImpDeliveryNoteId", importeddeliverynoteitems[0].ImpDeliveryNoteId);


                                        Dictionary<long, IList<ImportedDeliveryNoteItems>> impdelnoteitemslist = orderService.GetImportedDeliveryNoteItemsListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                                        IList<ImportedDeliveryNoteItems> delnoteitems = impdelnoteitemslist.FirstOrDefault().Value.ToList();
                                        UpdateImportedDeliveryNoteItemDetailsinOrdItemsTbl1(delnoteitems, UploadedFile);
                                        UploadedFilename.Append(fileName + ",");
                                    }

                                }
                                else
                                {
                                    ErrorFilename.Append(fileName + ",");
                                }
                            }
                            else
                            {
                                ErrorFilename.Append(fileName + ",");
                            }
                        }
                        success = success + 1;

                    }
                    catch (Exception ex)
                    {
                        ErrorFilename.Append(fileName + ",");

                        ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                    }
                }
            }
            else
            {
                ErrorFilename.Append(fileName + ",");
                //  return Json(new { success = false, result = "You have uploded the empty file. Please upload the correct file." }, "text/html", JsonRequestBehavior.AllowGet);
            }

            INSIGHT.Entities.User Userobj1 = (INSIGHT.Entities.User)Session["objUser"];

            string loggedInUserId1 = Userobj1.UserId;

            if (UploadedFilename != null && !string.IsNullOrEmpty(UploadedFilename.ToString()))
            {
                retValue.Append("-------files uploaded successfully-----------");
                retValue.Append("<br />");
                string[] upfiles = UploadedFilename.ToString().Split(',');
                if (upfiles != null && upfiles.Count() > 0)
                {
                    foreach (string s in upfiles)
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                            retValue.Append(s + ";");
                            retValue.Append("<br />");
                        }

                    }

                    retValue.Append("<br />");
                    retValue.Append("Successfully uploaded files" + Convert.ToInt32(UploadedFilename.ToString().Split(',').Count() - 1));
                    retValue.Append("<br />");
                    //retValue.Append("-----------------------------------------------------");
                    InsightReport insreport = new InsightReport();
                    insreport.CreatedBy = loggedInUserId1;
                    insreport.CreatedDate = DateTime.Now;
                    insreport.ReportCode = "SuccessfulDeliveryNoteUpload";
                    insreport.Description = "SuccessfulDeliveryNoteUpload";
                    insreport.FileNames = UploadedFilename.ToString();
                    orderService.SaveOrUpdateInsightReport(insreport);
                }
            }
            if (alreadyExists != null && !string.IsNullOrEmpty(alreadyExists.ToString()))
            {
                retValue.Append("-----------files already exists--------------");
                retValue.Append("<br />");
                string[] existsfiles = alreadyExists.ToString().Split(',');
                if (existsfiles != null && existsfiles.Count() > 0)
                {
                    foreach (string s in existsfiles)
                    { if (!string.IsNullOrEmpty(s)) retValue.Append(s + ";"); retValue.Append("<br />"); }
                    //retValue.Append("-------------------------------------------------");
                }
                InsightReport insreport = new InsightReport();
                insreport.CreatedBy = loggedInUserId1;
                insreport.CreatedDate = DateTime.Now;
                insreport.ReportCode = "AlreadyExistDeliveryNoteUpload";
                insreport.Description = "Already exist file";
                insreport.FileNames = alreadyExists.ToString();
                orderService.SaveOrUpdateInsightReport(insreport);
            }
            if (ErrorFilename != null && !string.IsNullOrEmpty(ErrorFilename.ToString()))
            {
                retValue.Append("-----------error occured Files--------------");
                string[] errfiles = ErrorFilename.ToString().Split(',');
                if (errfiles != null && errfiles.Count() > 0)
                {
                    foreach (string s in errfiles)
                    { if (!string.IsNullOrEmpty(s))retValue.Append(s + ";"); retValue.Append("<br />"); }
                    //retValue.Append("-------------------------------------------------");
                    InsightReport insreport = new InsightReport();
                    insreport.CreatedBy = loggedInUserId1;
                    insreport.CreatedDate = DateTime.Now;
                    insreport.ReportCode = "ErrorDeliveryNoteUpload";
                    insreport.Description = "Error file in Delivery Note";
                    insreport.FileNames = ErrorFilename.ToString();
                    orderService.SaveOrUpdateInsightReport(insreport);
                }
            }
            // return Json(new { success = true, result = retValue.ToString().Replace(Environment.NewLine, "<br />") }, "text/html", JsonRequestBehavior.AllowGet);
            //}
            //catch (Exception ex)
            //{
            //    ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
            //}
            //MyLabel.Text = sb.ToString().Replace(Environment.NewLine, "<br />");
            return Json(new { success = true, result = retValue.ToString().Replace(Environment.NewLine, "<br />") }, "text/html", JsonRequestBehavior.AllowGet);

        }

        //Imported delivery note list in jqgrid
        public JsonResult ImportedDeliveryNoteListJQGrid(long? OrdersId, string searchItems, string ControlId, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                OrdersService us = new OrdersService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();

                if (!string.IsNullOrWhiteSpace(sord) && sord == "desc")
                    sord = "Desc";
                else
                    sord = "Asc";



                if (string.IsNullOrWhiteSpace(searchItems) && string.IsNullOrWhiteSpace(ControlId))
                {

                    return null;
                }
                else
                {

                    #region search criteria


                    if (searchItems != null && searchItems != "")
                    {
                        var Items = searchItems.ToString().Split(',');
                        if (!string.IsNullOrWhiteSpace(Items[0])) { criteria.Add("Sector", Items[0]); }
                        if (!string.IsNullOrWhiteSpace(Items[1])) { criteria.Add("Name", Items[1]); }
                        if (!string.IsNullOrWhiteSpace(Items[2])) { criteria.Add("Location", Items[2]); }
                        if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Period", Items[3]); }
                        //if (!string.IsNullOrWhiteSpace(Items[4])) { criteria.Add("PeriodYear", Items[4]); }
                        //criteria.Add("Sector", Items[0]);
                        //criteria.Add("Name", Items[1]);
                        //criteria.Add("Location", Items[2]);
                        //criteria.Add("Period", Items[3]);
                        var TrimItems = searchItems.Replace(",", "");
                        if (string.IsNullOrWhiteSpace(TrimItems))
                            return null;
                    }
                    else
                    {
                        string userid = (Session["UserId"] == null) ? "" : Session["UserId"].ToString();
                        UserService usrSrv = new UserService();
                        Dictionary<string, object> criteriaUserAppRole = new Dictionary<string, object>();
                        criteriaUserAppRole.Add("UserId", userid);
                        Dictionary<long, IList<UserAppRole>> userAppRole = usrSrv.GetAppRoleForAnUserListWithPagingAndCriteria(0, 1000, string.Empty, string.Empty, criteriaUserAppRole);
                        if (userAppRole != null && userAppRole.Count > 0 && userAppRole.First().Key > 0)
                        {
                            int count = userAppRole.First().Value.Count;
                            //if it has values then for each concatenate APP+ROLE
                            string[] sectorCodeArr = new string[count];
                            string[] contingentCodeArr = new string[count];
                            string[] locationArr = new string[count];
                            string[] periodArr = new string[count];
                            int i = 0;
                            foreach (UserAppRole uar in userAppRole.First().Value)
                            {

                                string SectorCode = uar.SectorCode;
                                string ContingentCode = uar.ContingentCode;
                                string location = uar.Location;


                                if (!string.IsNullOrEmpty(SectorCode))
                                {
                                    sectorCodeArr[i] = SectorCode;
                                }
                                if (!string.IsNullOrEmpty(ContingentCode))
                                {
                                    contingentCodeArr[i] = ContingentCode;
                                }
                                if (!string.IsNullOrEmpty(location))
                                {
                                    locationArr[i] = location;
                                }

                                i++;
                            }
                            sectorCodeArr = sectorCodeArr.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                            contingentCodeArr = contingentCodeArr.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                            locationArr = locationArr.Where(x => !string.IsNullOrEmpty(x)).ToArray();

                            criteria.Add("Sector", sectorCodeArr);
                            criteria.Add("Name", contingentCodeArr);
                            criteria.Add("Location", locationArr);
                        }
                    }

                    if (OrdersId != null && OrdersId != 0)
                    {
                        //long[] OrderIds = OrdersId.Split(',').Select(n => Convert.ToInt64(n)).ToArray();
                        criteria.Add("OrderId", OrdersId);

                    }
                    if (!string.IsNullOrWhiteSpace(ControlId)) { criteria.Add("ControlId", ControlId); }
                    #endregion

                    Dictionary<long, IList<ImportedDeliveryNote>> impdelnote = us.GetImportedDeliveryNoteListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);

                    if (impdelnote != null && impdelnote.Count > 0 && impdelnote.Values.Count != 0)
                    {
                        long totalrecords = impdelnote.First().Key;
                        int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                        var jsondat = new
                        {
                            total = totalPages,
                            page = page,
                            records = totalrecords,

                            rows = (from items in impdelnote.First().Value
                                    select new
                                    {
                                        i = 2,
                                        cell = new string[] {
                                              items. ImpDeliveryNoteId.ToString(),
                                              items.ImpDeliveryNoteName.ToString(),
                                              items.ImpControlId.ToString(),
                                              items. ImpRequestNo.ToString(),
                                              items.ImpWarehouse.ToString(),
                                              items.ImpStrength.ToString(),
                                              items.ImpDOS.ToString(),
                                              items.ImpManDays.ToString(),
                                              items.ImpConsumptionWeek.ToString(),
                                              items.ImpDeliveryWeek.ToString(),
                                              items.ImpDeliveryMode,
                                              items.ImpSealNo,
                                              items.ImpShipmentDate.ToString(),
                                              items.ImpUNFoodOrder,
                                              items.ImpUNWeek.ToString(),
                                              items.ImpPeriod,
                                              items.OrderId.ToString(),
                                              items.Sector,
                                              items.Name,
                                              items.Location,
                                              items.Period,
                                              items.PeriodYear
                                              
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
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }


        //Imported delivery note items 
        public ActionResult ImportedDeliveryNoteItems(long ImpDeliveryNoteId)
        {
            try
            {
                INSIGHT.Entities.User Userobj = (INSIGHT.Entities.User)Session["objUser"];
                if (Userobj == null || (Userobj != null && Userobj.UserId == null))
                { return RedirectToAction("LogOn", "Account"); }
                string loggedInUserId = Userobj.UserId;
                ViewBag.CurrentUser = loggedInUserId;
                ImportedDeliveryNote impdel = orderService.GetImportedDeliveryNoteDetailsbyImpDeliveryNoteId(ImpDeliveryNoteId);
                return View(impdel);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }

        }

        //Imported delivery note itesm jqgrid
        public JsonResult ImportedDeliveryNoteItemsJQGrid(long ImpDeliveryNoteId, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                Dictionary<string, object> criteria = new Dictionary<string, object>();

                //sord = sord == "desc" ? "Desc" : "Asc";
                criteria.Add("ImpDeliveryNoteId", ImpDeliveryNoteId);


                sord = sord == "desc" ? "Desc" : "Asc";
                Dictionary<long, IList<ImportedDeliveryNoteItems>> impdelnoteitems = null;
                //if (!string.IsNullOrWhiteSpace(Commodity))
                //{
                //    orditems = orderService.GetOrderItemsListWithLikeSearchPagingAndCriteria(page - 1, 9999, sidx, sord, criteria);
                //}
                //else
                {
                    impdelnoteitems = orderService.GetImportedDeliveryNoteItemsListWithPagingAndCriteria(page - 1, 9999, sidx, sord, criteria);
                }



                if (impdelnoteitems != null && impdelnoteitems.Count > 0)
                {
                    long totalrecords = impdelnoteitems.First().Key;
                    int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                    var jsondat = new
                    {
                        total = totalPages,
                        page = page,
                        records = totalrecords,

                        rows = (from items in impdelnoteitems.First().Value
                                select new
                                {
                                    i = 2,
                                    cell = new string[] {
                                        items. ImpDeliveryNoteItemsId.ToString(),
                                        items.ImpDeliveryNoteId.ToString(),
                                        items.ImpDeliveryNoteName.ToString(),
                                        items.ImpControlId.ToString(),
                                        items. ImpUNCode.ToString(),
                                        items.ImpCommodity.ToString(),
                                        items.ImpOrderQty==0?"":Math.Round(items.ImpOrderQty,3).ToString(),
                                        items.ImpDeliveryQty==0?"":Math.Round(items.ImpDeliveryQty,3).ToString(),
                                        items.ImpNoOfPacks==0?"":Math.Round(items.ImpNoOfPacks,3).ToString(),
                                        items.ImpNoOfPieces==0?"":Math.Round(items.ImpNoOfPieces,3).ToString(),
                                        items.ImpSubsItemCode==0?"":items.ImpSubsItemCode.ToString(),
                                        items.ImpSubsItemName.ToString(),
                                        items.ImpUOM.ToString(),
                                        items.ImpIssueType.ToString(),
                                        items.ImpRemarks.ToString(),
                                        items.OrderId.ToString()
                                       
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
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }

        public void UpdateImportedDeliveryNoteItemDetailsinOrdItemsTbl(IList<ImportedDeliveryNoteItems> delnoteitems, byte[] UploadedFile)
        {
            try
            {
                foreach (var impdelitems in delnoteitems)
                {
                    string IndividualSubstitution = string.Empty;
                    string GeneralSubstitution = string.Empty;
                    string IndividualReplacement = string.Empty;
                    string GeneralReplacement = string.Empty;



                    decimal DeliveredQty = 0;
                    //INSIGHT.Entities.User Userobj = (INSIGHT.Entities.User)Session["objUser"];
                    //if (Userobj == null || (Userobj != null && Userobj.UserId == null))
                    //{ methodtest(); }
                    string userid = delnoteitems.FirstOrDefault().CreatedBy;


                    //string userid = Session["UserId"].ToString();
                    //string loggedInUserId = Userobj.UserId;------> need to be clarified
                    //Getting the ImportedDeliveryNoteDetails by id
                    ImportedDeliveryNote impdel = orderService.GetImportedDeliveryNoteDetailsbyImpDeliveryNoteId(impdelitems.ImpDeliveryNoteId);

                    //Getting the orderitems details by orderid
                    Dictionary<string, object> criteria = new Dictionary<string, object>();
                    Dictionary<string, object> likeSearchCriteria = new Dictionary<string, object>();

                    criteria.Clear();
                    criteria.Add("OrderId", impdelitems.OrderId);
                    criteria.Add("UNCode", impdelitems.ImpUNCode);
                    // Dictionary<long, IList<OrderItems>> orditems = orderService.GetOrderItemsListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                    // IList<OrderItems> orditemslist = orditems.FirstOrDefault().Value;
                    Dictionary<long, IList<OrderItems>> orditemslist = orderService.GetOrderItemsListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);


                    //Get deliverynote details by deliverynotename

                    DeliveryNote delnote = orderService.GetDeliverynoteDetailsByDeliveryNoteName(impdelitems.ImpDeliveryNoteName);
                    if (delnote == null)
                    {
                        delnote = new DeliveryNote();

                        delnote.DeliveryNoteName = impdelitems.ImpDeliveryNoteName;
                        delnote.OrderId = impdelitems.OrderId;
                        delnote.DeliveryMode = impdelitems.ImpDeliveryMode;
                        delnote.DeliveryStatus = "Completed";
                        delnote.CreatedBy = impdelitems.CreatedBy;
                        delnote.CreatedDate = DateTime.Now;
                        delnote.DocumentData = UploadedFile;
                        delnote.DeliveryNoteType = impdel.ImpDeliveryNoteType;
                        delnote.ActualDeliveryDate = impdel.ImpShipmentDate;
                        orderService.SaveOrUpdateDeliveryNote(delnote);
                    }

                    //Add values in PODItems


                    POD pod = orderService.GetPODByOrderId(impdel.OrderId);
                    if (pod == null)
                    {
                        pod = new POD();
                        pod.OrderId = impdelitems.OrderId;
                        pod.DeliveryDate = impdel.ImpShipmentDate;
                        pod.CreatedBy = impdelitems.CreatedBy;
                        //pod.CreatedBy = loggedInUserId;----->need to be clarified
                        pod.CreatedDate = DateTime.Now;
                        pod.Status = "PODCompleted";
                        orderService.SaveOrUpdatePOD(pod);
                        orderService.GetPODById(pod.PODId);
                        pod.PODNo = "POD-" + pod.PODId;
                        orderService.SaveOrUpdatePOD(pod);
                    }

                    //Add the values in poditems
                    PODItems poditems = new PODItems();
                    poditems.PODId = pod.PODId;
                    poditems.OrderId = impdelitems.OrderId;
                    //Getting lineid from the orderitems

                    if (orditemslist.FirstOrDefault().Value.Count == 1)
                    {

                        if (orditemslist.FirstOrDefault().Value[0].UNCode == impdelitems.ImpUNCode)
                        {
                            poditems.LineId = orditemslist.FirstOrDefault().Value[0].LineId;
                            poditems.OrderedQty = orditemslist.FirstOrDefault().Value[0].OrderQty;
                        }
                    }
                    else if (orditemslist.FirstOrDefault().Value.Count == 0)
                    {
                        InsightReport insreport = new InsightReport();
                        insreport.OrderId = impdelitems.OrderId;
                        insreport.ControlId = impdelitems.ImpControlId;
                        insreport.DeliveryNoteName = impdelitems.ImpDeliveryNoteName;
                        insreport.UNCode = impdelitems.ImpUNCode;
                        insreport.Commodity = impdelitems.ImpCommodity;
                        insreport.SubsCode = impdelitems.ImpSubsItemCode;
                        insreport.SubsName = impdelitems.ImpSubsItemName;
                        insreport.ReportCode = "UnOrderedDelivery";
                        insreport.Description = "The delivered has been done for the un ordered items";
                        insreport.DeliveredQty = impdelitems.ImpDeliveryQty;
                        insreport.CreatedBy = impdelitems.CreatedBy;
                        insreport.CreatedDate = DateTime.Now;
                        orderService.SaveOrUpdateInsightReport(insreport);
                    }


                    //Getting total deliveredqty for the item based on lineid
                    criteria.Clear();
                    criteria.Add("OrderId", impdelitems.OrderId);
                    criteria.Add("LineId", poditems.LineId);

                    Dictionary<long, IList<DeliveredPODItems_vw>> poditmlist = orderService.GetDeliveredPODItemsListWithPagingAndCriteria(0, 99999, string.Empty, string.Empty, criteria);
                    IList<DeliveredPODItems_vw> poilist = poditmlist.FirstOrDefault().Value;
                    DeliveredQty = (from u in poilist select u.DeliveredQty).Sum();

                    //checking for individual and general  substutions/Replacement  in substitution master
                    if (impdelitems.ImpSubsStatus.ToUpper() != "NONE")
                    {
                        criteria.Clear();
                        Orders ord = new Orders();
                        string[] cri = { "Individual", "General" };
                        ord = orderService.GetOrdersById(impdelitems.OrderId);
                        criteria.Add("Period", ord.Period);
                        criteria.Add("PeriodYear", ord.PeriodYear);
                        criteria.Add("Category", cri);
                        //criteria.Add("Category", "General");
                        criteria.Add("UNCode", impdelitems.ImpUNCode);
                        // likeSearchCriteria.Add("Sector", impdel.Sector);
                        //criteria.Add("Sector", impdel.Sector);
                        //criteria.Add("ControlId", impdelitems.ImpControlId);
                        Dictionary<long, IList<SubstitutionMaster>> subsmstlist = orderService.GetSubstitutionMasterListWithPagingAndCriteria(0, 99999, string.Empty, string.Empty, criteria, likeSearchCriteria);
                        criteria.Clear();
                        IList<SubstitutionMaster> subsmstdetlist = subsmstlist.FirstOrDefault().Value;

                        //Checking the substitution
                        //Individual substitution
                        var IndiSubsList = (from u in subsmstdetlist where u.SubsOrReplace.ToUpper() == "SUBSTITUTION" && u.Category.ToUpper() == "INDIVIDUAL" && u.ControlId == impdelitems.ImpControlId && impdelitems.ImpSubsStatus.ToUpper() == "SUBSTITUTION" select new { u.UNCode, u.ItemName, u.OrderedQty, u.SubstituteItemCode, u.SubstituteItemName, u.AcceptedQty, u.Sector }).ToArray();
                        for (int i = 0; i < IndiSubsList.Length; i++)
                        {
                            if (IndiSubsList[i].UNCode == impdelitems.ImpUNCode && IndiSubsList[i].SubstituteItemCode == impdelitems.ImpSubsItemCode)
                            {
                                IndividualSubstitution = "true";
                                break;
                            }
                        }
                        //General Substitution
                        var GenSubsList = (from u in subsmstdetlist where u.SubsOrReplace.ToUpper() == "SUBSTITUTION" && u.Category.ToUpper() == "GENERAL" && impdelitems.ImpSubsStatus.ToUpper() == "SUBSTITUTION" select new { u.UNCode, u.ItemName, u.OrderedQty, u.SubstituteItemCode, u.SubstituteItemName, u.AcceptedQty, u.Sector }).ToArray();
                        for (int i = 0; i < GenSubsList.Length; i++)
                        {
                            if (GenSubsList[i].UNCode == impdelitems.ImpUNCode && GenSubsList[i].SubstituteItemCode == impdelitems.ImpSubsItemCode && GenSubsList[i].Sector.Contains(impdel.Sector))
                            {
                                GeneralSubstitution = "true";
                                break;
                            }
                        }
                        //Checking the replacement
                        //Individual Replacement
                        var IndiRepList = (from u in subsmstdetlist where u.SubsOrReplace.ToUpper() == "REPLACEMENT" && u.Category.ToUpper() == "INDIVIDUAL" && u.ControlId == impdelitems.ImpControlId && impdelitems.ImpSubsStatus.ToUpper() == "REPLACEMENT" select new { u.UNCode, u.ItemName, u.OrderedQty, u.SubstituteItemCode, u.SubstituteItemName, u.AcceptedQty, u.Sector }).ToArray();
                        for (int i = 0; i < IndiRepList.Length; i++)
                        {
                            if (IndiRepList[i].UNCode == impdelitems.ImpUNCode && IndiRepList[i].SubstituteItemCode == impdelitems.ImpSubsItemCode)
                            {
                                IndividualReplacement = "true";
                                break;
                            }
                        }

                        //General Substitution
                        var GenRepList = (from u in subsmstdetlist where u.SubsOrReplace.ToUpper() == "REPLACEMENT" && u.Category.ToUpper() == "GENERAL" && impdelitems.ImpSubsStatus.ToUpper() == "REPLACEMENT" select new { u.UNCode, u.ItemName, u.OrderedQty, u.SubstituteItemCode, u.SubstituteItemName, u.AcceptedQty, u.Sector }).ToArray();
                        for (int i = 0; i < GenRepList.Length; i++)
                        {
                            if (GenRepList[i].UNCode == impdelitems.ImpUNCode && GenRepList[i].SubstituteItemCode == impdelitems.ImpSubsItemCode && GenRepList[i].Sector.Contains(impdel.Sector))
                            {
                                GeneralReplacement = "true";
                                break;
                            }
                        }


                        // var submst = (from u in subsmstdetlist where u.SubsOrReplace.ToUpper()== "SUBSTITUTION" select new { u.UNCode, u.ItemName, u.OrderedQty, u.SubstituteItemCode, u.SubstituteItemName, u.AcceptedQty,u.Sector}).ToArray();
                        // for (int i = 0; i < submst.Length; i++)
                        // {
                        //     //  if (submst[i].UNCode == impdelitems.ImpUNCode && submst[i].ItemName == impdelitems.ImpCommodity && submst[i].SubstituteItemCode == impdelitems.ImpSubsItemCode && submst[i].SubstituteItemName == impdelitems.ImpSubsItemName)
                        //     if (submst[i].UNCode == impdelitems.ImpUNCode && submst[i].SubstituteItemCode == impdelitems.ImpSubsItemCode && submst[i].Sector.Contains(impdel.Sector))
                        //     {

                        //         SubstitutionFlag = "true";
                        //         break;
                        //     }

                        // }

                        //var repmst = (from u in subsmstdetlist where u.SubsOrReplace.ToUpper() == "REPLACEMENT" select new { u.UNCode, u.ItemName, u.OrderedQty, u.SubstituteItemCode, u.SubstituteItemName, u.AcceptedQty, u.Sector }).ToArray();
                        //for (int i = 0; i < submst.Length; i++)
                        //{
                        //    //  if (submst[i].UNCode == impdelitems.ImpUNCode && submst[i].ItemName == impdelitems.ImpCommodity && submst[i].SubstituteItemCode == impdelitems.ImpSubsItemCode && submst[i].SubstituteItemName == impdelitems.ImpSubsItemName)
                        //    if (submst[i].UNCode == impdelitems.ImpUNCode && submst[i].SubstituteItemCode == impdelitems.ImpSubsItemCode && submst[i].Sector.Contains(impdel.Sector))
                        //    {
                        //        ReplacementFlag = "true";
                        //        break;
                        //    }

                        //}
                    }
                    // poditems.OrderedQty = impdelitems.ImpOrderQty;----------------------->orderqty is taken from the orderitems
                    poditems.DeliveredQty = impdelitems.ImpDeliveryQty;
                    poditems.AcceptedQty = impdelitems.ImpDeliveryQty;
                    //poditems.DeliveredDate = impdel.ImpShipmentDate;
                    if (impdelitems.ImpDeliveryDate != null)
                    {
                        poditems.DeliveredDate = impdelitems.ImpDeliveryDate;
                    }
                    poditems.CreatedBy = impdelitems.CreatedBy;
                    poditems.CreatedDate = DateTime.Now;
                    poditems.SubstituteItemCode = impdelitems.ImpSubsItemCode;
                    poditems.SubstituteItemName = impdelitems.ImpSubsItemName;
                    poditems.RemainingQty = impdelitems.ImpOrderQty - (DeliveredQty + impdelitems.ImpDeliveryQty);
                    poditems.Status = "DeliveryCompleted";
                    poditems.DeliveryNoteId = delnote.DeliveryNoteId;
                    poditems.DeliveryNoteName = delnote.DeliveryNoteName;
                    if (impdelitems.ImpSubsItemCode != 0 && impdelitems.ImpSubsItemName != null && impdelitems.ImpSubsStatus.ToString().ToUpper() == "SUBSTITUTION")
                    {
                        poditems.DiscrepancyCode = "AS";
                    }
                    if (impdelitems.ImpSubsItemCode != 0 && impdelitems.ImpSubsItemName != null && impdelitems.ImpSubsStatus.ToString().ToUpper() == "REPLACEMENT")
                    {
                        poditems.DiscrepancyCode = "AR";
                    }

                    if (poditems.LineId != 0)
                    {
                        if (impdelitems.ImpSubsStatus.ToUpper() == "NONE")
                        {
                            orderService.SaveOrUpdatePODItems(poditems);

                            //Adding invoiceqty and deliverysector in poditems table
                            criteria.Clear();
                            criteria.Add("PODItemsId", poditems.PODItemsId);
                            criteria.Add("OrderId", impdelitems.OrderId);
                            Dictionary<long, IList<InvoiceQtyDelSector_PODItems>> poditems_invqty = orderService.GetPODItems_InvQtyandDeliverySectorListWithPagingAndCriteria(0, 99999, string.Empty, string.Empty, criteria);
                            IList<InvoiceQtyDelSector_PODItems> poditems_invqtylist = poditems_invqty.FirstOrDefault().Value;
                            var InvQty = (from u in poditems_invqtylist select new { u.InvoiceQty, u.DeliverySector, u.PODItemsId }).ToArray();
                            criteria.Clear();
                            poditems = orderService.GetPodItemsValById(poditems.PODItemsId);
                            poditems.InvoiceQty = InvQty.FirstOrDefault().InvoiceQty;
                            poditems.DeliverySector = InvQty.FirstOrDefault().DeliverySector;
                            orderService.SaveOrUpdatePODItems(poditems);

                        }
                        else if (IndividualSubstitution == "true" || GeneralSubstitution == "true" || IndividualReplacement == "true" || GeneralReplacement == "true")
                        {
                            orderService.SaveOrUpdatePODItems(poditems);

                            //Adding invoiceqty and deliverysector in poditems table
                            criteria.Clear();
                            criteria.Add("PODItemsId", poditems.PODItemsId);
                            criteria.Add("OrderId", impdelitems.OrderId);
                            Dictionary<long, IList<InvoiceQtyDelSector_PODItems>> poditems_invqty = orderService.GetPODItems_InvQtyandDeliverySectorListWithPagingAndCriteria(0, 99999, string.Empty, string.Empty, criteria);
                            IList<InvoiceQtyDelSector_PODItems> poditems_invqtylist = poditems_invqty.FirstOrDefault().Value;
                            var InvQty = (from u in poditems_invqtylist select new { u.InvoiceQty, u.DeliverySector, u.PODItemsId }).ToArray();
                            criteria.Clear();
                            poditems = orderService.GetPodItemsValById(poditems.PODItemsId);
                            poditems.InvoiceQty = InvQty.FirstOrDefault().InvoiceQty;
                            poditems.DeliverySector = InvQty.FirstOrDefault().DeliverySector;
                            orderService.SaveOrUpdatePODItems(poditems);
                        }
                        else
                        {
                            InsightReport insreport = new InsightReport();
                            insreport.OrderId = impdelitems.OrderId;
                            insreport.ControlId = impdelitems.ImpControlId;
                            insreport.DeliveryNoteName = impdelitems.ImpDeliveryNoteName;
                            insreport.UNCode = impdelitems.ImpUNCode;
                            insreport.Commodity = impdelitems.ImpCommodity;
                            insreport.SubsCode = impdelitems.ImpSubsItemCode;
                            insreport.SubsName = impdelitems.ImpSubsItemName;
                            insreport.ReportCode = "SUBSTITUTIONMISMATCH";
                            insreport.Description = "Substitution mismatch";
                            insreport.DeliveredQty = impdelitems.ImpDeliveryQty;
                            insreport.CreatedBy = impdelitems.CreatedBy;
                            insreport.CreatedDate = DateTime.Now;
                            orderService.SaveOrUpdateInsightReport(insreport);
                        }
                    }
                    OrderItems orderitems = orderService.GetOrderItemsById(poditems.LineId);
                    if (orderitems != null)
                    {

                        orderitems.DeliveredOrdQty = orderitems.DeliveredOrdQty + impdelitems.ImpDeliveryQty;
                        orderitems.AcceptedOrdQty = orderitems.AcceptedOrdQty + impdelitems.ImpDeliveryQty;
                        orderitems.RemainingOrdQty = orderitems.RemainingOrdQty - impdelitems.ImpDeliveryQty;
                        orderitems.SubstituteItemCode = impdelitems.ImpSubsItemCode;
                        orderitems.SubstituteItemName = impdelitems.ImpSubsItemName;
                        if (impdelitems.ImpSubsItemCode != 0 && impdelitems.ImpSubsItemName != null && impdelitems.ImpSubsStatus.ToString().ToUpper() == "SUBSTITUTION")
                        {
                            orderitems.DiscrepancyCode = "AS";
                        }
                        if (impdelitems.ImpSubsItemCode != 0 && impdelitems.ImpSubsItemName != null && impdelitems.ImpSubsStatus.ToString().ToUpper() == "REPLACEMENT")
                        {
                            orderitems.DiscrepancyCode = "AR";
                        }
                        if (poditems.LineId != 0)
                        {
                            if (impdelitems.ImpSubsStatus.ToUpper() == "NONE")
                            {
                                orderService.SaveOrUpdateOrderItems(orderitems);
                            }
                            else if (IndividualSubstitution == "true" || GeneralSubstitution == "true" || IndividualReplacement == "true" || GeneralReplacement == "true")
                            {
                                orderService.SaveOrUpdateOrderItems(orderitems);
                            }
                        }

                        orderitems = orderService.GetOrderItemsById(poditems.LineId);
                        if (orderitems != null)
                        {

                            criteria.Clear();
                            criteria.Add("OrderId", impdelitems.OrderId);
                            criteria.Add("UNCode", impdelitems.ImpUNCode);
                            Dictionary<long, IList<InvQtyandInvValForOrderItems_vw>> invdetails = orderService.GetInvQtyInvVaListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                            IList<InvQtyandInvValForOrderItems_vw> invdetailslist = invdetails.FirstOrDefault().Value;
                            var invqty = (from u in invdetails.First().Value select new { u.InvoiceQty, u.InvoiceValue }).ToArray();
                            orderitems.InvoiceQty = invqty.FirstOrDefault().InvoiceQty;
                            orderitems.InvoiceValue = invqty.FirstOrDefault().InvoiceValue;
                            if (poditems.LineId != 0)
                            {
                                if (impdelitems.ImpSubsStatus.ToUpper() == "NONE")
                                {
                                    orderService.SaveOrUpdateOrderItems(orderitems);
                                }
                                else if (IndividualSubstitution == "true" || GeneralSubstitution == "true" || IndividualReplacement == "true" || GeneralReplacement == "true")
                                {
                                    orderService.SaveOrUpdateOrderItems(orderitems);
                                }
                            }
                        }
                    }


                }

            }

            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }

        }


        #region
        //Populating the imported delivery note using old concept
        public void UpdateImportedDeliveryNoteItemDetailsinOrdItemsTbl1(IList<ImportedDeliveryNoteItems> delnoteitems, byte[] UploadedFile)
        {
            try
            {

                foreach (var impdelitems in delnoteitems)
                {
                    string IndividualSubstitution = string.Empty;
                    string GeneralSubstitution = string.Empty;
                    string IndividualReplacement = string.Empty;
                    string GeneralReplacement = string.Empty;



                    decimal DeliveredQty = 0;
                    //INSIGHT.Entities.User Userobj = (INSIGHT.Entities.User)Session["objUser"];
                    //if (Userobj == null || (Userobj != null && Userobj.UserId == null))
                    //{ methodtest(); }
                    string userid = delnoteitems.FirstOrDefault().CreatedBy;


                    //string userid = Session["UserId"].ToString();
                    //string loggedInUserId = Userobj.UserId;------> need to be clarified
                    //Getting the ImportedDeliveryNoteDetails by id
                    ImportedDeliveryNote impdel = orderService.GetImportedDeliveryNoteDetailsbyImpDeliveryNoteId(impdelitems.ImpDeliveryNoteId);

                    //Getting the orderitems details by orderid
                    Dictionary<string, object> criteria = new Dictionary<string, object>();
                    Dictionary<string, object> likeSearchCriteria = new Dictionary<string, object>();

                    criteria.Clear();
                    criteria.Add("OrderId", impdelitems.OrderId);
                    criteria.Add("UNCode", impdelitems.ImpUNCode);
                    // Dictionary<long, IList<OrderItems>> orditems = orderService.GetOrderItemsListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                    // IList<OrderItems> orditemslist = orditems.FirstOrDefault().Value;
                    Dictionary<long, IList<OrderItems>> orditemslist = orderService.GetOrderItemsListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);


                    //Get deliverynote details by deliverynotename

                    DeliveryNote delnote = orderService.GetDeliverynoteDetailsByDeliveryNoteName(impdelitems.ImpDeliveryNoteName);
                    if (delnote == null)
                    {
                        delnote = new DeliveryNote();

                        delnote.DeliveryNoteName = impdelitems.ImpDeliveryNoteName;
                        delnote.OrderId = impdelitems.OrderId;
                        delnote.DeliveryMode = impdelitems.ImpDeliveryMode;
                        delnote.DeliveryStatus = "Completed";
                        delnote.CreatedBy = impdelitems.CreatedBy;
                        delnote.CreatedDate = DateTime.Now;
                        delnote.DocumentData = UploadedFile;
                        delnote.ActualDeliveryDate = impdel.ImpShipmentDate;
                        delnote.DeliveryNoteType = impdel.ImpDeliveryNoteType;

                        orderService.SaveOrUpdateDeliveryNote(delnote);
                    }

                    //Add values in PODItems


                    POD pod = orderService.GetPODByOrderId(impdel.OrderId);
                    if (pod == null)
                    {
                        pod = new POD();
                        pod.OrderId = impdelitems.OrderId;
                        pod.DeliveryDate = impdel.ImpShipmentDate;
                        pod.CreatedBy = impdelitems.CreatedBy;
                        //pod.CreatedBy = loggedInUserId;----->need to be clarified
                        pod.CreatedDate = DateTime.Now;
                        pod.Status = "PODCompleted";
                        orderService.SaveOrUpdatePOD(pod);
                        orderService.GetPODById(pod.PODId);
                        pod.PODNo = "POD-" + pod.PODId;
                        orderService.SaveOrUpdatePOD(pod);
                    }

                    //Add the values in poditems
                    PODItems poditems = new PODItems();
                    poditems.PODId = pod.PODId;
                    poditems.OrderId = impdelitems.OrderId;
                    //Getting lineid from the orderitems

                    if (orditemslist.FirstOrDefault().Value.Count == 1)
                    {

                        if (orditemslist.FirstOrDefault().Value[0].UNCode == impdelitems.ImpUNCode)
                        {
                            poditems.LineId = orditemslist.FirstOrDefault().Value[0].LineId;
                            poditems.OrderedQty = orditemslist.FirstOrDefault().Value[0].OrderQty;
                        }
                    }
                    else if (orditemslist.FirstOrDefault().Value.Count == 0)
                    {
                        InsightReport insreport = new InsightReport();
                        insreport.OrderId = impdelitems.OrderId;
                        insreport.ControlId = impdelitems.ImpControlId;
                        insreport.DeliveryNoteName = impdelitems.ImpDeliveryNoteName;
                        insreport.UNCode = impdelitems.ImpUNCode;
                        insreport.Commodity = impdelitems.ImpCommodity;
                        insreport.SubsCode = impdelitems.ImpSubsItemCode;
                        insreport.SubsName = impdelitems.ImpSubsItemName;
                        insreport.ReportCode = "UnOrderedDelivery";
                        insreport.Description = "The delivered has been done for the un ordered items";
                        insreport.DeliveredQty = impdelitems.ImpDeliveryQty;
                        insreport.CreatedBy = impdelitems.CreatedBy;
                        insreport.CreatedDate = DateTime.Now;
                        orderService.SaveOrUpdateInsightReport(insreport);
                    }


                    //Getting total deliveredqty for the item based on lineid
                    criteria.Clear();
                    criteria.Add("OrderId", impdelitems.OrderId);
                    criteria.Add("LineId", poditems.LineId);

                    Dictionary<long, IList<DeliveredPODItems_vw>> poditmlist = orderService.GetDeliveredPODItemsListWithPagingAndCriteria(0, 99999, string.Empty, string.Empty, criteria);
                    IList<DeliveredPODItems_vw> poilist = poditmlist.FirstOrDefault().Value;
                    DeliveredQty = (from u in poilist select u.DeliveredQty).Sum();

                    //checking for individual and general  substutions/Replacement  in substitution master
                    if (impdelitems.ImpSubsStatus.ToUpper() != "NONE")
                    {
                        criteria.Clear();
                        Orders ord = new Orders();
                        string[] cri = { "Individual", "General" };
                        ord = orderService.GetOrdersById(impdelitems.OrderId);
                        criteria.Add("Period", ord.Period);
                        criteria.Add("PeriodYear", ord.PeriodYear);
                        criteria.Add("Category", cri);
                        //criteria.Add("Category", "General");
                        criteria.Add("UNCode", impdelitems.ImpUNCode);
                        // likeSearchCriteria.Add("Sector", impdel.Sector);
                        //criteria.Add("Sector", impdel.Sector);
                        //criteria.Add("ControlId", impdelitems.ImpControlId);
                        Dictionary<long, IList<SubstitutionMaster>> subsmstlist = orderService.GetSubstitutionMasterListWithPagingAndCriteria(0, 99999, string.Empty, string.Empty, criteria, likeSearchCriteria);
                        criteria.Clear();
                        IList<SubstitutionMaster> subsmstdetlist = subsmstlist.FirstOrDefault().Value;

                        //Checking the substitution
                        //Individual substitution
                        var IndiSubsList = (from u in subsmstdetlist where u.SubsOrReplace.ToUpper() == "SUBSTITUTION" && u.Category.ToUpper() == "INDIVIDUAL" && u.ControlId == impdelitems.ImpControlId && impdelitems.ImpSubsStatus.ToUpper() == "SUBSTITUTION" select new { u.UNCode, u.ItemName, u.OrderedQty, u.SubstituteItemCode, u.SubstituteItemName, u.AcceptedQty, u.Sector }).ToArray();
                        for (int i = 0; i < IndiSubsList.Length; i++)
                        {
                            if (IndiSubsList[i].UNCode == impdelitems.ImpUNCode && IndiSubsList[i].SubstituteItemCode == impdelitems.ImpSubsItemCode)
                            {
                                IndividualSubstitution = "true";
                                break;
                            }
                        }
                        //General Substitution
                        var GenSubsList = (from u in subsmstdetlist where u.SubsOrReplace.ToUpper() == "SUBSTITUTION" && u.Category.ToUpper() == "GENERAL" && impdelitems.ImpSubsStatus.ToUpper() == "SUBSTITUTION" select new { u.UNCode, u.ItemName, u.OrderedQty, u.SubstituteItemCode, u.SubstituteItemName, u.AcceptedQty, u.Sector }).ToArray();
                        for (int i = 0; i < GenSubsList.Length; i++)
                        {
                            if (GenSubsList[i].UNCode == impdelitems.ImpUNCode && GenSubsList[i].SubstituteItemCode == impdelitems.ImpSubsItemCode && GenSubsList[i].Sector.Contains(impdel.Sector))
                            {
                                GeneralSubstitution = "true";
                                break;
                            }
                        }
                        //Checking the replacement
                        //Individual Replacement
                        var IndiRepList = (from u in subsmstdetlist where u.SubsOrReplace.ToUpper() == "REPLACEMENT" && u.Category.ToUpper() == "INDIVIDUAL" && u.ControlId == impdelitems.ImpControlId && impdelitems.ImpSubsStatus.ToUpper() == "REPLACEMENT" select new { u.UNCode, u.ItemName, u.OrderedQty, u.SubstituteItemCode, u.SubstituteItemName, u.AcceptedQty, u.Sector }).ToArray();
                        for (int i = 0; i < IndiRepList.Length; i++)
                        {
                            if (IndiRepList[i].UNCode == impdelitems.ImpUNCode && IndiRepList[i].SubstituteItemCode == impdelitems.ImpSubsItemCode)
                            {
                                IndividualReplacement = "true";
                                break;
                            }
                        }

                        //General Substitution
                        var GenRepList = (from u in subsmstdetlist where u.SubsOrReplace.ToUpper() == "REPLACEMENT" && u.Category.ToUpper() == "GENERAL" && impdelitems.ImpSubsStatus.ToUpper() == "REPLACEMENT" select new { u.UNCode, u.ItemName, u.OrderedQty, u.SubstituteItemCode, u.SubstituteItemName, u.AcceptedQty, u.Sector }).ToArray();
                        for (int i = 0; i < GenRepList.Length; i++)
                        {
                            if (GenRepList[i].UNCode == impdelitems.ImpUNCode && GenRepList[i].SubstituteItemCode == impdelitems.ImpSubsItemCode && GenRepList[i].Sector.Contains(impdel.Sector))
                            {
                                GeneralReplacement = "true";
                                break;
                            }
                        }


                        // var submst = (from u in subsmstdetlist where u.SubsOrReplace.ToUpper()== "SUBSTITUTION" select new { u.UNCode, u.ItemName, u.OrderedQty, u.SubstituteItemCode, u.SubstituteItemName, u.AcceptedQty,u.Sector}).ToArray();
                        // for (int i = 0; i < submst.Length; i++)
                        // {
                        //     //  if (submst[i].UNCode == impdelitems.ImpUNCode && submst[i].ItemName == impdelitems.ImpCommodity && submst[i].SubstituteItemCode == impdelitems.ImpSubsItemCode && submst[i].SubstituteItemName == impdelitems.ImpSubsItemName)
                        //     if (submst[i].UNCode == impdelitems.ImpUNCode && submst[i].SubstituteItemCode == impdelitems.ImpSubsItemCode && submst[i].Sector.Contains(impdel.Sector))
                        //     {

                        //         SubstitutionFlag = "true";
                        //         break;
                        //     }

                        // }

                        //var repmst = (from u in subsmstdetlist where u.SubsOrReplace.ToUpper() == "REPLACEMENT" select new { u.UNCode, u.ItemName, u.OrderedQty, u.SubstituteItemCode, u.SubstituteItemName, u.AcceptedQty, u.Sector }).ToArray();
                        //for (int i = 0; i < submst.Length; i++)
                        //{
                        //    //  if (submst[i].UNCode == impdelitems.ImpUNCode && submst[i].ItemName == impdelitems.ImpCommodity && submst[i].SubstituteItemCode == impdelitems.ImpSubsItemCode && submst[i].SubstituteItemName == impdelitems.ImpSubsItemName)
                        //    if (submst[i].UNCode == impdelitems.ImpUNCode && submst[i].SubstituteItemCode == impdelitems.ImpSubsItemCode && submst[i].Sector.Contains(impdel.Sector))
                        //    {
                        //        ReplacementFlag = "true";
                        //        break;
                        //    }

                        //}
                    }
                    // poditems.OrderedQty = impdelitems.ImpOrderQty;----------------------->orderqty is taken from the orderitems
                    poditems.DeliveredQty = impdelitems.ImpDeliveryQty;
                    poditems.AcceptedQty = impdelitems.ImpDeliveryQty;
                    //poditems.DeliveredDate = impdel.ImpShipmentDate;
                    if (impdelitems.ImpDeliveryDate != null)
                    {
                        poditems.DeliveredDate = impdelitems.ImpDeliveryDate;
                    }
                    poditems.CreatedBy = impdelitems.CreatedBy;
                    poditems.CreatedDate = DateTime.Now;
                    poditems.SubstituteItemCode = impdelitems.ImpSubsItemCode;
                    poditems.SubstituteItemName = impdelitems.ImpSubsItemName;
                    poditems.RemainingQty = impdelitems.ImpOrderQty - (DeliveredQty + impdelitems.ImpDeliveryQty);
                    poditems.Status = "DeliveryCompleted";
                    poditems.DeliveryNoteId = delnote.DeliveryNoteId;
                    poditems.DeliveryNoteName = delnote.DeliveryNoteName;
                    if (impdelitems.ImpSubsItemCode != 0 && impdelitems.ImpSubsItemName != null && impdelitems.ImpSubsStatus.ToString().ToUpper() == "SUBSTITUTION")
                    {
                        poditems.DiscrepancyCode = "AS";
                    }
                    if (impdelitems.ImpSubsItemCode != 0 && impdelitems.ImpSubsItemName != null && impdelitems.ImpSubsStatus.ToString().ToUpper() == "REPLACEMENT")
                    {
                        poditems.DiscrepancyCode = "AR";
                    }

                    if (poditems.LineId != 0)
                    {
                        if (impdelitems.ImpSubsStatus.ToUpper() == "NONE")
                        {
                            orderService.SaveOrUpdatePODItems(poditems);

                            //Adding invoiceqty and deliverysector in poditems table
                            criteria.Clear();
                            criteria.Add("PODItemsId", poditems.PODItemsId);
                            criteria.Add("OrderId", impdelitems.OrderId);
                            Dictionary<long, IList<InvoiceQtyDelSector_PODItems>> poditems_invqty = orderService.GetPODItems_InvQtyandDeliverySectorListWithPagingAndCriteria(0, 99999, string.Empty, string.Empty, criteria);
                            IList<InvoiceQtyDelSector_PODItems> poditems_invqtylist = poditems_invqty.FirstOrDefault().Value;
                            var InvQty = (from u in poditems_invqtylist select new { u.InvoiceQty, u.DeliverySector, u.PODItemsId }).ToArray();
                            criteria.Clear();
                            poditems = orderService.GetPodItemsValById(poditems.PODItemsId);
                            poditems.InvoiceQty = InvQty.FirstOrDefault().InvoiceQty;
                            poditems.DeliverySector = InvQty.FirstOrDefault().DeliverySector;
                            orderService.SaveOrUpdatePODItems(poditems);

                        }
                        else if (IndividualSubstitution == "true" || GeneralSubstitution == "true" || IndividualReplacement == "true" || GeneralReplacement == "true")
                        {
                            orderService.SaveOrUpdatePODItems(poditems);

                            //Adding invoiceqty and deliverysector in poditems table
                            criteria.Clear();
                            criteria.Add("PODItemsId", poditems.PODItemsId);
                            criteria.Add("OrderId", impdelitems.OrderId);
                            Dictionary<long, IList<InvoiceQtyDelSector_PODItems>> poditems_invqty = orderService.GetPODItems_InvQtyandDeliverySectorListWithPagingAndCriteria(0, 99999, string.Empty, string.Empty, criteria);
                            IList<InvoiceQtyDelSector_PODItems> poditems_invqtylist = poditems_invqty.FirstOrDefault().Value;
                            var InvQty = (from u in poditems_invqtylist select new { u.InvoiceQty, u.DeliverySector, u.PODItemsId }).ToArray();
                            criteria.Clear();
                            poditems = orderService.GetPodItemsValById(poditems.PODItemsId);
                            poditems.InvoiceQty = InvQty.FirstOrDefault().InvoiceQty;
                            poditems.DeliverySector = InvQty.FirstOrDefault().DeliverySector;
                            orderService.SaveOrUpdatePODItems(poditems);
                        }
                        else
                        {
                            InsightReport insreport = new InsightReport();
                            insreport.OrderId = impdelitems.OrderId;
                            insreport.ControlId = impdelitems.ImpControlId;
                            insreport.DeliveryNoteName = impdelitems.ImpDeliveryNoteName;
                            insreport.UNCode = impdelitems.ImpUNCode;
                            insreport.Commodity = impdelitems.ImpCommodity;
                            insreport.SubsCode = impdelitems.ImpSubsItemCode;
                            insreport.SubsName = impdelitems.ImpSubsItemName;
                            insreport.ReportCode = "SUBSTITUTIONMISMATCH";
                            insreport.Description = "Substitution mismatch";
                            insreport.DeliveredQty = impdelitems.ImpDeliveryQty;
                            insreport.CreatedBy = impdelitems.CreatedBy;
                            insreport.CreatedDate = DateTime.Now;
                            orderService.SaveOrUpdateInsightReport(insreport);
                        }
                    }
                    OrderItems orderitems = orderService.GetOrderItemsById(poditems.LineId);
                    if (orderitems != null)
                    {

                        orderitems.DeliveredOrdQty = orderitems.DeliveredOrdQty + impdelitems.ImpDeliveryQty;
                        orderitems.AcceptedOrdQty = orderitems.AcceptedOrdQty + impdelitems.ImpDeliveryQty;
                        orderitems.RemainingOrdQty = orderitems.RemainingOrdQty - impdelitems.ImpDeliveryQty;
                        orderitems.SubstituteItemCode = impdelitems.ImpSubsItemCode;
                        orderitems.SubstituteItemName = impdelitems.ImpSubsItemName;
                        if (impdelitems.ImpSubsItemCode != 0 && impdelitems.ImpSubsItemName != null && impdelitems.ImpSubsStatus.ToString().ToUpper() == "SUBSTITUTION")
                        {
                            orderitems.DiscrepancyCode = "AS";
                        }
                        if (impdelitems.ImpSubsItemCode != 0 && impdelitems.ImpSubsItemName != null && impdelitems.ImpSubsStatus.ToString().ToUpper() == "REPLACEMENT")
                        {
                            orderitems.DiscrepancyCode = "AR";
                        }
                        if (poditems.LineId != 0)
                        {
                            if (impdelitems.ImpSubsStatus.ToUpper() == "NONE")
                            {
                                orderService.SaveOrUpdateOrderItems(orderitems);
                            }
                            else if (IndividualSubstitution == "true" || GeneralSubstitution == "true" || IndividualReplacement == "true" || GeneralReplacement == "true")
                            {
                                orderService.SaveOrUpdateOrderItems(orderitems);
                            }
                        }

                        orderitems = orderService.GetOrderItemsById(poditems.LineId);
                        if (orderitems != null)
                        {

                            criteria.Clear();
                            criteria.Add("OrderId", impdelitems.OrderId);
                            criteria.Add("UNCode", impdelitems.ImpUNCode);
                            Dictionary<long, IList<InvQtyandInvValForOrderItems_vw>> invdetails = orderService.GetInvQtyInvVaListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                            IList<InvQtyandInvValForOrderItems_vw> invdetailslist = invdetails.FirstOrDefault().Value;
                            var invqty = (from u in invdetails.First().Value select new { u.InvoiceQty, u.InvoiceValue }).ToArray();
                            orderitems.InvoiceQty = invqty.FirstOrDefault().InvoiceQty;
                            orderitems.InvoiceValue = invqty.FirstOrDefault().InvoiceValue;
                            if (poditems.LineId != 0)
                            {
                                if (impdelitems.ImpSubsStatus.ToUpper() == "NONE")
                                {
                                    orderService.SaveOrUpdateOrderItems(orderitems);
                                }
                                else if (IndividualSubstitution == "true" || GeneralSubstitution == "true" || IndividualReplacement == "true" || GeneralReplacement == "true")
                                {
                                    orderService.SaveOrUpdateOrderItems(orderitems);
                                }
                            }
                        }
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

        private ActionResult methodtest()
        {
            return RedirectToAction("LogOn", "Account");
        }

        //Deleting order by orderid
        public ActionResult DeleteOrderbyOrderId(string OrderIds)
        {
            try
            {
                //Deleting the orders table 
                //Orders ord = orderService.GetOrdersById(OrderIds);
                //orderService.DeleteOrderbyOrderObj(ord);
                string[] OrderIdArray = OrderIds.Split(',');

                for (int i = 0; i < OrderIdArray.Length; i++)
                {
                    orderService.DeleteOrdersOrdItemsPodPodItemsDelNoteByOrderId(Convert.ToInt64(OrderIdArray[i]));
                }


                return null;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }

        }
        #endregion

        # region new chnages
        //Updating delivery date in poditems table

        public ActionResult SaveDeliveredDateinPODItemsTblForAll(string AllPODItemsId, string AllDeliveredDate)
        {
            try
            {
                if (AllPODItemsId != null && AllDeliveredDate != null)
                {
                    string[] PODItemsIdArray = AllPODItemsId.Split(',');
                    string[] DeliveryDateArray = AllDeliveredDate.Split(',');

                    for (int i = 0; i < PODItemsIdArray.Count(); i++)
                    {

                        PODItems pod = orderService.GetPodItemsValById(Convert.ToInt64(PODItemsIdArray[i]));
                        if (pod != null)
                        {
                            if (!string.IsNullOrEmpty(DeliveryDateArray[i]))
                                pod.DeliveredDate = DateTime.Parse(DeliveryDateArray[i], provider, System.Globalization.DateTimeStyles.NoCurrentDateDefault);
                            orderService.SaveOrUpdatePODItems(pod);
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
        //Add new poditems in deliverynote
        public ActionResult AddNewPODIteminDeliveryNote(string DeliveryNoteName, long OrderId, long PODId, long DeliveryNoteId)
        {

            ViewBag.OrderId = OrderId;
            ViewBag.DeliveryNoteName = DeliveryNoteName;
            ViewBag.PODId = PODId;
            ViewBag.DeliveryNoteId = DeliveryNoteId;
            return View();
        }


        //Autocomplete item code for add new item in deliverynote
        public JsonResult GetItemName(string Commodtity, long OrderId)
        {
            try
            {

                MastersService mssvc = new MastersService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                criteria.Add("Commodtity", Commodtity);
                criteria.Add("OrderId", OrderId);
                Dictionary<long, IList<PODOrderItems_vw>> orditems = orderService.GetPODOrderItemsListWithLikeSearchPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                var Commodity = (from u in orditems.First().Value
                                 where u.Commodity != null
                                 select u.Commodity).Distinct().ToList();
                return Json(Commodity, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }

        //Get item code and orderqty and deliveryqty using autocomplete
        public JsonResult AutoCompleteItemcodeandOrderQty(string itemname, long OrderId)
        {
            try
            {
                MastersService mssvc = new MastersService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                criteria.Add("Commodity", itemname);
                criteria.Add("OrderId", OrderId);
                Dictionary<long, IList<PODOrderItems_vw>> orditems = orderService.GetPODOrderItemsListWithLikeSearchPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                var ItemDetails = (from u in orditems.First().Value

                                   select new { u.UNCode, u.OrderQty, u.RemainingOrdQty, u.LineId }).Distinct().ToList();
                return Json(ItemDetails, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }
        //Add PODitems in deliverynote page
        public ActionResult AddPODItemsInDeliveryNotePage(long UNCode, string Commodity, decimal OrderQty, decimal DelQty, decimal RemainingQty, long SubUNCode, string SubCommodity, long PODId, string DeliveryNoteName, long DeliveryNoteId, long OrderId, long LineId)
        {
            try
            {
                PODItems poditems = new PODItems();
                poditems.LineId = LineId;
                poditems.OrderedQty = OrderQty;
                poditems.DeliveredQty = DelQty;
                poditems.RemainingQty = RemainingQty;
                poditems.SubstituteItemCode = SubUNCode;
                poditems.SubstituteItemName = SubCommodity;
                poditems.DeliveryNoteId = DeliveryNoteId;
                poditems.DeliveryNoteName = DeliveryNoteName;
                poditems.PODId = PODId;
                poditems.OrderId = OrderId;
                orderService.SaveOrUpdatePODItems(poditems);


                //Updating in orderitems
                OrderItems orditems = orderService.GetOrderItemsById(LineId);
                orditems.DeliveredOrdQty = orditems.DeliveredOrdQty + DelQty;
                orditems.RemainingOrdQty = RemainingQty;
                orditems.SubstituteItemCode = SubUNCode;
                orditems.SubstituteItemName = SubCommodity;
                orderService.SaveOrUpdateOrderItems(orditems);


                return null;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }

        }
        //Delete poditems in the deliverynotepage
        public ActionResult DeletePODItemsInDeliveryNotePage(string PODItemsIds)
        {
            try
            {
                string[] PODItemsIdArray = PODItemsIds.ToString().Split(',');
                for (int i = 0; i < PODItemsIdArray.Count(); i++)
                {
                    PODItems poditems = orderService.GetPodItemsValById(Convert.ToInt64(PODItemsIdArray[i]));


                    //updating in order items table
                    OrderItems orditems = orderService.GetOrderItemsById(poditems.LineId);
                    orditems.DeliveredOrdQty = orditems.DeliveredOrdQty - poditems.DeliveredQty;
                    orditems.RemainingOrdQty = orditems.OrderQty - orditems.DeliveredOrdQty;
                    orderService.SaveOrUpdateOrderItems(orditems);

                    orderService.DeletePODItemsbyObj(poditems);
                }


                return null;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }

        }
        //Delete Deliverynote by deliverynoteid
        public JsonResult DeleteDeliveryNote(string DeliveryNoteIds, string DeliveryNoteNames)
        {
            try
            {
                //new Task(() =>
                //{
                DeleteDeliveryNoteParallel(DeliveryNoteIds, DeliveryNoteNames);



                //}).Start();



                //var DeliverynoteIdArray = DeliveryNoteIds.Split(',');
                //var DeliveryNoteNamesArray = DeliveryNoteNames.Split(',');
                //orderService = new OrdersService();
                //Dictionary<string, object> criteria = new Dictionary<string, object>();

                //if (DeliverynoteIdArray.Count() == DeliveryNoteNamesArray.Count())
                //{

                //    for (int i = 0; i < DeliverynoteIdArray.Count(); i++)
                //    {


                //        criteria.Add("DeliveryNoteId", Convert.ToInt64(DeliverynoteIdArray[i]));
                //        criteria.Add("DeliveryNoteName", DeliveryNoteNamesArray[i]);
                //        Dictionary<long, IList<DeliveredPODItems_vw>> poditmlist = orderService.GetDeliveredPODItemsListWithPagingAndCriteria(0, 99999, string.Empty, string.Empty, criteria);
                //        for (int j = 0; j < poditmlist.FirstOrDefault().Value.Count(); j++)
                //        {
                //            var SubstituteItemCode = poditmlist.FirstOrDefault().Value[j].SubstituteItemCode;
                //            var SubstituteItemName = poditmlist.FirstOrDefault().Value[j].SubstituteItemName;
                //            var actualitemcode = poditmlist.FirstOrDefault().Value[j].UNCode;
                //            var actualitemname = poditmlist.FirstOrDefault().Value[j].Commodity;
                //            OrderItems orditems = orderService.GetOrderItemsById(poditmlist.FirstOrDefault().Value[j].LineId);
                //            orditems.DeliveredOrdQty = orditems.DeliveredOrdQty - poditmlist.FirstOrDefault().Value[j].DeliveredQty;
                //            orditems.AcceptedOrdQty = orditems.AcceptedOrdQty - poditmlist.FirstOrDefault().Value[j].AcceptedQty;
                //            orditems.RemainingOrdQty = orditems.RemainingOrdQty + poditmlist.FirstOrDefault().Value[j].DeliveredQty;
                //            if (SubstituteItemCode != 0 && SubstituteItemCode == orditems.SubstituteItemCode && actualitemcode == orditems.UNCode)
                //            {
                //                orditems.SubstituteItemCode = 0;
                //                orditems.SubstituteItemName = "";
                //                orditems.DiscrepancyCode = null;
                //            }
                //            orderService.SaveOrUpdateOrderItems(orditems);
                //        }

                //        //Delete from deliverynote and importeddeliverynote
                //        orderService.DeleteDeliveryNoteandImportedDeliveryNoteTbl(Convert.ToInt64(DeliverynoteIdArray[i]), DeliveryNoteNamesArray[i]);

                //        //Delete the poditems
                //        orderService.DeletePODItemsByDeliveryNoteId(Convert.ToInt64(DeliverynoteIdArray[i]));
                //        criteria.Clear();
                //    }


                //}

                // return null;
                return Json("test", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }

        }

        public ActionResult DeleteDeliveryNoteParallel(string DeliveryNoteIds, string DeliveryNoteNames)
        {

            var DeliverynoteIdArray = DeliveryNoteIds.Split(',');
            var DeliveryNoteNamesArray = DeliveryNoteNames.Split(',');
            orderService = new OrdersService();
            Dictionary<string, object> criteria = new Dictionary<string, object>();

            if (DeliverynoteIdArray.Count() == DeliveryNoteNamesArray.Count())
            {

                for (int i = 0; i < DeliverynoteIdArray.Count(); i++)
                {


                    criteria.Add("DeliveryNoteId", Convert.ToInt64(DeliverynoteIdArray[i]));
                    criteria.Add("DeliveryNoteName", DeliveryNoteNamesArray[i]);
                    Dictionary<long, IList<DeliveredPODItems_vw>> poditmlist = orderService.GetDeliveredPODItemsListWithPagingAndCriteria(0, 99999, string.Empty, string.Empty, criteria);
                    for (int j = 0; j < poditmlist.FirstOrDefault().Value.Count(); j++)
                    {
                        var SubstituteItemCode = poditmlist.FirstOrDefault().Value[j].SubstituteItemCode;
                        var SubstituteItemName = poditmlist.FirstOrDefault().Value[j].SubstituteItemName;
                        var actualitemcode = poditmlist.FirstOrDefault().Value[j].UNCode;
                        var actualitemname = poditmlist.FirstOrDefault().Value[j].Commodity;
                        OrderItems orditems = orderService.GetOrderItemsById(poditmlist.FirstOrDefault().Value[j].LineId);
                        orditems.DeliveredOrdQty = orditems.DeliveredOrdQty - poditmlist.FirstOrDefault().Value[j].DeliveredQty;
                        orditems.AcceptedOrdQty = orditems.AcceptedOrdQty - poditmlist.FirstOrDefault().Value[j].AcceptedQty;
                        orditems.RemainingOrdQty = orditems.RemainingOrdQty + poditmlist.FirstOrDefault().Value[j].DeliveredQty;
                        if (SubstituteItemCode != 0 && SubstituteItemCode == orditems.SubstituteItemCode && actualitemcode == orditems.UNCode)
                        {
                            orditems.SubstituteItemCode = 0;
                            orditems.SubstituteItemName = "";
                            orditems.DiscrepancyCode = null;
                        }
                        orderService.SaveOrUpdateOrderItems(orditems);
                    }

                    //Delete from deliverynote and importeddeliverynote
                    orderService.DeleteDeliveryNoteandImportedDeliveryNoteTbl(Convert.ToInt64(DeliverynoteIdArray[i]), DeliveryNoteNamesArray[i]);

                    //Delete the poditems
                    orderService.DeletePODItemsByDeliveryNoteId(Convert.ToInt64(DeliverynoteIdArray[i]));
                    criteria.Clear();
                }


            }

            // return RedirectToAction("DeliveryNoteList","Orders");
            return null;
        }

        //public void DeleteDeliveryNote(string DeliveryNoteIds, string DeliveryNoteNames)
        //{
        //    try
        //    {
        //        var DeliverynoteIdArray = DeliveryNoteIds.Split(',');
        //        var DeliveryNoteNamesArray = DeliveryNoteNames.Split(',');
        //        orderService = new OrdersService();
        //        Dictionary<string, object> criteria = new Dictionary<string, object>();

        //        if (DeliverynoteIdArray.Count() == DeliveryNoteNamesArray.Count())
        //        {

        //            for (int i = 0; i < DeliverynoteIdArray.Count(); i++)
        //            {


        //                criteria.Add("DeliveryNoteId", Convert.ToInt64(DeliverynoteIdArray[i]));
        //                criteria.Add("DeliveryNoteName", DeliveryNoteNamesArray[i]);
        //                Dictionary<long, IList<DeliveredPODItems_vw>> poditmlist = orderService.GetDeliveredPODItemsListWithPagingAndCriteria(0, 99999, string.Empty, string.Empty, criteria);
        //                for (int j = 0; j < poditmlist.FirstOrDefault().Value.Count(); j++)
        //                {
        //                    var SubstituteItemCode = poditmlist.FirstOrDefault().Value[j].SubstituteItemCode;
        //                    var SubstituteItemName = poditmlist.FirstOrDefault().Value[j].SubstituteItemName;
        //                    var actualitemcode = poditmlist.FirstOrDefault().Value[j].UNCode;
        //                    var actualitemname = poditmlist.FirstOrDefault().Value[j].Commodity;
        //                    OrderItems orditems = orderService.GetOrderItemsById(poditmlist.FirstOrDefault().Value[j].LineId);
        //                    orditems.DeliveredOrdQty = orditems.DeliveredOrdQty - poditmlist.FirstOrDefault().Value[j].DeliveredQty;
        //                    orditems.AcceptedOrdQty = orditems.AcceptedOrdQty - poditmlist.FirstOrDefault().Value[j].AcceptedQty;
        //                    orditems.RemainingOrdQty = orditems.RemainingOrdQty + poditmlist.FirstOrDefault().Value[j].DeliveredQty;
        //                    if (SubstituteItemCode != 0 && SubstituteItemCode == orditems.SubstituteItemCode && actualitemcode == orditems.UNCode)
        //                    {
        //                        orditems.SubstituteItemCode = 0;
        //                        orditems.SubstituteItemName = "";
        //                        orditems.DiscrepancyCode = null;
        //                    }
        //                    orderService.SaveOrUpdateOrderItems(orditems);
        //                }

        //                //Delete from deliverynote and importeddeliverynote
        //                orderService.DeleteDeliveryNoteandImportedDeliveryNoteTbl(Convert.ToInt64(DeliverynoteIdArray[i]), DeliveryNoteNamesArray[i]);

        //                //Delete the poditems
        //                orderService.DeletePODItemsByDeliveryNoteId(Convert.ToInt64(DeliverynoteIdArray[i]));
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
        //        throw ex;
        //    }

        //}
        #endregion

        #region Added By John (Deleted Order,Deliverynote,Invoice,OrderItem,Pod,Poditem)

        public JsonResult DeleteOrders(string OrdersId, string DeliveryNoteId, string OrderItemId, string PodId, string PodItemId, string InvoiceId)
        {
            string userId = base.ValidateUser();
            bool Result = false;
            if (string.IsNullOrWhiteSpace(userId)) return Json(Result, JsonRequestBehavior.AllowGet);
            else
            {
                try
                {
                    Dictionary<string, object> criteria = new Dictionary<string, object>();
                    string[] arrOrderId = new string[0];
                    string[] arrOrderItemId = new string[0];
                    string[] arrPodId = new string[0];
                    string[] arrPodItemsId = new string[0];
                    string[] arrDeliveryNoteId = new string[0];
                    string[] arrInvoiceId = new string[0];

                    bool isOrdersInvolve = false;
                    //for Deleting records in Main table
                    IList<Orders> orderlist = null;
                    IList<OrderItems> orderItemlist = null;
                    IList<POD> podList = null;
                    IList<PODItems> podItemList = null;
                    IList<DeliveryNote> deliveryNoteList = null;
                    IList<Invoice> invoiceList = null;

                    //For Inserting Records in Delete tables
                    IList<OrdersDel> orderDellist = new List<OrdersDel>();
                    IList<OrderItemsDel> orderDelItemlist = new List<OrderItemsDel>();
                    IList<PODDel> podDelList = new List<PODDel>();
                    IList<PODItemsDel> podItemDelList = new List<PODItemsDel>();
                    IList<DeliveryNoteDel> deliveryNoteDelList = new List<DeliveryNoteDel>();
                    IList<InvoiceDel> invoiceDelList = new List<InvoiceDel>();

                    //string to array with comma separater
                    if (!string.IsNullOrEmpty(OrdersId))
                        arrOrderId = OrdersId.Split(',');
                    if (!string.IsNullOrEmpty(OrderItemId))
                        arrOrderItemId = OrderItemId.Split(',');
                    if (!string.IsNullOrEmpty(PodId))
                        arrPodId = PodId.Split(',');
                    if (!string.IsNullOrEmpty(PodItemId))
                        arrPodItemsId = PodItemId.Split(',');
                    if (!string.IsNullOrEmpty(DeliveryNoteId))
                        arrDeliveryNoteId = DeliveryNoteId.Split(',');
                    if (!string.IsNullOrEmpty(InvoiceId))
                        arrInvoiceId = InvoiceId.Split(',');



                    //Orders
                    if (!string.IsNullOrEmpty(OrdersId))
                    {
                        isOrdersInvolve = true;
                        criteria.Clear();
                        if (arrOrderId.Count() != 0)
                            criteria.Add("OrderId", arrOrderId);
                        Dictionary<long, IList<Orders>> ords = orderService.GetOrdersListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                        orderlist = ords.FirstOrDefault().Value.ToList();

                        if (orderlist.Count() != 0)
                        {
                            //using orderlist make delete list
                            long idCount = orderService.GetCurrentIntent("Orders_Del");
                            foreach (var item in orderlist)
                            {
                                OrdersDel od = new OrdersDel();
                                od.Id = idCount;
                                od.OrderId = item.OrderId;
                                od.InvoiceId = item.InvoiceId;
                                od.PODId = item.PODId;
                                od.Name = item.Name;
                                od.ContingentType = item.ContingentType;
                                od.Location = item.Location;
                                od.ControlId = item.ControlId;
                                od.StartDate = item.StartDate;
                                od.EndDate = item.EndDate;
                                od.Troops = item.Troops;
                                od.TotalAmount = item.TotalAmount;
                                od.LineItemsOrdered = item.LineItemsOrdered;
                                od.KgOrderedWOEggs = item.KgOrderedWOEggs;
                                od.EggsWeight = item.EggsWeight;
                                od.TotalWeight = item.TotalWeight;
                                od.CreatedBy = item.CreatedBy;
                                od.CreatedDate = item.CreatedDate;
                                od.ModifiedBy = item.ModifiedBy;
                                od.ModifiedDate = item.ModifiedDate;
                                od.LocationCMR = item.LocationCMR;
                                od.ControlCMR = item.ControlCMR;
                                od.Period = item.Period;
                                od.Sector = item.Sector;
                                od.Week = item.Week;
                                od.PeriodYear = item.PeriodYear;
                                od.ExpectedDeliveryDate = item.ExpectedDeliveryDate != null ? item.ExpectedDeliveryDate : null;
                                od.FinalStatus = item.FinalStatus != null ? item.FinalStatus : "";
                                od.OpeningStatus = item.OpeningStatus;
                                od.InvoiceStatus = item.InvoiceStatus;
                                od.CalYear = item.CalYear;
                                od.DeletedBy = userId;
                                od.DeletedDate = DateTime.Now;
                                orderDellist.Add(od);
                                idCount = idCount + 1;
                            }
                        }
                    }

                    //OrderItems
                    if ((!string.IsNullOrEmpty(OrderItemId)) || (isOrdersInvolve == true))
                    {
                        criteria.Clear();
                        if (arrOrderItemId.Count() != 0)
                            criteria.Add("LineId", arrOrderItemId);
                        if (arrOrderId.Count() != 0)
                            criteria.Add("OrderId", arrOrderId);
                        Dictionary<long, IList<OrderItems>> ordsitems = orderService.GetOrderItemsListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                        orderItemlist = ordsitems.FirstOrDefault().Value.ToList();

                        if (orderItemlist.Count() != 0)
                        {
                            long idCount = orderService.GetCurrentIntent("OrderItems_Del");
                            //using orderlist make delete list
                            foreach (var item in orderItemlist)
                            {
                                OrderItemsDel oid = new OrderItemsDel();
                                oid.Id = idCount;
                                oid.LineId = item.LineId;
                                oid.OrderId = item.OrderId;
                                oid.UNCode = item.UNCode;
                                oid.Commodity = item.Commodity;
                                oid.OrderQty = item.OrderQty;
                                oid.SectorPrice = item.SectorPrice;
                                oid.Total = item.Total;
                                oid.CreatedBy = item.CreatedBy;
                                oid.CreatedDate = item.CreatedDate != null ? item.CreatedDate : null;
                                oid.ModifiedBy = item.ModifiedBy;
                                oid.ModifiedDate = item.ModifiedDate != null ? item.ModifiedDate : null;
                                oid.AcceptedOrdQty = item.AcceptedOrdQty;
                                oid.DeliveredOrdQty = item.DeliveredOrdQty;
                                oid.RemainingOrdQty = item.RemainingOrdQty;
                                oid.Status = item.Status;
                                oid.SubstituteItemCode = item.SubstituteItemCode;
                                oid.SubstituteItemName = item.SubstituteItemName;
                                oid.InvoiceQty = item.InvoiceQty;
                                oid.InvoiceValue = item.InvoiceValue;
                                oid.DiscrepancyCode = item.DiscrepancyCode;
                                oid.DeletedBy = userId;
                                oid.DeletedDate = DateTime.Now;
                                orderDelItemlist.Add(oid);
                                idCount = idCount + 1;
                            }
                        }
                    }

                    //POD
                    if ((!string.IsNullOrEmpty(PodId)) || (isOrdersInvolve == true))
                    {
                        criteria.Clear();
                        if (arrPodId.Count() != 0)
                            criteria.Add("PODId", arrPodId);
                        if (arrOrderId.Count() != 0)
                            criteria.Add("OrderId", arrOrderId);
                        Dictionary<long, IList<POD>> podLists = orderService.GetPODMasterListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                        podList = podLists.FirstOrDefault().Value.ToList();

                        if (podList.Count() != 0)
                        {
                            long idCount = orderService.GetCurrentIntent("POD_Del");
                            foreach (var item in podList)
                            {
                                PODDel pd = new PODDel();
                                pd.Id = idCount;
                                pd.PODId = item.PODId;
                                pd.PODNo = item.PODNo;
                                pd.CreatedBy = item.CreatedBy;
                                pd.CreatedDate = item.CreatedDate != null ? item.CreatedDate : null;
                                pd.DeliveryDate = item.DeliveryDate != null ? item.DeliveryDate : null;
                                pd.OrderId = item.OrderId;
                                pd.Status = item.Status;
                                pd.DeletedBy = userId;
                                pd.DeletedDate = DateTime.Now;
                                podDelList.Add(pd);
                                idCount = idCount + 1;
                            }
                        }
                    }

                    //POD items
                    if ((!string.IsNullOrEmpty(PodItemId)) || (isOrdersInvolve == true))
                    {
                        criteria.Clear();
                        if (arrPodItemsId.Count() != 0)
                            criteria.Add("PODItemsId", arrPodItemsId);
                        if (arrOrderId.Count() != 0)
                            criteria.Add("OrderId", arrOrderId);
                        Dictionary<long, IList<PODItems>> podItems = orderService.GetPODItemsListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                        podItemList = podItems.FirstOrDefault().Value.ToList();

                        if (podItemList.Count() != 0)
                        {
                            long idCount = orderService.GetCurrentIntent("PODItems_Del");
                            foreach (var item in podItemList)
                            {
                                PODItemsDel pid = new PODItemsDel();
                                pid.Id = idCount;
                                pid.PODItemsId = item.PODItemsId;
                                pid.PODId = item.PODId;
                                pid.OrderId = item.OrderId;
                                pid.LineId = item.LineId;
                                pid.OrderedQty = item.OrderedQty;
                                pid.DeliveredQty = item.DeliveredQty;
                                pid.AcceptedQty = item.AcceptedQty;
                                pid.DeliveredDate = item.DeliveredDate != null ? item.DeliveredDate : null;
                                pid.CreatedBy = item.CreatedBy;
                                pid.CreatedDate = item.CreatedDate != null ? item.CreatedDate : null;
                                pid.ModifiedBy = item.ModifiedBy;
                                pid.ModifiedDate = item.ModifiedDate != null ? item.ModifiedDate : null;
                                pid.SubstituteItemCode = item.SubstituteItemCode;
                                pid.SubstituteItemName = item.SubstituteItemName;
                                pid.RemainingQty = item.RemainingQty;
                                pid.Status = item.Status;
                                pid.SubsDeliveredQty = item.SubsDeliveredQty;
                                pid.SubsAcceptedQty = item.SubsAcceptedQty;
                                pid.DeliveryNoteId = item.DeliveryNoteId;
                                pid.DeliveryNoteName = item.DeliveryNoteName;
                                pid.DiscrepancyCode = item.DiscrepancyCode;
                                pid.DeletedBy = userId;
                                pid.DeletedDate = DateTime.Now;
                                podItemDelList.Add(pid);
                                idCount = idCount + 1;
                            }
                        }
                    }

                    //Delivery Note
                    if ((!string.IsNullOrEmpty(DeliveryNoteId)) || (isOrdersInvolve == true))
                    {

                        criteria.Clear();
                        if (arrDeliveryNoteId.Count() != 0)
                            criteria.Add("DeliveryNoteId", arrDeliveryNoteId);
                        if (arrOrderId.Count() != 0)
                            criteria.Add("OrderId", arrOrderId);
                        Dictionary<long, IList<DeliveryNote>> deliveryNote = orderService.GetDeliveryNoteListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                        deliveryNoteList = deliveryNote.FirstOrDefault().Value.ToList();

                        if (deliveryNoteList.Count() != 0)
                        {
                            long idCount = orderService.GetCurrentIntent("DeliveryNote_Del");
                            foreach (var item in deliveryNoteList)
                            {
                                DeliveryNoteDel dnd = new DeliveryNoteDel();
                                dnd.Id = idCount;
                                dnd.DeliveryNoteId = item.DeliveryNoteId;
                                dnd.DeliveryNoteName = item.DeliveryNoteName;
                                dnd.CreatedBy = item.CreatedBy;
                                dnd.CreatedDate = item.CreatedDate;
                                dnd.OrderId = item.OrderId;
                                dnd.DeliveryStatus = item.DeliveryStatus;
                                dnd.DeliveryMode = item.DeliveryMode;
                                dnd.DeletedBy = userId;
                                dnd.DeletedDate = DateTime.Now;
                                deliveryNoteDelList.Add(dnd);
                                idCount = idCount + 1;
                            }
                        }
                    }

                    //Invoice 
                    if ((!string.IsNullOrEmpty(InvoiceId)) || (isOrdersInvolve == true))
                    {
                        criteria.Clear();
                        if (arrInvoiceId.Count() != 0)
                            criteria.Add("Id", arrInvoiceId);
                        if (arrOrderId.Count() != 0)
                            criteria.Add("OrderId", arrOrderId);
                        Dictionary<long, IList<Invoice>> invoice = IS.GetInvoicetableListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                        invoiceList = invoice.FirstOrDefault().Value.ToList();

                        if (invoiceList.Count() != 0)
                        {
                            long idCount = orderService.GetCurrentIntent("Invoice_Del");
                            foreach (var item in invoiceList)
                            {
                                InvoiceDel id = new InvoiceDel();
                                id.Id = idCount;
                                id.InvoiceId = item.Id;
                                id.InvoiceCode = item.InvoiceCode;
                                id.Contract = item.Contract;
                                id.PaymentTerms = item.PaymentTerms;
                                id.PO = item.PO;
                                id.Period = item.Period;
                                id.Week = item.Week;
                                id.UNINo = item.UNINo;
                                id.Sector = item.Sector;
                                id.TotalFeedTroopStrength = item.TotalFeedTroopStrength;
                                id.TotalMadays = item.TotalMadays;
                                id.CMR = item.CMR;
                                id.SubTotal = item.SubTotal;
                                id.GrandTotal = item.GrandTotal;
                                id.CreatedDate = item.CreatedDate != null ? item.CreatedDate : null;
                                id.CreatedBy = item.CreatedBy;
                                id.ModifiedBy = item.ModifiedBy;
                                id.ModifiedDate = item.ModifiedDate != null ? item.ModifiedDate : null;
                                id.OrderId = item.OrderId;
                                id.PeriodYear = item.PeriodYear;
                                id.DeletedBy = userId;
                                id.DeletedDate = DateTime.Now;
                                invoiceDelList.Add(id);
                                idCount = idCount + 1;
                            }
                        }
                    }

                    bool saveStatus = orderService.SaveDeletedListofOrdersList(orderDellist, orderDelItemlist, podDelList, podItemDelList, deliveryNoteDelList, invoiceDelList);
                    bool deleteStatus = orderService.DeletedListofOrdersList(orderlist, orderItemlist, podList, podItemList, deliveryNoteList, invoiceList);
                    Result = true;
                    return Json(Result, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                    throw ex;
                }
            }
        }

        #endregion


        #region   Bulk upload(Orders and Delivery Note related code)
        public ActionResult OrdersBulkuploadRequestCreation(long? RequestId)
        {

            try
            {

                //  long RequestIdTemp = RequestId != null ? RequestId : 0;
                //INSIGHT.Entities.User Userobj = (INSIGHT.Entities.User)Session["objUser"];
                //if (Userobj == null || (Userobj != null && Userobj.UserId == null))
                //{ return RedirectToAction("LogOn", "Account"); }
                //  string loggedInUserId = Userobj.UserId;
                // ViewBag.CreatedBy = loggedInUserId;
                // ViewBag.CreatedDate = DateTime.Now.ToShortDateString();
                // ViewBag.Status = "New";

                UploadRequest upreq = new UploadRequest();
                if (RequestId != null)
                {
                    upreq = orderService.GetUploadRequestbyRequestId(RequestId ?? 0);

                }
                return View(upreq);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult OrdersBulkuploadRequestCreation(UploadRequest upreq)
        {
            try
            {
                INSIGHT.Entities.User Userobj = (INSIGHT.Entities.User)Session["objUser"];
                if (Userobj == null || (Userobj != null && Userobj.UserId == null))
                { return RedirectToAction("LogOn", "Account"); }
                string loggedInUserId = Userobj.UserId;
                upreq.Category = "ORDERSUPLOAD";
                upreq.CreatedBy = loggedInUserId;
                upreq.CreatedDate = DateTime.Now;
                long reqid = orderService.SaveOrUpdateUploadRequest(upreq);
                upreq.RequestNo = "REQUEST-" + reqid;
                upreq.Status = "OPEN";
                upreq.UploadStatus = "Request Generated";
                orderService.SaveOrUpdateUploadRequest(upreq);
                //UploadRequest upreq1 = orderService.GetUploadRequestbyRequestId(reqid);
                //orderService.SaveOrUpdateUploadRequest(upreq);
                return (RedirectToAction("OrdersBulkuploadRequestCreation", new { RequestId = upreq.RequestId }));
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }

        //[HttpPost]
        //public ActionResult OrdersBulkuploadRequestCreation(HttpPostedFileBase[] uploadedFile)
        //{
        //    try
        //{
        //insight.entities.user userobj = (insight.entities.user)session["objuser"];
        //if (userobj == null || (userobj != null && userobj.userid == null))
        //{ return redirecttoaction("logon", "account"); }
        // string loggedinuserid = userobj.userid;

        //UploadRequest upreq = new UploadRequest();
        //upreq.Category = "ORDERSUPLOAD";
        //upreq.CreatedBy = loggedInUserId;
        //upreq.CreatedDate = DateTime.Now;
        //long reqid = orderService.SaveOrUpdateUploadRequest(upreq);
        //upreq= orderService.GetUploadRequestbyRequestId(reqid);
        //upreq.RequestName = "REQ-" + upreq.RequestId;
        //orderService.SaveOrUpdateUploadRequest(upreq);

        //for (int l = 0; l < uploadedFile.Length; l++)
        //{
        //    UploadRequestDetailsLog uploadlog = new UploadRequestDetailsLog();
        //    string path = uploadedFile[l].InputStream.ToString();
        //    byte[] imageSize = new byte[uploadedFile[l].ContentLength];
        //    uploadedFile[l].InputStream.Read(imageSize, 0, (int)uploadedFile[l].ContentLength);
        //    string fileName = uploadedFile[l].FileName;
        //    string fileExtn = Path.GetExtension(uploadedFile[l].FileName);
        //    string fileLocation = ConfigurationManager.AppSettings["BulkOrdersUploadFilePath"].ToString()+"\\" + upreq.RequestName+ "\\"+DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss").Replace(":", ".") + fileName;
        //    uploadedFile[l].SaveAs(fileLocation);
        //    uploadlog.RequestId = upreq.RequestId;
        //    uploadlog.FileName = fileName;
        //    uploadlog.CreatedBy = loggedInUserId;
        //    uploadlog.CreatedDate = DateTime.Now;
        //    orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);


        //}
        //        return null;
        //    }
        //    catch (Exception ex) {
        //        ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
        //        throw ex;

        //    }
        //}

        //Added  by jp

        public ActionResult Upload()
        {
            int ss = 0;
            var files = Request.Files["Filedata"];
            string savePath = Server.MapPath(@"~\Content\FileuploadFolder\" + files.FileName);
            files.SaveAs(savePath);
            string fileName = string.Empty;
            int length = files.ContentLength;
            fileName = files.FileName;
            long UploadReqDetLogId = 0;
            //Saving the bulk order upload request
            //long UploadReqDetLogId = CreateOrUpdateRequest(fileName);----> change the parameter
            UploadRequestDetailsLog uploadlog = orderService.GetUploadRequestDetailsLogbyRequestId(UploadReqDetLogId);//changed by kingston for testing
            try
            {
                //the bulk upload status related method
                INSIGHT.Entities.User Userobj = (INSIGHT.Entities.User)Session["objUser"];
                int AlreadyExistFile = 0;
                //string path = files.InputStream.ToString();
                //byte[] imageSize = new byte[files.ContentLength];
                //files.InputStream.Read(imageSize, 0, (int)files.ContentLength);
                string UploadConnStr = string.Empty;
                fileName = files.FileName;
                string fileExtn = Path.GetExtension(files.FileName);
                string fileLocation = savePath;

                if (fileExtn == ".xls")
                {
                    UploadConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\"";
                }
                if (fileExtn == ".xlsx")
                {
                    UploadConnStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";
                }
                OleDbConnection conn = new OleDbConnection();
                DataTable DtblXcelData = new DataTable();
                string QeryToGetXcelData = "select * from " + string.Format("{0}${1}", "[Form", "A1:AZ]");
                conn.ConnectionString = UploadConnStr;
                conn.Open();
                OleDbCommand cmd = new OleDbCommand(QeryToGetXcelData, conn);
                cmd.CommandType = CommandType.Text;
                OleDbDataAdapter DtAdptrr = new OleDbDataAdapter();
                DtAdptrr.SelectCommand = cmd;
                DtAdptrr.Fill(DtblXcelData);
                conn.Close();
                if (DtblXcelData.Rows.Count == 0)
                {
                    if (uploadlog.UploadStatus == "YetToUpload")
                        uploadlog.UploadStatus = "UploadFailed";
                    uploadlog.ErrDesc = "Empty file uploaded";
                    orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);

                }
                else if (DtblXcelData.Rows.Count > 0)
                {
                    QeryToGetXcelData = "select * from " + string.Format("{0}${1}", "[Form", "A7:AZ]");
                    conn.ConnectionString = UploadConnStr;
                    conn.Open();
                    cmd = new OleDbCommand(QeryToGetXcelData, conn);
                    cmd.CommandType = CommandType.Text;
                    DtAdptrr = new OleDbDataAdapter();
                    DtAdptrr.SelectCommand = cmd;
                    DtAdptrr.Fill(DtblXcelData);
                    string[] strArray = { "F1", "F2", "F3", "F4", "F5", "UNCode", "Commodity", "Order Qty (kg/lt/ea)", "Sector Price", "Total" };
                    char chrFlag = 'Y';
                    if (DtblXcelData.Columns.Count == strArray.Length)
                    {
                        int j = 0;
                        string[] strColumnsAray = new string[DtblXcelData.Columns.Count];
                        foreach (DataColumn dtColumn in DtblXcelData.Columns)
                        {
                            strColumnsAray[j] = dtColumn.ColumnName;
                            j++;
                        }
                        for (int i = 0; i < strArray.Length - 1; i++)
                        {
                            if (strArray[i].Trim() != strColumnsAray[i].Trim())
                            {
                                chrFlag = 'N';
                                break;
                            }
                        }
                        if (chrFlag == 'Y')
                        {
                            orderService = new OrdersService();
                            long orderid = GetCounterValue("Orders");
                            Orders ord = new Orders();
                            IList<OrderItems> Orderitemslist = new List<OrderItems>();
                            foreach (DataRow Ordline in DtblXcelData.Rows)
                            {
                                if (Ordline.ItemArray[0].ToString().Trim() == "Name")
                                {
                                    ord.Name = Ordline.ItemArray[1].ToString();
                                    ord.ContingentType = ord.Name.Contains("FPU") ? "FPU" : "MIL";
                                }
                                if (Ordline.ItemArray[3].ToString().Trim() == "Start Date")
                                {
                                    string ordstdt = Ordline.ItemArray[4].ToString();
                                    if (!string.IsNullOrEmpty(ordstdt))
                                        ord.StartDate = DateTime.Parse(ordstdt, provider, System.Globalization.DateTimeStyles.NoCurrentDateDefault);
                                }
                                if (Ordline.ItemArray[0].ToString().Trim() == "Location")
                                {
                                    ord.Location = Ordline.ItemArray[1].ToString();
                                }
                                if (Ordline.ItemArray[0].ToString().Trim() == "Location")
                                {
                                    ord.LocationCMR = Convert.ToDecimal(Ordline.ItemArray[2].ToString());
                                }
                                if (Ordline.ItemArray[3].ToString().Trim() == "End Date")
                                {
                                    string ordenddt = Ordline.ItemArray[4].ToString();
                                    if (!string.IsNullOrEmpty(ordenddt))
                                        ord.EndDate = DateTime.Parse(ordenddt, provider, System.Globalization.DateTimeStyles.NoCurrentDateDefault);
                                }
                                if (Ordline.ItemArray[0].ToString().Trim() == "Control#")
                                {
                                    ord.ControlId = Ordline.ItemArray[1].ToString();
                                    //Get previous controlids for validation
                                    Dictionary<string, object> criteria = new Dictionary<string, object>();
                                    Dictionary<long, IList<Orders>> orders = orderService.GetOrdersListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                                    if (orders != null && orders.Count > 0)
                                    {
                                        IList<Orders> orderlist = orders.FirstOrDefault().Value.ToList();
                                        var ControlIdArray = (from u in orderlist select new { u.ControlId }).Distinct().ToArray();
                                        for (int i = 0; i < ControlIdArray.Length; i++)
                                        {
                                            if (ControlIdArray[i].ControlId == ord.ControlId)
                                            {
                                                AlreadyExistFile = AlreadyExistFile + 1;
                                                //alreadyExists.Append(fileName);
                                                if (uploadlog.UploadStatus == "YetToUpload")
                                                    uploadlog.UploadStatus = "AlreadyExist";
                                                orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);
                                            }
                                        }
                                    }
                                    string[] controlIdarray = Ordline.ItemArray[1].ToString().Split('-');
                                    ord.Sector = controlIdarray[1];
                                    ord.Week = Convert.ToInt64(controlIdarray[5].ToString().Replace("WK", ""));
                                    ord.Period = controlIdarray[4];
                                    ord.PeriodYear = controlIdarray[6] + "-" + controlIdarray[7];
                                    ord.CalYear = Convert.ToInt64(controlIdarray[6]);
                                }
                                if (Ordline.ItemArray[0].ToString().Trim() == "Control#")
                                {
                                    ord.ControlCMR = Convert.ToDecimal(Ordline.ItemArray[2].ToString());
                                }
                                if (Ordline.ItemArray[3].ToString().Trim() == "Troops #")
                                {
                                    ord.Troops = Convert.ToDecimal(Ordline.ItemArray[4].ToString());
                                }
                                if (Ordline.ItemArray[2].ToString().Trim() == "Total Amount")
                                {
                                    ord.TotalAmount = Convert.ToDecimal(Ordline.ItemArray[4].ToString());
                                }
                                if (Ordline.ItemArray[2].ToString().Trim() == "# Line Items Ordered")
                                {
                                    ord.LineItemsOrdered = Convert.ToDecimal(Ordline.ItemArray[4].ToString());
                                }
                                if (Ordline.ItemArray[2].ToString().Trim() == "Kg Ordered w/o eggs")
                                {
                                    ord.KgOrderedWOEggs = Convert.ToDecimal(Ordline.ItemArray[4].ToString());
                                }
                                if (Ordline.ItemArray[2].ToString().Trim() == "Eggs weight")
                                {
                                    ord.EggsWeight = Convert.ToDecimal(Ordline.ItemArray[4].ToString());
                                }
                                if (Ordline.ItemArray[2].ToString().Trim() == "Total weight")
                                {
                                    ord.TotalWeight = Convert.ToDecimal(Ordline.ItemArray[4].ToString());
                                }
                                if (Ordline["UNCode"].ToString() != "")
                                {
                                    OrderItems orditem = new OrderItems();
                                    //orditem.OrderId = orderid;----------------------------------------->kingston
                                    // orditem.LineId = GetCounterValue("OrderItems");
                                    orditem.UNCode = Convert.ToInt64(Ordline["UNCode"].ToString());
                                    orditem.Commodity = Ordline["Commodity"].ToString();
                                    orditem.OrderQty = Convert.ToDecimal(Ordline["Order Qty (kg/lt/ea)"].ToString());
                                    orditem.SectorPrice = Convert.ToDecimal(Ordline["Sector Price"].ToString());
                                    orditem.Total = Convert.ToDecimal(Ordline["Total"].ToString().Replace("$", ""));
                                    orditem.CreatedBy = "";
                                    orditem.CreatedDate = DateTime.Now;
                                    orditem.RemainingOrdQty = Convert.ToDecimal(Ordline["Order Qty (kg/lt/ea)"].ToString());
                                    //  orditem.Status = "Not Delivered";
                                    //string total = Ordline["Total"].ToString().Replace("$", "");
                                    if (AlreadyExistFile == 0)
                                    {
                                        Orderitemslist.Add(orditem);
                                        //orderService.SaveOrUpdateOrderItems(orditem);
                                    }
                                }
                            }
                            if (AlreadyExistFile > 0)
                            {
                                return null;
                            }
                            ord.CreatedBy = "";
                            ord.CreatedDate = DateTime.Now;
                            ord.OrderId = orderid;
                            ord.InvoiceStatus = "YetToGenerate";
                            if (AlreadyExistFile == 0)
                            {

                                try
                                {
                                    orderService.SaveOrUpdateOrdersUsingSession(ord, Orderitemslist);
                                    if (uploadlog.UploadStatus == "YetToUpload")
                                        uploadlog.UploadStatus = "UploadSuccessfully";
                                    uploadlog.ReferenceNo = "ORD-" + orderid;
                                    orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);
                                }
                                catch (Exception ex)
                                {
                                    if (uploadlog.UploadStatus == "YetToUpload")
                                        uploadlog.UploadStatus = "UploadFailed";
                                    uploadlog.ErrDesc = "Orders upload transaction failiure----" + ex.ToString();
                                    orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);
                                }

                                //Storing the actal physical file in the database
                                byte[] actualfile = new byte[files.ContentLength];
                                InsightFileUploads fileupload = new InsightFileUploads();
                                fileupload.UploadFileName = fileName;
                                fileupload.UploadFile = actualfile;
                                fileupload.UploadFileCategory = "ORDERS";
                                fileupload.ReferenceId = "ORD" + ord.OrderId;
                                fileupload.CreatedBy = "";
                                fileupload.CreatedDate = DateTime.Now;
                                orderService.SaveOrUpdateInsightUploads(fileupload);
                                System.IO.DirectoryInfo downloadedMessageInfo = new DirectoryInfo(@"E:\kingston Working task\New folder (2)\INSIGHT\INSIGHT\Content\FileuploadFolder\");

                                foreach (FileInfo file in downloadedMessageInfo.GetFiles())
                                {
                                    //  file.Delete();
                                }
                                ss = ss + 1;
                                decimal OrderqtyEggsonly = 0;
                                decimal OrderqtyWithoutEggs = 0;
                                decimal TotalAmt = 0;

                                Dictionary<string, object> criteria = new Dictionary<string, object>();
                                criteria.Add("OrderId", orderid);
                                Dictionary<long, IList<OrderItems>> orditems = orderService.GetOrderItemsListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                                for (int i = 0; i < orditems.FirstOrDefault().Value.Count(); i++)
                                {
                                    TotalAmt = TotalAmt + (orditems.FirstOrDefault().Value[i].OrderQty * orditems.FirstOrDefault().Value[i].SectorPrice);
                                    if (orditems.FirstOrDefault().Value[i].UNCode == 1129)
                                    {
                                        OrderqtyEggsonly = OrderqtyEggsonly + (orditems.FirstOrDefault().Value[i].OrderQty * (decimal)0.058824);
                                    }
                                    if (orditems.FirstOrDefault().Value[i].UNCode != 1129)
                                    {
                                        OrderqtyWithoutEggs = OrderqtyWithoutEggs + (orditems.FirstOrDefault().Value[i].OrderQty);
                                    }
                                }
                                ord.TotalAmount = TotalAmt;
                                ord.EggsWeight = OrderqtyEggsonly;
                                ord.KgOrderedWOEggs = OrderqtyWithoutEggs;
                                ord.TotalWeight = OrderqtyEggsonly + OrderqtyWithoutEggs;
                                //orderService.SaveOrUpdateOrder(ord);
                                System.Threading.Tasks.Parallel.Invoke(() =>
                                {
                                    orderService.SaveOrUpdateOrder(ord);//GetLongestWord(words);
                                });
                            }
                        }
                        else
                        {
                            if (uploadlog.UploadStatus == "YetToUpload")
                                uploadlog.UploadStatus = "UploadFailed";
                            uploadlog.ErrDesc = "Headers error ---The no of columns will be leaser or spelling change in headers";
                            orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);
                            //ErrorFilename.Append(fileName + ",");
                        }
                    }
                    else
                    {
                        if (uploadlog.UploadStatus == "YetToUpload")
                            uploadlog.UploadStatus = "UploadFailed";
                        uploadlog.ErrDesc = "Headers error  or transaction errors";
                        orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);
                        //ErrorFilename.Append(fileName + ",");
                    }
                }
                //success = success + 1;
            }
            catch (Exception ex)
            {
                if (uploadlog.UploadStatus == "YetToUpload")
                    uploadlog.UploadStatus = "UploadFailed";
                uploadlog.ErrDesc = "Headers,transaction errors " + ex.ToString();
                orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);
            }
            //  return null;
            return Content(Url.Content(@"~\Content\" + files.FileName));
        }



        //Order upload request jqgrid
        public JsonResult OrdersBulkUploadRequestJQGrid(string Category, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                OrdersService us = new OrdersService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();

                if (!string.IsNullOrWhiteSpace(sord) && sord == "desc")
                    sord = "Desc";
                else
                    sord = "Asc";
                criteria.Add("Category", Category);
                //   criteria.Add("Category", "ORDERSUPLOAD");
                Dictionary<long, IList<UploadRequest>> uploadreqlist = us.GetUploadRequestCountListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);

                if (uploadreqlist != null && uploadreqlist.Count > 0)
                {
                    long totalrecords = uploadreqlist.First().Key;
                    int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                    var jsondat = new
                    {
                        total = totalPages,
                        page = page,
                        records = totalrecords,

                        rows = (from items in uploadreqlist.FirstOrDefault().Value
                                select new
                                {
                                    i = 2,
                                    cell = new string[] {
                                        
                                        items.RequestId.ToString(),
                                        items.RequestName,
                                        items.RequestNo,
                                        items.Category,
                                        items.Status,
                                        items.CreatedBy,
                                        items.CreatedDate!=null? ConvertDateTimeToDate(items.CreatedDate.ToString("dd/MM/yyyy"),"en-GB"):"",
                                      //  items.ModifiedDate!=null? ConvertDateTimeToDate(items.ModifiedDate.ToString("dd/MM/yyyy"),"en-GB"):"",
                                        items.ModifiedBy,
                                        items.UploadStatus
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
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }
        //BulkOrderUploadDetails
        public ActionResult BulkOrderUploadDetails(long RequestId)
        {
            ViewBag.RequestId = RequestId;
            UploadRequest upreq = orderService.GetUploadRequestbyRequestId(RequestId);
            return View(upreq);
        }

        //Order upload request detials

        public JsonResult BulkOrderUploadDetailsJqgrid(long RequestId, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                OrdersService us = new OrdersService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();

                if (!string.IsNullOrWhiteSpace(sord) && sord == "desc")
                    sord = "Desc";
                else
                    sord = "Asc";
                criteria.Add("RequestId", RequestId);
                Dictionary<long, IList<UploadRequestDetailsLog>> uploadreqdetails = us.GetUploadRequestDetailsLogCountListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);

                if (uploadreqdetails != null && uploadreqdetails.Count > 0)
                {
                    long totalrecords = uploadreqdetails.First().Key;
                    int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                    var jsondat = new
                    {
                        total = totalPages,
                        page = page,
                        records = totalrecords,

                        rows = (from items in uploadreqdetails.FirstOrDefault().Value
                                select new
                                {
                                    i = 2,
                                    cell = new string[] {
                                        
                                        items.UploadReqDetLogId.ToString(),
                                        items.RequestId!=null?"REQUEST-"+items.RequestId:"",
                                        items.FileName,
                                        items.UploadStatus,
                                        items.ErrDesc,
                                        items.CreatedBy,
                                        items.CreatedDate!=null? ConvertDateTimeToDate(items.CreatedDate.ToString("dd/MM/yyyy"),"en-GB"):"",
                                      //  items.ModifiedDate!=null? ConvertDateTimeToDate(items.ModifiedDate.ToString("dd/MM/yyyy"),"en-GB"):"",
                                        items.ModifiedBy,
                                        items.ReferenceNo
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
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }

        #region Bulk deliverynote upload related code

        public ActionResult DeliveryNoteBulkuploadRequestCreation(long? RequestId)
        {
            UploadRequest upreq = new UploadRequest();
            if (RequestId != null)
            {
                upreq = orderService.GetUploadRequestbyRequestId(RequestId ?? 0);

            }
            return View(upreq);
        }

        [HttpPost]
        public ActionResult DeliveryNoteBulkuploadRequestCreation(UploadRequest upreq)
        {
            try
            {
                INSIGHT.Entities.User Userobj = (INSIGHT.Entities.User)Session["objUser"];
                if (Userobj == null || (Userobj != null && Userobj.UserId == null))
                { return RedirectToAction("LogOn", "Account"); }
                string loggedInUserId = Userobj.UserId;
                upreq.Category = "DELIVERYNOTEUPLOAD";
                upreq.CreatedBy = loggedInUserId;
                upreq.CreatedDate = DateTime.Now;
                long reqid = orderService.SaveOrUpdateUploadRequest(upreq);
                upreq.RequestNo = "REQUEST-" + reqid;
                upreq.Status = "OPEN";
                upreq.UploadStatus = "Request Generated";
                orderService.SaveOrUpdateUploadRequest(upreq);
                //UploadRequest upreq1 = orderService.GetUploadRequestbyRequestId(reqid);
                //orderService.SaveOrUpdateUploadRequest(upreq);
                return (RedirectToAction("DeliveryNoteBulkuploadRequestCreation", new { RequestId = upreq.RequestId }));
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }

        public ActionResult UploadBulkDeliveryNote()
        {

            //construct the result string
            //first successful uploaded files, then already exists and error
            long ImportedDeliveryNoteId = GetCounterValue("ImportedDeliveryNote");
            int ss = 0;
            int AlreadyExistFile = 0;
            var files = Request.Files["Filedata"];
            string savePath = Server.MapPath(@"~\Content\FileuploadFolder\" + files.FileName);
            files.SaveAs(savePath);
            string fileName = string.Empty;
            int length = files.ContentLength;
            fileName = files.FileName;

            //Delivery note's in Byte array
            byte[] UploadedFile = System.IO.File.ReadAllBytes(fileName);

            //Saving the bulk order upload request
            //long UploadReqDetLogId = CreateOrUpdateRequest(fileName);-----> commented by kingston by testing purpose
            long UploadReqDetLogId = 0;
            UploadRequestDetailsLog uploadlog = orderService.GetUploadRequestDetailsLogbyRequestId(UploadReqDetLogId);




            if (files != null && files.ContentLength > 0)
            {
                //INSIGHT.Entities.User Userobj = (INSIGHT.Entities.User)Session["objUser"];
                //if (Userobj == null || (Userobj != null && Userobj.UserId == null))
                //{ return RedirectToAction("LogOn", "Account"); }
                //string loggedInUserId = Userobj.UserId;

                try
                {
                    string UploadConnStr = "";
                    string fileExtn = Path.GetExtension(files.FileName);
                    string fileLocation = savePath;

                    if (fileExtn == ".xls")
                    {
                        UploadConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\"";
                    }
                    if (fileExtn == ".xlsx")
                    {
                        UploadConnStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";
                    }
                    OleDbConnection conn = new OleDbConnection();
                    DataTable DtblXcelData = new DataTable();
                    string QeryToGetXcelData = "select * from " + string.Format("{0}${1}", "[Delivery_Note", "A9:L15]");
                    conn.ConnectionString = UploadConnStr;
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(QeryToGetXcelData, conn);
                    cmd.CommandType = CommandType.Text;
                    OleDbDataAdapter DtAdptrr = new OleDbDataAdapter();
                    DtAdptrr.SelectCommand = cmd;
                    DtAdptrr.Fill(DtblXcelData);
                    conn.Close();
                    if (DtblXcelData.Rows.Count == 0)
                    {
                        return Json(new { success = false, result = "No Rows available in the file to update!!!" }, "text/html", JsonRequestBehavior.AllowGet);
                    }
                    else if (DtblXcelData.Rows.Count > 0)
                    {
                        ImportedDeliveryNote impdel = new ImportedDeliveryNote();
                        foreach (DataRow delline in DtblXcelData.Rows)
                        {
                            if (delline.ItemArray[0].ToString().Trim() == "Delivery Note Number:")
                            {
                                string deliverynotename = delline.ItemArray[2].ToString();
                                impdel.ImpDeliveryNoteName = deliverynotename;
                                Dictionary<string, object> criteria = new Dictionary<string, object>();
                                Dictionary<long, IList<DeliveryNote>> delnote = orderService.GetDeliveryNoteListWithPagingAndCriteria(0, 999999, string.Empty, string.Empty, criteria);
                                if (delnote != null && delnote.Count > 0)
                                {
                                    IList<DeliveryNote> deliverynotelist = delnote.FirstOrDefault().Value.ToList();
                                    var DeliveryNoteName = (from u in deliverynotelist select u.DeliveryNoteName).Distinct().ToArray();

                                    Dictionary<long, IList<ImportedDeliveryNote>> impdelnote = orderService.GetImportedDeliveryNoteListWithPagingAndCriteria(0, 999999, string.Empty, string.Empty, criteria);
                                    if (delnote != null && delnote.Count > 0)
                                    {

                                        IList<ImportedDeliveryNote> importeddeliverynotelist = impdelnote.FirstOrDefault().Value.ToList();
                                        var impdelnotename = (from u in importeddeliverynotelist select u.ImpDeliveryNoteName).Distinct().ToArray();
                                        string[] AllDeliveryNotelist = DeliveryNoteName.ToArray();
                                        // string[] AllDeliveryNotelist = DeliveryNoteName.Union(impdelnotename).ToArray();
                                        for (int i = 0; i < AllDeliveryNotelist.Count(); i++)
                                        {
                                            if (AllDeliveryNotelist[i] == deliverynotename)
                                            {
                                                AlreadyExistFile = AlreadyExistFile + 1;
                                                // alreadyExists.Append(fileName);-------------------> need to be clarifided
                                            }
                                        }
                                    }
                                }

                            }
                            if (delline.ItemArray[8].ToString().Trim() == "Shipment Date:")
                            {

                                string shipmentdate = delline.ItemArray[9].ToString();
                                if (!string.IsNullOrEmpty(shipmentdate))
                                    impdel.ImpShipmentDate = DateTime.Parse(shipmentdate, provider, System.Globalization.DateTimeStyles.NoCurrentDateDefault);

                            }
                            if (delline.ItemArray[0].ToString().Trim() == "Request#:")
                            {
                                impdel.ImpRequestNo = delline.ItemArray[2].ToString().Trim() == "" ? 0 : Convert.ToInt64(delline.ItemArray[2].ToString());
                                // impdel.ImpRequestNo = Convert.ToInt64(delline.ItemArray[2].ToString());

                            }

                            if (delline.ItemArray[3].ToString().Trim() == "Consumption Week:")
                            {
                                // impdel.ImpConsumptionWeek = Convert.ToDecimal(delline.ItemArray[5].ToString());
                                impdel.ImpConsumptionWeek = delline.ItemArray[5].ToString().Trim() == "" ? 0 : Convert.ToDecimal(delline.ItemArray[5].ToString());

                            }

                            if (delline.ItemArray[8].ToString().Trim() == "Unit Control No:")
                            {
                                impdel.ImpControlId = delline.ItemArray[9].ToString();
                                string[] controlIdarray = delline.ItemArray[9].ToString().Split('-');
                                impdel.Sector = controlIdarray[1].ToString();
                                impdel.Name = controlIdarray[3].ToString();
                                impdel.Location = controlIdarray[1].ToString() + "-" + controlIdarray[2].ToString();
                                impdel.Period = controlIdarray[4].ToString();

                            }
                            if (delline.ItemArray[0].ToString().Trim() == "Warehouse:")
                            {
                                impdel.ImpWarehouse = delline.ItemArray[2].ToString();

                            }
                            if (delline.ItemArray[3].ToString().Trim() == "Delivery Week:")
                            {
                                impdel.ImpDeliveryWeek = delline.ItemArray[5].ToString().Trim() == "" ? 0 : Convert.ToDecimal(delline.ItemArray[5].ToString());

                            }
                            if (delline.ItemArray[0].ToString().Trim() == "Strength:")
                            {
                                impdel.ImpStrength = delline.ItemArray[2].ToString().Trim() == "" ? 0 : Convert.ToDecimal(delline.ItemArray[2].ToString());

                            }
                            if (delline.ItemArray[3].ToString().Trim() == "Delivery By:")
                            {
                                impdel.ImpDeliveryMode = delline.ItemArray[5].ToString();
                                //impdel.ImpDeliveryMode =delline.ItemArray[5].ToString().Trim()==""?0: delline.ItemArray[5].ToString();

                            }
                            if (delline.ItemArray[8].ToString().Trim() == "UN Food Order:")
                            {
                                impdel.ImpUNFoodOrder = delline.ItemArray[9].ToString();

                            }

                            if (delline.ItemArray[0].ToString().Trim() == "DOS:")
                            {
                                // impdel.ImpDOS = Convert.ToDecimal(delline.ItemArray[2].ToString());
                                impdel.ImpDOS = delline.ItemArray[2].ToString().ToString().Trim() == "" ? 0 : Convert.ToDecimal(delline.ItemArray[2].ToString());

                            }
                            if (delline.ItemArray[3].ToString().Trim() == "Seal No:")
                            {
                                impdel.ImpSealNo = delline.ItemArray[5].ToString();

                            }
                            if (delline.ItemArray[8].ToString().Trim() == "UN Week:")
                            {
                                //impdel.ImpUNWeek = Convert.ToDecimal(delline.ItemArray[9].ToString());
                                impdel.ImpUNWeek = delline.ItemArray[9].ToString().Trim() == "" ? 0 : Convert.ToDecimal(delline.ItemArray[9].ToString());

                            }
                            if (delline.ItemArray[0].ToString().Trim() == "Man days:")
                            {
                                //impdel.ImpManDays = Convert.ToDecimal(delline.ItemArray[2].ToString());
                                impdel.ImpManDays = delline.ItemArray[2].ToString().Trim() == "" ? 0 : Convert.ToDecimal(delline.ItemArray[2].ToString());

                            }
                            if (delline.ItemArray[8].ToString().Trim() == "Period:")
                            {
                                impdel.ImpPeriod = delline.ItemArray[9].ToString();

                            }



                        }
                        Orders ord = orderService.GetOrderByControlId(impdel.ImpControlId);
                        impdel.ImpDeliveryNoteId =
                        impdel.OrderId = ord.OrderId;
                        impdel.ImpDeliveryNoteId = ImportedDeliveryNoteId;
                        //impdel.CreatedBy = loggedInUserId;
                        impdel.CreatedDate = DateTime.Now;
                        //impdel.ImpDeliveryNoteId = ImportedDeliveryNoteId;
                        if (AlreadyExistFile == 0)
                        {

                            //  impdel.status = "ADDEDINIMPDELIVERYNOTE";
                            //  long ImportedDeliveryNoteId = ordser.SaveOrUpdateImportedDeliveryNote(impdel);

                            // UploadedFilename.Append(fileName + ",");
                        }
                        OleDbConnection itemconn = new OleDbConnection();
                        DataTable DtblXcelItemData = new DataTable();
                        string QeryToGetXcelItemData = "select [Line#],[Item],[Descreption],[Ordered Qty],[Delivered Qty],[Number of Packs],[Number of Pieces],[Substitute For],[Substitute Name],[UOM],[Issue Type],[Remarks],[Line Status if Substitution],[Actual Received Qty#],[Recd Date @ Contingent] from " + string.Format("{0}${1}", "[Delivery_Note", "A16:AZ]");
                        // string QeryToGetXcelItemData = "select * from " + string.Format("{0}${1}", "[Delivery_Note", "A16:AZ]");
                        itemconn.ConnectionString = UploadConnStr;
                        itemconn.Open();
                        cmd = new OleDbCommand(QeryToGetXcelItemData, conn);
                        cmd.CommandType = CommandType.Text;
                        DtAdptrr = new OleDbDataAdapter();
                        DtAdptrr.SelectCommand = cmd;
                        DtAdptrr.Fill(DtblXcelItemData);
                        string[] strArray = { "Line#", "Item", "Descreption", "Ordered Qty", "Delivered Qty", "Number of Packs", "Number of Pieces", "Substitute For", "Substitute Name", "UOM", "Issue Type", "Remarks", "Line Status if Substitution", "Actual Received Qty#", "Recd Date @ Contingent" };
                        char chrFlag = 'Y';
                        if (DtblXcelItemData.Columns.Count == strArray.Length)
                        {
                            int j = 0;
                            string[] strColumnsAray = new string[DtblXcelItemData.Columns.Count];
                            foreach (DataColumn dtColumn in DtblXcelItemData.Columns)
                            {
                                strColumnsAray[j] = dtColumn.ColumnName;
                                j++;
                            }
                            for (int i = 0; i < strArray.Length - 1; i++)
                            {
                                if (strArray[i].Trim() != strColumnsAray[i].Trim())
                                {
                                    chrFlag = 'N';
                                    break;
                                }
                            }
                            if (chrFlag == 'Y')
                            {
                                // ordser = new OrdersService();
                                IList<ImportedDeliveryNoteItems> importeddeliverynoteitems = new List<ImportedDeliveryNoteItems>();

                                foreach (DataRow OrdItemline in DtblXcelItemData.Rows)
                                {

                                    if (OrdItemline["Item"].ToString().Trim() != "")
                                    {
                                        ImportedDeliveryNoteItems impdelitems = new ImportedDeliveryNoteItems();
                                        impdelitems.OrderId = ord.OrderId;

                                        // long ImportedDeliveryNoteId = GetCounterValue("ImportedDeliveryNote");
                                        impdelitems.ImpDeliveryNoteId = impdel.ImpDeliveryNoteId;
                                        impdelitems.ImpControlId = impdel.ImpControlId;
                                        impdelitems.ImpDeliveryMode = impdel.ImpDeliveryMode;

                                        impdelitems.ImpDeliveryNoteName = impdel.ImpDeliveryNoteName;
                                        impdelitems.OrderId = ord.OrderId;
                                        impdelitems.ImpUNCode = Convert.ToInt64(OrdItemline["Item"].ToString().Trim());
                                        impdelitems.ImpCommodity = OrdItemline["Descreption"].ToString();
                                        impdelitems.ImpOrderQty = Convert.ToDecimal(OrdItemline["Ordered Qty"].ToString().Trim());
                                        impdelitems.ImpDeliveryQty = OrdItemline["Actual Received Qty#"].ToString().Trim() == "" ? 0 : Convert.ToDecimal(OrdItemline["Actual Received Qty#"].ToString().Trim());
                                        impdelitems.ImpNoOfPacks = OrdItemline["Number of Packs"].ToString().Trim() == "" ? 0 : Convert.ToDecimal(OrdItemline["Number of Packs"].ToString().Trim());
                                        //impdelitems.ImpNoOfPacks = Convert.ToDecimal(OrdItemline["Number of Packs"].ToString().Trim());
                                        impdelitems.ImpNoOfPieces = OrdItemline["Number of Pieces"].ToString().Trim() == "" ? 0 : Convert.ToDecimal(OrdItemline["Number of Pieces"].ToString().Trim());
                                        //impdelitems.ImpNoOfPieces = Convert.ToDecimal(OrdItemline["Number of Pieces"].ToString().Trim());
                                        impdelitems.ImpSubsItemCode = OrdItemline["Substitute For"].ToString().Trim() == "" ? 0 : Convert.ToInt64(OrdItemline["Substitute For"].ToString().Trim());

                                        impdelitems.ImpSubsItemName = OrdItemline["Substitute Name"].ToString().Trim();
                                        impdelitems.ImpUOM = OrdItemline["UOM"].ToString().Trim();
                                        impdelitems.ImpIssueType = OrdItemline["Issue Type"].ToString().Trim();
                                        impdelitems.ImpRemarks = OrdItemline["Remarks"].ToString().Trim();
                                        impdelitems.ImpSubsStatus = OrdItemline["Line Status if Substitution"].ToString().Trim();
                                        string ImpDeliveryDate = OrdItemline["Recd Date @ Contingent"].ToString().Trim();
                                        impdelitems.ImpExpDeliveryQty = OrdItemline["Delivered Qty"].ToString().Trim() == "" ? 0 : Convert.ToDecimal(OrdItemline["Delivered Qty"].ToString().Trim());
                                        if (!string.IsNullOrEmpty(ImpDeliveryDate))
                                            // impdelitems.ImpDeliveryDate = DateTime.Parse(ImpDeliveryDate, provider, System.Globalization.DateTimeStyles.NoCurrentDateDefault);
                                            impdelitems.ImpDeliveryDate = Convert.ToDateTime(OrdItemline["Recd Date @ Contingent"].ToString().Trim());
                                        //impdelitems.CreatedBy = loggedInUserId;
                                        impdelitems.CreatedDate = DateTime.Now;
                                        if (AlreadyExistFile == 0)
                                        {
                                            importeddeliverynoteitems.Add(impdelitems);
                                            //ordser.SaveOrUpdateImportedDeliveryNoteItems(impdelitems);
                                            //UpdateImportedDeliveryNoteItemDetailsinOrdItemsTbl(impdelitems);

                                        }
                                    }

                                }
                                if (importeddeliverynoteitems.Count != 0)
                                {
                                    orderService.SaveorUpdateDeliveryNoteInSingleSession(impdel, importeddeliverynoteitems);
                                    //orderService.SaveOrUpdateImportedDeliveryNoteItemsList(importeddeliverynoteitems);

                                    Dictionary<string, object> criteria = new Dictionary<string, object>();
                                    criteria.Clear();
                                    criteria.Add("ImpDeliveryNoteId", importeddeliverynoteitems[0].ImpDeliveryNoteId);


                                    Dictionary<long, IList<ImportedDeliveryNoteItems>> impdelnoteitemslist = orderService.GetImportedDeliveryNoteItemsListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                                    IList<ImportedDeliveryNoteItems> delnoteitems = impdelnoteitemslist.FirstOrDefault().Value.ToList();
                                    UpdateImportedDeliveryNoteItemDetailsinOrdItemsTbl(importeddeliverynoteitems, UploadedFile);
                                    //UploadedFilename.Append(fileName + ",");----->successful files
                                }

                            }
                            else
                            {
                                //ErrorFilename.Append(fileName + ",");----->Error Files
                            }
                        }
                        else
                        {
                            //ErrorFilename.Append(fileName + ",");-------->Error Files
                        }
                    }
                    // success = success + 1;

                }
                catch (Exception ex)
                {
                    //ErrorFilename.Append(fileName + ",");-------->Error files

                    //ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                }

            }
            else
            {
                // ErrorFilename.Append(fileName + ",");------>ErrorEventArgs files
                //  return Json(new { success = false, result = "You have uploded the empty file. Please upload the correct file." }, "text/html", JsonRequestBehavior.AllowGet);
            }

            //return Json(new { success = true, result = retValue.ToString().Replace(Environment.NewLine, "<br />") }, "text/html", JsonRequestBehavior.AllowGet);
            return Content(Url.Content(@"~\Content\" + files.FileName));

        }

        #endregion
        #endregion

        #region   Uploading expected deliveryDate
        public ActionResult UploadExpectedDeliveryDate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadExpectedDeliveryDate(HttpPostedFileBase uploadedFile, string RequestName, string Period, string PeriodYear)
        {
            UploadRequest upreq = new UploadRequest();
            INSIGHT.Entities.User Userobj = (INSIGHT.Entities.User)Session["objUser"];
            if (Userobj == null || (Userobj != null && Userobj.UserId == null))
            { return RedirectToAction("LogOn", "Account"); }
            string loggedInUserId = Userobj.UserId;
            upreq.Category = "ExpDeliveryDateUpload";
            upreq.CreatedBy = loggedInUserId;
            upreq.CreatedDate = DateTime.Now;
            upreq.RequestName = RequestName;
            upreq.Period = Period;
            upreq.PeriodYear = PeriodYear;
            long reqid = orderService.SaveOrUpdateUploadRequest(upreq);
            upreq.RequestNo = "REQUEST-" + reqid;
            // upreq.Status = "OPEN";
            //  upreq.UploadStatus = "Request Generated";
            upreq.UploadStatus = "InProgress";
            orderService.SaveOrUpdateUploadRequest(upreq);

            try
            {
                //construct the result string
                //first successful uploaded files, then already exists and error
                StringBuilder retValue = new StringBuilder();

                int success = 0;

                HttpPostedFileBase theFile = HttpContext.Request.Files["uploadedFile"];
                StringBuilder alreadyExists = new StringBuilder();
                StringBuilder ErrorFilename = new StringBuilder();
                StringBuilder UploadedFilename = new StringBuilder();
                if (theFile != null && theFile.ContentLength > 0)
                {
                    //INSIGHT.Entities.User Userobj = (INSIGHT.Entities.User)Session["objUser"];
                    //if (Userobj == null || (Userobj != null && Userobj.UserId == null))
                    //{ return RedirectToAction("LogOn", "Account"); }
                    //string loggedInUserId = Userobj.UserId;
                    string fileName = string.Empty;

                    try
                    {

                        string path = uploadedFile.InputStream.ToString();
                        byte[] imageSize = new byte[uploadedFile.ContentLength];
                        uploadedFile.InputStream.Read(imageSize, 0, (int)uploadedFile.ContentLength);
                        string UploadConnStr = "";
                        fileName = uploadedFile.FileName;
                        string fileExtn = Path.GetExtension(uploadedFile.FileName);
                        string fileLocation = ConfigurationManager.AppSettings["ImportedExpDelDtFilePath"].ToString() + DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss").Replace(":", ".") + fileName;
                        uploadedFile.SaveAs(fileLocation);
                        if (fileExtn == ".xls")
                        {
                            UploadConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\"";
                        }
                        if (fileExtn == ".xlsx")
                        {
                            UploadConnStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";
                        }

                        DataTable DtblXcelData = new DataTable();
                        OleDbConnection conn = new OleDbConnection();
                        OleDbDataAdapter DtAdptrr = new OleDbDataAdapter();
                        string QeryToGetXcelData = "select * from " + string.Format("{0}${1}", "[ExpectedDeliveryDate", "A1:AZ]");
                        OleDbCommand cmd = new OleDbCommand(QeryToGetXcelData, conn);
                        conn.ConnectionString = UploadConnStr;
                        conn.Open();
                        cmd = new OleDbCommand(QeryToGetXcelData, conn);
                        cmd.CommandType = CommandType.Text;
                        DtAdptrr = new OleDbDataAdapter();
                        DtAdptrr.SelectCommand = cmd;
                        DtAdptrr.Fill(DtblXcelData);
                        string[] strArray = { "S#No", "ControlId", "ExpectedDeliveryDate" };
                        char chrFlag = 'Y';
                        if (DtblXcelData.Columns.Count == strArray.Length)
                        {
                            int j = 0;
                            string[] strColumnsAray = new string[DtblXcelData.Columns.Count];
                            foreach (DataColumn dtColumn in DtblXcelData.Columns)
                            {
                                strColumnsAray[j] = dtColumn.ColumnName;
                                j++;
                            }
                            for (int i = 0; i < strArray.Length - 1; i++)
                            {
                                if (strArray[i].Trim() != strColumnsAray[i].Trim())
                                {
                                    chrFlag = 'N';
                                    break;
                                }
                            }
                            if (chrFlag == 'Y')
                            {
                                try
                                {
                                    IList<ImportedExpDelDate> expdeldtlist = new List<ImportedExpDelDate>();

                                    foreach (DataRow expdeldateline in DtblXcelData.Rows)
                                    {

                                        if (expdeldateline["ControlId"].ToString() != "" && expdeldateline["ExpectedDeliveryDate"].ToString() != "")
                                        {
                                            ImportedExpDelDate expdeldt = new ImportedExpDelDate();

                                            // orditem.LineId = GetCounterValue("OrderItems");

                                            string controlId = expdeldateline["ControlId"].ToString();
                                            string[] controlIdArray = controlId.Split('-');

                                            expdeldt.ControlId = "FFO-" + controlIdArray[1] + "-" + controlIdArray[2] + "-" + controlIdArray[4] + "-" + controlIdArray[3] + "-" + controlIdArray[5] + "-" + controlIdArray[6] + "-" + controlIdArray[7];
                                            expdeldt.Sector = controlIdArray[2].ToString();
                                            expdeldt.Name = controlIdArray[4].ToString();
                                            expdeldt.Period = controlIdArray[5].ToString();
                                            expdeldt.PeriodYear = controlIdArray[7].ToString();
                                            expdeldt.Location = controlIdArray[3].ToString();

                                            //expdeldt.ControlId = controlId;
                                            //expdeldt.Sector = controlIdArray[1].ToString();
                                            //expdeldt.Name = controlIdArray[3].ToString();
                                            //expdeldt.Period = controlIdArray[4].ToString();
                                            //expdeldt.PeriodYear = controlIdArray[6].ToString() + "-" + controlIdArray[7].ToString();
                                            //expdeldt.Location = controlIdArray[1].ToString() + "-" + controlIdArray[2].ToString();
                                            string ExpDeliveryDate = expdeldateline["ExpectedDeliveryDate"].ToString();
                                            if (!string.IsNullOrEmpty(ExpDeliveryDate))
                                                // impdelitems.ImpDeliveryDate = DateTime.Parse(ImpDeliveryDate, provider, System.Globalization.DateTimeStyles.NoCurrentDateDefault);
                                                expdeldt.ImpExpDeliveryDate = Convert.ToDateTime(expdeldateline["ExpectedDeliveryDate"].ToString().Trim());
                                            expdeldt.CreatedBy = loggedInUserId;
                                            expdeldt.CreatedDate = DateTime.Now;
                                            expdeldt.RequestId = upreq.RequestId;
                                            expdeldtlist.Add(expdeldt);
                                        }
                                    }

                                    if (expdeldtlist != null && expdeldtlist.Count > 0)
                                    {
                                        try
                                        {
                                            orderService.SaveOrUpdateImportedExpDelDateListInSingleSession(expdeldtlist);
                                            for (int i = 0; i < expdeldtlist.Count; i++)
                                            {
                                                Orders ord = new Orders();
                                                ord = orderService.GetOrderByControlId(expdeldtlist[i].ControlId);
                                                if (ord != null)
                                                {
                                                    ord.ExpectedDeliveryDate = expdeldtlist[i].ImpExpDeliveryDate;
                                                    orderService.SaveOrUpdateOrder(ord);
                                                }
                                            }
                                            upreq.UploadStatus = "Uploaded Successfully";
                                            orderService.SaveOrUpdateUploadRequest(upreq);
                                        }
                                        catch (Exception ex)
                                        {
                                            upreq.ErrorDesc = Convert.ToString(ex + "----------6");
                                            orderService.SaveOrUpdateUploadRequest(upreq);
                                            ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    upreq.ErrorDesc = Convert.ToString(ex + "------------------------5");
                                    orderService.SaveOrUpdateUploadRequest(upreq);
                                    ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                                }


                            }
                            else
                            {
                                try
                                {
                                    upreq.UploadStatus = "Upload Failed";
                                    orderService.SaveOrUpdateUploadRequest(upreq);
                                    return Json(new { success = false, result = "Headers mismatch!!!" }, "text/html", JsonRequestBehavior.AllowGet);
                                }
                                catch (Exception ex)
                                {
                                    upreq.ErrorDesc = Convert.ToString(ex + "--------------4");
                                    orderService.SaveOrUpdateUploadRequest(upreq);
                                    ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                upreq.UploadStatus = "Upload Failed";
                                orderService.SaveOrUpdateUploadRequest(upreq);
                                ErrorFilename.Append(fileName + ",");
                            }
                            catch (Exception ex)
                            {
                                upreq.ErrorDesc = Convert.ToString(ex + "-----------1");
                                orderService.SaveOrUpdateUploadRequest(upreq);
                                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                            }
                        }

                        success = success + 1;
                    }
                    catch (Exception ex)
                    {
                        upreq.UploadStatus = "Upload Failed";
                        upreq.ErrorDesc = Convert.ToString(ex + "-----------2");
                        orderService.SaveOrUpdateUploadRequest(upreq);
                        ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                    }

                }
                else
                {
                    return Json(new { success = false, result = "You have uploded the empty file. Please upload the correct file." }, "text/html", JsonRequestBehavior.AllowGet);
                }

                //MyLabel.Text = sb.ToString().Replace(Environment.NewLine, "<br />");
                return Json(new { success = true, result = "Expected delivery date updated successfully." }, "text/html", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                upreq.ErrorDesc = Convert.ToString(ex + "------------------3");
                orderService.SaveOrUpdateUploadRequest(upreq);
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;

            }
        }
        public JsonResult ImportedExpectedDeliveryNoteListJqgrid(long RequestId, string searchItems, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                if (!string.IsNullOrWhiteSpace(sord) && sord == "desc")
                    sord = "Desc";
                else
                    sord = "Asc";
                criteria.Add("RequestId", RequestId);

                if (searchItems != null)
                {
                    var Items = searchItems.ToString().Split(',');
                    if (!string.IsNullOrWhiteSpace(Items[0])) { criteria.Add("Sector", Items[0]); }
                    if (!string.IsNullOrWhiteSpace(Items[1])) { criteria.Add("Name", Items[1]); }
                    if (!string.IsNullOrWhiteSpace(Items[2])) { criteria.Add("Location", Items[2]); }
                    if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Period", Items[3]); }
                    if (!string.IsNullOrWhiteSpace(Items[4])) { criteria.Add("PeriodYear", Items[4]); }
                }

                Dictionary<long, IList<ImportedExpDelDate>> impexpdeliverydate = orderService.GetImprtedDeliveryDateListWithPagingAndCriteria(page - 1, 9999, sidx, sord, criteria);

                if (impexpdeliverydate != null && impexpdeliverydate.Count > 0)
                {
                    long totalrecords = impexpdeliverydate.First().Key;
                    int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                    var jsondat = new
                    {
                        total = totalPages,
                        page = page,
                        records = totalrecords,

                        rows = (from items in impexpdeliverydate.First().Value
                                select new
                                {
                                    i = 2,
                                    cell = new string[] {
                                        items.ImpExpDelDtId.ToString(),
                                        items.ControlId,
                                        items.Sector,
                                        items.Name,
                                        items.Location,
                                        items.Period,
                                        items.PeriodYear,
                                     //   items.ImpExpDeliveryDate.ToString()
                                         items.ImpExpDeliveryDate==null?"":items.ImpExpDeliveryDate.ToString("dd-MM-yyyy")
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
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }

        public ActionResult ExpDelDateRequestDetails(long RequestId)
        {
            UploadRequest upreq = orderService.GetUploadRequestbyRequestId(RequestId);
            return View(upreq);
        }


        //Deleting the Expecteddeliverydate by requestid 
        public void DeleteExpectedDeliverydate(string RequestIds)
        {
            try
            {
                if (RequestIds != null)
                {
                    var RequestIdArray = RequestIds.Split(',');
                    for (int i = 0; i < RequestIdArray.Length; i++)
                    {
                        Dictionary<string, object> criteria = new Dictionary<string, object>();
                        criteria.Add("RequestId", Convert.ToInt64(RequestIdArray[i]));
                        Dictionary<long, IList<ImportedExpDelDate>> impexpdeliverydate = orderService.GetImprtedDeliveryDateListWithPagingAndCriteria(0, 999999, string.Empty, string.Empty, criteria);
                        criteria.Clear();
                        for (int j = 0; j < impexpdeliverydate.FirstOrDefault().Value.Count; j++)
                        {
                            Orders ord = orderService.GetOrderByControlId(impexpdeliverydate.FirstOrDefault().Value[j].ControlId);
                            if (ord != null)
                            {
                                ord.ExpectedDeliveryDate = null;
                                orderService.SaveOrUpdateOrder(ord);
                            }
                        }

                    }

                    orderService.DeleteExpectedDeliveryDatebyRequestId(RequestIds);
                }


            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }

            //for (int i = 0; i < RequestIdArray.Length; i++)
            //{
            //    OrdersService.DeleteSubstitutionMasterByRequestId(Convert.ToInt64(RequestIdArray[i]));


            //}


        }
        #endregion

        #region upload the orders  and delivery note using parallel task

        public ActionResult UploadOrdersTest()
        {

            return View();
        }

        public ActionResult OrdersBulkUpload(long RequestId)
        {
            string RequestNo = "REQUEST-" + RequestId;
            var files = Request.Files["Filedata"];
            // string curpath = "D:\\INSIGHTBACKUP1\\LIVE\\Orders";
            string curpath = ConfigurationManager.AppSettings["OrdersUploadRequestFilePath"].ToString();
            string RequestFolder = String.Format("{0}\\" + RequestNo, curpath);
            // If the folder is not existed, create it.
            if (!Directory.Exists(RequestFolder)) { Directory.CreateDirectory(RequestFolder); }
            string savePath = RequestFolder + "\\" + files.FileName;

            files.SaveAs(savePath);
            //--------------------------Parallel task related code------------------


            //Parallel.Invoke(callParallel);
            //LongTimeTask_Delegate d = null;
            //d = new LongTimeTask_Delegate(CreateOrderParallel);
            //IAsyncResult R = null;
            //R = d.BeginInvoke(null, null);
            //new Task(() => { SendBulkEmailRequestWithAsync(ComposeId, (!string.IsNullOrEmpty(cei.Campus)) ? cei.Campus : Campus, IsAlterNativeMail); }).Start();
            //new Task(CreateOrderParallel).Start();

            //----------------------------Parallel task related code ends here --------------------------------------------------------
            return Content(Url.Content(@"~\Content\" + files.FileName));

        }

        public void callParallel(long RequestId)
        {
            // object lockObject = new object();
            //new Task(CreateOrderParallel).Start();
            new Task(() =>
            {

                CreateOrderParallel(RequestId);

            }).Start();
        }

        public void CreateOrderParallel(long RequestId)
        {
            try
            {

                long orderid = 0;
                string UploadConnStr = string.Empty;
                StringBuilder alreadyExists = new StringBuilder();
                StringBuilder ErrorFilename = new StringBuilder();
                StringBuilder UploadedFilename = new StringBuilder();
                UploadRequestDetailsLog uploadlog = new UploadRequestDetailsLog();
                int AlreadyExistFile = 0;

                string fileExtn = string.Empty;
                UploadRequest upreq = new UploadRequest();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                if (RequestId > 0)
                {
                    upreq = orderService.GetUploadRequestbyRequestId(RequestId);
                    if (upreq.UploadStatus == "Request Generated")
                    {
                        upreq.UploadStatus = "InProgress";
                        orderService.SaveOrUpdateUploadRequest(upreq);
                    }
                }
                // string curpath = "D:\\INSIGHTBACKUP1\\LIVE\\Orders\\" + "REQUEST-" + RequestId;
                string curpath = ConfigurationManager.AppSettings["OrdersUploadRequestFilePath"].ToString();
                //string[] filePaths = Directory.GetFiles(curpath + "\\REQUEST-" + RequestId, "*.xlsx");
                //string[] filePaths = Directory.GetFiles(curpath + "\\REQUEST-" + RequestId, "*.xls");

                string[] filePaths = Directory.GetFiles(curpath + "\\REQUEST-" + RequestId, "*.xlsx");
                if (filePaths.Count() == 0) filePaths = Directory.GetFiles(curpath + "\\REQUEST-" + RequestId, "*.xls");

                foreach (string s in filePaths)
                {
                    try
                    {
                        string AlreadyExistFlag = string.Empty;
                        string fileName = s.ToString();

                        //Delivery note's in Byte array
                        byte[] UploadedFile = System.IO.File.ReadAllBytes(fileName);

                        //Create or update the  saving the individual file status in uploadrequestdetailslog table
                        long UploadReqDetLogId = CreateOrUpdateRequest(fileName, RequestId);
                        uploadlog = orderService.GetUploadRequestDetailsLogbyRequestId(UploadReqDetLogId);

                        UploadConnStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + s + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";
                        OleDbConnection conn = new OleDbConnection();
                        DataTable DtblXcelData = new DataTable();
                        string QeryToGetXcelData = string.Empty;
                        string sht = string.Empty;
                        //string QeryToGetXcelData = "select * from " + string.Format("{0}${1}", "[Form", "A1:AZ]");
                        conn.ConnectionString = UploadConnStr;
                        conn.Open();

                        //** Get sheet name dynamically by Thamizhmani
                        DataTable Sheets = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        foreach (DataRow dr in Sheets.Rows)
                        {
                            sht = dr[2].ToString().Replace("$", "");
                            sht = sht.Replace("'", "");
                            QeryToGetXcelData = "select * from " + string.Format("{0}${1}", "[" + sht + "", "A1:AZ]");
                        }

                        OleDbCommand cmd = new OleDbCommand(QeryToGetXcelData, conn);
                        cmd.CommandType = CommandType.Text;
                        OleDbDataAdapter DtAdptrr = new OleDbDataAdapter();
                        DtAdptrr.SelectCommand = cmd;
                        DtAdptrr.Fill(DtblXcelData);
                        conn.Close();
                        if (DtblXcelData.Rows.Count == 0)
                        {
                            if (uploadlog.UploadStatus == "YetToUpload")
                                uploadlog.UploadStatus = "UploadFailed";
                            uploadlog.ErrDesc = "Empty file uploaded";
                            orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);
                            //return Json(new { success = false, result = "No Rows available in the file to update!!!" }, "text/html", JsonRequestBehavior.AllowGet);
                        }
                        else if (DtblXcelData.Rows.Count > 0)
                        {
                            QeryToGetXcelData = "select * from " + string.Format("{0}${1}", "[" + sht + "", "A7:AZ]");
                            conn.ConnectionString = UploadConnStr;
                            conn.Open();
                            cmd = new OleDbCommand(QeryToGetXcelData, conn);
                            cmd.CommandType = CommandType.Text;
                            DtAdptrr = new OleDbDataAdapter();
                            DtAdptrr.SelectCommand = cmd;
                            DtAdptrr.Fill(DtblXcelData);
                            string[] strArray = { "F1", "F2", "F3", "F4", "F5", "F6", "F7", "Line #", "UNRS", "Commodity", "Order Qty", "UOM", "Unit Price", "Total (USD)" };
                            char chrFlag = 'Y';
                            if (DtblXcelData.Columns.Count == strArray.Length)
                            {
                                int j = 0;
                                string[] strColumnsAray = new string[DtblXcelData.Columns.Count];
                                foreach (DataColumn dtColumn in DtblXcelData.Columns)
                                {
                                    strColumnsAray[j] = dtColumn.ColumnName;
                                    j++;
                                }
                                for (int i = 0; i < strArray.Length - 1; i++)
                                {
                                    if (strArray[i].Trim() != strColumnsAray[i].Trim())
                                    {
                                        chrFlag = 'N';
                                        break;
                                    }
                                }
                                if (chrFlag == 'Y')
                                {
                                    orderid = GetCounterValue("Orders");
                                    Orders ord = new Orders();
                                    IList<OrderItems> Orderitemslist = new List<OrderItems>();

                                    foreach (DataRow Ordline in DtblXcelData.Rows)
                                    {
                                        if (Ordline.ItemArray[0].ToString().Trim() == "Mission")
                                        {
                                            ord.Mission = Ordline.ItemArray[1].ToString();
                                            //ord.Name = Ordline.ItemArray[1].ToString();
                                            //ord.ContingentType = ord.Name.Contains("FPU") ? "FP" : "ML";
                                        }
                                        if (Ordline.ItemArray[5].ToString().Trim() == "Start Date")
                                        {
                                            string ordstdt = Ordline.ItemArray[6].ToString();
                                            if (!string.IsNullOrEmpty(ordstdt))
                                                ord.StartDate = DateTime.Parse(ordstdt, provider, System.Globalization.DateTimeStyles.NoCurrentDateDefault);
                                        }
                                        if (Ordline.ItemArray[0].ToString().Trim() == "DP")
                                        {
                                            ord.DP = Ordline.ItemArray[1].ToString();
                                            string[] contingentIdarray = Ordline.ItemArray[1].ToString().Split('[');
                                            ord.ContingentId = Convert.ToInt64(contingentIdarray[0]);
                                        }
                                        if (Ordline.ItemArray[0].ToString().Trim() == "DP")
                                        {
                                            ord.LocationCMR = Convert.ToDecimal(Ordline.ItemArray[3].ToString());
                                        }
                                        if (Ordline.ItemArray[5].ToString().Trim() == "End Date")
                                        {
                                            string ordenddt = Ordline.ItemArray[6].ToString();
                                            if (!string.IsNullOrEmpty(ordenddt))
                                                ord.EndDate = DateTime.Parse(ordenddt, provider, System.Globalization.DateTimeStyles.NoCurrentDateDefault);
                                        }
                                        if (Ordline.ItemArray[0].ToString().Trim() == "Control#")
                                        {
                                            ord.ControlId = Ordline.ItemArray[1].ToString();
                                            //Get previous controlids for validation
                                            criteria.Add("ControlId", ord.ControlId);
                                            Dictionary<long, IList<Orders>> orders = orderService.GetOrdersListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                                            if (orders != null && orders.Count > 0)
                                            {
                                                IList<Orders> orderlist = orders.FirstOrDefault().Value.ToList();
                                                var ControlIdArray = (from u in orderlist select new { u.ControlId }).Distinct().ToArray();
                                                for (int i = 0; i < ControlIdArray.Length; i++)
                                                {
                                                    if (ControlIdArray[i].ControlId == ord.ControlId)
                                                    {
                                                        //AlreadyExistFile = AlreadyExistFile + 1;
                                                        AlreadyExistFlag = "true";
                                                        alreadyExists.Append(fileName);
                                                        if (uploadlog.UploadStatus == "YetToUpload")
                                                            uploadlog.UploadStatus = "AlreadyExist";
                                                        orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);
                                                    }
                                                }
                                            }
                                            string[] controlIdarray = Ordline.ItemArray[1].ToString().Split('-');
                                            ord.Sector = controlIdarray[2];
                                            ord.Week = Convert.ToInt64(controlIdarray[6].ToString().Replace("WK", ""));
                                            ord.Period = controlIdarray[5];
                                            ord.PeriodYear = controlIdarray[7];
                                            ord.CalYear = Convert.ToInt64(controlIdarray[7]);
                                            ord.Name = controlIdarray[3];
                                            //ord.ContingentType = ord.Name.Contains("FPU") ? "FP" : "ML";// Modified by Thamizhmani on Nov 15 2016
                                            ord.ContingentType = ord.Name.Contains("FPU") ? "FPU" : "MIL";
                                            ord.Location = controlIdarray[4];
                                        }
                                        //if (Ordline.ItemArray[0].ToString().Trim() == "Control#")
                                        //{
                                        //    ord.ControlCMR = Convert.ToDecimal(Ordline.ItemArray[2].ToString());
                                        //}
                                        if (Ordline.ItemArray[3].ToString().Trim() == "Purch. Gr.")
                                        {
                                            ord.PurchGr = Ordline.ItemArray[4].ToString();
                                        }
                                        if (Ordline.ItemArray[5].ToString().Trim() == "Ship. Ins.")
                                        {
                                            ord.ShipIns = Ordline.ItemArray[6].ToString();
                                        }
                                        if (Ordline.ItemArray[0].ToString().Trim() == "FFO#")
                                        {
                                            ord.FFOId = Convert.ToInt64(Ordline.ItemArray[1].ToString());
                                        }
                                        if (Ordline.ItemArray[3].ToString().Trim() == "Troops #")
                                        {
                                            ord.Troops = Convert.ToDecimal(Ordline.ItemArray[4].ToString());
                                        }
                                        if (Ordline.ItemArray[5].ToString().Trim() == "Rel. Req.")
                                        {
                                            ord.RelReq = Ordline.ItemArray[6].ToString();
                                        }
                                        if (Ordline.ItemArray[3].ToString().Trim() == "Total amount")
                                        {
                                            ord.TotalAmount = Convert.ToDecimal(Ordline.ItemArray[6].ToString().Replace("USD", ""));
                                        }
                                        if (Ordline.ItemArray[3].ToString().Trim() == "# Line Items Ordered")
                                        {
                                            ord.LineItemsOrdered = Convert.ToDecimal(Ordline.ItemArray[6].ToString());
                                        }
                                        if (Ordline.ItemArray[3].ToString().Trim() == "Kg Ordered w/o eggs")
                                        {
                                            ord.KgOrderedWOEggs = Convert.ToDecimal(Ordline.ItemArray[6].ToString());
                                        }
                                        if (Ordline.ItemArray[3].ToString().Trim() == "Eggs weight")
                                        {
                                            ord.EggsWeight = Convert.ToDecimal(Ordline.ItemArray[6].ToString());
                                        }
                                        if (Ordline.ItemArray[3].ToString().Trim() == "Total weight")
                                        {
                                            ord.TotalWeight = Convert.ToDecimal(Ordline.ItemArray[6].ToString());
                                        }
                                        if (Ordline["UNRS"].ToString() != "")
                                        {
                                            OrderItems orditem = new OrderItems();
                                            // orditem.OrderId = orderid;
                                            //orditem.LineId = GetCounterValue("OrderItems");
                                            orditem.Line_No = Convert.ToInt64(Ordline["Line #"].ToString());
                                            orditem.UOM = Ordline["UOM"].ToString();
                                            orditem.UNCode = Convert.ToInt64(Ordline["UNRS"].ToString());
                                            orditem.Commodity = Ordline["Commodity"].ToString();
                                            orditem.OrderQty = Convert.ToDecimal(Ordline["Order Qty"].ToString());
                                            orditem.SectorPrice = Convert.ToDecimal(Ordline["Unit Price"].ToString());
                                            //orditem.Total = Convert.ToDecimal(Ordline["Total (USD)"].ToString().Replace("$", ""));
                                            orditem.Total = Convert.ToDecimal(Ordline["Total (USD)"].ToString());
                                            // orditem.CreatedBy = loggedInUserId;----> needed
                                            orditem.CreatedBy = uploadlog.CreatedBy;
                                            orditem.CreatedDate = DateTime.Now;
                                            orditem.RemainingOrdQty = Convert.ToDecimal(Ordline["Order Qty"].ToString());
                                            //orditem.Status = "Not Delivered";
                                            //string total = Ordline["Total"].ToString().Replace("$", "");
                                            //orditem.Total = Convert.ToDecimal();
                                            //  if (AlreadyExistFile == 0)
                                            if (AlreadyExistFlag != "true")
                                            {
                                                Orderitemslist.Add(orditem);
                                                //orderService.SaveOrUpdateOrderItems(orditem);
                                            }
                                        }
                                    }
                                    //if (AlreadyExistFile > 0)
                                    //{
                                    //    continue;
                                    //}
                                    ord.CreatedBy = uploadlog.CreatedBy;
                                    //ord.CreatedBy = loggedInUserId;===> need to be 
                                    ord.CreatedDate = DateTime.Now;
                                    // ord.OrderId = orderid;//----------------------------------------------->kingston
                                    ord.InvoiceStatus = "YetToGenerate";
                                    ord.DocumentData = UploadedFile; //add by john

                                    if (AlreadyExistFlag != "true")
                                    {
                                        try
                                        {
                                            orderService.SaveOrUpdateOrdersUsingSession(ord, Orderitemslist);
                                            if (uploadlog.UploadStatus == "YetToUpload")
                                                uploadlog.UploadStatus = "UploadedSuccessfully";
                                            uploadlog.ReferenceNo = "ORD-" + ord.OrderId;
                                            orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);
                                        }
                                        catch (Exception ex)
                                        {
                                            if (uploadlog.UploadStatus == "YetToUpload")
                                                uploadlog.UploadStatus = "UploadFailed";
                                            uploadlog.ErrDesc = "Orders upload transaction failiure----" + ex.ToString();
                                            orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);
                                        }

                                        //long ordid = orderService.SaveOrUpdateOrder(ord);
                                        //orderService.SaveOrUpdateOrderItemsList(Orderitemslist);
                                        decimal OrderqtyEggsonly = 0;
                                        decimal OrderqtyWithoutEggs = 0;
                                        decimal TotalAmt = 0;

                                        criteria.Clear();
                                        criteria.Add("OrderId", ord.OrderId);
                                        Dictionary<long, IList<OrderItems>> orditems = orderService.GetOrderItemsListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                                        for (int i = 0; i < orditems.FirstOrDefault().Value.Count(); i++)
                                        {
                                            TotalAmt = TotalAmt + (orditems.FirstOrDefault().Value[i].OrderQty * orditems.FirstOrDefault().Value[i].SectorPrice);

                                            if (orditems.FirstOrDefault().Value[i].UNCode == 1129)
                                            {
                                                OrderqtyEggsonly = OrderqtyEggsonly + (orditems.FirstOrDefault().Value[i].OrderQty * (decimal)0.058824);
                                            }
                                            if (orditems.FirstOrDefault().Value[i].UNCode != 1129)
                                            {
                                                OrderqtyWithoutEggs = OrderqtyWithoutEggs + (orditems.FirstOrDefault().Value[i].OrderQty);
                                            }
                                        }
                                        ord.TotalAmount = TotalAmt;
                                        ord.EggsWeight = OrderqtyEggsonly;
                                        ord.KgOrderedWOEggs = OrderqtyWithoutEggs;
                                        ord.TotalWeight = OrderqtyEggsonly + OrderqtyWithoutEggs;
                                        ord.DocumentData = UploadedFile; //add by john
                                        orderService.SaveOrUpdateOrder(ord);
                                        UploadedFilename.Append(fileName + ",");
                                    }


                                }
                                else
                                {
                                    if (uploadlog.UploadStatus == "YetToUpload")
                                        uploadlog.UploadStatus = "UploadFailed";
                                    uploadlog.ErrDesc = "Headers error ---The no of columns will be leaser or spelling change in headers";
                                    orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);

                                    //ErrorFilename.Append(fileName + ",");
                                }



                            }

                            else
                            {
                                if (uploadlog.UploadStatus == "YetToUpload")
                                    uploadlog.UploadStatus = "UploadFailed";
                                uploadlog.ErrDesc = "Headers error  or transaction errors";
                                orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);

                                //ErrorFilename.Append(fileName + ",");
                            }
                        }
                        //success = success + 1;

                    }
                    catch (Exception ex)
                    {
                        if (uploadlog.UploadStatus == "YetToUpload")
                            uploadlog.UploadStatus = "UploadFailed";
                        uploadlog.ErrDesc = ex.ToString();
                        orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);
                        ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                        throw ex;
                        //ErrorFilename.Append(fileName + ",");

                    }
                }
                criteria.Clear();
                StatusUpdateInUploadRequest(RequestId);
            }



            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }


        }



        //Bulk delivery note upload
        public void callParallelDeliveryNote(long RequestId)
        {

            new Task(() => { CreateDeliveryNoteParallelProcessing(RequestId); }).Start();
        }

        // Saving all delivery note files in one location

        public ActionResult DeliveryNoteBulkUpload(long RequestId)
        {
            string RequestNo = "REQUEST-" + RequestId;
            var files = Request.Files["Filedata"];

            string curpath = ConfigurationManager.AppSettings["DeliveryNoteUploadRequestFilePath"].ToString();
            // string curpath = "D:\\INSIGHTBACKUP1\\LIVE\\DeliveryNotes";
            string RequestFolder = String.Format("{0}\\" + RequestNo, curpath);
            // If the folder is not existed, create it.
            if (!Directory.Exists(RequestFolder)) { Directory.CreateDirectory(RequestFolder); }
            string savePath = RequestFolder + "\\" + files.FileName;
            files.SaveAs(savePath);
            return Content(Url.Content(@"~\Content\" + files.FileName));

        }

        //public void CreateDeliveryNoteParallelProcessing(long RequestId)
        //{
        //    try
        //    {

        //        //construct the result string
        //        //first successful uploaded files, then already exists and error
        //        Dictionary<string, object> criteria = new Dictionary<string, object>();
        //        string UploadConnStr = string.Empty;
        //        StringBuilder alreadyExists = new StringBuilder();
        //        StringBuilder ErrorFilename = new StringBuilder();
        //        StringBuilder UploadedFilename = new StringBuilder();
        //        int AlreadyExistFile = 0;
        //        string fileExtn = string.Empty;
        //        UploadRequest upreq = new UploadRequest();
        //        if (RequestId > 0)
        //        {
        //            upreq = orderService.GetUploadRequestbyRequestId(RequestId);
        //            if (upreq.UploadStatus == "Request Generated")
        //            {
        //                upreq.UploadStatus = "InProgress";
        //                orderService.SaveOrUpdateUploadRequest(upreq);
        //            }
        //        }
        //        //string curpath = "D:\\INSIGHTBACKUP1\\LIVE\\Orders\\" + "REQUEST-" + RequestId;
        //        string curpath = ConfigurationManager.AppSettings["DeliveryNoteUploadRequestFilePath"].ToString();
        //        string[] filePaths = Directory.GetFiles(curpath + "\\REQUEST-" + RequestId, "*.xlsx");
        //        // string[] filePaths = Directory.GetFiles(@"D:\INSIGHTBACKUP1\LIVE\DeliveryNotes\" + "REQUEST-" + RequestId, "*.xlsx");
        //        //string[] filePaths = Directory.GetFiles(@"E:\POManagement\UploadedDocs", "*.xlsx");
        //        foreach (string s in filePaths)
        //        {
        //            string AlreadyExistFlag = string.Empty;
        //            string fileName = s.ToString();
        //            long ImportedDeliveryNoteId = GetCounterValue("ImportedDeliveryNote");
        //            long UploadReqDetLogId = CreateOrUpdateRequest(fileName, RequestId);

        //            //Delivery note's in Byte array
        //            byte[] UploadedFile = System.IO.File.ReadAllBytes(fileName);

        //            UploadRequestDetailsLog uploadlog = orderService.GetUploadRequestDetailsLogbyRequestId(UploadReqDetLogId);
        //            UploadConnStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + s + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";
        //            OleDbConnection conn = new OleDbConnection();
        //            DataTable DtblXcelData = new DataTable();
        //            string QeryToGetXcelData = "select * from " + string.Format("{0}${1}", "[Delivery_Note", "A9:L15]");
        //            conn.ConnectionString = UploadConnStr;
        //            conn.Open();
        //            OleDbCommand cmd = new OleDbCommand(QeryToGetXcelData, conn);
        //            cmd.CommandType = CommandType.Text;
        //            OleDbDataAdapter DtAdptrr = new OleDbDataAdapter();
        //            DtAdptrr.SelectCommand = cmd;
        //            DtAdptrr.Fill(DtblXcelData);
        //            conn.Close();
        //            if (DtblXcelData.Rows.Count == 0)
        //            {
        //                if (uploadlog.UploadStatus == "YetToUpload")
        //                    uploadlog.UploadStatus = "UploadFailed";
        //                uploadlog.ErrDesc = "Empty file uploaded";
        //                orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);
        //                //return Json(new { success = false, result = "No Rows available in the file to update!!!" }, "text/html", JsonRequestBehavior.AllowGet);
        //            }
        //            else if (DtblXcelData.Rows.Count > 0)
        //            {
        //                ImportedDeliveryNote impdel = new ImportedDeliveryNote();
        //                foreach (DataRow delline in DtblXcelData.Rows)
        //                {
        //                    if (delline.ItemArray[0].ToString().Trim() == "Delivery Note Number:")
        //                    {
        //                        string deliverynotename = delline.ItemArray[2].ToString();
        //                        impdel.ImpDeliveryNoteName = deliverynotename;

        //                        Dictionary<long, IList<DeliveryNote>> delnote = orderService.GetDeliveryNoteListWithPagingAndCriteria(0, 999999, string.Empty, string.Empty, criteria);
        //                        if (delnote != null && delnote.Count > 0)
        //                        {
        //                            IList<DeliveryNote> deliverynotelist = delnote.FirstOrDefault().Value.ToList();
        //                            var DeliveryNoteName = (from u in deliverynotelist select u.DeliveryNoteName).Distinct().ToArray();

        //                            Dictionary<long, IList<ImportedDeliveryNote>> impdelnote = orderService.GetImportedDeliveryNoteListWithPagingAndCriteria(0, 999999, string.Empty, string.Empty, criteria);
        //                            if (delnote != null && delnote.Count > 0)
        //                            {

        //                                IList<ImportedDeliveryNote> importeddeliverynotelist = impdelnote.FirstOrDefault().Value.ToList();
        //                                var impdelnotename = (from u in importeddeliverynotelist select u.ImpDeliveryNoteName).Distinct().ToArray();
        //                                string[] AllDeliveryNotelist = DeliveryNoteName.ToArray();
        //                                // string[] AllDeliveryNotelist = DeliveryNoteName.Union(impdelnotename).ToArray();
        //                                for (int i = 0; i < AllDeliveryNotelist.Count(); i++)
        //                                {
        //                                    if (AllDeliveryNotelist[i] == deliverynotename)
        //                                    {
        //                                        AlreadyExistFlag = "true";
        //                                        // AlreadyExistFile = AlreadyExistFile + 1;
        //                                        alreadyExists.Append(fileName);
        //                                        if (uploadlog.UploadStatus == "YetToUpload")
        //                                            uploadlog.UploadStatus = "AlreadyExist";
        //                                        orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);
        //                                    }
        //                                }
        //                            }
        //                        }

        //                    }
        //                    if (delline.ItemArray[3].ToString().Trim() == "Delivery Note Type:") {
        //                        impdel.ImpDeliveryNoteType = delline.ItemArray[5].ToString();
        //                    }
        //                    if (delline.ItemArray[8].ToString().Trim() == "Shipment Date:")
        //                    {

        //                        string shipmentdate = delline.ItemArray[9].ToString();
        //                        if (!string.IsNullOrEmpty(shipmentdate))
        //                            impdel.ImpShipmentDate = DateTime.Parse(shipmentdate, provider, System.Globalization.DateTimeStyles.NoCurrentDateDefault);

        //                    }
        //                    if (delline.ItemArray[0].ToString().Trim() == "Request#:")
        //                    {
        //                        impdel.ImpRequestNo = delline.ItemArray[2].ToString().Trim() == "" ? 0 : Convert.ToInt64(delline.ItemArray[2].ToString());
        //                        // impdel.ImpRequestNo = Convert.ToInt64(delline.ItemArray[2].ToString());

        //                    }

        //                    if (delline.ItemArray[3].ToString().Trim() == "Consumption Week:")
        //                    {
        //                        // impdel.ImpConsumptionWeek = Convert.ToDecimal(delline.ItemArray[5].ToString());
        //                        impdel.ImpConsumptionWeek = delline.ItemArray[5].ToString().Trim() == "" ? 0 : Convert.ToDecimal(delline.ItemArray[5].ToString());

        //                    }

        //                    if (delline.ItemArray[8].ToString().Trim() == "Unit Control No:")
        //                    {
        //                        impdel.ImpControlId = delline.ItemArray[9].ToString();
        //                        string[] controlIdarray = delline.ItemArray[9].ToString().Split('-');
        //                        impdel.Sector = controlIdarray[1].ToString();
        //                        impdel.Name = controlIdarray[3].ToString();
        //                        impdel.Location = controlIdarray[1].ToString() + "-" + controlIdarray[2].ToString();
        //                        impdel.Period = controlIdarray[4].ToString();
        //                        impdel.PeriodYear = controlIdarray[6].ToString() + "-" + controlIdarray[7].ToString();

        //                    }
        //                    if (delline.ItemArray[0].ToString().Trim() == "Warehouse:")
        //                    {
        //                        impdel.ImpWarehouse = delline.ItemArray[2].ToString();

        //                    }
        //                    if (delline.ItemArray[3].ToString().Trim() == "Delivery Week:")
        //                    {
        //                        impdel.ImpDeliveryWeek = delline.ItemArray[5].ToString().Trim() == "" ? 0 : Convert.ToDecimal(delline.ItemArray[5].ToString());

        //                    }
        //                    if (delline.ItemArray[0].ToString().Trim() == "Strength:")
        //                    {
        //                        impdel.ImpStrength = delline.ItemArray[2].ToString().Trim() == "" ? 0 : Convert.ToDecimal(delline.ItemArray[2].ToString());

        //                    }
        //                    if (delline.ItemArray[3].ToString().Trim() == "Delivery Mode:")
        //                    {
        //                        impdel.ImpDeliveryMode = delline.ItemArray[5].ToString();
        //                        //impdel.ImpDeliveryMode =delline.ItemArray[5].ToString().Trim()==""?0: delline.ItemArray[5].ToString();

        //                    }
        //                    if (delline.ItemArray[8].ToString().Trim() == "UN Food Order:")
        //                    {
        //                        impdel.ImpUNFoodOrder = delline.ItemArray[9].ToString();

        //                    }

        //                    if (delline.ItemArray[0].ToString().Trim() == "DOS:")
        //                    {
        //                        // impdel.ImpDOS = Convert.ToDecimal(delline.ItemArray[2].ToString());
        //                        impdel.ImpDOS = delline.ItemArray[2].ToString().ToString().Trim() == "" ? 0 : Convert.ToDecimal(delline.ItemArray[2].ToString());

        //                    }
        //                    if (delline.ItemArray[3].ToString().Trim() == "Seal No:")
        //                    {
        //                        impdel.ImpSealNo = delline.ItemArray[5].ToString();

        //                    }
        //                    if (delline.ItemArray[8].ToString().Trim() == "UN Week:")
        //                    {
        //                        //impdel.ImpUNWeek = Convert.ToDecimal(delline.ItemArray[9].ToString());
        //                        impdel.ImpUNWeek = delline.ItemArray[9].ToString().Trim() == "" ? 0 : Convert.ToDecimal(delline.ItemArray[9].ToString());

        //                    }
        //                    if (delline.ItemArray[0].ToString().Trim() == "Man days:")
        //                    {
        //                        //impdel.ImpManDays = Convert.ToDecimal(delline.ItemArray[2].ToString());
        //                        impdel.ImpManDays = delline.ItemArray[2].ToString().Trim() == "" ? 0 : Convert.ToDecimal(delline.ItemArray[2].ToString());

        //                    }
        //                    if (delline.ItemArray[8].ToString().Trim() == "Period:")
        //                    {
        //                        impdel.ImpPeriod = delline.ItemArray[9].ToString();

        //                    }

        //                }
        //                Orders ord = orderService.GetOrderByControlId(impdel.ImpControlId);
        //                if (ord != null)
        //                {
        //                    impdel.OrderId = ord.OrderId;

        //                    //  impdel.ImpDeliveryNoteId = ImportedDeliveryNoteId;//--------------------------->kingston
        //                    impdel.CreatedBy = uploadlog.CreatedBy;
        //                    impdel.CreatedDate = DateTime.Now;
        //                    //impdel.ImpDeliveryNoteId = ImportedDeliveryNoteId;
        //                    if (AlreadyExistFile == 0)
        //                    {

        //                        //  impdel.status = "ADDEDINIMPDELIVERYNOTE";
        //                        //  long ImportedDeliveryNoteId = ordser.SaveOrUpdateImportedDeliveryNote(impdel);

        //                        // UploadedFilename.Append(fileName + ",");
        //                    }
        //                    OleDbConnection itemconn = new OleDbConnection();
        //                    DataTable DtblXcelItemData = new DataTable();
        //                    string QeryToGetXcelItemData = "select [Line#],[Item],[Descreption],[Ordered Qty],[Delivered Qty],[Number of Packs],[Number of Pieces],[Substitute For],[Substitute Name],[UOM],[Issue Type],[Remarks],[Line Status if Substitution],[Actual Received Qty#],[Recd Date @ Contingent] from " + string.Format("{0}${1}", "[Delivery_Note", "A16:AZ]");
        //                    //string QeryToGetXcelItemData = "select * from " + string.Format("{0}${1}", "[Delivery_Note", "A16:AZ]");
        //                    //string QeryToGetXcelItemData = "select * from " + string.Format("{0}${1}", "[Delivery_Note", "A16:AZ]");
        //                    itemconn.ConnectionString = UploadConnStr;
        //                    itemconn.Open();
        //                    cmd = new OleDbCommand(QeryToGetXcelItemData, conn);
        //                    cmd.CommandType = CommandType.Text;
        //                    DtAdptrr = new OleDbDataAdapter();
        //                    DtAdptrr.SelectCommand = cmd;
        //                    DtAdptrr.Fill(DtblXcelItemData);
        //                    string[] strArray = { "Line#", "Item", "Descreption", "Ordered Qty", "Delivered Qty", "Number of Packs", "Number of Pieces", "Substitute For", "Substitute Name", "UOM", "Issue Type", "Remarks", "Line Status if Substitution", "Actual Received Qty#", "Recd Date @ Contingent" };
        //                    char chrFlag = 'Y';
        //                    if (DtblXcelItemData.Columns.Count == strArray.Length)
        //                    {
        //                        int j = 0;
        //                        string[] strColumnsAray = new string[DtblXcelItemData.Columns.Count];
        //                        foreach (DataColumn dtColumn in DtblXcelItemData.Columns)
        //                        {
        //                            strColumnsAray[j] = dtColumn.ColumnName;
        //                            j++;
        //                        }
        //                        for (int i = 0; i < strArray.Length - 1; i++)
        //                        {
        //                            if (strArray[i].Trim() != strColumnsAray[i].Trim())
        //                            {
        //                                chrFlag = 'N';
        //                                break;
        //                            }
        //                        }
        //                        if (chrFlag == 'Y')
        //                        {
        //                            // ordser = new OrdersService();
        //                            IList<ImportedDeliveryNoteItems> importeddeliverynoteitems = new List<ImportedDeliveryNoteItems>();

        //                            foreach (DataRow OrdItemline in DtblXcelItemData.Rows)
        //                            {

        //                                if (OrdItemline["Item"].ToString().Trim() != "")
        //                                {
        //                                    ImportedDeliveryNoteItems impdelitems = new ImportedDeliveryNoteItems();
        //                                    impdelitems.OrderId = ord.OrderId;
        //                                    if (OrdItemline["Line Status if Substitution"].ToString().Trim() == "" || OrdItemline["Line Status if Substitution"].ToString().ToUpper() == "NONE")
        //                                    {

        //                                        impdelitems.ImpUNCode = Convert.ToInt64(OrdItemline["Item"].ToString().Trim());
        //                                        impdelitems.ImpCommodity = OrdItemline["Descreption"].ToString();

        //                                        impdelitems.ImpSubsItemCode = OrdItemline["Substitute For"].ToString().Trim() == "" ? 0 : Convert.ToInt64(OrdItemline["Substitute For"].ToString().Trim());
        //                                        impdelitems.ImpSubsItemName = OrdItemline["Substitute Name"].ToString().Trim();
        //                                        impdelitems.ImpSubsStatus = "NONE";
        //                                    }
        //                                    else if (OrdItemline["Line Status if Substitution"].ToString().Trim() != "")
        //                                    {
        //                                        impdelitems.ImpUNCode = OrdItemline["Substitute For"].ToString().Trim() == "" ? 0 : Convert.ToInt64(OrdItemline["Substitute For"].ToString().Trim());
        //                                        impdelitems.ImpCommodity = OrdItemline["Substitute Name"].ToString().Trim();

        //                                        impdelitems.ImpSubsItemCode = Convert.ToInt64(OrdItemline["Item"].ToString().Trim());
        //                                        impdelitems.ImpSubsItemName = OrdItemline["Descreption"].ToString();
        //                                        impdelitems.ImpSubsStatus = OrdItemline["Line Status if Substitution"].ToString().Trim();

        //                                    }
        //                                    // long ImportedDeliveryNoteId = GetCounterValue("ImportedDeliveryNote");
        //                                    //impdelitems.ImpDeliveryNoteId = ImportedDeliveryNoteId;//------------------------------------------------>kingston
        //                                    impdelitems.ImpControlId = impdel.ImpControlId;
        //                                    impdelitems.ImpDeliveryMode = impdel.ImpDeliveryMode;

        //                                    impdelitems.ImpDeliveryNoteName = impdel.ImpDeliveryNoteName;
        //                                    impdelitems.OrderId = ord.OrderId;

        //                                    impdelitems.ImpOrderQty = Convert.ToDecimal(OrdItemline["Ordered Qty"].ToString().Trim());
        //                                    impdelitems.ImpDeliveryQty = OrdItemline["Actual Received Qty#"].ToString().Trim() == "" ? 0 : Convert.ToDecimal(OrdItemline["Actual Received Qty#"].ToString().Trim());
        //                                    impdelitems.ImpNoOfPacks = OrdItemline["Number of Packs"].ToString().Trim() == "" ? 0 : Convert.ToDecimal(OrdItemline["Number of Packs"].ToString().Trim());
        //                                    //impdelitems.ImpNoOfPacks = Convert.ToDecimal(OrdItemline["Number of Packs"].ToString().Trim());
        //                                    impdelitems.ImpNoOfPieces = OrdItemline["Number of Pieces"].ToString().Trim() == "" ? 0 : Convert.ToDecimal(OrdItemline["Number of Pieces"].ToString().Trim());
        //                                    //impdelitems.ImpNoOfPieces = Convert.ToDecimal(OrdItemline["Number of Pieces"].ToString().Trim());

        //                                    impdelitems.ImpUOM = OrdItemline["UOM"].ToString().Trim();
        //                                    impdelitems.ImpIssueType = OrdItemline["Issue Type"].ToString().Trim();
        //                                    impdelitems.ImpRemarks = OrdItemline["Remarks"].ToString().Trim();
        //                                    //  impdelitems.ImpSubsStatus = OrdItemline["Line Status if Substitution"].ToString().Trim();
        //                                    string ImpDeliveryDate = OrdItemline["Recd Date @ Contingent"].ToString().Trim();
        //                                    impdelitems.ImpExpDeliveryQty = OrdItemline["Delivered Qty"].ToString().Trim() == "" ? 0 : Convert.ToDecimal(OrdItemline["Delivered Qty"].ToString().Trim());
        //                                    if (!string.IsNullOrEmpty(ImpDeliveryDate))
        //                                        // impdelitems.ImpDeliveryDate = DateTime.Parse(ImpDeliveryDate, provider, System.Globalization.DateTimeStyles.NoCurrentDateDefault);
        //                                        impdelitems.ImpDeliveryDate = Convert.ToDateTime(OrdItemline["Recd Date @ Contingent"].ToString().Trim());
        //                                    impdelitems.CreatedBy = uploadlog.CreatedBy;
        //                                    impdelitems.CreatedDate = DateTime.Now;
        //                                    // if (AlreadyExistFile == 0)
        //                                    if (AlreadyExistFlag != "true")
        //                                    {
        //                                        importeddeliverynoteitems.Add(impdelitems);
        //                                        //ordser.SaveOrUpdateImportedDeliveryNoteItems(impdelitems);
        //                                        // UpdateImportedDeliveryNoteItemDetailsinOrdItemsTbl(impdelitems);

        //                                    }
        //                                }

        //                            }
        //                            if (importeddeliverynoteitems.Count != 0)
        //                            {
        //                                try
        //                                {
        //                                    orderService.SaveorUpdateDeliveryNoteInSingleSession(impdel, importeddeliverynoteitems);
        //                                    //orderService.SaveOrUpdateImportedDeliveryNoteItemsList(importeddeliverynoteitems);
        //                                    criteria.Clear();
        //                                    criteria.Add("ImpDeliveryNoteId", importeddeliverynoteitems[0].ImpDeliveryNoteId);
        //                                    Dictionary<long, IList<ImportedDeliveryNoteItems>> impdelnoteitemslist = orderService.GetImportedDeliveryNoteItemsListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
        //                                    IList<ImportedDeliveryNoteItems> delnoteitems = impdelnoteitemslist.FirstOrDefault().Value.ToList();
        //                                    UpdateImportedDeliveryNoteItemDetailsinOrdItemsTbl(importeddeliverynoteitems, UploadedFile);
        //                                    criteria.Clear();
        //                                    //UploadedFilename.Append(fileName + ",");
        //                                    if (uploadlog.UploadStatus == "YetToUpload")
        //                                        uploadlog.UploadStatus = "UploadedSuccessfully";
        //                                    uploadlog.ReferenceNo = "DEL-" + importeddeliverynoteitems.FirstOrDefault().ImpDeliveryNoteId;
        //                                    orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);
        //                                }
        //                                catch (Exception ex)
        //                                {

        //                                    if (uploadlog.UploadStatus == "YetToUpload")
        //                                        uploadlog.UploadStatus = "UploadFailed";
        //                                    uploadlog.ErrDesc = "Orders upload transaction failiure----" + ex.ToString();
        //                                    orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);
        //                                }
        //                            }

        //                        }
        //                        else
        //                        {
        //                            if (uploadlog.UploadStatus == "YetToUpload")
        //                                uploadlog.UploadStatus = "UploadFailed";
        //                            uploadlog.ErrDesc = "Headers error ---The no of columns will be leaser or spelling change in headers";
        //                            orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);
        //                            //ErrorFilename.Append(fileName + ",");
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (uploadlog.UploadStatus == "YetToUpload")
        //                            uploadlog.UploadStatus = "UploadFailed";
        //                        uploadlog.ErrDesc = "Headers error  or transaction errors";
        //                        orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);

        //                        // ErrorFilename.Append(fileName + ",");
        //                    }
        //                }
        //                else
        //                {
        //                    if (uploadlog.UploadStatus == "YetToUpload")
        //                        uploadlog.UploadStatus = "UploadFailed";
        //                    uploadlog.ErrDesc = "Order not found";
        //                    orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);

        //                }
        //            }
        //            //   success = success + 1;
        //        }
        //        criteria.Clear();
        //        StatusUpdateInUploadRequest(RequestId);

        //    }
        //    catch (Exception ex)
        //    {

        //        ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
        //    }

        //}



        public void CreateDeliveryNoteParallelProcessing(long RequestId)
        {
            try
            {

                //construct the result string
                //first successful uploaded files, then already exists and error
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                string UploadConnStr = string.Empty;
                StringBuilder alreadyExists = new StringBuilder();
                StringBuilder ErrorFilename = new StringBuilder();
                StringBuilder UploadedFilename = new StringBuilder();
                int AlreadyExistFile = 0;
                string fileExtn = string.Empty;
                UploadRequest upreq = new UploadRequest();
                if (RequestId > 0)
                {
                    upreq = orderService.GetUploadRequestbyRequestId(RequestId);
                    if (upreq.UploadStatus == "Request Generated")
                    {
                        upreq.UploadStatus = "InProgress";
                        orderService.SaveOrUpdateUploadRequest(upreq);
                    }
                }
                //string curpath = "D:\\INSIGHTBACKUP1\\LIVE\\Orders\\" + "REQUEST-" + RequestId;
                string curpath = ConfigurationManager.AppSettings["DeliveryNoteUploadRequestFilePath"].ToString();
                string[] filePaths = Directory.GetFiles(curpath + "\\REQUEST-" + RequestId, "*.xlsx");
                // string[] filePaths = Directory.GetFiles(@"D:\INSIGHTBACKUP1\LIVE\DeliveryNotes\" + "REQUEST-" + RequestId, "*.xlsx");
                //string[] filePaths = Directory.GetFiles(@"E:\POManagement\UploadedDocs", "*.xlsx");
                foreach (string s in filePaths)
                {
                    string AlreadyExistFlag = string.Empty;
                    string fileName = s.ToString();
                    long ImportedDeliveryNoteId = GetCounterValue("ImportedDeliveryNote");
                    long UploadReqDetLogId = CreateOrUpdateRequest(fileName, RequestId);

                    //Delivery note's in Byte array
                    byte[] UploadedFile = System.IO.File.ReadAllBytes(fileName);

                    UploadRequestDetailsLog uploadlog = orderService.GetUploadRequestDetailsLogbyRequestId(UploadReqDetLogId);
                    UploadConnStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + s + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";
                    OleDbConnection conn = new OleDbConnection();
                    DataTable DtblXcelData = new DataTable();
                    string QeryToGetXcelData = "select * from " + string.Format("{0}${1}", "[Delivery_Note", "A9:L15]");
                    conn.ConnectionString = UploadConnStr;
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(QeryToGetXcelData, conn);
                    cmd.CommandType = CommandType.Text;
                    OleDbDataAdapter DtAdptrr = new OleDbDataAdapter();
                    DtAdptrr.SelectCommand = cmd;
                    DtAdptrr.Fill(DtblXcelData);
                    conn.Close();
                    if (DtblXcelData.Rows.Count == 0)
                    {
                        if (uploadlog.UploadStatus == "YetToUpload")
                            uploadlog.UploadStatus = "UploadFailed";
                        uploadlog.ErrDesc = "Empty file uploaded";
                        orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);
                        //return Json(new { success = false, result = "No Rows available in the file to update!!!" }, "text/html", JsonRequestBehavior.AllowGet);
                    }
                    else if (DtblXcelData.Rows.Count > 0)
                    {
                        ImportedDeliveryNote impdel = new ImportedDeliveryNote();
                        foreach (DataRow delline in DtblXcelData.Rows)
                        {
                            if (delline.ItemArray[0].ToString().Trim() == "Delivery Note Number:")
                            {
                                string deliverynotename = delline.ItemArray[2].ToString();
                                impdel.ImpDeliveryNoteName = deliverynotename;

                                Dictionary<long, IList<DeliveryNote>> delnote = orderService.GetDeliveryNoteListWithPagingAndCriteria(0, 999999, string.Empty, string.Empty, criteria);
                                if (delnote != null && delnote.Count > 0)
                                {
                                    IList<DeliveryNote> deliverynotelist = delnote.FirstOrDefault().Value.ToList();
                                    var DeliveryNoteName = (from u in deliverynotelist select u.DeliveryNoteName).Distinct().ToArray();

                                    Dictionary<long, IList<ImportedDeliveryNote>> impdelnote = orderService.GetImportedDeliveryNoteListWithPagingAndCriteria(0, 999999, string.Empty, string.Empty, criteria);
                                    if (delnote != null && delnote.Count > 0)
                                    {

                                        IList<ImportedDeliveryNote> importeddeliverynotelist = impdelnote.FirstOrDefault().Value.ToList();
                                        var impdelnotename = (from u in importeddeliverynotelist select u.ImpDeliveryNoteName).Distinct().ToArray();
                                        string[] AllDeliveryNotelist = DeliveryNoteName.ToArray();
                                        // string[] AllDeliveryNotelist = DeliveryNoteName.Union(impdelnotename).ToArray();
                                        for (int i = 0; i < AllDeliveryNotelist.Count(); i++)
                                        {
                                            if (AllDeliveryNotelist[i] == deliverynotename)
                                            {
                                                AlreadyExistFlag = "true";
                                                // AlreadyExistFile = AlreadyExistFile + 1;
                                                alreadyExists.Append(fileName);
                                                if (uploadlog.UploadStatus == "YetToUpload")
                                                    uploadlog.UploadStatus = "AlreadyExist";
                                                orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);
                                            }
                                        }
                                    }
                                }

                            }
                            if (delline.ItemArray[3].ToString().Trim() == "Delivery Note Type:")
                            {
                                impdel.ImpDeliveryNoteType = delline.ItemArray[5].ToString();
                            }
                            if (delline.ItemArray[8].ToString().Trim() == "Shipment Date:")
                            {

                                string shipmentdate = delline.ItemArray[9].ToString();
                                if (!string.IsNullOrEmpty(shipmentdate))
                                    impdel.ImpShipmentDate = DateTime.Parse(shipmentdate, provider, System.Globalization.DateTimeStyles.NoCurrentDateDefault);

                            }
                            if (delline.ItemArray[0].ToString().Trim() == "Request#:")
                            {
                                impdel.ImpRequestNo = delline.ItemArray[2].ToString().Trim() == "" ? 0 : Convert.ToInt64(delline.ItemArray[2].ToString());
                                // impdel.ImpRequestNo = Convert.ToInt64(delline.ItemArray[2].ToString());

                            }

                            if (delline.ItemArray[3].ToString().Trim() == "Consumption Week:")
                            {
                                // impdel.ImpConsumptionWeek = Convert.ToDecimal(delline.ItemArray[5].ToString());
                                impdel.ImpConsumptionWeek = delline.ItemArray[5].ToString().Trim() == "" ? 0 : Convert.ToDecimal(delline.ItemArray[5].ToString());

                            }

                            if (delline.ItemArray[8].ToString().Trim() == "Unit Control No:")
                            {
                                impdel.ImpControlId = delline.ItemArray[9].ToString();
                                string[] controlIdarray = delline.ItemArray[9].ToString().Split('-');
                                impdel.Sector = controlIdarray[1].ToString();
                                impdel.Name = controlIdarray[3].ToString();
                                impdel.Location = controlIdarray[1].ToString() + "-" + controlIdarray[2].ToString();
                                impdel.Period = controlIdarray[4].ToString();
                                impdel.PeriodYear = controlIdarray[6].ToString() + "-" + controlIdarray[7].ToString();

                            }
                            if (delline.ItemArray[0].ToString().Trim() == "Warehouse:")
                            {
                                impdel.ImpWarehouse = delline.ItemArray[2].ToString();

                            }
                            if (delline.ItemArray[3].ToString().Trim() == "Delivery Week:")
                            {
                                impdel.ImpDeliveryWeek = delline.ItemArray[5].ToString().Trim() == "" ? 0 : Convert.ToDecimal(delline.ItemArray[5].ToString());

                            }
                            if (delline.ItemArray[0].ToString().Trim() == "Strength:")
                            {
                                impdel.ImpStrength = delline.ItemArray[2].ToString().Trim() == "" ? 0 : Convert.ToDecimal(delline.ItemArray[2].ToString());

                            }
                            if (delline.ItemArray[3].ToString().Trim() == "Delivery Mode:")
                            {
                                impdel.ImpDeliveryMode = delline.ItemArray[5].ToString();
                                //impdel.ImpDeliveryMode =delline.ItemArray[5].ToString().Trim()==""?0: delline.ItemArray[5].ToString();

                            }
                            if (delline.ItemArray[8].ToString().Trim() == "UN Food Order:")
                            {
                                impdel.ImpUNFoodOrder = delline.ItemArray[9].ToString();

                            }

                            if (delline.ItemArray[0].ToString().Trim() == "DOS:")
                            {
                                // impdel.ImpDOS = Convert.ToDecimal(delline.ItemArray[2].ToString());
                                impdel.ImpDOS = delline.ItemArray[2].ToString().ToString().Trim() == "" ? 0 : Convert.ToDecimal(delline.ItemArray[2].ToString());

                            }
                            if (delline.ItemArray[3].ToString().Trim() == "Seal No:")
                            {
                                impdel.ImpSealNo = delline.ItemArray[5].ToString();

                            }
                            if (delline.ItemArray[8].ToString().Trim() == "UN Week:")
                            {
                                //impdel.ImpUNWeek = Convert.ToDecimal(delline.ItemArray[9].ToString());
                                impdel.ImpUNWeek = delline.ItemArray[9].ToString().Trim() == "" ? 0 : Convert.ToDecimal(delline.ItemArray[9].ToString());

                            }
                            if (delline.ItemArray[0].ToString().Trim() == "Man days:")
                            {
                                //impdel.ImpManDays = Convert.ToDecimal(delline.ItemArray[2].ToString());
                                impdel.ImpManDays = delline.ItemArray[2].ToString().Trim() == "" ? 0 : Convert.ToDecimal(delline.ItemArray[2].ToString());

                            }
                            if (delline.ItemArray[8].ToString().Trim() == "Period:")
                            {
                                impdel.ImpPeriod = delline.ItemArray[9].ToString();

                            }

                        }
                        Orders ord = orderService.GetOrderByControlId(impdel.ImpControlId);
                        if (ord != null)
                        {
                            impdel.OrderId = ord.OrderId;

                            //  impdel.ImpDeliveryNoteId = ImportedDeliveryNoteId;//--------------------------->kingston
                            impdel.CreatedBy = uploadlog.CreatedBy;
                            impdel.CreatedDate = DateTime.Now;
                            impdel.DocumentData = UploadedFile;
                            //impdel.ImpDeliveryNoteId = ImportedDeliveryNoteId;
                            if (AlreadyExistFile == 0)
                            {

                                //  impdel.status = "ADDEDINIMPDELIVERYNOTE";
                                //  long ImportedDeliveryNoteId = ordser.SaveOrUpdateImportedDeliveryNote(impdel);

                                // UploadedFilename.Append(fileName + ",");
                            }
                            OleDbConnection itemconn = new OleDbConnection();
                            DataTable DtblXcelItemData = new DataTable();
                            string QeryToGetXcelItemData = "select [Line#],[Item],[Description],[Ordered Qty],[Delivered Qty],[Number of Packs],[Number of Pieces],[Substitute For],[Substitute Name],[UOM],[Issue Type],[Remarks],[Line Status if Substitution],[Actual Received Qty#],[Recd Date @ Contingent] from " + string.Format("{0}${1}", "[Delivery_Note", "A16:AZ]");
                            //string QeryToGetXcelItemData = "select * from " + string.Format("{0}${1}", "[Delivery_Note", "A16:AZ]");
                            //string QeryToGetXcelItemData = "select * from " + string.Format("{0}${1}", "[Delivery_Note", "A16:AZ]");
                            itemconn.ConnectionString = UploadConnStr;
                            itemconn.Open();
                            cmd = new OleDbCommand(QeryToGetXcelItemData, conn);
                            cmd.CommandType = CommandType.Text;
                            DtAdptrr = new OleDbDataAdapter();
                            DtAdptrr.SelectCommand = cmd;
                            DtAdptrr.Fill(DtblXcelItemData);
                            string[] strArray = { "Line#", "Item", "Description", "Ordered Qty", "Delivered Qty", "Number of Packs", "Number of Pieces", "Substitute For", "Substitute Name", "UOM", "Issue Type", "Remarks", "Line Status if Substitution", "Actual Received Qty#", "Recd Date @ Contingent" };
                            char chrFlag = 'Y';
                            if (DtblXcelItemData.Columns.Count == strArray.Length)
                            {
                                int j = 0;
                                string[] strColumnsAray = new string[DtblXcelItemData.Columns.Count];
                                foreach (DataColumn dtColumn in DtblXcelItemData.Columns)
                                {
                                    strColumnsAray[j] = dtColumn.ColumnName;
                                    j++;
                                }
                                for (int i = 0; i < strArray.Length - 1; i++)
                                {
                                    if (strArray[i].Trim() != strColumnsAray[i].Trim())
                                    {
                                        chrFlag = 'N';
                                        break;
                                    }
                                }
                                if (chrFlag == 'Y')
                                {
                                    // ordser = new OrdersService();
                                    IList<ImportedDeliveryNoteItems> importeddeliverynoteitems = new List<ImportedDeliveryNoteItems>();

                                    foreach (DataRow OrdItemline in DtblXcelItemData.Rows)
                                    {

                                        if (OrdItemline["Item"].ToString().Trim() != "")
                                        {
                                            ImportedDeliveryNoteItems impdelitems = new ImportedDeliveryNoteItems();
                                            impdelitems.OrderId = ord.OrderId;
                                            if (OrdItemline["Line Status if Substitution"].ToString().Trim() == "" || OrdItemline["Line Status if Substitution"].ToString().ToUpper() == "NONE")
                                            {

                                                impdelitems.ImpUNCode = Convert.ToInt64(OrdItemline["Item"].ToString().Trim());
                                                impdelitems.ImpCommodity = OrdItemline["Description"].ToString();

                                                impdelitems.ImpSubsItemCode = OrdItemline["Substitute For"].ToString().Trim() == "" ? 0 : Convert.ToInt64(OrdItemline["Substitute For"].ToString().Trim());
                                                impdelitems.ImpSubsItemName = OrdItemline["Substitute Name"].ToString().Trim();
                                                impdelitems.ImpSubsStatus = "NONE";
                                            }
                                            else if (OrdItemline["Line Status if Substitution"].ToString().Trim() != "")
                                            {
                                                impdelitems.ImpUNCode = OrdItemline["Substitute For"].ToString().Trim() == "" ? 0 : Convert.ToInt64(OrdItemline["Substitute For"].ToString().Trim());
                                                impdelitems.ImpCommodity = OrdItemline["Substitute Name"].ToString().Trim();

                                                impdelitems.ImpSubsItemCode = Convert.ToInt64(OrdItemline["Item"].ToString().Trim());
                                                impdelitems.ImpSubsItemName = OrdItemline["Description"].ToString();
                                                impdelitems.ImpSubsStatus = OrdItemline["Line Status if Substitution"].ToString().Trim();

                                            }
                                            // long ImportedDeliveryNoteId = GetCounterValue("ImportedDeliveryNote");
                                            //impdelitems.ImpDeliveryNoteId = ImportedDeliveryNoteId;//------------------------------------------------>kingston
                                            impdelitems.ImpControlId = impdel.ImpControlId;
                                            impdelitems.ImpDeliveryMode = impdel.ImpDeliveryMode;

                                            impdelitems.ImpDeliveryNoteName = impdel.ImpDeliveryNoteName;
                                            impdelitems.OrderId = ord.OrderId;

                                            impdelitems.ImpOrderQty = Convert.ToDecimal(OrdItemline["Ordered Qty"].ToString().Trim());
                                            impdelitems.ImpDeliveryQty = OrdItemline["Actual Received Qty#"].ToString().Trim() == "" ? 0 : Convert.ToDecimal(OrdItemline["Actual Received Qty#"].ToString().Trim());
                                            impdelitems.ImpNoOfPacks = OrdItemline["Number of Packs"].ToString().Trim() == "" ? 0 : Convert.ToDecimal(OrdItemline["Number of Packs"].ToString().Trim());
                                            //impdelitems.ImpNoOfPacks = Convert.ToDecimal(OrdItemline["Number of Packs"].ToString().Trim());
                                            impdelitems.ImpNoOfPieces = OrdItemline["Number of Pieces"].ToString().Trim() == "" ? 0 : Convert.ToDecimal(OrdItemline["Number of Pieces"].ToString().Trim());
                                            //impdelitems.ImpNoOfPieces = Convert.ToDecimal(OrdItemline["Number of Pieces"].ToString().Trim());

                                            impdelitems.ImpUOM = OrdItemline["UOM"].ToString().Trim();
                                            impdelitems.ImpIssueType = OrdItemline["Issue Type"].ToString().Trim();
                                            impdelitems.ImpRemarks = OrdItemline["Remarks"].ToString().Trim();
                                            //  impdelitems.ImpSubsStatus = OrdItemline["Line Status if Substitution"].ToString().Trim();
                                            string ImpDeliveryDate = OrdItemline["Recd Date @ Contingent"].ToString().Trim();
                                            impdelitems.ImpExpDeliveryQty = OrdItemline["Delivered Qty"].ToString().Trim() == "" ? 0 : Convert.ToDecimal(OrdItemline["Delivered Qty"].ToString().Trim());
                                            if (!string.IsNullOrEmpty(ImpDeliveryDate))
                                                // impdelitems.ImpDeliveryDate = DateTime.Parse(ImpDeliveryDate, provider, System.Globalization.DateTimeStyles.NoCurrentDateDefault);
                                                impdelitems.ImpDeliveryDate = Convert.ToDateTime(OrdItemline["Recd Date @ Contingent"].ToString().Trim());
                                            impdelitems.CreatedBy = uploadlog.CreatedBy;
                                            impdelitems.CreatedDate = DateTime.Now;
                                            impdelitems.IsValid = "VALID";
                                            // if (AlreadyExistFile == 0)
                                            if (AlreadyExistFlag != "true")
                                            {
                                                importeddeliverynoteitems.Add(impdelitems);
                                                //ordser.SaveOrUpdateImportedDeliveryNoteItems(impdelitems);
                                                // UpdateImportedDeliveryNoteItemDetailsinOrdItemsTbl(impdelitems);

                                            }
                                        }

                                    }
                                    if (importeddeliverynoteitems.Count != 0)
                                    {
                                        try
                                        {
                                            orderService.SaveorUpdateDeliveryNoteInSingleSession(impdel, importeddeliverynoteitems);
                                            //orderService.SaveOrUpdateImportedDeliveryNoteItemsList(importeddeliverynoteitems);
                                            criteria.Clear();
                                            criteria.Add("ImpDeliveryNoteId", importeddeliverynoteitems[0].ImpDeliveryNoteId);
                                            Dictionary<long, IList<ImportedDeliveryNoteItems>> impdelnoteitemslist = orderService.GetImportedDeliveryNoteItemsListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                                            IList<ImportedDeliveryNoteItems> delnoteitems = impdelnoteitemslist.FirstOrDefault().Value.ToList();


                                            // checking the Unordered delivery and substitution master
                                            ValidatingAuthorizedSubstitutionAndDelivery(impdel, importeddeliverynoteitems);
                                            orderService.CallDeliveryNoteUpload_SP(impdel.ImpDeliveryNoteId);

                                            //UpdateImportedDeliveryNoteItemDetailsinOrdItemsTbl(importeddeliverynoteitems, UploadedFile);
                                            criteria.Clear();
                                            //UploadedFilename.Append(fileName + ",");
                                            if (uploadlog.UploadStatus == "YetToUpload")
                                                uploadlog.UploadStatus = "UploadedSuccessfully";
                                            uploadlog.ReferenceNo = "DEL-" + importeddeliverynoteitems.FirstOrDefault().ImpDeliveryNoteId;
                                            orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);
                                        }
                                        catch (Exception ex)
                                        {

                                            if (uploadlog.UploadStatus == "YetToUpload")
                                                uploadlog.UploadStatus = "UploadFailed";
                                            uploadlog.ErrDesc = "Orders upload transaction failiure----" + ex.ToString();
                                            orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);
                                        }
                                    }

                                }
                                else
                                {
                                    if (uploadlog.UploadStatus == "YetToUpload")
                                        uploadlog.UploadStatus = "UploadFailed";
                                    uploadlog.ErrDesc = "Headers error ---The no of columns will be leaser or spelling change in headers";
                                    orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);
                                    //ErrorFilename.Append(fileName + ",");
                                }
                            }
                            else
                            {
                                if (uploadlog.UploadStatus == "YetToUpload")
                                    uploadlog.UploadStatus = "UploadFailed";
                                uploadlog.ErrDesc = "Headers error  or transaction errors";
                                orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);

                                // ErrorFilename.Append(fileName + ",");
                            }
                        }
                        else
                        {
                            if (uploadlog.UploadStatus == "YetToUpload")
                                uploadlog.UploadStatus = "UploadFailed";
                            uploadlog.ErrDesc = "Order not found";
                            orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);

                        }
                    }
                    //   success = success + 1;
                }
                criteria.Clear();
                StatusUpdateInUploadRequest(RequestId);

            }
            catch (Exception ex)
            {

                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
            }

        }

        public ActionResult ValidatingAuthorizedSubstitutionAndDelivery(ImportedDeliveryNote impdel, IList<ImportedDeliveryNoteItems> importeddeliverynoteitems)
        {
            //Validating the Unordered Delivery
            Dictionary<string, object> criteria = new Dictionary<string, object>();
            var impitems = (from u in importeddeliverynoteitems select u.ImpUNCode).Distinct().ToArray();
            criteria.Add("OrderId", impdel.OrderId);
            Dictionary<long, IList<OrderItems>> orditems = orderService.GetOrderItemsListWithPagingAndCriteria(0, 99999, string.Empty, string.Empty, criteria);
            criteria.Clear();
            var orderitems = (from u in orditems.FirstOrDefault().Value select u.UNCode).Distinct().ToList();
            var result = impitems.Except(orderitems).ToArray();
            if (result.Length > 0)
            {
                criteria.Add("ImpUNCode", result);
                criteria.Add("ImpDeliveryNoteId", impdel.ImpDeliveryNoteId);
                //Getting list of unordered items
                Dictionary<long, IList<ImportedDeliveryNoteItems>> impdelnoteitemslist = orderService.GetImportedDeliveryNoteItemsListWithPagingAndCriteria(0, 99999, string.Empty, string.Empty, criteria);
                criteria.Clear();
                //saving the un ordered items
                SaveUnOrderItemsInInsightReportTbl(impdelnoteitemslist.FirstOrDefault().Value);
            }



            //Checking the substitution
            ValidatingSubsitutionsandReplacement(importeddeliverynoteitems);

            return null;
        }

        //Saving the un ordered items list in insight table while delivery note upload
        public void SaveUnOrderItemsInInsightReportTbl(IList<ImportedDeliveryNoteItems> impdelnoteitemslist)
        {

            foreach (ImportedDeliveryNoteItems imp in impdelnoteitemslist)
            {
                imp.IsValid = "UNORDERED";
            }
            orderService.SaveOrUpdateImportedDeliveryNoteItemsList(impdelnoteitemslist);



            //InsightReport insreport = new InsightReport();
            //insreport.OrderId = item.OrderId;
            //insreport.ControlId = item.ImpControlId;
            //insreport.DeliveryNoteName = item.ImpDeliveryNoteName;
            //insreport.UNCode = item.ImpUNCode;
            //insreport.Commodity = item.ImpCommodity;
            //insreport.SubsCode = item.ImpSubsItemCode;
            //insreport.SubsName = item.ImpSubsItemName;
            //insreport.ReportCode = "UnOrderedDelivery";
            //insreport.Description = "The delivered has been done for the un ordered items";
            //insreport.DeliveredQty = item.ImpDeliveryQty;
            //insreport.CreatedBy = item.CreatedBy;
            //insreport.CreatedDate = DateTime.Now;
            //orderService.SaveOrUpdateInsightReport(insreport);

            // }

        }

        //Validating the substitution master and insert in insight report table while delivery note upload
        public void ValidatingSubsitutionsandReplacement(IList<ImportedDeliveryNoteItems> importeddeliverynoteitems)
        {


            //Checking with substitution master
            Dictionary<string, object> likeSearchCriteria = new Dictionary<string, object>();
            criteria.Add("ControlId", importeddeliverynoteitems[0].ImpControlId);
            Dictionary<long, IList<SubstitutionMaster>> subsmstlist = orderService.GetSubstitutionMasterListWithPagingAndCriteria(0, 99999, string.Empty, string.Empty, criteria, likeSearchCriteria);
            criteria.Clear();
            //string [] status= {"Substitution","Replacement"};
            IList<SubstitutionMaster> subsmstdetlist = subsmstlist.FirstOrDefault().Value;
            //IList<ImportedDeliveryNoteItems> impitems = (from u in importeddeliverynoteitems
            //                                             where status.Contains(u.ImpSubsStatus)
            //                                             select u).ToList();

            //Individual substitution
            IList<ImportedDeliveryNoteItems> sublist = (from u in importeddeliverynoteitems where u.ImpSubsStatus.ToUpper() == "SUBSTITUTION" && u.IsValid != "UNORDERED" select u).ToList();

            foreach (var item in sublist)
            {
                var IndiSubsList = (from u in subsmstdetlist where u.SubsOrReplace.ToUpper() == "SUBSTITUTION" && u.Category.ToUpper() == "INDIVIDUAL" && u.ControlId == item.ImpControlId && item.ImpSubsStatus.ToUpper() == "SUBSTITUTION" select new { u.UNCode, u.ItemName, u.OrderedQty, u.SubstituteItemCode, u.SubstituteItemName, u.AcceptedQty, u.Sector }).ToArray();
                if (IndiSubsList.Length <= 0)
                {
                    item.IsValid = "SUBSTITUTIONMISMATCH";
                    orderService.SaveOrUpdateImportedDeliveryNoteItems(item);
                }
                else
                {
                    for (int i = 0; i < IndiSubsList.Length; i++)
                    {
                        item.IsValid = "SUBSTITUTIONMISMATCH";
                        orderService.SaveOrUpdateImportedDeliveryNoteItems(item);
                        if (IndiSubsList[i].UNCode == item.ImpUNCode && IndiSubsList[i].SubstituteItemCode == item.ImpSubsItemCode)
                        {
                            item.IsValid = "VALID";
                            orderService.SaveOrUpdateImportedDeliveryNoteItems(item);
                            break;

                        }
                    }
                }
            }
            IList<ImportedDeliveryNoteItems> replist = (from u in importeddeliverynoteitems where u.ImpSubsStatus.ToUpper() == "REPLACEMENT" select u).ToList();
            //Checking the replacement
            //Individual Replacement

            foreach (var item in replist)
            {
                var IndiRepList = (from u in subsmstdetlist where u.SubsOrReplace.ToUpper() == "REPLACEMENT" && u.Category.ToUpper() == "INDIVIDUAL" && u.ControlId == item.ImpControlId && item.ImpSubsStatus.ToUpper() == "REPLACEMENT" select new { u.UNCode, u.ItemName, u.OrderedQty, u.SubstituteItemCode, u.SubstituteItemName, u.AcceptedQty, u.Sector }).ToArray();
                if (IndiRepList.Length <= 0)
                {
                    item.IsValid = "SUBSTITUTIONMISMATCH";
                    orderService.SaveOrUpdateImportedDeliveryNoteItems(item);
                }
                else
                {
                    for (int i = 0; i < IndiRepList.Length; i++)
                    {
                        item.IsValid = "SUBSTITUTIONMISMATCH";
                        orderService.SaveOrUpdateImportedDeliveryNoteItems(item);
                        if (IndiRepList[i].UNCode == item.ImpUNCode && IndiRepList[i].SubstituteItemCode == item.ImpSubsItemCode)
                        {
                            item.IsValid = "VALID";
                            orderService.SaveOrUpdateImportedDeliveryNoteItems(item);
                            break;
                        }
                    }
                }
            }
        }



        #endregion

        #region Substitution master related code
        public ActionResult SubstitutionMasterUpload()
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

        [HttpPost]
        public ActionResult SubstitutionMasterUpload(HttpPostedFileBase uploadedFile, string Period, string PeriodYear, string RequestName)
        {

            //Creating the upload request for the substitution master upload
            UploadRequest upreq = new UploadRequest();
            INSIGHT.Entities.User Userobj = (INSIGHT.Entities.User)Session["objUser"];
            if (Userobj == null || (Userobj != null && Userobj.UserId == null))
            { return RedirectToAction("LogOn", "Account"); }
            string loggedInUserId = Userobj.UserId;
            upreq.Category = "SUBSTITUTIONMASTERUPLOAD";
            upreq.CreatedBy = loggedInUserId;
            upreq.CreatedDate = DateTime.Now;
            upreq.RequestName = RequestName;
            upreq.Period = Period;
            upreq.PeriodYear = PeriodYear;
            long reqid = orderService.SaveOrUpdateUploadRequest(upreq);
            upreq.RequestNo = "REQUEST-" + reqid;
            // upreq.Status = "OPEN";
            //  upreq.UploadStatus = "Request Generated";
            upreq.UploadStatus = "InProgress";
            orderService.SaveOrUpdateUploadRequest(upreq);
            try
            {

                HttpPostedFileBase theFile = HttpContext.Request.Files["uploadedFile"];
                StringBuilder alreadyExists = new StringBuilder();
                StringBuilder ErrorFilename = new StringBuilder();
                StringBuilder UploadedFilename = new StringBuilder();
                if (theFile != null && theFile.ContentLength > 0)
                {
                    string fileName = string.Empty;
                    string path = uploadedFile.InputStream.ToString();
                    byte[] imageSize = new byte[uploadedFile.ContentLength];
                    uploadedFile.InputStream.Read(imageSize, 0, (int)uploadedFile.ContentLength);
                    string UploadConnStr = "";
                    fileName = uploadedFile.FileName;
                    string fileExtn = Path.GetExtension(uploadedFile.FileName);
                    string fileLocation = ConfigurationManager.AppSettings["SubstitutionMasterFilePath"].ToString() + DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss").Replace(":", ".") + fileName;
                    uploadedFile.SaveAs(fileLocation);
                    if (fileExtn == ".xls")
                    {
                        UploadConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\"";
                    }
                    if (fileExtn == ".xlsx")
                    {
                        UploadConnStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";
                    }

                    //Individual substitution related code

                    DataTable DtblXcelData = new DataTable();
                    OleDbConnection conn = new OleDbConnection();
                    OleDbDataAdapter DtAdptrr = new OleDbDataAdapter();
                    string QeryToGetXcelData = "select * from " + string.Format("{0}${1}", "[Individual Substitutions", "A6:AZ]");
                    OleDbCommand cmd = new OleDbCommand(QeryToGetXcelData, conn);
                    conn.ConnectionString = UploadConnStr;
                    conn.Open();
                    cmd = new OleDbCommand(QeryToGetXcelData, conn);
                    cmd.CommandType = CommandType.Text;
                    DtAdptrr = new OleDbDataAdapter();
                    DtAdptrr.SelectCommand = cmd;
                    DtAdptrr.Fill(DtblXcelData);
                    string[] strArray = { "S#No", "Control #", "Item code", "Item Descreption", "Ordered Qty", "Substitute Item code", "Substitute Item Descreption", "Accepted Qty", "Substitution/Replacement" };
                    char chrFlag = 'Y';
                    if (DtblXcelData.Columns.Count == strArray.Length)
                    {
                        int j = 0;
                        string[] strColumnsAray = new string[DtblXcelData.Columns.Count];
                        foreach (DataColumn dtColumn in DtblXcelData.Columns)
                        {
                            strColumnsAray[j] = dtColumn.ColumnName;
                            j++;
                        }
                        for (int i = 0; i < strArray.Length - 1; i++)
                        {
                            if (strArray[i].Trim() != strColumnsAray[i].Trim())
                            {
                                chrFlag = 'N';
                                break;
                            }
                        }
                        if (chrFlag == 'Y')
                        {


                            IList<SubstitutionMaster> submstlist = new List<SubstitutionMaster>();

                            foreach (DataRow indisubmstline in DtblXcelData.Rows)
                            {

                                if (indisubmstline["Control #"].ToString() != "")
                                {


                                    SubstitutionMaster subsmst = new SubstitutionMaster();

                                    string controlId = indisubmstline["Control #"].ToString();
                                    string[] controlIdArray = controlId.Split('-');

                                    subsmst.ControlId = controlId;
                                    subsmst.Sector = controlIdArray[2].ToString();
                                    subsmst.Name = controlIdArray[3].ToString();
                                    subsmst.Period = controlIdArray[5].ToString();
                                    subsmst.PeriodYear = controlIdArray[7].ToString();
                                    subsmst.Location = controlIdArray[4].ToString();
                                    //subsmst.Sector = controlIdArray[1].ToString();
                                    //subsmst.Name = controlIdArray[3].ToString();
                                    //subsmst.Period = controlIdArray[4].ToString();
                                    //subsmst.PeriodYear = controlIdArray[6].ToString() + "-" + controlIdArray[7].ToString();
                                    //subsmst.Location = controlIdArray[1].ToString() + "-" + controlIdArray[2].ToString();
                                    subsmst.UNCode = indisubmstline["Item code"].ToString().Trim() == "" ? 0 : Convert.ToInt64(indisubmstline["Item code"].ToString().Trim());
                                    subsmst.ItemName = indisubmstline["Item Descreption"].ToString().Trim();
                                    subsmst.OrderedQty = indisubmstline["Ordered Qty"].ToString().Trim() == "" ? 0 : Convert.ToDecimal(indisubmstline["Ordered Qty"].ToString().Trim());
                                    subsmst.SubstituteItemCode = indisubmstline["Substitute Item code"].ToString().Trim() == "" ? 0 : Convert.ToInt64(indisubmstline["Substitute Item code"].ToString().Trim());
                                    subsmst.SubstituteItemName = indisubmstline["Substitute Item Descreption"].ToString().Trim();
                                    subsmst.AcceptedQty = indisubmstline["Accepted Qty"].ToString().Trim() == "" ? 0 : Convert.ToDecimal(indisubmstline["Accepted Qty"].ToString().Trim());
                                    subsmst.CreatedBy = loggedInUserId;
                                    subsmst.CreatedDate = DateTime.Now;
                                    subsmst.Category = "Individual";
                                    subsmst.RequestId = upreq.RequestId;
                                    Orders ord = orderService.GetOrderByControlId(subsmst.ControlId);
                                    if (ord != null)
                                    {
                                        subsmst.OrderId = ord.OrderId;
                                    }
                                    subsmst.SubsOrReplace = indisubmstline["Substitution/Replacement"].ToString().Trim();

                                    submstlist.Add(subsmst);
                                }
                            }

                            //AddGeneralSibstitutionsinSubstitutionMaster(submstlist);
                            //General substitions related code
                            DataTable DtblXcelData1 = new DataTable();
                            string QeryToGetXcelData1 = "select * from " + string.Format("{0}${1}", "[General Substitutions", "A6:AZ]");
                            cmd = new OleDbCommand(QeryToGetXcelData1, conn);
                            //  conn.ConnectionString = UploadConnStr;
                            // conn.Open();
                            cmd = new OleDbCommand(QeryToGetXcelData1, conn);
                            cmd.CommandType = CommandType.Text;
                            DtAdptrr = new OleDbDataAdapter();
                            DtAdptrr.SelectCommand = cmd;
                            DtAdptrr.Fill(DtblXcelData1);
                            string[] strArray1 = { "S#No", "Control #", "Item code", "Item Descreption", "Ordered Qty", "Substitute Item code", "Substitute Item Descreption", "Accepted Qty", "Sector", "Substitution/Replacement" };
                            char chrFlag1 = 'Y';
                            if (DtblXcelData1.Columns.Count == strArray1.Length)
                            {
                                j = 0;
                                string[] strColumnsAray1 = new string[DtblXcelData1.Columns.Count];
                                foreach (DataColumn dtColumn in DtblXcelData1.Columns)
                                {
                                    strColumnsAray1[j] = dtColumn.ColumnName;
                                    j++;
                                }
                                for (int i = 0; i < strArray1.Length - 1; i++)
                                {
                                    if (strArray1[i].Trim() != strColumnsAray1[i].Trim())
                                    {
                                        chrFlag1 = 'N';
                                        break;
                                    }
                                }
                                if (chrFlag1 == 'Y')
                                {


                                    IList<SubstitutionMaster> Gensubsmstlist = new List<SubstitutionMaster>();

                                    foreach (DataRow indisubmstline1 in DtblXcelData1.Rows)
                                    {

                                        if (indisubmstline1["Control #"].ToString() != "")
                                        {


                                            SubstitutionMaster subsmst = new SubstitutionMaster();

                                            string controlId = indisubmstline1["Control #"].ToString();
                                            string[] controlIdArray = controlId.Split('-');

                                            subsmst.ControlId = controlId;
                                            //subsmst.Sector = controlIdArray[1].ToString();
                                            //subsmst.Name = controlIdArray[3].ToString();
                                            //subsmst.Period = controlIdArray[4].ToString();
                                            //subsmst.PeriodYear = controlIdArray[6].ToString() + "-" + controlIdArray[7].ToString();
                                            //subsmst.Location = controlIdArray[1].ToString() + "-" + controlIdArray[2].ToString();
                                            subsmst.UNCode = indisubmstline1["Item code"].ToString().Trim() == "" ? 0 : Convert.ToInt64(indisubmstline1["Item code"].ToString().Trim());
                                            subsmst.ItemName = indisubmstline1["Item Descreption"].ToString().Trim();
                                            subsmst.OrderedQty = indisubmstline1["Ordered Qty"].ToString().Trim() == "" ? 0 : Convert.ToDecimal(indisubmstline1["Ordered Qty"].ToString().Trim());
                                            subsmst.SubstituteItemCode = indisubmstline1["Substitute Item code"].ToString().Trim() == "" ? 0 : Convert.ToInt64(indisubmstline1["Substitute Item code"].ToString().Trim());
                                            subsmst.SubstituteItemName = indisubmstline1["Substitute Item Descreption"].ToString().Trim();
                                            subsmst.AcceptedQty = indisubmstline1["Accepted Qty"].ToString().Trim() == "" ? 0 : Convert.ToDecimal(indisubmstline1["Accepted Qty"].ToString().Trim());
                                            subsmst.CreatedBy = loggedInUserId;
                                            subsmst.CreatedDate = DateTime.Now;
                                            subsmst.Category = "General";
                                            subsmst.Period = Period;
                                            subsmst.PeriodYear = PeriodYear;
                                            subsmst.RequestId = upreq.RequestId;

                                            //Orders ord = orderService.GetOrderByControlId(subsmst.ControlId);
                                            //if (ord != null)
                                            //{
                                            //    subsmst.OrderId = ord.OrderId;
                                            //}
                                            subsmst.SubsOrReplace = indisubmstline1["Substitution/Replacement"].ToString().Trim();
                                            subsmst.Sector = indisubmstline1["Sector"].ToString().Trim();
                                            Gensubsmstlist.Add(subsmst);
                                        }
                                    }


                                    var SubstitutionMstList = submstlist.Union(Gensubsmstlist).ToList();

                                    orderService.SaveOrUpdateSubstitutionMaster(SubstitutionMstList);
                                    upreq.UploadStatus = "Uploaded Successfully";
                                    orderService.SaveOrUpdateUploadRequest(upreq);


                                }

                                //else
                                //{
                                //    return Json(new { success = false, result = "Headers mismatch!!!" }, "text/html", JsonRequestBehavior.AllowGet);
                                //}
                            }
                            //else
                            //{
                            //    return Json(new { success = false, result = "Headers mismatch!!!" }, "text/html", JsonRequestBehavior.AllowGet);
                            //}



                        }
                        //else
                        //{
                        //    return Json(new { success = false, result = "You have uploded the empty file. Please upload the correct file." }, "text/html", JsonRequestBehavior.AllowGet);
                        //}
                        //MyLabel.Text = sb.ToString().Replace(Environment.NewLine, "<br />");
                        //return Json(new { success = true, result = "Substitution Master updated successfully." }, "text/html", JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = true, result = "Substitution Master updated successfully." }, "text/html", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                upreq.ErrorDesc = ex.ToString();
                upreq.UploadStatus = "Upload Failed";
                orderService.SaveOrUpdateUploadRequest(upreq);
                return Json(new { success = true, result = "Substitution Master updated Failed." }, "text/html", JsonRequestBehavior.AllowGet);
                // throw ex;

            }

        }

        //Substitution master jqgrid
        public JsonResult SubstitutionMasterJqGrid(string searchItems, long RequestId, string ControlId, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                Dictionary<string, object> likeSearchCriteria = new Dictionary<string, object>();

                //sord = sord == "desc" ? "Desc" : "Asc";
                sord = sord == "desc" ? "Desc" : "Asc";

                criteria.Add("RequestId", RequestId);
                Dictionary<long, IList<SubstitutionMaster>> subsmstlist = null;
                if (!string.IsNullOrWhiteSpace(ControlId))
                {
                    criteria.Add("ControlId", ControlId);
                    subsmstlist = orderService.GetSubstitutionMasterListWithLikeSearchPagingAndCriteria(page - 1, 9999, sidx, sord, criteria);
                }
                else
                {
                    //if (searchItems != null && searchItems != "")
                    //{
                    //    var Items = searchItems.ToString().Split(',');
                    //    if (!string.IsNullOrWhiteSpace(Items[0])) { criteria.Add("Sector", Items[0]); }


                    //    if (!string.IsNullOrWhiteSpace(Items[1])) { criteria.Add("Name", Items[1]); }
                    //    if (!string.IsNullOrWhiteSpace(Items[2])) { criteria.Add("Location", Items[2]); }
                    //    if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Period", Items[3]); }
                    //    if (!string.IsNullOrWhiteSpace(Items[4])) { criteria.Add("PeriodYear", Items[4]); }
                    //}

                    subsmstlist = orderService.GetSubstitutionMasterListWithPagingAndCriteria(page - 1, 9999, sidx, sord, criteria, likeSearchCriteria);
                }
                if (subsmstlist != null && subsmstlist.Count > 0)
                {
                    long totalrecords = subsmstlist.First().Key;
                    int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                    var jsondat = new
                    {
                        total = totalPages,
                        page = page,
                        records = totalrecords,

                        rows = (from items in subsmstlist.First().Value
                                select new
                                {
                                    i = 2,
                                    cell = new string[] {
                                        items.SubstitutionMstId.ToString(),
                                        items.ControlId,
                                       items.UNCode.ToString(),
                                        items.ItemName,
                                        items.OrderedQty==0?"":items.OrderedQty.ToString(),
                                        items.SubstituteItemCode.ToString(),
                                        items.SubstituteItemName,
                                        items.AcceptedQty==0?"":items.AcceptedQty.ToString(),
                                        items.SubsOrReplace.ToString().ToUpper()=="SUBSTITUTION"?"AS": items.SubsOrReplace.ToString().ToUpper()=="REPLACEMENT"?"AR": items.SubsOrReplace.ToString(),
                                        items.Sector,
                                        items.Name,
                                        items.Location,
                                        items.Period,
                                        items.PeriodYear,
                                        items.ContingentType,
                                        items.Category,
                                        items.OrderId.ToString(),
                                        items.CreatedBy,
                                        items.CreatedDate.ToString(),
                                        items.ModifiedBy,
                                        items.ModifiedDate.ToString()
                      
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
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }


        //Substitution master upload request jqgrid

        public JsonResult SubstitutionMasterUploadRequestJqGrid(string Category, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                Dictionary<string, object> criteria = new Dictionary<string, object>();

                //sord = sord == "desc" ? "Desc" : "Asc";
                sord = sord == "desc" ? "Desc" : "Asc";
                if (!string.IsNullOrWhiteSpace(Category)) { criteria.Add("Category", Category); }
                //  criteria.Add("Category", "SUBSTITUTIONMASTERUPLOAD");

                Dictionary<long, IList<UploadRequest>> uploadreqlist = orderService.GetUploadRequestCountListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                if (uploadreqlist != null && uploadreqlist.Count > 0)
                {
                    long totalrecords = uploadreqlist.First().Key;
                    int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                    var jsondat = new
                    {
                        total = totalPages,
                        page = page,
                        records = totalrecords,

                        rows = (from items in uploadreqlist.First().Value
                                select new
                                {
                                    i = 2,
                                    cell = new string[] {
                                        items.RequestId.ToString(),
                                        items.RequestName,
                                        items.Period,
                                        items.PeriodYear,
                                        items.RequestNo,
                                        items.Category,
                                        items.Status,
                                        items.CreatedBy,
                                        items.CreatedDate!=null? ConvertDateTimeToDate(items.CreatedDate.ToString("dd/MM/yyyy"),"en-GB"):"",
                                        items.UploadStatus
                                                             
                            }
                                })
                    };
                    return Json(jsondat, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(null, JsonRequestBehavior.AllowGet);

                }


            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }

        //Deleting the substitution master by requestid
        public void DeleteSubstitutionMaster(string RequestIds)
        {
            try
            {

                var RequestIdArray = RequestIds.Split(',');
                if (RequestIds != null && RequestIdArray.Length > 0)
                    orderService.DeleteSubstitutionMasterByRequestId(RequestIds);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }

            //for (int i = 0; i < RequestIdArray.Length; i++)
            //{
            //    OrdersService.DeleteSubstitutionMasterByRequestId(Convert.ToInt64(RequestIdArray[i]));


            //}


        }

        //Substitution master form page
        public ActionResult SubstitutionMasterDetails(long RequestId)
        {
            UploadRequest upreq = orderService.GetUploadRequestbyRequestId(RequestId);
            return View(upreq);
        }
        #endregion

        /// <summary>
        /// To Download Zip for both Excel and PDF
        /// The Buttons are Provided in Each Pager of PDF and EXCEL Documents Grid
        /// </summary>
        /// <param name="searchItems"></param>
        /// <param name="PDF"></param>
        /// <param name="EXCEL"></param>
        /// <returns>zip files of PDF and Excel</returns>
        /// 

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
                        if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Period", Items[3]); }
                        if (!string.IsNullOrWhiteSpace(Items[4])) { criteria.Add("PeriodYear", Items[4]); }
                        var TrimItems = searchItems.Replace(",", "");
                        if (string.IsNullOrWhiteSpace(TrimItems))
                            return null;
                    }
                    IList<Orders> Orderlist = null;
                    IList<DeliveryNoteOrders_vw> Deliverylist = null;

                    Dictionary<long, IList<Orders>> Orders = orderService.GetOrdersListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                    Dictionary<long, IList<DeliveryNoteOrders_vw>> delnote = orderService.GetDeliveryNoteOrderListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                    if (Orders != null && Orders.Count > 0)
                    {
                        Orderlist = Orders.FirstOrDefault().Value;
                    }
                    if (delnote != null && delnote.Count > 0)
                    {
                        Deliverylist = delnote.FirstOrDefault().Value;
                    }

                    //Create a Output Memory stream
                    MemoryStream outputMemStream = new MemoryStream();
                    //Create a Output Zipfile stream
                    ZipOutputStream zipStream = new ZipOutputStream(outputMemStream);

                    zipStream.SetLevel(3); //0-9, 9 being the highest level of compression

                    if (EXCEL == true && invType == "Orders")
                    {   //For Excel files
                        foreach (var item in Orderlist)
                        {
                            //Assigning Byte[] to a new stream 
                            MemoryStream stream = new MemoryStream(item.DocumentData);
                            ZipEntry xmlEntry = new ZipEntry("Con-" + item.ControlId + ".xlsx");
                            xmlEntry.DateTime = DateTime.Now;
                            zipStream.PutNextEntry(xmlEntry);
                            StreamUtils.Copy(stream, zipStream, new byte[4096]);
                            zipStream.CloseEntry(); //Close each Zip stream
                        }
                    }
                    if (EXCEL == true && invType == "DeliveryNotes")
                    {   //For Excel files
                        foreach (var item in Deliverylist)
                        {
                            if (item.DocumentData != null)
                            {
                                //Assigning Byte[] to a new stream 
                                var Delnote = item.DeliveryNoteName.ToString().Split('-');
                                var Contorl = item.ControlId.ToString().Split('-');

                                MemoryStream stream = new MemoryStream(item.DocumentData);
                                ZipEntry xmlEntry = new ZipEntry(Delnote[1] + "-" + item.Location + "-" + item.Name + "-" + item.Period + "-" + Contorl[5] + "-" + item.PeriodYear + ".xlsx");
                                xmlEntry.DateTime = DateTime.Now;
                                zipStream.PutNextEntry(xmlEntry);
                                StreamUtils.Copy(stream, zipStream, new byte[4096]);
                                zipStream.CloseEntry(); //Close each Zip stream
                            }
                        }
                    }
                    zipStream.IsStreamOwner = false;
                    zipStream.Close();

                    outputMemStream.Position = 0;

                    byte[] byteArray = outputMemStream.ToArray();

                    Response.Clear();
                    Response.AppendHeader("Content-Disposition", "attachment; filename=" + "GCC-" + invType + "-" + Items[3] + "-" + Items[4] + ".zip");
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

        #region GCCRevisedDNUpload
        public ActionResult GCCRevisedDNUpload()
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
        //[HttpPost]
        //public ActionResult GCCRevisedDNUpload(HttpPostedFileBase uploadedFile, string Period, string PeriodYear, string RequestName, long Week)
        //{
        //    new Task(() => {
        //        UploadGCCRevisedFiles(uploadedFile, Period, PeriodYear, RequestName, Week);
        //    }).Start();
        //    //Creating the upload request for the DN Data upload
        //    UploadRequest upreq = new UploadRequest();
        //    INSIGHT.Entities.User Userobj = (INSIGHT.Entities.User)Session["objUser"];
        //    if (Userobj == null || (Userobj != null && Userobj.UserId == null))
        //    { return RedirectToAction("LogOn", "Account"); }
        //    string loggedInUserId = Userobj.UserId;
        //    upreq.Category = "DNDataFormatUpload";
        //    upreq.CreatedBy = loggedInUserId;
        //    upreq.CreatedDate = DateTime.Now;
        //    upreq.RequestName = RequestName;
        //    upreq.Period = Period;
        //    upreq.PeriodYear = PeriodYear;

        //    upreq.Week = Week;

        //    long reqid = orderService.SaveOrUpdateUploadRequest(upreq);
        //    upreq.RequestNo = "REQUEST-" + reqid;

        //    upreq.UploadStatus = "InProgress";
        //    orderService.SaveOrUpdateUploadRequest(upreq);
        //    try
        //    {
        //        HttpPostedFileBase theFile = HttpContext.Request.Files["uploadedFile"];
        //        StringBuilder alreadyExists = new StringBuilder();
        //        StringBuilder ErrorFilename = new StringBuilder();
        //        StringBuilder UploadedFilename = new StringBuilder();
        //        if (theFile != null && theFile.ContentLength > 0)
        //        {
        //            string fileName = string.Empty;
        //            string path = uploadedFile.InputStream.ToString();
        //            byte[] imageSize = new byte[uploadedFile.ContentLength];
        //            uploadedFile.InputStream.Read(imageSize, 0, (int)uploadedFile.ContentLength);
        //            string UploadConnStr = "";
        //            fileName = uploadedFile.FileName;
        //            string fileExtn = Path.GetExtension(uploadedFile.FileName);
        //            string fileLocation = ConfigurationManager.AppSettings["GccRevisedFilePath"].ToString() + DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss").Replace(":", ".") + fileName;
        //            uploadedFile.SaveAs(fileLocation);
        //            if (fileExtn == ".xls")
        //            {
        //                UploadConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\"";
        //            }
        //            if (fileExtn == ".xlsx")
        //            {
        //                UploadConnStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";
        //            }
        //            DataTable DtblXcelData = new DataTable();
        //            OleDbConnection conn = new OleDbConnection();
        //            OleDbDataAdapter DtAdptrr = new OleDbDataAdapter();
        //            string QeryToGetXcelData = "select * from " + string.Format("{0}${1}", "[DN Data Template", "A1:AZ]");
        //            OleDbCommand cmd = new OleDbCommand(QeryToGetXcelData, conn);
        //            conn.ConnectionString = UploadConnStr;
        //            conn.Open();
        //            cmd = new OleDbCommand(QeryToGetXcelData, conn);
        //            cmd.CommandType = CommandType.Text;
        //            DtAdptrr = new OleDbDataAdapter();
        //            DtAdptrr.SelectCommand = cmd;
        //            DtAdptrr.Fill(DtblXcelData);
        //            string[] strArray = { "Line#", "Item", "Description", "Ordered Qty", "Delivered Qty", "Number of Packs", "Number of Pieces", "Substitute For", "Substitute Name", "UOM", "Issue Type", "Remarks", "Line Status if Substitution", "Actual Received Qty#", "Recd Date @ Contingent", "Unit Control No:", "DN Number", "DN Type", "Delivery Mode", "Approved Delivery Date", "Shipment Date", "Actual Warehouse Shipped Out Date", "Period", "UN Week", "Consumption Week", "Delivery Week", "Request#", "Warehouse", "Strength", "DOS", "Man days", "Storer Key" };
        //            char chrFlag = 'Y';
        //            if (DtblXcelData.Columns.Count == strArray.Length)
        //            {
        //                int j = 0;
        //                string[] strColumnsAray = new string[DtblXcelData.Columns.Count];
        //                foreach (DataColumn dtColumn in DtblXcelData.Columns)
        //                {
        //                    strColumnsAray[j] = dtColumn.ColumnName;
        //                    j++;
        //                }
        //                for (int i = 0; i < strArray.Length - 1; i++)
        //                {
        //                    if (strArray[i].Trim() != strColumnsAray[i].Trim())
        //                    {
        //                        chrFlag = 'N';
        //                        break;
        //                    }
        //                }
        //                if (chrFlag == 'Y')
        //                {
        //                    criteria.Clear();
        //                    criteria.Add("Period", Period);
        //                    criteria.Add("PeriodYear", PeriodYear);
        //                    criteria.Add("Week", Convert.ToInt64(Week));
        //                    Dictionary<long, IList<GCCRevised>> GccRevisedPeriodList = orderService.GetGccRevisedListWithPagingAndCriteria(0, 999999, string.Empty, string.Empty, criteria);
        //                    if (GccRevisedPeriodList.FirstOrDefault().Key == 0)
        //                    {
        //                        #region Read Excel
        //                        IList<GCCRevised> Itemlist = new List<GCCRevised>();

        //                        foreach (DataRow Item in DtblXcelData.Rows)
        //                        {
        //                            if (Item["Item"].ToString() != "")
        //                            {
        //                                GCCRevised DNObj = new GCCRevised();
        //                                string Sector = "";
        //                                string Name = "";
        //                                string Location = "";

        //                                string controlId = Item["Unit Control No:"].ToString();
        //                                string[] controlIdArray = controlId.Split('-');
        //                                Sector = controlIdArray[1];
        //                                Name = controlIdArray[3];
        //                                Location = controlIdArray[1] + "-" + controlIdArray[3];

        //                                if (Item["Line Status if Substitution"].ToString() == "" || Item["Line Status if Substitution"].ToString() == "None")
        //                                {
        //                                    DNObj.UNCode = Item["Item"].ToString() != "" ? Convert.ToInt64(Item["Item"].ToString()) : 0;
        //                                    DNObj.Commodity = Item["Description"].ToString();
        //                                    //DNObj.SubstituteItemCode = Item["Substitute For"].ToString() != "" ? Convert.ToInt64(Item["Substitute For"].ToString()) : 0;//Substitute For
        //                                    if (Item["Substitute For"].ToString().Trim() != "" || Item["Substitute For"].ToString().Trim() != "{}")
        //                                        DNObj.SubstituteItemCode = 0;
        //                                    else
        //                                        DNObj.SubstituteItemCode = Convert.ToInt64(Item["Substitute For"].ToString());
        //                                    DNObj.SubstituteItemName = Item["Substitute Name"].ToString();//Substitute Name
        //                                    DNObj.Authorized = Item["Line Status if Substitution"].ToString();//Line Status if Substitution
        //                                }
        //                                else
        //                                {
        //                                    //DNObj.UNCode = Item["Substitute For"].ToString() != "" ? Convert.ToInt64(Item["Substitute For"].ToString()) : 0;//Substitute For
        //                                    if (Item["Substitute For"].ToString().Trim() == "" || Item["Substitute For"].ToString().Trim() == "{}")
        //                                        DNObj.UNCode = 0;
        //                                    else
        //                                        DNObj.UNCode = Convert.ToInt64(Item["Substitute For"].ToString());

        //                                    DNObj.Commodity = Item["Substitute Name"].ToString();//Substitute Name

        //                                    DNObj.SubstituteItemCode = Item["Item"].ToString() != "" ? Convert.ToInt64(Item["Item"].ToString()) : 0;//Item
        //                                    DNObj.SubstituteItemName = Item["Description"].ToString();//Description
        //                                    DNObj.Authorized = Item["Line Status if Substitution"].ToString();//Line Status if Substitution
        //                                }
        //                                DNObj.OrderedQty = Item["Ordered Qty"].ToString() != "" ? Convert.ToDecimal(Item["Ordered Qty"].ToString()) : 0;
        //                                DNObj.DeliveredQty = Item["Delivered Qty"].ToString() != "" ? Convert.ToDecimal(Item["Delivered Qty"].ToString()) : 0;
        //                                DNObj.NoOfPacks = Item["Number of Packs"].ToString() != "" ? Convert.ToDecimal(Item["Number of Packs"].ToString()) : 0;
        //                                DNObj.NoOfPieces = Item["Number of Pieces"].ToString() != "" ? Convert.ToDecimal(Item["Number of Pieces"].ToString()) : 0;

        //                                DNObj.UOM = Item["UOM"].ToString();
        //                                DNObj.IssueType = Item["Issue Type"].ToString();
        //                                DNObj.Remarks = Item["Remarks"].ToString();

        //                                DNObj.ReceivedQty = Item["Actual Received Qty#"].ToString() != "" ? Convert.ToDecimal(Item["Actual Received Qty#"].ToString()) : 0;
        //                                if (Item["Recd Date @ Contingent"].ToString() != "")
        //                                    DNObj.ReceivedDate = (DateTime)Item["Recd Date @ Contingent"];
        //                                else
        //                                    DNObj.ReceivedDate = null;

        //                                DNObj.ControlId = Item["Unit Control No:"].ToString();
        //                                DNObj.DeliveryNoteName = Item["DN Number"].ToString();
        //                                DNObj.DeliveryNoteType = Item["DN Type"].ToString();
        //                                DNObj.DeliveryMode = Item["Delivery Mode"].ToString();

        //                                if (Item["Approved Delivery Date"].ToString() != "")
        //                                    DNObj.ApprovedDeliveryDate = (DateTime)Item["Approved Delivery Date"];
        //                                else
        //                                    DNObj.ApprovedDeliveryDate = null;
        //                                if (Item["Shipment Date"].ToString() != "")
        //                                    DNObj.ShipmentDate = (DateTime)Item["Shipment Date"];
        //                                else
        //                                    DNObj.ShipmentDate = null;
        //                                if (Item["Actual Warehouse Shipped Out Date"].ToString() != "")
        //                                    DNObj.ActualWarehouseShippedOutDate = (DateTime)Item["Actual Warehouse Shipped Out Date"];
        //                                else
        //                                    DNObj.ActualWarehouseShippedOutDate = null;

        //                                DNObj.ImpPeriod = Item["Period"].ToString();
        //                                DNObj.Week = Item["UN Week"].ToString() != "" ? Convert.ToInt64(Item["UN Week"].ToString()) : 0;
        //                                DNObj.ConsumptionWeek = Item["Consumption Week"].ToString() != "" ? Convert.ToDecimal(Item["Consumption Week"].ToString()) : 0;
        //                                DNObj.DeliveryWeek = Item["Delivery Week"].ToString() != "" ? Convert.ToDecimal(Item["Delivery Week"].ToString()) : 0;
        //                                DNObj.RequestNo = Item["Request#"].ToString() != "" ? Convert.ToDecimal(Item["Request#"].ToString()) : 0;
        //                                DNObj.Warehouse = Item["Warehouse"].ToString();
        //                                DNObj.Strength = Item["Strength"].ToString() != "" ? Convert.ToDecimal(Item["Strength"].ToString()) : 0;
        //                                DNObj.DOS = Item["DOS"].ToString() != "" ? Convert.ToDecimal(Item["DOS"].ToString()) : 0;
        //                                DNObj.ManDays = Item["Man days"].ToString() != "" ? Convert.ToDecimal(Item["Man days"].ToString()) : 0;

        //                                DNObj.StorerKey = Item["Storer Key"].ToString();

        //                                DNObj.RemainingQty = (Item["Ordered Qty"].ToString() != "" ? Convert.ToDecimal(Item["Ordered Qty"].ToString()) : 0) - (Item["Delivered Qty"].ToString() != "" ? Convert.ToDecimal(Item["Delivered Qty"].ToString()) : 0);
        //                                DNObj.Period = Period;
        //                                DNObj.PeriodYear = PeriodYear;
        //                                DNObj.CreatedDate = DateTime.Now.Date;
        //                                DNObj.CreatedBy = loggedInUserId;
        //                                DNObj.Sector = Sector.ToString();
        //                                DNObj.Name = Name.ToString();
        //                                DNObj.Location = Location.ToString();
        //                                DNObj.IsValid = "Valid";

        //                                DNObj.RequestId = upreq.RequestId;

        //                                if (DNObj.ControlId != "")
        //                                    Itemlist.Add(DNObj);
        //                            }

        //                        }
        //                        #endregion
        //                        orderService.SaveOrUpdateGCCRevisedList(Itemlist);
        //                        Dictionary<long, IList<GCCRevised>> GccRevisedList = orderService.GetGccRevisedListWithPagingAndCriteria(0, 999999, string.Empty, string.Empty, criteria);
        //                        IList<GCCRevised> deliverynotelist = GccRevisedList.FirstOrDefault().Value.ToList();
        //                        var ControlId = (from u in deliverynotelist select u.ControlId).Distinct().ToArray();
        //                        for (int i = 0; i < ControlId.Length; i++)
        //                        {
        //                            criteria.Clear();
        //                            criteria.Add("ControlId", ControlId[i]);
        //                            Dictionary<long, IList<GCCRevised>> GccRevisedDNList = orderService.GetGccRevisedListWithPagingAndCriteria(0, 999999, string.Empty, string.Empty, criteria);
        //                            IList<GCCRevised> GccRevisedDNName = GccRevisedDNList.FirstOrDefault().Value.ToList();
        //                            var DeliveryNoteName = (from u in GccRevisedDNName select u.DeliveryNoteName).Distinct().ToArray();
        //                            for (int k = 0; k < DeliveryNoteName.Length; k++)
        //                            {

        //                                    InsertUpdateProcessofGCCRevised(Period, PeriodYear, ControlId[i], DeliveryNoteName[k]);



        //                            }
        //                        }
        //                        upreq.UploadStatus = "Uploaded Successfully";
        //                        orderService.SaveOrUpdateUploadRequest(upreq);
        //                    }
        //                    else
        //                    {
        //                        upreq.UploadStatus = "Already Exist";
        //                        upreq.ErrorDesc = "Already Exists for the given period and period year ";
        //                        orderService.SaveOrUpdateUploadRequest(upreq);
        //                        return Json(new { success = true, result = "DeliveryNote for " + Period + "-" + PeriodYear + " Week-" + Week + " is already Exists." }, "text/html", JsonRequestBehavior.AllowGet);

        //                    }

        //                 //   return Json(new { success = true, result = "DeliveryNote updated successfully." }, "text/html", JsonRequestBehavior.AllowGet);
        //                }
        //            }
        //        }
        //        // Json(new { success = true, result = "DeliveryNote updated successfully." }, "text/html", JsonRequestBehavior.AllowGet);
        //         ViewBag.Message="success";
        //         return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        upreq.ErrorDesc = ex.ToString();
        //        upreq.UploadStatus = "Upload Failed";
        //        orderService.SaveOrUpdateUploadRequest(upreq);
        //        return Json(new { success = true, result = "DeliveryNote updated Failed." }, "text/html", JsonRequestBehavior.AllowGet);
        //        // throw;
        //    }
        //}


        [HttpPost]
        public ActionResult GCCRevisedDNUpload(HttpPostedFileBase uploadedFile, string Period, string PeriodYear, string RequestName, long Week, string Sector)
        {
            INSIGHT.Entities.User Userobj = (INSIGHT.Entities.User)Session["objUser"];
            if (Userobj == null || (Userobj != null && Userobj.UserId == null))
            { return RedirectToAction("LogOn", "Account"); }
            string loggedInUserId = Userobj.UserId;

            new Task(() =>
            {
                UploadGCCRevisedFiles(uploadedFile, Period, PeriodYear, RequestName, Week, loggedInUserId, Sector);
            }).Start();
            //Creating the upload request for the DN Data upload
            return Json(new { success = true, result = "DN Data format Uploading Initiated" }, "text/html", JsonRequestBehavior.AllowGet);
        }



        public void UploadGCCRevisedFiles(HttpPostedFileBase uploadedFile, string Period, string PeriodYear, string RequestName, long Week, string loggedInUserId, string sector)
        {

            UploadRequest upreq = new UploadRequest();
            upreq.Category = "DNDataFormatUpload";
            upreq.CreatedBy = loggedInUserId;
            upreq.CreatedDate = DateTime.Now;
            upreq.RequestName = RequestName;
            upreq.Period = Period;
            upreq.PeriodYear = PeriodYear;
            upreq.Sector = sector;

            upreq.Week = Week;

            long reqid = orderService.SaveOrUpdateUploadRequest(upreq);
            upreq.RequestNo = "REQUEST-" + reqid;

            upreq.UploadStatus = "InProgress";
            orderService.SaveOrUpdateUploadRequest(upreq);
            try
            {
                HttpPostedFileBase theFile = HttpContext.Request.Files["uploadedFile"];
                StringBuilder alreadyExists = new StringBuilder();
                StringBuilder ErrorFilename = new StringBuilder();
                StringBuilder UploadedFilename = new StringBuilder();
                if (theFile != null && theFile.ContentLength > 0)
                {
                    string fileName = string.Empty;
                    string path = uploadedFile.InputStream.ToString();
                    byte[] imageSize = new byte[uploadedFile.ContentLength];
                    uploadedFile.InputStream.Read(imageSize, 0, (int)uploadedFile.ContentLength);
                    string UploadConnStr = "";
                    fileName = uploadedFile.FileName;
                    string fileExtn = Path.GetExtension(uploadedFile.FileName);
                    string fileLocation = ConfigurationManager.AppSettings["GccRevisedFilePath"].ToString() + DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss").Replace(":", ".") + fileName;
                    uploadedFile.SaveAs(fileLocation);
                    if (fileExtn == ".xls")
                    {
                        UploadConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\"";
                    }
                    if (fileExtn == ".xlsx")
                    {
                        UploadConnStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";
                    }
                    DataTable DtblXcelData = new DataTable();
                    OleDbConnection conn = new OleDbConnection();
                    OleDbDataAdapter DtAdptrr = new OleDbDataAdapter();
                    string QeryToGetXcelData = "select * from " + string.Format("{0}${1}", "[DN Data Template", "A1:AF]");
                    OleDbCommand cmd = new OleDbCommand(QeryToGetXcelData, conn);
                    conn.ConnectionString = UploadConnStr;
                    conn.Open();
                    cmd = new OleDbCommand(QeryToGetXcelData, conn);
                    cmd.CommandType = CommandType.Text;
                    DtAdptrr = new OleDbDataAdapter();
                    DtAdptrr.SelectCommand = cmd;
                    DtAdptrr.Fill(DtblXcelData);
                    string[] strArray = { "Line#", "Item", "Description", "Ordered Qty", "Delivered Qty", "Number of Packs", "Number of Pieces", "Substitute For", "Substitute Name", "UOM", "Issue Type", "Remarks", "Line Status if Substitution", "Actual Received Qty#", "Recd Date @ Contingent", "Unit Control No:", "DN Number", "DN Type", "Delivery Mode", "Approved Delivery Date", "Shipment Date", "Actual Warehouse Shipped Out Date", "Period", "UN Week", "Consumption Week", "Delivery Week", "Request#", "Warehouse", "Strength", "DOS", "Man days", "Storer Key" };
                    char chrFlag = 'Y';
                    if (DtblXcelData.Columns.Count == strArray.Length)
                    {
                        int j = 0;
                        string[] strColumnsAray = new string[DtblXcelData.Columns.Count];
                        foreach (DataColumn dtColumn in DtblXcelData.Columns)
                        {
                            strColumnsAray[j] = dtColumn.ColumnName;
                            j++;
                        }
                        for (int i = 0; i < strArray.Length - 1; i++)
                        {
                            if (strArray[i].Trim() != strColumnsAray[i].Trim())
                            {
                                chrFlag = 'N';
                                break;
                            }
                        }
                        if (chrFlag == 'Y')
                        {
                            criteria.Clear();
                            criteria.Add("Period", Period);
                            criteria.Add("PeriodYear", PeriodYear);
                            criteria.Add("Week", Convert.ToInt64(Week));
                            criteria.Add("Sector", sector);
                            Dictionary<long, IList<GCCRevised>> GccRevisedPeriodList = orderService.GetGccRevisedListWithPagingAndCriteria(0, 999999, string.Empty, string.Empty, criteria);
                            if (GccRevisedPeriodList.FirstOrDefault().Key == 0)
                            {
                                #region Read Excel
                                IList<GCCRevised> Itemlist = new List<GCCRevised>();

                                foreach (DataRow Item in DtblXcelData.Rows)
                                {
                                    if (Item["Item"].ToString() != "")
                                    {
                                        GCCRevised DNObj = new GCCRevised();
                                        string Sector = "";
                                        string Name = "";
                                        string Location = "";

                                        string controlId = Item["Unit Control No:"].ToString();
                                        string[] controlIdArray = controlId.Split('-');
                                        Sector = controlIdArray[2];
                                        Name = controlIdArray[3];
                                        Location = controlIdArray[4];

                                        //Sector = controlIdArray[1];
                                        //Name = controlIdArray[3];
                                        //Location = controlIdArray[1] + "-" + controlIdArray[3];

                                        if (Item["Line Status if Substitution"].ToString() == "" || Item["Line Status if Substitution"].ToString() == "None")
                                        {
                                            DNObj.UNCode = Item["Item"].ToString() != "" ? Convert.ToInt64(Item["Item"].ToString()) : 0;
                                            DNObj.Commodity = Item["Description"].ToString();
                                            //DNObj.SubstituteItemCode = Item["Substitute For"].ToString() != "" ? Convert.ToInt64(Item["Substitute For"].ToString()) : 0;//Substitute For
                                            if (Item["Substitute For"].ToString().Trim() != "" || Item["Substitute For"].ToString().Trim() != "{}")
                                                DNObj.SubstituteItemCode = 0;
                                            else
                                                DNObj.SubstituteItemCode = Convert.ToInt64(Item["Substitute For"].ToString());
                                            DNObj.SubstituteItemName = Item["Substitute Name"].ToString();//Substitute Name
                                            DNObj.Authorized = Item["Line Status if Substitution"].ToString();//Line Status if Substitution
                                        }
                                        else
                                        {
                                            //DNObj.UNCode = Item["Substitute For"].ToString() != "" ? Convert.ToInt64(Item["Substitute For"].ToString()) : 0;//Substitute For
                                            if (Item["Substitute For"].ToString().Trim() == "" || Item["Substitute For"].ToString().Trim() == "{}")
                                                DNObj.UNCode = 0;
                                            else
                                                DNObj.UNCode = Convert.ToInt64(Item["Substitute For"].ToString());

                                            DNObj.Commodity = Item["Substitute Name"].ToString();//Substitute Name

                                            DNObj.SubstituteItemCode = Item["Item"].ToString() != "" ? Convert.ToInt64(Item["Item"].ToString()) : 0;//Item
                                            DNObj.SubstituteItemName = Item["Description"].ToString();//Description
                                            DNObj.Authorized = Item["Line Status if Substitution"].ToString();//Line Status if Substitution
                                        }
                                        DNObj.OrderedQty = Item["Ordered Qty"].ToString() != "" ? Convert.ToDecimal(Item["Ordered Qty"].ToString()) : 0;
                                        DNObj.DeliveredQty = Item["Delivered Qty"].ToString() != "" ? Convert.ToDecimal(Item["Delivered Qty"].ToString()) : 0;
                                        DNObj.NoOfPacks = Item["Number of Packs"].ToString() != "" ? Convert.ToDecimal(Item["Number of Packs"].ToString()) : 0;
                                        DNObj.NoOfPieces = Item["Number of Pieces"].ToString() != "" ? Convert.ToDecimal(Item["Number of Pieces"].ToString()) : 0;

                                        DNObj.UOM = Item["UOM"].ToString();
                                        DNObj.IssueType = Item["Issue Type"].ToString();
                                        DNObj.Remarks = Item["Remarks"].ToString();

                                        DNObj.ReceivedQty = Item["Actual Received Qty#"].ToString() != "" ? Convert.ToDecimal(Item["Actual Received Qty#"].ToString()) : 0;
                                        if (Item["Recd Date @ Contingent"].ToString() != "")
                                            DNObj.ReceivedDate = (DateTime)Item["Recd Date @ Contingent"];
                                        else
                                            DNObj.ReceivedDate = null;

                                        DNObj.ControlId = Item["Unit Control No:"].ToString();
                                        DNObj.DeliveryNoteName = Item["DN Number"].ToString();
                                        DNObj.DeliveryNoteType = Item["DN Type"].ToString();
                                        DNObj.DeliveryMode = Item["Delivery Mode"].ToString();

                                        if (Item["Approved Delivery Date"].ToString() != "")
                                            DNObj.ApprovedDeliveryDate = (DateTime)Item["Approved Delivery Date"];
                                        else
                                            DNObj.ApprovedDeliveryDate = null;
                                        if (Item["Shipment Date"].ToString() != "")
                                            DNObj.ShipmentDate = (DateTime)Item["Shipment Date"];
                                        else
                                            DNObj.ShipmentDate = null;
                                        if (Item["Actual Warehouse Shipped Out Date"].ToString() != "")
                                            DNObj.ActualWarehouseShippedOutDate = (DateTime)Item["Actual Warehouse Shipped Out Date"];
                                        else
                                            DNObj.ActualWarehouseShippedOutDate = null;

                                        DNObj.ImpPeriod = Item["Period"].ToString();
                                        DNObj.Week = Item["UN Week"].ToString() != "" ? Convert.ToInt64(Item["UN Week"].ToString()) : 0;
                                        DNObj.ConsumptionWeek = Item["Consumption Week"].ToString() != "" ? Convert.ToDecimal(Item["Consumption Week"].ToString()) : 0;
                                        DNObj.DeliveryWeek = Item["Delivery Week"].ToString() != "" ? Convert.ToDecimal(Item["Delivery Week"].ToString()) : 0;
                                        DNObj.RequestNo = Item["Request#"].ToString() != "" ? Convert.ToDecimal(Item["Request#"].ToString()) : 0;
                                        DNObj.Warehouse = Item["Warehouse"].ToString();
                                        DNObj.Strength = Item["Strength"].ToString() != "" ? Convert.ToDecimal(Item["Strength"].ToString()) : 0;
                                        DNObj.DOS = Item["DOS"].ToString() != "" ? Convert.ToDecimal(Item["DOS"].ToString()) : 0;
                                        DNObj.ManDays = Item["Man days"].ToString() != "" ? Convert.ToDecimal(Item["Man days"].ToString()) : 0;

                                        DNObj.StorerKey = Item["Storer Key"].ToString();

                                        DNObj.RemainingQty = (Item["Ordered Qty"].ToString() != "" ? Convert.ToDecimal(Item["Ordered Qty"].ToString()) : 0) - (Item["Delivered Qty"].ToString() != "" ? Convert.ToDecimal(Item["Delivered Qty"].ToString()) : 0);
                                        DNObj.Period = Period;
                                        DNObj.PeriodYear = PeriodYear;
                                        DNObj.CreatedDate = DateTime.Now.Date;
                                        DNObj.CreatedBy = loggedInUserId;
                                        DNObj.Sector = Sector.ToString();
                                        DNObj.Name = Name.ToString();
                                        DNObj.Location = Location.ToString();
                                        DNObj.IsValid = "Valid";

                                        DNObj.RequestId = upreq.RequestId;

                                        if (DNObj.ControlId != "")
                                            Itemlist.Add(DNObj);
                                    }

                                }
                                #endregion
                                orderService.SaveOrUpdateGCCRevisedList(Itemlist);
                                Dictionary<long, IList<GCCRevised>> GccRevisedList = orderService.GetGccRevisedListWithPagingAndCriteria(0, 999999, string.Empty, string.Empty, criteria);
                                IList<GCCRevised> deliverynotelist = GccRevisedList.FirstOrDefault().Value.ToList();
                                var ControlId = (from u in deliverynotelist select u.ControlId).Distinct().ToArray();
                                for (int i = 0; i < ControlId.Length; i++)
                                {
                                    criteria.Clear();
                                    criteria.Add("ControlId", ControlId[i]);
                                    Dictionary<long, IList<GCCRevised>> GccRevisedDNList = orderService.GetGccRevisedListWithPagingAndCriteria(0, 999999, string.Empty, string.Empty, criteria);
                                    IList<GCCRevised> GccRevisedDNName = GccRevisedDNList.FirstOrDefault().Value.ToList();
                                    var DeliveryNoteName = (from u in GccRevisedDNName select u.DeliveryNoteName).Distinct().ToArray();
                                    for (int k = 0; k < DeliveryNoteName.Length; k++)
                                    {
                                        InsertUpdateProcessofGCCRevised(Period, PeriodYear, ControlId[i], DeliveryNoteName[k]);
                                    }
                                }
                                upreq.UploadStatus = "Uploaded Successfully";
                                orderService.SaveOrUpdateUploadRequest(upreq);
                            }
                            else
                            {
                                upreq.UploadStatus = "Already Exist";
                                upreq.ErrorDesc = "Already Exists for the given period and period year ";
                                orderService.SaveOrUpdateUploadRequest(upreq);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                upreq.ErrorDesc = ex.ToString();
                upreq.UploadStatus = "Upload Failed";
                orderService.SaveOrUpdateUploadRequest(upreq);
                //  return Json(new { success = true, result = "DeliveryNote updated Failed." }, "text/html", JsonRequestBehavior.AllowGet);
                // throw;
            }
        }


        //GCCRevised DN upload request jqgrid
        public JsonResult GCCRevisedDNUploadRequestJqGrid(string Category, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                Dictionary<string, object> criteria = new Dictionary<string, object>();

                //sord = sord == "desc" ? "Desc" : "Asc";
                sord = sord == "desc" ? "Desc" : "Asc";
                if (!string.IsNullOrWhiteSpace(Category)) { criteria.Add("Category", Category); }
                //  criteria.Add("Category", "SUBSTITUTIONMASTERUPLOAD");

                Dictionary<long, IList<UploadRequest>> uploadreqlist = orderService.GetUploadRequestCountListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                if (uploadreqlist != null && uploadreqlist.Count > 0)
                {
                    long totalrecords = uploadreqlist.First().Key;
                    int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                    var jsondat = new
                    {
                        total = totalPages,
                        page = page,
                        records = totalrecords,

                        rows = (from items in uploadreqlist.First().Value
                                select new
                                {
                                    i = 2,
                                    cell = new string[] {
                                        items.RequestId.ToString(),
                                        items.RequestName,
                                        items.Period,
                                        items.PeriodYear,
                                        items.Sector,
                                        items.Week.ToString(),
                                        items.RequestNo,
                                        items.Category,
                                        items.Status,
                                        items.CreatedBy,
                                        items.CreatedDate!=null? ConvertDateTimeToDate(items.CreatedDate.ToString("dd/MM/yyyy"),"en-GB"):"",
                                        items.UploadStatus
                                                             
                            }
                                })
                    };
                    return Json(jsondat, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(null, JsonRequestBehavior.AllowGet);

                }


            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }
        public ActionResult DeliveryNoteDetails(long RequestId)
        {
            UploadRequest upreq = orderService.GetUploadRequestbyRequestId(RequestId);
            return View(upreq);
        }
        //GCCRevised DN jqgrid
        public ActionResult GCCDeliveryNoteJqGrid(string ExptType, string searchItems, long RequestId, string ControlId, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                Dictionary<string, object> likeSearchCriteria = new Dictionary<string, object>();

                //sord = sord == "desc" ? "Desc" : "Asc";
                sord = sord == "desc" ? "Desc" : "Asc";

                criteria.Add("RequestId", RequestId);
                Dictionary<long, IList<GCCRevised>> GCCRevisedItems = null;
                if (!string.IsNullOrWhiteSpace(ControlId))
                {
                    likeSearchCriteria.Add("ControlId", ControlId);
                    GCCRevisedItems = orderService.GetGCCRevisedUploadListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria, likeSearchCriteria);
                }
                else
                {
                    GCCRevisedItems = orderService.GetGCCRevisedUploadListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria, likeSearchCriteria);
                }
                string Docname = "";
                if (GCCRevisedItems != null && GCCRevisedItems.Count > 0)
                {
                    if (ExptType == "Excel")
                    {
                        Docname = "Delivery Note Data Template";
                        int j = 1;
                        foreach (var item in GCCRevisedItems.FirstOrDefault().Value)
                        {
                            item.Id = j;
                            j = j + 1;
                        }
                        var List = GCCRevisedItems.FirstOrDefault().Value;
                        List<string> lstHeader = new List<string>() { "Line#", "Item", "Description", "Ordered Qty", "Delivered Qty", "Number of Packs", "Number of Pieces", "Substitute For", "Substitute Name", "UOM", "Issue Type", "Remarks", "Line Status if Substitution", "Actual Received Qty#", "Recd Date @ Contingent", "Unit Control No:", "DN Number", "DN Type", "Delivery Mode", "Approved Delivery Date", "Shipment Date", "Actual Warehouse Shipped Out Date", "Period", "UN Week", "Consumption Week", "Delivery Week", "Request#", "Warehouse", "Strength", "DOS", "Man days", "Storer Key" };
                        base.NewExportToExcel(List, "" + Docname + "-" + List[0].Period + "-" + List[0].PeriodYear + "- (" + String.Format("{0:dd-MMM-yy}", DateTime.Now) + ")", (item => new
                        {
                            Id = item.Id,
                            UNCode = item.UNCode,
                            Commodity = item.Commodity,
                            OrderedQty = item.OrderedQty,
                            DeliveredQty = item.DeliveredQty,
                            NoOfPacks = item.NoOfPacks,
                            NoOfPieces = item.NoOfPieces,
                            SubstituteItemCode = item.SubstituteItemCode,
                            SubstituteItemName = item.SubstituteItemName,
                            UOM = item.UOM,
                            IssueType = item.IssueType,
                            Remarks = item.Remarks,
                            Authorized = item.Authorized,
                            ReceivedQty = item.ReceivedQty,
                            ReceivedDate = String.Format("{0:dd-MMM-yy}", item.ReceivedDate),
                            ControlId = item.ControlId,
                            DeliveryNoteName = item.DeliveryNoteName,
                            DeliveryNoteType = item.DeliveryNoteType,
                            DeliveryMode = item.DeliveryMode,
                            ApprovedDeliveryDate = String.Format("{0:dd-MMM-yy}", item.ApprovedDeliveryDate),
                            ShipmentDate = String.Format("{0:dd-MMM-yy}", item.ShipmentDate),
                            ActualWarehouseShippedOutDate = String.Format("{0:dd-MMM-yy}", item.ActualWarehouseShippedOutDate),
                            ImpPeriod = item.ImpPeriod,
                            UNWeek = item.Week,
                            ConsumptionWeek = item.ConsumptionWeek,
                            DeliveryWeek = item.DeliveryWeek,
                            RequestNo = item.RequestNo,
                            Warehouse = item.Warehouse,
                            Strength = item.Strength,
                            DOS = item.DOS,
                            ManDays = item.ManDays,
                            StorerKey = item.StorerKey,
                        }), lstHeader);

                        return new EmptyResult();
                    }
                    else
                    {
                        long totalrecords = GCCRevisedItems.First().Key;
                        int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                        var jsondata = new
                        {
                            total = totalPages,
                            page = page,
                            records = totalrecords,

                            rows = (from items in GCCRevisedItems.First().Value
                                    select new
                                    {
                                        i = 2,
                                        cell = new string[] {
                                            items.Id.ToString(),
                                            items.ControlId,
                                            items.ApprovedDeliveryDate!=null? items.ApprovedDeliveryDate.Value.ToString("dd/MM/yyyy"):items.ApprovedDeliveryDate.ToString(),
                                            //items.PlannedDeliveryDate !=null? items.PlannedDeliveryDate.Value.ToString("dd/MM/yyyy"):items.PlannedDeliveryDate.ToString(),
                                            items.DeliveryNoteName,
                                            //items.DeliveryNumber,
                                            items.DeliveryMode,
                                            //items.ActualDeliveryDate !=null? items.ActualDeliveryDate.Value.ToString("dd/MM/yyyy"):items.ActualDeliveryDate.ToString(),
                                            items.ReceivedDate!=null? items.ReceivedDate.Value.ToString("dd/MM/yyyy"):items.ReceivedDate.ToString(),
                                            items.UNCode.ToString(),
                                            items.Commodity,
                                            items.OrderedQty.ToString(),
                                            //items.OrderQty.ToString(),
                                            items.SubstituteItemCode !=0? items.SubstituteItemCode.ToString():"",
                                            items.SubstituteItemName,
                                            items.DeliveredQty.ToString(),
                                            //items.AcceptedQty.ToString(),
                                            items.ReceivedQty.ToString(),
                                            items.RemainingQty.ToString(),
                                            items.Authorized,
                                            items.Period,
                                            items.PeriodYear,
                                            items.CreatedBy,
                                            items.CreatedDate!=null? ConvertDateTimeToDate(items.CreatedDate.ToString("dd/MM/yyyy"),"en-GB"):"",
                                            //Added by Thamizh for Showing Storerkey
                                            items.StorerKey,
                            }
                                    })
                        };
                        return Json(jsondata, JsonRequestBehavior.AllowGet);
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
                UploadRequest upreq = new UploadRequest();
                upreq.ErrorDesc = ex.ToString();
                //ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }
        //Delete GCCRevised DN
        public JsonResult DeleteGCCRevisedDeliveryNote(string RequestIds)
        {
            try
            {
                var RequestIdArray = RequestIds.Split(',');
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                Dictionary<string, object> likeSearchCriteria = new Dictionary<string, object>();
                criteria.Clear();
                Dictionary<long, IList<GCCRevised>> GCCRevisedItems = null;
                if (RequestIdArray.Count() > 0)
                {

                    for (int i = 0; i < RequestIdArray.Count(); i++)
                    {
                        criteria.Clear();
                        criteria.Add("RequestId", Convert.ToInt64(RequestIdArray[i]));
                        GCCRevisedItems = orderService.GetGCCRevisedUploadListWithPagingAndCriteria(0, 99999, string.Empty, string.Empty, criteria, likeSearchCriteria);
                        var DeliveryNoteIds = (from u in GCCRevisedItems.FirstOrDefault().Value select new { u.DeliveryNoteId, u.DeliveryNoteName, u.RequestId }).Distinct().ToList();
                        if (DeliveryNoteIds.Count > 0)
                        {

                            for (int j = 0; j < DeliveryNoteIds.Count; j++)
                            {
                                criteria.Clear();
                                criteria.Add("DeliveryNoteId", Convert.ToInt64(DeliveryNoteIds[j].DeliveryNoteId));
                                criteria.Add("DeliveryNoteName", DeliveryNoteIds[j].DeliveryNoteName);
                                Dictionary<long, IList<DeliveredPODItems_vw>> poditmlist = orderService.GetDeliveredPODItemsListWithPagingAndCriteria(0, 99999, string.Empty, string.Empty, criteria);
                                for (int k = 0; k < poditmlist.FirstOrDefault().Value.Count(); k++)
                                {
                                    var SubstituteItemCode = poditmlist.FirstOrDefault().Value[k].SubstituteItemCode;
                                    var SubstituteItemName = poditmlist.FirstOrDefault().Value[k].SubstituteItemName;
                                    var actualitemcode = poditmlist.FirstOrDefault().Value[k].UNCode;
                                    var actualitemname = poditmlist.FirstOrDefault().Value[k].Commodity;
                                    OrderItems orditems = orderService.GetOrderItemsById(poditmlist.FirstOrDefault().Value[k].LineId);
                                    orditems.DeliveredOrdQty = orditems.DeliveredOrdQty - poditmlist.FirstOrDefault().Value[k].DeliveredQty;
                                    orditems.AcceptedOrdQty = orditems.AcceptedOrdQty - poditmlist.FirstOrDefault().Value[k].AcceptedQty;
                                    orditems.RemainingOrdQty = orditems.RemainingOrdQty + poditmlist.FirstOrDefault().Value[k].DeliveredQty;
                                    if (SubstituteItemCode != 0 && SubstituteItemCode == orditems.SubstituteItemCode && actualitemcode == orditems.UNCode)
                                    {
                                        orditems.SubstituteItemCode = 0;
                                        orditems.SubstituteItemName = "";
                                        orditems.DiscrepancyCode = null;
                                    }
                                    orderService.SaveOrUpdateOrderItems(orditems);
                                }
                                //Delete poditems and deliverynote
                                orderService.DeleteDeliveryNoteandPODItems(Convert.ToInt64(DeliveryNoteIds[j].DeliveryNoteId), DeliveryNoteIds[j].DeliveryNoteName);

                                //Delete the uploadrequest 
                                orderService.DeleteRevisedDNByRequestId(Convert.ToInt64(DeliveryNoteIds[j].RequestId));
                            }
                        }
                        //Delete the uploadrequest 
                        orderService.DeleteRevisedDNByRequestId(Convert.ToInt64(RequestIdArray[i]));
                    }
                }

                return Json(new { success = true, result = "DN Data format Deleted Successfully!!" }, "text/html", JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }

        }
        #endregion

        #region Upload Gcc Revised
        public ActionResult GCCRevisedUpload()
        {
            ViewBag.Result = " ";
            return View();
        }

        [HttpPost]
        public ActionResult GCCRevisedUpload(string Period, string PeriodYear, HttpPostedFileBase file)
        {
            try
            {
                INSIGHT.Entities.User Userobj = (INSIGHT.Entities.User)Session["objUser"];
                if (Userobj == null || (Userobj != null && Userobj.UserId == null))
                { return RedirectToAction("LogOn", "Account"); }
                else
                {
                    string loggedInUserId = Userobj.UserId;



                    DataSet ds = new DataSet();
                    Dictionary<string, object> criteria = new Dictionary<string, object>();

                    if (Request.Files["file"].ContentLength > 0)
                    {
                        int length = file.ContentLength;
                        string fileName = string.Empty;

                        //string path = file.InputStream.ToString();
                        //byte[] imageSize = new byte[file.ContentLength];
                        //file.InputStream.Read(imageSize, 0, (int)file.ContentLength);

                        //fileName = file.FileName;
                        //string fileExtn = Path.GetExtension(file.FileName);
                        //string fileLocation1 = ConfigurationManager.AppSettings["GccRevisedFilePath"].ToString() + DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss").Replace(":", ".") + fileName;
                        //file.SaveAs(fileLocation1);

                        string fileExtension =
                                             System.IO.Path.GetExtension(Request.Files["file"].FileName);

                        if (fileExtension == ".xls" || fileExtension == ".xlsx")
                        {
                            string path = file.InputStream.ToString();
                            byte[] imageSize = new byte[file.ContentLength];
                            file.InputStream.Read(imageSize, 0, (int)file.ContentLength);

                            fileName = file.FileName;
                            string fileExtn = Path.GetExtension(file.FileName);
                            string fileLocation = ConfigurationManager.AppSettings["GccRevisedFilePath"].ToString() + DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss").Replace(":", ".") + fileName;
                            //string fileLocation = Server.MapPath("~/Content/") + Request.Files["file"].FileName;
                            if (System.IO.File.Exists(fileLocation))
                            {

                                System.IO.File.Delete(fileLocation);
                            }
                            file.SaveAs(fileLocation);
                            //Request.Files["file"].SaveAs(fileLocation);
                            string excelConnectionString = string.Empty;
                            excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                            //connection String for xls file format.
                            if (fileExtension == ".xls")
                            {
                                excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                            }
                            //connection String for xlsx file format.
                            else if (fileExtension == ".xlsx")
                            {

                                excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                            }
                            //byte[] UploadedFile = System.IO.File.ReadAllBytes(fileName);
                            byte[] UploadedFile = System.IO.File.ReadAllBytes(fileLocation);

                            UploadGccRevisedRequest UploadGcc = new UploadGccRevisedRequest();
                            UploadGcc.Period = Period;
                            UploadGcc.PeriodYear = PeriodYear;
                            UploadGcc.FileName = fileName;
                            UploadGcc.DocumentData = UploadedFile;
                            UploadGcc.CreatedBy = loggedInUserId;
                            UploadGcc.CreatedDate = DateTime.Now;
                            UploadGcc.ModifiedBy = loggedInUserId;
                            UploadGcc.ModifiedDate = DateTime.Now;
                            orderService.SaveOrUpdateGccRevisedFiles(UploadGcc);

                            //Create Connection to Excel work book and add oledb namespace
                            OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                            excelConnection.Open();
                            DataTable dt = new DataTable();

                            dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            if (dt == null)
                            {
                                return null;
                            }

                            String[] excelSheets = new String[dt.Rows.Count];
                            int t = 0;
                            //excel data saves in temp file here.
                            foreach (DataRow row in dt.Rows)
                            {
                                excelSheets[t] = row["TABLE_NAME"].ToString();
                                t++;
                            }
                            OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                            string query = string.Format("Select * from [{0}]", excelSheets[0]);
                            using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                            {
                                dataAdapter.Fill(ds);
                            }
                        }
                        if (fileExtension.ToString().ToLower().Equals(".xml"))
                        {
                            string fileLocation = ConfigurationManager.AppSettings["GccRevisedFilePath"].ToString() + DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss").Replace(":", ".") + fileName;
                            if (System.IO.File.Exists(fileLocation))
                            {
                                System.IO.File.Delete(fileLocation);
                            }
                            file.SaveAs(fileLocation);
                            XmlTextReader xmlreader = new XmlTextReader(fileLocation);
                            // DataSet ds = new DataSet();
                            ds.ReadXml(xmlreader);
                            xmlreader.Close();
                        }
                        criteria.Add("Period", Period);
                        criteria.Add("PeriodYear", PeriodYear);

                        Dictionary<long, IList<GCCRevised>> GccRevisedPeriodList = orderService.GetGccRevisedListWithPagingAndCriteria(0, 999999, string.Empty, string.Empty, criteria);
                        if (GccRevisedPeriodList.FirstOrDefault().Key == 0)
                        {
                            List<GCCRevised> GCR = GetGCCRevisedListWithDataset(ds);//--------------------------
                            orderService.SaveOrUpdateGCCRevisedList(GCR);

                            Dictionary<long, IList<GCCRevised>> GccRevisedList = orderService.GetGccRevisedListWithPagingAndCriteria(0, 999999, string.Empty, string.Empty, criteria);
                            IList<GCCRevised> deliverynotelist = GccRevisedList.FirstOrDefault().Value.ToList();
                            var ControlId = (from u in deliverynotelist select u.ControlId).Distinct().ToArray();
                            for (int i = 0; i < ControlId.Length; i++)
                            {
                                criteria.Clear();
                                criteria.Add("ControlId", ControlId[i]);
                                Dictionary<long, IList<GCCRevised>> GccRevisedDNList = orderService.GetGccRevisedListWithPagingAndCriteria(0, 999999, string.Empty, string.Empty, criteria);
                                IList<GCCRevised> GccRevisedDNName = GccRevisedDNList.FirstOrDefault().Value.ToList();
                                var DeliveryNoteName = (from u in GccRevisedDNName select u.DeliveryNoteName).Distinct().ToArray();
                                for (int j = 0; j < DeliveryNoteName.Length; j++)
                                {
                                    //criteria.Add("DeliveryNoteName", DeliveryNoteName[j]);
                                    InsertUpdateProcessofGCCRevised(Period, PeriodYear, ControlId[i], DeliveryNoteName[j]);
                                }
                            }
                            criteria.Clear();

                            //Period = GCR[0].Period;
                            //PeriodYear = GCR[0].PeriodYear;





                            //  var delnotename = orderService.GetGccDeliverynoteDetailsByControlId(Convert.ToString(controlid[0]));
                            //var dnname = orderService.GetGccDeliverynoteDetailsByDeliveryNoteName();
                            //string Period = GCR[0].Period;
                            //string PeriodYear = GCR[0].PeriodYear;
                            //InsertUpdateProcessofGCCRevised(Period, PeriodYear);

                            ViewBag.Result = "" + Period + "/" + PeriodYear + "";
                            ViewBag.Flag = "True";
                            //ViewBag.Result = "" + GCR[0].Period + "/" + GCR[0].PeriodYear + "";
                            //ViewBag.Result = "P03/14-13";
                        }
                        else
                        {
                            ViewBag.Result = "" + Period + "/" + PeriodYear + "";
                        }
                    }
                    return View();
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private void InsertUpdateProcessofGCCRevised(string Period, string PeriodYear, string ControlId, string DeliveryNoteName)
        {
            try
            {
                orderService.CallGccRevisedUpload_SP(Period, PeriodYear, ControlId, DeliveryNoteName);
                //GCCRevised GRQuery = GetQueryListOfGCCRevised_New(Period, PeriodYear);
                //orderService.SaveOrUpdateGCCRevisedUsingSession(Period, PeriodYear);
                //List<GCCRevisedQueryList> GRQuerys = GetQueryListOfGCCRevised(Period, PeriodYear);
                //foreach (var item in GRQuerys)
                //{
                //    //if (item.Step == "Step14" || item.Step == "Step15")
                //    orderService.UpdateUsingQuries(item.Query);
                //}
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }

        private List<GCCRevisedQueryList> GetQueryListOfGCCRevised(string Period, string PeriodYear)
        {
            try
            {
                List<GCCRevisedQueryList> GRQL = new List<GCCRevisedQueryList>();
                int Id = 1;
                bool step1 = true; bool step2 = true; bool step3 = true; bool step4 = true; bool step5 = true;
                bool step6 = true; bool step7 = true; bool step8 = true; bool step9 = true; bool step10 = true;
                bool step11 = true; bool step12 = true;
                bool Update = false;
                string Query = "";
                for (int i = 0; i < 12; i++)
                {
                    Update = false;
                    GCCRevisedQueryList Gr = new GCCRevisedQueryList();
                    Gr.Id = Id + i;
                    Gr.Step = "Step" + Gr.Id;
                    if (step1 == true)
                    {
                        Gr.Query = "UPDATE GCCRevised SET OrderId=null,PODId=null,LineId=null WHERE Period='" + Period + "' AND PeriodYear='" + PeriodYear + "'";
                        step1 = false; Update = true;
                    }
                    else if (step2 == true)
                    {
                        Query = "";
                        Query = "UPDATE GCCRevised Set OrderId=a.OrderId from Orders a JOIN GCCRevised b on a.ControlId=b.ControlId Where a.period='" + Period + "' and a.PeriodYear='" + PeriodYear + "'";
                        Gr.Query = Query;
                        step2 = false; Update = true;

                    }
                    else if (step3 == true)
                    {
                        Query = "";
                        Query = "UPDATE Orders Set ExpectedDeliveryDate=a.PlannedDeliveryDate from GCCRevised a JOIN Orders b on a.ControlId=b.ControlId Where b.period='" + Period + "' and b.PeriodYear='" + PeriodYear + "'";
                        Gr.Query = Query;
                        step3 = false; Update = true;

                    }
                    else if (step4 == true)
                    {
                        Query = "";
                        Query = "Insert Into POD" + "(" + "CreatedBy,CreatedDate,OrderId,Status" + ")(" + "select '" + "Admin" + "'," + DateTime.Now.Date.ToString("MM/dd/yyyy") + "," + "OrderId" + ",'" + "PODCompleted" + "' from Orders Where period='" + Period + "' and PeriodYear='" + PeriodYear + "')";
                        Gr.Query = Query;
                        step4 = false; Update = true;

                    }
                    else if (step5 == true)
                    {
                        Query = "";
                        Query = "UPDATE POD SET PODNo= 'POD-'+ convert(varchar(10),PODId) from POD Where PODNo IS NULL";
                        Gr.Query = Query;
                        step5 = false; Update = true;

                    }
                    //completed
                    else if (step6 == true)
                    {
                        Query = "";
                        Query = "UPDATE Orders Set PODId=a.PODId,FinalStatus='OrderCompleted' from POD a INNER JOIN Orders b on a.OrderId=b.OrderId WHERE b.period='" + Period + "' and b.PeriodYear='" + PeriodYear + "'";
                        Gr.Query = Query;
                        step6 = false; Update = true;
                    }
                    else if (step7 == true)
                    {
                        Query = "";
                        Query = "Insert Into DeliveryNote(DeliveryNoteName,DeliveryMode,CreatedBy,CreatedDate,OrderId,DeliveryStatus)(select Distinct DeliveryNumber,DeliveryMode,'Benoe',GETDATE(),OrderId,'Completed' from GCCRevised where DeliveryNumber<> '')";
                        Gr.Query = Query;
                        step7 = false; Update = true;
                    }
                    else if (step8 == true)
                    {
                        Query = "";
                        Query = "UPDATE GCCRevised Set PODId=a.PODId from Orders a JOIN GCCRevised b on a.ControlId=b.ControlId Where a.period='" + Period + "' and a.PeriodYear='" + PeriodYear + "'";
                        Gr.Query = Query;
                        step8 = false; Update = true;
                    }
                    else if (step9 == true)
                    {
                        Query = "";
                        Query = "UPDATE GCCRevised SET lineid = (select oi.lineid from orderitems oi where oi.orderid = a.orderid and oi.uncode=a.uncode) From GCCRevised a Where a.period='" + Period + "' and a.PeriodYear='" + PeriodYear + "'";
                        Gr.Query = Query;

                        step9 = false; Update = true;
                    }
                    else if (step10 == true)
                    {
                        Query = "";
                        Query = Query + "INSERT Into PODItems (PODId,OrderId,LineId,OrderedQty,DeliveredQty,AcceptedQty,RemainingQty,DeliveredDate,CreatedBy,";
                        Query = Query + "CreatedDate,DeliveryNoteName,Status,SubstituteItemCode,SubstituteItemName,DiscrepancyCode,DeliverySector) ";
                        Query = Query + "(Select PODId,OrderId,LineId,OrderQty,DeliveredQty,DeliveredQty,RemainingQty,ActualDeliveryDate,'Admin',GETDATE(),";
                        Query = Query + "DeliveryNumber,'DeliveryCompleted',SubstituteItemCode,SubstituteItemName,CASE WHEN Authorized='Substitution' THEN 'AS' ELSE ";
                        Query = Query + "CASE WHEN Authorized='Replacement' THEN 'AR' ELSE NULL END END,";
                        Query = Query + "CASE WHEN (Substring(DeliveryNumber,9,1) = 1) THEN 'SN' ELSE ";
                        Query = Query + "CASE WHEN (Substring(DeliveryNumber,9,1) = 2) THEN 'SW' ELSE ";
                        Query = Query + "CASE WHEN (Substring(DeliveryNumber,9,1) = 3) THEN 'SS' ELSE NULL END END END ";
                        Query = Query + "from GCCRevised Where period='" + Period + "' and PeriodYear='" + PeriodYear + "')";
                        Gr.Query = Query;
                        step10 = false; Update = true;
                    }
                    else if (step11 == true)
                    {
                        Query = "";

                        Query = Query + "UPDATE PODItems SET PODItems.DeliveryNoteId = (";
                        Query = Query + "select oi.DeliveryNoteId from DeliveryNote oi where oi.orderid = PODItems.orderid and oi.DeliveryNoteName=PODItems.DeliveryNoteName)";
                        Query = Query + "from Poditems where  Orderid in (Select Orderid from Orders where period='" + Period + "' and PeriodYear='" + PeriodYear + "')";
                        Gr.Query = Query;
                        step11 = false; Update = true;
                    }
                    else if (step12 == true)
                    {
                        Query = "";
                        Query = Query + "UPDATE OrderItems SET ";
                        Query = Query + "OrderItems.AcceptedOrdQty = GCCRevised.DeliveredQty,OrderItems.RemainingOrdQty = GCCRevised.RemainingQty,OrderItems.DeliveredOrdQty = GCCRevised.DeliveredQty,OrderItems.InvoiceQty = GCCRevised.AcceptedQty,";
                        Query = Query + "OrderItems.SubstituteItemCode = GCCRevised.SubstituteItemCode,";
                        Query = Query + "OrderItems.SubstituteItemName = GCCRevised.SubstituteItemName,";
                        Query = Query + "OrderItems.DiscrepancyCode= (CASE WHEN GCCRevised.Authorized='Substitution' THEN 'AS' ELSE ";
                        Query = Query + "CASE WHEN GCCRevised.Authorized='Replacement' THEN 'AR' ELSE NULL END END)";
                        Query = Query + "FROM OrderItems INNER JOIN PODItems ON ";
                        Query = Query + "OrderItems.LineId = PODItems.LineId ";
                        Query = Query + "INNER JOIN GCCRevised ON OrderItems.LineId = GCCRevised.LineId ";
                        Query = Query + "INNER JOIN orders ON orders.OrderId = OrderItems.OrderId Where orders.period='" + Period + "' and orders.PeriodYear='" + PeriodYear + "'";
                        Gr.Query = Query;
                        step12 = false; Update = true;
                    }
                    //else if (step13 == true)
                    //{
                    //    Query = "";
                    //    Query = "UPDATE Orders SET InvoiceId=null Where period='" + Period + "' and PeriodYear='" + PeriodYear + "'";
                    //    Gr.Query = Query;
                    //    step13 = false; Update = true;

                    //}
                    //else if (step14 == true)
                    //{
                    //    Query = "";
                    //    Query = Query + "INSERT INTO Invoice(InvoiceCode,InvoiceDate,Period,Week,Sector,TotalFeedTroopStrength,";
                    //    Query = Query + "TotalMadays,CMR,SubTotal,GrandTotal,CreatedDate,CreatedBy,ModifiedDate,OrderId,PeriodYear,IsActive) ";
                    //    Query = Query + "(Select InvoiceCode= 'INV-'+a.Name+'-'+a.Period+'-WK'+RIGHT('00' + CONVERT(VARCHAR, a.Week), 2),";
                    //    Query = Query + "GETDATE(),a.Period,a.Week,a.Sector,a.Troops,(a.Troops * 7)As Mandays,";
                    //    Query = Query + "0,0,0,GETDATE(),'Admin',GETDATE(),a.OrderId,a.PeriodYear,1 from Orders a Where a.period='" + Period + "' and a.PeriodYear='" + PeriodYear + "')";
                    //    Gr.Query = Query;
                    //    step14 = false; Update = true;

                    //}
                    //else if (step15 == true)
                    //{
                    //    Query = "";
                    //    Query = Query + "UPDATE Orders Set ";
                    //    Query = Query + "InvoiceId= b.Id ";
                    //    Query = Query + "from Orders a Join ";
                    //    Query = Query + "Invoice b on a.OrderId=b.OrderId Where a.period='" + Period + "' and a.PeriodYear='" + PeriodYear + "'"; 
                    //    Gr.Query = Query;
                    //    step15 = false; Update = true;

                    //}
                    if (Update == true)
                        GRQL.Add(Gr);

                }
                return GRQL;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private List<GCCRevised> GetGCCRevisedListWithDataset(DataSet ds)
        {
            try
            {
                INSIGHT.Entities.User Userobj = (INSIGHT.Entities.User)Session["objUser"];
                string loggedInUserId = Userobj.UserId;
                string CurrentDate = DateTime.Now.Date.ToString("MM/dd/yyyy");

                List<GCCRevised> GCR = new List<GCCRevised>();

                //var Items = ds.Tables[0].Rows[0][15].ToString().Split('-');
                string Sector = "";
                string Name = "";
                string Location = "";
                string Period = "";
                string PeriodYear = "";

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    GCCRevised GR = new GCCRevised();
                    if (ds.Tables[0].Rows[i][0].ToString() != "")
                    {

                        var Items = ds.Tables[0].Rows[i][15].ToString().Split('-');
                        Sector = Items[1];
                        Name = Items[3];
                        Location = Items[1] + "-" + Items[3];
                        Period = Items[4];
                        PeriodYear = Items[6] + "-" + Items[7];
                    }
                    if (ds.Tables[0].Rows[i][12].ToString() == "" || ds.Tables[0].Rows[i][12].ToString() == "None")
                    {
                        GR.UNCode = ds.Tables[0].Rows[i][1].ToString() != "" ? Convert.ToInt64(ds.Tables[0].Rows[i][1].ToString()) : 0;
                        GR.Commodity = ds.Tables[0].Rows[i][2].ToString();
                        GR.SubstituteItemCode = ds.Tables[0].Rows[i][7].ToString() != "" ? Convert.ToInt64(ds.Tables[0].Rows[i][7].ToString()) : 0;
                        GR.SubstituteItemName = ds.Tables[0].Rows[i][8].ToString();
                        GR.Authorized = ds.Tables[0].Rows[i][12].ToString();
                    }
                    else
                    {
                        GR.UNCode = ds.Tables[0].Rows[i][7].ToString() != "" ? Convert.ToInt64(ds.Tables[0].Rows[i][7].ToString()) : 0;
                        GR.Commodity = ds.Tables[0].Rows[i][8].ToString();

                        GR.SubstituteItemCode = ds.Tables[0].Rows[i][1].ToString() != "" ? Convert.ToInt64(ds.Tables[0].Rows[i][1].ToString()) : 0;
                        GR.SubstituteItemName = ds.Tables[0].Rows[i][8].ToString();
                        GR.Authorized = ds.Tables[0].Rows[i][12].ToString();

                    }
                    GR.OrderedQty = ds.Tables[0].Rows[i][3].ToString() != "" ? Convert.ToDecimal(ds.Tables[0].Rows[i][3].ToString()) : 0;
                    GR.DeliveredQty = ds.Tables[0].Rows[i][4].ToString() != "" ? Convert.ToDecimal(ds.Tables[0].Rows[i][4].ToString()) : 0;
                    GR.NoOfPacks = ds.Tables[0].Rows[i][5].ToString() != "" ? Convert.ToDecimal(ds.Tables[0].Rows[i][5].ToString()) : 0;
                    GR.NoOfPieces = ds.Tables[0].Rows[i][6].ToString() != "" ? Convert.ToDecimal(ds.Tables[0].Rows[i][6].ToString()) : 0;
                    //if (ds.Tables[0].Rows[i][7].ToString() != "")
                    //{
                    //    GR.SubstituteItemCode = ds.Tables[0].Rows[i][7].ToString() != "" ? Convert.ToInt64(ds.Tables[0].Rows[i][7].ToString()) : 0;
                    //}
                    //GR.SubstituteItemCode = ds.Tables[0].Rows[i][7].ToString() != "" ? Convert.ToInt64(ds.Tables[0].Rows[i][7].ToString()) : 0;
                    //GR.SubstituteItemName = ds.Tables[0].Rows[i][8].ToString();
                    GR.UOM = ds.Tables[0].Rows[i][9].ToString();
                    GR.IssueType = ds.Tables[0].Rows[i][10].ToString();
                    GR.Remarks = ds.Tables[0].Rows[i][11].ToString();
                    //                    GR.Authorized = ds.Tables[0].Rows[i][12].ToString();
                    GR.ReceivedQty = ds.Tables[0].Rows[i][13].ToString() != "" ? Convert.ToDecimal(ds.Tables[0].Rows[i][13].ToString()) : 0;
                    if (ds.Tables[0].Rows[i][14].ToString() != "")
                        GR.ReceivedDate = (DateTime)ds.Tables[0].Rows[i][14];
                    else
                        GR.ReceivedDate = null;
                    GR.ControlId = ds.Tables[0].Rows[i][15].ToString();
                    GR.DeliveryNoteName = ds.Tables[0].Rows[i][16].ToString();
                    GR.DeliveryNoteType = ds.Tables[0].Rows[i][17].ToString();
                    GR.DeliveryMode = ds.Tables[0].Rows[i][18].ToString();
                    if (ds.Tables[0].Rows[i][19].ToString() != "")
                        GR.ApprovedDeliveryDate = (DateTime)ds.Tables[0].Rows[i][19];
                    else
                        GR.ApprovedDeliveryDate = null;
                    if (ds.Tables[0].Rows[i][20].ToString() != "")
                        GR.ShipmentDate = (DateTime)ds.Tables[0].Rows[i][20];
                    else
                        GR.ShipmentDate = null;
                    if (ds.Tables[0].Rows[i][21].ToString() != "")
                        GR.ActualWarehouseShippedOutDate = (DateTime)ds.Tables[0].Rows[i][21];
                    else
                        GR.ActualWarehouseShippedOutDate = null;
                    GR.ImpPeriod = ds.Tables[0].Rows[i][22].ToString();
                    GR.Week = ds.Tables[0].Rows[i][23].ToString() != "" ? Convert.ToInt64(ds.Tables[0].Rows[i][23].ToString()) : 0;
                    GR.ConsumptionWeek = ds.Tables[0].Rows[i][24].ToString() != "" ? Convert.ToDecimal(ds.Tables[0].Rows[i][24].ToString()) : 0;
                    GR.DeliveryWeek = ds.Tables[0].Rows[i][25].ToString() != "" ? Convert.ToDecimal(ds.Tables[0].Rows[i][25].ToString()) : 0;
                    GR.RequestNo = ds.Tables[0].Rows[i][26].ToString() != "" ? Convert.ToDecimal(ds.Tables[0].Rows[i][26].ToString()) : 0;
                    GR.Warehouse = ds.Tables[0].Rows[i][27].ToString();
                    GR.Strength = ds.Tables[0].Rows[i][28].ToString() != "" ? Convert.ToDecimal(ds.Tables[0].Rows[i][28].ToString()) : 0;
                    GR.DOS = ds.Tables[0].Rows[i][29].ToString() != "" ? Convert.ToDecimal(ds.Tables[0].Rows[i][29].ToString()) : 0;
                    GR.ManDays = ds.Tables[0].Rows[i][30].ToString() != "" ? Convert.ToDecimal(ds.Tables[0].Rows[i][30].ToString()) : 0;

                    GR.RemainingQty = (ds.Tables[0].Rows[i][3].ToString() != "" ? Convert.ToDecimal(ds.Tables[0].Rows[i][3].ToString()) : 0) - (ds.Tables[0].Rows[i][4].ToString() != "" ? Convert.ToDecimal(ds.Tables[0].Rows[i][4].ToString()) : 0);
                    GR.Period = Period;
                    GR.PeriodYear = PeriodYear;
                    GR.CreatedDate = DateTime.Now.Date;
                    GR.CreatedBy = loggedInUserId;
                    GR.Sector = Sector.ToString();
                    GR.Name = Name.ToString();
                    GR.Location = Location.ToString();
                    GR.IsValid = "Valid";

                    if (GR.ControlId != "")
                        GCR.Add(GR);

                }
                return GCR;



            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public ActionResult GCCRevisedUploadListJQGrid(string searchItems, int rows, string sidx, string sord, int? page = 1)
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
                        Dictionary<string, object> likeSearchCriteria = new Dictionary<string, object>();
                        if (!string.IsNullOrWhiteSpace(sord) && sord == "desc")
                            sord = "Desc";
                        else
                            sord = "Asc";
                        criteria.Clear();
                        if (searchItems != null && searchItems != "")
                        {
                            var Items = searchItems.ToString().Split(',');
                            //if (!string.IsNullOrWhiteSpace(Items[0])) { criteria.Add("Sector", Items[0]); }
                            //if (!string.IsNullOrWhiteSpace(Items[1])) { criteria.Add("Name", Items[1]); }
                            //if (!string.IsNullOrWhiteSpace(Items[2])) { criteria.Add("ContingentType", Items[2]); }
                            //if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Location", Items[3]); }
                            if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Period", Items[3]); }
                            if (!string.IsNullOrWhiteSpace(Items[4])) { criteria.Add("PeriodYear", Items[4]); }
                            var TrimItems = searchItems.Replace(",", "");
                            if (string.IsNullOrWhiteSpace(TrimItems))
                                return null;
                        }
                        Dictionary<long, IList<GCCRevised>> GCCRevisedItems = orderService.GetGCCRevisedUploadListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria, likeSearchCriteria);
                        if (GCCRevisedItems != null && GCCRevisedItems.Count > 0)
                        {
                            long totalrecords = GCCRevisedItems.First().Key;
                            int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                            var jsondata = new
                            {
                                total = totalPages,
                                page = page,
                                records = totalrecords,

                                rows = (from items in GCCRevisedItems.First().Value
                                        select new
                                        {
                                            i = 2,
                                            cell = new string[] {
                                            items.Id.ToString(),
                                            items.ControlId,
                                            items.ApprovedDeliveryDate!=null? items.ApprovedDeliveryDate.Value.ToString("dd/MM/yyyy"):items.ApprovedDeliveryDate.ToString(),
                                            //items.PlannedDeliveryDate !=null? items.PlannedDeliveryDate.Value.ToString("dd/MM/yyyy"):items.PlannedDeliveryDate.ToString(),
                                            items.DeliveryNoteName,
                                            //items.DeliveryNumber,
                                            items.DeliveryMode,
                                            //items.ActualDeliveryDate !=null? items.ActualDeliveryDate.Value.ToString("dd/MM/yyyy"):items.ActualDeliveryDate.ToString(),
                                            items.ReceivedDate!=null? items.ReceivedDate.Value.ToString("dd/MM/yyyy"):items.ReceivedDate.ToString(),
                                            items.UNCode.ToString(),
                                            items.Commodity,
                                            items.OrderedQty.ToString(),
                                            //items.OrderQty.ToString(),
                                            items.SubstituteItemCode !=0? items.SubstituteItemCode.ToString():"",
                                            items.SubstituteItemName,
                                            items.DeliveredQty.ToString(),
                                            //items.AcceptedQty.ToString(),
                                            items.ReceivedQty.ToString(),
                                            items.RemainingQty.ToString(),
                                            items.Authorized,
                                            items.Period,
                                            items.PeriodYear,
                                            items.CreatedBy,
                                            items.CreatedDate!=null? ConvertDateTimeToDate(items.CreatedDate.ToString("dd/MM/yyyy"),"en-GB"):"",
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

        #endregion


        #region OrderReport

        public void GetORDRPT_WeekConsolidateList()
        {
            try
            {


            }
            catch (Exception ex)
            {

                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }



        }
        #endregion


        #region Initial Orders Upload

        public ActionResult initialOrdersUpload()
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
        public ActionResult InitialOrdersBulkuploadRequestCreation(long? RequestId)
        {


            UploadRequest upreq = new UploadRequest();
            if (RequestId != null)
            {
                upreq = orderService.GetUploadRequestbyRequestId(RequestId ?? 0);

            }
            return View(upreq);
        }

        [HttpPost]
        public ActionResult InitialOrdersBulkUploadRequestCreation(UploadRequest upreq)
        {
            try
            {
                INSIGHT.Entities.User Userobj = (INSIGHT.Entities.User)Session["objUser"];
                if (Userobj == null || (Userobj != null && Userobj.UserId == null))
                { return RedirectToAction("LogOn", "Account"); }
                string loggedInUserId = Userobj.UserId;
                upreq.Category = "INITIALORDERSUPLOAD";
                upreq.CreatedBy = loggedInUserId;
                upreq.CreatedDate = DateTime.Now;
                long reqid = orderService.SaveOrUpdateUploadRequest(upreq);
                upreq.RequestNo = "REQUEST-" + reqid;
                upreq.Status = "OPEN";
                upreq.UploadStatus = "Request Generated";
                orderService.SaveOrUpdateUploadRequest(upreq);
                return (RedirectToAction("InitialOrdersBulkuploadRequestCreation", new { RequestId = upreq.RequestId }));
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }


        public ActionResult InitialOrdersBulkUpload(long RequestId)
        {
            string RequestNo = "REQUEST-" + RequestId;
            var files = Request.Files["Filedata"];
            string curpath = ConfigurationManager.AppSettings["InitialOrdersUploadRequestFilePath"].ToString();
            string RequestFolder = String.Format("{0}\\" + RequestNo, curpath);
            if (!Directory.Exists(RequestFolder)) { Directory.CreateDirectory(RequestFolder); }
            string savePath = RequestFolder + "\\" + files.FileName;

            files.SaveAs(savePath);

            return Content(Url.Content(@"~\Content\" + files.FileName));

        }
        public void callInitialOrderParallel(long RequestId)
        {
            try
            {
                new Task(() => CreateInitialOrderParallel(RequestId)).Start();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void CreateInitialOrderParallel(long RequestId)
        {
            try
            {

                long orderid = 0;
                string UploadConnStr = string.Empty;
                StringBuilder alreadyExists = new StringBuilder();
                StringBuilder ErrorFilename = new StringBuilder();
                StringBuilder UploadedFilename = new StringBuilder();
                UploadRequestDetailsLog uploadlog = new UploadRequestDetailsLog();

                string fileExtn = string.Empty;
                UploadRequest upreq = new UploadRequest();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                if (RequestId > 0)
                {
                    upreq = orderService.GetUploadRequestbyRequestId(RequestId);
                    if (upreq.UploadStatus == "Request Generated")
                    {
                        upreq.UploadStatus = "InProgress";
                        orderService.SaveOrUpdateUploadRequest(upreq);
                    }
                }
                string curpath = ConfigurationManager.AppSettings["InitialOrdersUploadRequestFilePath"].ToString();
                //string[] filePaths = Directory.GetFiles(curpath + "\\REQUEST-" + RequestId, "*.xlsx");

                string[] filePaths = Directory.GetFiles(curpath + "\\REQUEST-" + RequestId, "*.xlsx");
                if (filePaths.Count() == 0) filePaths = Directory.GetFiles(curpath + "\\REQUEST-" + RequestId, "*.xls");
                foreach (string s in filePaths)
                {
                    try
                    {
                        string AlreadyExistFlag = string.Empty;
                        string fileName = s.ToString();

                        //Delivery note's in Byte array
                        byte[] UploadedFile = System.IO.File.ReadAllBytes(fileName);

                        //Create or update the  saving the individual file status in uploadrequestdetailslog table
                        long UploadReqDetLogId = CreateOrUpdateRequest(fileName, RequestId);
                        uploadlog = orderService.GetUploadRequestDetailsLogbyRequestId(UploadReqDetLogId);
                        //  UploadConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + s + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\"";
                        UploadConnStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + s + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";

                        OleDbConnection conn = new OleDbConnection();
                        DataTable DtblXcelData = new DataTable();
                        string QeryToGetXcelData = "select * from " + string.Format("{0}${1}", "[Form", "A1:AZ]");
                        conn.ConnectionString = UploadConnStr;
                        conn.Open();
                        OleDbCommand cmd = new OleDbCommand(QeryToGetXcelData, conn);
                        cmd.CommandType = CommandType.Text;
                        OleDbDataAdapter DtAdptrr = new OleDbDataAdapter();
                        DtAdptrr.SelectCommand = cmd;
                        DtAdptrr.Fill(DtblXcelData);
                        conn.Close();
                        if (DtblXcelData.Rows.Count == 0)
                        {
                            if (uploadlog.UploadStatus == "YetToUpload")
                                uploadlog.UploadStatus = "UploadFailed";
                            uploadlog.ErrDesc = "Empty file uploaded";
                            orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);
                        }
                        else if (DtblXcelData.Rows.Count > 0)
                        {
                            QeryToGetXcelData = "select * from " + string.Format("{0}${1}", "[Form", "A7:AZ]");
                            conn.ConnectionString = UploadConnStr;
                            conn.Open();
                            cmd = new OleDbCommand(QeryToGetXcelData, conn);
                            cmd.CommandType = CommandType.Text;
                            DtAdptrr = new OleDbDataAdapter();
                            DtAdptrr.SelectCommand = cmd;
                            DtAdptrr.Fill(DtblXcelData);
                            string[] strArray = { "F1", "F2", "F3", "F4", "F5", "UNCode", "Commodity", "Order Qty", "Cost(USD)" };
                            char chrFlag = 'Y';
                            if (DtblXcelData.Columns.Count == strArray.Length)
                            {
                                int j = 0;
                                string[] strColumnsAray = new string[DtblXcelData.Columns.Count];
                                foreach (DataColumn dtColumn in DtblXcelData.Columns)
                                {
                                    strColumnsAray[j] = dtColumn.ColumnName;
                                    j++;
                                }
                                for (int i = 0; i < strArray.Length - 1; i++)
                                {
                                    if (strArray[i].Trim() != strColumnsAray[i].Trim())
                                    {
                                        chrFlag = 'N';
                                        break;
                                    }
                                }
                                if (chrFlag == 'Y')
                                {
                                    InitialOrders ord = new InitialOrders();
                                    IList<InitialOrderItems> Orderitemslist = new List<InitialOrderItems>();

                                    foreach (DataRow Ordline in DtblXcelData.Rows)
                                    {
                                        if (Ordline.ItemArray[0].ToString().Trim() == "Name")
                                        {
                                            ord.Name = Ordline.ItemArray[1].ToString();
                                            // ord.ContingentType = ord.Name.Contains("FPU") ? "FPU" : "MIL";
                                        }
                                        if (Ordline.ItemArray[3].ToString().Trim() == "Start Date")
                                        {
                                            string ordstdt = Ordline.ItemArray[4].ToString().Replace('.', '/');
                                            if (!string.IsNullOrEmpty(ordstdt))
                                                //ord.StartDate = DateTime.Parse(ordstdt);
                                                ord.StartDate = DateTime.Parse(ordstdt, provider, System.Globalization.DateTimeStyles.NoCurrentDateDefault);


                                        }
                                        if (Ordline.ItemArray[3].ToString().Trim() == "Troops #")
                                        {


                                            ord.Troops = Convert.ToInt64(Ordline.ItemArray[4].ToString());
                                        }

                                        if (Ordline.ItemArray[0].ToString().Trim() == "Location")
                                        {
                                            ord.Location = Ordline.ItemArray[1].ToString();
                                        }

                                        if (Ordline.ItemArray[3].ToString().Trim() == "End Date")
                                        {
                                            string ordenddt = Ordline.ItemArray[4].ToString().Replace('.', '/');
                                            if (!string.IsNullOrEmpty(ordenddt))
                                                ord.EndDate = DateTime.Parse(ordenddt, provider, System.Globalization.DateTimeStyles.NoCurrentDateDefault);
                                        }
                                        if (Ordline.ItemArray[0].ToString().Trim() == "Control#")
                                        {
                                            ord.ControlId = Ordline.ItemArray[1].ToString();
                                            //Get previous controlids for validation

                                            Dictionary<long, IList<InitialOrders>> orders = orderService.GetInitialOrdersListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                                            if (orders != null && orders.Count > 0)
                                            {
                                                IList<InitialOrders> orderlist = orders.FirstOrDefault().Value.ToList();
                                                var ControlIdArray = (from u in orderlist select new { u.ControlId }).Distinct().ToArray();
                                                for (int i = 0; i < ControlIdArray.Length; i++)
                                                {
                                                    if (ControlIdArray[i].ControlId == ord.ControlId)
                                                    {
                                                        AlreadyExistFlag = "true";
                                                        alreadyExists.Append(fileName);
                                                        if (uploadlog.UploadStatus == "YetToUpload")
                                                            uploadlog.UploadStatus = "AlreadyExist";
                                                        orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);
                                                    }

                                                }
                                            }
                                            string[] controlIdarray = Ordline.ItemArray[1].ToString().Split('-');
                                            if (string.Compare(controlIdarray[2], "FSC") == 0)
                                                ord.Sector = "FS";
                                            if (string.Compare(controlIdarray[2], "NSC") == 0)
                                                ord.Sector = "NS";
                                            if (string.Compare(controlIdarray[2], "GSC") == 0)
                                                ord.Sector = "GS";
                                            //ord.Sector = controlIdarray[2];
                                            ord.Period = controlIdarray[3];
                                            ord.PeriodYear = controlIdarray[4];
                                            ord.CalYear = Convert.ToInt64(controlIdarray[4]);

                                        }

                                        if (Ordline.ItemArray[2].ToString().Trim() == "Troops #")
                                        {
                                            ord.Troops = Convert.ToDecimal(Ordline.ItemArray[3].ToString());
                                        }
                                        if (Ordline.ItemArray[2].ToString().Trim() == "Total Amount")
                                        {
                                            if (!string.IsNullOrEmpty(Ordline.ItemArray[4].ToString()))
                                                ord.TotalAmount = Convert.ToDecimal(Ordline.ItemArray[4].ToString());
                                        }
                                        if (Ordline.ItemArray[2].ToString().Trim() == "# Line Items Ordered")
                                        {
                                            if (!string.IsNullOrEmpty(Ordline.ItemArray[4].ToString()))
                                                ord.LineItemsOrdered = Convert.ToDecimal(Ordline.ItemArray[4].ToString());
                                        }
                                        if (Ordline.ItemArray[2].ToString().Trim() == "Kg Ordered w/o eggs")
                                        {
                                            if (!string.IsNullOrEmpty(Ordline.ItemArray[4].ToString()))
                                                ord.KgOrderedWOEggs = Convert.ToDecimal(Ordline.ItemArray[4].ToString());
                                        }
                                        if (Ordline.ItemArray[2].ToString().Trim() == "Eggs weight")
                                        {
                                            if (!string.IsNullOrEmpty(Ordline.ItemArray[4].ToString()))
                                                ord.EggsWeight = Convert.ToDecimal(Ordline.ItemArray[4].ToString());
                                        }
                                        if (Ordline.ItemArray[2].ToString().Trim() == "Total weight")
                                        {
                                            if (!string.IsNullOrEmpty(Ordline.ItemArray[4].ToString()))
                                                ord.TotalWeight = Convert.ToDecimal(Ordline.ItemArray[4].ToString());
                                        }
                                        if (Ordline["UNCode"].ToString() != "")
                                        {
                                            InitialOrderItems orditem = new InitialOrderItems();
                                            orditem.UNCode = Convert.ToInt64(Ordline["UNCode"].ToString());
                                            orditem.Commodity = Ordline["Commodity"].ToString();
                                            orditem.OrderQty = Convert.ToDecimal(Ordline["Order Qty"].ToString());
                                            //orditem.SectorPrice = Convert.ToDecimal(Ordline["Sector Price"].ToString());
                                            orditem.Total = Convert.ToDecimal(Ordline["Cost (USD)"]);
                                            orditem.CreatedBy = uploadlog.CreatedBy;
                                            orditem.CreatedDate = DateTime.Now;
                                            orditem.RemainingOrdQty = Convert.ToDecimal(Ordline["Order Qty"].ToString());
                                            if (AlreadyExistFlag != "true")
                                            {
                                                Orderitemslist.Add(orditem);
                                            }
                                        }
                                    }
                                    ord.CreatedBy = uploadlog.CreatedBy;
                                    ord.CreatedDate = DateTime.Now;
                                    // ord.OrderId = orderid;//----------------------------------------------->kingston
                                    ord.InvoiceStatus = "YetToGenerate";
                                    ord.DocumentData = UploadedFile; //add by john

                                    if (AlreadyExistFlag != "true")
                                    {

                                        try
                                        {
                                            orderService.SaveOrUpdateInitialOrdersUsingSession(ord, Orderitemslist);
                                            if (uploadlog.UploadStatus == "YetToUpload")
                                                uploadlog.UploadStatus = "UploadedSuccessfully";
                                            uploadlog.ReferenceNo = "ORD-" + ord.OrderId;
                                            orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);
                                        }
                                        catch (Exception ex)
                                        {
                                            if (uploadlog.UploadStatus == "YetToUpload")
                                                uploadlog.UploadStatus = "UploadFailed";
                                            uploadlog.ErrDesc = "Orders upload transaction failiure----" + ex.ToString();
                                            orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);
                                        }

                                        //long ordid = orderService.SaveOrUpdateOrder(ord);
                                        //orderService.SaveOrUpdateOrderItemsList(Orderitemslist);
                                        //decimal OrderqtyEggsonly = 0;
                                        //decimal OrderqtyWithoutEggs = 0;
                                        //decimal TotalAmt = 0;

                                        //criteria.Clear();
                                        //criteria.Add("OrderId", ord.OrderId);
                                        //Dictionary<long, IList<OrderItems>> orditems = orderService.GetOrderItemsListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                                        //for (int i = 0; i < orditems.FirstOrDefault().Value.Count(); i++)
                                        //{
                                        //    TotalAmt = TotalAmt + (orditems.FirstOrDefault().Value[i].OrderQty * orditems.FirstOrDefault().Value[i].SectorPrice);

                                        //    if (orditems.FirstOrDefault().Value[i].UNCode == 1129)
                                        //    {
                                        //        OrderqtyEggsonly = OrderqtyEggsonly + (orditems.FirstOrDefault().Value[i].OrderQty * (decimal)0.0625);
                                        //    }
                                        //    if (orditems.FirstOrDefault().Value[i].UNCode != 1129)
                                        //    {
                                        //        OrderqtyWithoutEggs = OrderqtyWithoutEggs + (orditems.FirstOrDefault().Value[i].OrderQty);
                                        //    }
                                        //}
                                        //ord.TotalAmount = TotalAmt;
                                        //ord.EggsWeight = OrderqtyEggsonly;
                                        //ord.KgOrderedWOEggs = OrderqtyWithoutEggs;
                                        //ord.TotalWeight = OrderqtyEggsonly + OrderqtyWithoutEggs;
                                        //ord.DocumentData = UploadedFile; //add by john
                                        //orderService.SaveOrUpdateInitialOrder(ord);
                                        UploadedFilename.Append(fileName + ",");
                                    }
                                    //}
                                    //catch(Exception ex){
                                    //    if (uploadlog.UploadStatus == "YetToUpload")
                                    //        uploadlog.UploadStatus = "UploadFailed";
                                    //    uploadlog.ErrDesc = "Headers,transaction errors " + ex.ToString();
                                    //    orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);
                                    //}

                                }
                                else
                                {
                                    if (uploadlog.UploadStatus == "YetToUpload")
                                        uploadlog.UploadStatus = "UploadFailed";
                                    uploadlog.ErrDesc = "Headers error ---The no of columns will be leaser or spelling change in headers";
                                    orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);

                                    //ErrorFilename.Append(fileName + ",");
                                }



                            }

                            else
                            {
                                if (uploadlog.UploadStatus == "YetToUpload")
                                    uploadlog.UploadStatus = "UploadFailed";
                                uploadlog.ErrDesc = "Headers error  or transaction errors";
                                orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);

                                //ErrorFilename.Append(fileName + ",");
                            }
                        }
                        //success = success + 1;

                    }
                    catch (Exception ex)
                    {
                        if (uploadlog.UploadStatus == "YetToUpload")
                            uploadlog.UploadStatus = "UploadFailed";
                        uploadlog.ErrDesc = ex.ToString();
                        orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);

                        //ErrorFilename.Append(fileName + ",");

                    }
                }
                criteria.Clear();
                StatusUpdateInUploadRequest(RequestId);
            }



            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                //throw ex;
            }
        }
        #endregion


        #region Local purchase for P05

        public ActionResult LocalPurchaseFFOUpload()
        {

            return View();

        }


        [HttpPost]
        public ActionResult LocalPurchaseFFOUpload(HttpPostedFileBase uploadedFile, string Period, string PeriodYear, string RequestName)
        {
            UploadRequest upreq = new UploadRequest();
            INSIGHT.Entities.User Userobj = (INSIGHT.Entities.User)Session["objUser"];
            if (Userobj == null || (Userobj != null && Userobj.UserId == null))
            { return RedirectToAction("LogOn", "Account"); }
            string loggedInUserId = Userobj.UserId;
            upreq.Category = "ExpDeliveryDateUpload";
            upreq.CreatedBy = loggedInUserId;
            upreq.CreatedDate = DateTime.Now;
            upreq.RequestName = RequestName;
            upreq.Period = Period;
            upreq.PeriodYear = PeriodYear;
            long reqid = orderService.SaveOrUpdateUploadRequest(upreq);
            upreq.RequestNo = "REQUEST-" + reqid;
            // upreq.Status = "OPEN";
            //  upreq.UploadStatus = "Request Generated";
            upreq.UploadStatus = "InProgress";
            orderService.SaveOrUpdateUploadRequest(upreq);

            try
            {
                //construct the result string
                //first successful uploaded files, then already exists and error
                StringBuilder retValue = new StringBuilder();

                int success = 0;

                HttpPostedFileBase theFile = HttpContext.Request.Files["uploadedFile"];
                StringBuilder alreadyExists = new StringBuilder();
                StringBuilder ErrorFilename = new StringBuilder();
                StringBuilder UploadedFilename = new StringBuilder();
                if (theFile != null && theFile.ContentLength > 0)
                {
                    //INSIGHT.Entities.User Userobj = (INSIGHT.Entities.User)Session["objUser"];
                    //if (Userobj == null || (Userobj != null && Userobj.UserId == null))
                    //{ return RedirectToAction("LogOn", "Account"); }
                    //string loggedInUserId = Userobj.UserId;
                    string fileName = string.Empty;

                    try
                    {

                        string path = uploadedFile.InputStream.ToString();
                        byte[] imageSize = new byte[uploadedFile.ContentLength];
                        uploadedFile.InputStream.Read(imageSize, 0, (int)uploadedFile.ContentLength);
                        string UploadConnStr = "";
                        fileName = uploadedFile.FileName;
                        string fileExtn = Path.GetExtension(uploadedFile.FileName);
                        string fileLocation = ConfigurationManager.AppSettings["LocalpurchaseFFOUploadFilePath"].ToString() + DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss").Replace(":", ".") + fileName;
                        uploadedFile.SaveAs(fileLocation);
                        if (fileExtn == ".xls")
                        {
                            UploadConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\"";
                        }
                        if (fileExtn == ".xlsx")
                        {
                            UploadConnStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";
                        }

                        DataTable DtblXcelData = new DataTable();
                        OleDbConnection conn = new OleDbConnection();
                        OleDbDataAdapter DtAdptrr = new OleDbDataAdapter();
                        string QeryToGetXcelData = "select * from " + string.Format("{0}${1}", "[LPFFO", "A1:AZ]");
                        OleDbCommand cmd = new OleDbCommand(QeryToGetXcelData, conn);
                        conn.ConnectionString = UploadConnStr;
                        conn.Open();
                        cmd = new OleDbCommand(QeryToGetXcelData, conn);
                        cmd.CommandType = CommandType.Text;
                        DtAdptrr = new OleDbDataAdapter();
                        DtAdptrr.SelectCommand = cmd;
                        DtAdptrr.Fill(DtblXcelData);
                        string[] strArray = { "Sno", "ControlId", "StartDate", "EndDate", "Troops", "TotalAmount", "LineItemsOrdered", "KgOrderedWOEggs", "EggsWeight", "OrderCMR", "TotalWeight", "UNCode", "Commodity", "OrderQty", "SectorPrice", "Total" };
                        char chrFlag = 'Y';
                        if (DtblXcelData.Columns.Count == strArray.Length)
                        {
                            int j = 0;
                            string[] strColumnsAray = new string[DtblXcelData.Columns.Count];
                            foreach (DataColumn dtColumn in DtblXcelData.Columns)
                            {
                                strColumnsAray[j] = dtColumn.ColumnName;
                                j++;
                            }
                            for (int i = 0; i < strArray.Length - 1; i++)
                            {
                                if (strArray[i].Trim() != strColumnsAray[i].Trim())
                                {
                                    chrFlag = 'N';
                                    break;
                                }
                            }
                            if (chrFlag == 'Y')
                            {
                                try
                                {
                                    IList<Orders_LocalPurchase> orderlist = new List<Orders_LocalPurchase>();

                                    foreach (DataRow ordline in DtblXcelData.Rows)
                                    {

                                        if (ordline["ControlId"].ToString() != "")
                                        {
                                            Orders_LocalPurchase ord = new Orders_LocalPurchase();

                                            // orditem.LineId = GetCounterValue("OrderItems");

                                            string controlId = ordline["ControlId"].ToString();
                                            string[] controlIdArray = controlId.Split('-');

                                            ord.ControlId = controlId;
                                            ord.Sector = controlIdArray[1].ToString();
                                            ord.Name = controlIdArray[3].ToString();
                                            ord.Period = controlIdArray[4].ToString();
                                            ord.PeriodYear = controlIdArray[6].ToString() + "-" + controlIdArray[7].ToString();
                                            ord.Location = controlIdArray[1].ToString() + "-" + controlIdArray[2].ToString();

                                            string StartDate = ordline["StartDate"].ToString();
                                            if (!string.IsNullOrEmpty(StartDate))
                                                // impdelitems.ImpDeliveryDate = DateTime.Parse(ImpDeliveryDate, provider, System.Globalization.DateTimeStyles.NoCurrentDateDefault);
                                                ord.StartDate = Convert.ToDateTime(ordline["StartDate"].ToString().Trim());
                                            string EndDate = ordline["EndDate"].ToString();
                                            if (!string.IsNullOrEmpty(EndDate))
                                                // impdelitems.ImpDeliveryDate = DateTime.Parse(ImpDeliveryDate, provider, System.Globalization.DateTimeStyles.NoCurrentDateDefault);
                                                ord.EndDate = Convert.ToDateTime(ordline["EndDate"].ToString().Trim());
                                            ord.CreatedBy = loggedInUserId;
                                            ord.CreatedDate = DateTime.Now;
                                            ord.RequestId = upreq.RequestId;
                                            // impdel.ImpDeliveryWeek = delline.ItemArray[5].ToString().Trim() == "" ? 0 : Convert.ToDecimal(delline.ItemArray[5].ToString());
                                            ord.Troops = Convert.ToInt64(ordline["Troops"].ToString());
                                            ord.LineItemsOrdered = Convert.ToInt64(ordline["LineItemsOrdered"].ToString());
                                            ord.KgOrderedWOEggs = Convert.ToDecimal(ordline["KgOrderedWOEggs"].ToString());
                                            ord.EggsWeight = Convert.ToDecimal(ordline["EggsWeight"].ToString());
                                            ord.OrderCMR = Convert.ToDecimal(ordline["OrderCMR"].ToString());
                                            ord.TotalWeight = Convert.ToDecimal(ordline["TotalWeight"].ToString());
                                            ord.UNCode = Convert.ToInt64(ordline["UNCode"].ToString());
                                            ord.Commodity = ordline["Commodity"].ToString();
                                            ord.OrderQty = Convert.ToDecimal(ordline["OrderQty"].ToString());
                                            ord.SectorPrice = Convert.ToDecimal(ordline["SectorPrice"].ToString());
                                            ord.Total = Convert.ToDecimal(ordline["Total"].ToString());

                                            orderlist.Add(ord);
                                        }
                                    }

                                    if (orderlist != null && orderlist.Count > 0)
                                    {
                                        try
                                        {
                                            orderService.SaveorUpdateOrders_LocalPurchaseByList(orderlist);
                                            //orderService.InsertLocalPurchaseFFObySP(upreq.RequestId);
                                            //for (int i = 0; i < expdeldtlist.Count; i++)
                                            //{
                                            //    Orders ord = new Orders();
                                            //    ord = orderService.GetOrderByControlId(expdeldtlist[i].ControlId);
                                            //    if (ord != null)
                                            //    {
                                            //        ord.ExpectedDeliveryDate = expdeldtlist[i].ImpExpDeliveryDate;
                                            //        orderService.SaveOrUpdateOrder(ord);
                                            //    }
                                            //}
                                            upreq.UploadStatus = "Uploaded Successfully";
                                            orderService.SaveOrUpdateUploadRequest(upreq);
                                        }
                                        catch (Exception ex)
                                        {
                                            upreq.ErrorDesc = Convert.ToString(ex + "----------6");
                                            orderService.SaveOrUpdateUploadRequest(upreq);
                                            ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    upreq.ErrorDesc = Convert.ToString(ex + "------------------------5");
                                    orderService.SaveOrUpdateUploadRequest(upreq);
                                    ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                                }


                            }
                            else
                            {
                                try
                                {
                                    upreq.UploadStatus = "Upload Failed";
                                    orderService.SaveOrUpdateUploadRequest(upreq);
                                    return Json(new { success = false, result = "Headers mismatch!!!" }, "text/html", JsonRequestBehavior.AllowGet);
                                }
                                catch (Exception ex)
                                {
                                    upreq.ErrorDesc = Convert.ToString(ex + "--------------4");
                                    orderService.SaveOrUpdateUploadRequest(upreq);
                                    ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                upreq.UploadStatus = "Upload Failed";
                                orderService.SaveOrUpdateUploadRequest(upreq);
                                ErrorFilename.Append(fileName + ",");
                            }
                            catch (Exception ex)
                            {
                                upreq.ErrorDesc = Convert.ToString(ex + "-----------1");
                                orderService.SaveOrUpdateUploadRequest(upreq);
                                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                            }
                        }

                        success = success + 1;
                    }
                    catch (Exception ex)
                    {
                        upreq.UploadStatus = "Upload Failed";
                        upreq.ErrorDesc = Convert.ToString(ex + "-----------2");
                        orderService.SaveOrUpdateUploadRequest(upreq);
                        ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                    }

                }
                else
                {
                    return Json(new { success = false, result = "You have uploded the empty file. Please upload the correct file." }, "text/html", JsonRequestBehavior.AllowGet);
                }

                //MyLabel.Text = sb.ToString().Replace(Environment.NewLine, "<br />");
                return Json(new { success = true, result = "Expected delivery date updated successfully." }, "text/html", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                upreq.ErrorDesc = Convert.ToString(ex + "------------------3");
                orderService.SaveOrUpdateUploadRequest(upreq);
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;

            }
        }

        private void InsertExcelRecords(string FilePath, string loggedInUserId, string RequestName, string fileName)
        {

            UploadRequest upreq = new UploadRequest();
            upreq.Category = "Local Purchase FFO Uplolad";
            upreq.CreatedBy = loggedInUserId;
            upreq.CreatedDate = DateTime.Now;
            upreq.RequestName = RequestName;
            upreq.FileName = fileName;
            upreq.UploadStatus = "InProgress";
            long reqid = orderService.SaveOrUpdateUploadRequest(upreq);
            upreq.RequestNo = "REQUEST-" + reqid;
            string constr = string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0 Xml;HDR=YES;""", FilePath);
            OleDbConnection Econ = new OleDbConnection(constr);

            string Query = "Select * FROM " + string.Format("{0}${1}", "[LPFFO", "A1:X]");
            OleDbCommand Ecom = new OleDbCommand(Query, Econ);
            Econ.Open();

            DataSet ds = new DataSet();

            OleDbDataAdapter oda = new OleDbDataAdapter(Query, Econ);
            Econ.Close();
            oda.Fill(ds);


            DataTable Exceldt = ds.Tables[0];
            Exceldt.Columns.Add("RequestId", typeof(Int64));
            Exceldt.Columns.Add("CreatedBy", typeof(string));
            Exceldt.Columns.Add("CreatedDate", typeof(DateTime));
            Exceldt.Columns.Add("ValidStatus", typeof(string));
            foreach (DataRow dr in Exceldt.Rows)
            {

                dr["CreatedBy"] = loggedInUserId;
                dr["CreatedDate"] = DateTime.Now;

            }

            string sqlconn = ConfigurationManager.ConnectionStrings["EntLibConnString"].ConnectionString;
            SqlConnection con = new SqlConnection(sqlconn);
            //creating object of SqlBulkCopy    
            SqlBulkCopy objbulk = new SqlBulkCopy(con);
            //assigning Destination table name    
            objbulk.DestinationTableName = "Orders";
            //Mapping Table column Add("Excelcolumn","Tablecolumn")    
            objbulk.ColumnMappings.Add("ControlId", "ControlId");
            //objbulk.ColumnMappings.Add("Revision", "Revision");
            //objbulk.ColumnMappings.Add("Description", "Description");
            //objbulk.ColumnMappings.Add("RequestId", "RequestId");
            //objbulk.ColumnMappings.Add("Order Date", "OrderDate");
            //objbulk.ColumnMappings.Add("Status", "Status");
            //objbulk.ColumnMappings.Add("Vendor", "Vendor");
            //objbulk.ColumnMappings.Add("Vendor Name", "VendorName");
            //objbulk.ColumnMappings.Add("Currency", "Currency");
            //objbulk.ColumnMappings.Add("PO Line #", "POLine");
            //objbulk.ColumnMappings.Add("Item", "UNCode");
            //objbulk.ColumnMappings.Add("Item Description", "Commodity");
            //objbulk.ColumnMappings.Add("UOM", "UOM");
            //objbulk.ColumnMappings.Add("Ordered Qty", "OrderQty");
            //objbulk.ColumnMappings.Add("Remaining Qty", "RemainingQty");
            //objbulk.ColumnMappings.Add("Shipped Qty", "ShippedQty");
            //objbulk.ColumnMappings.Add("Shipped Weight", "ShippedWeight");
            //objbulk.ColumnMappings.Add("Received Qty", "ReceivedQty");
            //objbulk.ColumnMappings.Add("Balance Open Order", "BalanceOpenOrder");
            //objbulk.ColumnMappings.Add("Unit Cost", "UnitCost");
            //objbulk.ColumnMappings.Add("Line Cost", "LineCost");
            //objbulk.ColumnMappings.Add("Receipts Complete", "ReceiptsComplete");
            //objbulk.ColumnMappings.Add("Pack Size", "PackSize");
            //objbulk.ColumnMappings.Add("Freight Terms", "FreightTerms");
            //objbulk.ColumnMappings.Add("Payment Terms", "PaymentTerms");
            //objbulk.ColumnMappings.Add("CreatedBy", "CreatedBy");
            //objbulk.ColumnMappings.Add("CreatedDate", "CreatedDate");
            //objbulk.ColumnMappings.Add("ValidStatus", "ValidStatus");
            con.Open();
            objbulk.WriteToServer(Exceldt);
            con.Close();
            upreq.UploadStatus = "Uploaded Successfully";

            //    supplierservice.InsertPOListbyStoredProcedure(upreq.RequestId);

            // orderService.SaveOrUpdateUploadRequest(upreq);

        }



        #endregion

        #region Invoice number Master
        public ActionResult InvoiceNumberMasterUpload()
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult InvoiceNumberMasterUpload(HttpPostedFileBase uploadedFile, string PeriodYear, string Period, string Week, string RequestName)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    Dictionary<string, object> criteria = new Dictionary<string, object>();
                    if (!string.IsNullOrWhiteSpace(PeriodYear)) { criteria.Add("PeriodYear", PeriodYear); }
                    if (!string.IsNullOrWhiteSpace(Period)) { criteria.Add("Period", Period); }
                    if (!string.IsNullOrWhiteSpace(Week)) { criteria.Add("Week", Convert.ToInt64(Week)); }
                    criteria.Add("Category", "INVOICENUMBERMASTERUPLOAD");
                    criteria.Add("UploadStatus", "Uploaded Successfully");
                    Dictionary<long, IList<UploadRequest>> uploadreqlist = orderService.GetUploadRequestCountListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                    if (uploadreqlist.FirstOrDefault().Key == 0)
                    {
                        //Creating the upload request for the InvoiceNumber master upload
                        UploadRequest upreq = new UploadRequest();
                        upreq.Category = "INVOICENUMBERMASTERUPLOAD";
                        upreq.CreatedBy = userId;
                        upreq.CreatedDate = DateTime.Now;
                        upreq.RequestName = RequestName;
                        upreq.Period = Period;
                        upreq.PeriodYear = PeriodYear;
                        upreq.Week = Convert.ToInt64(Week);
                        long reqid = orderService.SaveOrUpdateUploadRequest(upreq);
                        upreq.RequestNo = "REQUEST-" + reqid;
                        upreq.UploadStatus = "InProgress";
                        orderService.SaveOrUpdateUploadRequest(upreq);
                        try
                        {
                            HttpPostedFileBase theFile = HttpContext.Request.Files["uploadedFile"];
                            StringBuilder alreadyExists = new StringBuilder();
                            StringBuilder ErrorFilename = new StringBuilder();
                            StringBuilder UploadedFilename = new StringBuilder();
                            if (theFile != null && theFile.ContentLength > 0)
                            {
                                string fileName = string.Empty;
                                string path = uploadedFile.InputStream.ToString();
                                byte[] imageSize = new byte[uploadedFile.ContentLength];
                                uploadedFile.InputStream.Read(imageSize, 0, (int)uploadedFile.ContentLength);
                                string UploadConnStr = "";
                                fileName = uploadedFile.FileName;
                                string fileExtn = Path.GetExtension(uploadedFile.FileName);
                                string fileLocation = ConfigurationManager.AppSettings["InvoiceNumberMasterFilePath"].ToString() + fileName;
                                uploadedFile.SaveAs(fileLocation);
                                if (fileExtn == ".xlsx")
                                {
                                    UploadConnStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";
                                }
                                DataTable DtblXcelData = new DataTable();
                                OleDbConnection conn = new OleDbConnection();
                                OleDbDataAdapter DtAdptrr = new OleDbDataAdapter();
                                string QeryToGetXcelData = "select * from " + string.Format("{0}${1}", "[Invoice Template", "A1:AZ]");
                                OleDbCommand cmd = new OleDbCommand(QeryToGetXcelData, conn);
                                conn.ConnectionString = UploadConnStr;
                                conn.Open();
                                cmd = new OleDbCommand(QeryToGetXcelData, conn);
                                cmd.CommandType = CommandType.Text;
                                DtAdptrr = new OleDbDataAdapter();
                                DtAdptrr.SelectCommand = cmd;
                                DtAdptrr.Fill(DtblXcelData);
                                string[] strArray = { "S#No", "ControlId", "InvoiceId" };
                                char chrFlag = 'Y';
                                if (DtblXcelData.Columns.Count == strArray.Length)
                                {
                                    int j = 0;
                                    string[] strColumnsAray = new string[DtblXcelData.Columns.Count];
                                    foreach (DataColumn dtColumn in DtblXcelData.Columns)
                                    {
                                        strColumnsAray[j] = dtColumn.ColumnName;
                                        j++;
                                    }
                                    for (int i = 0; i < strArray.Length - 1; i++)
                                    {
                                        if (strArray[i].Trim() != strColumnsAray[i].Trim())
                                        {
                                            chrFlag = 'N';
                                            break;
                                        }
                                    }
                                    if (chrFlag == 'Y')
                                    {
                                        IList<InvoiceNumberMaster> invNolist = new List<InvoiceNumberMaster>();

                                        foreach (DataRow invMstline in DtblXcelData.Rows)
                                        {

                                            if (invMstline["ControlId"].ToString() != "")
                                            {
                                                InvoiceNumberMaster invMst = new InvoiceNumberMaster();

                                                string controlId = invMstline["ControlId"].ToString();
                                                string[] controlIdArray = controlId.Split('-');

                                                invMst.ControlId = controlId;
                                                invMst.Period = Period;
                                                invMst.PeriodYear = PeriodYear;
                                                invMst.Week = Convert.ToInt64(Week);
                                                if (invMstline["InvoiceId"].ToString().Trim() != "")
                                                    invMst.InvoiceNumber = invMstline["InvoiceId"].ToString();

                                                invMst.CreatedBy = userId;
                                                invMst.CreatedDate = DateTime.Now;
                                                invMst.RequestId = upreq.RequestId;
                                                Orders ord = orderService.GetOrderByControlId(invMst.ControlId);
                                                if (ord != null)
                                                {
                                                    invMst.OrderId = ord.OrderId;
                                                    invMst.IsValid = "VALID";
                                                }
                                                else
                                                    invMst.IsValid = "INVALID";

                                                invNolist.Add(invMst);
                                            }
                                        }
                                        orderService.SaveOrUpdateInvoiceNumberMaster(invNolist);
                                        upreq.UploadStatus = "Uploaded Successfully";
                                        orderService.SaveOrUpdateUploadRequest(upreq);
                                    }
                                }
                            }
                            return Json(new { success = true, result = "InvoiceNumber Master uploaded successfully." }, "text/html", JsonRequestBehavior.AllowGet);
                        }
                        catch (Exception ex)
                        {
                            upreq.ErrorDesc = ex.ToString();
                            upreq.UploadStatus = "Upload Failed";
                            orderService.SaveOrUpdateUploadRequest(upreq);
                            return Json(new { success = true, result = "InvoiceNumber Master updated Failed." }, "text/html", JsonRequestBehavior.AllowGet);
                            //ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                            //throw ex;
                        }
                    }
                    return Json(new { success = false, result = "InvoiceNumber Master Already exists." }, "text/html", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }

        }

        //InvoiceNumber master upload request jqgrid
        public JsonResult InvoiceNumberMasterUploadRequestJqGrid(string Category, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                Dictionary<string, object> criteria = new Dictionary<string, object>();

                sord = sord == "desc" ? "Desc" : "Asc";
                if (!string.IsNullOrWhiteSpace(Category)) { criteria.Add("Category", Category); }
                Dictionary<long, IList<UploadRequest>> uploadreqlist = orderService.GetUploadRequestCountListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                if (uploadreqlist != null && uploadreqlist.Count > 0)
                {
                    long totalrecords = uploadreqlist.First().Key;
                    int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                    var jsondat = new
                    {
                        total = totalPages,
                        page = page,
                        records = totalrecords,

                        rows = (from items in uploadreqlist.First().Value
                                select new
                                {
                                    i = 2,
                                    cell = new string[] {
                                        items.RequestId.ToString(),
                                        items.RequestName,
                                        items.Period,
                                        items.PeriodYear,
                                        items.Week.ToString(),
                                        items.RequestNo,
                                        items.Category,
                                        items.Status,
                                        items.CreatedBy,
                                        items.CreatedDate!=null? ConvertDateTimeToDate(items.CreatedDate.ToString("dd/MM/yyyy"),"en-GB"):"",
                                        items.UploadStatus
                                                             
                            }
                                })
                    };
                    return Json(jsondat, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(null, JsonRequestBehavior.AllowGet);

                }


            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }
        //InvoiceNumber master form page
        public ActionResult InvoiceNumberMasterDetails(long RequestId)
        {
            UploadRequest upreq = orderService.GetUploadRequestbyRequestId(RequestId);
            return View(upreq);
        }
        //Invoicenumber master jqgrid
        public ActionResult InvoiceNumberMasterListJqGrid(long RequestId, string ControlId, string ExptType, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                Dictionary<string, object> likeSearchCriteria = new Dictionary<string, object>();

                sord = sord == "desc" ? "Desc" : "Asc";

                criteria.Add("RequestId", RequestId);
                Dictionary<long, IList<InvoiceNumberMaster>> invMstlist = null;
                if (!string.IsNullOrWhiteSpace(ControlId))
                {
                    criteria.Add("ControlId", ControlId);
                    invMstlist = orderService.GetInvoiceNumberMasterListWithLikeSearchPagingAndCriteria(page - 1, 9999, sidx, sord, criteria);
                }
                else
                {
                    invMstlist = orderService.GetInvoiceNumberMasterListWithPagingAndCriteria(page - 1, 9999, sidx, sord, criteria, likeSearchCriteria);
                }
                if (invMstlist != null && invMstlist.Count > 0)
                {
                    if (ExptType == "Excel")
                    {
                        int i = 1;
                        foreach (var item in invMstlist.FirstOrDefault().Value)
                        {
                            //item.Value.FirstOrDefault().InvoiceMasterId = i;
                            item.InvoiceMasterId = i;
                            i = i + 1;
                        }
                        var List = invMstlist.First().Value.ToList();
                        List<string> lstHeader = new List<string>() { "SNo", "ControlId", "InvoiceNumber" };
                        base.NewExportToExcel(List, "Invoice Template", (items => new
                        {
                            items.InvoiceMasterId,
                            items.ControlId,
                            items.InvoiceNumber,
                        }), lstHeader);
                        return new EmptyResult();
                    }
                    else
                    {
                        long totalrecords = invMstlist.First().Key;
                        int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                        var jsondat = new
                        {
                            total = totalPages,
                            page = page,
                            records = totalrecords,
                            rows = (from items in invMstlist.First().Value
                                    select new
                                    {
                                        i = 2,
                                        cell = new string[] {
                                        items.InvoiceMasterId.ToString(),
                                        items.ControlId,
                                       items.InvoiceNumber,
                                       items.PeriodYear,
                                        items.Period,
                                        items.Week.ToString(),
                                        items.IsValid,
                                        items.OrderId.ToString(),
                                        items.RequestId.ToString(),
                                        items.CreatedBy,
                                        items.CreatedDate.ToString(),
                                        items.ModifiedBy,
                                        items.ModifiedDate.ToString()
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
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }
        public JsonResult DeleteInvoiceNumberMaster(string RequestIds)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return null;
                else
                {
                    int successCount = 0;
                    int failureCount = 0;
                    Dictionary<string, object> criteria = new Dictionary<string, object>();
                    long[] RequestIdArray = RequestIds.Split(',').Select(long.Parse).ToArray();
                    if (RequestIdArray.Count() > 0)
                    {
                        for (int i = 0; i < RequestIdArray.Count(); i++)
                        {
                            UploadRequest requestObj = orderService.GetUploadRequestbyRequestId(RequestIdArray[i]);
                            criteria.Clear();
                            criteria.Add("Period", requestObj.Period);
                            criteria.Add("PeriodYear", requestObj.PeriodYear);
                            criteria.Add("Week", requestObj.Week);
                            Dictionary<long, IList<InvoiceManagementView>> invoiceItems = IS.GetInvoiceListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                            if (invoiceItems.FirstOrDefault().Key == 0)
                            {
                                //Delete the uploadrequest 
                                orderService.DeleteInvoiceSequenceandUploadRequestbyRequestId(RequestIdArray[i]);
                                successCount = successCount + 1;
                            }
                            else
                            {
                                failureCount = failureCount + 1;
                            }
                        }
                    }
                    string str = successCount + "/" + failureCount;
                    return Json(str, JsonRequestBehavior.AllowGet);
                    //string[] PeriodArray = (from period in requestList select period.Period).Distinct().ToArray();
                    //string[] PeriodyearArray = (from periodyear in requestList select periodyear.PeriodYear).Distinct().ToArray();
                    //long[] WeekArray = (from week in requestList select week.Week).Distinct().ToArray();
                    //criteria.Clear();
                    //criteria.Add("Period", PeriodArray);
                    //criteria.Add("PeriodYear", PeriodyearArray);
                    //criteria.Add("Week", WeekArray);
                    //Dictionary<long, IList<InvoiceManagementView>> invoiceItems = IS.GetInvoiceListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                    //if (invoiceItems.FirstOrDefault().Key == 0)
                    //{
                    //    //Delete the uploadrequest 
                    //    orderService.DeleteInvoiceSequenceandUploadRequestbyRequestId(RequestIdArray);
                    //    return Json(new { success = true }, "text/html", JsonRequestBehavior.AllowGet);
                    //}
                    //else
                    //{
                    //    return Json(new { success = false }, "text/html", JsonRequestBehavior.AllowGet);
                    //}
                    //var InvoiceList = (from u in invoiceItems.FirstOrDefault().Value select new { u.Period, u.PeriodYear, u.Week }).Distinct().ToArray();
                    //return Json(new { success = true }, "text/html", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }
        #endregion
        #region NPA Master Upload
        public ActionResult NPAMasterUpload()
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult NPAMasterUpload(HttpPostedFileBase uploadedFile, UploadRequestModal request)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    Dictionary<string, object> criteria = new Dictionary<string, object>();
                    //if (!string.IsNullOrWhiteSpace(request.PeriodYear)) { criteria.Add("PeriodYear", request.PeriodYear); }
                    //if (!string.IsNullOrWhiteSpace(request.Period)) { criteria.Add("Period", request.Period); }
                    //if (request.Week > 0) { criteria.Add("Week", request.Week); }
                    //criteria.Add("Category", "NPAMASTERUPLOAD");
                    //criteria.Add("UploadStatus", "Uploaded Successfully");
                    //Dictionary<long, IList<UploadRequest>> uploadreqlist = orderService.GetUploadRequestCountListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                    //if (uploadreqlist.FirstOrDefault().Key == 0)
                    //{
                    //Creating the upload request for the InvoiceNumber master upload
                    UploadRequest upreq = new UploadRequest();
                    upreq.Category = "NPAMASTERUPLOAD";
                    upreq.CreatedBy = userId;
                    upreq.CreatedDate = DateTime.Now;
                    upreq.RequestName = request.RequestName;
                    if (!string.IsNullOrEmpty(request.Period))
                        upreq.Period = request.Period;
                    if (!string.IsNullOrEmpty(request.PeriodYear))
                        upreq.PeriodYear = request.PeriodYear;
                    if (request.Week > 0)
                        upreq.Week = request.Week;
                    long reqid = orderService.SaveOrUpdateUploadRequest(upreq);
                    upreq.RequestNo = "REQUEST-" + reqid;
                    upreq.UploadStatus = "InProgress";
                    orderService.SaveOrUpdateUploadRequest(upreq);
                    try
                    {
                        HttpPostedFileBase theFile = HttpContext.Request.Files["uploadedFile"];
                        StringBuilder alreadyExists = new StringBuilder();
                        StringBuilder ErrorFilename = new StringBuilder();
                        StringBuilder UploadedFilename = new StringBuilder();
                        if (theFile != null && theFile.ContentLength > 0)
                        {
                            string fileName = string.Empty;
                            string path = uploadedFile.InputStream.ToString();
                            byte[] imageSize = new byte[uploadedFile.ContentLength];
                            uploadedFile.InputStream.Read(imageSize, 0, (int)uploadedFile.ContentLength);
                            string UploadConnStr = "";
                            fileName = uploadedFile.FileName;
                            string fileExtn = Path.GetExtension(uploadedFile.FileName);
                            string fileLocation = ConfigurationManager.AppSettings["NPAMasterFilePath"].ToString() + fileName;
                            uploadedFile.SaveAs(fileLocation);
                            if (fileExtn == ".xlsx")
                            {
                                UploadConnStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";
                            }
                            DataTable DtblXcelData = new DataTable();
                            OleDbConnection conn = new OleDbConnection();
                            OleDbDataAdapter DtAdptrr = new OleDbDataAdapter();
                            string QeryToGetXcelData = "select * from " + string.Format("{0}${1}", "[NPA TEMPLATE", "A1:AZ]");
                            OleDbCommand cmd = new OleDbCommand(QeryToGetXcelData, conn);
                            conn.ConnectionString = UploadConnStr;
                            conn.Open();
                            cmd = new OleDbCommand(QeryToGetXcelData, conn);
                            cmd.CommandType = CommandType.Text;
                            DtAdptrr = new OleDbDataAdapter();
                            DtAdptrr.SelectCommand = cmd;
                            DtAdptrr.Fill(DtblXcelData);
                            string[] strArray = { "S#No", "Delivery Note #", "Control ID", "UNRS Code", "NPA Code" };
                            char chrFlag = 'Y';
                            if (DtblXcelData.Columns.Count == strArray.Length)
                            {
                                int j = 0;
                                string[] strColumnsAray = new string[DtblXcelData.Columns.Count];
                                foreach (DataColumn dtColumn in DtblXcelData.Columns)
                                {
                                    strColumnsAray[j] = dtColumn.ColumnName;
                                    j++;
                                }
                                for (int i = 0; i < strArray.Length - 1; i++)
                                {
                                    if (strArray[i].Trim() != strColumnsAray[i].Trim())
                                    {
                                        chrFlag = 'N';
                                        break;
                                    }
                                }
                                if (chrFlag == 'Y')
                                {
                                    IList<NPAMaster> NPAlist = new List<NPAMaster>();

                                    foreach (DataRow NPAMstline in DtblXcelData.Rows)
                                    {

                                        if (NPAMstline["Control ID"].ToString() != "")
                                        {
                                            NPAMaster NPAObj = new NPAMaster();

                                            string controlId = NPAMstline["Control ID"].ToString();
                                            string[] controlIdArray = controlId.Split('-');

                                            NPAObj.ControlId = controlId;
                                            NPAObj.PeriodYear = controlIdArray[7].ToString();
                                            NPAObj.Period = controlIdArray[5].ToString();
                                            NPAObj.Week = Convert.ToInt64(controlIdArray[6].Replace("WK0", "").ToString());

                                            if (NPAMstline["Delivery Note #"].ToString().Trim() != "")
                                                NPAObj.DeliveryNoteName = NPAMstline["Delivery Note #"].ToString();

                                            if (NPAMstline["UNRS Code"].ToString().Trim() != "")
                                                NPAObj.UNRSCode = Convert.ToInt64(NPAMstline["UNRS Code"].ToString());

                                            if (NPAMstline["NPA Code"].ToString().Trim() != "")
                                                NPAObj.NPACode = NPAMstline["NPA Code"].ToString();

                                            NPAObj.IsValid = "VALID";

                                            NPAObj.CreatedBy = userId;
                                            NPAObj.CreatedDate = DateTime.Now;
                                            NPAObj.RequestId = upreq.RequestId;
                                            NPAlist.Add(NPAObj);
                                        }
                                    }
                                    orderService.SaveOrUpdateNPAMasterList(NPAlist);
                                    if (NPAlist != null && NPAlist.Count > 0)
                                    {
                                        orderService.UpdateNPAMasterSpbyRequestId(upreq.RequestId);
                                    }
                                    upreq.UploadStatus = "Uploaded Successfully";
                                    orderService.SaveOrUpdateUploadRequest(upreq);
                                }
                            }
                        }
                        return Json(new { success = true, result = "NPA Master uploaded successfully." }, "text/html", JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        upreq.ErrorDesc = ex.ToString();
                        upreq.UploadStatus = "Upload Failed";
                        orderService.SaveOrUpdateUploadRequest(upreq);
                        return Json(new { success = true, result = "NPA Master updated Failed." }, "text/html", JsonRequestBehavior.AllowGet);
                    }
                    //}
                    //return Json(new { success = false, result = "NPA Master Already exists." }, "text/html", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }
        //NPA master upload request jqgrid
        public JsonResult NPAMasterUploadRequestJqGrid(string Category, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                Dictionary<string, object> criteria = new Dictionary<string, object>();

                sord = sord == "desc" ? "Desc" : "Asc";
                if (!string.IsNullOrWhiteSpace(Category)) { criteria.Add("Category", Category); }
                Dictionary<long, IList<UploadRequest>> uploadreqlist = orderService.GetUploadRequestCountListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                if (uploadreqlist != null && uploadreqlist.Count > 0)
                {
                    long totalrecords = uploadreqlist.First().Key;
                    int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                    var jsondat = new
                    {
                        total = totalPages,
                        page = page,
                        records = totalrecords,

                        rows = (from items in uploadreqlist.First().Value
                                select new
                                {
                                    i = 2,
                                    cell = new string[] {
                                        items.RequestId.ToString(),
                                        items.RequestName,
                                        items.Period,
                                        items.PeriodYear,
                                        items.Week.ToString(),
                                        items.RequestNo,
                                        items.Category,
                                        items.Status,
                                        items.CreatedBy,
                                        items.CreatedDate!=null? ConvertDateTimeToDate(items.CreatedDate.ToString("dd/MM/yyyy"),"en-GB"):"",
                                        items.UploadStatus
                                                             
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
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }
        //InvoiceNumber master form page
        public ActionResult GetNPAMasterList(long RequestId)
        {
            UploadRequest upreq = orderService.GetUploadRequestbyRequestId(RequestId);
            return View(upreq);
        }
        public JsonResult GetNPAMasterListJqGrid(long RequestId, string ControlId, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                Dictionary<string, object> likeSearchCriteria = new Dictionary<string, object>();

                sord = sord == "desc" ? "Desc" : "Asc";

                criteria.Add("RequestId", RequestId);
                Dictionary<long, IList<NPAMaster>> NPAMstlist = null;
                if (!string.IsNullOrWhiteSpace(ControlId))
                {
                    criteria.Add("ControlId", ControlId);
                    NPAMstlist = orderService.GetNPAMasterListWithLikeSearchPagingAndCriteria(page - 1, 9999, sidx, sord, criteria);
                }
                else
                {
                    NPAMstlist = orderService.GetNPAMasterListWithPagingAndCriteria(page - 1, 9999, sidx, sord, criteria, likeSearchCriteria);
                }
                if (NPAMstlist != null && NPAMstlist.Count > 0)
                {
                    long totalrecords = NPAMstlist.First().Key;
                    int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                    var jsondat = new
                    {
                        total = totalPages,
                        page = page,
                        records = totalrecords,
                        rows = (from items in NPAMstlist.First().Value
                                select new
                                {
                                    i = 2,
                                    cell = new string[] {
                                        items.NPAMasterId.ToString(),
                                        items.ControlId,
                                       items.DeliveryNoteName,
                                       items.UNRSCode.ToString(),
                                       items.NPACode,
                                       items.PeriodYear,
                                        items.Period,
                                        items.Week.ToString(),
                                        items.IsValid,
                                        items.OrderId.ToString(),
                                        items.RequestId.ToString(),
                                        items.CreatedBy,
                                        items.CreatedDate.ToString(),
                                        items.ModifiedBy,
                                        items.ModifiedDate.ToString()
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
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }
        public JsonResult DeleteNPAMasterByRequestId(string RequestIds)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return null;
                else
                {
                    int successCount = 0;
                    int failureCount = 0;
                    Dictionary<string, object> criteria = new Dictionary<string, object>();
                    long[] RequestIdArray = RequestIds.Split(',').Select(long.Parse).ToArray();
                    if (RequestIdArray.Count() > 0)
                    {
                        for (int i = 0; i < RequestIdArray.Count(); i++)
                        {
                            IList<NPAMaster> requestObj = orderService.GetNPAMasterListbyRequestId(RequestIdArray[i]);
                            var OrderIdArr = (from u in requestObj select u.OrderId).Distinct().ToArray();
                            criteria.Clear();
                            criteria.Add("OrderId", OrderIdArr);
                            Dictionary<long, IList<PDFDocuments>> invoiceItems = IS.GetPDFDocumentListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                            //Dictionary<long, IList<InvoiceManagementView>> invoiceItems = IS.GetInvoiceListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                            if (invoiceItems.FirstOrDefault().Key == 0)
                            {
                                //Delete the uploadrequest 
                                orderService.DeleteNPAMasterandUploadRequestbyRequestId(RequestIdArray[i]);
                                successCount = successCount + 1;
                            }
                            else
                            {
                                failureCount = failureCount + 1;
                            }
                        }
                    }
                    string str = successCount + "/" + failureCount;
                    return Json(str, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }
        #endregion
        #region FIV Upload
        public ActionResult FIVBulkuploadRequestCreation(long? RequestId)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    UploadRequest upreq = new UploadRequest();
                    if (RequestId != null)
                    {
                        upreq = orderService.GetUploadRequestbyRequestId(RequestId ?? 0);

                    }
                    return View(upreq);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }
        /// <summary>
        /// FIV New request creation for the given Request Name
        /// </summary>
        /// <param name="upreq"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult FIVBulkuploadRequestCreation(UploadRequest upreq)
        {
            try
            {
                INSIGHT.Entities.User Userobj = (INSIGHT.Entities.User)Session["objUser"];
                if (Userobj == null || (Userobj != null && Userobj.UserId == null))
                { return RedirectToAction("LogOn", "Account"); }
                string loggedInUserId = Userobj.UserId;
                upreq.Category = "FIVUPLOAD";
                upreq.CreatedBy = loggedInUserId;
                upreq.CreatedDate = DateTime.Now;
                long reqid = orderService.SaveOrUpdateUploadRequest(upreq);
                upreq.RequestNo = "REQUEST-" + reqid;
                upreq.Status = "OPEN";
                upreq.UploadStatus = "Request Generated";
                orderService.SaveOrUpdateUploadRequest(upreq);
                return (RedirectToAction("FIVBulkuploadRequestCreation", new { RequestId = upreq.RequestId }));
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }
        public ActionResult FIVBulkUploadDetails(long RequestId)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    ViewBag.RequestId = RequestId;
                    UploadRequest upreq = orderService.GetUploadRequestbyRequestId(RequestId);
                    return View(upreq);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }
        /// <summary>
        /// Upload the FIV files to FIVUploadRequest file path one by one
        /// </summary>
        /// <param name="RequestId"></param>
        /// <returns></returns>
        public ActionResult FIVBulkUpload(long RequestId)
        {
            string RequestNo = "REQUEST-" + RequestId;
            var files = Request.Files["Filedata"];
            string curpath = ConfigurationManager.AppSettings["FIVUploadRequestFilePath"].ToString();
            string RequestFolder = String.Format("{0}\\" + RequestNo, curpath);
            // If the folder is not existed, create it.
            if (!Directory.Exists(RequestFolder)) { Directory.CreateDirectory(RequestFolder); }
            string savePath = RequestFolder + "\\" + files.FileName;
            files.SaveAs(savePath);
            return Content(Url.Content(@"~\Content\" + files.FileName));
        }
        /// <summary>
        /// Initiating parallel tasking after file upload queue completed
        /// </summary>
        /// <param name="RequestId"></param>
        public void InitiateFIVParallelCall(long RequestId)
        {
            new Task(() =>
            {
                ReadAndInsertFIVItemsbyRequestId(RequestId);
            }).Start();
        }
        public void ReadAndInsertFIVItemsbyRequestId(long RequestId)
        {
            try
            {
                string server = ConfigurationManager.AppSettings["InstanceName"].ToString();
                string database = ConfigurationManager.AppSettings["DatabaseName"].ToString();

                string UploadConnStr = string.Empty;
                StringBuilder alreadyExists = new StringBuilder();
                StringBuilder ErrorFilename = new StringBuilder();
                StringBuilder UploadedFilename = new StringBuilder();
                UploadRequestDetailsLog uploadlog = new UploadRequestDetailsLog();
                string fileExtn = string.Empty;
                UploadRequest upreq = new UploadRequest();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                if (RequestId > 0)
                {
                    upreq = orderService.GetUploadRequestbyRequestId(RequestId);
                    if (upreq.UploadStatus == "Request Generated")
                    {
                        upreq.UploadStatus = "InProgress";
                        orderService.SaveOrUpdateUploadRequest(upreq);
                    }
                }
                string curpath = ConfigurationManager.AppSettings["FIVUploadRequestFilePath"].ToString();
                //string server = "ADMIN-PC\\THAMIZH";
                //string database = "INSIGHT_SUDAN_eRMS_Live";
                //string server = "WIN-907F2IJ90N4\\SQLEXPRESS";
                //string database = "INSIGHT_SUDAN_BOERMS";
                string SQLServerConnectionString = String.Format("Data Source={0};Initial Catalog={1};Integrated Security=SSPI", server, database);
                try
                {
                    string CSVpath = @curpath + "\\REQUEST-" + RequestId; // CSV file Path
                    //string CSVFileConnectionString = String.Format("Provider=Microsoft.Jet.OLEDB.12.0;Data Source={0};;Extended Properties=\"text;HDR=Yes;FMT=Delimited\";", CSVpath);
                    string CSVFileConnectionString = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};;Extended Properties=\"text;HDR=Yes;FMT=Delimited\";", CSVpath);
                    //"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";
                    var AllFiles = new DirectoryInfo(CSVpath).GetFiles("*.CSV");
                    string File_Name = string.Empty;
                    foreach (var file in AllFiles)
                    {
                        try
                        {
                            string AlreadyExistFlag = string.Empty;
                            string fileName = CSVpath + "\\" + file.Name.ToString();
                            //Create or update the  saving the individual file status in uploadrequestdetailslog table
                            long UploadReqDetLogId = CreateOrUpdateRequest(fileName, RequestId);
                            uploadlog = orderService.GetUploadRequestDetailsLogbyRequestId(UploadReqDetLogId);
                            string[] FIVName = file.Name.Split('.').ToArray();
                            string ControlId = FIVName[0].Replace("FIV", "FFO").ToString();
                            Orders ord = orderService.GetOrderByControlId(ControlId);
                            FIVItems fiv = new FIVItems();
                            fiv = orderService.GetFIVItemsByOrderId(ord.OrderId);
                            if (ord != null && ord.OrderId > 0 && fiv == null)
                            {
                                DataTable dt = new DataTable();
                                using (OleDbConnection con = new OleDbConnection(CSVFileConnectionString))
                                {
                                    con.Open();
                                    var csvQuery = string.Format("select * from [{0}]", file.Name);
                                    using (OleDbDataAdapter da = new OleDbDataAdapter(csvQuery, con))
                                    {
                                        da.Fill(dt);
                                        //DataTable Exceldt = dt;
                                        dt.Columns.Add("ControlId", typeof(string));
                                        dt.Columns.Add("OrderId", typeof(Int64));
                                        dt.Columns.Add("RequestId", typeof(Int64));
                                        dt.Columns.Add("CreatedBy", typeof(string));
                                        dt.Columns.Add("CreatedDate", typeof(DateTime));
                                        dt.Columns.Add("IsValid", typeof(string));
                                        dt.Columns.Add("FIVStatus", typeof(string));
                                        foreach (DataRow dr in dt.Rows)
                                        {
                                            dr["ControlId"] = ControlId;
                                            dr["OrderId"] = ord.OrderId;
                                            dr["RequestId"] = RequestId;
                                            dr["CreatedBy"] = upreq.CreatedBy;
                                            dr["CreatedDate"] = DateTime.Now;
                                            dr["IsValid"] = "VALID";
                                            dr["FIVStatus"] = "OPEN";
                                        }
                                    }
                                }

                                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(SQLServerConnectionString))
                                {
                                    bulkCopy.ColumnMappings.Add("OrderId", "OrderId");
                                    bulkCopy.ColumnMappings.Add("ControlId", "ControlId");
                                    bulkCopy.ColumnMappings.Add("UNRS_Code", "UNCode");
                                    bulkCopy.ColumnMappings.Add("UNRS_Code(S)", "SubstituteItemCode");
                                    bulkCopy.ColumnMappings.Add("Ordered_Quantity", "OrderedQuantity");
                                    bulkCopy.ColumnMappings.Add("Sent_Quantity", "DeliveredQuantity");
                                    bulkCopy.ColumnMappings.Add("Accepted_Quantity", "AcceptedQuantity");
                                    bulkCopy.ColumnMappings.Add("Unit_Food_Price", "UnitPrice");
                                    bulkCopy.ColumnMappings.Add("Total_Price", "TotalPrice");
                                    bulkCopy.ColumnMappings.Add("RequestId", "RequestId");
                                    bulkCopy.ColumnMappings.Add("IsValid", "IsValid");
                                    bulkCopy.ColumnMappings.Add("CreatedBy", "CreatedBy");
                                    bulkCopy.ColumnMappings.Add("CreatedDate", "CreatedDate");
                                    bulkCopy.ColumnMappings.Add("FIVStatus", "FIVStatus");
                                    bulkCopy.DestinationTableName = "FIVItems";
                                    bulkCopy.BatchSize = 0;
                                    bulkCopy.WriteToServer(dt);
                                    bulkCopy.Close();
                                }
                                if (uploadlog.UploadStatus == "YetToUpload")
                                    uploadlog.UploadStatus = "UploadedSuccessfully";
                                uploadlog.ReferenceNo = ord.OrderId.ToString();
                                orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);
                                orderService.UpdateLineIdinFIVItems_SPByOrderId(ord.OrderId);

                            }
                            else
                            {
                                if (uploadlog.UploadStatus == "YetToUpload")
                                    uploadlog.UploadStatus = "UploadFailed";
                                if (ord == null)
                                    uploadlog.ErrDesc = "Order Not exists for the FIV";
                                if (fiv != null && fiv.OrderId > 0)
                                    uploadlog.UploadStatus = "AlreadyExist";
                                orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);
                            }

                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (uploadlog.UploadStatus == "YetToUpload")
                        uploadlog.UploadStatus = "UploadFailed";
                    uploadlog.ErrDesc = ex.ToString();
                    orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);
                    ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                    throw ex;
                }
                criteria.Clear();
                StatusUpdateInUploadRequest(RequestId);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }
        /// <summary>
        /// Delete FIV Request and reports
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public JsonResult DeleteFIVDetails(string Id)
        {
            try
            {
                var RequestIdArray = Id.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                Dictionary<string, object> likeSearchCriteria = new Dictionary<string, object>();
                criteria.Clear();
                if (RequestIdArray.Count() > 0)
                {
                    foreach (var item in RequestIdArray)
                    {
                        //Delete the uploadrequest 
                        orderService.DeleteFIVandUploadRequestbyFIVId(Convert.ToInt64(item));
                    }
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion
    }
}






































