using System;

namespace Checkbox.Analytics.Items.Configuration
{
    /// <summary>
    /// Base class for export items
    /// </summary>
    [Serializable]
    public abstract class ExportItemData : AnalysisItemData
    {
        /// <summary>
        /// Get/set whether to include open ended
        /// </summary>
        public virtual bool IncludeOpenEnded { get; set; }

        /// <summary>
        /// Get/set whether to include open ended
        /// </summary>
        public virtual bool ExportRankOrderPoints { get; set; }

        /// <summary>
        /// Get/set whether to merge select many
        /// </summary>
        public virtual bool MergeSelectMany { get; set; }

        /// <summary>
        /// Get/set whether to include hidden items
        /// </summary>
        public virtual bool IncludeHidden { get; set; }

        /// <summary>
        /// Get name of export item config table, though this value is never used
        /// at run time since export items are usually only created/destroyed at run time
        /// and not persisted to the database.
        /// </summary>
        public override string ItemDataTableName { get { return "ExportItemData"; } }

        /// <summary>
        /// Items are not persisted, so return empty string
        /// </summary>
        protected override string LoadSprocName { get { return string.Empty; } }
    }
}