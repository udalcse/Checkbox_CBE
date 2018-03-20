using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkbox.Forms.Workflow.RSM
{
    /// <summary>
    /// Response State Machine States
    /// </summary>
    public enum RSMState 
    { 
        New, 
        InProgress, 
        Saved, 
        Finished 
    };
}
