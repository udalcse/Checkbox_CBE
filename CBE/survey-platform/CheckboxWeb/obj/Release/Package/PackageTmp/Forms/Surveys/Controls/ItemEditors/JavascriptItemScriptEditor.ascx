<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="JavascriptItemScriptEditor.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.JavascriptItemScriptEditor" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Common" %>

<ckbx:ResolvingScriptElement ID="_includeAceEditor" runat="server" Source="~/Resources/ace-editor/ace.js" />

<div id="js-editor" uframeignore="true">
    <%= Utilities.AdvancedHtmlEncode(Script.Trim()) %>
</div>

<asp:HiddenField ID="_script" runat="server"/>

<script type="text/javascript">
    var editor_<%= ClientID %> = null;

    $(function () {
        ace.config.set("basePath", "<%= ApplicationManager.ApplicationRoot %>/Resources/ace-editor");
        editor_<%= ClientID %> = ace.edit("js-editor");
        editor_<%= ClientID %>.setOptions({
            maxLines: Infinity,
            autoScrollEditorIntoView: true,
            showPrintMargin: false
        });
        editor_<%= ClientID %>.setTheme("ace/theme/chrome");
        editor_<%= ClientID %>.renderer.setScrollMargin(10, 10, 10, 10);
        editor_<%= ClientID %>.getSession().setMode("ace/mode/javascript");
        editor_<%= ClientID %>.on("change", function(e) {
            $("#<%= _script.ClientID %>").val(editor_<%= ClientID %>.getSession().getValue());
        });
    });
</script>