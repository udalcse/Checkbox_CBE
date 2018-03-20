<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="SliderOptionsNormalEntry.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.SliderOptionsNormalEntry"  %>
<%@ Import Namespace="Checkbox.Web" %>

<script type="text/javascript">
    var options = new Array();
    var hasOptionPoints = <%=HasPoints?"true":"false"%>;
    var currentEditOptionId = null;
    var okButton;
    var okButtonHref;
    var imagesMode = <%= AreOptionsWithImage.ToString().ToLower() %>;
    var maxImageOptions = 11;

    $(document).ready(function () {
        $('#<%=_newOptionBtn.ClientID%>').attr('href', 'javascript:void(0);');
        if(getParameterByName("isNew")!="true") {
            $('#<%=_uploadImageBtn.ClientID%>').attr('href', 'javascript:void(0);');
            $('#<%=_updateOptionButton.ClientID %>').attr('href', 'javascript:void(0);');
            $('#<%=_cancelOptionButton.ClientID %>').attr('href', 'javascript:void(0);');
        }

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
                <%=listOption.Points %>,
                <%=listOption.ContentID.HasValue ? listOption.ContentID.Value.ToString() : "''" %>
            );
        <%} %>
        
        renderOptions(options);
        
        $(document).on('click', ".optionDefaultInput", function(e){
            e.stopPropagation();
        });
        
        $(document).on('mouseover', ".optionSelect", function() { $(this).addClass('hover'); });
        $(document).on('mouseout', ".optionSelect", function() { $(this).removeClass('hover'); });
        
        $(document).off('click', ".optionSelect");
        $(document).off('keydown', '#newOptionTxt');
        $(document).off('keydown', '#newOptionAlias');
        
        $(document).on('click', ".optionSelect", onOptionClick);

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

        if (hasOptionPoints)
        {
            $(document).off('keydown', '#newOptionPoints');
            $(document).on('keydown', '#newOptionPoints', function(e){
                if(e.which == 13){
                    $('#<%=_newOptionBtn.ClientID%>').click();
                }
            });
        }

        $(document).on('focus', '#newOptionTxt', function(){
            if($('#newOptionTxt').val() == '<%=WebTextManager.GetText("/pageText/forms/surveys/itemEditors/sliderOptionsNormalEntry.ascx/newChoice")%>'){
                $('#newOptionTxt').val('');
            }
        });

        $(document).on('focus', '#newOptionAlias', function(){
            if($('#newOptionAlias').val() == '<%=WebTextManager.GetText("/pageText/forms/surveys/itemEditors/sliderOptionsNormalEntry.ascx/enterAlias")%>'){
                $('#newOptionAlias').val('');
            }
        });


        if (hasOptionPoints){
            $(document).on('focus', '#newOptionPoints', function(){
                if($('#newOptionPoints').val() == '<%=WebTextManager.GetText("/pageText/forms/surveys/itemEditors/sliderOptionsNormalEntry.ascx/enterPoints")%>'){
                    $('#newOptionPoints').val('');
                }
            });
        }

        //Upload Image click
        $('#<%=_uploadImageBtn.ClientID %>').click(function () {
            if(typeof templateEditor != 'undefined') {
                templateEditor.openChildWindow(0, <%=PagePosition %>, '<%=ResolveUrl("~/Forms/Surveys/UploadImage.aspx")%>', null, 'wizard');
            }
        });

        $(document).off('click', '#<%=_newOptionBtn.ClientID%>');
        $(document).on('click', '#<%=_newOptionBtn.ClientID%>', function(e) {
          /*  var isValid = prePostValidation(e);
            if (!isValid)
                return;*/

            var isDefault=$('#<%=GetNewRowDefaultInputName() %>').is(":checked");
            <%if(AreOptionsWithImage)
              {%>
            var optionTxt = '';
            var optionImageId = $('#newOptionImageId').val();
            <%
              } else
              {%>
            var optionTxt = stripScripts($('#newOptionTxt').val());
            var optionImageId = '';
            <%
              }%>
            
            var optionAlias = stripScripts($('#newOptionAlias').val());
            var optionPoints = stripScripts($('#newOptionPoints').val());
            optionTxt = htmlEncode(optionTxt);
            optionAlias = htmlEncode(optionAlias);

            //Update current options
            UpdateOptions();

            if(<%=(!AreOptionsWithImage).ToString().ToLower() %>) {
                if (optionTxt == '') {
                    $('#newOptionTxt').val('');
                    return;
                }  
                if (optionTxt == '<%=WebTextManager.GetText("/pageText/forms/surveys/itemEditors/sliderOptionsNormalEntry.ascx/newChoice")%>') {
                    return;   
                }
            }

            <%if (AreOptionsWithImage) {%>   
                if(optionImageId == '') {
                    if (optionAlias == '') {
                        showError('alias-image');
                        $('#newOptionAlias').val('');
                        return;
                    }
                    if(optionAlias == '<%= WebTextManager.GetText("/pageText/forms/surveys/itemEditors/sliderOptionsNormalEntry.ascx/enterAlias")%>') {
                        showError('alias-image');
                        return;
                    }
                    showError('image');
                    return;
                }
                if(optionAlias == ''){
                    showError('alias');
                    $('#newOptionAlias').val('');
                    return;
                }
                if(optionAlias == '<%= WebTextManager.GetText("/pageText/forms/surveys/itemEditors/sliderOptionsNormalEntry.ascx/enterAlias")%>') {
                    return;                
                }
                var error = false;
                $.each(options, function(ind, option){
                    if(option.alias == optionAlias) {
                        showError('alias-duplicate');
                        error = true;
                        return;
                    }
                });
                if(error)
                    return;
            <%
              } else
              {%>
                if(optionAlias == '<%= WebTextManager.GetText("/pageText/forms/surveys/itemEditors/sliderOptionsNormalEntry.ascx/enterAlias")%>'){
                    optionAlias = '';
                }
              <%
              }%>


            if(isNaN(optionPoints)){
                optionPoints = '';
            }
            
            //calculate index
            var id = 0;
            if(options.length > 0)
            {
                for(var i=0; i<options.length; i++)
                {
                    if(options[i].optionId < id)
                        id = options[i].optionId;
                }
            }
            id = id - 1;

            addOption(
                id,
                isDefault,
                optionTxt,
                optionAlias,
                optionPoints,
                optionImageId
            );

            $('#newOptionTxt').val('');
            $('#newOptionPoints').val('');
            $('#newOptionAlias').val('');    
            $('#newOptionImageId').val('');
            $('#<%= _uploadImageBtn.ClientID %>').show();
            $('#_newUploadedImage').hide();
            $('#<%=GetNewRowDefaultInputName() %>').removeAttr('checked');

            renderOptions(options);

            checkForNewOptionButtonHiding();
            checkForOptionsLimitWarning();

            $('#newOptionTxt').focus();

            e.preventDefault();
            return;
        });

        $('#<%=_updateOptionButton.ClientID %>').click(function(e){            
            UpdateOptions();  
            currentEditOptionId = null;              

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

       checkForNewOptionButtonHiding();
       checkForNewOptionRowHiding();
       checkForOptionsLimitWarning();
    });
    
    //check if we should hide new option button
    function checkForNewOptionButtonHiding() {
        <% if(AreOptionsWithImage) { %>
            if(options.length > maxImageOptions-2) {
                $('#<%=_newOptionBtn.ClientID%>').hide();
            } else {
                $('#<%=_newOptionBtn.ClientID%>').show();
            }
        <% } %>
    }

    //check if we should hide new option row
    function checkForNewOptionRowHiding() {
        <% if(AreOptionsWithImage) { %>
            if(options.length > maxImageOptions-1) {
                $('#newOptionContainer').hide();
            } else {
                $('#newOptionContainer').show();
            }
        <% } %>
    }

    //check if we should hide new option row
    function checkForOptionsLimitWarning() {
        <% if(AreOptionsWithImage) { %>
            if (options.length > maxImageOptions-2) {
                $('.warning.options-limit').show();
            } else {
                $('.warning.options-limit').hide();
            }
        <% } %>
    }
    
    //show specific error
    function showError(className) {
        if ($('.validation-input-error.' + className).is(":hidden")) {
            $('.validation-input-error.' + className).slideDown(300).delay(2000).slideUp(600);
        }
    }

    //calls for validation image slider
    function prePostValidation(e) {        
        //check new option validation
        var isAliasValid = $('#newOptionAlias').val() != '' && 
            $('#newOptionAlias').val() != '<%= WebTextManager.GetText("/pageText/forms/surveys/itemEditors/sliderOptionsNormalEntry.ascx/enterAlias")%>';
        var isImageValid = $('#newOptionImageId').val() != '';
        var isTextValid = $('#newOptionTxt').val() != '' && 
            $('#newOptionTxt').val() != '<%=WebTextManager.GetText("/pageText/forms/surveys/itemEditors/sliderOptionsNormalEntry.ascx/newChoice")%>';
        if(imagesMode) {
            return isAliasValid && isImageValid;
        } else 
            return isTextValid;
    }

    //get parameter by name
    function getParameterByName(name) {
        name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
        var regexS = "[\\?&]" + name + "=([^&#]*)";
        var regex = new RegExp(regexS);
        var results = regex.exec(window.location.search);
        if(results == null)
        return "";
        else
        return decodeURIComponent(results[1].replace(/\+/g, " "));
    }

    //Change image on the specified option handler
    function changeImageOnOption(optionId){
        <% if(Context.Request.QueryString["isNew"] == "true") { %>
            $('#<%= _updateOptionHidden.ClientID %>').val(optionId);
            window.location = $('#<%= _uploadImageHiddenBtn.ClientID %>').attr('href');
        <% } else { %>
            var imageId;
            if (optionId == '')
                imageId = $('#newOptionImageId').val();
            else
                imageId = $("#optionImageInput_" + optionId).val();
            var params = new Array({name: 'optionId', value: optionId}, {name: 'imageId', value: imageId});
            templateEditor.openChildWindow(0, <%=PagePosition %>, '<%=ResolveUrl("~/Forms/Surveys/UploadImage.aspx")%>', params, 'wizard');
        <% } %>
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
        } else{
            $('#optionImageInput_' + args.optionId).val(args.imageId);
            $('#optionImageEdit_' + args.optionId + ' img').attr('src', '<%= ResolveUrl("~/ViewContent.aspx")%>?ContentId=' + args.imageId);
        }
        UpdateOptions();
	}
    
    //
    function onOptionClick(){
        var newEditOptionId = $(this).attr('id').replace('option_','');

        if (currentEditOptionId == newEditOptionId){
            return;
        }

        if (currentEditOptionId != null && currentEditOptionId != ''){
            onOptionEditCancel(currentEditOptionId);
        }

        currentEditOptionId = newEditOptionId;

        onOptionEdit(newEditOptionId);
    }

    function onOptionEdit(optionId){
        //Move buttons & piping.
        var updateButton = $('#<%=_updateOptionButton.ClientID %>').detach();
        var cancelButton = $('#<%=_cancelOptionButton.ClientID %>').detach();

        updateButton.appendTo('#editOptionButtons_'+ optionId);
        //$('nbsp;').appendTo('#editOptionButtons_'+ optionId);
        cancelButton.appendTo('#editOptionButtons_'+ optionId);

        //Bind keypress       
        $('#optionTextInput_' + optionId).bind('keyup', onOptionTextKeyPress);
        $('#optionAliasInput_' + optionId).bind('keyup', onOptionAliasKeyPress);
        $('#optionPointsInput_' + optionId).bind('keyup', onOptionPointsKeyPress);

        //Initialize editors
        $('#optionTextInput_' + optionId).val(trim($('#optionText_' + optionId).text()));
        $('#optionAliasInput_' + optionId).val(trim($('#optionAlias_' + optionId).text()));
        $('#optionPointsInput_' + optionId).val(trim($('#optionPoints_' + optionId).text()));
        var imageId = $('#optionImageInput_' + optionId).val();
        $('#optionImageEdit_' + optionId + ' img').attr('src', '<%= ResolveUrl("~/ViewContent.aspx")%>?ContentId=' + imageId);

        //Hide/show elements
        $('#optionText_' + optionId).hide();
        $('#optionAlias_' + optionId).hide();
        $('#removeOptionButton_' + optionId).hide();                        
        $('#optionImage_' + optionId).hide();

        $('#editOptionText_' + optionId).show();
        $('#editOptionAlias_' + optionId).show();                
        $('#optionImageEdit_' + optionId).show();

        if (hasOptionPoints){
            $('#optionPoints_' + optionId).hide();
            $('#editOptionPoints_' + optionId).show();
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
        $('#optionPointsInput_' + optionId).unbind('keyup', onOptionPointsKeyPress);

        $('#optionTextInput_' + optionId).val($("#optionText_" + optionId).text());
        $('#optionAliasInput_' + optionId).val($("#optionAlias_" + optionId).text());
        $('#optionPointsInput_' + optionId).val($("#optionPoints_" + optionId).text());    
        $('#optionImageInput_' + optionId).val($('#optionImageInputOld_' + optionId).val());

        //Hide/show elements
        $('#editOptionText_' + optionId).hide();
        $('#editOptionAlias_' + optionId).hide();        
        $('#optionImageEdit_' + optionId).hide();

        $('#optionText_' + optionId).show();
        $('#optionAlias_' + optionId).show();    
        $('#removeOptionButton_' + optionId).show();                              
        $('#optionImage_' + optionId).show();                         

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
            <%if(AreOptionsWithImage)
              {%>
             options[i].imageId = $("#optionImageInput_" + optionId).val();
            <%
              } else
              {%>
              var newText = htmlEncode(stripScripts($("#optionTextInput_" + optionId).val()));
              if (newText != '')
                options[i].text = newText;
            <%
              }%>
              var newAlias = htmlEncode(stripScripts($("#optionAliasInput_" + optionId).val()));    
            <%if (AreOptionsWithImage)
              {%>
              if (newAlias != '')
            <%
              }%>
            options[i].alias = newAlias;
            
            var points = htmlEncode(stripScripts($("#optionPointsInput_" + optionId).val()));
            
            if (isNaN(points)){
                points = options[i].points;
            }

            options[i].points = points;
        }
    }

    //
    //
    function addOption(optionId, isDefault, text, alias, points, imageId) {        
        options.push({ optionId: optionId, position: options.length + 1, text: text, alias: alias, points: points, isDefault: isDefault, imageId: imageId });      
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

        renderOptions(options);
        checkForNewOptionButtonHiding();
        checkForNewOptionRowHiding();
        checkForOptionsLimitWarning();
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
          var optionTextImageTemplate = '<div class="left input fixed_200"><div id="optionImage_[OPTIONID]"><img src="<%= ResolveUrl("~/ViewContent.aspx")%>?ContentId=[IMAGEID]" style="max-width:190px;" /></div><div id="optionImageEdit_[OPTIONID]" style="display:none;"><a href="javascript:changeImageOnOption([OPTIONID])"><img src="<%= ResolveUrl("~/ViewContent.aspx")%>?ContentId=[IMAGEID]" style="max-width:190px;" /></a></div><input type="hidden" id="optionImageInput_[OPTIONID]" value="[IMAGEID]" /><input type="hidden" id="optionImageInputOld_[OPTIONID]" value="[IMAGEID]" /></div>'; 
        <%
          } else
          {%>
          var optionTextImageTemplate = '<div class="left input fixed_200"><div id="optionText_[OPTIONID]">[OPTIONTEXT]</div><div id="editOptionText_[OPTIONID]" style="display:none;"><input type="text" name="optionTextInput_[OPTIONID]" id="optionTextInput_[OPTIONID]" value="[OPTIONTEXT]" /></div></div>';               
          <%
          }%>
        var optionAliasTemplate = '<div class="left input fixed_200"><div id="optionAlias_[OPTIONID]">[OPTIONALIAS]</div><div id="editOptionAlias_[OPTIONID]" style="display:none;"><input type="text" name="optionAliasInput_[OPTIONID]" id="optionAliasInput_[OPTIONID]" value="[OPTIONALIAS]" /></div></div>';
        var optionPointsTemplate = '<div class="left input fixed_50"><div id="optionPoints_[OPTIONID]">[OPTIONPOINTS]</div><div id="editOptionPoints_[OPTIONID]" style="display:none;"><input type="text" name="optionPointsInput_[OPTIONID]" id="optionPointsInput_[OPTIONID]" value="[OPTIONPOINTS]" style="width:50px;" /></div></div>';        
        var removeTemplate = '<div class="left input" optionPosition="[POSITION]"><a class="ckbxButton roundedCorners border999 shadow999 redButton OptionEditorButton" uframeignore="true" id="removeOptionButton_[OPTIONID]">-</a><div id="editOptionButtons_[OPTIONID]"></div></div><br class="clear" />';

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

            var optionTextImage = $(
                optionTextImageTemplate
                    .replace('[POSITION]', options[i].position)
                    .replace(/\[OPTIONID\]/g, options[i].optionId)
                    .replace(/\[OPTIONTEXT\]/g, (isEncoded(options[i].text)? options[i].text : optionHtmlEncode(options[i].text)))
                    .replace(/\[IMAGEID\]/g, options[i].imageId)
            );

            
            var optionAlias = $(
                optionAliasTemplate
                    .replace('[POSITION]', options[i].position)
                    .replace(/\[OPTIONID\]/g, options[i].optionId)
                    .replace(/\[OPTIONALIAS\]/g, (isEncoded(options[i].alias)? options[i].alias : optionHtmlEncode(options[i].alias)))
            );

            var optionPoints = $(
                optionPointsTemplate
                    .replace('[POSITION]', options[i].position)
                    .replace(/\[OPTIONID\]/g, options[i].optionId)
                    .replace(/\[OPTIONPOINTS\]/g, options[i].points)
            );

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

            optionList.append(optionDefault);
            optionList.append(optionTextImage);
            optionList.append(optionAlias);

            if (hasOptionPoints){
                optionList.append(optionPoints);
            }
            optionList.append(remove);

            
            $('#existingOptions').append(optionList);
        }

        if(options.length % 2 == 1){
            $('#newOptionContainer').addClass('detailZebra');
        }
        else{
           $('#newOptionContainer').removeClass('detailZebra');
        }
    }    
    function isEncoded(text) {
        return text.indexOf("&lt;") >= 0 || text.indexOf("&gt") >= 0;
    }
