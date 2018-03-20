
namespace Prezza.Framework.Data.Sprocs
{
    /// <summary>
    /// Stored procedure for fetching data
    /// </summary>
    public class UpdateProcedure : StoredProcedure
    {
        /// <summary>
        /// Constructor that accepts the name of the procedure
        /// </summary>
        /// <param name="name"></param>
        public UpdateProcedure(string name)
            : base(name)
        {
        }
    }
}
