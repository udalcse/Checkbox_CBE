using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Checkbox.Common;
using Checkbox.Globalization.Text;
using Checkbox.Management;
using Checkbox.Progress;

namespace Checkbox.Invitations.Export
{
    /// <summary>
    /// Survey data exporter that writes results data to a CSV file.
    /// </summary>
    public class CsvDataExporter : InvitationDataExporter
    {
        /// <summary>
        /// Get/set field index to start with when writing a row of data
        /// </summary>
        protected int StartFieldIndex { get; set; }

        /// <summary>
        /// Get/set field index to end with when writing a row of data.
        /// </summary>
        protected int EndFieldIndex { get; set; }

        /// <summary>
        /// Get/set newline replacement character
        /// </summary>
        protected string NewLineReplacement { get; set; }

        /// <summary>
        /// Get/set whether to replace newlines
        /// </summary>
        protected bool ReplaceNewLines { get; set; }

        /// <summary>
        /// Get/set chunk size (# line between buffer flushes) for export.
        /// </summary>
        protected int ChunkSize { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public CsvDataExporter()
        {
            ReplaceNewLines = ApplicationManager.AppSettings.ReplaceNewLine;
            NewLineReplacement = ApplicationManager.AppSettings.NewLineReplacement;
            ChunkSize = ApplicationManager.AppSettings.ResponseDataExportChunkSize;
        }

        /// <summary>
        /// Populate field names.
        /// </summary>
        /// <remarks>This override figures out which columns to include when splitting the
        /// export across files.</remarks>
        protected override void PopulateFieldNames(bool forceQuestionReload)
        {
            base.PopulateFieldNames(forceQuestionReload);

            CalculateStartAndEndIndices();
        }

        /// <summary>
        /// Calculate indices of first and last fields for writing data.
        /// </summary>
        protected virtual void CalculateStartAndEndIndices()
        {
            //Set initial values
            StartFieldIndex = 0;
            EndFieldIndex = AllFieldNames.Count - 1;
        }

        /// <summary>
        /// Write exported data to the specified text writer
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteExportData(TextWriter writer)
        {
            //Write headers
            WriteHeaders(writer);

            //Write newline
            writer.Write(Environment.NewLine);

            //Write responses
            WriteInvitationData(writer);
        }

        /// <summary>
        /// Write headers to the writer
        /// </summary>
        /// <param name="writer"></param>
        protected void WriteHeaders(TextWriter writer)
        {
            //Set initial progress
            if (Utilities.IsNotNullOrEmpty(ProgressKey))
            {
                //Start progress at 70, since we'll consider 70% of the work to be loading survey & items,
                // and answer data from database by analysis
                ProgressProvider.SetProgress(
                    ProgressKey,
                    new ProgressData
                    {
                        CurrentItem = 70,
                        Status = ProgressStatus.Pending,
                        Message = TextManager.GetText("/controlText/exportManager/writingHeaders", LanguageCode)
                    }
                );
            }

            for (int fieldIndex = StartFieldIndex; fieldIndex <= EndFieldIndex; fieldIndex++)
            {
                //Strip HTML from headers
                WriteValue(
                    Utilities.StripHtml(AllFieldNames[fieldIndex], null), 
                    writer);

                if (fieldIndex < EndFieldIndex)
                {
                    writer.Write(',');
                }
            }
        }

        /// <summary>
        /// Write responses
        /// </summary>
        /// <param name="writer"></param>
        protected void WriteInvitationData(TextWriter writer)
        {
            //Create a list to store all columns.  We will then selectively
            // write based on start/end index
            var allColumnData = new List<string>();

            //List recipients ids to export
            var recipients = Invitation.GetRecipients(RecipientFilter.All);
            int numRows = recipients.Count;

            var trackingText = TextManager.GetText("/controlText/exportManager/exportingInvitationDetails", LanguageCode);
            
            for (int rowNumber = 0; rowNumber < numRows; rowNumber++)
            {
                var recipient = recipients[rowNumber];

                allColumnData.AddRange(GetInvitationData(recipient));
                
                // write data to 
                for (int i = StartFieldIndex; i <= EndFieldIndex; i++)
                {
                    WriteValue(allColumnData[i], writer);
                    if (i < EndFieldIndex)
                    {
                        writer.Write(',');
                    }
                }

                writer.Write(Environment.NewLine);
                allColumnData.Clear();

                // Let the Response buffer fill up before flushing it to the client
                if (rowNumber % ChunkSize == 0)
                {
                    writer.Flush();
                }

                 //Update status -- Only update every 25 row
                if (Utilities.IsNotNullOrEmpty(ProgressKey) && rowNumber % 25 == 0)
                {
                    ProgressProvider.SetProgressCounter(
                        ProgressKey,
                        trackingText,
                        rowNumber,
                        numRows,
                        30,
                        100);
                }
            }

            writer.Flush();
        }

        /// <summary>
        /// Get a list of response property values converted to string format.
        /// </summary>
        /// <returns>List of property values.</returns>
        protected List<string> GetInvitationData(Recipient recipient)
        {
            return ListAllInvitationFieldNames().Select(t => FormatInvitationData(recipient, t)).ToList();
        }

        /// <summary>
        /// Write a value to the writer with property quote escaping.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="tw"></param>
        protected void WriteValue(string value, TextWriter tw)
        {
            value = value.Replace("\"", "\"\"");

            if (ReplaceNewLines)
            {
                value = value.Replace(Environment.NewLine, NewLineReplacement);
            }

            if (value.Contains('"') || value.Contains(','))
            value = string.Format("\"{0}\"", value);

            tw.Write(value);
        }
    }
}
