using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkbox.Forms.Workflow.SSM
{
    /// <summary>
    /// Actions for the Session State Machine
    /// </summary>
    public enum SSMAction
    {
        CreateSession,
        SetRespondent,
        LogUserIn,
        SetResponseLanguage,
        SetPassword,        
        SelectResponse,
        ResumeSavedResponse,
        PostPage,
        EditResponse
    }
}
