<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HeatMapDataTab.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors.HeatMapDataTab" %>

<%@ Import Namespace="Checkbox.Web" %>

<div class="formInput">
    <% if (_selectedSections.Items != null && _selectedSections.Items.Any())
        { %>
    <div>
        <div class="heatMapSectionName"><b style="margin-left: 10px;"><%=WebTextManager.GetText("/controlText/chartEditor/heatmapSection") %></b></div>
        <div class="heatMapSectionValue"><b style="padding-left:10px"><%=WebTextManager.GetText("/controlText/chartEditor/eSigma") %></b></div>
        <br class="clear" />
        <ol class="heatMapDataList">
            <asp:ListView ID="_selectedSections" runat="server">
                <LayoutTemplate>
                    <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
                </LayoutTemplate>
                <ItemTemplate>
                     <li id="selectedSections_<%#Eval("Key") %>">
                        <div class="heatMapSectionName"><%# (((KeyValuePair<int, KeyValuePair<string, string>>)Container.DataItem).Value.Key) %></div>
                        <asp:HiddenField ID="sectionId" runat="server" Value='<%# Eval("Key") %>' />
                        <div class="heatMapSectionValue">
                            <asp:TextBox runat="server" CausesValidation="True"  ID="sectionValue"  Value="<%# (((KeyValuePair<int, KeyValuePair<string, string>>)Container.DataItem).Value.Value) %>"></asp:TextBox>
                            <asp:RangeValidator ID="_rowsValidator" ControlToValidate="sectionValue" MinimumValue="1" MaximumValue="10" Type="Double" runat="server"  ErrorMessage="Please enter in a number between 1 and 10." ></asp:RangeValidator>
                        </div>
                        <br class="clear" />
                    </li>
                </ItemTemplate>
            </asp:ListView>
        </ol>
    </div>
    <br class="clear" />
    <br class="clear" />
    <% }
        else
        { %>
        <div> No selected source items </div>
      <br class="clear" />
    <% } %>
    <div class="left">
        <p>
            <label><%=WebTextManager.GetText("/controlText/chartEditor/useMeanValues") %></label></p>
    </div>
    <div class="left checkBox" style="margin-left: 15px;">
        <asp:CheckBox ID="_userMeanValues" runat="server" /></div>
    <br class="clear" />
        <div class="left">
        <p>
            <label><%=WebTextManager.GetText("/controlText/chartEditor/randomizeResponses") %></label></p>
    </div>
    <div class="left checkBox" style="margin-left: 15px;">
        <asp:CheckBox ID="_randomizeResponses" runat="server" /></div>
    <br class="clear" />
    <div id="errorMessage" style="color:red;margin-top:30px;"></div>
    
</div>

<script>
    $(document).on("change", "[id$='_sectionValue'], [id$='_userMeanValues']", function () {
        if (!$("[id$='_userMeanValues']").parent().hasClass("checked")
            && ($("[id$='_rowsValidator'][style*='visible']").length > 0
            || $("[id$='_sectionValue']").filter(function (x, y) { return $(y).val() == "" || parseInt($(y).val()) < 1 || parseInt($(y).val()) > 10 }).length > 0)) {
            $(".saveButton").addClass("disabled");
            $(".saveButton").css("pointer-events", "none");
            $("#errorMessage").text("All eSigma values are required");
            $("[id$='_sectionValue']").each(function (idx, value) {
                if (parseFloat($(value).val()) < 1 || parseFloat($(value).val()) > 10) {
                    $(value).next("span").css("visibility", "visible");
                }
            });
        }
        else {
            $(".saveButton").removeClass("disabled");
            $(".saveButton").css("pointer-events", "auto");
            $("#errorMessage").text("");
            $("[id$='_sectionValue']").next("span").css("visibility", "hidden");
        }
    });

    $(document).ready(function () {
        $("[id$='_sectionValue'], [id$='_userMeanValues']").trigger("change");
    });

    function isNumber(n) {
        return !isNaN(parseFloat(n)) && isFinite(n);
    }
</script>

