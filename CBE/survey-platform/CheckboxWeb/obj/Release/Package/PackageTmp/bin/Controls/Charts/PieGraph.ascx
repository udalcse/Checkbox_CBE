<%@ Import Namespace="Checkbox.Common" %>
<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="PieGraph.ascx.cs" Inherits="CheckboxWeb.Controls.Charts.PieGraph" %>
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
		                    pie: {
		                        allowPointSelect: true,
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
                        {
		                    type: 'pie', 
		                    name: 'Percentage',
		                    data: [
                            <% 
                                bool hasAnswer = false;
                                for (int row = 0; row < AggregateResults.Length; row++) { %>
                                <% if (AggregateResults[row].AnswerCount > 0 || Utilities.AsBool(Appearance["ShowDataLabelZeroValues"], false)) { %> 
                                <%=(hasAnswer ? "," : "") %> [htmlDecode('<%=EscapeJSStringConst(Wrap(AggregateResults[row].ResultText)) %>'), <%=UsePointsAsY ? AggregateResults[row].Points : AggregateResults[row].AnswerCount %>] 
                                <% hasAnswer = true;} %>
                            <%}; %>
                            ]
		                }],
                        legend: {
                            enabled: <%=(Utilities.AsBool(Appearance["ShowLegend"], true)).ToString().ToLower()%>,
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



