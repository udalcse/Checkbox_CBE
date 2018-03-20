using System;
using System.Text;
using System.Threading;
using System.Web.UI;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items;
using Checkbox.Globalization.Text;
using Checkbox.Web.Forms.UI.Rendering;
using Prezza.Framework.ExceptionHandling;

namespace CheckboxWeb
{
    /// <summary>
    /// Render HTML for an item
    /// </summary>
    public partial class ItemHtml : Checkbox.Web.Page.BasePage
    {
        /// <summary>
        /// Get survey id
        /// </summary>
        private  int? SurveyId
        {
            get { return Utilities.AsInt(Request.QueryString["s"]); }    
        }

        /// <summary>
        /// Get item id
        /// </summary>
        protected int? ItemId
        {
            get { return Utilities.AsInt(Request.QueryString["i"]); }    
        }

        /// <summary>
        /// 
        /// </summary>
        private bool ItemHtmlOnly
        {
            get { return Utilities.AsBool(Request.QueryString["iho"], false); }
        }

        /// <summary>
        /// Get language code
        /// </summary>
        private string LanguageCode
        {
            get
            {
                return
                    string.IsNullOrEmpty(Request.QueryString["l"])
                        ? TextManager.DefaultLanguage
                        : Request.QueryString["l"];
            }
        }

        /// <summary>
        /// Get render mode
        /// </summary>
        private RenderMode RenderMode
        {
            get { return (RenderMode) Enum.Parse(typeof (RenderMode), Request.QueryString["m"]); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            string errorMessage = "<div class=\"error\">An error occurred while generating HTML for item with id: " + ItemId + "<br />{0}</div>";
            
            try
            {
                base.OnInit(e);

                //Depending on context, get appropriate style
                if (SurveyId.HasValue)
                {
                    var styleString = new StringBuilder();
                    styleString.Append("<link rel=\"stylesheet\" type=\"text\\css\" src=\"");
                    styleString.Append(ResolveUrl("~/CustomSurveyStyles.css"));
                    styleString.AppendLine("\" />");

                    var lightweightRt = ResponseTemplateManager.GetLightweightResponseTemplate(SurveyId.Value);

                    if (lightweightRt != null
                        && lightweightRt.StyleTemplateId.HasValue)
                    {
                        styleString.Append("<link rel=\"stylesheet\" type=\"text\\css\" src=\"");
                        styleString.AppendLine(ResolveUrl("~/ViewContent.aspx"));
                        styleString.Append("?st=");
                        styleString.Append(lightweightRt.StyleTemplateId);
                        styleString.Append("\" />");
                    }

                    _styleInclude.Text = styleString.ToString();
                }

                //Include jQuery & other scripts
             //   var scriptString = string.Format("<script type=\"text/javascript\" src=\"{0}\"></script>", ResolveUrl("~/Resources/jquery-latest.min.js"));
              //  _scriptInclude.Text = scriptString;

                //Add control to page
                _itemPlace.Controls.Add(GetItemRenderer(ItemId.Value, RenderMode, LanguageCode));

                return;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "UIProcess");

                string message = ex.Message + "<br /><br />" + ex.StackTrace.Replace(Environment.NewLine, "<br />");

                errorMessage = string.Format(errorMessage, message);
            }
            
            _itemPlace.Controls.Add(new LiteralControl(string.Format(Utilities.RemoveScript(errorMessage), ItemId)));
        }

        /// <summary>
        /// Get survey item renderer HTML for an item
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="mode"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public Control GetItemRenderer(int itemId, RenderMode mode, string languageCode)
        {
            Control renderer = WebItemRendererManager.GetItemRenderer(itemId, SurveyId, mode, languageCode);

            if (renderer == null)
            {
                return new LiteralControl();
            }

            renderer.DataBind();

            //If renderer is an item renderer, and not an error control or such,
            // initialize it
            if (renderer is IItemRenderer)
            {
                ((IItemRenderer) renderer).BindModel();
            }

            return renderer;
        }

        /// <summary>
        /// Override render, if necessary
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            if (ItemHtmlOnly)
            {
                _itemPlace.RenderControl(writer);
            }
            else
            {
                base.Render(writer);
            }
        }
    }
}