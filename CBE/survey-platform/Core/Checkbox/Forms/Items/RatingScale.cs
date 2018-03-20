using System;
using System.Linq;
using System.Xml;
using System.Collections.Specialized;

using Checkbox.Common;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Validation;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Radio button scale business item
    /// </summary>
    [Serializable]
    public class RatingScale : Select1
    {
        private string _notApplicableTextID;


        /// <summary>
        /// Configure the item
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData configuration, string languageCode, int? templateId)
        {
            var data = (RatingScaleItemData)configuration;

            _notApplicableTextID = data.NotApplicableTextID;

            base.Configure(configuration, languageCode, templateId);

            StartText = GetText(data.StartTextID);
            MidText = GetText(data.MidTextID);
            EndText = GetText(data.EndTextID);
        }

        /// <summary>
        /// Set option text as option points
        /// </summary>
        /// <param name="metaOption"></param>
        /// <returns></returns>
        protected override ListOption CreateOption(ListOptionData metaOption)
        {
            ListOption option = base.CreateOption(metaOption);

            if (option.IsOther)
            {
                NotApplicableText = GetText(_notApplicableTextID);

                if (Utilities.IsNullOrEmpty(NotApplicableText))
                {
                    NotApplicableText = GetText("/controlText/ratingScale/notApplicableDefault");
                }

                if (Utilities.IsNullOrEmpty(NotApplicableText))
                {
                    NotApplicableText = "n/a";
                }

                option.Text = GetPipedText("OptionText_" + ID + "_" + option.ID, NotApplicableText);
            }
            else
            {
                option.Text = metaOption.Points.ToString();
            }

            return option;
        }


        /// <summary>
        /// Scale start text
        /// </summary>
        public string StartText { get; set; }

        /// <summary>
        /// Scale mid text
        /// </summary>
        public string MidText { get; set; }

        /// <summary>
        /// Scale end text
        /// </summary>
        public string EndText { get; set; }

        /// <summary>
        /// N/A option text
        /// </summary>
        public string NotApplicableText { get; set; }


        /// <summary>
        /// Get instance data values for serialization
        /// </summary>
        /// <returns></returns>
        protected override NameValueCollection GetInstanceDataValuesForSerialization()
        {
            NameValueCollection values = base.GetInstanceDataValuesForSerialization();

            ListOption naOption = Options.FirstOrDefault(p => p.IsOther);

            values["startText"] = StartText;
            values["midText"] = MidText;
            values["endText"] = EndText;
            values["notApplicableText"] = naOption == null ? "" : naOption.Text;

            return values;
        }

        /// <summary>
        /// Validate answers.
        /// </summary>
        /// <returns></returns>
        protected override bool ValidateAnswers()
        {
            var scaleItemValidator = new RatingScaleValidator();

            if (!scaleItemValidator.Validate(this))
            {
                ValidationErrors.Add(scaleItemValidator.GetMessage(LanguageCode));
                return false;
            }

            return true;
        }

        /// <summary>
        /// Override serialization to write N/A answer as "other" answer
        /// </summary>
        /// <param name="option"></param>
        /// <param name="writer"></param>
        protected override void WriteOptionAnswer(ListOption option, XmlWriter writer)
        {
            writer.WriteStartElement("answer");
            writer.WriteAttributeString("optionId", option.ID.ToString());

            writer.WriteCData(option.Text);

            writer.WriteEndElement();
        }
    }
}
