<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="SliderBehavior.ascx.cs"
    Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.SliderBehavior" %>
<script type="text/javascript">
    $(document).ready(function () {
        $("#<%=_valueListOptionsPanel.ClientID %>").hide();

        $('#<%=_minValueTxt.ClientID %>').numeric({ decimal: false, negative: true });
        $('#<%=_maxValueTxt.ClientID %>').numeric({ decimal: false, negative: true });
        $('#<%=_stepSizeTxt.ClientID %>').numeric({ decimal: false, negative: false });
        $('#<%=_defaultValueTxt.ClientID %>').numeric({ decimal: false, negative: true });

        //Handle change value type event
        $(document).on('change', "#<%=_valueTypeList.ClientID %>", function () {
            _choiceTabIsOld = true;
            var selected = $("#<%=_valueTypeList.ClientID %> :selected");
            if (selected.val() != 'NumberRange') {
                if (selected.val() == 'Image') {
                    $('#<%= _optionTypeList.ClientID %> option:last').attr('selected', 'selected');
                }
                if (selected.val() == 'Text') {
                    $('#<%= _optionTypeList.ClientID %> option:first').attr('selected', 'selected');
                }
                $("#<%=_numbersOptionsPanel.ClientID %>").hide();
            } else {
                $("#<%=_numbersOptionsPanel.ClientID %>").show();
            }
        });

        $(document).on('change', "#<%=_optionTypeList.ClientID %>", function () {
            _choiceTabIsOld = true;
        });

        sliderType = $("#<%=_valueTypeList.ClientID %> :selected").val();
    });
</script>

<!-- Format -->
<div class="formInput">
    <asp:Panel ID="_errorPanel" runat="server" CssClass="error message" Visible="false">
        <asp:Label ID="_errorLbl" runat="server" />
    </asp:Panel>
    
    <div class="formInput left fixed_250">
        <p><ckbx:MultiLanguageLabel AssociatedControlID="_valueTypeList" ID="_valueTypeLbl" runat="server" TextId="/controlText/sliderEditor/valueType" /></p>
    </div>
    <div class="formInput left">
        <ckbx:MultiLanguageDropDownList ID="_valueTypeList" uframeignore="true" runat="server">
            <asp:ListItem Text="" Value="NumberRange" TextId="/controlText/sliderEditor/numberRange" />
            <asp:ListItem Text="" Value="Image" TextId="/controlText/sliderEditor/image" />
            <asp:ListItem Text="" Value="Text" TextId="/controlText/sliderEditor/text" />
        </ckbx:MultiLanguageDropDownList>
    </div>
    <br class="clear"/>

    <div class="formInput left fixed_250">
        <p><ckbx:MultiLanguageLabel AssociatedControlID="_aliasText" ID="_aliasLbl" runat="server" TextId="/controlText/listEditor/alias" /></p>
    </div>
    <div class="formInput left">
        <asp:TextBox ID="_aliasText" runat="server" />
    </div>
    <br class="clear"/>

    
    <asp:Panel ID="_numbersOptionsPanel" runat="server">
        
        <div class="formInput left fixed_250">
            <p><ckbx:MultiLanguageLabel AssociatedControlID="_minValueTxt" ID="_minValueLbl" runat="server" TextId="/controlText/sliderEditor/minValue" /></p>
        </div>
        <div class="formInput left">
             <asp:TextBox ID="_minValueTxt" runat="server" />
        </div>
        <br class="clear"/>

        <div class="formInput left fixed_250">
            <p><ckbx:MultiLanguageLabel AssociatedControlID="_maxValueTxt" ID="_maxValueLbl" runat="server" TextId="/controlText/sliderEditor/maxValue" /></p>
        </div>
        <div class="formInput left">
            <asp:TextBox ID="_maxValueTxt" runat="server" />
        </div>
        <br class="clear"/>

        <div class="formInput left fixed_250">
             <p><ckbx:MultiLanguageLabel AssociatedControlID="_stepSizeTxt" ID="_stepSizeLbl" runat="server" TextId="/controlText/sliderEditor/stepSize" /></p>
        </div>
        <div class="formInput left">
            <asp:TextBox ID="_stepSizeTxt" runat="server" />
        </div>  
        <br class="clear"/>
        <div class="formInput left fixed_250">
            <p><ckbx:MultiLanguageLabel AssociatedControlID="_defaultValueTxt" ID="_defaultValueLbl" runat="server" TextId="/controlText/sliderEditor/defaultValue" /></p>
        </div>
        <div class="formInput left">
            <asp:TextBox ID="_defaultValueTxt" runat="server" />
        </div>
        <br class="clear"/>
    </asp:Panel>

    <asp:Panel ID="_valueListOptionsPanel" CssClass="hidden" runat="server">
        
        <div class="formInput left fixed_250">
            <p><ckbx:MultiLanguageLabel AssociatedControlID="_optionTypeList" ID="_optionTypeLbl" runat="server" TextId="/controlText/sliderEditor/valueListOptionType" /></p>
        </div>
        <div class="formInput left">
            <ckbx:MultiLanguageDropDownList ID="_optionTypeList" runat="server">
                <asp:ListItem Text="" Value="Text" TextId="/controlText/sliderEditor/text" />
                <asp:ListItem Text="" Value="Image" TextId="/controlText/sliderEditor/image" />
            </ckbx:MultiLanguageDropDownList>
        </div>
        <br class="clear"/>

    </asp:Panel>
</div>
<br class="clear" />
