using System;
using System.IO;
using System.Threading;
using System.Web.UI.WebControls;
using Checkbox.Analytics.Export;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Globalization.Text;
using Checkbox.Management;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Page;

namespace CheckboxWeb.Forms.Surveys.Responses
{
	/// <summary>
	/// Export results
	/// </summary>
	public partial class Export : ExportResultsPage
	{
		#region Init/Page_Load events
		/// <summary>
		/// Initialize the page and bind events
		/// </summary>
		protected override void OnPageInit()
		{
			base.OnPageInit();

			//Bind events
			_exportBtn.Click += ExportBtn_Click;
			_dlFilesBtn.Click += DownloadFiles_ClickEvent;


			//Set the default value to be the value defined in the application settings
            if (ExportOptions == null)
            {
                ExportOptions = new ExportOptions();
            }
		    ExportOptions.ExportMode = string.Compare(ApplicationManager.AppSettings.DefaultExportType, "Standard",
		                                              true) == 0
		                                   ? "CSV"
		                                   : ApplicationManager.AppSettings.DefaultExportType;
 
		    PopulateExportModes();
		    
            _exportMode.SelectedValue = _exportMode.Items.FindByValue(ExportOptions.ExportMode) == null ?  "CSV" : ExportOptions.ExportMode;

		    if (_exportMode.SelectedValue == "CSV")
			{
				_spssOptionsPanel.Visible = false;
				_csvOptionsPanel.Visible = true;
				EnableCSVOptions();
			}
			else
			{
				_spssOptionsPanel.Visible = true;
				_csvOptionsPanel.Visible = false;
				DisableCSVOptions();
			}



			//Set default values of responses options based on app settings
			SetDefaultValues();

		    //Set visibility of download files button
			ConfigureDownloadOptions();

			Master.HideDialogButtons();
		}

        /// <summary>
        /// Set default values of responses options based on app settings
        /// </summary>
	    private void SetDefaultValues()
	    {	        
	        _csvOptionsList.Items.FindByValue("EXPORT_INCOMPLETE_RESPONSES").Selected = ApplicationManager.AppSettings.CsvExportIncludeIncompleteResponses;
            _csvOptionsList.Items.FindByValue("MERGE_CHECKBOX_RESULTS").Selected = ApplicationManager.AppSettings.CsvExportMergeCheckboxResults;
            _csvOptionsList.Items.FindByValue("EXPORT_WITH_ALIASES").Selected = ApplicationManager.AppSettings.CsvExportUseAliases;
            _csvOptionsList.Items.FindByValue("EXPORT_OPEN_ENDED_RESULTS").Selected = ApplicationManager.AppSettings.CsvExportIncludeOpenendedResults;
            _csvOptionsList.Items.FindByValue("DETAILED_RESPONSE_INFO").Selected = ApplicationManager.AppSettings.CsvExportIncludeResponseDetails;
            _csvOptionsList.Items.FindByValue("DETAILED_USER_INFO").Selected = ApplicationManager.AppSettings.CsvExportIncludeUserDetails;
            _csvOptionsList.Items.FindByValue("EXPORT_HIDDEN_ITEMS").Selected = ApplicationManager.AppSettings.CsvExportIncludeHiddenItems;
            _csvOptionsList.Items.FindByValue("STRIP_HTML_TAGS").Selected = ApplicationManager.AppSettings.CsvExportStripHtmlTagsFromAnswers;
            _csvOptionsList.Items.FindByValue("RANK_ORDER_POINTS").Selected = ApplicationManager.AppSettings.CsvExportRankOrderPoints;
            _csvOptionsList.Items.FindByValue("TEST_RESPONSES").Selected = ApplicationManager.AppSettings.ExportIncludeTestResponses;

            var detailedScoreInfo = _csvOptionsList.Items.FindByValue("DETAILED_SCORE_INFO");
            var possibleScore = _csvOptionsList.Items.FindByValue("POSSIBLE_SCORE");
            if (ResponseTemplate.BehaviorSettings.EnableScoring)
            {
                detailedScoreInfo.Selected = ApplicationManager.AppSettings.CsvExportIncludeDetailedScoreInfo;
                detailedScoreInfo.Attributes.Add("detailedscore", "true");

                possibleScore.Selected = ApplicationManager.AppSettings.CsvExportIncludePossibleScore;
                possibleScore.Attributes.Add("possiblescore", "true");
                possibleScore.Attributes.Add("style", "display: none;");
            }
            else
            {
                _csvOptionsList.Items.Remove(detailedScoreInfo);
                _csvOptionsList.Items.Remove(possibleScore);
            }

            _spssExportIncompleteResponses.Checked = ApplicationManager.AppSettings.SpssExportIncludeIncompleteResponses;
            _spssExportOpenEndedResponses.Checked = ApplicationManager.AppSettings.SpssExportIncludeOpenendedResults;
            _spssExportResponseId.Checked = ApplicationManager.AppSettings.SpssExportIncludeResponseId;

            _outputEncoding.SelectedValue = ApplicationManager.AppSettings.DefaultExportEncoding;
	    }

