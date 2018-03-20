using System;
using System.Xml;
using System.Collections.Generic;
using System.Collections.Specialized;
using Checkbox.Common;
using Checkbox.Common.Captcha;
using Checkbox.Forms.Validation;
using Checkbox.Common.Captcha.Image;
using Checkbox.Forms.Items.Configuration;
using Prezza.Framework.Caching;
using Prezza.Framework.Data;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Item to support creation and generation of CAPTCHA code
    /// </summary>
    [Serializable]
    public class CaptchaItem : LabelledItem
    {
        private string _code;
        private int _imageID;

        private int _minCodeLength;
        private int _maxCodeLength;
        private CodeType _codeType;
        private List<TextStyleEnum> _textStyles;
        private ImageFormatEnum _imageFormat;

        /// <summary>
        /// 
        /// </summary>
        public int ImageHeight { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int ImageWidth { get; private set; }

        /// <summary>
        /// Configure the item
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData configuration, string languageCode, int? templateId)
        {
            Visible = ExportMode != ExportMode.Pdf;
            base.Configure(configuration, languageCode, templateId);

            _minCodeLength = ((CaptchaItemData)configuration).MinCodeLength;
            _maxCodeLength = ((CaptchaItemData)configuration).MaxCodeLength;
            _codeType = ((CaptchaItemData)configuration).CodeType;
            _textStyles = ((CaptchaItemData)configuration).TextStyles;
            _imageFormat = ((CaptchaItemData)configuration).ImageFormat;
            ImageHeight = ((CaptchaItemData)configuration).ImageHeight;
            ImageWidth = ((CaptchaItemData)configuration).ImageWidth;
            EnableSound = ((CaptchaItemData)configuration).EnableSound;
        }

        /// <summary>
        /// Generate the code and the image
        /// </summary>
        private void GenerateCodeAndImage(bool forceRegen)
        {
            if (forceRegen || GetImageID() == null || GetCode() == null)
            {
                //Clean up temp images
                DbUtility.DeleteTempImages();

                _code = GenerateCode();

                _imageID = GenerateImage(_code);

                //Set the answer as a concatenation of imageID and code
                StoreData(_imageID, _code);
            }
        }

        /// <summary>
        /// Generate the Captcha code based on supplied parameters.
        /// </summary>
        private string GenerateCode()
        {
            return CaptchaGenerator.GenerateCode(_codeType, _minCodeLength, _maxCodeLength);
        }

        /// <summary>
        /// Generate and store the actual imate
        /// </summary>
        private int GenerateImage(string code)
        {
            return CaptchaGenerator.GenerateAndStoreImage(code, _imageFormat, ImageWidth, ImageHeight, _textStyles);
        }

        /// <summary>
        /// Get the image URL for the CAPTCHA image.
        /// </summary>
        public string ImageUrl
        {
            get 
            {
                GenerateCodeAndImage(false);

                int? imageID = GetImageID();

                if (imageID.HasValue)
                {
                    return Management.ApplicationManager.ApplicationRoot + "/ViewContent.aspx?ImageID=" + imageID;
                }
                
                return string.Empty;
            }
        }

        /// <summary>
        /// Get the displayed code.
        /// </summary>
        public string Code
        {
            get 
            {
                GenerateCodeAndImage(false);
                return GetCode();
            }
        }

        /// <summary>
        /// Validate the captcha answers
        /// </summary>
        /// <returns></returns>
        protected override bool ValidateAnswers()
        {
            var validator = new CaptchaItemValidator();

            if (!validator.Validate(this))
            {
                //Set a message
                ValidationErrors.Add(validator.GetMessage(LanguageCode));

                //Also regenerate the image
                GenerateCodeAndImage(true);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Get whether sound is enabled.
        /// </summary>
        public bool EnableSound { get; private set; }

        /// <summary>
        /// Get the key for the capcha item cache
        /// </summary>
        /// <returns></returns>
        private string GetCacheKey()
        {
            return Response != null ? Response.ID + "__ " + ID + "__CODE" : string.Empty;
        }

        /// <summary>
        /// Set the answer to the item
        /// </summary>
        /// <param name="code"></param>
        /// <param name="imageID"></param>
        private void StoreData(int imageID, string code)
        {
            //Store the code in a persistent cache, so it is available on postback which prevents the issue
            // where a new code is generated before the user's answer is updated.
            string cacheKey = GetCacheKey();

            if (Utilities.IsNotNullOrEmpty(cacheKey))
            {
                CacheManager cacheManager = CacheFactory.GetCacheManager();
                cacheManager.Add(cacheKey, imageID + "__" + code);
            }
        }

        /// <summary>
        /// Get the answer for the item
        /// </summary>
        /// <returns></returns>
        private string GetCode()
        {
            string cacheKey = GetCacheKey();

            if (Utilities.IsNotNullOrEmpty(cacheKey))
            {
                CacheManager cacheManager = CacheFactory.GetCacheManager();

                if (cacheManager.Contains(cacheKey) && cacheManager[cacheKey] != null)
                {
                    string value = cacheManager[cacheKey].ToString();

                    string[] valueArray = value.Split(new[] { "__" }, StringSplitOptions.RemoveEmptyEntries);

                    if (valueArray.Length > 1)
                    {
                        return valueArray[1];
                    }
                    return null;
                }
            }
            
            return _code;
        }

        /// <summary>
        /// Get the answer for the item
        /// </summary>
        /// <returns></returns>
        private int? GetImageID()
        {
            string cacheKey = GetCacheKey();

            if (Utilities.IsNotNullOrEmpty(cacheKey))
            {
                CacheManager cacheManager = CacheFactory.GetCacheManager();

                if (cacheManager.Contains(cacheKey) && cacheManager[cacheKey] != null)
                {
                    string value = cacheManager[cacheKey].ToString();

                    string[] valueArray = value.Split(new[] { "__" }, StringSplitOptions.RemoveEmptyEntries);

                    if (valueArray.Length > 1)
                    {
                        return Convert.ToInt32(valueArray[0]);
                    }
                    return null;
                }
            }

            return _imageID;
        }

        /// <summary>
        /// Get meta data values for serialization
        /// </summary>
        /// <returns></returns>
        protected override NameValueCollection GetMetaDataValuesForSerialization()
        {
            NameValueCollection values = base.GetMetaDataValuesForSerialization();

            values["minCodeLength"] = _minCodeLength.ToString();
            values["maxCodeLength"] = _maxCodeLength.ToString();
            values["codeType"] = _codeType.ToString();
            values["imageFormat"] = _imageFormat.ToString();
            values["imageHeight"] = ImageHeight.ToString();
            values["imageWidth"] = ImageWidth.ToString();
            values["enableSound"] = EnableSound.ToString();

            return values;
        }

        /// <summary>
        /// Override write meta data to write the text format enum values
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteXmlMetaData(XmlWriter writer)
        {
            base.WriteXmlMetaData(writer);

            writer.WriteStartElement("textFormats");

            foreach (TextStyleEnum textStyle in _textStyles)
            {
                writer.WriteElementString("textFormat", textStyle.ToString());
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// Get instance-specific values, such as the code or image url
        /// </summary>
        /// <returns></returns>
        protected override NameValueCollection GetInstanceDataValuesForSerialization()
        {
            NameValueCollection values = base.GetInstanceDataValuesForSerialization();

            values["imageUrl"] = ImageUrl;
            values["code"] = Code;

            return values;
        }
    }
}
