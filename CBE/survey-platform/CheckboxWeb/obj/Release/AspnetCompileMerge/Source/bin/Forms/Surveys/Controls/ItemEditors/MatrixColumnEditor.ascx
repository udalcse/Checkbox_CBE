<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="MatrixColumnEditor.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.MatrixColumnEditor" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Forms.Data" %>
<%@ Import Namespace="Checkbox.Forms.Items.Configuration" %>
<%@ Import Namespace="Checkbox.Web" %>

<script type="text/javascript">
    $(document).ready(function () {
       
        <%if (!IsNew) //If this dialog is shown just after item creation, the control will use another click handlers.
          {%>        
            //Register callback for column edited
            templateEditor.registerCallback('dialogCallback_editMatrixColumn', onMatrixColumnEdited);
            templateEditor.registerCallback('dialogCallback_addMatrixColumn', onMatrixColumnAdded);

            $('.columnSelect').bind('mouseover', function () { $(this).addClass('hover') });
            $('.columnSelect').bind('mouseout', function () { $(this).removeClass('hover') });
            $('.columnSelect').bind('click', editColumnClick);

            $('#<%=addColumnBtn.ClientID %>').attr('href', 'javascript:void(0);');

            //Add column click
            $('#<%=addColumnBtn.ClientID %>').click(function () {
                var params = new Array(
                    {name: 'c', value: '<%=MatrixItemTextDecorator.Data.ColumnCount + 1 %>'}
                    <%if (AreColumnsCategorized) {%>
                        ,{name: 'categorized', value: 'true'}
                    <%} %>
                );
                        
                templateEditor.openChildWindow(<%=MatrixItemTextDecorator.Data.ID %>, <%=PagePosition %>, 'AddMatrixColumn.aspx', params, 'wizard');
            });

            //Edit column click
            $('a[columnNumber]').click(editColumnClick);
        <%
          }%>
        
        //Delete column
        $('a[deletedColumnNumber]').click(function(e){            
            onDeleteColumnClick($(this).attr('deletedColumnNumber'));
            e.stopPropagation();
        });

        //Sort
         $('#matrixColumnSortList').sortable({
            axis: 'y',
            update:onMatrixColumnReorder
        });
    });

    //
    function editColumnClick(){
        var columnNumber = $(this).attr('columnNumber');
       
        if(columnNumber == null || columnNumber == '0' || columnNumber == '-1'){
            return;
        }

        var params = new Array(
            {name: 'c', value: columnNumber}
        );
                        
        templateEditor.openChildWindow(<%=MatrixItemTextDecorator.Data.ID %>, <%=PagePosition %>, 'EditMatrixColumn.aspx', params, 'largeDialog');   
    }

    //
    function executeColumnEditorPostback(elementId) {
        if (typeof (UFrameManager) == 'undefined') {
            eval($('#' + elementId).attr('href'));
        }
        else {
            UFrameManager.executeASPNETPostback($('#' + elementId), $('#' + elementId).attr('href'));
        }
    }

    
    //Delete column click
    function onDeleteColumnClick(columnNumber){
        if(confirm('Are you sure you want to delete this column?'))
        {
            $('#<%=_columnToDelete.ClientID %>').val(columnNumber);
            executeColumnEditorPostback('<%=_deleteColumn.ClientID %>');
        }
    }


    //
    function onMatrixColumnEdited(arg){
        executeColumnEditorPostback('<%=_refreshColumns.ClientID %>');
    }

    //
    function onMatrixColumnAdded(arg){
        executeColumnEditorPostback('<%=_refreshColumns.ClientID %>');
    }


    //
    function onMatrixColumnReorder(event, ui){
        //Store new column order in hidden field to be saved when item updated
        var orderArray =  $('#matrixColumnSortList').sortable('toArray');
        
        var orderString = '';

        for(var i = 0; i < orderArray.length; i++){
            if(orderArray[i] != null && orderArray[i] != ''){
                if(i > 0){
                    orderString = orderString + ',';
                }

                orderString = orderString + orderArray[i].replace('matrixColumn_', '');
            }            
        }
        $('#<%=_newColumnOrder.ClientID %>').val(orderString);

         //Update alternating styles
        $('#matrixColumnSortList').children('ul').each(function (index) {
            //alert($(this).html());
            if (index % 2 == 1) {
                $(this).attr('class', 'allMenu dashStatsContent');
            } else {
                $(this).attr('class', 'allMenu dashStatsContent detailZebra');
            }
        });
    }

