//Handles tasks related to the survey style editor

var styleEditor = new styleEditorObj();

function styleEditorObj() {
    // Data Sources for font size and style drop down lists
    this.borderStyles = new Array();
    this.borderSizes = new Array();
    this.fontSizes = new Array();
    this.fontNames = new Array();

    this.customProperties = new Array();

    this.headerTextId = '';
    this.footerTextId = '';
    this.appRoot = '';
    this.imageFolder = '';

    this.authToken = '';
    this.id = '';
    this.minFrameWidth = 800;

    this.isInitialized = false;
    this.hasUnsavedUpdates = false;

    this.initialize = function (authToken, appRoot, headerId, footerId, callbackArgs) {
        styleEditor.appRoot = appRoot;
        styleEditor.authToken = authToken;
        styleEditor.headerTextId = headerId;
        styleEditor.footerTextId = footerId;
        styleEditor.imageFolder = appRoot + 'App_Themes/CheckboxTheme/Images/';

        var resourceUrl = (appRoot + '/Resources/CodeDependentResources.xml').replace('//', '/');
        $.ajax({
            type: 'GET',
            url: resourceUrl,
            dataType: 'xml'
        })
            .done(styleEditor.parseXml)
            .always(function () {
                styleEditor.loadStyleData(callbackArgs);
            });

        $(document).on('change', '#_contentFrameMinWidth', function () {
            try {
                var width = parseInt($('#_contentFrameMinWidth').val());
                if (width < styleEditor.minFrameWidth) {
                    $(this).val(styleEditor.minFrameWidth);
                    $('#_minWidthValidationError').html(textHelper.getTextValue('/pageText/styles/forms/editor.aspx/minWidthError', 'Minimum Content Frame Width must be at least ') + styleEditor.minFrameWidth + 'px');
                    setTimeout(function () { $('#_minWidthValidationError').hide('slow'); }, 5000);
                } else {
                    $('#_minWidthValidationError').html('');
                    styleEditor.updateStyle({ classNames: ['.surveyContentFrame'], propertyMap: { 'min-width': (this.value != null && this.value != '') ? this.value + 'px' : ''} });
                }
                try {
                    if (width > parseInt($('#_contentFrameMaxWidth').val())) {
                        $('#_contentFrameMaxWidth').val(width);
                        styleEditor.updateStyle({ classNames: ['.surveyContentFrame'], propertyMap: { 'max-width': width} });
                        $('#_minWidthValidationError').html(textHelper.getTextValue('/pageText/styles/forms/editor.aspx/maxWidthError', 'Maximum Content Frame Width must be at least ') + width + 'px');
                        setTimeout(function () { $('#_minWidthValidationError').hide('slow'); }, 5000);
                    }
                } catch (e) {
                }
            } catch (e) {
                $('#_contentFrameMinWidth').val("");
                styleEditor.updateStyle({ classNames: ['.surveyContentFrame'], propertyMap: { 'max-width': ''} });
            }
        });

        $(document).on('change', '#_contentFrameMaxWidth', function () {
            var width = styleEditor.minFrameWidth;
            try {
                width = parseInt($('#_contentFrameMinWidth').val());
            } catch (e) {
            }
            try {
                if (parseInt($(this).val()) < width) {
                    $(this).val(width);
                    styleEditor.updateStyle({ classNames: ['.surveyContentFrame'], propertyMap: { 'max-width': width} });
                    $('#_minWidthValidationError').html(textHelper.getTextValue('/pageText/styles/forms/editor.aspx/maxWidthError', 'Maximum Content Frame Width must be at least ') + width + 'px');
                    setTimeout(function () { $('#_minWidthValidationError').hide('slow'); }, 5000);
                } else {
                    $('#_minWidthValidationError').html('');
                    styleEditor.updateStyle({ classNames: ['.surveyContentFrame'], propertyMap: { 'max-width': (this.value != null && this.value != '') ? this.value + 'px' : ''} });
                    setTimeout(function () { $('#_minWidthValidationError').hide('slow'); }, 5000);
                }
            } catch (e) {
                $('#_contentFrameMaxWidth').val("");
                styleEditor.updateStyle({ classNames: ['.surveyContentFrame'], propertyMap: { 'max-width': ''} });
            }
        });

        $(window).bind('beforeunload', function () {
            // we only want to prevent navigation if the editor is fully intialized
            // and a change was detected AFTER complete initialization
            if (styleEditor.isInitialized && styleEditor.hasUnsavedUpdates) {
                return 'You have unsaved changes.';
            }
        });

        // HACK ATTACK
        // too many layers of jquery templates have made this super complicated or buggy
        // we'll just wait 3 secs to mark the editor as being successfully intitialized
        setTimeout(function () {
            styleEditor.isInitialized = true;
            styleEditor.hasUnsavedUpdates = false;
        }, 3000);
    };

    //////////////////////////////////////////////////////////////
    // Load the selected style data into the templates          //
    //////////////////////////////////////////////////////////////
    this.loadStyleData = function (args) {
        styleEditor.cleanStatusPanel();

        if (args.styleId == null || args.styleId == '') {
            ShowStatusMessage('The style editor requires an ID number and no ID number was found.', false);
        }

        styleEditor.id = args.styleId;
        //Start by loading survey meta data
        svcStyleEditor.getStyleData(styleEditor.authToken, args.styleId, styleEditor.onStyleDataLoaded, { styleId: args.styleId, at: styleEditor.authToken, tinyMCEPath: args.tinyMCEPath });
    };

    // Apply template to loaded data and then apply child templates
    this.onStyleDataLoaded = function (resultData, args) {
        // Load/Compile/Run Templates
        templateHelper.loadAndApplyTemplateD(
            'fontsColorsEditorTemplate.html',
            styleEditor.appRoot + '/Styles/Forms/jqtmpl/fontsColorsEditorTemplate.html',
            resultData,
            null,
            'fontsColorsPlace',
            true,
            args).then(function () {
                var colorsHtml = $('#fontsColorsAccordion')[0].innerHTML;
                $('#fontsColorsPlace').remove();
                var $editorAccordion = $('#styleEditorAccordion');
                $editorAccordion.prepend(colorsHtml);
                $editorAccordion.find('input[type=color]').mColorPicker({ imageFolder: styleEditor.imageFolder });
                // NOW that this set of items has been loaded,
                // initialize the ENTIRE Style Editor accordion
                $editorAccordion.show();
                $editorAccordion.show().accordion({ heightStyle: 'content', icons: '', collapsible: true });
                styleEditor.initializedTemplates++;
            });

        templateHelper.loadAndApplyTemplateD(
            'formControlsEditorTemplate.html',
            styleEditor.appRoot + '/Styles/Forms/jqtmpl/formControlsEditorTemplate.html',
            resultData,
            null,
            'formControlsPlace',
            true,
            args).then(function () {
                $('input[type=color]', '#formControlsPlace').mColorPicker({ imageFolder: styleEditor.imageFolder });
                styleEditor.initializedTemplates++;
            });

        templateHelper.loadAndApplyTemplateD(
            'headerFooterEditorTemplate.html',
            styleEditor.appRoot + '/Styles/Forms/jqtmpl/headerFooterEditorTemplate.html',
            resultData,
            null,
            'headerFooterPlace',
            true,
            args).then(function () {
                $('input[type=color]', '#headerFooterPlace').mColorPicker({ imageFolder: styleEditor.imageFolder });

                $('#_headerContent').html(textHelper.getTextValue(styleEditor.headerTextId, '').replace(/</gi, '&lt;').replace(/>/gi, '&gt;'));
                $('#_footerContent').html(textHelper.getTextValue(styleEditor.footerTextId, '').replace(/</gi, '&lt;').replace(/>/gi, '&gt;'));

                styleEditor.loadTinyMceEditor(args);
                styleEditor.initializedTemplates++;
            });
    };

    this.parseXml = function (xml) {
        //first we add 'not selected' values
       // styleEditor.borderStyles.push({ value: '', text: textHelper.getTextValue('/controlText/fontSelector/none', 'None') });
        styleEditor.borderSizes.push({ value: '', text: textHelper.getTextValue('/controlText/fontSelector/none', 'None') });
        styleEditor.fontSizes.push({ value: '', text: textHelper.getTextValue('/controlText/fontSelector/none', 'None') });
        styleEditor.fontNames.push({ value: '', text: textHelper.getTextValue('/controlText/fontSelector/none', 'None') });

        $(xml).find('BorderStyle').each(function () {
            styleEditor.borderStyles.push({ value: $(this).attr('Value'), text: $(this).attr('Text') });
        });

        $(xml).find('BorderSize').each(function () {
            styleEditor.borderSizes.push({ value: $(this).attr('Value'), text: $(this).attr('Text') });
        });

        $(xml).find('FontSize').each(function () {
            styleEditor.fontSizes.push({ value: $(this).attr('Value'), text: $(this).attr('Text') });
        });

        $(xml).find('FontName').each(function () {
            styleEditor.fontNames.push({ value: $(this).attr('Value'), text: $(this).attr('Text') });
        });
    };

    this.applyTemplate = function (resultData, args) {
        var dict = new Array();
        $.each(resultData.NameValueList, function (index, property) {
            dict[property.Name] = property.Value;
        });

        args.styleData = dict;
        templateHelper.loadAndApplyTemplateD(
            args.templateUrl,
            styleEditor.appRoot + 'Styles/Forms/jqtmpl/' + args.templateUrl,
            args,
            null,
            args.id,
            true,
            null).then(function () {
                var res = $('input[type=color]', '#' + args.id).mColorPicker({ imageFolder: styleEditor.imageFolder });

                var color = dict['color'];

                if (res.attr('id').indexOf("Border") > 0)
                    color = dict['border-color'];

                $.fn.mColorPicker.setInputColor(res.attr('id'), color, true);

                styleEditor.initializedTemplates++;
            });
    };

    this.setValues = function (resultData, args) {
        var dict = new Array();
        $.each(resultData.NameValueList, function (index, property) {
            dict[property.Name] = property.Value;
        });

        $.each(args, function (index, item) {
            if (item == 'background-color')
                $.fn.mColorPicker.setInputColor(index, dict['background-color'], true);
            else if (item == 'background-image') {
                if (typeof (dict[item]) != 'undefined')
                    $('#' + index).val(dict[item].replace('url("', '').replace('")', '').replace('url(', '').replace(')', ''));
            }
            else {
                if (typeof (dict[item]) != 'undefined') {

                    $('#' + index).val(dict[item].replace('px', ''));

                    //Trigger change event upon setting initial value so we can capture
                    // whether value is being cleared or not
                    if (index == '_contentFrameMinWidth') {
                        $('#_contentFrameMinWidth').trigger('change');
                    }
                    if (index == '_contentFrameMaxWidth') {
                        $('#_contentFrameMaxWidth').trigger('change');
                    }
                }
            }
        });

    };

    this.getValues = function (className, property) {
        var value = '';

        //jQuery doesn't seem to work with 'border-[xxx]' styles, but rather wants to get
        // border styles per side, such as 'border-left-width'
        if (property.toLowerCase() == 'border-width') {
            property = 'border-left-width';
        }

        if (property.toLowerCase() == 'border-style') {
            property = 'border-left-style';
        }

        if (property.toLowerCase() == 'border-color') {
            property = 'border-left-color';
        }

        if (className == 'body') {
            value = $('#_previewContainer').css(property);
        }
        else {
            //fix for matrix width: parse border-width value from inline style
            if (property.toLowerCase() == 'border-left-width' && className.toLowerCase() == '.matrix') {
                var inlineStyle = $(className).attr('style');
                var ind = inlineStyle.indexOf('border-width') + 'border-width'.length + 1;
                value = inlineStyle.substring(ind);
                var ind2 = value.indexOf(';');
                value = value.substring(0, ind2).replace(/^\s*/, '').replace(/\s*$/, '');
            } else {
                if (className === ".Flyover") {
                    value = $("input[name='flyoverFontColor']").val();
                }
                else if (className === '.Link') {
                    value = $("input[name='linkFontColor']").val();
                } else {
                    value = $('#_stylePreviewFrame').contents().find(className).css(property);
                }
            }
        }

        return value || '';
    };


    this.updateStyle = function (args) {
        $.each(args.classNames, function (index, className) {
            if (className == 'body') {
                className = '#_previewContainer';
            }

            if (className == '.Answer') {
                className = 'span.Answer';
            }

            //A little bit of hackery to allow clearing value and handling case where proper
            // values don't exist yet.
            if (className == '.surveyContentFrame') {
                //Store whether width had been cleared.  We'll use that information later when 
                // saving values.
                var widthCleared = args.propertyMap['min-width'] != null && args.propertyMap['min-width'] == '';
                $("#_contentFrameMinWidth").data('widthCleared', widthCleared);
            }

            //fix for IE & FF
            if ((className == '.Page') && (args.propertyMap['height'])) {
                args.propertyMap['height'] = args.propertyMap['height'] + ' !important';
                $('.Page').attr('style', 'height:' + args.propertyMap['height']);
            }
            $('#_stylePreviewFrame', top.document).contents().find(className).css(args.propertyMap);

            //Sometimes style preview can be too large. We need to resize the panels to show a tinyScrollbar.
            //This method is defined in DetailedList.master
            if (typeof (resizePanels) == 'function') {
                resizePanels();
            }

            if (styleEditor.isInitialized) {
                styleEditor.hasUnsavedUpdates = true;
            }
        });
    };

    this.updateCustomProperty = function(args) {
        styleEditor.customProperties = new Array();
        $.each(args.classNames, function(index, className) {
            styleEditor.customProperties.push({ Key: className, Value: args.propertyMap[index] });
            var elements = $('#_stylePreviewFrame', top.document).contents().find(className);
            $.each(args.propertyMap[index], function(ind, property) {
                elements.css(property.Key, property.Value);
            });
        });
        
        if (styleEditor.isInitialized) {
            styleEditor.hasUnsavedUpdates = true;
        }
    };

    this.updateBackgroundImage = function (args) {

        //$('#_stylePreviewFrame').contents().find('body').css('background-image', args.backgroundUrl);
        //$('#_stylePreviewFrame').contents().find('body').css('background-repeat', args.repeat);

        $('#_previewContainer').css('background-image', args.backgroundUrl);
        $('#_previewContainer').css('background-repeat', args.repeat);

        $('#_backgroundImageUrl').val(args.backgroundUrl.replace('url("', '').replace('")', ''));

        if (styleEditor.isInitialized) {
            styleEditor.hasUnsavedUpdates = true;
        }
    };

    this.deleteBackgroundImage = function () {
        $('#_previewContainer').css('background-image', 'none');
        $('#_backgroundImageUrl').val('');

        if (styleEditor.isInitialized) {
            styleEditor.hasUnsavedUpdates = true;
        }
    };

    this.cleanStatusPanel = function () {
        $('#_statusPanel').empty();
    };

    this.toggleEditor = function () {
        if ($('#_editorType').val() == 'footer') {
            $('#_footerContent_parent_parent').show();
            $('#_headerContent_parent').hide();
            return;
        }
        $('#_footerContent_parent_parent').hide();
        $('#_headerContent_parent').show();
    };

    this.loadTinyMceEditor = function (args) {

        $('textarea.tinymce').tinymce({
            // General options
            script_url: args.tinyMCEPath,
            relative_urls: false,
            remove_redundant_brs: true,
            forced_root_block: false,
            remove_script_host: false,
            mode: "textareas",
            
            height: 350,
            plugins: [
                "image charmap textcolor code upload link"
            ],
            toolbar1: "bold italic underline strikethrough superscript subscript | bullist numlist link | image upload | charmap code  | forecolor backcolor  | styleselect fontselect fontsizeselect ",

            menubar: false,
            // Example content CSS (should be your site CSS)
            //content_css: ,

            // Drop lists for link/image/media/template dialogs
            //template_external_list_url: "js/template_list.js",
            external_link_list_url: '<%=ResolveUrl("~/ContentList.aspx?contentType=documents")%>',
            external_image_list_url: '<%=ResolveUrl("~/ContentList.aspx?contentType=images")%>',
            media_external_list_url: '<%=ResolveUrl("~/ContentList.aspx?contentType=video")%>',

            //
            // Replace values for the template plugin
            //template_replace_values: {
            //username: "Some User",
            //staffid: "991234"
            //},
            setup: function (ed, l) {
                ed.on('change', function (l) {
                    styleEditor.updateHeaderFooter($('#_editorType').val(), l.level.content);
                });
            },
            gecko_spellcheck: true,
            fontsize_formats: "8px 10px 12px 14px 16px 18px 20px 24px 36px"
        });
    };

    this.updateHeaderFooter = function (type, content) {
        switch (type) {
            case 'header':
                $('#_stylePreviewFrame').contents().find('#headerPlace').html(content);
                break;
            case 'footer':
                $('#_stylePreviewFrame').contents().find('#footerPlace').html(content);
                break;
            default:
                break;
        }

        $('#_stylePreviewFrame')[0].height = $('#_stylePreviewFrame').contents()[0].height;

        if (styleEditor.isInitialized) {
            styleEditor.hasUnsavedUpdates = true;
        }
    };

    this.onDialogClosed = function (arg) {
        if (arg == null) {
            return;
        }

        if (arg.callbackArgs) {
            var fn = eval(arg.functionName);
            fn(arg.callbackArgs);
            styleEditor.updateStylePropertiesInList(arg.callbackArgs);
        }
    };

    this.updateStylePropertiesInList = function (arg) {
        $(arg.classNames).each(function (index, className) {
            var elems = $("[onchange*='" + className + "']");
            $(arg.propertyMap[index]).each(function (idx, pair) {
                if (pair.Key == 'font-family')
                    elems.filter("[name*='FontName']").val(pair.Value);
                else if (pair.Key == 'font-size')
                    elems.filter("[name*='FontSize']").val(pair.Value);
                else if (pair.Key == 'color') {
                    elems.filter("[name*='FontColor']").val(pair.Value);
                    elems.filter("[name*='FontColor']").siblings('span').css('background-color', pair.Value);
                }
                else if (pair.Key == 'background-color') {
                    elems.filter("[name*='BgColor']").val(pair.Value);
                    elems.filter("[name*='BgColor']").siblings('span').css('background-color', pair.Value);
                }
            });
        });
    };

    this.buildStyleData = function (callback, args) {

        //Special handling for content frame width.  Since the css('width') value for
        // content frame appears to return min-width from GlobalSurveyStyles.css when
        // width is explicitly set.  This will then be saved as the 'width' value, making
        // it impossible to allow content frame to expand by clearing value.

        var widthValue = '';

        var widthCleared = $('#_contentFrameMinWidth').data('widthCleared');

        if (widthCleared == null) {
            widthCleared = false;
        }

        widthValue = styleEditor.getValues('.surveyContentFrame', 'max-width');
        var minWidthValue = styleEditor.getValues('.surveyContentFrame', 'min-width');

        if (!widthCleared) {
            minWidthValue = styleEditor.getValues('.surveyContentFrame', 'width');
        }

        var styleData = {
            HeaderHtml: $('#_headerContent').val() || '',
            FooterHtml: $('#_footerContent').val() || '',
            Css: [
                {
                    Key: 'body',
                    Value: [{ Key: 'background-color', Value: styleEditor.getValues('body', 'background-color') }, { Key: 'background-image', Value: styleEditor.getValues('body', 'background-image') }, { Key: 'background-repeat', Value: styleEditor.getValues('body', 'background-repeat') }, { Key: 'font-size', Value: styleEditor.getValues('body', 'font-size') }, { Key: 'font-family', Value: styleEditor.getValues('body', 'font-family') }, { Key: 'color', Value: styleEditor.getValues('body', 'color') }, { Key: 'text-decoration', Value: styleEditor.getValues('body', 'text-decoration') }, { Key: 'font-style', Value: styleEditor.getValues('body', 'font-style') }, { Key: 'font-variant', Value: styleEditor.getValues('body', 'font-variant')}]
                },
                {
                    Key: '.Page',
                    Value: [{ Key: 'height', Value: styleEditor.getValues('.Page', 'height')}]
                },
                {
                    Key: '.Matrix .header',
                    Value: [{ Key: 'background-color', Value: styleEditor.getValues('.Matrix .header', 'background-color') }, { Key: 'font-size', Value: styleEditor.getValues('.Matrix .header', 'font-size') }, { Key: 'font-family', Value: styleEditor.getValues('.Matrix .header', 'font-family') }, { Key: 'color', Value: styleEditor.getValues('.Matrix .header', 'color') }, { Key: 'text-decoration', Value: styleEditor.getValues('.Matrix .header', 'text-decoration') }, { Key: 'font-style', Value: styleEditor.getValues('.Matrix .header', 'font-style') }, { Key: 'font-variant', Value: styleEditor.getValues('.Matrix .header', 'font-variant')}]
                },
                {
                    Key: '.Matrix .header td p',
                    Value: [{ Key: 'background-color', Value: styleEditor.getValues('.Matrix .header', 'background-color') }, { Key: 'font-size', Value: styleEditor.getValues('.Matrix .header', 'font-size') }, { Key: 'font-family', Value: styleEditor.getValues('.Matrix .header', 'font-family') }, { Key: 'color', Value: styleEditor.getValues('.Matrix .header', 'color') }, { Key: 'text-decoration', Value: styleEditor.getValues('.Matrix .header', 'text-decoration') }, { Key: 'font-style', Value: styleEditor.getValues('.Matrix .header', 'font-style') }, { Key: 'font-variant', Value: styleEditor.getValues('.Matrix .header', 'font-variant')}]
                },
                {
                    Key: '.Matrix .header td',
                    Value: [{ Key: 'background-color', Value: styleEditor.getValues('.Matrix .header', 'background-color') }, { Key: 'font-size', Value: styleEditor.getValues('.Matrix .header', 'font-size') }, { Key: 'font-family', Value: styleEditor.getValues('.Matrix .header', 'font-family') }, { Key: 'color', Value: styleEditor.getValues('.Matrix .header', 'color') }, { Key: 'text-decoration', Value: styleEditor.getValues('.Matrix .header', 'text-decoration') }, { Key: 'font-style', Value: styleEditor.getValues('.Matrix .header', 'font-style') }, { Key: 'font-variant', Value: styleEditor.getValues('.Matrix .header', 'font-variant')}]
                },
                {
                    Key: '.Matrix .header th',
                    Value: [{ Key: 'background-color', Value: styleEditor.getValues('.Matrix .header th', 'background-color') }, { Key: 'font-size', Value: styleEditor.getValues('.Matrix .header th', 'font-size') }, { Key: 'font-family', Value: styleEditor.getValues('.Matrix .header th', 'font-family') }, { Key: 'color', Value: styleEditor.getValues('.Matrix .header th', 'color') }, { Key: 'text-decoration', Value: styleEditor.getValues('.Matrix .header th', 'text-decoration') }, { Key: 'font-style', Value: styleEditor.getValues('.Matrix .header th', 'font-style') }, { Key: 'font-variant', Value: styleEditor.getValues('.Matrix .header th', 'font-variant')}]
                },
                {
                    Key: '.Matrix .subheader',
                    Value: [{ Key: 'background-color', Value: styleEditor.getValues('.Matrix .subheader', 'background-color') }, { Key: 'font-size', Value: styleEditor.getValues('.Matrix .subheader', 'font-size') }, { Key: 'font-family', Value: styleEditor.getValues('.Matrix .subheader', 'font-family') }, { Key: 'color', Value: styleEditor.getValues('.Matrix .subheader', 'color') }, { Key: 'text-decoration', Value: styleEditor.getValues('.Matrix .subheader', 'text-decoration') }, { Key: 'font-style', Value: styleEditor.getValues('.Matrix .subheader', 'font-style') }, { Key: 'font-variant', Value: styleEditor.getValues('.Matrix .subheader', 'font-variant')}]
                },
                {
                    Key: '.Matrix .subheader td',
                    Value: [{ Key: 'background-color', Value: styleEditor.getValues('.Matrix .subheader td', 'background-color') }, { Key: 'font-size', Value: styleEditor.getValues('.Matrix .subheader td', 'font-size') }, { Key: 'font-family', Value: styleEditor.getValues('.Matrix .subheader td', 'font-family') }, { Key: 'color', Value: styleEditor.getValues('.Matrix .subheader td', 'color') }, { Key: 'text-decoration', Value: styleEditor.getValues('.Matrix .subheader td', 'text-decoration') }, { Key: 'font-style', Value: styleEditor.getValues('.Matrix .subheader td', 'font-style') }, { Key: 'font-variant', Value: styleEditor.getValues('.Matrix .subheader td', 'font-variant')}]
                },
                {
                    Key: '.Matrix .Item',
                    Value: [{ Key: 'background-color', Value: styleEditor.getValues('.Matrix .Item', 'background-color') }, { Key: 'font-size', Value: styleEditor.getValues('.Matrix .Item', 'font-size') }, { Key: 'font-family', Value: styleEditor.getValues('.Matrix .Item', 'font-family') }, { Key: 'color', Value: styleEditor.getValues('.Matrix .Item', 'color') }, { Key: 'text-decoration', Value: styleEditor.getValues('.Matrix .Item', 'text-decoration') }, { Key: 'font-style', Value: styleEditor.getValues('.Matrix .Item', 'font-style') }, { Key: 'font-variant', Value: styleEditor.getValues('.Matrix .Item', 'font-variant')}]
                },
                {
                    Key: '.Matrix .Item td',
                    Value: [{ Key: 'background-color', Value: styleEditor.getValues('.Matrix .Item td', 'background-color') }, { Key: 'font-size', Value: styleEditor.getValues('.Matrix .Item td', 'font-size') }, { Key: 'font-family', Value: styleEditor.getValues('.Matrix .Item td', 'font-family') }, { Key: 'color', Value: styleEditor.getValues('.Matrix .Item td', 'color') }, { Key: 'text-decoration', Value: styleEditor.getValues('.Matrix .Item td', 'text-decoration') }, { Key: 'font-style', Value: styleEditor.getValues('.Matrix .Item td', 'font-style') }, { Key: 'font-variant', Value: styleEditor.getValues('.Matrix .Item td', 'font-variant')}]
                },
                {
                    Key: '.Matrix .AlternatingItem',
                    Value: [{ Key: 'background-color', Value: styleEditor.getValues('.Matrix .AlternatingItem', 'background-color') }, { Key: 'font-size', Value: styleEditor.getValues('.Matrix .AlternatingItem', 'font-size') }, { Key: 'font-family', Value: styleEditor.getValues('.Matrix .AlternatingItem', 'font-family') }, { Key: 'color', Value: styleEditor.getValues('.Matrix .AlternatingItem', 'color') }, { Key: 'text-decoration', Value: styleEditor.getValues('.Matrix .AlternatingItem', 'text-decoration') }, { Key: 'font-style', Value: styleEditor.getValues('.Matrix .AlternatingItem', 'font-style') }, { Key: 'font-variant', Value: styleEditor.getValues('.Matrix .AlternatingItem', 'font-variant')}]
                },
                {
                    Key: '.Matrix .AlternatingItem td',
                    Value: [{ Key: 'background-color', Value: styleEditor.getValues('.Matrix .AlternatingItem td', 'background-color') }, { Key: 'font-size', Value: styleEditor.getValues('.Matrix .AlternatingItem td', 'font-size') }, { Key: 'font-family', Value: styleEditor.getValues('.Matrix .AlternatingItem td', 'font-family') }, { Key: 'color', Value: styleEditor.getValues('.Matrix .AlternatingItem td', 'color') }, { Key: 'text-decoration', Value: styleEditor.getValues('.Matrix .AlternatingItem td', 'text-decoration') }, { Key: 'font-style', Value: styleEditor.getValues('.Matrix .AlternatingItem td', 'font-style') }, { Key: 'font-variant', Value: styleEditor.getValues('.Matrix .AlternatingItem td', 'font-variant')}]
                },
                {
                    Key: '.Matrix .Answer',
                    Value: [{ Key: 'font-size', Value: styleEditor.getValues('.Matrix .Answer', 'font-size') }, { Key: 'font-family', Value: styleEditor.getValues('.Matrix .Answer', 'font-family') }, { Key: 'color', Value: styleEditor.getValues('.Matrix .Answer', 'color') }, { Key: 'text-decoration', Value: styleEditor.getValues('.Matrix .Answer', 'text-decoration') }, { Key: 'font-style', Value: styleEditor.getValues('.Matrix .Answer', 'font-style') }, { Key: 'font-variant', Value: styleEditor.getValues('.Matrix .Answer', 'font-variant')}]
                },
                {
                    Key: '.Matrix .header td.Answer',
                    Value: [{ Key: 'font-size', Value: styleEditor.getValues('.Matrix .Answer', 'font-size') }, { Key: 'font-family', Value: styleEditor.getValues('.Matrix .Answer', 'font-family') }, { Key: 'color', Value: styleEditor.getValues('.Matrix .Answer', 'color') }, { Key: 'text-decoration', Value: styleEditor.getValues('.Matrix .Answer', 'text-decoration') }, { Key: 'font-style', Value: styleEditor.getValues('.Matrix .Answer', 'font-style') }, { Key: 'font-variant', Value: styleEditor.getValues('.Matrix .Answer', 'font-variant')}]
                },
                {
                    Key: '.Matrix .Answer td',
                    Value: [{ Key: 'font-size', Value: styleEditor.getValues('.Matrix .Answer td', 'font-size') }, { Key: 'font-family', Value: styleEditor.getValues('.Matrix .Answer td', 'font-family') }, { Key: 'color', Value: styleEditor.getValues('.Matrix .Answer td', 'color') }, { Key: 'text-decoration', Value: styleEditor.getValues('.Matrix .Answer td', 'text-decoration') }, { Key: 'font-style', Value: styleEditor.getValues('.Matrix .Answer td', 'font-style') }, { Key: 'font-variant', Value: styleEditor.getValues('.Matrix .Answer td', 'font-variant')}]
                },
                {
                    Key: '.Matrix',
                    Value: [{ Key: 'border-style', Value: styleEditor.getValues('.Matrix', 'border-style') }, { Key: 'border-color', Value: styleEditor.getValues('.Matrix', 'border-color') }, { Key: 'border-width', Value: styleEditor.getValues('.Matrix', 'border-width')}]
                },
                {
                    Key: '.surveyContentFrame',
                    Value: [{ Key: 'border-style', Value: styleEditor.getValues('.surveyContentFrame', 'border-style') }, { Key: 'border-color', Value: styleEditor.getValues('.surveyContentFrame', 'border-color') }, { Key: 'border-width', Value: styleEditor.getValues('.surveyContentFrame', 'border-width') }, { Key: 'background-color', Value: styleEditor.getValues('.surveyContentFrame', 'background-color') }, { Key: 'max-width', Value: widthValue }, { Key: 'min-width', Value: minWidthValue}]
                },
                {
                    Key: '.title',
                    Value: [{ Key: 'font-size', Value: styleEditor.getValues('.title', 'font-size') }, { Key: 'font-family', Value: styleEditor.getValues('.title', 'font-family') }, { Key: 'color', Value: styleEditor.getValues('.title', 'color') }, { Key: 'text-decoration', Value: styleEditor.getValues('.title', 'text-decoration') }, { Key: 'font-style', Value: styleEditor.getValues('.title', 'font-style') }, { Key: 'font-variant', Value: styleEditor.getValues('.title', 'font-variant')}]
                },
                {
                    Key: '.PageNumber',
                    Value: [{ Key: 'font-size', Value: styleEditor.getValues('.PageNumber', 'font-size') }, { Key: 'font-family', Value: styleEditor.getValues('.PageNumber', 'font-family') }, { Key: 'color', Value: styleEditor.getValues('.PageNumber', 'color') }, { Key: 'text-decoration', Value: styleEditor.getValues('.PageNumber', 'text-decoration') }, { Key: 'font-style', Value: styleEditor.getValues('.PageNumber', 'font-style') }, { Key: 'font-variant', Value: styleEditor.getValues('.PageNumber', 'font-variant')}]
                },
                {
                    Key: '.Error',
                    Value: [{ Key: 'font-size', Value: styleEditor.getValues('.Error', 'font-size') }, { Key: 'font-family', Value: styleEditor.getValues('.Error', 'font-family') }, { Key: 'color', Value: styleEditor.getValues('.Error', 'color') }, { Key: 'text-decoration', Value: styleEditor.getValues('.Error', 'text-decoration') }, { Key: 'font-style', Value: styleEditor.getValues('.Error', 'font-style') }, { Key: 'font-variant', Value: styleEditor.getValues('.Error', 'font-variant')}]
                },
                {
                    Key: '.Question',
                    Value: [{ Key: 'font-size', Value: styleEditor.getValues('.Question', 'font-size') }, { Key: 'font-family', Value: styleEditor.getValues('.Question', 'font-family') }, { Key: 'color', Value: styleEditor.getValues('.Question', 'color') }, { Key: 'text-decoration', Value: styleEditor.getValues('.Question', 'text-decoration') }, { Key: 'font-style', Value: styleEditor.getValues('.Question', 'font-style') }, { Key: 'font-variant', Value: styleEditor.getValues('.Question', 'font-variant')}]
                },
                {
                    Key: '.Question p',
                    Value: [{ Key: 'font-size', Value: styleEditor.getValues('.Question', 'font-size') }, { Key: 'font-family', Value: styleEditor.getValues('.Question', 'font-family') }, { Key: 'color', Value: styleEditor.getValues('.Question', 'color') }, { Key: 'text-decoration', Value: styleEditor.getValues('.Question', 'text-decoration') }, { Key: 'font-style', Value: styleEditor.getValues('.Question', 'font-style') }, { Key: 'font-variant', Value: styleEditor.getValues('.Question', 'font-variant')}]
                },
                {
                    Key: '.Question span',
                    Value: [{ Key: 'font-size', Value: styleEditor.getValues('.Question', 'font-size') }, { Key: 'font-family', Value: styleEditor.getValues('.Question', 'font-family') }, { Key: 'color', Value: styleEditor.getValues('.Question', 'color') }, { Key: 'text-decoration', Value: styleEditor.getValues('.Question', 'text-decoration') }, { Key: 'font-style', Value: styleEditor.getValues('.Question', 'font-style') }, { Key: 'font-variant', Value: styleEditor.getValues('.Question', 'font-variant') }]
                },
                {
                    Key: '.Answer',
                    Value: [{ Key: 'font-size', Value: styleEditor.getValues('span.Answer', 'font-size') }, { Key: 'font-family', Value: styleEditor.getValues('span.Answer', 'font-family') }, { Key: 'color', Value: styleEditor.getValues('span.Answer', 'color') }, { Key: 'text-decoration', Value: styleEditor.getValues('span.Answer', 'text-decoration') }, { Key: 'font-style', Value: styleEditor.getValues('span.Answer', 'font-style') }, { Key: 'font-variant', Value: styleEditor.getValues('span.Answer', 'font-variant')}]
                },
                {
                    Key: '.Description',
                    Value: [{ Key: 'font-size', Value: styleEditor.getValues('.Description', 'font-size') }, { Key: 'font-family', Value: styleEditor.getValues('.Description', 'font-family') }, { Key: 'color', Value: styleEditor.getValues('.Description', 'color') }, { Key: 'text-decoration', Value: styleEditor.getValues('.Description', 'text-decoration') }, { Key: 'font-style', Value: styleEditor.getValues('.Description', 'font-style') }, { Key: 'font-variant', Value: styleEditor.getValues('.Description', 'font-variant')}]
                },
                {
                    Key: "#tinymce a[href='#']",
                    Value: [ { Key: 'color', Value: styleEditor.getValues('.Flyover', 'color') }]
                },
                {
                    Key: "#tinymce a:not([href*='#'])",
                    Value: [{ Key: 'color', Value: styleEditor.getValues('.Link', 'color') }]
                },
                {
                    Key: "[id*='Renderer'] a[href='#']",
                    Value: [{ Key: 'color', Value: styleEditor.getValues('.Flyover', 'color') }]
                },
                {
                    Key: "[id*='Renderer'] a:not([href*='#'])",
                    Value: [{ Key: 'color', Value: styleEditor.getValues('.Link', 'color') }]
                },
                {
                    Key: '.Description p',
                    Value: [{ Key: 'font-size', Value: styleEditor.getValues('.Description', 'font-size') }, { Key: 'font-family', Value: styleEditor.getValues('.Description', 'font-family') }, { Key: 'color', Value: styleEditor.getValues('.Description', 'color') }, { Key: 'text-decoration', Value: styleEditor.getValues('.Description', 'text-decoration') }, { Key: 'font-style', Value: styleEditor.getValues('.Description', 'font-style') }, { Key: 'font-variant', Value: styleEditor.getValues('.Description', 'font-variant')}]
                },
                {
                    Key: '.surveyFooterButton',
                    Value: [{ Key: 'background-color', Value: styleEditor.getValues('.surveyFooterButton', 'background-color') }, { Key: 'font-size', Value: styleEditor.getValues('.surveyFooterButton', 'font-size') }, { Key: 'font-family', Value: styleEditor.getValues('.surveyFooterButton', 'font-family') }, { Key: 'color', Value: styleEditor.getValues('.surveyFooterButton', 'color') }, { Key: 'text-decoration', Value: styleEditor.getValues('.surveyFooterButton', 'text-decoration') }, { Key: 'font-style', Value: styleEditor.getValues('.surveyFooterButton', 'font-style') }, { Key: 'font-variant', Value: styleEditor.getValues('.surveyFooterButton', 'font-variant') }, { Key: 'border-style', Value: styleEditor.getValues('.surveyFooterButton', 'border-style') }, { Key: 'border-color', Value: styleEditor.getValues('.surveyFooterButton', 'border-color') }, { Key: 'border-width', Value: styleEditor.getValues('.surveyFooterButton', 'border-width')}]
                },
                {
                    Key: '.ProgressBar',
                    Value: [{ Key: 'height', Value: styleEditor.getValues('.ProgressBar', 'height') }, { Key: 'width', Value: styleEditor.getValues('.ProgressBar', 'width') }, { Key: 'background-color', Value: styleEditor.getValues('.ProgressBar', 'background-color') }, { Key: 'border-style', Value: styleEditor.getValues('.ProgressBar', 'border-style') }, { Key: 'border-color', Value: styleEditor.getValues('.ProgressBar', 'border-color') }, { Key: 'border-width', Value: styleEditor.getValues('.ProgressBar', 'border-width')}]
                },
                {
                    Key: '.ProgressBarInner',
                    Value: [{ Key: 'height', Value: styleEditor.getValues('.ProgressBarInner', 'height') }, { Key: 'width', Value: styleEditor.getValues('.ProgressBar', 'width') }, { Key: 'background-color', Value: styleEditor.getValues('.ProgressBarInner', 'background-color')}]
                }]
        };

        //update 'styleData' with custom properties
        $.each(styleEditor.customProperties, function (index, property) {
            var exists = false;
            $.each(styleData.Css, function (i, css) {
                if (css.Key == property.Key) {
                    exists = true;
                    findSameStyleProperties(css.Value, property.Value);
                }
            });
            if (exists == false) {
                styleData.Css.push(property);
            }
        });

        callback(styleEditor.authToken, styleEditor.id, 'en-US', styleData, args.callback);
    };

    //finds same file properties and overwrites they with new values
    function findSameStyleProperties(array1, array2) {
        $.each(array2, function (i, css2) {
            var exists = false;
            $.each(array1, function (j, css1) {
                if (css1.Key == css2.Key) {
                    css2.Value = css1.Value;
                    exists = true;
                    return false;
                }
            });
            if (exists == false) {
                array1.push(css2);
            }
        });
    }

    this.saveAndRedirect = function (id) {
        //TODO: language code for multi-language
        styleEditor.hasUnsavedUpdates = false;
        styleEditor.buildStyleData(svcStyleEditor.saveFormStyle, { styleData: [], callback: function () { styleEditor.hasUnsavedUpdates = false; document.location = (styleEditor.appRoot + 'Styles/Forms/Edit.aspx?s=' + id).replace('//', '/'); } });
    };
}
