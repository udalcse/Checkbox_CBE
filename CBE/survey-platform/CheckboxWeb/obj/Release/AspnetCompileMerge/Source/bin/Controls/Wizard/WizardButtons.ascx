<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="WizardButtons.ascx.cs" Inherits="CheckboxWeb.Controls.Wizard.WizardControls.WizardButtons" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>

<div style="padding:5px;">
    <div class="left">
        <btn:CheckboxButton ID="_cancelButton" runat="server" CssClass="cancelButton" OnClientClick="closeWindow();return true;" CommandName="Cancel" CausesValidation="false"/>
        <btn:CheckboxButton ID="_previousButton" runat="server" CssClass="ckbxButton roundedCorners shadow999 border999 silverButton" CommandName="MovePrevious" CausesValidation="false" />
    </div>
    <div class="right">
        <btn:CheckboxButton ID="_nextButton" runat="server" CssClass="ckbxButton roundedCorners shadow999 border999 silverButton" />
    </div>
    <br class="clear" />
</div>