using Checkbox.Forms.Items.Configuration;

namespace Checkbox.Analytics.Filters.Validators
{
    interface IFilterValidator
    {
        bool Validate(ItemData itemData, string value, out string errorMessage);
    }
}
