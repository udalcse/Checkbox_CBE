using System.Linq;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Globalization;
using Checkbox.Management;
using Checkbox.Web;
using Checkbox.Web.Page;
using System.Xml;
using Checkbox.Timeline;

namespace CheckboxWeb.Forms
{
    /// <summary>
    /// Manage surveys page
    /// </summary>
	public partial class Manage : SecuredPage
	{
        /// <summary>
        /// 
        /// </summary>
        [QueryParameter("s")]
        public int? CurrentSurveyId { get; set; }

		/// <summary>
		/// Require form edit permission to view this page
		/// </summary>
		protected override string PageRequiredRolePermission { get { return "Form.Edit,Analysis.Administer"; } }

        /// <summary>
        /// 
        /// </summary>
		protected override void OnPageInit()
		{
			base.OnPageInit();
			Master.SetTitle(WebTextManager.GetText("/pageText/forms/manage.aspx/surveyManage"));
		}

        protected bool ShowCreateSurveyDialogOnLoad { set; get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            if (!ApplicationManager.AppSettings.EmailEnabled)
            {
                var visibleEvents = TimelineManager.GetVisibleEvents("SurveyManager");
                _timeline.VisibleEvents = string.Empty;
                foreach (var visibleEvent in visibleEvents.Where(visibleEvent => !visibleEvent.Contains("INVITATION")))
                {
                    _timeline.VisibleEvents += string.Format("{0},", visibleEvent);
                }
            }

            if (ApplicationManager.AppSettings.EnableMultiDatabase &&
                CurrentPrincipal != null &&
                CurrentPrincipal.IsInRole("System Administrator"))
            {
                _productTour.Source = "~/Resources/feature-tour.js";
            }

            if (IsPostBack)
            {
                string target = Request.Params.Get("__EVENTTARGET");
                string arg = Request.Params.Get("__EVENTARGUMENT");

                if (target == "export")
                {
                    int id;
                    if (!int.TryParse(arg, out id))
                        return;

                    ResponseTemplate template = ResponseTemplateManager.GetResponseTemplate(id);
                    if (template != null)
                    {
                        string name = Utilities.AdvancedHtmlDecode(template.Name);
                        name = FileUtilities.SanitizeFileName(name, "");

                        GetDownloadResponse(name + "_export.xml");

                        var writer = new XmlTextWriter(Response.Output) { Formatting = Formatting.Indented };

                        template.Export(writer);

                        Response.Flush();
                        Response.End();                        
                    }

                    return;
                }
            }
            else
            {
                if (CurrentSurveyId.HasValue)
                {
                    var parentFolder = ResponseTemplateManager.GetResponseTemplateParentFolderId(CurrentSurveyId.Value);

                    Page.ClientScript.RegisterStartupScript(
                            GetType(),
                            "selectSurvey",
                            string.Format("showOnLoad({0},{1});", parentFolder ?? -1, CurrentSurveyId),
                            true);
                }
            }

            ShowCreateSurveyDialogOnLoad = ApplicationManager.AppSettings.EnableMultiDatabase &&
                                           ApplicationManager.IsDataContextTrial &&
                                           CurrentPrincipal != null &&
                                           CurrentPrincipal.IsInRole("System Administrator");

            LoadDatePickerLocalized();
        }

        /// <summary>
		/// 
		/// </summary>
		/// <param name="fileName"></param>
		protected void GetDownloadResponse(string fileName)
		{
			Response.Expires = -1;
			Response.BufferOutput = Checkbox.Management.ApplicationManager.AppSettings.BufferResponseExport;
			Response.Clear();
			Response.ClearHeaders();
			Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName));
			Response.ContentType = "application/octet-stream";
		}
	}
}
