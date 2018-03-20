<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Grid.ascx.cs" Inherits="CheckboxWeb.Controls.Grid" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Web" %>

<script type="text/javascript">
    //Instantiate helper instance
    var gridHelper_<%=ClientID %> = new gridHelperObj();

    //These variables are needed to reload the grid
    var listTemplateName_<%=ClientID %>;
    var listItemTemplateName_<%=ClientID %>;

    var loadInProgress_<%=ClientID %> = false;
    var grid_totalItemCount = 0;

    var isInitialized_<%=ClientID %> = false;

    var compileTemplate_<%=ClientID %>;

    //Initialize grid control
    // initialSortField     -- Field to sort by
    // listTemplatePath     -- Path to list jqTemplate
    // listItemTemplatePath -- Path to list item jqTemplate
    // loadDataCallback     -- Method returning loaded data.  Should accept following arguments:
    //      pageNumber, sortField, sortAscending, loadCompleteCallback, loadCompleteCallbackArgs
    // Load method should call the loadCompleteCallback, passing data and loadCompleteCallbackArgs as arguments:
    //      loadCompleteCallback(data, args)
    
    function initializeGrid_<%=ClientID %>(initialSortField, listTemplatePath, listTemplateName, listItemTemplatePath, listItemTemplateName, loadDataCallback, emptyGridText, initialFilterField, initialPage){
     //Init grid settings
        gridHelper_<%=ClientID %>.sortField = initialSortField;
        gridHelper_<%=ClientID %>.sortAscending = true;
        gridHelper_<%=ClientID %>.currentPage = initialPage;
        gridHelper_<%=ClientID %>.filterField = initialFilterField;
        gridHelper_<%=ClientID %>.filterValue = '';
        gridHelper_<%=ClientID %>.filterKey = '<%= InitialFilterKey %>';
        gridHelper_<%=ClientID %>.isAjaxScrollingMode = <%= IsAjaxScrollModeEnabled.ToString().ToLower() %>;

        listTemplateName_<%=ClientID %> = listTemplateName;
        listItemTemplateName_<%=ClientID %> = listItemTemplateName;

        $(window).resize(function () {
            loadNextPageOnLeftPanelExtraSize_<%=ClientID %>();
        });

        <% if(IsAjaxScrollModeEnabled) { %>            
            function onAjaxScroll(elem) {
                if($(elem).scrollTop() + $(elem).innerHeight() >= $(elem)[0].scrollHeight && !loadInProgress_<%=ClientID %>) {
                    loadNextPage_<%=ClientID %>(); 
                }           
            }
            compileTemplate_<%=ClientID %> = function() {
                templateHelper.loadAndCompileTemplate(
                    '<%=ListTemplatePath %>', 
                    listTemplateName_<%=ClientID %>,
                    function(){
                        templateHelper.loadAndCompileTemplate(
                            '<%=ListItemTemplatePath %>', 
                            listItemTemplateName_<%=ClientID %>,
                            function(){
                                //Load data
                                if(<%=LoadDataCallback %> != null){
                                    <%=LoadDataCallback %>(
                                        gridHelper_<%=ClientID %>.currentPage, 
                                        gridHelper_<%=ClientID %>.sortField, 
                                        gridHelper_<%=ClientID %>.sortAscending,
                                        gridHelper_<%=ClientID %>.filterField, 
                                        gridHelper_<%=ClientID %>.filterValue,
                                        function(resultData, args){ $('#listPlace_<%=ClientID %>').empty(); onGridDataLoaded_<%=ClientID %>(resultData, args); },
                                        {listTemplateName:listTemplateName_<%=ClientID %>, loadDataCallback:<%=LoadDataCallback %>, emptyGridText:'<%=EmptyGridText %>'},
                                        gridHelper_<%=ClientID %>.filterKey
                                    );
                                }
                            }
                        );
                    }
                );
            }
        
            if ($('#listPlace_<%=ClientID %>').hasClass('overflow-y')) {
                $('#listPlace_<%=ClientID %>.overflow-y').scroll(function () { 
                    onAjaxScroll(this);       
                });
            } else {
                $('#listPlace_<%=ClientID %>').parents('.overflow-y').scroll(function () { 
                    onAjaxScroll(this);       
                });
            }

            clearAndReload_<%=ClientID %>();

        <% } else { %>      
            $('#listPlace_<%=ClientID %>').empty();

            <% if(!DelayLoad) {%>
                showLoadingPanel_<%=ClientID %>();
            <% } %>                            
            
            compileTemplate_<%=ClientID %> = function() {
                templateHelper.loadAndCompileTemplate(
                listTemplatePath, 
                listTemplateName,
                function(){
                    templateHelper.loadAndCompileTemplate(
                        listItemTemplatePath, 
                        listItemTemplateName,
                        function(){
                        <% if(!DelayLoad) {%>
                            //Load data
                            if(loadDataCallback != null){
                                loadDataCallback(
                                    gridHelper_<%=ClientID %>.currentPage, 
                                    gridHelper_<%=ClientID %>.sortField, 
                                    gridHelper_<%=ClientID %>.sortAscending,
                                    onGridDataLoaded_<%=ClientID %>,
                                    {listTemplateName:listTemplateName, loadDataCallback:loadDataCallback, emptyGridText:emptyGridText},
                                     gridHelper_<%=ClientID %>.filterValue
                                );
                            }
                        <%} %>
                        }
                    );
                });
            }

            compileTemplate_<%=ClientID %>();
        <% } %>

        $(this).on('searchExecuted', function(e, searchTerm, filterItemType, period, eventName) {
            if(filterItemType == '<%=FilterItemType %>' && gridHelper_<%=ClientID %>.filterValue != searchTerm 
                && typeof(period) == 'undefined' && typeof(eventName) == 'undefined') {
                    gridHelper_<%=ClientID %>.filterValue = searchTerm; 
                    clearAndReload_<%=ClientID %>();
            }
        });
    }

    //Show loader
    function showLoader_<%=ClientID %>() {
        loadInProgress_<%=ClientID %> = true;
        $('#listPlace_<%=ClientID %>').append(
            '<div id="loader" style="text-align:center;">' +
                '<p><%=WebTextManager.GetText("/common/loading")%></p>' +
                '<p><asp:Image ID="_progressSpinnerAjax" runat="server" SkinId="ProgressSpinner" /></p>' +
            '</div>'
        );
    }

    //Stop loader
    function removeLoader_<%=ClientID %>() {
        loadInProgress_<%=ClientID %> = false;
        $('#listPlace_<%=ClientID %> #loader').remove();
    }

    //Reload the list
    function clearAndReload_<%=ClientID %>() {
        clearGrid_<%=ClientID %>();
        showLoader_<%=ClientID %>();
        compileTemplate_<%=ClientID %>();
    }

    //Clear the list
    function clearGrid_<%=ClientID %>() {
        $('#listPlace_<%=ClientID %>').empty();
        gridHelper_<%=ClientID %>.currentPage = 1;
        //hide pagination
        $('#paginationTopContainer_<%=ClientID %>').hide();
        $('#paginationBottomContainer_<%=ClientID %>').hide();
    }

    //Load next page
    function loadNextPage_<%=ClientID %>() {
        gridHelper_<%=ClientID %>.currentPage++;
        getFromServer_<%=ClientID %>();
    }

    //Reload data in survey and folder list
    function getFromServer_<%=ClientID %>(){
        showLoader_<%=ClientID %>();

        if(<%=LoadDataCallback %> != null){
            <%=LoadDataCallback %>(
                gridHelper_<%=ClientID %>.currentPage, 
                gridHelper_<%=ClientID %>.sortField, 
                gridHelper_<%=ClientID %>.sortAscending,
                gridHelper_<%=ClientID %>.filterField, 
                gridHelper_<%=ClientID %>.filterValue,
                onNextpageDataLoaded_<%=ClientID %>,
                {listTemplateName:listTemplateName_<%=ClientID %>, loadDataCallback:<%=LoadDataCallback %>, emptyGridText:'<%=EmptyGridText %>'},
                 gridHelper_<%=ClientID %>.filterKey
            );
        }
    }

    //Show loading panel
    function showLoadingPanel_<%=ClientID %>() {

        <% if(UseSimpleLoadingSpinner) { %>
            showLoadingPanelSimple_<%=ClientID %> ();
            return;
        <% } %>
        //Get parameters for translucent overlay panel
        var panelHeight = $('#listPlace_<%=ClientID %>').height();
        var panelWidth = $('#listPlace_<%=ClientID %>').width();
        
        if(panelHeight == null || panelHeight < 150) {
            panelHeight = 150;
        }
        
        if(panelWidth == null || panelWidth < 150) {
            panelWidth = 150;
        }

        //Set height/width
        $('#detailProgressContainer_<%=ClientID %>').height(panelHeight);
        $('#detailProgressContainer_<%=ClientID %>').width(panelWidth);
        
        //$('#listPlace_<%=ClientID %>').html($('#detailProgressContainer_<%=ClientID %>').html());

        //Move to position
        var offset = $('#listPlace_<%=ClientID %>').offset();
        
        $('#detailProgressContainer_<%=ClientID %>').css(offset);
        
        $('#detailProgress_<%=ClientID %>').css({ 
                top: offset.top + (panelHeight / 2) - ($('#detailProgress_<%=ClientID %>').height() / 2), 
                left: offset.left + (panelWidth /2) - ($('#detailProgress_<%=ClientID %>').width() / 2)
        });
        
        //Show
        $('#detailProgress_<%=ClientID %>').show();
        $('#detailProgressContainer_<%=ClientID %>').show();
        $('#listPlace_<%=ClientID %>').show();
    }
    
    //Simple loading panel that replaces contents of grid instead of doing
    // calculations and translucent overlays
    function showLoadingPanelSimple_<%=ClientID %>() {
        $('#listPlace_<%=ClientID %>').html($('#detailProgressContainerSimple_<%=ClientID %>').html());
        $('#listPlace_<%=ClientID %>').show();
    }

    //Hide loading panel
    function hideLoadingPanel_<%=ClientID %>(){
        $('#detailProgress_<%=ClientID %>').hide();
        $('#detailProgressContainer_<%=ClientID %>').hide();
        $('#detailProgressContainerSimple_<%=ClientID %>').hide();
    }

    //Hide loading panel
    function disableGrid_<%=ClientID %>(){
        $('#detailProgressContainer_<%=ClientID %>').hide();
        $('#detailProgressContainerSimple_<%=ClientID %>').hide();
        $('#emptyGrid_<%=ClientID %>').hide();
        $('#listPlace_<%=ClientID %>').hide();
    }

    //Reload the grid
    function reloadGrid_<%=ClientID %>(resetPaging, sortField, sortAscending){
        $('#listPlace_<%=ClientID %>').empty();//.html('');
        //showLoadingPanel_<%=ClientID %>();
        showLoader_<%=ClientID %>();

        if(resetPaging){
            gridHelper_<%=ClientID %>.currentPage = 1;
        }

        if (sortField){
            gridHelper_<%=ClientID %>.sortField = sortField;
        }
        
        if (sortAscending || sortAscending===false){
            gridHelper_<%=ClientID %>.sortAscending = (sortAscending === true) || (sortAscending == "true");
        }

        templateHelper.loadAndCompileTemplate(
            '<%=ListTemplatePath %>', 
            listTemplateName_<%=ClientID %>,
            function() {
                templateHelper.loadAndCompileTemplate(
                    '<%=ListItemTemplatePath %>', 
                    listItemTemplateName_<%=ClientID %>,
                    function(){
                        //Load data
                        if(<%=LoadDataCallback %> != null){
                            <%=LoadDataCallback %>(
                                gridHelper_<%=ClientID %>.currentPage, 
                                gridHelper_<%=ClientID %>.sortField, 
                                gridHelper_<%=ClientID %>.sortAscending,
                                <% if(IsAjaxScrollModeEnabled) { %>
                                gridHelper_<%=ClientID %>.filterField, 
                                gridHelper_<%=ClientID %>.filterValue,
                                <% } %>
                                onGridDataLoaded_<%=ClientID %>,
                                {listTemplateName:listTemplateName_<%=ClientID %>, loadDataCallback:<%=LoadDataCallback %>, emptyGridText:'<%=EmptyGridText %>'},
                                gridHelper_<%=ClientID %>.filterKey
                            );
                        }
                    }
                );
            }
        );
    }

    //populate grid
    function onNextpageDataLoaded_<%=ClientID %>(resultData, args) {
        var dataPage = null;

        if(resultData.ResultPage){
            dataPage = resultData.ResultPage;
            grid_totalItemCount = dataPage.length;
        }

        if(grid_totalItemCount == 0){
            if(gridHelper_<%=ClientID %>.currentPage > 1 ) {
                removeLoader_<%=ClientID %>();
                loadInProgress_<%=ClientID %> = true;
                return;
            }

            $('#emptyGrid_<%=ClientID %>').html(args.emptyGridText);
            $('#listPlace_<%=ClientID %>').hide();
            $('#emptyGrid_<%=ClientID %>').show();
        }
        else{
            $('#listPlace_<%=ClientID %>').show();
            $('#emptyGrid_<%=ClientID %>').hide();
        }

        templateHelper.loadAndApplyTemplateD(
            listItemTemplateName_<%=ClientID %>,
            '<%=ListItemTemplatePath %>', 
            dataPage,
            { pageNum : gridHelper_<%=ClientID %>.currentPage },
            'listPlace_<%=ClientID %>',
            false,
            args)
			   .then(function () {
                   $('#listPlace_<%=ClientID %> input').uniform();
               })
               .then(function() {
                   removeLoader_<%=ClientID %>();
                   loadNextPageOnLeftPanelExtraSize_<%=ClientID %>();
               });
    }

    //
    function loadNextPageOnLeftPanelExtraSize_<%=ClientID %>() {
        <% if(IsAjaxScrollModeEnabled) { %>            
            if(!loadInProgress_<%=ClientID %> && $('.leftPanel .viewport').height() >= $('#listPlace_<%=ClientID %>').height()) {
                loadNextPage_<%=ClientID %>();
            }
        <% } %>
    }

    <%-- Load/populate grid --%>
    function onGridDataLoaded_<%=ClientID %>(resultData, args){
        hideLoadingPanel_<%=ClientID %>();
        
         //new parameter for grids with aggregated results
        if ((typeof(args.isAggregatedResult) != "undefined") && args.isAggregatedResult && args.isAggregatedResult != "") {
            $.each(resultData.ResultPage, function(index, value) {
                value['isAggregativeResult'] = true;
            }); 
        } else { 
            if (typeof (resultData.ResultPage)!="undefined") {
                $.each(resultData.ResultPage, function(index, value) {
                    value['isAggregativeResult'] = false;
                });
            }
        }
        
        var dataPage = null;

        if(resultData.ResultPage){
            dataPage = resultData.ResultPage;
            grid_totalItemCount = resultData.TotalItemCount;
        }

        if(resultData.length){
            dataPage = resultData;
            grid_totalItemCount = resultData.length;
        }

        if(grid_totalItemCount == 0){
            $('#emptyGrid_<%=ClientID %>').html(args.emptyGridText);
            $('#listPlace_<%=ClientID %>').hide();
           $('#emptyGrid_<%=ClientID %>').show();
        }
        else{
            $('#listPlace_<%=ClientID %>').show();
           $('#emptyGrid_<%=ClientID %>').hide();
        }
        
        var arrowStyle = '<%=ArrowStyle %>';
        var gridCss = '<%=GridCssClass %>';

        var pagerSettings = {
            totalItems: grid_totalItemCount,
            currentPage: gridHelper_<%=ClientID %>.currentPage,
            pageSize: <%= ResultsPerPage %>,
            pageChanged: function(newPage) {

                showLoadingPanel_<%=ClientID %>();
                gridHelper_<%=ClientID %>.currentPage = newPage;

                if (args.loadDataCallback != null) {
                    args.loadDataCallback(
                        gridHelper_<%=ClientID %>.currentPage,
                        gridHelper_<%=ClientID %>.sortField,
                        gridHelper_<%=ClientID %>.sortAscending,
                        onGridDataLoaded_<%=ClientID %>,
                        { loadDataCallback: args.loadDataCallback, listTemplateName: args.listTemplateName },
                        gridHelper_<%=ClientID %>.filterKey
                    );
                }
            }
        };
        
        $('#paginationTop_<%=ClientID %>').pager(pagerSettings);
        $('#paginationBottom_<%=ClientID %>').pager(pagerSettings);
        
        //Hide pager if not necessary
        if(grid_totalItemCount == 0
            || grid_totalItemCount <= <%= ResultsPerPage %> || <%= IsAjaxScrollModeEnabled.ToString().ToLower() %>){
            $('#paginationTopContainer_<%=ClientID %>').hide();
            $('#paginationBottomContainer_<%=ClientID %>').hide();
        }
        else{
            $('#paginationTopContainer_<%=ClientID %>').show();
            $('#paginationBottomContainer_<%=ClientID %>').show();
        }

        var renderCompleteCallback = null;

        <% if(Utilities.IsNotNullOrEmpty(RenderCompleteCallback)) { %>
            renderCompleteCallback = <%=RenderCompleteCallback %>;
        <% } %>
        
        //field to identify grid sorter in template
        dataPage['ClientID'] = '<%=ClientID %>';
        
        //Render grid
        gridHelper_<%=ClientID %>.renderGrid(
            dataPage, 
            'listPlace_<%=ClientID %>', 
            args.listTemplateName, 
            '<%=ListTemplatePath %>',
            {
            <% if(!string.IsNullOrEmpty(ItemClickCallback)) { %>
                itemClick: <%= ItemClickCallback %>,
            <% } %>
                sortClick: function(sortField){
                    gridHelper_<%=ClientID %>.updateSort(sortField);
                    args.loadDataCallback(
                        gridHelper_<%=ClientID %>.currentPage,
                        gridHelper_<%=ClientID %>.sortField, 
                        gridHelper_<%=ClientID %>.sortAscending,
                        onGridDataLoaded_<%=ClientID %>,
                        {loadDataCallback:args.loadDataCallback, listTemplateName: args.listTemplateName},
                        gridHelper_<%=ClientID %>.filterKey
                    );
                },
                arrowStyle:arrowStyle,
                gridCss:gridCss
            },
            renderCompleteCallback
        );
        
        removeLoader_<%=ClientID %>();
        loadNextPageOnLeftPanelExtraSize_<%=ClientID %>();
        
        $('select, input:checkbox, input:radio, input:text').filter(':not([uniformIgnore])').uniform();

        if (!isInitialized_<%=ClientID %>) {
            isInitialized_<%=ClientID %> = true;
            $('#listPlace_<%=ClientID %>').trigger('gridIsInitialized', '<%=ClientID%>');
        }

    if (jQuery().multiselect) {
        $(".invitation-email-drop-down").multiselect({
            selectedText: "# of # selected",
            minWidth : 300,
            header:false,
            noneSelectedText: 'Select emails'
        });
        //option 0 will be always default email // or checked will be first of aditional
        //as control will send multiple callbacks make sure prop is not checked
        if ($("input[id*='option-0']").prop('checked') === false)
            $("input[id*='option-0']").click();

    }

    }
    
    $(document).on('click',"#toggle_arrow_sorter_<%=ClientID %>",function(){
        if ($(this).hasClass("toggle-arrow-down")) {
            gridHelper_<%=ClientID %>.sortField = $(this).attr('sortField');
            reloadGrid_<%=ClientID %>(true, null, true);
        }
        else if ($(this).hasClass("toggle-arrow-up")) {
            gridHelper_<%=ClientID %>.sortField = $(this).attr('sortField');
            reloadGrid_<%=ClientID %>(true, null, false);
        }
    });
    
    //click on the grid text header which triggers click on arrow and sorts grid
    $(document).on('click',".header_sorter", function(){
        var child = $(this).children('.header_sorter > .gridsorter');
        $(child).trigger('click');
    });
    
    $(document).on("mouseenter", '.headerPart',
            function () {
                $(this).find('#toggle_arrow_sorter_<%=ClientID %>').show();
            });
    $(document).on("mouseleave", '.headerPart',
            function () {
                if ($(this).find('#toggle_arrow_sorter_<%=ClientID %>').attr('sortField') != gridHelper_<%=ClientID %>.sortField) {
                    $(this).find('#toggle_arrow_sorter_<%=ClientID %>').hide('fast');
                }
            });

    function showSorter_<%=ClientID %>() {
        if (gridHelper_<%=ClientID %>.sortAscending == true) {
            $("#toggle_arrow_sorter_<%=ClientID %>[sortField=" + gridHelper_<%=ClientID %>.sortField + "]").removeClass('toggle-arrow-down');
            $("#toggle_arrow_sorter_<%=ClientID %>[sortField=" + gridHelper_<%=ClientID %>.sortField + "]").addClass('toggle-arrow-up'); 
            
        } else {
            $("#toggle_arrow_sorter_<%=ClientID %>[sortField=" + gridHelper_<%=ClientID %>.sortField + "]").removeClass('toggle-arrow-up');
            $("#toggle_arrow_sorter_<%=ClientID %>[sortField=" + gridHelper_<%=ClientID %>.sortField + "]").addClass('toggle-arrow-down');
        }
        $(".gridsorter[sortField=" + gridHelper_<%=ClientID %>.sortField + "]").show();
        $(".gridsorter:not([sortField=" + gridHelper_<%=ClientID %>.sortField + "])").hide();
    }
