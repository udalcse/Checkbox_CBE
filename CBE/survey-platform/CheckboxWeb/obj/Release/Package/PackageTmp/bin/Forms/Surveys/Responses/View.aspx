<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="View.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Responses.View" MasterPageFile="~/Dialog.Master" %>
<%@ Import Namespace="System.ComponentModel" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Forms" %>
<%@ Import Namespace="Checkbox.Forms.Items" %>
<%@ Import Namespace="Checkbox.Globalization" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Import Namespace="Checkbox.Web.UI.Controls" %>
<%@ Import Namespace="Checkbox.Users" %>
<%@ Import Namespace="Newtonsoft.Json" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register Src="~/Forms/Surveys/Controls/TermRenderer.ascx" TagPrefix="ckbx" TagName="TermRenderer" %>

<asp:Content ID="_script" runat="server" ContentPlaceHolderID="_headContent">
<style>
    #aspnetForm {
        overflow: visible !important;
    }
</style>
 <script type="text/javascript">
     var dateFormat = '<%=GlobalizationManager.GetDateFormat().ToLowerInvariant()%>';
     var timeFormat = '<%=GlobalizationManager.GetTimeFormat().ToUpperInvariant()%>';
     window.SurveyTerms = <%= JsonConvert.SerializeObject(ResponseTemplate.SurveyTerms) %>;

     $(function () {
         $('.serverDate').each(function (idx, el) {
             var dateString = $(el).text();
             dateString = $.trim(dateString);
             if (dateString != '<%= WebTextManager.GetText("/pageText/viewResponseDetails.aspx/notAvailable")%>') {
                 var d = new Date(dateString);
                 $(el).text($.localize(d, dateFormat) + ' ' + $.localize(d, timeFormat));
             }
         });

         if (typeof (setViewResponsePagination) == 'function') {
             setViewResponsePagination();
         }
         //hide all embedded download links, they are don't work in a dialog
         $('.uploadedFileLink').hide();
     });

     $(function () {
         var matrixProps = $(".profileAnswerFormatter.Matrix");

         $.each(matrixProps, function (i, val) {
             var matrixData = "";
             try {
                 matrixData = $.parseJSON($(this).text());
             }
             catch (ex) {
                 return true;
             }
             if (!matrixData) return true;

             $(this).html('');
             var matrixTable = '';
             matrixTable += '<table class="dynamicMatrix">';
             $.each(matrixData, function (index, row) {
                 matrixTable += '<tr class="dymanicMatrixRow">';
                 $.each(row.Cells, function (i, col) {
                     var cssClass = col.IsHeader || col.IsRowHeader ? "dynamicMatrixHeader" : "";
                     matrixTable += '<td class="dynamicMatrixCol ' + cssClass + '">'
                         + "&nbsp;" + col.Data + '</td>';
                 });
                 matrixTable += '</tr>';
             });
             matrixTable += '</table>';
             $(this).append(matrixTable);
             $(this).css('display', 'block');
         });
     });
     
     function doDownload(answerId) {
         UFrameManager.prepareOuterFormSubmit();
         
         $('#aspnetForm #__EVENTTARGET').attr('value','downloadFile');
         $('#aspnetForm #__EVENTARGUMENT').attr('value', answerId);
         $('#aspnetForm').submit();
     }

     function toggleDashSection(item, id) {
         if ($('#' + id).is(':visible')) {
             $(item).removeClass('pageArrowUp');
             $(item).addClass('pageArrowDown');
             $('#' + id).hide('blind', null, 'fast');
         } else {
             $(item).removeClass('pageArrowDown');
             $(item).addClass('pageArrowUp');
             $('#' + id).show('blind', null, 'fast');
         }
     }
    </script>

    <% if (PrintMode == ExportMode.Default) { %>
    <style>
        body {
            overflow-x: visible !important;
            overflow-y: visible !important;
        }
    </style>
    <script language="javascript">
        $(document).ready(
            function () {
                window.print();
            });
    </script>
    <%}; %>
</asp:Content>

