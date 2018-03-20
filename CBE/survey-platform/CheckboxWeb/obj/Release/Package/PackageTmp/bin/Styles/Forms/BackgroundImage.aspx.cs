using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Checkbox.Web;
using Checkbox.Styles;
using Checkbox.Common;
using Checkbox.Web.Page;

namespace CheckboxWeb.Styles.Forms
{
    public partial class BackgroundImage : EditStylePropertiesPage
    {
        [QueryParameter("img")]
        public string ImageUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected override string PageRequiredRolePermission { get { return "Form.Edit"; } }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            Master.OkClick += SubmitBtnClick;

            string bgImageUrl = string.Empty;
            string bgRepeat = string.Empty;

            if (TemplateId > 0)
            {
                if (CurrentStyleTemplate != null)
                {
                    bgImageUrl = CurrentStyleTemplate.GetElementProperty("body", "background-image").TrimStart("url(\"".ToCharArray()).TrimEnd("\")".ToCharArray());
                    bgRepeat = CurrentStyleTemplate.GetElementProperty("body", "background-repeat").ToLower();
                }
            }

            _isBackgroundRepeatDataChk.Checked = bgRepeat == "repeat";

            if (Utilities.IsNotNullOrEmpty(ImageUrl))
            {
                ImageUrl = ImageUrl.Replace("\"", string.Empty);

                if (!"none".Equals(ImageUrl, StringComparison.InvariantCultureIgnoreCase))
                {
                    bgImageUrl = ImageUrl;
                }
            }

            _imageSelector.Initialize(bgImageUrl, Page.IsPostBack);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SubmitBtnClick(object sender, EventArgs e)
        {
            string repeatValue = string.Empty;
            string url = string.Empty;

            if (CurrentStyleTemplate != null)
            {
                if (Utilities.IsNullOrEmpty(_imageSelector.ImageUrl))
                    url = "url(\"\")";
                else
                    url = "url(\"" + _imageSelector.ImageUrl + "\")";

                repeatValue = _isBackgroundRepeatDataChk.Checked ? "repeat" : "no-repeat";

                CurrentStyleTemplate.SetElementStyleProperty("body", "background-image", url);
                CurrentStyleTemplate.SetElementStyleProperty("body", "background-repeat", repeatValue);
            }


            var args = new Dictionary<string, string>();
            args["backgroundUrl"] = url;
            args["repeat"] = repeatValue;

            Master.CloseDialog("styleEditor.updateBackgroundImage", args);
        }
    }
}
