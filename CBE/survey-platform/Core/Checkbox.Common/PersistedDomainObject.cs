using System;
using System.Collections.Generic;
using System.Data;
using System.ComponentModel;
using System.Data.Common;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Prezza.Framework.Data;
using Prezza.Framework.Common;
using Prezza.Framework.ExceptionHandling;

namespace Checkbox.Common
{
    /// <summary>
    /// Base class for domain objects that persist data.  This class provides functionality for xml serialization, and transactional
    /// operations.
    /// </summary>
    [Serializable]
    public abstract class PersistedDomainObject : ITransactional, IDisposable
    {
        #region Members

        private int? _id;

        //Used for rollback & transaction management
        private int? _originalDataID;
        private bool _objectSaving;

        [NonSerialized]
        private EventHandlerList _eventHandlers;
        #endregion

        /// <summary>
        /// Get the data id of the item
        /// </summary>
        public int? ID
        {
            get { return _id; }
            set
            {
                _id = value;
                SetOriginalID(_id);
            }
        }

        /// <summary>
        /// Get name of persisted object type
        /// </summary>
        public abstract string ObjectTypeName { get; }

        /// <summary>
        /// Get timestamp of when item was last persisted
        /// </summary>
        [XmlIgnore]
        public DateTime? LastModified { get; protected set; }

        /// <summary>
        /// Who modified the template
        /// </summary>
        [XmlIgnore]
        public object ModifiedBy { get; set; }

        /// <summary>
        /// Get timestamp of when item was created
        /// </summary>
        [XmlIgnore]
        public DateTime? CreatedDate { get; protected set; }

        /// <summary>
        /// Get name of load data stored procedure
        /// </summary>
        protected abstract string LoadSprocName { get; }

        /// <summary>
        /// Get the name of the datatable containing configuration for this PersistedDomainObject.  This name will
        /// be used when searching for object data within a dataset.
        /// </summary>
        public virtual string DataTableName
        {
            get { throw new NotImplementedException(GetType().Name + ".DataTableName property is not implemented"); }
        }

        /// <summary>
        /// Get name of element for exporting/writing to XML
        /// </summary>
        public virtual string ExportElementName { get { return DataTableName; } }

        /// <summary>
        /// Get the name of the column of the <see cref="DataTable"/> with a name of <see cref="DataTableName"/> that
        /// contains identities for persisted objects.
        /// </summary>
        public virtual string IdentityColumnName
        {
            get { throw new NotImplementedException(GetType().Name + ".IdentityColumnName property is not implemented"); }
        }

        /// <summary>
        /// Get a list of parameters to pass to a load stored procedure
        /// </summary>
        /// <returns></returns>
        protected virtual List<DbParameter> GetLoadSprocParameters(PersistedDomainObjectDataSet dataSet)
        {
            return new List<DbParameter> { new GenericDbParameter(dataSet.IdentityColumnName, DbType.Int32, ID) };
        }

        /// <summary>
        /// Set the original id
        /// </summary>
        /// <param name="id"></param>
        protected void SetOriginalID(int? id)
        {
            _originalDataID = id;
        }

        #region Abstract Methods

        /// <summary>
        /// Create the object
        /// </summary>
        /// <param name="t">an <see cref="IDbTransaction"/> in which to participate</param>
        protected abstract void Create(IDbTransaction t);

        /// <summary>
        /// Update the object
        /// </summary>
        /// <param name="t">an <see cref="IDbTransaction"/> in which to participate</param>
        protected abstract void Update(IDbTransaction t);

        /// <summary>
        /// Delete the object
        /// </summary>
        /// <param name="t">an <see cref="IDbTransaction"/> in which to participate</param>
        public virtual void Delete(IDbTransaction t)
        {
            OnDeleting(t);
        }

        /// <summary>
        /// Get a dataset containing configuration for this object.
        /// </summary>
        /// <returns>a <see cref="DataSet"/></returns>
        public virtual PersistedDomainObjectDataSet GetConfigurationDataSet(int objectId)
        {
            PersistedDomainObjectDataSet ds = CreateConfigurationDataSet();

            LoadConfigurationDataSet(ds);

            return ds;
        }

        /// <summary>
        /// Use it for optimization purposes. It should load only specific configuration data for object.
        /// Base object data should be loaded by other ways
        /// </summary>
        /// <returns>a <see cref="DataSet"/></returns>
        public PersistedDomainObjectDataSet GetSpecificConfigurationDataSet(int objectId)
        {
            PersistedDomainObjectDataSet ds = CreateConfigurationDataSet();

            LoadSpecificConfigurationDataSet(ds);

            return ds;
        }

        /// <summary>
        /// Create the configuration data set.
        /// </summary>
        /// <returns></returns>
        protected abstract PersistedDomainObjectDataSet CreateConfigurationDataSet();

