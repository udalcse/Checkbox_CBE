using System;
using System.Data;
using Checkbox.Globalization.Text;
using Prezza.Framework.Data;
using Prezza.Framework.ExceptionHandling;
using Checkbox.LicenseLibrary;

namespace Checkbox.Management.Licensing.Limits
{
    /// <summary>
    /// Controls how namy emails are available to send.
    /// </summary>
    public class EmailLimit : DecrementLicenseLimit<long?>
    {
        public override string LimitName
        {
            get { return "EmailLimit"; }
        }

        /// <summary>
        /// Validate the limit
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public override LimitValidationResult ProtectedValidate(out string message)
        {
            long? currentValue = GetCurrentValue();
            if (!currentValue.HasValue)
            {
                message = TextManager.GetText("/emailsLimit/unableToGetCurrentValue") ??
                          "Unable to get current value of Emails Limit.";
                return LimitValidationResult.UnableToEvaluate;
            }

            if (currentValue.Value < 0)
            {
                message = TextManager.GetText("/emailsLimit/limitExceeded") ?? @"Current Value of Emails Limit is less than ""0"". So no more emails will be allowed until the resetting.";
                return LimitValidationResult.LimitExceeded;
            }

            if (currentValue.Value == 0)
            {
                message = TextManager.GetText("/emailsLimit/limitReached") ?? @"Current Value of Emails Limit is ""0"". So no more emails will be allowed until the resetting.";
                return LimitValidationResult.LimitReached;
            }

            message = string.Empty;
            return LimitValidationResult.LimitNotReached;
        }

        /// <summary>
        /// Get the current value of the decrement limit
        /// </summary>
        /// <returns></returns>
        public override long? GetCurrentValue()
        {
            //Limits only apply to CheckboxOnline
            if (!ApplicationManager.AppSettings.EnableMultiDatabase)
            {
                return null;
            }
                
            long? currentValue = null;
            try
            {
                Database db = DatabaseFactory.CreateDatabase(DatabaseFactory.MASTER_DB_NAME);
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_GetLicenseLimitValue");
                command.AddInParameter("ContextName", DbType.String, DatabaseFactory.CurrentDataContext ?? string.Empty);
                command.AddInParameter("LimitName", DbType.String, LimitName);

                using (IDataReader reader = db.ExecuteReader(command))
                {
                    try
                    {
                        if (reader.Read())
                        {
                            long temp;
                            if (
                                long.TryParse(
                                    DbUtility.GetValueFromDataReader<String>(reader, "ContextLimitValue", null),
                                    out temp))
                            {
                                currentValue = temp;
                            }
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                //Ignore error, but log it
                ExceptionPolicy.HandleException(ex, "BusinessProtected");
            }

            return currentValue;
        }

        /// <summary>
        /// Get the base value of a decrement limit.
        /// </summary>
        /// <returns></returns>
        public override long? GetBaseValue()
        {
            //Limits only apply to CheckboxOnline
            if (!ApplicationManager.AppSettings.EnableMultiDatabase)
            {
                return null;
            }

            long? baseValue = null;
            try
            {
                Database db = DatabaseFactory.CreateDatabase(DatabaseFactory.MASTER_DB_NAME);
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_GetLicenseLimitValue");
                command.AddInParameter("ContextName", DbType.String, DatabaseFactory.CurrentDataContext ?? string.Empty);
                command.AddInParameter("LimitName", DbType.String, LimitName);

                using (IDataReader reader = db.ExecuteReader(command))
                {
                    try
                    {
                        if (reader.Read())
                        {
                            long temp;
                            if (
                                long.TryParse(
                                    DbUtility.GetValueFromDataReader<String>(reader, "ContextLimitBase", null),
                                    out temp))
                            {
                                baseValue = temp;
                            }
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                //Ignore error, but log it
                ExceptionPolicy.HandleException(ex, "BusinessProtected");
            }

            return baseValue;
        }


        /// <summary>
        /// Decrement the current value
        /// </summary>
        public override void Decrement()
        {
            try
            {
                long? currentValue = GetCurrentValue();

                if (currentValue.HasValue) //If current value isn't specified, miss this limit.
                {
                    Database db = DatabaseFactory.CreateDatabase(DatabaseFactory.MASTER_DB_NAME);
                    DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_UpSertLicenseLimitValueTest");
                    command.AddInParameter("ContextName", DbType.String,
                                           DatabaseFactory.CurrentDataContext ?? string.Empty);
                    command.AddInParameter("LimitName", DbType.String, LimitName);
                    command.AddInParameter("LimitValue", DbType.String, currentValue - 1);
                    command.AddInParameter("PlanID", DbType.Int32, null);
                    db.ExecuteNonQuery(command);
                }
            }
            catch (Exception ex)
            {
                //Ignore error, but log it
                ExceptionPolicy.HandleException(ex, "BusinessProtected");
            }
        }
    }
}
