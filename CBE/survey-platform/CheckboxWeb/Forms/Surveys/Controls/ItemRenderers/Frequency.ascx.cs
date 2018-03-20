using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Web;
using Checkbox.Web.Forms.UI.Rendering;
using Checkbox.Analytics.Items.Configuration;
using Checkbox.Analytics.Items.UI;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Analytics.Items;
using Checkbox.Web.Analytics.UI.Rendering;
using Checkbox.Web.UI.Controls;


namespace CheckboxWeb.Forms.Surveys.Reports.Controls.ItemRenderers
{
    public partial class Frequency : UserControlAnalysisItemRendererBase
    {

        /// <summary>
        /// Get a summary grid for "other" text
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="appearance"></param>
        /// <returns></returns>
        protected virtual WebControl GetGrid(int itemID, AnalysisItemAppearanceData appearance)
        {
            SummaryGrid grid = new SummaryGrid();
            grid.Appearance = appearance;
            grid.ShowFooter = true;

            return grid;
        }

        /// <summary>
        /// Binds a model to controls
        /// </summary>
        public override void BindModel()
        {
            EnsureChildControls();

            FrequencyItem item = Model as FrequencyItem;
            FrequencyItemAppearanceData appearance = Appearance as FrequencyItemAppearanceData;
            TableCell cell = null;
            TableRow rowContent = null;

            if (item != null)
            {
                if (item.SourceItemIDs.Count == 0)
                {
                    rowContent = new TableRow();
                    tblContent.Controls.Add(rowContent);
                    cell = new TableCell();
                    rowContent.Controls.Add(cell);
                    cell.Text = "No items were selected.";
                }
                else
                {
                    foreach (int itemId in item.SourceItemIDs)
                    {
                        rowContent = new TableRow();
                        tblContent.Controls.Add(rowContent);
                        cell = new TableCell();

                        string chartTitle = GetChartTitle(itemId, item, appearance);

                        WebControl grid = GetGrid(itemId, appearance);

                        if (grid != null)
                        {
                            if (String.Compare(item.GetSourceItemTypeName(itemId), "RadioButtonScale", true) != 0)
                            {
                                BindGrid(itemId, grid, (DataSet)item.Data);

                                Label lbl = new Label();
                                lbl.Text = chartTitle;
                                lbl.Font.Bold = true;

                                cell.Controls.Add(lbl);                                
                                cell.Controls.Add(grid);
                            }

                            grid.Width = appearance.Width;
                        }

                        rowContent.Controls.Add(cell);
                        cell.Controls.Add(grid);
                    }
                }
            }
            else
            {
                cell = new TableCell();
                rowContent.Controls.Add(cell);

                cell.Text = "This item has no data.";
            }

            base.BindModel();
        }

        /// <summary>
        /// Bind answer data to the grid containing "other" answers.
        /// </summary>
        /// <param name="itemID">ID of item to display other answer for</param>
        /// <param name="grid">Grid to bind data to.</param>
        /// <param name="gridData">DataSet containing data to bind to grid.</param>
        protected virtual void BindGrid(int itemID, WebControl grid, DataSet gridData)
        {
            if (grid is SummaryGrid && gridData.Tables.Count > 1)
            {
                DataTable resultData = gridData.Tables[1].Clone();

                DataRow[] resultRows = gridData.Tables[1].Select("ItemID = " + itemID.ToString(), null, DataViewRowState.CurrentRows);

                foreach (DataRow resultRow in resultRows)
                {
                    //Exclude zero values if necessary
                    if (((FrequencyItemAppearanceData)Appearance).ShowDataLabelZeroValues
                        || (resultRow["AnswerCount"] != DBNull.Value && Convert.ToInt32(resultRow["AnswerCount"]) > 0))
                    {
                        resultData.ImportRow(resultRow);
                    }
                }

                //Accept changes
                resultData.AcceptChanges();

                string formatString = "{0:F" + ((AnalysisItemAppearanceData)Appearance).Precision.ToString() + "}";

                SummaryGrid summaryGrid = (SummaryGrid)grid;
                summaryGrid.AddBoundColumn("OptionText", WebTextManager.GetText("/controlText/frequencyItemRenderer/question"), string.Empty, HorizontalAlign.Left, HorizontalAlign.Left);
                summaryGrid.AddBoundColumn("AnswerCount", WebTextManager.GetText("/controlText/frequencyItemRenderer/count"), string.Empty, HorizontalAlign.Right, HorizontalAlign.Right);
                summaryGrid.AddBoundColumn("AnswerPercent", WebTextManager.GetText("/controlText/frequencyItemRenderer/percent"), formatString, HorizontalAlign.Right, HorizontalAlign.Right);
                summaryGrid.DataSource = resultData;
                summaryGrid.DataBind();
            }
        }
        /// <summary>
        /// Get the title of a chart
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="appearance"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        protected virtual string GetChartTitle(int itemID, AnalysisItem model, AnalysisItemAppearanceData appearance)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(model.GetItemText(itemID));

            if (appearance.ShowResponseCountInTitle)
            {
                if (model.SourceItemIDs.Count > 1)
                {
                    sb.Append("  ");
                }
                else
                {
                    sb.Append(Environment.NewLine);
                }
                sb.Append("(");

                if (model.Data != null)
                    sb.Append(model.GetItemResponseCount(itemID));
                else
                    sb.Append("0");

                sb.Append("  ");
                sb.Append(WebTextManager.GetText("/controlText/analysisItemRenderer/responses"));
                sb.Append(")");
            }

            return sb.ToString();
        }
    }
}

  
