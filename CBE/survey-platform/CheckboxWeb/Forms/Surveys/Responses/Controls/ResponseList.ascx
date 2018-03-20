<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ResponseList.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Responses.Controls.ResponseList" %>
<%@ Register TagPrefix="ckbx" TagName="Grid" Src="~/Controls/Grid.ascx" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web" %>

<script type="text/javascript">
  <%-- Ensure service initialized --%>
    $(document).ready(function() {
        //implementation all/none selection
        $(document).on('click', '.deleteResponse', function(event) {
            toggleGridActionButtons('response', this);
            event.stopPropagation();
        });
        $(document).on('click', '#_selectAllResponses', function() {
            var actionsAvailable = false;
            if ($(this).prop('checked')) {
                actionsAvailable = true;
                $('.deleteResponse').prop('checked', true);
            } else {
                $('.deleteResponse').prop('checked', false);
            }
            $.uniform.update('.deleteResponse');
            toggleGridActionButtons('response', this, actionsAvailable);
        });


        //Bind delete link click
        $('#_deleteSelectedLink').click(function() {
            if ($('.deleteResponse:checked').length > 0) {
                showConfirmDialogWithCallback(
                    '<%=WebTextManager.GetText("/pageText/forms/surveys/responses/manage.aspx/deleteSelectedConfirm") %>',
                    onDeleteSelectedConfirm,
                    337,
                    200,
                    '<%=WebTextManager.GetText("/pageText/forms/surveys/responses/manage.aspx/deleteSelected") %>'
                );
            }
        });

        $('#_deleteTestLink').click(function() {
            showConfirmDialogWithCallback(
                '<%=WebTextManager.GetText("/pageText/forms/surveys/responses/manage.aspx/deleteTestConfirm") %>',
                onDeleteTestConfirm,
                337,
                200,
                '<%=WebTextManager.GetText("/pageText/forms/surveys/responses/manage.aspx/deleteTest") %>'
            );
        });

        $('#_deleteAllLink').click(function() {
            showConfirmDialogWithCallback(
                '<%=WebTextManager.GetText("/pageText/forms/surveys/responses/manage.aspx/deleteAllConfirm") %>',
                onDeleteAllConfirm,
                337,
                200,
                '<%=WebTextManager.GetText("/pageText/forms/surveys/responses/manage.aspx/deleteAll") %>'
            );
        });

        $('#_exportToPdfLink').click(function() {
            var selectedRows = $(".ckbxGrid[id^='listPlace'] input.deleteResponse:checked").closest(".gridContent");
            var selectedGuids = {};

            $.each(selectedRows, function(ind, elem) {
              
                var username = $.tmplItem($(elem)).data.UserIdentifier;
                var guid = $.tmplItem($(elem)).data.Guid;
                var completionDate = dateUtils.jsonDateToInvariantDateString($.tmplItem($(elem)).data.CompletionDate, "m.d.yyyy", true).replace("/", ".").replace("/",".");
                selectedGuids[guid] = username + "_" + completionDate;
            });

            $.ajax({
                url: "Manage.aspx/SaveUserIds",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({userIds : JSON.stringify(selectedGuids)}),
                datatype: "json",
                success: function (result) {
                    showDialog("/Forms/Surveys/AdminResponses/Export.aspx?bulkPDF=" + result.d + "&loc=EN-US','properties', '', null, '#_surveyForm')");
                    //window.open("/Forms/Surveys/AdminResponses/Export.aspx?bulkPDF="+ , "mywindow", "menubar=0,resizable=0,width=600,height=370");
                }
            });
        });

        $("#clearRespondentFilter").click(function() {
            $("#profileFieldValue").val("");
            gridHelper_<%=_responseGrid.ClientID %>.filterKey = getFilterData();
            reloadResponseList();
            toggleGridActionButtons('response', $(".ckbxGrid[id^='listPlace']").children()[0]);
        })

        statusControl.initialize('_statusPanel');
        
        //if response list is the list of timeline results
        if (<%=TimelinePeriod%>!=0)
            $('#_deleteAllLink').hide();

        $('.gridFilter input').on('change', function() {
            gridHelper_<%=_responseGrid.ClientID %>.filterKey = getFilterData();
            reloadResponseList();
            toggleGridActionButtons('response', $(".ckbxGrid[id^='listPlace']").children()[0]);
        });
    });
    
    //
    function getFilterData() {
        var filter = [];
        var checkboxes = $('.gridFilter :checked');
        for (var i=0; check = checkboxes[i]; i++) {
            filter.push($(check).attr('data-filter'));
        }
        return filter.join();
    }

    //Reload list
    function reloadResponseList() {
        <%=_responseGrid.ReloadGridHandler %>(true);
    }

    <%-- Load survey response list --%>
    function loadResponseList(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs, filterKey) {
        loadCompleteArgs['isAggregatedResult'] = '<%=(TimelinePeriod!=0)? "true":null %>';

        var profileFieldId = $("select[id$='_searchProperties']").val();
        var profileFieldValue = $("#profileFieldValue").val();
        var searchForProfileField = false;

        if (profileFieldValue && profileFieldValue.trim() != '') 
        {
            searchForProfileField = true;
        }

        svcResponseData.listSurveyResponses(
            _at, 
            <%=SurveyId  %>, 
             {
                 pageNumber: currentPage,
                 resultsPerPage: <%= ApplicationManager.AppSettings.PagingResultsPerPage %>,
                 filterField: searchForProfileField ? 'ProfileField' : '',
                 filterValue: searchForProfileField ? profileFieldValue : '',
                 profielFieldId : searchForProfileField ? profileFieldId : 0,
                sortField: sortField,
                sortAscending: sortAscending,
                period: <%= TimelinePeriod %>,
                dateFieldName: 'Ended'
             }, 
            loadCompleteCallback,
            loadCompleteArgs,
            filterKey
        );       
        }

        //
        function onDeleteSelectedConfirm(args){
            if(args.success){
                var idArray = new Array();

                $('.deleteResponse:checked').each(function(index){
                    idArray.push($(this).attr('value'));
                });

                if(idArray.length > 0){
                    svcResponseData.deleteSelectedResponses(_at, <%=SurveyId %>, idArray, onResponsesDeleted);
                onResponsesDeleted();
            }
        }
    }

    //
    function onDeleteAllConfirm(args){
        if(args.success){
            svcResponseData.deleteAllSurveyResponses(_at, <%=SurveyId %>, onResponsesDeleted);
            onResponsesDeleted();
        }
    }

    //
    function onDeleteTestConfirm(args){
        if(args.success){
            svcResponseData.deleteTestSurveyResponses(_at, <%=SurveyId %>, onResponsesDeleted);
            onResponsesDeleted();
        }
    }
    
    //
    function onResponsesDeleted(){
        <%=_responseGrid.ReloadGridHandler %>();
        svcResponseData.getResponseSummaryD(_at, <%=SurveyId %>).then(onResponseSummaryGot);
    }

    //
    function onResponseSummaryGot(data) {
        $('#numberOfCompleteResponses').html(data.CompletedResponseCount);
        $('#numberOfIncompleteResponses').html(data.IncompleteResponseCount);
    }

    //
    function onDialogClosed(args){
        if(args && args.action && args.success) {
            if (args.action == 'import' && args.success == 'true') {
                <%=_responseGrid.ReloadGridHandler %> ();
            }
        }
    }
    
    //Render Grid comlete handler
    function gridRenderComplete() {
        <%=_responseGrid.ShowSorter %>();
    }
