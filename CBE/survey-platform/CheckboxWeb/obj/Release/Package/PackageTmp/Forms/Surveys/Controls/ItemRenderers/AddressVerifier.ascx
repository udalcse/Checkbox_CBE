<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="AddressVerifier.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.AddressVerifier" %>
<%@ Register TagPrefix="ckbx" TagName="QuestionText" Src="~/Forms/Surveys/Controls/ItemRenderers/QuestionText.ascx" %>
<%@ Import Namespace="Checkbox.Management"%>
<%@ Import Namespace="System.Globalization"%>
<%@ Import Namespace="Checkbox.Forms.Items"%>
<%@ Import Namespace="Checkbox.Common"%>
<%@ Import Namespace="Checkbox.Web.Forms.UI.Rendering" %>

<asp:Panel ID="_containerPanel" runat="server" CssClass="itemContainer">
    <asp:Panel ID="_contentPanel" runat="server" CssClass="itemContent">
        <asp:Panel ID="_topAndOrLeftPanel" runat="server" CssClass="topAndOrLeftContainer">
            <asp:Panel ID="_textContainer" runat="server" CssClass="textContainer">
                <ckbx:QuestionText ID="_questionText" runat="server" />
            </asp:Panel>

        </asp:Panel>

        <asp:Panel ID="_bottomAndOrRightPanel" runat="server" CssClass="bottomAndOrRightContainer">
            <asp:Panel ID="_inputPanel" runat="server" CssClass="inputContainer">
                <div style="float:left"><asp:TextBox
                    ID="_textInput" 
                    runat="server" 
                    TextMode="SingleLine"
                    Width="400px"
                />                
                <%if (AddressFinderShowCoordinates)
                  { %>
                  <asp:TextBox 
                    ID="_latitude"
                    runat="server"
                    Width="100px"
                    ReadOnly="true"
                    />
                  <asp:TextBox 
                    ID="_longitude"
                    runat="server"
                    Width="100px"
                    ReadOnly="true"
                    />
                    <%}
                  else
                  { %>
                <asp:HiddenField 
                    ID="_latitudeHidden"
                    runat="server"
                    />
                <asp:HiddenField 
                    ID="_longitudeHidden"
                    runat="server"
                    />
                    <%} %>
                </div>
                <div id="<%=ClientID%>_progress" style="display:none;float:left;margin-left:15px;">
                    Retrieving address information..
                </div>
            </asp:Panel> 
        </asp:Panel>

<script type="text/javascript">
    function show_address<%=ClientID%>(value, data) {
        
        $('#<%=ClientID%>__textInput').val(value);                
        $('#<%=ClientID%>__latitude<%=AddressFinderShowCoordinates?"":"Hidden"%>').val(data.y);
        $('#<%=ClientID%>__latitude<%=AddressFinderShowCoordinates?"":"Hidden"%>').attr('value', data.y);
        $('#<%=ClientID%>__longitude<%=AddressFinderShowCoordinates?"":"Hidden"%>').val(data.x);
        $('#<%=ClientID%>__longitude<%=AddressFinderShowCoordinates?"":"Hidden"%>').attr('value', data.x);
        $('input[type=submit]').attr('disabled', null);
        $('#<%=ClientID%>_progress').hide();
    }
    
    $(document).ready(
        function(){
            $('#<%=ClientID%>__textInput').change(function()
            {
                $('input[type=submit]').attr('disabled', 'true');
                $('#<%=ClientID%>_progress').show();
                setTimeout(function(){$('input[type=submit]').attr('disabled', null);$('#<%=ClientID%>_progress').hide();}, 2000);
            });

            var widget<%=ClientID%> = new AddressFinder.Widget(document.getElementById("<%=ClientID%>__textInput"), 
                "<%=ApplicationManager.AppSettings.AddressFinderScriptKey%>",
                {
                    address_params : {
                        region_code: '<%=Region%>',
                        rule: '<%=Rule%>',
                        rural: '<%=Rural%>'
                    }
                });
            
            widget<%=ClientID%>.on("result:select",function(value,data){show_address<%=ClientID%>(value, data);});

            /*
            //console.log(af_initialise);
            af_initialise(document.getElementById("<%=ClientID%>__textInput"), show_address<%=ClientID%> , {
                search_type: function() {
                    return "<%=SearchType%>";
                },
                extraParams: {
                    region: function() { return "<%=Region%>"; },
                    rule: function() { return "<%=Rule%>"; },
                    rural: function() { return "<%=Rural%>"; }
                }
            });
            */
        }
    );
</script>

    </asp:Panel>
</asp:Panel>


