<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="SourceItemSelector.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors.SourceItemSelector" %>
<%@ Import Namespace="Checkbox.Web"%>
<%@ Import Namespace="Checkbox.Common"%>
<%@ Import Namespace="Checkbox.Forms.Data"%>

<script type="text/javascript">
    
    $(document).ready(function () {
        var sourceItemsLimit = <%= SourceItemsLimit %>;
        $(document).on("change", '#<%=_primaryItem.ClientID%>', function(){
            $('#<%=_primaryItemHiddenID.ClientID %>').val($("#<%=_primaryItem.ClientID%> option:selected").val());
        });

        $('#<%=_addBtn.ClientID%>').attr('href', 'javascript:void(0);');
        $('#<%=_removeBtn.ClientID%>').attr('href', 'javascript:void(0);');
        
        <% if(!VerticalLayout){ %>
            $('#selectorRight_<%=ID %>').css({'margin-left': '50px', 'margin-top': '25px'});
        <%} %>

        $('#<%=_addBtn.ClientID%>').click(function () {

            var checkedElemetnsCount = $('input:checked', '#<%=ID%>availableContainer').length;
            var selectedElementsCount = $('input:checkbox', '#<%=ID%>selectedContainer').length;

            if (checkedElemetnsCount  > sourceItemsLimit || selectedElementsCount >= sourceItemsLimit) {
                alert("You can not add more than " + sourceItemsLimit + " element(s)");
                return;
            }

            $('input:checkbox','#<%=ID%>availableContainer').each(function () {
                if ($(this).closest("span").hasClass('checked')) {

                    var itemId = $(this).attr('name').replace('Item_', '');

                    //Update text fields used for postback
                    <%=ID %>removeItemId(
                        '<%=_itemsToRemoveTxt.ClientID %>',
                        itemId
                    );
                    <%=ID %>addItemId(
                        '<%=_itemsToAddTxt.ClientID %>',
                        itemId
                    );

                    //Rename and move div
                    var divToMove = $('#availableContainer_' + itemId, '#<%=ID %>availableContainer');
                    divToMove.attr('id', 'selectedContainer_' + itemId);
                    $('#<%=ID %>selectedContainer').append(divToMove);

                    //Update input class
                    $(this).closest("span").removeClass("checked");
                    $(this).attr('checked', false);
                    $.uniform.update('#selectedContainer_' + itemId);
                }
            });

            //Show/hide buttons, etc. if necessary
            <%=ID %>updateItemsViews();
        });

        $('#<%=_removeBtn.ClientID%>').click(function () {
            $('input:checkbox','#<%=ID%>selectedContainer').each(function () {
                
                if ($(this).closest("span").hasClass('checked')) {
                    //Update text fields used for postback
                    var itemId = $(this).attr('name').replace('Item_', '');

                    <%=ID %>removeItemId(
                        '<%=_itemsToAddTxt.ClientID %>',
                        itemId
                    );
                    <%=ID %>addItemId(
                        '<%=_itemsToRemoveTxt.ClientID %>',
                        itemId
                    );

                    //Rename and move div
                    var divToMove = $('#selectedContainer_' + itemId, '#<%=ID %>selectedContainer');
                    divToMove.attr('id', 'availableContainer_' + itemId);
                    $('#<%=ID %>availableContainer').append(divToMove);
                    
                    //Update input class
                    $(this).attr('checked', false);
                    $(this).closest("span").removeClass("checked");
                    $.uniform.update('#availableContainer_' + itemId);
                }
            });
            //Show/hide buttons, etc. if necessary
            <%=ID %>updateItemsViews();

        });

        //update views
        <%=ID %>updateItemsViews();

        //check all available items by default
        checkAllAvailableItems();
    });

    function checkAllAvailableItems(){
        $("#_sourceItemSelectoravailableContainer input[type='checkbox']").prop("checked", true);
        $("#_sourceItemSelectoravailableContainer input[type='checkbox']").parent().addClass("checked");
    }

    function uncheckAllAvailableItems(){
        $("#_sourceItemSelectoravailableContainer input[type='checkbox']").prop("checked", false);
        $("#_sourceItemSelectoravailableContainer input[type='checkbox']").parent().removeClass("checked");
    }

    //
    function <%=ID %>updateItemsViews() {
        if ($('input:checkbox','#<%=ID%>selectedContainer').length == 0) {
            $('#<%=ID %>noSelectedItemsDiv').show();
            $('#<%=_removeBtn.ClientID %>').hide();
            $('#<%=ID %>selectedContainer').hide();
            $('#<%=_primarySelectorDiv.ClientID%>').hide();
        }
        else {
            $('#<%=ID %>noSelectedItemsDiv').hide();
            $('#<%=_removeBtn.ClientID %>').show();
            $('#<%=ID %>selectedContainer').show();
            if ($('#<%=_primaryItemHiddenID.ClientID %>').val() || $('#<%=_primaryItemHiddenID.ClientID %>').val() == "0")
            {
                $('#<%=_primarySelectorDiv.ClientID%>').show();
                fillPrimaryItemDiv<%=ID%>();
            }
            else
            {
                $('#<%=_primarySelectorDiv.ClientID%>').hide();
            }
        }

        if ($('input:checkbox','#<%=ID%>availableContainer').length == 0) {
            $('#<%=ID %>noAvailableItemsDiv').show();
            $('#<%=_addBtn.ClientID %>').hide();
            $('#<%=ID %>availableContainer').hide();
        }
        else {
            $('#<%=ID %>noAvailableItemsDiv').hide();
            $('#<%=_addBtn.ClientID %>').show();
            $('#<%=ID %>availableContainer').show();
        }

      <%--  sortSourceItems($('#<%=ID%>selectedContainer'));
        sortSourceItems($('#<%=ID%>availableContainer'));--%>
    }

    //function sortSourceItems(parent) {
    //    debugger;
    //    var items = parent.children().sort(function (a, b) {
    //        var posA = $(a).find('.itemNumber').text();
    //        var posB = $(b).find('.itemNumber').text();

    //        var fA = parseFloat(posA);
    //        var fB = parseFloat(posB);

    //        if (fA === fB) {
    //            fA = parseMatrixChildPosition(posA);
    //            fB = parseMatrixChildPosition(posB);
    //        }

    //        return (fA < fB) ? -1 : (fA > fB) ? 1 : 0;
    //    });
    //    parent.append(items);
    //}

    function parseMatrixChildPosition(pos) {
        var dot1 = pos.indexOf('.');
        var dot2 = pos.indexOf('.', dot1 + 1);
        return parseFloat(pos.substr(dot2 + 1));
    }

    function fillPrimaryItemDiv<%=ID%>()
    {
        var oldVal = $('#<%=_primaryItemHiddenID.ClientID %>').val();
        $('#<%=_primaryItem.ClientID %>').find('option').remove();
        
        $('input:checkbox','#<%=ID%>selectedContainer').each(function f(i)
        {            
            var id = $(this).attr("name").replace("Item_", "");
            $('#<%=_primaryItem.ClientID %>').append('<option value="' + id + '">' +
                $($("#selectedContainer_" + id).children()[1]).text()
                + '</option>');
        });
        $('#<%=_primaryItem.ClientID %>').find('option[value=' + oldVal + ']').attr('selected', 'selected');
        
        $('#<%=_primaryItemHiddenID.ClientID %>').val(oldVal);
        $('#<%=_primaryItem.ClientID %>').append('<option value="0"><%=WebTextManager.GetText("/controlText/chartEditor/primarySourceItem/na") %></option>');
        $('#<%=_primaryItemHiddenID.ClientID %>').val($("#<%=_primaryItem.ClientID%> option:selected").val());
        $.uniform.update('#<%=_primaryItem.ClientID %>');
    }

    //
    function <%=ID %>addItemId(textBoxId, itemId) {
        var boxVal = $('#' + textBoxId).val();

        if (boxVal != '' && boxVal != null) {
            $('#' + textBoxId).val(boxVal + ',' + itemId);
        }
        else {
            $('#' + textBoxId).val(itemId);
        }
    }

    //
    function <%=ID %>removeItemId(textBoxId, itemId) {
        var boxVal = $('#' + textBoxId).val();
        boxVal = boxVal.replace(itemId, '');
        boxVal = boxVal.replace(',,', ',');

        $('#' + textBoxId).val(boxVal);
    }
