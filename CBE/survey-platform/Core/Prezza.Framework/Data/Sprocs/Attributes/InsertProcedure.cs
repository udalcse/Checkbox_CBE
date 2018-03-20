
namespace Prezza.Framework.Data.Sprocs
{
    /// <summary>
    /// Stored procedure for fetching data
    /// </summary>
    public class InsertProcedure : StoredProcedure
    {
        /// <summary>
        /// Constructor that accepts the name of the procedure
        /// </summary>
        /// <param name="name"></param>
        public InsertProcedure(string name)
            : base(name)
        {
        }
    }
}
