<%@ Page Title="" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="false" CodeBehind="ResultsDashboard.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.ResultsDashboard" %>
<%@ Import Namespace="Checkbox.Web" %>

<asp:Content ID="style" ContentPlaceHolderID="_styleContent" runat="server">
</asp:Content>

<asp:Content ID="includes" ContentPlaceHolderID="_scriptIncludes" runat="server">
    <ckbx:ResolvingScriptElement ID="_serviceHelper" runat="server" Source="~/Services/js/serviceHelper.js" />
    <ckbx:ResolvingScriptElement ID="_surveyManagementInclude" runat="server" Source="~/Services/js/svcSurveyManagement.js" />
    <ckbx:ResolvingScriptElement ID="_reportDataInclude" runat="server" Source="~/Services/js/svcReportData.js" />
    <ckbx:ResolvingScriptElement ID="_templateHelper" runat="server" Source="~/Resources/templateHelper.js" />
    <ckbx:ResolvingScriptElement ID="_templateEditor" runat="server" Source="~/Resources/templateEditor.js" />
    <ckbx:ResolvingScriptElement ID="_hoverIntent" runat="server" Source="~/Resources/jquery.hoverIntent.js" />

    <script type='text/javascript' src='https://www.google.com/jsapi'></script>
</asp:Content>

