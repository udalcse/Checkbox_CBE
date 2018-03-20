using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using Checkbox.Common;
using Checkbox.Management;


namespace Checkbox.Web.UI.Controls
{
    /// <summary>
    /// Web control for hosting children in a vertical scrolling div
    /// </summary>
    [ParseChildren(ChildrenAsProperties = false)]
    public class MultiLanguageSliderDiv : Common.WebControlBase
    {
        private Panel _headerContainer;

        private Image _toggleImg;
        private ImageButton _expandBtn;
        private ImageButton _collapseBtn;

        /// <summary>
        /// Get/set whether to use javascript
        /// </summary>
        public bool UseJavascript { get; set; }

        /// <summary>
        /// Get/set the text id for the header
        /// </summary>
        public string TextID { get; set; }

        /// <summary>
        /// Get/set css class for header
        /// </summary>
        public string HeaderCssClass { get; set; }

        /// <summary>
        /// Get/set whether the item is open by default
        /// </summary>
        public bool Opened
        {
            get 
            {
                if (ViewState["Opened_" + ID] == null)
                {
                    ViewState["Opened_" + ID] = false;
                }

                return (bool)ViewState["Opened_" + ID];
            }
            set 
            {
                ViewState["Opened_" + ID] = value;
            }
        }

        /// <summary>
        /// Get/set the expand icon.
        /// </summary>
        public string ExpandImage { get; set; }

        /// <summary>
        /// Get/set the "collapse" image
        /// </summary>
        public string CollapseImage { get; set; }

        /// <summary>
        /// Get/set the header icon
        /// </summary>
        public string HeaderIcon { get; set; }

        /// <summary>
        /// Get the ID of the div
        /// </summary>
        private string SliderDivID
        {
            get { return "slider_div_" + ID; }
        }

        /// <summary>
        /// Get the click action
        /// </summary>
        private string ClickAction
        {
            get
            {
                return "ToggleDiv('" + SliderDivID + "');";
            }
        }

        /// <summary>
        /// Ensure child controls are created
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            EnsureChildControls();
        }
        
        /// <summary>
        /// Handle on load to emit some javascript
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SetImageProperties();

