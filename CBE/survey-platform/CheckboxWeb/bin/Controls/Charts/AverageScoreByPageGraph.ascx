<%@ Import Namespace="Checkbox.Common" %>
<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="AverageScoreByPageGraph.ascx.cs" Inherits="CheckboxWeb.Controls.Charts.AverageScoreByPageGraph" %>
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
		                    renderTo: renderDiv,
		                    plotBackgroundColor: '<%=PlotAreaBackgroundColor%>',
		                    plotBorderWidth: null,
                            backgroundColor: '<%=BackgroundColor%>',
                            borderColor: '<%=BorderLineColor%>',
		                    borderWidth: <%=BorderLineWidth %>,
                            borderRadius: '<%=Utilities.AsInt(Appearance["BorderRadius"], 0)%>px',
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
                            text: ''
		                    
		                },
                        yAxis: {
                            min: 0,
                            title: ''
                        },
                        xAxis: {
                            labels: {enabled : false},
                            tickLength: 0
                        },
                        navigation: {
                                buttonOptions: {
                                    verticalAlign: 'top'
                                }
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
                        subtitle: {
                            style:
                                {
                                    color: '#000000',
                                    fontFamily: 'Arial'
                                }
                        },
		                tooltip: {
		                    pointFormat: '<%=TooltipWithoutPercentage%>',
		                    percentageDecimals: 2                              		                },
                        <%if (Colors != null && Colors.Length > 0) %>
                        colors: [
                            <%for (int color = 0; color < Colors.Length; color++) { %>
                                '<%=Colors[color] %>'<%=(color < Colors.Length - 1 ? "," : "")%>
                            <%}; %>
                        ],
		                plotOptions: {
		                    column: {
		                        allowPointSelect: true,
		                        animation: <%=Utilities.AsBool(Appearance["Animation"], true).ToString().ToLower()%>,
                                column: 'percent',
		                        cursor: 'pointer',
                                borderColor: '<%=PieBorderColor%>',
                                showInLegend: true,
		                        style: {
                                        fontFamily: '<%=Appearance["LegendFont"] %>',
                                        fontSize: '<%=Appearance["LabelFontSize"] %>pt'
                                },
                                dataLabels: {
		                            enabled: true
                                }
		                    }
		                },                        
		                series: [
                            <% 
                                bool hasAnswer = false;
                                for (int row = 0; row < Model.AggregateResults.Length; row++) { %>
                                <%=(hasAnswer ? "," : "") %>
                        {
		                    type: 'column',
                            name: '<%=EscapeJSStringConst(Wrap(Model.AggregateResults[row].ResultText)) %>',
		                    data: [
                                ['<%=EscapeJSStringConst(Wrap(Model.AggregateResults[row].ResultText)) %>', <%=string.Format("{0:0.00}", Model.AggregateResults[row].AnswerPercent).Replace(",", ".") %>]
                            ]
		                }
                            <% hasAnswer = true; }; %>
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
                            enabled: true,
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



