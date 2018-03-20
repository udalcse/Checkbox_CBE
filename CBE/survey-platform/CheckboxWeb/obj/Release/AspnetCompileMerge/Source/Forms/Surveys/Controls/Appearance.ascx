<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Appearance.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.Appearance" %>
<%@ Import Namespace="Checkbox.Web"%>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>

    <script type="text/javascript" language="javascript">
        function openPreviewWindow<%=ClientID%>Click()
        {
            var style = $('#<%=_styleList.ClientID%>').val();
            window.open('Preview.aspx?s=<%=SurveyID%>&st=' + style);
        }
    </script>

    <div class="dialogSubTitle"><%=WebTextManager.GetText("/pageText/forms/surveystyle.aspx/title") %></div>
    <div class="dialogSubContainer">
        <div class="left">
            <asp:DropDownList ID="_styleList" runat="server"  />
            <ckbx:MultiLanguageLabel ID="_noStylesLbl" runat="server" TextId="/pageText/surveyStyle.aspx/noStyleTemplates" />
        </div>
        <div class="left">
            <%if (PreviewButtonVisible)
              { %>
            <a href="javascript:return false;" onclick="openPreviewWindow<%=ClientID%>Click()" class="ckbxButton roundedCorners shadow999 border999 orangeButton"><%=WebTextManager.GetText("/pageText/surveyStyle.aspx/preview")%></a>
            <%} %>
        </div>
        <br class="clear" />
    </div>

    <!--<div class="dialogSubTitle"><%=WebTextManager.GetText("/pageText/surveyStyle.aspx/pageLayout")%></div>
    
    <div class="dialogSubContainer">
        <btn:CheckboxButton ID="_editLayoutButton" runat="server" TextID="/pageText/surveyStyle.aspx/editPageLayout" CssClass="ckbxButton roundedCorners shadow999 border999 orangeButton" OnClientClick="alert('Edit page layout form is not implemented yet');" />
    </div>-->

    <div class="dialogSubTitle"><%=WebTextManager.GetText("/pageText/surveyStyle.aspx/styleOptions")%></div>
    <div class="dialogSubContainer">
        <div class="formInput">
            <div class="left fixed_25 checkBox">
                <asp:CheckBox ID="_titleChk" runat="server" />
            </div>
            <div class="left">
                <p>
                    <ckbx:MultiLanguageLabel ID="MultiLanguageLabel1" AssociatedControlID="_titleChk" runat="server" TextId="/pageText/surveyStyle.aspx/showSurveyTitle" />
                </p>
            </div>
        </div>
        <br class="clear" />
    
        <div class="formInput">
            <div class="left fixed_25 checkBox">
                <asp:CheckBox ID="_progressBarChk" runat="server" />
            </div>
            <div class="left">
                <p>
                    <ckbx:MultiLanguageLabel ID="MultiLanguageLabel2" AssociatedControlID="_progressBarChk" runat="server" TextId="/pageText/surveyStyle.aspx/showProgressBar" />
                </p>
            </div>
        </div>
        <br class="clear" />
    
        <div class="formInput">
            <div class="left fixed_25 checkBox">
                <asp:CheckBox ID="_alertChk" runat="server" />
            </div>
            <div class="left">
                <p>
                    <ckbx:MultiLanguageLabel ID="MultiLanguageLabel3" AssociatedControlID="_alertChk" runat="server" TextId="/pageText/surveyStyle.aspx/showValidationAlert" />
                </p>
            </div>
        </div>
        <br class="clear" />
    
        <div class="formInput">
            <div class="left fixed_25 checkBox">
                <asp:CheckBox ID="_randomizeChk" runat="server" />
            </div>
            <div class="left">
                <p>
                    <ckbx:MultiLanguageLabel ID="_randomizeLbl" AssociatedControlID="_randomizeChk" runat="server" TextId="/pageText/surveyStyle.aspx/randomizeItems" />
                </p>
            </div>
        </div>
        <br class="clear" />
    
        <div class="formInput">
            <div class="left fixed_25 checkBox">
                <asp:CheckBox ID="_pageNumbersChk" runat="server" />
            </div>
            <div class="left">
                <p>
                    <ckbx:MultiLanguageLabel ID="_pageNumbersLbl" AssociatedControlID="_pageNumbersChk" runat="server" TextId="/pageText/surveyStyle.aspx/showPageNumbers" />
                </p>
            </div>
        </div>
        <br class="clear" />
    
        <div id="dynamicPageNumbers" class="formInput" style="margin-left:5px;">
            <div class="left fixed_25 checkBox">
                <asp:CheckBox ID="_dynamicPageNumbersChk" runat="server" />
            </div>
            <div class="left">
                <p>
                    <ckbx:MultiLanguageLabel ID="MultiLanguageLabel4" AssociatedControlID="_dynamicPageNumbersChk" runat="server" TextId="/pageText/surveyStyle.aspx/useDynamicPageNumbers" />
                </p>
            </div>
            <br class="clear" />
        </div>
        
    
        <div class="formInput">
            <div class="left fixed_25 checkBox">
                <asp:CheckBox ID="_itemNumberChk" runat="server" />
            </div>
            <div class="left">
                <p>
                    <ckbx:MultiLanguageLabel ID="MultiLanguageLabel5" AssociatedControlID="_itemNumberChk" runat="server" TextId="/pageText/surveyStyle.aspx/showQuestionNumbers" />
                </p>
            </div>
            <br class="clear" />
        </div>
        
    
        <div id="dynamicItemsNumbers" class="formInput" style="margin-left:5px;">
            <div class="left fixed_25 checkBox">
                <asp:CheckBox ID="_dynamicItemNumbersChk" runat="server" />
            </div>
            <div class="left">
                <p>
                    <ckbx:MultiLanguageLabel ID="MultiLanguageLabel6" AssociatedControlID="_dynamicPageNumbersChk" runat="server" TextId="/pageText/surveyStyle.aspx/useDynamicItemNumbers" />
                </p>
            </div>
        </div>
        <br class="clear" />

        <div id="asterisks" class="formInput" style="margin-left:5px;">
            <div class="left fixed_25 checkBox">
                <asp:CheckBox ID="_asterisksChk" runat="server" />
            </div>
            <div class="left">
                <p>
                    <ckbx:MultiLanguageLabel ID="MultiLanguageLabel7" AssociatedControlID="_asterisksChk" runat="server" TextId="/pageText/surveyStyle.aspx/showAsterisks" />
                </p>
            </div>
        </div>
        <br class="clear" />
    </div>
    <script type="text/javascript" language="javascript">
        $(document).ready(function () {
            if (!$('#<%=_pageNumbersChk.ClientID%>')[0].checked)
                $('#dynamicPageNumbers').hide('fast');
            if (!$('#<%=_itemNumberChk.ClientID%>')[0].checked)
                $('#dynamicItemsNumbers').hide('fast');

            $('#<%=_pageNumbersChk.ClientID%>').change(
                function (e) {
                    $('#<%=_dynamicPageNumbersChk.ClientID%>')[0].checked = false;
                    if (e.srcElement.checked)
                        $('#dynamicPageNumbers').show('fast');
                    else {
                        $('#dynamicPageNumbers').hide('fast');
                    }
                }
            );

            $('#<%=_itemNumberChk.ClientID%>').change(
                function (e) {
                $('#<%=_dynamicItemNumbersChk.ClientID%>')[0].checked = false;
                if (e.srcElement.checked)
                    $('#dynamicItemsNumbers').show('fast');
                else {
                    $('#dynamicItemsNumbers').hide('fast');
                }
            }
            );
            });
    </script>