<asp:Content ID="scripts" ContentPlaceHolderID="_scriptContent" runat="server">
    <script type="text/javascript">
        /*google.load("visualization", "1", {packages:["corechart"]});
        google.load('visualization', '1', {packages: ['table']});*/
        google.load('visualization', '1', {packages: ['charteditor']});

        var _templRoot = '<%=ResolveUrl("~/Forms/Surveys/Reports/jqtmpl/") %>';
        var _themeFolder = '<%=ResolveUrl("~/App_Themes/" + Page.Theme + "/images/") %>';
        var chartEditor = null;

        $(document).ready(function() {
            chartEditor = new google.visualization.ChartEditor();
            google.visualization.events.addListener(chartEditor, 'ok', redrawChart);
            
            loadResults();
        });

        //
        function loadResults() {
            $('#reportItemPlace').empty();
            
            svcSurveyManagement.getSurveyMetaDataD(
                _at,
                <%=SurveyId %>)
                .then(showItems);
        }
        
        //
        function showItems(result) {
            if(result == null || typeof(result.ItemIds) == 'undefined') {
                alert('No items result');
                return;
            }

            var maxCount = 10; //result.ItemIds.length
            //Create placeholders
            for( var itemNumber = 0; itemNumber < maxCount; itemNumber++) {
                $('<div />')
                    .attr('id', 'result_' + result.ItemIds[itemNumber])
                    .attr('itemId', result.ItemIds[itemNumber])
                    .append($('#detailProgressContainer').clone())
                    .appendTo($('#reportItemPlace'));
            }
            
            //Now do some templates!
            $('#reportItemPlace div[itemId]').each(function() {
                loadItemMetaData($(this).attr('itemId'));
            });
        }

        //
        function loadItemMetaData(itemId) {
            svcSurveyManagement.getSurveyItemMetaDataD(
                _at,
                <%=SurveyId %>,
                itemId)
                .then(applyItemTemplate);
        }
        
        //
        function applyItemTemplate(itemMetaData) {
            if (itemMetaData == null) {
                return;
            }

            //Only include answerable item
            if ('true'.toLowerCase() != getNameValueListProperty(itemMetaData.Properties.NameValueList, 'IsAnswerable').toLowerCase()) {
                $('#result_' + itemMetaData.ItemId).remove();
            }

            templateHelper.loadAndApplyTemplateD(
                'resultDashReportItemTemplate.html',
                _templRoot + 'resultDashReportItemTemplate.html',
                itemMetaData,
                {
                    getPropValue: function(propName) { return getNameValueListProperty(itemMetaData.Properties.NameValueList, propName); },
                    languageCode: '<%=ResponseTemplate.LanguageSettings.DefaultLanguage %>'
                },
                'result_' + itemMetaData.ItemId,
                true,
                null)
                .then(loadItemResult(itemMetaData));
        }
        
        //
        function loadItemResult(itemMetaData) {
            svcReportData.getResultsForSurveyItemD(
                _at,
                <%=SurveyId %> ,
                itemMetaData.ItemId,
                true,
                '<%=ResponseTemplate.LanguageSettings.DefaultLanguage %>',
                {sourceItemMetaData:itemMetaData}
            ).then(displayItemResult);
        }
        
        //
        function displayItemResult(itemResult, args) {
            if(itemResult == null || args == null || typeof(args.sourceItemMetaData) == 'undefined') {
                return;
            }
        
            var chartPlace = $('#chartPlace_' + args.sourceItemMetaData.ItemId);
            
            if(chartPlace.length == 0) {
                return;
            }

            //Associate data with chart place
            chartPlace.data('metaData', args.sourceItemMetaData);
            chartPlace.data('resultData', itemResult);
            
            buildChart(chartPlace);

            //alert('#chartPlace_' + args.sourceItemMetaData.ItemId);
            /*$.each(itemResult.AggregateResults, function(index, value) {
                $("<div />").html(value.ResultText + " -- " + value.AnswerPercent).appendTo(chartPlace);
            });*/
        }

        

        
        //
        function editCurrentChart() {
            var wrapper = getCurrentChartWrapper();
            chartEditor.openDialog(wrapper, {dataSourceInput: 'urlbox'});
        }
        
        //
        function redrawChart() {
            var wrapper = chartEditor.getChartWrapper();
            
            if(wrapper == null) {
                return;
            }
            
            wrapper.draw();
        }
        
        //
        function getNameValueListProperty(nameValueList, propertyName) {
            if (nameValueList == null || typeof(nameValueList.length) == 'undefined' || propertyName == null) {
                return '';
            }
            for (var index = 0; index < nameValueList.length; index++) {
                if (typeof(nameValueList[index].Name) != 'undefined'
                    && typeof(nameValueList[index].Value) != 'undefined'
                        && typeof(nameValueList[index].Name) != null
                            && typeof(nameValueList[index].Value) != null) {

                    if (nameValueList[index].Name.toLowerCase() == propertyName.toLowerCase()) {
                        return nameValueList[index].Value;
                    }
                }
            }

            return '';
        }
        
        //
        function buildChart(chartPlace) {
            var chartData = buildChartDataTable(chartPlace);
            
            if(chartData == null) {
                chartPlace.html('<div class="Error">Unable to build chart data table.</div>');
                return;
          }

            chartPlace.data('resultDataTable', chartData);

            var chartType;
            
            if(chartData.getNumberOfRows() < 10) {
                chartType = 'PieChart';
            }
            else {
                chartType = 'Table';
            }

            var chartWrapper = new google.visualization.ChartWrapper({
                    chartType: chartType,
                    options: {
                        'title':templateEditor.getTextValue(chartPlace.data('metaData').TextData, 'navText', '<%=ResponseTemplate.LanguageSettings.DefaultLanguage %>' ),
                        'height': 300,
                        'width':400
                    },
                    containerId: chartPlace.attr('id'),
                   dataTable: chartData
                });

            chartPlace.css('width', '425px');
            chartPlace.css('height', '325px');
            chartPlace.css('padding-left', '45px');
            chartPlace.data('chartWrapper', chartWrapper);

            google.visualization.events.addListener(chartWrapper, 'ready', function (){onChartReady(chartPlace)});
            
            chartWrapper.draw();
        }
        
        //
        function onChartReady(chartPlace) {
            chartPlace.hoverIntent(showChartMenu, hideChartMenu);
            //google.visualization.events.addListener(chartWrapper.getChart(), 'onmouseover', function() { alert('over'); });
            //google.visualization.events.addListener(chartWrapper, 'onmouseout', function (){hideChartMenu()(chartWrapper)});

        }
        
        //
        /*function getChartSourceUrl(surveyItemId) {
            return svcReportData.getServiceUrl('GetGoogleTableResultsForSurveyItem') + '?authTicket=' + _at + '&surveyId=<%=ResponseTemplate.ID %>&surveyItemId=' + surveyItemId + '&includeIncompleteResponses=true&languageCode=<%=Server.UrlEncode(ResponseTemplate.LanguageSettings.DefaultLanguage) %>';
        }*/
        
        //
        function buildChartDataTable(chartPlace) {
            var resultData = chartPlace.data('resultData');
            
            if(resultData == null || typeof(resultData.AggregateResults) == 'undefined') {
                return null;
            }
            
            var data = new google.visualization.DataTable();
            data.addColumn('string', 'Answer');
            
            if(resultData.AggregateResults.length < 10) {
                data.addColumn('number', 'Responses');
            }

            //Add a row for each result
            for(var resultIndex = 0; resultIndex < resultData.AggregateResults.length; resultIndex++) {
                if (resultData.AggregateResults[resultIndex].AnswerCount > 0) {
                    data.addRows(1);

                    if (resultData.AggregateResults.length < 10) {
                        data.setValue(resultIndex, 0, resultData.AggregateResults[resultIndex].ResultText);
                        data.setValue(resultIndex, 1, resultData.AggregateResults[resultIndex].AnswerCount);
                    }
                    else {
                        data.setCell(resultIndex, 0, resultData.AggregateResults[resultIndex].ResultText);
                    }
                }
            }

            return data;
        }
        
        //
        function showChartMenu() {
            var $menu = $('#chartMenu');
            var pos = $(this).offset();
            var width = $menu.width();

            $menu.css({ "left": (pos.left - width + 45) + "px", "top": pos.top + "px" });
            
            //Update menu buttons
            $('#pieChartBtn').data('chartPlaceId', $(this).attr('id'));
            $('#columnChartBtn').data('chartPlaceId', $(this).attr('id'));

            var currentWrapper = $(this).data('chartWrapper');
            updateChartMenu(currentWrapper.getChartType());
            
            if(!$('#chartMenu').is(':visible')) {
                $('#chartMenu').show('slide', { direction: 'right' }, 250);
            }
        }
        
        //
        function hideChartMenu() {
            //$('#chartMenu').hide('slide', {direction: 'right'}, 250);
        }
        
        //
        function updateChartType(chartType) {
            var wrapper = getCurrentChartWrapper();

            wrapper.setChartType(chartType);
            wrapper.draw();

            updateChartMenu(chartType);
        }
        
        function updateChartMenu(chartType) {
            if(chartType.toLowerCase() == 'piechart') {
                $('#pieChartBtn').attr('src', _themeFolder + 'pieChartActive.png');
                $('#columnChartBtn').attr('src', _themeFolder + 'columnChartInactive.png');
            }
            else {
                $('#pieChartBtn').attr('src', _themeFolder + 'pieChartInactive.png');
                $('#columnChartBtn').attr('src', _themeFolder + 'columnChartActive.png');
            }
        }
        
        //
        function getCurrentChartWrapper() {
            var chartPlaceId = $('#pieChartBtn').data('chartPlaceId');
            return $('#' + chartPlaceId).data('chartWrapper');
        }
    </script>
