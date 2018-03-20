/*
 * --------------------------------------------------------------------
 * jQuery-Plugin - selectToUISlider - creates a UI slider component from a select element(s)
 * by Scott Jehl, scott@filamentgroup.com
 * http://www.filamentgroup.com
 * reference article: http://www.filamentgroup.com/lab/update_jquery_ui_16_slider_from_a_select_element/
 * demo page: http://www.filamentgroup.com/examples/slider_v2/index.html
 * 
 * Copyright (c) 2008 Filament Group, Inc
 * Dual licensed under the MIT (filamentgroup.com/examples/mit-license.txt) and GPL (filamentgroup.com/examples/gpl-license.txt) licenses.
 *
 * Usage Notes: please refer to our article above for documentation
 *  
 * --------------------------------------------------------------------
 */


jQuery.fn.selectToUISlider = function(settings) {
    var selects = jQuery(this);

    //accessible slider options
    var options = jQuery.extend({
        labels: 3, //number of visible labels
        tooltip: true, //show tooltips, boolean
        tooltipSrc: 'text', //accepts 'value' as well
        labelSrc: 'value', //accepts 'value' as well	,
        showImages: false, // show images instead of text
        applicationRoot: '',
        sliderOptions: null
    }, settings);

    //handle ID attrs - selects each need IDs for handles to find them
    var handleIds = (function() {
        var tempArr = [];
        selects.each(function() {
            tempArr.push('handle_' + jQuery(this).attr('id'));
        });
        return tempArr;
    })();

    //array of all option elements in select element (ignores optgroups)
    var selectOptions = (function() {
        var opts = [];
        selects.eq(0).find('option').each(function() {
            opts.push({
                value: jQuery(this).attr('value'),
                text: jQuery(this).text()
            });
        });
        return opts;
    })();

    //array of opt groups if present
    var groups = (function() {
        if (selects.eq(0).find('optgroup').size() > 0) {
            var groupedData = [];
            selects.eq(0).find('optgroup').each(function(i) {
                groupedData[i] = { };
                groupedData[i].label = jQuery(this).attr('label');
                groupedData[i].options = [];
                jQuery(this).find('option').each(function() {
                    groupedData[i].options.push({ text: jQuery(this).text(), value: jQuery(this).attr('value') });
                });
            });
            return groupedData;
        } else return null;
    })();

    //check if obj is array

    function isArray(obj) {
        return obj.constructor == Array;
    }

//return tooltip text from option index

    function ttText(optIndex) {
        if (!options.tooltip)
            return false;

        return (options.tooltipSrc == 'text') ? selectOptions[optIndex].text : selectOptions[optIndex].value;
    }

    var fireChangeEventCandidate = null;
    var leftButtonIsDown = false;
    $(document).mousedown(function(e) {
        if (e.which === 1) leftButtonIsDown = true;
    });
    $(document).mouseup(function(e) {
        if (e.which === 1) leftButtonIsDown = false;
        if (fireChangeEventCandidate) {
            fireChangeEventCandidate.change();
            fireChangeEventCandidate = null;
        }
    });

    //plugin-generated slider options (can be overridden)
    var sliderOptions = {
        step: 1,
        min: 0,
        orientation: 'horizontal',
        max: selectOptions.length - 1,
        range: selects.length > 1, //multiple select elements = true
        slide: function(e, ui) { //slide function
            var thisHandle = jQuery(ui.handle);
            //handle feedback 
            var textval = ttText(ui.value);
            thisHandle
            //    .attr('aria-valuetext', textval)
              //  .attr('aria-valuenow', ui.value)
                .find('.ui-slider-tooltip .ttContent')
                .text(textval);

            //control original select menu
            var currSelect = jQuery('#' + thisHandle.attr('id').split('handle_')[1]);

            currSelect.find('option').removeAttr('selected'); //deselect selected values!!!
            currSelect.find('option').eq(ui.value).attr('selected', 'selected');
            //trigger change event
            if (leftButtonIsDown) {
                fireChangeEventCandidate = currSelect;
            } else {
                fireChangeEventCandidate = null;
                currSelect.change();
            }
            //currSelect.change();
        },
        values: (function() {
            var vals = [];
            selects.each(function() {
                vals.push(jQuery(this).get(0).selectedIndex);
            });
            return vals;
        })()
    };

    //slider options from settings
    options.sliderOptions = (settings) ? jQuery.extend(sliderOptions, settings.sliderOptions) : sliderOptions;

    //create slider component div
    var sliderComponent = jQuery('<div></div>');

    var orientation = options.sliderOptions.orientation;

    //CREATE HANDLES
    selects.each(function(i) {
        var hidett = '';

        //associate label for ARIA
        var thisLabel = jQuery('label[for=' + jQuery(this).attr('id') + ']');
        //labelled by aria doesn't seem to work on slider handle. Using title attr as backup
        var labelText = (thisLabel.size() > 0) ? 'Slider control for ' + thisLabel.text() + '' : '';
        var thisLabelId = thisLabel.attr('id') || thisLabel.attr('id', 'label_' + handleIds[i]).attr('id');

        if (options.tooltip == false) {
            hidett = ' style="display: none;"';
        }
        jQuery('<a ' +
            'href="#" tabindex="0" ' +
            'id="' + handleIds[i] + '" ' +
            'class="ui-slider-handle" ' +
            'role="slider" ' +
            'aria-labelledby="' + thisLabelId + '" ' +
            'aria-valuemin="' + options.sliderOptions.min + '" ' +
            'aria-valuemax="' + options.sliderOptions.max + '" ' +
            'aria-valuenow="' + options.sliderOptions.values[i] + '" ' +
            'aria-valuetext="' + ttText(options.sliderOptions.values[i]) + '" ' +
            '><span class="screenReaderContext">' + labelText + '</span>' +
            '<span class="ui-slider-tooltip ui-widget-content-' + orientation + ' ui-corner-all"' + hidett + '><span class="ttContent"></span>' +
            '<span class="ui-tooltip-pointer-down ui-widget-content-' + orientation + '"><span class="ui-tooltip-pointer-down-inner"></span></span>' +
            '</span></a>')
            .data('handleNum', i)
            .appendTo(sliderComponent);
    });

    //CREATE SCALE AND TICS    
    var isVertical = orientation == 'vertical';
    var shiftStr = isVertical ? 'bottom:' : 'left:';

    //write dl if there are optgroups
    if (groups) {
        var inc = 0;
        var scale = sliderComponent.append('<dl class="ui-slider-scale ui-helper-reset ' + orientation + '" role="presentation"></dl>').find('.ui-slider-scale:eq(0)');
        jQuery(groups).each(function(h) {
            scale.append('<dt style="width: ' + (100 / groups.length).toFixed(2) + '%' + '; ' + shiftStr + (h / (groups.length - 1) * 100).toFixed(2) + '%' + '"><span>' + this.label + '</span></dt>'); //class name becomes camelCased label
            var groupOpts = this.options;
            jQuery(this.options).each(function(i) {
                var style = (inc == selectOptions.length - 1 || inc == 0) ? 'style="display: none;"' : '';
                var labelText = (options.labelSrc == 'text') ? groupOpts[i].text : groupOpts[i].value;
                scale.append('<dd style="' + shiftStr + leftVal(inc) + '"><span class="ui-slider-label">' + labelText + '</span><span class="ui-slider-tic ui-widget-content-' + orientation + '"' + style + '></span></dd>');
                inc++;
            });
        });
    }
        //write ol
    else {
        var ol = $('<ol class="ui-slider-scale ui-helper-reset ' + orientation + ' ' + options.sliderOptions.imagePosition + '-image-pos" role="presentation"' + (isVertical ? 'style="height:250px;"' : '') + '>');
        jQuery(selectOptions).each(function(i) {
            var style = (i == selectOptions.length - 1 || i == 0) ? 'style="display: none;"' : '';
            var imageIdAndAlias = this.text.split(',');
            var image = imageIdAndAlias[0];
            imageIdAndAlias.splice(0, 1);
            var alias = imageIdAndAlias.join();
            var labelText = (options.labelSrc == 'text') ? (options.showImages ? '<img src="' + options.applicationRoot + '/ViewContent.aspx?ContentId=' + image + '" alt="' + alias + '" /><div style="clear:both;"></div>' : this.text) : this.value;
            if (options.showImages && options.sliderOptions.aliasPosition != 'dontshow') {
                if (options.sliderOptions.aliasPosition == 'bottom') {
                    labelText = labelText + '<div class="alias-text ui-slider-label ui-slider-label-show">' + alias + '</div>';
                } else {
                    labelText = '<div class="alias-text ui-slider-label ui-slider-label-show">' + alias + '</div>' + labelText;
                }
            }
            ol.append('<li style="' + shiftStr + leftVal(i) + '"><div class="ui-slider-label">' + labelText + '</div><span class="ui-slider-tic ui-widget-content-' + orientation + '"' + style + '></span></li>');
        });
        sliderComponent = $('<div></div>').append(ol).append(sliderComponent);
    }

    function leftVal(i) {
        if (selectOptions.length == 1)
            return '0';
        return (i / (selectOptions.length - 1) * 100).toFixed(2) + '%';

    }

    //show and hide labels depending on labels pref
    //show the last one if there are more than 1 specified
    if (options.labels > 0) sliderComponent.find('.ui-slider-scale li:last span.ui-slider-label, .ui-slider-scale dd:last span.ui-slider-label').addClass('ui-slider-label-show');

    //set increment
    var increm = Math.max(1, Math.round(selectOptions.length / options.labels));
    //show em based on inc
    for (var j = 0; j < selectOptions.length; j += increm) {
        if ((selectOptions.length - j) > increm) { //don't show if it's too close to the end label
            sliderComponent.find('.ui-slider-scale li:eq(' + j + ') span.ui-slider-label, .ui-slider-scale dd:eq(' + j + ') span.ui-slider-label').addClass('ui-slider-label-show');
        }
    }

    //style the dt's
    sliderComponent.find('.ui-slider-scale dt').each(function(i) {
        jQuery(this).css({
            'left': ((100 / (groups.length)) * i).toFixed(2) + '%'
        });
    });

    //inject and return 
    sliderComponent
        .insertAfter(jQuery(this).eq(this.length - 1))
        .slider(options.sliderOptions)
        .attr('role', 'application');


    /////////////////////////////////////
    //Slider labels margins
    /////////////////////////////////////
    var valueSliderContainer = $(this).parent().parent();

    //calculate width of labels
    var innerContainer = valueSliderContainer.find('.ui-slider-container');
    var labelWidth = 0;
    if (innerContainer.length > 0) {
        var width = innerContainer.css('width').replace('px', '');
        labelWidth = width / options.labels;
        var label = valueSliderContainer.find('.ui-slider-label').css('width', labelWidth + 'px');
        //try/catch to avoid IE7 jquery issue with 'attr' function
        try {
            //fix for IE9 wrong alignment. if it fails for IE7 it doesn't affect anything
            label.attr('align', 'middle');
        }catch (e){}
    }
    //set left-margin for container
    valueSliderContainer.css("margin-left", valueSliderContainer.find('.ui-slider-label').first().width() / 2 + 'px');


    /////////////////////////////////////
    //Slider images margins
    /////////////////////////////////////
    fixSliderHeight(valueSliderContainer, labelWidth);

    //inject and return 
    sliderComponent
        .slider(options.sliderOptions)
        .attr('role', 'application')
        .find('.ui-slider-label')
        .each(function() {
            if ($(this).children('img').length == 0) {
                $(this).css('marginLeft', isVertical ? 40 : -$(this).width() / 2);
                //jQuery(this).css('marginTop', -jQuery(this).height() / 2);
            }
        });

    if (isVertical) {
        jQuery("span.ui-slider-label img").load(function() {
            jQuery(this).attr('style', 'position:relative; top: -' + jQuery(this).height() / 2 + 'px;');
        });
    }

    //update tooltip arrow inner color
    sliderComponent.find('.ui-tooltip-pointer-down-inner').each(function() {
        var bWidth = jQuery('.ui-tooltip-pointer-down-inner').css('borderTopWidth');
        var bColor = jQuery(this).parents('.ui-slider-tooltip').css('backgroundColor');
        jQuery(this).css('border-top', bWidth + ' solid ' + bColor);
    });

    var values = sliderComponent.slider('values');

    if (isArray(values)) {
        jQuery(values).each(function(i) {
            sliderComponent.find('.ui-slider-tooltip .ttContent').eq(i).text(ttText(this));
        });
    } else {
        sliderComponent.find('.ui-slider-tooltip .ttContent').eq(0).text(ttText(values));
    }

    return this;
};

