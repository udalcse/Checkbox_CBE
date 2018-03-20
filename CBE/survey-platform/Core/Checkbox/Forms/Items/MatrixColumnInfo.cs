using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Items.UI;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Simple container for matrix column information
    /// </summary>
    /// <remarks>Auto get/set properties are used instead of public fields since properties
    /// are required for DataBinder.Eval(...) syntax which is useful for item renderers.</remarks>
    [Serializable]
    public class MatrixColumnInfo
    {
        private ItemData _prototypeItemData;
        private AppearanceData _prototypeAppearanceData;
        private Item _prototypeItem;

        /// <summary>
        /// Type of column
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Get/set response language
        /// </summary>
        public string LanguageCode { get; set; }

        /// <summary>
        /// Get/set column number
        /// </summary>
        public int ColumnNumber { get; set; }

        /// <summary>
        /// Get/set validation errors for column
        /// </summary>
        public List<string> ValidationErrors { get; set; }

        /// <summary>
        /// Get/set whether column requires unique answers
        /// </summary>
        public bool RequireUniqueAnswers { get; set; }

        /// <summary>
        /// Get/set width of column
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// Get/set whether column requires answers
        /// </summary>
        public bool RequireAnswers { get; set; }

      /// <summary>
        /// Get/set id of column prototype item
        /// </summary>
        public int PrototypeItemId { get; set; }

        /// <summary>
        /// Get/set template ID
        /// </summary>
        public int? TemplateID { get; set; }

        /// <summary>
        /// Column alias
        /// </summary>
        public string Alias
        {
            get { return PrototypeItemData != null ? PrototypeItemData.Alias : string.Empty; }
        }

        /// <summary>
        /// Create and populate an object to use for data transfer
        /// </summary>
        /// <returns></returns>
        public MatrixItemColumn GetDataTransferObject()
        {
            return new MatrixItemColumn
            {
                AnswerRequired = RequireAnswers,
                ColumnNumber = ColumnNumber,
                PrototypeItemId = PrototypeItemId,
                RequireUniqueAnswers = RequireUniqueAnswers,
                ScaleStartText = ScaleStartText,
                ScaleMidText = ScaleMidText,
                ScaleEndText = ScaleEndText,
                ValidationErrors = ValidationErrors.ToArray(),
                Width = Width,
                ColumnType = TypeName,
                OptionTexts = GetOptionTexts().ToArray(),
                HeaderText = Text,
                Alias = Alias
            };
        }

        /// <summary>
        /// Get data container for matrix column
        /// </summary>
        public ItemData PrototypeItemData 
        {
            get
            {
                if (_prototypeItemData == null
                    && PrototypeItemId > 0)
                {
                    _prototypeItemData = ItemConfigurationManager.GetConfigurationData(PrototypeItemId, true);
                }

                return _prototypeItemData;
            }
        }

        /// <summary>
        /// Get business logic container for prototype item
        /// </summary>
        public Item PrototypeItem 
        {
            get
            {
                if (_prototypeItem == null
                    && PrototypeItemData != null)
                {
                    _prototypeItem = PrototypeItemData.CreateItem(LanguageCode, TemplateID);
                }

                return _prototypeItem;
            }
        }

        /// <summary>
        /// Get appearance for column
        /// </summary>
        public AppearanceData PrototypeAppearance
        {
            get
            {
                if (_prototypeAppearanceData == null
                    && PrototypeItemId > 0)
                {
                    _prototypeAppearanceData = AppearanceDataManager.GetAppearanceDataForItem(PrototypeItemId);
                }

                return _prototypeAppearanceData;
            }
        }
        

        /// <summary>
        /// Get type id of item associated with column
        /// </summary>
        public int PrototypeItemTypeId
        {
            get
            {
                return PrototypeItemData != null ? PrototypeItemData.ItemTypeID : -1;
            }
        }
        
        /// <summary>
        /// Get type name of prototype item
        /// </summary>
        public string PrototypeItemTypeName
        {
            get
            {
                return PrototypeItemData != null ? PrototypeItemData.ItemTypeName : string.Empty;
            }
        }

        /// <summary>
        /// Get text of column
        /// </summary>
        public string Text
        {
            get
            {
                if (PrototypeItem != null
                    && PrototypeItem is LabelledItem)
                {
                    return ((LabelledItem)PrototypeItem).Text;
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Get option texts
        /// </summary>
        /// <returns></returns>
        public List<string> GetOptionTexts()
        {
            List<string> texts = new List<string>();

            if (PrototypeItem != null
               && PrototypeItem is SelectItem)
            {
                List<ListOption> options = ((SelectItem)PrototypeItem).Options;

                foreach (ListOption option in options)
                {
                    texts.Add(option.Text);
                }
            }

            return texts;
        }

        /// <summary>
        /// Get a boolean indicating if item is a rating scale
        /// </summary>
        public bool IsRatingScale
        {
            get
            {
                return PrototypeItemData != null && PrototypeItemData is RatingScaleItemData;
            }
        }

        /// <summary>
        /// Get start text of scale
        /// </summary>
        public string ScaleStartText
        {
            get
            {
                return IsRatingScale ? ((RatingScale)PrototypeItem).StartText : string.Empty;
            }
        }

        /// <summary>
        /// Get scale end text
        /// </summary>
        public string ScaleEndText
        {
            get
            {
                return IsRatingScale ? ((RatingScale)PrototypeItem).EndText : string.Empty;
            }
        }

        /// <summary>
        /// Get scale mid text
        /// </summary>
        public string ScaleMidText
        {
            get
            {
                return IsRatingScale ? ((RatingScale)PrototypeItem).MidText : string.Empty;
            }
        }
    }
}
