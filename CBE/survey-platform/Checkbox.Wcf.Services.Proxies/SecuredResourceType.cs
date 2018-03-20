using System;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Enumeration of secured resource types that support ACL and Policy editing
    /// </summary>
    [Serializable]
    public enum SecuredResourceType
    {
        Survey = 1,
        Folder,
        Report,
        UserGroup,
        Library,
        EmailList,
        User
    }
}