        /// <summary>
        /// Populate the configuration data set.
        /// </summary>
        /// <param name="dataSet"></param>
        protected virtual void LoadConfigurationDataSet(PersistedDomainObjectDataSet dataSet)
        {
            if (ID.HasValue)
            {
                dataSet.Load(ID.Value, LoadSprocName, GetLoadSprocParameters(dataSet));
                if (string.IsNullOrEmpty(LoadSprocName)) 
                    dataSet.OwningObjectId = ID.Value;
            }
        }

        /// <summary>
        /// Use it for optimization purposes. It should load only specific configuration data for object.
        /// Base object data should be loaded by other ways 
        /// </summary>
        /// <param name="dataSet"></param>
        protected void LoadSpecificConfigurationDataSet(PersistedDomainObjectDataSet dataSet)
        {
            if (ID.HasValue)
            {
                dataSet.LoadDataSet(ID.Value, LoadSprocName, GetLoadSprocParameters(dataSet));
                if (string.IsNullOrEmpty(LoadSprocName))
                    dataSet.OwningObjectId = ID.Value;
            }
        }

        /// <summary>
        /// Set column mappings for the dataset (e.g. Attribute, etc).  These mappings are used when the dataset is serialized to XML.
        /// </summary>
        /// <param name="data"></param>
        public virtual void SetConfigurationDataSetColumnMappings(DataSet data)
        {
        }

        #endregion

        #region Virtual Methods
        /// <summary>
        /// Save as part of a transaction
        /// </summary>
        /// <param name="t">an <see cref="IDbTransaction"/> in which to participate</param>
        public virtual void Save(IDbTransaction t)
        {
            if (!_objectSaving)
            {
                OnSaving(t);

                if (ID.HasValue && ID.Value > 0)
                {
                    Update(t);
                }
                else
                {
                    Create(t);
                }
                OnSaved();
            }
        }

        /// <summary>
        /// Save.  A transaction will be created
        /// </summary>
        public virtual void Save()
        {
            Database db = DatabaseFactory.CreateDatabase();

            using (IDbConnection connection = db.GetConnection())
            {
                connection.Open();
                IDbTransaction transaction = connection.BeginTransaction();
                DateTime? previousModified = LastModified;

                try
                {
                    LastModified = DateTime.Now;

                    Save(transaction);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "BusinessProtected");

                    transaction.Rollback();
                    LastModified = previousModified;
                    OnAbort(this, EventArgs.Empty);

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }

            //Call on commit only on success
            OnCommit(this, EventArgs.Empty);
        }

        /// <summary>
        /// Delete, a transaction will be created
        /// </summary>
        public virtual void Delete()
        {
            Database db = DatabaseFactory.CreateDatabase();

            using (IDbConnection connection = db.GetConnection())
            {
                connection.Open();
                IDbTransaction transaction = connection.BeginTransaction();

                try
                {
                    Delete(transaction);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "BusinessProtected");

                    transaction.Rollback();
                    OnAbort(this, EventArgs.Empty);

                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }

            //Now commit
            OnCommit(this, EventArgs.Empty);
        }

#if TRACE_LOADED_OBJECTS
        static Dictionary<string, int> _requests = new Dictionary<string, int>();
#endif

        /// <summary>
        /// Loads a persisted object based on its Database ID
        /// </summary>
        public virtual void Load()
        {
            //Get the data from the database
            if (!ID.HasValue || ID <= 0)
            {
                throw new Exception("DataID must be greater than zero.");
            }

            //some code that helps to trace which objects were loaded and how many times
#if TRACE_LOADED_OBJECTS
            string key = "Loading " + this.GetType().FullName + " ID = " + ID.ToString();
            int cnt = 1;

            if (_requests.ContainsKey(key))
            {
                cnt = _requests[key];
                _requests[key] = cnt + 1;
            }
            else
                _requests[key] = 1;

            System.Diagnostics.Trace.WriteLine(string.Format("{0} ({1})", key, cnt));
#endif

            Load(GetConfigurationDataSet(ID.Value));
        }

        /// <summary>
        /// Load a persisted object using its database ID
        /// </summary>
        /// <param name="dataId">the database ID of the object to load</param>
        public virtual void Load(Int32 dataId)
        {
            ID = dataId;
            _originalDataID = dataId;

            Load();
        }

