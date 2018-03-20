using System.Web;
using System.Web.UI.HtmlControls;
using Checkbox.Common;
using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.Styles;
using Checkbox.Security.Principal;
using Checkbox.Globalization.Text;

namespace CheckboxWeb.Styles
{
    public partial class Manage : SecuredPage
    {
        /// <summary>
        /// Require form edit permission to view this page
        /// </summary>
        protected override string PageRequiredRolePermission { get { return "Form.Edit"; } }

        /// <summary>
        /// 
        /// </summary>
        [QueryParameter(ParameterName = "t")]
        public string StyleType { get; set; }

        public string StyleTypeEncoded 
        {
            get { return Utilities.AdvancedHtmlEncode(StyleType); }
        }

        private void CreateTopMenuItem(string textId, string parameter)
        {
            HtmlGenericControl formListItem;//, chartListItem;
            HtmlAnchor formAnchorItem;//, chartAnchorItem;

            formListItem = new HtmlGenericControl("li");
            formAnchorItem = new HtmlAnchor();
            formAnchorItem.InnerText = WebTextManager.GetText(textId);
            formAnchorItem.Attributes["href"] = "javascript:loadGrid('" + parameter + "');";
            formListItem.Controls.Add(formAnchorItem);
            if (StyleType == parameter)
            {
                formListItem.Attributes["class"] = "active";
            }
            _titleActions.Controls.Add(formListItem);
        }

        protected override void OnPageInit()
        {
            base.OnPageInit();
            CreateTopMenuItem("/pageText/styles/manage.aspx/surveyStyle", "");
        }

		protected override void OnLoad(System.EventArgs e)
		{
			base.OnLoad(e);

			if (IsPostBack)
			{
				string target = Request.Params.Get("__EVENTTARGET");
				string idTxt = Request.Params.Get("__EVENTARGUMENT");

				if (target == "_copyLink")
				{
					int id = 0;

					if (int.TryParse(idTxt, out id))
					{
						StyleTemplate style = StyleTemplateManager.GetStyleTemplate(id);

						if (style != null)
						{
							StyleTemplate newStyle = StyleTemplateManager.CopyTemplate(style, TextManager.DefaultLanguage, HttpContext.Current.User as CheckboxPrincipal);
						}
					}
				}
			}
		}
    }
}
