using System;
using System.Web;
using System.Web.UI.WebControls;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Page;
using Prezza.Framework.ExceptionHandling;

namespace CheckboxWeb.Users.Groups
{
    public partial class AddGroupDialog : SecuredPage
    {
        protected override void OnPageInit()
        {
            base.OnPageInit();
            Master.OkClick += OkButtonClick;

            //Set window title
            Master.Title = string.Format("{0} - {1}", WebTextManager.GetText("/pageText/users/manage.aspx/title"), WebTextManager.GetText("/pageText/users/groups/add.aspx/title"));
        }

        /// <summary>
        /// Checking for an existing group name
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void _groupExistValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            string cleanName = _groupName.Text.Trim();

            if (!Checkbox.Management.ApplicationManager.AppSettings.AllowHTMLNames)
            {
                cleanName = Server.HtmlEncode(cleanName);
            }

            args.IsValid = !GroupManager.IsDuplicateName(null, cleanName);
        }

        /// <summary>
        /// Create new group
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OkButtonClick(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }
            string cleanName = _groupName.Text.Trim();
            string cleanDescription = _groupDescription.Text.Trim();

            if (!Checkbox.Management.ApplicationManager.AppSettings.AllowHTMLNames)
            {
                cleanName = Server.HtmlEncode(cleanName);
                cleanDescription = Server.HtmlEncode(cleanDescription);
            }

            Group newGroup = null;

            try
            {
                newGroup = GroupManager.CreateGroup(cleanName, cleanDescription);
                newGroup.ModifiedBy = HttpContext.Current.User.Identity.Name;
                newGroup.Save();
                //Redirect to user add wizard
                Response.Redirect(ResolveUrl("~/Users/Add.aspx"));
            }
            catch (Exception err)
            {
                ExceptionPolicy.HandleException(err, "UIProcess");
                return;
            }
        }
    }
}