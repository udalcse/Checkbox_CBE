using System.Collections.Generic;
using Checkbox.Forms.Data;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors.SourceItemSelectorBuilders
{
    public interface  ISourceItemBuilder
    {
        List<LightweightItemMetaData> Build(int responseTemplateId, List<LightweightItemMetaData> items,
            List<int> selectedItems, List<string> allowedItemTypes, List<string> notAllowedItemTypes);
    }
}