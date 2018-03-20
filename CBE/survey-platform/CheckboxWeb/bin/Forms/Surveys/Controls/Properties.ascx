<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Properties.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.Properties" %>
<%@ Import Namespace="Checkbox.Web" %>

<script type="text/javascript">
    /*
        Commenting out until poor display issue resolved
    $(document).ready(function () {
    $('#<%=_surveyNameTxt.ClientID %>').attr('title', '<%=WebTextManager.GetText("/pagetext/forms/surveys/create.aspx/surveyNameTip") %>');
    $('#<%=_shortUrlTxt.ClientID %>').attr('title', '<%=WebTextManager.GetText("/common/optional") %>');
    $('#<%=_surveyNameTxt.ClientID %>').watermark();
    $('#<%=_shortUrlTxt.ClientID %>').watermark();
    });*/

    $(document).ready(function() {
        $('#<%=_surveyNameTxt.ClientID%>').focus();
    });

    <%  if (_urlRewritingPlace.Visible)
        {%>

        var timeoutId = null;

        $(document).ready(function(){
             $("#<%=_shortUrlTxt.ClientID%>").keyup(function(){
                alternateUrlHasBeenChangedHandler();
             });

             $("#<%=_extensionDrop.ClientID %>").change(function(){
                alternateUrlHasBeenChangedHandler();
             });
            
             $("#<%=_surveyNameTxt.ClientID%>").keyup(function(){
                
                 var customUrl = GetHandledString($(this).val());
                 $("#<%=_shortUrlTxt.ClientID%>").val(customUrl);     
                 alternateUrlHasBeenChangedHandler();                           
             });
        });
        
        //
        function alternateUrlHasBeenChangedHandler(){
            if (timeoutId != null)
                clearTimeout(timeoutId);
            var customUrl= $("#<%=_shortUrlTxt.ClientID%>").val() + '.' + $("#<%=_extensionDrop.ClientID %>").val();

            timeoutId = setTimeout(function(){ checkAlternateUrl(customUrl); }, 500);
        }

        //
        function checkAlternateUrl(customUrl){
            svcSurveyManagement.ifAlternateUrlIsAvailable(templateEditor.authToken, customUrl, '<%=Server.MapPath("~/").Replace(@"\",@"\\") %>', function(result){
                $("#<%=_shortUrlErrorLbl.ClientID %>").remove();                    
                if (result.Yes){
                    $("#<%=_customUrlErrorPanel.ClientID %>").hide();
                }
                else{
                    $("#_customUrlValidationError").text(result.Message);
                    $("#<%=_customUrlErrorPanel.ClientID %>").show();
                }
            });
        }

        //
        function GetHandledString(surveyName)
        {
	        var result = '';
	        for(var j=0; j<surveyName.length; j++)
		    {
		        var char = surveyName.charAt(j);                
		        var hh = char.charCodeAt(0);                
                //Filter only alphanumerical characters
		        if((hh > 47 && hh<58) || (hh > 64 && hh<91) || (hh > 96 && hh<123))
		        {
                        result += char;
		        }
                //Replace space
                if (hh==32){
                    result += '-';
                }
 		    }
            return result;
        }
    <%
       }%>

    </script>

    <asp:Panel ID="_dialogSubTitle" CssClass="dialogSubTitle" runat="server"><%=WebTextManager.GetText("/pageText/forms/surveys/create.aspx/surveySettings")%></asp:Panel>
    <div class="dialogSubContainer" style="padding-bottom: 0px;">
    
    <div class="formInput condensed" style="padding-bottom: 5px;">
        <asp:Panel ID="_welcomeText" runat="server" CssClass="simple-survey-dialog-text" style="margin-bottom: 15px;"><%=WebTextManager.GetText("/pageText/forms/surveys/create.aspx/welcome")%></asp:Panel>
        <div class="left fixed_150">
            <p><label for="<%=_surveyNameTxt.ClientID %>"><%=WebTextManager.GetText("/pagetext/forms/surveys/create.aspx/surveyName")%></label></p>
        </div>
        <asp:TextBox ID="_surveyNameTxt" runat="server" Width="350" CssClass="left"/>
        <div class="clear"></div>
        <div class="left fixed_150">
        </div>
        <div class="left fixed_350 <%= IsDetailedForm? "createSurveyDialogValidationMessage" : "simpleCreateSurveyDialogValidationMessage" %>">
            <asp:RequiredFieldValidator Display="Dynamic" ID="_nameRequiredValidator" runat="server" ControlToValidate="_surveyNameTxt" />
            <asp:CustomValidator Display="Dynamic" ID="_nameInUseValidator" runat="server" ControlToValidate="_surveyNameTxt" />
        </div>
    </div>
    <asp:Panel ID="_helpText" runat="server" CssClass="simple-survey-dialog-text" style="margin-top: 15px;"><%=WebTextManager.GetText("/pageText/forms/surveys/create.aspx/help")%> <a href="https://www.checkbox.com/checkbox-6-quick-start-guide-2/" target="_blank">Quick Start Guide</a>.</asp:Panel>
    <div class="clear"></div>

    <%-- URL Rewriting --%>
    <asp:PlaceHolder ID="_urlRewritingPlace" runat="server">
        <div class="formInput condensed">
            <div class="left fixed_150">
                <p><label for="<%=_shortUrlTxt.ClientID %>"><%=WebTextManager.GetText("/pageText/surveyProperties.aspx/shortUrl")%></label></p> 
            </div>

            <div class="left">
                <asp:TextBox id="_shortUrlTxt" runat="server" Width="200px" CssClass="left" />
            </div>
            <div class="left dropDown">
                <asp:DropDownList ID="_extensionDrop" runat="server" />
            </div>
            <div class="clear"></div>
            <div Id= "_customUrlErrorPanel" runat="server" class='error' style="margin-top:2px; display:none;">
                <span id="_customUrlValidationError"></span>
                <asp:Label ID="_shortUrlErrorLbl" runat="server" CssClass="ErrorMessageFld" />
            </div>
        </div>
    </asp:PlaceHolder>
    
    <div class="clear"></div>
    
    <asp:Panel ID="_surveyOptionsListContainer" CssClass="dialogSubContainer" style="padding-left:0px;" runat="server">
        <div class="formInput condensed">
            <ckbx:MultiLanguageCheckBoxList runat="server" ID="_surveyOptionsList">
                <asp:ListItem TextId ="/pageText/surveyProperties.aspx/activateSurvey" Value="ACTIVATE_SURVEY"/>
                <asp:ListItem TextId ="/pagetext/forms/surveys/create.aspx/scoredSurvey" Value="SCORED_SURVEY"/>    
            </ckbx:MultiLanguageCheckBoxList>
        </div>
    </asp:Panel>
        <!--
     <div class="formInput condensed">
        <div class="left checkBox">
            <asp:Checkbox ID="_actSurveyChk" runat="server" Checked="true" />
        </div>
        <div class="left">
            <p><label for="<%=_actSurveyChk.ClientID %>"><%=WebTextManager.GetText("/pageText/surveyProperties.aspx/activateSurvey")%></label></p>
        </div>
    </div>
    
     <div class="clear"></div>

    <div class="formInput condensed">
        <div class="left checkBox">
            <asp:CheckBox ID="_scoredChk" runat="server" />
        </div>
        <div class="left">
            <p><label for="<%=_scoredChk.ClientID %>"><%=WebTextManager.GetText("/pagetext/forms/surveys/create.aspx/scoredSurvey") %></label></p>
        </div>
    </div>-->

   



</div>

<div class="clear"></div>

<asp:Panel ID="_appSettingOverridePanel" runat="server">
    <ckbx:MultiLanguageLabel ID="_appSettingOverride" runat="server" TextId="/pageText/surveyProperties.aspx/appSettingOverride" ></ckbx:MultiLanguageLabel>
</asp:Panel>

<div class="clear"></div>
