using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistenceFactory;
using INSIGHT.Entities.InvoiceEntities;
using System.Collections;
using INSIGHT.Entities;
using INSIGHT.Entities.DeletedRecordEntities;
using System.Data.SqlClient;

namespace INSIGHT.Component
{
    public class InvoicBC
    {
        PersistenceServiceFactory PSF = null;
        ProjectSpecificPSF SPSF = null;
        public InvoicBC()
        {
            List<String> Assembly = new List<String>();
            Assembly.Add("INSIGHT.Entities");
            Assembly.Add("INSIGHT.Entities.TicketingSystem");
            PSF = new PersistenceServiceFactory(Assembly);
            SPSF = new ProjectSpecificPSF(Assembly);

        }

        public Dictionary<long, IList<InvoiceManagementView>> GetInvoiceListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<InvoiceManagementView>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Invoice GetInvoiceDetailsById(long Id)
        {
            try
            {
                return PSF.Get<Invoice>("Id", Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Invoice GetInvoiceDetailsByOrderId(long OrderId)
        {
            try
            {
                return PSF.Get<Invoice>("OrderId", OrderId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public long SaveOrUpdateInvoice(Invoice Iv, string userId)
        {
            try
            {
                if (Iv != null && Iv.Id > 0)
                {
                    Iv.ModifiedBy = userId;
                    Iv.ModifiedDate = DateTime.Now;
                    PSF.Update<Invoice>(Iv);
                }
                else
                {

                    Iv.CreatedBy = userId;
                    Iv.CreatedDate = DateTime.Now;
                    PSF.Save<Invoice>(Iv);
                }
                return Iv.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool SaveOrUpdateInvoiceOrders(long invoiceId, long[] OrderId)
        {
            try
            {
                bool retValue = false;
                if (invoiceId > 0 && OrderId.Length > 0)
                {
                    foreach (long id in OrderId)
                    {
                        PSF.SaveOrUpdate<InvoiceOrders>(new InvoiceOrders() { InvoiceId = invoiceId, OrderId = id, Status = "Gen Invoice", CreatedDate = DateTime.Now });
                    }
                    retValue = true;
                }
                return retValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool SaveOrUpdateInvoiceItems(long invoiceId, long[] OrderItemIds)
        {
            try
            {
                bool retValue = false;
                if (invoiceId > 0 && OrderItemIds.Length > 0)
                {
                    foreach (long id in OrderItemIds)
                    {
                        //logic need to be finalysed
                        PSF.SaveOrUpdate<InvoiceItems>(new InvoiceItems() { InvoiceId = invoiceId, ItemId = id });
                    }
                    retValue = true;
                }
                return retValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Dictionary<long, IList<InvoiceItems>> GetInvoiceItemsListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<InvoiceItems>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Dictionary<long, IList<InvoiceOrders>> GetInvoiceOrdersListWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<InvoiceOrders>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Dictionary<long, IList<InvoiceOrdersView>> GetInvoiceOrdersViewListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<InvoiceOrdersView>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Dictionary<long, IList<DeliveriesPerOrdQty>> GetDeliveriesPerOrdQtyListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<DeliveriesPerOrdQty>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Dictionary<long, IList<DeliveryExceed>> GetDeliveryExceedListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<DeliveryExceed>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Dictionary<long, IList<InvoiceDeliveryExceedQty>> GetInvoiceDeliveryExceedQtyListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<InvoiceDeliveryExceedQty>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Dictionary<long, IList<InvoiceDeliveryPerQty>> GetInvoiceDeliveryPerQtyListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<InvoiceDeliveryPerQty>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Dictionary<long, IList<InvoiceDeliveryWithoutOrders>> GetInvoiceDeliveryWithoutOrderListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<InvoiceDeliveryWithoutOrders>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Dictionary<long, IList<DeliveriesPerOrdQty>> GetDeliveriesPerOrdQtyListWithOutEggs(int? page, int? pageSize, string sortby, string sortType, string ColumnName, int[] ExpId, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithNotInWithEQSearchCriteriaCountArray<DeliveriesPerOrdQty>(page, pageSize, sortby, sortType, ColumnName, ExpId, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Dictionary<long, IList<DeliveryExceed>> GetDeliveryExceedListWithOutEggs(int? page, int? pageSize, string sortby, string sortType, string ColumnName, int[] ExpId, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithNotInWithEQSearchCriteriaCountArray<DeliveryExceed>(page, pageSize, sortby, sortType, ColumnName, ExpId, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Dictionary<long, IList<DeliveryWithoutOrders>> GetDeliveryWithoutOrdersList(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<DeliveryWithoutOrders>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public DateTime GetApprovedDeliveryDateById(long OrderId)
        {
            try
            {
                //string query = "SELECT TOP 1 ActualDeliveryDate from Deliverynote WHERE DeliveryNoteType='Main DN' AND OrderId='" + OrderId + "'";
                //string query = "SELECT MAX(DeliveredDate) as Deliverydate FROM PODItems where OrderId='" + OrderId + "'";
                string query = "SELECT ApprovedDeliverydate FROM dbo.GetActualDeliveryDate_Vw where OrderId='" + OrderId + "'";
                IList list = PSF.ExecuteSql(query);
                if (list != null && list.Count > 0 && list[0] != null)
                {
                    return Convert.ToDateTime(list[0]); //list[0] = "0";
                }
                else
                {
                    string GetApprovedDeliveryDateQuery = "SELECT ExpectedDeliveryDate FROM dbo.Orders WHERE OrderId=" + OrderId;
                    IList GetApprovedDeliveryDatelist = PSF.ExecuteSql(GetApprovedDeliveryDateQuery);
                    if (GetApprovedDeliveryDatelist != null && GetApprovedDeliveryDatelist.Count > 0 && GetApprovedDeliveryDatelist[0] != null)
                    {
                        return Convert.ToDateTime(GetApprovedDeliveryDatelist[0]); //list[0] = "0";
                    }
                    else
                        return DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Dictionary<long, IList<OrderListWithoutDelivery>> GetOrdersListNotDelivered(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<OrderListWithoutDelivery>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #region Single Invoice List in Excel Added By Gobi
        public Dictionary<long, IList<SingleInvoiceView>> GetSingleInvoiceListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<SingleInvoiceView>(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion Single Invoice List in Excel

        public Dictionary<long, IList<PerformanceMatrix>> GetPerformanceMatrixListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<PerformanceMatrix>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PerformanceMatrix GetPerformanceMatrixById(long OrderId)
        {
            try
            {
                return PSF.Get<PerformanceMatrix>("OrderId", OrderId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<long, IList<PerformanceCalculateView>> GetPerformanceCalculateListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<PerformanceCalculateView>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public PerformanceCalculateView GetPerformanceCalculateById(long OrderId)
        {
            try
            {
                return PSF.Get<PerformanceCalculateView>("OrderId", OrderId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<long, IList<Invoice>> GetInvoicetableListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<Invoice>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Dictionary<long, IList<PerformanceDetails>> GetPerformanceDetailsListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<PerformanceDetails>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public PerformanceDetails GetPerformanceDetailsById(long OrderId)
        {
            try
            {
                return PSF.Get<PerformanceDetails>("OrderId", OrderId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool SaveWithNewSessionMethods(long OrderId)
        {
            try
            {
                Dictionary<long, IList<Orders>> OrdersList = null;
                Dictionary<long, IList<OrderItems>> OrderItemsList = null;
                Dictionary<long, IList<POD>> PODList = null;

                IList<OrdersDel> ordersDel = new List<OrdersDel>();
                IList<OrderItemsDel> orderItemsDel = new List<OrderItemsDel>();
                IList<PODDel> podDel = new List<PODDel>();

                //getting order value
                OrdersBC obc = new OrdersBC();
                Dictionary<string, object> criteria = new Dictionary<string, object>();
                criteria.Clear();
                criteria.Add("OrderId", OrderId);
                OrdersList = obc.GetOrdersListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                //getting order items value
                OrderItemsList = obc.GetOrderItemsListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                //getting POd value into a list
                PODList = obc.GetPODMasterListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                IList<Orders> order = new List<Orders>();
                order = OrdersList.FirstOrDefault().Value.ToList();
                IList<OrderItems> orderItems = new List<OrderItems>();
                orderItems = OrderItemsList.FirstOrDefault().Value.ToList();
                IList<POD> pod = new List<POD>();
                pod = PODList.FirstOrDefault().Value.ToList();

                if (order.Count() != 0)
                {
                    //using orderlist make delete list
                    long idCount = obc.GetCurrentIntent("Orders_Del");
                    foreach (var item in order)
                    {
                        OrdersDel od = new OrdersDel();
                        od.Id = idCount;
                        od.OrderId = item.OrderId;
                        od.InvoiceId = item.InvoiceId;
                        od.PODId = item.PODId;
                        od.Name = item.Name;
                        od.ContingentType = item.ContingentType;
                        od.Location = item.Location;
                        od.CreatedDate = item.CreatedDate;
                        od.EndDate = item.EndDate;
                        od.StartDate = item.StartDate;
                        od.ExpectedDeliveryDate = item.ExpectedDeliveryDate != null ? item.ExpectedDeliveryDate : null;
                        od.DeletedDate = DateTime.Now;
                        ordersDel.Add(od);
                        idCount = idCount + 1;
                    }
                }

                if (orderItems.Count() != 0)
                {
                    long idCount = obc.GetCurrentIntent("OrderItems_Del");
                    //using orderlist make delete list
                    foreach (var item in orderItems)
                    {
                        OrderItemsDel oid = new OrderItemsDel();
                        oid.Id = idCount;
                        oid.LineId = item.LineId;
                        oid.OrderId = item.OrderId;
                        oid.UNCode = item.UNCode;
                        oid.Commodity = item.Commodity;
                        oid.OrderQty = item.OrderQty;
                        oid.SectorPrice = item.SectorPrice;
                        oid.CreatedBy = item.CreatedBy;
                        oid.CreatedDate = item.CreatedDate != null ? item.CreatedDate : null;
                        oid.ModifiedBy = item.ModifiedBy;
                        oid.ModifiedDate = item.ModifiedDate != null ? item.ModifiedDate : null;
                        oid.DeletedDate = DateTime.Now;
                        orderItemsDel.Add(oid);
                        idCount = idCount + 1;
                    }
                }

                if (pod.Count() != 0)
                {
                    long idCount = obc.GetCurrentIntent("POD_Del");
                    foreach (var item in pod)
                    {
                        PODDel pd = new PODDel();
                        pd.Id = idCount;
                        pd.PODId = item.PODId;
                        pd.PODNo = item.PODNo;
                        pd.CreatedBy = item.CreatedBy;
                        pd.CreatedDate = item.CreatedDate;// != null ? item.CreatedDate : null;
                        pd.DeliveryDate = item.DeliveryDate;// != null ? item.DeliveryDate : null;
                        pd.OrderId = item.OrderId;
                        pd.Status = item.Status;
                        pd.DeletedDate = DateTime.Now;
                        podDel.Add(pd);
                        idCount = idCount + 1;
                    }
                }

                return SPSF.Save(ordersDel, orderItemsDel, podDel); ;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public long SaveOrUpdateExcelDocuments(ExcelDocuments ed, string userId)
        {
            try
            {
                if (ed != null && ed.Id > 0)
                {
                    ed.ModifiedBy = userId;
                    ed.ModifiedDate = DateTime.Now;
                    PSF.Update<ExcelDocuments>(ed);
                }
                else
                {
                    ed.CreatedBy = userId;
                    ed.CreatedDate = DateTime.Now;
                    PSF.Save<ExcelDocuments>(ed);
                }
                return ed.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Dictionary<long, IList<ExcelDocuments>> GetExcelDocumentListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithExactSearchCriteriaCountWithInCondition<ExcelDocuments>(page, pageSize, sortby, sortType, criteria);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Dictionary<long, IList<ExcelDocumentsView>> GetExcelDocumentViewListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithExactSearchCriteriaCountWithInCondition<ExcelDocumentsView>(page, pageSize, sortby, sortType, criteria);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ExcelDocuments GetExcelDocumentsDetailsById(long Id)
        {
            try
            {
                return PSF.Get<ExcelDocuments>("Id", Id);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public InvoiceManagementView GetInvoiceManagementViewDetailsByControlId(string ControlId)
        {
            try
            {
                return PSF.Get<InvoiceManagementView>("ControlId", ControlId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Dictionary<long, IList<PDFDocuments>> GetPDFDocumentListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithExactSearchCriteriaCountWithInCondition<PDFDocuments>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Dictionary<long, IList<PDFDocumentsView>> GetPDFDocumentViewListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithExactSearchCriteriaCountWithInCondition<PDFDocumentsView>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public PDFDocuments GetPDFDocumentsDetailsById(long Id)
        {
            try
            {
                return PSF.Get<PDFDocuments>("Id", Id);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public long SaveOrUpdatePDFDocuments(PDFDocuments pd, string userId)
        {
            try
            {
                if (pd != null && pd.Id > 0)
                {
                    pd.ModifiedBy = userId;
                    pd.ModifiedDate = DateTime.Now;
                    PSF.Update<PDFDocuments>(pd);
                }
                else
                {
                    pd.CreatedBy = userId;
                    pd.CreatedDate = DateTime.Now;
                    PSF.Save<PDFDocuments>(pd);
                }
                return pd.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public PDFDocuments GetPDFDocumentsDetailsByControlId(string ControlId)
        {
            try
            {
                return PSF.Get<PDFDocuments>("ControlId", ControlId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ExcelDocuments GetExcelDocumentsDetailsByControlId(string ControlId)
        {
            try
            {
                return PSF.Get<ExcelDocuments>("ControlId", ControlId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<long, IList<RecentDownloads>> GetRecentDowloadsListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<RecentDownloads>(page, pageSize, sortType, sortby, criteria);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public RecentDownloads GetRecentDocumentsDetailsById(long Id)
        {
            try
            {
                return PSF.Get<RecentDownloads>("Id", Id);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void SaveRecentDownloads(RecentDownloads rd, string userId)
        {
            try
            {
                //if (ed != null && ed.Id > 0)
                //{
                //    ed.ModifiedBy = userId;
                //    ed.ModifiedDate = DateTime.Now;
                //    PSF.Update<RecentDownloads>(ed);
                //}
                //else
                //{
                rd.CreatedBy = userId;
                rd.CreatedDate = DateTime.Now;
                PSF.Save<RecentDownloads>(rd);
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Clear Records
        public void ClearRecordsIntable(string table)
        {
            try
            {
                string query = "DELETE FROM " + table;
                PSF.ExecuteSql(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion end
        public Dictionary<long, IList<TransportInvoice>> GetTransportInvoiceListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithExactSearchCriteriaCountWithInCondition<TransportInvoice>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public Dictionary<long, IList<DeliverySequence_PODItems>> GetDeliverySequenceListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<DeliverySequence_PODItems>(page, pageSize, sortType, sortby, criteria);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Dictionary<long, IList<SubReplacementView>> GetSubstitudeReplacementList(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithExactSearchCriteriaCountWithInCondition<SubReplacementView>(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #region InvoiceReports
        public Dictionary<long, IList<InvoiceReports>> GetInvoiceReportsListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return PSF.GetListWithEQSearchCriteriaCount<InvoiceReports>(page, pageSize, sortType, sortby, criteria);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public InvoiceReports GetInvoiceReportsDetailsByOrderId(long OrderId, string ReportType)
        {
            try
            {
                return PSF.Get<InvoiceReports>("OrderId", OrderId, "ReportType", ReportType);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void SaveOrUpdateInvoiceReports(InvoiceReports rd, string userId)
        {
            try
            {
                if (rd != null && rd.Id > 0)
                {
                    rd.ModifiedBy = userId;
                    rd.ModifiedDate = DateTime.Now;
                    PSF.Update<InvoiceReports>(rd);
                }
                else
                {
                    rd.CreatedBy = userId;
                    rd.CreatedDate = DateTime.Now;
                    PSF.Save<InvoiceReports>(rd);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IList<InvoiceReports> GetInvoiceReportListtWithPagingAndCriteria(int? page, int? pageSize, string sortType, string sortby, Dictionary<string, object> criteria)
        {
            try
            {
                long[] val = { 0 };
                return PSF.GetList<InvoiceReports>(page, pageSize, sortType, sortby, string.Empty, val);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        public void UpdateRevisedOrdQtyInvQtyAsZeroForDelSequence(string Period, string PeriodYear, long Week)
        {
            try
            {
                string query = "update PODItems set RevisedOrderQty=0,InvoiceQty=0 where OrderId  in (select orderid from dbo.orders where Period='" + Period + "' and PeriodYear='" + PeriodYear + "' and Week=" + Week + ")";
                PSF.ExecuteSql(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void UpdateActiveStatusInTable(string Period, string PeriodYear, string IsActive, string Tablename)
        {
            try
            {
                string query = "Update " + Tablename + " Set IsActive='" + IsActive + "' from " + Tablename + " where Period='" + Period + "' and PeriodYear='" + PeriodYear + "'";
                PSF.ExecuteSql(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public IEnumerable<Orders> GetOrderListUsingSP(string Period, string PeriodYear)
        {
            try
            {
                return PSF.ExecuteStoredProcedure<Orders>("GetOrderList",
                    new[] { new SqlParameter("Period", Period), 
                            new SqlParameter("PeriodYear", PeriodYear) 
                    });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Dictionary<long, IList<SingleInvoiceView>> GetSingleInvoiceListUsingSP(long OrderId)
        {
            try
            {
                return PSF.ExecuteStoredProcedureByDictonary<SingleInvoiceView>("GetSingleInvoiceList",
                    new[] { new SqlParameter("OrderId", OrderId)
                    });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Dictionary<long, IList<SubReplacementView>> GetSubstitudeReplacementListUsingSP(long OrderId)
        {
            try
            {
                return PSF.ExecuteStoredProcedureByDictonary<SubReplacementView>("GetSubReplacementList",
                    new[] { new SqlParameter("OrderId", OrderId)
                    });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //deleting the weekwiseinvoicereports from the invoicereports table

        public void DeleteWeekInvoiceInInvoiceReportsTbl(long Week, string Period, string PeriodYear)
        {
            try
            {

                string query = "Delete from InvoiceReports where Week=" + Week + " and Period='" + Period + "' and  PeriodYear='" + PeriodYear + "' and ReportType='WKINV'";
                PSF.ExecuteSql(query);

            }
            catch (Exception)
            {
                throw;
            }
        }
        public TransportInvoice GetTransportInvoiceDetailsByOrderId(long OrderId)
        {
            try
            {
                return PSF.Get<TransportInvoice>("OrderId", OrderId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}



