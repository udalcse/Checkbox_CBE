using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Checkbox.Common;
using Checkbox.Forms.Data;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Logic.Configuration;
using Checkbox.Forms.Validation;
using Checkbox.Management;
using Checkbox.Pagination;
using Checkbox.Forms;
using Checkbox.Security;
using Checkbox.Security.Principal;
using Checkbox.Styles;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web;
using Prezza.Framework.Security;
using Prezza.Framework.Security.Principal;
using Checkbox.Users;
using Checkbox.Timeline;
using Checkbox.Forms.Piping;
using Checkbox.Forms.Logic;
using System.Web;
using System.Web.ApplicationServices;

namespace Checkbox.Wcf.Services
{
    /// <summary>
    /// Non service-specific methods for survey management service.
    /// </summary>
    public static class SurveyManagementServiceImplementation
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="parentItemId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="filterField"></param>
        /// <param name="filter"></param>
        /// <param name="period">0 - don't filter by period. 2 -- daily, 3 - weekly, 4 -- monthly created/modified entities</param>
        /// <param name="dateFieldName">Specifies what field of datetime type should be used when filtering by period. Null or empty -- don't filter by period.</param>
        /// <param name="includeSurveyResponseCount"></param>
        /// <param name="includeActive"></param>
        /// <param name="includeInactive"></param>
        /// <returns></returns>
        public static SurveyListItem[] ListSurveysAndFolders(CheckboxPrincipal userPrincipal, int parentItemId, int pageNumber, int pageSize, string filterField, string filter, int period, string dateFieldName, bool includeSurveyResponseCount, bool includeActive, bool includeInactive)
        {
            //Authorize user
            Security.AuthorizeUserContext(userPrincipal, "FormFolder.Read", "Analysis.Administer");

            //that's a full list of permissions
            var permissions = new List<string> { "FormFolder.Read", "Form.Edit", "Analysis.Create" };

            var authProvider = AuthorizationFactory.GetAuthorizationProvider();
            //if the role doesn't allow to edit surveys -- remove this permission from checking at ACL
            if (!authProvider.Authorize(userPrincipal, "Form.Edit"))
            {
                permissions.Remove("Form.Edit");
            }
            //if the role doesn't allow to create reports -- remove this permission from checking at ACL
            if (!authProvider.Authorize(userPrincipal, "Analysis.Create") && !authProvider.Authorize(userPrincipal, "Analysis.Administer"))
            {
                permissions.Remove("Analysis.Create");
            }

            if (filter == null)
                filter = string.Empty;

            var paginationContext = new PaginationContext
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                Permissions = permissions,
                PermissionJoin = PermissionJoin.Any,
                SortField = "ItemName",
                SortAscending = true,
                FilterField = filterField,
                FilterValue = Utilities.AdvancedHtmlDecode(filter)
            };

            if (!string.IsNullOrEmpty(dateFieldName))
            {
                DateTime now = DateTime.Now;
                paginationContext.DateFieldName = TimelineManager.ProtectFieldNameFromSQLInjections(dateFieldName);
                paginationContext.EndDate = period > 0 ? (DateTime?)now : null;
                paginationContext.StartDate = TimelineManager.GetStartFilterDate(period);
            }

            //Handle case of Root vs. Child folder.  Currently, Checkbox supports a 2-level hierarchy with folders and surveys
            // at the root, and only surveys at the second level.
            List<LightweightAccessControllable> templateList =
                parentItemId == 0 || !string.IsNullOrEmpty(dateFieldName) ? 
                ResponseTemplateManager.ListAccessibleTemplates(userPrincipal, null, paginationContext, includeActive, includeInactive)
                : (parentItemId == 1
                    ? ResponseTemplateManager.ListAccessibleTemplatesAndFolders(userPrincipal, paginationContext, includeActive, includeInactive)
                    : ResponseTemplateManager.ListAccessibleTemplates(userPrincipal, parentItemId, paginationContext, includeActive, includeInactive));

