using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using Checkbox.Configuration.Install;
using Checkbox.Globalization.Text;
using Checkbox.Management.Licensing.Limits;
using Checkbox.Management.Licensing.Limits.Static;
using Checkbox.LicenseLibrary;

namespace Checkbox.Management.Licensing
{
    /// <summary>
    /// Primary class for the Checkbox Licensing framework.
    /// </summary>
    public class CheckboxLicense : CheckboxLicenseData
    {
        private SurveyEditorLimit _surveyEditorLimit;
        private MandatoryCheckboxFooterLimit _mandatoryCheckboxFooterLimit;
        private RatingScaleStatisticsReportItemLimit _ratingScaleStatisticsReportItemLimit;
        private ScoredSurveyLimit _scoredSurveyLimit;
        private SimpleSecurityLimit _simpleSecurityLimit;
        private SpssLimit _spssLimit;
        private MultiLanguageLimit _multiLanguageLimit;
        private EmailLimit _emailLimit;
        private LibraryLimit _libraryLimit;

        /// <summary>
        /// 
        /// </summary>
        internal CheckboxLicense()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public CheckboxLicense(LicenseLimit[] limitArr)
        {
            if (limitArr == null)
                return;

            if (limitArr.Length == 0)
                return;

            _limits = new List<ILicenseLimit>();

            foreach (LicenseLimit limit in limitArr)
                _limits.Add(limit);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Dispose()
        {
            if (_limits != null)
            {
                _limits.Clear();
                _limits = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override string LicenseKey
        {
            get { return "Checkbox5"; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsTrial { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        internal bool IsValid { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string ValidationError { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public string LicenseId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? Timestamp { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SurveyEditorLimit SurveyEditorLimit
        {
            get { return _surveyEditorLimit ?? (_surveyEditorLimit = GetLimit("SurveyEditorLimit") as SurveyEditorLimit); }
        }

        /// <summary>
        /// 
        /// </summary>
        public MandatoryCheckboxFooterLimit MandatoryCheckboxFooterLimit
        {
            get
            {
                if (_mandatoryCheckboxFooterLimit == null)
                {
                    _mandatoryCheckboxFooterLimit = GetLimit("MandatoryCheckboxFooter") as MandatoryCheckboxFooterLimit;
                }

                return _mandatoryCheckboxFooterLimit;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public RatingScaleStatisticsReportItemLimit RatingScaleStatisticsReportItemLimit
        {
            get
            {
                if (_ratingScaleStatisticsReportItemLimit == null)
                {
                    _ratingScaleStatisticsReportItemLimit =
                        GetLimit("AllowRatingScaleStatisticsReportItem") as RatingScaleStatisticsReportItemLimit;
                }

                return _ratingScaleStatisticsReportItemLimit;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ScoredSurveyLimit ScoredSurveyLimit
        {
            get
            {
                if (_scoredSurveyLimit == null)
                {
                    _scoredSurveyLimit = GetLimit("AllowScoredSurvey") as ScoredSurveyLimit;
                }

                return _scoredSurveyLimit;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public SimpleSecurityLimit SimpleSecurityLimit
        {
            get
            {
                if (_simpleSecurityLimit == null)
                {
                    _simpleSecurityLimit = GetLimit("UseSimpleSecurity") as SimpleSecurityLimit;
                }

                return _simpleSecurityLimit;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public SpssLimit SpssLimit
        {
            get
            {
                if (_spssLimit == null)
                {
                    _spssLimit = GetLimit("AllowNativeSpssExport") as SpssLimit;
                }

                return _spssLimit;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public MultiLanguageLimit MultiLanguageLimit
        {
            get
            {
                if (_multiLanguageLimit == null)
                {
                    _multiLanguageLimit = GetLimit("AllowMultiLanguage") as MultiLanguageLimit;
                }

                return _multiLanguageLimit;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public LibraryLimit LibraryLimit
        {
            get { return _libraryLimit ?? (_libraryLimit = GetLimit("AllowLibraries") as LibraryLimit); }
        }

        /// <summary>
        /// 
        /// </summary>
        public EmailLimit EmailLimit
        {
            get
            {
                if (_emailLimit == null)
                {
                    _emailLimit = GetLimit("EmailLimit") as EmailLimit;
                }

                return _emailLimit;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string TimeStampString { get; set; }

        /// <summary>
        /// Add limits from hosting DB
        /// </summary>
        internal void AddLimitsFromHostingDB()
        {
            AddLimitFromHostingDB("EmailLimit");
            AddLimitFromHostingDB("MandatoryCheckboxFooterLimit");
            AddLimitFromHostingDB("SimpleSecurityLimit");
            AddLimitFromHostingDB("SpssLimit");
            AddLimitFromHostingDB("RatingScaleStatisticsReportItemLimit");
            AddLimitFromHostingDB("ScoredSurveyLimit");
            AddLimitFromHostingDB("MultiLanguageLimit");
            AddLimitFromHostingDB("SurveyEditorLimit");
            AddLimitFromHostingDB("LibraryLimit");
        }

        /// <summary>
        /// Add limit from hosting DB.
        /// </summary>
        /// <param name="limitName"></param>
        private void AddLimitFromHostingDB(String limitName)
        {
            //If this limit doesn't exist yet - create it.
            if (!_limits.Any(p => p.LimitName.Equals(limitName)))
            {
                Type type = LicenseLimit.GetLimit(limitName);
                LicenseLimit limit = Activator.CreateInstance(type) as LicenseLimit;

                _limits.Add(limit);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        internal void ReadFromFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                IsValid = false;
                ValidationError = "No license file path specified.";
                return;
            }

            if (!File.Exists(filePath))
            {
                IsValid = false;
                ValidationError = "Specified license file path does not exist.";
                return;
            }
            try
            {
                var licenseText = File.ReadAllText(filePath);

                if (string.IsNullOrEmpty(licenseText))
                {
                    IsValid = false;
                    ValidationError = "License file is empty";
                    return;
                }

                using (var stringReader = new StringReader(licenseText))
                {
                    try
                    {

                        var xmlTextReader = new XmlTextReader(stringReader);

                        try
                        {
                            ReadFromXml(xmlTextReader);
                        }
                        catch
                        {
                            IsValid = false;
                            ValidationError = "Error occurred reading license file, possibly due to invalid HTML.";
                        }

                    }
                    finally
                    {
                        stringReader.Close();
                    }
                }
            }
            catch (Exception)
            {
                IsValid = false;
                ValidationError = "Unable to read license file.";
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        internal void WriteToXml(XmlWriter writer)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("CheckboxLicense");

            if (!Timestamp.HasValue)
            {
                Timestamp = DateTime.Now;
            }

            TimeStampString = Timestamp.Value.ToUniversalTime().ToString("o");

            writer.WriteElementString("Serial", ComputeHash());
            writer.WriteElementString("LicenseId", LicenseId);
            writer.WriteElementString("Timestamp", TimeStampString);

            writer.WriteStartElement("Limits");
            writer.WriteAttributeString("Count", _limits.Count.ToString());

            foreach (LicenseLimit t in _limits)
            {
                if (t == null)
                    continue;

                writer.WriteStartElement(t.GetType().Name);
                writer.WriteAttributeString("Value", t.LimitValue);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();

            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected override void ReadFromXml(XmlReader reader)
        {
            if (!reader.MoveToNextElement("CheckboxLicense"))
            {
                IsValid = false;
                ValidationError = "Element missing from license file. (1)";
                return;
            }

            if (!reader.MoveToNextElement("Serial"))
            {
                IsValid = false;
                ValidationError = "Element missing from license file. (2)";
                return;
            }

            //Read serial
            string stringHash = reader.ReadElementContentAsString();


            //Read license id and timestamp
            if (reader.MoveToNextElement("LicenseId"))
            {
                LicenseId = reader.ReadElementContentAsString();
            }

            if (!reader.MoveToNextElement("Timestamp"))
            {
                IsValid = false;
                ValidationError = "Element missing from license file. (3)";
                return;
            }

            //Read time stamp string, which can be used for hashing purposes.
            TimeStampString = reader.ReadElementContentAsString();

            if (string.IsNullOrEmpty(TimeStampString))
            {
                IsValid = false;
                ValidationError = "License could not be validated. (t3)";
                return;
            }

            Timestamp = DateTime.Parse(TimeStampString);

            if (!reader.MoveToNextElement("Limits"))
            {
                IsValid = false;
                ValidationError = "Element missing from license file. (l2)";
                return;
            }

            reader.MoveToAttribute(0);

            int count = int.Parse(reader.Value);
            //writer.WriteStartElement("Limits");

            for (int i = 0; i < count; i++)
            {
                reader.MoveToNextElement(null);

                string limitTypeName = reader.Name;
                reader.MoveToAttribute(0);
                string limitValue = reader.Value;

                LicenseLimit limit = LicenseLimit.Create(limitTypeName, limitValue);

                _limits.Add(limit);
            }

            byte[] serialBytes = ComputeSerial(_limits, LicenseId, TimeStampString);
            byte[] serialHash = Hmacsha256.ComputeHash(serialBytes);
            byte[] xmlSerial = Convert.FromBase64String(stringHash);

            if (serialHash.Length != xmlSerial.Length)
            {
                _limits.Clear();

                IsValid = false;
                ValidationError = "License could not be validated. (4)";
                return;
            }

            if (xmlSerial.Where((t, i) => serialHash[i] != t).Any())
            {
                _limits.Clear();
                IsValid = false;
                ValidationError = "License could not be validated. (5)";
                return;
            }

            //Ensure trial limit present
            var typeLimit =
                _limits.FirstOrDefault(
                    limit => limit.LimitName.Equals("TrialLimit", StringComparison.InvariantCultureIgnoreCase));

            IsTrial = typeLimit != null && !"False".Equals(typeLimit.LimitValue, StringComparison.InvariantCultureIgnoreCase);

            if (!ApplicationManager.AppSettings.EnableMultiDatabase)
            {
                //check for minimum version
                string errorText;
                var minResult = ValidateLimit("VersionMinLimit", out errorText);
                if (minResult != LimitValidationResult.LimitNotReached)
                {
                    _limits.Clear();
                    IsValid = false;
                    ValidationError = errorText ?? "License could not be validated. (6)";
                    return;
                }

                //check for maximum version
                var maxResult = ValidateLimit("VersionMaxLimit", out errorText);
                if (maxResult != LimitValidationResult.LimitNotReached)
                {
                    _limits.Clear();
                    IsValid = false;
                    ValidationError = errorText ?? "License could not be validated. (6)";
                    return;
                }
            }

            IsValid = true;
            ValidationError = string.Empty;
        }

        private LimitValidationResult ValidateLimit(string name, out string errorText)
        {
            var versionLimit =
                _limits.FirstOrDefault(
                    limit => limit.LimitName.Equals(name, StringComparison.InvariantCultureIgnoreCase));

            if (versionLimit == null)
            {
                errorText = TextManager.GetText("/versionLimit/versionIsLow").Replace("{0}", ApplicationInstaller.ApplicationAssemblyShortVersion.ToString());
                return LimitValidationResult.UnableToEvaluate;
            }

            return versionLimit.Validate(out errorText);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToString(string format)
        {
            var sb = new StringBuilder();
            if (format == "xx33#8392sx#)(cj  s")
            {
                var sw = new StringWriter(sb);
                var writer = new XmlTextWriter(sw) { Formatting = Formatting.Indented };

                WriteToXml(writer);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Write this license to XML file
        /// </summary>
        /// <param name="filePath"></param>
        public void Save(string filePath)
        {
            File.WriteAllText(filePath, ToString("xx33#8392sx#)(cj  s"));
        }
    }
}
