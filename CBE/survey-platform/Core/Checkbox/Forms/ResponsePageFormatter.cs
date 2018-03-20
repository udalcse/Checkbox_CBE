using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Checkbox.Common;
using Checkbox.Forms.Items;
using Checkbox.Globalization.Text;
using Checkbox.Security;
using Checkbox.Users;
using Newtonsoft.Json;

namespace Checkbox.Forms
{
    /// <summary>
    /// Format response pages into text format
    /// </summary>
    public static class ResponsePageFormatter
    {
        /// <summary>
        /// Format a response page into the desired format.
        /// </summary>
        /// <param name="page">Page to format.</param>
        /// <param name="format">Format for page.</param>
        /// <returns>Page formatted as string.</returns>
        /// <remarks>Format strings of HTML and Text are supported, with text as the default.</remarks>
        public static string Format(ResponsePage page, string format, bool includeResponseDetails, bool showPageNumbers,
            bool showQuestionNumbers,
            bool includeMessageItems, bool showHiddenItems, bool showScore, List<SurveyTerm> terms = null)
        {
            if (format.Equals("Html", StringComparison.InvariantCultureIgnoreCase))
            {
                return FormatHtml(page, includeResponseDetails, showPageNumbers, showQuestionNumbers,
                    includeMessageItems, showHiddenItems, showScore, terms);
            }

            return FormatText(page, includeResponseDetails, showPageNumbers, showQuestionNumbers, includeMessageItems,
                showHiddenItems, showScore);
        }

