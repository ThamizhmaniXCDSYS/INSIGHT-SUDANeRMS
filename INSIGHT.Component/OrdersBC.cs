using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistenceFactory;
using INSIGHT.Entities;
using System.Collections;
using INSIGHT.Entities.DeletedRecordEntities;
using INSIGHT.Entities.InvoiceEntities;
using System.Data.SqlClient;
using INSIGHT.Entities.InputUploadEntities;
using INSIGHT.Entities.ReportEntities;
namespace INSIGHT.Component
{
    public class OrdersBC
    {
        PersistenceServiceFactory PSF = null;
        ProjectSpecificPSF SPSF = null;
        //   PSFForAnotherDatabase PSF = null;

        public OrdersBC()
        {
            List<String> Assembly = new List<String>();
            Assembly.Add("INSIGHT.Entities");
            Assembly.Add("INSIGHT.Entities.TicketingSystem");
            PSF = new PersistenceServiceFactory(Assembly);
            // PSF = new PSFForAnotherDatabase(Assembly, "Data source=KINGSTON;initial catalog=INSIGHT_DESO;Integrated Security=True");
            SPSF = new ProjectSpecificPSF(Assembly);

        }
        public Orders GetOrdersById(long OrderId)
        {
            try
            {
                return PSF.Get<Orders>("OrderId", OrderId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public long SaveOrUpdateOrder(Orders Orders)
        {
            try
            {
                if (Orders != null)
                    PSF.SaveOrUpdate<Orders>(Orders);
                else { throw new Exception("Orders is required and it cannot be null.."); }
                return Orders.OrderId;
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
        //        bool retValue = false;
        //        if (listOfItems != null && listOfItems.Count > 0)
        //        {
        //            PSF.SaveOrUpdate<OrderItems>(listOfItems);
        //            retValue = true;
        //        }
        //        else { throw new Exception("list Of Items is required and it cannot be null.."); }
        //        return retValue;
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
                return PSF.GetListWithEQSearchCriteriaCount<Orders>(page, pageSize, sortType, sortby, criteria);
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
                return PSF.GetListWithEQSearchCriteriaCount<OrderItems>(page, pageSize, sortType, sortby, criteria);
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
                // return PSF.GetListWithEQSearchCriteriaCount<OrderItems>(page, pageSize, sortType, sortby, criteria);
                return PSF.GetListWithLikeSearchCriteriaCount<OrderItems>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
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



        public Dictionary<long, IList<OrdersPerMonth_vw>> GetOrdersPerMonth_vwListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortBy, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithSearchCriteriaCount<OrdersPerMonth_vw>(page, pageSize, sortType, sortBy, criteria);
            }

            catch (Exception)
            {
                throw;
            }
        }

        public Dictionary<long, IList<OrderItemsCountAndMismatchCount_vw>> GetOrderItemsCountListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortBy, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithSearchCriteriaCount<OrderItemsCountAndMismatchCount_vw>(page, pageSize, sortType, sortBy, criteria);
            }

            catch (Exception)
            {
                throw;
            }
        }

        public Dictionary<long, IList<PODItemsCount_vw>> GetPOdItemsCount_vwListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortBy, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithSearchCriteriaCount<PODItemsCount_vw>(page, pageSize, sortType, sortBy, criteria);
            }

