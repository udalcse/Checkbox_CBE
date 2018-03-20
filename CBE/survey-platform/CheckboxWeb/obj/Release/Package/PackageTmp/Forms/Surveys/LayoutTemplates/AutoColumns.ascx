<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="AutoColumns.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.LayoutTemplates.AutoColumns" %>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Forms.PageLayout.Configuration" %>
<%@ Register TagPrefix="cc1" Namespace="Checkbox.Web.Forms.UI.Templates" Assembly="Checkbox.Web" %>
<%@ Register TagPrefix="cc1" Namespace="Checkbox.Web.UI.Controls" Assembly="Checkbox.Web" %>

<cc1:RoundedCorners id="_cornersCtrl" runat="server" RoundedBottom="true" Width="100%">
  <!-- Header -->
  <div>
    <cc1:ControlLayoutZone ID="_headerZone" ZoneName="Header" runat="server" DesignBlockMode="true"></cc1:ControlLayoutZone>
  </div>
    
  <cc1:ControlLayoutZone ID="_titleZone" ZoneName="Title" runat="server" DesignBlockMode="true"></cc1:ControlLayoutZone>
  <cc1:ControlLayoutZone ID="_progressZone" ZoneName="Progress Bar" runat="server" DesignBlockMode="true"></cc1:ControlLayoutZone>
  <cc1:ControlLayoutZone ID="_pageNumberZone" ZoneName="Page Numbers" runat="server" DesignBlockMode="true"></cc1:ControlLayoutZone>
  
  <!-- Top -->
  <cc1:ControlLayoutZone ID="_top" ZoneName="Top" runat="server" DesignBlockMode="true"></cc1:ControlLayoutZone>

  <!-- Dynamic Columns -->
  <cc1:ControlLayoutZone ID="_defaultZone" ZoneName="Default" runat="server" DesignBlockMode="true"></cc1:ControlLayoutZone>

  <!-- Bottom -->
  <cc1:ControlLayoutZone ID="_bottom" ZoneName="Bottom" runat="server" DesignBlockMode="true"></cc1:ControlLayoutZone>

  <!-- Buttons -->
  <div style="padding-top:25px;">
    <cc1:ControlLayoutZone ID="_previousZone" ZoneName="Back" runat="server" DesignBlockMode="false"></cc1:ControlLayoutZone>
    <cc1:ControlLayoutZone ID="_saveZone" ZoneName="Save and Quit" runat="server" DesignBlockMode="false"></cc1:ControlLayoutZone>
    <cc1:ControlLayoutZone ID="_resetZone" ZoneName="Form Reset" runat="server" DesignBlockMode="false"></cc1:ControlLayoutZone>
    <cc1:ControlLayoutZone ID="_nextZone" ZoneName="Next/Finish" runat="server" DesignBlockMode="false"></cc1:ControlLayoutZone>
  </div>
  <!-- Footer -->
  <div>
    <cc1:ControlLayoutZone ID="_footerZone" ZoneName="Footer" runat="server" DesignBlockMode="true"></cc1:ControlLayoutZone>
  </div>
</cc1:RoundedCorners>

