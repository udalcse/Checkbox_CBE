<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Javascript.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.Javascript" %>

<script type="text/javascript">
    <%= StriptScriptTags( Model.InstanceData["Script"]) %>
</script>
