<%@ Control Language="C#" CodeBehind="SurveyPreview.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.SurveyPreview" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Forms.Items" %>
<%@ Import Namespace="Checkbox.Forms.Items.Configuration" %>
<%@ Import Namespace="Checkbox.Wcf.Services.Proxies" %>
<%@ Import Namespace="CheckboxWeb.Forms.Surveys.Controls.TakeSurvey" %>
<%@ Import Namespace="Checkbox.Forms" %>
<%@ Register TagPrefix="ckbx" TagName="PreviewControls" Src="~/Forms/Surveys/Controls/PreviewControls.ascx" %>
<%@ Register TagPrefix="ckbx" TagName="PageView" Src="~/Forms/Surveys/Controls/TakeSurvey/PageView.ascx" %>
<%@ Register TagPrefix="ckbx" TagName="Login" Src="~/Forms/Surveys/Controls/TakeSurvey/Login.ascx" %>
<%@ Register TagPrefix="ckbx" TagName="Password" Src="~/Forms/Surveys/Controls/TakeSurvey/EnterPassword.ascx" %>
<%@ Register TagPrefix="ckbx" TagName="LanguageSelect" Src="~/Forms/Surveys/Controls/TakeSurvey/LanguageSelect.ascx" %>
<%@ Register TagPrefix="ckbx" TagName="ProgressSaved" Src="~/Forms/Surveys/Controls/TakeSurvey/ProgressSaved.ascx" %>
<%@ Register TagPrefix="ckbx" TagName="ResponseSelect" Src="~/Forms/Surveys/Controls/TakeSurvey/ResponseSelect.ascx" %>

<asp:PlaceHolder ID="_previewPlaceHolder" runat="server">
    <%-- Preview Navigation --%>
    <ckbx:PreviewControls ID="_previewNavigation" runat="server" />
</asp:PlaceHolder>

<%-- Page Repeater --%>
<asp:Repeater ID="_pageRepeater" runat="server" OnItemCreated="_pageRepeater_ItemCreated">
    <ItemTemplate>
        <ckbx:PageView ID="_pageView" runat="server" />
    </ItemTemplate>
</asp:Repeater>
<%-- Login Control --%>
<ckbx:Login ID="_login" runat="server" />

<%-- Enter Password --%>
<ckbx:Password ID="_password" runat="server" />

<%-- Select Language --%>
<ckbx:LanguageSelect ID="_language" runat="server" />

<%-- Progress Saved --%>
<ckbx:ProgressSaved ID="_progressSaved" runat="server" />

<%-- Choose Response to Edit --%>
<ckbx:ResponseSelect ID="_responseSelect" runat="server" />

