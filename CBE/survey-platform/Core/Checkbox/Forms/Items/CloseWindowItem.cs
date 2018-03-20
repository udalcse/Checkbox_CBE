using System;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Simple survey item to close a browser window.  There is no business logic, simply
    /// an associated renderer that emits Javascript.
    /// </summary>
    [Serializable]
    public class CloseWindowItem : ResponseItem
    {
    }
}
