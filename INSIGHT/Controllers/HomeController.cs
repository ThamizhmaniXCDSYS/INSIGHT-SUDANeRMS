using System;
using System.Web.Mvc;
using INSIGHT.WCFServices;
using System.Collections.Generic;
using INSIGHT.Entities;
using System.Linq;


namespace INSIGHT.Controllers
{
    public class HomeController : BaseController
    {
        Dictionary<string, object> criteria = new Dictionary<string, object>();
        MenuService ms = new MenuService();

        public ActionResult Home()
        {
            WindowsMailServices wc = new WindowsMailServices();
            //wc.StartWindowService("Start");
            return View();
        }
        
    
        //public ActionResult ExportExcel(string idno, string name, string section, string campname, string grade, string bType, string astatus, string acayear, int rows)
        //{
        //    try
        //    {
        //        string userId = base.ValidateUser();
        //        if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
        //        else
        //        {
        //            if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
        //            else
        //            {
        //                MasterDataService mds = new MasterDataService();
        //                AdmissionManagementService ams = new AdmissionManagementService();
        //                Dictionary<string, object> criteria = new Dictionary<string, object>();
        //                if (!string.IsNullOrWhiteSpace(idno))
        //                {
        //                    idno = idno.Trim();
        //                    criteria.Add("NewId", idno);
        //                }
        //                if (!string.IsNullOrWhiteSpace(name))
        //                {
        //                    name = name.Trim();
        //                    criteria.Add("Name", name);
        //                }
        //                if (!string.IsNullOrWhiteSpace(campname))
        //                {
        //                    if (campname.Contains("Select"))
        //                    {
        //                    }
        //                    else
        //                        criteria.Add("Campus", campname);
        //                }

        //                if (!string.IsNullOrWhiteSpace(section))
        //                {
        //                    if (section.Contains("Select"))
        //                    {
        //                    }
        //                    else
        //                        criteria.Add("Section", section);
        //                }

        //                if (!string.IsNullOrWhiteSpace(grade))
        //                {
        //                    if (grade.Contains("Select"))
        //                    {
        //                        grade = "";
        //                    }
        //                    else
        //                        criteria.Add("Grade", grade);
        //                }
        //                if (!string.IsNullOrWhiteSpace(bType))
        //                {
        //                    if (bType.Contains("Select"))
        //                    {
        //                    }
        //                    else
        //                        criteria.Add("BoardingType", bType);
        //                }
        //                if (!string.IsNullOrWhiteSpace(astatus))
        //                {
        //                    criteria.Add("AdmissionStatus", astatus);
        //                }

        //                if (!string.IsNullOrWhiteSpace(acayear))
        //                {
        //                    if (acayear.Contains("Select"))
        //                    {
        //                    }
        //                    else
        //                        criteria.Add("AcademicYear", acayear);
        //                }
        //                Dictionary<long, IList<StudentDetailsExport>> studentdetailslist;
        //                if (!string.IsNullOrWhiteSpace(grade))
        //                {

        //                    studentdetailslist = ams.GetStudentExportListWithEQsearchCriteria(0, rows, string.Empty, string.Empty, criteria);
        //                }
        //                else
        //                {
        //                    studentdetailslist = ams.GetStudentExportListWithPagingAndCriteria(0, rows, string.Empty, string.Empty, criteria);

        //                }
        //                if (studentdetailslist != null && studentdetailslist.First().Value != null && studentdetailslist.First().Value.Count > 0)
        //                {
        //                    var stuList = studentdetailslist.First().Value.ToList();
        //                    base.ExptToXL(stuList, "StudentList", (items => new
        //                    {
        //                        items.Campus,
        //                        items.CreatedDate,
        //                        items.NewId,
        //                        items.Name,
        //                        items.Gender,
        //                        items.Grade,
        //                        items.Section,
        //                        items.AcademicYear,
        //                        items.BoardingType,
        //                        items.FoodType,
        //                        items.TransportRequired,
        //                        items.FatherName,
        //                        items.MotherName,
        //                        items.FatherMobileNumber,
        //                        items.MotherMobileNumber,
        //                        items.EmailId,
        //                        items.FatherEmail,
        //                        items.MotherEmail,
        //                        items.Address
        //                    }));
        //                    return new EmptyResult();
        //                }
        //                else
        //                {
        //                    return null;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionPolicy.HandleException(ex, "CallMgmntPolicy");
        //        throw ex;
        //    }
        //}

        #region Admin added by Micheal
        public ActionResult Admin()
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
        public ActionResult DocUpload()
        {
            return View();
        }

