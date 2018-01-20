using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using INSIGHT.Entities;
using INSIGHT.Entities.EmailEntities;
using INSIGHT.Component;
using System.Threading.Tasks;
using System.Collections;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style;
using System.Data;
using System.Web;
using System.Configuration;
using System.Net.Mail;

namespace INSIGHT
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WindowsMailServices" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WindowsMailServices.svc or WindowsMailServices.svc.cs at the Solution Explorer and start debugging.
    public class WindowsMailServices : IWindowsMailServices
    {
        EmailBC EC = new EmailBC();
        InvoicBC IC = new InvoicBC();
        MastersBC MC = new MastersBC();
        Dictionary<string, object> criteria = new Dictionary<string, object>();
        public void StartWindowService(string Command)
        {
            EmailTemplateList();
        }

        private void EmailTemplateList()
        {
            try
            {
                
                //Retriving The MailTemplateIds which has scheduleNextdate from Today to Lastdate of the month. 
                long[] MailTemplateIds = EC.GetMailTemplatesIdUsingQuery();

                if (MailTemplateIds != null)
                {
                    criteria.Clear();
                    //Takeing all the MailTemplate for 1 month
                    criteria.Add("MailTemplateId", MailTemplateIds);
                    Dictionary<long, IList<MailConfig>> MailTemplateList = EC.GetMailConfigListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                    if (MailTemplateList != null && MailTemplateList.First().Key > 0)
                    {
                        IList<MailConfig> MailTempList = MailTemplateList.FirstOrDefault().Value.ToList();
                        foreach (var MailList in MailTempList)
                        {
                            MailColumn mc = EC.GetMailColumnDetailsById(MailList.MailColumnId);
                            //Report For Attachment
                            byte[] Attachement = GenerateReportUsingQueryString(MailList.QueryString, MailList.DestinationList, MailList.MailTemplateName);
                            string MailBody = GetBodyofReportMail();
                            //IList < MailActivity > Ma = 

                            IList<MailActivity> MailActivityList = (from items in MailList.MailActivityList
                                                                    where items.IsActive == true
                                                                    select items).OrderBy(i => i.ActivityId).Distinct().ToList();

                            foreach (var MailActiveList in MailActivityList)
                            {
                                //new Task(() => { SendEmailReport(MailList, mc, MailActiveList, MailBody, Attachement); }).Start();
                                SendEmailReport(MailList, mc, MailActiveList, MailBody, Attachement);
                            }
                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SendEmailReport(MailConfig MailList, MailColumn mc, MailActivity MailActiveList, string MailBody, byte[] Attachment)
        {
            try
            {
                string[] EmailIds = MailList.EmailList.Split(',');

                string SendEmail = ConfigurationManager.AppSettings["SendEmailOption"];
                string From = ConfigurationManager.AppSettings["From"];
                if (SendEmail == "false")
                { return; }
                else
                {
                    try
                    {
                        System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();

                        switch (MailActiveList.MailOn)
	                    {
                            case "Weekly":
                                mail.Subject = MailList.ReportName + " For Week" + "(" + DateTime.Now + ")";
                                break;
                            case "Daily":
                                mail.Subject = MailList.ReportName + " For Today" + "(" + String.Format("{0:dd/MM/yy}", DateTime.Now) + ")";
                                break;
                            case "Monthly":
                                mail.Subject = MailList.ReportName + " For Month" + "(" + DateTime.Now.ToString("MMMM yyyy") + ")";
                                break;
	                    	default:
                                break;
	                    }

                        string msg = "";

                        mail.To.Add(new MailAddress("insightsup247@gmail.com"));
                        foreach (string s in EmailIds)
                        {
                            if (!string.IsNullOrWhiteSpace(s))
                            {
                                string emailid = s.Trim();
                                mail.Bcc.Add(emailid);
                            }
                        }
                        msg += "PFA for the " + MailActiveList.MailOn + " wise " + MailList.ReportName;

                        MailBody = MailBody.Replace("{{DateTime}}", DateTime.Now.ToString());
                        MailBody = MailBody.Replace("{{Content}}", msg);
                        //MailBody = MailBody.Replace("{{UserName}}", MailTempList[0].UserName);
                        mail.Body = MailBody;

                        Attachment MailAttach = null;
                        MemoryStream memStream = new MemoryStream(Attachment);
                        MailAttach = new Attachment(memStream, MailList.ReportName + ".xlsx");
                        mail.Attachments.Add(MailAttach);
                        
                        mail.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient("localhost", 25);
                        smtp.Host = "smtp.gmail.com";
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.EnableSsl = true;
                        if (From == "live")
                        {
                            try
                            {
                                mail.From = new MailAddress("insightsup247@gmail.com");
                                smtp.Credentials = new System.Net.NetworkCredential
                               ("insightsup247@gmail.com", "Spring@2k14");
                                //smtp.Send(mail);
                            }

                            catch (Exception ex)
                            {
                                if (ex.Message.Contains("quota"))
                                {
                                    mail.From = new MailAddress("j.desouza@xcdsys.com");
                                    smtp.Credentials = new System.Net.NetworkCredential
                                    ("j.desouza@xcdsys.com", "Ginger@27");
                                    smtp.Send(mail);
                                }
                                else
                                {
                                    mail.From = new MailAddress("s.kingston@xcdsys.com");
                                    smtp.Credentials = new System.Net.NetworkCredential
                                    ("s.kingston@xcdsys.com", "JohnSGrasiasXcd");
                                    smtp.Send(mail);
                                }
                            }
                        }
                        else if (From == "test")
                        {
                            mail.From = new MailAddress("insightsup247@gmail.com");
                            smtp.Credentials = new System.Net.NetworkCredential
                            ("insightsup247@gmail.com", "Spring@2k14");
                            //this is to send email to test mail only to avoid mis communication to the parent
                            //  mail.To.Add("j.desouza@xcdsys.com");
                            smtp.Send(mail);
                        }
                        EmailLog el = new EmailLog();
                        el.EmailFrom = mail.From.ToString();
                        el.EmailTo = mail.To.ToString();
                        el.EmailCC = mail.CC.ToString();
                        if (mail.Bcc.ToString().Length < 3990)
                        {
                            el.EmailBCC = mail.Bcc.ToString();
                        }

                        el.Subject = mail.Subject.ToString();

                        if (mail.Body.ToString().Length < 3990)
                        {
                            el.Message = msg;
                        }
                        DateTime dt = DateTime.Now;
                        el.EmailDateTime = dt;
                        el.BCC_Count = mail.Bcc.Count;
                        el.Module = "ReportingMail";
                        el.IsSent = true;
                        el.Message = msg;
                        //log the email to the database
                        MC.CreateOrUpdateEmailLog(el);
                        UpdateMailActivity(MailActiveList);

                    }
                    catch (Exception)
                    {}
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void UpdateMailActivity(MailActivity MailActiveList)
        {
            try
            {
                if (MailActiveList != null)
                {
                    switch (MailActiveList.MailOn)
                    {
                        case "Weekly":
                            MailActiveList.MailSentDate = MailActiveList.ScheduleNextDate;
                            MailActiveList.ScheduleNextDate = MailActiveList.MailSentDate.AddDays(+7);
                            break;
                        case "Daily":
                            MailActiveList.MailSentDate = MailActiveList.ScheduleNextDate;
                            MailActiveList.ScheduleNextDate = MailActiveList.MailSentDate.AddDays(+1);
                            break;
                        case "Monthly":
                             MailActiveList.MailSentDate = MailActiveList.ScheduleNextDate;
                             DateTime dt = DateTime.Now;
                             dt = dt.AddMonths(1);
                             MailActiveList.ScheduleNextDate = new DateTime(dt.Year, dt.Month , DateTime.DaysInMonth(dt.Year, dt.Month)); 
                            break;
                        default:
                            break;
                    }
                    EC.SaveOrUpdateMailActivity(MailActiveList);
                }
            }
            catch (Exception)
            {}
        }

        public byte[] GenerateReportUsingQueryString(string QueryString, string DestinationList,string SheetName)
        {
            try
            {
                string HeaderList = "S.No," + DestinationList;

                List<string> lstHeader = new List<string>();
                var TempList = HeaderList.ToString().Split(',');
                //TempList to List of Strings
                foreach (var item in TempList)
                    lstHeader.Add(item);

                IList List = EC.GetListWithQuery(QueryString);
                DataTable Dt = EC.GetListWithDataTable(QueryString);

                #region ExcelGeneration for this List

                ExcelPackage pck = new ExcelPackage();
                pck.Workbook.Properties.Author = "John Desouza A";
                pck.Workbook.Properties.Title = "Automatic Generated Excel";
                pck.Workbook.Properties.Company = "GCC Services";

                var ws = pck.Workbook.Worksheets.Add(" " + SheetName + " Records");
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
                //ws.Cells["A2"].LoadFromCollection(List.Select(selector), false);

                List<object> Test = (List<object>)(List);

                ws.Cells["A2"].LoadFromDataTable(Dt, false);

                int Rowcount = Test.Count() + 2; //List.Count() + 2;
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

                byte[] data = pck.GetAsByteArray();
                #endregion
                
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
        public string GetBodyofReportMail()
        {
            string MessageBody = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Views/Shared/ReportEmailBody.html"));

            return MessageBody;
        }

        public long[] GetMailTemplatesIdUsingQuery() { 

            long[] MailTemplateIds = EC.GetMailTemplatesIdUsingQuery();
            return MailTemplateIds;
        }


    }
}
