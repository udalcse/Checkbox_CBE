using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Checkbox.Management;

namespace Checkbox.Invitations
{
    /// <summary>
    /// 
    /// </summary>
    public class InvitationEmailTextValidator
    {
        public enum ErrorType { Success, SurveyURLMissed, FooterMissed, OptOutLinkMissed }

        /// <summary>
        /// Validates invitation text
        /// </summary>
        /// <param name="invitationText"></param>
        /// <returns></returns>
        public static ErrorType ValidateInvitationText(string invitationText, 
            out List<CompanyProfile.Property> missedCompanyParameters, 
            bool isOptOutLinkRequired, bool fullValidation = true)
        {
            ErrorType res = ErrorType.Success;
            missedCompanyParameters = new List<CompanyProfile.Property>();

            if (fullValidation)
            {
                if (!invitationText.Contains("SURVEY_URL_PLACEHOLDER__DO_NOT_ERASE"))
                    return ErrorType.SurveyURLMissed;
            }

            if (ApplicationManager.AppSettings.FooterEnabled)
            {
                foreach (var prop in CompanyProfile.RequiredProperties)
                {
                    if (!invitationText.Contains(ApplicationManager.AppSettings.PipePrefix + prop))
                    {
                        missedCompanyParameters.Add(prop);
                        res = ErrorType.FooterMissed;
                    }
                }

                if (res != ErrorType.Success)
                    return res;
            }

            if (isOptOutLinkRequired)
            {
                //add unsubscribe-link placeholder
                var regex = new Regex("<a\\s([^>]*\\s)?href=\"" + 
                    InvitationManager.OPT_OUT_URL_PLACEHOLDER + "\"(.*?)>");

                if (!regex.IsMatch(invitationText) && !invitationText.Contains(InvitationManager.OPT_OUT_URL_PLACEHOLDER))
                    return ErrorType.OptOutLinkMissed;
            }

            return res;
        }
    }
}
