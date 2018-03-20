using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Prezza.Framework.Data;
using Prezza.Framework.ExceptionHandling;

namespace Checkbox.Management.Licensing.Limits
{
    /// <summary>
    /// Limit on number of survey editor users
    /// </summary>
    public class SurveyEditorLimit : RolePermissionLimit
    {
		/// <summary>
		/// 
		/// </summary>
		public SurveyEditorLimit() { }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="limitValue"></param>
		public SurveyEditorLimit(string limitValue) : base(limitValue) { }

        /// <summary>
        /// Get the name of the limit.
        /// </summary>
        public override string LimitName
        {
            get { return "SurveyEditorLimit"; }
        }

        /// <summary>
        /// Get the name of the permission to check
        /// </summary>
        public override IEnumerable<string> PermissionNames
        {
            get { return new List<string> { "Form.Edit", "Analysis.Administer" }; }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<string> UsersInRolePermissionLimit
        {
            get { return GetCurrentUsersInRole(); }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override string ValueColumnName
        {
            get { return "ContextLimitValue"; }
        }
    }
}
