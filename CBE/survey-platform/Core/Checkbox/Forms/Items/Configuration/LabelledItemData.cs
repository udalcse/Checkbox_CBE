//===============================================================================
// Checkbox Application Source Code
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================
using System;
using System.Collections.Generic;
using System.Xml;
using Checkbox.Common;
using Checkbox.Globalization.Text;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Base class for items that present an input interface.
    /// </summary>
    [Serializable]
    public abstract class LabelledItemData : LocalizableResponseItemData
    {
        /// <summary>
        /// Get/set the textId for text associated with an item.
        /// </summary>
        public virtual string TextID
        {
            get { return GetTextID("text"); }
        }

        /// <summary>
        /// Get/set the subTextId for text associated with an item.
        /// </summary>
        public virtual string SubTextID
        {
            get { return GetTextID("subText"); }
        }

        /// <summary>
        /// Get a decorator
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override ItemTextDecorator CreateTextDecorator(string languageCode)
        {
            return new LabelledItemTextDecorator(this, languageCode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string[] GetTextIdSuffixes()
        {
            return new List<string>(base.GetTextIdSuffixes()) {"text", "subText", "defaultText"}.ToArray();
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
                var textDataList = ((ItemMetaData) itemDto).TextData;

                foreach (var textData in textDataList)
                {
                    textData.TextValues["Text"] = TextManager.GetText(TextID, textData.LanguageCode);
                    textData.TextValues["Description"] = TextManager.GetText(SubTextID, textData.LanguageCode);

                    textData.TextValues["StrippedText"] = Utilities.StripHtml(textData.TextValues["Text"], null);
                    textData.TextValues["StrippedDescription"] = Utilities.StripHtml(textData.TextValues["Description"], null);

                    textData.TextValues["NavText"] = textData.TextValues["StrippedText"];
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        public override void UpdatePipes(ItemData.UpdatePipesCallback callback)
        {
            updatePipes(callback, TextID);
            updatePipes(callback, SubTextID);
        }
    }
}
