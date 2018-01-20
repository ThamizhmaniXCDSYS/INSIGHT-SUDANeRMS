// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PdfViewController.cs" company="SemanticArchitecture">
//   http://www.SemanticArchitecture.net pkalkie@gmail.com
// </copyright>
// <summary>
//   Extends the controller with functionality for rendering PDF views
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace INSIGHT.Controllers.PDFGeneration
{
    using INSIGHT.Entities.InvoiceEntities;
    using INSIGHT.Entities.PDFEntities;
    using INSIGHT.WCFServices;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Linq;
    using System;

    /// <summary>
    /// Extends the controller with functionality for rendering PDF views
    /// </summary>
    public class PdfViewController : BaseController
    {
        InvoiceService IS = new InvoiceService();
        Dictionary<string, object> criteria = new Dictionary<string, object>();
        public HtmlViewRenderer htmlViewRenderer;
        public HtmlViewRendererForParallel htmlViewRendererforParallel;
        public StandardPdfRenderer standardPdfRenderer;
        public StandardPdfRendererForParallel standardPdfRendererforParallel;

        public PdfViewController()
        {
            this.htmlViewRenderer = new HtmlViewRenderer();
            this.standardPdfRenderer = new StandardPdfRenderer();
            this.htmlViewRendererforParallel = new HtmlViewRendererForParallel();
            this.standardPdfRendererforParallel = new StandardPdfRendererForParallel();
        }

        public ActionResult ViewPdf(string pageTitle, string viewName, object model, string orientation)
        {
            string userId = base.ValidateUser();
            // Render the view html to a string.
            string htmlText = this.htmlViewRenderer.RenderViewToString(this, viewName, model);

            // Let the html be rendered into a PDF document through iTextSharp.
            byte[] buffer = standardPdfRenderer.Render(htmlText, pageTitle, orientation);

            if (viewName == "SingleInvoice")
            {
                SingleInvoice si = (SingleInvoice)model;
                PDFDocuments PD = IS.GetPDFDocumentsDetailsByControlId(si.UNID);
                if (PD != null)
                {
                    PD.DocumentData = buffer;
                    long id = IS.SaveOrUpdatePDFDocuments(PD, userId);
                    SaveDocumentToRecentDownloads(null, PD, string.Empty, null, string.Empty);
                }
                else
                {
                    PDFDocuments pd = new PDFDocuments();
                    InvoiceManagementView Imv = IS.GetInvoiceManagementViewDetailsByControlId(si.UNID);
                    pd.ControlId = Imv.ControlId;
                    pd.OrderId = Imv.OrderId;
                    pd.InvoiceId = Imv.Id;
                    pd.Location = Imv.Location;
                    pd.Name = Imv.Name;
                    pd.ContingentType = Imv.ContingentType;
                    pd.Period = Imv.Period;
                    pd.PeriodYear = Imv.PeriodYear;
                    pd.Sector = Imv.Sector;
                    pd.Week = Imv.Week;
                    pd.DocumentData = buffer;
                    pd.DocumentType = "PDF-Single";
                    pd.DocumentName = si.Reference;
                    long id = IS.SaveOrUpdatePDFDocuments(pd, userId);
                    SaveDocumentToRecentDownloads(null, pd, string.Empty, null, string.Empty);
                }
            }
            else if (viewName == "InvFPUWK3234")
            {
                InvoiceList ci = (InvoiceList)model;
                var InvNoSplit = ci.InvoiceNo.ToString().Split('-');
                var Period = ci.Period.ToString().Split('/');
                //To check weather cosolidate sheet is already exsist or not in PDF Documents table
                PDFDocuments PD = GetPDFDocumentForConsolidate(InvNoSplit[1], InvNoSplit[3], Period[0], Period[1]);
                if (PD != null && PD.Id > 0)
                {
                    PD.DocumentData = buffer;
                    long id = IS.SaveOrUpdatePDFDocuments(PD, userId);
                    SaveDocumentToRecentDownloads(null, PD, string.Empty, null, string.Empty);
                }
                else
                {
                    PDFDocuments pd = new PDFDocuments();
                    pd.ControlId = ci.InvoiceNo;
                    pd.ContingentType = InvNoSplit[3];
                    pd.Period = Period[0];
                    pd.PeriodYear = Period[1];
                    pd.Sector = InvNoSplit[1];
                    pd.DocumentData = buffer;
                    pd.DocumentType = "PDF-Consol";
                    pd.DocumentName = ci.InvoiceNo;
                    long id = IS.SaveOrUpdatePDFDocuments(pd, userId);
                    SaveDocumentToRecentDownloads(null, pd, string.Empty, null, string.Empty);
                }
            }
            // Return the PDF as a binary stream to the client.
            return new BinaryContentResult(buffer, "application/pdf");
        }
        /// <summary>
        /// To check weather cosolidate sheet is already exsist or not in PDF Documents table
        /// </summary>
        /// <param name="Sector"></param>
        /// <param name="ContingentType"></param>
        /// <param name="Period"></param>
        /// <param name="Year"></param>
        /// <returns>PDFDocuments</returns>
        private PDFDocuments GetPDFDocumentForConsolidate(string Sector, string ContingentType, string Period, string Year)
        {
            try
            {
                PDFDocuments pd = new PDFDocuments();
                criteria.Clear();
                if (!string.IsNullOrWhiteSpace(Sector)) { criteria.Add("Sector", Sector); }
                if (!string.IsNullOrWhiteSpace(ContingentType)) { criteria.Add("ContingentType", ContingentType); }
                if (!string.IsNullOrWhiteSpace(Period)) { criteria.Add("Period", Period); }
                if (!string.IsNullOrWhiteSpace(Year)) { criteria.Add("PeriodYear", Year); }
                if (!string.IsNullOrWhiteSpace(Year)) { criteria.Add("DocumentType", "PDF-Consol"); }
                IList<PDFDocuments> DocumentItemsList = null;
                Dictionary<long, IList<PDFDocuments>> DocumentItems = IS.GetPDFDocumentListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                if (DocumentItems != null && DocumentItems.First().Key > 0)
                {
                    DocumentItemsList = DocumentItems.FirstOrDefault().Value;
                    pd.Id = DocumentItemsList[0].Id;
                    pd.Sector = DocumentItemsList[0].Sector;
                    pd.ContingentType = DocumentItemsList[0].ContingentType;
                    pd.Period = DocumentItemsList[0].Period;
                    pd.PeriodYear = DocumentItemsList[0].PeriodYear;
                    pd.DocumentData = DocumentItemsList[0].DocumentData;
                    pd.DocumentType = DocumentItemsList[0].DocumentType;
                    pd.DocumentFor = DocumentItemsList[0].DocumentFor;
                    pd.DocumentName = DocumentItemsList[0].DocumentName;
                    pd.DocumentSize = DocumentItemsList[0].DocumentSize;
                    pd.ControlId = DocumentItemsList[0].ControlId;
                }
                return pd;
            }
            catch (System.Exception ex)
            {

                throw ex;
            }
        }

        #region Main invoice by Contingent by Thamizhmani
        /// <summary>
        /// To check weather consolidate sheet is already exist or not in PDF Documents table for the Order
        /// </summary>
        /// <param name="OrderId"></param>
        /// /// <param name="DocumentType"></param>
        /// <returns></returns>
        private PDFDocuments GetPDFDocumentForConsolidateByOrderId(long OrderId, string DocumentType)
        {
            try
            {
                PDFDocuments pd = new PDFDocuments();
                criteria.Clear();
                criteria.Add("OrderId", OrderId);
                //criteria.Add("DocumentType", "PDF-Consol");
                criteria.Add("DocumentType", DocumentType);
                IList<PDFDocuments> DocumentItemsList = null;
                Dictionary<long, IList<PDFDocuments>> DocumentItems = IS.GetPDFDocumentListWithPagingAndCriteria(0, 9999, string.Empty, string.Empty, criteria);
                if (DocumentItems != null && DocumentItems.First().Key > 0)
                {
                    DocumentItemsList = DocumentItems.FirstOrDefault().Value;
                    pd.Id = DocumentItemsList[0].Id;
                    pd.OrderId = DocumentItemsList[0].OrderId;
                    pd.InvoiceId = DocumentItemsList[0].InvoiceId;
                    pd.Name = DocumentItemsList[0].Name;
                    pd.ContingentType = DocumentItemsList[0].ContingentType;
                    pd.Location = DocumentItemsList[0].Location;
                    pd.ControlId = DocumentItemsList[0].ControlId;
                    pd.Period = DocumentItemsList[0].Period;
                    pd.Sector = DocumentItemsList[0].Sector;
                    pd.Week = DocumentItemsList[0].Week;
                    pd.PeriodYear = DocumentItemsList[0].PeriodYear;
                    pd.DocumentData = DocumentItemsList[0].DocumentData;
                    pd.DocumentType = DocumentItemsList[0].DocumentType;
                    pd.DocumentFor = DocumentItemsList[0].DocumentFor;
                    pd.DocumentName = DocumentItemsList[0].DocumentName;
                    pd.DocumentSize = DocumentItemsList[0].DocumentSize;
                }
                return pd;
            }
            catch (System.Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        public void ViewPdfForParallel(string pageTitle, string viewName, object model, string orientation)
        {
            string userId = "Binoe";
            // Render the view html to a string.
            string htmlText = this.htmlViewRendererforParallel.RenderViewToString(this, viewName, model);

            // Let the html be rendered into a PDF document through iTextSharp.
            byte[] buffer = standardPdfRendererforParallel.Render(htmlText, pageTitle, orientation);

            if (viewName == "SingleInvoice")
            {
                SingleInvoice si = (SingleInvoice)model;
                //PDFDocuments PD = IS.GetPDFDocumentsDetailsByControlId(si.UNID);
                InvoiceManagementView Imv = IS.GetInvoiceManagementViewDetailsByControlId(si.UNID);
                //To check weather cosolidate sheet is already exsist or not in PDF Documents table
                PDFDocuments PD = GetPDFDocumentForConsolidateByOrderId(Imv.OrderId, "PDF-Single");
                if (PD != null && PD.Id > 0)
                {
                    PD.DocumentData = buffer;
                    long id = IS.SaveOrUpdatePDFDocuments(PD, userId);
                }
                else
                {
                    PDFDocuments pd = new PDFDocuments();
                    //InvoiceManagementView Imv = IS.GetInvoiceManagementViewDetailsByControlId(si.UNID);
                    pd.ControlId = Imv.ControlId;
                    pd.OrderId = Imv.OrderId;
                    pd.InvoiceId = Imv.Id;
                    pd.Location = Imv.Location;
                    pd.Name = Imv.Name;
                    pd.ContingentType = Imv.ContingentType;
                    pd.Period = Imv.Period;
                    pd.PeriodYear = Imv.PeriodYear;
                    pd.Sector = Imv.Sector;
                    pd.Week = Imv.Week;
                    pd.DocumentData = buffer;
                    pd.DocumentType = "PDF-Single";
                    pd.DocumentName = si.Reference;
                    long id = IS.SaveOrUpdatePDFDocuments(pd, userId);
                }
            }
            else if (viewName == "InvFPUWK3234")
            {
                InvoiceList ci = (InvoiceList)model;
                var InvNoSplit = ci.InvoiceNo.ToString().Split('-');
                var Period = ci.Period.ToString().Split('/');
                var ControlIdArray = ci.ControlId.Split('-');
                //To check weather cosolidate sheet is already exsist or not in PDF Documents table
                PDFDocuments PD = GetPDFDocumentForConsolidateByOrderId(ci.OrderId, "PDF-Consol");
                //PDFDocuments PD = GetPDFDocumentForConsolidate(InvNoSplit[1], InvNoSplit[3], Period[0], Period[1]);
                if (PD != null && PD.Id > 0)
                {
                    PD.DocumentData = buffer;
                    long id = IS.SaveOrUpdatePDFDocuments(PD, userId);
                }
                else
                {
                    InvoiceManagementView Imv = new InvoiceManagementView();
                    if (ci.ControlId != "N/A")
                    {
                        Imv = IS.GetInvoiceManagementViewDetailsByControlId(ci.ControlId);
                        PDFDocuments pd = new PDFDocuments();


                        pd.OrderId = ci.OrderId;
                        pd.InvoiceId = Imv.Id;
                        pd.Name = ControlIdArray[3];
                        pd.ContingentType =InvNoSplit.Length>1?InvNoSplit[3]:"";
                        pd.Location = ControlIdArray[4];
                        pd.ControlId = ci.ControlId;
                        pd.Period = ControlIdArray[5];
                        pd.Sector = ControlIdArray[2];
                        pd.Week = ci.Week;
                        pd.PeriodYear = ControlIdArray[7];

                        pd.DocumentData = buffer;
                        pd.DocumentType = "PDF-Consol";
                        pd.DocumentName = ci.InvoiceNo;
                        long id = IS.SaveOrUpdatePDFDocuments(pd, userId);
                    }
                }
            }
            // Return the PDF as a binary stream to the client.
        }


    }
}