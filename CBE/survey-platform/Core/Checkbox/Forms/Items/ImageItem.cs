//===============================================================================
// Checkbox Application Source Code
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================
using System;
using System.Collections.Specialized;
using Prezza.Framework.Common;
using Checkbox.Forms.Items.Configuration;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Item containing an image.
    /// </summary>
    [Serializable]
    public class ImageItem : ResponseItem
    {
        private string _altTextID;

        /// <summary>
        /// Get the path to the image.
        /// </summary>
        public string ImagePath { get; private set; }

        /// <summary>
        /// Get the image alt text
        /// </summary>
        public string AlternateText
        {
            get { return GetText(_altTextID); }
        }

        /// <summary>
        /// Configure the item
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData configuration, string languageCode, int? templateId)
        {
            ArgumentValidation.CheckExpectedType(configuration, typeof(ImageItemData));

            base.Configure(configuration, languageCode, templateId);

            var config = (ImageItemData)configuration;
            _altTextID = config.AlternateTextID;
            ImagePath = config.ImagePath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override NameValueCollection GetInstanceDataValuesForSerialization()
        {
            var values = base.GetInstanceDataValuesForSerialization();

            values["ImagePath"] = ImagePath;
            values["AlternateText"] = AlternateText;

            return values;
        }
    }
}
