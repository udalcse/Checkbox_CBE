using System;
using System.Data;
using System.Collections.Generic;
using System.Xml;
using Checkbox.Common.Captcha;
using Checkbox.Common.Captcha.Image;
using Prezza.Framework.Common;
using Prezza.Framework.Data;
using Prezza.Framework.ExceptionHandling;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// MetaData for Captcha Items
    /// </summary>
    [Serializable]
    public class CaptchaItemData : LabelledItemData
    {
        private string _textStyles;

        /// <summary>
        /// Get data table name
        /// </summary>
        public override string ItemDataTableName { get { return "CaptchaItemData"; } }

        /// <summary>
        /// Get sproc for loading item
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_ItemData_GetCaptcha"; } }

        /// <summary>
        /// Default constructor
        /// </summary>
        public CaptchaItemData()
        {
            //Default value
            _textStyles = string.Empty;

            AddTextStyle(TextStyleEnum.AncientMosaic.ToString());

            MinCodeLength = 5;
            MaxCodeLength = 5;
            CodeType = CodeType.AlphaNumeric;
            ImageHeight = 200;
            ImageWidth = 250;
            ImageFormat = ImageFormatEnum.Gif;
            EnableSound = false;
        }

        /// <summary>
        /// Get/set the maximum length for the CAPTCHA code.
        /// </summary>
        /// <remarks>Value must be greater than the minimum value and must be greater than 0.</remarks>
        public int MaxCodeLength { get; set; }

        /// <summary>
        /// Get/set the minimum code length.
        /// </summary>
        /// <remarks>Value must be less than the maximum value and must be greater than 0.</remarks>
        public int MinCodeLength { get; set; }

        /// <summary>
        /// Get/set the type of code to use.
        /// </summary>
        public CodeType CodeType { get; set; }

        /// <summary>
        /// Get/set the height of the image to generate.
        /// </summary>
        public int ImageHeight { get; set; }

        /// <summary>
        /// Get/set the width of the image to generate
        /// </summary>
        public int ImageWidth { get; set; }

        /// <summary>
        /// Get/set the image format
        /// </summary>
        public ImageFormatEnum ImageFormat { get; set; }

        /// <summary>
        /// Get/set whether sound is enabled for the control.
        /// </summary>
        public bool EnableSound { get; set; }

        /// <summary>
        /// Get a list of text styles supported.
        /// </summary>
        public List<TextStyleEnum> TextStyles
        {
            get
            {
                var outStyles = new List<TextStyleEnum>();

                string[] styles = _textStyles.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string style in styles)
                {
                    try
                    {
                        outStyles.Add((TextStyleEnum)Enum.Parse(typeof(TextStyleEnum), style, true));
                    }
                    catch (Exception ex)
                    {
                        ExceptionPolicy.HandleException(ex, "BusinessPublic");
                    }
                }

                return outStyles;
            }
        }

        /// <summary>
        /// Remove the text style with the specified name from the list of supported styles.
        /// </summary>
        /// <param name="textStyleName">Name of the style to remove.  Should conform to a TextStyleEnum value.</param>
        public void RemoveTextStyle(string textStyleName)
        {
            //Remove from the string
            if (String.Compare(textStyleName, _textStyles, true) == 0)
            {
                _textStyles = string.Empty;
            }
            else if (_textStyles.Contains(textStyleName + ","))
            {
                _textStyles = _textStyles.Replace(textStyleName + ",", string.Empty);
            }
            else if (_textStyles.Contains("," + textStyleName))
            {
                _textStyles = _textStyles.Replace("," + textStyleName, string.Empty);
            }
        }

        /// <summary>
        /// Remove the specified style from the list of supported styles
        /// </summary>
        /// <param name="textStyle">Text style to remove from the list.</param>
        public void RemoveTextStyle(TextStyleEnum textStyle)
        {
            RemoveTextStyle(textStyle.ToString());
        }

        /// <summary>
        /// Add the text style with the specified name to the list of supported styles.
        /// </summary>
        /// <param name="textStyleName">Name of the text style to add.  This name must correspond to an value of the TextTypeEnum.</param>
        public void AddTextStyle(string textStyleName)
        {
            if (_textStyles.Trim() == string.Empty)
            {
                _textStyles = textStyleName;
            }
            else if (!_textStyles.Contains("," + textStyleName) && !_textStyles.Contains(textStyleName + ","))
            {
                _textStyles += "," + textStyleName;
            }
        }

        /// <summary>
        /// Add the specified text style to the list of supported styles.
        /// </summary>
        /// <param name="textStyle">Style to add.</param>
        public void AddTextStyle(TextStyleEnum textStyle)
        {
            AddTextStyle(textStyle.ToString());
        }

        /// <summary>
        /// Replace the list of text styles with the specified list.
        /// </summary>
        /// <param name="textStyles">List of styles to set.</param>
        public void SetTextStyles(List<TextStyleEnum> textStyles)
        {
            _textStyles = string.Empty;

            foreach (TextStyleEnum style in textStyles)
            {
                AddTextStyle(style);
            }
        }

        /// <summary>
        /// Replace the list of text styles with the specified list.
        /// </summary>
        /// <param name="textStyleNames">List of styles to set.</param>
        public void SetTextStyles(List<string> textStyleNames)
        {
            _textStyles = string.Empty;

            foreach (string style in textStyleNames)
            {
                AddTextStyle(style);
            }
        }

        /// <summary>
        /// Create an instance of a hidden item based on this configuration.
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new CaptchaItem();
        }

        /// <summary>
        /// Load the item's configuration from the specified data row.
        /// </summary>
        /// <param name="data">DataRow containing configuration information for this item.</param>
        protected override void LoadBaseObjectData(DataRow data)
        {
            base.LoadBaseObjectData(data);

            MaxCodeLength = DbUtility.GetValueFromDataRow(data, "MaxCodeLength", 5);
            MinCodeLength = DbUtility.GetValueFromDataRow(data, "MinCodeLength", 5);
            ImageHeight = DbUtility.GetValueFromDataRow(data, "ImageHeight", 200);
            ImageWidth = DbUtility.GetValueFromDataRow(data, "ImageWidth", 250);
            EnableSound = DbUtility.GetValueFromDataRow(data, "EnableSound", false);

            if (data["CodeType"] != DBNull.Value)
            {
                CodeType = (CodeType)Enum.Parse(typeof(CodeType), (string)data["CodeType"]);
            }

            if (data["TextStyles"] != DBNull.Value)
            {
                var styles = new List<string>(((string)data["TextStyles"]).Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries));
                SetTextStyles(styles);
            }

            if (data["ImageFormat"] != DBNull.Value)
            {
                ImageFormat = (ImageFormatEnum)Enum.Parse(typeof(ImageFormatEnum), (string)data["ImageFormat"]);
            }
        }

        /// <summary>
        /// Create a new entry for captcha item data in the database.
        /// </summary>
        /// <param name="t">Database transaction to participate in for database updates.</param>
        protected override void Create(IDbTransaction t)
        {
            base.Create(t);

            if (ID == null || ID <= 0)
            {
                throw new Exception("Unable to create configuration data data for CaptchaItem with null or negative id.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertCaptcha");
            AddParams(command);
            db.ExecuteNonQuery(command, t);
        }

        /// <summary>
        /// Update an entry for captcha item data in the database.
        /// </summary>
        /// <param name="t">Database transaction to participate in for database updates.</param>
        protected override void Update(IDbTransaction t)
        {
            base.Update(t);

            if (ID == null || ID <= 0)
            {
                throw new Exception("Unable to update configuration data data for CaptchaItem with null or negative id.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateCaptcha");
            AddParams(command);
            db.ExecuteNonQuery(command, t);
        }

        /// <summary>
        /// Add parameters to the 
        /// </summary>
        /// <param name="command"></param>
        private void AddParams(DBCommandWrapper command)
        {
            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("MaxCodeLength", DbType.Int32, MaxCodeLength);
            command.AddInParameter("MinCodeLength", DbType.Int32, MinCodeLength);
            command.AddInParameter("CodeType", DbType.String, CodeType.ToString());
            command.AddInParameter("ImageHeight", DbType.Int32, ImageHeight);
            command.AddInParameter("ImageWidth", DbType.Int32, ImageWidth);
            command.AddInParameter("TextStyles", DbType.String, _textStyles);
            command.AddInParameter("ImageFormat", DbType.String, ImageFormat);
            command.AddInParameter("EnableSound", DbType.Boolean, EnableSound);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteItemTypeSpecificXml(XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
        {
            base.WriteItemTypeSpecificXml(writer, externalDataCallback);

			writer.WriteElementString("MaxCodeLength", MaxCodeLength.ToString());
			writer.WriteElementString("MinCodeLength", MinCodeLength.ToString());
			writer.WriteElementString("CodeType", CodeType.ToString());
			writer.WriteElementString("ImageHeight", ImageHeight.ToString());
			writer.WriteElementString("ImageWidth", ImageWidth.ToString());
			writer.WriteElementString("ImageFormat", ImageFormat.ToString());
			writer.WriteElementString("TextStyles", _textStyles);
			writer.WriteElementString("EnableSound", EnableSound.ToString());
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        protected override void ReadItemTypeSpecificXml(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
        {
            base.ReadItemTypeSpecificXml(xmlNode, callback, creator);

            var enumString = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("CodeType"));

            if (string.IsNullOrEmpty(enumString))
            {
                enumString = CodeType.AlphaNumeric.ToString();
            }

            CodeType =
                (CodeType)
                Enum.Parse(typeof(CodeType), enumString);

            enumString = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("ImageFormat"));

            if (string.IsNullOrEmpty(enumString))
            {
                enumString = ImageFormatEnum.Jpeg.ToString();
            }

            ImageFormat =
                (ImageFormatEnum)
                Enum.Parse(typeof(ImageFormatEnum), enumString);

            MaxCodeLength = XmlUtility.GetNodeInt(xmlNode.SelectSingleNode("MaxCodeLength")) ?? 5;
            MinCodeLength = XmlUtility.GetNodeInt(xmlNode.SelectSingleNode("MinCodeLength")) ?? 5;
            ImageHeight = XmlUtility.GetNodeInt(xmlNode.SelectSingleNode("ImageHeight")) ?? 250;
            ImageWidth = XmlUtility.GetNodeInt(xmlNode.SelectSingleNode("ImageWidth")) ?? 250;
			_textStyles = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("TextStyles"));
			EnableSound = XmlUtility.GetNodeBool(xmlNode.SelectSingleNode("EnableSound"));
		}
    }
}
