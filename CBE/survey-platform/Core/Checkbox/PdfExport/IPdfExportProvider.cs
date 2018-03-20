using System;

namespace Checkbox.PdfExport
{
    internal interface IPdfExportProvider
    {
        PdfExportResult ExportReport(int reportId, PdfExportSettings settings);
        PdfExportResult ExportReportHtml(string text, Guid item, PdfExportSettings settings);
        PdfExportResult ExportSurvey(int surveyId, PdfExportSettings settings);
    }
}
