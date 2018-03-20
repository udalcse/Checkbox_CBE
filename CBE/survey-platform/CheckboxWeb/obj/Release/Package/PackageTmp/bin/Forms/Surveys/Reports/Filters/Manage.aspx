<%@ Page Title="" Language="C#" MasterPageFile="~/DetailList.Master" Theme="CheckboxTheme" AutoEventWireup="false" CodeBehind="Manage.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Filters.Manage" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ MasterType VirtualPath="~/DetailList.Master" %>
<%@ Register TagPrefix="ckbx" TagName="ReportFilterList" Src="~/Forms/Surveys/Reports/Filters/Controls/FilterList.ascx" %>

<asp:Content ID="head" ContentPlaceHolderID="_head" runat="server">
    <ckbx:ResolvingScriptElement runat="server" Source="../../../../Resources/surveys/reports/filters/manage.js" />
    
    <script type="text/javascript">
        var _currentFilterId = null;
        
        //Load filter on selected
        function onFilterSelected(filter) {
            _currentFilterId = filter.FilterId;
            $("#<%=Master.RightPaneClientId %>").hide();
            showDialog('<%=ResolveUrl("~/") %>Forms/Surveys/Reports/Filters/Edit.aspx?s=<%=SurveyId %>&filterId=' + _currentFilterId + '&onClose=onDialogClosed', 600, 800);
        }

        //Handle dialog close and reload filters list
        function onDialogClosed(arg) {
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

        //Update grid and preview
        function onFilterDataLoaded(filterData, args) {
            
            <%-- Method defined in FilterList.ascx --%>
            updateFilterRow(filterData);

            <%-- Method Default in FilterPreview.ascx --%>
            updateFilterName(filterData);
        }

        function OnFilterDeleted() {
            $('#introTxt').show();
            $('#previewHeader').hide();
        }
    </script>
</asp:Content>

<asp:Content ID="_titleLinks" runat="server" ContentPlaceHolderID="_titleLinks">
    <a class="ckbxButton silverButton" runat="server" id="_buttonAddFilter"><%=WebTextManager.GetText("/pageText/forms/surveys/reports/filters/add.aspx/addFilter")%></a>
</asp:Content>

<asp:Content ID="left" ContentPlaceHolderID="_leftContent" runat="server">
    <ckbx:ReportFilterList ID="_filterList" runat="server" FilterSelectedClientCallback="onFilterSelected" />
</asp:Content>
<asp:Content ID="right" ContentPlaceHolderID="_rightContent" runat="server" >
    
</asp:Content>
