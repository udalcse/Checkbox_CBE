using System.Data;

using Prezza.Framework.Data.Sprocs;
using Prezza.Framework.Security;

namespace Checkbox.Panels
{
    /// <summary>
    /// Lightweight representation of a panel suitable for
    /// the authorization purposes.
    /// </summary>
    [FetchProcedure("ckbx_sp_Panel_GetAccessControllableResource")]
    public class LightweightPanel : LightweightAccessControllable
    {
        private int _panelID;

        /// <summary>
        /// Internal constructor
        /// </summary>
        internal LightweightPanel(int panelID)
        {
            _panelID = panelID;
        }

        /// <summary>
        /// Get/set the template id
        /// </summary>
        [FetchParameter(Name="PanelID", DbType=DbType.Int32, Direction=ParameterDirection.Input)]
        public int PanelID
        {
            get { return _panelID; }
            set { _panelID = value; }
        }
    }
}