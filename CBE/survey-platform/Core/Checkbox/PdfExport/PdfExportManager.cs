using System;
using System.IO;

namespace Checkbox.PdfExport
{
    /// <summary>
    /// 
    /// </summary>
    public static class PdfExportManager
    {
        private static readonly IPdfExportProvider exportProvider = new HiQPdfExportProvider();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportId"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static PdfExportResult ExportReport(int reportId, PdfExportSettings settings)
        {
            return exportProvider.ExportReport(reportId, settings);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportId"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static PdfExportResult ExportReportHtml(string text, Guid item, PdfExportSettings settings)
        {
            return exportProvider.ExportReportHtml(text, item, settings);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="surveyId"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static PdfExportResult ExportSurvey(int surveyId, PdfExportSettings settings)
        {
            return exportProvider.ExportSurvey(surveyId, settings);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tw"> </param>
        /// <param name="reportId"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static void CommonReportExport(Stream tw, int reportId, PdfExportSettings settings)
        {
            PdfExportResult res = exportProvider.ExportReport(reportId, settings);

            tw.Write(res.Data, 0, res.Data.Length);
        }
    }
}
