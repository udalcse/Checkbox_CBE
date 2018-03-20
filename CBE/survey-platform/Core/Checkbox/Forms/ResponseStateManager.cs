using System;
using System.Data;
using Checkbox.Security.Principal;
using Prezza.Framework.Data;
using Checkbox.Forms.Security.Principal;

namespace Checkbox.Forms
{
    
    /// <summary>
    /// Get response guids
    /// </summary>
    public static class ResponseStateManager
    {
        /// <summary>
        /// Get the response state for the specified guid
        /// </summary>
        /// <param name="responseGuid"></param>
        /// <returns></returns>
        public static ResponseState GetResponseState(Guid responseGuid)
        {
            ResponseState state = new ResponseState();
            state.Load(responseGuid);
            return state;
        }

        /// <summary>
        /// Get the last incomplete response for an anonymous user
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="responseTemplateID"></param>
        /// <returns></returns>
        public static ResponseState GetLastIncompleteResponse(CheckboxPrincipal principal, Int32 responseTemplateID)
        {
            DataRow responseRow = GetLastIncompleteResponseRow(principal, responseTemplateID);

            if (responseRow == null)
            {
                return null;
            }
            
            ResponseState state = new ResponseState();
            state.Load((Guid)responseRow["Guid"]);
            return state;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="responseTemplateId"></param>
        /// <returns></returns>
        public static DataRow GetLastIncompleteResponseRow(CheckboxPrincipal principal, Int32 responseTemplateId)
        {
            DataTable responseTable;

            if (principal is AnonymousRespondent)
            {
                responseTable = GetResponseTable(principal.UserGuid, responseTemplateId);
            }
            else
            {
                responseTable = GetResponseTable(principal.Identity.Name, responseTemplateId);
            }

            if (responseTable == null)
            {
                return null;
            }

            DataRow[] rows = responseTable.Select("IsComplete IS NULL OR IsComplete = 0", "Started DESC", DataViewRowState.CurrentRows);

            if (rows.Length == 0)
            {
                return null;
            }

            return rows[0];
        }

        /// <summary>
        /// Get the completed response list for a user
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="responseTemplateID"></param>
        /// <returns></returns>
        public static DataTable GetCompletedResponseList(CheckboxPrincipal principal, Int32 responseTemplateID)
        {
            DataTable responseList;

            if (principal is AnonymousRespondent)
            {
                responseList = GetResponseTable(principal.UserGuid, responseTemplateID);
            }
            else
            {
                responseList = GetResponseTable(principal.Identity.Name, responseTemplateID);
            }

            if (responseList == null)
            {
                return null;
            }

            DataTable completedList = responseList.Clone();

            DataRow[] completedResponses = responseList.Select("IsComplete IS NOT NULL AND IsComplete = 1", "Started DESC", DataViewRowState.CurrentRows);

            foreach (DataRow row in completedResponses)
            {
                completedList.ImportRow(row);
            }

            return completedList;
        }


        /// <summary>
        /// Get a data table containing response information for a user
        /// </summary>
        /// <param name="uniqueIdentifier"></param>
        /// <param name="responseTemplateID"></param>
        /// <returns></returns>
        private static DataTable GetResponseTable(string uniqueIdentifier, Int32 responseTemplateID)
        {
            DataSet ds = new DataSet();

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Response_GetListForUser");
            command.AddInParameter("UniqueIdentifier", DbType.String, uniqueIdentifier);
            command.AddInParameter("ResponseTemplateID", DbType.Int32, responseTemplateID);

            db.LoadDataSet(command, ds, "ResponseList");

            return ds.Tables["ResponseList"];
        }

        /// <summary>
        /// Get a data table containing response information for a user
        /// </summary>
        /// <param name="anonymousUserGuid"></param>
        /// <param name="responseTemplateID"></param>
        /// <returns></returns>
        private static DataTable GetResponseTable(Guid anonymousUserGuid, Int32 responseTemplateID)
        {
            DataSet ds = new DataSet();

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Response_GetListForAnonymousUser");
            command.AddInParameter("RespondentGuid", DbType.Guid, anonymousUserGuid);
            command.AddInParameter("ResponseTemplateID", DbType.Int32, responseTemplateID);

            db.LoadDataSet(command, ds, "ResponseList");

            return ds.Tables["ResponseList"];
        }
    }
}
