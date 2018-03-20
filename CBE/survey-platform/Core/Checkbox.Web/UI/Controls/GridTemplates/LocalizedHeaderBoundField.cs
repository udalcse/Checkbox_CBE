using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Checkbox.Web;

namespace Checkbox.Web.UI.Controls.GridTemplates
{
    /// <summary>
    /// Custom bound field that supports a localized header
    /// </summary>
    public class LocalizedHeaderBoundField : BoundField
    {
        private string _headerTextID;

        /// <summary>
        /// Get/set the text id for the header text
        /// </summary>
        public string HeaderTextID
        {
            get { return _headerTextID; }
            set { _headerTextID = value; }
        }

        /// <summary>
        /// Create child controls and bind events
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
                l.DataBinding += new EventHandler(l_DataBinding);
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
        /// Get the text for the header
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
