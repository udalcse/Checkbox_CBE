<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="DetailsTable.ascx.cs" Inherits="CheckboxWeb.Controls.Charts.DetailsTable" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Globalization.Text" %>
<%@ Import Namespace="Checkbox.Users" %>
<%@ Import Namespace="Prezza.Framework.Security" %>
<%@ Import Namespace="Checkbox.Analytics.Configuration" %>

<style type="text/css">
    #<%=_theGrid.ClientID%> table
    {
        margin-left:auto;
        margin-right:auto;
        border:1px solid #999999;
        width:<%=Appearance["Width"]%>px;
    }
    #<%=_theGrid.ClientID%> th {padding:3px;border-left:1px solid #999999;background-color:#DEDEDE;}
    #<%=_theGrid.ClientID%> tr {background-color:#fff;}
    #<%=_theGrid.ClientID%> tr:hover {background-color:#f3f3f3;}
    #<%=_theGrid.ClientID%> td {padding:3px;border-left:1px solid #999999;border-top:1px solid #999999;}
    #<%=_theGrid.ClientID%> td.detailLink{width:16px;}
</style>

<div style="text-align:center;margin:5px;">
    <asp:Label ID="_lblTitle" runat="server" CssClass="Question"></asp:Label>
</div>
    
<asp:GridView ID="_theGrid" runat="server" ShowHeader="false"  ShowFooter="false" AutoGenerateColumns="false" CssClass="matrix Matrix" RowStyle-HorizontalAlign="Left" RowStyle-CssClass="Item" AlternatingRowStyle-CssClass="AlternatingItem" OnRowDataBound="TheGridOnRowDataBound" Style="margin:auto;">
    <Columns>
        <asp:BoundField DataField="ResultText" HtmlEncode="false"/>
        <asp:BoundField ItemStyle-CssClass="detailLink" HtmlEncode="false" />
    </Columns>
</asp:GridView>

