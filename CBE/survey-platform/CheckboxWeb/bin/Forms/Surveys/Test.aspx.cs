using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Globalization.Text;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Forms;
using Checkbox.Web.Page;

namespace CheckboxWeb.Forms.Surveys
{
    /// <summary>
    /// Test survey page.  Shows interface for configuring hidden items before taking survey.
    /// </summary>
    public partial class Test : ResponseTemplatePage
    {
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<string, HiddenItemData> HiddenItems
        {
            get
            {
                var res = Session["HiddenItems"] as Dictionary<string, HiddenItemData>;
                if (res == null)
                {
                    res = new Dictionary<string, HiddenItemData>();
                    Session["HiddenItems"] = res;
                }
                return res;
            }
        }

        /// <summary>
        /// Title for page
        /// </summary>
        protected override string PageSpecificTitle
        {
            get { return WebTextManager.GetText("/pageText/forms/surveys/edit.aspx/testSurvey"); }
        }

        /// <summary>
        /// Override page init to redirect to survey page.
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            Master.OkVisible = false;
            Master.CancelTextId = "/pageText/forms/surveys/test.aspx/close";
            Master.SetTitle(PageSpecificTitle);
            Master.CancelButtonClass = Master.CancelButtonClass.Replace("left", string.Empty);

            _surveyUrls.ResponseTemplateId = ResponseTemplateId;
            _surveyUrls.IsTestResponse = true;

            //Add user guid, if any
            var currentPrincipal = UserManager.GetCurrentPrincipal();

            if (currentPrincipal.UserGuid != Guid.Empty)
            {
                Session[FormQueryParameters.TEST_SESSION_EXTERNAL_UID_KEY] = string.Empty;
                _surveyUrls.AdditionalURLParams = "u=" + currentPrincipal.UserGuid + "&forceNew=true";
            }
            else
            {
                Session[FormQueryParameters.TEST_SESSION_EXTERNAL_UID_KEY] = currentPrincipal.Identity.Name;
            }

            LoadHiddenItemInputs();
//            Response.Redirect(ApplicationManager.ApplicationPath + "/Survey.aspx?s=" + ResponseTemplate.GUID + "&Test=true", false);
        }

        /// <summary>
        /// Add a hidden item input
        /// </summary>
        private void AddHiddenItemInput(string prefix, HiddenItemData item)
        {
            //Get item type
            string typeText;

            if (item.VariableSource == Checkbox.Forms.Items.HiddenVariableSource.Cookie)
            {
                typeText = WebTextManager.GetText("/enum/hiddenVariableSource/cookie");

                if (typeText == null || typeText.Trim() == string.Empty)
                {
                    typeText = "Cookie";
                }
            }
            else if (item.VariableSource == Checkbox.Forms.Items.HiddenVariableSource.QueryString)
            {
                typeText = WebTextManager.GetText("/enum/hiddenVariableSource/queryString");

                if (typeText == null || typeText.Trim() == string.Empty)
                {
                    typeText = "Query String";
                }
            }
            else
            {
                typeText = WebTextManager.GetText("/enum/hiddenVariableSource/session");

                if (typeText == null || typeText.Trim() == string.Empty)
                {
                    typeText = "Session";
                }
            }

            //Get the text
            //LabelledItemTextDecorator decorator = (LabelledItemTextDecorator)item.CreateTextDecorator(ResponseTemplate.LanguageSettings.DefaultLanguage);

            string questionText = Checkbox.Common.Utilities.StripHtml(prefix + ")  " + item.VariableName, 64);

            //Create an input
            var input = new TextBox {Width = Unit.Pixel(300), CssClass = "PrezzaNormal"};
            input.Attributes["paramtype"] = typeText;
            input.Attributes["paramname"] = item.VariableName;
            input.Attributes["iid"] = prefix;

            HiddenItems[prefix] = item;


            _hiddenItemRowPlace.Controls.Add(new LiteralControl("<tr>"));
            _hiddenItemRowPlace.Controls.Add(new LiteralControl("<td class=\"PrezzaNormal\">" + questionText + "</td>"));
            _hiddenItemRowPlace.Controls.Add(new LiteralControl("<td class=\"PrezzaNormal\">"));
            _hiddenItemRowPlace.Controls.Add(input);
            _hiddenItemRowPlace.Controls.Add(new LiteralControl("</td>"));
            _hiddenItemRowPlace.Controls.Add(new LiteralControl("<td class=\"PrezzaNormal\">(" + typeText + ")</td>"));
            _hiddenItemRowPlace.Controls.Add(new LiteralControl("</tr>"));
        }


