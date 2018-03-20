using System;
using System.Linq;
using System.Xml;
using System.Data;
using System.Collections.Generic;
using Checkbox.Common;
using Checkbox.Security.Principal;
using Prezza.Framework.Data;
using Prezza.Framework.Security;
using Prezza.Framework.Security.Principal;

using Checkbox.Globalization.Text;
using Checkbox.Pagination;

namespace Checkbox.Styles
{
    /// <summary>
    /// Handles creating/deleting/storing style templates
    /// </summary>
    public static class StyleTemplateManager
    {
        /// <summary>
        /// Get a new style template
        /// </summary>
        /// <param name="currentPrincipal">User attempting to create template</param>
        /// <returns></returns>
        public static StyleTemplate CreateStyleTemplate(ExtendedPrincipal currentPrincipal)
        {
            if (!AuthorizationFactory.GetAuthorizationProvider().Authorize(currentPrincipal, "Form.Edit"))
            {
                throw new AuthorizationException();
            }

            return new StyleTemplate {CreatedBy = currentPrincipal.Identity.Name};
        }

        /// <summary>
        /// Load a style template from xml
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <param name="currentPrincipal"></param>
        /// <returns></returns>
        public static StyleTemplate CreateStyleTemplate(XmlDocument xmlDoc, ExtendedPrincipal currentPrincipal)
        {
            if (!AuthorizationFactory.GetAuthorizationProvider().Authorize(currentPrincipal, "Form.Edit"))
            {
                throw new AuthorizationException();
            }

            //Figure out the name first, since the decorator's LoadTemplateFromXml method saves the template so
            // calling IsStyleNameInUse would always return true.
            XmlNode node = xmlDoc.SelectSingleNode("/CssDocument/TemplateName");

            if (node != null)
            {
                string templateName = node.InnerText;

				string proposedName = GetUniqueName(templateName);

                //Set the name
                node.InnerText = proposedName;
            }

            //An acceptable name has been found, now import and save the template
            var template = new StyleTemplate();
            var decorator = new StyleTemplateTextDecorator(template, TextManager.DefaultLanguage);

            //save to get an ID
            SaveTemplate(template, currentPrincipal);

            //LoadTemplateFromXml also causes the template to be saved
            decorator.LoadTemplateFromXml(xmlDoc, currentPrincipal);
            template.CreatedBy = currentPrincipal.Identity.Name;
            template.DateCreated = DateTime.Now;


            return template;
        }

		private static string GetUniqueName(string templateName)
		{
			//Now make sure there isn't name collision

			if (string.IsNullOrEmpty(templateName))
				templateName = "style";

			Int32 copyVersion = 1;
			string proposedName = templateName;
			while (IsStyleNameInUse(proposedName))
			{
				proposedName = templateName + " " + TextManager.GetText("/common/duplicate", TextManager.DefaultLanguage) + " " + copyVersion;
				copyVersion++;
			}
			return proposedName;
		}

        /// <summary>
        /// Get a style template with the specified ID
        /// </summary>
        /// <param name="templateID"></param>
        /// <returns></returns>
        public static StyleTemplate GetStyleTemplate(Int32 templateID)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Style_GetTemplate");
            command.AddInParameter("TemplateID", DbType.Int32, templateID);

            DataSet ds = db.ExecuteDataSet(command);

            //Quickly check data
            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return null;
            }

            var template = new StyleTemplate();
            template.Load(ds);

