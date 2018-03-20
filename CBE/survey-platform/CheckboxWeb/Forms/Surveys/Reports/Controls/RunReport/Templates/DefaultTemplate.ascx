<%@ Control Language="C#" CodeBehind="DefaultTemplate.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.RunReport.Templates.DefaultTemplate" %>

<%-- This file is used by Checkbox when a report page has no user-defined template associated with it --%>

<%-- View Report Frame --%>
<div class="wrapperMaster">
    <div class=" center borderRadius surveyContentFrame">
        <div class="innerSurveyContentFrame">
            <ckbx:ControlLayoutZone ID="_titleZone" ZoneName="Title" runat="server" DesignBlockMode="true"></ckbx:ControlLayoutZone>
            <div class="clear"></div>

            <ckbx:ControlLayoutZone ID="_pageNumberZone" ZoneName="Page Numbers" runat="server" DesignBlockMode="true"></ckbx:ControlLayoutZone>
            <div class="clear"></div>

             <div class="itemZoneWrapper">
                <%-- Item Zone --%>
                <ckbx:ControlLayoutZone ID="_defaultZone" ZoneName="Default" runat="server" DesignBlockMode="true" CssClass="Page"></ckbx:ControlLayoutZone>
                <div class="clear"></div>
            </div>
            <div class="clear"></div>
        </div>
    </div>
</div>

