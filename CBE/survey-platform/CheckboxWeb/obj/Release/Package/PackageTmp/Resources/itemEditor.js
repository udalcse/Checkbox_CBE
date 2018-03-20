
$(function () {
    setTimeout(function () { fixSelectWidth(); fixIFrames(); fixTableProperties(); }, 500);
});


function isMsieSevenOrNewer() {
    if (typeof window.XMLHttpRequest !== 'undefined') {
        return true;
    }

    return false;
}
