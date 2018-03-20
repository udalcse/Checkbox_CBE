<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="PipeControl.ascx.cs" Inherits="CheckboxWeb.Controls.Piping.PipeControl" %>
<%@ Register TagPrefix="questionBinding" TagName="QuestionBindingSelector" Src="~/Controls/Piping/QuestionBindingControl.ascx" %>
<%@ Register TagPrefix="termsBinding" TagName="TermBindingSelector" Src="~/Controls/Piping/TermsBindingControl.ascx" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Users" %>
<%@ Import Namespace="Checkbox.Web" %>

<div>
    <span>
        <select id="<%= ID %>_sourceList" pipetarget="<%= AssociatedControlId%>" uframeignore="true">
            <option value="__NONE__"><%=WebTextManager.GetText("/common/dropDownDefault") %></option>
        </select>

        <btn:CheckboxButton ID="_mergeButton" runat="server" TextId="/controlText/pipeControl.ascx/merge" CssClass="ckbxButton roundedCorners border999 shadow999 silverButton" Style="font-size: 11px;" uframeignore="true" />
    </span>
    <% if (IsTwoWayBindingControl)
        { %>
    <span class="formInput">
        <questionBinding:QuestionBindingSelector ID="_questionBindingSelector" runat="server" />
    </span>
    <% } %>
    <% if (UseTerms)
        { %>
    <span class="formInput">
        <termsBinding:TermBindingSelector ID="_termBindingSelector" runat="server" />
    </span>
    <% } %>
</div>

<script type="text/javascript">
    function <%= ID %>bindToInput(inputId) {
        pipeHandler.bind(inputId, '<%= ID %>_sourceList', '<%=_mergeButton.ClientID %>');
    }
    
    $(document).ready(function() {
        //for all item fileds
        pipeHandler.initialize(
            <%=ResponseTemplateId.HasValue ? ResponseTemplateId.ToString() : "null"%> ,
            <%=MaxPagePosition.HasValue ? MaxPagePosition.ToString() : "null"%> ,
            '<%=LanguageCode%>',
            '<%=ApplicationManager.AppSettings.PipePrefix%>');

        termHandler.initialize(
            <%=ResponseTemplateId.HasValue ? ResponseTemplateId.ToString() : "null"%> ,
            <%=MaxPagePosition.HasValue ? MaxPagePosition.ToString() : "null"%> ,
           '<%=LanguageCode%>',
           '<%=ApplicationManager.AppSettings.TermPrefix%>');
    });
</script>
