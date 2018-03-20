using System;
using System.Data;
using Prezza.Framework.Data;
using Prezza.Framework.Data.Sprocs;
using Checkbox.Globalization.Text;

namespace Checkbox.Forms.Periods
{
    /// <summary>
    /// Survey period class
    /// </summary>
    [FetchProcedure("ckbx_SurveyPeriod_Get")]
    [InsertProcedure("ckbx_SurveyPeriod_Insert")]
    [UpdateProcedure("ckbx_SurveyPeriod_Update")]
    [DeleteProcedure("ckbx_SurveyPeriod_Delete")]
    public class SurveyPeriod
    {
        /// <summary>
        /// Survey period ID
        /// </summary>
        [FetchParameter(Name = "SurveyPeriodID", DbType = DbType.Int32, Direction = ParameterDirection.ReturnValue)]
        [UpdateParameter(Name = "SurveyPeriodID", ConvertDBNullToNull = true, DbType = DbType.Int32, Direction=ParameterDirection.Input)]
        [InsertParameter(Name = "SurveyPeriodID", ConvertDBNullToNull = true, DbType = DbType.Int32, Direction = ParameterDirection.Output)]
        [DeleteParameter(Name = "SurveyPeriodID", ConvertDBNullToNull = true, DbType = DbType.Int32, Direction = ParameterDirection.Input)]
        public int? SurveyPeriodID { get; set; }

        /// <summary>
        /// Date when the period starts
        /// </summary>
        [FetchParameter(Name = "Start", DbType = DbType.DateTime, Direction = ParameterDirection.ReturnValue, AllowNull = true)]
        [UpdateParameter(Name = "Start", DbType = DbType.DateTime, Direction = ParameterDirection.Input, AllowNull = true)]
        [InsertParameter(Name = "Start", DbType = DbType.DateTime, Direction = ParameterDirection.Input, AllowNull = true)]
        public DateTime? Start { get; set; }

        /// <summary>
        /// Date when the period finishes
        /// </summary>
        [FetchParameter(Name = "Finish", DbType = DbType.DateTime, Direction = ParameterDirection.ReturnValue, AllowNull = true)]
        [UpdateParameter(Name = "Finish", DbType = DbType.DateTime, Direction = ParameterDirection.Input, AllowNull = true)]
        [InsertParameter(Name = "Finish", DbType = DbType.DateTime, Direction = ParameterDirection.Input, AllowNull = true)]
        public DateTime? Finish { get; set; }

        /// <summary>
        /// Period name
        /// </summary>
        public string PeriodName { get; set; }
        /// <summary>
        /// Period Name Text ID
        /// </summary>
        [FetchParameter(Name = "PeriodName", DbType = DbType.String, Direction = ParameterDirection.ReturnValue)]
        [UpdateParameter(Name = "PeriodName", DbType = DbType.String, Direction = ParameterDirection.Input)]
        [InsertParameter(Name = "PeriodName", DbType = DbType.String, Direction = ParameterDirection.Input)]
        public string PeriodNameTextID { get;  set; }

        /// <summary>
        /// ID of the parent survey
        /// </summary>
        [FetchParameter(Name = "ResponseTemplateID", DbType = DbType.Int32, Direction = ParameterDirection.ReturnValue)]
        [InsertParameter(Name = "ResponseTemplateID", DbType = DbType.Int32, Direction = ParameterDirection.Input)]
        public int ResponseTemplateID { get; set; }


        ResponseTemplate _responseTemplate = null;
        /// <summary>
        /// Parent Response Template
        /// </summary>
        public ResponseTemplate ResponseTemplate
        {
            get
            {
                if (_responseTemplate == null)
                    _responseTemplate = ResponseTemplateManager.GetResponseTemplate(ResponseTemplateID);
                return _responseTemplate;
            }
        }

        /// <summary>
        /// Save the item
        /// </summary>
        public void Save()
        {
            if (SurveyPeriodID.HasValue)
            {
                //update period data
                StoredProcedureCommandExtractor.ExecuteProcedure(ProcedureType.Update, this);
                //update name
                TextManager.SetText(PeriodNameTextID, ResponseTemplate.LanguageSettings.DefaultLanguage, PeriodName);
            }
            else
            {
                //Make name text id empty
                PeriodNameTextID = "";
                //create new period
                StoredProcedureCommandExtractor.ExecuteProcedure(ProcedureType.Insert, this);
                PeriodNameTextID = "/templateData/period/" + SurveyPeriodID.ToString() + "/name";
                //save TextID
                StoredProcedureCommandExtractor.ExecuteProcedure(ProcedureType.Update, this);
                //save name
                TextManager.SetText(PeriodNameTextID, ResponseTemplate.LanguageSettings.DefaultLanguage, PeriodName);
            }            
        }

        /// <summary>
        /// Delete the item
        /// </summary>
        public void Delete()
        {
            StoredProcedureCommandExtractor.ExecuteProcedure(ProcedureType.Delete, this);
        }

        /// <summary>
        /// Load the item
        /// </summary>
        public void Load()
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command;

            command = db.GetStoredProcCommandWrapper("ckbx_SurveyPeriod_Get");
            command.AddInParameter("SurveyPeriodID", DbType.Int32, SurveyPeriodID);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    reader.Read();
                    
                    StoredProcedureCommandExtractor.PopulateObjectFromReaderReturnValues(reader, this);
                    PeriodName = TextManager.GetText(PeriodNameTextID, ResponseTemplate.LanguageSettings.DefaultLanguage);
                    
                }
                finally
                {
                    reader.Close();
                }
            }
        }    
    }
}
