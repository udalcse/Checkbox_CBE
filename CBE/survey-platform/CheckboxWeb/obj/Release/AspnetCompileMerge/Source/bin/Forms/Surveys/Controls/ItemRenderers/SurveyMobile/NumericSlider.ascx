<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="NumericSlider.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.SurveyMobile.NumericSlider" %>

<% if (!ShowValue) { %>
<style type="text/css">
    #sliderPanel_<%= ClientID %> [type="number"] { display: none; }
    #sliderPanel_<%= ClientID %> .ui-slider-track { margin: 0;}
    #sliderPanel_<%= ClientID %> .left-value-label { margin-left: 10px; }
</style>
<% } %>

<ckbx:ResolvingScriptElement runat="server" Source="~/Resources/jquery.livequery.js" />

<script type="text/javascript">
    var timer_<%= ClientID %>;
    $(document).ready(function () { 
        $(document).on('change input', '#numericSlider_<%= ClientID %>', function () {
            $('.ui-tooltip').hide();

            var val = $(this).val();
            if (timer_<%= ClientID %>) 
                clearTimeout(timer_<%= ClientID %>);

            timer_<%= ClientID %> = setTimeout(function () {
                $("#<%=_currentValue.ClientID %>").val(val).change();                
            }, 800);
        });

        //trick to prevent jquery-ui tooltip loading
        $('.ui-tooltip').livequery(function() {
            var tooltipId = $('#sliderPanel_<%= ClientID %> .ui-slider-handle').attr('aria-describedby');
            if (tooltipId == $(this).attr('id'))
                $(this).hide();
        });
    });

</script>

<div class="ui-slider-container mobile-numeric-slider-container" id="sliderPanel_<%= ClientID %>">
    <div style="display: inline-block; width: 100%;">
        <span style="float: left; <%= (ShowValue ? "margin-left: 60px;" : "") %>" class="left-value-label"><%= MinValue %></span>
        <span style="float: right; margin-right: 10px;"><%= MaxValue %></span>        
    </div>
    <div>
        <input type="range" data-popup-enabled="false" data-show-value="false" name="numericSlider_<%= ClientID %>" id="numericSlider_<%= ClientID %>" 
            value="<%= AnsweredValue %>" min="<%= MinValue %>" max="<%= MaxValue %>" step="<%=StepSize %>" readonly="true"  />
    </div>

    <div style="display:none;" <% if(Model.IsSPCArgument) {%>spcmarker="true"<%} %>>
        <asp:TextBox ID="_currentValue" runat="server" />
        <span class="sliderDefaultValues" dataDefaultValue="<%= DefaultValue %>" dataMaxValue="<%= MaxValue %>" />
    </div>
</div>        


<script type="text/C#" runat="server">
    /// <summary>
    /// Initialize the control
    /// </summary>
    /// <param name="e"></param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        _currentValue.Text = AnsweredValue;
    } 
    
    /// <summary>
    /// 
    /// </summary>
    protected string AnsweredValue
    {
        get
        {
            if (Model != null)
            {
                return Model.Answers.Length > 0 ? Model.Answers[0].AnswerText : DefaultValue.ToString();
            }
            return "0";
        }
    }
</script>
