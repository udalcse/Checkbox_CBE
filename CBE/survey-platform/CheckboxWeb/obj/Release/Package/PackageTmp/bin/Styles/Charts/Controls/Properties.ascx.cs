using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Checkbox.Management;
using Checkbox.Styles;
using Checkbox.Users;
using Checkbox.Web;

namespace CheckboxWeb.Styles.Charts.Controls
{
    public partial class Properties : Checkbox.Web.Common.UserControlBase
    {
        public LightweightStyleTemplate ChartStyle { get; set; }
        public string StyleName { get { return _styleNameTxt.Text; } }
        public bool IsEditable { get { return _allowEditChk.Checked; } }
        public bool IsPublic { get { return _publicChk.Checked; } }

        /// <summary>
        /// Initialize control
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            var stringId = Request.QueryString["s"];

            if (!string.IsNullOrWhiteSpace(stringId))
            {
                ChartStyle = ChartStyleManager.GetChartStyle(int.Parse(stringId));

                _styleNameTxt.Text = ChartStyle.Name;
                _allowEditChk.Checked = ChartStyle.IsEditable;
                _publicChk.Checked = ChartStyle.IsPublic;
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

            if (value.Equals(ChartStyle.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                args.IsValid = true;
                return;
            }

            args.IsValid = !StyleTemplateManager.IsStyleNameInUse(value);
        }
    }
}