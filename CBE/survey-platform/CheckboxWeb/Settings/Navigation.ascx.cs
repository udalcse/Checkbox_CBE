using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using Checkbox.Common;
using Checkbox.Web;

namespace CheckboxWeb.Settings
{
    public partial class Navigation : UserControl
    {
        private string _page;
        private List<string> _surveyPages = null;
        private List<string> _reportPages = null;
        private List<string> _userPages = null;
        private List<string> _systemPages = null;
        private List<string> _textPages = null;

        public List<string> SurveyPages
        {
            get
            {
                if (_surveyPages == null)
                {
                    _surveyPages= new List<string>
                                      {
                                          FormatMenuItem("SurveyPreferences", "survey", false),
                                          FormatMenuItem("UploadItem", "survey", true)
                                      };
                }

                return _surveyPages;
            }
        }

        public List<string> ReportPages
        {
            get
            {
                if (_reportPages == null)
                {
                    _reportPages = new List<string>
                                      {
                                          FormatMenuItem("ReportPreferences", "report", false),
                                          FormatMenuItem("ResponseDetails", "report", false),
                                          FormatMenuItem("ResponseExport", "report", true)
                                      };
                }

                return _reportPages;
            }
        }

        public List<string> UserPages
        {
            get
            {
                if (_userPages == null)
                {
                    _userPages = new List<string>
                                     {
                                         FormatMenuItem("Users", "user", false),
                                         FormatMenuItem("ExternalUsers", "user", false),
                                         FormatMenuItem("CustomUserFields", "user", true)
                                     };
                }

                return _userPages;
            }
        }

        public List<string> SystemPages
        {
            get
            {
                if (_systemPages == null)
                {
                    _systemPages = new List<string>
                                      {
                                          FormatMenuItem("Branding", "system", false),
                                          FormatMenuItem("Email", "system", false),
                                          FormatMenuItem("Licensing", "system", false),
                                          FormatMenuItem("Performance", "system", false),
                                          FormatMenuItem("Security", "system", false),
                                          FormatMenuItem("SystemPreferences", "system", true)
                                      };
                }

                return _systemPages;
            }
        }

        public List<string> TextPages
        {
            get
            {
                if (_textPages == null)
                {
                    _textPages = new List<string>
                                      {
                                          FormatMenuItem("Languages", "text", false),
                                          FormatMenuItem("LanguageNames", "text", false),
                                          FormatMenuItem("WelcomeText", "text", false),
                                          FormatMenuItem("SurveyText", "text", false),
                                          FormatMenuItem("ValidationText", "text", false),
                                          FormatMenuItem("ImportText", "text", true)
                                      };
                }

                return _textPages;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string section = Page.Request.QueryString.Get("section");
            if (section != null)
            {
                switch (section)
                {
                    case "survey":
                        _items.DataSource = SurveyPages;
                        break;

                    case "report":
                        _items.DataSource = ReportPages;
                        break;

                    case "user":
                        _items.DataSource = UserPages;
                        break;

                    case "system":
                        _items.DataSource = SystemPages;
                        break;

                    case "text":
                        _items.DataSource = TextPages;
                        break;

                    default:
                        _menuPanel.Visible = false;
                        //                            _backPanel.Visible = false;
                        break;
                }

                _items.DataBind();
            }
            else
            {
                _menuPanel.Visible = false;
                //                    _backPanel.Visible = false;
            }
        }

        protected string FormatMenuItem(string key, string section, bool isLast)
        {
            if (Utilities.IsNullOrEmpty(_page))
                _page = Page.Request.Url.Segments[Page.Request.Url.Segments.Length - 1];

            bool isSelected = (_page.StartsWith(key, StringComparison.InvariantCultureIgnoreCase));

            string itemText = WebTextManager.GetText(string.Format("/pageText/settings/navigation.ascx/{0}", key));
            StringBuilder menuItem = new StringBuilder();

            if (isLast && isSelected)
                menuItem.Append("<li class=\"selected last\">");
            else if (isLast)
                menuItem.Append("<li class=\"last\">");
            else if (isSelected)
                menuItem.Append("<li class=\"selected\">");
            else
                menuItem.Append("<li>");

            menuItem.AppendFormat("<a href=\"./{0}.aspx?section={1}\">{2}</a>", key, section, itemText);
            menuItem.Append("</li>");

            return menuItem.ToString();
        }
    }
}
