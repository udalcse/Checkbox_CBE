<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="SingleLineText.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.LibraryPreview.SingleLineText" %>
<%@ Register TagPrefix="ckbx" TagName="QuestionText" Src="~/Forms/Surveys/Controls/ItemRenderers/SurveyEditor/QuestionText.ascx" %>
<%@ Import Namespace="Checkbox.Management"%>
<%@ Import Namespace="System.Globalization"%>
<%@ Import Namespace="Checkbox.Forms.Items"%>
<%@ Import Namespace="Checkbox.Common"%>
<%@ Import Namespace="Checkbox.Web.Forms.UI.Rendering" %>

<asp:Panel ID="_containerPanel" runat="server" CssClass="itemContainer">
    <script type="text/javascript">
        var initAutocomplete_<%=ClientID%> = function(input) {
            <% if(!string.IsNullOrWhiteSpace(AutocompleteData)) {%>
                    input.autocomplete({
                        source: <%=AutocompleteData%>
                    });
                <% } else if(!string.IsNullOrEmpty(AutocompleteRemote)) {%>
                    input.autocomplete({
                        source: function( request, response ) {
                            $.ajax({
                                url: "<%=AutocompleteRemote%>",
                                dataType: "jsonp",
                                data: {
                                    q: request.term
                                },
                                success: function(data) {
                                    for (var i=0;i<data.length;i++) {
                                        if (!data[i]) {
                                            data.splice(i,1);
                                            i--;
                                        }
                                    }
                                    response(data);
                                },
                                error: function (xhr) {
                                    alert('Request Status: ' + xhr.status + '\nStatus Text: ' + xhr.statusText + '\n' + xhr.responseText);
                                }
                            });
                        },
                        minLength: 2
                    });
                <% } %>
        }

        $(function () {
            $(window).resize(function() {
                $(".ui-autocomplete").hide();
            });
            <%if(_textInput.Visible) {%>
                initAutocomplete_<%=ClientID%>($("#<%=_textInput.ClientID%>"));
            <%}%>
            <%if(_numericInput.Visible) {%>
                initAutocomplete_<%=ClientID%>($("#<%=_numericInput.ClientID%>"));
            <%}%>
            <%if(_maskedInput.Visible) {%>
                initAutocomplete_<%=ClientID%>($("#<%=_maskedInput.ClientID%>"));
            <%}%>
        });
    </script>
    
    <asp:Panel ID="_contentPanel" runat="server" CssClass="itemContent">
        <asp:Panel ID="_topAndOrLeftPanel" runat="server" CssClass="topAndOrLeftContainer">
            <asp:Panel ID="_textContainer" runat="server" CssClass="textContainer">
                <ckbx:QuestionText ID="_questionText" runat="server" />
            </asp:Panel>
        </asp:Panel>

        <asp:Panel ID="_bottomAndOrRightPanel" runat="server" CssClass="bottomAndOrRightContainer">
            <asp:Panel ID="_inputPanel" runat="server">
                <asp:TextBox
                    ID="_textInput" 
                    runat="server" 
                    TextMode="SingleLine"
                />
                    
                <asp:TextBox
                    ID="_numericInput" 
                    runat="server"
                    TextMode="SingleLine"
                    Visible="false" />
                    
                <asp:TextBox
                    ID="_dateInput"
                    runat="server"
                    TextMode="SingleLine"
                    Visible="false"   />
                    
                <asp:ImageButton ID="_datePopup" runat="server" OnClientClick="return false;" />
                              
                <asp:TextBox ID="_maskedInput" runat="server" Visible="false" />
            </asp:Panel>
        </asp:Panel>
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
            _textInput.Text = Model.Answers[0].AnswerText;
        }
        else
        {
            _textInput.Text = Model.Metadata["DefaultText"] ?? string.Empty;
        }

        InitAutocomplete(_textInput);
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
            UpsertTextAnswer(_textInput.Text.Trim());
        }
        else if (_numericInput.Visible)
        {
            UpsertTextAnswer(_numericInput.Text.Trim());
        }
        else if (_dateInput.Visible)
        {
            UpsertTextAnswer(_dateInput.Text.Trim());
        }
        else if (_maskedInput.Visible)
        {
            UpsertTextAnswer(_maskedInput.Text.Trim());
        }
    }

    /// <summary>
    /// Get answer format from model
    /// </summary>
    /// <returns></returns>
    private AnswerFormat GetAnswerFormat()
    {
        AnswerFormat answerFormat = AnswerFormat.None;

        if (Utilities.IsNotNullOrEmpty(Model.Metadata["AnswerFormat"]))
        {
            try
            {
                answerFormat = (AnswerFormat)Enum.Parse(typeof(AnswerFormat), Model.Metadata["AnswerFormat"]);
            }
            catch
            {
            }
        }

        return answerFormat;
    }

    /// <summary>
    /// Set accepted value for inputs, based on format
    /// </summary>
    protected void SetInputProperties()
    {
        //By default, text input is visible and others are hidden
        _textInput.Visible = true;
        _dateInput.Visible = false;
        _datePopup.ImageUrl = ApplicationManager.ApplicationRoot + "/Resources/CalendarPopup.png";
        _datePopup.Visible = false;
        _numericInput.Visible = false;
        _maskedInput.Visible = false;

        AnswerFormat answerFormat = GetAnswerFormat();

        switch (answerFormat)
        {
            //Date Formats
            case AnswerFormat.Date:
                _dateInput.Visible = ApplicationManager.AppSettings.UseDatePicker;
                _datePopup.Visible = _dateInput.Visible;
                _textInput.Visible = !_dateInput.Visible;
                SetDateValues();
                break;
                
            case AnswerFormat.Date_ROTW:
                _dateInput.Visible = ApplicationManager.AppSettings.UseDatePicker;
                _datePopup.Visible = _dateInput.Visible;
                _textInput.Visible = !_dateInput.Visible;
                SetDateValues();
                break;
                
            case AnswerFormat.Date_USA:
                _dateInput.Visible = ApplicationManager.AppSettings.UseDatePicker;
                _datePopup.Visible = _dateInput.Visible;
                _textInput.Visible = !_dateInput.Visible;
                SetDateValues();
                break;
                
            //Masked formats
            case AnswerFormat.Phone:
                _textInput.Visible = false;
                _maskedInput.Visible = true;
                //_maskedInput.Mask = "(###) ### - ####";
                //_maskedInput.PromptChar = "#";
                break;
                
            case AnswerFormat.SSN:
                _textInput.Visible = false;
                _maskedInput.Visible = true;
                //_maskedInput.Mask = "###-##-####";
                //_maskedInput.PromptChar = "#";
                break;
                
            //Numeric formats
            case AnswerFormat.Integer:
                _textInput.Visible = false;
                _numericInput.Visible = true;
                //_numericInput.NumberFormat.DecimalDigits = 0;
                SetNumericValues();
                break;

            case AnswerFormat.Decimal:
            case AnswerFormat.Numeric:
                _textInput.Visible = false;
                _numericInput.Visible = true;
                SetNumericValues();
                break;
                
            //Text formats with JS input limitation in addition to server
            // side validation.
            case AnswerFormat.Alpha:
            case AnswerFormat.AlphaNumeric:
            case AnswerFormat.Lowercase:
            case AnswerFormat.Uppercase:
                SetMaxLength();
                break;
                
            //Other formats have only server-side validation & only need
            // answer values set and length restricted.
            default:
                SetMaxLength();
                break;
        }

        //_questionText.SetAssociatedInputId(inputId);
        _questionText.SetAssociatedInputId(_textInput.ClientID);
    }

    /// <summary>
    /// Set associated control id for 508 input
    /// </summary>
    /// <param name="e"></param>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (_maskedInput.Visible)
        {
            _questionText.SetAssociatedInputId(_maskedInput.ClientID);
        }

        if (_dateInput.Visible)
        {
            _questionText.SetAssociatedInputId(_dateInput.ClientID);
        }
        
        if (_textInput.Visible)
        {
            _questionText.SetAssociatedInputId(_textInput.ClientID);   
        }
    }

    /// <summary>
    /// Set min/max and answer date values
    /// </summary>
    protected void SetDateValues()
    {
        DateTime? answerValueDate = Utilities.GetDate(Model.InstanceData["AnswerValueAsDate"], CultureInfo.InvariantCulture);
        
        //if (minDate.HasValue)
        //{
        //    _datePicker.MinDate = minDate.Value;
        //}

        //if (maxDate.HasValue)
        //{
        //    _datePicker.MaxDate = maxDate.Value;
        //}

        if (answerValueDate.HasValue)
        {
            AnswerFormat answerFormat = GetAnswerFormat();

            string formatString = "d";

            if (answerFormat == AnswerFormat.Date_USA)
            {
                formatString = "MM/dd/yyyy";
            }

            if (answerFormat == AnswerFormat.Date_ROTW)
            {
                formatString = "dd/MM/yyyy";
            }
            
            if (ApplicationManager.AppSettings.UseDatePicker)
            {
                _dateInput.Text = answerValueDate.Value.ToUniversalTime().ToString(formatString, CultureInfo.InvariantCulture);
            }
            else
            {
                _textInput.Text = answerValueDate.Value.ToUniversalTime().ToString(formatString, CultureInfo.InvariantCulture);
            }
        }
    }

    
    /// <summary>
    /// Set min/max and answer values for numeric values
    /// </summary>
    protected void SetNumericValues()
    {
        //double? minNumericValue = Utilities.AsDouble(Model.Metadata["MinNumericValue"]);
        //double? maxNumericValue = Utilities.AsDouble(Model.Metadata["MaxNumericValue"]);
        
        //if (minNumericValue.HasValue)
        //{
            //_numericInput.MinValue = minNumericValue.Value;
        //}

        //if (maxNumericValue.HasValue)
        //{
            //_numericInput.MaxValue = maxNumericValue.Value;
        //}

        _numericInput.Text = Model.Answers.Length > 0
            ? Model.Answers[0].AnswerText
            : Model.InstanceData["DefaultText"];
    }
   
    /// <summary>
    /// Set max length of text
    /// </summary>
    protected void SetMaxLength()
    {
        int? maxLength = Utilities.AsInt(Model.Metadata["MaxLength"]);
        
        if (maxLength.HasValue)
        {
            _textInput.MaxLength = maxLength.Value;
        }
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
   
</script>
