using Checkbox.Forms;
using Checkbox.Security;
using Checkbox.Wcf.Services.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Checkbox.Common;
using Newtonsoft.Json;

namespace Checkbox.Users
{
    /// <summary>
    /// Represents radio button custom user field
    /// </summary>
    [Serializable]
    public class RadioButtonField : ProfileProperty
    {
       
        /// <summary>
        /// a list options for radio button type
        /// </summary>
        public List<RadioButtonFieldOption> Options { get; set; }

        /// <summary>
        /// a list options for radio button type
        /// </summary>
        public List<RadioButtonFieldOption> AllItemOptions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RadioButtonField"/> class.
        /// </summary>
        public RadioButtonField()
        {
            Options = new List<RadioButtonFieldOption>();
        }

        /// <summary>
        /// It is used to determine field type when object is serialized
        /// </summary>
        public CustomFieldType FieldType
        {
            get { return CustomFieldType.RadioButton; }
        }

        /// <summary>
        /// Binds Radio button item with radio profile field
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="renderMode">The render mode.</param>
        public static void BindRadioProfileFieldToItem(ItemProxyObject item, RenderMode renderMode, Guid? responseGuid = null)
        {
            if (item.TypeName == "RadioButtons")
            {
                var profileProperties = ProfileManager.GetPropertiesList();
                var bindedField = profileProperties.FirstOrDefault(prop => prop.BindedItemId.Any(bindedItemId => bindedItemId == item.ItemId));
                var userGuid = PropertyBindingManager.GetCurrentUserGuid();

                if (bindedField != null)
                {
                    RadioButtonField radioField = null;

                    if (!responseGuid.HasValue)
                    {
                        if (userGuid.Equals(Guid.Empty))
                        {
                            var radioFieldJson =
                                PropertyBindingManager.GetSessionAnonymousBindedFieldJson(bindedField.Name);
                            if (!string.IsNullOrWhiteSpace(radioFieldJson))
                                radioField = JsonConvert.DeserializeObject<RadioButtonField>(radioFieldJson);
                        }

                        if (radioField == null)
                        {
                            radioField = ProfileManager.GetRadioButtonField(bindedField.Name, userGuid);
                        }
                    }
                    else
                    {
                        var response = PropertyBindingManager.GetResponseFieldState(item.ItemId, responseGuid.Value,
                           HttpContext.Current.User?.Identity.Name);
                        if (!string.IsNullOrWhiteSpace(response))
                            radioField = JsonConvert.DeserializeObject<RadioButtonField>(response);
                    }


                    var optionAliases = ProfileManager.GetRadioOptionAliases(item.ItemId, radioField.Name);

                    SurveyResponseItem surveyResponseItem = null;
                    if (!(item is SurveyResponseItem))
                        return;

                    surveyResponseItem = item as SurveyResponseItem;
                    surveyResponseItem.Options = new SurveyResponseItemOption[radioField.Options.Count];
                    string alias = string.Empty;
                    
                    for (var i = 0; i < radioField.Options.Count; i++)
                    {
                        optionAliases.TryGetValue(radioField.Options[i].Name, out alias);
                        surveyResponseItem.Options[i] = new SurveyResponseItemOption()
                        {
                            Alias = alias,
                            ContentId = null,
                            IsDefault = false,
                            IsNoneOfAbove = false,
                            IsOther = radioField.Options[i].IsSelected,
                            IsSelected = radioField.Options[i].IsSelected,
                            OptionId = radioField.Options[i].Id,
                            Points = 0,
                            Text = string.IsNullOrEmpty(alias) ? radioField.Options[i].Name : alias
                        };
                    }

                    if (radioField != null && radioField.Options.Any())
                    {
                        if (surveyResponseItem.Answers != null && surveyResponseItem.Answers.Any())
                        {
                            surveyResponseItem.Answers.First().AnswerText = surveyResponseItem.Options.First(s => s.OptionId == surveyResponseItem.Answers.First().OptionId).Text;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds the empty option.
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllOptions()
        {
            return this.Options.Select(item => item.Name).ToList();
        }

        /// <summary>
        /// Adds the option.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="isChecked">if set to <c>true</c> [is checked].</param>
        public void AddOption(string title, bool isChecked)
        {
            Options.Add(new RadioButtonFieldOption(title, isChecked));

            if (isChecked)
                this.CheckOption(title);
        }

        /// <summary>
        /// Checks the option.
        /// </summary>
        /// <param name="index">The index.</param>
        public void CheckOption(int index)
        {
            foreach (var item in this.Options)
            {
                item.IsSelected = false;
            }

            this.Options[index].IsSelected = true;
        }

        /// <summary>
        /// Checks the option.
        /// </summary>
        /// <param name="name">The name.</param>
        public void CheckOption(string name)
        {
            foreach (var item in this.Options)
            {
                item.IsSelected = false;
            }

            var radioButtonFieldOption = this.Options.FirstOrDefault(item => item.Name.Equals(name));
            if (radioButtonFieldOption != null)
                radioButtonFieldOption.IsSelected = true;
        }

        /// <summary>
        /// Uns the check options.
        /// </summary>
        public void UnCheckOptions()
        {
            foreach (var item in this.Options)
                item.IsSelected = false;
        }

        /// <summary>
        /// Determines whether the specified title has option.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <returns>
        ///   <c>true</c> if the specified title has option; otherwise, <c>false</c>.
        /// </returns>
        public bool HasOption(string title)
        {
            return this.Options.Any(item => item.Name.Equals(title));
        }
    }

    /// <summary>
    /// Represents an option for radio button field
    /// </summary>
    [Serializable]
    public class RadioButtonFieldOption
    {
        /// <summary>
        /// Radio Button option id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Option name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// is current option selected
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// alias that displayed in a survey instead of option name
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Gets or sets the item identifier.
        /// </summary>
        /// <value>
        /// The item identifier.
        /// </value>
        public int ItemId { get; set; }

        /// <summary>
        /// constructor with parameters
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="isSelected">if set to <c>true</c> [is selected].</param>
        public RadioButtonFieldOption(string name, bool isSelected) : this(name, isSelected, "" , 0 , 0)
        {
        }

        /// <summary>
        /// constructor that takes alias
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="isSelected">if set to <c>true</c> [is selected].</param>
        /// <param name="alias">The alias.</param>
        public RadioButtonFieldOption(string name, bool isSelected, string alias) : this(name, isSelected, alias, 0, 0)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RadioButtonFieldOption" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="isSelected">if set to <c>true</c> [is selected].</param>
        /// <param name="alias">The alias.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="itemId">The item identifier.</param>
        public RadioButtonFieldOption(string name, bool isSelected, string alias, int id, int itemId) 
        {
            Name = name;
            IsSelected = isSelected;
            Alias = alias;
            Id = id;
            ItemId = itemId;
        }

        /// <summary>
        /// constructor without parameters
        /// </summary>
        public RadioButtonFieldOption() { }
    }
}
