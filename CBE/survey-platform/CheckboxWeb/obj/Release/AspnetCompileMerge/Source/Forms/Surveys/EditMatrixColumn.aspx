﻿<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="EditMatrixColumn.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.EditMatrixColumn" MasterPageFile="~/Dialog.Master" EnableEventValidation="false" ValidateRequest="false" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register Src="Controls/SurveyItemEditor.ascx" TagName="ItemEditor" TagPrefix="ckbx" %>

<asp:Content ID="_script" runat="server" ContentPlaceHolderID="_headContent">
    <script type="text/javascript">
        function okClick() {
            var inDialogWindow = (window.self != window.top);

            var args = { op: 'editMatrixColumn', result: 'ok'};
            if (inDialogWindow) {
                closeWindow(window.top.templateEditor.onDialogClosed, args);
            }
            else if (parent && parent.templateEditor) {
                parent.templateEditor.onDialogClosed(args);
            }
        }
    </script>
</asp:Content>

<asp:Content ID="_content" runat="server" ContentPlaceHolderID="_pageContent">
    <div style="height: 500px; overflow: auto">
        <ckbx:ItemEditor ID="_itemEditor" runat="server" />
    </div>
</asp:Content>