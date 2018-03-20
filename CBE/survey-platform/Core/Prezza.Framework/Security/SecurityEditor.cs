//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

using System;
using System.Collections.Generic;
using Prezza.Framework.Common;
using Prezza.Framework.Security.Principal;

namespace Prezza.Framework.Security
{
    ///<summary>
    ///Delegate for retrieving a list of access permissible objects that may be added to an ACL
    ///</summary>
    ///<param name="entryData"></param>
    public delegate IAccessPermissible PermissibleResourceDelegate(IAccessControlEntry entryData);

    /// <summary>
    /// Summary description for SharingController.
    /// </summary>
    public abstract class SecurityEditor
    {
        /// <summary>
        /// Callback for retrieving entities that can be added to the ACL.
        /// </summary>
        public PermissibleResourceDelegate GetPermissible;

        /// <summary>
        /// Authorization provider for the resource being edited.
        /// </summary>
        private readonly IAuthorizationProvider _authProvider;

        /// <summary>
        /// The IAccessControllable resource controlled by this SecurityEditor
        /// </summary>
        private readonly IAccessControllable _resource;

        /// <summary>
        /// Current principal
        /// </summary>
        private ExtendedPrincipal _currentPrincipal;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="resource">an <see cref="IAccessControllable"/> object to edit security on</param>
        protected SecurityEditor(IAccessControllable resource)
        {
            ArgumentValidation.CheckForNullReference(resource, "resource");
            _resource = resource;
            _authProvider = AuthorizationFactory.GetAuthorizationProvider();
        }

        /// <summary>
        /// Gets a list of permissible entities for selection into the resource ACL
        /// </summary>
        /// <returns>a table of available entities</returns>
        public abstract List<IAccessControlEntry> GetAccessPermissible(params string[] permissions);


        ///<summary>
        /// Gets a list of permissible entities for selection into the resource ACL
        ///</summary>
        ///<param name="provider"></param>
        ///<param name="currentPage"></param>
        ///<param name="pageSize"></param>
        ///<param name="filterValue"></param>
        ///<param name="permissions"></param>
        ///<returns></returns>
        public abstract List<IAccessControlEntry> GetAccessPermissible(string provider, int currentPage, 
                                                                       int pageSize, string filterValue,
                                                                       params string[] permissions);

        /// <summary>
        /// Initialize the editor with the current principal
        /// </summary>
        /// <param name="currentPrincipal"></param>
        public void Initialize(ExtendedPrincipal currentPrincipal)
        {
            _currentPrincipal = currentPrincipal;
        }

        /// <summary>
        /// Get the current principal
        /// </summary>
        public ExtendedPrincipal CurrentPrincipal
        {
            get
            {
                if (_currentPrincipal == null)
                {
                    throw new Exception("Security Editor not initialized with a user principal.");
                }

                return _currentPrincipal;
            }
        }

        /// <summary>
        /// Get the controllable resource
        /// </summary>
        public IAccessControllable ControllableResource
        {
            get { return _resource; }
        }

        /// <summary>
        /// Get the authorization provider
        /// </summary>
        public IAuthorizationProvider AuthorizationProvider
        {
            get { return _authProvider; }
        }

        /// <summary>
        /// Get the permission required to edit this object
        /// </summary>
        protected abstract string RequiredEditorPermission { get; }

        /// <summary>
        /// Save the resource's ACL
        /// </summary>
        public virtual void SaveAcl()
        {
            if (ControllableResource != null
                && ControllableResource.ACL != null)
            {
                ControllableResource.ACL.Save();
            }
        }

        /// <summary>
        /// Set the resource's Default Policy
        /// </summary>
        public abstract void SetDefaultPolicy(Policy p);

        /// <summary>
        /// Lists the Entries in the AccessControlList
        /// </summary>
        /// <returns>A DataTable of entry information</returns>
        public virtual List<IAccessControlEntry> List(params string[] permissions)
        {
            if (!AuthorizationProvider.Authorize(CurrentPrincipal, ControllableResource, RequiredEditorPermission))
            {
                throw new AuthorizationException();
            }

            if (permissions == null || permissions.Length == 0 || (permissions.Length == 1 && (permissions[0] == null || permissions[0].Trim() == string.Empty)))
            {
                return ControllableResource.ACL.SelectAll();
            }

            return ControllableResource.ACL.SelectAnd(permissions);
        }

