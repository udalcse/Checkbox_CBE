using System;
using System.Collections.Generic;
using System.Data;

using Checkbox.Common;
using Checkbox.Globalization.Text;

using Prezza.Framework.Data;
using Prezza.Framework.ExceptionHandling;

namespace Checkbox.Forms.Validation
{
    public class CustomFormatValidator : RegularExpressionValidator
    {
        /// <summary>
        /// Uniquely identifies the custom validator that is being used
        /// </summary>
        private string _formatId;

        public CustomFormatValidator(string format)
        {
            _formatId = format;
            _regex = GetRegularExpression(format);
        }

        /// <summary>
        /// Validate the input string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool Validate(string input)
        {
            if (Utilities.IsNullOrEmpty(_regex))
                return true;

            try
            {
                return RegularExpression.IsMatch(input.Normalize());
            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessPublic");

                if (rethrow)
                {
                    throw;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Get the validation error message.
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override string GetMessage(string languageCode)
        {
            return TextManager.GetText(string.Format("/validationMessages{0}", _formatId), languageCode);
        }

        /// <summary>
        /// Returns the set of configured custom validator. Validators consist of a plain text description
        /// and uniquely identifier.
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetFormats(string languageCode)
        {
            Dictionary<string, string> formatNames = new Dictionary<string, string>();
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_CustomValidators_GetFormats");
            DataSet ds = db.ExecuteDataSet(command);

            if (ds.Tables.Count == 1 && ds.Tables[0].Rows != null)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    string formatId = DbUtility.GetValueFromDataRow(row, "FormatId", string.Empty);

                    if (Utilities.IsNotNullOrEmpty(formatId))
                    {
                        string formatName = TextManager.GetText(string.Format("{0}/Name", formatId), languageCode);
                        
                        if (Utilities.IsNotNullOrEmpty(formatName))
                        {
                            formatNames.Add(formatId, formatName);
                        }
                    }
                }
            }

            return formatNames;
        }

        /// <summary>
        /// Retrieves the regular expression associated with a specified custom validator.
        /// </summary>
        /// <param name="format">The unique identifier for a custom format.</param>
        /// <returns></returns>
        private string GetRegularExpression(string format)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_CustomValidators_GetExpression");
            command.AddInParameter("FormatId", DbType.String, format);
            object result = db.ExecuteScalar(command);

            if (result != null)
                return (string)result;

            return string.Empty;
        }
    }
}
