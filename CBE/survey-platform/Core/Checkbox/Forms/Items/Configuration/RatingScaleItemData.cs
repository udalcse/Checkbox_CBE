using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Prezza.Framework.Common;
using Prezza.Framework.Data;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Item data for a radio button scale
    /// </summary>
    [Serializable]
    public class RatingScaleItemData : Select1Data
    {
        protected int _startValue;
        protected int _endValue;
        protected bool _displayNotApplicable;

        /// <summary>
        /// Get name of table containing data
        /// </summary>
        public override string ItemDataTableName { get { return "RatingScaleData"; } }

        /// <summary>
        /// Load data
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_ItemData_GetRS"; } }

        /// <summary>
        /// Get/set the scale start value
        /// </summary>
        public int StartValue
        {
            get { return _startValue; }
            set
            {
                _startValue = value;
                GenerateOptions();
            }
        }

        /// <summary>
        /// Get/set the scale end value
        /// </summary>
        public int EndValue
        {
            get { return _endValue; }
            set
            {
                _endValue = value;
                GenerateOptions();
            }
        }

        /// <summary>
        /// Get/set whethere or not the N/A option is enabled.
        /// </summary>
        public bool DisplayNotApplicable
        {
            get { return _displayNotApplicable; }
            set
            {
                _displayNotApplicable = value;
                GenerateOptions();
            }
        }

        /// <summary>
        /// Get text id of the scale start text
        /// </summary>
        public string StartTextID
        {
            get { return GetTextID("startText"); }
        }

        /// <summary>
        /// Get text id of the scale mid text
        /// </summary>
        public string MidTextID
        {
            get { return GetTextID("midText"); }
        }

        /// <summary>
        /// Get text id of the scale end text
        /// </summary>
        public string EndTextID
        {
            get { return GetTextID("endText"); }
        }

        /// <summary>
        /// Get text id of the n/a text
        /// </summary>
        public string NotApplicableTextID
        {
            get { return GetTextID("otherText"); }
        }

        /// <summary>
        /// Get/set whether options are randomized
        /// </summary>
        public override bool Randomize
        {
            get
            {
                return false;
            }
            set
            {
                //Do nothing;
            }
        }

        /// <summary>
        /// Get/set whether "other" options are allowed
        /// </summary>
        public override bool AllowOther
        {
            get { return DisplayNotApplicable; } 
            set
            {
                //Do nothing;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="otherOption"></param>
        protected override void UpdateOtherOption(ListOptionData otherOption) { }

        /// <summary>
        /// 
        /// </summary>
        public override void SetDefaults(Template template)
        {
            StartValue = 1;
            EndValue = 5;
        }

        /// <summary>
        /// Create an instance of radio button scale data
        /// </summary>
        /// <param name="t"></param>
        protected override void Create(IDbTransaction t)
        {
            base.Create(t);

            if (ID <= 0)
            {
                throw new Exception("DataID must be specified to create item data.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertRS");

            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("StartValue", DbType.Int32, _startValue);
            command.AddInParameter("EndValue", DbType.Int32, _endValue);
            command.AddInParameter("TextID", DbType.String, TextID);
            command.AddInParameter("SubTextID", DbType.String, SubTextID);
            command.AddInParameter("IsRequired", DbType.Int32, IsRequired ? 1 : 0);
            command.AddInParameter("DisplayNotApplicableText", DbType.Boolean, DisplayNotApplicable);
            command.AddInParameter("StartTextID", DbType.String, StartTextID);
            command.AddInParameter("MidTextID", DbType.String, MidTextID);
            command.AddInParameter("EndTextID", DbType.String, EndTextID);
            command.AddInParameter("NotApplicableTextID", DbType.String, NotApplicableTextID);

            db.ExecuteNonQuery(command, t);
        }

        /// <summary>
        /// Update existing radio button scale data
        /// </summary>
        /// <param name="t"></param>
        protected override void Update(IDbTransaction t)
        {
            base.Update(t);

            if (ID <= 0)
            {
                throw new Exception("DataID must be specified to update item data.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateRS");

            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("StartValue", DbType.Int32, _startValue);
            command.AddInParameter("EndValue", DbType.Int32, _endValue);
            command.AddInParameter("TextID", DbType.String, TextID);
            command.AddInParameter("SubTextID", DbType.String, SubTextID);
            command.AddInParameter("IsRequired", DbType.Int32, IsRequired ? 1 : 0);
            command.AddInParameter("DisplayNotApplicableText", DbType.Boolean, DisplayNotApplicable);
            command.AddInParameter("StartTextID", DbType.String, StartTextID);
            command.AddInParameter("MidTextID", DbType.String, MidTextID);
            command.AddInParameter("EndTextID", DbType.String, EndTextID);
            command.AddInParameter("NotApplicableTextID", DbType.String, NotApplicableTextID);


            db.ExecuteNonQuery(command, t);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string[] GetTextIdSuffixes()
        {
            return new List<string>(base.GetTextIdSuffixes()) {"startText", "midText", "endText", "otherText"}.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteItemTypeSpecificXml(XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
        {
            base.WriteItemTypeSpecificXml(writer, externalDataCallback);

            writer.WriteElementString("StartValue", _startValue.ToString());
            writer.WriteElementString("EndValue", _endValue.ToString());
            writer.WriteElementString("IsRequired", IsRequired.ToString());
            writer.WriteElementString("DisplayNotApplicableText", DisplayNotApplicable.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        protected override void ReadItemTypeSpecificXml(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
        {
            base.ReadItemTypeSpecificXml(xmlNode, callback, creator);

            _startValue = XmlUtility.GetNodeInt(xmlNode.SelectSingleNode("StartValue")) ?? 1;
            _endValue = XmlUtility.GetNodeInt(xmlNode.SelectSingleNode("EndValue")) ?? 5;
            IsRequired = XmlUtility.GetNodeBool(xmlNode.SelectSingleNode("IsRequired"));
            DisplayNotApplicable =XmlUtility.GetNodeBool(xmlNode.SelectSingleNode("DisplayNotApplicableText"));
        }

        /// <summary>
        /// Generate options for the item
        /// </summary>
        protected virtual void GenerateOptions()
        {
            //Ensure item options match settings
            int start;
            int end;
            bool countDown = false;

            if (_startValue > _endValue)
            {
                start = _endValue;
                end = _startValue;
                countDown = true;
            }
            else
            {
                start = _startValue;
                end = _endValue;
            }

            //Remove/Update options that are not N/A options
            var optionsToRemove = new List<ListOptionData>();
            var optionsToUpdate = new List<ListOptionData>();

            var optionPoints = new List<double>();
            IEnumerable<ListOptionData> optionList = Options.Where(option => !option.IsOther);

            foreach (ListOptionData option in optionList)
            {
                optionPoints.Add(option.Points);

                //For a rating scale, points == value
                if ((option.Points < start || option.Points > end))
                {
                    optionsToRemove.Add(option);
                }
                else
                {
                    optionsToUpdate.Add(option);
                }
            }

            //Remove options
            foreach (ListOptionData option in optionsToRemove)
            {
                RemoveOption(option.OptionID);
            }

            //Update option positions
            foreach (ListOptionData option in optionsToUpdate)
            {
                int newPosition;

                if (countDown)
                {
                    newPosition = (end - (int)option.Points) + 1;
                }
                else
                {
                    newPosition = ((int)option.Points - start) + 1;
                }

                option.Position = newPosition;
                UpdateOption(option.OptionID, option.Alias, option.Category, option.IsDefault, newPosition, option.Points, option.IsNoneOfAbove, false, option.ContentID);
            }

            //Add any new options necessary
            for (int i = start; i <= end; i++)
            {
                bool found = optionsToUpdate.Any(option => (int)option.Points == i);

                if (found)
                {
                    continue;
                }

                int position;

                if (countDown)
                {
                    position = (end - i) + 1;
                }
                else
                {
                    position = (i - start) + 1;
                }

                AddOption(string.Empty, string.Empty, false, position, i, false, false, null);
            }

            GenerateNotApplicableOption();
        }

        /// <summary>
        /// Generate N/A option for the item
        /// </summary>
        private void GenerateNotApplicableOption()
        {
            //Get options.  There should never be more than 1 na option, but do check to handle some legacy
            // cases where multiple n/a options existed.
            var naOptions = new List<ListOptionData>(Options.Where(option => option.IsOther));

            //If no na option, remove all options
            if (!DisplayNotApplicable)
            {
                foreach (ListOptionData naOption in naOptions)
                {
                    RemoveOption(naOption.OptionID);
                }

                return;
            }

            //Remove extraneous options
            for (int i = 1; i < naOptions.Count; i++)
            {
                RemoveOption(naOptions[i].OptionID);
            }


            //N/A Option
            ListOptionData naOptionData = naOptions.Count == 0
                ? AddOption(string.Empty, string.Empty, false, NextOptionPosition(), 0, true, false, null)
                : naOptions[0];

            //Ensure na option is last option in cases where range was expanded/contracted
            UpdateOption(naOptionData.OptionID, naOptionData.Alias, naOptionData.Category, naOptionData.IsDefault, NextOptionPosition(), 0, true, false, naOptionData.ContentID);
        }

        /// <summary>
        /// Returns the next avialable option postion.
        /// </summary>
        /// <returns></returns>
        private int NextOptionPosition()
        {
            if (Options.Count == 0) { return 1; }

            int position = Options[0].Position;

            for (int i = 1; i < Options.Count; i++)
            {
                if (position < Options[i].Position)
                {
                    position = Options[i].Position;
                }
            }

            return position + 1;
        }

        ///// <summary>
        ///// Get radio button scale data
        ///// </summary>
        ///// <returns></returns>
        //protected override DataSet GetConcreteConfigurationDataSet()
        //{
        //    if (ID <= 0)
        //    {
        //        throw new Exception("No DataID specified.");
        //    }

        //    try
        //    {
        //        Database db = DatabaseFactory.CreateDatabase();
        //        DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_GetRS");
        //        command.AddInParameter("ItemID", DbType.Int32, ID);

        //        DataSet ds = new DataSet();
        //        db.LoadDataSet(command, ds, ConfigurationDataTableNames);

        //        if (ds.Tables.Contains("ItemOptions"))
        //        {
        //            ds.Tables["ItemOptions"].PrimaryKey = new[] { ds.Tables["ItemOptions"].Columns["OptionID"], ds.Tables["ItemOptions"].Columns["ItemID"] };
        //        }

        //        if (ds.Tables.Contains("ItemLists"))
        //        {
        //            ds.Tables["ItemLists"].PrimaryKey = new[] { ds.Tables["ItemLists"].Columns["ItemID"], ds.Tables["ItemLists"].Columns["ListID"] };
        //        }

        //        return ds;
        //    }
        //    catch (Exception ex)
        //    {
        //        bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessProtected");

        //        if (rethrow)
        //        {
        //            throw;
        //        }

        //        return null;
        //    }
        //}

        /// <summary>
        /// Load item data from the provided data row
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadBaseObjectData(DataRow data)
        {
            base.LoadBaseObjectData(data);

            _startValue = DbUtility.GetValueFromDataRow(data, "StartValue", 1);
            _endValue = DbUtility.GetValueFromDataRow(data, "EndValue", 5);
            IsRequired = DbUtility.GetValueFromDataRow(data, "IsRequired", 0) == 1;
            _displayNotApplicable = DbUtility.GetValueFromDataRow(data, "DisplayNotApplicableText", false);
        }


        /// <summary>
        /// Create an instance of a radio button scale business object
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new RatingScale();
        }


        /// <summary>
        /// Create a text decorator for the item
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override ItemTextDecorator CreateTextDecorator(string languageCode)
        {
            return new RatingScaleItemTextDecorator(this, languageCode);
        }

        /// <summary>
        /// Copy the rating scale item data
        /// </summary>
        /// <returns></returns>
        protected override ItemData Copy()
        {
            var theCopy = (RatingScaleItemData)base.Copy();

            if (theCopy != null)
            {
                theCopy.StartValue = StartValue;
                theCopy.EndValue = EndValue;
                theCopy.DisplayNotApplicable = DisplayNotApplicable;
            }

            return theCopy;
        }
    }
}
