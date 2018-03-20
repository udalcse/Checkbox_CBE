//===============================================================================
// Checkbox UI Controls
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================

using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Checkbox.Management;
using Prezza.Framework.Data;
using Image = System.Web.UI.WebControls.Image;

namespace Checkbox.Web.UI.Controls
{
    /// <summary>
    /// Control to provide interface to upload files/etc. to the web server
    /// </summary>
    public class FileUploadControl : Common.WebControlBase, INamingContainer
    {
        private string _buttonCssClass;
        private int _controlWidth;
        private TextBox _href;
        private Panel _hrefPlaceHolder;
        private RadioButton _hrefUploadRadio;

        private string _localFileTypeFilter;
        private HtmlInputFile _localPathBtn;
        private Panel _localPlaceHolder;

        private RadioButton _localUploadRadio;

        private HtmlAnchor _postbackLinkButton;
        private HtmlAnchor _previewFromLocalFolderLinkButton;
        private HtmlAnchor _previewFromWebSiteLinkButton;

        private Image _previewImage;
        private string _previewName;

        private Label _previewPath;
        private Panel _previewPlaceHolder;
        private Label _previewWarning;
        private Label _previewToUpload;

        private string _radioCssClass;
        private Panel _radioPanel;

        public string OnClientPreviewClick { get; set; }

        #region Overrides

        /// <summary>
        /// Override OnInit to call EnsureChildControls to make sure child controls exist for postback event processing.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnInit(EventArgs e)
        {
            //Make sure controls exist for postback processing
            EnsureChildControls();

            if (!Page.IsPostBack)
            {
                PreviewedPostedFile = null;
                PreviewedPostedFileName = string.Empty;
            }

            RegisterClientScriptInclude(GetType(), "fileUploadScript", ApplicationManager.ApplicationRoot + "/Resources/FileUploadHandler.js");

            base.OnInit(e);
        }

        /// <summary>
        /// Set controls visibility
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
                if (!AllowHrefUpload)
                {
                    _hrefUploadRadio.Visible = false;
                    _localUploadRadio.Visible = false;
                }

                if (!AllowLocalUpload)
                {
                    _localUploadRadio.Visible = false;
                    _localPlaceHolder.Visible = false;
                    _hrefUploadRadio.Visible = false;
                }

                if (LocalUpload)
                {
                    _localPlaceHolder.Visible = true;
                    _localUploadRadio.Checked = true;
                }
                else
                {
                    _localPlaceHolder.Visible = false;
                    _localUploadRadio.Checked = false;
                }

                if (HrefUpload)
                {
                    _hrefUploadRadio.Checked = true;
                    _hrefPlaceHolder.Visible = true;
                }
                else
                {
                    _hrefUploadRadio.Checked = false;
                    _hrefPlaceHolder.Visible = false;
                }

