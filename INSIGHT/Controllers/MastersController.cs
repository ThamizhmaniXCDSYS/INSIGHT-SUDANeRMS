using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using INSIGHT.WCFServices;
using INSIGHT.Entities;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using INSIGHT.Entities.InvoiceEntities;
using INSIGHT.Entities.EmailEntities;
using INSIGHT.Entities.Masters;



namespace INSIGHT.Controllers
{
    public class MastersController : BaseController
    {
        //
        // GET: /Masters/
        MastersService mssvc = new MastersService();
        EmailService ES = new EmailService();
        Dictionary<string, object> criteria = new Dictionary<string, object>();


        public ActionResult ContingentMaster()
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
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }

        }
        public ActionResult SectorMaster()
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
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        public ActionResult ItemMaster()
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
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        public ActionResult UNAMEIDMasterJqGrid(string UNCode, string Commodity, string UnitSizeReq, string APLCode, string ExptType, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    criteria.Clear();
                    sord = sord == "desc" ? "Desc" : "Asc";
                    if (!string.IsNullOrWhiteSpace(Commodity)) { criteria.Add("Commodity", Commodity); }
                    if (!string.IsNullOrWhiteSpace(UNCode)) { criteria.Add("UNCode", Convert.ToInt64(UNCode)); }
                    if (!string.IsNullOrWhiteSpace(APLCode)) { criteria.Add("APLCode", Convert.ToInt32(APLCode)); }
                    if (!string.IsNullOrWhiteSpace(UnitSizeReq)) { criteria.Add("UnitSizeReq", UnitSizeReq); }
                    Dictionary<long, IList<ItemMaster>> ItemMasterList = null;
                    if (!string.IsNullOrWhiteSpace(Commodity))
                    {
                        ItemMasterList = mssvc.GetItemMasterListWithLikeSearchPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                    }
                    else
                    {
                        ItemMasterList = mssvc.GetItemMasterListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                    }
                    if (ItemMasterList != null && ItemMasterList.FirstOrDefault().Value.Count > 0 && ItemMasterList.FirstOrDefault().Key > 0)
                    {
                        if (ExptType == "Excel")
                        {
                            int i = 1;
                            foreach (var item in ItemMasterList)
                            {
                                item.Value.FirstOrDefault().Id = i;
                                i = i + 1;
                            }
                            var List = ItemMasterList.First().Value.ToList();
                            List<string> lstHeader = new List<string>() { "SNo", "UNCode", "Commodity", "Unit Size Request", "APLCode", "Line Items Orderd", "Actual Count" };
                            base.NewExportToExcel(List, "Item Master", (items => new
                            {
                                items.Id,
                                items.UNCode,
                                items.Commodity,
                                items.UnitSizeReq,
                                items.APLCode,
                            }), lstHeader);
                            return new EmptyResult();
                        }
                        else
                        {
                            long totalrecords = ItemMasterList.First().Key;
                            int totalPages = (int)Math.Ceiling(totalrecords / (float)5);
                            var AssLst = new
                            {
                                total = totalPages,
                                page = page,
                                records = totalrecords,
                                rows = (
                                     from items in ItemMasterList.First().Value

                                     select new
                                     {
                                         cell = new string[] 
                                         {
                                             items.Id.ToString(),
                                             items.UNCode.ToString(),
                                             items.Commodity,
                                             items.UnitSizeReq,
                                             items.APLCode.ToString(),
                                         }
                                     }).ToList()
                            };

                            return Json(AssLst, JsonRequestBehavior.AllowGet);
                        }
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
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }

        }
        //public ActionResult ContingentMasterJqGrid(string ContingentControlNo, string TypeofUnit, string Nationality, string DeliveryPoint, string ExptType, string LocationCode, string TroopStrength, string DeliveryMode, string Distance, string SectorCode, string SectorName, string DeliveryModeDescription, long ContingentID, int rows, string sidx, string sord, int? page = 1)
        public ActionResult ContingentMasterJqGrid(ContingentMaster cm, string ExptType, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {

                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    criteria.Clear();
                    sord = sord == "desc" ? "Desc" : "Asc";
                    if (!string.IsNullOrWhiteSpace(cm.ContingentControlNo)) { criteria.Add("ContingentControlNo", cm.ContingentControlNo); }
                    if (!string.IsNullOrWhiteSpace(cm.TypeofUnit)) { criteria.Add("TypeofUnit", cm.TypeofUnit); }
                    if (!string.IsNullOrWhiteSpace(cm.Nationality)) { criteria.Add("Nationality", cm.Nationality); }
                    if (!string.IsNullOrWhiteSpace(cm.DeliveryPoint)) { criteria.Add("DeliveryPoint", cm.DeliveryPoint); }
                    if (!string.IsNullOrWhiteSpace(cm.LocationCode)) { criteria.Add("LocationCode", cm.LocationCode); }
                    if (cm.TroopStrength > 0) { criteria.Add("TroopStrength", cm.TroopStrength); }
                    if (!string.IsNullOrWhiteSpace(cm.DeliveryMode)) { criteria.Add("DeliveryMode", cm.DeliveryMode); }
                    if (cm.Distance > 0) { criteria.Add("Distance", cm.Distance); }
                    if (!string.IsNullOrWhiteSpace(cm.SectorCode)) { criteria.Add("SectorCode", cm.SectorCode); }
                    if (!string.IsNullOrWhiteSpace(cm.SectorName)) { criteria.Add("SectorName", cm.SectorName); }
                    if (!string.IsNullOrWhiteSpace(cm.DeliveryModeDescription)) { criteria.Add("DeliveryModeDescription", cm.ContingentControlNo); }
                    if (cm.ContingentID > 0) { criteria.Add("ContingentID", Convert.ToInt64(cm.ContingentID)); }
                    Dictionary<long, IList<ContingentMaster>> ContingentList = null;
                    if ((!string.IsNullOrWhiteSpace(cm.DeliveryPoint)) || (!string.IsNullOrWhiteSpace(cm.Nationality)) || (!string.IsNullOrWhiteSpace(cm.DeliveryMode)) || (!string.IsNullOrWhiteSpace(cm.SectorName)))
                    {
                        ContingentList = mssvc.GetContingentMasterListWithLikeSearchPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                    }
                    else
                    {
                        ContingentList = mssvc.GetContigentListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                    }
                    if (ContingentList != null && ContingentList.FirstOrDefault().Value.Count > 0 && ContingentList.FirstOrDefault().Key > 0)
                    {
                        if (ExptType == "Excel")
                        {
                            int i = 1;
                            foreach (var item in ContingentList)
                            {
                                item.Value.FirstOrDefault().Id = i;
                                i = i + 1;
                            }
                            var List = ContingentList.First().Value.ToList();
                            List<string> lstHeader = new List<string>() { "SNo", "Contingent Control No", "Type of Unit", "Sector Code", "Nationality", "Delivery Point", "Location Code", "Troop Strength", "Delivery Mode", "Distance", "Sector Name" };
                            base.NewExportToExcel(List, "Contingent Master", (items => new
                            {
                                items.Id,
                                items.ContingentControlNo,
                                items.TypeofUnit,
                                items.SectorCode,
                                items.Nationality,
                                items.DeliveryPoint,
                                items.LocationCode,
                                items.TroopStrength,
                                items.DeliveryMode,
                                items.Distance,
                                items.SectorName,
                                items.DeliveryModeDescription,
                                items.ContingentID,
                            }), lstHeader);
                        }
                        else
                        {
                            long totalrecords = ContingentList.First().Key;
                            int totalPages = (int)Math.Ceiling(totalrecords / (float)5);
                            var AssLst = new
                            {
                                total = totalPages,
                                page = page,
                                records = totalrecords,
                                rows = (
                                     from items in ContingentList.First().Value

                                     select new
                                     {
                                         cell = new string[] 
                                         {
                                             items.Id.ToString(),
                                             items.ContingentControlNo,
                                             items.TypeofUnit,
                                             items.SectorCode,
                                             items.Nationality,
                                             items.DeliveryPoint,
                                             items.LocationCode,
                                             items.TroopStrength.ToString(),
                                             items.DeliveryMode,
                                             items.Distance.ToString(),
                                             items.SectorName,
                                             items.DeliveryModeDescription,
                                             items.ContingentID.ToString()
                                         }
                                     }).ToList()
                            };

                            return Json(AssLst, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else return null;

                }
                return null;

            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        public ActionResult SectorMasterJqGrid(string SectorCode, string SectorName, string Description, string Location, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    criteria.Clear();
                    sord = sord == "desc" ? "Desc" : "Asc";
                    if (!string.IsNullOrWhiteSpace(SectorCode)) { criteria.Add("SectorCode", SectorCode); }
                    if (!string.IsNullOrWhiteSpace(SectorName)) { criteria.Add("SectorName", SectorName); }
                    if (!string.IsNullOrWhiteSpace(Description)) { criteria.Add("Description", Description); }
                    if (!string.IsNullOrWhiteSpace(Location)) { criteria.Add("Location", Location); }
                    Dictionary<long, IList<SectorMaster>> SectorList = null;

                    if ((!string.IsNullOrWhiteSpace(SectorName)) || (!string.IsNullOrWhiteSpace(Location)) || (!string.IsNullOrWhiteSpace(Description)))
                    {
                        SectorList = mssvc.GetSectorMasterListWithLikeSearchPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                    }
                    else
                    {
                        SectorList = mssvc.GetSectorMasterListWithPagingAndCriteria(page - 1, rows, sord, sidx, criteria);
                    }
                    if (SectorList != null && SectorList.FirstOrDefault().Value.Count > 0 && SectorList.FirstOrDefault().Key > 0)
                    {
                        long totalrecords = SectorList.First().Key;
                        int totalPages = (int)Math.Ceiling(totalrecords / (float)5);
                        var AssLst = new
                        {
                            total = totalPages,
                            page = page,
                            records = totalrecords,
                            rows = (
                            from items in SectorList.First().Value

                            select new
                            {
                                cell = new string[]
                                {
                                items.Id.ToString(),
                                items.SectorCode,
                                items.SectorName,
                                
                                items.Description,
                                items.Location,
                                }
                            }).ToList()
                        };

                        return Json(AssLst, JsonRequestBehavior.AllowGet);
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
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }

        public ActionResult PeriodMaster()
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
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }

        }
        public ActionResult LocationMaster()
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
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        public ActionResult CountryMaster()
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
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        public ActionResult UNSectorMapping()
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
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        public ActionResult PeriodMasterJqGrid(string Week, string Year, string FromDate, string EndDate, string ExptType, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    criteria.Clear();
                    sord = sord == "desc" ? "Desc" : "Asc";
                    if (!string.IsNullOrWhiteSpace(Week)) { criteria.Add("Week", Convert.ToInt32(Week)); }
                    if (!string.IsNullOrWhiteSpace(Year)) { criteria.Add("Year", Year); }
                    if (!string.IsNullOrWhiteSpace(FromDate)) { criteria.Add("FromDate", FromDate); }
                    if (!string.IsNullOrWhiteSpace(EndDate)) { criteria.Add("EndDate", EndDate); }
                    Dictionary<long, IList<PeriodMaster>> PeriodMasterList = null;
                    PeriodMasterList = mssvc.GetPeriodMasterListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                    if (PeriodMasterList != null && PeriodMasterList.FirstOrDefault().Value.Count > 0 && PeriodMasterList.FirstOrDefault().Key > 0)
                    {
                        if (ExptType == "Excel")
                        {
                            int i = 1;
                            foreach (var item in PeriodMasterList.FirstOrDefault().Value)
                            {
                                item.Id = i;
                                i = i + 1;
                            }
                            var List = PeriodMasterList.First().Value.ToList();
                            List<string> lstHeader = new List<string>() { "SNo", "Period", "Week", "Year", "Start Date", "End Date", "Start Day", "End Day" };
                            base.NewExportToExcel(List, "Period Master", (items => new
                            {
                                items.Id,
                                items.Period,
                                items.Week,
                                items.Year,
                                items.StartDate,
                                items.EndDate,
                                items.StartDay,
                                items.EndDay
                            }), lstHeader);
                        }

                        else
                        {
                            long totalrecords = PeriodMasterList.First().Key;
                            int totalPages = (int)Math.Ceiling(totalrecords / (float)5);
                            var AssLst = new
                            {
                                total = totalPages,
                                page = page,
                                records = totalrecords,
                                rows = (
                                     from items in PeriodMasterList.First().Value

                                     select new
                                     {
                                         cell = new string[] 
                                         {
                                             items.Id.ToString(),
                                             items.Period,
                                             items.Week.ToString(),
                                             items.Year,
                                             items.StartDate,
                                             items.EndDate,
                                             items.StartDay,
                                             items.EndDay
                                         }
                                     }).ToList()
                            };

                            return Json(AssLst, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        var jsondat = new { rows = (new { cell = new string[] { } }) };
                        return Json(jsondat, JsonRequestBehavior.AllowGet);

                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }

        public ActionResult CountryMasterJqGrid(string CountryCode, string CountryName, string SectorCode, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    criteria.Clear();
                    sord = sord == "desc" ? "Desc" : "Asc";
                    if (!string.IsNullOrWhiteSpace(CountryCode)) { criteria.Add("CountryCode", CountryCode); }
                    if (!string.IsNullOrWhiteSpace(CountryName)) { criteria.Add("CountryName", CountryName); }
                    if (!string.IsNullOrWhiteSpace(SectorCode)) { criteria.Add("SectorCode", SectorCode); }
                    Dictionary<long, IList<CountryMaster>> CountryMasterList = null;
                    if (!string.IsNullOrWhiteSpace(CountryName))
                    {
                        CountryMasterList = mssvc.GetCountryMasterListWithLikeSearchPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                    }
                    else
                    {
                        CountryMasterList = mssvc.GetCountryMasterListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                    }
                    if (CountryMasterList != null && CountryMasterList.FirstOrDefault().Value.Count > 0 && CountryMasterList.FirstOrDefault().Key > 0)
                    {
                        long totalrecords = CountryMasterList.First().Key;
                        int totalPages = (int)Math.Ceiling(totalrecords / (float)5);
                        var AssLst = new
                        {
                            total = totalPages,
                            page = page,
                            records = totalrecords,
                            rows = (
                                 from items in CountryMasterList.First().Value

                                 select new
                                 {
                                     cell = new string[] 
                                         {
                                             items.Id.ToString(),
                                             items.CountryCode,
                                             items.CountryName,
                                             items.SectorCode,
                                         }
                                 }).ToList()
                        };

                        return Json(AssLst, JsonRequestBehavior.AllowGet);
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
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }


        public ActionResult LocationMasterJqgrid(string LocationCode, string LocationName, string CountryCode, string ExptType, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    criteria.Clear();
                    sord = sord == "desc" ? "Desc" : "Asc";
                    if (!string.IsNullOrWhiteSpace(LocationCode)) { criteria.Add("LocationCode", LocationCode); }
                    if (!string.IsNullOrWhiteSpace(LocationName)) { criteria.Add("LocationName", LocationName); }
                    if (!string.IsNullOrWhiteSpace(CountryCode)) { criteria.Add("CountryCode", CountryCode); }
                    Dictionary<long, IList<LocationMaster>> LocationMasterList = null;
                    LocationMasterList = mssvc.GetLocationMasterListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                    if (LocationMasterList != null && LocationMasterList.FirstOrDefault().Value.Count > 0 && LocationMasterList.FirstOrDefault().Key > 0)
                    {

                        if (ExptType == "Excel")
                        {
                            int i = 1;
                            foreach (var item in LocationMasterList)
                            {
                                item.Value.FirstOrDefault().Id = i;
                                i = i + 1;
                            }
                            var List = LocationMasterList.First().Value.ToList();
                            List<string> lstHeader = new List<string>() { "SNo", "Location Code", "Location Name", "CountryCode" };
                            base.NewExportToExcel(List, "Location Master", (items => new
                            {
                                items.Id,
                                items.LocationCode,
                                items.LocationName,
                                items.CountryCode,
                            }), lstHeader);
                        }
                        else
                        {
                            long totalrecords = LocationMasterList.First().Key;
                            int totalPages = (int)Math.Ceiling(totalrecords / (float)5);
                            var AssLst = new
                            {
                                total = totalPages,
                                page = page,
                                records = totalrecords,
                                rows = (
                                     from items in LocationMasterList.First().Value

                                     select new
                                     {
                                         cell = new string[] 
                                         {
                                             items.Id.ToString(),
                                             items.LocationCode,
                                             items.LocationName,
                                             items.CountryCode,
                                         }
                                     }).ToList()
                            };

                            return Json(AssLst, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        var jsondat = new { rows = (new { cell = new string[] { } }) };
                        return Json(jsondat, JsonRequestBehavior.AllowGet);

                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }

        public ActionResult UNSectorMappingtJqgrid(string MappingCode, string UNCode, string SectorCode, string PriceOrUnit, string ExptType, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    criteria.Clear();
                    sord = sord == "desc" ? "Desc" : "Asc";
                    if (!string.IsNullOrWhiteSpace(MappingCode)) { criteria.Add("MappingCode", MappingCode); }
                    if (!string.IsNullOrWhiteSpace(UNCode)) { criteria.Add("UNCode", UNCode); }
                    if (!string.IsNullOrWhiteSpace(SectorCode)) { criteria.Add("SectorCode", SectorCode); }
                    if (!string.IsNullOrWhiteSpace(PriceOrUnit)) { criteria.Add("UnitPrice", PriceOrUnit); }
                    Dictionary<long, IList<UNSectorConMapping>> UNSectorMappingList = null;
                    UNSectorMappingList = mssvc.GetUNSectorMappingMasterListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                    if (UNSectorMappingList != null && UNSectorMappingList.FirstOrDefault().Value.Count > 0 && UNSectorMappingList.FirstOrDefault().Key > 0)
                    {
                        if (ExptType == "Excel")
                        {
                            int i = 1;
                            foreach (var item in UNSectorMappingList)
                            {
                                item.Value.FirstOrDefault().Id = i;
                                i = i + 1;
                            }
                            var List = UNSectorMappingList.First().Value.ToList();
                            List<string> lstHeader = new List<string>() { "SNo", "Mapping Code", "UNCode", "Commodity", "Sector Code", "UnitP rice" };
                            base.NewExportToExcel(List, "UNSectorMapping Master", (items => new
                            {
                                items.Id,
                                items.MappingCode,
                                items.UNCode,
                                items.Commodity,
                                items.SectorCode,
                                items.UnitPrice
                            }), lstHeader);
                        }

                        else
                        {
                            long totalrecords = UNSectorMappingList.First().Key;
                            int totalPages = (int)Math.Ceiling(totalrecords / (float)5);
                            var AssLst = new
                            {
                                total = totalPages,
                                page = page,
                                records = totalrecords,
                                rows = (
                                     from items in UNSectorMappingList.First().Value

                                     select new
                                     {
                                         cell = new string[] 
                                         {
                                             items.Id.ToString(),
                                             items.MappingCode,
                                             items.UNCode,
                                             items.Commodity,
                                             items.SectorCode,
                                             items.UnitPrice.ToString()
                                         }
                                     }).ToList()
                            };

                            return Json(AssLst, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        var jsondat = new { rows = (new { cell = new string[] { } }) };
                        return Json(jsondat, JsonRequestBehavior.AllowGet);

                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }

        # region ItemMaster
        public ActionResult AddItemMaster(ItemMaster im)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    criteria.Clear();
                    Dictionary<long, IList<ItemMaster>> ItemMasterList = mssvc.GetItemMasterListWithPagingAndCriteria(0, 99999, string.Empty, string.Empty, criteria);
                    if (ItemMasterList != null && ItemMasterList.FirstOrDefault().Value.Count > 0 && ItemMasterList.FirstOrDefault().Key > 0)
                    {
                        foreach (var item in ItemMasterList.First().Value)
                        {
                            if (item.UNCode == im.UNCode)
                            {
                                var script = @"ErrMsg(""Already Exist"");";
                                return JavaScript(script);
                            }
                            else { }
                        }
                    }
                    im.DateCreated = DateTime.Now;
                    im.CreatedBy = userId;
                    im.DateModified = DateTime.Now;
                    im.ModifiedBy = userId;
                    mssvc.SaveOrUpdateItemMasterDetails(im);
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        public ActionResult EditItemMaster(ItemMaster im)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    if (im.Id > 0)
                    {
                        ItemMaster itm = mssvc.GetItemDetailsById(im.Id);
                        if (itm.UNCode == im.UNCode)
                        {
                            var script = @"ErrMsg(""Already Exist"");";
                            return JavaScript(script);
                        }
                        else if (itm.Commodity == im.Commodity)
                        {
                            var script = @"ErrMsg(""Already Exist"");";
                            return JavaScript(script);
                        }
                        else if (itm.APLCode == im.APLCode)
                        {
                            var script = @"ErrMsg(""Already Exist"");";
                            return JavaScript(script);
                        }
                        else if (itm.UnitSizeReq == im.UnitSizeReq)
                        {
                            var script = @"ErrMsg(""Already Exist"");";
                            return JavaScript(script);
                        }
                        else
                        {
                            itm.UNCode = im.UNCode;
                            itm.Commodity = im.Commodity;
                            itm.UnitSizeReq = im.UnitSizeReq;
                            itm.APLCode = im.APLCode;
                            itm.ModifiedBy = userId;
                            itm.DateModified = DateTime.Now;
                            mssvc.SaveOrUpdateItemMasterDetails(itm);
                        }
                    }
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        public ActionResult DeleteItemMaster(string[] Id)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    int i;
                    string[] arrayId = Id[0].Split(',');
                    for (i = 0; i < arrayId.Length; i++)
                    {
                        var singleId = arrayId[i];
                        ItemMaster im = mssvc.GetItemDetailsById(Convert.ToInt64(singleId));
                        mssvc.GetDeleteItemMasterById(im);
                    }
                    var script = @"SucessMsg(""Deleted Sucessfully"");";
                    return JavaScript(script);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        #endregion  ItemMaster

        # region Sector Master
        public ActionResult AddSectorMaster(SectorMaster secm)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    criteria.Clear();
                    Dictionary<long, IList<SectorMaster>> SectorList = mssvc.GetSectorMasterListWithPagingAndCriteria(99999, 0, string.Empty, string.Empty, criteria);
                    if (SectorList != null && SectorList.FirstOrDefault().Value.Count > 0 && SectorList.FirstOrDefault().Key > 0)
                    {
                        foreach (var item in SectorList.First().Value)
                        {
                            if (item.SectorCode == secm.SectorCode)
                            {
                                var script = @"ErrMsg(""Already Exist"");";
                                return JavaScript(script);
                            }
                            else { }
                        }
                    }
                    secm.DateCreated = DateTime.Now;
                    secm.CreatedBy = userId;
                    secm.DateModified = DateTime.Now;
                    secm.ModifiedBy = userId;
                    mssvc.SaveOrUpdateSectorMasterDetails(secm);
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        public ActionResult EditSectorMaster(SectorMaster secm)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    if (secm.Id > 0)
                    {
                        SectorMaster sectm = mssvc.GetSectorDetailsById(secm.Id);
                        if (sectm.Location == secm.Location)
                        {
                            var script = @"ErrMsg(""Already Exist"");";
                            return JavaScript(script);
                        }

                        else if (sectm.Description == secm.Description)
                        {
                            var script = @"ErrMsg(""Already Exist"");";
                            return JavaScript(script);
                        }

                        else if (sectm.SectorCode == secm.SectorCode)
                        {
                            var script = @"ErrMsg(""Already Exist"");";
                            return JavaScript(script);
                        }

                        else if (sectm.SectorName == secm.SectorName)
                        {
                            var script = @"ErrMsg(""Already Exist"");";
                            return JavaScript(script);
                        }
                        else
                        {
                            sectm.SectorCode = secm.SectorCode;
                            sectm.SectorName = secm.SectorName;
                            sectm.Description = secm.Description;
                            sectm.Location = secm.Location;
                            sectm.ModifiedBy = userId;
                            sectm.DateModified = DateTime.Now;
                            mssvc.SaveOrUpdateSectorMasterDetails(sectm);
                        }
                    }
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        public ActionResult DeleteSectorMaster(string[] Id)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    int i;
                    string[] arrayId = Id[0].Split(',');
                    for (i = 0; i < arrayId.Length; i++)
                    {
                        var singleId = arrayId[i];
                        SectorMaster sm = mssvc.GetSectorDetailsById(Convert.ToInt64(singleId));
                        mssvc.GetDeleteSectorMasterById(sm);
                    }
                    var script = @"SucessMsg(""Deleted Sucessfully"");";
                    return JavaScript(script);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        # endregion SectionMaster

        # region ContingentMaster
        public ActionResult AddContingentMaster(ContingentMaster cm)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    criteria.Clear();
                    criteria.Add("ContingentControlNo", cm.ContingentControlNo);
                    criteria.Add("TypeofUnit", cm.TypeofUnit);
                    criteria.Add("Nationality", cm.Nationality);
                    criteria.Add("DeliveryPoint", cm.DeliveryPoint);
                    criteria.Add("LocationCode", cm.LocationCode);
                    criteria.Add("TroopStrength", cm.TroopStrength);
                    criteria.Add("DeliveryMode", cm.DeliveryMode);
                    criteria.Add("Distance", cm.Distance);
                    criteria.Add("SectorCode", cm.SectorCode);
                    criteria.Add("SectorName", cm.SectorName);
                    criteria.Add("DeliveryModeDescription", cm.DeliveryModeDescription);
                    criteria.Add("ContingentID", cm.ContingentID);


                    Dictionary<long, IList<ContingentMaster>> ContingentList = mssvc.GetContigentListWithPagingAndCriteria(99999, 0, string.Empty, string.Empty, criteria);
                    if (ContingentList != null && ContingentList.FirstOrDefault().Value.Count > 0 && ContingentList.FirstOrDefault().Key > 0)
                    {
                        var script = @"ErrMsg(""Already Exist"");";
                        return JavaScript(script);

                    }
                    cm.DateCreated = DateTime.Now;
                    cm.CreatedBy = userId;
                    cm.DateModified = DateTime.Now;
                    cm.ModifiedBy = userId;
                    mssvc.SaveOrUpdateContingentMasterDetails(cm);
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        public ActionResult EditContingentMaster(ContingentMaster cm)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    if (cm.Id > 0)
                    {
                        ContingentMaster cntm = mssvc.GetContingentDetailsById(cm.Id);
                        cntm.ContingentControlNo = cm.ContingentControlNo;
                        cntm.TypeofUnit = cm.TypeofUnit;
                        cntm.Nationality = cm.Nationality;
                        cntm.DeliveryPoint = cm.DeliveryPoint;
                        cntm.LocationCode = cm.LocationCode;
                        cntm.TroopStrength = cm.TroopStrength;
                        cntm.DeliveryMode = cm.DeliveryMode;
                        cntm.Distance = cm.Distance;
                        cntm.SectorCode = cm.SectorCode;
                        cntm.SectorName = cm.SectorName;
                        cntm.DeliveryModeDescription = cm.DeliveryModeDescription;
                        cntm.ContingentID = cm.ContingentID;
                        cntm.ModifiedBy = userId;
                        cntm.DateModified = DateTime.Now;
                        mssvc.SaveOrUpdateContingentMasterDetails(cntm);
                    }
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        public ActionResult DeleteContingentMaster(string[] Id)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    int i;
                    string[] arrayId = Id[0].Split(',');
                    for (i = 0; i < arrayId.Length; i++)
                    {
                        var singleId = arrayId[i];
                        ContingentMaster cm = mssvc.GetContingentDetailsById(Convert.ToInt64(singleId));
                        mssvc.GetDeleteContingentrowById(cm);
                    }
                    var script = @"SucessMsg(""Deleted Sucessfully"");";
                    return JavaScript(script);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        #endregion

        //Added by Gobi start

        # region Location Master
        public ActionResult AddLocationMasterDetails(LocationMaster lm)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    criteria.Clear();
                    Dictionary<long, IList<LocationMaster>> LocationMasterList = mssvc.GetLocationMasterListWithPagingAndCriteria(0, 99999, string.Empty, string.Empty, criteria);
                    if (LocationMasterList != null && LocationMasterList.FirstOrDefault().Value.Count > 0 && LocationMasterList.FirstOrDefault().Key > 0)
                    {
                        foreach (var item in LocationMasterList.First().Value)
                        {
                            if (item.LocationCode == lm.LocationCode)
                            {
                                var script = @"ErrMsg(""Already Exist"");";
                                return JavaScript(script);
                            }
                            else if (item.LocationName == lm.LocationName)
                            {
                                var script = @"ErrMsg(""Already Exist"");";
                                return JavaScript(script);
                            }
                            else { }
                        }
                    }
                    lm.DateCreated = DateTime.Now;
                    lm.CreatedBy = userId;
                    lm.DateModified = DateTime.Now;
                    lm.ModifiedBy = userId;
                    mssvc.SaveOrUpdateLocationMasterDetails(lm);
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        public ActionResult EditLocationMasterDetails(LocationMaster lm)
        {
            try
            {

                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    if (lm.Id > 0)
                    {
                        LocationMaster LocMast = mssvc.GetLocationDetailsById(lm.Id);
                        if (LocMast.Id == lm.Id)
                        {
                            var script = @"ErrMsg(""Already Exist"");";
                            return JavaScript(script);
                        }
                        else if (LocMast.LocationCode == lm.LocationCode)
                        {
                            var script = @"ErrMsg(""Already Exist"");";
                            return JavaScript(script);
                        }
                        else if (LocMast.LocationName == lm.LocationName)
                        {
                            var script = @"ErrMsg(""Already Exist"");";
                            return JavaScript(script);
                        }
                        else if (LocMast.CountryCode == lm.CountryCode)
                        {
                            var script = @"ErrMsg(""Already Exist"");";
                            return JavaScript(script);
                        }
                        else
                        {
                            LocMast.LocationCode = lm.LocationCode;
                            LocMast.LocationName = lm.LocationName;
                            LocMast.CountryCode = lm.CountryCode;
                            LocMast.ModifiedBy = userId;
                            LocMast.DateModified = DateTime.Now;
                            mssvc.SaveOrUpdateLocationMasterDetails(LocMast);
                        }
                    }
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        public ActionResult DeleteLocationMasterDetails(string[] Id)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    int i;
                    string[] arrayId = Id[0].Split(',');
                    for (i = 0; i < arrayId.Length; i++)
                    {
                        var singleId = arrayId[i];
                        LocationMaster lm = mssvc.GetLocationDetailsById(Convert.ToInt64(singleId));
                        mssvc.GetDeleteLocationMasterById(lm);
                    }
                    var script = @"SucessMsg(""Deleted Sucessfully"");";
                    return JavaScript(script);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        # endregion

        # region Country Master
        public ActionResult AddCountryMasterDetails(CountryMaster cm)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    criteria.Clear();
                    Dictionary<long, IList<CountryMaster>> CountryMasterList = mssvc.GetCountryMasterListWithPagingAndCriteria(0, 99999, string.Empty, string.Empty, criteria);
                    if (CountryMasterList != null && CountryMasterList.FirstOrDefault().Value.Count > 0 && CountryMasterList.FirstOrDefault().Key > 0)
                    {
                        foreach (var item in CountryMasterList.First().Value)
                        {
                            if (item.CountryCode == cm.CountryCode)
                            {
                                var script = @"ErrMsg(""Already Exist"");";
                                return JavaScript(script);
                            }
                            else if (item.SectorCode == cm.SectorCode)
                            {
                                var script = @"ErrMsg(""Already Exist"");";
                                return JavaScript(script);
                            }
                            else { }
                        }
                    }
                    cm.DateCreated = DateTime.Now;
                    cm.CreatedBy = userId;
                    cm.DateModified = DateTime.Now;
                    cm.ModifiedBy = userId;
                    mssvc.SaveOrUpdateCountryMasterDetails(cm);
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        public ActionResult EditCountryMasterDetails(CountryMaster cm)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    if (cm.Id > 0)
                    {
                        CountryMaster CountMast = mssvc.GetCountryDetailsById(cm.Id);
                        if (CountMast.Id == cm.Id)
                        {
                            var script = @"ErrMsg(""Already Exist"");";
                            return JavaScript(script);
                        }
                        else if (CountMast.CountryCode == cm.CountryCode)
                        {
                            var script = @"ErrMsg(""Already Exist"");";
                            return JavaScript(script);
                        }
                        else if (CountMast.CountryName == cm.CountryName)
                        {
                            var script = @"ErrMsg(""Already Exist"");";
                            return JavaScript(script);
                        }
                        else if (CountMast.SectorCode == cm.SectorCode)
                        {
                            var script = @"ErrMsg(""Already Exist"");";
                            return JavaScript(script);
                        }
                        else
                        {
                            CountMast.CountryCode = cm.CountryCode;
                            CountMast.CountryName = cm.CountryName;
                            CountMast.SectorCode = cm.SectorCode;

                            CountMast.ModifiedBy = userId;
                            CountMast.DateModified = DateTime.Now;
                            mssvc.SaveOrUpdateCountryMasterDetails(CountMast);
                        }
                    }
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        public ActionResult DeleteCountryMasterDetails(string[] Id)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    int i;
                    string[] arrayId = Id[0].Split(',');
                    for (i = 0; i < arrayId.Length; i++)
                    {
                        var singleId = arrayId[i];
                        CountryMaster cm = mssvc.GetCountryDetailsById(Convert.ToInt64(singleId));
                        mssvc.GetDeleteCountryMasterById(cm);
                    }
                    var script = @"SucessMsg(""Deleted Sucessfully"");";
                    return JavaScript(script);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        # endregion

        #region UNSectorMappping Master
        public ActionResult AddUNSectorMapppingMasterDetails(UNSectorConMapping unsm)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    criteria.Clear();
                    Dictionary<long, IList<UNSectorConMapping>> UNSectorMapppingMasterList = mssvc.GetUNSectorMappingMasterListWithPagingAndCriteria(0, 99999, string.Empty, string.Empty, criteria);
                    if (UNSectorMapppingMasterList != null && UNSectorMapppingMasterList.FirstOrDefault().Value.Count > 0 && UNSectorMapppingMasterList.FirstOrDefault().Key > 0)
                    {
                        foreach (var item in UNSectorMapppingMasterList.First().Value)
                        {
                            if (item.MappingCode == unsm.MappingCode)
                            {
                                var script = @"ErrMsg(""Already Exist"");";
                                return JavaScript(script);
                            }
                            else if (item.UNCode == unsm.UNCode)
                            {
                                var script = @"ErrMsg(""Already Exist"");";
                                return JavaScript(script);
                            }
                            else if (item.SectorCode == unsm.SectorCode)
                            {
                                var script = @"ErrMsg(""Already Exist"");";
                                return JavaScript(script);
                            }
                            else if (item.UnitPrice == unsm.UnitPrice)
                            {
                                var script = @"ErrMsg(""Already Exist"");";
                                return JavaScript(script);
                            }
                            else { }
                        }
                    }
                    unsm.DateCreated = DateTime.Now;
                    unsm.CreatedBy = userId;
                    unsm.DateModified = DateTime.Now;
                    unsm.ModifiedBy = userId;
                    mssvc.SaveOrUpdateUNSectorMapppingMasterDetails(unsm);
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        public ActionResult EditUNSectorMapppingMasterDetails(UNSectorConMapping unsm)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    if (unsm.Id > 0)
                    {
                        UNSectorConMapping UnsmMast = mssvc.GetUNSectorMapppingDetailsById(unsm.Id);
                        if (UnsmMast.Id == unsm.Id)
                        {
                            var script = @"ErrMsg(""Already Exist"");";
                            return JavaScript(script);
                        }
                        else if (UnsmMast.MappingCode == unsm.MappingCode)
                        {
                            var script = @"ErrMsg(""Already Exist"");";
                            return JavaScript(script);
                        }
                        else if (UnsmMast.UNCode == unsm.UNCode)
                        {
                            var script = @"ErrMsg(""Already Exist"");";
                            return JavaScript(script);
                        }
                        else if (UnsmMast.SectorCode == unsm.SectorCode)
                        {
                            var script = @"ErrMsg(""Already Exist"");";
                            return JavaScript(script);
                        }
                        else if (UnsmMast.UnitPrice == unsm.UnitPrice)
                        {
                            var script = @"ErrMsg(""Already Exist"");";
                            return JavaScript(script);
                        }
                        else
                        {
                            UnsmMast.MappingCode = unsm.MappingCode;
                            UnsmMast.UNCode = unsm.UNCode;
                            UnsmMast.SectorCode = unsm.SectorCode;
                            UnsmMast.UnitPrice = unsm.UnitPrice;

                            UnsmMast.ModifiedBy = userId;
                            UnsmMast.DateModified = DateTime.Now;
                            mssvc.SaveOrUpdateUNSectorMapppingMasterDetails(UnsmMast);
                        }
                    }
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        public ActionResult DeleteUNSectorMapppingMasterDetails(string[] Id)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {

                    int i;
                    string[] arrayId = Id[0].Split(',');
                    for (i = 0; i < arrayId.Length; i++)
                    {
                        var singleId = arrayId[i];
                        UNSectorConMapping unsm = mssvc.GetUNSectorMapppingDetailsById(Convert.ToInt64(singleId));
                        mssvc.GetDeleteUNSectorMapppingMasterById(unsm);
                    }
                    var script = @"SucessMsg(""Deleted Sucessfully"");";
                    return JavaScript(script);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        # endregion

        #region CMR Master
        public ActionResult AddCMRMasterDetails(CMRMaster cmr)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    criteria.Clear();
                    Dictionary<long, IList<CMRMaster>> CMRMasterList = mssvc.GetCMRMasterListWithPagingAndCriteria(0, 99999, string.Empty, string.Empty, criteria);
                    if (CMRMasterList != null && CMRMasterList.FirstOrDefault().Value.Count > 0 && CMRMasterList.FirstOrDefault().Key > 0)
                    {
                        foreach (var item in CMRMasterList.First().Value)
                        {
                            if (item.SectorCode == cmr.SectorCode)
                            {
                                var script = @"ErrMsg(""Already Exist"");";
                                return JavaScript(script);
                            }
                            else if (item.Price == cmr.Price)
                            {
                                var script = @"ErrMsg(""Already Exist"");";
                                return JavaScript(script);
                            }
                            else { }
                        }
                    }
                    cmr.DateCreated = DateTime.Now;
                    cmr.CreatedBy = userId;
                    cmr.DateModified = DateTime.Now;
                    cmr.ModifiedBy = userId;
                    mssvc.SaveOrUpdateCMRMasterDetails(cmr);
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        public ActionResult EditCMRMasterDetails(CMRMaster cmr)
        {
            try
            {

                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    if (cmr.Id > 0)
                    {
                        CMRMaster cmrMast = mssvc.GetCMRDetailsById(cmr.Id);
                        if (cmrMast.SectorCode == cmr.SectorCode)
                        {
                            var script = @"ErrMsg(""Already Exist"");";
                            return JavaScript(script);
                        }
                        else if (cmrMast.Price == cmr.Price)
                        {
                            var script = @"ErrMsg(""Already Exist"");";
                            return JavaScript(script);
                        }
                        else
                        {
                            cmrMast.SectorCode = cmr.SectorCode;
                            cmrMast.Price = cmr.Price;
                            cmrMast.ModifiedBy = userId;
                            cmrMast.DateModified = DateTime.Now;
                            mssvc.SaveOrUpdateCMRMasterDetails(cmrMast);
                        }
                    }
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        public ActionResult DeleteCMRMasterDetails(string[] Id)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    int i;
                    string[] arrayId = Id[0].Split(',');
                    for (i = 0; i < arrayId.Length; i++)
                    {
                        var singleId = arrayId[i];
                        CMRMaster cmr = mssvc.GetCMRDetailsById(Convert.ToInt64(singleId));
                        mssvc.GetDeleteCMRMasterById(cmr);
                    }
                    var script = @"SucessMsg(""Deleted Sucessfully"");";
                    return JavaScript(script);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        # endregion

        #region Discrepancy Master
        public ActionResult AddDiscrepancyMasterDetails(DiscrepancyMaster dm)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    criteria.Clear();
                    Dictionary<long, IList<DiscrepancyMaster>> DiscrepancyMasterList = mssvc.GetDiscrepancyMasterListWithPagingAndCriteria(0, 99999, string.Empty, string.Empty, criteria);
                    if (DiscrepancyMasterList != null && DiscrepancyMasterList.FirstOrDefault().Value.Count > 0 && DiscrepancyMasterList.FirstOrDefault().Key > 0)
                    {
                        foreach (var item in DiscrepancyMasterList.First().Value)
                        {
                            if (item.DiscrepancyCode == dm.DiscrepancyCode)
                            {
                                var script = @"ErrMsg(""Already Exist"");";
                                return JavaScript(script);
                            }
                            else if (item.Description == dm.Description)
                            {
                                var script = @"ErrMsg(""Already Exist"");";
                                return JavaScript(script);
                            }
                            else { }
                        }
                    }
                    dm.DateCreated = DateTime.Now;
                    dm.CreatedBy = userId;
                    dm.DateModified = DateTime.Now;
                    dm.ModifiedBy = userId;
                    mssvc.SaveOrUpdateDiscrepancyMasterDetails(dm);
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        public ActionResult EditDiscrepancyMasterDetails(DiscrepancyMaster dm)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    if (dm.Id > 0)
                    {
                        DiscrepancyMaster dmMast = mssvc.GetDiscrepancyDetailsById(dm.Id);
                        if (dmMast.DiscrepancyCode == dm.DiscrepancyCode)
                        {
                            var script = @"ErrMsg(""Already Exist"");";
                            return JavaScript(script);
                        }
                        else if (dmMast.Description == dm.Description)
                        {
                            var script = @"ErrMsg(""Already Exist"");";
                            return JavaScript(script);
                        }
                        else
                        {
                            dmMast.DiscrepancyCode = dm.DiscrepancyCode;
                            dmMast.Description = dm.Description;

                            dmMast.ModifiedBy = userId;
                            dmMast.DateModified = DateTime.Now;
                            mssvc.SaveOrUpdateDiscrepancyMasterDetails(dmMast);
                        }
                    }
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        public ActionResult DeleteDiscrepancyMasterDetails(string[] Id)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    int i;
                    string[] arrayId = Id[0].Split(',');
                    for (i = 0; i < arrayId.Length; i++)
                    {
                        var singleId = arrayId[i];
                        DiscrepancyMaster dm = mssvc.GetDiscrepancyDetailsById(Convert.ToInt64(singleId));
                        mssvc.GetDeleteDiscrepancyMasterById(dm);
                    }
                    var script = @"SucessMsg(""Deleted Sucessfully"");";
                    return JavaScript(script);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        # endregion

        #region UOMMasters
        public ActionResult AddUOMMasterMaster(UOMMaster im)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    criteria.Clear();
                    Dictionary<long, IList<UOMMaster>> UOMMasterList = mssvc.GetUOMMasterListWithPagingAndCriteria(0, 999999, string.Empty, string.Empty, criteria);
                    if (UOMMasterList != null && UOMMasterList.FirstOrDefault().Value.Count > 0 && UOMMasterList.FirstOrDefault().Key > 0)
                    {
                        foreach (var item in UOMMasterList.First().Value)
                        {
                            if (item.UNCode == im.UNCode)
                            {
                                var script = @"ErrMsg(""Already Exist"");";
                                return JavaScript(script);
                            }
                            else { }

                        }
                    }
                    im.CreatedBy = userId;
                    im.CreatedDate = DateTime.Now;
                    im.ModifiedDate = DateTime.Now;
                    mssvc.SaveOrUpdateUOMMasterDetails(im);
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        public ActionResult EditUOMMasterMaster(UOMMaster uom)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    if (uom.Id > 0)
                    {
                        UOMMaster UOMM = mssvc.GetUOMMasterDetailsById(uom.Id);
                        if (UOMM.UNCode == uom.UNCode)
                        {
                            var script = @"ErrMsg(""Already Exist"");";
                            return JavaScript(script);
                        }
                        else
                        {
                            UOMM.UNCode = uom.UNCode;
                            UOMM.ItemName = uom.ItemName;
                            UOMM.Type = uom.Type;
                            UOMM.UOM = uom.UOM;
                            UOMM.ModifiedBy = userId;
                            UOMM.ModifiedDate = DateTime.Now;
                            mssvc.SaveOrUpdateUOMMasterDetails(UOMM);
                        }

                    }
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        public ActionResult DeleteUOMMasterMaster(string[] Id)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    if (!string.IsNullOrEmpty(Id[0]))
                    {
                        var id = Id[0].Split(',');
                        long[] Ids = new long[id.Length];
                        int i = 0;
                        foreach (string val in id)
                        {
                            Ids[i] = Convert.ToInt64(val);
                            i++;
                        }
                        mssvc.DeleteUOMMasterDetails(Ids);

                    }
                    return Json(null, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        #endregion


        //Added by Gobi end
        public JsonResult FillSector()
        {
            try
            {
                string userid = (Session["UserId"] == null) ? "" : Session["UserId"].ToString();
                MastersService ms = new MastersService();
                Dictionary<long, IList<SectorMaster>> SectorMaster = ms.GetSectorMasterListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, null);
                if (SectorMaster != null && SectorMaster.First().Value != null && SectorMaster.First().Value.Count > 0)
                {
                    var BranchCodeList = (
                             from items in SectorMaster.First().Value
                             select new
                             {
                                 Text = items.SectorCode,
                                 Value = items.SectorCode
                             }).Distinct().ToList().OrderBy(x => x.Text);
                    return Json(BranchCodeList, JsonRequestBehavior.AllowGet);
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

        public JsonResult FillContingent(string SectorCode)
        {
            try
            {
                string userid = (Session["UserId"] == null) ? "" : Session["UserId"].ToString();
                MastersService ms = new MastersService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                if (!string.IsNullOrWhiteSpace(SectorCode)) { criteria.Add("SectorCode", SectorCode); }
                Dictionary<long, IList<ContingentMaster>> ContingentMaster = ms.GetContigentListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                if (ContingentMaster != null && ContingentMaster.First().Value != null && ContingentMaster.First().Value.Count > 0)
                {
                    var BranchCodeList = (
                             from items in ContingentMaster.First().Value
                             select new
                             {
                                 Text = items.ContingentControlNo,
                                 Value = items.ContingentControlNo
                             }).Distinct().ToList().OrderBy(x => x.Text);
                    return Json(BranchCodeList, JsonRequestBehavior.AllowGet);
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

        public JsonResult FillPeriod(string PeriodYear)
        {
            try
            {
                string userid = (Session["UserId"] == null) ? "" : Session["UserId"].ToString();
                MastersService ms = new MastersService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                if (!string.IsNullOrWhiteSpace(PeriodYear)) { criteria.Add("Year", PeriodYear); }
                Dictionary<long, IList<PeriodMaster>> PeriodMaster = ms.GetPeriodMasterListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                if (PeriodMaster != null && PeriodMaster.First().Value != null && PeriodMaster.First().Value.Count > 0)
                {
                    var BranchCodeList = (
                             from items in PeriodMaster.First().Value
                             select new
                             {
                                 Text = items.Period,
                                 Value = items.Period
                             }).Distinct().ToList().OrderBy(x => x.Text);
                    return Json(BranchCodeList, JsonRequestBehavior.AllowGet);
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
        //Added by Thamizhmani
        public JsonResult FillWeekbyPeriodandPeriodYear(string Period, string PeriodYear)
        {
            try
            {
                string userid = (Session["UserId"] == null) ? "" : Session["UserId"].ToString();
                MastersService ms = new MastersService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                if (!string.IsNullOrWhiteSpace(Period)) { criteria.Add("Period", Period); }
                if (!string.IsNullOrWhiteSpace(PeriodYear)) { criteria.Add("Year", PeriodYear); }
                Dictionary<long, IList<PeriodMaster>> PeriodMaster = ms.GetPeriodMasterListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                if (PeriodMaster != null && PeriodMaster.First().Value != null && PeriodMaster.First().Value.Count > 0)
                {
                    var BranchCodeList = (
                             from items in PeriodMaster.First().Value
                             select new
                             {
                                 Text = items.Week,
                                 Value = items.Week
                             }).Distinct().ToList().OrderBy(x => x.Text);
                    return Json(BranchCodeList, JsonRequestBehavior.AllowGet);
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

        public JsonResult FillLocation()
        {
            try
            {
                string userid = (Session["UserId"] == null) ? "" : Session["UserId"].ToString();
                MastersService ms = new MastersService();
                Dictionary<long, IList<LocationMaster>> LocationMaster = ms.GetLocationMasterListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, null);
                if (LocationMaster != null && LocationMaster.First().Value != null && LocationMaster.First().Value.Count > 0)
                {
                    var BranchCodeList = (
                             from items in LocationMaster.First().Value
                             select new
                             {
                                 Text = items.LocationName,
                                 Value = items.LocationName
                             }).Distinct().ToList().OrderBy(x => x.Text);
                    return Json(BranchCodeList, JsonRequestBehavior.AllowGet);
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


        #region New Masters

        public ActionResult CMRMaster()
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
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }

        public ActionResult UOMMaster()
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {

                    UOMMaster uom = new UOMMaster();
                    return View(uom);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }

        public ActionResult DiscrepancyMaster()
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
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }

        public ActionResult UOMMasterJqgrid(string UNCode, string ItemName, string Type, string ExptType, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    criteria.Clear();
                    sord = sord == "desc" ? "Desc" : "Asc";
                    if (!string.IsNullOrWhiteSpace(UNCode)) { criteria.Add("UNCode", Convert.ToInt64(UNCode)); }
                    if (!string.IsNullOrWhiteSpace(ItemName)) { criteria.Add("ItemName", ItemName); }
                    if (!string.IsNullOrWhiteSpace(Type)) { criteria.Add("Type", Type); }
                    Dictionary<long, IList<UOMMaster>> UOMMasterList = mssvc.GetUOMMasterListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                    if (UOMMasterList != null && UOMMasterList.FirstOrDefault().Value.Count > 0 && UOMMasterList.FirstOrDefault().Key > 0)
                    {


                        if (ExptType == "Excel")
                        {
                            int i = 1;
                            foreach (var item in UOMMasterList)
                            {
                                item.Value.FirstOrDefault().Id = i;
                                i = i + 1;
                            }
                            var List = UOMMasterList.First().Value.ToList();
                            List<string> lstHeader = new List<string>() { "SNo", "UNCode", "Item Name", "Type", "UOM" };
                            base.NewExportToExcel(List, "UOM Master", (items => new
                            {
                                items.Id,
                                items.UNCode,
                                items.ItemName,
                                items.Type,
                                items.UOM,
                            }), lstHeader);
                        }
                        else
                        {
                            long totalrecords = UOMMasterList.First().Key;
                            int totalPages = (int)Math.Ceiling(totalrecords / (float)5);
                            var AssLst = new
                            {
                                total = totalPages,
                                page = page,
                                records = totalrecords,
                                rows = (
                                     from items in UOMMasterList.First().Value

                                     select new
                                     {
                                         cell = new string[] 
                                         {
                                             items.Id.ToString(),
                                             items.UNCode.ToString(),
                                             items.ItemName,
                                             items.Type,
                                             items.UOM,
                                         }
                                     }).ToList()
                            };

                            return Json(AssLst, JsonRequestBehavior.AllowGet);
                        }
                        return Json(null, JsonRequestBehavior.AllowGet);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }

        public ActionResult DiscrepancyMasterJqgrid(string DiscrepancyCode, string Description, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    criteria.Clear();
                    sord = sord == "desc" ? "Desc" : "Asc";
                    if (!string.IsNullOrWhiteSpace(DiscrepancyCode)) { criteria.Add("DiscrepancyCode", DiscrepancyCode); }
                    if (!string.IsNullOrWhiteSpace(Description)) { criteria.Add("Description", Description); }
                    Dictionary<long, IList<DiscrepancyMaster>> DiscrepancyMasterList = null;
                    if (!string.IsNullOrWhiteSpace(Description))
                    {
                        DiscrepancyMasterList = mssvc.GetDiscrepancyMasterListWithLikeSearchPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                    }
                    else
                    {
                        DiscrepancyMasterList = mssvc.GetDiscrepancyMasterListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                    }
                    if (DiscrepancyMasterList != null && DiscrepancyMasterList.FirstOrDefault().Value.Count > 0 && DiscrepancyMasterList.FirstOrDefault().Key > 0)
                    {
                        long totalrecords = DiscrepancyMasterList.First().Key;
                        int totalPages = (int)Math.Ceiling(totalrecords / (float)5);
                        var AssLst = new
                        {
                            total = totalPages,
                            page = page,
                            records = totalrecords,
                            rows = (
                                 from items in DiscrepancyMasterList.First().Value

                                 select new
                                 {
                                     cell = new string[] 
                                         {
                                             items.Id.ToString(),
                                             items.DiscrepancyCode,
                                             items.Description,
                                         }
                                 }).ToList()
                        };

                        return Json(AssLst, JsonRequestBehavior.AllowGet);
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
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }

        public ActionResult CMRMasterJqgrid(string SectorCode, string Price, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    criteria.Clear();
                    sord = sord == "desc" ? "Desc" : "Asc";
                    if (!string.IsNullOrWhiteSpace(SectorCode)) { criteria.Add("SectorCode", SectorCode); }
                    if (!string.IsNullOrWhiteSpace(Price)) { criteria.Add("Price", Price); }
                    Dictionary<long, IList<CMRMaster>> CMRMasterList = null;
                    CMRMasterList = mssvc.GetCMRMasterListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                    if (CMRMasterList != null && CMRMasterList.FirstOrDefault().Value.Count > 0 && CMRMasterList.FirstOrDefault().Key > 0)
                    {
                        long totalrecords = CMRMasterList.First().Key;
                        int totalPages = (int)Math.Ceiling(totalrecords / (float)5);
                        var AssLst = new
                        {
                            total = totalPages,
                            page = page,
                            records = totalrecords,
                            rows = (
                                 from items in CMRMasterList.First().Value

                                 select new
                                 {
                                     cell = new string[] 
                                         {
                                             items.Id.ToString(),
                                             items.SectorCode,
                                             items.Price,
                                         }
                                 }).ToList()
                        };

                        return Json(AssLst, JsonRequestBehavior.AllowGet);
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
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }

        #endregion

        #region Added by anbu
        public JsonResult FillSectorbyUser()
        {
            try
            {
                string userid = (Session["UserId"] == null) ? "" : Session["UserId"].ToString();
                UserService us = new UserService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                criteria.Add("UserId", userid);
                Dictionary<long, IList<UserAppRole>> UserAppRole = us.GetAppRoleForAnUserListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                if (UserAppRole != null && UserAppRole.First().Value != null && UserAppRole.First().Value.Count > 0)
                {
                    var UserRoleList = (from u in UserAppRole.First().Value
                                        where u.RoleCode != null
                                        select new { u.AppCode }).Distinct().ToArray();
                    //UserRoleList="all"?"All":"all";
                    foreach (var item in UserRoleList)
                    {

                        //if (item.AppCode == "all" | item.AppCode == "All")
                        //{
                        criteria.Clear();
                        Dictionary<long, IList<SectorMaster>> SectorList = mssvc.GetSectorMasterListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                        if (SectorList != null && SectorList.First().Value != null && SectorList.First().Value.Count > 0)
                        {
                            var SectorMasterList = (
                                                     from items in SectorList.First().Value
                                                     select new
                                                     {
                                                         Text = items.SectorCode,
                                                         Value = items.SectorCode
                                                     }).Distinct().ToList().OrderBy(x => x.Text);
                            return Json(SectorMasterList, JsonRequestBehavior.AllowGet);
                        }
                        //}

                    }
                    var SectorCodeList = (
                    from items in UserAppRole.First().Value
                    select new
                    {
                        Text = items.SectorCode,
                        Value = items.SectorCode
                    }).Distinct().ToList().OrderBy(x => x.Text);
                    return Json(SectorCodeList, JsonRequestBehavior.AllowGet);
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

        public JsonResult FillContingentbyUser(string SectorCode)
        {
            try
            {
                string userid = (Session["UserId"] == null) ? "" : Session["UserId"].ToString();
                UserService us = new UserService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                criteria.Add("UserId", userid);
                //if (!string.IsNullOrWhiteSpace(SectorCode) && SectorCode != "undefined") { criteria.Add("SectorCode", SectorCode); }
                Dictionary<long, IList<UserAppRole>> UserAppRole = us.GetAppRoleForAnUserListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                if (UserAppRole != null && UserAppRole.First().Value != null && UserAppRole.First().Value.Count > 0)
                {
                    var UserRoleList = (from u in UserAppRole.First().Value
                                        where u.RoleCode != null
                                        select new { u.AppCode }).Distinct().ToArray();
                    //UserRoleList="all"?"All":"all";
                    foreach (var item in UserRoleList)
                    {

                        //if (item.AppCode == "all" | item.AppCode == "All")
                        //{
                        criteria.Clear();
                        if (!string.IsNullOrWhiteSpace(SectorCode) && SectorCode != "undefined") { criteria.Add("SectorCode", SectorCode); }
                        Dictionary<long, IList<ContingentMaster>> ContingentMaster = mssvc.GetContigentListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                        if (ContingentMaster != null && ContingentMaster.First().Value != null && ContingentMaster.First().Value.Count > 0)
                        {
                            var ContingentMasterList = (
                                                     from items in ContingentMaster.First().Value
                                                     select new
                                                     {
                                                         Text = items.ContingentControlNo,
                                                         Value = items.ContingentControlNo
                                                     }).Distinct().ToList().OrderBy(x => x.Text);
                            return Json(ContingentMasterList, JsonRequestBehavior.AllowGet);
                        }
                        //}

                    }
                    var ContingentCodeList = (
                    from items in UserAppRole.First().Value
                    select new
                    {
                        Text = items.ContingentCode,
                        Value = items.ContingentCode
                    }).Distinct().ToList().OrderBy(x => x.Text);
                    return Json(ContingentCodeList, JsonRequestBehavior.AllowGet);
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

        public JsonResult FillLocationbyUser(string SectorCode, string ContingentCode)
        {
            try
            {
                string userid = (Session["UserId"] == null) ? "" : Session["UserId"].ToString();
                UserService us = new UserService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                criteria.Add("UserId", userid);
                //if (!string.IsNullOrWhiteSpace(SectorCode) && SectorCode != "undefined") { criteria.Add("SectorCode", SectorCode); }
                //if (!string.IsNullOrWhiteSpace(ContingentCode) && ContingentCode != "undefined") { criteria.Add("ContingentCode", ContingentCode); }
                Dictionary<long, IList<UserAppRole>> UserAppRole = us.GetAppRoleForAnUserListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                if (UserAppRole != null && UserAppRole.First().Value != null && UserAppRole.First().Value.Count > 0)
                {
                    var UserRoleList = (from u in UserAppRole.First().Value
                                        where u.RoleCode != null
                                        select new { u.AppCode }).Distinct().ToArray();
                    //UserRoleList="all"?"All":"all";
                    foreach (var item in UserRoleList)
                    {

                        //if (item.AppCode == "all" | item.AppCode == "All")
                        //{
                        criteria.Clear();
                        if (!string.IsNullOrWhiteSpace(SectorCode) && SectorCode != "undefined") { criteria.Add("SectorCode", SectorCode); }
                        if (!string.IsNullOrWhiteSpace(ContingentCode) && ContingentCode != "undefined") { criteria.Add("ContingentCode", ContingentCode); }
                        Dictionary<long, IList<LocationMaster>> LocationMaster = mssvc.GetLocationMasterListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, null);
                        if (LocationMaster != null && LocationMaster.First().Value != null && LocationMaster.First().Value.Count > 0)
                        {
                            var LocationMasterList = (
                                                     from items in LocationMaster.First().Value
                                                     select new
                                                     {
                                                         Text = items.LocationCode,
                                                         Value = items.LocationCode
                                                     }).Distinct().ToList().OrderBy(x => x.Text);
                            return Json(LocationMasterList, JsonRequestBehavior.AllowGet);
                        }
                        //}

                    }
                    var ContingentCodeList = (
                    from items in UserAppRole.First().Value
                    select new
                    {
                        Text = items.Location,
                        Value = items.Location
                    }).Distinct().ToList().OrderBy(x => x.Text);
                    return Json(ContingentCodeList, JsonRequestBehavior.AllowGet);
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

        #endregion

        #region Period Master
        public ActionResult AddPeriodMaster(PeriodMaster pm)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    criteria.Clear();
                    Dictionary<long, IList<PeriodMaster>> PeriodMasterList = mssvc.GetPeriodMasterListWithPagingAndCriteria(0, 99999, string.Empty, string.Empty, criteria);
                    if (PeriodMasterList != null && PeriodMasterList.FirstOrDefault().Value.Count > 0 && PeriodMasterList.FirstOrDefault().Key > 0)
                    {
                        foreach (var item in PeriodMasterList.First().Value)
                        {
                            if (item.Week == pm.Week)
                            {
                                if (item.Week == pm.Week && item.Year == pm.Year)
                                {
                                    var script = @"ErrMsg(""Already Exist"");";
                                    return JavaScript(script);
                                }
                            }
                            else if (item.StartDate == pm.StartDate)
                            {
                                var script = @"ErrMsg(""Already Exist"");";
                                return JavaScript(script);
                            }
                            else if (item.EndDate == pm.EndDate)
                            {
                                var script = @"ErrMsg(""Already Exist"");";
                                return JavaScript(script);
                            }
                            else { }

                        }
                    }
                    pm.DateCreated = DateTime.Now;
                    pm.CreatedBy = userId;
                    pm.DateModified = DateTime.Now;
                    pm.ModifiedBy = userId;
                    mssvc.SaveOrUpdatePeriodMasterDetails(pm);
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        public ActionResult EditPeriodMaster(PeriodMaster pm)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    if (pm.Id > 0)
                    {
                        PeriodMaster itm = mssvc.GetPeriodDetailsById(pm.Id);
                        if (itm.Period == pm.Period)
                        {
                            var script = @"ErrMsg(""Already Exist"");";
                            return JavaScript(script);
                        }
                        else if (itm.Week == pm.Week)
                        {
                            var script = @"ErrMsg(""Already Exist"");";
                            return JavaScript(script);
                        }
                        else if (itm.Year == pm.Year)
                        {
                            var script = @"ErrMsg(""Already Exist"");";
                            return JavaScript(script);
                        }
                        else if (itm.StartDate == pm.StartDate)
                        {
                            var script = @"ErrMsg(""Already Exist"");";
                            return JavaScript(script);
                        }
                        else if (itm.EndDate == pm.EndDate)
                        {
                            var script = @"ErrMsg(""Already Exist"");";
                            return JavaScript(script);
                        }
                        else
                        {
                            itm.Period = pm.Period;
                            itm.Week = pm.Week;
                            itm.Year = pm.Year;
                            itm.StartDate = pm.StartDate;
                            itm.EndDate = pm.EndDate;
                            itm.StartDay = pm.StartDay;
                            itm.EndDay = pm.EndDay;
                            itm.ModifiedBy = userId;
                            itm.DateModified = DateTime.Now;
                            mssvc.SaveOrUpdatePeriodMasterDetails(itm);
                        }
                    }
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        public ActionResult DeletePeriodMaster(string[] Id)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    int i;
                    string[] arrayId = Id[0].Split(',');
                    for (i = 0; i < arrayId.Length; i++)
                    {
                        var singleId = arrayId[i];
                        PeriodMaster pm = mssvc.GetPeriodDetailsById(Convert.ToInt64(singleId));
                        mssvc.GetDeletePeriodMasterById(pm);
                    }
                    var script = @"SucessMsg(""Deleted Sucessfully"");";
                    return JavaScript(script);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        #endregion

        #region New Calculating date
        public ActionResult CalculatingDate(string SrtDate)
        {
            IFormatProvider provider = new System.Globalization.CultureInfo("en-CA", true);
            DateTime startDate = DateTime.Parse(SrtDate, provider, System.Globalization.DateTimeStyles.NoCurrentDateDefault);
            DateTime endDate = startDate.AddDays(6);
            var exDate = endDate.ToString("dd/MM/yyyy");
            return Json(exDate, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region Master Week added by arun

        public JsonResult FillWeek(string Period, string PeriodYear)
        {
            try
            {
                string userid = (Session["UserId"] == null) ? "" : Session["UserId"].ToString();
                MastersService ms = new MastersService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                if (!string.IsNullOrWhiteSpace(Period)) { criteria.Add("Period", Period); criteria.Add("Year", PeriodYear); }
                Dictionary<long, IList<PeriodMaster>> PeriodMaster = ms.GetPeriodMasterListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                if (PeriodMaster != null && PeriodMaster.First().Value != null && PeriodMaster.First().Value.Count > 0)
                {
                    var BranchCodeList = (
                    from items in PeriodMaster.First().Value
                    select new
                    {
                        Text = items.Week,
                        Value = items.Week
                    }).Distinct().ToList().OrderBy(x => x.Text);
                    return Json(BranchCodeList, JsonRequestBehavior.AllowGet);
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

        #endregion

        public JsonResult FillPeriodYear()
        {
            try
            {
                string userid = (Session["UserId"] == null) ? "" : Session["UserId"].ToString();
                MastersService ms = new MastersService();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                //if (!string.IsNullOrWhiteSpace(Period)) { criteria.Add("Period", Period); }
                Dictionary<long, IList<PeriodMaster>> PeriodMaster = ms.GetPeriodMasterListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                if (PeriodMaster != null && PeriodMaster.First().Value != null && PeriodMaster.First().Value.Count > 0)
                {
                    var PeriodYearList = (
                    from items in PeriodMaster.First().Value
                    select new
                    {
                        Text = items.Year,
                        Value = items.Year
                    }).Distinct().ToList().OrderBy(x => x.Text);
                    return Json(PeriodYearList, JsonRequestBehavior.AllowGet);
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

        public ActionResult TransportPriceMaster()
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

        public ActionResult TransportPriceMasterJqgrid(string Sector, string ExptType, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    criteria.Clear();
                    sord = sord == "desc" ? "Desc" : "Asc";
                    if (!string.IsNullOrWhiteSpace(Sector)) { criteria.Add("Sector", Sector); }
                    Dictionary<long, IList<TransportPriceMaster>> transportPriceMaster = null;
                    if (!string.IsNullOrWhiteSpace(Sector))
                    {
                        transportPriceMaster = mssvc.GetTransportPriceMasterWithLikeSearchPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                    }
                    else
                    {
                        transportPriceMaster = mssvc.GetTransportPriceMasterWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                    }
                    if (transportPriceMaster != null && transportPriceMaster.FirstOrDefault().Value.Count > 0 && transportPriceMaster.FirstOrDefault().Key > 0)
                    {

                        if (ExptType == "Excel")
                        {
                            int i = 1;
                            foreach (var item in transportPriceMaster)
                            {
                                item.Value.FirstOrDefault().Id = i;
                                i = i + 1;
                            }
                            var List = transportPriceMaster.First().Value.ToList();
                            List<string> lstHeader = new List<string>() { "SNo", "Sector", "Sector Name", "Start Distance", "End Distance", " Surface Price", "Air Price" };
                            base.NewExportToExcel(List, "Trasnport Price Master", (items => new
                            {
                                items.Id,
                                items.Sector,
                                items.SectorName,
                                items.StartDistance,
                                items.EndDistance,
                                items.SurfacePrice,
                                items.AirPrice,

                            }), lstHeader);
                        }
                        else
                        {
                            long totalrecords = transportPriceMaster.First().Key;
                            int totalPages = (int)Math.Ceiling(totalrecords / (float)5);
                            var AssLst = new
                            {
                                total = totalPages,
                                page = page,
                                records = totalrecords,
                                rows = (
                                from items in transportPriceMaster.First().Value

                                select new
                                {
                                    cell = new string[]
                                {
                                items.Id.ToString(),
                                items.Sector,
                                items.SectorName,
                                items.StartDistance.ToString(),
                                items.EndDistance.ToString(),
                                items.SurfacePrice.ToString(),
                                items.AirPrice.ToString(),
                                items.CreatedDate.ToString()
                                }
                                }).ToList()
                            };

                            return Json(AssLst, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        var jsondat = new { rows = (new { cell = new string[] { } }) };
                        return Json(jsondat, JsonRequestBehavior.AllowGet);

                    }
                }
                return null;

            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }


        # region PONumber Master

        public ActionResult POMasters()
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
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }

        }

        public ActionResult AddPONumberMaster(POMasters pm)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    criteria.Clear();
                    Dictionary<long, IList<POMasters>> POList = mssvc.GetPOMastersListWithPagingAndCriteria(99999, 0, string.Empty, string.Empty, criteria);
                    //if (POList != null && POList.FirstOrDefault().Value.Count > 0 && POList.FirstOrDefault().Key > 0)
                    //{
                    //    foreach (var item in POList.First().Value)
                    //    {
                    //        if (item.PONumber == pm.PONumber)
                    //        {
                    //            var script = @"ErrMsg(""Already Exist"");";
                    //            return JavaScript(script);
                    //        }
                    //        else { }
                    //    }
                    //}
                    pm.CreatedDate = DateTime.Now;
                    pm.CreatedBy = userId;
                    pm.ModifiedDate = DateTime.Now;
                    pm.ModifiedBy = userId;
                    mssvc.SaveOrUpdatePoMasterDetails(pm);
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }

        //public ActionResult AddPONumberMaster(POMasters pm)
        //{
        //    try
        //    {
        //        string userId = base.ValidateUser();
        //        if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
        //        else
        //        {
        //            criteria.Clear();
        //            Dictionary<long, IList<POMasters>> POList = mssvc.GetPOMastersListWithPagingAndCriteria(99999, 0, string.Empty, string.Empty, criteria);
        //            if (POList != null && POList.FirstOrDefault().Value.Count > 0 && POList.FirstOrDefault().Key > 0)
        //            {
        //                foreach (var item in POList.First().Value)
        //                {
        //                    if (item.PONumber == pm.PONumber)
        //                    {
        //                        var script = @"ErrMsg(""Already Exist"");";
        //                        return JavaScript(script);
        //                    }
        //                    else { }
        //                }
        //            }
        //            pm.CreatedDate = DateTime.Now;
        //            pm.CreatedBy = userId;
        //            pm.ModifiedDate = DateTime.Now;
        //            pm.ModifiedBy = userId;
        //            mssvc.SaveOrUpdatePoMasterDetails(pm);
        //            return Json(null, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
        //        throw ex;
        //    }
        //}

        public ActionResult EditPONumberMaster(POMasters pm)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    if (pm.Id > 0)
                    {
                        POMasters pom = mssvc.GetPoMasterDetailsById(pm.Id);
                        if (pom.PONumber == pm.PONumber)
                        {
                            var script = @"ErrMsg(""Already Exist"");";
                            return JavaScript(script);
                        }
                        else
                        {
                            pom.POType = pm.POType;
                            pom.PONumber = pm.PONumber;
                            pom.Period = pm.Period;
                            pom.PeriodYear = pm.PeriodYear;
                            pom.ModifiedBy = userId;
                            pom.ModifiedDate = DateTime.Now;
                            mssvc.SaveOrUpdatePoMasterDetails(pom);
                        }
                    }
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }

        public ActionResult DeletePONumberMaster(string[] Id)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    int i;
                    string[] arrayId = Id[0].Split(',');
                    for (i = 0; i < arrayId.Length; i++)
                    {
                        var singleId = arrayId[i];
                        POMasters pm = mssvc.GetPoMasterDetailsById(Convert.ToInt64(singleId));
                        mssvc.GetDeletePoMasterById(pm);
                    }
                    var script = @"SucessMsg(""Deleted Sucessfully"");";
                    return JavaScript(script);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }

        public ActionResult POMasterJqGrid(string POType, string PONumber, string Period, string PeriodYear, string ExptType, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {

                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    criteria.Clear();
                    sord = sord == "desc" ? "Desc" : "Asc";
                    if (!string.IsNullOrWhiteSpace(POType)) { criteria.Add("POType", POType); }
                    if (!string.IsNullOrWhiteSpace(PONumber)) { criteria.Add("PONumber", PONumber); }
                    if (!string.IsNullOrWhiteSpace(Period)) { criteria.Add("Period", Period); }
                    if (!string.IsNullOrWhiteSpace(PeriodYear)) { criteria.Add("PeriodYear", PeriodYear); }
                    Dictionary<long, IList<POMasters>> POList = null;
                    if ((!string.IsNullOrWhiteSpace(POType)) || (!string.IsNullOrWhiteSpace(PONumber)) || (!string.IsNullOrWhiteSpace(Period)) || (!string.IsNullOrWhiteSpace(PeriodYear)))
                    {
                        POList = mssvc.GetPOMastersListWithLikeSearchPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                    }
                    else
                    {
                        POList = mssvc.GetPOMastersListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                    }
                    if (POList != null && POList.FirstOrDefault().Value.Count > 0 && POList.FirstOrDefault().Key > 0)
                    {
                        if (ExptType == "Excel")
                        {
                            int i = 1;
                            foreach (var item in POList)
                            {
                                item.Value.FirstOrDefault().Id = i;
                                i = i + 1;
                            }
                            var List = POList.First().Value.ToList();
                            List<string> lstHeader = new List<string>() { "SNo", "POType", "PONumber", "Period", "Period Year" };
                            base.NewExportToExcel(List, "PO Master", (items => new
                            {
                                items.Id,
                                items.POType,
                                items.PONumber,
                                items.Period,
                                items.PeriodYear,

                            }), lstHeader);
                        }
                        else
                        {
                            long totalrecords = POList.First().Key;
                            int totalPages = (int)Math.Ceiling(totalrecords / (float)5);
                            var AssLst = new
                            {
                                total = totalPages,
                                page = page,
                                records = totalrecords,
                                rows = (
                                     from items in POList.First().Value

                                     select new
                                     {
                                         cell = new string[] 
                                         {
                                             items.Id.ToString(),
                                             items.POType,
                                             items.PONumber,
                                             items.Period,
                                             items.PeriodYear,
                                             items.CreatedBy,
                                             items.CreatedDate.ToString()
                                         }
                                     }).ToList()
                            };

                            return Json(AssLst, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        var jsondat = new { rows = (new { cell = new string[] { } }) };
                        return Json(jsondat, JsonRequestBehavior.AllowGet);

                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }

        #endregion


        public JsonResult FillMailPeriod()
        {
            try
            {
                string userid = (Session["UserId"] == null) ? "" : Session["UserId"].ToString();
                Dictionary<long, IList<MailPeriodMaster>> PeriodMaster = ES.GetMailPeriodMasterListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, null);
                if (PeriodMaster != null && PeriodMaster.First().Value != null && PeriodMaster.First().Value.Count > 0)
                {
                    var PeriodList = (
                             from items in PeriodMaster.First().Value
                             select new
                             {
                                 Text = items.MailPeriod,
                                 Value = items.MailPeriod
                             }).Distinct().ToList().OrderBy(x => x.Text);
                    return Json(PeriodList, JsonRequestBehavior.AllowGet);
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

        public JsonResult FillMailTemplate()
        {
            try
            {
                string userid = (Session["UserId"] == null) ? "" : Session["UserId"].ToString();
                Dictionary<long, IList<MailTemplateMaster>> TemplateMaster = ES.GetMailTemplateMasterListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, null);
                if (TemplateMaster != null && TemplateMaster.First().Value != null && TemplateMaster.First().Value.Count > 0)
                {
                    var PeriodList = (
                             from items in TemplateMaster.First().Value
                             select new
                             {
                                 Text = items.MailTemplate,
                                 Value = items.MailTemplateMasterId
                             }).Distinct().ToList().OrderBy(x => x.Text);
                    return Json(PeriodList, JsonRequestBehavior.AllowGet);
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

        #region UNFoodItem Detailed Master
        public ActionResult FoodItemsDetailedPrice()
        {

            return View();
        }

        public ActionResult UNDetailedItemMasterPriceJqgrid(UNSectorDetailedItemPrice objUNSectorDetailedItemPrice, string ExptType, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    criteria.Clear();
                    sord = sord == "desc" ? "Desc" : "Asc";
                    if (!string.IsNullOrWhiteSpace(objUNSectorDetailedItemPrice.UNCode.ToString()) && objUNSectorDetailedItemPrice.UNCode != 0) { criteria.Add("UNCode", objUNSectorDetailedItemPrice.UNCode); }
                    if (!string.IsNullOrWhiteSpace(objUNSectorDetailedItemPrice.Commodity)) { criteria.Add("Commodity", objUNSectorDetailedItemPrice.Commodity); }
                    if (!string.IsNullOrWhiteSpace(objUNSectorDetailedItemPrice.SectorCode)) { criteria.Add("SectorCode", objUNSectorDetailedItemPrice.SectorCode); }

                    Dictionary<long, IList<UNSectorDetailedItemPrice>> UNSectorMappingList = null;
                    UNSectorMappingList = mssvc.GetUNSectorDetailedItemPriceListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                    if (UNSectorMappingList != null && UNSectorMappingList.FirstOrDefault().Value.Count > 0 && UNSectorMappingList.FirstOrDefault().Key > 0)
                    {

                        if (ExptType == "Excel")
                        {
                            int i = 1;
                            foreach (var item in UNSectorMappingList.FirstOrDefault().Value)
                            {
                                item.Id = i;
                                i = i + 1;
                            }
                            var List = UNSectorMappingList.First().Value.ToList();
                            List<string> lstHeader = new List<string>() { "SNo", "UNCode", "Commodity", "SectorCode", "UnitPrice_Halal", " UnitPrice_NonHalal", "Transportation Cost", "Insurance Cost", "Surfacedeliveries_Halal ", "Surfacedeliveries_NonHalal", "AirCostfromRegional ", "AirCostfromOriginal ", "AirCostfromKhartoum ", " Warehouse" };
                            base.NewExportToExcel(List, "Food Item Detailed Price Master", (items => new
                            {
                                items.Id,
                                items.UNCode,
                                items.Commodity,
                                items.SectorCode,
                                items.UnitPrice_Halal,
                                items.UnitPrice_NonHalal,

                                items.TransportationCost,
                                items.InsuranceCost,
                                items.Surfacedeliveries_Halal,
                                items.Surfacedeliveries_NonHalal,
                                items.AirCostfromRegional,
                                items.AirCostfromOriginal,
                                items.AirCostfromKhartoum,
                                items.Warehouse
                            }), lstHeader);
                        }
                        else
                        {
                            long totalrecords = UNSectorMappingList.First().Key;
                            int totalPages = (int)Math.Ceiling(totalrecords / (float)5);
                            var AssLst = new
                            {
                                total = totalPages,
                                page = page,
                                records = totalrecords,
                                rows = (
                                     from items in UNSectorMappingList.First().Value

                                     select new
                                     {
                                         cell = new string[] 
                                         {
                                             items.Id.ToString(),
                                             items.UNCode.ToString(),
                                             items.Commodity,
                                             items.SectorCode,
                                             items.UnitPrice_Halal!=null?items.UnitPrice_Halal.ToString():"0.00",
                                             items.UnitPrice_NonHalal!=null?items.UnitPrice_NonHalal.ToString():"0.00",
                                             
                                             items.TransportationCost!=null?items.TransportationCost.ToString():"0.00",
                                             items.InsuranceCost!=null?items.InsuranceCost.ToString():"0.00",
                                             items.Surfacedeliveries_Halal!=null?items.Surfacedeliveries_Halal.ToString():"0.00",
                                             items.Surfacedeliveries_NonHalal!=null?items.Surfacedeliveries_NonHalal.ToString():"0.00",
                                             items.AirCostfromRegional!=null?items.AirCostfromRegional.ToString():"0.00",
                                             items.AirCostfromOriginal!=null?items.AirCostfromOriginal.ToString():"0.00",
                                             items.AirCostfromKhartoum!=null?items.AirCostfromKhartoum.ToString():"0.00",
                                             items.Warehouse!=null?items.Warehouse.ToString():""
                                             

                                         }
                                     }).ToList()
                            };

                            return Json(AssLst, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        var jsondat = new { rows = (new { cell = new string[] { } }) };
                        return Json(jsondat, JsonRequestBehavior.AllowGet);

                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }

        public ActionResult AddUNDetailedItemMasterPrice(UNSectorDetailedItemPrice objUNSectorDetailedItemPrice)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {

                    if (objUNSectorDetailedItemPrice.Id == 0)
                    {

                        objUNSectorDetailedItemPrice.CreatedBy = userId;
                        objUNSectorDetailedItemPrice.CreatedDate = DateTime.Now;
                        mssvc.SaveOrUpdateUNDetailedItemMasterPrice(objUNSectorDetailedItemPrice);
                        return Json(null, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        UNSectorDetailedItemPrice obj = mssvc.GetNDetailedItemMasterPriceById(objUNSectorDetailedItemPrice.Id);


                        obj.UNCode = objUNSectorDetailedItemPrice.UNCode;
                        obj.Commodity = objUNSectorDetailedItemPrice.Commodity;
                        obj.SectorCode = objUNSectorDetailedItemPrice.SectorCode;
                        obj.UnitPrice_Halal = objUNSectorDetailedItemPrice.UnitPrice_Halal;
                        obj.UnitPrice_NonHalal = objUNSectorDetailedItemPrice.UnitPrice_NonHalal;
                        obj.TransportationCost = objUNSectorDetailedItemPrice.TransportationCost;
                        obj.InsuranceCost = objUNSectorDetailedItemPrice.InsuranceCost;
                        obj.Surfacedeliveries_Halal = objUNSectorDetailedItemPrice.Surfacedeliveries_Halal;
                        obj.Surfacedeliveries_NonHalal = objUNSectorDetailedItemPrice.Surfacedeliveries_NonHalal;
                        obj.AirCostfromRegional = objUNSectorDetailedItemPrice.AirCostfromRegional;
                        obj.AirCostfromOriginal = objUNSectorDetailedItemPrice.AirCostfromOriginal;
                        obj.AirCostfromKhartoum = objUNSectorDetailedItemPrice.AirCostfromKhartoum;
                        obj.Warehouse = objUNSectorDetailedItemPrice.Warehouse;

                        obj.ModifiedBy = userId;
                        obj.ModifiedDate = DateTime.Now;



                        mssvc.SaveOrUpdateUNDetailedItemMasterPrice(obj);
                        return Json(null, JsonRequestBehavior.AllowGet);
                    }
                    //criteria.Clear();
                    //criteria.Add("UNCode", objUNSectorDetailedItemPrice.UNCode);
                    //criteria.Add("Commodity", objUNSectorDetailedItemPrice.Commodity);
                    //criteria.Add("SectorCode", objUNSectorDetailedItemPrice.SectorCode);

                    //Dictionary<long, IList<UNSectorDetailedItemPrice>> detailedprice = mssvc.GetUNSectorDetailedItemPriceListWithPagingAndCriteria(0, 1000, string.Empty, string.Empty, criteria);
                    //if (detailedprice != null && detailedprice.FirstOrDefault().Value.Count > 0 && detailedprice.FirstOrDefault().Key > 0)
                    //{
                    //    objUNSectorDetailedItemPrice.ModifiedBy = userId;
                    //    objUNSectorDetailedItemPrice.ModifiedDate = DateTime.Now;


                    //    mssvc.SaveOrUpdateUNDetailedItemMasterPrice(objUNSectorDetailedItemPrice);
                    //    return Json(null, JsonRequestBehavior.AllowGet);
                    //    //var script = @"ErrMsg(""Already Exist"");";
                    //    //return JavaScript(script);
                    //}
                    //objUNSectorDetailedItemPrice.CreatedBy = userId;
                    //objUNSectorDetailedItemPrice.CreatedDate = DateTime.Now;


                    //mssvc.SaveOrUpdateUNDetailedItemMasterPrice(objUNSectorDetailedItemPrice);
                    //return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }

        }

        public ActionResult DeleteUNDetailedItemMasterPrice(UNSectorDetailedItemPrice objUNSectorDetailedItemPrice)
        {
            try
            {

                mssvc.DeleteUNDetailedItemMasterPrice(objUNSectorDetailedItemPrice);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }






        #endregion
        #region Inventory Management Masters
        public ActionResult currencyddl()
        {
            try
            {
                string userId = ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    Dictionary<long, string> appcd = new Dictionary<long, string>();
                    Dictionary<string, object> criteria = new Dictionary<string, object>();
                    criteria.Clear();
                    Dictionary<long, IList<Inventory_CurrencyMaster>> currencyType = mssvc.GetCurrencyMasterListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, null);
                    foreach (Inventory_CurrencyMaster curr in currencyType.First().Value)
                    {
                        appcd.Add(curr.Currency_Id, curr.Currency);
                    }
                    return PartialView("SelectPartial", appcd);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        public ActionResult CurrencyMaster()
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
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        public ActionResult CurrencyMasterJqGrid(Inventory_CurrencyMaster currencyMaster, string ExptType, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    criteria.Clear();
                    sord = sord == "desc" ? "Desc" : "Asc";
                    if (currencyMaster != null)
                    {
                        if (!string.IsNullOrWhiteSpace(currencyMaster.Currency)) { criteria.Add("Currency", currencyMaster.Currency); }
                    }
                    Dictionary<long, IList<Inventory_CurrencyMaster>> currencyMasterList = null;
                    currencyMasterList = mssvc.GetCurrencyMasterListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                    if (currencyMasterList != null && currencyMasterList.FirstOrDefault().Value.Count > 0 && currencyMasterList.FirstOrDefault().Key > 0)
                    {
                        if (ExptType == "Excel")
                        {
                            int i = 1;
                            foreach (var item in currencyMasterList.FirstOrDefault().Value)
                            {
                                item.Currency_Id = i;
                                i = i + 1;
                            }
                            var List = currencyMasterList.First().Value.ToList();
                            List<string> lstHeader = new List<string>() { "SNo", "Currency", "Description", "Created By", "Created Date" };
                            base.NewExportToExcel(List, "Currency Master", (items => new
                            {
                                items.Currency_Id,
                                items.Currency,
                                items.Description,
                                items.CreatedBy,
                                items.CreatedDate,
                            }), lstHeader);
                        }
                        else
                        {
                            long totalrecords = currencyMasterList.First().Key;
                            int totalPages = (int)Math.Ceiling(totalrecords / (float)5);
                            var AssLst = new
                            {
                                total = totalPages,
                                page = page,
                                records = totalrecords,
                                rows = (
                                     from items in currencyMasterList.First().Value

                                     select new
                                     {
                                         cell = new string[] 
                                         {
                                             items.Currency_Id.ToString(),
                                             items.Currency,
                                             items.Description,
                                             items.CreatedBy,
                                             items.CreatedDate.Value.ToString("dd/MM/yyyy")
                                         }
                                     }).ToList()
                            };
                            return Json(AssLst, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        var jsondat = new { rows = (new { cell = new string[] { } }) };
                        return Json(jsondat, JsonRequestBehavior.AllowGet);

                    }
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        public ActionResult AddCurrencyMaster(Inventory_CurrencyMaster currencyMaster)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    var script = string.Empty;
                    string strCurrency = string.Empty;
                    criteria.Clear();
                    if (currencyMaster != null && !string.IsNullOrEmpty(currencyMaster.Currency))
                    {
                        strCurrency = currencyMaster.Currency.ToUpper().Replace(" ", "");
                        criteria.Add("Currency", strCurrency);
                    }
                    Dictionary<long, IList<Inventory_CurrencyMaster>> currencyMasterList = mssvc.GetCurrencyMasterListWithPagingAndCriteria(0, 99999, string.Empty, string.Empty, criteria);
                    if (currencyMasterList.FirstOrDefault().Key == 0)
                    {
                        currencyMaster.Currency = strCurrency;
                        currencyMaster.CreatedDate = DateTime.Now;
                        currencyMaster.CreatedBy = userId;
                        mssvc.SaveOrUpdateCurrencyMasterDetails(currencyMaster);
                        script = @"SucessMsg(""Added Successfully"");";
                        return JavaScript(script);
                    }
                    script = @"ErrMsg(""Already Exist"");";
                    return JavaScript(script);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        public ActionResult EditCurrencyMaster(Inventory_CurrencyMaster currencyMaster)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    var script = string.Empty;
                    string strCurrency = string.Empty;
                    criteria.Clear();
                    if (currencyMaster != null && !string.IsNullOrEmpty(currencyMaster.Currency))
                    {
                        strCurrency = currencyMaster.Currency.ToUpper().Replace(" ", "");
                        criteria.Add("Currency", strCurrency);
                    }
                    Dictionary<long, IList<Inventory_CurrencyMaster>> currencyMasterList = mssvc.GetCurrencyMasterListWithPagingAndCriteria(0, 99999, string.Empty, string.Empty, criteria);
                    if (currencyMasterList.FirstOrDefault().Key == 0)
                    {
                        if (currencyMaster.Currency_Id > 0)
                        {
                            Inventory_CurrencyMaster currObj = mssvc.GetCurrencyDetailsById(currencyMaster.Currency_Id);
                            currObj.Currency = strCurrency;
                            currObj.Description = currencyMaster.Description;
                            currObj.ModifiedBy = userId;
                            currObj.ModifiedDate = DateTime.Now;
                            mssvc.SaveOrUpdateCurrencyMasterDetails(currObj);
                            script = @"SucessMsg(""Updated Successfully"");";
                            return JavaScript(script);
                        }
                    }
                    else
                    {
                        script = @"ErrMsg(""Already Exist"");";
                        return JavaScript(script);
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        public ActionResult DeleteCurrencyMaster(string Id)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    string[] arrayId = Id.Split(',');
                    long[] CurrencyIdArr = Array.ConvertAll(arrayId, Int64.Parse);
                    mssvc.DeleteCurrencyMasterById(CurrencyIdArr);
                    var script = @"SucessMsg(""Deleted  Successfully"");";
                    return JavaScript(script);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        public ActionResult ExchangeRateMaster()
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
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        public ActionResult ExchangeRateMasterJqGrid(Inventory_ExchangeRateMaster exchangeRateMaster, string ExptType, int rows, string sidx, string sord, int? page = 1)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    criteria.Clear();
                    sord = sord == "desc" ? "Desc" : "Asc";
                    if (exchangeRateMaster != null)
                    {
                        // if (!string.IsNullOrWhiteSpace(exchangeRateMaster.Inventory_CurrencyMaster.Currency_Id)) { criteria.Add("Currency", currencyMaster.Currency); }
                    }
                    Dictionary<long, IList<Inventory_ExchangeRateMaster>> exchangeRateMasterList = null;
                    exchangeRateMasterList = mssvc.GetExchangeRateMasterListWithPagingAndCriteria(page - 1, rows, sidx, sord, criteria);
                    if (exchangeRateMasterList != null && exchangeRateMasterList.FirstOrDefault().Value.Count > 0 && exchangeRateMasterList.FirstOrDefault().Key > 0)
                    {
                        if (ExptType == "Excel")
                        {
                            int i = 1;
                            foreach (var item in exchangeRateMasterList.FirstOrDefault().Value)
                            {
                                item.Rate_Id = i;
                                i = i + 1;
                            }
                            var List = exchangeRateMasterList.First().Value.ToList();
                            List<string> lstHeader = new List<string>() { "SNo", "Currency", "ExchangeRate", "Created By", "Created Date" };
                            base.NewExportToExcel(List, "Exchange Rate Master", (items => new
                            {
                                items.Rate_Id,
                                items.Inventory_CurrencyMaster.Currency,
                                items.ExchangeRate,
                                items.CreatedBy,
                                items.CreatedDate,
                            }), lstHeader);
                        }
                        else
                        {
                            long totalrecords = exchangeRateMasterList.First().Key;
                            int totalPages = (int)Math.Ceiling(totalrecords / (float)5);
                            var AssLst = new
                            {
                                total = totalPages,
                                page = page,
                                records = totalrecords,
                                rows = (
                                     from items in exchangeRateMasterList.First().Value

                                     select new
                                     {
                                         cell = new string[] 
                                         {
                                             items.Rate_Id.ToString(),
                                             items.Inventory_CurrencyMaster.Currency,
                                             items.ExchangeRate.ToString(),
                                             items.CreatedBy,
                                             items.CreatedDate.Value.ToString("dd/MM/yyyy")
                                         }
                                     }).ToList()
                            };
                            return Json(AssLst, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        var jsondat = new { rows = (new { cell = new string[] { } }) };
                        return Json(jsondat, JsonRequestBehavior.AllowGet);

                    }
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        public ActionResult AddExchangeRateMaster(Inventory_ExchangeRateMaster rateMaster)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    var script = string.Empty;
                    criteria.Clear();
                    if (rateMaster != null && rateMaster.Inventory_CurrencyMaster != null)
                    {
                        if (rateMaster.Inventory_CurrencyMaster.Currency_Id > 0)
                            criteria.Add("Inventory_CurrencyMaster.Currency_Id", rateMaster.Inventory_CurrencyMaster.Currency_Id);
                        //if(rateMaster.ExchangeRate>0)
                        //    criteria.Add("ExchangeRate", rateMaster.ExchangeRate);
                        criteria.Add("GLDate", DateTime.Now.ToString("dd/MM/yyyy"));
                    }
                    Dictionary<long, IList<Inventory_ExchangeRateMaster>> rateMasterList = mssvc.GetExchangeRateMasterListWithPagingAndCriteria(0, 99999, string.Empty, string.Empty, criteria);
                    if (rateMasterList.FirstOrDefault().Key == 0)
                    {
                        rateMaster.GLDate = DateTime.Now.ToString("dd/MM/yyyy");
                        rateMaster.CreatedDate = DateTime.Now;
                        rateMaster.CreatedBy = userId;
                        mssvc.SaveOrUpdateExchangeRateMasterDetails(rateMaster);
                        script = @"SucessMsg(""Added Successfully"");";
                        return JavaScript(script);
                    }
                    script = @"ErrMsg(""Already Exist"");";
                    return JavaScript(script);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        public ActionResult EditExchangeRateMaster(Inventory_ExchangeRateMaster rateMaster)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    var script = string.Empty;
                    criteria.Clear();
                    if (rateMaster != null && rateMaster.Inventory_CurrencyMaster != null)
                    {
                        if (rateMaster.Inventory_CurrencyMaster.Currency_Id > 0)
                            criteria.Add("Inventory_CurrencyMaster.Currency_Id", rateMaster.Inventory_CurrencyMaster.Currency_Id);
                        if (rateMaster.ExchangeRate > 0)
                            criteria.Add("ExchangeRate", rateMaster.ExchangeRate);
                        criteria.Add("GLDate", DateTime.Now.ToString("dd/MM/yyyy"));
                    }
                    Dictionary<long, IList<Inventory_ExchangeRateMaster>> rateMasterList = mssvc.GetExchangeRateMasterListWithPagingAndCriteria(0, 99999, string.Empty, string.Empty, criteria);
                    if (rateMasterList.FirstOrDefault().Key == 0)
                    {
                        if (rateMaster.Rate_Id > 0)
                        {
                            Inventory_ExchangeRateMaster rateObj = mssvc.GetExchangeRateDetailsById(rateMaster.Rate_Id);
                            rateObj.ExchangeRate = rateMaster.ExchangeRate;
                            rateObj.ModifiedBy = userId;
                            rateObj.ModifiedDate = DateTime.Now;
                            mssvc.SaveOrUpdateExchangeRateMasterDetails(rateObj);
                            script = @"SucessMsg(""Updated Successfully"");";
                            return JavaScript(script);
                        }
                    }
                    else
                    {
                        script = @"ErrMsg(""Already Exist"");";
                        return JavaScript(script);
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        public ActionResult DeleteExchangeRateMaster(string Id)
        {
            try
            {
                string userId = base.ValidateUser();
                if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("LogOn", "Account");
                else
                {
                    string[] arrayId = Id.Split(',');
                    long[] rateIdArr = Array.ConvertAll(arrayId, Int64.Parse);
                    mssvc.DeleteExchangeRateMasterById(rateIdArr);
                    var script = @"SucessMsg(""Deleted  Successfully"");";
                    return JavaScript(script);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "InsightMasterPolicy");
                throw ex;
            }
        }
        #endregion
    }
}
