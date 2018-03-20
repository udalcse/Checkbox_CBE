/////////////////////////////////////////////////////////////////////////////
// Helper for accessing localized strings by JS objects.                   //
/////////////////////////////////////////////////////////////////////////////

var textHelper = new textHelperObj();

/////////////////////////////////////////////////////////////////////////////
// Helper object for managing text strings. At this point not too much     //
//  more than array storage of text keys and strings                       //
/////////////////////////////////////////////////////////////////////////////
function textHelperObj() {
    this.textArray = new Array();
    this.debugMode = false;

    //Initialize helper to use debug mode, which will cause ID to be returned when
    // text value not found.
    this.initialize = function (debugMode) {
        this.debugMode = debugMode;
    }

    //Store a text value
    this.setTextValue = function (id, value) {
        textHelper.textArray[id.toLowerCase()] = value;
    }

    //Get a text value.  If value not found and default value specified use that instead.  Otherwise
    // return empty string or text id depending on debugMode setting.
    this.getTextValue = function (id, defaultValue) {
        var theValue = textHelper.textArray[id.toLowerCase()];

        if (theValue != null) {
            return theValue;
        }

        if (defaultValue != null) {
            if (textHelper.debugMode) {
                return '[NL]' + defaultValue;
            }
            
            return defaultValue;
        }

        if (textHelper.debugMode) {
            return '[NL]' + id;
        }

        return '';
    }

    //Translate boolean to yes/no
    this.getBooleanValue = function (value) {
        if (value == null) {
            return textHelper.getTextValue('/common/no', 'No');
        }

        if (value) {
            return textHelper.getTextValue('/common/yes', 'Yes');
        }

        return textHelper.getTextValue('/common/no', 'No');
    }

    //Search for all page elements with TextId values and replace them with localized text
    this.localizePage = function () {
        $('*[textId]').each(function () {
            $(this).html(textHelper.getTextValue($(this).attr('textId'), $(this).html()));
        });
    }

    //Determines if text is the appropriate length. 
    // If the text is too long the middle is truncated in order to maintain readability.
    this.truncateText = function (value, maxLength) {
        if (maxLength != null) {
            if (value.length > maxLength) {
                // Add addtional 4 to account for ellipsis inserted into middle of text and add one in case rounding of numToRemove/2
                // would cause an additional character to be included.
                var numToRemove = value.length - maxLength + 4;
                var firstSegmentEnd = (value.length / 2) - (numToRemove / 2);
                var secondSegmentStart = (value.length / 2) + (numToRemove / 2);

                value = value.substr(0, firstSegmentEnd) + "..." + value.substr(secondSegmentStart);
            }
        }
        return value;
    }
}