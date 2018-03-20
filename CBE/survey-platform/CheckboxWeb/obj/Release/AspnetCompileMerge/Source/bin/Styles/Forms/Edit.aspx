<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Edit.aspx.cs" Inherits="CheckboxWeb.Styles.Forms.Edit" MasterPageFile="~/DetailList.Master" IncludeJsLocalization="true" %>
<%@ Register TagPrefix="ckbx" TagName="StyleEditor" Src="~/Styles/Controls/StyleEditor.ascx" %>
<%@ Import Namespace="Checkbox.Globalization.Text" %>
<%@ Import Namespace="Checkbox.Security.Principal" %>
<%@ Import Namespace="Checkbox.Styles" %>
<%@ Import Namespace="System.Xml" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Import Namespace="Checkbox.Users" %>

<%@ MasterType VirtualPath="~/DetailList.Master" %>

<asp:Content ContentPlaceHolderID="_head" ID="_head" runat="server" >
    <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/mColorPicker.min.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/mDOMupdate.js" />

    <script type="text/javascript">

        // Called when the fontsColorsEditorTemplate.html template
        // is finished loading (see /Resources/styleEditor.js)
        function fontsColorsEditorTemplateLoaded() {
        }

        function fixHeight()
        {
            var w = $('#_stylePreviewFrame').parent().parent().parent().parent().width();
            var h = $('#_stylePreviewFrame').parent().parent().parent().parent().height();
            if (h < 500)
                h = 500;

            $('#_stylePreviewFrame').height(h - 130);
            $('#_styleSaveCancelButtonsContainer').css('margin-left', w/2 - $('#_styleSaveCancelButtonsContainer').width()/2);
        }

        function onStyleDeleted() {
            window.location = '<%= ResolveUrl("~/Styles/Manage.aspx") %>';
        }

        function onDeleteStyleConfirm(args) {
            if (args.success){
                svcStyleManagement.deleteFormStyle(
                    _at,
                    <%= StyleId %>,
                    onStyleDeleted
                );
            }
        }

        $(document).ready(function() {
            $('#_stylePreviewFrame').parent().parent().parent().parent().resize(
                function() {
                    fixHeight();
                }
            );

            fixHeight();

            $('#deleteStyleLink').click(function() {
                showConfirmDialogWithCallback(
                    '<%=WebTextManager.GetText("/pageText/Styles/Manage.aspx/deleteStyleConfirm") %>',
                    onDeleteStyleConfirm,
                    350,
                    200,
                    '<%=WebTextManager.GetText("/pageText/Styles/Manage.aspx/deleteStyle") %>');
            });

            //Precompile templates used
            templateHelper.loadAndCompileTemplate('<%=ResolveUrl("~/Styles/Forms/jqtmpl/fontsColorsEditorTemplate.html") %>', 'fontsColorsEditorTemplate.html');
            templateHelper.loadAndCompileTemplate('<%=ResolveUrl("~/Styles/Forms/jqtmpl/formControlsEditorTemplate.html") %>', 'formControlsEditorTemplate.html');
            templateHelper.loadAndCompileTemplate('<%=ResolveUrl("~/Styles/Forms/jqtmpl/fontSelectorTemplate.html") %>', 'fontSelectorTemplate.html');
            templateHelper.loadAndCompileTemplate('<%=ResolveUrl("~/Styles/Forms/jqtmpl/headerFooterEditorTemplate.html") %>', 'headerFooterEditorTemplate.html');

            var headerTextStrings = {
                <%
        int counter = 0;
        foreach(var languageCode in TextManager.SurveyLanguages)
        {
            if(counter > 0)
            {
%>,<%
            }
%>
                '<%=languageCode.ToLower() %>': '<%=(TextManager.GetText(Template.HeaderTextID, languageCode) ?? string.Empty).Replace("'", "\\'").Replace(Environment.NewLine, string.Empty).Replace("\n", string.Empty) %>'<%
            counter++;
        }
%>
            };

            var footerTextStrings = {
                <%
        counter = 0;
        foreach(var languageCode in TextManager.SurveyLanguages)
        {
            if(counter > 0)
            {
%>,<%
            }
%>
                '<%=languageCode.ToLower() %>': '<%=(TextManager.GetText(Template.FooterTextID, languageCode) ?? string.Empty).Replace("'",  "\\'").Replace(Environment.NewLine, string.Empty).Replace("\n", string.Empty) %>'<%
            counter++;
        }
%>
            };

            var langauges = {
                <%
        counter = 0;
        foreach(var languageCode in TextManager.SurveyLanguages)
        {
            if(counter > 0)
            {
%>,<%
            }
%>
                '<%=languageCode.ToLower() %>': '<%=(WebTextManager.GetText("/languageText/" + languageCode) ?? languageCode).Replace("'",  "\\'") %>'<%
            counter++;
        }
%>
            };


            //TODO: Configure style editor for localized header editing
            textHelper.setTextValue('<%=Template.HeaderTextID %>', headerTextStrings['en-us']);
            textHelper.setTextValue('<%=Template.FooterTextID %>', footerTextStrings['en-us']);

            styleEditor.initialize(_at, '<%=ResolveUrl("~/") %>', '<%=Template.HeaderTextID %>', '<%=Template.FooterTextID %>', { styleId: <%= StyleId %>, tinyMCEPath: '<%=ResolveUrl("~/Resources/tiny_mce/tinymce.min.js") %>' });
            statusControl.initialize('_statusPanel');

            //Show preview
            UFrameManager.init({
                id: '_stylePreviewFrame',
                loadFrom: '<%=ResolveUrl("~/Styles/SurveyStylePreview.aspx")%>?s=<%=StyleId %>',
                progressTemplate: $('#styleProgressContainer').html(),
                showProgress: true
            });

            $('#styleName')
                .addClass('hand')
                .ckbxEditable({
                    inputCssClass: 'surveyEditorNameInput',
                    onSave: updateStyleSetting
                });
        });

        function updateStyleSetting(settingElement, newValue, oldValue) {
            svcStyleEditor.updateStyleTemplateSetting(
                _at,
                <%=Template.TemplateID%>,
                settingElement.attr('settingName'),
                escapeInjections(newValue),
                null
                )
                .then(function(result, args) {
                    if (settingElement.attr('settingName') == 'Name') {
                        settingElement.html(result);
                    }
                });
        }
        
        <%-- Show status message --%>
        function ShowStatusMessage(message, isSucceeded) {
            statusControl.showStatusMessage(message, isSucceeded ? StatusMessageType.success : StatusMessageType.error);
        }

        function doCopy(style) {
            var r = confirm("Are you sure to copy this style?");
            if (r == true) {
                UFrameManager.prepareOuterFormSubmit();
                __doPostBack('_copyLink', style);
            }
        }
    </script>
