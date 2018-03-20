﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="Edit.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Filters.Edit" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="_headContent" runat="server">
    <ckbx:ResolvingScriptElement ID="_datetimepickerAddon" runat="server" Source="~/Resources/jquery-ui-timepicker-addon.js" />
    <ckbx:ResolvingScriptElement ID="_datePickerLocaleResolver" runat="server" />
    <script type="text/javascript" >
        $(function () {
            $('[datetimepicker="true"] input').datetimepicker({ numberOfMonths: 2 });
        });
        
        function showError(errorMessage) {
            if ($('.validation-input-error').is(':hidden')) {
                $('.validation-input-error').text(errorMessage).slideDown(300).delay(2000).slideUp(600);
            }
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="_pageContent" runat="server">
    <asp:Panel CssClass="error message validation-input-error" runat="server" style="display: none;" />
    <div class="padding15">
        <asp:PlaceHolder ID="_filterEditorPlace" runat="server"  />
    </div>
    <div class="dialogFormPush">&nbsp;</div>
</asp:Content>