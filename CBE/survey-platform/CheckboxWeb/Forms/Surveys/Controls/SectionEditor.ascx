<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="SectionEditor.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.SectionEditor" %>


<div class ="section-editor-container">
    <div><asp:CheckBox runat="server" ID="_reportableSection" Text="Reportable section" CssClass="item-editor-content reportable-section"/></div>
    <div id="dual-box-container">
        <asp:HiddenField runat="server" ID="_sectionEditorValues"  > </asp:HiddenField>
        <asp:ListBox multiple="multiple"  SelectionMode="Multiple" id="_sectionDualBox" runat="server"  ></asp:ListBox>
    </div>
</div>


<script type="text/javascript">

    $(function () {

        var dualBoxSelector = "#dual-box-container";

        if (!$("[id*='_reportableSection']").is(':checked')) {
            $(dualBoxSelector).hide();
        }

        $(document).on("change", "[id*='_reportableSection']", function (event) {
            event.stopPropagation();
            (this.checked === true) ? $(dualBoxSelector).show() : $(dualBoxSelector).hide();
        });

        //init dual box control
        $('[id*="_sectionDualBox"]').lwMultiSelect({
            selectedLabel: "Values accepted",
            hiddenInput : "_sectionEditorValues"
        });

    });

</script>