<ckbx:MultiLanguageLabel runat="server" ID="_tableRowLimitForPdfPrintWarning" Visible="False" ></ckbx:MultiLanguageLabel>

 <script type="text/C#" runat="server">
     private IAuthorizationProvider _authorizationProvider;

     protected IAuthorizationProvider AuthorizationProvider
     {
         get { return _authorizationProvider ?? (_authorizationProvider = AuthorizationFactory.GetAuthorizationProvider()); }
     }

     /// <summary>
     /// Get whether Include Response Details option is enabled or not
     /// </summary>
     public bool IncludeResponseDetails
     {
         get { return "true".Equals(Model.Metadata["IncludeResponseDetails"], StringComparison.InvariantCultureIgnoreCase); }
     }

     /// <summary>
     /// Get whether Show Page Numbers option is enabled or not
     /// </summary>
     public bool ShowPageNumbers
     {
         get { return "true".Equals(Model.Metadata["ShowPageNumbers"], StringComparison.InvariantCultureIgnoreCase); }
     }

     /// <summary>
     /// Get whether Include Message/HTML Items option is enabled or not
     /// </summary>
     public bool IncludeMessageItems
     {
         get { return "true".Equals(Model.Metadata["IncludeMessageItems"], StringComparison.InvariantCultureIgnoreCase); }
     }

     /// <summary>
     /// Get whether Include Message/HTML Items option is enabled or not
     /// </summary>
     public bool ShowHiddenItems
     {
         get { return "true".Equals(Model.Metadata["ShowHiddenItems"], StringComparison.InvariantCultureIgnoreCase); }
     }

     /// <summary>
     /// Initialize grid/text
     /// </summary>
     protected override void InlineInitialize()
     {
         base.InlineInitialize();

         //Set title
         _lblTitle.Text = GetTitle();
     }

     /// <summary>
     /// Bind grid
     /// </summary>
     protected override void InlineBindModel()
     {
         base.InlineBindModel();

         DecodeOptions();

         var results = Model.DetailResults;
         if (ExportMode == ExportMode.Pdf)
         {
             int max = ((ReportPerformanceConfiguration)
                 Prezza.Framework.Configuration.ConfigurationManager.
                     GetConfiguration("checkboxReportPerformanceConfiguration")).
                         MaxResponseDetailsItemRowsForPdfExport;

             if (results.Length > max)
             {
                 results = results.Take(max).ToArray();
                 var warningText = TextManager.GetText("/itemText/tableRowLimitForPdfExport");
                 _tableRowLimitForPdfPrintWarning.Text = string.Format(warningText, max);
                 _tableRowLimitForPdfPrintWarning.Visible = true;
             }
         }

         //Order by response id / item position by default
         if (Model.DetailResults != null)
         {
             _theGrid.DataSource = results
                 .OrderBy(detail => detail.ResponseId)
                 .OrderBy(detail => detail.ResultIndex);

             _theGrid.DataBind();
             SetWidth();
         }
     }

     private void DecodeOptions()
     {
         foreach (var result in Model.DetailResults)
         {
             string text = result.ResultText;
             text = Utilities.AdvancedHtmlDecode(text);
             text = Utilities.EncodeTagsInHtmlContent(text);
             result.ResultText = text;
         }
     }

     /// <summary>
     /// Get language code for text
     /// </summary>
     public string LanguageCode
     {
         get
         {
             return string.IsNullOrEmpty(Model.InstanceData["LanguageCode"])
                 ? TextManager.DefaultLanguage
                 : Model.InstanceData["LanguageCode"];
         }
     }

     /// <summary>
     /// Get response count for item
     /// </summary>
     /// <param name="itemId"></param>
     /// <returns></returns>
     public int GetItemResponseCount(int itemId)
     {
         var itemData = Model.SourceItems.FirstOrDefault(item => item.ItemId == itemId);

         return itemData != null ? itemData.ResponseCount : 0;
     }

     /// <summary>
     /// Get the chart title
     /// </summary>
     protected string GetTitle()
     {
         var sb = new StringBuilder();

         bool showResponseCount = "true".Equals(Appearance["ShowResponseCountInTitle"], StringComparison.InvariantCultureIgnoreCase);
         bool multiSource = Model.SourceItems.Length > 1;

         foreach (var sourceItem in Model.SourceItems
             .OrderBy(si => si.PagePosition)
             .ThenBy(si => si.ItemPosition)
             .ThenBy(si => si.ParentColumnNumber)
             .ThenBy(si => si.ParentRowNumber))
         {
             sb.Append(Utilities.AdvancedHtmlEncode(Utilities.DecodeAndStripHtml(sourceItem.ReportingText)));

             if (showResponseCount)
             {
                 if (multiSource)
                 {
                     sb.Append("  ");
                     sb.Append(GetItemResponseCount(sourceItem.ItemId));
                     sb.Append(" ");
                     sb.Append(TextManager.GetText("/controlText/analysisItemRenderer/responses", LanguageCode));
                 }
                 else
                 {
                     sb.Append("<br />");
                     sb.Append(GetItemResponseCount(sourceItem.ItemId));
                     sb.Append(" ");
                     sb.Append(TextManager.GetText("/controlText/analysisItemRenderer/responses", LanguageCode));
                 }
             }

             if (multiSource)
             {
                 sb.Append("<br />");
             }
         }

         return sb.ToString();
     }

     public void TheGridOnRowDataBound(object sender, GridViewRowEventArgs e)
     {
         if(e.Row.RowType == DataControlRowType.DataRow
             && AuthorizationProvider.Authorize(UserManager.GetCurrentPrincipal(), "Analysis.Responses.View")
             && bool.Parse(Model.Metadata["LinkToResponseDetails"]))
         {
             var responseGuid = ((Checkbox.Wcf.Services.Proxies.DetailResult) e.Row.DataItem).ResponseGuid;

             if (responseGuid.HasValue)
             {
                 e.Row.Cells[1].Text = string.Format("<a style='vertical-align:middle;' href='" +
                     ResolveUrl("~/Forms/Surveys/Responses/View.aspx?responseGuid={0}&showMessages={1}&showPageNumbers={2}&includeDetails={3}") +
                     "' target='_blank'><img src='" + ResolveUrl("~/App_Themes/CheckboxTheme/Images/details16.gif") + "' /></a>",
                     responseGuid, IncludeMessageItems.ToString(), ShowPageNumbers.ToString(),IncludeResponseDetails.ToString());
             }
             else
             {
                 e.Row.Cells[1].Text = "<a style='vertical-align:middle;' href='javascript:void(0);'><img src='" + ResolveUrl("~/App_Themes/CheckboxTheme/Images/details16.gif") + "' /></a>";
             }
         }
     }

     /// <summary>
     /// 
     /// </summary>
     /// <returns></returns>
     protected void SetWidth()
     {
         if (!string.IsNullOrEmpty(Appearance["Width"]))
         {
             int width;

             if (int.TryParse(Appearance["Width"], out width))
             {
                 _theGrid.Width = Unit.Pixel(width);
             }
         }
     }

 </script>