<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Slider.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.AppearanceEditors.Slider" %>

<script type="text/javascript">
    $(document).ready(function () {
        $('#<%=_widthTxt.ClientID %>').numeric({ decimal: false, negative: false });
        $('#<%=_heightTxt.ClientID %>').numeric({ decimal: false, negative: false });

        $('#<%=_orientationList.ClientID%>').change(onOrientationChanged);

        if (sliderType == 'NumberRange') {
            $('#<%= _numericSliderOptionsPanel.ClientID %>').show();
            $('#<%= _imageSliderOptionsPanel.ClientID %>').hide();
        } else {
            $('#<%= _numericSliderOptionsPanel.ClientID %>').hide();
            if (sliderType == 'Image') {
                $('#<%= _imageSliderOptionsPanel.ClientID %>').show();
            } else {
                $('#<%= _imageSliderOptionsPanel.ClientID %>').hide();
            }
        }
    });
    
    //
    function onOrientationChanged() {
        var width = $('#<%=_widthTxt.ClientID %>').val();
        var height = $('#<%=_heightTxt.ClientID %>').val();

        $('#<%=_widthTxt.ClientID %>').val(height);
        $('#<%=_heightTxt.ClientID %>').val(width);
    }
</script>

<div class="formInput">
    <!-- Hider orientation and height for now until we can clean up display -->
    <div style="display: none;">
        <div class="formInput left fixed_250">
            <p><ckbx:MultiLanguageLabel AssociatedControlID="_orientationList" ID="_orientationLbl" runat="server" TextId="/controlText/singleLineTextAppearanceEditor/orientation" /></p>
        </div>
        <div class="formInput left">
            <ckbx:MultiLanguageDropDownList ID="_orientationList" runat="server" uframeIgnore="true">
                <asp:ListItem Value="Horizontal" Text="Horizontal" TextId="/enum/orientation/horizontal" />
                <asp:ListItem Value="Vertical" Text="Vertical" TextId="/enum/orientation/vertical" />
            </ckbx:MultiLanguageDropDownList>
        </div>
        <br class="clear"/>
    </div>

    <div id="sliderWidthContainer">
        <div class="formInput left fixed_250">
            <p><ckbx:MultiLanguageLabel AssociatedControlID="_widthTxt" ID="_widthLbl" runat="server" TextId="/controlText/sliderEditor/width" /></p>
        </div>
        <div class="formInput left">
            <asp:TextBox ID="_widthTxt" runat="server" Width="80px" />
        </div>
        <br class="clear"/>

    </div>

    <asp:Panel id="_imageSliderOptionsPanel" runat="server">
        <div class="formInput left fixed_250">
            <p><ckbx:MultiLanguageLabel AssociatedControlID="_aliasOrientationList" ID="_aliasOrientationLbl" runat="server" TextId="/controlText/sliderEditor/aliasPosition" /></p>
        </div>
        <div class="formInput left">
            <ckbx:MultiLanguageDropDownList ID="_aliasOrientationList" runat="server" uframeIgnore="true">
                <asp:ListItem Value="DontShow" Text="Don't show" TextId="/enum/aliasPosition/dontShow" />
                <asp:ListItem Value="Top" Text="Top" TextId="/enum/aliasPosition/top" />
                <asp:ListItem Value="Bottom" Text="Bottom" TextId="/enum/aliasPosition/bottom" />
            </ckbx:MultiLanguageDropDownList>
        </div>
        <br class="clear"/>

        <div class="formInput left fixed_250">
            <p><ckbx:MultiLanguageLabel AssociatedControlID="_imageOrientationList" ID="_imageOrientationLbl" runat="server" TextId="/controlText/sliderEditor/imagePosition" /></p>
        </div>
        <div class="formInput left">
            <ckbx:MultiLanguageDropDownList ID="_imageOrientationList" runat="server" uframeIgnore="true">
                <asp:ListItem Value="Top" Text="Top" TextId="/enum/sliderImagePosition/top" />
                <asp:ListItem Value="Bottom" Text="Bottom" TextId="/enum/sliderImagePosition/bottom" />
            </ckbx:MultiLanguageDropDownList>
        </div>
        <br class="clear"/>

    </asp:Panel>

    <!-- Hider orientation and height for now until we can clean up display -->
    <div style="display: none;">
        <div id="sliderHeightContainer">
            <div class="formInput left fixed_250">
                <p><ckbx:MultiLanguageLabel AssociatedControlID="_heightTxt" ID="MultiLanguageLabel1" runat="server" TextId="/controlText/sliderEditor/height" /></p>
            </div>
            <div class="formInput left">
                <asp:TextBox ID="_heightTxt" runat="server" Width="80px" />
            </div>
            <br class="clear"/>
        </div>
    </div>
    
    <asp:Panel id="_numericSliderOptionsPanel" runat="server">
        
        <div class="formInput left fixed_250">
            <p><ckbx:MultiLanguageLabel AssociatedControlID="_showLabelCkbx" ID="_showValueLbl" runat="server" TextId="/controlText/singleLineTextAppearanceEditor/showValue" /></p>
        </div>
        <div class="formInput left">
            <asp:CheckBox ID="_showLabelCkbx" runat="server" />
        </div>
        <br class="clear"/>
    </asp:Panel>

    <br class="clear" />
</div>