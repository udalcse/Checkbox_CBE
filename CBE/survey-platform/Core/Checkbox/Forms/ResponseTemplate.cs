using System;
using System.Data.Common;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using System.Xml;

using Checkbox.Analytics.Filters.Configuration;
using Checkbox.Analytics.Items;
using Checkbox.Common;
using Checkbox.Forms.Data;
using Checkbox.Forms.Items;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Logic;
using Checkbox.Forms.Logic.Configuration;
using Checkbox.Forms.Piping.Tokens;
using Checkbox.Forms.Security;
using Checkbox.Globalization.Text;
using Checkbox.Management;
using Prezza.Framework.Common;
using Prezza.Framework.Data;
using Prezza.Framework.ExceptionHandling;
using Prezza.Framework.Security;
using System.IO;
using System.Threading;
using Checkbox.Security.Principal;
using Prezza.Framework.Caching;

namespace Checkbox.Forms
{
    /// <summary>
    /// This is the "Survey" configuration component.  It provides configuration operations and acts as a factory of <see cref="Response"/> objects
    /// </summary>
    [Serializable]
    public class ResponseTemplate : Template
    {
        #region Member Variables

        private SurveyBehaviorSettings _behaviorSettings;
        private SurveyStyleSettings _styleSettings;
        private List<SurveyTerm> _surveyTerms;
        private SurveyLanguageSettings _languageSettings;

        private readonly Dictionary<string, ItemToken> _responsePipes;
        private ResponseTemplateFilterCollection _filters;

        #endregion

        /// <summary>
        /// Get type name for this pdo
        /// </summary>
        public override string ObjectTypeName { get { return "ResponseTemplate"; } }

        /// <summary>
        /// Get the name of the library template id column
        /// </summary>
        public override string IdentityColumnName
        {
            get { return "ResponseTemplateID"; }
        }

        /// <summary>
        /// Get RT sproc name
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_RT_GetResponseTemplate"; } }

        #region Constructor/Init

        /// <summary>
        /// Construct a response template
        /// </summary>
        internal ResponseTemplate()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Constructor for response template
        /// </summary>
        internal ResponseTemplate(string createdBy)
            : base(new[] { "Form.Administer", "Form.Edit", "Form.Fill", "Analysis.ViewResponses", "Analysis.Analyze" },
                   new[] { "Form.Administer", "Form.Create", "Form.Delete", "Form.Edit", "Form.Fill", "Analysis.Create", "Analysis.Responses.View", "Analysis.Responses.Export", "Analysis.Responses.Edit", "Analysis.ManageFilters" })
        {
            CreatedBy = createdBy;
            _responsePipes = new Dictionary<string, ItemToken>();
            _surveyTerms = new List<SurveyTerm>();
            SetDefaultValues();
        }

        /// <summary>
        /// Create a configuration data set
        /// </summary>
        /// <returns></returns>
        protected override PersistedDomainObjectDataSet CreateConfigurationDataSet()
        {
            return new ResponseTemplateDataSet(ObjectTypeName) {DataSetName = "ConfigurationData"};
        }

        /// <summary>
        /// Initialize the access for the object
        /// </summary>
        /// <param name="defaultPolicy"></param>
        /// <param name="acl"></param>
        internal void InitializeAccess(Policy defaultPolicy, AccessControlList acl)
        {
            if (ID != null && ID > 0)
            {
                throw new Exception("Access can only be initialized for a new response template.");
            }

            ArgumentValidation.CheckExpectedType(defaultPolicy, typeof(FormPolicy));

            SetAccess(defaultPolicy, acl);
        }

        /// <summary>
        /// Create a policy with the specified permissions
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public override Policy CreatePolicy(string[] permissions)
        {
            return new FormPolicy(permissions);
        }

        #endregion


        #region Load Methods

        /// <summary>
        /// Gets the Data object by which a ResponseTemplate's state is loaded
        /// </summary>
        /// <returns></returns>
        protected override void LoadConfigurationDataSet(PersistedDomainObjectDataSet ds)
        {
            //To avoid having each page, item, etc. call to the database, we use a system
            //where item data is retrieved all at once.  However, to support plugging new items
            //in, it will not be required that every item's data be in the retrieved dataset.  One issue
            //is that we will not, in code, know the names of each table returned by ckbx_GetResponseTemplate,
            //so what we'll do is look that up.
            ds.Load(ID.Value, "ckbx_sp_RT_GetResponseTemplate", new List<DbParameter>() { new GenericDbParameter("ResponseTemplateID", DbType.Int32, ID) });

            if (ds.DomainObjectDataRow == null)
            {
                throw new TemplateDoesNotExist(ID.Value);
            }

            //Set xml mappings
            //TODO: Is this necessary
            SetConfigurationDataSetColumnMappings(ds);

            //Set id for language settings so text id can be determined
            LanguageSettings.SurveyId = ID.Value;
        }

        /// <summary>
        /// Factory method returns a new <see cref="RuleDataService"/> wrapper with which to effect 
        /// Rule related configuration changes and operations on this <see cref="ResponseTemplate"/>
        /// </summary>
        /// <returns></returns>
        public RuleDataService CreateRuleDataService(bool useCache = true)
        {
            RuleDataService rds = null;

            if (useCache)
                rds = SurveyMetaDataProxy.GetRuleDataServiceFromCache(ID.Value);

            if (rds == null)
            {
                rds = new RuleDataService();
                rds.Initialize(ID.Value);
            }

            return rds;
        }

        /// <summary>
        /// Load the template data
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadAdditionalData(PersistedDomainObjectDataSet data)
        {
            base.LoadAdditionalData(data);

            //Load response pipes from the data source
            LoadResponsePipes(data);

            //Load response pipes from the data source
            LoadResponseTerms(data);
        }

        /// <summary>
        /// Load base template data
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadBaseObjectData(DataRow data)
        {
            LoadTemplateData(data);
        }

