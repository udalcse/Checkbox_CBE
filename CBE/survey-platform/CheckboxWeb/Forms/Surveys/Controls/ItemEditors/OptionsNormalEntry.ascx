<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="OptionsNormalEntry.ascx.cs"  Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.OptionsNormalEntry" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register src="~/Controls/Piping/PipeControl.ascx" tagname="PipeSelector" tagprefix="pipe" %>

<script type="text/javascript">
    var options = new Array();
    var hasOptionPoints = <%=HasPoints?"true":"false"%>;
    var areColumnsCategorized = <%=AreColumnsCategorized? "true" : "false" %>;
    var currentEditOptionId = null;

    $(document).ready(function () {
        $('#<%=_newOptionBtn.ClientID%>').attr('href', 'javascript:void(0);');
        $('#<%=_htmlEditorBtn.ClientID%>').attr('href', 'javascript:void(0);');
        $('#<%=_updateOptionButton.ClientID %>').attr('href', 'javascript:void(0);');
        $('#<%=_cancelOptionButton.ClientID %>').attr('href', 'javascript:void(0);');

        /*$('#newOptionTxt').attr('title', '<%=WebTextManager.GetText("/pageText/forms/surveys/itemEditors/optionsNormalEntry.ascx/newChoice")%>');
        $('#newOptionAlias').attr('title', '<%=WebTextManager.GetText("/pageText/forms/surveys/itemEditors/optionsNormalEntry.ascx/enterAlias")%>');
        $('#newOptionCategory').attr('title', '<%=WebTextManager.GetText("/pageText/forms/surveys/itemEditors/optionsNormalEntry.ascx/enterCategory")%>');
        $('#newOptionPoints').attr('title', '<%=WebTextManager.GetText("/pageText/forms/surveys/itemEditors/optionsNormalEntry.ascx/enterPoints")%>');
        
        $('#newOptionTxt').watermark();
        $('#newOptionAlias').watermark();
        $('#newOptionCategory').watermark();
        $('#newOptionPoints').watermark();*/

         //Sort
        $('#existingOptions').sortable({
            axis: 'y',
            update: onOptionReorder
        });

        <%foreach(var listOption in Options.Where(opt => !opt.IsOther && !opt.IsNoneOfAbove).OrderBy(opt => opt.Position)){ %>
            addOption(
                <%=listOption.OptionID %>,
                <%=listOption.IsDefault.ToString().ToLower() %>,
                '<%=GetOptionText(listOption.OptionID) %>',
                '<%=listOption.Alias.Replace("'", "\\\'")%>',
                '<%=listOption.Category.Replace("'", "\\\'") %>',
                <%=listOption.Points %>
            );
        <%} %>

        //This method (renderOptions) will be executed later (See the end of the control). It's needed because it uses
        //scripts from pipeControl and UFrameManager evaluates this script($(document).ready();) before pipeControl's script. 
        //That is why this method is commented here. It is written at the end of the control.
        //renderOptions(options);

        $(document).off('click', ".optionDefaultInput");
        $(document).on('click', ".optionDefaultInput", function(e){
            e.stopPropagation();
        });
        
        $(document).off('click', ".optionText");
        $(document).off('click', ".optionAlias");
        $(document).off('click', ".optionPoints");
        
        $(document).on('click', ".optionText", onOptionTextClick);
        $(document).on('click', ".optionAlias", onOptionAliasClick);
        $(document).on('click', ".optionPoints", onOptionPointsClick);

        $(document).off('keydown', '#newOptionTxt');
        $(document).on('keydown', '#newOptionTxt', function(e){
            if(e.which == 13){
                $('#<%=_newOptionBtn.ClientID%>').click();
            }
        });

        $(document).off('keydown', '#newOptionAlias');
        $(document).on('keydown', '#newOptionAlias', function(e){
            if(e.which == 13){
                $('#<%=_newOptionBtn.ClientID%>').click();
            }
        });

        if (hasOptionPoints)
        {
            $(document).off('keydown', '#newOptionPoints');
            $(document).on('keydown', '#newOptionPoints', function(e){
                if(e.which == 13){
                    $('#<%=_newOptionBtn.ClientID%>').click();
                }
            });
        }

        $(document).off('click', '#<%=_newOptionBtn.ClientID%>');
        $(document).on('click', '#<%=_newOptionBtn.ClientID%>', function(e) {
            var isDefault=$('#<%=GetNewRowDefaultInputName() %>').is(":checked");
            
            var optionTxt = stripScripts($('#newOptionTxt').val());
            var optionAlias = stripScripts($('#newOptionAlias').val());
            optionAlias = htmlEncode(optionAlias);

            var optionCategory = $("#newOptionCategory").val();
            var optionPoints = $('#newOptionPoints').val();
            
            //Update current options
            UpdateOptions();

            if(isNaN(optionPoints)){
                optionPoints = '';
            }

            addOption(
                -1 * (options.length + 1),
                isDefault,
                optionTxt,
                optionAlias,
                optionCategory,
                optionPoints
            );

            $('#newOptionTxt').val('');
            $('#newOptionPoints').val('');
            $('#newOptionAlias').val('');            
            $('#newOptionCategory').val('');
            $('#<%=GetNewRowDefaultInputName() %>').removeAttr('checked');
            $.uniform.update('#<%=GetNewRowDefaultInputName() %>');

            renderOptions(options);

            $('#newOptionTxt').focus();
            
            e.preventDefault();
            return;
        });
        
        $(document).off('click', '#<%=_htmlEditorBtn.ClientID%>');
        $(document).on('click', '#<%=_htmlEditorBtn.ClientID%>', function(e) {
            $('#<%=_newOptionBtn.ClientID%>').click();
            $('#existingOptions .optionSelect').last().find('.editHtmlDiv').click();
        });

        $('#<%=_updateOptionButton.ClientID %>').click(function(e){            
            UpdateOptions();  
            currentEditOptionId = null;              

            movePipeControlToNewOption();
            renderOptions(options);
            
            e.preventDefault();
            e.stopPropagation();
            return;        
        });

        $('#<%=_cancelOptionButton.ClientID %>').click(function(e){        
            onOptionEditCancel(currentEditOptionId);
            e.preventDefault();
            e.stopPropagation();
            return;        
        });
        
        <% if(RestrictHtmlOptions) { %>
            $('#<%= _newOptionBtn.ClientID%>').css('margin-left', '5px');
        <% } %>
    });

    function onOptionTextClick() {
        var newEditOptionId = $(this).attr('id').replace('optionText_','');
        onOptionClick("text", newEditOptionId);	    
    }
    
    function onOptionAliasClick() {
        var newEditOptionId = $(this).attr('id').replace('optionAlias_','');
        onOptionClick("alias", newEditOptionId);
    }
    
    function onOptionPointsClick() {
        var newEditOptionId = $(this).attr('id').replace('optionPoints_','');
        onOptionClick("points", newEditOptionId);
    }

    //
    function onOptionClick(target, newEditOptionId){
        var isHtmlOption = isHtml($('#optionText_' + newEditOptionId).html());

        if(target == "text" && isHtmlOption) {
            $('#editHtmlRowLink_' + newEditOptionId).click();
            return;
        }

        if (currentEditOptionId == newEditOptionId){
            return;
        }

        if (currentEditOptionId != null && currentEditOptionId != ''){
            onOptionEditCancel(currentEditOptionId);
        }

        currentEditOptionId = newEditOptionId;

        onOptionEdit(newEditOptionId, isHtmlOption);
    }

    function onOptionEdit(optionId, isHtmlOption){
        //Move buttons & piping.
        var updateButton = $('#<%=_updateOptionButton.ClientID %>').detach();
        var cancelButton = $('#<%=_cancelOptionButton.ClientID %>').detach();

        updateButton.appendTo('#editOptionButtons_'+ optionId);
        //$('nbsp;').appendTo('#editOptionButtons_'+ optionId);
        cancelButton.appendTo('#editOptionButtons_'+ optionId);

        //Bind keypress       
        $('#optionTextInput_' + optionId).bind('keyup', onOptionTextKeyPress);
        $('#optionAliasInput_' + optionId).bind('keyup', onOptionAliasKeyPress);
        $('#optionCategoryInput_' + optionId).bind('keyup', onOptionCategoryKeyPress);
        $('#optionPointsInput_' + optionId).bind('keyup', onOptionPointsKeyPress);
        
        //Initialize editors
        $('#optionAliasInput_' + optionId).val(trim($('#optionAlias_' + optionId).text()));
        $('#optionCategoryInput_' + optionId).val(trim($('#optionCategory_' + optionId).text()));
        $('#optionPointsInput_' + optionId).val(trim($('#optionPoints_' + optionId).text()));

        //Hide/show elements
        $('#optionAlias_' + optionId).hide();
        $('#removeOptionButton_' + optionId).hide();                        
        $('#editHtmlRowLink_' + optionId).hide();                

        $('#editOptionAlias_' + optionId).show();                

        if (areColumnsCategorized){
            $('#optionCategory_' + optionId).hide();
            $('#editOptionCategory_' + optionId).show();
        }

        if (hasOptionPoints){
            $('#optionPoints_' + optionId).hide();
            $('#editOptionPoints_' + optionId).show();
        }
        
        if(!isHtmlOption) {
            $('#optionTextInput_' + optionId).val(trim($('#optionText_' + optionId).text()));
            
            $('#optionText_' + optionId).hide();
            $('#editOptionText_' + optionId).show();
            
            var pipeControl = $('#pipeContainer').detach();
            pipeControl.appendTo('#pipeItem_' + optionId);

            //Update pipe control
            <%= _pipeSelector.ID %>bindToInput ('optionTextInput_' + optionId);
        }
    }

    //
    function onOptionTextKeyPress(e){
        if (e.which == 13){
            UpdateOptions();
        }
    }

    //
    function onOptionAliasKeyPress(e){
        if (e.which == 13){
            UpdateOptions();          
        }
    }

    //
    function onOptionCategoryKeyPress(e){
        if (e.which == 13){
            UpdateOptions();           
        }
    }

    //
    function onOptionPointsKeyPress(e){
        if (e.which == 13){
            UpdateOptions();            
        }
    }

    //
    function onOptionEditCancel(optionId){
        if (optionId == null || optionId == ''){
            return;
        }

        currentEditOptionId = null;

        //Unbind events
        $('#optionTextInput_' + optionId).unbind('keyup', onOptionTextKeyPress);
        $('#optionAliasInput_' + optionId).unbind('keyup', onOptionAliasKeyPress);
        $('#optionCategoryInput_' + optionId).unbind('keyup', onOptionCategoryKeyPress);
        $('#optionPointsInput_' + optionId).unbind('keyup', onOptionPointsKeyPress);

        $('#optionTextInput_' + optionId).val($("#optionText_" + optionId).text());
        $('#optionAliasInput_' + optionId).val($("#optionAlias_" + optionId).text());
        $('#optionCategoryInput_' + optionId).val($("#optionCategory_" + optionId).text());
        $('#optionPointsInput_' + optionId).val($("#optionPoints_" + optionId).text());

        //Hide/show elements
        $('#editOptionText_' + optionId).hide();
        $('#editOptionAlias_' + optionId).hide();
        
        movePipeControlToNewOption();

        $('#optionText_' + optionId).show();
        $('#optionAlias_' + optionId).show();    
        $('#removeOptionButton_' + optionId).show();                              
        $('#editHtmlRowLink_' + optionId).show();                

        if (areColumnsCategorized){
            $('#editOptionCategory_' + optionId).hide();
            $('#optionCategory_' + optionId).show();          
        }

        if (hasOptionPoints){
            $('#editOptionPoints_' + optionId).hide();
            $('#optionPoints_' + optionId).show();        
        }

        moveEditingButtonsToHiddenControls();
    }

    //Trim a string
    function trim(str) {
        return str.replace(/^\s+|\s+$/g, "");
    }

    //Update current options
    function UpdateOptions(){
        for(var i=0;i<options.length;i++){
            var optionId = options[i].optionId;

            options[i].isDefault = $("#optionDefaultInput_" + optionId).is(":checked");
            var text = $("#optionTextInput_" + optionId).val();
            if (isHtml(text) && text.indexOf("&lt;") == 0) {
                text = htmlDecode(text);
            }
            options[i].text = stripScripts(text);

            options[i].alias = htmlEncode(stripScripts($("#optionAliasInput_" + optionId).val()));
            
            if (areColumnsCategorized){
                options[i].category = $("#optionCategoryInput_" + optionId).val();
            }

            var points = $("#optionPointsInput_" + optionId).val();
            
            if (isNaN(points)){
                points = options[i].points;
            }

            options[i].points = points;
        }
    }

    //
    //
    function addOption(optionId, isDefault, text, alias, category, points) {
        options.push({ optionId: optionId, position: options.length + 1, text: text, alias: alias, category: category, points: points, isDefault: isDefault });
        sortOptions();
    }

    //
    function removeOption(optionPosition){
        currentEditOptionId = null;
        var indexToRemove = -1;
        for(var i = 0; i < options.length; i++){
            if(options[i].position == optionPosition){
                indexToRemove = i;
                break;
            }
        }

        if(indexToRemove < 0)
            return;
        
        options.splice(indexToRemove, 1);

        //Sort
        sortOptions();

        //Update positions
        for(var i = 0; i < options.length; i++){
            options[i].position = i + 1;
        }

        UpdateOptions();

        renderOptions(options);
    }

    //
    function moveOption(initialPosition, newPosition) {
        //Find option
        var optionIndex = -1;
        for (var i = 0; i < options.length; i++) {
            if (options[i].position == initialPosition) {
                optionIndex = i;
                break;
            }
        }

        if (optionIndex < 0) {
            return;
        }

        //Remove option
        var optionToMove = options.splice(optionIndex, 1)[0];
        optionToMove.position = newPosition;

        //increment positions of all options to come after new option position
        for (var i = 0; i < options.length; i++) {
            if (options[i].position >= newPosition) {
                options[i].position++;
            }
        }

        //re-add moved option
        options.push(optionToMove);

        //sort
        sortOptions();
    }

     //
    function onOptionReorder(event, ui) {
        //Store new column order in hidden field to be saved when item updated
        var orderArray = $('#existingOptions').sortable('toArray');

        var orderString = '';

        for (var i = 0; i < orderArray.length; i++) {
            if (orderArray[i] != null && orderArray[i] != '') {
                if (orderString.length > 0) {
                    orderString = orderString + ',';
                }

                orderString = orderString + orderArray[i].replace('option_', '');
            }
        }
        $('#normalEntryOptionOrder').val(orderString);

        //Set new option position.
        for(var i = 0; i < options.length; i++) {
            
            var tempStr = orderString.substr(0, orderString.search(options[i].optionId));
            options[i].position = CountSubstrInStr(tempStr, ",") + 1;            
        }

        //Sort options
        options.sort(compareOptions);
        
        //Update option position for remove.
        for(var i = 0; i < options.length; i++) {
            $('#removeOptionButton_' + options[i].optionId).parent().attr('optionposition', (i + 1) );
        }

        //Update alternating styles
        $('#existingOptions').children('div').each(function(index){
            if(index % 2 == 1){
                $(this).attr('class', 'dashStatsContent allMenu detailZebra');
            } else{
               $(this).attr('class', 'dashStatsContent allMenu');
            }
        });
    }

    //Count of substr in str.
    function CountSubstrInStr(str, substr){
        return (str.length - str.replace(new RegExp(substr,"g"),'').length) / substr.length;
    }

    //
    function sortOptions(){
        //Sort options
        options.sort(compareOptions);

        //Update option order hidden field
        var orderString = '';

        for(var i = 0; i < options.length; i++){
            if(i > 0){
                orderString += ',';
            }
            orderString += options[i].optionId;
        }

        $('#normalEntryOptionOrder').val(orderString);
    }

    //
    function compareOptions(a, b) {
        if (a.position < b.position) {
            return -1;
        }
        else if (a.position > b.position) {
            return 1;
        }
        else {
            return 0;
        }
    }

    //
    function moveEditingButtonsToHiddenControls(){
        //Replace the buttons
        var updateButton = $('#<%=_updateOptionButton.ClientID %>').detach();
        var cancelButton = $('#<%=_cancelOptionButton.ClientID %>').detach();
        
        updateButton.appendTo('#hiddenControls');
        cancelButton.appendTo('#hiddenControls');
    }

    //
    function movePipeControlToNewOption(){
        //Replace pipeControl
        var pipeControl = $('#pipeContainer').detach();
        pipeControl.appendTo('#newOptionPipe');
        
        <%=_pipeSelector.ID %>bindToInput ('newOptionTxt');
    }

    function optionHtmlEncode(value){
        return $('<div/>').text(value).html().split("\"").join("&quot;");
    }
    
    //
    function onEditHtmlClick(e) {
        e.stopPropagation();
        
        var currentRow = $(this).attr('optionId');
        var html = $('#optionText_' + currentRow).html().replace(/#/g, '%23').replace(/&/g, '%26');

        <% if(IsNew || MatrixColumnItem) { %>
            $('#<%= _currentrow.ClientID %>').val(currentRow);
            $('#<%= _currenthtml.ClientID %>').val(html);

            window.location = $('#<%= _postOptions.ClientID %>').attr('href');
            return;
        <% } %>

        var params = new Array({name: 'html', value: html}, {name: 'row', value: currentRow}, {name: 'callback', value: 'onHtmlDialogClosed'});

        templateEditor.openChildWindow(<%=ItemId %>, <%=PagePosition %>, 'HtmlEditor.aspx', params);
    }
    /*
    function addParamsToUrl(url, params) {
        if (url == null) {
            url = '';
        }

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
    }
    */
    //
    function onHtmlDialogClosed(args) {
        $('#optionText_' + args.rowNumber).html(htmlDecode(args.html));
        $('#optionTextInput_' + args.rowNumber).val(args.html);
    }

    //Render options
    function renderOptions(options){       
        moveEditingButtonsToHiddenControls();  

        //Clear existing
        $('#existingOptions').empty();

        //Add new
        var optionListTemplate = '<div id="option_[OPTIONID]" class="dashStatsContent [ROWCLASS] optionSelect"></div>';
        var optionListDefaultTemplate = '<div class="upDownArrow left fixed_50" style="text-align:center;"></div>';
        var optionTextTemplate = '<div class="left fixed_200"><div class="optionText" id="optionText_[OPTIONID]">[OPTIONTEXT]</div><div id="editOptionText_[OPTIONID]" style="display:none;"><input type="text" name="optionTextInput_[OPTIONID]" id="optionTextInput_[OPTIONID]" value="[OPTIONTEXT]" /></div></div>';
        var optionAliasTemplate = '<div class="left fixed_200"><div class="optionAlias" id="optionAlias_[OPTIONID]">[OPTIONALIAS]</div><div id="editOptionAlias_[OPTIONID]" style="display:none;"><input type="text" name="optionAliasInput_[OPTIONID]" id="optionAliasInput_[OPTIONID]" value="[OPTIONALIAS]" /></div></div>';
        var optionCategoryTemplate = '<div class="left fixed_200"><div id="optionCategory_[OPTIONID]">[OPTIONCATEGORY]</div><div id="editOptionCategory_[OPTIONID]" style="display:none;"><input type="text" name="optionCategoryInput_[OPTIONID]" id="optionCategoryInput_[OPTIONID]" value="[OPTIONCATEGORY]" /></div></div>';
        var optionPointsTemplate = '<div class="left fixed_50"><div class="optionPoints" id="optionPoints_[OPTIONID]">[OPTIONPOINTS]</div><div id="editOptionPoints_[OPTIONID]" style="display:none;"><input type="text" name="optionPointsInput_[OPTIONID]" id="optionPointsInput_[OPTIONID]" value="[OPTIONPOINTS]" style="width:50px;" maxlength="10" /></div></div>';        
        var optionHtmlTemplate = '<div class="left editHtmlDiv" optionId="[OPTIONID]" optionPosition="[POSITION]"><a class="editHtmlLink ckbxButton roundedCorners border999 shadow999 orangeButton OptionEditorButton" uframeignore="true" id="editHtmlRowLink_[OPTIONID]">HTML</a></div>';
        var removeTemplate = '<div class="left" style="margin-left:5px;" optionPosition="[POSITION]"><a class="ckbxButton roundedCorners border999 shadow999 redButton OptionEditorButton" uframeignore="true" id="removeOptionButton_[OPTIONID]">-</a><div id="editOptionButtons_[OPTIONID]" style="margin-left:5px;"></div></div><br class="clear" />';
        var pipeTemplate = '<div id="pipeItem_[OPTIONID]"></div>';

        for(var i = 0; i < options.length; i++){
            var rowClass = '';

            if(i % 2 == 1){
                rowClass = 'detailZebra';
            }
            
            var optionList = $(
                optionListTemplate
                    .replace('[OPTIONID]', options[i].optionId)
                    .replace('[POSITION]', options[i].position)
                    .replace('[ROWCLASS]', rowClass)
            );

            var defaultInputName = '<%=GetRowDefaultGroupName() %>';

            if(defaultInputName == ''){
                defaultInputName = 'defaultSelect_' + options[i].optionId;
            }

            var defaultOption = $(document.createElement("input"))
                .attr("name",defaultInputName)
                .attr("id",'optionDefaultInput_'+options[i].optionId)
                .attr("type",'<%=GetRowDefaultInputType() %>')
                .attr("class",'optionDefaultInput')
                .val(options[i].optionId);

            if(options[i].isDefault){
                defaultOption.attr('checked','checked');
            }

            var optionDefault = $(optionListDefaultTemplate).append(defaultOption);

            var optionText = $(
                optionTextTemplate
                    .replace('[POSITION]', options[i].position)
                    .replace(/\[OPTIONID\]/g, options[i].optionId)
                    .replace(/\[OPTIONTEXT\]/g, (isHtml(options[i].text)? options[i].text : optionHtmlEncode(options[i].text)))                 
            );

            var optionAlias = $(
                optionAliasTemplate
                    .replace('[POSITION]', options[i].position)
                    .replace(/\[OPTIONID\]/g, options[i].optionId)
                    .replace(/\[OPTIONALIAS\]/g, (isEncoded(options[i].alias)? options[i].alias : optionHtmlEncode(options[i].alias)))
            );

            var optionCategory = $(
                optionCategoryTemplate
                    .replace(/\[OPTIONID\]/g, options[i].optionId)
                    .replace(/\[OPTIONCATEGORY\]/g, options[i].category)
            );

            var optionPoints = $(
                optionPointsTemplate
                    .replace('[POSITION]', options[i].position)
                    .replace(/\[OPTIONID\]/g, options[i].optionId)
                    .replace(/\[OPTIONPOINTS\]/g, options[i].points)
            );

            var edit = $(
                optionHtmlTemplate
                    .replace('[POSITION]', options[i].position)
                    .replace(/\[OPTIONID\]/g, options[i].optionId)
            );
            edit.bind('click', onEditHtmlClick);

            var remove = $(
                removeTemplate
                    .replace('[POSITION]', options[i].position)
                    .replace(/\[OPTIONID\]/g, options[i].optionId)
            );
            
            remove.bind('click', function(e){
                var position = $(this).attr('optionPosition');

                if(position == null || position == '' || isNaN(position)){
                    return;
                }

                removeOption(parseInt(position, 10));
                e.stopPropagation();
            });

            var piping = $(
                pipeTemplate
                    .replace(/\[OPTIONID\]/g, options[i].optionId)
            );

            //decode for html options
            if (isHtml(options[i].text)) {
                var input = optionText.find('input');
                var div = optionText.find('.optionText');
                
                optionText.empty();
                
                optionText.append(div);
                optionText.append(input);
                
                input.val(options[i].text);
                input.css('display', 'none');
            }

            optionList.append(optionDefault);
            optionList.append(optionText);
            optionList.append(optionAlias);

            if (areColumnsCategorized){
                optionList.append(optionCategory);
            }

            if (hasOptionPoints){
                optionList.append(optionPoints);
            }
            
            <% if(!RestrictHtmlOptions) { %>
                optionList.append(edit);
            <% } %>
            optionList.append(remove);
            optionList.append(piping);
            
            $('#existingOptions').append(optionList);
                        
        }

        if(options.length % 2 == 1){
            $('#newOptionContainer').addClass('detailZebra');
        }
        else{
           $('#newOptionContainer').removeClass('detailZebra');
        }
        
        movePipeControlToNewOption();  
    }

    function isHtml(text) {
        return text.indexOf('html-wrapper') > 0 || text.indexOf('<p>') == 0;
    }
    function isEncoded(text) {
        return text.indexOf("&lt;") >= 0 || text.indexOf("&gt") >= 0;
    }
</script>

<%-- Hidden input to store option order  --%>
<div id="hiddenControls" style="display:none;">
    <asp:HiddenField ID="_currentrow" runat="server" />
    <asp:HiddenField ID="_currenthtml" runat="server" />
    <btn:CheckboxButton ID="_postOptions" runat="server" style="display: none;" uframeignore="true" />
    <input type="text" id="normalEntryOptionOrder" name="normalEntryOptionOrder" />
    <btn:CheckboxButton ID="_updateOptionButton" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton smallButton OptionEditorButton" runat="server" TextId="/pageText/forms/surveys/itemEditors/optionsNormalEntry.ascx/Update" uframeignore="true" />
    <btn:CheckboxButton ID="_cancelOptionButton" CssClass="ckbxButton roundedCorners border999 shadow999 redButton smallButton OptionEditorButton" runat="server" TextId="/pageText/forms/surveys/itemEditors/optionsNormalEntry.ascx/Cancel" uframeignore="true" />
</div>

<%-- Existing Options --%>
<div class="statsContentHeader">
    <div class="fixed_50 left"><%=WebTextManager.GetText("/pageText/forms/surveys/itemEditors/optionsNormalEntry.ascx/default") %></div>
    <div class="fixed_200 left"><%=WebTextManager.GetText("/pageText/forms/surveys/itemEditors/optionsNormalEntry.ascx/text") %></div>
    <div class="fixed_200 left"><%=WebTextManager.GetText("/pageText/forms/surveys/itemEditors/optionsNormalEntry.ascx/alias") %></div>
<% if (AreColumnsCategorized)
    {%>    
    <div class="fixed_100 left"><%=WebTextManager.GetText("/pageText/forms/surveys/itemEditors/optionsNormalEntry.ascx/category") %></div>
<%
    }%>
<% if (HasPoints)
    { %>
    <div class="fixed_50 left"><%=WebTextManager.GetText("/pageText/forms/surveys/itemEditors/optionsNormalEntry.ascx/points") %></div>
<%} %>
    <br class="clear" />
</div>

<div id="existingOptions"></div>

<%-- New Option --%>
<div class="dashStatsContent allMenu" id="newOptionContainer">
    <div class="left fixed_50" style="text-align:center;"><input type="<%=GetRowDefaultInputType() %>" name="<%=GetNewRowDefaultInputName()%>" id="<%=GetNewRowDefaultInputName()%>" /></div>
    <div class="left fixed_200"><input type="text" id="newOptionTxt" style="width:185px;" /></div>
    <div class="left fixed_200"><input type="text" id="newOptionAlias" style="width:185px;" /></div>
<% if (AreColumnsCategorized)
    {%>
    <div class="left fixed_100"><input type="text" id="newOptionCategory" style="width:85px;" /></div>
<%
    }%>
    
<% if (HasPoints) { %>
    <div class="fixed_75 left"><input type="text" id="newOptionPoints" style="width:65px;" maxlength="5" /></div>
<%} %>
    <div class="left"><btn:CheckboxButton ID="_newOptionBtn" runat="server" Text="+" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton OptionEditorButton" uframeignore="true" /></div>
    <div class="left"><btn:CheckboxButton ID="_htmlEditorBtn" style="margin-left: 5px;" runat="server" Text="HTML" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton OptionEditorButton" uframeignore="true" /></div>
    <br class="clear" />
    
    <div class="left" id="newOptionPipe">
        <div id="pipeContainer">
            <div class="left fixed_50">&nbsp;</div>
            <div class="left"><pipe:PipeSelector ID="_pipeSelector" runat="server" /></div>
            <br class="clear" />
        </div>
    </div>
    <br class="clear" />
</div>

<script type="text/C#" runat="server">
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected string GetRowDefaultInputType()
    {
        return AllowMultiDefaultSelect ? "checkbox" : "radio";
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected string GetRowDefaultGroupName()
    {
        return !AllowMultiDefaultSelect ? "defaultSelect" : string.Empty;
    }

     /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected string GetNewRowDefaultInputName()
    {
        return !AllowMultiDefaultSelect ? "defaultSelect" : "defaultSelect_new";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="optionId"></param>
    /// <returns></returns>
    protected string GetOptionText(int optionId)
    {
        string option = OptionTexts.ContainsKey(optionId) ?
            OptionTexts[optionId]
                .Replace("\\", "\\\\")
                .Replace("'", "\\'")
                .Replace("\n", string.Empty)
                .Replace(Environment.NewLine, string.Empty)
            : string.Empty;

        if (option.StartsWith(Utilities.AdvancedHtmlEncode("<p>")) && option.EndsWith(Utilities.AdvancedHtmlEncode("</p>")))
            option = Utilities.AdvancedHtmlDecode(option);
        
        return option;
    }

</script>


<script type="text/javascript">
    $(document).ready(function () {
        //This method is replaced from the begining of the control due to UFrame inline script execution issue.
        //See more detailed description at the begining of the control.
        renderOptions(options);
    });
</script>