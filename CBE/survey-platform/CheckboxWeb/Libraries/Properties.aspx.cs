using System;
using Checkbox.Web.Page;
using Checkbox.Forms;
using Checkbox.Management;
using Checkbox.Globalization.Text;
using Checkbox.Web;
using Checkbox.Common;
using Prezza.Framework.Security;

namespace CheckboxWeb.Libraries
{
	public partial class Properties : SecuredPage
    {
		[QueryParameter("lib", IsRequired = true)]
		public int LibraryId { get; set; }

	    private LibraryTemplate _library;

        /// <summary>
        /// 
        /// </summary>
        public LibraryTemplate Library
        {
            get { return _library ?? (_library = LibraryTemplateManager.GetLibraryTemplate(LibraryId)); }
        }

	    /// <summary>
        /// 
        /// </summary>
	    private string ExistingLibraryName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override IAccessControllable GetControllableEntity()
        {
            return Library;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override string ControllableEntityRequiredPermission { get { return "Library.Edit"; } }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            _nameRequired.Text = WebTextManager.GetText("/pageText/libraries/create.aspx/nameRequired");

            ExistingLibraryName = WebTextManager.GetText(Library.NameTextID);

            _libraryName.Text = Server.HtmlDecode(ExistingLibraryName);
            _description.Text = WebTextManager.GetText(Library.DescriptionTextID);

            //Bind click event
            Master.OkClick += SubmitBtn_Click;

            //Set Title
            var title = WebTextManager.GetText("/pageText/libraries/properties.aspx/libraryProperties");
            title = String.Format(title, Utilities.TruncateText(ExistingLibraryName, 25));
            Master.Title = title;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SubmitBtn_Click(object sender, EventArgs e)
        {
            var name = _libraryName.Text.Trim();
            var desc = _description.Text.Trim();

            if (!ApplicationManager.AppSettings.AllowHTMLNames)
            {
                name = Server.HtmlEncode(name);
                desc = Server.HtmlEncode(desc);
            }

			if (!name.Equals(ExistingLibraryName, StringComparison.InvariantCulture))
			{
				if (!LibraryTemplateManager.LibraryTemplateExists(name, LibraryId))
				{
					var lang = WebTextManager.GetUserLanguage();

					TextManager.SetText("/library/" + LibraryId + "/name", lang, name);
					TextManager.SetText("/library/" + LibraryId + "/description", lang, desc);

                    Master.CloseDialog("{op:'properties'}", true);
				}
				else
				{
					_duplicateName.Visible = true;
				}
			}
			else
			{
				TextManager.SetText("/library/" + LibraryId + "/description", WebTextManager.GetUserLanguage(), desc);

			    Master.CloseDialog("{op:'properties'}", true);
			}
		}
    }
}
