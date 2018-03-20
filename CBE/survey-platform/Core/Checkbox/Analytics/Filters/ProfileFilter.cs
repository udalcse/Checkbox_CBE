using Checkbox.Analytics.Data;
using Checkbox.Forms;
using Checkbox.Security.Principal;
using Checkbox.Users;
using Checkbox.Common;

namespace Checkbox.Analytics.Filters
{
    /// <summary>
    /// Filter based on respondent profile information
    /// </summary>
    public class ProfileFilter : ResponseFilter
    {
        /// <summary>
        /// Evaluate the filter
        /// </summary>
        /// <param name="answer"></param>
        /// <param name="answerData"> </param>
        /// <param name="responseProperties"></param>
        /// <param name="answerHasValue"></param>
        /// <returns></returns>
        public override bool EvaluateFilter(ItemAnswer answer, AnalysisAnswerData answerData, ResponseProperties responseProperties, out bool answerHasValue)
        {
            string profileValueString = null;
            string uniqueIdentifier = responseProperties.GetStringValue("UniqueIdentifier");

            if (Utilities.IsNotNullOrEmpty(uniqueIdentifier))
            {
                CheckboxPrincipal principal = UserManager.GetUserPrincipal(uniqueIdentifier);

                if (principal != null)
                {
                    profileValueString = principal[PropertyName];
                }
            }

            answerHasValue = Utilities.IsNotNullOrEmpty(profileValueString);

            return CompareValue(profileValueString);
        }

        /// <summary>
        /// Filter parameter for user profile properties
        /// </summary>
        public override string FilterParameter
        {
            get
            {
                return "UserPropertiesFilterString";
            }
        }

        /// <summary>
        /// Filter string for profile properties
        /// </summary>
        public override string FilterString
        {
            get
            {
                if (Operator == Forms.Logic.LogicalOperator.IsNull)
                    return string.Format("(CustomUserFieldName = '{0}' AND Value is null)", this.PropertyName);
                else if (Operator == Forms.Logic.LogicalOperator.IsNotNull)
                    return string.Format("(CustomUserFieldName = '{0}' AND Value is not null)", this.PropertyName);
                else if (Operator == Forms.Logic.LogicalOperator.Contains)
                    return string.Format("(CustomUserFieldName = '{0}' AND Value like '%{1}%')", this.PropertyName, this.Value.ToString().Replace("'", ""));
                else if (Operator == Forms.Logic.LogicalOperator.DoesNotContain)
                    return string.Format("(CustomUserFieldName = '{0}' AND not(Value like '%{1}%'))", this.PropertyName, this.Value.ToString().Replace("'", ""));

                return string.Format("(CustomUserFieldName = '{0}' AND Value {2} {1})", this.PropertyName, 
                    this.WrappedValue,
                    OperatorAsString);
            }
        }

    }
}
