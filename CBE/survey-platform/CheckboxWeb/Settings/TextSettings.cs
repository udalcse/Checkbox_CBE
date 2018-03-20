using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Globalization.Text;

namespace Checkbox.Web.Page
{
    public class TextSettings : SecuredPage
    {
        /// <summary>
        /// Get the list of language names
        /// </summary>
        protected List<string> GetLanguageNamesList()
        {
            List<string> languages = new List<string>();

            foreach (string language in ApplicationLanguages)
            {
                if (!languages.Contains(language))
                {
                    languages.Add(language);
                }
            }

            foreach (string language in SurveyLanguages)
            {
                if (!languages.Contains(language))
                {
                    languages.Add(language);
                }
            }

            return languages;
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
            dropDownList.DataSource = dataSource;
            dropDownList.DataBind();

            if (dropDownList.Items.FindByValue(selectedValue) != null)
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
