<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ItemActiveDisplay.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemActiveDisplay" %>
<%@ Import Namespace="Checkbox.Web" %>

<asp:Panel runat="server" ID="_itemActiveDisplay" CssClass="itemIsNotActiveWarning" style="display: none">
    <%=WebTextManager.GetText("/controlText/templatePageEditor/inactiveWarning")%>
</asp:Panel>

<script type="text/javascript">
    <% if (!_isActive) { %>
          $(function() {
              $('#<%=_itemActiveDisplay.ClientID %>').show();        
          });
    <% } %>
</script>