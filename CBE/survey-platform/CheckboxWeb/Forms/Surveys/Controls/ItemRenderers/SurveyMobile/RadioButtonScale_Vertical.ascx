<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="RadioButtonScale_Vertical.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.SurveyMobile.RadioButtonScale_Vertical" %>

<% var options = GetNonOtherOptions().ToList(); %>

<div id="<%= ClientID %>_ratingScaleItem" class="rating-scale-mobile">
    <table>
        <tr>
            <td>
                <table class="<%= VerticalSeparator ? "rating-scale-options-separator" : string.Empty %>">
                <%
                    int optionsCount = options.Count();
                    
                    for(int i=0; i<optionsCount; i++) {
                        var option = options.ElementAt(i);
                        int optionId = option.OptionId; %>                       
                        <tr>
                            <td>
                                <div >         
                                    <% if (option.IsSelected) { %>
                                    <input type="radio" checked="checked" name="<%= UniqueID + "_rating_scale" %>" value="<%= optionId %>" id="<%= optionId %>" />                                       
                                    <% } else { %>
                                    <input type="radio" name="<%= UniqueID + "_rating_scale" %>" value="<%= optionId %>" id="<%= optionId %>" />
                                    <% } %>
                                    <label for="<%= optionId %>" >
                                        <div class="radioButtonLabel Answer"><%=  option.Text %></div>
                                    </label>
                                </div>
                            </td>
                        </tr>
                    <% } %>
                </table>
            </td>
            <td style="vertical-align: middle; height: 100%">
                <table class="rating-scale-mobile-labels-table">
                    <tr>
                        <td style="vertical-align: top;">
                            <div style="margin-top: 10px;" class="Answer" ><%= Model.InstanceData["StartText"] %></div>
                        </td>
                    </tr>
                    <tr>
                        <td style="vertical-align: middle;">
                            <div class="Answer" ><%= Model.InstanceData["MidText"] %></div>
                        </td>
                    </tr>
                    <tr>
                        <td style="vertical-align: bottom;">
                            <div style="margin-bottom: 10px;" class="Answer" ><%= Model.InstanceData["EndText"] %></div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    
</div>

<% if(OtherOption != null) { %>
    <div id="other">         
        <% if (OtherOption.IsSelected) { %>
        <input type="radio" checked="checked" name="<%= UniqueID + "_rating_scale" %>" value="<%= OtherOption.OptionId %>" id="<%= OtherOption.OptionId %>" />                                       
        <% } else { %>
        <input type="radio" name="<%= UniqueID + "_rating_scale" %>" value="<%= OtherOption.OptionId %>" id="<%= OtherOption.OptionId %>"/>
        <% } %>
        <label for="<%= OtherOption.OptionId %>">
            <div class="radioButtonLabel Answer"><%= Model.InstanceData["NotApplicableText"] %></div>
        </label>
    </div>
<% } %>
    
<script type="text/javascript">
    $(document).delegate('.ui-page', 'pageshow', function () {
        var self = $('#<%= ClientID %>_ratingScaleItem');
        var height = self.height();
        self.find('.rating-scale-mobile-labels-table').height(height);
    });
</script>
    