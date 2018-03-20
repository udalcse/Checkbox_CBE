using System.Web.UI;
using Checkbox.LicenseLibrary;
using Checkbox.Management;
using Checkbox.Management.Licensing.Limits.Static;
using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.Forms;
using Prezza.Framework.Security;
using Checkbox.Users;

namespace CheckboxWeb.Libraries
{
    public partial class Manage : SecuredPage
    {
        /// <summary>
        /// Survey editor access
        /// </summary>
        protected override string PageRequiredRolePermission
        {
            get { return "Form.Edit"; }
        }

        /// <summary>
        /// Id of default selected library
        /// </summary>
        [QueryParameter("l")]
        public int? InitialLibraryId { get; set; }

        /// <summary>
        /// 
        /// </summary>
		protected override void  OnPageInit()
		{
            base.OnPageInit();

            string title = WebTextManager.GetText("/pageMenu/library_manager/_libraries");

			if (string.IsNullOrEmpty(title))
				title = "Library Manager";

			Master.SetTitle(title);
		}

        /// <summary>
        /// 
        /// </summary>
        protected override bool LicenseControlledFeature
        {
            get { return true; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override bool FeatureCriterion
        {
            get
            {
                if (!ApplicationManager.AppSettings.EnableMultiDatabase)
                    return true;

                string message;
                var limit = new LibraryLimit();
                var result = limit.Validate(out message);
                return (result == LimitValidationResult.LimitNotReached);
            }
        }

		protected override void OnLoad(System.EventArgs e)
		{
			base.OnLoad(e);

			if (IsPostBack)
			{
				string target = Request.Params.Get("__EVENTTARGET");
				string idTxt = Request.Params.Get("__EVENTARGUMENT");

				int libId;

				if (target == "DeleteItem")
				{
					string[] args = idTxt.Split(':');

					libId = int.Parse(args[0]);

					InitialLibraryId = libId;

					int itemId = int.Parse(args[1]);

					LibraryTemplate lib = LibraryTemplateManager.GetLibraryTemplate(libId);

					if (lib == null)
						return;

					//Ensure auth
					if (!AuthorizationFactory.GetAuthorizationProvider().Authorize(UserManager.GetCurrentPrincipal(), lib, "Library.Edit"))
					{
						return;
					}

					int? pagePos = lib.GetPagePositionForItem(itemId);

					int pageId = 0;

					if (pagePos.HasValue)
					{
						TemplatePage page = lib.GetPageAtPosition(pagePos.Value);

						pageId = page.ID.Value;
					}

					lib.DeleteItemFromPage(pageId, itemId);

					return;
				}
			}
		}
    }
}
