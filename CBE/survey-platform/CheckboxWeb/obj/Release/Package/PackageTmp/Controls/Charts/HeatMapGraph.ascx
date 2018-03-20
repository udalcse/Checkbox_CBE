<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HeatMapGraph.ascx.cs" Inherits="CheckboxWeb.Controls.Charts.HeatMapGraph" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>

<script type="text/javascript">
    $(function() {
        var fullReport = [];
        var heatselector = "#heatmap__<%= ClientID %>";
        //reportSections
        var sections = <%= new JavaScriptSerializer().Serialize(Model.HeatMapAnalysisResult) %>;

        var appearenceStyles = {
            labelFont: '<%= Appearance["FontFamily"].ToLower() %>',
            labelSize: <%= Appearance["LableFontSize"].ToLower() %> + "px",
            gridLine: <%= Appearance["GridLine"].ToLower() %>,
            showLabels: <%= Appearance["RespondentLabels"].ToLower() %>
            };

        //counter user to itarate through sections
        var sectionCounter = 0;

        //parsing object from backend
        for (var section in sections.SectionsMeanValues) {

            var reportSection = {
                upperCount: '',
                leftCount: '',
                titleText: '',
                footerAvgValue: '',
                sectionCellsArray: []
            };

            reportSection.titleText = getRoman(sectionCounter+1);
            reportSection.leftCount = sections.Responses.length;
            reportSection.footerAvgValue = Math.round(sections.SectionsMeanValues[section] * 100)/ 100;

            reportSection.upperCount = sections.Responses[0].Sections[sectionCounter].Answers.length;

            for (var responseIndex = 0; responseIndex < sections.Responses.length; responseIndex++) {
                for (var answerIndex = 0;
                    answerIndex < sections.Responses[responseIndex].Sections[sectionCounter].Answers.length;
                    answerIndex++) {
                    reportSection.sectionCellsArray.push({
                        'cellValue': sections.Responses[responseIndex].Sections[sectionCounter].Answers[answerIndex]
                            .Points
                    });
                }
            }
            fullReport.push(reportSection);
            sectionCounter++;
        }

        //left column numeral array
        var leftCounterObjArray = [];
        //fake fill
        for (var j = 1; j <= sections.Responses.length; j++) {
            leftCounterObjArray.push({ 'leftRowNumber': j });
        }

        //1. compile main template
        //Table with Left content and overal structure
        (function() {
            $.template("mainTemplate",
                "<table class=\"mainHeatContent\">" +
                "<tr class=\"heatContentRow\">" +
                "<td style=\"vertical-align: middle;\">" +
                "<table class=\"leftColumn\">" +
                "<thead class=\"sectionCounter\">" +
                "<tr>" +
                "<td>" +
                "</td>" +
                "</tr>" +
                "</thead>" +
                "<tfoot>" +
                "<tr>" +
                "<td>" +
                "</td>" +
                "</tr>" +
                "</tfoot>" +
                "<tr class=\"sectionCounter\">" +
                "<td>" +
                "</td>" +
                "</tr>" +
                "</table>" +
                "</td>" +
                "</tr>" +
                "</table>");
            //render markup to heatmap div
            $.tmpl("mainTemplate").appendTo($(heatselector));
            //compile counterTemplate(left counter coulmn)
            $.template("counterTemplate",
                "<tr class=\"sectionCounter\"><td><p class=\"leftPanelNumber\">${leftRowNumber}</p></td></tr>");
            //render left column appending it into its place in main markup
            $.tmpl("counterTemplate", leftCounterObjArray).appendTo($(heatselector).find('.leftColumn').last());
        })();

       for (var i = 0; i < fullReport.length; i++) {
            renderSection(fullReport[i]);
        }
      
        function renderSection(reportSection) {
            //2. heat content table craft
            var sectionHeader = "<thead class=\"sectionHead\"><tr><td>" +
                reportSection
                .titleText +
                "</td></tr></thead>";

            var sectionFooter = "<tr class=\"sectionFoot\"><td>" +
                reportSection.footerAvgValue.toFixed(2) +
                "</td></tr>";

            var sectionCounter =
                "<tr id=\"upperCounter\"><td><table class=\"sectionGroup\"><tr class=\"sectionCounter\">" +
                    renderUpperCounter(reportSection.upperCount) +
                    "</tr></table></td></tr>";

            var sectionCells = "<tr><td id=\"coloredCells\"><table class=\"sectionGroup\">" +
                renderSectionRows(reportSection.leftCount, reportSection) +
                "</table></td></tr>";
            var sectionBody = "<tbody class=\"sectionBody\">" + sectionCounter + sectionCells + sectionFooter + "</tbody>";

            //Full compiulation
            var sectionCompile = "<td class=\"heatSection\"><table class=\"heatContent\">" +
                sectionHeader +
                sectionBody +
                "</table></td>";

            //3. appending full section to content holder
            $.tmpl(sectionCompile).appendTo($(heatselector).find('.heatContentRow').last());

            applyStyles();
        }

        //helper functions
        function renderUpperCounter(cycles) {
            var counterNumeralsMarkup = "";
            for (var i = 1; i <= cycles; i++) {
                counterNumeralsMarkup += "<td class=\"secCounter\"></td>";
            }

            return counterNumeralsMarkup;
        }

        function renderSectionRows(cycles, reportObject) {
            var sectionCellsMarkup = "";
            for (var i = 0; i < cycles; i++) {
                sectionCellsMarkup += "<tr>" + renderSectionRow(i, reportObject) + "</tr>";
            }

            return sectionCellsMarkup;
        }

        function renderSectionRow(digit, reportObject) {
            var sectionOneRowMarkup = "";
            var position = digit * reportObject.upperCount;
            var dataGroupLength = position + reportObject.upperCount;

            for (var i = position; i < dataGroupLength; i++) {
                sectionOneRowMarkup += "<td class=\"cell " +
                    getClass(reportObject.sectionCellsArray[i].cellValue, sections.MaxRatingRange) +
                    "\"><p class=\"hiddenCellValue\">" +
                    reportObject.sectionCellsArray[i].cellValue +
                    "</p></td>";
            }

            return sectionOneRowMarkup;
        }

        function getClass(number, maxValue) {
            var percentage = number / maxValue;

            if (percentage <= 1 && percentage >= 0.8)
                return 'bar-success';
            else if (percentage < 0.8 && percentage >= 0.4)
                return 'bar-warning';
            else if (percentage == 0)
                return 'bar-na-answer';
            else
                return 'bar-error';
        };

        function getRoman(num) {
            if (!+num)
                return "";
            var digits = String(+num).split(""),
                key = [
                    "", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM",
                    "", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC",
                    "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX"
                ],
                roman = "",
                i = 3;
            while (i--)
                roman = (key[+digits.pop() + (i * 10)] || "") + roman;
            return Array(+digits.join("") + 1).join("M") + roman;
        }

        function heightFix() {
            return;
            var theadHeaight = Math.round($('#heatmap__<%= ClientID %> .sectionHead').first().css('height').slice(0, -2))+5;
            $('.leftColumn > thead > tr').css('height', theadHeaight+'px');
        }

        function setAppearance() {
            //set fonts
            $(heatselector).find('.sectionHead, .sectionFoot, tr.sectionCounter *').css({'font-family' : appearenceStyles.labelFont,'font-size': appearenceStyles.labelSize, 'color' : 'black'});

            if (!appearenceStyles.gridLine) {
                $(heatselector).find('.sectionGroup tr').css('border', 0);
                $(heatselector).find('.sectionGroup td').css('border', 0);
                $(heatselector).find('td .secCounter').css('border-bottom', '1px solid');
            }

            if (!appearenceStyles.showLabels) {
                $(heatselector).find('.leftColumn').css('display', 'none');
            }
        }

        function applyStyles() {
            setAppearance();

            var labelScaling = <%= Appearance["LableFontSize"].ToLower() %>;

            setCellSize(labelScaling + 10);

            function setCellSize(cellBorderValue) {
                //left column cells
                $(heatselector).find('.leftColumn tr td ').css('width', cellBorderValue);
                $(heatselector).find('.leftColumn tr').css('height', cellBorderValue);
                //cestions cells
                $(heatselector).find('.sectionGroup tr').css('height', cellBorderValue);
                $(heatselector).find('.sectionGroup tr td ').css('width', cellBorderValue);
                //head and footer heigth
                $(heatselector).find('.heatContent td').css('height', cellBorderValue);
            }

            $.each($(heatselector).find('.secCounter'),
                function(index, elem) {
                    $(elem).text(index + 1);
                });

            //fix if theres only one column in reportable sectioon
            $.each($('.heatContent'),
                function() {
                    if ($(this.lastChild).find('.secCounter').length > 1)
                        return;

                    var width = $(this).width();
                    $(this).find('.sectionGroup tr td').css('width', width);
                });
        }

    });
