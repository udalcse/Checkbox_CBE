<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="FormPreview.ascx.cs" Inherits="CheckboxWeb.Styles.Controls.FormPreview" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>

<btn:CheckboxButton ID="update" runat="server" TextID="/common/update" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton" /><br style="clear:both;"/>
<asp:Placeholder ID="_cssContainer" runat="server" ></asp:Placeholder>
<div id="_previewContainer" style="border-style:solid;border-width:1px;border-color:Black;padding-left:15px;padding-right:15px;">
    <div style="">
      <asp:Panel id="_headerPlace" Runat="server"></asp:Panel>
      <br />
      <ckbx:MultiLanguageLabel id="surveyTitleLbl" runat="server" CssClass="title" TextId="/pageText/previewStyle.aspx/surveyTitle">Survey Title</ckbx:MultiLanguageLabel>
      <br />
      <div style="padding-top:5px;">
        <ckbx:ProgressBar ID="_progressBar" runat="server" Progress="75" />
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
    <div style="padding-left:5px">
      <br />
      <table class="Matrix" cellspacing="0" rules="all" border="1" style="WIDTH:400px;BORDER-COLLAPSE:collapse">
        <tr class="header">
          <td style="WIDTH:100px"></td>
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
          <td colspan="2" style="PADDING-BOTTOM:2px;WIDTH:100px;PADDING-TOP:2px"><ckbx:MultiLanguageLabel runat="server" ID="sportsLbl" TextId="/pageText/previewStyle.aspx/sports">Sports</ckbx:MultiLanguageLabel></td>
        </tr>
        <tr class="AlternatingItem">
          <td style="WIDTH:100px"><ckbx:MultiLanguageLabel runat="server" ID="baseballLbl"  TextId="/pageText/previewStyle.aspx/baseball">Baseball</ckbx:MultiLanguageLabel></td>
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
        <tr class="Item">
          <td style="WIDTH:100px"><ckbx:MultiLanguageLabel runat="server" ID="footballLbl"  TextId="/pageText/previewStyle.aspx/football">Football</ckbx:MultiLanguageLabel></td>
          <td align="center">
           <table id="_1015__ctl4__ctl2" border="0" style="WIDTH:100%;TEXT-ALIGN:center">
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
        <tr class="AlternatingItem">
          <td style="WIDTH:100px"><ckbx:MultiLanguageLabel runat="server" ID="soccerLbl"  TextId="/pageText/previewStyle.aspx/soccer">Soccer</ckbx:MultiLanguageLabel></td>
          <td align="center">
            <table id="_1015__ctl5__ctl2" border="0" style="WIDTH:100%;TEXT-ALIGN:center">
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
        <tr class="Item">
            <td style="WIDTH:100px"><ckbx:MultiLanguageLabel runat="server" ID="tennisLbl"  TextId="/pageText/previewStyle.aspx/tennis">Tennis</ckbx:MultiLanguageLabel></td>
            <td align="center">
                <table id="_1015__ctl6__ctl2" border="0" style="WIDTH:100%;TEXT-ALIGN:center">
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
        <tr class="AlternatingItem">
            <td style="WIDTH:100px"><ckbx:MultiLanguageLabel runat="server" ID="cricketLbl"  TextId="/pageText/previewStyle.aspx/cricket">Cricket</ckbx:MultiLanguageLabel></td>
            <td align="center">
              <table id="_1015__ctl7__ctl2" border="0" style="WIDTH:100%;TEXT-ALIGN:center">
                <tr>
                  <td><input id="_1015__ctl7__ctl2_0" type="radio" name="_1015:_ctl7:_ctl2" value="_1000_1714" /></td>
                  <td><input id="_1015__ctl7__ctl2_1" type="radio" name="_1015:_ctl7:_ctl2" value="_1000_1715" /></td>
                  <td><input id="_1015__ctl7__ctl2_2" type="radio" name="_1015:_ctl7:_ctl2" value="_1000_1716" /></td>
                  <td><input id="_1015__ctl7__ctl2_3" type="radio" name="_1015:_ctl7:_ctl2" value="_1000_1717" /></td>
                  <td><input id="_1015__ctl7__ctl2_4" type="radio" name="_1015:_ctl7:_ctl2" value="_1000_1718" /></td>
                </tr>
            </table>
          </td>
        </tr>
