using System;
using System.Text;
using Checkbox.Common;

namespace CheckboxWeb.Controls.Charts
{
    public partial class AverageScoreByPageGraph : ChartControlBase
    {
        protected override string GetTitle()
        {
            if ("false".Equals(Appearance["ShowTitle"], StringComparison.InvariantCultureIgnoreCase))
                return string.Empty;

            return "Average score per page";

            var sb = new StringBuilder();

            int titleFontSize = Utilities.AsInt(Appearance["TitleFontSize"], 18);

            bool showResponseCount = false; /* "true".Equals(Appearance["ShowResponseCountInTitle"], StringComparison.InvariantCultureIgnoreCase)
                 && "true".Equals(Appearance["ShowAnswerCount"], StringComparison.InvariantCultureIgnoreCase);*/
            bool multiSource = Model.SourcePages.Length > 1;

            foreach (var sourcePage in Model.SourcePages)
            {
                var sbForOneItem = new StringBuilder();

                string text = Utilities.DecodeAndStripHtml(sourcePage.ReportingText);
                sbForOneItem.Append(/*multiSource ? */Wrap(text)/* : text*/);
                /*
                if (showResponseCount)
                {
                    sbForOneItem.Append(multiSource ? "  <span style=\"font-size:" + titleFontSize * 2 / 3 + "pt;\">(" : Environment.NewLine);

                    int count = GetItemResponseCount(sourceItem.ItemId);
                    sbForOneItem.Append(count);
                    sbForOneItem.Append(" ");
                    sbForOneItem.Append(TextManager.GetText(count == 1 ? "/controlText/analysisItemRenderer/response" : "/controlText/analysisItemRenderer/responses", LanguageCode));

                    if (multiSource)
                        sbForOneItem.Append(")</span>");
                }
                */
                sb.Append(sbForOneItem);
                if (multiSource)
                    sb.Append(Environment.NewLine);

                //                sb.Append(SplitAndWrap(sbForOneItem.ToString())/* Wrap()*/);
            }

            return sb.ToString();
        }
    }
}