        /// <summary>
        /// Load template data from the specified data row.
        /// </summary>
        /// <param name="responseTemplateDr"></param>
        private void LoadTemplateData(DataRow responseTemplateDr)
        {
            //Set the response template properties
            AclID = DbUtility.GetValueFromDataRow<int?>(responseTemplateDr, "AclID", null);
            DefaultPolicyID = DbUtility.GetValueFromDataRow<int?>(responseTemplateDr, "DefaultPolicy", null);

            ID = DbUtility.GetValueFromDataRow<int?>(responseTemplateDr, "ResponseTemplateID", null);
            GUID = DbUtility.GetValueFromDataRow(responseTemplateDr, "GUID", Guid.Empty);

            CreatedBy = DbUtility.GetValueFromDataRow(responseTemplateDr, "CreatedBy", string.Empty);
            CreatedDate = DbUtility.GetValueFromDataRow<DateTime?>(responseTemplateDr, "CreatedDate", null);
            LastModified = DbUtility.GetValueFromDataRow<DateTime?>(responseTemplateDr, "ModifiedDate", null);

            StyleSettings.StyleTemplateId = DbUtility.GetValueFromDataRow<int?>(responseTemplateDr, "StyleTemplateID", null);
            StyleSettings.TabletStyleTemplateId = DbUtility.GetValueFromDataRow<int?>(responseTemplateDr, "TabletStyleTemplateID", null);
            StyleSettings.SmartPhoneStyleTemplateId = DbUtility.GetValueFromDataRow<int?>(responseTemplateDr, "SmartPhoneStyleTemplateId", null);
            StyleSettings.MobileStyleId = DbUtility.GetValueFromDataRow<int?>(responseTemplateDr, "MobileStyleId", 1000);
            BehaviorSettings.IsActive = DbUtility.GetValueFromDataRow(responseTemplateDr, "IsActive", false);

            BehaviorSettings.SecurityType = (SecurityType)DbUtility.GetValueFromDataRow(responseTemplateDr, "SecurityType", 1);
            BehaviorSettings.ReportSecurityType = (ReportSecurityType)DbUtility.GetValueFromDataRow(responseTemplateDr, "ReportSecurityType", 3);


            Name = DbUtility.GetValueFromDataRow(responseTemplateDr, "TemplateName", string.Empty);
            BehaviorSettings.ActivationStartDate = DbUtility.GetValueFromDataRow<DateTime?>(responseTemplateDr, "ActivationStart", null);
            BehaviorSettings.ActivationEndDate = DbUtility.GetValueFromDataRow<DateTime?>(responseTemplateDr, "ActivationEnd", null);
            BehaviorSettings.MaxTotalResponses = DbUtility.GetValueFromDataRow<int?>(responseTemplateDr, "MaxTotalResponses", null);
            BehaviorSettings.MaxResponsesPerUser = DbUtility.GetValueFromDataRow<int?>(responseTemplateDr, "MaxResponsesPerUser", null);
            BehaviorSettings.AllowContinue = DbUtility.GetValueFromDataRow(responseTemplateDr, "AllowContinue", false);
            BehaviorSettings.AllowEdit = DbUtility.GetValueFromDataRow(responseTemplateDr, "AllowEdit", false);
            BehaviorSettings.AnonymizeResponses = DbUtility.GetValueFromDataRow(responseTemplateDr, "AnonymizeResponses", false);
            MobileCompatible = DbUtility.GetValueFromDataRow(responseTemplateDr, "MobileCompatible", false);
            StyleSettings.ShowItemNumbers = DbUtility.GetValueFromDataRow(responseTemplateDr, "ShowItemNumbers", false);
            StyleSettings.ShowPageNumbers = DbUtility.GetValueFromDataRow(responseTemplateDr, "ShowPageNumbers", false);

            StyleSettings.ShowTopSurveyButtons = DbUtility.GetValueFromDataRow(responseTemplateDr, "ShowTopSurveyButtons", false);
            StyleSettings.HideTopSurveyButtonsOnFirstAndLastPage = DbUtility.GetValueFromDataRow(responseTemplateDr, "HideTopSurveyButtonsOnFirstAndLastPage", false);
            BehaviorSettings.DisplayPDFDownloadButton = DbUtility.GetValueFromDataRow(responseTemplateDr, "DisplayPDFDownloadButton", false);

            StyleSettings.EnableDynamicPageNumbers = DbUtility.GetValueFromDataRow(responseTemplateDr, "EnableDynamicPageNumbers", false);
            StyleSettings.EnableDynamicItemNumbers = DbUtility.GetValueFromDataRow(responseTemplateDr, "EnableDynamicItemNumbers", false);
            StyleSettings.ShowProgressBar = DbUtility.GetValueFromDataRow(responseTemplateDr, "ShowProgressBar", false);
            StyleSettings.ProgressBarOrientation = DbUtility.GetValueFromDataRow(responseTemplateDr, "ProgressBarOrientation", ProgressBarOrientation.Top_Left);
            StyleSettings.ShowAsterisks = DbUtility.GetValueFromDataRow(responseTemplateDr, "ShowAsterisks", true);
            StyleSettings.HideFooterHeader = DbUtility.GetValueFromDataRow(responseTemplateDr, "HideFooterHeader", true);
            StyleSettings.ShowTitle = DbUtility.GetValueFromDataRow(responseTemplateDr, "ShowTitle", false);
            LanguageSettings.DefaultLanguage = DbUtility.GetValueFromDataRow(responseTemplateDr, "DefaultLanguage", TextManager.DefaultLanguage);
            LanguageSettings.SupportedLanguages = new List<string>(DbUtility.GetValueFromDataRow(responseTemplateDr, "SupportedLanguages", string.Empty).Split(';'));
            LanguageSettings.LanguageSource = DbUtility.GetValueFromDataRow(responseTemplateDr, "LanguageSource", string.Empty);
            LanguageSettings.LanguageSourceToken = DbUtility.GetValueFromDataRow(responseTemplateDr, "LanguageSourceToken", string.Empty);
            BehaviorSettings.EnableScoring = DbUtility.GetValueFromDataRow(responseTemplateDr, "EnableScoring", false);
            BehaviorSettings.AllowFormReset = DbUtility.GetValueFromDataRow(responseTemplateDr, "AllowFormReset", false);
            IsDeleted = DbUtility.GetValueFromDataRow(responseTemplateDr, "Deleted", false);
            BehaviorSettings.RandomizeItemsInPages = DbUtility.GetValueFromDataRow(responseTemplateDr, "RandomizeItemsInPages", false);
            StyleSettings.ShowValidationMessage = DbUtility.GetValueFromDataRow(responseTemplateDr, "ShowValidationMessage", false);
            StyleSettings.ShowValidationErrorAlert = DbUtility.GetValueFromDataRow(responseTemplateDr, "RequiredFieldsAlert", false);
            BehaviorSettings.CompletionType = DbUtility.GetValueFromDataRow(responseTemplateDr, "CompletionType", -1);
            BehaviorSettings.DisableBackButton = DbUtility.GetValueFromDataRow(responseTemplateDr, "DisableBackButton", false);
            IsPoll = DbUtility.GetValueFromDataRow(responseTemplateDr, "IsPoll", false);
            StyleSettings.ChartStyleId = DbUtility.GetValueFromDataRow<int?>(responseTemplateDr, "ChartStyleID", null);
            StyleSettings.Height = DbUtility.GetValueFromDataRow<int?>(responseTemplateDr, "Height", null);
            StyleSettings.Width = DbUtility.GetValueFromDataRow<int?>(responseTemplateDr, "Width", null);
            StyleSettings.BorderWidth = DbUtility.GetValueFromDataRow<int?>(responseTemplateDr, "BorderWidth", null);
            StyleSettings.BorderColor = DbUtility.GetValueFromDataRow(responseTemplateDr, "BorderColor", string.Empty);
            StyleSettings.BorderStyle = DbUtility.GetValueFromDataRow(responseTemplateDr, "BorderStyle", string.Empty);
            BehaviorSettings.AllowSurveyEditWhileActive = DbUtility.GetValueFromDataRow(responseTemplateDr, "AllowSurveyEditWhileActive", false);
            BehaviorSettings.ShowSaveAndQuit = DbUtility.GetValueFromDataRow(responseTemplateDr, "ShowSaveAndQuit", false);
            BehaviorSettings.Password = DbUtility.GetValueFromDataRow<string>(responseTemplateDr, "GuestPassword", null);
            BehaviorSettings.GoogleAnalyticsTrackingID = DbUtility.GetValueFromDataRow<string>(responseTemplateDr, "GoogleAnalyticsTrackingID", null);
          
        }

        /// <summary>
        /// Load response terms from the supplied data
        /// </summary>
        /// <param name="data"></param>
        private void LoadResponseTerms(DataSet data)
        {
            _surveyTerms.Clear();

            if (data.Tables.Contains("ResponseTerms"))
            {
                foreach (DataRow row in data.Tables["ResponseTerms"].Rows)
                {
                    _surveyTerms.Add(new SurveyTerm()
                    {
                        Id = Convert.ToInt32(row["ID"]),
                        Name = (string)row["Name"],
                        Term = (string)row["Term"],
                        Definition = (string)row["Definition"],
                        Link = (string)row["Link"]
                    });
                }
            }
        }

        private void LoadResponsePipes(DataSet data)
        {
            //Load pipe information
            _responsePipes.Clear();

            if (data.Tables.Contains("ResponsePipes"))
            {
                foreach (DataRow row in data.Tables["ResponsePipes"].Rows)
                {
                    if (row["PipeName"] != DBNull.Value && row["ItemID"] != DBNull.Value)
                    {
                        var responsePipeData = new ItemToken((string)row["PipeName"], Convert.ToInt32(row["ItemID"]));
                        _responsePipes[responsePipeData.TokenName] = responsePipeData;
                    }
                }
            }
        }

        /// <summary>
        /// Copy a response template and return the copied template
        /// </summary>
        /// <param name="source">template to copy</param>
        /// <returns></returns>
        public static ResponseTemplate Copy(ResponseTemplate source)
		{
			return ResponseTemplateManager.CopyTemplate(source.ID.Value, null, Users.UserManager.GetCurrentPrincipal(), TextManager.DefaultLanguage);
		}