        /// <summary>
        /// Register a JS open script
        /// </summary>
        /// <param name="url"></param>
        /// <param name="name"></param>
        private void RegisterOpenScript(string url, string name)
        {
            if (!ClientScript.IsStartupScriptRegistered(name))
            {
                ClientScript.RegisterStartupScript(GetType(), name, "javascript:void window.open('" + url + "', '_adminTest', '');", true);
            }
        }
    
        /// <summary>
        /// Load inputs for hidden items
        /// </summary>
        private void LoadHiddenItemInputs()
        {
            if (ResponseTemplate == null)
                return;

            foreach (int itemID in ResponseTemplate.ListTemplateItemIds())
            {
                ItemData item = ResponseTemplate.GetItem(itemID);
                if (item is HiddenItemData && item.ID != null)
                {
                    _hiddenItemsPlace.Visible = true;

                    AddHiddenItemInput((ResponseTemplate.GetPagePositionForItem(item.ID.Value) - 1).ToString() + "." + ResponseTemplate.GetItemPositionWithinPage(item.ID.Value), item as HiddenItemData);
                }
            }

            //Add an input for survey language
            if (TextManager.MultiLanguageEnabled && ResponseTemplate.LanguageSettings.SupportedLanguages.Count > 0)
            {
                if (ResponseTemplate.LanguageSettings.LanguageSource != null && (ResponseTemplate.LanguageSettings.LanguageSource.ToLower() == "querystring" || ResponseTemplate.LanguageSettings.LanguageSource.ToLower() == "session"))
                {
                    _hiddenItemsPlace.Visible = true;

                    //Create an input
                    TextBox input = new TextBox();
                    input.CssClass = "PrezzaNormal";
                    input.ID = "languageSelect";

                    string questionText = WebTextManager.GetText("/pageText/adminTakeSurvey.aspx/languageSource");
                    string typeText;

                    if (ResponseTemplate.LanguageSettings.LanguageSource.ToLower() == "querystring")
                    {
                        typeText = WebTextManager.GetText("/enum/hiddenVariableSource/queryString");

                        if (typeText == null || typeText.Trim() == string.Empty)
                        {
                            typeText = "Query String";
                        }
                    }
                    else
                    {
                        typeText = WebTextManager.GetText("/enum/hiddenVariableSource/session");

                        if (typeText == null || typeText.Trim() == string.Empty)
                        {
                            typeText = "Session";
                        }
                    }


                    input.Attributes["paramtype"] = typeText;
                    input.Attributes["paramname"] = ResponseTemplate.LanguageSettings.LanguageSourceToken;

                    _hiddenItemRowPlace.Controls.Add(new LiteralControl("<tr>"));
                    _hiddenItemRowPlace.Controls.Add(new LiteralControl("<td class=\"PrezzaNormal\">" + questionText + "</td>"));
                    _hiddenItemRowPlace.Controls.Add(new LiteralControl("<td class=\"PrezzaNormal\">"));
                    _hiddenItemRowPlace.Controls.Add(input);
                    _hiddenItemRowPlace.Controls.Add(new LiteralControl("</td>"));
                    _hiddenItemRowPlace.Controls.Add(new LiteralControl("<td class=\"PrezzaNormal\">(" + typeText + ")</td>"));
                    _hiddenItemRowPlace.Controls.Add(new LiteralControl("</tr>"));
                }
            }            
        }

        [WebMethod(EnableSession=true)]
        public static void SaveParameter(string ticket, string type, string id, string value)
        {
            //System.Diagnostics.Trace.WriteLine(type + " " + value);
            Dictionary<string, HiddenItemData> items = HttpContext.Current.Session["HiddenItems"] as Dictionary<string, HiddenItemData>;
            if (items == null)
                return;
            HiddenItemData item = items[id];
            if (id == null)
                return;
            switch (type)
            {
                case "Cookie" :
                {
                    HttpContext.Current.Response.SetCookie(new HttpCookie(item.VariableName, value));
                }
                break;
                case "Session":
                {
                    HttpContext.Current.Session[item.VariableName] = value;
                }
                break;
            }
        }
    }
}
