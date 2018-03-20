//===============================================================================
// Checkbox Application Source Code
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Xml;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Configuration information for Text items.
    /// </summary>
    [Serializable]
    public abstract class TextItemData : LabelledItemData
    {
        ///<summary>
        /// Specify if the HTML editor must be used
        ///</summary>
        public virtual bool IsHtmlFormattedData { get; set; }

        /// <summary>
        /// Get/set the default text for the item
        /// </summary>
        public virtual string DefaultTextID
        {
            get { return GetTextID("defaultText"); }
        }

        /// <summary>
        /// Get/set the <see cref="AnswerFormat"/> for the item.
        /// </summary>
        public virtual AnswerFormat Format { get; set; }

        /// <summary>
        /// Get/set the unique identifier for a custom <see cref="AnswerFormat"/>
        /// </summary>
        public virtual string CustomFormatId { get; set; }

        /// <summary>
        /// Get/set the maximum length for the item
        /// </summary>
        public virtual int? MaxLength { get; set; }

        /// <summary>
        /// Get/set the minimum length for the item
        /// </summary>
        public virtual int? MinLength { get; set; }

        /// <summary>
        /// Get whether this represents an answerable item
        /// </summary>
        public override bool ItemIsIAnswerable
        {
            get { return true; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string[] GetTextIdSuffixes()
        {
            return new List<string>(base.GetTextIdSuffixes()) {"defaultText"}.ToArray();
        }
        
        /// <summary>
        /// Copy the item data
        /// </summary>
        /// <returns></returns>
        protected override ItemData Copy()
        {
            var theCopy = (TextItemData)base.Copy();

            if (theCopy != null)
            {
                theCopy.Format = Format;
                theCopy.MaxLength = MaxLength;
                theCopy.MinLength = MinLength;
                theCopy.CustomFormatId = CustomFormatId;
                theCopy.IsHtmlFormattedData = IsHtmlFormattedData;
            }

            return theCopy;
        }
    }
}
