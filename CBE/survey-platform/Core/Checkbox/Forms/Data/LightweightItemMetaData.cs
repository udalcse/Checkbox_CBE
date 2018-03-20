using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Checkbox.Common;
using Checkbox.Forms.Items.Configuration;
using Prezza.Framework.Common;

namespace Checkbox.Forms.Data
{

    /// <summary>
    /// Lightweight container to store meta data information for items contained in a survey being reported on.
    /// </summary>
    /// <remarks><see cref="Checkbox.Forms.Items.Configuration.ItemData"/> and <see cref="Checkbox.Forms.Items.Configuration.ListData" /> objects are fairly
    /// heavyweight and often contain internal <see cref="System.Data.DataSet"/> to store their data.  Since report data may be cached, and therefore serialized in
    /// multi-machine/process environments, this serves as a more efficient container class to use for reporting.</remarks>
    [Serializable]
    public class LightweightItemMetaData : ICloneable
    {
        private List<int> _children;
        private List<int> _options;
        private Dictionary<string, string> _textDictionary;

        private Dictionary<int, string> _matrixColumnTypes;
        private Dictionary<int, string> _matrixRowTypes;
        private Dictionary<int, int> _matrixColumnPrototypeIds;
        private Dictionary<int, int> _matrixRowKeyItems;

        private Dictionary<int, Coordinate> _matrixChildCoordindates;

        private List<string> _populatedLanguages;

        /// <summary>
        /// Text dictionary
        /// </summary>
        private Dictionary<string, string> TextDictionary
        {
            get { return _textDictionary ?? (_textDictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)); }
        }

        /// <summary>
        /// Get a list containing the database Ids of all options associated with the item.
        /// </summary>
        public List<int> Options
        {
            get { return _options ?? (_options = new List<int>()); }
        }

        /// <summary>
        /// Get list of languages populated for the item. For sake of efficiency, items are only populated with text of "active" survey
        /// languages and this value is used to validate item text in cases where active languages list has changed between when item
        /// is cached and when item is accessed.
        /// </summary>
        public List<string> PopulatedLanguages 
        { 
            get { return _populatedLanguages ?? (_populatedLanguages = new List<string>()); } 
            set { _populatedLanguages = value; }
        }

        /// <summary>
        /// Get a list containing the database Ids of all children of this item.
        /// </summary>
        public List<int> Children
        {
            get { return _children ?? (_children = new List<int>()); }
        }

