using System;
using System.IO;
using Checkbox.Common;
using Checkbox.Progress;
using Prezza.Framework.ExceptionHandling;

namespace Checkbox.Analytics.Export
{
    ///<summary>
    ///</summary>
    public static class ExportManager
    {

        public static SurveyDataExporter GetExporter(string exportMode)
        {
            //Create an exporter
            SurveyDataExporter exporter = null;

            if ("CSV".Equals(exportMode, StringComparison.InvariantCultureIgnoreCase))
                exporter = new CsvDataExporter();

            if ("XML".Equals(exportMode, StringComparison.InvariantCultureIgnoreCase))
                exporter = new XmlDataExporter();

            if (exporter == null)
                exporter = new SpssCompatibleCsvDataExporter();

            return exporter;
        }

        /// <summary>
        /// Create a native SPSS export for responses to the provided survey.
        /// </summary>
        /// <param name="responseTemplateId">Survey to export response data for.</param>
        /// <param name="exportOptions">Options for exporting</param>
        /// <param name="languageCode">Language code for exported data.</param>
        /// <param name="progressKey">Progress tracking key.</param>
        /// <param name="outputFileName">Name of output file to write.</param>
        public static void WriteNativeSpssExportToFile(int responseTemplateId,
                                                       ExportOptions exportOptions,
                                                       string languageCode,
                                                       string progressKey,
                                                       string outputFileName)
        {
            try
            {
                NativeSpssDataExporter exporter = new NativeSpssDataExporter();
                exporter.Initialize(responseTemplateId, languageCode, exportOptions, progressKey);

                exporter.WriteToFile(outputFileName);
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
                            ErrorMessage = ex.Message
                        }
                    );
                }

            }
        }
        

        /// <summary>
        /// Export results to a CSV file
        /// </summary>
        /// <param name="responseTemplateId"></param>
        /// <param name="options"></param>
        /// <param name="languageCode"></param>
        /// <param name="progressKey"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static void WriteCommonExportToFile(int responseTemplateId,
                                                ExportOptions options,
                                                string languageCode,
                                                string progressKey,
                                                string filePath)
        {
            try
            {
                //Create an exporter
                SurveyDataExporter exporter = GetExporter(options.ExportMode);

                //Initialize
                exporter.Initialize(responseTemplateId, languageCode, options, progressKey);

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
                            ErrorMessage = ex.Message
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
        public static void WriteExportToFile(SurveyDataExporter exporter, string outputFilePath, string progressKey)
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
                            ErrorMessage = ex.Message
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
        public static void WriteExportToTextWriter(SurveyDataExporter exporter, TextWriter tw, string progressKey)
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
                            ErrorMessage = ex.Message
                        }
                    );
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tw"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="options"></param>
        /// <param name="languageCode"></param>
        /// <param name="progressKey"></param>
        public static void WriteCommonExportToTextWriter(TextWriter tw,
                                                      int responseTemplateId,
                                                      ExportOptions options,
                                                      string languageCode,
                                                      string progressKey)
        {
            try
            {
                //Create an exporter
				SurveyDataExporter exporter = GetExporter(options.ExportMode);

                //Initialize
                exporter.Initialize(responseTemplateId, languageCode, options, progressKey);

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
                            ErrorMessage = ex.Message
                        }
                    );
                }
            }
        }
    }
}
