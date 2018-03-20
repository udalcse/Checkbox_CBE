using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Checkbox.Common;
using Checkbox.Forms.Items;
using Checkbox.Globalization.Text;
using Checkbox.Management;
using Checkbox.Progress;
using Ionic.Zip;
using Prezza.Framework.Data;
using Prezza.Framework.ExceptionHandling;
using Affirma.ThreeSharp;
using Affirma.ThreeSharp.Query;
using Affirma.ThreeSharp.Model;

namespace Checkbox.Forms
{
    /// <summary>
    /// Enumeration of "type" of uploaded files.
    /// </summary>
    public enum UploadedFileType
    {
        /// <summary>
        /// Unable to determine
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Represents a readable document
        /// </summary>
        Document,

        /// <summary>
        /// Viewable image
        /// </summary>
        Image,

        /// <summary>
        /// Audio data
        /// </summary>
        Audio,

        /// <summary>
        /// Video data
        /// </summary>
        Video
    }

    /// <summary>
    /// A management class designed to handle persistence and retrieval of upload files
    /// </summary>
    public static class UploadItemManager
    {
        /// <summary>
        /// Returns the master list of all allowed file types which can be assigned to an UploadItem.
        /// </summary>
        /// <returns></returns>
        public static List<string> AllAllowedFileTypes
        {
            get
            {
                var allowedFileTypes = new List<string>();
                Database db = DatabaseFactory.CreateDatabase();
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_FileUpload_GetAllowedFileTypes");

                using (IDataReader reader = db.ExecuteReader(command))
                {
                    try
                    {
                        while (reader.Read())
                        {
                            allowedFileTypes.Add((string)reader["TypeExtension"]);
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionPolicy.HandleException(ex, "BusinessPublic");
                    }
                    finally
                    {
                        reader.Close();
                    }
                }

                return allowedFileTypes;
            }
        }

        /// <summary>
        /// Remove invalid characters from a file name
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="invalidCharReplacement"></param>
        /// <returns></returns>
        public static string SanitizeFileName(string fileName, string invalidCharReplacement)
        {
            return FileUtilities.SanitizeFileName(fileName, invalidCharReplacement);
        }

        /// <summary>
        /// Constructs a file name that is guaranteed to be unique. Uniqueness is ensured by adding 
        /// the response id and answer id to the file name. The name is formatted as:
        /// ResponseID_AnswerID_FileName
        /// </summary>
        /// <param name="answerId"></param>
        /// <param name="uploadItem"></param>
        /// <returns></returns>
        private static string GetUploadedFileExportName(UploadItem uploadItem, long answerId)
        {
            var responseId = uploadItem.ResponseId;
            var fileName = uploadItem.FileName;

            var formattedName = new StringBuilder();

            if (responseId > 0)
                formattedName.AppendFormat("{0}_", responseId);

            if (answerId > 0)
                formattedName.AppendFormat("{0}_", answerId);

            if (fileName != string.Empty)
                formattedName.Append(fileName);

            return formattedName.ToString();
        }


        /// <summary>
        /// Determine the number of uploaded files associated with a specific response template.
        /// </summary>
        /// <param name="responseTemplateId">The response template that the UploadItem question is associated with.</param>
        /// <returns>The number of uploaded files.</returns>
        public static int GetFileCount(int responseTemplateId)
        {
            Database db = DatabaseFactory.CreateDatabase();

            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_FileUpload_GetFileCountByResponseTemplateID");
            command.AddInParameter("ResponseTemplateId", DbType.Int32, responseTemplateId);

            int count = Int32.Parse(db.ExecuteScalar(command).ToString());

            return count;
        }

        /// <summary>
        /// Retrieves all uploaded files associated with a specific response template.
        /// </summary>
        /// <param name="responseTemplateId">The response template that the UploadItem question is associated with.</param>
        public static List<long> GetFileAnswerIdsByResponseTemplateId(int responseTemplateId)
        {
            Database db = DatabaseFactory.CreateDatabase();

            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_FileUpload_GetFileByResponseTemplateID");
            command.AddInParameter("ResponseTemplateId", DbType.Int32, responseTemplateId);

            var answerIds = new List<long>();

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while(reader.Read())
                    {
                        var answerId = DbUtility.GetValueFromDataReader(reader, "AnswerId", (long) -1);

                        if (answerId > 0)
                        {
                            answerIds.Add(answerId);
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return answerIds;
        }

        /// <summary>
        /// Download all of the uploaded files associated with a specific response template and saves them to disk.
        /// </summary>
        /// <param name="responseTemplateId">The response template that the UploadItem question is associated with.</param>
        /// <param name="tempFolderPath">Temp folder to use for writing files.</param>
        public static void SaveFilesToDisk(int responseTemplateId, string tempFolderPath)
        {
            SaveFilesToDisk(
                responseTemplateId,
                FileUtilities.JoinPathAndFileName(tempFolderPath, string.Format("UploadedSurveyFiles_{0}", responseTemplateId)),
                null,
                null);
        }

        /// <summary>
        /// Download all of the uploaded files associated with a specific response template and saves them to disk.
        /// </summary>
        /// <param name="responseTemplateId">The response template that the UploadItem question is associated with.</param>
        /// <param name="progressKey">Progress key to use if tracking progresss.  If not tracking progress, pass null or empty string.</param>
        /// <param name="tempFolderPath">Temp folder to place files in.</param>
        /// <param name="progressMessageLanguageCode">Language code for localized progress messages.</param>
        public static void SaveFilesToDisk(int responseTemplateId, string tempFolderPath, string progressKey, string progressMessageLanguageCode)
        {
            string progressBaseText = null;

            if (Utilities.IsNotNullOrEmpty(progressKey))
            {
                progressBaseText = TextManager.GetText("/controlText/uploadItemManager/downloadFilesToTempFolder",
                                                       progressMessageLanguageCode);

                ProgressProvider.SetProgress(
                    progressKey,
                    TextManager.GetText("/controlText/uploadItemManager/generatingFileList", progressMessageLanguageCode),
                    string.Empty,
                    ProgressStatus.Running,
                    0,
                    1);
            }

            var itemAnswerIds = GetFileAnswerIdsByResponseTemplateId(responseTemplateId);

            FileUtilities.CreateFolder(tempFolderPath, false);
            int fileCount = itemAnswerIds.Count;

            for (int i = 0; i < fileCount; i++)
            {
                if (Utilities.IsNotNullOrEmpty(progressKey))
                {
                    string progressText = progressBaseText;

                    if (Utilities.IsNotNullOrEmpty(progressBaseText)
                        && progressBaseText.Contains("{0}")
                        && progressBaseText.Contains("{1}"))
                    {
                        progressText = string.Format(progressBaseText, i + 1, fileCount);
                    }

                    //Set progress.  Call download step 80% of total work with zipping (done elsewhere)
                    // being rest.
                    ProgressProvider.SetProgress(
                        progressKey,
                        progressText,
                        string.Empty,
                        ProgressStatus.Running,
                        (int) ((0.8)*i),
                        fileCount);
                }

                //Get file
                var uploadItem = GetFileByAnswerID(itemAnswerIds[i]);

                if (uploadItem == null || uploadItem.Data == null || uploadItem.Data.Length == 0)
                {
                    continue;
                }

                //Save it
                FileUtilities.SaveFile(tempFolderPath, GetUploadedFileExportName(uploadItem, itemAnswerIds[i]), uploadItem.Data);
            }
        }

        /// <summary>
        /// Retrieve the uploaded files associated with a specific response template and saves them to
        /// an zip archive in memory.
        /// </summary>
        /// <param name="responseTemplateId">The response template that the UploadItem question is associated with.</param>
        public static byte[] GetFilesAsArchive(int responseTemplateId)
        {
            var answerIds = GetFileAnswerIdsByResponseTemplateId(responseTemplateId);
            var archive = new ZipFile();

            foreach(var answerId in answerIds)
            {
                UploadItem item = GetFileByAnswerID(answerId);

                if (item != null)
                {
                    var stream = new MemoryStream(item.Data);
                    archive.AddEntry(GetUploadedFileExportName(item, answerId), stream);
                }
            }

            var output = new MemoryStream();
            archive.Save(output);
            return output.ToArray();
        }

        /// <summary>
		/// Returns the UploadItem object which is associated with a given answer.
		/// </summary>
		/// <param name="answerId">The answer which the upload file is associated with.</param>
		/// <returns></returns>
		public static UploadItem GetFileByAnswerID(long answerId)
		{
			Database db = DatabaseFactory.CreateDatabase();
			UploadItem item = null;

			DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_FileUpload_GetFileByAnswerID");
			command.AddInParameter("AnswerID", DbType.Int64, answerId);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        var fileName = DbUtility.GetValueFromDataReader(reader, "FileName", string.Empty);
                        var fileType = DbUtility.GetValueFromDataReader(reader, "FileType", string.Empty);
                        int fileId = DbUtility.GetValueFromDataReader(reader, "FileID", -1);
                        long responseId = DbUtility.GetValueFromDataReader(reader, "ResponseId", (long) -1);
                        Guid fileGuid = DbUtility.GetValueFromDataReader(reader, "FileGuid", Guid.Empty);

                        byte[] data = null;

                        if (UseS3ForUploadedFiles)
                        {
                            data = GetFileFromS3(fileId, fileName);
                        }

                        //Try to handle case where S3 enabled after some files were already uploaded
                        if ((data == null || data.Length == 0)
                            && reader["FileData"] != DBNull.Value)
                        {
                            data = (byte[])reader["FileData"];
                        }

                        item = new UploadItem(data, fileName, fileType, responseId, fileGuid);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

			return item;
		}

        /// <summary>
        /// Returns the UploadItem object which is associated with a given answer.
        /// </summary>
        /// <param name="fileGuid">The answer which the upload file is associated with.</param>
        /// <returns></returns>
        public static UploadItem GetFileByGuid(Guid fileGuid)
        {
            Database db = DatabaseFactory.CreateDatabase();
            UploadItem item = null;

            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_FileUpload_GetFileByGuid");
            command.AddInParameter("FileGuid", DbType.Guid, fileGuid);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        var fileName = DbUtility.GetValueFromDataReader(reader, "FileName", string.Empty);
                        var fileType = DbUtility.GetValueFromDataReader(reader, "FileType", string.Empty);
                        int fileId = DbUtility.GetValueFromDataReader(reader, "FileID", -1);
                        long responseId = DbUtility.GetValueFromDataReader(reader, "ResponseId", -1L);

                        byte[] data = null;

                        if (UseS3ForUploadedFiles)
                        {
                            data = GetFileFromS3(fileId, fileName);
                        }

                        //Try to handle case where S3 enabled after some files were already uploaded
                        if ((data == null || data.Length == 0)
                            && reader["FileData"] != DBNull.Value)
                        {
                            data = (byte[])reader["FileData"];
                        }

                        item = new UploadItem(data, fileName, fileType, responseId, fileGuid);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return item;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        /// <param name="???"></param>
        /// <param name="answerId"> </param>
        public static void DownloadFile(HttpResponse response, long answerId)
        {
            var item = GetFileByAnswerID(answerId);
            WriteToResponse(response, item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        /// <param name="fileGuid"> </param>
        public static void DownloadFile(HttpResponse response, Guid fileGuid)
        {
            var item = GetFileByGuid(fileGuid);
            WriteToResponse(response, item);
        }

        private static void WriteToResponse(HttpResponse response, UploadItem item)
        {
            if (item != null && item.FileName != null && item.Data != null)
            {
                response.Expires = -1;
                response.BufferOutput = true;
                response.Clear();
                response.ClearHeaders();
                response.AddHeader("Content-Disposition",
                                   "attachment;filename=" + "\"" + HttpUtility.HtmlEncode(item.FileName) + "\"");
                response.ContentType = "application/octet-stream";
                response.ContentEncoding = Encoding.UTF8;

                response.BinaryWrite(item.Data);
                response.Flush();
                response.End();
            }
        }

        /// <summary>
        /// Verify that S3 settings enabled and have values
        /// </summary>
        public static bool UseS3ForUploadedFiles
        {
            get
            {
                if (!ApplicationManager.AppSettings.UseS3ForUploadedFiles)
                {
                    return false;
                }

                if (Utilities.IsNullOrEmpty(ApplicationManager.AppSettings.S3AccessKeyID))
                {
                    ExceptionPolicy.HandleException(
                        new Exception("S3 upload file storage is enabled, but S3AccessKeyID is not specified."),
                        "BusinessPublic");

                    return false;
                }

                if (Utilities.IsNullOrEmpty(ApplicationManager.AppSettings.S3SecretAccessKey))
                {
                    ExceptionPolicy.HandleException(
                        new Exception("S3 upload file storage is enabled, but S3SecretAccessKey is not specified."),
                        "BusinessPublic");

                    return false;
                }

                if (Utilities.IsNullOrEmpty(ApplicationManager.AppSettings.S3BucketName))
                {
                    ExceptionPolicy.HandleException(
                        new Exception("S3 upload file storage is enabled, but S3BucketName is not specified."),
                        "BusinessPublic");

                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Verify that S3 settings enabled and have values
        /// </summary>
        public static bool UseS3ForTempFiles
        {
            get
            {
                if (!ApplicationManager.AppSettings.UseS3ForTempFiles)
                {
                    return false;
                }

                if (Utilities.IsNullOrEmpty(ApplicationManager.AppSettings.S3AccessKeyID))
                {
                    ExceptionPolicy.HandleException(
                        new Exception("S3 upload file storage is enabled, but S3AccessKeyID is not specified."),
                        "BusinessPublic");

                    return false;
                }

                if (Utilities.IsNullOrEmpty(ApplicationManager.AppSettings.S3SecretAccessKey))
                {
                    ExceptionPolicy.HandleException(
                        new Exception("S3 upload file storage is enabled, but S3SecretAccessKey is not specified."),
                        "BusinessPublic");

                    return false;
                }

                if (Utilities.IsNullOrEmpty(ApplicationManager.AppSettings.S3TempBucketName))
                {
                    ExceptionPolicy.HandleException(
                        new Exception("S3 upload file storage is enabled, but S3TempBucketName is not specified."),
                        "BusinessPublic");

                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Get a temp file from S3
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static byte[] GetTempFileFromS3(int fileId, string fileName)
        {
            return GetFileFromS3(fileId, fileName, ApplicationManager.AppSettings.S3TempBucketName);
        }

        /// <summary>
        /// Get persistent file from S3
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static byte[] GetFileFromS3(int fileId, string fileName)
        {
            return GetFileFromS3(fileId, fileName, ApplicationManager.AppSettings.S3BucketName);
        }

        ///<summary>
        ///</summary>
        ///<param name="fileId"></param>
        ///<param name="fileName"></param>
        ///<returns></returns>
        private static byte[] GetFileFromS3(int fileId, string fileName, string bucketName)
        {
            try
            {
                if (fileId <= 0)
                {
                    return new byte[] { };
                }

                var config = new ThreeSharpConfig
                {
                    AwsAccessKeyID = ApplicationManager.AppSettings.S3AccessKeyID,
                    AwsSecretAccessKey = ApplicationManager.AppSettings.S3SecretAccessKey,
                    IsSecure = false
                };

                byte[] fileBytes;
                IThreeSharp service = new ThreeSharpQuery(config);
                using (var request = new ObjectGetRequest(bucketName, GetS3FileName(fileId, fileName)))
                {
                    using (ObjectGetResponse response = service.ObjectGet(request))
                    {
                        fileBytes = response.StreamResponseToBytes();
                    }
                }

                return fileBytes;
            }
            catch (Exception ex)
            {
                return new byte[] { };
            }
        }

        /// <summary>
        /// Look for file with name on bucket and get download link
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetS3TempFileDownloadLink(string fileName)
        {
            return GetS3FileDownloadLink(fileName, ApplicationManager.AppSettings.S3TempBucketName);
        }

        /// <summary>
        /// Look for file with name on bucket and get download link
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetS3FileDownloadLink(string fileName)
        {
            return GetS3FileDownloadLink(fileName, ApplicationManager.AppSettings.S3BucketName);
        }

        /// <summary>
        /// Look for file with name on bucket and get download link
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        private static string GetS3FileDownloadLink(string fileName, string bucketName)
        {
            //Upload the file to S3

            IThreeSharp service = new ThreeSharpQuery(
                new ThreeSharpConfig
                {
                    AwsAccessKeyID = ApplicationManager.AppSettings.S3AccessKeyID,
                    AwsSecretAccessKey = ApplicationManager.AppSettings.S3SecretAccessKey,
                    IsSecure = false
                });

            //Get signed link for download
            using (var getRequest = new UrlGetRequest(bucketName, fileName))
            {
                //Link expires in 30 minutes
                getRequest.ExpiresIn = 60 * 30 * 1000;

                using (UrlGetResponse urlGetResponse = service.UrlGet(getRequest))
                {
                    return urlGetResponse.StreamResponseToString();
                }
            }
        }


        /// <summary>
        /// Persists item data to the database.
        /// </summary>
        /// <param name="data">The file data.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="fileType">The file type.</param>
        /// <param name="fileGuid"> </param>
        /*public static int Save(byte[] data, string fileName, string fileType, out Guid fileGuid)
        {
            if (data == null) throw new ArgumentNullException("data");
            if (fileName == null) throw new ArgumentNullException("fileName");
            if (fileType == null) throw new ArgumentNullException("fileType");

            Database db = DatabaseFactory.CreateDatabase();
            using (IDbConnection connection = db.GetConnection())
            {
                connection.Open();
                IDbTransaction transaction = connection.BeginTransaction();

                int fileId;
                try
                {
                    // TODO: implement S3 file upload for 'UseS3ForUploadedFiles' mode
                    fileId = SaveToDatabase(transaction, data, fileName, fileType, out fileGuid);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    connection.Close();
                }

                return fileId;
            }
        }*/


        /// <summary>
        /// Persists item data to the database.
        /// </summary>
        /// <param name="data">The file data.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="fileType">The file type.</param>
        /// <param name="itemId">The id of the item to associate with.</param>
        /// <param name="fileGuid"> </param>
       /* public static int Save(byte[] data, string fileName, string fileType, int itemId, out Guid fileGuid)
		{
			if (data == null) throw new ArgumentNullException("data");
			if (fileName == null) throw new ArgumentNullException("fileName");
			if (fileType == null) throw new ArgumentNullException("fileType");

            Database db = DatabaseFactory.CreateDatabase();
			using (IDbConnection connection = db.GetConnection())
			{
				connection.Open();
				IDbTransaction transaction = connection.BeginTransaction();

			    int fileId;
			    try
				{
					// TODO: implement S3 file upload for 'UseS3ForUploadedFiles' mode
                    fileId = SaveToDatabase(transaction, data, fileName, fileType, out fileGuid);
					SaveItemDataRelationship(transaction, itemId, fileId, 0);

					transaction.Commit();
				}
				catch (Exception)
				{
					transaction.Rollback();
					throw;
				}
				finally
				{
					connection.Close();
				}

				return fileId;
			}
		}*/

        /// <summary>
        /// Persists item data to the database.
        /// </summary>
        /// <param name="data">The file data.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="fileType">The file type.</param>
        /// <param name="itemId">The id of the question.</param>
        /// <param name="answerIds">The answer which the upload file is associated with.</param>
        /// <param name="fileGuid"> </param>
        public static void Save(byte[] data, string fileName, string fileType, int itemId, List<long> answerIds, Guid fileGuid)
        {
            if (data == null) throw new ArgumentNullException("data");
            if (fileName == null) throw new ArgumentNullException("fileName");
            if (fileType == null) throw new ArgumentNullException("fileType");
            if (answerIds == null) throw new ArgumentNullException("answerIds");

            Database db = DatabaseFactory.CreateDatabase();
            using (IDbConnection connection = db.GetConnection())
            {
                connection.Open();
                IDbTransaction transaction = connection.BeginTransaction();

                try
                {
                    int fileId = SaveToDatabase(transaction, data, fileName, fileType, fileGuid);

                    foreach (long answerId in answerIds)
                    {
                        SaveItemDataRelationship(transaction, itemId, fileId, answerId);
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Persist an uploaded file to the database server.
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="data">The file data.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="fileType">The file type.</param>
        /// <param name="fileGuid"> </param>
        private static int SaveToDatabase(IDbTransaction transaction, byte[] data, string fileName, string fileType, Guid fileGuid)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (fileName == null) throw new ArgumentNullException("fileName");
            if (data == null) throw new ArgumentNullException("data");
            if (fileType == null) throw new ArgumentNullException("fileType");

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_FileUpload_InsertFile");

            if (UseS3ForUploadedFiles)
            {
                fileName = SanitizeFileName(fileName, ".");
                command.AddInParameter("FileData", DbType.Binary, DBNull.Value);
            }
            else
            {
                command.AddInParameter("FileData", DbType.Binary, data);
            }
            command.AddInParameter("FileName", DbType.String, fileName);
            command.AddInParameter("FileType", DbType.String, fileType);
            command.AddInParameter("FileSize", DbType.Int32, data.Length);
            command.AddInParameter("FileGuid", DbType.Guid, fileGuid);

            command.AddOutParameter("FileID", DbType.Int32, 4);

            db.ExecuteNonQuery(command, transaction);

            object id = command.GetParameterValue("FileID");

            if (id == null || id == DBNull.Value)
            {
                throw new Exception("An error occurred while creating entry for file data.  ID returned was null.");
            }

            if (UseS3ForUploadedFiles)
            {
                SaveFileToS3(GetS3FileName((int)id, fileName), data);
            }
            return (int)id;
        }

        /// <summary>
        /// Save file data to S3 temp file store.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileBytes"></param>
        public static void SaveTempFileToS3(string fileName, byte[] fileBytes)
        {
            SaveFileToS3(fileName, fileBytes, ApplicationManager.AppSettings.S3TempBucketName);
        }

        /// <summary>
        /// Save a persistent file to S3
        /// </summary>
        /// <param name="fileName">File name should be unique for store and for multi-hosting situations.</param>
        /// <param name="fileBytes">Data to save</param>
        private static void SaveFileToS3(string fileName, byte[] fileBytes)
        {
            SaveFileToS3(fileName, fileBytes, ApplicationManager.AppSettings.S3BucketName);
        }

        /// <summary>
        /// Save a file to S3
        /// </summary>
        /// <param name="fileName">File name should be unique for store and for multi-hosting situations.</param>
        /// <param name="fileBytes">Data to save</param>
        /// <param name="bucketName"></param>
        private static void SaveFileToS3(string fileName, byte[] fileBytes, string bucketName)
        {
            //Upload the file to S3
            var config = new ThreeSharpConfig
            {
                AwsAccessKeyID = ApplicationManager.AppSettings.S3AccessKeyID,
                AwsSecretAccessKey = ApplicationManager.AppSettings.S3SecretAccessKey,
                IsSecure = false
            };

            IThreeSharp service = new ThreeSharpQuery(config);
            using (var request = new ObjectAddRequest(bucketName, fileName))
            {
                UploadedFileType itemType;
                request.LoadStreamWithBytes(fileBytes, DetermineContentType(fileName, out itemType));
                service.ObjectAdd(request);
            }
        }

        /// <summary>
        /// Save a file to S3
        /// </summary>
        /// <param name="fileName">File name should be unique for store and for multi-hosting situations.</param>
        /// <param name="filePath">Path of file to save</param>
        public static string SaveTempFileToS3(string fileName, string filePath)
        {
            byte[] fileBytes = null;

            //Load the file
            using (FileStream fs = File.OpenRead(filePath))
            {
                try
                {
                    fileBytes = new byte[fs.Length];
                    fs.Read(fileBytes, 0, fileBytes.Length);
                }
                finally
                {
                    fs.Close();
                }
            }

            //Save file to S3
            SaveTempFileToS3(fileName, fileBytes);

            return GetS3TempFileDownloadLink(fileName);
        }

        /// <summary>
        /// Save a file to S3
        /// </summary>
        /// <param name="fileName">File name should be unique for store and for multi-hosting situations.</param>
        /// <param name="filePath">Path of file to save</param>
        public static string SaveFileToS3(string fileName, string filePath)
        {
            byte[] fileBytes = null;

            //Load the file
            using (FileStream fs = File.OpenRead(filePath))
            {
                try
                {
                    fileBytes = new byte[fs.Length];
                    fs.Read(fileBytes, 0, fileBytes.Length);
                }
                finally
                {
                    fs.Close();
                }
            }

            //Save file to S3
            SaveFileToS3(fileName, fileBytes);

            return GetS3FileDownloadLink(fileName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public static string DetermineContentType(string fileName, out UploadedFileType fileType)
        {
            fileType = UploadedFileType.Unknown;

            if (string.IsNullOrEmpty(fileName))
            {
                return string.Empty;
            }
            string extension = Path.GetExtension(fileName);

            if (string.IsNullOrEmpty(extension))
            {
                return string.Empty;
            }

            switch (extension.ToLowerInvariant())
            {
                case ".gif":
                    fileType = UploadedFileType.Image;
                    return "image/gif";

                case ".png":
                    fileType = UploadedFileType.Image;
                    return "image/x-png";

                case ".jpeg":
                    fileType = UploadedFileType.Image;
                    return "image/jpeg";

                case ".jpg":
                    fileType = UploadedFileType.Image;
                    return "image/jpeg";

                case ".tif":
                case ".tiff":
                    fileType = UploadedFileType.Image;
                    return "image/tiff";

                case ".pdf":
                    fileType = UploadedFileType.Document;
                    return "application/pdf";

                case ".doc":
                case ".docx":
                    fileType = UploadedFileType.Document;
                    return "application/msword";

                case ".xls":
                case ".xlsx":
                case ".csv":
                    fileType = UploadedFileType.Document;
                    return "application/ms-excel";

                case ".mp3":
                    fileType = UploadedFileType.Audio;
                    return "audio/mpeg";
                
                case ".wav":
                    fileType = UploadedFileType.Audio;
                    return "audio/vnd.wave";

                case ".wma":
                    fileType = UploadedFileType.Audio;
                    return "audio/x-ms-wma";

                case ".mp4":
                    fileType = UploadedFileType.Video;
                    return "video/mp4";
    
                case ".mpg":
                case ".mpeg":
                    fileType = UploadedFileType.Video;
                    return "video/mpeg";

                case ".wmv":
                    fileType = UploadedFileType.Video;
                    return "video/x-ms-wmv";

                case ".swf":
                    fileType = UploadedFileType.Video;
                    return "application/x-shockwave-flash";

                case ".flv":
                    fileType = UploadedFileType.Video;
                    return "video/x-flv";


                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Get name of file name when stored in S3 Bucket
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetS3FileName(int fileId, string fileName)
        {
            return string.Format("{0}.{1}.{2}", ApplicationManager.ApplicationDataContext, fileId, fileName);
        }

        /// <summary>
        /// Persist which question and answer an uploaded file is associated with.
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="itemId">The id of the question.</param>
        /// <param name="fileId">The id of the uploaded file.</param> 
        /// <param name="answerId">The answer which the upload file is associated with.</param>
        private static void SaveItemDataRelationship(IDbTransaction transaction, int itemId, int fileId, long answerId)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpsertFileUploadFiles");

            command.AddInParameter("ItemID", DbType.Int32, itemId);
            command.AddInParameter("FileID", DbType.Int32, fileId);
            command.AddInParameter("AnswerID", DbType.Int64, answerId);

            db.ExecuteNonQuery(command, transaction);
        }

        /// <summary>
        /// Updates the list of allowed file types.
        /// If a file type is removed that has already been assigned to a question that question will
        /// still allow the removed file.
        /// </summary>
        /// <param name="fileTypes">The list of allowed file types.</param>
        public static void UpdateAllowedFileTypes(IList<string> fileTypes)
        {
            if (fileTypes == null) throw new ArgumentNullException("fileTypes");

            List<string> allowedFileTypes = AllAllowedFileTypes;

            var removed = allowedFileTypes.Where(type => !fileTypes.Contains(type)).ToList();
            var added = fileTypes.Where(type => !allowedFileTypes.Contains(type)).ToList();

            Database db = DatabaseFactory.CreateDatabase();
            using (IDbConnection connection = db.GetConnection())
            {
                connection.Open();
                IDbTransaction transaction = connection.BeginTransaction();

                try
                {
                    foreach (string type in removed)
                    {
                        DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_FileUpload_RemoveAllowedFileType");
                        command.AddInParameter("TypeExtension", DbType.String, type);
                        db.ExecuteNonQuery(command, transaction);
                    }

                    foreach (string type in added)
                    {
                        DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_FileUpload_AddAllowedFileType");
                        command.AddInParameter("TypeExtension", DbType.String, type);
                        command.AddInParameter("TypeDescription", DbType.String, string.Empty);
                        db.ExecuteNonQuery(command, transaction);
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Ensures that the specified directory can be written to.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static bool ValidateDownloadDirectory(string directory)
        {
            if (Utilities.IsNullOrEmpty(directory)) return false;

            byte[] testData = { 0, 1, 2 };
            string testFileName = directory.EndsWith("\\") || directory.EndsWith("/")
                ? string.Format("{0}_AccessValidation.txt", DateTime.Now.Ticks)
                : string.Format("/{0}_AccessValidation.txt", DateTime.Now.Ticks);

            string fullPath = FileUtilities.JoinPathAndFileName(directory, testFileName);

            try
            {
                FileUtilities.SaveFile(directory, testFileName, testData);
                FileUtilities.DeleteFile(fullPath);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