        /// <summary>
        /// Get/set whether item supports answers or not
        /// </summary>
        public bool IsAnswerable { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool RequiresAnswer { get; set; }

        /// <summary>
        /// Get/set whether item supports score values
        /// </summary>
        public bool IsScored { get; set; }

        /// <summary>
        /// Get/set position of page containing item
        /// </summary>
        public int PagePosition { get; set; }

        /// <summary>
        /// Get/set position of item on containing page
        /// </summary>
        public int ItemPosition { get; set; }

        /// <summary>
        /// Get/set item coordinate if item is a child.  NULL for non-child items
        /// </summary>
        public Coordinate Coordinate { get; set; }

        /// <summary>
        /// Get/set text of row containing this item, if item is child of a matrix
        /// </summary>
        public string GetMatrixRowText(string languageCode)
        {
            return TextDictionary.ContainsKey("RowText_" + languageCode) 
                ? TextDictionary["RowText_" + languageCode] 
                : string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="languageCode"></param>
        public void SetMatrixRowText(string text, string languageCode)
        {
            TextDictionary["RowText_" + languageCode] = text;
        }

        /// <summary>
        /// Get/set alias of row containing this item, if item is child of matrix
        /// </summary>
        public string MatrixRowAlias { get; set; }

        /// <summary>
        /// Get position text for item
        /// </summary>
        public string PositionText
        {
            get
            {
                if (PagePosition == 0 || ItemPosition == 0)
                {
                    return string.Empty;
                }

                return Coordinate != null
                           ? string.Format("{0}.{1}.{2}.{3}", PagePosition - 1, ItemPosition, Coordinate.Y, Coordinate.X)
                           : string.Format("{0}.{1}", PagePosition - 1, ItemPosition);
            }
        }

        /// <summary>
        /// Get/set the database Id of the item.
        /// </summary>
        public int ItemId { get; set; }


        /// <summary>
        /// Get  the text of the item.
        /// </summary>
        public string GetText(string languageCode)
        {
            var itemText = new StringBuilder();

            //Append matrix row text/alias if necessary
            if (!string.IsNullOrEmpty(GetMatrixRowText(languageCode)))
            {
                itemText.Append(GetMatrixRowText(languageCode));
                itemText.Append(" - ");
            }
            else if (!string.IsNullOrEmpty(MatrixRowAlias))
            {
                itemText.Append(MatrixRowAlias);
                itemText.Append(" - ");
            }

            if (TextDictionary.ContainsKey("Text_" + languageCode))
            {
                itemText.Append(TextDictionary["Text_" + languageCode]);
            }

            return itemText.ToString();
        }

        /// <summary>
        /// Set item descrpition
        /// </summary>
        /// <param name="languageCode"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public void SetDescription(string languageCode, string description)
        {
            TextDictionary["Description_" + languageCode] = description;
        }

        /// <summary>
        /// Get  the description of the item.
        /// </summary>
        public string GetDescription(string languageCode)
        {
            if (TextDictionary.ContainsKey("Description_" + languageCode))
            {
                return TextDictionary["Description_" + languageCode];
            }

            return string.Empty;
        }

        /// <summary>
        /// Set item text
        /// </summary>
        /// <param name="languageCode"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public void SetText(string languageCode, string text)
        {
            TextDictionary["Text_" + languageCode] = text;
        }

        /// <summary>
        /// Get/set id of template item belongs to
        /// </summary>
        public int TemplateId { get; set; }

        /// <summary>
        /// Get/set item alias.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Get/set the type of the item
        /// </summary>
        public string ItemType { get; set; }

        /// <summary>
        /// Get/set id of this item's parent.
        /// </summary>
        /// <remarks>In a non-customized environment, this value represents the ID of a matrix item that contains this child item.  If this item is a 
        /// standalone item, the ancestor id value will be 0 or negative.</remarks>
        public int AncestorId { get; set; }

        /// <summary>
        /// Get/set whether this item supports "other" options.
        /// </summary>
        /// <remarks>This is applicable only to Select items, such as Drop Down Lists, Checkboxes, Radio Buttons, and Rating Scales.</remarks>
        public bool AllowOther { get; set; }

        /// <summary>
        /// Matrix only
        /// </summary>
        public int RowCount
        {
            get { return RowTypes.Count; }
        }

        /// <summary>
        /// Matrix only
        /// </summary>
        public int ColumnCount
        {
            get { return ColumnTypes.Count; }
        }

        /// <summary>
        /// Matrix only
        /// </summary>
        public string GetColumnType(int columnNumber)
        {
            if (ColumnTypes.ContainsKey(columnNumber))
            {
                return ColumnTypes[columnNumber];
            }

            return string.Empty;
        }

        /// <summary>
        /// Matrix only
        /// </summary>
        public int? GetColumnPrototypeID(int columnNumber)
        {
            if (ColumnPrototypeIds.ContainsKey(columnNumber))
            {
                return ColumnPrototypeIds[columnNumber];
            }

            return null;
        }


        /// <summary>
        /// Matrix only
        /// </summary>
        public int? GetRowPkItemId(int rowNumber)
        {
            if (RowKeyItems.ContainsKey(rowNumber))
            {
                return RowKeyItems[rowNumber];
            }

            return null;
        }

        /// <summary>
        /// Matrix only
        /// </summary>
        public string GetRowType(int rowNumber)
        {
            if (RowTypes.ContainsKey(rowNumber))
            {
                return RowTypes[rowNumber];
            }

            return string.Empty;
        }

        /// <summary>
        /// Get row types dictionary
        /// </summary>
        private Dictionary<int, string> RowTypes
        {
            get { return _matrixRowTypes ?? (_matrixRowTypes = new Dictionary<int, string>()); }
        }

        /// <summary>
        /// Get column types dictionary
        /// </summary>
        private Dictionary<int, string> ColumnTypes
        {
            get { return _matrixColumnTypes ?? (_matrixColumnTypes = new Dictionary<int, string>()); }
        }

        /// <summary>
        /// Get column types dictionary
        /// </summary>
        private Dictionary<int, int> ColumnPrototypeIds
        {
            get { return _matrixColumnPrototypeIds ?? (_matrixColumnPrototypeIds = new Dictionary<int, int>()); }
        }

        /// <summary>
        /// Get column types dictionary
        /// </summary>
        public Dictionary<int, int> RowKeyItems
        {
            get { return _matrixRowKeyItems ?? (_matrixRowKeyItems = new Dictionary<int, int>()); }
        }

        /// <summary>
        /// Get column types dictionary
        /// </summary>
        private Dictionary<int, Coordinate> ChildCoordinates
        {
            get { return _matrixChildCoordindates ?? (_matrixChildCoordindates = new Dictionary<int, Coordinate>()); }
        }

        /// <summary>
        /// Add column type
        /// </summary>
        /// <param name="columnNumber"></param>
        /// <param name="columnPrototypeId"></param>
        /// <param name="type"></param>
        public void AddColumn(int columnNumber, int columnPrototypeId, string type)
        {
            ColumnTypes[columnNumber] = type;
            ColumnPrototypeIds[columnNumber] = columnPrototypeId;
        }

        /// <summary>
        /// Add row type
        /// </summary>
        /// <param name="rowNumber"></param>
        /// <param name="rowKeyItemId"></param>
        /// <param name="type"></param>
        public void AddRow(int rowNumber, int rowKeyItemId, string type)
        {
            RowTypes[rowNumber] = type;
            RowKeyItems[rowNumber] = rowKeyItemId;
        }

        /// <summary>
        /// Get child coordinates
        /// </summary>
        /// <param name="childItemId"></param>
        /// <param name="coordinate"></param>
        public void SetChildCoordinate(int childItemId, Coordinate coordinate)
        {
            ChildCoordinates[childItemId] = coordinate;
        }

        /// <summary>
        /// Get coordinate of child item
        /// </summary>
        /// <param name="childItemId"></param>
        /// <returns></returns>
        public Coordinate GetChildCoordinate(int childItemId)
        {
            if (ChildCoordinates.ContainsKey(childItemId))
            {
                return ChildCoordinates[childItemId];
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetAlias()
        {
            var alias = new StringBuilder();

            //Append matrix row text/alias if necessary
            if (!string.IsNullOrEmpty(MatrixRowAlias))
            {
                alias.Append(MatrixRowAlias);
                alias.Append(" - ");
            }

            return alias.Append(String.IsNullOrEmpty(Alias) ? Alias : Utilities.AdvancedHtmlEncode(Alias)).ToString();
        }

        /// <summary>
        /// Get the text or alias of the item, depending in the <paramref name="preferAlias"/> parameter value.  If the text is requested, but not found
        /// the method will fall back to returning the description and, if no description exists, returning the alias.
        /// </summary>
        /// <param name="preferAlias">Indicate whether the alias is the prefered value to return.</param>
        /// <param name="languageCode">Language code for teaxt</param>
        /// <returns>Item text or alias as indicated by <paramref name="preferAlias"/> parameter value.</returns>
        public string GetText(bool preferAlias, string languageCode)
        {
            //Try the text or alias, depending on input flag
            string text = preferAlias ? GetAlias() : GetText(languageCode);

            //If preferring alias and there is no alias, try text
            //Otherwise try description
            if (Utilities.IsNullOrEmpty(text))
            {
                text = preferAlias
                    ? GetText(languageCode)
                    : GetDescription(languageCode);
            }

            //Finally, if preferring alias and no alias or text, use description
            //Otherwise if preferring text and there is no text or description, use alias
            if (Utilities.IsNullOrEmpty(text))
            {
                text = preferAlias
                    ? GetDescription(languageCode)
                    : GetAlias();
            }

            return text;
        }


        /// <summary>
        /// Get/set last-modified date for the item.
        /// </summary>
        public DateTime LastModified { get; set; }


        /// <summary>
        /// Validate that the data is not out of date.
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            //Validate text languages
            if (ResponseTemplateManager.ActiveSurveyLanguages.Any(language => !PopulatedLanguages.Contains(language, StringComparer.InvariantCultureIgnoreCase)))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Clone item
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new LightweightItemMetaData
            {
                _children = new List<int>(_children ?? new List<int>()),
                _matrixChildCoordindates = new Dictionary<int, Coordinate>(_matrixChildCoordindates ?? new Dictionary<int, Coordinate>()),
                _options = new List<int>(_options ?? new List<int>()),
                _textDictionary = new Dictionary<string, string>(_textDictionary ?? new Dictionary<string, string>()),
                _matrixColumnTypes = new Dictionary<int, string>(_matrixColumnTypes ?? new Dictionary<int, string>()),
                _matrixRowTypes = new Dictionary<int, string>(_matrixRowTypes ?? new Dictionary<int, string>()),
                _matrixColumnPrototypeIds = new Dictionary<int, int>(_matrixColumnPrototypeIds ?? new Dictionary<int, int>()),
                _matrixRowKeyItems = new Dictionary<int, int>(_matrixRowKeyItems ?? new Dictionary<int, int>()),
                IsAnswerable = IsAnswerable,
                IsScored = IsScored,
                PagePosition =  PagePosition,
                ItemPosition = ItemPosition,
                Coordinate = Coordinate != null ? new Coordinate(Coordinate.X, Coordinate.Y) : null,
                ItemId =  ItemId,
                TemplateId = TemplateId,
                Alias = Alias,
                AncestorId = AncestorId,
                AllowOther = AllowOther,
                ItemType = ItemType,
                LastModified = LastModified,
                MatrixRowAlias = MatrixRowAlias
            };
        }
    }
}
