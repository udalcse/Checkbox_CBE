using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using Checkbox.Management;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Page;
using Prezza.Framework.Data;
using System.Web.UI.WebControls;
using Checkbox.Web.UI.Controls;
using Checkbox.Management.Licensing.Limits;
using System.Globalization;
using Checkbox.LicenseLibrary;

namespace CheckboxWeb
{
    /// <summary>
    /// Default page
    /// </summary>
    public partial class Default : ApplicationPage
    {
        /// <summary>
        /// Override page init
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                _loggedInView.Visible = true;
                _anonymousView.Visible = false;
            }
            else
            {
                _loggedInView.Visible = false;
                _anonymousView.Visible = true;
            }

            _showNews.Click += _showNews_Click;
            _hideNews.Click += _hideNews_Click;
            _setBtn.Click += _setBtn_Click;

            //Show product tour, if necessary
            ShowTourMessages();

            //Show hosting messages, if necessary
            ShowHostingMessages();

            //Show warning messages, if necessary.
            ShowWarningMessages();

            //Populdate license limits
            PopulateLicenseLimits();

            //TODO: Show/hide body lbl based on news
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _hideNews_Click(object sender, EventArgs e)
        {
            _newsLbl.Visible = false;
            _showNews.Visible = true;
            _hideNews.Visible = false;
            _pnlHideNews.Visible = false;

            try
            {
                Database db = DatabaseFactory.CreateDatabase(DatabaseFactory.MASTER_DB_NAME);
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_SetNewsVisibility");
                command.AddInParameter("contextname", DbType.String, Request.Headers["Host"]);
                command.AddInParameter("HideNews", DbType.Int32, 1);
                db.ExecuteNonQuery(command);
            }
            catch
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _showNews_Click(object sender, EventArgs e)
        {
            _newsLbl.Visible = true;
            _showNews.Visible = false;
            _hideNews.Visible = true;
            _pnlHideNews.Visible = true;

            try
            {
                Database db = DatabaseFactory.CreateDatabase(DatabaseFactory.MASTER_DB_NAME);
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_SetNewsVisibility");
                command.AddInParameter("contextname", DbType.String, Request.Headers["Host"]);
                command.AddInParameter("HideNews", DbType.Int32, 0);
                db.ExecuteNonQuery(command);
            }
            catch
            {

            }
        }

        /// <summary>
        /// Determine if the warning about not setting a time zone should be shown.
        /// </summary>
        public bool ShouldTimeZoneNotSetWarningBeShown
        {
            get { return ApplicationManager.AppSettings.IsTimeZoneSet && User.IsInRole("System Administrator"); }
        }

        /// <summary>
        /// Show warning messages.
        /// </summary>
        private void ShowWarningMessages()
        {
            if (ShouldTimeZoneNotSetWarningBeShown)
            {
                _timeZoneNotSetWarning.Visible = true;
                if (!IsPostBack)
                {
                    _timeZone.SelectedValue = ApplicationManager.AppSettings.TimeZone.ToString().Replace(',', '.');
                }
            }
            else
                _timeZoneNotSetWarning.Visible = false;            
        }

        /// <summary>
        /// 
        /// </summary>
        private void ShowHostingMessages()
        {
            _hostingWarningPlace.Visible = false;
            _newsPlace.Visible = false;
            if (!ApplicationManager.AppSettings.EnableMultiDatabase)
            {

                return;
            }

            _newsPlace.Visible = true;
            _hostingWarningPlace.Visible = true;

            ShowExpirationWarning();
            PopulateNews();
        }

        /// <summary>
        /// 
        /// </summary>
        private void PopulateNews()
        {
            string newsHtml = string.Empty;
            int hideNews = 0;
            Database db = DatabaseFactory.CreateDatabase(DatabaseFactory.MASTER_DB_NAME);
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_GetHostingNews");
            command.AddInParameter("contextname", DbType.String, Request.Headers["Host"]);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        hideNews = DbUtility.GetValueFromDataReader(reader, "HideNews", 0);
                        newsHtml = DbUtility.GetValueFromDataReader(reader, "NewsHTML", string.Empty);

                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            _newsLbl.Text = "<br /><ul class='newslist'>" + newsHtml.Replace("<a", "<a target='_blank'") + "<ul>";

            if (hideNews == 0)
            {
                _newsLbl.Visible = true;
                _showNews.Visible = false;
                _hideNews.Visible = true;
                _pnlHideNews.Visible = true;

            }
            else
            {
                _newsLbl.Visible = false;
                _showNews.Visible = true;
                _hideNews.Visible = false;
                _pnlHideNews.Visible = false;
            }
        }

        /// <summary>
        /// Populate limits
        /// </summary>
        private void PopulateLicenseLimits()
        {
            if (!ApplicationManager.AppSettings.EnableMultiDatabase)
            {
            
                return;
            }
            _limitContainer.Visible = false;
            MultiLanguageLabel maxNumbLimitsLbl = new MultiLanguageLabel();
            maxNumbLimitsLbl.TextId = "/licenseLimit/maximumNumberLimits";
            maxNumbLimitsLbl.Style["font-weight"] = "bold";

            _licenseLimitsPlaceholder.Controls.Add(maxNumbLimitsLbl);

            Table maximumNumberLimitsTable = new Table { CssClass = "limitTable" };
            maximumNumberLimitsTable.Rows.Add(new TableHeaderRow());
            maximumNumberLimitsTable.Rows.Add(new TableRow());

            maximumNumberLimitsTable.Rows[0].Cells.Add(new TableHeaderCell { Text = WebTextManager.GetText("/licenseLimit/limitName") });
            maximumNumberLimitsTable.Rows[0].Cells.Add(new TableHeaderCell { Text = WebTextManager.GetText("/licenseLimit/currentValue") });
            maximumNumberLimitsTable.Rows[0].Cells.Add(new TableHeaderCell { Text = WebTextManager.GetText("/licenseLimit/limitValue") });

            if (ActiveLicense.SurveyEditorLimit != null)
            {
                maximumNumberLimitsTable.Rows[1].Cells.Add(new TableCell { Text = WebTextManager.GetText("/licenseLimit/surveyEditorLimit") });
                maximumNumberLimitsTable.Rows[1].Cells.Add(new TableCell { Text = ActiveLicense.SurveyEditorLimit.CurrentCount.ToString(), CssClass = "values" });
                maximumNumberLimitsTable.Rows[1].Cells.Add(new TableCell { Text = ActiveLicense.SurveyEditorLimit.RuntimeLimitValue.ToString(), CssClass = "values" });
            }
            else
            {
                maximumNumberLimitsTable.Rows[1].Cells.Add(new TableCell { Text = WebTextManager.GetText("/licenseLimit/surveyEditorLimit") });
                maximumNumberLimitsTable.Rows[1].Cells.Add(new TableCell { Text = string.Empty, CssClass = "values" });
                maximumNumberLimitsTable.Rows[1].Cells.Add(new TableCell { Text = WebTextManager.GetText("/licenseLimit/unlimited"), CssClass = "values" });
            }

            _licenseLimitsPlaceholder.Controls.Add(maximumNumberLimitsTable);

            MultiLanguageLabel onOffLimitsLbl = new MultiLanguageLabel();
            onOffLimitsLbl.TextId = "/licenseLimit/onOffLimits";
            onOffLimitsLbl.Style["font-weight"] = "bold";
            _licenseLimitsPlaceholder.Controls.Add(onOffLimitsLbl);

            Table onOffLimitsTable = new Table { CssClass = "limitTable" };
            onOffLimitsTable.Rows.Add(new TableHeaderRow());

            for (int i = 0; i < 6; i++) onOffLimitsTable.Rows.Add(new TableRow());

            String errorStr;

            onOffLimitsTable.Rows[0].Cells.Add(new TableHeaderCell { Text = WebTextManager.GetText("/licenseLimit/limitName") });
            onOffLimitsTable.Rows[0].Cells.Add(new TableHeaderCell { Text = WebTextManager.GetText("/licenseLimit/limitValue") });

            onOffLimitsTable.Rows[1].Cells.Add(new TableCell { Text = WebTextManager.GetText("/licenseLimit/mandatoryCheckboxFooter") });
            onOffLimitsTable.Rows[1].Cells.Add(new TableCell
                                                   {
                                                       Text =
                                                           GetOnOrOffText(ActiveLicense.MandatoryCheckboxFooterLimit),
                                                       CssClass = "values"
                                                   });

            onOffLimitsTable.Rows[2].Cells.Add(new TableCell { Text = WebTextManager.GetText("/licenseLimit/simpleSecurity") });
            onOffLimitsTable.Rows[2].Cells.Add(new TableCell
                                                   {
                                                       Text =
                                                           GetOnOrOffText(ActiveLicense.SimpleSecurityLimit),
                                                       CssClass = "values"
                                                   });

            onOffLimitsTable.Rows[3].Cells.Add(new TableCell { Text = WebTextManager.GetText("/licenseLimit/SPSSExport") });
            onOffLimitsTable.Rows[3].Cells.Add(new TableCell
            {
                Text =
                    GetOnOrOffText(ActiveLicense.SpssLimit),
                CssClass = "values"
            });

            onOffLimitsTable.Rows[4].Cells.Add(new TableCell { Text = WebTextManager.GetText("/licenseLimit/ratingScaleStatisticsReportItem") });
            onOffLimitsTable.Rows[4].Cells.Add(new TableCell
            {
                Text =
                    GetOnOrOffText(ActiveLicense.RatingScaleStatisticsReportItemLimit),
                CssClass = "values"
            });

            onOffLimitsTable.Rows[5].Cells.Add(new TableCell { Text = WebTextManager.GetText("/licenseLimit/scoredSurvey") });
            onOffLimitsTable.Rows[5].Cells.Add(new TableCell
            {
                Text =
                    GetOnOrOffText(ActiveLicense.ScoredSurveyLimit),
                CssClass = "values"
            });

            onOffLimitsTable.Rows[6].Cells.Add(new TableCell { Text = WebTextManager.GetText("/licenseLimit/multiLanguageSupport") });
            onOffLimitsTable.Rows[6].Cells.Add(new TableCell
            {
                Text =
                    GetOnOrOffText(ActiveLicense.MultiLanguageLimit),
                CssClass = "values"
            });


            _licenseLimitsPlaceholder.Controls.Add(onOffLimitsTable);

            MultiLanguageLabel decrementLimitsLbl = new MultiLanguageLabel();
            decrementLimitsLbl.TextId = "/licenseLimit/decrementLimits";
            decrementLimitsLbl.Style["font-weight"] = "bold";
            _licenseLimitsPlaceholder.Controls.Add(decrementLimitsLbl);

            Table decrementLimitsTable = new Table { CssClass = "limitTable" };

            decrementLimitsTable.Rows.Add(new TableHeaderRow());
            decrementLimitsTable.Rows.Add(new TableRow());

            decrementLimitsTable.Rows[0].Cells.Add(new TableHeaderCell { Text = WebTextManager.GetText("/licenseLimit/limitName") });
            decrementLimitsTable.Rows[0].Cells.Add(new TableHeaderCell { Text = WebTextManager.GetText("/licenseLimit/currentValue") });
            decrementLimitsTable.Rows[0].Cells.Add(new TableHeaderCell { Text = WebTextManager.GetText("/licenseLimit/baseValue") });

            long? currentValue = ActiveLicense.EmailLimit.CurrentValue;
            long? baseValue = ActiveLicense.EmailLimit.BaseValue;

            decrementLimitsTable.Rows[1].Cells.Add(new TableCell { Text = WebTextManager.GetText("/licenseLimit/emailLimit") });

            if (currentValue.HasValue && baseValue.HasValue)
            {
                double availablePercent = ((double)currentValue.Value * 100) / baseValue.Value;

                decrementLimitsTable.Rows[1].Cells.Add(new TableCell { Text = currentValue.ToString(), CssClass = "values" });
                decrementLimitsTable.Rows[1].Cells.Add(new TableCell { Text = baseValue.ToString(), CssClass = "values" });

                if (ApplicationManager.AppSettings.MinPercentOfAvailableLimit > availablePercent)
                {
                    decrementLimitsTable.Rows[1].CssClass = "highlighted";
                }
            }
            else
            {
                decrementLimitsTable.Rows[1].Cells.Add(new TableCell { Text = "-", CssClass = "values" });
                decrementLimitsTable.Rows[1].Cells.Add(new TableCell { Text = "-", CssClass = "values" });
            }

            _licenseLimitsPlaceholder.Controls.Add(decrementLimitsTable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="licenseLimit"></param>
        /// <returns></returns>
        private static String GetOnOrOffText(LicenseLimit licenseLimit)
        {
            return WebTextManager.GetText(IsLimitValid(licenseLimit) ? "/licenseLimit/on" : "/licenseLimit/off");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="licenseLimit"></param>
        /// <returns></returns>
        private static bool IsLimitValid(LicenseLimit licenseLimit)
        {
            String errorMsg;
            return licenseLimit.Validate(out errorMsg) == LimitValidationResult.LimitNotReached;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ShowExpirationWarning()
        {

            DateTime endDate = DateTime.Now;

            Database db = DatabaseFactory.CreateDatabase(DatabaseFactory.MASTER_DB_NAME);
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_GetApplicationContextDetails");
            command.AddInParameter("contextname", DbType.String, Request.Headers["Host"]);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        endDate = DbUtility.GetValueFromDataReader(reader, "enddate", DateTime.MinValue);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            if (endDate == DateTime.MinValue)
            {
                _hostingWarningMessagePanel.Visible = false;
                _hostingExpirationPanel.Visible = false;
                return;
            }

            var diffTimeSpan = endDate.Subtract(DateTime.Now);

            _hostingExpirationWarningDate.Text = endDate.ToShortDateString();
            _hostingExpirationDate.Text = endDate.ToString();
            _hostingExpirationWarningDays.Text = diffTimeSpan.Days.ToString();

            //Only show expiration date if no warnings
            if (diffTimeSpan.Days > 60)
            {
                _hostingWarningMessagePanel.Visible = false;
                _hostingExpirationPanel.Visible = true;
                return;
            }

            _hostingWarningMessagePanel.Visible = true;
            _hostingExpirationPanel.Visible = false;


            if (diffTimeSpan.Days > 30)
            {
                _renewMsg.Text = "Please <a href=\"http://www.checkbox.com/contactus.aspx\" target=\"_blank\">contact sales</a> to renew and save 10%";
                _hostingWarningMessagePanel.Style[HtmlTextWriterStyle.Color] = "#F38800";
            }
            else
            {
                _renewMsg.Text = "Please <a href=\"http://www.checkbox.com/contactus.aspx\" target=\"_blank\">contact sales</a> to avoid any interruption.";
                _hostingWarningMessagePanel.Style[HtmlTextWriterStyle.Color] = "#EE0000";
            }
            if(ApplicationManager.IsDataContextTrial)
            {
                _hostingWarningPlace.Visible = false;
            }
        }

        /// <summary>
        /// Product tour
        /// </summary>
        private void ShowTourMessages()
        {
            if (ApplicationManager.AppSettings.DisplayProductTour && !GetSessionValue("ProductTourShown", false))
            {
                List<FileInfo> tourFiles = UserManager.GetProductTourMessages(UserManager.GetCurrentPrincipal(), Page.Request.PhysicalApplicationPath + "Help\\Tour\\");

                if (tourFiles.Count > 0)
                {
                    ClientScript.RegisterClientScriptBlock(GetType(), "WelcomeMessage", "showDialog('" + ResolveUrl("~/Messages.aspx") + "');", true);
                    Session["Messages"] = tourFiles;
                }
            }

            Session["ProductTourShown"] = true;
        }

        /// <summary>
        /// Welcome message that remains the same for all the users
        /// </summary>
        protected string CommonWelcomeMessage
        {
            get
            {
                return WebTextManager.GetText("/siteText/welcomeBodyText");
            }
        }

        /// <summary>
        /// Welcome message that remains the same for not logged in users
        /// </summary>
        protected string AnonymousWelcomeMessage
        {
            get
            {
                return WebTextManager.GetText("/siteText/welcomeBodyTextAnonymous");
            }
        }

        /// <summary>
        /// Message that depends on the user role
        /// </summary>
        protected string RoleSpecificWelcomeMessage
        {
            get
            {
                if (string.IsNullOrEmpty(User.Identity.Name))
                    return null;
                else if (User.IsInRole("System Administrator"))
                    return WebTextManager.GetText("/siteText/welcomeBodyTextSystemAdministrator");
                else if (User.IsInRole("User Administrator") || User.IsInRole("Group Administrator"))
                    return WebTextManager.GetText("/siteText/welcomeBodyTextUserAdministrator");
                else if (User.IsInRole("Survey Administrator"))
                    return WebTextManager.GetText("/siteText/welcomeBodyTextSurveyEditor");
                else if (User.IsInRole("Respondent"))
                    return WebTextManager.GetText("/siteText/welcomeBodyTextRespondent");
                else if (User.IsInRole("Report Viewer"))
                    return WebTextManager.GetText("/siteText/welcomeBodyTextReportViewer");
                else if (User.IsInRole("Report Administrator"))
                    return WebTextManager.GetText("/siteText/welcomeBodyTextReportEditor");
                else if (User.IsInRole("Survey Editor"))
                    return WebTextManager.GetText("/siteText/welcomeBodyTextSurveyEditor");

                return "";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _setBtn_Click(object sender, EventArgs e)
        {
            ApplicationManager.AppSettings.TimeZone = DoubleParse(_timeZone.SelectedValue);

            //Don't show time "time zone isn't set warning message"
            _timeZoneNotSetWarning.Visible = false;
        }

        /// <summary>
        /// Parse double string regardless of current culture settings
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private double DoubleParse(string value)
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");           
            double temp;
            if (Double.TryParse(value, NumberStyles.Float, culture, out temp))
                return temp;
            return 0;
        }
    }
}
