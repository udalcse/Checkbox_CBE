<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Manage.aspx.cs" Inherits="CheckboxWeb.Libraries.Manage" MasterPageFile="~/DetailList.Master" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ MasterType VirtualPath="~/DetailList.Master" %>
<%@ Register TagPrefix="ckbx" TagName="LibraryList" Src="~/Libraries/Controls/LibraryList.ascx" %>
<%@ Register TagPrefix="ckbx" TagName="LibraryPreview" Src="~/Libraries/Controls/LibraryPreview.ascx" %>

<asp:Content ID="head" ContentPlaceHolderID="_head" runat="server">
    <%-- Global Survey Stylesheets --%>
    <ckbx:ResolvingCssElement runat="server" media="screen" Source="~/GlobalSurveyStyles.css" />
    <ckbx:ResolvingCssElement runat="server" media="screen" Source="~/ScreenSurveyStyles.css" />

    <ckbx:ResolvingScriptElement runat="server" Source="../Resources/globalHelper.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="../Resources/StatusControl.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="../Resources/libraries/manage.js" />
    
    <script type="text/javascript">
        var _currentLibraryId = null;

        $(document).ready(function(){

             <% if(InitialLibraryId.HasValue) { %>
                _currentLibraryId = <%= InitialLibraryId %>;
                reloadCurrentLibrary(true);
             <% } %>

             statusControl.initialize('_statusPanel');
        });

        function reloadLibrary() {
                reloadCurrentLibrary(true);
        }

        //Load library on selected
        function onLibrarySelected(library) {
            $('#introTxt').hide();
            _currentLibraryId = library.DatabaseId;
            loadLibraryPreview(library);
            $('.gridContent').removeClass('gridActive');
            $('#libraryName_' + library.DatabaseId).parent().addClass('gridActive');
        }

        //Handle dialog close and reload survey dashboard
        function onDialogClosed(arg) {
            if(arg == null) {
                return;
            }

            //Figure out operation performed
            if (arg.op == null
                || arg.op == '') {
                return;
            }
            
            //Reload properties
            if (arg.op == 'properties') {
                reloadLibraryList();
                reloadCurrentLibrary(false);
            }

            if(arg.op == 'addItem'
                && arg.result == 'ok'){
                reloadCurrentLibrary(true);
            }

            if(arg.op == 'editItem'
                && arg.result == 'ok'
                && arg.libraryId != null
                && arg.itemId != null){
                <%-- Defined in LibraryPreview.ascx --%>
                loadItemPreview(arg.libraryId, arg.itemId);
            }
        }

        //Reload current library
        function reloadCurrentLibrary(reloadPreview) {
            $('#introTxt').hide();
            svcSurveyManagement.getLibraryData(_at, _currentLibraryId, onLibraryDataLoaded, {reloadPreview:reloadPreview});
        }

        //Update grid and preview
        function onLibraryDataLoaded(libraryData, args) {
            <%-- Method defined in LibraryList.ascx --%>
            updateLibraryRow(libraryData);

            <%-- Method Default in LibraryPreview.ascx --%>
            updateLibraryName(libraryData);

            if(args && args.reloadPreview){
                loadLibraryPreview(libraryData);
            }
        }
        function doExport(){
            UFrameManager.prepareOuterFormSubmit();
            __doPostBack('_exportLink',_currentLibraryId);
        }

        function OnLibraryDeleted() {
            $('#introTxt').show();
            $('#previewHeader').hide();
            $('#previewContainer').hide();
        }

        <%-- Show status message --%>
        function ShowStatusMessage(message, isSucceeded){
            statusControl.showStatusMessage(message, isSucceeded? StatusMessageType.success : StatusMessageType.error);
        }
    </script>
</asp:Content>

<asp:Content ID="_titleLinks" runat="server" ContentPlaceHolderID="_titleLinks">
    <div class="libraries-manage-buttons">
        <a id="librarymanager_addlibrary" class="header-button ckbxButton blueButton" href="javascript:showDialog('Create.aspx', 'properties');"><%=WebTextManager.GetText("/pageText/libraries/manage.aspx/createLibrary")%></a>
        <div id="librarymanager_addlibrary_menu" class="groupMenu" style="display: none;">              
            <ul class="allMenu">
                <li><a class="ckbxButton blueButton" id="_buttonImportLibrary" href="javascript:showDialog('Import.aspx?onClose=onDialogClosed');"><%=WebTextManager.GetText("/pageText/libraries/manage.aspx/importLibrary")%></a></li>
            </ul>
        </div>
    </div>
    
</asp:Content>

<asp:Content ID="left" ContentPlaceHolderID="_leftContent" runat="server">
    <ckbx:LibraryList ID="_libraryList" runat="server" LibrarySelectedClientCallback="onLibrarySelected" OnLibraryDeleted="OnLibraryDeleted" ShowStatusMessageHandler="ShowStatusMessage" />
</asp:Content>

<asp:Content ID="right" ContentPlaceHolderID="_rightContent" runat="server">
    <div class="padding10 dashboard" id="introTxt">
        <div class="introPage">
            
        </div>        
    </div>
    <ckbx:LibraryPreview ID="_libraryPreview" runat="server" ShowStatusMessageHandler="ShowStatusMessage"/>
</asp:Content>