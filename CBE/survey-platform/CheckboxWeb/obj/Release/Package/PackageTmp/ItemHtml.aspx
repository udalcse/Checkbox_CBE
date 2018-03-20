<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="ItemHtml.aspx.cs" Inherits="CheckboxWeb.ItemHtml" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>Render Item</title>
        <ckbx:ResolvingCssElement runat="server" Source="~/Resources/css/smoothness/jquery-ui-1.10.2.custom.css" />
        
        <asp:Literal ID="_styleInclude" runat="server" />
        <asp:Literal ID="_scriptInclude" runat="server"></asp:Literal>
    </head>
    <body>
        <form id="_theForm" runat="server">
            <div class="itemDiv_<%= ItemId ?? 0 %> itemHtmlDiv" >
                <asp:PlaceHolder ID="_itemPlace" runat="server" />
            </div>
        </form>
    </body>
</html>

<script type="text/javascript">
    $(function() {
        if (getIeVersion() != 7) {
            $('.itemDiv_<%= ItemId ?? 0 %>').find('input, select').filter(':not([uniformIgnore])').uniform();
        }
    });
</script>