        /// <summary>
        /// Performs validation operations on the pending additions to the AccessControlList.
        /// </summary>
        /// <remarks>Validation is implemented by inheritors of SecurityEditor and will differ depending on the 
        /// requirements of the implementation. 
        /// </remarks>
        /// <param name="pendingEntries">a list of new <see cref="IAccessControlEntry"/> to validate</param>
        /// <returns>true if valid; otherwise false</returns>
        public virtual bool Validate(IAccessControlEntry[] pendingEntries)
        {
            if (!AuthorizationProvider.Authorize(CurrentPrincipal, ControllableResource, RequiredEditorPermission))
            {
                throw new AuthorizationException();
            }

            return true;
        }

        /// <summary>
        /// Replace access for specified entries with permissions
        /// </summary>
        /// <param name="pendingEntries"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public virtual void ReplaceAccess(IAccessControlEntry[] pendingEntries, string[] permissions)
        {
            GrantAccess(pendingEntries, permissions);
        }

        /// <summary>
        /// Replace access for specified entries with permissions
        /// </summary>
        /// <param name="permissibleEntity"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public virtual void ReplaceAccess(IAccessPermissible permissibleEntity, string[] permissions)
        {
            if (!AuthorizationProvider.Authorize(CurrentPrincipal, ControllableResource, RequiredEditorPermission))
            {
                throw new AuthorizationException();
            }

            //Don't use RemoveAccess, GrantAccess which make individual permissions checks.  This could cause problems
            // if the current principal is editing it's own policy. 

            //Remove access
            if (permissibleEntity != null)
            {
                ControllableResource.ACL.Delete(permissibleEntity);
            }

            //Grant access
            if (permissibleEntity != null)
            {
                ControllableResource.ACL.Add(
                    permissibleEntity,
                    ControllableResource.CreatePolicy(permissions));
            }
        }

        /// <summary>
        /// Grants selected entries permissions on the resource object
        /// </summary>
        /// <param name="pendingEntries"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public virtual void GrantAccess(IAccessControlEntry[] pendingEntries, params string[] permissions)
        {
            if (!AuthorizationProvider.Authorize(CurrentPrincipal, ControllableResource, RequiredEditorPermission))
            {
                throw new AuthorizationException();
            }

            string[] policyPermissions = new string[] {};

            if (permissions.Length > 0)
            {
                policyPermissions = permissions;
            }
            else if (ControllableResource.DefaultPolicy != null)
            {
                policyPermissions = ControllableResource.DefaultPolicy.Permissions.ToArray();
            }

            foreach (IAccessControlEntry entry in pendingEntries)
            {
                GrantAccess(GetPermissible(entry), policyPermissions);
            }
        }

        /// <summary>
        /// Grants selected entries permissions on the resource object
        /// </summary>
        /// <param name="permissibleEntity"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public virtual void GrantAccess(IAccessPermissible permissibleEntity, string[] permissions)
        {
            if (!AuthorizationProvider.Authorize(CurrentPrincipal, ControllableResource, RequiredEditorPermission))
            {
                throw new AuthorizationException();
            }

            if (permissibleEntity != null)
            {
                ControllableResource.ACL.Delete(permissibleEntity);
                
                ControllableResource.ACL.Add(
                    permissibleEntity, 
                    ControllableResource.CreatePolicy(permissions));
            }
        }

        /// <summary>
        /// Remove selected entries permissions on the resource object
        /// </summary>
        /// <param name="pendingEntries"></param>
        /// <returns></returns>
        public virtual void RemoveAccess(IAccessControlEntry[] pendingEntries)
        {
            if (!AuthorizationProvider.Authorize(CurrentPrincipal, ControllableResource, RequiredEditorPermission))
            {
                throw new AuthorizationException();
            }

            foreach (IAccessControlEntry entry in pendingEntries)
            {
                RemoveAccess(GetPermissible(entry));
            }
        }

        /// <summary>
        /// Remove selected entries permissions on the resource object
        /// </summary>
        /// <param name="permissibleEntity"></param>
        /// <returns></returns>
        public virtual void RemoveAccess(IAccessPermissible permissibleEntity)
        {
            if (!AuthorizationProvider.Authorize(CurrentPrincipal, ControllableResource, RequiredEditorPermission))
            {
                throw new AuthorizationException();
            }

            if (permissibleEntity != null)
            {
                ControllableResource.ACL.Delete(permissibleEntity);
            }
        }
    }
}