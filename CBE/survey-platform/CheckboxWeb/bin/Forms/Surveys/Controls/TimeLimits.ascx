<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="TimeLimits.ascx.cs"
    Inherits="CheckboxWeb.Forms.Surveys.Controls.TimeLimits" %>
<%@ Import Namespace="Checkbox.Web" %>

<ckbx:ResolvingScriptElement ID="_datePickerLocaleResolver" runat="server" />
<ckbx:ResolvingScriptElement runat="server" Source="~/Resources/jquery-ui-timepicker-addon.js" />

<script type="text/javascript">
   /* $(document).ready(function () {
        $('.datepicker').datetimepicker({
            numberOfMonths: 2
        });
    });*/
</script>
<div class="dialogSubTitle">
    <%=WebTextManager.GetText("/controlText/forms/surveys/timeLimits.ascx/timeLimits") %>
</div>
<div class="dialogSubContainer">
    <div class="dialogInstructions">
        <%=WebTextManager.GetText("/controlText/forms/surveys/timeLimits.ascx/timeLimitsInstructions") %>
    </div>
    <div class="formInput">
        <div class="left fixed_100">
            <p>
                <ckbx:MultiLanguageLabel ID="_startDateLbl" runat="server" AssociatedControlID="_startDatePicker"
                    TextId="/controlText/forms/surveys/timeLimits.ascx/startDate">Survey is only available on or after this date:</ckbx:MultiLanguageLabel></p>
        </div>
        <div class="left">
            <ckbx:DateTimePicker ID="_startDatePicker" runat="server" CssClass="left" NoDefaultDate="True" />
        </div>
    </div>
    <br class="clear" />
    <div class="formInput">
        <div class="left fixed_100">
            <p>
                <ckbx:MultiLanguageLabel ID="_endDateLbl" runat="server" AssociatedControlID="_endDatePicker"
                    TextId="/controlText/forms/surveys/timeLimits.ascx/endDate">Survey is only available on or before this date:</ckbx:MultiLanguageLabel></p>                    
        </div>
        <div class="left">
            <ckbx:DateTimePicker ID="_endDatePicker" runat="server" CssClass="left" NoDefaultDate="True"/>
        </div>
    </div>
    <asp:Panel ID="_validationError" runat="server" Visible="false" CssClass="error message"><%= ValidationErrorText %></asp:Panel>
    <br class="clear" />
</div>
