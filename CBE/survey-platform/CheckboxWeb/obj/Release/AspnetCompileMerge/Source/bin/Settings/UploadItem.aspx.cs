using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Management;
using Checkbox.Web;
using Checkbox.Web.Page;

namespace CheckboxWeb.Settings
{
    public partial class UploadItem : SettingsPage
    {
        private List<WebControl> _allInputs;
        
        /// <summary>
        /// 
        /// </summary>
        private IEnumerable<WebControl> AllInputs
        {
            get
            {
                return _allInputs ?? (_allInputs = new List<WebControl>
                                                       {
                                                           _fileType,
                                                           _addFileType,
                                                           _fileTypeError,
                                                           _allowedFileTypes,
                                                           _deleteFileType,
                                                           _restrictExport
                                                       });
            }
        }

        /// <summary>
        /// The edited list of allowed file types.
        /// </summary>
        private List<string> AllowedFileTypes
        {
            get { return (List<string>)ViewState["AllowedFileTypes"] ?? UploadItemManager.AllAllowedFileTypes; }
            set { ViewState["AllowedFileTypes"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            Master.IsDialog = false;
            base.OnPageInit();

            //Set up the page title with link back to mananger
            var titleControl = new PlaceHolder();

            var managerLink = new HyperLink
                                  {
                                      NavigateUrl = "~/Settings/Manage.aspx",
                                      Text = WebTextManager.GetText("/pageText/settings/manage.aspx/title")
                                  };

            var pageTitleLabel = new Label {Text = " - "};
            pageTitleLabel.Text += WebTextManager.GetText("/pageText/settings/uploadItem.aspx/title");

            titleControl.Controls.Add(managerLink);
            titleControl.Controls.Add(pageTitleLabel);

            Master.SetTitleControl(titleControl);

            Master.OkClick += Master_OkClick;

            if (!Page.IsPostBack)
            {
                _enableFileUpload.Checked = ApplicationManager.AppSettings.EnableUploadItem;

                _allowedFileTypes.DataSource = AllowedFileTypes;
                _allowedFileTypes.DataBind();

                _restrictExport.Checked = ApplicationManager.AppSettings.RestrictUploadFileExport;

                ToggleControls(AllInputs, _enableFileUpload.Checked);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Master_OkClick(object sender, EventArgs e)
        {
            ApplicationManager.AppSettings.EnableUploadItem = _enableFileUpload.Checked;

            UploadItemManager.UpdateAllowedFileTypes(AllowedFileTypes);
            ApplicationManager.AppSettings.RestrictUploadFileExport = _restrictExport.Checked;

            Master.ShowStatusMessage(WebTextManager.GetText("/pageText/settings/updateSuccessful"), StatusMessageType.Success);
        }

        /// <summary>
        /// Remove the selected items from list of allowed file types
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Delete_ClickEvent(object sender, EventArgs e)
        {
            //Use wrapper to implement error handling
            EventHandlerWrapper(sender, e, DeleteFileType);
        }

        /// <summary>
        /// Handle delete file type button clicks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DeleteFileType(object sender, EventArgs e)
        {
            var allowedFileTypes = (from ListItem item in _allowedFileTypes.Items where !item.Selected select item.Text).ToList();

            //rebind the list
            _allowedFileTypes.DataSource = allowedFileTypes;
            _allowedFileTypes.DataBind();

            AllowedFileTypes = allowedFileTypes;
        }

        /// <summary>
        /// Add the specified items to the list of allowed file types
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Add_ClickEvent(object sender, EventArgs e)
        {
            //Use wrapper to implement error handling
            EventHandlerWrapper(sender, e, AddFileType);
        }

        /// <summary>
        /// Handle add file type button clicks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AddFileType(object sender, EventArgs e)
        {
            string fileType = GetFormattedFileTypeInput();

            if (ValidateFileType(fileType))
            {
                List<string> allowedFileTypes = AllowedFileTypes;
                allowedFileTypes.Add(fileType);

                //rebind the list
                _allowedFileTypes.DataSource = allowedFileTypes;
                _allowedFileTypes.DataBind();

                AllowedFileTypes = allowedFileTypes;

                //clear the enter field
                _fileType.Text = string.Empty;
            }
        }

        /// <summary>
        /// Return a properly formatted and sanitized value for the file type input field.
        /// </summary>
        /// <returns></returns>
        private string GetFormattedFileTypeInput()
        {
            string fileType = _fileType.Text;

            if (Utilities.IsNullOrEmpty(fileType))
                return string.Empty;

            fileType = fileType.Trim();

            //ensure that if the file type is specified that it beings with a period
            if (fileType[0] != '.')
                fileType = string.Format(".{0}", fileType);

            return fileType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool ValidateFileType(string fileType)
        {
            if (Utilities.IsNullOrEmpty(fileType))
            {
                _fileTypeError.Text = WebTextManager.GetText("/pageText/settings/uploadItem.aspx/emptyFileTypeError");
                _fileTypeError.Visible = true;
                return false;
            }

            if (fileType.Trim().Equals(".", StringComparison.InvariantCultureIgnoreCase))
            {
                _fileTypeError.Text = WebTextManager.GetText("/pageText/settings/uploadItem.aspx/emptyFileTypeError");
                _fileTypeError.Visible = true;
                return false;
            }

            if (AllowedFileTypes.Contains(fileType))
            {
                _fileTypeError.Text = WebTextManager.GetText("/pageText/settings/uploadItem.aspx/duplicateFileTypeError");
                _fileTypeError.Visible = true;
                return false;
            }

            _fileTypeError.Visible = false;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controls"></param>
        /// <param name="isEnabled"></param>
        private void ToggleControls(IEnumerable<WebControl> controls, bool isEnabled)
        {
            foreach (WebControl control in controls)
            {
                control.Enabled = isEnabled;
            }

            ////Set the styles on the headers
            //filetypeOptionsH1.Attributes["class"] += isEnabled ? "" : "disabled";
            //exportOptionsH1.Attributes["class"] += isEnabled ? "" : "disabled";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void EnableFileUpload_ClickEvent(object sender, EventArgs e)
        {
            ToggleControls(AllInputs, _enableFileUpload.Checked);
        }
    }
}
