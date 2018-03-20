using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Checkbox.Users;
using Checkbox.Web.Security;
using Checkbox.Web.UI.Controls.Security;
using Prezza.Framework.Security;

namespace Checkbox.Web.Page
{
    public abstract class FolderSecurityEditorPage : AccessControllableEditor
    {
        /// <summary>
        /// Security editor context
        /// </summary>
        private SecurityEditorData _contextData;

        /// <summary>
        /// Security editor
        /// </summary>
        private SecurityEditor _editor;

        /// <summary>
        /// Get the security editor
        /// </summary>
        protected SecurityEditor Editor
        {
            get
            {
                if (_editor == null)
                {
                    _editor = CreateSecurityEditor();

                    if (_editor != null)
                    {
                        _editor.Initialize(UserManager.GetCurrentPrincipal());
                    }
                }

                return _editor;
            }
        }

        /// <summary>
        /// Get the security context data
        /// </summary>
        protected SecurityEditorData ContextData
        {
            get
            {
                if (_contextData == null)
                {
                    _contextData = GetSessionValue<SecurityEditorData>("SecurityContextData", true, null);
                }

                return _contextData;
            }
        }

        /// <summary>
        /// Create an instance of the security editor control
        /// </summary>
        /// <returns></returns>
        protected virtual SecurityEditor CreateSecurityEditor()
        {
            return ContextData.ControllableEntity.GetEditor();
        }

        /// <summary>
        /// Get the security editor control for the page
        /// </summary>
        protected abstract SecurityEditorControl EditorControl { get; }

        /// <summary>
        /// Get the page current controllable entity
        /// </summary>
        protected override IAccessControllable GetControllableEntity()
        {
            return ContextData.ControllableEntity;
        }

        /// <summary>
        /// Get permission required to edit acl of controllable entity
        /// </summary>
        protected override string ControllableEntityRequiredPermission
        {
            get { return ContextData.RequiredPermission; }
        }

        /// <summary>
        /// Grant access to entries with the specified permissions.
        /// </summary>
        /// <param name="entries"></param>
        /// <param name="permissions"></param>
        public virtual void GrantAccess(IAccessControlEntry[] entries, params string[] permissions)
        {
            Editor.GrantAccess(entries, permissions);
            Editor.SaveAcl();
        }

        /// <summary>
        /// Remove entries from ACL
        /// </summary>
        /// <param name="entries"></param>
        public virtual void RemoveAccess(IAccessControlEntry[] entries)
        {
            Editor.RemoveAccess(entries);
            Editor.SaveAcl();
        }

        /// <summary>
        /// Page initialization
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            //Init editor control
            EditorControl.Initialize(Editor);
        }

        /// <summary>
        /// Call control's load
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            EditorControl.DoLoad();
        }
    }
}