</script>

<%-- Hidden input to store option order  --%>
<div id="hiddenControls" style="display:none;">
    <input type="text" id="normalEntryOptionOrder" name="normalEntryOptionOrder" />
    <btn:CheckboxButton ID="_updateOptionButton" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton smallButton" runat="server" TextId="/pageText/forms/surveys/itemEditors/sliderOptionsNormalEntry.ascx/Update" uframeignore="true" />
    <btn:CheckboxButton ID="_cancelOptionButton" CssClass="ckbxButton roundedCorners border999 shadow999 redButton smallButton" runat="server" TextId="/pageText/forms/surveys/itemEditors/sliderOptionsNormalEntry.ascx/Cancel" uframeignore="true" />
</div>

 <%
     if (AreOptionsWithImage)
   {
%>
    <div class="warning message" style="display:none;"><%=WebTextManager.GetText("/controlText/sliderEditor/aliasRequired") %></div>
    <div class="warning message options-limit" style="display:none;"><%=WebTextManager.GetText("/controlText/sliderEditor/optionsLimitReached") %></div>
    <div class="error message alias validation-input-error" style="display:none;"><%=WebTextManager.GetText("/controlText/sliderEditor/aliasRequiredError") %></div>
    <div class="error message alias-duplicate validation-input-error" style="display:none;"><%=WebTextManager.GetText("/controlText/sliderEditor/aliasDuplicatedError")%></div>
    <div class="error message image validation-input-error" style="display:none;"><%=WebTextManager.GetText("/controlText/sliderEditor/imageRequiredError") %></div>
    <div class="error message alias-image validation-input-error" style="display:none;"><%=WebTextManager.GetText("/controlText/sliderEditor/aliasAndimageRequiredError") %></div>
<%
   }
   %>
       
       

   <%-- Existing Options --%>