</script>
    <div style="display:none;">
        <asp:TextBox ID="_newColumnOrder" runat="server" Text="" />
        <asp:TextBox ID="_columnToDelete" runat="server" Text="" />
        <asp:LinkButton ID="_refreshColumns" runat="server" Text="Refresh" />
        <asp:LinkButton ID="_deleteColumn" runat="server" Text="Delete Column" />
    </div>
   
    <div class="clear"></div>

    <%-- Headers --%>
    <div class="statsContentHeader">
        <div class="left input fixed_25">&nbsp;</div>
        <div class="left input fixed_25">#</div>
        <div class="left input fixed_150">Text</div>
        <div class="left input fixed_100">Type</div>
        <div class="left input fixed_125">Alias</div>
        <div class="left input fixed_75">Unique</div>
        <div class="left input fixed_75">Width</div>
        <div class="left input fixed_75">Delete</div>
        <div class="clear"></div>
    </div>
    <div class="clear"></div>
    <div id="matrixColumnSortList" class="overflowPanel_400">
        <% 
            for(int columnNumber = 1; columnNumber <= MatrixItemTextDecorator.Data.ColumnCount; columnNumber++)
            {
        %>
            <div columnNumber="<%=columnNumber %>" id="matrixColumn_<%=columnNumber %>" class="<%= GetRowStyle(columnNumber) %>">
                <div class="upDownArrow left fixed_25">
                    &nbsp;
                </div>
                <div class="left input fixed_25">
                    <b><%=GetColumnEditLink(columnNumber) %></b>
                </div>
                <div sortColumn="true" class="left input fixed_150"><b><%=GetColumnText(columnNumber) %></b>&nbsp;</div>
                <div class="left input fixed_100"><%=GetColumnTypeName(columnNumber) %></div>
                <div class="left input fixed_125"><%=GetColumnAlias(columnNumber) %>&nbsp;</div>
                <div class="left input fixed_75"><%=GetColumnUniqueness(columnNumber) %>&nbsp;</div>
                <div class="left input fixed_75"><%=GetColumnWidth(columnNumber) %>&nbsp;</div>
                <div class="left input fixed_75">
                    <% if(columnNumber != MatrixItemTextDecorator.Data.PrimaryKeyColumnIndex) {%>
                    <a deletedColumnNumber='<%=columnNumber %>' class="deleteColumn ckbxButton roundedCorners border999 shadow999 redButton OptionEditorButton" uframeignore="true">-</a>
                    <%} %>
                </div>
                <div class="clear"></div>
            </div>
            <div class="clear"></div>
        <%} %>
        <div class="clear"></div>
    </div>
    <div class="clear"></div>
    <ul id="newColumn" class="<%= GetRowStyle(MatrixItemTextDecorator.Data.ColumnCount + 1) %>">
        <li class="fixed_25">&nbsp;</li>
        <li class="fixed_150">
            <btn:CheckboxButton ID="addColumnBtn" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton" TextID="/controlText/tabularItemEditorGrid/addColumn" uframeignore="true" style="color:white;" />
        </li>
    <div class="clear"></div>
    


