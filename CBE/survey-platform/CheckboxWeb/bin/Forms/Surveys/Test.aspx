<%@ Page Language="C#" AutoEventWireup="False" CodeBehind="Test.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Test" MasterPageFile="~/Dialog.Master" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register Src="Controls/SurveyUrls.ascx" TagName="SurveyUrls" TagPrefix="ckbx" %>

<asp:Content ID="_headContent" ContentPlaceHolderID="_headContent" runat="server">

<script type="text/javascript" language="javascript" >
    $(document).ready(function () {
        $("input[type=text][paramtype='Query String']").each(function (i, el) {
            el.name = el.attributes["paramname"].value;
        });

        $("input[type=text]").keyup(function (e) {
            if ($(this).attr('paramtype') != "Query String") {
                $.ajax({
                    type: "POST",
                    url: '<%=ResolveUrl("~//Forms//Surveys//Test.aspx")%>/SaveParameter',
                    data: '{ ticket: "' + _at + '", type:"' + $(this).attr('paramtype') + '", id: "' + $(this).attr('iid') + '", value:"' + $(this).val() + '" }',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (msg) {
                    }
                });
            } else {
                updateUrls();
            }
        });
    });
    
    //Update test urls with input data

    function updateUrls() {
        var queryStringAddition = '';

        $("input[type=text][paramtype='Query String']").each(function() {
            if ($(this).val() != null && $(this).val() != '') {
                queryStringAddition += '&' + $(this).attr('paramname') + '=' + $(this).val();
            }
        });

        $('a.ckbxLink').each(function (index, value) {

            var originalUrl = $(this).attr('origUrl');
            var origHtml = $(this).attr('origHtml');

            if (originalUrl == null || originalUrl == '') {
                originalUrl = $(this).attr('href');
                origHtml = $(this).html();
            }

            $(this).attr('origUrl', originalUrl);
            $(this).attr('origHtml', origHtml);
            $(this).attr('href', originalUrl + queryStringAddition);
            $(this).html(origHtml + queryStringAddition);


        });
    }
    
</script>


</asp:Content>

<asp:Content ID="_pageContent" runat="server" ContentPlaceHolderID="_pageContent">
    <div class="padding10">
        <asp:Panel runat="server" ID="_hiddenItemsPlace" Visible="false">
            <div class="dialogSubTitle">
                <ckbx:MultiLanguageLabel runat="server" ID="_hiddenItemTitle" TextId="/pageText/adminTakeSurvey.aspx/hiddenItem" />
            </div>
            <div class="dialogInstructions">
                <ckbx:MultiLanguageLabel runat="server" ID="_hiddenItemConfigText" TextId="/pageText/adminTakeSurvey.aspx/hiddenItemInstructions" />
            </div>
            <div class="padding10" style="overflow-x:auto; max-height:175px; ">
                <table>
                    <asp:PlaceHolder ID="_hiddenItemRowPlace" runat="server" />
                </table>
            </div>
            <div class="dialogSubTitle" style="margin-top:15px;">
                <ckbx:MultiLanguageLabel runat="server" TextId="/pageText/adminTakeSurvey.aspx/testSurvey" />
            </div>
            
        </asp:Panel>
        <ckbx:SurveyUrls ID="_surveyUrls" runat="server" MessageTextID="/pageText/forms/surveys/test.aspx/summary" GUIDAndCustomTextID="/pageText/forms/surveys/test.aspx/guidAndCustom" GUIDOnlyTextID="/pageText/forms/surveys/test.aspx/guidOnly" ContainerCSSClass="leftContainer"></ckbx:SurveyUrls>
    </div>
</asp:Content>