<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GovernancePrioritySummary.ascx.cs" Inherits="CheckboxWeb.Controls.Charts.GovernancePrioritySummary" %>
<%@ Import Namespace="Checkbox.Common" %>

<script type="text/javascript">
    $(function() {
        var chart;

        ////setting default height fir editor container
        $('#itemEditorContainer').css('height', '700px');

        $(document).ready(function() {
            //settings
            var renderDiv = 'highchart-render-place';
            if ($('#' + renderDiv).length < 1) {
                renderDiv = '_container__<%= ClientID %>';
            }
            //check if we are currently in preview
            var isPreview = window.location.href.indexOf('Analysis') < 0;
            //increase bar size in this case
            if (isPreview) {
                $('#barchart__<%= ClientID %>').css('width', 1300);
            }
            //check if grid line should appear
            var gridLine = <%= Appearance["GridLine"].ToLower() %> ? 1 : 0;
            //data result
            var result = {
                'categories': [
                    <% for (var iCategory = 0; iCategory < Model.CalculateResult.Length; iCategory++)
                                                      {
                                                   %> <% if (iCategory > 0)
                                     { %>,<% } %>
                    htmlDecode('<%= EscapeJSStringConst(Wrap(Utilities.StripHtml(Model.CalculateResult[iCategory].ResultKey))) %>')
                    <% } %>
                ],
                'minmax': [
                    <% for (var iCategory = 0; iCategory < Model.CalculateResult.Length; iCategory++)
                                                  {
                                               %> <% if (iCategory > 0)
                                 { %>,<% } %>
                    {
                        min: <%= Model.CalculateResult[iCategory].MinValue %>,
                        max: <%= Model.CalculateResult[iCategory].MaxValue %>
                    }
                    <% } %>
                ],
                'valuesData': [
                    <% for (var iCategory = 0; iCategory < Model.CalculateResult.Length; iCategory++)
                                                      {
                                                   %> <% if (iCategory > 0)
                                     { %>,<% } %>
                    [
                        '<%= EscapeJSStringConst(Wrap(Model.CalculateResult[iCategory].ResultKey)) %>',
                        <%=  String.Format("{0:0.00}", Model.CalculateResult[iCategory].ResultValue)  %>
                    ]
                    <% } %>
                ],

                'maxRange': [<%= Model.CalculateResult.Any() && Model.CalculateResult.FirstOrDefault().Points != 0 ? Model.CalculateResult.FirstOrDefault().Points  : 10  %>],
                'sectionsCount': <%= Model.CalculateResult.Any() ?  Model.CalculateResult.Length : 0 %>

            };

            //if no data - do not render
            if (result.valuesData.length < 1) {
                return;
            }

            //function to get color regarding value/ depends on max rating value
            var getClass = function(number, maxValue) {
                var percentage = number / maxValue;

                if (percentage <= 1 && percentage >= 0.8)
                    return 'bar-success';
                if (percentage < 0.8 && percentage >= 0.4)
                    return 'bar-warning';
                else
                    return 'bar-error';
            };

            //minmax function implementation, also takes the square line size to draw it with dynamic size
            var applyMinMax = function(minmaxvalues, squareSideSize) {

                var minMaxTemplate =
                    "<div  class='range-line' style= 'position:absolute;' ><div class=\"bar-message ${minClass}\"><p class=\"bar-item\">${minVal}</p></div><div class=\"bar-message ${maxClass}\"><p class=\"bar-item\">${maxVal}</p></div></div>";

                var maxRatePossible = result.maxRange;

                for (var i = 0; i < minmaxvalues.length; i++) {
                    var min = minmaxvalues[i].min,
                        max = minmaxvalues[i].max;

                    var tmplObj = {
                        'minVal': min,
                        'maxVal': max,
                        'minClass': getClass(min, maxRatePossible),
                        'maxClass': getClass(max, maxRatePossible)
                    };

                    //appending template to minmaxcontainer
                    $.tmpl(minMaxTemplate, tmplObj).appendTo('#minmaxcontainer__<%= ClientID %>');
                }
                //if item size is greater than 79px that means that there is only one bar
                var isOneItem = (squareSideSize > 79);
                //setting max item size
                var squareSize = (isOneItem) ? 79 : squareSideSize;

                //setting minmax height/width
                if (result.sectionsCount < 5) {
                    $('#barchart__<%= ClientID %> .bar-message').css('width', squareSize).css('height', squareSize);
                }
                else{
                    $('#barchart__<%= ClientID %> .bar-message').css('width', squareSize * 2).css('height', squareSize);
                }

                var rangeLineSelector = "#barchart__<%= ClientID %> .range-line:not(:first)";

                $.each($(rangeLineSelector),
                    function(item) {
                        //getting position on each bar to set the minmax containers on same height
                        var barTop = $($('#barchart__<%= ClientID %> .highcharts-tracker').find('rect')[item]).offset().top;
                        var barChartTop = $('#barchart__<%= ClientID %>').offset().top;

                        $(this).css('top', barTop - barChartTop);
                    });

                if (result.sectionsCount > 2) {
                    $("#minmaxcontainer__<%= ClientID %>").css("width", 150);
                }

                var marginRight = Math.abs(parseInt($('#barchart__<%= ClientID %> .bar-message:first .bar-item').css("margin-right"), 10));

                var lowRangeIndicator = $("#barchart__<%= ClientID %> .minmax-category.low");

                $(lowRangeIndicator).css("margin-right", marginRight);


            };

            // Clean up default exporting btns 
            if (!isPreview) {
                Highcharts.getOptions().exporting.buttons.exportButton.menuItems = [];
            }

            var chartExportContainer = $("#barchart__<%= ClientID %>").parents("[id*='Wrapper']");

            chart = new Highcharts.Chart({
                chart: {
                    "type": "column",
                    "renderTo": renderDiv,
                    "inverted": true,
                    "polar": false,
                    "spacingRight": 20,
                    "style": {
                        "fontFamily":
                            "\"Lucida Grande\", \"Lucida Sans Unicode\", Verdana, Arial, Helvetica, sans-serif",
                        "color": "#333",
                        "fontSize": "12px",
                        "fontWeight": "bold",
                        "fontStyle": "normal"
                    },
                    "width": 800,
                    "height": isPreview ? 560 : 375
                },
                title: {
                    "text": "",
                    "style": {
                        "fontFamily":
                            "\"Lucida Grande\", \"Lucida Sans Unicode\", Verdana, Arial, Helvetica, sans-serif",
                        "color": "#333333",
                        "fontSize": "14px",
                        "fontWeight": "normal",
                        "fontStyle": "normal",
                        "fill": "#333333",
                        "width": "1160px"
                    }
                },
                subtitle: {
                    "text": ""
                },
                exporting: {
                    buttons: {
                        exportButton: {
                            enabled: (navigator.userAgent.indexOf("HiQPdf") == -1),
                            menuItems: [
                                {
                                    textKey: 'downloadPNG',
                                    onclick: function() {
                                        highchartCanvasExporting.exportChart(chartExportContainer, "#_container__<%= ClientID %>", "png");
                                    }
                                }, {
                                    textKey: 'downloadJPEG',
                                    onclick: function() {
                                        highchartCanvasExporting.exportChart(chartExportContainer, "#_container__<%= ClientID %>", "jpeg");
                                    }
                                },
                                {
                                    textKey: 'downloadPDF',
                                    onclick: function() {
                                        highchartCanvasExporting.exportChart(chartExportContainer, "#_container__<%= ClientID %>", "pdf");
                                    }
                                }
                            ]
                        },
                        printButton: {
                            enabled: false
                        }

                    }
                },
                yAxis: [
                    {
                        min: 1.0,
                        max: result.maxRange,
                        format: '{value:.2f}',
                        title: {
                            "style": {
                                "fontFamily": "Courier",
                                "color": "#666666",
                                "fontSize": "28px",
                                "fontWeight": "normal",
                                "fontStyle": "normal"
                            },
                            "text": ""
                        },
                        labels: {
                            formatter: function() {
                                return this.value + '.00';
                            },
                            style: {
                                "color": "#666666",
                                "fontFamily": '<%= Appearance["FontFamily"].ToLower() %>',
                                "fontSize": <%= Appearance["TitleFontSize"].ToLower() %> + "px"
                            }
                        },
                        "reversed": false,
                        "opposite": false,
                        "startOnTick": true,
                        "endOnTick": false,
                        gridLineWidth: gridLine,
                        tickInterval: 1,
                        showEmpty: true,
                        showFirstLabel: true,
                        showLastLabel: true
                    }
                ],
                xAxis: [
                    {
                        categories: result.categories,
                        title: {
                            "text": "",
                            "style": {
                                "color": "#666666",
                                "fontFamily": '<%= Appearance["FontFamily"].ToLower() %>',
                                "fontSize": <%= Appearance["TitleFontSize"].ToLower() %> + "px",
                                "fontWeight": "normal",
                                "fontStyle": "normal"
                            }
                        },

                        "opposite": false,
                        showEmpty: true,
                        type: "category",
                        tickInterval: 1,
                        gridLineWidth: gridLine,
                        showFirstLabel: true,
                        showLastLabel: true,
                        plotLines: [
                            {
                                color: '#C0C0C0',
                                width: 1,
                                value: Number('<%= Model.CalculateResult.Length%>') - 0.5
                            },
                            {
                                color: '#C0C0C0',
                                width: gridLine,
                                value: -0.5
                            }
                        ],
                        labels: {
                            style: {
                                "color": "#666666",
                                "fontFamily": '<%= Appearance["FontFamily"].ToLower() %>',
                                "fontSize": <%= Appearance["TitleFontSize"].ToLower() %> + "px"
                            }
                        }
                    }
                ],
                series: [
                    {
                        type: "column",
                        data: result.valuesData,

                        "name": "Category",
                        "turboThreshold": 0,
                        "_colorIndex": 0,
                        "color": '<%= Appearance["BarColor"] %>',
                        "negativeColor": "#fce4ec",
                        "colorByPoint": false,
                        "dashStyle": "Solid",
                        "marker": {
                            "symbol": "square"
                        }
                    }
                ],
                plotOptions: {
                    "series": {
                        "animation": true,
                        "dataLabels": {
                            formatter: function() {
                                if (this.y === 1) {
                                    return "";
                                }
                                return Number(this.y).toFixed(2);
                            },
                            "enabled": <%= Appearance["ShowValuesOnBars"].ToLower() %>,
                            "style": {
                                "color": "contrast",
                                "fontFamily": '<%= Appearance["FontFamily"].ToLower() %>',
                                "fontSize": <%= Appearance["TitleFontSize"].ToLower() %> + "px",
                                "fontWeight": "bold"
                            },
                            "inside": true,
                            x: -20,
                            y: 5,
                            zIndex: 10
                        }
                    }
                },
                legend: {
                    "enabled": false,
                    "layout": "vertical",
                    "align": "left"
                },
                credits: {
                    text: "",
                    href: ""
                }
            });

            //getting first bar to get it width
            //should by get after highcharts drawn
            var barWidth = $($('#barchart__<%= ClientID %> .highcharts-tracker').find('rect').first()).attr('width');

            applyMinMax(result.minmax, barWidth);

            $("#barchart__<%= ClientID %> .bar-item ").each(function() {
                barWidth = barWidth > 79 ? 79 : barWidth;
                $(this).css("line-height", barWidth + "px");
            });
        });
    });
</script>
<div id="consoleLog"> </div>
<div id="barchart__<%= ClientID %>" style="display: inline-flex;margin-right: 50px; margin-left: 50px; margin-top: 30px;">
    <div id="_container__<%= ClientID %>"  style="display: inline-block; position: relative; float: left;">
    </div>
     <%if (Model.CalculateResult.Any()) { %>
       <div id="minmaxcontainer__<%= ClientID %>" style="position: relative; width: 185px; text-align: center; display: inline-block; float: right;">
       <div id="range-container__<%= ClientID %>" style="margin-top: -30px; ">
            <div class="range-raiting-title" style="font-size: 14px; position: relative">
                <p><u>Range of Ratings</u></p>
            </div>
           
        </div>
         <div class="range-line">
             <div class="minmax-category low">
                 <div>Low</div>
             </div>
             <div class="minmax-category high">
                 <div>High</div>
             </div>
         </div>
    </div>
    <%} %>
 
</div>