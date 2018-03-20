using System;
using System.Linq;
using System.Web;
using System.Xml;
using System.Collections.Generic;
using System.Collections.Specialized;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Common;
using Checkbox.Forms.Items.Configuration;
using Prezza.Framework.Security.Principal;
using Checkbox.Common;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Base class definition for items that participate in a survey response.
    /// </summary>
    [Serializable]
    public abstract class ResponseItem : Item
    {
        private Response _response;
        private List<string> _validationErrors;
        
        /// <summary>
        /// 
        /// </summary>
        protected ResponseItem()
        {
            Valid = true;
        }

        /// <summary>
        /// Gets whether the Item is required
        /// </summary>
        public virtual bool Required { get; private set; }

        /// <summary>
        /// Get whether the item is valid
        /// </summary>
        public bool Valid { get; protected set; }

        /// <summary>
        /// Get any validation errors.
        /// </summary>
        public List<string> ValidationErrors
        {
            get { return _validationErrors ?? (_validationErrors = new List<string>()); }
        }

        /// <summary>
        /// Validate the current state of the item
        /// </summary>
        /// <returns></returns>
        public virtual void Validate()
        {
            ValidationErrors.Clear();
            Valid = DoValidateItem();
        }

        /// <summary>
        /// Validate the state of the item
        /// </summary>
        protected virtual bool DoValidateItem()
        {
            return true;
        }

        /// <summary>
        /// Configure the response item.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData configuration, string languageCode, int? templateId)
        {
            base.Configure(configuration, languageCode, templateId);

            ArgumentValidation.CheckExpectedType(configuration, typeof(ResponseItemData));
            Required = ((ResponseItemData)configuration).IsRequired;
        }

        /// <summary>
        /// On load override
        /// </summary>
        protected override void OnPageLoad()
        {
            RunRules();

            base.OnPageLoad();
        }

        /// <summary>
        /// Runs any Rules for this Item
        /// </summary>
        public virtual void RunRules()
        {
            _response.RulesEngine.RunRules(ID, Response);
            if (!Excluded)
                InitializeDefaults();
        }

        /// <summary>
        /// Return a boolean indicating if in "Edit Mode" or not.  Considered "Edit Mode" if
        /// no response has been set.
        /// </summary>
        public bool EditMode { get { return Response == null; } }

        /// <summary>
        /// Gets and sets the parent <see cref="Response"/> for this Item
        /// </summary>
        public Response Response
        {
            get { return GetResponse(); }
            set
            {
                SetResponse(value);
                OnResponseSet();
            }
        }

        /// <summary>
        /// There are other items on the same page that depends on this item
        /// </summary>
        public bool? IsSPCArgument { get; set; }

        /// <summary>
        /// Overridable method to set the response
        /// </summary>
        /// <param name="response"></param>
        public virtual void SetResponse(Response response, bool subscribe = true)
        {
            _response = response;
            if (subscribe)
            {
                _response.StateRestored += ResponseStateRestored;
            }            
        }

        /// <summary>
        /// Overridable method to get response
        /// </summary>
        /// <returns></returns>
        protected virtual Response GetResponse()
        {
            return _response;
        }

        /// <summary>
        /// Overridable method called when the response is set
        /// </summary>
        protected virtual void OnResponseSet()
        {
        }

        /// <summary>
        /// Call overridable state restored handler when response state is restored
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResponseStateRestored(object sender, ResponseStateEventArgs e)
        {
            OnStateRestored();
        }

        /// <summary>
        /// Overriable handler for response state restored
        /// </summary>
        protected virtual void OnStateRestored()
        {
        }

        /// <summary>
        /// Get text from the pipe manager, registering the text if necessary.  If
        /// response is null, the text value is returned.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        protected virtual string GetPipedText(string key, string text)
        {
            if (Response != null && Response.PipeMediator != null)
            {
                if (!Response.PipeMediator.IsInitialized && Response.Respondent != null)
                    Response.PipeMediator.Initialize(Response, Response.Respondent);

                return Response.PipeMediator.GetText(ID, key, text);
            }

            return text;
        }

        /// <summary>
        /// Create the data transfer object to use for remote survey taking
        /// </summary>
        /// <returns></returns>
        public override IItemProxyObject CreateDataTransferObject()
        {
            return new SurveyResponseItem();
        }

        /// <summary>
        /// Build data transfer object
        /// </summary>
        /// <param name="itemDto"></param>
        protected override void BuildDataTransferObject(IItemProxyObject itemDto)
        {
            base.BuildDataTransferObject(itemDto);

            if (itemDto is SurveyResponseItem)
            {
                //Response item specific values
                ((SurveyResponseItem)itemDto).Visible = Visible;
                ((SurveyResponseItem)itemDto).Excluded = Excluded;
                ((SurveyResponseItem)itemDto).LanguageCode = LanguageCode;
                ((SurveyResponseItem)itemDto).IsValid = !ValidationErrors.Any();
                ((SurveyResponseItem)itemDto).IsSPCArgument = IsSPCArgument.HasValue && IsSPCArgument.Value;
                ((SurveyResponseItem)itemDto).ValidationErrors = ValidationErrors.ToArray();
                ((SurveyResponseItem)itemDto).AnswerRequired = Required;
              
                //Other values specific to child types. Set default values
                ((SurveyResponseItem)itemDto).Options = new SurveyResponseItemOption[] { };
                ((SurveyResponseItem)itemDto).Answers = new SurveyResponseItemAnswer[] { };
                ((SurveyResponseItem)itemDto).Description = string.Empty;
                ((SurveyResponseItem)itemDto).Text = string.Empty;
            }
        }

        /// <summary>
        /// Update repsonse item state from new state in item dto.  It is up to individual response items
        /// to implement this functionality as default implementation is a no op.
        /// </summary>
        /// <param name="dto"></param>
        public override void UpdateFromDataTransferObject(IItemProxyObject dto)
        {
        }

        /// <summary>
        /// Get instance data for the item
        /// </summary>
        /// <returns></returns>
        protected override NameValueCollection GetInstanceDataValuesForSerialization()
        {
            NameValueCollection values = base.GetInstanceDataValuesForSerialization();

            values["isValid"] = Valid.ToString();

            return values;
        }

        /// <summary>
        /// Write instance data and add validation messages
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="isText"></param>
        public override void WriteXmlInstanceData(XmlWriter writer, bool isText)
        {
            base.WriteXmlInstanceData(writer, isText);

            writer.WriteStartElement("validationErrors");

            foreach (string errorMessage in ValidationErrors)
            {
                writer.WriteElementString("validationError", errorMessage);
            }
            writer.WriteEndElement();
        }
    }
}
