<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ProfileUpdater.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.SurveyEditor.ProfileUpdater" %>
<%@ Import Namespace="Checkbox.Forms.Items" %>
<%@ Import Namespace="Checkbox.Web"%>
<%@ Import Namespace="Checkbox.Forms.Items.Configuration"%>

<div style="float:left;">
    <asp:GridView ID="_propertiesGrid" runat="server" AutoGenerateColumns="False">
        <EmptyDataTemplate>
            <ckbx:MultiLanguageLabel ID="_noRecordsLbl" runat="server" TextId="/controlText/profileUpdatorItemEditor/noProperties" />
        </EmptyDataTemplate>
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <%# ItemConfigurationManager.GetItemText(
                            (int)DataBinder.Eval(Container.DataItem, "SourceItemId"),
                            "en-US",
                            64,
                            false,
                            true)%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="ProviderName" />
            <asp:BoundField DataField="PropertyName" />
        </Columns>
        <EmptyDataTemplate>
            <div class="warning message" style="margin:0;"><%=WebTextManager.GetText("/controlText/profileUpdatorItemEditor/noProperties")%></div>
        </EmptyDataTemplate>
    </asp:GridView>
</div>

<script type="text/C#" runat="server">
    
    /// <summary>
    /// Initialize grid
    /// </summary>
    protected override void InlineInitialize()
    {
        _propertiesGrid.Columns[0].HeaderText = WebTextManager.GetText("/controlText/profileUpdaterItemEditor/sourceQuestion");
        _propertiesGrid.Columns[1].HeaderText = WebTextManager.GetText("/controlText/profileUpdaterItemEditor/profileProvider");
        _propertiesGrid.Columns[2].HeaderText = WebTextManager.GetText("/controlText/profileUpdaterItemEditor/profileProperty");

        //Hide provider column if only profile provider is checkbox provider
        _propertiesGrid.Columns[1].Visible = false;

        foreach (ProfileProvider provider in System.Web.Profile.ProfileManager.Providers)
        {
            if (!provider.Name.Equals("CheckboxProfileProvider", StringComparison.InvariantCultureIgnoreCase))
            {
                _propertiesGrid.Columns[2].Visible = true;
            }
        }
    }
    
    /// <summary>
    /// Bind data to grid
    /// </summary>
    protected override void InlineBindModel()
    {
        if(Model.AdditionalData != null)
        {
            _propertiesGrid.DataSource =  Model.AdditionalData as string[] ?? new string[]{};
            _propertiesGrid.DataBind();
        }
    }
        
</script>