</asp:Content>

<asp:Content ContentPlaceHolderID="_leftContent" ID="left" runat="server">
    <ckbx:StyleEditor id="_editor" runat="server"></ckbx:StyleEditor>
</asp:Content>

<asp:Content ContentPlaceHolderID="_rightContent" ID="right" runat="server">

    <div class="survey-header-container edit-style-container clearfix">
        <div class="header-content">
            <span class="editing-label">Editing</span>
            <h3 id="styleName" editmode="Text" settingName="Name" class="page-name" ><%=Name %></h3>
            
            <a class="action-menu-toggle action-button ckbxButton silverButton" href="#" id="style_actions_button">
                <%= WebTextManager.GetText("/pageText/styles/edit.aspx/styleactions", null, "Style Actions") %>
            </a>

            <div id="style_actions_menu" class="groupMenu" style="display: none;margin-top:30px !important;">
                <ul class="itemActionMenu">
                    <li><a class="ckbxButton roundedCorners border999 shadow999 silverButton" href="javascript:showDialog('Properties.aspx?s=<%= StyleId %>', 'properties');" id="_propertiesLink">
                            <%= WebTextManager.GetText("/pageText/styles/edit.aspx/properties", null, "Properties") %>  </a></li>
                    <li><a class="ckbxButton roundedCorners border999 shadow999 silverButton" href="javascript:doCopy(<%= StyleId %>);" id="_copyLink">
                            <%= WebTextManager.GetText("/pageText/styles/edit.aspx/copy", null, "Copy") %>
                        </a></li>
                    <li><a class="ckbxButton roundedCorners border999 shadow999 silverButton statistics_ExportData" href="<%= ResolveUrl("~/Styles/Export.ashx?id="+ StyleId +"&type=form") %>" >
                            <%= WebTextManager.GetText("/pageText/styles/edit.aspx/export", null, "Export") %>
                        </a></li>
                    <% if (LightweightTemplate != null && LightweightTemplate.CanBeEdited) { %>
                    <li><a class="ckbxButton roundedCorners border999 shadow999 redButton" id="deleteStyleLink" href="javascript:void(0);">
                            <%= WebTextManager.GetText("/pageText/styles/edit.aspx/delete", null, "Delete")%>
                        </a></li>
                    <% } %>
                </ul>    
            </div>
        </div>
    </div>

    <div id="_stylePreviewFrame" style="overflow-y:scroll;overflow-x:hidden;"></div>
    <div id="styleProgressContainer" style="display:none;">
        <div style="text-align:center;">
            <p><%=WebTextManager.GetText("/common/loading") %></p>
            <p>
                <asp:Image ID="_progressSpinner" runat="server" SkinId="ProgressSpinner" />
            </p>
        </div>
    </div>
    <div id="_styleSaveCancelButtonsContainer" style="text-align: center; position: fixed; bottom: 110px; ">
        <a onclick="(function() { styleEditor.hasUnsavedUpdates = false; document.location = (styleEditor.appRoot + 'Styles/Manage.aspx?').replace('//', '/');})();" class="cancelButton"><%=WebTextManager.GetText("/common/cancel")%></a>
        <a href="javascript:styleEditor.saveAndRedirect('<%=StyleId %>');" class="ckbxButton roundedCorners border999 shadow999 silverButton" style="margin-right:35px;margin-left:25px;"><%=WebTextManager.GetText("/common/save") %></a>
        
        <br class="clear" />
    </div>
