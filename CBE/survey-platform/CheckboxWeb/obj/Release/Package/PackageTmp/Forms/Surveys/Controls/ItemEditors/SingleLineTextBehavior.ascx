<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="SingleLineTextBehavior.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.SingleLineTextBehavior" %>
<%@ Register src="~/Controls/Piping/PipeControl.ascx" tagname="PipeSelector" tagprefix="pipe" %>

<script language="javascript">
    var oldFormat = "";
    var formatDropDown = null;

    function IsDateFormatSelected(strFormat) {
        return strFormat.indexOf("Date") > -1;
    }

    function IsNumericFormatSelected(strFormat) {
        return strFormat == "Decimal" || strFormat == "Integer" || strFormat == "Money" || strFormat == "Numeric";
    }

    $.fn.swapMnD = function(oldFormat, newFormat) {
        $(this).each(function(i) {
            var oldValue = "";
            var newValue = "";
            oldValue = $(this).val();
            if (!oldValue || oldValue == "") {
                return $(this);
            }
            var date = $.datepicker.parseDate(oldFormat, oldValue);
            $(this).val($.datepicker.formatDate(newFormat, date));
            return $(this);
        });
        return $(this);
    };

    function swapMonthAndDay(oldFormat, newFormat) {
        $(".datepicker").swapMnD(oldFormat, newFormat);
    };

    function updateDynamicControls() {
        if (IsDateFormatSelected(oldFormat) || IsNumericFormatSelected(oldFormat)) {
            $("#minMaxPlace").show();
            $("#maxLengthPlace").hide();
        }
        else {
            $("#minMaxPlace").hide();
            $("maxLengthPlace").show();
        }
    };

    function setDatePickerOptions(input) {
        input.datepicker("option", "showOn", "both");
        input.datepicker("option", "buttonImageOnly", true);
        input.datepicker("option", "buttonImage", '<%=ResolveUrl("~/Resources/CalendarPopup.png") %>');
        input.datepicker("option", "buttonText", "Calendar");
    };

    $(document).ready(function () {
        $('#<%=_maxLengthTxt.ClientID %>').numeric({ decimal: false, negative: false });

        $("#<%=_textFormatList.ClientID %>").each(function () { formatDropDown = this; });
        oldFormat = formatDropDown.options[formatDropDown.selectedIndex].value;

        updateDynamicControls();

        if (IsDateFormatSelected(oldFormat)) {
            $(".datepicker").datepicker();
            setDatePickerOptions($(".datepicker"));
            $.datepicker.setDefaults($.datepicker.regional[oldFormat == "Date_ROTW" ? "dd/mm/yy" : ""]);
        }

        $(document).off('change', "#<%=_textFormatList.ClientID %>");
        $(document).on('change', "#<%=_textFormatList.ClientID %>", function () {
            var newFormat = formatDropDown.options[formatDropDown.selectedIndex].value;

            if (IsDateFormatSelected(newFormat)) {
                if (!IsDateFormatSelected(oldFormat)) {
                    $(".datepicker").datepicker();
                    setDatePickerOptions($(".datepicker"));
                    $(".datepicker").datepicker("option", "dateFormat", oldFormat == "Date_ROTW" ? "dd/mm/yy" : "mm/dd/yy");
                    $(".datepicker").val('');
                }
                else {
                    if (oldFormat == "Date_ROTW") {
                        $(".datepicker").datepicker("option", "dateFormat", "mm/dd/yy");
                    }
                    else {
                        $(".datepicker").datepicker("option", "dateFormat", "dd/mm/yy");
                    }
                }
                $("#minMaxPlace").show();

                $.uniform.update('.datepicker');
            }
            else {
                if (IsDateFormatSelected(oldFormat)) {
                    $(".datepicker").datepicker("destroy");
                }

                if (IsNumericFormatSelected(newFormat)) {
                    if (!IsNumericFormatSelected(oldFormat)) {
                        $(".datepicker").val('');
                    }
                    $("#minMaxPlace").show();
                }
                else
                    $("#minMaxPlace").hide();
            }

            oldFormat = formatDropDown.options[formatDropDown.selectedIndex].value;
            updateDynamicControls();
        });
    });     
                           
</script>

