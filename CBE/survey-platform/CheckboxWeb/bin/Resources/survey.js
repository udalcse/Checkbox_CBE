function isMsieSevenOrNewer() {
    if (typeof window.XMLHttpRequest !== 'undefined') {
        return true;
    }

    return false;
}

function applyUniform(selector) {
    var prefix = '';
    if (typeof selector != "undefined")
        prefix = selector + ' ';

    if (isMobile) {
        //if there is a mobile device, then reapply jquery-mobile
        $(prefix + '#surveyPage').page().show().trigger('pagecontentshow');
    } else {
        if (isMsieSevenOrNewer()) {
            $(prefix + 'select, input:checkbox, input:radio, input:text').filter(':not([uniformIgnore])').uniform();
        } else if ($('.slider-value-list-container').length > 0) {
            $(prefix + '.surveyContentFrame').css('position', 'relative');
            $('body').css('position', 'relative');
        }
    }
}

$(function () {
    if (!isMobile) {
        applyUniform();
    }

    fixSelectWidth();
    fixIFrames();
    fixTableProperties();
});
