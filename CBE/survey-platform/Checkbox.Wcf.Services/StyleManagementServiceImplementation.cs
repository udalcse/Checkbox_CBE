using System;
using System.Collections.Generic;
using System.Linq;
using Checkbox.Management;
using Checkbox.Pagination;
using Checkbox.Security.Principal;
using Checkbox.Styles;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web;

namespace Checkbox.Wcf.Services
{
    public static class StyleManagementServiceImplementation
    {
        public static PagedListResult<StyleListItem[]> ListFormStyles(CheckboxPrincipal userPrincipal, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            //Authorize user
            Security.AuthorizeUserContext(userPrincipal, "Form.Edit");

            var paginationContext = new PaginationContext
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                SortField = sortField,
                SortAscending = sortAscending,
                FilterField = filterField,
                FilterValue = filterValue,
                Permissions = new List<string> { "Form.Edit" },
                PermissionJoin = Checkbox.Security.PermissionJoin.Any
            };

            var styleList = StyleTemplateManager.GetPagedStyleData(userPrincipal, paginationContext);

            return new PagedListResult<StyleListItem[]>(){ResultPage = ToStyleListItemList(styleList).ToArray(), TotalItemCount = paginationContext.ItemCount};
        }

        public static StyleListItem[] ListChartStyles(CheckboxPrincipal userPrincipal, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            //Authorize user
            Security.AuthorizeUserContext(userPrincipal, "Form.Edit");

            var paginationContext = new PaginationContext
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                SortField = sortField,
                SortAscending = sortAscending,
                Permissions = new List<string> { "Form.Edit" },
                PermissionJoin = Checkbox.Security.PermissionJoin.Any
            };

            var styleList = ChartStyleManager.GetPagedStyleData(userPrincipal, true, paginationContext);

            return ToStyleListItemList(styleList).ToArray();
        }

        public static StyleListItem GetStyleListItem(CheckboxPrincipal userPrincipal, int styleId, string type)
        {
            //Authorize user
            Security.AuthorizeUserContext(userPrincipal, "Form.Edit");

            var style = type.Equals("form",StringComparison.InvariantCultureIgnoreCase)
                                                 ? StyleTemplateManager.GetLightweightStyleTemplate(styleId, userPrincipal)
                                                 : ChartStyleManager.GetChartStyle(styleId);

            return ToStyleListItem(style);
        }

        public static bool DeleteFormStyle(CheckboxPrincipal userPrincipal, int styleId)
        {
            //Authorize user
            Security.AuthorizeUserContext(userPrincipal, "Form.Edit");

            try
            {
                StyleTemplateManager.DeleteTemplate(styleId, userPrincipal);
                return true;
            }
            catch
            {
                return false;
            }
        }


        public static bool DeleteFormStyles(CheckboxPrincipal userPrincipal, int[] styleIds)
        {
            //Authorize user
            Security.AuthorizeUserContext(userPrincipal, "Form.Edit");

            try
            {
                foreach (int styleId in styleIds)
                {
                    StyleTemplateManager.DeleteTemplate(styleId, userPrincipal);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


        public static bool DeleteChartStyle(CheckboxPrincipal userPrincipal, int styleId)
        {
            //Authorize user
            Security.AuthorizeUserContext(userPrincipal, "Form.Edit");

            try
            {
                ChartStyleManager.DeleteStyle(styleId);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static object CopyFormStyle(CheckboxPrincipal userPrincipal, int styleId, string languageCode)
        {
            //Authorize user
            Security.AuthorizeUserContext(userPrincipal, "Form.Edit");
            var template = StyleTemplateManager.GetStyleTemplate(styleId);

            return StyleTemplateManager.CopyTemplate(template, languageCode, userPrincipal);
        }

        public static object CopyChartStyle(CheckboxPrincipal userPrincipal, int styleId)
        {
            ////Authorize user
            //var user = Security.AuthorizeUserContext(userPrincipal, "Form.Edit");

            //ChartStyleManager.CreateStyle()
            throw new NotImplementedException();
        }

        private static StyleListItem ToStyleListItem(LightweightStyleTemplate style)
        {            
            return style.AppearanceId.HasValue
                       ? new StyleListItem
                             {
                                 Id = style.TemplateId,
                                 Name = style.Name,
                                 Type = WebTextManager.GetText("/controlText/styles/styletype/" + style.Type.ToString().ToLower()),
                                 CreatedBy = style.CreatedBy,
                                 IsDefault = ApplicationManager.AppSettings.DefaultChartStyle == style.TemplateId,
                                 IsInUse = false,
                                 CanBeEdited = style.CanBeEdited,
                                 DateCreated = style.DateCreated
                             }
                       : new StyleListItem
                             {
                                 Id = style.TemplateId,
                                 Name = style.Name,
                                 Type = WebTextManager.GetText("/controlText/styles/styletype/" + style.Type.ToString().ToLower()),
                                 CreatedBy = style.CreatedBy,
                                 IsDefault = ApplicationManager.AppSettings.DefaultStyleTemplate == style.TemplateId,
                                 IsInUse = StyleTemplateManager.IsStyleInUse(style.TemplateId),
                                 CanBeEdited = style.CanBeEdited,
                                 DateCreated = style.DateCreated
                             };
        }

        private static List<StyleListItem> ToStyleListItemList(IEnumerable<LightweightStyleTemplate> styleList)
        {
            return styleList.Select(ToStyleListItem).ToList();
        }
    }
}
