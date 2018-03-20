using System.Web.UI.WebControls;
using Checkbox.Globalization.Text;
using Checkbox.Users;

namespace Checkbox.Analytics.Export
{
    /// <summary>
    /// A simple class which contains the supported export formats.
    /// </summary>
    public static class ExportFormat
    {
        public static string Csv
        {
            get { return "CSV"; }
        }

        public static string SpssCsv
        {
            get { return "SPSS_CSV"; }
        }

        public static string SpssNative
        {
            get { return "SPSS_Native"; }
        }

        public static ListItemCollection SupportedFormats(string languageCode)
        {
            ListItemCollection list = new ListItemCollection
                                          {
                                              new ListItem(
                                                  TextManager.GetText(
                                                      "/pageText/forms/surveys/responses/export.aspx/standardCsv",
                                                      languageCode), Csv),
                                              new ListItem(
                                                  TextManager.GetText(
                                                      "/pageText/forms/surveys/responses/export.aspx/spss",
                                                      languageCode), SpssCsv),
                                              new ListItem(
                                                  TextManager.GetText(
                                                      "/pageText/forms/surveys/responses/export.aspx/spssNative",
                                                      languageCode), SpssNative)
                                          };

            return list;
        }
    }
}
