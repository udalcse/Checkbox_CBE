using System.Collections.Generic;
using System.Web.UI;
using Checkbox.Forms;

namespace CheckboxWeb.Forms.Surveys.Controls
{
    public partial class SurveyText : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// Get currente language code for control
        /// </summary>
        public string LanguageCode { get; private set; }

        /// <summary>
        /// Initialize control with survey to edit
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="currentLanguage"></param>
        /// <param name="textOverrides"></param>
        public void Initialize(ResponseTemplate rt, string currentLanguage, Dictionary<string, string> textOverrides)
        {
            //Set current langauge
            LanguageCode = currentLanguage;

            //Add text items to text editor controls to template.
            AddTextItems(rt, currentLanguage, textOverrides);

            _textEditor.DataBind();
        }

        /// <summary>
        /// Bind text editor controls to rt.
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="currentLanguage"></param>
        /// <param name="textOverrides"></param>
        protected virtual void AddTextItems(ResponseTemplate rt, string currentLanguage, Dictionary<string, string> textOverrides)
        {
        }

        /// <summary>
        /// Get text values
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetTextValues()
        {
            return _textEditor.GetTextValues();
        }
    }
}