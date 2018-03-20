<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="AverageScoreMargins.ascx.cs" Inherits="CheckboxWeb.Styles.Charts.AverageScore.EditorControls.AverageScoreMargins" %>
<%@ Import Namespace="Checkbox.Web"%>

<div class="formInput">
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