<script type="text/C#" runat="server">

    /// <summary>
    /// Handle item created to insert the page view(s) into layout place.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void _pageRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
    {
        //Add a layout template and bind
        if (e.Item.DataItem == null
            || !(e.Item.DataItem is TemplatePage))
        {
            return;
        }

        //Get the page
        var page = (TemplatePage)e.Item.DataItem;

        //Create a page view
        var pageView = e.Item.FindControl("_pageView") as PageView;

        if (pageView == null)
        {
            //TODO: Log/show error
            return;
        }

        //Count items on previous pages
        var previousItemCount = 0;

        //Skip hidden items
        for (int pagePosition = 2; pagePosition < page.Position; pagePosition++)
        {
            var previousPage = ResponseTemplate.GetPageAtPosition(pagePosition);

            if (previousPage != null)
            {
                previousItemCount +=
                    previousPage
                        .ListItemIds()
                        .Select(itemId => ResponseTemplate.GetItem(itemId))
                        .Count(itemMetaData => itemMetaData != null && itemMetaData.ItemIsIAnswerable && !"HiddenItem".Equals(itemMetaData.ItemTypeName, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        //Initialize and bind
        pageView.Initialize(
            page.GetDataTransferObject(),
            GetPageItems,
            RenderMode,
            GetDisplayFlags(page),
            ResponseTemplate.LanguageSettings,
            LanguageCode,
            ResponseTemplate,
            new PageNumberInfo { CurrentPageNumber = page.Position - 1, FirstItemNumber = previousItemCount + 1, TotalPageCount = -1 },
            ExportMode);

        if (!ResponseTemplate.StyleSettings.HideFooterHeader ||
            (RenderMode != RenderMode.SurveyMobile && RenderMode != RenderMode.SurveyMobilePreview))
        {
            pageView.ApplyHeaderAndFooter(HeaderHtml, FooterHtml);
        }

        pageView.BindRenderers();

        var printPanel = new Panel();

        if (page.ShouldForceBreak.HasValue
            && page.ShouldForceBreak.Value)
        {
            printPanel.Style["page-break-after"] = "always";
        }
        else
        {
            printPanel.Style["margin-bottom"] = "-70px";
        }

        e.Item.Controls.Add(printPanel);
    }


    /// <summary>
    /// Callback used to get response page items that will be bound to item renderers for preview.
    /// </summary>
    /// <param name="pageId"></param>
    /// <returns></returns>
    private IItemProxyObject[] GetPageItems(int pageId)
    {
        var result = new List<IItemProxyObject>();

        foreach (int itemId in ResponseTemplate.GetPage(pageId).ListItemIds())
        {
            //try to get item from ResponseTemplate to avoid overhead call of ItemConfigurationManager
            ItemData itemData = ResponseTemplate.GetItem(itemId);

            //Get item's configuration, which also acts as a factory for Item objects.
            if (itemData == null)
                ItemConfigurationManager.GetConfigurationData(itemId);

            //Exclude active items
            if (itemData == null || !itemData.IsActive)
            {
                continue;
            }

            //Create the item object, which is the encapsulation of any business logic for a
            // survey item.
            Item responseItem = itemData.CreateItem(LanguageCode, ResponseTemplate.ID, ExportMode);

            if (responseItem == null)
            {
                continue;
            }

            //Since item renderers are now designed to work through a service layer, they work on 
            // simple data objects, not full Item objects which contain business logic and data, 
            // so we need to create the data-only object, which supports serialization to XML,
            // JSON, etc.
            result.Add(responseItem.GetDataTransferObject());
        }

        return result.ToArray();
    }

    /// <summary>
    /// Get display flags that are based on style settings.
    /// </summary>
    /// <returns></returns>
    private ResponseViewDisplayFlags GetDisplayFlags(TemplatePage page)
    {
        //Navigation elements aren't needed in "print" mode
        if (ExportMode != ExportMode.None)
        {
            var printDisplayFlags = new ResponseViewDisplayFlags();
            if (ResponseTemplate.StyleSettings.ShowItemNumbers)
                printDisplayFlags |= ResponseViewDisplayFlags.ItemNumbers;

            if (ResponseTemplate.StyleSettings.ShowPageNumbers)
                printDisplayFlags |= ResponseViewDisplayFlags.PageNumbers;

            if (ResponseTemplate.StyleSettings.ShowTitle)
                printDisplayFlags |= ResponseViewDisplayFlags.Title;

            return printDisplayFlags;

        }

        //Handle completion page specially
        if (page.PageType == TemplatePageType.Completion)
        {
            ResponseViewDisplayFlags completionPageDisplayFlags = new ResponseViewDisplayFlags();

            if (ResponseTemplate.StyleSettings.ShowProgressBar)
            {
                completionPageDisplayFlags |= ResponseViewDisplayFlags.ProgressBar;
            }

            if (ResponseTemplate.StyleSettings.ShowTitle)
            {
                completionPageDisplayFlags |= ResponseViewDisplayFlags.Title;
            }

            return completionPageDisplayFlags;
        }


        ResponseViewDisplayFlags displayFlags =
            ResponseTemplate.StyleSettings.GetDisplayFlags()
            | ResponseTemplate.BehaviorSettings.GetDisplayFlags();

        //For last page show finish button, but for others show next button
        int[] pageIds = ResponseTemplate.ListTemplatePageIds();

        //If page is second to last page...
        if (pageIds.Length > 1 && page.ID == pageIds[pageIds.Length - 2])
        {
            displayFlags |= ResponseViewDisplayFlags.FinishButton;
        }
        else
        {
            displayFlags |= ResponseViewDisplayFlags.NextButton;
        }

        //Hidden items = Page 1 and first page = page 2.
        if (page.Position < 3)
        {
            //Mask &= ~(ulong)flg   
            displayFlags &= ~ResponseViewDisplayFlags.BackButton;
        }

        return displayFlags;
    }
</script>
