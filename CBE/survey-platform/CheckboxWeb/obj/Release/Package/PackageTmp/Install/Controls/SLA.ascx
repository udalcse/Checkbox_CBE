<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SLA.ascx.cs" Inherits="CheckboxWeb.Install.Controls.SLA" %>

<div id="checkboxSla">
    <div class="dialogInstructions">
        <asp:Label runat="server" >
		    Before proceeding, you must view and agree to the Checkbox&reg; Software License Agreement.
	    </asp:Label>
    </div>
    <div class="padding5" >
        <span class="slaLink" >
            View the Checkbox&reg; Software License Agreement
        </span>
    </div>
    <div class="input padding5">
        <input id="authorizedToAgree" type="checkbox" />
        <label for="authorizedToAgree">I am authorized by my organization to agree to the terms of the License Agreement</label><br/>
        
        <input id="agreeToTerms" type="checkbox" />
        <label for="agreeToTerms">I agree to the terms of the License Agreement</label>

        <input id="isLinkClicked" type="hidden" Value="false" />
    </div>
    <div>&nbsp;</div>
</div>

<script type="text/javascript">
    $(function () {
        <%if (HideCloseButtonInHeader){%>
        $('.simplemodal-close').hide();
        <%}%>

        $('.slaLink').on('click', function () {
            window.open('http://www.checkbox.com/support/checkbox-server-software-license-agreement/', 'sla');
            $('#isLinkClicked').val('true');
            checkOkButtonVisibility();
        });

        $('#checkboxSla input').on('change', function () {
            checkOkButtonVisibility();
        });
    });

    function checkOkButtonVisibility() {
        if (validateSLA()) 
            $('.sla-ok').show();
        else 
            $('.sla-ok').hide();
    }

    function validateSLA() {
        return $('#authorizedToAgree:checked').length > 0
            && $('#agreeToTerms:checked').length > 0
            && $('#isLinkClicked').val() == 'true';
    }
</script>