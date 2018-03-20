<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="RankOrderBehavior.ascx.cs"
    Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.RankOrderBehavior" %>
<script type="text/javascript">
    var _choiceTabIsOld=false;
    var rankOrderType = '';

    $(document).ready(function () {
        $('#<%=_countOfShownOptions.ClientID %>').numeric({ decimal: true, negative: false });
        //Handle change value type event
        $(document).on('change', "#<%=_rankOrderTypeList.ClientID %>", function () {
            _choiceTabIsOld = true;
            if ($("#<%=_rankOrderTypeList.ClientID %> :selected").val() == 'TopN') {
                $("#<%=_topNOrderPanel.ClientID %>").show();
                $("#<%=_numericPanel.ClientID %>").hide();
                $("#<%=_optionTypePanel.ClientID %>").hide();
                $("#<%=_selectableOrderPanel.ClientID %>").hide();
                $("#<%=_requiredPanel.ClientID %>").show();
                $("#<%=_countOfShownOptions.ClientID %>").change();
            } else if ($("#<%=_rankOrderTypeList.ClientID %> :selected").val() == 'DragnDroppable') {
                $("#<%=_topNOrderPanel.ClientID %>").hide();
                $("#<%=_numericPanel.ClientID %>").hide();
                $("#<%=_optionTypePanel.ClientID %>").show();
                $("#<%=_selectableOrderPanel.ClientID %>").hide();
                $("#<%=_requiredPanel.ClientID %>").hide();
            } else if ($("#<%=_rankOrderTypeList.ClientID %> :selected").val() == 'SelectableDragnDrop') {
                $("#<%=_topNOrderPanel.ClientID %>").hide();
                $("#<%=_numericPanel.ClientID %>").hide();
                $("#<%=_optionTypePanel.ClientID %>").show();
                $("#<%=_selectableOrderPanel.ClientID %>").show();
                $("#<%=_requiredPanel.ClientID %>").show();
                $("#<%=_countOfSelectedOptions.ClientID %>").change();
            }
            else {
                //Numeric rank order
                $("#<%=_topNOrderPanel.ClientID %>").hide();
                $("#<%=_numericPanel.ClientID %>").show();
                $("#<%=_optionTypePanel.ClientID %>").show();
                $("#<%=_selectableOrderPanel.ClientID %>").hide();
                $("#<%=_requiredPanel.ClientID %>").show();
                $("#<%=_countOfRequiredRankOptions.ClientID %>").change();
            }
        });

        $(document).on('change', "#<%=_optionTypeList.ClientID %>", function () {
            _choiceTabIsOld = true;
        });

        $('#<%= _countOfSelectedOptions.ClientID %>, #<%= _countOfRequiredRankOptions.ClientID %>, #<%= _countOfShownOptions.ClientID %>').on('change', function () {
            var num = parseInt($(this).val());
            if (isNaN(num))
                return;
            else if (num == 0 || ($(this).attr('id') == '<%= _countOfRequiredRankOptions.ClientID %>' && num < 2)) {
                $('#<%= _requiredPanel.ClientID %>').css('opacity', '0.5').find('input').attr('checked', false).parent().removeClass('checked');
                $('#<%= _requiredPanel.ClientID %> input').prop('disabled', true);
                $('#requiredNumberValidation').show();
            } else {
                $('#<%= _requiredPanel.ClientID %>').css('opacity', '1').find('input').prop('disabled', false);                
                $('#requiredNumberValidation').hide();
            }
        }).change();

        rankOrderType = $("#<%=_optionTypeList.ClientID %> :selected").val();

    });
</script>

<div class="formInput left fixed_250">
    <p><ckbx:MultiLanguageLabel ID="_aliasLabel" runat="server" TextId="/controlText/listEditor/alias" AssociatedControlID="_aliasText" /></p>
</div>
<div class="left input">
    <asp:TextBox ID="_aliasText" runat="server" />
</div>
<br class="clear" />

<div class="formInput left fixed_250">
    <p><ckbx:MultiLanguageLabel ID="_rankOrderTypeLbl" runat="server" TextId="/itemType/rankOrderEditor/rankOrderType" AssociatedControlID="_rankOrderTypeList" /></p>
