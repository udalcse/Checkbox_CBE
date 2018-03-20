using System;
using System.Web;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Web;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    public partial class NonOfAboveEntry : Checkbox.Web.Common.UserControlBase
    {
        [QueryParameter("isNew")]
        public bool IsNew { get; set; }

        [QueryParameter("i")]
        public int ItemId { get; set; }

        [QueryParameter("lid")]
        public int? LibraryTemplateId { get; set; }

        [QueryParameter("c")]
        public int? ColumnNumber { get; set; }

        public bool IsMatrix { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool RestrictHtmlOptions { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public int PagePosition { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string OptionLabelContent { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string OptionLabelClientID
        {
            get { return _labelTxt.ClientID; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string OptionLabelText
        {
            get { return _labelTxt.Text; }
        }

        protected bool AllowOption { set; get; }

        public string AllowOptionCheckedChangedHandlerName
        {
            get { return "allowOptionCheckedChanged_" + ClientID; }
        }

        public string ToggleOptionControlClientId { set; get; }
        
        public string EditOptionHtmlLinkClientId 
        {
            get { return "editNoneOfAboveHtmlLink_" + ClientID; }
        }

        /// <summary>
        /// Enables option formatting in html editor
        /// </summary>
        protected bool DisableHtmlFormattedOption { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string LanguageCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ResponseTemplateId { get; set; }

        /// <summary>
        /// Fires before redirect to html options editor
        /// </summary>
        public event EventHandler OnHtmlEditorRedirect;

        private void PostData()
        {
            if (OnHtmlEditorRedirect != null)
                OnHtmlEditorRedirect(this, new EventArgs());

            var html = Utilities.AdvancedHtmlDecode(_currentOptionHtml.Value);

            var uri = ResolveUrl("~/Forms/Surveys/HtmlEditor.aspx") +
                  "?s=" + ResponseTemplateId +
                  "&p=" + PagePosition +
                  "&i=" + ItemId +
                  "&l=" + LanguageCode +
                  "&c=" + ColumnNumber +
                  "&isNew=" + IsNew +
                  "&isMatrix=" + IsMatrix +
                  "&lid=" + (LibraryTemplateId.HasValue ? LibraryTemplateId.ToString() : string.Empty) +
                  ((Request == null || string.IsNullOrEmpty(Request["w"])) ? "" : ("&w=" + Request["w"]));
            uri += "&isBehavior=true&optionType=noneofabove&html=" + html;

            Response.Redirect(uri);
        }

        protected void PostFromOnClick(object sender, EventArgs eventArgs)
        {
            PostData();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultOptionText"></param>
        /// <param name="templateId"></param>
        /// <param name="pagePosition"></param>
        /// <param name="editMode"></param>
        /// <param name="language"></param>
        /// <param name="allowOption"> </param>
        /// <param name="disableHtmlFormattedOption"> </param>
        /// <param name="labelText"></param>
        /// <param name="toggleOptionControlClientId"></param>
        /// <param name="isMatrix"></param>
        public void Initialize(string defaultOptionText, int templateId, int? pagePosition, EditMode editMode,
            string language, bool allowOption, bool disableHtmlFormattedOption, string labelText, string toggleOptionControlClientId, bool isMatrix)
        {
            IsMatrix = isMatrix;
            IsNew = Convert.ToBoolean(HttpContext.Current.Request.Params["isNew"]);
            ItemId = Convert.ToInt32(HttpContext.Current.Request.Params["i"]);

            int lid;
            LibraryTemplateId = null;
            if (int.TryParse(HttpContext.Current.Request.Params["lid"], out lid))
                LibraryTemplateId = lid;

            int c;
            ColumnNumber = null;
            if (int.TryParse(HttpContext.Current.Request.Params["c"], out c))
                ColumnNumber = c;

            _lbl.Text = labelText;
            ToggleOptionControlClientId = toggleOptionControlClientId;
            DisableHtmlFormattedOption = disableHtmlFormattedOption;
            AllowOption = allowOption;
            ResponseTemplateId = templateId;
            PagePosition = pagePosition ?? 0;
            LanguageCode = language;

            //Initialize pipeSelector
            switch (editMode)
            {
                case EditMode.Survey:
                    _pipeSelectorForOption.Initialize(templateId, pagePosition, language, _labelTxt.ClientID);
                    break;
                case EditMode.Library:
                    _pipeSelectorForOption.Initialize(null, null, language, _labelTxt.ClientID);
                    break;
                case EditMode.Report:
                    _pipeSelectorForOption.Visible = false;
                    break;
            }

            OptionLabelContent = Utilities.IsHtmlFormattedText(defaultOptionText) ? Utilities.AdvancedHtmlDecode(defaultOptionText) : defaultOptionText;
            _labelTxt.Text = Utilities.AdvancedHtmlDecode(defaultOptionText);
        }
    }
}