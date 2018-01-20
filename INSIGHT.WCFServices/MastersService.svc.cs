using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using INSIGHT.Entities;
using INSIGHT.Component;
using INSIGHT.Entities.InvoiceEntities;
using INSIGHT.Entities.Masters;

namespace INSIGHT.WCFServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "MasterService" in code, svc and config file together.
    public class MastersService : IMastersSC
    {
        MastersBC mbc = new MastersBC();
        #region"Menu"
        public Dictionary<long, IList<Menu>> GetMenuListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                MastersBC menu = new MastersBC();
                return menu.GetMenuListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                MastersBC MastersBC = new MastersBC();
                MastersBC.SaveOrUpdateMenuDetails(mu);
                return mu.Id;
            }
            catch (Exception ex)
            {
                if (ex.ToString().Contains("Cannot insert duplicate key row in object 'dbo.QUEUE' with"))
                {
                    throw new FaultException("Cannot insert duplicate key in Queue Name.");
                }
                throw;
            }
            finally { }
        }
        public Menu GetDeleteMenurowById(long Id)
        {
            try
            {
                MastersBC MastersBC = new MastersBC();
                return MastersBC.GetDeleteMenurowById(Id);
            }
            catch (Exception)
            {
                throw;
            }
            finally { }
        }
        public long DeleteMenufunction(Menu mu)
        {
            try
            {
                MastersBC MastersBC = new MastersBC();
                MastersBC.DeleteMenufunction(mu);
                return 0;
            }
            catch (Exception ex)
            {
                if (ex.ToString().Contains("Cannot insert duplicate key row in object 'dbo.QUEUE' with"))
                {
                    throw new FaultException("Cannot insert duplicate key in Queue Name.");
                }
                throw;
            }
            finally { }
        }
        public long SaveOrUpdateSubMenuDetails(Menu mu)
        {
            try
            {
                MastersBC MastersBC = new MastersBC();
                MastersBC.SaveOrUpdateSubMenuDetails(mu);
                return mu.Id;
            }
            catch (Exception ex)
            {
                if (ex.ToString().Contains("Cannot insert duplicate key row in object 'dbo.QUEUE' with"))
                {
                    throw new FaultException("Cannot insert duplicate key in Queue Name.");
                }
                throw;
            }
            finally { }
        }
        public Menu DeleteSubMenurowById(long Id)
        {
            try
            {
                MastersBC MastersBC = new MastersBC();
                return MastersBC.DeleteSubMenurowById(Id);
            }
            catch (Exception)
            {
                throw;
            }
            finally { }
        }
        #endregion"Menu"

        #region GCC Masters Added By Micheal
        //public Dictionary<long, IList<ItemMaster>> GetItemMasterListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        //{
        //    try
        //    {
        //        return mbc.GetItemMasterListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        //Modified by Gobi

        public Dictionary<long, IList<ItemMaster>> GetItemMasterListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return mbc.GetItemMasterListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                return mbc.GetItemMasterListWithLikeSearchPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                return mbc.GetContigentListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                return mbc.GetContingentMasterListWithLikeSearchPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }


        //Modified by Gobi
        public Dictionary<long, IList<SectorMaster>> GetSectorMasterListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return mbc.GetSectorMasterListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                return mbc.GetSectorMasterListWithLikeSearchPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public long SaveOrUpdateContingentMasterDetails(ContingentMaster cntm)
        {
            try
            {
                mbc.SaveOrUpdateContingentDetails(cntm);
                return cntm.Id;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally { }
        }
        public ContingentMaster GetContingentDetailsById(long Id)
        {
            try
            {
                if (Id > 0)
                {
                    return mbc.GetContingentDetailsById(Id);
                }
                else throw new Exception("Id is required and it cannot be null or empty.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        public long GetDeleteContingentrowById(ContingentMaster cntm)
        {
            try
            {
                return mbc.GetDeleteContingentrowById(cntm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }

        public long SaveOrUpdateItemMasterDetails(ItemMaster itm)
        {
            try
            {
                mbc.SaveOrUpdateItemMasterDetails(itm);
                return itm.Id;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally { }
        }
        public ItemMaster GetItemDetailsById(long Id)
        {
            try
            {
                if (Id > 0)
                {
                    return mbc.GetItemDetailsById(Id);
                }
                else throw new Exception("Id is required and it cannot be null or empty.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        public long GetDeleteItemMasterById(ItemMaster itm)
        {
            try
            {
                return mbc.GetDeleteItemrowById(itm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }

        public long SaveOrUpdateSectorMasterDetails(SectorMaster secm)
        {
            try
            {
                mbc.SaveOrUpdateSectorMasterDetails(secm);
                return secm.Id;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally { }
        }
        public SectorMaster GetSectorDetailsById(long Id)
        {
            try
            {
                if (Id > 0)
                {
                    return mbc.GetSectorDetailsById(Id);
                }
                else throw new Exception("Id is required and it cannot be null or empty.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        public long GetDeleteSectorMasterById(SectorMaster secm)
        {
            try
            {
                return mbc.GetDeleteSectorrowById(secm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }

        //Modified by Gobi
        public Dictionary<long, IList<PeriodMaster>> GetPeriodMasterListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return mbc.GetPeriodMasterListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                return mbc.GetPeriodMasterListWithLikeSearchPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Modified by Gobi
        public Dictionary<long, IList<LocationMaster>> GetLocationMasterListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return mbc.GetLocationMasterListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                return mbc.GetLocationMasterListWithLikeSearchPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }


        //Modified by Gobi
        public Dictionary<long, IList<CountryMaster>> GetCountryMasterListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return mbc.GetCountryMasterListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                return mbc.GetCountryMasterListWithLikeSearchPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }


        //Modified by Gobi
        public Dictionary<long, IList<UNSectorConMapping>> GetUNSectorMappingMasterListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return mbc.GetUNSectorMappingMasterListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                return mbc.GetUNSectorMappingMasterListWithLikeSearchPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Dictionary<long, IList<UserTypeMaster>> GetUserTypeMasterListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return mbc.GetUserTypeMasterListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /*new methods*/
        public long CreateOrUpdateDocumentUpload(DocumentUpload doc)
        {
            try
            {
                mbc.CreateOrUpdateDocumentUpload(doc);
                return doc.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }

        #region New Masters
        public Dictionary<long, IList<UOMMaster>> GetUOMMasterListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return mbc.GetUOMMasterListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                return mbc.GetCMRMasterListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                return mbc.GetCMRMasterListWithLikeSearchPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
        //        return mbc.GetDiscrepancyMasterListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                return mbc.GetDiscrepancyMasterListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                return mbc.GetDiscrepancyMasterListWithLikeSearchPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        #endregion

        #region GCC Masters Added By Gobi

        #region Location Master
        public long SaveOrUpdateLocationMasterDetails(LocationMaster lm)
        {
            try
            {
                mbc.SaveOrUpdateLocationMasterDetails(lm);
                return lm.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        public LocationMaster GetLocationDetailsById(long Id)
        {
            try
            {
                if (Id > 0)
                {
                    return mbc.GetLocationDetailsById(Id);
                }
                else throw new Exception("Id is required and it cannot be null or empty.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        public long GetDeleteLocationMasterById(LocationMaster lm)
        {
            try
            {
                if (lm.Id > 0)
                {
                    mbc.GetDeleteLocationrowById(lm);
                    return lm.Id;
                }

                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        #endregion

        #region Country Master
        public long SaveOrUpdateCountryMasterDetails(CountryMaster cm)
        {
            try
            {
                mbc.SaveOrUpdateCountryMasterDetails(cm);
                return cm.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        public CountryMaster GetCountryDetailsById(long Id)
        {
            try
            {
                if (Id > 0)
                {
                    return mbc.GetCountryDetailsById(Id);
                }
                else throw new Exception("Id is required and it cannot be null or empty.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        public long GetDeleteCountryMasterById(CountryMaster cm)
        {
            try
            {
                if (cm.Id > 0)
                {
                    mbc.GetDeleteCountryrowById(cm);
                    return cm.Id;
                }

                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        #endregion

        #region UNSectorMappping Master
        public long SaveOrUpdateUNSectorMapppingMasterDetails(UNSectorConMapping unsm)
        {
            try
            {
                mbc.SaveOrUpdateUNSectorMapppingMasterDetails(unsm);
                return unsm.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        public UNSectorConMapping GetUNSectorMapppingDetailsById(long Id)
        {
            try
            {
                if (Id > 0)
                {
                    return mbc.GetUNSectorMapppingDetailsById(Id);
                }
                else throw new Exception("Id is required and it cannot be null or empty.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        public long GetDeleteUNSectorMapppingMasterById(UNSectorConMapping unsm)
        {
            try
            {
                if (unsm.Id > 0)
                {
                    mbc.GetDeleteUNSectorMapppingrowById(unsm);
                    return unsm.Id;
                }

                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        #endregion

        #region CMR Master
        public long SaveOrUpdateCMRMasterDetails(CMRMaster cmr)
        {
            try
            {
                mbc.SaveOrUpdateCMRMasterDetails(cmr);
                return cmr.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        public CMRMaster GetCMRDetailsById(long Id)
        {
            try
            {
                if (Id > 0)
                {
                    return mbc.GetCMRDetailsById(Id);
                }
                else throw new Exception("Id is required and it cannot be null or empty.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        public long GetDeleteCMRMasterById(CMRMaster cmr)
        {
            try
            {
                if (cmr.Id > 0)
                {
                    mbc.GetDeleteCMRrowById(cmr);
                    return cmr.Id;
                }

                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        #endregion

        #region Discrepancy Master
        public long SaveOrUpdateDiscrepancyMasterDetails(DiscrepancyMaster dm)
        {
            try
            {
                mbc.SaveOrUpdateDiscrepancyMasterDetails(dm);
                return dm.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        public DiscrepancyMaster GetDiscrepancyDetailsById(long Id)
        {
            try
            {
                if (Id > 0)
                {
                    return mbc.GetDiscrepancyDetailsById(Id);
                }
                else throw new Exception("Id is required and it cannot be null or empty.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        public long GetDeleteDiscrepancyMasterById(DiscrepancyMaster dm)
        {
            try
            {
                if (dm.Id > 0)
                {
                    mbc.GetDeleteDiscrepancyrowById(dm);
                    return dm.Id;
                }

                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        #endregion


        #endregion

        public CMRMaster GetCMRMasterBySectorCode(string sc)
        {
            try
            {
                return mbc.GetCMRMasterBySectorCode(sc);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public long SaveOrUpdatePeriodMasterDetails(PeriodMaster itm)
        {
            try
            {
                mbc.SaveOrUpdatePeriodMasterDetails(itm);
                return itm.Id;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally { }
        }
        public PeriodMaster GetPeriodDetailsById(long Id)
        {
            try
            {
                if (Id > 0)
                {
                    return mbc.GetPeriodDetailsById(Id);
                }
                else throw new Exception("Id is required and it cannot be null or empty.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        public long GetDeletePeriodMasterById(PeriodMaster itm)
        {
            try
            {
                return mbc.GetDeletePeriodMasterById(itm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }

        #region UOMMaster

        public long SaveOrUpdateUOMMasterDetails(UOMMaster itm)
        {
            try
            {
                mbc.SaveOrUpdateUOMMasterDetails(itm);
                return itm.Id;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally { }
        }
        public UOMMaster GetUOMMasterDetailsById(long Id)
        {
            try
            {
                if (Id > 0)
                {
                    return mbc.GetUOMMasterDetailsById(Id);
                }
                else throw new Exception("Id is required and it cannot be null or empty.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        public bool DeleteUOMMasterDetails(long[] Id)
        {
            try
            {

                mbc.DeleteUOMMastersDetails(Id);
                return true;
            }
            catch (Exception ex)
            {
                if (ex.ToString().Contains("Cannot insert duplicate key row in object 'dbo.QUEUE' with"))
                {
                    throw new FaultException("Cannot insert duplicate key in Queue Name.");
                }
                throw;
            }
            finally { }
        }
        #endregion
        //Added by kingston for item master auto complete
        //GetItemMasterListWithPagingAndCriteriaForAutoComplete
        public Dictionary<long, IList<ItemMaster>> GetItemMasterListWithPagingAndCriteriaForAutoComplete(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return mbc.GetItemMasterListWithPagingAndCriteriaForAutoComplete(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Substitution Master jqgrid

        public Dictionary<long, IList<SubstitutionMaster>> GetISubstitutionMasterListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return mbc.GetISubstitutionMasterListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                return mbc.GetTransportPriceMasterWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                return mbc.GetTransportPriceMasterWithLikeSearchPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                MastersBC documenttypeBC = new MastersBC();
                return documenttypeBC.GetDocumentTypeMasterListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public long CreateOrUpdateEmailLog(EmailLog el)
        {
            try
            {
                MastersBC documenttypeBC = new MastersBC();
                documenttypeBC.CreateOrUpdateEmailLog(el);
                return el.Id;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally { }
        }

        #region POMmasters
        public Dictionary<long, IList<POMasters>> GetPOMastersListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return mbc.GetPOMastersListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                return mbc.GetPOMastersListWithLikeSearchPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public long SaveOrUpdatePoMasterDetails(POMasters itm)
        {
            try
            {
                mbc.SaveOrUpdatePoMasterDetails(itm);
                return itm.Id;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally { }
        }
        public POMasters GetPoMasterDetailsById(long Id)
        {
            try
            {
                if (Id > 0)
                {
                    return mbc.GetPoMasterDetailsById(Id);
                }
                else throw new Exception("Id is required and it cannot be null or empty.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        public long GetDeletePoMasterById(POMasters itm)
        {
            try
            {
                return mbc.GetDeletePoMasterById(itm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }

        public Dictionary<long, IList<POTypeMaster>> GetPOTypeListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return mbc.GetPOTypeListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                MastersBC mBC = new MastersBC();
                long a = mBC.GetCurrentIntent(table);
                return a;
            }
            catch (Exception ex)
            {
                if (ex.ToString().Contains("Cannot insert duplicate key row in object 'dbo.QUEUE' with"))
                {
                    throw new FaultException("Cannot insert duplicate key in Queue Name.");
                }
                throw;
            }
            finally { }
        }
        #endregion end


        #region  UNItemsDetailedPrice
        public Dictionary<long, IList<UNSectorDetailedItemPrice>> GetUNSectorDetailedItemPriceListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                MastersBC masterbcobj = new MastersBC();
                return masterbcobj.GetUNSectorDetailedItemPriceListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public long SaveOrUpdateUNDetailedItemMasterPrice(UNSectorDetailedItemPrice itm)
        {
            try
            {
                MastersBC masterbcobj = new MastersBC();
                masterbcobj.SaveOrUpdateUNDetailedItemMasterPrice(itm);
                return itm.Id;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally { }
        }

        public void DeleteUNDetailedItemMasterPrice(UNSectorDetailedItemPrice itm)
        {
            try
            {
                mbc.DeleteUNDetailedItemMasterPrice(itm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        public UNSectorDetailedItemPrice GetNDetailedItemMasterPriceById(long Id)
        {
            try
            {
                if (Id > 0)
                {
                    return mbc.GetNDetailedItemMasterPriceById(Id);
                }
                else throw new Exception("Id is required and it cannot be null or empty.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }

        #endregion
        #region Inventory Management
        public Dictionary<long, IList<Inventory_CurrencyMaster>> GetCurrencyMasterListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return mbc.GetCurrencyMasterListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                mbc.SaveOrUpdateCurrencyMasterDetails(currencyMaster);
                return currencyMaster.Currency_Id;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally { }
        }
        public Inventory_CurrencyMaster GetCurrencyDetailsById(long Id)
        {
            try
            {
                if (Id > 0)
                {
                    return mbc.GetCurrencyDetailsById(Id);
                }
                else throw new Exception("Id is required and it cannot be null or empty.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        public bool DeleteCurrencyMasterById(long[] id)
        {
            try
            {
                return mbc.DeleteCurrencyMasterById(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        public Dictionary<long, IList<Inventory_ExchangeRateMaster>> GetExchangeRateMasterListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return mbc.GetExchangeRateMasterListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                mbc.SaveOrUpdateExchangeRateMasterDetails(rateMaster);
                return rateMaster.Rate_Id;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally { }
        }
        public Inventory_ExchangeRateMaster GetExchangeRateDetailsById(long Id)
        {
            try
            {
                if (Id > 0)
                {
                    return mbc.GetExchangeRateDetailsById(Id);
                }
                else throw new Exception("Id is required and it cannot be null or empty.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        public bool DeleteExchangeRateMasterById(long[] id)
        {
            try
            {
                return mbc.DeleteExchangeRateMasterById(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        public Inventory_ExchangeRateMaster GetExchangeRateByGLDateandCurrency(long Currency_Id, string GLDate)
        {
            try
            {
                if (Currency_Id > 0 && !string.IsNullOrEmpty(GLDate))
                {
                    return mbc.GetExchangeRateByGLDateandCurrency(Currency_Id, GLDate);
                }
                else throw new Exception("Id is required and it cannot be null or empty.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        public Inventory_CurrencyMaster GetCurrencyDetailsByCurrency(string Currency)
        {
            try
            {
                if (!string.IsNullOrEmpty(Currency))
                {
                    return mbc.GetCurrencyDetailsByCurrency(Currency);
                }
                else throw new Exception("Currency is required and it cannot be null or empty.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        #endregion

    }
}
