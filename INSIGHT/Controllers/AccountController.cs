using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using CustomAuthentication;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using INSIGHT.Entities;
using INSIGHT.WCFServices;
namespace INSIGHT.Controllers
{
    public class AccountController : BaseController
    {
        MastersService mssvc = new MastersService();
        Dictionary<string, object> criteria = new Dictionary<string, object>();
        public ActionResult CustomRegister()
        {
            try
            {
                User u = new User();
                u.UserType = "";
                criteria.Clear();
                return View(u);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "CustomAccountPolicy");
                throw ex;
            }

        }
        [HttpPost]
        public ActionResult CustomRegister(User user)
        {
            try
            {
                //check newpassword and confirm password matches or not
                if (user.Password == user.ConfirmPassword)
                {
                    user.CreatedDate = DateTime.Now;
                    user.ModifiedDate = DateTime.Now;
                    UserService us = new UserService();
                    PassworAuth PA = new PassworAuth();
                    //encode and save the password
                    user.Password = PA.base64Encode(user.Password);
                    user.IsActive = true;
                    us.CreateOrUpdateUser(user);
                    TempData["SuccessUserCreation"] = 1;
                    return View();
                }
                else throw new Exception("Password and ConfirmPassword doesn't match.");
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "CustomAccountPolicy");
                throw ex;
            }
        }
        public ActionResult LogOn()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "CustomAccountPolicy");
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult LogOn(User user)
        {
            Session["Role"] = "";
            Session["AdminRole"] = "";
            Session["userrole"] = "";
            Session["staffapproverrole"] = "";
            try
            {
                if (!string.IsNullOrWhiteSpace(user.UserId) && !string.IsNullOrWhiteSpace(user.Password))
                {
                    UserService us = new UserService();
                    User User = us.GetUserByUserId(user.UserId);
                    PassworAuth PA = new PassworAuth();
                    if (User != null)
                    {
                        if (!string.Equals(user.UserId, User.UserId))
                        {
                            ViewBag.User = "Case Sensitive";
                            return View();
                        }
                        if (User.IsActive == false)
                        {
                            ViewBag.User = "Deactivated";
                            return View();
                        }
                        else
                        {
                            if (Request["Password"] == PA.base64Decode2(User.Password))
                            {
                                Session s = GetLoginDetails(User);
                                us.CreateOrUpdateSession(s);
                                Session["objUser"] = User;
                                Session["UserId"] = User.UserId;
                                Dictionary<string, object> criteriamain = new Dictionary<string, object>();
                                criteriamain.Add("UserId", user.UserId);
                                Dictionary<long, IList<UserAppRole>> userappmain = us.GetAppRoleForAnUserListWithPagingAndCriteria(0, 10000, string.Empty, string.Empty, criteriamain);
                                if (userappmain.FirstOrDefault().Value != null && userappmain.FirstOrDefault().Value.Count != 0)
                                {
                                    if (userappmain.First().Value[0].AppCode != null)
                                    {
                                        var appCode = (from u in userappmain.First().Value
                                                       select u.AppCode).Distinct().ToArray();
                                        Session["Role"] = appCode;
                                        var adminRole = (from v in appCode where v == "All" select v);
                                        if (!string.IsNullOrEmpty(adminRole.FirstOrDefault()))
                                        {
                                            Session["AdminRole"] = adminRole.First().ToString();
                                        }
                                        //changed by micheal usercampus as Sectorcode
                                        var Sectorcode = (from u in userappmain.First().Value
                                                          where u.SectorCode != null
                                                          select u.SectorCode).Distinct().ToArray();
                                        Session["SectorCode"] = Sectorcode;
                                    }
                                }
                                var userrole = (from r in userappmain.First().Value
                                                select r.RoleCode).Distinct().ToArray();
                                Session["userrolesarray"] = userrole;
                                if (userrole.Contains("ADM-APP"))            //to check if user has student approver access
                                {
                                    Session["userrole"] = "ADM-APP";
                                }
                                if (userrole.Contains("STM-APP"))          //to check if user has staff approver access
                                {
                                    Session["staffapproverrole"] = "STM-APP";
                                }
                                Session["SiteLinks"] = IdHtmlTags();
                                //return RedirectToAction("TemplateConfiguration", "Email");
                                return RedirectToAction("Home", "Home");
                            }
                            else return View();
                        }
                    }
                    else
                    {
                        ViewBag.User = -1;
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "CustomAccountPolicy");
                throw ex;
            }
            return View();
        }

        #region HtmlTags
        //public string IdHtmlTags()
        //{
        //    try
        //    {
        //        if (Session["Role"].ToString() != "")
        //        {
        //            System.Text.StringBuilder html = new System.Text.StringBuilder();
        //            MenuService ms = new MenuService();
        //            Dictionary<string, object> criteria = new Dictionary<string, object>();
        //            var rle = Session["Role"] as IEnumerable<string>;
        //            if (rle.Contains("All"))
        //                return AdminRoleMenuBuilding(ms, html, criteria);
        //            else
        //                return OtherRolesMenuBuilding(ms, html, criteria, rle);
        //        }
        //        else { return string.Empty; }
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionPolicy.HandleException(ex, "CustomAccountPolicy");
        //        throw ex;
        //    }
        //}
        //private string AdminRoleMenuBuilding(MenuService ms, System.Text.StringBuilder html, Dictionary<string, object> criteria)
        //{
        //    try
        //    {
        //        Dictionary<long, IList<Menu>> submenuitems;
        //        Dictionary<long, IList<Menu>> thirdlevelmenuitems;
        //        html.Clear();
        //        criteria.Add("ParentORChild", true);
        //        Dictionary<long, IList<Menu>> mainmenu = ms.GetMenuListWithPagingAndCriteria(0, 1000, string.Empty, string.Empty, criteria);
        //        html.Append("<ul>");
        //        foreach (var items in mainmenu.First().Value)
        //        {
        //            if (items.MenuName == "Home")
        //            {
        //                html.Append("<li><a href='/Home/Home'>Home</a></li>");
        //            }
        //            else
        //            {
        //                html.Append("<li><a>" + items.MenuName + "</a><ul>");
        //                criteria.Clear();
        //                criteria.Add("ParentRefId", Convert.ToInt32(items.Id));
        //                criteria.Add("MenuLevel", "Level2");
        //                submenuitems = ms.GetMenuListWithPagingAndCriteria(0, 100, string.Empty, string.Empty, criteria);
        //                foreach (var var in submenuitems.First().Value)
        //                {
        //                    html.Append("<li><a href='/" + var.Controller + "/" + var.Action + "'>" + var.MenuName + "</a>");

        //                    criteria.Clear();
        //                    criteria.Add("ParentRefId", Convert.ToInt32(var.Id));
        //                    criteria.Add("MenuLevel", "Level3");
        //                    thirdlevelmenuitems = ms.GetMenuListWithPagingAndCriteria(0, 100, string.Empty, string.Empty, criteria);
        //                    if (thirdlevelmenuitems.First().Value.Count > 0)             // if there is a third level menu
        //                    {
        //                        int j = 0;
        //                        foreach (var tid in thirdlevelmenuitems.First().Value)
        //                        {
        //                            if (j == 0)   // To add ul tag only for first time
        //                            {
        //                                html.Append("<ul>");
        //                            }
        //                            html.Append("<li><a href='/" + tid.Controller + "/" + tid.Action + "'>" + tid.MenuName + "</a> </li>");
        //                            j++;
        //                        }
        //                        if (j != 0)   //  if ul open tag is added then to add the close tag
        //                        {
        //                            html.Append("</ul>");
        //                        }
        //                    }
        //                    html.Append("</li>");
        //                }
        //                html.Append("</ul></li>");
        //                criteria.Clear();
        //            }
        //        }
        //        html.Append("</ul>");
        //        return html.ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionPolicy.HandleException(ex, "CustomAccountPolicy");
        //        throw ex;
        //    }
        //}
        //private string OtherRolesMenuBuilding(MenuService ms, System.Text.StringBuilder html, Dictionary<string, object> criteria, IEnumerable<string> rle)
        //{
        //    try
        //    {
        //        long[] parentrefid = new long[rle.Count()];
        //        int i = 0;
        //        criteria.Clear();
        //        criteria.Add("Role", rle.ToArray());
        //        criteria.Add("ParentORChild", false);
        //        Dictionary<long, IList<Menu>> mainmenuparentid = ms.GetMenuListWithPagingAndCriteria(0, 100, string.Empty, string.Empty, criteria);
        //        if (mainmenuparentid != null && mainmenuparentid.FirstOrDefault().Value != null && mainmenuparentid.FirstOrDefault().Value.Count > 0)
        //        {
        //            foreach (Menu m in mainmenuparentid.First().Value)
        //            {
        //                if (!parentrefid.Contains(m.ParentRefId))
        //                {
        //                    parentrefid[i] = m.ParentRefId;
        //                    i++;
        //                }
        //            }
        //        }
        //        html.Append("<ul>");
        //        html.Append("<li><a href='/Home/Home'>Home</a></li>");
        //        //get all the menu items inside the for each loop need to be moved here
        //        //some other time this need to be concentrated to get all the menu table list, after that the same need to be used for the menu building
        //        //this is to avoid repeated db read on the same table
        //        IList<Menu> menuList = ms.GetMenusById(parentrefid.Distinct().ToArray());
        //        if (menuList != null)
        //        {
        //            foreach (long id in parentrefid.Distinct().ToArray())  // to remove repeated parent id's for two or more submenu items
        //            {
        //                if (id != 0)
        //                {
        //                    criteria.Clear();
        //                    criteria.Add("Id", id);
        //                    Menu mainmenubyid = menuList.First(s => s.Id == id);
        //                    if (mainmenubyid.MenuLevel == "Level1")//to take level1 menu--- comments updated by jp,anbu
        //                        html.Append("<li><a>" + mainmenubyid.MenuName + "</a><ul>");

        //                    criteria.Clear();
        //                    criteria.Add("ParentRefId", Convert.ToInt32(id));
        //                    criteria.Add("MenuLevel", "Level2");
        //                    Dictionary<long, IList<Menu>> submenubyparentid = ms.GetMenuListWithPagingAndCriteria(0, 100, string.Empty, string.Empty, criteria);
        //                    foreach (var pid in submenubyparentid.First().Value)
        //                    {
        //                        if (rle.Contains(pid.Role))
        //                        {
        //                            html.Append("<li><a href='/" + pid.Controller + "/" + pid.Action + "'>" + pid.MenuName + "</a>");
        //                            criteria.Clear();
        //                            criteria.Add("ParentRefId", Convert.ToInt32(pid.Id));
        //                            criteria.Add("MenuLevel", "Level3");
        //                            Dictionary<long, IList<Menu>> thirdlevelmenu = ms.GetMenuListWithPagingAndCriteria(0, 100, string.Empty, string.Empty, criteria);
        //                            if (thirdlevelmenu.First().Value.Count > 0)             // if there is a third level menu
        //                            {
        //                                int j = 0;
        //                                foreach (var tid in thirdlevelmenu.First().Value)
        //                                {
        //                                    if (rle.Contains(tid.Role))
        //                                    {
        //                                        if (j == 0)   // To add ul tag only for first time
        //                                        {
        //                                            html.Append("<ul>");
        //                                        }
        //                                        html.Append("<li><a href='/" + tid.Controller + "/" + tid.Action + "'>" + tid.MenuName + "</a> </li>");
        //                                        j++;
        //                                    }
        //                                }
        //                                if (j != 0)   //  if ul open tag is added then to add the close tag
        //                                {
        //                                    html.Append("</ul>");
        //                                }
        //                            }
        //                            html.Append("</li>");
        //                        }
        //                    }
        //                    html.Append("</ul></li>");
        //                }
        //            }
        //            html.Append("</ul>");
        //            return html.ToString();
        //        }
        //        else return string.Empty;
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionPolicy.HandleException(ex, "CustomAccountPolicy");
        //        throw ex;
        //    }
        //}
        #endregion
        public string IdHtmlTags()
        {
            try
            {
                if (Session["Role"].ToString() != "")
                {
                    System.Text.StringBuilder html = new System.Text.StringBuilder();
                    MenuService ms = new MenuService();
                    Dictionary<string, object> criteria = new Dictionary<string, object>();
                    var rle = Session["Role"] as IEnumerable<string>;
                    if (rle.Contains("All"))
                        return AdminRoleMenuBuilding(ms, html, criteria);
                    else
                        return OtherRolesMenuBuilding(ms, html, criteria, rle);
                }
                else { return string.Empty; }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "AdmissionPolicy");
                throw ex;
            }
        }
        private string AdminRoleMenuBuilding(MenuService ms, System.Text.StringBuilder html, Dictionary<string, object> criteria)
        {
            Dictionary<long, IList<Menu>> submenuitems;
            Dictionary<long, IList<Menu>> thirdlevelmenuitems;

            bool IsSubsite = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSubsite"]);
            string SubsiteName = ConfigurationManager.AppSettings["SubsiteName"].ToString();

            string sidx = "OrderNo";
            string sord = "";
            sord = sord == "desc" ? "Desc" : "Asc";

            html.Clear();
            criteria.Add("ParentORChild", true);
            criteria.Add("IsActive", true);
            Dictionary<long, IList<Menu>> mainmenu = ms.GetMenuListWithPagingAndCriteria(0, 1000, sord, sidx, criteria);
            html.Append("<ul>");
            foreach (var items in mainmenu.First().Value)
            {
                if (items.MenuName == "Home")
                {
                    if (IsSubsite == true)
                        html.Append("<li><a href='/" + SubsiteName + "/Home/Home'>Home</a></li>");
                    else
                        html.Append("<li><a href='/Home/Home'>Home</a></li>");
                }
                else
                {
                    html.Append("<li><a>" + items.MenuName + "</a><ul>");
                    criteria.Clear();
                    criteria.Add("ParentRefId", Convert.ToInt32(items.Id));
                    criteria.Add("MenuLevel", "Level2");
                    criteria.Add("IsActive", true);
                    submenuitems = ms.GetMenuListWithPagingAndCriteria(0, 100, sord, sidx, criteria);
                    foreach (var var in submenuitems.First().Value)
                    {
                        if (IsSubsite == true)
                            html.Append("<li><a href='/" + SubsiteName + "/" + var.Controller + "/" + var.Action + "'>" + var.MenuName + "</a>");
                        else
                            html.Append("<li><a href='/" + var.Controller + "/" + var.Action + "'>" + var.MenuName + "</a>");
                        criteria.Clear();
                        criteria.Add("ParentRefId", Convert.ToInt32(var.Id));
                        criteria.Add("MenuLevel", "Level3");
                        criteria.Add("IsActive", true);
                        thirdlevelmenuitems = ms.GetMenuListWithPagingAndCriteria(0, 100, sord, sidx, criteria);
                        if (thirdlevelmenuitems.First().Value.Count > 0)             // if there is a third level menu
                        {
                            int j = 0;
                            foreach (var tid in thirdlevelmenuitems.First().Value)
                            {
                                if (j == 0)   // To add ul tag only for first time
                                {
                                    html.Append("<ul>");
                                }
                                if (IsSubsite == true)
                                    html.Append("<li><a href='/" + SubsiteName + "/" + tid.Controller + "/" + tid.Action + "'>" + tid.MenuName + "</a> </li>");
                                else
                                    html.Append("<li><a href='/" + tid.Controller + "/" + tid.Action + "'>" + tid.MenuName + "</a> </li>");
                                j++;
                            }
                            if (j != 0)   //  if ul open tag is added then to add the close tag
                            {
                                html.Append("</ul>");
                            }
                        }
                        html.Append("</li>");
                    }
                    html.Append("</ul></li>");
                    criteria.Clear();
                }
            }
            html.Append("</ul>");
            return html.ToString();
        }
        private string OtherRolesMenuBuilding(MenuService ms, System.Text.StringBuilder html, Dictionary<string, object> criteria, IEnumerable<string> rle)
        {
            bool IsSubsite = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSubsite"]);
            string SubsiteName = ConfigurationManager.AppSettings["SubsiteName"].ToString();
            string sidx = "OrderNo";
            string sord = "";
            sord = sord == "desc" ? "Desc" : "Asc";
            long[] parentrefid = new long[rle.Count()];
            int i = 0;
            criteria.Clear();
            criteria.Add("Role", rle.ToArray());
            criteria.Add("ParentORChild", false);
            criteria.Add("IsActive", true);
            Dictionary<long, IList<Menu>> mainmenuparentid = ms.GetMenuListWithPagingAndCriteria(0, 100, sord, sidx, criteria);
            if (mainmenuparentid != null && mainmenuparentid.FirstOrDefault().Value != null && mainmenuparentid.FirstOrDefault().Value.Count > 0)
            {
                foreach (Menu m in mainmenuparentid.First().Value)
                {
                    if (!parentrefid.Contains(m.ParentRefId))
                    {
                        parentrefid[i] = m.ParentRefId;
                        i++;
                    }
                }
            }
            html.Append("<ul>");
            if (IsSubsite == true)
                html.Append("<li><a href='/" + SubsiteName + "/Home/Home'>Home</a></li>");
            else
                html.Append("<li><a href='/Home/Home'>Home</a></li>");
            //get all the menu items inside the for each loop need to be moved here
            //some other time this need to be concentrated to get all the menu table list, after that the same need to be used for the menu building
            //this is to avoid repeated db read on the same table
            IList<Menu> menuList = ms.GetMenusById(parentrefid.Distinct().ToArray());
            if (menuList != null)
            {
                foreach (long id in parentrefid.Distinct().ToArray())  // to remove repeated parent id's for two or more submenu items
                {
                    if (id != 0)
                    {
                        criteria.Clear();
                        criteria.Add("Id", id);
                        Menu mainmenubyid = menuList.First(s => s.Id == id);
                        if (mainmenubyid.MenuLevel == "Level1")//to take level1 menu--- comments updated by jp,anbu
                            html.Append("<li><a>" + mainmenubyid.MenuName + "</a><ul>");

                        criteria.Clear();
                        criteria.Add("ParentRefId", Convert.ToInt32(id));
                        criteria.Add("MenuLevel", "Level2");
                        criteria.Add("IsActive", true);
                        Dictionary<long, IList<Menu>> submenubyparentid = ms.GetMenuListWithPagingAndCriteria(0, 100, sord, sidx, criteria);
                        if (submenubyparentid.FirstOrDefault().Key > 0)
                        {
                            foreach (var pid in submenubyparentid.First().Value)
                            {
                                if (rle.Contains(pid.Role))
                                {
                                    if (IsSubsite == true)
                                        html.Append("<li><a href='/" + SubsiteName + "/" + pid.Controller + "/" + pid.Action + "'>" + pid.MenuName + "</a>");
                                    else
                                        html.Append("<li><a href='/" + pid.Controller + "/" + pid.Action + "'>" + pid.MenuName + "</a>");

                                    criteria.Clear();
                                    criteria.Add("ParentRefId", Convert.ToInt32(pid.Id));
                                    criteria.Add("MenuLevel", "Level3");
                                    criteria.Add("IsActive", true);
                                    Dictionary<long, IList<Menu>> thirdlevelmenu = ms.GetMenuListWithPagingAndCriteria(0, 100, sord, sidx, criteria);
                                    if (thirdlevelmenu.First().Value.Count > 0)             // if there is a third level menu
                                    {
                                        int j = 0;
                                        foreach (var tid in thirdlevelmenu.First().Value)
                                        {
                                            if (rle.Contains(tid.Role))
                                            {
                                                if (j == 0)   // To add ul tag only for first time
                                                {
                                                    html.Append("<ul>");
                                                }
                                                if (IsSubsite == true)
                                                    html.Append("<li><a href='/" + SubsiteName + "/" + tid.Controller + "/" + tid.Action + "'>" + tid.MenuName + "</a> </li>");
                                                else
                                                    html.Append("<li><a href='/" + tid.Controller + "/" + tid.Action + "'>" + tid.MenuName + "</a> </li>");
                                                j++;
                                            }
                                        }
                                        if (j != 0)   //  if ul open tag is added then to add the close tag
                                        {
                                            html.Append("</ul>");
                                        }
                                    }
                                    html.Append("</li>");
                                }
                            }
                            html.Append("</ul></li>");
                        }

                    }
                }
                html.Append("</ul>");
                return html.ToString();
            }
            else return string.Empty;
        }

        public ActionResult CustomChangePassword()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "CustomAccountPolicy");
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult CustomChangePassword(User user)
        {
            string oldPassword = Request["Password"];
            try
            {
                //old password should be verified with the stored password in table
                UserService us = new UserService();
                User User = us.GetUserByUserId(user.UserId);
                if (User != null)
                {
                    PassworAuth PA = new PassworAuth();
                    if (oldPassword != PA.base64Decode2(User.Password))
                    {
                        throw new Exception("Old password doesn't match with the stored password for user " + user.UserId + "");
                    }
                    //new and confirm should match
                    else if (PA.base64Encode(user.NewPassword) != PA.base64Encode(user.ConfirmPassword))
                    {
                        throw new Exception("new password doesn't match with the confirm password for user " + user.UserId + "");
                    }
                    else
                    {
                        User.Password = PA.base64Encode(user.NewPassword);
                        us.CreateOrUpdateUser(User);
                        //change the password for the user
                        TempData["SuccessPassChange"] = 1;
                        return RedirectToAction("LogOn", "Account");
                    }
                }
                else
                {
                    ViewBag.User = -1;
                    return View();
                }

            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "CustomAccountPolicy");
                throw ex;
            }
        }
        public ActionResult CustomForgotPassword()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "CustomAccountPolicy");
                throw ex;
            }
        }
        //Comments JP and Anbu
        //Send email need to be with new task so that ui no need to wait for response --done
        [HttpPost]
        public ActionResult CustomForgotPassword(User user)
        {
            try
            {
                UserService us = new UserService();
                User User = us.GetUserByEmailId(user.EmailId);
                if (User != null)
                {
                    if (!User.IsActive)
                    {
                        ViewBag.User = "Deactivated";
                        return View();
                    }
                    else
                    {
                        PassworAuth PA = new PassworAuth();
                        string password = PA.base64Decode2(User.Password);
                        string SendEmail = ConfigurationManager.AppSettings["SendEmailOption"];
                        if (SendEmail == "false")
                            return null;
                        else
                        {
                            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                            mail.To.Add(user.EmailId);
                            mail.Subject = "Password";

                            string Body = "Dear Sir/Madam, <br/><br/>" +
                                        " Your password is " + password + ". <br/><br/>" +
                                        " Try to login with the given password. <br/><br/>" +
                                        " Regards, <br/>" +
                                        " Insight Support";
                            mail.Body = Body;
                            mail.IsBodyHtml = true;
                            SmtpClient smtp = new SmtpClient("localhost", 25);
                            smtp.Host = "smtp.gmail.com";
                            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                            smtp.EnableSsl = true;
                            try
                            {
                                mail.From = new MailAddress("insightsup247@gmail.com");
                                smtp.Credentials = new System.Net.NetworkCredential
                               ("insightsup247@gmail.com", "Spring@2k14");

                                if (ValidEmailOrNot(mail.From.ToString()) && ValidEmailOrNot(mail.To.ToString()))
                                {
                                    new Task(() => { SendEmailWithForNewTask(mail, smtp); }).Start();
                                }
                                ViewBag.PasswordSentMessage = 1;
                            }
                            catch (Exception ex)
                            {
                                if (ex.Message.Contains("quota"))
                                {
                                    mail.From = new MailAddress("insightsup247@gmail.com");
                                    smtp.Credentials = new System.Net.NetworkCredential
                                    ("insightsup247@gmail.com", "Spring@2k14");
                                    if (ValidEmailOrNot(mail.From.ToString()) && ValidEmailOrNot(mail.To.ToString()))
                                    {
                                        new Task(() => { SendEmailWithForNewTask(mail, smtp); }).Start();
                                    }
                                }
                            }
                            return View();
                        }
                    }
                }
                else
                {
                    ViewBag.Message = -1;
                    return View();
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "CustomAccountPolicy");
                throw ex;
            }
        }

        public ActionResult LogOff()
        {
            string userId = (Session["UserId"] == null) ? "" : Session["UserId"].ToString();
            if (!string.IsNullOrWhiteSpace(userId))
            {
                try
                {
                    new Task(() => { UpdateUserLogoff(userId); }).Start();
                    Session.Remove("UserId");
                    Session.Remove("User");
                    Session.Remove("SiteLinks");
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "CustomAccountPolicy");
                    throw ex;
                }
            }
            return RedirectToAction("LogOn", "Account");
        }

        public ActionResult UserList()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "CustomAccountPolicy");
                throw ex;
            }
        }

        public ActionResult UserListJqGrid(string UserId, string UserName, string UserType, string EmailId, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                UserService us = new UserService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                if (!string.IsNullOrEmpty(UserId))
                {
                    criteria.Add("UserId", UserId);
                }
                if (!string.IsNullOrEmpty(UserName))
                {
                    criteria.Add("UserName", UserName);
                }
                if (!string.IsNullOrEmpty(UserType))
                {
                    criteria.Add("UserType", UserType);
                }
                if (!string.IsNullOrEmpty(EmailId))
                {
                    criteria.Add("EmailId", EmailId);
                }
                sord = sord == "desc" ? "Desc" : "Asc";
                Dictionary<long, IList<User>> UserList = us.GetUserListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                if (UserList != null && UserList.Count > 0)
                {
                    long totalrecords = UserList.First().Key;
                    int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                    var AssLst = new
                    {
                        total = totalPages,
                        page = page,
                        records = totalrecords,
                        rows = (
                             from items in UserList.First().Value

                             select new
                             {
                                 cell = new string[] 
                                         {
                                            items.Id.ToString(),
                                            items.UserId,
                                            items.UserName,
                                            items.UserType,
                                            items.EmailId,
                                            items.Password,
                                            items.IsActive.ToString(),
                                         }
                             }).ToList()
                    };
                    return Json(AssLst, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "CustomAccountPolicy");
                throw ex;
            }
        }
        public ActionResult UserModification()
        {
            try
            {
                return View(new User() { UserType = "" });
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "CustomAccountPolicy");
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult UserModification(User user)
        {
            try
            {
                UserService us = new UserService();
                if (user != null)
                {
                    us.ModifyUser(user);
                    ViewBag.SuccessMsg = "User details changed successfully";
                    return View();
                } return null;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "CustomAccountPolicy");
                throw ex;
            }
        }

        public ActionResult LoginHistory()
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
                ExceptionPolicy.HandleException(ex, "CustomAccountPolicy");
                throw ex;
            }
        }
        public ActionResult FillUserType()
        {
            try
            {
                criteria.Clear();
                Dictionary<long, IList<UserTypeMaster>> UserTypeList = mssvc.GetUserTypeMasterListWithPagingAndCriteria(0, 1000, string.Empty, string.Empty, criteria);
                if (UserTypeList != null && UserTypeList.First().Value != null && UserTypeList.First().Value.Count > 0)
                {
                    var UserType = (
                             from items in UserTypeList.First().Value

                             select new
                             {
                                 Text = items.UserType,
                                 Value = items.UserType
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
        public ActionResult LoginListJQGrid(string UserId, string usertyp, string FrmDate, string srchtyp, string ExportType, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                if (!string.IsNullOrEmpty(UserId))
                    criteria.Add("UserId", UserId);
                if (!string.IsNullOrEmpty(usertyp))
                    criteria.Add("UserType", usertyp);
                if ((!string.IsNullOrEmpty(FrmDate) && !(string.IsNullOrEmpty(FrmDate.Trim()))))
                {
                    IFormatProvider provider = new System.Globalization.CultureInfo("en-CA", true);
                    FrmDate = FrmDate.Trim();
                    DateTime FromDate = DateTime.Parse(FrmDate, provider, System.Globalization.DateTimeStyles.NoCurrentDateDefault);
                    string To = string.Format("{0:MM/dd/yyyy}", DateTime.Now);
                    DateTime ToDate = Convert.ToDateTime(To + " " + "23:59:59");
                    DateTime[] fromto = new DateTime[2];
                    fromto[0] = Convert.ToDateTime(FromDate);
                    fromto[1] = ToDate;
                    criteria.Add("TimeIn", fromto);
                }
                if (!string.IsNullOrEmpty(srchtyp))
                {
                    DateTime fdate = DateTime.Now;
                    DateTime tdate = DateTime.Now;
                    DateTime[] fromto = new DateTime[2];
                    switch (srchtyp)
                    {
                        case "Today":
                            {
                                string from = string.Format("{0:MM/dd/yyyy}", DateTime.Now);
                                fdate = Convert.ToDateTime(from + " " + "12:00:00 AM");
                                tdate = Convert.ToDateTime(from + " " + "23:59:59");
                                fromto[0] = fdate;
                                fromto[1] = tdate;
                                criteria.Add("TimeIn", fromto);
                                break;
                            }
                        case "Yesterday":
                            {
                                string from = string.Format("{0:MM/dd/yyyy}", DateTime.Now.AddDays(-1));
                                fdate = Convert.ToDateTime(from + " " + "12:00:00 AM");
                                tdate = Convert.ToDateTime(from + " " + "23:59:59");
                                fromto[0] = fdate;
                                fromto[1] = tdate;
                                criteria.Add("TimeIn", fromto);
                                break;
                            }
                        case "This Week":
                            {
                                CultureInfo info = Thread.CurrentThread.CurrentCulture;
                                DayOfWeek firstday = info.DateTimeFormat.FirstDayOfWeek;
                                DayOfWeek today = info.Calendar.GetDayOfWeek(DateTime.Now);

                                int diff = today - firstday;
                                DateTime firstDate = DateTime.Now.AddDays(-diff);
                                DateTime LastDate = firstDate.AddDays(7);

                                string from = string.Format("{0:MM/dd/yyyy}", firstDate);
                                string to = string.Format("{0:MM/dd/yyyy}", LastDate);
                                fdate = Convert.ToDateTime(from + " " + "12:00:00 AM");
                                tdate = Convert.ToDateTime(to + " " + "23:59:59");
                                fromto[0] = fdate;
                                fromto[1] = tdate;
                                criteria.Add("TimeIn", fromto);
                                break;
                            }
                        case "Last Week":
                            {
                                int days = DateTime.Now.DayOfWeek - DayOfWeek.Sunday;
                                DateTime firstDate = DateTime.Now.AddDays(-(days + 7));
                                DateTime LastDate = firstDate.AddDays(6);
                                string from = string.Format("{0:MM/dd/yyyy}", firstDate);
                                string to = string.Format("{0:MM/dd/yyyy}", LastDate);
                                fdate = Convert.ToDateTime(from + " " + "12:00:00 AM");
                                tdate = Convert.ToDateTime(to + " " + "23:59:59");
                                fromto[0] = fdate;
                                fromto[1] = tdate;
                                criteria.Add("TimeIn", fromto);
                                break;
                            }
                        case "This Month":
                            {
                                DateTime firstDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                                DateTime LastDate = firstDate.AddMonths(1).AddDays(-1);
                                string from = string.Format("{0:MM/dd/yyyy}", firstDate);
                                string to = string.Format("{0:MM/dd/yyyy}", LastDate);
                                fdate = Convert.ToDateTime(from + " " + "12:00:00 AM");
                                tdate = Convert.ToDateTime(to + " " + "23:59:59");
                                fromto[0] = fdate;
                                fromto[1] = tdate;
                                criteria.Add("TimeIn", fromto);
                                break;
                            }
                        case "Last Month":
                            {
                                DateTime firstDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);
                                DateTime LastDate = firstDate.AddMonths(1).AddDays(-1);
                                string from = string.Format("{0:MM/dd/yyyy}", firstDate);
                                string to = string.Format("{0:MM/dd/yyyy}", LastDate);
                                fdate = Convert.ToDateTime(from + " " + "12:00:00 AM");
                                tdate = Convert.ToDateTime(to + " " + "23:59:59");
                                fromto[0] = fdate;
                                fromto[1] = tdate;
                                criteria.Add("TimeIn", fromto);
                                break;
                            }
                        default: break;
                    }
                }
                sord = sord == "desc" ? "Desc" : "Asc";
                UserService us = new UserService();
                Dictionary<long, IList<Session>> LoginList = us.GetSessionListWithPaging(page - 1, rows, sord, sidx, criteria);
                if (LoginList != null && LoginList.Count > 0)
                {
                    if (ExportType == "Excel")
                    {
                        var List = LoginList.First().Value.ToList();
                        base.ExptToXL(List, "LoginList", (items => new
                        {
                            items.Id,
                            items.UserId,
                            items.UserType,
                            TimeIn = items.TimeIn != null ? items.TimeIn.Value.ToString("dd/MM/yyyy hh:mm:ss tt") : null,
                            TimeOut = items.TimeOut != null ? items.TimeOut.Value.ToString("dd/MM/yyyy hh:mm:ss tt") : null,
                            items.IPAddress,
                            items.BrowserName,
                            items.BrowserVersion,
                            items.BrowserType,
                            items.Platform,
                        }));
                        return new EmptyResult();

                    }
                    else
                    {
                        long totalrecords = LoginList.First().Key;
                        int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                        var LoginLst = new
                        {
                            total = totalPages,
                            page = page,
                            records = totalrecords,

                            rows = (
                                 from items in LoginList.First().Value

                                 select new
                                 {
                                     cell = new string[] 
                                         {
                                            items.Id.ToString(),
                                            items.UserId,
                                            items.UserType,
                                            items.TimeIn!=null?items.TimeIn.Value.ToString("dd/MM/yyyy hh:mm:ss tt"):null,
                                            items.TimeOut!=null?items.TimeOut.Value.ToString("dd/MM/yyyy hh:mm:ss tt"):null,
                                            items.IPAddress,
                                            items.BrowserName,
                                            items.BrowserVersion,
                                            items.BrowserType,
                                            items.Platform,
                                           
                                         }
                                 }).ToList()
                        };
                        return Json(LoginLst, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "CustomAccountPolicy");
                throw ex;
            }
        }
        public Session GetLoginDetails(User u)
        {
            try
            {
                Session s = new Session();
                System.Web.HttpBrowserCapabilitiesBase browser = Request.Browser;
                s.UserId = u.UserId;
                s.UserType = u.UserType;
                s.TimeIn = DateTime.Now;
                s.IPAddress = Request.UserHostAddress;
                s.BrowserName = browser.Browser;
                s.BrowserVersion = browser.Version;
                s.Platform = browser.Platform;
                s.BrowserType = browser.Type;
                return s;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "CustomAccountPolicy");
                throw ex;
            }
        }
        public ActionResult GetPassword()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "CustomAccountPolicy");
                throw ex;
            }
        }

        public ActionResult GetPasswordJqGrid(string userid, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                UserService us = new UserService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                if (!string.IsNullOrEmpty(userid))
                {
                    criteria.Add("UserId", userid);
                }
                sord = sord == "desc" ? "Desc" : "Asc";
                Dictionary<long, IList<User>> UserList = us.GetUserListWithPagingAndCriteria(0, 9999, sidx, sord, criteria);
                PassworAuth PA = new PassworAuth();
                IList<User> UserListValue = new List<User>();

                int count = UserList.First().Value.Count;
                if (UserList != null && UserList.Count > 0 && UserList.First().Key > 0)
                {
                    string[] Password = new string[count];
                    string[] UserId = new string[count];
                    var li = (from items in UserList.First().Value
                              select new { userid = items.UserId, pass = items.Password }).ToArray();

                    for (int i = 0; i < li.Length; i++)
                    {
                        User u = new User();
                        u.UserId = li[i].userid;
                        u.Password = PA.base64Decode2(li[i].pass);
                        UserListValue.Add(u);
                    }
                }

                if (UserListValue != null && UserListValue.Count > 0)
                {
                    long totalrecords = UserList.First().Key;
                    int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                    var jsondat = new
                    {
                        total = totalPages,
                        page = page,
                        records = totalrecords,

                        rows = (from items in UserListValue
                                select new
                                {
                                    cell = new string[] {
                               items.UserId,
                               items.Password,
                            }
                                }).ToList()
                    };
                    return Json(jsondat, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "CustomAccountPolicy");
                throw ex;
            }
        }
        public JsonResult GetUserIds(string term)
        {
            try
            {
                UserService us = new UserService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                criteria.Add("UserId", term);
                //change the method to not to use the count since it is not being used here "REVISIT"
                Dictionary<long, IList<User>> UserList = us.GetUserListWithPagingAndCriteriaLikeSearch(0, 9999, string.Empty, string.Empty, criteria);
                var UserIds = (from u in UserList.First().Value
                               where u.UserId != null
                               select u.UserId).Distinct().ToList();
                return Json(UserIds, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "CustomAccountPolicy");
                throw ex;
            }
        }
        #region "MenuList Details"

        public ActionResult Menu()
        {
            try
            {
                string userId = ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "CustomAccountPolicy");
                throw ex;
            }
        }

        public JsonResult Menujqgrid(string ParentId, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                MastersService ms = new MastersService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                Dictionary<long, IList<Menu>> menulist = new Dictionary<long, IList<Menu>>();
                if (string.IsNullOrEmpty(ParentId))
                {
                    criteria.Add("ParentORChild", true);
                    menulist = ms.GetMenuListWithPagingAndCriteria(0, 1000, string.Empty, string.Empty, criteria);
                }
                else
                {
                    int parentid = Convert.ToInt32(ParentId);
                    criteria.Add("ParentRefId", parentid);
                    menulist = ms.GetMenuListWithPagingAndCriteria(0, 1000, string.Empty, string.Empty, criteria);
                }

                if (menulist != null && menulist.Count > 0)
                {
                    long totalrecords = menulist.First().Key;
                    int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                    var jsondat1 = new
                    {
                        total = totalPages,
                        page = page,
                        records = totalrecords,

                        rows = (from items in menulist.First().Value
                                select new
                                {
                                    i = 2,
                                    cell = new string[] {
                               items.Id.ToString(),
                               items.MenuName,
                               items.MenuLevel,
                               items.Role,
                               items.Controller,
                               items.Action,
                            }
                                })
                    };
                    return Json(jsondat1, JsonRequestBehavior.AllowGet);
                }
                return null;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "CustomAccountPolicy");
                throw ex;
            }
        }
        //felix need to cehck why he added the edit as parameter "REVISIT"
        public ActionResult AddMenu(Menu m, string edit)
        {
            try
            {
                string userId = ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    MastersService ms = new MastersService();
                    m.ParentORChild = true;
                    ms.SaveOrUpdateMenuDetails(m);
                }
                return RedirectToAction("Masters", "Home");
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "CustomAccountPolicy");
                throw ex;
            }
        }
        //this method uses for each to remove the child of a parent "REVISIT"
        //this need to be changed bu getting all the items by using long[] array and delete using deleteAll
        public ActionResult DeleteMenu(long id)
        {
            try
            {
                string userId = ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    MastersService ms = new MastersService();
                    Menu mu = new Menu();
                    Dictionary<string, object> criteria = new Dictionary<string, object>();
                    criteria.Add("ParentRefId", (int)id);
                    Dictionary<long, IList<Menu>> DeleteMenuList = ms.GetMenuListWithPagingAndCriteria(0, 100, string.Empty, string.Empty, criteria);
                    if (DeleteMenuList != null && DeleteMenuList.Count > 0 && DeleteMenuList.First().Key > 0)
                    {
                        long[] SubMenuIds = (from items in DeleteMenuList.First().Value
                                             select items.Id).ToArray();
                        for (int i = 0; i < SubMenuIds.Length; i++)
                        {
                            mu = ms.GetDeleteMenurowById(SubMenuIds[i]);
                            ms.DeleteMenufunction(mu);
                        }
                    }
                    mu = ms.GetDeleteMenurowById(id);
                    ms.DeleteMenufunction(mu);
                    var script = @"SucessMsg(""Deleted  Successfully"");";
                    return JavaScript(script);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "CustomAccountPolicy");
                throw ex;
            }
        }

        public ActionResult AddSubMenus(Menu Submenu, int ids)
        {
            try
            {
                string userId = ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    MastersService ms = new MastersService();
                    Submenu.ParentORChild = false;
                    Submenu.MenuLevel = "Level2";
                    Submenu.ParentRefId = ids;
                    ms.SaveOrUpdateSubMenuDetails(Submenu);
                }
                return RedirectToAction("Masters", "Home");
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "CustomAccountPolicy");
                throw ex;
            }
        }

        public ActionResult DeleteSubMenus(long id)
        {
            try
            {
                string userId = ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    MastersService ms = new MastersService();
                    Menu mu = ms.DeleteSubMenurowById(id);
                    ms.DeleteMenufunction(mu);
                    var script = @"SucessMsg(""Deleted  Successfully"");";
                    return JavaScript(script);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "CustomAccountPolicy");
                throw ex;
            }
        }
        #endregion "End MenuList Details"

        public JsonResult GetEmailList(string Ids)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Ids) && Ids != "undefined")
                {
                    UserService us = new UserService();
                    Dictionary<string, object> criteria = new Dictionary<string, object>();

                    long[] UserIds = new long[Ids.Split(',').Length];

                    UserIds = Ids.Split(',').Select(n => Convert.ToInt64(n)).ToArray();

                    criteria.Add("Id", UserIds);
                    //change the method to not to use the count since it is not being used here "REVISIT"
                    Dictionary<long, IList<User>> UserList = us.GetUserListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                    if (UserList != null && UserList.First().Value != null && UserList.First().Value.Count > 0)
                    {
                        var EmailList = (
                                 from items in UserList.First().Value
                                 select new
                                 {
                                     Text = items.EmailId,
                                     Value = items.EmailId
                                 }).Distinct().ToList().OrderBy(x => x.Text);
                        return Json(EmailList, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(null, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "CustomAccountPolicy");
                throw ex;
            }
        }
    }
}
// First Review Completed (by JP and Anbu on 15 Mar 2014 - 21 mar 2014)
