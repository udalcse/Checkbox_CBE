using System;
using System.Collections.Generic;
using System.Data;
using Checkbox.Common;
using Prezza.Framework.Data;

namespace Checkbox.Forms
{
    /// <summary>
    /// Template data for response templates
    /// </summary>
    public class ResponseTemplateDataSet : TemplateDataSet
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="owningObjectTypeName"></param>
        public ResponseTemplateDataSet(string owningObjectTypeName)
            : base(owningObjectTypeName)
        {
        }

        /// <summary>
        /// Set identity column
        /// </summary>
        public override string IdentityColumnName { get { return "ResponseTemplateId"; } }

        /// <summary>
        /// Get names of data tables for this object's configuration data set.
        /// </summary>
        public override List<string> ObjectDataTableNames
        {
            get
            {
                var tableNames = new List<string>();

                Database db = DatabaseFactory.CreateDatabase();
                DBCommandWrapper dbNameCommand = db.GetStoredProcCommandWrapper("ckbx_sp_RT_GetTableNames");
                using (IDataReader reader = db.ExecuteReader(dbNameCommand))
                {
                    try
                    {
                        while (reader.Read())
                        {
                            string tableName = DbUtility.GetValueFromDataReader(reader, "TableName", string.Empty);

                            if (Utilities.IsNotNullOrEmpty(tableName))
                            {
                                tableNames.Add(tableName);
                            }
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }
                }

                return tableNames;
            }
        }

        /// <summary>
        /// Initialize data
        /// </summary>
        protected override void InitializeDataTables()
        {
            base.InitializeDataTables();

            var responsePipeTable = new DataTable();
            responsePipeTable.Columns.Add("ResponseTemplateID", typeof(Int32));
            responsePipeTable.Columns.Add("PipeName", typeof(string));
            responsePipeTable.Columns.Add("ItemID", typeof(Int32));

            responsePipeTable.TableName = "ResponsePipes";

            Tables.Add(responsePipeTable);

            var responseTermTable = new DataTable();
            responseTermTable.Columns.Add("Id", typeof(Int32));
            responseTermTable.Columns.Add("Name", typeof(string));
            responseTermTable.Columns.Add("Term", typeof(string));
            responseTermTable.Columns.Add("Definition", typeof(string));
            responseTermTable.Columns.Add("Link", typeof(string));

            responseTermTable.TableName = "ResponseTerms";

            Tables.Add(responseTermTable);
        }

		/// <summary>
		/// Initialize the template data table
		/// </summary>
		protected override void InitializeTemplateData()
		{
			var templateDataTable = new DataTable { TableName = DataTableName };
			templateDataTable.Columns.Add(IdentityColumnName, typeof(Int32));
			templateDataTable.Columns.Add("ModifiedDate", typeof(DateTime));
			templateDataTable.Columns.Add("Deleted", typeof(bool));
			templateDataTable.Columns.Add("DefaultPolicy", typeof(Int32));
			templateDataTable.Columns.Add("AclID", typeof(Int32));
			templateDataTable.Columns.Add("CreatedDate", typeof(DateTime));
			templateDataTable.Columns.Add("CreatedBy", typeof(string));
			templateDataTable.Columns.Add(base.IdentityColumnName, typeof(Int32));
			templateDataTable.Columns.Add("TemplateName", typeof(string));

			templateDataTable.Columns.Add("ActivationStart", typeof( DateTime));
			templateDataTable.Columns.Add("ActivationEnd", typeof( DateTime));
			templateDataTable.Columns.Add("MaxTotalResponses", typeof(int));
			templateDataTable.Columns.Add("MaxResponsesPerUser", typeof(int));
			templateDataTable.Columns.Add("AllowContinue", typeof(bool));
			templateDataTable.Columns.Add("AllowEdit", typeof(bool));
			templateDataTable.Columns.Add("MobileCompatible", typeof(bool));
            templateDataTable.Columns.Add("ShowAsterisks", typeof(bool));
			templateDataTable.Columns.Add("ShowItemNumbers", typeof(bool));
			templateDataTable.Columns.Add("ShowPageNumbers", typeof(bool));
			templateDataTable.Columns.Add("EnableDynamicPageNumbers", typeof(bool));
			templateDataTable.Columns.Add("EnableDynamicItemNumbers", typeof(bool));
			templateDataTable.Columns.Add("ShowProgressBar", typeof(bool));
            templateDataTable.Columns.Add("ProgressBarOrientation", typeof(Int32));
            templateDataTable.Columns.Add("ShowTitle", typeof(bool));
			templateDataTable.Columns.Add("DefaultLanguage", typeof(string));
			templateDataTable.Columns.Add("SupportedLanguages", typeof(string));
			templateDataTable.Columns.Add("LanguageSource", typeof(string));
			templateDataTable.Columns.Add("LanguageSourceToken", typeof(string));
			templateDataTable.Columns.Add("EnableScoring", typeof(bool));
			templateDataTable.Columns.Add("RandomizeItemsInPages", typeof(bool));
			templateDataTable.Columns.Add("ShowValidationMessage", typeof(bool));
			templateDataTable.Columns.Add("RequiredFieldsAlert", typeof(bool));
			templateDataTable.Columns.Add("CompletionType", typeof(int));
			templateDataTable.Columns.Add("DisableBackButton", typeof(bool));
			templateDataTable.Columns.Add("IsPoll", typeof(bool));
			templateDataTable.Columns.Add("ChartStyleID", typeof(int));
			templateDataTable.Columns.Add("Height", typeof(int));
			templateDataTable.Columns.Add("Width", typeof(int));
			templateDataTable.Columns.Add("BorderWidth", typeof(int));
			templateDataTable.Columns.Add("BorderColor", typeof(string));
			templateDataTable.Columns.Add("BorderStyle", typeof(string));
			templateDataTable.Columns.Add("AllowSurveyEditWhileActive", typeof(bool));

            templateDataTable.Columns.Add("DisplayPDFDownloadButton", typeof(bool));

            templateDataTable.Columns.Add("ShowSaveAndQuit", typeof(bool));
			templateDataTable.Columns.Add("GuestPassword", typeof(string));
            templateDataTable.Columns.Add("GoogleAnalyticsTrackingID", typeof(string));

            Tables.Add(templateDataTable);
		}
    }
}
