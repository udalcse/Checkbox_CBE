<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="MatrixRowsNormalEntry.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.MatrixRowsNormalEntry" %>
<%@ Register src="~/Controls/Piping/PipeControl.ascx" tagname="PipeSelector" tagprefix="pipe" %>

<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Web"%>

<script type="text/javascript">

    $(document).ready(function () {
        debugger;
        //Clear current edit row
        $('#<%= _currentEditRow.ClientID %>').val('');
        $('#<%= _cancelRowButton.ClientID %>').attr('href', 'javascript:void(0);');

        //Sort
        $('#matrixRowSortList').sortable({
            axis: 'y',
            update: onMatrixRowReorder
        });

        //Click
        $('.editRowLink').bind('click', onRowClick);
        $('.deleteRowLink').bind('click', onRowDeleteClick);
        $('.editHtmlLink').bind('click', onEditHtmlClick);

        $('.rowSelect').bind('mouseover', function () { $(this).addClass('hover'); });
        $('.rowSelect').bind('mouseout', function () { $(this).removeClass('hover'); });
        
        $('.matrixRowText').bind('click', onRowTextClick);
        $('.matrixRowAlias').bind('click', onRowAliasClick);
        
        //Focus
        $('#<%=_newMatrixRowText.ClientID %>').bind('click', function () {
            onRowEditCancel($('#<%= _currentEditRow.ClientID %>').val());
            $('#<%= _currentEditRow.ClientID %>').val('');
            $('#<%=_newMatrixRowText.ClientID %>').focus();
        });

        //
        $('#<%=_cancelRowButton.ClientID %>').click(function (e) {
            e.stopPropagation();
            onRowEditCancel($('#<%= _currentEditRow.ClientID %>').val());
            $('#<%= _currentEditRow.ClientID %>').val('');
        });
        
        //prevent empty titles for rows
        $('#<%=_addRowLink.ClientID %>').attr('oldhref', $('#<%=_addRowLink.ClientID %>').attr('href'));
        $('#<%=_addRowLink.ClientID %>').attr('href', '#');

        $('#<%=_addRowLink.ClientID %>').click(function (e) {
            e.stopPropagation();

            //strip possible script injections
            var text = $('#<%=_newMatrixRowText.ClientID %>').val();
            $('#<%=_newMatrixRowText.ClientID %>').val(stripScripts(text));
            
            //strip script for alias
            var alias = $('#<%=_newMatrixRowAlias.ClientID %>').val();
            $('#<%=_newMatrixRowAlias.ClientID %>').val(stripScripts(alias));            

            if (!($.trim(text))) {
                showMessage('<%=WebTextManager.GetText("/pageText/editMatrixRows.aspx/rowTextIsRequired") %>', '', 300, 150, function(){$('#<%=_newMatrixRowText.ClientID %>').focus();});                
            } else {
                executeRowEditorPostback('<%=_addRowLink.ClientID %>', 'oldhref');
            }
        });
        
        //prevent empty titles for rows
        $('#<%=_updateRowButton.ClientID %>').attr('oldhref', $('#<%=_updateRowButton.ClientID %>').attr('href'));
        $('#<%=_updateRowButton.ClientID %>').attr('href', '#');
        
        $('#<%=_updateRowButton.ClientID %>').click(function (e) {
          
            e.stopPropagation();

            var num =  $('#<%=_updateRowButton.ClientID %>').parent().attr('id').replace('rowButtons_', '');
            
            //strip possible script injections
            var textInputId = '#rowTextInput_' + num;
            var textEditedText = stripScripts($(textInputId).val());
            $(textInputId).val(textEditedText);
            if ($('#rowText_' + num + ':visible') && !($.trim(textEditedText))) {
                var rowText = $('#rowText_' + num).html();
                if (isHtml(rowText)) {
                    textEditedText = rowText;
                    $('#<%= _updatedRowText.ClientID %>').val(rowText);
                }
            }

            //strip script for alias input
            var aliasInputId = '#rowAliasInput_' + num;
            $('#<%= _updatedRowAlias.ClientID %>').val(stripScripts($(aliasInputId).val()));

            if (!($.trim(textEditedText))) {
                showMessage('<%=WebTextManager.GetText("/pageText/editMatrixRows.aspx/rowTextIsRequired") %>', '', 300, 150, function(){$(textInputId).focus();});                
            } else {
                executeRowEditorPostback('<%=_updateRowButton.ClientID %>', 'oldhref');
            }
        });
        
        //
        $('#<%=_newMatrixRowText.ClientID%>').bind('keypress', onNewRowKeyPress);
        $('#<%=_newMatrixRowAlias.ClientID %>').bind('keypress', onNewRowKeyPress);

        $('#<%=_newMatrixRowText.ClientID %>').focus();

        $.each($('.matrixRowText'), function (ind, elem) {
            debugger;
            var text = $(elem).text();
            if(isHtml(text)) {
                $(elem).html(text);
            }
        });

        $(document).on("change", '#<%= _newRowType.ClientID %>', function() {
            debugger;
            if($(this).val() == 'Other') {
                $('#<%= _htmlEditorLink.ClientID %>').hide();
            } else {
                $('#<%= _htmlEditorLink.ClientID %>').show();
            }
        });
    });
    
    //Handle row added by clearing inputs and setting focus
    function onRowAdded(openHtmlRedactor) {
        debugger;
        $('#<%=_newRowType.ClientID %>').val('Normal');
        $('#<%=_newMatrixRowText.ClientID %>').val('');
        $('#<%=_newMatrixRowAlias.ClientID %>').val('');
        
        $('#<%=_newMatrixRowText.ClientID %>').focus();
        
        if (openHtmlRedactor) {
            goToEditor($('.rowSelect').last().find('.editHtmlLink').attr('rowNumber'));
        }
    }

    function onRowTextClick() {
        debugger;
        var newEditRow = $(this).attr('id').replace('rowText_','');
        onRowClick("text", newEditRow);	    
    }
    
    function onRowAliasClick() {
        var newEditRow = $(this).attr('id').replace('rowAlias_','');
        onRowClick("alias", newEditRow);
    }
    
    //
    function onRowClick(target, newEditRow) {
        debugger;
        if (typeof(newEditRow) == 'undefined')
            newEditRow = $(this).attr("rownumber");
        var isHtmlOption = isHtml($('#rowText_' + newEditRow).html());

        if (target == "text" && isHtmlOption) {
            $('#editHtmlRowLink_' + newEditRow).click();
            return;
        }

        var currentRow = $('#<%= _currentEditRow.ClientID %>').val();

        if (newEditRow == currentRow) {
            return;
        }

        if (currentRow != null && currentRow != '') {
            onRowEditCancel(currentRow);
        }

        $('#<%=_currentEditRow.ClientID %>').val(newEditRow);

        onRowEdit(newEditRow, isHtmlOption);
    }

    var lastRow = -1;

    //
    function executeRowEditorPostback(elementId, tag) {
        debugger;
        if (typeof(tag) == 'undefined')
            tag = 'href';

        var link = $('#' + elementId).attr(tag);
        if (typeof (UFrameManager) == 'undefined') 
            eval(link);
        else 
            UFrameManager.executeASPNETPostback($('#' + elementId), link);
    }

    //
    function onEditHtmlClick(e) {
        debugger;
        e.stopPropagation();
        goToEditor($(this).attr('rowNumber'));
    }

    function goToEditor(currentRow) {
        debugger;
        <% if(MatrixItemTextDecorator == null || MatrixItemTextDecorator.Data == null) { %>
            return;
        <% } %>
        
        var html = $('#rowText_' + currentRow).html().replace(/#/g, '%23').replace(/&/g, '%26');
        var Data_WithOuthtml=escape(html);
        html=Data_WithOuthtml;
        <% if(IsNew) { %>
            $('#<%= _currentrow.ClientID %>').val(currentRow);
            $('#<%= _currenthtml.ClientID %>').val(html);

            window.location = $('#<%= _postRows.ClientID %>').attr('href');
            //redirectToHtmlEditor(currentRow, html);
            return;
        <% } %>

        //HandleHtml(html);

        var params = new Array({name: 'html', value: html}, {name: 'row', value: currentRow}, {name: 'callback', value: 'onHtmlDialogClosed'});

        templateEditor.openChildWindow(<%=MatrixItemTextDecorator.Data.ID %>, <%=PagePosition %>, 'HtmlEditor.aspx', params, null /*function(){
            
            $($('#simplemodal-data iframe').contents().find('textarea[tinymce="true"]').val()).appendTo($('#textHelper'));
            var text = trim($('#textHelper').text());

            if (!text)
            {
                $('#simplemodal-data iframe').contents().find('#emptyTextWarning').show();
                setTimeout(function(){$('#simplemodal-data iframe').contents().find('#emptyTextWarning').hide('slow');}, 3000);
                return false;
            }
            return true;
        }*/
        );
    }

    

    //
    function redirectToHtmlEditor(row, html) {
        debugger;
        html = html.replace(/&/g, '%26');

        var url = addParamsToUrl('<%= ResolveUrl("~/Forms/Surveys/HtmlEditor.aspx") %>?s=<%=SurveyId%>', new Array(
            { name: 'p', value: '<%= PagePosition %>' },
            { name: 'i', value: '<%= ItemId %>' },
            { name: 'l', value: '<%= LanguageCode %>' },
            { name: 'row', value: row },
            { name: 'isNew', value: '<%= IsNew %>' },
            { name: 'lid', value: '<%= LibraryTemplateId %>' },
            { name: 'html', value: html },
            { name: 'w', value: '<%= (Request == null || string.IsNullOrEmpty(Request["w"])) ? "" : Request["w"] %>' }
        ));

        location.href = url;
    }
    
    function addParamsToUrl(url, params) {
        if (url == null) {
            url = '';
        }

        //TODO: URL Encode values

        if (params != null && params.length > 0) {
            for (var i = 0; i < params.length; i++) {
                var param = params[i];

                if (param.name != null
                    && param.name != ''
                        && param.value != null
                            && param.value != '') {
                    url += '&' + param.name + '=' + param.value;
                }
            }
        }

        return url.replace('//', '/');
    };

    //
    function onHtmlDialogClosed(args) {
        debugger;
        $('#rowText_' + args.rowNumber).html(htmlDecode(args.html));
        //$('#rowText_' + args.rowNumber).text(htmlDecode(args.html));
        $('#<%= _updatedRowText.ClientID %>').val(htmlDecode(args.html));
        $('#<%= _currentEditRow.ClientID %>').val(args.rowNumber);
        executeRowEditorPostback('<%=_updateRowButton.ClientID %>', 'oldhref');
    }

    //
    function onRowDeleteClick(e) {
        e.stopPropagation();
        if (confirm('<%=WebTextManager.GetText("/pageText/editMatrixRows.aspx/deleteConfirm") %>')) {            
            var currentRow = $('#<%= _currentEditRow.ClientID %>').val();

            if (currentRow != null && currentRow != '') {
                onRowEditCancel(currentRow);
            }

            var deleteLinkId = $(this).attr('id');

            if (deleteLinkId == null || deleteLinkId == '') {
                return;
            }

            var rowNumber = deleteLinkId.replace('deleteRowLink_', '');
            $('#<%= _currentEditRow.ClientID %>').val(rowNumber);

            executeRowEditorPostback('<%=_deleteRowButton.ClientID %>', 'href');
        }
    }

    //
    function onMatrixRowReorder(event, ui) {
        var rowList = $('#matrixRowSortList');
        
        //Store new column order in hidden field to be saved when item updated
        var orderArray = rowList.sortable('toArray');
        var orderString = '';

        for (var i = 0; i < orderArray.length; i++) {
            if (orderArray[i] != null && orderArray[i] != '') {
                if (i > 0) {
                    orderString = orderString + ',';
                }
                orderString = orderString + orderArray[i].replace('matrixRow_', '');
            }
        }

        $('#<%=_newRowOrder.ClientID %>').val(orderString);

        //Update alternating styles
        rowList.children('.rowSelect').each(function (index, elem) {
            if (index % 2 == 1) {
                $(elem).attr('class', 'allMenu dashStatsContent');
            } else {
                $(elem).attr('class', 'allMenu dashStatsContent detailZebra');
            }
        });

        executeRowEditorPostback('<%=_updateRowsOrder.ClientID %>', 'href');
    }

    //Trim a string
    function trim(str) {
        return str.replace(/^\s+|\s+$/g, "");
    }

    //
    function onRowEdit(rowNumber, isHtmlOption) {
        debugger;
        //Move buttons & pipeing
        var updateButton = $('#<%=_updateRowButton.ClientID %>').detach();
        var cancelButton = $('#<%=_cancelRowButton.ClientID %>').detach();

        updateButton.appendTo('#rowButtons_' + rowNumber);
        //$('nbsp;').appendTo('#rowButtons_' + rowNumber);
        cancelButton.appendTo('#rowButtons_' + rowNumber);

        //Clear edit values
        $('#<%=_updatedRowType.ClientID %>').val('[EMPTY]');
        $('#<%=_updatedRowText.ClientID %>').val('[EMPTY]');
        $('#<%=_updatedRowAlias.ClientID %>').val('[EMPTY]');

        //Bind change events
        $('#rowTypeSelect_' + rowNumber).bind('change', onRowTypeChange);

        //Bind keypress
        $('#rowTextInput_' + rowNumber).bind('change', onRowTextChange);
        $('#rowTextInput_' + rowNumber).bind('keyup', onRowTextKeyPress);
        $('#rowAliasInput_' + rowNumber).bind('keyup', onRowAliasKeyPress);

        //Initialize editors
        var rowType = $('#rowType_' + rowNumber).text().toLowerCase();
        rowType = trim(rowType.replace(/"/g, ""));
        if (rowType == "header")
            rowType = "subheading";

        $('#rowAliasInput_' + rowNumber).val(trim($('#rowAlias_' + rowNumber).text()));
        $('#rowTypeSelect_' + rowNumber + ' option').removeAttr('selected');
        $('#rowTypeSelect_' + rowNumber + ' option[value="' + rowType + '"]').attr('selected', 'selected');

        //Hide/show elements
        $('#rowType_' + rowNumber).hide();
        $('#rowAlias_' + rowNumber).hide();
        $('#deleteRowLink_' + rowNumber).hide();
        $('#editHtmlRowLink_' + rowNumber).hide();

        $('#editRowType_' + rowNumber).show();
        $('#editRowAlias_' + rowNumber).show();
        $('#rowButtons_' + rowNumber).show();

        if(!isHtmlOption) {
            $('#rowTextInput_' + rowNumber).val(trim($('#rowText_' + rowNumber).text()));
            
            $('#rowText_' + rowNumber).hide();
            $('#editRowText_' + rowNumber).show();
            
            var pipe = $('#pipeContainer').detach();
            pipe.appendTo('#pipeItem_' + rowNumber);
            
            //Update pipe control
            <%= _pipeSelector.ID %>bindToInput ('rowTextInput_' + rowNumber);
        }
    }

    //
    function onRowEditCancel(rowNumber) {
        if (rowNumber == null || rowNumber == '') {
            return;
        }

        //Update pipe control
        <%=_pipeSelector.ID %>bindToInput ('<%=_newMatrixRowText.ClientID %>');

        var pipe = $('#pipeContainer').detach();
        pipe.appendTo('#newRowPipe');

        //Unbind change events
        $('#rowTypeSelect_' + rowNumber).unbind('change', onRowTypeChange);

        //unbind keypress
        $('#rowTextInput_' + rowNumber).unbind('change', onRowTextChange);
        $('#rowTextInput_' + rowNumber).unbind('keyup', onRowTextKeyPress);
        $('#rowAliasInput_' + rowNumber).unbind('keyup', onRowAliasKeyPress);

        //Show/hide elements
        $('#editRowType_' + rowNumber).hide();
        $('#editRowAlias_' + rowNumber).hide();
        $('#editRowText_' + rowNumber).hide();
        $('#rowButtons_' + rowNumber).hide();

        $('#rowType_' + rowNumber).show();
        $('#rowText_' + rowNumber).show();
        $('#rowAlias_' + rowNumber).show();
        $('#deleteRowLink_' + rowNumber).show();
        $('#editHtmlRowLink_' + rowNumber).show();
    }

    //
    function onRowTypeChange() {
        $('#<%=_updatedRowType.ClientID %>').val($(this).val());
    }

    //
    function onNewRowKeyPress(e) {
        if (e.which == 13) {
            executeRowEditorPostback('<%=_addRowLink.ClientID %>', 'oldhref');
        }
    }

   //
    function onRowTextKeyPress(e) {
        if (e.which == 13) {
            executeRowEditorPostback('<%=_updateRowButton.ClientID %>', 'oldhref');
        }
        else {
            var currentRow = $('#<%= _currentEditRow.ClientID %>').val();

            if (currentRow != null && currentRow != '') {
                $('#<%=_updatedRowText.ClientID %>').val($('#rowTextInput_' + currentRow).val());
            }
        }
    }

    //
    function onRowTextChange(e) {
        debugger;
        var currentRow = $('#<%= _currentEditRow.ClientID %>').val();

        if (currentRow != null && currentRow != '') {
            $('#<%=_updatedRowText.ClientID %>').val($('#rowTextInput_' + currentRow).val());
        }
    }

    function onRowAliasKeyPress(e) {
        if (e.which == 13) {
            executeRowEditorPostback('<%=_updateRowButton.ClientID %>', 'oldhref');
        } else {
            
            var currentRow = $('#<%= _currentEditRow.ClientID %>').val();

            if (currentRow != null && currentRow != '') {
                $('#<%=_updatedRowAlias.ClientID %>').val($('#rowAliasInput_' + currentRow).val());
            }
        }
    }

    function isHtml(text) {
        return text.indexOf('html-wrapper') > 0 || text.indexOf('<p>') == 0;
    }
    function isEncoded(text) {
        return text.indexOf("&lt;") >= 0 || text.indexOf("&gt") >= 0;
    }
</script>
    <div style="display:none;" id="textHelper"></div>

    <div style="display:none;">
        <asp:TextBox ID="_newRowOrder" runat="server" Text="" />
        <asp:TextBox ID="_currentEditRow" runat="server" Text="" />
        <asp:TextBox ID="_updatedRowType" runat="server" Text="[EMPTY]" />
        <asp:TextBox ID="_updatedRowText" runat="server" Text="[EMPTY]" />
        <asp:TextBox ID="_updatedRowAlias" runat="server" Text="[EMPTY]" />
        <btn:CheckboxButton ID="_updateRowButton" uframeignore="true" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton" runat="server" TextId="/pageText/editMatrixRows.aspx/update" />
        <asp:LinkButton ID="_deleteRowButton" runat="server" Text="Delete" />
        <btn:CheckboxButton ID="_cancelRowButton" uframeignore="true"  runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 redButton" TextId="/pageText/editMatrixRows.aspx/cancel" />
    </div>
   


<div style="clear:both;"></div>
    <%-- Headers --%>
    <div class="statsContentHeader">
        <div class="fixed_25 left">&nbsp;</div>
        <div class="fixed_50 left">Row</div>
        <div class="fixed_100 left" style="margin-right:4px;"><%= WebTextManager.GetText("/pageText/editMatrixRows.aspx/rowType")%></div>
        <div class="fixed_275 left">Text</div>
        <div class="fixed_125 left"><%= WebTextManager.GetText("/pageText/forms/surveys/itemEditors/optionsNormalEntry.ascx/alias") %> </div>
        <div class="fixed_175 left" style="border-style:none;">&nbsp;</div>
        <div class="clear"></div>
    </div>
    <div class="clear"></div>
    <div id="matrixRowSortList">
        <% 
            for (int rowNumber = 1; rowNumber <= MatrixItemTextDecorator.Data.RowCount; rowNumber++)
            {
                string rowType = GetRowType(rowNumber);
        %>

            <div rowNumber="<%=rowNumber %>" id="matrixRow_<%=rowNumber %>" class="rowSelect <%= GetRowStyle(rowNumber) %>">
                <div class="upDownArrow left fixed_25">
                    &nbsp;
                </div>
                <div class="fixed_50 left input">
                    <a href="javascript:void(0);" uframeignore="true" class="editRowLink" rowNumber="<%=rowNumber %>"><%=rowNumber %></a>
                </div>
                <div class="fixed_100 left input" style="margin-right:4px;">
                    <div id="rowType_<%=rowNumber %>"><%=rowType %></div>
                    <div id="editRowType_<%=rowNumber %>" style="display:none;">
                        <select uframeignore="true" id="rowTypeSelect_<%=rowNumber %>">
                            <%= GetRowTypeOptions(rowNumber) %>
                        </select>
                    </div>
                </div>
                <div class="fixed_275 left input">
                    <div id="rowText_<%=rowNumber %>" class="matrixRowText"><%=GetRowText(rowNumber) %>&nbsp;</div>
                    <div id="editRowText_<%=rowNumber %>" style="display:none;"><input type="text" id="rowTextInput_<%=rowNumber %>" style="width:250px;" /></div>
                </div>
                <div class="fixed_135 left input">
                    <div id="rowAlias_<%=rowNumber %>" class="matrixRowAlias"><%=GetRowAlias(rowNumber) %>&nbsp;</div>
                    <div id="editRowAlias_<%=rowNumber %>" style="display:none;"><input type="text" id="rowAliasInput_<%=rowNumber %>" style="width:120px;" /></div>
                </div>
                <div class="fixed_175 left input">
                    <div id="rowButtons_<%=rowNumber %>"></div>
                    <% if (rowType != RowTypeOther) { %>
                    <a href="javascript:void(0);" rowNumber="<%=rowNumber %>" class="editHtmlLink ckbxButton roundedCorners border999 shadow999 orangeButton OptionEditorButton" uframeignore="true" id="editHtmlRowLink_<%=rowNumber %>">
                        <span>HTML</span>
                    </a>
                    <% } %>
                    <a href="javascript:void(0);" class="deleteRowLink ckbxButton roundedCorners border999 shadow999 redButton OptionEditorButton" uframeignore="true" id="deleteRowLink_<%=rowNumber %>">
                        <span>-</span>
                    </a>
                </div>
                <div class="clear"></div>
                <div id="pipeItem_<%=rowNumber %>">
                    
                </div>
                <div class="clear"></div>
            </div>
            <div class="clear"></div>
        <%} %>
        <div class="clear"></div>
    </div>
    <div class="clear"></div>
    
     <div class="<%=GetRowStyle(MatrixItemTextDecorator.Data.RowCount + 1) %>" id="newRowItem">
        <div class="left fixed_25">
            &nbsp;
        </div>
        <div class="fixed_50 left input">
            <%= MatrixItemTextDecorator.Data.RowCount + 1 %>
        </div>
        <div class="fixed_100 left input" style="margin-right:4px;">
            <ckbx:MultiLanguageDropDownList ID="_newRowType" runat="server">
                <asp:ListItem TextId="/pageText/editMatrixRows.aspx/normal" Value="Normal" Text="Normal"></asp:ListItem>
                <asp:ListItem TextId="/pageText/editMatrixRows.aspx/other" Value="Other" Text="Other"></asp:ListItem>
                <asp:ListItem TextId="/pageText/editMatrixRows.aspx/header" Value="Subheading" Text="Header"></asp:ListItem>
            </ckbx:MultiLanguageDropDownList>
        </div>
        <div class="fixed_275 left input">
            <asp:TextBox ID="_newMatrixRowText" runat="server" style="width:250px;"></asp:TextBox>
        </div>
        <div class="fixed_135 left input">
            <asp:TextBox ID="_newMatrixRowAlias" runat="server" style="width:120px;"></asp:TextBox>
        </div>
        <div class="fixed_125 left input">
            <btn:CheckboxButton ID="_addRowLink" runat="server" Text="+"  uframeignore="true" CssClass="left ckbxButton roundedCorners border999 shadow999 orangeButton OptionEditorButton" />
            <btn:CheckboxButton ID="_htmlEditorLink" runat="server" Text="HTML" style="margin-left: 5px;" CssClass="left ckbxButton roundedCorners border999 shadow999 orangeButton OptionEditorButton" />
        </div>
        <div class="clear"></div>
        <div id="newRowPipe">
            <div style="margin-left:175px;margin-bottom:5px;" id="pipeContainer"><pipe:PipeSelector ID="_pipeSelector" runat="server" /></div>
        </div>
    </div>
    
    <div class="clear"></div>
    <asp:HiddenField ID="_currentrow" runat="server" />
    <asp:HiddenField ID="_currenthtml" runat="server" />
    <btn:CheckboxButton ID="_postRows" runat="server" style="display: none;" uframeignore="true" />
    <btn:CheckboxButton ID="_updateRowsOrder" runat="server" style="display: none;" uframeignore="true" />
    
<script type="text/C#" runat="server">

    //private const string WRAPPER = "<div class=\"html-wrapper\">";

    // private string HandleHtml(string html)
    //    {
    //        if (string.IsNullOrEmpty(html))
    //            return string.Empty;

    //        html = Utilities.AdvancedHtmlDecode(html);

    //        if (html.Contains(WRAPPER))
    //        {
    //            html = html.Replace(WRAPPER, string.Empty);
    //            int closingDivIndex = html.LastIndexOf("</div>");
    //            html = html.Remove(closingDivIndex);
    //        }

    //        return html;
    //    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="rowNumber"></param>
    /// <returns></returns>
    protected string GetRowStyle(int rowNumber)
    {
        if (rowNumber % 2 == 1)
        {
            return "dashStatsContent allMenu detailZebra";
        }

        return "dashStatsContent allMenu";
    }

    /// <summary>
    /// Get type name of matrix column
    /// </summary>
    /// <param name="rowNumber"></param>
    /// <returns></returns>
    protected string GetRowType(int rowNumber)
    {
        if(MatrixItemTextDecorator.Data.IsRowOther(rowNumber))
        {
            return RowTypeOther;
        }

        if (MatrixItemTextDecorator.Data.IsRowSubheading(rowNumber))
        {
            return RowTypeHeader;
        }

        return WebTextManager.GetText("/pageText/editMatrixRows.aspx/normal");
    }

    private string RowTypeOther
    {
        get { return WebTextManager.GetText("/pageText/editMatrixRows.aspx/other"); }
    }

    private string RowTypeHeader
    {
        get { return WebTextManager.GetText("/pageText/editMatrixRows.aspx/header"); }
    }

    /// <summary>
    /// Get type name of matrix column
    /// </summary>
    /// <param name="rowNumber"></param>
    /// <returns></returns>
    protected string GetRowTypeOptions(int rowNumber)
    {
        var optionStringBuilder = new StringBuilder();

        string rowType = MatrixItemTextDecorator.Data.IsRowOther(rowNumber)
                             ? "other"
                             : MatrixItemTextDecorator.Data.IsRowSubheading(rowNumber)
                                   ? "subheading"
                                   : "normal";

        optionStringBuilder.Append(
            string.Format(
                "<option {0} value=\"{1}\">{2}</option>",
                rowType == "normal" ? "selected" : string.Empty,
                "normal",
                WebTextManager.GetText("/pageText/editMatrixRows.aspx/normal")));

        optionStringBuilder.Append(
            string.Format(
                "<option {0} value=\"{1}\">{2}</option>",
                rowType == "subheading" ? "selected" : string.Empty,
                "subheading",
                WebTextManager.GetText("/pageText/editMatrixRows.aspx/header")));

        optionStringBuilder.Append(
            string.Format(
                "<option {0} value=\"{1}\">{2}</option>",
                rowType == "other" ? "selected" : string.Empty,
                "other",
                WebTextManager.GetText("/pageText/editMatrixRows.aspx/other")));

        return optionStringBuilder.ToString();
    }




</script>
    