                if (!AllowPreview)
                {
                    if (_previewFromLocalFolderLinkButton != null)
                        _previewFromLocalFolderLinkButton.Visible = false;
                }
            }

        }

        /// <summary>
        /// Create child controls that comprise the control.
        /// </summary>
        protected override void CreateChildControls()
        {
            //Add input for local upload
            _radioPanel = new Panel();
            _radioPanel.Style.Add("padding", "10px");

            _localUploadRadio = new RadioButton
                                    {
                                        Text = WebTextManager.GetText("/pageText/controls/fileuploadcontrol.aspx/uploadLocal"),//"Upload Image from Local Folder",
                                        GroupName = "UploadMode",
                                        ToolTip = WebTextManager.GetText("/pageText/controls/fileuploadcontrol.aspx/uploadLocalTooltip"),// "Choose an image to upload from your computer.",
                                        AutoPostBack = true,
                                        CssClass = RadioCssClass,
                                        ID = "localUploadRadio",
                                        Enabled = this.Enabled
                                    };
            _localUploadRadio.CheckedChanged += localUploadRadio_CheckedChanged;

            _hrefUploadRadio = new RadioButton
                                   {
                                       Text = WebTextManager.GetText("/pageText/controls/fileuploadcontrol.aspx/uploadWeb"),// "Use Image from Web Site",
                                       GroupName = "UploadMode",
                                       ToolTip = WebTextManager.GetText("/pageText/controls/fileuploadcontrol.aspx/uploadWebTooltip"),// "Choose an image to upload from an accessible URL.",
                                       AutoPostBack = true,
                                       CssClass = RadioCssClass,
                                       ID = "hrefUploadRadio",
                                       Enabled = this.Enabled
                                   };
            _hrefUploadRadio.CheckedChanged += hrefUploadRadio_CheckedChanged;

            _radioPanel.Controls.Add(_localUploadRadio);
            _radioPanel.Controls.Add(new LiteralControl("<br />"));
            _radioPanel.Controls.Add(_hrefUploadRadio);

            Controls.Add(_radioPanel);

            //Add local button
            _localPlaceHolder = new Panel();
            _localPlaceHolder.Style.Add("padding", "10px");

            _localPathBtn = new HtmlInputFile
                                {
                                    Accept = LocalFileTypeFilter,
                                    ID = "localPathBtn",
                                    Disabled = !this.Enabled
                                };
            _localPathBtn.Attributes.Add("alt", WebTextManager.GetText("/pageText/controls/fileuploadcontrol.aspx/uploadLocalPath"));// "Local path of image to upload."

            _localPlaceHolder.Controls.Add(_localPathBtn);

            Controls.Add(_localPlaceHolder);

            //Add href input
            _hrefPlaceHolder = new Panel();
            _hrefPlaceHolder.Style.Add("padding", "10px");

            _href = new TextBox
                        {
                            ToolTip = WebTextManager.GetText("/pageText/controls/fileuploadcontrol.aspx/uploadWebUrl"),//	"URL of image to upload.", 
                            Width = 360,
                            ID = "href",
                            Enabled = this.Enabled
                        };

            _hrefPlaceHolder.Controls.Add(_href);
            Controls.Add(_hrefPlaceHolder);

            _localPlaceHolder.Controls.Add(new LiteralControl("<br /><br />"));

            //PostBack button
            _postbackLinkButton = new HtmlAnchor();
            _postbackLinkButton.Style[HtmlTextWriterStyle.Visibility] = "hidden";
            _localPlaceHolder.Controls.Add(_postbackLinkButton);
            _postbackLinkButton.ID = "postback";
            _postbackLinkButton.ServerClick += previewButton_Click;

            //PreviewFromLocal button and panel
            _previewFromLocalFolderLinkButton = new HtmlAnchor();
            _previewFromLocalFolderLinkButton.InnerHtml = "<span>" + WebTextManager.GetText("/pageText/controls/fileuploadcontrol.aspx/uploadPreviewText") + "</span>";
            _previewFromLocalFolderLinkButton.HRef = "#";
            _previewFromLocalFolderLinkButton.Attributes["class"] = "ckbxButton roundedCorners shadow999 border999 orangeButton";
            _previewFromLocalFolderLinkButton.Attributes["uframeignore"] = "true";
            _previewFromLocalFolderLinkButton.ID = "_previewFromLocalFolderLinkButton";
            _previewFromLocalFolderLinkButton.Disabled = !this.Enabled;
            _localPlaceHolder.Controls.Add(_previewFromLocalFolderLinkButton);
            _previewFromLocalFolderLinkButton.Attributes["onclick"] = GetLoadFileIntoSessionScript();

            //PreviewToUpload label
            _previewToUpload = new Label
            {
                Text =
                    WebTextManager.GetText(
                        "/pageText/controls/fileuploadcontrol.aspx/uploadPreview")
            };
            _localPlaceHolder.Controls.Add(_previewToUpload);

            _hrefPlaceHolder.Controls.Add(new LiteralControl("<br /><br />"));

            //_previewFromWebSiteLinkButton
            _previewFromWebSiteLinkButton = new HtmlAnchor();
            _previewFromWebSiteLinkButton.InnerHtml = "<span>" + WebTextManager.GetText("/pageText/controls/fileuploadcontrol.aspx/uploadPreviewText") + "</span>";
            _previewFromWebSiteLinkButton.HRef = "#";
            _previewFromWebSiteLinkButton.Attributes["class"] = "ckbxButton roundedCorners shadow999 border999 orangeButton";
            _previewFromWebSiteLinkButton.ID = "_previewFromWebSiteLinkButton";
            _previewFromWebSiteLinkButton.Disabled = !this.Enabled;
            _hrefPlaceHolder.Controls.Add(_previewFromWebSiteLinkButton);
            _previewFromWebSiteLinkButton.ServerClick += previewButton_Click;

            _previewPlaceHolder = new Panel();
            _previewPlaceHolder.Style.Add("padding", "10px");

            _previewWarning = new Label { CssClass = "ErrorMessageFld", Visible = false, ID = "previewWarningLbl"};

            _previewImage = new Image { ID = "previewImg" };
            _previewPlaceHolder.Controls.Add(_previewWarning);
            _previewPlaceHolder.Controls.Add(_previewImage);
            _previewPlaceHolder.Controls.Add(new LiteralControl("<br />"));

            _previewPath = new Label { CssClass = "PrezzaNormal", ID = "previewPath" };
            _previewPlaceHolder.Controls.Add(_previewPath);

            Controls.Add(_previewPlaceHolder);

            if (!Page.IsPostBack)
            {
                Href = ApplicationManager.AppSettings.HeaderLogo;
                _previewPath.Text = ApplicationManager.AppSettings.HeaderLogo;
            }

            if (!String.IsNullOrEmpty(Href))
            {
                UploadMode = "Href";
            }
            else
                _previewPlaceHolder.Visible = false;
        }

        /// <summary>
        /// Set "Enable" property of child controls
        /// </summary>
        /// <param name="isEnable"></param>
        private void SetEnableChildControls(bool isEnable)
        {
            _href.Enabled = isEnable;
            _hrefPlaceHolder.Enabled = isEnable;
            _hrefUploadRadio.Enabled = isEnable;

            _localPathBtn.Disabled = !isEnable;
            _localPlaceHolder.Enabled = isEnable;

            _localUploadRadio.Enabled = isEnable;

            _postbackLinkButton.Disabled = !isEnable;
            _previewFromLocalFolderLinkButton.Disabled = !isEnable;
            _previewFromWebSiteLinkButton.Disabled = !isEnable;

            _previewImage.Enabled = isEnable;

            _previewPlaceHolder.Enabled = isEnable;
            _previewToUpload.Enabled = isEnable;

            _radioPanel.Enabled = isEnable;

            if (isEnable)
                _previewFromLocalFolderLinkButton.Attributes["onclick"] = GetLoadFileIntoSessionScript();
            else
                _previewFromLocalFolderLinkButton.Attributes["onclick"] = "return false;";
        }

        /// <summary>
        /// Override Enable property
        /// </summary>
        public override bool Enabled
        {
            get
            {
                return base.Enabled;                
            }
            set
            {
                SetEnableChildControls(value);
                base.Enabled = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Upload the file to the database.
        /// </summary>
        /// <returns>ID of the saved image.</returns>
        public string UploadFile(out bool success)
        {
            if (PreviewedPostedID != -1)
            {
                success = true;
                return "~/ViewContent.aspx?ImageId=" + PreviewedPostedID.ToString();
            }

            string ext;

            _previewPath.Text = string.Empty;

            string name;
            if (ValidateFile(out name, out ext))
            {
                //TODO: Make more general for other file types
                if (UploadMode == "Local")
                {
                    StoreFile();
                }
                success = true;
                return Href;
            }
            success = false;
            return ApplicationManager.AppSettings.HeaderLogo;
        }

        /// <summary>
        /// Checks if file name is valid
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsFileNameValid(string name, HttpPostedFile f)
        {
            if (name == string.Empty && !string.IsNullOrEmpty(PreviewedPostedFileName))
            {
                name = PreviewedPostedFileName;

                if (name == string.Empty)
                {
                    _previewWarning.Text = WebTextManager.GetText("/pageText/controls/fileuploadcontrol.aspx/uploadPreviewWarningExt");// "Please select a .gif, .jpg, or .png file.";
                    _previewWarning.Visible = true;
                    _previewImage.Visible = false;
                    return false;
                }

                if (PreviewedPostedFile.Length == 0)
                {
                    _previewWarning.Text = string.Format(WebTextManager.GetText("/pageText/controls/fileuploadcontrol.aspx/uploadPreviewWarningFile"), name);// "The selected image file [" + name + "] does not exist or has no data.";
                    _previewWarning.Visible = true;
                    _previewImage.Visible = false;
                    return false;
                }
            }
            else
            {
                if (name == string.Empty)
                {
                    _previewWarning.Text = WebTextManager.GetText("/pageText/controls/fileuploadcontrol.aspx/uploadPreviewWarningExt");// "Please select a .gif, .jpg, or .png file.";
                    _previewWarning.Visible = true;
                    _previewImage.Visible = false;
                    return false;
                }

                if (f.ContentLength == 0)
                {
                    _previewWarning.Text = string.Format(WebTextManager.GetText("/pageText/controls/fileuploadcontrol.aspx/uploadPreviewWarningFile"), name);// "The selected image file [" + name + "] does not exist or has no data.";
                    _previewWarning.Visible = true;
                    _previewImage.Visible = false;
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Stores a file to the database. All the information stored in Session.
        /// </summary>
        public void StoreFile()
        {
            HttpPostedFile httpfile = Context.Session["file_" + _localPathBtn.ClientID] as HttpPostedFile;
            byte[] byteArray = Context.Session["buffer_" + _localPathBtn.ClientID] as byte[];

            Context.Session.Remove("file_" + _localPathBtn.ClientID);
            Context.Session.Remove("buffer_" + _localPathBtn.ClientID);

            if (httpfile != null && httpfile.FileName != string.Empty)
            {
                string contentType = httpfile.ContentType;

                if (byteArray == null)
                {
                    byteArray = new byte[httpfile.ContentLength];
                    httpfile.InputStream.Read(byteArray, 0, byteArray.Length);
                }

                PreviewedPostedID = DbUtility.SaveImage(byteArray, contentType, null, Path.GetFileName(httpfile.FileName), Guid.NewGuid().ToString());

                //Store some information
                PreviewedPostedFile = byteArray;
                PreviewedPostedFileName = httpfile.FileName;

                Href = "~/ViewContent.aspx?ImageID=" + PreviewedPostedID;
            }
        }


        #endregion

        #region Private Methods

        private bool ValidateFile(out string name, out string extension)
        {
            _previewPlaceHolder.Visible = true;

            name = string.Empty;
            extension = string.Empty;

            if (UploadMode == "Local")
            {
                HttpPostedFile httpfile = Context.Session["file_" + _localPathBtn.ClientID] as HttpPostedFile;

                if (httpfile != null)
                {
                    name = httpfile.FileName;
                    if (!IsFileNameValid(name, httpfile))
                        return false;

                    //TODO: Make more general for other file types
                    if (!name.ToLower().Contains("viewcontent.aspx"))
                    {
                        if (!Path.HasExtension(name))
                        {
                            _previewWarning.Text = WebTextManager.GetText("/pageText/controls/fileuploadcontrol.aspx/uploadPreviewWarningExt");// "Please select a .gif, .jpg, or .png file.";
                            _previewWarning.Visible = true;
                            _previewImage.Visible = false;
                            return false;
                        }

                        extension = Path.GetExtension(name);

                        //TODO: Make more general for other file types
                        if (string.Compare(extension, ".gif", true) != 0 && string.Compare(extension, ".jpg", true) != 0 && string.Compare(extension, ".png", true) != 0)
                        {
                            _previewWarning.Text = WebTextManager.GetText("/pageText/controls/fileuploadcontrol.aspx/uploadPreviewWarningExt");// "Please select a .gif, .jpg, or .png file.";
                            _previewWarning.Visible = true;
                            _previewImage.Visible = false;
                            return false;
                        }
                    }
                }
                else
                {
                    _previewWarning.Text = WebTextManager.GetText("/pageText/controls/fileuploadcontrol.aspx/uploadPreviewWarningExt");// "Please select a .gif or .jpg file.";
                    _previewWarning.Visible = true;
                    _previewImage.Visible = false;
                    return false;
                }
            }
            else if (UploadMode == "Href")
            {
                if (String.IsNullOrEmpty(_href.Text))
                {
                    _previewWarning.Text = WebTextManager.GetText("/pageText/controls/fileuploadcontrol.aspx/uploadPreviewWarningNoUrl");// "Please enter an accessible URL to the image.";
                    _previewWarning.Visible = true;
                    _previewImage.Visible = false;
                    return false;
                }
                name = _href.Text;
            }

            _previewWarning.Visible = false;
            _previewPath.Visible = true;

            return true;
        }

        /// <summary>
        /// Script which should be assigned to the button onclick event, if a file is needed.
        /// </summary>
        private string GetLoadFileIntoSessionScript()
        {
            return string.Format("startFileUploading('{0}', '{1}', '{2}?controlID={0}');",
                                 _localPathBtn.ClientID,
                                 _postbackLinkButton.ClientID,
                                 (Page == null ? HTTPHandlerURL : Page.ResolveUrl(HTTPHandlerURL))
                );
        }

        #endregion

        #region EventHandlers

        private void previewButton_Click(object sender, EventArgs e)
        {
            string name;
            string ext;

            if (ValidateFile(out name, out ext))
            {
                _previewPath.Text = name;

                //Name and extension check out ok, now make a preview.  For Href, no problem, just set the link
                if (UploadMode == "Href")
                {
                    _previewImage.ToolTip = name;
                    _previewImage.ImageUrl = name;
                    _previewImage.Visible = true;
                }
                //If not href, need to upload and view
                else
                {
                    StoreFile();
                }
            }
            else
            {
                _previewPath.Text = string.Empty;
            }
        }

        private void localUploadRadio_CheckedChanged(object sender, EventArgs e)
        {
            var button = (RadioButton)sender;
            UploadMode = button.Checked ? "Local" : "Href";

            _localPlaceHolder.Visible = true;
            _hrefPlaceHolder.Visible = false;
            _previewPath.Text = string.Empty;
            PreviewedPostedFile = null;
            PreviewedPostedFileName = string.Empty;
            _previewImage.Visible = false;
            _previewWarning.Visible = false;
        }

        private void hrefUploadRadio_CheckedChanged(object sender, EventArgs e)
        {
            var button = (RadioButton)sender;
            UploadMode = button.Checked ? "Href" : "Local";

            _localPlaceHolder.Visible = false;
            _hrefPlaceHolder.Visible = true;
            _previewPath.Text = string.Empty;
            PreviewedPostedFile = null;
            PreviewedPostedFileName = string.Empty;
            Href = ApplicationManager.AppSettings.HeaderLogo;
            _previewPath.Text = ApplicationManager.AppSettings.HeaderLogo;
            _previewImage.Visible = true;
            _previewWarning.Visible = false;

            PreviewedPostedID = -1;
        }

        #endregion

        #region Properties

        private string UploadMode
        {
            get
            {
                if (HttpContext.Current.Session["UploadMode"] == null)
                {
                    if (AllowLocalUpload)
                    {
                        HttpContext.Current.Session["UploadMode"] = "Local";
                    }
                    else if (AllowHrefUpload)
                    {
                        HttpContext.Current.Session["UploadMode"] = "Href";
                    }
                    else
                    {
                        HttpContext.Current.Session["UploadMode"] = "UNKNOWN";
                    }
                }

                return (string)HttpContext.Current.Session["UploadMode"];
            }

            set { HttpContext.Current.Session["UploadMode"] = value; }
        }

        private bool LocalUpload
        {
            get
            {
                if (UploadMode == "Local")
                    return true;

                return false;
            }
        }

        private bool HrefUpload
        {
            get
            {
                if (UploadMode == "Href")
                    return true;

                return false;
            }
        }

        private static byte[] PreviewedPostedFile
        {
            get { return (byte[])HttpContext.Current.Session["PreviewedPostedFile"]; }
            set { HttpContext.Current.Session["PreviewedPostedFile"] = value; }
        }

        private static string PreviewedPostedFileName
        {
            get { return (string)HttpContext.Current.Session["PreviewedPostedFileName"]; }
            set { HttpContext.Current.Session["PreviewedPostedFileName"] = value; }
        }

        private int PreviewedPostedID
        {
            get
            {
                if (ViewState["PreviewedPostedID"] == null)
                {
                    ViewState["PreviewedPostedID"] = -1;
                }

                return (int)ViewState["PreviewedPostedID"];
            }

            set { ViewState["PreviewedPostedID"] = value; }
        }

        /// <summary>
        /// URL for HTTPHandler
        /// </summary>
        private string HTTPHandlerURL
        {
            get { return "~/UFrameFileUploadHandler.axd"; }
        }

        /// <summary>
        /// Get whether the file is an HREF or a local file.
        /// </summary>
        public bool IsHref
        {
            get
            {
                if (UploadMode == "Href")
                {
                    return true;
                }

                return false;
            }
            set
            {
                EnsureChildControls();

                if (value)
                {
                    if (AllowHrefUpload)
                    {
                        ViewState["UploadMode"] = "Href";
                        _hrefUploadRadio.Checked = true;
                    }
                }
                else
                {
                    if (AllowLocalUpload)
                    {
                        ViewState["UploadMode"] = "Local";
                        _localUploadRadio.Checked = true;
                    }
                }
            }
        }

        /// <summary>
        /// Get the HREF set.
        /// </summary>
        public string Href
        {
            get { return _href.Text; }
            set
            {
                if (string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(_previewImage.ImageUrl))
                    return;

                EnsureChildControls();
                _href.Text = value;

                _previewImage.ImageUrl = value;
                _previewImage.Visible = true;
                _previewPlaceHolder.Visible = true;
                _previewWarning.Visible = false;
            }
        }

        /// <summary>
        /// Get/Set the width of the control.
        /// </summary>
        public int ControlWidth
        {
            get
            {
                if (_controlWidth == 0)
                {
                    _controlWidth = 400;
                }

                return _controlWidth;
            }
            set { _controlWidth = value; }
        }

        /// <summary>
        /// Get/Set whether upload from a user's local drive is allowed.
        /// </summary>
        public bool AllowLocalUpload { get; set; }

        /// <summary>
        /// Get/Set whether upload from a URL reference is allowed.
        /// </summary>
        public bool AllowHrefUpload { get; set; }

        /// <summary>
        /// Get/set whether a preview is allowed.
        /// </summary>
        public bool AllowPreview { get; set; }

        /// <summary>
        /// Get/Set the local file types allowed.
        /// </summary>
        public string LocalFileTypeFilter
        {
            get
            {
                if (_localFileTypeFilter == null)
                {
                    _localFileTypeFilter = "*/*";
                }

                return _localFileTypeFilter;
            }

            set { _localFileTypeFilter = value; }
        }

        /// <summary>
        /// Get/set the css class for checkboxes
        /// </summary>
        public string RadioCssClass
        {
            get
            {
                if (_radioCssClass == null)
                {
                    _radioCssClass = "PrezzaBold";
                }

                return _radioCssClass;
            }

            set { _radioCssClass = value; }
        }

        /// <summary>
        /// Get/Set the css class for the buttons
        /// </summary>
        public string ButtonCssClass
        {
            get
            {
                if (_buttonCssClass == null)
                {
                    _buttonCssClass = "PrezzaButton";
                }

                return _buttonCssClass;
            }

            set { _buttonCssClass = value; }
        }

        /// <summary>
        /// Get/Set the name of the previewed image
        /// </summary>
        public string PreviewName
        {
            get
            {
                if (string.IsNullOrEmpty(_previewName))
                {
                    _previewName = "upload_preview";
                }

                return _previewName;
            }
            set { _previewName = value; }
        }

        #endregion
    }
}