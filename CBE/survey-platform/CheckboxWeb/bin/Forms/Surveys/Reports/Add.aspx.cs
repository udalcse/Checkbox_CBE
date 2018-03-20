using System;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using Checkbox.Analytics;
using Checkbox.Analytics.Items.Configuration;
using Checkbox.Common;
using Checkbox.Globalization.Text;
using Checkbox.Management;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Page;

using Prezza.Framework.ExceptionHandling;
using Checkbox.Security.Principal;
using System.Web;

namespace CheckboxWeb.Forms.Surveys.Reports
{
    public partial class Add : ResponseTemplatePage
    {
        /// <summary>
        /// 
        /// </summary>
        protected override string PageSpecificTitle
        {
            get
            {
                return Utilities.StripHtml(String.Format(WebTextManager.GetText("/pageText/forms/surveys/reports/add.aspx/title"), ResponseTemplate.Name), 64);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override string PageRequiredRolePermission
        {
            get { return "Analysis.Create"; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

			if (!IsPostBack)
			{
				string name = Utilities.AdvancedHtmlDecode(ResponseTemplate.Name) + " report";
				int idx = 1;

				while (AnalysisTemplateManager.AnalysisTemplateExists(name, ResponseTemplateId, null))
					name = Utilities.AdvancedHtmlDecode(ResponseTemplate.Name) + " report " + (++idx);

				_properties.ReportName = name;
			}

            _properties.SurveyId = ResponseTemplateId;
            _properties.SurveyName = Utilities.AdvancedHtmlDecode(ResponseTemplate.Name);
            _properties.IsNewReport = true;
            _properties.DisplaySurveyTitle = ApplicationManager.AppSettings.DisplaySurveyTitle;
            _properties.DisplayPdfExportButton = ApplicationManager.AppSettings.DisplayPdfExportButton;

            //Set up the localized text for wizard navigation elements (can't use inline code in the wizard tag)
            foreach (WizardStep step in _reportWizard.WizardSteps)
            {
                step.Title = WebTextManager.GetText(String.Format("/pageText/forms/surveys/reports/add.aspx/wizardStepTitle/{0}", step.ID));
            }

            //Set title & hide default dialog buttons
            Master.SetTitle(PageSpecificTitle);
            Master.HideDialogButtons();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AnalysisWizard_NextButtonClick(object sender, WizardNavigationEventArgs e)
        {
            if (!Page.IsValid)
            {
                e.Cancel = true;
                return;
            }

            //Reset validation errors
            _properties.NameValidator.Visible = false;

            if ("StartStep".Equals(_reportWizard.WizardSteps[e.CurrentStepIndex].ID, StringComparison.InvariantCultureIgnoreCase))
            {
                _items.UserInputItemsVisibility = AnalysisTemplateManager.ShowUserInputItemsForReport(ResponseTemplateId);

                if (AnalysisTemplateManager.AnalysisTemplateExists(_properties.ReportName, ResponseTemplateId, null))
                {
                    _properties.NameValidator.Visible = true;
                    e.Cancel = true;
                    return;
                }
                if (!_properties.IsWizard)
                {
                    _reportWizard.ActiveStepIndex = 3;
                    return;
                }
            }

            //load the review step values before navigating to it
            if ("OptionsStep".Equals(_reportWizard.WizardSteps[e.CurrentStepIndex].ID, StringComparison.InvariantCultureIgnoreCase))
            {
                _reportTypeReview.Text = WebTextManager.GetText(_properties.IsWizard ? "/pageText/reportProperties.ascx/wizardReport" : "/pageText/reportProperties.ascx/blankReport");

                _reportNameReview.Text = _properties.ReportName;
                _styleTemplateReview.Text = _properties.StyleTemplateName;
                

                _radioButtonsReview.Text = _items.RadioButtonGraphType;
                _checkboxesReview.Text = _items.CheckboxGraphType;
                _sliderReview.Text = _items.SliderGraphType;
                _rankOrderReview.Text = _items.RankOrderGraphType;
                _ratingScaleReview.Text = _items.RatingScaleGraphType;
                _dropDownListReview.Text = _items.DropDownListGraphType;
                _matrixReview.Text = _items.MatrixGraphType;
                _hiddenItemsReview.Text = _items.HiddenItemGraphType;
                _netPromoterScoreReview.Text = _items.NetPromoterScoreGraphType;

                if (AnalysisTemplateManager.ShowUserInputItemsForReport(ResponseTemplateId))
                {
                    _sltReview.Text = _items.SingleLineTextGraphType;
                    _mltReview.Text = _items.MultiLineTextGraphType;
                }
                else
                {
                    _sltReview.Visible = false;
                    _mltReview.Visible = false;
                }

                _useAliaseReview.Text = _options.UseAlias
                                            ? WebTextManager.GetText("/common/yes")
                                            : WebTextManager.GetText("/common/no");

                _multiPageReview.Text = _options.IsSinglePageReport
                                            ? WebTextManager.GetText("/common/yes")
                                            : WebTextManager.GetText("/common/no");

                _itemPositionReview.Text = _options.ItemPosition;
                _maxOptionsReview.Text = _options.MaxOptions.ToString();
            }

            bool isWizard = _properties.IsWizard;

            _reportWizardPanel.Visible = isWizard;
            _optionsPanel.Visible = isWizard;
            _questionsReviewPanel.Visible = isWizard;
            _optionsReviewPanel.Visible = isWizard;

            _blankWizardPanel.Visible = !isWizard;
            _blankOptionsPanel.Visible = !isWizard;
            _questionsReviewNAPanel.Visible = !isWizard;
            _optionsReviewNAPanel.Visible = !isWizard;
        }

        /// <summary>
        /// Registers a script that will reload a master page
        /// </summary>
        protected void reloadReportList()
        {
            if (!ClientScript.IsClientScriptBlockRegistered("reloadReportList"))
                ClientScript.RegisterClientScriptBlock(GetType(), "reloadReportList", "<script type=\"text/javascript\">if (typeof(parent.reloadList) != 'undefined') parent.reloadList(); </script>");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AnalysisWizard_FinishButtonClick(object sender, WizardNavigationEventArgs e)
        {
            try
            {
                if (AnalysisTemplateManager.AnalysisTemplateExists(_properties.ReportName, ResponseTemplateId))
                {
                    //                        reportNameErrLbl.Visible = true;
                    //                        reportNameErrLbl.Text = WebTextManager.GetText("/pageText/AutogenerateReport.aspx/reportNameExists");
                    return;
                }

                //This logic is here to ensure that a positive value is specified do we need this check?
                if (_options.MaxOptions < 0)
                {
                    //                maxOptionsErrLbl.Visible = true;
                    //                maxOptionsErrLbl.Text = WebTextManager.GetText("/pageText/AutogenerateReport.aspx/maxOptionsErrLbl");
                    return;
                }

                AnalysisTemplate currentAnalysisTemplate;

                if (_properties.IsWizard)
                {
                    currentAnalysisTemplate = AnalysisTemplateManager.AutoGenerateAnalysisTemplate(_properties.ReportName,
                        ResponseTemplateId, GetOptions(AnalysisTemplateManager.ShowUserInputItemsForReport(ResponseTemplateId)), 
                        _properties.StyleTemplateId, 
                        _properties.ChartStyleId, 
                        UserManager.GetCurrentPrincipal());
                }
                else
                {
                    //Create template and add a page
                    currentAnalysisTemplate = AnalysisTemplateManager.CreateAnalysisTemplate(_properties.ReportName, ResponseTemplateId, UserManager.GetCurrentPrincipal());
                    string url = ResolveUrl(string.Format("~/Forms/Surveys/Reports/Edit.aspx?r={0}", currentAnalysisTemplate.ID));

                    var args = new Dictionary<string, string> { { "op", "addReport" }, { "url", url } };
                    Master.CloseDialog(args);
                }

                //Add a page if there is no one
                if (currentAnalysisTemplate.PageCount <= 0)
                {
                    currentAnalysisTemplate.AddPageToTemplate(1, true);
                }

                TextManager.SetText(currentAnalysisTemplate.NameTextID, WebTextManager.GetUserLanguage(), _properties.ReportName);
                currentAnalysisTemplate.Name = _properties.ReportName;
                currentAnalysisTemplate.StyleTemplateID = _properties.StyleTemplateId;
                currentAnalysisTemplate.ChartStyleID = _properties.ChartStyleId;
                currentAnalysisTemplate.DisplaySurveyTitle = _properties.DisplaySurveyTitle;
                currentAnalysisTemplate.DisplayPdfExportButton = _properties.DisplayPdfExportButton;

                //            if (ApplicationManager.UseSimpleSecurity)
                //            {
                //                SetSimpleSecurityPolicy();
                //            }

                currentAnalysisTemplate.ModifiedBy = ((CheckboxPrincipal)HttpContext.Current.User).Identity.Name;
                currentAnalysisTemplate.Save();
                Session["NewReportId"] = currentAnalysisTemplate.ID;
                reloadReportList();
            }
            catch (Exception err)
            {
                ExceptionPolicy.HandleException(err, "UIProcess");
                //                _completionTitle.Text = WebTextManager.GetText("/pageText/users/add.aspx/errorTitle");
                //                _createUserError.Text = String.Format(WebTextManager.GetText("/pageText/users/add.aspx/errorMessage"), err.Message);
                //                _createUserError.Visible = true;
            }
        }

        /// <summary>
        /// Created report ID
        /// </summary>
        protected int ReportID
        {
            get
            {
                if (Session["NewReportId"] != null)
                {
                    return (int)Session["NewReportId"];
                }
                return 0;
            }            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AnalysisWizard_CancelButtonClick(object sender, EventArgs e)
        {
            Response.Redirect(ResolveUrl(string.Format("~/Forms/Surveys/Reports/Manage.aspx?s={0}", ResponseTemplateId)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void EditButton_OnClick(object sender, EventArgs e)
        {
			object idObj = Session["NewReportId"];

			if (idObj != null)
            {
				String url = ResolveUrl(string.Format("~/Forms/Surveys/Reports/Edit.aspx?r={0}", idObj));

                var args = new Dictionary<string, string> {{"op", "addReport"}, {"url", url}};
                Master.CloseDialog(args);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RestartButton_OnClick(object sender, EventArgs e)
        {
            Response.Redirect(ResolveUrl(string.Format("~/Forms/Surveys/Reports/Add.aspx?s={0}", ResponseTemplateId)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ExitButton_OnClick(object sender, EventArgs e)
        {
            object newReportIdObj = Session["NewReportId"];

			if (newReportIdObj != null)
			{
				var args = new Dictionary<string, string>
                           {
                               {"op", "addReport"},
                               {"reportId", newReportIdObj.ToString()},
                               {"showReportsTab", "true"}
                           };

				Master.CloseDialog(args);
			}

			Master.CloseDialog(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private AnalysisWizardOptions GetOptions(bool showUserInput)
        {
            var options = new AnalysisWizardOptions
                                                {
                                                    UseAliases = _options.UseAlias,
                                                    DisplayStatistics = _options.DisplayStatistics,
                                                    DisplayAnswers = _options.DisplayAnswers,
                                                    ItemPostion = _options.ItemPosition,
                                                    MaxOptions = _options.MaxOptions,
                                                    IsSinglePageReport = _options.IsSinglePageReport,
                                                    IncludeIncompleteResponses = _options.ReportIncompleteResponses,
                                                    IncludeTestResponses = _options.ReportTestResponses,

                                                    SingleLineTextGraphType = showUserInput ? _items.SingleLineTextGraphType :  string.Empty,
                                                    MultiLineTextGraphType = showUserInput ? _items.MultiLineTextGraphType :  string.Empty,
                                                    HiddenItemGraphType = _items.HiddenItemGraphType,
                                                    NetPromoterScoreGraphType = _items.NetPromoterScoreGraphType,

                                                    MatrixGraphType = _items.MatrixGraphType,
                                                    RadioButtonGraphType = _items.RadioButtonGraphType,
                                                    DropDownListGraphType = _items.DropDownListGraphType,
                                                    CheckboxGraphType = _items.CheckboxGraphType,
                                                    RatingScaleGraphType = _items.RatingScaleGraphType,
                                                    SliderGraphType = _items.SliderGraphType,
                                                    RankOrderGraphType = _items.RankOrderGraphType
                                                };
            return options;
        }
    }
}
