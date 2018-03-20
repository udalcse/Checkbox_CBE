using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Page;

using Prezza.Framework.Security;

namespace CheckboxWeb.Forms.Surveys
{
	/// <summary>
	/// Move/Copy survey page
	/// </summary>
	public partial class Copy : SecuredPage
	{
		[QueryParameter("s", IsRequired = true)]
		public int? ResponseTemplateId { get; set; }

		/// <summary>
		/// Bind events
		/// </summary>
		protected override void OnPageInit()
		{
			base.OnPageInit();

			Master.OkClick += _okButton_Click;

		    var lightweightRt = ResponseTemplateManager.GetLightweightResponseTemplate(ResponseTemplateId.Value);

            if (lightweightRt == null)
            {
                throw new Exception("Unable to load survey with id: " + ResponseTemplateId);
            }

            string decodedName = Utilities.AdvancedHtmlDecode(ResponseTemplateManager.GetUniqueName(lightweightRt.Name, null));

            _nameText.Text = decodedName;

            Master.SetTitle(string.Format("{0} - {1}", WebTextManager.GetText("/pageText/forms/surveys/copy.aspx/moveCopySurvey"), Utilities.StripHtml(decodedName, 36)));

			PopulateFolderList();
		}

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            _copyNamePanel.Enabled = _radCopy.Checked;
            _nameText.Enabled = _radCopy.Checked;
        }

		/// <summary>
		/// Populate list of folders
		/// </summary>
		private void PopulateFolderList()
		{
			List<LightweightAccessControllable> folderList = FolderManager.ListAccessibleFolders(UserManager.GetCurrentPrincipal(), "FormFolder.Read");

			int selectedFolderIndex = -1;

			for (int f = 0; f < folderList.Count; f++)
			{
				List<int> items = FolderManager.ListFolderItems(folderList[f].ID);

				if (items.Contains(ResponseTemplateId.Value))
					selectedFolderIndex = f;
			}

			_destinationList.DataSource = folderList;

			_destinationList.DataTextField = "Name";
			_destinationList.DataValueField = "ID";

			_destinationList.DataBind();

			//Add root folder
			_destinationList.Items.Insert(0, new ListItem(WebTextManager.GetText("/pageText/moveToFolder.aspx/root"), "1"));

			_destinationList.SelectedIndex = selectedFolderIndex >= 0 ? selectedFolderIndex + 1 : 0;

		}

		/// <summary>
		/// Handle ok button click.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void _okButton_Click(object sender, System.EventArgs e)
		{
			if (!ResponseTemplateId.HasValue)
			{
				CloseAndRedirect(false);

				return;
			}

			ResponseTemplate targetRt = ResponseTemplateManager.GetResponseTemplate(ResponseTemplateId.Value);

			int targetRtId;

			if (targetRt == null)
			{
				throw new Exception("Unable to load template to move/copy [" + ResponseTemplateId + "]");
			}

			//Get id of folder to move survey to
			int? destinationFolderId = Utilities.AsInt(_destinationList.SelectedValue);

			if (_radCopy.Checked)
			{
				ResponseTemplate newTemplate = ResponseTemplateManager.CopyTemplate(
					targetRt.ID.Value, _nameText.Text,
					UserManager.GetCurrentPrincipal(),
					targetRt.LanguageSettings.DefaultLanguage);

				if (newTemplate == null)
				{
					CloseAndRedirect(false);

					return;

					//throw new Exception("Unknown error occurred copying template with id: " + targetRt.ID);
				}

				targetRtId = newTemplate.ID.Value;
			}
			else
			{
				targetRtId = ResponseTemplateId.Value;
			}

			//Get folder
			FormFolder destinationFolder = (destinationFolderId.Value == 1)
				? FolderManager.GetRoot()
				: FolderManager.GetFolder(destinationFolderId.Value);

			if (destinationFolder == null)
			{
				throw new Exception("Unable to load destination folder [" + destinationFolderId + "].");
			}

			//Do the move
			destinationFolder.Add(targetRtId);

			//Close window and report status to status page.
			CloseAndRedirect(true);
		}

		void CloseAndRedirect(bool reload)
		{            
            if (reload)
            {
                var args = new Dictionary<string, string> {{"op", "moveOrCopySurvey"}};
                Master.CloseDialog(args); 
            }
            else
            {
                Master.CloseDialog("cancel", false);
            }

		    
/*            
            if (!reload)
			{
				Page.ClientScript.RegisterClientScriptBlock(
					GetType(),
					"CloseAndRedirect",
					"closeWindow('');",
					true);
			}
			else
			{
				Page.ClientScript.RegisterClientScriptBlock(
					GetType(),
					"CloseAndRedirect",
                    "closeWindow('refresh');reloadSurveyAndFolderList();",
					true);
			}*/
		}
	}
}