</script>

<div style="display:none;">
    <asp:TextBox ID="_itemsToAddTxt" runat="server" />
    <asp:TextBox ID="_itemsToRemoveTxt" runat="server" />
</div>

<div class="fixed_300 left">
    <a class="ckbxButton roundedCorners border999 shadow999 silverButton" onclick="checkAllAvailableItems()">Check All</a>
    <a class="ckbxButton roundedCorners border999 shadow999 silverButton" onclick="uncheckAllAvailableItems()">Uncheck All</a>
    <div class="border999 shadow999 dashStatsWrapper">
        <div class="dialogSubTitle blueBackground"><%= Utilities.IsNotNullOrEmpty(TitleTextId) ? WebTextManager.GetText(TitleTextId) : WebTextManager.GetText("/controlText/chartEditor/availableItems") %></div>
    
        <div id="<%=ID %>noAvailableItemsDiv" class="padding10">
            <%=WebTextManager.GetText("/controlText/chartEditor/noItems")%>
        </div>
        
        <div style="height:<%= ItemsContainerHeight %>px;overflow-y:scroll;" id="<%=ID %>availableContainer">
            <asp:ListView ID="_availableList" runat="server">
                <LayoutTemplate>
                <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
                </LayoutTemplate>
                <ItemTemplate>
                    <div class="dashStatsContent zebra" id="availableContainer_<%#Eval("ItemId") %>">
                        <div class="itemNumber hidden"><%# (Container.DataItem as LightweightItemMetaData).PositionText %></div>
                        <div class="fixed_25 left"><input type="checkbox" name="Item_<%#Eval("ItemId") %>" /></div>
                        <div class="left fixed_225"><%# GetItemNumber(Container.DataItem as LightweightItemMetaData)%> - <%# GetItemText(Container.DataItem as LightweightItemMetaData)%></div>
                        <br class="clear" />
                    </div>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <div class="dashStatsContent detailZebra" id="availableContainer_<%#Eval("ItemId") %>">
                        <div class="itemNumber hidden"><%# (Container.DataItem as LightweightItemMetaData).PositionText %></div>
                        <div class="fixed_25 left"><input type="checkbox" name="Item_<%#Eval("ItemId") %>" /></div>
                        <div class="left fixed_225"><%# GetItemNumber(Container.DataItem as LightweightItemMetaData)%> - <%# GetItemText(Container.DataItem as LightweightItemMetaData)%></div>
                        <br class="clear" />
                    </div>
                </AlternatingItemTemplate>
            </asp:ListView>
        </div>
    </div>
    <div class="right">
        <btn:CheckboxButton ID="_addBtn" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 silverButton" TextID="/controlText/chartEditor/addSelectedItems" uframeignore="true" />
    </div>
