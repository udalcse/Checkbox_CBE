using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Xml;
using Checkbox.Web;
using Checkbox.Web.Page;
using Prezza.Framework.Configuration;
using Checkbox.Globalization.Text;

namespace CheckboxWeb.Settings
{
    public partial class AddLanguage : TextSettings
    {
        protected override string PageTitleTextId
        {
            get { return string.Empty; }
        }

        protected override void OnPageInit()
        {
            base.OnPageInit();

            Master.SetTitle(WebTextManager.GetText("/pageText/settings/navigation.ascx/languages"));
            Master.OkClick += Save_Click;
        }

        /// <summary>
        /// Get the required page role
        /// </summary>
        protected override string PageRequiredRole { get { return "System Administrator"; } }

        /// <summary>
        /// Export Selected Texts
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Save_Click(object sender, EventArgs e)
        {
            EventHandlerWrapper(sender, e, DoSave);
        }


        /// <summary>
        /// Handle save click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DoSave(object sender, EventArgs e)
        {
            if (ValidateInputs())
            {
                String message = null;
                String newLanguageCode = _languageCode.Text.Trim();


                if (Request.QueryString["type"] == "survey")
                {
                    AddLanguageCode("surveyLanguages", newLanguageCode);
                    SurveyLanguages.Add(newLanguageCode);
                    message = WebTextManager.GetText("/pageText/settings/languages.aspx/surveyLanguageAdded");
                }
                else
                {
                    AddLanguageCode("applicationLanguages", newLanguageCode);
                    ApplicationLanguages.Add(newLanguageCode);
                    message = WebTextManager.GetText("/pageText/settings/languages.aspx/applicationLanguageAdded");
                }

                //Update language text information in DB
                List<string> languages = GetLanguageList();

                foreach (string languageCode in languages)
                {
                    TextManager.SetText("/languageText/" + newLanguageCode, languageCode, newLanguageCode);
                }

                foreach (string languageCode in languages)
                {
                    if (languageCode != newLanguageCode)
                        TextManager.SetText("/languageText/" + languageCode, newLanguageCode, languageCode);
                }

                String argMessage = String.Empty;

                foreach (string word in message.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    argMessage += word + "+";
                }

                if (!String.IsNullOrEmpty(argMessage))
                    argMessage = argMessage.Remove(argMessage.Length - 1);

                Dictionary<string, string> args = new Dictionary<string, string>();

                args.Add("result", "ok");
                args.Add("dialog", "Languages.aspx");

                args.Add("message", argMessage);

                Master.CloseDialog(args);
            }
        }


        /// <summary>
        /// Add a language to the globalization configuration
        /// </summary>
        /// <param name="languageType"></param>
        /// <param name="languageCode"></param>
        private void AddLanguageCode(string languageType, string languageCode)
        {
            string glPath;
            XmlDocument glConfig = GetGlobalizationConfig(out glPath);

            XmlNode parentNode = glConfig.SelectSingleNode("/globalizationConfiguration/" + languageType);

            if (parentNode != null)
            {
                //Check for language already existing
                XmlNode languageNode = parentNode.SelectSingleNode("language[. = '" + languageCode + "']");

                if (languageNode == null)
                {
                    languageNode = glConfig.CreateElement("language");
                    languageNode.InnerText = languageCode;
                    parentNode.AppendChild(languageNode);
                    glConfig.Save(glPath);
                }
            }

            if (languageType == "applicationLanguages")
                Checkbox.Globalization.Text.TextManager.AddApplicationLanguage(languageCode);
            else if (languageType == "surveyLanguages")
                Checkbox.Globalization.Text.TextManager.AddSurveyLanguage(languageCode);

            //Checkbox.Globalization.Text.TextManager.RefreshGlobalizationConfiguration(); does not work!!!
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
        /// Depending on the mode, check for text existance
        /// </summary>
        protected bool ValidateInputs()
        {
            string languageCode = _languageCode.Text.Trim();

            if (languageCode.Length != 5)
            {
                Master.ShowStatusMessage(WebTextManager.GetText("/pageText/settings/addLanguage.aspx/languageCodeFormat"), StatusMessageType.Error);
                return false;

            }

            if (languageCode.Length == 5 && languageCode[2] != '-')
            {
                Master.ShowStatusMessage(WebTextManager.GetText("/pageText/settings/addLanguage.aspx/languageCodeFormat"), StatusMessageType.Error);
                return false;

            }

            List<string> listToCheck;

            if (Request.QueryString["type"] == "survey")
            {
                listToCheck = SurveyLanguages;
            }
            else
            {
                listToCheck = ApplicationLanguages;
            }

            foreach (string language in listToCheck)
            {
                if (language.Equals(languageCode, StringComparison.InvariantCultureIgnoreCase))
                {
                    Master.ShowStatusMessage(WebTextManager.GetText("/pageText/settings/addLanguage.aspx/languageInUse"), StatusMessageType.Error);
                    return false;
                }
            }

            return true;
        }
    }
}
