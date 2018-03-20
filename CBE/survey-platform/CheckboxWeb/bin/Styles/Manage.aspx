<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Manage.aspx.cs" Inherits="CheckboxWeb.Styles.Manage" MasterPageFile="~/DetailList.master" IncludeJsLocalization="true" %>
<%@ Register TagPrefix="ckbx" TagName="StyleList" Src="~/Styles/Controls/StyleList.ascx" %>
<%@ Register Src="~/Styles/Controls/StyleDashboard.ascx" TagName="StyleDashboard" TagPrefix="ckbx" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ MasterType VirtualPath="~/DetailList.master" %>

<asp:Content ContentPlaceHolderID="_head" runat="server" ID="_headContent">
    <ckbx:ResolvingScriptElement runat="server" Source="../Resources/styles/manage.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="../Resources/StatusControl.js" />
    
    <script type="text/javascript">
        <%-- Ensure statusControl initialized --%>
        $(document).ready(function(){
            statusControl.initialize('_statusPanel');
            $('#<%=_titleActions.ClientID %>').val('<%=StyleTypeEncoded %>');
        });

        //Load style dashboard
        function onFormStyleSelected(style) {
            loadStyleData(style.Id, 'form');
            $('.gridContent').removeClass('gridActive');
            $('#style_' + style.Id).addClass('gridActive');
        }

        //Load group dashboard
        function onChartStyleSelected(style) {
            //Load group
            loadStyleData(style.Id, 'chart');
            $('.gridContent').removeClass('gridActive');
            $('#style_' + style.Id).addClass('gridActive');
        }

        <%-- Handle Survey Click --%>
        function onStyleClick(arg) {
            var styleId = arg.attr('styleId');
            var type = arg.attr('styleType');

            if (styleId == null || styleId == '') {
                return;
            }

            <%-- Call load js method exposed by Dashboard control --%>
            loadStyleData(styleId, type);
        }

        //Handler for OnStyleDeleted event
        function styleDeletedHandler(){
            //call the methods exposed by Dashboard control
            reloadStyleList(true); 
            cleanStyleDashboard();
        }

        <%-- Show status message --%>
        function ShowStatusMessage(message, isSucceeded){
            statusControl.showStatusMessage(message, isSucceeded ? StatusMessageType.success : StatusMessageType.error);
        }

       function loadGrid(gridType) {
            document.location = "Manage.aspx?t=" + gridType;
        }

       function doCopy(style){
        var r=confirm("Are you sure to copy this style?");
        if (r==true)
        {
            UFrameManager.prepareOuterFormSubmit();
            __doPostBack('_copyLink', style);
        }
    }
    </script>
</asp:Content>

<asp:Content ID="_title" runat="server" ContentPlaceHolderID="_titlePlace">
    <ul id="_titleActions" runat="server" class="tabsMenu" style="display: none; font-size:22px;"></ul>
</asp:Content>

<asp:Content ID="_titleLinks" runat="server" ContentPlaceHolderID="_titleLinks">
    <div class="styles-manage-buttons">
        <a id="stylemanager_addstyle" class="header-button ckbxButton blueButton" href="javascript:showDialog('Forms/Create.aspx?t=<%=StyleTypeEncoded%>', 'properties');"><%=WebTextManager.GetText("/pageText/styles/manage.aspx/createStyle")%></a>
        <div id="stylemanager_addstyle_menu" class="groupMenu" style="display: none;">              
            <ul class="allMenu">
                <li><a class="ckbxButton blueButton" id="_buttonImportSurveyStyle" href="javascript:showDialog('Forms/Import.aspx', 'properties');"><%=WebTextManager.GetText("/pageText/styles/manage.aspx/importStyle")%></a></li>
            </ul>
        </div>
        
        <% if (false)  { %>><a class="ckbxButton silverButton" id="_buttonCreateChartStyle" href="javascript:showDialog('Charts/Create.aspx', 'properties');"><%=WebTextManager.GetText("/pageText/styles/manage.aspx/createChart")%></a> <%} %>
    </div>
</asp:Content>

<asp:Content ID="_leftContent" runat="server" ContentPlaceHolderID="_leftContent">
    <ckbx:StyleList ID="_styleList" runat="server" StyleSelectedClientCallback ="onFormStyleSelected" OnStyleDeleted="styleDeletedHandler" ShowStatusMessageHandler="ShowStatusMessage" />
</asp:Content>

<asp:Content ID="_rightContent" runat="server" ContentPlaceHolderID="_rightContent">
    <ckbx:StyleDashboard ID="_dashboard" runat="server" OnStyleDeleted="styleDeletedHandler" ShowStatusMessageHandler="ShowStatusMessage" />
</asp:Content>