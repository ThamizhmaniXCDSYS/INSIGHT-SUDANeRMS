using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using INSIGHT.Entities.InvoiceEntities;
using INSIGHT.Component;
using INSIGHT.Entities;

namespace INSIGHT.WCFServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "InvoiceService" in code, svc and config file together.
    public class InvoiceService : IInvoiceSC
    {
        InvoicBC invoiceBc = new InvoicBC();
        public Dictionary<long, IList<InvoiceManagementView>> GetInvoiceListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {

                return invoiceBc.GetInvoiceListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                return invoiceBc.GetInvoiceDetailsById(Id);
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
                return invoiceBc.GetInvoiceDetailsByOrderId(OrderId);
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
                return invoiceBc.SaveOrUpdateInvoice(Iv,userId);
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
                return invoiceBc.SaveOrUpdateInvoiceOrders(invoiceId, OrderId);
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
                return invoiceBc.SaveOrUpdateInvoiceItems(invoiceId, OrderItemIds);
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

                return invoiceBc.GetInvoiceItemsListWithPagingAndCriteria(page, pageSize, sortType, sortby, criteria);
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

                return invoiceBc.GetInvoiceOrdersListWithPagingAndCriteria(page, pageSize, sortType, sortby, criteria);
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

                return invoiceBc.GetInvoiceOrdersViewListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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

                return invoiceBc.GetDeliveriesPerOrdQtyListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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

                return invoiceBc.GetDeliveryExceedListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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

                return invoiceBc.GetInvoiceDeliveryExceedQtyListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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

                return invoiceBc.GetInvoiceDeliveryPerQtyListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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

                return invoiceBc.GetInvoiceDeliveryWithoutOrderListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                return invoiceBc.GetDeliveriesPerOrdQtyListWithOutEggs(page, pageSize, sortby, sortType, ColumnName, ExpId, criteria);
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
                return invoiceBc.GetDeliveryExceedListWithOutEggs(page, pageSize, sortby, sortType, ColumnName, ExpId, criteria);
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
                return invoiceBc.GetDeliveryWithoutOrdersList(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DateTime GetApprovedDeliveryDateById(long OrderId)
        {
            return invoiceBc.GetApprovedDeliveryDateById(OrderId);

        }
        public Dictionary<long, IList<OrderListWithoutDelivery>> GetOrdersListNotDelivered(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return invoiceBc.GetOrdersListNotDelivered(page, pageSize, sortby, sortType, criteria);
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

                return invoiceBc.GetSingleInvoiceListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                return invoiceBc.GetPerformanceMatrixListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                return invoiceBc.GetPerformanceMatrixById(OrderId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Dictionary<long, IList<PerformanceCalculateView>> GetPerformanceCalculateListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return invoiceBc.GetPerformanceCalculateListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                return invoiceBc.GetPerformanceCalculateById(OrderId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Dictionary<long, IList<Invoice>> GetInvoicetableListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return invoiceBc.GetInvoicetableListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                return invoiceBc.GetPerformanceDetailsListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                return invoiceBc.GetPerformanceDetailsById(OrderId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public bool SaveWithNewSessionMethods(long OrderId)
        {
            try
            {
                return invoiceBc.SaveWithNewSessionMethods(OrderId);
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
                return invoiceBc.SaveOrUpdateExcelDocuments(ed, userId);
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

                return invoiceBc.GetExcelDocumentListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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

                return invoiceBc.GetExcelDocumentViewListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                return invoiceBc.GetExcelDocumentsDetailsById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public InvoiceManagementView GetInvoiceManagementViewDetailsByControlId(string ControlId)
        {
            try
            {
                return invoiceBc.GetInvoiceManagementViewDetailsByControlId(ControlId);
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

                return invoiceBc.GetPDFDocumentListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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

                return invoiceBc.GetPDFDocumentViewListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                return invoiceBc.GetPDFDocumentsDetailsById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public long SaveOrUpdatePDFDocuments(PDFDocuments pd, string userId)
        {
            try
            {
                return invoiceBc.SaveOrUpdatePDFDocuments(pd, userId);
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
                return invoiceBc.GetPDFDocumentsDetailsByControlId(ControlId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ExcelDocuments GetExcelDocumentsDetailsByControlId(string ControlId)
        {
            try
            {
                return invoiceBc.GetExcelDocumentsDetailsByControlId(ControlId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Dictionary<long, IList<RecentDownloads>> GetRecentDowloadsListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {

                return invoiceBc.GetRecentDowloadsListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                return invoiceBc.GetRecentDocumentsDetailsById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void SaveRecentDownloads(RecentDownloads rd, string userId)
        {
            try
            {
                invoiceBc.SaveRecentDownloads(rd, userId);
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
                invoiceBc.ClearRecordsIntable(table);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        #endregion end

        public Dictionary<long, IList<TransportInvoice>> GetTransportInvoiceListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {

                return invoiceBc.GetTransportInvoiceListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region  Added by kingston (DeliverySequence related code)

        public Dictionary<long, IList<DeliverySequence_PODItems>> GetDeliverySequenceListWithPagingAndCriteria(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {

                return invoiceBc.GetDeliverySequenceListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void UpdateRevisedOrdQtyInvQtyAsZeroForDelSequence(string Period, String PeriodYear, long Week)
        {
            try
            {

                invoiceBc.UpdateRevisedOrdQtyInvQtyAsZeroForDelSequence(Period, PeriodYear, Week);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        public Dictionary<long, IList<SubReplacementView>> GetSubstitudeReplacementList(int? page, int? pageSize, string sortby, string sortType, Dictionary<string, object> criteria)
        {
            try
            {
                return invoiceBc.GetSubstitudeReplacementList(page, pageSize, sortby, sortType, criteria);
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

                return invoiceBc.GetInvoiceReportsListWithPagingAndCriteria(page, pageSize, sortby, sortType, criteria);
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
                return invoiceBc.GetInvoiceReportsDetailsByOrderId(OrderId, ReportType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void SaveOrUpdateInvoiceReports(InvoiceReports rd, string userId)
        {
            try
            {
                invoiceBc.SaveOrUpdateInvoiceReports(rd, userId);
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
                return invoiceBc.GetInvoiceReportListtWithPagingAndCriteria(page, pageSize, sortType, sortby, criteria);
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        #endregion

        public void UpdateActiveStatusInTable(string Period, string PeriodYear, string IsActive, string Tablename)
        {
            try
            {
                invoiceBc.UpdateActiveStatusInTable(Period, PeriodYear, IsActive, Tablename);
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
                return invoiceBc.GetOrderListUsingSP(Period, PeriodYear);
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
                return invoiceBc.GetSingleInvoiceListUsingSP(OrderId);
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
                return invoiceBc.GetSubstitudeReplacementListUsingSP(OrderId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Added by kingston to delete the weekwiseinvoicereports from the invoicereports table

        public void DeleteWeekInvoiceInInvoiceReportsTbl(long Week,string Period,string PeriodYear)
        {
            try
            {

                 invoiceBc.DeleteWeekInvoiceInInvoiceReportsTbl(Week, Period, PeriodYear);
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
                return invoiceBc.GetTransportInvoiceDetailsByOrderId(OrderId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
