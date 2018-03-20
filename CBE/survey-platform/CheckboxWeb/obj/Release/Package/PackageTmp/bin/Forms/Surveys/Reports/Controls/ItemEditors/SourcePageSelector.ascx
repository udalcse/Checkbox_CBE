<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="SourcePageSelector.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors.SourcePageSelector" %>
<%@ Import Namespace="Checkbox.Globalization.Text" %>
<%@ Import Namespace="Checkbox.Web"%>
<%@ Import Namespace="Newtonsoft.Json" %>

<script type="text/javascript">
    var pageText = '<%= TextManager.GetText("/controlText/averageScoreByPageItem/pageSource", LanguageCode) %>';
    var wholeSurvey = '<%= TextManager.GetText("/controlText/averageScoreByPageItem/totalSurveySource", LanguageCode) %>';
    var <%=ID%>_pagesArray = <%= JsonConvert.SerializeObject(PageList) %>;

    function processTemplate(elem) {
        var text = elem.ID == -1 ? wholeSurvey : pageText + ' ' + elem.Position;
        return '<div class="dashStatsContent">' +
            '<div class="fixed_25 left"><input type="checkbox" pageId="'+elem.ID+'" /></div>' +
            '<div class="left fixed_225">'+ text +'</div>' +
            '<br class="clear" />' +
        '</div>';
    } 

    function updateLists() {
        $('#<%=ID %>availablePagesContainer, #<%=ID %>sourcePagesContainer').empty();
        for (var i = 0; i < <%=ID%>_pagesArray.length; i++) {
            if (<%=ID%>_pagesArray[i].IsSelected)
                $('#<%=ID %>sourcePagesContainer').append(processTemplate(<%=ID%>_pagesArray[i]));
            else
                $('#<%=ID %>availablePagesContainer').append(processTemplate(<%=ID%>_pagesArray[i]));
        }
        $('#<%=ID %>sourcePagesContainer .dashStatsContent:odd, #<%=ID %>availablePagesContainer .dashStatsContent:odd').addClass('zebra');
        $('#<%=ID %>sourcePagesContainer .dashStatsContent:even, #<%=ID %>availablePagesContainer .dashStatsContent:even').addClass('detailZebra');
        $('#<%=ID %>sourcePagesContainer input, #<%=ID %>availablePagesContainer input').uniform();
        
        var noSourcePages = $('#<%=ID %>sourcePagesContainer .dashStatsContent').length == 0;
        $('#<%=ID %>sourcePagesContainer').height(noSourcePages ? 0 : 250);
        $('#<%=ID %>noPagesDiv').toggle(noSourcePages);
        $('#<%= ID %>_removeBtn').toggle(!noSourcePages);

        var noAvailablePages = $('#<%=ID %>availablePagesContainer .dashStatsContent').length == 0;
        $('#<%=ID %>availablePagesContainer').height(noAvailablePages ? 0 : 250);
        $('#<%=ID %>noAvailablePagesDiv').toggle(noAvailablePages);
        $('#<%= ID %>_addBtn').toggle(!noAvailablePages);
    }

    function getSelected(container){
        var elements = container.find('input:checked');
        var selected = [];
        for (var i = 0; i < elements.length; i++) {
            selected.push(parseInt($(elements[i]).attr('pageId')));
        }
        return selected;
    }

    function updateSelected(container, select) {
        var elements = container.find('input:checked');
        if (elements.length == 0)
            return false;

        var selected = [];
        for (var i = 0; i < elements.length; i++) {
            selected.push(parseInt($(elements[i]).attr('pageId')));
        }
        var selectedStr = '';
        for (var i = 0; i < <%=ID%>_pagesArray.length; i++) {
            if (selected.indexOf(<%=ID%>_pagesArray[i].ID) > -1)
                <%=ID%>_pagesArray[i].IsSelected = select;
            
            //update selected ids string
            if (<%=ID%>_pagesArray[i].IsSelected) {
                if (selectedStr != '') 
                    selectedStr += ',';
                selectedStr += <%=ID%>_pagesArray[i].ID;
            }
        }
        $('#<%= _selectedPagesTxt.ClientID %>').val(selectedStr);
        updateLists();
        return false;
    }

    $(function () {
        $('#<%= ID %>_addBtn').on('click', function() {
            return updateSelected($('#<%=ID %>availablePagesContainer'), true);
        });
        $('#<%= ID %>_removeBtn').on('click', function() {
            return updateSelected($('#<%=ID %>sourcePagesContainer'), false);
        });
        updateLists();
    });
</script>

<asp:HiddenField ID="_selectedPagesTxt" runat="server" />

<div class="fixed_300 left">
    <div class="border999 shadow999 dashStatsWrapper">
        <div class="dialogSubTitle blueBackground"><%= WebTextManager.GetText("/controlText/chartEditor/sourcePages") %></div>
        
        <div id="<%=ID %>noPagesDiv" class="padding10">
            <%=WebTextManager.GetText("/controlText/chartEditor/noPages")%>
        </div>

        <div style="height:250px; overflow-y:auto;" id="<%=ID %>sourcePagesContainer" >
        </div>
    </div>
    <div class="right">
        <a id="<%= ID %>_removeBtn" href="#" class="ckbxButton roundedCorners border999 shadow999 redButton" uframeignore="true" >
            <%=TextManager.GetText("/controlText/chartEditor/removeSelectedPages", LanguageCode) %> 
        </a>
    </div>
    <br class="clear" />
</div>

<div class="fixed_300 left"  style="margin-left: 50px;">
    <div class="border999 shadow999 dashStatsWrapper">
        <div class="dialogSubTitle blueBackground"><%= WebTextManager.GetText("/controlText/chartEditor/availablePages") %></div>
    
        <div id="<%=ID %>noAvailablePagesDiv" class="padding10">
            <%=WebTextManager.GetText("/controlText/chartEditor/noPages")%>
        </div>
        
        <div style="height:250px; overflow-y:auto;" id="<%=ID %>availablePagesContainer" >
        </div>
    </div>
    <div class="right">
        <a id="<%= ID %>_addBtn" href="#" class="ckbxButton roundedCorners border999 shadow999 silverButton" uframeignore="true" >
            <%=TextManager.GetText("/controlText/chartEditor/addSelectedPages", LanguageCode) %> 
        </a>
    </div>
</div>


<br class="clear" />


