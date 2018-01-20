using INSIGHT.Entities.EmailEntities;
using INSIGHT.WCFServices;
using INSIGHT.Entities;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace INSIGHT.Controllers
{
    public class EmailController : BaseController
    {
        OrdersService OS = new OrdersService();
        InvoiceService IS = new InvoiceService();
        MastersService MS = new MastersService();
        EmailService ES = new EmailService();
        UserService US = new UserService();
        Dictionary<string, object> criteria = new Dictionary<string, object>();
        IFormatProvider provider = new System.Globalization.CultureInfo("en-CA", true);

        public ActionResult MailSchedule()
        {
            return View();
        }
        public ActionResult MailScheduleListJQGrid(string searchItems, int rows, string sidx, string sord, int? page = 1)
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
                            if (!string.IsNullOrWhiteSpace(Items[0])) { criteria.Add("MailPeriod", Items[0]); }
                            if (!string.IsNullOrWhiteSpace(Items[1])) { criteria.Add("MailTemplate", Items[1]); }
                            if (!string.IsNullOrWhiteSpace(Items[2])) { criteria.Add("IsActive", Convert.ToBoolean(Items[2])); }
                            if (!string.IsNullOrWhiteSpace(Items[3])) { criteria.Add("Username", Items[3]); }
                            var TrimItems = searchItems.Replace(",", "");
                            if (string.IsNullOrWhiteSpace(TrimItems))
                                return null;
                        }
                        Dictionary<long, IList<EmailScheduleView>> EmailScheduleList = ES.GetEmailScheduleViewListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                        if (EmailScheduleList != null && EmailScheduleList.Count > 0)
                        {
                            long totalrecords = EmailScheduleList.First().Key;
                            int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                            var jsondata = new
                            {
                                total = totalPages,
                                page = page,
                                records = totalrecords,

                                rows = (from items in EmailScheduleList.First().Value
                                        select new
                                        {
                                            i = 2,
                                            cell = new string[] {
                                            items.Id.ToString(),
                                            items.MailTemplate.ToString(),
                                            items.MailPeriod.ToString(),
                                            items.ScheduleDate !=null? items.ScheduleDate.ToString("dd/MM/yyyy") : "",
                                            items.Username,
                                            items.Usermailid,
                                            items.Createdby,
                                            items.IsActive.ToString()
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
                    ExceptionPolicy.HandleException(ex, "EmailServicePolicy");
                    throw ex;
                }
            }
        }

        public ActionResult EmailConfiguration()
        {
            return View();
        }
        public ActionResult TemplateManagement()
        {
            return View();
        }
        public ActionResult TemplateManagementListJQGrid(string searchItems, int rows, string sidx, string sord, int? page = 1)
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
                            if (!string.IsNullOrWhiteSpace(Items[0])) { criteria.Add("MailTemplateMasterId", Convert.ToInt64(Items[0])); }
                            if (!string.IsNullOrWhiteSpace(Items[1])) { criteria.Add("IsActive", Convert.ToBoolean(Items[1])); }
                            if (!string.IsNullOrWhiteSpace(Items[2])) { criteria.Add("UserName", Items[2]); }
                            var TrimItems = searchItems.Replace(",", "");
                            if (string.IsNullOrWhiteSpace(TrimItems))
                                return null;
                        }
                        Dictionary<long, IList<MailTemplate>> MailTemplateList = ES.GetMailTemplateListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                        if (MailTemplateList != null && MailTemplateList.Count > 0)
                        {
                            long totalrecords = MailTemplateList.First().Key;
                            int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                            var jsondata = new
                            {
                                total = totalPages,
                                page = page,
                                records = totalrecords,

                                rows = (from items in MailTemplateList.First().Value
                                        select new
                                        {
                                            i = 2,
                                            cell = new string[] {
                                            String.Format(@"<a style='color:#034af3;text-decoration:underline' href= '/Email/TemplateConfiguration?templateId="+items.MailTemplateId+"'>{0}</a>","REGTEMP00"+items.MailTemplateId),
                                            items.MailTemplateId.ToString(),
                                            items.NewTemplateName,
                                            items.ReportName,
                                            items.UserName,
                                            items.StartDate!=null? ConvertDateTimeToDate(items.StartDate.ToString("dd/MM/yyyy"),"en-GB"):"",
                                            items.IsActive == true? "Active" : "In Active"
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
                    ExceptionPolicy.HandleException(ex, "EmailServicePolicy");
                    throw ex;
                }
            }
        }

        public ActionResult TemplateConfiguration(long? templateId)
        {
            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
            {
                User ul = US.GetUserByUserId(userId);
                MailTemplate mt = new MailTemplate();
                if (templateId != null)
                {
                    mt = ES.GetMailTemplateDetailsById(Convert.ToInt64(templateId));
                    ViewBag.MailTempDDL = "No";
                    mt.UserRefId = ul.Id;
                    mt.UserName = ul.UserId;
                    mt.UserRole = ul.UserType;
                    mt.UserEmail = ul.EmailId;
                    mt.ScheduleDate = mt.StartDate;
                    //mt.ScheduleDate = ConvertDateTimeToDate(mt.StartDate.ToString("dd/MM/yyyy"), "en-GB");
                    ViewBag.SDate = ConvertDateTimeToDate(mt.StartDate.ToString("dd/MM/yyyy"), "en-GB");
                    mt.ActiveStatus = ul.IsActive == true ? "Active" : "In Active";
                    return View(mt);
                }
                else
                {
                    string table = "MailTemplate";
                    ViewBag.TemplateId = MS.GetCurrentIntent(table);
                    ViewBag.MailTempDDL = "Yes";
                    mt.UserRefId = ul.Id;
                    mt.UserName = ul.UserId;
                    mt.UserRole = ul.UserType;
                    mt.UserEmail = ul.EmailId;
                    mt.ActiveStatus = ul.IsActive == true ? "Active" : "In Active";
                    return View(mt);
                }
            }
        }
        [HttpPost]
        public ActionResult TemplateConfiguration(MailTemplate mt, HttpPostedFileBase file1)
        {
            string userId = base.ValidateUser();
            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
            else
            {
                 MailTemplate rc = new MailTemplate();
                 //else
                 //{

                     User ul = US.GetUserByUserId(userId);
                     mt.EmailList = mt.EmailList.Remove(mt.EmailList.Length - 1);
                     mt.DestinationList = mt.DestinationList.Remove(mt.DestinationList.Length - 1);
                     mt.SourceList = mt.SourceList.Remove(mt.SourceList.Length - 1);
                     if (!string.IsNullOrWhiteSpace(Request["StartDate"]))
                     {
                         mt.StartDate = DateTime.Parse(Request["StartDate"], provider, System.Globalization.DateTimeStyles.NoCurrentDateDefault);
                     }
                     ViewBag.MailTempDDL = "NO";
                     mt.UserRefId = ul.Id;
                     mt.UserName = ul.UserId;
                     mt.UserRole = ul.UserType;
                     mt.UserEmail = ul.EmailId;
                     mt.ScheduleDate = mt.StartDate;
                     mt.ActiveStatus = ul.IsActive == true ? "Active" : "In Active";
                     mt.IsActive = true;
                     //Adding Columns and Columnids

                     string TableName = "";
                     string RollOverId = "";
                     criteria.Add("MailTemplateMasterId", mt.MailTemplateMasterId);
                     Dictionary<long, IList<MailColumn>> ColumnMaster = ES.GetMailColumnListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                     if (ColumnMaster != null && ColumnMaster.First().Value != null && ColumnMaster.First().Value.Count > 0)
                     {
                         mt.MailColumnId = ColumnMaster.FirstOrDefault().Value[0].MailColumnId;
                         mt.MailColumns = ColumnMaster.FirstOrDefault().Value[0].Columns;
                         TableName = ColumnMaster.FirstOrDefault().Value[0].TableName;
                         RollOverId = ColumnMaster.FirstOrDefault().Value[0].RollOverId;
                     }

                     //Makeing a Select QueryString
                     mt.QueryString = "";
                     mt.QueryString = mt.QueryString + "SELECT ";
                     mt.QueryString = mt.QueryString + "ROW_NUMBER() OVER (ORDER BY " + RollOverId + ") AS Id,";
                     mt.QueryString = mt.QueryString + mt.DestinationList;
                     mt.QueryString = mt.QueryString + " FROM ";
                     mt.QueryString = mt.QueryString + TableName;

                     ViewBag.SDate = ConvertDateTimeToDate(mt.StartDate.ToString("dd/MM/yyyy"), "en-GB");
                     ES.SaveOrUpdateMailTemplate(mt, userId);
                     if (mt.MailTemplateId != 0)
                     { CreateMailActivity(mt); }
                 //}
                return View(mt);
            }
        }

        public void CreateMailActivity(MailTemplate mt)
        {
            try
            {

                bool day=false; bool month=false; bool week=false;
                for (int i = 0; i < 3; i++)
                {
                    MailActivity ma = new MailActivity();
                    ma.MailTemplateId = mt.MailTemplateId;
                    ma.StartDate = mt.StartDate;
                    ma.MailTo = mt.EmailList;
                    ma.Subject = mt.ReportName;
                    ma.MailSentDate = mt.StartDate;
                    ma.CreatedDate = DateTime.Now;
                    if (day == false)
                    {
                        ma.ScheduleNextDate = mt.StartDate.AddDays((double)1);
                        ma.MailOn = "Daily";
                        day = true;

                        if (mt.DailyMail == true)
                            ma.IsActive = true;
                        else
                            ma.IsActive = false;

                        MailActivity mailAct = ES.GetMailActivityDetailsByMailTemplateId(mt.MailTemplateId, ma.MailOn);
                        if(mailAct == null)
                        ES.SaveOrUpdateMailActivity(ma);
                    }
                    else if (week == false)
                    {
                        DateTime StartOfWeek = mt.StartDate.AddDays(-Convert.ToInt32(mt.StartDate.DayOfWeek));
                        DateTime EndOfLastWeek = StartOfWeek.AddDays(+7);
                        ma.ScheduleNextDate = EndOfLastWeek;
                        ma.MailOn = "Weekly";
                        week = true;

                        if (mt.WeeklyMail == true)
                            ma.IsActive = true;
                        else
                            ma.IsActive = false;

                        MailActivity mailAct = ES.GetMailActivityDetailsByMailTemplateId(mt.MailTemplateId, ma.MailOn);
                        if (mailAct == null)
                        ES.SaveOrUpdateMailActivity(ma);
                    }
                    else if (month == false)
                    {
                        ma.ScheduleNextDate = new DateTime(mt.StartDate.Year, mt.StartDate.Month, DateTime.DaysInMonth(mt.StartDate.Year, mt.StartDate.Month)); 
                        ma.MailOn = "Monthly";
                        month = true;

                        if (mt.MonthlyMail == true)
                            ma.IsActive = true;
                        else
                            ma.IsActive = false;

                        MailActivity mailAct = ES.GetMailActivityDetailsByMailTemplateId(mt.MailTemplateId, ma.MailOn);
                        if (mailAct == null)
                        ES.SaveOrUpdateMailActivity(ma);
                    }
                    else { ma.IsActive = false; }
                }

            }
            catch (Exception)
            {}
        }

        public JsonResult FillTemplateColumn(long TemplateId)
        {
            try
            {
                if (TemplateId != 0) { criteria.Add("MailTemplateMasterId", TemplateId); }
                Dictionary<long, IList<MailColumn>> ColumnMaster = ES.GetMailColumnListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);

                if (ColumnMaster != null && ColumnMaster.First().Value != null && ColumnMaster.First().Value.Count > 0)
                {
                    var ColumnListMaster = ColumnMaster.FirstOrDefault().Value[0].Columns.ToString().Split(',');

                    var ColumnList = (
                             from items in ColumnListMaster
                             select new
                             {
                                 Text = items,
                                 Value = items
                             }).Distinct().ToList().OrderBy(x => x.Text);
                    return Json(ColumnList, JsonRequestBehavior.AllowGet);
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

        public JsonResult FillTemplateName(long TemplateId)
        {
            try
            {

                MailTemplateMaster Mtm = ES.GetMailTemplateMasterDetailsById(TemplateId);
                if (Mtm != null)
                {
                    return Json(Mtm.Description, JsonRequestBehavior.AllowGet);
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

        public ActionResult MailList()
        {
            return View();
        }
    }
}


