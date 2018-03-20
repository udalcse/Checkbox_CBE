<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Select1Behavior.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.Select1Behavior" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Register TagPrefix="ckbx" TagName="OtherOptionEntry" Src="OtherOptionEntry.ascx" %> 
<script type="text/javascript">
    var isOtherOptionsEnabled;
    $(document).ready(function () {
        //"Allow other" change handle
        isOtherOptionsEnabled = $("#<%=_allowOtherCheck.ClientID %>:checked").length > 0;
        $("#<%=_allowOtherCheck.ClientID %>").click(function () {
            if ($("#<%=_allowOtherCheck.ClientID %>:checked").length > 0) {
                $("#<%= _otherOptionsPanel.ClientID %> :input").removeAttr('disabled');
                isOtherOptionsEnabled = true;
            }
            else {
                $("#<%= _otherOptionsPanel.ClientID %> :input").attr('disabled', 'disabled');
                $("#<%= _otherOptionEditor.OtherLabelClientID %>").val("<%=Utilities.AdvancedHtmlEncode(DefaultOtherText) %>");
                isOtherOptionsEnabled = false;
            }
            allowOtherCheckedChanged(isOtherOptionsEnabled);

            $.uniform.update("#<%= _otherOptionsPanel.ClientID %> :input");
        });
    });
</script>


<div class="formInput left fixed_250">
    <p><ckbx:MultiLanguageLabel ID="_aliasTextlbl" AssociatedControlID="_aliasText" runat="server" TextId="/controlText/listEditor/alias" /></p>
</div>
<div class="formInput left">
    <asp:TextBox ID="_aliasText" runat="server" />
</div>
<br class="clear"/>
<div class="formInput left fixed_250">
    <p><ckbx:MultiLanguageLabel ID="_requiredChecklbl" runat="server" TextId="/controlText/selectItemEditor/answerRequired" AssociatedControlID="_requiredCheck"/></p>
</div>
<div class="formInput left">
    <asp:CheckBox runat="server" ID="_requiredCheck"/>
</div>
<br class="clear"/>

<asp:PlaceHolder ID="_randomizeOptionsPlace" runat="server">
    <div class="formInput left fixed_250">
        <p style="font-weight:bold"><ckbx:MultiLanguageLabel ID="_randomizeOptionsChecklbl" runat="server" TextId="/controlText/selectItemEditor/answerRandomize"/></p>
    </div>
    <div class="formInput left">
        <asp:Checkbox runat="server" ID="_randomizeOptionsCheck" />
    </div>
    <br class="clear"/>
</asp:PlaceHolder>

<asp:PlaceHolder ID="_allowOtherPlace" runat="server">
    <div class="formInput left fixed_250">
        <p style="font-weight:bold"><ckbx:MultiLanguageLabel ID="_allowOtherChecklbl" runat="server" TextId="/controlText/allowOtherControl/allowOther"/></p>
    </div>
    <div class="formInput left">
        <asp:CheckBox runat="server" ID="_allowOtherCheck"/>
    </div>
    <br class="clear"/>

    <asp:Panel ID="_otherOptionsPanel" runat="server">
            <ckbx:OtherOptionEntry runat="server" ID="_otherOptionEditor" RestrictHtmlOptions="False" />
    </asp:Panel>
</asp:PlaceHolder>


    

  