var loadedSliders = {};

function fixSliderHeight(valueSliderContainers, labelWidth) {
    $.each(valueSliderContainers, function (index, valueSliderContainer) {
        var maxHeight = 0;

        var images = $(valueSliderContainer).find('img');
        if (images.length > 0) {
            if (typeof labelWidth == "undefined")
                return;

            var sliderHandleHeight = 12;
            //Do Firefox-related activities
            if (navigator.userAgent.toLowerCase().indexOf('firefox') > -1) {
                //for case if the slider width is null, we wait 100 ms and reload the slider
                if ($(valueSliderContainer).find('.ui-slider').width() == 0) {
                    setTimeout(function () {
                        fixSliderHeight(valueSliderContainers, labelWidth);
                    }, 100);
                    return;
                } else {
                    if (loadedSliders[$(valueSliderContainer).attr('id')]) {
                        //if the slider has been loaded already we should reload its images by force 
                        $.each(images, function (ind, img) {
                            onSliderImageLoad(img, sliderHandleHeight, maxHeight, images);
                        });
                    } else {
                        //if the slider hasn't been loaded yet, lets do it by the normal way as for other browsers
                        images.load(function () {
                            onSliderImageLoad(this, sliderHandleHeight, maxHeight, images);
                        });
                        //mark it
                        loadedSliders[$(valueSliderContainer).attr('id')] = true;
                    }
                }
            } else {
                images.load(function () {
                    onSliderImageLoad(this, sliderHandleHeight, maxHeight, images);
                });
            }

            images.css('max-width', $(valueSliderContainer).find('.ui-slider').width() / images.length + 'px');
            images.first().load(function () {
                var imageWidth = $(this).width();
                var marginLeft = imageWidth > labelWidth ? imageWidth : labelWidth;
                $(this).css('max-width', $(this).closest('.ui-slider').width() / images.length + 'px');
                $(this).closest('.slider-value-list-container').css("margin-left", marginLeft / 2 + 'px');
            });
        } else {
            //calculate min-height property by label height
            $.each($(valueSliderContainer).find('.ui-slider-label'), function (ind, elem) {
                var height = $(elem).height();
                if (height > maxHeight)
                    maxHeight = height;
            });
            $(valueSliderContainer).css('min-height', maxHeight + 'px');
        }
    });
}

function onSliderImageLoad(img, sliderHandleHeight, maxHeight, images) {
    $(img).css('max-width', $(img).closest('.ui-slider').width() / images.length + 'px');
    var sliderContainer = $(img).closest('.slider-value-list-container');
    var labelHeight = $(img).closest('.ui-slider-label').height();
    if (sliderContainer.find('ol').hasClass('top-image-pos')) {
        $(img).closest('.ui-slider-label').css("margin-top", -labelHeight - sliderHandleHeight * 2 + 'px');

        var marginTop = parseInt(sliderContainer.css("margin-top").replace('px', ''));
        if (marginTop < labelHeight) {
            sliderContainer.css("margin-top", labelHeight + 'px');
        }
    } else {
        if (labelHeight > maxHeight) {
            maxHeight = labelHeight;
            sliderContainer.css('min-height', maxHeight + 'px');
        }
        //add bottom padding
        sliderContainer.css('padding-bottom', '20px');
    }
}

jQuery.fn.toSlider = function() {
    this.val = function(value, max) {
        var offset = 0;
        if (max > 0) {
            offset = (value / max * 100).toFixed(2);
        }
        $(this).find('a.ui-slider-handle').css('left', offset + '%');
    };
    return this;
};
