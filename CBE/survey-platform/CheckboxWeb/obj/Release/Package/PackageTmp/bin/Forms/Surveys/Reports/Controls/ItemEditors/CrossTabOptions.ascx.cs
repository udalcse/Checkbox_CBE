using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Analytics.Items.Configuration;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors
{
    public partial class CrossTabOptions : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// Initialize options
        /// </summary>
        /// <param name="itemData"></param>
        public void Initialize(CrossTabItemData itemData)
        {
            _aliasesChk.Checked = itemData.UseAliases;
        }

        /// <summary>
        /// Update data
        /// </summary>
        /// <param name="itemData"></param>
        public void UpdateData(CrossTabItemData itemData)
        {
            itemData.UseAliases = _aliasesChk.Checked;
        }
    }
}