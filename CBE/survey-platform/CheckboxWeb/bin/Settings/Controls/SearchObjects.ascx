<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchObjects.ascx.cs" Inherits="CheckboxWeb.Settings.Controls.SearchObjects" %>
<%@ Import Namespace="Checkbox.Web" %>

<script type="text/javascript">
    if (typeof (serviceHelper) != 'undefined') { serviceHelper.initialize('<%=ResolveUrl("~/Services/") %>', '<%=ResolveUrl("~") %>'); }
    $(document).ready(function () {
        showLoader();
        loadSearchSettings();
    });

    //load Search settings
    function loadSearchSettings() {
        templateHelper.loadAndCompileTemplateD('<%=ResolveUrl("~/Settings/jqtmpl/searchObjectsListTemplate.html") %>', 'searchObjectsListTemplate.html');
        $('#settingsListContainer').empty();
        loadSearchSettingsList(_at, { resultContainer: 'settingsListContainer' });
    }

    //Make ajax call to load Search settings list and then call selected callback
    function loadSearchSettingsList(at, callbackArgs) {
        svcSearch.GetSearchSettings(at, onSearchSettingsListLoaded, callbackArgs);
    }

    //Handle list load completed
    function onSearchSettingsListLoaded(resultData, args) {
        if (resultData == null || args.resultContainer == null) {
            return;
        }
        $('#' + args.resultContainer).empty();

        templateHelper.loadAndApplyTemplateD(
                'searchObjectsListTemplate.html',
                '<%=ResolveUrl("~/Settings/jqtmpl/searchObjectsListTemplate.html") %>',
                resultData,
                null,
                args.resultContainer,
                false,
                args)
                .then(onListTemplateApplied);

    }

    //Handle Search settings template applied
    function onListTemplateApplied(args) {

        //bind clicks and grug and drops here        
        $('#settingsListContainer').sortable(
            {
                axis: 'y',
                update: onItemDrop
            }

        );

        $('.settingsCheckbox').bind('click', function (event) {
            //service call
            svcSearch.ToggleSearchObjectType(
                _at,
                $(this).attr("objectType"),
                this.checked);
        });

        $('.searchRoles').click(function (a, b) {
            showRolesDialog($(this).attr('objecttype'), $(this).html());
        });

        //make zebra style for event settings
        $("tr:odd").addClass("detailZebra");
        $("tr:even").addClass("zebra");
        removeLoader();

    }

    var selectedRoles = [];
    var currentObjectType = '';

    function showRolesDialog(objectType, roles) {
        selectedRoles = roles.split(",");
        currentObjectType = objectType;

        $('input.selectedRolesCheckbox').unbind('change');
        $('input.selectedRolesCheckbox').removeAttr("checked");

        for (var i = 0; i < selectedRoles.length; i++) {
            $('.' + selectedRoles[i].replace(" ", "")).attr("checked", "checked");
        }

        $.uniform.update($('input.selectedRolesCheckbox'));

        $('#roleSelectorContainer').modal({
            minHeight: 370,
            maxHeight: 370,
            minWidth: 350,
            maxWidth: 350,
            appendTo: 'form',
            closeHTML: '<div class="ckbx_dialogTitleBar"><div class="ckbx_dialogTitle">Roles</div><br class="clear" /></div>',
            closeClass: 'ckbx-closedialog',
            onOpen: function (dialog) {
                dialog.overlay.fadeIn('300');
                dialog.container.fadeIn('300');
                dialog.data.fadeIn('300');

                $('input.selectedRolesCheckbox').bind('change', function (e) {
                    var input = e.target;
                    if (typeof (input) == 'undefined') {
                        input = e.srcElement;
                    }

                    if ($(input).attr('checked')) {
                        selectedRoles.push($(input).attr('role'));
                    }
                    else {
                        var i = selectedRoles.indexOf($(input).attr('role'));
                        if (i >= 0) {
                            selectedRoles.splice(i, 1);
                        };
                    }
                    $.uniform.update(input);
                });
            },
            onClose: function (dialog) {
                dialog.overlay.fadeOut('300');
                dialog.container.fadeOut('300');
                dialog.data.fadeOut('300', function () {
                    $.modal.close();
                });
            },
            onShow: function (dialog) {
            }
        });
    }
    
    function onSaveClick() {
        svcSearch.UpdateObjectsRoles(_at, currentObjectType, selectedRoles.join(), function () {
            loadSearchSettings();
        }, null);
        $.modal.close();
    }

    function saveRoles(objectType, roles) {
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
            for (var i = newEventOrder; i < oldEventOrder ; i++) {
                $(settingsItems[i]).attr("eventOrder", parseInt($(settingsItems[i]).attr("eventOrder")) + 1);
                svcSearch.UpdateSearchResultsOrder(
                    _at,
                    $(settingsItems[i]).attr("objectType"),
                    $(settingsItems[i]).attr("eventOrder"));
            }
                
        }
        else {
            for (var i = oldEventOrder-1; i < newEventOrder-1; i++) {
                $(settingsItems[i]).attr("eventOrder", $(settingsItems[i]).attr("eventOrder") - 1);
                svcSearch.UpdateSearchResultsOrder(
                    _at,
                    $(settingsItems[i]).attr("objectType"),
                    $(settingsItems[i]).attr("eventOrder"));
            }
        }
        $(settingsItems[newEventOrder - 1]).attr("eventOrder", newEventOrder);
        svcSearch.UpdateSearchResultsOrder(
                    _at,
                    $(settingsItems[newEventOrder - 1]).attr("objectType"),
                    $(settingsItems[newEventOrder - 1]).attr("eventOrder"));
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
            <td width="15%"><ckbx:MultiLanguageLabel class="mainStats left" runat="server" TextId="/pageText/settings/searchsettings.aspx/entitytype"/></td>
            <td width="20%"><ckbx:MultiLanguageLabel class="mainStats left" runat="server" TextId="/pageText/settings/searchsettings.aspx/include"/></td>
            <td width="65%"><ckbx:MultiLanguageLabel ID="MultiLanguageLabel1" class="mainStats left" runat="server" TextId="/pageText/settings/searchsettings.aspx/roles"/></td>
        </tr>
    </table>
    <div id="settingsListContainer" class="customFieldsGrid">
    </div>
    <div id="roleSelectorContainer" style="display:none">
        <div style="padding:10px">
            <div class="fixed_200 left dashStatsContentHeader customFieldsGrid">
                Role
            </div>
            <div class="fixed_100 left dashStatsContentHeader customFieldsGrid">
                Included
            </div>
            <br class="clear" />
            <asp:Repeater ID="_rolesRepeater" runat="server">
                <ItemTemplate>
                        <div class="fixed_200 left" style="padding-top:3px">
                            <%# Container.DataItem %>
                        </div>
                        <div class="fixed_100 left" style="padding-top:3px;text-align:center">
                            <input type="checkbox" class="selectedRolesCheckbox <%# Container.DataItem.ToString().Replace(" ", "") %>" role='<%# Container.DataItem %>'/>
                        </div>
                        <br class="clear" />
                </ItemTemplate>
            </asp:Repeater>
            <br class="clear" />
            <div class="dialogFormPush">&nbsp;</div>
            <div class="left">
                <a href="javascript:onSaveClick();" id="_save" class="center ckbxButton roundedCorners orangeButton shadow999 border999"><%=WebTextManager.GetText("/common/save") %></a>
            </div>
            <div class="right">
            <a href="javascript:void(0);" id="_close" class="center ckbxButton roundedCorners redButton shadow999 border999 ckbx-closedialog"><%=WebTextManager.GetText("/common/close") %></a>
            </div>
        </div>
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
           "svcSearch.js",
           ResolveUrl("~/Services/js/svcSearch.js"));
        RegisterClientScriptInclude(
           "serviceHelper.js",
           ResolveUrl("~/Services/js/serviceHelper.js"));
        RegisterClientScriptInclude(
          "templateHelper.js",
          ResolveUrl("~/Resources/templateHelper.js"));
    }
</script>