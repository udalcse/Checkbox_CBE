(function ($) {

    var _appRootAlerted = false;

    var methods = {
        //Init
        ckbxEditable: function (options) {

            //Supported setting default values
            var settings = {
                inputCssClass: '',
                saveButtonCssClass: 'checkOn',
                cancelButtonCssClass: 'checkOff',
                spinnerCssClass: 'spinner16',
                clickPrompt: 'Click to edit',
                saveTitle: 'Save',
                cancelTitle: 'Cancel',
                onSave: null,
                onCancel: null,
                minInputSize: 10,
                dateInputSize: 11,
                dateTimeInputSize: 20,
                numericInputSize: 11,
                textAreaColumns: 50,
                textAreaRows: 5,
                appRoot: ''
            };

            return this.each(function () {
                try {
                    //Merge options with settings
                    if (options) {
                        $.extend(settings, options);
                    }

                    var $this = $(this);

                    $this.attr('title', settings.clickPrompt);

                    //Bind events
                    $this.bind('click', { settings: settings }, onInputClick);

                }
                catch (err) {
                    alert('jquery.ckbxEditable.js error.');
                }
            });
        }
    };

    //
    function onInputClick(event) {
        var settings = event.data.settings;
        event.stopPropagation();

        //Cancel other edits
        $('.ckbxEditableCancel').trigger('click');

        //Hide related error messages
        $(this).siblings('.error').hide();

        var editMode = $(this).attr('editMode');

        if (editMode) {
            editMode = editMode.toLowerCase();
        }

        var input = getInput($(this), settings, editMode);
        var saveBtn = $('<div class="hand ' + settings.saveButtonCssClass + '" title="' + settings.saveTitle + '">&nbsp;</div>');
        var cancelBtn = $('<div class="hand ' + settings.cancelButtonCssClass + ' ckbxEditableCancel" title="' + settings.cancelTitle + '">&nbsp;</div>');

        if (input.get(0).tagName.toLowerCase() == 'textarea') {
            saveBtn.css('vertical-align', 'top');
            cancelBtn.css('vertical-align', 'top');
        }

        $(this).hide();

        var inputContainer = $('<div style="display:inline-block;"></div>').append(input);
        $(this).after(cancelBtn).after(saveBtn).after(inputContainer);

        input.focus();

        saveBtn.bind('click', { settings: settings, editable: $(this), saveBtn: saveBtn, cancelBtn: cancelBtn, input: input }, onSaveClick);
        cancelBtn.bind('click', { settings: settings, editable: $(this), saveBtn: saveBtn, cancelBtn: cancelBtn, input: input }, onCancelClick);
        input.bind('keyup', function (e) {
            if (e.which == 13) {
                saveBtn.trigger('click');
                return;
            }

            if (e.which == 27) {
                cancelBtn.trigger('click');
                return;
            }

            //Otherwise, adjust textbox size, if necessary
            if ($(this).is('input')) {
                var inputSize = parseInt($(this).attr('size'));

                if (isNaN(inputSize)) {
                    return;
                }

                if (inputSize - $(this).val().length < 5) {
                    $(this).attr('size', $(this).val().length + 5);
                }
            }
        });


        if (editMode == 'tinymce') {
            initTinyMCE(input, settings);
        }
    }

    // replaces quates
    function escapeInjections(htmlString) {
        return htmlString
                        .replace(/"/g, "&quot;")
                        .replace(/'/g, "&#39;");
    }

    //
    function getInput(elementToEdit, settings, editMode) {
        var html = $(elementToEdit)[0].innerHTML;
        var inputSize = html.length;

        if (inputSize < settings.minInputSize) {
            inputSize = settings.minInputSize;
        }

        var textInput = $('<input type="text" value="' + escapeInjections(html.replace(/^\s\s*/, '')) + '" class="hand ' + settings.inputCssClass + '" />');

        if (editMode == 'date') {
            textInput.datepicker();
            textInput.attr("settingName", elementToEdit.attr("settingName"));
            textInput.change(function () {
                $("[editMode='Time'][settingName='" + textInput.attr("settingName") + "']").show();
            });
            inputSize = settings.dateInputSize;
        }


        if (editMode == 'datetime') {
            textInput.datetimepicker({
                numberOfMonths: 2,
                timeFormat: 'hh:mm TT'
            });
            inputSize = settings.dateTimeInputSize;
        }

        if (editMode == 'time') {
            textInput.timePicker({ show24Hours: false });
            inputSize = settings.dateInputSize;
        }

        if (editMode == 'numeric') {
            textInput.numeric({ decimal: false, negative: false });
            inputSize = settings.numericInputSize;
        }

        if (editMode == 'textarea' || inputSize >= settings.textAreaColumns) {
            //Figure rows/columns
            var rows = (inputSize / settings.textAreaColumns) + 1;

            if (rows < settings.textAreaRows) {
                rows = settings.textAreaRows;
            }

            textInput = $('<textarea class="hand ' + settings.inputCssClass + '">' + html.replace(/^\s\s*/, '') + '</textarea>')
                .attr('rows', rows)
                .attr('cols', settings.textAreaColumns);
        }
        else {
            textInput.attr('size', inputSize);
        }

        return textInput;
    }

    //
    function initTinyMCE(textInput, settings) {

        if (settings.appRoot == '') {
            if (!_appRootAlerted) {
                alert('appRoot required for ckbxEditable and tinyMCE edit mode.');
                _appRootAlerted = true;
            }

            return;
        }
        textInput.tinymce({
            // Location of TinyMCE script
            script_url: settings.appRoot + 'Resources/tiny_mce/tinymce.min.js',
            height: 325,
            relative_urls: false,
            remove_script_host: false,

            // Drop lists for link/image/media/template dialogs
            //template_external_list_url: 'lists/template_list.js',
            external_link_list_url: settings.appRoot + 'ContentList.aspx?contentType=documents',
            external_image_list_url: settings.appRoot + 'ContentList.aspx?contentType=images',
            media_external_list_url: settings.appRoot + 'ContentList.aspx?contentType=video',

            //Cause contents to be written back to base textbox on change
            onchange_callback: function (ed) { ed.save(); },
            gecko_spellcheck: true

            //init_instance_callback: applyMCE
        });
    }


    //
    function onSaveClick(event) {
        //Update text
        var newText = event.data.input.val();
        var oldValue = event.data.editable.html();

        event.data.editable.text(newText);

        //Restore state
        restoreText(event.data.editable, event.data.saveBtn, event.data.cancelBtn, event.data.input);

        if (event.data.settings.onSave != null && typeof (event.data.settings.onSave) == 'function') {
            event.data.settings.onSave(event.data.editable, newText, oldValue);
        }
    }

    //
    function onCancelClick(event) {
        //Restore state
        restoreText(event.data.editable, event.data.saveBtn, event.data.cancelBtn, event.data.input);

        if (event.data.settings.onCancel != null && typeof (event.data.settings.onCancel) == 'function') {
            event.data.settings.onCancel(event.data.editable);
        }
    }

    //
    function restoreText(editable, saveBtn, cancelBtn, input) {
        editable.show();
        saveBtn.remove();
        cancelBtn.remove();

        input.parent().remove();
    }

    $.fn.ckbxEditable = function (method) {
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.ckbxEditable.apply(this, arguments);
        }
    };
})(jQuery);