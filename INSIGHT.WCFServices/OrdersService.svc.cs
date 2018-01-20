using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using INSIGHT.Entities;
using INSIGHT.Component;
using System.Collections;
using INSIGHT.Entities.DeletedRecordEntities;
using INSIGHT.Entities.InputUploadEntities;
using INSIGHT.Entities.ReportEntities;
namespace INSIGHT.WCFServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "OrdersService" in code, svc and config file together.
    public class OrdersService : IOrdersService
    {
        //OrdersBC ordersBC = new OrdersBC();
        public Orders GetOrdersById(long OrderId)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetOrdersById(OrderId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public long SaveOrUpdateOrder(Orders Orders)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.SaveOrUpdateOrder(Orders);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public long SaveOrUpdateOrderItems(OrderItems orditems)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.SaveOrUpdateOrderItems(orditems);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool SaveOrUpdateOrderItemsList(IList<OrderItems> listOfItems)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.SaveOrUpdateOrderItemsList(listOfItems);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public bool SaveOrUpdateOrderItems(IList<OrderItems> listOfItems)
        //{
        //    try
        //    {
        //        OrdersBC OrdersBC = new OrdersBC();
        //        return OrdersBC.SaveOrUpdateOrderItems(listOfItems);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        public Dictionary<long, IList<Orders>> GetOrdersListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetOrdersListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<long, IList<OrderItems>> GetOrderItemsListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetOrderItemsListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<long, IList<OrderItems>> GetOrderItemsListWithLikeSearchPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetOrderItemsListWithLikeSearchPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IList GetCounterValue(string TableName)
        {
            MastersBC mstbc = new MastersBC();
            return mstbc.GetCounterValue(TableName);
        }

        public Dictionary<long, IList<OrdersPerMonth_vw>> GetOrdersPerMonth_vwListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortBy, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetOrdersPerMonth_vwListWithPagingAndCriteria(page, pageSize, sortType, sortBy, criteria);
            }
            catch (Exception)
            {
                throw;
            }
            finally { }
        }

        public Dictionary<long, IList<OrderItemsCountAndMismatchCount_vw>> GetOrderItemsCountListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortBy, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetOrderItemsCountListWithPagingAndCriteria(page, pageSize, sortType, sortBy, criteria);
            }
            catch (Exception)
            {
                throw;
            }
            finally { }
        }

        public Dictionary<long, IList<PODItemsCount_vw>> GetPOdItemsCount_vwListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortBy, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetPOdItemsCount_vwListWithPagingAndCriteria(page, pageSize, sortType, sortBy, criteria);
            }
            catch (Exception)
            {
                throw;
            }
            finally { }
        }


        #region Delivery Reports
        public Dictionary<long, IList<PODItems>> GetPodItemsListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetPodItemsListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public PODItems GetPodItemsValById(long PODItemsId)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetPodItemsValById(PODItemsId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public long SaveOrUpdatePODItems(PODItems podItems)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.SaveOrUpdatePODItems(podItems);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DeliveryNote GetDeliveryNoteById(long DeliveryNoteId)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetDeliveryNoteById(DeliveryNoteId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion end Delivery Reports

        #region kingston pod related code
        //PODMasterJQGrid
        public Dictionary<long, IList<POD>> GetPODMasterListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetPODMasterListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }


        //GetPODOrderItemsListWithPagingAndCriteria
        public Dictionary<long, IList<PODOrderItems_vw>> GetPODOrderItemsListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetPODOrderItemsListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //get details from table by

        public POD GetPODById(long PODId)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetPODById(PODId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //POD Master save or update
        public long SaveOrUpdatePOD(POD pod)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.SaveOrUpdatePOD(pod);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public OrderItems GetOrderItemsById(long LineId)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetOrderItemsById(LineId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Delvered pod items jqgrid

        public Dictionary<long, IList<DeliveredPODItems_vw>> GetDeliveredPODItemsListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetDeliveredPODItemsListWithPagingAndCriteria(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Delvered pod items jqgrid like search for commodity
        public Dictionary<long, IList<DeliveredPODItems_vw>> GetDeliveredPODItemsListWithLikePagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetDeliveredPODItemsListWithLikePagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public POD GetPODByOrderId(long OrderId)
        {
            try
            {
                OrdersBC ordersBC = new OrdersBC();
                return ordersBC.GetPODByOrderId(OrderId);
            }
            catch (Exception)
            {
                throw;
            }
        }


        //PODItemsList

        public Dictionary<long, IList<PODItems>> GetPODItemsListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetPODItemsListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //updating orders ---expected delivery date
        public void updateOrders(long OrderId, DateTime? ExpectedDeliveryDate)
        {
            OrdersBC ordersBC = new OrdersBC();
            ordersBC.updateOrders(OrderId, ExpectedDeliveryDate);
        }


        //Creating Delivery note
        public long SaveOrUpdateDeliveryNote(DeliveryNote dn)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.SaveOrUpdateDeliveryNote(dn);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //updating DeliveryNoteId in PODItems table by sending the PODItemsId as an array

        public void updateDeliveryNoteIdinPODItemsTbl(long DeliveryNoteId, string DeliveryNoteName, string PODItemsId, string DeliverySector)
        {
            OrdersBC ordersBC = new OrdersBC();
            ordersBC.updateDeliveryNoteIdinPODItemsTbl(DeliveryNoteId, DeliveryNoteName, PODItemsId, DeliverySector);

        }

        //Listing deliver note values

        public Dictionary<long, IList<DeliveryNote>> GetDeliveryNoteListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetDeliveryNoteListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //updating deliveredQty cell


        public void UpdateDeliveredQtyCell(long LineId, Decimal DelQty, Decimal RemainingQty)
        {
            OrdersBC ordersBC = new OrdersBC();
            ordersBC.UpdateDeliveredQtyCell(LineId, DelQty, RemainingQty);

        }


        public void UpdateSubstituteItemNameCell(long LineId, string SubstituteItemName, long subitemcode, string DiscrepancyCode)
        {
            OrdersBC ordersBC = new OrdersBC();
            ordersBC.UpdateSubstituteItemNameCell(LineId, SubstituteItemName, subitemcode, DiscrepancyCode);

        }

        //PODOrders list page grid


        public Dictionary<long, IList<PODOrdersList_vw>> GetPODOrdersListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetPODOrdersListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //get list from PODOrdersList_vw view by podid


        public PODOrdersList_vw GetPODOrdersListById(long PODId)
        {
            try
            {
                OrdersBC ordersBC = new OrdersBC();
                return ordersBC.GetPODOrdersListById(PODId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //PODOrderItems_vw like search for commodity


        public Dictionary<long, IList<PODOrderItems_vw>> GetPODOrderItemsListWithLikeSearchPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetPODOrderItemsListWithLikeSearchPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Delete PODItems table by object

        public void DeletePODItemsbyObj(PODItems poditems)
        {
            try
            {
                OrdersBC ordersBC = new OrdersBC();
                ordersBC.DeletePODItemsbyObj(poditems);
            }
            catch (Exception ex)
            { throw ex; }


        }
        //Delivery note list page

        public Dictionary<long, IList<DeliveryNoteOrders_vw>> GetDeliveryNoteOrderListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetDeliveryNoteOrderListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //delivery note list Deliverynotename like search

        public Dictionary<long, IList<DeliveryNoteOrders_vw>> GetDeliveryNoteOrderListWithLikeSearchPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetDeliveryNoteOrderListWithLikeSearchPagingAndCriteria(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //updating Substitute item name and item code in PODItems table
        public void UpdateSubItemDetailsinPODItemsTbl(long GreatestPODItemsId, string SubstituteItemName, long subitemcode, string DiscrepancyCode)
        {
            try
            {

                OrdersBC ordersBC = new OrdersBC();

                ordersBC.UpdateSubItemDetailsinPODItemsTbl(GreatestPODItemsId, SubstituteItemName, subitemcode, DiscrepancyCode);

            }

            catch (Exception ex)
            {
                throw ex;
            }

        }

        #endregion


        #region Added by arun orderitems report
        public Dictionary<long, IList<OrderItemsReport_vw>> GetOrderListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetOrderListWithPagingAndCriteria(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //Week wise report
        //GetWeekWiseReportWithPagingAndCriteria
        //WeekWiseReport

        public Dictionary<long, IList<ContingentWeekWiseReport_vw>> GetWeekWiseReportListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetWeekWiseReportListWithPagingAndCriteria(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        //GetWeekWiseFOPListtWithPagingAndCriteria
        public Dictionary<long, IList<Vw_DistinctValuesFOP>> GetWeekWiseFOPListtWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetWeekWiseFOPListtWithPagingAndCriteria(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        //GetWeekWiseFinalOutputListtWithPagingAndCriteria
        public IList<Vw_WeekWiseFinalOutPut> GetWeekWiseFinalOutputListtWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetWeekWiseFinalOutputListtWithPagingAndCriteria(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        //GetItemMasterListWithPagingAndCriteriaForAutoComplete
        #endregion


        //Added By Gobi 
        #region Temp Order Item Report for vw_OrderItemsReport
        public Dictionary<long, IList<vw_OrderItemsReport>> TempGetOrderListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.TempGetOrderListWithPagingAndCriteria(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Dictionary<long, IList<vw_OrderItemsReport>> TempGetOrderListWithLikeSearchPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.TempGetOrderListWithLikeSearchPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion


        #region Imported delivery note related code

        //SaveOrUpdateImportedDeliveryNote
        public long SaveOrUpdateImportedDeliveryNote(ImportedDeliveryNote impdelnote)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.SaveOrUpdateImportedDeliveryNote(impdelnote);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //get order object by controlid

        public Orders GetOrderByControlId(string ControlId)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetOrderByControlId(ControlId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Save or update for imported delivery note items

        public decimal SaveOrUpdateImportedDeliveryNoteItems(ImportedDeliveryNoteItems impdelnoteitems)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.SaveOrUpdateImportedDeliveryNoteItems(impdelnoteitems);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Imported delivery note list jqgrid

        public Dictionary<long, IList<ImportedDeliveryNote>> GetImportedDeliveryNoteListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetImportedDeliveryNoteListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Get ImportedDeliveryNote details by importeddeliverynoteid

        public ImportedDeliveryNote GetImportedDeliveryNoteDetailsbyImpDeliveryNoteId(long ImpDeliveryNoteId)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetImportedDeliveryNoteDetailsbyImpDeliveryNoteId(ImpDeliveryNoteId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Imported deliverynote items list jqgrid


        public Dictionary<long, IList<ImportedDeliveryNoteItems>> GetImportedDeliveryNoteItemsListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetImportedDeliveryNoteItemsListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Get delivery note details by deliverynote name

        public DeliveryNote GetDeliverynoteDetailsByDeliveryNoteName(string DeliveryNoteName)
        {
            try
            {
                OrdersBC ordersBC = new OrdersBC();
                return ordersBC.GetDeliverynoteDetailsByDeliveryNoteName(DeliveryNoteName);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Getting invoice value and invoiceqty from InvQtyandInvValForOrderItems_vw

        public Dictionary<long, IList<InvQtyandInvValForOrderItems_vw>> GetInvQtyInvVaListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetInvQtyInvVaListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Delete the order by orderid

        // 
        public void DeleteOrderbyOrderObj(Orders ord)
        {
            try
            {
                OrdersBC ordersBC = new OrdersBC();
                ordersBC.DeleteOrderbyOrderObj(ord);
            }
            catch (Exception ex)
            { throw ex; }


        }

        public void DeleteOrdersOrdItemsPodPodItemsDelNoteByOrderId(long OrderId)
        {
            try
            {

                OrdersBC ordersBC = new OrdersBC();

                ordersBC.DeleteOrdersOrdItemsPodPodItemsDelNoteByOrderId(OrderId);

            }

            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region new changes
        //save or update mehtod for Insight report



        public long SaveOrUpdateInsightReport(InsightReport insreport)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.SaveOrUpdateInsightReport(insreport);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Saveorupdateimporteddeliverynoteitemslist
        public void SaveOrUpdateImportedDeliveryNoteItemsList(IList<ImportedDeliveryNoteItems> importeddeliverynoteitems)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                OrdersBC.SaveOrUpdateImportedDeliveryNoteItemsList(importeddeliverynoteitems);

            }
            catch (Exception ex)
            {

                throw;
            }


        }
        //Delete the deliverynoteandimported deliverynote by deliverynoteid
        public void DeleteDeliveryNoteandImportedDeliveryNoteTbl(long DeliveryNoteId, string DeliveryNoteNames)
        {
            try
            {

                OrdersBC OrdersBC = new OrdersBC();
                OrdersBC.DeleteDeliveryNoteandImportedDeliveryNoteTbl(DeliveryNoteId, DeliveryNoteNames);
            }
            catch (Exception ex)
            {
                throw;

            }

        }
        //Delete poditems based on the deliverynoteid
        public void DeletePODItemsByDeliveryNoteId(long DeliveryNoteId)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                OrdersBC.DeletePODItemsByDeliveryNoteId(DeliveryNoteId);

            }
            catch (Exception ex)
            {

                throw;
            }

        }

        #endregion
        #region

        public Dictionary<long, IList<InsightReport>> InsightReportListWithLikeSearchPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.InsightReportListWithLikeSearchPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Added By John
        public Dictionary<long, IList<OrderItemMismatch>> OrderItemMismatchReportListWithLikeSearchPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.OrderItemMismatchReportListWithLikeSearchPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        public bool SaveDeletedListofOrdersList(IList<OrdersDel> orderDellist, IList<OrderItemsDel> orderDelItemlist, IList<PODDel> podDelList, IList<PODItemsDel> podItemDelList, IList<DeliveryNoteDel> deliveryNoteDelList, IList<InvoiceDel> invoiceDelList)
        {
            try
            {

                OrdersBC ordersBC = new OrdersBC();
                return ordersBC.SaveDeletedListofOrdersList(orderDellist, orderDelItemlist, podDelList, podItemDelList, deliveryNoteDelList, invoiceDelList);

            }

            catch (Exception ex)
            {
                throw ex;
            }
        }


        public bool DeletedListofOrdersList(IList<Orders> orderlist, IList<OrderItems> orderItemlist, IList<POD> podList, IList<PODItems> podItemList, IList<DeliveryNote> deliveryNoteList, IList<Entities.InvoiceEntities.Invoice> invoiceList)
        {
            try
            {

                OrdersBC ordersBC = new OrdersBC();
                return ordersBC.DeletedListofOrdersList(orderlist, orderItemlist, podList, podItemList, deliveryNoteList, invoiceList);

            }

            catch (Exception ex)
            {
                throw ex;
            }
        }



        #region Get Ident_Current
        public long GetCurrentIntent(string table)
        {
            try
            {
                OrdersBC OrderBC = new OrdersBC();
                long a = OrderBC.GetCurrentIntent(table);
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


        #region upload request related code

        public long SaveOrUpdateUploadRequest(UploadRequest upreq)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.SaveOrUpdateUploadRequest(upreq);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //Get upload request by request id

        public UploadRequest GetUploadRequestbyRequestId(long RequestId)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetUploadRequestbyRequestId(RequestId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public long SaveOrUpdateUploadRequestDetailsLog(UploadRequestDetailsLog uploadlog)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.SaveOrUpdateUploadRequestDetailsLog(uploadlog);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //Get upload request by request id

        public UploadRequestDetailsLog GetUploadRequestDetailsLogbyRequestId(long RequestId)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetUploadRequestDetailsLogbyRequestId(RequestId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Calling DeliveryNoteUpload stored procedure by ImportedDeliveryNoteId

        public void CallDeliveryNoteUpload_SP(decimal ImpDelliveryNoteId)
        {

            try
            {
                OrdersBC ordersBC = new OrdersBC();
                ordersBC.CallDeliveryNoteUpload_SP(ImpDelliveryNoteId);


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //save or update the insight file uploads

        public long SaveOrUpdateInsightUploads(InsightFileUploads fileupload)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.SaveOrUpdateInsightUploads(fileupload);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Session related changes
        public void SaveOrUpdateOrdersUsingSession(Orders ord, IList<OrderItems> Orderitemslist)
        {

            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                OrdersBC.SaveOrUpdateOrdersUsingSession(ord, Orderitemslist);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //list the uploadrequest in jqgrid
        public Dictionary<long, IList<UploadRequest>> GetUploadRequestCountListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortBy, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetUploadRequestCountListWithPagingAndCriteria(page, pageSize, sortType, sortBy, criteria);
            }
            catch (Exception)
            {
                throw;
            }
            finally { }
        }
        //listing the UploadRequestDetailsLog table in jqgrid
        public Dictionary<long, IList<UploadRequestDetailsLog>> GetUploadRequestDetailsLogCountListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortBy, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetUploadRequestDetailsLogCountListWithPagingAndCriteria(page, pageSize, sortType, sortBy, criteria);
            }
            catch (Exception)
            {
                throw;
            }
            finally { }
        }
        #endregion

        public Dictionary<long, IList<Vw_WeekWiseFinalOutPut>> GetWeekWiseFinalOutPutListWithDictionaryPagingAndCriteriaAnd(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrderBC = new OrdersBC();
                return OrderBC.GetWeekWiseFinalOutPutListWithDictionaryPagingAndCriteriaAnd(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //save or update importeddeliverynote and importeddeliverynoteitems in a single session

        public void SaveorUpdateDeliveryNoteInSingleSession(ImportedDeliveryNote impdel, IList<ImportedDeliveryNoteItems> importeddeliverynoteitems)
        {

            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                OrdersBC.SaveorUpdateDeliveryNoteInSingleSession(impdel, importeddeliverynoteitems);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Save or update the imported expected delivery date in a single session

        public void SaveOrUpdateImportedExpDelDateListInSingleSession(IList<ImportedExpDelDate> expdeldtlist)
        {

            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                OrdersBC.SaveOrUpdateImportedExpDelDateListInSingleSession(expdeldtlist);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Imported expected delivery date list jqgrid

        public Dictionary<long, IList<ImportedExpDelDate>> GetImprtedDeliveryDateListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrderBC = new OrdersBC();
                return OrderBC.GetImprtedDeliveryDateListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Save or update the substitution master by sending the list

        public bool SaveOrUpdateSubstitutionMaster(IList<SubstitutionMaster> SubmstList)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.SaveOrUpdateSubstitutionMaster(SubmstList);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Substitution master jqgrid


        public Dictionary<long, IList<SubstitutionMaster>> GetSubstitutionMasterListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria, Dictionary<string, object> likeSearchCriteria)
        {
            try
            {
                OrdersBC OrderBC = new OrdersBC();
                return OrderBC.GetSubstitutionMasterListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria, likeSearchCriteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Substitution master like search based on the controlid 

        public Dictionary<long, IList<SubstitutionMaster>> GetSubstitutionMasterListWithLikeSearchPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetSubstitutionMasterListWithLikeSearchPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }


        //invoice qty and delivery sector for the poditems table

        public Dictionary<long, IList<InvoiceQtyDelSector_PODItems>> GetPODItems_InvQtyandDeliverySectorListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrderBC = new OrdersBC();
                return OrderBC.GetPODItems_InvQtyandDeliverySectorListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Dictionary<long, IList<OrderItems>> GetOrderItemsListWithNotInSearchPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, string ColumnName, int[] ExpId, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetOrderItemsListWithNotInSearchPagingAndCriteria(page, pageSize, sortby, sortType, ColumnName, ExpId, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Deleting the substitution master based on requestid 

        public void DeleteSubstitutionMasterByRequestId(string RequestIds)
        {
            try
            {
                OrdersBC ordersBC = new OrdersBC();
                ordersBC.DeleteSubstitutionMasterByRequestId(RequestIds);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void DeleteExpectedDeliveryDatebyRequestId(string RequestIds)
        {
            try
            {
                OrdersBC ordersBC = new OrdersBC();
                ordersBC.DeleteExpectedDeliveryDatebyRequestId(RequestIds);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Dictionary<long, IList<GCCRevised_vw>> GetGCCRevisedListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrderBC = new OrdersBC();
                return OrderBC.GetGCCRevisedListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Dictionary<long, IList<OrderitemOrderCombineView>> GetOrderitemOrderCombineViewListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrderBC = new OrdersBC();
                return OrderBC.GetOrderitemOrderCombineViewListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void SaveOrUpdateGCCRevisedList(IList<GCCRevised> revisedlist)
        {

            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                OrdersBC.SaveOrUpdateGCCRevisedList(revisedlist);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SaveOrUpdateGCCRevisedUsingSession(string Period, string PeriodYear)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                OrdersBC.SaveOrUpdateGCCRevisedUsingSession(Period, PeriodYear);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateUsingQuries(string Query)
        {
            try
            {
                OrdersBC ordersBC = new OrdersBC();
                ordersBC.UpdateUsingQuries(Query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Dictionary<long, IList<GCCRevised>> GetGCCRevisedUploadListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria, Dictionary<string, object> likeSearchCriteria)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetGCCRevisedUploadListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria, likeSearchCriteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #region Delete GccRevisedDN Items
        //Delete the deliverynoteandPODItems by deliverynoteid
        public void DeleteDeliveryNoteandPODItems(long DeliveryNoteId, string DeliveryNoteNames)
        {
            try
            {

                OrdersBC OrdersBC = new OrdersBC();
                OrdersBC.DeleteDeliveryNoteandPODItems(DeliveryNoteId, DeliveryNoteNames);
            }
            catch (Exception ex)
            {
                throw;

            }

        }
        //Delete GCCRevisedDn from UploadRequest by RequestID
        public void DeleteRevisedDNByRequestId(long RequestId)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                OrdersBC.DeleteRevisedDNByRequestId(RequestId);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion
        #region OrderItemsReport_SP added by Thamizhmani
        public IList<OrderItemsReport_SP> GetOrderItemsReport(string Period, string PeriodYear)
        {
            OrdersBC ordersBC = new OrdersBC();
            return ordersBC.GetOrderItemsReport(Period, PeriodYear);
        }
        #endregion



        #region DeliveryNoteBasedReport
        public Dictionary<long, IList<DeliveryNoteBasedReport_Vw>> GetDeliveryNoteBasedReportListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC ordersBC = new OrdersBC();
                return ordersBC.GetDeliveryNoteBasedReportListWithPagingAndCriteria(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception Ex)
            {

                throw Ex;
            }

        }
        public DeliveryNoteBasedReport_Vw GetDeliveryNoteListById(long Id)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetDeliveryNoteListById(Id);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        #region

        public long SaveOrUpdateGccRevisedFiles(UploadGccRevisedRequest UploadGcc)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.SaveOrUpdateGccRevisedFiles(UploadGcc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public Dictionary<long, IList<GCCRevised>> GetGccRevisedListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetGccRevisedListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public void CallGccRevisedUpload_SP(string Period, string PeriodYear, string ControlId, string DeliveryNoteName)
        {

            try
            {
                OrdersBC ordersBC = new OrdersBC();
                ordersBC.CallGccRevisedUpload_SP(Period, PeriodYear, ControlId, DeliveryNoteName);


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region GCCRevised by kingston

        public Dictionary<long, IList<GCCRevised_vw>> GetGCCRevisedListbySP(string Period, string PeriodYear)
        {
            try
            {
                OrdersBC ordersBC = new OrdersBC();
                return ordersBC.GetGCCRevisedListbySP(Period, PeriodYear);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Delete the GCCRevised version
        public void DeleteGCCRevisedVersion(string Period, string PeriodYear)
        {
            try
            {
                OrdersBC ordersBC = new OrdersBC();
                ordersBC.DeleteGCCRevisedVersion(Period, PeriodYear);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
        #region FinalFoodOrder Report
        public Dictionary<long, IList<FinalFoodOrderDetails_vw>> GetFinalOrderListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC ordersBC = new OrdersBC();
                return ordersBC.GetFinalOrderListWithPagingAndCriteria(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception Ex)
            {

                throw Ex;
            }

        }
        public Dictionary<long, IList<FinalFoodOrderdetails_SP>> GetFinalFoodOrderListbySP(string Period, string PeriodYear)
        {
            try
            {
                OrdersBC ordersBC = new OrdersBC();
                return ordersBC.GetFinalFoodOrderListbySP(Period, PeriodYear);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Dictionary<long, IList<ORDRPT_FinalFoodOrderReport_SP>> GetFinalFoodOrderRequistionListbySP(string Period, string PeriodYear, long Week)
        {
            try
            {
                OrdersBC ordersBC = new OrdersBC();
                return ordersBC.GetFinalFoodOrderRequistionListbySP(Period, PeriodYear, Week);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Weekly consolidate

        public Dictionary<long, IList<ORDRPT_ConsolidatedWeekReport>> GetORDRPT_WeekConsolidateList(string Period, string PeriodYear, long Week1, long Week2, long Week3, long Week4)
        {
            try
            {
                OrdersBC ordersBC = new OrdersBC();
                return ordersBC.GetORDRPT_WeekConsolidateList(Period, PeriodYear, Week1, Week2, Week3, Week4);

            }
            catch (Exception ex)
            {

                // ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }



        }

        public Dictionary<long, IList<ORDRPT_ConsolidateWeekReport_vw>> GetORDRPT_ConsolidateWeekReport_vwListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC ordersBC = new OrdersBC();
                return ordersBC.GetORDRPT_ConsolidateWeekReport_vwListWithPagingAndCriteria(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception Ex)
            {

                throw Ex;
            }

        }

        public Dictionary<long, IList<ORDRPT_BulkFoodOrder_SP>> GetORDRPT_BulkFoodOrderList(string Period, string PeriodYear)
        {
            try
            {
                OrdersBC ordersBC = new OrdersBC();
                return ordersBC.GetORDRPT_BulkFoodOrderList(Period, PeriodYear);

            }
            catch (Exception ex)
            {

                // ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }



        }

        public Dictionary<long, IList<ORDRPT_AnalysisSP>> GetORDRPT_AnalysisSPList(string Period, string PeriodYear)
        {
            try
            {
                OrdersBC ordersBC = new OrdersBC();
                return ordersBC.GetORDRPT_AnalysisSPList(Period, PeriodYear);

            }
            catch (Exception ex)
            {

                // ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }



        }


        public Dictionary<long, IList<ORDRPT_FinalReport_SP>> GetORDRPT_FinalReportList(string Period, string PeriodYear, long Week1, long Week2, long Week3, long Week4)
        {
            try
            {
                OrdersBC ordersBC = new OrdersBC();
                return ordersBC.GetORDRPT_FinalReportList(Period, PeriodYear, Week1, Week2, Week3, Week4);

            }
            catch (Exception ex)
            {

                // ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }


        public Dictionary<long, IList<ORDRPT_Summary>> GetSummaryList(string Period, string PeriodYear)
        {
            try
            {
                OrdersBC ordersBC = new OrdersBC();
                return ordersBC.GetSummaryList(Period, PeriodYear);

            }
            catch (Exception ex)
            {

                // ExceptionPolicy.HandleException(ex, "InsightOrderPolicy");
                throw ex;
            }
        }
        public void DeleteConsolidateFoodReport(string Period, string PeriodYear)
        {

            try
            {
                OrdersBC ordersBC = new OrdersBC();
                ordersBC.DeleteConsolidateFoodReport(Period, PeriodYear);


            }
            catch (Exception ex)
            {
                throw ex;
            }


        }



        public Dictionary<long, IList<ORDRPT_SummaryTroops>> GetORDRPT_SummaryTroopsWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC ordersBC = new OrdersBC();
                return ordersBC.GetORDRPT_SummaryTroopsWithPagingAndCriteria(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception Ex)
            {

                throw Ex;
            }

        }
        #endregion
        #endregion

        #region Initial Bulk orders upload
        public void SaveOrUpdateInitialOrdersUsingSession(InitialOrders ord, IList<InitialOrderItems> Orderitemslist)
        {

            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                OrdersBC.SaveOrUpdateInitialOrdersUsingSession(ord, Orderitemslist);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public long SaveOrUpdateInitialOrder(InitialOrders Orders)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.SaveOrUpdateInitialOrder(Orders);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Dictionary<long, IList<InitialOrders>> GetInitialOrdersListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetInitialOrdersListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }


        #endregion


        #region  Local Pruchase

        public Dictionary<long, IList<LocalPurchase>> GetLocalPurchaseFilesListWithPagingAndCriteria(int? page, int? pagesize, string sortby, string sorttype, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC ordersBC = new OrdersBC();
                return ordersBC.GetLocalPurchaseFilesListWithPagingAndCriteria(page, pagesize, sortby, sorttype, criteria);
            }
            catch (Exception)
            {
                throw;
            }

        }

        //Local purchase FFO upload
        public void InsertLocalPurchaseFFObySP(long RequestId)
        {

            try
            {
                OrdersBC ordersBC = new OrdersBC();
                ordersBC.InsertLocalPurchaseFFObySP(RequestId);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public bool SaveorUpdateOrders_LocalPurchaseByList(IList<Orders_LocalPurchase> listOfItems)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.SaveorUpdateOrders_LocalPurchaseByList(listOfItems);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Error Log
        public long SaveOrUpdateErrorLog(ErrorLog errlog)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.SaveOrUpdateErrorLog(errlog);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region Invoice number Master
        //Save or update the InvoiceNumber master by sending the list

        public bool SaveOrUpdateInvoiceNumberMaster(IList<InvoiceNumberMaster> invMstlist)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.SaveOrUpdateInvoiceNumberMaster(invMstlist);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //InvoiceNumber master like search based on the controlid 

        public Dictionary<long, IList<InvoiceNumberMaster>> GetInvoiceNumberMasterListWithLikeSearchPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetInvoiceNumberMasterListWithLikeSearchPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //InvoiceNumber master jqgrid


        public Dictionary<long, IList<InvoiceNumberMaster>> GetInvoiceNumberMasterListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria, Dictionary<string, object> likeSearchCriteria)
        {
            try
            {
                OrdersBC OrderBC = new OrdersBC();
                return OrderBC.GetInvoiceNumberMasterListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria, likeSearchCriteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //get Invoie Number  by OrderId
        public InvoiceNumberMaster GetInvoiceNumberByOrderId(long OrderId)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetInvoiceNumberByOrderId(OrderId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        //public IList<UploadRequest> GetUploadRequestbyRequestId(long RequestIds)
        //{
        //    try
        //    {
        //        OrdersBC ordersBC = new OrdersBC();
        //        return ordersBC.GetUploadRequestbyRequestId(RequestIds);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    finally { }
        //}
        //Delete InvoiceSequence and UploadRequest by RequestID
        public void DeleteInvoiceSequenceandUploadRequestbyRequestId(long RequestId)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                OrdersBC.DeleteInvoiceSequenceandUploadRequestbyRequestId(RequestId);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #region NPA Master Upload
        public bool SaveOrUpdateNPAMasterList(IList<NPAMaster> NPAlist)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.SaveOrUpdateNPAMasterList(NPAlist);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<long, IList<NPAMaster>> GetNPAMasterListWithLikeSearchPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetNPAMasterListWithLikeSearchPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<long, IList<NPAMaster>> GetNPAMasterListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria, Dictionary<string, object> likeSearchCriteria)
        {
            try
            {
                OrdersBC OrderBC = new OrdersBC();
                return OrderBC.GetNPAMasterListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria, likeSearchCriteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void UpdateNPAMasterSpbyRequestId(long RequestId)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                OrdersBC.UpdateNPAMasterSpbyRequestId(RequestId);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void DeleteNPAMasterandUploadRequestbyRequestId(long RequestId)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                OrdersBC.DeleteNPAMasterandUploadRequestbyRequestId(RequestId);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public IList<NPAMaster> GetNPAMasterListbyRequestId(long RequestId)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetNPAMasterListbyRequestId(RequestId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region FIV
        public FIVItems GetFIVItemsByOrderId(long OrderId)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetFIVItemsByOrderId(OrderId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
      
        public Dictionary<long, IList<FIVItemsReport>> GetFIVItemsReportListbySP(string OrderId)
        {
            try
            {
                OrdersBC ordersBC = new OrdersBC();
                return ordersBC.GetFIVItemsReportListbySP(OrderId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void updateFIVStatusByOrderId(string OrderId)
        {
            OrdersBC ordersBC = new OrdersBC();
            ordersBC.updateFIVStatusByOrderId(OrderId);
        }
        public void UpdateLineIdinFIVItems_SPByOrderId(long OrderId)
        {

            try
            {
                OrdersBC ordersBC = new OrdersBC();
                ordersBC.UpdateLineIdinFIVItems_SPByOrderId(OrderId);


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Dictionary<long, IList<FIVItems_vw>> GetFIVItemsListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetFIVItemsListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<long, IList<FIVItemsReport>> GetFIVItemsReportListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                return OrdersBC.GetFIVItemsReportListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void DeleteFIVandUploadRequestbyFIVId(long RequestId)
        {
            try
            {
                OrdersBC OrdersBC = new OrdersBC();
                OrdersBC.DeleteFIVandUploadRequestbyFIVId(RequestId);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion
    }
}