        /// <summary>
        /// Load from serialized data.
        /// </summary>
        /// <param name="xmlNode"></param>
        public virtual void Import(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
        {
            PersistedDomainObjectDataSet data = CreateConfigurationDataSet();
            data.ReadXml(new XmlNodeReader(xmlNode));

            Load(data);

            if(callback != null)
            {
                callback(this, xmlNode, creator);
            }
        }

        /// <summary>
        /// Loads the object data from a <see cref="DataSet"/>
        /// </summary>
        public virtual void Load(PersistedDomainObjectDataSet data)
        {
            // load the object from a DataSet.  This dataset would be part of a large batch select.  
            // the individual objects can query the dataset for their data...
            if (data != null)
            {
                LoadBaseObjectData(data.DomainObjectDataRow);
                LoadAdditionalData(data);
            }
            else
            {
                Load();
            }
        }

        /// <summary>
        /// Loads a persisted object data from the supplied <see cref="DataRow"/>.
        /// </summary>
        /// <param name="data"></param>
        protected virtual void LoadBaseObjectData(DataRow data) { }

        /// <summary>
        /// Load additional, related data for the object.
        /// </summary>
        /// <param name="data"></param>
        protected virtual void LoadAdditionalData(PersistedDomainObjectDataSet data) { }

        #endregion

        #region ITransactional Members
        private static readonly object _eventTransactionAborted = new object();
        private static readonly object _eventTransactionCommitted = new object();


        /// <summary>
        /// Transaction Abort
        /// </summary>
        public event EventHandler TransactionAborted
        {
            add
            {
                // here we may want to mark the object state to rollback to
                Events.AddHandler(_eventTransactionAborted, value);
            }
            remove { Events.RemoveHandler(_eventTransactionAborted, value); }
        }

        /// <summary>
        /// Transaction Committed
        /// </summary>
        public event EventHandler TransactionCommitted
        {
            add { Events.AddHandler(_eventTransactionCommitted, value); }
            remove { Events.RemoveHandler(_eventTransactionCommitted, value); }
        }

        /// <summary>
        /// Begin rollback processing when the transaction that this object's load/save was a part of
        /// has been rolled back.
        /// </summary>
        public void Rollback()
        {
            OnRollback();
        }

        /// <summary>
        /// Perform processing for rollback of save for this item.
        /// </summary>
        protected virtual void OnRollback()
        {
            // for now, we just assume that nullifying the ID is equivalent to object rollback
            //TODO:  mark the ID of the Item when wired to the Event and rollback to that.
            // this way, if there was an ID already, it won't get nullified.
            ID = _originalDataID;
        }

        /// <summary>
        /// Notify the object that the transaction its load/save was part of has been committed.
        /// </summary>
        public void NotifyCommit(object sender, EventArgs e)
        {
            OnCommit(sender, e);
        }

        /// <summary>
        /// Notify the persisted domain object that the transaction it's save was part of has been
        /// committed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnCommit(object sender, EventArgs e)
        {
            _objectSaving = false;

            // unwire from the sender
            if (sender != null)
            {
                ((ITransactional)sender).TransactionAborted -= OnAbort;
                ((ITransactional)sender).TransactionCommitted -= OnCommit;
            }


            var handler = (EventHandler)Events[_eventTransactionCommitted];
            if (handler != null)
                handler(this, null);
        }

        /// <summary>
        /// Notify the persisted domain object that the transaction it's save was part of has been
        /// aborted.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void NotifyAbort(object sender, EventArgs e)
        {
            OnAbort(sender, e);
        }

        /// <summary>
        /// Perform abort processing when the transaction that this object's load/save participated in has been aborted.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnAbort(object sender, EventArgs e)
        {
            // rollback this object's state
            Rollback();

            _objectSaving = false;

            // unwire from the sender
            if (sender != null)
            {
                ((ITransactional)sender).TransactionAborted -= OnAbort;
                ((ITransactional)sender).TransactionCommitted -= OnCommit;
            }

            // fire the event for any observers to use
            var handler = (EventHandler)Events[_eventTransactionAborted];
            if (handler != null)
                handler(this, null);
        }
        #endregion

        #region Eventing
        /// <summary>
        /// Get the event handler list
        /// </summary>
        protected EventHandlerList Events
        {
            get { return _eventHandlers ?? (_eventHandlers = new EventHandlerList()); }
        }

        private static readonly object _eventSaved = new object();
        private static readonly object _eventSaving = new object();
        private static readonly object _eventDeleted = new object();
        private static readonly object _eventDeleting = new object();

        /// <summary>
        /// Fired after the call to Save() but before the object is saved
        /// </summary>
        public event PersistedDomainObjectSaveEventHandler Saving
        {
            add { Events.AddHandler(_eventSaving, value); }
            remove { Events.RemoveHandler(_eventSaving, value); }
        }

        /// <summary>
        /// Fired after the call to Delete() but before the object is deleted
        /// </summary>
        public event PersistedDomainObjectSaveEventHandler Deleting
        {
            add { Events.AddHandler(_eventDeleting, value); }
            remove { Events.RemoveHandler(_eventDeleting, value); }
        }

        /// <summary>
        /// Fired after the object has been saved
        /// </summary>
        public event EventHandler Saved
        {
            add { Events.AddHandler(_eventSaved, value); }
            remove { Events.RemoveHandler(_eventSaved, value); }
        }

        /// <summary>
        /// Fired after the object has been deleted
        /// </summary>
        public event EventHandler Deleted
        {
            add { Events.AddHandler(_eventDeleted, value); }
            remove { Events.RemoveHandler(_eventDeleted, value); }
        }

        /// <summary>
        /// Overridable handler for saved event.
        /// </summary>
        protected virtual void OnSaved()
        {
            _objectSaving = false;

            var handler = (EventHandler)Events[_eventSaved];
            if (handler != null)
                handler(this, null);
        }

        /// <summary>
        /// Overridable handler for saving event.  Base implementation sets a flag indicating a save is in progress.
        /// This flag is cleared by OnCommit() or OnAbort()
        /// </summary>
        /// <param name="t"></param>
        protected virtual void OnSaving(IDbTransaction t)
        {
            _objectSaving = true;

            var handler = (PersistedDomainObjectSaveEventHandler)Events[_eventSaving];
            if (handler != null)
            {
                var e = new PersistedDomainObjectSaveEventArgs(t);
                handler(this, e);
            }
        }

        /// <summary>
        /// Overridable handler for deleting event.
        /// </summary>
        /// <param name="t"></param>
        protected virtual void OnDeleting(IDbTransaction t)
        {
            var handler = (PersistedDomainObjectSaveEventHandler)Events[_eventDeleting];
            if (handler != null)
            {
                var e = new PersistedDomainObjectSaveEventArgs(t);
                handler(this, e);
            }
        }

        /// <summary>
        /// Overridable handler for deleted event.
        /// </summary>
        protected virtual void OnDeleted()
        {
            var handler = (EventHandler)Events[_eventDeleted];
            if (handler != null)
                handler(this, null);
        }
        #endregion

        /// <summary>
        /// Delegate for persisted domain object saving events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void PersistedDomainObjectSaveEventHandler(object sender, PersistedDomainObjectSaveEventArgs e);

        /// <summary>
        /// Delegate for writing exernal data when exporting object to xml
        /// </summary>
        /// <param name="writer"></param>
        public delegate void WriteExternalDataCallback(PersistedDomainObject obj, XmlWriter writer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="xmlnode"></param>
        public delegate void ReadExternalDataCallback(PersistedDomainObject obj, XmlNode xmlnode, object creator);

        #region IXmlSerializable Members
        /// <summary>
        /// Get the schema for this object
        /// </summary>
        /// <returns></returns>
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Serialize this object to XML
        /// </summary>
        /// <param name="writer"></param>
        public void Export(XmlWriter writer)
        {
            if (!ID.HasValue || ID <= 0)
            {
                throw new Exception("Unable to export object with null or negative id.");
            }

            //Write document header & checkbox version
            writer.WriteStartDocument();

            //Write data
            WriteXml(writer);
            
            //End data
            writer.WriteEndDocument();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="externalDataCallback"></param>
        public void WriteXml(XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
        {
            writer.WriteStartElement(ExportElementName);
            WriteExportAttributes(writer);

            writer.WriteElementString("ExportDate", DateTime.Now.ToString());

            WriteConfigurationToXml(writer, externalDataCallback);

            if(externalDataCallback != null)
            {
                externalDataCallback(this, writer);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public virtual void WriteExportAttributes(XmlWriter writer)
        {
            writer.WriteAttributeString("Version", Assembly.GetAssembly(GetType()).GetName().Version.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected virtual void WriteConfigurationToXml(XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
        {
            //Get the object's configuration
            PersistedDomainObjectDataSet configuration = GetConfigurationDataSet(ID.Value);

            //Write the XML
            configuration.WriteXml(writer, XmlWriteMode.IgnoreSchema);
        }

     
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Dispose of the item
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Overridable dispose method
        /// </summary>
        /// <param name="isDisposing"></param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                if (_eventHandlers != null)
                {
                    _eventHandlers.Dispose();
                }
            }
        }
        #endregion

    }

    /// <summary>
    /// Event arguments for saving persisted domain objects.
    /// </summary>
    public class PersistedDomainObjectSaveEventArgs : EventArgs
    {
        private readonly IDbTransaction _transaction;

        /// <summary>
        /// Constructor that accepts the transaction context the save is occurring in.
        /// </summary>
        /// <param name="transaction"></param>
        public PersistedDomainObjectSaveEventArgs(IDbTransaction transaction)
        {
            _transaction = transaction;
        }

        /// <summary>
        /// Get the transaction contexts for the save operation.
        /// </summary>
        public IDbTransaction Transaction
        {
            get { return _transaction; }
        }
    }
}