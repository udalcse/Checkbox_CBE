using System;


namespace Checkbox.Forms.Logic
{
    /// <summary>
    /// Logical operand for profile data
    /// </summary>
    [Serializable]
    public class ProfileOperand : Operand
    {
        private readonly string _profileKey;

        /// <summary>
        /// Construct a profile operand
        /// </summary>
        /// <param name="profileKey">Profile property name.</param>
        public ProfileOperand(string profileKey)
        {
            _profileKey = profileKey;
        }

        /// <summary>
        /// Get the profile property key
        /// </summary>
        public string ProfilePropertyKey
        {
            get{return _profileKey;}
        }

        /// <summary>
        /// Get an operand value
        /// </summary>
        /// <returns></returns>
        protected override object GetValue(Response response)
        {
            if (response.Respondent != null)
            {
                return response.Respondent[ProfilePropertyKey];
            }
            
            return null;
        }
    }
}
