using System;
using Checkbox.Common;
using Checkbox.Web;
using Checkbox.Invitations;
using Checkbox.Web.Page;
using Checkbox.Security.Principal;

namespace CheckboxWeb.Forms.Surveys.Invitations
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Properties : InvitationPropertiesPage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            Master.OkClick += UpdatePropertiesButton_Click;

            Master.SetTitle(string.Format("{0} - {1}", WebTextManager.GetText("/pageText/forms/surveys/invitations/properties.aspx/optionsTitle"), Utilities.StripHtml(Invitation.Name, 64)));

            _properties.Initialize(Invitation);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void UpdatePropertiesButton_Click(object sender, EventArgs e)
        {
            _properties.UpdateInvitation(Invitation);
            
            Invitation.Save(Context.User as CheckboxPrincipal);

            Master.CloseDialog("propertiesUpdated", false);
        }
    }
}
