<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="HorizontalLine.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.HorizontalLine" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemConditionEditor.ascx" TagPrefix="ckbx" TagName="ConditionEditor" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemConditionsRuleDislpay.ascx" TagPrefix="ckbx" TagName="RuleDisplay" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemActiveDisplay.ascx" TagPrefix="ckbx" TagName="ActiveDisplay" %>

  <script type="text/javascript">
        $(document).ready(function () {
            $('#<%=ID %>_Color').mColorPicker({imageFolder:'<%=ResolveUrl("~/App_Themes/CheckboxTheme/images") %>/'});
            
            $('#<%=_widthTxt.ClientID %>').numeric({ decimal: false, negative: false });
            $('#<%=_thicknessTxt.ClientID %>').numeric({ decimal: false, negative: false });
            
            $('#itemEditorTabs').ckbxTabs({ 
                tabName: 'itemEditorTabs',
                initialTabIndex:<%=_currentTabIndex.Text %>,
                onTabClick: function(index){$('#<%=_currentTabIndex.ClientID %>').val(index)},
                onTabsLoaded: function(){$('.tabContainer').show();$('.tabContentContainer').show();}
            });

            <%if(HidePreview) { %>
                $('#itemEditorTabs').ckbxTabs('hideTab', 0);
            <% } %>

            <% if (EditMode != Checkbox.Forms.EditMode.Survey) { %>
                $('#itemEditorTabs').ckbxTabs('hideTab', 2);
            <%} %>
        });

        //
        function <%=ID %>UpdateSelectedColor(newColor){
            $('#<%=_selectedColor.ClientID %>').val(newColor);
        }
    </script>

    <div style="display:none;">
        <asp:TextBox ID="_currentTabIndex" runat="server" Text="0" />
        <asp:TextBox ID="_selectedColor" runat="server" />
    </div>

    <ul id="itemEditorTabs" class="tabContainer">
        <li><%=WebTextManager.GetText("/pageText/formEditor.aspx/preview")%></li>
        <li><%=WebTextManager.GetText("/pageText/formEditor.aspx/edit")%></li>
        <li><%=WebTextManager.GetText("/controlText/templatePageEditor/conditions")%></li>
    </ul>

    <div class="tabContentContainer">
        <div id="itemEditorTabs-0-tabContent" class="padding10">
            <ckbx:ActiveDisplay ID="_activeDisplay" runat="server" />
            
            <asp:PlaceHolder ID="_previewPlace" runat="server"></asp:PlaceHolder>
            
            <br class="clear" />
            <hr size="1" />
            <ckbx:RuleDisplay ID="_ruleDisplay" runat="server" />
            <br class="clear" />
        </div>

        <div id="itemEditorTabs-1-tabContent" class="padding10">
            <div class="formInput">
                <p><ckbx:MultiLanguageLabel AssociatedControlID="_widthTxt" runat="server" ID="_widthLbl" TextId="/controlText/horizontalLineEditor/width" /></p>
                <asp:TextBox ID="_widthTxt" runat="server" Width="50px" />
                <ckbx:MultiLanguageDropDownList ID="_unitList" runat="server">
                    <asp:ListItem Text="%" Value="Percent" TextId="/controlText/horizontalLineEditor/percent" />
                    <asp:ListItem Text="Pixels" Value="Pixels" TextId="/controlText/horizontalLineEditor/pixels" />
                </ckbx:MultiLanguageDropDownList>
            </div>

            <br class="clear" />

            <div class="formInput">
                <p><ckbx:MultiLanguageLabel AssociatedControlID="_thicknessTxt" runat="server" ID="_unitLbl" TextId="/controlText/horizontalLineEditor/thickness" /></p>
                <asp:TextBox ID="_thicknessTxt" Width="50px" runat="server" />
            </div>

            <br class="clear" />
            
            <div class="formInput">
                <div class="left">
                    <p><label><%=WebTextManager.GetText("/controlText/horizontalLineEditor/color")%></label></p>
                </div>
                <div class="left" style="margin-left:10px;margin-top:5px;">
                    <input name="<%=ID %>_Color" id="<%=ID %>_Color" type="fpp" hex="true" text="hidden" style="width:20px;height:20px;display:none;" value="<%=_selectedColor.Text %>" onchange="<%=ID %>UpdateSelectedColor(this.value);" />
                </div>
            </div>

            <br class="clear" />
        </div>        
        
        <div id="itemEditorTabs-2-tabContent">
            <ckbx:ConditionEditor ID="_conditionEditor" runat="server" />
        </div>    
    </div>