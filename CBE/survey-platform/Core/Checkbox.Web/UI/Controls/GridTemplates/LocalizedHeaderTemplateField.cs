using System;
using System.Web.UI.WebControls;

namespace Checkbox.Web.UI.Controls.GridTemplates
{
    /// <summary>
    /// Template field that supports a localized header text
    /// </summary>
    public class LocalizedHeaderTemplateField : TemplateField
    {
        /// <summary>
        /// Get/set the text id for the header
        /// </summary>
        public string HeaderTextID { get; set; }

        /// <summary>
        /// Override initialize cell to bind databind event for header label
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="cellType"></param>
        /// <param name="rowState"></param>
        /// <param name="rowIndex"></param>
        public override void InitializeCell(DataControlFieldCell cell, DataControlCellType cellType, DataControlRowState rowState, int rowIndex)
        {
            if (cellType == DataControlCellType.Header)
            {
                Label l = new Label();
                l.DataBinding += l_DataBinding;
                cell.Controls.Add(l);
            }
            else
            {
                base.InitializeCell(cell, cellType, rowState, rowIndex);
            }

            if (ItemStyle.HorizontalAlign != HorizontalAlign.NotSet)
                cell.Attributes.Add("align", ItemStyle.HorizontalAlign.ToString());
            if (!ItemStyle.Width.IsEmpty)
                cell.Attributes.Add("width", ItemStyle.Width.ToString());          
        }

        /// <summary>
        /// Bind header data to header label
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void l_DataBinding(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(HeaderTextID))
            {
                ((Label)sender).Text = WebTextManager.GetText(HeaderTextID);
            }
        }
    }
}
