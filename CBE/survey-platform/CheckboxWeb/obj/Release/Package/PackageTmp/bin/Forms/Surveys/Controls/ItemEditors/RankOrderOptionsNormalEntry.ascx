<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="RankOrderOptionsNormalEntry.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.RankOrderOptionsNormalEntry" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Web" %>

<script type="text/javascript">
    var options = new Array();
    var currentEditOptionId = null;
    var ignoreNewOptionAlias = false;
    var aliasDefaultText = '<%= WebTextManager.GetText("/pageText/forms/surveys/itemEditors/rankOrderOptionsNormalEntry.ascx/enterAlias")%>';

    //show specific error
    function showOptionsError(className) {
        if ($('.validation-input-error.' + className).is(":hidden")) {
            $('.validation-input-error.' + className).slideDown(300).delay(2000).slideUp(600);
        }
    }
    $(document).ready(function () {
        $('#<%=_newOptionBtn.ClientID%>').attr('href', 'javascript:void(0);');
        $('#<%=_updateOptionButton.ClientID %>').attr('href', 'javascript:void(0);');
        $('#<%=_cancelOptionButton.ClientID %>').attr('href', 'javascript:void(0);');
        $('#<%=_htmlEditorBtn.ClientID%>').attr('href', 'javascript:void(0);');

         //Sort
        $('#existingOptions').sortable({
            axis: 'y',
            update: onOptionReorder
        });

        <%foreach(var listOption in Options.Where(opt => !opt.IsOther)){ %>
            addOption(
                <%=listOption.OptionID %>,
                <%=listOption.IsDefault.ToString().ToLower() %>,
                '<%=GetOptionText(listOption.OptionID) %>',
                '<%=listOption.Alias.Replace("'", "\\\'")%>',
                <%=listOption.ContentID.HasValue ? listOption.ContentID.Value.ToString() : "''" %>
            );
        <%} %>
        
        renderOptions(options);
        
        $(document).on('click', ".optionDefaultInput", function(e){
            e.stopPropagation();
        });
        
//        $(".optionSelect").live('mouseover', function() { $(this).addClass('hover'); });
//        $(".optionSelect").live('mouseout', function() { $(this).removeClass('hover'); });

        $(document).off('click', '.optionText');
        $(document).off('click', '.optionAlias');
        $(document).off('keydown', '#newOptionTxt');
        $(document).off('keydown', '#newOptionAlias');
        
        $(document).on('click', '.optionText', onOptionTextClick);
        $(document).on('click', '.optionAlias', onOptionAliasClick);

        $(document).on('keydown', '#newOptionTxt', function(e){
            if(e.which == 13){
                $('#<%=_newOptionBtn.ClientID%>').click();
            }
        });

        $(document).on('keydown', '#newOptionAlias', function(e){
            if(e.which == 13){
                $('#<%=_newOptionBtn.ClientID%>').click();
            }
        });

        $(document).on('focus', '#newOptionTxt', function(){
            if($('#newOptionTxt').val() == '<%=WebTextManager.GetText("/pageText/forms/surveys/itemEditors/rankOrderOptionsNormalEntry.ascx/newChoice")%>'
            || $('#newOptionTxt').val() == '<%=WebTextManager.GetText("/pageText/forms/surveys/itemEditors/rankOrderOptionsNormalEntry.ascx/enterLabel")%>'){
                $('#newOptionTxt').val('');
            }
        });

        $(document).on('focus', '#newOptionAlias', function(){
            if($('#newOptionAlias').val() == aliasDefaultText){
                $('#newOptionAlias').val('');
            }
        });

        $('#<%=_uploadImageBtn.ClientID %>').attr('href', '#');
        //Upload Image click
        $('#<%=_uploadImageBtn.ClientID %>').click(function (e) {  
            e.stopPropagation();
            if (typeof(templateEditor) == 'undefined')
            {
                showDialog('<%=ResolveUrl("~/") %>Forms/Surveys/UploadImage.aspx', 550, 650);
            }
            else
            {
                templateEditor.openChildWindow(0, <%=PagePosition %>, 'UploadImage.aspx', null, 'wizard');
            }
        });

        $(document).off('click', '#<%=_newOptionBtn.ClientID%>');
        $(document).on('click', '#<%=_newOptionBtn.ClientID%>', function(e) {
            var isDefault=$('#<%=GetNewRowDefaultInputName() %>').is(":checked");

            var optionImageId = getNewOptionImageId();

            var optionTxt = stripScripts($('#newOptionTxt').val());
            var optionAlias = stripScripts($('#newOptionAlias').val());
            optionAlias = htmlEncode(optionAlias);
            
            //Update current options
            UpdateOptions();
            if(<%=(!AreOptionsWithImage).ToString().ToLower() %> && (optionTxt == '' 
                || optionTxt == '<%=WebTextManager.GetText("/pageText/forms/surveys/itemEditors/rankOrderOptionsNormalEntry.ascx/newChoice")%>')){
                $('#selectEditorTabContainer').animate({scrollTop: $('#selectEditorTabContainer').height()}, 'slow');
                //return;
                $('#newOptionTxt').val('');
                optionTxt = '';
            }

            if (<%= AreOptionsWithImage.ToString().ToLower() %> && (optionTxt == '<%=WebTextManager.GetText("/pageText/forms/surveys/itemEditors/rankOrderOptionsNormalEntry.ascx/enterLabel")%>'))
                optionTxt = '';

            <%if (AreOptionsWithImage)
              {%>              
                if (optionImageId == ''){
                    showOptionsError('image');
                    return;
                }
                if(optionAlias == '' || optionAlias == aliasDefaultText){
                    if (ignoreNewOptionAlias)
                    {
                        optionAlias = '';
                    }
                    else
                    {
                        showOptionsError('alias');
                        return;
                    }
                }
            <%
              } else
              {%>
                if(optionAlias == aliasDefaultText){
                    optionAlias = '';
                }
              <%
              }%>

            addOption(
                -1 * (options.length + 1),
                isDefault,
                optionTxt,
                optionAlias,
                optionImageId
            );

            $('#newOptionTxt').val('');
            $('#newOptionAlias').val('');    
            $('#newOptionImageId').val('');
            $('#<%= _uploadImageBtn.ClientID %>').show();
            $('#_newUploadedImage').hide();
            $('#<%=GetNewRowDefaultInputName() %>').removeAttr('checked');
            renderOptions(options);

            $('#newOptionTxt').focus();

            e.preventDefault();
            return;
        });

        $(document).off('click', '#<%=_htmlEditorBtn.ClientID%>');
        $(document).on('click', '#<%=_htmlEditorBtn.ClientID%>', function(e) {
            var imageOptions = <%= AreOptionsWithImage.ToString().ToLower() %>;
            var imageError = imageOptions && getNewOptionImageId() == '';
            var optionAlias = $('#newOptionAlias').val();
            
            if (imageError) {
                showOptionsError('image');                
            } else if (imageOptions && (optionAlias == '' || optionAlias == aliasDefaultText)) {
                showOptionsError('alias'); 
            } else {
                $('#<%=_newOptionBtn.ClientID%>').click();
                $('#existingOptions .optionSelect').last().find('.editHtmlDiv').click();
            }
        });

        bindCancelUpdateEvents();

    });

    function getNewOptionImageId() {
        <%if(AreOptionsWithImage){%>
                return $('#newOptionImageId').val();
        <% } else { %>
                return '';
        <% } %>
    }

    function bindCancelUpdateEvents() {
        $(".optionActions").on('click', '#<%=_updateOptionButton.ClientID %>', function(e) {
            UpdateOptions();  
            currentEditOptionId = null;              

            renderOptions(options);
            e.preventDefault();
            e.stopPropagation();
            return;        
        });

        $('.optionActions').on('click', '#<%=_cancelOptionButton.ClientID %>', function(e){
            onOptionEditCancel(currentEditOptionId);
            e.preventDefault();
            e.stopPropagation();
            return;        
        });
    }
    //Change image on the specified option handler
    function changeImageOnOption(optionId){
        var imageId;
        if (optionId == '')
            imageId = $('#newOptionImageId').val();
        else
            imageId = $("#optionImageInput_" + optionId).val();
        if (typeof(templateEditor) == 'undefined')
        {
            showDialog('<%=ResolveUrl("~/") %>Forms/Surveys/UploadImage.aspx?optionId=' + optionId + '&imageId=' + imageId, 550, 650);
        }
        else
        {
            var params = new Array({name: 'optionId', value: optionId}, {name: 'imageId', value: imageId});
            templateEditor.openChildWindow(0, <%=PagePosition %>, 'UploadImage.aspx', params, 'wizard');
        }

    }

    //Handle upload image dialog closed
	function imageUploaded(args){
        if (args.op!='imageUploaded')
            return;

        if (args.optionId==''){
            $('#newOptionImageId').val(args.imageId);
            $('#_newUploadedImage img').attr('src','<%=ResolveUrl("~/ViewContent.aspx") %>?ContentId=' + args.imageId);
            $('#_newUploadedImage').show();
            $('#<%=_uploadImageBtn.ClientID %>').hide();
        }
        else{
            $('#optionImageInput_' + args.optionId).val(args.imageId);
            $('#optionImageEdit_' + args.optionId + ' img').attr('src', '<%= ResolveUrl("~/ViewContent.aspx")%>?ContentId=' + args.imageId);
        }
	}

    function onOptionTextClick() {
        var newEditOptionId = $(this).attr('id').replace('optionText_','');
        onOptionClick("text", newEditOptionId);	    
    }
    
    function onOptionAliasClick() {
        var newEditOptionId = $(this).attr('id').replace('optionAlias_','');
        onOptionClick("alias", newEditOptionId);
    }

    //
    function onOptionClick(target, newEditOptionId){
        var isHtmlOption = isHtml($('#optionText_' + newEditOptionId).html());

        if(target == 'text' && isHtmlOption) {
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

        //Initialize editors
        $('#optionAliasInput_' + optionId).val(trim($('#optionAlias_' + optionId).text()));
        var imageId = $('#optionImageInput_' + optionId).val();
        $('#optionImageEdit_' + optionId + ' img').attr('src', '<%= ResolveUrl("~/ViewContent.aspx")%>?ContentId=' + imageId);

        //Hide/show elements
        $('#optionAlias_' + optionId).hide();
        $('#removeOptionButton_' + optionId).hide();                        
        $('#optionImage_' + optionId).hide();

        $('#editOptionAlias_' + optionId).show();                
        $('#optionImageEdit_' + optionId).show();
        
        if(!isHtmlOption) {
            $('#optionTextInput_' + optionId).val(trim($('#optionText_' + optionId).text()));
            
            $('#optionText_' + optionId).hide();
            $('#editOptionText_' + optionId).show();
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
    function onOptionEditCancel(optionId){
        if (optionId == null || optionId == ''){
            return;
        }

        currentEditOptionId = null;

        //Unbind events
        $('#optionTextInput_' + optionId).unbind('keyup', onOptionTextKeyPress);
        $('#optionAliasInput_' + optionId).unbind('keyup', onOptionAliasKeyPress);        

        $('#optionTextInput_' + optionId).val($("#optionText_" + optionId).text());
        $('#optionAliasInput_' + optionId).val($("#optionAlias_" + optionId).text());
        $('#optionImageInput_' + optionId).val($('#optionImageInputOld_' + optionId).val());

        //Hide/show elements
        $('#editOptionText_' + optionId).hide();
        $('#editOptionAlias_' + optionId).hide();        
        $('#optionImageEdit_' + optionId).hide();

        $('#optionText_' + optionId).show();
        $('#optionAlias_' + optionId).show();    
        $('#removeOptionButton_' + optionId).show();                              
        $('#optionImage_' + optionId).show();                         

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
            <%if(AreOptionsWithImage){%>
             options[i].imageId = $("#optionImageInput_" + optionId).val();
            <%}%>
            
            var newText = $("#optionTextInput_" + optionId).val();

            if (isHtml(newText) && newText.indexOf("&lt;") == 0) {
                newText = htmlDecode(newText);
            }

            <% if (!AreOptionsWithImage) {%>
                if (newText != '')
            <%}%>
            {
                options[i].text = stripScripts(newText);  
            }

            var newAlias = htmlEncode(stripScripts($("#optionAliasInput_" + optionId).val()));    
            <%if (AreOptionsWithImage){%>
              if (newAlias != '')
            <%}%>
            options[i].alias = newAlias;            
        }
    }

    //
    //
    function addOption(optionId, isDefault, text, alias, imageId) {        
        options.push({ optionId: optionId, position: options.length + 1, text: text, alias: alias, isDefault: isDefault, imageId: imageId });      
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
        $('#existingOptions').children('ul').each(function(index){
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
    
    function optionHtmlEncode(value){
        return $('<div/>').text(value).html().split("\"").join("&quot;");
    }
    
    //
    function onEditHtmlClick(e) {
        e.stopPropagation();
        
        var currentRow = $(this).attr('optionId');
        var html = $('#optionText_' + currentRow).html().replace(/#/g, '%23').replace(/&/g, '%26');

        <% if(IsNew) { %>
            $('#<%= _currentrow.ClientID %>').val(currentRow);
            $('#<%= _currenthtml.ClientID %>').val(html);

            window.location = $('#<%= _postOptions.ClientID %>').attr('href');
            return;
        <% } %>

        var params = new Array({name: 'html', value: html}, {name: 'row', value: currentRow}, {name: 'callback', value: 'onHtmlDialogClosed'});

        templateEditor.openChildWindow(<%=ItemId %>, <%=PagePosition %>, 'HtmlEditor.aspx', params);
    }

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
        <%if(AreOptionsWithImage)
          {%>
          var optionImageTemplate = '<div class="left input fixed_200"><div id="optionImage_[OPTIONID]"><img src="<%= ResolveUrl("~/ViewContent.aspx")%>?ContentId=[IMAGEID]" style="max-width:190px;" /></div><div id="optionImageEdit_[OPTIONID]" style="display:none;"><a href="javascript:changeImageOnOption([OPTIONID])"><img src="<%= ResolveUrl("~/ViewContent.aspx")%>?ContentId=[IMAGEID]" style="max-width:190px;" /></a></div><input type="hidden" id="optionImageInput_[OPTIONID]" name="optionImageInput_[OPTIONID]" value="[IMAGEID]" /><input type="hidden" id="optionImageInputOld_[OPTIONID]" name="optionImageInputOld_[OPTIONID]" value="[IMAGEID]" /></div>';               
        <%
          }%>
        var optionTextTemplate = '<div class="left input fixed_200"><div id="optionText_[OPTIONID]" class="optionText">[OPTIONTEXT]</div><div id="editOptionText_[OPTIONID]" style="display:none;"><input type="text" name="optionTextInput_[OPTIONID]" id="optionTextInput_[OPTIONID]" value="[OPTIONTEXT]" /></div></div>';               
        var optionAliasTemplate = '<div class="left input fixed_200"><div id="optionAlias_[OPTIONID]" class="optionAlias">[OPTIONALIAS]</div><div id="editOptionAlias_[OPTIONID]" style="display:none;"><input type="text" name="optionAliasInput_[OPTIONID]" id="optionAliasInput_[OPTIONID]" value="[OPTIONALIAS]" /></div></div>';
        var optionHtmlTemplate = '<div class="left editHtmlDiv" style="margin-right: 5px;" optionId="[OPTIONID]" optionPosition="[POSITION]"><a class="editHtmlLink ckbxButton roundedCorners border999 shadow999 orangeButton OptionEditorButton" uframeignore="true" id="editHtmlRowLink_[OPTIONID]">HTML</a></div>';        
        var removeTemplate = '<div class="left input optionActions" optionPosition="[POSITION]"><a class="ckbxButton roundedCorners border999 shadow999 redButton OptionEditorButton" uframeignore="true" id="removeOptionButton_[OPTIONID]">-</a><div id="editOptionButtons_[OPTIONID]"></div></div><br class="clear" />';

        for(var i = 0; i < options.length; i++){
            <%if(AreOptionsWithImage)
              {%>
              if (options[i].imageId == '')
                continue;
            <%
              }%>

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

            <%if(AreOptionsWithImage)
              {%>
              var optionImage = $(
                optionImageTemplate
                    .replace('[POSITION]', options[i].position)
                    .replace(/\[OPTIONID\]/g, options[i].optionId)
                    .replace(/\[OPTIONTEXT\]/g, (isHtml(options[i].text)? options[i].text : optionHtmlEncode(options[i].text)))                 
                    .replace(/\[IMAGEID\]/g, options[i].imageId)
            );
            <%
              }%>            

            var optionText = $(
                optionTextTemplate
                    .replace('[POSITION]', options[i].position)
                    .replace(/\[OPTIONID\]/g, options[i].optionId)
                    .replace(/\[OPTIONTEXT\]/g, (isHtml(options[i].text)? options[i].text : optionHtmlEncode(options[i].text)))             
                    .replace(/\[IMAGEID\]/g, options[i].imageId)
            );
            
            var optionAlias = $(
                optionAliasTemplate
                    .replace('[POSITION]', options[i].position)
                    .replace(/\[OPTIONID\]/g, options[i].optionId)
                    .replace(/\[OPTIONALIAS\]/g, (isEncoded(options[i].alias)? options[i].alias : optionHtmlEncode(options[i].alias)))
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
            <%if (AreOptionsWithImage) {%>
              optionList.append(optionImage);
            <% } %>
            optionList.append(optionText);
            optionList.append(optionAlias);
            optionList.append(edit);

            optionList.append(remove);
            
            $('#existingOptions').append(optionList);
        }

        if(options.length % 2 == 1){
            $('#newOptionContainer').addClass('detailZebra');
        }
        else{
           $('#newOptionContainer').removeClass('detailZebra');
        }

        bindCancelUpdateEvents();
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
    <input type="text" id="normalEntryOptionOrder" name="normalEntryOptionOrder" />
    <btn:CheckboxButton ID="_postOptions" runat="server" style="display: none;" uframeignore="true" />
    <btn:CheckboxButton ID="_updateOptionButton" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton smallButton" runat="server" TextId="/pageText/forms/surveys/itemEditors/rankOrderOptionsNormalEntry.ascx/Update" uframeignore="true" />
    <btn:CheckboxButton ID="_cancelOptionButton" CssClass="ckbxButton roundedCorners border999 shadow999 redButton smallButton" runat="server" TextId="/pageText/forms/surveys/itemEditors/rankOrderOptionsNormalEntry.ascx/Cancel" uframeignore="true" />
</div>

<%-- Existing Options --%>
<div class="statsContentHeader">
    <div class="fixed_50 left"><%=WebTextManager.GetText("/pageText/forms/surveys/itemEditors/rankOrderOptionsNormalEntry.ascx/default") %></div>    
    <% if (AreOptionsWithImage)
       {%>
       <div class="fixed_200 left"><%=WebTextManager.GetText("/pageText/forms/surveys/itemEditors/rankOrderOptionsNormalEntry.ascx/image") %></div>
    <%
       }%>
    <div class="fixed_200 left"><%=WebTextManager.GetText("/pageText/forms/surveys/itemEditors/rankOrderOptionsNormalEntry.ascx/" + (AreOptionsWithImage ? "label" : "text")) %></div>
    <div class="fixed_200 left"><%=WebTextManager.GetText("/pageText/forms/surveys/itemEditors/rankOrderOptionsNormalEntry.ascx/alias") %></div>
    <br class="clear" />
</div>

<div id="existingOptions"></div>

<div class="error message alias validation-input-error" style="display:none;"><%=WebTextManager.GetText("/controlText/sliderEditor/aliasRequiredError") %></div>
<div class="error message image validation-input-error" style="display:none;"><%=WebTextManager.GetText("/controlText/sliderEditor/imageRequiredError") %></div>
<div class="error message alias-image validation-input-error" style="display:none;"><%=WebTextManager.GetText("/controlText/sliderEditor/aliasAndimageRequiredError") %></div>

<%-- New Option --%>
<div class="dashStatsContent allMenu" id="newOptionContainer">
    <div class="left input fixed_50" style="text-align:center;"><input type="<%=GetRowDefaultInputType() %>" name="<%=GetNewRowDefaultInputName()%>" id="<%=GetNewRowDefaultInputName()%>" /></div>
        <% if (AreOptionsWithImage)
           {%>
       <div class="left input fixed_200">    
           <btn:CheckboxButton ID="_uploadImageBtn" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton" TextID="/pageText/forms/surveys/itemEditors/rankOrderOptionsNormalEntry.ascx/upload" uframeignore="true" style="color:white;" />
           <div id="_newUploadedImage" style="display:none;"><a href="javascript:changeImageOnOption('');"><img src="<%=ResolveUrl("~/App_Themes/CheckboxTheme/Images/empty.gif") %>" style="max-width:190px;" /></a></div>
           <input type="hidden" id="newOptionImageId" />  
       </div> 
    <%}%>
    <div class="left input fixed_200">
        <input type="text" id="newOptionTxt" value="<%=
                       WebTextManager.GetText("/pageText/forms/surveys/itemEditors/rankOrderOptionsNormalEntry.ascx/" + (AreOptionsWithImage ? "enterLabel" : "newChoice"))%>" />
    </div>
    <div class="left input fixed_200"><input type="text" id="newOptionAlias" value="<%=WebTextManager.GetText("/pageText/forms/surveys/itemEditors/rankOrderOptionsNormalEntry.ascx/enterAlias") %>" /></div>
    
    <div class="left input"><btn:CheckboxButton ID="_newOptionBtn" runat="server" Text="+" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton OptionEditorButton" uframeignore="true" OnClientClick="return false;" /></div>
    <div class="left input"><btn:CheckboxButton ID="_htmlEditorBtn" style="margin-left: 5px;" runat="server" Text="HTML" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton OptionEditorButton" uframeignore="true" /></div>
    <br class="clear" />    
</div>

<script type="text/C#" runat="server">
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected string GetRowDefaultInputType()
    {
        return "radio";
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected string GetRowDefaultGroupName()
    {
        return "defaultSelect";
    }

     /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected string GetNewRowDefaultInputName()
    {
        return "defaultSelect";
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