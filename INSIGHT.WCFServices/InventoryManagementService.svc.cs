using INSIGHT.Component;
using INSIGHT.Entities.InventoryManagementEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace INSIGHT.WCFServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "InventoryManagementService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select InventoryManagementService.svc or InventoryManagementService.svc.cs at the Solution Explorer and start debugging.
    public class InventoryManagementService : IInventoryManagementService
    {
        #region Inventory related dropdown list
        public Dictionary<long, IList<Inventory_PurchaseOrder>> GetPONumberWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                InventoryManagementBC inventoryBC = new InventoryManagementBC();
                return inventoryBC.GetPONumberWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                InventoryManagementBC inventoryBC = new InventoryManagementBC();
                return inventoryBC.GetPurchaseOrderInvoiceListWithPagingAndCriteriaLikeSearch(page, pageSize, sortby, sortType, criteria);
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
                InventoryManagementBC inventoryBC = new InventoryManagementBC();
                return inventoryBC.GetPurchaseOrderDetailsListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria, likeCriteria);
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
                InventoryManagementBC inventoryBC = new InventoryManagementBC();
                return inventoryBC.GetPurchaseOrderbyPurchaseOrderId(PurchOrderId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region Receipt
        public Dictionary<long, IList<Inventory_PurchaseOrder>> GetPODetailsListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria, Dictionary<string, object> likeCriteria)
        {
            try
            {
                InventoryManagementBC inventoryBC = new InventoryManagementBC();
                return inventoryBC.GetPODetailsListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria, likeCriteria);
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
                InventoryManagementBC inventoryBC = new InventoryManagementBC();
                return inventoryBC.GetPurchaseOrderItemListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<long, IList<ReceiptReport>> GetReceiptReportListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria, Dictionary<string, object> likeCriteria)
        {
            try
            {
                InventoryManagementBC inventoryBC = new InventoryManagementBC();
                return inventoryBC.GetReceiptReportListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria, likeCriteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void InsertPOMasterDetailsByRequestId_sp(long RequestId)
        {
            try
            {
                InventoryManagementBC inventoryBC = new InventoryManagementBC();
                inventoryBC.InsertPOMasterDetailsByRequestId_sp(RequestId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void InsertorUpdatePOReceiptReportDetailsByRequestId_sp(long RequestId)
        {
            try
            {
                InventoryManagementBC inventoryBC = new InventoryManagementBC();
                inventoryBC.InsertorUpdatePOReceiptReportDetailsByRequestId_sp(RequestId);
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
                InventoryManagementBC inventoryBC = new InventoryManagementBC();
                return inventoryBC.GetGITReportListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria, likeCriteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        public Inventory_PurchaseOrderInvoice GetInvoiceByInvoiceNumber( string InvoiceNumber)
        {
            try
            {
                InventoryManagementBC inventoryBC = new InventoryManagementBC();
                return inventoryBC.GetInvoiceByInvoiceNumber(InvoiceNumber);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public long SaveOrUpdatePurchaseOrderInvocie(Inventory_PurchaseOrderInvoice purchaseOrderInvoice)
        {
            try
            {
                InventoryManagementBC inventoryBC = new InventoryManagementBC();
                return inventoryBC.SaveOrUpdatePurchaseOrderInvocie(purchaseOrderInvoice);
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
                InventoryManagementBC inventoryBC = new InventoryManagementBC();
                return inventoryBC.GetPurchaseOrderItemsById(POLineId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public long SaveOrUpdateInventory_PurchaseOrderItem(Inventory_PurchaseOrderItem poOrditems)
        {
            try
            {
                InventoryManagementBC inventoryBC = new InventoryManagementBC();
                return inventoryBC.SaveOrUpdatePurchaseOrderItems(poOrditems);
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
                InventoryManagementBC inventoryBC = new InventoryManagementBC();
                return inventoryBC.SaveOrUpdatePurchaseOrderInvoiceItems(podInvoiceitems);
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
                InventoryManagementBC inventoryBC = new InventoryManagementBC();
                return inventoryBC.SaveOrUpdatePurchaseOrder(purchaseOrder);
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
                InventoryManagementBC inventoryBC = new InventoryManagementBC();
                return inventoryBC.GetPurchaseOrderInvoiceItemsByPOIdandPOInvoiceIdandPOLineId(PurchOrderId, POInvoiceId,POLineId);
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
                InventoryManagementBC inventoryBC = new InventoryManagementBC();
                return inventoryBC.GetPurchaseOrderInvoiceItemsById(POInvoiceItemId);
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
                InventoryManagementBC inventoryBC = new InventoryManagementBC();
                return inventoryBC.GetPurchaseOrderInvoiceItemsListByPurchOrderId(PurchOrderId);
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
                InventoryManagementBC inventoryBC = new InventoryManagementBC();
                return inventoryBC.GetPurchaseOrderInvoiceItemListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria, likeCriteria);
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
                InventoryManagementBC inventoryBC = new InventoryManagementBC();
                return inventoryBC.GetPurchaseOrderInvoiceItemsListByPOLineId(POLineId);
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
                InventoryManagementBC inventoryBC = new InventoryManagementBC();
                return inventoryBC.GetPurchaseOrderItemListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria, likeCriteria);
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
                InventoryManagementBC inventoryBC = new InventoryManagementBC();
                return inventoryBC.SaveOrUpdateInvoiceItemList(invoiceItemlist);
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
                InventoryManagementBC inventoryBC = new InventoryManagementBC();
                return inventoryBC.GetPurchaseOrderInvoiceItemsListByPOInvoiceId(POInvoiceId);
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
                InventoryManagementBC inventoryBC = new InventoryManagementBC();
                return inventoryBC.GetInvoiceByPOInvoiceId(POInvoiceId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Dictionary<long, IList<Inventory_PurchaseOrderInvoiceItem_VW>> GetPurchaseInvoiceItemListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                InventoryManagementBC inventoryBC = new InventoryManagementBC();
                return inventoryBC.GetPurchaseInvoiceItemListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                InventoryManagementBC inventoryBC = new InventoryManagementBC();
                return inventoryBC.SaveOrUpdatepoInvoiceMappingList(poInvoicelist);
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
                InventoryManagementBC inventoryBC = new InventoryManagementBC();
                return inventoryBC.GetPurchaseOrderInvoiceItemListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                InventoryManagementBC inventoryBC = new InventoryManagementBC();
                return inventoryBC.GetPurchaseOrderInvoiceMappingListByPOInvoiceId(POInvoiceId);
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
                InventoryManagementBC inventoryBC = new InventoryManagementBC();
                inventoryBC.DeleteInventory_PurchaseOrderInvoiceMappingById(POInvoiceId, PurchOrderId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
    }
}
