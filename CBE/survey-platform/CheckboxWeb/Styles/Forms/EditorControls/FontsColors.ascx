<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="FontsColors.ascx.cs" Inherits="CheckboxWeb.Styles.Forms.EditorControls.FontsColors" %>
<%@ Register TagPrefix="style" Src="~/Styles/Controls/FontSelector.ascx" TagName="FontSelector" %>
<%@ Import Namespace="Checkbox.Web" %>

<!-- Data Sources for font size and style drop down lists -->
<asp:XmlDataSource ID="_borderStyleDataSource" runat="server" DataFile="~/Resources/CodeDependentResources.xml" XPath="/CodeDependentResources/BorderStyles/BorderStyle" />
<asp:XmlDataSource ID="_borderSizeDataSource" runat="server" DataFile="~/Resources/CodeDependentResources.xml" XPath="/CodeDependentResources/BorderSizes/BorderSize" />

<div class="styleSectionHeader"><ckbx:MultiLanguageLabel ID="FontsColorsLbl" runat="server" TextId="/pageText/styles/forms/editor.aspx/fontsColors" /></div>

<ajaxToolkit:Accordion ID="_fontsColorsAccordion" runat="server" SelectedIndex="0" HeaderCssClass="menu_head" HeaderSelectedCssClass="menu_head_selected" AutoSize="Fill" FadeTransitions="true" TransitionDuration="150" FramesPerSecond="40" RequireOpenedPane="true" SuppressHeaderPostbacks="false" Height="550" >
  <Panes>
    <ajaxToolkit:AccordionPane ID="bodyStyles" runat="server" >
      <Header><ckbx:MultiLanguageLinkButton ID="bodyStylesLbl" runat="server" TextId="/pageText/styles/forms/editor.aspx/bodyStyles" SkinID="PageViewLinkHeader" CommandArgument="body" /></Header>
      <Content>
        <ckbx:MultiLanguageLabel ID="backgroundColorLbl" runat="server" TextId="/pageText/styles/forms/editor.aspx/backgroundColor" SkinID="PageViewItemLabel" />
        <telerik:RadColorPicker ID="_backgroundColorPicker" runat="server" PaletteModes="WebPalette,RGBSliders" ShowIcon="true" />

        <ckbx:MultiLanguageLabel ID="backgroundImgLbl" runat="server" TextId="/pageText/styles/forms/editor.aspx/backgroundImage" SkinID="PageViewItemLabel" />
        <asp:Label ID="_backgroundImageUrl" runat="server" />
        <ckbx:MultiLanguageImageButton ID="_changeBgImageLnk" runat="server" SkinId="StyleEditorChangeImage" ToolTipTextId="/pageText/styles/forms/editor.aspx/changeImage" Visible="true" />
        <ckbx:MultiLanguageImageButton ID="_deleteBgImageLnk" runat="server" SkinID="StyleEditorDeleteImage" ToolTipTextId="/pageText/styles/forms/editor.aspx/deleteBgImg" Visible="false" />

        <style:FontSelector ID="_baseFont" runat="server" TextId="/pageText/styles/forms/editor.aspx/baseFont" ElementName="body" EnableStyleBuilder="false" />
        <ckbx:MultiLanguageLabel ID="itemSpaceLbl" runat="server" TextId="/pageText/styles/forms/editor.aspx/itemSpacing" SkinID="PageViewItemLabel" /><asp:TextBox ID="_itemSpace" runat="server" Width="60px" />px
      </Content>
    </ajaxToolkit:AccordionPane>
    
    <ajaxToolkit:AccordionPane ID="fontStyles" runat="server" >
      <Header><ckbx:MultiLanguageLabel ID="FontStylesLbl" runat="server" TextId="/pageText/styles/forms/editor.aspx/fontStyles" SkinID="PageViewHeader" /></Header>
      <Content>
        <style:FontSelector ID="_titleFont" runat="server" TextId="/pageText/styles/forms/editor.aspx/titleFont" ElementName=".title" EnableStyleBuilder="true" OnBuildStyleClick="EditStyle" />
        <style:FontSelector ID="_pageNumberFont" runat="server" TextId="/pageText/styles/forms/editor.aspx/pageNumber" ElementName=".PageNumber" EnableStyleBuilder="true" OnBuildStyleClick="EditStyle" />
        <style:FontSelector ID="_errorMessage" runat="server" TextId="/pageText/styles/forms/editor.aspx/errorMessage" ElementName=".Error" EnableStyleBuilder="true" OnBuildStyleClick="EditStyle" />
        <style:FontSelector ID="_questionFont" runat="server" TextId="/pageText/styles/forms/editor.aspx/questionFont" ElementName=".Question" EnableStyleBuilder="true" OnBuildStyleClick="EditStyle" />
        <style:FontSelector ID="_subtextFont" runat="server" TextId="/pageText/styles/forms/editor.aspx/subtextFont" ElementName=".Description" EnableStyleBuilder="true" OnBuildStyleClick="EditStyle" />
        <style:FontSelector ID="_answerFont" runat="server" TextId="/pageText/styles/forms/editor.aspx/answerFont" ElementName=".Answer" EnableStyleBuilder="true" OnBuildStyleClick="EditStyle" />
        <div style="clear:left;"><ckbx:MultiLanguageCheckBox ID="pageNumbers" runat="server" TextId="/pageText/styles/forms/editor.aspx/pageNumber" /></div>
      </Content>
    </ajaxToolkit:AccordionPane>
    <ajaxToolkit:AccordionPane ID="matrix" runat="server" >
      <Header><ckbx:MultiLanguageLabel ID="MatrixStylesLbl" runat="server" TextId="/pageText/styles/forms/editor.aspx/matrixStyles" SkinID="PageViewHeader" /></Header>
      <Content>
        <style:FontSelector runat="server" ID="_matrixHeaderQuestionFont"
             TextId="/pageText/styles/forms/editor.aspx/headerQuestionFont"
             ElementName=".Matrix .header" 
             AltElementName=".Matrix .header td,.Matrix .header th"
             EnableStyleBuilder="true"
             OnBuildStyleClick="EditStyle"
              />
         <style:FontSelector runat="server" ID="_matrixHeaderAnswerFont"
             TextID="/pageText/styles/forms/editor.aspx/headerAnswerFont"
             ElementName=".Matrix .Answer"
             AltElementName=".Matrix .Answer td"
             EnableStyleBuilder="true"
             OnBuildStyleClick="EditStyle"
              />
          <style:FontSelector runat="server" ID="_matrixSubheaderFont"
             TextID="/pageText/styles/forms/editor.aspx/subheadingFont"
             ElementName=".Matrix .subheader"
             AltElementName=".Matrix .subheader td"
             EnableStyleBuilder="true" 
             OnBuildStyleClick="EditStyle"
              />
          <style:FontSelector runat="server" ID="_matrixRowFont"
             TextID="/pageText/styles/forms/editor.aspx/rowFont"
             ElementName=".Matrix .Item"
             AltElementName=".Matrix .Item td"
             EnableStyleBuilder="true" 
             OnBuildStyleClick="EditStyle"
              />
          <style:FontSelector runat="server" ID="_matrixAltRowFont"
             TextID="/pageText/styles/forms/editor.aspx/alternatingRowFont"
             ElementName=".Matrix .AlternatingItem"
             AltElementName=".Matrix .AlternatingItem td"
             EnableStyleBuilder="true"
             OnBuildStyleClick="EditStyle"
              />
        </Content>
    </ajaxToolkit:AccordionPane>
    <ajaxToolkit:AccordionPane ID="otherMatrix" runat="server" >
      <Header><ckbx:MultiLanguageLabel ID="otherMatrixStylesLbl" runat="server" TextId="/pageText/styles/forms/editor.aspx/otherMatrixStyles" SkinID="PageViewHeader" /></Header>
      <Content>
          <div>
            <div style="float:left;"><ckbx:MultiLanguageLabel ID="_matrixBorderLbl" runat="server" TextId="/pageText/styles/forms/editor.aspx/matrixBorder" SkinID="PageViewItemLabel" /></div>
            <div style="float:left;"><asp:DropDownList ID="_matrixBorderWidth" runat="server" SkinID="StyleEditorDropDown" DataTextField="Text" DataValueField="Value" DataSourceID="_borderSizeDataSource"  /></div>
            <div style="float:left;"><asp:DropDownList ID="_matrixBorderStyle" runat="server" SkinID="StyleEditorDropDown" DataTextField="Text" DataValueField="Value" DataSourceID="_borderStyleDataSource"  /></div>
            <div style="float:left;"><telerik:RadColorPicker ID="_matrixBorderColorPicker" runat="server" PaletteModes="WebPalette,RGBSliders" ShowIcon="true"  /></div>
          </div>
          <br /><br />
          <div><div style="float:left;clear:left;"><ckbx:MultiLanguageLabel ID="gridRowHeaderRowLbl" runat="server" TextId="/pageText/styles/forms/editor.aspx/headerRow" SkinID="PageViewItemLabel" /></div><div style="float:left;"><telerik:RadColorPicker ID="_matrixHeaderRowColor" runat="server" PaletteModes="WebPalette,RGBSliders" ShowIcon="true"  /></div></div>
          <div><div style="float:left;clear:left;"><ckbx:MultiLanguageLabel ID="gridRowSubHeadingLabel" runat="server" TextId="/pageText/styles/forms/editor.aspx/subHeadingRow" SkinID="PageViewItemLabel" /></div><div style="float:left;"><telerik:RadColorPicker ID="_matrixSubHeadingRowColor" runat="server" PaletteModes="WebPalette,RGBSliders" ShowIcon="true"  /></div></div>
          <div><div style="float:left;clear:left;"><ckbx:MultiLanguageLabel ID="gridRowDefaultRowLbl" runat="server" TextId="/pageText/styles/forms/editor.aspx/defaultRow" SkinID="PageViewItemLabel" /></div><div style="float:left;"><telerik:RadColorPicker ID="_matrixDefaultRowColor" runat="server" PaletteModes="WebPalette,RGBSliders" ShowIcon="true"  /></div></div>
          <div><div style="float:left;clear:left;"><ckbx:MultiLanguageLabel ID="gridRowAlternateRowLbl" runat="server" TextId="/pageText/styles/forms/editor.aspx/alternatingRow" SkinID="PageViewItemLabel" /></div><div style="float:left;"><telerik:RadColorPicker ID="_matrixAltRowColor" runat="server" PaletteModes="WebPalette,RGBSliders" ShowIcon="true"  /></div></div>
      </Content>
    </ajaxToolkit:AccordionPane>
  </Panes>
</ajaxToolkit:Accordion>