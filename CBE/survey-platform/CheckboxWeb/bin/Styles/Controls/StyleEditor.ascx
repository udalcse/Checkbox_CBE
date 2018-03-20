<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="StyleEditor.ascx.cs" Inherits="CheckboxWeb.Styles.Controls.StyleEditor" %>

<div id="styleEditorAccordion" style="display: none; height:auto;">
    <!--<h3 class="dashStatsHeader border999"><span class="left mainStats">Fonts and Colors</span></h3>-->
    <div id="fontsColorsPlace"></div>
    <h3 class="dashStatsHeader border999" style="margin-top:2px;"><span class="left mainStats">Progress Bar and Buttons</span></h3>
    <div id="formControlsPlace" class="border999" style="margin-top:-1px;"></div>
    <h3 class="dashStatsHeader border999 header-footer-menu-item" style="margin-top:2px;"><span class="left mainStats">Header and Footer</span></h3>
    <div id="headerFooterPlace" class="border999" style="margin-top:-1px;"></div>
</div>

<script type="text/javascript">
    var largeExpanded = false;

    $(function () {
        $('div.left-header').remove();
        $(document).on("click", 'h3.dashStatsHeader, #styleEditorAccordion h3.menu_head', function () {
            var $content = $('.pageContent');
            var expandedWidth = 320;
            $content.addClass(forceCollapsedLeftClass);
            $content.addClass('doNotRemoveCollapsedLeftClass');
            if ($(this).hasClass('header-footer-menu-item')) {
                collapsedLeftWidth = 360;
                expandedLeftWidth = 550;
                expandedWidth = 550;
                largeExpanded = true;
                toggleLeftPanel(null, false, false);
            } else if (largeExpanded) {
                collapsedLeftWidth = 60;
                expandedLeftWidth = 360; 
                $content.addClass(leftResizingClass);
                largeExpanded = false;
                toggleLeftPanel(null, false, false);
            }
            $('.leftPanel, .leftPanel .overview .overview-inner, .left-header').width(expandedWidth);
        });
    });
</script>