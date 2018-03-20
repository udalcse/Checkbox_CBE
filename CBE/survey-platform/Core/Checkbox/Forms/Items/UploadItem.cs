using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Xml;
using Checkbox.Common;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Validation;
using Checkbox.Globalization.Text;
using Checkbox.Management;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Data;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Upload items during a survey
    /// </summary>
    [Serializable]
    public class UploadItem : LabelledItem
    {
        #region Member Variables
        private List<string> _allowedFileTypes;

        #endregion

        #region Properties

        /// <summary>
        /// Get/Set the uploaded file.
        /// </summary>
        public byte[] Data { get; private set; }

        /// <summary>
        /// Get/Set the file name of an uploaded file.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Used only for downloading files
        /// </summary>
        public long ResponseId { get; set; }

        /// <summary>
        /// Set/Get file uniqu identifier
        /// </summary>
        public Guid FileGuid { get; set; }

        /// <summary>
        /// Get the file size of an uploaded file in bytes.
        /// </summary>
        public int FileSize
        {
            get { return Data != null ? Data.Length : 0; }
        }

        /// <summary>
        /// Get the string representation of the file size in kilo bytes.
        /// </summary>
        public string FileSizeDisplay
        {
            get
            {
                if (1024 > FileSize)
                {
                    return string.Format("{0} B", FileSize);
                }

                return string.Format("{0} KB", (FileSize / 1024));
            }
        }

        /// <summary>
        /// Get/Set the file type of an uploaded file.
        /// </summary>
        public string FileType { get; private set; }

        /// <summary>
        /// Get/Set the list of files types which are approved for upload.
        /// </summary>
        public List<string> AllowedFileTypes
        {
            get { return _allowedFileTypes ?? new List<string>(); }
            private set { _allowedFileTypes = value; }
        }

        /// <summary>
        /// Get the list of files types which are approved for upload as a comma separated list.
        /// </summary>
        public string AllowedFileTypesCSV
        {
            get { return string.Join(", ", AllowedFileTypes.ToArray()); }
        }

        #endregion

        #region Constructor(s)
        /// <summary>
        /// Construct an uploaded item.
        /// </summary>
        public UploadItem() {}

        /// <summary>
        /// Construct an uploaded item.
        /// </summary>
        /// <param name="data">The binary data of the upoaded file.</param>
        /// <param name="fileName">The name of the uploaded file.</param>
        /// <param name="fileType">The type of the uploaded file.</param>
        /// <param name="responseId"> </param>
        /// <param name="fileGuid"> </param>
        public UploadItem(byte[] data, string fileName, string fileType, long responseId, Guid fileGuid)
        {
            Data = data;
            FileName = fileName;
            FileType = fileType;
            ResponseId = responseId;
            FileGuid = fileGuid;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Configure the item
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData configuration, string languageCode, int? templateId)
        {
            Visible = ExportMode != ExportMode.Pdf;
            if (!ApplicationManager.AppSettings.EnableUploadItem)
            {
                Excluded = true;
            }

            base.Configure(configuration, languageCode, templateId);
            AllowedFileTypes = ((UploadItemData) configuration).AllowedFileTypes;
        }

        /// <summary>
        /// Upload a file to the web server.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
        /// <param name="fileType"></param>
        public void UploadFile(string fileName, byte[] data, string fileType)
        {
            //Do nothing if no data specified
            if (Utilities.IsNullOrEmpty(fileName)
                || data == null
                || data.Length == 0)
            {
                return;
            }

            //populate member data so that is accessible in the OnAnswerDataSaved
            Data = data;
            FileName = fileName;
            FileType = fileType;

            SetAnswer(string.Format("{0} - {1}", FileName, FileSizeDisplay));

            if (ValidateFileType())
            {
                Response.SaveCurrentState();

                var answerIds = GetAllAnswerIds();
                if (FileType != null && answerIds != null && answerIds.Count > 0)
                {
                    FileGuid = Guid.NewGuid();
                    UploadItemManager.Save(Data, FileName, FileType, ID, answerIds, FileGuid);
                }
            }
        }

        /// <summary>
        /// Returns a list containing all the answer ids for an item.
        /// </summary>
        /// <returns>The list of all answer ids. If no answers are found an empty List is returned.</returns>
        public virtual List<long> GetAllAnswerIds()
        {
            return AnswerData.GetAllAnswerIds(ID);
        }

        /// <summary>
        /// Get the validator for this item
        /// </summary>
        /// <returns></returns>
        protected bool ValidateFileType()
        {
            var validator = new UploadItemValidator();

            if (!validator.Validate(this))
            {
                ValidationErrors.Add(validator.GetMessage(LanguageCode));
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool ValidateAnswers()
        {
            //Check required
            if (Required && !HasAnswer)
            {
                ValidationErrors.Add(TextManager.GetText("/validationMessages/regex/required", LanguageCode));
                return false;
            }

            //Validate file type
            return ValidateFileType();
        }

        #endregion

        /// <summary>
        /// Update survey response from data transfer object
        /// </summary>
        /// <param name="dto"></param>
        public override void UpdateFromDataTransferObject(IItemProxyObject dto)
        {
            base.UpdateFromDataTransferObject(dto);

            if (dto.AdditionalData is UploadItemAdditionalData)
            {
                UploadFile(
                    dto.InstanceData["FileName"],
                    ((UploadItemAdditionalData) dto.AdditionalData).FileBytes,
                    dto.InstanceData["FileType"]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override NameValueCollection GetMetaDataValuesForSerialization()
        {
            var values = base.GetMetaDataValuesForSerialization();

            values["AllowedFileTypesCSV"] = AllowedFileTypesCSV;

            return values;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override NameValueCollection GetInstanceDataValuesForSerialization()
        {
            var values = base.GetInstanceDataValuesForSerialization();

            values["FileName"] = FileName;
            values["FileSize"] = FileSize.ToString();
            values["FileSizeDisplay"] = FileSizeDisplay;
            values["FileType"] = FileType;
            values["FileGuid"] = FileGuid.ToString();

            return values;
        }

        /// <summary>
        /// Write instance data to XML
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="isText"></param>
        protected override void WriteAnswers(XmlWriter writer, bool isText)
        {
            if (HasAnswer)
            {
                writer.WriteStartElement("answer");
                string rawAnswer = string.Format(
                    isText ? "{0} : {1}" : "{0}<br/><a class=\"uploadedFileLink\" href=\"{1}\">{2}</a>",
                    Utilities.RemoveScript(GetRawAnswer()), GetDownloadUrl(),
                    TextManager.GetText("/pageText/viewResponseDetails.aspx/downloadFile"));

                writer.WriteValue(rawAnswer);
                writer.WriteEndElement();                    
            }
        }

        private string GetDownloadUrl()
        {
            if (AnswerData != null)
            {
                var answers = AnswerData.GetAllAnswerIds(ID);

                long answerId;
                if (answers.Any() && (answerId = answers.First()) > -1)
                {
                    var uploadItem = UploadItemManager.GetFileByAnswerID(answerId);
                    return ApplicationManager.ApplicationPath + "/FileDownloader.aspx?guid=" + uploadItem.FileGuid;
                }
            }

            return string.Empty;
        }
    }
}