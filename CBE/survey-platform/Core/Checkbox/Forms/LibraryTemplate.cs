using System;
using System.Data;
using Checkbox.Common;
using Prezza.Framework.Common;
using Prezza.Framework.Data;
using Prezza.Framework.Security;
using Checkbox.Forms.Security;
using Checkbox.Globalization.Text;
using System.Xml;
using System.IO;
using System.Text;
using Checkbox.Analytics.Export;
using Checkbox.Security.Principal;


namespace Checkbox.Forms
{
    /// <summary>
    /// Container for survey items outside of a survey.
    /// </summary>
    [Serializable]
    public class LibraryTemplate : Template
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public LibraryTemplate()
            : base(new[] { "Library.Edit", "Library.View" },
                   new[] { "Library.Create", "Library.Delete", "Library.Edit", "Library.View" })
        {

        }


        /// <summary>
        /// Get object type name
        /// </summary>
        public override string ObjectTypeName { get { return "LibraryTemplate"; } }

        /// <summary>
        /// Get load sproc name
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_Library_Get"; } }

        /// <summary>
        /// Get template data set
        /// </summary>
        /// <returns></returns>
        protected override PersistedDomainObjectDataSet CreateConfigurationDataSet()
        {
            return new LibraryTemplateDataSet(ObjectTypeName);
        }

        /// <summary>
        /// Initialize access for a new library
        /// </summary>
        /// <param name="defaultPolicy"></param>
        /// <param name="acl"></param>
        internal void InitializeAccess(Policy defaultPolicy, AccessControlList acl)
        {
            if (ID != null && ID > 0)
            {
                throw new Exception("Access can only be initialized for a new library template.");
            }

            ArgumentValidation.CheckExpectedType(defaultPolicy, typeof(LibraryPolicy));

            SetAccess(defaultPolicy, acl);

        }

        ///<summary>
        ///</summary>
        ///<param name="itemId"></param>
        public void AddItem(int itemId)
        {
            if (PageIds.Count > 0)
            {
                AddItemToPage(PageIds[0], itemId, null);
            }
        }

        ///<summary>
        ///</summary>
        ///<param name="itemId"></param>
        public void RemoveItem(int itemId)
        {
            if(PageIds.Count > 0)
            {
                RemoveItemFromPage(PageIds[0], itemId);
            }
        }

        /// <summary>
        /// Copy item
        /// </summary>
        /// <param name="itemId"></param>
        public void CopyItem(int itemId, CheckboxPrincipal principal)
        {
            if (PageIds.Count > 0)
            {
                CopyItemFromPage(PageIds[0], itemId, principal);
            }
        }

