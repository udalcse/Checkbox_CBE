<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Sorter.ascx.cs" Inherits="CheckboxWeb.Controls.Sorter" %>

<script type="text/javascript">
    $(document).ready(function () {

        $("#<%=_listOptions.ClientID %>").change(function () {
            var sortField = $("#<%=_listOptions.ClientID %> option:selected").val();
            <%=AssociatedGrid.ReloadGridHandler %>(true, sortField);
        });

        $("#<%=_up.ClientID %>").click(function(){
            <%=AssociatedGrid.ReloadGridHandler %>(true, null, false);
            $(this).hide();
            $("#<%=_down.ClientID %>").show();
        });

        $("#<%=_down.ClientID %>").click(function(){
            <%=AssociatedGrid.ReloadGridHandler %>(true, null, true);
            $(this).hide();
            $("#<%=_up.ClientID %>").show();
        });

    });
</script>

<asp:Label ID="_textLabel" runat="server"></asp:Label>
<asp:DropDownList ID="_listOptions" uniformIgnore="true" runat="server" CssClass="gridActions border999 shadow999 roundedCorners"></asp:DropDownList>

<asp:Image ID="_up" runat="server" ImageUrl="~/App_Themes/CheckboxTheme/Images/upSelect.png" />
<asp:Image ID="_down" runat="server" ImageUrl="~/App_Themes/CheckboxTheme/Images/downSelect.png" style="display:none"/>