using Checkbox.Analytics.Items;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items;

namespace Checkbox.Analytics.Export
{
    /// <summary>
    /// CSV exporter that can export a CSV format readable by SPSS.
    /// </summary>
    public class SpssCompatibleCsvDataExporter : CsvDataExporter
    {
        /// <summary>
        /// Create the analysis to use
        /// </summary>
        /// <returns></returns>
        protected override AnalysisTemplate CreateAnalysisTemplate()
        {
            return AnalysisTemplateManager.GenerateSPSSExportTemplate(ResponseTemplate, Options);
        }

        /// <summary>
        /// Get actual text name for field name
        /// </summary>
        /// <param name="fieldName">Name of field to list actual text for. (SPSS fields are named Q1, Q2, etc. and the method returns
        /// actual question text)</param>
        /// <returns>Question text associated with the specific field.</returns>
        public string GetActualTextForField(string fieldName)
        {
            string value = string.Empty;

            foreach(Item item in Analysis.Items)
            {
                if (item is SpssExportItem)
                {
                   value = ((SpssExportItem)item).GetActualText(fieldName);
                   break;
                }
            }

            if (Utilities.IsNullOrEmpty(value))
            {
                value = fieldName;
            }

            return value;
        }

        /// <summary>
        /// Get id of item associated with a field.  Does not include SelectMany items.
        /// </summary>
        /// <param name="fieldName">SPSS named field (Q1, Q2, etc.)</param>
        /// <returns>ID of item associated with field.</returns>
        public int GetItemIdForField(string fieldName)
        {
            foreach (Item item in Analysis.Items)
            {
                if (item is SpssExportItem)
                {
                    return ((SpssExportItem)item).GetItemId(fieldName);
                }
            }

            return -1;
        }
    }
}
