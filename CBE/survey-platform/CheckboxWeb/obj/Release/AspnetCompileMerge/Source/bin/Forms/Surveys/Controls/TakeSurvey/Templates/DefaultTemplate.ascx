<%@ Control Language="C#" CodeBehind="DefaultTemplate.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.TakeSurvey.Templates.DefaultTemplate" %>
<%@ Import Namespace="Checkbox.Management" %>

<%-- This file is used by Checkbox when a survey page has no user-defined template associated with it --%>

<script type="text/javascript">
    $(function () {
        <% if(!ApplicationManager.AppSettings.SetSurveyDefaultButton) { %>
        $('body').keypress(function(e) {
            if(e.keyCode == 13){ 
                return false;
            } 
        });
        <% } %>

       
    });
</script>

<%-- Header --%>
<div class="headWrapper">
    <ckbx:ControlLayoutZone ID="_headerZone" ZoneName="Header" runat="server" DesignBlockMode="true"></ckbx:ControlLayoutZone>
    <div class="clear"></div>
</div>
<div class="clear"></div>

<%-- Take Survey Frame --%>
<div class="wrapperMaster">
    <div class="center borderRadius surveyContentFrame">
        <div class="innerSurveyContentFrame">
            <ckbx:ControlLayoutZone ID="_titleZone" ZoneName="Title" runat="server" DesignBlockMode="true"></ckbx:ControlLayoutZone>
            <div class="clear"></div>
    
            <ckbx:ControlLayoutZone ID="_progressZoneTop" ZoneName="Progress Bar Top" runat="server" DesignBlockMode="true"></ckbx:ControlLayoutZone>
            <div class="clear"></div>

            <ckbx:ControlLayoutZone ID="_pageNumberZone" ZoneName="Page Numbers" runat="server" DesignBlockMode="true"></ckbx:ControlLayoutZone>
            <div class="clear"></div>

            <div class="itemZoneWrapper">
                <%-- Item Zone --%>
                <ckbx:ControlLayoutZone ID="_defaultZone" ZoneName="Default" runat="server" DesignBlockMode="true" CssClass="Page"></ckbx:ControlLayoutZone>
                <div class="clear"></div>
            </div>
            <div class="clear"></div>
            
            <ckbx:ControlLayoutZone ID="_progressZoneBottom" ZoneName="Progress Bar Bottom" runat="server" DesignBlockMode="true"></ckbx:ControlLayoutZone>
        </div>
        <%-- Buttons --%>
        <div class="navigationWrapper">
            <!--<hr size="1" />-->
            <div class="surveyButtonsContainer">
                <ckbx:ControlLayoutZone ID="_previousZone" ZoneName="Back" runat="server" DesignBlockMode="false" CssClass="preview-button"></ckbx:ControlLayoutZone>
                <ckbx:ControlLayoutZone ID="_saveZone" ZoneName="Save and Quit" runat="server" DesignBlockMode="false" CssClass="preview-button"></ckbx:ControlLayoutZone>
                <ckbx:ControlLayoutZone ID="_resetZone" ZoneName="Form Reset" runat="server" DesignBlockMode="false" CssClass="preview-button"></ckbx:ControlLayoutZone>
                <ckbx:ControlLayoutZone ID="_nextZone" ZoneName="Next/Finish" runat="server" DesignBlockMode="false" CssClass="preview-button"></ckbx:ControlLayoutZone>
                <div class="clear"></div>
            </div>
        </div>
    </div>
</div>

<%-- Footer --%>
<div class="footerWrapper">
    <ckbx:ControlLayoutZone ID="_footerZone" ZoneName="Footer" runat="server" DesignBlockMode="true"></ckbx:ControlLayoutZone>
    <div class="clear"></div>
</div>
<div class="clear"></div>

<script>
    //disable buttons on preivew mode
    $(document).ready(function () {
        if (window.location.href.indexOf("Forms/Surveys/Preview") != -1) {
            $('.preview-button').each(function (index, item) {
                $(item.firstChild).attr("disabled", "disabled");
            });
        }

        // if there is a preview container - no need to copy navigation buttons
        if (!$(".previewControlsPagingContainer").length && ShowTopButtons()) {

            var navContainer = '<div class="surveyButtonsContainer"></div>';
            var navigationBtns = $("[id$='_saveBtn'], [id$='_nextBtn'], [id$='_prevBtn']").clone();
            $(".innerSurveyContentFrame").prepend(navContainer);
            $(".innerSurveyContentFrame .surveyButtonsContainer").append(navigationBtns);
        }

        function ShowTopButtons() {
            if (typeof styleConfigs !== 'undefined' && styleConfigs.ShowTopSurveyButtons) {
                var currentPage = $("[id*='_hiddenCurrentPage']").val();
                if (styleConfigs.HideTopSurveyButtonsOnFirstAndLastPage && (currentPage == 1 || currentPage == totalPageNumbers)) {
                    return false;
                } else {
                    return true;
                }
            }

            return false;

        }

    })
</script>