<script type="text/C#" runat="server">
    /// <summary>
    /// 
    /// </summary>
    /// <param name="columnNumber"></param>
    /// <returns></returns>
    protected string GetRowStyle(int columnNumber)
    {
        if (columnNumber == MatrixItemTextDecorator.Data.PrimaryKeyColumnIndex)
        {
            return "allMenu dashStatsContent";
        }

        if (columnNumber > MatrixItemTextDecorator.Data.ColumnCount)
        {
            return columnNumber % 2 == 1 ? "allMenu dashStatsContent" : "allMenu dashStatsContent detailZebra";
        }
        
        return columnNumber % 2 == 1 ? "columnSelect allMenu dashStatsContent" : "columnSelect allMenu dashStatsContent detailZebra";
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="columnNumber"></param>
    /// <returns></returns>
    protected string GetColumnEditLink(int columnNumber)
    {
        if(columnNumber == MatrixItemTextDecorator.Data.PrimaryKeyColumnIndex)
            return columnNumber.ToString();

        if (IsNew)
            return string.Format("<a href=\"javascript:__doPostBack('_editColumnLink','{0}')\">{1}</a>", columnNumber, columnNumber);

        return string.Format("<a uframeignore=\"true\" columnNumber=\"{0}\" href=\"javascript:void(0);\">{1}</a>", columnNumber, columnNumber);
    }

    /// <summary>
    /// Get type name of matrix column
    /// </summary>
    /// <param name="columnNumber"></param>
    /// <returns></returns>
    protected string GetColumnTypeName(int columnNumber)
    {
        if (columnNumber == MatrixItemTextDecorator.Data.PrimaryKeyColumnIndex)
        {
            return "Row Texts";
        }
        
        var protoypeItemId = MatrixItemTextDecorator.Data.GetColumnPrototypeId(columnNumber);

        if (protoypeItemId <= 0)
        {
            return "[NO ID]";
        }

        var itemTypeName = ItemConfigurationManager.GetItemTypeName(protoypeItemId);

        return WebTextManager.GetText("/itemType/" + itemTypeName + "/name", WebTextManager.GetUserLanguage(), itemTypeName);
    }
    
    /// <summary>
    /// Get text for a column
    /// </summary>
    /// <param name="columnNumber"></param>
    /// <returns></returns>
    protected string GetColumnText(int columnNumber)
    {
        return Utilities.StripHtml(MatrixItemTextDecorator.GetColumnText(columnNumber), 22);
    }

    /// <summary>
    /// Get text for a column
    /// </summary>
    /// <param name="columnNumber"></param>
    /// <returns></returns>
    protected string GetColumnAlias(int columnNumber)
    {
        var protoypeItemId = MatrixItemTextDecorator.Data.GetColumnPrototypeId(columnNumber);

        if (protoypeItemId <= 0)
        {
            return string.Empty;
        }

        var itemData = SurveyMetaDataProxy.GetItemData(protoypeItemId, false);

        return itemData != null
                   ? itemData.Alias
                   : string.Empty;
    }

    /// <summary>
    /// Get uniqueness image
    /// </summary>
    /// <param name="columnNumber"></param>
    /// <returns></returns>
    protected string GetColumnUniqueness(int columnNumber)
    {
        var protoypeItemId = MatrixItemTextDecorator.Data.GetColumnPrototypeId(columnNumber);

        if (protoypeItemId <= 0)
        {
            return string.Empty;
        }

        if (MatrixItemTextDecorator.Data.GetColumnUniqueness(columnNumber))
        {
            return "<img src=\"" + ResolveUrl("~/App_Themes/CheckboxTheme/Images/check.png") + "\" />";
        }

        return string.Empty;
    }

    /// <summary>
    /// Get width
    /// </summary>
    /// <param name="columnNumber"></param>
    /// <returns></returns>
    protected string GetColumnWidth(int columnNumber)
    {
        var protoypeItemId = MatrixItemTextDecorator.Data.GetColumnPrototypeId(columnNumber);
        var width = MatrixItemTextDecorator.Data.GetColumnWidth(columnNumber);
        
        if (protoypeItemId <= 0
            || !width.HasValue)
        {
            return string.Empty;
        }

        return width.ToString();
    }
</script>