</div>
<div class="left input">
    <ckbx:MultiLanguageDropDownList ID="_rankOrderTypeList" runat="server">
        <asp:ListItem Text="" Value="SelectableDragnDrop" TextId="/itemType/rankOrderEditor/selectableDragNDrop" title="Test2" />
        <asp:ListItem Text="" Value="DragnDroppable" TextId="/itemType/rankOrderEditor/dragAndDropOrder" />
        <asp:ListItem Text="" Value="Numeric" TextId="/itemType/rankOrderEditor/numericRankOrder" />
        <asp:ListItem Text="" Value="TopN" TextId="/itemType/rankOrderEditor/topNOrder"/>
    </ckbx:MultiLanguageDropDownList>
</div>
<br class="clear" />

<asp:Panel ID="_optionTypePanel" runat="server">
    <div class="formInput left fixed_250">
        <p><ckbx:MultiLanguageLabel ID="_optionTypeLbl" runat="server" TextId="/itemType/rankOrderEditor/optionType" AssociatedControlID="_optionTypeList"/></p>
    </div>
    <div class="left input">
        <ckbx:MultiLanguageDropDownList ID="_optionTypeList" runat="server">
            <asp:ListItem Text="" Value="Text" TextId="/itemType/rankOrderEditor/text" />
            <asp:ListItem Text="" Value="Image" TextId="/itemType/rankOrderEditor/image" />        
        </ckbx:MultiLanguageDropDownList>
    </div>
    <br class="clear" />
</asp:Panel>

<asp:Panel ID="_topNOrderPanel" runat="server">
    <div class="formInput left fixed_250">
        <p><ckbx:MultiLanguageLabel ID="_countOfShownOptionsLbl" runat="server" TextId="/itemType/rankOrderEditor/countOfShownOptions" AssociatedControlID="_countOfShownOptions"/></p>
    </div>
    <div class="left input">
        <asp:TextBox ID="_countOfShownOptions" runat="server" />
    </div>
    <br class="clear" />
</asp:Panel>

<asp:Panel ID="_numericPanel" runat="server">
    <div class="formInput left fixed_250">
        <p><ckbx:MultiLanguageLabel ID="_countOfRequiredRankOptionsLbl" runat="server" TextId="/itemType/rankOrderEditor/countOfShownOptions" AssociatedControlID="_countOfRequiredRankOptions"/></p>
    </div>
    <div class="left input">
        <asp:TextBox ID="_countOfRequiredRankOptions" runat="server" />
        <span id="requiredNumberValidation" class="error" style="color:Red; display: none;"> Must be 2 or more </span>
    </div>
    <br class="clear" />
</asp:Panel>

<asp:Panel ID="_selectableOrderPanel" runat="server">
    <div class="formInput left fixed_250">
        <p><ckbx:MultiLanguageLabel ID="_countOfSelectedOptionsLbl" runat="server" TextId="/itemType/rankOrderEditor/countOfSelectedOptions" AssociatedControlID="_countOfSelectedOptions"/></p>
    </div>
    <div class="left input">
        <asp:TextBox ID="_countOfSelectedOptions" runat="server" />
    </div>
    <br class="clear" />
</asp:Panel>


<div class="formInput left fixed_250">
    <p><ckbx:MultiLanguageLabel ID="_randomizeLbl" runat="server" TextId="/itemType/rankOrderEditor/randomize" AssociatedControlID="_randomize"/></p>
</div>
<div class="left input">
    <asp:CheckBox ID="_randomize" runat="server" />
</div>
<br class="clear" />

<asp:Panel ID="_requiredPanel" runat="server">
    <div class="formInput left fixed_250">
        <p><ckbx:MultiLanguageLabel ID="_requiredLbl" runat="server" TextId="/controlText/selectItemEditor/answerRequired"  AssociatedControlID="_requiredCheck"/></p>
    </div>
    <div class="left input">
        <asp:CheckBox ID="_requiredCheck" runat="server" />
    </div>
</asp:Panel>

<br class="clear" />

