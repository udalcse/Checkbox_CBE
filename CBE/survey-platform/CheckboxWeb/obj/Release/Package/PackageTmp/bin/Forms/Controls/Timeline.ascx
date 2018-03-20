<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Timeline.ascx.cs" Inherits="CheckboxWeb.Forms.Controls.Timeline" %>
<%@ Import Namespace="Checkbox.Security" %>
<%@ Import Namespace="Checkbox.Users" %>
<%@ Import Namespace="Checkbox.Web" %>

<ckbx:ResolvingScriptElement runat="server" Source="../../Resources/timeline.js" />

<script language="javascript" type="text/javascript">
    $(document).ready(
        function () {
            timeline.initialize(
            {
                at: _at,
                requestId : 0,
                id: '<%=ClientID%>_list', 
                manager: '<%=Manager%>',
                recordsPerPage : <%=RecordsPerPage%>,
                <%if (!string.IsNullOrEmpty(VisibleEvents)) {%>
                visibleEvents : [
                <%for (int i = 0; i < ParsedEvents.Length; i++) {%>
                    '<%=ParsedEvents[i]%>'<%=i < ParsedEvents.Length - 1 ? "," : "" %>
                <%} %>
                ],
                <%}%>
                <%if (!string.IsNullOrEmpty(OnClientLoad)) {%>
                onload : function() { <%=OnClientLoad%>(); },
                <%}%>
                expiration : <%=RequestExpiration%>,
                graphContainerId: '<%=ClientID%>_graph', 
                showGraph : <%=(ShowGraph && RoleManager.UserHasRoleWithPermission(UserManager.GetCurrentPrincipal().Identity.Name, "Analysis.Responses.View")).ToString().ToLower() %>
            },
            '<%=ResolveUrl("~/") %>');
        });

</script>
<div id="<%=ClientID%>_container" class="recent-activity">
    <h4><%=WebTextManager.GetText("/controlText/timeline.ascx/recent_activity")%></h4>
    <div id="<%=ClientID%>_graph" class="graph" style="display:none">Graph</div>
    <ul id="<%=ClientID%>_list" class="activity-list">
        <img id="ctl00_ctl00__pageContent__leftContent__surveyList__progressSpinner" class="ProgressSpinner" src="../App_Themes/CheckboxTheme/Images/ProgressSpinner.gif" style="height:31px;width:31px;border-width:0px;">
    </ul>
    <div><a href="javascript:timeline.showAll('<%=ClientID%>_list');" id="<%=ClientID%>_list_show">Show All</a></div>
</div>

<script type="text/C#" runat="server">
    /// <summary>
    /// Override OnLoad to ensure necessary scripts are loaded.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        RegisterClientScriptInclude(
            "timeline.js",
            ResolveUrl("~/Resources/timeline.js"));

        RegisterClientScriptInclude(
            "svcResponseData.js",
            ResolveUrl("~/Services/js/svcResponseData.js"));

        RegisterClientScriptInclude(
            "svcTimeline.js",
            ResolveUrl("~/Services/js/svcTimeline.js"));
    }

</script>