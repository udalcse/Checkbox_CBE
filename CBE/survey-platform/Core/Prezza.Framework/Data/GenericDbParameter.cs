using System.Data;
using System.Data.Common;

namespace Prezza.Framework.Data
{
    /// <summary>
    /// Generic db agnostic db parameter for internal use in checkbox
    /// </summary>
    public class GenericDbParameter : DbParameter
    {
        ///<summary>
        ///</summary>
        public GenericDbParameter()
        {
        }

        ///<summary>
        ///</summary>
        ///<param name="parameterName"></param>
        ///<param name="dbType"></param>
        ///<param name="value"></param>
        public GenericDbParameter(string parameterName, DbType dbType, object value)
        {
            ParameterName = parameterName;
            DbType = dbType;
            Value = value;
        }

        public override DbType DbType { get; set; }
        public override ParameterDirection Direction { get; set; }
        public override bool IsNullable { get; set; }
        public override string ParameterName { get; set; }
        public override void ResetDbType() { }
        public override int Size { get; set; }
        public override string SourceColumn { get; set; }
        public override bool SourceColumnNullMapping { get; set; }
        public override DataRowVersion SourceVersion { get; set; }
        public override object Value { get; set; }
    }
}
