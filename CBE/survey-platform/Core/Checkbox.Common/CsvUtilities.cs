
using System;
using System.IO;
using System.Collections.Generic;
using LumenWorks.Framework.IO.Csv;

namespace Checkbox.Common
{
    /// <summary>
    /// CSV-Related utilities.
    /// </summary>
    public class CsvUtilities
    {
        private CsvReader _reader;
        private List<string> _parserErrors;
        private List<string> _currentRecord;
        private string _currentRawData;

        /// <summary>
        /// Get parser errors
        /// </summary>
        private List<string> ParserErrors
        {
            get
            {
                if(_parserErrors == null)
                {
                    _parserErrors =new List<string>();
                }

                return _parserErrors;
            }
        }

        /// <summary>
        /// Ensure reader has been intialized and throw an exception if it has not been.
        /// </summary>
        private void EnsureInitialized()
        {
            if(_reader == null)
            {
                throw new Exception("CSV Reader Not Initialized.");
            }
        }

        /// <summary>
        /// Initialize the Csv Reader class with a text reader input.
        /// </summary>
        /// <param name="inReader">Text reader to read data from.f</param>
        /// <param name="inputHasHeaders">Specify whether speicfied input contains a header row.</param>
        /// <param name="fieldDelimiter">CSV field delimiter character.</param>
        /// <param name="quoteChar">CSV field quote character.</param>
        /// <param name="escapeChar">CSV escape character (can be same as quote character)</param>
        /// <param name="commentDelimiter">Character to indicate comment rows.</param>
        /// <param name="trimSpaces">Trim whitespace from character.</param>
        /// <param name="abortOnParseError">Specify whether to abort and throw an exeption when there is a parser error or just
        /// keep going but log the error.</param>
        /// <remarks>NOTE: It is the responsibility of the calling code for opending/closing readers.</remarks>
        public void Initialize(
            TextReader inReader, 
            bool inputHasHeaders, 
            char fieldDelimiter, 
            char quoteChar, 
            char escapeChar, 
            char commentDelimiter, 
            bool trimSpaces,
            bool abortOnParseError)
        {

            ValueTrimmingOptions trimOptions = ValueTrimmingOptions.None;
            if ( trimSpaces)
            {
                trimOptions = ValueTrimmingOptions.All;
            }
            //Create reader object
            _reader = new CsvReader(
                inReader,
                inputHasHeaders,
                fieldDelimiter,
                quoteChar,
                escapeChar,
                commentDelimiter,
                trimOptions);

            //Set actions
            _reader.MissingFieldAction = MissingFieldAction.ReplaceByNull;

            if(abortOnParseError)
            {
                _reader.DefaultParseErrorAction = ParseErrorAction.ThrowException;
            }
            else
            {
                _reader.DefaultParseErrorAction = ParseErrorAction.RaiseEvent;
                _reader.ParseError += new System.EventHandler<ParseErrorEventArgs>(_reader_ParseError);
            }

            //Reset parse errors
            _parserErrors = new List<string>();
        }
    
        /// <summary>
        /// Handler for parse error event.  S
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _reader_ParseError(object sender, ParseErrorEventArgs e)
        {
            //On parse error event, record the issue and keep going
            ParserErrors.Add(string.Format("Line {0}, Field {1}: {2}", e.Error.CurrentRecordIndex, e.Error.CurrentFieldIndex, e.Error.Message));
            e.Action = ParseErrorAction.AdvanceToNextLine;            
        }

        /// <summary>
        /// Read the next record from the CSV
        /// </summary>
        /// <returns></returns>
        public bool ReadNextRecord()
        {
            EnsureInitialized();
            
            bool recordExists = _reader.ReadNextRecord();

            _currentRawData = string.Empty;
            _currentRecord = new List<string>();

            if(recordExists)
            {
                _currentRawData = _reader.GetCurrentRawData();
               
                for(int i = 0; i < _reader.FieldCount; i++)
                {
                    _currentRecord.Add(_reader[i]);
                }
            }

            return recordExists;
        }

        /// <summary>
        /// Get a boolean indicating if there are more records to read in the CSV
        /// </summary>
        public List<string> CurrentRecord
        {
            get
            {
                EnsureInitialized();
                return _currentRecord;
            }
        }

        /// <summary>
        /// Get current raw data 
        /// </summary>
        public string CurrentRawData
        {
            get
            {
                EnsureInitialized();

                return string.Join(",", _currentRecord.ToArray());
            }
        }
    }
}
