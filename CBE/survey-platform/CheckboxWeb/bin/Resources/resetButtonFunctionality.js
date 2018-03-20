function bindResetButton(selector, isMobile) {
    $(selector).attr('onclick', 'javascript:void(0);').on('click', function () {
        //reset checkboxes, radiobuttons
        if (isMobile) {
            $.each($('form input[type=radio], form input[type=checkbox]'), function (ind, elem) {
                var isDefault = $(elem).attr('dataDefaultValue') == 'checked';
                $(elem).prop('checked', isDefault).checkboxradio("refresh");
            });
        } else {
            $.each($('form input[type=checkbox], form input[type=radio]'), function(ind, elem) {
                if ($(elem).attr('dataDefaultValue') == 'checked')
                    $(elem).prop('checked', true).parent().addClass('checked');
                else
                    $(elem).prop('checked', false).parent().removeClass('checked');
            });
        }
        //reset dropdowns
        $.each($('form .selector select, form .ui-select select'), function (ind, elem) {
            var firstElem = $(elem).children('option').first();
            var defVal = firstElem.val();
            var defaultOption = $(elem).children('option[dataDefaultValue=selected]');
            if (defaultOption.length > 0) {
                defVal = defaultOption.val();
            }
            $(elem).val(defVal);
            if (typeof ($.uniform) != 'undefined') 
                $.uniform.update('#' + $(elem).attr('id'));

            if (typeof ($.mobile) != 'undefined') 
                $(elem).selectmenu("refresh");
        });
        //hide other texts
        $(".otherText").hide();
        //reset sinle-line textbox
        $.each($('form input[type=text],form input[type=date],form input[type=tel],form input[type=number]'), function (ind, elem) {
            $(elem).val($(elem).attr('dataDefaultValue'));
        });
        //reset multi-line textbox
        $.each($('form .itemContent .inputContainer textarea'), function (ind, elem) {
            var defText = $(elem).attr('dataDefaultValue');
            $(elem).val(defText);
            if (typeof (tinymce) != 'undefined')
                tinymce.get($(elem).attr('id')).setContent(defText);
        });

        //reset sliders
        if (isMobile) {
            resetSlider('form .vertical-mobile-text-slider-container', true);

            $.each($('form .mobile-numeric-slider-container, form .matrix-slider'), function (ind, elem) {
                var defaultVal = parseInt($(elem).find('.sliderDefaultValues').attr('dataDefaultValue'));
                $(elem).find('[data-type="range"]').val(defaultVal);
                $(elem).find('[data-type="range"]').slider('refresh');

                //set value
                $(elem).find('input[type="text"]').val(defaultVal);
            });
        } else {
            resetSlider('form .slider-value-list-container, form .matrix-slider', false);
        }
        //reset file uploads
        $.each($('form input[type=file]'), function (ind, elem) {
            $(elem).val(null).trigger('file-upload-reset');
        });
        //reset rankorders
        $.each($('div .inputContainer'), function (ind, elem) {
            var src = $(elem).find('#' + elem.id.replace('inputPanel', 'selectableDragnDrop_rankOrder'));
            var dest = $(elem).find('#' + elem.id.replace('inputPanel', 'selectableDragnDrop_rankOrderSelected'));
            //selectable dragndrop rank order
            if (src.length && dest.length) {
                var toSrc = dest.find('> div');
                toSrc.appendTo(src);

                src.children().sort(function (a, b) {
                    var aa = parseInt($(a).attr('num'));
                    var bb = parseInt($(b).attr('num'));
                    return (aa < bb) ? -1 : (aa > bb) ? 1 : 0;
                }).appendTo(src);
            }
            //simple dragndrop rank order
            src = $(elem).find('#' + elem.id.replace('inputPanel', 'rankOrderDragNDrop_rankOrder'));
            if (src.length) {
                src.children().sort(function (a, b) {
                    var aa = parseInt($(a).attr('num'));
                    var bb = parseInt($(b).attr('num'));
                    return (aa < bb) ? -1 : (aa > bb) ? 1 : 0;
                }).appendTo(src);
                eval("onListOrderUpdate_" + src.attr("id").replace("Drop_rankOrder", "Drop") + "();");
            }
        });
        //refresh the page if SPC is available
        if (typeof (surveyWorkflow) != 'undefined' && surveyWorkflow.IsSpcEnabled) 
            surveyWorkflow.makePostAction('spc');

        return false;
    });
}

function resetSlider(selector, isMobile) {
    $.each($(selector), function (ind, elem) {
        var maxVal = $(elem).find('.sliderDefaultValues').attr('dataMaxValue');
        var defaultId = $(elem).find('.sliderDefaultValues').attr('dataDefaultId');
        var defaultVal = $(elem).find('.sliderDefaultValues').attr('dataDefaultValue');
        if (isMobile) {
            $(elem).find('select').val(defaultId).trigger('reset', [defaultVal]);
        } else {
            $(elem).toSlider().val(defaultVal, maxVal);
            $(elem).find('.sliderSelectedValue').val(defaultVal);
            $(elem).find('.sliderSelectedValue').text(defaultVal);
            $(elem).find('input[type=text]').val(defaultVal);
            $(elem).find('select').val(defaultId);
        }
    });
}