<script type="text/C#" runat="server">
    
    private List<Control> _defaultZoneControls;
    private int _columns = 1;

    private bool _controlsModified;

    /// <summary>
    /// This control is not editable within checkbox
    /// </summary>
    public override bool ReadOnly { get { return true; } }
        
    /// <summary>
    /// 
    /// </summary>
    private List<Control>  DefaultZoneControls
    {
        get { return _defaultZoneControls ?? (_defaultZoneControls = new List<Control>()); }
    }

    /// <summary>
    /// Get the type name for this control
    /// </summary>
    public override string TypeName
    {
        get { return "AutoColumns"; }
    }

    /// <summary>
    /// Get/set number of columns for control
    /// </summary>
    public int Columns { get; set; }

    /// <summary>
    /// Get/set whether to show rounded corners
    /// </summary>
    public bool RoundedCorners { get; set; }

    /// <summary>
    /// Get/set border width
    /// </summary>
    public int BorderWidth { get; set; }

    /// <summary>
    /// Get/set the background color
    /// </summary>
    public Color BackgroundColor { get; set; }

    /// <summary>
    /// Get/set the border color
    /// </summary>
    public Color BorderColor { get; set; }

    /// <summary>
    /// Override clear method to clear internal collection as well.
    /// </summary>
    public override void ClearZones()
    {
        base.ClearZones();

        DefaultZoneControls.Clear();
    }

    /// <summary>
    /// Get/set repeat direction for the control
    /// </summary>
    public RepeatDirection RepeatDirection { get; set; }

    /// <summary>
    /// Initialize the item
    /// </summary>
    /// <param name="layoutData"></param>
    /// <param name="languageCode"></param>
    public override void Initialize(PageLayoutTemplateData layoutData, string languageCode)
    {
        base.Initialize(layoutData,languageCode);

        Columns = Convert.ToInt32(layoutData.GetPropertyValue("Columns"));

        RepeatDirection = string.Compare((string)layoutData.GetPropertyValue("RepeatDirection"), "Horizontal", true) == 0 ? RepeatDirection.Horizontal : RepeatDirection.Vertical;

        if (layoutData.GetPropertyValue("RoundedCorners") != null && layoutData.GetPropertyValue("RoundedCorners").ToString().ToLower() == "yes")
        {
            RoundedCorners = true;
        }
        else
        {
            RoundedCorners = false;
        }

        if (layoutData.GetPropertyValue("BorderWidth") != null
            && Utilities.IsNotNullOrEmpty(layoutData.GetPropertyValue("BorderWidth").ToString()))
        {
            BorderWidth = Convert.ToInt32(layoutData.GetPropertyValue("BorderWidth"));
        }
        else
        {
            BorderWidth = 0;
        }

        if (layoutData.GetPropertyValue("BorderColor") != null
            && Utilities.IsNotNullOrEmpty(layoutData.GetPropertyValue("BorderColor").ToString()))
        {
            try
            {
                var colorValue = (string)layoutData.GetPropertyValue("BorderColor");

                BorderColor = colorValue.Equals("Transparent", StringComparison.InvariantCultureIgnoreCase) 
                    ? Color.Transparent 
                    : Utilities.GetColor((string)layoutData.GetPropertyValue("BorderColor"), true);
            }
            catch
            {
                BorderColor = Color.Empty;
            }
        }
        else
        {
            BorderColor = Color.Empty;
        }

        if (layoutData.GetPropertyValue("BackgroundColor") != null
            && Utilities.IsNotNullOrEmpty(layoutData.GetPropertyValue("BackgroundColor").ToString()))
            {
            try
            {
                var colorValue = (string)layoutData.GetPropertyValue("BackgroundColor");

                BackgroundColor = colorValue.Equals("Transparent", StringComparison.InvariantCultureIgnoreCase) 
                    ? Color.Transparent 
                    : Utilities.GetColor((string)layoutData.GetPropertyValue("BackgroundColor"), true);
            }
            catch
            {
                BackgroundColor = Color.Empty;
            }
        }
        else
        {
            BackgroundColor = Color.Empty;
        }
    }

    /// <summary>
    /// Add a control to a zone
    /// </summary>
    /// <param name="zoneName"></param>
    /// <param name="c"></param>
    public override void AddControlToZone(string zoneName, Control c)
    {
        _controlsModified = true;

        //Intercept items going to the control zone
        if (string.Compare(zoneName, "Default", true) == 0)
        {
            DefaultZoneControls.Add(c);
        }
        else
        {
            base.AddControlToZone(zoneName, c);
        }
    }

    /// <summary>
    /// Override on init to place controls on page.  Controls should be placed by now so they can 
    /// participate in viewstate restoration
    /// </summary>
    /// <param name="e"></param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        AddDefaultControls();
        _controlsModified = false;
    }

    /// <summary>
    /// Need to handle pre-render rather than init in cases of moving back in the survey
    /// </summary>
    /// <param name="e"></param>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (_controlsModified)
        {
            AddDefaultControls();
            _controlsModified = false;
        }
    }

    /// <summary>
    /// Return a boolean indicating if a control is a spacer (panels with class = page)
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    private bool ControlIsSpacer(Control c)
    {
        return c is Panel && ((Panel)c).CssClass.Equals("Page", StringComparison.InvariantCultureIgnoreCase);
    }

    /// <summary>
    /// Get number of controls to render, excluding spacers if requested
    /// </summary>
    /// <param name="excludeSpacers"></param>
    private int GetControlCount(bool excludeSpacers)
    {
        //If not excluding spacers, simply return collection count
        if(!excludeSpacers)
        {
            return DefaultZoneControls.Count;
        }

        //Otherwise, count controls that are not panels with class = page
        int controlCount = DefaultZoneControls.Count;


        foreach (Control c in DefaultZoneControls)
        {
            //Decrement count if control is a panel with cssclass="page"
            if (ControlIsSpacer(c))
            {
                controlCount--;
            }
        }

        return controlCount;
    }

    /// <summary>
    /// Add controls to the default zone
    /// </summary>
    protected void AddDefaultControls()
    {
        int rowCount = GetControlCount(true) / Columns;

        if (DefaultZoneControls.Count % Columns != 0)
        {
            rowCount++;
        }

        double columnWidth = (double)100 / (double)Columns;

        var t = new Table {Width = Unit.Percentage(100), CellSpacing = 10, CellPadding = 10};

        //_defaultZone.Controls.Add(t);
        _defaultZone.Controls.Clear();
        _defaultZone.Controls.Add(t);

        if (RepeatDirection == RepeatDirection.Horizontal)
        {
            int itemCount = 0;
            for (int i = 0; i < rowCount; i++)
            {
                var tr = new TableRow {VerticalAlign = VerticalAlign.Top};

                for (int j = 0; j < Columns; j++)
                {
                    var tc = new TableCell {Width = Unit.Percentage(columnWidth)};
                    //LiteralControl lc = new LiteralControl(string.Format("i:{0}  j:{1}   itemCount:{2}  {3}  ", i, j, itemCount, _defaultZoneControls.Count));
                    //tc.Controls.Add(lc);

                    //Add renderer control
                    if (itemCount < DefaultZoneControls.Count)
                    {
                        tc.Controls.Add(DefaultZoneControls[itemCount]);
                        itemCount++;
                    }

                    //Add spacer control, if the next control is spacer
                    if(itemCount < DefaultZoneControls.Count
                        && ControlIsSpacer(DefaultZoneControls[itemCount]))
                    {
                        tc.Controls.Add(DefaultZoneControls[itemCount]);
                        itemCount++;
                    }

                    tr.Cells.Add(tc);
                }

                t.Rows.Add(tr);
            }
        }
        else
        {
            var rows = new List<TableRow>();

            for (int i = 0; i < rowCount; i++)
            {
                rows.Add(new TableRow {VerticalAlign = VerticalAlign.Top});
            }

            int itemCount = 0;

            for (int i = 0; i < Columns; i++)
            {
                for (int j = 0; j < rowCount; j++)
                {
                    //LiteralControl lc = new LiteralControl(string.Format("i:{0}  j:{1}   itemCount:{2}  {3}  ", i, j, itemCount, _defaultZoneControls.Count));
                    var c1 = new TableCell {Width = Unit.Percentage(columnWidth)};
                    //c1.Controls.Add(lc);

                    //Add renderer
                    if (itemCount < DefaultZoneControls.Count)
                    {
                        c1.Controls.Add(DefaultZoneControls[itemCount]);
                        itemCount++;
                    }

                    //Add spacer
                        //Add spacer control, if the next control is spacer
                    if(itemCount < DefaultZoneControls.Count
                        && ControlIsSpacer(DefaultZoneControls[itemCount]))
                    {
                        c1.Controls.Add(DefaultZoneControls[itemCount]);
                        itemCount++;
                    }

                    //Add cell to row
                    rows[j].Cells.Add(c1);
                }
            }

            foreach (TableRow row in rows)
            {
                t.Rows.Add(row);
            }
        }

        _cornersCtrl.BorderColor = BorderColor;
        _cornersCtrl.BackColor = BackgroundColor;
        _cornersCtrl.BorderStyle = BorderStyle.Solid;
        _cornersCtrl.BorderWidth = Unit.Pixel(BorderWidth);
        _cornersCtrl.RoundedBottom = RoundedCorners;
        _cornersCtrl.RoundedTop = RoundedCorners;
    }

</script>