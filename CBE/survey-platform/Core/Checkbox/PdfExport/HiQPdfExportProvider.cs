using System;
using System.Web;
using System.Web.Security;
using Affirma.ThreeSharp.Model;
using Checkbox.Management;
using Checkbox.Users;
using HiQPdf;

namespace Checkbox.PdfExport
{
    internal class HiQPdfExportProvider : IPdfExportProvider
    {
        public PdfExportResult ExportReport(int reportId, PdfExportSettings settings)
        {
            AppSettings appSettings = new AppSettings();
            PdfExportResult result = new PdfExportResult();

            HtmlToPdf htmlToPdfConverter = CreateConverter(settings);

            //settings margin top for pdf export from database settings table
            htmlToPdfConverter.Document.Margins.Top = appSettings.GetPdfExportMarginTop;
            htmlToPdfConverter.Document.Margins.Bottom = appSettings.GetPdfExportMarginBottom;

            // get report url
            string url = settings.ApplicationPath + "/RunAnalysis.aspx?p=ALL_PAGES&print=pdf&aid=" 
                 + reportId + "&progresskey=" + settings.ProgressKey
                 + "&ticket=" + settings.AuthTicket + "&orientation=" + settings.Orientation;

            try
            {
                byte[] pdfBuffer = htmlToPdfConverter.ConvertUrlToMemory(url);

                result.Data = pdfBuffer;
                result.Name = settings.Name;
            }
            catch (Exception)
            {
                throw new ApplicationException("The report with ID=" + reportId + " cannot be printed. Please try to reduce its size.");
            }

            return result;
        }

        public PdfExportResult ExportReportHtml(string text, Guid item, PdfExportSettings settings)
        {
            AppSettings appSettings = new AppSettings();
            PdfExportResult result = new PdfExportResult();

            HtmlToPdf htmlToPdfConverter = CreateConverter(settings);

            string url = settings.ApplicationPath + "/Forms/Surveys/Responses/View.aspx?responseGuid="+ item.ToString() + "&print=default";

            //settings margin top for pdf export from database settings table
            htmlToPdfConverter.Document.Margins.Top = appSettings.GetPdfExportMarginTop;
            htmlToPdfConverter.Document.Margins.Bottom = appSettings.GetPdfExportMarginBottom;

            try
            {
                byte[] pdfBuffer = htmlToPdfConverter.ConvertHtmlToMemory(text, url);

                result.Data = pdfBuffer;
                result.Name = settings.Name;
            }
            catch (Exception)
            {
                throw new ApplicationException("The report with ID=" + item + " cannot be printed. Please try to reduce its size.");
            }

            return result;
        }

        public PdfExportResult ExportSurvey(int surveyId, PdfExportSettings settings)
        {
            PdfExportResult result = new PdfExportResult();
            AppSettings appSettings = new AppSettings();

            HtmlToPdf htmlToPdfConverter = CreateConverter(settings);

            //settings margin top for pdf export from database settings table
            htmlToPdfConverter.Document.Margins.Top = appSettings.GetPdfExportMarginTop;
            htmlToPdfConverter.Document.Margins.Bottom = appSettings.GetPdfExportMarginBottom;

            string url;

            //client pdf mode takes care about client pdf export with field state 
            if (!settings.PrintClientPdf)
            {
                // get report url
                string encryptedAuthTicket;
                UserManager.GenerateAuthenticationTicket(UserManager.GetCurrentPrincipal(), out encryptedAuthTicket);

                url = settings.ApplicationPath + "/Forms/Surveys/Preview.aspx?&s=" + surveyId + "&loc=" +
                      settings.Language + "&ticket=" + encryptedAuthTicket + "&print=pdf";
            }
            
            else
            {
                url = settings.ApplicationPath + "/Survey.aspx?s=" + settings.SurveyTemplateGUID + "&print=clientpdf";

                if (!string.IsNullOrEmpty(settings.ResumeId))
                {
                    url += $"&iid={settings.ResumeId}";
                }

                if (settings.CustomResponseGUID != null)
                {
                    url += $"&rGuid={settings.CustomResponseGUID}";
                }

                if (!string.IsNullOrWhiteSpace(settings.InvitationId))
                    url += $"&i={settings.InvitationId}";

                //set authentification cookie to restore client state on survey 
                htmlToPdfConverter.HttpCookies.AddCookie("ASP.NET_SessionId", settings.Session);
                htmlToPdfConverter.HttpCookies.AddCookie(".ASPXAUTH", settings.AuthenticationCookie);
            }
            

            byte[] pdfBuffer = htmlToPdfConverter.ConvertUrlToMemory(url);

            result.Data = pdfBuffer;
            result.Name = settings.Name;

            return result;
        }

        private HtmlToPdf CreateConverter(PdfExportSettings settings)
        {
            // create the HTML to PDF converter
            HtmlToPdf htmlToPdfConverter = new HtmlToPdf();

            // set a demo serial number
            htmlToPdfConverter.SerialNumber = "C0NiWltv-bUdiaXlq-eXIyJTsr-Ois/Kz8y-MzorODol-OjklMjIy-Mg==";

            //wait for N seconds before highcharts will be drawn
            htmlToPdfConverter.WaitBeforeConvert = 3;

            //wait for unlimited time to avoid timeout errors
            htmlToPdfConverter.HtmlLoadedTimeout = int.MaxValue;

            htmlToPdfConverter.Document.FitPageWidth = true;
            // htmlToPdfConverter.Document.FitPageHeight = true;

            // set document orientation
            htmlToPdfConverter.Document.PageOrientation = ConvertFromPdfExportOrientation(settings.Orientation);

            return htmlToPdfConverter;
        }

        private static PdfPageOrientation ConvertFromPdfExportOrientation(PdfExportOrientation orientation)
        {
            return orientation == PdfExportOrientation.Landscape ? PdfPageOrientation.Landscape :
                PdfPageOrientation.Portrait;
        }
    }
}
