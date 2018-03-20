/***************************************************************************
 * Helper class to encapsulate common functionality required when using    *
 *   handling js dates.                                                    *
 ***************************************************************************/

Date.prototype.addHours = function (h) {
    this.setHours(this.getHours() + h);
    return this;
}

$(document).on('change', 'input.hasDatepicker', function () {
    var hidden = $(this).parent().find(':hidden.forDatepicker');
    if (!hidden.length) {
        $(this).append("<input type='hidden' class='forDatepicker' name='" + $(this).attr('name') + "_date' id='" + $(this).attr('id') + "_date'>");
    }
    hidden = $(this).parent().find(':hidden.forDatepicker');
    if ($(this).datetimepicker("getDate") != null) {
        hidden.val($(this).datetimepicker("getDate").toISOString());
    }
    else {
        hidden.val("");
    }
});

//Create instance of helper object
var dateUtils = new dateUtilsObj();

//Definition of helper object
function dateUtilsObj() {
    this.timeFormat = '';
    this.dateFormat = '';

    //Add leading zero to number if necessary
    var addLeadingZero = function (theNumber) {
        if (theNumber > 9)
            return theNumber;

        return '0' + theNumber;
    };

    var parseJsonDateTime = function(jsonDate) {
        return new Date(parseInt(jsonDate.match(/\/Date\(([0-9]+)(?:.*)\)\//)[1]));
    };

    //Convert date to invariant YYYY-MM-DD HH:MM:SS format
    this.jsonDateToInvariantDateString = function (jsonDate, defaultValue, dateOnly) {
        if (jsonDate == null
            || jsonDate == '') {

            if (defaultValue == null) {
                return '';
            }

            return defaultValue;
        }

        var theDate;
        var dateIndex = jsonDate.indexOf("/Date(");
        if (dateIndex < 0) {
            // ISO 8601 date format
            theDate = moment(jsonDate).toDate();
            // theDate = new Date(jsonDate); // ISO 8601 date format
        } else
            theDate = parseJsonDateTime(jsonDate); // default json date format

        if (dateUtils.dateFormat == '') {
            dateUtils.dateFormat = _dateFormat.toLowerCase();
        }

        returnValue = $.localize(theDate, dateUtils.dateFormat);

        if (dateOnly == null || !dateOnly) {
            if (dateUtils.timeFormat == '') {
                dateUtils.timeFormat = _timeFormat.toUpperCase();
            }
            returnValue += ' ' + $.localize(theDate, dateUtils.timeFormat);
        }
        return returnValue;
    };

    //get time from json time date
    this.jsonDateToInvariantTimeString = function (jsonDate, defaultValue) {
        if (jsonDate == null
            || jsonDate == '') {

            if (defaultValue == null) {
                return '';
            }

            return defaultValue;
        }

        var theDate = new Date(parseInt(jsonDate.substr(6)));

        h = theDate.getHours();
        m = theDate.getMinutes();

        returnValue = h == 0 && m == 0 ? '12:00 AM' :
                addLeadingZero(h <= 12 ? h : h - 12) + ':'
                        + addLeadingZero(m) + (h > 11 ? ' PM' : ' AM');

        return returnValue;
    };

    //extract time from the datetime string 'MM/DD/YYYY HH:MI'
    this.extractTime = function (date, defaultValue) {
        a = date.split(' ');
        if (a.length < 2 || a[1].length == 0)
            return defaultValue;
        return a.length > 2 ? a[1] + ' ' + a[2] : a[1];
    }

    //extract date from the datetime string 'MM/DD/YYYY HH:MI'
    this.extractDate = function (date, defaultValue) {
        if (date == null || date == '' || date == 'No Restriction')
            return defaultValue;
        if (date.indexOf(' ') < 0)
            return date;

        a = date.split(' ');
        return a[0];
    }
}