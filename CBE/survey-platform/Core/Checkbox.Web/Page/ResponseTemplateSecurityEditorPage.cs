using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web.Security;
using Prezza.Framework.Security;

namespace Checkbox.Web.Page
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class  ResponseTemplateSecurityEditorPage : ResponseTemplatePage
    {
        /// <summary>
        /// Security editor context
        /// </summary>
        private SecurityEditorData _contextData;

        /// <summary>
        /// Get the security context data
        /// </summary>
        protected SecurityEditorData ContextData
        {
            get
            {
                if (_contextData == null)
                {
                    _contextData = GetSessionValue<SecurityEditorData>("SecurityContextData", false, null);
                }

                //Create new context data if not posting page back
                if (_contextData == null || !Page.IsPostBack)
                {
                    _contextData = new SecurityEditorData
                    {
                         SecuredResourceId = ResponseTemplate.ID.Value,
                         SecuredResourceType = SecuredResourceType.Survey,
                         RequiredPermission = "Form.Administer",
                         SecuredResourceDefaultPolicyId = ResponseTemplate.DefaultPolicyID.Value,
                         SecuredResourceName = ResponseTemplate.Name
                    };

                    Session["SecurityContextData"] = _contextData;
                }

                return _contextData;
            }
        }

        /// <summary>
        /// Get permission required to edit acl of controllable entity
        /// </summary>
        protected override string ControllableEntityRequiredPermission
        {
            get { return ContextData.RequiredPermission; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override string PageSpecificTitle
        {
            get { return string.Empty; }
        }
    }
}
