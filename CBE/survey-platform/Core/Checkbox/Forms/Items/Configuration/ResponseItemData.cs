using System;
using System.Data;
using System.Xml;
using Checkbox.Forms.Data;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Common;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Abstract base class for item meta data for items that store information in a survey response.
    /// </summary>
    [Serializable]
    public abstract class ResponseItemData : ItemData
    {
        /// <summary>
        /// Get/set whether an item must be answered.
        /// </summary>
        public virtual bool IsRequired { get; set; }

        /// <summary>
        /// Copy the item
        /// </summary>
        /// <returns></returns>
        protected override ItemData Copy()
        {
            var theCopy = (ResponseItemData)base.Copy();

            if (theCopy != null)
            {
                theCopy.IsRequired = IsRequired;
            }

            return theCopy;
        }

        #region Data Transfer

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IItemMetadata CreateDataTransferObject()
        {
            return new SurveyItemMetaData();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemDto"></param>
        protected override void BuildDataTransferObject(IItemMetadata itemDto)
        {
            base.BuildDataTransferObject(itemDto);

            if (itemDto is SurveyItemMetaData)
            {
                ((SurveyItemMetaData) itemDto).IsAnswerRequired = IsRequired;
                ((SurveyItemMetaData) itemDto).IsAnswerable = ItemIsIAnswerable;

                ((SurveyItemMetaData)itemDto).RowPosition = -1;
                ((SurveyItemMetaData)itemDto).ColumnPosition = -1;

                ((SurveyItemMetaData)itemDto).ChildItemIds = new int[]{};
                ((SurveyItemMetaData)itemDto).Options = new SurveyOptionMetaData[] { };
            }
        }

        /// <summary>
        /// Save.  A transaction will be created
        /// </summary>
        public override void Save()
        {
            base.Save();

            if (ID.HasValue)
                SurveyMetaDataProxy.RemoveItemFromCache(ID.Value);
        }

        #endregion

        #region Export/Import

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteItemTypeSpecificXml(XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
        {
            base.WriteItemTypeSpecificXml(writer, externalDataCallback);

            writer.WriteElementString("IsRequired", IsRequired.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        protected override void ReadItemTypeSpecificXml(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
        {
            base.ReadItemTypeSpecificXml(xmlNode, callback, creator);
            
            IsRequired =  XmlUtility.GetNodeBool(xmlNode.SelectSingleNode("IsRequired"));
        }

        /// <summary>
        /// Do nothing
        /// </summary>
        /// <param name="callback"></param>
        public override void UpdatePipes(ItemData.UpdatePipesCallback callback)
        {
            
        }
        #endregion
    }
}