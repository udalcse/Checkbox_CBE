using System;
using System.Data;
using System.Xml;
using Checkbox.Management;
using Prezza.Framework.Common;
using Prezza.Framework.Data;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Data class for display analysis option
    /// </summary>
    [Serializable]
    public class DisplayAnalysisItemData : RedirectItemData
    {
        /// <summary>
        /// Get data table name
        /// </summary>
        public override string ItemDataTableName { get { return "DisplayAnalysisItemData"; } }

        /// <summary>
        /// Get load data sproc
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_ItemData_GetDisplayAnalysis"; } }

        /// <summary>
        /// Get/set the id of the analysis to display
        /// </summary>
        public Guid AnalysisGUID { get; set; }

        /// <summary>
        /// Always return false
        /// </summary>
        public override bool RestartSurvey
        {
            get { return false; }
            set { }
        }

        /// <summary>
        /// Get the URL to redirect to
        /// </summary>
        public override string URL
        {
            get { return ApplicationManager.ApplicationPath + "/RunAnalysis.aspx?ag=" + AnalysisGUID; }
            set { }
        }

        /// <summary>
        /// Show in a new browser tab
        /// </summary>
        public bool ShowInNewTab { get; set; }

        /// <summary>
        /// Load the item data
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadBaseObjectData(DataRow data)
        {
            base.LoadBaseObjectData(data);

            if (data["AnalysisGUID"] != DBNull.Value)
            {
                AnalysisGUID = new Guid(data["AnalysisGUID"].ToString());
            }
            ShowInNewTab = DbUtility.GetValueFromDataRow(data, "ShowInNewTab", false);
        }
    

        /// <summary>
        /// Create the data in the database
        /// </summary>
        /// <param name="t"></param>
        protected override void Create(IDbTransaction t)
        {
            base.Create(t);

            if (ID == null || ID <= 0)
            {
                throw new Exception("Unable to create display analysis item.  DataID was not specified.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertDA");
            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("AnalysisGUID", DbType.Guid, AnalysisGUID);
            command.AddInParameter("ShowInNewTab", DbType.Boolean, ShowInNewTab);

            db.ExecuteNonQuery(command, t);
        }

        /// <summary>
        /// Update the data in the database
        /// </summary>
        /// <param name="t"></param>
        protected override void Update(IDbTransaction t)
        {
            base.Update(t);

            if (ID == null || ID <= 0)
            {
                throw new Exception("Unable to update display analysis item.  DataID was not specified.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateDA");
            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("AnalysisGUID", DbType.Guid, AnalysisGUID);
            command.AddInParameter("ShowInNewTab", DbType.Boolean, ShowInNewTab);

            db.ExecuteNonQuery(command, t);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteItemTypeSpecificXml(System.Xml.XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
        {
            base.WriteItemTypeSpecificXml(writer, externalDataCallback);

			writer.WriteElementString("AnalysisGUID", AnalysisGUID.ToString());
            writer.WriteElementString("ShowInNewTab", ShowInNewTab.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        protected override void ReadItemTypeSpecificXml(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
        {
            base.ReadItemTypeSpecificXml(xmlNode, callback, creator);

            ShowInNewTab = XmlUtility.GetNodeBool(xmlNode.SelectSingleNode("ShowInNewTab"));
            AnalysisGUID = Guid.Parse(XmlUtility.GetNodeText(xmlNode.SelectSingleNode("AnalysisGUID")));
		}

        /// <summary>
        /// Create an instance of the display analysis item
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new DisplayAnalysisItem();
        }

        /// <summary>
        /// Get a text decorator for the Display Analysis>
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override ItemTextDecorator CreateTextDecorator(string languageCode)
        {
            return new DisplayAnalysisTextDecorator(this, languageCode);
        }
    }
}