</asp:Content>

<asp:Content ID="page" ContentPlaceHolderID="_pageContent" runat="server">
    <%-- Chart Menu --%>
    <div id="chartMenu" style="display:none;position:absolute;background-color:#FFFFFF;border:1px solid #FFFFFF;border-radius:3px;width:27px;z-index:152;">
        <div>
            <a href="javascript:updateChartType('PieChart')"><img id="pieChartBtn" src="<%=ResolveUrl("~/App_Themes/" + Theme + "/images/pieChartActive.png") %>" /></a>
        </div>
        <div>
            <a href="javascript:updateChartType('ColumnChart')"><img id="columnChartBtn" src="<%=ResolveUrl("~/App_Themes/" + Theme + "/images/columnChartInactive.png") %>" /></a>
        </div>

        <div style="margin-top:5px;font-size:10px;">
            <a href="javascript:editCurrentChart();">Edit</a>
        </div>

    </div>


    <!-- Loading Div -->
    <%-- Loading Div for Translucent Overlay --%>
    <div style="display:none;">
        <div id="detailProgressContainer" style="z-index:150;width:200px;background-color:#333;opacity:0.5;filter:alpha(opacity=50);">
            <div id="detailProgress" style="text-align:center;background-color:White;border: 3px double #DEDEDE;height:100px;width:200px;vertical-align:middle;z-index:151;">
                <div style="text-align:center;margin-top:25px;"><%=WebTextManager.GetText("/common/loading") %></div>
                <div>
                    <asp:Image ID="_progressSpinner" runat="server" SkinId="ProgressSpinner" />
                </div>
            </div>
            <div class="clear"></div>
        </div>
    </div>

 
    
    <div style="height:750px;width:750px;border:1px solid gray;overflow-y:auto;">
        <h3><%=ResponseTemplate.Name %></h3>
        <div id="reportItemPlace" style="margin-left:auto;margin-right:auto;width:400px;">
        </div>
    </div>
</asp:Content>

