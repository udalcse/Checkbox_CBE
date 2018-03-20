using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Management;
using Checkbox.Forms;
using Checkbox.Web;

namespace CheckboxWeb.Forms.Surveys.Controls
{
    /// <summary>
    /// Displays a button Share via Twitter and allows to share the survey
    /// </summary>
    public partial class TwitterButton : Checkbox.Web.Common.UserControlBase
    {
        public string ResponseTemplateName
        {
            get;
            set;
        }

        public string SurveyURL
        {
            get;
            set;
        }

        public string Text
        {
            get
            {
                return string.Format("{0} {1}", WebTextManager.GetText("/twitterButton/tweetText"), ResponseTemplateName);
            }
        }
    }
}