<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="StyleDashboard.ascx.cs" Inherits="CheckboxWeb.Styles.Controls.StyleDashboard" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web" %>
<script type="text/javascript">
	var _dashStyleId = null;
    var _dashStyleType = null;
    var _reloaded = false;

    <%-- Ensure services initialized --%>
	$(document).ready(function () {
        //Precompile templates used
        templateHelper.loadAndCompileTemplate('<%=ResolveUrl("~/Styles/jqtmpl/formStyleDashboardTemplate.html") %>', 'formStyleDashboardTemplate.html');
        templateHelper.loadAndCompileTemplate('<%=ResolveUrl("~/Styles/jqtmpl/chartStyleDashboardTemplate.html") %>', 'chartStyleDashboardTemplate.html');
	});

    <%-- Expose Method to Load Style --%>
    function loadStyleData(styleId, styleType){
        if(styleId == null || styleId == '' || styleId == 0){
            return;
        }

        _dashStyleId = styleId;
        _dashStyleType = styleType;

        <%-- Start by loading style meta data --%>
        svcStyleManagement.getStyleListItem(_at, styleId, styleType, onStyleDataLoaded, { styleId: styleId, at: _at, type: styleType });
    }

	<%-- Apply template to loaded metadata and then apply child templates --%>
	function onStyleDataLoaded(resultData, args) {
		if (resultData == null) {
			$('#infoPlace').html('<div class="error message" style="margin:15px;padding:5px;">Unable to load style information for style with id: ' + args.styleId + '.</div>');
		}

        args.styleId = resultData.Id;

        if(args.type == 'form') {
		    <%-- Load/Compile/Run Dashboard Template --%>
		    templateHelper.loadAndApplyTemplate(
                'formStyleDashboardTemplate.html',
                '<%=ResolveUrl("~/Styles/jqtmpl/formStyleDashboardTemplate.html") %>', 
                resultData,
                {appRoot: '<%=ApplicationManager.ApplicationPath %>'},
                'infoPlace',
                true,
                onStyleDataTemplateLoaded,
                args);
        }
        else {
		    <%-- Load/Compile/Run Dashboard Template --%>
		    templateHelper.loadAndApplyTemplate(
                'chartStyleDashboardTemplate.html',
                '<%=ResolveUrl("~/Styles/jqtmpl/chartStyleDashboardTemplate.html") %>', 
                resultData,
                {appRoot: '<%=ApplicationManager.ApplicationPath %>'},
                'infoPlace',
                true,
                onStyleDataTemplateLoaded,
                args);
        }
	}

    <%-- Start loading child templates --%>
    function onStyleDataTemplateLoaded(args){
        //Bind events
        $('#deleteStyleLink').click(function() { 
            showConfirmDialogWithCallback(
            '<%=WebTextManager.GetText("/pageText/Styles/Manage.aspx/deleteStyleConfirm") %>', 
            onDeleteStyleConfirm,
            350,
            200,
            '<%=WebTextManager.GetText("/pageText/Styles/Manage.aspx/deleteStyle") %>');
        });

        $('#stylePreviewFrame').empty();

        if(args.type == 'form') {
            //Show survey preview
            UFrameManager.init({
                id: 'stylePreviewFrame',
                loadFrom: '<%=ResolveUrl("~/Styles/SurveyStylePreview.aspx")%>?s='+ args.styleId + '&d=' + Math.random(),
                progressTemplate: $('#styleProgressContainer').html(),
                showProgress: true
            });
        }
        else {
            //Show chart preview
            UFrameManager.init({
                id: 'stylePreviewFrame',
                loadFrom: '<%=ResolveUrl("~/Styles/ChartStylePreview.aspx")%>?s='+ args.styleId,
                progressTemplate: $('#styleProgressContainer').html(),
                showProgress: true
            });
        }
    }

    //Delete style confirm handler
    function onDeleteStyleConfirm(args){
        if(args.success){
            if (_dashStyleType=='form'){
               svcStyleManagement.deleteFormStyle(
                    _at,
                    _dashStyleId,
                    onStyleDeleted
                );
            }
            else{
               svcStyleManagement.deleteChartStyle(
                    _at,
                    _dashStyleId,
                    onStyleDeleted
                );
            }
        }
    }

    //Style deleted handler
    function onStyleDeleted(){
        <%if (!String.IsNullOrEmpty(OnStyleDeleted))
          {%>
            <%=OnStyleDeleted %>();
        <%
          }%>  

        <%if (!String.IsNullOrEmpty(ShowStatusMessageHandler))
          {%>
          var message = '<%=WebTextManager.GetText("/pageText/styles/Manage.aspx/styleDeleted") %>';
          <%=ShowStatusMessageHandler %>(message, true);
        <%
          }%> 
    }



    <%-- Handle dialog close and reload style dashboard --%>
    function onDialogClosed(arg){
        if (arg == "refresh")
        {
            <%-- //Attempt to reload style data in list --%>
            reloadStyleList(true); 
        }

        if(arg == null || arg == 'cancel') {
            return;
        }

        if (arg.callbackArgs) {
            var fn = eval(arg.functionName);
            fn(arg.callbackArgs);
        }

        if (arg.p == "properties")
        {
            <%-- //Attempt to reload style data in list --%>
            reloadStyleListData(_dashStyleId, _dashStyleType);
        }
    }

    <%-- //Reload data in style dash --%>
    function reloadStyleListData(styleId, styleType){
        //Update styleList
        if (styleId)
            svcStyleManagement.getStyleListItem(_at, styleId, styleType, onStyleListItemLoaded, {styleId: styleId, styleType: styleType});
        
        //Reload dashboard
        _reloaded = false;
        loadStyleData(styleId, styleType);
    }

    <%-- //Run template for style list item --%>
    function onStyleListItemLoaded(styleData, args){
        if(styleData == null){return;}

        <%-- Pulsate --%>
         $('#style_' + styleData.Id).effect(
            'shake', 
            {
                times:2,
                distance:10,
                duration:250
            },
            function(){
                <%-- //Required to remove jagged text left behind in IE for some reason
                     //when pulsate is called. --%>
                if (this.style.removeAttribute != null)
                    this.style.removeAttribute('filter');

                var index = $('#styleContainer_' + styleData.Id).attr('index');

                if (typeof(index)=='undefined' || index==null)
                    return;
                
                <%-- Apply template --%>
                templateHelper.loadAndApplyTemplate(
                    args.styleType + 'StyleListItemTemplate.html',
                    '<%=ResolveUrl("~/Styles/jqtmpl/")%>' + args.styleType + 'StyleListItemTemplate.html',
                    styleData,
                    {index: index},
                    'styleContainer_' + styleData.Id,
                    true);
            });
    }

    //Make dashboard empty
    function cleanStyleDashboard(){
        $('#infoPlace').empty();       
    }
