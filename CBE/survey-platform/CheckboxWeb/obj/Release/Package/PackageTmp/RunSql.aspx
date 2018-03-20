<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RunSql.aspx.cs" Inherits="CheckboxWeb.RunSql" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Run Sql</title>
    <ckbx:ResolvingCssElement runat="server" Source="App_Themes/CheckboxTheme/Checkbox.css" />
</head>
<body>
    <form id="_sqlScriptForm" class="sql-run-form" runat="server">
        <p>
            <asp:Label runat="server" AssociatedControlID="_sqlScript">Run sql script </asp:Label>
        </p>
        <asp:TextBox runat="server" ID="_sqlScript" TextMode="MultiLine" Width="1050" Height="200"></asp:TextBox>
        <p>
            <asp:Button runat="server" ID="_runSql" OnClick="RunSqlEventHandler" Text="Run sql" CssClass="action-share action-button ckbxButton silverButton run-sql-script-btn" />
        </p>
        <p>

            <asp:GridView ID="_sqlResult" runat="server" Width="600px"
                AllowPaging="true" PageSize="30" OnPageIndexChanging="gvEmployee_PageIndexChanging"
                CssClass="run-sql-grid"
                AlternatingRowStyle-CssClass="alt"
                PagerStyle-CssClass="pgr">
            </asp:GridView>

        </p>
    </form>

</body>
</html>
