using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Checkbox.Web.UI.Controls
{
    /// <summary>
    /// Override for multi language label
    /// </summary>
    public class MultiLanguageCallout : CompositeControl
    {
        private MultiLanguageLabel _textLabel;
        private MultiLanguageLabel _subTextLabel;

        private Panel _popupContainer;
        private LiteralControl _textSpacer;
        private Image _closeImage;

        /// <summary>
        /// Override containing tag to be a div instead of a span
        /// </summary>
        protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Div; } }

        /// <summary>
        /// Create child controls
        /// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            _textLabel = new MultiLanguageLabel();
            _subTextLabel = new MultiLanguageLabel();

            _textSpacer = new LiteralControl("<br />");

            Style[HtmlTextWriterStyle.Position] = "absolute";

            //Create images
            string imageRoot = Management.ApplicationManager.ApplicationURL + Management.ApplicationManager.ApplicationRoot + "/Images/";            
            Image warningImage = new Image {ImageUrl = (imageRoot + "warning.gif")};

            _closeImage = new Image {ImageUrl = (imageRoot + "close.gif")};
            _closeImage.Style[HtmlTextWriterStyle.Cursor] = "pointer";
            
            //Create container panel for the arrow
            Panel arrowPanel = new Panel();
            arrowPanel.Style[HtmlTextWriterStyle.FontSize] = "1px";
            arrowPanel.Style[HtmlTextWriterStyle.Position] = "relative";
            arrowPanel.Style[HtmlTextWriterStyle.Left] = "1px";
            arrowPanel.Style["border-top"] = BorderString;

            //Create the child panels that comprise the arrow
            for (int i = 14; i > 0; i--)
            {
                Panel childPanel = new Panel();
                childPanel.Style[HtmlTextWriterStyle.Width] = i + "px";
                childPanel.Style[HtmlTextWriterStyle.Height] = "1px";
                childPanel.Style[HtmlTextWriterStyle.Overflow] = "hidden";
                childPanel.Style[HtmlTextWriterStyle.BackgroundColor] = GetColorString(BackColor);
                childPanel.Style["border-left"] = BorderString;
                arrowPanel.Controls.Add(childPanel);
            }
            
            //Create layout for the arrow panel
            Table arrowTable = new Table {CellSpacing = 0, CellPadding = 0, BorderStyle = BorderStyle.None};
            arrowPanel.Style[HtmlTextWriterStyle.Height] = "100%";

            TableRow arrowRow = new TableRow {VerticalAlign = VerticalAlign.Top};

            TableCell arrowCell = new TableCell {HorizontalAlign = HorizontalAlign.Right};
            arrowCell.Style[HtmlTextWriterStyle.PaddingTop] = "8px";
            arrowCell.Style[HtmlTextWriterStyle.FontSize] = "1px";
            arrowCell.Controls.Add(arrowPanel);

            arrowRow.Cells.Add(arrowCell);
            arrowTable.Rows.Add(arrowRow);

            //Create layout controls
            //Create a container table
            Table t = new Table {CellPadding = 0, CellSpacing = 0};
            t.Style[HtmlTextWriterStyle.Height] = "100%";

            TableCell arrowTableCell = new TableCell();
            arrowTableCell.Controls.Add(arrowTable);

            TableCell warningImageCell = new TableCell();
            warningImageCell.Style[HtmlTextWriterStyle.Padding] = "5px";
            warningImageCell.Style["border-top"] = BorderString;
            warningImageCell.Style["border-bottom"] = BorderString;
            warningImageCell.Style["border-left"] = BorderString;
            warningImageCell.Style[HtmlTextWriterStyle.BackgroundColor] = GetColorString(BackColor);
            warningImageCell.Controls.Add(warningImage);
            warningImageCell.VerticalAlign = VerticalAlign.Top;

            TableCell errorMessageCell = new TableCell();
            errorMessageCell.Style["border-top"] = BorderString;
            errorMessageCell.Style["border-bottom"] = BorderString;
            errorMessageCell.Style[HtmlTextWriterStyle.BackgroundColor] = GetColorString(BackColor);
            errorMessageCell.Controls.Add(_textLabel);
            errorMessageCell.Controls.Add(_textSpacer);
            errorMessageCell.Controls.Add(_subTextLabel);
            errorMessageCell.Style[HtmlTextWriterStyle.Padding] = "5px";

            TableCell closeImageCell = new TableCell();
            closeImageCell.Style[HtmlTextWriterStyle.Padding] = "2px";
            closeImageCell.HorizontalAlign = HorizontalAlign.Right;
            closeImageCell.Style[HtmlTextWriterStyle.Cursor] = "pointer";
            closeImageCell.Style["border-top"] = BorderString;
            closeImageCell.Style["border-bottom"] = BorderString;
            closeImageCell.Style["border-right"] = BorderString;
            closeImageCell.Style[HtmlTextWriterStyle.BackgroundColor] = GetColorString(BackColor);
            closeImageCell.Controls.Add(_closeImage);

            TableRow tr = new TableRow {VerticalAlign = VerticalAlign.Top};
            tr.Cells.Add(arrowTableCell);
            tr.Cells.Add(warningImageCell);
            tr.Cells.Add(errorMessageCell);
            tr.Cells.Add(closeImageCell);

            t.Rows.Add(tr);

            _popupContainer = new Panel();
            _popupContainer.Style[HtmlTextWriterStyle.Position] = "absolute";
            _popupContainer.ID = DateTime.Now.Ticks.ToString();

            _popupContainer.Controls.Add(t);

            _closeImage.Visible = false;

            Controls.Add(_popupContainer);
        }

        /// <summary>
        /// Get a color string
        /// </summary>
        /// <returns></returns>
        private static string GetColorString(System.Drawing.Color color)
        {
            if (color.IsEmpty)
            {
                return string.Empty;
            }

            return System.Drawing.ColorTranslator.ToHtml(color);
        }

        /// <summary>
        /// Override onprerender to show/hide spacer as necessary
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            //Set css class
            _textLabel.CssClass = TextCssClass;
            _subTextLabel.CssClass = SubTextCssClass;

            //Show/hide spacer
            if (_textLabel.Text == null
                || _textLabel.Text.Trim() == string.Empty
                || _subTextLabel.Text == null
                || _subTextLabel.Text.Trim() == string.Empty)
            {
                _textSpacer.Visible = false;
            }
            else
            {
                _textSpacer.Visible = true;
            }

            //Emit javascript to show hide
            //if (NamingContainer != null && ControlToCallout != null && ControlToCallout.Trim() != string.Empty)
            //{
               // Control c = NamingContainer.FindControl(ControlToCallout);

                //if (c != null)
                //{
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("function GetObj" + _popupContainer.ID + "(objName) {");
                    sb.AppendLine("  if (document.getElementById)");
                    sb.AppendLine("  {");
                    sb.AppendLine("    return document.getElementById(objName);");
                    sb.AppendLine("  }");
                    sb.AppendLine("  else if (document.all)");
                    sb.AppendLine("  {");
                    sb.AppendLine("    return document.all[objName];");
                    sb.AppendLine("  }");
                    sb.AppendLine("  else if (document.layers)");
                    sb.AppendLine("  {");
                    sb.AppendLine("    return document.layers[objName];");
                    sb.AppendLine("  }");
                    sb.AppendLine("}");
                    /* sb.AppendLine("function PositionDiv" + _popupContainer.ID + "() {");
                     sb.AppendLine("  var docID = '" + c.ClientID + "'");
                     sb.AppendLine("  var thisID = '" + this.ClientID + "'");
                     sb.AppendLine("  var obj = GetObj" + _popupContainer.ID + "(docID);");
                     sb.AppendLine("  var thisObj = GetObj" + _popupContainer.ID + "(thisID)");
                     sb.AppendLine("  if((obj) && (thisObj))");
                     sb.AppendLine("  {");
                     sb.AppendLine("    var offsetTop;");
                     sb.AppendLine("    var offSetLeft;");
                     sb.AppendLine("    var tmpOffsetParent = obj;");
                    
                     sb.AppendLine("    var valueT = 0;");
                     sb.AppendLine("    var valueL = 0;");
                     sb.AppendLine("    do{");
                     sb.AppendLine("      valueT += tmpOffsetParent.offsetTop  || 0;");
                     sb.AppendLine("      valueL += tmpOffsetParent.offsetLeft  || 0;");
                     sb.AppendLine("      tmpOffsetParent = tmpOffsetParent.offsetParent;");
                     sb.AppendLine("    } while(tmpOffsetParent);");

                     sb.AppendLine("   thisObj.style.top = valueT + 'px';");
                     sb.AppendLine("   thisObj.style.left = valueL + obj.scrollWidth + 8 + 'px';");
                     sb.AppendLine("  }");
                     sb.AppendLine("}");*/
                    sb.AppendLine("function HideObj" + _popupContainer.ID + "(objID) {");
                    sb.AppendLine("  var obj = GetObj" + _popupContainer.ID + "(objID);");
                    sb.AppendLine("  if(obj)");
                    sb.AppendLine("    obj.style.display = 'none';");
                    sb.AppendLine("}");

                    //sb.AppendLine("PositionDiv" + _popupContainer.ID + "();");
                    
                    Page.ClientScript.RegisterStartupScript(GetType(), _popupContainer.ID, sb.ToString(), true);

                    _closeImage.Attributes["onClick"] = "HideObj" + _popupContainer.ID + "('" + _popupContainer.ClientID + "');";
                    _closeImage.Visible = true;
                //}
            //}
        }

        /// <summary>
        /// Get the border string
        /// </summary>
        private string BorderString
        {
            get { return "1px solid " + GetColorString(BorderColor); }
        }

        /// <summary>
        /// Get/set the image name, relative to the app's images folder
        /// </summary>
        public virtual string ImageName { get; set; }

        /// <summary>
        /// Get/set the text css class
        /// </summary>
        public virtual string TextCssClass { get; set; }

        /// <summary>
        /// Get/set the subtext css class
        /// </summary>
        public virtual string SubTextCssClass { get; set; }

        /// <summary>
        /// Get/set text id for text
        /// </summary>
        public string TextID
        {
            get 
            {
                EnsureChildControls();
                return _textLabel.TextId;
            }
            set 
            {
                EnsureChildControls();
                _textLabel.TextId = value;
            }
        }

        /// <summary>
        /// Get/set text id for subtext
        /// </summary>
        public string SubTextID
        {
            get 
            {
                EnsureChildControls();
                return _subTextLabel.TextId;
            }
            set 
            {
                EnsureChildControls();
                _subTextLabel.TextId = value;
            }
        }

        /// <summary>
        /// Get/set the text
        /// </summary>
        public string Text
        {
            get 
            {
                EnsureChildControls();
                return _textLabel.Text;
            }
            set
            {
                EnsureChildControls();
                _textLabel.Text = value;
            }
        }

        /// <summary>
        /// Get/set the sub text
        /// </summary>
        public string SubText
        {
            get
            {
                EnsureChildControls();
                return _subTextLabel.Text;
            }
            set
            {
                EnsureChildControls();
                _subTextLabel.Text = value;
            }
        }

        /// <summary>
        /// Get/set the id of the control to callout
        /// </summary>
        public string ControlToCallout { get; set; }
    }
}
