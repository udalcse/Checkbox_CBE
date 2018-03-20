/// <reference path="~/Resources/jquery-1.3.2.min.js" / >
/// <reference path="~/Resources/jquery.corner.js" / >
$(document).ready(function() {


    HighlightActiveStep();

    $(".WizardItem a").parent().mouseenter(function() {
        $(this).addClass("SelectedBody");
        $(this).children(".WizardLink").removeClass("WizardLink").addClass("WizardLinkSelectedHead");
        $(this).children(".WizardLinkSelectedTail").removeClass("WizardLinkSelectedTail").addClass("WizardLinkSelectedHeadTail");
        $(this).children(".WizardCompletedMark").removeClass("WizardCompletedMark").addClass("WizardCompletedMarkSelected");
        $(this).prev().children(".WizardLink").removeClass("WizardLink").addClass("WizardLinkSelectedTail");
        $(this).prev().children(".WizardLinkSelectedHead").removeClass("WizardLinkSelectedHead").addClass("WizardLinkSelectedHeadTail");
    });
    $(".WizardItem a").parent().mouseleave(function() {
        $(this).removeClass("SelectedBody");
        $(this).children(".WizardLinkSelectedHead").removeClass("WizardLinkSelectedHead").addClass("WizardLink");
        $(this).children(".WizardLinkSelectedHeadTail").removeClass("WizardLinkSelectedHeadTail").addClass("WizardLinkSelectedTail");
        $(this).children(".WizardCompletedMarkSelected").removeClass("WizardCompletedMarkSelected").addClass("WizardCompletedMark");
        $(this).prev().children(".WizardLinkSelectedTail").removeClass("WizardLinkSelectedTail").addClass("WizardLink");
        $(this).prev().children(".WizardLinkSelectedHeadTail").removeClass("WizardLinkSelectedHeadTail").addClass("WizardLinkSelectedHead");
    });

});

function HighlightActiveStep() {

    $(".SelectedBody").prev().children("a").removeClass("WizardLink").addClass("WizardLinkSelectedTail");
    $(".SelectedBody").prev().children("span").removeClass("WizardLink").addClass("WizardLinkSelectedTail");

}