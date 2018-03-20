//===============================================================================
// Checkbox Application Source Code
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Xml;
using Prezza.Framework.Common;
using Prezza.Framework.Data;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Container class for configuration information for a select many item.
    /// </summary>
    [Serializable]
    public class SelectManyData : SelectItemData
    {
        /// <summary>
        /// 
        /// </summary>
        public override string ItemDataTableName { get { return "SelectManyData"; } }

        /// <summary>
        /// Get load item procedure
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_ItemData_GetSelectMany"; } }

        /// <summary>
        /// Get whether the item is required based on the min/max values
        /// </summary>
        public override bool IsRequired
        {
            get
            {
                if (MinToSelect != null && MinToSelect > 0)
                {
                    return true;
                }

                return false;
            }
            set
            {
                //Do nothing
            }
        }

        /// <summary>
        /// Get/set the minimum number of items to select.  The default value is zero.
        /// </summary>
        public int? MinToSelect { get; set; }
        /// <summary>
        /// Get/set the maximum number of items to select.  The default value is the number of total options
        /// </summary>
        public int? MaxToSelect { get; set; }

        /// <summary>
        /// Get/set if 'none of above' option is 
        /// </summary>
        public bool AllowNoneOfAbove { get; set; }

        /// <summary>
        /// Create an instance of a Select1 configuration in the persistent store
        /// </summary>
        protected override void Create(IDbTransaction transaction)
        {
            base.Create(transaction);

            if (ID <= 0)
            {
                throw new ApplicationException("No DataID specified.");
            }

            Database db = DatabaseFactory.CreateDatabase();

            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertSelectMany");
            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("TextID", DbType.String, TextID);
            command.AddInParameter("SubTextID", DbType.String, SubTextID);
            command.AddInParameter("IsRequired", DbType.Int32, IsRequired);
            command.AddInParameter("AllowOther", DbType.Int32, AllowOther);
            command.AddInParameter("OtherTextID", DbType.String, OtherTextID);
            command.AddInParameter("Randomize", DbType.Int32, Randomize);
            command.AddInParameter("MinToSelect", DbType.Int32, MinToSelect);
            command.AddInParameter("MaxToSelect", DbType.Int32, MaxToSelect);
            command.AddInParameter("AllowNoneOfAbove", DbType.Int32, AllowNoneOfAbove);

            db.ExecuteNonQuery(command, transaction);

            UpdateLists(transaction);
        }

        /// <summary>
        /// Update an instance of a SelectMany configuration in the persistent store
        /// </summary>
        protected override void Update(IDbTransaction transaction)
        {
            base.Update(transaction);

            if (ID <= 0)
            {
                throw new ApplicationException("No DataID specified.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateSelectMany");

            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("TextID", DbType.String, TextID);
            command.AddInParameter("SubTextID", DbType.String, SubTextID);
            command.AddInParameter("IsRequired", DbType.Int32, IsRequired);
            command.AddInParameter("AllowOther", DbType.Int32, AllowOther);
            command.AddInParameter("OtherTextID", DbType.String, OtherTextID);
            command.AddInParameter("Randomize", DbType.Int32, Randomize);
            command.AddInParameter("MinToSelect", DbType.Int32, MinToSelect);
            command.AddInParameter("MaxToSelect", DbType.Int32, MaxToSelect);
            command.AddInParameter("AllowNoneOfAbove", DbType.Int32, AllowNoneOfAbove);

            db.ExecuteNonQuery(command, transaction);

            UpdateLists(transaction);
        }

        /// <summary>
        /// Load the configuration information from the supplied <see cref="DataRow"/>.
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadBaseObjectData(DataRow data)
        {
            base.LoadBaseObjectData(data);

            IsRequired = DbUtility.GetValueFromDataRow(data, "IsRequired", 0) == 1;
            AllowOther = DbUtility.GetValueFromDataRow(data, "AllowOther", 0) == 1;
            Randomize = DbUtility.GetValueFromDataRow(data, "Randomize", 0) == 1;

            MinToSelect = DbUtility.GetValueFromDataRow<int?>(data, "MinToSelect", null);
            MaxToSelect = DbUtility.GetValueFromDataRow<int?>(data, "MaxToSelect", null);
            AllowNoneOfAbove = DbUtility.GetValueFromDataRow(data, "AllowNoneOfAbove", false);
        }

        /// <summary>
        /// Create an instance of a <see cref="SelectMany"/> item based on this configuration.
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new SelectMany();
        }

        /// <summary>
        /// Copy the item
        /// </summary>
        /// <returns></returns>
        protected override ItemData Copy()
        {
            var theCopy = (SelectManyData)base.Copy();

            if (theCopy != null)
            {
                theCopy.MinToSelect = MinToSelect;
                theCopy.MaxToSelect = MaxToSelect;
                theCopy.AllowNoneOfAbove = AllowNoneOfAbove;
            }

            return theCopy;
        }

        /// <summary>
        /// Get access to the list data datatable
        /// </summary>
        protected DataTable ListData { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteItemTypeSpecificXml(XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
        {
            base.WriteItemTypeSpecificXml(writer, externalDataCallback);

			writer.WriteElementValue("MinToSelect", MinToSelect);
			writer.WriteElementValue("MaxToSelect", MaxToSelect);
            writer.WriteElementValue<bool>("AllowNoneOfAbove", AllowNoneOfAbove);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        protected override void ReadItemTypeSpecificXml(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
		{
            base.ReadItemTypeSpecificXml(xmlNode, callback, creator);

            MinToSelect = XmlUtility.GetNodeInt(xmlNode.SelectSingleNode("MinToSelect"));
			MaxToSelect = XmlUtility.GetNodeInt(xmlNode.SelectSingleNode("MaxToSelect"));
            AllowNoneOfAbove = XmlUtility.GetNodeBool(xmlNode.SelectSingleNode("AllowNoneOfAbove"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="noneOfAboveOption"></param>
        protected void UpdateNoneOfAboveOption(ListOptionData noneOfAboveOption)
        {
            //Add an "other" option if necessary or move the existing "other" option to the end
            if (AllowNoneOfAbove && OptionsList != null)
            {
                if (noneOfAboveOption == null)
                {
                    //OptionsList.AddOption(ID.Value, "other", otherOption.Category, false, OptionsList.ListOptions.Count + 1, 0, true);
                    OptionsList.AddOption(ID.Value, "none of the above", String.Empty, false, OptionsList.ListOptions.Count + 1, 0, false, true, null);
                }
                else
                {
                    OptionsList.UpdateOption(noneOfAboveOption.OptionID, -1, "none of the above", noneOfAboveOption.Category, false, OptionsList.ListOptions.Count + 1, 0, false, true, null);
                }
            }
        }

        /// <summary>
        /// Update lists associated with the select item
        /// </summary>
        /// <param name="transaction"></param>
        protected override void UpdateLists(IDbTransaction transaction)
        {
            //Make sure lists contain "other" and "none of above", if necessary
            ListOptionData otherOption = null;
            ListOptionData noneOfAboveOption = null;

            var optionsToRemove = new List<ListOptionData>();
            ReadOnlyCollection<ListOptionData> tempListOptions = OptionsList.ListOptions;

            foreach (ListOptionData option in tempListOptions)
            {
                if (option.IsOther)
                {
                    otherOption = option;
                    if (!AllowOther)
                    {
                        optionsToRemove.Add(option);
                        otherOption = null;
                    }
                }

                if (option.IsNoneOfAbove)
                {
                    noneOfAboveOption = option;
                    if (!AllowNoneOfAbove)
                    {
                        optionsToRemove.Add(option);
                        noneOfAboveOption = null;
                    }
                }
            }

            foreach (ListOptionData option in optionsToRemove)
            {
                OptionsList.RemoveOption(option.OptionID);
            }

            UpdateOtherOption(otherOption);
            UpdateNoneOfAboveOption(noneOfAboveOption);

            //Update the lists
            OptionsList.Save(transaction);

            Database db = DatabaseFactory.CreateDatabase();

            //AddListData will not add duplicates, so it's safe to call for all lists
            DBCommandWrapper addCommand = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_AddListData");
            addCommand.AddInParameter("ListID", DbType.Int32, OptionsList.ID);
            addCommand.AddInParameter("ItemID", DbType.Int32, ID);
            db.ExecuteNonQuery(addCommand, transaction);
        }

    }
}