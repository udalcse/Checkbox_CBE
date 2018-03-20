<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TermRenderer.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.TermRenderer" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Import Namespace="System.Web.UI" %>

<script type="text/javascript">
    $(function () {
        debugger;
        $(".Message,.questionTextContainer,.Matrix,.matrix,.viewResponseTD,.rankOrderQuestion,.radioButtonList,.checkboxToggleLabel").each(function (ind, elem) {
            var html = $(elem).html();
            if (window.SurveyTerms) {
                for (var i in window.SurveyTerms) {
                    var term = "%%" + window.SurveyTerms[i].Name;
                    var index = html.indexOf(term);
                    if (index >= 0 && (/[^\w]/.test(html[index + term.length]) || html.length == term.length)) {
                        if (window.SurveyTerms[i].Definition && window.SurveyTerms[i].Definition != '') {
                            $(elem).html(html.replace(term, "<a href='#'  title='" + window.SurveyTerms[i].Definition + "'>" + window.SurveyTerms[i].Term + "</a>"));
                        }
                        else {
                            $(elem).html(html.replace(term, "<a href='" + window.SurveyTerms[i].Link + "' target=\"_blank\">" + window.SurveyTerms[i].Term + "</a>"));
                        }
                        html = $(elem).html();
                    }
                }
            }
        });
    });
</script>
