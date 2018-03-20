<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="CloseWindow.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.CloseWindow" %>
<%@ Import Namespace="Checkbox.Common" %>

<% if (ExportMode == ExportMode.None)
   {%>
<script type="text/javascript">
    window.close();
</script>
<%} %>

<%-- CloseWindow Item View for Survey --%>
<input type="button" value="Close This Window" onclick="javascript:window.close();" />

