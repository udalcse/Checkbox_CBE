using System;
using System.Collections.Generic;
using System.Linq;
using Checkbox.Forms.Items;
using Checkbox.Globalization.Text;

namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// Validator for matrix answers
    /// </summary>
    public class MatrixItemValidator : Validator<MatrixItem>
    {
        private Dictionary<int, List<string>> _columnValidationErrors;

        /// <summary>
        /// Get the validation error collection
        /// </summary>
        public Dictionary<int, List<string>> ColumnValidationErrors
        {
            get
            {
                if (_columnValidationErrors == null)
                {
                    _columnValidationErrors = new Dictionary<int, List<string>>();
                }

                return _columnValidationErrors;
            }
        }

        /// <summary>
        /// Add a validation error for a column
        /// </summary>
        /// <param name="column"></param>
        /// <param name="error"></param>
        private void AddColumnValidationError(int column, string error)
        {
            if (!ColumnValidationErrors.ContainsKey(column))
            {
                ColumnValidationErrors[column] = new List<string>();    
            }
            
            ColumnValidationErrors[column].Add(error);
        }

        /// <summary>
        /// Get the current matrix-wide error message
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override string GetMessage(string languageCode)
        {
            return TextManager.GetText("/validationMessages/matrix/validationError", languageCode);
        }

        /// <summary>
        /// Validate the matrix item
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool Validate(MatrixItem input)
        {
            bool valid = true;

            for (int column = 1; column <= input.ColumnCount; column++)
            {
                valid = valid & ValidateColumn(input, column);
            }

            return valid;
        }

        /// <summary>
        /// Validate the matrix column
        /// </summary>
        /// <param name="matrixItem"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        protected virtual bool ValidateColumn(MatrixItem matrixItem, int column)
        {
            bool valid = true;

            MatrixColumnInfo columnInfo = matrixItem.GetColumnInfo(column);
            string columnTypeName = columnInfo.PrototypeItemTypeName;

            //Validate columns have unique answers, if specified.
            if (columnInfo.RequireUniqueAnswers)
            {
                valid = ValidateUniqueAnswers(matrixItem, column);
            }

            //Check matrix sum total value in addition to rules on each item in the sum total column
            if (columnTypeName != null && columnTypeName.Equals("MatrixSumTotal", StringComparison.InvariantCultureIgnoreCase))
            {
                valid = valid & ValidateSumTotalColumn(matrixItem, column);
            }

            if (columnTypeName != null && columnTypeName.Equals("RowSelector", StringComparison.InvariantCultureIgnoreCase))
            {
                valid = valid & ValidateRowSelectorColumn(matrixItem, column);
            }

            //Now validate items in the column
            for (int row = 1; row <= matrixItem.RowCount; row++)
            {
                MatrixRowInfo rowInfo = matrixItem.GetRowInfo(row);

                //Don't validate subheading rows and excluded rows
                if (rowInfo.RowType != RowType.Subheading && !matrixItem.IsRowExcluded(row))
                {
                    valid = valid & ValidateItem(matrixItem, column, row);
                }
            }

            return valid;
        }



        /// <summary>
        /// Validate the item in the matrix
        /// </summary>
        /// <param name="matrixItem"></param>
        /// <param name="column"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        protected virtual bool ValidateItem(MatrixItem matrixItem, int column, int row)
        {
            AnswerableItem item = matrixItem.GetItemAt(row, column) as AnswerableItem;

            if (item != null)
            {
                if (item.Excluded)
                {
                    return true;
                }
                
                if (matrixItem.GetRowInfo(row).RowType == RowType.Other)
                {
                    //Handle "other" rows differently since they are optional
                    return ValidateOtherItem(matrixItem, item, column, row);
                }
                
                item.Validate();

                if (!item.Valid)
                {
                    ColumnValidationErrors[column] = item.ValidationErrors;
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Validate an item in an "other" row.
        /// </summary>
        /// <param name="matrixItem"></param>
        /// <param name="otherItem"></param>
        /// <param name="column"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        protected virtual bool ValidateOtherItem(MatrixItem matrixItem, AnswerableItem otherItem, int column, int row)
        {
            Item pkItem = matrixItem.GetItemAt(row, matrixItem.PrimaryColumnIndex);

            //If the column is the primary column, then the input is the "other" text entry field for the row.  This
            // value is required if any other items in the row have been answered
            if (column == matrixItem.PrimaryColumnIndex)
            {
                if (pkItem != null && pkItem is AnswerableItem)
                {
                    bool rowHasAnswers = false;

                    for (int i = 1; i < matrixItem.ColumnCount; i++)
                    {
                        if (i != column)
                        {
                            Item item = matrixItem.GetItemAt(row, i);

                            if (item != null && item is AnswerableItem && ((AnswerableItem)item).HasAnswer)
                            {
                                rowHasAnswers = true;
                                break;
                            }
                        }
                    }

                    //Row answers
                    if (rowHasAnswers)
                    {
                        //Item is valid if it has been answered
                        RequiredItemValidator validator = new RequiredItemValidator();
                        
                        if(!validator.Validate((AnswerableItem)pkItem))
                        {
                            List<string> tempList = new List<string>();
                            tempList.Add(validator.GetMessage(matrixItem.LanguageCode));

                            ColumnValidationErrors[column] = tempList;

                            return false;
                        }
                        
                        return true;
                    }
                }
            }
            else
            {
                //Other items in the "other" row follow their validation rules only if the "other" text item has been answered
                if (pkItem is AnswerableItem && ((AnswerableItem)pkItem).HasAnswer)
                {
                    Item item = matrixItem.GetItemAt(row, column);

                    if (item != null && item is AnswerableItem)
                    {
                        ((AnswerableItem)item).Validate();

                        if(!((AnswerableItem)item).Valid)
                        {
                            ColumnValidationErrors[column] = ((AnswerableItem)item).ValidationErrors;
                            return false;
                        }
                    }
                }
            }

            //Return true by default
            return true;
        }

        /// <summary>
        /// Validate row selector column
        /// </summary>
        /// <param name="matrixItem"></param>
        /// <param name="columnNumber"></param>
        /// <returns></returns>
        protected virtual bool ValidateRowSelectorColumn(MatrixItem matrixItem, int columnNumber)
        {
            RowSelectValidator validator = new RowSelectValidator(columnNumber);

            bool isValid = validator.Validate(matrixItem);
            
            if (!isValid)
            {
                AddColumnValidationError(columnNumber, validator.GetMessage(matrixItem.LanguageCode));
            }

            return isValid;
        }

        /// <summary>
        /// Validate a sum total column
        /// </summary>
        /// <param name="matrixItem"></param>
        /// <param name="columnNumber"></param>
        /// <returns></returns>
        protected virtual bool ValidateSumTotalColumn(MatrixItem matrixItem, int columnNumber)
        {
            if (matrixItem.RowCount == 0)            
                return true;
           
            //Get matrix item
            MatrixSumTotalItem sumTotalItem = matrixItem.GetItemAt(1, columnNumber) as MatrixSumTotalItem;

            if (sumTotalItem == null)
            {
                return true;
            }

            ItemSumValidator validator = new ItemSumValidator(sumTotalItem.TotalValue, sumTotalItem.ComparisonOperator);

            //If not valid, set the message and return false
            if (!validator.Validate(GetAnswerableColumnItems(matrixItem, columnNumber)))
            {
                AddColumnValidationError(columnNumber, validator.GetMessage(matrixItem.LanguageCode));
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validate column answers are unique
        /// </summary>
        /// <param name="matrixItem"></param>
        /// <param name="columnNumber"></param>
        /// <returns></returns>
        protected virtual bool ValidateUniqueAnswers(MatrixItem matrixItem, int columnNumber)
        {
            ItemAnswerUniqueValidator validator = new ItemAnswerUniqueValidator();

            if (!validator.Validate(GetAnswerableColumnItems(matrixItem, columnNumber)))
            {
                AddColumnValidationError(columnNumber, validator.GetMessage(matrixItem.LanguageCode));
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get answerable items in a column
        /// </summary>
        /// <param name="matrixItem"></param>
        /// <param name="columnNumber"></param>
        /// <returns></returns>
        protected virtual List<IAnswerable> GetAnswerableColumnItems(MatrixItem matrixItem, int columnNumber)
        {
            List<IAnswerable> items = new List<IAnswerable>();
            int rowCount = matrixItem.RowCount;

            for (int row = 1; row <= rowCount; row++)
            {
                Item childItem = matrixItem.GetItemAt(row, columnNumber);

                if (childItem != null && childItem is IAnswerable)
                {
                    items.Add((IAnswerable)childItem);
                }
            }

            return items;
        }
    }
}
