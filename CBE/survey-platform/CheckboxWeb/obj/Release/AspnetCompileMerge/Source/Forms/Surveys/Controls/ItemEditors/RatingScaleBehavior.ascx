<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="RatingScaleBehavior.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.RatingScaleBehavior" %>
<%@ Register src="~/Controls/Piping/PipeControl.ascx" tagname="PipeSelector" tagprefix="pipe" %>
    <script type="text/javascript">
        $(document).ready(function () {            
            $('#<%=_minValue.ClientID %>').numeric({ decimal: false, negative: false });
            $('#<%=_maxValue.ClientID %>').numeric({ decimal: false, negative: false });

            //"Allow other" change handle
            $("#<%=_allowNaCheck.ClientID %>").click(function () {
                if ($("#<%=_allowNaCheck.ClientID %>:checked").length > 0) {
                    $("#<%= _otherOptionsPanel.ClientID %> :input").removeAttr('disabled');
                }
                else {
                    $("#<%= _otherOptionsPanel.ClientID %> :input").attr('disabled', 'disabled');
                    $("#<%= _otherLabelTxt.ClientID %>").val("<%=DefaultNaText %>");
                }
            });

            $.uniform.update("#<%= _otherOptionsPanel.ClientID %> :input");
        });
    </script>

    <!-- Alias -->
    <div class="formInput">
        <div class="left fixed_150">
            <p><ckbx:MultiLanguageLabel AssociatedControlID="_aliasText" ID="_aliasLbl" runat="server" TextId="/controlText/listEditor/alias" /></p>
        </div>
        <div class="left">
            <asp:TextBox ID="_aliasText" runat="server" />
        </div>
    </div>

    <div class="formInput">
        <div class="left fixed_150">
            <p><ckbx:MultiLanguageLabel AssociatedControlID="_minValue" ID="_startValueLbl" runat="server" TextId="/controlText/ratingScaleItemEditor/startValue" /></p>
        </div>
        <div class="left">
            <asp:TextBox ID="_minValue" runat="server" Width="50px"  />
        </div>
    </div>

    <div class="formInput">
        <div class="left fixed_150">
            <p><ckbx:MultiLanguageLabel AssociatedControlID="_maxValue" ID="_endValueLbl" runat="server" TextId="/controlText/ratingScaleItemEditor/endValue" /></p>
        </div>
        <div class="left">
            <asp:TextBox ID="_maxValue" Width="50px" runat="server" />
        </div>
    </div>
    <br class="clear" />
    <div class="formInput">
        <div class="left fixed_150">
            <p><ckbx:MultiLanguageLabel AssociatedControlID="_startTextTxt" ID="_startValueLabelLbl" runat="server" TextId="/controlText/ratingScaleItemEditor/scaleStartText" /></p>
        </div>
        <div class="left">
            <asp:TextBox ID="_startTextTxt" runat="server" />
        </div>
    </div>
    <div class="formInput">
        <div class="left fixed_150">
            <p><ckbx:MultiLanguageLabel AssociatedControlID="_midTextTxt" ID="_midValueLabelLbl" runat="server" TextId="/controlText/ratingScaleItemEditor/scaleMidText" /></p>
        </div>
        <div class="left">
            <asp:TextBox ID="_midTextTxt" runat="server" /> 
        </div>
    </div>

    <div class="formInput">
        <div class="left fixed_150">
            <p><ckbx:MultiLanguageLabel AssociatedControlID="_endTextTxt" ID="_endValueLabelLbl" runat="server" TextId="/controlText/ratingScaleItemEditor/scaleEndText" />
        </div>
        <div class="left">
            <asp:TextBox ID="_endTextTxt" runat="server" />
        </div>
    </div>
    <br class="clear" />
    
    <div class="formInput checkBox">
        <p><ckbx:MultiLanguageCheckBox ID="_requiredCheck" runat="server" TextId="/controlText/ratingScaleItemEditor/required" /></p>
    </div>

    <div class="formInput checkBox">
        <p><ckbx:MultiLanguageCheckBox ID="_allowNaCheck" runat="server" TextId="/controlText/ratingScaleItemEditor/displayNotApplicableText" /></p>
    </div>

    <asp:Panel ID="_otherOptionsPanel" runat="server" Style="padding-left: 25px;" CssClass="formInput">
        <div class="left">
            <p><ckbx:MultiLanguageLabel AssociatedControlID="_otherLabelTxt" ID="_nAOptionsLbl" runat="server" TextId="/controlText/ratingScaleItemEditor/notApplicableText" /></p>
        </div>
        <div class="left" style="margin-left:15px;margin-top:3px;">
            <asp:TextBox ID="_otherLabelTxt" runat="server" />
        </div>
        <div class="left">
            <pipe:PipeSelector ID="_pipeSelectorForOther" runat="server" />
        </div>
        <br class="clear" />
    </asp:Panel>