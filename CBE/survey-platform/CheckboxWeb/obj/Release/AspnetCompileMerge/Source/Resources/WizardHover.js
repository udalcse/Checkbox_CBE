/// <reference path="~/Resources/jquery-1.3.2.min.js" / >
$(document).ready(function() {
    $(".WizardItem").mouseenter(function() {
        $(this).addClass("SelectedBody").children("a").removeClass("WizardLink").addClass("WizardLinkSelectedHead");
        $(this).prev().children("a").removeClass("WizardLink").addClass("WizardLinkSelectedTail");
    });
    $(".WizardItem").mouseleave(function() {
        $(this).removeClass("SelectedBody").children("a").removeClass("WizardLinkSelectedHead").addClass("WizardLink");
        $(this).prev().children("a").removeClass("WizardLinkSelectedTail").addClass("WizardLink");
    });

});