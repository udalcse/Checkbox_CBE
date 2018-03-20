<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuestionBindingControl.ascx.cs" Inherits="CheckboxWeb.Controls.Piping.QuestionBindingControl" %>

<span style="padding-left: 15px">
    <label for="_questionBinding">Bind this item to profile property:</label>
</span>
<asp:DropDownList id="_questionBinding"
                  AutoPostBack="false"
                  runat="server">
</asp:DropDownList>