using System;
using System.Collections.Generic;
using System.Data;

using Prezza.Framework.Data;

namespace Checkbox.Forms.Logic
{
    class ExpressionBuilder
    {
        /*****************************************************
         * The process of building an expression is as follows
         * 1. Select the left Operand source.  This will be
         * either an Item, Profile, or ResponseMetaData
         * 2. Lookup the supported Operands for this source. 
         * If there is only one, you don't need to select it.
         * 3. Construct the Operand with its context (Item, Profile, etc...)
         * 4. Retrieve the supported LogicalOperators from the 
         * newly created Operand.  This will be dynamically filtered 
         * based on the context.
         * 5. Set the LogicalOperator
         * 6. Retrieve the supported right Operand types from the 
         * left Operand and construct it.
         * 7. Save
         * **************************************************/

        /// <summary>
        /// Given a Type, retrieves the supported operands from the registry
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        public static List<Type> GetSupportedOperandTypes(Type subject)
        {
            List<Type> operandTypes = new List<Type>();

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Rules_SupportedOperands");
            command.AddInParameter("TypeName", DbType.String, subject.FullName);
            command.AddInParameter("TypeAssembly", DbType.String, subject.Assembly.GetName().Name);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        string typename = (string)reader[0];
                        string typeassembly = (string)reader[1];

                        operandTypes.Add(Type.GetType(typename + "," + typeassembly));
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    reader.Close();
                }
            }
            
            return operandTypes;
        }

        /// <summary>
        /// Given a left operand and an operation, retrieves the supported right-hand operands from the registry
        /// </summary>
        /// <param name="left"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        public static List<Type> GetComparableOperandTypes(Operand left, LogicalOperator operation)
        {
            List<Type> operandTypes = new List<Type>();

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Rules_ComparableOperands");
            command.AddInParameter("LeftOperandTypeName", DbType.String, left.GetType().FullName);
            command.AddInParameter("LeftOperandTypeAssembly", DbType.String, left.GetType().Assembly.GetName().Name);
            command.AddInParameter("LogicalOperation", DbType.String, operation.ToString());

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        string typename = (string)reader[0];
                        string typeassembly = (string)reader[1];

                        operandTypes.Add(Type.GetType(typename + "," + typeassembly));
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    reader.Close();
                }
            }

            return operandTypes;
        }
    }
}