</script>
<%-- Container for Results --%>
<div id="infoPlace">
    <div class="introPage">
       
    </div>
</div>

 <div id="styleProgressContainer" style="display:none;">
    <div style="text-align:center;">
        <p><%=WebTextManager.GetText("/common/loading") %></p>
        <p>
            <asp:Image ID="_progressSpinner" runat="server" SkinId="ProgressSpinner" />
        </p>
    </div>
</div>

<script type="text/C#" runat="server">
    /// <summary>
    /// Get/set callback for handling style delete event
    /// </summary>
    public string OnStyleDeleted { get; set; }

    /// <summary>
    /// Get/set handler for showing status message.
    /// The first parameter must be a message.
    /// The second parameter must determine if an operation was succeeded or not.
    /// </summary>
    public string ShowStatusMessageHandler { get; set; }

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
            "svcStyleManagement.js",
            ResolveUrl("~/Services/js/svcStyleManagement.js"));

        RegisterClientScriptInclude(
            "templateHelper.js",
            ResolveUrl("~/Resources/templateHelper.js"));

        //Helper for uframe
        RegisterClientScriptInclude(
            "htmlparser.js",
            ResolveUrl("~/Resources/htmlparser.js"));

        //Helper for loading pages into divs
        RegisterClientScriptInclude(
            "UFrame.js",
            ResolveUrl("~/Resources/UFrame.js"));
    }
</script>