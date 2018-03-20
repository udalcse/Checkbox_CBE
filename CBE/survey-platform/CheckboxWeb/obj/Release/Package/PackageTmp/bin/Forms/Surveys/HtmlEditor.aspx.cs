using System;
using System.Collections.Generic;
using System.Linq;
using Checkbox.Common;
using Checkbox.Web;
using Checkbox.Web.Page;

namespace CheckboxWeb.Forms.Surveys
{
    public partial class HtmlEditor : SecuredPage
    {
        [QueryParameter("s")]
        public int ResponseTemplateId { get; set; }

        [QueryParameter("p")]
        public int? PagePosition { get; set; }

        [QueryParameter("l")]
        public string LanguageCode { get; set; }

        [QueryParameter("html")]
        public string Html { get; set; }

        [QueryParameter("row")]
        public int RowNumber { get; set; }

        [QueryParameter("callback")]
        public string OnCloseCallback { get; set; }

        [QueryParameter("i")]
        public int? ItemId { get; set; }

        [QueryParameter("isNew")]
        public bool IsNew { get; set; }

        [QueryParameter("isBehavior")]
        public bool IsBehavior { get; set; }

        [QueryParameter("isMatrix")]
        public bool IsMatrix { get; set; }

        [QueryParameter("lid")]
        public int? LibraryTemplateId { get; set; }

        [QueryParameter("optionType")]
        public string OptionType { get; set; }

        private const string WRAPPER = "<div class=\"html-wrapper\">";

        /// <summary>
        /// Check arguments
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPageInit()
        {
            base.OnPageInit();            

            Master.SetTitle(WebTextManager.GetText("/pageText/matrixItemEditor.aspx/editHtml"));
            
            Master.OkEnable = true;
            Master.OkTextId = "/pageText/matrixItemEditor.aspx/submit";
            Master.OkClick += (sender, args) =>
                                  {
                                      if (IsNew || IsMatrix)
                                          ReturnToEditItemPage(false);
                                      else
                                          Master.CloseDialog(OnCloseCallback, new Dictionary<string, string>
                                                   {
                                                       { "html", PrepareHtml(true) },
                                                       { "rowNumber", RowNumber.ToString() }
                                                   });
                                  };

            Master.CancelEnable = true;
            Master.CancelTextId = "/pageText/matrixItemEditor.aspx/cancel";

            if (IsNew)
                Master.CancelClick += (sender, args) => ReturnToEditItemPage(true);

            _htmlTextbox.Text = HandleHtml(Html);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Page == null)
                return;

            _pipeSelector.Initialize(ResponseTemplateId, PagePosition, LanguageCode, _htmlTextbox.ClientID);

            //Helper for loading pages into divs
            RegisterClientScriptInclude(
                "UFrame.js",
                ResolveUrl("~/Resources/UFrame.js"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string HandleHtml(string html)
        {
            if (string.IsNullOrEmpty(html))
                return string.Empty;

            html = Utilities.AdvancedHtmlDecode(html);

            if (html.Contains(WRAPPER))
            {
                html = html.Replace(WRAPPER, string.Empty);
                int closingDivIndex = html.LastIndexOf("</div>");
                html = html.Remove(closingDivIndex);
            }

            return html;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string PrepareHtml(bool encode)
        {
            string html = Utilities.AdvancedHtmlDecode(_htmlTextbox.Text);
      //      string html = _htmlTextbox.Text;
            if (html != null && html.IndexOf("html-wrapper") != WRAPPER.IndexOf("html-wrapper"))
            {
                html = string.Format("<div class=\"html-wrapper\">{0}</div>", Utilities.RemoveScript(html));
            }
            html = Utilities.EncodeTagsInHtmlContent(html);
            var array = html.Split(new[] { "\r\n" }, StringSplitOptions.None);
            html = array.Aggregate(string.Empty, (current, s) => current + s);
            return encode ? Utilities.AdvancedHtmlEncode(html) : html;
        }

        /// <summary>
        /// Return to edit item page. This method is used when a column is added just after item creation.
        /// </summary>
        private void ReturnToEditItemPage(bool cancel)
        {
            string url;
            if (LibraryTemplateId.HasValue)
                url = IsMatrix ? ResolveUrl("~/Libraries/EditMatrixColumn.aspx") : ResolveUrl("~/Libraries/EditItem.aspx");    
            else
                url = IsMatrix ? "EditMatrixColumn.aspx" : "EditItem.aspx";
            
            url = url + "?";
            
            foreach (string key in Request.QueryString.AllKeys)
            {
                if (key != "showRowsTab" && key != "html" && key != "added"
                    && key != "showBehaviorTab" && key != "fromHtmlRedactor")
                    url += key + "=" + Request.QueryString[key] + "&";
            }

            if (IsBehavior)
                url += "showBehaviorTab=true&fromHtmlRedactor=true";
            else
                url += "showRowsTab=true&fromHtmlRedactor=true";
            
            if (cancel)
                url += "&cancel=true";
            else
                StoreHtmlIntoSession();

            Response.Redirect(url);
        }

        private int? ColumnNumber
        {
            get { return Session["matrixColumn_r=" + (ResponseTemplateId > 0 ? ResponseTemplateId : LibraryTemplateId) + "_i=" + ItemId] as int?; }
        }

        //
        private void StoreHtmlIntoSession()
        {
            if(ItemId.HasValue)
            {
                if (IsBehavior)
                {
                    if (OptionType.ToLower() == "noneofabove")
                        Session["temporary_noneOfAbove_" + ItemId.Value + "_c=" + ColumnNumber] = PrepareHtml(false);
                    else
                        Session["temporary_otherOption_" + ItemId.Value + "_c=" + ColumnNumber] = PrepareHtml(false);
                }
                else
                    Session["temporary_html_" + ItemId.Value + "_c=" + ColumnNumber] = PrepareHtml(false);
            }
        }
    }
}