<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ProfileUpdater.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.ProfileUpdater" %>
<%@ Register TagPrefix="ckbx" TagName="Grid" Src="~/Controls/Grid.ascx" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemConditionEditor.ascx" TagPrefix="ckbx" TagName="ConditionEditor" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemConditionsRuleDislpay.ascx" TagPrefix="ckbx" TagName="RuleDisplay" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemActiveDisplay.ascx" TagPrefix="ckbx" TagName="ActiveDisplay" %>


    <script type="text/javascript">
         $(document).ready(function () {
            $('#itemEditorTabs').ckbxTabs({ 
                tabName: 'itemEditorTabs',
                initialTabIndex: <%=_currentTabIndex.Text %>,
                onTabClick: function(index){$('#<%=_currentTabIndex.ClientID %>').val(index)},
                onTabsLoaded: function(){$('.tabContainer').show();$('.tabContentContainer').show();}
            });

            <%if(HidePreview) { %>
                $('#itemEditorTabs').ckbxTabs('hideTab', 0);
            <% } %>

            <% if (EditMode != Checkbox.Forms.EditMode.Survey) { %>
                $('#itemEditorTabs').ckbxTabs('hideTab', 3);
            <%} %>
        });
    </script>


    <div style="display:none;">
        <asp:TextBox ID="_currentTabIndex" runat="server" Text="0" />
    </div>
    
    <ul id="itemEditorTabs" class="tabContainer">
        <li><%=WebTextManager.GetText("/pageText/formEditor.aspx/preview")%></li>
        <li><%=WebTextManager.GetText("/pageText/formEditor.aspx/edit")%></li>
        <li><%=WebTextManager.GetText("/controlText/templatePageEditor/conditions")%></li>
    </ul>

    <div class="clear"></div>

    <div class="tabContentContainer">
     <div id="itemEditorTabs-0-tabContent" class="padding10">
            <ckbx:ActiveDisplay ID="_activeDisplay" runat="server" />
            
            <asp:PlaceHolder ID="_previewPlace" runat="server"></asp:PlaceHolder>

            <div class="clear"></div>
            <hr size="1" />
            <ckbx:RuleDisplay ID="_ruleDisplay" runat="server" />
        </div>

        <div id="itemEditorTabs-1-tabContent" class="padding10">
            <!-- Existing Properties -->
            <ckbx:MultiLanguageLabel CssClass="dialogSubTitle" ID="_existingPropertiesLbl" runat="server" TextId="/controlText/profileUpdaterItemEditor/existingUpdaters" />

            <div style="margin-top:15px;">
                <asp:GridView ID="_propertiesGrid" runat="server" AutoGenerateColumns="false" CssClass="DefaultGrid" RowStyle-CssClass="EvenRow" AlternatingRowStyle-CssClass="OddRow">
                    <Columns>
                        <asp:BoundField DataField="SourceItemId" Visible="false"/>
                        <grd:ItemTextField HeaderTextID="/controlText/profileUpdaterItemEditor/sourceQuestion" DataField="SourceItemId" ItemStyle-Width="150px" HeaderStyle-HorizontalAlign="Left"></grd:ItemTextField>
                        <grd:LocalizedHeaderBoundField HeaderTextID="/controlText/profileUpdaterItemEditor/profileProvider" DataField="ProviderName" ItemStyle-Width="150px" HeaderStyle-HorizontalAlign="Left" Visible="false" />
                        <grd:LocalizedHeaderBoundField HeaderTextID="/controlText/profileUpdaterItemEditor/profileProperty" DataField="PropertyName" ItemStyle-Width="200px" HeaderStyle-HorizontalAlign="Left"/>
                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="100">
                            <ItemTemplate>
                                <btn:CheckboxButton ID="CheckboxButton1" runat="server" Text="Delete" CssClass="ckbxButton roundedCorners shadow999 border999 redButton smallButton" CommandName="Delete" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="warning message" style="margin:0;"><%=WebTextManager.GetText("/controlText/profileUpdatorItemEditor/noProperties")%></div>
                    </EmptyDataTemplate>
                </asp:GridView>

                <div class="formInput fixed_75 padding10" style="padding-bottom:0px">
                    <p><ckbx:MultiLanguageLabel AssociatedControlID="_aliasText" ID="MultiLanguageLabel1" runat="server" TextId="/controlText/listEditor/alias" /></p>
                </div>
                <div class="formInput padding10" style="padding-top:0px">
                    <asp:TextBox ID="_aliasText" runat="server" />
                </div>

            </div>

            <asp:PlaceHolder ID="_noQuestionsPlace" runat="server">
                <p>
                    <!-- No Questions Label -->
                    <ckbx:MultiLanguageLabel ID="_noQuestionsLbl" runat="server" TextId="/controlText/profileUpdaterItemEditor/noQuestionsHelperText" />
                </p>
            </asp:PlaceHolder>

            <!-- New Properties -->
            <asp:Panel ID="_addPlace" runat="server" style="margin-top:25px;">
                
                <div class="dialogSubTitle">
                    <%=WebTextManager.GetText("/controlText/profileUpdatorItemEditor/newProperty") %>
                </div>

                <div class="formInput">
                    <p><ckbx:MultiLanguageLabel AssociatedControlID="_questionList" ID="_questionLbl" runat="server" TextId="/controlText/profileUpdaterItemEditor/sourceQuestion" /></p>
                    <asp:DropDownList ID="_questionList" runat="server" uframeignore="true" />

                    <p><ckbx:MultiLanguageLabel AssociatedControlID="_propertyList" ID="_propertyLbl" runat="server" TextId="/controlText/profileUpdaterItemEditor/profileProperty" /></p>
                    <asp:DropDownList ID="_propertyList" runat="server" uframeignore="true" />

                    <br class="clear"/>

                    <btn:CheckboxButton ID="_addBtn" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton" TextID="/controlText/profileUpdaterItemEditor/add" />
                </div>
            </asp:Panel>

            <br class="clear"/>
        </div>
        
        <div id="itemEditorTabs-2-tabContent">
            <ckbx:ConditionEditor ID="_conditionEditor" runat="server" />
        </div>
    </div>
    <div class="clear"></div>