</script>


<div id="container" style="display: inline-flex; margin-top: 3%;" class="heat-map-container">
    <div id="_container__<%= ClientID %>" style="display: inline-block; position: relative;">
        <div id="heatmap__<%= ClientID %>">
        </div>
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
</div>

<script>
    $(document).ready(function() {
        var heatMapHaveRows =  $('#_container__<%= ClientID %> .heatContentRow').children().size() > 1;

        if (navigator.userAgent.indexOf("HiQPdf") != -1 || !heatMapHaveRows) {
            $('#_container__<%= ClientID %> .highchart-export-menu-icon').hide();
        }
        $('#_container__<%= ClientID %> .highchart-export-menu-icon').click(function(event) {
            event.preventDefault();
            $('#_container__<%= ClientID %> .highchart-export-menu').toggleClass('visible');
        });

        $('#_container__<%= ClientID %> .highchart-export-menu-icon .menu-item').click(function() {
            var mode = $(this).data("mode");
            highchartCanvasExporting.exportChart("#_container__<%= ClientID %>", null, mode);
        });

        $("#_container__<%= ClientID %> .highchart-export-menu-icon").mouseleave(function(){
            if ($("#_container__<%= ClientID %> .highchart-export-menu").hasClass("visible")) {
                $("#_container__<%= ClientID %> .highchart-export-menu").toggleClass('visible');
            }
        });

    });
</script>