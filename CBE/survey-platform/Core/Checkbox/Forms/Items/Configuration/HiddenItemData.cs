//===============================================================================
// Checkbox Application Source Code
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================
using System;
using System.Data;
using System.Linq;
using System.Xml;
using Checkbox.Common;
using Checkbox.Forms.Items.UI;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Common;
using Prezza.Framework.Data;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Container for configuration information associated with Hidden Items.
    /// </summary>
    [Serializable]
    public class HiddenItemData : LabelledItemData
    {
        /// <summary>
        /// Hidden item data table name
        /// </summary>
        public override string ItemDataTableName { get { return "HiddenItemData"; } }

        /// <summary>
        /// Load sproc name
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_ItemData_GetHidden"; } }

        /// <summary>
        /// Get/set the name of the variable associated with hidden variables of this type.
        /// </summary>
        public string VariableName { get; set; }

        /// <summary>
        /// Get/set the location (e.g. Session, Database, etc.) of variables
        /// </summary>
        public HiddenVariableSource VariableSource { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public override void SetDefaults(Template template)
        {
            VariableSource = HiddenVariableSource.QueryString;

            //Set unique name
            var names = template.ListTemplateItemIds().Select(template.GetItem).OfType<HiddenItemData>().Select(item => item.VariableName).ToList();

            int number = 1;
            string name = "Variable_1";
            while (names.Contains(name))
            {
                number++;
                name = "Variable_" + number;
            }

            VariableName = name;
        }

        /// <summary>
        /// Create the item configuration in the persistent data store
        /// </summary>
        protected override void Create(IDbTransaction t)
        {
            base.Create(t);

            if (ID <= 0)
            {
                throw new ApplicationException("No DataID specified for Create()");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertHidden");

            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("TextID", DbType.String, TextID);
            command.AddInParameter("SubTextID", DbType.String, SubTextID);
            command.AddInParameter("VariableName", DbType.String, VariableName);
            command.AddInParameter("VariableSource", DbType.String, VariableSource);

            db.ExecuteNonQuery(command, t);

            AppearanceData appearance = new AppearanceDataFactory().CreateAppearanceData("Checkbox.Forms.Items.UI.Hidden,Checkbox");
            appearance.Save(t, ID.Value);
        }

        /// <summary>
        /// Update the item in the persistent data store
        /// </summary>
        protected override void Update(IDbTransaction t)
        {
            base.Update(t);

            if (ID <= 0)
            {
                throw new Exception("No DataID specified for Update()");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateHidden");

            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("TextID", DbType.String, TextID);
            command.AddInParameter("SubTextID", DbType.String, SubTextID);
            command.AddInParameter("VariableName", DbType.String, VariableName);
            command.AddInParameter("VariableSource", DbType.String, VariableSource);

            db.ExecuteNonQuery(command, t);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteItemTypeSpecificXml(XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
        {
            base.WriteItemTypeSpecificXml(writer, externalDataCallback);

			writer.WriteElementString("VariableName", VariableName);
			writer.WriteElementString("VariableSource", VariableSource.ToString());
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        protected override void ReadItemTypeSpecificXml(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
        {
            base.ReadItemTypeSpecificXml(xmlNode, callback, creator);

			VariableName = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("VariableName"));

            var enumString = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("VariableSource"));

            if (string.IsNullOrEmpty(enumString))
            {
                enumString = HiddenVariableSource.QueryString.ToString();
            }

            VariableSource =
                (HiddenVariableSource)
                Enum.Parse(typeof(HiddenVariableSource), enumString);
		}

        /// <summary>
        /// Load the hidden item data from the provided <see cref="DataRow"/>.
        /// </summary>
        /// <param name="data"><see cref="DataRow"/> containing configuration information.</param>
        protected override void LoadBaseObjectData(DataRow data)
        {
            base.LoadBaseObjectData(data);

            VariableSource = (HiddenVariableSource)Enum.Parse(
                typeof(HiddenVariableSource),
                (string)data["VariableSource"]);

            VariableName = DbUtility.GetValueFromDataRow(data, "Variablename", string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemDto"></param>
        protected override void BuildDataTransferObject(IItemMetadata itemDto)
        {
            base.BuildDataTransferObject(itemDto); 
            
            if (itemDto is ItemMetaData)
            {
                var textDataList = ((ItemMetaData)itemDto).TextData;

                foreach (var textData in textDataList)
                {
                    textData.TextValues["NavText"] = 
                        string.Format("{0} - {1}[\"{2}\"]",
                            Utilities.TruncateText(textData.TextValues["StrippedText"], 50),
                            VariableSource,
                            VariableName);
                }
            }
        }

        /// <summary>
        /// Create an instance of a hidden item based on this configuration.
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new HiddenItem();
        }

        /// <summary>
        /// Report that the associated Item implements the IAnswerable interface
        /// </summary>
        public override bool ItemIsIAnswerable
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Create a text decorator for this item
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override ItemTextDecorator CreateTextDecorator(string languageCode)
        {
            return new LabelledItemTextDecorator(this, languageCode);
        }
    }
}
