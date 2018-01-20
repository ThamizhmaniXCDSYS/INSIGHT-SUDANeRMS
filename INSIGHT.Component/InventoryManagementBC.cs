using INSIGHT.Entities.InventoryManagementEntities;
using PersistenceFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INSIGHT.Component
{
    public class InventoryManagementBC
    {
        PersistenceServiceFactory PSF = null;
        ProjectSpecificPSF SPSF = null;
        public InventoryManagementBC()
        {
            List<String> Assembly = new List<String>();
            Assembly.Add("INSIGHT.Entities.InventoryManagementEntities");
            PSF = new PersistenceServiceFactory(Assembly);
            SPSF = new ProjectSpecificPSF(Assembly);

        }
        #region Inventory related dropdown list
        public Dictionary<long, IList<Inventory_PurchaseOrder>> GetPONumberWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<Inventory_PurchaseOrder>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<long, IList<Inventory_PurchaseOrderInvoice>> GetPurchaseOrderInvoiceListWithPagingAndCriteriaLikeSearch(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithExactSearchCriteriaCount<Inventory_PurchaseOrderInvoice>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        #region POMaster
        public Dictionary<long, IList<Inventory_PurchaseOrder>> GetPurchaseOrderDetailsListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria, Dictionary<string, object> likeCriteria)
        {
            try
            {
                return PSF.GetListWithExactAndLikeSearchCriteriaWithCount<Inventory_PurchaseOrder>(page, pageSize, sortType, sortby, criteria, likeCriteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Inventory_PurchaseOrder GetPurchaseOrderbyPurchaseOrderId(long PurchOrderId)
        {
            try
            {
                return PSF.Get<Inventory_PurchaseOrder>("PurchOrderId", PurchOrderId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Dictionary<long, IList<Inventory_PurchaseOrder>> GetPODetailsListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria, Dictionary<string, object> likeCriteria)
        {
            try
            {
                return PSF.GetListWithExactAndLikeSearchCriteriaWithCount<Inventory_PurchaseOrder>(page, pageSize, sortType, sortby, criteria, likeCriteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<long, IList<Inventory_PurchaseOrderItem_vw>> GetPurchaseOrderItemListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<Inventory_PurchaseOrderItem_vw>(page, pageSize, sortType, sortby, criteria);
                //return PSF.GetListWithSearchCriteriaCount<Inventory_PurchaseOrderItem_vw>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        #region Receipt
        public Dictionary<long, IList<ReceiptReport>> GetReceiptReportListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria, Dictionary<string, object> likeCriteria)
        {
            try
            {
                return PSF.GetListWithExactAndLikeSearchCriteriaWithCount<ReceiptReport>(page, pageSize, sortType, sortby, criteria, likeCriteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void InsertPOMasterDetailsByRequestId_sp(long RequestIds)
        {
            try
            {
                //string POInsertQuery = "EXEC dbo.InsertPOMasterDetailsByRequestId_sp " + RequestIds;
                string POInsertQuery = "EXEC dbo.Inventory_InsertPurchaseOrderandOrderItemByRequestId_sp " + RequestIds;
                PSF.ExecuteSql(POInsertQuery);
            }
            catch (Exception)
            {
                throw;
            }

        }
        public void InsertorUpdatePOReceiptReportDetailsByRequestId_sp(long RequestIds)
        {
            try
            {
                string POInsertQuery = "EXEC dbo.ReceiptReportUpdate_sp " + RequestIds;
                PSF.ExecuteSql(POInsertQuery);
            }
            catch (Exception)
            {
                throw;
            }

        }
        #endregion
        #region Report Generation
        public Dictionary<long, IList<GITReport_vw>> GetGITReportListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria, Dictionary<string, object> likeCriteria)
        {
            try
            {
                return PSF.GetListWithExactAndLikeSearchCriteriaWithCount<GITReport_vw>(page, pageSize, sortType, sortby, criteria, likeCriteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        public Inventory_PurchaseOrderInvoice GetInvoiceByInvoiceNumber(string InvoiceNumber)
        {
            try
            {
                return PSF.Get<Inventory_PurchaseOrderInvoice>("InvoiceNumber", InvoiceNumber);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public long SaveOrUpdatePurchaseOrderInvocie(Inventory_PurchaseOrderInvoice purchaseOrderInvoice)
        {
            try
            {
                if (purchaseOrderInvoice != null)
                    PSF.SaveOrUpdate<Inventory_PurchaseOrderInvoice>(purchaseOrderInvoice);
                else { throw new Exception("Purchase order is required and it cannot be null.."); }
                return purchaseOrderInvoice.POInvoiceId;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Inventory_PurchaseOrderItem GetPurchaseOrderItemsById(long POLineId)
        {
            try
            {
                return PSF.Get<Inventory_PurchaseOrderItem>("POLineId", POLineId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public long SaveOrUpdatePurchaseOrderItems(Inventory_PurchaseOrderItem poOrditems)
        {
            try
            {
                if (poOrditems != null)
                    PSF.SaveOrUpdate<Inventory_PurchaseOrderItem>(poOrditems);
                else { throw new Exception("PO is required and it cannot be null.."); }
                return poOrditems.POLineId;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public long SaveOrUpdatePurchaseOrderInvoiceItems(Inventory_PurchaseOrderInvoiceItem podInvoiceitems)
        {
            try
            {
                if (podInvoiceitems != null)
                    PSF.SaveOrUpdate<Inventory_PurchaseOrderInvoiceItem>(podInvoiceitems);
                else { throw new Exception("PO is required and it cannot be null.."); }
                return podInvoiceitems.PurchOrderId;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public long SaveOrUpdatePurchaseOrder(Inventory_PurchaseOrder purchaseOrder)
        {
            try
            {
                if (purchaseOrder != null)
                    PSF.SaveOrUpdate<Inventory_PurchaseOrder>(purchaseOrder);
                else { throw new Exception("Orders is required and it cannot be null.."); }
                return purchaseOrder.PurchOrderId;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Inventory_PurchaseOrderInvoiceItem GetPurchaseOrderInvoiceItemsByPOIdandPOInvoiceIdandPOLineId(long PurchOrderId, long POInvoiceId, long POLineId)
        {
            try
            {
                return PSF.Get<Inventory_PurchaseOrderInvoiceItem>("PurchOrderId", PurchOrderId, "POInvoiceId", POInvoiceId, "POLineId", POLineId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Inventory_PurchaseOrderInvoiceItem GetPurchaseOrderInvoiceItemsById(long POInvoiceItemId)
        {
            try
            {
                return PSF.Get<Inventory_PurchaseOrderInvoiceItem>("POInvoiceItemId", POInvoiceItemId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IList<Inventory_PurchaseOrderInvoiceItem> GetPurchaseOrderInvoiceItemsListByPurchOrderId(long PurchOrderId)
        {
            try
            {
                return PSF.GetList<Inventory_PurchaseOrderInvoiceItem>("PurchOrderId", PurchOrderId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<long, IList<Inventory_PurchaseOrderInvoice>> GetPurchaseOrderInvoiceItemListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria, Dictionary<string, object> likeCriteria)
        {
            try
            {
                return PSF.GetListWithExactAndLikeSearchCriteriaWithCount<Inventory_PurchaseOrderInvoice>(page, pageSize, sortType, sortby, criteria, likeCriteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IList<Inventory_PurchaseOrderInvoiceItem> GetPurchaseOrderInvoiceItemsListByPOLineId(long POLineId)
        {
            try
            {
                return PSF.GetList<Inventory_PurchaseOrderInvoiceItem>("POLineId", POLineId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<long, IList<Inventory_PurchaseOrderItem>> GetPurchaseOrderItemListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria, Dictionary<string, object> likeCriteria)
        {
            try
            {
                return PSF.GetListWithExactAndLikeSearchCriteriaWithCount<Inventory_PurchaseOrderItem>(page, pageSize, sortType, sortby, criteria, likeCriteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool SaveOrUpdateInvoiceItemList(IList<Inventory_PurchaseOrderInvoiceItem> invoiceItemlist)
        {
            try
            {
                bool retValue = false;
                if (invoiceItemlist != null && invoiceItemlist.Count > 0)
                {
                    PSF.SaveOrUpdate<Inventory_PurchaseOrderInvoiceItem>(invoiceItemlist);
                    retValue = true;
                }
                else { throw new Exception("Invoice Item is required and it cannot be null.."); }
                return retValue;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IList<Inventory_PurchaseOrderInvoiceItem> GetPurchaseOrderInvoiceItemsListByPOInvoiceId(long POInvoiceId)
        {
            try
            {
                return PSF.GetList<Inventory_PurchaseOrderInvoiceItem>("POInvoiceId", POInvoiceId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Inventory_PurchaseOrderInvoice GetInvoiceByPOInvoiceId(long POInvoiceId)
        {
            try
            {
                return PSF.Get<Inventory_PurchaseOrderInvoice>("POInvoiceId", POInvoiceId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<long, IList<Inventory_PurchaseOrderInvoiceItem_VW>> GetPurchaseInvoiceItemListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<Inventory_PurchaseOrderInvoiceItem_VW>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool SaveOrUpdatepoInvoiceMappingList(IList<Inventory_PurchaseOrderInvoiceMapping> poInvoicelist)
        {
            try
            {
                bool retValue = false;
                if (poInvoicelist != null && poInvoicelist.Count > 0)
                {
                    PSF.SaveOrUpdate<Inventory_PurchaseOrderInvoiceMapping>(poInvoicelist);
                    retValue = true;
                }
                else { throw new Exception("Invoice Item is required and it cannot be null.."); }
                return retValue;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<long, IList<Inventory_PurchaseOrderInvoiceItem_VW>> GetPurchaseOrderInvoiceItemListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<Inventory_PurchaseOrderInvoiceItem_VW>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IList<Inventory_PurchaseOrderInvoiceMapping> GetPurchaseOrderInvoiceMappingListByPOInvoiceId(long POInvoiceId)
        {
            try
            {
                return PSF.GetList<Inventory_PurchaseOrderInvoiceMapping>("POInvoiceId", POInvoiceId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void DeleteInventory_PurchaseOrderInvoiceMappingById(long POInvoiceId, string PurchOrderId)
        {
            try
            {
                string InvoiceMappingDeletequery = "DELETE FROM dbo.Inventory_PurchaseOrderInvoiceMapping WHERE POInvoiceId=" + POInvoiceId + " AND PurchOrderId IN (" + PurchOrderId + ")";
                PSF.ExecuteSql(InvoiceMappingDeletequery);
                string InvoiceItemDeletequery = "DELETE FROM dbo.Inventory_PurchaseOrderInvoiceItem WHERE POInvoiceId=" + POInvoiceId + " AND PurchOrderId IN (" + PurchOrderId + ")";
                PSF.ExecuteSql(InvoiceItemDeletequery);
                UpdateInvoiceAmountandInvoiceQtyinInventory_PurchaseOrderItemandInventory_PurchaseOrder(PurchOrderId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void UpdateInvoiceAmountandInvoiceQtyinInventory_PurchaseOrderItemandInventory_PurchaseOrder(string PurchOrderId)
        {
            try
            {
                string Inventory_PurchaseOrderItemUpdatequery = "UPDATE dbo.Inventory_PurchaseOrderItem SET InvoicedQty=(SELECT SUM(InvoicedQty) FROM dbo.Inventory_PurchaseOrderInvoiceItem WHERE POLineId IN(SELECT POLineId FROM dbo.Inventory_PurchaseOrderItem"
                + "WHERE PurchOrderId IN(" + PurchOrderId + "))),RemainingQty=OrderedQty-ISNULL(InvoicedQty,0)WHERE PurchOrderId IN(" + PurchOrderId + ")";
                PSF.ExecuteSql(Inventory_PurchaseOrderItemUpdatequery);

                string Inventory_PurchaseOrderUpdatequery = "UPDATE dbo.Inventory_PurchaseOrder SET InvoiceAmount=(SELECT SUM(InvoiceValue) FROM dbo.Inventory_PurchaseOrderInvoiceItem WHERE POLineId IN(SELECT POLineId FROM dbo.Inventory_PurchaseOrderItem "
                + "WHERE PurchOrderId IN(" + PurchOrderId + ")))WHERE PurchOrderId IN("+PurchOrderId+")";
                PSF.ExecuteSql(Inventory_PurchaseOrderUpdatequery);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
