$(function () {
    $('select, input:checkbox, input:radio, input:text').filter(':not([uniformIgnore])').uniform();
    fixSelectWidth();
    fixIFrames();
});

function isMsieSevenOrNewer() {
    if (typeof window.XMLHttpRequest !== 'undefined') {
        return true;
    }

    return false;
}
