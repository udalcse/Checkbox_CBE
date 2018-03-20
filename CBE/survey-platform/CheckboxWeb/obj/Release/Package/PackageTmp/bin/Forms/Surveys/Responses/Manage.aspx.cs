using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.UI.WebControls;
using Checkbox.Analytics;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items;
using Checkbox.Management;
using Checkbox.Timeline;
using Checkbox.Web;
using Checkbox.Web.Page;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CheckboxWeb.Forms.Surveys.Responses
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Manage : ResponseTemplatePage
    {
        /// <summary>
        /// 
        /// </summary>
        [QueryParameter("ret")]
        public string ReturnPage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [QueryParameter("term")]
        public string SearchTerm { get; set; }

        [QueryParameter("r")]
        public long? ResponseId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected override string PageSpecificTitle
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// Require form edit permission to view this page
        /// </summary>
        protected override string PageRequiredRolePermission { get { return "Analysis.Responses.View"; } }

        /// <summary>
        /// Require view responses permission for the page
        /// </summary>
        protected override string ControllableEntityRequiredPermission
        {
            get { return "Analysis.Responses.View"; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _responseList.SurveyId = ResponseTemplateId;
            _responseList.TimelinePeriod = TimelinePeriod;

            if (ResponseTemplate != null)
                Master.SetTitle(WebTextManager.GetText("/pageText/forms/surveys/responses/manage.aspx/title") + " - " + Utilities.StripHtml(ResponseTemplate.Name, 64));

            //Set return url
            var returnUrl = ResolveUrl("~/Forms/Manage.aspx?s=" + ResponseTemplateId);

            if ("search".Equals(ReturnPage, StringComparison.InvariantCultureIgnoreCase))
            {
                returnUrl = ResolveUrl("~/Search.aspx") + "?term=" + Server.UrlEncode(SearchTerm);
            }

            Master.ShowBackButton(returnUrl, true);
		}

        /// <summary>
        /// Initialize controls
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            //fix for the case when the list of responses is the admin timeline result
            if (ResponseTemplateId == -1)
            {
                _manageButtons.Visible = false;
            }
            if (IsPostBack)
            {
                string[] targets = Request.Params.Get("__EVENTTARGET").Split(',');
                string[] args = Request.Params.Get("__EVENTARGUMENT").Split(',');

                for (int i = 0; i < targets.Length; i++)
                {
                    long answerId;
                    if (targets[i] == "downloadFile" && i < args.Length && long.TryParse(args[i], out answerId))
                    {
                        DownloadFile(answerId);
                    }
                }
            }

            Master.LeftPanelWidth = Unit.Pixel(570);

            LoadDatePickerLocalized();

            //Helper for uframe
            RegisterClientScriptInclude(
                "htmlparser.js",
                ResolveUrl("~/Resources/htmlparser.js"));

            //Helper for uframe
            RegisterClientScriptInclude(
                "UFrame.js",
                ResolveUrl("~/Resources/UFrame.js"));

            RegisterClientScriptInclude(
                 "svcSurveyEditor.js",
                 ResolveUrl("~/Services/js/svcSurveyEditor.js"));

            if (ResponseId.HasValue && !IsPostBack)
            {
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "showResponse", 
                    @"$(function(){ 
                        $(document).on('gridIsInitialized', function(){
                            $('#response_" + ResponseId + @"').trigger('click');
                        });
                    });", true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="answerId"></param>
        private void DownloadFile(long answerId)
        {
            UploadItem item = UploadItemManager.GetFileByAnswerID(answerId);
            if (item.FileName != null && item.Data != null)
            {
                Response.Expires = -1;
                Response.BufferOutput = true;
                Response.Clear();
                Response.ClearHeaders();
                Response.AddHeader("Content-Disposition", "attachment;filename=" + "\"" + HttpUtility.HtmlEncode(item.FileName) + "\"");
                Response.ContentType = "application/octet-stream";
                Response.ContentEncoding = System.Text.Encoding.UTF8;

                Response.BinaryWrite(item.Data);
                Response.Flush();
                Response.End();
            }
        }

        [WebMethod]
        public static string SaveUserIds(string userIds)
        {
            var newKey = Guid.NewGuid();
            var items = JsonConvert.DeserializeObject<Dictionary<Guid, string>>(userIds);

            HttpContext.Current.Session.Add(newKey.ToString(), items);

            return newKey.ToString();
        }
    }
}