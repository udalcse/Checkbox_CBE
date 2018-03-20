<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="MatrixRowEditor.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.MatrixRowEditor" %>
<%@ Register TagPrefix="nce" TagName="NormalEntry" Src="MatrixRowsNormalEntry.ascx" %> 
<%@ Register TagPrefix="qe" TagName="QuickEntry" Src="MatrixRowsQuickEntry.ascx" %> 

    
    <script type="text/javascript">
        //Handle tab click
        function onRowEntryTypeTabClick(tabIndex) {
            $('#rowEntryItemTabContainer > div').each(function (index) {
                if (tabIndex == index) {
                    $(this).show();
                }
                else {
                    $(this).hide();
                }
            });
        }
    </script>

    <!--
    <ul class="itemActionMenu allMenu">
        <li class="groupAttributes"><a href="javascript:onRowEntryTypeTabClick(0);">Normal</a></li>
        <li class="groupAttributes"><a href="javascript:onRowEntryTypeTabClick(1);">Quick Entry</a></li>
    </ul>
    -->
    <div class="clear"></div>

    <div id="rowEntryItemTabContainer" class="padding10" style='height:<%=!string.IsNullOrEmpty(Request["isNew"]) ? "400" : "500" %>px;overflow:auto;'>
        <div>
            <nce:NormalEntry ID="_normalEntry" runat="server" />
        </div>
        <div style="display:none;">
            <qe:QuickEntry ID="_quickEntry" runat="server" />
        </div>
        <div class="clear"></div>
    </div>