            return ToSurveyListItemList(userPrincipal, templateList, includeSurveyResponseCount, includeActive, includeInactive).ToArray();
        }

        /// <summary>
        /// Get list of favorite surveys
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="parentItemId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="filter"></param>
        /// <param name="includeSurveyResponseCount"></param>
        /// <returns></returns>
        public static SurveyListItem[] ListFavoriteSurveys(CheckboxPrincipal userPrincipal, int parentItemId, int pageNumber, int pageSize, string filter, bool includeSurveyResponseCount)
        {
            //Authorize user
            Security.AuthorizeUserContext(userPrincipal, "FormFolder.Read", "Analysis.Administer");

            var permissions = new List<string> { "FormFolder.Read" };

            if (userPrincipal.IsInRole("Report Administrator") && !userPrincipal.IsInRole("Survey Editor")
                && !(userPrincipal.IsInRole("System Administrator") || userPrincipal.IsInRole("Survey Administrator")))
            {
                permissions.Add("Analysis.Create");
            }
            else
            {
                permissions.Add("Form.Edit");
            }

            var paginationContext = new PaginationContext
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                Permissions = permissions,
                PermissionJoin = PermissionJoin.Any,
                SortField = "ItemName",
                SortAscending = true,
                FilterField = "Name",
                FilterValue = filter
            };

            List<LightweightAccessControllable> templateList =
                ResponseTemplateManager.ListFavoriteSurveys(userPrincipal, paginationContext);

            return ToSurveyListItemList(userPrincipal, templateList, includeSurveyResponseCount, true, true).ToArray();
        }

        /// <summary>
        /// Add survey to list of favorite ones
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        public static Boolean AddFavoriteSurvey(CheckboxPrincipal userPrincipal, int surveyId)
        {
            //Authorize user
            Security.AuthorizeUserContext(userPrincipal, "FormFolder.Read", "Analysis.Administer");
            List<String> permissions = new List<string>();

            return ResponseTemplateManager.AddFavoriteSurvey(userPrincipal, surveyId, permissions);
        }

        /// <summary>
        /// Remove favorite survey
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        public static Boolean RemoveFavoriteSurvey(CheckboxPrincipal userPrincipal, int surveyId)
        {
            return ResponseTemplateManager.RemoveFavoriteSurvey(userPrincipal, surveyId);
        }

        /// <summary>
        /// Check if specified survey is favorite
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        public static Boolean IsFavoriteSurvey(CheckboxPrincipal userPrincipal, int surveyId)
        {
            return ResponseTemplateManager.IsFavoriteSurvey(userPrincipal, surveyId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="parentItemId"></param>
        /// <param name="includeSurveyResponseCount"></param>
        public static PagedListResult<SurveyListItem[]> ListAvailableSurveys(CheckboxPrincipal userPrincipal, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            //Authorize user
            //Security.AuthorizeUserContext(userPrincipal, "Form.Fill");

            filterValue = filterValue == null ? null : Utilities.AdvancedHtmlDecode(filterValue);
            var paginationContext = CreatePaginationContext(filterField, filterValue, sortField, sortAscending,
                pageNumber, pageSize);
            paginationContext.Permissions = new List<string>();
            paginationContext.PermissionJoin = PermissionJoin.Any;
            paginationContext.CurrentPage = -1;

         
            var res = new PagedListResult<SurveyListItem[]>();
            var allAvailableSurveys = ResponseTemplateManager.ListTakeableResponseTemplates(userPrincipal,
                                                                                            paginationContext);

            var filteredByLimits = userPrincipal.IsInRole("System Administrator") ? allAvailableSurveys :                
                (from s in allAvailableSurveys
                                    where
                                        (ResponseTemplateManager.MoreResponsesAllowed(s.ID, s.MaxTotalResponses,
                                                                                     s.MaxResponsesPerUser,
                                                                                     userPrincipal, s.AnonymizeResponses) || s.AllowEdit) //allow edit overrides limits
                                        && s.IsActive && s.SecurityType != SecurityType.InvitationOnly && (s.ActivationEndDate == null || s.ActivationEndDate >= DateTime.Now)
                                        && (s.ActivationStartDate == null || s.ActivationStartDate <= DateTime.Now)
                                    select s
                                   );

            if (filterField == "Name")
                filteredByLimits = filteredByLimits.Where(s => s.Name.ToLower().Contains(filterValue.ToLower()));

            res.TotalItemCount = filteredByLimits.Count();

            //filter out by pagination
            filteredByLimits = filteredByLimits.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            
            res.ResultPage = ToSurveyListItemList(userPrincipal, filteredByLimits, false, true, true).ToArray();
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="listItemId"></param>
        /// <param name="type"></param>
        /// <param name="includeSurveyResponseCount"></param>
        public static SurveyListItem GetSurveyListItem(CheckboxPrincipal userPrincipal, int listItemId, string type, bool includeSurveyResponseCount)
        {
            if (Utilities.IsNullOrEmpty(type))
            {
                throw new Exception("Type value must be either of \"survey\" or \"folder\"");
            }

            //Authorize user
            Security.AuthorizeUserContext(userPrincipal, "FormFolder.Read");

            //Get template
            if ("survey".Equals(type, StringComparison.InvariantCultureIgnoreCase))
            {
                var lightweightTemplate = ResponseTemplateManager.GetLightweightResponseTemplate(listItemId);

                return lightweightTemplate == null
                           ? null
                           : ToSurveyListItem(userPrincipal, lightweightTemplate, includeSurveyResponseCount, true, true);
            }

            //Get folder
            if ("folder".Equals(type, StringComparison.InvariantCultureIgnoreCase))
            {
                //Root folder with ID = 1 is an implicit folder, and therefore can't be retrieved.
                var folder = listItemId > 1
                                 ? FolderManager.GetLightweightFolder(listItemId)
                                 : null;

                if (folder == null)
                {
                    return null;
                }

                //Build list item
                var folderListItem = ToSurveyListItem(userPrincipal, folder, includeSurveyResponseCount, true, true);

                //Add children
                folderListItem.ChildItems = ListSurveysAndFolders(userPrincipal, listItemId, -1, -1, "Name", string.Empty, -1, string.Empty, includeSurveyResponseCount, true, true);

                return folderListItem;
            }

            throw new Exception("Type value must be either of \"survey\" or \"folder\"");
        }

        /// <summary>
        /// Get metadata associated with a survey
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        public static SurveyMetaData GetSurveyMetaData(CheckboxPrincipal userPrincipal, int surveyId)
        {
            return GetSurveyInfo(userPrincipal, () => ResponseTemplateManager.GetResponseTemplate(surveyId));
        }

        /// <summary>
        /// Get metadata associated with a survey by GUID
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="rtGuid"></param>
        /// <returns></returns>
        public static SurveyMetaData GetSurveyInfoByGuid(CheckboxPrincipal userPrincipal, Guid rtGuid)
        {
            return GetSurveyInfo(userPrincipal, () => ResponseTemplateManager.GetResponseTemplate(rtGuid));
        }

        public static SurveyMetaData GetSurveyInfoByName(CheckboxPrincipal userPrincipal, string surveyName)
        {
            return GetSurveyInfo(userPrincipal, () => ResponseTemplateManager.GetResponseTemplate(surveyName));
        }

        public static int CopySurvey(CheckboxPrincipal userPrincipal, int surveyId, string surveyName)
        {
            var targetRt = ResponseTemplateManager.GetResponseTemplate(surveyId);

            Security.AuthorizeUserContext(userPrincipal, targetRt, "Form.Edit", "Form.Create");

            if (targetRt == null)
            {
                throw new Exception("Unable to load template to move/copy [" + surveyId + "]");
            }

            var newTemplate = ResponseTemplateManager.CopyTemplate(
                    targetRt.ID.Value, surveyName,
                    userPrincipal,
                    targetRt.LanguageSettings.DefaultLanguage);

            var destFolder = FolderManager.GetRoot();

            destFolder.Add(newTemplate);

            return (int)newTemplate.ID;
        }

        private static SurveyMetaData GetSurveyInfo(ExtendedPrincipal userPrincipal, Func<ResponseTemplate> getSurvey)
        {
            var responseTemplate = getSurvey();
            if (responseTemplate == null)
                return null;

            Security.AuthorizeUserContext(userPrincipal, responseTemplate, "Form.Edit", "Analysis.Create");

            var result = CreateSurveyMetaData(userPrincipal, responseTemplate);

            result.CanAssignStyleTemplates = ApplicationManager.AppSettings.AllowEditSurveyStyleTemplate ||
                                             userPrincipal.IsInRole("System Administrator");
            result.CanChangeLanguages = ApplicationManager.AppSettings.AllowMultiLanguage;

            result.CanRewriteSurveyUrl = ApplicationManager.AppSettings.AllowSurveyUrlRewriting;

            return result;
        }

        /// <summary>
        /// Add a user to a survey's access list with the specified permissions.
        /// </summary>
        /// <param name="userPrincipal">User context. This user must have administrative permissions on the survey</param>
        /// <param name="uniqueIdentifier">Unique identifier of the user.</param>
        /// <param name="responseTemplateID">ID of the survey to add a user to.</param>
        /// <param name="permissions">Permissions to apply.</param>
        /// <returns></returns>
        public static void AddUserToSurveyAccessList(CheckboxPrincipal userPrincipal, string uniqueIdentifier, int responseTemplateID, string[] permissions)
        {
            Security.AuthorizeUserContext(userPrincipal, "Form.Administer");
            LightweightResponseTemplate lightweightRt = ResponseTemplateManager.GetLightweightResponseTemplate(responseTemplateID);

            if (lightweightRt != null)
            {
                Security.AuthorizeUserContext(userPrincipal, lightweightRt, "Form.Administer");
                SecurityEditor securityEditor = lightweightRt.GetEditor();
                ExtendedPrincipal principal = UserManager.GetUserPrincipal(uniqueIdentifier);
                securityEditor.Initialize(userPrincipal);
                securityEditor.ReplaceAccess(principal, permissions);
                securityEditor.SaveAcl();
            }
            else
                throw new Exception("Survey with ID = " + responseTemplateID.ToString() + " has not been found");
        }

        /// <summary>
        /// Get metadata associated with a survey
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="surveyId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public static SurveyPageMetaData GetSurveyPageMetaData(CheckboxPrincipal userPrincipal, int surveyId, int pageId)
        {
            var responseTemplate = ResponseTemplateManager.GetResponseTemplate(surveyId);

            if (responseTemplate == null)
            {
                return null;
            }

            if (!responseTemplate.ListTemplatePageIds().Contains(pageId))
            {
                return null;
            }

            Security.AuthorizeUserContext(userPrincipal, responseTemplate, "Form.Edit");

            var templatePage = responseTemplate.GetPage(pageId);

            return templatePage == null
                       ? null
                       : new SurveyPageMetaData
                             {
                                 ItemIds = templatePage.ListItemIds().ToArray(),
                                 LayoutTemplateId = templatePage.LayoutTemplateId,
                                 Position = templatePage.Position,
                                 PageType = templatePage.PageType.ToString(),
                                 Id = templatePage.ID.Value
                             };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="customUrl"></param>
        /// <param name="serverApplicationPath"></param>
        /// <returns></returns>
        public static QuestionResult IfAlternateUrlIsAvailable(CheckboxPrincipal userPrincipal, string customUrl, string serverApplicationPath)
        {
            QuestionResult result = new QuestionResult();
            result.Yes = true;

            if (ApplicationManager.AppSettings.AllowSurveyUrlRewriting && Utilities.IsNotNullOrEmpty(customUrl))
            {
                string shortUrl = string.Format("{0}/{1}",
                                                ApplicationManager.ApplicationRoot,
                                                customUrl);

                //See if a survey url is already mapped to this short url
                bool mappingExists = UrlMapper.SourceMappingExists(shortUrl);

                if (mappingExists)
                {
                    result.Yes = false;
                    result.Message = WebTextManager.GetText("/pageText/surveyProperties.aspx/thisCustomUrlIsUsed");
                    return result;
                }

                //Make sure the mapping does not point to an existing file
                if (File.Exists(serverApplicationPath + customUrl))
                {
                    result.Yes = false;
                    result.Message =
                        WebTextManager.GetText("/pageText/surveyProperties.aspx/mappedFileExists");
                    return result;
                }

                //Finally, ensure that there are no special characterss
                //Make sure there are no special characters other than the underscore
                var validator = new AlphaNumericValidator();
                customUrl = customUrl.Substring(0, customUrl.LastIndexOf('.')); //Delete '.aspx' and other endings
                if (!validator.Validate(customUrl.Replace("_", string.Empty).Replace("-", string.Empty)))
                {
                    result.Yes = false;
                    result.Message = WebTextManager.GetText("/pageText/surveyProperties.aspx/invalidName");
                    return result;
                }
            }

            return result;
        }

        /// <summary>
        /// Get metadata associated with a survey
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="surveyId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static IItemMetadata GetSurveyItemMetaData(CheckboxPrincipal userPrincipal, int surveyId, int itemId)
        {
            var responseTemplate = ResponseTemplateManager.GetResponseTemplate(surveyId);

            if (responseTemplate == null)
            {
                return null;
            }

            Security.AuthorizeUserContext(userPrincipal, responseTemplate, "Form.Edit");

            var itemDto = PrepareItemMetaData(itemId, responseTemplate);

            return itemDto;
        }

        /// <summary>
        /// Get metadata associated with a survey
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="surveyId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public static IItemMetadata[] ListPageItemsData(CheckboxPrincipal userPrincipal, int surveyId, int pageId)
        {
            var responseTemplate = ResponseTemplateManager.GetResponseTemplate(surveyId);

            if (responseTemplate == null)
            {
                return null;
            }

            Security.AuthorizeUserContext(userPrincipal, responseTemplate, "Form.Edit");

            var result = new List<IItemMetadata>();

            var page = responseTemplate.GetPage(pageId);
            foreach (int itemId in page.ListItemIds())
            {
                result.Add(PrepareItemMetaData(itemId, responseTemplate));
            }

            return result.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="responseTemplate"></param>
        /// <returns></returns>
        private static IItemMetadata PrepareItemMetaData(int itemId, ResponseTemplate responseTemplate)
        {
            var itemData = responseTemplate.GetItem(itemId);

            var itemDto = itemData.GetDataTransferObject(responseTemplate);

            //If item is in a matrix, get more context inforation.  We can tell this if the item is
            // in the template but not in the direct list of item ids
            if (itemDto is SurveyItemMetaData && !responseTemplate.ListTemplateItemIds().Contains(itemId))
            {
                var childItemData = SurveyMetaDataProxy.GetItemData(itemId, false);

                if (childItemData != null && childItemData.Coordinate != null)
                {
                    ((SurveyItemMetaData)itemDto).RowPosition = childItemData.Coordinate.Y;
                    ((SurveyItemMetaData)itemDto).ColumnPosition = childItemData.Coordinate.X;
                }
            }
            return itemDto;
        }

        /// <summary>
        /// Get column prototypes for the matrix
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="surveyId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static IItemMetadata[] GetColumnPrototypes(CheckboxPrincipal userPrincipal, int surveyId, int itemId)
        {
            var res = new List<IItemMetadata>();

            var responseTemplate = ResponseTemplateManager.GetResponseTemplate(surveyId);

            if (responseTemplate == null)
            {
                return null;
            }


            var itemData = responseTemplate.GetItem(itemId);

            if (itemData == null)
            {
                return null;
            }

            if (itemData is MatrixItemData)
            {
                var mi = itemData as MatrixItemData;
                for (int i = 0; i < mi.ColumnCount; i++)
                {
                    int itemID = mi.GetColumnPrototypeId(i + 1);
                    if (itemID > 0)
                        res.Add(ItemConfigurationManager.GetConfigurationData(itemID).GetDataTransferObject(responseTemplate));
                }
                
            }

            return res.ToArray();
        }


        /// <summary>
        /// Get metadata associated with a survey
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="libraryId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static IItemMetadata GetLibraryItemMetaData(CheckboxPrincipal userPrincipal, int libraryId, int itemId)
        {
            var libraryTemplate = LibraryTemplateManager.GetLibraryTemplate(libraryId);

            if (libraryTemplate == null)
            {
                return null;
            }

            if (!libraryTemplate.ListTemplateItemIds().Contains(itemId))
            {
                return null;
            }

            var itemData = libraryTemplate.GetItem(itemId);

            return itemData == null
                ? null
                : itemData.GetDataTransferObject(libraryTemplate);
        }

        public static void SetItemLibraryOptions(CheckboxPrincipal userPrincipal, int itemId, bool shouldShow)
        {
            LibraryTemplateManager.UpdateItemLibraryOption(itemId, shouldShow);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField">Supports Name or CreatedBy</param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField">Supports Name or CreatedBy</param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public static PagedListResult<StyleListItem[]> ListSurveyStyleTemplates(CheckboxPrincipal userPrincipal, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            var paginationContext = new PaginationContext
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                SortField = sortField,
                SortAscending = sortAscending,
                FilterField = filterField,
                FilterValue = filterValue
            };

            var styleList = StyleTemplateManager.GetPagedStyleData(userPrincipal, paginationContext);

            return new PagedListResult<StyleListItem[]>
            {
                TotalItemCount = paginationContext.ItemCount,
                ResultPage = styleList.Select(CreateStyleListItem).ToArray()
            };
        }


        public static bool AddItemsFromLibrary(CheckboxPrincipal userPrincipal, int pageId, int itemId, int responseTemplateId, int libraryId)
        {
            try {
                var responseTemplate = ResponseTemplateManager.GetResponseTemplate(responseTemplateId);
                var library = LibraryTemplateManager.GetLibraryTemplate(libraryId); 
                var itemData = library.GetItem(itemId);
                ItemData copy = ItemConfigurationManager.CopyItem(itemData, userPrincipal);

                if (copy == null)
                    return false;

                responseTemplate.AddItemToPage(pageId, copy.ID.Value, null);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public static PagedListResult<LibraryData[]> ListItemLibraries(CheckboxPrincipal userPrincipal, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            //check params
            switch (sortField)
            {
                case "LibraryTemplateID":
                case "Name":
                    break;
                default:
                    sortField = "Name";
                    break;
            }

            var paginationContext = new PaginationContext
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                SortField = sortField,
                SortAscending = sortAscending,
                FilterField = filterField,
                FilterValue = filterValue,
                Permissions = new List<string>(new[] { "Library.View" })
            };

            var libraryTemplates = LibraryTemplateManager.GetAvailableLibraryTemplates(userPrincipal, paginationContext);

            return new PagedListResult<LibraryData[]>
            {
                TotalItemCount = paginationContext.ItemCount,
                ResultPage = libraryTemplates.Select(CreateLibraryData).ToArray()
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="libraryId"></param>
        /// <returns></returns>
        public static LibraryData GetLibraryData(CheckboxPrincipal userPrincipal, int libraryId)
        {
            var ltLibrary = LibraryTemplateManager.GetLightweightLibraryTemplate(libraryId);

            Security.AuthorizeUserContext(userPrincipal, ltLibrary, "Library.View");

            return CreateLibraryData(ltLibrary);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lightweightStyleTemplate"></param>
        /// <returns></returns>
        private static StyleListItem CreateStyleListItem(LightweightStyleTemplate lightweightStyleTemplate)
        {
            return new StyleListItem
                       {
                           CreatedBy = lightweightStyleTemplate.CreatedBy,
                           DateCreated = lightweightStyleTemplate.DateCreated,
                           Name = lightweightStyleTemplate.Name,
                           Type = lightweightStyleTemplate.Type.ToString(),
                           Id = lightweightStyleTemplate.TemplateId,
                           IsDefault = ApplicationManager.AppSettings.DefaultStyleTemplate == lightweightStyleTemplate.TemplateId
                       };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lightweightLibraryTemplate"></param>
        /// <returns></returns>
        private static LibraryData CreateLibraryData(LightweightLibraryTemplate lightweightLibraryTemplate)
        {
            return new LibraryData
            {
                Name = lightweightLibraryTemplate.Name,
                CreatedBy = lightweightLibraryTemplate.CreatedBy,
                CreatedDate = lightweightLibraryTemplate.CreatedDate,
                LastModified = lightweightLibraryTemplate.ModifiedDate,
                DatabaseId = lightweightLibraryTemplate.ID,
                ItemIds = GetLibraryTemplateItemIds(lightweightLibraryTemplate.ID),
                Description = lightweightLibraryTemplate.Description,
                ItemAliases = GetLibraryTemplateItemAliases(lightweightLibraryTemplate.ID),
                MenuSettings = GetLibraryItemSettings(lightweightLibraryTemplate.ID)
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="libraryTemplateId"></param>
        /// <returns></returns>
        private static int[] GetLibraryTemplateItemIds(int libraryTemplateId)
        {
            var libraryTemplate = LibraryTemplateManager.GetLibraryTemplate(libraryTemplateId);

            return libraryTemplate != null
                ? libraryTemplate.ListTemplateItemIds()
                : new int[] { };
        }

        private static Dictionary<int, bool> GetLibraryItemSettings(int libraryTemplateId)
        {
            var libraryTemplate = LibraryTemplateManager.GetLibraryTemplate(libraryTemplateId);
            int[] itemIds = libraryTemplate != null ? libraryTemplate.ListTemplateItemIds() : new int[] { };

            return LibraryTemplateManager.GetItemLibraryOptions(itemIds);
        }

        private static Dictionary<int, string> GetLibraryTemplateItemAliases(int libraryTemplateId)
        {
            var libraryTemplate = LibraryTemplateManager.GetLibraryTemplate(libraryTemplateId);
            int[] itemIds = libraryTemplate != null ? libraryTemplate.ListTemplateItemIds() : new int[] { };
            Dictionary<int, string> itemAliases = new Dictionary<int, string>();

            foreach(var itemId in itemIds)
            {
                ItemData itemData = libraryTemplate.GetItem(itemId, true);
                itemAliases.Add(itemId, string.IsNullOrWhiteSpace(itemData.Alias) ? itemData.ItemTypeName : itemData.Alias );
            }
            return itemAliases;
        }


        /// <summary>
        /// Delete the specific folder
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="folderId"></param>
        /// <returns>Returns value, indicates if the folder was deleted or not</returns>
        public static bool DeleteFolder(CheckboxPrincipal userPrincipal, int folderId)
        {
            Security.AuthorizeUserContext(userPrincipal, "FormFolder.FullControl");
            FormFolder folder = FolderManager.GetFolder(folderId);
            if (folder != null)
            {
                try
                {
                    folder.Delete(userPrincipal);
                    return true;
                }
                catch (AuthorizationException)
                {
                    return false;
                }

            }

            return false;
        }

        /// <summary>
        /// Delete the specific survey
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="surveyId"></param>
        /// <returns>Returns value, indicates if the survey was deleted or not</returns>
        public static bool DeleteSurvey(CheckboxPrincipal userPrincipal, int surveyId)
        {
            Security.AuthorizeUserContext(userPrincipal, "Form.Delete");
            if (ResponseTemplateManager.CanBeDeleted(surveyId, userPrincipal))
            {
                ResponseTemplateManager.DeleteResponseTemplate(surveyId);
                return true;
            }
            return false;
        }


        /// <summary>
        /// Copy the specific survey page. The new page will follow after the source page.
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="surveyId"></param>
        /// <param name="pageId"></param>
        public static void CopySurveyPage(CheckboxPrincipal userPrincipal, int surveyId, int pageId)
        {
            ResponseTemplate responseTemplate = ResponseTemplateManager.GetResponseTemplate(surveyId);

            if (responseTemplate == null)
            {
                throw new Exception("Unable to load template with [" + surveyId + "]");
            }

            Security.AuthorizeUserContext(userPrincipal, responseTemplate, "Form.Edit");

            TemplatePage page = responseTemplate.GetPage(pageId);
            int newPageId = responseTemplate.AddPageToTemplate(page.Position + 1, true);
            foreach (int itemId in page.ListItemIds())
            {
                ItemData item = responseTemplate.GetItem(itemId, true);
                ItemData copy = ItemConfigurationManager.CopyItem(item, userPrincipal);

                if (copy.ID.HasValue)
                {
                    responseTemplate.AddItemToPage(newPageId, copy.ID.Value, null);
                }
                else
                {
                    throw new Exception("Error creating copy of item.");
                }
            }

            responseTemplate.ModifiedBy = userPrincipal.Identity.Name;
            responseTemplate.Save();
            ResponseTemplateManager.MarkTemplateUpdated(responseTemplate.ID.Value);
        }

        /// <summary>
        /// Delete the specific survey page.
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="surveyId"></param>
        /// <param name="pageId"></param>
        public static void DeleteSurveyPage(CheckboxPrincipal userPrincipal, int surveyId, int pageId)
        {
            ResponseTemplate responseTemplate = ResponseTemplateManager.GetResponseTemplate(surveyId);

            if (responseTemplate == null)
            {
                throw new Exception("Unable to load template with [" + surveyId + "]");
            }

            Security.AuthorizeUserContext(userPrincipal, responseTemplate, "Form.Edit");

            TemplatePage page = responseTemplate.GetPage(pageId);

            if (page.PageType == TemplatePageType.HiddenItems || page.PageType == TemplatePageType.Completion)
            {
                throw new Exception("Completion page and 'Hidden items' page cannot be deleted.");
            }

            responseTemplate.DeletePage(pageId);
            responseTemplate.ModifiedBy = userPrincipal.Identity.Name;
            responseTemplate.Save();
            ResponseTemplateManager.MarkTemplateUpdated(responseTemplate.ID.Value);
        }


        /// <summary>
        /// Delete the specific survey item 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="surveyId"></param>
        /// <param name="itemId"></param>
        public static void DeleteSurveyItem(CheckboxPrincipal userPrincipal, int surveyId, int itemId)
        {
            ResponseTemplate responseTemplate = ResponseTemplateManager.GetResponseTemplate(surveyId);

            if (responseTemplate == null)
            {
                throw new Exception("Unable to load template with [" + surveyId + "]");
            }

            Security.AuthorizeUserContext(userPrincipal, responseTemplate, "Form.Edit");

            int? pagePosition = responseTemplate.GetPagePositionForItem(itemId);

            if (!pagePosition.HasValue)
            {
                throw new Exception(String.Format("The item (ID={0}) isn't contained in the survey (ID={1})", itemId, surveyId));
            }

            responseTemplate.DeleteItemFromPage(responseTemplate.GetPageAtPosition(pagePosition.Value).ID.Value, itemId);
            responseTemplate.ModifiedBy = userPrincipal.Identity.Name;
            responseTemplate.Save();

            ResponseTemplateManager.MarkTemplateUpdated(surveyId);
        }

        /// <summary>
        /// Add response pipe to survey.
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="surveyId"></param>
        /// <param name="itemId"></param>
        /// <param name="pipeValue"></param>
        /// <returns></returns>
        public static void AddResponsePipeToSurvey(CheckboxPrincipal userPrincipal, int surveyId, int itemId, string pipeValue)
        {
            ResponseTemplate responseTemplate = ResponseTemplateManager.GetResponseTemplate(surveyId);
            responseTemplate.AddResponsePipe(pipeValue, itemId);

            responseTemplate.ModifiedBy = userPrincipal.Identity.Name;
            responseTemplate.Save();
            ResponseTemplateManager.MarkTemplateUpdated(surveyId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="surveyId"></param>
        /// <param name="pageId"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public static RuleMetaData GetPageCondition(CheckboxPrincipal userPrincipal, int surveyId, int pageId, string languageCode)
        {
            ResponseTemplate responseTemplate = ResponseTemplateManager.GetResponseTemplate(surveyId);

            if (responseTemplate == null)
            {
                throw new Exception("Unable to load template with [" + surveyId + "]");
            }

            Security.AuthorizeUserContext(userPrincipal, responseTemplate, "Form.Edit");

            var rds = responseTemplate.CreateRuleDataService();

            var pageRuleData = rds.GetConditionForPage(pageId);

            return CreateRuleMetaData(pageRuleData, languageCode);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="surveyId"></param>
        /// <param name="itemId"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public static RuleMetaData GetItemCondition(CheckboxPrincipal userPrincipal, int surveyId, int itemId, string languageCode)
        {
            ResponseTemplate responseTemplate = ResponseTemplateManager.GetResponseTemplate(surveyId);

            if (responseTemplate == null)
            {
                throw new Exception("Unable to load template with [" + surveyId + "]");
            }

            Security.AuthorizeUserContext(userPrincipal, responseTemplate, "Form.Edit");

            var rds = responseTemplate.CreateRuleDataService();

            var itemRuleData = rds.GetConditionForItem(itemId);

            return itemRuleData != null
                       ? CreateRuleMetaData(itemRuleData, languageCode)
                       : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="surveyId"></param>
        /// <param name="pageId"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public static RuleMetaData[] GetPageBranches(CheckboxPrincipal userPrincipal, int surveyId, int pageId, string languageCode)
        {
            ResponseTemplate responseTemplate = ResponseTemplateManager.GetResponseTemplate(surveyId);

            if (responseTemplate == null)
            {
                throw new Exception("Unable to load template with [" + surveyId + "]");
            }

            Security.AuthorizeUserContext(userPrincipal, responseTemplate, "Form.Edit");

            var rds = responseTemplate.CreateRuleDataService();

            var pageBranchRules = rds.GetBranchRulesForPage(pageId);

            return pageBranchRules.Select(pageBranchRule => CreateRuleMetaData(pageBranchRule, languageCode)).ToArray();
        }

        /// <summary>
        /// Get page logic
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="surveyId"></param>
        /// <param name="pageId"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public static PageLogic GetPageLogic(CheckboxPrincipal userPrincipal, int surveyId, int pageId, string languageCode)
        {
            ResponseTemplate responseTemplate = ResponseTemplateManager.GetResponseTemplate(surveyId);

            if (responseTemplate == null)
            {
                throw new Exception("Unable to load template with [" + surveyId + "]");
            }

            Security.AuthorizeUserContext(userPrincipal, responseTemplate, "Form.Edit");

            var rds = responseTemplate.CreateRuleDataService();
            PageLogic pageLogic = new PageLogic();
            
            pageLogic.BranchesCount = rds.GetBranchRulesForPage(pageId).Count();
            pageLogic.ConditionsCount = (rds.GetConditionForPage(pageId) == null) ? 0 : rds.GetConditionForPage(pageId).TotalConditionsCount;
            pageLogic.PageId = pageId;
            return pageLogic;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public static GroupedResult<SurveyListItem>[] Search(CheckboxPrincipal userPrincipal, string searchTerm)
        {
            if (!RoleManager.UserHasRoleWithPermission(userPrincipal.Identity.Name, "FormFolder.Read") 
                && RoleManager.UserHasRoleWithPermission(userPrincipal.Identity.Name, "Form.Fill"))
            {
                return SearchForRespondent(userPrincipal, searchTerm);
            }

            Security.AuthorizeUserContext(userPrincipal, "FormFolder.Read");

            var paginationContext = new PaginationContext
            {
                CurrentPage = -1,
                PageSize = -1,
                Permissions = new List<string> { "FormFolder.Read", "Form.Edit" },
                PermissionJoin = Checkbox.Security.PermissionJoin.Any,
                SortField = "ItemName",
                SortAscending = true,
                FilterValue = searchTerm
            };

            //Perform a number of searches based on the search term

            //Surveys/folders matching name
            paginationContext.FilterField = "Name";
            var itemsByName = ResponseTemplateManager.ListAccessibleTemplatesAndFolders(userPrincipal, paginationContext, true, true);

            //List surveys/folders created by user matching name
            paginationContext.FilterField = "CreatedBy";
            var itemsByOwner = ResponseTemplateManager.ListAccessibleTemplatesAndFolders(userPrincipal, paginationContext, true, true);

            //List surveys/folders created matching ID
            bool isNumeric = Utilities.AsInt(searchTerm).HasValue;

            var itemsById = new List<LightweightAccessControllable>();

            if (isNumeric)
            {
                paginationContext.FilterField = "Id";
                itemsById = ResponseTemplateManager.ListAccessibleTemplatesAndFolders(userPrincipal, paginationContext, true, true);
            }

            //List surveys/folders created matching GUID
            paginationContext.FilterField = "Guid";
            var itemsByGuid = ResponseTemplateManager.ListAccessibleTemplatesAndFolders(userPrincipal, paginationContext, true, true);

            //Return value
            var resultList = new List<GroupedResult<SurveyListItem>>();

            resultList.Add(new GroupedResult<SurveyListItem>
            {
                GroupKey = "matchingName",
                GroupResults = itemsByName.Select(r => ToSurveyListItem(userPrincipal, r, false, true, true)).ToArray()
            });

            resultList.Add(new GroupedResult<SurveyListItem>
            {
                GroupKey = "matchingOwner",
                GroupResults = itemsByOwner.Select(r => ToSurveyListItem(userPrincipal, r, false, true, true)).ToArray()

            });

            resultList.Add(new GroupedResult<SurveyListItem>
            {
                GroupKey = "matchingId",
                GroupResults = itemsById.Select(r => ToSurveyListItem(userPrincipal, r, false, true, true)).ToArray()

            });

            resultList.Add(new GroupedResult<SurveyListItem>
            {
                GroupKey = "matchingGuid",
                GroupResults = itemsByGuid.Select(r => ToSurveyListItem(userPrincipal, r, false, true, true)).ToArray()

            });


            return resultList.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        private static GroupedResult<SurveyListItem>[] SearchForRespondent(CheckboxPrincipal userPrincipal, string searchTerm)
        {
            Security.AuthorizeUserContext(userPrincipal, "Form.Fill");

            var availableSurveys = ListAvailableSurveys(userPrincipal, 1, int.MaxValue, "TemplateName", true, "Name", searchTerm);

            var resultTemplates = availableSurveys.ResultPage.Select(t => new SurveyListItem
                                                                            {
                                                                                Name = t.Name,
                                                                                Type = "ResponseTemplate",
                                                                                ID = t.ID,
                                                                                SurveyGuid = t.SurveyGuid
                                                                            }).ToArray();

            //Return value
            return new[]
            {
                new GroupedResult<SurveyListItem>
                    {
                        GroupKey = "matchingName",
                        GroupResults = resultTemplates
                    }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static SimpleNameValueCollection GetExpressionLeftParamByType(CheckboxPrincipal userPrincipal, int responseTemplateId, int dependentItemId, string leftParamType, string ruleType, int? maxSourceQuestionPagePosition, string languageCode)
        {
            ResponseTemplate responseTemplate = ResponseTemplateManager.GetResponseTemplate(responseTemplateId);
            
            if (responseTemplate == null)
            {
                throw new Exception("Unable to load template with [" + responseTemplateId + "]");
            }

            Security.AuthorizeUserContext(userPrincipal, responseTemplate, "Form.Edit");

            var res = new Dictionary<string, string>();

            var type = (ExpressionSourceType)Enum.Parse(typeof(ExpressionSourceType), leftParamType);
            
            res.Add("NotSelected", "Source");

            switch (type)
            {
                case ExpressionSourceType.Question:
                    PopulateQuestionsForExpression(res, responseTemplateId, dependentItemId, maxSourceQuestionPagePosition, languageCode);
                    break;
                case ExpressionSourceType.ResponseProperty:
                {
                    RuleType rule;
                    if (Enum.TryParse(ruleType, true, out rule))
                        PopulateResponsePropertiesForExpression(res, maxSourceQuestionPagePosition, rule);
                    else
                        PopulateResponsePropertiesForExpression(res, maxSourceQuestionPagePosition, null);

                    break;
                }
                case ExpressionSourceType.UserAttribute:
                    PopulateUserAttributesForExpression(res);
                    break;
                case ExpressionSourceType.CategorizedType:
                    PopulateCategorizedQuestionsForExpression(res, responseTemplateId, maxSourceQuestionPagePosition, languageCode);
                    break;
            }

            return new SimpleNameValueCollection(res);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="leftParamType"></param>
        /// <param name="leftParam"></param>
        /// <param name="maxSourceQuestionPagePosition"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public static SimpleNameValueCollection GetExpressionOperators(CheckboxPrincipal userPrincipal, int responseTemplateId, string leftParamType, string leftParam, int? maxSourceQuestionPagePosition, string languageCode)
        {
            ResponseTemplate responseTemplate = ResponseTemplateManager.GetResponseTemplate(responseTemplateId);

            if (responseTemplate == null)
            {
                throw new Exception("Unable to load template with [" + responseTemplateId + "]");
            }

            Security.AuthorizeUserContext(userPrincipal, responseTemplate, "Form.Edit");

            var res = new Dictionary<string, string>();

            var type = (ExpressionSourceType)Enum.Parse(typeof(ExpressionSourceType), leftParamType);


            // check if current drop down is user attribute type and field is matrix type
            CustomFieldType propertyType = 0;

            if (type == ExpressionSourceType.UserAttribute)
            {
                var profileProperties = ProfileManager.GetProfileProperties(userPrincipal.Identity.Name, false, false,
                    userPrincipal.UserGuid);

                if (profileProperties != null && profileProperties.Any())
                {
                    var property = profileProperties.FirstOrDefault(item => item.Name.Equals(leftParam));
                    if (property != null && property.FieldType == CustomFieldType.Matrix)
                        propertyType = CustomFieldType.Matrix;
                }
            }

            res.Add("OPERATOR", WebTextManager.GetText("/pageText/EditExpression.aspx/comparison"));

            if (propertyType != CustomFieldType.Matrix)
            {
               
                res.Add("Equal", WebTextManager.GetText("/enum/logicalOperator/equal"));
                res.Add("NotEqual", WebTextManager.GetText("/enum/logicalOperator/notEqual"));
                res.Add("GreaterThan", WebTextManager.GetText("/enum/logicalOperator/greaterThan"));
                res.Add("GreaterThanEqual", WebTextManager.GetText("/enum/logicalOperator/greaterThanEqual"));
                res.Add("LessThan", WebTextManager.GetText("/enum/logicalOperator/lessThan"));
                res.Add("LessThanEqual", WebTextManager.GetText("/enum/logicalOperator/lessThanEqual"));

                if (type != ExpressionSourceType.CategorizedType)
                {
                    res.Add("Contains", WebTextManager.GetText("/enum/logicalOperator/contains"));
                    res.Add("DoesNotContain", WebTextManager.GetText("/enum/logicalOperator/doesNotContain"));

                    if (type == ExpressionSourceType.Question)
                    {
                        res.Add("Answered", WebTextManager.GetText("/enum/logicalOperator/answered"));
                        res.Add("NotAnswered", WebTextManager.GetText("/enum/logicalOperator/notAnswered"));
                    }
                }
            }
            if (type != ExpressionSourceType.Question)
            {
                res.Add("IsNull", WebTextManager.GetText("/enum/logicalOperator/isNull"));
                res.Add("IsNotNull", WebTextManager.GetText("/enum/logicalOperator/isNotNull"));
            }

            return new SimpleNameValueCollection(res);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="leftParamType"></param>
        /// <param name="leftParam"></param>
        /// <param name="selectedOperator"></param>
        /// <param name="maxSourceQuestionPagePosition"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public static ExpressionRightParamData GetExpressionRightParams(CheckboxPrincipal userPrincipal, int responseTemplateId, string leftParamType, string leftParam, string selectedOperator, int? maxSourceQuestionPagePosition, string languageCode)
        {
            ResponseTemplate responseTemplate = ResponseTemplateManager.GetResponseTemplate(responseTemplateId);

            if (responseTemplate == null)
            {
                throw new Exception("Unable to load template with [" + responseTemplateId + "]");
            }

            Security.AuthorizeUserContext(userPrincipal, responseTemplate, "Form.Edit");

            var res = new ExpressionRightParamData();

            var expressionLeftParamType = (ExpressionSourceType)Enum.Parse(typeof(ExpressionSourceType), leftParamType);
            var expressionOperator = (LogicalOperator)Enum.Parse(typeof(LogicalOperator), selectedOperator);

            if (expressionOperator == LogicalOperator.Answered || expressionOperator == LogicalOperator.NotAnswered ||
                expressionOperator == LogicalOperator.IsNotNull || expressionOperator == LogicalOperator.IsNull)
            {
                return res;
            }

            if (expressionLeftParamType == ExpressionSourceType.Question)
            {
                int sourceItemId = 0;
                try
                {
                    sourceItemId = int.Parse(leftParam);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Cannot parse item ID [{0}]", leftParam), ex);
                }

                //check for select
                LightweightItemMetaData sourceItemData = SurveyMetaDataProxy.GetItemData(sourceItemId, false);

                bool isRatingScale = sourceItemData != null && "RadioButtonScale".Equals(sourceItemData.ItemType, StringComparison.InvariantCultureIgnoreCase);
                bool isSelectItem = sourceItemData != null && sourceItemData.Options.Count > 0;
                RadioButtonField radioButtonField = null;

                if ("RadioButtons".Equals(sourceItemData.ItemType, StringComparison.InvariantCultureIgnoreCase))
                {
                    var name = ProfileManager.GetConnectedProfileFieldName(sourceItemId);

                    if (!string.IsNullOrEmpty(name))
                    {
                        radioButtonField = ProfileManager.GetRadioButtonField(name, userPrincipal.UserGuid);
                        isSelectItem = true;
                    }
                }

                if (sourceItemData != null)
                {
                    var options = new Dictionary<string, string>();

                    if (isSelectItem && ((expressionOperator == LogicalOperator.Equal
                            || expressionOperator == LogicalOperator.NotEqual) ||
                            (isRatingScale && ((expressionOperator == LogicalOperator.GreaterThan
                            || expressionOperator == LogicalOperator.GreaterThanEqual || expressionOperator == LogicalOperator.LessThan || expressionOperator == LogicalOperator.LessThanEqual))
                            )
                        ))
                    {
                        options.Add("VALUE", "Value");

                        res.Type = "Select";

                        if (radioButtonField == null)
                        {
                            foreach (int optionId in sourceItemData.Options)
                            {
                                options.Add(optionId.ToString(), Utilities.DecodeAndStripHtml(SurveyMetaDataProxy.GetOptionText(sourceItemData.ItemId, optionId, languageCode, false, false), 64));
                            }
                        }
                        else
                        {
                            foreach (var option in radioButtonField.Options)
                            {
                                options.Add(option.Id.ToString(), Utilities.DecodeAndStripHtml(option.Name, 64));
                            }
                        }

                        res.Options = new SimpleNameValueCollection(options);
                        return res;
                    }
                }
            }

            if (expressionLeftParamType == ExpressionSourceType.ResponseProperty)
            {
                if (leftParam.Equals("CurrentDateROTW", StringComparison.InvariantCultureIgnoreCase) ||
                    leftParam.Equals("CurrentDateUS", StringComparison.InvariantCultureIgnoreCase) ||
                    leftParam.Equals("Started", StringComparison.InvariantCultureIgnoreCase) ||
                    leftParam.Equals("Ended", StringComparison.InvariantCultureIgnoreCase) ||
                    leftParam.Equals("LastEdit", StringComparison.InvariantCultureIgnoreCase))
                {
                    res.Type = "Date";
                    return res;
                }
            }

            res.Type = "Text";

            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="expressionId"></param>
        /// <param name="maxSourceQuestionPagePosition"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public static ExpressionRightParamData GetExistingExpressionRightParams(CheckboxPrincipal userPrincipal, int responseTemplateId, int expressionId, int? maxSourceQuestionPagePosition, string languageCode)
        {
            ResponseTemplate responseTemplate = ResponseTemplateManager.GetResponseTemplate(responseTemplateId);

            if (responseTemplate == null)
            {
                throw new Exception("Unable to load template with [" + responseTemplateId + "]");
            }

            Security.AuthorizeUserContext(userPrincipal, responseTemplate, "Form.Edit");

            var res = new ExpressionRightParamData();

            var rds = responseTemplate.CreateRuleDataService();
            var exp = rds.GetExpression(expressionId);

            var expression = rds.GetExpression(expressionId);
            var expressionData = rds.GetExpressionData(expressionId);
            var expressionMetaData = CreateExpressionMetaData(expressionData, languageCode);

            var leftParamType = expressionMetaData.SourceType;
            var selectedOperator = expressionData.Operator.ToString();
            var leftParam = expression.LeftOperand.ItemId.HasValue ? expression.LeftOperand.ItemId.ToString() :
                (expression.LeftOperand.ProfileKey == null ? expression.LeftOperand.ResponseKey : expression.LeftOperand.ProfileKey);

            return GetExpressionRightParams(userPrincipal, responseTemplateId, leftParamType, leftParam, selectedOperator, maxSourceQuestionPagePosition, languageCode);        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="rootExpressionId"></param>
        /// <param name="expressionId"></param>
        /// <param name="leftParamType"></param>
        /// <param name="leftParam"></param>
        /// <param name="selectedOperator"></param>
        /// <param name="data"></param>
        /// <param name="maxSourceQuestionPagePosition"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public static ExpressionMetaData AddExpression(CheckboxPrincipal userPrincipal, string ruleType, int dependentItemId, int dependentPageId, int targetPageId, int responseTemplateId, int rootExpressionId, int expressionId, string leftParamType, string leftParam, string selectedOperator, string data, int maxSourceQuestionPagePosition, string languageCode)
        {
            ResponseTemplate responseTemplate = ResponseTemplateManager.GetResponseTemplate(responseTemplateId);

            if (responseTemplate == null)
            {
                throw new Exception("Unable to load template with [" + responseTemplateId + "]");
            }

            Security.AuthorizeUserContext(userPrincipal, responseTemplate, "Form.Edit");

            var rds = responseTemplate.CreateRuleDataService();

            if (expressionId > 0)
            {
                var expr = rds.GetExpression(expressionId);
                if (expr == null || expr.Root == null)
                {
                    //invalid parent expression id (the expression was deleted)
                    expressionId = -1; 
                    rootExpressionId = -1; 
                }
            }

            if (expressionId <= 0)
            {
                //build rule if does not exists
                if (rootExpressionId == 0)
                {
                    RuleType type = (RuleType)Enum.Parse(typeof(RuleType), ruleType);

                    rootExpressionId = GetRootExpressionID(dependentItemId, dependentPageId, targetPageId, rds, type);
                    if (rootExpressionId < 0)
                    {
                        //rds = responseTemplate.CreateRuleDataService();
                        rootExpressionId = GetRootExpressionID(dependentItemId, dependentPageId, targetPageId, rds, type);
                    }
                }

                //create an expression if does not exists
                expressionId = rds.CreateANDCompositeExpression(rootExpressionId);
            }

            var expressionLeftParamType = (ExpressionSourceType)Enum.Parse(typeof(ExpressionSourceType), leftParamType);
            var expressionOperator = (LogicalOperator)Enum.Parse(typeof(LogicalOperator), selectedOperator);

            int? newExpressionId = null;
            switch (expressionLeftParamType)
            {
                case ExpressionSourceType.Question :
                    {
                        int sourceItemId = 0;
                        try
                        {
                            sourceItemId = int.Parse(leftParam);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(string.Format("Cannot parse item ID [{0}]", leftParam), ex);
                        }

                        //check for select
                        LightweightItemMetaData sourceItemData = SurveyMetaDataProxy.GetItemData(sourceItemId, false);

                        bool isRatingScale = sourceItemData != null && "RadioButtonScale".Equals(sourceItemData.ItemType, StringComparison.InvariantCultureIgnoreCase);
                        bool isSelectItem = sourceItemData != null && sourceItemData.Options.Count > 0;

                        if (sourceItemData.Options.Count > 0 && ((expressionOperator == LogicalOperator.Equal
                                                    || expressionOperator == LogicalOperator.NotEqual) ||
                                                    (isRatingScale && ((expressionOperator == LogicalOperator.GreaterThan
                                                    || expressionOperator == LogicalOperator.GreaterThanEqual || expressionOperator == LogicalOperator.LessThan || expressionOperator == LogicalOperator.LessThanEqual))
                                                    )
                                                ))
                        {
                            int? optionId = int.Parse(data.Trim());
                            newExpressionId = rds.CreateItemExpression(expressionId, sourceItemId, expressionOperator, optionId, null);
                        }
                        else
                        {
                            newExpressionId = rds.CreateItemExpression(expressionId, sourceItemId, expressionOperator, null, data);
                        }
                        rds.SaveRuleData();
                    }
                    break;
                case ExpressionSourceType.UserAttribute:
                    {
                        newExpressionId = rds.CreateProfileExpression(expressionId, leftParam, expressionOperator, data);
                        rds.SaveRuleData();
                    }
                    break;
                case ExpressionSourceType.ResponseProperty:
                    {
                        newExpressionId = rds.CreateResponseExpression(expressionId, leftParam, expressionOperator, data);
                        rds.SaveRuleData();
                    }
                    break;
                case ExpressionSourceType.CategorizedType:
                    {
                        throw new Exception("Not implemented yet");
                    }                  
            }

            if (newExpressionId.HasValue)
            {
                int? newRealExpressionId = rds.GetRealExpressionId(newExpressionId.Value);

                if (newRealExpressionId.HasValue)
                {
                    return new ExpressionMetaData() { ExpressionId = newRealExpressionId.Value };
                }                
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="rootExpressionId"></param>
        /// <param name="expressionId"></param>
        /// <param name="leftParamType"></param>
        /// <param name="leftParam"></param>
        /// <param name="selectedOperator"></param>
        /// <param name="data"></param>
        /// <param name="maxSourceQuestionPagePosition"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public static ExpressionMetaData EditExpression(CheckboxPrincipal userPrincipal, string ruleType, int dependentItemId, int dependentPageId, int targetPageId, int responseTemplateId, int rootExpressionId, int expressionId, string leftParamType, string leftParam, string selectedOperator, string data, int maxSourceQuestionPagePosition, string languageCode)
        {
            ResponseTemplate responseTemplate = ResponseTemplateManager.GetResponseTemplate(responseTemplateId);

            if (responseTemplate == null)
            {
                throw new Exception("Unable to load template with [" + responseTemplateId + "]");
            }

            Security.AuthorizeUserContext(userPrincipal, responseTemplate, "Form.Edit");

            var rds = responseTemplate.CreateRuleDataService();

            if (expressionId <= 0)
            {
                throw new Exception("Unknown expression ID");
            }
            if (expressionId <= 0)
            {
                throw new Exception("Unknown root expression ID");
            }

            int oldExpressionID = expressionId;

            int? parentExpressionID = rds.GetExpressionParentID(expressionId);
            if (parentExpressionID == null)
            {
                throw new Exception("Unknown expression ID");
            }

            //Get parent expression ID
            expressionId = parentExpressionID.Value;

            int? result = null;

            if (leftParamType == null)
            {
                var expression = rds.GetExpression(oldExpressionID);

                int optionID = 0;
                if (int.TryParse(data, out optionID) && expression.RightOperand.OptionId.HasValue)
                {
                    expression.RightOperand.OptionId = optionID;
                }
                if (expression.RightOperand.AnswerValue != null)
                    expression.RightOperand.AnswerValue = data;
                if (expression.RightOperand.ProfileKey != null)
                    expression.RightOperand.ProfileKey = data;
                if (expression.RightOperand.ResponseKey != null)
                    expression.RightOperand.ResponseKey = data;
                expression.Update();

                rds.SaveRuleData();

                result = expression.Id;
            }
            else
            {
                var expressionLeftParamType = (ExpressionSourceType)Enum.Parse(typeof(ExpressionSourceType), leftParamType);
                var expressionOperator = (LogicalOperator)Enum.Parse(typeof(LogicalOperator), selectedOperator);

                int? newExpressionId = null;
                switch (expressionLeftParamType)
                {
                    case ExpressionSourceType.Question:
                        {
                            int sourceItemId = 0;
                            try
                            {
                                sourceItemId = int.Parse(leftParam);
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(string.Format("Cannot parse item ID [{0}]", leftParam), ex);
                            }

                            //check for select
                            LightweightItemMetaData sourceItemData = SurveyMetaDataProxy.GetItemData(sourceItemId, false);

                            bool isRatingScale = sourceItemData != null && "RadioButtonScale".Equals(sourceItemData.ItemType, StringComparison.InvariantCultureIgnoreCase);
                            bool isSelectItem = sourceItemData != null && sourceItemData.Options.Count > 0;

                            if (sourceItemData.Options.Count > 0 && ((expressionOperator == LogicalOperator.Equal
                                                        || expressionOperator == LogicalOperator.NotEqual) ||
                                                        (isRatingScale && ((expressionOperator == LogicalOperator.GreaterThan
                                                        || expressionOperator == LogicalOperator.GreaterThanEqual || expressionOperator == LogicalOperator.LessThan || expressionOperator == LogicalOperator.LessThanEqual))
                                                        )
                                                    ))
                            {
                                int? optionId = int.Parse(data.Trim());
                                newExpressionId = rds.CreateItemExpression(expressionId, sourceItemId, expressionOperator, optionId, null);
                            }
                            else
                            {
                                newExpressionId = rds.CreateItemExpression(expressionId, sourceItemId, expressionOperator, null, data);
                            }
                            rds.SaveRuleData();
                        }
                        break;
                    case ExpressionSourceType.UserAttribute:
                        {
                            newExpressionId = rds.CreateProfileExpression(expressionId, leftParam, expressionOperator, data);
                            rds.SaveRuleData();
                        }
                        break;
                    case ExpressionSourceType.ResponseProperty:
                        {
                            newExpressionId = rds.CreateResponseExpression(expressionId, leftParam, expressionOperator, data);
                            rds.SaveRuleData();
                        }
                        break;
                    case ExpressionSourceType.CategorizedType:
                        {
                            throw new Exception("Not implemented yet");
                        }
                }

                rds.DeleteExpression(oldExpressionID);

                if (newExpressionId.HasValue)
                    result = newExpressionId.Value;
            }
            
            rds.SaveRuleData();

            if (result.HasValue)
                return new ExpressionMetaData() { ExpressionId = result.Value };

            return null;
        }

        /// <summary>
        /// Change OR connectors to AND or vice versa
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="ruleType"></param>
        /// <param name="dependentItemId"></param>
        /// <param name="dependentPageId"></param>
        /// <param name="targetPageId"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="rootExpressionId"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public static string ReorganizeExpressions(CheckboxPrincipal userPrincipal, string ruleType, int dependentItemId, int dependentPageId, int targetPageId, int responseTemplateId, int rootExpressionId, string connector)
        {
            ResponseTemplate responseTemplate = ResponseTemplateManager.GetResponseTemplate(responseTemplateId);

            if (responseTemplate == null)
            {
                throw new Exception("Unable to load template with [" + responseTemplateId + "]");
            }

            Security.AuthorizeUserContext(userPrincipal, responseTemplate, "Form.Edit");

            var rds = responseTemplate.CreateRuleDataService();

            var ruleData = GetRuleData(dependentItemId, dependentPageId, targetPageId, rds, (RuleType)Enum.Parse(typeof(RuleType), ruleType));

            //Each rule has a top-level expression, which  contains child OR expressions
            var ruleExpression = ruleData.Expression as CompositeExpressionData;

            if (ruleExpression == null)
            {
                throw new Exception("Unable to convert conditions.");
            }

            //Top-Level or expressions
            List<ExpressionData> orExpressions = ruleExpression.Children;

            if (orExpressions.Count == 0)
            {
                return connector;
            }

            //Get the first or expression
            var firstOr = orExpressions[0] as CompositeExpressionData;

            //Do nothing if no expressions found yet
            if (firstOr == null)
            {
                return connector;
            }

            //Make all expressions top-level OR expressions, or make all expressions AND expressions
            if (connector.Equals("OR", StringComparison.InvariantCultureIgnoreCase))
            {
                //All and expressions will have their own or expressions
                List<ExpressionData> childExpressions = firstOr.Children;
                var expressionsToMove = new List<ExpressionData>();

                //Leave first child in place, and move others
                for (int i = 1; i < childExpressions.Count; i++)
                {
                    expressionsToMove.Add(childExpressions[i]);
                }

                foreach (ExpressionData expressionToMove in expressionsToMove)
                {
                    var newOr = new CompositeExpressionData(new List<ExpressionData>(), LogicalConnector.AND);

                    //Add the new child to the rule expression
                    ruleExpression.Add(newOr);

                    //Save
                    rds.UpdateExpressionData(newOr);

                    firstOr.MoveChild(expressionToMove, newOr);

                    rds.UpdateExpressionData(expressionToMove);

                }
                rds.UpdateExpressionData(firstOr);

                rds.UpdateExpressionData(ruleExpression);
            }
            else
            {
                //The first OR will be the parent of all and expressions
                var expressionsToDelete = new List<ExpressionData>();

                //Move the children of the second and following or expressions to the first or    
                for (int i = 1; i < orExpressions.Count; i++)
                {
                    var composite = orExpressions[i] as CompositeExpressionData;

                    if (composite != null)
                    {
                        var andExpressions = new List<ExpressionData>(composite.Children);

                        foreach (ExpressionData andExpression in andExpressions)
                        {
                            composite.MoveChild(andExpression, firstOr);

                            //Persist change to data set
                            rds.UpdateExpressionData(andExpression);

                            //Delete the superfluous or
                            expressionsToDelete.Add(composite);
                        }
                    }
                }

                foreach (ExpressionData expressionToDelete in expressionsToDelete)
                {
                    ruleExpression.RemoveChild(expressionToDelete);
                }

                rds.UpdateExpressionData(firstOr);

                rds.UpdateExpressionData(ruleExpression);
            }

            //Save rule data
            rds.SaveRuleData();

            return connector;
        }


        /// <summary>
        /// Set target page for the given rule
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="ruleId"></param>
        /// <param name="targetPageId"></param>
        /// <returns></returns>
        public static void SetPageBranchTargetPage(CheckboxPrincipal userPrincipal, int responseTemplateId, int ruleId, int targetPageId)
        {
            ResponseTemplate responseTemplate = ResponseTemplateManager.GetResponseTemplate(responseTemplateId);

            if (responseTemplate == null)
            {
                throw new Exception("Unable to load template with [" + responseTemplateId + "]");
            }

            Security.AuthorizeUserContext(userPrincipal, responseTemplate, "Form.Edit");

            var rds = responseTemplate.CreateRuleDataService();

            rds.SetPageBranchTargetPageId(ruleId, targetPageId);

            rds.SaveRuleData();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependentItemId"></param>
        /// <param name="dependentPageId"></param>
        /// <param name="targetPageId"></param>
        /// <param name="rds"></param>
        /// <param name="type"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        private static int GetRootExpressionID(int dependentItemId, int dependentPageId, int targetPageId, RuleDataService rds, RuleType type)
        {
            RuleData rule = GetRuleData(dependentItemId, dependentPageId, targetPageId, rds, type);

            return rule.Expression.ExpressionId.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependentItemId"></param>
        /// <param name="dependentPageId"></param>
        /// <param name="targetPageId"></param>
        /// <param name="rds"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static RuleData GetRuleData(int dependentItemId, int dependentPageId, int targetPageId, RuleDataService rds, RuleType type)
        {
            RuleData rule = null;
            switch (type)
            {
                case RuleType.ItemCondition:
                    rule = rds.GetConditionForItem(dependentItemId) ?? rds.CreateEmptyConditionRuleForItem(dependentItemId);
                    break;
                case RuleType.PageCondition:
                    rule = rds.GetConditionForPage(dependentPageId) ?? rds.CreateEmptyConditionRuleForPage(dependentPageId);
                    break;
                case RuleType.PageBranchCondition:
                    var rules = rds.GetBranchRulesForPage(dependentPageId);
                    rule = rules.FirstOrDefault(r => (r.Actions[0] is BranchPageActionData) && (r.Actions[0] as BranchPageActionData).GoToPageID == targetPageId);
                    if (rule == null)
                    {
                        rule = rds.CreateBranchRule(dependentPageId, targetPageId);
                    }
                    break;
            }
            rds.SaveRuleData();

            return rule;
        }

        /*
        ///<summary>
        /// Create a new rule and return the id of it's root expression
        /// </summary>
        /// <returns></returns>
        private static int CreateNewRule()
        {
            RuleData ruleData;

            if (Params.RuleType == RuleType.PageBranchCondition)
            {
                ruleData = RuleDataService.CreateBranchRule(Params.DependentPageId.Value, TargetPageId.Value);
            }
            else
            {
                ruleData = Params.DependentItemId.HasValue
                    ? RuleDataService.CreateEmptyConditionRuleForItem(Params.DependentItemId.Value)
                    : RuleDataService.CreateEmptyConditionRuleForPage(Params.DependentPageId.Value);
            }

            if (ruleData != null
                && ruleData.Expression != null
                && ruleData.Expression.ExpressionId.HasValue)
            {
                return ruleData.Expression.ExpressionId.Value;
            }

            throw new Exception("Unable to create new rule.");
        }

        /// <summary>
        /// Get the root expression id, creating it if necessary
        /// </summary>
        /// <returns></returns>
        protected static int GetRootExpressionID()
        {
            //Get the root expression id, creating it if necessary
            if (Params.RuleType == RuleType.PageBranchCondition)
            {
                //Find the rule
                foreach (RuleData ruleData in RuleData)
                {
                    if (ruleData.Expression != null
                        && ruleData.Expression.ExpressionId.HasValue
                        && Params.RuleDataService.GetTargetPageId(ruleData.RuleId) == TargetPageId)
                    {
                        return ruleData.Expression.ExpressionId.Value;
                    }
                }
            }
            else if (ConditionRuleData != null
                     && ConditionRuleData.Expression != null
                     && ConditionRuleData.Expression.ExpressionId.HasValue)
            {
                return ConditionRuleData.Expression.ExpressionId.Value;
            }

            return CreateNewRule();
        }
        */


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="expressionId"></param>
        /// <returns></returns>
        public static int[] RemoveExpression(CheckboxPrincipal userPrincipal, int responseTemplateId, int expressionId)
        {
            ResponseTemplate responseTemplate = ResponseTemplateManager.GetResponseTemplate(responseTemplateId);
                        
            if (responseTemplate == null)
            {
                throw new Exception("Unable to load template with [" + responseTemplateId + "]");
            }

            Security.AuthorizeUserContext(userPrincipal, responseTemplate, "Form.Edit");

            var rds = responseTemplate.CreateRuleDataService(false);
            rds.DeleteExpression(expressionId);
            var deletedRules = rds.DeletedRules;
            rds.SaveRuleData();

            return deletedRules;
        }

        #region Utility

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="ruleData"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        private static RuleMetaData CreateRuleMetaData(RuleData ruleData, string languageCode)
        {
            var ruleMetaData = new RuleMetaData();
            if (ruleData != null)
            {
                ruleMetaData.RuleId = ruleData.RuleId;

                var data = ruleData.Expression as CompositeExpressionData;
                if (data != null)
                {
                    ruleMetaData.RootExpressionId = data.ExpressionId.HasValue ? data.ExpressionId.Value : 0;
                    //Children, if any
                    ruleMetaData.OrExpressions =
                        data.Children.Select(
                            childExpressionData => CreateExpressionMetaData(childExpressionData, languageCode)).ToArray();
                }


                ruleMetaData.TotalConditionsCount = ruleData.TotalConditionsCount;
            }
            else
            {
                //if rulemeta data was null, then there are no conditions, so set the total condition count to 0
                ruleMetaData.TotalConditionsCount = 0;
            }
            return ruleMetaData;
        }

        /// <summary>
        /// Lists all user properties
        /// </summary>
        /// <param name="res"></param>
        private static void PopulateUserAttributesForExpression(Dictionary<string, string> res)
        {
            var pipeNames = new List<string>(PipeManager.GetProfilePipeNames(ProfileManager.ListPropertyNames()));
            pipeNames.Sort(StringComparer.InvariantCultureIgnoreCase);

            foreach (string propertyName in pipeNames)
            {
                res.Add(Utilities.StripHtml(propertyName, 64), propertyName);
            }
        }

        /// <summary>
        /// Lists all response properties
        /// </summary>
        /// <param name="maxPagePosition"></param>
        /// <param name="res"></param>
        /// <param name="ruleType"></param>
        private static void PopulateResponsePropertiesForExpression(Dictionary<string, string> res, int? maxPagePosition, RuleType? ruleType)
        {
            var propertyNames = new List<string>(ResponseProperties.PropertyNames);
            propertyNames.Sort(StringComparer.InvariantCultureIgnoreCase);

            foreach (string propertyName in propertyNames)
            {
                //skip total possible score as it's not related to user properties/answers
                if (propertyName.ToLower() == "totalpossiblescore")
                    continue;

                res.Add(propertyName, Utilities.StripHtml(WebTextManager.GetText(string.Format("/responseProperty/{0}/text", propertyName)), 64));

                //add score by pages
                if (maxPagePosition.HasValue && propertyName.ToLower() == "currentscore")
                {
                    foreach (var pair in ResponseProperties.GetPageScoreProperties(maxPagePosition.Value - (ruleType == RuleType.PageCondition ? 0 : 1), false))
                    {
                        res.Add(pair.Key, pair.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Lists all questions
        /// </summary>
        /// <param name="res"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="dependentItemId"></param>
        /// <param name="maxSourceQuestionPagePosition"></param>
        /// <param name="languageCode"></param>
        private static void PopulateQuestionsForExpression(Dictionary<string, string> res, int responseTemplateId, int dependentItemId, int? maxSourceQuestionPagePosition, string languageCode)
        {
            var surveyItems = ItemConfigurationManager.ListBasicItemsData(responseTemplateId, maxSourceQuestionPagePosition, languageCode);

            //get matrix data, if dependent item is row element
            int? dependentParentId = null;
            int? dependentRow = null;
            var dependentItem = ItemConfigurationManager.GetConfigurationData(dependentItemId);
            if (dependentItem != null && dependentItem.ParentItemId.HasValue)
            {
                dependentParentId = dependentItem.ParentItemId;
                var parent = ItemConfigurationManager.GetConfigurationData(dependentItem.ParentItemId.Value) as TabularItemData;

                if (parent != null)
                {
                    var coord = parent.GetItemCoordinate(dependentItemId);
                    if (coord != null)
                        dependentRow = coord.Row;
                }
            }
            
            foreach (var itemData in surveyItems)
            {
                if (itemData.ItemId != dependentItemId && itemData.ParentId != dependentItemId && 
                        (!dependentRow.HasValue || dependentParentId.Value != itemData.ParentId || dependentRow.Value != itemData.RowNumber))
                {
                    var text = Utilities.DecodeAndStripHtml(itemData.ItemText, 64);
                    if (string.IsNullOrEmpty(text))
                        text = "Item: " + itemData.ItemId;

                    res.Add(itemData.ItemId.ToString(), text);
                }
            }
        }


        /// <summary>
        /// Lists all categorized questions
        /// </summary>
        /// <param name="res"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="maxSourceQuestionPagePosition"></param>
        /// <param name="languageCode"></param>
        private static void PopulateCategorizedQuestionsForExpression(Dictionary<string, string> res, int responseTemplateId, int? maxSourceQuestionPagePosition, string languageCode)
        {
            List<LightweightItemMetaData> itemList = ItemConfigurationManager.ListResponseTemplateCategorizedItems(
                responseTemplateId,
                maxSourceQuestionPagePosition,
                languageCode);

            foreach (LightweightItemMetaData itemData in itemList)
            {
                String text = itemData.GetText(false, languageCode);
                if (String.IsNullOrEmpty(text))
                    text = "Item: " + itemData.ItemId;
                res.Add(itemData.ItemId.ToString(), Utilities.StripHtml(itemData.PositionText + " " + text, 64));
            }
        }


        /// <summary>
        /// Create expression data
        /// </summary>
        /// <param name="expressionData"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        private static ExpressionMetaData CreateExpressionMetaData(ExpressionData expressionData, string languageCode)
        {
            ExpressionSourceType sourceType = ExpressionSourceType.SourceTypeNotSpecified;

            if (expressionData.Left is CategorizedMatrixItemOperandData)
            {
                sourceType = ExpressionSourceType.CategorizedType;
            }
            else if (expressionData.Left is MatrixItemOperandData)
            {
                sourceType = ExpressionSourceType.Question;
            }
            else if (expressionData.Left is ItemOperandData)
            {
                sourceType = ExpressionSourceType.Question;
            }
            else if (expressionData.Left is ResponseOperandData)
            {
                sourceType = ExpressionSourceType.ResponseProperty;
            }
            else if (expressionData.Left is ProfileOperandData)
            {
                sourceType = ExpressionSourceType.UserAttribute;
            }
            //Base properties

            string rightExpressionText = null;

            if (expressionData.Left != null && expressionData.Left is ItemOperandData)
            {
                var bindProfileName = ProfileManager.GetConnectedProfileFieldName(((ItemOperandData)expressionData.Left).ItemId.Value);
                if (!string.IsNullOrEmpty(bindProfileName) && expressionData.Right != null && expressionData.Right is StringOperandData)
                {
                    var radioButtonField = ProfileManager.GetRadioButtonField(bindProfileName, ((CheckboxPrincipal)HttpContext.Current.User).UserGuid);
                    var optionId = int.Parse(((StringOperandData)expressionData.Right).Value);

                    rightExpressionText = radioButtonField.Options.First(s => s.Id == optionId)?.Name;
                }
            }
            
            var expressionMetaData = new ExpressionMetaData
            {
                ExpressionId = expressionData.ExpressionId.Value,
                LeftOperandText = expressionData.LeftOperandText(languageCode),
                LeftOperandType = expressionData.Left == null ? string.Empty : expressionData.Left.OperandTypeName,
                SourceType = sourceType.ToString(),
                RightOperandText = rightExpressionText != null ? rightExpressionText : expressionData.RightOperandText(languageCode),
                RightOperandID = expressionData.Right == null ? null : expressionData.Right.OperandId,
                RightOperandType = expressionData.Right == null ? string.Empty : expressionData.Right.OperandTypeName,
                Operator = expressionData.OperatorText
            };

            if (expressionData is CompositeExpressionData
                && ((CompositeExpressionData)expressionData).Children.Count > 0)
            {
                //Children, if any
                expressionMetaData.AndExpressions =
                    ((CompositeExpressionData)expressionData).Children.Select(
                        childExpressionData => CreateExpressionMetaData(childExpressionData, languageCode)).ToArray();
            }

            return expressionMetaData;
        }

        /// <summary>
        /// Create survey metadata object from response template
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="rt"></param>
        /// <returns></returns>
        private static SurveyMetaData CreateSurveyMetaData(ExtendedPrincipal userPrincipal, ResponseTemplate rt)
        {
            var data = new SurveyMetaData();

            if (rt != null)
            {
                data.Name = rt.Name;
                data.ActivationEndDate = WebUtilities.ConvertToClientTimeZone(rt.BehaviorSettings.ActivationEndDate);
                data.ActivationStartDate = WebUtilities.ConvertToClientTimeZone(rt.BehaviorSettings.ActivationStartDate);
                data.AllowEdit = rt.BehaviorSettings.AllowEdit;
                data.AllowResume = rt.BehaviorSettings.AllowContinue;
                data.AllowSurveyEditWhileActive = rt.BehaviorSettings.AllowSurveyEditWhileActive;
                data.DisplayPDFDownloadButton = rt.BehaviorSettings.DisplayPDFDownloadButton;
                data.CreatedBy = rt.CreatedBy;
                data.DefaultLanguage = rt.LanguageSettings.DefaultLanguage;
                Dictionary<string, string> allLanguages = WebTextManager.GetSurveyLanguagesDictionary();
                //this line caused an exception "The given key was not present in the dictionary" in case when the imported survey had a language, not registered in this Checkbox instance
                //data.SupportedLanguages = new SimpleNameValueCollection(rt.LanguageSettings.SupportedLanguages.ToDictionary(l => l, l => allLanguages[l]));
                data.SupportedLanguages = new SimpleNameValueCollection(rt.LanguageSettings.SupportedLanguages.Where(l => allLanguages.ContainsKey(l)).ToDictionary(l => l, l => allLanguages[l]));
                data.LanguageSource = rt.LanguageSettings.LanguageSource;
                data.LanguageSourceToken = rt.LanguageSettings.LanguageSourceToken;
                data.CreatedDate = WebUtilities.ConvertToClientTimeZone(rt.CreatedDate);
                data.Id = rt.ID.Value;
                data.DisableBackButton = rt.BehaviorSettings.DisableBackButton;
                data.EnableScoring = rt.BehaviorSettings.EnableScoring;
                data.AllowFormReset = rt.BehaviorSettings.AllowFormReset;
                data.Guid = rt.GUID;
                data.IsActive = rt.BehaviorSettings.IsActive;
                data.LastModified = WebUtilities.ConvertToClientTimeZone(rt.LastModified);
                data.MaxResponsesPerUser = rt.BehaviorSettings.MaxResponsesPerUser;
                data.MaxTotalResponses = rt.BehaviorSettings.MaxTotalResponses;
                data.RandomizeItems = rt.BehaviorSettings.RandomizeItemsInPages;
                data.ShowAsterisks = rt.StyleSettings.ShowAsterisks;
                data.HideFooterHeader = rt.StyleSettings.HideFooterHeader;
                data.ShowInputErrorAlert = rt.StyleSettings.ShowValidationErrorAlert;
                data.ShowItemNumbers = rt.StyleSettings.ShowItemNumbers;
                data.ShowPageNumbers = rt.StyleSettings.ShowPageNumbers;
                data.ShowTopSurveyButtons = rt.StyleSettings.ShowTopSurveyButtons;
                data.HideTopSurveyButtonsOnFirstAndLastPage = rt.StyleSettings.HideTopSurveyButtonsOnFirstAndLastPage;
                data.ShowProgressBar = rt.StyleSettings.ShowProgressBar;
                data.ProgressBarOrientation = (int)rt.StyleSettings.ProgressBarOrientation;
                data.ShowSaveAndQuit = rt.BehaviorSettings.ShowSaveAndQuit;
                data.ShowTitle = rt.StyleSettings.ShowTitle;
                data.StyleTemplateID = rt.StyleSettings.StyleTemplateId;
                data.MobileStyleID = rt.StyleSettings.MobileStyleId;
                data.TabletStyleTemplateId = rt.StyleSettings.TabletStyleTemplateId;
                data.SmartPhoneStyleTemplateId = rt.StyleSettings.SmartPhoneStyleTemplateId;
                data.UseDynamicItemNumbers = rt.StyleSettings.EnableDynamicItemNumbers;
                data.UseDynamicPageNumbers = rt.StyleSettings.EnableDynamicPageNumbers;
                data.SecurityType = rt.BehaviorSettings.SecurityType.ToString();
                data.AnonymizeResponses = rt.BehaviorSettings.AnonymizeResponses;
                data.Password = rt.BehaviorSettings.Password ?? string.Empty;
                data.SheduledInvitations = Messaging.Email.EmailGateway.ProviderSupportsBatches;
                data.ShowInvitations = ApplicationManager.AppSettings.EmailEnabled;
                data.ShowFacebook = !string.IsNullOrEmpty(ApplicationManager.AppSettings.FacebookAppID);
                string notAvailableReason;
                data.IsAvailable = rt.BehaviorSettings.GetIsActiveOnDate(DateTime.Now, out notAvailableReason);
                data.NotAvailableCode = notAvailableReason;
                data.FormEditPermission = AuthorizationFactory.GetAuthorizationProvider().Authorize(userPrincipal, rt, "Form.Edit");
                data.GoogleAnalyticsTrackingID = rt.BehaviorSettings.GoogleAnalyticsTrackingID;

                //Page and item ids
                data.PageIds = rt.ListTemplatePageIds().ToArray();
                data.ItemIds = rt.ListTemplateItemIds().ToArray();
                data.PageBreaks = rt.TemplatePages.ToDictionary(d => d.Value.ID, d => d.Value.ShouldForceBreak);

                //Terms
                data.Terms = rt.SurveyTerms.Select(s => new TermsMetaData {
                    Id = s.Id,
                    Definition = s.Definition,
                    Link = s.Link,
                    Name = s.Name,
                    Term = s.Term
                }).ToList();

                //Survey urls
                var surveyUrls = new List<string>();

                //Add default url
                var urlWithGuid = "/Survey.aspx?s=" + rt.GUID.ToString().Replace("-", string.Empty);
                surveyUrls.Add(ApplicationManager.ApplicationPath + urlWithGuid);
                //Add this URL only when AllowResponseTemplateIDLookup is true
                //if (ApplicationManager.AppSettings.AllowResponseTemplateIDLookup)
                //    surveyUrls.Add(ApplicationManager.ApplicationPath + "/Survey.aspx?surveyId=" + rt.ID);

                //Custom URL
                if (ApplicationManager.AppSettings.AllowSurveyUrlRewriting)
                {
                    var customUrl = UrlMapper.GetSource(ApplicationManager.ApplicationRoot + urlWithGuid);
                    if (!string.IsNullOrEmpty(customUrl))
                    {
                        surveyUrls.Add(ApplicationManager.ApplicationURL + customUrl);
                    }
                }

                data.SurveyUrls = surveyUrls.ToArray();

                //Social Media Urls
                if (surveyUrls.Count() == 1)
                {
                    string text = string.Format("{0} {1}", WebTextManager.GetText("/twitterButton/tweetText"), rt.Name);
                    data.TwitterUrl = "https://twitter.com/intent/tweet?url=" +
                                      Utilities.AdvancedHtmlEncode(surveyUrls[0]) +
                                      "&text=" + Utilities.AdvancedHtmlEncode(text);
                    data.FacebookUrl = Utilities.AdvancedHtmlEncode("http://www.facebook.com/sharer.php?s=100&p[url]=" +
                                                                    surveyUrls[0]);
                    data.LinkedInUrl =
                        Utilities.AdvancedHtmlEncode("http://www.linkedin.com/shareArticle?mini=true&url=" +
                                                     surveyUrls[0]);
                    data.GplusUrl =
                        Utilities.AdvancedHtmlEncode("https://plus.google.com/share?url=" +
                                                     surveyUrls[0]);
                }
                else
                {
                    string text = string.Format("{0} {1}", WebTextManager.GetText("/twitterButton/tweetText"), rt.Name);
                    data.TwitterUrl = "https://twitter.com/intent/tweet?url=" +
                                      Utilities.AdvancedHtmlEncode(surveyUrls[1]) +
                                      "&text=" + Utilities.AdvancedHtmlEncode(text);
                    data.FacebookUrl = Utilities.AdvancedHtmlEncode("http://www.facebook.com/sharer.php?s=100&p[url]=" +
                                                                    surveyUrls[1]);
                    data.LinkedInUrl =
                        Utilities.AdvancedHtmlEncode("http://www.linkedin.com/shareArticle?mini=true&url=" +
                                                     surveyUrls[1]);
                    data.GplusUrl =
                        Utilities.AdvancedHtmlEncode("https://plus.google.com/share?url=" +
                                                     surveyUrls[1]);
                }


            }

            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessControllables"></param>
        /// <param name="includeSurveyResponseCount"></param>
        /// <param name="includeActive"></param>
        /// <param name="includeInactive"></param>
        /// <returns></returns>
        private static List<SurveyListItem> ToSurveyListItemList(CheckboxPrincipal userPrincipal, IEnumerable<LightweightAccessControllable> accessControllables, bool includeSurveyResponseCount, bool includeActive, bool includeInactive)
        {
            return accessControllables.Select(r => ToSurveyListItem(userPrincipal, r, includeSurveyResponseCount, includeActive, includeInactive)).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessControllable"></param>
        /// <param name="includeResponseCount"></param>
        /// <param name="includeActive"></param>
        /// <param name="includeInactive"></param>
        /// <returns></returns>
        private static SurveyListItem ToSurveyListItem(CheckboxPrincipal userPrincipal, LightweightAccessControllable accessControllable, bool includeResponseCount, bool includeActive, bool includeInactive)
        {
            var surveyListItem = new SurveyListItem
            {
                ID = accessControllable.ID,
                Name = accessControllable.Name,
                Type = accessControllable.EntityType,
                Creator = accessControllable.Owner,
            };

            if (accessControllable is LightweightResponseTemplate)
            {
                surveyListItem.Creator = ((LightweightResponseTemplate)accessControllable).CreatedBy;
                surveyListItem.LastModified = WebUtilities.ConvertToClientTimeZone(((LightweightResponseTemplate)accessControllable).ModifiedDate);
                surveyListItem.IsActive = ((LightweightResponseTemplate)accessControllable).IsActive;
                surveyListItem.CompletedResponseCount = includeResponseCount ? ResponseTemplateManager.GetTemplateResponseCount(accessControllable.ID, false, true) : -1;
                surveyListItem.SurveyGuid = ((LightweightResponseTemplate)accessControllable).GUID;
            }
            else
            {
                surveyListItem.ChildrenCount =
                    ListSurveysAndFolders(userPrincipal, surveyListItem.ID, -1, -1, "Name", string.Empty, -1, string.Empty, includeResponseCount, includeActive, includeInactive).Length;
            }

            return surveyListItem;
        }
        #endregion

        /// <summary>
        /// Delete specified libraries
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="libraryIds"></param>
        public static bool DeleteLibraries(CheckboxPrincipal userPrincipal, int[] libraryIds)
        {
            Security.AuthorizeUserContext(userPrincipal, "Library.Delete");

            foreach (var id in libraryIds)
            {
                LibraryTemplateManager.DeleteLibraryTemplate(userPrincipal, id);
            }

            return true;
        }

        /// <summary>
        /// Move servey item
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="itemId"></param>
        /// <param name="newPageId"></param>
        /// <param name="position"></param>
        public static void MoveSurveyItem(CheckboxPrincipal userPrincipal, int responseTemplateId, int itemId, int? newPageId, int position)
        {
            var responseTemplate = ResponseTemplateManager.GetResponseTemplate(responseTemplateId);

            if (responseTemplate == null)
            {
                throw new Exception("Unable to load template [" + responseTemplateId + "] to move/copy item");
            }

            Security.AuthorizeUserContext(userPrincipal, responseTemplate, "Form.Edit");

            //Find survey item in page
            var oldPagePosition = responseTemplate.GetPagePositionForItem(itemId);

            TemplatePage oldPage = null;
            TemplatePage newPage = null;

            if (oldPagePosition.HasValue)
            {
                oldPage = responseTemplate.GetPageAtPosition(oldPagePosition.Value);
            }

            var item = responseTemplate.GetItem(itemId);

            if (newPageId.HasValue)
                newPage = responseTemplate.GetPage(newPageId.Value);

            if (oldPage != null
                && oldPage.ID.HasValue
                && newPage != null
                && item != null
                && ((item.ItemIsIAnswerable && newPage.PageType != TemplatePageType.Completion) || !item.ItemIsIAnswerable)
                && (item.ItemTypeName != "HiddenItem" || newPage.PageType == TemplatePageType.HiddenItems))
            {
                responseTemplate.MoveItemToPage(itemId, oldPage.ID.Value, newPageId.Value, position);
                responseTemplate.ModifiedBy = userPrincipal.Identity.Name;
                responseTemplate.Save();
                ResponseTemplateManager.MarkTemplateUpdated(responseTemplateId);
                //Remove metadata from cache to refresh cache
                SurveyMetaDataProxy.RemoveItemFromCache(itemId);
            }
        }

        /// <summary>
        /// Move servey item
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="pageId"></param>
        /// <param name="position"></param>
        public static void MoveSurveyPage(CheckboxPrincipal userPrincipal, int responseTemplateId, int pageId, int position)
        {
            var responseTemplate = ResponseTemplateManager.GetResponseTemplate(responseTemplateId);

            Security.AuthorizeUserContext(userPrincipal, responseTemplate, "Form.Edit");

            responseTemplate.MovePage(pageId, position);
            responseTemplate.ModifiedBy = userPrincipal.Identity.Name;
            responseTemplate.Save();

            ResponseTemplateManager.MarkTemplateUpdated(responseTemplateId);
        }

        /// <summary>
        /// Add new page
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="responseTemplateId"></param>
        public static void AddSurveyPage(CheckboxPrincipal userPrincipal, int responseTemplateId)
        {
            var responseTemplate = ResponseTemplateManager.GetResponseTemplate(responseTemplateId);

            Security.AuthorizeUserContext(userPrincipal, responseTemplate, "Form.Edit");

            responseTemplate.AddPageToTemplate(null, true);
            responseTemplate.ModifiedBy = userPrincipal.Identity.Name;
            responseTemplate.Save();

            ResponseTemplateManager.MarkTemplateUpdated(responseTemplateId);
        }

        /// <summary>
        /// Inserts a page break after given page
        /// </summary>
        /// <param name="pageId">Page id after which page break should be inserted</param>
        /// <param name="templateId">template id that this pageId belongs to in order to mark template as updated</param>
        public static void AddPageBreak(int pageId, bool shouldPageBreak, int templateId)
        {
            ResponseTemplateManager.InsertPageBreak(pageId, shouldPageBreak, templateId);
        }

        private static PaginationContext CreatePaginationContext(string filterField = "", string filterValue = "",
            string sortField = "", bool sortAscending = true, int pageNumber = -1, int pageSize = -1)
        {
            //check params
            switch (sortField)
            {
                case "ResponseTemplateId":
                case "CreatedBy":
                case "TemplateName":
                    break;
                default:
                    sortField = "TemplateName";
                    break;
            }
            return new PaginationContext
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                SortField = sortField,
                SortAscending = sortAscending,
                FilterField = filterField,
                FilterValue = Utilities.ProtectFilterFieldFromSQLInjections(filterValue)
            };
        }
    }
}
