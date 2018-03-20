using System;
using System.Linq;
using Checkbox.Common;
using Checkbox.Forms.Data;
using Checkbox.Globalization.Text;
using Checkbox.Web.Analytics.UI.Rendering;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.ItemRenderers
{
    public partial class NetPromoterScoreTable : UserControlAnalysisItemRendererBase
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

            _colDetractors.Visible = ShowDetractors;
            _colNetPromoterScore.Visible = ShowNetPromoterScore;
            _colPassive.Visible = ShowPassive;
            _colPromoters.Visible = ShowPromoters;
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
        /// Determine if promoters should be shown
        /// </summary>
        protected bool ShowPromoters { get { return Model.AppearanceData["ShowPromoters"].Equals("True", StringComparison.InvariantCultureIgnoreCase); } }

        /// <summary>
        /// Determine if passive should be shown
        /// </summary>
        protected bool ShowPassive { get { return Model.AppearanceData["ShowPassive"].Equals("True", StringComparison.InvariantCultureIgnoreCase); } }

        /// <summary>
        /// Determine if detractors should be shown
        /// </summary>
        protected bool ShowDetractors { get { return Model.AppearanceData["ShowDetractors"].Equals("True", StringComparison.InvariantCultureIgnoreCase); } }

        /// <summary>
        /// Determine if net promoter score should be shown
        /// </summary>
        protected bool ShowNetPromoterScore { get { return Model.AppearanceData["ShowNetPromoterScore"].Equals("True", StringComparison.InvariantCultureIgnoreCase); } }

        /// <summary>
        /// 
        /// </summary>
        protected double GetDetractors(int itemId)
        {
            return Model.CalculateResult.First(p => (p.ResultKey.Equals(itemId.ToString())) && p.ResultText.Equals("Detractors")).ResultValue;
        }

        /// <summary>
        /// 
        /// </summary>
        protected double GetPassives(int itemId)
        {
            return Model.CalculateResult.First(p => (p.ResultKey.Equals(itemId.ToString())) && p.ResultText.Equals("Passives")).ResultValue;
        }

        /// <summary>
        /// 
        /// </summary>
        protected double GetPromoters(int itemId)
        {
            return Model.CalculateResult.First(p => (p.ResultKey.Equals(itemId.ToString())) && p.ResultText.Equals("Promoters")).ResultValue;
        }

        /// <summary>
        /// 
        /// </summary>
        protected double GetNetPromoterScore(int itemId)
        {
            return Model.CalculateResult.First(p => (p.ResultKey.Equals(itemId.ToString())) && p.ResultText.Equals("Net Promoter Score")).ResultValue;
        }
    }
}