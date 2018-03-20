<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ValueListSlider.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.SurveyMobile.ValueListSlider" %>
<%@ Import Namespace="Checkbox.Forms.Items.Configuration" %>
<%@ Import Namespace="Checkbox.Wcf.Services.Proxies" %>

<ckbx:ResolvingScriptElement runat="server" Source="~/Resources/jquery.mobile-verticalSlider.js" />

<div class="ui-slider-container vertical-mobile-text-slider-container" <%= Model.IsSPCArgument ? "spcMarker='true'" : "" %>>   
    <asp:DropDownList ID="_slider" runat="server" AutoPostBack="False" data-role="none" type="vertical-range" />
    
    <span dataDefaultId="<%= DefaultOptionId %>" dataDefaultValue="<%= DefaultOptionIndex %>" dataMaxValue="<%= Model.Options.Count() - 1 %>" 
        class="clear sliderDefaultValues"></span>
</div>

<script type="text/C#" runat="server">
    /// <summary>
    /// Initialize the control
    /// </summary>
    /// <param name="radioButtonScaleItem"></param>
    public override void Initialize(SurveyResponseItem radioButtonScaleItem)
    {
        base.Initialize(radioButtonScaleItem);

        _slider.Attributes["class"] = "slider" + Model.ItemId;
        _slider.Attributes["type"] = "vertical-range";
        
        InitializeOptionsList();
    }
    
    /// <summary>
    /// Default option index
    /// </summary>
    protected int DefaultOptionIndex
    {
       get
       {
           var defaultOption = Model.Options.Select((o, i) => new{ option = o, index = i })
               .FirstOrDefault(o => o.option.IsDefault);
           
           return defaultOption != null ? defaultOption.index : 0;
       }
    }

    /// <summary>
    /// Default option id
    /// </summary>
    protected int? DefaultOptionId
    {
        get
        {
            var defaultOption = Model.Options.FirstOrDefault(o => o.IsDefault);
            return defaultOption != null ? defaultOption.OptionId : default(int?);
        }
    }
        
    
    /// <summary>
    /// Initialize options list
    /// </summary>
    protected void InitializeOptionsList()
    {
        _slider.Items.Clear();

        foreach (SurveyResponseItemOption option in Model.Options)
        {
            if (ValueListOptionType == SliderValueListOptionType.Text)
                _slider.Items.Add(new ListItem(GetOptionText(option.Text), option.OptionId.ToString()));
            else
                _slider.Items.Add(new ListItem(option.ContentId + "," + GetOptionText(option.Alias), option.OptionId.ToString()));

            if (option.IsSelected)
                _slider.SelectedValue = option.OptionId.ToString();
        }
    }
</script>