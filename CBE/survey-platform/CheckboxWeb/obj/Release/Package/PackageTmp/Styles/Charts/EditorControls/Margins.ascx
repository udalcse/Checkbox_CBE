<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Margins.ascx.cs" Inherits="CheckboxWeb.Styles.Charts.EditorControls.Margins" %>
<%@ Import Namespace="Checkbox.Web"%>

<div class="formInput">
    <p style="text-decoration:underline;">
        <label >
            <%=WebTextManager.GetText("/pageText/styles/charts/margins.aspx/chart")%>
        </label>
    </p>
    <table>
        <tr>
            <td>
            </td>
            <td>
                <%=WebTextManager.GetText("/pageText/styles/charts/margins.aspx/top")%> 
                <br />
                <asp:TextBox ID="_chartTopMargin" runat="server" Width="40px" />
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td>
                <%=WebTextManager.GetText("/pageText/styles/charts/margins.aspx/left")%> 
                <br />
                <asp:TextBox ID="_chartLeftMargin" runat="server" Width="40px" />
            </td>
            <td>
            </td>
            <td>
                <%=WebTextManager.GetText("/pageText/styles/charts/margins.aspx/right")%> 
                <br />
                <asp:TextBox ID="_chartRightMargin" runat="server" Width="40px" />
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
                <%=WebTextManager.GetText("/pageText/styles/charts/margins.aspx/bottom")%> 
                <br />
                <asp:TextBox ID="_chartBottomMargin" runat="server" Width="40px" />
            </td>
            <td>
            </td>
        </tr>
    </table>
</div>