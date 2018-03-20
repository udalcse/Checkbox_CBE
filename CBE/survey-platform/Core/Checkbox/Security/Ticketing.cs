using System;
using System.Data;

using Prezza.Framework.Data;
using Prezza.Framework.ExceptionHandling;

namespace Checkbox.Security
{
    /// <summary>
    /// Simple static class for storing and retrieving tickets.  Currently, the logic contained is pretty limited but
    /// it can eventually be extended to be more useful
    /// </summary>
    public static class Ticketing
    {
        /// <summary>
        /// Insert a ticket with the specified ID and expiration into the database.  If a ticket with the specified ID
        /// exists, a new ticket will not be inserted but the expiration will be updated.
        /// </summary>
        /// <param name="ticketID">GUID of the ticket</param>
        /// <param name="expiration">Ticket expiration date</param>
        public static void CreateTicket(Guid ticketID, DateTime expiration)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper commmand = db.GetStoredProcCommandWrapper("ckbx_sp_Ticket_Insert");

            commmand.AddInParameter("TicketGUID", DbType.Guid, ticketID);
            commmand.AddInParameter("Expiration", DbType.DateTime, expiration);

            db.ExecuteNonQuery(commmand);
        }

        /// <summary>
        /// Delete ticket with specified guid
        /// </summary>
        /// <param name="ticketId"></param>
        public static void DeleteTicket(Guid ticketId)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper commmand = db.GetStoredProcCommandWrapper("ckbx_sp_Ticket_Delete");

            commmand.AddInParameter("TicketGUID", DbType.Guid, ticketId);

            db.ExecuteNonQuery(commmand);
        }

        /// <summary>
        /// Validate that the ticket with the given ID has not expired
        /// </summary>
        /// <param name="ticketID">ID of the ticket to validate.</param>
        /// <returns>True if the ticket is valid, false otherwise (ticket expired or does not exist)</returns>
        public static bool ValidateTicket(Guid ticketID)
        {
            bool valid = false;

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Ticket_Get");
            command.AddInParameter("TicketGUID", DbType.Guid, ticketID);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        if (reader["Expiration"] != DBNull.Value)
                        {
                            if (DateTime.Now < (DateTime)reader["Expiration"])
                            {
                                valid = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "BusinessProtected");
                }
                finally
                {
                    reader.Close();
                }
            }

            return valid;
        }
    }
}
