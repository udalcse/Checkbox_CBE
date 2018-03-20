using System;
using System.Data;
using System.Web;
using System.Web.UI;

using Prezza.Framework.Data;

namespace Checkbox.Web.Page
{
    /// <summary>
    /// A custom PageStatePersister which persist the ViewState to a database.
    /// </summary>
    public class DatabasePageStatePersister : PageStatePersister
    {
        private readonly string _guid = string.Empty;
        private readonly string _sessionId = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string GUID
        {
            get { return _guid; }
        }

        /// <summary>
        /// 
        /// </summary>
        private string SessionId
        {
            get { return _sessionId; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="p">Requesting Page.</param>
        /// <param name="guid">ViewState data unique identifier.</param>
        /// <param name="sessionId">Session id.</param>
        public DatabasePageStatePersister(System.Web.UI.Page p, string guid, string sessionId) : base(p)
        {
            _guid = guid;
            _sessionId = sessionId;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Load()
        {
            try
            {
                string serializedState = GetState(GUID);

                IStateFormatter formatter = StateFormatter;
                Pair statePair = (Pair)formatter.Deserialize(serializedState);

                ViewState = statePair.First;
                ControlState = statePair.Second;
            }
            catch (Exception)
            {
                //An exception is thrown when a loss of state occurs. 
                //This exception is intentionally disregarded as there is logic in place that handles this scenario.
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Save()
        {
            IStateFormatter formatter = StateFormatter;
            Pair statePair = new Pair(ViewState, ControlState);
            string serializedState = formatter.Serialize(statePair);

            SaveState(GUID, serializedState);
        }

        /// <summary>
        /// Save the ViewState data to the database.
        /// </summary>
        /// <param name="guid">GUID which uniquely identifies the ViewState data.</param>
        /// <param name="data">ViewState data.</param>
        private void SaveState(string guid, string data)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase();
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ViewStateSave");
                command.AddInParameter("GUID", DbType.String, guid);
                command.AddInParameter("SessionID", DbType.String, SessionId);
                command.AddInParameter("ViewState", DbType.String, data);

                db.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
                throw new HttpException("Unable to store ViewState", ex);
            }
        }

        /// <summary>
        /// Retrieve the ViewState data from the database.
        /// </summary>
        /// <param name="guid">GUID which uniquely identifies the ViewState data.</param>
        /// <returns></returns>
        private string GetState(string guid)
        {
            string persistedState;

            try
            {
                Database db = DatabaseFactory.CreateDatabase();
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ViewStateLoad");
                command.AddInParameter("GUID", DbType.String, guid);

                persistedState = (string)db.ExecuteScalar(command);
            }
            catch (Exception ex)
            {
                throw new HttpException("Unable to load ViewState", ex);
            }

            return persistedState;
        }
    }
}