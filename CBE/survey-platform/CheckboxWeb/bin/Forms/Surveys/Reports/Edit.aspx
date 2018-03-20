<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Edit.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Edit"  MasterPageFile="~/DetailList.Master" IncludeJsLocalization="true" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ MasterType VirtualPath="~/DetailList.Master" %>
<%-- Script Content --%>
<asp:Content ID="_scriptContent" runat="server" ContentPlaceHolderID="_head">
    <ckbx:ResolvingScriptElement runat="server" Source="../../../Resources/surveys/reports/edit.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="../../../Resources/jquery.truncate.js" />
    
    <script type="text/javascript">

        <%-- Id of language select container --%>
        var languageListId = '<%= _languageList.ClientID %>';
        var _currentFilterId = null;

        <%--
            Init
         --%>
        $(document).ready(function () {
            <%-- Initialize Report Editor Obj --%>
            reportEditor.initialize(
                _at,
                '<%=ResolveUrl("~/") %>',
                <%= ReportId %>,
                '<%= LanguageCode %>',
                'ReportEditor',
                new Array({name: 'r', value: <%=ReportId %>}),
                'itemEditorContainer',
                'itemTitle',
                'itemType',
                'itemId',
                'detailContent',
                'detailWelcome',
                'detailProgressContainer',
                'detailMenuContainer'
            );

            reportEditor.loadNavigation('reportNavigatorPlace');

            //Bind language change
            $('#' + languageListId).change(onLanguageChange);
            
            $('#' + languageListId).uniform();

            statusControl.initialize('_statusPanel');

            $('#report_filters_button').on('click', function() {
                $('#detailContent').hide();
                $('#settingsContent').hide();
                $('#filtersContent').show();
            });

            $('#report_settings_button').on('click', function() {
                $('#detailContent').hide();
                $('#filtersContent').hide();
                $('#settingsContent').show();
            });
        });

        //Load filter on selected
        function onFilterSelected(filter) {
            _currentFilterId = filter.FilterId;
       //     $("#<%=Master.RightPaneClientId %>").hide();
            showDialog('<%=ResolveUrl("~/") %>Forms/Surveys/Reports/Filters/Edit.aspx?s=<%=ResponseTemplateId %>&filterId=' + _currentFilterId + '&onClose=onFilterDialogClosed', 450, 600);
        }

        /////////////////////////////////////////////////////////////////////
        // Handle language change
        function onLanguageChange(){
            reportEditor.onLanguageChange($('#' + languageListId).val());
        }

        function deletePage(id){
            var r=confirm("Are you sure to delete this report page?");
            if (r==true)
            {
                reportEditor.deletePage('', id, onReportPageDeleted, null);
            }
        }

        function onReportPageDeleted(args) {
            templateEditor.refreshDisplay(null);            
        }  
        
        //Handle dialog close and reload filters list
        function onFilterDialogClosed(arg) {
            if (arg == null) {
                return;
            }
            
            //Figure out operation performed
            if (arg.op == null
                || arg.op == '') {
                return;
            }

            if(arg.op == 'addFilter'
                && arg.result == 'ok'){
                _currentFilterId = arg.filterId;
                reloadFilterList();
            }
            
            if (arg.op == 'editFilter') {
                reloadFilterList();
            }
        }
    </script>
</asp:Content>

<%-- Language Select --%>
<asp:Content ID="_titleButtons" runat="server" ContentPlaceHolderID="_titleLinks">
    <div class="reports-edit-buttons">
        <a class="header-button ckbxButton blueButton statistics_AddPage" href="javascript:templateEditor.addPage();"><%=WebTextManager.GetText("/pageText/formEditor.aspx/newPage")%></a>
        <a class="header-button ckbxButton blueButton statistics_ExpandPages expand-up-angle-arrow" style="left: 180px;">Collapse All</a>
        <a class="header-button ckbxButton blueButton" href="<%=ResolveUrl("~/RunAnalysis.aspx?aid=") + ReportId.Value %>" target="_blank"><%=WebTextManager.GetText("/pageText/editAnalysis.aspx/runAnalysis") %></a>
    </div>
    <asp:DropDownList ID="_languageList" runat="server" CssClass="right"></asp:DropDownList>
</asp:Content>

<%-- Left Content --%>
<asp:Content ID="_listContent" runat="server" ContentPlaceHolderID="_leftContent">
    <div id="reportNavigatorPlace"></div>
</asp:Content>

