//===============================================================================
// Checkbox Application Source Code
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================
using System;
using System.Collections.Specialized;

using Checkbox.Common;
using Prezza.Framework.Common;
using Checkbox.Forms.Items.Configuration;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Represents an item that has no user-interface component, but can capture values from other sources.
    /// </summary>
    [Serializable]
    public class HiddenItem : LabelledItem
    {
        /// <summary>
        /// Gets an enum that indicates the location of the variables
        /// </summary>
        public HiddenVariableSource VariableSource { get; private set; }

        /// <summary>
        /// Get the name of the variable containing the hidden item data
        /// </summary>
        public string VariableName { get; private set; }

        /// <summary>
        /// Get the alias for the item.  Overriden to return the variable
        /// name as the alias.
        /// </summary>
        public override string Alias
        {
            get
            {
                if(Utilities.IsNotNullOrEmpty(base.Alias))
                {
                    return base.Alias;
                }
                
                return VariableName;
            }
        }

        /// <summary>
        /// Override visible flag, since this item is only visible when
        /// editing a survey
        /// </summary>
        public override bool Visible
        {
            get{return Response == null;}
        }

        /// <summary>
        /// Validate answers
        /// </summary>
        /// <returns></returns>
        protected override bool ValidateAnswers()
        {
            return true;
        }

        /// <summary>
        /// Configure the item based on the supplied <see cref="HiddenItemData"/> and language code
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData configuration, string languageCode, int? templateId)
        {
            base.Configure(configuration, languageCode, templateId);

            ArgumentValidation.CheckExpectedType(configuration, typeof(HiddenItemData));
            var config = (HiddenItemData)configuration;
            
            VariableSource = config.VariableSource;
            VariableName = config.VariableName;
        }

        /// <summary>
        /// Get the name value collection for serialization of meta data
        /// </summary>
        /// <returns></returns>
        protected override NameValueCollection GetMetaDataValuesForSerialization()
        {
            NameValueCollection values =  base.GetMetaDataValuesForSerialization();

            values["VariableSource"] = VariableSource.ToString();
            values["VariableName"] = VariableName;

            return values;
        }
    }

    /// <summary>
    /// An enumeration of the supported sources from which to set the hidden value
    /// </summary>
    [Serializable]
    public enum HiddenVariableSource
    {
        /// <summary>
        /// Read value from URL query string parameter.
        /// </summary>
        QueryString = 1,

        /// <summary>
        /// Read value from ASP.NET session.
        /// </summary>
        Session,

        /// <summary>
        /// Read value from prezza cookie (name defined in Application Settings).
        /// </summary>
        Cookie
    }
}