</div>

<div class="fixed_300 left" id="selectorRight_<%=ID %>">
    <div class="border999 shadow999 dashStatsWrapper">
        <div class="dialogSubTitle blueBackground"><%= Utilities.IsNotNullOrEmpty(TitleTextId) ? WebTextManager.GetText(TitleTextId) : WebTextManager.GetText("/controlText/chartEditor/sourceItems") %></div>

        <div id="<%=ID %>noSelectedItemsDiv" class="padding10">
            <%=WebTextManager.GetText("/controlText/chartEditor/noItems")%>
        </div>
        <div style="height:<%= ItemsContainerHeight %>px;overflow-y:scroll;" id="<%=ID %>selectedContainer">
            <asp:ListView ID="_selectedList" runat="server">
                <LayoutTemplate>
                    <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
                </LayoutTemplate>
                <ItemTemplate>
                    <div class="dashStatsContent zebra" id="selectedContainer_<%#Eval("ItemId") %>">
                        <div class="itemNumber hidden"><%# (Container.DataItem as LightweightItemMetaData).PositionText %></div>
                        <div class="fixed_25 left"><input type="checkbox" name="Item_<%#Eval("ItemId") %>" /></div>
                        <div class="left fixed_225"><%# GetItemNumber(Container.DataItem as LightweightItemMetaData)%> - <%# GetItemText(Container.DataItem as LightweightItemMetaData)%></div>
                        <br class="clear" />
                    </div>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <div class="dashStatsContent detailZebra" id="selectedContainer_<%#Eval("ItemId") %>">
                        <div class="itemNumber hidden"><%# (Container.DataItem as LightweightItemMetaData).PositionText %></div>
                        <div class="fixed_25 left"><input type="checkbox" name="Item_<%#Eval("ItemId") %>" /></div>
                        <div class="left fixed_225"><%# GetItemNumber(Container.DataItem as LightweightItemMetaData)%> - <%# GetItemText(Container.DataItem as LightweightItemMetaData)%></div>
                        <br class="clear" />
                    </div>
                </AlternatingItemTemplate>
            </asp:ListView>
        </div>
    </div>
    <div class="right">
            <btn:CheckboxButton ID="_removeBtn" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 redButton" TextID="/controlText/chartEditor/removeSelectedItems" uframeignore="true" />
    </div>
    <br class="clear" />
</div>

<br class="clear" />
<div id="_primarySelectorDiv" runat="server" class="formInput">
    <p>
        <ckbx:MultiLanguageLabel ID="_primaryItemLbl" AssociatedControlID="_primaryItem" runat="server" TextId="/controlText/chartEditor/primarySourceItem" />
    </p>
    <ckbx:MultiLanguageDropDownList ID="_primaryItem" runat="server" ToolTipTextId="/controlText/chartEditor/primarySourceItem/title">
    </ckbx:MultiLanguageDropDownList>
    <asp:HiddenField ID="_primaryItemHiddenID" runat="server" />
</div>
<br class="clear" />

