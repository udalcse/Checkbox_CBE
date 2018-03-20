<%@ Control Language="C#" CodeBehind="PageConditionsRuleDisplay.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.PageConditionsRuleDisplay" %>

<asp:Panel ID="_errorPnl" runat="server" CssClass="error message" Visible="false">
    <ckbx:MultiLanguageLabel ID="_errorLbl" runat="server" TextId="/controlText/templatePageEditor/errorLoadingPageConditions" />
</asp:Panel>

<ckbx:MultiLanguageLabel ID="_ruleLbl" runat="server" CssClass="surveyEditorRuleLabel"  TextId="/controlText/templatePageEditor/pageConditions" />
<br />
<ckbx:MultiLanguageLabel ID="_noConditionsLbl" runat="server" CssClass="surveyEditorRuleText" TextId="/controlText/templatePageEditor/noPageConditions" />
<ckbx:MultiLanguageLabel ID="_pageWillBeDisplayedLbl" runat="server" CssClass="surveyEditorRuleText" TextId="/controlText/templatePageEditor/pageWillBeDisplayed" />
<br />
<div style="padding-left:10px;">
    <asp:Repeater ID="_orLevelRepeater" runat="server">
        <ItemTemplate>
            <asp:Repeater ID="_andLevelRepeater" runat="server">
                <ItemTemplate>
                    <div style="padding-top:4px;">
                        <div style="float:left;" class="surveyEditorRuleText">
                                '<asp:Label CssClass="surveyEditorRuleText" ID="_leftOperandLbl" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "LeftOperandText") %>' />'&nbsp;
                                <asp:Label CssClass="surveyEditorRuleText" Font-Italic="true" ID="_operatorLbl" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "OperatorText") %>' />&nbsp;
                                '<asp:Label CssClass="surveyEditorRuleText" ID="_rightOperandLbl" runat="server"  Text='<%# Server.HtmlEncode((string)DataBinder.Eval(Container.DataItem, "RightOperandText")) %>' />'
                        </div>
                    </div>
                </ItemTemplate>
                <SeparatorTemplate>
                    <div style="float:left;padding-left:5px;">                                    
                        <ckbx:MultiLanguageLabel ID="MultiLanguageLabel2" CssClass="surveyEditorRuleText" Font-Bold="true" runat="server" TextId="/pageText/conditionEditor.aspx/and" />
                    </div>
                    <br style="clear:both" />
                </SeparatorTemplate>
            </asp:Repeater>
            <br style="clear:both;" />
        </ItemTemplate>
        <SeparatorTemplate>
            <div style="clear:both;padding-left:5px;">
                <ckbx:MultiLanguageLabel ID="MultiLanguageLabel4" CssClass="surveyEditorRuleText" Font-Bold="true" runat="server" TextId="/pageText/conditionEditor.aspx/or" />
            </div>
        </SeparatorTemplate>
    </asp:Repeater>
</div>