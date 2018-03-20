using System;
using Checkbox.Web.Page;
using Checkbox.Styles;
using Checkbox.Globalization.Text;
using System.Web;
using Checkbox.Security.Principal;

namespace CheckboxWeb.Styles.Forms
{
    public partial class Edit : SecuredPage
    {
        protected override string  PageRequiredRolePermission { get { return "Form.Edit"; } }

        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            RegisterClientScriptInclude(
                "statusControl.js",
                ResolveUrl("~/Resources/StatusControl.js"));
        
            RegisterClientScriptInclude(
                "serviceHelper.js",
                ResolveUrl("~/Services/js/serviceHelper.js"));

            RegisterClientScriptInclude(
                "svcStyleEditor.js",
                ResolveUrl("~/Services/js/svcStyleEditor.js"));

            RegisterClientScriptInclude(
                "templateHelper.js",
                ResolveUrl("~/Resources/templateHelper.js"));

            RegisterClientScriptInclude(
                "styleEditor.js",
                ResolveUrl("~/Resources/styleEditor.js"));

            RegisterClientScriptInclude(
                "dateUtils.js",
                ResolveUrl("~/Resources/dateUtils.js"));

            //Moment.js: datetime utilities
            RegisterClientScriptInclude(
                "moment.js",
                ResolveUrl("~/Resources/moment.js"));

            RegisterClientScriptInclude(
                "DictionaryTools.js",
                ResolveUrl("~/Resources/DictionaryTools.js"));

            //Helper for uframe
            RegisterClientScriptInclude(
                "htmlparser.js",
                ResolveUrl("~/Resources/htmlparser.js"));

            //Helper for loading pages into divs
            RegisterClientScriptInclude(
                "UFrame.js",
                ResolveUrl("~/Resources/UFrame.js"));

            RegisterClientScriptInclude(
                "jquery.ckbxEditable.js",
                ResolveUrl("~/Resources/jquery.ckbxEditable.js"));

            RegisterClientScriptInclude(
                "manage.js",
                ResolveUrl("~/Resources/styles/manage.js"));

            RegisterClientScriptInclude(
                "svcStyleManagement.js",
                ResolveUrl("~/Services/js/svcStyleManagement.js"));

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
                            Response.Redirect("~/Styles/Forms/Edit.aspx?s=" + newStyle.TemplateID);
                        }
                    }
                }
            }
        }

    }
}