//===============================================================================
// Checkbox Application Source Code
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================
using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Checkbox.Forms.Items
{
    ///<summary>
    ///</summary>
    ///<param name="optionId"></param>
    ///<param name="defaultText"></param>
    public delegate string GetOptionTextDelegate(int optionId, string defaultText);

    /// <summary>
    /// Items in a select item list.
    /// </summary>
    [Serializable]
    public class 
        ListOption : IEquatable<ListOption>, IEquatable<Int32>, IXmlSerializable
    {
        private string _text;

        /// <summary>
        /// Delegate for getting option text
        /// </summary>
        public GetOptionTextDelegate OptionTextDelegate;

        /// <summary>
        /// Get/set the item ID
        /// </summary>
        public Int32 ID { get; set; }

        /// <summary>
        /// Get/set point value
        /// </summary>
        public double Points { get; set; }

        /// <summary>
        /// Get/set the item text
        /// </summary>
        public string Text
        {
            get
            {
                if (OptionTextDelegate != null)
                {
                    return OptionTextDelegate(ID, _text);
                }

                return _text;
            }
            set { _text = value; }
        }

        /// <summary>
        /// Get/set whether the option is selected by default.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Get/set the option Category
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Get/Set the item alias
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Get/set if the item is selected
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Get/set if the item is an "other"
        /// </summary>
        public bool IsOther { get; set; }

        /// <summary>
        /// Get/set if the item is an "none of above"
        /// </summary>
        public bool IsNoneOfAbove { get; set; }

        /// <summary>
        /// Get/set image ID
        /// </summary>
        public int? ContentID { get; set; }

        /// <summary>
        /// Get/set if the item is disabled
        /// </summary>
        public bool Disabled { get; set; }

        #region IEquatable<ListOption> Members

        /// <summary>
        /// Determine if items are equal
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ListOption other)
        {
            return (ID == other.ID);
        }

        #endregion

        #region IEquatable<int> Members

        /// <summary>
        /// More equals
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(int other)
        {
            return (ID == other);
        }

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// Return NULL per MSDN documentation
        /// </summary>
        /// <returns></returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Read XML -- Not implemented
        /// </summary>
        /// <param name="reader"></param>
        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Write XML
        /// </summary>
        /// <param name="writer"></param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("listOption");
            writer.WriteAttributeString("optionId", ID.ToString());
            writer.WriteElementString("text", Text);
            writer.WriteElementString("alias", Alias);
            writer.WriteElementString("isSelected", IsSelected.ToString());
            writer.WriteElementString("isOther", IsOther.ToString());
            writer.WriteElementString("IsNoneOfAbove", IsNoneOfAbove.ToString());
            writer.WriteElementString("isEnabled", (!Disabled).ToString());
            writer.WriteElementString("points", Points.ToString());

            writer.WriteEndElement();
        }
        #endregion
    }
}
