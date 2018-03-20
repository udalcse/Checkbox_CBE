<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="NumericSlider.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.NumericSlider" %>
<%@ Import Namespace="Checkbox.Web" %>


<script type="text/javascript">
    $(document).ready(function () {
        $('.sliderContainer<%=Model.ItemId%> #<%=sliderPanel.ClientID %>').slider({
            min: <%=MinValue %>,
            max: <%=MaxValue %>,
            step: <%=StepSize %>,
            tooltip: false,
            value: <%=_currentValue.Text %>,
            orientation:'<%=Orientation %>', 
            slide : function(event, ui){
              <%if(ShowValue)
              {%>
                $("#selectedValue_<%=Model.ItemId %>").val(ui.value);
              <%
              }%>
                $("#<%=_currentValue.ClientID %>").val(ui.value);
              },
            change:function(event, ui){       
                $("#<%=_currentValue.ClientID %>").change();
            }
        });

        $("a.ui-slider-handle.ui-state-default.ui-corner-all").attr('uframeIgnore','true');
    });
</script>
<div style="position:relative" class="sliderContainer<%=Model.ItemId%>">
    <% if (Orientation == "horizontal" && ShowValue) {%>
        <div class="slider-value-wrapper">
            <input id="selectedValue_<%=Model.ItemId %>" readonly="true" class="sliderSelectedValue" value="<%=CurrentValue%>" />
        </div>
    <% } %>
    <div style="width:<%= Width %>; float:left;" class="ui-slider-container <%= (ShowValue ? "" : "numeric-slider-margin") %>">
        <div style="float:left;">
            <%=Orientation == "horizontal" ? MinValue : MaxValue %>
        </div>

        <% if (Orientation == "horizontal") {%>
                <div style="float:right;">
                    <%=MaxValue %>
                </div>
        <% } %>
        <div style="clear:both"></div>
        <asp:Panel ID="sliderPanel" runat="server"/>

           <% if (Orientation == "vertical") {%>
                <div style="">
                    <%= MinValue %>
                    <div id="selectedValue_<%=Model.ItemId %>" style="font-weight:bold;">
                    <%if(ShowValue)
                        {%>
                            <%=CurrentValue %>
                        <%
                        }%>
                    </div>
                </div>
           <% } %>
    </div>
</div>

<div style="display:none;" <% if(Model.IsSPCArgument) {%>spcmarker="true"<%} %>>
    <asp:TextBox ID="_currentValue" runat="server" />
    <div class="sliderDefaultValues" dataDefaultValue="<%= DefaultValue %>" dataMaxValue="<%= MaxValue %>" />
</div>

<script type="text/C#" runat="server">
    /// <summary>
    /// Initialize the control
    /// </summary>
    /// <param name="e"></param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        
        _currentValue.Text = CurrentValue;
    } 
    
    /// <summary>
    /// 
    /// </summary>
    protected string CurrentValue
    {
        get
        {
            return Model.Answers.Length > 0 ? Model.Answers[0].AnswerText : DefaultValue.ToString();        
        }
    }
</script>