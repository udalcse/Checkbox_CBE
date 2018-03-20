using Checkbox.Web.Page;

namespace CheckboxWeb.Styles.Charts
{
    public partial class Editor : SecuredPage //, IStatusPage
    {
        //protected LightweightStyleTemplate Style
        //{
        //    get { return (LightweightStyleTemplate)HttpContext.Current.Session["CurrentChartStyleProperties"]; }
        //    set { HttpContext.Current.Session["CurrentChartStyleProperties"] = value; }
        //}

        //protected SummaryChartItemAppearanceData Appearance
        //{
        //    get { return (SummaryChartItemAppearanceData)HttpContext.Current.Session["CurrentChartAppearance"]; }
        //    set { HttpContext.Current.Session["CurrentChartAppearance"] = value; }
        //}

        //protected override void OnPageInit()
        //{
        //    string id = Request.QueryString["id"];
        //    if (!Page.IsPostBack)
        //    {
        //        if (!string.IsNullOrEmpty(id))
        //        {
        //            var style = ChartStyleManager.GetChartStyle(Int32.Parse(id));

        //            Style = new LightweightStyleTemplate
        //            {
        //                TemplateId = style.TemplateId,
        //                Name = style.Name,
        //                IsEditable = style.IsEditable,
        //                IsPublic = style.IsPublic
        //            };
        //            Appearance =(SummaryChartItemAppearanceData) ChartStyleManager.GetChartStyleAppearance(Int32.Parse(id));
        //        }
        //        else
        //        {
        //            Style = new LightweightStyleTemplate
        //            {
        //                Name = string.Empty,
        //                IsEditable = true,
        //                IsPublic = false
        //            };
        //            Appearance = new SummaryChartItemAppearanceData();
        //        }
        //    }

        //    _chartPreview.Initialize(Appearance);

        //    if (!Page.IsPostBack)
        //    {
        //        AddItem(WebTextManager.GetText("/pageText/styles/charts/editor.aspx/general"), "General");
        //        AddPageView(_panelBar.FindItemByValue("General"));
        //        AddItem(WebTextManager.GetText("/pageText/styles/charts/editor.aspx/text"), "Text");
        //        AddItem(WebTextManager.GetText("/pageText/styles/charts/editor.aspx/border"), "Border");
        //        AddItem(WebTextManager.GetText("/pageText/styles/charts/editor.aspx/graphType"), "GraphOptions");
        //        AddItem(WebTextManager.GetText("/pageText/styles/charts/editor.aspx/other"), "Other");
        //    }

        //    _panelBar.ItemClick += _panelBar_ItemClick;

        //    base.OnPageInit();
        //}

        ///// <summary>
        ///// Update appearance on postback
        ///// </summary>
        //protected override void OnPageLoad()
        //{
        //    base.OnPageLoad();

        //    //Create controls for title lbl
        //    PlaceHolder titlePlaceholder = new PlaceHolder();
        //    Label label = new Label { Text = WebTextManager.GetText("/pageText/styles/charts/editor.aspx/editChart") + " - " };
        //    HyperLink link = new HyperLink
        //    {
        //        NavigateUrl = ApplicationManager.ApplicationRoot + "/Styles/Manage.aspx?grid=chartgrid",
        //        Text = Utilities.StripHtml(Style.Name, 255)
        //    };

        //    titlePlaceholder.Controls.Add(label);
        //    titlePlaceholder.Controls.Add(link);

        //    ((BaseMasterPage)Master).SetTitleControl(titlePlaceholder);

        //    WireStatusControl(_multiPage);

        //    RegisterClientScriptInclude("HideStatus", ResolveUrl("~/Resources/HideStatus.js"));
        //}

        //private void AddPageView(RadPanelItem item)
        //{
        //    RadPageView pageView = new RadPageView();
        //    pageView.ID = item.Value;

