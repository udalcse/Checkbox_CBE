using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Management;
using Checkbox.Web.Page;
using Prezza.Framework.Caching;

namespace CheckboxWeb.Settings
{
    /// <summary>
    /// 
    /// </summary>
    public class CacheBindingObject
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int Count { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CacheItemBindingObject
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class Cache : SettingsPage
    {
        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            //Bind item created so internal lists can be bound
            _cacheList.ItemCreated += _cacheList_ItemCreated;

            BindList();

            Master.HideDialogButtons();
        }

        /// <summary>
        /// 
        /// </summary>
        private void BindList()
        {
            var cacheBindingObjects =
              CacheFactory
                  .GetCacheManagers()
                  .Select(manager => new CacheBindingObject { Name = manager, Type = "CacheManager", Count = GetCacheCount(manager, "CacheManager") })
                  .ToList();

            cacheBindingObjects.Add(new CacheBindingObject { Name = "Settings", Type = "Static", Count = GetCacheCount("Settings", "Static") });

            _cacheList.DataSource = cacheBindingObjects.OrderBy(bo => bo.Name);
            _cacheList.DataBind();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _cacheList_ItemCreated(object sender, ListViewItemEventArgs e)
        {
            var itemBindingObject = e.Item.DataItem as CacheBindingObject;
            var cacheItemPlace = e.Item.FindControl("_cacheItemPlaceHolder") as PlaceHolder;
            var flushBtn = e.Item.FindControl("_flushButton") as IButtonControl;

            const string evenRow =
                "<div class=\"zebra cacheItem\"><div class=\"cacheKey\">{0}</div><div class=\"cacheValue\">{1}</div><div class=\"clear\"></div></div><div class=\"clear\"></div>";
            var oddRow = evenRow.Replace("zebra", "detailZebra");

            if (itemBindingObject == null || cacheItemPlace == null)
            {
                return;
            }

            if (flushBtn != null)
            {
                flushBtn.CommandArgument = itemBindingObject.Name;
                flushBtn.Click += flushBtn_Click;
            }

            var cacheItemList = GetCacheItems(itemBindingObject);

            bool isOdd = false;

            foreach (var cacheItem in cacheItemList)
            {
                cacheItemPlace.Controls.Add(new LiteralControl(
                                                isOdd
                                                    ? string.Format(oddRow, cacheItem.Key, cacheItem.Value)
                                                    : string.Format(evenRow, cacheItem.Key, cacheItem.Value))
                    );

                isOdd = !isOdd;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void flushBtn_Click(object sender, EventArgs e)
        {
            try
            {
                var button = sender as IButtonControl;

                if (button == null || string.IsNullOrEmpty(button.CommandArgument))
                {
                    return;
                }

                if (button.CommandArgument.Equals("Settings", StringComparison.InvariantCultureIgnoreCase))
                {
                    ApplicationManager.FlushSettingsCache();
                    return;
                }

                var cacheManager = CacheFactory.GetCacheManager(button.CommandArgument);

                if (cacheManager != null)
                {
                    cacheManager.Flush();
                }
            }
            catch
            {

            }
            finally
            {
                BindList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        private int GetCacheCount(string cacheName, string cacheType)
        {
            return GetCacheItems(new CacheBindingObject {Name = cacheName, Type = cacheType}).Count();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheBindingObject"></param>
        /// <returns></returns>
        private IEnumerable<CacheItemBindingObject> GetCacheItems(CacheBindingObject cacheBindingObject)
        {
            //Depending on item type, bind
            if (cacheBindingObject.Type.Equals("CacheManager", StringComparison.InvariantCultureIgnoreCase))
            {
                var cacheManager = CacheFactory.GetCacheManager(cacheBindingObject.Name);

                if (cacheManager == null)
                {
                    return new List<CacheItemBindingObject>();
                }

                var keys = cacheManager.ListKeys();
                cacheBindingObject.Count = keys.Length;

                return keys.Take(50)
                        .Select(
                            key =>
                            new CacheItemBindingObject {Key = key, Value = GetCacheValueHtml(cacheManager[key])});
            }

            //
            if (cacheBindingObject.Name.Equals("Settings", StringComparison.InvariantCultureIgnoreCase))
            {
                return GetSettingsCacheItems();
            }

            return new List<CacheItemBindingObject>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerable<CacheItemBindingObject> GetSettingsCacheItems()
        {

            var appSettingsType = ApplicationManager.AppSettings.GetType();

            //Now get the parameters
            PropertyInfo[] properties = appSettingsType.GetProperties();

            var cacheItems = new List<CacheItemBindingObject>();

            foreach (var propInfo in properties)
            {
                try
                {
                    //Ignore address finder
                    if (propInfo.Name.ToLower().Contains("addressfinder"))
                    {
                        continue;
                    }

                    if (propInfo.Name.ToLower().Contains("connectionstring") || propInfo.Name.ToLower().Contains("password") || propInfo.Name.ToLower().Contains("key"))
                    {
                        cacheItems.Add(new CacheItemBindingObject{Key = propInfo.Name, Value = "[REDACTED]"});
                    }
                    else
                    {
                        cacheItems.Add(new CacheItemBindingObject
                                           {
                                               Key = propInfo.Name,
                                               Value = GetCacheValueHtml(propInfo.GetValue(ApplicationManager.AppSettings, null))
                                           });
                    }
                }
                catch
                {

                }
            }

            return cacheItems;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheItemValue"></param>
        private string GetCacheValueHtml(object cacheItemValue)
        {
            if (cacheItemValue == null)
            {
                return string.Empty;
            }

            if (cacheItemValue is string)
            {
                return Server.HtmlEncode((string) cacheItemValue);
            }

            if (cacheItemValue is IEnumerable)
            {
                var valueStringBuilder = new StringBuilder();

                foreach (var itemValue in ((IEnumerable)cacheItemValue).Cast<object>().Where(itemValue => itemValue != null))
                {
                    valueStringBuilder.Append(Server.HtmlEncode(itemValue.ToString()));
                    valueStringBuilder.AppendLine("<br />");
                }

                return valueStringBuilder.ToString();
            }

            return Server.HtmlEncode(cacheItemValue.ToString());
        }
    }
}