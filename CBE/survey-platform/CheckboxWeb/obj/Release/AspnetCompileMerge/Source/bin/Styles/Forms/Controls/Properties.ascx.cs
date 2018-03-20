using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Checkbox.Common;
using Checkbox.Management;
using Checkbox.Styles;
using Checkbox.Users;
using Checkbox.Web;
using Prezza.Framework.Security.Principal;

namespace CheckboxWeb.Styles.Forms.Controls
{
    public partial class Properties : Checkbox.Web.Common.UserControlBase
    {
        public LightweightStyleTemplate StyleTemplate { get; set; }
        public string StyleName { get { return _styleNameTxt.Text; } }
        public int BaseStyleID { get { return int.Parse(_baseStyle.SelectedValue); } }
        public bool IsEditable { get { return _allowEditChk.Checked; } }
        public bool IsPublic { get { return _publicChk.Checked; } }

        /// <summary>
        /// Style ID
        /// </summary>
        protected string StyleID
        {
            get
            {
                return Request.QueryString["s"];
            }
        }

        /// <summary>
        /// Initialize control
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (!IsPostBack)
            {
                fillBaseStyles();
            }


            if (!string.IsNullOrWhiteSpace(StyleID))
            {
                StyleTemplate = StyleTemplateManager.GetLightweightStyleTemplate(int.Parse(StyleID), UserManager.GetCurrentPrincipal());

                _styleNameTxt.Text = Utilities.AdvancedHtmlDecode(StyleTemplate.Name);
                _styleNameTxt.ReadOnly = !StyleTemplate.CanBeEdited;
                _allowEditChk.Checked = StyleTemplate.IsEditable;
                _allowEditChk.Enabled = StyleTemplate.CanBeEdited;
                _publicChk.Checked = StyleTemplate.IsPublic;
                _publicChk.Enabled = StyleTemplate.CanBeEdited;
            }
            else
            {
                _styleNameTxt.Text = "";
                _allowEditChk.Checked = true;
                _publicChk.Checked = true;

            }

            //Validation 
            _nameRequiredValidator.Text = WebTextManager.GetText("/controlText/styles/nameRequired");
            _nameInUseValidator.Text = WebTextManager.GetText("/controlText/styles/nameInUse");

            _nameInUseValidator.ServerValidate += NameInUseValidatorServerValidate;
        }

        /// <summary>
        /// Adds items to the Base Style drop down
        /// </summary>
        void fillBaseStyles()
        {
            _baseStyle.Items.Clear();
            List<LightweightStyleTemplate> Styles = StyleTemplateManager.ListStyleTemplates((ExtendedPrincipal)UserManager.GetCurrentPrincipal(), StyleTemplateType.PC);
            Styles.Insert(0, new LightweightStyleTemplate(){TemplateId = 0, Name = WebTextManager.GetText("/controlText/styles/baseStyle/na")});
            _baseStyle.DataSource = Styles;
            _baseStyle.DataBind();
        }

        /// <summary>
        /// Validation of style name
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        void NameInUseValidatorServerValidate(object source, ServerValidateEventArgs args)
        {
            var value = (args.Value ?? string.Empty).Trim();

            if (!ApplicationManager.AppSettings.AllowHTMLNames)
            {
                value = Server.HtmlEncode(value);
            }

            if (StyleTemplate != null && value.Equals(StyleTemplate.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                args.IsValid = true;
                return;
            }

            args.IsValid = !StyleTemplateManager.IsStyleNameInUse(value);
        } 
    }
}