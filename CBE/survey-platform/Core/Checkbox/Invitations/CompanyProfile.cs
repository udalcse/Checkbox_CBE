using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Checkbox.Management;
using Prezza.Framework.Data;

namespace Checkbox.Invitations
{
    /// <summary>
    /// 
    /// </summary>
    public class CompanyProfile
    {
        private readonly Dictionary<Property, string> _properties = new Dictionary<Property, string>(); 

        public enum Property { FooterCompany, FooterCountry, FooterState, FooterCity, FooterPostCode, FooterAddress1, FooterAddress2, FooterOptOutLink }
        private static readonly Property[] _requiredProperties = { Property.FooterCompany, Property.FooterCountry, Property.FooterCity, Property.FooterPostCode, Property.FooterAddress1 };

        public int? ProfileId { get; private set; }
        public string ProfileName { get; set; }
        public bool IsDefault { get; set; }

        public CompanyProfile()
        {
        }

        public CompanyProfile(int profileId)
        {
            Load(profileId);
        }

        /// <summary>
        /// 
        /// </summary>
        public static Property[] RequiredProperties
        {
            get { return _requiredProperties; }
        }

        public void Load(int profileId)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_CompanyProfile_Get");
            command.AddInParameter("ID", DbType.Int32, profileId);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        ProfileId = profileId;
                        ProfileName = DbUtility.GetValueFromDataReader(reader, "ProfileName", string.Empty);
                        IsDefault = DbUtility.GetValueFromDataReader(reader, "IsDefault", false);

                        _properties[Property.FooterCompany] = DbUtility.GetValueFromDataReader(reader, "Company", string.Empty);
                        _properties[Property.FooterCountry] = DbUtility.GetValueFromDataReader(reader, "Country", string.Empty);
                        _properties[Property.FooterState] = DbUtility.GetValueFromDataReader(reader, "State", string.Empty);
                        _properties[Property.FooterCity] = DbUtility.GetValueFromDataReader(reader, "City", string.Empty);
                        _properties[Property.FooterPostCode] = DbUtility.GetValueFromDataReader(reader, "PostCode", string.Empty);
                        _properties[Property.FooterAddress1] = DbUtility.GetValueFromDataReader(reader, "Address1", string.Empty);
                        _properties[Property.FooterAddress2] = DbUtility.GetValueFromDataReader(reader, "Address2", string.Empty);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
        }

        public void Save()
        {
            if (ApplicationManager.AppSettings.EnableMultiDatabase && !IsValid)
                throw new Exception("Not all required company profile fields are populated");

            Database db = DatabaseFactory.CreateDatabase();

            if (ProfileId.HasValue)
            {
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_CompanyProfile_Update");
                command.AddInParameter("ID", DbType.Int32, ProfileId.Value);
                command.AddInParameter("IsDefault", DbType.Boolean, IsDefault);
                command.AddInParameter("ProfileName", DbType.String, ProfileName);
                command.AddInParameter("Address1", DbType.String, GetProperty(Property.FooterAddress1));
                command.AddInParameter("Address2", DbType.String, GetProperty(Property.FooterAddress2));
                command.AddInParameter("City", DbType.String, GetProperty(Property.FooterCity));
                command.AddInParameter("Company", DbType.String, GetProperty(Property.FooterCompany));
                command.AddInParameter("Country", DbType.String, GetProperty(Property.FooterCountry));
                command.AddInParameter("PostCode", DbType.String, GetProperty(Property.FooterPostCode));
                command.AddInParameter("State", DbType.String, GetProperty(Property.FooterState));

                db.ExecuteNonQuery(command);
            }
            else
            {
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_CompanyProfile_Insert");
                command.AddInParameter("ProfileName", DbType.String, ProfileName);
                command.AddInParameter("IsDefault", DbType.Boolean, IsDefault);
                command.AddInParameter("Address1", DbType.String, GetProperty(Property.FooterAddress1));
                command.AddInParameter("Address2", DbType.String, GetProperty(Property.FooterAddress2));
                command.AddInParameter("City", DbType.String, GetProperty(Property.FooterCity));
                command.AddInParameter("Company", DbType.String, GetProperty(Property.FooterCompany));
                command.AddInParameter("Country", DbType.String, GetProperty(Property.FooterCountry));
                command.AddInParameter("PostCode", DbType.String, GetProperty(Property.FooterPostCode));
                command.AddInParameter("State", DbType.String, GetProperty(Property.FooterState));
                command.AddOutParameter("ID", DbType.Int32, 4);

                db.ExecuteNonQuery(command);

                var id = command.GetParameterValue("ID");

                if (id != null && id != DBNull.Value)
                    ProfileId = (int)id;
            }
        }

        public string GetProperty(Property property)
        {
            return _properties.ContainsKey(property) ? _properties[property] : null;
        }

        public string GetProperty(string property)
        {
            return _properties.FirstOrDefault(p => p.Key.ToString().ToLower() == property.ToLower()).Value;
        }

        public string SetProperty(Property property, string value)
        {
            return _properties[property] = value;
        }

        public bool IsValid
        {
            get
            {
                return _requiredProperties.All(property => !string.IsNullOrEmpty(GetProperty(property)));
            }
        }
    }
}
