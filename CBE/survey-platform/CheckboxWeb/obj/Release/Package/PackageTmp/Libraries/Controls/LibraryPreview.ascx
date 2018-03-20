<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="LibraryPreview.ascx.cs" Inherits="CheckboxWeb.Libraries.Controls.LibraryPreview" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web" %>

<script type="text/javascript">
    <%-- Id of language select container --%>
    var _languageListId = '<%= _languageList.ClientID %>';

    <%-- Ensure service initialized --%>
    $(document).ready(function() {
        templateEditor.currentLanguage = $('#' + _languageListId).val();

         $('#' + _languageListId).change(function(){
            templateEditor.currentLanguage = $('#' + _languageListId).val();
            //alert(templateEditor.currentLanguage);
            reloadPreview();
         });
        
        $('#previewContainer').ajaxComplete(function() {
             resizePanels();
        });

        $('[marker="previewSpinner"]').css('width', '16px').css('height', '16px').css('position', 'absolute').css('left', '20px');
    });

    var _currentLibraryId = -1;
    
    //Reload item preview
    function reloadPreview(){
        svcSurveyManagement.getLibraryData(_at, _currentLibraryId, loadLibraryPreview);
    }

    function clearCurrentItem() {
        var iframe = document.getElementById('templateEditorModal');
        var innerDoc = iframe.contentDocument || iframe.contentWindow.document;
        var form = innerDoc.getElementById("aspnetForm");
        $.ajax({
            type: "POST",
            url: $(form).attr("action") + "&cmd=clearCurrentItem",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function(msg) {
            }
        });
    }
    
    
    //Load item previews
    function loadLibraryPreview(library) {
        //Clear preview container and add containers for each item
        $('#previewContainer').empty();
        $('#previewContainer').hide();
               
        if (library == null || library.ItemIds == null || library.DatabaseId == null) {
            return;
        }

        $('#loadingPanel').show();

        _currentLibraryId = library.DatabaseId;

        $('#previewHeader').show();

        //Update links
        var propsLink = 'javascript:templateEditor.openChildWindow("", "", "Properties.aspx", new Array({name:"onClose", value:"onDialogClosed"},{name:"lib",value:' + library.DatabaseId + '}), "properties");';
        var securityLink = 'javascript:templateEditor.openChildWindow("", "", "Permissions.aspx", new Array({name:"onClose", value:"onDialogClosed"},{name:"lib",value:' + library.DatabaseId + '}), "security");';
        var addItemLink = 'javascript:templateEditor.openChildWindow("", "", "AddItem.aspx", new Array({name:"onClose", value:"onDialogClosed"},{name:"lib",value:' + library.DatabaseId + '}, {name:"l",value:"' + templateEditor.currentLanguage + '"}), "wizard", null, clearCurrentItem);';

        $('#_securityLink').attr('href', securityLink);
        $('#_propertiesLink').attr('href', propsLink);
        $('#_buttonAddItem').attr('href', addItemLink);
        $('#_deleteLink').attr('href', 'javascript:alert("Delete template function is not implemented yet")');

        updateLibraryName(library);

        //Show/hide Edit library links based on security
        securityHelper.protect(
            '<%=WebUtilities.GetCurrentUserEncodedName() %>',
            svcAuthorization.RESOURCETYPE_LIBRARY,
            library.DatabaseId,
            '#libraryMenuContainer')
        .then(
            function(){
                $('#loadingPanel').hide();
                $('#previewContainer').show();
            }
        );

        //Show/hide edit item links based on security
        svcAuthorization.authorizeAccessD(
            '<%=WebUtilities.GetCurrentUserEncodedName() %>',
            svcAuthorization.RESOURCETYPE_LIBRARY,
            library.DatabaseId,
            'Library.Edit')
        .then(
            function(allowEdit){
                $('#previewContainer').empty();
                disableExport();
                for (var i = 0; i < library.ItemIds.length; i++) {         
                    var itemId = library.ItemIds[i];

                    var isCheckedMenu = false;
                    var menuIndex = library.MenuSettings.findIndex(function(s) { return s.Key == itemId });
                    if (menuIndex != -1) {
                        isCheckedMenu = library.MenuSettings[menuIndex].Value;
                    }

                    var zebraClass = i%2 == 0 ? ' zebra' : '';

                    var editItemLink = '';
                    var deleteItemLink = '';
                    var showInMenuLink = '';

                    if(allowEdit){
                        editItemLink = '<a class="ckbxButton smallButton silverButton roundedCorners shadow999 border999 editItemLink" id="editItem_' + itemId + '" href="#">Edit</a>';
                        deleteItemLink = '<a class="cancelButton" style="padding-right:10px;" id="deleteItem_' + itemId + '" href="#">Delete</a>';
                        showInMenuLink = '<div style="margin-right:30px;"><label for="showInMenu_'+ itemId + '" style="line-height:14px; padding-right:5px;">Include in quick-list</label><input type="checkbox" onchange="saveMenuState(this)" ' + (isCheckedMenu ? ' checked ' : '')  + ' style="vertical-align:bottom" data-id="'+itemId+'" id="showInMenu_' + itemId + '"/></div>';
                    }


                    //Add placeholder
                    $('#previewContainer').append(
                        '<div class="dashStatsWrapper border999 shadow999">' +
                        '   <div class="dashStatsHeader" id="itemHeader_' + itemId + '"><span class="mainStats left">Item: ' + (i + 1) + '</span><span class="mainStats left" id="itemType_' + itemId + '"></span><ul class="itemActionMenuSmall right allMenu"><li>' + showInMenuLink + '</li><li>' + deleteItemLink + '</li><li>' + editItemLink + '</li></ul><br class="clear" /></div>' +
                        '   <div class="padding10" id="itemContainer_' + library.ItemIds[i] + '">' + $('#loadingPanel').html() + '</div><div class="clear"></div>' +
                        '</div>'
                    );
                }
                if (library.ItemIds.length > 0)
                {
                    //Load item
                    loadItemPreview(library.DatabaseId, library.ItemIds[0], library.ItemIds.slice(1, library.ItemIds.length));
                }
                else
                {
                    enableExport();
                }
            }
        );
    }

    function saveMenuState(elem) {
        var data = $(elem).data();
        svcSurveyManagement.setItemLibraryOptions(_at, data.id, $(elem).is(":checked"));
    }

    function enableExport()
    {
        setTimeout(function(){ 
            $('#<%= _exportLink.ClientID%>').attr('class', $('#<%= _exportLink.ClientID%>').attr('class').replace(' inactive', ''));
            $('#<%= _copyLink.ClientID%>').attr('class', $('#<%= _copyLink.ClientID%>').attr('class').replace(' inactive', ''));
            $('[marker="previewSpinner"]').hide();
        }, 1000);
    }

    function disableExport()
    {
        $('#<%= _exportLink.ClientID%>').attr('class', $('#<%= _exportLink.ClientID%>').attr('class') + ' inactive');
        $('#<%= _copyLink.ClientID%>').attr('class', $('#<%= _copyLink.ClientID%>').attr('class') + ' inactive');
        $('[marker="previewSpinner"]').show();
    }

    function onExportClick(event)
    {
        if ($('[marker="previewSpinner"]').css('display') == 'none')
        {
            doExport();
        }        
        else
        {
            event = event || window.event;
            event.stopPropagation();
        }
    };

    function onCopyClick(event)
    {
        if ($('[marker="previewSpinner"]').css('display') == 'none')
        {
            doCopy();
        }        
        else
        {
            event = event || window.event;
            event.stopPropagation();
        }
    };



    //Update library name and description
    function updateLibraryName(libraryData){
        //Update properties and load preview/edit
        $('#libraryName').html(libraryData.Name);
        $('#libraryDescription').html('<i>' + libraryData.Description + '</i>');
    }

    //Load item preview
    function loadItemPreview(libraryId, itemId, itemsToLoad){
            svcSurveyManagement.getLibraryItemMetaData(
                _at, 
                libraryId, 
                itemId, 
                onItemMetaDataLoaded, 
                {itemId:itemId, libraryId:libraryId, items: itemsToLoad});
    }

    //
    function onItemMetaDataLoaded(itemData, args){
        if(itemData == null){
            $('#itemContainer_' + itemData.ItemId).html('<div class="Error">Unable to load preview for item with id: ' + args.itemId + '</div>');
            return;
        }
        //Set type
        $('#itemType_' + itemData.ItemId).html(itemData.TypeName);

        //Update edit link
        var editItemLink = 'javascript:templateEditor.openChildWindow(' + args.itemId + ', "", "EditItem.aspx", new Array({name:"lid",value:' + args.libraryId + '}, {name:"isNew",value: true}, {name:"l",value:"' + templateEditor.currentLanguage + '"}), "wizard");';

        $('#editItem_' + itemData.ItemId).attr('href', editItemLink);

        //Update delete link
        var deleteItemLink = 'javascript:doDeleteItem(' + args.itemId + ')';

        $('#deleteItem_' + itemData.ItemId).attr('href', deleteItemLink);
        var applicationRoot = '<%=ApplicationManager.ApplicationRoot %>';
        var applicatonRootForPath = applicationRoot == '/' ? '' : applicationRoot;

        //Load preview
        UFrameManager.init({
            id: 'itemContainer_' + itemData.ItemId,
            loadFrom : applicatonRootForPath + '/ItemHtml.aspx',
            params : {s:args.libraryId, i:itemData.ItemId, m: 'LibraryPreview', l: templateEditor.currentLanguage},
            progressTemplate : $('#<%=_itemLoadingPanel.ClientID %>').html(),
            showProgress: true
        });

        if (typeof args.items != "undefined" && args.items.length > 0)
        {
            //Load item
            loadItemPreview(args.libraryId, args.items[0], args.items.slice(1, args.items.length));
        }
        else
        {
            enableExport();
        }
    }

    function doDeleteItem(itemId){
        if (itemId == null || itemId == -1) {
            return; // if there is no selected item, then exit.
        }
        var args = {itemId: itemId};
        showConfirmDialogWithCallback(
            '<%=WebTextManager.GetText("/pageText/libraries/manage.aspx/deleteItemConfirmation") %>', 
            onDeleteLibraryItemConfirm,
            375,
            200,
            '<%=WebTextManager.GetText("/common/confirmDelete") %>',
            args
        );
    }

    //Confirm handler for deleting library item
    function onDeleteLibraryItemConfirm(args){
        UFrameManager.prepareOuterFormSubmit();
        __doPostBack('DeleteItem', _currentLibraryId + ':' + args.itemId);
    }

    //
    function editItem(itemId){
        showDialog('EditItem.aspx?i=' + itemId + '&lid=' + _currentLibraryId + '&p=1' ,'wizard');
    }

    function doCopy(){
        UFrameManager.prepareOuterFormSubmit();
        __doPostBack('_copyLink',_currentLibraryId);
    }

    function doDelete(){
        if(_currentLibraryId == null || _currentLibraryId == '' || _currentLibraryId == -1){
			return;
		}

        showConfirmDialogWithCallback(
            '<%=WebTextManager.GetText("/pageText/libraries/manage.aspx/deleteLibraryConfirm") %>', 
            onDeleteSelectedLibraryConfirm,
            375,
            200,
            '<%=WebTextManager.GetText("/pageText/libraries/manage.aspx/deleteLibraryConfirmation") %>'
        );
    }

    //Confirm handler for deleting libraries
    function onDeleteSelectedLibraryConfirm(args){
        var idArray = new Array();
        idArray.push(_currentLibraryId);

        if (idArray.length > 0){
            svcSurveyManagement.deleteLibraries(
                    _at,
                    idArray,
                    onLibraryDeleted,
                    idArray.length                     
                );
        }
    }

    //Library deleted handler
    function onLibraryDeleted(resultData){
        reloadLibraryList(); 
        <%if (!String.IsNullOrEmpty(OnLibraryDeleted))
          {%>
            <%=OnLibraryDeleted %>();
        <%
          }%>  
        <%if (!String.IsNullOrEmpty(ShowStatusMessageHandler))
          {%>
          var message = '<%=WebTextManager.GetText("/pageText/libraries/manage.aspx/libraryDeleted") %>';
          <%=ShowStatusMessageHandler %>(message, resultData);
        <%
          }%>
    }
