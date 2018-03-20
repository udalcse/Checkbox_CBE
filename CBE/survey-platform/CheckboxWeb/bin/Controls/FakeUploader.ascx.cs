using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Forms;
using Checkbox.Web;

namespace CheckboxWeb.Controls
{
    public partial class FakeUploader : System.Web.UI.UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public string UploadKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SelectFilePromptTextId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected string SelectFilePrompt { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UploadedCallback { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int MaxUploadingLength { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int MaxDowloadErrorLength { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int MaxDowloadedLength { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            UploadKey = UploadKey ?? Session.SessionID;
            SelectFilePrompt = string.IsNullOrEmpty(SelectFilePromptTextId)
                                         ? "Select File to Upload..."
                                         : WebTextManager.GetText(SelectFilePromptTextId);

            Page.ClientScript.RegisterStartupScript(
                GetType(),
                "appRoot",
                "var _appRoot = '" + ResolveUrl("~") + "';",
                true);
        }
    }
}