</script>

<%-- Container for Results --%>
<div id="paginationTopContainer_<%=ClientID %>" class="<%=GridCssClass %> border999 shadow999 gridPagingContainer" style="display:none;">
    <ul id="paginationTop_<%=ClientID %>" class="pagination"></ul>
    <br class="clear"  />
</div>

<div id="listPlace_<%=ClientID %>" class="border999 shadow999 <%= GridCssClass %>" style="min-width:150px;min-height:68px;"></div>

<div id="emptyGrid_<%=ClientID %>" class="emptyGrid <%= GridCssClass %>" style="display:none;"></div>

<div id="paginationBottomContainer_<%=ClientID %>" class="<%=GridCssClass %> border999 shadow999 gridPagingContainer" style="display:none;">
    <ul id="paginationBottom_<%=ClientID %>" class="pagination"></ul>
    <br class="clear" />
</div>

<%-- Loading Div for Translucent Overlay --%>
<div id="detailProgressContainer_<%=ClientID %>" style="display:none;position:absolute;top:0;left:0;z-index:150;background-color:#333;opacity:0.5;filter:alpha(opacity=50); ">
   &nbsp;
</div>

 <div id="detailProgress_<%=ClientID %>" style="text-align:center;position:absolute;display:none;background-color:White;border: 3px double #DEDEDE;height:100px;width:200px;vertical-align:middle;z-index:151;">
    <div style="text-align:center;margin-top:25px;"><%=WebTextManager.GetText(LoadingTextId) %></div>
        <div>
            <asp:Image ID="_progressSpinner" runat="server" SkinId="ProgressSpinner" />
        </div>
    </div>

