using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.IO;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using System.Threading.Tasks;
using INSIGHT.Entities.TicketingSystem;
using INSIGHT.WCFServices;
using INSIGHT.Entities;
using INSIGHT.Controllers;
using NSIGHT.Service.TicketingSystem;

namespace INSIGHT.Helpers
{
    public class EmailHelper : BaseController
    {

        public void SendEmailNotification(TicketSystem objTcktSys, string userid, string activityName, bool? isrejction, string MailBody)
        {
            try
            {
                // IList<CampusEmailId> campusemaildet = GetEmailIdByCampus(objTcktSys.BranchCode, ConfigurationManager.AppSettings["CampusEmailType"].ToString());
                bool rejection = isrejction ?? true;// false;
                UserService us = new UserService();
                Dictionary<long, IList<UserAppRole>> userAppRole = new Dictionary<long, IList<UserAppRole>>();
                Dictionary<string, object> criteriaUserAppRole = new Dictionary<string, object>();

                //Based on activity need to assign the role to get the users
                //logic need to be written
                criteriaUserAppRole.Add("IsActive", true);
                //criteriaUserAppRole.Add("AppCode", "TKT");
                //if (activityName == "LogETicket" || activityName == "ResolveETicketRejection" || activityName == "CloseETicket")
                //{
                //    criteriaUserAppRole.Add("RoleCode", "ETR");
                //}
                //else if (activityName == "ResolveETicket" || activityName == "CloseETicketRejection")
                //{ criteriaUserAppRole.Add("RoleCode", "ETC"); }
                //else return;

                criteriaUserAppRole.Add("AppCode", new string[] { "TKT", "All" });
                if (activityName == "LogETicket" || activityName == "ResolveETicketRejection" || activityName == "CloseETicket")
                {
                    criteriaUserAppRole.Add("RoleCode", new string[] { "ETR", "All" });
                }
                else if (activityName == "ResolveETicket" || activityName == "CloseETicketRejection")
                { criteriaUserAppRole.Add("RoleCode", new string[] { "ETC", "All" }); }
                else return;
                userAppRole = us.GetRoleUsersListWithPagingAndCriteria(0, 1000, string.Empty, string.Empty, criteriaUserAppRole);
                //if list userAppRole is null then empty grid
                if (userAppRole != null && userAppRole.Count > 0 && userAppRole.First().Key > 0)
                {
                    int count = userAppRole.First().Value.Count;
                    //if it has values then for each concatenate APP+ROLE 
                    string[] userEmails = new string[count];

                    int i = 0;
                    Dictionary<string, object> criteria = new Dictionary<string, object>();

                    string[] userToEmail = (from u in userAppRole.FirstOrDefault().Value where u.AppCode != "All" && !string.IsNullOrEmpty(u.Email) select u.Email).ToArray();
                    string[] userBccEmail = (from u in userAppRole.FirstOrDefault().Value where u.AppCode == "All" && !string.IsNullOrEmpty(u.Email) select u.Email).ToArray();

                    //foreach (UserAppRole uar in userAppRole.First().Value)
                    //{
                    //    if (!string.IsNullOrWhiteSpace(uar.Email))
                    //    {
                    //        userEmails[i] = uar.Email.Trim();
                    //    }
                    //    i++;
                    //}
                    if (userToEmail == null || (userToEmail[0] == null && userToEmail.Length == 0)) { return; }
                    if (userToEmail != null && userToEmail.Length > 0)
                    {
                        string SendEmail = ConfigurationManager.AppSettings["SendEmailOption"];
                        string From = ConfigurationManager.AppSettings["From"];
                        if (SendEmail == "false")
                        { return; }
                        else
                        {
                            try
                            {
                                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                                mail.Subject = "e-Ticket Notification" + objTcktSys.TicketNo + " "; string msg = "";

                                foreach (string s in userToEmail)
                                {
                                    if (!string.IsNullOrWhiteSpace(s))
                                    {
                                        bool IsValid = IsValidEmail(s);
                                        if (IsValid == true)
                                            mail.To.Add(s);
                                    }

                                }
                                if (userBccEmail != null && userBccEmail.Length > 0)
                                {
                                    foreach (string s in userBccEmail)
                                    {
                                        if (!string.IsNullOrWhiteSpace(s))
                                        {
                                            bool IsValid = IsValidEmail(s);
                                            if (IsValid == true)
                                                mail.Bcc.Add(s);
                                        }

                                    }
                                }
                                switch (activityName)
                                {
                                    case "LogETicket":
                                        {
                                            msg += "An e-Ticket " + objTcktSys.TicketNo + " has been Logged for the module " + objTcktSys.Module + " with " + objTcktSys.Priority + " priority. The e-Ticket is Raised by " + userid + ". Please try resolving the ticket based on Priority. The summary of the ticket is <b><i>\"" + objTcktSys.Summary + "\"</i></b> ";
                                            break;
                                        }
                                    case "ResolveETicket":
                                        {
                                            if (rejection)
                                            {
                                                msg += "An e-Ticket " + objTcktSys.TicketNo + " has been Rejected for the module " + objTcktSys.Module + ". The e-Ticket is Rejected for additional information by " + userid + ". Please try replying the same based on the comments.<b><i>\"" + ShowComments(objTcktSys.Id, userid, "ResolveETicket", "Rejection", objTcktSys.TicketNo) + "\"</i></b>";
                                                msg = msg + CommentSummary(objTcktSys.Id, userid, "ResolveETicket", "Rejection", objTcktSys.TicketNo);
                                            }
                                            else
                                            {
                                                msg += "An e-Ticket " + objTcktSys.TicketNo + " has been Resolved for the module " + objTcktSys.Module + ". The e-Ticket is Resolved by " + userid + " with comments <b><i>\"" + ShowComments(objTcktSys.Id, userid, "ResolveETicket", "Resolution", objTcktSys.TicketNo) + "\"</i></b>. Please verify the same and complete/Reject based on the results. ";
                                                msg = msg + CommentSummary(objTcktSys.Id, userid, "ResolveETicket", "Rejection", objTcktSys.TicketNo);
                                            }
                                            break;
                                        }
                                    case "ResolveETicketRejection":
                                        {
                                            msg += "An e-Ticket " + objTcktSys.TicketNo + " has been Replied for the module " + objTcktSys.Module + ".by " + userid + " with Comments. <b><i>\"" + ShowComments(objTcktSys.Id, userid, "ResolveETicketRejection", "Resolution", objTcktSys.TicketNo) + "\"</i></b>. Please try resolving the same based on the reply comments. ";
                                            msg = msg + CommentSummary(objTcktSys.Id, userid, "ResolveETicket", "Rejection", objTcktSys.TicketNo);
                                            break;
                                        }
                                    case "CloseETicketRejection":
                                        {
                                            if (rejection)
                                            {
                                                msg += "";
                                            }
                                            else
                                            {
                                                msg += "An e-Ticket " + objTcktSys.TicketNo + " has been Rejected for the module " + objTcktSys.Module + ". The e-Ticket is Rejected by " + userid + " <b><i>\"" + ShowComments(objTcktSys.Id, userid, "CloseETicketRejection", "Resolution", objTcktSys.TicketNo) + "\"</i></b>. Please try resolving the same based on the comments. ";
                                                msg = msg + CommentSummary(objTcktSys.Id, userid, "ResolveETicket", "Rejection", objTcktSys.TicketNo);
                                            }
                                            break;
                                        }
                                    case "CloseETicket":
                                        {
                                            if (rejection)
                                            {
                                                msg += "An e-Ticket " + objTcktSys.TicketNo + " has been Rejected for the module " + objTcktSys.Module + ". The e-Ticket is Rejected for additionnal information by " + userid + " <b><i>\"" + ShowComments(objTcktSys.Id, userid, "CloseETicket", "Rejection", objTcktSys.TicketNo) + "\"</i></b>. Please try replying the same based on the comments. ";
                                                msg = msg + CommentSummary(objTcktSys.Id, userid, "ResolveETicket", "Rejection", objTcktSys.TicketNo);
                                            }
                                            //else msg += "An e-Ticket " + objTcktSys.TicketNo + " has been Completed for the module " + objTcktSys.Module + ". The e-Ticket is Completed by " + userid + " with comments <b><i>\"" + objTcktSys.Comments + "\"</i></b> ";
                                            else
                                            {
                                                msg += "An e-Ticket " + objTcktSys.TicketNo + " has been Completed for the module " + objTcktSys.Module + ". The e-Ticket is Completed by " + userid + " with comments <b><i>\"" + ShowComments(objTcktSys.Id, userid, "CloseETicket", "Resolution", objTcktSys.TicketNo) + "\"</i></b> ";
                                                msg = msg + CommentSummary(objTcktSys.Id, userid, "ResolveETicket", "Rejection", objTcktSys.TicketNo);
                                            }
                                            break;
                                        }
                                    default:
                                        return;
                                }
                                MailBody = MailBody.Replace("{{DateTime}}", DateTime.Now.ToString());
                                MailBody = MailBody.Replace("{{Content}}", msg);
                                //Body = Body.Replace("{{footer}}", footer);
                                mail.Body = MailBody;
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
                                        smtp.Send(mail);
                                    }
                                    catch (Exception ex)
                                    {
                                        #region saveorupdate Errorlog
                                        OrdersService OrdSer = new OrdersService();
                                        ErrorLog err = new ErrorLog();
                                        err.Controller = "EmailHelper";
                                        err.Action = "SendEmailNotification";
                                        err.Err_Desc = ex.ToString();
                                        err.CreatedDate = DateTime.Now;
                                        OrdSer.SaveOrUpdateErrorLog(err);
                                        #endregion

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
                                el.Id = 0;

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
                                el.EmailDateTime = DateTime.Now;
                                el.BCC_Count = mail.Bcc.Count;
                                el.Module = "ETicket";
                                //create the admission management object
                                MastersService msObj = new MastersService();
                                //log the email to the database
                                msObj.CreateOrUpdateEmailLog(el);
                            }
                            catch (Exception ex)
                            {
                                #region saveorupdate Errorlog
                                OrdersService OrdSer = new OrdersService();
                                ErrorLog err = new ErrorLog();
                                err.Controller = "EmailHelper";
                                err.Action = "SendEmailNotification";
                                err.Err_Desc = ex.ToString();
                                err.CreatedDate = DateTime.Now;
                                OrdSer.SaveOrUpdateErrorLog(err);
                                #endregion

                                throw ex;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                #region saveorupdate Errorlog
                OrdersService OrdSer = new OrdersService();
                ErrorLog err = new ErrorLog();
                err.Controller = "EmailHelper";
                err.Action = "SendEmailNotification";
                err.Err_Desc = ex.ToString();
                err.CreatedDate = DateTime.Now;
                OrdSer.SaveOrUpdateErrorLog(err);
                #endregion
                ExceptionPolicy.HandleException(ex, "TicketingSystem");
                throw ex;
            }
        }
        public string ShowComments(long ticketId, string userId, string activityName, string comments, string TktNo)
        {
            string retVal = "";
            string RejectionComments = "";
            string TicketSummary = "";
            TicketSystemService tcktSrvs = new TicketSystemService();
            Dictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Clear();
            criteria.Add("TicketId", ticketId);
            //if (!string.IsNullOrEmpty(comments)) { criteria.Add("IsRejectionOrResolution", comments); } 
            if (comments == "Resolution") { if (!string.IsNullOrEmpty(comments)) { criteria.Add("IsRejectionOrResolution", comments); } }
            if (!string.IsNullOrEmpty(userId)) { criteria.Add("CommentedBy", userId); }
            if (!string.IsNullOrEmpty(activityName)) { criteria.Add("ActivityName", activityName); }
            string sord = "Desc";
            sord = sord == "desc" ? "Desc" : "Asc";
            Dictionary<long, IList<TicketComments>> objTcktCmnts = tcktSrvs.GetTicketCommentsListWithPaging(0, 10, string.Empty, sord, criteria);
            int count = objTcktCmnts.FirstOrDefault().Value.Count();
            RejectionComments = RejectionComments + TicketSummary;
            if (objTcktCmnts != null && objTcktCmnts.FirstOrDefault().Value.Count > 0 && objTcktCmnts.FirstOrDefault().Key > 0)
            {
                retVal = objTcktCmnts.FirstOrDefault().Value[count - 1].RejectionComments;
            }
            return retVal;
        }
        public string CommentSummary(long ticketId, string userId, string activityName, string comments, string TktNo)
        {
            string retVal = "";
            string RejectionComments = "";
            string TicketSummary = "";
            TicketSystemService tcktSrvs = new TicketSystemService();
            Dictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Clear();
            criteria.Add("TicketNo", TktNo);
            Dictionary<long, IList<TicketSystem>> TktSystmList = tcktSrvs.GetTicketSystemBCListWithPagingAndCriteria(0, 10, string.Empty, string.Empty, criteria);
            if (TktSystmList != null && TktSystmList.FirstOrDefault().Value.Count > 0 && TktSystmList.FirstOrDefault().Key > 0)
            {
                TicketSummary = TktSystmList.FirstOrDefault().Value[0].Summary;
            }
            //TicketSummary = "<br/>Ticket Summary: <br/><b><i>" + TicketSummary + "</i></b>";
            criteria.Clear();
            criteria.Add("TicketId", ticketId);
            Dictionary<long, IList<TicketComments>> objTcktCmnts = tcktSrvs.GetTicketCommentsListWithPaging(0, 10, string.Empty, string.Empty, criteria);
            int count = objTcktCmnts.FirstOrDefault().Value.Count();
            if (count > 1)
            {
                TicketSummary = "<br/><br/>Ticket Summary: <br/><b><i>" + TicketSummary + "</i></b><br/><br/> Ticket History:";
            }
            else
            {
                TicketSummary = "<br/><br/>Ticket Summary: <br/><b><i>" + TicketSummary + "</i></b>";
            }
            //RejectionComments = RejectionComments + TicketSummary;
            if (objTcktCmnts != null && objTcktCmnts.FirstOrDefault().Value.Count > 0 && objTcktCmnts.FirstOrDefault().Key > 0)
            {
                //if (count == 1)
                //{
                //    for (int i = 0; i < count; i++)
                //    {
                //        RejectionComments = RejectionComments + objTcktCmnts.FirstOrDefault().Value[i].CommentedBy + " Gives " + objTcktCmnts.FirstOrDefault().Value[i].IsRejectionOrResolution + " Comment. Comment is <b><i>\"" + objTcktCmnts.FirstOrDefault().Value[i].RejectionComments + "\"</i></b><br/>";
                //    }
                //}
                if (count > 1)
                {
                    for (int i = 0; i < count - 1; i++)
                    {
                        //RejectionComments = RejectionComments + objTcktCmnts.FirstOrDefault().Value[i].CommentedBy + " Gives " + objTcktCmnts.FirstOrDefault().Value[i].IsRejectionOrResolution + " Comment. Comment is <b><i>\"" + objTcktCmnts.FirstOrDefault().Value[i].RejectionComments + "\"</i></b><br/>";

                        // RejectionComments = RejectionComments + 
                        //objTcktCmnts.FirstOrDefault().Value[i].CommentedBy + " Gives " + 
                        //objTcktCmnts.FirstOrDefault().Value[i].IsRejectionOrResolution + " Comment. Comment is <b><i>\"" + 
                        //objTcktCmnts.FirstOrDefault().Value[i].RejectionComments + "\"</i></b><br/>";
                        RejectionComments = RejectionComments +
                            objTcktCmnts.FirstOrDefault().Value[i].IsRejectionOrResolution + " Comment : <br/> &nbsp;&nbsp;&nbsp;&nbsp;"
                            + objTcktCmnts.FirstOrDefault().Value[i].RejectionComments + " - By " + objTcktCmnts.FirstOrDefault().Value[i].CommentedBy + "<br/>";
                    }
                }
            }
            retVal = TicketSummary + "<br/>" + RejectionComments;
            return retVal;
        }
        /// <summary>
        /// To Validate email address valid or not
        /// </summary>
        /// <param name="emailaddress"></param>
        /// <returns></returns>
        public bool IsValidEmail(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}