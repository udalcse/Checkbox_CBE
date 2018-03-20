//bindTooltips
function bindTooltips () {
    $('.tooltip').click(function (e) {
        e.preventDefault();
    }).mouseover(function (e) {
        toggleTooltipMessage(this, true);
    }).mouseout(function (e) {
        toggleTooltipMessage(this, false);
    });
}

// Activate tooltips
$(function () {
    bindTooltips();
});

// Toggle tooltip message
function toggleTooltipMessage(tt, show) {
    var ttMsgId = 'tooltip_message';
    var $message = $('#' + ttMsgId);
    if ($message.length == 0) {
        $message = $('<div id="' + ttMsgId + '" class="tooltip-message" />');
        $message.appendTo('body');
    }
    if (show && typeof $(tt).attr('data-message') !== 'undefined') {
        //get alignment
        var alignment = 'top';
        if (typeof $(tt).attr('data-alignment') !== 'undefined') {
            alignment = $(tt).attr('data-alignment');
        }
        var msgText = $(tt).attr('data-message');
        $message.html(msgText);
        $message.css('top', '-9999em').show();
        var msgHeight = $message.outerHeight();
        $message.hide();
        var ttOffset = $(tt).offset();
        var leftPos = ttOffset.left;
        var topPos = 0;
        if (alignment == 'free' && typeof $(tt).attr('data-top') !== 'undefined') {
            topPos = parseInt($(tt).attr('data-top'));
        } else {
            topPos = ttOffset.top - msgHeight - 5;
        }
        $message.css({ left: leftPos + 'px', top: topPos + 'px' }).stop(true, true).text(msgText).fadeIn();
    }
    else {
        $message.fadeOut('fast', function () {
            $(this).html('');
        });
    }
}