            catch (Exception)
            {
                throw;
            }
        }
        #region Delivery Reports
        public Dictionary<long, IList<PODItems>> GetPodItemsListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<PODItems>(page, pageSize, sortType, sortby, criteria);
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
                return PSF.Get<PODItems>("PODItemsId", PODItemsId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public long SaveOrUpdatePODItems(PODItems poditems)
        {
            try
            {
                if (poditems != null)
                    PSF.SaveOrUpdate<PODItems>(poditems);
                else { throw new Exception("Orders is required and it cannot be null.."); }
                return poditems.OrderId;
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
                return PSF.Get<DeliveryNote>(DeliveryNoteId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion end Delivery Reports


        #region kingston POD Related code
        public long SaveOrUpdateOrderItems(OrderItems orditems)
        {
            try
            {
                if (orditems != null)
                    PSF.SaveOrUpdate<OrderItems>(orditems);
                else { throw new Exception("Orders is required and it cannot be null.."); }
                return orditems.LineId;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool SaveOrUpdateOrderItemsList(IList<OrderItems> orditems)
        {
            bool retValue = false;
            try
            {
                if (orditems != null)
                {
                    PSF.SaveOrUpdate<OrderItems>(orditems);
                    retValue = true;
                }
                else { throw new Exception("Orders is required and it cannot be null.."); }
                return retValue;
            }

            catch (Exception)
            {
                throw;
            }
        }
        //PODMAster JQGrid
        public Dictionary<long, IList<POD>> GetPODMasterListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<POD>(page, pageSize, sortType, sortby, criteria);
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
                return PSF.GetListWithEQSearchCriteriaCount<PODOrderItems_vw>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }


        }
        //get POD table details by PODID
        public POD GetPODById(long PODId)
        {
            try
            {
                return PSF.Get<POD>("PODId", PODId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //save or update pod master
        public long SaveOrUpdatePOD(POD pod)
        {
            try
            {
                if (pod != null)
                    PSF.SaveOrUpdate<POD>(pod);
                else { throw new Exception("Orders is required and it cannot be null.."); }
                return pod.PODId;
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
                return PSF.Get<OrderItems>("LineId", LineId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Delivered PODItems jqgrid

        public Dictionary<long, IList<DeliveredPODItems_vw>> GetDeliveredPODItemsListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<DeliveredPODItems_vw>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }


        }
        //Delivered poditem jqgrid like search
        public Dictionary<long, IList<DeliveredPODItems_vw>> GetDeliveredPODItemsListWithLikePagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithLikeSearchCriteriaCount<DeliveredPODItems_vw>(page, pageSize, sortby, sortType, criteria);
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
                return PSF.Get<POD>("OrderId", OrderId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Dictionary<long, IList<PODItems>> GetPODItemsListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<PODItems>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void updateOrders(long OrderId, DateTime? ExpectedDeliveryDate)
        {
            try
            {

                string query = "update Orders set ExpectedDeliveryDate='" + ExpectedDeliveryDate + "' where OrderId='" + OrderId + "'";
                PSF.ExecuteSql(query);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //creating delivery note


        public long SaveOrUpdateDeliveryNote(DeliveryNote dn)
        {
            try
            {
                if (dn != null)
                    PSF.SaveOrUpdate<DeliveryNote>(dn);
                else { throw new Exception("Delivery note is required and it cannot be null.."); }
                return dn.DeliveryNoteId;
            }
            catch (Exception)
            {
                throw;
            }
        }
        //updating DeliveryNote id in PODItemsTable

        public void updateDeliveryNoteIdinPODItemsTbl(long DeliveryNoteId, string DeliveryNoteName, string SelPODItemsId, string DeliverySector)
        {
            try
            {
                string[] SelPODItemsIdArray = SelPODItemsId.Split(',');

                for (int i = 0; i < SelPODItemsIdArray.Length; i++)
                {
                    string query = "update PODItems set DeliveryNoteId=" + DeliveryNoteId + " , DeliverySector='" + DeliverySector + "' , DeliveryNoteName='" + DeliveryNoteName + "' where PODItemsId =" + Convert.ToInt64(SelPODItemsIdArray[i]);
                    PSF.ExecuteSql(query);
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        //GetDeliveryNoteListWithPagingAndCriteria
        public Dictionary<long, IList<DeliveryNote>> GetDeliveryNoteListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<DeliveryNote>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //updating the delivery qty cell


        public void UpdateDeliveredQtyCell(long LineId, Decimal DelQty, Decimal RemainingQty)
        {
            try
            {




                string query = "update OrderItems set DeliveredOrdQty=" + DelQty + ", RemainingOrdQty=" + RemainingQty + "where LineId =" + LineId;
                PSF.ExecuteSql(query);


            }
            catch (Exception)
            {
                throw;
            }
        }



        public void UpdateSubstituteItemNameCell(long LineId, string SubstituteItemName, long subitemcode, string DiscrepancyCode)
        {
            try
            {

                string query = "update OrderItems set SubstituteItemName='" + SubstituteItemName + "' ,SubstituteItemCode=" + subitemcode + " , DiscrepancyCode='" + DiscrepancyCode + "' where LineId =" + LineId;
                PSF.ExecuteSql(query);


            }
            catch (Exception)
            {
                throw;
            }
        }


        //pod orders list page


        public Dictionary<long, IList<PODOrdersList_vw>> GetPODOrdersListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<PODOrdersList_vw>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Get PODOrdersList_vw list 

        public PODOrdersList_vw GetPODOrdersListById(long PODId)
        {
            try
            {
                return PSF.Get<PODOrdersList_vw>("PODId", PODId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //PODOrderItems_vw like search for commodity
        public Dictionary<long, IList<PODOrderItems_vw>> GetPODOrderItemsListWithLikeSearchPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                // return PSF.GetListWithEQSearchCriteriaCount<OrderItems>(page, pageSize, sortType, sortby, criteria);
                return PSF.GetListWithLikeSearchCriteriaCount<PODOrderItems_vw>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Deleting the PODItems by object
        public void DeletePODItemsbyObj(PODItems poditems)
        {
            try
            {
                PSF.Delete(poditems);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //Delivery note list page

        public Dictionary<long, IList<DeliveryNoteOrders_vw>> GetDeliveryNoteOrderListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<DeliveryNoteOrders_vw>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Delivery note  list with delivery note name like search


        public Dictionary<long, IList<DeliveryNoteOrders_vw>> GetDeliveryNoteOrderListWithLikeSearchPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithLikeSearchCriteriaCount<DeliveryNoteOrders_vw>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Updating the substitute item details in PODItemsTable
        public void UpdateSubItemDetailsinPODItemsTbl(long GreatestPODItemsId, string SubstituteItemName, long subitemcode, string DiscrepancyCode)
        {
            try
            {

                string query = "update PODItems set SubstituteItemName='" + SubstituteItemName + "' ,SubstituteItemCode=" + subitemcode + " , DiscrepancyCode='" + DiscrepancyCode + "' where PODItemsId =" + GreatestPODItemsId;
                PSF.ExecuteSql(query);

            }
            catch (Exception ex) { throw ex; }
        }
        #endregion

        #region Added by arun order items report
        public Dictionary<long, IList<OrderItemsReport_vw>> GetOrderListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<OrderItemsReport_vw>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Week wise report

        public Dictionary<long, IList<ContingentWeekWiseReport_vw>> GetWeekWiseReportListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<ContingentWeekWiseReport_vw>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //GetWeekWiseFOPListtWithPagingAndCriteria
        public Dictionary<long, IList<Vw_DistinctValuesFOP>> GetWeekWiseFOPListtWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<Vw_DistinctValuesFOP>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //GetWeekWiseFinalOutputListtWithPagingAndCriteria
        public IList<Vw_WeekWiseFinalOutPut> GetWeekWiseFinalOutputListtWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                long[] val = { 0 };
                return PSF.GetList<Vw_WeekWiseFinalOutPut>(page, pageSize, sortType, sortby, string.Empty, val);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        //Added by Gobi for Order Items Report vw_OrderItemsReport
        #region Temp Order Items Report
        public Dictionary<long, IList<vw_OrderItemsReport>> TempGetOrderListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<vw_OrderItemsReport>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<long, IList<vw_OrderItemsReport>> TempGetOrderListWithLikeSearchPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithLikeSearchCriteriaCount<vw_OrderItemsReport>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region imported delivery note related code

        public long SaveOrUpdateImportedDeliveryNote(ImportedDeliveryNote impdelnote)
        {
            try
            {
                if (impdelnote != null)
                    PSF.SaveOrUpdate<ImportedDeliveryNote>(impdelnote);
                else { throw new Exception("Orders is required and it cannot be null.."); }
                return impdelnote.ImpDeliveryNoteId;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Get order object by controlid

        public Orders GetOrderByControlId(string ControlId)
        {
            try
            {
                return PSF.Get<Orders>("ControlId", ControlId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Save or update the imported delivery note items details

        public decimal SaveOrUpdateImportedDeliveryNoteItems(ImportedDeliveryNoteItems impdelnoteitems)
        {
            try
            {
                if (impdelnoteitems != null)
                    PSF.SaveOrUpdate<ImportedDeliveryNoteItems>(impdelnoteitems);
                else { throw new Exception("ImportedDeliveryNoteItems is required and it cannot be null.."); }
                return impdelnoteitems.ImpDeliveryNoteId;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public Dictionary<long, IList<ImportedDeliveryNote>> GetImportedDeliveryNoteListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<ImportedDeliveryNote>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Get importeddeliverynote details by importeddeliverynoteid

        public ImportedDeliveryNote GetImportedDeliveryNoteDetailsbyImpDeliveryNoteId(long ImpDeliveryNoteId)
        {
            try
            {
                return PSF.Get<ImportedDeliveryNote>("ImpDeliveryNoteId", ImpDeliveryNoteId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Imported delivery note items list jqgrid


        public Dictionary<long, IList<ImportedDeliveryNoteItems>> GetImportedDeliveryNoteItemsListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<ImportedDeliveryNoteItems>(page, pageSize, sortType, sortby, criteria);
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
                return PSF.Get<DeliveryNote>("DeliveryNoteName", DeliveryNoteName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Getting invoice value and invoiceqty from InvQtyandInvValForOrderItems_vw
        //
        public Dictionary<long, IList<InvQtyandInvValForOrderItems_vw>> GetInvQtyInvVaListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<InvQtyandInvValForOrderItems_vw>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //deleting orders by orderobj
        public void DeleteOrderbyOrderObj(Orders ord)
        {
            try
            {
                PSF.Delete(ord);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public void DeleteOrdersOrdItemsPodPodItemsDelNoteByOrderId(long OrderId)
        {
            try
            {

                string deleteorder = "update  Orders set ActiveStatus='" + "InActive" + "' where OrderId=" + OrderId;
                PSF.ExecuteSql(deleteorder);
                //string deleteorderitems = "Delete from OrderItems where OrderId=" + OrderId;
                //PSF.ExecuteSql(deleteorderitems);
                //string deletepod = "Delete from POD where OrderId=" + OrderId;
                //PSF.ExecuteSql(deletepod);
                //string deletepoditems = "Delete from PODItems where OrderId=" + OrderId;
                //PSF.ExecuteSql(deletepoditems);
                //string deletedeliverynote = "Delete from DeliveryNote where OrderId=" + OrderId;
                //PSF.ExecuteSql(deletedeliverynote);
            }
            catch (Exception ex) { throw ex; }
        }


        #endregion

        #region new changes
        //save method for insight report


        public long SaveOrUpdateInsightReport(InsightReport insreport)
        {
            try
            {
                if (insreport != null)
                    PSF.SaveOrUpdate<InsightReport>(insreport);
                else { throw new Exception("Insight report is required and it cannot be null.."); }
                return insreport.ReportId;
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Save importeddeliverynoteitemslist 
        public void SaveOrUpdateImportedDeliveryNoteItemsList(IList<ImportedDeliveryNoteItems> importeddeliverynoteitems)
        {

            try
            {

                if (importeddeliverynoteitems != null)
                    PSF.SaveOrUpdate<ImportedDeliveryNoteItems>(importeddeliverynoteitems);
                else { throw new Exception("importeddeliverynoteitems  is required and it cannot be null.."); }

            }
            catch (Exception ex)
            {
                throw;
            }

        }
        //Delete deliverynote and imported deliverynote
        public void DeleteDeliveryNoteandImportedDeliveryNoteTbl(long DeliveryNoteId, string DeliveryNoteName)
        {
            try
            {

                string deletedeliverynote = "Delete DeliveryNote where  DeliveryNoteId=" + DeliveryNoteId;

                PSF.ExecuteSql(deletedeliverynote);
                string deleteimpdeliverynote = "Delete ImportedDeliveryNote where  ImpDeliveryNoteName='" + DeliveryNoteName + "'";
                PSF.ExecuteSql(deleteimpdeliverynote);
                string deleteimpdeliverynoteItems = "Delete ImportedDeliveryNoteItems where  ImpDeliveryNoteName='" + DeliveryNoteName + "'";
                PSF.ExecuteSql(deleteimpdeliverynoteItems);
                string deleteinsightreport = "Delete InsightReport  where  DeliveryNoteName='" + DeliveryNoteName + "'";
                PSF.ExecuteSql(deleteinsightreport);
                string deleteGCCReviseditems = "Delete GCCRevised where  DeliveryNoteName='" + DeliveryNoteName + "'";
                PSF.ExecuteSql(deleteGCCReviseditems);
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        //Delete poditems by deliverynoteid
        public void DeletePODItemsByDeliveryNoteId(long DeliveryNoteId)
        {
            try
            {
                string delPODItemsByDelnoteId = "Delete  from PODItems where DeliveryNoteId=" + DeliveryNoteId;
                PSF.ExecuteSql(delPODItemsByDelnoteId);
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
                return PSF.GetListWithLikeSearchCriteriaCount<InsightReport>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Added By John
        public Dictionary<long, IList<OrderItemMismatch>> OrderItemMismatchReportListWithLikeSearchPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithLikeSearchCriteriaCount<OrderItemMismatch>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        public bool SaveDeletedListofOrdersList(IList<OrdersDel> orderDellist, IList<OrderItemsDel> orderDelItemlist, IList<PODDel> podDelList, IList<PODItemsDel> podItemDelList, IList<DeliveryNoteDel> deliveryNoteDelList, IList<InvoiceDel> invoiceDelList)
        {
            try
            {
                if (orderDellist.Count() != 0)
                    PSF.Save<OrdersDel>(orderDellist);
                if (orderDellist.Count() != 0)
                    PSF.Save<OrderItemsDel>(orderDelItemlist);
                if (podDelList.Count() != 0)
                    PSF.Save<PODDel>(podDelList);
                if (podItemDelList.Count() != 0)
                    PSF.Save<PODItemsDel>(podItemDelList);
                if (deliveryNoteDelList.Count() != 0)
                    PSF.Save<DeliveryNoteDel>(deliveryNoteDelList);
                if (invoiceDelList.Count() != 0)
                    PSF.Save<InvoiceDel>(invoiceDelList);
                return true;
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
                if (orderlist.Count() != 0)
                    PSF.DeleteAll<Orders>(orderlist);
                if (orderItemlist.Count() != 0)
                    PSF.DeleteAll<OrderItems>(orderItemlist);
                if (podList.Count() != 0)
                    PSF.DeleteAll<POD>(podList);
                if (podItemList.Count() != 0)
                    PSF.DeleteAll<PODItems>(podItemList);
                if (deliveryNoteList.Count() != 0)
                    PSF.DeleteAll<DeliveryNote>(deliveryNoteList);
                if (invoiceList.Count() != 0)
                    PSF.DeleteAll<Invoice>(invoiceList);
                return true;
            }
            catch (Exception)
            {
                throw;
            }

        }

        #region Get Ident_Current
        public long GetCurrentIntent(string table)
        {
            try
            {
                string query = "select IDENT_CURRENT('" + table + "')";
                IList list = PSF.ExecuteSql(query);
                if (list != null && list[0] != null)
                {
                    return Convert.ToInt64(list[0].ToString()); //list[0] = "0";
                }
                else return 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion end

        #region bulk upload related code
        //
        public long SaveOrUpdateUploadRequest(UploadRequest upreq)
        {
            try
            {
                if (upreq != null)
                    PSF.SaveOrUpdate<UploadRequest>(upreq);
                else { throw new Exception("upreq is required and it cannot be null.."); }
                return upreq.RequestId;
            }
            catch (Exception)
            {
                throw;
            }
        }
        //get upload request by request id

        public UploadRequest GetUploadRequestbyRequestId(long RequestId)
        {
            try
            {
                return PSF.Get<UploadRequest>("RequestId", RequestId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        //
        public long SaveOrUpdateUploadRequestDetailsLog(UploadRequestDetailsLog uploadlog)
        {
            try
            {
                if (uploadlog != null)
                    PSF.SaveOrUpdate<UploadRequestDetailsLog>(uploadlog);
                else { throw new Exception("UploadRequestDetailsLog is required and it cannot be null.."); }
                return uploadlog.UploadReqDetLogId;
            }
            catch (Exception)
            {
                throw;
            }
        }
        //get upload request by request id

        public UploadRequestDetailsLog GetUploadRequestDetailsLogbyRequestId(long UploadReqDetLogId)
        {
            try
            {
                return PSF.Get<UploadRequestDetailsLog>("UploadReqDetLogId", UploadReqDetLogId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //calling DeliveryNoteUpload Stored procedure
        public void CallDeliveryNoteUpload_SP(decimal ImpDeliveryNoteId)
        {

            try
            {
                if (ImpDeliveryNoteId > 0)
                {
                    string query = "Exec DeliveryNoteUpload   " + ImpDeliveryNoteId;

                    PSF.ExecuteSql(query);

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //File upload related conde in Insightfileupload table

        public long SaveOrUpdateInsightUploads(InsightFileUploads fileupload)
        {
            try
            {
                if (fileupload != null)
                    PSF.SaveOrUpdate<InsightFileUploads>(fileupload);
                else { throw new Exception("fileupload is required and it cannot be null.."); }
                return fileupload.UploadFileId;
            }
            catch (Exception)
            {
                throw;
            }
        }
        //save or update orders using session
        public void SaveOrUpdateOrdersUsingSession(Orders ord, IList<OrderItems> Orderitemslist)
        {
            try
            {
                if (ord != null && Orderitemslist != null)
                {
                    SPSF.SaveOrUpdateOrdersUsingSession(ord, Orderitemslist);
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        //GetUploadRequestCountListWithPagingAndCriteria
        public Dictionary<long, IList<UploadRequest>> GetUploadRequestCountListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<UploadRequest>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //GetUploadRequestDetailsLogCountListWithPagingAndCriteria
        public Dictionary<long, IList<UploadRequestDetailsLog>> GetUploadRequestDetailsLogCountListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<UploadRequestDetailsLog>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        public Dictionary<long, IList<Vw_WeekWiseFinalOutPut>> GetWeekWiseFinalOutPutListWithDictionaryPagingAndCriteriaAnd(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<Vw_WeekWiseFinalOutPut>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void SaveorUpdateDeliveryNoteInSingleSession(ImportedDeliveryNote impdel, IList<ImportedDeliveryNoteItems> importeddeliverynoteitems)
        {
            try
            {
                if (impdel != null && importeddeliverynoteitems != null)
                {
                    SPSF.SaveorUpdateDeliveryNoteInSingleSession(impdel, importeddeliverynoteitems);
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        //Saveorupdate the imported expected delivery date in a single session

        public void SaveOrUpdateImportedExpDelDateListInSingleSession(IList<ImportedExpDelDate> expdeldtlist)
        {
            try
            {
                if (expdeldtlist != null)
                {
                    SPSF.SaveOrUpdateImportedExpDelDateListInSingleSession(expdeldtlist);
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        //imported expected delivery date jqgrid


        public Dictionary<long, IList<ImportedExpDelDate>> GetImprtedDeliveryDateListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<ImportedExpDelDate>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //save or update the substitution master by sending the list

        public bool SaveOrUpdateSubstitutionMaster(IList<SubstitutionMaster> subsmstlist)
        {
            try
            {
                bool retValue = false;
                if (subsmstlist != null && subsmstlist.Count > 0)
                {
                    PSF.SaveOrUpdate<SubstitutionMaster>(subsmstlist);
                    retValue = true;
                }
                else { throw new Exception("List of Substitution master is required and it cannot be null.."); }
                return retValue;
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
                // return PSF.GetListWithEQSearchCriteriaCount<SubstitutionMaster>(page, pageSize, sortType, sortby, criteria);
                return PSF.GetListWithExactAndLikeSearchCriteriaWithCount<SubstitutionMaster>(page, pageSize, sortType, sortby, criteria, likeSearchCriteria);
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
                return PSF.GetListWithLikeSearchCriteriaCount<SubstitutionMaster>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }


        }
        //invoice qty and deliverysector  in poditems table

        public Dictionary<long, IList<InvoiceQtyDelSector_PODItems>> GetPODItems_InvQtyandDeliverySectorListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<InvoiceQtyDelSector_PODItems>(page, pageSize, sortType, sortby, criteria);
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
                return PSF.GetListWithNotInWithEQSearchCriteriaCountArray<OrderItems>(page, pageSize, sortby, sortType, ColumnName, ExpId, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Deleting the substitution master based on the request id

        public void DeleteSubstitutionMasterByRequestId(string RequestIds)
        {
            try
            {

                string query = "Delete from UploadRequest where RequestId in (" + RequestIds + ")";
                PSF.ExecuteSql(query);
                string deletesubstitutionmaster = "Delete from SubstitutionMaster where RequestId in (" + RequestIds + ")";
                PSF.ExecuteSql(deletesubstitutionmaster);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Deleting the expected deliverydate based on the request id
        public void DeleteExpectedDeliveryDatebyRequestId(string RequestIds)
        {
            try
            {

                string query = "Delete from UploadRequest where RequestId in (" + RequestIds + ")";
                PSF.ExecuteSql(query);
                string deleteexpdeldate = "Delete from ImportedExpDelDate where RequestId in (" + RequestIds + ")";
                PSF.ExecuteSql(deleteexpdeldate);
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
                return PSF.GetListWithEQSearchCriteriaCount<GCCRevised_vw>(page, pageSize, sortType, sortby, criteria);
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
                return PSF.GetListWithEQSearchCriteriaCount<OrderitemOrderCombineView>(page, pageSize, sortType, sortby, criteria);
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
                if (revisedlist != null)
                {
                    SPSF.SaveOrUpdateGCCRevisedList(revisedlist);
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        public void SaveOrUpdateGCCRevisedUsingSession(string Period, string PeriodYear)
        {
            try
            {
                if (Period != "" && PeriodYear != "")
                {
                    SPSF.SaveOrUpdateGCCRevisedUsingSession(Period, PeriodYear);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void UpdateUsingQuries(string Query)
        {
            try
            {
                if (Query != "")
                    PSF.ExecuteSql(Query);
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
                return PSF.GetListWithExactAndLikeSearchCriteriaWithCount<GCCRevised>(page, pageSize, sortType, sortby, criteria, likeSearchCriteria);
            }
            catch (Exception)
            {
                throw;
            }
        }


        #region OrderItemsReport_SP Added by Thamizh
        public IList<OrderItemsReport_SP> GetOrderItemsReport(string Period, string PeriodYear)
        {
            try
            {
                string query = "Exec OrderItemsReport_SP @SPPeriod='" + Period + "' , @SPPeriodYear='" + PeriodYear + "'";
                IList list = PSF.ExecuteSql(query);
                IList<OrderItemsReport_SP> OrderItemsList = new List<OrderItemsReport_SP>();
                foreach (var item in list)
                {
                    OrderItemsReport_SP OIReports = new OrderItemsReport_SP();
                    OIReports.Deliveredvalue = Convert.ToDecimal(((object[])(item))[0]);
                    OIReports.Id = Convert.ToInt64(((object[])(item))[1]);
                    OIReports.OrderId = Convert.ToInt64(((object[])(item))[2]);
                    OIReports.ControlId = Convert.ToString(((object[])(item))[3]);
                    OIReports.Location = Convert.ToString(((object[])(item))[4]);
                    OIReports.Strength = Convert.ToInt64(((object[])(item))[5]);
                    OIReports.Noofdays = Convert.ToInt64(((object[])(item))[6]);
                    OIReports.Week = Convert.ToInt64(((object[])(item))[7]);
                    OIReports.Period = Convert.ToString(((object[])(item))[8]);
                    OIReports.PeriodYear = Convert.ToString(((object[])(item))[9]);
                    OIReports.Sector = Convert.ToString(((object[])(item))[10]);
                    OIReports.Contingent = Convert.ToString(((object[])(item))[11]);
                    OIReports.ContingentType = Convert.ToString(((object[])(item))[12]);
                    OIReports.Lineitemordered = Convert.ToInt64(((object[])(item))[13]);
                    OIReports.Totallineitemsubstituted = Convert.ToInt64(((object[])(item))[14]);
                    OIReports.Orderedqty = Convert.ToDecimal(((object[])(item))[15]);
                    OIReports.Deliveredqty = Convert.ToDecimal(((object[])(item))[16]);
                    OIReports.Acceptedqty = Convert.ToDecimal(((object[])(item))[17]);
                    OIReports.Amountordered = Convert.ToDecimal(((object[])(item))[18]);
                    OIReports.Amountaccepted = Convert.ToDecimal(((object[])(item))[19]);
                    OIReports.Troopstrengthdiscount = Convert.ToDecimal(((object[])(item))[20]);
                    OIReports.Othercreditnotes = Convert.ToDecimal(((object[])(item))[21]);
                    OIReports.Weeklyinvoicediscount = Convert.ToDecimal(((object[])(item))[22]);
                    OIReports.Netamountforrations = Convert.ToDecimal(((object[])(item))[23]);
                    OIReports.AplTimelydelivery = Convert.ToDecimal(((object[])(item))[24]);
                    OIReports.AplOrderbylineitems = Convert.ToDecimal(((object[])(item))[25]);
                    OIReports.AplOrdersbyweight = Convert.ToDecimal(((object[])(item))[26]);
                    OIReports.AplNoofauthorizedsubstitutions = Convert.ToDecimal(((object[])(item))[27]);
                    OIReports.Totalinvoiceamount = Convert.ToDecimal(((object[])(item))[28]);
                    OIReports.Modeoftransportation = Convert.ToString(((object[])(item))[29]);
                    OIReports.Distancekm = Convert.ToInt64(((object[])(item))[30]);
                    OIReports.Transportationperkgcost = Convert.ToDecimal(((object[])(item))[31]);
                    OIReports.Totaltransportationcost = Convert.ToDecimal(((object[])(item))[32]);
                    OIReports.Dn = Convert.ToString(((object[])(item))[33]);
                    OIReports.Approveddeliverydate = Convert.ToDateTime(((object[])(item))[34]);
                    OIReports.Actualdateofreceipt = Convert.ToDateTime(((object[])(item))[35]);
                    OIReports.Daysdelay = Convert.ToInt64(((object[])(item))[36]);
                    OIReports.Authorizedcmr = Convert.ToDecimal(((object[])(item))[37]);
                    OIReports.Ordercmr = Convert.ToDecimal(((object[])(item))[38]);
                    OIReports.Acceptedcmr = Convert.ToDecimal(((object[])(item))[39]);
                    OIReports.Cmrutilized = Convert.ToDecimal(((object[])(item))[40]);
                    OIReports.Lineitemdelivered98 = Convert.ToInt64(((object[])(item))[41]);
                    OIReports.Confirmitytolineitemorder98 = Convert.ToDecimal(((object[])(item))[42]);
                    OIReports.Conformitytoorderbyweight = Convert.ToDecimal(((object[])(item))[43]);
                    OIReports.Noofsubtitution = Convert.ToDecimal(((object[])(item))[44]);
                    OIReports.Amountsubstituted = Convert.ToDecimal(((object[])(item))[45]);
                    OIReports.Daysdelayperformance = Convert.ToDecimal(((object[])(item))[46]);
                    OIReports.DeliveryNotes = Convert.ToString(((object[])(item))[47]);
                    OIReports.CreatedDate = Convert.ToDateTime(((object[])(item))[48]);
                    OIReports.CreatedBy = Convert.ToString(((object[])(item))[49]);
                    OIReports.ModifiedBy = Convert.ToString(((object[])(item))[50]);
                    OIReports.ModifiedDate = Convert.ToDateTime(((object[])(item))[51]);
                    OIReports.ReportType = Convert.ToString(((object[])(item))[52]);
                    OIReports.IsActive = Convert.ToBoolean(((object[])(item))[53]);
                    OIReports.InvoiceNumber = Convert.ToString(((object[])(item))[54]);
                    OIReports.ContingentID = Convert.ToInt64(((object[])(item))[55]);
                    OrderItemsList.Add(OIReports);
                }
                return OrderItemsList;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion


        #region DeliveryNoteBasedReport
        public Dictionary<long, IList<DeliveryNoteBasedReport_Vw>> GetDeliveryNoteBasedReportListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<DeliveryNoteBasedReport_Vw>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DeliveryNoteBasedReport_Vw GetDeliveryNoteListById(long Id)
        {
            try
            {
                return PSF.Get<DeliveryNoteBasedReport_Vw>(Id);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion


        #region Added by thamizhmani
        public long SaveOrUpdateGccRevisedFiles(UploadGccRevisedRequest UploadGcc)
        {
            try
            {
                if (UploadGcc != null)
                    PSF.SaveOrUpdate<UploadGccRevisedRequest>(UploadGcc);
                else { throw new Exception("Orders is required and it cannot be null.."); }
                return UploadGcc.Id;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Dictionary<long, IList<GCCRevised>> GetGccRevisedListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<GCCRevised>(page, pageSize, sortType, sortby, criteria);
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
                if (Period != null && PeriodYear != null)
                {
                    string query = "Exec GCCRevisedUpload   '" + ControlId + "','" + DeliveryNoteName + "','" + Period + "','" + PeriodYear + "'";

                    PSF.ExecuteSql(query);

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region Delete GccRevised DN Items
        //Delete deliverynote and imported deliverynote
        public void DeleteDeliveryNoteandPODItems(long DeliveryNoteId, string DeliveryNoteName)
        {
            try
            {

                string deletedeliverynote = "Delete DeliveryNote where  DeliveryNoteId=" + DeliveryNoteId;
                PSF.ExecuteSql(deletedeliverynote);

                string deleteinsightreport = "Delete InsightReport  where  DeliveryNoteName='" + DeliveryNoteName + "'";
                PSF.ExecuteSql(deleteinsightreport);

                string deleteGccRevisedItems = "Delete GCCRevised where DeliveryNoteId=" + DeliveryNoteId;
                PSF.ExecuteSql(deleteGccRevisedItems);

                string delPODItemsByDelnoteId = "Delete  from PODItems where DeliveryNoteId=" + DeliveryNoteId;
                PSF.ExecuteSql(delPODItemsByDelnoteId);


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
                string delUploadRequestByRequestId = "Delete from UploadRequest where RequestId=" + RequestId;
                PSF.ExecuteSql(delUploadRequestByRequestId);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Added by kingston GCCRevised
        public Dictionary<long, IList<GCCRevised_vw>> GetGCCRevisedListbySP(string Period, string PeriodYear)
        {
            try
            {
                return PSF.ExecuteStoredProcedureByDictonary<GCCRevised_vw>("GetGCCRevisedList",
                    new[] { new SqlParameter("Period", Period),
                        new SqlParameter("PeriodYear", PeriodYear)

                    });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteGCCRevisedVersion(string Period, string PeriodYear)
        {
            try
            {
                string query = "Delete from Documents_Excel where Period ='" + Period + "' and PeriodYear='" + PeriodYear + "' and DocumentType='Revised-Book'";
                PSF.ExecuteSql(query);


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region FinalFood Order Report
        public Dictionary<long, IList<FinalFoodOrderDetails_vw>> GetFinalOrderListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<FinalFoodOrderDetails_vw>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Dictionary<long, IList<FinalFoodOrderdetails_SP>> GetFinalFoodOrderListbySP(string Period, string PeriodYear)
        {
            try
            {
                return PSF.ExecuteStoredProcedureByDictonary<FinalFoodOrderdetails_SP>("GetFinalFoodOrderList",
                    new[] { new SqlParameter("Period", Period),
                        new SqlParameter("PeriodYear", PeriodYear)

                    });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion


        #region OrderReport
        public Dictionary<long, IList<ORDRPT_FinalFoodOrderReport_SP>> GetFinalFoodOrderRequistionListbySP(string Period, string PeriodYear, long Week)
        {
            try
            {
                return PSF.ExecuteStoredProcedureByDictonary<ORDRPT_FinalFoodOrderReport_SP>("GetFinalFoodOrderRequistionList",
                    new[] { new SqlParameter("Period", Period),
                        new SqlParameter("PeriodYear", PeriodYear),
                        new SqlParameter("Week",Week)

                    });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Dictionary<long, IList<ORDRPT_ConsolidatedWeekReport>> GetORDRPT_WeekConsolidateList(string Period, string PeriodYear, long Week1, long Week2, long Week3, long Week4)
        {
            try
            {
                return PSF.ExecuteStoredProcedureByDictonary<ORDRPT_ConsolidatedWeekReport>("OrdRptconsolidate",
                    new[] { new SqlParameter("Period", Period),
                        new SqlParameter("PeriodYear", PeriodYear),
                        new SqlParameter("Week1", Week1),
                        new SqlParameter("Week2", Week2),
                        new SqlParameter("Week3", Week3),
                        new SqlParameter("Week4", Week4)


                    });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public Dictionary<long, IList<ORDRPT_ConsolidateWeekReport_vw>> GetORDRPT_ConsolidateWeekReport_vwListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<ORDRPT_ConsolidateWeekReport_vw>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public Dictionary<long, IList<ORDRPT_BulkFoodOrder_SP>> GetORDRPT_BulkFoodOrderList(string Period, string PeriodYear)
        {
            try
            {
                return PSF.ExecuteStoredProcedureByDictonary<ORDRPT_BulkFoodOrder_SP>("BulkFoodOrderList",
                    new[] { new SqlParameter("Period", Period),
                        new SqlParameter("PeriodYear", PeriodYear)
                        

                    });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public Dictionary<long, IList<ORDRPT_AnalysisSP>> GetORDRPT_AnalysisSPList(string Period, string PeriodYear)
        {
            try
            {
                return PSF.ExecuteStoredProcedureByDictonary<ORDRPT_AnalysisSP>("AnalysisList",
                    new[] { new SqlParameter("Period", Period),
                        new SqlParameter("PeriodYear", PeriodYear)
                        

                    });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public Dictionary<long, IList<ORDRPT_FinalReport_SP>> GetORDRPT_FinalReportList(string Period, string PeriodYear, long Week1, long Week2, long Week3, long Week4)
        {
            try
            {
                return PSF.ExecuteStoredProcedureByDictonary<ORDRPT_FinalReport_SP>("GetFinalFoodList",
                    new[] { new SqlParameter("Period", Period),
                        new SqlParameter("PeriodYear", PeriodYear),
                         new SqlParameter("Week1", Week1),
                        new SqlParameter("Week2", Week2),
                         new SqlParameter("Week3", Week3),
                        new SqlParameter("Week4", Week4)
                    });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public Dictionary<long, IList<ORDRPT_Summary>> GetSummaryList(string Period, string PeriodYear)
        {
            try
            {
                return PSF.ExecuteStoredProcedureByDictonary<ORDRPT_Summary>("GetSummaryList",
                    new[] { new SqlParameter("Period", Period),
                        new SqlParameter("PeriodYear", PeriodYear)
                         
                    });
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
                return PSF.GetListWithEQSearchCriteriaCount<ORDRPT_SummaryTroops>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteConsolidateFoodReport(string Period, string PeriodYear)
        {
            try
            {
                string query = "Delete from Documents_Excel where Period ='" + Period + "' and PeriodYear='" + PeriodYear + "' and DocumentType='Consolidate Food Order Report'";
                PSF.ExecuteSql(query);


            }
            catch (Exception ex)
            {
                throw ex;
            }



        }
        #endregion

        #region Initial Orders upload

        //save or update orders using session
        public void SaveOrUpdateInitialOrdersUsingSession(InitialOrders ord, IList<InitialOrderItems> Orderitemslist)
        {
            try
            {
                if (ord != null && Orderitemslist != null)
                {
                    SPSF.SaveOrUpdateInitialOrdersUsingSession(ord, Orderitemslist);
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        public long SaveOrUpdateInitialOrder(InitialOrders Orders)
        {
            try
            {
                if (Orders != null)
                    PSF.SaveOrUpdate<InitialOrders>(Orders);
                else { throw new Exception("Initial Orders is required and it cannot be null.."); }
                return Orders.OrderId;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Dictionary<long, IList<InitialOrders>> GetInitialOrdersListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<InitialOrders>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion



        //LocalPurchaseFiles
        #region
        public Dictionary<long, IList<LocalPurchase>> GetLocalPurchaseFilesListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithExactSearchCriteriaCount<LocalPurchase>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception)
            {
                throw;
            }
        }



        public void InsertLocalPurchaseFFObySP(long RequestId)
        {

            string query = "Exec LocalPurchaseFFOUpload_SP   " + RequestId;

            PSF.ExecuteSql(query);
        }


        public bool SaveorUpdateOrders_LocalPurchaseByList(IList<Orders_LocalPurchase> orditems)
        {
            bool retValue = false;
            try
            {
                if (orditems != null)
                {
                    PSF.SaveOrUpdate<Orders_LocalPurchase>(orditems);
                    retValue = true;
                }
                else { throw new Exception("Orders is required and it cannot be null.."); }
                return retValue;
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
                if (errlog != null)
                    PSF.SaveOrUpdate<ErrorLog>(errlog);
                else { throw new Exception("UploadRequestDetailsLog is required and it cannot be null.."); }
                return errlog.Err_Id;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Invoice number Master
        //save or update the InvoiceNumber master by sending the list

        public bool SaveOrUpdateInvoiceNumberMaster(IList<InvoiceNumberMaster> invMstlist)
        {
            try
            {
                bool retValue = false;
                if (invMstlist != null && invMstlist.Count > 0)
                {
                    PSF.SaveOrUpdate<InvoiceNumberMaster>(invMstlist);
                    retValue = true;
                }
                else { throw new Exception("List of InvoieNumber master is required and it cannot be null.."); }
                return retValue;
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
                return PSF.GetListWithLikeSearchCriteriaCount<InvoiceNumberMaster>(page, pageSize, sortby, sortType, criteria);
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
                return PSF.GetListWithExactAndLikeSearchCriteriaWithCount<InvoiceNumberMaster>(page, pageSize, sortType, sortby, criteria, likeSearchCriteria);
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
                return PSF.Get<InvoiceNumberMaster>("OrderId", OrderId);
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
        //        if (RequestIds != null && RequestIds > 0)
        //            return PSF.GetListById<UploadRequest>("RequestId", RequestIds);
        //        else { throw new Exception("Id is required and it cannot be 0"); }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        //Delete deliverynote and imported deliverynote
        public void DeleteInvoiceSequenceandUploadRequestbyRequestId(long RequestIds)
        {
            try
            {
                string deleteInvoiceSequence = "DELETE FROM dbo.InvoiceNumberMaster WHERE RequestId IN(" + RequestIds + ")";
                PSF.ExecuteSql(deleteInvoiceSequence);

                string delUploadrequestbyId = "DELETE FROM dbo.UploadRequest WHERE Category='INVOICENUMBERMASTERUPLOAD' AND RequestId IN(" + RequestIds + ")";
                PSF.ExecuteSql(delUploadrequestbyId);
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        #region NPA Master Upload
        /// <summary>
        /// Save or Update NPA Master list
        /// </summary>
        /// <param name="NPAlist"></param>
        /// <returns></returns>
        public bool SaveOrUpdateNPAMasterList(IList<NPAMaster> NPAlist)
        {
            try
            {
                bool retValue = false;
                if (NPAlist != null && NPAlist.Count > 0)
                {
                    PSF.SaveOrUpdate<NPAMaster>(NPAlist);
                    retValue = true;
                }
                else { throw new Exception("List of NPA master is required and it cannot be null.."); }
                return retValue;
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
                return PSF.GetListWithLikeSearchCriteriaCount<NPAMaster>(page, pageSize, sortby, sortType, criteria);
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
                return PSF.GetListWithExactAndLikeSearchCriteriaWithCount<NPAMaster>(page, pageSize, sortType, sortby, criteria, likeSearchCriteria);
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
                string updateNPAQuery = "EXEC dbo.NPAMasterUpload_SP " + RequestId;
                PSF.ExecuteSql(updateNPAQuery);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public void DeleteNPAMasterandUploadRequestbyRequestId(long RequestIds)
        {
            try
            {
                string deleteNPAMasterQuery = "EXEC dbo.DeleteNPAMaster_Sp " + RequestIds ;
                PSF.ExecuteSql(deleteNPAMasterQuery);
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public IList<NPAMaster> GetNPAMasterListbyRequestId(long RequestIds)
        {
            try
            {
                if (RequestIds != null && RequestIds > 0)
                    return PSF.GetListById<NPAMaster>("RequestId", RequestIds);
                else { throw new Exception("Id is required and it cannot be 0"); }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        #region FIV
        public FIVItems GetFIVItemsByOrderId(long OrderId)
        {
            try
            {
                return PSF.Get<FIVItems>("OrderId", OrderId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public Dictionary<long, IList<FIVItemsReport>> GetFIVItemsReportListbySP(string OrderId)
        {
            try
            {
                return PSF.ExecuteStoredProcedureByDictonary<FIVItemsReport>("GenerateFIVReportList",
                    new[] { new SqlParameter("OrderId", OrderId) });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void updateFIVStatusByOrderId(string OrderId)
        {
            try
            {

                string query = "update FIVItems set FIVStatus='FAILLED' where OrderId='" + OrderId + "'";
                PSF.ExecuteSql(query);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void UpdateLineIdinFIVItems_SPByOrderId(long OrderId)
        {

            try
            {
                if (OrderId > 0)
                {
                    string query = "Exec dbo.UpdateLineIdinFIVItems_SP   " + OrderId;

                    PSF.ExecuteSql(query);

                }

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
                return PSF.GetListWithEQSearchCriteriaCount<FIVItems_vw>(page, pageSize, sortType, sortby, criteria);
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
                return PSF.GetListWithEQSearchCriteriaCount<FIVItemsReport>(page, pageSize, sortType, sortby, criteria);
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
                string deleteInvoiceSequence = "EXEC dbo.DeleteFIVDetails_sp " + RequestId + "";
                PSF.ExecuteSql(deleteInvoiceSequence);
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        #endregion
    }
}





