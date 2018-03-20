<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Survey.aspx.cs" Inherits="CheckboxWeb.Survey" Theme="" EnableEventValidation="false" %>
<%@ Register Src="~/Forms/Surveys/Controls/TakeSurvey/ResponseView.ascx" TagPrefix="ckbx" TagName="ResponseView" %>
<%@ Register Src="~/Controls/GoogleAnalytics/GoogleAnalytics.ascx" TagPrefix="ckbx" TagName="GoogleAnalytics" %>
<%@ Register Src="~/Forms/Surveys/Controls/TermRenderer.ascx" TagPrefix="ckbx" TagName="TermRenderer" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Newtonsoft.Json" %>

<asp:PlaceHolder ViewStateMode="Disabled" EnableViewState="False" runat="server">

    <!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

    <html xmlns="http://www.w3.org/1999/xhtml" >
        <head runat="server" id="_head">
            <meta http-equiv="x-ua-compatible" content="IE=Edge">
            <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no">
            <meta http-equiv="expires" content="0" /> 
            <meta http-equiv="PRAGMA" content="NO-CACHE" />

            <asp:PlaceHolder runat="server">
            <!-- for Facebook -->
            <meta property ="og:title" content ="<%=_pageTitleLiteral.Text%>"/>
            <meta property ="og:type" content="website"/>
            <meta property ="og:url" content="<%= SPCInitUrlEncoded %>"/>
            <meta property ="og:description" content="<%=SocialMediaDescription %>"/>
            <!-- for Google -->
            <meta itemprop= "description" content="<%=SocialMediaDescription %>" />
            <meta itemprop="name" content="<%=_pageTitleLiteral.Text %>">
                </asp:PlaceHolder>
        
            <title><asp:Literal id="_pageTitleLiteral" runat="server" /></title>

            <% 
                bool isMobile = IsBrowserMobile;
                bool isAjaxifyed = IsAjaxifyingSupported;
            %>
                     
            <%-- Global Survey Stylesheets --%>
            <ckbx:ResolvingCssElement runat="server" Source="GlobalSurveyStyles.css" media="screen" />
            <ckbx:ResolvingCssElement runat="server" Source="Resources/css/smoothness/jquery-ui-1.10.2.custom.css" media="screen" />
              
            <ckbx:ResolvingScriptElement runat="server" Source="Resources/jquery-latest.min.js" />
            <ckbx:ResolvingScriptElement runat="server" Source="Resources/jquery-ui-1.10.2.custom.min.js" />
            <ckbx:ResolvingScriptElement runat="server" Source="Resources/jquery.uniform.min.js" />
            <ckbx:ResolvingScriptElement runat="server" Source="Resources/tiny_mce/jquery.tinymce.min.js" />
            <ckbx:ResolvingScriptElement runat="server" Source="Resources/globalHelper.js" />
            <ckbx:ResolvingScriptElement runat="server" Source="Resources/htmlparser.js" />
		    <% if (isAjaxifyed) { %>
            <ckbx:ResolvingScriptElement runat="server" Source="Resources/workflow.js" />
            <% } else { %>
            <ckbx:ResolvingScriptElement runat="server" Source="Resources/spc.js" />
            <% } %>
        
		    <% if (isMobile) { %>
            <ckbx:ResolvingCssElement runat="server" Source="CheckboxHandheld.css" media="all" />
            <ckbx:ResolvingCssElement runat="server" Source="HandheldSurveyStyles.css" media="all" />
            <ckbx:ResolvingCssElement runat="server" ID="_mobileStyleInclude"/>        
        
            <ckbx:ResolvingScriptElement runat="server" Source="Resources/jquery.mobile-latest.min.js" />
            <% } else { %>
            <ckbx:ResolvingCssElement runat="server" Source="CheckboxScreen.css" media="all" />
            <ckbx:ResolvingCssElement runat="server" Source="ScreenSurveyStyles.css" media="all" />
        
            <ckbx:ResolvingScriptElement runat="server" Source="Resources/jquery-ui-combobox.js" />
            <ckbx:ResolvingScriptElement runat="server" Source="Resources/jquery.uniform.min.js" />
            <% } %>
		
            <!--[if lte IE 7]>
                <link rel="Stylesheet" type="text/css" media="screen" href="GlobalSurveyStyles_IE.css" />
            <![endif]-->
           
            <%-- Survey-Specific Stylesheet --%>
            <asp:PlaceHolder ID="_surveyStylePlace" runat="server" />

            <%-- Specified script include placeholder --%>
            <asp:PlaceHolder ID="_scriptIncludesPlace" runat="server" />

            <%-- Overridable Files --%>
            <ckbx:ResolvingCssElement runat="server" Source="CustomSurveyStyles.css" media="all" />
            <ckbx:ResolvingCssElement runat="server" Source="App_Themes/CheckboxTheme/Checkbox.css" media="all" />

            <script type="text/javascript">
                
                //prevent timestamps in the end of requests
                $.ajaxSetup({'cache':true});

                var globalTinyMceStyle = "";
                var htmlDecode = function(val) {
                    return (val) ? $('<div/>').html(val).text() : '';
                };
                var surveyUrl = htmlDecode('<%= SPCInitUrlEncoded %>');
                var isMobile = <%= isMobile.ToString().ToLower() %>;
                <% if (isAjaxifyed) {%>
                surveyWorkflow.init(surveyUrl, '<%=ResolveUrl("~") %>', isMobile);
                <% } else { %>    
                SPCManager.initSPC(surveyUrl, '<%=ResolveUrl("~") %>');
                <% } %>    

                function addStylesToTinyMce(css) {
                    globalTinyMceStyle = css;
                }

                window.SurveyTerms = <%= JsonConvert.SerializeObject(RenderedResponseTemplate.SurveyTerms) %>;

                /*            $(document).ready(function() {
                                $("body").bind("touchend", function(e) {
                                    if (isMobile) {
                                        if ($(e.target)[0] != $("a[aria-describedby*='ui-tooltip']")[0]) {
                                            $(".ui-tooltip").remove()
                                        }
                                    }
                                });
                            }); */

                function customDialog(element) {
                }

                if (isMobile) {
                    $(document).on('click', 'a', function() {
                        if ($(this).attr('title')) {
                            var $popUp = $("<div/>").popup({
                                dismissible: true,
                                theme: "a",
                                overlyaTheme: "e",
                                transition: "pop"
                            }).on("popupafterclose", function () {
                                $(this).remove();
                            }).css({
                                'max-width': '95%',
                                'padding': '5px'
                            });
                            $('<a href="#" data-rel="back" data-role="button" data-theme="b" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>').appendTo($popUp);
                            $("<h2>"+$(this).text()+"</h2>").appendTo($popUp);
                            $("<p>"+$(this).attr('title')+"</p>").appendTo($popUp);
                            $popUp.popup('open', {positionTo:$(this)}).trigger("create");
                        }
                    });
                } else {
                    $(document).tooltip();
                }

            </script>

            <ckbx:ResolvingScriptElement runat="server" Source="Resources/survey.js" />
        
            <ckbx:TermRenderer runat="server" ID="_termRenderer" />
            <ckbx:GoogleAnalytics runat="server" ID="_googleAnalytics" Visible="False" />
        </head>
    
        <body>
            <form id="_surveyForm" data-ajax="false" runat="server" >
                <%if (ApplicationManager.AppSettings.IsPrepMode && QueryParameters.ExportMode != ExportMode.ClientPdf) %>
                <%{ %>
                    <div class="prepModeBanner">System is in Prep Mode - survey responses will not be saved.</div>
                <%} %>
                <div id="surveyPage">
        		    <%-- Add a dummy textbox because IE will not always submit the name/value of the button      --%>
				    <%-- when the screen contains exactly one textbox --%>
				    <asp:TextBox ID="_dummyBox" uniform-ignore="true" runat="server" style="visibility:hidden;display:none;" />
				
				        <%-- Last Chance Error (used only when error can't be displayed via response view) --%>
				    <asp:Panel ID="_errorPanel" runat="server" Visible="false" EnableViewState="false" />
			
				    <%-- Response View --%>
				    <ckbx:ResponseView ID="_responseView" runat="server" />
			    </div>
            </form>
            
        </body>
        
         <script>
                // if it is client pdf mode replace controls with text answer 
               
                <% if (QueryParameters.ExportMode == ExportMode.ClientPdf) {%>
                    setTimeout(function() {

                        $("body").find("input[type='text']").each(function (ind, elem) {
                            $(elem).replaceWith("<b>" + $(elem).val() + "</b>");
                        });

                
                        //replace multilines lines
                        $("body").find("textarea").each(function(ind, elem) {


                            if (typeof tinyMCE !== 'undefined') {
                                var tinyMceEditor = tinyMCE.get(elem.id);

                                if (tinyMceEditor != undefined) {
                                    var content = tinyMceEditor.getContent();
                                    var iframe = document.createElement('iframe');
                                    $(elem).replaceWith(iframe);

                                    iframe.className = "tinyMceContent";
                                    var iframeBody = $(iframe).contents().find("body");
                                    iframeBody.append(content);

                                    tinyMceEditor.remove();
                                }

                            } else {
                                $(elem).replaceWith("<b>" + $(elem).val() + "</b>");
                            }
                        });

                        //replace radiobuttons
                        $("body").find(".radioButtonList input[type='radio']:checked").each(function (index, elem) {

                            var titleElement = $(elem).closest("tr").find(".radioButtonLabel.Answer").text();

                            var container = $(elem).parents(".radioButtonList");

                            $(elem).parents("table").remove();

                            container.append("<b>" + titleElement + "</b>");
                        });

                        $("body").find(".Matrix .selector").each(function (index, elem) {
                            var span = $(elem).find("span").first();
                            $(elem).replaceWith("<b>" + $(span).text() + "</b>");
                        });

                        $(".surveyContentFrame").removeClass("borderRadius");
                        $(".surveyContentFrame").css("background-color", "#fff");

                        var iFrames = $('iframe.tinyMceContent');
                        iFrames.each(function() {
                            this.width  = this.contentWindow.document.body.scrollWidth;
                            this.height = this.contentWindow.document.body.scrollHeight;
                        });

                        
                        $("a").each(function (index, elem){
                            var html = $(elem).html();
                            $(elem).replaceWith(html);
                        });

                    }, 2000);
               
            <% } %>
        </script>
    </html>

</asp:PlaceHolder>
