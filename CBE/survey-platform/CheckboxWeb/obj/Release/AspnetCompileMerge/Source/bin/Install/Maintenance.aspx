<%@ Page Title="" Language="C#" MasterPageFile="~/Install/Install.Master" AutoEventWireup="false" CodeBehind="Maintenance.aspx.cs" Inherits="CheckboxWeb.Install.Maintenance" %>
<%@ MasterType VirtualPath="~/Install/Install.Master" %>
<%@ Register Src="~/Install/Controls/SLA.ascx" TagName="SLA" TagPrefix="ckbx" %>

<asp:Content ID="scripts" runat="server" ContentPlaceHolderID="scriptContent">
    <ckbx:ResolvingScriptElement runat="server" Source="../Resources/install/maintenance.js" />

    <script type="text/javascript">
        var _isSlaAccepted = false;

        function OnSlaDialogClose(patchPath) {
            _isSlaAccepted = validateSLA();
            if (_isSlaAccepted)
                setTimeout(function () { loadPatchInfo(patchPath); }, 500);
        }

        function loadPatchInfo(patchPath) {
            if (_isSlaAccepted)
                showDialog(patchPath, 450, 600);
            else
                showDialog('slaModal', 220, 620, function () { OnSlaDialogClose(patchPath); });
        }

        function loadRightPanel(panelUrl) {
            UFrameManager.init({
                id: 'rightPanelViewport',
                loadFrom: panelUrl,
                progressTemplate: $('#detailProgressContainerSimple').html(),
                showProgress: true
            });
            $('.rightPanel').show();
        }
    </script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlace" runat="server">
    <div class="leftPanel" style="float: left;">
        <h3 class="pageTitle" style="padding: 15px 0 0 20px">Checkbox&reg; 6 Maintenance</h3>
        <div class="left-header clearfix">
            <div class="header-actions"></div>
        </div>
        <div class="viewport">
            <div class="overview">
                <div>
                    <div class="dialogSubTitle sectionTitle">
                        <asp:Label ID="_PatchesTitle" runat="server" meta:resourcekey="_PatchesTitle">Patches</asp:Label>
                    </div>
                    <div class="dashStatsWrapper border999 shadow999">
                        <div class="dashStatsHeader">
                            <asp:Label ID="_installedPatchesTitle" runat="server" CssClass="mainStats left" meta:resourcekey="_installedPatchesTitle">Patches</asp:Label>
                        </div>
                        <asp:GridView runat="server" ID="_installedPatchesGrid" CellPadding="4" CellSpacing="4" AutoGenerateColumns="false" border="0" Width="100%" BorderStyle="None" ShowHeader="false">
                            <AlternatingRowStyle CssClass="dashStatsContent detailZebra" />
                            <RowStyle CssClass="dashStatsContent zebra" />
                            <Columns>
                                <asp:BoundField HeaderText="Patch" DataField="Name" />
                                <asp:BoundField HeaderText="Description" DataField="Description" />
                                <asp:HyperLinkField HeaderText="Install" DataNavigateUrlFields="SetupURL" />
                            </Columns>
                            <EmptyDataTemplate>
                                <div class="padding10">No patches have been installed.</div>
                            </EmptyDataTemplate>
                        </asp:GridView>
                    </div>
                    <div class="dashStatsWrapper border999 shadow999">
                        <div class="dashStatsHeader">
                            <asp:Label ID="_availablePatchesTitle" runat="server" CssClass="mainStats left" meta:resourcekey="_availablePatchesTitle">Patches</asp:Label>
                        </div>
                        <asp:GridView ShowHeader="false" runat="server" ID="_availablePatchesGrid" BorderStyle="None" CellPadding="4" CellSpacing="1" AutoGenerateColumns="false" Border="0" Width="100%">
                            <AlternatingRowStyle CssClass="dashStatsContent detailZebra" />
                            <RowStyle CssClass="dashStatsContent zebra" />
                            <HeaderStyle CssClass="" HorizontalAlign="Left" VerticalAlign="Middle"></HeaderStyle>
                            <EmptyDataTemplate>
                                <div class="padding10">No patches are available.</div>
                            </EmptyDataTemplate>
                            <Columns>
                                <asp:BoundField HeaderText="Patch" DataField="Name" />
                                <asp:TemplateField HeaderText="Release Notes">
                                    <ItemTemplate>
                                        <a class="ckbxButton roundedCorners shadow999 border999 silverButton smallButton" target="_blank" href="http://www.checkbox.com/category/release-notes/">View Release Notes</a>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Install">
                                    <ItemTemplate>
                                        <a class="ckbxButton roundedCorners shadow999 border999 redButton smallButton" href="javascript:loadPatchInfo('Patches/<%# DataBinder.Eval(Container.DataItem, "Version") %>/Setup.aspx');">Load Patch Information</a>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="rightPanel left introPage">
        <span class="openLeftPanel">Open Left Panel</span>
        <div class="scrollbar">
            <div class="track">
                <div class="thumb">
                    <div class="end"></div>
                </div>
            </div>
        </div>
        <div class="viewport" id="rightPanelViewport">
            <div class="overview">
                <div class="dashboard">
                    <div class="dashPadding">
                        <p>
                            <asp:Label ID="_patchInstructions" runat="server" meta:resourcekey="_patchInstructions">Patches are intended to add additional core application functionality and fix issues that may exist.  To install a patch, click the Install link in the list of available patches.  Patches are designed to be cumulative, so if there are multiple available patches, install only the one with the highest version number.</asp:Label>
                        </p>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <%-- Loading Div for Simple --%>
    <div id="detailProgressContainerSimple" style="display: none;">
        <div style="text-align: center;">
            <p>Loading...</p>
            <p>
                <asp:Image ID="_progressSpinnerSimple" runat="server" SkinID="ProgressSpinner" />
            </p>
        </div>
    </div>

    <div id="slaModal" style="display:none;">
        <ckbx:SLA ID="_sla" runat="server" />
        <a class="ckbxButton roundedCorners border999 shadow999 orangeButton sla-ok" href="javascript:closeWindow();" style="margin-left:15px; display: none;">Agree and Continue</a>
    </div>                
</asp:Content>