	    /// <summary>
        /// 
        /// </summary>
		protected void PopulateExportModes()
		{
			_exportMode.Items.Clear();
			_exportMode.Items.Add(new ListItem(WebTextManager.GetText("/pageText/forms/surveys/responses/export.aspx/standardCSV"), "CSV"));
			if (ApplicationManager.AppSettings.AllowNativeSpssExport)
			{
				_exportMode.Items.Add(
					new ListItem(WebTextManager.GetText("/pageText/forms/surveys/responses/export.aspx/spss"),
								 "SPSS_CSV"));
				_exportMode.Items.Add(
					new ListItem(WebTextManager.GetText("/pageText/forms/surveys/responses/export.aspx/spssNative"),
								 "SPSS_Native"));
			}

            //Disable for now until XML IMport is working
			//_exportMode.Items.Add(new ListItem(WebTextManager.GetText("/pageText/forms/surveys/responses/export.aspx/xml"), "XML"));
		}

		/// <summary>
		/// Page load
		/// </summary>
        protected override void OnPageLoad()
		{
		    base.OnPageLoad();

		    //Update link for spss key to include open-ended results or not
		    _spssKeyLink.NavigateUrl = "SPSSKey.aspx?includeOpenEnded=" + _spssExportOpenEndedResponses.Checked + "&s=" +
		                               ResponseTemplate.ID;

		    _responseIdPlace.Visible = !_exportMode.SelectedValue.Equals("CSV", StringComparison.InvariantCultureIgnoreCase);

            //Update UI
		    UpdateUI();
		}

	    /// <summary>
		/// 
		/// </summary>
		private void ConfigureDownloadOptions()
		{
			if (ApplicationManager.AppSettings.EnableUploadItem && UploadItemManager.GetFileCount(ResponseTemplate.ID.Value) > 0)
			{
				if (ApplicationManager.AppSettings.RestrictUploadFileExport)
				{
					var currentPrincipal = UserManager.GetCurrentPrincipal();

					if (currentPrincipal != null && currentPrincipal.IsInRole("System Administrator"))
					{
						_fileDownloadPanel.Visible = true;
					}
				}
				else
				{
					_fileDownloadPanel.Visible = true;
				}
			}
		}

		#endregion

		#region Event handlers
      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
		public static string GetSpssOptionsPanelText(string mode)
		{
			if (mode.Equals("XML", StringComparison.InvariantCultureIgnoreCase))
				return "XML Options";

			return WebTextManager.GetText("/pageText/forms/surveys/responses/export.aspx/spssOptions");
		}

		/// <summary>
		/// Show/hide elements based on export format
		/// </summary>
		private void UpdateUI()
		{
			if (_exportMode.SelectedValue.Equals("XML", StringComparison.InvariantCultureIgnoreCase))
			{
				_spssOptionsPanel.Visible = true;
				_csvOptionsPanel.Visible = false;
				_spssKeyLink.Visible = false;
				_spssExportResponseId.Visible = false;
				spssNewWindow.Visible = false;
				DisableCSVOptions();

				return;
			}

			if (_exportMode.SelectedValue == "CSV")
			{
				_spssOptionsPanel.Visible = false;
				_csvOptionsPanel.Visible = true;
				EnableCSVOptions();
			}
			else
			{
				_spssOptionsPanel.Visible = true;
				_csvOptionsPanel.Visible = false;
				_spssExportResponseId.Visible = true;
                spssNewWindow.Visible = _spssKeyLink.Visible = _exportMode.SelectedValue == "SPSS_CSV" || _exportMode.SelectedValue == "SPSS_Native";				
				DisableCSVOptions();
			}
		}

