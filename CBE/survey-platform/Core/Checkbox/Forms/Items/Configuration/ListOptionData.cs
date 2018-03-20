using System;
using System.Data;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Meta data for list options in surveys
    /// </summary>
    [Serializable]
    public class ListOptionData : IEquatable<ListOptionData>, IComparable<ListOptionData>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ListOptionData()
        {
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="data"></param>
        public ListOptionData(DataRow data)
        {
            Load(data);
        }

        /// <summary>
        /// Get whether option is deleted.
        /// </summary>
        public bool Deleted { get; private set; }

        /// <summary>
        /// Load the item from the data
        /// </summary>
        /// <param name="data"></param>
        public void Load(DataRow data)
        {
            //Option ID
            OptionID = data["OptionID"] != DBNull.Value ? Convert.ToInt32(data["OptionID"]) : -1;

            //Alias
            Alias = data["Alias"] != DBNull.Value ? (string)data["Alias"] : string.Empty;

            //Is Default
            object isDefault = data["IsDefault"];

            if (isDefault == DBNull.Value)
            {
                IsDefault = false;
            }
            else
            {
                IsDefault = Convert.ToInt32(isDefault) == 1;
            }

            //Is Other
            object isOther = data["IsOther"];

            if (isOther == DBNull.Value)
            {
                IsOther = false;
            }
            else
            {
                IsOther = Convert.ToInt32(isOther) == 1;
            }

            //Is none of the above
            object isNoneOfAbove = data["isNoneOfAbove"];

            if (isNoneOfAbove == DBNull.Value)
            {
                IsNoneOfAbove = false;
            }
            else
            {
                IsNoneOfAbove = Convert.ToInt32(isNoneOfAbove) == 1;
            }

            //Deleted
            object deleted = data["Deleted"];

            if (deleted == DBNull.Value)
            {
                Deleted = false;
            }
            else
            {
                Deleted = Convert.ToInt32(deleted) == 1;
            }

            //Position
            Position = data["Position"] != DBNull.Value ? Convert.ToInt32(data["Position"]) : 1;

            //Points
            Points = data["Points"] != DBNull.Value ? Convert.ToDouble(data["Points"]) : 0;

            //Category
            Category = data["Category"] != DBNull.Value ? data["Category"].ToString() : String.Empty;

            //ContentID
            ContentID = data["ContentID"] != DBNull.Value ? Convert.ToInt32(data["ContentID"]) : (int?)null;
        }

        /// <summary>
        /// Get the ID of this option
        /// </summary>
        public int OptionID { get; set; }

        /// <summary>
        /// Get/set the option alias
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Get/set the option Category
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Get/set whether this option is a default value
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Get/set whether this option is an "other" option
        /// </summary>
        public bool IsOther { get; set; }

        /// <summary>
        /// Get/set whether this option is an "none of above" option
        /// </summary>
        public bool IsNoneOfAbove { get; set; }

        /// <summary>
        /// Get/set the position of the item
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Get/set the points value for the item
        /// </summary>
        public double Points { get; set; }

        /// <summary>
        /// Item is not enabled
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Get/set image ID
        /// </summary>
        public int? ContentID { get; set; }

        /// <summary>
        /// Get the text id for the option
        /// </summary>
        public string TextID
        {
            get
            {
                if (OptionID > 0)
                {
                    return "/listOption/" + OptionID + "/text";
                }

                return string.Empty;
            }
        }

        #region IEquatable<ListOptionData> Members

        bool IEquatable<ListOptionData>.Equals(ListOptionData other)
        {
            if (other.OptionID == OptionID)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region IComparable<ListOptionData> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(ListOptionData other)
        {
            if (Position > other.Position)
            {
                return 1;
            }

            if (Position < other.Position)
            {
                return -1;
            }

            return 0;
        }

        #endregion
    }
}
