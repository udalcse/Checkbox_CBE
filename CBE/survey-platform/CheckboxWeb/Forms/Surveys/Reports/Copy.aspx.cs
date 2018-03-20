using System;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.Analytics;
using Checkbox.Security.Principal;
using Checkbox.Globalization.Text;
using Prezza.Framework.Security;

namespace CheckboxWeb.Forms.Surveys.Reports
{
    /// <summary>
    /// Move/Copy report
    /// </summary>
    public partial class Copy : SecuredPage
    {
        [QueryParameter("r", IsRequired = true)]
        public int? ReportId { get; set; }

        /// <summary>
        /// Bind events
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            if (!IsPostBack)
            {
                AnalysisTemplate at = AnalysisTemplateManager.GetAnalysisTemplate(ReportId.Value);

                Master.SetTitle(string.Format(WebTextManager.GetText("/pageText/forms/surveys/reports/copy.aspx/pagetitle", null, "Copy Report {0}"), Server.HtmlDecode(at.Name)));

                _nameText.Text = Server.HtmlDecode(AnalysisTemplateManager.GetUniqueName(at.ResponseTemplateID, at.Name, TextManager.DefaultLanguage));
                _nameText.Focus();
            }

            Master.OkClick += _okButton_Click;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override string PageRequiredRolePermission
        {
            get { return "Analysis.Create"; }
        }
        
        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();
        }

        /// <summary>
        /// Handle ok button click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _okButton_Click(object sender, System.EventArgs e)
        {
            AnalysisTemplate at = AnalysisTemplateManager.GetAnalysisTemplate(ReportId.Value);
            AnalysisTemplateManager.CopyTemplate(ReportId.Value, Server.HtmlEncode(_nameText.Text), User as CheckboxPrincipal, TextManager.DefaultLanguage);
            Dictionary<String, String> args = new Dictionary<string, string>();
            args.Add("op", "refresh");

            Master.CloseDialog(args);
        }
    }
}