<script language="C#" runat="server">
    
    /// <summary>
    /// 
    /// </summary>
    public override List<UserControlItemRendererBase> ChildUserControls
    {
        get
        {
            var childControls = base.ChildUserControls;
            childControls.Add(_questionText);
            return childControls;
        }
    }
    
    /// <summary>
    /// Initialize child user controls to set repeat columns and other appearance properties
    /// </summary>
    protected override void InlineInitialize()
    {
        //Item and label position
        SetLabelPosition();
        SetItemPosition();
        SetInputWidth();
    }


    /// <summary>
    /// Bind controls to survey item.
    /// </summary>
    protected override void InlineBindModel()
    {
        base.InlineBindModel();

        //Input properties, such as showing/hiding proper input as well
        // as width and any restrictions based on answer format
        SetInputProperties();

        if (Model.Answers.Length > 0)
        {
            if (Model.Answers[0].AnswerText.Contains("~"))
            {
                string[] elements = Model.Answers[0].AnswerText.Split('~');
                if (elements.Length == 3)
                {
                    _textInput.Text = elements[0];
                    if (AddressFinderShowCoordinates)
                    {
                        _latitude.Text = elements[1];
                        _longitude.Text = elements[2];
                    }
                    else
                    {
                        _latitudeHidden.Value = elements[1];
                        _longitudeHidden.Value = elements[2];
                    }
                }
                else
	            {
                    _textInput.Text = elements[0];                    
	            }
            }
            else
            _textInput.Text = Model.Answers[0].AnswerText;
        }
        else
        {
            _textInput.Text = Model.Metadata["DefaultText"] ?? string.Empty;
        }
    }

    /// <summary>
    /// Update model
    /// </summary>
    protected override void InlineUpdateModel()
    {
        base.InlineUpdateModel();

        //Set text as answer
        //Get answer from whatever input is visible
        if (_textInput.Visible)
        {
            var longitude = Request.Form[AddressFinderShowCoordinates ? _longitude.UniqueID : _longitudeHidden.UniqueID].Trim();
            var latitude = Request.Form[AddressFinderShowCoordinates ? _latitude.UniqueID : _latitudeHidden.UniqueID].Trim();
            var text = Request.Form[_textInput.UniqueID];
            UpsertTextAnswer(string.Format("{0}~{1}~{2}", string.IsNullOrEmpty(text) ? string.Empty : text.Trim(), latitude, longitude));
        }
    }

    /// <summary>
    /// Get answer format from model
    /// </summary>
    /// <returns></returns>
    private AnswerFormat GetAnswerFormat()
    {
        AnswerFormat answerFormat = AnswerFormat.Postal;

        return answerFormat;
    }

    /// <summary>
    /// Set accepted value for inputs, based on format
    /// </summary>
    protected void SetInputProperties()
    {
        //By default, text input is visible and others are hidden
        _textInput.Visible = true;
    }

    /// <summary>
    /// Set associated control id for 508 input
    /// </summary>
    /// <param name="e"></param>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (_textInput.Visible)
        {
            _questionText.SetAssociatedInputId(_textInput.ClientID);
        }
        RegisterScripts();
    }
    
    /// <summary>
    /// Set width of text input
    /// </summary>
    protected void SetInputWidth()
    {
        if (Utilities.IsNotNullOrEmpty(Appearance["Width"]))
        {
            _textInput.Width = Unit.Pixel(int.Parse(Appearance["Width"]));
        }
    }


    /// <summary>
    /// Reorganize controls and/or apply specific styles depending
    /// on item's label position setting.
    /// </summary>
    protected void SetLabelPosition()
    {
        //When label is set to bottom, we need to move controls from the top panel
        // to the bottom panel.  Otherwise, position changes are managed by setting
        // CSS class.
        if ("Bottom".Equals(Appearance["LabelPosition"], StringComparison.InvariantCultureIgnoreCase))
        {
            //Move text controls to bottom
            _bottomAndOrRightPanel.Controls.Add(_textContainer);

            //Move input to top
            _topAndOrLeftPanel.Controls.Add(_inputPanel);
        }
        
        //Set css classes
        _topAndOrLeftPanel.CssClass = "topAndOrLeftContainer label" + (Utilities.IsNotNullOrEmpty(Appearance["LabelPosition"]) ? Appearance["LabelPosition"] : "Top");
        _bottomAndOrRightPanel.CssClass = "bottomAndOrRightContainer inputForLabel" + (Utilities.IsNotNullOrEmpty(Appearance["ItemPosition"]) ? Appearance["ItemPosition"] : "Top");
    }

    /// <summary>
    /// Set item position.
    /// </summary>
    protected void SetItemPosition()
    {
        _containerPanel.CssClass = "itemContainer itemPosition" + (Utilities.IsNotNullOrEmpty(Appearance["ItemPosition"]) ? Appearance["ItemPosition"] : "Left");

        if ("center".Equals(Appearance["ItemPosition"], StringComparison.InvariantCultureIgnoreCase))
        {
            _contentPanel.Style[HtmlTextWriterStyle.Display] = "inline-block";
        }
    }

    /// <summary>
    /// Registers an include of Address Finder API 
    /// </summary>
    protected void RegisterScripts()
    {
        RegisterClientScriptInclude("IncludeAddressFinder", "http://www.addressfinder.co.nz/assets/v2/widget.js");
    }

    /// <summary>
    /// Show or hide input fields with coordinates
    /// </summary>
    protected bool AddressFinderShowCoordinates
    {
        get
        {
            return false;
        }
    }

    /// <summary>
    /// Returns a region
    /// </summary>
    protected string Region
    {
        get
        {
            return Model.Metadata["Region"];
        }
    }

    /// <summary>
    /// Returns a search type
    /// </summary>
    protected string SearchType
    {
        get
        {
            return Model.Metadata["SearchType"];
        }
    }

    /// <summary>
    /// Returns a search rule
    /// </summary>
    protected string Rule
    {
        get
        {
            return Model.Metadata["Rule"] == "strict" ? "1" : "0";
        }
    }
    
    /// <summary>
    /// Returns a rural mode
    /// </summary>
    protected string Rural
    {
        get
        {
            switch (Model.Metadata["Rural"])
            {
                case "rural_only": 
                    return "1";
                case "urban_only":
                    return "0";
            }
            //both
            return "";
        }
    }
   
</script>