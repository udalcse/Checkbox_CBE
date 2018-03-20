<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ItemListFromSurvey.ascx.cs" Inherits="CheckboxWeb.Libraries.Controls.ItemListFromSurvey" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register TagPrefix="grid" Namespace="Checkbox.Web.UI.Controls.GridTemplates" Assembly="Checkbox.Web" %>

<script type="text/javascript">
    //Select/Deselect all checkboxes
    function selectAllChangedFromSurvey(selectAllItem) {
        if ($(selectAllItem).prop("checked")) {
            $('.<%=CustomCheckBoxField.CheckboxesCss%> input').prop('checked', true);
            $('.<%=CustomCheckBoxField.CheckboxesCss%> span').addClass('checked');
        } 
        else {
            $('.<%=CustomCheckBoxField.CheckboxesCss%> input').prop('checked', false);
            $('.<%=CustomCheckBoxField.CheckboxesCss%> span').removeClass('checked');
        }
        $.uniform.update('.<%=CustomCheckBoxField.CheckboxesCss%> input');
    }

    //Click Preview handle
    function onPreviewClick(itemId) {
        showDialog('<%=ResolveUrl("~/") %>Libraries/ViewItem.aspx?i=' + itemId, 'properties');
    }
</script>

<asp:Panel ID="_panelSurveysNotFound" runat="server" CssClass="warning message">
    <%=WebTextManager.GetText("/controlText/libraries/controls/ItemListFromSurvey.ascx/noSurveysFound")%>
</asp:Panel>

<asp:Panel ID="_panelSurveys" CssClass="importToLibrariesSourcePanel padding10" runat="server">
    <div style="overflow-y: auto; overflow-x: hidden; height:430px;">
        <asp:GridView ID="_surveyList" runat="server" AutoGenerateColumns="false" CssClass="DefaultGrid border999 shadow999" Width="100%" OnRowCommand="_surveyList_RowCommand">
            <HeaderStyle CssClass="HeaderRow" />
            <RowStyle CssClass="EvenRow" />
            <AlternatingRowStyle CssClass="OddRow" />
            <Columns>
                <asp:BoundField DataField="SurveyID" Visible="false"/>                
                <grid:LocalizedHeaderBoundField HeaderTextID="/controlText/libraries/controls/ItemListFromSurvey.ascx/surveyName" DataField="SurveyName" ItemStyle-Width="720px" />
                <grid:LocalizedHeaderTemplateField HeaderTextID="/controlText/libraries/controls/ItemListFromSurvey.ascx/view" ItemStyle-Width="60px" ItemStyle-HorizontalAlign="center" HeaderStyle-HorizontalAlign="center">
                    <ItemTemplate>
                        <asp:ImageButton 
                            id="_viewBtn"
                            runat="server"
                            ImageUrl="~/App_Themes/CheckboxTheme/Images/survey24.gif"                        
                            CommandName="ViewSurvey"
                            CommandArgument='<%#Eval("SurveyID") %>' />
                    </ItemTemplate>
                </grid:LocalizedHeaderTemplateField>
            </Columns>
        </asp:GridView>
        <div class="centerContent" style="margin-bottom:5px;width:180px;">
            <ckbx:PagingControl ID="_surveyPager" runat="server" OnOnPagingChanged="_surveyPager_OnPagingChanged"/>
        </div>
    </div>
</asp:Panel>

<asp:Panel ID="_panelEmptyItemList" runat="server" Visible="false">
    <a id="_toSurveySelectionLink" runat="server" class="ckbxButton roundedCorners border999 shadow999 silverButton" OnServerClick="_toSurveySelectionLink_Click"><%=WebTextManager.GetText("/common/back")%></a>
    <div class="error message">
        <%=WebTextManager.GetText("/controlText/libraries/controls/ItemListFromSurvey.ascx/noItemsFound")%>
    </div>
</asp:Panel>

<asp:Panel ID="_panelItemList" runat="server" Visible="false">
    <div class="spacing">
        <a id="_toSurveySelection" runat="server" class="ckbxButton roundedCorners border999 shadow999 silverButton" OnServerClick="_toSurveySelectionLink_Click"><%=WebTextManager.GetText("/common/back")%></a>
    </div>
    <div style="overflow-y: auto; overflow-x: hidden; height:400px;">
        <asp:GridView ID="_itemList" runat="server" AutoGenerateColumns="false" CssClass="DefaultGrid border999 shadow999" Width="100%">
            <HeaderStyle CssClass="HeaderRow" />
            <RowStyle CssClass="EvenRow" />
            <AlternatingRowStyle CssClass="OddRow" />
            <Columns>
                <asp:BoundField DataField="ItemID" Visible="false"/>
                <grid:CustomCheckboxField HeaderTextID="blah/controlText/libraries/controls/ItemListFromSurvey.ascx/select" AllowSelectAll="true" OnClientClick="selectAllChangedFromSurvey" DataField="Checked" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center" />
                <grid:ItemTextField DataField="ItemID" HeaderTextID="/controlText/libraries/controls/ItemListFromSurvey.ascx/itemText" ItemStyle-Width="520px"/>
                <grid:LocalizedHeaderTemplateField HeaderTextID="/controlText/libraries/controls/ItemListFromSurvey.ascx/itemType" ItemStyle-Width="120px">
                    <ItemTemplate>
                        <asp:Label
                            id="_typeLbl"
                            runat="server"
                            CssClass="PrezzaNormal"
                            Text='<%# WebTextManager.GetText("/itemType/" + Eval("ItemType") + "/name") %>'
                        />
                    </ItemTemplate>
                </grid:LocalizedHeaderTemplateField>
                <grid:LocalizedHeaderTemplateField HeaderTextID="/controlText/libraries/controls/ItemListFromSurvey.ascx/view" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="center" HeaderStyle-HorizontalAlign="center">
                    <ItemTemplate>
                        <asp:ImageButton 
                            id="_viewBtn"
                            runat="server"
                            ImageUrl="~/App_Themes/CheckboxTheme/Images/button-details-sq.gif"                        
                            OnClientClick='<%#  "onPreviewClick(" +Eval("ItemID") +"); return false;"%>'
                            ToolTip='<%# WebTextManager.GetText("/common/newWindow") %>'
                        />
                    </ItemTemplate>
                </grid:LocalizedHeaderTemplateField>
            </Columns>
        </asp:GridView>
        <div class="centerContent" style="margin-bottom:5px;width:180px;">
            <ckbx:PagingControl ID="_itemPager" runat="server" OnOnPagingChanged="_pager_OnPagingChanged"/>
        </div>
    </div>
</asp:Panel>
