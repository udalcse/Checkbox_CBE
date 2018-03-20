function fixSelectWidthForPrintMode() {
    $('select').each(function (i, el) {
        if ($(el).width() > 150) {
            $(el).css('width', '150px');
        }
    });
}

$(function () {
    fixTableProperties();
    fixSelectWidthForPrintMode();
    fixIFrames();
    $('.Answer label').each(function (idx, el) {
        el.innerHTML = $(el).text();
    });
    window.print();
});