		/// <summary>
		/// 
		/// </summary>
        /// <param name="xmlNode"></param>
        protected override void LoadTemplateData(XmlNode xmlNode)
		{
            //For backwards compatibility, look for TemplateData node
		    var templateNode = xmlNode.SelectSingleNode("TemplateData") ?? xmlNode;

		    Name = XmlUtility.GetNodeText(templateNode.SelectSingleNode("TemplateName"), false);
            
            BehaviorSettings.ActivationStartDate = XmlUtility.GetNodeDate(templateNode.SelectSingleNode("ActivationStart"));
            BehaviorSettings.ActivationEndDate = XmlUtility.GetNodeDate(templateNode.SelectSingleNode("ActivationEnd"));

            BehaviorSettings.MaxTotalResponses = XmlUtility.GetNodeInt(templateNode.SelectSingleNode("MaxTotalResponses"));
            BehaviorSettings.MaxResponsesPerUser = XmlUtility.GetNodeInt(templateNode.SelectSingleNode("MaxResponsesPerUser"));

            BehaviorSettings.AllowContinue = XmlUtility.GetNodeBool(templateNode.SelectSingleNode("AllowContinue"));
            BehaviorSettings.AllowEdit = XmlUtility.GetNodeBool(templateNode.SelectSingleNode("AllowEdit"));
            BehaviorSettings.EnableScoring = XmlUtility.GetNodeBool(templateNode.SelectSingleNode("EnableScoring"));
            BehaviorSettings.AllowFormReset = XmlUtility.GetNodeBool(templateNode.SelectSingleNode("AllowFormReset"));
            BehaviorSettings.RandomizeItemsInPages = XmlUtility.GetNodeBool(templateNode.SelectSingleNode("RandomizeItemsInPages"));
            BehaviorSettings.CompletionType = XmlUtility.GetNodeInt(templateNode.SelectSingleNode("CompletionType")) ?? 1;
            BehaviorSettings.DisableBackButton = XmlUtility.GetNodeBool(templateNode.SelectSingleNode("DisableBackButton"));
            BehaviorSettings.AllowSurveyEditWhileActive = XmlUtility.GetNodeBool(templateNode.SelectSingleNode("AllowSurveyEditWhileActive"));
            BehaviorSettings.ShowSaveAndQuit = XmlUtility.GetNodeBool(templateNode.SelectSingleNode("ShowSaveAndQuit"));
            BehaviorSettings.Password = XmlUtility.GetNodeText(templateNode.SelectSingleNode("Password"));
            BehaviorSettings.AnonymizeResponses = XmlUtility.GetNodeBool(templateNode.SelectSingleNode("AnonymizeResponses"));
            BehaviorSettings.GoogleAnalyticsTrackingID = XmlUtility.GetNodeText(templateNode.SelectSingleNode("GoogleAnalyticsTrackingID"));

            MobileCompatible = XmlUtility.GetNodeBool(templateNode.SelectSingleNode("MobileCompatible"));

            StyleSettings.ShowItemNumbers = XmlUtility.GetNodeBool(templateNode.SelectSingleNode("ShowItemNumbers"));
            StyleSettings.ShowPageNumbers = XmlUtility.GetNodeBool(templateNode.SelectSingleNode("ShowPageNumbers"));
            StyleSettings.EnableDynamicPageNumbers = XmlUtility.GetNodeBool(templateNode.SelectSingleNode("EnableDynamicPageNumbers"));
            StyleSettings.EnableDynamicItemNumbers = XmlUtility.GetNodeBool(templateNode.SelectSingleNode("EnableDynamicItemNumbers"));
            StyleSettings.ShowProgressBar = XmlUtility.GetNodeBool(templateNode.SelectSingleNode("ShowProgressBar"));
            StyleSettings.ProgressBarOrientation = (ProgressBarOrientation?)XmlUtility.GetNodeInt(templateNode.SelectSingleNode("ProgressBarOrientation"));
            StyleSettings.ShowTitle = XmlUtility.GetNodeBool(templateNode.SelectSingleNode("ShowTitle"));
            StyleSettings.ShowAsterisks = XmlUtility.GetNodeBool(templateNode.SelectSingleNode("ShowAsterisks"));
            StyleSettings.HideFooterHeader = XmlUtility.GetNodeBool(templateNode.SelectSingleNode("HideFooterHeader"));
            StyleSettings.ShowValidationMessage = XmlUtility.GetNodeBool(templateNode.SelectSingleNode("ShowValidationMessage"));
            StyleSettings.ShowValidationErrorAlert = XmlUtility.GetNodeBool(templateNode.SelectSingleNode("ShowValidationErrorAlert"));
            StyleSettings.ChartStyleId = XmlUtility.GetNodeInt(templateNode.SelectSingleNode("ChartStyleId"));
            StyleSettings.Height = XmlUtility.GetNodeInt(templateNode.SelectSingleNode("Height"));
            StyleSettings.Width = XmlUtility.GetNodeInt(templateNode.SelectSingleNode("Width"));
            StyleSettings.BorderWidth = XmlUtility.GetNodeInt(templateNode.SelectSingleNode("BorderWidth"));
            StyleSettings.BorderColor = XmlUtility.GetNodeText(templateNode.SelectSingleNode("BorderColor"));
            StyleSettings.BorderStyle = XmlUtility.GetNodeText(templateNode.SelectSingleNode("BorderStyle"));

            LanguageSettings.DefaultLanguage = XmlUtility.GetNodeText(templateNode.SelectSingleNode("DefaultLanguage"));

            for (int i = LanguageSettings.SupportedLanguages.Count - 1; i >= 0; i--)
                if (string.IsNullOrEmpty(LanguageSettings.SupportedLanguages[i]))
                    LanguageSettings.SupportedLanguages.RemoveAt(i);

            LanguageSettings.SupportedLanguages.AddRange(XmlUtility.GetNodeText(templateNode.SelectSingleNode("SupportedLanguages")).Trim(';').Split(';'));

            LanguageSettings.LanguageSource = XmlUtility.GetNodeText(templateNode.SelectSingleNode("LanguageSource"));
            LanguageSettings.LanguageSourceToken = XmlUtility.GetNodeText(templateNode.SelectSingleNode("LanguageSourceToken"));
            IsDeleted = XmlUtility.GetNodeBool(templateNode.SelectSingleNode("IsDeleted"));
            IsPoll = XmlUtility.GetNodeBool(templateNode.SelectSingleNode("IsPoll"));

            var terms = templateNode.SelectSingleNode("Terms");
            if (terms != null && terms.HasChildNodes)
            {
                foreach(XmlNode term in terms.ChildNodes)
                {
                    var newTerm = new SurveyTerm()
                    {
                        Name = XmlUtility.GetNodeText(term.SelectSingleNode("Name")),
                        Term = XmlUtility.GetNodeText(term.SelectSingleNode("Term")),
                        Definition = XmlUtility.GetNodeText(term.SelectSingleNode("Definition")),
                        Link = XmlUtility.GetNodeText(term.SelectSingleNode("Link"))
                    };
                    SurveyTerms.Add(newTerm);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        protected override void LoadTemplateCustomTextData(XmlNode xmlNode)
        {
            //check ID
            if (!ID.HasValue)
                return; //nowhere to save

            var customTextNode = xmlNode.SelectSingleNode("CustomTextData");

            //nothing to import
            if (customTextNode == null)
                return;

            foreach (XmlNode languageNode in customTextNode.ChildNodes)
            {
                string language = XmlUtility.GetAttributeText(languageNode, "Name");
                foreach (XmlNode textNode in languageNode.ChildNodes)
                {
                    TextManager.SetText(GetSurveySpecificTextId(textNode.Name, ID.Value), language, XmlUtility.GetNodeText(textNode));
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        protected override void LoadTemplatePageData(CheckboxPrincipal principal, XmlNode xmlNode, string progressKey, string progressLanguage, PageImportReader reader = null, bool forCopy = false)
        {
            var ruleDataService = new RuleDataService();
            ruleDataService.Initialize(ID.Value);

            //Initialize page reader with rule data
            var pageReader = new PageImportReader(ruleDataService);

            //Call base method to load pages and items
            base.LoadTemplatePageData(principal, xmlNode, progressKey, progressLanguage, pageReader, forCopy);

            //Now update ids of pages/items/options for rules
            pageReader.UpdateRules();

            //Update ids in the pipes
            pageReader.UpdatePipes(AddResponsePipe);

            //Resolves id dependencies 
            pageReader.ResolveIdDependencies();

            //Save rule data
            ruleDataService.SaveRuleData();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteTemplateData(XmlWriter writer)
        {
            writer.WriteHtml("TemplateName", Name);
            writer.WriteElementString("Guid", GUID.ToString());

            writer.WriteElementValue<DateTime>("ActivationStart", BehaviorSettings.ActivationStartDate);
            writer.WriteElementValue<DateTime>("ActivationEnd", BehaviorSettings.ActivationEndDate);
            writer.WriteElementValue<int>("MaxTotalResponses", BehaviorSettings.MaxTotalResponses);
            writer.WriteElementValue<int>("MaxResponsesPerUser", BehaviorSettings.MaxResponsesPerUser);
            writer.WriteElementString("AllowContinue", BehaviorSettings.AllowContinue.ToString());
            writer.WriteElementString("AllowEdit", BehaviorSettings.AllowEdit.ToString());
            writer.WriteElementString("EnableScoring", BehaviorSettings.EnableScoring.ToString());
            writer.WriteElementString("AllowFormReset", BehaviorSettings.AllowFormReset.ToString());
            writer.WriteElementString("RandomizeItemsInPages", BehaviorSettings.RandomizeItemsInPages.ToString());
            writer.WriteElementString("CompletionType", BehaviorSettings.CompletionType.ToString());
            writer.WriteElementString("DisableBackButton", BehaviorSettings.DisableBackButton.ToString());
            writer.WriteElementString("AllowSurveyEditWhileActive", BehaviorSettings.AllowSurveyEditWhileActive.ToString());

            writer.WriteElementString("DisplayPDFDownloadButton", BehaviorSettings.DisplayPDFDownloadButton.ToString());

            writer.WriteElementString("ShowSaveAndQuit", BehaviorSettings.ShowSaveAndQuit.ToString());
            writer.WriteElementString("Password", BehaviorSettings.Password);
            writer.WriteElementValue<bool>("AnonymizeResponses", BehaviorSettings.AnonymizeResponses);
            writer.WriteElementString("GoogleAnalyticsTrackingID", BehaviorSettings.GoogleAnalyticsTrackingID);

            writer.WriteElementString("MobileCompatible", MobileCompatible.ToString());

            writer.WriteElementString("ShowItemNumbers", StyleSettings.ShowItemNumbers.ToString());
            writer.WriteElementString("ShowPageNumbers", StyleSettings.ShowPageNumbers.ToString());
            writer.WriteElementString("EnableDynamicPageNumbers", StyleSettings.EnableDynamicPageNumbers.ToString());
            writer.WriteElementString("EnableDynamicItemNumbers", StyleSettings.EnableDynamicItemNumbers.ToString());
            writer.WriteElementString("ShowAsterisks", StyleSettings.ShowAsterisks.ToString());
            writer.WriteElementString("HideFooterHeader", StyleSettings.HideFooterHeader.ToString());
            writer.WriteElementString("ShowProgressBar", StyleSettings.ShowProgressBar.ToString());
            writer.WriteElementValue("ProgressBarOrientation", (int?)StyleSettings.ProgressBarOrientation);
            writer.WriteElementString("ShowTitle", StyleSettings.ShowTitle.ToString());
            writer.WriteElementString("ShowValidationMessage", StyleSettings.ShowValidationMessage.ToString());
            writer.WriteElementString("ShowValidationErrorAlert", StyleSettings.ShowValidationErrorAlert.ToString());
            writer.WriteElementValue<int>("ChartStyleId", StyleSettings.ChartStyleId);
            writer.WriteElementValue<int>("Height", StyleSettings.Height);
            writer.WriteElementValue<int>("Width", StyleSettings.Width);
            writer.WriteElementValue<int>("BorderWidth", StyleSettings.BorderWidth);
            writer.WriteElementString("BorderColor", StyleSettings.BorderColor);
            writer.WriteElementString("BorderStyle", StyleSettings.BorderStyle);

            writer.WriteElementString("DefaultLanguage", LanguageSettings.DefaultLanguage);

            StringBuilder sbLang = new StringBuilder();
            LanguageSettings.SupportedLanguages.ForEach((v) => sbLang.Append(v + ";"));

            writer.WriteElementString("SupportedLanguages", sbLang.ToString());
            writer.WriteElementString("LanguageSource", LanguageSettings.LanguageSource);
            writer.WriteElementString("LanguageSourceToken", LanguageSettings.LanguageSourceToken);
            writer.WriteElementString("IsDeleted", IsDeleted.ToString());
            writer.WriteElementString("IsPoll", IsPoll.ToString());

            writer.WriteStartElement("Terms");

            foreach (var term in SurveyTerms)
            {
                writer.WriteStartElement("Term");

                writer.WriteElementString("Name", term.Name);
                writer.WriteElementString("Term", term.Term);
                writer.WriteElementString("Definition", term.Definition);
                writer.WriteElementString("Link", term.Link);

                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteCustomTextData(XmlWriter writer)
        {
            writer.WriteStartElement("CustomTextData");
            Dictionary<string, string>.KeyCollection keys = ResponseTemplateManager.SurveyTextKeyMap.Keys;
            foreach (string language in ResponseTemplateManager.ActiveSurveyLanguages)
            {
                writer.WriteStartElement("Language");
                writer.WriteAttributeString("Name", language);
                foreach (string key in keys)
                {
                    string customText = TextManager.GetText(GetSurveySpecificTextId(key, ID.Value), language);
                    if (!String.IsNullOrEmpty(customText))
                        writer.WriteHtml(key, customText);
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override TemplatePageExportWriter GetPageExportWriter()
        {
            var rules = new RuleDataService();
            rules.Initialize(ID.Value);

            return new TemplatePageExportWriter(rules, GetItem);
        }

        /// <summary>
        /// Override an item being saved to update any rules that may have become invalidated.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="t"></param>
        protected override void OnItemSaved(ItemData item, IDbTransaction t)
        {
            base.OnItemSaved(item, t);

            ValidateItemRules(t);
        }


        /// <summary>
        /// Validate item source rules for items in the survey
        /// </summary>
        public virtual void ValidateItemRules()
        {
            ValidateItemRules(null);
        }

        /// <summary>
        /// Validate item source rules for items in the survey
        /// </summary>
        public virtual void ValidateItemRules(IDbTransaction t)
        {
            RuleDataService service = CreateRuleDataService(ID.Value);

            if (service.DeleteInvalidSubscriberExpressions(ItemIds))
            {
                if (t != null)
                {
                    service.SaveRuleData(t);
                }
                else
                {
                    service.SaveRuleData();
                }
            }
        }

        /// <summary>
        /// Add child items to the list
        /// </summary>
        /// <param name="parentItemData"></param>
        /// <returns></returns>
        private static List<ItemData> GetChildItems(ItemData parentItemData)
        {
            var childItems = new List<ItemData>();

            //TODO: Matrix
            //if (parentItemData is ICompositeItemData)
            //{
            //    foreach (ItemData childItemData in ((ICompositeItemData)parentItemData).GetChildItemDatas())
            //    {
            //        childItems.Add(childItemData);
            //        childItems.AddRange(GetChildItems(childItemData));
            //    }
            //}

            return childItems;
        }

        /// <summary>
        /// Clean up conditions based on a removed item.
        /// </summary>
        /// <param name="itemId">the id of the removed <see cref="ItemData"/></param>
        protected override void OnItemRemoved(int itemId)
        {
            RuleDataService service = CreateRuleDataService(ID.Value);
            service.DeleteSubscriberExpressions(itemId);
            service.SaveRuleData();

            base.OnItemRemoved(itemId);
        }

        /// <summary>
        /// Clean up conditions on a removed page.
        /// </summary>
        /// <param name="pageId">Id of the removed page</param>
        protected override void OnPageRemoved(int pageId)
        {
            RuleDataService service = CreateRuleDataService(ID.Value);            
            service.DeleteSubscriberExpressions(pageId);
            service.SaveRuleData();

            base.OnPageRemoved(pageId);
        }

        /// <summary>
        /// Delete the item from the page and also clear up any response pipes with the item as a source
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="itemId"></param>
        public override void DeleteItemFromPage(int pageId, int itemId)
        {
            //Remove tokens
            foreach (ItemToken itemToken in ResponsePipes)
            {
                if (itemToken.ItemID == itemId)
                {
                    DeleteResponsePipe(itemToken.TokenName);
                }
            }

            RuleDataService service = CreateRuleDataService(ID.Value);
            // Here we delete any rules that this item touches
            service.DeleteSubscriberExpressions(itemId);
            service.SaveRuleData();

            //Delete items
            base.DeleteItemFromPage(pageId, itemId);
        }

        /// <summary>
        /// Overridden. Deletes a <see cref="TemplatePage"/> adding functionality to account for Rule logic associated with the 
        /// <see cref="TemplatePage"/> to be deleted
        /// </summary>
        /// <param name="pageId">ID of a <see cref="TemplatePage"/></param>
        public override void DeletePage(int pageId)
        {
            RuleDataService service = CreateRuleDataService(ID.Value);
            // Here we delete any rules that this page or its children my observe
            service.DeletePageRules(pageId);

            // now delete dependencies upon any ItemData's within this Page
            foreach (var itemId in PageItems[pageId])
            {
                service.DeleteSubscriberExpressions(itemId);
                CleanupItemCaches(itemId);
            }

            //Delete rules where this page is the action target 
            service.DeletePageTargetExpressions(pageId);

            service.SaveRuleData();

            base.DeletePage(pageId);
        }

        /// <summary>
        /// Overridden.  Moves a <see cref="TemplatePage"/> within this <see cref="ResponseTemplate"/> adding functionality to account for Rule logic associated with the 
        /// <see cref="TemplatePage"/> to be moved
        /// </summary>
        /// <param name="pageId">the <see cref="TemplatePage"/></param>
        /// <param name="position">the 'to' position</param>
        public override void MovePage(int pageId, int position)
        {
            RuleDataService service = CreateRuleDataService(ID.Value);
            TemplatePage page = GetPage(pageId);
            service.HandlePageMoved(pageId, position);

           
            if (page.Position < position)
            {
                // if the page is moving back in order, need to ensure that all item subscribers are 
                // still valid, i.e., that these items are still sequentially prior to any dependents
                foreach (int itemId in page.ListItemIds())
                {
                    service.NotifySubscribingExpressionsOfPublisherItemMoved(itemId, position, page);
                }
            }

            service.SaveRuleData();

            base.MovePage(pageId, position);
        }

        /// <summary>
        /// Overridden.  Moves an <see cref="ItemData"/> from one <see cref="TemplatePage"/> to another adding functionality 
        /// to account for Rule logic associated with the <see cref="ItemData"/> to be moved
        /// </summary>
        /// <param name="itemId">the <see cref="ItemData"/> to move</param>
        /// <param name="fromPageId">the <see cref="TemplatePage"/> to move it from</param>
        /// <param name="toPageId">the <see cref="TemplatePage"/> to move it to</param>
        /// <param name="position"></param>
        public override void MoveItemToPage(int itemId, int fromPageId, int toPageId, int position)
        {
            TemplatePage fromPage = GetPage(fromPageId);
            TemplatePage toPage = GetPage(toPageId);

            RuleDataService service = CreateRuleDataService(ID.Value);            
            service.HandleItemMoved(itemId, toPage.Position);            
            
            if (fromPage.Position < toPage.Position)
            {
                service.NotifySubscribingExpressionsOfPublisherItemMoved(itemId, toPage.Position, toPage);
            }

            service.SaveRuleData();
            
            base.MoveItemToPage(itemId, fromPageId, toPageId, position);
        }

        /// <summary>
        /// Determine if rules will be changed if the item moves to the page
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="fromPageId"></param>
        /// <param name="toPageId"></param>
        /// <returns></returns>
        public bool WillRulesBeChangedIfItemMovesToPage(int itemId, int fromPageId, int toPageId)
        {
            TemplatePage fromPage = GetPage(fromPageId);
            TemplatePage toPage = GetPage(toPageId);

            RuleDataService service = CreateRuleDataService(ID.Value);
            service.HandleItemMoved(itemId, toPage.Position);

            if (fromPage.Position < toPage.Position)
            {
                service.NotifySubscribingExpressionsOfPublisherItemMoved(itemId, toPage.Position, toPage);
            }

            return service.AreThereUnsavedChanges;
        }

        /// <summary>
        /// Determine if rules will be changed if the page deletes
        /// </summary>
        /// <param name="pageId"></param>
        public bool WillRulesBeChangedIfPageDeletes(int pageId)
        {
            RuleDataService service = CreateRuleDataService(ID.Value);
            // Here we delete any rules that this page or its children my observe
            service.DeletePageRules(pageId);

            // now delete dependencies upon any ItemData's within this Page
            foreach (var itemId in PageItems[pageId])
                service.DeleteSubscriberExpressions(itemId);

            //Delete rules where this page is the action target 
            service.DeletePageTargetExpressions(pageId);

            return service.AreThereUnsavedChanges;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool WillRulesBeChangedIfPageMoves(int pageId, int position)
        {
            RuleDataService service = CreateRuleDataService(ID.Value);
            service.HandlePageMoved(pageId, position);

            TemplatePage page = GetPage(pageId);
            if (page.Position < position)
            {
                // if the page is moving back in order, need to ensure that all item subscribers are 
                // still valid, i.e., that these items are still sequentially prior to any dependents
                foreach (int itemId in page.ListItemIds())
                {
                    service.NotifySubscribingExpressionsOfPublisherItemMoved(itemId, position, page);
                }
            }

            return service.AreThereUnsavedChanges;
        }

        

        //TODO: Hook this back up
        ///// <summary>
        ///// Overridden.  Adds functionality to save for Rule data associations
        ///// </summary>
        ///// <param name="transaction">a transaction scope in which to participate</param>
        //protected override void SavePageData(IDbTransaction transaction)
        //{
        //    // include a save to the RuleDataService in this transaction
        //    CreateRuleDataService().Save(transaction);
        //    base.SavePageData(transaction);
        //}

        ///// <summary>
        ///// Overridden.  Adds functionality to save for Rule data associations
        ///// </summary>
        ///// <param name="transaction">a transaction scope in which to participate</param>
        //protected override void SavePageItemData(IDbTransaction transaction)
        //{
        //    CreateRuleDataService().Save(transaction);
        //    base.SavePageItemData(transaction);
        //}

        /// <summary>
        /// Set default values for the template data
        /// </summary>
        private void SetDefaultValues()
        {
            try
            {
                //Set the response template properties
                ID = null;
                CreatedDate = DateTime.Now;
                LastModified = DateTime.Now;
                Name = string.Empty;
                BehaviorSettings.IsActive = false;
                BehaviorSettings.MaxTotalResponses = null;
                BehaviorSettings.MaxResponsesPerUser = null;
                BehaviorSettings.AllowContinue = false;
                BehaviorSettings.ShowSaveAndQuit = false;
                BehaviorSettings.AllowEdit = false;
                BehaviorSettings.AllowSurveyEditWhileActive = true;
                BehaviorSettings.DisplayPDFDownloadButton = true;
                MobileCompatible = false;
                BehaviorSettings.Password = string.Empty;
                StyleSettings.StyleTemplateId = null;
                StyleSettings.TabletStyleTemplateId = null;
                StyleSettings.SmartPhoneStyleTemplateId = null;
                StyleSettings.MobileStyleId = null;
                LanguageSettings.DefaultLanguage = string.Empty;
                BehaviorSettings.SecurityType = SecurityType.Public;
                BehaviorSettings.ReportSecurityType = ReportSecurityType.SummaryPrivate;
                AclID = null;
                DefaultPolicyID = -1;
                BehaviorSettings.EnableScoring = false;
                BehaviorSettings.AllowFormReset = false;
                IsDeleted = false;
            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessPrivate");

                if (rethrow)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Factory method returns a new <see cref="RuleDataService"/> wrapper with which to effect 
        /// Rule related configuration changes and operations on this <see cref="ResponseTemplate"/>
        /// </summary>
        /// <returns></returns>
        public static RuleDataService CreateRuleDataService(int responseTemplateId)
        {
            var rds = new RuleDataService();
            rds.Initialize(responseTemplateId);

            return rds;
        }


        #endregion

        #region Save Methods

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="t"></param>
        protected override void Create(IDbTransaction t)
        {
            //Call template create
            base.Create(t);

            if (ID == null || ID.Value <= 0)
            {
                throw new Exception("Unable to create response template with no DataID.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper rtCommand = db.GetStoredProcCommandWrapper("ckbx_sp_RT_Insert");
            rtCommand.AddInParameter("ResponseTemplateID", DbType.Int32, ID);
            AddCommonSprocParams(rtCommand);
            GUID = Guid.NewGuid();
            rtCommand.AddInParameter("GUID", DbType.Guid, GUID);
            rtCommand.AddInParameter("CreatedBy", DbType.String, CreatedBy);

            db.ExecuteNonQuery(rtCommand, t);

            //Update response pipes
            UpdateResponsePipes(t);
        }

        /// <summary>
        /// Update the response template
        /// </summary>
        /// <param name="t"></param>
        protected override void Update(IDbTransaction t)
        {
            base.Update(t);

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper rtCommand = db.GetStoredProcCommandWrapper("ckbx_sp_RT_Update");
            rtCommand.AddInParameter("ResponseTemplateID", DbType.Int32, ID);
            AddCommonSprocParams(rtCommand);
            db.ExecuteNonQuery(rtCommand, t);

            //Update response pipes
            UpdateResponsePipes(t);

            //Update terms
            UpdateTerms(ID.Value);
        }

        /// <summary>
        /// Update response pipe data
        /// </summary>
        private void UpdateResponsePipes(IDbTransaction t)
        {
            //Calling add/delete updates the internal pipe dictionary, so
            // get a copy.
            ReadOnlyCollection<ItemToken> pipesToUpdate = ResponsePipes;

            foreach (ItemToken token in pipesToUpdate)
            {
                //Update by deleting & re-adding
                DeleteResponsePipe(token.TokenName, t);
                AddResponsePipe(token.TokenName, token.ItemID, t);
            }
        }

        private void UpdateTerms(int id)
        {
            foreach (var term in SurveyTerms)
            {
                term.ResponseTemplateId = id;
                AddOrChangeSurveyTerm(term);
            }
        }

        #endregion

        #region IAccessControllable Members

        /// <summary>
        /// Gets the <see cref="SecurityEditor"/> for a <see cref="ResponseTemplate"/>
        /// </summary>
        /// <returns>a <see cref="SecurityEditor"/> of type <see cref="FormSecurityEditor"/></returns>
        public override SecurityEditor GetEditor()
        {
            return new FormSecurityEditor(this);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Add common insert/update parameters to the specified command
        /// </summary>
        /// <param name="command"></param>
        private void AddCommonSprocParams(DBCommandWrapper command)
        {
            if (command != null)
            {
                command.AddInParameter("TemplateName", DbType.String, Name);
                command.AddInParameter("NameTextID", DbType.String, LanguageSettings.NameTextId);
                command.AddInParameter("TitleTextID", DbType.String, LanguageSettings.TitleTextId);
                command.AddInParameter("DescriptionTextID", DbType.String, LanguageSettings.DescriptionTextId);
                command.AddInParameter("IsActive", DbType.Boolean, BehaviorSettings.IsActive);
                command.AddInParameter("ActivationStart", DbType.DateTime, BehaviorSettings.ActivationStartDate);
                command.AddInParameter("ActivationEnd", DbType.DateTime, BehaviorSettings.ActivationEndDate);
                command.AddInParameter("MaxTotalResponses", DbType.Int32, BehaviorSettings.MaxTotalResponses);
                command.AddInParameter("MaxResponsesPerUser", DbType.Int32, BehaviorSettings.MaxResponsesPerUser);
                command.AddInParameter("AllowContinue", DbType.Boolean, BehaviorSettings.AllowContinue);
                command.AddInParameter("ShowSaveAndQuit", DbType.Boolean, BehaviorSettings.ShowSaveAndQuit);
                command.AddInParameter("AllowEdit", DbType.Boolean, BehaviorSettings.AllowEdit);
                command.AddInParameter("AllowFormReset", DbType.Boolean, BehaviorSettings.AllowFormReset);
                command.AddInParameter("DisableBackButton", DbType.Boolean, BehaviorSettings.DisableBackButton);
                command.AddInParameter("AnonymizeResponses", DbType.Boolean, BehaviorSettings.AnonymizeResponses);
                command.AddInParameter("GoogleAnalyticsTrackingID", DbType.String, BehaviorSettings.GoogleAnalyticsTrackingID);

                command.AddInParameter("RandomizeItemsInPages", DbType.Boolean, BehaviorSettings.RandomizeItemsInPages);
                command.AddInParameter("ShowValidationMessage", DbType.Boolean, StyleSettings.ShowValidationMessage);
                command.AddInParameter("RequiredFieldsAlert", DbType.Boolean, StyleSettings.ShowValidationErrorAlert);

                command.AddInParameter("MobileCompatible", DbType.Boolean, MobileCompatible);
                command.AddInParameter("ButtonContinueTextID", DbType.String, LanguageSettings.ContinueButtonTextId);
                command.AddInParameter("ButtonBackTextID", DbType.String, LanguageSettings.BackButtonTextId);
                command.AddInParameter("GuestPassword", DbType.String, BehaviorSettings.Password);
                command.AddInParameter("StyleTemplateID", DbType.Int32, StyleSettings.StyleTemplateId);
                command.AddInParameter("MobileStyleId", DbType.Int32, StyleSettings.MobileStyleId);
                command.AddInParameter("TabletStyleTemplateID", DbType.Int32, StyleSettings.TabletStyleTemplateId);
                command.AddInParameter("SmartPhoneStyleTemplateID", DbType.Int32, StyleSettings.SmartPhoneStyleTemplateId);
                command.AddInParameter("ShowPageNumbers", DbType.Boolean, StyleSettings.ShowPageNumbers);

                command.AddInParameter("ShowTopSurveyButtons", DbType.Boolean, StyleSettings.ShowTopSurveyButtons);
                command.AddInParameter("HideTopSurveyButtonsOnFirstAndLastPage", DbType.Boolean, StyleSettings.HideTopSurveyButtonsOnFirstAndLastPage);
                command.AddInParameter("DisplayPDFDownloadButton", DbType.Boolean, BehaviorSettings.DisplayPDFDownloadButton);

                command.AddInParameter("EnableDynamicPageNumbers", DbType.Boolean, StyleSettings.EnableDynamicPageNumbers);
                command.AddInParameter("EnableDynamicItemNumbers", DbType.Boolean, StyleSettings.EnableDynamicItemNumbers);
                command.AddInParameter("ShowProgressBar", DbType.Boolean, StyleSettings.ShowProgressBar);
                command.AddInParameter("ProgressBarOrientation", DbType.Int32, (int?)StyleSettings.ProgressBarOrientation);
                command.AddInParameter("ShowItemNumbers", DbType.Boolean, StyleSettings.ShowItemNumbers);
                command.AddInParameter("ShowTitle", DbType.Boolean, StyleSettings.ShowTitle);
                command.AddInParameter("ShowAsterisks", DbType.Boolean, StyleSettings.ShowAsterisks);
                command.AddInParameter("HideFooterHeader", DbType.Boolean, StyleSettings.HideFooterHeader);

                command.AddInParameter("CompletionType", DbType.Int32, BehaviorSettings.CompletionType);

                //**********
                command.AddInParameter("AllowSurveyEditWhileActive", DbType.Boolean, BehaviorSettings.AllowSurveyEditWhileActive);
                //**********

                command.AddInParameter("IsPoll", DbType.Boolean, IsPoll);
                command.AddInParameter("ChartStyleID", DbType.Int32, StyleSettings.ChartStyleId);
                command.AddInParameter("Height", DbType.Int32, StyleSettings.Height);
                command.AddInParameter("Width", DbType.Int32, StyleSettings.Width);
                command.AddInParameter("BorderWidth", DbType.Int32, StyleSettings.BorderWidth);
                command.AddInParameter("BorderColor", DbType.String, StyleSettings.BorderColor);
                command.AddInParameter("BorderStyle", DbType.String, StyleSettings.BorderStyle);

                string languages = string.Empty;

                foreach (string language in LanguageSettings.SupportedLanguages)
                {
                    if (languages == string.Empty)
                    {
                        languages = language;
                    }
                    else
                    {
                        languages = languages + ";" + language;
                    }
                }
                command.AddInParameter("SupportedLanguages", DbType.String, languages);
                command.AddInParameter("DefaultLanguage", DbType.String, LanguageSettings.DefaultLanguage);
                command.AddInParameter("LanguageSource", DbType.String, LanguageSettings.LanguageSource);
                command.AddInParameter("LanguageSourceToken", DbType.String, LanguageSettings.LanguageSourceToken);
                command.AddInParameter("SecurityType", DbType.Int32, (int)BehaviorSettings.SecurityType);
                command.AddInParameter("ReportSecurityType", DbType.Int32, (int)BehaviorSettings.ReportSecurityType);
                command.AddInParameter("EnableScoring", DbType.Boolean, BehaviorSettings.EnableScoring);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the Data object by which a ResponseTemplate's state is loaded
        /// </summary>
        /// <returns></returns>
        public override PersistedDomainObjectDataSet GetConfigurationDataSet(int objectId)
        {
            //To avoid having each page, item, etc. call to the database, we use a system
            //where item data is retrieved all at once.  However, to support plugging new items
            //in, it will not be required that every item's data be in the retrieved dataset.  One issue
            //is that we will not, in code, know the names of each table returned by ckbx_GetResponseTemplate,
            //so what we'll do is look that up.

            //Step 1:  Look up the table names; table names must be ordered by position
            var tableNames = new List<string>();
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper dbNameCommand = db.GetStoredProcCommandWrapper("ckbx_sp_RT_GetTableNames");

            using (IDataReader reader = db.ExecuteReader(dbNameCommand))
            {
                try
                {
                    while (reader.Read())
                    {
                        tableNames.Add(reader["TableName"].ToString());
                    }
                }
                catch (Exception ex)
                {
                    bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessProtected");

                    if (rethrow)
                    {
                        throw;
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            //Step 2:  Get the data from the database, using the table names we just retrieved
            PersistedDomainObjectDataSet data = CreateConfigurationDataSet();
            data.OwningObjectId = objectId;
            DBCommandWrapper dbDataCommand = db.GetStoredProcCommandWrapper("ckbx_sp_RT_GetResponseTemplate");
            dbDataCommand.AddInParameter("ResponseTemplateID", DbType.Int32, objectId);
            db.LoadDataSet(dbDataCommand, data, tableNames.ToArray());

            if (data.Tables.Count == 0 || !data.Tables.Contains(DataTableName) || data.Tables[DataTableName].Rows.Count <= 0)
            {
                throw new TemplateDoesNotExist(objectId);
            }

            SetConfigurationDataSetColumnMappings(data);

            return data;
        }

        /// <summary>
        /// Ensure template has a hidden items page and a completion page
        /// </summary>
        protected virtual void EnsureIntrinsicPages()
        {
            bool needsHiddenItemsPage = false;
            bool needsCompletionPage = false;

            if (PageIds.Count == 0)
            {
                needsHiddenItemsPage = true;
                needsCompletionPage = true;
            }

            if (PageIds.Count > 0)
            {
                TemplatePage tmpPage = GetPage(PageIds[0]);

                if (tmpPage == null || tmpPage.PageType != TemplatePageType.HiddenItems)
                {
                    needsHiddenItemsPage = true;
                }

                tmpPage = GetPage(PageIds[PageIds.Count - 1]);

                if (tmpPage == null || tmpPage.PageType != TemplatePageType.Completion)
                {
                    needsCompletionPage = true;
                }
            }

            if (needsHiddenItemsPage)
            {
                //Base class saves page and handles adding things to collections
                base.CreatePage(1, TemplatePageType.HiddenItems);
            }

            if (needsCompletionPage)
            {
                //Base class saves page and handles adding things to collections
                base.CreatePage(PageIds.Count + 1, TemplatePageType.Completion);
            }
        }

        /// <summary>
        /// Override create page to validate page position
        /// </summary>
        /// <param name="position"></param>
        /// <param name="ensureIntrinsicPages"></param>
        /// <returns></returns>
        protected override TemplatePage CreatePage(int position, bool ensureIntrinsicPages)
        {
            //Ensure intrinsic pages for survey
            if (ensureIntrinsicPages)
                EnsureIntrinsicPages();

            //Ensure page has a positive position
            if (position <= 0)
            {
                throw new Exception("Page position for new page must be greater than 0.");
            }

            //Ensure page is before completion page
            var completion = PageIds.Count > 0
                ? GetPage(PageIds[PageIds.Count - 1])
                : null;

            if (completion != null
                && completion.Position < position)
            {
                //Since order of pages is stored in PageIds collection and new ids are
                // added via Insert, set position to be count - 1, which ensure it will 
                // be inserted before last (completion) page.
                position = PageIds.Count - 1;
            }

            return base.CreatePage(position, TemplatePageType.ContentPage);
        }

        /// <summary>
        /// Get the hidden items survey page.
        /// </summary>
        /// <returns></returns>
        public TemplatePage GetHiddenItemsPage()
        {
            EnsureIntrinsicPages();

            if (PageIds.Count > 0)
            {
                return GetPage(PageIds[0]);
            }

            return null;
        }

        /// <summary>
        /// Get the completion page
        /// </summary>
        /// <returns></returns>
        public TemplatePage GetCompletionPage()
        {
            EnsureIntrinsicPages();

            if (PageIds.Count > 0)
            {
                return GetPage(PageIds[PageIds.Count - 1]);
            }

            return null;
        }

        /// <summary>
        /// Get the response pipes for a survey
        /// </summary>
        public ReadOnlyCollection<ItemToken> ResponsePipes
        {
            get
            {
                return new ReadOnlyCollection<ItemToken>(_responsePipes.Values.ToList());
            }
        }

        /// <summary>
        /// Add a response pipe
        /// </summary>
        /// <param name="name"></param>
        /// <param name="itemID"></param>
        public void AddResponsePipe(string name, int itemID)
        {
            AddResponsePipe(name, itemID, null);
        }

        /// <summary>
        /// Add a response pipe using a transaction
        /// </summary>
        /// <param name="name"></param>
        /// <param name="itemID"></param>
        /// <param name="t"></param>
        private void AddResponsePipe(string name, int itemID, IDbTransaction t)
        {

            if (!string.IsNullOrEmpty(name) && !_responsePipes.ContainsKey(name) && itemID > 0)
            {
                _responsePipes[name] = new ItemToken(name, itemID);

                Database db = DatabaseFactory.CreateDatabase();
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_RT_AddResponsePipe");
                command.AddInParameter("ResponseTemplateID", DbType.Int32, ID.Value);
                command.AddInParameter("PipeName", DbType.String, name);
                command.AddInParameter("ItemID", DbType.Int32, itemID);

                if (t == null)
                {
                    db.ExecuteNonQuery(command);
                }
                else
                {
                    db.ExecuteNonQuery(command, t);
                }
            }
        }

        /// <summary>
        /// Delete a response pipe
        /// </summary>
        /// <param name="name"></param>
        public void DeleteResponsePipe(string name)
        {
            DeleteResponsePipe(name, null);
        }

        /// <summary>
        /// Delete a response pipe using a transaction
        /// </summary>
        /// <param name="name"></param>
        /// <param name="t"></param>
        private void DeleteResponsePipe(string name, IDbTransaction t)
        {
            if (name != null)
            {
                if (_responsePipes.ContainsKey(name))
                {
                    _responsePipes.Remove(name);

                    Database db = DatabaseFactory.CreateDatabase();
                    DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_RT_DeleteResponsePipe");
                    command.AddInParameter("ResponseTemplateID", DbType.Int32, ID.Value);
                    command.AddInParameter("PipeName", DbType.String, name);

                    if (t == null)
                    {
                        db.ExecuteNonQuery(command);
                    }
                    else
                    {
                        db.ExecuteNonQuery(command, t);
                    }
                }
            }
        }


        static readonly CacheManager _cache = CacheFactory.GetCacheManager("stateMachines");

        private static CacheManager CacheManager
        {
            get
            {
                return _cache;
            }
        }

        private RulesEngine CreateRulesEngine()
        {
            var rulesEngine = SurveyMetaDataProxy.GetRulesEngineFromCache(ID.Value);

            if (rulesEngine == null)
            {
                var allItemIds = new List<int>();

                //get all survey items ids
                foreach (int templatePageId in PageIds)
                {
                    var templatePage = GetPage(templatePageId);

                    if (templatePage != null)
                    {
                        var pageItemIds = templatePage.ListItemIds().ToList();
                        var childIds = ChildItemIds.Where(i => pageItemIds.Contains(i.Value)).Select(i => i.Key).ToList();

                        allItemIds.AddRange(pageItemIds);
                        allItemIds.AddRange(childIds);
                    }
                }

                var ruleDataSet = new RulesObjectSet();
                ruleDataSet.Load(ID.Value);

                rulesEngine = new RulesEngine();
                rulesEngine.Initialize(ruleDataSet, PageIds, allItemIds);

                SurveyMetaDataProxy.AddRulesEngineToCache(ID.Value, rulesEngine);
            }

            return rulesEngine;
        }

        /// <summary>
        /// Factory method creates an instance of <see cref="Response"/> using this <see cref="ResponseTemplate"/>
        /// </summary>
        /// <param name="languageCode">the language to create the Response in</param>
        /// <param name="responseGuid"> </param>
        /// <returns>a new <see cref="Response"/></returns>
        public Response CreateResponse(string languageCode, Guid responseGuid)
        {
            return CreateResponse(languageCode, responseGuid, false);
        }

        /// <summary>
        /// Factory method creates an instance of <see cref="Response"/> using this <see cref="ResponseTemplate"/>
        /// </summary>
        /// <param name="languageCode">the language to create the Response in</param>
        /// <returns>a new <see cref="Response"/></returns>
        public Response CreateResponse(string languageCode)
        {
            return CreateResponse(languageCode, null, false);
        }

        /// <summary>
        /// Factory method creates an instance of <see cref="Response"/> using this <see cref="ResponseTemplate"/>
        /// </summary>
        /// <param name="languageCode">the language to create the Response in</param>
        /// <param name="responseGuid"> </param>
        /// <param name="byPageItemCreation"></param>
        /// <returns>a new <see cref="Response"/></returns>
        public Response CreateResponse(string languageCode, Guid? responseGuid, bool byPageItemCreation)
        {
            var response = new Response { ResponseTemplateID = ID.Value, ResponseTemplateGuid = GUID, AnonymizeResponses = BehaviorSettings.AnonymizeResponses, LanguageCode = languageCode};

            bool isNew = !responseGuid.HasValue;
            if (isNew)
            {
                responseGuid = Guid.NewGuid();
                response.TemporaryGUID = responseGuid;
            }

            //create rule data 
            response.RulesEngine = CreateRulesEngine();

            //Add pages
            foreach (int templatePageId in PageIds)
            {
                var templatePage = GetPage(templatePageId);
                if (templatePage == null)
                    continue;

                var responsePage = new ResponsePage(
                    templatePageId,
                    templatePage.Position,
                    templatePage.PageType,
                    BehaviorSettings.RandomizeItemsInPages,
                    templatePage.LayoutTemplateId);

                var itemIds = templatePage.ListItemIds().ToList();

                foreach (var itemId in itemIds)
                {
                    if (!byPageItemCreation)
                    {
                        var item = LoadItem(itemId, languageCode, responseGuid.Value);
                        response.AddItem(item);
                    }

                    responsePage.AddItemID(itemId);
                }

                response.AddPage(responsePage);
            }

            if (byPageItemCreation)
                response.ReloadCurrentPage(this);

            //Load response pipes
            foreach (ItemToken responsePipeData in _responsePipes.Values)
            {
                response.ResponsePipes.Add(responsePipeData);
            }

            //Set navigation parameters and localized texts
            response.AllowBack = !BehaviorSettings.DisableBackButton;
            response.TemplateName = Name;
            response.ShowItemNumbers = StyleSettings.ShowItemNumbers;
            response.UseDynamicItemNumbers = StyleSettings.EnableDynamicPageNumbers;//StyleSettings.EnableDynamicItemNumbers;
            response.UseDynamicPageNumbers = StyleSettings.EnableDynamicPageNumbers;
            response.ScoringEnabled = BehaviorSettings.EnableScoring;

            return response;
        }

        #region Async cache loading

        private class LoadPageParams
        {
            public int[] ItemIds { set; get; }
            public string LanguageCode { set; get; }
            public Guid ResponseGuid { set; get; }
            public string ApplicationContext { set; get; }
        }

        /// <summary>
        /// Add items to the queue for async pre-loading
        /// </summary>
        /// <param name="itemIds"></param>
        /// <param name="languageCode"></param>
        /// <param name="responseGuid"></param>
        public void LoadItemsAsync(int[] itemIds, string languageCode, Guid responseGuid)
        {
            ThreadPool.QueueUserWorkItem(LoadPageAsync, new LoadPageParams
            {
                ItemIds = itemIds,
                LanguageCode = languageCode,
                ResponseGuid = responseGuid,
                ApplicationContext = ApplicationManager.ApplicationDataContext
            });
        }
        
        /// <summary>
        /// 
        /// </summary>
        private void LoadPageAsync(object o)
        {
            var data = o as LoadPageParams;
            var isOnline = ApplicationManager.AppSettings.EnableMultiDatabase;
            if (data != null && (!isOnline || !string.IsNullOrEmpty(data.ApplicationContext)))
            {
                if (isOnline)
                    Thread.SetData(Thread.GetNamedDataSlot(DataContextProvider.APPLICATION_CONTEXT_KEY), data.ApplicationContext);

                foreach (var itemId in data.ItemIds)
                {
                    LoadItem(itemId, data.LanguageCode, data.ResponseGuid);
                }
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public Item LoadItem(int itemId, string languageCode, Guid responseGuid, Dictionary<int, bool> excludedFlags = null)
        {
            Item item = null;
            //get from cache
            if (CacheManager != null)
                item = CacheManager.GetData("Item_" + responseGuid + "_" + itemId + "_" + languageCode) as Item;

            return item ?? CreateItem(languageCode, itemId, responseGuid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="languageCode"></param>
        /// <param name="itemId"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public Item CreateItem(string languageCode, int itemId, Response response = null)
        {
            return CreateItem(languageCode, itemId, response, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="languageCode"></param>
        /// <param name="itemId"></param>
        /// <param name="responseGuid"></param>
        /// <returns></returns>
        public Item CreateItem(string languageCode, int itemId, Guid? responseGuid)
        {
            return CreateItem(languageCode, itemId, null, responseGuid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="languageCode"></param>
        /// <param name="itemId"></param>
        /// <param name="response"> </param>
        /// <param name="responseGuid"></param>
        /// <returns></returns>
        private Item CreateItem(string languageCode, int itemId, Response response, Guid? responseGuid)
        {
            Item theItem = null;

            var itemData = GetItem(itemId, false);

            if (itemData != null)
            {
                theItem = itemData.CreateItem(languageCode, ID, response);

                if (!responseGuid.HasValue && response != null)
                    responseGuid = response.GUID;

                if (responseGuid.HasValue && responseGuid.Value != default(Guid))
                {
                    string key = "Item_" + responseGuid.Value + "_" + itemId + "_" + languageCode;
                    CacheManager.Add(key, theItem);
                    AddItemCacheKey(itemId, key);
                }
            }
            return theItem;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cachableResponse"></param>
        /// <param name="excludedFlags"></param>
        /// <returns></returns>
        public Response InitializeResponseFromCached(CachableResponse cachableResponse, Dictionary<int, bool> excludedFlags = null)
        {
            var response = new Response(cachableResponse) { ResponseTemplateID = ID.Value, ResponseTemplateGuid = GUID, AnonymizeResponses = BehaviorSettings.AnonymizeResponses };

            //Add pages
            foreach (int templatePageId in PageIds)
            {
                var templatePage = GetPage(templatePageId);
                if (templatePage == null)
                    continue;

                var responsePage = new ResponsePage(
                    templatePageId,
                    templatePage.Position,
                    templatePage.PageType,
                    BehaviorSettings.RandomizeItemsInPages,
                    templatePage.LayoutTemplateId);

                var itemIds = templatePage.ListItemIds().ToList();
                itemIds.ForEach(i => responsePage.AddItemID(i));

                response.AddPage(responsePage);
            }

            response.InitVisiblePages(cachableResponse);
            response.ReloadCurrentPage(this);

            //Set navigation parameters and localized texts
            response.AllowBack = !BehaviorSettings.DisableBackButton;
            response.TemplateName = Name;
            response.ShowItemNumbers = StyleSettings.ShowItemNumbers;
            response.UseDynamicItemNumbers = StyleSettings.EnableDynamicPageNumbers;//StyleSettings.EnableDynamicItemNumbers;
            response.UseDynamicPageNumbers = StyleSettings.EnableDynamicPageNumbers;

            response.ScoringEnabled = BehaviorSettings.EnableScoring;            

            return response;
        }

        /// <summary>
        /// Add a language to the list of languages supported by the survey
        /// </summary>
        /// <param name="language"></param>
        public void AddSupportedLanguage(string language)
        {
            if (!LanguageSettings.SupportedLanguages.Contains(language))
            {
                LanguageSettings.SupportedLanguages.Add(language);
            }
        }

        /// <summary>
        /// Remove a language from the list of supported languages
        /// </summary>
        /// <param name="language"></param>
        public void RemoveSupportedLanguage(string language)
        {
            if (string.Compare(language, LanguageSettings.DefaultLanguage, true) != 0
                && LanguageSettings.SupportedLanguages.Contains(language, StringComparer.InvariantCultureIgnoreCase))
            {
                LanguageSettings.SupportedLanguages.Remove(language);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public override TemplatePage CopyPage(int pageId, int position, CheckboxPrincipal principal)
        {
            TemplatePage page = GetPage(pageId);
            TemplatePage newPage = base.CopyPage(pageId, position, principal);
            if (newPage == null || newPage.ID < 0)
                return newPage;

            //copy page conditions
            RuleDataService rds = CreateRuleDataService();
            if (rds == null)
                return newPage;

            RuleData condition = rds.GetConditionForPage(pageId);
            if (condition == null)
                return newPage;

            var stream = new MemoryStream();
            var writer = new XmlTextWriter(stream, Encoding.UTF8);
            writer.WriteStartElement("page");

            var pageRuleWriter = new PageRuleWriter(rds);
            pageRuleWriter.WriteRuleData(page, writer);

            writer.WriteEndElement();
            writer.Flush();

            stream.Seek(0, SeekOrigin.Begin);

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(stream);

            if (xmlDoc.FirstChild != null)
            {
                var pageRuleImportReader = new PageRuleImportReader(rds);
                pageRuleImportReader.ReadRuleData(newPage, xmlDoc.FirstChild);
            }
                        
            rds.SaveRuleData();

            return newPage;
        }

        //TODO: Figure out where this should go
        ///// <summary>
        ///// Returns a Dictionary which contains the list of all answerable located on the pages before
        ///// the specified page.
        ///// The results are formatted question text, question id.
        ///// </summary>
        ///// <param name="pageNumber">The current page number.</param>
        //public Dictionary<string, string> GetAnswerableQuestions(int? pageNumber)
        //{
        //    Dictionary<string, string> questions = new Dictionary<string, string>();
        //    foreach (TemplatePage page in TemplatePages)
        //    {
        //        if (pageNumber == null || pageNumber > page.Position)
        //        {
        //            int itemPos = 1;

        //            foreach (ItemData itemData in page.Items)
        //            {
        //                string textPrefix = string.Format("{0}.{1}", page.Position, itemPos);

        //                if (itemData is MatrixItemData)
        //                {
        //                    for (int row = 1; row <= ((MatrixItemData)itemData).RowCount; row++)
        //                    {
        //                        for (int column = 1; column <= ((MatrixItemData)itemData).ColumnCount; column++)
        //                        {
        //                            if (column != ((MatrixItemData)itemData).PrimaryKeyColumnIndex)
        //                            {
        //                                ItemData childItemData = ((MatrixItemData)itemData).GetItemAt(row, column);

        //                                if (childItemData != null && childItemData.ID.HasValue)
        //                                {
        //                                    string questionText = ItemConfigurationManager.GetMatrixItemCellText(itemData, row, column, EditLanguage, 64, false);
        //                                    questionText = string.Format("{0}.{1}.{2}  {3}", textPrefix, row, column, questionText);

        //                                    questions.Add(questionText, childItemData.ID.ToString());
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //                else if (itemData.ItemIsIAnswerable)
        //                {
        //                    string itemText = Utilities.StripHtml(ItemConfigurationManager.GetItemText(itemData, EditLanguage, null, false, false), 64);
        //                    itemText = string.Format("{0}  {1}", textPrefix, itemText);

        //                    questions.Add(itemText, itemData.ID.ToString());
        //                }

        //                itemPos++;
        //            }
        //        }
        //    }

        //    return questions;
        //}
        #endregion

        #region Public Properties

        /// <summary>
        /// Get survey behavior settings
        /// </summary>
        public SurveyBehaviorSettings BehaviorSettings
        {
            get { return _behaviorSettings ?? (_behaviorSettings = new SurveyBehaviorSettings()); }
        }

        public List<SurveyTerm> SurveyTerms
        {
            get { return _surveyTerms ?? (_surveyTerms = new List<SurveyTerm>()); }
        }

        /// <summary>
        /// Get survey style settings
        /// </summary>
        public SurveyStyleSettings StyleSettings
        {
            get { return _styleSettings ?? (_styleSettings = new SurveyStyleSettings()); }
        }

        /// <summary>
        /// 
        /// </summary>
        public SurveyLanguageSettings LanguageSettings
        {
            get
            {
                if (_languageSettings == null)
                {
                    _languageSettings = new SurveyLanguageSettings();
                }

                if (ID.HasValue)
                {
                    _languageSettings.SurveyId = ID.Value;
                }

                _languageSettings.TextIdPrefix = TextIDPrefix;

                return _languageSettings;
            }
        }



        /// <summary>
        /// Get/set a boolean indicating if this is a simpler form of response template called a poll.
        /// </summary>
        public bool IsPoll { get; set; }


        /// <summary>
        /// Gets the database identity of this <see cref="ResponseTemplate"/>
        /// </summary>
        internal int? Identity
        {
            set { ID = value; }
        }

        /// <summary>
        /// Gets the Globally Unique Identifier (GUID) for this <see cref="ResponseTemplate"/>
        /// </summary>
        public Guid GUID { get; private set; }

        /// <summary>
        /// Gets or sets the global name for this <see cref="ResponseTemplate"/>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets a flag indicating whether this <see cref="ResponseTemplate"/> has been deleted
        /// </summary>
        public bool IsDeleted { get; private set; }

        /// <summary>
        /// Gets or sets a flag indicating whether indicating whether the <see cref="ResponseTemplate"/> support Mobile compatible ItemTypes and settings
        /// </summary>
        public bool MobileCompatible { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public override string ExportElementName
        {
            get { return "ResponseTemplate"; }
        }

        #endregion

        #region Private Properties
        /// <summary>
        /// Gets the TextID for this ResponseTemplate's Name
        /// </summary>
        private string NameTextID
        {
            get
            {
                if (ID > 0)
                {
                    return "/" + TextIDPrefix + "/" + ID + "/name";
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the TextID for the Title
        /// </summary>
        public string TitleTextID
        {
            get
            {
                if (ID > 0)
                {
                    return "/" + TextIDPrefix + "/" + ID + "/title";
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the TextID for the Description
        /// </summary>
        private string DescriptionTextID
        {
            get
            {
                if (ID > 0)
                {
                    return "/" + TextIDPrefix + "/" + ID + "/description";
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the TextID for the Continue button
        /// </summary>
        public string ContinueButtonTextID
        {
            get
            {
                if (ID > 0)
                {
                    return "/" + TextIDPrefix + "/" + ID + "/continue";
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the TextID for the Back button
        /// </summary>
        public string BackButtonTextID
        {
            get
            {
                if (ID > 0)
                {
                    return "/" + TextIDPrefix + "/" + ID + "/back";
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the TextID for the Finish button
        /// </summary>
        public string FinishButtonTextID
        {
            get
            {
                if (ID > 0)
                {
                    return "/" + TextIDPrefix + "/" + ID + "/finish";
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Get the text id for the save progress button
        /// </summary>
        public string SaveProgressButtonTextID
        {
            get
            {
                if (ID > 0)
                {
                    return "/" + TextIDPrefix + "/" + ID + "/saveProgress";
                }

                return string.Empty;
            }
        }

        #endregion

        #region DBCommand

        /// <summary>
        /// Get command to delete page
        /// </summary>
        /// <returns></returns>
        protected override DBCommandWrapper GetDeletePageCommand()
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper delete = db.GetStoredProcCommandWrapper("ckbx_sp_RT_DeletePage");
            delete.AddInParameter("ResponseTemplateID", DbType.Int32, ID);
            delete.AddInParameter("PageID", DbType.Int32, "PageID", DataRowVersion.Current);

            return delete;
        }

        #endregion

        #region Filters for Analysis

        /// <summary>
        /// Get the report filters for this survey
        /// </summary>
        private ResponseTemplateFilterCollection FilterCollection
        {
            get
            {
                if (_filters == null)
                {
                    try
                    {
                        _filters = new ResponseTemplateFilterCollection { ParentID = ID.Value };
                        _filters.Load(ID.Value);
                    }
                    catch
                    {
                        _filters = null;
                        throw;
                    }
                }

                return _filters;
            }
        }

        /// <summary>
        /// Get the analysis filters associated with this response template
        /// </summary>
        public List<FilterData> GetFilterDataObjects()
        {
            return FilterCollection.GetFilterDataObjects();
        }

        /// <summary>
        /// Add a filter to this response template
        /// </summary>
        /// <param name="filter"></param>
        public void AddFilter(FilterData filter)
        {
            FilterCollection.AddFilter(filter);
        }

        /// <summary>
        /// Delete a filter
        /// </summary>
        /// <param name="filter"></param>
        public void DeleteFilter(FilterData filter)
        {
            FilterCollection.DeleteFilter(filter);
        }

        /// <summary>
        /// Delete a filter
        /// </summary>
        /// <param name="filterID"></param>
        public void DeleteFilter(Int32 filterID)
        {
            FilterCollection.DeleteFilter(filterID);
        }

        /// <summary>
        /// Save the filters
        /// </summary>
        public void SaveFilters()
        {
            if (FilterCollection != null)
            {
                Database db = DatabaseFactory.CreateDatabase();

                using (IDbConnection connection = db.GetConnection())
                {
                    try
                    {
                        connection.Open();
                        IDbTransaction t = connection.BeginTransaction();
                        try
                        {
                            FilterCollection.Save(t, db, false);
                            t.Commit();

                            //Force filters to be reloaded
                            _filters = null;
                        }
                        catch (Exception)
                        {
                            t.Rollback();
                            throw;
                        }
                    }
                    catch (Exception ex)
                    {
                        bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessPublic");

                        if (rethrow)
                        {
                            throw;
                        }
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Removes the item from all the caches
        /// </summary>
        /// <param name="itemData"></param>
        public void CleanupItemCaches(int itemId)
        {
            //invalidate item data
            ItemConfigurationManager.InvalidateCachedItemData(itemId);
           
            if (CacheManager != null)
            {
                string itemKey = "ResponsesByItem_" + itemId;
                List<string> responseKeys = CacheManager.GetData(itemKey) as List<string>;

                if (responseKeys != null)
                {
                    foreach (var key in responseKeys)
                    {
                        if (CacheManager.Contains(key))
                        {
                            CacheManager.Remove(key);
                        }
                    }
                    CacheManager.Remove(itemKey);
                }
            }
        }

        /// <summary>
        /// Stores response keys for item. We need them to remove from cache it the item will be updated 
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="key"></param>
        public void AddItemCacheKey(int itemId, string key)
        {
            if (CacheManager != null)
            {
                string itemKey = "ResponsesByItem_" + itemId;
                List<string> responseKeys = CacheManager.GetData(itemKey) as List<string> ?? new List<string>();

                if (!responseKeys.Contains(key))
                {
                    responseKeys.Add(key);
                    CacheManager.Add(itemKey, responseKeys);
                }
            }
        }

        public int AddOrChangeSurveyTerm(SurveyTerm termDefinition)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_RT_CreateOrUpdateResponseTerm");
            command.AddInParameter("ResponseTemplateID", DbType.Int32, termDefinition.ResponseTemplateId);
            command.AddInParameter("ID", DbType.Int32, termDefinition.Id);
            command.AddInParameter("Name", DbType.String, termDefinition.Name);
            command.AddInParameter("Term", DbType.String, termDefinition.Term);
            command.AddInParameter("Definition", DbType.String, termDefinition.Definition);
            command.AddInParameter("Link", DbType.String, termDefinition.Link);

            if (termDefinition.Id == 0)
            {
                return Convert.ToInt32(db.ExecuteScalar(command));
            }
            else
            {
                db.ExecuteNonQuery(command);
                return 0;
            }
        }

        public void DeleteSurveyTerm(int termId)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper delete = db.GetStoredProcCommandWrapper("ckbx_sp_RT_DeleteResponseTerm");
            delete.AddInParameter("ID", DbType.Int32, termId);
            db.ExecuteNonQuery(delete);
        }
    }
}