<%-- Loading Div for Simple --%>
<div id="detailProgressContainerSimple_<%=ClientID %>" style="display:none;">
    <div style="text-align:center;">
        <p><%=WebTextManager.GetText(LoadingTextId) %></p>
        <p>
            <asp:Image ID="_progressSpinnerSimple" runat="server" SkinId="ProgressSpinner" />
        </p>
    </div>
</div>

<%-- Event Handling  --%>
<script type="text/C#" runat="server">
    /// <summary>
    /// Override OnLoad to ensure necessary scripts are loaded.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        RegisterClientScriptInclude(
            "jquery.tmpl.min.js",
            ResolveUrl("~/Resources/jquery.tmpl.min.js"));

        RegisterClientScriptInclude(
            "templateHelper.js",
            ResolveUrl("~/Resources/templateHelper.js"));

        RegisterClientScriptInclude(
            "grid.js",
            ResolveUrl("~/Resources/grid.js"));

        RegisterClientScriptInclude(
            "jquery.pager.js",
            ResolveUrl("~/Resources/jquery.pager.js"));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        //Do nothing if not visible
        if (!Visible)
        {
            return;
        }
        
        
        if (string.IsNullOrEmpty(ListTemplatePath))
        {
            throw new Exception("List template path not specified for grid with ID: " + ID);
        }

        if (string.IsNullOrEmpty(ListItemTemplatePath))
        {
            throw new Exception("List item template path not specified for grid with ID: " + ID);
        }
            
        var listTemplateNames = ListTemplatePath.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
        var listItemTemplateNames = ListItemTemplatePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

        var listTemplateName = listTemplateNames.Length > 0
                                   ? listTemplateNames[listTemplateNames.Length - 1]
                                   : string.Empty;

        var listItemTemplateName = listItemTemplateNames.Length > 0
                                       ? listItemTemplateNames[listItemTemplateNames.Length - 1]
                                       : string.Empty;
            
        //Initialize grid
        Page.ClientScript.RegisterStartupScript(
            GetType(),
            "DefineInitGrid_" + ClientID,
            string.Format(
                "gridHelper.initGrid_{0} = function(){{initializeGrid_{1}('{2}','{3}', '{4}', '{5}', '{6}', {7},'{8}','{9}', {10});}};",
                ClientID,
                ClientID, 
                InitialSortField, 
                ListTemplatePath, 
                listTemplateName, 
                ListItemTemplatePath, 
                listItemTemplateName, 
                LoadDataCallback, 
                EmptyGridText,
                InitialFilterField,
                InitialPageNumber),
            true);


        //Initialize grid
        if (Visible)
        {
            Page.ClientScript.RegisterStartupScript(
                GetType(),
                "InitGrid_" + ClientID,
                string.Format("$(window).load(function(){{gridHelper.initGrid_{0}();}});", ClientID),
                true);
        }
    }
</script>
