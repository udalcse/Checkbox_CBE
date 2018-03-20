<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ResponseSelect.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.TakeSurvey.ResponseSelect" %>

<div class="wrapperMaster">
    <div class="center borderRadius surveyContentFrame surveyDialogFrame">
        <div class="innerSurveyContentFrame">
            <asp:Panel ID="_resumePanel" runat="server">
                <b>Resume</b>
                <asp:GridView ID="_resumeGrid" runat="server" AutoGenerateColumns="false" CssClass="Matrix" HeaderStyle-CssClass="header" RowStyle-CssClass="Item" AlternatingRowStyle-CssClass="AlternatingItem">
                    <Columns>
                        <asp:TemplateField HeaderText="ID">
                            <ItemTemplate>
                                <div style="margin-left:15px;margin-right:15px;padding:3px;"><%# DataBinder.Eval(Container.DataItem, "ResponseId") %></div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Started">
                            <ItemTemplate>
                                <div style="margin-left:15px;margin-right:15px;padding:3px;"><%# DataBinder.Eval(Container.DataItem, "DateStarted") %></div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Last Updated">
                            <ItemTemplate>
                                <div style="margin-left:15px;margin-right:15px;padding:3px;"><%# DataBinder.Eval(Container.DataItem, "DateLastEdited") %></div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <div style="margin-left:15px;margin-right:15px;padding:3px;">
                                    <a class="workflowAjaxGetAction" href="<%# GetResumeLink(Container) %>">Resume</a>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </asp:Panel>

            <asp:Panel ID="_editPanel" runat="server">
                <b>Edit</b>
                <asp:GridView ID="_editGrid" runat="server" AutoGenerateColumns="false" CssClass="Matrix" HeaderStyle-CssClass="header" RowStyle-CssClass="Item" AlternatingRowStyle-CssClass="AlternatingItem">
                    <Columns>
                        <asp:TemplateField HeaderText="ID">
                            <ItemTemplate>
                                <div style="margin-left:15px;margin-right:15px;padding:3px;"><%# DataBinder.Eval(Container.DataItem, "ResponseId") %></div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Started">
                            <ItemTemplate>
                                <div style="margin-left:15px;margin-right:15px;padding:3px;"><%# DataBinder.Eval(Container.DataItem, "DateStarted") %></div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Completed">
                            <ItemTemplate>
                                <div style="margin-left:15px;margin-right:15px;padding:3px;"><%# DataBinder.Eval(Container.DataItem, "DateCompleted") %></div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Last Edit">
                            <ItemTemplate>
                                <div style="margin-left:15px;margin-right:15px;padding:3px;"><%# DataBinder.Eval(Container.DataItem, "DateLastEdited") %></div>
                            </ItemTemplate>
                        </asp:TemplateField>
                         <asp:TemplateField>
                            <ItemTemplate>
                                <div style="margin-left:15px;margin-right:15px;padding:3px;">
                                    <a class="workflowAjaxGetAction" href="<%# GetEditLink(Container) %>">Edit</a>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </asp:Panel>

            <asp:Panel ID="_newResponsePanel" runat="server" style="margin-top:25px;">
                <asp:Hyperlink ID="_newResponseLink" runat="server" CssClass="workflowAjaxGetAction" Text="Start New Response >>" NavigateUrl="javascript:void(0);" />
            </asp:Panel>
        </div>
    </div>
</div>
