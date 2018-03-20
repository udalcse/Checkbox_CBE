using System;
using System.Web;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Web;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    public partial class OtherOptionEntry : Checkbox.Web.Common.UserControlBase
    {
        [QueryParameter("isNew")]
        public bool IsNew { get; set; }

        [QueryParameter("i")]
        public int ItemId { get; set; }

        [QueryParameter("lid")]
        public int? LibraryTemplateId { get; set; }

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
        public string OtherLabelContent { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string OtherLabelClientID
        {
            get { return _otherLabelTxt.ClientID; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string OtherLabelText
        {
            get { return _otherLabelTxt.Text; }
        }

        protected bool AllowOtherButton { set; get; }

        /// <summary>
        /// Enables other option formatting in html editor
        /// </summary>
        protected bool DisableHtmlFormattedOtherOption { set; get; }

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
                OnHtmlEditorRedirect (this, new EventArgs());

            var html = Utilities.AdvancedHtmlDecode(_currentOtherHtml.Value);

            var uri = ResolveUrl("~/Forms/Surveys/HtmlEditor.aspx") +
                  "?s=" + ResponseTemplateId +
                  "&p=" + PagePosition +
                  "&i=" + ItemId +
                  "&l=" + LanguageCode +
                  "&isNew=" + IsNew +
                  "&lid=" + (LibraryTemplateId.HasValue ? LibraryTemplateId.ToString() : string.Empty) +
                  ((Request == null || string.IsNullOrEmpty(Request["w"])) ? "" : ("&w=" + Request["w"]));
            uri += "&isBehavior=true&optionType=other&html=" + html;

            Response.Redirect(uri);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
            {
                string target = Request.Params.Get("__EVENTTARGET");
                if (target == "HTML_EDITOR_CALLED_FOR_OTHER")
                    PostData();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultOtherText"></param>
        /// <param name="templateId"></param>
        /// <param name="pagePosition"></param>
        /// <param name="editMode"></param>
        /// <param name="language"></param>
        /// <param name="allowOtherButton"> </param>
        /// <param name="disableHtmlFormattedOtherOption"> </param>
        public void Initialize(string defaultOtherText, int templateId, int? pagePosition, EditMode editMode, string language, bool allowOtherButton, bool disableHtmlFormattedOtherOption)
        {
            IsNew = Convert.ToBoolean(HttpContext.Current.Request.Params["isNew"]);
            ItemId = Convert.ToInt32(HttpContext.Current.Request.Params["i"]);

            int lid;
            LibraryTemplateId = null;
            if (int.TryParse(HttpContext.Current.Request.Params["lid"], out lid))
                LibraryTemplateId = lid;

            DisableHtmlFormattedOtherOption = disableHtmlFormattedOtherOption;
            AllowOtherButton = allowOtherButton;
            ResponseTemplateId = templateId;
            PagePosition = pagePosition ?? 0;
            LanguageCode = language;

            //Initialize pipeSelector
            switch (editMode)
            {
                case EditMode.Survey:
                    _pipeSelectorForOther.Initialize(templateId, pagePosition, language, _otherLabelTxt.ClientID);
                    break;
                case EditMode.Library:
                    _pipeSelectorForOther.Initialize(null, null, language, _otherLabelTxt.ClientID);
                    break;
                case EditMode.Report:
                    _pipeSelectorForOther.Visible = false;
                    break;
            }

            OtherLabelContent = Utilities.IsHtmlFormattedText(defaultOtherText) ? Utilities.AdvancedHtmlDecode(defaultOtherText) : defaultOtherText;

            _otherLabelTxt.Text = Utilities.AdvancedHtmlDecode(defaultOtherText);
        }
    }
}