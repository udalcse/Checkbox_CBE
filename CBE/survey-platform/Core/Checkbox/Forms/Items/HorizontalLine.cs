using System;
using System.Collections.Specialized;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Common;

using Checkbox.Common;
using Checkbox.Forms.Items.Configuration;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Business object for a horizontal line
    /// </summary>
    [Serializable]
    public class HorizontalLine : ResponseItem
    {
        private int? _width;
        private string _unit;


        /// <summary>
        /// Configure the item
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData configuration, string languageCode, int? templateId)
        {
            base.Configure(configuration, languageCode, templateId);

            ArgumentValidation.CheckExpectedType(configuration, typeof(HorizontalLineItemData));
            var data = (HorizontalLineItemData)configuration;

            _width = data.LineWidth;
            _unit = data.WidthUnit;
            Thickness = data.Thickness;
            Color = data.Color;
        }

        /// <summary>
        /// Get the width of the item
        /// </summary>
        public string Width
        {
            get
            {
                if (_width.HasValue)
                {
                    return _width.Value + Unit;
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Get the thickness of the item
        /// </summary>
        public int? Thickness { get; private set; }

        /// <summary>
        /// Get the color of the item
        /// </summary>
        public string Color { get; private set; }

        /// <summary>
        /// Get the unit specification for the line width
        /// </summary>
        private string Unit
        {
            get
            {
                if (Utilities.IsNullOrEmpty(_unit))
                {
                    return string.Empty;
                }

                if (string.Compare(_unit, "Pixels", true) == 0)
                {
                    return "px";
                }

                return "%";
            }
        }

        /// <summary>
        /// Get meta data for the item
        /// </summary>
        /// <returns></returns>
        protected override NameValueCollection  GetMetaDataValuesForSerialization()
        {
            NameValueCollection values = base.GetMetaDataValuesForSerialization();

            values["Width"] = Width;
            values["Color"] = Color;
            values["Unit"] = Unit;
            values["Thickness"] = Thickness.HasValue ? Thickness.ToString() : null;

            return values;
        }
    }
}
