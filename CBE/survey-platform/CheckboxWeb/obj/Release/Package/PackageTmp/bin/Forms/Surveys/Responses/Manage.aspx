<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Manage.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Responses.Manage" MasterPageFile="~/DetailList.Master" %>
<%@ Register TagPrefix="ckbx" TagName="ResponseList" Src="~/Forms/Surveys/Responses/Controls/ResponseList.ascx" %>
<%@ MasterType VirtualPath="~/DetailList.Master" %>
<%@ Import Namespace="Checkbox.Web" %>

<asp:Content ID="_script" runat="server" ContentPlaceHolderID="_head">
    <ckbx:ResolvingScriptElement runat="server" Source="../../../Resources/surveys/responses/manage.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="../../../Resources/StatusControl.js" />
    
    <script type="text/javascript">
        <%-- Ensure statusControl initialized--%>
        $(document).ready(function(){
            statusControl.initialize('_statusPanel');
        });

        function onResponseSelected(response) {
            UFrameManager.init({
                id: 'responseContent',
                loadFrom: '<%=ResolveUrl("~/") %>Forms/Surveys/Responses/View.aspx?responseGuid=' + response.Guid,
                params : {},
                progressTemplate : $('#detailProgressContainer').html(),
                showProgress: true
            });
        }
        function toggleDashSection(item, id){
		    if ($('#' + id).is(':visible')) {
		        $(item).removeClass('pageArrowUp');
		        $(item).addClass('pageArrowDown');
		        $('#' + id).hide('blind', null, 'fast');
		    }
		    else {
		        $(item).removeClass('pageArrowDown');
		        $(item).addClass('pageArrowUp');
		        $('#' + id).show('blind', null, 'fast');
		    }
        }
    </script>
</asp:Content>

<asp:Content ID="_titleContent" name="_titleContent" runat="server" ContentPlaceHolderID="_titleLinks">
    <div id="_manageButtons" runat="server">
        <a class="cancelButton" style="text-decoration: underline;padding-right:10px;" href="#" id="_deleteTestLink"><%=WebTextManager.GetText("/pageText/forms/surveys/responses/manage.aspx/deleteTest")%></a>
        <a class="ckbxButton roundedCorners border999 shadow999 silverButton" href="javascript:showDialog('Export.aspx?s=<%=ResponseTemplateId %>', 'longlargeProperties');" id="_exportLink"><%=WebTextManager.GetText("/pageText/forms/surveys/responses/manage.aspx/export", null, "Export responses")%></a>
        <!--<a class="ckbxButton roundedCorners border999 shadow999 orangeButton" href="javascript:showDialog('Import.aspx?s=<%=ResponseTemplateId %>');" id="_importLink"><%=WebTextManager.GetText("/pageText/forms/surveys/responses/manage.aspx/import", null, "Import responses")%></a>-->
    </div>
</asp:Content>

<asp:Content ID="_leftContent" runat="server" ContentPlaceHolderID="_leftContent">
    <ckbx:ResponseList ID="_responseList" runat="server" ResponseSelectedClientCallback="onResponseSelected" />
</asp:Content>

<asp:Content ID="_rightContent" runat="server" ContentPlaceHolderID="_rightContent">
        <div id="detailProgressContainer" style="display:none;">
            <div id="detailProgress" style="text-align:center;">
                <p>Loading...</p>
                <p>
                    <asp:Image ID="_progressSpinner" runat="server" SkinId="ProgressSpinner" />
                </p>
            </div>
        </div>
        <div id="view_response_header" class="response-header clearfix">
            <div class="num-responses">Viewing #<span id="view_current_response_num"></span> of <span id="view_total_response_num"></span> responses</div>
            <div class="response-navigation">
                <a class="navigate-link prev" href="javascript:void(0)">&laquo; Prev</a>
                <a class="navigate-link next" href="javascript:void(0)">Next &raquo;</a>
            </div>
        </div>
        <div id="responseContent">
            <div class="introPage padding10">
                <p>Click on a response to view details.</p>
            </div>
        </div>
</asp:Content>
