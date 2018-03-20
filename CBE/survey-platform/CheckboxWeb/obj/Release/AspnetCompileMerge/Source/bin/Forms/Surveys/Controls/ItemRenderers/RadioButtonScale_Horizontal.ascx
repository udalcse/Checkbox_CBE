<%@ Control Language="C#" CodeBehind="RadioButtonScale_Horizontal.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.RadioButtonScale_Horizontal" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Wcf.Services.Proxies" %>
<%@ Register Namespace="CheckboxWeb.Controls.ControlsForRepeater" Assembly="CheckboxWeb" TagPrefix="ctrlForRepeater" %>

<%-- Renderer for Horizontal Radio Button Scales --%>
<asp:ObjectDataSource 
    ID="_optionsSource" 
    runat="server" 
    SelectMethod="GetNonOtherOptions" 
    TypeName="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.RadioButtonScale_Horizontal" 
    DataObjectTypeName="Checkbox.Forms.Items.ListOption" 
    OnObjectCreating="OptionsSource_ObjectCreating" />

<div style="min-width:<%=GetMinContainerWidth()%>;">
    <asp:Repeater ID="_headerrepeater"  runat="server" DataSourceID="_optionsSource" OnDataBinding="HeaderRepeater_DataBinding">
        <ItemTemplate>
            <div class="<%# GetHeaderDivClass(Container.ItemIndex) %>" style="width:<%# GetHeaderDivWidth(Container.ItemIndex) %>"><%# GetHeaderDivText(Container.ItemIndex) %></div>
        </ItemTemplate>
    </asp:Repeater>

    <div style="clear:both;"></div>

    <asp:Repeater ID="_horizontalOptionRepeater" runat="server" DataSourceID="_optionsSource" OnDataBinding="OptionRepeater_DataBinding" OnItemCreated="OptionRepeater_ItemCreated">
        <ItemTemplate>
            <div class="ratingScaleOptionContainer_horizontal" style="width:<%=OptionWidth%>;<%=GetBorderOverride()%>">
                <div class="ratingScaleOptionInput_horizontal">
                    <ctrlForRepeater:RadioButtonForRepeater ID="_inputRad" runat="server" Text="" GroupName='<%# "ScaleRads_" + Model.ItemId %>' Checked='<%# GetOptionChecked(DataBinder.Eval(Container.DataItem, "OptionId")) %>' />
                </div>
                <div class="ratingScaleOptionText_horizontal">
                    <span class="Answer">
                        <asp:Literal ID="_optionText" Text='<%# DataBinder.Eval(Container.DataItem, "Text") %>' runat="server" />
                    </span>
                </div>
            </div>
        </ItemTemplate>
        <FooterTemplate>
            <asp:PlaceHolder runat="server" ID="_naOptionPlace"  Visible='<%# IsNotApplicableOptionEnabled() %>'>
                 <div class="ratingScaleNaOptionContainer_horizontal" style="width:<%=NaOptionWidth%>;<%=GetBorderOverride()%>;">
                     <div style="text-align:center">
                        <ctrlForRepeater:RadioButtonForRepeater ID="_naInputRad" GroupName='<%# "ScaleRads_" + Model.ItemId %>' runat="server" Text="" Checked='<%# NaOptionSelected %>' />
                    </div>
                    <div style="text-align:center">
                        <span class="Answer">
                            <asp:Literal ID="_optionText" Text='<%# Model.InstanceData["NotApplicableText"] %>'  runat="server" />
                        </span>
                    </div>
                </div>
            </asp:PlaceHolder>
            <div style="clear:both;"></div>
        </FooterTemplate>
    </asp:Repeater>
