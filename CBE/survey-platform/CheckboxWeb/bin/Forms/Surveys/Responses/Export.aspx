<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.master" AutoEventWireup="false" CodeBehind="Export.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Responses.Export" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web"%>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="head" ContentPlaceHolderID="_headContent" runat="server">
    <script type="text/javascript">
        function setDatePickerOptions(input) {
            input.datepicker("option", "showOn", "both");
            input.datepicker("option", "buttonImageOnly", true);
            input.datepicker("option", "buttonImage", '<%=ResolveUrl("~/Resources/CalendarPopup.png") %>');
            input.datepicker("option", "buttonText", "Calendar");
            input.datepicker("option", "dateFormat", '<%= System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern.ToLower().Replace("yyyy", "yy") %>');
        };

        function setPossibleScoreWarningVisibility() {
            var possibleScore = $('[possiblescore="true"]');
            var detailedScore = $('[detailedscore="true"]');

            if (possibleScore.find(':checked').length > 0 && detailedScore.find(':checked').length > 0)
                $('#possibleScoreWarning').show();
            else                
                $('#possibleScoreWarning').hide();
        }

        $(document).ready(function () {
            $('.datepicker').attr('title', '<%=WebTextManager.GetText("/pageText/forms/surveys/responses/export.aspx/optional")%>');
            $('.datepicker').datepicker();
            setDatePickerOptions($('.datepicker'));
            $('.datepicker').watermark();

            //hide possible score
            var possibleScore = $('[possiblescore="true"]');
            var detailedScore = $('[detailedscore="true"]');
            <% if (ResponseTemplate.BehaviorSettings.EnableScoring && ApplicationManager.AppSettings.CsvExportIncludeDetailedScoreInfo) {%>
                possibleScore.show();
            <% } %>
            detailedScore.on('change', function() {
                possibleScore.toggle();
                setPossibleScoreWarningVisibility();
            });
            possibleScore.on('change', function () {
                setPossibleScoreWarningVisibility();
            });

            setPossibleScoreWarningVisibility();
        });
    </script>
</asp:Content>


