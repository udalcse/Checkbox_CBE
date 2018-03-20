using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Checkbox.Globalization.Text;
using Checkbox.Common;

namespace Checkbox.Web.UI.Controls
{    
    /// <summary>
    /// Allows to upload a file while containing page is embedded into UFrame
    /// </summary>
    public class UFrameFileUploadControl : Common.WebControlBase, INamingContainer
    {
        /// <summary>
        /// Arguments that control returns when a file was uploaded
        /// </summary>
        public class UFrameFileUploadEventArgs : EventArgs 
        {
            /// <summary>
            /// A posted file
            /// </summary>
            public HttpPostedFile PostedFile
            {
                get;
                private set;
            }

            /// <summary>
            /// A file content
            /// </summary>
            public byte[] Buffer
            {
                get;
                private set;
            }
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="file"></param>
            public UFrameFileUploadEventArgs(HttpPostedFile file, byte[] buffer)
            {
                PostedFile = file;
                Buffer = buffer;
            }
        }

        /// <summary>
        /// Delegate for an event of uploaded file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public delegate void FileUploadedDelegate(object sender, UFrameFileUploadEventArgs args);

        /// <summary>
        /// Event that fires on postback when a file was successfully posted
        /// </summary>
        public event FileUploadedDelegate OnFileUploaded;
        
        private HtmlInputFile _file;
        private HtmlAnchor _uploadLinkButton;
        private HtmlAnchor _postbackLinkButton;



        /// <summary>
        /// Override OnInit to call EnsureChildControls to make sure child controls exist for postback event processing.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnInit(EventArgs e)
        {
            //Make sure controls exist for postback processing
            EnsureChildControls();

            base.OnInit(e);
        }

        /// <summary>
        /// Create child controls that comprise the control.
        /// </summary>
        protected override void CreateChildControls()
        {
            _postbackLinkButton = new HtmlAnchor();
            _postbackLinkButton.Style[HtmlTextWriterStyle.Visibility] = "hidden";
            Controls.Add(_postbackLinkButton);
            _postbackLinkButton.ID = "postback";
            _postbackLinkButton.ServerClick += new EventHandler(_postbackLinkButton_ServerClick);

            _file = new HtmlInputFile
            {
                Accept = LocalFileTypeFilter,
                ID = "file",
                Disabled = !this.Enabled
            };
            _file.Attributes["class"] = FileCssClass;
            Controls.Add(_file);

            if (string.IsNullOrEmpty(HTTPHandlerURL))
                HTTPHandlerURL = "~/UFrameFileUploadHandler.axd";

            _uploadLinkButton = new HtmlAnchor();
            _uploadLinkButton.InnerHtml = "<span>" + GetText(ButtonTextID) + "</span>";
            _uploadLinkButton.HRef = "#";
            _uploadLinkButton.ID = "upload";
            _uploadLinkButton.Attributes["uframeignore"] = "true;";
            if (this.Enabled)
                _uploadLinkButton.Attributes["onclick"] = buildOnClickScript();
            _uploadLinkButton.Attributes["class"] = ButtonCssClass;
            Controls.Add(_uploadLinkButton);
        }

        private string buildOnClickScript()
        {
            return string.Format("startFileUploading('{0}', '{1}', '{2}?controlID={0}');",
                _file.ClientID,
                _postbackLinkButton.ClientID,
                (Page == null ? HTTPHandlerURL : Page.ResolveUrl(HTTPHandlerURL))
            );
        }

        private void _postbackLinkButton_ServerClick(object sender, EventArgs e)
        {
            if (OnFileUploaded != null)
            {
                OnFileUploaded(this, new UFrameFileUploadEventArgs(
                    Context.Session["file_" + _file.ClientID] as HttpPostedFile,
                    Context.Session["buffer_" + _file.ClientID] as byte[]));
                Context.Session.Remove("file_" + _file.ClientID);
                Context.Session.Remove("buffer_" + _file.ClientID);
            }            
        }

        /// <summary>
        /// Filter for the files in the file open dialog
        /// </summary>
        public string LocalFileTypeFilter
        {
            get;
            set;
        }

        /// <summary>
        /// Class for the file selector
        /// </summary>
        public string FileCssClass
        {
            get;
            set;
        }

        /// <summary>
        /// Class for the upload button
        /// </summary>
        public string ButtonCssClass
        {
            get;
            set;
        }

        /// <summary>
        /// Get/set the language code if the current user's language isn't to be used
        /// </summary>
        public virtual string LanguageCode
        {
            get;
            set;
        }

        /// <summary>
        /// Text for the button
        /// </summary>
        public string ButtonTextID
        {
            get;
            set;
        }

        /// <summary>
        /// Text for the label
        /// </summary>
        public string LabelTextID
        {
            get;
            set;
        }

        /// <summary>
        /// URL for HTTPHandler
        /// </summary>
        public string HTTPHandlerURL
        {
            get;
            set;
        }

        /// <summary>
        /// Returns a text using a predefined language by TextID
        /// </summary>
        /// <param name="textID"></param>
        /// <returns></returns>
        protected virtual string GetText(string textID)
        {
            if (Utilities.IsNotNullOrEmpty(textID))
            {
                if (Utilities.IsNotNullOrEmpty(LanguageCode))
                {
                    return TextManager.GetText(textID, LanguageCode);
                }
                else
                {
                    return WebTextManager.GetText(textID);
                }
            }

            return string.Empty;
        }
    }

}
