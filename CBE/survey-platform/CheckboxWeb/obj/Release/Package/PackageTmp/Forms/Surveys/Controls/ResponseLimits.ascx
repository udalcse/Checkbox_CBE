<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ResponseLimits.ascx.cs"
    Inherits="CheckboxWeb.Forms.Surveys.Controls.ResponseLimits" %>
<%@ Import Namespace="Checkbox.Web" %>
<script type="text/javascript">
    $(document).ready(function () {
        $('#<%=_totalResponseLimit.ClientID %>').numeric({ decimal: false, negative: false });
        $('#<%=_perRespondentLimit.ClientID %>').numeric({ decimal: false, negative: false });
    });
</script>
<div class="dialogSubTitle">
    <%=WebTextManager.GetText("/controlText/forms/surveys/responseLimits.ascx/responseLimits") %>
</div>
<div class="dialogSubContainer">
    <div class="dialogInstructions">
        <%=WebTextManager.GetText("/controlText/forms/surveys/responseLimits.ascx/responseLimitsInstructions") %>
    </div>
    <div class="formInput">
        <div class="left fixed_225">
            <p>
                <ckbx:MultiLanguageLabel ID="_totalResponsesLbl" runat="server" TextId="/controlText/forms/surveys/responseLimits.ascx/limitTotalResponses"
                    AssociatedControlID="_totalResponseLimit">Limit the total number of responses to the survey to: </ckbx:MultiLanguageLabel></p>
        </div>
        <div class="left">
            <asp:TextBox ID="_totalResponseLimit" runat="server" CssClass="left" MaxLength="10" />
            <asp:RangeValidator ID="_totalResponsesLblRangeValidator" runat="server" ControlToValidate="_totalResponseLimit"
                MinimumValue="0" MaximumValue="2147483647" Type="Integer" Display="Dynamic" class="error message" style="margin: 1px 0px 0px 4px;float:left"> 
            <%=WebTextManager.GetText("/controlText/ResponseLimits/_totalRespondentLimit")%>
            </asp:RangeValidator>
        </div>
    </div>
    <br class="clear" />
    <div class="formInput">
        <div class="left fixed_225">
            <p>
                <ckbx:MultiLanguageLabel ID="_perRespondentLimitLbl" runat="server" AssociatedControlID="_perRespondentLimit"
                    TextId="/controlText/forms/surveys/responseLimits.ascx/limitResponsesPerRespondent">Limit the total number of responses allowed per respondent to:</ckbx:MultiLanguageLabel></p>
        </div>
        <div class="left">
            <asp:TextBox ID="_perRespondentLimit" runat="server" CssClass="left" MaxLength="10" />
            <asp:RangeValidator ID="_perRespondentLimitRangeValidator" runat="server" ControlToValidate="_perRespondentLimit"
                MinimumValue="0" MaximumValue="2147483647" Type="Integer" Display="Dynamic" class="error message" style="margin: 1px 0px 0px 4px; float:left"> 
            <%=WebTextManager.GetText("/controlText/ResponseLimits/_perRespondentLimit")%>
            </asp:RangeValidator>
        </div>        
    </div>
    <br class="clear" />
</div>
