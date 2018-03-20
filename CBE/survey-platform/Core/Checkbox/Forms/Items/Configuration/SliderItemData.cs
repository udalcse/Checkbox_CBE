using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Xml;
using Prezza.Framework.Common;
using Prezza.Framework.Data;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Container for configuration information associated with Slider items.
    /// </summary>
    [Serializable]
    public class SliderItemData : SelectItemData
    {
        /// <summary>
        /// Data table name for slider item configuration data
        /// </summary>
        public override string ItemDataTableName
        {
            get { return "SliderItemData"; }
        }

        /// <summary>
        /// Load procedure name
        /// </summary>
        protected override string LoadSprocName
        {
            get { return "ckbx_sp_ItemData_GetSlider"; }
        }

        /// <summary>
        /// Get/Set Value Type
        /// </summary>
        public SliderValueType ValueType { get; set; }

        /// <summary>
        /// Get/Set Min Value
        /// </summary>
        public int? MinValue { get; set; }

        /// <summary>
        /// Get/Set Max Value
        /// </summary>
        public int? MaxValue { get; set; }

        /// <summary>
        /// Size of a given “step” when moving the slider.  
        /// </summary>
        public int? StepSize { get; set; }

        /// <summary>
        /// Initial starting value for the slider. Uses only in NumberRange value type.
        /// </summary>
        public int? DefaultValue { get; set; }

        /// <summary>
        /// Get/set value list option type
        /// </summary>
        public SliderValueListOptionType ValueListOptionType { get; set; }

        /// <summary>
        /// Load configuration data for this item from the supplied <see cref="DataRow"/>.
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadBaseObjectData(DataRow data)
        {
            base.LoadBaseObjectData(data);

            ValueType =
                (SliderValueType)
                Enum.Parse(typeof(SliderValueType), DbUtility.GetValueFromDataRow(data, "ValueType", SliderValueType.NumberRange.ToString()));

            MinValue = DbUtility.GetValueFromDataRow<int?>(data, "MinValue", null);
            MaxValue = DbUtility.GetValueFromDataRow<int?>(data, "MaxValue", null);
            StepSize = DbUtility.GetValueFromDataRow<int?>(data, "StepSize", null);
            DefaultValue = DbUtility.GetValueFromDataRow<int?>(data, "DefaultValue", null);

            ValueListOptionType =
                (SliderValueListOptionType)
                Enum.Parse(typeof(SliderValueListOptionType),
                           DbUtility.GetValueFromDataRow(data, "ValueListOptionType", SliderValueListOptionType.Text.ToString()));
        }

        /// <summary>
        /// Create an instance of a slider item based on this configuration
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new Slider();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void SetDefaults(Template template)
        {
            ValueType = SliderValueType.NumberRange;
            ValueListOptionType = SliderValueListOptionType.Text;
            MinValue = 0;
            MaxValue = 100;
            StepSize = 1;
        }

        /// <summary>
        /// Create the item
        /// </summary>
        /// <param name="transaction"></param>
        protected override void Create(IDbTransaction transaction)
        {
            base.Create(transaction);

            if (ID <= 0)
            {
                throw new ApplicationException("No DataID specified.");
            }

            Database db = DatabaseFactory.CreateDatabase();

            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertSlider");
            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("TextID", DbType.String, TextID);
            command.AddInParameter("SubTextID", DbType.String, SubTextID);
            command.AddInParameter("ValueType", DbType.String, ValueType.ToString());
            command.AddInParameter("MinValue", DbType.Int32, MinValue);
            command.AddInParameter("MaxValue", DbType.Int32, MaxValue);
            command.AddInParameter("StepSize", DbType.Int32, StepSize);
            command.AddInParameter("DefaultValue", DbType.Int32, DefaultValue);
            command.AddInParameter("ValueListOptionType", DbType.String, ValueListOptionType);

            db.ExecuteNonQuery(command, transaction);

            UpdateLists(transaction);
        }

        /// <summary>
        /// Update an instance of a Slider configuration in the persistent store
        /// </summary>
        /// <param name="t"></param>
        protected override void Update(IDbTransaction t)
        {
            base.Update(t);

            if (ID <= 0)
            {
                throw new ApplicationException("No DataID specified.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateSlider");

            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("TextID", DbType.String, TextID);
            command.AddInParameter("SubTextID", DbType.String, SubTextID);
            command.AddInParameter("ValueType", DbType.String, ValueType.ToString());
            command.AddInParameter("MinValue", DbType.Int32, MinValue);
            command.AddInParameter("MaxValue", DbType.Int32, MaxValue);
            command.AddInParameter("StepSize", DbType.Int32, StepSize);
            command.AddInParameter("DefaultValue", DbType.Int32, DefaultValue);
            command.AddInParameter("ValueListOptionType", DbType.String, ValueListOptionType);

            db.ExecuteNonQuery(command, t);

            UpdateLists(t);
        }

        /// <summary>
        /// Ensure a max of one option is selected by default
        /// </summary>
        public override ReadOnlyCollection<ListOptionData> Options
        {
            get
            {
                bool selected = false;
                ReadOnlyCollection<ListOptionData> options = base.Options;

                foreach (ListOptionData option in options)
                {
                    if (option.IsDefault)
                    {
                        if (selected)
                        {
                            option.IsDefault = false;
                        }

                        selected = true;
                    }
                }

                return options;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteItemTypeSpecificXml(XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
        {
            base.WriteItemTypeSpecificXml(writer, externalDataCallback);

            writer.WriteElementString("ValueType", ValueType.ToString());
            writer.WriteElementString("ValueListOptionType", ValueListOptionType.ToString());
            writer.WriteElementString("MinValue", MinValue.ToString());
            writer.WriteElementString("MaxValue", MaxValue.ToString());
            writer.WriteElementString("StepSize", StepSize.ToString());
            writer.WriteElementString("DefaultValue", DefaultValue.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        protected override void ReadItemTypeSpecificXml(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
        {
            base.ReadItemTypeSpecificXml(xmlNode, callback, creator);
            
            var enumString = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("ValueType"));

            if (string.IsNullOrEmpty(enumString))
            {
                enumString = SliderValueType.NumberRange.ToString();
            }

            ValueType =
                (SliderValueType)
                Enum.Parse(typeof(SliderValueType), enumString);

            enumString = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("ValueListOptionType"));

            if (string.IsNullOrEmpty(enumString))
            {
                enumString = SliderValueListOptionType.Text.ToString();
            }

            ValueListOptionType =
                (SliderValueListOptionType)
                Enum.Parse(typeof(SliderValueListOptionType), enumString);

            MinValue = XmlUtility.GetNodeInt(xmlNode.SelectSingleNode("MinValue"));
            MaxValue = XmlUtility.GetNodeInt(xmlNode.SelectSingleNode("MaxValue"));
            StepSize = XmlUtility.GetNodeInt(xmlNode.SelectSingleNode("StepSize"));
            DefaultValue = XmlUtility.GetNodeInt(xmlNode.SelectSingleNode("DefaultValue"));

            Save();
        }


        /// <summary>
        /// Ruturns the clone of the object
        /// </summary>
        /// <returns></returns>
        public new object Clone()
        {
            var result = base.Clone() as SliderItemData;

            result.ValueListOptionType = ValueListOptionType;
            result.ValueType = ValueType;
            result.MinValue = MinValue;
            result.MaxValue = MaxValue;
            result.StepSize = StepSize;
            result.DefaultValue = DefaultValue;

            return result;
        }
    }

    /// <summary>
    /// Describes slider's value types
    /// </summary>
    [Serializable]
    public enum SliderValueType
    {
        /// <summary>
        /// Survey editor chooses minimum and maximum values for slider.
        /// </summary>
        NumberRange = 1,

        /// <summary>
        /// Survey editor specifies possible values.
        /// </summary>
        ValueList,

        /// <summary>
        /// Each option of value list is a simple text value
        /// </summary>
        Text,

        /// <summary>
        /// Each option of value list is an image
        /// </summary>
        Image
    }

    /// <summary>
    /// Describes slider's value list option types
    /// </summary>
    [Serializable]
    public enum SliderValueListOptionType
    {
        /// <summary>
        /// Each option of value list is a simple text value
        /// </summary>
        Text = 1,

        /// <summary>
        /// Each option of value list is an image
        /// </summary>
        Image
    }
}
