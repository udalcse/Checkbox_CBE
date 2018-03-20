using System.Collections.Generic;
using Checkbox.Globalization.Text;
using Checkbox.Web;
using Checkbox.Web.Page;
using Prezza.Framework.Caching;

namespace CheckboxWeb.Settings
{
    public partial class Default : TextSettings
    {
        /// <summary>
        /// 
        /// </summary>
        protected override string PageRequiredRole { get { return "System Administrator"; } }

        /// <summary>
        /// 
        /// </summary>
        public string CacheManagementPage
        {
            get
            {
                var cacheType = CacheFactory.CacheType;
                return cacheType.Replace("Prezza.Framework.Caching.", "") + ".aspx";
            }
        }

        protected override void OnInit(System.EventArgs e)
        {
            base.OnInit(e);

            //Helper for uframe
            RegisterClientScriptInclude(
                "htmlparser.js",
                ResolveUrl("~/Resources/htmlparser.js"));

            //Helper for loading pages into divs
            RegisterClientScriptInclude(
                "UFrame.js",
                ResolveUrl("~/Resources/UFrame.js"));

            RegisterClientScriptInclude(
                "jquery.fileDownload.js",
                ResolveUrl("~/Resources/jquery.fileDownload.js"));

            Master.SetTitle(WebTextManager.GetText("/pageMenu/settings/_home"));
        }

        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

			if (!IsPostBack)
			{
                string target = Request.Params.Get("target");
                string exportOptionTxt = Request.Params.Get("argument");

			    switch (target)
			    {
                    case "_textExportLink":
                        DoExport(exportOptionTxt);
                        break;
                    case "_exportMiscTextBtn":
                        ExportText(SurveyLanguages,
                           "/common/dropDownDefault",
                           "/pageText/survey.aspx/pageValidationPopup",
                           "/pageText/survey.aspx/",
                           "/pageText/takeSurvey.aspx/selectLanguage",
                           "/pageText/takeSurvey.aspx/continue",
                           "/controlText/responseView/");
			            break;
                    case "_exportSelfRegistrationMsgsBtn":
                    case "_exportValidationMsgsBtn":
                    case "_exportLanguageNamesBtn":
                        ExportText(SurveyLanguages, exportOptionTxt);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Do actual export work
        /// </summary>
        private void DoExport(string exportOptionTxt)
        {
            List<string> filters = new List<string>();

            if ("AllText".Equals(exportOptionTxt))
            {
                filters.Add("/");
            }
            else
            {
                filters.Add("/common/");
                filters.Add("/condition/");
                filters.Add("/pageBranch/");
                filters.Add("/controlText/");
                filters.Add("/enum/");
                filters.Add("/errorMessage/");
                filters.Add("/errorMessages/");
                filters.Add("/itemEditor/");
                filters.Add("/languages/");
                filters.Add("/pageText/");
                filters.Add("/responseProperty/");
                filters.Add("/validationMessages/");
                filters.Add("/selfRegistrationScreen/");
            }

            Response.Expires = -1;
            Response.Buffer = true;
            Response.Clear();
            Response.ClearHeaders();
            Response.AddHeader("Content-Disposition", "attachment;filename=CheckboxSurveyTextExport.xml");
            Response.ContentType = "text/xml";
            Response.ContentEncoding = System.Text.Encoding.UTF8;

            TextManager.ExportFilteredTexts(Response.Output, null, filters.ToArray());
            Response.Flush();
            Response.End();
        }

        protected override string PageTitleTextId
        {
            get { return string.Empty; }
        }
    }
}