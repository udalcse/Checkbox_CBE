<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TermsBindingControl.ascx.cs" Inherits="CheckboxWeb.Controls.Piping.TermsBindingControl" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Users" %>
<%@ Import Namespace="Checkbox.Web" %>

<span style="padding-left: 15px">
    <label for="_termsBinding">Bind terms:</label>
</span>
<asp:DropDownList id="_termsBinding"
                  AutoPostBack="false"
                  runat="server" uframeignore="true">
</asp:DropDownList>

 <btn:CheckboxButton ID="_mergeButton" runat="server" TextId="/controlText/pipeControl.ascx/merge" CssClass="ckbxButton roundedCorners border999 shadow999 silverButton" Style="font-size: 11px;" uframeignore="true" />
