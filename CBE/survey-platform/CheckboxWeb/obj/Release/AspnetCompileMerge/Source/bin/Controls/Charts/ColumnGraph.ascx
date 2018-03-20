<%@ Import Namespace="Checkbox.Common" %>
<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ColumnGraph.ascx.cs" Inherits="CheckboxWeb.Controls.Charts.ColumnGraph" %>
		<script type="text/javascript">
		    $(function () {
		        var chart;
		        $(document).ready(function () {
                    var renderDiv = 'highchart-render-place';
                    if ($('#'+renderDiv).length < 1) {
                        renderDiv = '_container__<%=ClientID%>';
                    }
		            chart = new Highcharts.Chart({
		                chart: {
                            animation: <%=Utilities.AsBool(Appearance["Animation"], true).ToString().ToLower()%>,
		                    renderTo: renderDiv,
		                    plotBackgroundColor: '<%=PlotAreaBackgroundColor%>',
		                    plotBorderWidth: null,
                            backgroundColor: '<%=BackgroundColor%>',
		                    borderWidth: 0,
                            width: <%=Utilities.AsInt(Appearance["Width"], 800) %>,
                            height: <%=Utilities.AsInt(Appearance["Height"], 400) %>,
		                    plotShadow: null,
                            marginTop: '<%=Utilities.AsInt(Appearance["ChartMarginTop"], 50)%>',
                            marginBottom: '<%=Utilities.AsInt(Appearance["ChartMarginBottom"], 100)%>',
                            marginLeft: '<%=Utilities.AsInt(Appearance["ChartMarginLeft"], 80)%>',
                            marginRight: '<%=Utilities.AsInt(Appearance["ChartMarginRight"], 50)%>',
                            events : {
                                load: function(a)
                                {
                                    $(this.container).css("margin", "0 auto");
                                    
                                    //fix to avoid dismissed render div
                                    $('#highchart-render-wrapper #highchart-render-place').appendTo('#_container__<%=ClientID%>');
                                    $('#highchart-render-wrapper').append('<div id="highchart-render-place"></div>');

                                    //fix title font weight
                                    $(this.container).find('text.highcharts-title tspan').css('font-weight', 'bold');
                                }    
                            }
		                },
                        exporting:
                        {                            
                            enabled:<%=Utilities.AsBool(Appearance["AllowExporting"], true).ToString().ToLower()%>                            
                        },
                        credits:
                        {   
                            enabled:false
                        },
		                title: {
		                    text: '<b>'+ htmlDecode('<%=EscapeJSStringConst(GetTitle().Replace("\n", "~~~newline~~~")) %>').replace(/~~~newline~~~/g, '<br/>') + '</b>',
                            style:
                                {
                                    color: '<%=TextColor%>',
                                    fontFamily: '<%=Appearance["TitleFont"] %>',
                                    fontSize: '<%=Appearance["TitleFontSize"] %>pt'
                                }
		                },
                        xAxis: [
                        <% for (int iSeries = 0; iSeries < xAxisCategories.Count; iSeries++) {
                        %> <% if (iSeries > 0) {%>,<%} %>
                            {
                                categories: [
                                    <% for (int iCategory = 0; iCategory < xAxisCategories[iSeries].Count; iCategory++) {
                                    %> <% if (iCategory > 0) {%>,<%} %>
                                    htmlDecode('<%=EscapeJSStringConst(Wrap(xAxisCategories[iSeries][iCategory]))%>')
                                    <%}%>
                                ]
                            }
                        <%} %>                
                        ],
                        yAxis: {
                            min: 0,
                            title: ''
                        },
                        navigation: {
                                buttonOptions: {
                                    verticalAlign: 'top'
                                }
                            },
                        subtitle: {
                            style:
                                {
                                    color: '<%=TextColor%>',
                                    fontFamily: '<%=Appearance["TitleFont"] %>'
                                }
                        },
		                tooltip: {
		                    pointFormat: '<%=Tooltip%>',
		                    percentageDecimals: 2                              
		                },
                        <%if (Colors != null && Colors.Length > 0) %>
                        colors: [
                            <%for (int color = 0; color < Colors.Length; color++) { %>
                                '<%=Colors[color] %>'<%=(color < Colors.Length - 1 ? "," : "")%>
                            <%}; %>
                        ],
		                plotOptions: {
		                    column: {
		                        allowPointSelect: true,
                                column: 'percent',
		                        cursor: 'pointer',
                                borderColor: '<%=PieBorderColor%>',
		                        dataLabels: {
		                            enabled: 
                        <%if (Utilities.AsBool(Appearance["ShowAnswerCount"], true) || Utilities.AsBool(Appearance["ShowPercent"], true)) { %>
                                    true,
                        <% } else { %>
                                    false,
                        <% } %>                                    
                                    connectorColor : '<%=HintTextColor%>',
                                    color: '<%=HintTextColor%>',
                                    style: {
                                        fontFamily: '<%=Appearance["LegendFont"] %>',
                                        fontSize: '<%=Appearance["LabelFontSize"] %>pt'
                                    },
		                            formatter: function () {
                        <%if (Utilities.AsBool(Appearance["ShowAnswerCount"], true) && Utilities.AsBool(Appearance["ShowPercent"], true)) { %>
		                                return this.y + '(' + Math.round(this.percentage * 100) / 100 + '%)';
                        <% } else if (Utilities.AsBool(Appearance["ShowAnswerCount"], true)) { %>
		                                return this.y;
                        <% } else if (Utilities.AsBool(Appearance["ShowPercent"], true)) { %>
		                                return Math.round(this.percentage * 100) / 100 + '%';
                        <% } else { %>
                                        return "";
                        <% }%>
		                            }
		                        },
                                showInLegend: true
		                    }
		                },                        
		                series: [
                    <%
                    for (int s = 0; s < Series.Length; s++) {                        
                    %>
                        <%if (s > 0) {%>,<%}%>
                        {
		                    type: 'column',
		                    name: htmlDecode('<%=EscapeJSStringConst(Series[s].Name)%> 1'),
                            xAxis: <%=xAxisCategories.Count == 1 ? 0 : s%>,
		                    data: [
                            <% 
                                bool hasAnswer = false;
                                for (int row = 0; row < Series[s].AggregateResults.Length; row++) { %>
                                <% if (Series[s].AggregateResults[row].AnswerCount > 0 || Utilities.AsBool(Appearance["ShowDataLabelZeroValues"], false)) { %> 
                                <%=(hasAnswer ? "," : "") %> {name: htmlDecode('<%=EscapeJSStringConst(Wrap(Series[s].AggregateResults[row].ResultText)) %>'), y : <%=UsePointsAsY ? Series[s].AggregateResults[row].Points : Series[s].AggregateResults[row].AnswerCount %>, percentage : <%=Series[s].AggregateResults[row].AnswerPercent.ToString().Replace(",", ".") %>} 
                                <% hasAnswer = true;} %>
                            <%}; %>
                            ]
		                }


                    <%} %>
                        
                        ],
                        legend: {
                            enabled: <%=(Utilities.AsBool(Appearance["ShowLegend"], false)).ToString().ToLower()%>,
                            itemStyle:
                                {
                                    color: '<%=LegendTextColor%>',
                                    fontFamily: '<%=Appearance["LegendFont"] %>',
                                    fontSize: '<%=Appearance["LegendFontSize"] %>pt'
                                },
                            backgroundColor: '<%=LegendBackgroundColor%>',
                            layout: '<%=Appearance["LegendLayout"] %>',
                            align: '<%=Appearance["LegendAlign"] %>',
                            verticalAlign: '<%=Appearance["LegendVerticalAlign"] %>'
                        },
                        labels: {
                            style: {
                                fontFamily: '<%=Appearance["TitleFont"] %>',
                                color: '<%=TextColor%>'
                            }
                        }
		            });
		        });

		    });
		</script>


        <div id="_container__<%=ClientID%>" style="margin: 0 auto"></div>



