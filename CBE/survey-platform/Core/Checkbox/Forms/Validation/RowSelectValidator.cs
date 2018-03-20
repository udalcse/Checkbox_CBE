using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Checkbox.Forms.Items;
using Checkbox.Globalization.Text;

namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// Validator for row select column
    /// </summary>
    public class RowSelectValidator : Validator<MatrixItem>
    {
        protected String ErrorMessage { get; set; }

        private int ColumnNumber { get; set; }

        public RowSelectValidator(int columnNumber)
        {
            ColumnNumber = columnNumber;
        }

        /// <summary>
        /// Get the error's message text
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override string GetMessage(string languageCode)
        {
            return ErrorMessage;
        }

        /// <summary>
        /// Validate the 'selection' status.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool Validate(MatrixItem input)
        {
            int selectedRows = 0;

            //This item will be needed for determine if multiple selection is allowed 
            //or minSelectedOptions count, maxSelectedOptions count.
            RowSelect firstItem = null;

            for (int row = 1; row <= input.RowCount; row++)
            {
                var item = input.GetItemAt(row, ColumnNumber) as RowSelect;
                if (firstItem == null)
                    firstItem = item;
                selectedRows += item.Options.Where(p => p.IsSelected).Count();
            }

            if (firstItem.AllowMultipleSelection)
            {
                if (firstItem.MinToSelect.HasValue && selectedRows < firstItem.MinToSelect.Value)
                {
                    if (firstItem.MinToSelect.Value == 1)
                        ErrorMessage = TextManager.GetText("/validationMessages/rowSelect/minErrorSingular",
                                                           input.LanguageCode);
                    else
                        ErrorMessage =
                            TextManager.GetText("/validationMessages/rowSelect/minError", input.LanguageCode).Replace(
                                "{min}", firstItem.MinToSelect.ToString());

                    return false;
                }

                if (firstItem.MaxToSelect.HasValue && selectedRows > firstItem.MaxToSelect.Value)
                {
                    ErrorMessage =
                        TextManager.GetText("/validationMessages/rowSelect/maxError", input.LanguageCode).Replace(
                            "{max}", firstItem.MaxToSelect.ToString());
                    return false;
                }
            }
            else
            {
                if (firstItem.Required && selectedRows == 0)
                {
                    ErrorMessage = TextManager.GetText("/validationMessages/rowSelect/required", input.LanguageCode);
                    return false;
                }
            }

            return true;
        }
    }
}