</script>

 <div class="padding10" id="previewHeader" style="display:none;">
    <div id="loadingPanel" style="display:none;">
        <asp:Panel ID="_itemLoadingPanel" runat="server">
            <asp:Image ID="_spinner" runat="server" SkinID="ProgressSpinner" />
        </asp:Panel>
    </div>

   <div class="survey-header-container">
        <div class="header-content clearfix">
            <a class="action-menu-toggle action-button ckbxButton silverButton" id="library_actions_button" href="#"><span class="toggle-arrow"></span><%=WebTextManager.GetText("/pageText/editLibrary.aspx/libraryActions")%></a>
            <asp:DropDownList ID="_languageList" runat="server" CssClass="language-dropdown"></asp:DropDownList>
            <a class="action-button ckbxButton silverButton statistics_AddItem" protectPermission="Library.Edit" href="javascript:void(0);" id="_buttonAddItem"><%= WebTextManager.GetText("/pageText/editLibrary.aspx/addItem")%></a>
            <div id="library_actions_menu" class="groupMenu" style="margin-top: 30px !important;">
                <ul id="libraryActionsPlace" class="itemActionMenu">
                    <li><a class="ckbxButton silverButton" protectPermission="Library.Edit" href="javascript:void(0);" id="_propertiesLink"><%= WebTextManager.GetText("/pageText/editLibrary.aspx/properties")%></a></li>
                    <li><a class="ckbxButton silverButton" protectPermission="Library.Edit" href="javascript:void(0);" id="_securityLink"><%= WebTextManager.GetText("/pageText/editLibrary.aspx/permissions")%></a></li>
                    <li><asp:Image ID="_copySpinner" runat="server" SkinID="ProgressSpinner" marker="previewSpinner"/><a class="ckbxButton silverButton" href="#" onclick="onCopyClick(event);return false;" runat="server" id="_copyLink">Copy</a></li>
                    <li><asp:Image ID="_exportSpinner" runat="server" SkinID="ProgressSpinner" marker="previewSpinner"/><a class="ckbxButton silverButton statistics_ExportData" href="#" onclick="onExportClick(event);return false;" runat="server" id="_exportLink">Export</a></li>
                    <li><a class="ckbxButton redButton" protectPermission="Library.Edit" href="javascript:doDelete()" runat="server" id="_deleteLink">Delete</a></li>
                </ul>    
            </div>
            <h3 id="libraryName"></h3>
        </div>
    </div>
    <div id="libraryDescription" class="surveyDescription"></div>

    <div id="libraryMenuContainer">
        
    </div>
