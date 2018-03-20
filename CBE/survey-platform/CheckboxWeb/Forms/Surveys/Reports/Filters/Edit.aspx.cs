using System;
using System.Collections.Generic;
using Checkbox.Analytics.Filters.Configuration;
using Checkbox.Analytics.Filters.Validators;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Globalization;
using Checkbox.Web;
using Checkbox.Web.Analytics.UI.Editing;
using Checkbox.Web.Page;

namespace CheckboxWeb.Forms.Surveys.Reports.Filters
{
    public partial class Edit : SecuredPage
    {
        private Int32 SurveyId { get; set; }

        private FilterEditor _filterEditor;
        private ResponseTemplate _currentSurvey;

        [QueryParameter("filterId")]
        public Int32 _filterId;

        private ResponseTemplate CurrentSurvey
        {
            get { return _currentSurvey ?? (_currentSurvey = ResponseTemplateManager.GetResponseTemplate(SurveyId)); }
        }

        /// <summary>
        /// Initialize form and bind data
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();
            Master.OkClick += Master_OkClick;
            Master.CancelClick += Master_CancelClick;
            SurveyId = Int32.Parse(GetQueryStringValue("s", "-1"));

            _filterEditor = new FilterEditor(CurrentSurvey);
            _filterEditorPlace.Controls.Add(_filterEditor);
            _filterEditorPlace.Visible = true;

            Master.SetTitle(WebTextManager.GetText("/pageText/forms/surveys/reports/filters/edit.aspx/title") + CurrentSurvey.Name);

            _filterEditor.AllowItemChange = false;

            if (!Page.IsPostBack)
            {
                BindEditorToFilter(FilterFactory.GetFilterData(_filterId));
            }

            var resource = GlobalizationManager.GetDatePickerLocalizationFile();
            if (!string.IsNullOrEmpty(resource))
                _datePickerLocaleResolver.Source = "~/Resources/" + resource;
        }
        void Master_CancelClick(object sender, EventArgs e)
        {
            var args = new Dictionary<string, string>
                           {
                               {"op", "editFilter"}
                           };
            Master.CloseDialog(args);
        }

        void Master_OkClick(object sender, EventArgs e)
        {
            //check the value if it's required
            if (_filterEditor.Operator.HasValue && _filterEditor.Operator.Value != Checkbox.Forms.Logic.LogicalOperator.Answered
                    && _filterEditor.Operator.Value != Checkbox.Forms.Logic.LogicalOperator.NotAnswered)
            {
                //validation
                string errorMessage;
                if (_filterEditor.SourceType == "ITEM" && _filterEditor.SourceItemID.HasValue &&
                    !Validate(ItemConfigurationManager.GetConfigurationData(_filterEditor.SourceItemID.Value),
                    _filterEditor.Value, out errorMessage))
                {
                    Page.ClientScript.RegisterClientScriptBlock(GetType(), "validation", "$(function(){ showError('" + errorMessage + "'); });", true);
                    return;
                }
            }

            FilterData filterData = FilterFactory.GetFilterData(_filterId);
            UpdateFilter(filterData);

            //Re-add the filter to update it
            CurrentSurvey.AddFilter(filterData);

            CurrentSurvey.SaveFilters();

            var args = new Dictionary<string, string>
                           {
                               {"op", "editFilter"}
                           };
            Master.CloseDialog(args);
        }

        /// <summary>
        /// Bind the filter editor to the selected filter
        /// </summary>
        /// <param name="filterData"></param>
        private void BindEditorToFilter(FilterData filterData)
        {
            if (filterData == null)
            {
                return;
            }

            _filterEditor.SourceType = filterData.FilterTypeName;

            _filterEditor.UpdateOperators();

            _filterEditor.Operator = filterData.Operator;

            if (filterData is ItemFilterData)
            {
                _filterEditor.SourceItemID = ((ItemFilterData)filterData).ItemId;
            }
            else if (filterData is ProfileFilterData)
            {
                _filterEditor.SourceProfileProperty = ((ResponseFilterData)filterData).PropertyName;
            }
            else if (filterData is ResponseFilterData)
            {
                _filterEditor.SourceResponseProperty = ((ResponseFilterData)filterData).PropertyName;
            }

            if (filterData.Value != null)
            {
                var dateTime = Utilities.GetDate(filterData.Value.ToString());
                _filterEditor.Value = dateTime.HasValue ? 
                    GlobalizationManager.FormatTheDate(dateTime.Value) : filterData.Value.ToString();
            }

            _filterEditor.CheckDateTimePicker();
        }

        /// <summary>
        /// Update the filter with editor options
        /// </summary>
        /// <param name="filterData"></param>
        private void UpdateFilter(FilterData filterData)
        {
            filterData.Operator = _filterEditor.Operator.Value;
            filterData.Value = _filterEditor.Value;

            if (filterData is ItemFilterData
                && _filterEditor.SourceItemID.HasValue
                && _filterEditor.AllowItemChange)
            {
                ((ItemFilterData)filterData).ItemId = _filterEditor.SourceItemID.Value;
            }
            else if (filterData is ProfileFilterData)
            {
                ((ProfileFilterData)filterData).PropertyName = _filterEditor.SourceProfileProperty;
            }
            else if (filterData is ResponseFilterData)
            {
                ((ResponseFilterData)filterData).PropertyName = _filterEditor.SourceResponseProperty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="errorMessage"></param>
        /// <param name="itemData"></param>
        /// <returns></returns>
        private bool Validate(ItemData itemData, string value, out string errorMessage)
        {
            errorMessage = null;

            if (itemData is SliderItemData)
                return (new NumericSliderFilterValidator()).Validate(itemData, value, out errorMessage);

            return true;
        }
    }
}