        public ActionResult Masters()
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    criteria.Clear();
                    criteria.Add("ParentORChild", true);
                    criteria.Add("MenuName", "Masters");
                    Dictionary<long, IList<Menu>> mainmenu = ms.GetMenuListWithPagingAndCriteria(0, 1000, string.Empty, string.Empty, criteria);
                    criteria.Clear();
                    criteria.Add("ParentRefId",Convert.ToInt32(mainmenu.FirstOrDefault().Value[0].Id));
                    Dictionary<long, IList<Menu>> MastersList = ms.GetMenuListWithPagingAndCriteria(0, 1000, string.Empty, string.Empty, criteria);
                    ViewBag.ddlMaster = MastersList.FirstOrDefault().Value;
                    return View();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult UserAppRole()
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    UserService us = new UserService();
                    Dictionary<string, object> criteria = new Dictionary<string, object>();
                    Dictionary<long, IList<Application>> appcode = us.GetApplicationListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                    Dictionary<long, IList<Role>> rolecode = us.GetRoleListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                    //var usrcmp = Session["SectorCode"] as IEnumerable<string>;
                    //if (usrcmp.First() != null)            // to check if the usrcmp obj is null or with data
                    //{
                    //    criteria.Add("Name", usrcmp);
                    //}
                    MastersService ms = new MastersService();
                    criteria.Clear();

                    ViewBag.appcodeddl = appcode.First().Value;
                    ViewBag.rolecodeddl = rolecode.First().Value;

                    return View();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult UserAppRolejqgrid(string id, string txtSearch, string userid, string appcd, string rlcd, string brncd, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {

                UserService us = new UserService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();


                if (!string.IsNullOrWhiteSpace(userid))
                {
                    criteria.Add("UserId", userid);
                }
                if ((!string.IsNullOrWhiteSpace(appcd)))
                {
                    if (appcd.Contains("Select"))
                    {

                    }
                    else
                    {
                        criteria.Add("AppCode", appcd);
                    }
                }
                if (!string.IsNullOrWhiteSpace(rlcd))
                {
                    if (rlcd.Contains("Select"))
                    {

                    }
                    else
                    {
                        criteria.Add("RoleCode", rlcd);
                    }
                }
                if (!string.IsNullOrWhiteSpace(brncd))
                {
                    if (brncd.Contains("Select"))
                    {

                    }
                    else
                    {
                        criteria.Add("SectorCode", brncd);
                    }
                }
                if (!string.IsNullOrWhiteSpace(sord) && sord == "desc")
                    sord = "Desc";
                else
                    sord = "Asc";
                Dictionary<long, IList<UserAppRole>> userapprole = us.GetAppRoleForAnUserListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                if (userapprole != null && userapprole.Count > 0)
                {
                    long totalrecords = userapprole.First().Key;
                    int totalPages = (int)Math.Ceiling(totalrecords / (float)rows);
                    var jsondat = new
                    {
                        total = totalPages,
                        page = page,
                        records = totalrecords,

                        rows = (from items in userapprole.First().Value
                                select new
                                {
                                    i = 2,
                                    cell = new string[] {
                               items.UserId,
                               items.AppCode,
                               items.RoleCode,
                               items.SectorCode,
                               items.Email,
                               items.Id.ToString() 
                               //String.Format(@"<img src='/Images/history3.jpg' width='20px' height='20px'  id='ImgHistory' onclick='ShowComments('" + items.Id + "')' />") 
                            }
                                })
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
                throw ex;
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddUserAppRole(UserAppRole apm, string test)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    UserService aps = new UserService();
                    Dictionary<string, object> criteria = new Dictionary<string, object>();
                    criteria.Add("UserId", apm.UserId);
                    criteria.Add("AppCode", apm.AppCode);
                    criteria.Add("RoleCode", apm.RoleCode);
                    criteria.Add("SectorCode", apm.SectorCode);
                    Dictionary<long, IList<UserAppRole>> userapprole = aps.GetAppRoleForAnUserListWithPagingAndCriteria(null, null, null, null, criteria);
                    if (test == "edit")
                    {
                        if (userapprole != null && userapprole.First().Value != null && userapprole.First().Value.Count > 1)
                        {
                            return null; //return Json("false"); //throw new Exception("The given region is already exists!");
                        }
                        else
                        {
                            if (apm.RoleCode == "CSE")
                            {
                                apm.ContingentCode = null;
                            }
                            apm.RoleName = "nil";
                            apm.AppName = "nil";
                            ViewBag.flag = 1;
                            aps.CreateOrUpdateUserAppRole(apm);
                            return null;
                        }
                    }
                    else
                    {
                        if (userapprole != null && userapprole.First().Value != null && userapprole.First().Value.Count > 0)
                        {
                            var script1 = @"ErrMsg(""This Combination already exists"");";
                            return JavaScript(script1);
                        }
                        else
                        {
                            if (apm.RoleCode == "CSE")
                            {
                                apm.ContingentCode = null;
                            }
                            apm.AppName = "depart";
                            apm.RoleName = "role";
                            aps.CreateOrUpdateUserAppRole(apm);
                            var script = @"SucessMsg(""Role mapped Sucessfully"");";
                            return JavaScript(script);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        public ActionResult KeepAlive() {
            return View();
        }

        public ActionResult Test()
        {
            return null;
        }
    }
}

