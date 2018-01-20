using INSIGHT.Entities;
using INSIGHT.Entities.InventoryManagementEntities;
using INSIGHT.Entities.InvoiceEntities;
using INSIGHT.Entities.Masters;
using INSIGHT.WCFServices;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace INSIGHT.Controllers
{
    public class InventoryManagementController : BaseController
    {
        IFormatProvider provider = new System.Globalization.CultureInfo("en-IN", true);
        //
        // GET: /InventoryManagement/

        public ActionResult Index()
        {
            return View();
        }
        #region Inventory related dropdown list
        public JsonResult FillPONumber()
        {
            try
            {
                string userid = (Session["UserId"] == null) ? "" : Session["UserId"].ToString();
                InventoryManagementService inventoryService = new InventoryManagementService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                Dictionary<long, IList<Inventory_PurchaseOrder>> POMasterList = inventoryService.GetPONumberWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                if (POMasterList != null && POMasterList.First().Value != null && POMasterList.First().Value.Count > 0)
                {
                    var POMasterCodeList = (
                             from items in POMasterList.First().Value
                             select new
                             {
                                 Text = items.PO,
                                 Value = items.PurchOrderId
                             }).Distinct().ToList().OrderBy(x => x.Text);
                    return Json(POMasterCodeList, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        public ActionResult FillPONumberddl()
        {
            try
            {
                string userid = (Session["UserId"] == null) ? "" : Session["UserId"].ToString();
                InventoryManagementService inventoryService = new InventoryManagementService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                Dictionary<long, string> appcd = new Dictionary<long, string>();
                Dictionary<long, IList<Inventory_PurchaseOrder>> POMasterList = inventoryService.GetPONumberWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                if (POMasterList != null && POMasterList.First().Value != null && POMasterList.First().Value.Count > 0)
                {
                    foreach (Inventory_PurchaseOrder curr in POMasterList.First().Value)
                    {
                        appcd.Add(curr.PurchOrderId, curr.PO.ToString());
                    }
                    return PartialView("SelectPartial", appcd);
                }
                else
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        public JsonResult GetInvoiceNumber(string term)
        {
            try
            {
                InventoryManagementService inventoryService = new InventoryManagementService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                criteria.Add("InvoiceNumber", term);
                Dictionary<long, IList<Inventory_PurchaseOrderInvoice>> poInvoiceList = inventoryService.GetPurchaseOrderInvoiceListWithPagingAndCriteriaLikeSearch(0, 9999, string.Empty, string.Empty, criteria);
                var InvoiceNumbers = (from u in poInvoiceList.First().Value
                                      where u.InvoiceNumber != null
                                      select new
                                      {
                                          Text = u.InvoiceNumber,
                                          Value = u.POInvoiceId
                                      }).Distinct().ToList().OrderBy(x => x.Value);
                return Json(InvoiceNumbers, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        #endregion
        #region POUpload
        public ActionResult POUpload(long? RequestId)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    OrdersService orderService = new OrdersService();
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

                throw ex;
            }
        }
        [HttpPost]
        public ActionResult POUpload(UploadRequest upreq)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    OrdersService orderService = new OrdersService();
                    upreq.Category = "POUPLOAD";
                    upreq.CreatedBy = userId;
                    upreq.CreatedDate = DateTime.Now;
                    long reqid = orderService.SaveOrUpdateUploadRequest(upreq);
                    upreq.RequestNo = "REQUEST-" + reqid;
                    upreq.Status = "OPEN";
                    upreq.UploadStatus = "Request Generated";
                    orderService.SaveOrUpdateUploadRequest(upreq);
                    return (RedirectToAction("POUpload", new { RequestId = upreq.RequestId }));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public ActionResult POBulkUpload(long RequestId)
        {
            string RequestNo = "REQUEST-" + RequestId;
            var files = Request.Files["Filedata"];
            string curpath = ConfigurationManager.AppSettings["POUploadRequestFilePath"].ToString();
            string RequestFolder = String.Format("{0}\\" + RequestNo, curpath);
            // If the folder is not existed, create it.
            if (!Directory.Exists(RequestFolder)) { Directory.CreateDirectory(RequestFolder); }
            string savePath = RequestFolder + "\\" + files.FileName;
            files.SaveAs(savePath);
            return Content(Url.Content(@"~\Content\" + files.FileName));
        }
        public void callParallel(long RequestId)
        {
            try
            {
                new Task(() =>
                {
                    CreateParallelPOUpload(RequestId);
                }).Start();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        #region POUpload
        public void CreateParallelPOUpload(long RequestId)
        {
            try
            {
                string server = ConfigurationManager.AppSettings["InstanceName"].ToString();
                string database = ConfigurationManager.AppSettings["DatabaseName"].ToString();
                OrdersService orderService = new OrdersService();
                InventoryManagementService inventoryService = new InventoryManagementService();
                UploadRequestDetailsLog uploadlog = new UploadRequestDetailsLog();
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
                string curpath = ConfigurationManager.AppSettings["POUploadRequestFilePath"].ToString();
                string SQLServerConnectionString = String.Format("Data Source={0};Initial Catalog={1};Integrated Security=SSPI", server, database);
                try
                {
                    string[] filePaths = Directory.GetFiles(curpath + "\\REQUEST-" + RequestId, "*.xlsx");
                    if (filePaths.Count() == 0) filePaths = Directory.GetFiles(curpath + "\\REQUEST-" + RequestId, "*.xls");
                    foreach (string s in filePaths)
                    {
                        try
                        {
                            string UploadConnStr = string.Empty;
                            string fileName = s.ToString();
                            byte[] UploadedFile = System.IO.File.ReadAllBytes(fileName);
                            //Create or update the  saving the individual file status in uploadrequestdetailslog table
                            long UploadReqDetLogId = CreateOrUpdateRequest(fileName, RequestId);
                            uploadlog = orderService.GetUploadRequestDetailsLogbyRequestId(UploadReqDetLogId);
                            UploadConnStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + s + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";
                            DataTable dt = new DataTable();
                            using (OleDbConnection con = new OleDbConnection(UploadConnStr))
                            {
                                con.Open();
                                //** Get sheet name dynamically by Thamizhmani
                                string sht = string.Empty;
                                string QeryToGetXcelData = string.Empty;
                                DataTable Sheets = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                foreach (DataRow dr in Sheets.Rows)
                                {
                                    sht = dr[2].ToString().Replace("$", "");
                                    sht = sht.Replace("'", "");
                                    QeryToGetXcelData = "select * from " + string.Format("{0}${1}", "[" + sht + "", "A1:AZ]");
                                    break;
                                }
                                using (OleDbDataAdapter da = new OleDbDataAdapter(QeryToGetXcelData, con))
                                {
                                    da.Fill(dt);
                                    dt.Columns.Add("RequestId", typeof(Int64));
                                    dt.Columns.Add("CreatedBy", typeof(string));
                                    dt.Columns.Add("CreatedDate", typeof(DateTime));
                                    dt.Columns.Add("IsValid", typeof(string));
                                    foreach (DataRow dr in dt.Rows)
                                    {
                                        dr["RequestId"] = RequestId;
                                        dr["CreatedBy"] = upreq.CreatedBy;
                                        dr["CreatedDate"] = DateTime.Now;
                                        dr["IsValid"] = "VALID";
                                    }
                                }
                            }
                            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(SQLServerConnectionString))
                            {
                                bulkCopy.ColumnMappings.Add("PO#", "PO");
                                bulkCopy.ColumnMappings.Add("PO Issued date", "POIssueddate");
                                bulkCopy.ColumnMappings.Add("Item Code", "UNCode");
                                bulkCopy.ColumnMappings.Add("ITEM DESCRIPTION", "Commodity");
                                bulkCopy.ColumnMappings.Add("PO Ordered Quantity", "OrderedQty");
                                bulkCopy.ColumnMappings.Add("PO Unit price", "POUnitPrice");
                                bulkCopy.ColumnMappings.Add("PO Value", "POValue");
                                bulkCopy.ColumnMappings.Add("PO Currency", "POCurrency");
                                bulkCopy.ColumnMappings.Add("Supplier", "Supplier");
                                bulkCopy.ColumnMappings.Add("Supplier Number", "SupplierNumber");

                                bulkCopy.ColumnMappings.Add("IsValid", "IsValid");
                                bulkCopy.ColumnMappings.Add("CreatedDate", "CreatedDate");
                                bulkCopy.ColumnMappings.Add("CreatedBy", "CreatedBy");
                                bulkCopy.ColumnMappings.Add("RequestId", "RequestId");

                                bulkCopy.DestinationTableName = "Inventory_POBulkupload_TEMP";
                                bulkCopy.BatchSize = 0;
                                bulkCopy.WriteToServer(dt);
                                bulkCopy.Close();
                            }
                            if (uploadlog.UploadStatus == "YetToUpload")
                                uploadlog.UploadStatus = "UploadedSuccessfully";
                            orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);
                            inventoryService.InsertPOMasterDetailsByRequestId_sp(RequestId);
                        }
                        catch (Exception ex)
                        {
                            if (uploadlog.UploadStatus == "YetToUpload")
                                uploadlog.UploadStatus = "UploadFailed";
                            uploadlog.ErrDesc = ex.ToString();
                            orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);
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
                    throw ex;
                }
                StatusUpdateInUploadRequest(RequestId);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }
        #endregion
        public ActionResult PurchaseOrder()
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

                throw ex;
            }
        }
        public JsonResult PurchaseOrderListJQGrid(Inventory_PurchaseOrder poMaster, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                InventoryManagementService inventryService = new InventoryManagementService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                Dictionary<string, object> likeCriteria = new Dictionary<string, object>();

                if (!string.IsNullOrWhiteSpace(sord) && sord == "desc")
                    sord = "Desc";
                else
                    sord = "Asc";

                if (poMaster != null)
                {
                    if (poMaster.PO > 0) criteria.Add("PO", poMaster.PO);
                }

                Dictionary<long, IList<Inventory_PurchaseOrder>> purchaseorderList = inventryService.GetPurchaseOrderDetailsListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria, likeCriteria);
                if (purchaseorderList != null && purchaseorderList.Count > 0)
                {
                    long totalrecords = purchaseorderList.First().Key;
                    int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                    var jsondat = new
                    {
                        total = totalPages,
                        page = page,
                        records = totalrecords,

                        rows = (from items in purchaseorderList.FirstOrDefault().Value
                                select new
                                {
                                    i = 2,
                                    cell = new string[] {
                                            items.PurchOrderId.ToString(),
                                            items.PO.ToString(),
                                            items.POIssuedDate!=null? items.POIssuedDate.Value.ToString("dd/MM/yyyy"):"",
                                            items.POCurrency,
                                            items.Supplier,
                                            items.SupplierNumber,

                                            items.CreatedDate!=null?items.CreatedDate.Value.ToString("dd/MM/yyyy"):"",
                                            items.CreatedBy,
                                            items.ModifiedDate!=null?items.ModifiedDate.Value.ToString("dd/MM/yyyy"):"",
                                            items.ModifiedBy,
                                            items.RequestId.ToString()
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

                throw ex;
            }
        }
        public ActionResult PODetailsList(long PurchOrderId)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    InventoryManagementService inventryService = new InventoryManagementService();
                    ViewBag.POId = PurchOrderId;
                    Inventory_PurchaseOrder purchaseOrder = inventryService.GetPurchaseOrderbyPurchaseOrderId(PurchOrderId);
                    return View(purchaseOrder);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public JsonResult POItemDetailsListJQGrid(long PurchOrderId, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                InventoryManagementService inventryService = new InventoryManagementService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                Dictionary<string, object> likeCriteria = new Dictionary<string, object>();

                if (!string.IsNullOrWhiteSpace(sord) && sord == "desc")
                    sord = "Desc";
                else
                    sord = "Asc";
                if (PurchOrderId > 0)
                    criteria.Add("Inventory_PurchaseOrder.PurchOrderId", PurchOrderId);

                Dictionary<long, IList<Inventory_PurchaseOrderItem>> poItems = inventryService.GetPurchaseOrderItemListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria, likeCriteria);
                if (poItems != null && poItems.Count > 0)
                {
                    long totalrecords = poItems.First().Key;
                    int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                    var jsondat = new
                    {
                        total = totalPages,
                        page = page,
                        records = totalrecords,

                        rows = (from items in poItems.FirstOrDefault().Value
                                select new
                                {
                                    i = 2,
                                    cell = new string[] {
                                            items.POLineId.ToString(),
                                            items.Inventory_PurchaseOrder.PurchOrderId.ToString(),
                                            items.UNCode.ToString(),
                                            items.Commodity,
                                            items.OrderedQty.ToString(),
                                            items.POUnitPrice.ToString(),
                                            items.POValue.ToString(),
                                            items.InvoicedQty.ToString(),
                                            items.RemainingQty.ToString(),
                                            items.CreatedDate!=null?items.CreatedDate.Value.ToString("dd/MM/yyyy"):"",
                                            items.CreatedBy,
                                            items.ModifiedDate!=null?items.ModifiedDate.Value.ToString("dd/MM/yyyy"):"",
                                            items.ModifiedBy
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

                throw ex;
            }
        }
        #endregion
        #region Receipts Report
        public ActionResult ReceiptsReportUploadRequestCreation(long? RequestId)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    OrdersService orderService = new OrdersService();
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

                throw ex;
            }
        }
        [HttpPost]
        public ActionResult ReceiptsReportUploadRequestCreation(UploadRequest upreq)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    OrdersService orderService = new OrdersService();
                    upreq.Category = "RECEIPTSREPORTUPLOAD";
                    upreq.CreatedBy = userId;
                    upreq.CreatedDate = DateTime.Now;
                    long reqid = orderService.SaveOrUpdateUploadRequest(upreq);
                    upreq.RequestNo = "REQUEST-" + reqid;
                    upreq.Status = "OPEN";
                    upreq.UploadStatus = "Request Generated";
                    orderService.SaveOrUpdateUploadRequest(upreq);
                    return (RedirectToAction("ReceiptsReportUploadRequestCreation", new { RequestId = upreq.RequestId }));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public ActionResult ReceiptsReportBulkUpload(long RequestId)
        {
            string RequestNo = "REQUEST-" + RequestId;
            var files = Request.Files["Filedata"];
            string curpath = ConfigurationManager.AppSettings["ReceiptsReportUploadRequestFilePath"].ToString();
            string RequestFolder = String.Format("{0}\\" + RequestNo, curpath);
            // If the folder is not existed, create it.
            if (!Directory.Exists(RequestFolder)) { Directory.CreateDirectory(RequestFolder); }
            string savePath = RequestFolder + "\\" + files.FileName;
            files.SaveAs(savePath);
            return Content(Url.Content(@"~\Content\" + files.FileName));
        }
        public void callReceiptReportParallel(long RequestId)
        {
            try
            {
                new Task(() =>
                {
                    CreateParallelTaskforReceiptReport(RequestId);
                }).Start();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void CreateParallelTaskforReceiptReport(long RequestId)
        {
            try
            {
                string server = ConfigurationManager.AppSettings["InstanceName"].ToString();
                string database = ConfigurationManager.AppSettings["DatabaseName"].ToString();
                OrdersService orderService = new OrdersService();
                InventoryManagementService inventoryService = new InventoryManagementService();
                UploadRequestDetailsLog uploadlog = new UploadRequestDetailsLog();
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
                string curpath = ConfigurationManager.AppSettings["ReceiptsReportUploadRequestFilePath"].ToString();
                string SQLServerConnectionString = String.Format("Data Source={0};Initial Catalog={1};Integrated Security=SSPI", server, database);
                try
                {
                    string CSVpath = @curpath + "\\REQUEST-" + RequestId; // CSV file Path
                    string CSVFileConnectionString = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};;Extended Properties=\"text;HDR=Yes;FMT=Delimited\";", CSVpath);
                    var AllFiles = new DirectoryInfo(CSVpath).GetFiles("*.CSV");
                    string File_Name = string.Empty;
                    foreach (var file in AllFiles)
                    {
                        try
                        {
                            string fileName = CSVpath + "\\" + file.Name.ToString();
                            //Create or update the  saving the individual file status in uploadrequestdetailslog table
                            long UploadReqDetLogId = CreateOrUpdateRequest(fileName, RequestId);
                            uploadlog = orderService.GetUploadRequestDetailsLogbyRequestId(UploadReqDetLogId);
                            DataTable dt = new DataTable();
                            using (OleDbConnection con = new OleDbConnection(CSVFileConnectionString))
                            {
                                con.Open();
                                string QeryToGetXcelData = string.Empty;
                                QeryToGetXcelData = string.Format("select * from [{0}]", file.Name);
                                using (OleDbDataAdapter da = new OleDbDataAdapter(QeryToGetXcelData, con))
                                {
                                    da.Fill(dt);
                                    dt.Columns.Add("PORequestId", typeof(Int64));
                                    dt.Columns.Add("RequestId", typeof(Int64));
                                    dt.Columns.Add("CreatedBy", typeof(string));
                                    dt.Columns.Add("CreatedDate", typeof(DateTime));
                                    dt.Columns.Add("IsValid", typeof(string));
                                    foreach (DataRow dr in dt.Rows)
                                    {
                                        dr["PORequestId"] = Convert.ToInt64(upreq.Sector);
                                        dr["RequestId"] = RequestId;
                                        dr["CreatedBy"] = upreq.CreatedBy;
                                        dr["CreatedDate"] = DateTime.Now;
                                        dr["IsValid"] = "VALID";
                                    }
                                }
                                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(SQLServerConnectionString))
                                {
                                    bulkCopy.ColumnMappings.Add("RECEIPTKEY", "ReceiptKey");
                                    bulkCopy.ColumnMappings.Add("EXTERNPOKEY", "ExternPOKey");
                                    bulkCopy.ColumnMappings.Add("STORERKEY", "StorerKey");
                                    bulkCopy.ColumnMappings.Add("COMPANY", "Company");
                                    bulkCopy.ColumnMappings.Add("SKU", "SKU");
                                    bulkCopy.ColumnMappings.Add("DESCRIPTION", "Description");
                                    bulkCopy.ColumnMappings.Add("QTYEXPECTED", "QtyExpected");
                                    bulkCopy.ColumnMappings.Add("QTYRECEIVED", "QtyReceived");
                                    bulkCopy.ColumnMappings.Add("DATERECEIVED", "DateReceived");
                                    bulkCopy.ColumnMappings.Add("STOCKTYPE", "StockType");
                                    bulkCopy.ColumnMappings.Add("BATCH", "Batch");
                                    bulkCopy.ColumnMappings.Add("MANDATE", "ManDate");
                                    bulkCopy.ColumnMappings.Add("RECEIPTDATE", "ReceiptDate");
                                    bulkCopy.ColumnMappings.Add("EXPIRYDATE", "ExpiryDate");
                                    bulkCopy.ColumnMappings.Add("COST", "Cost");
                                    bulkCopy.ColumnMappings.Add("UNITCOSTINUSD", "UnitCostInUSD");
                                    bulkCopy.ColumnMappings.Add("TOTALCOST", "TotalCost");
                                    bulkCopy.ColumnMappings.Add("TOTALCOSTINUSD", "TotalCostInUSD");
                                    bulkCopy.ColumnMappings.Add("POKEY", "POKey");
                                    bulkCopy.ColumnMappings.Add("SUPPLIERCODE", "SupplierCode");
                                    bulkCopy.ColumnMappings.Add("SUPPLIERNAME", "SupplierName");
                                    bulkCopy.ColumnMappings.Add("SUPPLIERDELIVERYNOTENO", "SupplierDeliveryNoteNo");
                                    bulkCopy.ColumnMappings.Add("INVOICENUMBER", "InvoiceNumber");
                                    bulkCopy.ColumnMappings.Add("CONTAINERKEY", "ContainerKey");
                                    bulkCopy.ColumnMappings.Add("BILLOFLOADING", "BillofLoading");
                                    bulkCopy.ColumnMappings.Add("NOTES", "Notes");
                                    bulkCopy.ColumnMappings.Add("DAMAGEQTY", "DamageQty");
                                    bulkCopy.ColumnMappings.Add("CONDITIONCODE", "ConditionCode");
                                    bulkCopy.ColumnMappings.Add("LOT", "LOT");
                                    bulkCopy.ColumnMappings.Add("RETAILSKU", "RetailSKU");
                                    bulkCopy.ColumnMappings.Add("ASNPRICE", "ASNPrice");
                                    bulkCopy.ColumnMappings.Add("CURRENCY", "Currency");
                                    bulkCopy.ColumnMappings.Add("PORTALPRICE", "PortalPrice");
                                    bulkCopy.ColumnMappings.Add("SHELFLIFEATRECEIPT", "ShelfLifeAtReceipt");
                                    bulkCopy.ColumnMappings.Add("SHELFLIFE", "ShelfLife");
                                    bulkCopy.ColumnMappings.Add("SHELFLIFEPERCENTAGEATRECEIPT", "ShelfLifePercentageAtReceipt");
                                    bulkCopy.ColumnMappings.Add("DATESTART", "DateStart");
                                    bulkCopy.ColumnMappings.Add("DATEEND", "DateEnd");
                                    bulkCopy.ColumnMappings.Add("ASNSTART", "ASNStart");
                                    bulkCopy.ColumnMappings.Add("ASNEND", "ASNEnd");
                                    bulkCopy.ColumnMappings.Add("SKUSTART", "SKUStart");
                                    bulkCopy.ColumnMappings.Add("SKUEND", "SKUEnd");
                                    bulkCopy.ColumnMappings.Add("LOTTABLE01", "Lottable01");
                                    bulkCopy.ColumnMappings.Add("LOTTABLE09", "Lottable09");
                                    bulkCopy.ColumnMappings.Add("CONTAINERQTY", "ContainerQty");
                                    bulkCopy.ColumnMappings.Add("TOTALQTYRECEIVED", "TotalQtyReceived");

                                    bulkCopy.ColumnMappings.Add("CreatedBy", "CreatedBy");
                                    bulkCopy.ColumnMappings.Add("CreatedDate", "CreatedDate");
                                    bulkCopy.ColumnMappings.Add("RequestId", "RequestId");
                                    bulkCopy.ColumnMappings.Add("PORequestId", "PORequestId");
                                    bulkCopy.ColumnMappings.Add("IsValid", "IsValid");

                                    bulkCopy.DestinationTableName = "ReceiptReport";
                                    bulkCopy.BatchSize = 0;
                                    bulkCopy.WriteToServer(dt);
                                    bulkCopy.Close();
                                }
                                if (uploadlog.UploadStatus == "YetToUpload")
                                    uploadlog.UploadStatus = "UploadedSuccessfully";
                                orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);
                                inventoryService.InsertorUpdatePOReceiptReportDetailsByRequestId_sp(RequestId);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (uploadlog.UploadStatus == "YetToUpload")
                                uploadlog.UploadStatus = "UploadFailed";
                            uploadlog.ErrDesc = ex.ToString();
                            orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);
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
                    throw ex;
                }
                StatusUpdateInUploadRequest(RequestId);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public ActionResult ReceiptReport()
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

                throw ex;
            }
        }
        public JsonResult ReceiptReportJQGrid(ReceiptReport receiptReport, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                InventoryManagementService inventryService = new InventoryManagementService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                Dictionary<string, object> likeCriteria = new Dictionary<string, object>();

                if (!string.IsNullOrWhiteSpace(sord) && sord == "desc")
                    sord = "Desc";
                else
                    sord = "Asc";

                if (receiptReport != null)
                {
                    if (receiptReport.ReceiptKey > 0) criteria.Add("ReceiptKey", receiptReport.ReceiptKey);
                    if (receiptReport.ExternPOKey > 0) criteria.Add("ExternPOKey", receiptReport.ExternPOKey);
                    if (receiptReport.StorerKey > 0) criteria.Add("StorerKey", receiptReport.StorerKey);
                    if (receiptReport.POKey > 0) criteria.Add("POKey", receiptReport.POKey);
                    if (!string.IsNullOrEmpty(receiptReport.InvoiceNumber)) criteria.Add("InvoiceNumber", receiptReport.InvoiceNumber);
                    if (!string.IsNullOrEmpty(receiptReport.ContainerKey)) criteria.Add("ContainerKey", receiptReport.ContainerKey);
                }
                Dictionary<long, IList<ReceiptReport>> ReceiptReportList = inventryService.GetReceiptReportListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria, likeCriteria);
                if (ReceiptReportList != null && ReceiptReportList.Count > 0)
                {
                    long totalrecords = ReceiptReportList.First().Key;
                    int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                    var jsondat = new
                    {
                        total = totalPages,
                        page = page,
                        records = totalrecords,

                        rows = (from items in ReceiptReportList.FirstOrDefault().Value
                                select new
                                {
                                    i = 2,
                                    cell = new string[] {
                                        items.Receipt_Id.ToString(),
                                        items.ReceiptKey.ToString(),
                                        items.ExternPOKey.ToString(),
                                        items.StorerKey.ToString(),
                                        items.Company,
                                        items.SKU.ToString(),
                                        items.Description,
                                        items.QtyExpected.ToString(),
                                        items.QtyReceived.ToString(),
                                        items.DateReceived!=null?items.DateReceived.Value.ToString("dd/MM/yyyy"):"",
                                        items.StockType,
                                        items.Batch,
                                        items.ManDate!=null?items.ManDate.Value.ToString("dd/MM/yyyy"):"",
                                        items.ReceiptDate!=null?items.ReceiptDate.Value.ToString("dd/MM/yyyy"):"",
                                        items.ExpiryDate!=null?items.ExpiryDate.Value.ToString("dd/MM/yyyy"):"",
                                        items.Cost.ToString(),
                                        items.UnitCostInUSD.ToString(),
                                        items.TotalCost.ToString(),
                                        items.TotalCostInUSD.ToString(),
                                        items.POKey.ToString(),
                                        items.SupplierCode,
                                        items.SupplierName,
                                        items.SupplierDeliveryNoteNo,
                                        items.InvoiceNumber,
                                        items.ContainerKey,
                                        items.BillofLoading,
                                        items.Notes,
                                        items.DamageQty.ToString(),
                                        items.ConditionCode,
                                        items.LOT.ToString(),
                                        items.RetailSKU.ToString(),
                                        items.ASNPrice.ToString(),
                                        items.Currency,
                                        items.PortalPrice.ToString(),
                                        items.ShelfLifeAtReceipt.ToString(),
                                        items.ShelfLife.ToString(),
                                        items.ShelfLifePercentageAtReceipt.ToString(),
                                        items.DateStart!=null?items.DateStart.Value.ToString("dd/MM/yyyy"):"",
                                        items.DateEnd!=null?items.DateEnd.Value.ToString("dd/MM/yyyy"):"",
                                        items.ASNStart,
                                        items.ASNEnd,
                                        items.SKUStart,
                                        items.SKUEnd,
                                        items.Lottable01,
                                        items.Lottable09,
                                        items.ContainerQty.ToString(),
                                        items.TotalQtyReceived.ToString(),
                                        items.CreatedBy,
                                        items.CreatedDate!=null?items.CreatedDate.Value.ToString("dd/MM/yyyy"):"",
                                        items.ModifiedBy,
                                        items.ModifiedDate!=null?items.ModifiedDate.Value.ToString("dd/MM/yyyy"):"",
                                        items.RequestId.ToString(),
                                        items.IsValid
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

                throw ex;
            }
        }
        #endregion
        #region Report Generation
        public ActionResult GITReport()
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

                throw ex;
            }
        }
        public JsonResult GITReportJQGrid(string Status)
        {
            try
            {
                string userId = base.ValidateUser();
                InventoryManagementService inventryService = new InventoryManagementService();
                InvoiceService IS = new InvoiceService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                Dictionary<string, object> likeCriteria = new Dictionary<string, object>();

                //criteria.Add("POStatus", Status);
                Dictionary<long, IList<GITReport_vw>> GITReportList = inventryService.GetGITReportListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria, likeCriteria);
                if (GITReportList != null && GITReportList.Count > 0)
                {
                    #region Excel generation
                    List<GITReport_vw> GITExcelReport = GITReportList.FirstOrDefault().Value.ToList();
                    var List = GITExcelReport;
                    int i = 1;
                    foreach (var item in List)
                    {
                        item.Id = i;
                        i = i + 1;
                    }
                    List<string> lstHeader = new List<string>() { "Id", "PO#", "Item Code", "ITEM DESCRIPTION", "Qty in kgs", "total qty's shipped & invoiced", "PO Unit price", " Invoice currency", "Invoice Unit price", "Invoice Amount", "Exchange Rate", "Invoice Value In  USD", "UNIT PRICE $", "SUPPLIER", "BL #", "Cont. #", "Invoice #", "Invoice date", "Voucher # in Oracle (Pur. Clg)", "updates", "Openig GIT Balance", "Sector", "Inventory Qty", "Inventory Value", "Variance Qty", "Variance Value as per IPC Rate", "Variance Value ", "Remarks" };
                    string txtTName = "GIT on " + DateTime.Now.ToString("dd/MM/yyyy");
                    byte[] data = GenerateByteExcel_GCCRevised(List, txtTName, (items => new
                    {
                        Id = items.Id,
                        ControlId = items.PONumber,
                        UNCode = items.UNCode,
                        Commodity = items.Commodity,
                        OrderQty = string.Format("{0:N}", items.OrderQty),
                        ShippedQty = string.Format("{0:N}", items.ShippedQty),
                        POUnitPrice = string.Format("{0:N}", items.POUnitPrice),
                        InvoiceCurrency = items.InvoiceCurrency,
                        InvoiceUnitPrice = string.Format("{0:N}", items.InvoiceUnitPrice),
                        InvoiceAmount = string.Format("{0:N}", items.InvoiceAmount),
                        ExchangeRate = string.Format("{0:N}", items.ExchangeRate),
                        InvoiceValueUSD = string.Format("{0:N}", items.InvoiceValueUSD),
                        UnitPriceUSD = string.Format("{0:N}", items.UnitPriceUSD),
                        Supplier = items.Supplier,
                        BLNo = items.BLNo,
                        ContainerNo = items.ContainerNo,
                        InvoiceNo = items.InvoiceNo,
                        InvoiceDate = items.InvoiceDate != null ? items.InvoiceDate.Value.ToString("dd/MM/yyyy") : "",
                        OracleVoucherNo = string.Format("{0:N}", items.OracleVoucherNo),
                        Updates = items.InvoiceDate != null ? items.Updates.Value.ToString("dd/MM/yyyy") : "",
                        OpeningGITBal = string.Format("{0:N}", items.OpeningGITBal),
                        Sector = items.Sector,
                        InventoryQty = string.Format("{0:N}", items.InventoryQty),
                        InventoryValue = string.Format("{0:N}", items.InventoryValue),
                        VarianceQty = string.Format("{0:N}", items.VarianceQty),
                        VarianceValueperIPCRate = string.Format("{0:N}", items.VarianceValueperIPCRate),
                        VarianceValue = string.Format("{0:N}", items.VarianceValue),
                        Remarks = items.Remarks
                    }), lstHeader);

                    ExcelDocuments doc = new ExcelDocuments();
                    doc.ControlId = txtTName;
                    doc.DocumentData = data;
                    doc.DocumentType = "GITReport";
                    doc.DocumentFor = "Inventory";
                    doc.DocumentName = txtTName;
                    long id = IS.SaveOrUpdateExcelDocuments(doc, userId);
                    #endregion


                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var jsondat = new { rows = (new { cell = new string[] { } }) };
                    return Json(jsondat, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion
        #region PurchaseOrderInvoice
        public ActionResult POInvoice()
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

                throw ex;
            }
        }

        #endregion
        #region PO Invoice
        public ActionResult PurchaseOrderInvoice()
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

                throw ex;
            }
        }
        public JsonResult GetPODetailsbyPO(string PurchOrderIds, long POInvoiceId)
        {
            try
            {
                if (!string.IsNullOrEmpty(PurchOrderIds) && PurchOrderIds != "null" || POInvoiceId > 0)
                {
                    InventoryManagementService inventoryService = new InventoryManagementService();
                    Dictionary<string, object> criteria = new Dictionary<string, object>();
                    Inventory_PurchaseOrderInvoice PurchaseOrderInvoiceObj = new Inventory_PurchaseOrderInvoice();
                    Dictionary<long, IList<Inventory_PurchaseOrder>> POMasterList = null;
                    if (POInvoiceId > 0)
                        PurchaseOrderInvoiceObj = inventoryService.GetInvoiceByPOInvoiceId(POInvoiceId);
                    if (PurchaseOrderInvoiceObj != null && PurchaseOrderInvoiceObj.POInvoiceId > 0)
                    {
                        string[] strPOIdArr = PurchOrderIds.Split(new string[] { ",", null }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                        long[] POIdArr = Array.ConvertAll(strPOIdArr, element => Convert.ToInt64(element));
                        criteria.Clear();
                        IList<Inventory_PurchaseOrderInvoiceItem> InvoiceItemvalueList = inventoryService.GetPurchaseOrderInvoiceItemsListByPOInvoiceId(PurchaseOrderInvoiceObj.POInvoiceId);
                        long[] ArrPOId = (from u in InvoiceItemvalueList select u.PurchOrderId).Distinct().ToArray();
                        long[] POIds = POIdArr.Union(ArrPOId).ToArray();
                        if (ArrPOId.Length > 0)
                            criteria.Add("PurchOrderId", POIds);
                        POMasterList = inventoryService.GetPONumberWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);

                        string strInvDate = PurchaseOrderInvoiceObj.InvoiceDate != null ? PurchaseOrderInvoiceObj.InvoiceDate.Value.ToString("dd/MM/yyyy") : "";
                        string strGLDate = PurchaseOrderInvoiceObj.GLDate != null ? PurchaseOrderInvoiceObj.GLDate.Value.ToString("dd/MM/yyyy") : "";
                        var PODetailsList = (from u in POMasterList.FirstOrDefault().Value select new { u.SupplierNumber, u.Supplier, u.POCurrency, PurchaseOrderInvoiceObj.InvoiceNumber, PurchaseOrderInvoiceObj.BillOfLading, PurchaseOrderInvoiceObj.ContainerNumber, PurchaseOrderInvoiceObj.InvoiceAmount, strGLDate, PurchaseOrderInvoiceObj.VoucherNumber, strInvDate }).Distinct().ToList();
                        return Json(PODetailsList, JsonRequestBehavior.AllowGet);
                    }
                    var jsondat = new { rows = (new { cell = new string[] { } }) };
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

                throw ex;
            }
        }
        public JsonResult PurchaseOrderDetailsListJQGrid(string PurchOrderId, long POInvoiceId, Inventory_PurchaseOrderItem_vw poItemObj, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                InventoryManagementService inventryService = new InventoryManagementService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();

                if (!string.IsNullOrWhiteSpace(sord) && sord == "desc")
                    sord = "Desc";
                else
                    sord = "Asc";

                Dictionary<long, IList<Inventory_PurchaseOrderItem_vw>> poItems = new Dictionary<long, IList<Inventory_PurchaseOrderItem_vw>>();
                if (!string.IsNullOrEmpty(PurchOrderId) && PurchOrderId != "null")
                {
                    string[] strPOIdArr = PurchOrderId.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    long[] POIdArr = Array.ConvertAll(strPOIdArr, element => Convert.ToInt64(element));
                    criteria.Add("PurchOrderId", POIdArr);
                    if (POInvoiceId > 0)
                        criteria.Add("POInvoiceId", POInvoiceId);
                    if (poItemObj != null)
                    {
                        if (poItemObj.UNCode > 0)
                            criteria.Add("UNCode", poItemObj.UNCode);
                        if (!string.IsNullOrEmpty(poItemObj.Commodity))
                            criteria.Add("Commodity", poItemObj.Commodity);
                        if (poItemObj.OrderedQty > 0)
                            criteria.Add("OrderedQty", poItemObj.OrderedQty);
                        if (poItemObj.POUnitPrice > 0)
                            criteria.Add("POUnitPrice", poItemObj.POUnitPrice);
                        if (poItemObj.POValue > 0)
                            criteria.Add("POValue", poItemObj.POValue);
                    }
                    poItems = inventryService.GetPurchaseOrderItemListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                }
                if (poItems != null && poItems.Count > 0)
                {
                    long totalrecords = poItems.First().Key;
                    int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                    var jsondat = new
                    {
                        total = totalPages,
                        page = page,
                        records = totalrecords,

                        rows = (from items in poItems.FirstOrDefault().Value
                                select new
                                {
                                    i = 2,
                                    cell = new string[] {
                                        items.Id.ToString(),
                                            items.PurchOrderId.ToString(),
                                            items.PO.ToString(),
                                            items.POLineId.ToString(),
                                            items.UNCode.ToString(),
                                            items.Commodity,
                                            items.OrderedQty.ToString(),
                                            items.POUnitPrice.ToString(),
                                            items.POValue.ToString(),
                                            items.InvoicedQty.ToString(),
                                            items.RemainingQty.ToString(),
                                            items.InvoiceUnitPrice.ToString()
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

                throw ex;
            }
        }
        public JsonResult SaveInvoiceDetailsforPO(string PurchOrderId, string InvDate, Inventory_PurchaseOrderInvoice Inventory_PurchaseOrderInvoiceObj)
        {
            try
            {
                string userId = base.ValidateUser();
                InventoryManagementService inventryService = new InventoryManagementService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                Inventory_PurchaseOrderInvoice PurchaseOrderInvoiceObj = new Inventory_PurchaseOrderInvoice();
                string strInvoiceNo = string.Empty;
                IList<Inventory_PurchaseOrderInvoiceMapping> poInvoiceMapList = new List<Inventory_PurchaseOrderInvoiceMapping>();
                if (Inventory_PurchaseOrderInvoiceObj != null && !string.IsNullOrEmpty(Inventory_PurchaseOrderInvoiceObj.InvoiceNumber))
                {
                    strInvoiceNo = Inventory_PurchaseOrderInvoiceObj.InvoiceNumber.ToUpper().Replace(" ", "");
                    PurchaseOrderInvoiceObj = inventryService.GetInvoiceByInvoiceNumber(strInvoiceNo);
                    if (PurchaseOrderInvoiceObj == null && PurchaseOrderInvoiceObj.POInvoiceId == 0)
                    {
                        Inventory_PurchaseOrderInvoice poInvoiceObj = new Inventory_PurchaseOrderInvoice();
                        poInvoiceObj.InvoiceNumber = strInvoiceNo;
                        poInvoiceObj.InvoiceDate = DateTime.Parse(InvDate, provider, System.Globalization.DateTimeStyles.NoCurrentDateDefault);
                        poInvoiceObj.BillOfLading = Inventory_PurchaseOrderInvoiceObj.BillOfLading.ToUpper().Replace(" ", "");
                        poInvoiceObj.ContainerNumber = Inventory_PurchaseOrderInvoiceObj.ContainerNumber.ToUpper().Replace(" ", "");
                        poInvoiceObj.InvCurrency = Inventory_PurchaseOrderInvoiceObj.InvCurrency;
                        poInvoiceObj.InvoiceAmount = Inventory_PurchaseOrderInvoiceObj.InvoiceAmount;
                        poInvoiceObj.RemainingAmount = Inventory_PurchaseOrderInvoiceObj.InvoiceAmount;
                        poInvoiceObj.CreatedBy = userId;
                        poInvoiceObj.CreatedDate = DateTime.Now;
                        inventryService.SaveOrUpdatePurchaseOrderInvocie(poInvoiceObj);
                        if (poInvoiceObj.POInvoiceId > 0)
                        {
                            string[] strPOIdArr = PurchOrderId.Split(new string[] { ",", null }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                            long[] POIdArr = Array.ConvertAll(strPOIdArr, element => Convert.ToInt64(element));
                            if (POIdArr.Length > 0)
                            {
                                for (int i = 0; i < POIdArr.Length; i++)
                                {
                                    Inventory_PurchaseOrderInvoiceMapping poInvoiceMap = new Inventory_PurchaseOrderInvoiceMapping();
                                    poInvoiceMap.POInvoiceId = poInvoiceObj.POInvoiceId;
                                    poInvoiceMap.PurchOrderId = POIdArr[i];
                                    poInvoiceMap.CreatedBy = userId;
                                    poInvoiceMap.CreatedDate = DateTime.Now;
                                    poInvoiceMapList.Add(poInvoiceMap);
                                }
                                inventryService.SaveOrUpdatepoInvoiceMappingList(poInvoiceMapList);
                            }
                        }
                        return Json(new { msg = "success", InvoiceId = poInvoiceObj.POInvoiceId }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { msg = "Invoice Already Exists!!", InvoiceId = 0 }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public JsonResult EditPOInvoiceDetails(string PurchOrderId, string InvDate, Inventory_PurchaseOrderInvoice Inventory_PurchaseOrderInvoiceObj)
        {
            try
            {
                string userId = base.ValidateUser();
                InventoryManagementService inventryService = new InventoryManagementService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                Inventory_PurchaseOrderInvoice PurchaseOrderInvoiceObj = new Inventory_PurchaseOrderInvoice();
                string strInvoiceNo = string.Empty;
                
                if (Inventory_PurchaseOrderInvoiceObj != null && !string.IsNullOrEmpty(Inventory_PurchaseOrderInvoiceObj.InvoiceNumber))
                {
                    strInvoiceNo = Inventory_PurchaseOrderInvoiceObj.InvoiceNumber.ToUpper().Replace(" ", "");
                    PurchaseOrderInvoiceObj = inventryService.GetInvoiceByPOInvoiceId(Inventory_PurchaseOrderInvoiceObj.POInvoiceId);
                    if (strInvoiceNo != PurchaseOrderInvoiceObj.InvoiceNumber)
                    {
                        PurchaseOrderInvoiceObj = new Inventory_PurchaseOrderInvoice();
                        PurchaseOrderInvoiceObj = inventryService.GetInvoiceByInvoiceNumber(strInvoiceNo);
                        if (PurchaseOrderInvoiceObj.InvoiceNumber == strInvoiceNo)
                        {
                            return Json(new { msg = "Invoice Already Exists!!", InvoiceId = 0 }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    if (Inventory_PurchaseOrderInvoiceObj.POInvoiceId > 0)
                    {
                        //PurchaseOrderInvoiceObj.InvoiceNumber = strInvoiceNo;
                        PurchaseOrderInvoiceObj.InvoiceDate = DateTime.Parse(InvDate, provider, System.Globalization.DateTimeStyles.NoCurrentDateDefault);
                        PurchaseOrderInvoiceObj.BillOfLading = Inventory_PurchaseOrderInvoiceObj.BillOfLading.ToUpper().Replace(" ", "");
                        PurchaseOrderInvoiceObj.ContainerNumber = Inventory_PurchaseOrderInvoiceObj.ContainerNumber.ToUpper().Replace(" ", "");
                        PurchaseOrderInvoiceObj.InvCurrency = Inventory_PurchaseOrderInvoiceObj.InvCurrency;
                        PurchaseOrderInvoiceObj.InvoiceAmount = Inventory_PurchaseOrderInvoiceObj.InvoiceAmount;
                        PurchaseOrderInvoiceObj.RemainingAmount = Inventory_PurchaseOrderInvoiceObj.InvoiceAmount;
                        PurchaseOrderInvoiceObj.ModifiedBy = userId;
                        PurchaseOrderInvoiceObj.ModifiedDate = DateTime.Now;
                        inventryService.SaveOrUpdatePurchaseOrderInvocie(PurchaseOrderInvoiceObj);
                        if (PurchaseOrderInvoiceObj.POInvoiceId > 0)
                        {
                            SaveorUpdateInventory_PurchaseOrderInvoiceMapping(PurchaseOrderInvoiceObj.POInvoiceId, PurchOrderId);
                            
                        }
                        return Json(new { msg = "success", InvoiceId = PurchaseOrderInvoiceObj.POInvoiceId }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { msg = "Invoice Already Exists!!", InvoiceId = 0 }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public ActionResult SaveorUpdateInventory_PurchaseOrderInvoiceMapping(long POInvoiceId, string PurchOrderId)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    InventoryManagementService inventryService = new InventoryManagementService();
                    IList<Inventory_PurchaseOrderInvoiceMapping> poInvoiceMapList = new List<Inventory_PurchaseOrderInvoiceMapping>();
                    IList<Inventory_PurchaseOrderInvoiceMapping> InvoicePOMapList = inventryService.GetPurchaseOrderInvoiceMappingListByPOInvoiceId(POInvoiceId);
                    if (InvoicePOMapList != null && InvoicePOMapList.Count > 0)
                    {
                        string[] strPOIdArr = PurchOrderId.Split(new string[] { ",", null }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                        long[] POIdArr = Array.ConvertAll(strPOIdArr, element => Convert.ToInt64(element));
                        long[] ExistPOIdArr = (from u in InvoicePOMapList select u.PurchOrderId).Distinct().ToArray();
                        var OldPOId = POIdArr.Intersect(ExistPOIdArr);
                        var NewPOId = POIdArr.Except(ExistPOIdArr);
                        var deletePOId = ExistPOIdArr.Except(POIdArr);
                        if (NewPOId.Count() > 0)
                        {
                            foreach (var item in NewPOId)
                            {
                                Inventory_PurchaseOrderInvoiceMapping poInvoiceMap = new Inventory_PurchaseOrderInvoiceMapping();
                                poInvoiceMap.POInvoiceId = POInvoiceId;
                                poInvoiceMap.PurchOrderId = item;
                                poInvoiceMap.CreatedBy = userId;
                                poInvoiceMap.CreatedDate = DateTime.Now;
                                poInvoiceMapList.Add(poInvoiceMap);
                            }
                            inventryService.SaveOrUpdatepoInvoiceMappingList(poInvoiceMapList);
                        }
                        if (deletePOId.Count() > 0)
                        {
                            var result = string.Join(",", deletePOId);
                            inventryService.DeleteInventory_PurchaseOrderInvoiceMappingById(POInvoiceId, result);
                            updateInvoicedAmountinPurchaseOrderInvoicebyInvoiceId(POInvoiceId);//Update InvoicedAmount
                        }
                        return null;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public ActionResult UpdateInvoiceUnitPriceCell(Inventory_PurchaseOrderInvoiceItem_VW invoiceItemObj)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    InventoryManagementService inventryService = new InventoryManagementService();
                    Inventory_PurchaseOrderInvoiceItem poInvoiceItem = new Inventory_PurchaseOrderInvoiceItem();
                    Inventory_PurchaseOrder purchaseOrder = new Inventory_PurchaseOrder();
                    Inventory_PurchaseOrderItem orditems = inventryService.GetPurchaseOrderItemsById(invoiceItemObj.POLineId);
                    if (invoiceItemObj != null && invoiceItemObj.POInvoiceItemId > 0)
                    {
                        poInvoiceItem = inventryService.GetPurchaseOrderInvoiceItemsById(invoiceItemObj.POInvoiceItemId);
                        poInvoiceItem.InvoiceUnitPrice = invoiceItemObj.InvoiceUnitPrice;
                        poInvoiceItem.InvoiceValue = poInvoiceItem.InvoicedQty * invoiceItemObj.InvoiceUnitPrice;
                        poInvoiceItem.ModifiedBy = userId;
                        poInvoiceItem.ModifiedDate = DateTime.Now;
                        inventryService.SaveOrUpdatePurchaseOrderInvoiceItems(poInvoiceItem);

                        /////Update total Invoice amount in PurchaseOrder
                        IList<Inventory_PurchaseOrderInvoiceItem> InvoiceItemList = inventryService.GetPurchaseOrderInvoiceItemsListByPurchOrderId(invoiceItemObj.PurchOrderId);
                        decimal Totalamount = InvoiceItemList.Sum(x => x.InvoiceValue);
                        purchaseOrder = inventryService.GetPurchaseOrderbyPurchaseOrderId(invoiceItemObj.PurchOrderId);
                        purchaseOrder.InvoiceAmount = Totalamount;
                        purchaseOrder.ModifiedBy = userId;
                        purchaseOrder.ModifiedDate = DateTime.Now;
                        inventryService.SaveOrUpdatePurchaseOrder(purchaseOrder);


                        return Json(new { poInvoiceItem.InvoiceUnitPrice }, JsonRequestBehavior.AllowGet);
                    }
                    if (invoiceItemObj != null && invoiceItemObj.POInvoiceItemId == 0)
                    {
                        poInvoiceItem = new Inventory_PurchaseOrderInvoiceItem();
                        poInvoiceItem.INVConfig_Id = invoiceItemObj.INVConfig_Id;
                        poInvoiceItem.POInvoiceId = invoiceItemObj.POInvoiceId;
                        poInvoiceItem.PurchOrderId = invoiceItemObj.PurchOrderId;
                        poInvoiceItem.POLineId = invoiceItemObj.POLineId;
                        poInvoiceItem.OrderedQty = orditems.OrderedQty;
                        poInvoiceItem.InvoiceUnitPrice = invoiceItemObj.InvoiceUnitPrice;
                        poInvoiceItem.CreatedBy = userId;
                        poInvoiceItem.CreatedDate = DateTime.Now;
                        inventryService.SaveOrUpdatePurchaseOrderInvoiceItems(poInvoiceItem);

                        return Json(new { poInvoiceItem.InvoiceUnitPrice }, JsonRequestBehavior.AllowGet);
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public ActionResult UpdateInvoicedQtyCell(Inventory_PurchaseOrderInvoiceItem_VW invoiceItemObj)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    InventoryManagementService inventryService = new InventoryManagementService();
                    Inventory_PurchaseOrder purchaseOrder = new Inventory_PurchaseOrder();
                    Inventory_PurchaseOrderInvoiceItem poInvoiceItem = new Inventory_PurchaseOrderInvoiceItem();
                    Inventory_PurchaseOrderItem orditems = new Inventory_PurchaseOrderItem();
                    orditems = inventryService.GetPurchaseOrderItemsById(invoiceItemObj.POLineId);
                    if (invoiceItemObj != null && invoiceItemObj.POInvoiceItemId == 0)
                    {
                        poInvoiceItem = new Inventory_PurchaseOrderInvoiceItem();
                        poInvoiceItem.INVConfig_Id = invoiceItemObj.INVConfig_Id;
                        poInvoiceItem.POInvoiceId = invoiceItemObj.POInvoiceId;
                        poInvoiceItem.PurchOrderId = invoiceItemObj.PurchOrderId;
                        poInvoiceItem.POLineId = invoiceItemObj.POLineId;
                        poInvoiceItem.OrderedQty = orditems.OrderedQty;
                        poInvoiceItem.InvoiceUnitPrice = invoiceItemObj.InvoiceUnitPrice;
                        poInvoiceItem.InvoicedQty = invoiceItemObj.InvoiceQty;
                        poInvoiceItem.InvoiceValue = poInvoiceItem.InvoiceUnitPrice * poInvoiceItem.InvoicedQty;
                        poInvoiceItem.CreatedBy = userId;
                        poInvoiceItem.CreatedDate = DateTime.Now;
                        inventryService.SaveOrUpdatePurchaseOrderInvoiceItems(poInvoiceItem);
                        /////Update total Invoice amount in PurchaseOrder
                        IList<Inventory_PurchaseOrderInvoiceItem> InvoiceItemList = inventryService.GetPurchaseOrderInvoiceItemsListByPurchOrderId(invoiceItemObj.PurchOrderId);
                        decimal Totalamount = InvoiceItemList.Sum(x => x.InvoiceValue);
                        purchaseOrder = inventryService.GetPurchaseOrderbyPurchaseOrderId(invoiceItemObj.PurchOrderId);
                        purchaseOrder.InvoiceAmount = Totalamount;
                        purchaseOrder.ModifiedBy = userId;
                        purchaseOrder.ModifiedDate = DateTime.Now;
                        inventryService.SaveOrUpdatePurchaseOrder(purchaseOrder);

                        ////Update Invoice Qty in PurchaseOrderItem
                        IList<Inventory_PurchaseOrderInvoiceItem> InvoicePOItemList = inventryService.GetPurchaseOrderInvoiceItemsListByPOLineId(invoiceItemObj.POLineId);
                        decimal TotalInvoicedQty = InvoicePOItemList.Sum(x => x.InvoicedQty);
                        orditems.InvoicedQty = TotalInvoicedQty;
                        orditems.RemainingQty = orditems.OrderedQty - TotalInvoicedQty;
                        inventryService.SaveOrUpdateInventory_PurchaseOrderItem(orditems);

                        return Json(new { poInvoiceItem.POInvoiceItemId, orditems.InvoicedQty, orditems.RemainingQty }, JsonRequestBehavior.AllowGet);
                    }
                    if (invoiceItemObj != null && invoiceItemObj.POInvoiceItemId > 0)
                    {
                        poInvoiceItem = inventryService.GetPurchaseOrderInvoiceItemsById(invoiceItemObj.POInvoiceItemId);
                        poInvoiceItem.InvoiceUnitPrice = invoiceItemObj.InvoiceUnitPrice;
                        poInvoiceItem.InvoicedQty = invoiceItemObj.InvoiceQty;
                        poInvoiceItem.InvoiceValue = poInvoiceItem.InvoiceUnitPrice * poInvoiceItem.InvoicedQty;
                        poInvoiceItem.ModifiedBy = userId;
                        poInvoiceItem.ModifiedDate = DateTime.Now;
                        inventryService.SaveOrUpdatePurchaseOrderInvoiceItems(poInvoiceItem);
                        /////Update total Invoice amount in PurchaseOrder
                        IList<Inventory_PurchaseOrderInvoiceItem> InvoiceItemList = inventryService.GetPurchaseOrderInvoiceItemsListByPurchOrderId(invoiceItemObj.PurchOrderId);
                        decimal Totalamount = InvoiceItemList.Sum(x => x.InvoiceValue);
                        purchaseOrder = inventryService.GetPurchaseOrderbyPurchaseOrderId(invoiceItemObj.PurchOrderId);
                        purchaseOrder.InvoiceAmount = Totalamount;
                        purchaseOrder.ModifiedBy = userId;
                        purchaseOrder.ModifiedDate = DateTime.Now;
                        inventryService.SaveOrUpdatePurchaseOrder(purchaseOrder);

                        ////Update Invoice Qty in PurchaseOrderItem
                        IList<Inventory_PurchaseOrderInvoiceItem> InvoicePOItemList = inventryService.GetPurchaseOrderInvoiceItemsListByPOLineId(invoiceItemObj.POLineId);
                        decimal TotalInvoicedQty = InvoicePOItemList.Sum(x => x.InvoicedQty);
                        orditems.InvoicedQty = TotalInvoicedQty;
                        orditems.RemainingQty = orditems.OrderedQty - TotalInvoicedQty;
                        inventryService.SaveOrUpdateInventory_PurchaseOrderItem(orditems);

                        return Json(new { poInvoiceItem.POInvoiceItemId, orditems.InvoicedQty, orditems.RemainingQty }, JsonRequestBehavior.AllowGet);
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public ActionResult UpdateInvoiceRemarksCell(Inventory_PurchaseOrderInvoiceItem_VW invoiceItemObj)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    InventoryManagementService inventryService = new InventoryManagementService();
                    Inventory_PurchaseOrderItem orditems = inventryService.GetPurchaseOrderItemsById(invoiceItemObj.POLineId);
                    Inventory_PurchaseOrderInvoiceItem poInvoiceItem = new Inventory_PurchaseOrderInvoiceItem();
                    if (invoiceItemObj != null && invoiceItemObj.POInvoiceItemId > 0)
                    {
                        poInvoiceItem = inventryService.GetPurchaseOrderInvoiceItemsById(invoiceItemObj.POInvoiceItemId);
                        poInvoiceItem.Remarks = invoiceItemObj.Remarks;
                        poInvoiceItem.ModifiedBy = userId;
                        poInvoiceItem.ModifiedDate = DateTime.Now;
                        inventryService.SaveOrUpdatePurchaseOrderInvoiceItems(poInvoiceItem);
                        return Json(new { poInvoiceItem.Remarks }, JsonRequestBehavior.AllowGet);
                    }
                    if (invoiceItemObj != null && invoiceItemObj.POInvoiceItemId == 0)
                    {
                        poInvoiceItem = new Inventory_PurchaseOrderInvoiceItem();
                        poInvoiceItem = new Inventory_PurchaseOrderInvoiceItem();
                        poInvoiceItem.INVConfig_Id = invoiceItemObj.INVConfig_Id;
                        poInvoiceItem.POInvoiceId = invoiceItemObj.POInvoiceId;
                        poInvoiceItem.PurchOrderId = invoiceItemObj.PurchOrderId;
                        poInvoiceItem.POLineId = invoiceItemObj.POLineId;
                        poInvoiceItem.OrderedQty = orditems.OrderedQty;
                        poInvoiceItem.Remarks = invoiceItemObj.Remarks;
                        poInvoiceItem.CreatedBy = userId;
                        poInvoiceItem.CreatedDate = DateTime.Now;
                        inventryService.SaveOrUpdatePurchaseOrderInvoiceItems(poInvoiceItem);
                        return Json(new { poInvoiceItem.Remarks }, JsonRequestBehavior.AllowGet);
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// Temp class to receive array objects from the client side
        /// </summary>
        public class InvoiceItemObj
        {
            public int PurchOrderId { get; set; }
            public int POLineId { get; set; }
            public int POInvoiceId { get; set; }
        }
        public ActionResult DisconnectInvoiceItem(List<InvoiceItemObj> DataObj)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    InventoryManagementService inventryService = new InventoryManagementService();
                    if (DataObj != null)
                    {
                        Inventory_PurchaseOrderInvoiceItem poInvoiceItem = new Inventory_PurchaseOrderInvoiceItem();
                        IList<Inventory_PurchaseOrderInvoiceItem> invoiceItemList = new List<Inventory_PurchaseOrderInvoiceItem>();
                        foreach (var item in DataObj)
                        {
                            poInvoiceItem = inventryService.GetPurchaseOrderInvoiceItemsByPOIdandPOInvoiceIdandPOLineId(item.PurchOrderId, item.POInvoiceId, item.POLineId);
                            if (poInvoiceItem != null && poInvoiceItem.POInvoiceItemId > 0)
                            {
                                poInvoiceItem.IsDisconnect = true;
                                poInvoiceItem.ModifiedBy = userId;
                                poInvoiceItem.ModifiedDate = DateTime.Now;
                                invoiceItemList.Add(poInvoiceItem);
                            }
                        }
                        inventryService.SaveOrUpdateInvoiceItemList(invoiceItemList);
                        return Json(new { Msg = "Disconnected Successfully!!" }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { Msg = "Failled to Disconnect!!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// Complete Invoice if invoice not having Disconnect
        /// </summary>
        /// <param name="POInvoiceId"></param>
        /// <returns></returns>
        public ActionResult CompletePurchaseOrderInvoice(string GLDate, Inventory_PurchaseOrderInvoice Inventory_PurchaseOrderInvoiceObj)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    InventoryManagementService inventryService = new InventoryManagementService();
                    var script = string.Empty;
                    bool IsContainsDisconnect = false;
                    Inventory_PurchaseOrderInvoice invoiceObj = new Inventory_PurchaseOrderInvoice();
                    IList<Inventory_PurchaseOrderInvoiceItem> InvoiceItemList = inventryService.GetPurchaseOrderInvoiceItemsListByPOInvoiceId(Inventory_PurchaseOrderInvoiceObj.POInvoiceId);
                    if (InvoiceItemList != null && InvoiceItemList.Count > 0)
                    {
                        invoiceObj = inventryService.GetInvoiceByPOInvoiceId(Inventory_PurchaseOrderInvoiceObj.POInvoiceId);
                        IsContainsDisconnect = InvoiceItemList.Any(inv => inv.IsDisconnect != null && inv.IsDisconnect == true);
                        if (IsContainsDisconnect == true)
                        {
                            if (invoiceObj != null && invoiceObj.POInvoiceId > 0)
                            {
                                invoiceObj.IsComplete = false;
                                invoiceObj.ModifiedBy = userId;
                                invoiceObj.ModifiedDate = DateTime.Now;
                                inventryService.SaveOrUpdatePurchaseOrderInvocie(invoiceObj);
                            }
                            script = @"ErrMsg(""Invoice having Disconnect. It cannot be completed!!"");";
                            return JavaScript(script);
                        }
                        else
                        {
                            if (invoiceObj != null && invoiceObj.POInvoiceId > 0)
                            {
                                if (!string.IsNullOrEmpty(GLDate))
                                    invoiceObj.GLDate = DateTime.Parse(GLDate, provider, System.Globalization.DateTimeStyles.NoCurrentDateDefault);
                                invoiceObj.VoucherNumber = Inventory_PurchaseOrderInvoiceObj.VoucherNumber;
                                invoiceObj.ExchangeRate = Inventory_PurchaseOrderInvoiceObj.ExchangeRate;
                                invoiceObj.IsComplete = true;
                                invoiceObj.ModifiedBy = userId;
                                invoiceObj.ModifiedDate = DateTime.Now;
                                inventryService.SaveOrUpdatePurchaseOrderInvocie(invoiceObj);
                            }
                            script = @"SucessMsg(""Invoice Completed Successfully!!"");";
                            return JavaScript(script);
                        }
                    }
                    script = @"ErrMsg(""Transaction Error Occured!!"");";
                    return JavaScript(script);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public ActionResult GetExchangeRateByGLDateandCurrency(string GLDate, string Currency)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    MastersService mssvc = new MastersService();
                    Inventory_ExchangeRateMaster rateObj = new Inventory_ExchangeRateMaster();
                    Inventory_CurrencyMaster currencyObj = new Inventory_CurrencyMaster();
                    currencyObj = mssvc.GetCurrencyDetailsByCurrency(Currency);
                    if (currencyObj != null && currencyObj.Currency_Id > 0)
                        rateObj = mssvc.GetExchangeRateByGLDateandCurrency(currencyObj.Currency_Id, GLDate);
                    if (rateObj != null && rateObj.Rate_Id > 0)
                    {
                        return Json(rateObj.ExchangeRate, JsonRequestBehavior.AllowGet);
                    }
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion
        #region InvoiceManagement
        public ActionResult PurchaseOrderInvoiceCreation()
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

                throw ex;
            }
        }
        public JsonResult PurchaseOrderInvoiceDetailsListJQGrid(int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                InventoryManagementService inventryService = new InventoryManagementService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                Dictionary<string, object> likeCriteria = new Dictionary<string, object>();

                if (!string.IsNullOrWhiteSpace(sord) && sord == "desc")
                    sord = "Desc";
                else
                    sord = "Asc";
                Dictionary<long, IList<Inventory_PurchaseOrderInvoice>> poInvoiceItems = new Dictionary<long, IList<Inventory_PurchaseOrderInvoice>>();
                //if (PurchOrderId > 0)
                //{
                //    criteria.Add("Inventory_PurchaseOrder.PurchOrderId", PurchOrderId);
                //}
                poInvoiceItems = inventryService.GetPurchaseOrderInvoiceItemListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria, likeCriteria);

                if (poInvoiceItems != null && poInvoiceItems.Count > 0)
                {
                    long totalrecords = poInvoiceItems.First().Key;
                    int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                    var jsondat = new
                    {
                        total = totalPages,
                        page = page,
                        records = totalrecords,

                        rows = (from items in poInvoiceItems.FirstOrDefault().Value
                                select new
                                {
                                    i = 2,
                                    cell = new string[] {
                                        items.POInvoiceId.ToString(),
                                            items.InvoiceNumber,
                                            items.InvoiceNumber,
                                            items.Inventory_PurchaseOrderInvoiceMappingList.Count.ToString(),
                                            items.Inventory_PurchaseOrderInvoiceMappingList.Count>0?items.Inventory_PurchaseOrderInvoiceMappingList.FirstOrDefault().PurchOrderId.ToString():"",
                                            items.InvoiceDate.Value.ToString("dd/MM/yyyy"),
                                            items.InvoiceAmount.ToString(),
                                            items.RemainingAmount.ToString(),
                                            items.InvoiceAmountUSD.ToString(),
                                            items.ExchangeRate.ToString(),
                                            items.ContainerNumber,
                                            items.BillOfLading,
                                            items.GLDate!=null?items.GLDate.Value.ToString("dd/MM/yyyy"):"",
                                            items.VoucherNumber.ToString(),
                                            items.IsComplete==true?"Completed":"OPEN",
                                            items.CreatedBy,
                                            items.CreatedDate.Value.ToString("dd/MM/yyyy")
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

                throw ex;
            }
        }
        public JsonResult GetPODetailsforInvoiceByInvoiceId(string PurchOrderIds)
        {
            try
            {
                if (!string.IsNullOrEmpty(PurchOrderIds) && PurchOrderIds != "null")
                {
                    InventoryManagementService inventoryService = new InventoryManagementService();
                    Dictionary<string, object> criteria = new Dictionary<string, object>();
                    Dictionary<long, IList<Inventory_PurchaseOrder>> POMasterList = null;

                    string[] strPOIdArr = PurchOrderIds.Split(new string[] { ",", null }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    long[] POIdArr = Array.ConvertAll(strPOIdArr, element => Convert.ToInt64(element));
                    criteria.Clear();
                    if (POIdArr.Length > 0)
                        criteria.Add("PurchOrderId", POIdArr);
                    POMasterList = inventoryService.GetPONumberWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                    var PODetailsList = (from u in POMasterList.FirstOrDefault().Value select new { u.SupplierNumber, u.Supplier, u.POCurrency }).Distinct().ToList();
                    return Json(PODetailsList, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var jsondat = new { rows = (new { cell = new string[] { } }) };
                    return Json(jsondat, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion
        #region Invoice key Form
        public ActionResult POInvoiceManagementForm(long POInvoiceId)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    InventoryManagementService inventryService = new InventoryManagementService();
                    Inventory_PurchaseOrderInvoice poInvoice = new Inventory_PurchaseOrderInvoice();
                    poInvoice = inventryService.GetInvoiceByPOInvoiceId(POInvoiceId);
                    return View(poInvoice);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public JsonResult PurchaseOrderInvoiceItemDetailsListJQGrid(long POInvoiceId, Inventory_PurchaseOrderInvoiceItem_VW poItemObj, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                InventoryManagementService inventryService = new InventoryManagementService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();

                if (!string.IsNullOrWhiteSpace(sord) && sord == "desc")
                    sord = "Desc";
                else
                    sord = "Asc";

                Dictionary<long, IList<Inventory_PurchaseOrderInvoiceItem_VW>> poInvoiceItems = new Dictionary<long, IList<Inventory_PurchaseOrderInvoiceItem_VW>>();
                if (POInvoiceId > 0)
                    criteria.Add("POInvoiceId", POInvoiceId);
                if (poItemObj != null)
                {
                    if (poItemObj.UNCode > 0)
                        criteria.Add("UNCode", poItemObj.UNCode);
                    if (!string.IsNullOrEmpty(poItemObj.Commodity))
                        criteria.Add("Commodity", poItemObj.Commodity);
                    if (poItemObj.OrderedQty > 0)
                        criteria.Add("OrderedQty", poItemObj.OrderedQty);
                    if (poItemObj.POUnitPrice > 0)
                        criteria.Add("POUnitPrice", poItemObj.POUnitPrice);
                    if (poItemObj.POValue > 0)
                        criteria.Add("POValue", poItemObj.POValue);
                }
                poInvoiceItems = inventryService.GetPurchaseOrderInvoiceItemListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                if (poInvoiceItems != null && poInvoiceItems.Count > 0)
                {
                    long totalrecords = poInvoiceItems.First().Key;
                    int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                    var jsondat = new
                    {
                        total = totalPages,
                        page = page,
                        records = totalrecords,

                        rows = (from items in poInvoiceItems.FirstOrDefault().Value
                                select new
                                {
                                    i = 2,
                                    cell = new string[] {
                                        items.Id.ToString(),
                                            items.PurchOrderId.ToString(),
                                            items.POInvoiceItemId.ToString(),
                                            items.INVConfig_Id.ToString(),
                                            items.PO.ToString(),
                                            items.POLineId.ToString(),
                                            items.UNCode.ToString(),
                                            items.Commodity,
                                            items.OrderedQty.ToString(),
                                            items.POUnitPrice.ToString(),
                                            items.POValue.ToString(),
                                            items.InvoicedQty.ToString(),
                                            items.RemainingQty.ToString(),
                                            items.InvoiceUnitPrice!=0?items.InvoiceUnitPrice.ToString():items.POUnitPrice.ToString(),
                                            Convert.ToString("0"),
                                            items.InvoiceQty!=0?items.InvoiceQty.ToString():Convert.ToString("0"),
                                            items.Remarks!=null?items.Remarks:"",
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

                throw ex;
            }
        }
        #endregion
        public decimal updateInvoicedAmountinPurchaseOrderInvoicebyInvoiceId(long POInvoiceId)
        {
            try
            {
                InventoryManagementService inventryService = new InventoryManagementService();
                Inventory_PurchaseOrderInvoice invoiceObj = new Inventory_PurchaseOrderInvoice();
                invoiceObj = inventryService.GetInvoiceByPOInvoiceId(POInvoiceId);
                IList<Inventory_PurchaseOrderInvoiceItem> InvoiceItemvalueList = inventryService.GetPurchaseOrderInvoiceItemsListByPOInvoiceId(POInvoiceId);
                invoiceObj.InvoiceAmountUSD = InvoiceItemvalueList.Sum(x => x.InvoiceValue);
                invoiceObj.RemainingAmount = invoiceObj.InvoiceAmount - invoiceObj.InvoiceAmountUSD;
                inventryService.SaveOrUpdatePurchaseOrderInvocie(invoiceObj);
                return invoiceObj.InvoiceAmountUSD;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