		/// <summary>
		/// Export!
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ExportBtn_Click(object sender, EventArgs e)
		{
			EventHandlerWrapper(sender, e, DoExport);
		}

		/// <summary>
		/// Do the export
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DoExport(object sender, EventArgs e)
		{
			try
			{
				if (ValidateDateFilters())
				{
					//Store options
					StoreExportOptions();

					//Set file path.  This will be used by native spss export and by
					// ajax export page.
					ExportFilePath = string.Format(
						@"{0}\{1}_{2}_{3}",
						TempFolderPath,
						ApplicationManager.ApplicationDataContext,
						DateTime.Now.Ticks,
						GetAttachmentFileName());

					//See if redirect possible then do it.
					if (DoRedirectToAjaxEnabledPage("ExportProgress.aspx"))
					{
						return;
					}

					//No ajax, so use default method
					//Code path for SPSS and CSV export are the same, since both use a generated analysis to export
					// data.  The difference is in the type of export item added to the analysis.
					if ("CSV".Equals(_exportMode.SelectedValue, StringComparison.InvariantCultureIgnoreCase) ||
						"SPSS_CSV".Equals(_exportMode.SelectedValue, StringComparison.InvariantCultureIgnoreCase) ||
						"XML".Equals(_exportMode.SelectedValue, StringComparison.InvariantCultureIgnoreCase))
					{
						DoCommonExport();
					}
					else
					{
						DoNativeSpssExport();
					}
				}
			}
			catch (ThreadAbortException) { }
		}

		/// <summary>
		/// Check to see if Ajax enabled page is possible and if possible, set up redirect.  To avoid spurious
		/// errors, end response parameter is passed as false, so calling method should read return value and
		/// act accordingly.  Value of true means redirect was set.
		/// </summary>
		/// <returns></returns>
		private bool DoRedirectToAjaxEnabledPage(string pageName)
		{
			//Ensure output directory can be written to.
			//If directory can't be written, use default output method.
			//Check to see if file can be created. If it can, redirect to Ajaxy page to write data to file. If not, write export directly to response
			// using existing mechanism.
			bool writeToTempFile = UploadItemManager.ValidateDownloadDirectory(TempFolderPath);

			//Permissions check out, so store some state information and redirect to ajax page
			if (writeToTempFile)
			{
				//Set progress key
				Session["ExportResultsProgressKey"] = "ExportResults_" + ResponseTemplate.ID + "_" + Session.SessionID;

				//Redirect
				Response.Redirect(pageName + "?s=" + ResponseTemplate.ID, false);

				return true;
			}

			return false;
		}

		/// <summary>
		/// Download any uploaded files associated with the responseTemplate
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void DownloadFiles_ClickEvent(object sender, EventArgs e)
		{
			EventHandlerWrapper(sender, e, DownloadFiles);
		}