            if(Page != null && UseJavascript)
            {
                RegisterScripts();
            }
        }

        /// <summary>
        /// Set the visibility of the buttons depending on mode and whether images are set
        /// </summary>
        private void SetImageProperties()
        {
            if (UseJavascript)
            {
                _toggleImg.Visible = true;
                _expandBtn.Visible = false;
                _collapseBtn.Visible = false;

                if (Utilities.IsNotNullOrEmpty(ExpandImage) && Utilities.IsNotNullOrEmpty(CollapseImage))
                {
                    _toggleImg.ImageUrl = Opened ? CollapseImage : ExpandImage;
                    _toggleImg.AlternateText = Opened ? WebTextManager.GetText("/controlText/multiLanguageSliderDiv/collapse") : WebTextManager.GetText("/controlText/multiLanguageSliderDiv/expand");
                }
            }
            else
            {
                _toggleImg.Visible = false;

                if (Page != null && !Page.IsPostBack)
                {
                    _collapseBtn.Visible = Opened;
                    _expandBtn.Visible = !Opened;
                }

                if (Utilities.IsNotNullOrEmpty(ExpandImage))
                {
                    _expandBtn.ImageUrl = ExpandImage;
                }

                if (Utilities.IsNotNullOrEmpty(CollapseImage))
                {
                    _collapseBtn.ImageUrl = CollapseImage;
                }
            }
        }

        /// <summary>
        /// Register any necessary javascript
        /// </summary>
        private void RegisterScripts()
        {
            //Add the link to the javascript source
            RegisterClientScriptInclude(GetType(), "sliderScript", ApplicationManager.ApplicationRoot + "/Resources/SlidingDiv.js");

            //Initialize the image paths
            string expandImageString = "SetExpandImage('" + ExpandImage + "');";
            string collapseImageString = "SetCollapseImage('" + CollapseImage + "');";

            if (Utilities.IsNotNullOrEmpty(ExpandImage) && Utilities.IsNotNullOrEmpty(CollapseImage))
            {
                Page.ClientScript.RegisterStartupScript(GetType(), "imagePathExpandStartup_" + ID, expandImageString, true);
                Page.ClientScript.RegisterStartupScript(GetType(), "imagePathCollapseStartup_" + ID, collapseImageString, true);
            }
        }

        /// <summary>
        /// Render the control
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            _headerContainer.RenderControl(writer);

            if (Opened)
            {
                writer.Write("<div id=\"" + SliderDivID + "\" style=\"display:block;\">");
            }
            else
            {
                writer.Write("<div id=\"" + SliderDivID + "\" style=\"display:none;\">");
            }

            //Render children
            foreach (Control c in Controls)
            {
                if (c.ID != "HeaderContainer_" + ID)
                {
                    c.RenderControl(writer);
                }
            }

            writer.Write("</div>");
        }

        /// <summary>
        /// Create the div header controls
        /// </summary>
        protected override void  CreateChildControls()
        {
            //Create the header container
            _headerContainer = new Panel {ID = ("HeaderContainer_" + ID)};
            _headerContainer.Style[HtmlTextWriterStyle.Height] = "18px;";
            
            //Title container
            Panel titleContainer = new Panel();
            titleContainer.Style["float"] = "left";
            titleContainer.CssClass = HeaderCssClass;
            titleContainer.Style[HtmlTextWriterStyle.Height] = "18px;";

            //Image container
            Panel imageContainer = new Panel();
            imageContainer.Style["float"] = "right";
            imageContainer.CssClass = HeaderCssClass;
            imageContainer.Style[HtmlTextWriterStyle.Height] = "18px;";

            //Add controls
            _headerContainer.Controls.Add(titleContainer);
            _headerContainer.Controls.Add(imageContainer);

            Controls.Add(_headerContainer);

            //Attempt to set the width for the title and image
            if (!Width.IsEmpty)
            {
                _headerContainer.Style[HtmlTextWriterStyle.Width] = Width.ToString();
                titleContainer.Style[HtmlTextWriterStyle.Width] = (Width.Value - 23) + "px;";
                imageContainer.Style[HtmlTextWriterStyle.Width] = "15px";
            }

            //Make the header clickable
            if (UseJavascript)
            {
                _headerContainer.Attributes["onclick"] = ClickAction;
            }

            //Set the title text
            if (Utilities.IsNotNullOrEmpty(TextID))
            {
                titleContainer.Controls.Add(new LiteralControl(WebTextManager.GetText(TextID)));
            }

            //Create the image controls

            //Plain image for JS mode
            _toggleImg = new Image();
            imageContainer.Controls.Add(_toggleImg);

            //Image buttons for postback mode
            _expandBtn = new ImageButton {AlternateText = WebTextManager.GetText("/controlText/multiLanguageSliderDiv/expand")};
            _expandBtn.Click += _expandBtn_Click;
            _expandBtn.ID = "Expand_" + ID;
            imageContainer.Controls.Add(_expandBtn);

            _collapseBtn = new ImageButton {AlternateText = WebTextManager.GetText("/controlText/multiLanguageSliderDiv/collapse")};
            _collapseBtn.Click += _collapseBtn_Click;
            _collapseBtn.ID = "Collapse_" + ID;
            imageContainer.Controls.Add(_collapseBtn);
        }

        /// <summary>
        /// Handle clicking the "collapse" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _collapseBtn_Click(object sender, ImageClickEventArgs e)
        {
            Opened = false;

            _expandBtn.Visible = true;
            _collapseBtn.Visible = false;
        }

        /// <summary>
        /// Handle clicking the "expand" button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _expandBtn_Click(object sender, ImageClickEventArgs e)
        {
            Opened = true;

            _expandBtn.Visible = false;
            _collapseBtn.Visible = true;
        }
    }
}
