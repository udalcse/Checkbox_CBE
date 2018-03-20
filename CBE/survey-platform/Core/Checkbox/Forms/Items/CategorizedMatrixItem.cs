using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Validation;
using Checkbox.Globalization.Text;

using Prezza.Framework.Common;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Categorized Matrix item contains all functionality of a standard MatrixItem as well as additional functionality
    /// requested by the Greater Wellington Regional Council. The Categorized Matrix Item supports both categorizations
    /// of options and Row Select item types.
    /// </summary>
    [Serializable]
    public class CategorizedMatrixItem : MatrixItem
    {
        /// <summary>
        /// Initialize the matrix item with its configuration data
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData configuration, string languageCode, int? templateId)
        {
            ArgumentValidation.CheckExpectedType(configuration, typeof(CategorizedMatrixItemData));
            base.Configure(configuration, languageCode, templateId);
        }        
    }
}