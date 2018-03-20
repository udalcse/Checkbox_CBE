<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ItemConditionEditor.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemConditionEditor" %>
<%@ Register TagPrefix="ckbx" TagName="RuleEditor" Src="~/Forms/Surveys/Controls/RuleEditor.ascx" %>

<div id="ruleEditorContainer" <%=!string.IsNullOrEmpty(Request["isNew"]) ? "style='height:500px;'" : "" %>>
    <ckbx:RuleEditor id="_ruleEditor" runat="server" TabStyle="inverted" />
</div>