using System;
using System.Collections.Generic;
using System.Data;
using Checkbox.Common;
using Prezza.Framework.Data;
using Prezza.Framework.ExceptionHandling;
using Prezza.Framework.Security;

namespace Checkbox.Panels
{
    /// <summary>
    /// Abstract base for panels
    /// </summary>
    [Serializable]
    public abstract class Panel : AccessControllablePersistedDomainObject, IPanel
    {
        private List<Panelist> _panelists;

        private List<Panelist> _addedPanelists;
        private List<Panelist> _removedPanelists;

        protected Panel(string[] supportedPermissionMasks, string[] supportedPermissions) : base(supportedPermissionMasks, supportedPermissions) { }

        /// <summary>
        /// Get id of panel
        /// </summary>
        public int PanelId { get { return ID.HasValue ? ID.Value : 0; } }

        /// <summary>
        /// Get type of object
        /// </summary>
        public override string ObjectTypeName { get { return "InvitationPanel"; } }

        //TODO: Upgrade task to add panel type names to panel type table
        /// <summary>
        /// Get type name of panel
        /// </summary>
        public abstract string PanelTypeName { get; }

        /// <summary>
        /// Get type id of panel
        /// </summary>
        public int PanelTypeId { get; protected set; }

        /// <summary>
        /// Get list of panelists
        /// </summary>
        public List<Panelist> Panelists
        {
            get
            {
                if (_panelists == null)
                {
                    _panelists = new List<Panelist>();
                    LoadPanelists();
                }

                return _panelists;
            }
        }


        /// <summary>
        /// Get collection of added panelists
        /// </summary>
        protected List<Panelist> AddedPanelists
        {
            get
            {
                if (_addedPanelists == null)
                {
                    _addedPanelists = new List<Panelist>();
                }

                return _addedPanelists;
            }
        }

        /// <summary>
        /// Get collection of added panelists
        /// </summary>
        protected List<Panelist> RemovedPanelists
        {
            get
            {
                if (_removedPanelists == null)
                {
                    _removedPanelists = new List<Panelist>();
                }

                return _removedPanelists;
            }
        }

        /// <summary>
        /// Get/set the panel description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Get/set the panel's creator
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Get/set the panel's editor
        /// </summary>
        public string ModifiedBy { get; set; }

        /// <summary>
        /// Get procedure used to load panel
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_Panel_GetPanel"; } }

        /// <summary>
        /// Create a panel configuration data set.
        /// </summary>
        /// <returns></returns>
        protected override PersistedDomainObjectDataSet CreateConfigurationDataSet()
        {
            return new PanelDataSet(ObjectTypeName);
        }

        /// <summary>
        /// Load panelists for the panel.  Added separately to support
        /// lazy loading.
        /// </summary>
        private void LoadPanelists()
        {
            AddedPanelists.Clear();
            RemovedPanelists.Clear();

            _panelists.AddRange(GetPanelists());
        }

        /// <summary>
        /// Get panelists
        /// </summary>
        /// <returns></returns>
        protected abstract List<Panelist> GetPanelists();

        /// <summary>
        /// Get the specfied panelist
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public virtual Panelist GetPanelist(string identifier)
        {
            return Panelists.Find(p => p.Email.Equals(identifier, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Update the panel
        /// </summary>
        /// <param name="t"></param>
        protected override void Update(IDbTransaction t)
        {
            Database db = DatabaseFactory.CreateDatabase();

            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Panel_Update");

            command.AddInParameter("PanelID", DbType.Int32, ID.Value);
            command.AddInParameter("Name", DbType.String, Name);
            command.AddInParameter("Description", DbType.String, Description);
            command.AddInParameter("ModifiedBy", DbType.String, ModifiedBy);

            db.ExecuteNonQuery(command, t);

            UpdatePanelists(t);
        }

        /// <summary>
        /// Insert the panel
        /// </summary>
        /// <param name="t"></param>
        protected override void Create(IDbTransaction t)
        {
            Database db = DatabaseFactory.CreateDatabase();

            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Panel_Insert");
            command.AddInParameter("Name", DbType.String, Name);
            command.AddInParameter("Description", DbType.String, Description);
            command.AddInParameter("DateCreated", DbType.DateTime, DateTime.Now);
            command.AddInParameter("CreatedBy", DbType.String, CreatedBy);
            command.AddInParameter("PanelTypeName", DbType.String, PanelTypeName);
            command.AddOutParameter("PanelID", DbType.Int32, 4);

            db.ExecuteNonQuery(command, t);

            //Set the item id so it can be used by the derived class' create method
            object id = command.GetParameterValue("PanelID");

            if (id == null || id == DBNull.Value)
            {
                throw new Exception("Unable to save Panel data.");
            }

            ID = (int)id;

            UpdatePanelists(t);
        }

        /// <summary>
        /// Update the list of panelists
        /// </summary>
        /// <param name="t"></param>
        protected virtual void UpdatePanelists(IDbTransaction t)
        {

        }

        /// <summary>
        /// Delete the panel
        /// </summary>
        /// <param name="transaction"></param>
        public override void Delete(IDbTransaction transaction)
        {
            if (ID == null || ID <= 0)
            {
                return;
            }

            try
            {
                Database db = DatabaseFactory.CreateDatabase();
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Panel_Delete");
                command.AddInParameter("PanelID", DbType.Int32, ID.Value);
                db.ExecuteNonQuery(command, transaction);
            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessPublic");

                if (rethrow)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Load the panel from the row
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadBaseObjectData(DataRow data)
        {
            base.LoadBaseObjectData(data);

            Name = DbUtility.GetValueFromDataRow(data, "Name", string.Empty);
            Description = DbUtility.GetValueFromDataRow(data, "Description", string.Empty);
            CreatedDate = DbUtility.GetValueFromDataRow<DateTime?>(data, "DateCreated", null);
            CreatedBy = DbUtility.GetValueFromDataRow(data, "CreatedBy", string.Empty);
            PanelTypeId = DbUtility.GetValueFromDataRow(data, "PanelTypeId", -1);
        }

        /// <summary>
        /// Currently, only email panels are access controllable, but eventually more panels will
        /// be.
        /// </summary>
        public override string DomainDBTableName
        {
            get { return "ckbx_Panel"; }
        }

        /// <summary>
        /// Currently, only email panels are access controllable, but eventually more panels will
        /// be.
        /// </summary>
        public override string DomainDBIdentityColumnName
        {
            get { return "PanelID"; }
        }

        /// <summary>
        /// Currently, only email panels are access controllable, but eventually more panels will
        /// be.
        /// </summary>
        /// <returns></returns>
        public override SecurityEditor GetEditor()
        {
            throw new NotImplementedException();
        }
    }
}
