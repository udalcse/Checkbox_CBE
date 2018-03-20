using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Prezza.Framework.Data;
using Property = Checkbox.Invitations.CompanyProfile.Property;

namespace Checkbox.Invitations
{
    /// <summary>
    /// Manages company profile data.
    /// 
    /// Currently all the company data being saved into the applicaiton settings, but probably in the future 
    /// they'll be moved into the ckbx_Text and be language sensitive
    /// </summary>
    public static class CompanyProfileFacade
    {
        static string[] _propNames;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string[] ListCompanyPropertyNames()
        {
            if (_propNames == null)
            {
                var list = new List<string>();
                var v = Enum.GetValues(typeof(Property));
                foreach (var vv in v)
                {
                    list.Add(vv.ToString());
                }
                _propNames = list.ToArray();
            }
            return _propNames;            
        }

        /// <summary>
        /// Comparer class
        /// </summary>
        public class StringCaseInsensitiveComparer : IEqualityComparer<string>
        {
            // Summary:
            //     Determines whether the specified objects are equal.
            //
            // Parameters:
            //   x:
            //     The first object of type T to compare.
            //
            //   y:
            //     The second object of type T to compare.
            //
            // Returns:
            //     true if the specified objects are equal; otherwise, false.
            public bool Equals(string x, string y)
            {
                return x.ToLowerInvariant().Equals(y.ToLowerInvariant());
            }
            //
            // Summary:
            //     Returns a hash code for the specified object.
            //
            // Parameters:
            //   obj:
            //     The System.Object for which a hash code is to be returned.
            //
            // Returns:
            //     A hash code for the specified object.
            //
            // Exceptions:
            //   System.ArgumentNullException:
            //     The type of obj is a reference type and obj is null.
            public int GetHashCode(string obj)
            {
                return obj.ToLowerInvariant().GetHashCode();
            }
        }

        static StringCaseInsensitiveComparer _stringCaseInsensitiveComparer;
        
        /// <summary>
        /// Factory property
        /// </summary>
        public static IEqualityComparer<string> PipeComparer
        {
            get
            {
                if (_stringCaseInsensitiveComparer == null)
                {
                    _stringCaseInsensitiveComparer = new StringCaseInsensitiveComparer();
                }

                return _stringCaseInsensitiveComparer;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, string> ListProfiles()
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_CompanyProfile_List");

            var result = new Dictionary<int, string>();

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    int index = 1;
                    while (reader.Read())
                    {
                        var id = DbUtility.GetValueFromDataReader(reader, "ID", -1);
                        var profile = DbUtility.GetValueFromDataReader(reader, "ProfileName", "Profile " + index);

                        result.Add(id, profile);

                        index++;
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static int? GetDefaultCompanyProfileId()
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_CompanyProfile_GetDefault");

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        var id = DbUtility.GetValueFromDataReader(reader, "ID", -1);
                        if (id != -1)
                            return id;
                    }
                }
                finally
                {
                    reader.Close();
                }

            }

            var profiles = ListProfiles();
            if (profiles.Count > 0)
                return profiles.First().Key;

            return null;
        }

    }
}
