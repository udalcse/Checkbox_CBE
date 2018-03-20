var $chatTopbar;
var $chatContainer;
var $chatConversation;
var chatExpandedClass = "olrk-state-expanded";
var chatCompressedClass = "olrk-state-compressed";
var topbarExpandedClass = "habla_topbar_div_expanded";
var topbarCompressedClass = "habla_topbar_div_compressed";
var topbarHiddenClass = "topbar_hidden";
var olarkSetupInterval;

$(document).ready(function () {
    // Set up the OLark chat toggle functionality
    olarkSetupInterval = setInterval(function () {
        initOLarkChat();
    }, 500);

   
});

function initOLarkChat() {
    $chatContainer = $("#habla_window_state_div");
    if ($chatContainer.length > 0) {
        $chatTopbar = $("#habla_topbar_div");
        $chatConversation = $("#habla_conversation_div");

        $("#habla_topbar_div").click(function () {
            toggleOLarkChatClasses();
        });
        $("#habla_sizebutton_a").click(function () {
            toggleOLarkChatClasses();
        });
        $("#habla_oplink_a").click(function () {
            toggleOLarkChatClasses();
        });
        $("#habla_pre_chat_submit_input").click(function () {
            toggleOLarkChatClasses();
        });
        $("#habla_conversation_div").click(function () {
            toggleOLarkChatClasses();
        });
        $("#habla_wcsend_input").focus(function () {
            toggleOLarkChatClasses();
        }).blur(function () {
            toggleOLarkChatClasses();
        }).mouseover(function () {
            toggleOLarkChatClasses();
        }).click(function () {
            toggleOLarkChatClasses();
        });
        
        $(".welcome-header-text").click(function () {
            $("#habla_sizebutton_a").click();
        });

        toggleOLarkChatClasses();

        clearInterval(olarkSetupInterval);
    }
}




// Toggle the correct OLark chat display classes
function toggleOLarkChatClasses() {
    if ($chatContainer.hasClass(chatExpandedClass)) {
        $chatTopbar.addClass(topbarExpandedClass);
        $chatTopbar.removeClass(topbarCompressedClass);

        if ($chatConversation.is(":visible")) {
            $chatTopbar.removeClass(topbarHiddenClass);

            // Alter the display of the initial "start chatting" screen
            if ($("#hbl_body_message").is(":visible")) {
                $chatTopbar.removeClass(topbarHiddenClass);
                $("#habla_oplink_a").html("Start Chatting");
            }
        }
        else {
            $chatTopbar.addClass(topbarHiddenClass);
        }
    }
    else {
        $chatTopbar.removeClass(topbarExpandedClass);
        $chatTopbar.addClass(topbarCompressedClass);
    }
}