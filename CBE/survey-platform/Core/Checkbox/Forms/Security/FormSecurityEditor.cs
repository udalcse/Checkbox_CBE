using System;
using System.Collections.Generic;
using Checkbox.Security;
using Prezza.Framework.Security;

namespace Checkbox.Forms.Security
{
    /// <summary>
    /// Summary description for FormSecurityEditor.
    /// </summary>
    public class FormSecurityEditor : AccessControllablePDOSecurityEditor
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="form">Form to edit.</param>
        public FormSecurityEditor(IAccessControllable form)
            : base(form)
        {
        }

        /// <summary>
        /// Require form administer permission
        /// </summary>
        protected override string RequiredEditorPermission { get { return "Form.Administer"; } }


        /// <summary>
        /// Grant entities specific access.  Merging with existing permissions
        /// </summary>
        /// <param name="pendingEntries"></param>
        /// <param name="newPermissions"></param>
        /// <param name="remove"></param>
        protected void UpdateAccess(IAccessControlEntry[] pendingEntries, string[] newPermissions, bool remove)
        {
            if (!AuthorizationProvider.Authorize(CurrentPrincipal, ControllableResource, RequiredEditorPermission))
            {
                throw new AuthorizationException();
            }

            // note:  it doesn't matter if the calling user isn't in a role with this permission, their role check will restrict them 
            // first.
            foreach (IAccessControlEntry entry in pendingEntries)
            {
                // get the IAccesPermissible to enter
                IAccessPermissible permissible = GetPermissbleEntity(entry);

                if (permissible != null)
                {
                    //If the entry is on the ACL, add permissions if it is not already
                    Policy policy = ControllableResource.ACL.GetPolicy(permissible);

                    //Create a policy if necessary
                    if (policy == null)
                    {
                        policy = ControllableResource.CreatePolicy(newPermissions);
                        ControllableResource.ACL.Add(permissible, policy);
                    }
                    else
                    {
                        bool updatedPermissions = false;
                        List<string> currentPermissions = new List<string>(policy.Permissions);

                        //Add missing permissions
                        foreach (string newPermission in newPermissions)
                        {
                            if (remove && currentPermissions.Contains(newPermission))
                            {
                                currentPermissions.Remove(newPermission);
                                updatedPermissions = true;
                            }
                            else if (!remove && !currentPermissions.Contains(newPermission))
                            {
                                currentPermissions.Add(newPermission);
                                updatedPermissions = true;
                            }
                        }

                        if (updatedPermissions)
                        {
                            ControllableResource.ACL.Delete(permissible);
                            ControllableResource.ACL.Add(permissible, ControllableResource.CreatePolicy(currentPermissions.ToArray()));
                        }
                    }
                }
            }

            //Update ACL
            SaveAcl();
        }

        /// <summary>
        /// Get a
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public virtual List<IAccessControlEntry> GetMergedAccessPermissible(params string[] permissions)
        {
            //If necessary, merge users and groups on ACL that DON'T have the specified permissions

            //Get list of entries that can be added to acl by removing entries already only the acl
            // from the list of permissible entries.
            List<IAccessControlEntry> permissibleEntries = base.GetAccessPermissible(permissions);

            //Find entries that are already on the acl, but don't have any of the desired permissions
            if (permissions != null)
            {
                var allEntries = new List<IAccessControlEntry>(List(null));

                //Find entries that have none of the specified permissions
                permissibleEntries.AddRange(
                    allEntries.FindAll(entry => entry.Policy != null && !entry.Policy.HasAtLeastOnePermission(permissions))
                 );
            }

            return permissibleEntries;
        }
        
        /// <summary>
        /// Save form acl and mark template as updated
        /// </summary>
        public override void SaveAcl()
        {
            base.SaveAcl();

            MarkTemplateUpdated();
        }

        /// <summary>
        /// Set form default policy and mark as updated
        /// </summary>
        /// <param name="p"></param>
        public override void SetDefaultPolicy(Policy p)
        {
            base.SetDefaultPolicy(p);

            ControllableResource.DefaultPolicy.Permissions.Clear();
            ControllableResource.DefaultPolicy.Permissions.AddRange(p.Permissions);

            MarkTemplateUpdated();
        }

        /// <summary>
        /// Mark the response template as updated
        /// </summary>
        protected void MarkTemplateUpdated()
        {
            int? templateId = null;

            if (ControllableResource is ResponseTemplate)
            {
                templateId = ((ResponseTemplate)ControllableResource).ID;
            }
            else if (ControllableResource is LightweightResponseTemplate)
            {
                templateId = ((LightweightResponseTemplate)ControllableResource).ID;
            }

            if (templateId.HasValue)
            {
                ResponseTemplateManager.MarkTemplateUpdated(templateId.Value);
            }
        }
    }
}
