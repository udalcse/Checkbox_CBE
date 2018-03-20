<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="UserDashboard.ascx.cs" Inherits="CheckboxWeb.Users.Controls.UserDashboard" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web" %>

<script  type="text/javascript">
	var _dashUniqueIdentifier = null;
    var _oldDashUniqueIdentifier = null; // it is needed, because user can change userName, and the necessary elements can be found only using old UserName    

    // Ensure services initialized
	$(document).ready(function () {
        //Precompile templates used
        templateHelper.loadAndCompileTemplate('<%=ResolveUrl("~/Users/jqtmpl/userDashboardTemplate.html") %>', 'userDashboardTemplate.html');
        templateHelper.loadAndCompileTemplate('<%=ResolveUrl("~/Users/jqtmpl/userListItemTemplate.html") %>', 'userListItemTemplate.html');
	});

    // Expose Method to Load user
    function loadUserData(uniqueIdentifier, reloadListData){
    
        if(uniqueIdentifier == null || uniqueIdentifier == ''){
            return;
        }
        
        _dashUniqueIdentifier = uniqueIdentifier;

        // if the username has been changed - change the appropriate elements
        if(reloadListData && (_oldDashUniqueIdentifier != _dashUniqueIdentifier)){
            $('div[id="user_'+_oldDashUniqueIdentifier+'"]').attr('id','user_'+_dashUniqueIdentifier);
            $('div[id="userContainer_'+_oldDashUniqueIdentifier+'"]').attr('id','userContainer_'+_dashUniqueIdentifier);
        }

        // Start by loading user data
        svcUserManagement.getUserData(_at, uniqueIdentifier, onUserDataLoaded, {reloadListData:reloadListData, uniqueIdentifier: uniqueIdentifier, at: _at});
    }
	
    function isJSON(str) {
        try {
            JSON.parse(str);
        } catch (e) {
            return false;
        }
        return true;
    }

	// Apply template to loaded metadata and then apply child templates
	function onUserDataLoaded(resultData, args) {
	    if (resultData == null) {
	        $('#infoPlace').html('<div class="error message" style="margin:15px;padding:5px;">Unable to load information user with id: ' + args.uniqueIdentifier + '.</div>');
	        return;
	    }

	    for (var i = 0; i < resultData.Profile.NameValueList.length; i++) {
	        var text = resultData.Profile.NameValueList[i].Value;
            
	        if (/^[\],:{}\s]*$/.test(text.replace(/\\["\\\/bfnrtu]/g, '@').
replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g, ']').
replace(/(?:^|:|,)(?:\s*\[)+/g, ''))) {
	            if(text) {
	                //resultData.Profile.NameValueList[i].Value = $.parseJSON(text); TODO: to figure out why parse json was used here 
	                if (isJSON(text)) {
	                    resultData.Profile.NameValueList[i].Value = $.parseJSON(text);
	                } else {
	                    resultData.Profile.NameValueList[i].Value = text;;
	                }


	            }
	        }
	    }

	    // Run Dashboard Template preloaded in document.ready
	    templateHelper.loadAndApplyTemplate(
	        'userDashboardTemplate.html',
	        '<%=ResolveUrl("~/Users/jqtmpl/userDashboardTemplate.html") %>',
	        resultData,
	        { appRoot: '<%=ApplicationManager.ApplicationPath %>' },
	        'infoPlace',
	        true,
	        onUserDataTemplateApplied,
	        args
	    );
        
	    $(".decodePropery").each(function() {
	        $(this).html(htmlDecode($(this).html()));
	    });
        //this is fix like in globalHelper.js file to display tables for multyline
	    fixTablePropertiesforMultyLine();

        //Show/hide Edit user links based on security
        $('#infoPlace').hide();
        securityHelper.protect(
            '<%=WebUtilities.GetCurrentUserEncodedName()%>',
            svcAuthorization.RESOURCETYPE_USER,
            resultData.UniqueIdentifier,
            '#infoPlace')
        .then(
            function(){
                $('#infoPlace').show();
            }
        );

	    // Reload User List, if necessary
	    if (args.reloadListData) {
	        //Pulsate
	        $('div[id="user_' + resultData.UniqueIdentifier + '"]').effect(
	            'shake',
	            {
	                times: 2,
	                distance: 10,
	                duration: 250
	            },
	            function() {
	                //Required to remove jagged text left behind in IE for some reason
	                // when pulsate is called.
	                if (typeof(this.style.removeAttribute) != 'undefined' && this.style.removeAttribute != null)
	                    this.style.removeAttribute('filter');

	                var index = $('div[id="userContainer_' + resultData.UniqueIdentifier + '"]').attr('index');
	                if (typeof(index) == 'undefined' || index == null)
	                    return;

	                //Apply template
	                templateHelper.loadAndApplyTemplate(
	                    'userListItemTemplate.html',
	                    '<%=ResolveUrl("~/Users/jqtmpl/userListItemTemplate.html") %>',
	                    resultData,
	                    { index: index },
	                    $('div[id="userContainer_' + resultData.UniqueIdentifier + '"]'), // id can contain a slash. jQuery selector doesn't work with such ids. So we should select the element thinking that ID is ordinary attribute.
	                    true);
                    $('div[id="userContainer_' + resultData.UniqueIdentifier + '"] input').uniform();
	            });
	    }
<% if (!HttpContext.Current.User.IsInRole("System Administrator"))
   { %>
	    $("#autoLogin").remove();
<% } %>
	    
	    if (jQuery.inArray("System Administrator", resultData.RoleMemberships) != -1) {
	        $("#lockOut").remove();
	    } else {
            $('#_lockUserSwitch').slickswitch({
                toggledOn: function() {
                    svcUserManagement.unlockUser(_at, resultData.UniqueIdentifier, function(resultData){ });
                },
                toggledOff: function() {
                    svcUserManagement.lockUser(_at, resultData.UniqueIdentifier, function(resultData){ });
                }
            });
	        
            if (resultData.LockedOut) {
                $('#_lockUserSwitch').removeAttr('checked');
                $('#_lockUserSwitch').trigger('ss-update', [true]);
            }
	    }
	    
        if ("<%=WebUtilities.GetCurrentUserEncodedName() %>" == resultData.UniqueIdentifier) {
            $("#_deleteUserLink").remove();
        }

        resizePanels();
	}
	

	// Start loading child templates
    function onUserDataTemplateApplied(args){
    }

    //Determine if the window is a confirm window
    function checkConfirmDialog(window){
        var re = new RegExp("^confirm");
        return re.test(window.get_name());
    }

    //Handle dialog close and reload user dashboard
    function onDialogClosed(arg){
        if(arg == null) {
            return;
        }
        
        var reloadListData = true;

        _oldDashUniqueIdentifier = _dashUniqueIdentifier;

        if (arg.page=="credentials"){
            _dashUniqueIdentifier=arg.newUserName;
        }

        if (arg.page=="addUser" || arg.page=="importUsers"){
            _oldDashUniqueIdentifier = _dashUniqueIdentifier = arg.newUserName;

            <%if(!String.IsNullOrEmpty(UserListUpdateHandler))
              {%>;
            <%=UserListUpdateHandler %>();
            <%
              }%>;
            reloadListData = false;
        }

        //Reload dash
        loadUserData(_dashUniqueIdentifier, reloadListData);
    }

    //Clean user dashboard
    function cleanUserDashboard(){
        $("#infoPlace").empty();//.html('');
    }
</script>

<%-- Container for Results --%>
<div id="infoPlace" style="white-space: pre-wrap">
    <div class="introPage">
       
    </div>
</div>

<script type="text/C#" runat="server">
    
    /// <summary>
    /// Get/set handler for showing status message.
    /// The first parameter must be a message.
    /// The second parameter must determine if an operation was succeeded or not.
    /// </summary>
    public string ShowStatusMessageHandler { get; set; }

    /// <summary>
    /// Get/set handler for updating the userList.
    /// </summary>
    public string UserListUpdateHandler { get; set; }
    
	/// <summary>
	/// Override OnLoad to ensure necessary scripts are loaded.
	/// </summary>
	/// <param name="e"></param>
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
        
        RegisterClientScriptInclude(
            "serviceHelper.js",
            ResolveUrl("~/Services/js/serviceHelper.js"));

		RegisterClientScriptInclude(
            "svcUserManagement.js",
			ResolveUrl("~/Services/js/svcUserManagement.js"));

		RegisterClientScriptInclude(
			"templateHelper.js",
			ResolveUrl("~/Resources/templateHelper.js"));

        RegisterClientScriptInclude(
            "dateUtils.js",
            ResolveUrl("~/Resources/dateUtils.js"));

        RegisterClientScriptInclude(
            "globalHelper.js",
            ResolveUrl("~/Resources/globalHelper.js"));

        //Moment.js: datetime utilities
        RegisterClientScriptInclude(
            "moment.js",
            ResolveUrl("~/Resources/moment.js"));

        RegisterClientScriptInclude(
            "slickswitch.js",
            ResolveUrl("~/Resources/jquery.slickswitch.js"));
	}
</script>