</script>

<div class="gridMenu clearfix">
    <div style="display: inline-block">
        <div class="gridSorter left" style="padding: 5px 5px 5px 15px;">
            <%=WebTextManager.GetText("/pageText/forms/surveys/responses/manage.aspx/numOfComplete")%> : <span id="numberOfCompleteResponses"><%= NumberOfCompleteResponses %></span>
        </div>
        <div class="gridSorter left" style="padding: 5px;">
            <%=WebTextManager.GetText("/pageText/forms/surveys/responses/manage.aspx/numOfIncomplete")%> : <span id="numberOfIncompleteResponses"><%= NumberOfIncompleteResponses %></span>
        </div>
    </div>
    <div class="left gridSorter">
        <div class="gridFilter">
            <div style="display: inline-block; padding: 5px;">
                <span>Filter :&nbsp;&nbsp;</span>
                <label for="filter-complete">Complete</label>
                <input id="filter-complete" data-filter="Complete" checked="checked" type="checkbox" />
                <label for="filter-incomplete">Incomplete</label>
                <input id="filter-incomplete" data-filter="Incomplete" checked="checked" type="checkbox" />
                <label for="filter-test">Test</label>
                <input id="filter-test" data-filter="Test" checked="checked" type="checkbox" />
            </div>
        </div>
    </div>
   <div class="clear"></div>
    <div class="left gridSorter">
        <div class="gridFilter">
            <div style="display: inline-block; padding: 5px;">
                <span><%=WebTextManager.GetText("/pageText/forms/surveys/responses/manage.aspx/respondentFilter")%> :&nbsp;&nbsp;</span>
                <asp:DropDownList id="_searchProperties"
                    runat="server">
               </asp:DropDownList>
                <span><%=WebTextManager.GetText("/pageText/forms/surveys/responses/manage.aspx/respondentFilterEqual")%> :&nbsp;&nbsp;</span>
                <input type="textbox" value="" id="profileFieldValue"/>
                <a class="cancelButton" style="text-decoration: underline" href="#" id="clearRespondentFilter"><%=WebTextManager.GetText("/pageText/forms/surveys/responses/manage.aspx/clearRespondentFilter")%></a>
            </div>
        </div>
    </div>

    <div class="gridButtons itemActionMenu" style="float: right; margin: 0 5px 0 0;">
        <a class="cancelButton" style="text-decoration: underline" href="#" id="_deleteSelectedLink"><%=WebTextManager.GetText("/pageText/forms/surveys/responses/manage.aspx/deleteSelected")%></a>
    </div>
    <div class="gridButtons itemActionMenu" style="display: block; float: right;">
        <a class="cancelButton" style="text-decoration: underline" href="#" id="_deleteAllLink"><%=WebTextManager.GetText("/pageText/forms/surveys/responses/manage.aspx/deleteAll")%></a>
    </div>
    <div class="gridButtons itemActionMenu" style="float: right; margin: 0 5px 0 0;">
        <a class="cancelButton" style="text-decoration: underline" href="#" id="_exportToPdfLink"><%=WebTextManager.GetText("/pageText/forms/surveys/responses/manage.aspx/exportToPdf")%></a>
    </div>
</div>

<%-- Container for Results --%>
<ckbx:Grid ID="_responseGrid" runat="server" GridCssClass="ckbxGrid" />

<%-- Event Handling  --%>
<script type="text/C#" runat="server">
    /// <summary>
    /// Override OnLoad to ensure necessary scripts are loaded.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        RegisterClientScriptInclude(
           "serviceHelper.js",
           ResolveUrl("~/Services/js/serviceHelper.js"));

        RegisterClientScriptInclude(
            "templateHelper.js",
            ResolveUrl("~/Resources/templateHelper.js"));

        RegisterClientScriptInclude(
           "svcResponseData.js",
           ResolveUrl("~/Services/js/svcResponseData.js"));

        RegisterClientScriptInclude(
            "dateUtils.js",
            ResolveUrl("~/Resources/dateUtils.js"));

        //Moment.js: datetime utilities
        RegisterClientScriptInclude(
            "moment.js",
            ResolveUrl("~/Resources/moment.js"));

        RegisterClientScriptInclude(
            "grid.js",
            ResolveUrl("~/Resources/grid.js"));

        RegisterClientScriptInclude(
            "jquery.pager.js",
            ResolveUrl("~/Resources/jquery.pager.js"));
    }
</script>
