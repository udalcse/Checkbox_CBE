using System.Web.UI;
using AjaxControlToolkit;

[assembly: WebResource("Checkbox.Web.Ajax.Controls.ProgressBehavior.js", "text/javascript")]

namespace Checkbox.Web.Ajax.Controls
{
    /// <summary>
    /// 
    /// </summary>
    [RequiredScript(typeof(CommonToolkitScripts))]
    [RequiredScript(typeof(AnimationScripts))]
    [ClientScriptResource("Checkbox.Web.Ajax.Controls.ProgressBehavior", "Checkbox.Web.Ajax.Controls.ProgressBehavior.js")]
    public class AjaxProgressBar : ScriptControlBase
    {
        /// <summary>
        /// Get/set css class for progress bar outer div
        /// </summary>
        public string OuterDivCssClass { get; set; }

        /// <summary>
        /// Get/set css class for progress bar inner container.
        /// </summary>
        public string InnerDivCssClass { get; set; }

        /// <summary>
        /// Get/set css class for progress indicator
        /// </summary>
        public string IndicatorCssClass { get; set; }

        /// <summary>
        /// Get/set url for progress indicator image
        /// </summary>
        public string IndicatorImageUrl { get; set; }

        /// <summary>
        /// Get/set url for background image for progress
        /// </summary>
        public string InnerBgImageUrl { get; set; }

        /// <summary>
        /// Get/set background image for indicator
        /// </summary>
        public string IndicatorBgImageUrl { get; set; }

        /// <summary>
        /// Get/set text css class
        /// </summary>
        public string TextCssClass { get; set; }

        /// <summary>
        /// Get/set initial width percentage
        /// </summary>
        public int? InitialWidth { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected override HtmlTextWriterTag TagKey
        {
            get { return HtmlTextWriterTag.Div; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, OuterDivCssClass);
            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID + "_outer");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Width, Width.ToString());
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, InnerDivCssClass);
            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID + "_inner");

            if (!string.IsNullOrEmpty(InnerBgImageUrl))
            {
                writer.AddStyleAttribute("background", "url(" +  InnerBgImageUrl + ")");
            }

            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, IndicatorCssClass);
            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID + "_indicator");

            if (!string.IsNullOrEmpty(IndicatorBgImageUrl))
            {
                writer.AddStyleAttribute("background", "url(" + IndicatorBgImageUrl + ")");
            }

            if (!string.IsNullOrEmpty(IndicatorImageUrl))
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundImage, IndicatorImageUrl);
            }

            if(InitialWidth.HasValue)
            {
                if (InitialWidth < 0)
                {
                    InitialWidth = 0;
                }

                if(InitialWidth > 100)
                {
                    InitialWidth = 100;
                }

                writer.AddStyleAttribute(HtmlTextWriterStyle.Width, string.Format("{0}%", InitialWidth));
            }

            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            writer.RenderEndTag();
            writer.RenderEndTag();
            writer.RenderEndTag();

            writer.AddAttribute(HtmlTextWriterAttribute.Class, TextCssClass);
            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID + "_info");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            if (InitialWidth.HasValue)
            {
                writer.Write(string.Format("{0}%", InitialWidth));
            }

            writer.RenderEndTag();
        }

        /// <summary>
        /// 
        /// </summary>
        protected V GetPropertyValue<V>(string propertyName, V nullValue)
        {
            if (ViewState[propertyName] == null)
            {
                return nullValue;
            }
            return (V)ViewState[propertyName];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        protected void SetPropertyValue<V>(string propertyName, V value)
        {
            ViewState[propertyName] = value;
        }


        /// <summary>
        /// 
        /// </summary>
        [ExtenderControlProperty]
        [RequiredProperty]
        public ProgressBarMode Mode
        {
            get { return GetPropertyValue("Mode", ProgressBarMode.Manual); }
            set { SetPropertyValue("Mode", value); }
        }
    }
}