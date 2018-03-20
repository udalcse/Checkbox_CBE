using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Xml;
using Checkbox.Web;
using Checkbox.Web.Page;
using Prezza.Framework.Configuration;
using Prezza.Framework.ExceptionHandling;
using Checkbox.Management;

namespace CheckboxWeb.Settings
{
    public partial class Languages : TextSettings
    {
        [QueryParameter("message")]
        public String Message { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected override string PageTitleTextId { get { return "/pageText/settings/languages.aspx/title"; } }

        /// <summary>
        /// Require administrator access
        /// </summary>
        protected override string PageRequiredRole { get { return "System Administrator"; } }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            Master.IsDialog = false;
            base.OnPageInit();

            Master.HideDialogButtons();

            _surveyLanguageRepeater.ItemCommand += LanguageRepeater_ItemCommand;
            _applicationLanguagesRepeater.ItemCommand += LanguageRepeater_ItemCommand;


            BindLanguageDropDown();
            BindRepeater(_surveyLanguageRepeater, SurveyLanguages);
            BindRepeater(_applicationLanguagesRepeater, ApplicationLanguages);

            _defaultLanguageList.SelectedIndexChanged += new EventHandler(_defaultLanguageList_SelectedIndexChanged);
            _deleteLanguage.Click += new EventHandler(_deleteLanguage_Click);
            
            if(!IsPostBack && !String.IsNullOrEmpty(Message))
            {
                Master.ShowStatusMessage(Message, StatusMessageType.Success);
            }
        }

        void _deleteLanguage_Click(object sender, EventArgs e)
        {
            DeleteSurveyLanguage(_languageToDelete.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _defaultLanguageList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Do nothing for now
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();
            if (ApplicationManager.AppSettings.EnableMultiDatabase)
            {
                Response.Redirect(UserDefaultRedirectUrl);
            }
        }

        /// <summary>
        /// Handle clicking of repeater buttons
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void LanguageRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (ApplicationManager.AppSettings.EnableMultiDatabase)
            {
                return;
            }

            try
            {
                switch (e.CommandName.ToLower())
                {
                    case "deletesurveylanguage":
                        DeleteSurveyLanguage(e.CommandArgument.ToString());
                        break;

                    case "deleteapplanguage":
                        DeleteAppLanguage(e.CommandArgument.ToString());
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "UIProcess");

                Master.ShowStatusMessage(WebTextManager.GetText("/errorMessages/common/errorOccurred"), StatusMessageType.Error);
            }
        }


        /// <summary>
        /// Delete the survey language
        /// </summary>
        /// <param name="languageToDelete"></param>
        private void DeleteSurveyLanguage(string languageToDelete)
        {
            if (SurveyLanguages.Count <= 1)
            {
                Master.ShowStatusMessage(WebTextManager.GetText("/pageText/settings/languages.aspx/unableToDeleteOnlySurveyLanguage"), StatusMessageType.Warning);
                return;
            }

            string configPath;
            XmlDocument config = GetGlobalizationConfig(out configPath);

            //Attempt to find the survey language node
            XmlNode surveyLanguageNode = config.SelectSingleNode("/globalizationConfiguration/surveyLanguages/language[. = '" + languageToDelete + "']");

            if (surveyLanguageNode != null)
            {
                surveyLanguageNode.ParentNode.RemoveChild(surveyLanguageNode);
                config.Save(configPath);

                //                _restartRequiredPnl.Visible = true;
                Application["RestartRequired"] = true;
            }

            SurveyLanguages.Remove(languageToDelete);

            //Rebind the survey language repeater
            BindRepeater(_surveyLanguageRepeater, SurveyLanguages);

            //Rebind language select lists
            BindLanguageDropDown();

            Checkbox.Globalization.Text.TextManager.RemoveSurveyLanguage(languageToDelete);

            Master.ShowStatusMessage(WebTextManager.GetText("/pageText/settings/languages.aspx/surveyLanguageRemoved"), StatusMessageType.Success);
        }

        /// <summary>
        /// Delete an application language
        /// </summary>
        /// <param name="languageToDelete"></param>
        private void DeleteAppLanguage(string languageToDelete)
        {
            if (ApplicationLanguages.Count <= 1)
            {
                Master.ShowStatusMessage(WebTextManager.GetText("/pageText/settings/languages.aspx/unableToDeleteOnlyAppLanguage"), StatusMessageType.Warning);
                return;
            }

            string configPath;
            XmlDocument config = GetGlobalizationConfig(out configPath);

            //Attempt to find the survey language node
            XmlNode surveyLanguageNode = config.SelectSingleNode("/globalizationConfiguration/applicationLanguages/language[. = '" + languageToDelete + "']");

            if (surveyLanguageNode != null)
            {
                surveyLanguageNode.ParentNode.RemoveChild(surveyLanguageNode);
                config.Save(configPath);

                //                _restartRequiredPnl.Visible = true;
                Application["RestartRequired"] = true;
            }

            ApplicationLanguages.Remove(languageToDelete);

            //Rebind app language repeater
            BindRepeater(_applicationLanguagesRepeater, ApplicationLanguages);

            //Rebind language lists
            BindLanguageDropDown();

            Checkbox.Globalization.Text.TextManager.RemoveApplicationLanguage(languageToDelete);

            Master.ShowStatusMessage(WebTextManager.GetText("/pageText/settings/languages.aspx/applicationLanguageRemoved"), StatusMessageType.Success);
        }

        /// <summary>
        /// Get the globalization config document
        /// </summary>
        /// <returns></returns>
        private XmlDocument GetGlobalizationConfig(out string globalizationConfigPath)
        {
            //Update Globalization config
            string appRoot = ConfigurationManager.DetermineAppConfigPath();
            XmlDocument doc = new XmlDocument();

            globalizationConfigPath = string.Format("{0}\\GlobalizationConfiguration.xml", appRoot);

            doc.Load(globalizationConfigPath);

            return doc;
        }

        /// <summary>
        /// Bind repeaters to the specified data source
        /// </summary>
        /// <param name="repeater"></param>
        /// <param name="dataSource"></param>
        private void BindRepeater(Repeater repeater, List<string> dataSource)
        {
            repeater.DataSource = dataSource;
            repeater.DataBind();
        }

        /// <summary>
        /// Bind the drop down list
        /// </summary>
        private void BindLanguageDropDown()
        {
            _defaultLanguageList.DataSource = ApplicationLanguages;
            _defaultLanguageList.DataBind();

            if (_defaultLanguageList.Items.FindByValue(DefaultLanguage) != null)
            {
                _defaultLanguageList.SelectedValue = DefaultLanguage;
            }

            _defaultLanguagePlace.Visible = _defaultLanguageList.Items.Count > 1;
        }
    }
}
