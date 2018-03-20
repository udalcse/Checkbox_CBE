//===============================================================================
// Checkbox Application Source Code
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================
using System;
using System.Xml;
using System.Xml.Schema;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Collections.Specialized;
using Checkbox.Globalization.Text;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Common;
using System.Collections.Generic;
using Checkbox.Common;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Encapsulates the business logic and persistence logic for an atomic 
    /// set of data.
    /// <remarks>
    /// Items are the component parts of a form or survey.  Each Item acts as a container of 
    /// response answer state and performs business rules and business logic. 
    /// 
    /// Items also provide custom XML serialization to include only item state
    /// information.
    /// </remarks>
    /// </summary>
    [Serializable]
    public abstract class Item : IDisposable, IXmlSerializable
    {
        #region Members

        private bool _exclude;

        [NonSerialized]
        private EventHandlerList _eventHandlers;

        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        protected Item()
        {
            Visible = true;
        }

        #region Properties

        /// <summary>
        /// Get/set the database PK for this Item
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Get/set parent item
        /// </summary>
        public Item Parent { set; get; }

        /// <summary>
        /// Get the template ID
        /// </summary>
        public int? TemplateID { get; protected set; }

        /// <summary>
        /// Gets the Alias of this Item
        /// </summary>
        public virtual string Alias { get; protected set; }

        /// <summary>
        /// Get the type of the item
        /// </summary>
        public int TypeID { get; protected set; }

        /// <summary>
        /// Get the item type name
        /// </summary>
        public string ItemTypeName { get; protected set; }

        /// <summary>
        /// Gets/sets a flag indicating whether this Item has a visible display
        /// </summary>
        /// <remarks>
        /// An ItemRenderer control binds its Visible property to this property. 
        /// </remarks>
        public virtual bool Visible { get; protected set; }

        /// <summary>
        /// Gets a flag indicating whether this Item participates in a Response
        /// </summary>
        /// <remarks>
        /// This property is set to false if the Item is conditionally excluded
        /// </remarks>
        public bool Excluded
        {
            get { return _exclude; }
            set
            {
                if (value != _exclude)
                {
                    _exclude = value;

                    if (_exclude)
                    {
                        OnItemExcluded();
                    }
                    else
                    {
                        OnItemIncluded();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool ShouldRender => this is ImageItem || this is Message || this is HtmlItem;

        ///<summary>
        ///Get/set whether item is "active" and participates in responses.
        ///</summary>
        public bool IsActive { get; set; }

        // <summary>
        /// Gets a string indicating the language code (e.g., 'en-US') used by this Item to determine string selection
        /// </summary>
        public string LanguageCode { get; set; }

        /// <summary>
        /// Gets the <see cref="EventHandlerList"/> for this Item
        /// </summary>
        protected EventHandlerList Events
        {
            get { return _eventHandlers ?? (_eventHandlers = new EventHandlerList()); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ExportMode ExportMode { set; get; }

        #endregion

        #region Events

        // event handler keys
        private static readonly object EventItemExcluded = new object();
        private static readonly object EventItemIncluded = new object();
        private static readonly object EventLoad = new object();

        /// <summary>
        /// Fired when an Item's Excluded property is set to 'true'
        /// </summary>
        public event EventHandler ItemExcluded
        {
            add { Events.AddHandler(EventItemExcluded, value); }
            remove { Events.RemoveHandler(EventItemExcluded, value); }
        }

        /// <summary>
        /// Fired when an Item's Excluded property is set to 'false'
        /// </summary>
        public event EventHandler ItemIncluded
        {
            add { Events.AddHandler(EventItemIncluded, value); }
            remove { Events.RemoveHandler(EventItemIncluded, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler Load
        {
            add { Events.AddHandler(EventLoad, value); }
            remove { Events.RemoveHandler(EventLoad, value); }
        }

        /// <summary>
        /// Event handler for Page loading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal virtual void Page_Load(object sender, EventArgs e)
        {
            OnPageLoad();
        }

        /// <summary>
        /// Called when this Item's Excluded property is set to false
        /// </summary>
        protected virtual void OnItemIncluded()
        {
            var handler = (EventHandler)Events[EventItemIncluded];
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// Called when this Item's Excluded property is set to true
        /// </summary>
        protected virtual void OnItemExcluded()
        {
            var handler = (EventHandler)Events[EventItemExcluded];
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// Called when a Page containing this Item is loaded.  Children of Item can choose to 
        /// override OnPageLoad() to perform actions immediately upon loading, such as sending an email, 
        /// or reading from the Request object
        /// </summary>
        protected virtual void OnPageLoad()
        {
            OnLoad();
        }

        /// <summary>
        /// Fire on load events
        /// </summary>
        protected virtual void OnLoad()
        {
            var handler = (EventHandler)Events[EventLoad];
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion

        /// <summary>
        /// Configure the item
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public virtual void Configure(ItemData configuration, string languageCode, int? templateId)
        {
            Alias = configuration.Alias;

            if (configuration.ID.HasValue)
            {
                ID = configuration.ID.Value;
            }

            TemplateID = templateId;
            LanguageCode = languageCode;
            TypeID = configuration.ItemTypeID;
            ItemTypeName = configuration.ItemTypeName;
            IsActive = configuration.IsActive;
       }

        /// <summary>
        /// Get the text for the specified text id
        /// </summary>
        /// <param name="textID"></param>
        /// <returns></returns>
        protected virtual string GetText(string textID)
        {
            //Get text for the item, using alternate languages if available
            return Utilities.StripMSWordTags(TextManager.GetText(textID, LanguageCode));
        }

        #region IDisposable Members

        /// <summary>
        /// Dispose of the item.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            //Instruct GC not to finialize this object
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Overridable dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
        }

        #endregion

        #region Data Transfer Objects for Remote Survey Taking

        /// <summary>
        /// Create the data transfer object for the item
        /// </summary>
        /// <returns></returns>
        public abstract IItemProxyObject CreateDataTransferObject();

        /// <summary>
        /// Update item state based on input DTO
        /// </summary>
        /// <param name="dto"></param>
        public abstract void UpdateFromDataTransferObject(IItemProxyObject dto);

        /// <summary>
        /// Get data transfer object for item. This object is suitable for binding to item renderers
        /// and/or to external data processes and removes the need for references to core Checkbox dlls.
        /// </summary>
        /// <returns></returns>
        public IItemProxyObject GetDataTransferObject()
        {
            //Create the object
            var itemDto = CreateDataTransferObject();

            //Build object
            if (itemDto != null)
            {
                BuildDataTransferObject(itemDto);
                /* -- this code drastically hurts performance!!! 
                 * so let's set these showAsterisks flags outside in the PageView
                ResponseTemplate rt = ResponseTemplateManager.GetResponseTemplate(itemDto.ParentTemplateId);
                if (rt != null)
                {
                    ResponseViewDisplayFlags displayFlags = rt.StyleSettings.GetDisplayFlags();
                    itemDto.Metadata["showAsterisks"] = ((displayFlags & ResponseViewDisplayFlags.Asterisks) == ResponseViewDisplayFlags.Asterisks).ToString();
                }*/
            }
            
            return itemDto;
        }

        /// <summary>
        /// Build up data transfer object for survey item.
        /// </summary>
        /// <param name="itemDto"></param>
        protected virtual void BuildDataTransferObject(IItemProxyObject itemDto)
        {
            if (itemDto is ItemProxyObject)
            {
                ((ItemProxyObject)itemDto).ItemId = ID;
                ((ItemProxyObject)itemDto).TypeName = ItemTypeName;
                ((ItemProxyObject)itemDto).InstanceData = new SimpleNameValueCollection(GetInstanceDataValuesForSerialization());
                ((ItemProxyObject)itemDto).Metadata = new SimpleNameValueCollection(GetMetaDataValuesForSerialization());
                ((ItemProxyObject) itemDto).ParentTemplateId = TemplateID ?? -1;
            }
        }

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// Per MSDN, this method should return a null value.  The best way to include schema
        /// information is to
        /// </summary>
        /// <returns></returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="reader"></param>
        public void ReadXml(XmlReader reader)
        {
        }

        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="writer"></param>
        public void WriteXml(XmlWriter writer)
        {
            WriteXml(writer, false);
        }

        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="isText"></param>
        public void WriteXml(XmlWriter writer, bool isText)
        {
            //Write the top level element
            writer.WriteStartElement(XmlElementName);

            //Add the id, visible, & excluded flags
            writer.WriteAttributeString("itemId", ID.ToString());
            writer.WriteAttributeString("type", ItemTypeName);
            writer.WriteAttributeString("alias", Alias ?? string.Empty);
            writer.WriteAttributeString("isVisible", Visible.ToString());
            writer.WriteAttributeString("isExcluded", Excluded.ToString());
            writer.WriteAttributeString("isActive", IsActive.ToString());
            
            //Now write metadata
            writer.WriteStartElement(XmlMetaDataElementName);
            WriteXmlMetaData(writer);
            writer.WriteEndElement();

            //Write instance data
            writer.WriteStartElement(XmlInstanceDataElementName);
            WriteXmlInstanceData(writer, isText);
            writer.WriteEndElement();

            //Write the end element
            writer.WriteEndElement();
        }

        #endregion

        #region Xml Serialization Helpers

        /// <summary>
        /// Get xml element name for item
        /// </summary>
        protected virtual string XmlElementName
        {
            get { return "item"; }
        }

        /// <summary>
        /// Get the name of the element containing information
        /// specific to this instance of the item.
        /// </summary>
        protected virtual string XmlInstanceDataElementName
        {
            get { return "instanceData"; }
        }

        /// <summary>
        /// Get name of element containing meta data for the item,
        /// such as number to select, default values, etc.
        /// </summary>
        protected virtual string XmlMetaDataElementName
        {
            get { return "metaData"; }
        }

        /// <summary>
        /// Write metadata for the item.  Such data includes business rule
        /// configuration, etc.
        /// </summary>
        /// <param name="writer"></param>
        public virtual void WriteXmlMetaData(XmlWriter writer)
        {
            XmlUtility.SerializeNameValueCollection(
                writer,
                GetMetaDataValuesForSerialization(),
                true);
        }

        /// <summary>
        /// Write xml instance data, such as the item's current state, etc.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="isText"></param>
        public virtual void WriteXmlInstanceData(XmlWriter writer, bool isText)
        {
            XmlUtility.SerializeNameValueCollection(
               writer,
               GetInstanceDataValuesForSerialization(),
               true);
        }

        /// <summary>
        /// Get a name value collection of data to serialize to xml.
        /// </summary>
        /// <returns></returns>
        protected virtual NameValueCollection GetMetaDataValuesForSerialization()
        {
            return new NameValueCollection();
        }

        /// <summary>
        /// Get a name value collection of data to serialize to xml.
        /// </summary>
        /// <returns></returns>
        protected virtual NameValueCollection GetInstanceDataValuesForSerialization()
        {
            return new NameValueCollection();
        }

        #endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="srcItem"></param>
		/// <param name="answers"></param>
		internal protected virtual void ImportAnswers(Checkbox.Analytics.Import.ItemInfo srcItem, List<Checkbox.Analytics.Data.ItemAnswer> answers)
		{
			//throw new NotImplementedException("Method ImportAnswer(..) is not implemented for type " + this.GetType().Name);
		}


        internal virtual void InitializeDefaults()
        {            
        }

    }
}
