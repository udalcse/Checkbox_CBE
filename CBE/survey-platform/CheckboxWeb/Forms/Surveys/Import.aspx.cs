using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Globalization.Text;
using Checkbox.Management;
using Checkbox.Styles;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Page;
using Prezza.Framework.Security;
using System.Xml;
using Prezza.Framework.ExceptionHandling;
using Checkbox.Progress;

namespace CheckboxWeb.Forms.Surveys
{
    public partial class Import : SecuredPage
    {
        [QueryParameter("f")]
        public int? FolderId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? SelectedStyleTemplateIdPC
        {
            get
            {
                if (_styleListPC.Items.Count == 0)
                {
                    return null;
                }

                return Utilities.AsInt(_styleListPC.SelectedValue);
            }
        }

        public int? SelectedStyleTemplateIdMobile
        {
            get { return Utilities.AsInt(_styleListMobile.SelectedValue); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsFileUploaded
        {
            get { return _uploader.GetUploadedFileDocument() != null; }
        }

        /// <summary>
        /// Bind init events
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            if (!IsFileUploaded)
                _wizard.MoveTo(_stepStart);

            Master.HideDialogButtons();
            _wizard.FinishButtonClick += _wizard_FinishButtonClick;
            _wizard.NextButtonClick += _wizard_NextButtonClick;
            _wizard.ActiveStepChanged += _wizard_ActiveStepChanged;
            _wizard.CancelButtonClick += _wizard_CancelButtonClick;

            Master.SetTitle(WebTextManager.GetText("/pageText/forms/surveys/import.aspx/importSurvey"));

            BindFolderList();

            //populate styles
            PopulateStyleList(_styleListPC, StyleTemplateType.PC, ApplicationManager.AppSettings.DefaultStyleTemplate);

            _styleListMobile.DataSource =
                MobileStyleManager.GetAllStyles(TextManager.DefaultLanguage).OrderByDescending(s => s.IsDefault);
            _styleListMobile.DataBind();

            _closeBtn.Click += _closeBtn_Click;

            foreach (WizardStepBase step in _wizard.WizardSteps)
            {
                step.AllowReturn = false;
            }
        }

        void _wizard_FinishButtonClick(object sender, WizardNavigationEventArgs e)
        {
            RegisterIncludes();
            RegisterStartProgressScript(
                ProgressKey,
                ResolveUrl("~/Forms/Surveys/DoImport.aspx"),
                GetWorkerUrlData(),
                ProgressContainerId,
                OnErrorMethod,
                OnUpdateMethod,
                OnCompleteMethod);
        }

        /// <summary>
        /// Ok Button click handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _closeBtn_Click(object sender, EventArgs e)
        {
            int responseTemplateId = -1;
            string url = ApplicationManager.ApplicationPath + "/Forms/Manage.aspx";
            if (int.TryParse(ProgressProvider.GetProgress(ProgressKey).AdditionalData, out responseTemplateId))
            {
                url += "?s=" + responseTemplateId;
            }

            if (responseTemplateId > 0)
            {
                int selectedFolderId = int.Parse(_folderList.SelectedValue);

                if (selectedFolderId == 0)
                {
                    RootFormFolder folder = FolderManager.GetRoot();
                    folder.Add(responseTemplateId);
                }
                else
                {
                    FormFolder folder = FolderManager.GetFolder(selectedFolderId);

                    if (folder == null)
                    {
                        throw new Exception("Unable to load folder with id: " + selectedFolderId);
                    }

                    folder.Add(responseTemplateId);
                }

                //Close import dialog and select imported survey
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "closeWindow", "closeWindowAndRedirectParentPage(null,null,'" + url + "');", true);
            }
            else
            {
                //Close import dialog and select imported survey
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "closeWindow", "closeWindow(null,null,'" + url + "');", true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _wizard_NextButtonClick(object sender, WizardNavigationEventArgs e)
        {
            if (e.CurrentStepIndex == 0)
            {
                if (!_uploader.ValidateFile())
                {
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        /// Handle cancel click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _wizard_CancelButtonClick(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "CloseWindow", "closeWindow();return false;", true);
        }

        /// <summary>
        /// Bind lists when moving to second page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _wizard_ActiveStepChanged(object sender, EventArgs e)
        {
            if (_wizard.ActiveStepIndex == 1 && string.IsNullOrEmpty(_nameTxt.Text))
            {
                _nameTxt.Text = ResponseTemplateManager.GetUniqueName(_uploader.SurveyName, WebTextManager.GetUserLanguage());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string GetWorkerUrl()
        {
            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string GenerateProgressKey()
        {
            return Session.SessionID + "_ImportSurvey";
        }

        /// <summary>
        /// Get worker url params
        /// </summary>
        /// <returns></returns>
        protected override object GetWorkerUrlData()
        {
            return new { f = FolderId ?? -1, s = SelectedStyleTemplateIdPC ?? -1, mid = SelectedStyleTemplateIdMobile ?? - 1,
                p = Server.UrlEncode(_uploader.UploadedTempFilePath), n = Server.UrlEncode(_nameTxt.Text.Trim()) };
        }


        /// <summary>
        /// 
        /// </summary>
        private void BindFolderList()
        {
            //Get folder list
            List<LightweightAccessControllable> folderList = FolderManager.ListAccessibleFolders(
                UserManager.GetCurrentPrincipal(),
                "FormFolder.Read");

            //Strip HTML and truncate
            foreach (LightweightAccessControllable folder in folderList)
            {
                folder.Name = Utilities.StripHtml(folder.Name, 64);
            }

            _folderList.DataSource = folderList;

            _folderList.DataTextField = "Name";
            _folderList.DataValueField = "ID";

            _folderList.DataBind();

            //Add "Root" and set selected value
            _folderList.Items.Insert(
                0,
                new ListItem(WebTextManager.GetText("/pageText/forms/surveys/create.aspx/rootFolder"), "0"));

            if (FolderId.HasValue
                && _folderList.Items.FindByValue(FolderId.ToString()) != null)
            {
                _folderList.SelectedValue = FolderId.ToString();
            }
        }

        /// <summary>
        /// Fills one list with styles of the defined type
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="type"></param>
        private void PopulateStyleList(DropDownList ddl, StyleTemplateType type, int defaultStyleId)
        {
            ddl.DataSource = StyleTemplateManager.ListStyleTemplatesForDataBinding(UserManager.GetCurrentPrincipal(), type);
            ddl.DataBind();
            ddl.Items.Insert(
                0,
                new ListItem(WebTextManager.GetText("/pageText/forms/surveys/import.aspx/none"), "0"));
            if (defaultStyleId > 0
                && ddl.Items.FindByValue(defaultStyleId.ToString()) != null)
            {
                ddl.SelectedValue = defaultStyleId.ToString();
            }
        }

        /// <summary>
        /// Get the current folder
        /// </summary>
        /// <returns></returns>
        private FormFolder CurrentFolder
        {
            get
            {

                try
                {
                    if (!FolderId.HasValue || FolderId.Value <= 0)
                        return FolderManager.GetRoot();


                    return FolderManager.GetFolder(FolderId.Value);
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
