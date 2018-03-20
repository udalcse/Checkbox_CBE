using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Checkbox.Forms.Items;
using Checkbox.Globalization.Text;
using Checkbox.Management;
using Checkbox.Users;

namespace Checkbox.Forms
{
    /// <summary>
    /// Format responses into various text formats.
    /// </summary>
    public static class ResponseFormatter
    {
        /// <summary>
        /// Format the response in the desired format.  The boolean indicates whether skipped
        /// pages and items should be marked or omitted entirely.
        /// </summary>
        /// <param name="response">Response to format.</param>
        /// <param name="format">Format for response.</param>
        /// <returns>String formatted response.</returns>
        /// <remarks>
        /// This is more of a stop-gap measure for now until Response, Page, and Items are refactored
        /// to better separate state, business, and data access logic.
        /// </remarks>
        public static string Format(Response response, string format, bool includeResponseDetails, bool showPageNumbers,
            bool showQuestionNumbers, bool includeMessageItems, bool showHiddenItems, bool includeResponseIds = true, List<SurveyTerm> terms = null)
        {
            //Xml format
            if (format.Equals("Xml", StringComparison.InvariantCultureIgnoreCase))
            {
                return FormatXml(response);
            }

            //Html format
            if (format.Equals("Html", StringComparison.InvariantCultureIgnoreCase))
            {
                return FormatHtml(response, includeResponseDetails, showPageNumbers, showQuestionNumbers, includeMessageItems, showHiddenItems, includeResponseIds, terms);
            }
            //Default to text format
            return FormatText(response, includeResponseDetails, showPageNumbers, showQuestionNumbers, includeMessageItems, showHiddenItems);
        }

        /// <summary>
        /// Format the rseponse in XML format
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private static string FormatXml(Response response)
        {
            StringWriter stringWriter = new StringWriter();
            XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter);

            xmlWriter.Formatting = Formatting.Indented;

            response.WriteXml(xmlWriter);

            xmlWriter.Flush();
            xmlWriter.Close();

            return stringWriter.ToString();
        }

        /// <summary>
        /// Format the response
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private static string FormatHtml(Response response, bool includeResponseDetails, bool showPageNumbers,
            bool showQuestionNumbers, bool includeMessageItems, bool showHiddenItems, bool includeResponseIds = true, List<SurveyTerm> terms = null)
        {
            StringBuilder sb = new StringBuilder();

            //Add response properties

            //Title
            sb.Append("<div class=\"title\">");
            sb.Append(response.Title);
            sb.AppendLine("</div>");

            //Spacer
            sb.AppendLine("<hr size=\"1\" />");

            if (includeResponseDetails)
            {
                //Respondent unique identifier
                sb.Append("<div style=\"padding:3px;\"><span style=\"width:150px;font-weight:bold;\">");
                sb.Append(TextManager.GetText("/responseProperty/uniqueIdentifier/text", response.LanguageCode));
                sb.Append(":&nbsp;&nbsp;");
                sb.Append("</span><span>");
                sb.Append(response.UniqueIdentifier);
                sb.Append("</span></div>");

                if (includeResponseIds)
                {
                    //Response Id
                    sb.Append("<div style=\"padding:3px;\"><span style=\"width:150px;font-weight:bold;\">");
                    sb.Append(TextManager.GetText("/responseProperty/responseId/text", response.LanguageCode));
                    sb.Append(":&nbsp;&nbsp;");
                    sb.Append("</span><span>");
                    sb.Append(response.ID.ToString());
                    sb.Append("</span></div>");

                    //Guid
                    sb.Append("<div style=\"padding:3px;\"><span style=\"width:150px;font-weight:bold;\">");
                    sb.Append(TextManager.GetText("/responseProperty/responseGuid/text", response.LanguageCode));
                    sb.Append(":&nbsp;&nbsp;");
                    sb.Append("</span><span>");
                    sb.Append(response.GUID.ToString());
                    sb.Append("</span></div>");
                }

                //Started
                sb.Append("<div style=\"padding:3px;\"><span style=\"width:150px;font-weight:bold;\">");
                sb.Append(TextManager.GetText("/responseProperty/started/text", response.LanguageCode));
                sb.Append(":&nbsp;&nbsp;");
                sb.Append("</span><span>");
                sb.Append(ConvertToClientTimeZone(response.DateCreated.Value).ToLongDateString());
                sb.Append("&nbsp;");
                sb.Append(ConvertToClientTimeZone(response.DateCreated.Value).ToLongTimeString());
                sb.Append("</span></div>");

                //Completed
                sb.Append("<div style=\"padding:3px;\"><span style=\"width:150px;font-weight:bold;\">");
                sb.Append(TextManager.GetText("/responseProperty/ended/text", response.LanguageCode));
                sb.Append(":&nbsp;&nbsp;");
                sb.Append("</span><span>");
                sb.Append(response.DateCompleted.HasValue ? ConvertToClientTimeZone(response.DateCompleted.Value).ToLongDateString() : string.Empty);
                sb.Append("&nbsp;");
                sb.Append(response.DateCompleted.HasValue ? ConvertToClientTimeZone(response.DateCompleted.Value).ToLongTimeString() : string.Empty);
                sb.Append("</span></div>");
                //Spacer
                sb.AppendLine("<hr size=\"1\" />");
            }

            sb.Append(RenderPages(response, includeResponseDetails, showPageNumbers, showQuestionNumbers, includeMessageItems, showHiddenItems, "Html", terms));

            return sb.ToString();
        }

