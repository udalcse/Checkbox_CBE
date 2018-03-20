<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GradientColorDirectorSkillsMatrixChart.ascx.cs" Inherits="CheckboxWeb.Controls.Charts.GradientColorDirectorSkillsMatrixChart" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
<div id="_container__<%= ClientID %>" style="margin: auto; position: relative;" class="clearfix">
    <div id="headText" class="gradientLabels gradientText"></div>
    <div id="director__<%= ClientID %>">
         <%if (!Model.IsPreview) { %>
        <a href="#" class="highchart-export-menu-icon" data-html2canvas-ignore="true"> 
            <ul class="highchart-export-menu">
                <li class="menu-item" data-mode="png"><span>Download PNG image</span></li>
                <li class="menu-item" data-mode="jpeg"><span>Download JPEG image</span></li>
                <li class="menu-item" data-mode="pdf"><span>Download PDF document</span></li>
            </ul>
        </a>
          <%} %>

    </div>
    <div id="footText" class="gradientLabels gradientText"></div>
</div>
<script>
    $(document).ready(function() {
        var container = '#_container__<%= ClientID %>';
        var graphSelector = "#director__<%= ClientID %>"; // should be dynamic control
        var parentContainer = $(graphSelector).parent();

        var appearenceStyles = {
            labelFont: '<%= Appearance["FontFamily"].ToLower() %>',
            labelSize: '<%= Appearance["TitleFontSize"].ToLower() %>' + 'pt',
            gridLine: '<%= Appearance["GridLine"].ToLower() %>',
            showLabels: '<%= Appearance["RespondentLabels"].ToLower() %>',
            itemColumnHeader: '<%= Appearance["ItemColumnHeader"] %>',
            averagesColumnHeader: '<%= Appearance["AveragesColumnHeader"] %>',
            ratingDetailsHeader: '<%= Appearance["RatingDetailsHeader"] %>',
            summaryHeader: '<%= Appearance["SummaryHeader"] %>',
            directorAverages: '<%= Appearance["DirectorAverages"].ToLower() %>'
        };

        var colorsMaxFive = [
            {
                min: 1.0,
                max: 2.99,
                color: '#F8696B'
            }, {
                min: 3.00,
                max: 3.39,
                color: '#FA9171'
            }, {
                min: 3.40,
                max: 3.59,
                color: '#FDCF7D'
            }, {
                min: 3.60,
                max: 3.69,
                color: '#FEE481'
            }, {
                min: 3.70,
                max: 3.79,
                color: '#FFEB83'
            }, {
                min: 3.80,
                max: 3.89,
                color: '#F0E783'
            }, {
                min: 3.90,
                max: 3.99,
                color: '#E0E382'
            }, {
                min: 4.00,
                max: 4.39,
                color: '#B0D57F'
            }, {
                min: 4.40,
                max: 5.00,
                color: '#62BD7A'
            }
        ];

        var colorsMaxTen = [
            {
                min: 1.0,
                max: 7.09,
                color: '#F8696B'
            }, {
                min: 7.10,
                max: 7.19,
                color: '#FA9171'
            }, {
                min: 7.20,
                max: 7.99,
                color: '#FA9774'
            }, {
                min: 8.00,
                max: 8.29,
                color: '#FDCF7D'
            }, {
                min: 8.30,
                max: 8.39,
                color: '#FEE481'
            }, {
                min: 8.40,
                max: 8.49,
                color: '#FFEB83'
            }, {
                min: 8.50,
                max: 8.59,
                color: '#F0E783'
            }, {
                min: 8.60,
                max: 8.79,
                color: '#E0E382'
            }, {
                min: 8.80,
                max: 8.99,
                color: '#B0D57F'
            }, {
                min: 9.00,
                max: 9.29,
                color: '#82C77C'
            }, {
                min: 9.30,
                max: 10.00,
                color: '#62BD7A'
            }
        ];

        var model = <%= new JavaScriptSerializer().Serialize(Model.GradientColorDirectorMatrixResult) %>;

        if (model.Options.length < 1) {
            $(graphSelector).text('');
            return;
        }

        if (model.Respondents.length < 1) {
            $(graphSelector).text('No responses');
            return;
        }

        //mock objects
        (function() {
            $.template("mainTemplate",
                "<div class=\"graphTableSection\">" +
                "<div class=\"graphBodySection\">" +
                "</div>" +
                "</div>");
            //render markup to heatmap div
            $.tmpl("mainTemplate").appendTo(graphSelector);

            var bodySection = $(graphSelector).find('.graphBodySection');
            //compile counterTemplate(left counter coulmn
            //order is important
            bodySection.append("<div class=\"graphHeadRowSection\"></div>");
            //appending all data rows that would be
            for (var i = 0; i < model.Options.length; i++) {
                bodySection.append("<div class=\"graphRowSection\"></div>");
            }
            bodySection.append("<div class=\"graphFootRowSection\"></div>");
        })();
        //head
        $.each($(graphSelector).find('.graphHeadRowSection'),
            function() {
                var dataRow = '';
                //row data label
                dataRow += "<div class=\"graphTableCellSection graphRowDataLabel\"><p class=\"gradientText\">" +
                    appearenceStyles.itemColumnHeader + //title from appearence
                    "</p></div>";
                //average score from averege column
                dataRow += '<div class=\"graphTableCellSection graphDataAverage\"><p class=\"gradientText\">' +
                    appearenceStyles.averagesColumnHeader +
                    '<p></div>';

                //filling data cells
                for (var i = 0; i < model.Respondents.length; i++) {
                    dataRow += '<div class=\"graphTableCellSection headRowDataCounter gradientText\"></div>';
                }
                $(this).append(dataRow);
            });
        //body
        $.each($(graphSelector).find('.graphRowSection'),
            function(index) {
                var dataRow = '';
                //row data label
                dataRow += '<div class=\"graphTableCellSection graphRowDataLabel\"><p class=\"gradientText\">' +
                    model.Options[index].OptionTitle +
                    '</p></div>';
                //average score from averege column
                dataRow +=
                    '<div class=\"graphTableCellSection graphDataAverage gradientText\" style=\"background:' +
                    getColor(model.Options[index].AverageScore) +
                    ';\">' +
                    model.Options[index].AverageScore.toFixed(2) +
                    '</div>';
                //filling data cells
                for (var i = 0; i < model.Respondents.length; i++) {
                    var cell = model.Respondents[i].AnswerScore[index];
                    dataRow +=
                        '<div class=\"graphTableCellSection graphDataCell gradientText\" style=\"background:' +
                        getColor(cell) +
                        ';\">' +
                        cell +
                        '</div>';
                }
                $(this).append(dataRow);
            });
        //footer
        $.each($(graphSelector).find('.graphFootRowSection'),
            function() {
                var dataRow = '';
                //row data label
                dataRow += '<div class=\"graphTableCellSection graphRowDataLabel gradientText\"></div>';
                //average score from averege column
                dataRow += '<div class=\"graphTableCellSection graphDataAverage gradientText\">' +
                    model.OptionAverage.toFixed(2) +
                    '</div>';
                //filling data cells
                for (var i = 0; i < model.Respondents.length; i++) {
                    var cell = model.Respondents[i].AverageByDirector.toFixed(2);
                    dataRow += '<div class=\"graphTableCellSection graphDataCell\"><p class=\"gradientText\">' +
                        cell +
                        '</p></div>';
                }

                $(this).append(dataRow);
            });

        //upper counter
        $.each($(graphSelector).find('.headRowDataCounter'),
            function(index, elem) {
                $(elem).text(index + 1);
            });

        //set borders
        $.each($(graphSelector).find('.graphBodySection').children(),
            function() {
                $(this).children().last().css('border-right', '1px solid black');
            });

        //Apply styles
        $.each($('.gradientText'),
            function() {
                $(this).css({
                    'font-family': appearenceStyles.labelFont,
                    'font-size': appearenceStyles.labelSize.toString()
                });
            });

        //word-wrapping for header titles
        $.each($(graphSelector).find('.graphHeadRowSection p.gradientText'),
            function() {
                $(this).css('max-width', $(this).width() / 2 + 10);
            });

        //getting & setting parent control width as  graph
        var graphWidth = $($(graphSelector).find('.graphTableSection')).width();
        $(parentContainer).css('width', graphWidth);
        //count position for foot and head text
        var headFoot = [
            {
                item: ('#headText'),
                text: appearenceStyles.ratingDetailsHeader
            },
            {
                item: ('#footText'),
                text: appearenceStyles.summaryHeader
            }
        ];

        //setting text and block width for graph titles
        $.each(headFoot,
            function(selector, obj) {
                var item = $(parentContainer).find(obj.item);
                var widthValue = countWidth();
                $(item).text(obj.text);
                $(item).css('max-width', widthValue).css('width', widthValue);
            });

        //should the averages be displayed?
        //foot average row visability
        $(graphSelector).find('.graphFootRowSection .graphDataCell p')
            .css('visibility', appearenceStyles.directorAverages === 'true' ? 'normal' : 'hidden');

        //adding gridlines
        if (appearenceStyles.gridLine === 'true')
            $(graphSelector)
                .find('.graphTableCellSection.graphDataCell.gradientText, .headRowDataCounter, .graphFootRowSection .graphTableCellSection').css('border-right', '1px solid');

        //get pallete depending on maxrange
        function GetPallette() {
            return model.MaxScaleRatingRange < 6 ? colorsMaxFive : colorsMaxTen;
        }

        function getColor(value) {
            var between = function(min, max) {
                return value >= min && value <= max;
            };

            for (var item in GetPallette()) {
                if (between(GetPallette()[item].min, GetPallette()[item].max))
                    return GetPallette()[item].color;
            }
        }

        function countWidth() {
            this.value = 0;

            $.each($(graphSelector).find('.headRowDataCounter'),
                function() {
                    value += $(this).width() + 10; //10 for padding
                });
            return value;
        }

        $(container + ' .highchart-export-menu-icon').click(function(event) {
            event.preventDefault();
            $(container + ' .highchart-export-menu').toggleClass('visible');
        });

        $(container + ' .highchart-export-menu-icon .menu-item').click(function() {
            highchartCanvasExporting.exportChart(container, null, $(this).data("mode"));
        });

        $(container + ' .highchart-export-menu-icon').mouseleave(function() {
            if ($(container + ' .highchart-export-menu').hasClass("visible")) {
                $(container + ' .highchart-export-menu').toggleClass('visible');
            }
        });

        if (navigator.userAgent.indexOf("HiQPdf") != -1) {
            $(graphSelector).find(".highchart-export-menu-icon").hide();
        }

        // if there are only few answers then set absolute positioning for labels
        if(model.Respondents.length <= 5){
            var freeSpaceInTableHeader = 25;
            var headerLabelHeight = $("#headText").height();
            $("#headText").css({
                "top": -1 * (headerLabelHeight - freeSpaceInTableHeader) + "px",
                "position" : "absolute",
                "right": "0px"
            });
            $("[id$='_gradientColorDirectorSkillsMatrixChart']").css("top", headerLabelHeight- freeSpaceInTableHeader + "px");
            $("[id$='_gradientColorDirectorSkillsMatrixChart']").parent().css("margin-bottom", headerLabelHeight- freeSpaceInTableHeader + "px");
        }
    });

</script>