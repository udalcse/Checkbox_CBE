<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="StylePreview.ascx.cs" Inherits="CheckboxWeb.Styles.Controls.StylePreview" %>
<%@ Import Namespace="Checkbox.Web" %>

<div id="_previewContainer">
    <div class="wrapperMaster" style="margin:0;">
        <div id="headerPlace"><%= HeaderHtml %></div>
        <div class="center borderRadius surveyContentFrame" style="display: block;">
            <div>
                <ckbx:MultiLanguageLabel id="surveyTitleLbl" runat="server" CssClass="title" TextId="/pageText/previewStyle.aspx/surveyTitle">Survey Title</ckbx:MultiLanguageLabel>
                <br />
                <div style="padding-top:5px;">
                    <div class="ProgressBar">
                        <div class="ProgressBarInner" style="width:75%"></div>
                    </div>
                </div>
                <br />
                <div class="PageNumber">
                    <ckbx:MultiLanguageLabel ID="pageNumberLbl" runat="server" TextId="/pageText/previewSurvey.aspx/page"></ckbx:MultiLanguageLabel>
                    &nbsp;1&nbsp;
                    <ckbx:MultiLanguageLabel ID="pageNumberOfLbl" runat="server" TextId="/pageText/previewSurvey.aspx/of"></ckbx:MultiLanguageLabel>
                    &nbsp;4&nbsp;
                </div>
                <br />
                <ckbx:MultiLanguageLabel Runat="server" CssClass="Question" id="questionTxtLbl" TextId="/pageText/previewStyle.aspx/questionText">Question Text</ckbx:MultiLanguageLabel>
                <br />
                <ckbx:MultiLanguageLabel Runat="server" CssClass="Description" id="questionDescLbl" TextId="/pageText/previewStyle.aspx/questionDescription">Question Description</ckbx:MultiLanguageLabel>
                <br />
                <ckbx:MultiLanguageLabel Runat="server" CssClass="Answer" id="answer1Lbl" TextId="/pageText/previewStyle.aspx/answer1">Answer 1</ckbx:MultiLanguageLabel>
                <br />
                <ckbx:MultiLanguageLabel Runat="server" CssClass="Answer" id="answer2Lbl" TextId="/pageText/previewStyle.aspx/answer2">Answer 2</ckbx:MultiLanguageLabel>
                <br />
                <ckbx:MultiLanguageLabel Runat="server" CssClass="Answer" id="answer3Lbl" TextId="/pageText/previewStyle.aspx/answer3">Answer 3</ckbx:MultiLanguageLabel>
                <br />
                <ckbx:MultiLanguageLabel Runat="server" CssClass="Error" id="answer4Lbl" TextId="/pageText/previewStyle.aspx/errorMessage">Error Message</ckbx:MultiLanguageLabel>
                <br />
            </div>
            <div class="Page"></div>
            <div style="padding-left:5px">
                <br />
                <table class="Matrix" cellspacing="0" rules="all" border="1" style="width:400px;border-collapse:collapse">
                    <tr class="header">
                        <td style="width:100px"></td>
                        <td align="center" valign="bottom">
                        <div class="header">What is your favorite color?</div>
                        <table style="width:300px" align="center" border="0">
                            <tr align="center" class="Answer">
                                <td align="center" style="width:100px"><ckbx:MultiLanguageLabel ID="noIntererstLbl" runat="server" TextId="/pageText/previewStyle.aspx/noInterest">No interest</ckbx:MultiLanguageLabel></td>
                                <td align="center" style="width:50px">&nbsp;</td>
                                <td align="center" style="width:100px"><ckbx:MultiLanguageLabel ID="indifferentLbl" runat="server" TextId="/pageText/previewStyle.aspx/indifferent">Indifferent</ckbx:MultiLanguageLabel></td>
                                <td align="center" style="width:50px">&nbsp;</td>
                                <td align="center" style="width:100px"><ckbx:MultiLanguageLabel ID="greatlyInterestedLbl" runat="server" TextId="/pageText/previewStyle.aspx/greatlyInterested">Greatly Interested</ckbx:MultiLanguageLabel></td>
                            </tr>
                        </table>
                        <table width="100%">
                            <tr align="center" class="Answer">
                                <td valign="bottom" align="center" id="__SetOption" width="20%">1</td>
                                <td valign="bottom" align="center" id="Td1" width="20%">2</td>
                                <td valign="bottom" align="center" id="Td2" width="20%">3</td>
                                <td valign="bottom" align="center" id="Td3" width="20%">4</td>
                                <td valign="bottom" align="center" id="Td4" width="20%">5</td>
                            </tr>
                        </table>
                        </td>
                    </tr>
                    <tr class="subheader" IsSubheading="1">
                        <td colspan="2" style="padding-bottom:2px;width:100px;padding-top:2px"><ckbx:MultiLanguageLabel runat="server" ID="sportsLbl" TextId="/pageText/previewStyle.aspx/sports">Sports</ckbx:MultiLanguageLabel></td>
                    </tr>
                    <tr class="Item">
                        <td style="width:100px"><ckbx:MultiLanguageLabel runat="server" ID="baseballLbl"  TextId="/pageText/previewStyle.aspx/baseball">Baseball</ckbx:MultiLanguageLabel></td>
                        <td align="center">
                        <table border="0" style="WIDTH:100%;TEXT-ALIGN:center">
                            <tr>
                                <td><input id="_1015__ctl3__ctl2_0" type="radio" name="_1015:_ctl3:_ctl2" value="_1000_1714" /></td>
                                <td><input id="_1015__ctl3__ctl2_1" type="radio" name="_1015:_ctl3:_ctl2" value="_1000_1715" /></td>
                                <td><input id="_1015__ctl3__ctl2_2" type="radio" name="_1015:_ctl3:_ctl2" value="_1000_1716" /></td>
                                <td><input id="_1015__ctl3__ctl2_3" type="radio" name="_1015:_ctl3:_ctl2" value="_1000_1717" /></td>
                                <td><input id="_1015__ctl3__ctl2_4" type="radio" name="_1015:_ctl3:_ctl2" value="_1000_1718" /></td>
                            </tr>
                        </table>
                        </td>
                    </tr>
                    <tr class="AlternatingItem">
                        <td style="width:100px"><ckbx:MultiLanguageLabel runat="server" ID="footballLbl"  TextId="/pageText/previewStyle.aspx/football">Football</ckbx:MultiLanguageLabel></td>
                        <td align="center">
                        <table id="_1015__ctl4__ctl2" border="0" style="width:100%;text-align:center">
                            <tr>
                                <td><input id="_1015__ctl4__ctl2_0" type="radio" name="_1015:_ctl4:_ctl2" value="_1000_1714" /></td>
                                <td><input id="_1015__ctl4__ctl2_1" type="radio" name="_1015:_ctl4:_ctl2" value="_1000_1715" /></td>
                                <td><input id="_1015__ctl4__ctl2_2" type="radio" name="_1015:_ctl4:_ctl2" value="_1000_1716" /></td>
                                <td><input id="_1015__ctl4__ctl2_3" type="radio" name="_1015:_ctl4:_ctl2" value="_1000_1717" /></td>
                                <td><input id="_1015__ctl4__ctl2_4" type="radio" name="_1015:_ctl4:_ctl2" value="_1000_1718" /></td>
                            </tr>
                        </table>
                        </td>
                    </tr>
                    <tr class="Item">
                        <td style="width:100px"><ckbx:MultiLanguageLabel runat="server" ID="soccerLbl"  TextId="/pageText/previewStyle.aspx/soccer">Soccer</ckbx:MultiLanguageLabel></td>
                        <td align="center">
                        <table id="_1015__ctl5__ctl2" border="0" style="width:100%;text-align:center">
                            <tr>
                                <td><input id="_1015__ctl5__ctl2_0" type="radio" name="_1015:_ctl5:_ctl2" value="_1000_1714" /></td>
                                <td><input id="_1015__ctl5__ctl2_1" type="radio" name="_1015:_ctl5:_ctl2" value="_1000_1715" /></td>
                                <td><input id="_1015__ctl5__ctl2_2" type="radio" name="_1015:_ctl5:_ctl2" value="_1000_1716" /></td>
                                <td><input id="_1015__ctl5__ctl2_3" type="radio" name="_1015:_ctl5:_ctl2" value="_1000_1717" /></td>
                                <td><input id="_1015__ctl5__ctl2_4" type="radio" name="_1015:_ctl5:_ctl2" value="_1000_1718" /></td>
                            </tr>
                        </table>
                        </td>
                    </tr>
                    <tr class="AlternatingItem">
                        <td style="width:100px"><ckbx:MultiLanguageLabel runat="server" ID="tennisLbl"  TextId="/pageText/previewStyle.aspx/tennis">Tennis</ckbx:MultiLanguageLabel></td>
                        <td align="center">
                            <table id="_1015__ctl6__ctl2" border="0" style="width:100%;text-align:center">
                                <tr>
                                    <td><input id="_1015__ctl6__ctl2_0" type="radio" name="_1015:_ctl6:_ctl2" value="_1000_1714" /></td>
                                    <td><input id="_1015__ctl6__ctl2_1" type="radio" name="_1015:_ctl6:_ctl2" value="_1000_1715" /></td>
                                    <td><input id="_1015__ctl6__ctl2_2" type="radio" name="_1015:_ctl6:_ctl2" value="_1000_1716" /></td>
                                    <td><input id="_1015__ctl6__ctl2_3" type="radio" name="_1015:_ctl6:_ctl2" value="_1000_1717" /></td>
                                    <td><input id="_1015__ctl6__ctl2_4" type="radio" name="_1015:_ctl6:_ctl2" value="_1000_1718" /></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <div style="padding-top:15px;padding-bottom:15px;">
                    <input type="button" class="surveyFooterButton" value="<%=WebTextManager.GetText("/pageText/responseTemplate.cs/backDefaultText") %>" />
                    &nbsp;&nbsp;&nbsp;
                    <input type="button" class="surveyFooterButton" value="<%=WebTextManager.GetText("/pageText/responseTemplate.cs/saveDefaultText") %>" />
                    &nbsp;&nbsp;&nbsp;
                    <input type="button" class="surveyFooterButton" value="<%=WebTextManager.GetText("/pageText/responseTemplate.cs/nextDefaultText") %>" />
                </div>
            </div>
        </div>
        <div id="footerPlace"><%= FooterHtml %></div>
    </div>
</div>

<script type="text/javascript">
    $(document).ready(function () {
        //Sometimes style preview can be too large. We need to resize panels to show a tinyScrollbar.
        //This method is defined in DetailedList.master
        if (typeof (resizePanels) == 'function')
            resizePanels();
    });
</script>
