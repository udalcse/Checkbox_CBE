using System;

using Checkbox.Forms.Items.Configuration;
using Checkbox.Analytics.Items.Configuration;

using Prezza.Framework.Common;

namespace Checkbox.Analytics.Items
{
    /// <summary>
    /// Export to CSV
    /// </summary>
    [Serializable]
    public class CSVExportItem : ExportItem
    {
        private bool _includeOpenEnded;
        private bool _mergeSelectMany;
        private bool _includeHidden;
        private bool _exportRankOrderPoints;

        /// <summary>
        /// Configure the item
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData itemData, string languageCode, int? templateId)
        {
            ArgumentValidation.CheckExpectedType(itemData, typeof(CSVExportItemData));

            _includeOpenEnded = ((CSVExportItemData)itemData).IncludeOpenEnded;
            _mergeSelectMany = ((CSVExportItemData)itemData).MergeSelectMany;
            _includeHidden = ((CSVExportItemData)itemData).IncludeHidden;
            _exportRankOrderPoints = ((CSVExportItemData)itemData).ExportRankOrderPoints;

            base.Configure(itemData, languageCode, templateId);
        }

        /// <summary>
        /// Get whether to include open-ended results for export
        /// </summary>
        protected override bool IncludeOpenEnded
        {
            get { return _includeOpenEnded; }
        }

        /// <summary>
        /// Get whether to merge select many items for export
        /// </summary>
        protected override bool MergeSelectMany
        {
            get { return _mergeSelectMany; }
        }

        /// <summary>
        /// Get whether to export hidden items
        /// </summary>
        public override bool IncludeHidden
        {
            get { return _includeHidden; }
        }

        /// <summary>
        /// Get whether to export Rank Order points
        /// </summary>
        public override bool ExportRankOrderPoints
        {
            get { return _exportRankOrderPoints; }
        }
    }
}
