<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ItemConditionsRuleDislpay.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemConditionsRuleDislpay" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Forms.Logic.Configuration" %>
<%@ Import Namespace="Checkbox.Web" %>

<asp:Panel ID="_errorPnl" runat="server" CssClass="error message" >
    <ckbx:MultiLanguageLabel ID="_errorLbl" runat="server" TextId="/controlText/templatePageEditor/errorLoadingItemConditions" />
</asp:Panel>

<div style="font-weight:bold;margin-top:25px;">
    <%=WebTextManager.GetText("/controlText/templatePageEditor/itemConditions")%>
</div>
<ckbx:MultiLanguageLabel ID="_noConditionsLbl" runat="server" CssClass="surveyEditorRuleText" TextId="/controlText/templatePageEditor/noItemConditions" />
<ckbx:MultiLanguageLabel ID="_pageWillBeDisplayedLbl" runat="server" CssClass="surveyEditorRuleText" TextId="/controlText/templatePageEditor/itemWillBeDisplayed" />
<br />
<div style="padding-left:10px;">
    <asp:Repeater ID="_orLevelRepeater" runat="server">
        <ItemTemplate>
            <asp:Repeater ID="_andLevelRepeater" runat="server">
                <ItemTemplate>
                    <div class="surveyEditorRuleText" style="padding-top:4px;">
                        <div style="float:left;">
                            <%# Utilities.CustomDecode(GetExpressionText(Container.DataItem))  %>
                        </div>
                    </div>
                </ItemTemplate>
                <SeparatorTemplate>
                    <div class="surveyEditorRuleText" style="float:left;padding-left:5px;">                                    
                        <b><%=WebTextManager.GetText("/pageText/conditionEditor.aspx/and")%></b>
                    </div>
                    <br style="clear:both" />
                </SeparatorTemplate>
            </asp:Repeater>
            <br style="clear:both;" />
        </ItemTemplate>
        <SeparatorTemplate>
            <div class="surveyEditorRuleText" style="clear:both;padding-left:5px;">
                <b><%=WebTextManager.GetText("/pageText/conditionEditor.aspx/or")%></b>
            </div>
        </SeparatorTemplate>
    </asp:Repeater>
</div>

<script type="text/c#" runat="server">
    protected string GetExpressionText(object expressionData)
    {
        if (expressionData == null
            || !(expressionData is ExpressionData))
        {
            return string.Empty;
        }

        return Utilities.AdvancedHtmlEncode(((ExpressionData)expressionData).ToString(LanguageCode));
    }
</script>