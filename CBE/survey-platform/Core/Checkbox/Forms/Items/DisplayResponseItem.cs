using System;
using System.Linq;
using System.Xml;
using System.Text;
using System.Collections.Specialized;
using System.Collections.Generic;

using Checkbox.Common;
using Checkbox.Security;
using Checkbox.Management;
using Checkbox.Globalization.Text;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Forms;
using Checkbox.Forms.Items;
using Checkbox.Users;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Display a response in the survey or as a link to separate window
    /// </summary>
    [Serializable()]
    public class DisplayResponseItem : ResponseItem
    {
        private string _editorText;
        private string _linkText;
        private bool _displayInline;
        private bool _showHiddenItems;
        private bool _showPageNumbers;
        private bool _showQuestionNumbers;
        private bool _includeResponseDetails;
        private bool _includeMessageItems;
        private string _linkUrl;
        private ResponseTemplate _responseTemplate;

        /// <summary>
        /// Get survey associated with response
        /// </summary>
        protected ResponseTemplate ResponseTemplate
        {
            get { return _responseTemplate ?? (_responseTemplate = GetResponseTemplate()); }
        }

        /// <summary>
        /// Get a response template based on a responseGUID
        /// </summary>
        /// <returns></returns>
        private ResponseTemplate GetResponseTemplate()
        {
            return ResponseTemplateManager.GetResponseTemplateFromResponseGUID(Response.GUID.Value);
        }

        /// <summary>
        /// Configure the item with its configuration information
        /// </summary>
        /// <param name="configuration">Item configuration</param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData configuration, string languageCode, int? templateId)
        {
            Visible = ExportMode != ExportMode.Pdf;
            base.Configure(configuration, languageCode, templateId);

            _displayInline = ((DisplayResponseItemData)configuration).DisplayInlineResponse;
            _includeMessageItems = ((DisplayResponseItemData)configuration).IncludeMessageItems;
            _includeResponseDetails = ((DisplayResponseItemData)configuration).IncludeResponseDetails;
            _showHiddenItems = ((DisplayResponseItemData)configuration).ShowHiddenItems;
            _showPageNumbers = ((DisplayResponseItemData)configuration).ShowPageNumbers;
            _showQuestionNumbers = ((DisplayResponseItemData)configuration).ShowQuestionNumbers;

            _editorText = TextManager.GetText("/itemText/displayResponseItem/editorText", languageCode);
            _linkText = GetText(((DisplayResponseItemData)configuration).LinkTextID);

            if (_editorText == null || _editorText.Trim() == string.Empty)
            {
                _editorText = TextManager.GetText("/itemText/displayResponseItem/editorText", TextManager.DefaultLanguage);
            }
        }

        /// <summary>
        /// Get the text for the link
        /// </summary>
        /// <returns></returns>
        private string GetLinkText()
        {
            string res = GetPipedText("LinkText", _linkText);
            if (string.IsNullOrEmpty(res))
                res = "View Response";
            return res;
        }

        /// <summary>
        /// Get the link URL
        /// </summary>
        /// <returns></returns>
        private string GetLinkUrl()
        {
            if(Utilities.IsNullOrEmpty(_linkUrl))
            {
                if (Response == null)
                {
                    return "#";
                }
                
                StringBuilder sb = new StringBuilder();

                Ticketing.CreateTicket(Response.GUID.Value, DateTime.Now.AddMinutes(ApplicationManager.AppSettings.ViewReportTicketDuration));

                
                sb.Append(ApplicationManager.ApplicationRoot);
                //sb.Append("/Analytics/Data/ViewResponseDetails.aspx?responseGuid=");
                sb.Append("/Forms/Surveys/Responses/View.aspx?responseGuid=");
                sb.Append(Response.GUID);
                sb.Append("&amp;noChrome=true");

                _linkUrl = sb.ToString();
            }

            return _linkUrl;
        }

        /// <summary>
        /// Get whether to display an inline response
        /// </summary>
        public bool DisplayInlineResponse
        {
            get { return _displayInline; }
        }

        /// <summary>
        /// Get HTML to display 
        /// </summary>
        /// <returns></returns>
        public string GetResponseHtml()
        {
            StringBuilder sb = new StringBuilder();

            if (!_displayInline)
            {
                sb.Append("<br /><a href=\"");
                sb.Append(GetLinkUrl());
                sb.Append("\">");
                sb.Append(GetLinkText());
                sb.Append("</a>");
            }
            else
            {
                if (Response == null)
                {
                    sb.Append("<br /><div style=\"padding:5px;font-family:Verdana,Arial;font-size:10px;color:black;\">");
                    sb.Append(_editorText);
                    sb.Append("</div><br />");
                }
                else
                {
                    sb.Append(GetText("/itemText/displayResponseItem/surveyTitle"));
                    sb.Append(" ");
                    sb.Append(Response.Title);
                    sb.AppendLine("<br />");

                    if (!string.IsNullOrEmpty(Response.Description))
                    {
                        sb.Append(Response.Description);
                        sb.AppendLine("<br />");
                    }

                    if (_includeResponseDetails)
                    {
                        sb.Append(GetText("/itemText/displayResponseItem/responseGUID"));
                        sb.Append(" ");
                        sb.Append(Response.GUID.ToString());
                        sb.AppendLine("<br />");

                        sb.Append(GetText("/itemText/displayResponseItem/responseStarted"));
                        sb.Append(" ");
                        sb.Append(Response.DateCreated.ToString());
                        sb.AppendLine("<br />");

                        sb.Append(GetText("/itemText/displayResponseItem/responseCompleted"));
                        sb.Append(" ");
                        sb.Append(Response.LastModified.ToString());
                        sb.AppendLine("<br /><br />");
                    }

                    //reload response pages
                    foreach (ResponsePage p in Response.GetResponsePages())
                    {
                        Response.ReloadPageItems(p, ResponseTemplate);
                        p.OnLoad(false);
                    }
                    if(_showQuestionNumbers)
                        Response.GenerateItemNumbers();

                    foreach (ResponsePage p in Response.GetResponsePages())
                    {
                        if (p.PageType == TemplatePageType.HiddenItems && !_showHiddenItems)
                            continue;

                        var psb = new StringBuilder();

                        if (_showPageNumbers)
                            psb.AppendFormat("<div class=\"ResponseDetailsHeader\" style=\"margin-top:10px;\">{0}</div>", GetPageTitle(p));

                        var hasItems = false;
                        if (!p.Excluded)
                        {
                            foreach (Item i in p.GetItems())
                            {
                                var answerable = i as IAnswerable;
                                if (!i.Excluded && answerable != null && answerable.HasAnswer)
                                {
                                    string itemHtml = GetItemHtml(i);

                                    if (itemHtml == null)
                                        continue;

                                    string paddingTop = "padding-top:5px;";

                                    if (i.ItemTypeName.Equals("Slider", StringComparison.InvariantCultureIgnoreCase) ||
                                        i.ItemTypeName.Equals("RadioButtons", StringComparison.InvariantCultureIgnoreCase))
                                        paddingTop = string.Empty;

                                    string questionNumber = _showQuestionNumbers ? string.Format("<div style=\"padding-right:10px;\"><div class=\"Question\"><p>{0}</p></div></div>", GetItemNumber(i)) : string.Empty;
                                    string padding = string.Format("<div style=\"float:left;\"><div style=\"{0}\">{1}</div></div>", paddingTop, questionNumber);

                                    psb.AppendFormat("{0}<div style=\"float:left;\">{1}</div><div class=\"clear\"></div>", padding, itemHtml);

                                    hasItems = true;
                                }
                            }
                        }

                        if (hasItems)
                            sb.Append(psb);
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        protected string GetPageTitle(ResponsePage page)
        {
            if (page == null)
            {
                return string.Empty;
            }

            if (page.PageType == TemplatePageType.HiddenItems)
            {
                return TextManager.GetText("/pageText/viewResponseDetails.aspx/hiddenItems");
            }

            if (page.PageType == TemplatePageType.Completion)
            {
                return TextManager.GetText("/pageText/viewResponseDetails.aspx/completionEventsPage");
            }
             
            var pageScoreString = string.Empty;

            if (ResponseTemplate.BehaviorSettings.EnableScoring && page.PageType == TemplatePageType.ContentPage)
            {
                var pageScore = page.GetItems().Where(item => item is IScored).Sum(item => ((IScored)item).GetScore());

                pageScoreString = string.Format("&nbsp;({0})", pageScore);
            }

            //Subtract 1 from page position to account for hidden items at page 1
            return string.Format("{0} {1}{2}", TextManager.GetText("/pageText/viewResponseDetails.aspx/page"), page.Position - 1, pageScoreString);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected string GetItemNumber(Item item)
        {
            var answerableItem = item as IAnswerable;

            if (answerableItem == null || !answerableItem.HasAnswer)
            {
                return string.Empty;
            }
            
            var itemNumber = Response.GetItemNumber(item.ID);

            return itemNumber.HasValue ? itemNumber.ToString() : string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected string GetItemHtml(Item item)
        {
            if (item.ShouldRender && !_includeMessageItems)
                return null;

            var answerableItem = item as IAnswerable;

            if ((answerableItem == null || !answerableItem.HasAnswer) && !(item.ShouldRender) && PropertyBindingManager.IsBinded(item.ID))
            {
                return string.Empty;
            }

            //Add HTML for the item                                                
            IItemFormatter itemFormatter = GetItemFormatter(item.TypeID);

            if (itemFormatter == null)
            {
                return string.Empty;
            }

            return itemFormatter.Format(item, "html", Response.ScoringEnabled);
        }

        private Dictionary<int, IItemFormatter> _formatterDictionary;
        /// <summary>
        /// Get the temporary item formatter dictionary
        /// </summary>
        protected Dictionary<int, IItemFormatter> FormatterDictionary
        {
            get { return _formatterDictionary ?? (_formatterDictionary = new Dictionary<int, IItemFormatter>()); }
        }


        /// <summary>
        /// Get the html formatter for a given item
        /// </summary>
        /// <param name="itemTypeId"></param>
        protected IItemFormatter GetItemFormatter(int itemTypeId)
        {
            if (!FormatterDictionary.ContainsKey(itemTypeId))
            {
                IItemFormatter formatter = ItemFormatterFactory.GetItemFormatter(itemTypeId, "html");

                if (formatter == null)
                {
                    return null;
                }

                FormatterDictionary[itemTypeId] = formatter;
            }

            return FormatterDictionary[itemTypeId];
        }



        /// <summary>
        /// Get meta data for serialization
        /// </summary>
        /// <returns></returns>
        protected override NameValueCollection GetMetaDataValuesForSerialization()
        {
            NameValueCollection values = base.GetMetaDataValuesForSerialization();

            values["DisplayInForm"] = DisplayInlineResponse.ToString();

            return values;
        }

        /// <summary>
        /// Get instance-specific information
        /// </summary>
        /// <returns></returns>
        protected override NameValueCollection GetInstanceDataValuesForSerialization()
        {
            NameValueCollection values = base.GetInstanceDataValuesForSerialization();

            values["LinkText"] = GetLinkText();
            values["LinkUrl"] = GetLinkUrl();
            values["ResponseHtml"] = GetResponseHtml();

            return values;
        }
    }
}
