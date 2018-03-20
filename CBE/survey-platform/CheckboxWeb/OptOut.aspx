<%@ Language="C#" AutoEventWireup="true" Inherits="CheckboxWeb.OptOut" Codebehind="OptOut.aspx.cs" %>
<%@ Import Namespace="Checkbox.Web" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <meta name="viewport" content="width=device-width, initial-scale=1">

        <title>Opt Out</title>
        <ckbx:ResolvingCssElement runat="server" Source="App_Themes/CheckboxTheme/Checkbox.css" />
        <ckbx:ResolvingCssElement runat="server" Source="GlobalSurveyStyles.css" media="screen" />

        <%-- Survey-Specific Stylesheet --%>
        <asp:PlaceHolder ID="_surveyStylePlace" runat="server" />

        <ckbx:ResolvingScriptElement runat="server" Source="Resources/jquery-latest.min.js" />
        
        <asp:PlaceHolder ID="_scriptsPlace" runat="server" />
        
        <%-- Specified script include placeholder --%>
        <asp:PlaceHolder ID="_scriptIncludesPlace" runat="server" />
    </head>
    <body class="optOutBody">
        <script type="text/javascript">
            $(function () {
                if ($('#<%= _options.ClientID %> input:checked').length > 0) {
                    $('#reasons').show();
                } else {
                    $('#reasons').hide();
                }

                $('#<%= _options.ClientID %> input').click(function () {
                    $('#reasons').show();
                });
            });
        </script> 

        <form runat="server">
            <asp:Panel ID="_greetingsPanel" runat="server" CssClass="center optOutFrame optScreenText">
                <h1><%=SurveyTitle%></h1>
                <div class="optOutInnerFrame">
                    <h2><%=WebTextManager.GetText("/pageText/optOut.aspx/unsubscribeRequest")%></h2>
                    <p class="optScreenText">
                        <%=WebTextManager.GetText("/pageText/optOut.aspx/optOutText")%>
                    </p>
                    <br />
                    <div id="optOutControls">
                        <ckbx:MultiLanguageRadioButtonList ID="_options" runat="server" CssClass="optionsList">
                            <asp:ListItem TextId="/pageText/optOut.aspx/dontWantThisSurvey" Value="BlockSurvey" />
                            <asp:ListItem TextId="/pageText/optOut.aspx/dontWantThisSender" Value="BlockSender" />
                            <asp:ListItem TextId="/pageText/optOut.aspx/thisIsSpam" Value="MarkAsSpam" />
                        </ckbx:MultiLanguageRadioButtonList>
                
                        <div id="reasons">
                            <p class="optScreenText">
                                <%= WebTextManager.GetText("/pageText/optOut.aspx/comments")%>
                            </p>
                            <asp:TextBox ID="_userCommentTextArea" TextMode="MultiLine" Columns="40" Rows="5" MaxLength="1000" runat="server"></asp:TextBox>
                            <br/>
                            <ckbx:MultiLanguageButton class="surveyFooterButton" TextId = "/pageText/optOut.aspx/submit" ID="_submitOptout" runat="server"/>                            
                        </div>

                        <p style="">
                            <ckbx:MultiLanguageHyperLink  class="surveyFooterButton" TextId = "/pageText/optOut.aspx/continueWithSurvey" ID="_takeSurvey" Target="_self" runat="server"/>                            
                        </p>
                    </div>
                </div>
            </asp:Panel>
            <asp:Panel ID="_completePanel" runat="server" CssClass="center optOutFrame optScreenText">
                <div class="optOutInnerFrame">
                <p class="optScreenText">
                    <%= WebTextManager.GetText("/pageText/optOut.aspx/thanks")%>
                </p>
                </div>
            </asp:Panel>
        </form>
    </body>   
</html>