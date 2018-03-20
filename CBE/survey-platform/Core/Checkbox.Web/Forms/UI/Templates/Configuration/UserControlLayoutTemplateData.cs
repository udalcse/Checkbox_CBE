using System;
using System.IO;
using System.Web;
using System.Data;
using System.Web.UI;
using Checkbox.Common;
using Checkbox.Management;
using Checkbox.Forms.PageLayout.Configuration;

using Prezza.Framework.Data;

namespace Checkbox.Web.Forms.UI.Templates.Configuration
{
    ///<summary>
    ///</summary>
    [Serializable]
    public class UserControlLayoutTemplateData : PageLayoutTemplateData
    {
        /// <summary>
        /// Get name of load procedure for item
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_LayoutTemplate_UserControl_Get"; } }
     
        /// <summary>
        /// Get the type name for the item
        /// </summary>
        public override string TypeName
        {
            get { return "UserControl"; }
        }

        /// <summary>
        /// Get/set the control source for this user control
        /// </summary>
        public string ControlSource { get; set; }

        protected override PersistedDomainObjectDataSet  CreateConfigurationDataSet()
        {
            return new PersistedDomainObjectDataSet(ObjectTypeName, "LayoutTemplateData", "LayoutTemplateId", "ItemZoneMappings");
        }

        /// <summary>
        /// Create the layout template
        /// </summary>
        /// <param name="languageCode">Language code to use for ML controls in the template.</param>
        /// <returns></returns>
        public override object CreateTemplate(string languageCode)
        {
            if (HttpContext.Current != null && HttpContext.Current.Server != null)
            {
                string controlPath = ApplicationManager.ApplicationRoot + "/Forms/Surveys/LayoutTemplates/" + ControlSource;

                if (File.Exists(HttpContext.Current.Server.MapPath(controlPath)))
                {
                    UserControl c = new UserControl();
                    UserControlLayoutTemplate template = c.LoadControl(controlPath) as UserControlLayoutTemplate;

                    if (template != null)
                    {
                        template.Initialize(this, languageCode);
                        return template;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Load the data from the data row
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadBaseObjectData(DataRow data)
        {
            base.LoadBaseObjectData(data);

            ControlSource = DbUtility.GetValueFromDataRow(data, "TemplateControlSource", string.Empty);
        }

        /// <summary>
        /// Create a new instance of the template in the database
        /// </summary>
        /// <param name="t"></param>
        protected override void Create(IDbTransaction t)
        {
            base.Create(t);

            UpSert(t);
        }

        /// <summary>
        /// Update an instance of the template in the database
        /// </summary>
        /// <param name="t"></param>
        protected override void Update(IDbTransaction t)
        {
            base.Update(t);

            UpSert(t);
        }

        /// <summary>
        /// Do the work
        /// </summary>
        /// <param name="t"></param>
        protected virtual void UpSert(IDbTransaction t)
        {
            Database db = DatabaseFactory.CreateDatabase();

            DBCommandWrapper upsertCommand = GetUpSertCommand(db);

            db.ExecuteNonQuery(upsertCommand, t);

            SaveMappings(t);
        }

        /// <summary>
        /// Delete the user control template
        /// </summary>
        /// <param name="t"></param>
        public override void Delete(IDbTransaction t)
        {
            base.Delete(t);

            Database db = DatabaseFactory.CreateDatabase();

            DBCommandWrapper deleteCommand = GetDeleteCommand(db);

            db.ExecuteNonQuery(deleteCommand, t);
        }

        /// <summary>
        /// Get the db command to load the item
        /// </summary>
        /// <returns></returns>
        protected virtual DBCommandWrapper GetLoadCommand(Database db)
        {
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_LayoutTemplate_UserControl_Get");
            command.AddInParameter("LayoutTemplateID", DbType.Int32, ID);

            return command;
        }

        /// <summary>
        /// Get the upsert command
        /// </summary>
        /// <returns></returns>
        protected virtual DBCommandWrapper GetUpSertCommand(Database db)
        {
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_LayoutTemplate_UserControl_UpSert");

            command.AddInParameter("LayoutTemplateID", DbType.Int32, ID);
            command.AddInParameter("TemplateControlSource", DbType.String, ControlSource);

            return command;
        }

        /// <summary>
        /// Get the command to delete
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        protected virtual DBCommandWrapper GetDeleteCommand(Database db)
        {
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_LayoutTemplate_UserControl_Delete");
            command.AddInParameter("LayoutTemplateID", DbType.Int32, ID);

            return command;
        }
    }
}
