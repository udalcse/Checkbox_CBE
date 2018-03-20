/****************************************************************************
 * IImportExportTextProvider.cs                                             *
 * Interface definition for a text provider that supports bulk import       *
 * and export operations                                                    *
 ****************************************************************************/
namespace Checkbox.Globalization.Text
{
    /// <summary>
    /// Text provider supports getting/setting localizable texts
    /// </summary>
    public interface IImportExportTextProvider
    {
        /// <summary>
        /// Import texts from a stream and write them to the database.
        /// </summary>
        /// <param name="reader">Opened reader containing the stream to read data from.</param>
        void ImportTexts(System.IO.TextReader reader);

        /// <summary>
        /// Export all texts from the database and write them to a stream.
        /// </summary>
        /// <param name="writer">Opened writer to write data do.</param>
        void ExportAllTexts(System.IO.TextWriter writer);

        /// <summary>
        /// Export all texts that have a TextID which contains the specified partial TextID.
        /// This is useful for getting all tText of a specific type, for example validation messages.
        /// </summary>
        /// <param name="writer">Opened writer to write data to.</param>
        /// <param name="languageCodes">Language codes to export text for. If null, Application Languages will be used.</param>
        /// <param name="partialTextId">List of partial TextIDs to search for. </param>
        void ExportFilteredTexts(System.IO.TextWriter writer, string[] languageCodes, params string[] partialTextId);

        /// <summary>
        /// Export all texts that have an id equal to the specified TextID and the language code.
        /// </summary>
        /// <param name="writer">Opened writer to write data to.</param>
        /// <param name="languageCodes">Language codes to export text for. If null, Application Languages will be used.</param>
        /// <param name="textIds">The List of TextIds to export.</param>
        void ExportTextsById(System.IO.TextWriter writer, string[] languageCodes, string[] textIds);
    }
}
