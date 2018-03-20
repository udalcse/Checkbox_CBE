using System;
using Checkbox.Common;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Data for item to close the survey window
    /// </summary>
    [Serializable]
    public class CloseWindowItemData : ResponseItemData
    {
        /// <summary>
        /// Item has no specific configuration data, so just reference a dummy name as table name.
        /// </summary>
        public override string ItemDataTableName { get { return "CloseWindowItem"; } }

        /// <summary>
        /// Get load sproc name.  This type of item has no special configuration data, so there is no load sproc.
        /// </summary>
        protected override string LoadSprocName { get { return string.Empty; } }

		/// <summary>
		/// Item has no specific configuration data, so just reference parent table as data
		/// table name.
		/// </summary>
		public override string DataTableName { get { return ParentDataTableName; } }


        /// <summary>
        /// Create the item
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new CloseWindowItem();
        }

        /// <summary>
        /// Since item has no configuration, override this method to set item type name
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadAdditionalData(PersistedDomainObjectDataSet data)
        {
            base.LoadAdditionalData(data);

            ItemTypeName = "CloseWindow";
            ItemTypeID = ItemConfigurationManager.GetTypeIdFromName(ItemTypeName).Value;
        }

        #region Data Transfer
        
        /// <summary>
        /// Get data transfer object for item.  Since item has no configuration state, return empty item.
        /// </summary>
        /// <returns></returns>
        public override IItemMetadata CreateDataTransferObject()
        {
            return new ItemMetaData();
        }

        #endregion
    }
}
    
