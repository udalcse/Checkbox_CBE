using System;
using System.Data;
using Prezza.Framework.Data;
using Prezza.Framework.ExceptionHandling;
using Checkbox.LicenseLibrary;

namespace Checkbox.Management.Licensing.Limits
{
    /// <summary>
    /// Limit based on values.
    /// </summary>
    public abstract class ValueLimit<T> : LicenseLimit
    {
        /// <summary>
        /// Value of the limit as stored in the license file.
        /// </summary>
        public abstract T LicenseFileLimitValue { get; }

        /// <summary>
        /// Get the currently in-effect limit value.  This is useful is certain
        /// limits need to override the license-specified limit such as when limit
        /// value should be stored in the database, etc.
        /// </summary>
        public virtual T RuntimeLimitValue
        {
            get
            {
                //First look up the limit in the DB.                
                T limit = default(T);
                if (ApplicationManager.AppSettings.EnableMultiDatabase)
                {
                    limit = GetLimitValueFromMasterDb<T>(LimitName);
                }

                //Then if the DB doesn't have any information about it, look at the license file
                if (limit == null)
                    limit = LicenseFileLimitValue;

                return limit;
            }

        }

        /// <summary>
        /// Get a boolean indicating if the limit is valid when no limit value is
        /// found in the application license.  By default, this value is true.
        /// </summary>
        protected virtual bool ValidIfLimitNull
        {
            get { return true; }
        }

        /// <summary>
        /// Validate the limit against application data.
        /// </summary>
        /// <returns></returns>
        public override LimitValidationResult Validate(out string message)
        {
            //Save a little work if the limit has no value and that indicates a valid
            //limit.
            if (RuntimeLimitValue == null && ValidIfLimitNull)
            {
                message = string.Empty;
                return LimitValidationResult.LimitNotReached;
            }

            return ProtectedValidate(out message);
        }

        /// <summary>
        /// Overridable validation method
        /// </summary>
        /// <returns></returns>
        public abstract LimitValidationResult ProtectedValidate(out string message);

        /// <summary>
        /// 
        /// </summary>
        protected virtual string ValueColumnName
        {
            get { return "LimitValue"; }
        }

        /// <summary>
        /// Get a limit value from the database.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="limitName"></param>
        /// <returns></returns>
        protected V GetLimitValueFromMasterDb<V>(string limitName)
        {
            V limitValue = default(V);

            try
            {
                Database db = DatabaseFactory.CreateDatabase(DatabaseFactory.MASTER_DB_NAME);
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_GetLicenseLimitValue");
                command.AddInParameter("ContextName", DbType.String, DatabaseFactory.CurrentDataContext ?? string.Empty);
                command.AddInParameter("LimitName", DbType.String, limitName);

                using (IDataReader reader = db.ExecuteReader(command))
                {
                    try
                    {
                        if (reader.Read())
                        {
                            limitValue = DbUtility.GetValueFromDataReader(reader, ValueColumnName, default(V));

                            //if V is boolean, value could be stored not as true/false, but also as 1/0
                            if (limitValue == null && (typeof(V) == typeof(bool) || typeof(V) == typeof(bool?)))
                            {
                                int? val = DbUtility.GetValueFromDataReader(reader, ValueColumnName, default(int?));
                                if (val != null)
                                    limitValue = (V)(object)(val > 0);
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

            return limitValue;

        }
    }
}
