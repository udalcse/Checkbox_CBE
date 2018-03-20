using System;
using System.Collections.Generic;
using Checkbox.Analytics.Filters.Configuration;
using Checkbox.Analytics.Filters.Validators;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Globalization;
using Checkbox.Globalization.Text;
using Checkbox.Web;
using Checkbox.Web.Analytics.UI.Editing;
using Checkbox.Web.Page;

namespace CheckboxWeb.Forms.Surveys.Reports.Filters
{
    public partial class Add : SecuredPage
    {
        private Int32 SurveyId { get; set; }

        private FilterEditor _filterEditor;
        private ResponseTemplate _currentSurvey;

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

            SurveyId = Int32.Parse(GetQueryStringValue("s", "-1"));

            _filterEditor = new FilterEditor(CurrentSurvey);
            //_filterEditor.InputControlWidth = System.Web.UI.WebControls.Unit.Pixel(620);
            _filterEditorPlace.Controls.Add(_filterEditor);
            _filterEditor.AllowItemChange = true;

            Master.SetTitle(String.Format(WebTextManager.GetText("/pageText/forms/surveys/reports/filters/add.aspx/title")));

            //Show/hide none available message
            if (_filterEditor.SourceItemListEmpty)
            {
                _noFilterableItemsLbl.Visible = true;
                _filterEditorPlace.Visible = false;

                Master.OkVisible = false;
                Master.CancelTextId = "/common/close";
            }
            else
            {
                _noFilterableItemsLbl.Visible = false;
                _filterEditorPlace.Visible = true;

                Master.OkClick += Master_OkClick;
                Master.CancelClick += Master_CancelClick;
            }

            _datePickerLocaleResolver.Source = "~/Resources/" + GlobalizationManager.GetDatePickerLocalizationFile();
        }

        void Master_CancelClick(object sender, EventArgs e)
        {
            Master.CloseDialog(null);
        }

        void Master_OkClick(object sender, EventArgs e)
        {
            Dictionary<string, string> args = null;
            try
            {
                string errorMessage;

                //check the value if it's required
                if (_filterEditor.Operator.HasValue && _filterEditor.Operator.Value != Checkbox.Forms.Logic.LogicalOperator.Answered
                        && _filterEditor.Operator.Value != Checkbox.Forms.Logic.LogicalOperator.NotAnswered)
                {
                    if (_filterEditor.SourceType == "ITEM" && _filterEditor.SourceItemID.HasValue &&
                        !Validate(ItemConfigurationManager.GetConfigurationData(_filterEditor.SourceItemID.Value),
                        _filterEditor.Value, out errorMessage))
                    {
                        Page.ClientScript.RegisterClientScriptBlock(GetType(), "validation", "$(function(){ showError('" + errorMessage + "'); });", true);
                        return;
                    }
                }

                FilterData filterData = FilterFactory.CreateFilterData(_filterEditor.SourceType);
                CreateFilter(filterData);
                CurrentSurvey.AddFilter(filterData);
                CurrentSurvey.SaveFilters();

                var newReportId = Session["NewReportId"] as int?;

                args = new Dictionary<string, string>
                           {
                               {"op", "addFilter"},
                               {"result", "ok"},
                               {"filterId", filterData.ID.ToString()}
                           };
            }
            catch (Exception ex)
            {
                args = new Dictionary<string, string>
                           {
                               {"op", "addFilter"},
                               {"result", "fail"},
                               {"filterId", "0"}
                           };                
            }

            Master.CloseDialog(args);
        }

        /// <summary>
        /// Update the filter with editor options
        /// </summary>
        /// <param name="filterData"></param>
        private void CreateFilter(FilterData filterData)
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