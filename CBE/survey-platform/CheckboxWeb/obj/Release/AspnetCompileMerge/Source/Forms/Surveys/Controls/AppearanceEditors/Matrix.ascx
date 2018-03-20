<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Matrix.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.AppearanceEditors.Matrix" %>

<div class="formInput">
    <div class="formInput left fixed_250">
        <p><ckbx:MultiLanguageLabel AssociatedControlID="_gridLinesList" ID="_gridLinesLbl" runat="server" TextId="/controlText/matrixItemAppearanceEditor/gridLines" /></p>
    </div>
    <div class="formInput left">
        <ckbx:MultiLanguageDropDownList ID="_gridLinesList" runat="server" uframeignore="true">
            <asp:ListItem Value="None" Text="None" TextId="/controlText/matrixItemAppearanceEditor/none" />
            <asp:ListItem Value="Both" Text="Both" TextId="/controlText/matrixItemAppearanceEditor/both" />
            <asp:ListItem Value="Vertical" Text="Vertical" TextId="/controlText/matrixItemAppearanceEditor/vertical" />
            <asp:ListItem Value="Horizontal" Text="Horizontal" TextId="/controlText/matrixItemAppearanceEditor/horizontal" />
        </ckbx:MultiLanguageDropDownList>
    </div>
    <br class="clear"/>

    <div class="formInput left fixed_250">
        <p><ckbx:MultiLanguageLabel AssociatedControlID="_widthTxt" ID="_widthLbl" runat="server" TextId="/controlText/matrixItemAppearanceEditor/width" /></p>
    </div>
    <div class="formInput left">
        <asp:TextBox ID="_widthTxt" runat="server" /><label for="<%=_widthTxt.ClientID %>" style="font-size:15px;">px</label>
    </div>
    <br class="clear"/>

    <div class="formInput left fixed_250">
        <p><ckbx:MultiLanguageLabel AssociatedControlID="_itemPositionList" ID="_itemPositionLbl" runat="server" TextId="/controlText/matrixItemAppearanceEditor/itemPosition" /></p>
    </div>
    <div class="formInput left">
        <ckbx:MultiLanguageDropDownList ID="_itemPositionList" runat="server" uframeignore="true">
            <asp:ListItem Value="Left" Text="Left" TextId="/controlText/matrixItemAppearanceEditor/left" />
            <asp:ListItem Value="Center" Text="Center" TextId="/controlText/matrixItemAppearanceEditor/center" />
            <asp:ListItem Value="Right" Text="Right" TextId="/controlText/matrixItemAppearanceEditor/right" />
        </ckbx:MultiLanguageDropDownList>
    </div>
    <br class="clear"/>

    <div class="formInput left fixed_250">
        <p><ckbx:MultiLanguageLabel AssociatedControlID="_labelPositionList" ID="_labelPositionLbl" runat="server" TextId="/controlText/matrixItemAppearanceEditor/labelPosition" /></p>
    </div>
    <div class="formInput left">
        <ckbx:MultiLanguageDropDownList ID="_labelPositionList" runat="server" uframeignore="true">
            <asp:ListItem Value="Top" Text="Top" TextId="/enum/labelPosition/top" />
            <asp:ListItem Value="Left" Text="Left" TextId="/enum/labelPosition/left" />
            <asp:ListItem Value="Bottom" Text="Bottom" TextId="/enum/labelPosition/bottom" />
            <asp:ListItem Value="Right" Text="Right" TextId="/enum/labelPosition/right" />
        </ckbx:MultiLanguageDropDownList>
    </div>
    <br class="clear"/>

    <div class="formInput left fixed_250">
        <p><ckbx:MultiLanguageLabel AssociatedControlID="_rowTextPositionList" ID="_rowTextPosition" runat="server" TextId="/controlText/matrixItemAppearanceEditor/rowTextPosition" /></p>
    </div>
    <div class="formInput left">
        <ckbx:MultiLanguageDropDownList ID="_rowTextPositionList" runat="server" uframeignore="true">
            <asp:ListItem Value="Left" Text="Left" TextId="/controlText/matrixItemAppearanceEditor/left" />
            <asp:ListItem Value="Center" Text="Center" TextId="/controlText/matrixItemAppearanceEditor/center" />
            <asp:ListItem Value="Right" Text="Right" TextId="/controlText/matrixItemAppearanceEditor/right" />
        </ckbx:MultiLanguageDropDownList>
    </div>
    <br class="clear"/>
</div>