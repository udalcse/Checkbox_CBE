using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Web;
using Checkbox.Web.Page;

namespace CheckboxWeb.Forms.Surveys.Invitations
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Responses : InvitationPropertiesPage
    {
        protected override void OnPageInit()
        {
            base.OnPageInit();

            Master.HideDialogButtons();

            Master.SetTitle(WebTextManager.GetText("/pageText/forms/surveys/invitations/responses.aspx/title") + " - " + Utilities.StripHtml(Invitation.Name, 64));

            _responsesGrid.InitialSortField = "UniqueIdentifier";
            _responsesGrid.ListTemplatePath = ResolveUrl("~/Forms/Surveys/Invitations/jqtmpl/responsesGridListTemplate.html");
            _responsesGrid.ListItemTemplatePath = ResolveUrl("~/Forms/Surveys/Invitations/jqtmpl/responsesGridListItemTemplate.html");
            _responsesGrid.LoadDataCallback = "loadResponseList";
            _responsesGrid.EmptyGridText = WebTextManager.GetText("/pageText/forms/surveys/invitations/responses.aspx/noResponses");

            RegisterClientScriptInclude(
                "serviceHelper.js",
                ResolveUrl("~/Services/js/serviceHelper.js"));

            RegisterClientScriptInclude(
               "svcInvitationManagement.js",
               ResolveUrl("~/Services/js/svcInvitationManagement.js"));

            RegisterClientScriptInclude(
                "dateUtils.js",
                ResolveUrl("~/Resources/dateUtils.js"));

            //Moment.js: datetime utilities
            RegisterClientScriptInclude(
                "moment.js",
                ResolveUrl("~/Resources/moment.js"));

            LoadDatePickerLocalized();
        }

    }
}