<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ValueListSlider.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.ValueListSlider" %>
<%@ Import Namespace="Checkbox.Forms.Items.Configuration" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Wcf.Services.Proxies" %>

<!--for some reasons 'if IE9' method doesn't work for slider preview-->
<style type="text/css">
.ie9-slider-image {
	margin-left: -50% !important;
    padding-right: 50%;
}
</style>

<script type="text/javascript">
    $(document).ready(function () {
        var options = {
            labels : <%=Model.Options.Length %>,            
            labelSrc: 'text',
            tooltip: false,
            showImages: <%= (ValueListOptionType == SliderValueListOptionType.Image).ToString().ToLower() %>,
            applicationRoot: '<%=ApplicationManager.ApplicationPath %>',
            sliderOptions: {orientation: '<%=Orientation %>', imagePosition: '<%= ImagePosition %>', aliasPosition: '<%= AliasPosition %>'}
        };

        $(".slider<%= Model.ItemId %>").selectToUISlider(options);
        $("a.ui-slider-handle.ui-state-default.ui-corner-all").attr('uframeIgnore','true');
        $(".ui-slider-label").addClass('Answer');

        if (getIeVersion() == 9) {
            $('.ui-slider ol.horizontal img').addClass('ie9-slider-image');
        }
    });
</script>

<div style="width:<%=Width%>;height:<%=Height%>;" class="ui-slider-container" >
    <asp:DropDownList ID="_options" runat="server" uniformIgnore="true" style="display:none;"/>
    
    <div dataDefaultId="<%= DefaultOptionId %>" dataDefaultValue="<%= DefaultOptionIndex %>" dataMaxValue="<%= Model.Options.Count() - 1 %>" 
        class="clear sliderDefaultValues"></div>
</div>

<script type="text/C#" runat="server">
    /// <summary>
    /// Initialize the control
    /// </summary>
    /// <param name="radioButtonScaleItem"></param>
    public override void Initialize(SurveyResponseItem radioButtonScaleItem)
    {
        base.Initialize(radioButtonScaleItem);

        _options.Attributes["class"] = "slider" + Model.ItemId;
        
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
            if (defaultOption != null)
                return defaultOption.OptionId;
            
            var firstOption = Model.Options.FirstOrDefault();
            if (firstOption != null)
                return firstOption.OptionId;
            
            return null;
        }
    }
        
    
    /// <summary>
    /// Initialize options list
    /// </summary>
    protected void InitializeOptionsList()
    {
        _options.Items.Clear();
                
        foreach (SurveyResponseItemOption option in Model.Options)
        {            
            if (ValueListOptionType == SliderValueListOptionType.Text)
                _options.Items.Add(new ListItem(GetOptionText(option), option.OptionId.ToString()));
            else
                _options.Items.Add(new ListItem(option.ContentId + "," + GetOptionText(option.Alias), option.OptionId.ToString()));
            
            if (option.IsSelected)
                _options.SelectedValue = option.OptionId.ToString();
        }        
    }
</script>