<%--            <tr class="subheader" IsSubheading="1">
          <td colspan="2" style="PADDING-BOTTOM:5px;WIDTH:100px;PADDING-TOP:5px"><ckbx:MultiLanguageLabel runat="server" ID="hobbiesLbl"  TextId="/pageText/previewStyle.aspx/hobbies">Hobbies</ckbx:MultiLanguageLabel></td>
        </tr>
        <tr class="AlternatingItem">
          <td style="WIDTH:100px"><ckbx:MultiLanguageLabel runat="server" ID="readingLbl"  TextId="/pageText/previewStyle.aspx/reading">Reading</ckbx:MultiLanguageLabel></td>
          <td align="center">
            <table id="_1015__ctl9__ctl2" border="0" style="WIDTH:100%;TEXT-ALIGN:center">
              <tr>
                <td><input id="_1015__ctl9__ctl2_0" type="radio" name="_1015:_ctl9:_ctl2" value="_1000_1714" /></td>
                <td><input id="_1015__ctl9__ctl2_1" type="radio" name="_1015:_ctl9:_ctl2" value="_1000_1715" /></td>
                <td><input id="_1015__ctl9__ctl2_2" type="radio" name="_1015:_ctl9:_ctl2" value="_1000_1716" /></td>
                <td><input id="_1015__ctl9__ctl2_3" type="radio" name="_1015:_ctl9:_ctl2" value="_1000_1717" /></td>
                <td><input id="_1015__ctl9__ctl2_4" type="radio" name="_1015:_ctl9:_ctl2" value="_1000_1718" /></td>
              </tr>
            </table>
          </td>
        </tr>
        <tr class="Item">
          <td style="WIDTH:100px"><ckbx:MultiLanguageLabel runat="server" ID="travelingLbl"  TextId="/pageText/previewStyle.aspx/traveling">Traveling</ckbx:MultiLanguageLabel></td>
          <td align="center">
            <table id="_1015__ctl10__ctl2" border="0" style="WIDTH:100%;TEXT-ALIGN:center">
              <tr>
                  <td><input id="_1015__ctl10__ctl2_0" type="radio" name="_1015:_ctl10:_ctl2" value="_1000_1714" /></td>
                  <td><input id="_1015__ctl10__ctl2_1" type="radio" name="_1015:_ctl10:_ctl2" value="_1000_1715" /></td>
                  <td><input id="_1015__ctl10__ctl2_2" type="radio" name="_1015:_ctl10:_ctl2" value="_1000_1716" /></td>
                  <td><input id="_1015__ctl10__ctl2_3" type="radio" name="_1015:_ctl10:_ctl2" value="_1000_1717" /></td>
                  <td><input id="_1015__ctl10__ctl2_4" type="radio" name="_1015:_ctl10:_ctl2" value="_1000_1718" /></td>
              </tr>
            </table>
          </td>
        </tr>
        <tr class="AlternatingItem">
            <td style="WIDTH:100px"><ckbx:MultiLanguageLabel runat="server" ID="fishingLbl"  TextId="/pageText/previewStyle.aspx/fishing">Fishing</ckbx:MultiLanguageLabel></td>
            <td align="center">
                <table id="_1015__ctl11__ctl2" border="0" style="WIDTH:100%;TEXT-ALIGN:center">
                    <tr>
                        <td><input id="_1015__ctl11__ctl2_0" type="radio" name="_1015:_ctl11:_ctl2" value="_1000_1714" /></td>
                        <td><input id="_1015__ctl11__ctl2_1" type="radio" name="_1015:_ctl11:_ctl2" value="_1000_1715" /></td>
                        <td><input id="_1015__ctl11__ctl2_2" type="radio" name="_1015:_ctl11:_ctl2" value="_1000_1716" /></td>
                        <td><input id="_1015__ctl11__ctl2_3" type="radio" name="_1015:_ctl11:_ctl2" value="_1000_1717" /></td>
                        <td><input id="_1015__ctl11__ctl2_4" type="radio" name="_1015:_ctl11:_ctl2" value="_1000_1718" /></td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr class="Item">
          <td style="WIDTH:100px"><ckbx:MultiLanguageLabel runat="server" ID="huntingLbl"  TextId="/pageText/previewStyle.aspx/traveling">Hunting</ckbx:MultiLanguageLabel></td>
          <td align="center">
            <table id="_1015__ctl12__ctl2" border="0" style="WIDTH:100%;TEXT-ALIGN:center">
              <tr>
                <td><input id="_1015__ctl12__ctl2_0" type="radio" name="_1015:_ctl12:_ctl2" value="_1000_1714" /></td>
                <td><input id="_1015__ctl12__ctl2_1" type="radio" name="_1015:_ctl12:_ctl2" value="_1000_1715" /></td>
                <td><input id="_1015__ctl12__ctl2_2" type="radio" name="_1015:_ctl12:_ctl2" value="_1000_1716" /></td>
                <td><input id="_1015__ctl12__ctl2_3" type="radio" name="_1015:_ctl12:_ctl2" value="_1000_1717" /></td>
                <td><input id="_1015__ctl12__ctl2_4" type="radio" name="_1015:_ctl12:_ctl2" value="_1000_1718" /></td>
              </tr>
            </table>
          </td>
        </tr>
--%>           </table>
       <div style="padding-top:15px;padding-bottom:15px;">
            <ckbx:MultiLanguageButton ID="_prevBtn" runat="server" CssClass="button" TextId="/pageText/responseTemplate.cs/backDefaultText" Enabled="false" />
            &nbsp;&nbsp;&nbsp;
            <ckbx:MultiLanguageButton ID="_saveBtn" runat="server" CssClass="button" TextId="/pageText/responseTemplate.cs/saveDefaultText" Enabled="false" />
            &nbsp;&nbsp;&nbsp;
            <ckbx:MultiLanguageButton ID="_nextBtn" runat="server" CssClass="button" TextId="/pageText/responseTemplate.cs/nextDefaultText" Enabled="false" />

       </div>
      <asp:Panel ID="_footerPlace" Runat="server"></asp:Panel>
    </div>
</div>