<asp:Content ID="_content" runat="server" ContentPlaceHolderID="_pageContent">
    <ckbx:TermRenderer runat="server" ID="_termRenderer"/>
    <asp:Panel ID="_responseContent" CssClass="padding10" runat="server">
        <asp:Panel ID="_editResponsePlace" runat="server">
            <a class="ckbxButton roundedCorners border999 shadow999 orangeButton" uframeignore="_true" target="_blank" href="#" id="_editLink" runat="server" visible="false"><%=WebTextManager.GetText("/controlText/forms/surveys/controls/previewControls.ascx/editResponse")%></a>
            <a style="margin-left: 5px;" class="ckbxButton silverButton" uframeignore="_true" id="_printLink" runat="server" target="_blank">Print Response</a>
        </asp:Panel>
        <asp:Panel ID="_responseInfoPanel" runat="server">
            <div class="dashStatsWrapper border999 shadow999">
                <div class="dashStatsHeader">
                    <span class="mainStats left"><%=WebTextManager.GetText("/pageText/viewResponseDetails.aspx/responseInformation")%></span>
                    <span id="surveyStatusToggle" class="pageArrowUp" onclick="toggleDashSection(this, 'responseInfoContainer');">&nbsp;</span>
                </div>
                <div id="responseInfoContainer">
                    <div class="dashStatsContent zebra">
                        <div class="fixed_75 left"><%=WebTextManager.GetText("/pageText/viewResponseDetails.aspx/timeStarted")%></div>
                        <div class="left"><b class="serverDate">
                            <%= ResponseDateCreatedOnClientSide.HasValue 
                                ?ResponseDateCreatedOnClientSide.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
                                : WebTextManager.GetText("/pageText/viewResponseDetails.aspx/notAvailable") %></b>
                        </div>
                        <br class="clear" />
                    </div>
                    <div class="dashStatsContent detailZebra">
                        <div class="fixed_75 left"><%=WebTextManager.GetText("/pageText/viewResponseDetails.aspx/timeCompleted")%></div>
                        <div class="left"><b class="serverDate">
                            <%= ResponseDateCompletedOnClientSide.HasValue
                                ? ResponseDateCompletedOnClientSide.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
                                : WebTextManager.GetText("/pageText/viewResponseDetails.aspx/notAvailable") %></b>
                        </div>
                        <br class="clear" />
                    </div>
                    <div class="dashStatsContent zebra">
                        <div class="fixed_75 left"><%=WebTextManager.GetText("/pageText/viewResponseDetails.aspx/lastEdited")%></div>
                        <div class="left"><b class="serverDate">
                            <%= ResponseDateLastModifiedOnClientSide.HasValue
                                ? ResponseDateLastModifiedOnClientSide.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
                                : WebTextManager.GetText("/pageText/viewResponseDetails.aspx/notAvailable") %></b>
                        </div>
                        <br class="clear" />
                    </div>
                    <div class="dashStatsContent detailZebra">
                        <div class="fixed_75 left"><%=WebTextManager.GetText("/pageText/viewResponseDetails.aspx/totalTime")%></div>
                        <div class="left"><b>
                            <%= CurrentResponse.DateCompleted.HasValue  && CurrentResponse.DateCreated.HasValue
                                ? CurrentResponse.DateCompleted.Value.Subtract(CurrentResponse.DateCreated.Value).ToString()
                                : string.Empty %></b>
                        </div>
                        <br class="clear" />
                    </div>
                    <div class="dashStatsContent zebra">
                        <div class="fixed_75 left"><%=WebTextManager.GetText("/pageText/viewResponseDetails.aspx/isTest")%></div>
                        <div class="left"><b>
                            <%= WebTextManager.GetText(CurrentResponse.IsTest? "/common/yes" : "/common/no")%></b>
                        </div>
                        <br class="clear" />
                    </div>
                    <% if(ApplicationManager.AppSettings.LogIpAddresses) {%>
                    <div class="dashStatsContent detailZebra">
                        <div class="fixed_75 left"><%=WebTextManager.GetText("/pageText/viewResponses.aspx/ipAddress")%>:</div>
                        <div class="left"><b>
                            <%= CurrentResponse.IPAddress %>
                            </b>
                        </div>
                        <br class="clear" />
                    </div>
                    <%} %>
                    <asp:PlaceHolder ID="_scorePlace" runat="server">
                        <div class="dashStatsContent zebra">
                            <div class="fixed_75 left"><%=WebTextManager.GetText("/pageText/viewResponseDetails.aspx/totalScore")%></div>
                            <div class="left"><b><%= CurrentResponse.TotalScore.HasValue ? CurrentResponse.TotalScore.ToString() : string.Empty %></b></div>
                            <br class="clear" />
                        </div>
                    </asp:PlaceHolder>
                </div>
            </div>
        </asp:Panel>

    <asp:Panel ID="_userInfoPanel" runat="server">
        <div class="dashStatsWrapper border999 shadow999">
            <div class="dashStatsHeader">
                <span class="mainStats left"><%=WebTextManager.GetText("/pageText/viewResponseDetails.aspx/userInformation")%></span>
                <span id="userInfoToggle" class="pageArrowUp" onclick="toggleDashSection(this, 'userInfoContainer');">&nbsp;</span>
            </div>
            <div id="userInfoContainer">
                <div class="dashStatsContentHeader">Login Info</div>
                <div class="dashStatsContent zebra">
                    <div class="fixed_75 left"><%=WebTextManager.GetText("/pageText/viewResponseDetails.aspx/userName")%>:</div>
                    <div class="left"><b><%= CurrentResponse.UniqueIdentifier %></b></div>
                    <br class="clear" />
                </div>
                <div class="dashStatsContent detailZebra">
                    <div class="fixed_75 left"><%=WebTextManager.GetText("/pageText/viewResponseDetails.aspx/invitee")%>:</div>
                    <div class="left"><b><%= CurrentResponse.Invitee %></b></div>
                    <br class="clear" />
                </div>
                <asp:PlaceHolder ID="_networkUserPlace" runat="server">
                    <div class="dashStatsContent detailZebra">
                        <div class="fixed_75 left"><%=WebTextManager.GetText("/pageText/viewResponseDetails.aspx/networkUser")%>:</div>
                        <div class="left"><b><%= CurrentResponse.NetworkUser %></b></div>
                        <br class="clear" />
                    </div>
                </asp:PlaceHolder>
            
                <!--
                <ul class="dashStatsContent allMenu">
                    <li class="fixed_75 left"><%=WebTextManager.GetText("/pageText/viewResponseDetails.aspx/invitee")%></li>
                    <li></li>
                    <br class="clear" />
                </ul> -->

                <asp:PlaceHolder ID="_userPropertiesPlace" runat="server">
                    <asp:Repeater ID="_userPropertiesRepeater" runat="server">
                        <HeaderTemplate>
                            <div class="dashStatsContentHeader">User Profile</div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <div class="dashStatsContent zebra">
                                <div class="fixed_100 left"><%# DataBinder.Eval(Container.DataItem, "Name") %>:</div>
                                <div class="left profileAnswerFormatter <%# DataBinder.Eval(Container.DataItem, "FieldType").ToString()%>"><b><%#Server.HtmlDecode(DataBinder.Eval(Container.DataItem, "Value").ToString())%></b></div>
                                <br class="clear" />
                            </div>
                        </ItemTemplate>
                        <AlternatingItemTemplate>
                            <div class="dashStatsContent detailZebra">
                                <div class="fixed_100 left"><%# DataBinder.Eval(Container.DataItem, "Name") %>:</div>
                                <div class="left profileAnswerFormatter <%# DataBinder.Eval(Container.DataItem, "FieldType").ToString()%>"><b><%#Server.HtmlDecode(DataBinder.Eval(Container.DataItem, "Value").ToString())%></b></div>
                                <br class="clear" />
                            </div>
                        </AlternatingItemTemplate>
                    </asp:Repeater>
                </asp:PlaceHolder>
            </div>
        </div>
    </asp:Panel>
  
        <div class="dashStatsWrapper border999 shadow999">
            <div class="dashStatsHeader">
                <span class="mainStats left"><%=WebTextManager.GetText("/pageText/viewResponseDetails.aspx/responseDetails")%></span>
            </div>

            <asp:Repeater ID="_pageRepeater" runat="server" OnItemCreated="pageRepeaterItemCreated">
                <ItemTemplate>
                    <div class="viewResponsePageHeader" style="font-size:16px;"><%#GetPageTitle(Container.DataItem as ResponsePage) %></div>

                    <asp:Repeater ID="_itemRepeater" runat="server" OnItemCreated="itemRepeaterItemCreated">
                        <HeaderTemplate>
                            <table>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr style="page-break-inside: avoid">
                                <td class="viewResponseTD">
                                    <div class="viewResponseQuestion fixed_25"><p><strong><%# GetItemNumber(Container.DataItem as Item) %> - </strong></p></div>
                                </td>
                                <td class="viewResponseTD">
                                    <div class="left"><%# GetItemHtml(Container.DataItem as Item).Replace("><&nbsp;<", ">&lt;<") %><asp:Panel ID="_uploadedItemsPlace" runat="server"></asp:Panel></div>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </asp:Panel>

    <script type="text/javascript">
        if(typeof(resizePanels) == 'function') {
            resizePanels();
        }

        $(document).ready(function() {
            fixTablePropertiesforMultyLine();
        });
    </script>
</asp:Content>

<script type="text/C#" runat="server">
  

</script>