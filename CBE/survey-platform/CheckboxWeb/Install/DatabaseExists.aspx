<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="DatabaseExists.aspx.cs" Inherits="CheckboxWeb.Install.DatabaseExists" %>
<%@ Register assembly="CheckboxWeb" namespace="CheckboxWeb.Controls.Button" tagprefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
    <head id="Head1" runat="server">
        <title id="_title" runat="server">Database Exists</title>
        
        <%-- Specified script include placeholder --%>
        <asp:PlaceHolder ID="_scriptIncludesPlace" runat="server" />
    </head>
    <body>
        <form id="form1" runat="server">
            <div class="padding10">
                <div class="dialogInstructions">
                    <asp:Label ID="_instructionsLabel" runat="server" meta:resourcekey="_instructionsLabel">
		              It appears that Checkbox&reg; Survey has already been installed in the specified database.  Please select an option below
		              and click the continue button to proceed.
		            </asp:Label>
                </div>
                <div class="input">
		            <asp:RadioButtonList ID="_dbExistsOptions" runat="server">
		                <asp:ListItem Selected="True" Text="Use this database as it is, preserving existing data" Value="UseDB" />
		                <asp:ListItem Text="Use this database, but overwrite all Checkbox&reg data" Value="Overwrite" />
		                <asp:ListItem Text="Change the database server settings" Value="ChangeSettings" />
		            </asp:RadioButtonList>
                </div>
                <div>&nbsp;</div>
                <cc1:CheckboxButton ID="_okButton" runat="server" Text="OK" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton" OnClick="OKButton_Click" Style="margin-left:75px;" />
            </div>
        </form>
    </body>
</html>