        /// <summary>
        /// Format a response in text format
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private static string FormatText(Response response, bool includeResponseDetails, bool showPageNumbers,
           bool showQuestionNumbers, bool includeMessageItems, bool showHiddenItems, List<SurveyTerm> terms = null)
        {
            StringBuilder sb = new StringBuilder();

            //Add response properties

            //Title
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(response.Title);
            sb.Append('-', response.Title.Length);
            sb.AppendLine();

            if (includeResponseDetails)
            {
                //Respondent unique identifier
                sb.Append(TextManager.GetText("/responseProperty/uniqueIdentifier/text", response.LanguageCode));
                sb.Append(":  ");
                sb.AppendLine(response.UniqueIdentifier);

                //Response Id
                sb.Append(TextManager.GetText("/responseProperty/responseId/text", response.LanguageCode));
                sb.Append(":  ");
                sb.AppendLine(response.ID.ToString());

                //Guid
                sb.Append(TextManager.GetText("/responseProperty/responseGuid/text", response.LanguageCode));
                sb.Append(":  ");
                sb.AppendLine(response.GUID.ToString());

                //Started
                sb.Append(TextManager.GetText("/responseProperty/started/text", response.LanguageCode));
                sb.Append(":  ");
                sb.Append(response.DateCreated.Value.ToLongDateString());
                sb.Append(" ");
                sb.AppendLine(response.DateCreated.Value.ToLongTimeString());

                //Completed
                sb.Append(TextManager.GetText("/responseProperty/ended/text", response.LanguageCode));
                sb.Append(":  ");
                sb.Append(response.DateCompleted.HasValue ? response.DateCompleted.Value.ToLongDateString() : string.Empty);
                sb.Append(" ");
                sb.AppendLine(response.DateCompleted.HasValue ? response.DateCompleted.Value.ToLongTimeString() : string.Empty);
                sb.AppendLine();
            }

            sb.Append(RenderPages(response, includeResponseDetails, showPageNumbers, showQuestionNumbers, includeMessageItems, showHiddenItems, "Text", terms));

            return sb.ToString();
        }

        private static string RenderPages(Response response, bool includeResponseDetails, bool showPageNumbers,
            bool showQuestionNumbers, bool includeMessageItems, bool showHiddenItems, string format, List<SurveyTerm> terms = null)
        {
            var sb = new StringBuilder();

            //Render pages
            foreach (ResponsePage page in response.GetResponsePages())
            {
                if (!page.ItemsLoaded)
                {
                    response.ReloadPageItems(page, null);
                    page.OnLoad(false);
                }
                
                if (!page.Excluded && page.GetItems().Any(pi => (pi is IAnswerable && ((IAnswerable)pi).HasAnswer) || MatrixField.IsMatrixInResponces(pi.ID, (Guid)response.GUID, response.UniqueIdentifier)))
                    sb.Append(ResponsePageFormatter.Format(page, format, includeResponseDetails, showPageNumbers, showQuestionNumbers, includeMessageItems, showHiddenItems, response.ScoringEnabled, terms));
            }

            return sb.ToString();
        }

        private static DateTime ConvertToClientTimeZone(DateTime dateTime)
        {
            return DateTime.SpecifyKind(dateTime.AddHours(ApplicationManager.AppSettings.TimeZone - ApplicationManager.ServersTimeZone), DateTimeKind.Unspecified);
        }
    }
}