<%-- Right Content --%>
<asp:Content ID="_detailContent" runat="server" ContentPlaceHolderID="_rightContent">
    <!-- Report Header -->
    <div class="survey-header-container clearfix" style="margin:-10px 0 0 5px;">
        <div class="header-content">
            <a id="report_editor_actions_button"  class="action-menu-toggle action-button ckbxButton silverButton" href="#"><span class="toggle-arrow"></span>Report Actions</a>
            <a protectPermission="Analysis.Administer" id="report_settings_button" class="action-menu-toggle action-button ckbxButton silverButton" href="#" ><span class="toggle-arrow"></span>Settings</a>
            <a protectPermission="Analysis.Administer" id="report_sharing_button" class="action-share action-menu-toggle action-button ckbxButton silverButton" href="#"><span class="toggle-arrow"></span>Sharing</a>
            <a protectPermission="Analysis.ManageFilters" id="report_filters_button" class="action-menu-toggle action-button ckbxButton silverButton" href="#"><span class="toggle-arrow"></span>Filters</a>
            
            <div id="report_sharing_menu" class="groupMenu glimpse-options-menu">
                <ul class="itemActionMenu">
                    <li><a class="ckbxButton statistics_TwitterShare" target="_blank" href="<%= TwitterUrl %>">Twitter</a></li>
                    <% if (ShowFacebook) { %>
                            <li><a class="ckbxButton" href="javascript:showDialog('../FacebookMenu.aspx?r=<%= ReportId.HasValue ? ReportId.Value.ToString() : string.Empty %>', 520, 650)">Facebook</a></li>
                    <% } %>
                    <li><a class="ckbxButton" href="javascript:showDialog('../EmbedMenu.aspx?r=<%= ReportId.HasValue ? ReportId.Value.ToString() : string.Empty %>', 450, 650)">Link or Embed</a></li>
                    <li><a protectPermission="Analysis.Run" class="ckbxButton" href="javascript:showDialog('Export.aspx?r=<%= ReportId.Value %>&onClose=onDialogClosed', 'properties')" ><%=WebTextManager.GetText("/pageText/editAnalysis.aspx/pdfPrint")%> </a></li>
                </ul>
            </div>

            <div id="report_actions_menu" class="groupMenu glimpse-options-menu">
                <ul class="itemActionMenu">
                    <li><a protectPermission="Analysis.Run" class="ckbxButton" target="_blank" href="<%= RunReportUrl %>"><%=WebTextManager.GetText("/pageText/editAnalysis.aspx/run")%></a></li>
                    <li><a protectPermission="Analysis.Administer" class="ckbxButton" href="javascript:showDialog('Copy.aspx?r=<%= ReportId.Value %>&onClose=onDialogClosed', 'properties')" ><%=WebTextManager.GetText("/pageText/editAnalysis.aspx/copy")%> </a></li>
                    <li><a protectPermission="Analysis.Run" class="ckbxButton" href="javascript:showDialog('Export.aspx?r=<%= ReportId.Value %>&onClose=onDialogClosed', 'properties')" ><%=WebTextManager.GetText("/pageText/editAnalysis.aspx/pdfPrint")%> </a></li>
                </ul>
            </div>   

            <div id="report_filters_menu" class="groupMenu glimpse-options-menu">
                <ul class="itemActionMenu">
                    <li><a class="ckbxButton" href="javascript:showDialog('<%= ResolveUrl("~/Forms/Surveys/Reports/ReportFilters.aspx?r=" + ReportId ) %>', 600, 800);">Apply Filters</a></li>
                    <li><a class="ckbxButton" href="javascript:showDialog('<%= ResolveUrl("~/Forms/Surveys/Reports/SetDateFilter.aspx?r=" + ReportId ) %>', 570, 800);">Set Date Filter</a></li>
                    <li><a class="ckbxButton" href="javascript:showDialog('<%= ResolveUrl("~/Forms/Surveys/Reports/Filters/Add.aspx?s=" + ResponseTemplate.ID) %>', 520, 650);" >+ Filter</a></li>
                    <li><a class="ckbxButton" target="_blank" href="<%= ResolveUrl("~/Forms/Surveys/Reports/Filters/Manage.aspx?s=" + ResponseTemplate.ID ) %>" >Manage Filters</a></li>
                </ul>
            </div>   

            <div id="report_settings_menu" class="groupMenu glimpse-options-menu">
                <ul class="itemActionMenu">
                    <li><a class="ckbxButton" href="javascript:showDialog('<%= ResolveUrl("~/Forms/Surveys/Reports/Properties.aspx?ag=" + ReportTemplate.Guid) %>', 450, 600);" >Appearance</a></li>
                    <li><a class="ckbxButton" href="javascript:showDialog('<%= ResolveUrl("~/Forms/Surveys/Reports/Permissions.aspx?r=" + ReportId ) %>', 620, 800);">Permissions</a></li>
                    <li><a class="ckbxButton" href="javascript:showDialog('<%= ResolveUrl("~/Forms/Surveys/Reports/ResponseSettings.aspx?ag=" + ReportTemplate.Guid ) %>', 450, 600);">Responses</a></li>
                </ul>
            </div>   
        </div>
    </div>
    
    <div id="detailProgressContainer" style="display: none;">
        <div id="detailProgress" style="text-align: center;">
            <p><%=WebTextManager.GetText("/common/loading") %></p>
            <p>
                <asp:Image ID="_progressSpinner" runat="server" SkinID="ProgressSpinner" />
            </p>
        </div>
    </div>
    <div id="detailContentContainer" class="padding10">
        <div id="detailWelcome" class="introPage">
           
        </div>
        <div id="detailContent" style="display:none;">
            <div class="survey-header-container">
                <div class="header-content clearfix">
                    <a id="reportitem_actions_button" href="#" class="action-menu-toggle action-button">Item Actions</a>
                    <div id="reportitem_actions_menu" class="groupMenu">
                        <ul class="itemActionMenu">
                            <li><a href="#" class="groupMenuToggle action-menu-toggle">Item Actions</a></li>
                            <!--<li><a class="ckbxButton roundedCorners border999 shadow999 silverButton" href="javascript:void(0);" id="exportItemLink"><%=WebTextManager.GetText("/pageText/formEditor.aspx/export")%></a></li>-->
                            <li><a class="ckbxButton roundedCorners border999 shadow999 silverButton" href="javascript:void(0);" id="moveCopyItemLink"><%=WebTextManager.GetText("/pageText/formEditor.aspx/moveCopy")%></a></li>
                            <li><a class="ckbxButton roundedCorners border999 shadow999 redButton" href="javascript:void(0);" id="deleteItemLink"><%=WebTextManager.GetText("/pageText/formEditor.aspx/delete")%></a></li>
                        </ul>    
                    </div>
                    <h3 id="itemTitle"></h3>
                </div>
            </div>
            <div id="itemEditorContainer" style="margin-top:10px;"></div>
            <div id="detailFooter"></div>
        </div>
    </div>
    
</asp:Content>