		/// <summary>
		/// Download files
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DownloadFiles(object sender, EventArgs e)
		{
			//Attempt to redirect and if no redirect, revert to "classic" download
			// method.

			//Store export file path
			ExportFilePath = string.Format(
					@"{0}\{1}_{2}_{3}_{4}",
					TempFolderPath,
					ApplicationManager.ApplicationDataContext,
					DateTime.Now.Ticks,
					"UploadedSurveyFiles",
					ResponseTemplate.ID);

			if (!DoRedirectToAjaxEnabledPage("DownloadProgress.aspx"))
			{
				//Use upload item path configured in app setting when falling back
				const string downloadName = "UploadItems.zip";
				try
				{
					if (UploadItemManager.ValidateDownloadDirectory(TempFolderPath))
					{
						UploadItemManager.SaveFilesToDisk(ResponseTemplate.ID.Value, ExportFilePath, null, null);
						FileUtilities.CompressFolder(ExportFilePath, downloadName);
						DownloadZipFromDisk(string.Format("{0}/{1}", ExportFilePath, downloadName), downloadName);
					}
					else
					{
						byte[] zip = UploadItemManager.GetFilesAsArchive(ResponseTemplate.ID.Value);
						if (zip != null)
						{
							DownloadZipFromMemory(zip, downloadName);
						}
					}
				}
				catch (Exception)
				{
					//Intentional no-op as error messages are written to the redirected response
				}
			}
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private bool ValidateDateFilters()
		{
			MinCompletedDate = Utilities.GetDate(_startDatePicker.Text);
            MaxCompletedDate = Utilities.GetDate(_endDatePicker.Text);

			//Ensure start date is beginning of day
			if (MinCompletedDate.HasValue)
			{
				MinCompletedDate = new DateTime(MinCompletedDate.Value.Year, MinCompletedDate.Value.Month, MinCompletedDate.Value.Day);
			}

			//Ensure end date is the end of the selected day
			if (MaxCompletedDate.HasValue)
			{
				//Start by truncating any time information
				MaxCompletedDate = new DateTime(MaxCompletedDate.Value.Year, MaxCompletedDate.Value.Month, MaxCompletedDate.Value.Day);

				//Add a day, then subtract a millisecond
				MaxCompletedDate = MaxCompletedDate.Value.Date.AddDays(1);
				MaxCompletedDate = MaxCompletedDate.Value.Date.Subtract(new TimeSpan(0, 0, 0, 0, 1));
			}

            //Convert to the server's time zone.
		    MinCompletedDate = WebUtilities.ConvertFromClientToServerTimeZone(MinCompletedDate);
		    MaxCompletedDate = WebUtilities.ConvertFromClientToServerTimeZone(MaxCompletedDate);

			if (MinCompletedDate.HasValue && MaxCompletedDate.HasValue
				&& MinCompletedDate > MaxCompletedDate)
			{
				//TypedMaster.ShowError(WebTextManager.GetText("/pageText/exportResults.aspx/dateFilterError"), null);
				return false;
			}

			return true;
		}

		/// <summary>
		/// Store current export options in t
		/// </summary>
		public void StoreExportOptions()
		{
			//Get export options
			var options = new ExportOptions
			{
				IncludeHidden = _csvOptionsList.Items.FindByValue("EXPORT_HIDDEN_ITEMS").Selected,
				StartDate = MinCompletedDate,
				EndDate = MaxCompletedDate,
				FileSet = null,
				IncludeResponseId = _exportMode.SelectedValue.Equals("CSV", StringComparison.InvariantCultureIgnoreCase) || _spssExportResponseId.Checked,
				ExportMode = _exportMode.SelectedValue
			};

			if (_exportMode.SelectedValue.Equals("CSV", StringComparison.InvariantCultureIgnoreCase) ||
				_exportMode.SelectedValue.Equals("XML", StringComparison.InvariantCultureIgnoreCase))
			{
				options.MergeSelectMany = _csvOptionsList.Items.FindByValue("MERGE_CHECKBOX_RESULTS").Selected;
				options.UseAliases = _csvOptionsList.Items.FindByValue("EXPORT_WITH_ALIASES").Selected;
				options.IncludeOpenEnded = _csvOptionsList.Items.FindByValue("EXPORT_OPEN_ENDED_RESULTS").Selected;
				options.IncludeIncomplete = _csvOptionsList.Items.FindByValue("EXPORT_INCOMPLETE_RESPONSES").Selected;
				options.IncludeDetailedResponseInfo = _csvOptionsList.Items.FindByValue("DETAILED_RESPONSE_INFO").Selected;
				options.IncludeDetailedUserInfo = _csvOptionsList.Items.FindByValue("DETAILED_USER_INFO").Selected;
				options.IncludeScore = ResponseTemplate.BehaviorSettings.EnableScoring;
			    options.StripHtmlTags = _csvOptionsList.Items.FindByValue("STRIP_HTML_TAGS").Selected;
                options.ExportRankOrderPoints = _csvOptionsList.Items.FindByValue("RANK_ORDER_POINTS").Selected;
                options.IncludeTestResponses = _csvOptionsList.Items.FindByValue("TEST_RESPONSES").Selected;
                options.IncludeDetailedScoreInfo = ResponseTemplate.BehaviorSettings.EnableScoring && _csvOptionsList.Items.FindByValue("DETAILED_SCORE_INFO").Selected;
                options.IncludePossibleScore = ResponseTemplate.BehaviorSettings.EnableScoring && _csvOptionsList.Items.FindByValue("DETAILED_SCORE_INFO").Selected && _csvOptionsList.Items.FindByValue("POSSIBLE_SCORE").Selected;
            }
            else
			{
				options.IncludeOpenEnded = _spssExportOpenEndedResponses.Checked;
				options.IncludeIncomplete = _spssExportIncompleteResponses.Checked;
				options.UseAliases = false;
				options.MergeSelectMany = false;
				options.IncludeDetailedResponseInfo = false;
				options.IncludeDetailedUserInfo = false;
				options.IncludeScore = false;
			    options.IncludeDetailedScoreInfo = false;
                options.IncludePossibleScore = false;
            }

            switch (_outputEncoding.SelectedValue.ToLower())
			{
				case "ascii":
					options.OutputEncoding = System.Text.Encoding.ASCII;
					break;
				case "unicode":
					options.OutputEncoding = System.Text.Encoding.Unicode;
					break;
				case "utf7":
					options.OutputEncoding = System.Text.Encoding.UTF7;
					break;
				case "utf8":
					options.OutputEncoding = System.Text.Encoding.UTF8;
					break;
				default:
					options.OutputEncoding = System.Text.Encoding.Default;
					break;
			}

			ExportOptions = options;
		}

		/// <summary>
		/// Export to CSV
		/// </summary>
		public void DoCommonExport()
		{
			//See if temp file can be used for better performance
			bool writeToTempFile = UploadItemManager.ValidateDownloadDirectory(TempFolderPath);

			PrepareResponse();

			if (writeToTempFile)
			{
				ExportManager.WriteCommonExportToFile(
					ResponseTemplate.ID.Value,
					ExportOptions,
					LanguageCode,
					null,
					ExportFilePath);

				//Get file size
				var info = new FileInfo(ExportFilePath);

				Response.AddHeader("Content-Length", info.Length.ToString());

				Response.TransmitFile(ExportFilePath);
			}
			else
			{
				ExportManager.WriteCommonExportToTextWriter(
					Response.Output,
					ResponseTemplate.ID.Value,
					ExportOptions,
					LanguageCode,
					ProgressKey);
			}

			Response.Flush();
			Response.End();
		}

		/// <summary>
		/// Peform the native SPSS export
		/// </summary>
		public void DoNativeSpssExport()
		{
			try
			{
				DateTime now = DateTime.Now;

				string fileName = Server.MapPath(string.Format("~/Temp/{0}_SPSS.sav", now.Ticks));

				ExportManager.WriteNativeSpssExportToFile(ResponseTemplate.ID.Value, ExportOptions, LanguageCode, ProgressKey, fileName);

				PrepareResponse();

				//Get file size
				var info = new FileInfo(fileName);

				Response.AddHeader("Content-Length", info.Length.ToString());

				Response.TransmitFile(fileName);

				Response.Flush();
				Response.End();
			}
			catch
			{
				throw;
			}
			//catch (SpssException ex)
			//{
			//    //Ensure error gets logged
			//    ExceptionPolicy.HandleException(ex, "UIProcess");

			//    //Attempt to provide a more useful error message.
			//    if (ex.SpssResultCode == ReturnCode.SPSS_INVALID_FILE
			//        || ex.SpssResultCode == ReturnCode.SPSS_INVALID_HANDLE)
			//    {
			//        TypedMaster.ShowError("Unable to create temporary file for SPSS export. Please ensure that a temporary folder has been created and properly configured.  Please see installation or patch instructions for details.", null);
			//    }
			//}
		}


		/// <summary>
		/// Enables all CSV export options
		/// </summary>
		protected void EnableCSVOptions()
		{
			foreach (ListItem item in _csvOptionsList.Items)
			{
				item.Enabled = true;
			}
		}

		/// <summary>
		/// Disables all CSV export options
		/// </summary>
		protected void DisableCSVOptions()
		{
			foreach (ListItem item in _csvOptionsList.Items)
			{
				item.Enabled = false;
			}
		}
	}
}
