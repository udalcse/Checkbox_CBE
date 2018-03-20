var highchartCanvasExporting = (function () {

    //download file data
    var download = function(data, filename) {
        var a = document.createElement('a');
        a.download = filename;
        a.href = data;
        document.body.appendChild(a);
        a.click();
        a.remove();
    };

    var replaceSvg = function (svgContainer) {

        var svgElements = $(svgContainer).find('svg');

        //replace all svgs with a temp canvas
        svgElements.each(function () {
            var canvas, xml;

            // canvg doesn't cope very well with em font sizes so find the calculated size in pixels and replace it in the element.
            $.each($(this).find('[style*=em]'), function (index, el) {
                $(this).css('font-size', getStyle(el, 'font-size'));
            });

            canvas = document.createElement("canvas");
            canvas.className = "screenShotTempCanvas";
            //convert SVG into a XML string
            xml = (new XMLSerializer()).serializeToString(this);

            // Removing the name space as IE throws an error
            xml = xml.replace(/xmlns=\"http:\/\/www\.w3\.org\/2000\/svg\"/, '');

            //draw the SVG onto a canvas
            canvg(canvas, xml);
            $(canvas).insertAfter(this);
            //hide the SVG element
            $(this).attr('class', 'tempHide');
            $(this).hide();
        });

    };

    
    var exportChart = function (exportContainer, svgContainer, format) {


        if (svgContainer) {
       
            //hide exporting btn, this is working with bar graph and need to be tested with all other types of chart
            $(exportContainer).find(".highcharts-tracker").next().hide().
                next().hide();
            replaceSvg(svgContainer);
        }

        //var containerWidth = $(exportContainer).width();
        //var containerHight = $(exportContainer).width();

        html2canvas($(exportContainer), {
            background: '#FFFFFF',
            taintTest: true,
            allowTaint : true,
            onrendered: function (canvas) {

                canvas.webkitImageSmoothingEnabled = false;
                canvas.mozImageSmoothingEnabled = false;
                canvas.imageSmoothingEnabled = false;

                if (format === "pdf") {
                    var pdfData = canvas.toDataURL('image/jpeg');
                    var doc = new jsPDF("l", "mm", "a4");
                    doc.addImage(pdfData, 'JPEG', 10, 10);
                    doc.save('chart.pdf');
                } else {
                    var dataFormat = "";

                    switch (format) {
                    case "jpeg":
                        dataFormat = "image/jpeg";
                        break;
                    case "png":
                        dataFormat = "image/png";
                        break;
                    default:
                        throw new Error("Not supported data format")
                    }

                    var imgData = canvas.toDataURL(dataFormat);
                    download(imgData, "chart" + '.' + format);
                }

               
                    $(svgContainer).find('.screenShotTempCanvas').remove();
                    $(svgContainer).find('.tempHide').show().removeClass('tempHide');

                    //show exporting button
                    $(svgContainer + " .highcharts-tracker").next().show().
                        next().show();
               
            }
        });
    }

    return {
        exportChart: exportChart
    };

})();