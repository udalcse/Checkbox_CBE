using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkbox.Forms.Workflow.SSM
{
    [Serializable]
    public enum SSMState
    {
        Initial,
        RespondentRequired,
        LanguageRequired,
        PasswordRequired,
        Authorize,
        SelectResponse,
        HandleResponse,
        CompleteResponse,
        Stopped,
        PostPage,
        SavedProgress
    }
}