</div>
<div id="previewContainer" class="padding10 libraryPreviewContainer"></div> 

<script type="text/C#" runat="server">
    /// <summary>
    /// Get/set handler for showing status message.
    /// The first parameter must be a message.
    /// The second parameter must determine if an operation was succeeded or not.
    /// </summary>
    public string ShowStatusMessageHandler { get; set; }

    /// <summary>
    /// Get/set callback for handling user delete event
    /// </summary>
    public string OnLibraryDeleted { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        
        //Template Editor
        RegisterClientScriptInclude(
            GetType(),
            "templateEditor.js",
            ResolveUrl("~/Resources/templateEditor.js"));

        //Template Editor
        RegisterClientScriptInclude(
            GetType(),
            "templateHelper.js",
            ResolveUrl("~/Resources/templateHelper.js"));

        //Template Editor
        RegisterClientScriptInclude(
            GetType(),
            "htmlParser.js",
            ResolveUrl("~/Resources/htmlParser.js"));

        //Template Editor
        RegisterClientScriptInclude(
            GetType(),
            "UFrame.js",
            ResolveUrl("~/Resources/UFrame.js"));

        RegisterClientScriptInclude(
            GetType(),
            "svcAuthorization.js",
            ResolveUrl("~/Services/js/svcAuthorization.js"));

        RegisterClientScriptInclude(
            GetType(),
            "jquery.ckbxprotect.js",
            ResolveUrl("~/Resources/jquery.ckbxprotect.js"));

        RegisterClientScriptInclude(
            GetType(),
            "securityHelper.js",
            ResolveUrl("~/Resources/securityHelper.js"));
    }
</script>
