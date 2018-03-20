using System;
using System.Web;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Wcf.Services.Proxies;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI;

namespace Checkbox.Web.Forms.UI.Rendering
{
    /// <summary>
    /// Base class for survey item renderer user controls
    /// </summary>
    public class UserControlSurveyItemRendererBase : UserControlItemRendererBase
    {
        /// <summary>
        /// Get/set item number for survey.
        /// </summary>
        public int? ItemNumber { get; set; }

        /// <summary>
        /// Get whether to show the item number
        /// </summary>
        protected bool ShowItemNumber { get { return ItemNumber.HasValue; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataTransferObject"></param>
        /// <param name="itemNumber"></param>
        /// <param name="exportMode"></param>
        public override void Initialize(IItemProxyObject dataTransferObject, int? itemNumber, ExportMode exportMode)
        {
            if (string.IsNullOrEmpty(ID))
            {
                ID = dataTransferObject.ItemId.ToString();
                ClientIDMode = System.Web.UI.ClientIDMode.Predictable;
            }

            base.Initialize(dataTransferObject, itemNumber, exportMode);

            ItemNumber = itemNumber;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataTransferObject"></param>
        /// <param name="itemNumber"></param>
        public override void Initialize(IItemProxyObject dataTransferObject, int? itemNumber)
        {
            Initialize(dataTransferObject, itemNumber, ExportMode.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            if (DataTransferObject != null && Model.IsSPCArgument)
            {   
                foreach (var c in Controls)
                {
                    MarkSPCAttribute(c);
                }
            }                
            base.OnLoad(e);
        }

        /// <summary>
        /// Mark SPC attribute and take into account place holders
        /// </summary>
        /// <param name="c"></param>
        private static void MarkSPCAttribute(object c)
        {
            if (c is PlaceHolder)
            {
                foreach (var cc in (c as PlaceHolder).Controls)
                {
                    MarkSPCAttribute(cc);
                }
            }
            else
            {
                if (c is WebControl)
                {
                    ((WebControl)c).Attributes.Add("spcMarker", "true");
                } else
                if (c is HtmlControl)
                {
                    ((HtmlControl)c).Attributes.Add("spcMarker", "true");
                } else
                if (c is UserControl)
                {
                    ((UserControl)c).Attributes.Add("spcMarker", "true");
                }
            }
        }


        /// <summary>
        /// Get survey item model
        /// </summary>
        public SurveyResponseItem Model
        {
            get
            {
                if (DataTransferObject != null && !(DataTransferObject is SurveyResponseItem))
                {
                    throw new Exception("Model for survey item renderer must be of or extend type: Checkbox.Wcf.Proxies.SurveyResponseItem.");
                }

                return (SurveyResponseItem)DataTransferObject;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool IsMobileSurvey
        {
            get
            {
                var renderMode = RenderMode;
                if (renderMode != RenderMode.Default)
                    return renderMode == RenderMode.SurveyMobilePreview;

                if (Page == null)
                    return false;
                
                return WebUtilities.IsBrowserMobile(Request ?? Page.Request);
            }
        }

        /// <summary>
        /// Get render mode
        /// </summary>
        protected RenderMode RenderMode
        {
            get
            {
                string mode = HttpContext.Current.Request.QueryString["mode"];
                RenderMode renderMode;
                if (Enum.TryParse(mode, out renderMode))
                    return renderMode;

                return RenderMode.Default;
            }
        }

        /// <summary>
        /// Gets the option text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="encode">if set to <c>true</c> [encode].</param>
        /// <returns></returns>
        protected string GetOptionText(string text, bool encode = true)
        {
            if (!encode)
                return text;

            string formatted = Utilities.EncodeTagsInHtmlContent(Utilities.AdvancedHtmlDecode(text));

            if (!string.IsNullOrEmpty(formatted) && !Utilities.IsHtmlFormattedText(formatted)
                && !formatted.Contains(Utilities.AdvancedHtmlEncode("<"))
                && !formatted.Contains(Utilities.AdvancedHtmlEncode(">")))
                formatted = Utilities.AdvancedHtmlEncode(formatted);

            return formatted;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        protected string GetOptionText(SurveyResponseItemOption option)
        {
            return GetOptionText(option.Text);
        }

        /// <summary>
        /// Gets the matrix row text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="encode">if set to <c>true</c> [encode].</param>
        /// <returns></returns>
        protected string GetMatrixRowText(string text, bool encode = true)
        {
            return GetOptionText(text, encode);
        }

    }
}
