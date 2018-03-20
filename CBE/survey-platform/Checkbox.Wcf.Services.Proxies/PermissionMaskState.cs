using System;

namespace Checkbox.Wcf.Services.Proxies
{
    [Serializable]
    public enum PermissionMaskState
    {
        /// <summary>
        /// No masked permissions are selected.
        /// </summary>
        NotSelected = 1,

        /// <summary>
        /// All masked permissions are selected.
        /// </summary>
        Selected = 2,

        /// <summary>
        /// Some masked permissions are selected.
        /// </summary>
        PartiallySelected = 3
    }
}
