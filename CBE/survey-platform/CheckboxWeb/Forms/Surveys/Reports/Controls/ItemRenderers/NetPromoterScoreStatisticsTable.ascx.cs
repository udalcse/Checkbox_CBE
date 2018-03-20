using System;
using System.Linq;
using Checkbox.Common;
using Checkbox.Forms.Data;
using Checkbox.Globalization.Text;
using Checkbox.Web.Analytics.UI.Rendering;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.ItemRenderers
{
    public partial class NetPromoterScoreStatisticsTable : UserControlAnalysisItemRendererBase
    {
        /// <summary>
        /// Override OnInit to add styles to the page
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (Page != null && Page.Header != null)
            {
                Page.Header.Controls.Add(_dataControlStyles);
            }
            
            _colMinHeader.Visible = ShowMin;
            _colMaxHeader.Visible = ShowMax;
            _colAverageHeader.Visible = ShowAverage;
            _colVarianceHeader.Visible = ShowVariance;
            _colStdDevHeader.Visible = ShowStandartDeviation;
            _colResponsesHeader.Visible = ShowTotalResponses;
            _colRespondentsHeader.Visible = ShowTotalRespondents;
        }

        protected override void InlineBindModel()
        {
            base.InlineBindModel();

            _rowRepeater.DataSource = Model.SourceItems.OrderBy(i => i.ItemPosition).OrderBy(i => i.PagePosition).Select(p => p.ItemId);
            _rowRepeater.DataBind();
        }

        /// <summary>
        /// Get language code for text
        /// </summary>
        public string LanguageCode
        {
            get
            {
                return string.IsNullOrEmpty(Model.InstanceData["LanguageCode"])
                    ? TextManager.DefaultLanguage
                    : Model.InstanceData["LanguageCode"];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        protected string GetItemText(int itemId)
        {
            return Utilities.StripHtml(SurveyMetaDataProxy.GetItemText(itemId, LanguageCode, Model.UseAliases, true), 32);
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool IsVisible(string field)
        {
            return Model.AppearanceData[field].Equals("True", StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// 
        /// </summary>
        protected double GetValue(int itemId, string field)
        {
            return Model.CalculateResult.First(p => (p.ResultKey.Equals(itemId.ToString())) && p.ResultText.Equals(field)).ResultValue;
        }

        protected bool ShowMin
        {
            get
            {
                return Model.AppearanceData["ShowMin"].Equals("True", StringComparison.InvariantCultureIgnoreCase); 
            }
        }

        protected bool ShowMax
        {
            get
            {
                return Model.AppearanceData["ShowMax"].Equals("True", StringComparison.InvariantCultureIgnoreCase); 
            }
        }

        protected bool ShowAverage
        {
            get
            {
                return Model.AppearanceData["ShowAverage"].Equals("True", StringComparison.InvariantCultureIgnoreCase); 
            }
        }

        protected bool ShowVariance
        {
            get
            {
                return Model.AppearanceData["ShowVariance"].Equals("True", StringComparison.InvariantCultureIgnoreCase); 
            }
        }

        protected bool ShowStandartDeviation
        {
            get
            {
                return Model.AppearanceData["ShowStandartDeviation"].Equals("True", StringComparison.InvariantCultureIgnoreCase); 
            }
        }

        protected bool ShowTotalResponses
        {
            get
            {
                return Model.AppearanceData["ShowTotalResponses"].Equals("True", StringComparison.InvariantCultureIgnoreCase); 
            }
        }

        protected bool ShowTotalRespondents
        {
            get
            {
                return Model.AppearanceData["ShowTotalRespondents"].Equals("True", StringComparison.InvariantCultureIgnoreCase); 
            }
        }

    }
}