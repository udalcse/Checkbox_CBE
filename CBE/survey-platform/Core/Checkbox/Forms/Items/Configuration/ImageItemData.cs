//===============================================================================
// Checkbox Application Source Code
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml;
using Checkbox.Common;
using Checkbox.Globalization.Text;
using Checkbox.Management;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Common;
using Prezza.Framework.Data;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Container for configuration data associated with Image Items.
    /// </summary>
    [Serializable]
    public class ImageItemData : LocalizableResponseItemData
    {
        private byte[] _imageData;
        private Guid? _guid;
        private string _imageName;
        private string _contentType;

        /// <summary>
        /// Get the name of the table containing image item data
        /// </summary>
        public override string ItemDataTableName { get { return "ImageItemData"; } }

        /// <summary>
        /// Get load item sproc.
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_ItemData_GetImage"; } }

        /// <summary>
        /// Get/set the path for the image item.
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// Get/set the image id
        /// </summary>
        public int? ImageID { get; set; }

        /// <summary>
        /// Get the image alt. text id
        /// </summary>
        public string AlternateTextID
        {
            get { return GetTextID("altText"); }
        }

        /// <summary>
        /// Image data
        /// </summary>
        public byte[] ImageData
        {
            get
            {
                return _imageData;
            }
            set
            {
                _imageData = value;
            }
        }

        /// <summary>
        /// Create an instance of configuration information in the data store.
        /// </summary>
        protected override void Create(IDbTransaction t)
        {
            base.Create(t);

            if (ID <= 0)
            {
                throw new ApplicationException("No DataID specified for Create()");
            }

            SaveImageData(t);

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertImage");
            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("ImagePath", DbType.String, ImagePath);
            command.AddInParameter("ImageID", DbType.Int32, ImageID);

            db.ExecuteNonQuery(command, t);
        }

        /// <summary>
        /// Update an instance of configuration information in the data store.
        /// </summary>
        protected override void Update(IDbTransaction t)
        {
            base.Update(t);

            if (ID <= 0)
            {
                throw new ApplicationException("No DataID specified for Update()");
            }

            SaveImageData(t);

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateImage");

            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("ImagePath", DbType.String, ImagePath);
            command.AddInParameter("ImageID", DbType.Int32, ImageID);

            db.ExecuteNonQuery(command, t);
        }

        /// <summary>
        /// Save the image data, if necessary
        /// </summary>
        /// <param name="t"></param>
        protected virtual void SaveImageData(IDbTransaction t)
        {
            //If necessary, save the image data.  We want to save when image data is present and not saved or when
            // image path has been set.
            if (!ImageID.HasValue || ImageID.Value <= 0)
            {
                string guid = _guid.HasValue ? _guid.ToString() : Guid.NewGuid().ToString();

                if (_imageData != null)
                {
                    ImageID = DbUtility.SaveImage(_imageData, _contentType, null, _imageName, guid);
                    ImagePath = Management.ApplicationManager.ApplicationRoot + "/ViewContent.aspx?ImageID=" + ImageID;
                }
                else if (Utilities.IsNotNullOrEmpty(ImagePath))
                {
                    ImageID = DbUtility.SaveImage(null, _contentType, ImagePath, _imageName, guid);
                }
            }
        }

        /// <summary>
        /// Load the image item configuration properties from the supplied <see cref="DataRow"/>.
        /// </summary>
        /// <param name="data"><see cref="DataRow"/> containing configuration information.</param>
        protected override void LoadBaseObjectData(DataRow data)
        {
            base.LoadBaseObjectData(data);

            ImagePath = DbUtility.GetValueFromDataRow(data, "ImagePath", string.Empty);
            ImageID = DbUtility.GetValueFromDataRow<int?>(data, "ImageId", null);
            _imageData = DbUtility.GetValueFromDataRow<byte[]>(data, "ImageData", null);
            _imageName = DbUtility.GetValueFromDataRow(data, "ImageName", string.Empty);
            string guid = DbUtility.GetValueFromDataRow<string>(data, "Guid", null);
            if (!string.IsNullOrEmpty(guid))
                _guid = Guid.Parse(guid);
            _contentType = DbUtility.GetValueFromDataRow(data, "ContentType", string.Empty);

            //Ensure there is an image path if there is image data
            if (_imageData != null)
            {
                ImagePath = Management.ApplicationManager.ApplicationRoot + "/ViewContent.aspx?ImageID=" + ImageID;
            }
        }

        /// <summary>
        /// Add text data to metadata object
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
                    textData.TextValues["Text"] = ImagePath;

                    textData.TextValues["StrippedText"] = ImagePath;

                    textData.TextValues["NavText"] = Utilities.TruncateText(ImagePath, 50);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string[] GetTextIdSuffixes()
        {
            return new List<string>(base.GetTextIdSuffixes()) {AlternateTextID}.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteItemTypeSpecificXml(XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
        {
            base.WriteItemTypeSpecificXml(writer, externalDataCallback);

            writer.WriteElementString("Name", _imageName);
			writer.WriteElementString("Type", _contentType);
            writer.WriteElementString("ImagePath", ImagePath);
			writer.WriteStartElement("Data");

			int len = _imageData == null ? 0 : _imageData.Length;

			writer.WriteAttributeString("Length", len.ToString());

			if (_imageData != null)
				writer.WriteBase64(_imageData, 0, _imageData.Length);

			writer.WriteEndElement();
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        protected override void ReadItemTypeSpecificXml(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
        {
            base.ReadItemTypeSpecificXml(xmlNode, callback, creator);

			_imageName = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("Name"));
			_contentType = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("Type"));
            ImagePath = CheckImagePath(XmlUtility.GetNodeText(xmlNode.SelectSingleNode("ImagePath")));

            var dataNode = xmlNode.SelectSingleNode("Data");

            if(dataNode == null)
            {
                return;
            }

            var dataLength = XmlUtility.GetAttributeInt(dataNode, "Length");

            if(dataLength > 0)
            {
                _imageData = Convert.FromBase64String(dataNode.InnerText);
            }
		}

        private string CheckImagePath(string path)
        {
            //there could be a sutiation when the image path has an obsolete application context path
            //we support only imports from the same application context
            if (!string.IsNullOrEmpty(path) 
                && path.Contains("ViewContent")
                && !path.Contains(ApplicationManager.ApplicationRoot))
            {
                return "none";
            }

            return path;
        }

        /// <summary>
        /// Create an instance of an image item based on this configuration.
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new ImageItem();
        }

        /// <summary>
        /// Get a text decorator for this item
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override ItemTextDecorator CreateTextDecorator(string languageCode)
        {
            return new ImageItemTextDecorator(this, languageCode);
        }
    }
}
