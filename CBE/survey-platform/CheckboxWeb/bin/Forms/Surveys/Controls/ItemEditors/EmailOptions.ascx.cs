using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Styles;
using Checkbox.Users;
using System.Linq;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    /// <summary>
    /// User control for editing email item options
    /// </summary>
    public partial class EmailOptions : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// Event fired when email message format is changed.
        /// </summary>
        public event EventHandler MessageFormatChanged;

        /// <summary>
        /// Get selected message format.
        /// </summary>
        public string SelectedMessageFormat { get { return _formatList.SelectedValue; } }

        /// <summary>
        /// 
        /// </summary>
        protected int? PagePosition { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected string LanguageCode { get; set; }

        /// <summary>
        /// Template ID
        /// </summary>
        int _templateID = 0;

        /// <summary>
        /// Populate list of available styles on init
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _formatList.SelectedIndexChanged += _formatList_SelectedIndexChanged;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {            
            base.OnLoad(e);
            if (_templateID > 0)
            {
                _pipeSelector_bccTxt.Initialize(_templateID, PagePosition, LanguageCode, _bccTxt.ClientID);
                _pipeSelector_fromTxt.Initialize(_templateID, PagePosition, LanguageCode, _fromTxt.ClientID);
                _pipeSelector_subjectTxt.Initialize(_templateID, PagePosition, LanguageCode, _subjectTxt.ClientID);
                _pipeSelector_toTxt.Initialize(_templateID, PagePosition, LanguageCode, _toTxt.ClientID);
            }
        }

        /// <summary>
        /// Handle email format change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _formatList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(MessageFormatChanged != null)
            {
                MessageFormatChanged(this, new EventArgs());
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="decorator"></param>
        /// <param name="templateId"></param>
        /// <param name="pagePosition"></param>
        /// <param name="languageCode"></param>
        public void Initialize(EmailItemTextDecorator decorator, int templateId, int? pagePosition, string languageCode)
        {
            PagePosition = pagePosition;
            LanguageCode = languageCode;

            if (_formatList.Items.FindByValue(decorator.Data.MessageFormat) != null)
            {
                _formatList.SelectedValue = decorator.Data.MessageFormat;
            }

            var styleTemplates = StyleTemplateManager.ListStyleTemplates(UserManager.GetCurrentPrincipal());
            //Sort by stripped name
            styleTemplates.OrderBy(st => Utilities.StripHtml(st.Name, 64));

            _styleList.Items.Add(new ListItem(string.Empty, "NONE"));

            if (!styleTemplates.Any())
            {
                _stylePanel.Visible = false;
            }
            else
            {
                foreach (var template in styleTemplates)
                {
                    _styleList.Items.Add(new ListItem(
                                             Utilities.StripHtml(template.Name, 64),
                                             template.TemplateId.ToString()));
                }

                if (decorator.Data.StyleTemplateID.HasValue)
                {
                    if (_styleList.Items.FindByValue(decorator.Data.StyleTemplateID.ToString()) != null)
                    {
                        _styleList.SelectedValue = decorator.Data.StyleTemplateID.ToString();
                    }
                }
            }

            _sendOnceChk.Checked = decorator.Data.SendOnce;
            _fromTxt.Text = decorator.Data.From;
            _toTxt.Text = decorator.Data.To;
            _bccTxt.Text = decorator.Data.BCC;
            _subjectTxt.Text = decorator.Subject;

            _templateID = templateId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="decorator"></param>
        public void UpdateData(EmailItemTextDecorator decorator)
        {
            decorator.Data.StyleTemplateID = Utilities.AsInt(_styleList.SelectedValue);
            decorator.Data.MessageFormat = _formatList.SelectedValue;
            decorator.Data.SendOnce = _sendOnceChk.Checked;
            decorator.Data.From = _fromTxt.Text.Trim();
            decorator.Data.To = _toTxt.Text.Trim();
            decorator.Data.BCC = _bccTxt.Text.Trim();
            decorator.Subject = _subjectTxt.Text.Trim();
        }
    }
}