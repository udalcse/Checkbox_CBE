using System;
using System.IO;
using Checkbox.Common;
using Checkbox.Progress;
using Prezza.Framework.ExceptionHandling;

namespace Checkbox.Invitations.Export
{
    ///<summary>
    ///</summary>
    public static class ExportManager
    {
        ///<summary>
        ///</summary>
        ///<returns></returns>
        public static InvitationDataExporter GetExporter()
        {
            return new CsvDataExporter();
        }

        /// <summary>
        /// Export results to a CSV file
        /// </summary>
        /// <param name="invitationId"></param>
        /// <param name="languageCode"></param>
        /// <param name="progressKey"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static void WriteCommonExportToFile(int invitationId,
                                                string languageCode,
                                                string progressKey,
                                                string filePath)
        {
            try
            {
                //Create an exporter
                InvitationDataExporter exporter = GetExporter();

                //Initialize
                exporter.Initialize(invitationId, languageCode, progressKey);

                //Write
                exporter.WriteToFile(filePath);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessPublic");

                //Set initial progress
                if (Utilities.IsNotNullOrEmpty(progressKey))
                {
                    ProgressProvider.SetProgress(
                        progressKey,
                        new ProgressData
                        {
                            CurrentItem = 0,
                            Status = ProgressStatus.Error,
                            Message = ex.Message
                        }
                    );
                }
            }
        }

        /// <summary>
        /// Use the passed-in exporter to write data to a file.  This facilitates reusing an exporter, such as
        /// when splitting files, to prevent multiple loadings of answers, etc.
        /// </summary>
        /// <param name="exporter">Exporter to use.</param>
        /// <param name="outputFilePath"></param>
        /// <param name="progressKey"></param>
        public static void WriteExportToFile(InvitationDataExporter exporter, string outputFilePath, string progressKey)
        {
            try
            {
                exporter.WriteToFile(outputFilePath);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessPublic");

                //Set initial progress
                if (Utilities.IsNotNullOrEmpty(progressKey))
                {
                    ProgressProvider.SetProgress(
                        progressKey,
                        new ProgressData
                        {
                            CurrentItem = 0,
                            Status = ProgressStatus.Error,
                            Message = ex.Message
                        }
                    );
                }
            }
        }

        /// <summary>
        /// Use the passed-in exporter to write data to a text writer.  This facilitates reusing an exporter, such as
        /// when splitting files, to prevent multiple loadings of answers, etc.
        /// </summary>
        /// <param name="exporter">Exporter to use.</param>
        /// <param name="tw"></param>
        /// <param name="progressKey"></param>
        public static void WriteExportToTextWriter(InvitationDataExporter exporter, TextWriter tw, string progressKey)
        {
            try
            {
                exporter.WriteToTextWriter(tw);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessPublic");

                //Set initial progress
                if (Utilities.IsNotNullOrEmpty(progressKey))
                {
                    ProgressProvider.SetProgress(
                        progressKey,
                        new ProgressData
                        {
                            CurrentItem = 0,
                            Status = ProgressStatus.Error,
                            Message = ex.Message
                        }
                    );
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tw"></param>
        /// <param name="invitationId"></param>
        /// <param name="languageCode"></param>
        /// <param name="progressKey"></param>
        public static void WriteExportToTextWriter(TextWriter tw,
                                                      int invitationId,
                                                      string languageCode,
                                                      string progressKey)
        {
            try
            {
                //Create an exporter
                InvitationDataExporter exporter = GetExporter();

                //Initialize
                exporter.Initialize(invitationId, languageCode, progressKey);

                //Write
                exporter.WriteToTextWriter(tw);

            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessPublic");

                //Set initial progress
                if (Utilities.IsNotNullOrEmpty(progressKey))
                {
                    ProgressProvider.SetProgress(
                        progressKey,
                        new ProgressData
                        {
                            CurrentItem = 0,
                            Status = ProgressStatus.Error,
                            Message = ex.Message
                        }
                    );
                }
            }
        }
    }
}
