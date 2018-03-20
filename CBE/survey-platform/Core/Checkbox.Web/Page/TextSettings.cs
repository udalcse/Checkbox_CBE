using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Globalization.Text;

using System.Linq;

namespace Checkbox.Web.Page
{
    /// <summary>
    /// Base class for text settings
    /// </summary>
    public abstract class TextSettings : SettingsPage
    {
        /// <summary>
        /// Get title text id
        /// </summary>
        protected abstract string PageTitleTextId { get; }

        /// <summary>
        /// Set title
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            if (Master is BaseMasterPage)
            {
                //Set up the page title with link back to mananger
                PlaceHolder titleControl = new PlaceHolder();
                titleControl.Controls.Add(new HyperLink { NavigateUrl = "~/Settings/Manage.aspx", Text = WebTextManager.GetText("/pageText/settings/manage.aspx/title") });

                if (Utilities.IsNotNullOrEmpty(PageTitleTextId))
                {
                    titleControl.Controls.Add(new Label { Text = " - " + WebTextManager.GetText(PageTitleTextId) });
                }
                ((BaseMasterPage)Master).SetTitleControl(titleControl);
            }
        }

        /// <summary>
        /// Get the list of language names
        /// </summary>
        protected List<string> GetLanguageList()
        {
            List<string> languages = new List<string>(TextManager.ApplicationLanguages);
            languages.AddRange(TextManager.SurveyLanguages);

            return new List<string>(languages.Distinct());
        }

        /// <summary>
        /// Get the list of survey languages
        /// </summary>
        protected List<string> SurveyLanguages
        {
            get
            {
                if (Application["LanguageEdit_SurveyLanguages"] == null)
                {
                    Application["LanguageEdit_SurveyLanguages"] = new List<string>(TextManager.SurveyLanguages);
                }

                return (List<string>)Application["LanguageEdit_SurveyLanguages"];
            }
        }

        /// <summary>
        /// Get the list of application languages
        /// </summary>
        protected List<string> ApplicationLanguages
        {
            get
            {
                if (Application["LanguageEdit_ApplicationLanguages"] == null)
                {
                    Application["LanguageEdit_ApplicationLanguages"] = new List<string>(TextManager.ApplicationLanguages);
                }

                return (List<string>)Application["LanguageEdit_ApplicationLanguages"];
            }
        }


        /// <summary>
        /// Get currently selected defualt language
        /// </summary>
        protected string DefaultLanguage
        {
            get
            {
                if (Application["LanguageEdit_DefaultLanguage"] == null)
                {
                    Application["LanguageEdit_DefaultLanguage"] = TextManager.DefaultLanguage;
                }

                return (string)Application["LanguageEdit_DefaultLanguage"];
            }
        }


        /// <summary>
        /// Bind the drop down list
        /// </summary>
        /// <param name="dropDownList"></param>
        /// <param name="dataSource"></param>
        /// <param name="selectedValue"></param>
        protected void BindLanguageDropDown(DropDownList dropDownList, List<string> dataSource, string selectedValue)
        {
            dropDownList.Items.Clear();

            foreach (string langugageCode in dataSource)
            {
                dropDownList.Items.Add(langugageCode);
            }

            if (!Page.IsPostBack &&
                Utilities.IsNotNullOrEmpty(selectedValue)
                && dropDownList.Items.FindByValue(selectedValue) != null)
            {
                dropDownList.SelectedValue = selectedValue;
            }
        }

        /// <summary>
        /// Export texts
        /// </summary>
        /// <param name="languageCodes"></param>
        /// <param name="textIdMatches"></param>
        protected void ExportText(List<string> languageCodes, params string[] textIdMatches)
        {
            //Set up response
            Response.Expires = -1;
            Response.Buffer = true;
            Response.Clear();
            Response.ClearHeaders();
            Response.AddHeader("Content-Disposition", "attachment;filename=CheckboxTextExport.xml");
            Response.ContentType = "text/xml";
            Response.ContentEncoding = System.Text.Encoding.UTF8;

            //Write the data
            TextManager.ExportFilteredTexts(Response.Output, languageCodes.ToArray(), textIdMatches);

            //Flush & end
            Response.Flush();
            Response.End();
        }

        /// <summary>
        /// Get a text table
        /// </summary>
        /// <param name="languageCode"></param>
        /// <param name="matchStrings"></param>
        /// <returns></returns>
        protected DataTable GetTextTable(string languageCode, params string[] matchStrings)
        {
            DataTable returnData = null;

            DataTable allValidTexts = TextManager.GetAllMatchingTexts(matchStrings);

            returnData = allValidTexts.Clone();

            //If there is not text entry for a textId and language, it will not be present
            // in the list, so an empty entry will need to be added.

            //Build a list of all text ids
            DataRow[] textRows = allValidTexts.Select();

            List<string> allTextIds = new List<string>();

            foreach (DataRow textRow in textRows)
            {
                if (textRow["TextId"] != DBNull.Value && !allTextIds.Contains((string)textRow["TextId"]))
                {
                    if (!textRow["TextId"].ToString().ToLower().EndsWith("/description"))
                    {
                        allTextIds.Add((string)textRow["TextId"]);
                    }
                }
            }

            //Now get the texts
            foreach (string textId in allTextIds)
            {
                string text = TextManager.GetText(textId, languageCode);

                if (Utilities.IsNullOrEmpty(text) || text.Equals(textId, StringComparison.InvariantCultureIgnoreCase))
                {
                    text = "[" + languageCode + "] " + TextManager.GetText(textId, "en-US");
                }

                DataRow newRow = returnData.NewRow();
                newRow["TextId"] = textId;
                newRow["LanguageCode"] = languageCode;
                newRow["TextValue"] = text;

                returnData.Rows.Add(newRow);

            }

            return returnData;
        }
    }


}
