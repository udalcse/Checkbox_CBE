/// <reference path="~/Resources/jquery-1.3.2.min.js" / >

var menuClassName = ".floatMenu";
var menuHomeTop = null;
var menuTop = null;
var menuBottom = null;
var menuLeft = null;

$(document).ready(function() {
menuHomeTop = $(menuClassName).offset().top;


menuTop = $(menuClassName).offset().top;
menuBottom = menuTop + $(menuClassName).height();
menuLeft = $(menuClassName).offset().left;


//    $(menuClassName).css("position", "absolute");
//    $(menuClassName).css("left", menuLeft);
//    $(menuClassName).css("top", menuHomeTop);



    $(window).scroll(function() {
        
        menuTop = $(menuClassName).offset().top;
        menuBottom = menuTop + $(menuClassName).height();
        menuLeft = $(menuClassName).offset().left;

        if ($(window).scrollTop() > menuBottom) {
            $(menuClassName).css("position", "absolute");
            $(menuClassName).css("left", menuLeft);
            var newTop = $(document).scrollTop() + "px";
            $(menuClassName).animate({ top: newTop }, { duration: 500, queue: false });
        }
        if (menuTop > $(window).scrollTop() && $(window).scrollTop() > menuHomeTop) {
            $(menuClassName).css("position", "absolute");
            $(menuClassName).css("left", menuLeft);
            var newTop = $(document).scrollTop() + "px";
            $(menuClassName).animate({ top: newTop }, { duration: 500, queue: false });
        }
        if ($(window).scrollTop() == 0) {
            $(menuClassName).animate({ top: menuHomeTop }, { duration: 500, queue: false });
        }

    });
}); 
