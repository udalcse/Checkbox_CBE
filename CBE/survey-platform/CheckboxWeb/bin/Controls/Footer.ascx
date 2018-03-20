<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Footer.ascx.cs" Inherits="CheckboxWeb.Controls.Footer" %>
<%@ Import Namespace="Checkbox.Management" %>

<div id="footer" class="Footer">
    <div class="content">
        EnGauge Software - Copyright ©2016-<%=DateTime.Now.Year %> The Center for Board Excellence </br>
        EnGauge is a trademark of The Center for Board Excellence </br>
        <%=ApplicationManager.AppSettings.EnvironmentName%>
    </div>
</div>

<script type="text/javascript">
    $(function () {
        <% if (ApplicationManager.AppSettings.EnableMultiDatabase &&
  CurrentPrincipal != null &&
  CurrentPrincipal.IsInRole("System Administrator"))
    { %>
        var isHome = window.location.href.indexOf('Forms/Manage.aspx') >= 0;
        if (isHome) {
            $('#productTourLink').show();
        }
        <% } %>
    });
</script>
