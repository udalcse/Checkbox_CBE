using Checkbox.Analytics.Items.Configuration;
using Checkbox.Web.Analytics.UI.Editing;
using Checkbox.Web.Forms.UI.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors
{
    public partial class HeatMapBehavior : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// Initialize control.
        /// </summary>
        /// <param name="itemData"></param>
        public void Initialize(AnalysisItemData itemData)
        {
            _aliasesChk.Checked = itemData.UseAliases;
        }

        /// <summary>
        /// Update item data with selected options.
        /// </summary>
        /// <param name="itemData"></param>
        public void UpdateItemData(AnalysisItemData itemData)
        {
            itemData.UseAliases = _aliasesChk.Checked;
        }
    }
}