        //    _multiPage.PageViews.Clear();
        //    if(_multiPage.FindPageViewByID(pageView.ID) == null)
        //        _multiPage.PageViews.Add(pageView);

        //    pageView.CssClass = "pageView";
        //    pageView.Selected = true;
        //}

        //private void AddItem(string itemName, string itemValue)
        //{
        //    RadPanelItem item = new RadPanelItem();
        //    item.Text = itemName;
        //    item.Value = itemValue;
        //    _panelBar.Items.Add(item);
        //}

        //protected void _multiPage_PageViewCreated(object sender, RadMultiPageEventArgs e)
        //{
        //    string userControlName = e.PageView.ID + ".ascx";

        //    Control userControl = Page.LoadControl("EditorControls/" + userControlName);
        //    userControl.ID = e.PageView.ID + "_userControl";

        //    if (userControl is ChartStyleUserControl)
        //    {
        //        ((ChartStyleUserControl)userControl).Initialize(Appearance);
        //        ((ChartStyleUserControl)userControl).Force3DSettingsEnabled = true;
        //        ((ChartStyleUserControl)userControl).ShowDoughnutOptions = true;
        //        ((ChartStyleUserControl)userControl).ShowPieOptions = true;
        //        ((ChartStyleUserControl)userControl).ShowBarOptions = true;
        //    }

        //    e.PageView.Controls.Add(userControl);
        //}

        //protected void _panelBar_ItemClick(object sender, RadPanelBarEventArgs e)
        //{
        //    AddPageView(e.Item);
        //}

        //protected void _cancel_Click(object sender, EventArgs e)
        //{
        //    HttpContext.Current.Session["CurrentChartStyleProperties"] = null;
        //    HttpContext.Current.Session["CurrentChartAppearance"] = null;

        //    Response.Redirect("~/Styles/Manage.aspx?grid=chartgrid");
        //}

        //protected void _save_Click(object sender, EventArgs e)
        //{
        //    if (Page.IsValid)
        //    {
        //        if (String.IsNullOrEmpty(Request.QueryString["id"]))
        //        {
        //            Appearance.Save(null);
        //            ChartStyleManager.CreateStyle(Appearance.ID.Value, Style.Name, HttpContext.Current.User.Identity.Name, Style.IsPublic, Style.IsEditable);
        //        }
        //        else
        //        {
        //            ChartStyleManager.UpdateStyle(Style.TemplateId, Style.Name, Style.IsPublic, Style.IsEditable);
        //            Appearance.Save(null);
        //        }

        //        HttpContext.Current.Session["CurrentChartStyleProperties"] = null;
        //        HttpContext.Current.Session["CurrentChartAppearance"] = null;

        //        Response.Redirect("~/Styles/Manage.aspx?grid=chartgrid");
        //    }
        //    else
        //    {
        //        ShowStatusMessage(WebTextManager.GetText("/pageText/styles/editor.aspx/editorError"), StatusMessageType.Error);
        //    }
        //}

        //#region IStatusPage Members

        //public void WireStatusControl(Control sourceControl)
        //{
        //    RadAjaxManager manager = RadAjaxManager.GetCurrent(this);
        //    manager.AjaxSettings.AddAjaxSetting(sourceControl, _editorStatus);
        //}

        //public void WireUndoControl(Control sourceControl)
        //{
        //    throw new NotImplementedException();
        //}

        //public void ShowStatusMessage(string message, StatusMessageType messageType)
        //{
        //    ShowStatusMessage(message, messageType, string.Empty, string.Empty);
        //}

        //public void ShowStatusMessage(string message, StatusMessageType messageType, string actionText, string actionArgument)
        //{
        //    _editorStatus.Message = message;
        //    _editorStatus.MessageType = messageType;
        //    _editorStatus.ActionText = actionText;
        //    _editorStatus.ActionArgument = actionArgument;
        //    _editorStatus.ShowStatus();
        //}

        //#endregion
    }
}
