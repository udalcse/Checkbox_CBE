using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Xml;
using System.IO;

namespace Checkbox.LicenseLibrary
{
    /// <summary>
    /// Primary class for the Checkbox Licensing framework.
    /// </summary>
    public class CheckboxLicenseData : License
    {
        protected static readonly HMACSHA256 Hmacsha256;
        protected List<ILicenseLimit> _limits;

        /// <summary>
        /// 
        /// </summary>
        static CheckboxLicenseData()
        {
            // 64 bit key
            Hmacsha256 = new HMACSHA256(new byte[] { 5, 77, 44, 2, 0, 90, 200, 110 });
        }

        /// <summary>
        /// 
        /// </summary>
        public CheckboxLicenseData()
        {
            _limits = new List<ILicenseLimit>();
        }

        /// <summary>
        /// 
        /// </summary>
        public CheckboxLicenseData(ILicenseLimit[] limitArr)
        {
            if (limitArr == null)
                return;

            if (limitArr.Length == 0)
                return;

            _limits = new List<ILicenseLimit>();

            foreach (ILicenseLimit limit in limitArr)
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
        internal string ValidationError { get; private set; }

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
        public string TimeStampString { get; set; }

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

            foreach (ILicenseLimit t in _limits)
            {
                if (t == null)
                    continue;

                writer.WriteStartElement(GetLimitName(t));
                writer.WriteAttributeString("Value", t.LimitValue);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();

            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

        protected virtual string GetLimitName(ILicenseLimit limit)
        {
            return limit.LimitName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected virtual void ReadFromXml(XmlReader reader)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string ComputeHash()
        {
            byte[] serialBytes = ComputeSerial(_limits, LicenseId, TimeStampString);

            byte[] serialHash = Hmacsha256.ComputeHash(serialBytes);

            return Convert.ToBase64String(serialHash);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="limitList"></param>
        /// <param name="licenseId"></param>
        /// <param name="timeStampString"></param>
        /// <returns></returns>
        protected static byte[] ComputeSerial(IEnumerable<ILicenseLimit> limitList, string licenseId, string timeStampString)
        {
            var serial = new StringBuilder();

            foreach (ILicenseLimit t in limitList)
            {
                if (t == null)
                    continue;

                serial.Append(t.LimitTypeName);
                serial.Append(t.LimitValue);
                serial.Append(' ');
            }


            if (!string.IsNullOrEmpty(licenseId))
            {
                serial.Append(licenseId);
                serial.Append(' ');
            }

            serial.Append(timeStampString);


            return Encoding.UTF8.GetBytes(serial.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueName"></param>
        /// <returns></returns>
        public string GetValue(string valueName)
        {
            if (_limits == null)
                return null;

            return (from t in _limits where t.LimitName == valueName select t.LimitValue).FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ILicenseLimit GetLimit(string name)
        {
            if (_limits == null)
                return null;

            return (from t in _limits where t.LimitName == name select t).FirstOrDefault();
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

        /// <summary>
        /// Writes this license to the stream
        /// </summary>
        /// <param name="s"></param>
        /// <param name="enc"></param>
        public void ToStream(Stream s, Encoding enc)
        {
            var writer = new XmlTextWriter(s, enc){Formatting = Formatting.Indented};
            WriteToXml(writer);
            writer.Flush();
            writer.Close();
        }
    }
}
