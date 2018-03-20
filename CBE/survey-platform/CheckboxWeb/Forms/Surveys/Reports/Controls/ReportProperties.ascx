<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ReportProperties.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ReportProperties" %>
<%@ Import Namespace="Checkbox.Web"%>


<div class="padding10">
    <div class="formInput">
        <p><ckbx:MultiLanguageLabel runat="server" AssociatedControlID="_reportName" TextId="/pageText/reportProperties.ascx/analysisName">Analysis Name</ckbx:MultiLanguageLabel></p>
        <asp:textbox id="_reportName" runat="server" Width="475px" Columns="66" ></asp:textbox>
        <div>
            <asp:RequiredFieldValidator ID="_nameRequiredFieldValidator" runat="server" ControlToValidate="_reportName" CssClass="error message">
                <%= WebTextManager.GetText("/pageText/reportProperties.ascx/noAnalysisName")%>
            </asp:RequiredFieldValidator>
            <ckbx:MultiLanguageLabel ID="_nameInUseValidator" runat="server" CssClass="error message" TextID="/pageText/reportProperties.ascx/analysisNameExists" Visible="false"/>
        </div>
    </div>
    <br class="clear" />
        
    <asp:PlaceHolder ID="_reportTypePlace" runat="server">
        <div class="formInput">
            <p><ckbx:MultiLanguageLabel runat="server" AssociatedControlID="_reportTypeWizardRad" TextId="/pageText/reportProperties.ascx/reportType"/></p>
            <div>
                <div class="left fixed_25 radioButton">
                    <asp:RadioButton ID="_reportTypeWizardRad" runat="server" GroupName="ReportType" Checked="true" />
                </div>
                <div class="left">
                    <ckbx:MultiLanguageLabel ID="_reportWizardLbl" runat="server" AssociatedControlID="_reportTypeWizardRad" TextId="/pageText/reportProperties.ascx/wizardReport" />
                </div>
            </div>
            <br class="clear" />

            <div class="left fixed_25 radioButton">
                <asp:RadioButton ID="_reportTypeBlankRad" runat="server" GroupName="ReportType" />
            </div>
            <div class="left">
                <ckbx:MultiLanguageLabel runat="server" AssociatedControlID="_reportTypeBlankRad" TextId="/pageText/reportProperties.ascx/blankReport" />
            </div>
            <br class="clear" />

        </div>
    </asp:PlaceHolder>
    
    <br class="clear" />
    
    <div class="formInput">
        <p>
            <ckbx:MultiLanguageLabel runat="server" AssociatedControlID="_templates" TextId="/pageText/reportProperties.ascx/styleTemplate" />
        </p>
        <asp:DropDownList  ID="_templates" runat="server" /><ckbx:MultiLanguageLabel ID="_noTemplates" runat="server"  TextId="/pageText/reportProperties.ascx/noStyleTemplates" />
        <p>
            <ckbx:MultiLanguageCheckBox ID="_displaySurveyTitle" runat="server" TextId="/pageText/reportProperties.ascx/displaySurveyTitle" />
        </p>
        <p>
            <ckbx:MultiLanguageCheckBox ID="_displayPdfExportButton" runat="server" TextId="/pageText/reportProperties.ascx/displayPdfExportButton" />
        </p>
    </div>

    <br class="clear" />

<!--    
    <div class="label">
        <ckbx:MultiLanguageLabel runat="server" AssociatedControlID="_chartStyles" TextId="/pageText/reportProperties.ascx/chartStyle">Chart Style:</ckbx:MultiLanguageLabel>
    </div>
    <div class="labeledElement">
        <asp:DropDownList  ID="_chartStyles" runat="server" AutoPostBack="true" /><ckbx:MultiLanguageLabel ID="_noChartStyles" runat="server"  TextId="/pageText/reportProperties.ascx/noChartStyles" />
    </div>
    <div class="clear" style="height:10px"></div> -->
    <asp:Panel ID="_simpleSecurityOptions" runat="server">
        <ckbx:MultiLanguageLabel ID="_securityOptionsLbl" runat="server" TextId="/pageText/reportProperties.ascx/simpleSecurityOptions" CssClass="PrezzaLabel"></ckbx:MultiLanguageLabel>
        <ckbx:MultiLanguageDropDownList ID="_securityOptions" runat="server" >
                <asp:ListItem Text="" Selected="True" Value="PUBLIC" ></asp:ListItem>
                <asp:ListItem Text="" Selected="False" Value="PRIVATE" ></asp:ListItem>
                <asp:ListItem Text="" Selected="False" Value="REGISTERED" ></asp:ListItem>
        </ckbx:MultiLanguageDropDownList>
    </asp:Panel>
</div>


<script type="text/javascript" language="javascript">
    <%/*hide invisible validators*/%>
        $(document).ready(function () {
            $('.validationError').filter(function(){return $(this).css('visibility') == 'hidden';}).hide();
            });
</script>