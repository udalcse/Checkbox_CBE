
namespace Prezza.Framework.Data.Sprocs
{
    /// <summary>
    /// Stored procedure for fetching data
    /// </summary>
    public class FetchProcedure  : StoredProcedure
    {
        /// <summary>
        /// Constructor that accepts the name of the procedure
        /// </summary>
        /// <param name="name"></param>
        public FetchProcedure(string name)
            : base(name)
        {
        }
    }
}
