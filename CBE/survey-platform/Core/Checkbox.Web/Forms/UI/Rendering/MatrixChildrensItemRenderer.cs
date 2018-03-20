using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using Checkbox.Forms.Items;

namespace Checkbox.Web.Forms.UI.Rendering
{

    /// <summary>
    /// This class is used only for childers of Matrix item. It doesn't wrapped control in any tag.
    /// </summary>
    public class MatrixChildrensItemRenderer : UserControlHostItemRenderer
    {
        /// <summary>
        /// This property can be None, Both, Vertical or Horizontal
        /// </summary>
        public String GridLineMode { get; set; }

        /// <summary>
        /// return width of the column
        /// </summary>
        public int? ColumnWidth { get; set; }

        /// <summary>
        /// This property can be Left, Right or Center
        /// </summary>
        public String RowTextPosition { get; set; }

        /// <summary>
        /// This property can be RowText or null (other variants can be implemented later)
        /// </summary>
        public MatrixChildType ChildType { get; set; }
        
        /// <summary>
        /// Render the control
        /// </summary>
        /// <param name="writer"></param>
        /// <remarks>This method is needed not to wrap the control in any tag.</remarks>
        public override void RenderControl(HtmlTextWriter writer)
        {
            foreach (Control control in Controls)
            {
                control.RenderControl(writer);
            }
        } 
    }
}
