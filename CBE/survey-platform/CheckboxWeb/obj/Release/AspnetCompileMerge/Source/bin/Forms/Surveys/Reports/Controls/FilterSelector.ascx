<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="FilterSelector.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.FilterSelector" %>
<%@ Import Namespace="Checkbox.Common"%>
<%@ Import Namespace="Checkbox.Analytics.Filters.Configuration"%>

    <div class="fixed_350 height_460 left" style="margin-left:10px;">
        <asp:Panel ID="_availableFiltersPanel" runat="server" CssClass="dashStatsWrapper border999 shadow999" style="max-height:400px;overflow-y:auto;">
            <div class="dashStatsHeader">
                <ckbx:MultiLanguageLabel ID="_availableFiltersLbl" runat="server" CssClass="mainStats left" TextId="/pageText/filterSelector.aspx/availableFilters" />
            </div>
            <ckbx:MultiLanguageLabel ID="_noAvailableFiltersLbl" runat="server" CssClass="dialogInstructions dashStatsContent" Style="display:block;" TextId="/pageText/filterSelector.aspx/noFiltersAvailable" Visible="false" />
            <asp:ListView ID="_availableFiltersView" runat="server" ItemPlaceholderID="_filterPlaceholder">
                <LayoutTemplate>
                    <asp:Panel ID="_filterPlaceholder" runat="server" style="height:250px;overflow-y:scroll;">&nbsp;</asp:Panel>
                </LayoutTemplate>
                <ItemTemplate>
                    <div class="dashStatsContent zebra">
                        <div class="left fixed_25">
                            <asp:CheckBox ID="_addChk" runat="server" />
                        </div>
                         <div class="left fixed_250">
                            <%# Utilities.StripHtml(((FilterData)Container.DataItem).ToString(LanguageCode), 128) %>
                        </div>
                        <br class="clear" />
                    </div>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <div class="dashStatsContent zebra">
                        <div class="left fixed_25">
                            <asp:CheckBox ID="_addChk" runat="server" />
                        </div>
                         <div class="left fixed_250">
                            <%# Utilities.StripHtml(((FilterData)Container.DataItem).ToString(LanguageCode), 128) %>
                        </div>
                        <br class="clear" />
                    </div>
                </AlternatingItemTemplate>
            </asp:ListView>
        </asp:Panel>
        <btn:CheckboxButton ID="_addBtn" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 silverButton right" TextID="/pageText/filterSelector.aspx/applyFilters" Visible="false"/>
        <br class="clear" />
    </div>

    <div class="fixed_350 height_460 left" style="margin-left:50px;">
        <asp:Panel ID="_sourceItemsContainer" runat="server" CssClass="dashStatsWrapper border999 shadow999" style="max-height:400px;overflow-y:auto;">
            <div class="dashStatsHeader">
                <span class="left mainStats"><ckbx:MultiLanguageLabel ID="_appliedFiltersLbl" runat="server" CssClass="" TextId="/pageText/filterSelector.aspx/associatedFilters" /></span>
            </div>
            <ckbx:MultiLanguageLabel ID="_noAppliedFiltersLbl" runat="server" CssClass="dialogInstructions dashStatsContent" Style="display:block;" TextId="/pageText/filterSelector.aspx/noFiltersApplied" Visible="false"/>
            <asp:ListView ID="_appliedFiltersView" runat="server" ItemPlaceholderID="_filterPlaceholder">
                <LayoutTemplate>
                    <asp:Panel ID="_filterPlaceholder" runat="server" style="height:250px;overflow-y:scroll;">&nbsp;</asp:Panel>
                </LayoutTemplate>
                <ItemTemplate>
                    <div class="dashStatsContent zebra">
                        <div class="left fixed_25">
                            <asp:CheckBox ID="_removeChk" runat="server" />
                        </div>
                         <div class="left fixed_250">
                            <%# Utilities.StripHtml(((FilterData)Container.DataItem).ToString(LanguageCode), 128) %>
                        </div>
                        <br class="clear" />
                    </div>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <div class="dashStatsContent detailZebra">
                        <div class="left fixed_25">
                            <asp:CheckBox ID="_removeChk" runat="server" />
                        </div>
                         <div class="left fixed_250">
                            <%# Utilities.StripHtml(((FilterData)Container.DataItem).ToString(LanguageCode), 128) %>
                        </div>
                        <br class="clear" />
                    </div>
                </AlternatingItemTemplate>
            </asp:ListView>
        </asp:Panel>
        <btn:CheckboxButton ID="_removeBtn" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 redButton right" TextID="/pageText/filterSelector.aspx/removeFilters" Visible="false"/>
        <br class="clear" />
    </div>
    <br class="clear" />
