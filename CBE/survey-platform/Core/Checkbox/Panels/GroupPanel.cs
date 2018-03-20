using System;
using System.Collections.Generic;
using System.Data;
using Checkbox.Users;

using Prezza.Framework.Data;


namespace Checkbox.Panels
{
    /// <summary>
    /// Group panel
    /// </summary>
    [Serializable]
    public class GroupPanel : Panel
    {
        private Group _group;

        /// <summary>
        /// Constructor
        /// </summary>
        public GroupPanel()
            : base(new string[] { }, new string[] { })
        {

        }

        /// <summary>
        /// Get panel type name
        /// </summary>
        public override string PanelTypeName { get { return "Checkbox.Panels.GroupPanel"; } }

        /// <summary>
        /// Get/set group id of group associated w/panel
        /// </summary>
        public int? GroupId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Group"/> for this GroupPanel
        /// </summary>
        protected Group Group
        {
            get
            {
                if (_group == null && GroupId.HasValue)
                {
                    _group = GroupManager.GetGroup(GroupId.Value);
                }

                return _group;
            }
        }

        /// <summary>
        /// Create the group panel
        /// </summary>
        /// <param name="t"></param>
        protected override void Create(IDbTransaction t)
        {
            base.Create(t);

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper insert = db.GetStoredProcCommandWrapper("ckbx_sp_Panel_InsertGroupPanel");
            insert.AddInParameter("PanelID", DbType.Int32, ID.Value);
            insert.AddInParameter("GroupID", DbType.Int32, Group.ID);
            db.ExecuteNonQuery(insert, t);
        }

        /// <summary>
        /// Get list of panelists
        /// </summary>
        /// <returns></returns>
        protected override List<Panelist> GetPanelists()
        {
            List<Panelist> panelists = new List<Panelist>();

            string[] userIdentifiers = Group.GetUserIdentifiers();

            foreach (string userIdentifier in userIdentifiers)
            {
                panelists.Add(new UserPanelist { UniqueIdentifier = userIdentifier });
            }

            return panelists;
        }

        /// <summary>
        /// Get a particular panelist
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public override Panelist GetPanelist(string identifier)
        {
            return Panelists.Find(p => ((UserPanelist)p).UniqueIdentifier.Equals(identifier, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Load the panel from the datarow
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadBaseObjectData(DataRow data)
        {
            base.LoadBaseObjectData(data);

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Panel_GetGroupPanel");
            command.AddInParameter("PanelID", DbType.Int32, ID.Value);

            object groupRecord = db.ExecuteScalar(command);
            if (groupRecord != DBNull.Value && groupRecord != null)
            {
                GroupId = (int)groupRecord;
            }
        }
    }
}
