using System;
using Checkbox.Styles;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Charts;
using Checkbox.Web.Page;

namespace CheckboxWeb.Styles.Charts
{
    public partial class Create : ApplicationPage
    {
        /// <summary>
        /// Bind events
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            Master.OkClick += OkBtn_Click;

            //Set page title
            Master.Title = WebTextManager.GetText("/pageText/styles/charts/create.aspx/createStyle");
        }

        private void OkBtn_Click(object sender, EventArgs e)
        {
            //if there are validation errors we do nothing and the validation errors will be displayed
            if (!Page.IsValid) return;

            var id = _styleProperties.ChartStyle != null ? _styleProperties.ChartStyle.TemplateId : - 1;

            //if the _styleProperties.StyleTemplate property is null then we must create a new style
            if (_styleProperties.ChartStyle == null)
            {
                var appearance = new SummaryChartItemAppearanceData();
                appearance.Save(null);
                if (appearance.ID != null)
                    id = ChartStyleManager.CreateStyle(
                        appearance.ID.Value,
                        _styleProperties.StyleName,
                        UserManager.GetCurrentPrincipal().Identity.Name,
                        _styleProperties.IsPublic,
                        _styleProperties.IsEditable);
            }
            else
            {
                ChartStyleManager.UpdateStyle(_styleProperties.ChartStyle.TemplateId, _styleProperties.StyleName, _styleProperties.IsPublic, _styleProperties.IsEditable);
            }

            CloseAndRedirect(id);
        }

        private void CloseAndRedirect(int id)
        {
            Page.ClientScript.RegisterClientScriptBlock(
                GetType(),
                "Redirect",
                string.Format("closeWindowAndRedirectParentPage('', null, 'Edit.aspx?s={0}');", id),
                true);
        }
    }
}