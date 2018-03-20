<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Categorized_Matrix.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.AppearanceEditors.Categorized_Matrix" %>

<div class="itemEditorLabel_100">
    <ckbx:MultiLanguageLabel ID="_gridLinesLbl" runat="server" TextId="/controlText/matrixItemAppearanceEditor/gridLines" />
</div>

<div class="itemEditorInput">
    <ckbx:MultiLanguageDropDownList ID="_gridLinesList" runat="server" uframeignore="true">
        <asp:ListItem Value="None" Text="None" TextId="/controlText/matrixItemAppearanceEditor/none" />
        <asp:ListItem Value="Both" Text="Both" TextId="/controlText/matrixItemAppearanceEditor/both" />
        <asp:ListItem Value="Vertical" Text="Vertical" TextId="/controlText/matrixItemAppearanceEditor/vertical" />
        <asp:ListItem Value="Horizontal" Text="Horizontal" TextId="/controlText/matrixItemAppearanceEditor/horizontal" />
    </ckbx:MultiLanguageDropDownList>
</div>

<div style="clear: both;">
</div>

<div class="itemEditorLabel_100">
    <ckbx:MultiLanguageLabel ID="_widthLbl" runat="server" TextId="/controlText/matrixItemAppearanceEditor/width" />
</div>

<div class="itemEditorInput">
    <asp:TextBox ID="_widthTxt" runat="server"  />
</div>

<div style="clear: both;">
</div>


<div class="itemEditorLabel_100">
    <ckbx:MultiLanguageLabel ID="_itemPositionLbl" runat="server" TextId="/controlText/matrixItemAppearanceEditor/itemPosition" />
</div>

<div class="itemEditorInput">
    <ckbx:MultiLanguageDropDownList ID="_itemPositionList" runat="server" uframeignore="true">
        <asp:ListItem Value="Left" Text="Left" TextId="/controlText/matrixItemAppearanceEditor/left" />
        <asp:ListItem Value="Center" Text="Center" TextId="/controlText/matrixItemAppearanceEditor/center" />
        <asp:ListItem Value="Right" Text="Right" TextId="/controlText/matrixItemAppearanceEditor/right" />
    </ckbx:MultiLanguageDropDownList>
</div>

<div style="clear: both;">
</div>

<div class="itemEditorLabel_100">
    <ckbx:MultiLanguageLabel ID="_labelPositionLbl" runat="server" TextId="/controlText/matrixItemAppearanceEditor/labelPosition" />
</div>

<div class="itemEditorInput">
    <ckbx:MultiLanguageDropDownList ID="_labelPositionList" runat="server" uframeignore="true">
        <asp:ListItem Value="Top" Text="Top" TextId="/enum/labelPosition/top" />
        <asp:ListItem Value="Left" Text="Left" TextId="/enum/labelPosition/left" />
        <asp:ListItem Value="Bottom" Text="Bottom" TextId="/enum/labelPosition/bottom" />
        <asp:ListItem Value="Right" Text="Right" TextId="/enum/labelPosition/right" />
    </ckbx:MultiLanguageDropDownList>
</div>
