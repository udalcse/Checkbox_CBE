using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using Checkbox.Common;
using Prezza.Framework.Configuration;
using Prezza.Framework.Data;

namespace Checkbox.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public static class AutocompleteListManager
    {
        /// <summary>
        /// Returns a dictionary with listIds and list names
        /// </summary>
        /// <returns></returns>
        private static Dictionary<int, string> ListPredefinedLists()
        {
            var result = new Dictionary<int, string>();
            string appRoot = ConfigurationManager.DetermineAppConfigPath();

            var doc = XDocument.Load(string.Format("{0}\\AutocompleteListConfiguration.xml", appRoot));
            if (doc.Root != null)
            {
                var listNodes = doc.Root.Elements();
                foreach (var node in listNodes)
                {
                    var idAttr = node.Attribute("id");
                    if (idAttr != null)
                    {
                        var id = Utilities.AsInt(idAttr.Value);
                        if (id.HasValue)
                        {
                            var nameAttr = node.Attribute("name");
                            result[id.Value] = nameAttr != null && !string.IsNullOrEmpty(nameAttr.Value)
                                ? nameAttr.Value
                                : "list-"+id;
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Returns a dictionary with listIds and list names
        /// </summary>
        /// <returns></returns>
        private static Dictionary<int, string> ListCustomListNames()
        {
            var result = new Dictionary<int, string>();

            var db = DatabaseFactory.CreateDatabase();
            var command = db.GetStoredProcCommandWrapper("ckbx_sp_Autocomplete_GetLists");

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        var id = DbUtility.GetValueFromDataReader(reader, "ListId", -1);
                        if (id >= 1000)
                        {
                            var name = DbUtility.GetValueFromDataReader(reader, "Name", string.Empty);
                            result.Add(id, string.IsNullOrWhiteSpace(name) ? "list-"+id : name);
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a dictionary with listIds and list names
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, string> ListAllListNames()
        {
            var result = ListCustomListNames();
            var predefined = ListPredefinedLists();
            foreach (var pair in predefined)
            {
                result[pair.Key] = pair.Value;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string[] ListItems(int listId)
        {
            if (listId >= 1000)
            {
                var db = DatabaseFactory.CreateDatabase();
                var command = db.GetStoredProcCommandWrapper("ckbx_sp_Autocomplete_GetListData");
                command.AddInParameter("ListId", DbType.Int32, listId);

                using (IDataReader reader = db.ExecuteReader(command))
                {
                    try
                    {
                        if (reader.Read())
                        {
                            var data = DbUtility.GetValueFromDataReader(reader, "Data", string.Empty);
                            return data.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
            }
            else
            {
                string appRoot = ConfigurationManager.DetermineAppConfigPath();
                var doc = XDocument.Load(string.Format("{0}\\AutocompleteListConfiguration.xml", appRoot));
                if (doc.Root != null)
                {
                    var node = doc.Root.Elements().FirstOrDefault(e => e.Attribute("id") != null && Utilities.AsInt(e.Attribute("id").Value) == listId);
                    if (node != null)
                        return node.Elements("item").Select(e => e.Value).ToArray();
                }
            }

            return new string[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static int? CreateList(string name, string[] items)
        {
            var db = DatabaseFactory.CreateDatabase();
            var command = db.GetStoredProcCommandWrapper("ckbx_sp_Autocomplete_AddList");
            command.AddInParameter("Name", DbType.String, name);
            command.AddInParameter("Data", DbType.String, string.Join(Environment.NewLine, items));
            command.AddOutParameter("ListId", DbType.Int32, 4);

            db.ExecuteNonQuery(command);
            return (int)command.GetParameterValue("ListId");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="name"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static void EditList(int listId, string name, string[] items)
        {
            var db = DatabaseFactory.CreateDatabase();
            var command = db.GetStoredProcCommandWrapper("ckbx_sp_Autocomplete_UpdateList");
            command.AddInParameter("ListId", DbType.Int32, listId);
            command.AddInParameter("Name", DbType.String, name);
            command.AddInParameter("Data", DbType.String, string.Join(Environment.NewLine, items));
            db.ExecuteNonQuery(command);
        }
    }
}
