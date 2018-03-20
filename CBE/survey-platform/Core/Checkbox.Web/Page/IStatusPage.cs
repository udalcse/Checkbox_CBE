using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Checkbox.Web.Page
{
    public interface IStatusPage
    {
        void WireStatusControl(Control sourceControl);
        void WireUndoControl(Control sourceControl);
        void ShowStatusMessage(String message, StatusMessageType messageType);
        void ShowStatusMessage(String message, StatusMessageType messageType, String actionText, String actionArgument);
    }

    public enum StatusMessageType { Success, Warning, Error };
}