<div class="statsContentHeader">
    <div class="fixed_75 left"><%=WebTextManager.GetText("/pageText/forms/surveys/itemEditors/sliderOptionsNormalEntry.ascx/default") %></div>    
    <div class="fixed_200 left"><%=WebTextManager.GetText("/pageText/forms/surveys/itemEditors/sliderOptionsNormalEntry.ascx/" + (AreOptionsWithImage ? "image" : "text")) %></div>
    <div class="fixed_200 left"><%=WebTextManager.GetText("/pageText/forms/surveys/itemEditors/sliderOptionsNormalEntry.ascx/alias") %></div>
<% if (HasPoints)
    { %>
    <div class="fixed_50 left"><%=WebTextManager.GetText("/pageText/forms/surveys/itemEditors/sliderOptionsNormalEntry.ascx/points") %></div>
<%} %>
    <br class="clear" />
</div>

<div id="existingOptions"></div>

<btn:CheckboxButton OnClientClick="return false;" ID="_uploadImageHiddenBtn" runat="server" uframeignore="true" style="display:none;" />
<asp:HiddenField ID="_updateOptionHidden" runat="server" />

<%-- New Option --%>
<div class="dashStatsContent allMenu" id="newOptionContainer">
    <div class="left input fixed_75" style="text-align:center;"><input type="<%=GetRowDefaultInputType() %>" name="<%=GetNewRowDefaultInputName()%>" id="<%=GetNewRowDefaultInputName()%>" /></div>
    <div class="left input fixed_200">    
        <% if (AreOptionsWithImage) { %>
       <btn:CheckboxButton ID="_uploadImageBtn" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton" TextID="/pageText/forms/surveys/itemEditors/sliderOptionsNormalEntry.ascx/upload" uframeignore="true" style="color:white;" />
       <div id="_newUploadedImage" style="display:none;"><a href="javascript:changeImageOnOption('');"><img src="<%=ResolveUrl("~/App_Themes/CheckboxTheme/Images/empty.gif") %>" style="max-width:190px;" /></a></div>
       <input type="hidden" id="newOptionImageId" />   
       <input type="hidden" id="newOptionImageName" />   
    <%}
      else
      {%>               
        <input type="text" id="newOptionTxt" value="<%=
                   WebTextManager.GetText("/pageText/forms/surveys/itemEditors/sliderOptionsNormalEntry.ascx/newChoice")%>" />
        <%
           }%>
    </div>
    <div class="left input fixed_200"><input type="text" id="newOptionAlias" value="<%=WebTextManager.GetText("/pageText/forms/surveys/itemEditors/sliderOptionsNormalEntry.ascx/enterAlias") %>" /></div>
    
<% if (HasPoints)
    { %>
    <div class="fixed_50 left input"><input type="text" id="newOptionPoints" style="width:45px;" value="<%=WebTextManager.GetText("/pageText/forms/surveys/itemEditors/sliderOptionsNormalEntry.ascx/enterPoints") %>" /></div>
<%} %>
    <div class="left input" style="padding-left:5px;"><btn:CheckboxButton ID="_newOptionBtn" runat="server" Text="+" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton OptionEditorButton" uframeignore="true" OnClientClick="return false;" /></div>
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
        return OptionTexts.ContainsKey(optionId) ?
            OptionTexts[optionId]
                .Replace("'", "\\'")
                .Replace("\n", string.Empty)
                .Replace(Environment.NewLine, string.Empty)
            : string.Empty;
    }

</script>