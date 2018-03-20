<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="SelectManyBehavior.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.SelectManyBehavior" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Register TagPrefix="ckbx" TagName="OtherOptionEntry" Src="OtherOptionEntry.ascx" %> 
<%@ Register TagPrefix="ckbx" TagName="NonOfAboveEntry" Src="NonOfAboveEntry.ascx" %> 
<script type="text/javascript">
    var isOtherOptionsEnabled, isNonOfAboveEnabled;
    $('#required_tooltip').click(function(ev) {
        ev.preventDefault();
        ev.stopPropagation();
    });
    $(document).ready(function() {
        $('#<%= _maxToSelectTxt.ClientID %>').numeric({ decimal: false, negative: false });
        $('#<%= _minToSelectTxt.ClientID %>').numeric({ decimal: false, negative: false });

        //"Allow other" change handle
        isOtherOptionsEnabled = $("#<%= _allowOtherCheck.ClientID %>:checked").length > 0;
        $("#<%= _allowOtherCheck.ClientID %>").click(function() {
            if ($("#<%= _allowOtherCheck.ClientID %>:checked").length > 0) {
                $("#<%= _otherOptionsPanel.ClientID %> :input").removeAttr('disabled');
                $('#editOtherHtmlLink').show();
                isOtherOptionsEnabled = true;
            } else {
                $("#<%= _otherOptionsPanel.ClientID %> :input").attr('disabled', 'disabled');
                $("#<%= _otherOptionEditor.OtherLabelClientID %>").val("<%= Utilities.AdvancedHtmlEncode(DefaultOtherText) %>");
                isOtherOptionsEnabled = false;
            }
            allowOtherCheckedChanged(isOtherOptionsEnabled);

            $.uniform.update("#<%= _otherOptionsPanel.ClientID %> :input");
        });

        //"None of above" change handle
        isNonOfAboveEnabled = $("#<%= _allowNonOfAbove.ClientID %>:checked").length > 0;
        $("#<%= _allowNonOfAbove.ClientID %>").click(function () {
            if ($("#<%= _allowNonOfAbove.ClientID %>:checked").length > 0) {
                $("#<%= _nonOfAboveOptionsPanel.ClientID %> :input").removeAttr('disabled');
                $('#<%= _nonOfAboveOptionEditor.EditOptionHtmlLinkClientId %>').show();
                isNonOfAboveEnabled = true;
            } else {
                $("#<%= _nonOfAboveOptionsPanel.ClientID %> :input").attr('disabled', 'disabled');
                $("#<%= _nonOfAboveOptionEditor.OptionLabelClientID %>").val("<%= Utilities.AdvancedHtmlEncode(DefaultNoneOfAboveText) %>");
                isNonOfAboveEnabled = false;
            }
            <%= _nonOfAboveOptionEditor.AllowOptionCheckedChangedHandlerName %>(isNonOfAboveEnabled);

            $.uniform.update("#<%= _nonOfAboveOptionsPanel.ClientID %> :input");
        });
    });
</script>

<div class="formInput left fixed_250">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_aliasText" ID="MultiLanguageLabel1" runat="server" TextId="/controlText/listEditor/alias" /></p>
</div>
<div class="formInput left">
    <asp:TextBox ID="_aliasText" runat="server" />
</div>
<br class="clear"/>

<div class="formInput left fixed_250">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_minToSelectTxt" ID="_minToSelectLbl" runat="server" TextId="/controlText/selectManyEditor/minToSelect" /><span class="tooltip" title="Setting this value to 1 or more will make this item required"  style="line-height:12px !important;margin-top:-.05px;"></span></p>
</div>
<div class="formInput left">
    <asp:TextBox runat="server" ID="_minToSelectTxt" />
</div>
<br class="clear"/>

<div class="formInput left fixed_250">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_maxToSelectTxt" ID="_maxToSelectLbl" runat="server" TextId="/controlText/selectManyEditor/maxToSelect" /></p>
</div>
<div class="formInput left">
    <asp:TextBox runat="server" ID="_maxToSelectTxt" />
</div>
<br class="clear"/>

<asp:PlaceHolder ID="_randomizeOptionsPlace" runat="server">
    <div class="formInput left fixed_250">
        <p style="font-weight:bold;"><ckbx:MultiLanguageLabel AssociatedControlID="" ID="_randomizeOptionsChecklbl" runat="server" TextId="/controlText/selectItemEditor/answerRandomize"/></p>
    </div>
    <div class="formInput left">
        <asp:CheckBox runat="server" ID="_randomizeOptionsCheck" />
    </div>
    <br class="clear"/>
</asp:PlaceHolder>


<asp:PlaceHolder ID="_allowOtherPlace" runat="server">
    
    <div class="formInput left fixed_250">
        <p style="font-weight:bold;"><ckbx:MultiLanguageLabel ID="_allowOtherChecklbl" runat="server"   TextId="/controlText/allowOtherControl/allowOther" /></p>
    </div>
    <div class="formInput left">
        <asp:Checkbox ID="_allowOtherCheck" runat="server" />
    </div>
    <br class="clear" />

    <asp:Panel ID="_otherOptionsPanel" runat="server">
        <ckbx:OtherOptionEntry runat="server" ID="_otherOptionEditor" RestrictHtmlOptions="False" />
    </asp:Panel>
    <br class="clear"/>
</asp:PlaceHolder>

<asp:PlaceHolder ID="_nonOfAbovePlace" runat="server">
    
    <div class="formInput left fixed_250">
        <p style="font-weight:bold;"><ckbx:MultiLanguageLabel ID="_allowNonOfAboveLbl" runat="server"   TextId="/controlText/nonOfAboveEntry/allowNonOfAbove" /></p>
    </div>
    <div class="formInput left">
        <asp:Checkbox ID="_allowNonOfAbove" runat="server" />
    </div>
    <br class="clear" />

    <asp:Panel ID="_nonOfAboveOptionsPanel" runat="server">
        <ckbx:NonOfAboveEntry runat="server" ID="_nonOfAboveOptionEditor" RestrictHtmlOptions="False" />
    </asp:Panel>
    <br class="clear"/>
</asp:PlaceHolder>