</asp:Content>
<%-- Event Handling  --%>
<script type="text/C#" runat="server">

    /// <summary>
    /// Get/set handler for showing status message.
    /// The first parameter must be a message.
    /// The second parameter must determine if an operation was succeeded or not.
    /// </summary>
    public string ShowStatusMessageHandler { get; set; }

    /// <summary>
    /// 
    /// </summary>
    protected string TypeCode
    {
        get
        {
            return Template == null ? "p" : Template.Type.ToString().Substring(0, 1).ToLower();
        }
    }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        Master.SetTitle(WebTextManager.GetText("/pageText/styles/forms/editor.aspx/editStyle") + " - " + Template.Name);
        Master.ShowBackButton(ResolveUrl("~/Styles/Manage.aspx"), true);
    }
       
    //private void SaveClick(object sender, EventArgs e)
    //{
    //    //save template and redirect to manage
    //    ClientScript.RegisterClientScriptBlock(GetType(), "saveAndRedirect", "styleEditor.saveAndRedirect();", true);
    //}

    /// <summary>
    /// Get/set style id to load.  StyleId == 1000 indicates default style
    /// </summary>
    public int? StyleId
    {
        get
        {
            int id;
            if (int.TryParse(Request.QueryString["s"], out id))
                return id;

            return null;
        }
    }

    private StyleTemplate _Template;
    protected StyleTemplate Template
    {
        get
        {
            if (_Template != null)
                return _Template;

            if (StyleId.HasValue)
                _Template = StyleTemplateManager.GetStyleTemplate(StyleId.Value);
            else 
                EnsureTemplate();
            
            return _Template;
        }
    }

    private LightweightStyleTemplate _lightweightTemplate;
    protected LightweightStyleTemplate LightweightTemplate
    {
        get
        {
            if (_lightweightTemplate != null)
                return _lightweightTemplate;

            if (StyleId.HasValue)
                _lightweightTemplate = StyleTemplateManager.GetLightweightStyleTemplate(StyleId.Value, UserManager.GetCurrentPrincipal());

            return _lightweightTemplate;
        }
    }
    
    protected void EnsureTemplate()
    {
        if (_Template != null) return;

        var doc = new XmlDocument();
        doc.Load(Server.MapPath("~/Resources/DefaultStyle.xml"));
        _Template = StyleTemplateManager.CreateStyleTemplate(doc, HttpContext.Current.User as CheckboxPrincipal);
    }

    protected string Name { 
        set { Template.Name = value; }
        get { return Template.Name; }
    }


    /// <summary>
    /// Authorise to ensure that the style is editable or creator was the same as the current user
    /// </summary>
    /// <returns></returns>
    protected override bool AuthorizePage()
    {
        if (UserManager.GetCurrentPrincipal() != null)
        {
            if (!Template.IsEditable && !Template.CreatedBy.Equals(User.Identity.Name))
            {
                string[] roles = UserManager.GetCurrentPrincipal().GetRoles();
                if (!roles.Any(r => r.Equals("System Administrator")))
                {
                    return false;
                }
            }
        }
        return base.AuthorizePage();
    }
    
</script>