<div class="formInput left fixed_250">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_textFormatList" ID="_formatLbl" runat="server" TextId="/controlText/singleLineTextEditor/answerFormat" /></p>
</div>
<div class="formInput left">
     <ckbx:MultiLanguageDropDownList ID="_textFormatList" runat="server" AutoPostBack="false">
        <asp:ListItem Text="" Value="None" TextId="/enum/answerFormat/none" />        
        <asp:ListItem Text="" Value="Date_USA" TextId="/enum/answerFormat/dateUS" />
        <asp:ListItem Text="" Value="Date_ROTW" TextId="/enum/answerFormat/dateROTW" />
        <asp:ListItem Text="" Value="AlphaNumeric" TextId="/enum/answerFormat/alphanumeric" />
        <asp:ListItem Text="" Value="Alpha" TextId="/enum/answerFormat/alpha" />
        <asp:ListItem Text="" Value="Decimal" TextId="/enum/answerFormat/decimal" />
        <asp:ListItem Text="" Value="Email" TextId="/enum/answerFormat/email" />
        <asp:ListItem Text="" Value="Integer" TextId="/enum/answerFormat/integer" />
        <asp:ListItem Text="" Value="Money" TextId="/enum/answerFormat/money" />
        <asp:ListItem Text="" Value="Numeric" TextId="/enum/answerFormat/numeric" />
        <asp:ListItem Text="" Value="Postal" TextId="/enum/answerFormat/postal" />
        <asp:ListItem Text="" Value="Phone" TextId="/enum/answerFormat/phone" />
        <asp:ListItem Text="" Value="SSN" TextId="/enum/answerFormat/ssn" />
        <asp:ListItem Text="" Value="Uppercase" TextId="/enum/answerFormat/uppercase" />
        <asp:ListItem Text="" Value="Lowercase" TextId="/enum/answerFormat/lowercase" />
        <asp:ListItem Text="" Value="URL" TextId="/enum/answerFormat/url" />
    </ckbx:MultiLanguageDropDownList>
</div>
<br class="clear"/>

<div id="maxLengthPlace">
<div class="formInput left fixed_250">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_maxLengthTxt" ID="_maxLengthLbl" runat="server" TextId="/controlText/singleLineTextEditor/maxLength" /></p>
</div>
<div class="formInput left">
     <asp:TextBox ID="_maxLengthTxt" runat="server" Width="50px" />
        <asp:Label ID="_maxLengthError" runat="server" CssClass="error message" Visible="false"></asp:Label>
</div>
    </div>

<div id="minMaxPlace">
    <div class="formInput left fixed_250">
        <p><ckbx:MultiLanguageLabel AssociatedControlID="_minValueNumberTxt" ID="_minValueLbl" runat="server" TextId="/controlText/singleLineTextEditor/minValue"/></p>
    </div>
    <div class="formInput left">
      
        <asp:TextBox ID="_minValueNumberTxt" runat="server"  CssClass="datepicker"></asp:TextBox>
        <asp:Label ID="_minValueError" runat="server" CssClass="error message" Visible="false"></asp:Label>
    </div>
    <br class="clear"/>
    
    <div class="formInput left fixed_250">
        <p><ckbx:MultiLanguageLabel AssociatedControlID="_maxValueNumberTxt" ID="_maxValueLbl" runat="server" TextId="/controlText/singleLineTextEditor/maxValue"/></p>
    </div>
    <div class="formInput left">
        <asp:TextBox ID="_maxValueNumberTxt"  runat="server"  CssClass="datepicker"></asp:TextBox>
        <asp:Label ID="_maxValueError" runat="server" CssClass="error message" Visible="false"></asp:Label>
    </div>
    <br class="clear"/>
</div>

<br class="clear" />
<div class="formInput left fixed_250">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_requiredChk" ID="_requiredLbl" runat="server" TextId="/controlText/singleLineTextEditor/required" /></p>
</div>
<div class="formInput left">
    <asp:CheckBox ID="_requiredChk" runat="server" />
</div>
<br class="clear"/>

<div class="formInput left fixed_250">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_aliasText" ID="MultiLanguageLabel1" runat="server" TextId="/controlText/listEditor/alias" /></p>
</div>
<div class="formInput left">
    <asp:TextBox ID="_aliasText" runat="server" />
</div>
<br class="clear"/>

<div class="formInput left fixed_250">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_defaultTextTxt" ID="_defaultValueLbl" Width="250px" runat="server" TextId="/controlText/singleLineTextEditor/defaultText" /></p>
</div>
<div class="formInput left">
    <asp:TextBox ID="_defaultTextTxt" runat="server" CssClass="datepicker"/><asp:Label ID="_defaultValueError" runat="server" CssClass="error message" Visible="false"></asp:Label>
</div>
<br class="clear"/>

<div class="formInput left fixed_250"></div>
<div class="formInput left">
    <pipe:PipeSelector ID="_pipeSelector" runat="server" />
</div>
<br class="clear"/>




