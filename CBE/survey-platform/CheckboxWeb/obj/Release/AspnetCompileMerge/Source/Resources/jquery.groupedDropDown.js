(function ($) {
    var methods = {
        group: function () {
            return this.each(function () {
                try {
                    var $this = $(this);

                    //List options of select item
                    var options = $this.children('option');

                    //Options to Remove that were promoted to groups
                    var optionsToRemove = new Array();

                    var curOptGroup = null;

                    for (var i = 0; i < options.length; i++) {
                        if ($(options[i]).attr('header') == 'true') {
                            curOptGroup = $('<optgroup label="' + $(options[i]).text() + '"></optgroup>');
                            $this.append(curOptGroup);

                            //Hide the option that has been promoted instead of removing to prevent possible ASP.NET
                            // postback issue.
                            optionsToRemove.push($(options[i]));
                        }
                        else {
                            if (curOptGroup != null) {
                                curOptGroup.append($(options[i]));
                            }
                        }
                    }

                    for (var i = 0; i < optionsToRemove.length; i++) {
                        optionsToRemove[i].remove();
                    }
                }
                catch (err) {
                    //alert(err);
                }
            });
        }
    }

    $.fn.groupListItems = function (method) {
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        }
    };
})(jQuery);