using System.Collections.Generic;
using Checkbox.Analytics;
using Checkbox.Analytics.Filters.Configuration;
using Checkbox.Analytics.Items.Configuration;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Web.Analytics.UI.Rendering;
using Checkbox.Web.Forms.UI.Editing;

namespace Checkbox.Web.Analytics.UI.Editing
{
    /// <summary>
    /// Item editor base
    /// </summary>
    public class UserControlAnalysisItemEditorBase : UserControlItemEditorBase
    {
        private int? _sourceResponseTemplateId;

        /// <summary>
        /// Return reference to filter selector, if any, that will be used to select filters.
        /// </summary>
        public virtual IFilterSelector FilterSelector { get { return null; } }

        /// <summary>
        /// Renderer to show applied filters in preview
        /// </summary>
        public virtual IFilterDisplay AppliedFilterDispay { get { return null; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="pagePosition"></param>
        /// <param name="data"></param>
        /// <param name="currentLanguage"></param>
        /// <param name="surveyLanguages"></param>
        /// <param name="isPagePostBack"></param>
        /// <param name="editMode"></param>
        /// <param name="hidePreview"></param>
        public override void Initialize(int templateId, int pagePosition, ItemData data, string currentLanguage, List<string> surveyLanguages, bool isPagePostBack, EditMode editMode, bool hidePreview)
        {
            base.Initialize(templateId, pagePosition, data, currentLanguage, surveyLanguages, isPagePostBack, editMode, hidePreview);

            var surveyId = GetSourceResponseTemplateId();

            if (data is AnalysisItemData)
            {
                if (surveyId.HasValue && FilterSelector != null)
                {
                    FilterSelector.Initialize(((AnalysisItemData)ItemData).Filters, ResponseTemplateManager.GetResponseTemplate(surveyId.Value).GetFilterDataObjects(), CurrentLanguage, isPagePostBack);
                }

                if (AppliedFilterDispay != null)
                {
                    AppliedFilterDispay.InitializeAndBind(((AnalysisItemData)ItemData).Filters, currentLanguage);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void UpdateData()
        {
            base.UpdateData();

            if (FilterSelector != null && ItemData is AnalysisItemData)
            {
                var appliedFilters = new List<FilterData>(FilterSelector.GetAppliedFilters());

                ((AnalysisItemData)ItemData).ClearFilters();

                foreach (FilterData appliedFilter in appliedFilters)
                {
                    ((AnalysisItemData)ItemData).AddFilter(appliedFilter);
                }

                ((AnalysisItemData)ItemData).SaveFilters();
            }
        }


        /// <summary>
        /// Get source survey id
        /// </summary>
        /// <returns></returns>
        public int? GetSourceResponseTemplateId()
        {
            //If item is in a survey, templateId represents survey id.  Otherwise
            // it respresents report id.
            if (EditMode == EditMode.Survey)
            {
                return TemplateId;
            }

            if (!_sourceResponseTemplateId.HasValue
                && TemplateId > 0)
            {
                LightweightAnalysisTemplate lightweightTemplate = AnalysisTemplateManager.GetLightweightAnalysisTemplate(TemplateId);
                
                if (lightweightTemplate != null)
                {
                    _sourceResponseTemplateId = lightweightTemplate.ResponseTemplateId;
                }
            }

            return _sourceResponseTemplateId;
        }
    }
}
