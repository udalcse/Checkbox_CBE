/// <reference path="~/Resources/jquery-1.3.2.min.js" / >
function HideStatus() {
    $(document).ready(function() {
        setTimeout(function() { $(".StatusPanel").fadeOut('slow') }, 5000);
    });
}