<asp:Content ID="content" ContentPlaceHolderID="_pageContent" runat="server">
    <div class="padding10">
        <div style="padding-left:10px;">
        <div class="dialogSubTitle">
            <%= WebTextManager.GetText("/pageText/forms/surveys/responses/export.aspx/options")%>
        </div>
            </div>
        <div class="dialogSubContainer">
            <div class="formInput condensed">
                <div class="left fixed_100" >
                    <p><ckbx:MultiLanguageLabel ID="MultiLanguageLabel2" runat="server" AssociatedControlID="_exportMode"  TextId="/pageText/forms/surveys/responses/export.aspx/exportMode" /></p>
                </div>
                <div class="left">
                    <ckbx:MultiLanguageDropDownList ID="_exportMode" runat="server" AutoPostBack="true" />
                </div>
            </div>
           <div style="clear:both" />
             <div class="formInput condensed" style="margin-left:-1px;">
                <div class="left fixed_100">
                    <p><ckbx:MultiLanguageLabel ID="MultiLanguageLabel3" runat="server" CssClass="field_100" TextId="/pageText/forms/surveys/responses/export.aspx/dateStart" AssociatedControlID="_startDatePicker" /></p>
                </div>
                <div class="left">
                    <asp:TextBox ID="_startDatePicker" runat="server" Width="97px" CssClass="datepicker" />
                </div>
             </div>
            <div style="clear:both" />
             <div class="formInput condensed">
                <div class="left fixed_100">
                    <p><ckbx:MultiLanguageLabel ID="MultiLanguageLabel1" runat="server" CssClass="field_100" TextId="/pageText/forms/surveys/responses/export.aspx/dateEnd" AssociatedControlID="_endDatePicker" /></p>
                </div>
                <div class="left">
                    <asp:TextBox ID="_endDatePicker" runat="server" Width="97px" CssClass="datepicker" />
                </div>
             </div>
            <div style="clear:both" />
        </div>
        <div style="clear:both" />
             
    <asp:Panel ID="_csvOptionsPanel" runat="server">
        <div class="dialogSubTitle">
            <%= WebTextManager.GetText("/pageText/forms/surveys/responses/export.aspx/csvOptions")%>
        </div>
        <div class="dialogSubContainer" style="padding-left:0px;">
            <div class="formInput condensed">
                <ckbx:MultiLanguageCheckBoxList ID="_csvOptionsList" runat="server" RepeatColumns="2" RepeatDirection="Vertical">
                    <asp:ListItem TextId="/pageText/forms/surveys/responses/export.aspx/detailedResponseInfo" Value="DETAILED_RESPONSE_INFO" />
                    <asp:ListItem TextId="/pageText/forms/surveys/responses/export.aspx/detailedUserInfo" Value="DETAILED_USER_INFO" />
                    <asp:ListItem TextId="/pageText/forms/surveys/responses/export.aspx/mergeCheckboxResults" Value="MERGE_CHECKBOX_RESULTS" />
                    <asp:ListItem TextId="/pageText/forms/surveys/responses/export.aspx/exportOpenEndedResults" Value="EXPORT_OPEN_ENDED_RESULTS" />
                    <asp:ListItem TextId="/pageText/forms/surveys/responses/export.aspx/exportWithAliases" Value="EXPORT_WITH_ALIASES" />
                    <asp:ListItem TextId="/pageText/forms/surveys/responses/export.aspx/exportHiddenItems" Value="EXPORT_HIDDEN_ITEMS" />
                    <asp:ListItem TextId="/pageText/forms/surveys/responses/export.aspx/exportIncompleteResponses" Value="EXPORT_INCOMPLETE_RESPONSES" />
                    <asp:ListItem TextId="/pageText/forms/surveys/responses/export.aspx/stripHtmlTagsInCsvExport" Value="STRIP_HTML_TAGS" />
                    <asp:ListItem TextId="/pageText/forms/surveys/responses/export.aspx/exportRankOrderPoints" Value="RANK_ORDER_POINTS" />
                    <asp:ListItem TextId="/pageText/forms/surveys/responses/export.aspx/exportTestResponses" Value="TEST_RESPONSES" />
                    <asp:ListItem TextId="/pageText/forms/surveys/responses/export.aspx/detailedScoreData" Value="DETAILED_SCORE_INFO" />
                    <asp:ListItem TextId="/pageText/forms/surveys/responses/export.aspx/possibleScore" Value="POSSIBLE_SCORE" />
                </ckbx:MultiLanguageCheckBoxList>
            </div>
        </div>
        <div style="clear:both" />
        
        <div id="possibleScoreWarning" class="warning message hidden">
            <%= WebTextManager.GetText("/pageText/forms/surveys/responses/export.aspx/possibleScoreWarning") %>
        </div>
    </asp:Panel>
    
    <asp:Panel ID="_spssOptionsPanel" runat="server">
        <div class="dialogSubTitle">
            <%= GetSpssOptionsPanelText( _exportMode.SelectedValue)%>
        </div>
        <div class="dialogSubContainer" style="margin-left:-10px;">
            <asp:PlaceHolder ID="_responseIdPlace" runat="server">
                <div class="formInput condensed">
                    <div class="left fixed_20 checkBox"><asp:Checkbox ID="_spssExportResponseId" runat="server" /></div>
                    <div class="left"><p><ckbx:MultiLanguageLabel runat="server" TextId="/pageText/forms/surveys/responses/export.aspx/useResponseID" AssociatedControlID="_spssExportResponseId" /></p></div>
                </div>
                <div style="clear:both" />
            </asp:PlaceHolder>

            <div class="formInput condensed">
                <div class="left fixed_20 checkBox"><asp:Checkbox ID="_spssExportIncompleteResponses" runat="server" /></div>
                <div class="left"><p><ckbx:MultiLanguageLabel ID="MultiLanguageLabel4" runat="server" TextId="/pageText/forms/surveys/responses/export.aspx/exportIncompleteResponses" AssociatedControlID="_spssExportIncompleteResponses" /></p></div>
            </div>
            <div style="clear:both" />
            <div class="formInput condensed">
                <div class="left fixed_20 checkBox"><asp:Checkbox ID="_spssExportOpenEndedResponses" runat="server" /></div>
                <div class="left"><p><ckbx:MultiLanguageLabel ID="MultiLanguageLabel5" runat="server" TextId="/pageText/forms/surveys/responses/export.aspx/exportOpenEndedResults" AssociatedControlID="_spssExportOpenEndedResponses" /></p></div>
            </div>
            <div style="clear:both" />
            <div style="margin-top:15px;">
                <ckbx:MultiLanguageHyperLink ID="_spssKeyLink" runat="server" Target="_blank" NavigateUrl="SPSSKey.aspx" TextId="/pageText/forms/surveys/responses/export.aspx/viewSpssKey" Style="text-decoration: none;font-weight:bold;color:Black;" />
                <ckbx:MultiLanguageImage ID="spssNewWindow" runat="server" ImageUrl="~/App_Themes/CheckboxTheme/Images/newWindow.gif" ToolTipTextId="/common/newWindow" />
            </div>
            <div style="clear:both" />
            <div style="clear:both" />    
        </div>
    </asp:Panel>

    <asp:Panel ID="_fileDownloadPanel" runat="server" Visible="false">
        <div class="dashStatsWrapper">
            <ul class="dialogSubTitle">
                <li class="mainStats"><%= WebTextManager.GetText("/pageText/forms/surveys/responses/export.aspx/uploadedFiles")%></li>
                <div style="clear:both" />
                <div style="clear:both" />
            </ul>
            <div style="padding-top:5px;">
                <btn:CheckboxButton ID="_dlFilesBtn" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 silverButton" TextId="/pageText/forms/surveys/responses/export.aspx/downloadFiles"/>
            </div>
        </div>
    </asp:Panel>


        <div class="dialogSubTitle">
            <%= WebTextManager.GetText("/pageText/forms/surveys/responses/export.aspx/outputEncoding")%>
        </div>
        <div class="dialogSubContainer" style="padding-left:0px;">
            <div class="formInput condensed radioList">
                <ckbx:MultiLanguageRadioButtonList ID="_outputEncoding" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem TextId="/pageText/forms/surveys/responses/export.aspx/ascii" Value="ASCII" />
                    <asp:ListItem TextId="/pageText/forms/surveys/responses/export.aspx/utf8" Value="UTF8" />
                </ckbx:MultiLanguageRadioButtonList>
            </div>
        </div>
       <div style="clear:both" />
        <div style="float:left;">
            <btn:CheckboxButton ID="_exportBtn" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 silverButton statistics_ExportData" TextId="/pageText/forms/surveys/responses/export.aspx/export"/>
        </div>
        <div style="clear:both" />
    </div>
</asp:Content>
