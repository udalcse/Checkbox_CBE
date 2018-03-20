<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="HiddenItem.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.HiddenItem" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemActiveDisplay.ascx" TagPrefix="ckbx" TagName="ActiveDisplay" %>
<%@ Import Namespace="Checkbox.Web" %>
    
    <script type="text/javascript">
        $(document).ready(function () {
            <%if(HidePreview) { %>
                $('#itemEditorTabs').hide();
                $('#itemEditorTabs-0-tabContent').hide();
                $('.tabContentContainer').show()

            <% } else { %>
                $('#itemEditorTabs').ckbxTabs({ 
                    tabName: 'itemEditorTabs',
                    initialTabIndex: $('#<%=_currentTabIndex.ClientID %>').val(),
                    onTabClick: function(index){$('#<%=_currentTabIndex.ClientID %>').val(index)},
                    onTabsLoaded: function(){$('.tabContainer').show();$('.tabContentContainer').show();}
                });
            <% } %>          
        });
    </script>

    <div style="display:none;">
        <asp:TextBox ID="_currentTabIndex" runat="server" Text="0" />
    </div>

    <ul id="itemEditorTabs" class="tabContainer">
        <li><%=WebTextManager.GetText("/pageText/formEditor.aspx/preview")%></li>
        <li><%=WebTextManager.GetText("/pageText/formEditor.aspx/edit")%></li>
    </ul>
    <div class="clear"></div>

      <div class="tabContentContainer padding10">
        <div id="itemEditorTabs-0-tabContent">
            <ckbx:ActiveDisplay ID="_activeDisplay" runat="server" />
            <asp:PlaceHolder ID="_previewPlace" runat="server"></asp:PlaceHolder>
            <div class="clear"></div>
        </div>
        <div id="itemEditorTabs-1-tabContent">
            <asp:Panel ID="_errorPanel" runat="server" Visible="false" CssClass="error message" style="margin-bottom:4px;">
                <asp:Label ID="_variableNameError" runat="server"/>
            </asp:Panel>

            <div class="formInput">
                <p><ckbx:MultiLanguageLabel ID="_textLbl" AssociatedControlID="_questionTxt" runat="server" TextId="/controlText/hiddenItemEditor/questionText" /></p>
                <asp:TextBox ID="_questionTxt" runat="server" Width="400" />
            </div>

            <br class="clear" />

            <div class="formInput">
                <p><ckbx:MultiLanguageLabel ID="_aliasLbl" AssociatedControlID="_aliasTxt" runat="server" TextId="/controlText/hiddenItemEditor/questionAlias" /></p>
                <asp:TextBox ID="_aliasTxt" runat="server" Width="400" />
            </div>

            <br class="clear" />

             <div class="formInput">
                <p><ckbx:MultiLanguageLabel ID="_variableNameLbl" AssociatedControlID="_variableNameTxt" runat="server" TextId="/controlText/hiddenItemEditor/variableName" /></p>
                <asp:TextBox ID="_variableNameTxt" runat="server" Width="400" />                
            </div>

            <br class="clear" />

            <div class="formInput radioList">
                <p><ckbx:MultiLanguageLabel ID="_itemTypeLbl" AssociatedControlID="_itemTypeList" runat="server" TextId="/controlText/hiddenItemEditor/hiddenFieldType" /></p>            
                <ckbx:MultiLanguageRadioButtonList ID="_itemTypeList" runat="server">
                    <asp:ListItem Value="QueryString" Text="Query String" TextId="/enum/hiddenVariableSource/queryString" />
                    <asp:ListItem Value="Session" Text="Session Variable" TextId="/enum/hiddenVariableSource/session" />
                    <asp:ListItem Value="Cookie" Text="Query String" TextId="/enum/hiddenVariableSource/cookie" />
                </ckbx:MultiLanguageRadioButtonList>
            </div>

            <br class="clear" />
        </div>        
    </div>

    <br class="clear" />



