/////////////////////////////////////////////////////////////////////////////
// globalHelper.js                                                         //
//                                                                         //
/////////////////////////////////////////////////////////////////////////////
function getIeVersion()
// Returns the version of Windows Internet Explorer or a -1
// (indicating the use of another browser).
{
    var rv = -1; // Return value assumes failure.


    var ua = navigator.userAgent;

    if (navigator.appName == 'Microsoft Internet Explorer') {
        var re = new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");
        if (re.exec(ua) != null)
            rv = parseFloat(RegExp.$1);
    }

    if (rv == -1) {
        //IE11 navigator.userAgent contains "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E; rv:11.0) like Gecko"
        if (ua.indexOf('.NET4.0') > 0) {
            var re = new RegExp("rv:([0-9]{1,}[\.0-9]{0,})");
            if (re.exec(ua) != null)
                rv = parseFloat(RegExp.$1);
        }
    }
    return rv;
}

/**
* HELPERS
*/

function createCookie(name, value, days) {
    var expires;
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toGMTString();
    }
    else expires = "";
    document.cookie = name + "=" + value + expires + "; path=/";
}

function readCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

function eraseCookie(name) {
    createCookie(name, "", -1);
}

function fixIFrames() {
    $('div.iframe').each(function (idx, el) {
        var attributes = $(el).prop("attributes");
        var iframe = $("<iframe/>");
        $.each(attributes, function () {
            $(iframe).attr(this.name, this.value);
        });
        $(iframe).insertAfter(el);
        $(el).remove();
    });
}

function fixSelectWidth() {
    $('select').filter(':not([uniformIgnore])').each(function (i, el) {
        if ($(el).parent().attr('class') == 'selector' && $(el).attr('uframeWidth')) {
            $(el).prev().css('width', ($(el).attr('uframeWidth') - 50) + 'px');
            $(el).parent().css('width', ($(el).attr('uframeWidth') - 20) + 'px');
        }
        if ($(el).parent().attr('class') == 'selector' && $(el).parent().width() > 150) {
            $(el).prev().css('width', '120px');
            $(el).parent().css('width', '150px');
        }
    });
}

function fixTableProperties() {
    $.each($('.questionTextContainer > table, .descriptionTextContainer > table, .HtmlItemControl > table, .Message > table'), function (ind, elem) {
        var border = $(elem).attr('border');
        if (typeof border !== 'undefined' && border !== false) {
            $(elem).css('border-width', border);
            $(elem).find('> tbody > tr > td').css('border-width', 1);
        }
        var cellpadding = $(elem).attr('cellpadding');
        if (typeof cellpadding !== 'undefined') {
            $(elem).find('> tbody > tr > td').css('padding', parseInt(cellpadding) + 'px');
        }
        var cellspacing = $(elem).attr('cellspacing');
        if (typeof cellspacing !== 'undefined') {
            $(elem).css('border-spacing', cellspacing + 'px');
        }
    });
}

function fixTablePropertiesforMultyLine() {
    $.each($('table'), function (ind, elem) {
        var border = $(elem).attr('border');
        if (typeof border !== 'undefined' && border !== false) {
            $(elem).css('border', border + " solid black");
            $(elem).find('> tbody > tr > td').css('border', "1px solid black");
        }
        var cellpadding = $(elem).attr('cellpadding');
        if (typeof cellpadding !== 'undefined') {
            $(elem).find('> tbody > tr > td').css('padding', parseInt(cellpadding) + 'px');
        }
        var cellspacing = $(elem).attr('cellspacing');
        if (typeof cellspacing !== 'undefined') {
            $(elem).css('border-spacing', cellspacing + 'px');
        }
        $(elem).closest('div').css('width', '100%');
    });
}

function stripScripts(s) {
    if (typeof s == "undefined")
        return "";

    return s.replace(/<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>/gi, '');
}

function hasDuplicates(array) {
    var valuesSoFar = Object.create(null);
    for (var i = 0; i < array.length; ++i) {
        var value = array[i];
        if (value in valuesSoFar) {
            return true;
        }
        valuesSoFar[value] = true;
    }
    return false;
}