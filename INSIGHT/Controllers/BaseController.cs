using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using INSIGHT.Entities;
using INSIGHT.WCFServices;
using System.Web;
using System.IO;
using System.Globalization;
using System.Text;
using INSIGHT.Entities.InvoiceEntities;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style;
using INSIGHT.Entities.PDFEntities;
using Microsoft.Build.Utilities;
using System.Threading.Tasks;


namespace INSIGHT.Controllers
{
    public class BaseController : Controller
    {
        MastersService ms = new MastersService();
        Dictionary<string, object> criteria = new Dictionary<string, object>();
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
        }

        /// <summary>
        /// Return the exception message
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public JsonResult ThrowJSONError(Exception e)
        {
            Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            Response.TrySkipIisCustomErrors = true;
            //Log your exception
            return Json(new { Message = e.Message }, "text/x-json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }

        public void ExptToXL<T, TResult>(IList<T> stuList, string filename, Func<T, TResult> selector)
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
            Response.Write(stw.ToString());
            Response.End();
        }
        public ActionResult SignOut()
        {
            Session.Abandon();
            Response.Cookies["ASP.NET_SessionID"].Value = null;
            return new EmptyResult();
        }
        public string ValidateUser()
        {
            string userId = (Session["UserId"] == null) ? "" : Session["UserId"].ToString();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return "";
            }
            else return userId;
        }
        /// <summary>
        /// Return the exception message
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public JsonResult ThrowJSONErrorNew(Exception e, String exPolicyName)
        {
            Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            Response.TrySkipIisCustomErrors = true;
            //Log your exception
            ExceptionPolicy.HandleException(e, exPolicyName);
            return Json(new { Message = e.Message }, "text/x-json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        #region "Email methods"
        public string GetContentTypeByFileExtension(string FileExtension)
        {
            string ContentType = "";
            switch (FileExtension)
            {
                case ".bmp":
                    ContentType = "image/bmp";
                    break;
                case ".gif":
                    ContentType = "image/gif";
                    break;
                case ".jpeg":
                    ContentType = "image/jpeg";
                    break;
                case ".jpg":
                    ContentType = "image/jpeg";
                    break;
                case ".png":
                    ContentType = "image/png";
                    break;
                case ".tif":
                    ContentType = "image/tiff";
                    break;
                case ".tiff":
                    ContentType = "image/tiff";
                    break;
                //'Documents'
                case ".doc":
                    ContentType = "application/msword";
                    break;
                case ".docx":
                    ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    break;
                case ".pdf":
                    ContentType = "application/pdf";
                    break;
                //'Slideshows'
                case ".ppt":
                    ContentType = "application/vnd.ms-powerpoint";
                    break;
                case ".pptx":
                    ContentType = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                    break;
                //'Data'
                case ".xlsx":
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    break;
                case ".xls":
                    ContentType = "application/vnd.ms-excel";
                    break;
                case ".csv":
                    ContentType = "text/csv";
                    break;
                case ".xml":
                    ContentType = "text/xml";
                    break;
                case ".txt":
                    ContentType = "text/plain";
                    break;
                //'Compressed Folders'
                case ".zip":
                    ContentType = "application/zip";
                    break;
                //'Audio'
                case ".ogg":
                    ContentType = "application/ogg";
                    break;
                case ".mp3":
                    ContentType = "audio/mpeg";
                    break;
                case ".wma":
                    ContentType = "audio/x-ms-wma";
                    break;
                case ".wav":
                    ContentType = "audio/x-wav";
                    break;
                //'Video'
                case ".wmv":
                    ContentType = "audio/x-ms-wmv";
                    break;
                case ".swf":
                    ContentType = "application/x-shockwave-flash";
                    break;
                case ".avi":
                    ContentType = "video/avi";
                    break;
                case ".mp4":
                    ContentType = "video/mp4";
                    break;
                case ".mpeg":
                    ContentType = "video/mpeg";
                    break;
                case ".mpg":
                    ContentType = "video/mpeg";
                    break;
                case ".qt":
                    ContentType = "video/quicktime";
                    break;
                default:
                    ContentType = "text/plain";
                    break;
            }
            return ContentType;
        }
        #endregion

        public DataTable ListToDataTable<T>(List<T> list)
        {
            DataTable dt = new DataTable();

            foreach (PropertyInfo info in typeof(T).GetProperties())
            {
                Type propType = info.PropertyType;
                if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    propType = Nullable.GetUnderlyingType(propType);
                }
                dt.Columns.Add(new DataColumn(info.Name, propType));
            }
            foreach (T t in list)
            {
                DataRow row = dt.NewRow();
                foreach (PropertyInfo info in typeof(T).GetProperties())
                {
                    object colVal = info.GetValue(t, null);
                    if (colVal != null)
                    {
                        row[info.Name] = colVal.ToString();
                    }
                    else
                    {
                        row[info.Name] = DBNull.Value;
                    }
                }
                dt.Rows.Add(row);
            }
            return dt;
        }

        //added by JP with felix to check whether an email id is valid or not as for as syntax is concerned
        public bool ValidEmailOrNot(string emailId)
        {
            bool valid = false;

            if (!string.IsNullOrEmpty(emailId) && Regex.IsMatch(emailId,
                      @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$",
                      RegexOptions.IgnoreCase))
            {
                valid = true;
            }
            return valid;
        }
        //added by JP with anbu to email with a new task so that the current thread will not wait for the email sending time
        public bool SendEmailWithForNewTask(System.Net.Mail.MailMessage mail, SmtpClient smtp)
        {
            bool retValue = false;
            try
            {
                smtp.Send(mail);
                retValue = true;
            }
            catch (Exception) { return retValue; }
            return retValue;
        }
        //added by JP with anbu to store the user log off while clicking logout link and will be called with new task
        public void UpdateUserLogoff(string userId)
        {
            UserService us = new UserService();
            Dictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("UserId", userId);
            Dictionary<long, IList<Session>> sessionList = us.GetSessionListWithPaging(0, 100, "Desc", "Id", criteria);
            if (sessionList != null && sessionList.First().Value != null && sessionList.First().Value.Count > 0)
            {
                sessionList.FirstOrDefault().Value[0].TimeOut = DateTime.Now;
                us.UpdateSession(sessionList.FirstOrDefault().Value[0]);
            }
        }
        #region Added by Micheal
        public ActionResult FillAllSectorCode()
        {
            try
            {
                MastersService mssvc = new MastersService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                Dictionary<long, IList<SectorMaster>> SectorList = mssvc.GetSectorMasterListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                ViewBag.Sector = SectorList.FirstOrDefault().Value;
                if (SectorList != null && SectorList.First().Value != null && SectorList.First().Value.Count > 0)
                {
                    var UserType = (
                             from items in SectorList.First().Value

                             select new
                             {
                                 Text = items.SectorCode,
                                 Value = items.SectorCode
                             }).Distinct().ToList().OrderBy(x => x.Text);

                    return Json(UserType, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(null, JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "CustomAccountPolicy");
                throw ex;
            }
        }
        public ActionResult AppCodeddl()
        {
            try
            {
                string userId = ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    UserService aps = new UserService();
                    Dictionary<long, string> appcd = new Dictionary<long, string>();
                    Dictionary<string, object> criteria = new Dictionary<string, object>();
                    Dictionary<long, IList<Application>> application = aps.GetApplicationListWithPagingAndCriteria(0, 9999, null, null, criteria);
                    foreach (Application app in application.First().Value)
                    {
                        appcd.Add(app.Id, app.AppCode);
                    }
                    return PartialView("Select", appcd);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "CallMgmntPolicy");
                throw ex;
            }
        }

        public ActionResult RoleCodeddl()
        {
            try
            {
                string userId = ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    UserService rcs = new UserService();
                    Dictionary<long, string> rlcd = new Dictionary<long, string>();
                    Dictionary<string, object> criteria = new Dictionary<string, object>();
                    Dictionary<long, IList<Role>> role = rcs.GetRoleListWithPagingAndCriteria(0, 9999, null, null, criteria);
                    foreach (Role rol in role.First().Value)
                    {
                        rlcd.Add(rol.Id, rol.RoleCode);
                    }
                    return PartialView("Select", rlcd);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "CallMgmntPolicy");
                throw ex;
            }
        }
        public ActionResult SectorCodeddl()
        {
            try
            {
                MastersService mssvc = new MastersService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                Dictionary<long, IList<SectorMaster>> SectorList = mssvc.GetSectorMasterListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                ViewBag.Sector = SectorList.FirstOrDefault().Value;
                Dictionary<long, string> brncd = new Dictionary<long, string>();

                foreach (SectorMaster brnch in SectorList.First().Value)
                {
                    brncd.Add(brnch.Id, brnch.SectorCode);
                }
                return PartialView("Select", brncd);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "CallMgmntPolicy");
                throw ex;
            }
        }

