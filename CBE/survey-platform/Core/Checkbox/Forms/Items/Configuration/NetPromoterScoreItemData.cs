using System;
using System.Data;
using Checkbox.Globalization.Text;
using Prezza.Framework.Data;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class NetPromoterScoreItemData : RatingScaleItemData
    {
        /// <summary>
        /// Create an instance of a radio button scale business object
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new NetPromoterScore();
        }

        /// <summary>
        /// Get name of table containing data
        /// </summary>
        public override string ItemDataTableName { get { return "NetPromoterScoreData"; } }

        /// <summary>
        /// 
        /// </summary>
        public override void SetDefaults(Template template)
        {
            StartValue = 0;
            EndValue = 10;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void SetDefaultTexts()
        {
            //set default "net promoter score" text
            TextManager.SetText(TextID, TextManager.GetText("/controlText/netPromoterScore/defaultQuestionText"));
        }

        /// <summary>
        /// Load item data from the provided data row
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadBaseObjectData(DataRow data)
        {
            base.LoadBaseObjectData(data);

            _startValue = DbUtility.GetValueFromDataRow(data, "StartValue", 0);
            _endValue = DbUtility.GetValueFromDataRow(data, "EndValue", 10);
            IsRequired = DbUtility.GetValueFromDataRow(data, "IsRequired", 0) == 1;
            _displayNotApplicable = false;
        }
    }
}