        /// <summary>
        /// Format the response page as html
        /// </summary>
        /// <param name="page">Page to format.</param>
        /// <returns>Page formatted as html.</returns>
        private static string FormatHtml(ResponsePage page, bool includeResponseDetails, bool showPageNumbers,
            bool showQuestionNumbers, bool includeMessageItems, bool showHiddenItems, bool showScore, List<SurveyTerm> terms = null)
        {
            if (page.Excluded || page.GetItems().Count == 0 ||
                (page.PageType == TemplatePageType.HiddenItems && !showHiddenItems))
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            sb.Append("<div style=\"border:1px solid #B2B2B2;margin-top:25px;padding:5px;\">");
            sb.Append("<div style=\"font-size:14px;font-weight:bold;text-decoration:underline;\">");


            if (showPageNumbers)
            {
                if (page.PageType == TemplatePageType.Completion)
                {
                    sb.Append(TextManager.GetText("/pageText/formEditor.aspx/completionEvents", page.Parent.LanguageCode));
                }
                else if (page.Position > 1)
                {
                    sb.Append(TextManager.GetText("/controlText/templatePageEditor/page", page.Parent.LanguageCode));
                    sb.Append(" ");
                    sb.Append(page.Position - 1);
                }
                else
                {
                    sb.Append(TextManager.GetText("/pageText/formEditor.aspx/hiddenItems", page.Parent.LanguageCode));
                }
            }

            sb.Append("</div>");

            //Add the items
            var pageItems = page.GetItems();
            bool containsFormattedItems = false;
            foreach (Item item in pageItems)
            {
                if (!item.Excluded)
                {
                    if (item is IAnswerable || (item.ShouldRender && includeMessageItems))
                    {
                        IItemFormatter formatter = ItemFormatterFactory.GetItemFormatter(item.TypeID, "Html");

                        if (formatter != null)
                        {
                            string formattedItem = formatter.Format(item, "Html", showScore);

                            if (item.ItemTypeName.Equals("Matrix") && PropertyBindingManager.IsBinded(item.ID))
                            {
                                var jsonValue = PropertyBindingManager.GetResponseFieldState(item.ID,
                                    page.Parent.GUID.Value, page.Parent.UniqueIdentifier);
                                var matrix = MatrixField.JsonToMatrix(jsonValue);
                                formattedItem = string.Format("<div>{0}</div>{1}", (item as MatrixItem).Text,
                                    MatrixField.BuildMatrixHtmlFromJobject(MatrixField.MatrixRowsToJson(matrix)));
                            }
                            else if (item.ItemTypeName.Equals("RadioButtons") && PropertyBindingManager.IsBinded(item.ID))
                            {
                                MapBindedStateToSelectItem(item, page);
                            }

                            if (Utilities.IsNotNullOrEmpty(formattedItem))
                            {
                                containsFormattedItems = true;

                                sb.Append("<br /><br />");
                                if (showQuestionNumbers)
                                    sb.AppendFormat("<p>{0}</p>", GetItemNumber(page, item));

                                formattedItem = ReplaceTerms(formattedItem, terms);
                                sb.Append(formattedItem);
                            }
                        }
                    }
                }
            }

            sb.Append("</div>");


            if (containsFormattedItems)
            {
                return sb.ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Replace temrs occurance with text
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="formaterItem">html for item.</param>
        private static string ReplaceTerms(string formaterItem, List<SurveyTerm> terms = null)
        {
            if (terms == null)
            {
                return formaterItem;
            }

            var reg = new Regex(@"[^\w]");

            foreach (var term in terms)
            {
                var termName = "%%" + term.Name;
                var index = -1;
                while ((index = formaterItem.IndexOf(termName, index < 0 ? 0 : index)) > 0 && index < formaterItem.Length)
                {
                    if (reg.Match(formaterItem[index + termName.Length].ToString()).Success)
                    {
                        formaterItem = formaterItem.Replace(termName, term.Term);
                    }
                    index++;
                }
            }

            return formaterItem;
        }
        /// <summary>
        /// Maps the binded state to select item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="page">The page.</param>
        private static void MapBindedStateToSelectItem(Item item, ResponsePage page)
        {
            var jsonValue = PropertyBindingManager.GetResponseFieldState(item.ID,
                          page.Parent.GUID.Value, page.Parent.UniqueIdentifier);

            if (!string.IsNullOrEmpty(jsonValue))
            {
                var radioButton = JsonConvert.DeserializeObject<RadioButtonField>(jsonValue);

                var radioItem = item as Select1;
                if (radioItem != null)
                {
                    var radioButtonFieldOption =
                        radioButton.Options.FirstOrDefault(radioOption => radioOption.IsSelected);
                    if (radioButtonFieldOption != null)
                    {
                        radioItem.Options.Clear();

                        foreach (var option in radioButton.Options)
                            radioItem.Options.Add(new ListOption()
                            {
                                Text = string.IsNullOrEmpty(option.Alias) ? option.Name : option.Alias,
                                IsSelected = option.IsSelected
                            });
                    }

                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private static string GetItemNumber(ResponsePage page, Item item)
        {
            var answerableItem = item as IAnswerable;

            if (answerableItem == null || (!answerableItem.HasAnswer && !(answerableItem is MatrixItem)))
            {
                return string.Empty;
            }

            var itemNumber = page.Parent.GetItemNumber(item.ID);

            return itemNumber.HasValue ? itemNumber.ToString() : string.Empty;
        }


        /// <summary>
        /// Format the page as text.
        /// </summary>
        /// <param name="page">Page to format.</param>
        /// <returns>Page formatted as text.</returns>
        private static string FormatText(ResponsePage page, bool includeResponseDetails, bool showPageNumbers,
            bool showQuestionNumbers, bool includeMessageItems, bool showHiddenItems, bool showScore)
        {
            if (page.Excluded || page.GetItems().Count == 0 || (page.PageType == TemplatePageType.HiddenItems && !showHiddenItems))
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine();

            if (showPageNumbers)
            {
                if (page.PageType == TemplatePageType.Completion)
                {
                    sb.Append(TextManager.GetText("/pageText/formEditor.aspx/completionEvents", page.Parent.LanguageCode));
                }
                else
                    if (page.Position > 1)
                    {
                        sb.Append(TextManager.GetText("/controlText/templatePageEditor/page", page.Parent.LanguageCode));
                        sb.Append(" ");
                        sb.Append(page.Position - 1);
                    }
                    else
                    {
                        sb.Append(TextManager.GetText("/pageText/formEditor.aspx/hiddenItems", page.Parent.LanguageCode));
                    }
            }

            //Add the items
            var pageItems = page.GetItems();
            bool containsFormattedItems = false;

            foreach (Item item in pageItems)
            {

                if (!item.Excluded)
                {
                    if (item is IAnswerable || (item.ShouldRender && includeMessageItems))
                    {
                        IItemFormatter formatter = ItemFormatterFactory.GetItemFormatter(item.TypeID, "Text");

                        if (formatter != null)
                        {
                            string formattedItem = formatter.Format(item, "Text", showScore);

                            if (Utilities.IsNotNullOrEmpty(formattedItem))
                            {
                                sb.AppendLine();
                                if (showQuestionNumbers)
                                {
                                    sb.AppendFormat("{0} ", GetItemNumber(page, item));
                                }

                                containsFormattedItems = true;

                                sb.AppendLine(formattedItem);
                            }
                        }
                    }
                }
            }

            if (containsFormattedItems)
            {
                return sb.ToString();
            }

            return string.Empty;
        }
    }
}