            return template;
        }

        /// <summary>
        /// Get a lightweight style template with the specified ID
        /// </summary>
        /// <param name="templateID"></param>
        /// <returns></returns>
        public static LightweightStyleTemplate GetLightweightStyleTemplate(Int32 templateID, CheckboxPrincipal userPrincipal)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Style_GetProperties");
            command.AddInParameter("TemplateID", DbType.Int32, templateID);

            var style = new LightweightStyleTemplate();

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        style.TemplateId = DbUtility.GetValueFromDataReader(reader, "TemplateID", -1);
                        style.Name = DbUtility.GetValueFromDataReader(reader, "Name", string.Empty);
                        style.IsEditable = DbUtility.GetValueFromDataReader(reader, "IsEditable", true);
                        style.IsPublic = DbUtility.GetValueFromDataReader(reader, "IsPublic", false);
                        style.Type = (StyleTemplateType)Enum.Parse(typeof(StyleTemplateType), DbUtility.GetValueFromDataReader(reader, "Type", "PC"));
                        style.CreatedBy = DbUtility.GetValueFromDataReader(reader, "CreatedBy", string.Empty);
                        style.DateCreated = DbUtility.GetValueFromDataReader(reader, "DateCreated",
                                                                                DateTime.MinValue);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }            

            StyleTemplateManager.UpdateStyleEditability(userPrincipal, style);

            return style;
        }

        /// <summary>
        /// Save the specified style template
        /// </summary>
        /// <param name="template"></param>
        /// <param name="currentPrincipal"></param>
        public static void SaveTemplate(StyleTemplate template, ExtendedPrincipal currentPrincipal)
        {
            if (!AuthorizationFactory.GetAuthorizationProvider().Authorize(currentPrincipal, "Form.Edit"))
            {
                throw new AuthorizationException();
            }

            template.Save();
        }

        /// <summary>
        /// Delete the specified style template.
        /// </summary>
        /// <param name="styleTemplateID">ID of the template to delete.</param>
        /// <param name="currentPrincipal"></param>
        public static void DeleteTemplate(int styleTemplateID, ExtendedPrincipal currentPrincipal)
        {
            if (!AuthorizationFactory.GetAuthorizationProvider().Authorize(currentPrincipal, "Form.Edit"))
            {
                throw new AuthorizationException();
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Style_DeleteTemplate");
            command.AddInParameter("TemplateID", DbType.Int32, styleTemplateID);

            db.ExecuteNonQuery(command);
        }

        /// <summary>
        /// Copy a style template
        /// </summary>
        /// <param name="template"></param>
        /// <param name="languageCode"></param>
        /// <param name="currentPrincipal"></param>
        /// <remarks>This method saves the copied template to the DB</remarks>
        public static StyleTemplate CopyTemplate(StyleTemplate template, string languageCode, ExtendedPrincipal currentPrincipal)
        {
            if (template == null)
            {
                return null;
            }
            if (!AuthorizationFactory.GetAuthorizationProvider().Authorize(currentPrincipal, "Form.Edit"))
            {
                throw new AuthorizationException();
            }

            string proposedName = GetUniqueName( template.Name);

            var newTemplate = new StyleTemplate();

            var currentTemplateDecorator = new StyleTemplateTextDecorator(template, languageCode);
            
            

            //Set the name and other properties
            newTemplate.Name = proposedName;
            newTemplate.CreatedBy = currentPrincipal.Identity.Name;
            newTemplate.DateCreated = DateTime.Now;
            newTemplate.IsPublic = template.IsPublic;
            newTemplate.Save();

            var newTemplateDecorator = new StyleTemplateTextDecorator(newTemplate, languageCode);
            
            //Populate the new template with data from the template to copy, repopulate properties to prevent overwriting by load method
            newTemplateDecorator.LoadTemplateFromXml(currentTemplateDecorator.GetTemplateXml(), currentPrincipal);
            newTemplate.Name = proposedName;
            newTemplate.CreatedBy = currentPrincipal.Identity.Name;
            newTemplate.DateCreated = DateTime.Now;
            newTemplate.IsPublic = template.IsPublic;
            newTemplateDecorator.Save(currentPrincipal, null);

            return newTemplate;
        }

        /// <summary>
        /// A list of available style templates
        /// </summary>
        /// <param name="currentPrincipal"></param>
        public static List<LightweightStyleTemplate> ListStyleTemplates(ExtendedPrincipal currentPrincipal, StyleTemplateType? type = null)
        {
            var styles = new List<LightweightStyleTemplate>();

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command;

            if (currentPrincipal == null)
            {
                //Pass a dummy value to get only public styles
                command = db.GetStoredProcCommandWrapper("ckbx_sp_Style_ListForUser");
                command.AddInParameter("UniqueIdentifier", DbType.String, "[[NULL]]");
            }
            else if (currentPrincipal.IsInRole("System Administrator"))
            {
                command = db.GetStoredProcCommandWrapper("ckbx_sp_Style_ListAll");
            }
            else
            {
                command = db.GetStoredProcCommandWrapper("ckbx_sp_Style_ListForUser");
                command.AddInParameter("UniqueIdentifier", DbType.String, currentPrincipal.Identity.Name);
            }

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while(reader.Read())
                    {
                        int templateId = DbUtility.GetValueFromDataReader(reader, "TemplateId", -1);
                        string templateName = DbUtility.GetValueFromDataReader(reader, "Name", string.Empty);

                        if (templateId > 0 && Utilities.IsNotNullOrEmpty(templateName))
                        {
                            StyleTemplateType t = (StyleTemplateType)Enum.Parse(typeof(StyleTemplateType), DbUtility.GetValueFromDataReader(reader, "Type", "PC"));
                            if (type != null && t != type.Value)
                                continue;
                            styles.Add(new LightweightStyleTemplate
                            {
                                Name = templateName,
                                TemplateId = templateId,
                                IsPublic = DbUtility.GetValueFromDataReader(reader, "IsPublic", false),
                                IsEditable = DbUtility.GetValueFromDataReader(reader, "IsEditable", false),
                                DateCreated = DbUtility.GetValueFromDataReader(reader, "DateCreated", DateTime.MinValue),
                                CreatedBy = DbUtility.GetValueFromDataReader(reader, "CreatedBy", string.Empty),
                                Type = t
                            });
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return styles;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentPrincipal"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ListStyleTemplatesForDataBinding(ExtendedPrincipal currentPrincipal)
        {
            //Sort by stripped name
            return ListStyleTemplates(currentPrincipal)
                .OrderBy(st => Utilities.StripHtml(st.Name, 64))
                .ToDictionary(template => template.TemplateId.ToString(), template => Utilities.StripHtml(template.Name, 64));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentPrincipal"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ListStyleTemplatesForDataBinding(ExtendedPrincipal currentPrincipal, StyleTemplateType type)
        {
            //Sort by stripped name
            return ListStyleTemplates(currentPrincipal)
                .Where(st => st.Type.Equals(type))
                .OrderBy(st => Utilities.StripHtml(st.Name, 64))
                .ToDictionary(template => template.TemplateId.ToString(), template => Utilities.StripHtml(template.Name, 64));
        }

        /// <summary>
        /// Get style data 
        /// </summary>
        public static LightweightStyleTemplate[] GetPagedStyleData(ExtendedPrincipal currentPrincipal, PaginationContext paginationContext)
        {
            //1. Get list from database
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command;

            if (currentPrincipal == null)
            {
                //Pass a dummy value to get only public styles
                command = db.GetStoredProcCommandWrapper("ckbx_sp_Style_ListForUser");
                command.AddInParameter("UniqueIdentifier", DbType.String, "[[NULL]]");
            }
            else if (currentPrincipal.IsInRole("System Administrator"))
            {
                command = db.GetStoredProcCommandWrapper("ckbx_sp_Style_ListAll");
            }
            else
            {
                command = db.GetStoredProcCommandWrapper("ckbx_sp_Style_ListForUser");
                command.AddInParameter("UniqueIdentifier", DbType.String, currentPrincipal.Identity.Name);
            }

            var templateList = new List<LightweightStyleTemplate>();

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        int templateId = DbUtility.GetValueFromDataReader(reader, "TemplateId", -1);
                        string templateName = DbUtility.GetValueFromDataReader(reader, "Name", string.Empty);

                        if (templateId > 0 && Utilities.IsNotNullOrEmpty(templateName))
                        {
                            templateList.Add(new LightweightStyleTemplate
                            {
                                Name = templateName,
                                TemplateId = templateId,
                                IsPublic = DbUtility.GetValueFromDataReader(reader, "IsPublic", false),
                                Type = (StyleTemplateType)Enum.Parse(typeof(StyleTemplateType), DbUtility.GetValueFromDataReader(reader, "Type", "PC")),
                                IsEditable = DbUtility.GetValueFromDataReader(reader, "IsEditable", false),
                                DateCreated = DbUtility.GetValueFromDataReader(reader, "DateCreated", DateTime.MinValue),
                                CreatedBy = DbUtility.GetValueFromDataReader(reader, "CreatedBy", string.Empty)
                            });
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            //Filter -- Support by type, name and created by
            var filteredList = Utilities.IsNotNullOrEmpty(paginationContext.FilterField)
                ? "CreatedBy".Equals(paginationContext.FilterField, StringComparison.InvariantCultureIgnoreCase)
                        ? templateList.Where(style => style.CreatedBy.Contains(paginationContext.FilterValue))
                        : 
                            ("Type".Equals(paginationContext.FilterField, StringComparison.InvariantCultureIgnoreCase) 
                            ? templateList.Where(style => style.Type.ToString().Equals(paginationContext.FilterValue))
                            : templateList.Where(style => style.Name.IndexOf(paginationContext.FilterValue, StringComparison.InvariantCultureIgnoreCase) >=0))
                : templateList;

            //Sort
            var sortedList = Utilities.IsNotNullOrEmpty(paginationContext.SortField)
                ? "CreatedBy".Equals(paginationContext.SortField, StringComparison.InvariantCultureIgnoreCase)
                    ? paginationContext.SortAscending
                            ? filteredList.OrderBy(style => style.CreatedBy)
                            : filteredList.OrderByDescending(style => style.CreatedBy)
                    : paginationContext.SortAscending
                            ? filteredList.OrderBy(style => style.Name)
                            : filteredList.OrderByDescending(style => style.Name)
                : filteredList;

            var pagedList = paginationContext.CurrentPage > 0 && paginationContext.PageSize > 0
                ? sortedList.Skip((paginationContext.CurrentPage - 1) * paginationContext.PageSize).Take(paginationContext.PageSize)
                : sortedList;

            paginationContext.ItemCount = filteredList.Count();

            return UpdateStyleEditability(pagedList.ToArray(), currentPrincipal);
        }

        private static LightweightStyleTemplate[] UpdateStyleEditability(LightweightStyleTemplate[] styles, ExtendedPrincipal currentPrincipal)
        {
            foreach (LightweightStyleTemplate style in styles)
            {
                UpdateStyleEditability(currentPrincipal, style);
            }
            return styles;
        }

        public static void UpdateStyleEditability(ExtendedPrincipal currentPrincipal, LightweightStyleTemplate style)
        {
            style.CanBeEdited = style.IsEditable;
            if (!style.IsEditable && !style.CreatedBy.Equals(currentPrincipal.Identity.Name))
            {
                string[] roles = currentPrincipal.GetRoles();
                style.CanBeEdited = roles.Any(r => r.Equals("System Administrator"));
            }
            else
                style.CanBeEdited = true;
        }

        /// <summary>
        /// Detect if the style in use by a response template or an analysis template
        /// </summary>
        /// <param name="styleTemplateID"></param>
        /// <returns></returns>
        public static bool IsStyleInUse(Int32 styleTemplateID)
        {

            //Surveys
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Style_GetRTUsing");
            command.AddInParameter("TemplateID", DbType.Int32, styleTemplateID);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        return true;
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            //Reports
            DBCommandWrapper command2 = db.GetStoredProcCommandWrapper("ckbx_sp_Style_GetATUsing");
            command2.AddInParameter("TemplateID", DbType.Int32, styleTemplateID);

            using (IDataReader reader = db.ExecuteReader(command2))
            {
                try
                {
                    if (reader.Read())
                    {
                        return true;
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            //Email items
            DBCommandWrapper command3 = db.GetStoredProcCommandWrapper("ckbx_sp_Style_GetEmailUsing");
            command3.AddInParameter("TemplateID", DbType.Int32, styleTemplateID);

            using (IDataReader reader = db.ExecuteReader(command3))
            {
                try
                {
                    if (reader.Read())
                    {
                        return true;
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return false;
        }


        /// <summary>
        /// Get a boolean indicating if the specified style name is already being used
        /// </summary>
        /// <param name="styleName"></param>
        /// <returns></returns>
        public static bool IsStyleNameInUse(string styleName)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Style_GetIDFromName");
            command.AddInParameter("Name", DbType.String, styleName);
            command.AddOutParameter("TemplateID", DbType.Int32, 4);

            db.ExecuteNonQuery(command);

            object templateID = command.GetParameterValue("TemplateID");

            if (templateID != null && templateID != DBNull.Value && (Int32)templateID > 0)
            {
                return true;
            }
            
            return false;
        }
    }
}
