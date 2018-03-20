using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkbox.Forms.Workflow.RSM
{
    /// <summary>
    /// Actions for Response State Machine
    /// </summary>
    public enum RSMAction
    {
        Start, //starts a response state machine
        Save, //saves the progress
        Edit, //starts editing the response
        Resume, //resumes saved response
        Backward, //moves one page back
        Forward, //conducts state machine to Finished state
        UpdateConditions //recalculates rules for the current page
    }
}
