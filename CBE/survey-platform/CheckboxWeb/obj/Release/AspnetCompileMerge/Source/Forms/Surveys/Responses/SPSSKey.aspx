<%@ Page Language="C#" CodeBehind="SPSSKey.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Responses.SPSSKey" MasterPageFile="~/Dialog.master" %>
<%@ Import Namespace="Checkbox.Forms.Items.Configuration" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Import Namespace="Checkbox.Web.UI.Controls" %>
<%@ Import Namespace="Prezza.Framework.ExceptionHandling" %>

<%@ MasterType VirtualPath="~/Dialog.master" %>
<asp:Content ID="content" runat="server" ContentPlaceHolderID="_pageContent">
    <script type="text/javascript">
        function showTable() {
            document.getElementById('spssTable').removeAttribute('class');
        }
    </script>
    
    <br />
    <br />
    <table id="spssTable" class="hidden" cellspacing="1" cellpadding="2" style="border:1px solid gray;">
        <tr valign="top" style="font-size:14px;font-weight:bold;text-decoration:underline;">
            <td style="width:100px"> <%= WebTextManager.GetText("/pageText/spssKey.aspx/spssCode") %> </td>
            <td><%= WebTextManager.GetText("/pageText/spssKey.aspx/fullText") %> </td>
        </tr>
            <%   
                try
                {                    
                    foreach (int pageId in ResponseTemplate.ListTemplatePageIds())
                    {
                        var page = ResponseTemplate.GetPage(pageId);

                        if (page.IsAnyAnswerableItemOnPage(ResponseTemplate, IncludeOpenEnded)) { %>
                            <tr>
                                <td colspan="2"><hr size="1" width="100%"></td>
                            </tr>
                        <% }
                 
                        foreach (int itemDataId in page.ListItemIds())
                        {
                            var itemData = ResponseTemplate.GetItem(itemDataId);
                            if (itemData.ItemIsIAnswerable)
                            {
                                if (itemData is ICompositeItemData)
                                {
    %>
                                <tr valign="top">
                                    <td colspan="2"><br /></td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td><hr size="1" width="100%" style="color:#EFEFEF;"></td>
                                </tr>
                                <tr valign="top">
                                    <td style="width:100px"></td>
                                    <td style="font-size:12px;">
                                        <%= GetItemText(itemData) %>
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td><hr size="1" width="100%" style="color:#EFEFEF;"></td>
                                </tr>
                                <%= AddItemKey(itemData, string.Empty) %>
                                <tr>
                                    <td></td>
                                    <td><hr size="1" width="100%" style="color:#EFEFEF;"></td>
                                </tr>

                            <% } 
                                //Add select items, and if including open-ended, add text and hidden items
                                else if(page.IsItemAnswerable(itemData, IncludeOpenEnded))
                                { %> 
                                <%= AddItemKey(itemData, string.Empty) %>
                            <% }
                            }
                        }
                    }%>
                    <!--if there are no errors, show the table -->
                    <script type="text/javascript"> showTable();</script>
                <%}
                catch(Exception ex)
                {
                    HandleExceptions(ex);
                }
            %>
    </table>
        
    <asp:Placeholder ID="_errorPlaceholder" runat="server"></asp:Placeholder>
</asp:Content>

<script runat="server" type="text/C#">
    /// <summary>
    /// Add the key for the item
    /// </summary>
    /// <param name="itemData"></param>
    /// <param name="prefix"></param>
    protected string AddItemKey(ItemData itemData, string prefix)
    {
        StringBuilder stringBuilder = new StringBuilder();

        if (itemData is TabularItemData)
        {
            MatrixItemTextDecorator decorator = null;

            if (itemData is MatrixItemData)
            {
                decorator = (MatrixItemTextDecorator)itemData.CreateTextDecorator(LanguageCode);
            }

            for (int i = 1; i <= ((TabularItemData)itemData).RowCount; i++)
            {
                string childPrefix = string.Empty;

                if (decorator != null)
                    childPrefix = decorator.GetRowText(i) + " - ";

                for (int j = 1; j <= ((TabularItemData)itemData).ColumnCount; j++)
                {
                    int? childItemId = ((TabularItemData)itemData).GetItemIdAt(i, j);
                    if (!childItemId.HasValue)
                        continue;

                    ItemData childItemData = ResponseTemplate.GetItem(childItemId.Value);

                    if (childItemData != null && childItemData.ItemIsIAnswerable)
                    {
                        if (decorator == null)
                        {
                            AddItemKey(childItemData, childPrefix);
                        }
                        else if (childItemData is SelectItemData || 
                            (childItemData is TextItemData && IncludeOpenEnded))
                        {
                            ItemCount++;
                            stringBuilder.Append(BuildItemKey(childItemData, childPrefix, true, decorator, j));
                        }
                    }
                }
            }
        }
        else
        {
            ItemCount++;
            stringBuilder.Append(BuildItemKey(itemData, prefix, false, null, null));
        }

        return stringBuilder.ToString();
    }

    private void HandleExceptions(Exception ex)
    {
        ExceptionPolicy.HandleException(ex, "UIProcess");

        var errorMessage = new ErrorMessage
        {
            Message = WebTextManager.GetText("/errorMessages/common/errorOccurred"),
            Exception = ex,
            Visible = true
        };

        _errorPlaceholder.Controls.Add(errorMessage);
    }

    private string BuildItemKey(ItemData itemData, string prefix, bool isTabularItem, MatrixItemTextDecorator decorator, int? column)
    {
        StringBuilder stringBuilder = new StringBuilder();
        
        if (!isTabularItem)
            stringBuilder.Append("<tr valign=\"top\"><td colspan=\"2\"><br /></td></tr>");

        stringBuilder.Append("<tr valign=\"top\">");

        if (!isTabularItem)
            stringBuilder.Append("<td style=\"width:100px;font-size:12px;\" cssClass=\"PrezzaNormal\">Q" + ItemCount + "</td>");
        else
            stringBuilder.Append("<td style=\"width:100px\" style=\"font-size:12px;\">Q" + ItemCount + "</td>");
        
        stringBuilder.Append("<td  style=\"font-size:12px;\">");
        stringBuilder.Append(prefix + GetItemText(itemData));
        stringBuilder.Append("</td>");
        stringBuilder.Append("</tr>");

        //Now enumerate the options     
        stringBuilder.Append("<tr valign=\"top\">");
        stringBuilder.Append("<td style=\"width:100px\"></td><td style=\"font-size:12px;\">");
        stringBuilder.Append("<table cellspacing=\"1\" cellpadding=\"2\" width=\"100%\">");

        if (itemData is SelectItemData)
        {
            for (int k = 1; k <= ((SelectItemData)itemData).Options.Count; k++)
            {
                stringBuilder.Append("<tr>");
                stringBuilder.Append("<td align=\"left\" style=\"font-size:12px;width:25px\">" + k + "</td>");
                
                if (isTabularItem)
                    stringBuilder.Append("<td align=\"left\" style=\"font-size:12px;\">" + decorator.GetOptionText(column.Value, ((SelectItemData)itemData).Options[k - 1].Position) + "</td>");
                else
                    stringBuilder.Append("<td align=\"left\" style=\"font-size:12px;\">" + GetOptionText((SelectItemData)itemData, ((SelectItemData)itemData).Options[k - 1]) + "</td>");
                
                stringBuilder.Append("</tr>");
            }
        }

        stringBuilder.Append("</table>");
        stringBuilder.Append("</tr></td></tr>");

        return stringBuilder.ToString();
    }
</script>

