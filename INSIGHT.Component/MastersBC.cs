using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistenceFactory;
using INSIGHT.Entities;
using System.Collections;
using INSIGHT.Entities.InvoiceEntities;
using INSIGHT.Entities.Masters;



namespace INSIGHT.Component
{
    public class MastersBC
    {
        PersistenceServiceFactory PSF = null;
        public MastersBC()
        {
            List<String> Assembly = new List<String>();
            Assembly.Add("INSIGHT.Entities");
            Assembly.Add("INSIGHT.Entities.TicketingSystem");
            PSF = new PersistenceServiceFactory(Assembly);
        }
        #region"Menu"
        public Dictionary<long, IList<Menu>> GetMenuListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithExactSearchCriteriaCount<Menu>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public long SaveOrUpdateMenuDetails(Menu mu)
        {
            try
            {
                if (mu != null)
                    PSF.SaveOrUpdate<Menu>(mu);
                else { throw new Exception("Webtemplate is required and it cannot be null.."); }
                return mu.Id;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Menu GetDeleteMenurowById(long Id)
        {
            try
            {
                Menu mu = null;

                if (Id > 0)
                    mu = PSF.Get<Menu>(Id);
                else { throw new Exception("Id is required and it cannot be 0"); }
                return mu;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public long DeleteMenufunction(Menu mu)
        {
            try
            {
                if (mu != null)
                    PSF.Delete<Menu>(mu);
                else { throw new Exception("Value is required and it cannot be null.."); }
                return 0;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public long SaveOrUpdateSubMenuDetails(Menu mu)
        {
            try
            {
                if (mu != null)
                    PSF.SaveOrUpdate<Menu>(mu);
                else { throw new Exception("Webtemplate is required and it cannot be null.."); }
                return mu.Id;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Menu DeleteSubMenurowById(long Id)
        {
            try
            {
                Menu mu = null;
                if (Id > 0)
                    mu = PSF.Get<Menu>(Id);
                else { throw new Exception("Id is required and it cannot be 0"); }
                return mu;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion"Menu"
        #region GCC Masters Added By Micheal
        //Modified by Gobi
        public Dictionary<long, IList<ItemMaster>> GetItemMasterListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<ItemMaster>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Added by Gobi
        public Dictionary<long, IList<ItemMaster>> GetItemMasterListWithLikeSearchPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithLikeSearchCriteriaCount<ItemMaster>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }




        //Modified by Gobi
        public Dictionary<long, IList<ContingentMaster>> GetContigentListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<ContingentMaster>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Added by Gobi
        public Dictionary<long, IList<ContingentMaster>> GetContingentMasterListWithLikeSearchPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithLikeSearchCriteriaCount<ContingentMaster>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Sector Master
        //Modified By Gobi
        public Dictionary<long, IList<SectorMaster>> GetSectorMasterListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<SectorMaster>(page, pageSize, sortby, sortType, criteria);
                //return PSF.GetListWithExactSearchCriteriaCount<SectorMaster>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Added by Gobi
        public Dictionary<long, IList<SectorMaster>> GetSectorMasterListWithLikeSearchPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithLikeSearchCriteriaCount<SectorMaster>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        public long SaveOrUpdateContingentDetails(ContingentMaster cntm)
        {
            try
            {
                if (cntm != null)
                    PSF.SaveOrUpdate<ContingentMaster>(cntm);
                else { throw new Exception("All Fields are required and it cannot be null.."); }
                return cntm.Id;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public ContingentMaster GetContingentDetailsById(long Id)
        {
            try
            {
                ContingentMaster cntm = null;

                if (Id > 0)
                    cntm = PSF.Get<ContingentMaster>(Id);
                else { throw new Exception("Id is required and it cannot be 0"); }
                return cntm;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public long GetDeleteContingentrowById(ContingentMaster cntm)
        {
            try
            {
                if (cntm != null)
                    PSF.Delete<ContingentMaster>(cntm);
                else { throw new Exception("Value is required and it cannot be null.."); }
                return 0;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public long SaveOrUpdateItemMasterDetails(ItemMaster im)
        {
            try
            {
                if (im != null)
                    PSF.SaveOrUpdate<ItemMaster>(im);
                else { throw new Exception("All Fields are required and it cannot be null.."); }
                return im.Id;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public ItemMaster GetItemDetailsById(long Id)
        {
            try
            {
                ItemMaster cntm = null;

                if (Id > 0)
                    cntm = PSF.Get<ItemMaster>(Id);
                else { throw new Exception("Id is required and it cannot be 0"); }
                return cntm;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public long GetDeleteItemrowById(ItemMaster im)
        {
            try
            {
                if (im != null)
                    PSF.Delete<ItemMaster>(im);
                else { throw new Exception("Value is required and it cannot be null.."); }
                return 0;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public long SaveOrUpdateSectorMasterDetails(SectorMaster im)
        {
            try
            {
                if (im != null)
                    PSF.SaveOrUpdate<SectorMaster>(im);
                else { throw new Exception("All Fields are required and it cannot be null.."); }
                return im.Id;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public SectorMaster GetSectorDetailsById(long Id)
        {
            try
            {
                SectorMaster secm = null;

                if (Id > 0)
                    secm = PSF.Get<SectorMaster>(Id);
                else { throw new Exception("Id is required and it cannot be 0"); }
                return secm;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public long GetDeleteSectorrowById(SectorMaster im)
        {
            try
            {
                if (im != null)
                    PSF.Delete<SectorMaster>(im);
                else { throw new Exception("Value is required and it cannot be null.."); }
                return 0;
            }
            catch (Exception)
            {

                throw;
            }
        }

        #region Period Master
        //Modified by Gobi
        public Dictionary<long, IList<PeriodMaster>> GetPeriodMasterListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<PeriodMaster>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Added by Gobi
        public Dictionary<long, IList<PeriodMaster>> GetPeriodMasterListWithLikeSearchPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithLikeSearchCriteriaCount<PeriodMaster>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Location Master
        //Modified by Gobi
        public Dictionary<long, IList<LocationMaster>> GetLocationMasterListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<LocationMaster>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Added by Gobi
        public Dictionary<long, IList<LocationMaster>> GetLocationMasterListWithLikeSearchPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithLikeSearchCriteriaCount<LocationMaster>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Country Master
        //Modified by Gobi
        public Dictionary<long, IList<CountryMaster>> GetCountryMasterListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<CountryMaster>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Added by Gobi
        public Dictionary<long, IList<CountryMaster>> GetCountryMasterListWithLikeSearchPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithLikeSearchCriteriaCount<CountryMaster>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region UNSectorMapping Master
        //Modified by Gobi
        public Dictionary<long, IList<UNSectorConMapping>> GetUNSectorMappingMasterListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<UNSectorConMapping>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Added by Gobi
        public Dictionary<long, IList<UNSectorConMapping>> GetUNSectorMappingMasterListWithLikeSearchPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithLikeSearchCriteriaCount<UNSectorConMapping>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        public Dictionary<long, IList<UserTypeMaster>> GetUserTypeMasterListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithExactSearchCriteriaCount<UserTypeMaster>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public long CreateOrUpdateDocumentUpload(DocumentUpload doc)
        {
            try
            {
                if (doc != null)
                    PSF.SaveOrUpdate<DocumentUpload>(doc);
                else { throw new Exception("Document is required and it cannot be null.."); }
                return doc.Id;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region GCC Masters Added By Gobi

        #region Location Master
        public long SaveOrUpdateLocationMasterDetails(LocationMaster lm)
        {
            try
            {
                if (lm != null)
                    PSF.SaveOrUpdate<LocationMaster>(lm);
                else { throw new Exception("All Fields are required and it cannot be null.."); }
                return lm.Id;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public LocationMaster GetLocationDetailsById(long Id)
        {
            try
            {
                LocationMaster LocationDetails = null;

                if (Id > 0)
                    LocationDetails = PSF.Get<LocationMaster>(Id);
                else { throw new Exception("Id is required and it cannot be 0"); }
                return LocationDetails;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public long GetDeleteLocationrowById(LocationMaster lm)
        {
            try
            {
                if (lm != null)
                    PSF.Delete<LocationMaster>(lm);
                else { throw new Exception("Value is required and it cannot be null.."); }
                return 0;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Country Master
        public long SaveOrUpdateCountryMasterDetails(CountryMaster cm)
        {
            try
            {
                if (cm != null)
                    PSF.SaveOrUpdate<CountryMaster>(cm);
                else { throw new Exception("All Fields are required and it cannot be null.."); }
                return cm.Id;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public CountryMaster GetCountryDetailsById(long Id)
        {
            try
            {
                CountryMaster CountryDetails = null;

                if (Id > 0)
                    CountryDetails = PSF.Get<CountryMaster>(Id);
                else { throw new Exception("Id is required and it cannot be 0"); }
                return CountryDetails;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public long GetDeleteCountryrowById(CountryMaster cm)
        {
            try
            {
                if (cm != null)
                    PSF.Delete<CountryMaster>(cm);
                else { throw new Exception("Value is required and it cannot be null.."); }
                return 0;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region UNSectorMappping Master
        public long SaveOrUpdateUNSectorMapppingMasterDetails(UNSectorConMapping unsm)
        {
            try
            {
                if (unsm != null)
                    PSF.SaveOrUpdate<UNSectorConMapping>(unsm);
                else { throw new Exception("All Fields are required and it cannot be null.."); }
                return unsm.Id;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public UNSectorConMapping GetUNSectorMapppingDetailsById(long Id)
        {
            try
            {
                UNSectorConMapping UNSectorMapppingDetails = null;

                if (Id > 0)
                    UNSectorMapppingDetails = PSF.Get<UNSectorConMapping>(Id);
                else { throw new Exception("Id is required and it cannot be 0"); }
                return UNSectorMapppingDetails;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public long GetDeleteUNSectorMapppingrowById(UNSectorConMapping unsm)
        {
            try
            {
                if (unsm != null)
                    PSF.Delete<UNSectorConMapping>(unsm);
                else { throw new Exception("Value is required and it cannot be null.."); }
                return 0;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region CMR Master
        public long SaveOrUpdateCMRMasterDetails(CMRMaster cmr)
        {
            try
            {
                if (cmr != null)
                    PSF.SaveOrUpdate<CMRMaster>(cmr);
                else { throw new Exception("All Fields are required and it cannot be null.."); }
                return cmr.Id;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public CMRMaster GetCMRDetailsById(long Id)
        {
            try
            {
                CMRMaster CMRDetails = null;

                if (Id > 0)
                    CMRDetails = PSF.Get<CMRMaster>(Id);
                else { throw new Exception("Id is required and it cannot be 0"); }
                return CMRDetails;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public long GetDeleteCMRrowById(CMRMaster cmr)
        {
            try
            {
                if (cmr != null)
                    PSF.Delete<CMRMaster>(cmr);
                else { throw new Exception("Value is required and it cannot be null.."); }
                return 0;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Discrepancy Master
        public long SaveOrUpdateDiscrepancyMasterDetails(DiscrepancyMaster dm)
        {
            try
            {
                if (dm != null)
                    PSF.SaveOrUpdate<DiscrepancyMaster>(dm);
                else { throw new Exception("All Fields are required and it cannot be null.."); }
                return dm.Id;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public DiscrepancyMaster GetDiscrepancyDetailsById(long Id)
        {
            try
            {
                DiscrepancyMaster DiscrepancyDetails = null;

                if (Id > 0)
                    DiscrepancyDetails = PSF.Get<DiscrepancyMaster>(Id);
                else { throw new Exception("Id is required and it cannot be 0"); }
                return DiscrepancyDetails;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public long GetDeleteDiscrepancyrowById(DiscrepancyMaster dm)
        {
            try
            {
                if (dm != null)
                    PSF.Delete<DiscrepancyMaster>(dm);
                else { throw new Exception("Value is required and it cannot be null.."); }
                return 0;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion


        #endregion

        #region Excel upload
        public IList GetCounterValue(string TableName)
        {
            try
            {
                return PSF.GetCounterValue(TableName);
            }
            catch (Exception) { throw; }
            finally { if (PSF != null) PSF.Dispose(); }
        }
        #endregion
        #region New Masters
        public Dictionary<long, IList<UOMMaster>> GetUOMMasterListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithExactSearchCriteriaCount<UOMMaster>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Modified by Gobi
        public Dictionary<long, IList<CMRMaster>> GetCMRMasterListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<CMRMaster>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Added by Gobi
        public Dictionary<long, IList<CMRMaster>> GetCMRMasterListWithLikeSearchPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithLikeSearchCriteriaCount<CMRMaster>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }


        //public Dictionary<long, IList<DiscrepancyMaster>> GetDiscrepancyMasterListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        //{
        //    try
        //    {
        //        return PSF.GetListWithExactSearchCriteriaCount<DiscrepancyMaster>(page, pageSize, sortby, sortType, criteria);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        //Modified by Gobi

        public Dictionary<long, IList<DiscrepancyMaster>> GetDiscrepancyMasterListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<DiscrepancyMaster>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Added by Gobi
        public Dictionary<long, IList<DiscrepancyMaster>> GetDiscrepancyMasterListWithLikeSearchPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithLikeSearchCriteriaCount<DiscrepancyMaster>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        public CMRMaster GetCMRMasterBySectorCode(string sc)
        {
            try
            {
                return PSF.Get<CMRMaster>("SectorCode", sc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public long SaveOrUpdatePeriodMasterDetails(PeriodMaster im)
        {
            try
            {
                if (im != null)
                    PSF.SaveOrUpdate<PeriodMaster>(im);
                else { throw new Exception("All Fields are required and it cannot be null.."); }
                return im.Id;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public PeriodMaster GetPeriodDetailsById(long Id)
        {
            try
            {
                PeriodMaster cntm = null;

                if (Id > 0)
                    cntm = PSF.Get<PeriodMaster>(Id);
                else { throw new Exception("Id is required and it cannot be 0"); }
                return cntm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public long GetDeletePeriodMasterById(PeriodMaster im)
        {
            try
            {
                if (im != null)
                    PSF.Delete<PeriodMaster>(im);
                else { throw new Exception("Value is required and it cannot be null.."); }
                return 0;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        #region UOMMaster
        public bool DeleteUOMMastersDetails(long[] Id)
        {
            try
            {
                IList<UOMMaster> UOMMasterDetails = PSF.GetListByIds<UOMMaster>(Id);
                PSF.DeleteAll<UOMMaster>(UOMMasterDetails);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public UOMMaster GetUOMMasterDetailsById(long Id)
        {
            try
            {
                UOMMaster UOM = null;

                if (Id > 0)
                    UOM = PSF.Get<UOMMaster>(Id);
                else { throw new Exception("Id is required and it cannot be 0"); }
                return UOM;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public long SaveOrUpdateUOMMasterDetails(UOMMaster im)
        {
            try
            {
                if (im != null)
                    PSF.SaveOrUpdate<UOMMaster>(im);
                else { throw new Exception("All Fields are required and it cannot be null.."); }
                return im.Id;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
        //Added by kingston for substitute item  name auto complete
        public Dictionary<long, IList<ItemMaster>> GetItemMasterListWithPagingAndCriteriaForAutoComplete(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithLikeSearchCriteriaCount<ItemMaster>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Substitution master

        public Dictionary<long, IList<SubstitutionMaster>> GetISubstitutionMasterListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<SubstitutionMaster>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<long, IList<TransportPriceMaster>> GetTransportPriceMasterWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<TransportPriceMaster>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<long, IList<TransportPriceMaster>> GetTransportPriceMasterWithLikeSearchPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithLikeSearchCriteriaCount<TransportPriceMaster>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<long, IList<DocumentTypeMaster>> GetDocumentTypeMasterListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithExactSearchCriteriaCount<DocumentTypeMaster>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public long CreateOrUpdateEmailLog(EmailLog Mt)
        {
            try
            {

                if (Mt != null && Mt.Id > 0)
                {
                    PSF.Update<EmailLog>(Mt);
                }
                else
                {
                    PSF.Save<EmailLog>(Mt);
                }
                return Mt.Id;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region PONumber's
        public Dictionary<long, IList<POMasters>> GetPOMastersListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<POMasters>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<long, IList<POMasters>> GetPOMastersListWithLikeSearchPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithLikeSearchCriteriaCount<POMasters>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public long SaveOrUpdatePoMasterDetails(POMasters im)
        {
            try
            {
                if (im != null)
                    PSF.SaveOrUpdate<POMasters>(im);
                else { throw new Exception("All Fields are required and it cannot be null.."); }
                return im.Id;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public POMasters GetPoMasterDetailsById(long Id)
        {
            try
            {
                POMasters cntm = null;

                if (Id > 0)
                    cntm = PSF.Get<POMasters>(Id);
                else { throw new Exception("Id is required and it cannot be 0"); }
                return cntm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public long GetDeletePoMasterById(POMasters im)
        {
            try
            {
                if (im != null)
                    PSF.Delete<POMasters>(im);
                else { throw new Exception("Value is required and it cannot be null.."); }
                return 0;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Dictionary<long, IList<POTypeMaster>> GetPOTypeListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<POTypeMaster>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Get Ident_Current
        public long GetCurrentIntent(string table)
        {
            try
            {
                string query = "select IDENT_CURRENT('" + table + "')";
                IList list = PSF.ExecuteSql(query);
                if (list != null && list[0] != null)
                {
                    return Convert.ToInt64(list[0].ToString()) + 1; //list[0] = "0";
                }
                else return 0;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion end

        #region UNItemsDetailsedprice master

        public Dictionary<long, IList<UNSectorDetailedItemPrice>> GetUNSectorDetailedItemPriceListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithExactSearchCriteriaCount<UNSectorDetailedItemPrice>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public long SaveOrUpdateUNDetailedItemMasterPrice(UNSectorDetailedItemPrice im)
        {
            try
            {
                if (im != null)
                    PSF.SaveOrUpdate<UNSectorDetailedItemPrice>(im);
                else { throw new Exception("All Fields are required and it cannot be null.."); }
                return im.Id;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public void DeleteUNDetailedItemMasterPrice(UNSectorDetailedItemPrice im)
        {
            try
            {
                if (im != null)
                    PSF.Delete<UNSectorDetailedItemPrice>(im);
                else { throw new Exception("Value is required and it cannot be null.."); }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public UNSectorDetailedItemPrice GetNDetailedItemMasterPriceById(long Id)
        {
            try
            {
                UNSectorDetailedItemPrice cntm = null;

                if (Id > 0)
                    cntm = PSF.Get<UNSectorDetailedItemPrice>(Id);
                else { throw new Exception("Id is required and it cannot be 0"); }
                return cntm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
        #region Inventory Management
        public Dictionary<long, IList<Inventory_CurrencyMaster>> GetCurrencyMasterListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<Inventory_CurrencyMaster>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public long SaveOrUpdateCurrencyMasterDetails(Inventory_CurrencyMaster currencyMaster)
        {
            try
            {
                if (currencyMaster != null)
                    PSF.SaveOrUpdate<Inventory_CurrencyMaster>(currencyMaster);
                else { throw new Exception("All Fields are required and it cannot be null.."); }
                return currencyMaster.Currency_Id;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Inventory_CurrencyMaster GetCurrencyDetailsById(long Id)
        {
            try
            {
                Inventory_CurrencyMaster currencyMstr = null;
                if (Id > 0)
                    currencyMstr = PSF.Get<Inventory_CurrencyMaster>(Id);
                else { throw new Exception("Id is required and it cannot be 0"); }
                return currencyMstr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool DeleteCurrencyMasterById(long[] id)
        {
            try
            {
                IList<Inventory_CurrencyMaster> currencymaster = PSF.GetListByIds<Inventory_CurrencyMaster>(id);
                PSF.DeleteAll<Inventory_CurrencyMaster>(currencymaster);
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Dictionary<long, IList<Inventory_ExchangeRateMaster>> GetExchangeRateMasterListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<Inventory_ExchangeRateMaster>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public long SaveOrUpdateExchangeRateMasterDetails(Inventory_ExchangeRateMaster rateMaster)
        {
            try
            {
                if (rateMaster != null)
                    PSF.SaveOrUpdate<Inventory_ExchangeRateMaster>(rateMaster);
                else { throw new Exception("All Fields are required and it cannot be null.."); }
                return rateMaster.Rate_Id;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Inventory_ExchangeRateMaster GetExchangeRateDetailsById(long Id)
        {
            try
            {
                Inventory_ExchangeRateMaster rateMstr = null;
                if (Id > 0)
                    rateMstr = PSF.Get<Inventory_ExchangeRateMaster>(Id);
                else { throw new Exception("Id is required and it cannot be 0"); }
                return rateMstr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool DeleteExchangeRateMasterById(long[] id)
        {
            try
            {
                IList<Inventory_ExchangeRateMaster> ratemaster = PSF.GetListByIds<Inventory_ExchangeRateMaster>(id);
                PSF.DeleteAll<Inventory_ExchangeRateMaster>(ratemaster);
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Inventory_ExchangeRateMaster GetExchangeRateByGLDateandCurrency( long Currency_Id,string GLDate)
        {
            try
            {
                return PSF.Get<Inventory_ExchangeRateMaster>( "Inventory_CurrencyMaster.Currency_Id", Currency_Id,"GLDate", GLDate);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Inventory_CurrencyMaster GetCurrencyDetailsByCurrency(string Currency)
        {
            try
            {
                return PSF.Get<Inventory_CurrencyMaster>("Currency", Currency);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
