<%@ Import Namespace="Checkbox.Web"%>
<%@ Import Namespace="Checkbox.Management"%>
<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="MatrixSumTotalBehavior.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.MatrixSumTotalBehavior" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/SingleLineTextBehavior.ascx" TagPrefix="be" TagName="Behavior" %>

<script type="text/javascript">
    $(document).ready(function () {
        $('#<%=_total.ClientID %>').numeric({ decimal: true, negative: true });
    });
</script>


<be:Behavior ID="_singleLineBehaviour" runat="server" EnableViewState="true"/>

<div class="formInput">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_operator" ID="_lblOperator" runat="server" TextId="/controlText/matrixSumTotalItemEditor/operator"/></p>
     <ckbx:MultiLanguageDropDownList ID="_operator" runat="server">
        <asp:ListItem runat="server" Text="Equal" TextId="/enum/logicalOperator/equal" Value="Equal"/>
        <asp:ListItem runat="server" Text="GreaterThan" TextId="/enum/logicalOperator/greaterThan" Value="GreaterThan"/>
        <asp:ListItem runat="server" Text="GreaterThanEqual" TextId="/enum/logicalOperator/greaterThanEqual" Value="GreaterThanEqual"/>
        <asp:ListItem runat="server" Text="LessThan" TextId="/enum/logicalOperator/lessThan" Value="LessThan"/>
        <asp:ListItem runat="server" Text="LessThanEqual" TextId="/enum/logicalOperator/lessThanEqual" Value="LessThanEqual"/>
     </ckbx:MultiLanguageDropDownList>

    <p><ckbx:MultiLanguageLabel AssociatedControlID="_total" ID="_lblTotal" runat="server" TextId="/controlText/matrixSumTotalItemEditor/sum" /></p>
    <asp:TextBox ID="_total" runat="server"  />
    <ckbx:MultiLanguageLabel ID="_totalErrorLbl" visible="false" runat="server" TextId="/controlText/matrixSumTotalItemEditor/totalError" />
</div>

<br class="clear"/>