</div>
<div style="clear:both;"></div>
<script type="text/C#" runat="server">

    /// <summary>
    /// Dictionary of inputs.
    /// </summary>
    protected Dictionary<int, RadioButton> InputDictionary { get; set; }
    
    /// <summary>
    /// Width for individual options
    /// </summary>
    protected Unit? OptionWidth { get; set; }

    /// <summary>
    /// Width for N/A Option
    /// </summary>
    protected Unit? NaOptionWidth { get; set; }

    /// <summary>
    /// Selected option id
    /// </summary>
    protected int? SelectedOptionId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    protected int MiddleDiv { get; set; }

    /// <summary>
    /// Specify whether selected option is the na option
    /// </summary>
    protected bool NaOptionSelected { get; set; }

    /// <summary>
    /// Determine  if NA option enabled.
    /// </summary>
    /// <returns></returns>
    protected bool IsNotApplicableOptionEnabled()
    {
        //N/A is enabled if the item contains an "other" option.
        return GetAllOptions().FirstOrDefault(opt => opt.IsOther) != null;
    }

    /// <summary>
    /// Calculate width of option header text
    /// </summary>
    /// <returns></returns>
    protected void CalculateContainerWidths()
    {
        //Header text container width needs to be set so that it is large enough
        // to contain all text and span all options except the N/A option (if any).
        //So, the width will be the greater of a fixed value * (number of options) or 
        // another fixed value * (number of characters in largest word in scale text).

        const int pixelsPerOption = 30;
        const int pixelsPerLetter = 15;

        string startText = Utilities.IsNotNullOrEmpty(Model.InstanceData["StartText"]) ? Model.InstanceData["StartText"] : string.Empty;
        string midText = Utilities.IsNotNullOrEmpty(Model.InstanceData["MidText"]) ? Model.InstanceData["MidText"] : string.Empty;
        string endText = Utilities.IsNotNullOrEmpty(Model.InstanceData["EndText"]) ? Model.InstanceData["EndText"] : string.Empty;
        string naText = Utilities.IsNotNullOrEmpty(Model.InstanceData["NotApplicableText"]) ? Model.InstanceData["NotApplicableText"] : string.Empty;

        //Get sum of lengths of longest words for start/mid/end text so that we can
        // ensure word will fit w/out need for wrapping in middle of word.
        int maxWordLengthSum = new[]
            {
                Utilities.IsNotNullOrEmpty(startText) ? startText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Max(word => word.Length) : 1,
                Utilities.IsNotNullOrEmpty(midText) ? midText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Max(word => word.Length) : 1,
                Utilities.IsNotNullOrEmpty(endText) ? endText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Max(word => word.Length) : 1
            }.Sum();

        //Sum lengths

        //Min header text size (in pixels)
        int minHeaderTextSize = maxWordLengthSum * pixelsPerLetter;
        int minOptionSize = GetNonOtherOptions().Count() * pixelsPerOption;
        int naOptionSize = (Utilities.IsNotNullOrEmpty(naText) ? naText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Max(word => word.Length) : 1) * pixelsPerLetter;

        int containerWidth = Math.Max(minOptionSize, minHeaderTextSize);


        //Divide container width by number of non-other options to set option width,
        // which is necessary in cases where start/mid/end texts require more room
        // than options.
        OptionWidth = Unit.Pixel(containerWidth / Math.Max(GetNonOtherOptions().Count(), 1));
        NaOptionWidth = Unit.Pixel(Math.Max(naOptionSize, Math.Max((int)OptionWidth.Value.Value, 1)));
        
        //Finally, check to see if option width specified in appearance
        int? appearanceOptionWidth = Utilities.AsInt(Appearance["OptionWidth"]);

        if (appearanceOptionWidth.HasValue)
        {
            OptionWidth = Unit.Pixel(appearanceOptionWidth.Value);
            NaOptionWidth = Unit.Pixel(appearanceOptionWidth.Value);
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected string GetMinContainerWidth()
    {
        EnsureInitialized();

        return ((GetNonOtherOptions().Count()*OptionWidth.Value.Value) + NaOptionWidth.Value.Value + 25).ToString() + "px";
    }
    
    /// <summary>
    /// Get class to use for header div
    /// </summary>
    /// <param name="itemIndex"></param>
    /// <returns></returns>
    protected string GetHeaderDivClass(int itemIndex)
    {
        if (itemIndex == 0)
        {
            return "ratingScaleStartText_horizontal";
        }

        if (IsMiddleDiv(itemIndex))
        {
            return "ratingScaleMidText_horizontal";
        }

        return itemIndex == (GetNonOtherOptions().Count() - 1) 
            ? "ratingScaleEndText_horizontal"
            : "ratingScaleSpacerText_horizontal";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemIndex"></param>
    /// <returns></returns>
    protected string GetHeaderDivWidth(int itemIndex)
    {
        if (!IsMiddleDiv(itemIndex))
        {
            if (IsMiddleDiv(itemIndex - 1) && GetNonOtherOptions().Count() % 2 == 0)
                return "0px";
            return OptionWidth.ToString();
        }

        return GetNonOtherOptions().Count()%2 == 1
                   ? OptionWidth.ToString()
                   : Unit.Pixel((int) (2*OptionWidth.Value.Value)).ToString();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemIndex"></param>
    /// <returns></returns>
    protected string GetHeaderDivText(int itemIndex)
    {
        string returnVal = string.Empty;

        if (itemIndex == 0)
            returnVal = Model.InstanceData["StartText"];
        else if (IsMiddleDiv(itemIndex))
            returnVal = Model.InstanceData["MidText"];
        else if (itemIndex == (GetNonOtherOptions().Count() - 1))
            returnVal = Model.InstanceData["EndText"];

        return string.IsNullOrEmpty(returnVal) ? "&nbsp;" : Utilities.AdvancedHtmlEncode(returnVal);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemIndex"></param>
    /// <returns></returns>
    protected bool IsMiddleDiv(int itemIndex)
    {
        var optionCount = GetNonOtherOptions().Count();
        
        //If odd number, middle div = ((count + 1) / 2) - 1 (add 1 after to account for 0-index)
        if (optionCount % 2 == 1)
        {
            //
            return itemIndex == ((optionCount + 1) / 2) - 1;
        }

        return itemIndex == (optionCount / 2) - 1;
    }
    

    /// <summary>
    /// Handle repeater binding event to perform calculations, identify selected option, etc.
    /// </summary>
    protected void EnsureInitialized()
    {
        if (InputDictionary != null)
        {
            return;
        }
        
        //Locate selected options
        SurveyResponseItemOption selectedOption = GetAllOptions().FirstOrDefault(opt => opt.IsSelected);

        SelectedOptionId = (selectedOption != null)
            ? selectedOption.OptionId
            : (int?)null;

        NaOptionSelected = (selectedOption != null)
            ? selectedOption.IsOther
            : false;

        //Calculate various widthds
        CalculateContainerWidths();

        //Initialize radio buttons collection
        InputDictionary = new Dictionary<int, RadioButton>();
    }    
    
   
    /// <summary>
    /// Handle repeater binding event to perform calculations, identify selected option, etc.
    /// </summary>
    protected void OptionRepeater_DataBinding(object sender, EventArgs e)
    {
        EnsureInitialized();
    }



    /// <summary>
    /// Handle repeater binding event to perform calculations, identify selected option, etc.
    /// </summary>
    protected void HeaderRepeater_DataBinding(object sender, EventArgs e)
    {
        EnsureInitialized();
    }    

    /// <summary>
    /// Handle item created event to store a reference to created radio buttons for easy reference.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OptionRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
    {
        SurveyResponseItemOption itemOption = e.Item.DataItem as SurveyResponseItemOption;
          
        if (e.Item.ItemType == ListItemType.Item
            || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            if (itemOption == null)
            {
                return;
            } 
            
            RadioButton radInput = e.Item.FindControl("_inputRad") as RadioButton;

            if (radInput != null)
            {
                InputDictionary[itemOption.OptionId] = radInput;
            }   
        }

        if (e.Item.ItemType == ListItemType.Footer)
        {
            RadioButton radInput = e.Item.FindControl("_naInputRad") as RadioButton;
            var naOption = GetAllOptions().FirstOrDefault(opt => opt.IsOther);
            
            if (radInput != null && InputDictionary != null && naOption!=null)
            {
                InputDictionary[naOption.OptionId] = radInput;
            }
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public string GetBorderOverride()
    {
        return "Yes".Equals(Appearance["ShowSeparator"], StringComparison.InvariantCultureIgnoreCase) 
            ? string.Empty 
            : "border-style:none;";
    }

    /// <summary>
    /// Return a boolean indicating if the specified option should be checked.
    /// </summary>
    /// <param name="optionIdAsObject"></param>
    /// <returns></returns>
    protected bool GetOptionChecked(object optionIdAsObject)
    {
        //If no options selected, do nothing and return false
        if(!SelectedOptionId.HasValue)
        {
            return false;
        }
        
        //Otherwise, convert to option id.
        if (optionIdAsObject == null)
        {
            return false;
        }

        try
        {
            return (int)optionIdAsObject == SelectedOptionId.Value;
        }
        catch
        {
        }
        
        //Error occurred, return false;
        return false;
    }

    /// <summary>
    /// Get the ID of the selected option
    /// </summary>
    /// <returns></returns>
    public override int? GetSelectedOptionId()
    {
        var selectedValue = Request["ScaleRads_" + Model.ItemId];
        var radio = InputDictionary.FirstOrDefault(i => i.Value.ClientID == selectedValue);
        if (radio.Key != default(int))
            return radio.Key;
        
        //Return first selected option id.
       /* foreach(int optionId in InputDictionary.Keys)
        {
            if (InputDictionary[optionId].Checked)
                return optionId;
        }*/

        return null;
    }
  
    
</script>