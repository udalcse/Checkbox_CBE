<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="SliderOptionsEditor.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.SliderOptionsEditor" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register TagPrefix="nce" TagName="SliderOptionsNormalEntry" Src="SliderOptionsNormalEntry.ascx" %> 
<%@ Register TagPrefix="qe" TagName="QuickEntry" Src="OptionsQuickEntry.ascx" %> 

<script type="text/javascript">
     $(document).ready(function () {
         $('#selectEditorTabs').ckbxTabs({
             tabName: 'selectEditorTabs',
             tabStyle: 'inverted',
             initialTabIndex: <%=_optionTabIndex.Text %>,
             onTabClick: function (index) { onTabChange(index); },
             onTabsLoaded: function () { $('.tabContainer').show(); $('.tabContentContainer').show(); }
         });
     });

     //
     function onTabChange(newTabIndex) {
         //Set value of new tab and cause postback, so options entry dialogs can be updated
         $('#<%=_newTabIndex.ClientID%>').val(newTabIndex);

         //Simulate button click
         if (typeof (UFrameManager) == 'undefined') {
            eval($('#<%=_tabChangeBtn.ClientID %>').attr('href'));
        }
        else {
            UFrameManager.executeASPNETPostback($('#<%=_tabChangeBtn.ClientID %>'), $('#<%=_tabChangeBtn.ClientID %>').attr('href'));
        }
     }
    </script>

<asp:Panel ID="_numberRangeWarningPanel" runat="server" CssClass="warning message" style="margin-top:10px;">
    <ckbx:MultiLanguageLabel ID="_numberRangeWarningLbl" runat="server" TextId="/controlText/sliderEditor/numberRangeWarning" />
</asp:Panel>

<asp:Panel ID="_optionsEditorPanel" runat="server">  
    <div style="display:none;">
        <asp:TextBox ID="_optionTabIndex" runat="server" Text="0"/>
        <asp:TextBox ID="_newTabIndex" runat="server" Text="0" />
        <asp:LinkButton ID="_tabChangeBtn" runat="server" />
    </div>

    <ul id="selectEditorTabs" class="tabContainer">
        <li><%= WebTextManager.GetText("/controlText/listOptionsEditor/normalView") %></li>
        <% if (!AreOptionsWithImage)
           {%>
        <li><%=WebTextManager.GetText("/controlText/listOptionsEditor/quickEntry")%></li>
        <%
           }%>
    </ul>
    <div class="clear"></div>


    <div id="selectEditorTabContainer" class="padding10 tabContentContainer tabOverflow">
        <div id="selectEditorTabs-0-tabContent">
            <nce:SliderOptionsNormalEntry ID="_optionsEntry" runat="server" />
        </div>
        <%if (!AreOptionsWithImage)
          {%>
        <div id="selectEditorTabs-1-tabContent">
            <qe:QuickEntry ID="_quickEntry" runat="server" />
        </div>
        <%
          }%>

    </div>
</asp:Panel>