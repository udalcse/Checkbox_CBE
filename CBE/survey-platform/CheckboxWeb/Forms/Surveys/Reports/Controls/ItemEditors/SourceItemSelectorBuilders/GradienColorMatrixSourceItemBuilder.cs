using System.Collections.Generic;
using System.Linq;
using Checkbox.Forms.Data;
using Checkbox.Users;
using Checkbox.Wcf.Services;
using Checkbox.Wcf.Services.Proxies;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors.SourceItemSelectorBuilders
{
    /// <summary>
    /// Build source item list for gradient matrix 
    /// </summary>
    /// <seealso cref="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors.SourceItemSelectorBuilders.ISourceItemBuilder" />
    public class GradienColorMatrixSourceItemBuilder : ISourceItemBuilder
    {
        /// <summary>
        /// Matrix with 1 column raiting scale must have 2 columns including primary one
        /// </summary>
        private const int RequiredColumnCount = 2;

        /// <summary>
        /// The name of the matrix question type element
        /// </summary>
        private const string MatrixType = "matrix";

        /// <summary>
        /// The  column type to filter 
        /// </summary>
        private const string MatrixRatingScaleColumn = "RadioButtonScale";

        public List<LightweightItemMetaData> Build(int responseTemplateId, List<LightweightItemMetaData> items, List<int> selectedItems, List<string> allowedItemTypes, List<string> notAllowedItemTypes)
        {
            items = FilterOneRaitingColumnMatrices(items,responseTemplateId);

            //Build list of non-selected items
            return items
                .Where(data => !selectedItems.Contains(data.ItemId) && (allowedItemTypes == null || allowedItemTypes.Count == 0 || allowedItemTypes.Contains(data.ItemType))
                        && (notAllowedItemTypes == null || !notAllowedItemTypes.Contains(data.ItemType)))
                .ToList();
        }

        private List<LightweightItemMetaData>  FilterOneRaitingColumnMatrices(List<LightweightItemMetaData> items, int responseId)
        {
            List<LightweightItemMetaData> result = new List<LightweightItemMetaData>();

            items = items.Where(
                    item =>
                        item.ItemType.ToLower().Equals(MatrixType)).ToList();

            foreach (var item in items.ToList())
            {
                var itemData = ReportDataServiceImplementation.GetItem(UserManager.GetCurrentPrincipal(), responseId, item.ItemId, null,
                    false, false);

                var additionalData = itemData.AdditionalData as MatrixAdditionalData;

                if (additionalData != null && additionalData.Columns.Length == RequiredColumnCount &&
                    additionalData.Columns[1].ColumnType.Equals(MatrixRatingScaleColumn))
                {
                    result.Add(item);
                }
            }

            return result;
        }
    }
}