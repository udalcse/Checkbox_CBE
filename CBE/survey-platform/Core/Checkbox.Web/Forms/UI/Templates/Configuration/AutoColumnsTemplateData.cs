using System;
using System.Data;
using System.Collections.Generic;
using Prezza.Framework.Data;
using Checkbox.Forms.PageLayout.Configuration;

namespace Checkbox.Web.Forms.UI.Templates.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class AutoColumnsTemplateData : UserControlLayoutTemplateData
    {

        /// <summary>
        /// Default contstructor
        /// </summary>
        public AutoColumnsTemplateData()
        {
            AddExtendedProperties();
        }

        /// <summary>
        /// Get the type name
        /// </summary>
        public override string TypeName
        {
            get { return "AutoColumns"; }
        }

        /// <summary>
        /// Load data from the data row
        /// </summary>
        /// <param name="data"></param>
        protected override void  LoadBaseObjectData(DataRow data)
        {
 	        base.LoadBaseObjectData(data);

            LoadExtendedProperties(data);
        }

        /// <summary>
        /// Load extended property data
        /// </summary>
        /// <param name="data"></param>
        private void LoadExtendedProperties(DataRow data)
        {
            List<string> propertyNames = GetExtendedPropertyNames();

            foreach (string propertyName in propertyNames)
            {
                SetPropertyValue(propertyName, DbUtility.GetValueFromDataRow<object>(data, propertyName, null));
            }
        }

        /// <summary>
        /// Add extended property to the list
        /// </summary>
        private void AddExtendedProperties()
        {
            var prop1 = new LayoutTemplateExtendedProperty
            {
                Name = "Columns",
                MinValue = 1,
                Type = LayoutTemplateExtendedPropertyType.Integer,
                PossibleValues = new List<string>(),
                Value = 1
            };
            AddExtendedProperty(prop1);

            var prop2 = new LayoutTemplateExtendedProperty
            {
                Name = "RepeatDirection",
                Type = LayoutTemplateExtendedPropertyType.String,
                PossibleValues = new List<string>(new[] { "Vertical", "Horizontal" }),
                Value = "Vertical"
            };
            AddExtendedProperty(prop2);

            var prop3 = new LayoutTemplateExtendedProperty
            {
                Name = "RoundedCorners",
                Type = LayoutTemplateExtendedPropertyType.String,
                PossibleValues = new List<string>(new[] { "No", "Yes" }),
                Value = "No"
            };
            AddExtendedProperty(prop3);

            var prop4 = new LayoutTemplateExtendedProperty
            {
                Name = "BackgroundColor",
                Type = LayoutTemplateExtendedPropertyType.Color,
                Value = ""
            };
            AddExtendedProperty(prop4);

            var prop5 = new LayoutTemplateExtendedProperty
            {
                Name = "BorderColor",
                Type = LayoutTemplateExtendedPropertyType.Color,
                Value = ""
            };
            AddExtendedProperty(prop5);

            var prop6Values = new List<string>();

            for (int i = 0; i <= 4; i++)
            {
                prop6Values.Add(i.ToString());
            }

            var prop6 = new LayoutTemplateExtendedProperty
            {
                Name = "BorderWidth",
                Value = LayoutTemplateExtendedPropertyType.String,
                PossibleValues = prop6Values
            };

            prop6.Value = "1";
            AddExtendedProperty(prop6);

        }

        /// <summary>
        /// Get the command to load the item
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        protected override DBCommandWrapper GetLoadCommand(Database db)
        {
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_LayoutTemplate_AutoColumns_Get");
            command.AddInParameter("LayoutTemplateID", DbType.Int32, ID.Value);

            return command;
        }

        /// <summary>
        /// Get the command to delete the item
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        protected override DBCommandWrapper GetDeleteCommand(Database db)
        {
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_LayoutTemplate_AutoColumns_Delete");
            command.AddInParameter("LayoutTemplateID", DbType.Int32, ID.Value);

            return command;
        }

        /// <summary>
        /// Get the command to update/insert the item 
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        protected override DBCommandWrapper GetUpSertCommand(Database db)
        {
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_LayoutTemplate_AutoColumns_UpSert");
            command.AddInParameter("LayoutTemplateID", DbType.Int32, ID.Value);
            command.AddInParameter("TemplateControlSource", DbType.String, ControlSource);

            object columns = GetPropertyValue("Columns");
            object repeatDirection = GetPropertyValue("RepeatDirection");
            object roundedCorners = GetPropertyValue("RoundedCorners");
            object borderWidth = GetPropertyValue("BorderWidth");
            object backgroundColor = GetPropertyValue("BackgroundColor");
            object borderColor = GetPropertyValue("BorderColor");

            //Columns
            command.AddInParameter("Columns", DbType.Int32, columns != null ? Convert.ToInt32(columns) : 1);

            //Repeat Direction
            command.AddInParameter("RepeatDirection", DbType.String, repeatDirection ?? "Vertical");

            //Rounded Corners
            if (roundedCorners != null)
            {
                command.AddInParameter("RoundedCorners", DbType.Boolean, (roundedCorners.ToString().ToLower() == "yes"));
            }
            else
            {
                command.AddInParameter("RoundedCorners", DbType.Boolean, false);
            }

            //BorderWidth
            command.AddInParameter("BorderWidth", DbType.Int32, borderWidth != null ? Convert.ToInt32(borderWidth) : 0);

            //Background color
            command.AddInParameter("BackgroundColor", DbType.String, backgroundColor ?? "");

            //Border color
            command.AddInParameter("BorderColor", DbType.String, borderColor ?? "");

            return command;
        }
    }
}
