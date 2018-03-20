using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Styles;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Page;

namespace CheckboxWeb.Styles.Charts
{
    public partial class Properties : ApplicationPage
    {
        /// <summary>
        /// Bind events
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            Master.OkClick += OkBtn_Click;

            //Set page title
            Master.Title = WebTextManager.GetText("/pageText/styles/forms/properties.aspx/styleProperties");
        }

        private void OkBtn_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            ChartStyleManager.UpdateStyle(_styleProperties.StyleTemplate.TemplateId, _styleProperties.StyleName,
                                          _styleProperties.IsPublic, _styleProperties.IsEditable);

            //TODO: Redirect to chart list instead of survey style list
            ClientScript.RegisterClientScriptBlock(GetType(), "closeAndReload", "closeWindowAndRefreshParentPage();", true);
        }
    }
}