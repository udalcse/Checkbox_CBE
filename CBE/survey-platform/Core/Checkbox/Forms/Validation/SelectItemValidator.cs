using System.Collections.Generic;
using System.Linq;
using System.Web;
using Checkbox.Common;
using Checkbox.Forms.Items;
using Checkbox.Globalization.Text;
using Checkbox.Security;
using Checkbox.Security.Principal;
using Checkbox.Users;

namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// Validator for select items
    /// </summary>
    public class SelectItemValidator : Validator<SelectItem>
    {
        /// <summary>
        /// Get/set text id for error message
        /// </summary>
        protected string ErrorMessage { get; set; }

        /// <summary>
        /// Get the error message text
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override string GetMessage(string languageCode)
        {
            return ErrorMessage;
        }

        /// <summary>
        /// Validate the select input's answer(s)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool Validate(SelectItem input)
        {
            ErrorMessage = string.Empty;

            if (!ValidateRequired(input))
                return false;

            List<ListOption> selectedOptions = input.SelectedOptions;
            foreach (ListOption option in selectedOptions)
            {
                if (option.IsOther && Utilities.IsNullOrEmpty(input.OtherText))
                {
                    ErrorMessage = TextManager.GetText("/validationMessages/selectOne/otherIsRequired", input.LanguageCode);
                    return false;
                }
            }
                
            return true;
        }

        /// <summary>
        /// Validate required status, i.e. that an item has been selected
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected virtual bool ValidateRequired(SelectItem input)
        {
            bool valid = !input.Required || (input.SelectedOptions.Count > 0);

            if (!valid)
            {
                if (HttpContext.Current.User is CheckboxPrincipal)
                {
                    var data = ProfileManager.GetCustomFieldDataByItemId(input.ID, ((CheckboxPrincipal)HttpContext.Current.User).Identity.Name);

                    if (data != null && data.Any())
                    {
                        return true;
                    }
                }

                ErrorMessage = TextManager.GetText("/validationMessages/regex/required", input.LanguageCode);
            }

            return valid;
        }
    }
}
