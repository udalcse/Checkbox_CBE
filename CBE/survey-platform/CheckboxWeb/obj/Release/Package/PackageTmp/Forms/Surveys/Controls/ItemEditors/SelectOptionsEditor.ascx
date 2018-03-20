<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="SelectOptionsEditor.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.SelectOptionsEditor" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Import namespace="Checkbox.Users" %>
<%@ Register TagPrefix="nce" TagName="NormalOptionsEntry" Src="OptionsNormalEntry.ascx" %> 
<%@ Register TagPrefix="qe" TagName="QuickEntry" Src="OptionsQuickEntry.ascx" %> 

 <script type="text/javascript">
     var quickOptionsTextChanged = false;

     $(document).ready(function () {
         $('#selectEditorTabs').ckbxTabs({
             tabName: 'selectEditorTabs',
             tabStyle: 'inverted',
             initialTabIndex: <%=_optionTabIndex.Text %>,
             onTabClick: function (index) { onTabChange(index); },
             onTabsLoaded: function () { $('.tabContainer').show(); $('.tabContentContainer').show(); }
         });
         $('.quickEntryText').change(function(){
            quickOptionsTextChanged = true;
         });

     });

     //
     function onTabChange(newTabIndex) {
        //Set value of new tab and cause postback, so options entry dialogs can be updated
         $('#<%=_newTabIndex.ClientID%>').val(newTabIndex);

         if (!newTabIndex)
         {
            if (quickOptionsTextChanged)
            {
                var optionText = $('.quickEntryText').val();
                var optionTexts = optionText.split('\n');
                var pos = 1;
                options = new Array();
                for (var i = 0; i < optionTexts.length; i++)
                {
                    if ((optionTexts[i] != null) && (typeof(optionTexts[i]) != 'undefined') && (optionTexts[i].length > 0))
                    {
                        var optionData = optionTexts[i].split(',');
                        var text = optionData[0];
                        var isDefault = '0';
                        var alias = '';
                        
                        //There are possible situations when ',' sign could be used in the text body. Let's try to catch it
                        for (var j = 1; j < optionData.length; j++) {
                            var value = optionData[j].toLowerCase();
                            if (value !== '0' && value !== '1' &&
                                value !== 'false' && value !== 'true' &&
                                value !== 'no' && value !== 'yes')
                                text += ',' + optionData[j];
                            else {
                                isDefault = value;
                                for (var k = j+1; k < optionData.length; k++) {
                                    alias += optionData[k];
                                }
                                break;
                            }
                        }

                        var option = 
                        {
                            text: text,
                            isDefault: isDefault != '0' && isDefault != 'false' && isDefault != 'no',
                            alias: alias,
                            optionId: -pos,
                            position: pos++
                        };
                        options.push(option);
                    }
                }

                sortOptions();
                renderOptions(options);
            }
         }
         else
         {
            if (typeof(options != 'undefined') && options != null)
            {
                var optionsText = '';

                for (var i = 0; i < options.length; i++)
                {
                    optionsText = optionsText + (optionsText.length ? '\n' : '') + options[i].text + ',' + options[i].isDefault + ',' + options[i].alias;
                }

                if (optionsText.length)
                {
                    $('.quickEntryText').val(optionsText);
                }
            }
         }
         //Simulate button click
         if( typeof (UFrameManager) == 'undefined' || !UFrameManager.isInit()){
            eval($('#<%=_tabChangeBtn.ClientID %>').attr('href'));
         } 
         else {
            UFrameManager.executeASPNETPostback($('#<%=_tabChangeBtn.ClientID %>'), $('#<%=_tabChangeBtn.ClientID %>').attr('href'));
         }
     }
    </script>
    
<div class="normalMode">
    <div style="display:none;">
        <asp:TextBox ID="_optionTabIndex" runat="server" Text="0"/>
        <asp:TextBox ID="_newTabIndex" runat="server" Text="0" />
        <asp:LinkButton ID="_tabChangeBtn" runat="server" />
    </div>

    <ul id="selectEditorTabs" class="tabContainer">
        <li><%= WebTextManager.GetText("/controlText/listOptionsEditor/normalView") %></li>
        <li><%= WebTextManager.GetText("/controlText/listOptionsEditor/quickEntry") %></li>
        <!--<li><%= WebTextManager.GetText("/controlText/listOptionsEditor/xmlImport") %></li>-->
    </ul>
    <div class="clear"></div>

    <%-- <ul class="tabsMenu allMenu">
        <li id="tab_0" class="active"><a href="javascript:onSelectEditorTabClick(0);"><%= WebTextManager.GetText("/controlText/listOptionsEditor/normalView") %></a></li>
        <li id="tab_1" ><a href="javascript:onSelectEditorTabClick(1);"><%= WebTextManager.GetText("/controlText/listOptionsEditor/quickEntry") %></a></li>
        <li id="tab_2" ><a href="javascript:onSelectEditorTabClick(2);"><%= WebTextManager.GetText("/controlText/listOptionsEditor/xmlImport") %></a></li>
    </ul>
    <div class="clear"></div> --%>

    <div id="selectEditorTabContainer" class="padding10 tabContentContainer tabOverflow">
        <div id="selectEditorTabs-0-tabContent">
            <nce:NormalOptionsEntry ID="_optionsEntry" runat="server" />
        </div>
        <div id="selectEditorTabs-1-tabContent">
            <qe:QuickEntry ID="_quickEntry" runat="server" />
        </div>
        <!--<div id="selectEditorTabs-2-tabContent">
            Import/Export Goes Here
        </div>-->
    </div>
</div>

<div class="customFieldMode padding10" style="display:none">
    <table class="radioFieldAliases">
        <thead style="border-bottom:1px dashed #dddddd;">
            <tr>
                <th>Option name</th>
                <th>Alias</th> 
            </tr>
        </thead>
        <tbody></tbody>
    </table>
</div>