        public ActionResult DocmentUpload(HttpPostedFileBase[] file2)
        {
            MastersService mssvc = new MastersService();
            for (int i = 0; i < file2.Length; i++)
            {
                string[] strAttachname = file2[i].FileName.Split('\\');

                //string path = file1.InputStream.ToString();
                byte[] imageSize = new byte[file2[i].ContentLength];
                file2[i].InputStream.Read(imageSize, 0, (int)file2[i].ContentLength);
                DocumentUpload doc = new DocumentUpload();
                doc.Document = imageSize;
                doc.DocumentName = strAttachname.First().ToString();
                doc.DocumentType = file2[i].ContentType;
                string path = Directory.GetCurrentDirectory();
                //file.SaveAs(path);
                mssvc.CreateOrUpdateDocumentUpload(doc);

            }
            return Json(new { success = true }, "text/html", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region  Date time format
        public static string ConvertDateTimeToDate(string dateTimeString, String langCulture)
        {
            if (!string.IsNullOrEmpty(langCulture))
            {
                CultureInfo culture = new CultureInfo(langCulture);
                DateTime dt = DateTime.MinValue;

                if (DateTime.TryParse(dateTimeString, out dt))
                {
                    return dt.ToString("d", culture);
                }
            }
            else
            {
                langCulture = "en-GB";
                CultureInfo culture = new CultureInfo(langCulture);
                DateTime dt = DateTime.MinValue;

                if (DateTime.TryParse(dateTimeString, out dt))
                {
                    return dt.ToString("d", culture);
                }
            }
            return dateTimeString;
        }


        #endregion
        public static string NumberToText(long number)
        {
            StringBuilder wordNumber = new StringBuilder();

            string[] powers = new string[] { "Thousand ", "Million ", "Billion " };
            string[] tens = new string[] { "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };
            string[] ones = new string[] { "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten",
"Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };

            if (number == 0) { return "Zero"; }
            if (number < 0)
            {
                wordNumber.Append("Negative ");
                number = -number;
            }

            long[] groupedNumber = new long[] { 0, 0, 0, 0 };
            int groupIndex = 0;

            while (number > 0)
            {
                groupedNumber[groupIndex++] = number % 1000;
                number /= 1000;
            }

            for (int i = 3; i >= 0; i--)
            {
                long group = groupedNumber[i];

                if (group >= 100)
                {
                    wordNumber.Append(ones[group / 100 - 1] + " Hundred ");
                    group %= 100;

                    if (group == 0 && i > 0)
                        wordNumber.Append(powers[i - 1]);
                }

                if (group >= 20)
                {
                    if ((group % 10) != 0)
                        wordNumber.Append(tens[group / 10 - 2] + " " + ones[group % 10 - 1] + " ");
                    else
                        wordNumber.Append(tens[group / 10 - 2] + " ");
                }
                else if (group > 0)
                    wordNumber.Append(ones[group - 1] + " ");

                if (group != 0 && i > 0)
                    wordNumber.Append(powers[i - 1]);
            }

            return wordNumber.ToString().Trim();
        }


        public class DateFormatInseries : IFormatProvider, ICustomFormatter
        {
            public object GetFormat(Type formatType)
            {
                if (formatType == typeof(ICustomFormatter))
                    return this;
                else
                    return null;
            }

            public string Format(string fmt, object arg, IFormatProvider formatProvider)
            {
                var monthYear = ((DateTime)arg).ToString(" MMMM yyyy");
                var day = ((DateTime)arg).ToString("d ").Trim();

                switch (((DateTime)arg).Day)
                {
                    case 1:
                    case 21:
                    case 31:
                        day += "st";
                        break;
                    case 2:
                    case 22:
                        day += "nd";
                        break;
                    case 3:
                    case 23:
                        day += "rd";
                        break;
                    default:
                        day += "th";
                        break;
                }
                return day + " " + monthYear;
            }
        }
        /// <summary>
        /// Saving The History of Downloads
        /// </summary>
        /// <param name="ed"></param>
        /// <param name="pd"></param>
        /// <param name="searchItems"></param>
        /// <param name="byteArray"></param>
        public void SaveDocumentToRecentDownloads(ExcelDocuments ed, PDFDocuments pd, string searchItems, byte[] byteArray, string invType)
        {
            try
            {
                InvoiceService IS = new InvoiceService();
                string userId = ValidateUser();
                RecentDownloads rd = new RecentDownloads();
                if (ed != null)
                {
                    rd.Name = ed.Name;
                    rd.ContingentType = ed.ContingentType;
                    rd.Location = ed.Location;
                    rd.ControlId = ed.ControlId;
                    rd.Period = ed.Period;
                    rd.PeriodYear = ed.PeriodYear;
                    rd.Week = ed.Week;
                    rd.Sector = ed.Sector;
                    rd.DocumentType = "Excel";
                    rd.DocumentData = ed.DocumentData;
                    rd.DocumentName = ed.DocumentName;
                }
                else if (pd != null)
                {
                    rd.Name = pd.Name;
                    rd.ContingentType = pd.ContingentType;
                    rd.Location = pd.Location;
                    rd.ControlId = pd.ControlId;
                    rd.Period = pd.Period;
                    rd.PeriodYear = pd.PeriodYear;
                    rd.Week = pd.Week;
                    rd.Sector = pd.Sector;
                    rd.DocumentType = "PDF";
                    rd.DocumentData = pd.DocumentData;
                    rd.DocumentName = pd.DocumentName;
                }
                else
                {
                    if (searchItems != null && searchItems != "")
                    {

                        var Items = searchItems.ToString().Split(',');
                        if (!string.IsNullOrWhiteSpace(invType) && invType == "Excel-Consol")
                        {
                            Items[0] = "Consol";
                            Items[2] = "Excel";
                            rd.ControlId = "GCC-" + Items[0] + "-" + Items[2] + "-" + Items[3] + "-" + Items[4];
                            rd.DocumentName = "GCC-" + Items[0] + "-" + Items[2] + "-" + Items[3] + "-" + Items[4];
                        }
                        else if (!string.IsNullOrWhiteSpace(invType) && invType == "PDF-Consol")
                        {
                            Items[0] = "Consol";
                            Items[2] = "PDF";
                            rd.ControlId = "GCC-" + Items[0] + "-" + Items[2] + "-" + Items[3] + "-" + Items[4];
                            rd.DocumentName = "GCC-" + Items[0] + "-" + Items[2] + "-" + Items[3] + "-" + Items[4];
                        }
                        else
                        {
                            rd.ControlId = "GCC-" + Items[0] + "-" + Items[2] + "-" + Items[3] + "-" + Items[4];
                            rd.DocumentName = "GCC-" + Items[0] + "-" + Items[2] + "-" + Items[3] + "-" + Items[4];
                            rd.Sector = Items[0];
                            rd.Name = Items[1];
                            rd.ContingentType = Items[2];
                        }
                        rd.Period = Items[3];
                        rd.PeriodYear = Items[4];
                        rd.DocumentType = "ZIP";
                        rd.DocumentData = byteArray;

                    }
                }
                IS.SaveRecentDownloads(rd, userId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void NewExportToExcel<T, TResult>(IList<T> List, string filename, Func<T, TResult> selector, List<string> lstHeader)
        {

            ExcelPackage pck = new ExcelPackage();
            pck.Workbook.Properties.Author = "John Desouza A";
            pck.Workbook.Properties.Title = "Export To Excel";
            pck.Workbook.Properties.Company = "GCC Services";

            var ws = pck.Workbook.Worksheets.Add(" " + filename + " Records");
            ws.View.ZoomScale = 85;
            ws.View.ShowGridLines = false;
            ws.Row(1).Height = 28.50;

            //Color Selection
            Color WhiteHex = System.Drawing.ColorTranslator.FromHtml("#F2F2F2");
            Color GreenHex = System.Drawing.ColorTranslator.FromHtml("#70AD47");

            //Header Section
            for (int i = 0; i < lstHeader.Count; i++)
            {
                ws.Cells[1, i + 1].Value = lstHeader[i];
                ws.Cells[1, i + 1].Style.Font.Bold = true;
                ws.Cells[1, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[1, i + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(GreenHex);
                ws.Cells[1, i + 1].Style.Font.Color.SetColor(Color.White);
                ws.Cells[1, i + 1].AutoFitColumns(15);
            }

            //Loading the Collection With Selected List
            ws.Cells["A2"].LoadFromCollection(List.Select(selector), false);

            int Rowcount = List.Count() + 2;
            int columnCount = lstHeader.Count() + 1;

            //Modified by kingston

            ws.Cells["A2:V" + Rowcount].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells["A2:V" + Rowcount].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            ws.Cells["A2:V" + Rowcount].Style.Border.Bottom.Color.SetColor(GreenHex);
            ws.Cells["A2:V" + Rowcount].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            ws.Cells["A2:V" + Rowcount].Style.Border.Left.Color.SetColor(GreenHex);
            ws.Cells["A2:V" + Rowcount].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            ws.Cells["A2:V" + Rowcount].Style.Border.Right.Color.SetColor(GreenHex);


            ////Borders Matrix Logic
            //for (int i = 1; i < Rowcount; i++)
            //{
            //    for (int j = 1; j < columnCount; j++)
            //    {
            //        ws.Cells[i, j].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            //        ws.Cells[i, j].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            //        ws.Cells[i, j].Style.Border.Bottom.Color.SetColor(GreenHex);
            //        ws.Cells[i, j].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            //        ws.Cells[i, j].Style.Border.Left.Color.SetColor(GreenHex);
            //        ws.Cells[i, j].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            //        ws.Cells[i, j].Style.Border.Right.Color.SetColor(GreenHex);
            //    }
            //}

            //Auto filter's for columns
            ws.Cells[1, 1, 1, (columnCount - 1)].AutoFilter = true;
            //Auto fit
            for (int k = 1; k < columnCount; k++)
            {
                // Get all column's cells
                ExcelRange columnCells = ws.Cells[ws.Dimension.Start.Row, k, ws.Dimension.End.Row, k];
                // Check what is the longest string and set the length
                int maxLength = columnCells.Max(cell => Convert.ToString(cell.Value = cell.Value != null ? cell.Value : " ").Count(c => char.IsLetterOrDigit(c)));
                ws.Column(k).Width = maxLength + 7; // 2 is just an extra buffer for all that is not letter/digits.
            }

            if (!string.IsNullOrWhiteSpace(filename))
            {
                string[] Excel = new string[] { "OrderItemsReport", "WeekInvoiceReport" };
                var Items = filename.ToString().Split('-');
                if (Excel.Contains(Items[0]))
                {
                    ws = CustumformatedStyles(filename, ws);
                }
            }

            byte[] data = pck.GetAsByteArray();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
            Response.BinaryWrite(data);
            Response.End();
        }

        private ExcelWorksheet CustumformatedStyles(string filename, ExcelWorksheet ws)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(filename))
                {
                    var Items = filename.ToString().Split('-');
                    if (!string.IsNullOrWhiteSpace(Items[0]) && Items[0] == "OrderItemsReport" || !string.IsNullOrWhiteSpace(Items[0]) && Items[0] == "WeekInvoiceReport")
                    {
                        int[] no = new int[] { 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 25, 26,28 };
                        foreach (var item in no)
                        {
                            ws.Column(item).Style.Numberformat.Format = "#,##0.000";
                        }
                        //ws.Column(28).Style.Numberformat.Format = "dd-MMM-yyyy";
                        //ws.Column(29).Style.Numberformat.Format = "dd-MMM-yyyy";
                    }
                    var Items1 = filename.ToString().Split('P');
                    if (!string.IsNullOrWhiteSpace(Items1[0]) && Items1[0] == "GCCRevised")
                    {
                        int[] no = new int[] { 9, 12, 13, 14 };
                        foreach (var item in no)
                        {
                            ws.Column(item).Style.Numberformat.Format = "#,##0.000";
                        }
                        ws.Column(3).Style.Numberformat.Format = "dd-MMM-yyyy";
                        ws.Column(6).Style.Numberformat.Format = "dd-MMM-yyyy";
                    }
                }

                return ws;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SaveOrUpdateOrderitemsReports(Orders Ord, SingleInvoice si, InvoiceReports Ir)
        {
            try
            {
                if (Ord != null && si != null)
                {
                    bool IsWeeklyDiscount = Convert.ToBoolean(ConfigurationManager.AppSettings["IsWeeklyDiscount"]);
                    InvoiceService IS = new InvoiceService();
                    Decimal APLDeduction = 0;
                    string userId = "";

                    //if (ValidateUser() != "")
                    //    userId = ValidateUser();
                    //else
                    userId = Ord.CreatedBy;

                    Ir.OrderId = Ord.OrderId;
                    Ir.ControlId = Ord.ControlId;
                    Ir.Location = Ord.Location;
                    Ir.Strength = Convert.ToInt64(Ord.Troops);
                    Ir.Noofdays = 7;
                    Ir.Week = Ord.Week;
                    Ir.Period = Ord.Period;
                    Ir.PeriodYear = Ord.PeriodYear;
                    Ir.Sector = Ord.Sector;
                    Ir.Contingent = Ord.Name;
                    Ir.ContingentType = Ord.ContingentType;
                    Ir.Lineitemordered = Convert.ToInt64(Ord.LineItemsOrdered);
                    Ir.Totallineitemsubstituted = si.SubstituteCount;
                    Ir.Orderedqty = si.TotalOrderedQtySum;
                    //Ir.Deliveredqty = si.TotalDeliveredQtySum + si.SDeliveryQuantity + si.RDeliveryQuantity;
                    Ir.Deliveredqty = si.TotalDeliveredQtySum;
                    Ir.Acceptedqty = si.TotalInvoiceQtySum + si.SAcceptedQuantity + si.RAcceptedQuantity;
                    Ir.Amountordered = si.OrdervalueSum;
                    //decimal AmtAccptd = si.NetAmountSum + si.SAcceptedamt + si.RAcceptedamt;
                    Ir.Amountaccepted = si.AmountAccepted;
                    Ir.Troopstrengthdiscount = 0;
                    Ir.Othercreditnotes = 0;
                    //Ir.Weeklyinvoicediscount = 0;
                    //Ir.Netamountforrations = si.NetAmountSum + si.SAcceptedamt + si.RAcceptedamt;

                    //Weekly Discount by Thamizhmani
                    //decimal WeekDiscnt = 0; decimal NetRationAmount = 0;
                    //if (IsWeeklyDiscount == true)
                    //{
                    //    WeekDiscnt = Math.Round(Convert.ToDecimal(0.35 / 100) * (AmtAccptd), 2);
                    //    NetRationAmount = AmtAccptd - WeekDiscnt;
                    //    Ir.Weeklyinvoicediscount = WeekDiscnt;
                    //    Ir.Netamountforrations = NetRationAmount;
                    //}
                    //else
                    //{
                    //    Ir.Weeklyinvoicediscount = WeekDiscnt;
                    //    Ir.Netamountforrations = AmtAccptd - WeekDiscnt; ;
                    //}

                    Ir.Weeklyinvoicediscount = si.Weeklyinvoicediscount;
                    Ir.Netamountforrations = si.NetRationAmount;

                    Ir.AplTimelydelivery = si.DeliveryDeduction;
                    Ir.AplOrderbylineitems = si.LineItemDeduction;
                    Ir.AplOrdersbyweight = si.OrderWightDeduction;
                    Ir.AplNoofauthorizedsubstitutions = si.SubtitutionDeduction;


                    //Ir.Totalinvoiceamount = ((si.NetAmountSum + si.SAcceptedamt + si.RAcceptedamt) - (si.DeliveryDeduction + si.LineItemDeduction + si.OrderWightDeduction + si.SubtitutionDeduction));
                    //Ir.Totalinvoiceamount = si.NetRationAmount;
                    APLDeduction = Math.Round(si.DeliveryDeduction + si.LineItemDeduction + si.OrderWightDeduction + si.SubtitutionDeduction, 2);
                    Ir.Totalinvoiceamount = Math.Round((si.NetRationAmount - APLDeduction) + si.AcceptedTransportCost, 2);

                    Ir.Modeoftransportation = si.DeliveryMode;
                    Ir.Distancekm = 0;
                    Ir.Transportationperkgcost = 0;
                    Ir.Totaltransportationcost = 0;
                    Ir.Dn = "";
                    Ir.Approveddeliverydate = (DateTime)Ord.ExpectedDeliveryDate;
                    Ir.Actualdateofreceipt = si.ApprovedDeliveryDate;
                    Ir.Daysdelay = si.TotalDays;
                    Ir.Authorizedcmr = si.AuthorizedCMR;
                    Ir.Ordercmr = si.OrderCMR;
                    Ir.Acceptedcmr = si.AcceptedCMR;
                    Ir.Cmrutilized = si.CMRUtilized;
                    Ir.Lineitemdelivered98 = Convert.ToInt64(si.TotalLineitem98);
                    Ir.Confirmitytolineitemorder98 = si.LineItemPerformance;
                    Ir.Conformitytoorderbyweight = si.OrderWightPerformance;
                    Ir.Noofsubtitution = si.SubtitutionPerformance;
                    Ir.Daysdelayperformance = si.DeliveryPerformance;
                    Ir.Amountsubstituted = si.AmountSubstituted;
                    Ir.DeliveryNotes = si.Deliverynotes;
                    Ir.Totaltransportationcost = si.AcceptedTransportCost;
                    Ir.ContingentID = si.ContingentID;
                    IS.SaveOrUpdateInvoiceReports(Ir, userId);
                }
            }
            catch (Exception ex)
            {
                #region saveorupdate Errorlog
                OrdersService OrdSer = new OrdersService();
                ErrorLog err = new ErrorLog();
                err.Controller = "Base";
                err.Action = "SaveOrUpdateOrderitemsReports";
                err.Err_Desc = ex.ToString();
                OrdSer.SaveOrUpdateErrorLog(err);
                #endregion
            }
        }

        public PenaltyCaculation GetPenaltyCalculationValues(PenaltyCaculation pcl)
        {
            try
            {
                #region Delivery

                //Conformity to DeliveryPerformance

                //if (-3 <= pcl.TotalDays || pcl.TotalDays <= 3)
                if (pcl.TotalDays >= -3 && pcl.TotalDays <= 3)
                    pcl.DeliveryPerformance = 0;
                else if (pcl.TotalDays > 0 && pcl.TotalDays >= 3)
                    pcl.DeliveryPerformance = Math.Round((decimal)pcl.TotalDays - 3, 2);
                else if (pcl.TotalDays < 0 && pcl.TotalDays <= -3)
                    pcl.DeliveryPerformance = Math.Round((decimal)pcl.TotalDays + 3, 2);

                //Conformity to DeliveryDeduction
                if (pcl.DeliveryPerformance <= 0 && pcl.DeliveryPerformance >= -3)
                    pcl.DeliveryDeduction = 0;
                else if (pcl.DeliveryPerformance == 1 || pcl.DeliveryPerformance == -1)
                    pcl.DeliveryDeduction = Math.Round(((decimal)1.20 / 100) * pcl.OrdervalueSum, 2);
                else
                    pcl.DeliveryDeduction = Math.Round(((decimal)3.00 / 100) * pcl.OrdervalueSum, 2);

                //if (pcl.DeliveryPerformance <= 0)
                //    pcl.DeliveryDeduction = 0;
                //else if (pcl.DeliveryPerformance == 1)
                //    pcl.DeliveryDeduction = ((decimal)1.20 / 100) * pcl.OrdervalueSum;
                //else
                //    pcl.DeliveryDeduction = ((decimal)3.00 / 100) * pcl.OrdervalueSum;

                #endregion

                #region LineItem

                //Conformity to LineItemPerformance

                if (pcl.TotalLineitem == 0 || pcl.TotalLineitem98 == 0)
                    pcl.LineItemPerformance = 0;
                else
                    pcl.LineItemPerformance = Math.Round((pcl.TotalLineitem98 / pcl.TotalLineitem) * 100, 2);

                //Conformity to LineItemDeduction

                if (pcl.LineItemPerformance < (decimal)95.00)
                    pcl.LineItemDeduction = Math.Round(((decimal)3.00 / 100) * pcl.OrdervalueSum, 2, MidpointRounding.AwayFromZero);
                else if ((pcl.LineItemPerformance < (decimal)98.00) && (pcl.LineItemPerformance >= (decimal)95.00))
                    pcl.LineItemDeduction = Math.Round(((decimal)1.20 / 100) * pcl.OrdervalueSum, 2, MidpointRounding.AwayFromZero);
                else
                    pcl.LineItemDeduction = 0;

                #endregion

                #region OrderWeight

                //Conformity to OrderWeightPerformance

                if (pcl.InvoiceQty == 0 || pcl.OrderedQty == 0)
                    pcl.OrderWeightPerformance = 0;
                else
                    pcl.OrderWeightPerformance = Math.Round((pcl.InvoiceQty / pcl.OrderedQty) * 100, 2);

                //Conformity to OrderWeightDeduction

                if (pcl.OrderWeightPerformance < (decimal)92.00)
                    pcl.OrderWeightDeduction = Math.Round(((decimal)4.50 / 100) * pcl.OrdervalueSum, 2, MidpointRounding.AwayFromZero);
                else if ((pcl.OrderWeightPerformance < (decimal)95.00) && (pcl.OrderWeightPerformance >= (decimal)92.00))
                    pcl.OrderWeightDeduction = Math.Round(((decimal)1.80 / 100) * pcl.OrdervalueSum, 2, MidpointRounding.AwayFromZero);
                else
                    pcl.OrderWeightDeduction = 0;

                #endregion

                #region Complaints

                //Conformity to ComplaintsPerformance

                if (pcl.TotalLineitem == 0 || pcl.SubstituteCount == 0)
                    pcl.ComplaintsPerformance = 0;
                else
                    pcl.ComplaintsPerformance = Math.Round((pcl.SubstituteCount / pcl.TotalLineitem) * 100, 2);

                //Conformity to ComplaintsDeduction

                if (pcl.ComplaintsPerformance >= (decimal)4.00)
                    pcl.ComplaintsDeduction = Math.Round(((decimal)4.50 / 100) * pcl.OrdervalueSum, 2, MidpointRounding.AwayFromZero);
                else if ((pcl.ComplaintsPerformance < (decimal)4.00) && (pcl.ComplaintsPerformance > (decimal)3.00))
                    pcl.ComplaintsDeduction = Math.Round(((decimal)1.80 / 100) * pcl.OrdervalueSum, 2, MidpointRounding.AwayFromZero);
                else
                    pcl.ComplaintsDeduction = 0;

                #endregion

                return pcl;
            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }

        public ActionResult Periodddl()
        {
            try
            {
                string userId = ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    UserService aps = new UserService();
                    Dictionary<long, string> appcd = new Dictionary<long, string>();
                    Dictionary<string, object> criteria = new Dictionary<string, object>();
                    Dictionary<long, IList<PeriodMaster>> PeriodMaster = ms.GetPeriodMasterListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, null);

                    var periodlist = (from items in PeriodMaster.First().Value
                                      select new { items.Period }).Distinct().ToList();
                    long Id = 1;
                    foreach (var item in periodlist)
                    {
                        appcd.Add(Id, item.Period);
                        Id = Id + 1;
                    }
                    return PartialView("Select", appcd);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "CallMgmntPolicy");
                throw ex;
            }
        }
        public ActionResult PeriodYearddl()
        {
            try
            {
                string userId = ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    UserService aps = new UserService();
                    Dictionary<long, string> appcd = new Dictionary<long, string>();
                    Dictionary<string, object> criteria = new Dictionary<string, object>();
                    Dictionary<long, IList<PeriodMaster>> PeriodMaster = ms.GetPeriodMasterListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, null);

                    var periodYearlist = (from items in PeriodMaster.First().Value
                                          select new { items.Year }).Distinct().ToList();
                    long Id = 1;
                    foreach (var item in periodYearlist)
                    {
                        appcd.Add(Id, item.Year);
                        Id = Id + 1;
                    }
                    return PartialView("Select", appcd);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "CallMgmntPolicy");
                throw ex;
            }
        }
        public ActionResult POTypeddl()
        {
            try
            {
                string userId = ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    UserService aps = new UserService();
                    Dictionary<long, string> appcd = new Dictionary<long, string>();
                    Dictionary<string, object> criteria = new Dictionary<string, object>();
                    Dictionary<long, IList<POTypeMaster>> POType = ms.GetPOTypeListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, null);
                    foreach (POTypeMaster pty in POType.First().Value)
                    {
                        appcd.Add(pty.Id, pty.POType);
                    }
                    return PartialView("Select", appcd);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "CallMgmntPolicy");
                throw ex;
            }
        }
        public string GetPONumberFromMaster(string Period, string PeriodYear, string POType)
        {
            try
            {
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                if (!string.IsNullOrWhiteSpace(POType)) { criteria.Add("POType", POType); }
                if (!string.IsNullOrWhiteSpace(Period)) { criteria.Add("Period", Period); }
                if (!string.IsNullOrWhiteSpace(PeriodYear)) { criteria.Add("PeriodYear", PeriodYear); }
                Dictionary<long, IList<POMasters>> POList = null;
                string PONumber = "";
                POList = ms.GetPOMastersListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                if (POList != null && POList.FirstOrDefault().Value.Count > 0 && POList.FirstOrDefault().Key > 0)
                { PONumber = POList.FirstOrDefault().Value[0].PONumber; }
                return PONumber;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public byte[] GenerateByteExcel<T, TResult>(IList<T> List, string filename, Func<T, TResult> selector, List<string> lstHeader)
        {

            ExcelPackage pck = new ExcelPackage();
            pck.Workbook.Properties.Author = "John Desouza A";
            pck.Workbook.Properties.Title = "Export To Excel";
            pck.Workbook.Properties.Company = "GCC Services";

            var ws = pck.Workbook.Worksheets.Add(" " + filename + " Records");
            ws.View.ZoomScale = 85;
            ws.View.ShowGridLines = false;
            ws.Row(1).Height = 28.50;

            //Color Selection
            Color WhiteHex = System.Drawing.ColorTranslator.FromHtml("#F2F2F2");
            Color GreenHex = System.Drawing.ColorTranslator.FromHtml("#70AD47");

            //Header Section
            for (int i = 0; i < lstHeader.Count; i++)
            {
                ws.Cells[1, i + 1].Value = lstHeader[i];
                ws.Cells[1, i + 1].Style.Font.Bold = true;
                ws.Cells[1, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[1, i + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(GreenHex);
                ws.Cells[1, i + 1].Style.Font.Color.SetColor(Color.White);
                ws.Cells[1, i + 1].AutoFitColumns(15);

            }

            //Loading the Collection With Selected List
            ws.Cells["A2"].LoadFromCollection(List.Select(selector), false);

            int Rowcount = List.Count() + 2;
            int columnCount = lstHeader.Count() + 1;

            //Borders Matrix Logic
            for (int i = 1; i < Rowcount; i++)
            {
                for (int j = 1; j < columnCount; j++)
                {
                    ws.Cells[i, j].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells[i, j].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    ws.Cells[i, j].Style.Border.Bottom.Color.SetColor(GreenHex);
                    ws.Cells[i, j].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells[i, j].Style.Border.Left.Color.SetColor(GreenHex);
                    ws.Cells[i, j].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells[i, j].Style.Border.Right.Color.SetColor(GreenHex);
                }
            }



            //Auto filter's for columns
            ws.Cells[1, 1, 1, (columnCount - 1)].AutoFilter = true;
            //Auto fit
            for (int k = 1; k < columnCount; k++)
            {
                // Get all column's cells
                ExcelRange columnCells = ws.Cells[ws.Dimension.Start.Row, k, ws.Dimension.End.Row, k];
                // Check what is the longest string and set the length
                int maxLength = columnCells.Max(cell => Convert.ToString(cell.Value = cell.Value != null ? cell.Value : " ").Count(c => char.IsLetterOrDigit(c)));
                ws.Column(k).Width = maxLength + 7; // 2 is just an extra buffer for all that is not letter/digits.


            }

            if (!string.IsNullOrWhiteSpace(filename))
            {
                string[] Excel = new string[] { "OrderItemsReport", "GCCRevised" };
                var Items = filename.ToString().Split('-');
                if (Excel.Contains(Items[0]))
                {
                    ws = CustumformatedStyles(filename, ws);
                }
            }


            byte[] data = pck.GetAsByteArray();
            return data;
        }


        //Added by kingston for GCCrevised verion excel generation

        public byte[] GenerateByteExcel_GCCRevised<T, TResult>(IList<T> List, string filename, Func<T, TResult> selector, List<string> lstHeader)
        {

            ExcelPackage pck = new ExcelPackage();
            pck.Workbook.Properties.Author = "John Desouza A";
            pck.Workbook.Properties.Title = "Export To Excel";
            pck.Workbook.Properties.Company = "GCC Services";

            var ws = pck.Workbook.Worksheets.Add(" " + filename + " Records");
            ws.View.ZoomScale = 85;
            ws.View.ShowGridLines = false;
            ws.Row(1).Height = 28.50;

            //Color Selection
            Color WhiteHex = System.Drawing.ColorTranslator.FromHtml("#F2F2F2");
            Color GreenHex = System.Drawing.ColorTranslator.FromHtml("#70AD47");

            //Header Section
            for (int i = 0; i < lstHeader.Count; i++)
            {
                ws.Cells[1, i + 1].Value = lstHeader[i];
                ws.Cells[1, i + 1].Style.Font.Bold = true;
                ws.Cells[1, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[1, i + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(GreenHex);
                ws.Cells[1, i + 1].Style.Font.Color.SetColor(Color.White);
                ws.Cells[1, i + 1].AutoFitColumns(15);

            }

            //Loading the Collection With Selected List

            try
            {
                ws.Cells["A2"].LoadFromCollection(List.Select(selector), false);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            int Rowcount = List.Count() + 2;
            int columnCount = lstHeader.Count() + 1;

            //Borders Matrix Logic

            //ws.Cells["A1:T" + Rowcount].Style.Fill.PatternType = ExcelFillStyle.Solid;
            //Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#B7DEE8");
            //ws.Cells["A2:T" + Rowcount].Style.Fill.BackgroundColor.SetColor(colFromHex);
            ws.Cells["A2:T" + Rowcount].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells["A2:T" + Rowcount].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            ws.Cells["A2:T" + Rowcount].Style.Border.Bottom.Color.SetColor(GreenHex);
            ws.Cells["A2:T" + Rowcount].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            ws.Cells["A2:T" + Rowcount].Style.Border.Left.Color.SetColor(GreenHex);
            ws.Cells["A2:T" + Rowcount].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            ws.Cells["A2:T" + Rowcount].Style.Border.Right.Color.SetColor(GreenHex);



            //Auto filter's for columns
            ws.Cells[1, 1, 1, (columnCount - 1)].AutoFilter = true;
            //Auto fit
            for (int k = 1; k < columnCount; k++)
            {
                // Get all column's cells
                ExcelRange columnCells = ws.Cells[ws.Dimension.Start.Row, k, ws.Dimension.End.Row, k];
                // Check what is the longest string and set the length
                int maxLength = columnCells.Max(cell => Convert.ToString(cell.Value = cell.Value != null ? cell.Value : " ").Count(c => char.IsLetterOrDigit(c)));
                ws.Column(k).Width = maxLength + 7; // 2 is just an extra buffer for all that is not letter/digits.


            }

            if (!string.IsNullOrWhiteSpace(filename))
            {
                string[] Excel = new string[] { "OrderItemsReport", "GCCRevised" };
                var Items = filename.ToString().Split('-');
                if (Excel.Contains(Items[0]))
                {
                    ws = CustumformatedStyles(filename, ws);
                }
            }


            byte[] data = pck.GetAsByteArray();
            return data;
        }

        public int GetSerialNoforPeriod(string Period, string PeriodYear, long Week)
        {
            try
            {
                //temp=42 from Period P05 16 by Thamizhmani
                int temp = 42;
                criteria.Clear();
                //criteria.Add("Period", Ord.Period);
                //criteria.Add("Year", Ord.PeriodYear);
                Dictionary<long, IList<PeriodMaster>> PeriodMasterList = null;
                PeriodMasterList = ms.GetPeriodMasterListWithPagingAndCriteria(0, 999999, string.Empty, string.Empty, criteria);
                if (PeriodMasterList != null && PeriodMasterList.FirstOrDefault().Value.Count > 0 && PeriodMasterList.FirstOrDefault().Key > 0)
                {
                    var List = (from items in PeriodMasterList.First().Value
                                select new { items.Period, items.Year, items.Week }).OrderBy(i => i.Period).OrderBy(i => i.Year).Distinct().ToArray();

                    foreach (var item in List)
                    {
                        temp = temp + 1;
                        if ((item.Period == Period) && (item.Year == PeriodYear) && (item.Week == Week))
                        {
                            break;
                        }

                    }
                }
                return temp;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        #region Upload files related
        public void StatusUpdateInUploadRequest(long RequestId)
        {
            try
            {
                OrdersService orderService = new OrdersService();
                UploadRequest upreq = new UploadRequest();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                criteria.Add("RequestId", RequestId);
                Dictionary<long, IList<UploadRequestDetailsLog>> uploadreqdetails = orderService.GetUploadRequestDetailsLogCountListWithPagingAndCriteria(0, 99999, string.Empty, string.Empty, criteria);
                IList<UploadRequestDetailsLog> uploadreqdetailslist = uploadreqdetails.FirstOrDefault().Value.ToList();
                var status = (from u in uploadreqdetailslist select u.UploadStatus).Distinct().ToArray();
                upreq = orderService.GetUploadRequestbyRequestId(RequestId);
                if (status.Length == 1)
                {
                    if (status[0] == "UploadedSuccessfully" && upreq.UploadStatus == "InProgress")
                    {
                        upreq.UploadStatus = "Completed Successfully";
                        orderService.SaveOrUpdateUploadRequest(upreq);
                    }
                    if (status[0] == "UploadFailed" && upreq.UploadStatus == "InProgress")
                    {
                        upreq.UploadStatus = "Completed with Errors";
                        orderService.SaveOrUpdateUploadRequest(upreq);
                    }
                    if (status[0] == "AlreadyExist" && upreq.UploadStatus == "InProgress")
                    {
                        upreq.UploadStatus = "Alread Exist";
                        orderService.SaveOrUpdateUploadRequest(upreq);
                    }

                }
                else
                {
                    for (int i = 0; i < status.Length; i++)
                    {
                        if (status[i] == "UploadFailed" && upreq.UploadStatus == "InProgress")
                        {
                            upreq.UploadStatus = "Completed with Errors";
                            orderService.SaveOrUpdateUploadRequest(upreq);
                        }
                        else
                        {
                            upreq.UploadStatus = "Partially Completed";
                            orderService.SaveOrUpdateUploadRequest(upreq);
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
        //Create or update the upload request detail log.. 
        public long CreateOrUpdateRequest(string fileName, long RequestId)
        {
            try
            {
                OrdersService orderService = new OrdersService();
                UploadRequestDetailsLog uploadlog = new UploadRequestDetailsLog();
                string[] fileNameonly = fileName.Split('\\');

                UploadRequest upreq = orderService.GetUploadRequestbyRequestId(RequestId);

                uploadlog.RequestId = RequestId;
                uploadlog.FileName = fileNameonly[8];
                uploadlog.CreatedBy = upreq.CreatedBy;
                uploadlog.CreatedDate = DateTime.Now;
                uploadlog.UploadStatus = "YetToUpload";
                long UploadReqDetLogId = orderService.SaveOrUpdateUploadRequestDetailsLog(uploadlog);
                return UploadReqDetLogId;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;

            }
        }
        #endregion

    }
}