        /// <summary>
        /// Create a library
        /// </summary>
        /// <param name="t"></param>
        protected override void Create(IDbTransaction t)
        {
            base.Create(t);

            if (ID <= 0)
            {
                throw new Exception("Unable to save library template data.  DataID <= 0.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Library_Insert");
            command.AddInParameter("LibraryTemplateID", DbType.Int32, ID);
            command.AddInParameter("NameTextID", DbType.String, NameTextID);
            command.AddInParameter("DescriptionTextID", DbType.String, DescriptionTextID);

            db.ExecuteNonQuery(command, t);

        }

        /// <summary>
        /// Get the text ID for the library name
        /// </summary>
        public string NameTextID
        {
            get
            {
                if (ID > 0)
                {
                    return "/library/" + ID + "/name";
                }

                return string.Empty;
            }
        }

		public override string Name
		{
			get
			{
				if (string.IsNullOrEmpty(base.Name) && ID.HasValue)
					base.Name = TextManager.GetText(NameTextID);

				return base.Name;
			}
			set
			{
				string name = base.Name;

				if (string.IsNullOrEmpty(name) || name != value)
				{
					base.Name = value;

					if (ID.HasValue)
						TextManager.SetText(NameTextID, value ?? string.Empty);
				}
			}
		}

        /// <summary>
        /// Get the text id for the library description
        /// </summary>
        public string DescriptionTextID
        {
            get
            {
                if (ID > 0)
                {
                    return "/library/" + ID + "/description";
                }

                return string.Empty;
            }
        }


        /// <summary>
        /// Create a policy for this library with the specified permissions
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public override Policy CreatePolicy(string[] permissions)
        {
            return new LibraryPolicy(permissions);
        }

        /// <summary>
        /// Get a security editor for this library
        /// </summary>
        /// <returns></returns>
        public override SecurityEditor GetEditor()
        {
            return new LibrarySecurityEditor(this);
        }

        /// <summary>
        /// Load base template data
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadBaseObjectData(DataRow data)
        {
            LoadTemplateData(data, false);
        }

        /// <summary>
        /// Load template data from the specified data row.
        /// </summary>
        /// <param name="libraryTemplateDR"></param>
        /// <param name="isImport"></param>
        private void LoadTemplateData(DataRow libraryTemplateDR, bool isImport)
        {
            //Set the library template properties
            //If importing, do not import acl & default policy id & data id & some other data
            if (!isImport)
            {
                AclID = DbUtility.GetValueFromDataRow<int?>(libraryTemplateDR, "AclID", null);
                DefaultPolicyID = DbUtility.GetValueFromDataRow<int?>(libraryTemplateDR, "DefaultPolicy", null);

                ID = DbUtility.GetValueFromDataRow<int?>(libraryTemplateDR, "LibraryTemplateID", null);

                CreatedBy = DbUtility.GetValueFromDataRow(libraryTemplateDR, "CreatedBy", string.Empty);
                CreatedDate = DbUtility.GetValueFromDataRow<DateTime?>(libraryTemplateDR, "CreatedDate", null);
                LastModified = DbUtility.GetValueFromDataRow<DateTime?>(libraryTemplateDR, "ModifiedDate", null);
            }
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static LibraryTemplate Copy(LibraryTemplate source)
		{
			LibraryTemplate newLibrary = LibraryTemplateManager.CreateLibraryTemplate(null, Users.UserManager.GetCurrentPrincipal());

			var sb = new StringBuilder();
			var sw = new StringWriter(sb);

			var xml = new XmlTextWriter(sw);

			source.Export(xml);

		    var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sb.ToString());

            newLibrary.Import(xmlDocument.DocumentElement, null, Users.UserManager.GetCurrentPrincipal());

			newLibrary.Save();

			return newLibrary;
		}

		/// <summary>
		/// Gets Unique Name
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static string GetUniqueName(string name)
		{
			if (string.IsNullOrEmpty(name))
				name = "Library";
            int idx = 1;
            string newName = String.Format("{0} Copy {1}", name, idx);

			while (LibraryTemplateManager.LibraryTemplateExists(newName, null))
                newName = String.Format("{0} Copy {1}", name, ++idx);

			return newName;
		}


		#region Xml IO

        /// <summary>
        /// 
        /// </summary>
        public override string  ExportElementName
        {
            get { return "LibraryTemplate"; }
        }


		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteTemplateData(XmlWriter writer)
		{
			writer.WriteHtml("Name", TextManager.GetText(NameTextID, TextManager.DefaultLanguage));
			string descr = TextManager.GetText(DescriptionTextID);
			writer.WriteHtml("Description", descr);
		}

		/// <summary>
		/// 
		/// </summary>
        /// <param name="xmlNode"></param>
		protected override void LoadTemplateData(XmlNode xmlNode)
		{
            //For backwards compatibility
		    var libraryDataNode = xmlNode.SelectSingleNode("LibraryData") ?? xmlNode;

            Name = GetUniqueName(XmlUtility.GetNodeText(libraryDataNode.SelectSingleNode("Name")));
            TextManager.SetText(DescriptionTextID, XmlUtility.GetNodeText(libraryDataNode.SelectSingleNode("Description")));
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        protected override void LoadTemplateCustomTextData(XmlNode xmlNode)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteCustomTextData(XmlWriter writer)
        {
        }


		#endregion

	}
}
