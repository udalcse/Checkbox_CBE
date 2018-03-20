<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Matrix_SingleLineText.ascx.cs"  Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.Matrix_SingleLineText" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Forms.Items" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web.Forms.UI.Rendering" %>

<td align="center" valign="middle" class="<%=GetCellClassName() %>" style="<%=GetCellWidthStyle() %>" <% if (Model.IsSPCArgument) { %> spcMarker="true" <% } %>>
    <asp:TextBox ID="_textBox"  runat="server" TextMode="multiline"  />
    <%--<asp:TextBox ID="txtname" runat="server" TextMode="MultiLine" ></asp:TextBox>--%>
    
</td>
     
<script language="C#" runat="server">

  

</script>
