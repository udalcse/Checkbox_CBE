<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimelineSettings.ascx.cs" Inherits="CheckboxWeb.Settings.Controls.TimelineSettings" %>
<%@ Import Namespace="Checkbox.Web" %>

<script type="text/javascript">
    if (typeof (serviceHelper) != 'undefined') { serviceHelper.initialize('<%=ResolveUrl("~/Services/") %>', '<%=ResolveUrl("~") %>'); }
    $(document).ready(function () {
        $('.DialogButtonsContainer').hide();
        showLoader();
        loadTimelineSettings();
    });

    //load timeline settings
    function loadTimelineSettings() {
        templateHelper.loadAndCompileTemplateD('<%=ResolveUrl("~/Settings/jqtmpl/timelineSettingsListTemplate.html") %>', 'timelineSettingsListTemplate.html');
        $('#settingsListContainer').empty();
        loadTimelineSettingsList(_at, '<%=Manager%>', { resultContainer: 'settingsListContainer' });
    }

    //Make ajax call to load timeline settings list and then call selected callback
    function loadTimelineSettingsList(at, managerName, callbackArgs) {
        svcTimeline.GetTimelineSettingsList(at, managerName, callbackArgs)
                .then(onTimelineSettingsListLoaded);
    }

    //Handle list load completed
    function onTimelineSettingsListLoaded(resultData, args) {
        if (resultData == null || args.resultContainer == null) {
            return;
        }
        $('#' + args.resultContainer).empty();

        templateHelper.loadAndApplyTemplateD(
                'timelineSettingsListTemplate.html',
                '<%=ResolveUrl("~/Settings/jqtmpl/timelineSettingsListTemplate.html") %>',
                resultData,
                null,
                args.resultContainer,
                false,
                args)
                .then(onListTemplateApplied);

    }

    //Handle timeline settings template applied
    function onListTemplateApplied(args) {

        //bind clicks and grug and drops here
        $('#settingsListContainer').sortable(
            {
                axis: 'y',
                update: onItemDrop,
                cursor: 'move'
            }

        );

        $('.settingsCheckbox').bind('click', function (event) {
            //service call
            svcTimeline.UpdateTimelineSettingsForEvent(
                _at,
                $(this).attr("manager"),
                $(this).attr("eventName"),
                $(this).attr("name"),
                this.checked, null);
        });

        //make zebra style for event settings
        $("tr:odd").addClass("detailZebra");
        $("tr:even").addClass("zebra");
        removeLoader();
        
    }

    function onItemDrop(event, ui) {
        //update zebra style for list of event settings
        //first clear
        $("tr").removeClass("detailZebra");
        $("tr").removeClass("zebra");
        //then apply needed classes
        $("tr:odd").addClass("detailZebra");
        $("tr:even").addClass("zebra");
        
        //handle change of order
        var oldEventOrder = $(ui.item).attr('eventOrder');
        if (oldEventOrder == null || oldEventOrder == '') {
            return;
        }

        //Get new page ordering
        var newEventOrder;
        var settingsArray = $('#settingsListContainer').children();
        $.each(settingsArray, function (i, val) {
            if ($(val).attr("eventOrder") == oldEventOrder) {
                newEventOrder = i + 1;
            }
        });

        if (newEventOrder < 0) {
            return;
        }

        var settingsItems = $('#settingsListContainer').children();
        if (oldEventOrder > newEventOrder) {
            for (var i = newEventOrder; i < oldEventOrder; i++) {
                $(settingsItems[i]).attr("eventOrder", parseInt($(settingsItems[i]).attr("eventOrder")) + 1);
                svcTimeline.UpdateTimelineSettingsEventOrder(
                    _at,
                    $(settingsItems[i]).attr("manager"),
                    $(settingsItems[i]).attr("eventName"),
                    $(settingsItems[i]).attr("eventOrder"),
                    null);
            }
                
        }
        else {
            for (var i = oldEventOrder - 1; i < newEventOrder - 1; i++) {
                $(settingsItems[i]).attr("eventOrder", $(settingsItems[i]).attr("eventOrder") - 1);
                svcTimeline.UpdateTimelineSettingsEventOrder(
                    _at,
                    $(settingsItems[i]).attr("manager"),
                    $(settingsItems[i]).attr("eventName"),
                    $(settingsItems[i]).attr("eventOrder"),
                    null);
            }
        }
        $(settingsItems[newEventOrder - 1]).attr("eventOrder", newEventOrder);
        svcTimeline.UpdateTimelineSettingsEventOrder(
                            _at,
                            $(settingsItems[newEventOrder - 1]).attr("manager"),
                            $(settingsItems[newEventOrder - 1]).attr("eventName"),
                            $(settingsItems[newEventOrder - 1]).attr("eventOrder"),
                            null);
    }
    
    //Show loader
    function showLoader() {
        $('#settingsListContainer').append(
                '<div id="loader" style="text-align:center;">' +
                    '<p><%=WebTextManager.GetText("/common/loading")%></p>' +
                        '<p><asp:Image ID="_progressSpinner" runat="server" SkinId="ProgressSpinner" /></p>' +
                            '</div>'
            );
    }

    //Stop loader
    function removeLoader() {
        $('#settingsListContainer #loader').remove();
    }

    </script>
    <table class="dashStatsContentHeader customFieldsGrid" style="width:100%">
        <tr>
            <td width="40%"><ckbx:MultiLanguageLabel class="mainStats left" runat="server" TextId="/timeline/eventsettings/caption/event_name_setting"/></td>
            <td width="15%"><ckbx:MultiLanguageLabel class="mainStats left" runat="server" TextId="/timeline/eventsettings/caption/single_period_setting"/></td>
            <td width="15%"><ckbx:MultiLanguageLabel class="mainStats left" runat="server" TextId="/timeline/eventsettings/caption/daily_period_setting"/></td>
            <td width="15%"><ckbx:MultiLanguageLabel class="mainStats left" runat="server" TextId="/timeline/eventsettings/caption/weekly_period_setting"/></td>
            <td width="15%"><ckbx:MultiLanguageLabel class="mainStats left"  runat="server" TextId="/timeline/eventsettings/caption/monthly_period_setting"/></td>
        </tr>
    </table>
    <div id="settingsListContainer" class="customFieldsGrid">
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
           "svcTimeline.js",
           ResolveUrl("~/Services/js/svcTimeline.js"));
        RegisterClientScriptInclude(
           "serviceHelper.js",
           ResolveUrl("~/Services/js/serviceHelper.js"));
        RegisterClientScriptInclude(
           "timeline.js",
           ResolveUrl("~/Resources/timeline.js"));
        RegisterClientScriptInclude(
          "templateHelper.js",
          ResolveUrl("~/Resources/templateHelper.js"));
    }
</script>