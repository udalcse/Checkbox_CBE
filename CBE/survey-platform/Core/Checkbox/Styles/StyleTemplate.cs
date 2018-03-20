using System;
using System.Xml;
using System.Data;
using System.Text;
using System.Collections.Generic;

using Checkbox.Common;
using Checkbox.Globalization.Text;

using Prezza.Framework.Data;
using Prezza.Framework.Common;
using Prezza.Framework.ExceptionHandling;

namespace Checkbox.Styles
{
    /// <summary>
    /// Style template
    /// </summary>
    [Serializable]
    public class StyleTemplate
    {
        private DataSet _data;
        private int? _templateID;
        private bool? _isPublic;
        private bool? _isEditable;

        private const string CssTableName = "Css";
        private const string CssPropTableName = "CssProp";
        private const string TemplateTableName = "TemplateData";

        private static readonly List<string> ExcludedSelectors = new List<string>
                                                            {
                                                                ".Matrix .AlternatingItem td input",
                                                                ".Matrix .Item td input"
                                                            };

        #region Construct/Init

        /// <summary>
        /// Default constructor
        /// </summary>
        internal StyleTemplate()
        {
            InitializeData();
        }

        /// <summary>
        /// Initialize data for the template
        /// </summary>
        private void InitializeData()
        {
            DataTable cssTable = new DataTable { TableName = CssTableName };
            cssTable.Columns.Add("ElementName", typeof(string));
            cssTable.Columns.Add("ElementID", typeof(Int32));

            cssTable.Columns["ElementID"].AutoIncrement = true;
            cssTable.Columns["ElementID"].AutoIncrementSeed = -1;
            cssTable.Columns["ElementID"].AutoIncrementStep = -1;

            DataTable cssPropTable = new DataTable { TableName = CssPropTableName };
            cssPropTable.Columns.Add("ElementID", typeof(Int32));
            cssPropTable.Columns.Add("PropertyName", typeof(string));
            cssPropTable.Columns.Add("PropertyValue", typeof(string));

            _data = new DataSet();
            _data.Tables.Add(cssTable);
            _data.Tables.Add(cssPropTable);

            DataRelation relation = new DataRelation("ElemID", cssTable.Columns["ElementID"], cssPropTable.Columns["ElementID"]);
            _data.Relations.Add(relation);

            relation.ChildKeyConstraint.AcceptRejectRule = AcceptRejectRule.None;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the style template ID
        /// </summary>
        public int? TemplateID
        {
            get
            {
                if(_templateID.HasValue)
                    return _templateID.Value;
                return null;
            }
        }

        /// <summary>
        /// Get/set the style template name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Get/set the style template type
        /// </summary>
        public StyleTemplateType Type { get; set; }

        /// <summary>
        /// Get/set whether the style template can be used by any survey editor or just
        /// the creator.
        /// </summary>
        public bool IsPublic
        {
            get { return _isPublic.HasValue ? _isPublic.Value : false; }
            set { _isPublic = value; }
        }

        /// <summary>
        /// Get/set whether the style template can be edited by any survey editor or
        /// just the creator.
        /// </summary>
        public bool IsEditable
        {
            get { return _isEditable.HasValue ? _isEditable.Value : false; }
            set { _isEditable = value; }
        }

        /// <summary>
        /// Get/set the creator of the template
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Get/set the created date of the item
        /// </summary>
        public DateTime? DateCreated { get; set; }

        /// <summary>
        /// Get/set the HTML header
        /// </summary>
        [Obsolete("HeaderHTML is localizable, so the TextManager should be used to get/set the text.")]
        public string HeaderHTML { get; set; }

        /// <summary>
        /// Get/set the HTML footer
        /// </summary>
        [Obsolete("FooterHTML is localizable, so the TextManager should be used to get/set the text.")]
        public string FooterHTML { get; set; }

        /// <summary>
        /// Get the text id for the header
        /// </summary>
        public string HeaderTextID
        {
            get
            {
                if (TemplateID.HasValue)
                {
                    return "/styleTemplate/" + TemplateID + "/header";
                }

                return null;
            }
        }

        /// <summary>
        /// Get the text id for the footer
        /// </summary>
        public string FooterTextID
        {
            get
            {
                if (TemplateID.HasValue)
                {
                    return "/styleTemplate/" + TemplateID + "/footer";
                }

                return null;
            }
        }

        /// <summary>
        /// Set ID, for use by factory
        /// </summary>
        /// <param name="templateID"></param>
        internal void SetTemplateID(Int32 templateID)
        {
            _templateID = templateID;
        }

        #endregion

        #region Load/Save

        /// <summary>
        /// Load the style template from XML
        /// </summary>
        /// <param name="xml"></param>
        internal void Load(string xml)
        {
            if (xml == null || xml.Trim() == string.Empty)
            {
                return;
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            XmlNamespaceManager ns = new XmlNamespaceManager(xmlDoc.NameTable);
            ns.AddNamespace("prz", "http://www.prezzatech.com/schemas/StyleTemplateSchema.xsd");

            //Step 1:  Test if we are loading old-format stuff
            if (xmlDoc.SelectSingleNode("/prz:StyleTemplate", ns) != null)
            {
                LoadPreviousVersionExport(xmlDoc, ns);
            }
            else
            {
                Name = XmlUtility.GetNodeText(xmlDoc.SelectSingleNode("/CssDocument/TemplateName"));
                
                string strType = XmlUtility.GetNodeText(xmlDoc.SelectSingleNode("/CssDocument/Type"));

                if (!string.IsNullOrEmpty(strType))
                {
                    Type = (StyleTemplateType)Enum.Parse(typeof(StyleTemplateType), strType);
                }

                HeaderHTML = XmlUtility.GetNodeText(xmlDoc.SelectSingleNode("/CssDocument/Header"));
                FooterHTML = XmlUtility.GetNodeText(xmlDoc.SelectSingleNode("/CssDocument/Footer"));

                LoadCssClasses(xmlDoc);
            }
        }

        /// <summary>
        /// Load the css classes from the XML export
        /// </summary>
        /// <param name="xmlDoc"></param>
        internal void LoadCssClasses(XmlDocument xmlDoc)
        {
            XmlNodeList classNodes = xmlDoc.SelectNodes("/CssDocument/CssClass");

            foreach (XmlNode classNode in classNodes)
            {
                string elementName = XmlUtility.GetNodeText(classNode.SelectSingleNode("Name"));

                XmlNodeList styleNodes = classNode.SelectNodes("Style");

                foreach (XmlNode styleNode in styleNodes)
                {
                    string propertyName = XmlUtility.GetNodeText(styleNode.SelectSingleNode("Property"));
                    string propertyValue = XmlUtility.GetNodeText(styleNode.SelectSingleNode("Value"));

                    if (elementName != string.Empty && propertyName != string.Empty)
                    {
                        SetElementStyleProperty(elementName, propertyName, propertyValue);
                    }
                }
            }
        }

        /// <summary>
        /// Load a stylesheet exported from a pre 4.0 release
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <param name="ns"></param>
        private void LoadPreviousVersionExport(XmlDocument xmlDoc, XmlNamespaceManager ns)
        {
            Name = XmlUtility.GetNodeText(xmlDoc.SelectSingleNode("/prz:StyleTemplate/prz:TemplateName", ns));

            string headerText = XmlUtility.GetNodeText(xmlDoc.SelectSingleNode("/prz:StyleTemplate/prz:CustomHeader", ns));
            headerText = headerText.Replace("&gt;", ">");
            headerText = headerText.Replace("&lt;", "<");
            HeaderHTML = headerText;

            string footerText = XmlUtility.GetNodeText(xmlDoc.SelectSingleNode("/prz:StyleTemplate/prz:CustomFooter", ns));
            footerText = footerText.Replace("&gt;", ">");
            footerText = footerText.Replace("&lt;", "<");
            FooterHTML = footerText;

            XmlNode cssNode = xmlDoc.SelectSingleNode("/prz:StyleTemplate/prz:Css", ns);

            if (cssNode != null)
            {
                XmlDocument cssDoc = new XmlDocument();

                string innerXML = cssNode.InnerXml;
                innerXML = innerXML.Replace("&gt;", ">");
                innerXML = innerXML.Replace("&lt;", "<");

                cssDoc.LoadXml(innerXML);
                LoadCssClasses(cssDoc);
            }
        }

        /// <summary>
        /// Load the stylesheet from the datarow
        /// </summary>
        /// <param name="ds"></param>
        internal void Load(DataSet ds)
        {
            if (ds == null)
            {
                throw new Exception("DataSet was null.");
            }

            if (ds.Tables.Count < 3)
            {
                throw new Exception("DataSet did not contain the expected number of tables.");
            }

            ds.Tables[0].TableName = TemplateTableName;
            ds.Tables[1].TableName = CssTableName;
            ds.Tables[2].TableName = CssPropTableName;


            if (ds.Tables[TemplateTableName].Rows.Count <= 0)
            {
                throw new Exception("TemplateData table has no rows.");
            }

            DataRow templateDataRow = ds.Tables[TemplateTableName].Rows[0];

            _templateID = (int?)templateDataRow["TemplateID"];
            HeaderHTML = DbUtility.GetValueFromDataRow<string>(templateDataRow, "Header", null);
            FooterHTML = DbUtility.GetValueFromDataRow<string>(templateDataRow, "Footer", null);
            Name = DbUtility.GetValueFromDataRow<string>(templateDataRow, "Name", null);
            Type = (StyleTemplateType)Enum.Parse(typeof(StyleTemplateType), DbUtility.GetValueFromDataRow<string>(templateDataRow, "Type", "PC"));
            CreatedBy = DbUtility.GetValueFromDataRow<string>(templateDataRow, "CreatedBy", null);
            _isPublic = DbUtility.GetValueFromDataRow(templateDataRow, "IsPublic", false);
            _isEditable = DbUtility.GetValueFromDataRow(templateDataRow, "IsEditable", false);
            DateCreated = DbUtility.GetValueFromDataRow<DateTime?>(templateDataRow, "DateCreated", null);

            _data.Tables[CssPropTableName].Rows.Clear();
            _data.Tables[CssTableName].Rows.Clear();

            foreach (DataRow row in ds.Tables[CssTableName].Rows)
            {
                _data.Tables[CssTableName].ImportRow(row);
            }

            foreach (DataRow row in ds.Tables[CssPropTableName].Rows)
            {
                _data.Tables[CssPropTableName].ImportRow(row);
            }
        }

        /// <summary>
        /// Save the style template
        /// </summary>
        internal void Save()
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase();

                using (IDbConnection connection = db.GetConnection())
                {
                    connection.Open();
                    IDbTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        if (_templateID != null && _templateID.Value > 0)
                        {
                            Update(transaction);
                        }
                        else
                        {
                            Create(transaction);
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw new Exception("Unable to save style data.");
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "UIProcess");

                if (rethrow)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Create the db entry
        /// </summary>
        /// <param name="t"></param>
        private void Create(IDbTransaction t)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Style_InsertTemplate");

            command.AddInParameter("Name", DbType.String, Name);
            command.AddInParameter("CreatedBy", DbType.String, CreatedBy);
            command.AddInParameter("Header", DbType.String, HeaderHTML);
            command.AddInParameter("Footer", DbType.String, FooterHTML);
            command.AddInParameter("IsPublic", DbType.Boolean, IsPublic);
            command.AddInParameter("IsEditable", DbType.Boolean, IsEditable);
            command.AddInParameter("Type", DbType.String, Type.ToString());
            command.AddOutParameter("TemplateID", DbType.Int32, 4);

            db.ExecuteNonQuery(command);

            object id = command.GetParameterValue("TemplateID");

            if (id == null || id == DBNull.Value)
            {
                throw new Exception("Unable to save template data.");
            }

            _templateID = Convert.ToInt32(id);

            //Update elements
            db.UpdateDataSet(
                _data,
                CssTableName,
                GetElementInsertCommand(db),
                GetElementUpdateCommand(db),
                GetElementDeleteCommand(db),
                t);

            //Update element properties
            db.UpdateDataSet(
                _data,
                CssPropTableName,
                GetElementPropertyInsertCommand(db),
                GetElementPropertyUpdateCommand(db),
                GetElementPropertyDeleteCommand(db),
                t);
        }

        /// <summary>
        /// Update the db entry
        /// </summary>
        /// <param name="t"></param>
        private void Update(IDbTransaction t)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Style_UpdateTemplate");

            command.AddInParameter("TemplateID", DbType.Int32, _templateID);
            command.AddInParameter("Name", DbType.String, Name);
            command.AddInParameter("CreatedBy", DbType.String, CreatedBy);
            command.AddInParameter("Header", DbType.String, HeaderHTML);
            command.AddInParameter("Footer", DbType.String, FooterHTML);
            command.AddInParameter("IsPublic", DbType.Boolean, IsPublic);
            command.AddInParameter("IsEditable", DbType.Boolean, IsEditable);
            command.AddInParameter("Type", DbType.String, Type.ToString());

            db.ExecuteNonQuery(command);

            object id = command.GetParameterValue("TemplateID");

            if (id == null || id == DBNull.Value)
            {
                throw new Exception("Unable to save template data.");
            }

            _templateID = Convert.ToInt32(id);

            //Update elements
            db.UpdateDataSet(
                _data,
                CssTableName,
                GetElementInsertCommand(db),
                GetElementUpdateCommand(db),
                GetElementDeleteCommand(db),
                t);

            //Update element properties
            db.UpdateDataSet(
                _data,
                CssPropTableName,
                GetElementPropertyInsertCommand(db),
                GetElementPropertyUpdateCommand(db),
                GetElementPropertyDeleteCommand(db),
                t);
        }

        /// <summary>
        /// Get the command to insert a css element
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        private DBCommandWrapper GetElementInsertCommand(Database db)
        {
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Style_InsertElement");
            command.AddInParameter("TemplateID", DbType.Int32, _templateID);
            command.AddInParameter("ElementName", DbType.String, "ElementName", DataRowVersion.Current);
            command.AddParameter("ElementID", DbType.Int32, ParameterDirection.Output, "ElementID", DataRowVersion.Current, null);
            return command;
        }

        /// <summary>
        /// Get the command to update a css element
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        private DBCommandWrapper GetElementUpdateCommand(Database db)
        {
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Style_UpdateElement");
            command.AddInParameter("TemplateID", DbType.Int32, _templateID);
            command.AddInParameter("ElementID", DbType.Int32, "ElementID", DataRowVersion.Current);
            command.AddInParameter("ElementName", DbType.String, "ElementName", DataRowVersion.Current);
            return command;
        }

        /// <summary>
        /// Get the command to delete a css element
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        private static DBCommandWrapper GetElementDeleteCommand(Database db)
        {
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Style_DeleteElement");
            command.AddInParameter("ElementID", DbType.Int32, "ElementID", DataRowVersion.Current);
            return command;
        }

        /// <summary>
        /// Get the command to insert a css element property
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        private static DBCommandWrapper GetElementPropertyInsertCommand(Database db)
        {
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Style_InsertProperty");
            command.AddInParameter("ElementID", DbType.Int32, "ElementID", DataRowVersion.Current);
            command.AddInParameter("PropertyName", DbType.String, "PropertyName", DataRowVersion.Current);
            command.AddInParameter("PropertyValue", DbType.String, "PropertyValue", DataRowVersion.Current);
            return command;
        }

        /// <summary>
        /// Get the command to update a css element property
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        private static DBCommandWrapper GetElementPropertyUpdateCommand(Database db)
        {
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Style_UpdateProperty");
            command.AddInParameter("ElementID", DbType.Int32, "ElementID", DataRowVersion.Current);
            command.AddInParameter("PropertyName", DbType.String, "PropertyName", DataRowVersion.Current);
            command.AddInParameter("PropertyValue", DbType.String, "PropertyValue", DataRowVersion.Current);
            return command;
        }

        /// <summary>
        /// Get the command to delete a css element property
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        private static DBCommandWrapper GetElementPropertyDeleteCommand(Database db)
        {
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Style_DeleteProperty");
            command.AddInParameter("ElementID", DbType.Int32, "ElementID", DataRowVersion.Current);
            command.AddInParameter("PropertyName", DbType.String, "PropertyName", DataRowVersion.Current);
            return command;

        }

        #endregion

        #region Misc.

        /// <summary>
        /// Get the elements of the style template
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> Elements
        {
            get
            {
                Dictionary<string, Dictionary<string, string>> elementStyles = new Dictionary<string, Dictionary<string, string>>();

                DataRow[] elementRows = _data.Tables[CssTableName].Select(null, null, DataViewRowState.CurrentRows);

                foreach (DataRow elementRow in elementRows)
                {
                    int? elementID = DbUtility.GetValueFromDataRow<int?>(elementRow, "ElementID", null);
                    string elementName = DbUtility.GetValueFromDataRow(elementRow, "ElementName", string.Empty);

                    if (elementID.HasValue && Utilities.IsNotNullOrEmpty(elementName))
                    {
                        Dictionary<string, string> properties = new Dictionary<string, string>();

                        DataRow[] propertyRows = _data.Tables[CssPropTableName].Select("ElementID = " + elementID, null, DataViewRowState.CurrentRows);

                        foreach (DataRow propertyRow in propertyRows)
                        {
                            string propertyName = DbUtility.GetValueFromDataRow(propertyRow, "PropertyName", string.Empty);
                            string propertyValue = DbUtility.GetValueFromDataRow(propertyRow, "PropertyValue", string.Empty);

                            if (Utilities.IsNotNullOrEmpty(propertyName)
                                && Utilities.IsNotNullOrEmpty(propertyValue))
                            {
                                properties[propertyName] = propertyValue;
                            }
                        }

                        elementStyles[elementName] = properties;
                    }
                }

                return elementStyles;
            }
        }

        /// <summary>
        /// Get the styles for an element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetElementStyle(string element)
        {
            if (element == null || element.Trim() == string.Empty)
            {
                return null;
            }
            Dictionary<string, string> styles = new Dictionary<string, string>();

            DataRow[] elementRows = _data.Tables[CssTableName].Select(string.Format("ElementName = '{0}'", Utilities.SqlEncode(element)), null, DataViewRowState.CurrentRows);

            //If the element exists, see if there are rows to remove
            if (elementRows.Length > 0)
            {
                int? elementID = DbUtility.GetValueFromDataRow<int?>(elementRows[0], "ElementID", null);

                if (elementID.HasValue)
                {
                    DataRow[] propertyRows = _data.Tables[CssPropTableName].Select(string.Format("ElementID = {0}", elementID), null, DataViewRowState.CurrentRows);

                    foreach (DataRow propertyRow in propertyRows)
                    {
                        string propertyName = DbUtility.GetValueFromDataRow(propertyRow, "PropertyName", string.Empty);
                        string propertyValue = DbUtility.GetValueFromDataRow(propertyRow, "PropertyValue", string.Empty);

                        if (Utilities.IsNotNullOrEmpty(propertyName))
                        {
                            styles[propertyName] = propertyValue;
                        }
                    }
                }
            }

            return styles;
        }

        /// <summary>
        /// Get the proprety value for an element style
        /// </summary>
        /// <param name="element"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public string GetElementProperty(string element, string property)
        {
            if (element == null || element.Trim() == string.Empty || property == null || property.Trim() == string.Empty)
            {
                return string.Empty;
            }

            Dictionary<string, string> styles = GetElementStyle(element);

            return styles.ContainsKey(property) ? styles[property] : string.Empty;
        }

        /// <summary>
        /// Set the element style
        /// </summary>
        /// <param name="element"></param>
        /// <param name="styles"></param>
        public void SetElementStyle(string element, Dictionary<string, string> styles)
        {
            //Validate data
            if (element == null || element.Trim() == string.Empty || styles == null)
            {
                return;
            }

            //See if there is an existing element
            DataRow[] elementRows = _data.Tables[CssTableName].Select("ElementName = '" + Utilities.SqlEncode(element) + "'", null, DataViewRowState.CurrentRows);

            //If the element exists, see if there are rows to remove
            if (elementRows.Length > 0)
            {
                int? elementID = DbUtility.GetValueFromDataRow<int?>(elementRows[0], "ElementID", null);

                if (elementID.HasValue)
                {
                    DataRow[] propertyRows = _data.Tables[CssPropTableName].Select(string.Format("ElementID = {0}", elementID), null, DataViewRowState.CurrentRows);

                    foreach (DataRow propertyRow in propertyRows)
                    {
                        string propertyName = DbUtility.GetValueFromDataRow(propertyRow, "PropertyName", string.Empty);

                        if (Utilities.IsNotNullOrEmpty(propertyName))
                        {
                            if (!styles.ContainsKey(propertyName))
                            {
                                RemoveElementStyleProperty(elementID.Value, propertyName);
                            }
                        }
                    }
                }
            }

            //Update/add values
            foreach (string key in styles.Keys)
            {
                SetElementStyleProperty(element, key, styles[key]);
            }
        }

        /// <summary>
        /// Set the element style property
        /// </summary>
        /// <param name="element"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public void SetElementStyleProperty(string element, string property, string value)
        {
            //Validate input
            if (element == null || element.Trim() == string.Empty || property == null || property.Trim() == string.Empty)
            {
                return;
            }

            DataRow elementRow;

            //See if there is an existing element
            DataRow[] elementRows = _data.Tables[CssTableName].Select("ElementName = '" + Utilities.SqlEncode(element) + "'", null, DataViewRowState.CurrentRows);

            //Get the element row, if it exists
            if (elementRows.Length > 0)
            {
                elementRow = elementRows[0];
            }
            else
            {
                elementRow = _data.Tables[CssTableName].NewRow();
                elementRow["ElementName"] = element;
                _data.Tables[CssTableName].Rows.Add(elementRow);
            }

            int? elementID = DbUtility.GetValueFromDataRow<int?>(elementRow, "ElementID", null);

            if (elementID.HasValue)
            {
                //Create values
                //See if an existing property needs to be created/updated
                DataRow[] propertyRows = _data.Tables[CssPropTableName].Select(string.Format("ElementID = {0} AND PropertyName = '{1}'", elementID, property), null, DataViewRowState.CurrentRows);

                if (propertyRows.Length == 0)
                {
                    DataRow newRow = _data.Tables[CssPropTableName].NewRow();
                    newRow["ElementID"] = elementID;
                    newRow["PropertyName"] = property;

                    newRow["PropertyValue"] = value ?? string.Empty;

                    newRow.SetParentRow(elementRow);
                    _data.Tables[CssPropTableName].Rows.Add(newRow);
                }
                else
                {
                    string propertyValue = DbUtility.GetValueFromDataRow(propertyRows[0], "PropertyValue", string.Empty);

                    if (property == "font-family")
                        value = RemoveQuotationFromFontFamily(value);

                    if (propertyValue != value)
                    {
                        propertyRows[0]["PropertyValue"] = value ?? string.Empty;
                    }
                }
            }
        }

        private string RemoveQuotationFromFontFamily(string propertyValue)
        {
            var result = propertyValue;

            if(propertyValue.Length < 2)
                return result;

            if (propertyValue[0] == '\'' && propertyValue[propertyValue.Length - 1] == '\'')
            {
                result = result.Remove(0, 1);
                result = result.Remove(result.Length - 1, 1);
            }

            return result;
        }

        /// <summary>
        /// Remove an element
        /// </summary>
        /// <param name="element"></param>
        public void RemoveElement(string element)
        {
            //Validate input
            if (element == null || element.Trim() == string.Empty)
            {
                return;
            }

            DataRow[] rowsToDelete = _data.Tables[CssTableName].Select("ElementName = '" + Utilities.SqlEncode(element) + "'", null, DataViewRowState.CurrentRows);

            foreach (DataRow rowToDelete in rowsToDelete)
            {
                rowToDelete.Delete();
            }
        }

        /// <summary>
        /// Remove a style property of an element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="property"></param>
        public void RemoveElementStyleProperty(string element, string property)
        {
            //Validate input
            if (element == null || element.Trim() == string.Empty || property == null || property.Trim() == string.Empty)
            {
                return;
            }

            DataRow[] elementRows = _data.Tables[CssTableName].Select(string.Format("ElementName = '{0}'", Utilities.SqlEncode(element)), null, DataViewRowState.CurrentRows);

            if (elementRows.Length > 0)
            {
                int? elementID = DbUtility.GetValueFromDataRow<int?>(elementRows[0], "ElementID", null);

                if (elementID.HasValue)
                {
                    RemoveElementStyleProperty(elementID.Value, property);
                }
            }
        }

        /// <summary>
        /// Remove a style property from an element
        /// </summary>
        /// <param name="elementID"></param>
        /// <param name="property"></param>
        protected void RemoveElementStyleProperty(Int32 elementID, string property)
        {
            //Validate input
            if (property == null || property.Trim() == string.Empty)
            {
                return;
            }

            DataRow[] rowsToDelete = _data.Tables[CssPropTableName].Select(string.Format("ElementID = {0} AND PropertyName = '{1}'", elementID, property), null, DataViewRowState.CurrentRows);

            foreach (DataRow rowToDelete in rowsToDelete)
            {
                rowToDelete.Delete();
            }
        }

        /// <summary>
        /// Get the CSS for this style template adapted for mobile
        /// </summary>
        public string GetCssForMobile()
        {
            return GetCss(true);
        }

        /// <summary>
        /// Get the CSS for this style template
        /// </summary>
        public string GetCss()
        {
            return GetCss(false);
        }

        protected string GetCss(bool isMobile)
        {
            StringBuilder sb = new StringBuilder();

            DataRow[] elementRows = _data.Tables[CssTableName].Select(null, null, DataViewRowState.CurrentRows);

            foreach (DataRow elementRow in elementRows)
            {
                string elementName = DbUtility.GetValueFromDataRow(elementRow, "ElementName", string.Empty);
                int? elementID = DbUtility.GetValueFromDataRow<int?>(elementRow, "ElementID", null);

                if (elementID.HasValue && Utilities.IsNotNullOrEmpty(elementName) 
                    && !ExcludedSelectors.Contains(elementName))
                {
                    string css = GetCss(elementID.Value);

                    // To override the styles from GlobalSurveyStyles.css                    
                    if (elementName == ".Page")
                    {
                        css = css.Replace(";", " !important;");
                    }

                    if (isMobile) 
                        css = HandleForMobile(elementName, css);
                    
                    if (elementName != null && elementName.Trim() != string.Empty)
                    {
                        sb.Append(elementName);
                        sb.Append("{");
                        sb.Append(css);
                        sb.Append("}");
                        sb.AppendLine();

                        if (string.Compare(elementName, "Item", true) == 0 || string.Compare(elementName, "AlternatingItem", true) == 0)
                        {
                            sb.Append(elementName + " td");
                            sb.Append("{");
                            sb.Append(css);
                            sb.Append("}");
                            sb.AppendLine();
                        }
                    }
                }
            }

            return sb.ToString();
        }

        protected string HandleForMobile(string elementName, string css)
        {
            if (string.IsNullOrEmpty(elementName) || string.IsNullOrEmpty(css))
                return css;

            if (elementName.ToLower() == ".surveycontentframe")
            {
                int startIndex = css.IndexOf("min-width");
                int endIndex = css.IndexOf(';', startIndex);

                css = css.Remove(startIndex, endIndex - startIndex + 1);
            }
            
            return css;
        }

        /// <summary>
        /// Get the Css for a specific element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public string GetCss(string element)
        {
            //Validate input
            if (element == null || element.Trim() == string.Empty)
            {
                return string.Empty;
            }

            DataRow[] elementRows = _data.Tables[CssTableName].Select("ElementName = '" + Utilities.SqlEncode(element) + "'", null, DataViewRowState.CurrentRows);

            if (elementRows.Length > 0)
            {
                int? elementID = DbUtility.GetValueFromDataRow<int?>(elementRows[0], "ElementID", null);

                if (elementID.HasValue)
                {
                    return GetCss(elementID.Value);
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Get the css for a specific element
        /// </summary>
        /// <param name="elementID"></param>
        /// <returns></returns>
        protected string GetCss(int elementID)
        {
            StringBuilder sb = new StringBuilder();

            DataRow[] propertyRows = _data.Tables[CssPropTableName].Select(string.Format("ElementID = {0}", elementID), null, DataViewRowState.CurrentRows);

            foreach (DataRow propertyRow in propertyRows)
            {
                string propName = DbUtility.GetValueFromDataRow(propertyRow, "PropertyName", string.Empty);
                string propValue = DbUtility.GetValueFromDataRow(propertyRow, "PropertyValue", string.Empty);

                // Here we do some sweet conditional logic to make sure that null or empty
                // images and properties are not added to the css, because they are invalid
                if (Utilities.IsNotNullOrEmpty(propName)
                    && Utilities.IsNotNullOrEmpty(propValue) && !propValue.Contains("url(\"\")"))
                {
                    sb.Append(propName);
                    sb.Append(":");
                    sb.Append(propValue);
                    sb.Append(";");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Get the template in XML format
        /// </summary>
        public XmlDocument ToXml()
        {
            Dictionary<string, Dictionary<string, string>> styles = Elements;

            XmlDocument doc = new XmlDocument();

            XmlNode docNode = doc.CreateElement("CssDocument");

            XmlNode templateNameNode = doc.CreateElement("TemplateName");
            templateNameNode.InnerText = Name;

            docNode.AppendChild(templateNameNode);

            XmlNode templateTypeNode = doc.CreateElement("Type");
            templateTypeNode.InnerText = Type.ToString();

            docNode.AppendChild(templateTypeNode);


            XmlElement headerElement = doc.CreateElement("Header");

            XmlElement footerElement = doc.CreateElement("Footer");

            docNode.AppendChild(headerElement);
            ExportText(doc, headerElement, HeaderTextID, "headerText");

            docNode.AppendChild(footerElement);
            ExportText(doc, footerElement, FooterTextID, "footerText");

            foreach (string styleKey in styles.Keys)
            {
                Dictionary<string, string> properties = styles[styleKey];

                XmlNode classNode = doc.CreateElement("CssClass");
                XmlNode nameNode = doc.CreateElement("Name");

                nameNode.InnerText = styleKey;

                classNode.AppendChild(nameNode);

                foreach (string propKey in properties.Keys)
                {
                    XmlNode styleNode = doc.CreateElement("Style");
                    XmlNode propertyNameNode = doc.CreateElement("Property");
                    XmlNode propertyValueNode = doc.CreateElement("Value");

                    propertyNameNode.InnerText = propKey;
                    propertyValueNode.InnerText = properties[propKey];

                    styleNode.AppendChild(propertyNameNode);
                    styleNode.AppendChild(propertyValueNode);

                    classNode.AppendChild(styleNode);
                }

                docNode.AppendChild(classNode);
            }

            doc.AppendChild(docNode);


            return doc;
        }

        private void ExportText(XmlDocument doc, XmlElement element, string ID, string subNodeName)
        {
            DataTable table = TextManager.GetTextData(ID);

            foreach (DataRow row in table.Rows)
            {
                XmlElement textDataElement = doc.CreateElement(subNodeName);
                element.AppendChild(textDataElement);
                XmlAttribute languageCodeAttribute =  doc.CreateAttribute("languageCode");
                languageCodeAttribute.Value = row[1].ToString();
                textDataElement.Attributes.Append(languageCodeAttribute);
                //textDataElement.InnerText = row[2].ToString();
				XmlCDataSection cd = doc.CreateCDataSection(row[2].ToString());
				textDataElement.AppendChild(cd);
            }
        }
        #endregion
    }
}
