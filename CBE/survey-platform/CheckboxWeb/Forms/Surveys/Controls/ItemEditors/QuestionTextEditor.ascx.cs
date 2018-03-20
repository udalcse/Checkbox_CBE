using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Management;
using Checkbox.Styles;
using Checkbox.Users;
using Checkbox.Wcf.Services;
using Checkbox.Web;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    /// <summary>
    /// Control for editing question text.
    /// </summary>
    public partial class QuestionTextEditor : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        public string Text
        {
            get { return Utilities.ReplaceHtmlAttributes(Utilities.StripIframes(_questionText.Text), false); }
            set { _questionText.Text = value; }
        }

        /// <summary>
        /// Get/set current edit mode
        /// </summary>
        public string CurrentEditMode
        {
            get { return _currentEditMode.Text; }
            set { _currentEditMode.Text = value; }
        }

        /// <summary>
        /// Determine if this item was shown in dialog just after item creation
        /// </summary>
        public bool IsNew
        {
            get { return (Request.QueryString["isNew"] ?? "").ToLower() == "true"; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string LanguageCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool PageIsPostback { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsHorizontalRuleEnabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected int TemplateId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected int? PagePosition { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected EditMode EditMode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected CustomFieldType? ProfileFieldType {get; set;}
        /// <summary>
        /// 
        /// </summary>
        protected string StyleListUrl
        {
            get
            {
                if (Page == null || TemplateId <= 0)
                {
                    return string.Empty;
                }

                var rt = ResponseTemplateManager.GetResponseTemplate(TemplateId);

                if (rt == null || !rt.StyleSettings.StyleTemplateId.HasValue)
                {
                    return string.Empty;
                }

                //content_css: '<%=ResolveUrl("~/ViewContent.aspx?st=1004") %>',
                return "content_css: '" + ResolveUrl("~/ViewContent.aspx?st=" + rt.StyleSettings.StyleTemplateId) + "',";
            }
        }

        /// <summary>
        /// Initialize the control
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="isPagePostBack">if set to <c>true</c> [is page post back].</param>
        /// <param name="currentLanguage">The current language.</param>
        /// <param name="templateId">The template identifier.</param>
        /// <param name="pagePosition">The page position.</param>
        /// <param name="editMode">The edit mode.</param>
        /// <param name="profileFieldType">Type of the profile field.</param>
        public void Initialize(string text, bool isPagePostBack, string currentLanguage, int templateId, int? pagePosition, EditMode editMode, CustomFieldType? profileFieldType = null)
        {
            if (!isPagePostBack)
            {
                Text = Utilities.CustomDecode(text);
                CurrentEditMode = ApplicationManager.AppSettings.UseHTMLEditor ? ApplicationManager.AppSettings.DefaultQuestionEditorView : "Textarea";
            }

            LanguageCode = currentLanguage;
            PageIsPostback = isPagePostBack;

            TemplateId = templateId;
            PagePosition = pagePosition;
            EditMode = editMode;
            ProfileFieldType = profileFieldType;

            //Initialize pipeSelector
            switch (EditMode)
            {
                case EditMode.Survey:
                    _pipeSelector.Initialize(TemplateId, PagePosition,LanguageCode, _questionText.ClientID, ProfileFieldType);
                    break;
                case EditMode.Library:
                    _pipeSelector.Initialize(null, null, LanguageCode, _questionText.ClientID);
                    break;
                case EditMode.Report:
                    _pipeSelector.Visible = false;
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            _pipeSelector.ID = ID + "_" + _pipeSelector.ID;
            base.OnInit(e);
        }

        /// <summary>
        /// Set initial tab onload
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Page == null)
            {
                return;
            }
            
            //Set initial tab
            RegisterClientScriptInclude(
                "jquery.ckbxtab.js",
                ResolveUrl("~/Resources/jquery.ckbxtab.js"));
        }
    }
}