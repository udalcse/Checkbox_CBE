( function( $, undefined ) {

$.widget("mobile.verticalSlider", $.mobile.widget, $.extend({
    widgetEventPrefix: "verticalSlider",

    options: {
        theme: null,
        trackTheme: null,
        disabled: false,
        initSelector: "[type='vertical-range']",
        mini: false,
        highlight: false,
        image: false,
        applicationRoot: '',
        showAliases: false
    },

    _create: function () {

        // TODO: Each of these should have comments explain what they're for
        var self = this,
			control = this.element,
			parentTheme = $.mobile.getInheritedTheme(control, "c"),
			theme = this.options.theme || parentTheme,
			trackTheme = this.options.trackTheme || parentTheme,
			cType = control[0].nodeName.toLowerCase(),
			isRangeslider = control.parent().is(":jqmData(role='rangeslider')"),
			selectClass = "",
			controlID = control.attr("id"),
			$label = $("[for='" + controlID + "']"),
			labelID = $label.attr("id") || controlID + "-label",
			label = $label.attr("id", labelID),
			min = !false ? parseFloat(control.attr("min")) : 0,
			max = !false ? parseFloat(control.attr("max")) : control.find("option").length - 1,
            step = window.parseFloat(control.attr("step") || 1),
			miniClass = (this.options.mini || control.jqmData("mini")) ? " ui-mini" : "",
			domHandle = document.createElement("a"),
			handle = $(domHandle),
			domSlider = document.createElement("div"),
			slider = $(domSlider),
			valuebg = this.options.highlight && !false ? (function () {
			    var bg = document.createElement("div");
			    bg.className = "ui-slider-bg " + $.mobile.activeBtnClass + " ui-btn-corner-all";
			    return $(bg).prependTo(slider);
			})() : false,
			options,
			wrapper;

        $(control).hide();

        //check options in attributes
        var attr = $(control).attr('data-images');
        if (typeof attr !== 'undefined' && attr !== false) {
            this.options.image = attr === 'true';
        }
        
        //check options in attributes
        attr = $(control).attr('data-approot');
        if (typeof attr !== 'undefined' && attr !== false) {
            this.options.applicationRoot = attr;
        }
        
        //check options in attributes
        attr = $(control).attr('data-show-aliases');
        if (typeof attr !== 'undefined' && attr !== false) {
            this.options.showAliases = attr === 'true';
        }
        
        domHandle.setAttribute("href", "#");
        domSlider.setAttribute("role", "application");
        domSlider.className = ["ui-slider-vertical-track ", selectClass, " ui-btn-down-", trackTheme, " ui-btn-corner-all", miniClass].join("");
        domHandle.className = "ui-slider-handle-vertical";
        domSlider.appendChild(domHandle);

        handle.buttonMarkup({ corners: true, theme: theme, shadow: true })
				.attr({
				    "role": "slider",
				    "aria-valuemin": min,
				    "aria-valuemax": max,
				    "aria-valuenow": $(control)[0].selectedIndex,
				    "aria-valuetext": $(control)[0].selectedIndex,
				    //"title": $(control)[0].selectedIndex,
				    "aria-labelledby": labelID
				});

        $.extend(this, {
            slider: slider,
            handle: handle,
            type: cType,
            step: step,
            max: max,
            min: min,
            valuebg: valuebg,
            isRangeslider: isRangeslider,
            dragging: false,
            beforeStart: null,
            userModified: false,
            mouseMoved: false
        });

        label.addClass("ui-slider");

        // monitor the input for updated values
        control.addClass("ui-slider-input");

        this._on(control, {
			"reset": "_controlReset",
            "change": "_controlChange",
            "keyup": "_controlKeyup",
            "blur": "_controlBlur",
            "vmouseup": "_controlVMouseUp"
        });

        slider.bind("vmousedown", $.proxy(this._sliderVMouseDown, this))
			.bind("vclick", false);

        // We have to instantiate a new function object for the unbind to work properly
        // since the method itself is defined in the prototype (causing it to unbind everything)
        this._on(document, { "vmousemove": "_preventDocumentDrag" });
        this._on(slider.add(document), { "vmouseup": "_sliderVMouseUp" });

        slider.insertAfter(control);

        // wrap in a div for styling purposes
        if (!false && !isRangeslider) {
            wrapper = this.options.mini ? "<div class='ui-slider ui-mini'>" : "<div class='ui-slider'>";

            control.add(slider).wrapAll(wrapper);
        }

        // bind the handle event callbacks and set the context to the widget instance
        this._on(this.handle, {
            "vmousedown": "_handleVMouseDown",
            "keydown": "_handleKeydown",
            "keyup": "_handleKeyup"
        });

        this.handle.bind("vclick", false);

        this._handleFormReset();

        this.refresh($(control)[0].selectedIndex, undefined, true);
    },
    
    _controlReset: function (event, defaultVal) {
        this.refresh(defaultVal, undefined, true);
    },

    _controlClick: function (event) {
        var value = $(event.target).parent().attr('data-option-value');
        var index = $(event.target).parent().attr('data-option-index');
        $(this.element).val(value);
        this.refresh(index, undefined, true);
        $(this.element).change();
    },

    _controlChange: function (event) {
        // if the user dragged the handle, the "change" event was triggered from inside refresh(); don't call refresh() again
        if (this._trigger("controlchange", event) === false) {
            return false;
        }
        if (!this.mouseMoved) {
            this.refresh((this.element)[0].selectedIndex, true);
        }
        //update select item on slider change
        var index = $(this.element).parent().find('.ui-slider-handle-vertical').attr('aria-valuenow');
        $(this.element)[0].selectedIndex = index;
    },

    _controlKeyup: function (event) { // necessary?
        this.refresh(this._value(), true, true);
    },

    _controlBlur: function (event) {
        this.refresh(this._value(), true);
    },

    // it appears the clicking the up and down buttons in chrome on
    // range/number inputs doesn't trigger a change until the field is
    // blurred. Here we check thif the value has changed and refresh
    _controlVMouseUp: function (event) {
        this._checkedRefresh();
    },

    // NOTE force focus on handle
    _handleVMouseDown: function (event) {
        this.handle.focus();
    },

    _handleKeydown: function (event) {
        var index = this._value();
        if (this.options.disabled) {
            return;
        }

        // In all cases prevent the default and mark the handle as active
        switch (event.keyCode) {
            case $.mobile.keyCode.HOME:
            case $.mobile.keyCode.END:
            case $.mobile.keyCode.PAGE_UP:
            case $.mobile.keyCode.PAGE_DOWN:
            case $.mobile.keyCode.UP:
            case $.mobile.keyCode.RIGHT:
            case $.mobile.keyCode.DOWN:
            case $.mobile.keyCode.LEFT:
                event.preventDefault();

                if (!this._keySliding) {
                    this._keySliding = true;
                    this.handle.addClass("ui-state-active");
                }

                break;
        }

        // move the slider according to the keypress
        switch (event.keyCode) {
            case $.mobile.keyCode.HOME:
                this.refresh(this.min);
                break;
            case $.mobile.keyCode.END:
                this.refresh(this.max);
                break;
            case $.mobile.keyCode.PAGE_UP:
            case $.mobile.keyCode.UP:
            case $.mobile.keyCode.RIGHT:
                this.refresh(index + this.step);
                break;
            case $.mobile.keyCode.PAGE_DOWN:
            case $.mobile.keyCode.DOWN:
            case $.mobile.keyCode.LEFT:
                this.refresh(index - this.step);
                break;
        }
    }, // remove active mark

    _handleKeyup: function (event) {
        if (this._keySliding) {
            this._keySliding = false;
            this.handle.removeClass("ui-state-active");
        }
    },

    _sliderVMouseDown: function (event) {
        // NOTE: we don't do this in refresh because we still want to
        //       support programmatic alteration of disabled inputs
        if (this.options.disabled || !(event.which === 1 || event.which === 0 || event.which === undefined)) {
            return false;
        }
        if (this._trigger("beforestart", event) === false) {
            return false;
        }
        this.dragging = true;
        this.userModified = false;
        this.mouseMoved = false;

        this.refresh(event);
        this._trigger("start");
        return false;
    },

    _sliderVMouseUp: function () {
        if (this.dragging) {
            this.dragging = false;

            this.mouseMoved = false;
            this._trigger("stop");
            return false;
        }
    },

    _preventDocumentDrag: function (event) {
        // NOTE: we don't do this in refresh because we still want to
        //       support programmatic alteration of disabled inputs
        if (this._trigger("drag", event) === false) {
            return false;
        }
        if (this.dragging && !this.options.disabled) {

            // this.mouseMoved must be updated before refresh() because it will be used in the control "change" event
            this.mouseMoved = true;

            this.refresh(event);

            // only after refresh() you can calculate this.userModified
            this.userModified = this.beforeStart !== this.element[0].selectedIndex;
            return false;
        }
    },

    _checkedRefresh: function () {
        if (this.value !== this._value()) {
            this.refresh(this._value());
        }
    },

    _value: function () {
        return parseFloat(this.element.val());
    },


    _reset: function () {
      //  this.refresh(undefined, false, true);
    },

    refresh: function (val, isfromControl, preventInputUpdate) {
        // NOTE: we don't return here because we want to support programmatic
        //       alteration of the input value, which should still update the slider

        var self = this,
			parentTheme = $.mobile.getInheritedTheme(this.element, "c"),
			theme = this.options.theme || parentTheme,
			trackTheme = this.options.trackTheme || parentTheme,
			left, width, data, tol;

        self.slider[0].className = ["ui-slider-vertical-track", " ui-btn-down-" + trackTheme, ' ui-btn-corner-all', (this.options.mini) ? " ui-mini" : ""].join("");
        if (this.options.disabled || this.element.attr("disabled")) {
            this.disable();
        }

        var slider = $(this.slider).parent().parent();
        var image = this.options.image;

        //vertical step between options
        var optionStep = image ? 60 : 40;

        var optLen = $(this.element).find('option').length - 1;
        
        //create options only one time to prevent image slider blinking
        if (slider.find('.mobileTextSliderOptions div').length == 0) {
            //array of all option elements in select element
            var opts = [];
            $(this.element).find('option').each(function() {
                var opt = new Object();
                opt.value = $(this).attr('value');
                if (image) {
                    var imageIdAndAlias = $(this).text().split(',');
                    opt.image = imageIdAndAlias[0];
                    imageIdAndAlias.splice(0, 1);
                    opt.text = imageIdAndAlias.join();
                } else {
                    opt.text = $(this).text();
                }
                opts.push(opt);
            });

            //detach old options and attach a new one
            slider.find('.mobileTextSliderOptions').detach();
            slider.append('<div class="mobileTextSliderOptions"></div>');
            var optionsDiv = slider.find('.mobileTextSliderOptions');

            var applicationRoot = this.options.applicationRoot;
            var showAliases = this.options.showAliases;
            $.each(opts, function (ind, elem) {
                var option = optionsDiv.append('<div data-option-value="' + elem.value + '" data-option-index="' + ind + '" style="top:' + ind * 100 / optLen + '%;"></div>').find('div').last();
                if (image) {
                    option.append('<img src="' + applicationRoot + '/ViewContent.aspx?ContentId=' + elem.image + '" alt="' + elem.text + '"></img>');
                }
                if (showAliases)
                    option.append('<span class="ui-slider-label Answer" >' + elem.text + '</span>');
            });
            
            //bind labels
            this._on(slider.find('.mobileTextSliderOptions img, .mobileTextSliderOptions span'), {
                "click": "_controlClick"
            });
        }
        //calculate height of slider track 
        $(this.slider).css('height', ' ' + optLen * optionStep + 'px');
        $(this.slider).parent().parent().css('height', ' ' + optLen * optionStep + 'px');

        // set the stored value for comparison later
        this.value = this._value();
        if (this.options.highlight && !false && this.slider.find(".ui-slider-bg").length === 0) {
            this.valuebg = (function () {
                var bg = document.createElement("div");
                bg.className = "ui-slider-bg " + $.mobile.activeBtnClass + " ui-btn-corner-all";
                return $(bg).prependTo(self.slider);
            })();
        }
        this.handle.buttonMarkup({ corners: true, theme: theme, shadow: true });

        var pxStep, percent,
			control = this.element,
			isInput = !false,
			optionElements = isInput ? [] : control.find("option"),
			min = 0,
			max = optLen,
			step = 1;

        if (typeof val === "object") {
            data = val;
            // a slight tolerance helped get to the ends of the slider
            tol = 8;

          //  left = this.slider.offset().left;
            var top = this.slider.offset().top;
            width = this.slider.width();
            var height = this.slider.height();
            pxStep = height / ((max - min) / step);
            if (!this.dragging ||
					data.pageY < top - tol ||
					data.pageY > top + height + tol) {
                return;
            }
            if (pxStep > 1) {
                percent = ((data.pageY - top) / height) * 100;
            } else {
                percent = Math.round(((data.pageY - top) / height) * 100);
            }
        } else {
            if (val == null) {
                val = isInput ? parseFloat(control.val() || 0) : control[0].selectedIndex;
            }
            percent = (parseFloat(val) - min) / (max - min) * 100;
        }

        if (isNaN(percent)) {
            return;
        }

        var newval = (percent / 100) * (max - min) + min;

        //from jQuery UI slider, the following source will round to the nearest step
        var valModStep = (newval - min) % step;
        var alignValue = newval - valModStep;

        if (Math.abs(valModStep) * 2 >= step) {
            alignValue += (valModStep > 0) ? step : (-step);
        }

        var percentPerStep = 100 / ((max - min) / step);
        // Since JavaScript has problems with large floats, round
        // the final value to 5 digits after the decimal point (see jQueryUI: #4124)
        newval = parseFloat(alignValue.toFixed(5));

        if (typeof pxStep === "undefined") {
            pxStep = width / ((max - min) / step);
        }
        if (pxStep > 1 && isInput) {
            percent = (newval - min) * percentPerStep * (1 / step);
        }
        if (percent < 0) {
            percent = 0;
        }

        if (percent > 100) {
            percent = 100;
        }

        if (newval < min) {
            newval = min;
        }

        if (newval > max) {
            newval = max;
        }

        this.handle.css("top", percent + "%");
        this.handle[0].setAttribute("aria-valuenow", isInput ? newval : optionElements.eq(newval).attr("value"));
        this.handle[0].setAttribute("aria-valuetext", isInput ? newval : optionElements.eq(newval).getEncodedText());
        //this.handle[0].setAttribute("title", isInput ? newval : optionElements.eq(newval).getEncodedText());

        if (this.valuebg) {
            this.valuebg.css("width", percent + "%");
        }

        // drag the label widths
        if (this._labels) {
            var handlePercent = this.handle.width() / this.slider.width() * 100,
				aPercent = percent && handlePercent + (100 - handlePercent) * percent / 100,
				bPercent = percent === 100 ? 0 : Math.min(handlePercent + 100 - aPercent, 100);

            this._labels.each(function () {
                var ab = $(this).is(".ui-slider-label-a");
                $(this).width((ab ? aPercent : bPercent) + "%");
            });
        }

        if (!preventInputUpdate) {
            var valueChanged = false;

            // update control"s value
            if (isInput) {
                valueChanged = control.val() !== newval;
                control.val(newval);
            } else {
                valueChanged = control[0].selectedIndex !== newval;
                control[0].selectedIndex = newval;
            }
            if (this._trigger("beforechange", val) === false) {
                return false;
            }
            if (!isfromControl && valueChanged) {
                control.trigger("change");
            }
        }
    },

    enable: function () {
        this.element.attr("disabled", false);
        this.slider.removeClass("ui-disabled").attr("aria-disabled", false);
        return this._setOption("disabled", false);
    },

    disable: function () {
        this.element.attr("disabled", true);
        this.slider.addClass("ui-disabled").attr("aria-disabled", true);
        return this._setOption("disabled", true);
    }

}, $.mobile.behaviors.formReset));


//auto self-init widgets
$(document).bind("pagecreate create", function (e) {
    $.mobile.slider.prototype.enhanceWithin(e.target, true);
});

})(jQuery);

$(document).ready(function () {
    $("[type='vertical-range']").verticalSlider();
});